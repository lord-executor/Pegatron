using Pegatron.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegatron
{
	public static class GrammarBuilderExtensions
	{
		public static IRuleRef<TNode> Terminal<TNode>(this IGrammarBuilder<TNode> grammar, string terminalName, Predicate<IToken> tokenMatcher, string? name = null)
		{
			return grammar.DefineRule(name, new Terminal(name, terminalName, tokenMatcher));
		}

		public static IRuleRef<TNode> Terminal<TNode>(this IGrammarBuilder<TNode> grammar, string tokenType, string? name = null)
		{
			return grammar.DefineRule(name, new Terminal(name, tokenType));
		}

		public static IRuleRef<TNode> Sequence<TNode>(this IGrammarBuilder<TNode> grammar, string? name, params IRuleRef[] rules)
		{
			return grammar.DefineRule(name, new Sequence(name, rules));
		}

		public static IRuleRef<TNode> Choice<TNode>(this IGrammarBuilder<TNode> grammar, string? name, params IRuleRef[] rules)
		{
			return grammar.DefineRule(name, new Choice(name, rules));
		}

		public static IRuleRef<TNode> Repeat<TNode>(this IGrammarBuilder<TNode> grammar, string? name, IRuleRef rule, int min = 0, int max = -1)
		{
			return grammar.DefineRule(name, new Repeat(name, rule, min, max));
		}

		public static IRuleRef<TNode> ZeroOrMore<TNode>(this IGrammarBuilder<TNode> grammar, string? name, IRuleRef rule)
		{
			return grammar.DefineRule(name, new Repeat(name, rule, 0, -1));
		}

		public static IRuleRef<TNode> OneOrMore<TNode>(this IGrammarBuilder<TNode> grammar, string? name, IRuleRef rule)
		{
			return grammar.DefineRule(name, new Repeat(name, rule, 1, -1));
		}

		public static IRuleRef<TNode> Any<TNode>(this IGrammarBuilder<TNode> grammar, string? name)
		{
			return grammar.DefineRule(name, new AnyTerminal(name));
		}

		public static IRuleRef<TNode> And<TNode>(this IGrammarBuilder<TNode> grammar, string? name, IRuleRef target)
		{
			return grammar.DefineRule(name, new And(name, target));
		}

		public static IRuleRef<TNode> Not<TNode>(this IGrammarBuilder<TNode> grammar, string? name, IRuleRef target)
		{
			return grammar.DefineRule(name, new Not(name, target));
		}
	}
}
