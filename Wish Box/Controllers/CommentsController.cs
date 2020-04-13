using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wish_Box.Models;
using Wish_Box.ViewModels;

namespace Wish_Box.Controllers
{
    public class CommentsController : Controller
    {
        AppDbContext db;

        public CommentsController(AppDbContext context)
        {
            db = context;
        }
        public async Task<IActionResult> GetComments()
        {
            var wishId = Convert.ToInt32(RouteData.Values["id"]);
            return PartialView(await db.Comments.Where(c => c.Wish.Id == wishId).ToListAsync());
        }

        [HttpPost]
        public IActionResult AddComment(CommentModel comment)
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            var wish = db.Wishes.FirstOrDefault(p => p.Id == comment.WishId);
            if (comment != null && user != null && wish != null)
            {
                Comment commentEntity = new Comment
                {
                    Description = comment.Description,
                    WishId = comment.WishId,
                    UserId = user.Id,
                    InReplyId = comment.InReplyId
                };
                db.Comments.Add(commentEntity);
                db.SaveChanges();
            }
            return RedirectToAction("Show", "Users");
        }

        [HttpPost]
        public IActionResult Delete()
        {
            var commentId = Convert.ToInt32(RouteData.Values["id"]);
            if (commentId >= 0)
            {
                Comment c = new Comment { Id = commentId };
                db.Entry(c).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Show", "Users");
            }
            return NotFound();
        }
    }
}