﻿// Copyright (c) 2018 Siegfried Pammer
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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using ICSharpCode.Decompiler.Util;

using LightJson.Serialization;

namespace ICSharpCode.Decompiler.Metadata
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
				if (parts.Length > 1)
				{
					this.Version = parts[1];
				}
				else
				{
					this.Version = "<UNKNOWN>";
				}

				this.Type = type;
				this.Path = path;
				this.RuntimeComponents = runtimeComponents ?? Empty<string>.Array;
			}
		}

		static readonly string[] LookupPaths = new string[] {
			Environment.GetEnvironmentVariable("NUGET_PACKAGES"),
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages")
		};

		static readonly string[] RuntimePacks = new[] {
			"Microsoft.NETCore.App",
			"Microsoft.WindowsDesktop.App",
			"Microsoft.AspNetCore.App",
			"Microsoft.AspNetCore.All"
		};

		readonly DotNetCorePackageInfo[] packages;
		readonly List<string> searchPaths = new List<string>();
		readonly List<string> packageBasePaths = new List<string>();
		readonly Version targetFrameworkVersion;
		readonly string dotnetBasePath = FindDotNetExeDirectory();
		readonly string preferredRuntimePack;

		public DotNetCorePathFinder(TargetFrameworkIdentifier targetFramework, Version targetFrameworkVersion,
			string preferredRuntimePack)
		{
			this.targetFrameworkVersion = targetFrameworkVersion;
			this.preferredRuntimePack = preferredRuntimePack;

			if (targetFramework == TargetFrameworkIdentifier.NETStandard)
			{
				// .NET Standard 2.1 is implemented by .NET Core 3.0 or higher
				if (targetFrameworkVersion.Major == 2 && targetFrameworkVersion.Minor == 1)
				{
					this.targetFrameworkVersion = new Version(3, 0, 0);
				}
			}
		}

		public DotNetCorePathFinder(string parentAssemblyFileName, string targetFrameworkIdString, string preferredRuntimePack,
			TargetFrameworkIdentifier targetFramework, Version targetFrameworkVersion, ReferenceLoadInfo loadInfo = null)
			: this(targetFramework, targetFrameworkVersion, preferredRuntimePack)
		{
			string assemblyName = Path.GetFileNameWithoutExtension(parentAssemblyFileName);
			string basePath = Path.GetDirectoryName(parentAssemblyFileName);

			searchPaths.Add(basePath);

			var depsJsonFileName = Path.Combine(basePath, $"{assemblyName}.deps.json");
			if (File.Exists(depsJsonFileName))
			{
				packages = LoadPackageInfos(depsJsonFileName, targetFrameworkIdString).ToArray();

				foreach (var path in LookupPaths)
				{
					if (string.IsNullOrWhiteSpace(path))
					{
						continue;
					}
					foreach (var p in packages)
					{
						foreach (var item in p.RuntimeComponents)
						{
							var itemPath = Path.GetDirectoryName(item);
							var fullPath = Path.Combine(path, p.Name, p.Version, itemPath).ToLowerInvariant();
							if (Directory.Exists(fullPath))
								packageBasePaths.Add(fullPath);
						}
					}
				}
			}
			else
			{
				loadInfo?.AddMessage(assemblyName, MessageKind.Warning, $"{assemblyName}.deps.json could not be found!");
			}
		}

		public void AddSearchDirectory(string path)
		{
			this.searchPaths.Add(path);
		}

		public void RemoveSearchDirectory(string path)
		{
			this.searchPaths.Remove(path);
		}

		public string TryResolveDotNetCore(IAssemblyReference name)
		{
			foreach (var basePath in searchPaths.Concat(packageBasePaths))
			{
				if (File.Exists(Path.Combine(basePath, name.Name + ".dll")))
				{
					return Path.Combine(basePath, name.Name + ".dll");
				}
				else if (File.Exists(Path.Combine(basePath, name.Name + ".exe")))
				{
					return Path.Combine(basePath, name.Name + ".exe");
				}
			}

			return TryResolveDotNetCoreShared(name, out _);
		}

		internal string GetReferenceAssemblyPath(string targetFramework)
		{
			var (tfi, version) = UniversalAssemblyResolver.ParseTargetFramework(targetFramework);
			string identifier, identifierExt;
			switch (tfi)
			{
				case TargetFrameworkIdentifier.NETCoreApp:
					identifier = "Microsoft.NETCore.App";
					identifierExt = "netcoreapp" + version.Major + "." + version.Minor;
					break;
				case TargetFrameworkIdentifier.NETStandard:
					identifier = "NETStandard.Library";
					identifierExt = "netstandard" + version.Major + "." + version.Minor;
					break;
				case TargetFrameworkIdentifier.NET:
					identifier = "Microsoft.NETCore.App";
					identifierExt = "net" + version.Major + "." + version.Minor;
					break;
				default:
					throw new NotSupportedException();
			}
			string basePath = Path.Combine(dotnetBasePath, "packs", identifier + ".Ref");
			string versionFolder = GetClosestVersionFolder(basePath, version);
			return Path.Combine(basePath, versionFolder, "ref", identifierExt);
		}

		static IEnumerable<DotNetCorePackageInfo> LoadPackageInfos(string depsJsonFileName, string targetFramework)
		{
			var dependencies = JsonReader.Parse(File.ReadAllText(depsJsonFileName));
			var runtimeInfos = dependencies["targets"][targetFramework].AsJsonObject;
			var libraries = dependencies["libraries"].AsJsonObject;
			if (runtimeInfos == null || libraries == null)
				yield break;
			foreach (var library in libraries)
			{
				var type = library.Value["type"].AsString;
				var path = library.Value["path"].AsString;
				var runtimeInfo = runtimeInfos[library.Key].AsJsonObject?["runtime"].AsJsonObject;
				string[] components = new string[runtimeInfo?.Count ?? 0];
				if (runtimeInfo != null)
				{
					int i = 0;
					foreach (var component in runtimeInfo)
					{
						components[i] = component.Key;
						i++;
					}
				}
				yield return new DotNetCorePackageInfo(library.Key, type, path, components);
			}
		}

		public string TryResolveDotNetCoreShared(IAssemblyReference name, out string runtimePack)
		{
			if (dotnetBasePath == null)
			{
				runtimePack = null;
				return null;
			}

			IEnumerable<string> runtimePacks = RuntimePacks;

			if (preferredRuntimePack != null)
			{
				runtimePacks = new[] { preferredRuntimePack }.Concat(runtimePacks);
			}

			foreach (string pack in runtimePacks)
			{
				runtimePack = pack;
				string basePath = Path.Combine(dotnetBasePath, "shared", pack);
				if (!Directory.Exists(basePath))
					continue;
				var closestVersion = GetClosestVersionFolder(basePath, targetFrameworkVersion);
				if (File.Exists(Path.Combine(basePath, closestVersion, name.Name + ".dll")))
				{
					return Path.Combine(basePath, closestVersion, name.Name + ".dll");
				}
				else if (File.Exists(Path.Combine(basePath, closestVersion, name.Name + ".exe")))
				{
					return Path.Combine(basePath, closestVersion, name.Name + ".exe");
				}
			}
			runtimePack = null;
			return null;
		}

		static string GetClosestVersionFolder(string basePath, Version version)
		{
			var foundVersions = new DirectoryInfo(basePath).GetDirectories()
				.Select(ConvertToVersion)
				.Where(v => v.version != null);
			foreach (var folder in foundVersions.OrderBy(v => v.version))
			{
				if (folder.version >= version
					&& folder.directory.EnumerateFiles("*.dll", SearchOption.AllDirectories).Any())
				{
					return folder.directory.Name;
				}
			}
			return version.ToString();
		}

		internal static (Version version, DirectoryInfo directory) ConvertToVersion(DirectoryInfo directory)
		{
			string RemoveTrailingVersionInfo()
			{
				string shortName = directory.Name;
				int dashIndex = shortName.IndexOf('-');
				if (dashIndex > 0)
				{
					shortName = shortName.Remove(dashIndex);
				}
				return shortName;
			}

			try
			{
				return (new Version(RemoveTrailingVersionInfo()), directory);
			}
			catch (Exception ex)
			{
				Trace.TraceWarning(ex.ToString());
				return (null, null);
			}
		}

		public static string FindDotNetExeDirectory()
		{
			string dotnetExeName = (Environment.OSVersion.Platform == PlatformID.Unix) ? "dotnet" : "dotnet.exe";
			foreach (var item in Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator))
			{
				try
				{
					string fileName = Path.Combine(item, dotnetExeName);
					if (!File.Exists(fileName))
						continue;
					if (Environment.OSVersion.Platform == PlatformID.Unix)
					{
						if ((new FileInfo(fileName).Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
						{
							fileName = GetRealPath(fileName, Encoding.Default);
							if (!File.Exists(fileName))
								continue;
						}
					}
					return Path.GetDirectoryName(fileName);
				}
				catch (ArgumentException) { }
			}
			return null;
		}

		static unsafe string GetRealPath(string path, Encoding encoding)
		{
			var bytes = encoding.GetBytes(path);
			fixed (byte* input = bytes)
			{

				byte* output = GetRealPath(input, null);
				if (output == null)
				{
					return null;
				}
				int len = 0;
				for (byte* c = output; *c != 0; c++)
				{
					len++;
				}
				byte[] result = new byte[len];
				Marshal.Copy((IntPtr)output, result, 0, result.Length);
				Free(output);
				return encoding.GetString(result);
			}
		}

		[DllImport("libc", EntryPoint = "realpath")]
		static extern unsafe byte* GetRealPath(byte* path, byte* resolvedPath);

		[DllImport("libc", EntryPoint = "free")]
		static extern unsafe void Free(void* ptr);
	}
}
