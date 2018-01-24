using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LightJson.Serialization;
using Mono.Cecil;

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
			 Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages")
		};

		readonly Dictionary<string, DotNetCorePackageInfo> packages;
		ISet<string> packageBasePaths = new HashSet<string>(StringComparer.Ordinal);
		readonly string assemblyName;
		readonly string basePath;
		readonly string targetFrameworkId;
		readonly string version;
		readonly string dotnetBasePath = FindDotNetExeDirectory();

		public DotNetCorePathFinder(string parentAssemblyFileName, string targetFrameworkId, string version, ReferenceLoadInfo loadInfo = null)
		{
			this.assemblyName = Path.GetFileNameWithoutExtension(parentAssemblyFileName);
			this.basePath = Path.GetDirectoryName(parentAssemblyFileName);
			this.targetFrameworkId = targetFrameworkId;
			this.version = version;

			var depsJsonFileName = Path.Combine(basePath, $"{assemblyName}.deps.json");
			if (!File.Exists(depsJsonFileName)) {
				loadInfo?.AddMessage(assemblyName, MessageKind.Warning, $"{assemblyName}.deps.json could not be found!");
				return;
			}

			packages = LoadPackageInfos(depsJsonFileName, targetFrameworkId).ToDictionary(i => i.Name);

			foreach (var path in LookupPaths) {
				foreach (var pk in packages) {
					foreach (var item in pk.Value.RuntimeComponents) {
						var itemPath = Path.GetDirectoryName(item);
						var fullPath = Path.Combine(path, pk.Value.Name, pk.Value.Version, itemPath).ToLowerInvariant();
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

			return FallbackToDotNetSharedDirectory(name, new Version(version));
		}

		static IEnumerable<DotNetCorePackageInfo> LoadPackageInfos(string depsJsonFileName, string targetFramework)
		{
			var dependencies = JsonReader.Parse(File.ReadAllText(depsJsonFileName));
			var runtimeInfos = dependencies["targets"][targetFramework + "/"].AsJsonObject;
			var libraries = dependencies["libraries"].AsJsonObject;
			if (runtimeInfos == null || libraries == null)
				yield break;
			foreach (var library in libraries) {
				var type = library.Value["type"].AsString;
				var path = library.Value["path"].AsString;
				var runtimeInfo = runtimeInfos[library.Key].AsJsonObject?["runtime"].AsJsonObject;
				string[] components = new string[runtimeInfo?.Count ?? 0];
				if (runtimeInfo != null) {
					int i = 0;
					foreach (var component in runtimeInfo) {
						components[i] = component.Key;
						i++;
					}
				}
				yield return new DotNetCorePackageInfo(library.Key, type, path, components);
			}
		}

		string FallbackToDotNetSharedDirectory(AssemblyNameReference name, Version version)
		{
			if (dotnetBasePath == null) return null;
			var basePath = Path.Combine(dotnetBasePath, "shared", "Microsoft.NETCore.App");
			var closestVersion = GetClosestVersionFolder(basePath, version);
			if (File.Exists(Path.Combine(basePath, closestVersion, name.Name + ".dll"))) {
				return Path.Combine(basePath, closestVersion, name.Name + ".dll");
			} else if (File.Exists(Path.Combine(basePath, closestVersion, name.Name + ".exe"))) {
				return Path.Combine(basePath, closestVersion, name.Name + ".exe");
			}
			return null;
		}

		static string GetClosestVersionFolder(string basePath, Version version)
		{
			string result = null;
			foreach (var folder in new DirectoryInfo(basePath).GetDirectories().Select(d => ConvertToVersion(d.Name)).Where(v => v.Item1 != null).OrderByDescending(v => v.Item1)) {
				if (folder.Item1 >= version)
					result = folder.Item2;
			}
			return result ?? version.ToString();
		}

		static (Version, string) ConvertToVersion(string name)
		{
			string RemoveTrailingVersionInfo()
			{
				string shortName = name;
				int dashIndex = shortName.IndexOf('-');
				if (dashIndex > 0) {
					shortName = shortName.Remove(dashIndex);
				}
				return shortName;
			}

			try {
				return (new Version(RemoveTrailingVersionInfo()), name);
			} catch (Exception ex) {
				Trace.TraceWarning(ex.ToString());
				return (null, null);
			}
		}

		static string FindDotNetExeDirectory()
		{
			string dotnetExeName = (Environment.OSVersion.Platform == PlatformID.Unix) ? "dotnet" : "dotnet.exe";
			foreach (var item in Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator)) {
				try {
					string fileName = Path.Combine(item, dotnetExeName);
					if (!File.Exists(fileName))
						continue;
					if (Environment.OSVersion.Platform == PlatformID.Unix) {
						if ((new FileInfo(fileName).Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint) {
							var sb = new StringBuilder();
							realpath(fileName, sb);
							fileName = sb.ToString();
							if (!File.Exists(fileName))
								continue;
						}
					}
					return Path.GetDirectoryName(fileName);
				} catch (ArgumentException) { }
			}
			return null;
		}

		[DllImport("libc")]
		static extern void realpath(string path, StringBuilder resolvedPath);
	}
}
