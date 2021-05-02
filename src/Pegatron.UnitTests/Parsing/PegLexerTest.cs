using FluentAssertions;
using NUnit.Framework;
using Pegatron.Grammars.Peg;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pegatron.UnitTests.Parsing
{
	[TestFixture]
	public class PegLexerTest
	{
		[Test]
		[TestCaseSource("SampleText")]
		public void ReadTokens_WithInputText_RecognizesTokensCorrectly(string text, Token[] expectedTokens)
		{
			var reader = new StringReader(text);
			var lexer = new Lexer(reader);

			var pairs = lexer.ReadTokens().Zip(expectedTokens).ToList();
			for (var i = 0; i < pairs.Count; i++)
			{
				pairs[i].First.Value.Should().Be(pairs[i].Second.Value);
				pairs[i].First.Type.Should().Be(pairs[i].Second.Type);
			}
		}

		private static IEnumerable<TestCaseData> SampleText()
		{
			yield return new TestCaseData("T<foo>", new Token[] {
				Token("T"), Token("<"), Token("foo", TokenType.Identifier),	Token(">"),	Pegatron.Token.Eos
			});

			yield return new TestCaseData("'Hell\\'0 World' 42 '42'", new Token[] {
				Token("'Hell'0 World'", TokenType.Literal), Token("42", TokenType.Number), Token("'42'", TokenType.Literal), Pegatron.Token.Eos
			});

			yield return new TestCaseData("expr    :=  atom (expr op)+ | atom", new Token[] {
				Token("expr", TokenType.Identifier), Token(":="), Token("atom", TokenType.Identifier), Token("("), Token("expr", TokenType.Identifier),
				Token("op", TokenType.Identifier), Token(")"), Token("+"), Token("|"), Token("atom", TokenType.Identifier), Pegatron.Token.Eos
			});
		}

		private static Token Token(string value, TokenType type = TokenType.Special)
		{
			return new Token(type.ToString())
			{
				Value = value
			};
		}
	}
}
