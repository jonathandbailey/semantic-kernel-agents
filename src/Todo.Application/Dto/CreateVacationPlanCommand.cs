using MediatR;
using Todo.Core.Vacations;

namespace Todo.Application.Dto
{
    public class CreateVacationPlanCommand(Guid id) : IRequest<VacationPlan>
    {
        public Guid Id { get; } = id;
    }
}
