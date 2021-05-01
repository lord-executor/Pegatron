using Pegatron.Core;

namespace Pegatron
{
	public interface IRuleRef : IRule
	{
		string? RefName { get; }
	}

	public interface IRuleRef<TNode> : IRuleRef
	{
		Reducer<TNode>? Reducer { get; }
		IRuleRef<TNode> As(string refName);
		IRuleRef<TNode> ReduceWith(Reducer<TNode> reducer);
	}

	public static class RuleRefExtensions
	{
		public static IRuleRef<TNode> ReduceWith<TNode>(this IRuleRef<TNode> ruleRef, SingleReducer<TNode> reducer)
		{
			return ruleRef.ReduceWith((rule, page) => EnumSequence.Of(reducer(rule, page)));
		}
	}
}
