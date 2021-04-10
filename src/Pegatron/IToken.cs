namespace Pegatron
{
	public interface IToken
	{
		public string Type { get; }
		public string? Value { get; }
		public uint Line { get; }
		public uint Start { get; }
		public uint Length { get; }
		public bool IsEndOfStream { get; }
	}
}
