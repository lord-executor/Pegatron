using System.Collections.Generic;
using System.Linq;

namespace Pegatron.Grammars.Math.Ast
{
	public class BinaryChain : Node
    {
        public Node Head { get; }
		public IList<(string op, Node rhs)> Tail { get; }
		
        public BinaryChain(Node head, IEnumerable<BinaryChainLink> tail)
        {
            Head = head;
            Tail = tail.Select(l => (l.Op.Token.Value!, l.Right)).ToList();
        }
    }
}
