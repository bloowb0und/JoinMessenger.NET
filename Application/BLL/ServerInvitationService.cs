using BLL.Abstractions;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ServerInvitationService : IServerInvitationService
    {
        private readonly NetworkCredential _networkCredential;

        public ServerInvitationService()
        {
            _networkCredential = new NetworkCredential()
            {
                UserName = "test@gmail.com",
                Password = "876543"
            };
        }

        public async Task InviteByEmailAsync(Server server, User user)
        {
            if (server is null || user is null)
            {
                return;
            }

            // checking if this user is already in the server
            if (server.Users.FirstOrDefault(u => u.Id == user.Id) is not null)
            {
                return;
            }

            var smptClient = new SmtpClient()
            {
                Port = 25,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = this._networkCredential,
            };

            using (var mailMessage = new MailMessage(
                       new MailAddress(this._networkCredential.UserName, "Bonjour"),
                       new MailAddress(user.Email, user.Name)))
            {
                mailMessage.Subject = "Join server invitation";
                mailMessage.Body = "Hello\n" +
                    $"You have been invited to a Join Server called {server.Name}\n" +
                    "Click the link below to join\n" +
                    "Regards\n";
                    
                mailMessage.IsBodyHtml = true;

                await smptClient.SendMailAsync(mailMessage);
            }
        }
    }
}
