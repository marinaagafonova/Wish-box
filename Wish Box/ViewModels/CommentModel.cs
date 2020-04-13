using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.ViewModels
{
    public class CommentModel
    {
        public string Description { get; set; }
        public int WishId { get; set; }
        public int InReplyId { get; set; }
    }
}
