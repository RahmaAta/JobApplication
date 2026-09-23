using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Services
{
    public class ApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationService(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task Cancel(int applicationId, int candidateId)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
            {
                throw new KeyNotFoundException($"Application with ID {applicationId} was not found.");
            }

            if (application.CandidateId != candidateId)
            {
                throw new UnauthorizedAccessException("You do not have permission to cancel this application.");
            }

            if (application.JobApplicationStatus != JobApplicationStatus.Applied &&
                application.JobApplicationStatus != JobApplicationStatus.UnderReview)
            {
                throw new InvalidOperationException($"Application cannot be cancelled because its current status is '{application.JobApplicationStatus}'. Only applications with status 'Applied' or 'UnderReview' can be cancelled.");
            }

            application.JobApplicationStatus = JobApplicationStatus.Cancelled;
            var now = DateTime.UtcNow;
            application.CancelledAt = now;
            application.StatusUpdatedAt = now;

            _applicationRepository.Update(application);
            await _applicationRepository.SaveChangesAsync();
        }
    }
}
