using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegatron.Grammars.Math
{
	public enum TokenType
	{
		Identifier,
		Number,
		MultiplicativeOperator,
		AdditiveOperator,
		ParenLeft,
		ParenRight
	}
}
