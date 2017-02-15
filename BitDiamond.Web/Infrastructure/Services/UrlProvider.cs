using Axis.Luna;
using BitDiamond.Core.Services;
using BitDiamond.Web.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitDiamond.Web.Infrastructure.Services
{
    public class UrlProvider: IAppUrlProvider
    {
        private OwinContextProvider _owinProvider;

        public UrlProvider(OwinContextProvider owinProvider)
        {
            _owinProvider = owinProvider;
        }

        public Operation<string> GenerateContextVerificationApiUrl(string verificationToken, string targetUser)
        => Operation.Try(() =>
        {
            var ruri = _owinProvider.Owin.Request.Uri;
            var email = targetUser.Split('@');
            return new Uri($"{ruri.Scheme}://{ruri.Authority}/Account/login#/verify-registration/{verificationToken}/{email[0]}/{email[1]}").ToString();
        });

        public Operation<string> GenerateBlobUrl(string blobName)
        => Operation.Try(() =>
        {
            var ruri = _owinProvider.Owin.Request.Uri;
            return new Uri($"{ruri.Scheme}://{ruri.Authority}/Content/Blob/{blobName}").ToString();
        });
    }
}