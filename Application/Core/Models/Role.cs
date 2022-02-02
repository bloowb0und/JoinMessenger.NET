using System.Collections.Generic;

namespace Core.Models
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }

        public RoleType RoleType;

        public List<ChatRole> ChatRoles { get; set; }
        public List<ServerRole> ServerRoles { get; set; }
    }
}