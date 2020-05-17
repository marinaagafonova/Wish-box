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
    [Route("[controller]")]
    [ApiController]
    public class FollowingsController : Controller
    {
        readonly AppDbContext db;

        public FollowingsController(AppDbContext context)
        {
            db = context;
        }

        //[Route("Add/{id:int}")]
        //[HttpPost]
        [HttpPost("{id:int}")]
        public async Task<ActionResult<Following>> Add([FromRoute] int id)
        {
            var followed_id = Convert.ToInt32(RouteData.Values["id"]);
            var current_user = await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name);
            if (User.Identity.Name != null)
            {

                db.Followings.Add(new Following
                {
                    UserFId = current_user.Id,
                    UserIsFId = id
                });
                await db.SaveChangesAsync();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index", "Account");
        }

        //[Route("Remove/{id:int}")]
        //[HttpDelete]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Following>> Remove([FromRoute]int id)
        {
            //var followed_id = Convert.ToInt32(RouteData.Values["id"]);
            var current_user = await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name);
            if (current_user != null)
            {
                db.Entry(await db.Followings.FirstOrDefaultAsync(p => p.UserFId == current_user.Id && p.UserIsFId == id)).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index", "Account");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Following>>> Show()
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
                FollowingViewModel fvm = new FollowingViewModel()
                {
                    followedUsersList = followedUsersList
                };

                return View(fvm);
            }
            return RedirectToAction("Index", "Account");
        }
    }
}
