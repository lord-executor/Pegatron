using System;
using System.Collections.Generic;

namespace Pegatron.Core
{
	public class TokenStream
	{
		private readonly IEnumerator<IToken> _enumerator;
		private readonly IList<IToken> _buffer;
		private bool _isValid = true;

		public bool IsCompleted { get; private set; }

		public TokenStream(ILexer<IToken> lexer)
		{
			_enumerator = lexer.ReadTokens().GetEnumerator();
			_buffer = new List<IToken>();
			IsCompleted = false;
		}

		public IToken Get(int index)
		{
			while (index >= _buffer.Count && _isValid && (_isValid = _enumerator.MoveNext()))
			{
				_buffer.Add(_enumerator.Current);
			}

			if (!IsCompleted && !_isValid)
			{
				if (_buffer.Count == 0 || !_buffer[_buffer.Count - 1].IsEndOfStream)
				{
					throw new LexerException(LexerExceptionId.MissingEosToken);
				}
				IsCompleted = true;
			}

			if (index < _buffer.Count)
			{
				return _buffer[index];
			}
			else
			{
				throw new IndexOutOfRangeException("Trying to read past the end of the token stream");
			}
		}
	}
}
