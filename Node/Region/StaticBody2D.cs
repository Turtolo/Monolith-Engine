using System.Collections.Generic;
using ConstructEngine.Region;


namespace ConstructEngine.Nodes
{
    public class StaticBody2D : RegionNode
    {
        public StaticBody2D() {}

        public StaticBody2D(IRegionShape2D shape)
        {
            Shape = shape;
        }
    }
}