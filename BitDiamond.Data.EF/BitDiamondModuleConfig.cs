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
            string gerald = "gerald.jax@icloud.com",
                   onotu = "onotukivie.gbejewoh@gmail.com",
                   valejoma = "valejoma@live.com",
                   bunny = "bunnyrico@yahoo.co.uk";
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
                    },

                    //other users,
                    new User
                    {
                        EntityId = gerald,
                        Status = AccountStatus.Active.As<int>()
                    },
                    new User
                    {
                        EntityId = onotu,
                        Status = AccountStatus.Active.As<int>()
                    },
                    new User
                    {
                        EntityId = valejoma,
                        Status = AccountStatus.Active.As<int>()
                    },
                    new User
                    {
                        EntityId = bunny,
                        Status = AccountStatus.Active.As<int>()
                    }
                }
                .ForAll((cnt, next) => cxt.Add(next));

                cxt.CommitChanges();

                //add roles
                new[]
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
                    },

                    //other roles,
                    new UserRole
                    {
                        UserId = gerald,
                        RoleName = Constants.Roles_BitMemberRole
                    },
                    new UserRole
                    {
                        UserId = onotu,
                        RoleName = Constants.Roles_BitMemberRole
                    },
                    new UserRole
                    {
                        UserId = valejoma,
                        RoleName = Constants.Roles_BitMemberRole
                    },
                    new UserRole
                    {
                        UserId = bunny,
                        RoleName = Constants.Roles_BitMemberRole
                    }
                }
                .ForAll((cnt, next) => cxt.Add(next));

                //credential
                new[] 
                {
                    new Credential
                    {
                        Metadata = CredentialMetadata.Password,
                        OwnerId = Constants.SystemUsers_Apex,
                        Status = CredentialStatus.Active,
                        Value = Encoding.UTF8.GetBytes("Nuid9x11")
                    },

                    //others
                    new Credential
                    {
                        Metadata = CredentialMetadata.Password,
                        OwnerId = gerald,
                        Status = CredentialStatus.Active,
                        Value = Encoding.UTF8.GetBytes("YpbCR2")
                    },
                    new Credential
                    {
                        Metadata = CredentialMetadata.Password,
                        OwnerId = onotu,
                        Status = CredentialStatus.Active,
                        Value = Encoding.UTF8.GetBytes("2008teflondoc")
                    },
                    new Credential
                    {
                        Metadata = CredentialMetadata.Password,
                        OwnerId = valejoma,
                        Status = CredentialStatus.Active,
                        Value = Encoding.UTF8.GetBytes("@unutovogtb1912@")
                    },
                    new Credential
                    {
                        Metadata = CredentialMetadata.Password,
                        OwnerId = bunny,
                        Status = CredentialStatus.Active,
                        Value = Encoding.UTF8.GetBytes("karim1986")
                    }
                }
                .ForAll((cnt, next) =>
                {
                    var credentialAuthority = new CredentialAuthentication(cxt, new DefaultHasher());
                    credentialAuthority.AssignCredential(next.OwnerId, next).Resolve();
                });

                //add bit level and verified blocktransaction for apex user
                var addresses = new[]
                {
                    new BitcoinAddress
                    {
                        OwnerId = Constants.SystemUsers_Apex,
                        BlockChainAddress = "12b8exzGA5Dmq4dcCFq68Eym3maKnmN5pn",
                        IsActive = true,
                        IsVerified = true
                    },

                    //others,
                    new BitcoinAddress
                    {
                        OwnerId = gerald,
                        BlockChainAddress = "19yabtQHnu4PEd23w9dmhu5cGiQST9wnXi",
                        IsActive = true,
                        IsVerified = true
                    },
                    new BitcoinAddress
                    {
                        OwnerId = onotu,
                        BlockChainAddress = "14JhghD4C1D27p7KJMro2vHQzYzEQGMTkF",
                        IsActive = true,
                        IsVerified = true
                    },
                    new BitcoinAddress
                    {
                        OwnerId = valejoma,
                        BlockChainAddress = "1FXXAVYJfV2gL4juph5dfYmaC3v8tEboxY",
                        IsActive = true,
                        IsVerified = true
                    },
                    new BitcoinAddress
                    {
                        OwnerId = bunny,
                        BlockChainAddress = "1MtUiijySbfihb7XcuudFo6iZxronYdgdg",
                        IsActive = true,
                        IsVerified = true
                    }
                }
                .ToDictionary(_x => _x.OwnerId);
                addresses.ForAll((cnt, _bcaddress) =>
                {
                    cxt.Add(_bcaddress.Value).Context.CommitChanges();
                });

                var levels = new[]
                {
                    new BitLevel
                    {
                        Cycle = int.MaxValue,
                        Level = (int)maxBitLevelSetting.ParseData<long>(),
                        UserId = Constants.SystemUsers_Apex
                    },

                    //others,
                    new BitLevel
                    {
                        Cycle = 1,
                        Level = 2,
                        UserId = gerald
                    },
                    new BitLevel
                    {
                        Cycle = 1,
                        Level = 1,
                        UserId = onotu
                    },
                    new BitLevel
                    {
                        Cycle = 1,
                        Level = 1,
                        UserId = valejoma
                    },
                    new BitLevel
                    {
                        Cycle = 1,
                        Level = 1,
                        UserId = bunny
                    }
                }
                .ToDictionary(lvl => lvl.UserId);
                levels.ForAll((cnt, bitlevel) =>
                {
                    cxt.Add(bitlevel.Value).Context.CommitChanges();
                });

                var transactions = new[]
                {
                    new BlockChainTransaction
                    {
                        Amount = 0m,
                        ContextId = levels[Constants.SystemUsers_Apex].Id.ToString(),
                        ContextType = Constants.TransactionContext_UpgradeBitLevel,
                        LedgerCount = int.MaxValue,
                        SenderId = addresses[Constants.SystemUsers_Apex].Id,
                        ReceiverId = addresses[Constants.SystemUsers_Apex].Id,
                        Status = BlockChainTransactionStatus.Verified,
                        TransactionHash = ""
                    },

                    //others,
                    new BlockChainTransaction
                    {
                        Amount = 0.3652m,
                        ContextId = levels[gerald].Id.ToString(),
                        ContextType = Constants.TransactionContext_UpgradeBitLevel,
                        SenderId = addresses[gerald].Id,
                        ReceiverId = addresses[Constants.SystemUsers_Apex].Id,
                        Status = BlockChainTransactionStatus.Unverified
                    },
                    new BlockChainTransaction
                    {
                        Amount = 0.1826m,
                        ContextId = levels[onotu].Id.ToString(),
                        ContextType = Constants.TransactionContext_UpgradeBitLevel,
                        SenderId = addresses[onotu].Id,
                        ReceiverId = addresses[Constants.SystemUsers_Apex].Id,
                        Status = BlockChainTransactionStatus.Unverified
                    },
                    new BlockChainTransaction
                    {
                        Amount = 0.1826m,
                        ContextId = levels[valejoma].Id.ToString(),
                        ContextType = Constants.TransactionContext_UpgradeBitLevel,
                        SenderId = addresses[valejoma].Id,
                        ReceiverId = addresses[gerald].Id,
                        Status = BlockChainTransactionStatus.Unverified
                    },
                    new BlockChainTransaction
                    {
                        Amount = 0.1826m,
                        ContextId = levels[bunny].Id.ToString(),
                        ContextType = Constants.TransactionContext_UpgradeBitLevel,
                        SenderId = addresses[bunny].Id,
                        ReceiverId = addresses[gerald].Id,
                        Status = BlockChainTransactionStatus.Unverified
                    }
                }
                .ToDictionary(trnx => trnx.SenderId);
                transactions.ForAll((cnt, next) =>
                {
                    cxt.Add(next.Value).Context.CommitChanges();
                });

                levels.ForAll((cnt, bitlevel) =>
                {
                    bitlevel.Value.DonationId = transactions[addresses[bitlevel.Key].Id].Id;
                    cxt.Modify(bitlevel.Value).Context.CommitChanges();
                });

                //referal manager
                new[]
                {
                    new ReferralNode
                    {
                        ReferenceCode = ReferralHelper.GenerateCode(Constants.SystemUsers_Apex),
                        UserId = Constants.SystemUsers_Apex
                    },

                    //others,
                    new ReferralNode
                    {
                        ReferenceCode = ReferralHelper.GenerateCode(gerald),
                        ReferrerCode = ReferralHelper.GenerateCode(Constants.SystemUsers_Apex),
                        UplineCode = ReferralHelper.GenerateCode(Constants.SystemUsers_Apex),
                        UserId = gerald
                    },
                    new ReferralNode
                    {
                        ReferenceCode = ReferralHelper.GenerateCode(onotu),
                        ReferrerCode = ReferralHelper.GenerateCode(Constants.SystemUsers_Apex),
                        UplineCode = ReferralHelper.GenerateCode(Constants.SystemUsers_Apex),
                        UserId = onotu
                    },
                    new ReferralNode
                    {
                        ReferenceCode = ReferralHelper.GenerateCode(valejoma),
                        ReferrerCode = ReferralHelper.GenerateCode(gerald),
                        UplineCode = ReferralHelper.GenerateCode(gerald),
                        UserId = valejoma
                    },
                    new ReferralNode
                    {
                        ReferenceCode = ReferralHelper.GenerateCode(bunny),
                        ReferrerCode = ReferralHelper.GenerateCode(gerald),
                        UplineCode = ReferralHelper.GenerateCode(gerald),
                        UserId = bunny
                    }
                }
                .ForAll((cnt, next) =>
                {
                    cxt.Add(next).Context.CommitChanges();
                });

                cxt.CommitChanges();
            });
            #endregion

        }
    }
}
