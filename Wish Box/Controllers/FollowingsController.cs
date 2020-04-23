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
            if (User.Identity.Name != null)
            {

                db.Followings.Add(new Following
                {
                    UserFId = current_user.Id,
                    UserIsFId = followed_id
                });
                await db.SaveChangesAsync();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Account", "Index");
        }

        public async Task<IActionResult> Remove() //should be [httpPost]
        {
            var followed_id = Convert.ToInt32(RouteData.Values["id"]);
            var current_user = await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name);
            if (current_user != null)
            {
                db.Entry(await db.Followings.FirstOrDefaultAsync(p => p.UserFId == current_user.Id && p.UserIsFId == followed_id)).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Account", "Index");
        }

        [HttpGet]
        public async Task<IActionResult> Show()
        {
            if (User.Identity.IsAuthenticated)
            {
                var CurrentUser = await db.Users.FirstOrDefaultAsync(x => x.Login == User.Identity.Name);
                List<User> followedUsersList = new List<User>();
                var followings = await db.Followings.Where(p => p.UserFId == CurrentUser.Id).ToListAsync();
                if (followings != null)
                {
                    foreach (var followed in followings)
                    {
                        var new_following = await db.Users.FirstOrDefaultAsync(x => x.Id == followed.UserIsFId);
                        followedUsersList.Add(new_following);
                    }
                }
                ViewBag.followingUsers = followedUsersList;

                return View();
            }
            return RedirectToAction("Account", "Index");
        }
    }
}
