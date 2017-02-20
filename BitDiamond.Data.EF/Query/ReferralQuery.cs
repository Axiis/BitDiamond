using Axis.Jupiter;
using BitDiamond.Core.Services.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;

using static Axis.Luna.Extensions.ExceptionExtensions;
using System.Data.SqlClient;
using Axis.Jupiter.Europa;
using Axis.Luna.Extensions;

namespace BitDiamond.Data.EF.Query
{
    public class ReferralQuery: IReferralQuery
    {
        private IDataContext _europa = null;

        public ReferralQuery(IDataContext context)
        {
            ThrowNullArguments(() => context);

            _europa = context;
        }

        public IEnumerable<ReferralNode> AllDownlines(ReferralNode node)
        {
            var query =
@"
WITH DownLinesCTE (ReferenceCode)
AS
(
-- Anchor member definition
    SELECT r.ReferenceCode
    FROM dbo.ReferalNode AS r
    WHERE r.UplineCode = @reference

    UNION ALL

-- Recursive member definition
    SELECT r.ReferenceCode
    FROM DownLinesCTE as pr
    JOIN r FROM dbo.ReferalNode ON r.UplineCode = pr.ReferenceCode
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
                qcommand.Parameters.Add(new SqlParameter("reference", node.ReferenceCode));

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

        public IEnumerable<ReferralNode> DirectDownlines(ReferralNode node)
        => _europa.Store<ReferralNode>()
                  .QueryWith(_r => _r.User)
                  .Where(_r => _r.UplineCode == node.ReferenceCode)
                  .AsEnumerable();

        public IEnumerable<string> GetAllReferenceCodes()
        => _europa.Store<ReferralNode>().Query.Select(_r => _r.ReferenceCode).AsEnumerable();

        public ReferralNode GetReferalNode(string referenceCode)
        => _europa.Store<ReferralNode>()
                  .QueryWith(_r => _r.User)
                  .Where(_r => _r.ReferenceCode == referenceCode)
                  .FirstOrDefault();

        public User GetUserById(string userId)
        => _europa.Store<User>().Query.FirstOrDefault(_u => _u.EntityId == userId);

        public IEnumerable<ReferralNode> Uplines(ReferralNode node)
        {
            var query =
@"
WITH DownLinesCTE (ReferenceCode)
AS
(
-- Anchor member definition: statrt with the first parent/upline
    SELECT r.ReferenceCode
    FROM dbo.ReferalNode AS r
    WHERE r.ReferenceCode = @referrerCode

    UNION ALL

-- Recursive member definition
    SELECT r.ReferenceCode
    FROM DownLinesCTE as r
    JOIN pr FROM dbo.ReferalNode ON pr.ReferenceCode = r.UplineCode
)

-- Statement that executes the CTE
SELECT r.ReferenceCode, r.ReferrerCode, r.UplineCode, r.CreatedOn, r.ModifiedOn, r.Id, 
       u.EntityId AS u_EntityId, u.CreatedOn AS u_CreatedOn, u.ModifiedOn AS u_ModifiedOn, u.Status as u_Status, u.UId AS u_UId
FROM dbo.ReferalNode AS r
JOIN dbo.User AS u ON u.EntityId = r.UserId
JOIN DownLinesCTE AS ul ON ul.ReferenceCode = r.ReferenceCode
";
            
            using (var connection = new SqlConnection((_europa as EuropaContext).Database.Connection.ConnectionString))
            {
                var qcommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = query,
                    //CommandTimeout = 
                };
                qcommand.Parameters.Add(new SqlParameter("referrerCode", node.UplineCode));

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
    }
}
