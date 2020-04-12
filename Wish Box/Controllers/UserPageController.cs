using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wish_Box.Models;

namespace Wish_Box.Controllers
{
    public class UserPageController : Controller
    {
        private readonly AppDbContext db;
        public UserPageController(AppDbContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Show()
        {
            var login = RouteData.Values["id"].ToString();
            var currentUser = db.Users.FirstOrDefault(x => x.Login == login);

            ViewBag.CurrentUser = currentUser;
            return View(db.Wishes.ToList());
        }
    }
}
