using Pegatron.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegatron.UnitTests.Mocks
{
	public class RuleOperationsMock : IRuleOperations
	{
		private static readonly RuleOperation _noOpRuleOperation = () => { };

		private Stack<CoroutineResult<RuleResult>> _results = new Stack<CoroutineResult<RuleResult>>();
		private TokenStreamIndex _index;
		private RuleResult? _result;
		public RuleResult Result => _result ?? throw new Exception($"Called rule did not return a result.");
		public IList<IToken> Tokens { get; } = new List<IToken>();

		public RuleOperationsMock(TokenStreamIndex index)
		{
			_index = index;
		}

		public RuleOperationsMock Evaluate(IRule rule)
		{
			// Capture result of the initial rule call
			var result = new CoroutineResult<RuleResult>();
			result.OnResolve += r => _result = r;
			_results.Push(result);

			var context = new RuleContext(new SimpleRef(rule), _index, this);
			// In order to fully execute the rule, we have to iterate completely through the returned
			// rule operations, simulating the execution of the called coroutines.
			rule.Grab(context).ToList();
			return this;
		}

		public string ConcatTokens()
		{
			return Tokens.Select(t => t.Value).StrJoin(String.Empty);
		}

		public RuleOperation Call(IRuleRef rule, TokenStreamIndex index, out CoroutineResult<RuleResult> result)
		{
			// Make sure that each "nested" call gets its own result
			result = new CoroutineResult<RuleResult>();
			_results.Push(result);

			var context = new RuleContext(rule, index, this);
			rule.Grab(context).ToList();

			return _noOpRuleOperation;
		}

		public RuleOperation Complete(IRuleRef rule, RuleResult result)
		{
			_results.Pop().Resolve(result);
			return _noOpRuleOperation;
		}

		public RuleOperation Token(IRuleRef rule, IToken token)
		{
			Tokens.Add(token);
			return _noOpRuleOperation;
		}
	}
}
