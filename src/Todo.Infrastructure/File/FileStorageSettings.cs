namespace Todo.Infrastructure.File
{
    public class FileStorageSettings
    {
        public string ApplicationName { get; set; } = string.Empty;

        public string TemplateFolder { get; set; } = string.Empty;

        public string ChatHistoryFolder { get; set; } = string.Empty;

        public string AgentTaskFolder { get; set; } = string.Empty;

        public string VacationPlanFolder { get; set; } = string.Empty;
    }
}
