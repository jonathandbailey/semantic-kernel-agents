namespace Todo.Core.Vacations
{
    public class VacationPlan
    {
        public PlanStatus Status { get; } = PlanStatus.Open;

        public string Title { get; } = string.Empty;

        public string Description { get; } = string.Empty;

        private readonly List<IVacationPlanStage> _stages  = [];

        public IReadOnlyCollection<IVacationPlanStage> Stages => _stages.AsReadOnly();

        public VacationPlan()
        {
            _stages.Add(new TravelPlan());
        }
    }
}
