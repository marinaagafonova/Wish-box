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
using Microsoft.AspNetCore.Hosting;

namespace Wish_Box.Controllers
{
    public class WishController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly AppDbContext db;

        public WishController(AppDbContext context, IWebHostEnvironment appEnvironment)
        {
            db = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Create(WishViewModel wvm)
        {
            if (User.Identity.IsAuthenticated)
            {
                Wish wish = new Wish
                {
                    Description = wvm.Description
                };
                if (wvm.Attachment != null)
                {
                    string path = "/Files/" + wvm.Attachment.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await wvm.Attachment.CopyToAsync(fileStream);
                    }
                    wish.Attachment = path;
                }
                wish.IsTaken = false;
                wish.User = db.Users.Where(p => p.Login == User.Identity.Name).ToList()[0]; //await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name);
                wish.UserId = wish.User.Id;
                db.Wishes.Add(wish);
                await db.SaveChangesAsync();
                return RedirectToAction("OwnList");
            }
            return RedirectToAction("Index", "Account");
        }

        public async Task<IActionResult> Edit()
        {
            if (User.Identity.IsAuthenticated)
            {
                var id = Convert.ToInt32(RouteData.Values["id"]);
                if (id >= 0)
                {
                    Wish wish = await db.Wishes.FirstOrDefaultAsync(p => p.Id == id);
                    if (wish != null)
                    {
                        User user = await db.Users.FirstOrDefaultAsync(p => p.Id == wish.UserId);
                        if (user != null && user.Login == User.Identity.Name)
                            return View(wish);
                    }
                }
                return NotFound();

            }
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(WishViewModel wvm)
        {
            if (User.Identity.IsAuthenticated)
            {
                Wish wish = await db.Wishes.FirstOrDefaultAsync(p => p.Id == wvm.Id);
                if (wvm.Attachment != null)
                {
                    string path = "/Files/" + wvm.Attachment.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await wvm.Attachment.CopyToAsync(fileStream);
                    }
                    if (System.IO.File.Exists(_appEnvironment.WebRootPath + wish.Attachment))
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath + wish.Attachment);
                    }
                    wish.Attachment = path;
                }
                if (wvm.Description != null)
                {
                    wish.Description = wvm.Description;
                }
                db.Wishes.Update(wish);
                await db.SaveChangesAsync();
                return RedirectToAction("OwnList");

            }
            return RedirectToAction("Index", "Account");
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete()
        {
            if (User.Identity.IsAuthenticated)
            {
                var id = Convert.ToInt32(RouteData.Values["id"]);
                if (id >= 0)
                {
                    Wish wish = await db.Wishes.FirstOrDefaultAsync(p => p.Id == id);
                    if (wish != null)
                        return PartialView(wish);
                }
                return NotFound();

            }
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            if (User.Identity.IsAuthenticated)
            {
                var id = Convert.ToInt32(RouteData.Values["id"]);
                if (id > 0)
                {
                    Wish wish = new Wish { Id = id };
                    var comments = await db.Comments.Where(p => p.WishId == id).ToListAsync();
                    foreach(var comment in comments)
                    {
                        db.Entry(comment).State = EntityState.Deleted;
                    }
                    db.Entry(wish).State = EntityState.Deleted;
                    await db.SaveChangesAsync();
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                return NotFound();

            }
            return RedirectToAction("Index", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> OwnList()
        {

            if (User.Identity.IsAuthenticated)
            {
                var current_user = await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name);
                var wishes = await db.Wishes.Where(p => p.UserId == current_user.Id).ToListAsync();
                return View(wishes);
            }
            return RedirectToAction("Index", "Account");
        }

        public async Task<IActionResult> RatingPlus()
        {
            if (User.Identity.IsAuthenticated)
            {
                var wish_id = Convert.ToInt32(RouteData.Values["id"]);
                var currentUser = await db.Users.FirstOrDefaultAsync(x => x.Login == User.Identity.Name);
                var currentWish = await db.Wishes.FirstOrDefaultAsync(x => x.Id == wish_id);
                var currentRate = await db.WishRatings.FirstOrDefaultAsync(x => x.UserId == currentUser.Id && x.WishId == wish_id);

                if (currentRate == null)
                {
                    currentWish.Rating += 1;
                    db.Wishes.Update(currentWish);

                    db.WishRatings.Add(new WishRating()
                    {
                        WishId = wish_id,
                        UserId = currentUser.Id,
                        Rate = true
                    });
                }
                else
                {
                    if (!currentRate.Rate)
                    {
                        currentWish.Rating += 2;
                        db.Wishes.Update(currentWish);

                        currentRate.Rate = true;
                        db.WishRatings.Update(currentRate);
                    }
                }

                if (currentWish.Rating > -5)
                {
                    currentWish.IsVisible = true;
                    db.Wishes.Update(currentWish);
                }
                await db.SaveChangesAsync();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index", "Account");
        }

        public async Task<IActionResult> RatingMinus()
        {
            if (User.Identity.IsAuthenticated)
            {
                var wish_id = Convert.ToInt32(RouteData.Values["id"]);
                var currentUser = await db.Users.FirstOrDefaultAsync(x => x.Login == User.Identity.Name);
                var currentWish = await db.Wishes.FirstOrDefaultAsync(x => x.Id == wish_id);
                var currentRate = await db.WishRatings.FirstOrDefaultAsync(x => x.UserId == currentUser.Id && x.WishId == wish_id);

                if (currentRate == null)
                {
                    currentWish.Rating -= 1;
                    db.Wishes.Update(currentWish);

                    db.WishRatings.Add(new WishRating()
                    {
                        WishId = wish_id,
                        UserId = currentUser.Id,
                        Rate = false
                    });
                }
                else
                {
                    if (currentRate.Rate)
                    {
                        currentWish.Rating -= 2;
                        db.Wishes.Update(currentWish);

                        currentRate.Rate = false;
                        db.WishRatings.Update(currentRate);
                    }
                }

                if (currentWish.Rating <= -5)
                {
                    currentWish.IsVisible = false;
                    db.Wishes.Update(currentWish);
                }
                await db.SaveChangesAsync();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index", "Account");
        }
    }
}
