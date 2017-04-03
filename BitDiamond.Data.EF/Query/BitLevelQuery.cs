using BitDiamond.Core.Services.Query;
using System.Collections.Generic;
using System.Linq;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using Axis.Jupiter;
using static Axis.Luna.Extensions.ExceptionExtensions;
using System.Data.SqlClient;
using Axis.Luna.Extensions;
using System;
using System.Configuration;
using Axis.Luna;
using BitDiamond.Core.Utils;

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

        #region Base Queries
        public IQueryable<BitLevelJoiner> BaseBitLevelQuery()
        => from bl in _europa.Store<BitLevel>().Query
           join btc in _europa.Store<BlockChainTransaction>().Query on bl.DonationId equals btc.Id
           join rad in _europa.Store<BitcoinAddress>().Query on btc.ReceiverId equals rad.Id
           join sad in _europa.Store<BitcoinAddress>().Query on btc.SenderId equals sad.Id
           join rref in _europa.Store<ReferralNode>().Query on rad.OwnerId equals rref.UserId
           join sref in _europa.Store<ReferralNode>().Query on sad.OwnerId equals sref.UserId
           join r in _europa.Store<User>().Query on rad.OwnerId equals r.EntityId
           join s in _europa.Store<User>().Query on sad.OwnerId equals s.EntityId
           join rb in _europa.Store<BioData>().Query on rad.OwnerId equals rb.OwnerId into _rb
           join sb in _europa.Store<BioData>().Query on sad.OwnerId equals sb.OwnerId into _sb
           join rc in _europa.Store<ContactData>().Query on rad.OwnerId equals rc.OwnerId into _rc
           join sc in _europa.Store<ContactData>().Query on sad.OwnerId equals sc.OwnerId into _sc
           join rpi in _europa.Store<UserData>().Query on new { rad.OwnerId, Name = Constants.UserData_ProfileImage } equals new { rpi.OwnerId, rpi.Name } into _rpi
           join spi in _europa.Store<UserData>().Query on new { sad.OwnerId, Name = Constants.UserData_ProfileImage } equals new { spi.OwnerId, spi.Name } into _spi
           from __spi in _spi.DefaultIfEmpty()
           from __rpi in _rpi.DefaultIfEmpty()
           from __rb in _rb.DefaultIfEmpty()
           from __sb in _sb.DefaultIfEmpty()
           from __rc in _rc.DefaultIfEmpty()
           from __sc in _sc.DefaultIfEmpty()
           select new BitLevelJoiner
           {
               Level = bl,
               Transaction = btc,
               ReceiverAddress = rad,
               SenderAddress = sad,
               ReceiverNode = rref,
               SenderNode = sref,
               SenderBio = __sb,
               ReceiverBio = __rb,
               Receiver = r,
               Sender = s,
               ReceiverContact = __rc,
               SenderContact = __sc,
               ReceiverProfileImage = __rpi,
               SenderProfileImage = __spi
           };

        public IQueryable<TransactionJoiner> BaseBlockChainQuery()
        => from btc in _europa.Store<BlockChainTransaction>().Query
           join rad in _europa.Store<BitcoinAddress>().Query on btc.ReceiverId equals rad.Id
           join sad in _europa.Store<BitcoinAddress>().Query on btc.SenderId equals sad.Id
           join r in _europa.Store<User>().Query on rad.OwnerId equals r.EntityId
           join s in _europa.Store<User>().Query on sad.OwnerId equals s.EntityId
           join rref in _europa.Store<ReferralNode>().Query on rad.OwnerId equals rref.UserId
           join sref in _europa.Store<ReferralNode>().Query on sad.OwnerId equals sref.UserId
           join rb in _europa.Store<BioData>().Query on rad.OwnerId equals rb.OwnerId into _rb
           join sb in _europa.Store<BioData>().Query on sad.OwnerId equals sb.OwnerId into _sb
           join rc in _europa.Store<ContactData>().Query on rad.OwnerId equals rc.OwnerId into _rc
           join sc in _europa.Store<ContactData>().Query on sad.OwnerId equals sc.OwnerId into _sc
           join rpi in _europa.Store<UserData>().Query on new { rad.OwnerId, Name = Constants.UserData_ProfileImage } equals new { rpi.OwnerId, rpi.Name } into _rpi
           join spi in _europa.Store<UserData>().Query on new { sad.OwnerId, Name = Constants.UserData_ProfileImage } equals new { spi.OwnerId, spi.Name } into _spi
           from __spi in _spi.DefaultIfEmpty()
           from __rpi in _rpi.DefaultIfEmpty()
           from __rb in _rb.DefaultIfEmpty()
           from __sb in _sb.DefaultIfEmpty()
           from __rc in _rc.DefaultIfEmpty()
           from __sc in _sc.DefaultIfEmpty()
           select new TransactionJoiner
           {
               Transaction = btc,
               ReceiverAddress = rad,
               SenderAddress = sad,
               ReceiverNode = rref,
               SenderNode = sref,
               SenderBio = __sb,
               ReceiverBio = __rb,
               Receiver = r,
               Sender = s,
               ReceiverContact = __rc,
               SenderContact = __sc,
               ReceiverProfileImage = __rpi,
               SenderProfileImage = __spi
           };

        public IQueryable<BitcoinAddressJoiner> BaseBitcoinAddressQuery()
        => from rad in _europa.Store<BitcoinAddress>().Query
           join rref in _europa.Store<ReferralNode>().Query on rad.OwnerId equals rref.UserId
           join rb in _europa.Store<BioData>().Query on rad.OwnerId equals rb.OwnerId into _rb
           join o in _europa.Store<User>().Query on rad.OwnerId equals o.EntityId
           join rc in _europa.Store<ContactData>().Query on rad.OwnerId equals rc.OwnerId into _rc
           join rpi in _europa.Store<UserData>().Query on new { rad.OwnerId, Name = Constants.UserData_ProfileImage } equals new { rpi.OwnerId, rpi.Name } into _rpi
           from __rpi in _rpi.DefaultIfEmpty()
           from __rc in _rc.DefaultIfEmpty()
           from __rb in _rb.DefaultIfEmpty()
           select new BitcoinAddressJoiner
           {
               Address = rad,
               RefNode = rref,
               UserBio = __rb,
               UserContact = __rc,
               Owner = o,
               ProfileImage = __rpi
           };

        public IQueryable<ReferralNodeJoiner> BaseReferralNodeQuery()
        => from rref in _europa.Store<ReferralNode>().Query
           join rb in _europa.Store<BioData>().Query on rref.UserId equals rb.OwnerId into _rb
           join o in _europa.Store<User>().Query on rref.UserId equals o.EntityId
           join rc in _europa.Store<ContactData>().Query on rref.UserId equals rc.OwnerId into _rc
           join rpi in _europa.Store<UserData>().Query on new { OwnerId = rref.UserId, Name = Constants.UserData_ProfileImage } equals new { rpi.OwnerId, rpi.Name } into _rpi
           from __rpi in _rpi.DefaultIfEmpty()
           from __rc in _rc.DefaultIfEmpty()
           from __rb in _rb.DefaultIfEmpty()
           select new ReferralNodeJoiner
           {
               RefNode = rref,
               UserBio = __rb,
               UserContact = __rc,
               Owner = o,
               ProfileImage = __rpi
           };
        #endregion


        public BitLevel CurrentBitLevel(User user)
        => BaseBitLevelQuery()
            .Where(_jo => _jo.Level.UserId == user.EntityId)
            .OrderByDescending(_jo => _jo.Level.Cycle)
            .ThenByDescending(_jo => _jo.Level.Level)
            .FirstOrDefault()?
            .ToBitLevel();

        public IEnumerable<ReferralNode> Downlines(User user)
        => UserRef(user).Pipe(_ur => _refQuery.AllDownlines(_ur));

        public BitcoinAddress GetActiveBitcoinAddress(User user)
        => BaseBitcoinAddressQuery()
            .Where(_bca => _bca.Address.Owner.EntityId == user.EntityId)
            .Where(_bca => _bca.Address.IsActive)
            .FirstOrDefault()?
            .ToAddress();

        public BitLevel GetBitLevelById(long id)
        => BaseBitLevelQuery()
            .Where(_bca => _bca.Level.Id == id)
            .FirstOrDefault()?
            .ToBitLevel();

        public IEnumerable<BitLevel> GetBitLevelHistory(User user)
        => BaseBitLevelQuery()
            .Where(_bca => _bca.Level.User.EntityId == user.EntityId)
            .OrderByDescending(_bca => _bca.Level.CreatedOn)
            .AsEnumerable()
            .Select(_jo => _jo.ToBitLevel())
            .ToArray();

        public SequencePage<BitLevel> GetPagedBitLevelHistory(User user, int pageSize, long pageIndex)
        => BaseBitLevelQuery()
            .Where(_bca => _bca.Level.User.EntityId == user.EntityId)
            .OrderByDescending(_bca => _bca.Level.CreatedOn)
            .Pipe(_p => new SequencePage<BitLevel>(_p.Skip((int)(pageSize * pageIndex))
                                                     .Take(pageSize)
                                                     .AsEnumerable()
                                                     .Select(_jo => _jo.ToBitLevel())
                                                     .ToArray(), _p.Count(), pageSize, pageIndex));

        public BitLevel GetNextUpgradeBeneficiary(User user)
        {
            var uplines = Uplines(user);
            var userLevel = CurrentBitLevel(user);
            return uplines.Select(_ul => CurrentBitLevel(_ul.User))
                          .Where(_bl => (_bl.Level > userLevel.Level && _bl.Cycle == userLevel.Cycle) || _bl.Cycle > userLevel.Cycle)
                          .FirstOrDefault();
        }

        public User GetUser(string targetUser)
        => _europa.Store<User>().Query.FirstOrDefault(_u => _u.EntityId == targetUser);

        public IEnumerable<ReferralNode> Referrals(User user)
        {
            var query =
@"
WITH DownLinesCTE (ReferenceCode, [rank])
AS
(
-- Anchor member definition
    SELECT r.ReferenceCode, 0
    FROM dbo.ReferralNode AS r
    WHERE r.UserId = @userId

    UNION ALL

-- Recursive member definition
    SELECT referred.ReferenceCode, code.[rank] + 1
    FROM DownLinesCTE AS code
    JOIN dbo.ReferralNode AS referred ON referred.ReferrerCode = code.ReferenceCode
)

-- Statement that executes the CTE
SELECT r.ReferenceCode, r.ReferrerCode, r.UplineCode, r.CreatedOn, r.ModifiedOn, r.Id, 
       u.EntityId AS u_EntityId, u.CreatedOn AS u_CreatedOn, u.ModifiedOn AS u_ModifiedOn, u.Status AS u_Status, u.UId AS u_UId,
       bd.FirstName, bd.LastName, cd.Phone AS u_Phone, ud.Data AS u_ProfileImage
FROM dbo.ReferralNode AS r
JOIN dbo.[User] AS u ON u.EntityId = r.UserId
JOIN DownLinesCTE  AS dl ON dl.ReferenceCode = r.ReferenceCode
LEFT JOIN dbo.BioData AS bd ON bd.OwnerId = u.EntityId
LEFT JOIN dbo.ContactData AS cd ON cd.OwnerId = u.EntityId
LEFT JOIN dbo.UserData AS ud ON ud.OwnerId = u.EntityId AND ud.Name = '" + Constants.UserData_ProfileImage + @"'
ORDER BY dl.[rank]
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
                            ModifiedOn = row.IsDBNull(4) ? (DateTime?)null : row.GetDateTime(4),
                            Id = row.GetInt64(5),
                            User = userCache.GetOrAdd(row.GetString(6), _uid => new User
                            {
                                EntityId = _uid,
                                CreatedOn = row.GetDateTime(7),
                                ModifiedOn = row.IsDBNull(8) ? (DateTime?)null : row.GetDateTime(8),
                                Status = row.GetInt32(9),
                                UId = row.GetGuid(10)
                            }),
                            UserBio = !hasBio(row)? null: new BioData
                            {
                                FirstName = row.IsDBNull(11) ? null : row.GetString(11),
                                LastName = row.IsDBNull(12) ? null : row.GetString(12)
                            },
                            UserContact = row.IsDBNull(13)? null: new ContactData
                            {
                                Phone = row.GetString(13)
                            }                            
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
        => BaseReferralNodeQuery()
            .Where(_r => _r.RefNode.User.EntityId == user.EntityId)
            .FirstOrDefault()?
            .ToRefNode();

        public BitcoinAddress GetBitcoinAddressById(long id)
        => BaseBitcoinAddressQuery()
            .Where(_ba => _ba.Address.Id == id)
            .FirstOrDefault()?
            .ToAddress();

        public BitLevel GetBitLevel(User user)
        => BaseBitLevelQuery()
            .Where(_ba => _ba.Level.UserId == user.UserId)
            .FirstOrDefault()?
            .ToBitLevel();

        public BitLevel PreviousBitLevel(User targetUser)
        => BaseBitLevelQuery()
            .OrderByDescending(_bl => _bl.Level.CreatedOn)
            .Skip(1).Take(1)
            .FirstOrDefault()?
            .ToBitLevel();

        public BlockChainTransaction GetTransactionWithHash(string transactionHash)
        => BaseBlockChainQuery()
             .Where(_bct => _bct.Transaction.TransactionHash == transactionHash)
             .FirstOrDefault()?
             .ToBlockChainTransaction();

        public BitLevel GetBitLevelHavingTransaction(long id)
        => BaseBitLevelQuery()
            .Where(_bl => _bl.Level.DonationId == id)
            .FirstOrDefault()?
            .ToBitLevel();

        public BlockChainTransaction GetBlockChainTransaction(long transactionId)
        => BaseBlockChainQuery()
            .Where(_bct => _bct.Transaction.Id == transactionId)
            .FirstOrDefault()?
            .ToBlockChainTransaction();

        public IEnumerable<BitcoinAddress> GetBitcoinAddresses(User user)
        => BaseBitcoinAddressQuery()
            .Where(_bca => _bca.Address.OwnerId == user.EntityId)
            .AsEnumerable()
            .Select(_jo => _jo.ToAddress())
            .ToArray();

        public IEnumerable<BitcoinAddress> GetAllBitcoinAddresses(User user)
        => BaseBitcoinAddressQuery()
            .Where(_bca => _bca.Address.OwnerId == user.UserId)
            .AsEnumerable()
            .Select(_jo => _jo.ToAddress())
            .ToArray();

        private bool hasBio(SqlDataReader row) => !row.IsDBNull(11) || !row.IsDBNull(12);

        public bool AddressExists(string blockChainAddress)
        => _europa.Store<BitcoinAddress>().Query.Any(_bc => _bc.BlockChainAddress == blockChainAddress);


        #region Joiner helper classes
        public class BitLevelJoiner
        {
            public BitLevel Level { get; set; }
            public BlockChainTransaction Transaction { get; set; }
            public BitcoinAddress SenderAddress { get; set; }
            public BitcoinAddress ReceiverAddress { get; set; }
            public ReferralNode SenderNode { get; set; }
            public ReferralNode ReceiverNode { get; set; }
            public BioData SenderBio { get; set; }
            public BioData ReceiverBio { get; set; }
            public ContactData SenderContact { get; set; }
            public ContactData ReceiverContact { get; set; }
            public User Receiver { get; set; }
            public User Sender { get; set; }
            public UserData ReceiverProfileImage { get; internal set; }
            public UserData SenderProfileImage { get; internal set; }

            public BitLevel ToBitLevel()
            {
                if (SenderBio != null) SenderBio.Owner = Sender;
                if (ReceiverBio != null) ReceiverBio.Owner = Receiver;

                if (SenderContact != null) SenderContact.Owner = Sender;
                if (ReceiverContact != null) ReceiverContact.Owner = Receiver;

                SenderNode.UserBio = SenderBio;
                SenderNode.UserContact = SenderContact;
                SenderNode.ProfileImageUrl = SenderProfileImage?.Data;
                SenderNode.User = Sender;

                ReceiverNode.UserBio = ReceiverBio;
                ReceiverNode.UserContact = ReceiverContact;
                ReceiverNode.ProfileImageUrl = ReceiverProfileImage?.Data;
                ReceiverNode.User = Receiver;

                SenderAddress.OwnerRef = SenderNode;
                ReceiverAddress.OwnerRef = ReceiverNode;

                Transaction.Sender = SenderAddress;
                Transaction.Receiver = ReceiverAddress;

                Level.Donation = Transaction;
                Level.User = Sender;
                return Level;
            }
        }

        public class TransactionJoiner
        {
            public BlockChainTransaction Transaction { get; set; }
            public BitcoinAddress SenderAddress { get; set; }
            public BitcoinAddress ReceiverAddress { get; set; }
            public ReferralNode SenderNode { get; set; }
            public ReferralNode ReceiverNode { get; set; }
            public BioData SenderBio { get; set; }
            public BioData ReceiverBio { get; set; }
            public ContactData SenderContact { get; set; }
            public ContactData ReceiverContact { get; set; }
            public User Receiver { get; set; }
            public User Sender { get; set; }
            public UserData ReceiverProfileImage { get; internal set; }
            public UserData SenderProfileImage { get; internal set; }

            public BlockChainTransaction ToBlockChainTransaction()
            {
                if (SenderBio != null) SenderBio.Owner = Sender;
                if (ReceiverBio != null) ReceiverBio.Owner = Receiver;

                if (SenderContact != null) SenderContact.Owner = Sender;
                if (ReceiverContact != null) ReceiverContact.Owner = Receiver;

                SenderNode.UserBio = SenderBio;
                SenderNode.UserContact = SenderContact;
                SenderNode.ProfileImageUrl = SenderProfileImage?.Data;
                SenderNode.User = Sender;

                ReceiverNode.UserBio = ReceiverBio;
                ReceiverNode.UserContact = ReceiverContact;
                ReceiverNode.ProfileImageUrl = ReceiverProfileImage?.Data;
                ReceiverNode.User = Receiver;

                SenderAddress.OwnerRef = SenderNode;
                ReceiverAddress.OwnerRef = ReceiverNode;

                Transaction.Sender = SenderAddress;
                Transaction.Receiver = ReceiverAddress;

                return Transaction;
            }
        }

        public class BitcoinAddressJoiner
        {
            public BitcoinAddress Address { get; set; }
            public ReferralNode RefNode { get; set; }
            public BioData UserBio { get; set; }
            public ContactData UserContact { get; set; }
            public User Owner { get; set; }
            public UserData ProfileImage { get; internal set; }

            public BitcoinAddress ToAddress()
            {
                if (UserBio != null) UserBio.Owner = Owner;
                if (UserContact != null) UserContact.Owner = Owner;

                RefNode.UserBio = UserBio;
                RefNode.UserContact = UserContact;
                RefNode.ProfileImageUrl = ProfileImage?.Data;
                Address.OwnerRef = RefNode;
                Address.Owner = Owner;

                return Address;
            }
        }

        public class ReferralNodeJoiner
        {
            public ReferralNode RefNode { get; set; }
            public BioData UserBio { get; set; }
            public ContactData UserContact { get; set; }
            public User Owner { get; set; }
            public UserData ProfileImage { get; internal set; }

            public ReferralNode ToRefNode()
            {
                if (UserBio != null) UserBio.Owner = Owner;
                if (UserContact != null) UserContact.Owner = Owner;

                RefNode.UserBio = UserBio;
                RefNode.UserContact = UserContact;
                RefNode.User = Owner;
                RefNode.ProfileImageUrl = ProfileImage?.Data;

                return RefNode;
            }
        }
        #endregion
    }
}
