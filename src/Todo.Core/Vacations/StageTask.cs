namespace Todo.Core.Vacations
{
    public class StageTask
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;
    }
}
