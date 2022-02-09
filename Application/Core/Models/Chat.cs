using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Chat : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public ChatType Type { get; set; }
        [Required]
        public Server Server { get; set; }

        public ICollection<ChatPermissionRole> ChatPermissionRoles { get; set; } // changed permission for a specific role
    }
}