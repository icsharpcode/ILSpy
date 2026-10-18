// Copyright (c) 2026 Christoph Wille
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

// IgToContainerProfile: turns Inspektor Gadget recordings of one container into a Kubescape
// ContainerProfile, the document format of a Software Bill of Behavior (SBOB,
// https://billofbehavior.com/bob/docs/spec/). Kubescape's node-agent learns profiles from the
// same gadgets inside Kubernetes; this tool does the equivalent for a container recorded with
// the standalone `ig` CLI, so no cluster is needed.
//
// usage: dotnet run IgToContainerProfile.cs -- --in <dir> --out <file.yaml> --name <name>
//            --namespace <ns> --image-id <id> --image-tag <tag> [--anchor <prefix>]...
//
// <dir> holds, per gadget, the JSON stream `ig run <gadget> -o json` wrote (one event per line):
//   trace_exec.json (recorded with --paths), trace_open.json (--paths), trace_tcp.json,
//   trace_dns.json, trace_capabilities.json, and advise_seccomp.txt (the gadget's default
//   output: a `// <container>` comment followed by a seccomp profile in JSON).
//
// Generalization applied here mirrors scripts/sbob-from-learned.py and portable-sbob.py in
// https://github.com/k8sstormcenter/bob:
//   - exec args collapse to [argv0, "..."] (zero-or-more wildcard): a CLI's argv is user input.
//   - segments that rotate between runs (pids, tids, the shared-framework patch version, hash
//     names) become the one-segment wildcard; a directory with 50 or more distinct children
//     collapses to one wildcard entry, as Kubescape's storage does on save.
//   - every open under an --anchor prefix collapses to <prefix>/* with the union of the flags,
//     for directories whose contents the user chooses (input, output, temp).
//   - external TCP peers are keyed by the DNS name that resolved to them; the DNS server itself
//     becomes the kube-dns selector every Kubescape-learned profile carries, since a resolver
//     address is only meaningful on the machine it was recorded on. Loopback peers are dropped.
//   - events of the container runtime (crun/conmon) are not the workload's; Kubescape handles
//     the runtime's init phase through rulePolicies, which the output carries as well.

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

const string OneSegment = "⋯";       // exactly one path segment / one argument
const string ZeroOrMoreArgs = "⋯⋯";
string[] runtimeComms = ["crun", "runc", "conmon"];

string inDir = "", outFile = "", name = "", ns = "", imageId = "", imageTag = "";
var anchors = new List<string>();
for (int i = 0; i < args.Length; i++)
{
	switch (args[i])
	{
		case "--in": inDir = args[++i]; break;
		case "--out": outFile = args[++i]; break;
		case "--name": name = args[++i]; break;
		case "--namespace": ns = args[++i]; break;
		case "--image-id": imageId = args[++i]; break;
		case "--image-tag": imageTag = args[++i]; break;
		case "--anchor": anchors.Add(args[++i].TrimEnd('/')); break;
		default: Console.Error.WriteLine($"unknown argument {args[i]}"); return 64;
	}
}
if (inDir == "" || outFile == "" || name == "" || ns == "")
{
	Console.Error.WriteLine("usage: --in <dir> --out <file> --name <name> --namespace <ns> [--image-id <id>] [--image-tag <tag>] [--anchor <prefix>]...");
	return 64;
}

IEnumerable<JsonElement> Events(string gadget)
{
	string path = Path.Combine(inDir, gadget + ".json");
	if (!File.Exists(path))
	{
		Console.Error.WriteLine($"warning: {path} missing, section left empty");
		yield break;
	}
	foreach (string line in File.ReadLines(path))
	{
		if (line.Length == 0 || line[0] != '{')
			continue;
		var e = JsonDocument.Parse(line).RootElement;
		string comm = e.GetProperty("proc").GetProperty("comm").GetString() ?? "";
		if (!runtimeComms.Contains(comm))
			yield return e;
	}
}

// execs
var execs = new SortedDictionary<string, string>(StringComparer.Ordinal);
foreach (var e in Events("trace_exec"))
{
	string exe = e.GetProperty("exepath").GetString() ?? "";
	if (exe == "")
		continue;
	// ig joins argv with U+00A0, which cannot occur inside a Linux argument.
	string argv0 = (e.GetProperty("args").GetString() ?? "").Split(' ')[0];
	execs.TryAdd(exe, argv0 == "" ? exe : argv0);
}

