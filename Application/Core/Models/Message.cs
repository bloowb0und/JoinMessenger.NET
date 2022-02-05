using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Message : BaseEntity
    {
        public User User { get; set; }
        public Chat Chat { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(500)")]
        public string Value { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        public DateTime? DateLastEdited { get; set; }
    }
}