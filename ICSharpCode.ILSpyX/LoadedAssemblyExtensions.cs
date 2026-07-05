// Copyright (c) 2018 Siegfried Pammer
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
using System.IO;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpyX
{
	public static class LoadedAssemblyExtensions
	{
		/// <summary>
		/// This method creates a Cecil object model from a PEFile. It is intended as helper method for plugins.
		/// Note that this method is expensive and creates high memory pressure!
		/// Note that accessing the Cecil objects created by this method after an assembly has been unloaded by ILSpy
		/// might lead to <see cref="AccessViolationException"/> or similar.
		/// </summary>
		/// <remarks>Use only as last resort if there is something missing in the official ILSpy API.
		/// Consider creating an issue at https://github.com/icsharpcode/ILSpy/issues/new
		/// and discussing the problem with us.</remarks>
		public unsafe static Mono.Cecil.ModuleDefinition CreateCecilObjectModel(this PEFile file)
		{
			if (!file.Reader.IsEntireImageAvailable)
				throw new InvalidOperationException("Need full image to create Cecil object model!");
			var image = file.Reader.GetEntireImage();
			return Mono.Cecil.ModuleDefinition.ReadModule(new UnmanagedMemoryStream(image.Pointer, image.Length));
		}

		public static IAssemblyResolver GetAssemblyResolver(this MetadataFile file, bool loadOnDemand = true)
		{
			return GetLoadedAssembly(file).GetAssemblyResolver(loadOnDemand);
		}

		internal static IAssemblyResolver GetAssemblyResolver(this MetadataFile file, AssemblyListSnapshot snapshot, bool loadOnDemand = true)
		{
			return GetLoadedAssembly(file).GetAssemblyResolver(snapshot, loadOnDemand);
		}

		public static IDebugInfoProvider? GetDebugInfoOrNull(this MetadataFile file)
		{
			return GetLoadedAssembly(file).GetDebugInfoOrNull();
		}

		public static ICompilation? GetTypeSystemOrNull(this MetadataFile file)
		{
			return GetLoadedAssembly(file).GetTypeSystemOrNull();
		}

		public static ICompilation? GetTypeSystemWithDecompilerSettingsOrNull(this MetadataFile file, DecompilerSettings settings)
		{
			return GetLoadedAssembly(file).GetTypeSystemOrNull(DecompilerTypeSystem.GetOptions(settings));
		}

		public static LoadedAssembly GetLoadedAssembly(this MetadataFile file)
		{
			if (file == null)
				throw new ArgumentNullException(nameof(file));
			LoadedAssembly? loadedAssembly;
			lock (LoadedAssembly.loadedAssemblies)
			{
				if (!LoadedAssembly.loadedAssemblies.TryGetValue(file, out loadedAssembly))
					throw new ArgumentException("The specified file is not associated with a LoadedAssembly!");
			}
			return loadedAssembly;
		}
	}
}
