using Pegatron.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Pegatron
{
	public class Parser<TNode>
	{
		private readonly IGrammar<TNode> _grammar;
		private Stack<RuleState<TNode>> _ruleStack = new Stack<RuleState<TNode>>();
		public static IDebugHooks<TNode> DebugHooks { get; set; } = new NullDebugger<TNode>();
		private RuleState<TNode> Current => _ruleStack.Peek();

		private (RuleState<TNode> State, RuleResult Result)? _longestMatch;

		public Parser(IGrammar<TNode> grammar)
		{
			_grammar = grammar;
		}

		public TNode Parse(TokenStreamIndex index)
		{
			_ruleStack = new Stack<RuleState<TNode>>();
			return Run(_grammar.Start(), index);
		}

		private TNode Run(IRuleRef startRule, TokenStreamIndex index)
		{
			var coroutines = new ParserCoroutines(this);

			// this is essentially a dummy state that serves ad the receiver of the reduced "root" rule node
			var nullRule = new NullRule(startRule);
			var startState = new RuleState<TNode>(nullRule, index, coroutines, parent: null);
			_ruleStack.Push(startState);

			ProcessStack();

			// finish the iterator of the NullRule
			startState.Iterator.MoveNext();

			if (nullRule.Result == null)
			{
				throw new ParserException(ParserExceptionId.StartRuleMissingResult);
			}
			if (!nullRule.Result.IsSuccess)
			{
				throw new ParserException(ParserExceptionId.ParsingFailed, CreateParserErrorMessage());
			}
			if (!nullRule.Result.Index.Get().IsEndOfStream)
			{
				throw new ParserException(ParserExceptionId.PartialMatch, nullRule.Result.Index.Index, CreateParserErrorMessage());
			}
			if (startState.NodeContext.Count > 1)
			{
				throw new ParserException(ParserExceptionId.ReducerError, startState.NodeContext.Count);
			}

			return startState.NodeContext.Get(0);
		}

		private RuleState<TNode> ProcessStack()
		{
			// The first and only operation of the NullRule is to call the actual root rule of the grammar, so after the
			// first iteration we are guaranteed to only arrive at a stack count of 1 again after the root rule has been
			// completed.
			do
			{
				var state = Current;
				if (state.Iterator.MoveNext())
				{
					// call next rule operation
					state.Iterator.Current();
				}
				else
				{
					_ruleStack.Pop();
					if (!state.Result.IsResolved)
					{
						throw new ParserException(ParserExceptionId.MissingResult, state.Rule.ToDisplayText(DisplayMode.Long));
					}

					var result = state.Result.Value;
					DebugHooks.OnAfterCall(state, state.Rule, result.Index, result);

					if (result.IsSuccess)
					{
						if (!_longestMatch.HasValue || _longestMatch.Value.Result.Index.Index <= result.Index.Index)
						{
							_longestMatch = (state, result);
						}
						Current.NodeContext.Add(state.Rule.RefName, (state.Rule.Reducer ?? _grammar.DefaultReducer)(state.Rule, state.NodeContext));
					}
				}
			} while (_ruleStack.Count > 1);

			// Pops the last remaining state - of the NullRule
			return _ruleStack.Pop();
		}

		private StringBuilder CreateParserErrorMessage()
		{
			var sb = new StringBuilder();
			if (_longestMatch.HasValue)
			{
				var state = _longestMatch.Value.State;
				var result = _longestMatch.Value.Result;
				var token = result.Index.Get();
				sb.AppendLine($"Longest match at index {result.Index.Index} with rule {state.Rule.ToDisplayText(DisplayMode.Long)}");
				sb.AppendLine($"Line: {token.Line}, Position: {token.Start}, Value: {token.Value}");
				sb.AppendLine($"Stack:");

				var debugView = new RuleState<TNode>.RuleStateDebugView(state);
				var count = 0;
				foreach (var entry in debugView.StackTrace)
				{
					sb.AppendLine($"{count++,2}: {entry.Rule}");
					sb.AppendLine($"    {entry.Stream}");
				}
			}
			return sb;
		}

		public class ParserCoroutines : IRuleOperations
		{
			private readonly Parser<TNode> _parser;

			public ParserCoroutines(Parser<TNode> parser)
			{
				_parser = parser;
			}

			public RuleOperation Call(IRuleRef rule, TokenStreamIndex index, out CoroutineResult<RuleResult> result)
			{
				var state = new RuleState<TNode>(rule, index, this, _parser._ruleStack.Peek());
				result = state.Result;

				return () =>
				{
					DebugHooks.OnBeforeCall(state, rule, index);
					_parser._ruleStack.Push(state);
				};
			}

			public RuleOperation Complete(IRuleRef rule, RuleResult result)
			{
				return () =>
				{
					_parser.Current.Result.Resolve(result);
				};
			}

			public RuleOperation Token(IRuleRef rule, IToken token)
			{
				return () =>
				{
					DebugHooks.OnToken(_parser.Current, token);
					_parser.Current.NodeContext.Add(rule.RefName, _parser._grammar.TerminalReducer(rule, token));
				};
			}
		}

		private class NullRule : IRuleRef<TNode>
		{
			private readonly IRuleRef _root;

			[ExcludeFromCodeCoverage]
			public Reducer<TNode> Reducer => (_, _) => throw new NotImplementedException();
			public string Name => "NULL";
			public RuleType RuleType => RuleType.SingleMatch;
			public string? RefName => null;
			public RuleResult? Result { get; private set; }

			public NullRule(IRuleRef root)
			{
				_root = root;
			}

			[ExcludeFromCodeCoverage]
			public IRuleRef<TNode> As(string refName)
			{
				throw new NotImplementedException();
			}

			[ExcludeFromCodeCoverage]
			IRuleRef IRuleRef.As(string refName)
			{
				return As(refName);
			}

			public IEnumerable<RuleOperation> Grab(IRuleContext ctx)
			{
				yield return ctx.Call(_root, ctx.Index, out var result);
				Result = result.Value;
				yield break;
			}

			[ExcludeFromCodeCoverage]
			public IRuleRef<TNode> ReduceWith(Reducer<TNode> reducer)
			{
				throw new NotImplementedException();
			}

			public string DisplayText(DisplayMode mode)
			{
				return Name;
			}
		}
	}
}
