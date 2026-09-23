using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using JobApplication.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobApplication.Application.Features.JobCandidateApplications.Commands.CancelJobCandidateApplication
{
    public class CancelJobCandidateApplicationHandler : IRequestHandler<CancelJobCandidateApplicationCommand, JobCandidateApplication?>
    {
        private readonly IRepository<JobCandidateApplication> _jobApplicationRepository;

        public CancelJobCandidateApplicationHandler(
            IRepository<JobCandidateApplication> jobApplicationRepository)
        {
            _jobApplicationRepository = jobApplicationRepository;
        }

        public async Task<JobCandidateApplication?> Handle(
            CancelJobCandidateApplicationCommand request,
            CancellationToken cancellationToken)
        {
            var application = await _jobApplicationRepository
                .Get()
                .FirstOrDefaultAsync(
                    a => a.Id == request.Id,
                    cancellationToken);

            if (application == null)
            {
                throw new KeyNotFoundException(
                    $"Application with ID {request.Id} was not found.");
            }

            if (application.CandidateId != request.CandidateId)
            {
                throw new UnauthorizedAccessException(
                    "You do not have permission to cancel this application.");
            }

            if (application.JobApplicationStatus != JobApplicationStatus.Applied &&
                application.JobApplicationStatus != JobApplicationStatus.UnderReview)
            {
                throw new InvalidOperationException(
                    $"Application cannot be cancelled because its current status is " +
                    $"'{application.JobApplicationStatus}'. " +
                    "Only applications with status 'Applied' or 'UnderReview' can be cancelled.");
            }

            application.JobApplicationStatus = JobApplicationStatus.Cancelled;

            var now = DateTime.UtcNow;

            application.CancelledAt = now;
            application.StatusUpdatedAt = now;

            await _jobApplicationRepository.SaveChangesAsync();

            return application;
        }
    }
}
