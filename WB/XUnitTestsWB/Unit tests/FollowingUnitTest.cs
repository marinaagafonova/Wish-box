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
    public class FollowingUnitTest
    {
        private Mock<IRepository<User>> User_rep { get; set; }
        private Mock<IRepository<Following>> Following_rep { get; set; }

        [Fact]
        public async Task PostFollowingAsync()
        {
            //Arrange
            User_rep = new Mock<IRepository<User>>();
            Following_rep = new Mock<IRepository<Following>>();
            int test_id = 1;
            User_rep.Setup(repo => repo.GetAll()).Returns(TestData.GetTestUsers());
            

            FollowingsController controller = new FollowingsController(Following_rep.Object, User_rep.Object);

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
            string login = controller.ControllerContext.HttpContext.User.Identity.Name;
            User_rep.Setup(repo => repo.FindFirstOrDefault(p => p.Login == login)).Returns(Task.FromResult(new User()
            {
                Id = 2,
                Login = "login2",
                Password = "1",
                dayOfBirth = DateTime.Now,
                Country = "None",
                City = "None"
            }));
            // Act
            var result = await controller.PostFollowing(test_id);

            // Assert
            var viewResult = Assert.IsType<JsonResult>(result);
            dynamic resultData = new JsonResultDynamicWrapper(viewResult);
            Assert.Equal(true, resultData.success);
        }
        private async Task<User> GetUser()
        {
            User fakeuser = new User()
           {
               Id = 2,
               Login = "login2",
               Password = "1",
               dayOfBirth = DateTime.Now,
               Country = "None",
               City = "None"
           };
            return await Task.Delay(0)
                .ContinueWith(t => fakeuser);
        }
    }
}
