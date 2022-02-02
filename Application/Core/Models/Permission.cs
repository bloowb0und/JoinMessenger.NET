using System.Collections.Generic;

namespace Core.Models
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; }
        
        public string Description { get; set; }

        public List<ChatRolePermission> ChatRolePermissions { get; set; }

        public List<ServerRolePermission> ServerRolePermissions { get; set; }
    }
}