using System;
using System.Collections.Generic;
using Axis.Luna;
using BitDiamond.Core.Models;
using BitDiamond.Core.Services.Query;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Pollux.RBAC.Services;
using BitDiamond.Core.Utils;
using Axis.Luna.Extensions;
using Newtonsoft.Json;
using Axis.Jupiter.Kore.Command;
using Axis.Pollux.Identity.Principal;
using System.Linq;
using Haxh;
using BitDiamond.Core.Models.Email;

namespace BitDiamond.Core.Services
{
    public class BitLevelManager : IBitLevelManager, IUserContextAware
    {
        public static readonly string BackgroundJob_AutomaticConfirmation = "BitDiamond.BitLevel.AutoConfirmation";

        private IBitLevelQuery _query = null;
        private IReferralQuery _refQuery = null;
        private IPersistenceCommands _pcommand = null;
        private IUserAuthorization _authorizer = null;
        private IUserNotifier _notifier = null;
        private IBlockChainService _blockChain = null;
        private ISettingsManager _settingsManager = null;
        //private IEmailPush _emailService = null;
        private IBackgroundOperationScheduler _backgroundProcessor;
        private IAppUrlProvider _urlProvider;

        public IUserContext UserContext { get; private set; }

        public BitLevelManager(IUserAuthorization authorizer, IUserContext userContext, IBitLevelQuery query, 
                               IPersistenceCommands pcommand, IUserNotifier notifier, IBlockChainService blockChain,
                               ISettingsManager settingsManager, IReferralQuery refQuery,
                               //IEmailPush emailService, 
                               IBackgroundOperationScheduler backgroundProcessor,
                               IAppUrlProvider urlProvider)
        {
            ThrowNullArguments(() => userContext,
                               () => query,
                               () => pcommand,
                               () => notifier,
                               () => blockChain,
                               () => settingsManager,
                               () => refQuery,
                               //() => emailService,
                               () => backgroundProcessor,
                               () => urlProvider);

            _query = query;
            _pcommand = pcommand;
            _authorizer = authorizer;
            _notifier = notifier;
            _blockChain = blockChain;
            _refQuery = refQuery;
            _settingsManager = settingsManager;
            //_emailService = emailService;
            _backgroundProcessor = backgroundProcessor;
            _urlProvider = urlProvider;

            UserContext = userContext;
        }
               

