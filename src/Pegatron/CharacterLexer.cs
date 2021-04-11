using System;
using System.Collections.Generic;
using System.IO;

namespace Pegatron
{
	public class CharacterLexer : ILexer<Token>
	{
		private readonly TextReader _reader;

		public CharacterLexer(TextReader reader)
		{
			_reader = reader;
		}

		public CharacterLexer(string text)
			: this(new StringReader(text))
		{
		}

		public IEnumerable<Token> ReadTokens()
		{
			var line = _reader.ReadLine();
			uint lineCount = 0;
			int index = 0;

			while (line != null)
			{
				lineCount++;
				index = 0;
				while (index < line.Length)
				{
					var c = line[index].ToString();
					var token = new Token(c) {
						Value = c,
						Line = lineCount,
						Start = (uint)index++,
					};
					yield return token;
				}

				line = _reader.ReadLine();
				if (line != null)
				{
					yield return new Token(nameof(Environment.NewLine))
					{
						Value = "\n",
						Line = lineCount,
						Start = (uint)index++,
					};
				}
			}

			yield return Token.Eos;
		}
	}
}
