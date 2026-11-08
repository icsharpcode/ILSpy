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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>An error or panic reported by the Rust engine.</summary>
	public sealed class RustEngineException : Exception
	{
		public RustEngineException(string message) : base(message)
		{
		}
	}

	/// <summary>
	/// The engine session of one type system: its compilation is loaded once and shared by every
	/// decompiler on that type system, also across threads.
	/// </summary>
	internal sealed class RustEngine
	{
		sealed class Entry
		{
			public RustEngine? Engine;
			public string? UnsupportedReason;
		}

		static readonly ConditionalWeakTable<IDecompilerTypeSystem, Entry> engines = new();

		/// <summary>The number of IL transforms the engine runs (<c>IL_PREFIX</c>).</summary>
		public static readonly int ILPrefix = (int)NativeMethods.ilspy_il_prefix();

		readonly IDecompilerTypeSystem typeSystem;
		readonly RustSession session;
		readonly IReadOnlyList<MetadataModule> modules;

		RustEngine(IDecompilerTypeSystem typeSystem)
		{
			this.typeSystem = typeSystem;
			session = RustSession.Create();
			using var table = new RustModuleTable(typeSystem);
			table.Load(session);
			modules = table.Modules;
		}

		/// <summary>
		/// The engine for <paramref name="typeSystem"/>, or null with the reason when the engine
		/// cannot describe it (a main module without a PE image, such as a synthetic one).
		/// </summary>
		public static RustEngine? For(IDecompilerTypeSystem typeSystem, out string? unsupportedReason)
		{
			var entry = engines.GetValue(typeSystem, Create);
			unsupportedReason = entry.UnsupportedReason;
			return entry.Engine;
		}

		static Entry Create(IDecompilerTypeSystem typeSystem)
		{
			if (typeSystem.MainModule is not MetadataModule { MetadataFile: PEFile file })
				return new Entry { UnsupportedReason = "the main module has no PE image" };
			if (!file.Reader.IsEntireImageAvailable)
				return new Entry { UnsupportedReason = "the main module's PE image is not in memory" };
			return new Entry { Engine = new RustEngine(typeSystem) };
		}

		/// <summary>
		/// The function of <paramref name="method"/> after the first <paramref name="ilPrefix"/>
		/// IL transforms. <see cref="RustStatus.Unsupported"/> leaves <paramref name="function"/>
		/// null; an engine error or panic throws <see cref="RustEngineException"/>.
		/// </summary>
		public RustStatus ReadFunction(MethodDefinitionHandle method, int ilPrefix, DecompilerSettings settings,
			IDebugInfoProvider? debugInfo, out ILFunction? function, out string? unsupportedReason)
		{
			function = null;
			unsupportedReason = null;
			uint flags = settings.UseRefLocalsForAccurateOrderOfEvaluation ? 1u : 0u;
			var bridge = debugInfo == null ? null : new RustDebugInfo(debugInfo);
			var status = NativeMethods.ilspy_session_read_function(session, (uint)MetadataTokens.GetToken(method), (uint)ilPrefix,
				flags, RustSettings.Bits(settings), IntPtr.Zero, bridge?.Delegate, out var buffer);
			GC.KeepAlive(bridge);
			byte[] bytes = NativeMethods.TakeBuffer(buffer);
			bridge?.ThrowIfFailed();
			switch (status)
			{
				case RustStatus.Ok:
					var (read, error) = IlAstMaterializer.ReadModule(bytes, typeSystem, modules)[MetadataTokens.GetRowNumber(method)];
					function = read ?? throw new RustEngineException(error ?? "no function");
					return status;
				case RustStatus.Unsupported:
					unsupportedReason = NativeMethods.LastError;
					return status;
				case RustStatus.Cancelled:
					throw new OperationCanceledException();
				default:
					throw new RustEngineException($"{status}: {NativeMethods.LastError}");
			}
		}
	}
}
