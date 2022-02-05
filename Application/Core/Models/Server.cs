using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Server : BaseEntity
    {
        [Required]
        [Column(TypeName = "nvarchar(30)")]
        public string Name { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime DateCreated { get; set; }

        public ICollection<Chat> Chats { get; set; } = new List<Chat>();

        public ICollection<User> Users { get; set; } = new List<User>();

        public ICollection<Message> Messages { get; set; } = new List<Message>();

        public List<UserServer> UserServers { get; set; }
        public List<ServerRole> ServerRoles { get; set; }

        public string Icon { get; set; }
    }
}