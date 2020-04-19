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
            var currentUser = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);
            var login = RouteData.Values["id"].ToString();
            var currentprofile = db.Users.FirstOrDefault(x => x.Login == login);
            //var current_user = await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name);
            List<int> following_ids = db.Followings.Where(p => p.UserFId == currentUser.Id).Select(p => p.UserIsFId).ToList();
            ViewBag.following_ids = following_ids;
            ViewBag.CurrentUser = currentprofile;
            return View(db.Wishes.ToList());
        }
    }
}
