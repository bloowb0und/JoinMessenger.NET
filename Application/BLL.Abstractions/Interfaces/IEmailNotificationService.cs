using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IEmailNotificationService
    {
        public void SendForgotPassword(User user);
    }
}