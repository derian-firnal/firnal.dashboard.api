using Microsoft.AspNetCore.Mvc;

namespace SnowflakeTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SnowflakeService _snowflakeService;

        public AuthController(SnowflakeService snowflakeService)
        {
            _snowflakeService = snowflakeService;
        }

        // 🔹 Query Snowflake using Key Pair Auth
        [HttpGet("GetLineItemData")]
        public async Task<IActionResult> GetDataAsync()
        {
            try
            {
                var results = await _snowflakeService.ExecuteQueryAsync("SELECT TOP 10 * FROM SHEET1.campaign");
                return Ok(new { data = results });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}