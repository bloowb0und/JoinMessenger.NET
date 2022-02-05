using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class UserServerRole : BaseEntity
    {
        public UserServer UserServer { get; set; }
        public Role Role { get; set; }
        
        public DateTime DateApplied { get; set; }
    }
}