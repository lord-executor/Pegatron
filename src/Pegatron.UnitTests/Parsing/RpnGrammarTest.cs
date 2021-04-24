using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using Pegatron.Grammars.Math;
using Pegatron.Grammars.Math.Ast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pegatron.UnitTests.Parsing
{
	[TestFixture]
	public class RpnGrammarTest
	{
		private static decimal _accuracy = 0.000001M;

		[Test]
		[TestCase("1 1 +", 2)]
		[TestCase("1 5 /", 0.2)]
		[TestCase("1.1 2 3 + *", 5.5)]
		[TestCase("42 6 / 3 + 8 *", 80)]
		[TestCase("3 3.5 2 * *\n6.65 8.33 6.02 + + +", 42)]
		public void Evaluate_SimpleNumericExpressions_ReturnsCorrectResult(string expression, decimal expectedResult)
		{
			var root = Parse(expression);
			var visitor = new Evaluator(EvaluationContext.Empty);

			var result = visitor.Visit(root);

			result.Should().BeApproximately(expectedResult, _accuracy);
		}

		[Test]
		[TestCase("foobar", 42)]
		[TestCase("foobar factor /", 10.5)]
		[TestCase("pi pi pi pi + + +", 4 * Math.PI)]
		[TestCase("test factor / foobar 100 foobar - + *", 305)]
		public void Evaluate_ExpressionsWithVariables_ReturnsCorrectResult(string expression, decimal expectedResult)
		{
			var root = Parse(expression);
			var visitor = new Evaluator(GetEvalContext());

			var result = visitor.Visit(root);

			result.Should().BeApproximately(expectedResult, _accuracy);
		}

		[Test]
		[TestCase("5 te.st")]
		[TestCase("2 2 %")]
		[TestCase("_42")]
		public void Parse_WithInvalidExpressionText_ThrowsLexerException(string expression)
		{
			Action action = () => Parse(expression);

			action.Should().Throw<LexerException>().Where(e => e.Id == LexerExceptionId.UnrecognizedInput);
		}

		[Test]
		[TestCase("5 (4 5 -) *")]
		[TestCase("1 1 )")]
		public void Parse_WithUnsupportedTokens_ThrowsParserException(string expression)
		{
			Action action = () => Parse(expression);

			action.Should().Throw<ParserException>().Where(e => e.Id == ParserExceptionId.PartialMatch);
		}

		[Test]
		public void Evaluate_WithUndefinedVariable_ThrowsException()
		{
			var root = Parse("5 x *");
			var visitor = new Evaluator(GetEvalContext());

			visitor.Invoking(v => v.Visit(root)).Should().Throw<Exception>();
		}

		[Test]
		public void Evaluate_OfIntermediateNode_ThrowsException()
		{
			var visitor = new Evaluator(GetEvalContext());

			visitor.Invoking(v => v.Visit(new CollectionNode(Enumerable.Empty<Node>()))).Should().Throw<ArgumentException>();
		}

		private Node Parse(string expression)
		{
			var lexer = new Lexer(new StringReader(expression));
			var grammar = new RpnGrammar();
			var parser = new Parser<Node>(grammar);

			return parser.Parse(new TokenStream(lexer).Start());
		}

		private EvaluationContext GetEvalContext()
		{
			return new EvaluationContext(new Dictionary<string, decimal>
			{
				["foobar"] = 42M,
				["test"] = 12.2M,
				["factor"] = 4M,
				["pi"] = (decimal)Math.PI
			});
		}
	}
}
