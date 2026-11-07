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
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// Describes a managed compilation to the engine: its metadata modules in compilation order
	/// (main module first) and, per AssemblyRef and ModuleRef row, the module the managed type
	/// system resolves it to. The engine resolves type names only within this table, so both
	/// sides agree on the reference closure by construction.
	/// </summary>
	internal sealed class RustModuleTable : IDisposable
	{
		readonly List<GCHandle> pins = new();
		readonly RustModule[] modules;
		readonly uint options;

		public RustModuleTable(ICompilation compilation)
		{
			// Modules without metadata (the synthetic MinimalCorlib) have nothing to pass; names
			// that resolve only to them stay unknown in the engine.
			var metadataModules = compilation.Modules.OfType<MetadataModule>()
				.Where(m => m.MetadataFile is PEFile).ToList();
			int IndexOf(IModule? m) => m is MetadataModule mm ? metadataModules.IndexOf(mm) : -1;
			options = (uint)((MetadataModule)compilation.MainModule).TypeSystemOptions;
			modules = new RustModule[metadataModules.Count];
			for (int i = 0; i < metadataModules.Count; i++)
			{
				var module = metadataModules[i];
				var metadata = module.MetadataFile.Metadata;
				var image = ((PEFile)module.MetadataFile).Reader.GetEntireImage();
				var asmRefs = Enumerable.Range(1, metadata.GetTableRowCount(TableIndex.AssemblyRef))
					.Select(row => IndexOf(module.ResolveModule(MetadataTokens.AssemblyReferenceHandle(row)))).ToArray();
				var modRefs = Enumerable.Range(1, metadata.GetTableRowCount(TableIndex.ModuleRef))
					.Select(row => IndexOf(module.ResolveModule(MetadataTokens.ModuleReferenceHandle(row)))).ToArray();
				byte[] name = Encoding.UTF8.GetBytes(module.AssemblyName ?? "");
				unsafe
				{
					modules[i] = new RustModule {
						Image = image.Pointer,
						ImageLength = (UIntPtr)image.Length,
						AssemblyName = (byte*)Pin(name),
						AssemblyNameLength = (UIntPtr)name.Length,
						AssemblyRefs = (int*)Pin(asmRefs),
						AssemblyRefCount = (UIntPtr)asmRefs.Length,
						ModuleRefs = (int*)Pin(modRefs),
						ModuleRefCount = (UIntPtr)modRefs.Length,
					};
				}
			}
		}

		IntPtr Pin(Array array)
		{
			var handle = GCHandle.Alloc(array, GCHandleType.Pinned);
			pins.Add(handle);
			return handle.AddrOfPinnedObject();
		}

		/// <summary>
		/// The engine's type-system listing of the main module (rust/ilspy-decompiler/src/typesystem/listing.rs).
		/// Test support only.
		/// </summary>
		public unsafe string DebugTypeListing()
		{
			RustStatus status;
			RustBuffer buffer;
			fixed (RustModule* p = modules)
				status = NativeMethods.ilspy_debug_type_listing(p, (UIntPtr)modules.Length, options, out buffer);
			byte[] bytes = NativeMethods.TakeBuffer(buffer);
			if (status != RustStatus.Ok)
				throw new InvalidOperationException($"ilspy_debug_type_listing: {status}: {NativeMethods.LastError}");
			return Encoding.UTF8.GetString(bytes);
		}

		public void Dispose()
		{
			foreach (var handle in pins)
				handle.Free();
			pins.Clear();
		}
	}
}
