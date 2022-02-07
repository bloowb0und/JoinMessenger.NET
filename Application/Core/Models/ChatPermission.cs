using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class ChatPermission : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        [Required]
        public bool DefaultStatus { get; set; } // default value for all chats
        
        public ICollection<ChatPermissionRole> ChatPermissionRoles { get; set; }
    }
}