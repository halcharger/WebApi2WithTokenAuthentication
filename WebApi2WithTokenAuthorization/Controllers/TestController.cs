using System;
using System.Web.Http;

namespace WebApi2WithTokenAuthorization.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        public string Error()
        {
            var zero = 0;
            var blah = 1/zero;

            return "boom";
        }
    }
}