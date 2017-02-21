using System;
using System.Collections.Generic;
using Axis.Luna;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using BitDiamond.Core.Services.Query;
using Axis.Pollux.Authentication.Service;
using Axis.Pollux.RBAC.Services;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Pollux.Authentication;
using BitDiamond.Core.Utils;
using System.Linq;
using Axis.Luna.Extensions;
using BitDiamond.Core.Models.Email;
using Axis.Jupiter.Kore.Command;

namespace BitDiamond.Core.Services
{
    public class AccountManager : IAccountManager, IUserContextAware
    {
        public IUserContext UserContext { get; private set; }
        private IAccountQuery _query;
        private ICredentialAuthentication _credentialAuth ;
        private IContextVerifier _contextVerifier ;
        private IBlobStore _blobStore ;
        private IEmailPush _messagePush ;
        private IUserAuthorization _authorizer;
        private IPersistenceCommands _pcommand;
        private ISettingsManager _settingsManager;
        private IAppUrlProvider _apiProvider;
        private IReferralManager _refManager;


        #region Init
        public AccountManager(ICredentialAuthentication credentialAuthentication,
                              IContextVerifier contextVerifier,
                              ISettingsManager settingsManager,
                              IUserAuthorization accessManager,
                              IPersistenceCommands pcommands,
                              IAppUrlProvider apiProvider,
                              IReferralManager refManager, 
                              IUserContext userContext,
                              IEmailPush messagePush,
                              IBlobStore blobStore,
                              IAccountQuery query)
        {
            ThrowNullArguments(() => userContext,
                               () => query,
                               () => credentialAuthentication,
                               () => contextVerifier,
                               () => accessManager,
                               () => blobStore,
                               () => messagePush,
                               () => pcommands,
                               () => settingsManager,
                               () => apiProvider,
                               () => refManager);

            UserContext = userContext;
            _query = query;
            _credentialAuth = credentialAuthentication;
            _contextVerifier = contextVerifier;
            _authorizer = accessManager;
            _blobStore = blobStore;
            _messagePush = messagePush;
            _settingsManager = settingsManager;
            _apiProvider = apiProvider;
            _refManager = refManager;
            _pcommand = pcommands;
        }
        #endregion

        #region Account
        public Operation<User> RegisterUser(string targetUser, string referrer, Credential secretCredential)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var user = _query.GetUserById(targetUser);

            if (user != null)
                throw new Exception($"{targetUser} already exists");

            else if (secretCredential == null)
                throw new Exception("user registration must contain a credential");

            else if (_query.GetRefNode(referrer) == null)
                throw new Exception("invalid referee");

