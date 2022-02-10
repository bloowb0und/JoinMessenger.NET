using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using BLL.Helpers;
using Core.Models;
using Microsoft.Extensions.Options;

namespace BLL.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly NetworkCredential _networkCredential;
        private readonly SmtpClient _smtpClient;
        private readonly IOptions<EmailCredentialsModel> _appSettings;

        public EmailNotificationService(IOptions<EmailCredentialsModel> appSettings)
        {
            _appSettings = appSettings;
            
            _networkCredential = new NetworkCredential()
            {
                UserName = _appSettings.Value.EmailCredentialsUsername,
                Password = _appSettings.Value.EmailCredentialsPassword
            };
            
            _smtpClient = new SmtpClient()
            {
                Host = _appSettings.Value.SmtpHost,
                Port = _appSettings.Value.SmtpPort,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = this._networkCredential,
            };
        }

        public async Task<bool> SendForgotPasswordAsync(User user)
        {
            using (var mailMessage = new MailMessage(
                       new MailAddress(this._networkCredential.UserName, "Sandra from Join"), 
                       new MailAddress(user.Email, user.Name)))
            {
                mailMessage.Subject = "Password recovery for Join";
                mailMessage.Body =
                    $"<h2>Forgot password</h2>Hello, <b>{user.Name}</b><br><br>We recently received a request about a forgot password on:<br><b>{user.Email}</b><br><br>The new password for your account is:<br><b>{user.Password}</b><br><br>If it wasn't you, please ignore this message.";
                mailMessage.IsBodyHtml = true;

                await _smtpClient.SendMailAsync(mailMessage);

                return true;
            }
        }

        public async Task InviteByEmailAsync(Server server, User user)
        {
            // checking if this user is already in the server
            if (server.UserServers
                    .FirstOrDefault(us => us.User.Id == user.Id && us.Server.Id == server.Id) != null)
            {
                return;
            }

            using (var mailMessage = new MailMessage(
                      new MailAddress(this._networkCredential.UserName, "Sandra from Join"),
                      new MailAddress(user.Email, user.Name)))
            {
                mailMessage.Subject = "You were invited to server on Join";
                mailMessage.Body = $"Hello, {user.Name}\n" +
                    $"You have been invited to a Join Server called {server.Name}\n" +
                    "Click the link below to join\n" +
                    "Regards\n";

                mailMessage.IsBodyHtml = true;

                await _smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}