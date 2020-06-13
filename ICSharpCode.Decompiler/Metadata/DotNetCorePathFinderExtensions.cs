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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace ICSharpCode.Decompiler.Metadata
{
	public static class DotNetCorePathFinderExtensions
	{
		static readonly string RefPathPattern =
			@"(Reference Assemblies[/\\]Microsoft[/\\]Framework[/\\](?<type>.NETFramework)[/\\]v(?<version>[^/\\]+)[/\\])" +
			@"|((?<type>Microsoft\.NET)[/\\]assembly[/\\]GAC_(MSIL|32|64)[/\\])" +
			@"|((?<type>Microsoft\.NET)[/\\]Framework(64)?[/\\](?<version>[^/\\]+)[/\\])" +
			@"|(NuGetFallbackFolder[/\\](?<type>[^/\\]+)\\(?<version>[^/\\]+)([/\\].*)?[/\\]ref[/\\])" +
			@"|(shared[/\\](?<type>[^/\\]+)\\(?<version>[^/\\]+)([/\\].*)?[/\\])";

		public static string DetectTargetFrameworkId(this PEFile assembly)
		{
			return DetectTargetFrameworkId(assembly.Reader, assembly.FileName);
		}

		public static string DetectTargetFrameworkId(this PEReader assembly, string assemblyPath = null)
		{
			if (assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			const string TargetFrameworkAttributeName = "System.Runtime.Versioning.TargetFrameworkAttribute";
			var reader = assembly.GetMetadataReader();

			foreach (var h in reader.GetCustomAttributes(Handle.AssemblyDefinition)) {
				var attribute = reader.GetCustomAttribute(h);
				if (attribute.GetAttributeType(reader).GetFullTypeName(reader).ToString() != TargetFrameworkAttributeName)
					continue;
				var blobReader = reader.GetBlobReader(attribute.Value);
				if (blobReader.ReadUInt16() == 0x0001) {
					return blobReader.ReadSerializedString();
				}
			}

			foreach (var h in reader.AssemblyReferences) {
				var r = reader.GetAssemblyReference(h);
				if (r.PublicKeyOrToken.IsNil)
					continue;
				string version;
				switch (reader.GetString(r.Name)) {
					case "netstandard":
						version = r.Version.ToString(3);
						return $".NETStandard,Version=v{version}";
					case "System.Runtime":
						// System.Runtime.dll uses the following scheme:
						// 4.2.0 => .NET Core 2.0
						// 4.2.1 => .NET Core 2.1 / 3.0
						// 4.2.2 => .NET Core 3.1
						if (r.Version >= new Version(4, 2, 0)) {
							version = "2.0";
							if (r.Version >= new Version(4, 2, 1)) {
								version = "3.0";
							}
							if (r.Version >= new Version(4, 2, 2)) {
								version = "3.1";
							}
							return $".NETCoreApp,Version=v{version}";
						} else {
							continue;
						}
					case "mscorlib":
						version = r.Version.ToString(2);
						return $".NETFramework,Version=v{version}";
				}
			}

			// Optionally try to detect target version through assembly path as a fallback (use case: reference assemblies)
			if (assemblyPath != null) {
				/*
				 * Detected path patterns (examples):
				 * 
				 * - .NETFramework -> C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\mscorlib.dll
				 * - .NETCore      -> C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.netcore.app\2.1.0\ref\netcoreapp2.1\System.Console.dll
				 * - .NETStandard  -> C:\Program Files\dotnet\sdk\NuGetFallbackFolder\netstandard.library\2.0.3\build\netstandard2.0\ref\netstandard.dll
				 */
				var pathMatch = Regex.Match(assemblyPath, RefPathPattern,
					RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
				if (pathMatch.Success) {
					var type = pathMatch.Groups["type"].Value;
					var version = pathMatch.Groups["version"].Value;
					if (string.IsNullOrEmpty(version))
						version = reader.MetadataVersion;

					if (type == "Microsoft.NET" || type == ".NETFramework") {
						return $".NETFramework,Version=v{version.TrimStart('v').Substring(0, 3)}";
					} else if (type.IndexOf("netcore", StringComparison.OrdinalIgnoreCase) >= 0) {
						return $".NETCoreApp,Version=v{version}";
					} else if (type.IndexOf("netstandard", StringComparison.OrdinalIgnoreCase) >= 0) {
						return $".NETStandard,Version=v{version}";
					}
				} else {
					return $".NETFramework,Version={reader.MetadataVersion.Substring(0, 4)}";
				}
			}

			return string.Empty;
		}
	}

	public class ReferenceLoadInfo
	{
		readonly Dictionary<string, UnresolvedAssemblyNameReference> loadedAssemblyReferences = new Dictionary<string, UnresolvedAssemblyNameReference>();

		public void AddMessage(string fullName, MessageKind kind, string message)
		{
			lock (loadedAssemblyReferences) {
				if (!loadedAssemblyReferences.TryGetValue(fullName, out var referenceInfo)) {
					referenceInfo = new UnresolvedAssemblyNameReference(fullName);
					loadedAssemblyReferences.Add(fullName, referenceInfo);
				}
				referenceInfo.Messages.Add((kind, message));
			}
		}

		public void AddMessageOnce(string fullName, MessageKind kind, string message)
		{
			lock (loadedAssemblyReferences) {
				if (!loadedAssemblyReferences.TryGetValue(fullName, out var referenceInfo)) {
					referenceInfo = new UnresolvedAssemblyNameReference(fullName);
					loadedAssemblyReferences.Add(fullName, referenceInfo);
					referenceInfo.Messages.Add((kind, message));
				} else {
					var lastMsg = referenceInfo.Messages.LastOrDefault();
					if (kind != lastMsg.Item1 && message != lastMsg.Item2)
						referenceInfo.Messages.Add((kind, message));
				}
			}
		}

		public bool TryGetInfo(string fullName, out UnresolvedAssemblyNameReference info)
		{
			lock (loadedAssemblyReferences) {
				return loadedAssemblyReferences.TryGetValue(fullName, out info);
			}
		}

		public IReadOnlyList<UnresolvedAssemblyNameReference> Entries {
			get {
				lock (loadedAssemblyReferences) {
					return loadedAssemblyReferences.Values.ToList();
				}
			}
		}

		public bool HasErrors {
			get {
				lock (loadedAssemblyReferences) {
					return loadedAssemblyReferences.Any(i => i.Value.HasErrors);
				}
			}
		}
	}
}
