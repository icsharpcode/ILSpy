# Headless ILSpy MCP Server Implementation Plan

Status: Proposed implementation plan; no code is implemented by this document.

Last verified: 2026-08-20

## 1. Decision Summary

Implement a separate, headless `ICSharpCode.ILSpyMcp` .NET executable that exposes ILSpy's read-only decompilation and inspection capabilities through the official Model Context Protocol C# SDK over stdio. This is feasible without changing the Avalonia UI. The existing `ICSharpCode.Decompiler` and `ICSharpCode.ILSpyX` projects already provide the required engine and AI context functionality; `ICSharpCode.ILSpyCmd` provides proven command-line behavior and tests that identify the remaining seams to extract.

The server must be a normal MCP stdio process. MCP JSON-RPC is the only stdout content. Logs and diagnostics go to stderr. Claude, Codex, and OpenCode can use the server through their stdio MCP adapters. Pi support is feasible, but the implementation must smoke-test the exact Pi version and document its adapter/configuration because Pi integrations vary by release.

Use the `ModelContextProtocol` package (the umbrella package intended for hosted non-HTTP servers), not ASP.NET Core, for the first release. Version `2.2.0` was the current stable package verified on the date above; re-check the package and API at implementation time, pin the selected version centrally, and regenerate all lock files.

## 2. Goal and Non-Goals

### Goal

A user gives an MCP client an allowed path to a managed .NET assembly. The client can inspect it, enumerate types/resources/metadata, decompile a selected entity, obtain method IL, and build bounded AI context. Every operation is deterministic, read-only, cancellable, bounded, and returns structured data suitable for an LLM.

### Non-goals for the first release

- No Avalonia/UI hosting, desktop process automation, or GUI state access.
- No subprocess invocation of `ilspycmd`; call decompiler APIs directly.
- No writes to assemblies, extracted files, projects, or the user's filesystem.
- No network access, package download, symbol-server access, or AI provider credentials.
- No prompts, sampling, elicitation, or model calls in the server. The MCP client owns model access.
- No HTTP/SSE/Streamable HTTP transport in the MVP.
- No static MCP resources or server-side assembly registry. Dynamic, potentially expensive path operations are tools.
- No project export, bundle/package handling, BAML extraction, or write-capable resource extraction in the MVP.
- No unbounded whole-assembly responses, metadata dumps, call graphs, or caches.

## 3. Repository Evidence and Constraints

The following facts drive the design. Keep them as acceptance constraints while implementing:

- Main cross-platform code targets `net10.0`; test projects target `net11.0`.
- `ICSharpCode.Decompiler` is the decompilation engine. `ICSharpCode.ILSpyX` is host-agnostic shared support and contains `AI/ContextBuilder.cs`.
- `ICSharpCode.ILSpyCmd` already references `ILSpyX`, `Decompiler`, BAML, and `Microsoft.Extensions.Hosting`; it is packaged as the `ilspycmd` global tool.
- Central package management is enabled in `Directory.Packages.props`; every package reference needs a central version. All projects use locked restore files.
- Use repository scripts: `pwsh ./restore.ps1`, `pwsh ./build.ps1 --no-restore`, and `pwsh ./updatedeps.ps1` after dependency changes. Tests run with `--report-trx`.
- Add the server and test projects to both `ILSpy.sln` and `ILSpy.XPlat.slnf`.
- New C# files require the repository's complete MIT header and the human contributor's name. Never put an AI name in a source header.
- New behavior follows TDD: create a failing test, implement the smallest change, then show the test green.
- Existing CLI tests capture stdout and stderr using `CliTestRunner`; follow that discipline for protocol contamination tests.

Relevant existing implementation seams:

- `IlspyCmdProgram.GetDecompiler` constructs `PEFile`, `UniversalAssemblyResolver`, `CSharpDecompiler`, and optional PDB loading.
- CLI type/member resolution accepts reflection names, XML documentation IDs, and metadata tokens.
- CLI lists resources and decompiles types/members; `MetadataTableDumper` loads deterministic table rows and has parseable JSON tests.
- `ContextBuilder.Build(IEntity, CSharpDecompiler)` already returns bounded C# context, identity, framework, attributes, interfaces, strings, optional callers/callees, optional method IL, unavailable-section diagnostics, and a token estimate. It validates module ownership and supported metadata handles.

## 4. Architecture

Create two standalone projects:

