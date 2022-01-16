using System.Collections.Generic;

namespace Core.Models
{
    public class ChatRole : BaseEntity
    {
        public Role Role;
        
        public Chat Chat;
        
        public List<ChatRolePermission> ChatRolePermissions;
    }
}