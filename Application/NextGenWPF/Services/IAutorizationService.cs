using Core.Models;
using System.Threading.Tasks;

namespace NextGenWPF.Services
{
    public interface IAutorizationService
    {
        public  Task<User> Autorization(User user);
        public Task<bool> Recover(string email);
    }
}
