using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class ChatPermissionRole : BaseEntity
    {
        [Required]
        public Chat Chat { get; set; }
        [Required]
        public Role Role { get; set; }
        [Required]
        public ChatPermission ChatPermission { get; set; }
        [Required]
        public bool Status { get; set; }
    }
}