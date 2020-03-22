using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public int WishId { get; set; }
        public Wish Wish { get; set; }
        public int CommentId { get; set; }
        public Comment InReply { get; set; }
    }
}
