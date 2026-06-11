// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.Documentation
{
	/// <summary>
	/// Helps finding and loading .xml documentation.
	/// </summary>
	public static class XmlDocLoader
	{
		static readonly Lazy<XmlDocumentationProvider> mscorlibDocumentation = new Lazy<XmlDocumentationProvider>(LoadMscorlibDocumentation);
		static readonly ConditionalWeakTable<MetadataFile, XmlDocumentationProvider> cache = new();

		static XmlDocumentationProvider LoadMscorlibDocumentation()
		{
			string xmlDocFile = FindXmlDocumentation("mscorlib.dll", TargetRuntime.Net_4_0)
				?? FindXmlDocumentation("mscorlib.dll", TargetRuntime.Net_2_0);
			if (xmlDocFile != null)
				return new XmlDocumentationProvider(xmlDocFile);
			// On modern .NET there is no .NET Framework reference-assembly documentation;
			// fall back to the ref pack parallel to the runtime hosting us (corlib lives in
			// dotnet/shared/Microsoft.NETCore.App/<version>/, the docs in the matching
			// dotnet/packs/Microsoft.NETCore.App.Ref/<version>/ref/<tfm>/ directory).
			return TryLoadModernRefPackDocumentation(typeof(object).Assembly.Location);
		}

		public static XmlDocumentationProvider MscorlibDocumentation {
			get { return mscorlibDocumentation.Value; }
		}

		public static XmlDocumentationProvider LoadDocumentation(MetadataFile module)
		{
			if (module == null)
				throw new ArgumentNullException(nameof(module));
			lock (cache)
			{
				if (!cache.TryGetValue(module, out XmlDocumentationProvider xmlDoc))
				{
					string xmlDocFile = LookupLocalizedXmlDoc(module.FileName);
					if (xmlDocFile == null)
					{
						xmlDocFile = FindXmlDocumentation(Path.GetFileName(module.FileName), module.GetRuntime());
					}
					if (xmlDocFile != null)
					{
						xmlDoc = new XmlDocumentationProvider(xmlDocFile);
					}
					else
					{
						// Last resort for modern .NET (.NET 5+): runtime DLLs ship under
						// dotnet/shared/Microsoft.NETCore.App/<version>/ with no .xml beside
						// them. The matching XML lives in the parallel ref pack under
						// dotnet/packs/Microsoft.NETCore.App.Ref/<version>/ref/<tfm>/. Aggregate
						// every *.xml in the matching tfm folder into a single provider so
						// type-forwarded entities (System.String docs live in System.Runtime.xml
						// but the metadata token comes from System.Private.CoreLib.dll) resolve
						// transparently.
						xmlDoc = TryLoadModernRefPackDocumentation(module.FileName);
					}
					cache.Add(module, xmlDoc); // cache null misses too — avoids re-scanning disk
				}
				return xmlDoc;
			}
		}

		static XmlDocumentationProvider TryLoadModernRefPackDocumentation(string assemblyFileName)
		{
			var refPackTfmDir = ResolveModernRefPackTfmDirectory(assemblyFileName);
			if (refPackTfmDir == null)
				return null;
			List<XmlDocumentationProvider> providers = null;
			foreach (var xmlPath in SafeGetXmlFiles(refPackTfmDir))
			{
				XmlDocumentationProvider p;
				try
				{
					p = new XmlDocumentationProvider(xmlPath);
				}
				catch
				{
					// One bad XML doesn't disable the rest — skip and continue.
					continue;
				}
				(providers ??= new List<XmlDocumentationProvider>()).Add(p);
			}
			return providers is { Count: > 0 } ? new AggregatingXmlDocumentationProvider(providers) : null;
		}

		/// <summary>
		/// Maps a runtime-DLL path like
		/// <c>X:\Program Files\dotnet\shared\Microsoft.NETCore.App\10.0.0\System.Private.CoreLib.dll</c>
		/// to the matching ref-pack tfm directory
		/// <c>X:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\10.0.0\ref\net10.0</c>.
		/// Returns <c>null</c> when the assembly isn't laid out under
		/// <c>shared/Microsoft.NETCore.App/&lt;version&gt;/</c> or when the parallel ref pack
		/// isn't installed.
		/// </summary>
		internal static string ResolveModernRefPackTfmDirectory(string assemblyFileName)
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
			var packRoot = Path.Combine(dotnetRoot, "packs", "Microsoft.NETCore.App.Ref");
			if (!Directory.Exists(packRoot))
				return null;
			// The targeting (ref) pack version usually differs from the installed runtime patch
			// version -- e.g. runtime shared/.../10.0.8 against pack .../10.0.0, or a relocated CI
			// install whose only ref pack is a different feature band. Prefer an exact match, then
			// the newest pack sharing the runtime's major.minor, then the newest pack present.
			var packVersionDir = SelectRefPackVersionDirectory(packRoot, Path.GetFileName(versionDir));
			if (packVersionDir == null)
				return null;
			var refPackRoot = Path.Combine(packVersionDir, "ref");
			if (!Directory.Exists(refPackRoot))
				return null;
			// One tfm folder per pack version (e.g. "net10.0"). Sort by parsed
			// (Major, Minor) — lexicographic compare puts "net10.0" behind "net9.0" because
			// '1' < '9'. Multiple tfms only show up across pack versions, not within one.
			return Directory.GetDirectories(refPackRoot)
				.OrderByDescending(d => ParseTfm(Path.GetFileName(d)))
				.FirstOrDefault();
		}

		// Picks the ref-pack version directory most appropriate for the running runtime: an exact
		// version match wins; otherwise the newest pack sharing the runtime's major.minor, then the
		// newest pack of the same major, then the newest pack present. Pre-release suffixes are
		// ignored when comparing.
		static string SelectRefPackVersionDirectory(string packRoot, string runtimeVersion)
		{
			var exact = Path.Combine(packRoot, runtimeVersion);
			if (Directory.Exists(exact))
				return exact;
			var rt = ParseVersionPrefix(runtimeVersion);
			string best = null;
			(int Major, int Minor, int Patch) bestVer = (-1, -1, -1);
			int bestRank = -1;
			foreach (var dir in Directory.GetDirectories(packRoot))
			{
				var v = ParseVersionPrefix(Path.GetFileName(dir));
				int rank = (v.Major == rt.Major && v.Minor == rt.Minor) ? 2 : (v.Major == rt.Major ? 1 : 0);
				if (rank > bestRank || (rank == bestRank && CompareVersion(v, bestVer) > 0))
				{
					best = dir;
					bestVer = v;
					bestRank = rank;
				}
			}
			return best;
		}

		static (int Major, int Minor, int Patch) ParseVersionPrefix(string version)
		{
			if (string.IsNullOrEmpty(version))
				return (-1, -1, -1);
			int dash = version.IndexOf('-');
			var core = dash >= 0 ? version.Substring(0, dash) : version;
			var parts = core.Split('.');
			int Part(int i) => i < parts.Length && int.TryParse(parts[i], out int n) ? n : 0;
			return (Part(0), Part(1), Part(2));
		}

		static int CompareVersion((int Major, int Minor, int Patch) a, (int Major, int Minor, int Patch) b)
		{
			if (a.Major != b.Major)
				return a.Major.CompareTo(b.Major);
			if (a.Minor != b.Minor)
				return a.Minor.CompareTo(b.Minor);
			return a.Patch.CompareTo(b.Patch);
		}

		static (int Major, int Minor) ParseTfm(string tfm)
		{
			// Avoid Span overloads — the shared library targets older TFMs that don't have
			// int.TryParse(ReadOnlySpan<char>, out int).
			if (tfm == null || !tfm.StartsWith("net", StringComparison.OrdinalIgnoreCase))
				return (-1, -1);
			var rest = tfm.Substring(3);
			var dot = rest.IndexOf('.');
			if (dot < 0)
				return (-1, -1);
			if (!int.TryParse(rest.Substring(0, dot), out int major))
				return (-1, -1);
			if (!int.TryParse(rest.Substring(dot + 1), out int minor))
				return (-1, -1);
			return (major, minor);
		}

		static IEnumerable<string> SafeGetXmlFiles(string directory)
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
		/// Aggregates several <see cref="XmlDocumentationProvider"/> instances behind one
		/// <see cref="XmlDocumentationProvider"/> surface. Used by
		/// <see cref="TryLoadModernRefPackDocumentation"/> because in modern .NET an
		/// entity's metadata-token-bearing assembly (<c>System.Private.CoreLib.dll</c>) is
		/// different from the assembly whose XML contains its docs (<c>System.Runtime.xml</c>):
		/// we have to probe every ref-pack XML for the same id string until one matches.
		/// First non-empty answer wins; id strings are unique across the pack.
		/// </summary>
		sealed class AggregatingXmlDocumentationProvider : XmlDocumentationProvider
		{
			readonly IReadOnlyList<XmlDocumentationProvider> providers;

			public AggregatingXmlDocumentationProvider(IReadOnlyList<XmlDocumentationProvider> providers) : base()
			{
				this.providers = providers;
			}

			public override string GetDocumentation(string key)
			{
				if (key == null)
					throw new ArgumentNullException(nameof(key));
				foreach (var p in providers)
				{
					var doc = p.GetDocumentation(key);
					if (!string.IsNullOrEmpty(doc))
						return doc;
				}
				return null;
			}
		}

		static readonly string referenceAssembliesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Reference Assemblies\Microsoft\\Framework");
		static readonly string frameworkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework");

		static string FindXmlDocumentation(string assemblyFileName, TargetRuntime runtime)
		{
			string fileName;
			switch (runtime)
			{
				case TargetRuntime.Net_1_0:
					fileName = LookupLocalizedXmlDoc(Path.Combine(frameworkPath, "v1.0.3705", assemblyFileName));
					break;
				case TargetRuntime.Net_1_1:
					fileName = LookupLocalizedXmlDoc(Path.Combine(frameworkPath, "v1.1.4322", assemblyFileName));
					break;
				case TargetRuntime.Net_2_0:
					fileName = LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, "v3.5", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v3.5\Profile\Client", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, "v3.0", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(frameworkPath, "v2.0.50727", assemblyFileName));
					break;
				case TargetRuntime.Net_4_0:
				default:
					fileName = LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.8.1", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.8", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.7.2", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.7.1", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.7", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.6.2", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.6.1", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.6", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.5.2", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.5.1", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.5", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(referenceAssembliesPath, @".NETFramework\v4.0", assemblyFileName))
						?? LookupLocalizedXmlDoc(Path.Combine(frameworkPath, "v4.0.30319", assemblyFileName));
					break;
			}
			return fileName;
		}

		/// <summary>
		/// Given the assembly file name, looks up the XML documentation file name.
		/// Returns null if no XML documentation file is found.
		/// </summary>
		internal static string LookupLocalizedXmlDoc(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;

			string xmlFileName = Path.ChangeExtension(fileName, ".xml");

			CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
			string localizedXmlDocFile = GetLocalizedName(xmlFileName, currentCulture.Name);
			string localizedXmlDocFallbackFile = GetLocalizedName(xmlFileName, currentCulture.TwoLetterISOLanguageName);

			//Debug.WriteLine("Try find XMLDoc @" + localizedXmlDocFile);
			if (File.Exists(localizedXmlDocFile))
			{
				return localizedXmlDocFile;
			}
			//Debug.WriteLine("Try find XMLDoc @" + localizedXmlDocFallbackFile);
			if (File.Exists(localizedXmlDocFallbackFile))
			{
				return localizedXmlDocFallbackFile;
			}
			//Debug.WriteLine("Try find XMLDoc @" + xmlFileName);
			if (File.Exists(xmlFileName))
			{
				return xmlFileName;
			}
			if (currentCulture.TwoLetterISOLanguageName != "en")
			{
				string englishXmlDocFile = GetLocalizedName(xmlFileName, "en");
				//Debug.WriteLine("Try find XMLDoc @" + englishXmlDocFile);
				if (File.Exists(englishXmlDocFile))
				{
					return englishXmlDocFile;
				}
			}
			return null;
		}

		private static string GetLocalizedName(string fileName, string language)
		{
			return Path.Combine(Path.GetDirectoryName(fileName), language, Path.GetFileName(fileName));
		}
	}
}
