namespace Pegatron.Grammars.Peg.Ast
{
	public class Value : INode
	{
		public string DisplayText => Text;
		public string Text { get; }

		public Value(string text)
		{
			Text = text;
		}
	}
}
