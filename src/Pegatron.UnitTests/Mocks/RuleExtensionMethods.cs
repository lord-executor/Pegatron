using Pegatron.Core;
using System.Linq;

namespace Pegatron.UnitTests.Mocks
{
	public static class RuleExtensionMethods
	{
		public static RuleResult Evaluate(this IRule rule, RuleContextMock context)
		{
			// In order to fully execute the rule, we have to iterate completely through the returned
			// rule operations, simulating the execution of the called coroutines.
			rule.Grab(context).ToList();
			return context.Result;
		}
	}
}
