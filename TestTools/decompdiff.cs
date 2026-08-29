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

#:property PublishAot=false

// decompdiff: decompiles a corpus of assemblies with TWO builds of
// ICSharpCode.Decompiler (loaded side-by-side via AssemblyLoadContext) and
// reports how the output differs, to assess quality/readability of decompiler
// changes across real-world code. The textual complement to the Windows
// round-trip tests, which verify correctness but not output quality.
//
// usage: dotnet run decompdiff.cs -- --old <commit-ish|ILSpy-checkout|Decompiler.dll> --new <...>
//                                    [-o <report-dir>] [--build] [--refs <dir>]... <dll|dir>...
//
// - checkout args are built on demand (Release; restore keeps packages.lock.json
//   whole via -p:RestoreEnablePackagePruning=false); pass --build to force rebuild.
// - a commit-ish (branch, tag, sha, FETCH_HEAD) is resolved against the repository
//   the tool is run from and checked out into a worktree under
//   ~/.cache/decompdiff/<repo>/<commit>, kept and reused so a rerun keeps the
//   Release build it already contains.
// - corpus dirs are scanned recursively for *.dll (e.g. ~/.cache/nugetfuzz).
// - changed/errored types are written to <report-dir>/{old,new}/...; inspect with
//   `git diff --no-index <report-dir>/old <report-dir>/new`, or open the generated
//   <report-dir>/index.html, which carries the same data with inline diffs.
// - exit code 1 when the new version has errors the old one didn't (regressions).
//
// Reference handling: every assembly is decompiled out of a staging directory that
// holds symlinks to itself, its original neighbours, and the transitive closure of
// its references as found in --refs directories, the machine-wide NuGet cache, and
// the reference-assembly pack matching the assembly's own target framework.
// UniversalAssemblyResolver searches
// that directory first (ResolveInternal -> SearchDirectory), so staging fixes both
// classic failure modes - a sibling package that does not sit next to the assembly,
// and the Windows-only mscorlib lookup that throws "Version not supported" on Linux
// - while still driving the stable 2-arg CSharpDecompiler ctor. Both sides read the
// SAME staging directory, so whatever stays unresolved degrades them identically
// and the diffs remain meaningful.

using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;

Trace.Listeners.Clear();
Trace.Listeners.Add(new ThrowOnAssert());
try
{
	Debug.Fail("self-test");
	Console.Error.WriteLine("FATAL: assert hook not active, Debug.Assert in Debug decompiler builds would kill the process");
	return 2;
}
catch (AssertionFailedException)
{
	// hook works
}

string? oldSpec = null, newSpec = null, reportDir = null;
bool forceBuild = false;
var corpus = new List<string>();
var refDirs = new List<string>();
for (int i = 0; i < args.Length; i++)
{
	switch (args[i])
	{
		case "--old":
			oldSpec = args[++i];
			break;
		case "--new":
			newSpec = args[++i];
			break;
		case "-o":
			reportDir = args[++i];
			break;
		case "--build":
			forceBuild = true;
			break;
		case "--refs":
			refDirs.Add(args[++i]);
			break;
		default:
			corpus.Add(args[i]);
			break;
	}
}
if (oldSpec == null || newSpec == null || corpus.Count == 0)
{
	Console.Error.WriteLine("usage: decompdiff --old <commit-ish|ILSpy-checkout|Decompiler.dll> --new <...> [-o report-dir] [--build] [--refs <dir>]... <dll|dir>...");
	return 1;
}
reportDir ??= "decompdiff-report";
if (Directory.Exists(reportDir))
{
	if (!File.Exists(Path.Combine(reportDir, "summary.md")) && Directory.EnumerateFileSystemEntries(reportDir).Any())
	{
		Console.Error.WriteLine($"refusing to reuse {reportDir}: exists, non-empty, and not a decompdiff report");
		return 1;
	}
	Directory.Delete(reportDir, true);
}
Directory.CreateDirectory(reportDir);

Side oldSide, newSide;
try
{
	oldSide = Side.Create("old", oldSpec, forceBuild);
	newSide = Side.Create("new", newSpec, forceBuild);
}
catch (ArgumentException ex)
{
	// A mistyped branch name is the easiest way to get here, and its message says more
	// than the stack trace does.
	Console.Error.WriteLine(ex.Message);
	return 1;
}
Console.WriteLine($"old: {oldSide.Description}");
Console.WriteLine($"new: {newSide.Description}");

var assemblies = corpus
	.SelectMany(a => Directory.Exists(a)
		? Directory.EnumerateFiles(a, "*.dll", SearchOption.AllDirectories)
		: [a])
	.Where(f => !f.EndsWith(".resources.dll", StringComparison.OrdinalIgnoreCase))
	.Distinct()
	.OrderBy(f => f)
	.ToList();
Console.WriteLine($"corpus: {assemblies.Count} assemblies");

var refIndex = new RefIndex(refDirs, corpus);
Console.WriteLine($"references: {refIndex.Description}");
var stageRoot = Path.Combine(reportDir, ".staging");
var unresolvedRefs = new SortedDictionary<string, List<string>>();  // assembly -> missing refs

int asmCount = 0, unchanged = 0;
var skipped = new List<string>();
var changed = new List<ChangedType>();
var transitions = new List<string>();  // fixed-error / NEW-ERROR / only-old / only-new lines
int newErrors = 0, fixedErrors = 0, bothErrors = 0;
var oldTotals = default(Metrics);
var newTotals = default(Metrics);

