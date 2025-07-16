using MediatR;
using Todo.Application.Dto;
using Todo.Core.Vacations;
using Todo.Infrastructure.File;

namespace Todo.Application.Handlers
{
    public class VacationPlanHandler(IVacationPlanRepository vacationPlanRepository, IVacationPlanService vacationPlanService) : 
        IRequestHandler<GetVacationPlanCatalogQuery, List<VacationPlanCatalogItem>>,
        IRequestHandler<GetVacationPlanQuery, VacationPlan>,
        IRequestHandler<CreateVacationPlanCommand, VacationPlan>
    {
        public async Task<List<VacationPlanCatalogItem>> Handle(GetVacationPlanCatalogQuery request, CancellationToken cancellationToken)
        {
            var catalog = await vacationPlanRepository.GetCatalog();

            return catalog;
        }

        public async Task<VacationPlan> Handle(GetVacationPlanQuery request, CancellationToken cancellationToken)
        {
            var vacationPlan = await vacationPlanRepository.Load(request.VacationPlanId);

            return vacationPlan;
        }

        public async Task<VacationPlan> Handle(CreateVacationPlanCommand request, CancellationToken cancellationToken)
        {
            var vacationPlan = await vacationPlanService.Create();

            return vacationPlan;
        }
    }
}
