using Core.Models;
using System.Threading.Tasks;

namespace NextGenWPF.Services
{
    public interface IRegistrationService
    {
        public Task<bool> Registration(User user);
    }
}
