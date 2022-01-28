using System.Collections.Generic;

namespace Core.Models
{
    
    public class User : BaseEntity
    { 
        public string Name { get; set; }
        
        public string Email { get; set; }

        public string Login { get; set; }
        
        public string Password { get; set; }

        public List<Server> Servers { get; set; }
    }
}