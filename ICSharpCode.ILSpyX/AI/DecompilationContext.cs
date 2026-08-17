// Copyright (c) 2026 Masroor
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.ILSpyX.AI
{
	public sealed record DecompilationContext
	{
		public string DecompiledCSharp { get; init; } = string.Empty;
		public string? IL { get; init; }
		public string FullyQualifiedName { get; init; } = string.Empty;
		public string AssemblyName { get; init; } = string.Empty;
		public string TargetFramework { get; init; } = string.Empty;
		public IReadOnlyList<string> Callers { get; init; } = Array.Empty<string>();
		public IReadOnlyList<string> Callees { get; init; } = Array.Empty<string>();
		public IReadOnlyList<string> ImplementedInterfaces { get; init; } = Array.Empty<string>();
		public IReadOnlyList<string> Attributes { get; init; } = Array.Empty<string>();
		public IReadOnlyList<string> StringLiterals { get; init; } = Array.Empty<string>();
		public int ApproximateTokenCount { get; init; }

		public string ToMarkdown()
		{
			if (string.IsNullOrEmpty(DecompiledCSharp)
				&& string.IsNullOrEmpty(IL)
				&& string.IsNullOrEmpty(FullyQualifiedName)
				&& string.IsNullOrEmpty(AssemblyName)
				&& string.IsNullOrEmpty(TargetFramework)
				&& Callers.Count == 0
				&& Callees.Count == 0
				&& ImplementedInterfaces.Count == 0
				&& Attributes.Count == 0
				&& StringLiterals.Count == 0)
				return string.Empty;

			var builder = new StringBuilder();
			builder.Append("# ").AppendLine(FullyQualifiedName);
			builder.AppendLine();
			builder.Append("**Assembly:** ").AppendLine(AssemblyName);
			if (!string.IsNullOrEmpty(TargetFramework))
				builder.Append("**Target Framework:** ").AppendLine(TargetFramework);
			builder.AppendLine();

			AppendList(builder, "**Attributes:**", Attributes);
			AppendList(builder, "**Implements:**", ImplementedInterfaces);

			builder.AppendLine("## Decompiled Code");
			builder.AppendLine();
			builder.AppendLine("```csharp");
			builder.AppendLine(DecompiledCSharp);
			builder.AppendLine("```");
			builder.AppendLine();

			if (!string.IsNullOrEmpty(IL))
			{
				builder.AppendLine("## IL Bytecode");
				builder.AppendLine();
				builder.AppendLine("```il");
				builder.AppendLine(IL);
				builder.AppendLine("```");
				builder.AppendLine();
			}

			AppendList(builder, "**String Literals:**", StringLiterals, value => "\"" + value + "\"", limit: 20);
			AppendList(builder, "**Called By:**", Callers, limit: 10);
			AppendList(builder, "**Calls:**", Callees, limit: 10);
			return builder.ToString();
		}

		static void AppendList(StringBuilder builder, string heading, IReadOnlyList<string> values, Func<string, string>? format = null, int limit = int.MaxValue)
		{
			if (values.Count == 0)
				return;
			builder.AppendLine(heading);
			int count = Math.Min(values.Count, limit);
			for (int i = 0; i < count; i++)
				builder.Append("- ").AppendLine(format is null ? values[i] : format(values[i]));
			if (count < values.Count)
				builder.Append("- ... and ").Append(values.Count - count).AppendLine(" more");
			builder.AppendLine();
		}
	}
}
