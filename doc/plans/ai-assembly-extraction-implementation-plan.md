# AI Assembly Extraction Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use `superpowers:subagent-driven-development` (recommended) or `superpowers:executing-plans` to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

> **Verified against the codebase** on 2026-08-21: file paths, type/namespace names, constructors, MEF attributes, composition mechanics, package versions, and CI steps referenced below were cross-checked against the working tree. Load-bearing verified facts: `AppComposition` catalogs only ILSpyX, the app assembly, and `*.Plugin.dll` (it never scans project references); `ContextBuilder` depends on internal ILSpyX analyzer helpers; `AISecurityAnalyzer` implements `IAnalyzer` and is discovered via MEF; the security confidence threshold is the double `0.70`, not an integer percentage; `PromptEmbedder` hard-codes its output namespace; mid-migration ILSpyX needs temporary bridge references to the new assemblies until its own AI consumers have moved.

**Goal:** Move only the implemented, user-authored AI functionality out of `ICSharpCode.ILSpyX` into `ICSharpCode.ILSpy.AI` and `ICSharpCode.ILSpy.AI.Decompiler`, migrate every production caller and test, and leave all Avalonia UI and desktop command wiring in `ILSpy`.

**Architecture:** Create a portable AI module that owns provider transport, configuration/profile selection, credential storage, prompts, and non-decompiler utilities. Create a second decompiler adapter module that owns every AI implementation which accepts or inspects `ICSharpCode.Decompiler` entities. `ILSpy` remains the desktop host: it retains Avalonia views, view models, menu commands, dialogs, settings panels, and the `AISelectionSettingsHost` adapter, but references the new modules instead of `ILSpyX` AI namespaces.

**Tech Stack:** .NET 10, SDK-style projects, Central Package Management, NuGet lock files, `System.Composition`, `HttpClient`, `System.Text.Json`, `YamlDotNet`, `ICSharpCode.Decompiler`, Avalonia, NUnit, AwesomeAssertions, GitHub Actions.

## Global Constraints

- Scope is limited to implemented AI code authored for this feature set. Do not migrate or implement the proposed AI-assisted decompilation features in `doc/plans/ai-assisted-decompilation-plan.md`.
- Do not migrate or implement the planned general obfuscation detector described in `doc/plans/obfuscation-analysis-implementation-plan.md`.
- Keep all Avalonia `.axaml` files, controls, views, view models, dialogs, context-menu entries, desktop commands, option panels, and `AISelectionSettingsHost` in the `ILSpy` project.
- Do not add a runtime plugin loader or change ILSpy's existing plugin discovery behavior.
- Preserve public behavior: settings schema, secret storage identities, provider validation, streaming behavior, prompt IDs/content/fallback, context budgets, privacy consent, rename annotation behavior, security confidence filtering, and chat commands must remain unchanged.
- `ICSharpCode.ILSpy.AI` must not reference `ICSharpCode.Decompiler`, `ICSharpCode.ILSpyX`, `ILSpy`, Avalonia, Dock, or desktop settings classes.
- `ICSharpCode.ILSpy.AI.Decompiler` may reference only `ICSharpCode.ILSpy.AI` and `ICSharpCode.Decompiler` among the three ILSpy product layers. It must not reference `ICSharpCode.ILSpyX` or `ILSpy`.
- `ILSpy` retains its current references to `ICSharpCode.ILSpyX` and `ICSharpCode.Decompiler` for unrelated functionality, and gains explicit references to both new AI projects.
- Preserve `net10.0`, nullable warnings-as-errors, strong naming using `ICSharpCode.Decompiler.snk`, central package versions, and `packages.lock.json` for every new project.
- Do not broaden `InternalsVisibleTo` merely to make extraction compile. Move internal implementation with its public facade, or introduce a narrow public seam where one is actually required.
- The new root namespace `ICSharpCode.ILSpy.AI` intentionally coincides with the desktop app's existing `ICSharpCode.ILSpy.AI` namespace (see `ILSpy/ViewLocator.cs`, `ILSpy/AI/AISelectionSettingsHost.cs`). There are no type-name collisions today; moved type names must stay collision-free, and the shared namespace is not a reason to move desktop types into the portable module.
- `AppComposition` (`ILSpy/AppEnv/AppComposition.cs`) catalogs exactly `typeof(IAnalyzer).Assembly` (ILSpyX), the desktop assembly, and `*.Plugin.dll` files — it does not scan project references. Any MEF export that leaves ILSpyX must have its new assembly added to `RegisterPluginResolver` in the same task that moves it; System.Composition resolution failures are lazy and surface at runtime, not compile time.
- Temporary bridge references are required, not a violation: while ILSpyX still compiles not-yet-migrated AI files that consume already-moved types (notably `AnalyzerContext` until Task 3.3), `ICSharpCode.ILSpyX.csproj` holds transitional ProjectReferences to the new AI assemblies. Add them with the first moved type; Task 3.4 removes them. The final dependency graph is unchanged.
- Migrate callers before deleting legacy source. Each phase must build and pass its focused tests before the next phase starts.
- Do not modify unrelated existing worktree changes, including `doc/plans/obfuscation-analysis-implementation-plan.md` and `.zcode/`.

---

## Target Dependency Graph

```text
ICSharpCode.Decompiler
        ^
        |
ICSharpCode.ILSpy.AI <--- ICSharpCode.ILSpy.AI.Decompiler
        ^                         ^
        |                         |
        +-------------------------+
                    ILSpy
```

`ICSharpCode.ILSpyX` must have no AI namespaces, prompt assets, prompt-generation target, or AI-specific package references after the final phase. `ILSpy` remains responsible for application composition and desktop lifecycle.

## Target Project and File Map

### New projects

- Create: `ICSharpCode.ILSpy.AI/ICSharpCode.ILSpy.AI.csproj`
  - Portable, packable `net10.0` AI assembly.
  - Owns providers, profile/selection state, secure credential implementation, prompt loading/assets, chat/history, and provider-independent utilities.
- Create: `ICSharpCode.ILSpy.AI.Decompiler/ICSharpCode.ILSpy.AI.Decompiler.csproj`
  - Packable `net10.0` adapter assembly.
  - References `ICSharpCode.ILSpy.AI` and `ICSharpCode.Decompiler`.
  - Owns decompiler-context construction, LLM-backed analysis/rename/search/security features, and rename annotation transforms.
- Create: `ICSharpCode.ILSpy.AI.Tests/ICSharpCode.ILSpy.AI.Tests.csproj`
  - `net10.0`, not packable; replaces the portable/core AI tests currently in `ICSharpCode.ILSpyX.Tests/AI` plus the model-level half of `ICSharpCode.ILSpyX.Tests/Settings/AISettingsTests.cs` and `AISettingsProfilesTests.cs`.
- Create: `ICSharpCode.ILSpy.AI.Decompiler.Tests/ICSharpCode.ILSpy.AI.Decompiler.Tests.csproj`
  - `net10.0`, not packable; replaces the decompiler-aware AI tests currently split across `ICSharpCode.ILSpyX.Tests/AI`, `Annotations`, and `Analyzers`. No AI search tests exist today (there is no `Search` test folder); their regression tests are written new, not moved.

### Move to `ICSharpCode.ILSpy.AI`

Move these files from `ICSharpCode.ILSpyX/AI/`, retaining namespaces as `ICSharpCode.ILSpy.AI` unless a type already belongs to a more specific public namespace established by the implementation:

- `AICredentialMigration.cs`
- `AIProfile.cs`
- `AIPromptMetadata.cs`
- `AIPromptProvider.cs`
- `AIProviderCatalog.cs`
- `AIProviderFactory.cs`
- `AISelectionService.cs`
- `AISelectionTypes.cs`
- `ChatHistory.cs`
- `ChatMessage.cs`
- `EmbeddingStore.cs`
- `ILLMProvider.cs`
- `LLMMessage.cs`
- `LLMRequest.cs`
- `MarkdownCodeFenceExtractor.cs`
- `SecureKeyStorage.cs`
- `SecureKeyStorageBackends.cs`
- `TokenCounter.cs`
- `Providers/OpenAIProvider.cs`
- `Providers/AnthropicProvider.cs`
- `prompts/*.prompt`
- `prompts/README.md`
- `EmbeddedPrompts.g.cs` generated output, but do not hand-edit it.

