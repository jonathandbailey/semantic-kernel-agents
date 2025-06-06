using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Todo.ConsoleApp.Settings;

public static class SettingsExtensions
{
    public static void AddJsonFiles(this IConfigurationBuilder configurationBuilder, IHostEnvironment env)
    {
        configurationBuilder.AddJsonFile(SettingsConstants.AppSettings, optional: false, reloadOnChange: true);
        configurationBuilder.AddJsonFile(string.Format(SettingsConstants.AppSettingsDevelopment, env.EnvironmentName), optional: false, reloadOnChange: true);
    }

    public static string GetRequiredValue(this HostBuilderContext context, string key)
    {
        var settingsValue =  context.Configuration.GetValue<string>(key);

        return string.IsNullOrEmpty(settingsValue)
            ? throw new InvalidOperationException($"Could not get configuration value {key}.")
            : settingsValue;
    }
}