```text
ICSharpCode.ILSpyMcp/
  ICSharpCode.ILSpyMcp.csproj
  Program.cs
  Configuration/McpServerOptions.cs
  Mcp/ILSpyTools.cs
  Services/AssemblySessionFactory.cs
  Services/AssemblyDecompilerService.cs
  Services/EntityResolver.cs
  Services/OutputLimiter.cs
  Services/PathAccessPolicy.cs
  Services/MetadataTableService.cs
  Contracts/Requests.cs
  Contracts/Responses.cs
  Errors/McpError.cs
ICSharpCode.ILSpyMcp.Tests/
  ICSharpCode.ILSpyMcp.Tests.csproj
  PathAccessPolicyTests.cs
  EntityResolverTests.cs
  OutputLimiterTests.cs
  AssemblyDecompilerServiceTests.cs
  McpToolContractTests.cs
  StdioProtocolTests.cs
  TestAssemblyFixtures.cs
```

The names are guidance, not permission to create unnecessary abstractions. Keep one responsibility per class and use records for transport contracts.

```text
MCP client (Claude/Codex/OpenCode/Pi)
        | JSON-RPC over stdin/stdout
Program -> generic host -> MCP stdio transport
                         -> ILSpyTools (thin adapter)
                             -> PathAccessPolicy
                             -> AssemblyDecompilerService
                                 -> AssemblySessionFactory
                                     -> PEFile + resolver + CSharpDecompiler
                             -> EntityResolver / OutputLimiter
                             -> ContextBuilder / metadata/resource APIs
        stderr <- logging and safe diagnostics only
```

`Program.cs` only builds the generic host, configures logging to stderr, registers options/services, registers MCP tools, and awaits `RunAsync`. Do not place decompiler logic in startup.

`ILSpyTools` contains MCP SDK attributes and argument descriptions only. It validates request shape, delegates to a service, maps known exceptions to stable MCP errors, and returns a serializable response. It must not construct `PEFile` or duplicate CLI logic.

### Service extraction decision

Concrete MVP decision: keep session construction, entity resolution, resource enumeration, and structured metadata reading inside `ICSharpCode.ILSpyMcp/Services`. Do not reference or modify the CLI executable project in the first MCP change. Treat CLI behavior and tests as the specification, but use the public `ICSharpCode.Decompiler`/`ICSharpCode.ILSpyX` APIs directly instead of copying CLI methods wholesale. All MCP tools share this one service layer, so logic is not duplicated among tools. Add a later, separately reviewed refactor to move proven host-neutral services into `ICSharpCode.ILSpyX` and make both CLI and MCP consume them. This decision keeps the first delivery isolated and testable.

Use request-scoped sessions. A session owns the `PEFile`, resolver, decompiler, and any metadata reader needed by one call, and is disposed before the call completes. Do not share a decompiler or mutable type system between concurrent requests. Omit caching in the MVP; add it only after memory bounds, file-change invalidation, resolver identity, and disposal are designed.

## 5. Project and Package Changes

1. Create `ICSharpCode.ILSpyMcp.csproj` targeting `net10.0`, executable, cross-platform, packable as a dotnet tool. Reference `ICSharpCode.ILSpyX`, `ICSharpCode.Decompiler`, and only additional libraries required by the chosen MCP SDK. Reference BAML only if a later tool needs it.
2. Create `ICSharpCode.ILSpyMcp.Tests.csproj` targeting `net11.0`, using the repository's NUnit and Microsoft Testing Platform conventions. Reference the MCP project and test fixture dependencies.
3. Add a central `ModelContextProtocol` package version in `Directory.Packages.props`; use the verified stable `2.2.0` baseline unless implementation-time verification selects a newer compatible stable version. Do not add preview packages without documenting why.
4. Run `pwsh ./updatedeps.ps1` and commit the generated lock files for every affected project. Never hand-edit lock files.
5. Add both projects to `ILSpy.sln` and the cross-platform project list in `ILSpy.XPlat.slnf`.
6. Add packaging/readme metadata: NuGet package ID `ICSharpCode.ILSpyMcp`, framework-dependent dotnet tool command `ilspy-mcp`, repository version, MIT license, icon/readme, and ILSpy project metadata. Keep the ID and command consistent in install docs and smoke tests.

