namespace Pegatron.Core
{
	public interface ITokenMatcher
	{
		string Name { get; }
		bool Match(IToken token);
	}
}
