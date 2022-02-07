using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Server : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public ICollection<UserServer> UserServers { get; set; }
        public ICollection<ServerPermissionRole> ServerPermissionRoles { get; set; } // changed permission for a specific role
    }
}