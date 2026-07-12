// Copyright (c) 2018 Andreas Weizel
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
using System.Linq;
using System.Text.RegularExpressions;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.AddIn
{
	public class AssemblyFileFinder
	{
		public static string FindAssemblyFile(Mono.Cecil.AssemblyDefinition assemblyDefinition, string assemblyFile)
		{
			string tfi = DetectTargetFrameworkId(assemblyDefinition, assemblyFile);
			UniversalAssemblyResolver assemblyResolver;
			if (IsReferenceAssembly(assemblyDefinition, assemblyFile))
			{
				assemblyResolver = new UniversalAssemblyResolver(null, throwOnError: false, tfi);
			}
			else
			{
				assemblyResolver = new UniversalAssemblyResolver(assemblyFile, throwOnError: false, tfi);
			}

			return assemblyResolver.FindAssemblyFile(AssemblyNameReference.Parse(assemblyDefinition.Name.FullName));
		}

		static readonly string RefPathPattern = @"NuGetFallbackFolder[/\\][^/\\]+[/\\][^/\\]+[/\\]ref[/\\]";

		public static bool IsReferenceAssembly(Mono.Cecil.AssemblyDefinition assemblyDef, string assemblyFile)
		{
			if (assemblyDef.CustomAttributes.Any(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.ReferenceAssemblyAttribute"))
				return true;

			// Try to detect reference assembly through specific path pattern
			var refPathMatch = Regex.Match(assemblyFile, RefPathPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			return refPathMatch.Success;
		}

		static readonly string DetectTargetFrameworkIdRefPathPattern =
			@"(Reference Assemblies[/\\]Microsoft[/\\]Framework[/\\](?<1>.NETFramework)[/\\]v(?<2>[^/\\]+)[/\\])" +
			@"|((NuGetFallbackFolder|packs|.nuget[/\\]packages)[/\\](?<1>[^/\\]+)\\(?<2>[^/\\]+)([/\\].*)?[/\\]ref[/\\])";

		public static string DetectTargetFrameworkId(Mono.Cecil.AssemblyDefinition assembly, string assemblyPath = null)
		{
			if (assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			const string TargetFrameworkAttributeName = "System.Runtime.Versioning.TargetFrameworkAttribute";

			foreach (var attribute in assembly.CustomAttributes)
			{
				if (attribute.AttributeType.FullName != TargetFrameworkAttributeName)
					continue;
				if (attribute.HasConstructorArguments)
				{
					if (attribute.ConstructorArguments[0].Value is string value)
						return value;
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
				 *                 -> C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\3.0.0\ref\netcoreapp3.0\System.Runtime.Extensions.dll
				 * - .NETStandard  -> C:\Program Files\dotnet\sdk\NuGetFallbackFolder\netstandard.library\2.0.3\build\netstandard2.0\ref\netstandard.dll
				 */
				var pathMatch = Regex.Match(assemblyPath, DetectTargetFrameworkIdRefPathPattern,
					RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
				if (pathMatch.Success)
				{
					var type = pathMatch.Groups[1].Value;
					var version = pathMatch.Groups[2].Value;

					if (type == ".NETFramework")
					{
						return $".NETFramework,Version=v{version}";
					}
					else if (type.ToLower().Contains("netcore"))
					{
						return $".NETCoreApp,Version=v{version}";
					}
					else if (type.ToLower().Contains("netstandard"))
					{
						return $".NETStandard,Version=v{version}";
					}
				}
			}

			return string.Empty;
		}
	}
}
