using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Todo.ConsoleApp;

public static class SettingsExtensions
{
    public static void AddJsonFiles(this IConfigurationBuilder configurationBuilder, IHostEnvironment env)
    {
        configurationBuilder.AddJsonFile(Constants.AppSettings, optional: false, reloadOnChange: true);
    }
}