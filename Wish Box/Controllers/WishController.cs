using System;
using System.Collections.Generic;
using System.IO;
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

        //public async Task<IActionResult> Index()
        //{
        //    return View(await db.Wishes.ToListAsync());
        //}

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(WishViewModel wvm)
        {
            Wish wish = new Wish
            {
                Description = wvm.Description
            };
            if (wvm.Attachment != null)
            {
                byte[] imageData = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(wvm.Attachment.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)wvm.Attachment.Length);
                }
                // установка массива байтов
                wish.Attachment = imageData;
            }
            wish.IsTaken = false;
            wish.User = db.Users.Where(p => p.Login == User.Identity.Name).ToList()[0];
            wish.UserId = wish.User.Id;
            db.Wishes.Add(wish);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id != null)
        //    {
        //        Wish wish = await db.Wishes.FirstOrDefaultAsync(p => p.Id == id);
        //        if (wish != null)
        //            return View(wish);
        //    }
        //    return NotFound();
        //}

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