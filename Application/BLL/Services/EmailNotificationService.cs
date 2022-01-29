using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;

namespace BLL.Services
{

    public class EmailNotificationService : IEmailNotificationService, IServerInvitationService
    {
        private readonly NetworkCredential _networkCredential;
        private SmtpClient _smtpClient;

        public EmailNotificationService()
        {
            _networkCredential = new NetworkCredential()
            {
                UserName = "joinMessenger@outlook.com",
                Password = "qweRty123321"
            };

            _smtpClient = new SmtpClient()
            {
                Host = "smtp-mail.outlook.com",
                Port = 25,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = this._networkCredential,
            };
        }

        public async Task SendForgotPassword(User user)
        {
            using (var mailMessage = new MailMessage(
                       new MailAddress(this._networkCredential.UserName, "Sandra from Join"), 
                       new MailAddress(user.Email, user.Name)))
            {
                mailMessage.Subject = "Password recovery for Join";
                mailMessage.Body =
                    $"<h2>Forgot password</h2>Hello, <b>{user.Name}</b><br><br>We recently received a request about a forgot password on:<br><b>{user.Email}</b><br><br>Password for your account is:<br><b>{user.Password}</b><br><br>If it wasn't you, please ignore this message.";
                mailMessage.IsBodyHtml = true;

                await _smtpClient.SendMailAsync(mailMessage);
            }
        }

        public async Task InviteByEmailAsync(Server server, User user)
        {
            if (server == null || user == null)
            {
                return;
            }

            // checking if this user is already in the server
            if (server.Users.FirstOrDefault(u => u.Id == user.Id) != null)
            {
                return;
            }

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

                await _smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}