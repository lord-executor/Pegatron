using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegatron
{
	public interface IGrammarRule<TNode>
	{
		void Register(IGrammarBuilder<TNode> grammar);
		TNode Reduce(IRule rule, INodeContext<TNode> page);
	}
}
