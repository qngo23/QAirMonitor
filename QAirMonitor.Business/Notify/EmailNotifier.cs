﻿using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using QAirMonitor.Abstract.Business;
using System.Threading.Tasks;

namespace QAirMonitor.Business.Notify
{
    public class EmailNotifier : INotifier<string>
    {
        public async Task SendNotificationAsync(string data)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("QNotifyApp", "qnotify@outlook.com"));
            emailMessage.To.Add(new MailboxAddress("QNotifyApp", "qnotify@outlook.com"));
            emailMessage.Subject = "QAirMonitor Notification";
            emailMessage.Body = new TextPart("plain") { Text = data };

            using (var client = new SmtpClient())
            {   
                await client.ConnectAsync("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls).ConfigureAwait(false);                
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.AuthenticateAsync("qnotify@outlook.com", "QT.red23");
                await client.SendAsync(emailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}