foreach (var dll in assemblies)
{
	var asmName = Path.GetFileNameWithoutExtension(dll);
	var (staged, missing) = RefIndex.Stage(dll, stageRoot, refIndex);
	if (missing.Count > 0)
		unresolvedRefs[asmName] = missing;
	var oldTypes = oldSide.DecompileAssembly(staged);
	if (oldTypes == null)
	{
		var why = missing.Count > 0 ? $"; unresolved refs: {string.Join(", ", missing.Take(5))}" : "";
		Console.WriteLine($"  skip {asmName}: not decompilable ({oldSide.LastAssemblyError}{why})");
		skipped.Add($"{asmName}: {oldSide.LastAssemblyError}{why}");
		continue;
	}
	var newTypes = newSide.DecompileAssembly(staged);
	if (newTypes == null)
	{
		Console.WriteLine($"  NEW-ERROR {asmName}: whole assembly failed only with new version ({newSide.LastAssemblyError})");
		transitions.Add($"NEW-ERROR (assembly) {asmName}: {newSide.LastAssemblyError}");
		newErrors++;
		continue;
	}
	asmCount++;
	int asmChanged = 0;
	foreach (var name in oldTypes.Keys.Union(newTypes.Keys).OrderBy(n => n))
	{
		var o = oldTypes.GetValueOrDefault(name);
		var n = newTypes.GetValueOrDefault(name);
		var location = $"{asmName} / {name}";
		if (o == null || n == null)
		{
			transitions.Add($"only-{(o != null ? "old" : "new")} {location}");
			continue;
		}
		if (o.Error != null || n.Error != null)
		{
			if (o.Error != null && n.Error != null)
			{
				bothErrors++;
				if (o.Error != n.Error)
					transitions.Add($"error-changed {location}: {o.Error} -> {n.Error}");
			}
			else if (o.Error != null)
			{
				fixedErrors++;
				transitions.Add($"fixed-error {location}: {o.Error}");
			}
			else
			{
				newErrors++;
				transitions.Add($"NEW-ERROR {location}: {n.Error}");
				DumpPair(location, o.Code!, $"// ERROR: {n.Error}");
			}
			continue;
		}
		oldTotals += o.Metrics;
		newTotals += n.Metrics;
		if (o.Code == n.Code)
		{
			unchanged++;
			continue;
		}
		asmChanged++;
		changed.Add(new ChangedType(location, o.Metrics, n.Metrics));
		DumpPair(location, o.Code!, n.Code!);
	}
	Console.WriteLine($"  {asmName}: {oldTypes.Count} types, {asmChanged} changed");
}

var summary = new StringBuilder();
summary.AppendLine("# decompdiff report");
summary.AppendLine();
summary.AppendLine($"- old: {oldSide.Description}");
summary.AppendLine($"- new: {newSide.Description}");
summary.AppendLine($"- corpus: {asmCount} assemblies, {unchanged + changed.Count} types compared");
summary.AppendLine();
summary.AppendLine($"| | old | new | delta |");
summary.AppendLine($"|---|---|---|---|");
summary.AppendLine(MetricRow("lines", oldTotals.Lines, newTotals.Lines));
summary.AppendLine(MetricRow("goto statements", oldTotals.Gotos, newTotals.Gotos));
summary.AppendLine(MetricRow("//IL_ warning comments", oldTotals.IlWarnings, newTotals.IlWarnings));
summary.AppendLine(MetricRow("compiler-generated name leaks (<>)", oldTotals.GeneratedNames, newTotals.GeneratedNames));
summary.AppendLine();
summary.AppendLine($"types: {unchanged} unchanged, {changed.Count} changed, {newErrors} NEW errors, {fixedErrors} fixed errors, {bothErrors} errored in both");
summary.AppendLine();
if (changed.Count > 0)
{
	summary.AppendLine("## Changed types (largest line delta first)");
	summary.AppendLine();
	foreach (var c in changed.OrderByDescending(c => Math.Abs(c.New.Lines - c.Old.Lines)).Take(50))
		summary.AppendLine($"- {c.Location}: {c.Old.Lines} -> {c.New.Lines} lines" + MetricNotes(c));
	if (changed.Count > 50)
		summary.AppendLine($"- ... {changed.Count - 50} more, see {reportDir}/{{old,new}}/");
	summary.AppendLine();
}
if (transitions.Count > 0)
{
	summary.AppendLine("## Error / presence transitions");
	summary.AppendLine();
	foreach (var t in transitions)
		summary.AppendLine($"- {t}");
	summary.AppendLine();
}
if (skipped.Count > 0)
{
	summary.AppendLine("## Skipped assemblies (undecompilable on BOTH sides)");
	summary.AppendLine();
	foreach (var s in skipped)
		summary.AppendLine($"- {s}");
	summary.AppendLine();
}
if (unresolvedRefs.Count > 0)
{
	// Point at what would widen the corpus: each name here is a reference no --refs
	// directory, the NuGet cache, or a reference-assembly pack could supply.
	summary.AppendLine("## Unresolved references (add --refs dirs to cover these)");
	summary.AppendLine();
	foreach (var (asm, refs) in unresolvedRefs)
		summary.AppendLine($"- {asm}: {string.Join(", ", refs)}");
	summary.AppendLine();
}
summary.AppendLine($"inspect changed output with: git diff --no-index {reportDir}/old {reportDir}/new");
summary.AppendLine($"or open {Path.Combine(reportDir, "index.html")}");
File.WriteAllText(Path.Combine(reportDir, "summary.md"), summary.ToString());
Report.WriteHtml(Path.Combine(reportDir, "index.html"), new ReportModel(
	oldSide.Description, newSide.Description, asmCount, unchanged, changed, transitions,
	skipped, unresolvedRefs, oldTotals, newTotals, newErrors, fixedErrors, bothErrors, reportDir));

Console.WriteLine();
Console.Write(summary);
return newErrors > 0 ? 1 : 0;

void DumpPair(string location, string oldCode, string newCode)
{
	foreach (var (side, code) in new[] { ("old", oldCode), ("new", newCode) })
	{
		var path = Path.Combine(reportDir, side, Report.SanitizeFileName(location.Replace(" / ", "/")) + ".cs");
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		File.WriteAllText(path, code);
	}
}