Before coding against SDK attributes or method signatures, inspect the samples for the pinned SDK version. The verified current pattern is `AddMcpServer().WithStdioServerTransport().WithTools<ToolType>()`, `[McpServerToolType]`, `[McpServerTool]`, and `System.ComponentModel.Description`. Reconfirm before implementation because SDK APIs can move.

## 6. Configuration Contract

Expose configuration through command-line options and equivalent environment variables. Command-line names are normative; environment names are uppercase with `ILSPY_MCP_` prefix.

| Option | Required behavior |
|---|---|
| `--allowed-root <absolute path>` | Repeatable. Canonicalize once at startup. At least one root is required by default. |
| `--reference-root <absolute path>` | Optional repeatable roots for dependency resolution; apply the same containment policy. |
| `--max-output-chars <n>` | Default `200000`; valid `1000..1000000`. Global response text cap. |
| `--max-page-size <n>` | Default `100`; valid `1..500`. Clamp client page sizes to this value. |
| `--timeout-seconds <n>` | Default `60`; valid `1..600`. Per-request linked-token timeout. |
| `--max-concurrent-decompilations <n>` | Default `2`; valid `1..16`. Bounded semaphore count. |
| `--log-level <level>` | Configure stderr logging only. |

Require absolute paths in the MVP. Reject relative assembly, allowed-root, reference-root, and per-request reference paths with `invalid_request`. This avoids client-dependent working-directory behavior. Environment variables for repeated roots use the platform path-list separator: `ILSPY_MCP_ALLOWED_ROOTS` and `ILSPY_MCP_REFERENCE_ROOTS`. Other environment variables map one-to-one, for example `ILSPY_MCP_MAX_OUTPUT_CHARS`. Command-line values override environment values.

Canonicalize every assembly and reference path with `Path.GetFullPath`. Require a regular existing file. Resolve symlinks/reparse points where platform APIs permit and re-check that the real target remains below an allowed root. Containment must be separator-aware: `/tmp/build-secret/a.dll` is not inside `/tmp/build`. Use OS-appropriate case sensitivity. Reject paths outside all configured roots with `path_not_allowed`.

Default to no arbitrary filesystem access. A missing allowed root is a startup configuration error, not permission to scan the current directory.

## 7. Shared Contracts

All tool responses must be JSON-serializable, deterministic, and bounded. Include a `schemaVersion` string (start at `1`) and a `diagnostics` array. Do not return raw exception objects.

Common pagination fields for list-like results:

```text
items: array
returnedCount: integer
totalCount: integer or null
nextCursor: opaque string or null
truncated: boolean
```

Use stable ordinal ordering before pagination. Cursors must encode the last stable sort key, not an array index that changes between calls. Reject malformed cursors with `invalid_request`.

Common bounded text fields include `text`, `returnedCharacters`, `maxCharacters`, and `truncated`. Cut on Unicode scalar boundaries; never split a UTF-16 surrogate pair. Prefer a successful truncated result for decompilation, with a diagnostic explaining the limit, rather than an oversized response.

Selectors are mutually exclusive. A selector record may contain exactly one of `typeName`, `memberDocumentationId`, or `metadataToken`; `wholeAssembly=true` is the fourth exclusive option for `decompile`. Empty strings are invalid. Tokens accept decimal or `0x` hexadecimal, then must be validated against the selected module and handle kind.

Map failures to these stable codes: `invalid_request`, `path_not_allowed`, `file_not_found`, `invalid_assembly`, `entity_not_found`, `ambiguous_entity`, `unsupported_entity`, `resolution_failed`, `decompilation_failed`, `cancelled`, `timeout`, and `output_limit_exceeded`. Include a short safe message and field name where applicable. Do not include stack traces, resolver search paths, or unrelated local paths.

## 8. MCP Tool Catalog

Implement tools in the order listed in the work packages. Names and fields below are the contract; do not add aliases without a compatibility reason.

### `inspect_assembly`

Input: `path`; optional `referencePaths` only when the server configuration permits them.

Behavior: authorize and canonicalize the path; open a request-scoped session; read module/assembly identity, target framework where detectable, architecture, runtime metadata, type count, and resource count. Do not decompile every type.

Response: canonical display path (only the requested path), file/assembly/module names, MVID if available, target framework, machine/bitness, metadata/runtime version, type count, resource count, and diagnostics.

Tests: valid managed DLL/EXE, native/non-managed file (`invalid_assembly`), missing path, denied path, and no stdout contamination.

