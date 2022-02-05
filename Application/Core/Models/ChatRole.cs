using System.Collections.Generic;

namespace Core.Models
{
    public class ChatRole : BaseEntity
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        public readonly RoleType RoleType = RoleType.Chat;

        public List<ChatRolePermission> ChatRolePermissions { get; set; }
    }
}