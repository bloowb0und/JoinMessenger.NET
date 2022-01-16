namespace Core.Models
{
    public class ServerRolePermission : BaseEntity
    {
        public ServerRole ServerRole;
        
        public Permission Permission;
        
        public bool Status;
    }
}