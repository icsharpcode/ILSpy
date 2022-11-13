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

using ICSharpCode.Decompiler.Metadata;

using System.Reflection.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Decompiler.Disassembler
{
	public abstract class Adapter
	{
		private readonly Lazy<string> displayName;

		protected Adapter(PEFile module, TypeDefinitionAdapter? containingType)
		{
			Module = module;
			ContainingType = containingType;
			displayName = new Lazy<string>(GetDisplayName);
		}

		public PEFile Module { get; }

		public string DisplayName => displayName.Value;

		public TypeDefinitionAdapter? ContainingType { get; }

		protected string GetString(StringHandle handle)
		{
			return Module.Metadata.GetString(handle);
		}

		protected abstract string GetDisplayName();
	}

	public abstract class Adapter<T> : Adapter where T : struct
	{
		protected Adapter(PEFile module, T handle, TypeDefinitionAdapter? containingType)
			: base(module, containingType)
		{
			Handle = handle;
		}

		public T Handle { get; }
	}

	public class TypeDefinitionAdapter : Adapter<TypeDefinitionHandle>
	{
		public TypeDefinitionAdapter(PEFile module, TypeDefinitionHandle handle, TypeDefinitionAdapter? containingType = null)
			: base(module, handle, containingType)
		{
			Definition = module.Metadata.GetTypeDefinition(handle);
			GenericContext = new MetadataGenericContext(handle, module);
		}

		public TypeDefinition Definition { get; }

		public MetadataGenericContext GenericContext { get; }

		protected override string GetDisplayName() => Definition.GetFullTypeName(Module.Metadata).ToILNameString();
	}

	public class MethodDefinitionAdapter : Adapter<MethodDefinitionHandle>
	{
		public MethodDefinitionAdapter(PEFile module, MethodDefinitionHandle handle, TypeDefinitionAdapter? containingType = null)
			: base(module, handle, containingType)
		{
			Definition = module.Metadata.GetMethodDefinition(handle);
			GenericContext = new MetadataGenericContext(handle, module);
		}

		public MethodDefinition Definition { get; }

		public MetadataGenericContext GenericContext { get; }

		protected override string GetDisplayName()
		{
			var output = new PlainTextOutput();

			output.Write(GetString(Definition.Name));
			var genericParameterCount = Definition.GetGenericParameters().Count;
			if (genericParameterCount > 0)
			{
				output.Write($"`{genericParameterCount}");
			}

			output.Write("(");

			var signatureProvider = new DisassemblerSignatureTypeProvider(Module, output);
			var signature = Definition.DecodeSignature(signatureProvider, GenericContext);
			var first = true;

			foreach (var action in signature.ParameterTypes)
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
	}

	public class InterfaceImplementationAdapter : Adapter<InterfaceImplementationHandle>
	{
		public InterfaceImplementationAdapter(PEFile module, InterfaceImplementationHandle handle, TypeDefinitionAdapter? containingType = null)
			: base(module, handle, containingType)
		{
			Implementation = module.Metadata.GetInterfaceImplementation(handle);
		}

		public InterfaceImplementation Implementation { get; }

		protected override string GetDisplayName() => Implementation.Interface.GetFullTypeName(Module.Metadata).ToILNameString();
	}

	public class FieldDefinitionAdapter : Adapter<FieldDefinitionHandle>
	{
		public FieldDefinitionAdapter(PEFile module, FieldDefinitionHandle handle, TypeDefinitionAdapter? containingType = null)
			: base(module, handle, containingType)
		{
			Definition = module.Metadata.GetFieldDefinition(handle);
		}

		public FieldDefinition Definition { get; }

		protected override string GetDisplayName() => GetString(Definition.Name);
	}

	public class PropertyDefinitionAdapter : Adapter<PropertyDefinitionHandle>
	{
		public PropertyDefinitionAdapter(PEFile module, PropertyDefinitionHandle handle, TypeDefinitionAdapter? containingType = null)
			: base(module, handle, containingType)
		{
			Definition = module.Metadata.GetPropertyDefinition(handle);
		}

		public PropertyDefinition Definition { get; }

		protected override string GetDisplayName() => GetString(Definition.Name);
	}

	public class EventDefinitionAdapter : Adapter<EventDefinitionHandle>
	{
		public EventDefinitionAdapter(PEFile module, EventDefinitionHandle handle, TypeDefinitionAdapter? containingType = null)
			: base(module, handle, containingType)
		{
			Definition = module.Metadata.GetEventDefinition(handle);
		}

		public EventDefinition Definition { get; }

		protected override string GetDisplayName() => GetString(Definition.Name);
	}

	public static class AdapterExtensionMethods
	{
		public static ICollection<TypeDefinitionAdapter> GetTopLevelTypeDefinitions(this PEFile module)
		{
			return module.Metadata.GetTopLevelTypeDefinitions().Select(h => new TypeDefinitionAdapter(module, h)).ToArray();
		}

		public static ICollection<TypeDefinitionAdapter> GetNestedTypes(this TypeDefinitionAdapter type)
		{
			return type.Definition.GetNestedTypes().Select(item => new TypeDefinitionAdapter(type.Module, item, type)).ToArray();
		}

		public static ICollection<MethodDefinitionAdapter> GetMethods(this TypeDefinitionAdapter type)
		{
			return type.Definition.GetMethods().Select(item => new MethodDefinitionAdapter(type.Module, item, type)).ToArray();
		}

		public static ICollection<InterfaceImplementationAdapter> GetInterfaceImplementations(this TypeDefinitionAdapter type)
		{
			return type.Definition.GetInterfaceImplementations().Select(item => new InterfaceImplementationAdapter(type.Module, item, type)).ToArray();
		}

		public static ICollection<FieldDefinitionAdapter> GetFields(this TypeDefinitionAdapter type)
		{
			return type.Definition.GetFields().Select(item => new FieldDefinitionAdapter(type.Module, item, type)).ToArray();
		}

		public static ICollection<EventDefinitionAdapter> GetEvents(this TypeDefinitionAdapter type)
		{
			return type.Definition.GetEvents().Select(item => new EventDefinitionAdapter(type.Module, item, type)).ToArray();
		}

		public static ICollection<PropertyDefinitionAdapter> GetProperties(this TypeDefinitionAdapter type)
		{
			return type.Definition.GetProperties().Select(item => new PropertyDefinitionAdapter(type.Module, item, type)).ToArray();
		}
	}
}
