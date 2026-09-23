using JobApplication.Application.DTOs;
using JobApplication.Application.Features.JobCandidateApplications.Commands.CreateJobCandidateApplication;
using JobApplication.Application.Features.JobCandidateApplications.Commands.UpdateJobCandidateApplicationStatus;
using JobApplication.Application.Features.JobCandidateApplications.Queries.GetAllJobCandidateApplications;
using JobApplication.Application.Features.JobCandidateApplications.Queries.GetJobCandidateApplicationById;
using JobApplication.Application.Features.JobCandidateApplications.Commands.CancelJobCandidateApplication;
using JobApplication.Application.Services;
using JobApplication.Domain.Enums;
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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApplicationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var applications = await _mediator.Send(new GetAllJobCandidateApplicationsQuery());
            return Ok(new { applications });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var application = await _mediator.Send(new GetJobCandidateApplicationByIdQuery() { Id = id });
            if (application is null) return NotFound(new
            {
                message = "invalid Id"
            });
            return Ok(new { application });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateJobCandidateApplicationDto createApplicationDto)
        {
            var id = await _mediator.Send(new CreateJobCandidateApplicationCommand() { JobId = createApplicationDto.JobId, CandidateId = createApplicationDto.CandidateId });
            return Ok(new { id = id });
        }

        [HttpPatch("{id}/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, JobApplicationStatus status)
        {
            var job = await _mediator.Send(new UpdateJobCandidateApplicationStatusCommand() { Id = id, Status = status });
            if (job == null) return NotFound();
            return Ok(new { id = job.Id });
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
                await _mediator.Send(new CancelJobCandidateApplicationCommand() { Id = id, CandidateId = candidateId });
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
