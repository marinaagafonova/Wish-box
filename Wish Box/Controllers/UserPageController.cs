using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wish_Box.Models;
using Wish_Box.ViewModels;

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
            var currentprofile = db.Users.FirstOrDefault(x => x.Login == login);
            List<int> following_ids = await db.Followings.Where(p => p.UserIsFId == currentprofile.Id).Select(p => p.UserFId).ToListAsync();
            List<Wish> user_wishes = await db.Wishes.Where(p => p.UserId == currentprofile.Id).ToListAsync();

            UserPageViewModel upvm = new UserPageViewModel()
            {
                User = currentprofile,
                UserWishes = user_wishes,
                Followers = following_ids,
                CurrentUser = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name)
            };
            return View(upvm);
        }
    }
}
