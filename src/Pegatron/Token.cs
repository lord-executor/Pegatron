namespace Pegatron
{
	public class Token : IToken
	{
		public string Type { get; }

		public string? Value { get; init; }

		public uint Line { get; init; }

		public uint Start { get; init; }

		public uint Length => (uint)(Value ?? string.Empty).Length;

		public bool IsEndOfStream { get; }

		public Token(string tokenType)
		{
			Type = tokenType;
		}

		private Token()
		{
			Type = "EOS";
			IsEndOfStream = true;
		}

		public static Token Eos { get; } = new Token();
	}
}
