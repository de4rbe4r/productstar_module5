using Microsoft.AspNetCore.Mvc;
using OrchestratorService.Handlers;

namespace WeaponsService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReserveController : ControllerBase
    {
        private readonly ReserveHandler reserveHandler;

        public ReserveController(ReserveHandler reserveHandler)
        {
            this.reserveHandler = reserveHandler;
        }

        [HttpGet("start-add-random-user-to-queue")]
        public async Task<IActionResult> StartAddRandomUserToQueue()
        {
            reserveHandler.StartAddRandomUserToQueue();
            return Ok();
        }
        
        
        [HttpGet("start-listen")]
        public Task StartListen()
        {
            reserveHandler.StartAsync(CancellationToken.None);
            return Task.CompletedTask;
        }
    }
}