static string MetricRow(string name, long oldValue, long newValue)
	=> $"| {name} | {oldValue} | {newValue} | {newValue - oldValue:+#;-#;0} |";

static string MetricNotes(ChangedType c)
{
	var notes = new List<string>();
	if (c.New.Gotos != c.Old.Gotos)
		notes.Add($"gotos {c.Old.Gotos}->{c.New.Gotos}");
	if (c.New.IlWarnings != c.Old.IlWarnings)
		notes.Add($"IL warnings {c.Old.IlWarnings}->{c.New.IlWarnings}");
	if (c.New.GeneratedNames != c.Old.GeneratedNames)
		notes.Add($"name leaks {c.Old.GeneratedNames}->{c.New.GeneratedNames}");
	return notes.Count > 0 ? $" ({string.Join(", ", notes)})" : "";
}

// Locates reference assemblies by simple name and stages them next to the assembly
// being decompiled. Sources, in order: the reference-assembly pack for the assembly's
// own target framework, the --refs directories (indexed once), and the machine-wide
// NuGet cache (probed per name, so nothing scans ~40k packages).
//
// The pack has to be chosen per assembly and has to win: a net9.0 assembly resolved
// against the .NET Framework packs finds mscorlib but not ValueTask or the async
// method builders, which collapses whole type hierarchies to Unknown and leaves
// AsyncAwaitDecompiler unable to recognise a state machine at all. The packs are also
// what makes classic net4x assemblies decompilable on Linux, whose mscorlib otherwise
// only exists behind a Windows path lookup.
sealed class RefIndex
{
	readonly Dictionary<string, string> byName = new(StringComparer.OrdinalIgnoreCase);
	readonly List<string> probeRoots = new();
	readonly string nugetRoot;
	// Reference assemblies are picked per corpus assembly, keyed by its TargetFrameworkAttribute:
	// a net9.0 assembly resolved against the .NET Framework packs finds mscorlib but not ValueTask
	// or the async method builders, which collapses whole type hierarchies to Unknown and leaves
	// AsyncAwaitDecompiler unable to recognise a state machine at all.
	readonly Dictionary<string, Dictionary<string, string>> byFramework = new(StringComparer.OrdinalIgnoreCase);
	public string Description { get; }

	public RefIndex(List<string> refDirs, List<string> corpus)
	{
		foreach (var dir in refDirs.Concat(corpus).Where(Directory.Exists))
			IndexDirectory(dir, byName);
		nugetRoot = Environment.GetEnvironmentVariable("NUGET_PACKAGES")
			?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
		if (Directory.Exists(nugetRoot))
			probeRoots.Add(nugetRoot);
		Description = $"{byName.Count} assemblies indexed"
			+ (probeRoots.Count > 0 ? $", NuGet cache probe at {string.Join(", ", probeRoots)}" : "");
	}

	// The reference assemblies for one target framework moniker, indexed by simple name.
	// Empty when no matching pack is installed, in which case resolution falls back to the
	// --refs directories, the corpus and the NuGet cache probe.
	public Dictionary<string, string> IndexFor(string? targetFramework)
	{
		var key = targetFramework ?? "";
		if (byFramework.TryGetValue(key, out var index))
			return index;
		index = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (var dir in RefPackDirs(key))
			IndexDirectory(dir, index);
		byFramework[key] = index;
		return index;
	}

