using MediatR;
using Todo.Core.Vacations;

namespace Todo.Application.Dto
{
    public class GetVacationPlanQuery : IRequest<VacationPlan>
    {
        public Guid UserId { get; set; }

        public Guid VacationPlanId { get; set; }

        public GetVacationPlanQuery(Guid userId, Guid vacationPlanId)
        {
            UserId = userId;
            VacationPlanId = vacationPlanId;
        }
    }
}
