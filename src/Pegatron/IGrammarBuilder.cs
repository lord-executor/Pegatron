namespace Pegatron
{
	public interface IGrammarBuilder<TNode>
	{
		IRuleRef<TNode> DefineRule(string? name, IRule rule);
		IRuleRef<TNode> Ref(string name);
	}
}
