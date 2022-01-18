using System.Collections.Generic;

namespace Core.Models
{
    public class ServerRole : BaseEntity
    {
        public Server Server { get; set; }
        
        public Role Role { get; set; }
        
        public readonly RoleType RoleType = RoleType.Server;

        public List<ServerRolePermission> ServerRolePermissions { get; set; }
    }
}