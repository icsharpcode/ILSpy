// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace LightJson.Serialization
{
	using System;
	using System.Globalization;
	using System.IO;
	using System.Text;

	using ErrorType = JsonParseException.ErrorType;

	/// <summary>
	/// Represents a reader that can read JsonValues.
	/// </summary>
	internal sealed class JsonReader
	{
		private TextScanner scanner;

		private JsonReader(TextReader reader)
		{
			this.scanner = new TextScanner(reader);
		}

		/// <summary>
		/// Creates a JsonValue by using the given TextReader.
		/// </summary>
		/// <param name="reader">The TextReader used to read a JSON message.</param>
		/// <returns>The parsed <see cref="JsonValue"/>.</returns>
		public static JsonValue Parse(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}

			return new JsonReader(reader).Parse();
		}

		/// <summary>
		/// Creates a JsonValue by reader the JSON message in the given string.
		/// </summary>
		/// <param name="source">The string containing the JSON message.</param>
		/// <returns>The parsed <see cref="JsonValue"/>.</returns>
		public static JsonValue Parse(string source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			using (var reader = new StringReader(source))
			{
				return Parse(reader);
			}
		}

		private string ReadJsonKey()
		{
			return this.ReadString();
		}

		private JsonValue ReadJsonValue()
		{
			this.scanner.SkipWhitespace();

			var next = this.scanner.Peek();

			if (char.IsNumber(next))
			{
				return this.ReadNumber();
			}

			switch (next)
			{
				case '{':
					return this.ReadObject();

				case '[':
					return this.ReadArray();

				case '"':
					return this.ReadString();

				case '-':
					return this.ReadNumber();

				case 't':
				case 'f':
					return this.ReadBoolean();

				case 'n':
					return this.ReadNull();

				default:
					throw new JsonParseException(
						ErrorType.InvalidOrUnexpectedCharacter,
						this.scanner.Position);
			}
		}

		private JsonValue ReadNull()
		{
			this.scanner.Assert("null");
			return JsonValue.Null;
		}

		private JsonValue ReadBoolean()
		{
			switch (this.scanner.Peek())
			{
				case 't':
					this.scanner.Assert("true");
					return true;

				default:
					this.scanner.Assert("false");
					return false;
			}
		}

		private void ReadDigits(StringBuilder builder)
		{
			while (true)
			{
				int next = this.scanner.Peek(throwAtEndOfFile: false);
				if (next == -1 || !char.IsNumber((char)next))
				{
					return;
				}

				builder.Append(this.scanner.Read());
			}
		}

		private JsonValue ReadNumber()
		{
			var builder = new StringBuilder();

			if (this.scanner.Peek() == '-')
			{
				builder.Append(this.scanner.Read());
			}

			if (this.scanner.Peek() == '0')
			{
				builder.Append(this.scanner.Read());
			}
			else
			{
				this.ReadDigits(builder);
			}

			if (this.scanner.Peek(throwAtEndOfFile: false) == '.')
			{
				builder.Append(this.scanner.Read());
				this.ReadDigits(builder);
			}

			if (this.scanner.Peek(throwAtEndOfFile: false) == 'e' || this.scanner.Peek(throwAtEndOfFile: false) == 'E')
			{
				builder.Append(this.scanner.Read());

				var next = this.scanner.Peek();

				switch (next)
				{
					case '+':
					case '-':
						builder.Append(this.scanner.Read());
						break;
				}

				this.ReadDigits(builder);
			}

			return double.Parse(
				builder.ToString(),
				CultureInfo.InvariantCulture);
		}

		private string ReadString()
		{
			var builder = new StringBuilder();

			this.scanner.Assert('"');

			while (true)
			{
				var errorPosition = this.scanner.Position;
				var c = this.scanner.Read();

				if (c == '\\')
				{
					errorPosition = this.scanner.Position;
					c = this.scanner.Read();

					switch (char.ToLower(c))
					{
						case '"':
						case '\\':
						case '/':
							builder.Append(c);
							break;
						case 'b':
							builder.Append('\b');
							break;
						case 'f':
							builder.Append('\f');
							break;
						case 'n':
							builder.Append('\n');
							break;
						case 'r':
							builder.Append('\r');
							break;
						case 't':
							builder.Append('\t');
							break;
						case 'u':
							builder.Append(this.ReadUnicodeLiteral());
							break;
						default:
							throw new JsonParseException(
								ErrorType.InvalidOrUnexpectedCharacter,
								errorPosition);
					}
				}
				else if (c == '"')
				{
					break;
				}
				else
				{
					if (char.IsControl(c))
					{
						throw new JsonParseException(
							ErrorType.InvalidOrUnexpectedCharacter,
							errorPosition);
					}
					else
					{
						builder.Append(c);
					}
				}
			}

			return builder.ToString();
		}

		private int ReadHexDigit()
		{
			var errorPosition = this.scanner.Position;
			switch (char.ToUpper(this.scanner.Read()))
			{
				case '0':
					return 0;

				case '1':
					return 1;

				case '2':
					return 2;

				case '3':
					return 3;

				case '4':
					return 4;

				case '5':
					return 5;

				case '6':
					return 6;

				case '7':
					return 7;

				case '8':
					return 8;

				case '9':
					return 9;

				case 'A':
					return 10;

				case 'B':
					return 11;

				case 'C':
					return 12;

				case 'D':
					return 13;

				case 'E':
					return 14;

				case 'F':
					return 15;

				default:
					throw new JsonParseException(
						ErrorType.InvalidOrUnexpectedCharacter,
						errorPosition);
			}
		}

		private char ReadUnicodeLiteral()
		{
			int value = 0;

			value += this.ReadHexDigit() * 4096; // 16^3
			value += this.ReadHexDigit() * 256;  // 16^2
			value += this.ReadHexDigit() * 16;   // 16^1
			value += this.ReadHexDigit();        // 16^0

			return (char)value;
		}

		private JsonObject ReadObject()
		{
			return this.ReadObject(new JsonObject());
		}

		private JsonObject ReadObject(JsonObject jsonObject)
		{
			this.scanner.Assert('{');

			this.scanner.SkipWhitespace();

			if (this.scanner.Peek() == '}')
			{
				this.scanner.Read();
			}
			else
			{
				while (true)
				{
					this.scanner.SkipWhitespace();

					var errorPosition = this.scanner.Position;
					var key = this.ReadJsonKey();

					if (jsonObject.ContainsKey(key))
					{
						throw new JsonParseException(
							ErrorType.DuplicateObjectKeys,
							errorPosition);
					}

					this.scanner.SkipWhitespace();

					this.scanner.Assert(':');

					this.scanner.SkipWhitespace();

					var value = this.ReadJsonValue();

					jsonObject.Add(key, value);

					this.scanner.SkipWhitespace();

					errorPosition = this.scanner.Position;
					var next = this.scanner.Read();
					if (next == ',')
					{
						// Allow trailing commas in objects
						this.scanner.SkipWhitespace();
						if (this.scanner.Peek() == '}')
						{
							next = this.scanner.Read();
						}
					}

					if (next == '}')
					{
						break;
					}
					else if (next == ',')
					{
						continue;
					}
					else
					{
						throw new JsonParseException(
							ErrorType.InvalidOrUnexpectedCharacter,
							errorPosition);
					}
				}
			}

			return jsonObject;
		}

		private JsonArray ReadArray()
		{
			return this.ReadArray(new JsonArray());
		}

		private JsonArray ReadArray(JsonArray jsonArray)
		{
			this.scanner.Assert('[');

			this.scanner.SkipWhitespace();

			if (this.scanner.Peek() == ']')
			{
				this.scanner.Read();
			}
			else
			{
				while (true)
				{
					this.scanner.SkipWhitespace();

					var value = this.ReadJsonValue();

					jsonArray.Add(value);

					this.scanner.SkipWhitespace();

					var errorPosition = this.scanner.Position;
					var next = this.scanner.Read();
					if (next == ',')
					{
						// Allow trailing commas in arrays
						this.scanner.SkipWhitespace();
						if (this.scanner.Peek() == ']')
						{
							next = this.scanner.Read();
						}
					}

					if (next == ']')
					{
						break;
					}
					else if (next == ',')
					{
						continue;
					}
					else
					{
						throw new JsonParseException(
							ErrorType.InvalidOrUnexpectedCharacter,
							errorPosition);
					}
				}
			}

			return jsonArray;
		}

		private JsonValue Parse()
		{
			this.scanner.SkipWhitespace();
			return this.ReadJsonValue();
		}
	}
}
