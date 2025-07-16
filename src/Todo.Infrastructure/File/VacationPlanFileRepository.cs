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

        public async Task AddVacationPlanToCatalog(VacationPlan vacationPlan)
        {
            try
            {
                var catalog = await GetCatalog();

                catalog.Add(new VacationPlanCatalogItem() { Title = vacationPlan.Title, Id = vacationPlan.Id});

                var json = JsonSerializer.Serialize(catalog);

                var filePath = Path.Combine(_directoryPath, $"VacationPlanCatalog.json");

                Verify.NotNullOrWhiteSpace(json);
                await System.IO.File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to Update Catalog");
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

        public async Task<List<VacationPlanCatalogItem>> GetCatalog()
        {
            try
            {
                var filePath = Path.Combine(_directoryPath, $"VacationPlanCatalog.json");

                var json = await System.IO.File.ReadAllTextAsync(filePath);

                Verify.NotNullOrWhiteSpace(json);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var vacationPlan = JsonSerializer.Deserialize<List<VacationPlanCatalogItem>>(json, options);

                Verify.NotNull(vacationPlan);

                return vacationPlan;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to Load Vacation Plan Catalog");
                throw;
            }
        }
    }

    public interface IVacationPlanRepository
    {
        Task Save(VacationPlan vacationPlan);
        Task<VacationPlan> Load(Guid id);
        Task<List<VacationPlanCatalogItem>> GetCatalog();
        Task AddVacationPlanToCatalog(VacationPlan vacationPlan);
    }
}
