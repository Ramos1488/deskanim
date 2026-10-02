using System;
using System.IO;

namespace DeskAnim;

public static class AppPaths
{
    public static readonly string Root = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DeskAnim");

    public static readonly string Library = Path.Combine(Root, "library");
    public static readonly string SettingsFile = Path.Combine(Root, "settings.json");
    public static readonly string LayoutFile = Path.Combine(Root, "layers.json");
}
