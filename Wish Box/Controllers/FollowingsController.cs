using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wish_Box.Models;

namespace Wish_Box.Controllers
{
    public class FollowingsController : Controller
    {
        readonly AppDbContext db;

        public FollowingsController(AppDbContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Add()
        {
            var followed_id = Convert.ToInt32(RouteData.Values["id"]);
            var current_user = await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name);
            db.Followings.Add(new Following
            {
                UserFId = current_user.Id,
                UserIsFId = followed_id
            });
            db.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public IActionResult Show()
        {
            var currentUser = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);

            List<User> followedUsersList = new List<User>();

            foreach (var followed in db.Followings)
            {
                if (followed.UserFId == currentUser.Id)
                {
                    followedUsersList.Add(db.Users.FirstOrDefault(x => x.Id == followed.UserIsFId));
                }
            }

            ViewBag.followingUsers = followedUsersList;

            return View();
        }
    }
}