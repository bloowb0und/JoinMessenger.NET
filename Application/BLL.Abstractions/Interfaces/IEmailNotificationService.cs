using System.Threading.Tasks;
using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IEmailNotificationService
    {
        Task<bool> SendForgotPassword(User user);
        Task InviteByEmailAsync(Server server, User user);
    }
}