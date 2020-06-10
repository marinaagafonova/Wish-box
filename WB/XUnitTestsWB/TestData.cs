using System;
using System.Collections.Generic;
using System.Text;
using Wish_Box.Models;

namespace XUnitTestsWB
{
    internal class TestData
    {
        internal static List<User> GetTestUsers()
        {
            var users = new List<User>
                {
                new User { Id = 1,
                Login = "login",
                Password = "1",
                dayOfBirth = DateTime.Now,
                Country = "None",
                City = "None"},
                new User { Id=2,
                Login = "login2",
                Password = "1",
                dayOfBirth = DateTime.Now,
                Country = "None",
                City = "None"},
                new User { Id=3,
                Login = "login3",
                Password = "1",
                dayOfBirth = DateTime.Now,
                Country = "None",
                City = "None"},
                };
            return users;
        }
    }
}
