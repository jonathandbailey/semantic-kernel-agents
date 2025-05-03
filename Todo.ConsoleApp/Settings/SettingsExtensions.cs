using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Todo.ConsoleApp.Settings;

public static class SettingsExtensions
{
    public static void AddJsonFiles(this IConfigurationBuilder configurationBuilder, IHostEnvironment env)
    {
        configurationBuilder.AddJsonFile(Constants.AppSettings, optional: false, reloadOnChange: true);
        configurationBuilder.AddJsonFile(string.Format(Constants.AppSettingsDevelopment, env.EnvironmentName), optional: true, reloadOnChange: true);
    }

    public static T GetRequiredSetting<T>(this HostBuilderContext context)
    {
        var section = context.Configuration.GetRequiredSection(typeof(T).Name);
        
        return section.Get<T>() ?? throw new InvalidOperationException($"Could not get configuration section {typeof(T).Name}.");
    }

    public static T GetRequiredSetting<T>(this HostBuilderContext context, string key)
    {
        var section = context.Configuration.GetRequiredSection(key);

        return section.Get<T>() ?? throw new InvalidOperationException($"Could not get configuration section {typeof(T).Name}.");
    }

    public static IConfigurationSection GetRequiredSection<T>(this HostBuilderContext context)
    {
        return context.Configuration.GetRequiredSection(typeof(T).Name);
    }

    public static string GetRequiredValue(this HostBuilderContext context, string key)
    {
        var settingsValue =  context.Configuration.GetValue<string>(key);

        return string.IsNullOrEmpty(settingsValue)
            ? throw new InvalidOperationException($"Could not get configuration value {key}.")
            : settingsValue;
    }
}