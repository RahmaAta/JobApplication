using JobApplication.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobApplication.API.Controllers
{
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
        public async Task<IActionResult> Cancel(int id, [FromQuery] int? candidateId,
            [FromHeader(Name = "X-Candidate-Id")] int? headerCandidateId,
            [FromHeader(Name = "CandidateId")] int? altHeaderCandidateId)
        {
            var resolvedCandidateId = candidateId ?? headerCandidateId ?? altHeaderCandidateId;
            if (!resolvedCandidateId.HasValue || resolvedCandidateId.Value <= 0)
            {
                return BadRequest(new { message = "CandidateId is required to verify ownership. Provide it via query parameter 'candidateId' or 'X-Candidate-Id' header." });
            }

            try
            {
                await _applicationService.Cancel(id, resolvedCandidateId.Value);
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
