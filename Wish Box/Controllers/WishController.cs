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
using Wish_Box.Repositories;

namespace Wish_Box.Controllers
{
    public class WishController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IRepository<Wish> wish_rep;
        private readonly IRepository<User> user_rep;
        private readonly IRepository<WishRating> wishRate_rep;
        private readonly IRepository<Comment> comment_rep;

        private List<string> formats = new List<string>()
        {
            ".gif",".jpg",".jpeg",".png"
        };

        public WishController(IRepository<Wish> wishRepository, IRepository<User> userRepository, IRepository<WishRating> wishRateRepository, IRepository<Comment> commentRepository, IWebHostEnvironment appEnvironment)
        {
            wish_rep = wishRepository;
            user_rep = userRepository;
            wishRate_rep = wishRateRepository;
            comment_rep = commentRepository;
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
                    string extension = Path.GetExtension(wvm.Attachment.FileName);
                    if (!formats.Contains(extension))
                    {
                        ModelState.AddModelError("Error", "Wrong file type");
                        return View("Create");
                    }
                   
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await wvm.Attachment.CopyToAsync(fileStream);
                    }
                    wish.Attachment = path;
                }
                wish.IsTaken = false;
                wish.User = user_rep.Find(p => p.Login == User.Identity.Name).ToList()[0];
                wish.UserId = wish.User.Id;
                await wish_rep.Create(wish);
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
                    Wish wish = await wish_rep.FindFirstOrDefault(p => p.Id == id);
                    if (wish != null)
                    {
                        User user = await user_rep.Get(wish.UserId);
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
                Wish wish = await wish_rep.Get(wvm.Id);
                if (wvm.Attachment != null)
                {
                    string path = "/Files/" + wvm.Attachment.FileName;
                    string extension = Path.GetExtension(wvm.Attachment.FileName);
                    if (!formats.Contains(extension))
                    {
                        ModelState.AddModelError("Error", "Wrong file type");
                        wish.Description = wvm.Description;
                        return View("Edit", wish);
                    }
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
                await wish_rep.Update(wish);
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
                    Wish wish = await wish_rep.FindFirstOrDefault(p => p.Id == id);
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
                    var comments = comment_rep.Find(p => p.WishId == id);
                    foreach (var comment in comments)
                    {
                        await comment_rep.Delete(comment.Id);
                    }
                    await wish_rep.Delete(id);
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
                var current_user = await user_rep.FindFirstOrDefault(p => p.Login == User.Identity.Name);
                var wishes = wish_rep.Find(p => p.UserId == current_user.Id).ToList();
                return View(wishes);
            }
            return RedirectToAction("Index", "Account");
        }

        public async Task<int> GetRating()
        {
            try 
            {
                Wish wish = await wish_rep.Get(Convert.ToInt32(RouteData.Values["id"]));
                return wish.Rating;
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e.Message); return -999; 
            }
        }

        public async Task<IActionResult> RatingPlus()
        {
            if (User.Identity.IsAuthenticated)
            {
                var wish_id = Convert.ToInt32(RouteData.Values["id"]);
                var currentUser = user_rep.Find(x => x.Login == User.Identity.Name).ToList()[0];
                Wish currentWish = await wish_rep.FindFirstOrDefault(p => p.Id == wish_id);
                List<WishRating> currentRates = wishRate_rep.Find(x => x.UserId == currentUser.Id && x.WishId == wish_id).ToList();
                var currentRate = new WishRating();
                if (currentRates != null)
                    currentRate = currentRates[0];

                if (currentRate == null)
                {
                    currentWish.Rating += 1;
                    await wish_rep.Update(currentWish);

                    await wishRate_rep.Create(new WishRating()
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
                        await wish_rep.Update(currentWish);

                        currentRate.Rate = true;
                        await wishRate_rep.Update(currentRate);
                    }
                }

                if (currentWish.Rating > -5)
                {
                    currentWish.IsVisible = true;
                    await wish_rep.Update(currentWish);
                }
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index", "Account");
        }

        public async Task<IActionResult> RatingMinus()
        {
            if (User.Identity.IsAuthenticated)
            {
                var wish_id = Convert.ToInt32(RouteData.Values["id"]);
                var currentUser = user_rep.Find(x => x.Login == User.Identity.Name).ToList()[0];
                Wish currentWish = await wish_rep.FindFirstOrDefault(p => p.Id == wish_id);
                var currentRates = wishRate_rep.Find(x => x.UserId == currentUser.Id && x.WishId == wish_id).ToList();
                var currentRate = new WishRating();
                if (currentRates != null)
                    currentRate = currentRates[0];

                if (currentRate == null)
                {
                    currentWish.Rating -= 1;
                    await wish_rep.Update(currentWish);

                    await wishRate_rep.Create(new WishRating()
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
                        await wish_rep.Update(currentWish);

                        currentRate.Rate = false;
                        await wishRate_rep.Update(currentRate);
                    }
                }

                if (currentWish.Rating <= -5)
                {
                    currentWish.IsVisible = false;
                    await wish_rep.Update(currentWish);
                }
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index", "Account");
        }
    }
}
