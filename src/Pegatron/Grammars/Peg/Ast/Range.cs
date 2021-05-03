namespace Pegatron.Grammars.Peg.Ast
{
	public class Range : INode
	{
		public string DisplayText => $"{{{Min}, {Max}}}";
		public int Min { get; }
		public int Max { get; }

		public Range(int min, int max)
		{
			Min = min;
			Max = max;
		}
	}
}
