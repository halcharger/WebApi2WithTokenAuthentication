using System;
using System.Web.Http;

namespace WebApi2WithTokenAuthentication.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        public string Error()
        {
            throw new Exception("web api exception");
        }
    }
}