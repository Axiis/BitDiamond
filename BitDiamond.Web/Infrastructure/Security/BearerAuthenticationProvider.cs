using Axis.Jupiter;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Linq;
using System.Threading.Tasks;
using UAParser;
using static Axis.Luna.Extensions.ExceptionExtensions;
using static Axis.Luna.Extensions.ObjectExtensions;

namespace BitDiamond.Web.Infrastructure.Security
{
    public class BearerAuthenticationProvider: OAuthBearerAuthenticationProvider
    {
        private IDataContext _dataContext = null;
        private Parser Parser = Parser.GetDefault();

        public BearerAuthenticationProvider(IDataContext dataContext)
        {
            ThrowNullArguments(() => dataContext);

            _dataContext = dataContext;

            //make sure a logon hasnt been invalidated!
            OnValidateIdentity = context => Task.Run(() =>
            {
                var token = context.Request.Headers.Get("Authorization").TrimStart("Bearer").Trim();
                var agent = Parser.Parse(context.Request.Headers.Get("User-Agent"));

                //in future, a realtime event will notify the bearer-provider of changes to a logon, so we dont need to keep quering the database
                var logon = _dataContext.Store<UserLogon>()
                                        .QueryWith(_ul => _ul.User)
                                        .Where(_ul => _ul.User.EntityId == context.Ticket.Identity.Name)
                                        .Where(_ul => _ul.OwinToken == token) //get the bearer token from the header
                                        .FirstOrDefault();

                if (logon == null)
                {
                    //create a new logon
                    _dataContext.Store<UserLogon>()
                                .Add(new UserLogon
                                {
                                    User = new User { EntityId = context.Ticket.Identity.Name },
                                    Client = new Core.Models.UserAgent
                                    {
                                        OS = agent.OS.Family,
                                        OSVersion = $"{agent.OS.Major}.{agent.OS.Minor}",

                                        Browser = agent.UserAgent.Family,
                                        BrowserVersion = $"{agent.UserAgent.Major}.{agent.OS.Minor}",

                                        Device = $"{agent.Device.Family}"
                                    },
                                    OwinToken = token,
                                    Location = null,

                                    ModifiedOn = DateTime.Now
                                })
                                .Context.CommitChanges();

                    context.Validated();
                }
                else if (logon.Invalidated) context.Rejected();
                else
                {
                    context.Validated();

                    //somehow implement "last active" by changing the "ModifiedOn" field of the Logon object
                    //do this asynchroniously if possible for speed sake
                    _dataContext.Store<UserLogon>().Modify(logon.With(new { ModifiedOn = DateTime.Now }), true);
                }
            });
        }
    }
}