using System.Collections.Generic;

namespace Core.Models
{
    public class User : BaseEntity
    {
        public string Name;
        
        public string Email;
        
        public string Login;
        
        public string Password;

        public List<Server> Servers;
    }
}