            else
            {
                #region Create user
                user = new User
                {
                    UserId = targetUser,
                    Status = (int)AccountStatus.InActive
                };
                #endregion
                return _pcommand
                    .Add(user) //add user
                    .Then(opr =>
                    {
                        #region Assign credentials (password)
                        //assign credentials
                        //load all credential expiration dates from settings
                        var passwordExpiration = _settingsManager.GetSetting(Constants.Settings_DefaultPasswordExpirationTime)
                                                                 .Resolve()
                                                                 .ParseData<TimeSpan>();

                        secretCredential.ExpiresIn = null; //<-- never expires

                        return _credentialAuth.AssignCredential(targetUser, secretCredential);
                        #endregion
                    })
                    .Then(opr =>
                    {
                        #region Assign role
                        return _authorizer.AssignRole(user, Constants.Roles_BitMemberRole);
                        #endregion
                    })
                    .Then(opr =>
                    {
                        #region create default bit level
                        var bitlevel = new BitLevel
                        {
                            Cycle = 1,
                            DonationCount = 0,
                            Level = 0,
                            SkipCount = 0,
                            User = user
                        };
                        return _pcommand.Add(bitlevel);
                        #endregion
                    })
                    .Then(opr =>
                    {
                        #region Place the user in the referal hierarchy
                        return _refManager.AffixNewUser(user.UserId, referrer);
                        #endregion
                    })
                    .Then(opr =>
                    {
                        #region Request context verification
                        return RequestUserActivation(user.UserId);
                        #endregion
                    })
                    .Then(opr => user);
            }
        });

        public Operation<User> RegisterAdminUser(string targetUser, Credential secretCredential)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            var user = _query.GetUserById(targetUser);
            if (user != null) throw new Exception($"{targetUser} already exists");

            else if (secretCredential == null)
                throw new Exception("user registration must contain a credential");

            else
            {
                #region Create user
                user = new User
                {
                    UserId = targetUser,
                    Status = (int)AccountStatus.InActive
                };
                #endregion
                return _pcommand
                    .Add(user) //add user
                    .Then(opr =>
                    {
                        #region Assign credentials (password)
                        //assign credentials
                        //load all credential expiration dates from settings
                        var passwordExpiration = _settingsManager.GetSetting(Constants.Settings_DefaultPasswordExpirationTime)
                                                                    .Resolve()
                                                                    .ParseData<TimeSpan>();

                        secretCredential.ExpiresIn = null; //<-- never expires

                        return _credentialAuth.AssignCredential(targetUser, secretCredential);
                        #endregion
                    })
                    .Then(opr =>
                    {
                        #region Assign role
                        return _authorizer.AssignRole(user, Constants.Roles_BitMemberRole);
                        #endregion
                    })
                    .Then(opr =>
                    {
                        #region Request context verification
                        return RequestUserActivation(user.UserId);
                        #endregion
                    })
                    .Then(opr => user);
            }
        });


        public Operation<User> DeactivateUser(string targetUser)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            var user = _query.GetUserById(targetUser);
            if (user == null) throw new Exception("user not found");
            else if (user.Status == (int)AccountStatus.Blocked) throw new Exception("user is already blocked");
            else if (user.Status == (int)AccountStatus.Active)
            {
                //invalidate all userlogons
                var logons = _query.GetUserLogins(targetUser);
                logons.Where(_l => !_l.Invalidated).ForAll((cnt, next) =>
                {
                    next.Invalidated = true;
                    _pcommand.Update(next);
                });

                //deactivate the user
                user.Status = (int)AccountStatus.InActive;
                return _pcommand.Update(user).Resolve();
            }
            else return user;
        });

        public Operation<User> BlockUser(string targetUser)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            var user = _query.GetUserById(targetUser);
            if (user == null) throw new Exception("user not found");
            else if (user.Status == (int)AccountStatus.Blocked) throw new Exception("user is already blocked");
            else 
            {
                //invalidate all userlogons
                var logons = _query.GetUserLogins(targetUser);
                logons.Where(_l => !_l.Invalidated).ForAll((cnt, next) =>
                {
                    next.Invalidated = true;
                    _pcommand.Update(next);
                });

                //deactivate the user
                user.Status = (int)AccountStatus.Blocked;
                return _pcommand.Update(user).Resolve();
            }
        });

        public Operation<ContextVerification> RequestUserActivation(string targetUser)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            var user = _query.GetUserById(targetUser);
            if (user.Status != (int)AccountStatus.InActive) throw new Exception("invalid account state");

            var verification = _query.GetLatestContextVerification(user, Constants.VerificationContext_UserActivation);

            //if an unverified context still exists in the db, return that
            if (verification != null && !verification.Verified) return verification;

            //else create a new one
            else
            {
                var expiry = _settingsManager.GetSetting(Constants.Settings_DefaultContextVerificationExpirationTime)
                                                .Resolve()
                                                .ParseData<TimeSpan>();
                verification = new ContextVerification
                {
                    Context = Constants.VerificationContext_UserActivation,
                    Target = user,
                    ExpiresOn = DateTime.Now + expiry,
                    VerificationToken = RandomAlphaNumericGenerator.RandomAlphaNumeric(50)
                };

                _pcommand.Add(verification)
                         .Then(opr =>
                         {
                             return _messagePush.SendMail(new AccountActivation
                             {
                                 From = "donotreply@bitdiamond.com",
                                 Subject = "Account - Email Verification",
                                 Target = user.UserId,
                                 Link = _apiProvider.GenerateUserActivationVerificationUrl(verification.VerificationToken, targetUser).Result
                             });
                         })
                         .Resolve();

                return verification;
            }
        });

        public Operation<User> VerifyUserActivation(string targetUser, string contextToken)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            var user = _query.GetUserById(targetUser);
            if (user == null) throw new Exception("invalid user");

            var verification = _query.GetContextVerification(user, Constants.VerificationContext_UserActivation, contextToken);
            if (verification.Verified || 
                verification.Context != Constants.VerificationContext_UserActivation) throw new Exception("invalid verification");
            else
            {
                verification.Verified = true;
                _pcommand.Update(verification).Resolve();

                user.Status = AccountStatus.Active.As<int>();
                return user;
            }
        });


        public Operation ResetCredential(Credential @new, string token, string targetUser)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _contextVerifier.VerifyContext(targetUser, Constants.VerificationContext_CredentialReset, token)
                                   .Then(opr => _credentialAuth.AssignCredential(targetUser, @new));
        });

        public Operation RequestCredentialReset(CredentialMetadata metadata, string targetUser)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            var user = _query.GetUserById(targetUser);
            if (user.Status != (int)AccountStatus.Active) throw new Exception("invalid account state");

            var verification = _query.GetLatestContextVerification(user, Constants.VerificationContext_CredentialReset);

            //if no unverified context still exists in the db, create a new one
            if (verification == null || verification.Verified) 
            {
                var expiry = _settingsManager.GetSetting(Constants.Settings_DefaultContextVerificationExpirationTime)
                                             .Resolve()
                                             .ParseData<TimeSpan>();
                verification = new ContextVerification
                {
                    Context = Constants.VerificationContext_CredentialReset,
                    Target = user,
                    ExpiresOn = DateTime.Now + expiry,
                    VerificationToken = RandomAlphaNumericGenerator.RandomAlphaNumeric(50)
                };

                _pcommand.Add(verification).Resolve();
            }

            return _messagePush.SendMail(new AccountActivation
            {
                Target = user.UserId,
                Link = _apiProvider.GenerateUserActivationVerificationUrl(verification.VerificationToken, targetUser).Result
            });
        });
        #endregion

        #region Biodata
        public Operation<BioData> UpdateBioData(BioData data) 
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            var user = UserContext.CurrentUser();
            var persisted = _query.GetBioData(user);

            if (persisted != null)
            {
                data.CopyTo(persisted,
                            nameof(BioData.OwnerId),
                            nameof(BioData.Owner),
                            nameof(BioData.CreatedOn),
                            nameof(BioData.ModifiedOn));

                return _pcommand.Update(persisted);
            }
            else return _pcommand.Add(persisted);
        });

        public Operation<BioData> GetBioData()
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            return _query.GetBioData(UserContext.CurrentUser());
        });
        #endregion

        #region Contact data
        public Operation<ContactData> UpdateContactData(ContactData data)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            var user = UserContext.CurrentUser();
            var persisted = _query.GetContactData(user);

            if (persisted != null)
            {
                data.CopyTo(persisted,
                            nameof(ContactData.OwnerId),
                            nameof(ContactData.Owner),
                            nameof(ContactData.CreatedOn),
                            nameof(ContactData.ModifiedOn));

                return _pcommand.Update(persisted);
            }
            else return _pcommand.Add(persisted);
        });

        public Operation<ContactData> GetContactData()
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            return _query.GetContactData(UserContext.CurrentUser());
        });
        #endregion

        #region User data
        public Operation<IEnumerable<UserData>> AddData(UserData[] data)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            var user = UserContext.CurrentUser();
            return data.Select(_data =>
            {
                if (_query.GetUserData(user, _data.Name) != null) return null;

                _data.EntityId = 0;
                _data.Owner = user;
                _data.OwnerId = user.UserId;

                return _pcommand.Add(_data).Resolve();
            })
            .Where(_data => _data != null)
            .ToArray()
            .AsEnumerable();
        });
        public Operation<IEnumerable<UserData>> UpdateData(UserData[] data)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            var user = UserContext.CurrentUser();
            return data.Select(_data =>
            {
                var persisted = _query.GetUserData(user, _data.Name);
                if (persisted == null) return null;

                _data.CopyTo(persisted,
                             nameof(UserData.Owner),
                             nameof(UserData.OwnerId),
                             nameof(UserData.CreatedOn),
                             nameof(UserData.ModifiedOn));

                return _pcommand.Update(_data).Resolve();
            })
            .Where(_data => _data != null)
            .ToArray()
            .AsEnumerable();
        });

        public Operation<IEnumerable<UserData>> RemoveData(string[] names)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            var user = UserContext.CurrentUser();
            return names.Select(_name =>
            {
                var data = _query.GetUserData(user, _name);
                if (data == null) return null;
                else return _pcommand.Delete(data).Resolve();
            })
            .Where(_data => _data != null)
            .ToArray()
            .AsEnumerable();
        });

        public Operation<IEnumerable<UserData>> GetUserData()
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            return _query.GetUserData(UserContext.CurrentUser());
        });

        public Operation<UserData> GetUserData(string name)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            return _query.GetUserData(UserContext.CurrentUser(), name);
        });

        public Operation<string> UpdateProfileImage(EncodedBinaryData image, string oldImageUrl)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            var user = UserContext.CurrentUser();

            var url = _blobStore.Delete(oldImageUrl) //shouldnt fail even if oldImageUrl is null
                                .Then(opr => _blobStore.Persist(image))
                                .Resolve();

            //create the ProfileImage UserData and store the url
            var userData = _query.GetUserData(user, Constants.UserData_ProfileImage) ?? new UserData
            {
                Name = Constants.UserData_ProfileImage,
                Owner = user,
                Type = CommonDataType.Url
            };

            //set the data
            userData.Data = url;

            if (userData.EntityId > 0)
                return _pcommand.Update(userData).Resolve().Data;

            else
                return _pcommand.Add(userData).Resolve().Data;
        });
        #endregion
    }
}
