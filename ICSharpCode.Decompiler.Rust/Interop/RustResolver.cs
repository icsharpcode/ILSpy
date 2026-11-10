// Copyright (c) 2026 Christoph Wille
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
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>A <c>CanTransformToExtensionMethodCall</c> question the engine asks.</summary>
	internal sealed class RustResolverQuery
	{
		/// <summary><c>NullPropagationTransform.CanTransformToExtensionMethodCall(call)</c>.</summary>
		public const byte CallKind = 0;
		/// <summary><c>CanTransformToExtensionMethodCall(method, ignoreTypeArguments: true)</c>.</summary>
		public const byte MethodKind = 1;

		public byte Kind;
		public IMethod Method = null!;
		public IType? Target;
		/// <summary>The target variable's type and its single store's value type.</summary>
		public (IType Variable, IType Value)? TargetStore;
		public IType[] Arguments = [];
	}

	/// <summary>
	/// Serves a <see cref="CSharpResolver"/> to the engine through <c>ResolverCallback</c>
	/// (rust/ilspy-decompiler-ffi/src/lib.rs) while it reads the main module's MethodDef
	/// <c>row</c>. An exception cannot cross the native frames: it is captured and rethrown by
	/// <see cref="ThrowIfFailed"/> once the engine call has returned.
	/// </summary>
	internal sealed class RustResolver
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate int Callback(IntPtr context, byte* query, UIntPtr length);

		readonly CSharpResolver resolver;
		readonly int row;
		readonly ICompilation compilation;
		readonly IReadOnlyList<MetadataModule> modules;
		readonly DecompilerSettings settings;
		ExceptionDispatchInfo? failure;

		/// <summary>The delegate to pass to the engine; keep this object alive for the call.</summary>
		public Callback Delegate { get; }

		public unsafe RustResolver(CSharpResolver resolver, int row, ICompilation compilation,
			IReadOnlyList<MetadataModule> modules, DecompilerSettings settings)
		{
			this.resolver = resolver;
			this.row = row;
			this.compilation = compilation;
			this.modules = modules;
			this.settings = settings;
			Delegate = Answer;
		}

		unsafe int Answer(IntPtr context, byte* query, UIntPtr length)
		{
			try
			{
				var wire = new ReadOnlySpan<byte>(query, checked((int)length)).ToArray();
				return Answer(IlAstMaterializer.ReadResolverQuery(wire, row, compilation, modules, settings)) ? 1 : 0;
			}
			catch (Exception ex)
			{
				failure ??= ExceptionDispatchInfo.Capture(ex);
				return -1;
			}
		}

		bool Answer(RustResolverQuery query)
		{
			if (query.Kind == RustResolverQuery.MethodKind)
				return resolver.CanTransformToExtensionMethodCall(query.Method, ignoreTypeArguments: true);
			if (query.Kind != RustResolverQuery.CallKind)
				throw new InvalidOperationException($"Unknown resolver query kind {query.Kind}");
			if (!CanTransform(query.Target!))
				return false;
			if (query.TargetStore is var (variable, value))
				return variable.Equals(value) && CanTransform(value);
			return true;

			bool CanTransform(IType target) => resolver.CanTransformToExtensionMethodCall(query.Method, Array.Empty<IType>(),
				new ResolveResult(target), query.Arguments.Select(a => new ResolveResult(a)).ToArray(), argumentNames: null);
		}

		/// <summary>Rethrows the first exception the resolver threw during the engine call.</summary>
		public void ThrowIfFailed() => failure?.Throw();
	}
}
