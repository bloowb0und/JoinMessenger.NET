using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(User user);
        User SignIn(string username, string password);
        Task<bool> PasswordRecoveryAsync(string email);
        
        /// <summary>
        /// Method ChangeUserData allows user to change their data, except for email
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="userDataType">Which value to change - Password, login or name</param>
        /// <param name="oldValue">Old value for selected data</param>
        /// <param name="newValue">New value for selected data</param>
        Task<bool>  ChangeUserDataAsync(User user, UserDataTypes userDataType, string oldValue, string newValue);
    }
}