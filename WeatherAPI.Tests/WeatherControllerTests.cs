using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WeatherAPI.Tests
{
    public class WeatherControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public WeatherControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]

        public async Task GetWeather_Returns200_ForKnownCity()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/weather/Tehran");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            Assert.Contains("temperature", json);
        }

        [Fact]
        public async Task GetWeather_Returns404_ForUnknownCity()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/weather/UnknownCity");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

}
