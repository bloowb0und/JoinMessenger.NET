using System.Threading.Tasks;
using Core.Models;
using Core.Models.ServiceMethodsModels;
using FluentResults;

namespace BLL.Abstractions.Interfaces
{
    public interface IUserService
    {
        Task<Result> RegisterAsync(User user);
        Task<Result<User>> SignInAsync(string username, string password);
        Task<Result> PasswordRecoveryAsync(string email);
        Task<Result>  ChangeUserDataAsync(User user, UserServiceChangeUserData newUserData);
    }
}