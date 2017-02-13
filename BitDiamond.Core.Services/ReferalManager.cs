using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Axis.Luna;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Pollux.RBAC.Services;
using BitDiamond.Core.Services.Query;
using Axis.Luna.Extensions;
using BitDiamond.Core.Services.Command;

namespace BitDiamond.Core.Services
{
    using RANG = RandomAlphaNumericGenerator;

    public class ReferalManager : IReferalManager, IUserContextAware
    {
        private string UniversalLock = "Referal.UniversalLock";

        private IUserAuthorization _authorizer;
        private IReferralQuery _query;
        private IPersistenceCommands _pcommand;

        public IUserContext UserContext { get; private set; }

        public ReferalManager(IUserContext userContext, 
                              IUserAuthorization authorizer,
                              IReferralQuery query,
                              IPersistenceCommands pcommand)
        {
            ThrowNullArguments(() => userContext,
                               () => authorizer,
                               () => query,
                               () => pcommand);

            UserContext = userContext;
            _authorizer = authorizer;
            _query = query;
            _pcommand = pcommand;
        }


        public Operation<ReferalNode> AffixNewUser(string userId, string refereeCode)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            //lock because referal placement is not thread safe!
            lock (UniversalLock)
            {
                var referee = _query.GetReferalNode(refereeCode).ThrowIfNull("invalid referee code");
                var downlines = _query.AllDownlines(referee);
                var user = _query.GetUserById(userId);

                //affix to the referee if he has no downlines
                if (downlines.Count() == 0)
                {
                    var @ref = new ReferalNode
                    {
                        UplineCode = refereeCode,
                        ReferrerCode = refereeCode,
                        ReferenceCode = GenerateReferenceCode(),
                        User = user
                    };

                    return _pcommand.Add(@ref);
                }

                //generate a map for each downline referal node, with its reference code as key
                var hierarchyMap = GenerateHierarchyMap(downlines);

                //calculate and assign levels based on distance from the referee node in the hierarchy
                hierarchyMap.ForAll((cnt, duo) => duo.Value.Level = DistanceFromReferee(referee, duo.Key, hierarchyMap));

                //rearrange and order the downlines, then get the first incomplete duo, and place the new referal in the slot
                return hierarchyMap
                    .Values
                    .OrderBy(_duo => _duo.Level)
                    .ThenBy(_duo => _duo.Count)
                    .FirstOrDefault(_duo => _duo.Count < 2)
                    .Pipe(_duo =>
                    {
                        var @ref = new ReferalNode
                        {
                            UplineCode = _duo.UplineCode,
                            ReferrerCode = refereeCode,
                            ReferenceCode = GenerateReferenceCode(),
                            User = user
                        };

                        return _pcommand.Add(@ref);
                    });
            }
        });

        public Operation<IEnumerable<ReferalNode>> AllDownlines(ReferalNode node)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            return _query.AllDownlines(node);
        });

        public Operation<IEnumerable<ReferalNode>> DirectDownlines(ReferalNode node)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            return _query.DirectDownlines(node);
        });

        public Operation<IEnumerable<ReferalNode>> Uplines(ReferalNode node)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            return _query.Uplines(node);
        });

        private string GenerateReferenceCode()
        {
            var codes = _query.GetAllReferenceCodes();
            string newCode = null;
            do
            {
                newCode = $"{RANG.RandomAlphaNumeric(5)}-{RANG.RandomAlphaNumeric(5)}-{RANG.RandomAlphaNumeric(10)}";
                newCode = newCode.ToUpper()
                                 .Replace("I", "A").Replace("1", "A")
                                 .Replace("O", "B").Replace("0", "B");
            }
            while (codes.Contains(newCode));

            return newCode;
        }

        private Dictionary<string, DownlineDuo> GenerateHierarchyMap(IEnumerable<ReferalNode> nodes)
        {
            var dict = new Dictionary<string, DownlineDuo>();
            nodes.ForAll((cnt, next) =>
            {
                var duo = dict.GetOrAdd(next.UplineCode, _c => new DownlineDuo { UplineCode = _c });
                if (duo.Left == null) duo.Left = next;
                else if (duo.Right == null) duo.Right = next;
                else throw new Exception();

                if(!dict.ContainsKey(next.ReferenceCode))
                    dict[next.ReferenceCode] = new DownlineDuo { UplineCode = next.ReferenceCode };
            });

            return dict;
        }
        
        private int DistanceFromReferee(ReferalNode referee, string referenceCode, Dictionary<string, DownlineDuo> map)
        {
            var count = 1;
            if (referenceCode == referee.ReferenceCode) return count;
            else
            {
                do
                {
                    referenceCode = map.Values
                        .FirstOrDefault(_duo => _duo.ContainsNode(referenceCode))
                        .UplineCode;

                    count++;
                }
                while (referenceCode != referee.ReferenceCode);

                return count;
            }
        }
    }

    public class DownlineDuo
    {
        public string UplineCode { get; set; }

        public int Level { get; set; }

        private ReferalNode[] _nodes = new ReferalNode[2];
        public ReferalNode Left { get  { return _nodes[0]; } set { _nodes[0] = value; } }
        public ReferalNode Right { get { return _nodes[1]; } set { _nodes[1] = value; } }

        public int Count => _nodes.Count(_n => _n != null);

        public bool ContainsNode(string referenceCode)
            => (Left?.ReferenceCode == referenceCode || Right?.ReferenceCode == referenceCode) &&
               referenceCode != null;
    }
}
