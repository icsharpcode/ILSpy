using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class InterpolatedStringHandlers
	{
		[InterpolatedStringHandler]
		public ref struct LogHandler(int literalLength, int formattedCount)
		{
			private DefaultInterpolatedStringHandler builder = new DefaultInterpolatedStringHandler(literalLength, formattedCount);

			public void AppendLiteral(string s)
			{
				builder.AppendLiteral(s);
			}

			public void AppendFormatted<T>(T value)
			{
				builder.AppendFormatted(value);
			}

			public void AppendFormatted<T>(T value, string format)
			{
				builder.AppendFormatted(value, format);
			}

			internal string GetText()
			{
				return builder.ToStringAndClear();
			}
		}

		[InterpolatedStringHandler]
		public ref struct ConditionalLogHandler
		{
			private DefaultInterpolatedStringHandler builder;

			public ConditionalLogHandler(int literalLength, int formattedCount, Logger logger, out bool shouldAppend)
			{
				shouldAppend = logger.Enabled;
				builder = (shouldAppend ? new DefaultInterpolatedStringHandler(literalLength, formattedCount) : default(DefaultInterpolatedStringHandler));
			}

			public void AppendLiteral(string s)
			{
				builder.AppendLiteral(s);
			}

			public void AppendFormatted<T>(T value)
			{
				builder.AppendFormatted(value);
			}

			internal string GetText()
			{
				return builder.ToStringAndClear();
			}
		}

		[InterpolatedStringHandler]
		public ref struct BoolLogHandler(int literalLength, int formattedCount)
		{
			private DefaultInterpolatedStringHandler builder = new DefaultInterpolatedStringHandler(literalLength, formattedCount);

			public bool AppendLiteral(string s)
			{
				builder.AppendLiteral(s);
				return true;
			}

			public bool AppendFormatted<T>(T value)
			{
				builder.AppendFormatted(value);
				return true;
			}

			internal string GetText()
			{
				return builder.ToStringAndClear();
			}
		}

		public class Logger
		{
			public bool Enabled;

			public void Log(LogHandler message)
			{
				Console.WriteLine(message.GetText());
			}

			public void LogIfEnabled([InterpolatedStringHandlerArgument("")] ConditionalLogHandler message)
			{
				if (Enabled)
				{
					Console.WriteLine(message.GetText());
				}
			}

			public void LogBool(BoolLogHandler message)
			{
				Console.WriteLine(message.GetText());
			}

			public static void LogStatic(Logger logger, [InterpolatedStringHandlerArgument("logger")] ConditionalLogHandler message)
			{
				if (logger.Enabled)
				{
					Console.WriteLine(message.GetText());
				}
			}
		}

		public void Use(Logger logger, int x, string s)
		{
			logger.Log($"x = {x}, s = {s}");
		}

		public void UseWithFormat(Logger logger, double d)
		{
			logger.Log($"d = {d:N2}");
		}

		public void UseConditional(Logger logger, int x)
		{
			logger.LogIfEnabled($"expensive: {x}");
		}

		public void UseStaticConditional(Logger logger, int x)
		{
			Logger.LogStatic(logger, $"static {x}");
		}

		public void UseBool(Logger logger, int x)
		{
			logger.LogBool($"bool {x}");
		}

		public string WithCulture(double d, int i)
		{
			return string.Create(CultureInfo.InvariantCulture, $"{d:N2} {i,5}");
		}

		public string WithCultureStackalloc(double d)
		{
			return string.Create(CultureInfo.InvariantCulture, stackalloc char[64], $"{d:N2}");
		}

		public string SpanWithAlignment(ReadOnlySpan<char> span)
		{
			return $"{span,10}";
		}
	}
}
