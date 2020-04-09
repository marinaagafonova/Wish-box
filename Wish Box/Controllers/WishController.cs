using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wish_Box.Models;
using System.Drawing;

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

        public async Task<IActionResult> Edit()
        {
            var id = Convert.ToInt32(RouteData.Values["id"]);
            if (id >=0)
            {
                Wish wish = await db.Wishes.FirstOrDefaultAsync(p => p.Id == id);
                if (wish != null)
                {
                    return View(wish);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(WishViewModel wvm)
        {
            Wish wish = await db.Wishes.FirstOrDefaultAsync(p => p.Id == wvm.Id);
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
            if(wvm.Description != null)
            {
                wish.Description = wvm.Description;
            }
            db.Wishes.Update(wish);
            await db.SaveChangesAsync();
            return RedirectToAction("OwnList", "Wish");
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

        [HttpGet]
        public async Task<IActionResult> OwnList()
        {
            var current_user = await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name);
            var wishes = db.Wishes.Where(p => p.UserId == current_user.Id).ToList();
            return View(wishes);
        }
    }
}