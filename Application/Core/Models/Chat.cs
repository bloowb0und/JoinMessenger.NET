using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Chat : BaseEntity
    {
        [Required]
        [Column(TypeName = "nvarchar(30)")]
        public string Name { get; set; }

        [Required]
        public ChatType Type { get; set; }

        public int ServerId { get; set; }

        public Server Server { get; set; }

        public ICollection<Message> Messages { get; set; }

        public List<ChatRole> ChatRoles { get; set; }
    }
}