using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace firnal.dashboard.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SnowflakeService _snowflakeService;

        public UserController(SnowflakeService snowflakeService)
        {
            _snowflakeService = snowflakeService;
        }

        // GET: api/<ValuesController>
        [HttpGet("GetTodaysUsers")]
        public async Task<IActionResult> Get()
        {
            var results = await _snowflakeService.ExecuteQueryAsync("SELECT count(distinct first_name, last_name) FROM SHEET1.campaign");
            return Ok(new { data = results });
        }
    }
}
