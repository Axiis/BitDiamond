using Axis.Luna;
using BitDiamond.Core.Services;
using System;
using System.Text;

namespace BitDiamond.Web.Infrastructure.Services
{
    public class UrlProvider: IAppUrlProvider
    {
        private OwinContextProvider _owinProvider;

        public UrlProvider(OwinContextProvider owinProvider)
        {
            _owinProvider = owinProvider;
        }

        public Operation<string> GenerateUserActivationVerificationUrl(string verificationToken, string targetUser)
        => Operation.Try(() =>
        {
            var ruri = _owinProvider.Owin.Request.Uri;
            var json = $"{{\"Email\":\"{targetUser}\", \"Token\":\"{verificationToken}\"}}";
            var utf8Array = Encoding.UTF8.GetBytes(json);
            var urlB64 = Convert.ToBase64String(utf8Array).Replace("/", "_").Replace("+", "-");
            return new Uri($"{ruri.Scheme}://{ruri.Authority}/account/index#!/verify-registration/{urlB64}").ToString();
        });

        public Operation<string> GenerateBlobUrl(string blobName)
        => Operation.Try(() =>
        {
            var ruri = _owinProvider.Owin.Request.Uri;
            return new Uri($"{ruri.Scheme}://{ruri.Authority}/Content/Blob/{blobName}").ToString();
        });

        public Operation<string> GeneratePasswordUpdateVerificationUrl(string verificationToken, string user)
        => Operation.Try(() =>
        {
            var ruri = _owinProvider.Owin.Request.Uri;
            var json = $"{{\"Email\":\"{user}\", \"Token\":\"{verificationToken}\"}}";
            var utf8Array = Encoding.UTF8.GetBytes(json);
            var urlB64 = Convert.ToBase64String(utf8Array).Replace("/", "_").Replace("+", "-");
            return new Uri($"{ruri.Scheme}://{ruri.Authority}/account/index#!/recover-password/{urlB64}").ToString();
        });

        public Operation<string> GenerateWelcomeMessageUrl()
        => Operation.Try(() =>
        {
            var ruri = _owinProvider.Owin.Request.Uri;
            return new Uri($"{ruri.Scheme}://{ruri.Authority}/account/index").ToString();
        });
    }
}