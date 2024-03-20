using Aukcionas.Models;
using Aukcionas.Utils;
using MailKit.Net.Smtp;
using MimeKit;

namespace Aukcionas.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration configuration) 
        {
            _config = configuration;
        }
        public void SendEmail(EmailModel emailModel)
        {
            var emailMessage = new MimeMessage();
            var from = _config["EmailConfiguration:From"];
            emailMessage.From.Add(new MailboxAddress("Reset Password", from));
            emailMessage.To.Add(new MailboxAddress(emailModel.To, emailModel.To));
            emailMessage.Subject = emailModel.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = string.Format(emailModel.Content)
            };
            using(var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_config["EmailConfiguration:SmtpServer"], 465, true);
                    client.Authenticate(_config["EmailConfiguration:From"], _config["EmailConfiguration:Password"]);
                    client.Send(emailMessage);
                }
                catch (Exception ex) 
                {
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }

        }
    }
}
