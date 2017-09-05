using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using Mono.Cecil;
using Newtonsoft.Json.Linq;

namespace ICSharpCode.Decompiler
{
	public class DotNetCorePathFinder
	{
		class DotNetCorePackageInfo
		{
			public readonly string Name;
			public readonly string Version;
			public readonly string Type;
			public readonly string Path;
			public readonly string[] RuntimeComponents;

			public DotNetCorePackageInfo(string fullName, string type, string path, string[] runtimeComponents)
			{
				var parts = fullName.Split('/');
				this.Name = parts[0];
				this.Version = parts[1];
				this.Type = type;
				this.Path = path;
				this.RuntimeComponents = runtimeComponents ?? new string[0];
			}
		}

		static readonly string[] LookupPaths = new string[] {
			 Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget\\packages")
		};

		readonly Dictionary<string, DotNetCorePackageInfo> packages;
		ISet<string> packageBasePaths = new HashSet<string>(StringComparer.Ordinal);
		readonly string assemblyName;
		readonly string basePath;
		readonly string targetFrameworkId;
		readonly string version;

		public DotNetCorePathFinder(string parentAssemblyFileName, string targetFrameworkId, string version)
		{
			this.assemblyName = Path.GetFileNameWithoutExtension(parentAssemblyFileName);
			this.basePath = Path.GetDirectoryName(parentAssemblyFileName);
			this.targetFrameworkId = targetFrameworkId;
			this.version = version;

			DecompilerEventSource.Log.Info($"Looking for {assemblyName}.deps.json in {basePath}");

			var depsJsonFileName = Path.Combine(basePath, $"{assemblyName}.deps.json");
			if (!File.Exists(depsJsonFileName)) {
				DecompilerEventSource.Log.Info($"{assemblyName}.deps.json not found.");
				return;
			}

			packages = LoadPackageInfos(depsJsonFileName, targetFrameworkId).ToDictionary(i => i.Name);

			foreach (var path in LookupPaths) {
				foreach (var pk in packages) {
					foreach (var item in pk.Value.RuntimeComponents) {
						var itemPath = Path.GetDirectoryName(item);
						var fullPath = Path.Combine(path, pk.Value.Name, pk.Value.Version, itemPath);
						if (Directory.Exists(fullPath))
							packageBasePaths.Add(fullPath);
					}
				}
			}
		}

		public string TryResolveDotNetCore(AssemblyNameReference name)
		{
			DecompilerEventSource.Log.Info($"TryResolveDotNetCore: {assemblyName}, version: {version}, reference: {name}");

			foreach (var basePath in packageBasePaths) {
				DecompilerEventSource.Log.Info($"TryResolveDotNetCore: scanning {basePath}...");

				if (File.Exists(Path.Combine(basePath, name.Name + ".dll"))) {
					return Path.Combine(basePath, name.Name + ".dll");
				} else if (File.Exists(Path.Combine(basePath, name.Name + ".exe"))) {
					return Path.Combine(basePath, name.Name + ".exe");
				}
			}

			return FallbackToDotNetSharedDirectory(name, version);
		}

		static IEnumerable<DotNetCorePackageInfo> LoadPackageInfos(string depsJsonFileName, string targetFramework)
		{
			var dependencies = JObject.Parse(File.ReadAllText(depsJsonFileName));
			var runtimeInfos = dependencies["targets"][targetFramework].Children().OfType<JProperty>().ToArray();
			var libraries = dependencies["libraries"].Children().OfType<JProperty>().ToArray();

			foreach (var library in libraries) {
				var type = library.First()["type"].ToString();
				var path = library.First()["path"]?.ToString();
				var runtimeInfo = runtimeInfos.FirstOrDefault(r => r.Name == library.Name)?.First()["runtime"]?.Children().OfType<JProperty>().Select(i => i.Name).ToArray();

				yield return new DotNetCorePackageInfo(library.Name, type, path, runtimeInfo);
			}
		}

		static string FallbackToDotNetSharedDirectory(AssemblyNameReference name, string version)
		{
			var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet\\shared\\Microsoft.NETCore.App", version);
			DecompilerEventSource.Log.Info($"Fallback: use {basePath}");
			if (File.Exists(Path.Combine(basePath, name.Name + ".dll"))) {
				return Path.Combine(basePath, name.Name + ".dll");
			} else if (File.Exists(Path.Combine(basePath, name.Name + ".exe"))) {
				return Path.Combine(basePath, name.Name + ".exe");
			}
			return null;
		}
	}
}
