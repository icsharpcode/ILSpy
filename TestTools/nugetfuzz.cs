// Copyright (c) 2026 Siegfried Pammer
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

#:project ../ICSharpCode.Decompiler/ICSharpCode.Decompiler.csproj
#:package NuGet.Packaging@*
#:package Mono.Cecil@0.11.6
#:property PublishAot=false

// nugetfuzz: downloads nuget packages (sequentially), resolves their dependency
// closure, picks a lib TFM (installed runtime, or classic .NET Framework via the
// Microsoft.NETFramework.ReferenceAssemblies packages), then decompiles every
// assembly type-by-type and reports Debug.Assert failures / exceptions.
//
// With --pdb it instead generates a portable PDB for each assembly and checks it: Mono.Cecil
// (the consumer ILLink uses) has to be able to read every method body through it, and the PDB
// metadata has to satisfy a structural lint. --pdb-lint runs the same checks against a PDB that
// already exists next to the assembly, which is how the lint is calibrated: it must report
// nothing at all for a PDB the C# compiler wrote.
//
// usage: dotnet run nugetfuzz.cs -- [--download-only|--pdb] <PackageId[@Version]>... | @packagelist.txt
//        dotnet run nugetfuzz.cs -- --pdb-lint <file.dll|dir>... | @corpus.txt

using System.Diagnostics;
using System.IO.Compression;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;

// Cecil is used through aliases: its MethodDefinition/SequencePoint/MethodBody names collide with
// System.Reflection.Metadata's and the decompiler's, and every one of the three is used here.
using Cecil = Mono.Cecil;
using CecilCil = Mono.Cecil.Cil;

using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Versioning;

Trace.Listeners.Clear();
Trace.Listeners.Add(new ThrowOnAssert());
try
{
	Debug.Fail("self-test");
	Console.Error.WriteLine("FATAL: assert hook not active, Debug.Assert would go unreported");
	return 2;
}
catch (AssertionFailedException)
{
	// hook works
}

var http = new HttpClient();
http.DefaultRequestHeaders.UserAgent.ParseAdd("nugetfuzz/1.0");
var cacheRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache", "nugetfuzz");
var globalPackagesRoot = Environment.GetEnvironmentVariable("NUGET_PACKAGES")
	?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
var installedTfm = NuGetFramework.Parse($"net{Environment.Version.Major}.{Environment.Version.Minor}");
var net48 = NuGetFramework.Parse("net48");
var reducer = new FrameworkReducer();
var failures = new Dictionary<string, Finding>();
var formatting = new DecompilerSettings().CSharpFormattingOptions;
int assemblyCount = 0, typeCount = 0, pdbChecked = 0, pdbSkipped = 0;
long charCount = 0, refsResolved = 0, refsTotal = 0;
bool verbose = Environment.GetEnvironmentVariable("NUGETFUZZ_VERBOSE") != null;
var dumpDir = Environment.GetEnvironmentVariable("NUGETFUZZ_DUMP");
if (dumpDir != null)
	Directory.CreateDirectory(dumpDir);
var versionCache = new Dictionary<string, List<NuGetVersion>>();
var noSuchPackage = new HashSet<string>();

// Legacy framework-satellite assemblies whose nuget package id differs from the assembly name.
var assemblyPackageAlias = new Dictionary<string, string> {
	["System.Web.Mvc"] = "Microsoft.AspNet.Mvc",
	["System.Web.Razor"] = "Microsoft.AspNet.Razor",
	["System.Web.WebPages"] = "Microsoft.AspNet.WebPages",
	["System.Web.WebPages.Razor"] = "Microsoft.AspNet.WebPages",
	["System.Web.WebPages.Deployment"] = "Microsoft.AspNet.WebPages",
	["System.Web.Helpers"] = "Microsoft.AspNet.WebPages",
	["System.Web.Http"] = "Microsoft.AspNet.WebApi.Core",
	["System.Web.Http.WebHost"] = "Microsoft.AspNet.WebApi.WebHost",
	["System.Web.Http.SelfHost"] = "Microsoft.AspNet.WebApi.SelfHost",
	["System.Net.Http.Formatting"] = "Microsoft.AspNet.WebApi.Client",
	["Microsoft.Practices.Unity"] = "Unity",
	["System.Data.SqlServerCe"] = "Microsoft.SqlServer.Compact",
	["Microsoft.Practices.ServiceLocation"] = "CommonServiceLocator",
};

const string NetCoreRefPack = "Microsoft.NETCore.App.Ref";

// WPF/WinForms assemblies of .NET (Core) live in the WindowsDesktop ref pack, not on nuget.
var windowsDesktopPrefixes = new[] {
	"PresentationCore", "PresentationFramework", "WindowsBase", "System.Xaml",
	"System.Windows.", "System.Drawing", "ReachFramework", "System.Printing",
	"UIAutomation", "Microsoft.VisualBasic.Forms",
};

// Render the aggregate report of a sweep and exit; nothing is decompiled in this mode.
if (args is ["--report", var ledgerPath, ..])
{
	var outPath = args.Length > 2 ? args[2] : Path.ChangeExtension(ledgerPath, ".html");
	RenderLedger(ledgerPath, outPath);
	Console.WriteLine($"report: {Path.GetFullPath(outPath)}");
	return 0;
}

// Populates the cache without decompiling: the sweep is the slow part, and a corpus
// only needs the assemblies on disk.
var downloadOnly = args.Contains("--download-only");
// Generates a PDB per assembly and checks it, instead of decompiling type by type. The two are
// alternatives rather than additions: a whole-assembly PDB is far more expensive than the type
// sweep, and they answer different questions.
var pdbMode = args.Contains("--pdb");
// Checks the PDB the assembly already ships with. Calibration, not a sweep.
var pdbLint = args.Contains("--pdb-lint");
// Findings first hit by the assembly being decompiled. They get their reference context
// once it is done, because the resolver keeps discovering references until then.
var pendingContext = new List<string>();
var arguments = args
	.Where(a => !a.StartsWith("--"))
	.SelectMany(a => a.StartsWith('@') ? File.ReadAllLines(a[1..]) : new[] { a })
	.Select(l => l.Trim())
	.Where(l => l.Length > 0 && !l.StartsWith('#'))
	.ToList();
if (arguments.Count == 0)
{
	Console.Error.WriteLine("usage: nugetfuzz [--download-only|--pdb] <PackageId[@Version]>... | @packagelist.txt");
	Console.Error.WriteLine("       nugetfuzz --pdb-lint <file.dll|dir>... | @corpus.txt");
	Console.Error.WriteLine("       nugetfuzz --report <ledger.jsonl> [out.html]");
	return 1;
}

if (pdbLint)
{
	foreach (var dll in ExpandDlls(arguments))
		LintExistingPdb(dll);
}
else
{
	foreach (var spec in arguments)
	{
		try
		{
			await ProcessPackage(spec);
		}
		catch (Exception ex) when (
			ex is InvalidOperationException && ex.Message.Contains("not found")
			|| ex is InvalidDataException)
		{
			// Deleted/delisted package or corrupt nupkg on nuget.org - not a decompiler issue.
			Console.WriteLine($"  skip {spec}: {ex.Message}");
		}
		catch (Exception ex)
		{
			Report(spec, "-", "-", ex);
		}
	}
}

Console.WriteLine();
if (pdbMode || pdbLint)
	Console.WriteLine($"=== {assemblyCount} assemblies, {pdbChecked} PDBs checked, {pdbSkipped} skipped, {failures.Count} distinct failures ({failures.Values.Sum(f => f.Count)} total) ===");
else
	Console.WriteLine($"=== {assemblyCount} assemblies, {typeCount} types decompiled ({charCount} chars), {refsResolved}/{refsTotal} refs resolved, {failures.Count} distinct failures ({failures.Values.Sum(f => f.Count)} total) ===");
foreach (var entry in failures.Values.OrderByDescending(f => f.Count))
	Console.WriteLine($"{entry.Count,6}x {entry.Describe()}");
// A sweep runs this program once per package, so per-run findings are appended to a
// shared ledger; `--report <ledger>` renders the aggregate. Without a ledger the run
// reports only itself.
var ledger = Environment.GetEnvironmentVariable("NUGETFUZZ_LEDGER");
if (downloadOnly)
{
	// Nothing was decompiled, so there are no findings to report on.
}
else if (ledger != null)
{
	AppendToLedger(ledger, failures.Values, assemblyCount, typeCount, refsResolved, refsTotal);
	Console.WriteLine($"ledger: {Path.GetFullPath(ledger)}");
}
else
{
	var htmlPath = Path.GetFullPath(Environment.GetEnvironmentVariable("NUGETFUZZ_HTML") ?? "nugetfuzz-report.html");
	WriteHtmlReport(htmlPath, failures.Values.ToList(), assemblyCount, typeCount, refsResolved, refsTotal, dumpDir);
	Console.WriteLine($"report: {htmlPath}");
}
return failures.Count == 0 ? 0 : 1;

