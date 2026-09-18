using JobApplication.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JobApplication.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly ApplicationService _applicationService;

        public ApplicationsController(ApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var candidateIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                                  ?? User.FindFirst("CandidateId")?.Value
                                  ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(candidateIdClaim) || !int.TryParse(candidateIdClaim, out var candidateId) || candidateId <= 0)
            {
                return Unauthorized(new { message = "User is not authenticated or candidate ID is invalid." });
            }

            try
            {
                await _applicationService.Cancel(id, candidateId);
                return Ok(new { message = "Application cancelled successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
