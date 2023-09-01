namespace Downloader.QueueService;

internal class QueuedTileDownloaderOptions
{
    /// <summary>
    /// 最大并行的下载任务数。
    /// </summary>
    public int MaxParallelTasks { get; internal set; } = 128;
}