using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    
    public class User : BaseEntity
    { 
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "varchar(150)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(30)")]
        public string Login { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(30)")]
        public string Password { get; set; }
        
        public ICollection<UserServer> UserServers { get; set; }
    }
}