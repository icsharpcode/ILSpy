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
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Metadata
{
	public static class DotNetCorePathFinderExtensions
	{
		static readonly string PathPattern =
			@"(Reference Assemblies[/\\]Microsoft[/\\]Framework[/\\](?<type>.NETFramework)[/\\]v(?<version>[^/\\]+)[/\\])" +
			@"|((?<type>Microsoft\.NET)[/\\]assembly[/\\]GAC_(MSIL|32|64)[/\\])" +
			@"|((?<type>Microsoft\.NET)[/\\]Framework(64)?[/\\](?<version>[^/\\]+)[/\\])" +
			@"|(NuGetFallbackFolder[/\\](?<type>[^/\\]+)\\(?<version>[^/\\]+)([/\\].*)?[/\\]ref[/\\])" +
			@"|(shared[/\\](?<type>[^/\\]+)\\(?<version>[^/\\]+)([/\\].*)?[/\\])" +
			@"|(packs[/\\](?<type>[^/\\]+)\\(?<version>[^/\\]+)\\ref([/\\].*)?[/\\])";

		static readonly string RefPathPattern =
			@"(Reference Assemblies[/\\]Microsoft[/\\]Framework[/\\](?<type>.NETFramework)[/\\]v(?<version>[^/\\]+)[/\\])" +
			@"|(NuGetFallbackFolder[/\\](?<type>[^/\\]+)\\(?<version>[^/\\]+)([/\\].*)?[/\\]ref[/\\])" +
			@"|(packs[/\\](?<type>[^/\\]+)\\(?<version>[^/\\]+)\\ref([/\\].*)?[/\\])";

		public static string DetectTargetFrameworkId(this PEFile assembly)
		{
			return DetectTargetFrameworkId(assembly.Metadata, assembly.FileName);
		}

		public static string DetectTargetFrameworkId(this MetadataReader metadata, string assemblyPath = null)
		{
			if (metadata == null)
				throw new ArgumentNullException(nameof(metadata));

			const string TargetFrameworkAttributeName = "System.Runtime.Versioning.TargetFrameworkAttribute";

			foreach (var h in metadata.GetCustomAttributes(Handle.AssemblyDefinition))
			{
				try
				{
					var attribute = metadata.GetCustomAttribute(h);
					if (attribute.GetAttributeType(metadata).GetFullTypeName(metadata).ToString() != TargetFrameworkAttributeName)
						continue;
					var blobReader = metadata.GetBlobReader(attribute.Value);
					if (blobReader.ReadUInt16() == 0x0001)
					{
						return blobReader.ReadSerializedString()?.Replace(" ", "");
					}
				}
				catch (BadImageFormatException)
				{
					// ignore malformed attributes
				}
			}

			if (metadata.IsAssembly)
			{
				AssemblyDefinition assemblyDefinition = metadata.GetAssemblyDefinition();
				switch (metadata.GetString(assemblyDefinition.Name))
				{
					case "mscorlib":
						return $".NETFramework,Version=v{assemblyDefinition.Version.ToString(2)}";
					case "netstandard":
						return $".NETStandard,Version=v{assemblyDefinition.Version.ToString(2)}";
				}
			}

			foreach (var h in metadata.AssemblyReferences)
			{
				try
				{
					var r = metadata.GetAssemblyReference(h);
					if (r.PublicKeyOrToken.IsNil)
						continue;
					string version;
					switch (metadata.GetString(r.Name))
					{
						case "netstandard":
							version = r.Version.ToString(2);
							return $".NETStandard,Version=v{version}";
						case "System.Runtime":
							// System.Runtime.dll uses the following scheme:
							// 4.2.0 => .NET Core 2.0
							// 4.2.1 => .NET Core 2.1 / 3.0
							// 4.2.2 => .NET Core 3.1
							if (r.Version >= new Version(4, 2, 0))
							{
								version = "2.0";
								if (r.Version >= new Version(4, 2, 1))
								{
									version = "3.0";
								}
								if (r.Version >= new Version(4, 2, 2))
								{
									version = "3.1";
								}
								return $".NETCoreApp,Version=v{version}";
							}
							else
							{
								continue;
							}
						case "mscorlib":
							version = r.Version.ToString(2);
							return $".NETFramework,Version=v{version}";
					}
				}
				catch (BadImageFormatException)
				{
					// ignore malformed references
				}
			}

			// Optionally try to detect target version through assembly path as a fallback (use case: reference assemblies)
			if (assemblyPath != null)
			{
				/*
				 * Detected path patterns (examples):
				 * 
				 * - .NETFramework -> C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\mscorlib.dll
				 * - .NETCore      -> C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.netcore.app\2.1.0\ref\netcoreapp2.1\System.Console.dll
				 * - .NETStandard  -> C:\Program Files\dotnet\sdk\NuGetFallbackFolder\netstandard.library\2.0.3\build\netstandard2.0\ref\netstandard.dll
				 */
				var pathMatch = Regex.Match(assemblyPath, PathPattern,
					RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
				string version;
				if (pathMatch.Success)
				{
					var type = pathMatch.Groups["type"].Value;
					version = pathMatch.Groups["version"].Value;
					if (string.IsNullOrEmpty(version))
						version = metadata.MetadataVersion;
					if (string.IsNullOrEmpty(version))
						version = "4.0";
					version = version.TrimStart('v');

					if (type == "Microsoft.NET" || type == ".NETFramework")
					{
						return $".NETFramework,Version=v{version.Substring(0, Math.Min(3, version.Length))}";
					}
					else if (type.IndexOf("netcore", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						return $".NETCoreApp,Version=v{version}";
					}
					else if (type.IndexOf("netstandard", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						return $".NETStandard,Version=v{version}";
					}
				}
				else
				{
					version = metadata.MetadataVersion;
					if (string.IsNullOrEmpty(version))
						version = "4.0";
					version = version.TrimStart('v');
					return $".NETFramework,Version=v{version.Substring(0, Math.Min(3, version.Length))}";
				}
			}

			return string.Empty;
		}

		public static bool IsReferenceAssembly(this PEFile assembly)
		{
			return IsReferenceAssembly(assembly.Reader, assembly.FileName);
		}

		public static bool IsReferenceAssembly(this PEReader assembly, string assemblyPath)
		{
			if (assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			var metadata = assembly.GetMetadataReader();
			if (metadata.GetCustomAttributes(Handle.AssemblyDefinition).HasKnownAttribute(metadata, KnownAttribute.ReferenceAssembly))
				return true;

			// Try to detect reference assembly through specific path pattern
			var refPathMatch = Regex.Match(assemblyPath, RefPathPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			return refPathMatch.Success;
		}

		public static string DetectRuntimePack(this PEFile assembly)
		{
			if (assembly is null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			var metadata = assembly.Metadata;

			foreach (var r in metadata.AssemblyReferences)
			{
				var reference = metadata.GetAssemblyReference(r);

				if (reference.PublicKeyOrToken.IsNil)
					continue;

				if (metadata.StringComparer.Equals(reference.Name, "WindowsBase"))
				{
					return "Microsoft.WindowsDesktop.App";
				}

				if (metadata.StringComparer.Equals(reference.Name, "PresentationFramework"))
				{
					return "Microsoft.WindowsDesktop.App";
				}

				if (metadata.StringComparer.Equals(reference.Name, "PresentationCore"))
				{
					return "Microsoft.WindowsDesktop.App";
				}

				// TODO add support for ASP.NET Core
			}

			return "Microsoft.NETCore.App";
		}
	}
}
