using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Models
{
    public class TakenWish
    {
        public int Id { get; set; }
        public bool IsGiven { get; set; }


        public int WishId { get; set; }
        public Wish Wish { get; set; }
        public int WhoWishesId { get; set; }
       // public User WhoWishes { get; set; }
        public int WhoGivesId { get; set; }
    //    public User WhoGives { get; set; }
    }
}
