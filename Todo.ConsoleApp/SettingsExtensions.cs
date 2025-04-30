using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Todo.ConsoleApp;

public static class SettingsExtensions
{
    public static void Configure(this IConfigurationBuilder configurationBuilder, IHostEnvironment env)
    {
        configurationBuilder.AddJsonFile(Constants.AppSettings, optional: false, reloadOnChange: true);
        configurationBuilder.AddEnvironmentVariables();
        configurationBuilder.AddUserSecrets<Program>();
    }

    public static string GetConfigurationValue(this HostBuilderContext context, string key)
    {
        var settingsValue =  context.Configuration.GetValue<string>(key);

        if (string.IsNullOrEmpty(settingsValue))
        {
            throw new InvalidOperationException($"Could not get configuration value {key}.");
        }

        return settingsValue;
    }
}