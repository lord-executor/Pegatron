namespace Pegatron
{
	public interface IGrammarRule<TNode>
	{
		void Register(IGrammarBuilder<TNode> grammar);
	}
}
