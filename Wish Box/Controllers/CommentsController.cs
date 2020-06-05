using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wish_Box.Models;
using Wish_Box.Repositories;
using Wish_Box.ViewModels;

namespace Wish_Box.Controllers
{
    public class CommentsController : Controller
    {
        //private readonly AppDbContext db;
        private readonly IRepository<Comment> comment_rep;
        private readonly IRepository<User> user_rep;
        private readonly IRepository<Wish> wish_rep;

        public CommentsController(IRepository<Wish> wishRepository, IRepository<User> userRepository, IRepository<Comment> commentRepository)
        {
            comment_rep = commentRepository;
            user_rep = userRepository;
            wish_rep = wishRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetComments()
        {
            string id = RouteData.Values["id"].ToString();
            var wishId = Convert.ToInt32(RouteData.Values["id"]);
            var comments = comment_rep.Find(c => c.WishId == wishId)/*.OrderBy(p=>p.Id).ToList()*/;
            if(comments != null)
            {
                comments = comments.OrderBy(p => p.Id).ToList();
            }
            List<CommentViewModel> commentModels = new List<CommentViewModel>();
            foreach(Comment c in comments)
            {
                var currentUser = await user_rep.FindFirstOrDefault(u => u.Id == c.UserId);
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
            ViewBag.wishId = wishId;
            return PartialView(new CommentViewModel { WishId = wishId });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(CommentViewModel comment)
        {
            var user = await user_rep.FindFirstOrDefault(u => u.Login == User.Identity.Name);
            var wish = await wish_rep.FindFirstOrDefault(p => p.Id == comment.WishId);
            if (comment != null && user != null && wish != null && comment.Description != "")
            {
                Comment commentEntity = new Comment
                {
                    Description = comment.Description,
                    WishId = comment.WishId,
                    UserId = user.Id,
                    InReplyId = comment.InReplyId
                };
                await comment_rep.Create(commentEntity);
            }
            return RedirectToAction("Show", "UserPage", new { id = (await user_rep.FindFirstOrDefault(u => u.Id == wish.UserId)).Login });
        }

        [HttpPost]//[HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var commentId = Convert.ToInt32(RouteData.Values["id"]);
            var wishId = (await comment_rep.FindFirstOrDefault(c => c.Id == commentId)).WishId;
            var userId = (await wish_rep.FindFirstOrDefault(p => p.Id == wishId)).UserId;
            var username = (await user_rep.FindFirstOrDefault(u => u.Id == userId)).Login;
            if (commentId > 0 && User.Identity.Name != null)
            {
                await comment_rep.Delete(commentId);
                return RedirectToAction("Show", "UserPage", new { id = username });
            }
            return NotFound();
        }
    }
}