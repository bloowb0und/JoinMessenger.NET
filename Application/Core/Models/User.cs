using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    
    public class User : BaseEntity
    { 
        [Required]
        [Column(TypeName = "nvarchar(30)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "varchar(30)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(30)")]
        public string Login { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(30)")]
        public string Password { get; set; }

        public ICollection<Server> Servers { get; set; } = new List<Server>();

        public ICollection<Message> Messages { get; set; } = new List<Message>();

        public List<UserServer> UserServers { get; set; }
    }
}