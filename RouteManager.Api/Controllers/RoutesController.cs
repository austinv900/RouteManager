using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteManager.Managers.Options.Route;
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
        public async IAsyncEnumerable<IRoute> GetRoutes([FromQuery]RouteQueryOptions? options = null)
        {
            await foreach (RoutePlan plan in Routes.LookupModels(options, HttpContext.RequestAborted))
            {
                yield return plan;
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IRoute>> GetRoute(Guid id)
        {
            IAsyncEnumerable<IRoute> routes = Routes.LookupModels(new() { Id = id }, HttpContext.RequestAborted);
            IAsyncEnumerator<IRoute> enumorator = routes.GetAsyncEnumerator();

            if (await enumorator.MoveNextAsync())
            {
                return Ok(enumorator.Current);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<IRoute>> CreateRoute(RouteCreateOptions routeOptions)
        {
            IRoute route = await Routes.CreateModel(routeOptions, HttpContext.RequestAborted);
            return CreatedAtAction(nameof(GetRoute), new { id = route.Id }, route);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoute(Guid id, RouteUpdateOptions route)
        {
            try
            {
                IRoute r = await Routes.UpdateModel(id, route, HttpContext.RequestAborted);
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
            return Ok(await Routes.DeleteModel([id], HttpContext.RequestAborted));
        }
    }
}
