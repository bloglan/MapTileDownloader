using Downloader.Geographic;
using Downloader.QueueService;
using Microsoft.Extensions.Options;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Downloader.Manager;

internal class TileDownloadManager
{
    private readonly TileDownloadManagerOptions options;
    private readonly ILogger<TileDownloadManager>? logger;
    private readonly ICollection<TileDownloadPlan> plans;
    private readonly GeometryFactory geometryFactory;
    private readonly ITileDownloadTaskQueue tileTaskQueue;


    public TileDownloadManager(IOptions<TileDownloadManagerOptions> options, ILogger<TileDownloadManager>? logger, ITileDownloadTaskQueue tileTaskQueue)
    {
        this.options = options.Value;
        this.logger = logger;
        this.plans = new List<TileDownloadPlan>();
        this.geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(3857);
        this.tileTaskQueue = tileTaskQueue;
    }

    public ICollection<TileDownloadPlan> Plans => this.plans;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        this.logger?.LogInformation("已启动下载管理器。");
        foreach (var plan in this.plans)
        {
            this.logger?.LogInformation("正在处理下载计划{plan}", plan.Name);
            //处理地理范围。
            Geometry scope = plan.Scope.SRID switch
            {
                4326 => PseudoMercatorTransformer.ToWebMercator(plan.Scope),
                3857 => plan.Scope.Copy(),
                _ => throw new ArgumentException("下载任务给定的地理边界SRID无效。仅支持4326（WGS-84)和3857（WGS-84/PseudoMercator)。"),
            };

            //分层级
            for (int level = plan.MinLevel; level <= plan.MaxLevel; level++)
            {
                this.logger?.LogInformation("开始处理第{level}级。", level);
                double tileSize = PseudoMercatorConstants.C / Math.Pow(2, level);
                //计算要下载的块。
                //获取x的左右边界
                var left = scope.Envelope.Coordinates.Min(p => p.X);
                var right = scope.Envelope.Coordinates.Max(p => p.X);
                int colLeft = (int)Math.Floor((left + PseudoMercatorConstants.HALF_C) / tileSize);
                int colRight = (int)Math.Ceiling((right + PseudoMercatorConstants.HALF_C) / tileSize);

                //按x列展开
                for (int x = colLeft; x < colRight; x++)
                {
                    //构造一个矩形遮罩
                    var stripMask = this.geometryFactory.CreateRectangle(x * tileSize - PseudoMercatorConstants.HALF_C,
                                                                 PseudoMercatorConstants.HALF_C,
                                                                 (x + 1) * tileSize - PseudoMercatorConstants.HALF_C,
                                                                 0 - PseudoMercatorConstants.HALF_C);

                    //是地理范围和矩形求交，求交可能会得到多个多边形，需要遍历。
                    var strips = scope.Intersection(stripMask);
                    for (int stripIndicator = 0; stripIndicator < strips.NumGeometries; stripIndicator++)
                    {
                        var strip = strips.GetGeometryN(stripIndicator);
                        if (strip is not Polygon)
                            continue;
                        if (strip.IsEmpty)
                            continue;

                        //获取y的上下界
                        var top = strip.Envelope.Coordinates.Max(p => p.Y);
                        var bottom = strip.Envelope.Coordinates.Min(p => p.Y);
                        int rowTop = (int)Math.Ceiling((top + PseudoMercatorConstants.HALF_C) / tileSize);
                        int rowBottom = (int)Math.Floor((bottom + PseudoMercatorConstants.HALF_C) / tileSize);

                        this.logger?.LogDebug("level:{level}, x:{x} in [{left}, {right}]={xCount}, y:[{top}, {bottom}]={yCount}", level, x, colLeft, colRight, colRight - colLeft - 1, rowTop, rowBottom, rowTop - rowBottom - 1);

                        for (int y = rowBottom; y < rowTop; y++)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                this.logger?.LogInformation("已请求停止。");
                                return;
                            }

                            var task = new TileDownloadTask(this.logger, level, x, y, plan, this.options);
                            await this.tileTaskQueue.QueueAsync(task.DoGetAsync);
                            //continue; //暂时绕过
                        }
                    }

                }
            }
        }
    }
}