async Task ProcessPackage(string spec)
{
	var parts = spec.Split('@');
	var id = parts[0];
	var version = parts.Length > 1 ? NuGetVersion.Parse(parts[1]) : await ResolveVersion(id, null);
	if (version == null)
	{
		Console.WriteLine($"=== {id}: no versions found, skipping ===");
		return;
	}
	Console.WriteLine($"=== {id} {version} ===");
	var dir = await GetPackage(id, version);

	var matchTarget = installedTfm;
	var pick = PickLib(dir, matchTarget);
	if (pick == null)
	{
		matchTarget = net48;
		pick = PickLib(dir, matchTarget);
	}
	if (pick == null)
	{
		Console.WriteLine("  no compatible lib assemblies, skipping");
		return;
	}
	var (libFw, libDir) = pick.Value;
	Console.WriteLine($"  lib: {libFw.GetShortFolderName()}");

	var searchDirs = await CollectDependencies(dir, matchTarget, id);
	searchDirs.Insert(0, libDir);
	if (downloadOnly)
	{
		// The package and its dependency closure are in the cache now, which is all a
		// corpus needs; decompiling every type is what the sweep is for.
		Console.WriteLine($"  cached: {libDir}");
		return;
	}
	// The installed runtime dir is a last-resort fallback only: Microsoft.NETCore.App
	// ships stub facades (e.g. WindowsBase.dll without System.Windows.Point) that must
	// not shadow the real assemblies from ref packs or dependency packages.
	string? fallbackDir = null;
	if (libFw.Framework == FrameworkConstants.FrameworkIdentifiers.Net)
		searchDirs.AddRange(await GetNetFxRefDirs(libFw));
	else
		fallbackDir = Path.GetDirectoryName(typeof(object).Assembly.Location);

	foreach (var dll in Directory.GetFiles(libDir, "*.dll").OrderBy(f => f))
		await DecompileAssembly(id, dll, searchDirs, matchTarget, fallbackDir);
}

// Walks the dependency closure breadth-first, downloading each package once and
// collecting the lib dir that best matches the root package's target framework.
async Task<List<string>> CollectDependencies(string rootDir, NuGetFramework matchTarget, string rootId)
{
	var searchDirs = new List<string>();
	var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { rootId };
	var queue = new Queue<string>();
	queue.Enqueue(rootDir);
	while (queue.Count > 0)
	{
		var dir = queue.Dequeue();
		var nuspecPath = Directory.GetFiles(dir, "*.nuspec").FirstOrDefault();
		if (nuspecPath == null)
			continue;
		var groups = new NuspecReader(nuspecPath).GetDependencyGroups().ToList();
		var nearestGroupFw = reducer.GetNearest(matchTarget, groups.Select(g => g.TargetFramework));
		var group = groups.FirstOrDefault(g => g.TargetFramework.Equals(nearestGroupFw));
		if (group == null)
			continue;
		foreach (var dep in group.Packages)
		{
			if (!visited.Add(dep.Id))
				continue;
			try
			{
				var depVersion = await ResolveVersion(dep.Id, dep.VersionRange);
				if (depVersion == null)
					continue;
				var depDir = await GetPackage(dep.Id, depVersion);
				var depPick = PickLib(depDir, matchTarget);
				if (depPick != null)
					searchDirs.Add(depPick.Value.dir);
				queue.Enqueue(depDir);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"  ! dep {dep.Id}: {ex.Message}");
			}
		}
	}
	return searchDirs;
}

// Picks the lib/<tfm> directory nearest to the given target framework.
// Old-style packages with DLLs directly under lib/ are treated as classic .NET Framework.
(NuGetFramework fw, string dir)? PickLib(string pkgDir, NuGetFramework target)
{
	var libRoot = Path.Combine(pkgDir, "lib");
	if (!Directory.Exists(libRoot))
		return null;
	var map = new Dictionary<NuGetFramework, string>();
	foreach (var d in Directory.GetDirectories(libRoot))
	{
		NuGetFramework fw;
		try
		{
			fw = NuGetFramework.ParseFolder(Path.GetFileName(d));
		}
		catch
		{
			continue;
		}
		if (fw.IsSpecificFramework && Directory.GetFiles(d, "*.dll").Length > 0)
			map[fw] = d;
	}
	var nearest = reducer.GetNearest(target, map.Keys);
	if (nearest != null)
		return (nearest, map[nearest]);
	if (target.Framework == FrameworkConstants.FrameworkIdentifiers.Net
		&& Directory.GetFiles(libRoot, "*.dll").Length > 0)
	{
		return (target, libRoot);
	}
	return null;
}

// Classic .NET Framework has no reference assemblies on this machine; fetch the
// matching Microsoft.NETFramework.ReferenceAssemblies package instead.
async Task<List<string>> GetNetFxRefDirs(NuGetFramework libFw)
{
	var shortName = libFw.GetShortFolderName();
	if (!Regex.IsMatch(shortName, "^net[0-9]+$"))
		shortName = "net48";
	// Reference-assembly packs exist only for these TFMs; map anything else (net30,
	// net401, ...) to the smallest pack that is a superset of the requested framework.
	string[] knownPacks = ["net20", "net35", "net40", "net45", "net451", "net452", "net46", "net461", "net462", "net47", "net471", "net472", "net48", "net481"];
	if (!knownPacks.Contains(shortName))
	{
		static Version DigitsVersion(string tfm) => Version.Parse(string.Join('.', tfm[3..].ToCharArray()));
		var requested = DigitsVersion(shortName);
		shortName = knownPacks.FirstOrDefault(k => DigitsVersion(k) >= requested) ?? "net48";
	}
	var id = "Microsoft.NETFramework.ReferenceAssemblies." + shortName;
	var version = await ResolveVersion(id, null)
		?? throw new InvalidOperationException($"cannot resolve {id}");
	var dir = await GetPackage(id, version);
	var fxRoot = Directory.GetDirectories(Path.Combine(dir, "build", ".NETFramework")).Single();
	var dirs = new List<string> { fxRoot };
	var facades = Path.Combine(fxRoot, "Facades");
	if (Directory.Exists(facades))
		dirs.Add(facades);
	return dirs;
}

async Task<NuGetVersion?> ResolveVersion(string id, VersionRange? range)
{
	var versions = await GetVersions(id)
		?? throw new InvalidOperationException($"package {id} not found");
	if (range != null)
		return range.FindBestMatch(versions) ?? versions.LastOrDefault();
	return versions.LastOrDefault(v => !v.IsPrerelease) ?? versions.LastOrDefault();
}

// nuget.org stalls or resets a connection now and then. Without a retry such a flake
// travels all the way out as an unhandled exception, which the report files as a
// decompiler [EXCEPTION] - the one bucket that must contain nothing but real crashes -
// and the package is skipped without a single type being decompiled. A 404 is an
// answer, not a flake, so it is passed straight through to the caller.
async Task<T> WithRetry<T>(Func<Task<T>> request)
{
	for (int attempt = 1; ; attempt++)
	{
		try
		{
			return await request();
		}
		catch (Exception ex) when (attempt < 3 && IsTransient(ex))
		{
			Console.WriteLine($"  ! transient http failure ({ex.GetType().Name}), retry {attempt}/2");
			await Task.Delay(TimeSpan.FromSeconds(2 * attempt));
		}
	}

	static bool IsTransient(Exception ex) => ex switch {
		// HttpClient.Timeout surfaces as a cancellation, not as a timeout
		TaskCanceledException or IOException => true,
		// no status code at all means the request never got an answer: DNS, reset, TLS
		HttpRequestException { StatusCode: null } => true,
		HttpRequestException { StatusCode: var status } =>
			status == System.Net.HttpStatusCode.TooManyRequests || (int)status! >= 500,
		_ => false,
	};
}

async Task<List<NuGetVersion>?> GetVersions(string id)
{
	var key = id.ToLowerInvariant();
	if (noSuchPackage.Contains(key))
		return null;
	if (versionCache.TryGetValue(key, out var cached))
		return cached;
	try
	{
		var index = await WithRetry(() => http.GetFromJsonAsync<VersionIndex>(
			$"https://api.nuget.org/v3-flatcontainer/{key}/index.json"));
		var versions = index!.versions.Select(NuGetVersion.Parse).ToList();
		versionCache[key] = versions;
		return versions;
	}
	catch (HttpRequestException)
	{
		noSuchPackage.Add(key);
		return null;
	}
}