// opens
var opens = new SortedDictionary<string, SortedSet<string>>(StringComparer.Ordinal);
// Segments that differ on every run: process and thread ids, the shared-framework patch
// version, CoreCLR's named semaphores, and hash-named cache entries.
(Regex pattern, string replacement)[] rotating =
[
	(new Regex(@"^/proc/\d+(/|$)"), "/proc/" + OneSegment + "$1"),
	(new Regex(@"^(/proc/" + OneSegment + @"/task/)\d+(/|$)"), "$1" + OneSegment + "$2"),
	(new Regex(@"^(/usr/share/dotnet/(?:shared|host)/[^/]+/)\d+\.\d+\.\d+(/|$)"), "$1" + OneSegment + "$2"),
	(new Regex(@"^/dev/shm/[^/]+$"), "/dev/shm/" + OneSegment),
	(new Regex(@"/[0-9a-f]{32,}[^/]*(?=/|$)"), "/" + OneSegment),
];
int unresolved = 0;
foreach (var e in Events("trace_open"))
{
	// A failed open has no resolved path; ig then reports a meaningless fpath, and the
	// name the process asked for is the behavior to record (the runtime probes several
	// locations for optional libraries on every start).
	string path = (e.GetProperty("error").GetString() ?? "") == "" ? e.GetProperty("fpath").GetString() ?? "" : "";
	if (path == "")
		path = e.GetProperty("fname").GetString() ?? "";
	if (!path.StartsWith('/'))
	{
		unresolved++;
		continue;
	}
	path = path.Replace("//", "/");
	foreach (var (pattern, replacement) in rotating)
		path = pattern.Replace(path, replacement);
	string? anchor = anchors.FirstOrDefault(a => path.StartsWith(a + "/", StringComparison.Ordinal));
	if (anchor != null)
		path = anchor + "/*";
	var flags = (e.GetProperty("flags").GetString() ?? "").Split('|', StringSplitOptions.RemoveEmptyEntries);
	if (!opens.TryGetValue(path, out var set))
		opens[path] = set = new SortedSet<string>(StringComparer.Ordinal);
	set.UnionWith(flags);
}
if (unresolved > 0)
	Console.Error.WriteLine($"warning: {unresolved} open events had no absolute path (record with --paths) and were skipped");
// A directory with many distinct children (a CA bundle, a framework's assemblies) collapses
// to one wildcard entry, as Kubescape's storage does when it saves a learned profile.
const int CollapseThreshold = 50;
foreach (var dir in opens.Keys.Select(p => p[..p.LastIndexOf('/')]).Where(d => d != "").Distinct().ToList())
{
	var children = opens.Keys.Where(p => p.LastIndexOf('/') == dir.Length && p.StartsWith(dir + "/", StringComparison.Ordinal)).ToList();
	if (children.Count < CollapseThreshold)
		continue;
	var flags = new SortedSet<string>(children.SelectMany(c => opens[c]), StringComparer.Ordinal);
	foreach (string c in children)
		opens.Remove(c);
	opens[dir + "/" + OneSegment] = flags;
}

// capabilities
// Only audited checks are capability use. The kernel also probes capabilities opportunistically
// (CAP_SYS_ADMIN on every execve, for instance) and reports those with audit=0; a container
// running without the capability still produces them.
var caps = new SortedSet<string>(StringComparer.Ordinal);
foreach (var e in Events("trace_capabilities"))
	if (e.GetProperty("audit").GetInt32() == 1)
		caps.Add(e.GetProperty("cap").GetString() ?? "");
caps.Remove("");

// syscalls: advise_seccomp prints "// <container>" then a seccomp profile, once per container.
var syscalls = new SortedSet<string>(StringComparer.Ordinal);
string seccompFile = Path.Combine(inDir, "advise_seccomp.txt");
if (File.Exists(seccompFile))
{
	foreach (string block in Regex.Split(File.ReadAllText(seccompFile), @"^//.*$", RegexOptions.Multiline))
	{
		if (block.Trim().Length == 0)
			continue;
		foreach (var rule in JsonDocument.Parse(block).RootElement.GetProperty("syscalls").EnumerateArray())
			foreach (var n in rule.GetProperty("names").EnumerateArray())
				syscalls.Add(n.GetString() ?? "");
	}
	syscalls.Remove("");
}
else
	Console.Error.WriteLine($"warning: {seccompFile} missing, syscalls left empty");

// egress: DNS answers map addresses back to the names that were asked for.
var addrToName = new Dictionary<string, string>();
var dnsServers = new SortedSet<string>(StringComparer.Ordinal);  // "addr proto port"
foreach (var e in Events("trace_dns"))
{
	string qname = (e.GetProperty("name").GetString() ?? "").TrimEnd('.');
	if (e.GetProperty("qr").GetString() == "Q")
	{
		var dst = e.GetProperty("dst");
		dnsServers.Add($"{dst.GetProperty("addr").GetString()} {dst.GetProperty("proto").GetString()} {dst.GetProperty("port").GetInt32()}");
	}
	foreach (string a in (e.GetProperty("addresses").GetString() ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries))
		addrToName.TryAdd(a, qname);
}
// peer key: ("dns:<name>" | "ip:<addr>") -> set of "PROTO port"
var peers = new SortedDictionary<string, SortedSet<string>>(StringComparer.Ordinal);
foreach (var e in Events("trace_tcp"))
{
	if (e.GetProperty("type").GetString() != "connect")
		continue;
	var dst = e.GetProperty("dst");
	string addr = dst.GetProperty("addr").GetString() ?? "";
	if (addr.StartsWith("127.") || addr == "::1")
		continue;
	string key = addrToName.TryGetValue(addr, out var n) ? "dns:" + n : "ip:" + addr;
	if (!peers.TryGetValue(key, out var ports))
		peers[key] = ports = new SortedSet<string>(StringComparer.Ordinal);
	ports.Add($"{dst.GetProperty("proto").GetString()} {dst.GetProperty("port").GetInt32()}");
}

