using MimeKit;
using MailKit.Net.Smtp;

namespace WebRegistry.Services.EmailSender
{
    public class EmailSender:IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "portal@sial-group.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("EXCH.sial-group.ru", 25, false);
                await client.AuthenticateAsync("portal", "Z3delv!2eF");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }

      
    }

    
}
