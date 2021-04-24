using Pegatron.Grammars.Math.Ast;
using System;
using System.Collections.Generic;

namespace Pegatron.Grammars.Math
{
	public class Evaluator : IVisitor<decimal>
	{
		private static IDictionary<string, Func<decimal, decimal, decimal>> _operators = new Dictionary<string, Func<decimal, decimal, decimal>>
		{
			["+"] = (a, b) => a + b,
			["-"] = (a, b) => a - b,
			["*"] = (a, b) => a * b,
			["/"] = (a, b) => a / b,
		};

		private readonly EvaluationContext _context;

		public Evaluator(EvaluationContext context)
		{
			_context = context;
		}

		public decimal BinaryChain(BinaryChain binaryChain)
		{
			var accumulator = this.Visit(binaryChain.Head);
			foreach (var item in binaryChain.Tail)
			{
				accumulator = _operators[item.op](accumulator, this.Visit(item.rhs));
			}
			return accumulator;
		}

		public decimal Number(Number number)
		{
			return number.Value;
		}

		public decimal Variable(Variable variable)
		{
			return _context.Get(variable.VariableName);
		}
	}
}
