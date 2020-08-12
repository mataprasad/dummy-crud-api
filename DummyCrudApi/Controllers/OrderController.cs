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
    [Route("api/orders/")]
    [ApiController]
    public class OrderController : DummyCrudApiController
    {

        public OrderController(IDbContext dbContext, ILogger<OrderController> logger)
            : base(dbContext, logger) { }

        [HttpGet]
        public ActionResult<IEnumerable<Order>> Get([FromQuery] int pageSize = 100, [FromQuery] int pageNo = 1)
        {
            return dbContext.PagedList<Order>("Order", pageSize, pageNo).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Order> Get(string id)
        {
            var obj = dbContext.Single<Order>("Order", id);
            if (obj != null)
            {
                return obj;
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult<Order> Post([FromBody] Order value)
        {
            value.Id = DateTime.UtcNow.Ticks;
            dbContext.Insert("Order", value, value.Id);
            return StatusCode(201, "Created");
        }

        [HttpPut("{id}")]
        public ActionResult<Order> Put(string id, [FromBody] Order value)
        {
            var obj = dbContext.Single<Order>("Order", id);
            if (obj != null)
            {
                dbContext.Update("Order", value, id);
            }
            return Get(value._key);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            dbContext.Delete("Order", id);
            return Ok("Deleted!");
        }
    }
}
