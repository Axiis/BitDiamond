using Axis.Jupiter;
using BitDiamond.Core.Services.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;

using static Axis.Luna.Extensions.ExceptionExtensions;
using System.Data.SqlClient;
using Axis.Luna.Extensions;
using System.Configuration;

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
    FROM dbo.ReferralNode AS r
    WHERE r.UplineCode = @reference

    UNION ALL

-- Recursive member definition
    SELECT downline.ReferenceCode
    FROM DownLinesCTE as code
    JOIN dbo.ReferralNode AS downline ON downline.UplineCode = code.ReferenceCode
)

-- Statement that executes the CTE
SELECT r.ReferenceCode, r.ReferrerCode, r.UplineCode, r.CreatedOn, r.ModifiedOn, r.Id, 
       u.EntityId AS u_EntityId, u.CreatedOn AS u_CreatedOn, u.ModifiedOn AS u_ModifiedOn, u.Status as u_Status, u.UId AS u_UId
FROM dbo.ReferralNode AS r
JOIN dbo.[User] AS u ON u.EntityId = r.UserId
JOIN DownLinesCTE  AS dl ON dl.ReferenceCode = r.ReferenceCode
";

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["EuropaContext"].ConnectionString))
            {
                connection.Open();
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
                            ReferenceCode = row.IsDBNull(0) ? null : row.GetString(0),
                            ReferrerCode = row.IsDBNull(1) ? null : row.GetString(1),
                            UplineCode = row.IsDBNull(3) ? null : row.GetString(2),
                            CreatedOn = row.GetDateTime(3),
                            ModifiedOn = row.IsDBNull(4)? (DateTime?)null: row.GetDateTime(4),
                            Id = row.GetInt64(5),
                            User = userCache.GetOrAdd(row.GetString(6), _uid => new User //<-- the UserId of the ReferralNode is also set behind the scenes
                            {
                                EntityId = _uid,
                                CreatedOn = row.GetDateTime(7),
                                ModifiedOn = row.IsDBNull(8) ? (DateTime?)null : row.GetDateTime(8),
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
                  .ToArray();

        public IEnumerable<string> GetAllReferenceCodes()
        => _europa.Store<ReferralNode>().Query.Select(_r => _r.ReferenceCode).ToArray();

        public ReferralNode GetReferalNode(string referenceCode)
        => _europa.Store<ReferralNode>()
                  .QueryWith(_r => _r.User)
                  .Where(_r => _r.ReferenceCode == referenceCode)
                  .FirstOrDefault();

        public User GetUserById(string userId)
        => _europa.Store<User>().Query.FirstOrDefault(_u => _u.EntityId == userId);

        public ReferralNode GetUserReferalNode(User user)
        => _europa.Store<ReferralNode>()
                  .QueryWith(_r => _r.User)
                  .Where(_r => _r.UserId == user.UserId)
                  .FirstOrDefault();

        public IEnumerable<ReferralNode> Uplines(ReferralNode node)
        {
            var query =
@"
WITH UplinesCTE (ReferenceCode)
AS
(
-- Anchor member definition: statrt with the first parent/upline
    SELECT r.UplineCode
    FROM dbo.ReferralNode AS r
    WHERE r.ReferenceCode = @referrerCode

    UNION ALL

-- Recursive member definition
    SELECT node.UplineCode
    FROM dbo.ReferralNode as node
    JOIN UplinesCTE AS code ON code.ReferenceCode = node.ReferenceCode
)

-- Statement that executes the CTE
SELECT r.ReferenceCode, r.ReferrerCode, r.UplineCode, r.CreatedOn, r.ModifiedOn, r.Id, 
       u.EntityId AS u_EntityId, u.CreatedOn AS u_CreatedOn, u.ModifiedOn AS u_ModifiedOn, u.Status as u_Status, u.UId AS u_UId
FROM dbo.ReferralNode AS r
JOIN dbo.[User] AS u ON u.EntityId = r.UserId
JOIN UplinesCTE AS ul ON ul.ReferenceCode = r.ReferenceCode
";
            
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["EuropaContext"].ConnectionString))
            {
                connection.Open();
                var qcommand = new SqlCommand
                {
                    Connection = connection,
                    CommandText = query,
                    //CommandTimeout = 
                };
                qcommand.Parameters.Add(new SqlParameter("referrerCode", node.ReferenceCode));

                using (var row = qcommand.ExecuteReader())
                {
                    var refnodes = new List<ReferralNode>();
                    var userCache = new Dictionary<string, User>();
                    while (row.Read())
                    {
                        refnodes.Add(new ReferralNode
                        {
                            ReferenceCode = row.IsDBNull(0) ? null : row.GetString(0),
                            ReferrerCode = row.IsDBNull(1) ? null : row.GetString(1),
                            UplineCode = row.IsDBNull(2) ? null : row.GetString(2),
                            CreatedOn = row.GetDateTime(3),
                            ModifiedOn = row.IsDBNull(4)? (DateTime?)null: row.GetDateTime(4),
                            Id = row.GetInt64(5),
                            User = userCache.GetOrAdd(row.GetString(6), _uid => new User
                            {
                                EntityId = _uid,
                                CreatedOn = row.GetDateTime(7),
                                ModifiedOn = row.IsDBNull(8) ? (DateTime?)null : row.GetDateTime(8),
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
