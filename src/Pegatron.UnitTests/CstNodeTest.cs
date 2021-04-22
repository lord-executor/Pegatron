using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using System;

namespace Pegatron.UnitTests
{
	[TestFixture]
	public class CstNodeTest
	{
		[Test]
		public void CstNode_ValueNode_ShouldHaveValueAndNoChildren()
		{
			var node = new CstNode("MyNode", "MyValue");

			node.Name.Should().Be("MyNode");
			node.Value.Should().Be("MyValue");
			node.Children.Should().NotBeNull();
			node.Children.Should().BeEmpty();

			node.ToString().Should().Be(new[] { "MyNode(MyValue)", "" }.StrJoin(Environment.NewLine));
		}

		[Test]
		public void CstNode_ContainerNode_ShouldHaveChildrenAndNoValue()
		{
			var node = new CstNode("MyNode", new[] {
				new CstNode("Child1", "Value1"),
				new CstNode("Child2", "Value2"),
			});

			node.Name.Should().Be("MyNode");
			node.Value.Should().BeNull();
			node.Children.Should().NotBeNull();
			node.Children.Should().HaveCount(2);

			node.ToString().Should().Be(new[] { "MyNode", "  Child1(Value1)", "  Child2(Value2)", "" }.StrJoin(Environment.NewLine));
		}
	}
}
