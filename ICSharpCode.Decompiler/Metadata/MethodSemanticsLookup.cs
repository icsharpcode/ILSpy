// Copyright (c) 2018 Daniel Grunwald
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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Metadata
{
	/// <summary>
	/// Lookup structure that, for an accessor, can find the associated property/event.
	/// </summary>
	class MethodSemanticsLookup
	{
		const MethodSemanticsAttributes csharpAccessors =
			MethodSemanticsAttributes.Getter | MethodSemanticsAttributes.Setter
			| MethodSemanticsAttributes.Adder | MethodSemanticsAttributes.Remover;

		readonly struct Entry : IComparable<Entry>
		{
			public readonly MethodSemanticsAttributes Semantics;
			public readonly int MethodRowNumber;
			public MethodDefinitionHandle Method => MetadataTokens.MethodDefinitionHandle(MethodRowNumber);
			public readonly EntityHandle Association;

			public Entry(MethodSemanticsAttributes semantics, MethodDefinitionHandle method, EntityHandle association)
			{
				Semantics = semantics;
				MethodRowNumber = MetadataTokens.GetRowNumber(method);
				Association = association;
			}

			public int CompareTo(Entry other)
			{
				return MethodRowNumber.CompareTo(other.MethodRowNumber);
			}
		}

		// entries, sorted by MethodRowNumber
		readonly List<Entry> entries;

		public MethodSemanticsLookup(MetadataReader metadata, MethodSemanticsAttributes filter = csharpAccessors)
		{
			if ((filter & MethodSemanticsAttributes.Other) != 0)
			{
				throw new NotSupportedException("SRM doesn't provide access to 'other' accessors");
			}
			entries = new List<Entry>(metadata.GetTableRowCount(TableIndex.MethodSemantics));
			foreach (var propHandle in metadata.PropertyDefinitions)
			{
				var prop = metadata.GetPropertyDefinition(propHandle);
				var accessors = prop.GetAccessors();
				AddEntry(MethodSemanticsAttributes.Getter, accessors.Getter, propHandle);
				AddEntry(MethodSemanticsAttributes.Setter, accessors.Setter, propHandle);
			}
			foreach (var eventHandle in metadata.EventDefinitions)
			{
				var ev = metadata.GetEventDefinition(eventHandle);
				var accessors = ev.GetAccessors();
				AddEntry(MethodSemanticsAttributes.Adder, accessors.Adder, eventHandle);
				AddEntry(MethodSemanticsAttributes.Remover, accessors.Remover, eventHandle);
				AddEntry(MethodSemanticsAttributes.Raiser, accessors.Raiser, eventHandle);
			}
			entries.Sort();

			void AddEntry(MethodSemanticsAttributes semantics, MethodDefinitionHandle method, EntityHandle association)
			{
				if ((semantics & filter) == 0 || method.IsNil)
					return;
				entries.Add(new Entry(semantics, method, association));
			}
		}

		public (EntityHandle, MethodSemanticsAttributes) GetSemantics(MethodDefinitionHandle method)
		{
			int pos = entries.BinarySearch(new Entry(0, method, default(EntityHandle)));
			if (pos >= 0)
			{
				return (entries[pos].Association, entries[pos].Semantics);
			}
			else
			{
				return (default(EntityHandle), 0);
			}
		}
	}
}
