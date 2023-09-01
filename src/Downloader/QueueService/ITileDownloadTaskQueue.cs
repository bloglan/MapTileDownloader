namespace Downloader.QueueService;
public interface ITileDownloadTaskQueue
{
    ValueTask QueueAsync(Func<CancellationToken, ValueTask> workItem);

    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}
