using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Models
{
    public class Wish
    {
        public int Id { get; set; }
        //I'm unable to change DB in any way, so someone else should do it
        //add this ↓ please
        //public string Name { get; set; }
        public string Description { get; set; }
        public bool IsTaken { get; set; }
        public int UserId {get;set;}
        public User User { get; set; }
    }
}
