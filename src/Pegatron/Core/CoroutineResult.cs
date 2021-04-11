using System;

namespace Pegatron.Core
{
	/// <summary>
	/// This is effectively a <see cref="Lazy{T}"/> with some added functionality to side-channel the
	/// result when it is resolved
	/// </summary>
	public class CoroutineResult<T>
	{
		private T? _value;

		public bool IsResolved { get; private set; }
		public T Value => IsResolved ? _value! : throw new InvalidOperationException("Trying to access unresolved CoroutineResult value");

		public event Action<T>? OnResolve;

		public void Resolve(T value)
		{
			_value = value;
			IsResolved = true;
			OnResolve?.Invoke(value);
		}
	}
}
