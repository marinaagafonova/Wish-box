using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Wish_Box.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int WishId { get; set; }
        public int? InReplyId { get; set; }
        public string AuthorName { get; set; }
        public string Avatar { get; set; }
    }
}
