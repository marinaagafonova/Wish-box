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

        public async Task<IActionResult> Remove()
        {
            var followed_id = Convert.ToInt32(RouteData.Values["id"]);
            var current_user = await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name);
            var follow = await db.Followings.FirstOrDefaultAsync(p => p.UserFId == current_user.Id && p.UserIsFId == followed_id);
            db.Followings.Remove(follow);
            db.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public IActionResult Show()
        {
            var CurrentUser = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);

            List<User> followedUsersList = new List<User>();

            foreach (var followed in db.Followings)
            {
                if (followed.UserFId == CurrentUser.Id)
                {
                    followedUsersList.Add(db.Users.FirstOrDefault(x => x.Id == followed.UserIsFId));
                }
            }

            ViewBag.followingUsers = followedUsersList;

            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var follow = await db.Followings.FirstOrDefaultAsync(x => x.UserIsFId == id);
            if (follow == null)
            {
                return NotFound();
            }
            db.Followings.Remove(follow);
            db.SaveChanges();
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}