using System.Net.Mail;
using System.Net;

namespace NotificationServiceAPI.Helpers
{
    public class EmailService : IEmailService
    {
        
        
        private readonly SmtpClient _smtpClient;

        public EmailService()
        {
            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("danilonok1906@gmail.com", "igfcdtcndtstxwsn"),
                EnableSsl = true,
                Timeout = 30000
            };
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var mailMessage = new MailMessage("danilonok1906@gmail.com", toEmail, subject, body);
            await _smtpClient.SendMailAsync(mailMessage);
        }
        
    }
}