### `list_types`

Input: `path`; optional `kind` (`class`, `struct`, `interface`, `enum`, `delegate`, or `all`), `namespacePrefix`, `query`, `cursor`, and `pageSize`.

Behavior: enumerate definitions from the decompiler type system, filter, sort by fully qualified reflection name using ordinal comparison, and paginate. Return kind, namespace, reflection/full name, declaring type when nested, and metadata token.

Tests: nested types, filters, case/ordering contract, page boundaries, invalid page/cursor, and stable results across repeated calls.

### `decompile`

Input: `path`; exactly one of `wholeAssembly`, `typeName`, `memberDocumentationId`, or `metadataToken`; optional allowlisted settings (`languageVersion`/language, `showXmlDocumentation`, `showIL` only if the decompiler API supports it safely), `maxCharacters`, and `includeDiagnostics`. Do not expose arbitrary option bags.

Behavior: resolve the selector in the selected module; invoke `CSharpDecompiler` directly; use type-specific decompilation for a type and entity-specific decompilation for a member. Whole-assembly output must be subject to a stricter maximum and must return truncation metadata. Reject multiple selectors.

Response: selector echo, resolved entity identity/token, language, C# text, returned/max character counts, `truncated`, and diagnostics.

Tests: whole assembly, type, XML documentation ID, token, malformed token, missing/ambiguous entity, foreign module entity, settings validation, Unicode-safe truncation, cancellation, timeout, and decompiler exception mapping.

### `disassemble_method`

Input: `path` plus exactly one method selector (`memberDocumentationId` or `metadataToken`), optional `maxCharacters`.

Behavior: resolve only a method definition. Use `MethodBodyDisassembler` with the request cancellation token, not the CLI's whole-module `ReflectionDisassembler` path. Return method identity/token, IL text, and truncation metadata. Reject properties, fields, events, type tokens, abstract methods without a body, and unsupported handles with `unsupported_entity` or a clear diagnostic.

Tests: normal method, async/iterator method, abstract/no-body method, non-method selector, invalid token, and cancellation.

### `get_entity_context`

Input: `path`; exactly one supported entity selector; optional `maxTokens`, `includeIl`, and `includeCallGraph`.

Behavior: resolve a type, method, field, property, or event; construct `ContextBuilder` with bounded options; call `ContextBuilder.Build(entity, decompiler)`; enforce the request and server token/character limits after building. Preserve `UnavailableSections` as diagnostics rather than failing if optional metadata is unavailable.

Response: fully qualified name, assembly, target framework, decompiled C#, attributes, implemented interfaces, string literals, optional callers/callees, optional IL, unavailable sections, estimated/returned token count, and truncation metadata.

Tests: each supported handle kind, entity from another module, max-token reduction, optional IL/call graph, unavailable metadata, and deterministic ordering.

### `list_resources`

Input: `path`, optional `query`, `cursor`, and `pageSize`.

Behavior: adapt `ResourceExtensions.EnumerateResourcePaths` or a host-neutral equivalent. Return resource name/path, manifest metadata, known type, and size where cheaply available. Do not write or extract files.

Tests: embedded resources, empty resources, filtering, pagination, and malformed assembly.

### `dump_metadata_table`

Input: `path`, `table` (case-insensitive ECMA-335 table name or supported numeric value), optional `cursor`, `pageSize`, and `maxCharacters`.

Behavior: adapt `MetadataTableDumper` so row loading is a service returning bounded structured rows. Do not call its console writer. Preserve deterministic columns and values and expose supported table names in an invalid-table diagnostic.

Response: table name/number, ordered column names, row objects, row counts, cursor, and truncation metadata.

Tests: `TypeDef`, `Property`, `MethodSemantics`, `NestedClass`, `ClassLayout`, numeric names, case-insensitivity, unknown table, pagination, and JSON serialization.

## 9. Error, Cancellation, and Resource Rules

Every MCP method receives the SDK cancellation token and passes it through the service, decompiler, disassembler, and output loops. Create a linked token with the configured timeout. Distinguish caller cancellation (`cancelled`) from server timeout (`timeout`). Always dispose `PEFile`, metadata readers, streams, and temporary resolver state in `finally`/`using` paths.

Acquire the bounded decompilation semaphore around expensive decompilation/context/IL operations. Do not hold it while validating arguments or formatting an already-built small response. On cancellation, release it promptly.

