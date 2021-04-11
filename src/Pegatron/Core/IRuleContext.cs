namespace Pegatron.Core
{
	/// <summary>
	/// The rule context provides a slightly different interface than <see cref="IRuleOperations"/> for more
	/// convenient usage in rule code.
	/// </summary>
	public interface IRuleContext
	{
		TokenStreamIndex Index { get; }

		RuleOperation Token(IToken token);
		RuleOperation Call(IRuleRef rule, TokenStreamIndex index, out CoroutineResult<RuleResult> result);
		RuleOperation Success(TokenStreamIndex idx);
		RuleOperation Failure();
	}
}
