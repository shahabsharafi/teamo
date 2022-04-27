namespace Teamo.Workspace.API.Infrastructure.Models
{
    public class Tenant
    {
        public Guid WorkspaceId { get; set; }
        public string Name { get; set; } = "";
        public Storage Storage { get; set; } = new Storage();
    }
}
