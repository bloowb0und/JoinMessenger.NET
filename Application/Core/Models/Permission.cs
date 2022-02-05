using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Permission : BaseEntity
    {
        [Required]
        [Column(TypeName = "nvarchar(30)")]
        public string Name { get; set; }
        
        [Column(TypeName = "nvarchar(200)")]
        public string Description { get; set; }

        public List<ChatRolePermission> ChatRolePermissions { get; set; }

        public List<ServerRolePermission> ServerRolePermissions { get; set; }
    }
}