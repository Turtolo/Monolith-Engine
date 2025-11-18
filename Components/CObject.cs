using System;
using System.Collections.Generic;
using ConstructEngine.Components;
using ConstructEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstructEngine.Objects
{
    public interface IObject
    {
        void Load();
        void Unload();
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }

    public class CObject : IObject
    {
        public static List<IObject> ObjectList { get; private set; } = new();
        public static Dictionary<string, IObject> ObjectDict { get; private set; } = new();
        
        public Rectangle Rectangle { get; set; }
        public string Name { get; set; }
        public Vector2 Position {get => new Vector2(Rectangle.X, Rectangle.Y);}
        public Dictionary<string, object> Values { get; set; }

        public CObject()
        {
            ObjectList.Add(this);
        }

        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

        public static void LoadObjects() => ObjectList.ForEach(o => o.Load());
        public static void UpdateObjects(GameTime gameTime) => ObjectList.ForEach(o => o.Update(gameTime));
        public static void DrawObjects(SpriteBatch spriteBatch) => ObjectList.ForEach(o => o.Draw(spriteBatch));
    }
}