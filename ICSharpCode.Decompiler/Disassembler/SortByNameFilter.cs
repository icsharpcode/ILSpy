// Copyright (c) 2022 tom-englert.de for the SharpDevelop Team
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

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.Disassembler;

public class SortByNameFilter : IEntityProcessor
{
	public IReadOnlyCollection<InterfaceImplementationHandle> Filter(PEFile module,
		IReadOnlyCollection<InterfaceImplementationHandle> items)
	{
		return items.Select(item => new { Key = GetSortKey(item, module), Value = item })
			.OrderBy(item => item.Key)
			.Select(item => item.Value)
			.ToArray();
	}

	public IReadOnlyCollection<TypeDefinitionHandle> Filter(PEFile module,
		IReadOnlyCollection<TypeDefinitionHandle> items)
	{
		return items.Select(item => new { Key = GetSortKey(item, module), Value = item })
			.OrderBy(item => item.Key)
			.Select(item => item.Value)
			.ToArray();
	}

	public IReadOnlyCollection<MethodDefinitionHandle> Filter(PEFile module,
		IReadOnlyCollection<MethodDefinitionHandle> items)
	{
		return items.Select(item => new { Key = GetSortKey(item, module), Value = item })
			.OrderBy(item => item.Key)
			.Select(item => item.Value)
			.ToArray();
	}

	public IReadOnlyCollection<PropertyDefinitionHandle> Filter(PEFile module,
		IReadOnlyCollection<PropertyDefinitionHandle> items)
	{
		return items.Select(item => new { Key = GetSortKey(item, module), Value = item })
			.OrderBy(item => item.Key)
			.Select(item => item.Value)
			.ToArray();
	}

	public IReadOnlyCollection<EventDefinitionHandle> Filter(PEFile module,
		IReadOnlyCollection<EventDefinitionHandle> items)
	{
		return items.Select(item => new { Key = GetSortKey(item, module), Value = item })
			.OrderBy(item => item.Key)
			.Select(item => item.Value)
			.ToArray();
	}

	public IReadOnlyCollection<FieldDefinitionHandle> Filter(PEFile module,
		IReadOnlyCollection<FieldDefinitionHandle> items)
	{
		return items.Select(item => new { Key = GetSortKey(item, module), Value = item })
			.OrderBy(item => item.Key)
			.Select(item => item.Value)
			.ToArray();
	}

	private static string GetSortKey(TypeDefinitionHandle handle, PEFile module) =>
		handle.GetFullTypeName(module.Metadata).ToILNameString();

	private static string GetSortKey(MethodDefinitionHandle handle, PEFile module)
	{
		PlainTextOutput output = new PlainTextOutput();
		MethodDefinition definition = module.Metadata.GetMethodDefinition(handle);

		output.Write(module.Metadata.GetString(definition.Name));
		int genericParameterCount = definition.GetGenericParameters().Count;
		if (genericParameterCount > 0)
		{
			output.Write($"`{genericParameterCount}");
		}

		output.Write("(");

		DisassemblerSignatureTypeProvider signatureProvider = new DisassemblerSignatureTypeProvider(module, output);
		MethodSignature<Action<ILNameSyntax>> signature =
			definition.DecodeSignature(signatureProvider, new MetadataGenericContext(handle, module));
		bool first = true;

		foreach (Action<ILNameSyntax>? action in signature.ParameterTypes)
		{
			if (!first)
			{
				output.Write(", ");
			}

			first = false;
			action(ILNameSyntax.ShortTypeName);
		}

		output.Write(")");

		return output.ToString();
	}

	private static string GetSortKey(InterfaceImplementationHandle handle, PEFile module) =>
		module.Metadata.GetInterfaceImplementation(handle)
			.Interface
			.GetFullTypeName(module.Metadata)
			.ToILNameString();

	private static string GetSortKey(FieldDefinitionHandle handle, PEFile module) =>
		module.Metadata.GetString(module.Metadata.GetFieldDefinition(handle).Name);

	private static string GetSortKey(PropertyDefinitionHandle handle, PEFile module) =>
		module.Metadata.GetString(module.Metadata.GetPropertyDefinition(handle).Name);

	private static string GetSortKey(EventDefinitionHandle handle, PEFile module) =>
		module.Metadata.GetString(module.Metadata.GetEventDefinition(handle).Name);
}
