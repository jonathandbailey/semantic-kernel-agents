using MediatR;
using Todo.Core.Vacations;

namespace Todo.Application.Dto
{
    public class GetVacationPlanCatalogQuery : IRequest<List<VacationPlanCatalogItem>>
    {
        public Guid UserId { get; set; }

        public GetVacationPlanCatalogQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
