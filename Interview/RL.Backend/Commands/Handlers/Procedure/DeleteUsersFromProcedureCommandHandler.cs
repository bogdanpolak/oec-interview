using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;
using System.Linq;

namespace RL.Backend.Commands.Handlers.Procedure
{
    public class DeleteUsersFromProcedureCommandHandler : IRequestHandler<DeleteUsersFromProcedureCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;
        public DeleteUsersFromProcedureCommandHandler(RLContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<Unit>> Handle(DeleteUsersFromProcedureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //Validate request
                if (request.PlanId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
                if (request.ProcedureId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));

                var plan = await _context.Plans
                .Include(p => p.PlanProcedureUsers)
                .FirstOrDefaultAsync(p => p.PlanId == request.PlanId);
                if (plan is null)
                    return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));

                var procedures = await _context.Procedures
                    .Include(p => p.PlanProcedureUsers)
                    .FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId);
                if (procedures is null)
                    return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found"));

                procedures.PlanProcedureUsers.Clear();
                await _context.SaveChangesAsync();

                return ApiResponse<Unit>.Succeed(new Unit());
            }
            catch (Exception e)
            {
                return ApiResponse<Unit>.Fail(e);
            }
        }
    }
}
