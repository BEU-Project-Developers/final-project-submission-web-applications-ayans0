using Microsoft.AspNetCore.Mvc;
using MovieManagement.Models;

namespace MovieManagement.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Profile()
        {
            var user = new User
            {
                Id = 1,
                Username = "ayan",
                Email = "ayan@gmail.com",
                FirstName = "Ayan",
                LastName = "Rzali",
                RegistrationDate = System.DateTime.Now.AddYears(-1)
            };
            return View(user);
        }
    }
} 