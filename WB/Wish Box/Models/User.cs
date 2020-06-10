using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime dayOfBirth { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Avatar { get; set; }

    }
}
