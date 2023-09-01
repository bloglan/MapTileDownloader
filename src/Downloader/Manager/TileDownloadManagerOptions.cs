namespace Downloader.Manager;

internal class TileDownloadManagerOptions
{
    public HttpClient BackChannel { get; internal set; } = new HttpClient();
}