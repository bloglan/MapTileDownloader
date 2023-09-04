using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace Downloader.QueueService;
internal class DefaultTileDownloadTaskQueue : ITileDownloadTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;
    private readonly DefaultTileDownloadTaskQueueOptions options;

    public DefaultTileDownloadTaskQueue(IOptions<DefaultTileDownloadTaskQueueOptions> options)
    {
        this.options = options.Value;
        BoundedChannelOptions boundedOptions = new(this.options.QueueCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        this._queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(boundedOptions);
    }

    public async ValueTask QueueAsync(
        Func<CancellationToken, ValueTask> workItem)
    {
        if (workItem is null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        await this._queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
        CancellationToken cancellationToken)
    {
        Func<CancellationToken, ValueTask>? workItem =
            await this._queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }
}
