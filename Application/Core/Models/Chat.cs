using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Chat : BaseEntity
    {
        public string Name;
        
        public ChatType Type;
        
        public Server Server;

        public List<User> Users;

        public List<Message> Messages;

        public List<ChatRole> ChatRoles;
    }
}