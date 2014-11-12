using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace WebApi2WithTokenAuthorization
{
    public class GlobalExceptionHandler : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError){Content = new StringContent(context.Exception.ToString())};
        }
    }
}