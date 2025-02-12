using firnal.dashboard.services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace firnal.dashboard.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        // GET: api/<ValuesController>
        [HttpGet("GetTodaysUsers")]
        public async Task<IActionResult> Get()
        {
            var count = await _campaignService.GetTodaysUsersCountAsync();
            return Ok(new { count });
        }

        [HttpGet("GetCampaignUserDetails")]
        public async Task<IActionResult> GetCampaignUserDetails(string schemaName)
        {
            var campaignUserDetails = await _campaignService.GetCampaignUserDetailsAsync(schemaName);
            return Ok(campaignUserDetails);
        }

        [HttpGet("GetZips")]
        public async Task<IActionResult> GetZips(string schemaName)
        {
            var zips = await _campaignService.GetDistinctZips(schemaName);
            return Ok(zips);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var all = await _campaignService.GetAll();

            // Return CSV file as a response
            Response.Headers["Content-Disposition"] = "attachment; filename=campaign_data.csv";
            return File(all, "text/csv", "campaign_data.csv");
        }
    }
}
