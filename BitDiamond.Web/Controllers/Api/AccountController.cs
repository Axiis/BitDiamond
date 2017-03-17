using Axis.Luna;
using Axis.Luna.Extensions;
using Axis.Pollux.Authentication;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Services;
using BitDiamond.Core.Utils;
using BitDiamond.Web.Infrastructure.Exceptions;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Web.Http;

using static Axis.Luna.Extensions.ExceptionExtensions;
using BitDiamond.Web.Infrastructure.Utils;

namespace BitDiamond.Web.Controllers.Api
{
    public class AccountController : ApiController
    {
        private IAccountManager _account = null;

        #region init
        public AccountController(IAccountManager accountManager)
        {
            ThrowNullArguments(() => accountManager);

            _account = accountManager;
        }
        #endregion

        #region Account
        [HttpGet, Route("api/accounts/users/count")]
        public IHttpActionResult UserCount()
        => this.LogTime(() => _account.UserCount().OperationResult(Request));


        [HttpPost, Route("api/accounts/users")]
        public IHttpActionResult RegisterUser([FromBody] AccountControllerModels.RegisterUserArgs data)
        => this.LogTime(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _account.RegisterUser(data.TargetUser, data.Referrer, data.Credential))
            .OperationResult(Request));

        [HttpPost, Route("api/accounts/admins")]
        public IHttpActionResult RegisterAdminUser([FromBody] AccountControllerModels.RegisterUserArgs data)
        => this.LogTime(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _account.RegisterAdminUser(data.TargetUser, data.Credential))
            .OperationResult(Request));

        [HttpPut, Route("api/accounts/users/deactivate")]
        public IHttpActionResult DeactivateUser([FromBody] AccountControllerModels.UserEmailArgs data)
        => this.LogTime(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _account.DeactivateUser(data.TargetUser))
            .OperationResult(Request));

        [HttpPut, Route("api/accounts/users/block")]
        public IHttpActionResult BlockUser([FromBody] AccountControllerModels.UserEmailArgs data)
        => this.LogTime(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _account.BlockUser(data.TargetUser))
            .OperationResult(Request));


        [HttpPut, Route("api/accounts/users/activations")]
        public IHttpActionResult RequestUserActivation([FromBody] AccountControllerModels.UserEmailArgs data)
        => this.LogTime(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _account.RequestUserActivation(data.TargetUser))
            .OperationResult(Request));

        [HttpPut, Route("api/accounts/users/activations/verify")]
        public IHttpActionResult VerifyUserActivation([FromBody] AccountControllerModels.UserActivationArgs data)
        => this.LogTime(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _account.VerifyUserActivation(data.TargetUser, data.Token))
            .OperationResult(Request));


        [HttpPut, Route("api/accounts/users/credentials/reset-tokens")]
        public IHttpActionResult RequestCredentialReset([FromBody] AccountControllerModels.CredentialUpdateArgs data)
        => this.LogTime(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _account.RequestCredentialReset(data.Metadata, data.TargetUser))
            .OperationResult(Request));

        [HttpPut, Route("api/accounts/users/credentials/reset-tokens/verify")]
        public IHttpActionResult ResetCredential([FromBody] AccountControllerModels.CredentialUpdateArgs data)
        => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _account.ResetCredential(data.New, data.Token, data.TargetUser))
            .OperationResult(Request);

        [HttpPut, Route("api/accounts/users/logons/invalidate")]
        public IHttpActionResult InvalidateLogon([FromBody] AccountControllerModels.LogonArgs data)
        => this.LogTime(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _account.InvalidateLogon(data.Token))
            .OperationResult(Request));

        [HttpGet, Route("api/accounts/users/roles")]
        public IHttpActionResult GetRoles()
        => this.LogTime(() => Operation.Try(() => _account.GetRoles())
            .OperationResult(Request));

        [HttpGet, Route("api/accounts/users/current")]
        public IHttpActionResult GetSignedInUser()
        => this.LogTime(() => Operation.Try(() => _account.CurrentUser())
            .OperationResult(Request));
        #endregion

        #region Biodata
        [HttpPut, Route("api/accounts/biodata")]
        public IHttpActionResult UpdateBioData([FromBody] BioData data)
        => this.LogTime(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _account.UpdateBioData(data))
            .OperationResult(Request));

        [HttpGet, Route("api/accounts/biodata")]
        public IHttpActionResult GetBioData()
        => this.LogTime(() => _account.GetBioData().OperationResult(Request));
        #endregion

        #region Contact data
        [HttpPut, Route("api/accounts/contacts")]
        public IHttpActionResult UpdateContactData([FromBody] ContactData data)
        => this.LogTime(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _account.UpdateContactData(data))
            .OperationResult(Request));

        [HttpGet, Route("api/accounts/contacts")]
        public IHttpActionResult GetContactData()
        => this.LogTime(() => _account.GetContactData().OperationResult(Request));
        #endregion

        #region User data
        [HttpPost, Route("api/accounts/userdata")]
        public IHttpActionResult AddData([FromBody] AccountControllerModels.UserDataArgs data)
        => this.LogTime(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _account.AddData(data.Data))
            .OperationResult(Request));

        [HttpPut, Route("api/accounts/userdata")]
        public IHttpActionResult UpdateData([FromBody] AccountControllerModels.UserDataArgs data)
        => this.LogTime(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _account.UpdateData(data.Data))
            .OperationResult(Request));

        [HttpDelete, Route("api/accounts/userdata")]
        public IHttpActionResult RemoveData(string data)
        => this.LogTime(() => Operation.Try(() => ThrowIfFail(() => Encoding.UTF8.GetString(Convert.FromBase64String(data)), ex => new MalformedApiArgumentsException()))
            .Then(_jopr => ThrowIfFail(() => JsonConvert.DeserializeObject<AccountControllerModels.UserDataArgs>(_jopr.Result, Constants.Misc_DefaultJsonSerializerSettings), ex => new MalformedApiArgumentsException()))
            .Then(argopr => _account.RemoveData(argopr.Result.Names))
            .OperationResult(Request));

        [HttpGet, Route("api/accounts/userdata")]
        public IHttpActionResult GetUserData()
        => this.LogTime(() => _account.GetUserData().OperationResult(Request));

        [HttpGet, Route("api/accounts/userdata/filter")]
        public IHttpActionResult GetUserData(string data)
        => this.LogTime(() => Operation.Try(() => ThrowIfFail(() => Encoding.UTF8.GetString(Convert.FromBase64String(data)), ex => new MalformedApiArgumentsException()))
            .Then(_jopr => ThrowIfFail(() => JsonConvert.DeserializeObject<AccountControllerModels.UserDataArgs>(_jopr.Result, Constants.Misc_DefaultJsonSerializerSettings), ex => new MalformedApiArgumentsException()))
            .Then(argopr => _account.GetUserData(argopr.Result.Name))
            .OperationResult(Request));

        [HttpPut, Route("api/accounts/profile-images")]
        public IHttpActionResult UpdateProfileImage([FromBody] AccountControllerModels.ProfileImageArgs data)
        => this.LogTime(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _account.UpdateProfileImage(data.Image, data.OldImageUrl))
            .OperationResult(Request));
        #endregion
    }


    namespace AccountControllerModels
    {
        public class LogonArgs
        {
            public string Token { get; set; }
        }

        public class RegisterUserArgs
        {
            public string TargetUser { get; set; }
            public string Referrer { get; set; }
            public Credential Credential { get; set; }
        }

        public class UserEmailArgs
        {
            public string TargetUser { get; set; }
        }

        public class UserActivationArgs
        {
            public string TargetUser { get; set; }
            public string Token { get; set; }
        }

        public class UserDataArgs
        {
            public UserData[] Data { get; set; }
            public string[] Names { get; set; }
            public string Name { get; set; }
        }

        public class ProfileImageArgs
        {
            public EncodedBinaryData Image { get; set; }
            public string OldImageUrl { get; set; }
        }

        public class CredentialUpdateArgs
        {
            public Credential Old { get; set; }
            public Credential New { get; set; }
            public string Token { get; set; }
            public string TargetUser { get; set; }
            public CredentialMetadata Metadata { get; set; }
        }
    }
}
