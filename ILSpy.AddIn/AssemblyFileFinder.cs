using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Util;
using Mono.Cecil;

namespace ICSharpCode.ILSpy.AddIn
{
	public class AssemblyFileFinder
	{
		public static string FindAssemblyFile(AssemblyDefinition assemblyDefinition, string assemblyFile)
		{
			var assemblyName = assemblyDefinition.Name;

			var detectedTargetFramework = assemblyDefinition.DetectTargetFrameworkId(assemblyFile);
			if (string.IsNullOrEmpty(detectedTargetFramework)) {
				// Without a target framework id it makes no sense to continue
				return null;
			}

			var targetFramework = detectedTargetFramework.Split(new[] { ",Version=v" }, StringSplitOptions.None);
			string file = null;
			switch (targetFramework[0]) {
				case ".NETCoreApp":
				case ".NETStandard":
					if (targetFramework.Length != 2)
						return FindAssemblyFromGAC(assemblyDefinition);
					var version = targetFramework[1].Length == 3 ? targetFramework[1] + ".0" : targetFramework[1];
					var dotNetCorePathFinder = new DotNetCorePathFinder(assemblyFile, detectedTargetFramework, version);
					file = dotNetCorePathFinder.TryResolveDotNetCore(assemblyName);
					if (file != null)
						return file;
					return FindAssemblyFromGAC(assemblyDefinition);
				default:
					return FindAssemblyFromGAC(assemblyDefinition);
			}
		}

		static string FindAssemblyFromGAC(AssemblyDefinition assemblyDefinition)
		{
			return GacInterop.FindAssemblyInNetGac(assemblyDefinition.Name);
		}

		static readonly string RefPathPattern = @"NuGetFallbackFolder[/\\][^/\\]+[/\\][^/\\]+[/\\]ref[/\\]";

		public static bool IsReferenceAssembly(AssemblyDefinition assemblyDef, string assemblyFile)
		{
			if (assemblyDef.CustomAttributes.Any(ca => ca.AttributeType.FullName == "System.Runtime.CompilerServices.ReferenceAssemblyAttribute"))
				return true;

			// Try to detect reference assembly through specific path pattern
			var refPathMatch = Regex.Match(assemblyFile, RefPathPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			return refPathMatch.Success;
		}

	}
}
