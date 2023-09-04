using Downloader.Manager;
using Downloader.QueueService;
using Downloader.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<TileDownloadService>();
builder.Services.AddHostedService<QueuedTileDownloader>();

builder.Services.AddScoped<TileDownloadManager>();
builder.Services.AddSingleton<ITileDownloadTaskQueue, DefaultTileDownloadTaskQueue>();

var host = builder.Build();

host.Run();

