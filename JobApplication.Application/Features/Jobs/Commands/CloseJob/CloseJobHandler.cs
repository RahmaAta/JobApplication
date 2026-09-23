using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Features.Jobs.Commands.CloseJob
{
    public class CloseJobHandler : IRequestHandler<CloseJobCommand, bool>
    {
        private readonly IRepository<Job> _jobRepository;

        public CloseJobHandler(IRepository<Job> jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<bool> Handle(CloseJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _jobRepository.Get().FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);

            if (job == null)
            {
                throw new KeyNotFoundException(
                    $"Job with ID {request.JobId} was not found.");
            }

            if (job.RecruiterId != request.AuthenticatedRecruiterId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to close this job.");
            }

            if (job.ClosedAt.HasValue || !job.IsActive)
            {
                throw new InvalidOperationException(
                    "Job is already closed.");
            }

            job.ClosedAt = DateTime.UtcNow;
            job.ClosedBy = request.AuthenticatedRecruiterId;
            job.IsActive = false;

            await _jobRepository.SaveChangesAsync();

            return true;
        }
    }
}
