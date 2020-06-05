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
using Wish_Box.Repositories;

namespace Wish_Box.Controllers
{
    public class HomeController : Controller
    {
        private List<string> countries;
        private readonly IRepository<User> user_rep;
        private readonly IRepository<Following> following_rep;
        private readonly IRepository<Wish> wish_rep;


        public HomeController(IRepository<Wish> wishRepository, IRepository<User> userRepository, IRepository<Following> followingRepository)
        {
            user_rep = userRepository;
            following_rep = followingRepository;
            wish_rep = wishRepository;
        }

        //[Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var current_user = user_rep.FindFirstOrDefault(p => p.Login == User.Identity.Name);
                var followings = following_rep.Find(p => p.UserFId == current_user.Id).ToList();
                var wishes = new List<Wish>();
                if(followings.Count > 0)
                {
                    var is_followed_id = from f in followings select f.UserIsFId;
                    wishes = wish_rep.Find(p => is_followed_id.Contains(p.UserId)).OrderByDescending(p => p.Id).ToList();
                }
                
                foreach (var wish in wishes)
                {
                    wish.User = await user_rep.FindFirstOrDefault(p => p.Id == wish.UserId);
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
            var users = user_rep.Find(u => u.Login.Contains(keyword)).ToList();
            List<string> cities = GetCities("Afghanistan");
            GetCountries();
            var current_user = await user_rep.FindFirstOrDefault(p => p.Login == User.Identity.Name);
            var following_ids = following_rep.Find(p => p.UserFId == current_user.Id).Select(p => p.UserIsFId).ToList(); 
            if (model.Users == null)
            {
                model = new UserListViewModel
                {
                    Users = users.ToList(),
                    Countries = new SelectList(countries),
                    Cities = new SelectList(cities),
                    Name = keyword,
                    Following_ids = following_ids.ToList()
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
            IQueryable<User> users = user_rep.GetAll().AsQueryable();
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
            var current_user = await user_rep.FindFirstOrDefault(p => p.Login == User.Identity.Name);
            var following_ids = following_rep.Find(p => p.UserFId == current_user.Id).Select(p => p.UserIsFId).ToList(); 

            UserListViewModel viewModel = new UserListViewModel
            {
                Users = users.ToList(),
                Countries = new SelectList(countries, country),
                Cities = new SelectList(cities, city),
                Name = name,
                Following_ids = following_ids.ToList()
            };

            ViewBag.keyword = name;

            return View("Search", viewModel);
        }

    }
}
