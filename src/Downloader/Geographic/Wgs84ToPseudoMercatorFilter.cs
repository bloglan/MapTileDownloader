using NetTopologySuite.Geometries;

namespace Downloader.Geographic;
internal class Wgs84ToPseudoMercatorFilter : ICoordinateFilter
{
    public void Filter(Coordinate coord)
    {
        coord.X = coord.X * Math.PI / 180.0 * PseudoMercatorConstants.R;

        double a = coord.Y * Math.PI / 180.0;
        coord.Y = PseudoMercatorConstants.R / 2.0 * Math.Log((1.0 + Math.Sin(a)) / (1.0 - Math.Sin(a)));
    }
}
