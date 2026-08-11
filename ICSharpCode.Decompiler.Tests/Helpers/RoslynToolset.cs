// Copyright (c) 2020 Siegfried Pammer
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
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Metadata;

using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable",
		Justification = "Derived types are intended to be used as static singletons, each living until the process terminates.")]
	abstract class AbstractToolset
	{
		readonly SourceCacheContext cache;
		readonly SourceRepository repository, dotnetToolsFeed;
		readonly FindPackageByIdResource resource, dotnetToolsResource;
		protected readonly string baseDir;

		public AbstractToolset(string baseDir)
		{
			this.cache = new SourceCacheContext();
			this.repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
			this.resource = repository.GetResource<FindPackageByIdResource>();
			this.dotnetToolsFeed = Repository.Factory.GetCoreV3("https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json");
			this.dotnetToolsResource = dotnetToolsFeed.GetResource<FindPackageByIdResource>();
			this.baseDir = baseDir;
		}

		enum PackageVersionKind
		{
			Rtm,
			Preview,
			TransportFeed,
		}

		// RTM versions look like 5.3.0, and preview ones like 4.12.0-3.final. Transport feed ones eg 5.6.0-2.26177.1
		static PackageVersionKind ClassifyVersion(NuGetVersion version)
		{
			if (!version.IsPrerelease)
				return PackageVersionKind.Rtm;

			var labels = version.ReleaseLabels.ToList();
			bool looksLikeTransportFeed = labels.Count >= 3
				&& labels.All(static label => int.TryParse(label, out _));

			return looksLikeTransportFeed ? PackageVersionKind.TransportFeed : PackageVersionKind.Preview;
		}

		protected async Task FetchPackage(string packageName, string version, string sourcePath, string outputPath)
		{
			if (!Directory.Exists(Path.Combine(Roundtrip.RoundtripAssembly.TestDir, "nuget")))
				Assert.Fail("No nuget cache found!");

			ILogger logger = NullLogger.Instance;
			CancellationToken cancellationToken = CancellationToken.None;
			string pathToPackage = Path.Combine(Roundtrip.RoundtripAssembly.TestDir, "nuget", $"{packageName}-{version}.nupkg");
			Stream packageStream;
			if (File.Exists(pathToPackage))
			{
				packageStream = File.OpenRead(pathToPackage);
			}
			else
			{
				packageStream = new MemoryStream();

				NuGetVersion parsedVersion = NuGetVersion.Parse(version);
				PackageVersionKind versionKind = ClassifyVersion(parsedVersion);
				FindPackageByIdResource selectedResource = versionKind == PackageVersionKind.TransportFeed
					? dotnetToolsResource
					: resource;

				await selectedResource.CopyNupkgToStreamAsync(
					packageName,
					parsedVersion,
					packageStream,
					cache,
					logger,
					cancellationToken).ConfigureAwait(false);

				packageStream.Position = 0;
			}
			using (packageStream)
			{
				using PackageArchiveReader packageReader = new PackageArchiveReader(packageStream);
				NuspecReader nuspecReader = await packageReader.GetNuspecReaderAsync(cancellationToken).ConfigureAwait(false);

				var files = (await packageReader.GetFilesAsync(cancellationToken).ConfigureAwait(false)).ToArray();
				files = files.Where(f => f.StartsWith(sourcePath, StringComparison.OrdinalIgnoreCase)).ToArray();
				await packageReader.CopyFilesAsync(outputPath, files,
					(sourceFile, targetPath, fileStream) => {
						fileStream.CopyToFile(targetPath);
						return targetPath;
					},
					logger, cancellationToken).ConfigureAwait(false);
			}
		}
	}

	class RoslynToolset : AbstractToolset
	{
		// Registrations run concurrently while Tester.Initialize awaits all Fetch calls;
		// lookups only happen after Initialize completes, so the read paths stay lock-free.
		readonly Dictionary<string, string> installedCompilers = new Dictionary<string, string> {
			{ "legacy", Environment.ExpandEnvironmentVariables(@"%WINDIR%\Microsoft.NET\Framework\v4.0.30319") }
		};

		public RoslynToolset()
			: base(Path.Combine(AppContext.BaseDirectory, "roslyn"))
		{
		}

		public async Task Fetch(string version, string packageName = "Microsoft.Net.Compilers.Toolset", string sourcePath = "tasks/net472")
		{
			string path = Path.Combine(baseDir, version, sourcePath);
			if (!Directory.Exists(path))
			{
				await FetchPackage(packageName, version, sourcePath, Path.Combine(baseDir, version)).ConfigureAwait(false);
			}

			lock (installedCompilers)
			{
				installedCompilers.Add(SanitizeVersion(version), path);
			}
		}

		// In the .NET ("netcore") build of the compiler toolset the executables live in a
		// "bincore" subfolder of the tasks directory, as .dlls launched through the dotnet host.
		// The old Microsoft.Net.Compilers packages (Roslyn 1.x/2.x) have no .NET build; they
		// only ship .NET Framework executables, which non-Windows platforms host with Mono.
		public string GetCSharpCompiler(string version)
		{
			return GetHostedCompiler(version, "csc");
		}

		public string GetVBCompiler(string version)
		{
			return GetHostedCompiler(version, "vbc");
		}

		string GetHostedCompiler(string version, string name)
		{
			if (!OperatingSystem.IsWindows())
			{
				// The installed path may point directly at a dotnet-hosted compiler directory
				// (e.g. Microsoft.NETCore.Compilers' tools/bincore) or at a tasks directory
				// with a bincore subfolder (Microsoft.Net.Compilers.Toolset).
				string dll = GetCompiler($"{name}.dll", version);
				if (File.Exists(dll))
					return dll;
				dll = GetCompiler($"bincore/{name}.dll", version);
				if (File.Exists(dll))
					return dll;
			}
			return GetCompiler($"{name}.exe", version);
		}

		string GetCompiler(string compiler, string version)
		{
			if (installedCompilers.TryGetValue(SanitizeVersion(version), out var path))
				return Path.Combine(path, compiler);
			throw new NotSupportedException($"Cannot find {compiler} {version}, please add it to the initialization.");
		}

		internal static string SanitizeVersion(string version)
		{
			int index = version.IndexOf('-');
			if (index > 0)
				return version[..index];
			return version;
		}
	}

	class VsWhereToolset : AbstractToolset
	{
		string vswherePath;

		public VsWhereToolset()
			: base(Path.Combine(AppContext.BaseDirectory, "vswhere"))
		{
		}

		public async Task Fetch()
		{
			string path = Path.Combine(baseDir, "tools");
			if (!Directory.Exists(path))
			{
				await FetchPackage("vswhere", "2.8.4", "tools", baseDir).ConfigureAwait(false);
			}
			vswherePath = Path.Combine(path, "vswhere.exe");
		}

		public string GetVsWhere() => vswherePath;
	}

	class RefAssembliesToolset : AbstractToolset
	{
		// Registrations run concurrently while Tester.Initialize awaits all Fetch calls;
		// lookups only happen after Initialize completes, so the read paths stay lock-free.
		readonly Dictionary<string, string> installedFrameworks = new Dictionary<string, string> {
			{ "legacy", Path.Combine(Roundtrip.RoundtripAssembly.TestDir, "dotnet", "legacy") },
			{ "2.2.0", Path.Combine(Roundtrip.RoundtripAssembly.TestDir, "dotnet", "netcore-2.2") },
		};

		public RefAssembliesToolset()
			: base(Path.Combine(AppContext.BaseDirectory, "netfx"))
		{
		}

		public async Task Fetch(string version, string packageName = "Microsoft.NETCore.App.Ref", string sourcePath = "ref/net5.0")
		{
			string path = Path.Combine(baseDir, version, sourcePath);
			if (!Directory.Exists(path))
			{
				await FetchPackage(packageName, version, sourcePath, Path.Combine(baseDir, version)).ConfigureAwait(false);
			}

			lock (installedFrameworks)
			{
				installedFrameworks.Add(RoslynToolset.SanitizeVersion(version), path);
			}
		}

		internal string GetPath(string targetFramework)
		{
			var (id, version) = UniversalAssemblyResolver.ParseTargetFramework(targetFramework);
			string path;
			if (id == TargetFrameworkIdentifier.NETFramework)
			{
				path = installedFrameworks["legacy"];
			}
			else
			{
				path = installedFrameworks[version.ToString(3)];
			}
			Debug.Assert(Path.Exists(path));
			return path;
		}
	}
}
