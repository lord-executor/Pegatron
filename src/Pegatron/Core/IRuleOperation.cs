namespace Pegatron.Core
{
	public delegate void RuleOperation();

	public interface IRuleOperations
	{
		RuleOperation Token(IRuleRef rule, IToken token);
		RuleOperation Call(IRuleRef rule, TokenStreamIndex index, out CoroutineResult<RuleResult> result);
		RuleOperation Complete(IRuleRef rule, RuleResult result);
	}
}
