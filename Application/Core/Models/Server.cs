using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Server : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        
        public DateTime DateCreated { get; set; }

        public ICollection<Chat> Chats { get; set; }

        public ICollection<User> Users { get; set; }

        public ICollection<Message> Messages { get; set; }

        public List<UserServer> UserServers { get; set; }
        public List<ServerRole> ServerRoles { get; set; }

    }
}