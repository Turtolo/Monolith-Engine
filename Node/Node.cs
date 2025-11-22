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
    public abstract class Node
    {
        private static readonly List<Node> allInstances = new();
        private static readonly Dictionary<string, Node> allInstancesDetailed = new();

        private static readonly List<Node> pendingAdds = new();
        private static readonly List<Node> pendingRemovals = new();


        public static IReadOnlyList<Node> AllInstances => allInstances.AsReadOnly();
        public static IReadOnlyDictionary<string, Node> AllInstancesDetailed 
            => new ReadOnlyDictionary<string, Node>(allInstancesDetailed);
        public object Root { get; internal set; }
        public Type RootType { get =>  Root?.GetType(); }
        public IRegionShape2D Shape { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Values { get; set; } = new();


        /// <summary>
        /// Creates a new Node with itself as the root object.
        /// </summary>
        public Node()
        {
            QueueAdd(this);
        }

        /// <summary>
        /// Creates a new Node using the specified root object.
        /// </summary>
        public Node(object root)
        {
            Root = root;
            QueueAdd(this);
        }

        /// <summary>
        /// Queues a node for addition to the main instance list.
        /// </summary>
        private static void QueueAdd(Node node) => pendingAdds.Add(node);

        /// <summary>
        /// Queues a node for removal from the main instance list.
        /// </summary>
        public void QueeFree() => pendingRemovals.Add(this);

        /// <summary>
        /// Applies queued additions and removals before or after each lifecycle step.
        /// </summary>
        private static void ApplyPendingChanges()
        {
            if (pendingAdds.Count > 0)
            {
                allInstances.AddRange(pendingAdds);
                pendingAdds.Clear();
            }

            if (pendingRemovals.Count > 0)
            {
                foreach (var n in pendingRemovals)
                    allInstances.Remove(n);
                pendingRemovals.Clear();
            }
        }

        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

        /// <summary>
        /// Invokes Load() on all tracked nodes.
        /// </summary>
        public static void LoadObjects()
        {
            ApplyPendingChanges();

            for (int i = 0; i < allInstances.Count; i++)
            {
                allInstances[i].Load();
                ApplyPendingChanges();
            }
        }

        /// <summary>
        /// Invokes Unload() on all tracked nodes.
        /// </summary>
        public static void UnloadObjects()
        {
            ApplyPendingChanges();

            for (int i = 0; i < allInstances.Count; i++)
            {
                allInstances[i].Unload();
                ApplyPendingChanges();
            }
        }

        /// <summary>
        /// Updates all nodes using a shared GameTime object.
        /// </summary>
        public static void UpdateObjects(GameTime gameTime)
        {
            ApplyPendingChanges();

            for (int i = 0; i < allInstances.Count; i++)
            {
                allInstances[i].Update(gameTime);
                ApplyPendingChanges();
            }
        }

        /// <summary>
        /// Draws all nodes using the given SpriteBatch.
        /// </summary>
        public static void DrawObjects(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < allInstances.Count; i++)
            {
                allInstances[i].Draw(spriteBatch);
            }
        }

        public static void DumpAllInstances()
        {
            UnloadObjects();
            allInstances.Clear();
        }
    }
}
