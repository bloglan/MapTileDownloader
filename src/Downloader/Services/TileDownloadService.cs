using Downloader.Manager;
using Downloader.QueueService;
using NetTopologySuite;
using NetTopologySuite.IO;

namespace Downloader.Services;

internal class TileDownloadService : BackgroundService
{
    private readonly ILogger<TileDownloadService>? _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ITileDownloadTaskQueue queue;

    public TileDownloadService(IServiceScopeFactory scopeFactory, ITileDownloadTaskQueue queue, ILogger<TileDownloadService>? logger)
    {
        this._logger = logger;
        this._scopeFactory = scopeFactory;
        this.queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._logger?.LogDebug("开始执行");
        using var scope = this._scopeFactory.CreateScope();

        WKTReader wKTReader = new(NtsGeometryServices.Instance);
        using var file = File.OpenRead("yunnan-scope.wkt");
        var geoScope = wKTReader.Read(file);
        geoScope.SRID = 4326;

        var downloaderManager = scope.ServiceProvider.GetRequiredService<TileDownloadManager>();
        downloaderManager.Plans.Add(new TileDownloadPlan()
        {
            Name = "Google Maps Image",
            DataDirectory = "\\\\a-200029003\\e$\\gis\\tiles\\google-image",
            Scope = geoScope,
            MinLevel = 10,
            MaxLevel = 20,
            Override = false,
            TileUrlTemplate = "https://gac-geo.googlecnapps.cn/maps/vt?lyrs=s&z={level}&x={x}&y={googleY}",
            FileFormat = "jpg",
        });
        downloaderManager.Plans.Add(new TileDownloadPlan()
        {
            Name = "Tianditu Road",
            DataDirectory = "\\\\a-200029003\\e$\\gis\\tiles\\tianditu-road",
            Scope = geoScope,
            MinLevel = 8,
            MaxLevel = 18,
            Override = false,
            TileUrlTemplate = "https://maps.ynmap.cn/arcgis/rest/services/TdtYn/tdtYnImgLableWGS84/MapServer/tile/{level}/{googleY}/{x}",
            FileFormat = "png",
        });
        await downloaderManager.StartAsync(stoppingToken);

        this._logger?.LogDebug("执行已到结尾");
    }
}