        /// <summary>
        /// Does both upgrading and recycling
        /// </summary>
        /// <returns></returns>
        public Operation<BitLevel> Upgrade()
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {

            var maxLevelSettings = _settingsManager.GetSetting(Constants.Settings_MaxBitLevel).Resolve();
            var maxLevel = (int)maxLevelSettings.ParseData<long>();
            var targetUser = UserContext.CurrentUser();
            var currentLevel = _query.CurrentBitLevel(targetUser);

            if (currentLevel != null)
                ConfirmUpgradeDonation().Resolve();

            else currentLevel = new BitLevel
            {
                Level = maxLevel,
                Cycle = 0,
                Donation = new BlockChainTransaction { Status = BlockChainTransactionStatus.Verified }
            };


            if (currentLevel.Donation.Status == BlockChainTransactionStatus.Unverified)
                throw new Exception("Current level donnation is still unverified");

            var myAddress = _query.GetActiveBitcoinAddress(targetUser)
                                  .ThrowIfNull("You do not have a valid block chain transaction address yet.");

            if (currentLevel.Cycle == int.MaxValue && currentLevel.Level == maxLevel)
                throw new Exception("You cannot upgrade past the maximum cycle");

            var nextLevel = (currentLevel.Level + 1) % (maxLevel + 1);
            var nextCycle = nextLevel == 0 ? currentLevel.Cycle + 1 : currentLevel.Cycle;

            var nextUpgradeBeneficiary = NextUpgradeBeneficiary(nextLevel == 0 ? targetUser : currentLevel.Donation.Receiver.Owner, nextLevel, nextCycle).ThrowIfNull("Could not find a valid beneficiary");
            var beneficiaryAddress = _query.GetActiveBitcoinAddress(nextUpgradeBeneficiary);

            var bl = new BitLevel
            {
                Level = nextLevel,
                Cycle = nextCycle,
                DonationCount = 0,
                SkipCount = 0,
                UserId = targetUser.UserId
            };
            _pcommand.Add(bl).Resolve();

            var donation = new BlockChainTransaction
            {
                Amount = nextLevel == maxLevel ? 0 : GetUpgradeAmount(nextLevel + 1),
                LedgerCount = nextLevel == maxLevel ? int.MaxValue : 0,
                CreatedOn = DateTime.Now,
                Sender = myAddress,
                Receiver = beneficiaryAddress,
                ContextType = Constants.TransactionContext_UpgradeBitLevel,
                ContextId = bl.Id.ToString(),
                Status = nextLevel == maxLevel ?
                         BlockChainTransactionStatus.Verified : 
                         BlockChainTransactionStatus.Unverified
            };
            _pcommand.Add(donation).Resolve();

            bl.DonationId = donation.Id;
            _pcommand.Update(bl);

            bl.Donation = donation;

            //notify user
            _notifier.NotifyUser(new Notification
            {
                Type = NotificationType.Success,
                TargetId = targetUser.UserId,
                Title = "Congratulations!",
                Message = $"Well done, {targetUser.UserId}!! You are now <strong>proudly</strong> at Cycle-{nextCycle} / Level-{nextLevel}, no easy feat!<br/>"+
                @"Now you can receive even more donations from your downlines. 
                  <p>Remember though, that this is a race to the <strong class='text-primary'>top</strong>, and as such
                  you may miss the donations of any of your downlines who upgrades to levels higher than yours. So dont waste much time here, Upgrade as soon as you can!</p>"
            })
            .Resolve();

            return bl;
        });

        public Operation<BlockChainTransaction> VerifyAndSaveTransactionHash(string transactionHash)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var currentUser = UserContext.CurrentUser();
            var currentLevel = _query.CurrentBitLevel(currentUser);

            _query.GetTransactionWithHash(transactionHash)
                  .ThrowIfNotNull(new Exception("A transaction already exists in the system with the supplied hash"));

