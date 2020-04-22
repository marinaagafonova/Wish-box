using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wish_Box.Models;

namespace Wish_Box.ViewModels
{
    public class UserPageViewModel
    {
        public User User { get; set; }
        public User CurrentUser { get; set; }
        public List<Wish> UserWishes { get; set; }
        public List<int> Followers { get; set; }
        public List<int> TakenWishes { get; set; }
    }
}
