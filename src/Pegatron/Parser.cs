using Pegatron.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pegatron
{
	public class Parser<TNode>
	{
		private readonly IGrammar<TNode> _grammar;
		private readonly Stack<RuleState<TNode>> _ruleStack = new Stack<RuleState<TNode>>();
		public static IDebugHooks<TNode> DebugHooks { get; set; } = new NullDebugger<TNode>();
		private RuleState<TNode> Current => _ruleStack.Peek();

		public Parser(IGrammar<TNode> grammar)
		{
			_grammar = grammar;
		}

		public TNode Parse(TokenStreamIndex index)
		{
			return Run(_grammar.Start(), index);
		}

		private TNode Run(IRuleRef startRule, TokenStreamIndex index)
		{
			var coroutines = new ParserCoroutines(this);

			// this is essentially a dummy state that serves ad the receiver of the reduced "root" rule node
			var nullRule = new NullRule(startRule);
			var startState = new RuleState<TNode>(nullRule, index, coroutines);
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
				throw new ParserException(ParserExceptionId.ParsingFailed);
			}
			if (!nullRule.Result.Index.Get().IsEndOfStream)
			{
				throw new ParserException(ParserExceptionId.PartialMatch, nullRule.Result.Index.Index);
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
						throw new ParserException(ParserExceptionId.MissingResult, state.Rule.DisplayText);
					}

					var result = state.Result.Value;
					DebugHooks.OnAfterCall(state, state.Rule, result.Index, result);

					if (result.IsSuccess)
					{
						Current.NodeContext.Add(state.Rule.RefName, (state.Rule.Reducer ?? _grammar.DefaultReducer)(state.Rule, state.NodeContext));
					}
				}
			} while (_ruleStack.Count > 1);

			// Pops the last remaining state - of the NullRule
			return _ruleStack.Pop();
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
				var state = new RuleState<TNode>(rule, index, this);
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
					_parser.Current.NodeContext.Add(rule.RefName, _parser._grammar.TokenNodeFactory(rule, token));
				};
			}
		}

		private class NullRule : IRuleRef<TNode>
		{
			private readonly IRuleRef _root;

			public Reducer<TNode> Reducer => (_, _) => throw new NotImplementedException();
			public string Name => "NULL";
			public RuleType RuleType => RuleType.SingleMatch;
			public string? RefName => null;
			public RuleResult? Result { get; private set; }

			public NullRule(IRuleRef root)
			{
				_root = root;
			}

			public IRuleRef<TNode> As(string refName)
			{
				throw new NotImplementedException();
			}

			public IEnumerable<RuleOperation> Grab(IRuleContext ctx)
			{
				yield return ctx.Call(_root, ctx.Index, out var result);
				Result = result.Value;
				yield break;
			}

			public IRuleRef<TNode> ReduceWith(Reducer<TNode> reducer)
			{
				throw new NotImplementedException();
			}
		}
	}
}