Catch only known recoverable metadata/decompiler exceptions at service boundaries. Convert unknown exceptions to `decompilation_failed` with a correlation id logged to stderr; never serialize a stack trace.

## 10. Ordered Work Packages

Each package is TDD-ordered: add the named failing tests, run them to demonstrate red, implement the smallest change, run the named green command, and record acceptance evidence. Do not begin a later package while its prerequisites are red.

### WP0 - SDK and repository wiring

Files: `Directory.Packages.props`, both new project files, `ILSpy.sln`, `ILSpy.XPlat.slnf`, lock files, and a short server README.

Tasks: verify the pinned MCP SDK sample/API; create projects; add references; configure tool command and package metadata; add central version; update locks; add projects to solution/filter.

Red test: a project-load/build smoke test that expects the server assembly and MCP registration to exist.

Green commands: `pwsh ./updatedeps.ps1`; `pwsh ./build.ps1 --no-restore`; `dotnet test --project ICSharpCode.ILSpyMcp.Tests/ICSharpCode.ILSpyMcp.Tests.csproj --no-build --report-trx`.

Acceptance: clean restore in locked mode, server executable launches, no UI dependency, and no code is emitted on stdout before MCP transport starts.

### WP1 - Host, options, logging, and path policy

Files: `Program.cs`, `Configuration/McpServerOptions.cs`, `Services/PathAccessPolicy.cs`, corresponding tests.

Tests first: missing root, absolute/relative path behavior, sibling-prefix bypass, symlink/reparse escape, reference-root policy, file-vs-directory, invalid numeric options, and logger output capture.

Implementation: bind options; canonicalize roots once; enforce separator-aware containment; configure console logging with `LogToStandardErrorThreshold = LogLevel.Trace` or equivalent for the pinned SDK; register stdio transport; ensure all startup diagnostics use stderr.

Green command: focused MCP tests with `--report-trx`, then cross-platform build.

Acceptance: denied paths never reach `PEFile`; stdout contains only protocol bytes in a child-process test.

### WP2 - Session factory and entity resolution

Files: `AssemblySessionFactory.cs`, `EntityResolver.cs`, selector contracts, tests.

Tests first: fixture assembly path, type reflection name, nested type, XML documentation ID, decimal/hex token, malformed token, out-of-range token, ambiguous name, missing entity, foreign module, and unsupported handle.

Implementation: move/adapt proven CLI resolution semantics; construct `PEFile`, `UniversalAssemblyResolver`, `CSharpDecompiler`; add references only from authorized roots; make ownership checks explicit.

Acceptance: all selectors produce one entity or one stable error code; sessions are disposed; no CLI process or console writer is involved.

### WP3 - Core service and output limits

Files: `AssemblyDecompilerService.cs`, `OutputLimiter.cs`, response contracts, tests.

Tests first: inspect/list/decompile happy paths, deterministic sorting, cursor round trip, max page clamp, max character truncation, surrogate-pair boundary, exception mapping, timeout, and caller cancellation.

Implementation: create service methods matching the catalog; use request-scoped sessions; apply limits at the final response boundary; preserve diagnostics and counts.

Acceptance: service tests pass without MCP transport; outputs are serializable and bounded for adversarial sizes.

### WP4 - MCP tool adapter

Files: `Mcp/ILSpyTools.cs`, error mapper, adapter tests.

Tests first: exact tool names/descriptions, required/optional fields, mutually exclusive selectors, invalid requests, stable error envelopes, and cancellation propagation.

Implementation: add SDK attributes to thin methods; inject service/options; map service result/errors to SDK return values or `McpException` according to the pinned SDK; never log to stdout.

Acceptance: MCP tool discovery lists exactly the supported tools and their descriptions are useful to an LLM without hidden assumptions.

### WP5 - Resources and metadata

Files: `MetadataTableService.cs`, resource adapter, tests.

Tests first: the existing CLI table/resource fixtures ported to structured service responses, including numeric/case-insensitive tables and pagination.

Implementation: separate row loading from console formatting; keep supported table list explicit; reuse resource enumeration without extraction.

Acceptance: no unbounded writer output, all rows have stable JSON shape, and unsupported tables return valid-request diagnostics.

### WP6 - Protocol and packaging

Files: `StdioProtocolTests.cs`, package README, CI workflow changes if required.

