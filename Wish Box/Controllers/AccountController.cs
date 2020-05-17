using System.Collections.Generic;
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
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.Json;

namespace Wish_Box.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext db;
        private readonly IWebHostEnvironment _appEnvironment;

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
                var hash_pass = Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(model.Password)));
                User user = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == hash_pass);
                if (user != null)
                {
                    await Authenticate(model.Login); // аутентификация
                    return PartialView("SuccessLogin");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return PartialView("Login", model);
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
                    var hash_pass = Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(model.Password)));
                    User new_user = new User
                    {
                        Login = model.Login,
                        Password = hash_pass,
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

                    return PartialView("SuccessRegister");
                }
                    ModelState.AddModelError("", "Имя пользователя занято");
            }
            return PartialView("Register", model);
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
                        Login = user.Login,
                        Avatar = user.Avatar
                    };
                    return View(e);
                }
            }
            return NotFound();//может сделать страничку "вы должны быть авторизованны для этого действия"
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Edit2Model model)
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
                        if (model.Avatar != null)
                        {
                            string path = "/Files/" + model.Avatar.FileName;
                            using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                            {
                                await model.Avatar.CopyToAsync(fileStream);
                            }
                            if (System.IO.File.Exists(_appEnvironment.WebRootPath + user.Avatar))
                            {
                                System.IO.File.Delete(_appEnvironment.WebRootPath + user.Avatar);
                            }
                            user.Avatar = path;
                        }
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
                    var hash_pass = Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(model.OldPassword)));
                    if (user.Password == hash_pass)
                    {
                        user.Password = Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(model.Password)));
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
