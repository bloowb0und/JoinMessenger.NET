using System;
using System.ComponentModel.DataAnnotations.Schema;

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

        public Message RepliedMessage { get; set; }


        [Column(TypeName = "nvarchar(500)")]
        public string Value { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime? DateLastEdited { get; set; }
    }
}