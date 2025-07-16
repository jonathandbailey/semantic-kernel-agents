using System.Text.Json.Serialization;

namespace Todo.Core.Vacations
{
    public class TravelPlan : IVacationPlanStage
    {
        public Guid Id { get; }
        
        public PlanStatus Status { get; private set; }

        public string Title { get; } 

        public string Description { get; }

        public PlanStage Stage { get; }

        public List<StageTask> Tasks { get; private set; } = [];

        [JsonConstructor]
        public TravelPlan(Guid id, string title, string description, PlanStage stage, PlanStatus status, List<StageTask> tasks)
        {
            Id = id;
            Status = status;
            Title = title;
            Description = description;
            Stage = stage;
            Tasks = tasks;
        }

        public TravelPlan(Guid id, string title, string description, PlanStage stage, PlanStatus status)
        {
            Id = id;
            Status = status;
            Title = title;
            Description = description;
            Stage = stage;
        }

        public void UpdateStatus(PlanStatus status, List<StageTask> stageTasks)
        {
            Status = status;

            Tasks = stageTasks;
        }

        public void UpdateStatus(PlanStatus status)
        {
            Status = status;
        }
    }
}
