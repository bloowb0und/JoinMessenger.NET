using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class ServerPermissionRole : BaseEntity
    {
        [Required]
        public Role Role { get; set; }
        [Required]
        public ServerPermission ServerPermission { get; set; }
        [Required] 
        public bool Status { get; set; }
    }
}