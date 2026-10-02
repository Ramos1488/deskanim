using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeskAnim.Media;

public sealed class LocalSource : IMediaSource
{
    private static readonly string[] Extensions = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };

    public string Name => "My library";

    public LocalSource() => Directory.CreateDirectory(AppPaths.Library);

    /// <summary>Copies a file into the library and returns the new path.</summary>
    public string Import(string sourcePath)
    {
        var baseName = Path.GetFileNameWithoutExtension(sourcePath);
        var ext = Path.GetExtension(sourcePath);
        var dest = Path.Combine(AppPaths.Library, baseName + ext);

        for (int i = 1; File.Exists(dest); i++)
            dest = Path.Combine(AppPaths.Library, $"{baseName}-{i}{ext}");

        File.Copy(sourcePath, dest);
        return dest;
    }

    public Task<IReadOnlyList<MediaItem>> SearchAsync(string query, CancellationToken ct = default)
    {
        var items = Directory.EnumerateFiles(AppPaths.Library)
            .Where(f => Extensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
            .Where(f => string.IsNullOrWhiteSpace(query) ||
                        Path.GetFileNameWithoutExtension(f).Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(f => new MediaItem(f, Path.GetFileNameWithoutExtension(f), f, Name))
            .ToList();

        return Task.FromResult<IReadOnlyList<MediaItem>>(items);
    }
}
