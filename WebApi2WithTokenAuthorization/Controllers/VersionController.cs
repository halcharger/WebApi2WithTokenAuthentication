using System.Reflection;
using System.Web.Http;

namespace WebApi2WithTokenAuthorization.Controllers
{
    public class VersionController : ApiController
    {
        [Route("api/version")]
        public string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
