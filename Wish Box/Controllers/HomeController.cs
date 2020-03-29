using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Wish_Box.Models;

namespace Wish_Box.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext db;

        public HomeController(AppDbContext context)
        {
            db = context;
        }

        //[Authorize]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(db.Users.ToList());
            }
            return RedirectToAction("Logout", "Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "Account");
        }
    }
}
