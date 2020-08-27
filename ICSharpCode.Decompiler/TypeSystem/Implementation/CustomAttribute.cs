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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Custom attribute loaded from metadata.
	/// </summary>
	sealed class CustomAttribute : IAttribute
	{
		readonly MetadataModule module;
		readonly CustomAttributeHandle handle;
		public IMethod Constructor { get; }

		// lazy-loaded:
		CustomAttributeValue<IType> value;
		bool valueDecoded;
		bool hasDecodeErrors;

		internal CustomAttribute(MetadataModule module, IMethod attrCtor, CustomAttributeHandle handle)
		{
			Debug.Assert(module != null);
			Debug.Assert(attrCtor != null);
			Debug.Assert(!handle.IsNil);
			this.module = module;
			this.Constructor = attrCtor;
			this.handle = handle;
		}

		public IType AttributeType => Constructor.DeclaringType;

		public ImmutableArray<CustomAttributeTypedArgument<IType>> FixedArguments {
			get {
				DecodeValue();
				return value.FixedArguments;
			}
		}

		public ImmutableArray<CustomAttributeNamedArgument<IType>> NamedArguments {
			get {
				DecodeValue();
				return value.NamedArguments;
			}
		}

		public bool HasDecodeErrors {
			get {
				DecodeValue();
				return hasDecodeErrors;
			}
		}

		void DecodeValue()
		{
			lock (this)
			{
				try
				{
					if (!valueDecoded)
					{
						var metadata = module.metadata;
						var attr = metadata.GetCustomAttribute(handle);
						value = attr.DecodeValue(module.TypeProvider);
						valueDecoded = true;
					}
				}
				catch (EnumUnderlyingTypeResolveException)
				{
					value = new CustomAttributeValue<IType>(
						ImmutableArray<CustomAttributeTypedArgument<IType>>.Empty,
						ImmutableArray<CustomAttributeNamedArgument<IType>>.Empty
					);
					hasDecodeErrors = true;
					valueDecoded = true; // in case of errors, never try again.
				}
				catch (BadImageFormatException)
				{
					value = new CustomAttributeValue<IType>(
						ImmutableArray<CustomAttributeTypedArgument<IType>>.Empty,
						ImmutableArray<CustomAttributeNamedArgument<IType>>.Empty
					);
					hasDecodeErrors = true;
					valueDecoded = true; // in case of errors, never try again.
				}
			}
		}

		internal static IMember MemberForNamedArgument(IType attributeType, CustomAttributeNamedArgument<IType> namedArgument)
		{
			switch (namedArgument.Kind)
			{
				case CustomAttributeNamedArgumentKind.Field:
					return attributeType.GetFields(f => f.Name == namedArgument.Name).LastOrDefault();
				case CustomAttributeNamedArgumentKind.Property:
					return attributeType.GetProperties(p => p.Name == namedArgument.Name).LastOrDefault();
				default:
					return null;
			}
		}
	}
}
