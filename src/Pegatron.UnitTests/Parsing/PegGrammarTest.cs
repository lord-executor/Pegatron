using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using Pegatron.Grammars.Peg;
using Pegatron.Grammars.Peg.Ast;
using Pegatron.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pegatron.UnitTests.Parsing
{
	[TestFixture]
	public class PegGrammarTest
	{
		[Test]
		public void TEST()
		{
			var rule = Parse("definition        := T<IDENTIFIER>#name ':=' choice#rule");

			rule.Should().NotBeNull();
		}

		private INode Parse(string expression)
		{
			var lexer = new Lexer(new StringReader(expression));
			var grammar = new PegGrammar();
			var parser = new Parser<INode>(grammar);

			return parser.Parse(new TokenStream(lexer).Start());
		}
	}
}
