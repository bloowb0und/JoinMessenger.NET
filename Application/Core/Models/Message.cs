using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Message : BaseEntity
    {
        [Required]
        public User User { get; set; }
        [Required]
        public Chat Chat { get; set; }

        [Required]
        [MaxLength(500)]
        public string Value { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        public DateTime? DateLastEdited { get; set; }
    }
}