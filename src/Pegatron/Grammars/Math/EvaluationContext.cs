using System;
using System.Collections.Generic;

namespace Pegatron.Grammars.Math
{
	public class EvaluationContext
	{
		public static EvaluationContext Empty { get; } = new EvaluationContext(new Dictionary<string, decimal>());

		private readonly IDictionary<string, decimal> _variables;

		public EvaluationContext(IDictionary<string, decimal> variables)
		{
			_variables = variables;
		}

		public decimal Get(string variableName)
		{
			if (!_variables.ContainsKey(variableName))
			{
				throw new Exception($"Variable '{variableName}' is undefined");
			}
			return _variables[variableName];
		}
	}
}
