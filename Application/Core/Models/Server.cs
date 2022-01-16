using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class Server : BaseEntity
    {
        public string Name;
        
        public DateTime DateCreated;

        public List<User> Users;

        public List<Chat> Chats;

        public List<ServerRole> ServerRoles;
    }
}