using NetTopologySuite.Geometries;

namespace Downloader.Geographic;
internal static class GeometryFactoryExtensions
{
    /// <summary>
    /// 创建一个矩形。
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="left"></param>
    /// <param name="top"></param>
    /// <param name="right"></param>
    /// <param name="bottom"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Geometry CreateRectangle(this GeometryFactory factory, double left, double top, double right, double bottom)
    {
        return right < left
            ? throw new ArgumentException("right less than left.", nameof(right))
            : top < bottom
            ? throw new ArgumentException("top less than bottom.", nameof(top))
            : (Geometry)factory.CreatePolygon(new Coordinate[]
        {
            new Coordinate(left, top),
            new Coordinate(left, bottom),
            new Coordinate(right, bottom),
            new Coordinate(right, top),
            new Coordinate(left, top)
        });
    }
}
