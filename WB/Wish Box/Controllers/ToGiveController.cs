using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wish_Box.Models;
using Wish_Box.Repositories;

namespace Wish_Box.Controllers
{
    //[ApiController]
    public class ToGiveController : Controller
    {
        //private readonly AppDbContext db;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<Wish> wishRepository;
        private readonly IRepository<TakenWish> takenWishRepository;
        private readonly IRepository<Comment> commentRepository;

        public ToGiveController(IRepository<User> userRepository, IRepository<Wish> wishRepository, 
            IRepository<TakenWish> takenWishRepository, IRepository<Comment> commentRepository)
        {
            //db = context;
            this.userRepository = userRepository;
            this.wishRepository = wishRepository;
            this.takenWishRepository = takenWishRepository;
            this.commentRepository = commentRepository;
        }

        [HttpGet("[controller]/[action]")]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user_id = await userRepository.FindFirstOrDefault(u => u.Login == User.Identity.Name);
                List<int> wishId = (await takenWishRepository.Find(w => w.WhoGivesId == user_id.Id)).Select(w => w.WishId).ToList();
                var wishes = (await wishRepository.Find(w => wishId.Contains(w.Id))).ToList();
                foreach (var wish in wishes)
                    wish.User = await userRepository.FindFirstOrDefault(p => p.Id == wish.UserId);
                return View(wishes);
            }
            return RedirectToAction("Index", "Account");
        }

        //[HttpPost]
        [HttpPost("[controller]/[action]/{id}")]
        public async Task<IActionResult> Add([FromRoute] int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                int wishId = Convert.ToInt32(RouteData.Values["id"]);
                int whoWishesId = (await wishRepository.FindFirstOrDefault(w => w.Id == wishId)).UserId;
                int whoGivesId = (await userRepository.FindFirstOrDefault(u => u.Login == User.Identity.Name)).Id;
                TakenWish takenWish = new TakenWish()
                {
                    IsGiven = true,
                    WhoGivesId = whoGivesId,
                    WhoWishesId = whoWishesId,
                    WishId = wishId
                };
                await takenWishRepository.Create(takenWish);
                //await db.SaveChangesAsync();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index", "Account");
        }

        [HttpDelete("[controller]/[action]/{id}")]
        //[HttpPost("[controller]/[action]")]
        //[HttpPost]
        public async Task<IActionResult> Remove([FromRoute] int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                int wishId = Convert.ToInt32(RouteData.Values["id"]);
                int whoGivesId = (await userRepository.FindFirstOrDefault(u => u.Login == User.Identity.Name)).Id;
                TakenWish takenWish = (await takenWishRepository.FindFirstOrDefault(t => (t.WishId == wishId && t.WhoGivesId == whoGivesId)));
                //db.Entry(takenWish).State = EntityState.Deleted;
                await takenWishRepository.Delete(takenWish.Id);
                //await db.SaveChangesAsync();
                //return Redirect(Request.Headers["Referer"].ToString());
                return Json(new { success = true, responseText = "mark was deleted" });
            }
            return RedirectToAction("Index", "Account");
        }

        [HttpPost("[controller]/[action]/{id}")]
      //  [HttpPost]
        public async Task<IActionResult> MarkAsGiven([FromRoute]int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                int wishId = Convert.ToInt32(RouteData.Values["id"]);
                int whoGivesId = (await userRepository.FindFirstOrDefault(u => u.Login == User.Identity.Name)).Id;
                var comments = (await commentRepository.Find(p => p.WishId == wishId)).ToList();
                foreach (var comment in comments)
                {
                    //db.Entry(comment).State = EntityState.Deleted;
                    await commentRepository.Delete(comment.Id);
                    //await db.SaveChangesAsync();
                }
                TakenWish takenWish = (await takenWishRepository.FindFirstOrDefault(t => t.WishId == wishId && t.WhoGivesId == whoGivesId));
                //db.Entry(takenWish).State = EntityState.Deleted;
                await takenWishRepository.Delete(takenWish.Id);
                //await db.SaveChangesAsync();
                Wish wish = new Wish { Id = wishId };
                //db.Entry(wish).State = EntityState.Deleted;
                await wishRepository.Delete(wish.Id);
                //await db.SaveChangesAsync();

                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Account", "Index");
        }
    }
}