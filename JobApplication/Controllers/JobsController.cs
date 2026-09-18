using JobApplication.Application.DTOs;
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
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly JobService _JobService;

        public JobsController(JobService jobService)
        {
            _JobService = jobService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateJobDto createJobDto)
        {
            var recruiterId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("RecruiterId")?.Value
                              ?? User.FindFirst("sub")?.Value;

            var id = await _JobService.CreateAsync(createJobDto, recruiterId);
            return Ok(new
            {
                id = id
            });
        }

        [Authorize]
        [HttpPut("{id}/close")]
        public async Task<IActionResult> Close(int id)
        {
            var recruiterId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("RecruiterId")?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(recruiterId))
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            try
            {
                await _JobService.Close(id, recruiterId);
                return Ok(new { message = "Job closed successfully." });
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
