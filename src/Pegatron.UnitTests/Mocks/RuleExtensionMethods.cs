using Pegatron.Core;

namespace Pegatron.UnitTests.Mocks
{
	public static class RuleExtensionMethods
	{
		public static RuleOperationsMock OperationsMock(this TokenStreamIndex index)
		{
			return new RuleOperationsMock(index);
		}
	}
}
