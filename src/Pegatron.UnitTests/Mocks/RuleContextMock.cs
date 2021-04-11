using Pegatron.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegatron.UnitTests.Mocks
{
	public class RuleContextMock : IRuleContext
	{
		private static readonly RuleOperation _noOpRuleOperation = () => { };

		private RuleResult? _result;

		public TokenStreamIndex Index { get; }
		public RuleResult Result => _result ?? throw new Exception("Called rule did not return a result");
		public IList<IToken> Tokens { get; } = new List<IToken>();

		public RuleContextMock(TokenStreamIndex index)
		{
			Index = index;
		}

		public RuleOperation Call(IRuleRef rule, TokenStreamIndex index, out CoroutineResult<RuleResult> result)
		{
			var ctx = new RuleContextMock(index);
			rule.Grab(ctx).ToList();
			result = new CoroutineResult<RuleResult>();
			result.Resolve(ctx.Result);
			foreach (var t in ctx.Tokens)
			{
				Tokens.Add(t);
			}
			return _noOpRuleOperation;
		}

		public RuleOperation Failure()
		{
			if (_result != null)
			{
				throw new Exception("Rule can only either succeed OR fail, not both");
			}
			_result = new RuleResult(false, Index, 0, TokenStreamSpan.Empty);
			return _noOpRuleOperation;
		}

		public RuleOperation Success(TokenStreamIndex idx)
		{
			if (_result != null)
			{
				throw new Exception("Rule can only either succeed OR fail, not both");
			}
			_result = new RuleResult(true, idx, 0, TokenStreamSpan.Empty);
			return _noOpRuleOperation;
		}

		public RuleOperation Token(IToken token)
		{
			Tokens.Add(token);
			return _noOpRuleOperation;
		}

		public string ConcatTokens()
		{
			return Tokens.Select(t => t.Value).StrJoin(String.Empty);
		}
	}
}
