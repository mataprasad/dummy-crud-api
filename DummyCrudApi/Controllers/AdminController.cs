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

        public AdminController(IDbContext dbContext, ILogger<AdminController> logger)
            : base(dbContext, logger) { }

        [HttpPost("reset-data")]
        public ActionResult ResetData()
        {
            var token = System.Environment.GetEnvironmentVariable("AuthorizationToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                var httpClient = new HttpClient();
                var request = new HttpRequestMessage();
                request.Method = HttpMethod.Delete;
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                request.Headers.Add("Accept", "application/vnd.heroku+json; version=3");
                request.RequestUri = new System.Uri("https://api.heroku.com/apps/dummy-crud-api/dynos");
                var resp = httpClient.SendAsync(request).Result;
            }
            return Ok("Done!");
        }
    }
}
