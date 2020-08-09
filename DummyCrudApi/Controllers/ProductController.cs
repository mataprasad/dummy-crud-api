using System.Collections.Generic;
using System.Linq;
using DbContextProvider;
using DummyCrudApi.Models;
using DummyCrudApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DummyCrudApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/products/")]
    [ApiController]
    public class ProductController : DummyCrudApiController
    {

        public ProductController(IDbContext dbContext, ILogger<ProductController> logger)
            : base(dbContext, logger) { }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get([FromQuery] int pageSize = 100, [FromQuery] int pageNo = 1)
        {
            return dbContext.PagedList<Product>("Product", pageSize, pageNo).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Product> Get(long id)
        {
            var obj = dbContext.Single<Product>("Product", id);
            if (obj != null)
            {
                return obj;
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult<Product> Post([FromBody] Product value)
        {
            dbContext.Insert("Product", value);
            return Get(value.Id);
        }

        [HttpPut("{id}")]
        public ActionResult<Product> Put(long id, [FromBody] Product value)
        {
            var obj = dbContext.Single<Product>("Product", id);
            if (obj != null)
            {
                value.Id = id;
                dbContext.Update("Product", value, id);
            }
            return Get(value.Id);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            dbContext.Delete("Product", id);
            return Ok("Deleted!");
        }
    }
}
