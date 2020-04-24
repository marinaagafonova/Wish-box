using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Wish_Box.Models
{
    public class PersonViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime DayOfBirth { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public IFormFile Avatar { get; set; }
    }
}
