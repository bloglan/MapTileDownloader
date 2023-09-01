namespace Downloader.Basic;

internal class FileSystemTileStoreOptions
{
    public string RootDirectory { get; set; } = @"D:\TileData";

    public string PathFormat { get; set; } = @"{level}\{x}\{y}\.{extension}";
}