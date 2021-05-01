using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using Pegatron.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegatron.UnitTests
{
	[TestFixture]
	public class NodeContextTest
	{
		[Test]
		public void Context_NewlyCreated_IsEmpty()
		{
			var context = new NodeContext<int>();

			context.Count.Should().Be(0);
		}

		[Test]
		public void AddNamed_OnEmptyContext_ContainsSingleEntry()
		{
			var context = new NodeContext<int>();
			context.Add("name", 42);

			context.Count.Should().Be(1);
			context.Get(0).Should().Be(42);
			context.Get("name").Single().Should().Be(42);
		}

		[Test]
		public void SingleResult_OnEmptySet_ThrowsException()
		{
			var context = new NodeContext<int>();

			context.Invoking(ctx => ctx.Get("name").Single()).Should().Throw<Exception>();
		}

		[Test]
		public void Cast_ToIncompatibleType_ThrowsException()
		{
			var context = new NodeContext<NodeType>();
			context.Add("name", new NodeType("fortytwo"));
			context.Add(null, new NodeSubType("sub"));

			context.Invoking(ctx => ctx.Get("name").Single<NodeSubType>()).Should().Throw<Exception>();
			context.Invoking(ctx => ctx.Get("name").Optional<NodeSubType>()).Should().Throw<Exception>();
			context.Invoking(ctx => ctx.GetAll().As<NodeSubType>().ToList()).Should().Throw<Exception>();
		}

		[Test]
		public void Cast_ToCompatibleType_ReturnsCorrectType()
		{
			var context = new NodeContext<NodeType>();
			context.Add("name", new NodeSubType("fortytwo"));
			context.Add("value", new NodeSubType("sixsixsix"));

			context.Get("name").Single<NodeSubType>().Name.Should().Be("fortytwo");
			context.Get("name").Optional<NodeSubType>()?.Name.Should().Be("fortytwo");
			context.Get("missing").Optional<NodeSubType>().Should().BeNull();
			context.GetAll().As<NodeSubType>().Select(n => n.Name).Should().ContainInOrder("fortytwo", "sixsixsix");
		}

		[Test]
		public void OfType_OnContextValue_FiltersObjectsOfType()
		{
			var context = new NodeContext<NodeType>();
			context.Add(null, new NodeType("fortytwo"));
			context.Add(null, new NodeSubType("sixsixsix"));

			context.GetAll().Of<NodeType>().Select(n => n.Name).Should().ContainInOrder("fortytwo", "sixsixsix");
			context.GetAll().Of<NodeSubType>().Select(n => n.Name).Should().ContainInOrder("sixsixsix");
			context.Get("none").Of<NodeSubType>().Should().BeEmpty();
		}

		[Test]
		public void AddMultiple_WithSameName_GetByNameReturnsAll()
		{
			var context = new NodeContext<int>();
			context.Add("name", 42);
			context.Add("name", 666);
			context.Add("name", 1024);

			context.Count.Should().Be(3);
			context.Get(0).Should().Be(42);
			context.Get(1).Should().Be(666);
			context.Get(2).Should().Be(1024);
			context.Get("name").Should().ContainInOrder(42, 666, 1024);
		}

		private class NodeType
		{
			public string Name { get; }

			public NodeType(string name)
			{
				Name = name;
			}
		}

		private class NodeSubType : NodeType
		{
			public NodeSubType(string name)
				: base(name)
			{
			}
		}
	}
}
