using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApi2WithTokenAuthorization
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext() : base("AuthContext")
        {
            
        }
    }
}