using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Text.Encodings.Web;
using bugtracker.Models;
using bugtracker.Services;
using System.Diagnostics;

namespace bugtracker.Controllers
{
    public class LoginController : Controller
    {

        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // [AllowAnonymous]
        // [HttpPost("authenticate")]
        // public IActionResult Authenticate([FromBody]UserModel model)
        public IActionResult Authenticate(UserModel user)
        {
            // return NotFound();
            // Console.WriteLine("the message");
            Debug.WriteLine("\n\n\n-------User--------");
            Debug.WriteLine(user.email+"\n\n\n");
            
            var users = _authService.Authenticate(user.email, user.password);

            return Ok();
        }

    }
}