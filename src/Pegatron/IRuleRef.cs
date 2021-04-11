namespace Pegatron
{
	public interface IRuleRef : IRule
	{
		string RefName { get; }
	}

	public interface IRuleRef<TNode> : IRuleRef
	{
		Reducer<TNode> Reducer { get; }
		IRuleRef<TNode> As(string refName);
		IRuleRef<TNode> ReduceWith(Reducer<TNode> reducer);
	}
}
