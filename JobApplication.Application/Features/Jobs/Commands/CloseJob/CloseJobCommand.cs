using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Features.Jobs.Commands.CloseJob
{
    public class CloseJobCommand : IRequest<bool>
    {
        public int JobId { get; set; }
        public string AuthenticatedRecruiterId { get; set; }
    }
}
