using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Models
{
    public class UserListViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public SelectList Countries { get; set; }
        public SelectList Cities { get; set; }
        //public SelectList Ages { get; set; }
        public string Name { get; set; }
        public List<int> Following_ids { get; set; }
    }
}
