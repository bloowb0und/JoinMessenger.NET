using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class UserServer : BaseEntity
    {
        public User User { get; set; }
        
        public Server Server { get; set; }
        
        public DateTime DateEntered { get; set; }

        public List<UserServerRole> UserServerRoles { get; set; }
    }
}