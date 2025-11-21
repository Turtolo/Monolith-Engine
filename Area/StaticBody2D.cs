

using System.Collections.Generic;
using ConstructEngine.Objects;

namespace ConstructEngine.Area
{
    public class StaticBody2D : CTObject, IObject
    {
        private static List<StaticBody2D> allInstances = new List<StaticBody2D>();
    

        public static IReadOnlyList<StaticBody2D> AllInstances => allInstances.AsReadOnly();

        public StaticBody2D()
        {
            allInstances.Add(this);
        }

        public StaticBody2D(IRegionShape2D shape)
        {
            Shape = shape;
            allInstances.Add(this);
        }
    }
}