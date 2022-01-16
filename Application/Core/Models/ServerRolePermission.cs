namespace Core.Models
{
    public class ServerRolePermission : BaseEntity
    {
        public ServerRole ServerRole { get; set; }
        
        public Permission Permission { get; set; }
        
        public bool Status { get; set; }
    }
}