	// Ref-pack directories for a TargetFrameworkAttribute value, most specific first.
	IEnumerable<string> RefPackDirs(string targetFramework)
	{
		var version = TfmVersion(targetFramework);
		if (targetFramework.StartsWith(".NETCoreApp", StringComparison.OrdinalIgnoreCase) && version != null)
		{
			// The WindowsDesktop and AspNetCore packs come first: Microsoft.NETCore.App.Ref ships
			// stub facades for their assemblies (its WindowsBase.dll has no DependencyObject), and
			// whichever is indexed first wins.
			foreach (var pack in new[] { "microsoft.windowsdesktop.app.ref", "microsoft.aspnetcore.app.ref", "microsoft.netcore.app.ref" })
			{
				var dir = CorePackDir(pack, version);
				if (dir != null)
					yield return dir;
			}
			yield break;
		}
		// .NETFramework and .NETStandard both resolve against the classic reference assemblies,
		// but only a .NETFramework version selects a pack: a .NETStandard version is not a net4x
		// version, and feeding 2.0 to the picker would pick net45, whose Facades carry no
		// netstandard.dll. netstandard targets take the newest pack, with the widest facade set.
		var netFxVersion = targetFramework.StartsWith(".NETFramework", StringComparison.OrdinalIgnoreCase)
			? version : null;
		foreach (var dir in EnumerateFrameworkRefPacks(nugetRoot, netFxVersion))
			yield return dir;
		// On Windows the same reference assemblies also ship with the targeting packs, so a
		// net4x corpus resolves there without restoring the NuGet package first.
		var installedFxRefs = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
			"Reference Assemblies", "Microsoft", "Framework", ".NETFramework");
		if (Directory.Exists(installedFxRefs))
			yield return installedFxRefs;
	}

	static Version? TfmVersion(string targetFramework)
	{
		var i = targetFramework.IndexOf("Version=v", StringComparison.OrdinalIgnoreCase);
		return i >= 0 && Version.TryParse(targetFramework[(i + 9)..], out var v) ? v : null;
	}

	// ref/<tfm> of a .NET (Core) shared-framework pack. Never falls back to "newest available":
	// the packs only go back to 3.0, so a netcoreapp1.x/2.x assembly would silently bind against
	// a current BCL and decompile as a wall of Unknown. Better to resolve nothing and say so.
	string? CorePackDir(string packId, Version version)
	{
		var root = Path.Combine(nugetRoot, packId);
		if (!Directory.Exists(root))
			return null;
		var versions = Directory.EnumerateDirectories(root)
			.Select(d => (Dir: d, V: Version.TryParse(Path.GetFileName(d).Split('-')[0], out var v) ? v : null))
			.Where(x => x.V != null)
			.OrderBy(x => x.V)
			.ToList();
		var pick = versions.LastOrDefault(x => x.V!.Major == version.Major && x.V.Minor == version.Minor).Dir
			?? versions.LastOrDefault(x => x.V!.Major <= version.Major).Dir;
		if (pick == null)
			return null;
		var refRoot = Path.Combine(pick, "ref");
		return Directory.Exists(refRoot) ? Directory.EnumerateDirectories(refRoot).FirstOrDefault() : null;
	}

	// Reference assemblies for .NET Framework targets; the newest pack wins, and its
	// Facades subdirectory carries the type-forwarding shims netstandard code needs.
	// The single .NET Framework reference-assembly pack to bind against: the smallest one that
	// is still a superset of the requested version, or the newest installed when no version is
	// given (.NETStandard, which wants the widest set of facades). Indexing every installed pack
	// instead would let the oldest one win by name and hide the newer BCL from every assembly.
	static IEnumerable<string> EnumerateFrameworkRefPacks(string nugetRoot, Version? requested)
	{
		if (!Directory.Exists(nugetRoot))
			yield break;
		var packs = Directory.EnumerateDirectories(nugetRoot, "microsoft.netframework.referenceassemblies.net4*")
			.Select(d => (Dir: d, V: PackVersion(Path.GetFileName(d))))
			.Where(x => x.V != null)
			.OrderBy(x => x.V)
			.ToList();
		var pick = (requested != null ? packs.FirstOrDefault(x => x.V >= requested).Dir : null)
			?? packs.LastOrDefault().Dir;
		if (pick == null)
			yield break;
		foreach (var dir in Directory.EnumerateDirectories(pick, "v*", SearchOption.AllDirectories))
		{
			yield return dir;
			var facades = Path.Combine(dir, "Facades");
			if (Directory.Exists(facades))
				yield return facades;
		}
	}

	// "microsoft.netframework.referenceassemblies.net472" -> 4.7.2
	static Version? PackVersion(string packageId)
	{
		var tfm = packageId[(packageId.LastIndexOf('.') + 1)..];
		return tfm.Length > 3 && tfm.StartsWith("net", StringComparison.Ordinal)
			&& Version.TryParse(string.Join('.', tfm[3..].ToCharArray()), out var v) ? v : null;
	}

	static void IndexDirectory(string dir, Dictionary<string, string> index)
	{
		foreach (var dll in Directory.EnumerateFiles(dir, "*.dll", SearchOption.AllDirectories))
		{
			var name = Path.GetFileNameWithoutExtension(dll);
			// First indexed wins: --refs directories are added before the corpus, so an
			// explicitly supplied reference is never shadowed by a corpus copy.
			if (!index.ContainsKey(name))
				index[name] = dll;
		}
	}

	public string? Find(string simpleName, Dictionary<string, string> frameworkIndex)
	{
		// The target framework's own reference assemblies outrank everything else: a corpus
		// neighbour or a stray NuGet copy of System.Runtime would otherwise decide the BCL.
		if (frameworkIndex.TryGetValue(simpleName, out var fxHit))
			return fxHit;
		if (byName.TryGetValue(simpleName, out var hit))
			return hit;
		foreach (var root in probeRoots)
		{
			// NuGet lays packages out as <root>/<id lowercased>/<version>/lib/<tfm>/<id>.dll,
			// and the assembly name matches the package id often enough to be worth a look.
			var pkgDir = Path.Combine(root, simpleName.ToLowerInvariant());
			if (!Directory.Exists(pkgDir))
				continue;
			var candidate = Directory.EnumerateDirectories(pkgDir)
				.OrderByDescending(d => Path.GetFileName(d), StringComparer.OrdinalIgnoreCase)
				.SelectMany(v => Directory.EnumerateFiles(v, simpleName + ".dll", SearchOption.AllDirectories))
				.FirstOrDefault(f => f.Contains($"{Path.DirectorySeparatorChar}lib{Path.DirectorySeparatorChar}")
					|| f.Contains($"{Path.DirectorySeparatorChar}ref{Path.DirectorySeparatorChar}"));
			if (candidate != null)
			{
				byName[simpleName] = candidate;
				return candidate;
			}
		}
		byName[simpleName] = null!;   // negative cache: probing the filesystem twice buys nothing
		return null;
	}

	// Builds the staging directory for one assembly and returns the path to decompile
	// plus the reference names nothing could supply.
	public static (string Staged, List<string> Missing) Stage(string dll, string stageRoot, RefIndex refs)
	{
		var frameworkIndex = refs.IndexFor(TargetFrameworkOf(dll));
		var dir = Path.Combine(stageRoot, StageName(dll));
		Directory.CreateDirectory(dir);
		// Whatever sat next to the assembly keeps sitting next to it, so staging never
		// resolves LESS than decompiling in place would.
		foreach (var sibling in Directory.EnumerateFiles(Path.GetDirectoryName(Path.GetFullPath(dll))!, "*.dll"))
			Link(sibling, dir);
		Link(dll, dir);
		var missing = new List<string>();
		var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		var queue = new Queue<string>();
		foreach (var name in ReferencesOf(dll))
			queue.Enqueue(name);
		while (queue.Count > 0)
		{
			var name = queue.Dequeue();
			if (!seen.Add(name))
				continue;
			var staged = Path.Combine(dir, name + ".dll");
			if (!File.Exists(staged))
			{
				var found = refs.Find(name, frameworkIndex);
				if (found == null)
				{
					missing.Add(name);
					continue;
				}
				Link(found, dir);
			}
			// A staged reference brings its own references along: the type system follows
			// base types and type-forwards across the whole closure, not just one hop.
			foreach (var transitive in ReferencesOf(staged))
				queue.Enqueue(transitive);
		}
		missing.Sort(StringComparer.OrdinalIgnoreCase);
		return (Path.Combine(dir, Path.GetFileName(dll)), missing);
	}

	// Distinct per source path: two packages can ship the same assembly name with
	// different contents, and they must not share a staging directory.
	static string StageName(string dll)
	{
		var full = Path.GetFullPath(dll);
		var hash = Convert.ToHexString(System.Security.Cryptography.MD5.HashData(Encoding.UTF8.GetBytes(full)))[..8];
		return $"{Path.GetFileNameWithoutExtension(full)}-{hash}";
	}

	static void Link(string source, string targetDir)
	{
		var link = Path.Combine(targetDir, Path.GetFileName(source));
		if (File.Exists(link))
			return;
		try
		{
			File.CreateSymbolicLink(link, Path.GetFullPath(source));
		}
		catch (Exception e) when (e is IOException or UnauthorizedAccessException)
		{
			// Windows only hands out symlink privileges to elevated processes or with
			// Developer Mode enabled; copying costs disk but keeps staging working.
			File.Copy(source, link, overwrite: true);
		}
	}

	// The assembly's TargetFrameworkAttribute value (e.g. ".NETCoreApp,Version=v9.0"), or null
	// when it carries none - which is normal for the .NET Framework era and for ref assemblies.
	public static string? TargetFrameworkOf(string dll)
	{
		try
		{
			using var stream = File.OpenRead(dll);
			using var pe = new PEReader(stream);
			if (!pe.HasMetadata)
				return null;
			var md = pe.GetMetadataReader();
			foreach (var handle in md.GetAssemblyDefinition().GetCustomAttributes())
			{
				var attr = md.GetCustomAttribute(handle);
				if (attr.Constructor.Kind != HandleKind.MemberReference)
					continue;
				var ctor = md.GetMemberReference((MemberReferenceHandle)attr.Constructor);
				if (ctor.Parent.Kind != HandleKind.TypeReference)
					continue;
				var type = md.GetTypeReference((TypeReferenceHandle)ctor.Parent);
				if (md.GetString(type.Name) != "TargetFrameworkAttribute")
					continue;
				// blob: prolog (0x0001), then a SerString holding the moniker.
				var reader = md.GetBlobReader(attr.Value);
				if (reader.ReadUInt16() != 1)
					return null;
				return reader.ReadSerializedString();
			}
		}
		catch (Exception ex) when (ex is BadImageFormatException or IOException)
		{
			// Native or corrupt file: nothing to read.
		}
		return null;
	}

	public static List<string> ReferencesOf(string dll)
	{
		var names = new List<string>();
		try
		{
			using var stream = File.OpenRead(dll);
			using var pe = new PEReader(stream);
			if (!pe.HasMetadata)
				return names;
			var md = pe.GetMetadataReader();
			foreach (var handle in md.AssemblyReferences)
				names.Add(md.GetString(md.GetAssemblyReference(handle).Name));
		}
		catch (Exception ex) when (ex is BadImageFormatException or IOException)
		{
			// Native or corrupt file: it carries no managed references to follow.
		}
		return names;
	}
}

