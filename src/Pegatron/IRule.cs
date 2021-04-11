using Pegatron.Core;
using System.Collections.Generic;

namespace Pegatron
{
	public interface IRule
	{
		string Name { get; }
		IEnumerable<RuleOperation> Grab(IRuleContext ctx);
	}
}
