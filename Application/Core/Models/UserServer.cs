using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class UserServer : BaseEntity
    {
        public User User;
        
        public Server Server;
        
        public DateTime DateEntered;

        public List<Role> Roles;
    }
}