// Self-contained HTML view of a run: the same numbers summary.md carries, plus the
// actual diff of every changed type inline, so a corpus sweep can be surveyed in a
// browser instead of by shelling out to `git diff --no-index` per type. No assets,
// no scripts from anywhere - the file opens straight off disk.
static class Report
{
	public static void WriteHtml(string path, ReportModel m)
	{
		var html = new StringBuilder();
		html.AppendLine("""
			<!doctype html><html><head><meta charset="utf-8">
			<title>decompdiff report</title>
			<style>
			:root { color-scheme: light dark; --bg:#fff; --fg:#1a1a1a; --muted:#666; --line:#d8d8d8;
			        --add:#e6ffec; --addfg:#0a5c1e; --del:#ffebe9; --delfg:#8a1c11; --chip:#f0f0f0; }
			@media (prefers-color-scheme: dark) {
			  :root { --bg:#16181c; --fg:#e6e6e6; --muted:#9aa0a6; --line:#333;
			          --add:#12261a; --addfg:#7ee2a8; --del:#2b1416; --delfg:#ff9c92; --chip:#24262b; }
			}
			body { background:var(--bg); color:var(--fg); font:14px/1.5 system-ui,sans-serif; margin:0 auto; padding:24px; max-width:1100px; }
			h1 { font-size:20px; margin:0 0 4px; } h2 { font-size:16px; margin:28px 0 8px; }
			.meta { color:var(--muted); font-size:13px; }
			table { border-collapse:collapse; margin:12px 0; } th,td { border:1px solid var(--line); padding:4px 10px; text-align:right; }
			th:first-child,td:first-child { text-align:left; }
			.pos { color:var(--delfg); } .neg { color:var(--addfg); }
			details { border:1px solid var(--line); border-radius:6px; margin:6px 0; background:var(--chip); }
			summary { cursor:pointer; padding:8px 10px; font-family:ui-monospace,monospace; font-size:13px; }
			pre { margin:0; padding:10px; overflow-x:auto; background:var(--bg); font:12px/1.45 ui-monospace,monospace; }
			ins { background:var(--add); color:var(--addfg); text-decoration:none; display:block; }
			del { background:var(--del); color:var(--delfg); text-decoration:none; display:block; }
			span.ctx { display:block; color:var(--muted); }
			#filter { width:100%; padding:8px; margin:8px 0; border:1px solid var(--line); border-radius:6px;
			          background:var(--bg); color:var(--fg); font:13px ui-monospace,monospace; }
			ul { padding-left:20px; } li { font-family:ui-monospace,monospace; font-size:12.5px; }
			</style></head><body>
			""");
		html.AppendLine("<h1>decompdiff report</h1>");
		html.AppendLine($"<div class=meta>old: {Esc(m.Old)}<br>new: {Esc(m.New)}<br>"
			+ $"corpus: {m.Assemblies} assemblies, {m.Unchanged + m.Changed.Count} types compared</div>");

		html.AppendLine("<table><tr><th>metric</th><th>old</th><th>new</th><th>delta</th></tr>");
		html.AppendLine(Row("lines", m.OldTotals.Lines, m.NewTotals.Lines));
		html.AppendLine(Row("goto statements", m.OldTotals.Gotos, m.NewTotals.Gotos));
		html.AppendLine(Row("//IL_ warning comments", m.OldTotals.IlWarnings, m.NewTotals.IlWarnings));
		html.AppendLine(Row("compiler-generated name leaks", m.OldTotals.GeneratedNames, m.NewTotals.GeneratedNames));
		html.AppendLine("</table>");
		html.AppendLine($"<div class=meta>{m.Unchanged} unchanged, {m.Changed.Count} changed, "
			+ $"<b>{m.NewErrors} NEW errors</b>, {m.FixedErrors} fixed errors, {m.BothErrors} errored in both</div>");

		if (m.Changed.Count > 0)
		{
			html.AppendLine($"<h2>Changed types ({m.Changed.Count})</h2>");
			html.AppendLine("<input id=filter placeholder='filter by type or assembly name'>");
			foreach (var c in m.Changed.OrderByDescending(c => Math.Abs(c.New.Lines - c.Old.Lines)))
			{
				var file = SanitizeFileName(c.Location.Replace(" / ", "/")) + ".cs";
				var oldCode = ReadIfExists(Path.Combine(m.ReportDir, "old", file));
				var newCode = ReadIfExists(Path.Combine(m.ReportDir, "new", file));
				var delta = c.New.Lines - c.Old.Lines;
				html.AppendLine($"<details><summary>{Esc(c.Location)} "
					+ $"<span class=meta>({c.Old.Lines} &rarr; {c.New.Lines} lines, {delta:+#;-#;0})</span></summary>");
				html.AppendLine($"<pre>{Diff(oldCode, newCode)}</pre></details>");
			}
			html.AppendLine("""
				<script>
				const box = document.getElementById('filter');
				box.addEventListener('input', () => {
					const needle = box.value.toLowerCase();
					for (const d of document.querySelectorAll('details'))
						d.style.display = d.querySelector('summary').textContent.toLowerCase().includes(needle) ? '' : 'none';
				});
				</script>
				""");
		}
		AppendList(html, "Error / presence transitions", m.Transitions);
		AppendList(html, "Skipped assemblies (undecompilable on both sides)", m.Skipped);
		AppendList(html, "Unresolved references (pass --refs to cover these)",
			m.UnresolvedRefs.Select(kv => $"{kv.Key}: {string.Join(", ", kv.Value)}").ToList());
		html.AppendLine("</body></html>");
		File.WriteAllText(path, html.ToString());
	}

