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
