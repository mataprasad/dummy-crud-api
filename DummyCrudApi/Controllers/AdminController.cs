using System.Net.Http;
using DbContextProvider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DummyCrudApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/admin/")]
    [ApiController]
    public class AdminController : DummyCrudApiController
    {
        private HttpClient httpClient;

        public AdminController(IHttpClientFactory httpClient,IDbContext dbContext, ILogger<AdminController> logger)
            : base(dbContext, logger) {
            this.httpClient = httpClient.CreateClient();
        }

        [HttpPost("reset-data")]
        public ActionResult ResetData()
        {
            var token = System.Environment.GetEnvironmentVariable("AUTHORIZATION_TOKEN");
            var appName = System.Environment.GetEnvironmentVariable("APP_NAME");
            //dummy-crud-api
            if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(appName))
            {
                var request = new HttpRequestMessage();
                request.Method = HttpMethod.Delete;
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                request.Headers.Add("Accept", "application/vnd.heroku+json; version=3");
                request.RequestUri = new System.Uri(string.Format("https://api.heroku.com/apps/{0}/dynos",appName));
                
                var resp = httpClient.SendAsync(request).Result;
            }
            return Ok("Done!");
        }
    }
}
