using Pegatron.Core;
using Pegatron.Grammars.Math.Ast;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegatron.Grammars.Math
{
	public class RpnGrammar : Grammar<Node>
	{
		public RpnGrammar()
		{
			// atom    :=  IDENTIFIER | NUMBER
			this.Choice("atom",
				this.Terminal(TokenType.Identifier)
					.ReduceWith(Atom(tokenNode => new Variable(tokenNode.Token.Value!))),
				this.Terminal(TokenType.Number)
					.ReduceWith(Atom(tokenNode => new Number(decimal.Parse(tokenNode.Token.Value!))))
			);

			// op      :=  ('+' | '-' | '*' | '/')
			this.Choice("op",
				this.Terminal(TokenType.AdditiveOperator),
				this.Terminal(TokenType.MultiplicativeOperator)
			);

			// expr    :=  atom (expr op)+ | atom
			this.Choice("expr",
				this.Sequence(null,
					Ref("atom").As("head"),
					this.OneOrMore("pushop",
						this.Sequence(null,
							Ref("expr").As("value"),
							Ref("op").As("op")
						).ReduceWith(ChainLink)
					).As("body")).ReduceWith(Chain),
				Ref("atom")
			);

			StartWith("expr");
		}

		public override IEnumerable<Node> DefaultReducer(IRule rule, INodeContext<Node> page)
		{
			if (page.HasLift())
			{
				return page.GetLift();
			}

			if (rule.RuleType == RuleType.SingleMatch)
			{
				return page.GetAll();
			}

			return EnumSequence.Of(new CollectionNode(page.GetAll()));
		}

		public override IEnumerable<Node> TerminalReducer(IRule rule, IToken token)
		{
			return EnumSequence.Of(new TokenNode(token));
		}

		#region Reducers

		private Node Chain(IRule rule, INodeContext<Node> page)
		{
			var head = page.Get("head").Single();
			var body = page.Get("body").Single<CollectionNode>();

			return new BinaryChain(head, body.Nodes.OfType<BinaryChainLink>());
		}

		private Node ChainLink(IRule rule, INodeContext<Node> page)
		{
			var op = page.Get("op").Single<TokenNode>();
			var value = page.Get("value").Single();
			return new BinaryChainLink(op, value);
		}

		private SingleReducer<Node> Atom(Func<TokenNode, Node> factory)
		{
			return (rule, page) =>
			{
				var tokenNode = (TokenNode)page.Get(0);
				var node = factory(tokenNode);
				return node;
			};
		}

		#endregion
	}
}
