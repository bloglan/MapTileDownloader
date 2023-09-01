using Microsoft.Extensions.Options;

namespace Downloader.Basic;
internal class FileSystemTileStore : ITileStore
{
    private readonly FileSystemTileStoreOptions options;

    public FileSystemTileStore(IOptions<FileSystemTileStoreOptions> options)
    {
        this.options = options.Value;
    }

    public void Clear(int level, int x, int y)
    {
        throw new NotImplementedException();
    }

    public Tile Get(int level, int x, int y)
    {
        throw new NotImplementedException();
    }

    public void Set(int level, int x, int y, Tile tile)
    {
        throw new NotImplementedException();
    }
}
