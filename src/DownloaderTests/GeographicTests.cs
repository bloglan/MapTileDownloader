using Downloader.Geographic;
using NetTopologySuite.Geometries;

namespace DownloaderTests;

public class GeographicTests
{
    private readonly GeometryFactory factory;
    public GeographicTests()
    {
        this.factory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);
    }
    [Fact]
    public void TransformTo3857()
    {
        var point = this.factory.CreatePoint(new Coordinate(-180.0, -85.0511288));
        var result = PseudoMercatorTransformer.ToWebMercator(point);
        Assert.Equal(-20037508.368847027, result.Coordinate.Y, 7);
    }

    [Fact]
    public void TransformTo4326()
    {
        var point = this.factory.CreatePoint(new Coordinate(-20037508.342789244, -20037508.342789244));
        point.SRID = 3857;
        var result = PseudoMercatorTransformer.ToWgs84(point);
        Assert.Equal(-85.0511288, result.Coordinate.Y, 7);
    }
}