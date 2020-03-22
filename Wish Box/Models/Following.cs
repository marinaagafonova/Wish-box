using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Models
{
    public class Following
    {
        public int Id { get; set; }

        public int UserFId { get; set; }
    //    public User UserF { get; set; }
        public int UserIsFId { get; set; }
    //    public User UserIsF { get; set; }
    }
}
