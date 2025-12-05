using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Geometry;
using Monolith.Nodes;

namespace Monlith.Nodes
{
    public record class CollisionShapeConfig : SpatialNodeConfig
    {
        public IRegionShape2D Shape { get; set; }
    }

    public class CollisionShape2D : Node2D
    {
        public bool Disabled { get; set; }

        public CollisionShape2D(CollisionShapeConfig cfg) : base(cfg)
        {
            Console.WriteLine("yes");
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Unload()
        {
            base.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }


    }
}