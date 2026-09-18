using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Services
{
    public class JobService
    {
        private readonly IJobRepository _jobRepository;

        public JobService(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<int> CreateAsync(CreateJobDto createJobDto, string? recruiterId = null)
        {   
            var job = new Job()
            {
                Title = createJobDto.Title,
                Description = createJobDto.Description,
                IsActive = true,
                RecruiterId = recruiterId
            };
            await _jobRepository.InsertAsync(job);
            await _jobRepository.SaveChangesAsync();

            return job.Id; 
        }

        public async Task Close(int jobId, string authenticatedRecruiterId)
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null)
            {
                throw new KeyNotFoundException($"Job with ID {jobId} was not found.");
            }

            if (job.RecruiterId != authenticatedRecruiterId)
            {
                throw new UnauthorizedAccessException("You are not authorized to close this job.");
            }

            if (job.ClosedAt.HasValue || !job.IsActive)
            {
                throw new InvalidOperationException("Job is already closed.");
            }

            job.ClosedAt = DateTime.UtcNow;
            job.ClosedBy = authenticatedRecruiterId;
            job.IsActive = false;

            _jobRepository.Update(job);
            await _jobRepository.SaveChangesAsync();
        }

        public Task CloseAsync(int jobId, string authenticatedRecruiterId) => Close(jobId, authenticatedRecruiterId);
    }
}
