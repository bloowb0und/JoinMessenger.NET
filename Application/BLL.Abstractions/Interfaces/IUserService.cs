using Core.Models;

namespace BLL.Abstractions.Interfaces;

public interface IUserService
{
    public void Register(User user);
}