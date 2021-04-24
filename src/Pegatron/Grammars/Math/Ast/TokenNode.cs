
namespace Pegatron.Grammars.Math.Ast
{
	public class TokenNode : Node
    {
        public IToken Token { get; }

        public TokenNode(IToken token)
        {
            Token = token;
        }
    }
}