	static void AppendList(StringBuilder html, string title, List<string> items)
	{
		if (items.Count == 0)
			return;
		html.AppendLine($"<h2>{Esc(title)} ({items.Count})</h2><ul>");
		foreach (var item in items)
			html.AppendLine($"<li>{Esc(item)}</li>");
		html.AppendLine("</ul>");
	}

	static string Row(string name, long o, long n)
	{
		var delta = n - o;
		var cls = delta == 0 ? "" : delta > 0 ? " class=pos" : " class=neg";
		return $"<tr><td>{Esc(name)}</td><td>{o}</td><td>{n}</td><td{cls}>{delta:+#;-#;0}</td></tr>";
	}

	static string ReadIfExists(string path) => File.Exists(path) ? File.ReadAllText(path) : "";

	// Line diff: common prefix/suffix are cheap to strip and usually account for nearly
	// everything, leaving a middle small enough for an O(n*m) LCS. Beyond the cap the
	// middle is shown as a plain replacement rather than spending minutes on alignment.
	const int LcsCap = 1500;

	static string Diff(string oldCode, string newCode)
	{
		var a = oldCode.ReplaceLineEndings("\n").Split('\n');
		var b = newCode.ReplaceLineEndings("\n").Split('\n');
		int start = 0;
		while (start < a.Length && start < b.Length && a[start] == b[start])
			start++;
		int endA = a.Length, endB = b.Length;
		while (endA > start && endB > start && a[endA - 1] == b[endB - 1])
		{
			endA--;
			endB--;
		}
		var sb = new StringBuilder();
		// A few lines of context on each side make the hunk readable on its own.
		for (int i = Math.Max(0, start - 3); i < start; i++)
			sb.Append("<span class=ctx>").Append(Esc(a[i])).Append("</span>");
		int lenA = endA - start, lenB = endB - start;
		if (lenA <= LcsCap && lenB <= LcsCap)
		{
			foreach (var (tag, line) in LcsDiff(a[start..endA], b[start..endB]))
				sb.Append(tag switch { '+' => "<ins>", '-' => "<del>", _ => "<span class=ctx>" })
					.Append(Esc(line))
					.Append(tag switch { '+' => "</ins>", '-' => "</del>", _ => "</span>" });
		}
		else
		{
			for (int i = start; i < endA; i++)
				sb.Append("<del>").Append(Esc(a[i])).Append("</del>");
			for (int i = start; i < endB; i++)
				sb.Append("<ins>").Append(Esc(b[i])).Append("</ins>");
		}
		for (int i = endA; i < Math.Min(a.Length, endA + 3); i++)
			sb.Append("<span class=ctx>").Append(Esc(a[i])).Append("</span>");
		return sb.ToString();
	}

