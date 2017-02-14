using Axis.Jupiter;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Web.Infrastructure.Security
{
    public class BearerAuthenticationProvider: OAuthBearerAuthenticationProvider
    {
        private IDataContext _dataContext = null;

        public BearerAuthenticationProvider(IDataContext dataContext)
        {
            ThrowNullArguments(() => dataContext);

            _dataContext = dataContext;

            ///Accepts the 
            OnRequestToken = context => Task.Run(() =>
            {

            });

            OnValidateIdentity = context => Task.Run(() =>
            {

            });
        }
    }
}