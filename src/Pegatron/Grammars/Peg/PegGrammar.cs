using Pegatron.Core;
using Pegatron.Grammars.Peg.Ast;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegatron.Grammars.Peg
{
	public class PegGrammar : Grammar<INode>
	{
		public class Definition : IGrammarRule<INode>
		{
			public static string DefinitionText => "definition  :=  T<Identifier> #name ':=' choice #rule";

			public void Register(IGrammarBuilder<INode> grammar)
			{
				grammar.Sequence("definition",
					grammar.Terminal(TokenType.Identifier).As("name"),
					grammar.TerminalValue(":="),
					grammar.Ref("choice").As("rule")
				).ReduceWith(Reduce);
			}

			public INode Reduce(IRule rule, INodeContext<INode> page)
			{
				var name = page.Get("name").Single<Value>().Text;
				var definition = page.Get("rule").Single<ProtoRule>();
				definition.RuleName = name;

				RefNamePropagation(definition);

				return definition;
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
						if (name != IRuleRef.LiftRefName)
						{
							namedChildren.Single().RefName = IRuleRef.LiftRefName;
							return name;
						}
					}
				}

				return null;
			}
		}

		public class Choice : IGrammarRule<INode>
		{
			public static string DefinitionText => "choice  :=  sequence ('|' sequence #!)*";

			public void Register(IGrammarBuilder<INode> grammar)
			{
				grammar.Sequence("choice",
					grammar.Ref("sequence"),
					grammar.Optional(null, grammar.Sequence(null,
						grammar.TerminalValue("|"),
						grammar.Ref("sequence").Lift()
					))
				).ReduceWith(Reduce);
			}

			public INode Reduce(IRule rule, INodeContext<INode> page)
			{
				// choice with a single item => no choice needed
				if (page.Count == 1)
				{
					return page.Get(0);
				}

				var result = new ProtoRule(nameof(Core.Rules.Choice), (grammar, rule) => grammar.Choice(rule.RuleName, rule.All(grammar)));
				// filter out the "|" values
				result.AddChildren(page.GetAll().Of<ProtoRule>());

				return result;
			}
		}

		public class Sequence : IGrammarRule<INode>
		{
			public static string DefinitionText => "sequence  :=  namedAtom+";

			public void Register(IGrammarBuilder<INode> grammar)
			{
				grammar.OneOrMore("sequence",
					grammar.Ref("namedAtom")
				).ReduceWith(Reduce);
			}

			public INode Reduce(IRule rule, INodeContext<INode> page)
			{
				// sequence with a single item => no sequence needed
				if (page.Count == 1)
				{
					return page.Get(0);
				}

				var result = new ProtoRule(nameof(Core.Rules.Sequence), (grammar, rule) => grammar.Sequence(rule.RuleName, rule.All(grammar)));
				result.AddChildren(page.GetAll().Of<ProtoRule>());

				return result;
			}
		}

		public class NamedAtom : IGrammarRule<INode>
		{
			public static string DefinitionText => "namedAtom  :=  atomExpression #atom ('#' (T<Identifier> | '!') #name)?";

			public void Register(IGrammarBuilder<INode> grammar)
			{
				grammar.Sequence("namedAtom",
					grammar.Ref("atomExpression").As("atom"),
					grammar.Optional(null,
						grammar.Sequence(null,
							grammar.TerminalValue("#"),
							grammar.Choice(null,
								grammar.Terminal(TokenType.Identifier),
								grammar.TerminalValue("!")
							).Lift())
					).As("name")
				).ReduceWith(Reduce);
			}

			public INode Reduce(IRule rule, INodeContext<INode> page)
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
		}

		public class AtomExpression : IGrammarRule<INode>
		{
			public static string DefinitionText => "atomExpression  :=  ('&' | '!')? #prefix atom range? #suffix";

			private static readonly IDictionary<string, Func<ProtoRule>> _prefixMap = new Dictionary<string, Func<ProtoRule>>
			{
				["&"] = () => new ProtoRule(nameof(Core.Rules.And), (grammar, rule) => grammar.And(rule.RuleName, rule.Single(grammar))),
				["!"] = () => new ProtoRule(nameof(Core.Rules.Not), (grammar, rule) => grammar.Not(rule.RuleName, rule.Single(grammar))),
			};

			public void Register(IGrammarBuilder<INode> grammar)
			{
				grammar.Sequence("atomExpression",
					grammar.Optional(null,
						grammar.Choice(null,
							grammar.TerminalValue("&"),
							grammar.TerminalValue("!")
						)
					).As("prefix"),
					grammar.Ref("atom"),
					grammar.Optional(null, grammar.Ref("range")).As("suffix")
				).ReduceWith(Reduce);
			}

			public INode Reduce(IRule rule, INodeContext<INode> page)
			{
				var prefix = page.Get("prefix").Optional<Value>()?.Text;
				var range = page.Get("suffix").Optional<Ast.Range>();
				var atom = page.Get("atom").Single<ProtoRule>();
				var result = atom;

				if (range != null)
				{
					result = new ProtoRule(nameof(Core.Rules.Repeat), (grammar, rule) => grammar.Repeat(rule.RuleName, rule.Single(grammar), range.Min, range.Max));
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
		}

		public class RangeDef : IGrammarRule<INode>
		{
			public static string DefinitionText => "range  :=  ('*' | '+' | '?' | minmax)";

			public void Register(IGrammarBuilder<INode> grammar)
			{
				grammar.Choice("range",
					grammar.TerminalValue("*"),
					grammar.TerminalValue("+"),
					grammar.TerminalValue("?"),
					grammar.Ref("minmax")
				).ReduceWith(Reduce);
			}

			public INode Reduce(IRule rule, INodeContext<INode> page)
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
		}

		public class MinMaxDef : IGrammarRule<INode>
		{
			public static string DefinitionText => "minmax  :=  '{' T<Number> #min (',' T<Number> #max)? '}'";

			public void Register(IGrammarBuilder<INode> grammar)
			{
				grammar.Sequence("minmax",
					grammar.TerminalValue("{"),
					grammar.Terminal(TokenType.Number).As("min"),
					grammar.Optional(null, grammar.Sequence(null,
						grammar.TerminalValue(","),
						grammar.Terminal(TokenType.Number).Lift(),
						grammar.TerminalValue("}")
					)).As("max")
				).ReduceWith(Reduce);
			}

			public INode Reduce(IRule rule, INodeContext<INode> page)
			{
				var min = int.Parse(page.Get("min").Single<Value>().Text ?? "0");
				var max = int.Parse(page.Get("max").Single<Value>().Text ?? "0");
				return new Ast.Range(min, max);
			}
		}

		public class Atom : IGrammarRule<INode>
		{
			public static string DefinitionText => "atom  :=  ruleRef | terminal | '(' choice #! ')'";

			public void Register(IGrammarBuilder<INode> grammar)
			{
				grammar.Choice("atom",
					grammar.Ref("ruleRef"),
					grammar.Ref("terminal"),
					grammar.Sequence(null, grammar.TerminalValue("("), grammar.Ref("choice").Lift(), grammar.TerminalValue(")"))
				);
			}

			public INode Reduce(IRule rule, INodeContext<INode> page)
			{
				throw new NotImplementedException();
			}
		}

		public class RuleRefDef : IGrammarRule<INode>
		{
			public static string DefinitionText => "ruleRef  :=  T<Identifier>";

			public void Register(IGrammarBuilder<INode> grammar)
			{
				grammar.Terminal(TokenType.Identifier, "ruleRef")
					.ReduceWith(Reduce);
			}

			public INode Reduce(IRule rule, INodeContext<INode> page)
			{
				var ruleName = ((Value)page.Get(0)).Text ?? string.Empty;
				var result = new ProtoRule(nameof(Ref), (grammar, rule) => grammar.Ref(ruleName));
				result.RuleName = ruleName;
				return result;
			}
		}

		public class Terminal : IGrammarRule<INode>
		{
			public static string DefinitionText => "terminal  :=  terminalType | terminalLiteral | terminalAny";

			public void Register(IGrammarBuilder<INode> grammar)
			{
				grammar.Choice("terminal",
					grammar.Ref("terminalType"),
					grammar.Ref("terminalLiteral"),
					grammar.Ref("terminalAny")
				);
			}

			public INode Reduce(IRule rule, INodeContext<INode> page)
			{
				throw new NotImplementedException();
			}
		}

		public class TerminalType : IGrammarRule<INode>
		{
			public static string DefinitionText => "terminalType  :=  'T' '<' T<Identifier> #type '>'";

			public void Register(IGrammarBuilder<INode> grammar)
			{
				grammar.Sequence("terminalType",
					grammar.TerminalValue("T"),
					grammar.TerminalValue("<"),
					grammar.Terminal(TokenType.Identifier).As("type"),
					grammar.TerminalValue(">")
				).ReduceWith(Reduce);
			}

			public INode Reduce(IRule rule, INodeContext<INode> page)
			{
				var type = page.Get("type").Single<Value>().Text ?? string.Empty;
				return new ProtoRule(nameof(TerminalType), (grammar, rule) => grammar.TerminalType(type, rule.RuleName));
			}
		}

		public class TerminalLiteral : IGrammarRule<INode>
		{
			public static string DefinitionText => "terminalLiteral  :=  T<Literal> | 'T' '<' T<Literal> #! '>'";

			public void Register(IGrammarBuilder<INode> grammar)
			{
				grammar.Choice("terminalLiteral",
					grammar.Terminal(TokenType.Literal),
					grammar.Sequence(null,
						grammar.TerminalValue("T"),
						grammar.TerminalValue("<"),
						grammar.Terminal(TokenType.Literal).Lift(),
						grammar.TerminalValue(">")
					)
				).ReduceWith(Reduce);
			}

			public INode Reduce(IRule rule, INodeContext<INode> page)
			{
				var value = ((Value)page.Get(0)).Text ?? string.Empty;
				return new ProtoRule(nameof(TerminalLiteral), (grammar, rule) => grammar.TerminalValue(value, rule.RuleName));
			}
		}

		public class TerminalAny : IGrammarRule<INode>
		{
			public static string DefinitionText => "terminalAny  :=  '.'";

			public void Register(IGrammarBuilder<INode> grammar)
			{
				grammar.TerminalValue(".", "terminalAny")
					.ReduceWith(Reduce);
			}

			public INode Reduce(IRule rule, INodeContext<INode> page)
			{
				return new ProtoRule(nameof(Core.Rules.AnyTerminal), (grammar, rule) => grammar.Any(rule.RuleName));
			}
		}

		public PegGrammar()
		{
			new Definition().Register(this);
			new Choice().Register(this);
			new Sequence().Register(this);
			new NamedAtom().Register(this);
			new AtomExpression().Register(this);
			new RangeDef().Register(this);
			new MinMaxDef().Register(this);
			new Atom().Register(this);
			new RuleRefDef().Register(this);
			new Terminal().Register(this);
			new TerminalType().Register(this);
			new TerminalLiteral().Register(this);
			new TerminalAny().Register(this);

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
	}
}
