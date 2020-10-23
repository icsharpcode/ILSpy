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

using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	class RoslynToolset
	{
		readonly SourceCacheContext cache;
		readonly SourceRepository repository;
		readonly FindPackageByIdResource resource;
		readonly string nugetDir;
		readonly Dictionary<string, string> installedCompilers = new Dictionary<string, string> {
			{ "legacy", Environment.ExpandEnvironmentVariables(@"%WINDIR%\Microsoft.NET\Framework\v4.0.30319") }
		};
		readonly object syncObj = new object();

		public RoslynToolset()
		{
			this.cache = new SourceCacheContext();
			this.repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
			this.resource = repository.GetResource<FindPackageByIdResource>();
			this.nugetDir = Path.Combine(Path.GetDirectoryName(typeof(RoslynToolset).Assembly.Location), "roslyn");
		}

		public string GetCSharpCompiler(string version)
		{
			return GetCompiler("csc.exe", version);
		}

		public string GetVBCompiler(string version)
		{
			return GetCompiler("vbc.exe", version);
		}

		string GetCompiler(string compiler, string version)
		{
			lock (syncObj)
			{
				if (installedCompilers.TryGetValue(version, out var path))
					return Path.Combine(path, compiler);

				string outputPath = Path.Combine(nugetDir, version);
				path = Path.Combine(outputPath, "tools");

				if (!Directory.Exists(path))
				{
					FetchPackage(version, outputPath).GetAwaiter().GetResult();
				}

				installedCompilers.Add(version, path);
				return Path.Combine(path, compiler);
			}
		}

		async Task FetchPackage(string version, string outputPath)
		{
			ILogger logger = NullLogger.Instance;
			CancellationToken cancellationToken = CancellationToken.None;
			using MemoryStream packageStream = new MemoryStream();

			await resource.CopyNupkgToStreamAsync(
				"Microsoft.Net.Compilers",
				NuGetVersion.Parse(version),
				packageStream,
				cache,
				logger,
				cancellationToken);

			using PackageArchiveReader packageReader = new PackageArchiveReader(packageStream);
			NuspecReader nuspecReader = await packageReader.GetNuspecReaderAsync(cancellationToken);

			var files = await packageReader.GetFilesAsync(cancellationToken);
			files = files.Where(f => f.StartsWith("tools", StringComparison.OrdinalIgnoreCase));
			await packageReader.CopyFilesAsync(outputPath, files,
				(sourceFile, targetPath, fileStream) => {
					fileStream.CopyToFile(targetPath);
					return targetPath;
				},
				logger, cancellationToken);
		}
	}
}
