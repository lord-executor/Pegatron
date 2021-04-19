using Pegatron.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pegatron
{
	public class CstNode
	{
		public string? Name { get; }
		public IEnumerable<CstNode> Children { get; }
		public string? Value { get; }

		public CstNode(string? name, IEnumerable<CstNode> children)
		{
			Name = name;
			Children = children;
		}

		public CstNode(string? name, string value)
		{
			Name = name;
			Value = value;
			Children = Enumerable.Empty<CstNode>();
		}

		public void Render(TextWriter writer, int indentLvl)
		{
			var indent = Enumerable.Repeat("  ", indentLvl).StrJoin("");
			if (Children == null)
			{
				writer.WriteLine($"{indent}{Name}({Value})");
			}
			else
			{
				if (Name != null)
				{
					writer.WriteLine($"{indent}{Name}");
				}
				foreach (var c in Children)
				{
					c.Render(writer, indentLvl + 1);
				}
			}
		}

		public override string ToString()
		{
			using (var writer = new StringWriter())
			{
				Render(writer, 0);
				return writer.ToString();
			}
		}
	}
}