// YAML, written by hand: the document is three levels deep and every scalar is quoted, so a
// serializer would only add a package restore to a tool that runs inside a throwaway container.
var y = new StringBuilder();
string Q(string s) => "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
string Identifier(string s) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(s)));
void Ports(string indent, IEnumerable<string> protoPorts)
{
	y.AppendLine($"{indent}ports:");
	foreach (string pp in protoPorts)
	{
		var parts = pp.Split(' ');
		y.AppendLine($"{indent}- name: {Q(parts[0] + "-" + parts[1])}");
		y.AppendLine($"{indent}  port: {parts[1]}");
		y.AppendLine($"{indent}  protocol: {Q(parts[0])}");
	}
}

y.AppendLine("apiVersion: spdx.softwarecomposition.kubescape.io/v1beta1");
y.AppendLine("kind: ContainerProfile");
y.AppendLine("metadata:");
y.AppendLine($"  name: {Q(name)}");
y.AppendLine($"  namespace: {Q(ns)}");
y.AppendLine("  annotations:");
y.AppendLine("    kubescape.io/managed-by: User");
y.AppendLine("    sbob.io/spec-version: \"0.0.4\"");
y.AppendLine("spec:");
y.AppendLine("  architectures:");
y.AppendLine("  - amd64");
y.AppendLine($"  imageID: {Q(imageId)}");
y.AppendLine($"  imageTag: {Q(imageTag)}");
// An explicit empty list is NONE (any observation is a violation); a bare key would be NULL
// (implementation-defined), which is not what a tool that uses none of them means.
void Scalars(string key, IEnumerable<string> items)
{
	var list = items.ToList();
	y.AppendLine($"  {key}:" + (list.Count == 0 ? " []" : ""));
	foreach (string item in list)
		y.AppendLine($"  - {item}");
}
Scalars("capabilities", caps);
y.AppendLine("  execs:");
foreach (var (path, argv0) in execs)
{
	y.AppendLine($"  - path: {Q(path)}");
	y.AppendLine("    args:");
	y.AppendLine($"    - {Q(argv0)}");
	y.AppendLine($"    - {Q(ZeroOrMoreArgs)}");
}
y.AppendLine("  opens:");
foreach (var (path, flags) in opens)
{
	y.AppendLine($"  - path: {Q(path)}");
	y.AppendLine("    flags:");
	foreach (string f in flags)
		y.AppendLine($"    - {f}");
}
Scalars("syscalls", syscalls);
y.AppendLine("  endpoints: []");
y.AppendLine("  ingress: []");
y.AppendLine(dnsServers.Count == 0 && peers.Count == 0 ? "  egress: []" : "  egress:");
if (dnsServers.Count > 0)
{
	y.AppendLine($"  - identifier: {Q(Identifier("kube-dns"))}");
	y.AppendLine("    type: internal");
	y.AppendLine("    namespaceSelector:");
	y.AppendLine("      matchLabels:");
	y.AppendLine("        kubernetes.io/metadata.name: kube-system");
	y.AppendLine("    podSelector:");
	y.AppendLine("      matchLabels:");
	y.AppendLine("        k8s-app: kube-dns");
	Ports("    ", dnsServers.Select(s => string.Join(' ', s.Split(' ')[1..])).Distinct().Order());
}
foreach (var (key, ports) in peers)
{
	y.AppendLine($"  - identifier: {Q(Identifier(key))}");
	y.AppendLine("    type: external");
	if (key.StartsWith("dns:"))
	{
		y.AppendLine("    dnsNames:");
		y.AppendLine($"    - {Q(key[4..])}");
	}
	else
	{
		y.AppendLine("    ipAddresses:");
		y.AppendLine($"    - {Q(key[3..])}");
	}
	Ports("    ", ports);
}
// The container runtime's init phase touches files and capabilities no workload profile can
// predict; every shipped Kubescape profile exempts it the same way.
y.AppendLine("  rulePolicies:");
foreach (string rule in new[] { "R0002", "R0004" })
{
	y.AppendLine($"    {rule}:");
	y.AppendLine("      processAllowed:");
	foreach (string comm in new[] { "runc:[1:INIT]", "runc:[1:CHILD]", "runc:[2:INIT]", "runc:[3:INIT]" })
		y.AppendLine($"      - {Q(comm)}");
}

File.WriteAllText(outFile, y.ToString(), new UTF8Encoding(false));
Console.Error.WriteLine($"{outFile}: {execs.Count} execs, {opens.Count} opens, {caps.Count} capabilities, {syscalls.Count} syscalls, {peers.Count + (dnsServers.Count > 0 ? 1 : 0)} egress peers");
return 0;
