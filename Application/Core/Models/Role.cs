using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Role : BaseEntity
    {
        [Required]
        [Column(TypeName = "nvarchar(30)")]
        public string Name { get; set; }

        public RoleType RoleType;

        public List<ChatRole> ChatRoles { get; set; }
        public List<ServerRole> ServerRoles { get; set; }

        public List<UserServerRole> UserServerRoles { get; set; }
    }
}