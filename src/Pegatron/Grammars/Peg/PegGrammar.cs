using Pegatron.Core;
using Pegatron.Core.Rules;
using Pegatron.Grammars.Peg.Ast;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegatron.Grammars.Peg
{
	public class PegGrammar : Grammar<INode>
	{
		public PegGrammar()
		{
			// definition        := T<IDENTIFIER>#name ':=' choice#rule
			this.Sequence("definition",
				this.Terminal(TokenType.Identifier).As("name"),
				this.TerminalValue(":="),
				Ref("choice").As("rule")
			).ReduceWith(MakeDefinition);

			// choice            := sequence ('|' sequence #!)*
			this.Sequence("choice",
				Ref("sequence"),
				this.Optional(null, this.Sequence(null,
					this.TerminalValue("|"),
					Ref("sequence").Lift()
				))
			).ReduceWith(MakeChoice);

			// sequence          := namedAtom+
			this.OneOrMore("sequence",
				Ref("namedAtom")
			).ReduceWith(MakeSequence);

			// namedAtom         := atomExpression #atom ('#' T<IDENTIFIER> #name)?
			this.Sequence("namedAtom",
				Ref("atomExpression").As("atom"),
				this.Optional(null,
					this.Sequence(null, this.TerminalValue("#"), this.Terminal(TokenType.Identifier).Lift())
				).As("name")
			).ReduceWith(MakeNamedAtom);

			// atomExpression    := ('&' | '!')? #prefix atom range? #suffix
			this.Sequence("atomExpression",
				this.Optional(null, this.Choice(null, this.TerminalValue("&"), this.TerminalValue("!"))).As("prefix"),
				Ref("atom"),
				this.Optional(null, Ref("range")).As("suffix")
			).ReduceWith(MakeAtomExpression);

			// range             := ('*' | '+' | '?' | minmax)
			this.Choice("range",
				this.TerminalValue("*"),
				this.TerminalValue("+"),
				this.TerminalValue("?"),
				Ref("minmax")
			).ReduceWith(MakeRange);

			// minmax            := '{' T<NUMBER> #min (',' T<NUMBER> #max)? '}'
			this.Sequence("minmax",
				this.TerminalValue("{"),
				this.Terminal(TokenType.Number).As("min"),
				this.Optional(null, this.Sequence(null, 
					this.TerminalValue(","),
					this.Terminal(TokenType.Number).Lift()
				)).As("max")
			).ReduceWith(MinMaxRange);

			// atom              := ruleRef | terminal | '(' choice #! ')'
			this.Choice("atom",
				Ref("ruleRef"),
				Ref("terminal"),
				this.Sequence(null, this.TerminalValue("("), Ref("choice").Lift(), this.TerminalValue(")"))
			);

			// ruleRef           := T<IDENTIFIER>
			this.Terminal(TokenType.Identifier, "ruleRef")
				.ReduceWith(MakeReference);

			// terminal          := terminalType | terminalLiteral | terminalAny
			this.Choice("terminal",
				Ref("terminalType"),
				Ref("terminalLiteral"),
				Ref("terminalAny")
			);

			// terminalType      := 'T' '<' T<IDENTIFIER> #type '>'
			this.Sequence("terminalType",
				this.TerminalValue("T"),
				this.TerminalValue("<"),
				this.Terminal(TokenType.Identifier).As("type"),
				this.TerminalValue(">")
			).ReduceWith(TerminalType);

			// terminalLiteral   := T<LITERAL> | 'T' '<' T<LITERAL> #! '>'
			this.Choice("terminalLiteral",
				this.Terminal(TokenType.Literal),
				this.Sequence(null,
					this.TerminalValue("T"),
					this.TerminalValue("<"),
					this.Terminal(TokenType.Literal).Lift(),
					this.TerminalValue(">")
				)
			).ReduceWith(TerminalLiteral);

			// terminalAny       := '.'
			this.TerminalValue(".", "terminalAny")
				.ReduceWith(TerminalAny);

			StartWith("definition");
		}

		public override IEnumerable<INode> TerminalReducer(IRule rule, IToken token)
		{
			ArgAssert.NotNull(nameof(token.Value), token.Value);
			yield return new Value(token.Value);
		}

		public override IEnumerable<INode> DefaultReducer(IRule rule, INodeContext<INode> page)
		{
			if (page.HasLift())
			{
				return page.GetLift();
			}

			return page.GetAll();
		}

		private INode MakeDefinition(IRule rule, INodeContext<INode> page)
		{
			var name = page.Get("name").Single<Value>().Text;
			var definition = page.Get("rule").Single<ProtoRule>();
			definition.RuleName = name;

			RefNamePropagation(definition);

			return definition;
		}

		private INode MakeChoice(IRule rule, INodeContext<INode> page)
		{
			// choice with a single item => no choice needed
			if (page.Count == 1)
			{
				return page.Get(0);
			}

			var result = new ProtoRule(nameof(Choice), (grammar, rule) => grammar.Choice(rule.RuleName, rule.All(grammar)));
			// filter out the "|" values
			result.AddChildren(page.GetAll().Of<ProtoRule>());

			return result;
		}

		private INode MakeSequence(IRule rule, INodeContext<INode> page)
		{
			// sequence with a single item => no sequence needed
			if (page.Count == 1)
			{
				return page.Get(0);
			}

			var result = new ProtoRule(nameof(Sequence), (grammar, rule) => grammar.Sequence(rule.RuleName, rule.All(grammar)));
			result.AddChildren(page.GetAll().Of<ProtoRule>());

			return result;
		}

		private INode MakeNamedAtom(IRule rule, INodeContext<INode> page)
		{
			if (page.Count > 1)
			{
				var atom = page.Get("atom").Single<ProtoRule>();
				var name = page.Get("name").Single<Value>().Text;
				atom.RefName = name;
				return atom;
			}
			else
			{
				return page.Get(0);
			}
		}

		private static readonly IDictionary<string, Func<ProtoRule>> _prefixMap = new Dictionary<string, Func<ProtoRule>>
		{
			["&"] = () => new ProtoRule(nameof(And), (grammar, rule) => grammar.And(rule.RuleName, rule.Single(grammar))),
			["!"] = () => new ProtoRule(nameof(Not), (grammar, rule) => grammar.Not(rule.RuleName, rule.Single(grammar))),
		};

		private INode MakeAtomExpression(IRule rule, INodeContext<INode> page)
		{
			var prefix = page.Get("prefix").Optional<Value>()?.Text;
			var range = page.Get("suffix").Optional<Ast.Range>();
			var atom = page.Get("atom").Single<ProtoRule>();
			var result = atom;

			if (range != null)
			{
				result = new ProtoRule(nameof(Repeat), (grammar, rule) => grammar.Repeat(rule.RuleName, rule.Single(grammar), range.Min, range.Max));
				result.Children.Add(atom);
				atom = result;
			}

			if (prefix != null)
			{
				result = _prefixMap[prefix]();
				result.Children.Add(atom);
			}

			return result;
		}

		private INode MakeRange(IRule rule, INodeContext<INode> page)
		{
			var node = page.Get(0);
			if (node is Ast.Range)
			{
				return node;
			}

			var special = ((Value)node).Text;

			switch (special)
			{
				case "*":
					return new Ast.Range(0, -1);
				case "+":
					return new Ast.Range(1, -1);
				case "?":
					return new Ast.Range(0, 1);
				default:
					throw new InvalidOperationException();
			}
		}

		private INode MinMaxRange(IRule rule, INodeContext<INode> page)
		{
			var min = int.Parse(page.Get("min").Single<Value>().Text ?? "0");
			var max = int.Parse(page.Get("max").Single<Value>().Text ?? "0");
			return new Ast.Range(min, max);
		}

		private INode MakeReference(IRule rule, INodeContext<INode> page)
		{
			var ruleName = ((Value)page.Get(0)).Text ?? string.Empty;
			var result = new ProtoRule(nameof(Ref), (grammar, rule) => grammar.Ref(ruleName));
			result.RuleName = ruleName;
			return result;
		}

		private INode TerminalType(IRule rule, INodeContext<INode> page)
		{
			var type = page.Get("type").Single<Value>().Text ?? string.Empty;
			return new ProtoRule(nameof(TerminalType), (grammar, rule) => grammar.TerminalType(type, rule.RuleName));
		}

		private INode TerminalLiteral(IRule rule, INodeContext<INode> page)
		{
			var value = ((Value)page.Get(0)).Text ?? string.Empty;
			return new ProtoRule(nameof(TerminalLiteral), (grammar, rule) => grammar.TerminalValue(value, rule.RuleName));
		}

		private INode TerminalAny(IRule rule, INodeContext<INode> page)
		{
			return new ProtoRule(nameof(AnyTerminal), (grammar, rule) => grammar.Any(rule.RuleName));
		}

		private string? RefNamePropagation(ProtoRule rule, int level = 0)
		{
			foreach (var child in rule.Children)
			{
				var name = RefNamePropagation(child, level + 1);
				if (level > 0 && name != null)
				{
					if (rule.RefName != null)
					{
						throw new Exception($"Cannot propagate ref name inside of named RuleRef. {rule.DisplayText} is already named {rule.RefName}");
					}

					rule.RefName = name;
				}
			}

			if (level > 0)
			{
				var namedChildren = rule.Children.Where(r => r.RefName != null).ToList();
				if (namedChildren.Count > 1)
				{
					throw new Exception($"Cannot propagate multiple ref names inside of {rule.DisplayText}");
				}

				if (namedChildren.Count == 1)
				{
					var name = namedChildren.Single().RefName;
					namedChildren.Single().RefName = IRuleRef.LiftRefName;
					return name;
				}
			}

			return null;
		}
	}
}
