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
            List<int> following_ids = db.Followings.Where(p => p.UserFId == currentUser.Id).Select(p => p.UserIsFId).ToList();
            List<Wish> user_wishes = db.Wishes.Where(p => p.UserId == currentUser.Id).ToList();
            ViewBag.following_ids = following_ids;
            ViewBag.CurrentUser = currentUser;
            ViewBag.Wishes = user_wishes;
            return View();
        }
    }
}
