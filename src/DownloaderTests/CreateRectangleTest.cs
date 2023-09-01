using Downloader.Geographic;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace DownloaderTests;
public class CreateRectangleTest
{
    [Fact]
    public void CreateRectangle()
    {
        var factory = NtsGeometryServices.Instance.CreateGeometryFactory();
        var reference = factory.CreatePolygon(new Coordinate[]
        {
            new Coordinate(0,0),
            new Coordinate(1,0),
            new Coordinate(1,1),
            new Coordinate(0,1),
            new Coordinate(0,0),
        });

        var result = factory.CreateRectangle(0, 1, 1, 0);
        Assert.True(result.IsRectangle);
        Assert.True(result.Equals(reference));
    }
}
