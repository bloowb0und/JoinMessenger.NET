using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.ServiceMethodsModels;

namespace BLL.Abstractions.Interfaces
{
    public interface IRoleService
    {
        Task<bool> CreateRoleAsync(RoleServiceEditRole role);
        
        Task<bool> DeleteRoleAsync(Role role);
        
        Task<bool> AttachUserToRoleAsync(Role role, User user);
        
        Task<bool> AttachUsersToRoleAsync(Role role, IEnumerable<User> users);
        
        Task<bool> RemoveUserFromRoleAsync(Role role, User user);
        
        Task<bool> RemoveUsersFromRoleAsync(Role role, IEnumerable<User> users);

        Task<bool> RenameRoleAsync(Role role,RoleServiceEditRole newRole);

        Task<bool> EditRoleOnServerLevelAsync(ServerPermissionRole serverPermissionRole, RoleServiceEditRole newRole);
        
        Task<bool> EditRoleOnChatLevelAsync(ChatPermissionRole chatPermissionRole, RoleServiceEditRole newRole);
    }
}