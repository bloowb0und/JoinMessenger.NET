using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}