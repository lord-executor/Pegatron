namespace Pegatron
{
	public interface IGrammar<TNode>
	{
		IRuleRef<TNode>? StartRule { get; }

		TNode TokenNodeFactory(IRule rule, IToken token);

		TNode DefaultReducer(IRule rule, INodeContext<TNode> page);
	}

	public static class GrammarExtensions
	{
		public static IRuleRef<TNode> Start<TNode>(this IGrammar<TNode> grammar)
		{
			return grammar.StartRule ?? throw new GrammarException(GrammarExceptionId.StartRuleNotDefined);
		}
	}
}
