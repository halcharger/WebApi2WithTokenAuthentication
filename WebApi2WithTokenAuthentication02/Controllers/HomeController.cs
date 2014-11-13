using System.Web.Mvc;

namespace WebApi2WithTokenAuthentication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}