using System;

namespace Core.Models
{
    public class UserServerRole : BaseEntity
    {
        public int UserServerID { get; set; }
        public UserServer UserServer { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
        
        public DateTime DateApplied { get; set; }
    }
}