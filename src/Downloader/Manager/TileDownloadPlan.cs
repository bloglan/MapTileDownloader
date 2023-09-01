using NetTopologySuite.Geometries;

namespace Downloader.Manager;
internal class TileDownloadPlan
{
    /// <summary>
    /// 下载任务的名称。
    /// </summary>
    public string Name { get; set; } = "Untitled Plan";

    public Geometry Scope { get; set; } = Polygon.Empty;

    public int MinLevel { get; set; } = 0;

    public int MaxLevel { get; set; } = 0;

    public bool Override { get; set; } = false;

    public string DataDirectory { get; set; } = @"D:\Data";

    public string TileUrlTemplate { get; set; } = "http://localhost";

    public string FileFormat { get; set; } = "jpg";
}
