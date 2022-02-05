using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Server : BaseEntity
    {
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }
        
        public ICollection<UserServer> UserServers { get; set; }
    }
}