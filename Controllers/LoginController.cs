using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace bugtracker.Controllers
{
    public class LoginController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Authenticate()
        {
            return NotFound();
        }

    }
}