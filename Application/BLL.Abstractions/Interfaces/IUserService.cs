using Core.Models;

namespace BLL.Abstractions.Interfaces
{
    public interface IUserService
    {
        public void Register(User user);
        public User SignIn(string login, string password);
        public void PasswordRecovery(string email);
    }
}