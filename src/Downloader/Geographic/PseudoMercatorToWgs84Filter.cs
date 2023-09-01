using NetTopologySuite.Geometries;

namespace Downloader.Geographic;
internal class PseudoMercatorToWgs84Filter : ICoordinateFilter
{
    public void Filter(Coordinate coord)
    {
        var x = coord.X / (Math.PI * PseudoMercatorConstants.R) * 180.0;
        var y = coord.Y / (Math.PI * PseudoMercatorConstants.R) * 180.0;
        y = 180.0 / Math.PI * (2.0 * Math.Atan(Math.Exp(y * Math.PI / 180.0)) - Math.PI / 2.0);
        coord.X = x;
        coord.Y = y;
    }
}
