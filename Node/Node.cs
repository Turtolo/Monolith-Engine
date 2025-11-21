using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ConstructEngine.Components;
using ConstructEngine.Region;
using ConstructEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstructEngine.Nodes
{
    /// <summary>
    /// Represents a core engine “Node” which tracks a root object,
    /// region shape, metadata, and participates in load, update, and draw cycles.
    /// </summary>
    public class Node
    {
        private static readonly List<Node> allInstances = new();
        private static readonly Dictionary<string, Node> allInstancesDetailed = new();

        /// <summary>
        /// Gets a read-only list of all Node instances currently tracked.
        /// </summary>
        public static IReadOnlyList<Node> AllInstances => allInstances.AsReadOnly();

        /// <summary>
        /// Gets a read-only dictionary mapping names to Node instances.
        /// Useful for quick lookup or engine debugging.
        /// </summary>
        public static IReadOnlyDictionary<string, Node> AllInstancesDetailed 
            => new ReadOnlyDictionary<string, Node>(allInstancesDetailed);

        /// <summary>
        /// The root object associated with this node. Defaults to the node itself.
        /// </summary>
        public object Root { get; internal set; }
        
        /// <summary>
        /// The type of the root object. Automatically set in constructor.
        /// </summary>
        public Type RootType { get =>  Root?.GetType(); }

        /// <summary>
        /// Optional shape that defines a 2D region for collisions or spatial logic.
        /// </summary>
        public IRegionShape2D Shape { get; set; }

        /// <summary>
        /// Optional name for indexing and debugging.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Arbitrary metadata associated with the node.
        /// </summary>
        public Dictionary<string, object> Values { get; set; } = new();

        /// <summary>
        /// Creates a new Node with itself as the root object.
        /// </summary>
        public Node()
        {
            allInstances.Add(this);
        }

        /// <summary>
        /// Creates a new Node using the specified root object.
        /// </summary>
        public Node(object root)
        {
            allInstances.Add(this);
            Root = root;
        }

        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// Called every frame for rendering. Use SpriteBatch for drawing.
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch) { }

        /// <summary>
        /// Invokes Load() on all tracked nodes.
        /// </summary>
        public static void LoadObjects()
        {
            foreach (var o in allInstances.ToList())
                o.Load();
        }

        /// <summary>
        /// Invokes Unload() on all tracked nodes.
        /// </summary>
        public static void UnloadObjects()
        {
            foreach (var o in allInstances.ToList())
                o.Unload();
        }

        /// <summary>
        /// Updates all nodes using a shared GameTime object.
        /// </summary>
        public static void UpdateObjects(GameTime gameTime)
        {
            foreach (var o in allInstances.ToList())
                o.Update(gameTime);
        }

        /// <summary>
        /// Draws all nodes using the given SpriteBatch.
        /// </summary>
        public static void DrawObjects(SpriteBatch spriteBatch)
        {
            foreach (var o in allInstances.ToList())
                o.Draw(spriteBatch);
        }

        public static void DumpAllInstances()
        {
            UnloadObjects();
            allInstances.Clear();
        }
    }
}
