using System;

namespace Core.Models
{
    public class UserServerRole : BaseEntity
    {
        public UserServer ServerUser { get; set; }
        
        public Role Role { get; set; }
        
        public DateTime DateApplied { get; set; }
    }
}