using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IUserService
    {
        public void Register(User user);
        public User SignIn(string username, string password);
        public void PasswordRecovery(string email);
        
        /// <summary>
        /// Method ChangeUserData allows user to change their data, except for email
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="dataIdx">0 - password | 1 - login | 2 - name</param>
        /// <param name="oldValue">Old value for selected data</param>
        /// <param name="newValue">New value for selected data</param>
        public void ChangeUserData(User user, int dataIdx, string oldValue, string newValue);
    }
}