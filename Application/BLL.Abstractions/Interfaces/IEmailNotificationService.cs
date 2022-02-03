using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IEmailNotificationService
    {
        Task<bool> SendForgotPasswordAsync(User user);
        Task InviteByEmailAsync(Server server, User user);
    }
}