using Axis.Luna;
using BitDiamond.Core.Models.Email;

namespace BitDiamond.Core.Services
{
    public interface IEmailPush
    {
        Operation SendMail(MailModel model);
    }
}
