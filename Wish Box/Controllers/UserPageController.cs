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
    public class UserPageController : Controller
    {
        private readonly AppDbContext db;
        public UserPageController(AppDbContext context)
        {
            db = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Show([FromRoute]string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                User user = db.Users.FirstOrDefault(x => x.Login == id);  //the owner of the page we're on
                User currentUser = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);  //current logged in user
                List<int> following_ids = await db.Followings.Where(p => p.UserIsFId == user.Id).Select(p => p.UserFId).ToListAsync();
                List<Wish> user_wishes = await db.Wishes.Where(p => p.UserId == user.Id).ToListAsync();
                List<int> takenWishes = await db.TakenWishes.Where(t => t.WhoGivesId == currentUser.Id).Select(t => t.WishId).ToListAsync();
                UserPageViewModel upvm = new UserPageViewModel()
                {
                    User = user,
                    UserWishes = user_wishes,
                    Followers = following_ids,
                    CurrentUser = currentUser,
                    TakenWishes = takenWishes
                };
                return View(upvm);
            }
            return RedirectToAction("Index", "Account");
        }
    }
}
