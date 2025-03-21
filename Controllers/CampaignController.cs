using firnal.dashboard.services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace firnal.dashboard.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        // GET: api/<ValuesController>
        [HttpGet("GetTotalUsersAsync")]
        public async Task<IActionResult> Get(string schemaName)
        {
            var count = await _campaignService.GetTotalUsersAsync(schemaName);
            return Ok(new { count });
        }

        [HttpGet("GetNewUsers")]
        public async Task<IActionResult> GetNewUsers(string schemaName)
        {
            var newUsers = await _campaignService.GetNewUsersAsync(schemaName);
            return Ok(newUsers);
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
        public async Task<IActionResult> GetAll(string schemaName)
        {
            var all = await _campaignService.GetAll(schemaName);

            // Return CSV file as a response
            Response.Headers["Content-Disposition"] = "attachment; filename=campaign_data.csv";
            return File(all, "text/csv", "campaign_data.csv");
        }

        [HttpGet("GetNewUsersOverPast7Days")]
        public async Task<IActionResult> GetNewUsersOverPast7Days(string schemaName)
        {
            var users = await _campaignService.GetNewUsersOverPast7Days(schemaName);
            return Ok(users);
        }

        [HttpGet("GetGenderDistribution")]
        public async Task<IActionResult> GetGenderDistribution(string schemaName)
        {
            var genderDistribution = await _campaignService.GetGenderVariance(schemaName);
            return Ok(genderDistribution);
        }

        [HttpGet("GetAverageIncome")]
        public async Task<IActionResult> GetAverageIncome(string schemaName)
        {
            var averageIncome = await _campaignService.GetAverageIncome(schemaName);
            return Ok(averageIncome);
        }

        [HttpGet("GetAgeRange")]
        public async Task<IActionResult> GetAgeRange(string schemaName)
        {
            var ageRange = await _campaignService.GetAgeRange(schemaName);
            return Ok(ageRange);
        }

        [HttpGet("GetTopicBreakdown")]
        public async Task<IActionResult> GetTopicBreakdown(string schemaName)
        {
            var topicBreakdown = await _campaignService.GetTopicBreakdown(schemaName);
            return Ok(topicBreakdown);
        }
    }
}
