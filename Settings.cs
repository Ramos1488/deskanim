using System;
using System.IO;
using System.Text.Json;

namespace DeskAnim;

public sealed class AppSettings
{
    public bool DarkTheme { get; set; } = true;
}

public static class SettingsStore
{
    public static AppSettings Load()
    {
        try
        {
            if (!File.Exists(AppPaths.SettingsFile)) return new();
            return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(AppPaths.SettingsFile)) ?? new();
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static void Save(AppSettings settings)
    {
        try
        {
            Directory.CreateDirectory(AppPaths.Root);
            File.WriteAllText(AppPaths.SettingsFile, JsonSerializer.Serialize(settings));
        }
        catch (Exception)
        {
            // Non-critical.
        }
    }
}
