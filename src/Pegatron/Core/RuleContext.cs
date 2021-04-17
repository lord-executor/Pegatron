namespace Pegatron.Core
{
	public class RuleContext : IRuleContext
	{
		private readonly IRuleRef _rule;
		private readonly IRuleOperations _operations;
		private int stepCount;

		public TokenStreamIndex Index { get; }

		public RuleContext(IRuleRef rule, TokenStreamIndex index, IRuleOperations operations)
		{
			_rule = rule;
			Index = index;
			_operations = operations;
		}

		public RuleOperation Token(IToken token)
		{
			return _operations.Token(_rule, token);
		}

		public RuleOperation Call(IRuleRef rule, TokenStreamIndex index, out CoroutineResult<RuleResult> result)
		{
			var operation = _operations.Call(rule, index, out result);
			// accumulate step counts of called child rules
			result.OnResolve += value => { stepCount += value.StepCount; };
			return operation;
		}

		public RuleOperation Success(TokenStreamIndex idx)
		{
			return _operations.Complete(_rule, new RuleResult(true, idx, stepCount + 1, Index.Until(idx)));
		}

		public RuleOperation Failure()
		{
			return _operations.Complete(_rule, new RuleResult(false, Index, stepCount + 1, TokenStreamSpan.Empty));
		}
	}
}
