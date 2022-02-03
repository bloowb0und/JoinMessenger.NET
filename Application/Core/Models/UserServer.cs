using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class UserServer : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ServerId { get; set; }
        public Server Server { get; set; }
        
        [Column(TypeName = "datetime2(0)")]
        public DateTime DateEntered { get; set; }

        public List<UserServerRole> UserServerRoles { get; set; }
    }
}