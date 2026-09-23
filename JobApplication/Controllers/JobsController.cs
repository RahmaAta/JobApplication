using JobApplication.Application.DTOs;
using JobApplication.Application.Services;
using JobApplication.Application.Features.Jobs.Commands.CreateJob;
using JobApplication.Application.Features.Jobs.Commands.CloseJob;
using MediatR;
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
        private readonly IMediator _mediator;

        public JobsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateJobDto createJobDto)
        {
            var recruiterId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("RecruiterId")?.Value
                              ?? User.FindFirst("sub")?.Value;

            var id = await _mediator.Send(new CreateJobCommand());
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
                await _mediator.Send(new CloseJobCommand { JobId = id, AuthenticatedRecruiterId = recruiterId });
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
