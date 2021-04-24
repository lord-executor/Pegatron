using Pegatron.Grammars.Math.Ast;
using System;
using System.Linq;

namespace Pegatron.Grammars.Math
{
	public class RpnGrammar : Grammar<Node>
	{
		public RpnGrammar()
		{
			// atom    :=  IDENTIFIER | NUMBER
			this.Choice("atom",
				this.Terminal(TokenType.Identifier.ToString())
					.ReduceWith(Atom(tokenNode => new Variable(tokenNode.Token.Value!))),
				this.Terminal(TokenType.Number.ToString())
					.ReduceWith(Atom(tokenNode => new Number(decimal.Parse(tokenNode.Token.Value!))))
			);

			// op      :=  ('+' | '-' | '*' | '/')
			this.Choice("op",
				this.Terminal(TokenType.AdditiveOperator.ToString()),
				this.Terminal(TokenType.MultiplicativeOperator.ToString())
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
					).As("body").ReduceWith(Collection)).ReduceWith(Chain),
				Ref("atom")
			);

			StartWith("expr");
		}

		public override Node DefaultReducer(IRule rule, INodeContext<Node> page)
		{
			if (page.Has("main") || page.Count == 1)
			{
				var node = page.MaybeGet("main") ?? page.Get(0);
				return node;
			}
			else
			{
				return Collection(rule, page);
			}
		}

		public override Node TokenNodeFactory(IRule rule, IToken token)
		{
			return new TokenNode(token);
		}

		#region Reducers

		public Node Collection(IRule rule, INodeContext<Node> page)
		{
			return new CollectionNode(page.GetAll());
		}

		private Node Chain(IRule rule, INodeContext<Node> page)
		{
			var head = page.Get("head");
			var body = (CollectionNode)page.Get("body");

			return new BinaryChain(head, body.Nodes.OfType<BinaryChainLink>());
		}

		private Node ChainLink(IRule rule, INodeContext<Node> page)
		{
			var op = (TokenNode)page.Get("op");
			var value = page.Get("value");
			return new BinaryChainLink(op, value);
		}

		private Reducer<Node> Atom(Func<TokenNode, Node> factory)
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
