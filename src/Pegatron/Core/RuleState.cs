using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Pegatron.Core
{
	/// <summary>
	/// This is an implementation detail of the <see cref="Parser"/> and should not be used by any
	/// other class than the <see cref="Parser"/> and <see cref="IDebugHooks{TNode}"/>.
	/// </summary>
	[DebuggerTypeProxy(typeof(RuleState<>.RuleStateDebugView))]
	public class RuleState<TNode>
	{
		public IRuleRef<TNode> Rule { get; }
		public IEnumerator<RuleOperation> Iterator { get; }
		public CoroutineResult<RuleResult> Result { get; set; }
		public NodeContext<TNode> NodeContext { get; }
		public IRuleContext RuleContext { get; }
		public RuleState<TNode>? Parent { get; }

		internal RuleState(IRuleRef rule, TokenStreamIndex index, IRuleOperations coroutines, RuleState<TNode>? parent)
		{
			Rule = (IRuleRef<TNode>)rule;
			Result = new CoroutineResult<RuleResult>();
			NodeContext = new NodeContext<TNode>();
			Parent = parent;
			RuleContext = new RuleContext(Rule, index, coroutines);
			Iterator = rule.Grab(RuleContext).GetEnumerator();
		}

		[ExcludeFromCodeCoverage]
		internal class RuleStateDebugView
		{
			private readonly RuleState<TNode> _state;

			public NodeContext<TNode> NodeContext => _state.NodeContext;

			public RuleResult? Result => _state.Result.IsResolved ? _state.Result.Value : null;

			public TokenStreamIndex Index => _state.RuleContext.Index;


			public IEnumerable<StackEntry> StackTrace
			{
				get
				{
					return GetStack().Select(s => new StackEntry(s)).ToList();
				}
			}

			public RuleStateDebugView(RuleState<TNode> state)
			{
				_state = state;
			}

			private IEnumerable<RuleState<TNode>> GetStack()
			{
				var current = _state;
				while (current != null)
				{
					yield return current;
					current = current.Parent;
				}
			}
		}

		[ExcludeFromCodeCoverage]
		internal class StackEntry
		{
			public RuleState<TNode> State { get; }
			public string Rule { get; }
			public string Stream { get; }

			public StackEntry(RuleState<TNode> state)
			{
				State = state;
				Rule = state.Rule.ToDisplayText(DisplayMode.Definition);

				var endIndex = state.Result.IsResolved ? state.Result.Value.Index : state.RuleContext.Index;
				Stream = state.RuleContext.Index.Until(endIndex)
					.Select(t => FormatToken(t))
					.Concat(EnumSequence.Of("=>", FormatToken(endIndex.Get())))
					.StrJoin(" ");
			}

			private string FormatToken(IToken token)
			{
				return $"{token.Type}({token.Value})";
			}
		}
	}
}
