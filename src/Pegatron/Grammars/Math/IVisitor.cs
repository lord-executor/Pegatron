using Pegatron.Grammars.Math.Ast;
using System;

namespace Pegatron.Grammars.Math
{
	public interface IVisitor<TResult>
	{
		TResult BinaryChain(BinaryChain binaryChain);
		TResult Number(Number number);
		TResult Variable(Variable variable);
	}

	public static class VisitorExtensions
	{
		public static TResult Visit<TResult>(this IVisitor<TResult> visitor, Node node)
		{
			switch (node)
			{
				case BinaryChain binaryChain:
					return visitor.BinaryChain(binaryChain);

				case Number number:
					return visitor.Number(number);

				case Variable variable:
					return visitor.Variable(variable);

				default:
					throw new ArgumentException($"Unexpected node type {node.GetType().Name} in evaluation tree");
			}
		}
	}
}
