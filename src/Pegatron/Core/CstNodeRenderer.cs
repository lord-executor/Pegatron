using System.IO;
using System.Linq;

namespace Pegatron.Core
{
	public class CstNodeRenderer
	{
		public void Render(TextWriter writer, CstNode node)
		{
			Render(writer, node, 0);
		}

		public void Render(TextWriter writer, CstNode node, int indentLvl)
		{
			var indent = Enumerable.Repeat("  ", indentLvl).StrJoin("");
			if (!node.Children.Any())
			{
				writer.WriteLine($"{indent}{node.Name}({node.Value})");
			}
			else
			{
				if (node.Name != null)
				{
					writer.WriteLine($"{indent}{node.Name}");
				}
				foreach (var c in node.Children)
				{
					Render(writer, c, indentLvl + 1);
				}
			}
		}
	}
}
