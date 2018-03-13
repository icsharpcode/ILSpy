//
// ResolvedAttributeBlob.cs
//
// Author:
//       Daniel Grunwald <daniel@danielgrunwald.de>
//
// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	sealed class CecilResolvedAttribute : IAttribute
	{
		readonly ITypeResolveContext context;
		readonly byte[] blob;
		readonly IList<ITypeReference> ctorParameterTypes;

		IMethod constructor;
		volatile bool constructorResolved;

		IReadOnlyList<ResolveResult> positionalArguments;
		IReadOnlyList<KeyValuePair<IMember, ResolveResult>> namedArguments;
		
		public CecilResolvedAttribute(ITypeResolveContext context, UnresolvedAttributeBlob unresolved)
		{
			this.context = context;
			this.blob = unresolved.blob;
			this.ctorParameterTypes = unresolved.ctorParameterTypes;
			this.AttributeType = unresolved.attributeType.Resolve(context);
		}
		
		public CecilResolvedAttribute(ITypeResolveContext context, IType attributeType)
		{
			this.context = context;
			this.AttributeType = attributeType;
			this.ctorParameterTypes = EmptyList<ITypeReference>.Instance;
		}
		
		public IType AttributeType { get; }

		public IMethod Constructor {
			get {
				if (!constructorResolved) {
					constructor = ResolveConstructor();
					constructorResolved = true;
				}
				return constructor;
			}
		}
		
		IMethod ResolveConstructor()
		{
			var parameterTypes = ctorParameterTypes.Resolve(context);
			foreach (var ctor in AttributeType.GetConstructors(m => m.Parameters.Count == parameterTypes.Count)) {
				var ok = true;
				for (var i = 0; i < parameterTypes.Count; i++) {
					if (!ctor.Parameters[i].Type.Equals(parameterTypes[i])) {
						ok = false;
						break;
					}
				}
				if (ok)
					return ctor;
			}
			return null;
		}
		
		public IReadOnlyList<ResolveResult> PositionalArguments {
			get {
				var result = LazyInit.VolatileRead(ref this.positionalArguments);
				if (result != null) {
					return result;
				}
				DecodeBlob();
				return positionalArguments;
			}
		}
		
		public IReadOnlyList<KeyValuePair<IMember, ResolveResult>> NamedArguments {
			get {
				var result = LazyInit.VolatileRead(ref this.namedArguments);
				if (result != null) {
					return result;
				}
				DecodeBlob();
				return namedArguments;
			}
		}
		
		public override string ToString()
		{
			return "[" + AttributeType.ToString() + "(...)]";
		}
		
		void DecodeBlob()
		{
			var positionalArguments = new List<ResolveResult>();
			var namedArguments = new List<KeyValuePair<IMember, ResolveResult>>();
			DecodeBlob(positionalArguments, namedArguments);
			Interlocked.CompareExchange(ref this.positionalArguments, positionalArguments, null);
			Interlocked.CompareExchange(ref this.namedArguments, namedArguments, null);
		}
		
		void DecodeBlob(List<ResolveResult> positionalArguments, List<KeyValuePair<IMember, ResolveResult>> namedArguments)
		{
			if (blob == null)
				return;
			var reader = new BlobReader(blob, context.CurrentAssembly);
			if (reader.ReadUInt16() != 0x0001) {
				Debug.WriteLine("Unknown blob prolog");
				return;
			}
			foreach (var ctorParameter in ctorParameterTypes.Resolve(context)) {
				ResolveResult arg;
				bool isError;
				try {
					arg = reader.ReadFixedArg (ctorParameter);
					positionalArguments.Add(arg);
					isError = arg.IsError;
				} catch (Exception ex) {
					Debug.WriteLine("Crash during blob decoding: " + ex);
					isError = true;
				}
				if (isError) {
					// After a decoding error, we must stop decoding the blob because
					// we might have read too few bytes due to the error.
					// Just fill up the remaining arguments with ErrorResolveResult:
					while (positionalArguments.Count < ctorParameterTypes.Count)
						positionalArguments.Add(ErrorResolveResult.UnknownError);
					return;
				}
			}
			try {
				var numNamed = reader.ReadUInt16();
				for (var i = 0; i < numNamed; i++) {
					var namedArg = reader.ReadNamedArg(AttributeType);
					if (namedArg.Key != null)
						namedArguments.Add(namedArg);
				}
			} catch (Exception ex) {
				Debug.WriteLine("Crash during blob decoding: " + ex);
			}
		}
	}
}
