using System;
using Xunit;
using Moq;
using Wish_Box.Repositories;
using Wish_Box.Models;
using Wish_Box.Controllers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace XUnitTestsWB
{
    public class WishUnitTest
    {
        private Mock<IRepository<Wish>> Wish_rep { get; set; }

        private Mock<IRepository<User>> User_rep { get; set; }
        private Mock<IRepository<WishRating>> WishRate_rep { get; set; }
        private Mock<IRepository<Comment>> Comment_rep { get; set; }

        [Fact]
        public async Task EditWishAsync()
        {
            //Arrange

            Wish_rep = new Mock<IRepository<Wish>>();
            User_rep = new Mock<IRepository<User>>();
            WishRate_rep = new Mock<IRepository<WishRating>>();
            Comment_rep = new Mock<IRepository<Comment>>();
            Wish_rep.Setup(repo => repo.GetAll()).Returns(GetTestWishes());
            int id = 1;

            Wish_rep.Setup(repo => repo.Get(id)).Returns(GetWish());
            User_rep.Setup(repo => repo.GetAll()).Returns(TestData.GetTestUsers());

            Mock<IWebHostEnvironment> app_host = new Mock<IWebHostEnvironment>();

            WishController controller = new WishController(Wish_rep.Object, User_rep.Object, WishRate_rep.Object, Comment_rep.Object, app_host.Object);

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

            WishViewModel wvm = new WishViewModel()
            {
                Id = 1,
                Description = "new desc of wish 1 of user 2",
                Attachment = null
            };

            // Act
            var result = await controller.Edit(1, wvm);

            // Assert
            var viewResult = Assert.IsType<JsonResult>(result);
            dynamic resultData = new JsonResultDynamicWrapper(viewResult);
            Assert.Equal(true, resultData.success);
        }

        private Task<Wish> GetWish()
        {
            Wish fakewish = new Wish()
            {
                Id = 1,
                Description = "Wish 1 of user 2",
                IsTaken = false,
                UserId = 2,
                Rating = 0
            };
            return Task.Delay(0)
                .ContinueWith(t => fakewish);
        }

        private List<Wish> GetTestWishes()
        {
            var wishes = new List<Wish>
                {
                new Wish { Id=1,
                Description = "Wish 1 of user 2",
                IsTaken = false,
                UserId = 2,
                Rating = 0},
                new Wish { Id=2,
                Description = "Wish 2 of user 1",
                IsTaken = false,
                UserId = 1,
                Rating = 0},
                new Wish { Id=3,
                Description = "Wish 3 of user 1",
                IsTaken = false,
                UserId = 1,
                Rating = 0},
                new Wish { Id=4,
                Description = "Wish 4 of user 3",
                IsTaken = false,
                UserId = 2,
                Rating = 0},
                };
            return wishes;
        }



    }
}
