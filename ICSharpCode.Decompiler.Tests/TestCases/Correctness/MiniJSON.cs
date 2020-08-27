using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	// taken from https://github.com/Jackyjjc/MiniJSON.cs
	// Copyright (c) 2013 Calvin Rien.
	// Licensed under the MIT LICENSE.
	public class MiniJSONTest
	{
		public static void Main(string[] args)
		{
			var jsonString = "{ \"array\": [1.44,2,3], " +
							"\"object\": {\"key1\":\"value1\", \"key2\":256}, " +
							"\"string\": \"The quick brown fox \\\"jumps\\\" over the lazy dog \", " +
							"\"unicode\": \"\\u3041 Men\u00fa sesi\u00f3n\", " +
							"\"int\": 65536, " +
							"\"float\": 3.1415926, " +
							"\"bool\": true, " +
							"\"null\": null }";

			var dict = Json.Deserialize(jsonString) as Dictionary<string, object>;

			Console.WriteLine("deserialized: " + dict.GetType());
			Console.WriteLine("dict['array'][0]: " + ((List<object>)dict["array"])[0]);
			Console.WriteLine("dict['string']: " + (string)dict["string"]);
			Console.WriteLine("dict['float']: " + (double)dict["float"]); // floats come out as doubles
			Console.WriteLine("dict['int']: " + (long)dict["int"]); // ints come out as longs
			Console.WriteLine("dict['unicode']: " + (string)dict["unicode"]);

			var str = Json.Serialize(dict);

			Console.WriteLine("serialized: " + str);
		}
	}

	public static class Json
	{
		/// <summary>
		/// Parses the string json into a value
		/// </summary>
		/// <param name="json">A JSON string.</param>
		/// <returns>An List&lt;object&gt;, a Dictionary&lt;string, object&gt;, a double, an integer,a string, null, true, or false</returns>
		public static object Deserialize(string json)
		{
			// save the string for debug information
			if (json == null)
			{
				return null;
			}

			return Parser.Parse(json);
		}

		sealed class Parser : IDisposable
		{
			const string WORD_BREAK = "{}[],:\"";

			public static bool IsWordBreak(char c)
			{
				return Char.IsWhiteSpace(c) || WORD_BREAK.IndexOf(c) != -1;
			}

			const string HEX_DIGIT = "0123456789ABCDEFabcdef";

			public static bool IsHexDigit(char c)
			{
				return HEX_DIGIT.IndexOf(c) != -1;
			}

			enum TOKEN
			{
				NONE,
				CURLY_OPEN,
				CURLY_CLOSE,
				SQUARED_OPEN,
				SQUARED_CLOSE,
				COLON,
				COMMA,
				STRING,
				NUMBER,
				TRUE,
				FALSE,
				NULL
			};

			StringReader json;

			Parser(string jsonString)
			{
				json = new StringReader(jsonString);
			}

			public static object Parse(string jsonString)
			{
				using (var instance = new Parser(jsonString))
				{
					return instance.ParseValue();
				}
			}

			public void Dispose()
			{
				json.Dispose();
				json = null;
			}

			Dictionary<string, object> ParseObject()
			{
				Dictionary<string, object> table = new Dictionary<string, object>();

				// ditch opening brace
				json.Read();

				// {
				while (true)
				{
					switch (NextToken)
					{
						case TOKEN.NONE:
							return null;
						case TOKEN.COMMA:
							continue;
						case TOKEN.CURLY_CLOSE:
							return table;
						case TOKEN.STRING:
							// name
							string name = ParseString();
							if (name == null)
							{
								return null;
							}

							// :
							if (NextToken != TOKEN.COLON)
							{
								return null;
							}
							// ditch the colon
							json.Read();

							// value
							TOKEN valueToken = NextToken;
							object value = ParseByToken(valueToken);
							if (value == null && valueToken != TOKEN.NULL)
								return null;
							table[name] = value;
							break;
						default:
							return null;
					}
				}
			}

			List<object> ParseArray()
			{
				List<object> array = new List<object>();

				// ditch opening bracket
				json.Read();

				// [
				var parsing = true;
				while (parsing)
				{
					TOKEN nextToken = NextToken;

					switch (nextToken)
					{
						case TOKEN.NONE:
							return null;
						case TOKEN.COMMA:
							continue;
						case TOKEN.SQUARED_CLOSE:
							parsing = false;
							break;
						default:
							object value = ParseByToken(nextToken);
							if (value == null && nextToken != TOKEN.NULL)
								return null;
							array.Add(value);
							break;
					}
				}

				return array;
			}

			object ParseValue()
			{
				TOKEN nextToken = NextToken;
				return ParseByToken(nextToken);
			}

			object ParseByToken(TOKEN token)
			{
				switch (token)
				{
					case TOKEN.STRING:
						return ParseString();
					case TOKEN.NUMBER:
						return ParseNumber();
					case TOKEN.CURLY_OPEN:
						return ParseObject();
					case TOKEN.SQUARED_OPEN:
						return ParseArray();
					case TOKEN.TRUE:
						return true;
					case TOKEN.FALSE:
						return false;
					case TOKEN.NULL:
						return null;
					default:
						return null;
				}
			}

			string ParseString()
			{
				StringBuilder s = new StringBuilder();
				char c;

				// ditch opening quote
				json.Read();

				bool parsing = true;
				while (parsing)
				{

					if (json.Peek() == -1)
					{
						parsing = false;
						break;
					}

					c = NextChar;
					switch (c)
					{
						case '"':
							parsing = false;
							break;
						case '\\':
							if (json.Peek() == -1)
							{
								parsing = false;
								break;
							}

							c = NextChar;
							switch (c)
							{
								case '"':
								case '\\':
								case '/':
									s.Append(c);
									break;
								case 'b':
									s.Append('\b');
									break;
								case 'f':
									s.Append('\f');
									break;
								case 'n':
									s.Append('\n');
									break;
								case 'r':
									s.Append('\r');
									break;
								case 't':
									s.Append('\t');
									break;
								case 'u':
									var hex = new char[4];

									for (int i = 0; i < 4; i++)
									{
										hex[i] = NextChar;
										if (!IsHexDigit(hex[i]))
											return null;
									}

									s.Append((char)Convert.ToInt32(new string(hex), 16));
									break;
							}
							break;
						default:
							s.Append(c);
							break;
					}
				}

				return s.ToString();
			}

			object ParseNumber()
			{
				string number = NextWord;

				if (number.IndexOf('.') == -1 && number.IndexOf('E') == -1 && number.IndexOf('e') == -1)
				{
					long parsedInt;
					Int64.TryParse(number, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out parsedInt);
					return parsedInt;
				}

				double parsedDouble;
				Double.TryParse(number, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out parsedDouble);
				return parsedDouble;
			}

			void EatWhitespace()
			{
				while (Char.IsWhiteSpace(PeekChar))
				{
					json.Read();

					if (json.Peek() == -1)
					{
						break;
					}
				}
			}

			char PeekChar {
				get {
					return Convert.ToChar(json.Peek());
				}
			}

			char NextChar {
				get {
					return Convert.ToChar(json.Read());
				}
			}

			string NextWord {
				get {
					StringBuilder word = new StringBuilder();

					while (!IsWordBreak(PeekChar))
					{
						word.Append(NextChar);

						if (json.Peek() == -1)
						{
							break;
						}
					}

					return word.ToString();
				}
			}

			TOKEN NextToken {
				get {
					EatWhitespace();

					if (json.Peek() == -1)
					{
						return TOKEN.NONE;
					}

					switch (PeekChar)
					{
						case '{':
							return TOKEN.CURLY_OPEN;
						case '}':
							json.Read();
							return TOKEN.CURLY_CLOSE;
						case '[':
							return TOKEN.SQUARED_OPEN;
						case ']':
							json.Read();
							return TOKEN.SQUARED_CLOSE;
						case ',':
							json.Read();
							return TOKEN.COMMA;
						case '"':
							return TOKEN.STRING;
						case ':':
							return TOKEN.COLON;
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
						case '-':
							return TOKEN.NUMBER;
					}

					switch (NextWord)
					{
						case "false":
							return TOKEN.FALSE;
						case "true":
							return TOKEN.TRUE;
						case "null":
							return TOKEN.NULL;
					}

					return TOKEN.NONE;
				}
			}
		}

		/// <summary>
		/// Converts a IDictionary / IList object or a simple type (string, int, etc.) into a JSON string
		/// </summary>
		/// <param name="json">A Dictionary&lt;string, object&gt; / List&lt;object&gt;</param>
		/// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
		public static string Serialize(object obj)
		{
			return Serializer.Serialize(obj);
		}

		sealed class Serializer
		{
			StringBuilder builder;

			Serializer()
			{
				builder = new StringBuilder();
			}

			public static string Serialize(object obj)
			{
				var instance = new Serializer();

				instance.SerializeValue(obj);

				return instance.builder.ToString();
			}

			void SerializeValue(object value)
			{
				IList asList;
				IDictionary asDict;
				string asStr;

				if (value == null)
				{
					builder.Append("null");
				}
				else if ((asStr = value as string) != null)
				{
					SerializeString(asStr);
				}
				else if (value is bool)
				{
					builder.Append((bool)value ? "true" : "false");
				}
				else if ((asList = value as IList) != null)
				{
					SerializeArray(asList);
				}
				else if ((asDict = value as IDictionary) != null)
				{
					SerializeObject(asDict);
				}
				else if (value is char)
				{
					SerializeString(new string((char)value, 1));
				}
				else
				{
					SerializeOther(value);
				}
			}

			void SerializeObject(IDictionary obj)
			{
				bool first = true;

				builder.Append('{');

				foreach (object e in obj.Keys)
				{
					if (!first)
					{
						builder.Append(',');
					}

					SerializeString(e.ToString());
					builder.Append(':');

					SerializeValue(obj[e]);

					first = false;
				}

				builder.Append('}');
			}

			void SerializeArray(IList anArray)
			{
				builder.Append('[');

				bool first = true;

				for (int i = 0; i < anArray.Count; i++)
				{
					object obj = anArray[i];
					if (!first)
					{
						builder.Append(',');
					}

					SerializeValue(obj);

					first = false;
				}

				builder.Append(']');
			}

			void SerializeString(string str)
			{
				builder.Append('\"');

				char[] charArray = str.ToCharArray();
				for (int i = 0; i < charArray.Length; i++)
				{
					char c = charArray[i];
					switch (c)
					{
						case '"':
							builder.Append("\\\"");
							break;
						case '\\':
							builder.Append("\\\\");
							break;
						case '\b':
							builder.Append("\\b");
							break;
						case '\f':
							builder.Append("\\f");
							break;
						case '\n':
							builder.Append("\\n");
							break;
						case '\r':
							builder.Append("\\r");
							break;
						case '\t':
							builder.Append("\\t");
							break;
						default:
							int codepoint = Convert.ToInt32(c);
							if ((codepoint >= 32) && (codepoint <= 126))
							{
								builder.Append(c);
							}
							else
							{
								builder.Append("\\u");
								builder.Append(codepoint.ToString("x4"));
							}
							break;
					}
				}

				builder.Append('\"');
			}

			void SerializeOther(object value)
			{
				// NOTE: decimals lose precision during serialization.
				// They always have, I'm just letting you know.
				// Previously floats and doubles lost precision too.
				if (value is float)
				{
					builder.Append(((float)value).ToString("R", System.Globalization.CultureInfo.InvariantCulture));
				}
				else if (value is int
				  || value is uint
				  || value is long
				  || value is sbyte
				  || value is byte
				  || value is short
				  || value is ushort
				  || value is ulong)
				{
					builder.Append(value);
				}
				else if (value is double
				  || value is decimal)
				{
					builder.Append(Convert.ToDouble(value).ToString("R", System.Globalization.CultureInfo.InvariantCulture));
				}
				else
				{
					SerializeString(value.ToString());
				}
			}
		}
	}
}
