using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.Procedure
{
    public class AddUserToProcedureCommandHandler : IRequestHandler<AddUserToProcedureCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;
        public AddUserToProcedureCommandHandler(RLContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<Unit>> Handle(AddUserToProcedureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //Validate request
                if (request.PlanId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
                if (request.ProcedureId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));

                var plan = await _context.Plans
                .Include(p => p.ProcedureUsers)
                .FirstOrDefaultAsync(p => p.PlanId == request.PlanId);
                if (plan is null)
                    return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));

                var procedures = await _context.Procedures
                    .Include(p => p.ProcedureUsers)
                    .FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId);
                if (procedures is null)
                    return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found"));

                if (request.UserIds is null || request.UserIds.Count() < 1)
                {
                    procedures.ProcedureUsers.Clear();
                }
                else
                {
                    procedures.ProcedureUsers.Clear();
                    var validIds = await _context.Users.Select(u => u.UserId).ToListAsync();
                    bool isValidIds = request.UserIds.All(x => validIds.Contains(x));
                    if (!isValidIds)
                    {
                        return ApiResponse<Unit>.Fail(new NotFoundException($"Invalid UserIds"));
                    }

                    foreach (var userId in request.UserIds)
                    {
                        //    var procedureUser = await _context.ProcedureUsers
                        //.FirstOrDefaultAsync(pu => pu.PlanId == request.PlanId && pu.ProcedureId == request.ProcedureId && pu.UserId == userId);
                        //    if (procedureUser is not null)
                        procedures.ProcedureUsers.Add(new ProcedureUser
                        {
                            PlanId = request.PlanId,
                            Plan = plan,
                            ProcedureId = request.ProcedureId,
                            Procedure = procedures,
                            UserId = userId
                        });
                    }
                }
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
