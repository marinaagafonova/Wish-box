using System;
using Xunit;
using Moq;
using Wish_Box.Repositories;
using Wish_Box.Models;
using Wish_Box.Controllers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace XUnitTestsWB
{
    public class WishUnitTest
    {
        private Mock<IRepository<Wish>> Wish_rep { get; set; }

        private Mock<IRepository<User>> User_rep { get; set; }
        private Mock<IRepository<WishRating>> WishRate_rep { get; set; }
        private Mock<IRepository<Comment>> Comment_rep { get; set; }

        //private WishController Controller { get; }

        public WishUnitTest()
        {
            // Arrange
            //Wish_rep = new Mock<IRepository<Wish>>();
            //User_rep = new Mock<IRepository<User>>();

            //Wish_rep.Setup(repo => repo.GetAll()).Returns(GetTestWishes());
            //User_rep.Setup(repo => repo.GetAll()).Returns(GetTestUsers());

            //Mock<IWebHostEnvironment> app_host = new Mock<IWebHostEnvironment>();

            //Controller = new WishController(Wish_rep.Object, User_rep.Object, WishRate_rep.Object, Comment_rep.Object, app_host.Object);

            //User fakeUser = await User_rep.Object.Get(1);
        }


        [Fact]
        public async Task EditWishAsync()
        {
            Wish_rep = new Mock<IRepository<Wish>>();
            User_rep = new Mock<IRepository<User>>();
            WishRate_rep = new Mock<IRepository<WishRating>>();
            Comment_rep = new Mock<IRepository<Comment>>();
            Wish_rep.Setup(repo => repo.GetAll()).Returns(GetTestWishes());
            int id = 1;
            
            Wish_rep.Setup(repo => repo.Get(id)).Returns(GetWish());
            User_rep.Setup(repo => repo.GetAll()).Returns(GetTestUsers());

            Mock<IWebHostEnvironment> app_host = new Mock<IWebHostEnvironment>();

            WishController controller = new WishController(Wish_rep.Object, User_rep.Object, WishRate_rep.Object, Comment_rep.Object, app_host.Object);
            User user = new User
            {
                Id = 2,
                Login = "login2",
                Password = "1",
                dayOfBirth = DateTime.Now,
                Country = "None",
                City = "None"
            };

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

            //Arrange
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
            //var viewModel = Controller.Edit(1, wvm).Data as CustomViewData;
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

        private List<User> GetTestUsers()
        {
            var users = new List<User>
                {
                new User { Id = 1,
                Login = "login",
                Password = "1",
                dayOfBirth = DateTime.Now,
                Country = "None",
                City = "None"},
                new User { Id=2,
                Login = "login2",
                Password = "1",
                dayOfBirth = DateTime.Now,
                Country = "None",
                City = "None"},
                new User { Id=3,
                Login = "login3",
                Password = "1",
                dayOfBirth = DateTime.Now,
                Country = "None",
                City = "None"},
                };
            return users;
        }
    }
    public class JsonResultDynamicWrapper : DynamicObject
    {
        private readonly object _resultObject;

        public JsonResultDynamicWrapper([NotNull] JsonResult resultObject)
        {
            if (resultObject == null) throw new ArgumentNullException(nameof(resultObject));
            _resultObject = resultObject.Value;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (string.IsNullOrEmpty(binder.Name))
            {
                result = null;
                return false;
            }

            PropertyInfo property = _resultObject.GetType().GetProperty(binder.Name);

            if (property == null)
            {
                result = null;
                return false;
            }

            result = property.GetValue(_resultObject, null);
            return true;
        }
    }
    
}
