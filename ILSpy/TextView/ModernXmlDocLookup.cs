// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.Metadata;

namespace ILSpy.TextView
{
	/// <summary>
	/// Fallback XML-documentation locator for modern .NET (.NET 5+). The shared
	/// <see cref="XmlDocLoader"/> only knows about the .NET Framework reference-assemblies
	/// paths (v1.0 – v4.8.1) and the &quot;.xml beside the .dll&quot; convention. Modern
	/// runtime DLLs (<c>System.Private.CoreLib.dll</c>, <c>System.Linq.dll</c>, …) live
	/// under <c>dotnet/shared/Microsoft.NETCore.App/&lt;version&gt;/</c> with no
	/// matching XML; the actual XML files ship in the parallel reference pack under
	/// <c>dotnet/packs/Microsoft.NETCore.App.Ref/&lt;version&gt;/ref/&lt;tfm&gt;/</c>.
	/// This class walks that structure for a loaded <see cref="MetadataFile"/> and exposes
	/// an aggregate provider that searches every <c>.xml</c> in the matching ref-pack tfm
	/// folder. Cached per <see cref="MetadataFile"/> so the directory scan only happens
	/// once per loaded assembly.
	/// </summary>
	public static class ModernXmlDocLookup
	{
		// Keep one cache per loaded MetadataFile so re-hovering the same assembly doesn't
		// re-scan disk. The ConditionalWeakTable lets entries drop when the assembly is
		// unloaded — matches XmlDocLoader's caching strategy.
		static readonly ConditionalWeakTable<MetadataFile, Provider> cache = new();

		/// <summary>
		/// Returns an aggregate XML-doc provider that walks every XML in the modern .NET
		/// ref pack matching <paramref name="module"/>'s runtime, or <c>null</c> when the
		/// assembly isn't part of a modern .NET shared-framework layout (e.g. the user
		/// opened a NuGet-only assembly with its XML already next to it — that path is
		/// handled by <see cref="XmlDocLoader"/>).
		/// </summary>
		public static Provider? TryGetProvider(MetadataFile module)
		{
			if (module == null)
				return null;
			lock (cache)
			{
				if (cache.TryGetValue(module, out var cached))
					return cached.IsEmpty ? null : cached;
				var built = Build(module);
				cache.Add(module, built ?? Provider.Empty);
				return built;
			}
		}

		static Provider? Build(MetadataFile module)
		{
			var refPackDir = ResolveRefPackTfmDirectory(module.FileName);
			if (refPackDir == null)
				return null;
			var xmls = SafeGetXmlFiles(refPackDir);
			if (xmls.Count == 0)
				return null;
			var providers = new List<XmlDocumentationProvider>(xmls.Count);
			foreach (var x in xmls)
			{
				try
				{
					providers.Add(new XmlDocumentationProvider(x));
				}
				catch
				{
					// One bad XML doesn't disable the rest — skip and continue.
				}
			}
			return providers.Count > 0 ? new Provider(providers) : null;
		}

		/// <summary>
		/// Maps a runtime-DLL path like <c>X:\Program Files\dotnet\shared\Microsoft.NETCore.App\10.0.0\System.Private.CoreLib.dll</c>
		/// to the matching ref-pack tfm directory <c>X:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\10.0.0\ref\net10.0</c>.
		/// Returns <c>null</c> when the assembly isn't laid out under
		/// <c>shared/Microsoft.NETCore.App/&lt;version&gt;/</c>.
		/// </summary>
		internal static string? ResolveRefPackTfmDirectory(string? assemblyFileName)
		{
			if (string.IsNullOrEmpty(assemblyFileName))
				return null;
			var versionDir = Path.GetDirectoryName(assemblyFileName);
			if (versionDir == null)
				return null;
			var sharedFramework = Path.GetDirectoryName(versionDir);
			if (sharedFramework == null || !string.Equals(Path.GetFileName(sharedFramework), "Microsoft.NETCore.App", StringComparison.OrdinalIgnoreCase))
				return null;
			var sharedDir = Path.GetDirectoryName(sharedFramework);
			if (sharedDir == null || !string.Equals(Path.GetFileName(sharedDir), "shared", StringComparison.OrdinalIgnoreCase))
				return null;
			var dotnetRoot = Path.GetDirectoryName(sharedDir);
			if (dotnetRoot == null)
				return null;
			var version = Path.GetFileName(versionDir);
			var refPackRoot = Path.Combine(dotnetRoot, "packs", "Microsoft.NETCore.App.Ref", version, "ref");
			if (!Directory.Exists(refPackRoot))
				return null;
			// One tfm folder per pack version (e.g. "net10.0"). Pick the lexicographically
			// latest — multiple tfms only show up across versions, not within one ref-pack
			// version. Lexicographic order matches version order for "netN.M" formats up
			// to N=9 (and net10.0 beats net9.0 by string compare too because "1" < "9" but
			// length wins via the "0." prefix... actually no, this fails for net10.0).
			// Sort by parsed version-number triple to be safe.
			var tfmDir = Directory.GetDirectories(refPackRoot)
				.OrderByDescending(d => ParseTfm(Path.GetFileName(d)))
				.FirstOrDefault();
			return tfmDir;
		}

		static (int Major, int Minor) ParseTfm(string tfm)
		{
			// "net10.0" → (10, 0). Anything we don't recognise sorts last (negative).
			if (!tfm.StartsWith("net", StringComparison.OrdinalIgnoreCase))
				return (-1, -1);
			var rest = tfm.Substring(3);
			var dot = rest.IndexOf('.');
			if (dot < 0)
				return (-1, -1);
			if (!int.TryParse(rest.AsSpan(0, dot), out int major))
				return (-1, -1);
			if (!int.TryParse(rest.AsSpan(dot + 1), out int minor))
				return (-1, -1);
			return (major, minor);
		}

		static IReadOnlyList<string> SafeGetXmlFiles(string directory)
		{
			try
			{
				return Directory.GetFiles(directory, "*.xml");
			}
			catch
			{
				return Array.Empty<string>();
			}
		}

		/// <summary>
		/// Aggregate XML-doc provider that probes each contributing
		/// <see cref="XmlDocumentationProvider"/> in turn. First match wins — id strings are
		/// unique across the ref pack (a type forwarded into one assembly is documented in
		/// exactly one ref-pack XML).
		/// </summary>
		public sealed class Provider
		{
			internal static readonly Provider Empty = new(Array.Empty<XmlDocumentationProvider>());

			readonly IReadOnlyList<XmlDocumentationProvider> providers;

			internal Provider(IReadOnlyList<XmlDocumentationProvider> providers)
			{
				this.providers = providers;
			}

			internal bool IsEmpty => providers.Count == 0;

			public string? GetDocumentation(string idString)
			{
				foreach (var p in providers)
				{
					var doc = p.GetDocumentation(idString);
					if (!string.IsNullOrEmpty(doc))
						return doc;
				}
				return null;
			}
		}
	}
}