// Some packages reference assemblies they never declare as dependencies (e.g.
// itextsharp 5.5.13.6 -> itext.commons). Best effort: for any assembly reference
// not satisfiable from the current search dirs, try the same-named nuget package,
// preferring the package version that matches the assembly version.
// Called by LoggingResolver whenever a reference resolves nowhere - main-module refs
// and transitive refs alike (e.g. a fetched Microsoft.AspNet.Mvc needing WebPages).
// Tries framework ref packs and same-named/aliased nuget packages; returns true when
// the dll is available in `dirs` afterwards.
async Task<bool> TryFetchMissingRef(IAssemblyReference reference, Version? coreVersion, NuGetFramework matchTarget, List<string> dirs)
{
	bool Satisfied()
	{
		lock (dirs)
		{
			return dirs.Any(d => File.Exists(Path.Combine(d, reference.Name + ".dll")));
		}
	}
	if (coreVersion != null
		&& windowsDesktopPrefixes.Any(p => reference.Name.StartsWith(p, StringComparison.Ordinal)))
	{
		await AddRefPack("Microsoft.WindowsDesktop.App.Ref", coreVersion, dirs);
		if (Satisfied())
			return true;
	}
	if (coreVersion != null && reference.Name.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal))
	{
		await AddRefPack("Microsoft.AspNetCore.App.Ref", coreVersion, dirs);
		if (Satisfied())
			return true;
	}
	const string EntLib = "Microsoft.Practices.EnterpriseLibrary.";
	var packageId = assemblyPackageAlias.GetValueOrDefault(reference.Name)
		?? (reference.Name.StartsWith(EntLib, StringComparison.Ordinal)
			? "EnterpriseLibrary." + reference.Name[EntLib.Length..]
			: reference.Name);
	var versions = await GetVersions(packageId);
	if (versions == null || versions.Count == 0)
		return false;
	var version = versions.FirstOrDefault(v => !v.IsPrerelease && v.Version == reference.Version)
		?? versions.LastOrDefault(v => !v.IsPrerelease && v.Major == reference.Version?.Major)
		?? versions.LastOrDefault(v => !v.IsPrerelease) ?? versions[^1];
	try
	{
		var pkgDir = await GetPackage(packageId, version);
		var pick = PickLib(pkgDir, matchTarget);
		if (pick != null)
		{
			Console.WriteLine($"  + undeclared dependency {reference.Name} -> {packageId} {version} ({pick.Value.fw.GetShortFolderName()})");
			lock (dirs)
			{
				dirs.Add(pick.Value.dir);
			}
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine($"  ! undeclared dependency {reference.Name}: {ex.Message}");
	}
	return Satisfied();
}

// Adds the ref/<tfm> directory of a framework ref pack (WindowsDesktop, AspNetCore)
// matching the module's .NET version. No-op if already added or unavailable.
async Task AddRefPack(string packId, Version coreVersion, List<string> searchDirs)
{
	var versions = await GetVersions(packId);
	if (versions == null)
		return;
	// Never fall back to "newest available": the ref packs only go back to 3.0, so a
	// netcoreapp1.x/2.x assembly would silently bind against a current (or prerelease)
	// BCL and decompile as a wall of "Unknown result type". Better to add nothing and
	// say so than to resolve against the wrong framework.
	var version = versions.LastOrDefault(v => !v.IsPrerelease && v.Major == coreVersion.Major && v.Minor == coreVersion.Minor)
		?? versions.LastOrDefault(v => !v.IsPrerelease && v.Major <= coreVersion.Major);
	if (version == null)
	{
		Console.WriteLine($"  ! ref pack {packId}: nothing published for {coreVersion}, references may bind to the wrong framework");
		return;
	}
	try
	{
		var dir = await GetPackage(packId, version);
		var refRoot = Path.Combine(dir, "ref");
		var refDir = Directory.Exists(refRoot) ? Directory.GetDirectories(refRoot).FirstOrDefault() : null;
		if (refDir != null && !searchDirs.Contains(refDir))
		{
			Console.WriteLine($"  + ref pack {packId} {version}");
			// Microsoft.NETCore.App.Ref ships stub facades -- its WindowsBase.dll is 15 KB and
			// has no DependencyObject -- which shadow the real assemblies and collapse whole
			// type hierarchies to Unknown. Keep it behind every other ref pack.
			int netCorePack = searchDirs.FindIndex(
				d => d.Contains(NetCoreRefPack, StringComparison.OrdinalIgnoreCase));
			if (netCorePack >= 0 && packId != NetCoreRefPack)
				searchDirs.Insert(netCorePack, refDir);
			else
				searchDirs.Add(refDir);
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine($"  ! ref pack {packId}: {ex.Message}");
	}
}

async Task<string> GetPackage(string id, NuGetVersion version)
{
	var idLower = id.ToLowerInvariant();
	var v = version.ToNormalizedString().ToLowerInvariant();
	// The machine-wide NuGet cache is checked first: every `dotnet restore` on this box
	// already extracts packages there in the same layout, so anything a build has pulled
	// in never gets downloaded a second time. Our own cache stays the write target -
	// nothing here ever writes into the shared one.
	var globalDir = Path.Combine(globalPackagesRoot, idLower, v);
	if (Directory.Exists(globalDir))
		return globalDir;
	var dir = Path.Combine(cacheRoot, idLower, v);
	if (!Directory.Exists(dir))
	{
		Console.WriteLine($"  downloading {id} {v}");
		var bytes = await WithRetry(() => http.GetByteArrayAsync(
			$"https://api.nuget.org/v3-flatcontainer/{idLower}/{v}/{idLower}.{v}.nupkg"));
		var tmp = dir + ".tmp";
		if (Directory.Exists(tmp))
			Directory.Delete(tmp, true);
		ZipFile.ExtractToDirectory(new MemoryStream(bytes), tmp);
		Directory.Move(tmp, dir);
	}
	return dir;
}

async Task DecompileAssembly(string pkg, string dllPath, List<string> searchDirs, NuGetFramework matchTarget, string? fallbackDir)
{
	var name = Path.GetFileName(dllPath);
	PEFile module;
	try
	{
		module = new PEFile(dllPath);
	}
	catch (Exception ex) when (ex is BadImageFormatException or MetadataFileNotSupportedException)
	{
		Console.WriteLine($"  skip {name}: not a managed assembly");
		return;
	}
	using (module)
	{
		// The file name alone is ambiguous across a sweep: the same simple name ships in many
		// packages and many TFM folders. Identify findings by full assembly name plus path.
		name = $"{module.FullName} ({dllPath})";
		// ".NETCoreApp,Version=v5.0" -> 5.0; null for .NET Framework / netstandard modules.
		Version? coreVersion = null;
		var tfmId = module.DetectTargetFrameworkId();
		var versionIndex = tfmId.IndexOf("Version=v", StringComparison.Ordinal);
		if (tfmId.StartsWith(".NETCoreApp", StringComparison.Ordinal) && versionIndex >= 0)
			coreVersion = Version.Parse(tfmId[(versionIndex + 9)..]);

		var resolver = new UniversalAssemblyResolver(dllPath, throwOnError: false, tfmId);
		var orderedDirs = new List<string>(searchDirs);
		// Bind to the ref packs of the framework the assembly was built for. Otherwise the
		// references still resolve -- against whatever shared runtime happens to be
		// installed -- and every type that moved or was removed since then decompiles as
		// "Unknown result type", which is indistinguishable from a decompiler bug.
		// The desktop and web packs have to be seeded here rather than left to
		// TryFetchMissingRef, which only runs when a reference resolves nowhere: the
		// facades described in AddRefPack satisfy those references, so it never fires.
		if (coreVersion != null)
		{
			var refNames = module.AssemblyReferences.Select(r => r.Name).ToList();
			if (refNames.Any(n => windowsDesktopPrefixes.Any(p => n.StartsWith(p, StringComparison.Ordinal))))
				await AddRefPack("Microsoft.WindowsDesktop.App.Ref", coreVersion, orderedDirs);
			if (refNames.Any(n => n.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal)))
				await AddRefPack("Microsoft.AspNetCore.App.Ref", coreVersion, orderedDirs);
			await AddRefPack(NetCoreRefPack, coreVersion, orderedDirs);
		}
		// Last-resort only, for the same reason the ref packs are ordered as they are.
		if (fallbackDir != null)
			orderedDirs.Add(fallbackDir);
		foreach (var d in orderedDirs)
			resolver.AddSearchDirectory(d);
		var fetchGate = new object();
		var logResolver = new LoggingResolver(resolver, orderedDirs, reference => {
			lock (fetchGate)
			{
				return TryFetchMissingRef(reference, coreVersion, matchTarget, orderedDirs)
					.GetAwaiter().GetResult();
			}
		});
		CSharpDecompiler decompiler;
		try
		{
			decompiler = new CSharpDecompiler(module, logResolver, new DecompilerSettings());
		}
		catch (Exception ex)
		{
			Report(pkg, name, "<typesystem>", ex);
			return;
		}
		Console.WriteLine($"  {name}");
		assemblyCount++;
		if (pdbMode)
		{
			CheckGeneratedPdb(pkg, name, module, decompiler, dllPath, orderedDirs);
			ReportResolutions(logResolver, dllPath, orderedDirs);
			return;
		}
		foreach (var type in decompiler.TypeSystem.MainModule.TopLevelTypeDefinitions.ToList())
		{
			using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
			decompiler.CancellationToken = cts.Token;
			try
			{
				var tree = decompiler.DecompileType(type.FullTypeName);
				var code = SyntaxTreeToString(tree);
				typeCount++;
				charCount += code.Length;
				// Compiler-generated types (<Module>, <PrivateImplementationDetails>,
				// VB$AnonymousType_*, ...) are still decompiled to shake out edge cases,
				// but empty output is normal for them ('<' and '$' match the decompiler's
				// own generated-name detection in SRMExtensions.IsGeneratedName).
				bool generatedType = type.Name.StartsWith('<') || type.Name.Contains('$');
				if (string.IsNullOrWhiteSpace(code) && !generatedType)
					Report(pkg, name, type.FullTypeName.ToString(), new InvalidDataException("empty decompilation output"));
				else if (dumpDir != null)
					File.WriteAllText(Path.Combine(dumpDir, SanitizeFileName($"{pkg}.{type.FullTypeName}.cs")), code);
				// An identifier may only contain letters, digits and '_' (the rule
				// EscapeInvalidIdentifiers.IsValid applies for project output). Any other name in
				// the tree is a compiler-generated entity the decompiler failed to fold away, and
				// the output does not compile. Inside a generated type everything is mangled by
				// definition, so only user-written types are checked.
				if (!generatedType)
				{
					foreach (var leak in tree.DescendantsAndSelf.OfType<Identifier>()
						.Select(i => i.Name)
						.Where(n => !n.All(ch => char.IsLetterOrDigit(ch) || ch == '_'))
						.Distinct())
					{
						Report(pkg, name, type.FullTypeName.ToString(), new LeakedName(leak));
					}
				}
				// ILFunction warnings (unknown result types, stack type mismatches, invalid IL)
				// surface in the output as "//IL_xxxx: <message>" comments.
				foreach (var warning in Regex.Matches(code, @"//IL_[0-9a-fA-F]+: (.*)")
					.Select(m => m.Groups[1].Value.Trim()).Distinct())
				{
					Report(pkg, name, type.FullTypeName.ToString(), new DecompilerWarning(warning));
				}
			}
			catch (OperationCanceledException)
			{
				Report(pkg, name, type.FullTypeName.ToString(), new TimeoutException("decompilation timed out (60s)"));
			}
			catch (Exception ex)
			{
				Report(pkg, name, type.FullTypeName.ToString(), ex);
			}
		}
		ReportResolutions(logResolver, dllPath, orderedDirs);
	}
}

