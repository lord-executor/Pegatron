using System.Collections.Generic;

namespace Pegatron
{
	public interface IGrammar<TNode>
	{
		/// <summary>
		/// The start rule of the grammar.
		/// </summary>
		IRuleRef<TNode>? StartRule { get; }

		/// <summary>
		/// The terminal reducer is essentially a factory that lifts matched terminal tokens into the world
		/// of the AST by wrapping the token value in an appropriate node.
		/// </summary>
		/// <param name="rule">The rule that is matching the token - this is usually a terminal rule</param>
		/// <param name="token">The lexer token that is being matched</param>
		/// <returns>Zero or more nodes that represent the given token - usually exactly one</returns>
		IEnumerable<TNode> TerminalReducer(IRule rule, IToken token);

		/// <summary>
		/// The default reducer is called whenever a rule does not have its own reducer configured.
		/// </summary>
		/// <param name="rule">The rule that succeeded its matching and for which the reducer is called.</param>
		/// <param name="page">The node context containing the nodes produced from child rules</param>
		/// <returns>Zero or more nodes that represent the result of reducing the given rule</returns>
		IEnumerable<TNode> DefaultReducer(IRule rule, INodeContext<TNode> page);
	}

	public static class GrammarExtensions
	{
		public static IRuleRef<TNode> Start<TNode>(this IGrammar<TNode> grammar)
		{
			return grammar.StartRule ?? throw new GrammarException(GrammarExceptionId.StartRuleNotDefined);
		}
	}
}
