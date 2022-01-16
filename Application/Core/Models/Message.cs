using System;

namespace Core.Models
{
    public class Message : BaseEntity
    {
        public User User;

        public Server Server;
        
        public Chat Chat;

        public string Value;
        
        public DateTime DateCreated;
    }
}