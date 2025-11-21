using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace ConstructEngine.Area
{
    public interface IRegionShape2D
    {
        int X { get; set; }
        int Y { get; set; }

        Point Location { get; set; }
        void Offset(int x, int y);

        Rectangle BoundingBox { get; }

        bool Contains(Point p);
        bool Contains(IRegionShape2D other);
        bool Intersects(IRegionShape2D other);

        IRegionShape2D Clone();
    }


}