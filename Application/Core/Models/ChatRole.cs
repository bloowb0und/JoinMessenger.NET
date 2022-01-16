using System.Collections.Generic;

namespace Core.Models
{
    public class ChatRole : BaseEntity
    {
        public Role Role { get; set; }
        
        public Chat Chat { get; set; }
        
        public List<ChatRolePermission> ChatRolePermissions { get; set; }
    }
}