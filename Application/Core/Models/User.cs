using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    
    public class User : BaseEntity
    { 
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        public ICollection<Server> Servers { get; set; }

        public ICollection<Message> Messages { get; set; }

        public List<UserServer> UserServers { get; set; }
    }
}