Tests first: launch the executable as a child process, send MCP initialize/list-tools/call requests, parse stdout as JSON-RPC, assert stderr may contain logs but stdout has no non-protocol text, then cancel and shut down cleanly.

Implementation: add packaging and install instructions; publish a self-contained or framework-dependent tool according to repository conventions; document required `--allowed-root`.

Acceptance: `dotnet tool install --tool-path <temp> <package>` followed by `<temp>/ilspy-mcp --allowed-root <fixture-root>` can initialize, list tools, inspect a fixture, decompile a type, and exit without orphan processes.

### WP7 - Cross-client acceptance

Run against one pinned/current target version each for Claude Desktop/Claude Code, Codex, OpenCode, and Pi. Use the conceptual command `ilspy-mcp --allowed-root /absolute/path/to/artifacts`; development config may use `dotnet run --project ICSharpCode.ILSpyMcp --`.

Create date-stamped examples from current official/client-maintained documentation. As verified on 2026-08-20, use these expected shapes and re-check them before release:

```text
Claude Code:
  claude mcp add --transport stdio ilspy -- ilspy-mcp --allowed-root /absolute/path/to/artifacts

Codex:
  codex mcp add ilspy -- ilspy-mcp --allowed-root /absolute/path/to/artifacts
  Equivalent config.toml table: [mcp_servers.ilspy] with command and args.

OpenCode:
  opencode.json -> mcp.ilspy.type = local
                  mcp.ilspy.command = [ilspy-mcp, --allowed-root, /absolute/path/to/artifacts]

Pi:
  Pi has no native MCP client contract. Install and pin a maintained adapter such as
  pi-mcp-adapter, then add the same command/args under .mcp.json -> mcpServers.ilspy.
```

For each client: configure one stdio server, initialize, list tools, call `inspect_assembly`, call `decompile`, verify a denied path is rejected, and interrupt a long call. Record client version, config syntax, result, and known limitations. Do not claim native Pi support; report adapter name/version and smoke-test result.

## 11. Test Matrix

| Area | Required cases |
|---|---|
| Path policy | allowed root, sibling prefix bypass, symlink escape, missing/non-file, OS case behavior |
| Contracts | required fields, unknown fields policy, selector exclusivity, cursor/page validation |
| Resolution | type, nested type, XML ID, token, ambiguous, foreign module, unsupported handle |
| Decompiler | assembly/type/member, language allowlist, malformed/invalid assembly, exception mapping |
| IL/context | method body, no body, optional IL/call graph, budget reduction, unavailable sections |
| Lists | stable sort, filters, pagination, max page clamp, empty result |
| Metadata/resources | known tables, numeric names, unknown table, resource listing, JSON shape |
| Limits | characters, tokens, row count, Unicode boundary, truncation metadata |
| Operations | cancellation, timeout, semaphore limit, disposal, repeated calls |
| Protocol | initialize, list tools, call, invalid JSON-RPC, stderr logging, clean shutdown |
| Packaging | locked restore, build, install tool, executable invocation |
| Platforms | Windows, Linux, macOS path and stdio behavior |

Use a small checked-in fixture type with methods, properties, nested types, resources, and predictable metadata. Prefer `typeof(TestAssemblyFixtures).Assembly.Location` for the test path, matching existing CLI tests. Tests must fail loudly; never return early when a fixture is absent.

## 12. Security and Reliability Review

Threats and required mitigations:

- Arbitrary file read: mandatory allowed roots, canonicalization, symlink/reparse verification, and reference-root policy.
- Path confusion: absolute-path contract, separator-aware containment, OS-aware comparison.
- Resource exhaustion: output caps, page caps, timeouts, bounded concurrency, no cache, and request disposal.
- Protocol corruption: stdout reserved for MCP; stderr-only logging test.
- Information leakage: safe errors without stack traces/resolver paths; do not expose unrelated filesystem paths.
- Malformed assemblies: catch and classify metadata/decompiler failures; never crash the host for one bad request.
- Dependency resolution surprises: references only from configured roots; no network/package probing.
- Stale data: no cache in MVP. If a cache is added later, key it by canonical path, file identity/timestamp, resolver roots, and settings, with bounded eviction.

Before release, perform a threat-model review and document whether roots may contain secrets. The server is read-only from its own perspective, but decompilation can reveal source-like intellectual property; installation docs must make this explicit.

## 13. Build, CI, and Release Procedure

