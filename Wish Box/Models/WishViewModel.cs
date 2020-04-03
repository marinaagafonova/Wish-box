using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Models
{
    public class WishViewModel
    {
        public string Description { get; set; }
        public bool IsTaken { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public IFormFile Attachment { get; set; }
    }
}
