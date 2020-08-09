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
                httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", token));
                httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.heroku+json; version=3");
                httpClient.DeleteAsync("https://api.heroku.com/apps/dummy-crud-api/dynos");
            }
            return Ok("Done!");
        }
    }
}
