using System.Collections.Generic;

namespace Core.Models
{
    public class Chat : BaseEntity
    {
        public string Name { get; set; }
        
        public ChatType Type { get; set; }
        
        public Server Server { get; set; }

        public List<User> Users { get; set; }

        public List<Message> Messages { get; set; }

        public List<ChatRole> ChatRoles { get; set; }
    }
}