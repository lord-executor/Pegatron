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

		public override string ToString()
		{
			using (var writer = new StringWriter())
			{
				var renderer = new CstNodeRenderer();
				renderer.Render(writer, this);
				return writer.ToString();
			}
		}
	}
}
