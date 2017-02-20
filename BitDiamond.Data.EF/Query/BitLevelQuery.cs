using BitDiamond.Core.Services.Query;
using System.Collections.Generic;
using System.Linq;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using Axis.Jupiter;
using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Jupiter.Europa;
using System.Data.SqlClient;
using Axis.Luna.Extensions;

namespace BitDiamond.Data.EF.Query
{
    public class BitLevelQuery : IBitLevelQuery
    {
        private IDataContext _europa = null;
        private IReferralQuery _refQuery = null;

        public BitLevelQuery(IDataContext context, IReferralQuery referralQuery)
        {
            ThrowNullArguments(() => context,
                               () => referralQuery);

            _europa = context;
            _refQuery = referralQuery;
        }


        public BitLevel CurrentBitLevel(User user)
        => _europa.Store<BitLevel>()
                  .QueryWith(_bl => _bl.User, _bl => _bl.Donation)
                  .Where(_bl => _bl.User.EntityId == user.EntityId)
                  .OrderByDescending(_bl => _bl.Cycle)
                  .ThenByDescending(_bl => _bl.Level)
                  .FirstOrDefault();

        public IEnumerable<ReferralNode> Downlines(User user)
        => UserRef(user).Pipe(_ur => _refQuery.AllDownlines(_ur));

        public BitcoinAddress GetBitcoinAddress(User user)
        => _europa.Store<BitcoinAddress>()
                  .QueryWith(_bca => _bca.Owner)
                  .Where(_bca => _bca.Owner.EntityId == user.EntityId)
                  .FirstOrDefault();

        public BitLevel GetBitLevelById(long id)
        => _europa.Store<BitLevel>()
                  .QueryWith(_bca => _bca.User)
                  .Where(_bca => _bca.Id == id)
                  .FirstOrDefault();

        public IEnumerable<BitLevel> GetBitLevelHistory(User user)
        => _europa.Store<BitLevel>()
                  .QueryWith(_bca => _bca.User, _bca => _bca.Donation)
                  .Where(_bca => _bca.User.EntityId == user.EntityId)
                  .OrderByDescending(_bca => _bca.CreatedOn)
                  .AsEnumerable();

        public BitLevel GetClosestValidBeneficiary(User user)
        {
            var uplines = Uplines(user);
            var userLevel = CurrentBitLevel(user);
            return uplines.Select(_ul => CurrentBitLevel(_ul.User))
                          .Where(_bl => _bl.Donation.Status == BlockChainTransactionStatus.Valid)
                          .Where(_bl => (_bl.Level > userLevel.Level && _bl.Cycle == userLevel.Cycle) || _bl.Cycle > userLevel.Cycle)
                          .FirstOrDefault();
        }

        public IEnumerable<ReferralNode> Referrals(User user)
        {
            var query =
@"
WITH DownLinesCTE (ReferenceCode)
AS
(
-- Anchor member definition
    SELECT r.ReferenceCode
    FROM dbo.ReferalNode AS r
    WHERE r.UserId = @userId

    UNION ALL

-- Recursive member definition
    SELECT r.ReferenceCode
    FROM DownLinesCTE as pr
    JOIN r FROM dbo.ReferalNode ON r.ReferrerCode = pr.ReferenceCode
)

-- Statement that executes the CTE
SELECT r.ReferenceCode, r.ReferrerCode, r.UplineCode, r.CreatedOn, r.ModifiedOn, r.Id, 
       u.EntityId AS u_EntityId, u.CreatedOn AS u_CreatedOn, u.ModifiedOn AS u_ModifiedOn, u.Status as u_Status, u.UId AS u_UId
FROM dbo.ReferalNode AS r
JOIN dbo.User AS u ON u.EntityId = r.UserId
JOIN DownLinesCTE  AS dl ON dl.ReferenceCode = r.ReferenceCode
";

            using (var connection = new SqlConnection((_europa as EuropaContext).Database.Connection.ConnectionString))
            {
                var qcommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = query,
                    //CommandTimeout = 
                };
                qcommand.Parameters.Add(new SqlParameter("userId", user.UserId));

                using (var row = qcommand.ExecuteReader())
                {
                    var refnodes = new List<ReferralNode>();
                    var userCache = new Dictionary<string, User>();
                    while (row.Read())
                    {
                        refnodes.Add(new ReferralNode
                        {
                            ReferenceCode = row.GetString(0),
                            ReferrerCode = row.GetString(1),
                            UplineCode = row.GetString(2),
                            CreatedOn = row.GetDateTime(3),
                            ModifiedOn = row.GetDateTime(4),
                            Id = row.GetInt64(5),
                            User = userCache.GetOrAdd(row.GetString(6), _uid => new User
                            {
                                EntityId = _uid,
                                CreatedOn = row.GetDateTime(7),
                                ModifiedOn = row.GetDateTime(8),
                                Status = row.GetInt32(9),
                                UId = row.GetGuid(10)
                            })
                        });
                    }
                    return refnodes;
                }
            }
        }

        public ReferralNode Upline(User user, int uplineOffset)
        => Uplines(user).Skip(uplineOffset).FirstOrDefault();

        public IEnumerable<ReferralNode> Uplines(User user)
        => UserRef(user).Pipe(_ur => _refQuery.Uplines(_ur));

        public ReferralNode UserRef(User user)
        => _europa.Store<ReferralNode>()
                  .QueryWith(_r => _r.User)
                  .Where(_r => _r.User.EntityId == user.EntityId)
                  .FirstOrDefault();
    }
}
