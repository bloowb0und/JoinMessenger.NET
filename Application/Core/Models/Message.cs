using System;

namespace Core.Models
{
    public class Message : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ServerId { get; set; }
        public Server Server { get; set; }

        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        public string Value { get; set; }

        public DateTime DateCreated { get; set; }
        
        public DateTime? DateLastEdited { get; set; }
    }
}