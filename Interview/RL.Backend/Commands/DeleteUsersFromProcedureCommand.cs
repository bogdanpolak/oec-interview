using MediatR;
using RL.Backend.Models;

namespace RL.Backend.Commands
{
    public class DeleteUsersFromProcedureCommand : IRequest<ApiResponse<Unit>>
    {
        public int PlanId { get; set; }
        public int ProcedureId { get; set; }
    }
}