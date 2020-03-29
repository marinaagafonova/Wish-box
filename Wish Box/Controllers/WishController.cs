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
    public class WishController : Controller
    {
        private AppDbContext db;

        public WishController(AppDbContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Wishes.ToListAsync());
        }

        //how to include userId in wish, the author of the wish
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Wish wish)
        {
            db.Wishes.Add(wish);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                Wish wish = await db.Wishes.FirstOrDefaultAsync(p => p.Id == id);
                if (wish != null)
                    return View(wish);
            }
            return NotFound();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Wish wish = await db.Wishes.FirstOrDefaultAsync(p => p.Id == id);
                if (wish != null)
                    return View(wish);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Wish wish)
        {
                db.Wishes.Update(wish);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Wish wish = await db.Wishes.FirstOrDefaultAsync(p => p.Id == id);
                if (wish != null)
                    return View(wish);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Wish wish = new Wish { Id = id.Value };
                db.Entry(wish).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}