using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wish_Box.ViewModels; // пространство имен моделей RegisterModel и LoginModel
using Wish_Box.Models; // пространство имен UserContext и класса User
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Wish_Box.Controllers
{
    public class AccountController : Controller
    {
        private UserContext db;
        public AccountController(UserContext context)
        {
            db = context;
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
                    // добавляем пользователя в бд
                    db.Users.Add(new User { Login = model.Login, Password = model.Password, dayOfBirth = model.dayOfBirth, 
                        Country = model.Country, City = model.City });
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
            string name = User.Identity.Name;
            if (name != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Login == name);
                if (user != null)
                {
                    EditModel e = new EditModel() 
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
        public async Task<IActionResult> Edit(EditModel model)
        {
            if(ModelState.IsValid)
            {
                string name = User.Identity.Name;
                if (name != null)
                {
                    User user = await db.Users.FirstOrDefaultAsync(p => p.Login == name);
                    user.Login = model.Login;
                    user.City = model.City;
                    user.Country = model.Country;
                    user.dayOfBirth = model.dayOfBirth;
                    db.Users.Update(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
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