Do **not** move `ICSharpCode.ILSpyX/Settings/AISettings.cs` unchanged: it implements `ICSharpCode.ILSpyX.Settings.ISettingsSection`, which would make the portable AI project depend on ILSpyX. Instead split its responsibilities: move the AI-owned observable settings state, profile validation/migration, and XML serialization logic into `ICSharpCode.ILSpy.AI/Settings/AISettingsModel.cs`; create an `ILSpy`-owned `AISettingsSection : ISettingsSection` adapter that delegates loading/saving and change notification to the model while retaining XML section name `AISettings`. Change `AISelectionHost` to expose the portable model, not the ILSpyX settings section. The adapter must preserve the existing schema-2 XML and legacy migration behavior byte-for-byte where existing tests require it.

### Move to `ICSharpCode.ILSpy.AI.Decompiler`

Move these files, preserving non-AI placement only if required by existing extensibility discovery; otherwise group them under `AI` within the new project:

- `ICSharpCode.ILSpyX/AI/AIExplanationService.cs`
- `ICSharpCode.ILSpyX/AI/ContextBuilder.cs`
- `ICSharpCode.ILSpyX/AI/DecompilationContext.cs`
- `ICSharpCode.ILSpyX/AI/RenameSuggester.cs`
- `ICSharpCode.ILSpyX/AI/BatchRenameSuggester.cs`
- `ICSharpCode.ILSpyX/Search/AISearchStrategy.cs`
- `ICSharpCode.ILSpyX/Search/SemanticSearchStrategy.cs`
- `ICSharpCode.ILSpyX/Analyzers/AISecurityFinding.cs`
- `ICSharpCode.ILSpyX/Analyzers/Builtin/AISecurityAnalyzer.cs`
- `ICSharpCode.ILSpyX/Analyzers/Builtin/AISecurityAuditService.cs`
- `ICSharpCode.ILSpyX/Annotations/RenameAnnotations.cs`
- `ICSharpCode.ILSpyX/Annotations/RenameAnnotationTransform.cs`

Discovery-contract facts, verified against the current tree:

- `RenameAnnotationTransform : IAstTransform` implements a `ICSharpCode.Decompiler` contract (there is no `IAnnotationTransform` in ILSpyX) and is registered manually by `ILSpy/Languages/CSharpLanguage.cs` (~lines 334 and 527); no MEF is involved. Keep the type name — `ILSpy.Tests/Views/DebugStepsTests.cs` asserts the string `"RenameAnnotationTransform"` in debug output.
- `AISearchStrategy` and `SemanticSearchStrategy` are static classes with no base class and no MEF attributes; `ILSpy/Search/SearchPaneModel.cs` calls them directly in `StartSpecialSearch`. The `SearchMode.AI`/`SearchMode.Semantic` enum members stay in `ICSharpCode.ILSpyX/Search/AbstractSearchStrategy.cs`.
- `AISecurityAnalyzer` currently implements ILSpyX's `IAnalyzer`, takes `AnalyzerContext` parameters, and carries `[ExportAnalyzer(Header = "Security Risks (AI)", Order = 1000)] [Shared]`; the desktop discovers it through the MEF analyzer registry. It moves as a plain service with no `IAnalyzer`/`AnalyzerContext` dependency, and a thin desktop MEF adapter keeps the tree-analyzer feature working (Task 3.3).
- `AISecurityFinding` implements the decompiler's `ISymbol` and moves cleanly; its namespace change ripples into `ILSpy/Analyzers/AISecurityFindingTreeNode.cs`.
- `AISecurityAuditService` is a plain class the desktop news up directly (`ILSpy/AI/AISecurityAuditContextMenuEntry.cs`); its file also defines `AISecurityAuditPlan/Progress/Result/LimitException`, and `AnalyzerContext.AIProgress` currently references `AISecurityAuditProgress` (removed from `AnalyzerContext` in Task 3.3).
- `DecompilationContext` has zero decompiler dependencies (pure serialized record); it lives in this module by design choice because it is the decompiler-context DTO.

### Remain in `ILSpy`

Do not move any of these categories:

- All files under `ILSpy/AI/`, including `AIChatPaneModel`, `AIOutputPaneModel`, `AIChatFeatureCommands`, `AIEntityDecompilation`, `AssemblySummaryContextBuilder`, all context-menu entries, all dialogs and their view models, `Controls/*`, and all `.axaml` files.
- `ILSpy/AI/AISelectionSettingsHost.cs`.
- `ILSpy/Options/AISettingsViewModel.cs`, `AISettingsPanel.axaml`, and `AISettingsPanel.axaml.cs`.
- Desktop integration in `ILSpy/Search/SearchPaneModel.cs`, `ILSpy/Analyzers/AnalyzerSearchTreeNode.cs`, `ILSpy/Analyzers/AISecurityFindingTreeNode.cs`, `ILSpy/Languages/CSharpLanguage.cs`, and `ILSpy/ViewLocator.cs`.
- All existing desktop tests under `ILSpy.Tests/AI` and `ILSpy.Tests/Options` remain in `ILSpy.Tests`; update their project references/usings only.

### Project/build files that must be reviewed

- `ILSpy.sln`
- `ILSpy.XPlat.slnf`
- `ILSpy.Desktop.slnf`
- `Directory.Packages.props`
- `Directory.Build.props`
- `packages.lock.json` files for all affected and new projects
- `ICSharpCode.ILSpyX/ICSharpCode.ILSpyX.csproj`
- `ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj`
- `ILSpy/ILSpy.csproj`
- `ILSpy.Tests/ILSpy.Tests.csproj`
- `.github/workflows/build-ilspy.yml`

---

## Phase 0: Baseline and Dependency Inventory

**Purpose:** Establish a reproducible pre-migration baseline and prevent accidental migration of unimplemented or non-AI code.

### Task 0.1: Record the scoped source manifest

**Files:**
- Create: `doc/plans/ai-assembly-extraction-source-manifest.md`
- Inspect: every file listed in the Target Project and File Map.

**Interfaces:**
- Consumes: current compiled source tree.
- Produces: a checked migration manifest stating each file's source project, destination project, whether it is production/test/resource/build infrastructure, and whether it remains in `ILSpy`.

- [ ] List every current AI production file and classify it as `AI`, `AI.Decompiler`, `ILSpy desktop`, or `out of scope`.
- [ ] Explicitly list these as out of scope: the documents and any future source for broad static obfuscation detection, AI AST transforms, semantic variable naming, AI comment insertion, cleanup transforms, and intent reconstruction.
- [ ] List every AI test file in `ICSharpCode.ILSpyX.Tests/AI` (including `Providers/`), `ICSharpCode.ILSpyX.Tests/Settings` (`AISettingsTests.cs`, `AISettingsProfilesTests.cs`), `ICSharpCode.ILSpyX.Tests/Annotations`, `ICSharpCode.ILSpyX.Tests/Analyzers`, `ILSpy.Tests/AI`, and `ILSpy.Tests/Options` with its target destination. Record that no tests exist for `AISearchStrategy`/`SemanticSearchStrategy` today and that `ILSpy.Tests/Analyzers/Library/ExportAnalyzerAttributeTests.cs` asserts the analyzer inventory of the ILSpyX assembly and must be updated when `AISecurityAnalyzer` moves.
- [ ] List all prompt IDs and consumers: `explanation`, `rename`, `chat`, `security`, `security_audit`, `generate_docs`, `search`, and `assembly_summary`.
- [ ] Confirm that no source move changes prompt text, settings XML element names, credential identity generation, or public provider request contracts.
- [ ] Commit the manifest separately: `docs: record AI assembly extraction source manifest`.

### Task 0.2: Run and record baseline verification

**Files:**
- Create: `doc/plans/ai-assembly-extraction-baseline.md`
- Inspect: `.github/workflows/build-ilspy.yml` and existing project test commands.

