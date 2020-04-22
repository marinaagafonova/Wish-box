using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Models
{
    public class Wish
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsTaken { get; set; }
        public int UserId {get;set;}
        public User User { get; set; }
        public string Attachment { get; set; }
        public int Rating { get; set; }
    }
}
