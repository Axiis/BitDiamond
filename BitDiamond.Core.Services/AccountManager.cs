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
using BitDiamond.Core.Services.Command;
using System.Linq;
using Axis.Luna.Extensions;
using BitDiamond.Core.Models.Email;

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
        private IApiLinkProvider _apiProvider;
        private IReferalManager _refManager;


        #region Init
        public AccountManager(IUserContext userContext, IAccountQuery dataContext,
                              ICredentialAuthentication credentialAuthentication,
                              IContextVerifier contextVerifier,
                              ISettingsManager settingsManager,
                              IUserAuthorization accessManager,
                              IPersistenceCommands pcommands,
                              IApiLinkProvider apiProvider,
                              IReferalManager refManager,
                              IEmailPush messagePush,
                              IBlobStore blobStore)
        {
            ThrowNullArguments(() => userContext,
                               () => dataContext,
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
            _query = dataContext;
            _credentialAuth = credentialAuthentication;
            _contextVerifier = contextVerifier;
            _authorizer = accessManager;
            _blobStore = blobStore;
            _messagePush = messagePush;
            _settingsManager = settingsManager;
            _apiProvider = apiProvider;
            _refManager = refManager;
        }
        #endregion

        #region Account
        public Operation RegisterUser(string targetUser, string referee, Credential[] secretCredentials)
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
            {
                var user = _query.GetUserById(targetUser);
                if (user != null) throw new Exception($"{targetUser} already exists");

                else if (secretCredentials == null || secretCredentials.Length == 0)
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

                            secretCredentials.Where(scred => scred.Metadata == CredentialMetadata.Password)
                                             .ForAll((cnt, cred) =>
                                             {
                                                 //cred.ExpiresIn = passwordExpiration.Ticks;
                                                 cred.ExpiresIn = null; //<-- never expires
                                             });

                            secretCredentials.ForAll((cnt, cred) =>
                            {
                                _credentialAuth.AssignCredential(targetUser, cred)
                                    .ThrowIf(op => !op.Succeeded, op => new Exception("failed to assign credential"));
                            });
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
                            return _refManager.AffixNewUser(user, referee);
                            #endregion
                        })
                        .Then(opr =>
                        {
                            #region Request context verification
                            RequestUserActivation(user.UserId).Resolve();
                            #endregion
                        });
                }
            });

        public Operation RegisterAdminUser(string targetUser, Credential[] secretCredentials)
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
            {
                var user = _query.GetUserById(targetUser);
                if (user != null) throw new Exception($"{targetUser} already exists");

                else if (secretCredentials == null || secretCredentials.Length == 0)
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

                            secretCredentials.Where(scred => scred.Metadata == CredentialMetadata.Password)
                                             .ForAll((cnt, cred) =>
                                             {
                                                 //cred.ExpiresIn = passwordExpiration.Ticks;
                                                 cred.ExpiresIn = null; //<-- never expires
                                             });

                            secretCredentials.ForAll((cnt, cred) =>
                            {
                                _credentialAuth.AssignCredential(targetUser, cred)
                                    .ThrowIf(op => !op.Succeeded, op => new Exception("failed to assign credential"));
                            });
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
                            RequestUserActivation(user.UserId).Resolve();
                            #endregion
                        });
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
                if (!verification.Verified) return verification;

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
                                     Target = user.UserId,
                                     Link = _apiProvider.GenerateContextVerificationLink(verification.VerificationToken).Result
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

                var verification = _query.GetContextVerification(user, contextToken);
                if (verification.Verified || 
                    verification.Context != Constants.VerificationContext_UserActivation) throw new Exception("invalid verification");
                else
                {
                    verification.Verified = true;
                    _pcommand.Update(verification).Resolve();
                    return user;
                }
            });
        #endregion

        #region Biodata
        public Operation<BioData> ModifyBioData(BioData data)
        {

        }

        public Operation<BioData> GetBioData()
        {

        }
        #endregion

        #region Contact data

        public Operation<ContactData> ModifyContactData(ContactData data)
        {

        }

        public Operation<IEnumerable<ContactData>> RemoveContactData(long[] ids)
        {

        }

        public Operation<IEnumerable<ContactData>> GetContactData()
        {

        }
        #endregion

        #region User data
        public Operation<IEnumerable<UserData>> AddData(UserData[] data)
        {

        }

        public Operation<IEnumerable<UserData>> RemoveData(string[] names)
        {

        }

        public Operation<IEnumerable<UserData>> GetUserData()
        {

        }

        public Operation<UserData> GetUserData(string name)
        {

        }

        public Operation<string> UpdateProfileImage(EncodedBinaryData image, string oldImageUrl)
        {

        }
        #endregion
    }
}
