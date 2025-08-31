using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Games.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmail(string subject, string body, string toEmail)
        {
            var emailSettings = _config.GetSection("EmailSettings");

            var message = new MailMessage();
            message.From = new MailAddress(emailSettings["SenderEmail"], emailSettings["SenderName"]);
            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = false;

            using var client = new SmtpClient(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]));
            client.Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]);
            client.EnableSsl = true;

            await client.SendMailAsync(message);
        }
    }
}