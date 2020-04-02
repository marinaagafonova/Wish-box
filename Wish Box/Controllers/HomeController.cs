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

namespace Wish_Box.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext db;

        public HomeController(AppDbContext context)
        {
            db = context;
        }

        //[Authorize]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(db.Users.ToList());
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
        public IActionResult Search(UserListViewModel model = null)
        {
            var keyword = Request.Query["keyword"].ToString();
            var users = db.Users.Where(u => u.Login.Contains(keyword));
            List<string> countries = GetCountries();
            List<string> cities = GetCities();
            if(model.Users == null)
            {
                model = new UserListViewModel
                {
                    Users = users.ToList(),
                    Countries = new SelectList(countries),
                    Cities = new SelectList(cities),
                    Name = keyword
                };
            }
            ViewBag.keyword = keyword;
            return View(model);
        }

        private List<string> GetCities()
        {
            List<string> cities = db.Users.Select(p => p.City).ToList();
            cities.Insert(0, "Все");
            return cities;
        }
        private List<string> GetCountries()
        {
            List<string> countries = db.Users.Select(p => p.Country).ToList();
            countries.Insert(0, "Все");
            return countries;
        }

        public IActionResult Filter(string country, string city, string name)
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

            List<string> countries = GetCountries();

            List<string> cities = GetCities();

            UserListViewModel viewModel = new UserListViewModel
            {
                Users = users.ToList(),
                Countries = new SelectList(countries),
                Cities = new SelectList(cities),
                Name = name
            };
            return View("Search", viewModel);
        }
    }
}