Run in this order from the repository root:

1. `pwsh ./restore.ps1`.
2. `pwsh ./updatedeps.ps1` after any package/project reference change.
3. `pwsh ./build.ps1 --no-restore`.
4. Focused MCP tests with `--report-trx`.
5. Full cross-platform tests/build using the repository-supported solution/filter commands, always with `--report-trx` for tests.
6. `rtk git diff --check` and inspect generated lock-file diffs.

CI must build/test on Windows, Linux, and macOS. Add a package smoke job that installs the generated tool into a temporary directory and uses a fixture assembly. Do not require a desktop UI or network service.

The README must show installation, allowed-root configuration, development invocation, logging behavior, supported tools, read-only limitations, and client configuration examples. Keep client snippets versioned/date-stamped where syntax differs.

## 14. Definition of Done

- Standalone `net10.0` MCP executable and `net11.0` test project are in both cross-platform solution definitions.
- Pinned stable MCP SDK is centrally versioned and lock files are regenerated.
- Stdio initialize/list-tools/call works with protocol-only stdout and stderr diagnostics.
- All MVP tools have the schemas, stable errors, cancellation, limits, and tests defined above.
- No path operation escapes configured roots; no writes/network/AI credentials occur.
- Decompiler sessions are request-scoped and disposed; concurrency is bounded.
- Existing CLI behavior remains green; any shared extraction is behavior-preserving and covered by existing plus new tests.
- Tool package installs and runs from a clean temporary location.
- Claude, Codex, and OpenCode smoke tests pass; Pi support status is recorded against a concrete version/adapter.
- Documentation states limitations and gives a reproducible command.

## 15. Deferred Backlog

Evaluate only after MVP usage and profiling:

- Bounded session cache with invalidation.
- Streamable HTTP transport with authentication and remote threat model.
- MCP resource templates backed by an explicit assembly registry.
- Project decompilation/export, BAML/XAML extraction, and read-only resource retrieval.
- Rich call-graph queries and dependency graph pagination.
- Native-image/ReadyToRun-specific inspection where supported.
- Progress notifications for very large assemblies.

Each deferred item requires a new security/performance design and compatibility tests; do not smuggle it into the MVP tool contracts.

## 16. Locked Implementation Defaults

Use these choices unless a maintainer explicitly changes the plan before implementation:

1. Pin stable `ModelContextProtocol` `2.2.0`. If restore shows it is unavailable or superseded for a security reason, stop and request a plan update instead of silently choosing a preview.
2. Package ID is `ICSharpCode.ILSpyMcp`; executable/tool command is `ilspy-mcp`; publish a framework-dependent dotnet tool.
3. Keep host-neutral-looking services in the MCP project for the MVP; do not refactor `ilspycmd` in the same change.
4. Use the limits in Section 6: 200,000 output characters, page size 100, 60-second timeout, and concurrency 2, with the stated hard ranges.
5. Smoke-test Pi through a pinned `pi-mcp-adapter`; do not describe it as native Pi MCP support.
6. Enable whole-assembly decompilation only through explicit `wholeAssembly: true`. Apply the same 200,000-character default/hard request cap and return `truncated: true` when exceeded. Never create an output directory.

## 17. References

- `CLAUDE.md` for target frameworks, scripts, TDD, headers, and lock-file rules.
- `ICSharpCode.ILSpyCmd/IlspyCmdProgram.cs` for current CLI session, resolution, decompilation, resource, and IL behavior.
- `ICSharpCode.ILSpyX/AI/ContextBuilder.cs` for bounded entity context.
- `ICSharpCode.ILSpyCmd/MetadataTableDumper.cs` and `ICSharpCode.ILSpyCmd.Tests/DumpTableOptionTests.cs` for metadata table behavior and fixtures.
- `ICSharpCode.ILSpyCmd.Tests/MemberOptionTests.cs` for selector/error semantics.
- Official MCP C# SDK samples: `samples/QuickstartWeatherServer/Program.cs` and `samples/QuickstartWeatherServer/Tools/WeatherTools.cs`.
- MCP protocol documentation for stdio transport and current protocol version; verify against the pinned SDK before implementation.
- Claude Code MCP documentation, Codex MCP documentation, OpenCode MCP server documentation, and the maintained `pi-mcp-adapter` README; configuration shapes above were checked on 2026-08-20 and must be rechecked before release.
