using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.ServiceMethodsModels;

namespace BLL.Abstractions.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(User user);
        User SignIn(string username, string password);
        Task<bool> PasswordRecoveryAsync(string email);
        Task<bool>  ChangeUserDataAsync(User user, UserServiceChangeUserData newUserData);
    }
}