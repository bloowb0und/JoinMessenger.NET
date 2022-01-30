using System;

namespace Core.Models
{
    public class Message : BaseEntity
    {
        public User User { get; set; }

        public Server Server { get; set; }
        
        public Chat Chat { get; set; }

        public string Value { get; set; }

        public Message RepliedMessage { get; set; }
        
        public DateTime DateCreated { get; set; }
        
        public DateTime? DateLastEdited { get; set; }
    }
}