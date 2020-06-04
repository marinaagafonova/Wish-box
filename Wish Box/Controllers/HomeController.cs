using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Wish_Box.Models;
using PagedList.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Wish_Box.ViewModels;
using Microsoft.Data.SqlClient;

namespace Wish_Box.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext db;
        private List<string> countries;


        public HomeController(AppDbContext context)
        {
            db = context;
        }

        //[Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var current_user = await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name);
                List<int> is_followed_id = await db.Followings.Where(p => p.UserFId == current_user.Id).Select(p => p.UserIsFId).ToListAsync();
                List<Wish> wishes = await db.Wishes.Where(p => is_followed_id.Contains(p.UserId)).OrderByDescending(p => p.Id).ToListAsync();
                foreach (var wish in wishes)
                {
                    wish.User = db.Users.FirstOrDefault(p => p.Id == wish.UserId);
                }
                return View(wishes);
            }
            return RedirectToAction("Index", "Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "Account");
        }

        [HttpGet] 
        public async Task<IActionResult> Search(UserListViewModel model = null)
        {
            var keyword = Request.Query["keyword"].ToString();
            var users = db.Users.Where(u => u.Login.Contains(keyword));
            List<string> cities = GetCities("Afghanistan");
            GetCountries();
            var current_user = await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name);
            List<int> following_ids = await db.Followings.Where(p => p.UserFId == current_user.Id).Select(p => p.UserIsFId).ToListAsync();
            if (model.Users == null)
            {
                model = new UserListViewModel
                {
                    Users = users.ToList(),
                    Countries = new SelectList(countries),
                    Cities = new SelectList(cities),
                    Name = keyword,
                    Following_ids = following_ids
                };
            }
            ViewBag.keyword = keyword;
            return View(model);
        }


        private List<string> GetCities(string id)
        {
            List<string> cities = new List<string>();
            int country_id = 0;

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
                        country_id = Convert.ToInt32(dataReader["CountryId"]);
                    }
                }
                string sql2 = "Select CityName From City Where CountryId=" + country_id;
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
            //ViewBag.Cities = cities;
            //return PartialView("DisplayCities");
            return cities;
        }
        private void GetCountries()
        {
            if (countries == null)
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

        public async Task<IActionResult> Filter(string country, string city, string name)
        {
            IQueryable<User> users = db.Users;
            if (country != null && country != "" && country != "Все")
            {
                users = users.Where(p => p.Country == country);
            }
            if (city != null && city != "" && city != "Все")
            {
                users = users.Where(p => p.City == city);
            }
            if (!String.IsNullOrEmpty(name))
            {
                users = users.Where(p => p.Login.Contains(name));
            }


            List<string> cities = GetCities(country);
            var current_user = await db.Users.FirstOrDefaultAsync(p => p.Login == User.Identity.Name);
            List<int> following_ids = await db.Followings.Where(p => p.UserFId == current_user.Id).Select(p => p.UserIsFId).ToListAsync();

            UserListViewModel viewModel = new UserListViewModel
            {
                Users = users.ToList(),
                Countries = new SelectList(countries, country),
                Cities = new SelectList(cities, city),
                Name = name,
                Following_ids = following_ids
            };

            ViewBag.keyword = name;

            return View("Search", viewModel);
        }

    }
}
