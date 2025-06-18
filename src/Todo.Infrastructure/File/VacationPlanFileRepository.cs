using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Todo.Core.Vacations;

namespace Todo.Infrastructure.File
{
    public class VacationPlanFileRepository : IVacationPlanRepository
    {
        private readonly ILogger<VacationPlanFileRepository> _logger;
        private readonly string _directoryPath;

        public VacationPlanFileRepository(IOptions<FileStorageSettings> settings, ILogger<VacationPlanFileRepository> logger)
        {
            _logger = logger;

            _directoryPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\{settings.Value.ApplicationName}\{settings.Value.VacationPlanFolder}";
        }

        public async Task Save(VacationPlan vacationPlan)
        {
            try
            {
                var json = JsonSerializer.Serialize(vacationPlan);

                var filePath = Path.Combine(_directoryPath, $"{vacationPlan.Id}.json");

                Verify.NotNullOrWhiteSpace(json);
                await System.IO.File.WriteAllTextAsync(filePath, json);

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to Save Vacation Plan {id}", vacationPlan.Id);
                throw;
            }
        }

        public async Task<VacationPlan> Load(Guid id)
        {
            try
            {
                var filePath = Path.Combine(_directoryPath, $"{id}.json");

                var json = await System.IO.File.ReadAllTextAsync(filePath);

                Verify.NotNullOrWhiteSpace(json);

                var vacationPlan = JsonSerializer.Deserialize<VacationPlan>(json);

                Verify.NotNull(vacationPlan);

                return vacationPlan;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to Load Vacation Plan : {id}", id);
                throw;
            }
        }
    }

    public interface IVacationPlanRepository
    {
        Task Save(VacationPlan vacationPlan);
        Task<VacationPlan> Load(Guid id);
    }
}
