﻿using BugTracker.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BugTracker.Services
{
    public class BTEmailService : IEmailSender
    {
        private readonly MailSettings _mailSettings;

        public BTEmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            MimeMessage email = new();

            email.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
            email.To.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            email.Subject = subject;

            var builder = new BodyBuilder()
            {
                HtmlBody = htmlMessage
            };

            email.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.User, _mailSettings.Password);

                await smtp.SendAsync(email);

                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Sending Email. ---> {ex.Message}");
                throw;
            }
        }
    }
}
