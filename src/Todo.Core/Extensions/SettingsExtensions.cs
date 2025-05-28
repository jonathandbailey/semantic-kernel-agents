﻿using Microsoft.Extensions.Configuration;

namespace Todo.Core.Extensions;

public static class SettingsExtensions
{
    public static T GetRequiredSetting<T>(this IConfiguration configuration, string key)
    {
        var section = configuration.GetRequiredSection(key);

        if (!section.Exists())
        {
            throw new InvalidOperationException($"Configuration section '{key}' is missing.");
        }

        return section.Get<T>() ?? throw new InvalidOperationException($"Could not get configuration section {typeof(T).Name}.");
    }

    public static T GetRequiredSetting<T>(this IConfiguration configuration)
    {
        return GetRequiredSetting<T>(configuration, typeof(T).Name);
    }

    public static IConfigurationSection GetRequiredSection<T>(this IConfiguration configuration)
    {
        return configuration.GetRequiredSection(typeof(T).Name);
    }
}