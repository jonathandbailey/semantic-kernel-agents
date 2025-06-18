namespace Todo.Core.Vacations
{
    public class TravelPlan : IVacationPlanStage
    {
        public Guid Id { get; }
        
        public PlanStatus Status { get; private set; }

        public string Title { get; } 

        public string Description { get; }

        public PlanStage Stage { get; }
        
        public void UpdateStatus(PlanStatus status)
        {
            Status = status;
        }

        public TravelPlan(Guid id, string title, string description, PlanStage stage, PlanStatus status)
        {
            Id = id;
            Status = status;
            Title = title;
            Description = description;
            Stage = stage;
        }
    }
}
