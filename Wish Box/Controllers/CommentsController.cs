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
        private readonly AppDbContext db;

        public CommentsController(AppDbContext context)
        {
            db = context;
        }
        public async Task<IActionResult> GetComments()
        {
            var wishId = Convert.ToInt32(RouteData.Values["id"]);
            var comments = await db.Comments.Where(c => c.Wish.Id == wishId).OrderBy(p=>p.Id).ToListAsync();
            List<CommentViewModel> commentModels = new List<CommentViewModel>();
            foreach(Comment c in comments)
            {
                var currentUser = await db.Users.FirstOrDefaultAsync(u => u.Id == c.UserId);
                commentModels.Add(
                    new CommentViewModel
                    {
                        Id = c.Id,
                        Description = c.Description,
                        InReplyId = c.InReplyId,
                        WishId = c.WishId,
                        AuthorName = currentUser.Login,
                        Avatar = currentUser.Avatar
                    });
            }
            ViewBag.list = commentModels;
            return PartialView(new CommentViewModel { WishId = wishId });
        }

        [HttpPost]
        public IActionResult AddComment(CommentViewModel comment)
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
            return RedirectToAction("Show", "UserPage",new { id = db.Users.FirstOrDefault(u => u.Id == wish.UserId).Login });
        }

        [HttpPost]
        public IActionResult Delete()
        {
            var commentId = Convert.ToInt32(RouteData.Values["id"]);
            var username = db.Users.FirstOrDefault(u => u.Id ==
                            db.Wishes.FirstOrDefault(w => w.Id == 
                             db.Comments.FirstOrDefault(c => c.Id == commentId).WishId).UserId).Login;
            if (commentId > 0)
            {
                Comment c = new Comment { Id = commentId };
                db.Entry(c).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Show", "UserPage", new { id = username });
            }
            return NotFound();
        }
    }
}