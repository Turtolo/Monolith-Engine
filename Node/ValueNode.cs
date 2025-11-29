using System;
using Monolith;
using Monolith.Nodes;

namespace Monolith.Nodes
{
    public abstract class ValueNode<TValues> : Node
    {
        public TValues Values { get; }

        protected ValueNode(NodeConfig config) : base(config)
        {
            if (config.Values is not TValues values)
                throw new InvalidOperationException(
                    $"Node '{config.Name}' requires values of type {typeof(TValues).Name}");
            Values = values;
        }
    }
}
