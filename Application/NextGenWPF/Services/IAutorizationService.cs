using Core.Models;
using System.Threading.Tasks;

namespace NextGenWPF.Services
{
    public interface IAutorizationService
    {
        public  Task<bool> Autorization(User user);
    }
}
