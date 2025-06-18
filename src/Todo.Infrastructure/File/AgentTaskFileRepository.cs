using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Todo.Core.A2A;
using Todo.Infrastructure.Azure;

namespace Todo.Infrastructure.File
{
    public class AgentTaskFileRepository : IAgentTaskRepository
    {
        private readonly ILogger<AgentTaskFileRepository> _logger;
        private readonly string _directoryPath;


        public AgentTaskFileRepository(IOptions<FileStorageSettings> settings, ILogger<AgentTaskFileRepository> logger)
        {
            _logger = logger;
            _directoryPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\{settings.Value.ApplicationName}\{settings.Value.AgentTaskFolder}";
        }

        public async Task<AgentTask> LoadAsync(string taskId)
        {
            Verify.NotNullOrWhiteSpace(taskId);

            var filePath = Path.Combine(_directoryPath, $"{taskId}.json");

            try
            {
                var json = await System.IO.File.ReadAllTextAsync(filePath);

                Verify.NotNullOrWhiteSpace(json);

                var agentTask = JsonSerializer.Deserialize<AgentTask>(json);

                Verify.NotNull(agentTask);

                return agentTask;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Filed to load task {taskId} from path : {filePath}", taskId, filePath);
                throw;
            }
        }

        public async Task SaveAsync(AgentTask agentTask)
        {
            var filePath = Path.Combine(_directoryPath, $"{agentTask.TaskId}.json");

            try
            {
                var json = JsonSerializer.Serialize(agentTask);

                Verify.NotNullOrWhiteSpace(json);
                await System.IO.File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to Save task {taskId} to path {filePath}.", agentTask.TaskId, filePath);
            }
        }

        public Task<bool> Exists(string taskId)
        {
            Verify.NotNullOrWhiteSpace(taskId);

            var filePath = Path.Combine(_directoryPath, $"{taskId}.json");

            try
            {
                return Task.FromResult(System.IO.File.Exists(filePath));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to verify if {taskId}.json exists at path : {filePath}", taskId, filePath);
                throw;
            }
        }
    }
}
