using Axis.Jupiter.Europa;
using Axis.Jupiter.Europa.Module;
using Axis.Luna;
using Axis.Luna.Extensions;
using Axis.Pollux.Authentication;
using Axis.Pollux.CoreAuthentication;
using Axis.Pollux.CoreAuthentication.Services;
using Axis.Pollux.Identity.Principal;
using Axis.Pollux.RBAC.Auth;
using BitDiamond.Core.Models;
using BitDiamond.Core.Utils;
using System;
using System.Linq;
using System.Text;

namespace BitDiamond.Data.EF
{
    public class BitDiamondModuleConfig : BaseModuleConfigProvider
    {
        public override string ModuleName => "BitDiamond.Data.EF.OAModule";

        protected override void Initialize()
        {
            //load all mappings
            var asm = GetType().Assembly;
            asm.GetTypes()
               .Where(t => t.IsEntityMap() || t.IsComplexMap())
               .ForAll((cnt, t) => this.UsingConfiguration(Activator.CreateInstance(t).AsDynamic()));
            
            SystemSetting maxBitLevelSetting = null;

            ///Seed Data
            #region 1. Settings data
            UsingContext(cxt =>
            {
                if (cxt.Store<SystemSetting>().Query.Any()) return;

                new SystemSetting[]
                {
                    new SystemSetting
                    {
                        Name = Constants.Settings_DefaultContextVerificationExpirationTime,
                        Data = TimeSpan.FromDays(2).ToString(),
                        Type = Axis.Luna.CommonDataType.TimeSpan
                    },
                    new SystemSetting
                    {
                        Name = Constants.Settings_DefaultPasswordExpirationTime,
                        Data = TimeSpan.FromDays(730).ToString(), //two years
                        Type = Axis.Luna.CommonDataType.TimeSpan
                    },
                    maxBitLevelSetting = new SystemSetting
                    {
                        Name = Constants.Settings_MaxBitLevel,
                        Data = "3",
                        Type = Axis.Luna.CommonDataType.Integer
                    },
                    new SystemSetting
                    {
                        Name = Constants.Settings_UpgradeFeeVector,
                        Data = "[0.11, 0.1826, 0.3652]",
                        Type = Axis.Luna.CommonDataType.JsonObject
                    }
                }
                .ForAll((cnt, next) => cxt.Add(next));

                cxt.CommitChanges();
            });
            #endregion

            #region 2. Roles
            UsingContext(cxt =>
            {
                if (cxt.Store<Role>().Query.Any()) return;

                new Role[]
                {
                    new Role { RoleName = Constants.Roles_RootRole },
                    new Role { RoleName = Constants.Roles_AdminRole },
                    new Role { RoleName = Constants.Roles_GuestRole },
                    new Role { RoleName = Constants.Roles_BitMemberRole }
                }
                .ForAll((cnt, next) => cxt.Add(next));

                cxt.CommitChanges();
            });
            #endregion

            #region 3. Permissions
            UsingContext(cxt =>
            {
                if (cxt.Store<Permission>().Query.Any()) return;

                var set = ((EuropaContext)cxt).Set<Role>();

                #region Root
                new Permission[]
                {
                    new Permission
                    {
                        ResourceSelector = ":system/*",
                        RoleId = Constants.Roles_RootRole,
                        Effect = Effect.Grant
                    },
                }
                .ForAll((cnt, next) =>
                {
                    cxt.Add(next);
                });
                #endregion

                #region Guest
                new string[]
                {
                    ":system/accounts/users/@register",
                    ":system/accounts/admins/@register",
                    ":system/accounts/users/@activate",
                    ":system/accounts/users/activations/@request",
                    ":system/accounts/users/activations/@verify",
                    ":system/accounts/users/credentials/reset-tokens/@request",
                    ":system/accounts/users/credentials/reset-tokens/@verify",
                    ":system/accounts/users/@count",

                    ":system/settings/@get",
                    ":system/settings/all/@get",
                    ":system/bit-levels/cycles/@promote",
                    ":system/notifications/@notifyUser",
                    ":system/block-chain/transactions/@systemTotal"
                }
                .Select(_selector => new Permission
                {
                    ResourceSelector = _selector,
                    RoleId = Constants.Roles_GuestRole,
                    Effect = Effect.Grant
                })
                .ForAll((cnt, next) =>
                {
                    cxt.Add(next);
                });
                #endregion

                #region Admin
                new string[]
                {
                    ":system/accounts/users/@block",
                    ":system/accounts/users/@deactivate",
                    ":system/accounts/users/credentials/reset-tokens/@request",
                    ":system/accounts/users/credentials/reset-tokens/@verify",
                    ":system/accounts/biodata/*",
                    ":system/accounts/contacts/*",
                    ":system/accounts/userdata/*",
                    ":system/accounts/logons/@invalidate",
                    ":system/accounts/users/roles/@get",
                    ":system/accounts/users/@count",

                    ":system/settings/@get",
                    ":system/settings/all/@get",
                    ":system/settings/@update",

                    ":system/notifications/*",

                    ":system/posts/*",

                    ":system/block-chain/*"
                }
                .Select(_selector => new Permission
                {
                    ResourceSelector = _selector,
                    RoleId = Constants.Roles_AdminRole,
                    Effect = Effect.Grant
                })
                .ForAll((cnt, next) =>
                {
                    cxt.Add(next);
                });
                #endregion

                #region BitMember
                new string[]
                {
                    ":system/accounts/users/credentials/reset-tokens/@request",
                    ":system/accounts/users/credentials/reset-tokens/@verify",
                    ":system/accounts/biodata/*",
                    ":system/accounts/contacts/*",
                    ":system/accounts/userdata/*",
                    ":system/accounts/logons/@invalidate",
                    ":system/accounts/users/roles/@get",
                    ":system/accounts/users/@count",

                    ":system/bit-levels/*",

                    ":system/block-chain/*",

                    ":system/settings/@get",
                    ":system/settings/all/@get",

                    ":system/notifications/*",

                    ":system/posts/@getPaged",
                    ":system/posts/@getById"
                }
                .Select(_selector => new Permission
                {
                    ResourceSelector = _selector,
                    RoleId = Constants.Roles_BitMemberRole,
                    Effect = Effect.Grant
                })
                .ForAll((cnt, next) =>
                {
                    cxt.Add(next);
                });
                #endregion

                cxt.CommitChanges();

            });
            #endregion

            #region 4. default users (root, guest and apex)
            UsingContext(cxt =>
            {
                if (cxt.Store<User>().Query.Any()) return;

                new User[]
                {
                    new User
                    {
                        EntityId = Constants.SystemUsers_Root,
                        Status = AccountStatus.Active.As<int>()
                    },
                    new User
                    {
                        EntityId = Constants.SystemUsers_Guest,
                        Status = AccountStatus.Active.As<int>()
                    },
                    new User
                    {
                        EntityId = Constants.SystemUsers_Apex,
                        Status = AccountStatus.Active.As<int>()
                    }
                }
                .ForAll((cnt, next) => cxt.Add(next));

                cxt.CommitChanges();

                //add roles
                new UserRole[]
                {
                    new UserRole
                    {
                        UserId = Constants.SystemUsers_Root,
                        RoleName = Constants.Roles_RootRole
                    },
                    new UserRole
                    {
                        UserId = Constants.SystemUsers_Guest,
                        RoleName = Constants.Roles_GuestRole
                    },
                    new UserRole
                    {
                        UserId = Constants.SystemUsers_Apex,
                        RoleName = Constants.Roles_BitMemberRole
                    },
                    new UserRole
                    {
                        UserId = Constants.SystemUsers_Apex,
                        RoleName = Constants.Roles_AdminRole
                    }
                }
                .ForAll((cnt, next) => cxt.Add(next));

                //credential
                var credential = new Credential
                {
                    Metadata = CredentialMetadata.Password,
                    OwnerId = Constants.SystemUsers_Apex,
                    Status = CredentialStatus.Active,
                    Value = Encoding.UTF8.GetBytes("Nuid9x11")
                };
                var credentialAuthority = new CredentialAuthentication(cxt, new DefaultHasher());
                credentialAuthority.AssignCredential(Constants.SystemUsers_Apex, credential).Resolve();

                //add bit level and verified blocktransaction for apex user
                var bcaddress = new BitcoinAddress
                {
                    OwnerId = Constants.SystemUsers_Apex,
                    BlockChainAddress = "12b8exzGA5Dmq4dcCFq68Eym3maKnmN5pn",
                    IsActive = true,
                    IsVerified = true
                };
                cxt.Add(bcaddress).Context.CommitChanges();

                var bitlevel = new BitLevel
                {
                    Cycle = int.MaxValue,
                    Level = (int)maxBitLevelSetting.ParseData<long>(),
                    UserId = Constants.SystemUsers_Apex
                };
                cxt.Add(bitlevel).Context.CommitChanges();

                var transaction = new BlockChainTransaction
                {
                    Amount = 0m,
                    ContextId = bitlevel.Id.ToString(),
                    ContextType = Constants.TransactionContext_UpgradeBitLevel,
                    LedgerCount = int.MaxValue,
                    SenderId = bcaddress.Id,
                    ReceiverId = bcaddress.Id,
                    Status = BlockChainTransactionStatus.Verified,
                    TransactionHash = ""
                };
                cxt.Add(transaction).Context.CommitChanges();

                bitlevel.DonationId = transaction.Id;
                cxt.Modify(bitlevel).Context.CommitChanges();

                //referal manager
                var referral = new ReferralNode
                {
                    ReferenceCode = ReferralHelper.GenerateCode(Constants.SystemUsers_Apex),
                    UserId = Constants.SystemUsers_Apex
                };
                cxt.Add(referral).Context.CommitChanges();

                cxt.CommitChanges();
            });
            #endregion

        }
    }
}
