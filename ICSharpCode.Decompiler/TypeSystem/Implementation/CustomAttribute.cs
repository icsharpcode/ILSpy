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
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Util;
using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Custom attribute loaded from metadata.
	/// </summary>
	sealed class CustomAttribute : IAttribute
	{
		readonly MetadataAssembly assembly;
		readonly SRM.CustomAttributeHandle handle;
		public IMethod Constructor { get; }

		// lazy-loaded:
		SRM.CustomAttributeValue<IType> value;
		bool valueDecoded;

		internal CustomAttribute(MetadataAssembly assembly, IMethod attrCtor, SRM.CustomAttributeHandle handle)
		{
			Debug.Assert(assembly != null);
			Debug.Assert(attrCtor != null);
			Debug.Assert(!handle.IsNil);
			this.assembly = assembly;
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

		void DecodeValue()
		{
			lock (this) {
				if (!valueDecoded) {
					var metadata = assembly.metadata;
					var attr = metadata.GetCustomAttribute(handle);
					value = attr.DecodeValue(assembly.TypeProvider);
					valueDecoded = true;
				}
			}
		}

		internal static KeyValuePair<IMember, ResolveResult> MakeNamedArg(ICompilation compilation, IType attrType, string name, ResolveResult rr)
		{
			var field = attrType.GetFields(f => f.Name == name).FirstOrDefault();
			if (field != null) {
				return new KeyValuePair<IMember, ResolveResult>(field, rr);
			}
			var prop = attrType.GetProperties(f => f.Name == name).FirstOrDefault();
			if (prop != null) {
				return new KeyValuePair<IMember, ResolveResult>(prop, rr);
			}
			field = new FakeField(compilation) {
				DeclaringType = attrType,
				Name = name,
				ReturnType = rr.Type
			};
			return new KeyValuePair<IMember, ResolveResult>(field, rr);
		}

		internal static KeyValuePair<IMember, ResolveResult> MakeNamedArg(ICompilation compilation, IType attrType, SRM.CustomAttributeNamedArgumentKind kind, string name, ResolveResult rr)
		{
			if (kind == CustomAttributeNamedArgumentKind.Field) {
				var field = attrType.GetFields(f => f.Name == name).FirstOrDefault();
				if (field != null) {
					return new KeyValuePair<IMember, ResolveResult>(field, rr);
				}
			}
			if (kind == CustomAttributeNamedArgumentKind.Property) {
				var prop = attrType.GetProperties(f => f.Name == name).FirstOrDefault();
				if (prop != null) {
					return new KeyValuePair<IMember, ResolveResult>(prop, rr);
				}
			}
			var fakeField = new FakeField(compilation) {
				DeclaringType = attrType,
				Name = name,
				ReturnType = rr.Type
			};
			return new KeyValuePair<IMember, ResolveResult>(fakeField, rr);
		}

		internal static IReadOnlyList<KeyValuePair<IMember, ResolveResult>> ConvertNamedArguments(
			ICompilation compilation, IType attributeType, ImmutableArray<SRM.CustomAttributeNamedArgument<IType>> namedArgs)
		{
			var arr = new KeyValuePair<IMember, ResolveResult>[namedArgs.Length];
			for (int i = 0; i < arr.Length; i++) {
				var namedArg = namedArgs[i];
				arr[i] = MakeNamedArg(compilation, attributeType, namedArg.Kind, namedArg.Name,
					ConvertArgument(compilation, namedArg.Type, namedArg.Value));
			}
			return arr;
		}

		private static ResolveResult ConvertArgument(ICompilation compilation, IType type, object value)
		{
			if (value is ImmutableArray<SRM.CustomAttributeTypedArgument<IType>> arr) {
				var arrSize = new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), arr.Length);
				return new ArrayCreateResolveResult(type, new[] { arrSize },
					ConvertArguments(compilation, type, arr));
			} else if (value is IType valueType) {
				return new TypeOfResolveResult(type, valueType);
			} else {
				return new ConstantResolveResult(type, value);
			}
		}

		private static IReadOnlyList<ResolveResult> ConvertArguments(ICompilation compilation, IType type, ImmutableArray<SRM.CustomAttributeTypedArgument<IType>> arr)
		{
			return arr.SelectArray(arg => ConvertArgument(compilation, type ?? arg.Type, arg.Value));
		}
	}
}
