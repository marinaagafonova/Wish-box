using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Models
{
    public class WishRating
    {
        public int Id { get; set; }
        public int WishId { get; set; }
        public int UserId { get; set; }
        public bool Rate { get; set; }//false - minus, true - plus, null - null

    }
}
