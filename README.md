# Map Tile Downloader

Map Tile Downloader是一款地图瓦片下载器，可用于将指定范围和级别的地图瓦片从数据源下载到指定的位置（通常是文件系统）。

## 支持的特性

* 可使用多边形指定范围，将下载与多边形范围相关联的瓦片
* 可用xyz参数化设置数据源模板
* 支持多个下载任务并行

## 构建和要求

如果从源代码开始构建，您必须在计算机上安装[dotnet 7.0](https://dotnet.microsoft.com/zh-cn/download)

在sln所在目录下运行`dotnet build`以构建程序。

Map Tile Downloader是一个命令行启动的程序，如果仅执行已构建的程序，您的计算机上需要安装[dotnet runtime](https://dotnet.microsoft.com/zh-cn/download/dotnet/7.0)

## 用法

* 初始化下载管理器（DownloadManager）
* 创建下载计划（TileDownloadPlan），使用范围和级别等参数配置下载计划
* 将下载计划添加到下载管理器，然后启动下载管理器。

请参阅源代码中`TileDownloadService.cs`的代码描述。

## 路线图

* 计划支持更多的下载目标
* 从文件载入下载计划
* 支持从停止处继续开始下载任务