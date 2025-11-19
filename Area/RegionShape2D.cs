using Microsoft.Xna.Framework;


namespace ConstructEngine.Area
{
    public interface IRegionShape2D
    {
        bool Contains(Point p);
        bool Intersects(IRegionShape2D other);
    }

    public class RegionShape2D
    {
        public IRegionShape2D Shape { get; set; }

        public RegionShape2D(IRegionShape2D shape)
        {
            Shape = shape;
        }
    }

}