	static List<(char Tag, string Line)> LcsDiff(string[] a, string[] b)
	{
		var lcs = new int[a.Length + 1, b.Length + 1];
		for (int i = a.Length - 1; i >= 0; i--)
			for (int j = b.Length - 1; j >= 0; j--)
				lcs[i, j] = a[i] == b[j] ? lcs[i + 1, j + 1] + 1 : Math.Max(lcs[i + 1, j], lcs[i, j + 1]);
		var result = new List<(char, string)>();
		int x = 0, y = 0;
		while (x < a.Length && y < b.Length)
		{
			if (a[x] == b[y])
			{
				result.Add((' ', a[x]));
				x++;
				y++;
			}
			else if (lcs[x + 1, y] >= lcs[x, y + 1])
			{
				result.Add(('-', a[x++]));
			}
			else
			{
				result.Add(('+', b[y++]));
			}
		}
		while (x < a.Length)
			result.Add(('-', a[x++]));
		while (y < b.Length)
			result.Add(('+', b[y++]));
		return result;
	}

	static string Esc(string s) => s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

	// '/' survives as a directory separator; everything the platform rejects becomes '_'.
	// Long segments are truncated with a hash of the original appended, because a
	// namespace-qualified generic type name can push a report path past the 260-character
	// limit Windows applies unless long paths are enabled machine-wide.
	public static string SanitizeFileName(string s)
		=> string.Join('/', s.Split('/').Select(segment => {
			var clean = string.Concat(segment.Select(
				ch => Path.GetInvalidFileNameChars().Contains(ch) ? '_' : ch));
			return clean.Length <= 80
				? clean
				: clean[..72] + Convert.ToHexString(
					System.Security.Cryptography.MD5.HashData(Encoding.UTF8.GetBytes(clean)))[..8];
		}));
}

record ReportModel(
	string Old, string New, int Assemblies, int Unchanged, List<ChangedType> Changed,
	List<string> Transitions, List<string> Skipped, SortedDictionary<string, List<string>> UnresolvedRefs,
	Metrics OldTotals, Metrics NewTotals, int NewErrors, int FixedErrors, int BothErrors, string ReportDir);

record TypeResult(string? Code, string? Error, Metrics Metrics);

record ChangedType(string Location, Metrics Old, Metrics New);

record struct Metrics(int Lines, int Gotos, int IlWarnings, int GeneratedNames)
{
	public static Metrics Measure(string code) => new(
		code.Count(c => c == '\n') + 1,
		Regex.Matches(code, @"\bgoto ").Count,
		Regex.Matches(code, @"//IL_[0-9a-fA-F]+:").Count,
		Regex.Matches(code, @"<>").Count);

	public static Metrics operator +(Metrics a, Metrics b)
		=> new(a.Lines + b.Lines, a.Gotos + b.Gotos, a.IlWarnings + b.IlWarnings, a.GeneratedNames + b.GeneratedNames);
}

// One decompiler version: locates/builds ICSharpCode.Decompiler.dll, loads it in
// its own AssemblyLoadContext, and drives it via `dynamic` through the stable
// CSharpDecompiler(string, DecompilerSettings) API so any two versions work.
class Side
{
	readonly Assembly assembly;
	public string Description { get; }
	public string? LastAssemblyError { get; private set; }

	Side(Assembly assembly, string description)
	{
		this.assembly = assembly;
		Description = description;
	}

	public static Side Create(string name, string spec, bool forceBuild)
	{
		string dllPath;
		string description;
		if (File.Exists(spec))
		{
			dllPath = Path.GetFullPath(spec);
			description = dllPath;
		}
		else if (Directory.Exists(spec))
		{
			var checkout = Path.GetFullPath(spec);
			dllPath = BuildCheckout(checkout, forceBuild);
			// The dll timestamp exposes stale pre-existing builds; --build forces a fresh one.
			description = $"{checkout} ({GitDescribe(checkout)}, dll of {File.GetLastWriteTime(dllPath):yyyy-MM-dd HH:mm})";
		}
		else if (TryResolveCommit(spec, out var commit, out var repoRoot))
		{
			// A git ref, so a PR or a tag can be diffed without preparing checkouts by hand.
			// The worktree is keyed by commit and kept: reusing it reuses the Release build
			// already sitting in its bin/, which is what dominates the runtime of a rerun.
			var checkout = EnsureWorktree(repoRoot, commit);
			dllPath = BuildCheckout(checkout, forceBuild);
			description = $"{spec} ({commit[..9]}, dll of {File.GetLastWriteTime(dllPath):yyyy-MM-dd HH:mm})";
		}
		else
		{
			throw new ArgumentException(
				$"--{name} {spec}: not a file, a directory, or a commit-ish in the repository at {Environment.CurrentDirectory}");
		}
		var alc = new DecompilerLoadContext(name, dllPath);
		return new Side(alc.LoadFromAssemblyPath(dllPath), description);
	}

	static string BuildCheckout(string checkout, bool forceBuild)
	{
		var csproj = Path.Combine(checkout, "ICSharpCode.Decompiler", "ICSharpCode.Decompiler.csproj");
		if (!File.Exists(csproj))
			throw new ArgumentException($"{checkout}: no ICSharpCode.Decompiler/ICSharpCode.Decompiler.csproj");
		var binDir = Path.Combine(checkout, "ICSharpCode.Decompiler", "bin", "Release");
		var existing = Directory.Exists(binDir)
			? Directory.EnumerateFiles(binDir, "ICSharpCode.Decompiler.dll", SearchOption.AllDirectories)
				.OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault()
			: null;
		if (existing != null && !forceBuild)
			return existing;
		// A bare restore would prune the repo's packages.lock.json files; keep them whole.
		Run("dotnet", $"restore \"{csproj}\" -p:RestoreEnablePackagePruning=false");
		Run("dotnet", $"build \"{csproj}\" -c Release --no-restore");
		return Directory.EnumerateFiles(binDir, "ICSharpCode.Decompiler.dll", SearchOption.AllDirectories)
			.OrderByDescending(File.GetLastWriteTimeUtc).First();
	}

