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
        private AppDbContext db;
        
        public ToGiveController(AppDbContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            if(User.Identity.Name != null)
            {
                int id = (await db.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name)).Id;
                var takenWishes = await( db.TakenWishes.Where(w => w.WhoGivesId == id)).ToListAsync();
                List<int> wishId = new List<int>();
                foreach(TakenWish t in takenWishes)
                    wishId.Add(t.WishId);
                var wishes = await db.Wishes.Where(w => wishId.Contains(w.Id)).ToListAsync();
                //wishId.Clear();
                //foreach (Wish w in wishes)
                //    wishId.Add(w.UserId);
                //var users = await db.Users.Where(w => wishId.Contains(w.Id)).ToListAsync();
                //List<string> names = new List<string>();
                //foreach (User u in users)
                //    names.Add(u.Login);
                //ViewBag.Names = names;
                return View(wishes);
            }
            return NotFound();
        }
    }
}