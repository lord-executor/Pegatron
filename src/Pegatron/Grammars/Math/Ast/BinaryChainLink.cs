namespace Pegatron.Grammars.Math.Ast
{
    public class BinaryChainLink : Node
    {
        public Node Right { get; }
        public TokenNode Op { get; }

        public BinaryChainLink(TokenNode op, Node right)
        {
            Right = right;
            Op = op;
        }
    }
}
