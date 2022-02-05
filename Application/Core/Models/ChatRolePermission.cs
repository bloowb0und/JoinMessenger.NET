namespace Core.Models
{
    public class ChatRolePermission : BaseEntity
    {
        public int ChatRoleId { get; set; }
        public ChatRole ChatRole { get; set; }

        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
        
        public bool Status { get; set; }
    }
}