using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Wish_Box.Controllers;
using Wish_Box.Models;
using Wish_Box.Repositories;
using Xunit;

namespace XUnitTestsWB.Unit_tests
{
    public class CommentUnitTests
    {
        [Fact]
        public async Task DeleteCommentAsync()
        {
            //Arrange
            Mock<IRepository<User>> user_rep = new Mock<IRepository<User>>();
            Mock<IRepository<Comment>> comment_rep = new Mock<IRepository<Comment>>();
            Mock<IRepository<Wish>> wish_rep = new Mock<IRepository<Wish>>();

            int testComId = 1;
            int testWishId = 1;
            int testUserId = 2;

            comment_rep.Setup(repo => repo.FindFirstOrDefault(c => c.Id == testComId)).Returns(Task.FromResult(new Comment 
            { 
                 Id = 1,
                 Description = "comment 1 of wish 1 of user 2",
                 WishId = 1,
                 UserId = 2
            }));

            wish_rep.Setup(repo => repo.FindFirstOrDefault(p => p.Id == testWishId)).Returns(Task.FromResult(new Wish
            {
                Id = 1,
                Description = "Wish 1 of user 2",
                IsTaken = false,
                UserId = 2,
                Rating = 0
            }));

            user_rep.Setup(repo => repo.FindFirstOrDefault(u => u.Id == testUserId)).Returns(Task.FromResult(new User()
            {
                Id = 2,
                Login = "login2",
                Password = "1",
                dayOfBirth = DateTime.Now,
                Country = "None",
                City = "None"
            }));

            CommentsController controller = new CommentsController(wish_rep.Object, user_rep.Object, comment_rep.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, "login2")
                        }, "login2"))
                }
            };

            // Act
            var result = await controller.Delete(testComId);

            // Assert
            var viewResult = Assert.IsType<JsonResult>(result);
            dynamic resultData = new JsonResultDynamicWrapper(viewResult);
            Assert.Equal(true, resultData.success);
        }
    }
}
