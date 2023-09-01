using NetTopologySuite.Geometries;

namespace Downloader.Geographic;

internal class PseudoMercatorTransformer
{
    /// <summary>
    /// 转换到WebMocator坐标系。3857
    /// </summary>
    /// <param name="geom"></param>
    /// <returns></returns>
    public static Geometry ToWebMercator(Geometry geom)
    {
        var result = geom.Copy();
        result.Apply(new Wgs84ToPseudoMercatorFilter());
        result.SRID = 3857;
        return result;
    }

    public static Geometry ToWgs84(Geometry geom)
    {
        var result = geom.Copy();
        result.Apply(new PseudoMercatorToWgs84Filter());
        result.SRID = 4326;
        return result;
    }
}
