// Copyright (c) 2022 Tom-Englert
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
using System.Collections.Immutable;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.Disassembler
{
	public static class MethodSignatureProvider
	{
		public static IMethodSignature GetMethodSignature(this MethodDefinitionHandle handle, PEFile module)
		{
			return GetMethodSignature(handle, module.Metadata, new CSharpSignatureTypeProvider(), new MetadataGenericContext(handle, module));
		}

		public static IMethodSignature GetMethodSignature<TProvider, TGenericContext>(this MethodDefinitionHandle handle, MetadataReader metadata, TProvider provider, TGenericContext genericContext)
			where TProvider : ISignatureTypeProvider<Action<ITextOutput>, TGenericContext>
		{
			var definition = metadata.GetMethodDefinition(handle);
			var name = metadata.GetString(definition.Name);

			var signature = definition.DecodeSignature(provider, genericContext);

			var genericParameterCount = signature.GenericParameterCount;
			if (genericParameterCount > 0)
			{
				name += $"`{genericParameterCount}";
			}

			var returnType = new Lazy<string>(() => {
				var output = new PlainTextOutput();
				signature.ReturnType(output);
				return output.ToString();
			});

			var parameterTypes = new Lazy<IReadOnlyList<string>>(() => {
				var items = new List<string>(signature.ParameterTypes.Length);
				foreach (var parameterType in signature.ParameterTypes)
				{
					var output = new PlainTextOutput();
					parameterType(output);
					items.Add(output.ToString());
				}
				return items.ToImmutableArray();
			});

			return new MethodSignature(handle, name, returnType, parameterTypes);
		}

		private class MethodSignature : IMethodSignature
		{
			private readonly Lazy<string> returnType;
			private readonly Lazy<IReadOnlyList<string>> parameterTypes;

			public MethodSignature(MethodDefinitionHandle handle, string name, Lazy<string> returnType, Lazy<IReadOnlyList<string>> parameterTypes)
			{
				Handle = handle;
				Name = name;
				this.returnType = returnType;
				this.parameterTypes = parameterTypes;
			}

			public MethodDefinitionHandle Handle { get; }

			public string Name { get; }

			public string ReturnType => returnType.Value;

			public IReadOnlyList<string> ParameterTypes => parameterTypes.Value;
		}
	}
}
