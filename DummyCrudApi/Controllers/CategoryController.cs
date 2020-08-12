using System;
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
        public ActionResult<Category> Get(string id)
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
            value.Id = DateTime.UtcNow.Ticks;
            dbContext.Insert("Category", value, value.Id);
            return StatusCode(201, "Created");
        }

        [HttpPut("{id}")]
        public ActionResult<Category> Put(string id, [FromBody] Category value)
        {
            var obj = dbContext.Single<Category>("Category", id);
            if (obj != null)
            {
                dbContext.Update("Category", value, id);
            }
            return Get(value._key);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            dbContext.Delete("Category", id);
            return Ok("Deleted!");
        }
    }
}
