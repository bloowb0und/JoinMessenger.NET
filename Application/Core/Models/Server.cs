using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class Server : BaseEntity
    {
        public string Name { get; set; }
        
        public DateTime DateCreated { get; set; }

        public List<Chat> Chats { get; set; }

        public List<ServerRole> ServerRoles { get; set; }
    }
}