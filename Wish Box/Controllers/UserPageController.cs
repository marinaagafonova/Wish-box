using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Show()
        {
            var login = RouteData.Values["id"].ToString();
            var currentUser = db.Users.FirstOrDefault(x => x.Login == login);
            //var current_user = await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name);
            List<int> following_ids = await db.Followings.Where(p => p.UserFId == currentUser.Id).Select(p => p.UserIsFId).ToListAsync();
            ViewBag.following_ids = following_ids;
            ViewBag.CurrentUser = currentUser;
            return View(db.Wishes.ToList());
        }
    }
}
