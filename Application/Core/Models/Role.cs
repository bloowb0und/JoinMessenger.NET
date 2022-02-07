using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Role : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public Server Server { get; set; }
        
        public ICollection<UserServerRole> UserServerRoles { get; set; }
        public ICollection<ServerPermissionRole> ServerPermissionRoles { get; set; }
        public ICollection<ChatPermissionRole> ChatPermissionRoles { get; set; }
    }
}