namespace Pegatron.Core
{
	public static class TokenStreamExtensions
	{
		public static TokenStreamIndex Start(this TokenStream stream)
		{
			return new TokenStreamIndex(stream, 0);
		}
	}
}
