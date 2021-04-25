using Pegatron.Grammars.Math.Ast;
using System.Linq;

namespace Pegatron.Grammars.Math
{
	public class InfixRenderer : IVisitor<string>
	{
		public InfixRenderer()
		{
		}

		public string BinaryChain(BinaryChain binaryChain)
		{
			var head = this.Visit(binaryChain.Head);

			return binaryChain.Tail.Aggregate(head, (previous, item) => $"({previous} {item.op} {this.Visit(item.rhs)})");
		}

		public string Number(Number number)
		{
			return number.Value.ToString();
		}

		public string Variable(Variable variable)
		{
			return variable.VariableName;
		}
	}
}