- [ ] Restore the desktop solution filter in locked mode.
  Run: `dotnet restore ILSpy.Desktop.slnf --locked-mode`
  Expected: success with no lock-file changes.
- [ ] Build the desktop solution filter in Release.
  Run: `dotnet build ILSpy.Desktop.slnf --configuration Release --no-restore`
  Expected: success with zero warnings because warnings are errors.
- [ ] Run the current ILSpyX AI tests.
  Run: `dotnet test --project ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --configuration Release --no-build --filter "FullyQualifiedName~AI"`
  Expected: pass.
- [ ] Run current desktop AI/options tests.
  Run: `dotnet test --project ILSpy.Tests/ILSpy.Tests.csproj --configuration Release --no-build --filter "FullyQualifiedName~AI|FullyQualifiedName~AISettings"`
  Expected: pass.
- [ ] Record exact commands, operating system, SDK version, pass/fail counts, and any existing platform-gated failures. Do not mask existing failures as migration failures.
- [ ] Commit: `test: record AI extraction baseline`.

**Phase 0 exit criteria:** The migration source set is explicit, the baseline passes or has documented pre-existing exceptions, and no implementation source has moved.

---

## Phase 1: Add Buildable Project Skeletons

**Purpose:** Add the new assemblies, test assemblies, solution entries, signing, package metadata, and references while keeping all existing AI source in ILSpyX.

### Task 1.1: Create the portable AI project

**Files:**
- Create: `ICSharpCode.ILSpy.AI/ICSharpCode.ILSpy.AI.csproj`
- Create: `ICSharpCode.ILSpy.AI/PackageReadme.md`
- Create: `ICSharpCode.ILSpy.AI/packages.lock.json`
- Modify: `Directory.Packages.props` (likely no change — `System.Composition.AttributedModel`, `Microsoft.Extensions.Logging.Abstractions`, and `YamlDotNet` are already pinned centrally)
- Modify: `ILSpy.sln`, `ILSpy.XPlat.slnf`, `ILSpy.Desktop.slnf`

**Interfaces:**
- Produces assembly: `ICSharpCode.ILSpy.AI` targeting `net10.0`.
- Must not reference: `ICSharpCode.Decompiler`, `ICSharpCode.ILSpyX`, or `ILSpy`.

- [ ] Copy only structural MSBuild properties required from `ICSharpCode.ILSpyX.csproj`: `net10.0`, nullable enabled, nullable warnings as errors, assembly signing via `..\ICSharpCode.Decompiler\ICSharpCode.Decompiler.snk` (the only shared `.snk` in the tree; there is no repo-root key), generated version attributes disabled, `RestoreLockedMode`, the RID-complete `RuntimeIdentifiers` list (`win-x64;win-arm64;linux-x64;osx-arm64`, required so locked-mode RID publishes of the app restore on any host), the `ContinuousIntegrationBuild` property (set from `GITHUB_ACTIONS` in ILSpyX), and package/source-link/SBOM conventions.
- [ ] Set package identity to `ICSharpCode.ILSpy.AI`; use a description that identifies it as provider/configuration/prompt infrastructure and does not claim it is the ILSpyX platform package.
- [ ] Add only package references justified by actual Phase 2 source: `System.Composition.AttributedModel`, `Microsoft.Extensions.Logging.Abstractions`, and `YamlDotNet`. Do not copy `Mono.Cecil`, `Markdig`, `LZ4`, metadata packages, or decompiler dependencies.
- [ ] Add the project to `ILSpy.sln`, both solution filters, and relevant solution configurations using the same conventions as existing library projects.
- [ ] Restore in locked mode, commit generated lock files, and build the empty project.
  Run: `dotnet restore ICSharpCode.ILSpy.AI/ICSharpCode.ILSpy.AI.csproj --locked-mode`
  Run: `dotnet build ICSharpCode.ILSpy.AI/ICSharpCode.ILSpy.AI.csproj --configuration Release --no-restore`
  Expected: both succeed.
- [ ] Commit: `build: add portable AI project skeleton`.

### Task 1.2: Create the decompiler AI project

**Files:**
- Create: `ICSharpCode.ILSpy.AI.Decompiler/ICSharpCode.ILSpy.AI.Decompiler.csproj`
- Create: `ICSharpCode.ILSpy.AI.Decompiler/PackageReadme.md`
- Create: `ICSharpCode.ILSpy.AI.Decompiler/packages.lock.json`
- Modify: `ILSpy.sln`, `ILSpy.XPlat.slnf`, `ILSpy.Desktop.slnf`

**Interfaces:**
- Produces assembly: `ICSharpCode.ILSpy.AI.Decompiler`.
- References: `ICSharpCode.ILSpy.AI` and `ICSharpCode.Decompiler`.
- Must not reference: `ICSharpCode.ILSpyX` or `ILSpy`.

- [ ] Use the same target framework, signing, warnings, package metadata, lock-file, CI, SourceLink, and SBOM conventions as Phase 1.1.
- [ ] Add project references to exactly `../ICSharpCode.ILSpy.AI/ICSharpCode.ILSpy.AI.csproj` and `../ICSharpCode.Decompiler/ICSharpCode.Decompiler.csproj`.
- [ ] Initially add no package reference unless a moved file proves it needs one. Do not copy ILSpyX's broad package list by habit.
- [ ] Add project entries/configurations to the solution and both filters.
- [ ] Restore and build in Release before moving sources.
- [ ] Commit: `build: add decompiler AI project skeleton`.

### Task 1.3: Create test project skeletons and CI coverage

**Files:**
- Create: `ICSharpCode.ILSpy.AI.Tests/ICSharpCode.ILSpy.AI.Tests.csproj`
- Create: `ICSharpCode.ILSpy.AI.Decompiler.Tests/ICSharpCode.ILSpy.AI.Decompiler.Tests.csproj`
- Create: lock files for both projects.
- Modify: `ILSpy.sln`, `ILSpy.XPlat.slnf`, `ILSpy.Desktop.slnf`, `.github/workflows/build-ilspy.yml`

**Interfaces:**
- `AI.Tests` references only `AI` plus the prompt build tool when prompt-generation tests need it.
- `AI.Decompiler.Tests` references `AI.Decompiler`, `AI`, and any existing fixture/test support project required by moved tests.

