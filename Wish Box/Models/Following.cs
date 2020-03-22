using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Models
{
    public class Following
    {
        public int WhoFollowsId { get; set; }
        public User WhoFollows { get; set; }
        public int WhoIsFollowedId { get; set; }
        public User WhoIsFollowed { get; set; }
    }
}