string SyntaxTreeToString(SyntaxTree syntaxTree)
{
	var w = new StringWriter();
	syntaxTree.AcceptVisitor(new CSharpOutputVisitor(w, formatting));
	return w.ToString();
}

void ReportResolutions(LoggingResolver logResolver, string dllPath, List<string> orderedDirs)
{
	var resolutions = logResolver.Resolutions;
	var unresolved = resolutions.Where(kv => kv.Value == null).Select(kv => kv.Key).OrderBy(k => k).ToList();
	// What it takes to reproduce a finding by hand: the assembly it came from and the
	// references it was decompiled against, which are what the report is read for once
	// a warning turns out to be a reference problem rather than a decompiler defect.
	var context = new StringBuilder($"assembly: {dllPath}");
	foreach (var dir in orderedDirs)
		context.Append($"{Environment.NewLine}  -r {dir}");
	foreach (var (refName, refPath) in resolutions.OrderBy(kv => kv.Key, StringComparer.Ordinal))
		context.Append($"{Environment.NewLine}  {refName} -> {refPath ?? "NOT FOUND"}");
	foreach (var key in pendingContext)
		failures[key] = failures[key] with { Context = context.ToString() };
	pendingContext.Clear();
	refsTotal += resolutions.Count;
	refsResolved += resolutions.Count - unresolved.Count;
	Console.WriteLine($"    refs: {resolutions.Count - unresolved.Count}/{resolutions.Count} resolved");
	if (verbose)
	{
		foreach (var kv in resolutions.OrderBy(kv => kv.Key))
			Console.WriteLine($"      {kv.Key} -> {kv.Value ?? "NOT FOUND"}");
	}
	else
	{
		foreach (var u in unresolved)
			Console.WriteLine($"    ! unresolved: {u}");
	}
}

// ---------------------------------------------------------------------------------------------
// PDB verification. Two independent checks over the same PDB: the Cecil round-trip answers "can
// the consumer ILLink uses read this at all", which is how #2823 surfaced; the lint answers "is
// what it reads true of the assembly", which a crash-free but wrong PDB still fails.
// ---------------------------------------------------------------------------------------------

void CheckGeneratedPdb(string pkg, string asm, PEFile module, CSharpDecompiler decompiler,
	string dllPath, List<string> searchDirs)
{
	// The writer takes the PDB id from the PE's CodeView debug directory entry, and a consumer
	// rejects a PDB whose id does not match that entry, so without one there is nothing to check
	// - the same reason ilspycmd refuses these assemblies.
	if (!PortablePdbWriter.HasCodeViewDebugDirectoryEntry(module))
	{
		Console.WriteLine("    skip: no CodeView debug directory entry");
		pdbSkipped++;
		return;
	}
	var pdbStream = new MemoryStream();
	var asserts = new List<string>();
	using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10)))
	{
		decompiler.CancellationToken = cts.Token;
		Trace.Listeners.Clear();
		Trace.Listeners.Add(new CollectAsserts(asserts));
		try
		{
			new PortablePdbWriter { NoLogo = true }
				.WritePdb(module, decompiler, new DecompilerSettings(), pdbStream);
		}
		catch (OperationCanceledException)
		{
			Report(pkg, asm, "<pdb>", new TimeoutException("PDB generation timed out (10min)"));
			return;
		}
		catch (Exception ex)
		{
			Report(pkg, asm, "<pdb>", ex);
			return;
		}
		finally
		{
			Trace.Listeners.Clear();
			Trace.Listeners.Add(new ThrowOnAssert());
		}
	}
	// Metadata tokens in an assertion message vary per method; without normalising them one
	// defect would fill the ledger with a finding per method it fired on.
	foreach (var message in asserts.Select(m => Regex.Replace(m, @"\b[0-9A-Fa-f]{8}\b", "<token>")).Distinct())
		Report(pkg, asm, "<pdb>", new AssertionFailedException(message));

	pdbChecked++;
	var bodies = CollectBodyFacts(dllPath, searchDirs);
	CecilRoundTrip(pkg, asm, dllPath, pdbStream, searchDirs);
	pdbStream.Position = 0;
	using var provider = MetadataReaderProvider.FromPortablePdbStream(pdbStream);
	LintPdb(pkg, asm, module.Metadata, provider.GetMetadataReader(), bodies, EntryPoint(module.Reader));
}

// Runs the same two checks against the PDB an assembly already ships with. This is how the lint
// is calibrated: a PDB the C# compiler wrote must produce no findings at all, so anything
// reported here is a defect in the lint rather than in ILSpy.
void LintExistingPdb(string dllPath)
{
	using var peStream = File.OpenRead(dllPath);
	PEReader peReader;
	try
	{
		peReader = new PEReader(peStream);
		if (!peReader.HasMetadata)
			return;
	}
	catch (BadImageFormatException)
	{
		return;
	}
	using (peReader)
	{
		var asm = $"{peReader.GetMetadataReader().GetFullAssemblyName()} ({dllPath})";
		// A PDB next to the assembly if there is one, otherwise the one embedded in the PE.
		MemoryStream? pdbStream = null;
		MetadataReaderProvider? provider = null;
		var pdbPath = Path.ChangeExtension(dllPath, ".pdb");
		try
		{
			if (File.Exists(pdbPath))
			{
				var bytes = File.ReadAllBytes(pdbPath);
				// "BSJB": a Windows PDB is a different format the portable reader cannot open.
				if (bytes.Length < 4 || BitConverter.ToUInt32(bytes, 0) != 0x424A5342)
				{
					pdbSkipped++;
					return;
				}
				pdbStream = new MemoryStream(bytes);
				provider = MetadataReaderProvider.FromPortablePdbStream(pdbStream, MetadataStreamOptions.LeaveOpen);
			}
			else
			{
				var embedded = peReader.ReadDebugDirectory()
					.FirstOrDefault(e => e.Type == DebugDirectoryEntryType.EmbeddedPortablePdb);
				if (embedded.Type != DebugDirectoryEntryType.EmbeddedPortablePdb)
					return;
				provider = peReader.ReadEmbeddedPortablePdbDebugDirectoryData(embedded);
			}
		}
		catch (Exception ex) when (ex is IOException or BadImageFormatException)
		{
			pdbSkipped++;
			return;
		}
		using (provider)
		using (pdbStream)
		{
			Console.WriteLine($"  {asm}");
			assemblyCount++;
			pdbChecked++;
			var bodies = CollectBodyFacts(dllPath, null);
			CecilRoundTrip("-", asm, dllPath, pdbStream, null);
			LintPdb("-", asm, peReader.GetMetadataReader(), provider.GetMetadataReader(), bodies,
				EntryPoint(peReader));
		}
	}
}

