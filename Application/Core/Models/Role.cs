namespace Core.Models
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }

        public RoleType RoleType;
    }
}