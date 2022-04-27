using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teamo.Assets.Email
{
    public class EmailKitService : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration;

        public EmailKitService(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public async Task SendAsync(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

            if (emailMessage.FromAddresses == null)
            {
                message.From.Add(new MailboxAddress("", _emailConfiguration.SenderAddress));
            }
            else
            {
                message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            }

            message.Subject = emailMessage.Subject;
            //We will say we are sending HTML. But there are options for plaintext etc. 
            message.Body = new TextPart(TextFormat.Plain)
            {
                Text = emailMessage.Content
            };

            //Be careful that the SmtpClient class is the one from Mailkit not the framework!
            using (var emailClient = new SmtpClient())
            {
                //The last parameter here is to use SSL (Which you should!)
                emailClient.Connect(_emailConfiguration.Smtp.Server, _emailConfiguration.Smtp.Port, _emailConfiguration.Smtp.UseSsl);

                //Remove any OAuth functionality as we won't be using it. 
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(_emailConfiguration.Smtp.Username, _emailConfiguration.Smtp.Password);

                await emailClient.SendAsync(message);

                emailClient.Disconnect(true);
            }
        }
    }
}
