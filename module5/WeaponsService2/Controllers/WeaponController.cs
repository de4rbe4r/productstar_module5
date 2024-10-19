using Microsoft.AspNetCore.Mvc;
using WeaponsService2.Handlers;

namespace WeaponsService2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeaponController : ControllerBase
    {
        private readonly WeaponHandler weaponHandler;

        public WeaponController(WeaponHandler weaponHandler)
        {
            this.weaponHandler = weaponHandler;
        }

        [HttpGet("start-add-random-weapon")]
        public async Task<IActionResult> StartAddRandomWeapon()
        {
            weaponHandler.StartAddRandomWeapon();
            return Ok();
        }

        [HttpGet("stop-add-random-weapon")]
        public async Task<IActionResult> StopAddRandomWeapon()
        {
            weaponHandler.StopAddRandomWeapon();
            return Ok();
        }

        [HttpPost("reserve-weapon")]
        public async Task<IActionResult> ReserveWeapon([FromBody] long weaponId)
        {
            var result = weaponHandler.ReserveWeapon(weaponId);
            return Ok(result);
        }
    }
}