            return _blockChain
                .VerifyTransaction(transactionHash, currentLevel)
                .Then(opr =>
                {
                    currentLevel.Donation.TransactionHash = transactionHash;
                    return _pcommand.Update(currentLevel.Donation);
                })
                .Then(opr => _backgroundProcessor.RepeatOperation<IBitLevelManager>(BackgroundJob_AutomaticConfirmation, UserContext.Impersonate(Constants.SystemUsers_Root), _blm => _blm.AutoConfirmDonations(), ScheduleInterval.Hourly)
                .Then(_opr => opr.Result));
        });

        public Operation AutoConfirmDonations()
        => _authorizer.AuthorizeAccess(this.PermissionProfile(UserContext.CurrentUser()), () =>
        {
            _query.GetUsersWithUnconfirmedTransactions()
                  .ForAll((_cnt, _user) => Operation.Try(() => ConfirmDonation(_user)));
        });

        public Operation<BitLevel> Demote(string userRef, int units, string haxh)
        => _authorizer.AuthorizeAccess(this.PermissionProfile(UserContext.CurrentUser()), () =>
        {
            Haxher.IsValidHash(haxh).ThrowIf(_v => !_v, "Access Denied");

            var @ref = _refQuery.GetReferalNode(userRef);
            var targetUser = new User { EntityId = @ref.UserId };
            var currentLevel = _query.CurrentBitLevel(targetUser).ThrowIfNull("User has not begun cycling");
            var newLevel = BitCycle.Create(currentLevel.Cycle, currentLevel.Level).Decrement(units);

            currentLevel.Cycle = newLevel.Cycle;
            currentLevel.Level = newLevel.Level;
            _pcommand.Update(currentLevel);

            currentLevel.Donation.Amount = GetUpgradeAmount(newLevel.Level + 1);
            _pcommand.Update(currentLevel.Donation);

            //move all current donnors to the next available beneficiary
            _query.GetDonorLevels(targetUser)
                  .Where(_lvl => newLevel > new BitCycle { Cycle = _lvl.Cycle, Level = _lvl.Level })
                  .Select(_lvl =>
                  {
                      var nextLevel = new BitCycle { Cycle = _lvl.Cycle, Level = _lvl.Level }.Increment(1);
                      var beneficiary = NextUpgradeBeneficiary(targetUser, nextLevel.Level, nextLevel.Cycle);
                      var address = _query.GetActiveBitcoinAddress(beneficiary);

                      _lvl.Donation.ReceiverId = address.Id;

                      return _pcommand.Update(_lvl.Donation);
                  })
                  .ThrowIf(_ops => !_ops.Any(_op => _op.Succeeded), "failed to reassign some donors");

            return currentLevel;
        });


        public Operation<BitLevel> Promote(string userRef, int units, string securityHash)
        => _authorizer.AuthorizeAccess(this.PermissionProfile(UserContext.CurrentUser()), () =>
        {
            //verify the securityHash
            Haxher.IsValidHash(securityHash).ThrowIf(_v => !_v, "Access Denied");

            //do promotion logic here
            var @ref = _refQuery.GetReferalNode(userRef);
            var targetUser = new User { EntityId = @ref.UserId };
            var currentLevel = _query.CurrentBitLevel(targetUser);
            var newLevel = BitCycle.Create(currentLevel.Cycle, currentLevel.Level).Increment(units);
            var beneficiary = NextUpgradeBeneficiary(new User { UserId = currentLevel.Donation.Receiver.OwnerId }, newLevel.Level, newLevel.Cycle)
                .ThrowIfNull("could not find a suitable beneficiary");

            var beneficiaryAddress = _query.GetActiveBitcoinAddress(beneficiary);
            var targetUserAddress = _query.GetActiveBitcoinAddress(targetUser);

            //close off the old level
            currentLevel.Donation.Amount = 0;
            currentLevel.Donation.Status = BlockChainTransactionStatus.Verified;
            _pcommand.Update(currentLevel.Donation);

            var bl = new BitLevel
            {
                Level = newLevel.Level,
                Cycle = newLevel.Cycle,
                DonationCount = 0,
                SkipCount = 0,
                UserId = @ref.UserId
            };
            _pcommand.Add(bl).Resolve();

            var maxLevelSettings = _settingsManager.GetSetting(Constants.Settings_MaxBitLevel).Resolve();
            var maxLevel = (int)maxLevelSettings.ParseData<long>();

            var donation = new BlockChainTransaction
            {
                Amount = newLevel.Level == maxLevel ? 0 : GetUpgradeAmount(newLevel.Level + 1),
                LedgerCount = newLevel.Level == maxLevel ? int.MaxValue : 0,
                CreatedOn = DateTime.Now,
                Sender = targetUserAddress,
                Receiver = beneficiaryAddress,
                ContextType = Constants.TransactionContext_UpgradeBitLevel,
                ContextId = bl.Id.ToString(),
                Status = newLevel.Level == maxLevel ?
                         BlockChainTransactionStatus.Verified :
                         BlockChainTransactionStatus.Unverified
            };
            _pcommand.Add(donation).Resolve();

            bl.DonationId = donation.Id;
            _pcommand.Update(bl);

            bl.Donation = donation;

            //notify user
            _notifier.NotifyUser(new Notification
            {
                Type = NotificationType.Success,
                TargetId = targetUser.UserId,
                Title = "Congratulations!",
                Message = $"Well done, {targetUser.UserId}!! You are now <strong>proudly</strong> at Cycle-{newLevel.Cycle} / Level-{newLevel.Level}, no easy feat!<br/>" +
                @"Now you can receive even more donations from your downlines. 
                  <p>Remember though, that this is a race to the <strong class='text-primary'>top</strong>, and as such
                  you may miss the donations of any of your downlines who upgrades to levels higher than yours. So dont waste much time here, Upgrade as soon as you can!</p>"
            })
            .Resolve();

            return bl;
        });

        public Operation<BlockChainTransaction> ConfirmUpgradeDonation()
        => _authorizer.AuthorizeAccess(this.PermissionProfile(UserContext.CurrentUser()), () => ConfirmDonation(UserContext.CurrentUser()));


        public Operation<BitLevel> CurrentUserLevel()
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(),() =>
        {
            var level =_query.CurrentBitLevel(UserContext.CurrentUser());
            return level;
        });

        public Operation<BitLevel> GetBitLevelById(long id)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(),
                                       () => _query.GetBitLevelById(id));

        public Operation<IEnumerable<BitLevel>> UserUpgradeHistory()
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(),
                                       () => _query.GetBitLevelHistory(UserContext.CurrentUser()));

        public Operation<decimal> GetUpgradeFee(int level)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(),
                                       () => GetUpgradeAmount(level));

        public Operation<BlockChainTransaction> GetCurrentUpgradeTransaction()
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), 
                                       () => _query.CurrentBitLevel(UserContext.CurrentUser()).Donation);

        public Operation<IEnumerable<BitcoinAddress>> GetAllBitcoinAddresses()
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _query.GetAllBitcoinAddresses(UserContext.CurrentUser())
                         .OrderByDescending(_bca => _bca.CreatedOn)
                         .AsEnumerable();
        });

        public Operation<BitcoinAddress> AddBitcoindAddress(BitcoinAddress address)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            if (address == null || address.Id != 0) throw new Exception("Invalid bit address given");
            else if (_query.AddressExists(address.BlockChainAddress)) throw new Exception("This address has already been used");
            else return address.With(new { OwnerId = UserContext.CurrentUser().UserId }).Validate()
            .Then(opr =>
            {
                address.IsVerified = false;
                address.IsActive = false;

                return _pcommand.Add(address);
            });
        });

        public Operation<BitcoinAddress> ActivateAddress(long bitcoinAddressId)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var address = _query.GetBitcoinAddressById(bitcoinAddressId);
            if (address == null || address.OwnerId != UserContext.CurrentUser().UserId || !address.IsVerified) throw new Exception("Invalid bitcoin address");
            else if(!address.IsActive)
            {
                _query.GetActiveBitcoinAddress(UserContext.CurrentUser())

                      //deactivate the old address if it exists
                      .Pipe(_bca => _bca != null ? DeactivateAddress(_bca.Id) : Operation.FromValue<BitcoinAddress>(null))

                      //Change the address of all donations coming to my address that are not verified
                      .Then(opr =>
                      {
                          if (opr.Result != null)
                              _query.GetAllDonationsWithReceiverAddress(opr.Result.BlockChainAddress)
                             .Where(_bct => _bct.Status == BlockChainTransactionStatus.Unverified)
                             .ForAll((_cnt, _bct) =>
                             {
                                 _bct.Receiver = address;
                                 _pcommand.Update(_bct).Resolve();
                             });
                      })

                      //activate the new address
                      .Then(opr =>
                      {
                          address.IsActive = true;
                          return _pcommand.Update(address);
                      })

                      //Change the address of the donation to the next level...if the donation has not been verified
                      .Then(opr =>
                      {
                          var currentLevel = _query.CurrentBitLevel(UserContext.CurrentUser()); //<-- may return null for new users
                          if (currentLevel?.Donation.Status == BlockChainTransactionStatus.Unverified)
                          {
                              currentLevel.Donation.SenderId = opr.Result.Id;
                              _pcommand.Update(currentLevel.Donation);
                          }
                      })

                      .Resolve();
            }

            return address;
        });

        public Operation<BitcoinAddress> DeactivateAddress(long bitcoinAddressId)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var address = _query.GetBitcoinAddressById(bitcoinAddressId);
            if (address == null || address.OwnerId != UserContext.CurrentUser().UserId) throw new Exception("Invalid bitcoin address");
            else if (address.IsActive)
            {
                address.IsActive = false;
                _pcommand.Update(address);
            }

            return address;
        });

        public Operation<ReferralNode> GetUserRef(string userId)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _refQuery.GetUserReferalNode(_query.GetUser(userId));
        });

        public Operation<BitcoinAddress> VerifyAddress(long bitcoinAddressId)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var address = _query.GetBitcoinAddressById(bitcoinAddressId)
                                .ThrowIfNull("Invalid bitcoin id specified");

            return _blockChain.VerifyBitcoinAddress(address)
                              .Then(opr => _pcommand.Update(opr.Result.With(new { IsVerified = true })));
        });

        public Operation<BitcoinAddress> GetActiveBitcoinAddress()
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _query.GetActiveBitcoinAddress(UserContext.CurrentUser());
        });

        public Operation<SequencePage<BitLevel>> PagedUserUpgradeHistory(int pageSize, long pageIndex = 0)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _query.GetPagedBitLevelHistory(UserContext.CurrentUser(), pageSize, pageIndex);
        });


        private decimal GetUpgradeAmount(int level)
        => _settingsManager.GetSetting(Constants.Settings_UpgradeFeeVector)
                           .Resolve()
                           .ParseData(_d => JsonConvert.DeserializeObject<decimal[]>(_d))
                           [level-1];


        private bool IsSameTransaction(BlockChainTransaction tnx1, BlockChainTransaction tnx2)
        {
            if (tnx1 == null || tnx2 == null) return false;
            return tnx1.Amount == tnx2.Amount &&
                   (tnx1.ReceiverId.Equals(tnx2.ReceiverId)) &&
                   (tnx1.SenderId.Equals(tnx2.SenderId));
        }

        private User NextUpgradeBeneficiary(User lastBeneficiary, int nextLvel, int nextCycle)
        => _query.Uplines(lastBeneficiary)
                 .FirstOrDefault(_rn =>
                 {
                     var bl = _query.CurrentBitLevel(_rn.User);
                     if (bl == null) return false;
                     else if (bl.Cycle < nextCycle || bl.Cycle == nextCycle && bl.Level <= nextLvel)
                     {
                         bl.SkipCount++;
                         _pcommand.Update(bl);

                         var message = @"
<strong>Ouch!</strong> You just missed a donation...
<p>
    <span class='text-muted'>Your level</span><br />
    {0}
</p>
<p>
    <span class='text-muted'>Downline</span><br />
    {1}
</p>
<p>
    <span class='text-muted'>Downline Level</span><br />
    {2}
</p>
".ResolveParameters(new BitCycle { Level = nextLvel, Cycle = nextCycle }, _rn.UserId, new BitCycle { Level = bl.Level, Cycle = bl.Cycle });

                         //notify user
                         _notifier.NotifyUser(new Notification
                         {
                             Type = NotificationType.Info,
                             TargetId = bl.UserId,
                             Title = "Missed donation",
                             Message = message
                         })
                         .Resolve();

                         //send email
                         _backgroundProcessor.EnqueueOperation<IEmailPush>(_mp => _mp.SendMail(new GenericMessage
                         {
                             From = Constants.MailOrigin_DoNotReply,
                             Recipients = new[] { bl.UserId },
                             Subject = "Missed donation",
                             LogoUrl = _urlProvider.LogoUri().Resolve(),
                             LogoTextUrl = _urlProvider.LogoTextUri().Resolve(),
                             Message = message
                         }))
                         .Resolve();

                         return false;
                     }
                     else return true;
                 })?.User
                 ?? lastBeneficiary;

        public Operation<IEnumerable<BitcoinAddress>> GetReferencedAddresses()
        => _authorizer.AuthorizeAccess(this.PermissionProfile(UserContext.CurrentUser()), () =>
        {
            return _query.GetReferencedAddressesFor(UserContext.CurrentUser());
        });

        public Operation DeleteUnreferencedAddress(long id)
        => _authorizer.AuthorizeAccess(this.PermissionProfile(UserContext.CurrentUser()), () =>
        {
            if (_query.IsReferencedAddress(UserContext.CurrentUser(), id)) throw new Exception("Cannot delete a referenced address");

            var address = _query.GetBitcoinAddressById(id);
            _pcommand.Delete(address).Resolve();
        });


        private string AddressOwnerName(BitcoinAddress address)
        {
            if (address.OwnerRef == null) return address.OwnerId;
            else if (address.OwnerRef.UserBio == null) return address.OwnerRef.ReferenceCode;
            else if (!string.IsNullOrWhiteSpace(address.OwnerRef.UserBio.FirstName) ||
                    !string.IsNullOrWhiteSpace(address.OwnerRef.UserBio.LastName))
                return $"{address.OwnerRef.UserBio.FirstName} {address.OwnerRef.UserBio.LastName}";
            else return address.OwnerId;
        }

        private BlockChainTransaction ConfirmDonation(User currentUser)
        {

            var currentLevel = _query
                .CurrentBitLevel(currentUser)
                .ThrowIfNull("User has no BitLevel yet");

            //for a donation without receiver and sender
            if (currentLevel.Donation.SenderId <= 0 ||
                    currentLevel.Donation.ReceiverId <= 0)
                throw new Exception("Invalid Donation transaction Receiver/Sender");

            var blockChainTransaction = _blockChain
                .AcquireTransactionDetails(currentLevel.Donation.TransactionHash)
                .Resolve()
                .ThrowIf(_bct => _bct.LedgerCount < 3, "Transaction ledger count is less than threshold");

            if (IsSameTransaction(currentLevel.Donation, blockChainTransaction) &&
                blockChainTransaction.Status == BlockChainTransactionStatus.Verified)
            {
                currentLevel.Donation.Status = BlockChainTransactionStatus.Verified;
                currentLevel.Donation.LedgerCount = blockChainTransaction.LedgerCount;
                _pcommand.Update(currentLevel.Donation);

                //increment receiver's donnation count
                var receiverLevel = _query.CurrentBitLevel(blockChainTransaction.Receiver.Owner);
                receiverLevel.DonationCount++;
                _pcommand.Update(receiverLevel);

                //notify the receiver
                var message = @"
<h3>Congratulations!</h3>
<p>You just received a donation of " + blockChainTransaction.Amount + @" from " + AddressOwnerName(blockChainTransaction.Receiver) + @"</p>
Cheers.
";
                _backgroundProcessor.EnqueueOperation<IEmailPush>(_mp => _mp.SendMail(new GenericMessage
                {
                    From = Constants.MailOrigin_DoNotReply,
                    Recipients = new[] { blockChainTransaction.Receiver.OwnerId },
                    Subject = "Donation Received",
                    LogoUrl = _urlProvider.LogoUri().Resolve(),
                    LogoTextUrl = _urlProvider.LogoTextUri().Resolve(),
                    Message = message
                }))
                .Resolve();

                _notifier.NotifyUser(new Notification
                {
                    Message = message,
                    TargetId = blockChainTransaction.Receiver.OwnerId,
                    Title = "Donation Received",
                    Type = NotificationType.Info
                });

                return currentLevel.Donation;
            }
            else throw new Exception("Invalid transaction hash");
        }
    }
}
