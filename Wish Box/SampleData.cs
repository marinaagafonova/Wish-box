using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wish_Box.Models;

namespace Wish_Box
{
    public class SampleData
    {
        public static void Initialize (AppDbContext context)
        {
            if(!context.Users.Any())
            {
                context.Users.Add(
                    new User
                    {
                        Login = "test_user",
                        Password = "test",
                        dayOfBirth = DateTime.Now,
                        Country = "Russia",
                        City = "Voronezh"
                    }
                    );
                context.SaveChanges();
            }
        }
    }
}