Cecil.ReaderParameters CecilParameters(List<string>? searchDirs, Stream? symbols, bool readSymbols)
{
	var resolver = new Cecil.DefaultAssemblyResolver();
	foreach (var dir in searchDirs ?? [])
		resolver.AddSearchDirectory(dir);
	var parameters = new Cecil.ReaderParameters { AssemblyResolver = resolver };
	if (!readSymbols)
		return parameters;
	parameters.ReadSymbols = true;
	if (symbols != null)
	{
		symbols.Position = 0;
		parameters.SymbolReaderProvider = new CecilCil.PortablePdbReaderProvider();
		parameters.SymbolStream = symbols;
	}
	else
	{
		parameters.SymbolReaderProvider = new CecilCil.EmbeddedPortablePdbReaderProvider();
	}
	return parameters;
}

// Parses every method body without symbols, so the lint has the assembly's own truth about
// instruction boundaries, exception handlers and local counts to check the PDB against - and
// still has it for the very methods whose debug information breaks the round-trip below.
Dictionary<int, BodyFacts> CollectBodyFacts(string dllPath, List<string>? searchDirs)
{
	var facts = new Dictionary<int, BodyFacts>();
	try
	{
		using var assembly = Cecil.AssemblyDefinition.ReadAssembly(dllPath, CecilParameters(searchDirs, null, readSymbols: false));
		foreach (var type in assembly.MainModule.GetTypes())
		{
			foreach (var method in type.Methods)
			{
				if (!method.HasBody)
					continue;
				try
				{
					var body = method.Body;
					facts[(int)method.MetadataToken.RID] = new BodyFacts(
						body.CodeSize,
						body.Instructions.Select(i => i.Offset).ToHashSet(),
						body.ExceptionHandlers
							.Where(h => h.HandlerType == CecilCil.ExceptionHandlerType.Catch)
							.Select(h => h.HandlerStart.Offset).ToHashSet(),
						body.Variables.Count);
				}
				catch (Exception)
				{
					// A body that will not parse without symbols says nothing about the PDB; the
					// lint simply has no facts to check that one method against.
				}
			}
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine($"    ! body scan failed: {ex.GetType().Name}: {FirstLine(ex.Message)}");
	}
	return facts;
}

// Reads every method body through the PDB, which is what makes Cecil decode the custom debug
// information attached to it. This is the path that fails in #2823 and inside ILLink.
void CecilRoundTrip(string pkg, string asm, string dllPath, Stream? pdbStream, List<string>? searchDirs)
{
	Cecil.AssemblyDefinition assembly;
	try
	{
		assembly = Cecil.AssemblyDefinition.ReadAssembly(dllPath, CecilParameters(searchDirs, pdbStream, readSymbols: true));
	}
	catch (CecilCil.SymbolsNotMatchingException)
	{
		Report(pkg, asm, "<pdb>", new PdbFinding(
			"[MATCH] the PDB does not match the assembly's CodeView debug directory entry"));
		return;
	}
	catch (Exception ex)
	{
		Report(pkg, asm, "<pdb>", new PdbFinding(
			$"[CECIL] {ex.GetType().Name} opening the assembly with the PDB: {FirstLine(ex.Message)}"));
		return;
	}
	using (assembly)
	{
		foreach (var type in assembly.MainModule.GetTypes())
		{
			foreach (var method in type.Methods)
			{
				if (!method.HasBody)
					continue;
				try
				{
					_ = method.Body;
					_ = method.DebugInformation.SequencePoints;
				}
				catch (Cecil.AssemblyResolutionException)
				{
					// A reference the corpus does not contain, not a defect in the PDB.
				}
				catch (Exception ex)
				{
					// The Cecil frame separates distinct decode failures; the method name goes
					// into the location, so the message stays the same for all of them.
					var frame = (ex.StackTrace ?? "").Split('\n').Select(l => l.Trim())
						.FirstOrDefault(l => l.Contains("Mono.Cecil")) ?? "";
					Report(pkg, asm, method.FullName, new PdbFinding(
						$"[CECIL] {ex.GetType().Name} reading a method body through the PDB @ {frame}"));
				}
			}
		}
	}
}

// Structural lint: everything the PDB claims has to be true of the assembly it describes. The
// category is a prefix on the message, so one finding kind covers all of them in the report.
void LintPdb(string pkg, string asm, MetadataReader pe, MetadataReader pdb, Dictionary<int, BodyFacts> bodies,
	MethodDefinitionHandle entryPoint)
{
	var sourceLines = new Dictionary<int, string[]?>();

	string MethodName(int rid)
	{
		if (rid <= 0 || rid > pe.MethodDefinitions.Count)
			return "<pdb>";
		var method = pe.GetMethodDefinition(MetadataTokens.MethodDefinitionHandle(rid));
		var type = pe.GetTypeDefinition(method.GetDeclaringType());
		var ns = pe.GetString(type.Namespace);
		return $"{(ns.Length > 0 ? ns + "." : "")}{pe.GetString(type.Name)}.{pe.GetString(method.Name)}";
	}

	// One report per method per defect: a method with hundreds of sequence points would
	// otherwise contribute hundreds of hits for a single mistake.
	void Once(HashSet<string> seen, int rid, string message)
	{
		if (seen.Add(message))
			Report(pkg, asm, MethodName(rid), new PdbFinding(message));
	}

	// The embedded source, as lines, for checking that sequence points point at real text.
	string[]? EmbeddedLines(DocumentHandle handle)
	{
		int key = MetadataTokens.GetRowNumber(handle);
		if (sourceLines.TryGetValue(key, out var cached))
			return cached;
		string[]? lines = null;
		foreach (var infoHandle in pdb.GetCustomDebugInformation(handle))
		{
			var info = pdb.GetCustomDebugInformation(infoHandle);
			if (pdb.GetGuid(info.Kind) != KnownGuids.EmbeddedSource)
				continue;
			var blob = pdb.GetBlobBytes(info.Value);
			if (blob.Length < 4)
				break;
			// int32 uncompressed size, then the bytes - deflated when the size is non-zero.
			int uncompressedSize = BitConverter.ToInt32(blob, 0);
			using var raw = new MemoryStream(blob, 4, blob.Length - 4);
			using Stream content = uncompressedSize > 0
				? new DeflateStream(raw, CompressionMode.Decompress)
				: raw;
			using var reader = new StreamReader(content, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
			lines = reader.ReadToEnd().Replace("\r\n", "\n").Split('\n');
			break;
		}
		sourceLines[key] = lines;
		return lines;
	}

	// The MethodDebugInformation table is parallel to MethodDef: a consumer indexes it by
	// method row, so a short table silently shifts every method's debug info.
	if (pdb.MethodDebugInformation.Count != pe.MethodDefinitions.Count)
	{
		Report(pkg, asm, "<pdb>", new PdbFinding(
			"[DOCS] the MethodDebugInformation row count differs from the MethodDef row count"));
	}

	bool anySequencePoints = false;
	foreach (var infoHandle in pdb.MethodDebugInformation)
	{
		var info = pdb.GetMethodDebugInformation(infoHandle);
		if (info.SequencePointsBlob.IsNil)
			continue;
		int rid = MetadataTokens.GetRowNumber(infoHandle);
		bodies.TryGetValue(rid, out var facts);
		var seen = new HashSet<string>();
		int previousOffset = -1;
		foreach (var point in info.GetSequencePoints())
		{
			anySequencePoints = true;
			// Offsets are delta-encoded, so a non-increasing one is not just out of order:
			// it aliases into the record that means "the document changed here".
			if (point.Offset <= previousOffset)
				Once(seen, rid, "[SEQPOINT] sequence point offsets are not strictly increasing");
			previousOffset = point.Offset;
			if (facts != null && !facts.InstructionOffsets.Contains(point.Offset))
				Once(seen, rid, "[OFFSET] a sequence point offset is not an instruction boundary");
			if (point.IsHidden)
				continue;
			if (point.StartLine > point.EndLine
				|| (point.StartLine == point.EndLine && point.StartColumn >= point.EndColumn))
			{
				Once(seen, rid, "[SEQPOINT] a sequence point span is empty or inverted");
			}
			var lines = EmbeddedLines(point.Document);
			if (lines == null)
				continue;
			if (point.EndLine > lines.Length)
				Once(seen, rid, "[SOURCE] a sequence point points past the end of the embedded source");
			else if (SpanIsBlank(lines, point.StartLine, point.StartColumn, point.EndLine, point.EndColumn))
				Once(seen, rid, "[SOURCE] a sequence point span covers only whitespace");
		}
	}
	if (anySequencePoints && pdb.Documents.Count == 0)
		Report(pkg, asm, "<pdb>", new PdbFinding("[DOCS] the PDB has sequence points but no documents"));

	// The table is sorted by method, then start offset, then descending length, so the
	// enclosing scope of a row is always the innermost one still open.
	int currentMethod = 0;
	var open = new List<(int Start, int End)>();
	var scopeSeen = new HashSet<string>();
	foreach (var scopeHandle in pdb.LocalScopes)
	{
		var scope = pdb.GetLocalScope(scopeHandle);
		int rid = MetadataTokens.GetRowNumber(scope.Method);
		if (rid != currentMethod)
		{
			currentMethod = rid;
			open.Clear();
			scopeSeen.Clear();
		}
		bodies.TryGetValue(rid, out var facts);
		int start = scope.StartOffset, end = scope.EndOffset;
		if (facts != null)
		{
			if (!facts.InstructionOffsets.Contains(start))
				Once(scopeSeen, rid, "[OFFSET] a local scope starts at an offset that is not an instruction boundary");
			if (end > facts.CodeSize)
				Once(scopeSeen, rid, "[OFFSET] a local scope extends past the end of the method body");
			else if (end != facts.CodeSize && !facts.InstructionOffsets.Contains(end))
				Once(scopeSeen, rid, "[OFFSET] a local scope ends at an offset that is not an instruction boundary");
		}
		while (open.Count > 0 && open[^1].End <= start)
			open.RemoveAt(open.Count - 1);
		if (open.Count > 0 && end > open[^1].End)
			Once(scopeSeen, rid, "[LOCALS] a local scope is not nested inside its enclosing scope");
		open.Add((start, end));
		var slots = new HashSet<int>();
		foreach (var variableHandle in scope.GetLocalVariables())
		{
			var variable = pdb.GetLocalVariable(variableHandle);
			if (facts != null && variable.Index >= facts.LocalCount)
				Once(scopeSeen, rid, "[LOCALS] a local variable slot is past the end of the method's local signature");
			if (!slots.Add(variable.Index))
				Once(scopeSeen, rid, "[LOCALS] two local variables in one scope share a slot");
		}
	}

	// The expression evaluator resolves unqualified names by walking a scope up to the single
	// module-level root. A second root means some scopes hang off nothing, and everything
	// under them resolves against an empty chain no matter what the other root holds.
	// (An import scope carrying no imports at all is legal - a file need not have usings.)
	int rootImportScopes = 0;
	foreach (var importScopeHandle in pdb.ImportScopes)
	{
		if (pdb.GetImportScope(importScopeHandle).Parent.IsNil)
			rootImportScopes++;
	}
	if (rootImportScopes > 1)
	{
		Report(pkg, asm, "<pdb>", new PdbFinding(
			"[IMPORTS] the import scope table has more than one root, so some scopes chain to nothing"));
	}

	foreach (var infoHandle in pdb.CustomDebugInformation)
	{
		var info = pdb.GetCustomDebugInformation(infoHandle);
		if (info.Parent.Kind != HandleKind.MethodDefinition)
			continue;
		int rid = MetadataTokens.GetRowNumber((MethodDefinitionHandle)info.Parent);
		bodies.TryGetValue(rid, out var facts);
		var kind = pdb.GetGuid(info.Kind);
		var seen = new HashSet<string>();
		if (kind == KnownGuids.MethodSteppingInformation)
			CheckAsyncStepping(pdb.GetBlobReader(info.Value), rid, facts, seen);
		else if (kind == KnownGuids.StateMachineHoistedLocalScopes)
			CheckHoistedScopes(pdb.GetBlobReader(info.Value), rid, facts, seen);
	}

	void CheckAsyncStepping(BlobReader reader, int rid, BodyFacts? facts, HashSet<string> seen)
	{
		try
		{
			if (reader.RemainingBytes < 4)
			{
				Once(seen, rid, "[ASYNC] the async stepping blob is truncated");
				return;
			}
			long catchHandler = reader.ReadUInt32();
			if (catchHandler != 0)
			{
				// The field is the handler's offset plus one, and only for async void
				// methods; 0 otherwise. A consumer decodes it as (value - 1), so a raw
				// offset resolves to an address in the middle of an instruction.
				var kickoff = pdb.GetMethodDebugInformation(MetadataTokens.MethodDebugInformationHandle(rid))
					.GetStateMachineKickoffMethod();
				// The compiler records a handler for an async void method and for an async entry
				// point - both are shapes nothing is expected to await, so the debugger should treat
				// the exception as user-unhandled - and an async entry point returns Task.
				if (!kickoff.IsNil && !IsUserEntryPoint(pe, kickoff, entryPoint) && !ReturnsVoid(pe, kickoff))
					Once(seen, rid, "[ASYNC] async stepping information records a catch handler for a method whose kickoff method does not return void");
				int decoded = (int)catchHandler - 1;
				if (facts != null && !facts.InstructionOffsets.Contains(decoded))
					Once(seen, rid, "[OFFSET] the async stepping catch handler does not decode to an instruction boundary");
				else if (facts != null && !facts.CatchHandlerStarts.Contains(decoded))
					Once(seen, rid, "[ASYNC] the async stepping catch handler does not decode to the start of a catch handler");
			}
			while (reader.RemainingBytes > 0)
			{
				int yield = (int)reader.ReadUInt32();
				int resume = (int)reader.ReadUInt32();
				int resumeMethod = reader.ReadCompressedInteger();
				if (facts != null && !facts.InstructionOffsets.Contains(yield))
					Once(seen, rid, "[OFFSET] an async yield offset is not an instruction boundary");
				if (facts != null && !facts.InstructionOffsets.Contains(resume))
					Once(seen, rid, "[OFFSET] an async resume offset is not an instruction boundary");
				if (resumeMethod != rid)
					Once(seen, rid, "[ASYNC] an async resume method is not the method carrying the stepping information");
			}
		}
		catch (BadImageFormatException)
		{
			Once(seen, rid, "[ASYNC] the async stepping blob is malformed");
		}
	}

	void CheckHoistedScopes(BlobReader reader, int rid, BodyFacts? facts, HashSet<string> seen)
	{
		int rows = 0;
		while (reader.RemainingBytes >= 8)
		{
			int start = (int)reader.ReadUInt32();
			int length = (int)reader.ReadUInt32();
			rows++;
			if (facts != null && start + length > facts.CodeSize)
				Once(seen, rid, "[OFFSET] a state machine hoisted local scope extends past the end of the method body");
		}
		// The debugger indexes this table by the slot number it parses out of the state
		// machine's own field names, so rows missing at the end drop those locals entirely.
		if (rows < HoistedSlotCount(pe, rid))
			Once(seen, rid, "[STATEMACHINE] the state machine hoisted local scope table has fewer rows than the state machine has hoisted slots");
	}
}

// True when every character the span covers is whitespace - a sequence point the debugger would
// highlight as an empty stretch of the source it is embedded next to.
static bool SpanIsBlank(string[] lines, int startLine, int startColumn, int endLine, int endColumn)
{
	for (int line = startLine; line <= endLine && line <= lines.Length; line++)
	{
		var text = lines[line - 1];
		int from = line == startLine ? Math.Min(startColumn - 1, text.Length) : 0;
		int to = line == endLine ? Math.Min(endColumn - 1, text.Length) : text.Length;
		if (to > from && text[from..to].Trim().Length > 0)
			return false;
	}
	return true;
}

// A debugger finds a hoisted local by parsing the slot number N out of the state machine's own
// field names and reading row N-1 of the hoisted scope table, so the table has to reach the
// highest slot the debugger will ask for. Only user-visible hoisted fields are ever asked for:
// <name>5__N holds a hoisted local and <>8__N a hoisted display class, while <>s__N compiler
// temporaries and <>u__N awaiters share the same slot counter but are never looked up - which is
// why the compiler's own table stops at the last user-visible slot rather than at the last field.
static int HoistedSlotCount(MetadataReader pe, int methodRid)
{
	var method = pe.GetMethodDefinition(MetadataTokens.MethodDefinitionHandle(methodRid));
	int max = 0;
	foreach (var fieldHandle in pe.GetTypeDefinition(method.GetDeclaringType()).GetFields())
	{
		var match = Regex.Match(pe.GetString(pe.GetFieldDefinition(fieldHandle).Name), @"^<.*>[58]__([0-9]+)$");
		if (match.Success)
			max = Math.Max(max, int.Parse(match.Groups[1].Value));
	}
	return max;
}

// The assembly's entry point, or nil when there is none or it lives in another module of a
// multi-module assembly (where the token is a File token rather than a method definition).
static MethodDefinitionHandle EntryPoint(PEReader pe)
{
	int token = pe.PEHeaders.CorHeader?.EntryPointTokenOrRelativeVirtualAddress ?? 0;
	return (token >> 24) == 0x06 && (token & 0xFFFFFF) != 0
		? MetadataTokens.MethodDefinitionHandle(token & 0xFFFFFF)
		: default;
}

// Whether a state machine's kickoff is the entry point as the user wrote it. An async entry
// point compiles to the user's method plus a '<Main>' wrapper that awaits it, and it is the
// wrapper the entry point token names, so the kickoff is recognised through it: same declaring
// type, and the only two names the compiler gives a user entry point.
static bool IsUserEntryPoint(MetadataReader pe, MethodDefinitionHandle kickoff, MethodDefinitionHandle entryPoint)
{
	if (entryPoint.IsNil)
		return false;
	if (kickoff == entryPoint)
		return true;
	var wrapper = pe.GetMethodDefinition(entryPoint);
	if (pe.GetString(wrapper.Name) != "<Main>")
		return false;
	var definition = pe.GetMethodDefinition(kickoff);
	var name = pe.GetString(definition.Name);
	return (name == "Main" || name == "<Main>$")
		&& definition.GetDeclaringType() == wrapper.GetDeclaringType();
}

// Reads just the return type out of a method signature: enough to tell an async void kickoff
// method from an async Task one, without a type system or a resolved reference closure.
static bool ReturnsVoid(MetadataReader pe, MethodDefinitionHandle handle)
{
	var reader = pe.GetBlobReader(pe.GetMethodDefinition(handle).Signature);
	var header = reader.ReadSignatureHeader();
	if (header.IsGeneric)
		reader.ReadCompressedInteger();
	reader.ReadCompressedInteger();
	while (reader.RemainingBytes > 0)
	{
		byte element = reader.ReadByte();
		// CMOD_REQD / CMOD_OPT may precede the return type.
		if (element is 0x1f or 0x20)
		{
			reader.ReadCompressedInteger();
			continue;
		}
		return element == 0x01;
	}
	return false;
}

// The paths of a corpus argument: a dll, or a directory scanned recursively for dlls.
static IEnumerable<string> ExpandDlls(IEnumerable<string> entries)
	=> entries
		.SelectMany(e => Directory.Exists(e)
			? Directory.EnumerateFiles(e, "*.dll", SearchOption.AllDirectories)
			: [e])
		.Where(f => !f.EndsWith(".resources.dll", StringComparison.OrdinalIgnoreCase))
		.Distinct()
		.OrderBy(f => f, StringComparer.Ordinal);

void Report(string pkg, string asm, string type, Exception ex)
{
	// Key on the innermost exception so the same defect hit via many members dedupes.
	var inner = ex;
	while (inner.InnerException != null)
		inner = inner.InnerException;
	var topFrame = (inner.StackTrace ?? "").Split('\n')
		.Select(l => l.Trim())
		.FirstOrDefault(l => l.Contains("ICSharpCode.Decompiler")) ?? "";
	var kind = inner is AssertionFailedException ? "ASSERT"
		: inner is TimeoutException ? "TIMEOUT"
		: inner is DecompilerWarning ? "WARNING"
		: inner is LeakedName ? "LEAK"
		: inner is PdbFinding ? "PDB" : "EXCEPTION";
	var key = $"{kind}|{inner.GetType().Name}|{inner.Message}|{topFrame}";
	var location = $"{pkg} / {asm} / {type}";
	if (failures.TryGetValue(key, out var existing))
	{
		failures[key] = existing with { Count = existing.Count + 1 };
		Console.WriteLine($"  [{kind}] (dup) {type}: {FirstLine(inner.Message)}");
	}
	else
	{
		failures[key] = new Finding(kind, inner.GetType().Name, FirstLine(inner.Message),
			FirstLine(topFrame), location, ex.ToString(), 1);
		pendingContext.Add(key);
		Console.WriteLine($"  [{kind}] {location}");
		foreach (var line in ex.ToString().Split('\n').Take(30))
			Console.WriteLine("      " + line.TrimEnd());
	}
}

// One JSON line per finding plus one totals line, appended under a lock so concurrent
// package runs of a sweep can share the file. JSONL because a sweep is append-only and
// may be interrupted at any point: a truncated last line costs one finding, not the file.
static void AppendToLedger(string path, IEnumerable<Finding> findings, int assemblies, int types,
	long refsResolved, long refsTotal)
{
	var lines = new List<string>();
	foreach (var f in findings)
	{
		// Carry the run's reference-resolution state on every finding: a warning produced
		// while references were missing is an artefact of the missing references far more
		// often than a decompiler defect ("might be due to ... missing references" is what
		// the warning itself says), and the report separates the two on this basis.
		lines.Add(JsonSerializer.Serialize(new LedgerEntry("finding", f.Kind, f.ExceptionType, f.Message,
			f.Frame, f.FirstLocation, f.Detail, f.Count, 0, 0, refsResolved, refsTotal, f.Context)));
	}
	lines.Add(JsonSerializer.Serialize(new LedgerEntry("totals", "", "", "", "", "", "", 0,
		assemblies, types, refsResolved, refsTotal)));
	var full = Path.GetFullPath(path);
	Directory.CreateDirectory(Path.GetDirectoryName(full)!);
	// Retry briefly: a sweep can have several package runs finishing at once.
	for (int attempt = 0; ; attempt++)
	{
		try
		{
			using var stream = new FileStream(full, FileMode.Append, FileAccess.Write, FileShare.Read);
			using var writer = new StreamWriter(stream);
			foreach (var line in lines)
				writer.WriteLine(line);
			return;
		}
		catch (IOException) when (attempt < 20)
		{
			Thread.Sleep(50);
		}
	}
}

// Aggregates a sweep's ledger: findings with the same kind/type/message/frame collapse
// into one row carrying the summed hit count, so a defect hit by 400 packages reads as
// one entry, which is the whole point of surveying a sweep.
static void RenderLedger(string ledgerPath, string outPath)
{
	var merged = new Dictionary<string, Finding>();
	var degraded = new HashSet<string>();   // findings only ever seen with references missing
	var clean = new HashSet<string>();
	int assemblies = 0, types = 0;
	long refsResolved = 0, refsTotal = 0;
	int malformed = 0;
	foreach (var line in File.ReadLines(ledgerPath))
	{
		LedgerEntry? entry;
		try
		{
			entry = JsonSerializer.Deserialize<LedgerEntry>(line);
		}
		catch (JsonException)
		{
			malformed++;   // truncated tail of an interrupted run
			continue;
		}
		if (entry == null)
			continue;
		if (entry.Record == "totals")
		{
			assemblies += entry.Assemblies;
			types += entry.Types;
			refsResolved += entry.RefsResolved;
			refsTotal += entry.RefsTotal;
			continue;
		}
		var key = $"{entry.Kind}|{entry.ExceptionType}|{entry.Message}|{entry.Frame}";
		merged[key] = merged.TryGetValue(key, out var existing)
			? existing with { Count = existing.Count + entry.Count }
			: new Finding(entry.Kind, entry.ExceptionType, entry.Message, entry.Frame,
				entry.FirstLocation, entry.Detail, entry.Count, entry.Context);
		// Ledger lines written before this attribution existed carry 0/0; treat those as
		// unknown rather than clean, so they are never presented as confirmed defects.
		(entry.RefsTotal > 0 && entry.RefsResolved == entry.RefsTotal ? clean : degraded).Add(key);
	}
	if (malformed > 0)
		Console.WriteLine($"  ({malformed} malformed ledger lines skipped)");
	// A finding seen even once with every reference resolved is trustworthy; one that only
	// ever appeared in degraded runs is suspect.
	degraded.ExceptWith(clean);
	WriteHtmlReport(outPath, merged.Values.ToList(), assemblies, types, refsResolved, refsTotal, null, degraded);
}

// Self-contained HTML view of a fuzz run: findings grouped by kind, most frequent
// first, each expandable to the full exception text. Opens straight off disk - a
// sweep over thousands of packages is far easier to triage here than in scrollback.
static void WriteHtmlReport(string path, List<Finding> findings, int assemblies, int types,
	long refsResolved, long refsTotal, string? dumpDir, HashSet<string>? degradedKeys = null)
{
	bool IsDegraded(Finding f) =>
		degradedKeys?.Contains($"{f.Kind}|{f.ExceptionType}|{f.Message}|{f.Frame}") == true;
	string Esc(string s) => s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
	var html = new StringBuilder();
	html.AppendLine("""
		<!doctype html><html><head><meta charset="utf-8"><title>nugetfuzz report</title>
		<style>
		:root { color-scheme: light dark; --bg:#fff; --fg:#1a1a1a; --muted:#666; --line:#d8d8d8; --chip:#f0f0f0; }
		@media (prefers-color-scheme: dark) { :root { --bg:#16181c; --fg:#e6e6e6; --muted:#9aa0a6; --line:#333; --chip:#24262b; } }
		body { background:var(--bg); color:var(--fg); font:14px/1.5 system-ui,sans-serif; margin:0 auto; padding:24px; max-width:1100px; }
		h1 { font-size:20px; margin:0 0 4px; } h2 { font-size:16px; margin:26px 0 8px; }
		.meta { color:var(--muted); font-size:13px; }
		details { border:1px solid var(--line); border-radius:6px; margin:6px 0; background:var(--chip); }
		summary { cursor:pointer; padding:8px 10px; font-family:ui-monospace,monospace; font-size:13px; }
		pre { margin:0; padding:10px; overflow-x:auto; background:var(--bg); font:12px/1.45 ui-monospace,monospace; }
		.count { display:inline-block; min-width:3.5em; font-weight:600; }
		.suspect { font-size:11px; padding:1px 6px; border-radius:10px; background:#8a6d1f22;
		           color:#a8791f; border:1px solid #a8791f55; margin-left:6px; }
		.ASSERT { border-left:4px solid #d97706; } .EXCEPTION { border-left:4px solid #dc2626; }
		.TIMEOUT { border-left:4px solid #7c3aed; } .WARNING { border-left:4px solid #2563eb; }
		.PDB { border-left:4px solid #0d9488; } .LEAK { border-left:4px solid #db2777; }
		#filter { width:100%; padding:8px; margin:8px 0; border:1px solid var(--line); border-radius:6px;
		          background:var(--bg); color:var(--fg); font:13px ui-monospace,monospace; }
		</style></head><body>
		""");
	html.AppendLine("<h1>nugetfuzz report</h1>");
	html.AppendLine($"<div class=meta>{assemblies} assemblies, {types} types decompiled, "
		+ $"{refsResolved}/{refsTotal} references resolved, {findings.Count} distinct findings "
		+ $"({findings.Sum(f => f.Count)} total)"
		+ (dumpDir != null ? $"<br>decompiled sources dumped to {Esc(dumpDir)}" : "") + "</div>");
	html.AppendLine("<input id=filter placeholder='filter by message, type, package or frame'>");
	foreach (var kind in new[] { "ASSERT", "EXCEPTION", "TIMEOUT", "LEAK", "WARNING", "PDB" })
	{
		var group = findings.Where(f => f.Kind == kind).OrderByDescending(f => f.Count).ToList();
		if (group.Count == 0)
			continue;
		html.AppendLine($"<h2>{kind} ({group.Count} distinct, {group.Sum(f => f.Count)} hits)</h2>");
		foreach (var f in group)
		{
			// Only ever seen while references were missing: flagged, not hidden - the
			// warning text itself blames missing references, so it is weak evidence.
			var suspect = IsDegraded(f)
				? " <span class=suspect title='only seen in runs with unresolved references'>refs incomplete</span>"
				: "";
			html.AppendLine($"<details class={kind}><summary><span class=count>{f.Count}x</span> "
				+ $"{Esc(f.ExceptionType)}: {Esc(f.Message)}{suspect}</summary>");
			var context = f.Context.Length > 0 ? $"{Esc(f.Context)}\n" : "";
			html.AppendLine($"<pre>first: {Esc(f.FirstLocation)}\nframe: {Esc(f.Frame)}\n{context}\n{Esc(f.Detail)}</pre></details>");
		}
	}
	html.AppendLine("""
		<script>
		const box = document.getElementById('filter');
		box.addEventListener('input', () => {
			const needle = box.value.toLowerCase();
			for (const d of document.querySelectorAll('details'))
				d.style.display = d.textContent.toLowerCase().includes(needle) ? '' : 'none';
		});
		</script>
		</body></html>
		""");
	File.WriteAllText(path, html.ToString());
}

static string SanitizeFileName(string s)
	=> string.Concat(s.Split(Path.GetInvalidFileNameChars()));

static string FirstLine(string s)
{
	var i = s.IndexOfAny(['\r', '\n']);
	return i < 0 ? s : s[..i];
}

// One deduplicated defect: Count counts every location that hit it, Detail keeps the
// full exception text of the first one for triage.
record Finding(string Kind, string ExceptionType, string Message, string Frame,
	string FirstLocation, string Detail, int Count, string Context = "")
{
	public string Describe()
		=> $"[{Kind}] {ExceptionType}: {Message} @ {Frame}  (first: {FirstLocation})";
}

// One line of the sweep ledger: either a deduplicated finding or a per-run totals record.
record LedgerEntry(string Record, string Kind, string ExceptionType, string Message, string Frame,
	string FirstLocation, string Detail, int Count, int Assemblies, int Types,
	long RefsResolved, long RefsTotal, string Context = "");

record VersionIndex(string[] versions);

// What the PDB lint needs to know about a method body, read from the assembly alone.
record BodyFacts(int CodeSize, HashSet<int> InstructionOffsets, HashSet<int> CatchHandlerStarts, int LocalCount);

class AssertionFailedException(string message) : Exception(message);

class DecompilerWarning(string message) : Exception(message);

// A compiler-generated name that reached the output. The message keeps only the shape of
// the name - the part in angle brackets is the enclosing member and the digits are per
// occurrence - so that one bucket collects every hit of the same unfolded construct.
class LeakedName(string name)
	: Exception("leaked compiler-generated name: "
		+ Regex.Replace(Regex.Replace(name, "<[^<>]*>", "<>"), "[0-9]+", "N"));

// A defect in a generated PDB. The message describes the shape of the defect and never the
// instance that hit it - Report() dedupes on the message, so naming a method or an offset in it
// would turn one bug into one finding per method.
class PdbFinding(string message) : Exception(message);

// Records assertion failures instead of throwing them. Debug.Assert unwinding out of the middle
// of PortablePdbWriter would leave no PDB to check, and the writer is known to assert on
// real-world input; with Trace.Listeners holding only this one, execution continues past the
// assert and still produces a PDB to look at. The call site has to be captured here: nothing
// throws, so there is no stack left to read afterwards, and a message-less Debug.Assert would
// otherwise be reported as an empty string naming no code at all.
class CollectAsserts(List<string> messages) : TraceListener
{
	public override void Fail(string? message, string? detailMessage)
	{
		var frame = Environment.StackTrace.Split('\n').Select(l => l.Trim())
			.FirstOrDefault(l => l.Contains("ICSharpCode.Decompiler")) ?? "";
		messages.Add($"{message} {detailMessage}".Trim() + $" @ {frame}");
	}
	public override void Write(string? message)
	{
	}
	public override void WriteLine(string? message)
	{
	}
}

// Resolves assembly references from the given directories (in priority order) before
// falling back to the wrapped resolver, and records every resolution and its outcome.
// The wrapped UniversalAssemblyResolver consults the installed runtime BEFORE its search
// directories, which lets runtime stub facades shadow ref-pack assemblies - hence the
// directory probing happens here, in our order.
class LoggingResolver(IAssemblyResolver inner, List<string> dirs, Func<IAssemblyReference, bool> onMiss) : IAssemblyResolver
{
	public readonly Dictionary<string, string?> Resolutions = new();
	readonly Dictionary<string, MetadataFile?> loaded = new();

	public IDisposable? BeginSnapshot() => inner.BeginSnapshot();

	public MetadataFile? Resolve(IAssemblyReference reference)
	{
		var file = ResolveFromDirs(reference) ?? inner.Resolve(reference);
		if (file == null && onMiss(reference))
		{
			lock (loaded)
			{
				loaded.Remove(reference.Name);
			}
			file = ResolveFromDirs(reference);
		}
		return Track(reference.FullName, file);
	}
	public MetadataFile? ResolveModule(MetadataFile mainModule, string moduleName)
		=> Track($"{mainModule.Name}!{moduleName}", inner.ResolveModule(mainModule, moduleName));
	public Task<MetadataFile?> ResolveAsync(IAssemblyReference reference)
		=> Task.FromResult(Resolve(reference));
	public Task<MetadataFile?> ResolveModuleAsync(MetadataFile mainModule, string moduleName)
		=> Task.FromResult(ResolveModule(mainModule, moduleName));

	MetadataFile? ResolveFromDirs(IAssemblyReference reference)
	{
		string[] snapshot;
		lock (dirs)
		{
			snapshot = dirs.ToArray();
		}
		lock (loaded)
		{
			if (loaded.TryGetValue(reference.Name, out var cached))
				return cached;
			MetadataFile? result = null;
			foreach (var dir in snapshot)
			{
				var path = Path.Combine(dir, reference.Name + ".dll");
				if (!File.Exists(path))
					continue;
				try
				{
					result = new PEFile(path);
					break;
				}
				catch (Exception)
				{
					// unreadable candidate; keep probing lower-priority dirs
				}
			}
			loaded[reference.Name] = result;
			return result;
		}
	}

	MetadataFile? Track(string key, MetadataFile? file)
	{
		lock (Resolutions)
		{
			Resolutions[key] = file?.FileName;
		}
		return file;
	}
}

class ThrowOnAssert : TraceListener
{
	public override void Fail(string? message, string? detailMessage)
		=> throw new AssertionFailedException($"{message} {detailMessage}".Trim());
	public override void Write(string? message)
	{
	}
	public override void WriteLine(string? message)
	{
	}
}
