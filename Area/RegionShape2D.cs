using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace ConstructEngine.Area
{
    public interface IRegionShape2D
    {
        bool Contains(Point p);
        bool Intersects(IRegionShape2D other);
    }
}