	static void Run(string exe, string arguments)
	{
		Console.WriteLine($"  $ {exe} {arguments}");
		var psi = new ProcessStartInfo(exe, arguments) {
			RedirectStandardOutput = true,
			RedirectStandardError = true,
		};
		using var p = Process.Start(psi)!;
		var output = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
		p.WaitForExit();
		if (p.ExitCode != 0)
			throw new InvalidOperationException($"{exe} {arguments} failed:\n{output}");
	}

	// Resolves a commit-ish against the repository the tool is run from. Files and directories
	// win over refs, so a branch sharing a name with a directory still needs the path spelled out.
	static bool TryResolveCommit(string spec, out string commit, out string repoRoot)
	{
		commit = "";
		repoRoot = "";
		var root = Git(Environment.CurrentDirectory, "rev-parse", "--show-toplevel");
		if (root == null)
			return false;
		var resolved = Git(root, "rev-parse", "--verify", "--quiet", spec + "^{commit}");
		if (string.IsNullOrEmpty(resolved))
			return false;
		commit = resolved;
		repoRoot = root;
		return true;
	}

	// Worktrees live outside the repository, so they never show up in its status or get
	// swept up by a clean; `git worktree list` still shows them, and they are safe to delete.
	static string EnsureWorktree(string repoRoot, string commit)
	{
		var dir = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
			".cache", "decompdiff", Path.GetFileName(repoRoot), commit);
		if (Directory.Exists(dir))
			return dir;
		Directory.CreateDirectory(Path.GetDirectoryName(dir)!);
		Console.WriteLine($"  $ git worktree add --detach {dir} {commit[..9]}");
		if (Git(repoRoot, "worktree", "add", "--detach", dir, commit) == null)
			throw new InvalidOperationException($"git worktree add failed for {commit}");
		return dir;
	}

	// Runs git and returns its trimmed stdout, or null if it could not be run or failed.
	static string? Git(string workingDirectory, params string[] arguments)
	{
		try
		{
			var psi = new ProcessStartInfo("git") {
				WorkingDirectory = workingDirectory,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
			};
			foreach (var argument in arguments)
				psi.ArgumentList.Add(argument);
			using var p = Process.Start(psi)!;
			var output = p.StandardOutput.ReadToEnd().Trim();
			p.WaitForExit();
			return p.ExitCode == 0 ? output : null;
		}
		catch
		{
			return null;
		}
	}

	static string GitDescribe(string checkout)
	{
		var output = Git(checkout, "describe", "--always", "--dirty", "--exclude", "*");
		return string.IsNullOrEmpty(output) ? "unknown" : output;
	}

	// Decompiles every top-level type; null when the assembly itself cannot be
	// opened (not managed, type system init failed) - see LastAssemblyError.
	public Dictionary<string, TypeResult>? DecompileAssembly(string dllPath)
	{
		LastAssemblyError = null;
		dynamic decompiler;
		try
		{
			var settings = Activator.CreateInstance(assembly.GetType("ICSharpCode.Decompiler.DecompilerSettings", throwOnError: true)!)!;
			decompiler = Activator.CreateInstance(
				assembly.GetType("ICSharpCode.Decompiler.CSharp.CSharpDecompiler", throwOnError: true)!,
				dllPath, settings)!;
		}
		catch (Exception ex)
		{
			LastAssemblyError = Unwrap(ex).Message;
			return null;
		}
		var results = new Dictionary<string, TypeResult>();
		foreach (object type in (IEnumerable)decompiler.TypeSystem.MainModule.TopLevelTypeDefinitions)
		{
			// The type definition's runtime type is internal, so dynamic cannot bind
			// its members; go through the public interface property via reflection.
			object fullTypeName = GetProperty(type, "FullTypeName");
			string name = fullTypeName.ToString()!;
			using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
			try
			{
				decompiler.CancellationToken = cts.Token;
				string code = decompiler.DecompileTypeAsString((dynamic)fullTypeName);
				results[name] = new TypeResult(code, null, Metrics.Measure(code));
			}
			catch (Exception ex)
			{
				var inner = Unwrap(ex);
				var error = inner is OperationCanceledException
					? "timeout (60s)"
					: $"{inner.GetType().Name}: {FirstLine(inner.Message)}";
				results[name] = new TypeResult(null, error, default);
			}
		}
		return results;
	}

	static object GetProperty(object obj, string name)
	{
		var type = obj.GetType();
		var property = type.GetProperty(name)
			?? type.GetInterfaces().Select(i => i.GetProperty(name)).FirstOrDefault(p => p != null)
			?? throw new MissingMemberException(type.FullName, name);
		return property.GetValue(obj)!;
	}

	static Exception Unwrap(Exception ex)
	{
		while (ex is TargetInvocationException { InnerException: not null } tie)
			ex = tie.InnerException!;
		return ex;
	}

	static string FirstLine(string s)
	{
		var i = s.IndexOfAny(['\r', '\n']);
		return i < 0 ? s : s[..i];
	}
}

// Resolves the decompiler's own dependencies from its build-output directory,
// falling back to the default context for framework assemblies. Each Side gets
// its own context so two ICSharpCode.Decompiler versions can coexist.
class DecompilerLoadContext(string name, string mainDllPath) : AssemblyLoadContext(name)
{
	readonly string dir = Path.GetDirectoryName(Path.GetFullPath(mainDllPath))!;

	protected override Assembly? Load(AssemblyName assemblyName)
	{
		var candidate = Path.Combine(dir, assemblyName.Name + ".dll");
		return File.Exists(candidate) ? LoadFromAssemblyPath(candidate) : null;
	}
}

class AssertionFailedException(string message) : Exception(message);

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
