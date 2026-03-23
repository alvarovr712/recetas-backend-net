using Microsoft.AspNetCore.Mvc;
using RecetasAPINet.DTOs;
using RecetasAPINet.Services;

namespace RecetasAPINet.Controllers
{
    [ApiController]
    [Route("/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardDTO>> GetDashboard([FromQuery] int? month = null, [FromQuery] int? year = null)
        {
            var result = await _dashboardService.GetDashboardAsync(month, year);
            return Ok(result);
        }
    }
}