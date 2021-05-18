using Pegatron.Core;

namespace Pegatron
{
	public interface IRuleRef : IRule
	{
		public static string LiftRefName = "!";
		public static string UndefinedRef = "UNDEFINED";

		string? RefName { get; }

		IRuleRef As(string refName);
	}

	public interface IRuleRef<TNode> : IRuleRef
	{
		Reducer<TNode>? Reducer { get; }
		new IRuleRef<TNode> As(string refName);
		IRuleRef<TNode> ReduceWith(Reducer<TNode> reducer);
	}

	public static class RuleRefExtensions
	{
		public static IRuleRef<TNode> ReduceWith<TNode>(this IRuleRef<TNode> ruleRef, SingleReducer<TNode> reducer)
		{
			return ruleRef.ReduceWith((rule, page) => EnumSequence.Of(reducer(rule, page)));
		}

		public static IRuleRef<TNode> Lift<TNode>(this IRuleRef<TNode> ruleRef)
		{
			return ruleRef.As(IRuleRef.LiftRefName);
		}
	}
}
