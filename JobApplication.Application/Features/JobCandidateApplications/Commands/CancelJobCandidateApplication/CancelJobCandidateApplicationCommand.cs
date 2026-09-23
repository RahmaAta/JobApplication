using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JobApplication.Application.Features.JobCandidateApplications.Commands.CancelJobCandidateApplication
{
    public class CancelJobCandidateApplicationCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; }
        public int JobId { get; set; }
        public JobApplicationStatus Status { get; set; }
    }
}
