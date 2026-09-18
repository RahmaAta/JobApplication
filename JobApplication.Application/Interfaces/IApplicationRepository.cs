using JobApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    public interface IApplicationRepository
    {
        Task<JobCandidateApplication?> GetByIdAsync(int id);
        Task InsertAsync(JobCandidateApplication application);
        void Update(JobCandidateApplication application);
        IQueryable<JobCandidateApplication> Get();
        void Remove(JobCandidateApplication application);
        Task SaveChangesAsync();
    }
}
