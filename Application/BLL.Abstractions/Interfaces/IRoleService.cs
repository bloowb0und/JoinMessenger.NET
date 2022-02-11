using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.ServiceMethodsModels;
using FluentResults;

namespace BLL.Abstractions.Interfaces
{
    public interface IRoleService
    {
        Task<Result> CreateRoleAsync(RoleServiceEditRole role);
        
        Task<Result> DeleteRoleAsync(Role role);
        
        Task<Result> AttachUserToRoleAsync(Role role, User user);
        
        Task<Result> AttachUsersToRoleAsync(Role role, IEnumerable<User> users);
        
        Task<Result> RemoveUserFromRoleAsync(Role role, User user);
        
        Task<Result> RemoveUsersFromRoleAsync(Role role, IEnumerable<User> users);

        Task<Result> EditRoleAsync(Role role,RoleServiceEditRole newRole);
    }
}