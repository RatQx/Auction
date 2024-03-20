using Aukcionas.Auth.Model;
using Aukcionas.Data;
using Aukcionas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Aukcionas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<ForumRestUser> _userManager;

        public ReportController(DataContext context, UserManager<ForumRestUser> userManager)
        {
            _dataContext = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize(Roles = ForumRoles.ForumUser)]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Auction))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Report>>> CreateReport([FromBody] Report report)
        {
            if (ModelState.IsValid && report !=null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = await _userManager.FindByIdAsync(userId);
                report.auction_Id = report.auction_Id;
                report.report_Message = report.report_Message;
                report.report_Time = DateTime.Now;
                report.userName = userName.ToString();

                _dataContext.Reports.Add(report);
                await _dataContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetReport), new { id = report.id }, report);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("reports")]
        [Authorize(Roles = ForumRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<Report>>> GetReports()
        {
            try
            {
                var reports = await _dataContext.Reports.ToListAsync();
                if (reports == null || reports.Count == 0)
                {
                    return NoContent();
                }
                return Ok(reports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error when trying to get reports.");
            }
        }

        [HttpGet("grouped-reports")]
        [Authorize(Roles = ForumRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Dictionary<int, List<Report>>>> GetGroupedReports()
        {
            try
            {
                var reports = await _dataContext.Reports.ToListAsync();
                if (reports == null || reports.Count == 0)
                {
                    return NoContent();
                }
                var groupedReports = reports.GroupBy(r => r.auction_Id)
                                            .ToDictionary(g => g.Key, g => g.ToList());

                return Ok(groupedReports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error when trying to get grouped reports.");
            }
        }
        [HttpGet("report/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Report>> GetReport(int id)
        {
            try
            {
                var report = await _dataContext.Reports.FindAsync(id);

                if (report == null)
                {
                    return NotFound("Report not found");
                }

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error when trying to get report.");
            }
        }

        [HttpGet("reports/{auctionId}")]
        [Authorize(Roles = ForumRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<Report>>> GetAuctionReports(int auctionId)
        {
            try
            {
                var reports = await _dataContext.Reports
                    .Where(r => r.auction_Id == auctionId.ToString())
                    .ToListAsync();

                if (reports == null || reports.Count == 0)
                {
                    return NotFound("No reports found for the auction.");
                }

                return Ok(reports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error when trying to get auction reports.");
            }
        }

        [HttpDelete("delete-report/{id}")]
        [Authorize(Roles = ForumRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteReport(int id)
        {
            try
            {
                var report = await _dataContext.Reports.FindAsync(id);
                if (report == null)
                {
                    return NotFound("Report with this ID was not found");
                }

                _dataContext.Reports.Remove(report);

                return Ok(await _dataContext.SaveChangesAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error when trying to get auction reports.");
            }
        }

    }

}
