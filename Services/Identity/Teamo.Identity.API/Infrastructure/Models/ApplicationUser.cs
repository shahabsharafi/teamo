using AspNetCore.Identity.Mongo.Model;

namespace Teamo.Identity.API.Infrastructure.Models
{
    public class ApplicationUser : MongoUser
    {
        public string FullName { get; set; } = "";
        public bool IsDeleted { get; set; }
    }
}