- [ ] Mirror test infrastructure from `ICSharpCode.ILSpyX.Tests.csproj`: `net10.0`, signed executable test project, NUnit, NUnit adapter, Microsoft Testing Platform extensions, and AwesomeAssertions.
- [ ] Configure `InternalsVisibleTo` in each new production project only for its matching strong-named test project (same key pattern as ILSpyX's existing entries). Do not grant friends to `ILSpy.Tests` by default: ILSpyX currently grants `ILSpy.Tests` internals access, so audit whether any `ILSpy.Tests` AI test reaches internals of moved types (internal constructors, `SecureKeyStorage` backends, `ContextBuilder` internals) — such tests belong in the new module test projects, which receive access through their own IVT.
- [ ] CI coverage fact: `ICSharpCode.ILSpyX.Tests` runs only inside the Windows job's solution-wide `dotnet test --solution ilspy.sln` step; there are no per-project ILSpyX test commands and no test filters anywhere in the workflow, and ILSpyX tests are not run on Linux/macOS. Adding both new test projects to `ILSpy.sln` therefore enrolls them in the Windows run automatically. If cross-platform coverage of the new modules is wanted, add explicit per-project test steps to the Linux/macOS `Desktop` job — noting that `SecureKeyStorageSmokeTests` are platform-gated and currently execute only on Windows CI. Do not remove existing test steps.
- [ ] Build the solution filter and run the empty test projects.
- [ ] Commit: `test: add AI extraction test projects`.

**Phase 1 exit criteria:** Four new projects restore, compile, are included in correct solution filters, and CI will execute their tests. No existing caller references a new AI namespace yet.

---

## Phase 2: Migrate the Portable AI Module

**Purpose:** Move provider/configuration/prompt/credential functionality as one coherent module, preserve its behavior, and remove its AI-specific ownership from ILSpyX.

### Task 2.1: Migrate contracts and dependency-free utilities

**Files:**
- Move from `ICSharpCode.ILSpyX/AI/` to `ICSharpCode.ILSpy.AI/AI/`: `ILLMProvider.cs`, `LLMMessage.cs`, `LLMRequest.cs`, `AISelectionTypes.cs`, `ChatMessage.cs`, `ChatHistory.cs`, `EmbeddingStore.cs`, `MarkdownCodeFenceExtractor.cs`, `TokenCounter.cs`.
- Move matching tests from `ICSharpCode.ILSpyX.Tests/AI/` to `ICSharpCode.ILSpy.AI.Tests/AI/`: `LLMContractsTests.cs`, `AISelectionContractsTests.cs`, `ChatHistoryTests.cs`, `EmbeddingStoreTests.cs`, `MarkdownCodeFenceExtractorTests.cs`, `TokenCounterTests.cs`.
- Modify: `ILSpy/ILSpy.csproj` (add the `ICSharpCode.ILSpy.AI` reference) and `ICSharpCode.ILSpyX/ICSharpCode.ILSpyX.csproj` (temporary bridge reference, see below).
- Modify: namespaces/usings in moved production/tests and all current callers.

**Interfaces:**
- Produce the same public contracts with assembly identity changed to `ICSharpCode.ILSpy.AI`.
- Do not change record fields, enum values, method names, cancellation behavior, streaming interfaces, or serialization behavior.

- [ ] Add bridge references before moving anything: `ILSpy/ILSpy.csproj` gains `ICSharpCode.ILSpy.AI` (desktop callers consume the moved contracts directly), and `ICSharpCode.ILSpyX.csproj` gains a temporary reference to the same assembly so not-yet-migrated ILSpyX files that consume these types (`AnalyzerContext`'s `AISelectionSnapshot` property, `AISecurityAnalyzer`, `AISearchStrategy`, `AISecurityAuditService`) still compile. None of the files in this batch carry MEF exports, so no `AppComposition` change is needed yet.
- [ ] Move each source file with history where possible (`git mv`), then change only its namespace and imports needed to compile in the new project.
- [ ] Update all production and test callers to import `ICSharpCode.ILSpy.AI` rather than the old ILSpyX namespace.
- [ ] Add focused tests proving `LLMRequest` defaults/validation, `ChatHistory` target handling, token counting, code-fence extraction, embeddings, and selection records are behaviorally identical.
- [ ] Run `dotnet test` for `AI.Tests` and run the desktop AI test filter to catch assembly-identity fallout.
- [ ] Search for remaining definitions or imports of these moved types under `ICSharpCode.ILSpyX` and remove only the legacy source copies after all callers compile.
- [ ] Commit: `refactor: move portable AI contracts and utilities`.

### Task 2.2: Migrate profile, selection, settings model, and credential storage

**Files:**
- Move: `AIProfile.cs`, `AIProviderCatalog.cs`, `AISelectionService.cs`, `AICredentialMigration.cs`, `SecureKeyStorage.cs`, `SecureKeyStorageBackends.cs`.
- Create from the AI-owned portions of `ICSharpCode.ILSpyX/Settings/AISettings.cs`: `ICSharpCode.ILSpy.AI/Settings/AISettingsModel.cs`.
- Create: `ILSpy/AI/AISettingsSection.cs` implementing `ICSharpCode.ILSpyX.Settings.ISettingsSection` and delegating to `AISettingsModel`.
- Modify: `ILSpy/SettingsService.cs`, `ILSpy/AppEnv/AppComposition.cs`, `ILSpy/App.axaml.cs`, `ILSpy/AI/AISelectionSettingsHost.cs`, `ILSpy/Options/AISettingsViewModel.cs`, all desktop callers, and affected tests.
- Split/move tests: portable model/profile/selection tests go to `ICSharpCode.ILSpy.AI.Tests`; XML section registration and desktop persistence tests remain in `ILSpy.Tests`.

**Interfaces:**
- Produce portable `AISettingsModel : INotifyPropertyChanged` with the existing profile collection and AI preference properties.
- Produce desktop `AISettingsSection : ISettingsSection` with `SectionName => "AISettings"`, `LoadFromXml(XElement)`, `SaveToXml()`, and a stable `Model` property returning the live `AISettingsModel` instance.
- Change `AISelectionHost.Settings` to return `AISettingsModel`; keep `PersistAsync` unchanged.
- Preserve profile XML schema, profile ordering, credential IDs, property-change behavior, and persistence callback semantics.

- [ ] Write characterization tests around the existing `AISettings` before splitting it: feed schema-2 and every supported legacy XML fixture into `LoadFromXml`, assert all resulting properties/profiles, call `SaveToXml`, and record the expected XML structure/order/default omission behavior.
- [ ] Extract `AISettingsModel` without any reference to `ISettingsSection`, `SettingsServiceBase`, `ICSharpCode.ILSpyX`, or desktop types. Keep profile state, defaults, validation, migration helpers, and notification methods in the model.
- [ ] Re-anchor the two remaining consumers of the section type inside the portable module: `AICredentialMigration.EnsureMigratedAsync(AISettings, ...)` and `AISelectionService`'s internal test constructor both switch from `ICSharpCode.ILSpyX.Settings.AISettings` to `AISettingsModel`.
- [ ] Register `ICSharpCode.ILSpy.AI` in `AppComposition.RegisterPluginResolver`'s assembly list (`ILSpy/AppEnv/AppComposition.cs`; the list currently contains exactly `typeof(IAnalyzer).Assembly` plus the app assembly). This task moves the first MEF exports out of ILSpyX (`[Export] [Shared] AISelectionService`, `[Export] [Shared] SecureKeyStorage` and `SecureKeyStorageUnavailableException`), and the container does not scan project references. The desktop and the `ILSpy.Tests` harness (`TestApp.axaml.cs`, `ResetAppStateAttribute`) share this code path, so one registration covers both.
- [ ] Update `ILSpy/App.axaml.cs` by searching for the type name, not just using directives: it resolves `ICSharpCode.ILSpyX.AI.AISelectionService` fully-qualified (around line 90, inside the credential-migration startup block) with no `using` statement.
- [ ] Implement `AISettingsSection` in `ILSpy` as the single ILSpyX persistence adapter. It owns `SectionName`, translates XML to/from the portable model using the migrated serializer, forwards model property changes as needed by `SettingsServiceBase`, and never stores API key material in XML.
- [ ] Update `SettingsService` so its public AI settings access returns the live `AISettingsModel` (and privately retains/registers the `AISettingsSection` instance required by generic settings persistence). Do not change non-AI settings registration.
- [ ] Update `AISelectionHost.Settings`, `AISelectionSettingsHost`, and options UI to use `AISettingsModel`. `AISelectionSettingsHost` remains in `ILSpy` and continues to call `SettingsService.Save()`.
- [ ] Keep `SecureKeyStorage` public and preserve internal backend interfaces, P/Invokes, macOS platform attributes, Linux `secret-tool` behavior, file locations, exception types, and availability results.
- [ ] Keep `AISelectionService` MEF export/shared lifetime behavior unchanged. Its public host constructor must continue to receive `AISelectionHost`; do not expose its internal test constructor.
- [ ] Move profile/model/selection tests to `AI.Tests`. The existing characterization suites are `ICSharpCode.ILSpyX.Tests/Settings/AISettingsTests.cs` and `AISettingsProfilesTests.cs`: port the model-level cases (defaults, `RepairProfiles`, legacy migration semantics) to `AI.Tests`, and port the XML section/`SaveToXml` round-trip cases to `ILSpy.Tests` against the `AISettingsSection` adapter, because they cross the ILSpyX `ISettingsSection` host contract.
- [ ] Add regression tests for provider profile validation, profile duplication/movement/deletion fallback, persisted mutation callback behavior, unavailable credential backend, and platform-gated secure-store smoke paths.
- [ ] Run `AI.Tests`, desktop settings/options tests, and platform-specific secure-storage tests according to their existing gates.
- [ ] Delete `ICSharpCode.ILSpyX/Settings/AISettings.cs` only after the model, adapter, settings service, selection host, and all tests compile and pass.
- [ ] Commit: `refactor: extract AI settings model and desktop adapter`.

### Task 2.3: Migrate provider factory and transports

**Files:**
- Move: `AIProviderFactory.cs`, `Providers/OpenAIProvider.cs`, `Providers/AnthropicProvider.cs`.
- Move tests: `AIProviderFactorySnapshotTests.cs`, `Providers/OpenAIProviderTests.cs`, `Providers/AnthropicProviderTests.cs`.
- Modify: all provider consumers in `ILSpy`, the decompiler AI module, and tests.

**Interfaces:**
- Preserve `IAIProviderFactory.Create(AISelectionSnapshot)`, provider construction, `AIConfigurationException`, SSE stream behavior, endpoint validation, and `TestConnectionAsync` semantics.

- [ ] Preserve the provider factory MEF attributes and its public composition constructor.
- [ ] Preserve the internal injectable constructor only for the matching `AI.Tests` assembly through a narrow `InternalsVisibleTo` entry.
- [ ] Keep OpenAI-compatible endpoint support for OpenAI, Ollama, and custom providers, including loopback-only restriction for non-TLS HTTP endpoints.
- [ ] Keep Anthropic endpoint/model/API key validation and event parsing unchanged.
- [ ] Confirm that no vendor SDK package is added; the implementation must continue using `HttpClient`, JSON, and streaming HTTP events.
- [ ] Run moved provider tests plus an integration-level desktop test that resolves `IAIProviderFactory` from the existing composition container.
- [ ] Commit: `refactor: move AI provider transports`.

### Task 2.4: Migrate prompt assets and generated fallback

**Files:**
- Move directory: `ICSharpCode.ILSpyX/AI/prompts/` to `ICSharpCode.ILSpy.AI/AI/prompts/`.
- Move: `AIPromptMetadata.cs`, `AIPromptProvider.cs`.
- Generate in destination: `EmbeddedPrompts.g.cs`.
- Move tests: `AIPromptProviderTests.cs`, `PromptFileGeneratorTests.cs`.
- Modify: `ICSharpCode.ILSpy.AI.csproj`, `ICSharpCode.ILSpyX.csproj`, test project references, and every prompt consumer namespace.

**Interfaces:**
- Preserve the same eight prompt IDs, YAML metadata semantics, provider/model variation resolution, disk-first loading, invalid-file fallback, and embedded fallback content.

- [ ] Move prompt content copy rules, prompt embedder project reference with `ReferenceOutputAssembly="false"`, input/source item groups, properties, and `GenerateEmbeddedPrompts` target to `AI.csproj` with destination-relative paths.
- [ ] Update `BuildTools/PromptEmbedder` and `PromptFileGeneratorTests`: the generator currently hard-codes `namespace ICSharpCode.ILSpyX.AI` in `PromptFileGenerator.Generate` (`BuildTools/PromptEmbedder/Program.cs`, around line 69) and its CLI accepts only `<input> <output> [--check]`. Add the target namespace as an explicit CLI argument and emit `ICSharpCode.ILSpy.AI` for this project. PromptEmbedder is a dependency-free net10.0 tool with no `packages.lock.json`, excluded from both solution filters (its self-build target handles that); do not add it to the filters.
- [ ] Move the real `PromptEmbedder` ProjectReference currently held by `ICSharpCode.ILSpyX.Tests.csproj` (it exists for `PromptFileGeneratorTests`; ILSpyX itself uses only a `ReferenceOutputAssembly="false"` build-only reference) to `ICSharpCode.ILSpy.AI.Tests` together with the test, so generator tests exercise the parameterized tool directly.
- [ ] Ensure generated source is produced under `ICSharpCode.ILSpy.AI/AI/EmbeddedPrompts.g.cs` in namespace `ICSharpCode.ILSpy.AI`; do not maintain a second generated copy in ILSpyX.
- [ ] Remove AI prompt content items and generation target from `ICSharpCode.ILSpyX.csproj` only after the new project builds the generated file.
- [ ] Add test cases that load each prompt from disk, validate all IDs, test missing/invalid external files falling back to embedded content, and test provider/model variation selection.
- [ ] Build clean from a deleted `bin/obj` state to prove the generator runs before compilation.
- [ ] Inspect the final `ILSpy/bin/.../AI/prompts` output after a desktop build; all eight prompt files plus `README.md` must be present.
- [ ] Commit: `build: move AI prompt assets and generator`.

### Task 2.5: Finish core caller/test migration and remove ILSpyX AI core

**Files:**
- Modify: every source/test reference to moved portable types.
- Modify: `ICSharpCode.ILSpyX/ICSharpCode.ILSpyX.csproj`, `ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj`.
- Delete only after migration: old portable source files, `ICSharpCode.ILSpyX/AI/prompts`, and old generated prompt source.

- [ ] Use a whole-repository type search for every moved type and namespace. Update project references explicitly; do not depend on accidental transitive references.
- [ ] Remove `YamlDotNet` and `Microsoft.Extensions.Logging.Abstractions` from ILSpyX only if no non-AI ILSpyX source still references them. Verify each removal with a build; retain any dependency still needed by unrelated ILSpyX functionality. Expect `System.Composition.AttributedModel` to REMAIN in ILSpyX regardless: `ExportAnalyzerAttribute` derives from `ExportAttribute` and the non-AI analyzer/settings exports are MEF parts. The temporary ILSpyX → AI bridge reference from Task 2.1 stays in place for now; Task 3.4 removes it.
- [ ] Remove portable AI tests from `ICSharpCode.ILSpyX.Tests`; remove its PromptEmbedder project reference only after no remaining test relies on it.
- [ ] Verify `ICSharpCode.ILSpyX` builds without any `AI/` source, generated prompts, or prompt content output.
- [ ] Commit: `refactor: remove migrated AI core from ILSpyX`.

**Phase 2 exit criteria:** `AI` owns all portable AI functionality and its tests; `ILSpyX` contains none of those source files/assets; settings, credential storage, provider composition, prompt fallback, and desktop options behavior are unchanged.

---

## Phase 3: Migrate Decompiler-Aware AI Module

**Purpose:** Move every AI implementation that directly consumes decompiler entities into a module with an explicit decompiler dependency, without changing decompiler core behavior.

### Task 3.1: Migrate context DTOs and context builder

**Files:**
- Move to `ICSharpCode.ILSpy.AI.Decompiler/AI/`: `DecompilationContext.cs`, `ContextBuilder.cs`.
- Move tests: `ContextBuilderTests.cs`.
- Modify: `ILSpy/ILSpy.csproj` and `ICSharpCode.ILSpyX/ICSharpCode.ILSpyX.csproj` — both gain a reference to `ICSharpCode.ILSpy.AI.Decompiler` (the app consumes the moved types; ILSpyX's not-yet-moved explanation/rename/security code still compiles against `ContextBuilder` through the temporary bridge).
- Modify all explanation, rename, security, search, and desktop context callers.

**Interfaces:**
- Preserve `DecompilationContext` serialized field names/order/optional sections and `ContextBuilder` public construction/method contracts.
- Preserve token budget truncation, unavailable-section reporting, C# source, attributes/interfaces, literals, callers/callees, and optional IL output.

- [ ] Move source with history and change namespaces/usings only as needed for project ownership.
- [ ] Add the project references first: `AI.Decompiler` already references `AI` and `ICSharpCode.Decompiler` from Task 1.2; add the two bridge references listed above before moving the files.
- [ ] Re-implement same-module caller discovery inside `AI.Decompiler` in this task — `ContextBuilder` cannot compile there without it: `ContextBuilder.GetCallers` calls `MethodUsedByAnalyzer.FindCallers` (internal, `ICSharpCode.ILSpyX/Analyzers/Builtin/MethodUsedByAnalyzer.cs`, around line 47), which also uses `AnalyzerHelpers.IsPossibleReferenceTo` (internal, `ICSharpCode.ILSpyX/Analyzers/AnalyzerHelpers.cs`). Both are unreachable from the new assembly. Copy `FindCallers`, `GetTypes`, `GetMethodBody`, `ScanMethodBody`, `IsSupportedOpCode`, and `IsSameMember` (~200 lines) plus `IsPossibleReferenceTo` (~22 lines), preserving ordering, limits, and metadata exception handling. Every other dependency (`InheritanceHelper`, `ILParser`, `MetadataTokenHelpers`, `GenericContext`) comes from `ICSharpCode.Decompiler`. Callee discovery needs no work: `ScanMethodReferences` is already internal to `ContextBuilder` and moves with the file.
- [ ] Keep internal helpers internal. Do not make `ScanMethodReferences` or token-fitting helpers public to simplify desktop access. `ContextBuilderTests` exercises internal members (`TryFitCode` and friends), so this task relies on the Task 1.3 `InternalsVisibleTo` entry for `AI.Decompiler.Tests`.
- [ ] Run the migrated context tests against representative methods/types and assert exact or normalized outputs where tests already define behavior.
- [ ] Add regression assertions for context-budget clipping, call graph/IL opt-in flags, and unavailable context sections.
- [ ] Commit: `refactor: move AI decompilation context builder`.

### Task 3.2: Migrate explanation and rename services

**Files:**
- Move: `AIExplanationService.cs`, `RenameSuggester.cs`, `BatchRenameSuggester.cs`.
- Move tests: `AIExplanationServiceTests.cs`, `RenameSuggesterTests.cs`, `BatchRenameSuggesterTests.cs`.
- Modify desktop callers: `ILSpy/AI/AIChatFeatureCommands.cs`, `ExplainContextMenuEntry.cs`, `RenameAssistantContextMenuEntry.cs`, `GenerateDocsContextMenuEntry.cs`, `AssemblySummaryContextMenuEntry.cs`, related dialog/viewmodel files, and corresponding desktop tests.

**Interfaces:**
- Preserve explanation streaming/error classification, rename prompt/schema parsing, obfuscated-name heuristic, ranked suggestions, naming hint behavior, cancellation, and batch ordering/progress semantics.

- [ ] Keep `AIExplanationService` dependent on `IEntity`/`CSharpDecompiler`; do not create a new decompiler abstraction in this migration.
- [ ] Keep `RenameSuggester.IsLikelyObfuscated` behavior and non-destructive suggestion behavior unchanged.
- [ ] Preserve `BatchRenameSuggester` limits, dependency order, context calculation, cancellation, and progress callbacks.
- [ ] Update desktop code to call the moved types; do not move the command dispatch, selection resolution, dialogs, UI thread handling, or annotation application out of `ILSpy`.
- [ ] Add focused moved tests for stream success/error/cancellation, bounded JSON parsing, rejected non-obfuscated names, malformed responses, and batch dependency sequencing.
- [ ] Run desktop `/explain` and `/rename` pipeline tests after imports/project references change.
- [ ] Commit: `refactor: move AI explanation and rename services`.

### Task 3.3: Migrate annotations, AI search, and security analysis

**Files:**
- Move: `RenameAnnotations.cs`, `RenameAnnotationTransform.cs`, `AISearchStrategy.cs`, `SemanticSearchStrategy.cs`, `AISecurityFinding.cs`, `AISecurityAnalyzer.cs`, `AISecurityAuditService.cs`.
- Move tests: `Annotations/RenameAnnotationManagerTests.cs`, `Analyzers/AISecurityAnalyzerTests.cs`, and decompiler-side batch rename tests. AI search tests do not exist yet and are written new (below).
- Create: `ILSpy/Analyzers/AISecurityAnalyzerAdapter.cs` — the desktop MEF adapter described below.
- Modify: `ICSharpCode.ILSpyX/Analyzers/AnalyzerContext.cs` (remove AI properties), `ILSpy/Search/SearchPaneModel.cs`, `ILSpy/Analyzers/AnalyzerSearchTreeNode.cs`, `ILSpy/Analyzers/AISecurityFindingTreeNode.cs`, `ILSpy/Languages/CSharpLanguage.cs`, `ILSpy/AI/AISecurityAuditContextMenuEntry.cs`, `ILSpy/AI/AIChatFeatureCommands.cs`, and their tests.

**Interfaces:**
- Security analysis splits into core + adapter. The core (finding JSON parsing, confidence filtering against `MinimumFindingConfidence = 0.70` — a `double` in `[0,1]`, not an integer percentage — audit cap `MaximumTypesPerAudit = 50`, progress/cancellation) moves to `AI.Decompiler` as a plain service with no `IAnalyzer`/`AnalyzerContext`/`[ExportAnalyzer]` dependency (those are ILSpyX contracts the module must not reference), keeping the `AnalyzeSelectedTypeAsync` entry point used by `ILSpy/AI/AIChatFeatureCommands.cs`.
- Preserve search result entity resolution, audit caps/progress/cancellation, and rename annotation transform behavior unchanged.

- [ ] Keep the "Security Risks (AI)" tree analyzer working with a desktop adapter: create an `ILSpy`-owned `AISecurityAnalyzerAdapter : IAnalyzer` carrying `[ExportAnalyzer(Header = "Security Risks (AI)", Order = 1000)] [Shared]`, importing `AISelectionService`/`IAIProviderFactory` via `[ImportingConstructor]`, and delegating to the moved core service. The app assembly is already composed, so `AnalyzerRegistry`'s `[ImportMany("Analyzer")]` discovers the adapter exactly as it discovered the ILSpyX export — same header, same order, same results. Do not simply drop the export: that would silently remove a working feature.
- [ ] Strip AI members from `AnalyzerContext` (`AISelectionSnapshot`, `IAIProviderFactory`, `IProgress<AISecurityAuditProgress> AIProgress`, and the AI usings) once the adapter exists, and delete the AI resolution special-case in `AnalyzerSearchTreeNode.RunAnalyzer` (it unconditionally resolves `AISelectionService` and populates those properties; the adapter now supplies its own state). Keep the `AISecurityFinding` dispatch in `WrapResult`. Do not modify non-AI analyzer contracts.
- [ ] Update `ILSpy.Tests/Analyzers/Library/ExportAnalyzerAttributeTests.cs`: `ExportAnalyzerAttribute.GetAnnotatedAnalyzers()` reflects over the ILSpyX assembly only, so its expected analyzer inventory changes when `AISecurityAnalyzer` leaves. Assert the adapter's presence through the real composition container instead.
- [ ] Keep the type name `RenameAnnotationTransform` unchanged: `ILSpy.Tests/Views/DebugStepsTests.cs` asserts the string `"RenameAnnotationTransform"` in debug steps output. Registration in `CSharpLanguage` is manual (not MEF); update its using only.
- [ ] Write (do not expect to move) AI search regression tests: no tests exist for `AISearchStrategy`/`SemanticSearchStrategy` today. The strategies are static classes with no base class and no MEF attributes; `SearchPaneModel.StartSpecialSearch` calls them directly, and the `SearchMode.AI`/`SearchMode.Semantic` enum members stay in ILSpyX.
- [ ] Keep analyzer and search logic in the decompiler module because they consume `MetadataFile`, type-system entities, analyzer APIs, and decompilation output.
- [ ] Keep `AISecurityFindingTreeNode` in `ILSpy`; only its model/service reference changes.
- [ ] Keep `SearchPaneModel` and `AnalyzerSearchTreeNode` in `ILSpy`; maintain their existing `AppComposition` resolution behavior until a separate composition redesign is requested.
- [ ] Add regression tests for security confidence boundary values (`0.69` rejected, `0.70` retained, values above `1` rejected), malformed LLM JSON, cancellation, the configured audit cap (`MaximumTypesPerAudit = 50`), and stable result mapping to entities.
- [ ] Add regression tests for semantic/local search ranking and mapping results back to decompiler entities.
- [ ] Add regression tests for rename annotations continuing to render and not mutating original metadata.
- [ ] Commit: `refactor: move AI search security and annotations`.

### Task 3.4: Remove decompiler AI source from ILSpyX

**Files:**
- Modify: `ICSharpCode.ILSpyX.csproj`, `ICSharpCode.ILSpyX.Tests.csproj`.
- Delete after migration: moved decompiler-aware AI source/test files from ILSpyX locations.
- Modify: all direct consumers that still import legacy namespaces.

- [ ] Search the repository for each moved source type and old AI namespace; update all remaining imports and project references.
- [ ] Remove decompiler-specific AI tests from `ICSharpCode.ILSpyX.Tests` after they are passing in `AI.Decompiler.Tests`.
- [ ] Remove package references from ILSpyX only when no unrelated source needs them. In particular, verify `Mono.Cecil`, `Markdig`, `LZ4`, metadata packages, and MEF independently instead of deleting them as a group.
- [ ] Remove the temporary bridge references added in Tasks 2.1 and 3.1: `ICSharpCode.ILSpyX.csproj` must no longer reference `ICSharpCode.ILSpy.AI` or `ICSharpCode.ILSpy.AI.Decompiler`. This is possible only after Task 3.3 removed the AI members from `AnalyzerContext`; verify with a clean build.
- [ ] Build `ICSharpCode.ILSpyX` and inspect its compile items to prove it no longer compiles AI code.
- [ ] Commit: `refactor: remove migrated decompiler AI from ILSpyX`.

**Phase 3 exit criteria:** The decompiler-aware module owns every AI type that references decompiler entities. `ILSpyX` does not compile AI context, explanation, rename, search, security, or annotation code.

---

## Phase 4: Migrate and Stabilize Desktop Callers Without Extracting UI

**Purpose:** Complete the caller migration while retaining all Avalonia and command behavior in the existing desktop assembly.

### Task 4.1: Update desktop project references and composition

**Files:**
- Modify: `ILSpy/ILSpy.csproj`
- Modify: `ILSpy/AI/AISelectionSettingsHost.cs`
- Modify: all `ILSpy/AI/*.cs`, `ILSpy/Options/AISettingsViewModel.cs`, `ILSpy/Search/SearchPaneModel.cs`, `ILSpy/Analyzers/*.cs`, `ILSpy/Languages/CSharpLanguage.cs`, and `ILSpy/ViewLocator.cs` that import migrated types.

**Interfaces:**
- `ILSpy` references `ICSharpCode.ILSpy.AI` and `ICSharpCode.ILSpy.AI.Decompiler` directly.
- Existing MEF composition resolves `AISelectionService`, `IAIProviderFactory`, analyzers, and search strategies from the new assemblies.

- [ ] Verify (not add) the desktop project references: `ILSpy.csproj` gained `ICSharpCode.ILSpy.AI` in Task 2.1 and `ICSharpCode.ILSpy.AI.Decompiler` in Task 3.1. Do not remove the existing ILSpyX or Decompiler project references needed by non-AI desktop code.
- [ ] Update only namespaces/usings and type references in desktop classes. Do not relocate, rename, redesign, or change Avalonia markup.
- [ ] Update `ILSpy/App.axaml.cs` credential-migration startup call and every `SettingsService.AISettings` consumer to use the portable `AISettingsModel` through the retained ILSpy adapter.
- [ ] Confirm composition resolves the moved exports end-to-end. Fact: `AppComposition` catalogs exactly ILSpyX + the app assembly + `*.Plugin.dll` and does not scan project references; `ICSharpCode.ILSpy.AI` was registered in `RegisterPluginResolver` during Task 2.2. `ICSharpCode.ILSpy.AI.Decompiler` deliberately carries no MEF exports (search strategies are static; the analyzer adapter lives in the app assembly), so it needs no registration. Do not create a parallel container.
- [ ] Add or update a composition test proving imports of `AISelectionService` and `IAIProviderFactory` resolve from the new assembly identity, exercising the same `AppComposition` path used by `TestApp`/`ResetAppStateAttribute`. Validate AI search via `SearchPaneModel.StartSpecialSearch`'s direct static calls and security via the `AISecurityAnalyzerAdapter` export; the adapter replaces the former ILSpyX generic-analyzer export one-for-one.
- [ ] Commit: `refactor: migrate ILSpy desktop AI callers`.

### Task 4.2: Update desktop tests and preserve UI contracts

**Files:**
- Modify: `ILSpy.Tests/ILSpy.Tests.csproj`
- Modify tests under `ILSpy.Tests/AI/` and `ILSpy.Tests/Options/AISettingsViewModelTests.cs`.

- [ ] Keep all desktop AI tests in `ILSpy.Tests`; they validate desktop behavior, not portable module internals.
- [ ] Add direct test project references to the two new assemblies only when the test compiles against their public contracts without receiving them through `ILSpy`. Prefer the existing `ILSpy` project reference for end-to-end desktop behavior.
- [ ] Update namespaces/usings, mocks, and test setup to use the new assembly contracts without changing intended test behavior.
- [ ] Run the complete desktop AI test set, including chat pane, output pane, feature command, assembly summary, batch rename UI model, markdown editor, editor scroll state, and AI settings tests.
- [ ] Commit: `test: migrate desktop AI tests to extracted assemblies`.

### Task 4.3: Perform manual desktop smoke verification

**Files:**
- Create: `doc/plans/ai-assembly-extraction-manual-verification.md`

- [ ] Build and launch the desktop application in a configuration appropriate to the current platform.
- [ ] Open the AI settings page; create, edit, reorder, select, and delete a profile. Restart and verify persistence. Confirm API keys are not present in settings XML.
- [ ] Test provider connection failure and success using a non-production test endpoint or controlled fake server. Confirm invalid non-loopback plain HTTP remains rejected.
- [ ] Verify the chat pane loads, sends a normal message, preserves conversation behavior, and renders streamed text without UI exceptions.
- [ ] Verify `/explain`, `/rename`, `/summary`, and `/audit` command routing reaches the same desktop output surfaces as before.
- [ ] Verify Explain, Rename, Batch Rename, Generate Docs, Assembly Summary, Search, and Security Audit context/menu entry points are visible where they were before extraction.
- [ ] Verify a rename suggestion creates an annotation/overlay rather than mutating metadata.
- [ ] Verify AI prompts are present next to the desktop output and external prompt changes are observed after restart/reload according to current provider semantics.
- [ ] Record platform, configuration, exact actions, outcome, logs/exceptions, and any known test-provider limitation.
- [ ] Commit: `test: record AI assembly extraction desktop smoke results`.

**Phase 4 exit criteria:** Desktop UI remains in `ILSpy`, all desktop AI features resolve the extracted modules correctly, and no UI extraction work has been performed.

---

## Phase 5: Packaging, CI, Cleanup, and Final Verification

**Purpose:** Make the extraction maintainable and ensure package outputs, solution filters, lock files, and CI accurately represent the final architecture.

### Task 5.1: Validate package boundaries and outputs

**Files:**
- Modify as required: both new `.csproj` files, `Directory.Packages.props`, package readmes, package lock files.
- Inspect: produced `.nupkg` files and desktop publish output.

- [ ] Pack `ICSharpCode.ILSpy.AI` and inspect its `.nuspec`/dependency graph. Confirm it does not depend on `ICSharpCode.Decompiler`, `ICSharpCode.ILSpyX`, Avalonia, Dock, or desktop packages.
- [ ] Pack `ICSharpCode.ILSpy.AI.Decompiler` and confirm it depends on `AI` and `ICSharpCode.Decompiler`, but not `ILSpyX` or desktop packages.
- [ ] Confirm prompt content is included/copied with the `AI` package according to the chosen package convention and that generated fallback remains compiled into its assembly.
- [ ] Review package reference visibility. Mark build-only references such as SourceLink/SBOM private exactly as existing projects do; do not hide runtime dependencies required by package consumers.
- [ ] Confirm new package IDs, authorship/license/repository metadata, tags, readmes, and version propagation follow repository standards.
- [ ] Commit: `build: finalize AI package boundaries`.

### Task 5.2: Update CI and solution verification

**Files:**
- Modify: `.github/workflows/build-ilspy.yml`, solution/filter files, affected lock files.

- [ ] Ensure CI restores/builds both solution filters containing the new projects.
- [ ] Ensure CI executes `AI.Tests` and `AI.Decompiler.Tests` in the relevant OS matrix and continues running desktop UI tests separately.
- [ ] Preserve existing platform conditions for macOS/Linux/Windows and secure-storage smoke tests; do not claim a native platform test is cross-platform if it is not.
- [ ] Refresh lock files only through the repository's expected dependency-update workflow when package graph changes demand it.
- [ ] Confirm no stale CI artifact step assumes AI content is emitted from `ICSharpCode.ILSpyX`.
- [ ] Commit: `ci: verify extracted AI assemblies`.

### Task 5.3: Final regression and source-boundary audit

**Files:**
- Modify: `CHANGELOG.md` only if project policy requires architecture changes to be documented for users. Do not change feature behavior documentation otherwise.
- Create: `doc/plans/ai-assembly-extraction-validation.md`

- [ ] Restore all relevant projects with locked mode.
  Run: `dotnet restore ILSpy.XPlat.slnf --locked-mode`
  Run: `dotnet restore ILSpy.Desktop.slnf --locked-mode`
  Expected: success and no unexpected lock-file changes.
- [ ] Build both filters in Release.
  Run: `dotnet build ILSpy.XPlat.slnf --configuration Release --no-restore`
  Run: `dotnet build ILSpy.Desktop.slnf --configuration Release --no-restore`
  Expected: success with zero warnings.
- [ ] Run all new module tests.
  Run: `dotnet test --project ICSharpCode.ILSpy.AI.Tests/ICSharpCode.ILSpy.AI.Tests.csproj --configuration Release --no-build`
  Run: `dotnet test --project ICSharpCode.ILSpy.AI.Decompiler.Tests/ICSharpCode.ILSpy.AI.Decompiler.Tests.csproj --configuration Release --no-build`
  Expected: pass.
- [ ] Run desktop tests.
  Run: `dotnet test --project ILSpy.Tests/ILSpy.Tests.csproj --configuration Release --no-build`
  Expected: pass, subject only to documented pre-existing platform gates.
- [ ] Run remaining ILSpyX tests.
  Run: `dotnet test --project ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --configuration Release --no-build`
  Expected: pass with no migrated AI tests remaining.
- [ ] Whole-repository audit: verify no `ICSharpCode.ILSpyX/AI` source remains, no code imports legacy AI namespaces, no new AI project references `ILSpyX`, and no Avalonia UI file was moved.
- [ ] Inspect git diff to ensure no non-AI implementation files were refactored gratuitously and no unimplemented plan feature was added.
- [ ] Record commands/results/platform caveats in the validation document.
- [ ] Commit: `docs: record AI extraction validation`.

**Phase 5 exit criteria:** All tests and builds pass under their supported platform conditions; package boundaries match the target graph; `ILSpyX` is free of scoped AI implementation; and the desktop UI remains in `ILSpy`.

---

## Migration Order and Non-Negotiable Sequencing

1. Add empty projects/tests/CI first.
2. Move BCL-only contracts and utilities, adding the `ILSpy` → `AI` and the temporary `ILSpyX` → `AI` bridge references before the first file moves.
3. Move profiles/settings/credential storage and provider transport, registering `ICSharpCode.ILSpy.AI` in `AppComposition` when the first MEF exports move.
4. Move prompts and generator only after the portable project compiles.
5. Remove portable AI source from ILSpyX.
6. Move decompiler context (re-implementing internal caller discovery locally and adding the `AI.Decompiler` bridge references), then explanation/rename, then annotations/search/security with the desktop analyzer adapter and `AnalyzerContext` cleanup.
7. Remove decompiler-aware AI source from ILSpyX and drop both temporary bridge references.
8. Update desktop references/callers/tests, but do not move UI files.
9. Validate packages, CI, full builds, automated tests, and manual desktop smoke behavior.

Do not attempt to move all source files in one change. The executable must remain buildable after every completed phase.

## Known Risks and Required Responses

| Risk | Required response |
|---|---|
| `AISettings` depends on ILSpyX's `ISettingsSection` | Resolved by the Task 2.2 design: split into a portable `AISettingsModel` (pure `INotifyPropertyChanged`) plus an `ILSpy`-owned `AISettingsSection : ISettingsSection` adapter delegating to the model. Never reference ILSpyX from the AI project and never duplicate types. |
| MEF exports vanish when types leave ILSpyX | `AppComposition` catalogs only ILSpyX, the app assembly, and `*.Plugin.dll`; it never scans project references, and failures are lazy. Register `ICSharpCode.ILSpy.AI` in `RegisterPluginResolver` in the same task that moves the first export (Task 2.2). |
| ILSpyX cannot compile mid-migration | Not-yet-moved ILSpyX AI code consumes already-moved types. Add temporary ILSpyX → AI / AI.Decompiler bridge references when the first types move (Tasks 2.1 and 3.1) and remove them in Task 3.4, after `AnalyzerContext` is AI-free. |
| Caller discovery is internal to ILSpyX | `ContextBuilder` needs `MethodUsedByAnalyzer.FindCallers` and `AnalyzerHelpers.IsPossibleReferenceTo` (both internal). Copy them into `AI.Decompiler` in Task 3.1, preserving ordering, limits, and exception handling. |
| Analyzer tree feature silently disappears | Dropping `AISecurityAnalyzer`'s `[ExportAnalyzer]` would remove "Security Risks (AI)" from the analyzer tree. Keep it via the `ILSpy`-owned `AISecurityAnalyzerAdapter` (Task 3.3) and update `ExportAnalyzerAttributeTests`. |
| Prompt files disappear from desktop output | Fix the `AI.csproj` content-copy and generator paths; verify publish output before deleting old ILSpyX content rules. |
| MEF cannot resolve services after assembly move | Preserve export/shared attributes, make ILSpy reference the new assemblies directly, and use existing application composition conventions. Do not add a second service container. |
| Secure key storage changes platform behavior | Move facade and all internal platform backends together; preserve P/Invoke declarations, locations, and Linux process behavior. |
| New package drags in decompiler/UI dependencies | Inspect `dotnet pack` output. Keep provider/configuration code in `AI`; move only decompiler-bound code to `AI.Decompiler`; never reference `ILSpy` from either. |
| Tests depended on former internal access | Add friend access only from each new product assembly to its matching test assembly, signed with the existing key. Prefer public behavioral tests for desktop callers. |
| Source copied rather than moved leaves duplicate types | Migrate all callers first, then delete the old source in the same phase. Search for duplicate fully qualified type names before committing. |
| Desktop behavior changes while UI remains in place | Revert to namespace/project-reference-only changes in desktop files. Any UI design or lifecycle change is out of scope. |

## Acceptance Checklist

- [ ] `ICSharpCode.ILSpy.AI` exists, builds, packs, and has no decompiler or desktop dependency.
- [ ] `ICSharpCode.ILSpy.AI.Decompiler` exists, builds, packs, references only `AI` and `ICSharpCode.Decompiler` among product projects, and owns all decompiler-bound AI functionality.
- [ ] All scoped portable and decompiler-aware AI source has been removed from `ICSharpCode.ILSpyX`.
- [ ] All production callers and tests use the new contracts/assemblies.
- [ ] `ILSpy` retains all Avalonia UI, desktop command, menu, dialog, options, and host adapter files.
- [ ] `ICSharpCode.ILSpyX` no longer references either new AI assembly (the temporary bridge references from Tasks 2.1/3.1 were removed in Task 3.4).
- [ ] `AppComposition` catalogs `ICSharpCode.ILSpy.AI` (registered in Task 2.2), and the "Security Risks (AI)" tree analyzer still works through the desktop `AISecurityAnalyzerAdapter`.
- [ ] The existing provider protocols, settings migration, secure storage, prompts, chat commands, rename annotations, search, and security audit behavior are preserved.
- [ ] New projects, tests, solution filters, lock files, package metadata, and CI entries are complete.
- [ ] `ILSpy.XPlat.slnf`, `ILSpy.Desktop.slnf`, new module tests, remaining ILSpyX tests, and desktop tests pass within supported platform constraints.
- [ ] Manual desktop verification confirms the actual AI workflows still work after the assembly split.
