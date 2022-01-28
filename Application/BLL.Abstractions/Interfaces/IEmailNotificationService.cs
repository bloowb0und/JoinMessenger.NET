using System.Threading.Tasks;
using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IEmailNotificationService
    {
        public Task SendForgotPassword(User user);
    }
}