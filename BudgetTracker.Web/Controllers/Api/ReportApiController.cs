using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Models;
using BudgetTracker.Core.Services.Implementations;
using BudgetTracker.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace BudgetTracker.Controllers.Api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportApiController : ControllerBase
    {
        private readonly IReportAppService _reportAppService;

        public ReportApiController(IReportAppService reportAppService)
        {
            _reportAppService = reportAppService;
        }

        [EnableRateLimiting("ApiReportPolicy")]
        [HttpGet("GetReport")]
        public async Task<MonthlySummaryViewModel> GetReport()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var year = HttpContext.Request.Query.TryGetValue("year", out var yearValue) ? int.Parse(yearValue) : (int?)null;
            var rp = await _reportAppService.GetMonthlySummaryAsync(userId, DateTime.Now.Year);
            return rp;
        }
    }
}
