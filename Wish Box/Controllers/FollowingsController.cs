using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wish_Box.Models;
using Microsoft.AspNetCore.Mvc;

namespace Wish_Box.Controllers
{
    public class FollowingsController : Controller
    {
        AppDbContext db;

        public FollowingsController(AppDbContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Show()
        {
            var currentUser = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);

            ViewBag.CurrentUser = currentUser;
            ViewBag.AllUsers = db.Users.ToList();
            return View(db.Followings.ToList());
        }
    }
}
