using System;

namespace Core.Models
{
    public class UserServerRole : BaseEntity
    {
        public UserServer UserServer;
        
        public Role Role;
        
        public DateTime DateApplied;
    }
}