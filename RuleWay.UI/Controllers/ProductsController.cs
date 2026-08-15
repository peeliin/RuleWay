using Microsoft.AspNetCore.Mvc;

namespace RuleWay.UI.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
