using System;

namespace Core.Models
{
    public class Message : BaseEntity
    {
        public Server Server;
        public Chat Chat;
        public User User;
        public string Value;
        public DateTime DateCreated;
    }
}