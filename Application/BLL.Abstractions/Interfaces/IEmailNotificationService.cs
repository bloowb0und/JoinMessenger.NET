using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Core.Models;
using FluentResults;

namespace BLL.Abstractions.Interfaces
{
    public interface IEmailNotificationService
    {
        Task<Result> SendForgotPasswordAsync(User user);
        Task<Result> InviteByEmailAsync(Server server, User user);
    }
}