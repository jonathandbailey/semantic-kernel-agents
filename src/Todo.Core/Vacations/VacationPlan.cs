using System.Text.Json.Serialization;

namespace Todo.Core.Vacations
{
    public class VacationPlan
    {
        public Guid Id { get; }
        
        public PlanStatus Status { get; }

        public string Title { get; }

        public string Description { get; }

        public List<TravelPlan> Stages { get; } 
        

        [JsonConstructor]
        public VacationPlan(Guid id, PlanStatus status, string title, string description, List<TravelPlan> stages)
        {
            Status = status;
            Title = title;
            Description = description;
            Id = id;
            Stages = stages;
        }

        public VacationPlan(Guid id, PlanStatus status, string title, string description)
        {
            Status = status;
            Title = title;
            Description = description;
            Id = id;
            Stages = new List<TravelPlan>();
        }

        public void AddStage(TravelPlan stage)
        {
            Stages.Add(stage);
        }

        public void UpdateStageStatus(PlanStage planStage, PlanStatus status)
        {
            var stage = Stages.FirstOrDefault(x => x.Stage == planStage);

            if (stage == null)
            {
                throw new ArgumentException($"Vacation Plan : {Title}, ({Id}) does not have a {planStage} stage.");
            }

            stage.UpdateStatus(status);
        }

        public void UpdateStageStatus(Guid stageId, PlanStatus status)
        {
            var stage = Stages.FirstOrDefault(x => x.Id == stageId);

            if (stage == null)
            {
                throw new ArgumentException($"Vacation Plan : {Title}, ({Id}) does not have a stage with id : {stageId}.");
            }

            stage.UpdateStatus(status);
        }

        public void UpdateStageStatus(Guid stageId, PlanStatus status, List<StageTask> stageTasks)
        {
            var stage = Stages.FirstOrDefault(x => x.Id == stageId);

            if (stage == null)
            {
                throw new ArgumentException($"Vacation Plan : {Title}, ({Id}) does not have a stage with id : {stageId}.");
            }

            stage.UpdateStatus(status, stageTasks);
        }
    }
}
