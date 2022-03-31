using ScheduleJobs.Interfaces;
using System.Net;
using System.Net.Mail;

namespace ScheduleJobs.Services
{
    public class MailService : IMailService
    {
        private readonly IMailSettings _mailSettings;

        public MailService(IMailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }
        public async Task SendMail(string email, string subject, string body)
        {
            try
            {
                MailMessage message = new MailMessage(_mailSettings.Sender, email, subject, body);
                var client = new SmtpClient(_mailSettings.Host, _mailSettings.Port);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_mailSettings.Username, _mailSettings.Password);

                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }
    }
}
