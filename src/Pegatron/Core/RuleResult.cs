namespace Pegatron.Core
{
	public class RuleResult
	{
		public bool IsSuccess { get; }
		public TokenStreamIndex Index { get; }
		public int StepCount { get; }
		public TokenStreamSpan Match { get; }

		public RuleResult(bool success, TokenStreamIndex index, int stepCount, TokenStreamSpan match)
		{
			IsSuccess = success;
			Index = index;
			StepCount = stepCount;
			Match = match;
		}
	}
}
