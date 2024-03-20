using Aukcionas.Models;

namespace Aukcionas.Utils
{
    public interface IEmailService
    {
        void SendEmail(EmailModel emailModel);
    }
}
