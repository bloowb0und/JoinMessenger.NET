namespace Core.Models
{
    public class ChatRolePermission : BaseEntity
    {
        public ChatRole ChatRole { get; set; }
     
        public Permission Permission { get; set; }
        
        public bool Status { get; set; }
    }
}