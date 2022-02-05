using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class UserServerRole : BaseEntity
    {
        public int UserServerID { get; set; }
        public UserServer UserServer { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
        
        [Column(TypeName = "datetime2(0)")]
        public DateTime DateApplied { get; set; }
    }
}