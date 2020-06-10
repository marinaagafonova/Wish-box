using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wish_Box.Contracts;
using Wish_Box.Models;
using Wish_Box.Repositories;
using Wish_Box.ViewModels;

namespace Wish_Box.Controllers
{

    [ApiController]
    public class FollowingsController : Controller
    {
        //readonly AppDbContext db;
        private readonly IRepository<Following> rep_following;
        private readonly IRepository<User> rep_user;

        public FollowingsController(IRepository<Following> followingRepository, IRepository<User> userRepository)
        {
            rep_following = followingRepository;
            rep_user = userRepository;
            db = context;
        }

        [HttpPost("[controller]/[action]/{id}")]
        public async Task<IActionResult> PostFollowing([FromRoute] int id)
        {
            //var followed_id = Convert.ToInt32(RouteData.Values["id"]);
            //var users = rep_user.GetAll();
            string login = User.Identity.Name;
            var current_user = await rep_user.FindFirstOrDefault(p => p.Login == login);
            if (User.Identity.Name != null)
            {
                await rep_following.Create(new Following
                {
                    UserFId = current_user.Id,
                    UserIsFId = id
                });
                //await db.SaveChangesAsync();
                return Json(new { success = true, responseText = "Following was added!" });
               // return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index", "Account");
        }


        [HttpDelete("[controller]/[action]/{id}")]
        public async Task<ActionResult<Following>> Remove([FromRoute]int id)
        {
            //var followed_id = Convert.ToInt32(RouteData.Values["id"]);
            var current_user = await rep_user.FindFirstOrDefault(p => p.Login == User.Identity.Name); 
            if (current_user != null)
            {
                var follow = await rep_following.FindFirstOrDefault(p => p.UserFId == current_user.Id && p.UserIsFId == id);
                await rep_following.Delete(follow.Id);
                return Json(new { success = true, responseText = "Following was deleted!" });
                //return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index", "Account");
        }

        [HttpGet("[controller]/[action]/")]
        public async Task<ActionResult<IEnumerable<Following>>> Show()
        {
            if (User.Identity.IsAuthenticated)
            {
                string login = User.Identity.Name;
                var CurrentUser = await rep_user.FindFirstOrDefault(p => p.Login == login);
                List<User> followedUsersList = new List<User>();
                //var followings = await db.Followings.Where(p => p.UserFId == CurrentUser.Id).ToListAsync();
                var followings = await rep_following.Find(p => p.UserFId == CurrentUser.Id);
                if (followings != null)
                {
                    foreach (var followed in followings)
                    {
                        var new_following = await rep_user.FindFirstOrDefault(x => x.Id == followed.UserIsFId);
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
