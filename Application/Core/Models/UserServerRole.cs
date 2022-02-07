using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class UserServerRole : BaseEntity
    {
        [Required]
        public UserServer UserServer { get; set; }
        [Required]
        public Role Role { get; set; }
        [Required]
        public DateTime DateApplied { get; set; }
    }
}