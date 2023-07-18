using System;
using System.Collections.Generic;
using System.Linq;
using DbContextProvider;
using DummyCrudApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DummyCrudApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/todos/")]
    [ApiController]
    public class TodoController : DummyCrudApiController
    {

        public TodoController(IDbContext dbContext, ILogger<TodoController> logger)
            : base(dbContext, logger) { }

        [HttpGet]
        public ActionResult<IEnumerable<Todo>> Get([FromQuery] int pageSize = 100, [FromQuery] int pageNo = 1)
        {
            return dbContext.PagedList<Todo>("Todo", pageSize, pageNo).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Todo> Get(string id)
        {
            var obj = dbContext.Single<Todo>("Todo", id);
            if (obj != null)
            {
                return obj;
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult<Todo> Post([FromBody] Todo value)
        {
            value.Id = DateTime.UtcNow.Ticks;
            dbContext.Insert("Todo", value, value.Id);
            return StatusCode(201, "Created");
        }

        [HttpPut("{id}")]
        public ActionResult<Todo> Put(string id, [FromBody] Todo value)
        {
            var obj = dbContext.Single<Todo>("Todo", id);
            if (obj != null)
            {
                dbContext.Update("Todo", value, id);
            }
            return Get(value._key);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            dbContext.Delete("Todo", id);
            return Ok("Deleted!");
        }
    }
}