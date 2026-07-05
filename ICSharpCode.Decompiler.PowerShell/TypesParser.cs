// Copyright (c) 2017 Christoph Wille
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
using System.Linq;
using System.Text;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.PowerShell
{
	public static class TypesParser
	{
		public static HashSet<TypeKind> ParseSelection(string[] values)
		{
			var possibleValues = new Dictionary<string, TypeKind>(StringComparer.OrdinalIgnoreCase) { ["class"] = TypeKind.Class, ["struct"] = TypeKind.Struct, ["interface"] = TypeKind.Interface, ["enum"] = TypeKind.Enum, ["delegate"] = TypeKind.Delegate };
			HashSet<TypeKind> kinds = new HashSet<TypeKind>();
			if (values.Length == 1 && !possibleValues.Keys.Any(v => values[0].StartsWith(v, StringComparison.OrdinalIgnoreCase)))
			{
				foreach (char ch in values[0])
				{
					switch (ch)
					{
						case 'c':
							kinds.Add(TypeKind.Class);
							break;
						case 'i':
							kinds.Add(TypeKind.Interface);
							break;
						case 's':
							kinds.Add(TypeKind.Struct);
							break;
						case 'd':
							kinds.Add(TypeKind.Delegate);
							break;
						case 'e':
							kinds.Add(TypeKind.Enum);
							break;
					}
				}
			}
			else
			{
				foreach (var value in values)
				{
					string v = value;
					while (v.Length > 0 && !possibleValues.ContainsKey(v))
						v = v.Remove(v.Length - 1);
					if (possibleValues.TryGetValue(v, out var kind))
						kinds.Add(kind);
				}
			}
			return kinds;
		}
	}
}
