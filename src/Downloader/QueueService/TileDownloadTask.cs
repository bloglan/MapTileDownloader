using Downloader.Manager;
using System.Net;
using System.Text;

namespace Downloader.QueueService;
internal class TileDownloadTask
{
    private readonly ILogger? logger;
    private readonly int level;
    private readonly int x;
    private readonly int y;
    private readonly TileDownloadPlan plan;
    private readonly TileDownloadManagerOptions options;

    public TileDownloadTask(ILogger? logger, int level, int x, int y, TileDownloadPlan plan, TileDownloadManagerOptions options)
    {
        this.logger = logger;
        this.level = level;
        this.x = x;
        this.y = y;
        this.plan = plan;
        this.options = options;
    }


    public async ValueTask DoGetAsync(CancellationToken token)
    {
        this.logger?.LogDebug("正在处理瓦片({level}/{x},{y}）,线程[{thread}]。", this.level, this.x, this.y, Environment.CurrentManagedThreadId);
        if (!this.plan.Override)
        {
            if (File.Exists(this.plan.DataDirectory + $"\\{this.level}\\{this.x}\\{this.y}.0"))
            {
                this.logger?.LogDebug("空瓦片({level}/{x},{y}）已存在。", this.level, this.x, this.y);
                return;
            }

            var fileInfo = new FileInfo(this.plan.DataDirectory + $"\\{this.level}\\{this.x}\\{this.y}.{this.plan.FileFormat}");
            if (fileInfo.Exists)
            {
                if (fileInfo.Length > 0)
                {
                    this.logger?.LogDebug("瓦片({level}/{x},{y}）已存在。", this.level, this.x, this.y);
                    return;
                }
            }
        }

        int googleY = (int)Math.Pow(2, this.level) - this.y - 1;
        _ = Directory.CreateDirectory(this.plan.DataDirectory + $"\\{this.level}\\{this.x}");
        HttpResponseMessage response;
        try
        {
            var sb = new StringBuilder(this.plan.TileUrlTemplate);
            sb.Replace("{level}", this.level.ToString());
            sb.Replace("{x}", this.x.ToString());
            sb.Replace("{y}", this.y.ToString());
            sb.Replace("{googleY}", googleY.ToString());

            response = await this.options.BackChannel.GetAsync(sb.ToString(), token);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    this.logger?.LogDebug("服务器上没有瓦片({level}/{x},{y}）", this.level, this.x, this.y);
                    File.Create(this.plan.DataDirectory + $"\\{this.level}\\{this.x}\\{this.y}.0").Close();
                }
                else
                {
                    this.logger?.LogWarning("获取瓦片({level}/{x},{y}）时，服务器返回错误。", this.level, this.x, this.y);
                    //Debug.Fail("调试服务器错误问题");
                }
                return;
            }
        }
        catch (HttpRequestException httpEx)
        {
            this.logger?.LogWarning("获取HTTP远端响应时发生错误。消息是{message}", httpEx.Message);
            return;
        }
        catch (TaskCanceledException)
        {
            this.logger?.LogInformation("任务已取消");
            return;
        }
        catch (Exception ex)
        {
            this.logger?.LogError("在向远端发送HTTP请求时发生错误。消息是{message}", ex.Message);
            throw;
        }


        using var file = File.Create(this.plan.DataDirectory + $"\\{this.level}\\{this.x}\\{this.y}.{this.plan.FileFormat}");
        await response.Content.CopyToAsync(file, token);
        file.Close();
        this.logger?.LogDebug("已下载瓦片({level}/{x},{y}）", this.level, this.x, this.y);
        file.Dispose();
    }
}
