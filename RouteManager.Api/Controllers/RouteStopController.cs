using Microsoft.AspNetCore.Mvc;
using RouteManager.Models;

namespace RouteManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RouteStopController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> AddStop()
        {
            return null;
        }

        [HttpDelete("{routeId}/{stopId}")]
        public async Task<IActionResult> RemoveStop(Guid routeId, Guid stopId)
        {

        }
    }
}
