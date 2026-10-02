using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DeskAnim;

public sealed class LayerState
{
    public string FilePath { get; set; } = "";
    public double Left { get; set; }
    public double Top { get; set; }
    public double Scale { get; set; } = 1.0;
}

public static class LayoutStore
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static List<LayerState> Load()
    {
        try
        {
            if (!File.Exists(AppPaths.LayoutFile)) return new();
            var json = File.ReadAllText(AppPaths.LayoutFile);
            return JsonSerializer.Deserialize<List<LayerState>>(json) ?? new();
        }
        catch (Exception)
        {
            return new();
        }
    }

    public static void Save(IEnumerable<LayerState> layers)
    {
        try
        {
            Directory.CreateDirectory(AppPaths.Root);
            File.WriteAllText(AppPaths.LayoutFile, JsonSerializer.Serialize(layers, Options));
        }
        catch (Exception)
        {
            // Non-critical: layout will simply not persist.
        }
    }
}
