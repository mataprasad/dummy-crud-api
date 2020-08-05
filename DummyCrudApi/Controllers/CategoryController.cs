using System.Collections.Generic;
using System.Linq;
using DummyCrudApi.Models;
using DummyCrudApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DummyCrudApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/categories/")]
    [ApiController]
    public class CategoryController : DummyCrudApiController
    {

        public CategoryController(IDbContext dbContext, ILogger<CategoryController> logger)
            : base(dbContext, logger) { }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> Get([FromQuery] int pageSize = 100, [FromQuery] int pageNo = 1)
        {
            return dbContext.PagedList<Category>("Category", pageSize, pageNo).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Category> Get(long id)
        {
            var obj = dbContext.Single<Category>("Category", id);
            if (obj != null)
            {
                return obj;
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult<Category> Post([FromBody] Category value)
        {
            dbContext.Insert("Category", value);
            return Get(value.Id);
        }

        [HttpPut("{id}")]
        public ActionResult<Category> Put(long id, [FromBody] Category value)
        {
            var obj = dbContext.Single<Category>("Category", id);
            if (obj != null)
            {
                value.Id = id;
                dbContext.Update("Category", value, id);
            }
            return Get(value.Id);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            dbContext.Delete("Category", id);
            return Ok("Deleted!");
        }
    }
}
