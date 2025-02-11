using firnal.dashboard.services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetCampaignUserDetails()
        {
            var campaignUserDetails = await _campaignService.GetCampaignUserDetailsAsync();
            return Ok(campaignUserDetails);
        }

        [HttpGet("GetZips")]
        public async Task<IActionResult> GetZips()
        {
            var zips = await _campaignService.GetDistinctZips();
            return Ok(zips);
        }
    }
}
