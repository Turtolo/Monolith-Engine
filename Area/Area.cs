using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ConstructEngine.Helpers;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using ConstructEngine.Components;

namespace ConstructEngine.Area
{
    public class Area
    {
        private static List<Area> allInstances = new List<Area>();
        private bool wasInArea = false;
        
        public IRegionShape2D RegionShape {get; set;}
        public static IReadOnlyList<Area> AllInstances => allInstances.AsReadOnly();

        public Area(IRegionShape2D shape)
        {
            allInstances.Add(this);
            RegionShape = shape;
        }

        public bool AreaEntered(out Area overlapping)
        {
            overlapping = AllInstances
                .FirstOrDefault(a => a != this && RegionShape.Intersects(a.RegionShape));

            bool isInArea = overlapping != null;
            bool entered = !wasInArea && isInArea;

            wasInArea = isInArea;
            return entered;
        }

        public bool AreaEntered()
        {
            return AreaEntered(out _);
        }


        public bool AreaExited(out Area overlapping)
        {
            overlapping = AllInstances
                .FirstOrDefault(a => a != this && RegionShape.Intersects(a.RegionShape));

            bool isInArea = overlapping != null;
            bool exited = wasInArea && !isInArea;

            wasInArea = isInArea;
            return exited;
        }

        public bool AreaExited()
        {
            return AreaExited(out _);
        }
    }
}