using Microsoft.Extensions.Options;

namespace Downloader.QueueService;

/// <summary>
/// 队列多任务下载器
/// </summary>
internal class QueuedTileDownloader : BackgroundService
{
    private readonly ITileDownloadTaskQueue _taskQueue;
    private readonly ILogger<QueuedTileDownloader>? _logger;
    private readonly QueuedTileDownloaderOptions options;

    public QueuedTileDownloader(ITileDownloadTaskQueue taskQueue, ILogger<QueuedTileDownloader>? logger, IOptions<QueuedTileDownloaderOptions> options)
    {
        this._taskQueue = taskQueue;
        this._logger = logger;
        this.options = options.Value;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._logger?.LogInformation("{service}正在运行……", nameof(QueuedTileDownloader));
        return this.ProcessTaskQueueAsync(stoppingToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
    {
        Task[]? tasks = new Task[this.options.MaxParallelTasks];  //初始化任务池
        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.CompletedTask;
        }


        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var i = Task.WaitAny(tasks, stoppingToken);
                this._logger?.LogDebug("任务池[{index}]已完成。", i);

                Func<CancellationToken, ValueTask>? workItem = await this._taskQueue.DequeueAsync(stoppingToken);
                tasks[i] = Task.Run(async () =>
                {
                    await workItem(stoppingToken);
                }, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                this._logger?.LogDebug("已收到停止信号，将退出排队循环。");
                break;
            }
            catch (Exception ex)
            {
                this._logger?.LogError(ex, "Error occurred executing task work item.");
                throw;
            }
        }
        this._logger?.LogInformation("{type}已退出排队循环。", nameof(QueuedTileDownloader));
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        this._logger?.LogInformation($"{nameof(QueuedTileDownloader)}正在停止...");
        await base.StopAsync(stoppingToken);
    }
}
