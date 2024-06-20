using Microsoft.AspNetCore.Mvc;

namespace MVC_Test.Controllers
{
    public class PayController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
