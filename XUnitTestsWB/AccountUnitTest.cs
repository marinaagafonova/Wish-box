using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wish_Box.Controllers;
using Wish_Box.Models;
using Wish_Box.Repositories;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Wish_Box.ViewModels;

namespace XUnitTestsWB
{
    public class AccountUnitTest
    {
        private Mock<IRepository<User>> User_rep { get; set; }

        [Fact]
        public async Task EditAccountAsync()
        {
            //Arrange

            User_rep = new Mock<IRepository<User>>();
            Mock<IWebHostEnvironment> app_host = new Mock<IWebHostEnvironment>();
            Mock<IConfiguration> app_conf = new Mock<IConfiguration>();
            User_rep.Setup(repo => repo.GetAll()).Returns(TestData.GetTestUsers());

            AccountController controller = new AccountController(User_rep.Object, app_host.Object, app_conf.Object);
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

            //userRepository.Update(user)

            Edit2Model fakemodel = new Edit2Model() 
            { 
                Login = "login2",
                City = "EditCity",
                Country = "EditCountry",
                dayOfBirth = DateTime.Now
            }; 

            // Act
            var result = await controller.Edit(fakemodel);

            // Assert
            var viewResult = Assert.IsType<JsonResult>(result);
            dynamic resultData = new JsonResultDynamicWrapper(viewResult);
            Assert.Equal(true, resultData.success);
        }
    }
}
