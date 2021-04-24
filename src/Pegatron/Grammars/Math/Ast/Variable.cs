namespace Pegatron.Grammars.Math.Ast
{
	public class Variable : Node
    {
        public string VariableName { get; }

        public Variable(string variableName)
        {
			VariableName = variableName;
        }
    }
}
