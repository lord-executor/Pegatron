namespace Pegatron.Grammars.Math.Ast
{
	public class Number : Node
    {
        public decimal Value { get; }

        public Number(decimal value)
        {
            Value = value;
        }
    }
}
