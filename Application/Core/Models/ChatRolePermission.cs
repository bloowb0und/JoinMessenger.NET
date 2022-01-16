namespace Core.Models
{
    public class ChatRolePermission : BaseEntity
    {
        public ChatRole ChatRole;
     
        public Permission Permission;
        
        public bool Status;
    }
}