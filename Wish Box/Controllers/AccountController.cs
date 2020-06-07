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
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Wish_Box.Repositories;

namespace Wish_Box.Controllers
{
    [ApiController]
    public class AccountController : Controller
    {
        //private readonly AppDbContext db;
        private readonly IRepository<User> userRepository;

        private readonly IWebHostEnvironment _appEnvironment;
        private List<string> countries;

        private List<string> formats = new List<string>()
        {
            ".gif",".jpg",".jpeg",".png"
        };
        public AccountController(IRepository<User> userRepository, IWebHostEnvironment appEnvironment, IConfiguration configuration)
        {
            //db = context;
            this.userRepository = userRepository;
            _appEnvironment = appEnvironment;
        }
        private void InitCountryList()
        {
            if(countries == null)
            {
                countries = new List<string>();
                string connectionString = ConnectionString.Value;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "Select * From country";
                    SqlCommand command = new SqlCommand(sql, connection);
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            Country country = new Country();
                            country.Id = Convert.ToInt32(dataReader["CountryId"]);
                            country.CountryName = Convert.ToString(dataReader["CountryName"]);
                            countries.Add(country.CountryName);
                        }
                    }
                    connection.Close();
                }
            }
        }
        [HttpGet("[controller]/[action]/")]
        public IActionResult GetCityList(string id)
        {
            List<string> cities = new List<string>();
            List<int> country_id = new List<int>();
            string connectionString = ConnectionString.Value;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql1 = string.Format("Select CountryId From country Where CountryName='{0}'", id);
                SqlCommand command1 = new SqlCommand(sql1, connection);
                using (SqlDataReader dataReader = command1.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        country_id.Add(Convert.ToInt32(dataReader["CountryId"]));
                    }
                }
                string sql2 = "Select CityName From City Where CountryId=" + country_id[0];
                SqlCommand command2 = new SqlCommand(sql2, connection);
                using (SqlDataReader dataReader = command2.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        cities.Add(Convert.ToString(dataReader["CityName"]));
                    }
                }
                connection.Close();
            }
            ViewBag.Cities = cities;
            return PartialView("DisplayCities");
        }

        [HttpGet("[controller]/[action]/")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("[controller]/[action]/")]
        public IActionResult Login()
        {
            return PartialView();
        }
        [HttpPost("[controller]/[action]/")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm]LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var hash_pass = Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(model.Password)));
                User user = await userRepository.FindFirstOrDefault(u => u.Login == model.Login && u.Password == hash_pass);
                if (user != null)
                {
                    await Authenticate(model.Login); // аутентификация
                    return PartialView("SuccessLogin");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return PartialView("Login", model);
        }
        [HttpGet("[controller]/[action]/")]
        public IActionResult Register()
        {
            InitCountryList();
            SelectList sl = new SelectList(countries);
            ViewBag.Countries = sl;
            List<string> cities = new List<string>();
            string connectionString = ConnectionString.Value;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int countryId = 0;
                string sql1 = string.Format("Select CountryId From country Where CountryName='{0}'", "Afghanistan");
                SqlCommand command1 = new SqlCommand(sql1, connection);
                using (SqlDataReader dataReader = command1.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        countryId = Convert.ToInt32(dataReader["CountryId"]);
                    }
                }
                string sql2 = "Select CityName From City Where CountryId=" + countryId;
                SqlCommand command2 = new SqlCommand(sql2, connection);
                using (SqlDataReader dataReader = command2.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        cities.Add(Convert.ToString(dataReader["CityName"]));
                    }
                }
                connection.Close();
            }
            ViewBag.CitiesFirst = cities;
            return PartialView();
        }
        [HttpPost("[controller]/[action]/")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm]RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userRepository.FindFirstOrDefault(u => u.Login == model.Login);
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
                        string extension = Path.GetExtension(model.Avatar.FileName);
                        if (!formats.Contains(extension))
                        {
                            ModelState.AddModelError("Error", "Wrong file type");
                            return PartialView("Register");
                        }
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await model.Avatar.CopyToAsync(fileStream);
                        }
                        new_user.Avatar = path;
                    }
                    // добавляем пользователя в бд
                    await userRepository.Create(new_user);
                    //await db.SaveChangesAsync();

                    await Authenticate(model.Login); // аутентификация

                    return PartialView("SuccessRegister");
                }
                    ModelState.AddModelError("", "Имя пользователя занято");
            }
            return PartialView("Register", model);
        }
        [HttpGet("[controller]/[action]/")]
        public async Task<IActionResult> Edit()
        {
            if (User.Identity.Name != null)
            {
                InitCountryList();
                SelectList sl = new SelectList(countries);
                ViewBag.Countries = sl;
                List<string> cities = new List<string>();
                string connectionString = ConnectionString.Value;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int countryId = 0;
                    string sql1 = string.Format("Select CountryId From country Where CountryName='{0}'", "Afghanistan");
                    SqlCommand command1 = new SqlCommand(sql1, connection);
                    using (SqlDataReader dataReader = command1.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            countryId = Convert.ToInt32(dataReader["CountryId"]);
                        }
                    }
                    string sql2 = "Select CityName From City Where CountryId=" + countryId;
                    SqlCommand command2 = new SqlCommand(sql2, connection);
                    using (SqlDataReader dataReader = command2.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            cities.Add(Convert.ToString(dataReader["CityName"]));
                        }
                    }
                    connection.Close();
                }
                ViewBag.CitiesFirst = cities;
                User user = await userRepository.FindFirstOrDefault(p => p.Login == User.Identity.Name);
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
        [HttpPut("[controller]/[action]/")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm]Edit2Model model)
        {
            if (ModelState.IsValid)
            {
                string name = User.Identity.Name;
                if (name != null)
                {
                    if (CheckUserName(name, model.Login).Result)
                    {
                        User user = await userRepository.FindFirstOrDefault(p => p.Login == name);
                        user.Login = model.Login;
                        user.City = model.City;
                        user.Country = model.Country;
                        user.dayOfBirth = model.dayOfBirth;
                        if (model.Avatar != null)
                        {
                            string path = "/Files/" + model.Avatar.FileName;
                            string extension = Path.GetExtension(model.Avatar.FileName);
                            if (!formats.Contains(extension))
                            {
                                ModelState.AddModelError("Error", "Wrong file type");
                                Edit1Model e = new Edit1Model()
                                {
                                    City = model.City,
                                    Country = model.Country,
                                    dayOfBirth = model.dayOfBirth,
                                    Login = model.Login,
                                    Avatar = user.Avatar
                                };
                                return View("Edit", e);
                            }
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
                        await userRepository.Update(user);
                        //await db.SaveChangesAsync();
                        Authenticate(model.Login);
                        return Json(new { success = true, responseText = model.Login });
                        //return RedirectToAction("Show", "UserPage", new { id = model.Login });
                        //return Redirect(Request.Headers["Referer"].ToString());
                    }
                    else
                        ModelState.AddModelError("error - login isn't unique", "Имя пользователя занято!");
                }
                else return NotFound();
            }
            return View(model);
        }
        [NonAction]
        private async Task<bool> CheckUserName(string oldLogin, string newLogin)
        {
            if (oldLogin == newLogin)
                return true;
            else
                return await userRepository.FindFirstOrDefault(p => p.Login == newLogin) == null;
        }
        [HttpGet("[controller]/[action]/")]
        public async Task<IActionResult> ChangePassword()
        {
            if (User.Identity.Name != null && await userRepository.FindFirstOrDefault(p => p.Login == User.Identity.Name) != null)
                return View();
            return NotFound();
        }
        [HttpPut("[controller]/[action]/")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([FromForm]ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                string name = User.Identity.Name;
                if (name != null)
                {
                    User user = await userRepository.FindFirstOrDefault(p => p.Login == name);
                    var hash_pass = Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(model.OldPassword)));
                    if (user.Password == hash_pass)
                    {
                        user.Password = Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(model.Password)));
                        await userRepository.Update(user);
                        return Json(new { success = true, responseText = "Pass Edited!" });
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
        [NonAction]
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

        [HttpGet("[controller]/[action]/")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Account");
        }
    }
}
