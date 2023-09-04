namespace Downloader.QueueService;
internal class DefaultTileDownloadTaskQueueOptions
{
    /// <summary>
    /// 队列容量。
    /// </summary>
    public int QueueCapacity { get; set; } = 64;
}
