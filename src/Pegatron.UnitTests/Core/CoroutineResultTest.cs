using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using System;

namespace Pegatron.UnitTests
{
	[TestFixture]
	public class CoroutineResultTest
	{
		[Test]
		public void NewCoroutineResult_IsResolved_ShouldBeFalse()
		{
			var result = new CoroutineResult<string>();

			result.IsResolved.Should().BeFalse();
		}

		[Test]
		public void AccessValue_OfUnresolvedResult_ShouldThrow()
		{
			var result = new CoroutineResult<string>();

			result.Invoking(r => r.Value).Should().Throw<InvalidOperationException>();
		}

		[Test]
		public void Resolving_Result_ShouldTriggerOnResolveHook()
		{
			var resolveValue = "42";
			var onResolveCalled = false;
			var result = new CoroutineResult<string>();
			result.OnResolve += (value) => { onResolveCalled = true; value.Should().Be(resolveValue); };

			result.Resolve(resolveValue);
			onResolveCalled.Should().BeTrue();
			result.Value.Should().Be(resolveValue);
		}
	}
}
