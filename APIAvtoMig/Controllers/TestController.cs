using Microsoft.AspNetCore.Mvc;

namespace APIAvtoMig.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
