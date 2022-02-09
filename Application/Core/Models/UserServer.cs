using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class UserServer : BaseEntity
    {
        public User User { get; set; }
        public Server Server { get; set; }
        
        [Required]
        public DateTime DateEntered { get; set; }

        public ICollection<UserServerRole> UserServerRoles { get; set; }
    }
}