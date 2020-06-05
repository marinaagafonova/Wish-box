using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wish_Box.Models;
using Wish_Box.Repositories;
using Wish_Box.ViewModels;

namespace Wish_Box.Controllers
{
    public class UserPageController : Controller
    {
        //private readonly AppDbContext db;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<Following> followingRepository;
        private readonly IRepository<Wish> wishRepository;
        private readonly IRepository<TakenWish> takenWishRepository;
        public UserPageController(IRepository<User> userRepository, IRepository<Following> followingRepository, IRepository<Wish> wishRepository, IRepository<TakenWish> takenWishRepository)
        {
            //db = context;
            this.userRepository = userRepository;
            this.followingRepository = followingRepository;
            this.wishRepository = wishRepository;
            this.takenWishRepository = takenWishRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Show()
        {
            if (User.Identity.IsAuthenticated)
            {
                var login = RouteData.Values["id"].ToString();
                User user = await userRepository.FindFirstOrDefault(x => x.Login == login); //the owner of the page we're on
                User currentUser = await userRepository.FindFirstOrDefault(x => x.Login == User.Identity.Name); //current logged in user
                List<int> following_ids = followingRepository.Find(p => p.UserIsFId == user.Id).Select(p => p.UserFId).ToList();
                List<Wish> user_wishes = wishRepository.Find(p => p.UserId == user.Id).ToList();
                List<int> takenWishes = takenWishRepository.Find(t => t.WhoGivesId == currentUser.Id).Select(t => t.WishId).ToList();
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
