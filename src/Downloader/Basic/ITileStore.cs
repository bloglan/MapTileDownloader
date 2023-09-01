namespace Downloader.Basic;

internal interface ITileStore
{
    Tile Get(int level, int x, int y);

    void Set(int level, int x, int y, Tile tile);

    void Clear(int level, int x, int y);
}
