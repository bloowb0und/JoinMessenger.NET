using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class ServerPermission : BaseEntity
    {
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string Description { get; set; }
        [Required]
        public bool DefaultStatus { get; set; } // default value for all servers
        
        public ICollection<ServerPermissionRole> ServerPermissionRoles { get; set; }
    }
}