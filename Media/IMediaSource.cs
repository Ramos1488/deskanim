using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeskAnim.Media;

/// <summary>
/// A place animations can come from: local library, online API (Giphy/Tenor), bundled assets.
/// Remote sources should download the chosen file into the local library and return a local FilePath.
/// </summary>
public interface IMediaSource
{
    string Name { get; }
    Task<IReadOnlyList<MediaItem>> SearchAsync(string query, CancellationToken ct = default);
}
