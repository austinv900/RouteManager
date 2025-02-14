using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteManager.Models;

namespace RouteManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoutesController : ControllerBase
    {
        private Managers.RouteManager Routes { get; }

        public RoutesController(Managers.RouteManager context)
        {
            Routes = context;
        }

        [HttpGet]
        public async IAsyncEnumerable<IRoute> GetRoutes([FromQuery]Managers.RouteManager.SearchOptions? options = null)
        {
            await foreach (RoutePlan plan in Routes.Find(options, HttpContext.RequestAborted))
            {
                yield return plan;
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IRoute>> GetRoute(Guid id)
        {
            IAsyncEnumerable<IRoute> routes = Routes.Find(new() { Id = id }, HttpContext.RequestAborted);
            IAsyncEnumerator<IRoute> enumorator = routes.GetAsyncEnumerator();

            if (await enumorator.MoveNextAsync())
            {
                return Ok(enumorator.Current);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<IRoute>> CreateRoute(Managers.RouteManager.CreateOptions routeOptions)
        {
            IRoute route = await Routes.Create(routeOptions, HttpContext.RequestAborted);
            return CreatedAtAction(nameof(GetRoute), new { id = route.Id }, route);
            //await Context.Routes.AddAsync(route, HttpContext.RequestAborted);
            //RouteStop rs = new RouteStop("Common Ground", "507 Carter Street West Eatonville, WA 98328", -48.3233234, 58.11123);
            //rs.Sequence = 1;
            //rs.WindowStart = DateTime.UtcNow.AddMinutes(30);
            //rs.DwellTime = TimeSpan.FromMinutes(30);
            //rs.RouteId = route.Id;
            //await Context.RouteStops.AddAsync(rs, HttpContext.RequestAborted);
            //await Context.SaveChangesAsync(HttpContext.RequestAborted);
            //route.Stops.Add(rs);
            //return CreatedAtAction(nameof(GetRoute), new { id = route.Id }, route);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoute(Guid id, Managers.RouteManager.UpdateOptions route)
        {
            try
            {
                IRoute r = await Routes.Update(id, route, HttpContext.RequestAborted);
                return AcceptedAtAction(nameof(GetRoute), new { id = r.Id }, r);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
                //if (!await RouteExists(id))
                //{
                //    return NotFound($"Route with ID {id} not found for update.");
                //}
                //else
                //{
                //    throw;
                //}
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(Guid id)
        {
            await using IAsyncEnumerator<IRoute> routes = Routes.Find(new() { Id = id }, HttpContext.RequestAborted).GetAsyncEnumerator(HttpContext.RequestAborted);

            if (!await routes.MoveNextAsync())
            {
                return NotFound();
            }

            var route = routes.Current;
            await Routes.Delete(route, HttpContext.RequestAborted);
            return NoContent();
        }
    }
}
