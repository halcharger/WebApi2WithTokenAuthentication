using System.Reflection;
using System.Web.Http;

namespace AdaptiveSystems.AspNetIdentity.OAuth.Controllers
{
    public class VersionController : ApiController
    {
        [Route("api/adaptivesystems.aspnetidentity.oauth/version")]
        public string GetVersion()
        {
            return Assembly.GetAssembly(GetType()).GetName().Version.ToString();
        }
    }
}
