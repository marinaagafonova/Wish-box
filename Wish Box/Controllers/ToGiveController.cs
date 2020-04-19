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
            if(User.Identity.Name != null)
            {
                List<int> wishId = await (db.TakenWishes
                    .Where(w => w.WhoGivesId == (db.Users.FirstOrDefault(u => u.Login == User.Identity.Name)).Id)
                    .Select(w => w.WishId)).ToListAsync();
                var wishes = await db.Wishes.Where(w => wishId.Contains(w.Id)).ToListAsync();
                return View(wishes);
            }
            return NotFound();
        }
    }
}