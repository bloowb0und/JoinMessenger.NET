using System.Collections.Generic;

namespace Core.Models
{
    public class ServerRole : BaseEntity
    {
        public Server Server;
        
        public Role Role;

        public List<ServerRolePermission> ServerRolePermissions;
    }
}