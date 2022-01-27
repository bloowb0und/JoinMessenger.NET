using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;

namespace BLL.Services
{

    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly NetworkCredential _networkCredential;

        public EmailNotificationService()
        {
            _networkCredential = new NetworkCredential()
            {
                UserName = "joinMessenger@outlook.com",
                Password = "qweRty123321"
            };
        }

        public async Task SendForgotPassword(User user)
        {
            var smptClient = new SmtpClient()
            {
                Host = "smtp-mail.outlook.com",
                Port = 25,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = this._networkCredential,
            };

            using (var mailMessage = new MailMessage(
                       new MailAddress(this._networkCredential.UserName, "Sandra from Join"), 
                       new MailAddress(user.Email, user.Name)))
            {
                mailMessage.Subject = "Password recovery for Join";
                mailMessage.Body =
                    $"<h2>Forgot password</h2>Hello, <b>{user.Name}</b><br><br>We recently received a request about a forgot password on:<br><b>{user.Email}</b><br><br>Password for your account is:<br><b>{user.Password}</b><br><br>If it wasn't you, please ignore this message.";
                mailMessage.IsBodyHtml = true;

                await smptClient.SendMailAsync(mailMessage);
            }
        }
    }
}