using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Wish_Box;
using Wish_Box.Models;
using Microsoft.Extensions.DependencyInjection;


namespace XUnitTestsWB
{
    public class BaseTestServerFixture
    {
        public TestServer TestServer { get; }
        public AppDbContext DbContext { get; }
        public HttpClient Client { get; }

        public BaseTestServerFixture()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("Testing")
                .UseStartup<Startup>();

            TestServer = new TestServer(builder);
            Client = TestServer.CreateClient();

            DbContext = TestServer.Host.Services.GetService<AppDbContext>();
        }

        public void Dispose()
        {
            Client.Dispose();
            TestServer.Dispose();
        }
    }
}
