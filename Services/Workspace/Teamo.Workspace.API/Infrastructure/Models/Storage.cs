namespace Teamo.Workspace.API.Infrastructure.Models
{
    public class Storage
    {
        public string Name { get; set; } = "";
        public string ConnectionString { get; set; } = "";
        public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
    }
}