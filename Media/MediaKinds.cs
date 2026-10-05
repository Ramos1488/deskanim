using System;
using System.IO;
using System.Linq;

namespace DeskAnim.Media;

public static class MediaKinds
{
    public static readonly string[] ImageExtensions = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };
    public static readonly string[] VideoExtensions = { ".mp4", ".wmv", ".avi", ".mov", ".webm", ".mkv" };

    public static bool IsVideo(string path) =>
        VideoExtensions.Contains(Path.GetExtension(path).ToLowerInvariant());

    public static bool IsSupported(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ImageExtensions.Contains(ext) || VideoExtensions.Contains(ext);
    }
}
