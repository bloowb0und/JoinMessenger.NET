namespace Core.Models.ServiceMethodsModels
{
    public class RoleServiceEditRole
    {
        public Server RoleServer { get; set; }
        public string RoleName { get; set; }
        public ServerPermissionRole ServerPermissionRole { get; set; }
        public ChatPermissionRole ChatPermissionRole { get; set; }
    }
}