using System.Collections.Generic;

namespace Pegatron.Grammars.Math.Ast
{
	public class CollectionNode : Node
    {
        public IEnumerable<Node> Nodes { get; }

        public CollectionNode(IEnumerable<Node> nodes)
        {
            Nodes = nodes;
        }
    }
}
