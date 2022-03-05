using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    
    public class User : BaseEntity
    { 
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        [MaxLength(30)]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
        
        public ICollection<UserServer> UserServers { get; set; }
    }
}