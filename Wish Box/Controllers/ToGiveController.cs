using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wish_Box.Models;

namespace Wish_Box.Controllers
{
    public class ToGiveController : Controller
    {
        private readonly AppDbContext db;
        
        public ToGiveController(AppDbContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                List<int> wishId = await (db.TakenWishes
                    .Where(w => w.WhoGivesId == (db.Users.FirstOrDefault(u => u.Login == User.Identity.Name)).Id)
                    .Select(w => w.WishId)).ToListAsync();
                var wishes = await db.Wishes.Where(w => wishId.Contains(w.Id)).ToListAsync();
                return View(wishes);
            }
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Add()
        {
            if (User.Identity.IsAuthenticated)
            {
                int wishId = Convert.ToInt32(RouteData.Values["id"]);
                int whoWishesId = (await db.Wishes.FirstOrDefaultAsync(w => w.Id == wishId)).UserId;
                int whoGivesId = (await db.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name)).Id;
                TakenWish takenWish = new TakenWish()
                {
                    IsGiven = true,
                    WhoGivesId = whoGivesId,
                    WhoWishesId = whoWishesId,
                    WishId = wishId
                };
                db.TakenWishes.Add(takenWish);
                await db.SaveChangesAsync();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Remove()
        {
            if (User.Identity.IsAuthenticated)
            {
                int wishId = Convert.ToInt32(RouteData.Values["id"]);
                int whoGivesId = (await db.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name)).Id;
                TakenWish takenWish = (await db.TakenWishes.FirstOrDefaultAsync(t => (t.WishId == wishId && t.WhoGivesId == whoGivesId)));
                db.Entry(takenWish).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index", "Account");
        }
    }
}