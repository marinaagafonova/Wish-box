﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wish_Box.ViewModels; // пространство имен моделей RegisterModel и LoginModel
using Wish_Box.Models; // пространство имен UserContext и класса User
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IO;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Wish_Box.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext db;
        IWebHostEnvironment _appEnvironment;

        public AccountController(AppDbContext context, IWebHostEnvironment appEnvironment)
        {
            db = context;
            _appEnvironment = appEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(model.Login); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return PartialView(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Login);
                if (user == null)
                {
                    User new_user = new User
                    {
                        Login = model.Login,
                        Password = model.Password,
                        dayOfBirth = model.dayOfBirth,
                        Country = model.Country,
                        City = model.City
                    };
                    if (model.Avatar != null)
                    {
                        string path = "/Files/" + model.Avatar.FileName;
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await model.Avatar.CopyToAsync(fileStream);
                        }
                        new_user.Avatar = path;
                    }
                    // добавляем пользователя в бд
                    db.Users.Add(new_user);
                    await db.SaveChangesAsync();

                    await Authenticate(model.Login); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return PartialView(model);
        }
        public async Task<IActionResult> Edit()
        {
            if (User.Identity.Name != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name);
                if (user != null)
                {
                    Edit1Model e = new Edit1Model()
                    {
                        City = user.City,
                        Country = user.Country,
                        dayOfBirth = user.dayOfBirth,
                        Login = user.Login
                    };
                    return View(e);
                }
            }
            return NotFound();//может сделать страничку "вы должны быть авторизованны для этого действия"
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Edit1Model model)
        {
            if (ModelState.IsValid)
            {
                string name = User.Identity.Name;
                if (name != null)
                {
                    if (CheckUserName(name, model.Login).Result)
                    {
                        User user = await db.Users.FirstOrDefaultAsync(p => p.Login == name);
                        user.Login = model.Login;
                        user.City = model.City;
                        user.Country = model.Country;
                        user.dayOfBirth = model.dayOfBirth;
                        db.Users.Update(user);
                        await db.SaveChangesAsync();
                        await Authenticate(model.Login);
                        return RedirectToAction("Show", "UserPage", new { id = model.Login });
                    }
                    else
                        ModelState.AddModelError("error - login isn't unique", "Имя пользователя занято!");
                }
                else return NotFound();
            }
            return View(model);
        }
        private async Task<bool> CheckUserName(string oldLogin, string newLogin)
        {
            if (oldLogin == newLogin)
                return true;
            else
                return await db.Users.FirstOrDefaultAsync(p => p.Login == newLogin) == null;
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            if (User.Identity.Name != null && await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name) != null)
                return View();
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                string name = User.Identity.Name;
                if (name != null)
                {
                    User user = await db.Users.FirstOrDefaultAsync(p => p.Login == name);
                    if (user.Password == model.OldPassword)
                    {
                        user.Password = model.Password;
                        db.Users.Update(user);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Старый пароль введён неверно");
                        return View(model);
                    }
                }
                return NotFound();
            }
            return View(model);
        }
        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Account");
        }
    }
}