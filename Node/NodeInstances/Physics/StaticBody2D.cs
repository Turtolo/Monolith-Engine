using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monolith.Geometry;


namespace Monolith.Nodes
{
    public record class StaticBodyConfig : SpatialNodeConfig
    {
        public bool Collidable { get; set; }
        public bool OneWay { get; set; }
    }
    public class StaticBody2D : Node2D
    {
        public bool Collidable;
        public bool OneWay;

        public StaticBody2D(StaticBodyConfig cfg) : base(cfg)
        {
            Collidable = cfg.Collidable;
            OneWay = cfg.OneWay;
        }
    }
}