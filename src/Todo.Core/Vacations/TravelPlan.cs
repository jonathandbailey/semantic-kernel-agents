namespace Todo.Core.Vacations
{
    public class TravelPlan : IVacationPlanStage
    {
        public PlanStatus Status { get; } = PlanStatus.Open;

        public string Title { get; } = string.Empty;

        public string Description { get; } = string.Empty;
    }
}
