using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;
using System;

namespace XUnitTestsWB.Integration_tests
{
  
    public class HttpTests : IClassFixture<ExampleAppTestFixture>, IDisposable
    {
        readonly ExampleAppTestFixture _fixture;
        readonly HttpClient _client;

        public HttpTests(ExampleAppTestFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            fixture.Output = output;
            _client = fixture.CreateClient();
        }

        public void Dispose() => _fixture.Output = null;

        [Fact]
        public async Task CanCallApi()
        {
            var result = await _client.GetAsync("/");

            result.EnsureSuccessStatusCode();

            Assert.Equal("text/html; charset=utf-8",
                       result.Content.Headers.ContentType.ToString());
        }
    }
}
