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
		readonly string dotnetBasePath = FindDotNetExeDirectory();
		readonly Dictionary<string, UnresolvedAssemblyNameReference> loadInfo;

		public DotNetCorePathFinder(string parentAssemblyFileName, string targetFrameworkId, string version, Dictionary<string, UnresolvedAssemblyNameReference> loadInfo)
		{
			this.assemblyName = Path.GetFileNameWithoutExtension(parentAssemblyFileName);
			this.basePath = Path.GetDirectoryName(parentAssemblyFileName);
			this.targetFrameworkId = targetFrameworkId;
			this.version = version;
			this.loadInfo = loadInfo;

			var depsJsonFileName = Path.Combine(basePath, $"{assemblyName}.deps.json");
			if (!File.Exists(depsJsonFileName)) {
				loadInfo.AddMessage(assemblyName, MessageKind.Error, $"{assemblyName}.deps.json could not be found!");
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
			foreach (var basePath in packageBasePaths) {
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

		string FallbackToDotNetSharedDirectory(AssemblyNameReference name, string version)
		{
			if (dotnetBasePath == null) return null;
			var basePath = Path.Combine(dotnetBasePath, "shared\\Microsoft.NETCore.App", version);
			if (File.Exists(Path.Combine(basePath, name.Name + ".dll"))) {
				return Path.Combine(basePath, name.Name + ".dll");
			} else if (File.Exists(Path.Combine(basePath, name.Name + ".exe"))) {
				return Path.Combine(basePath, name.Name + ".exe");
			}
			return null;
		}

		static string FindDotNetExeDirectory()
		{
			string dotnetExeName = (Environment.OSVersion.Platform == PlatformID.Unix) ? "dotnet" : "dotnet.exe";
			foreach (var item in Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator)) {
				try {
					if (File.Exists(Path.Combine(item, dotnetExeName)))
						return item;
				} catch (ArgumentException) { }
			}
			return null;
		}
	}
}
