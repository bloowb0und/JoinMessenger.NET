namespace Core.Models
{
    public class ServerRolePermission : BaseEntity
    {
        public int ServerRoleId { get; set; }
        public ServerRole ServerRole { get; set; }

        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
        
        public bool Status { get; set; }
    }
}