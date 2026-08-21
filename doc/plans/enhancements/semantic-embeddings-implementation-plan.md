# Remote Embedding-Backed Semantic Search — Implementation Plan

> **For agentic workers:** Use superpowers:subagent-driven-development (recommended) or
> superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox
> (`- [ ]`) syntax for tracking.

Status: Proposed — awaiting review
Created: 2026-08-21
Audience: implementer working in this repository with no assumed knowledge of the AI roadmap
Source: gap analysis — "Remote embedding-backed semantic search" against roadmap Phase 4.6
Related: `doc/plans/enhancements/ai-search-architecture-decision.md`, `doc/plans/AI-INTEGRATION-ROADMAP.md` (§4.6), `doc/plans/enhancements/ai-gap-closure-implementation-plan.md` (§3 decision 4, §13)

**Goal:** Ship the roadmap Phase 4.6 embedding-based semantic search: provider-backed embeddings (remote or local via Ollama), SQLite persistence with assembly-change invalidation, opt-in background indexing, and cosine top-k search — with the existing hash-vector heuristic retained as automatic fallback.

**Architecture:** A new `IEmbeddingProvider` (OpenAI-compatible `/embeddings` wire protocol, one client serves OpenAI/Ollama/custom) is created from the same immutable `AISelectionSnapshot` the chat path uses. A background `SemanticIndexService` decompiles candidate types/methods, batches them to the provider, and persists vectors in a per-user SQLite database keyed by assembly MVID. `SemanticSearchStrategy.SearchAsync` embeds the query, cosine-ranks stored vectors, resolves tokens back to `IEntity`, and falls back to the local heuristic whenever the remote index is unavailable, stale, or disabled. AI/semantic modes remain the documented `SearchPaneModel` special case; no change to the synchronous strategy registry.

**Tech Stack:** .NET 10, `Microsoft.Data.Sqlite` (new, central-versioned), `System.Text.Json`, existing MEF composition, NUnit.

## Global Constraints

- **Snapshot discipline:** every provider request resolves its target through one `AISelectionSnapshot` captured at the operation boundary (`AISelectionService.ResolveSnapshotAsync`). No request path may re-read mutable `AISettings` after start. This is locked by `ai-gap-closure-implementation-plan.md` §3 and must not regress.
- **Opt-in by default:** `SemanticIndexingEnabled` defaults to `false`. Bulk indexing sends decompiled code for every loaded assembly to the provider; it must never start without (a) `PrivacyConsentAccepted == true`, (b) the explicit toggle, (c) a ready snapshot whose provider supports embeddings.
- **Loopback-only HTTP:** non-HTTPS endpoints are rejected except loopback, same rule as `OpenAIProvider` ctor (`OpenAIProvider.cs:43`).
- **No thresholds pulled from thin air:** query ranking returns plain top-k (roadmap §4.6); no cosine cutoff constant is introduced without evaluation data.
- **Named constants, never magic numbers:** `EmbeddingBatchSize`, `MaxConcurrentEmbeddingRequests`, `MaxEntitiesPerAssembly`, `MaxEmbeddingTextLength`, `IndexDebounceDelay` — each declared once and tested.
- **UI thread safety:** all bound-property writes post-`ConfigureAwait(false)` go through `Dispatcher.UIThread` (pattern established in `SearchPaneModel.StartSpecialSearch`).
- **Scope guard:** AI/semantic modes stay a `SearchPaneModel` special case. Do not register them into the synchronous strategy registry (decision doc line 24-25).
- **Tests:** new tests live in `ICSharpCode.ILSpyX.Tests/AI/` and `ILSpy.Tests/Search/` following existing conventions; no network access in tests — all provider tests use fakes/handlers.
- **Build commands:** use the existing solution filter and test projects: `rtk dotnet build ILSpy.Desktop.slnf --no-restore --verbosity minimal`; focused suites per phase below. If `rtk` is unavailable, plain `dotnet` with identical arguments.

---

## 1. Objective and definition of done

Close the Phase 4.6 gap: today `SemanticSearchStrategy` (ICSharpCode.ILSpyX/Search/SemanticSearchStrategy.cs:14) rebuilds a dependency-free 128-dim hash-vector `EmbeddingStore` per query over name text only — no provider, no persistence, no decompiled bodies, recomputed every keystroke-session.

Definition of done:

- Roadmap §4.6 acceptance criteria all hold: background indexing on assembly load (opt-in), decompile-and-embed of candidate entities, SQLite storage of float vectors, query embedding + cosine top-k, cache invalidated only when assembly/model changes, and users who don't want embedding API cost can skip everything (toggle off → current heuristic, zero requests).
- One snapshot per indexing run and per query; provider changes mid-run never affect in-flight work.
- Search pane: single `SearchMode.Semantic` entry; engine (remote index vs local heuristic) is chosen automatically and surfaced in a one-line status; hard failures fall back to the existing literal-search path.
- Settings: consent-gated enable toggle, embedding model editor, per-assembly index list with delete, delete-all.
- Storage, privacy, model, and evaluation designs (the four items the architecture decision doc requires before un-descoping) are implemented as specified in §3 and recorded in the decision doc.
- All phase gates pass; decision doc and roadmap updated to reflect what ships.

## 2. Current state (verified)

| Concern | Current | Location |
|---|---|---|
| Similarity engine | `EmbeddingStore`: 128-dim token-hash bag, in-memory, cosine | `ICSharpCode.ILSpyX/AI/EmbeddingStore.cs` |
| Strategy | synchronous, rebuilds store per query, indexes `FullName + Name` of types+methods | `ICSharpCode.ILSpyX/Search/SemanticSearchStrategy.cs` |
| Pane integration | special-cased async path, no snapshot for semantic mode (AI mode only) | `ILSpy/Search/SearchPaneModel.cs:294-377` |
| Provider stack | `ILLMProvider` chat-only; `OpenAIProvider` (OpenAI-compatible, SSE), `AnthropicProvider`; factory validates snapshot; catalog `openai/anthropic/ollama/custom` | `ICSharpCode.ILSpyX/AI/*` |
| Settings | `AISettings` XML: profiles, `PrivacyConsentAccepted`, `MaxContextTokens`, `SendIL`, `SendCallGraph`; no embedding fields | `ICSharpCode.ILSpyX/Settings/AISettings.cs` |
| Persistence | none for search; chat history JSON next to assembly file; settings in `%APPDATA%/ICSharpCode/ILSpy.xml` | `ILSpy/App.axaml.cs:161` |
| SQLite | not referenced anywhere in the solution; central package management via `Directory.Packages.props` | — |
| Assembly events | `Util.MessageBus<Util.CurrentAssemblyListChangedEventArgs>` (already consumed by `SearchPaneModel` and `DockWorkspace`) | `ILSpy/Util/MessageBus.cs:72` |
| Entity enumeration | duplicated `GetCandidates` in `AISearchStrategy` and `SemanticSearchStrategy` | both strategy files |
| Token→entity | `MetadataModule.ResolveEntity(EntityHandle, GenericContext)` exists | `ICSharpCode.Decompiler/TypeSystem/MetadataModule.cs:790` |
| Assembly identity | `MetadataFile.Metadata.GetModuleDefinition().Mvid`, `LoadedAssembly.FileName`, `GetMetadataFileOrNull()` | `ICSharpCode.ILSpyX/LoadedAssembly.cs` |

## 3. Locked decisions (privacy, storage, model, evaluation)

The architecture decision doc requires a separate privacy, storage, model, and evaluation design before remote embeddings may be added. These four designs are locked here; implementers must not weaken them.

**D1 — Privacy.** Indexing sends decompiled source (signatures + bodies) of all candidate entities of all loaded assemblies to the embedding endpoint of the active profile. Gates, all three required: existing `PrivacyConsentAccepted`, new explicit `AISettings.SemanticIndexingEnabled` (default false), and provider readiness via one snapshot. The settings toggle is disabled in UI until consent is accepted and shows the exact copy: "Indexing sends decompiled code from every loaded assembly to your embedding provider to build a local search index." The database contains no decompiled text, no credentials, and no API keys — only file names, MVIDs, entity full names, and vectors. Users can delete per-assembly rows or the whole index from settings. The embedding client enforces the same loopback-only HTTP rule as the chat providers.

**D2 — Storage.** SQLite via `Microsoft.Data.Sqlite`, one per-user database at `%APPDATA%/ICSharpCode/semantic-index.db` (path supplied by the app layer through a `SemanticIndexHost` bridge, mirroring the existing `AISelectionHost` pattern — `ICSharpCode.ILSpyX/AI/AISelectionService.cs:325`). WAL journal mode so reads never block writes. Schema in §5. Assembly identity is the MVID string; a stored row is a cache hit only when MVID, file length, last-write ticks, embedding model, dimensions all match and status is complete — any mismatch deletes and reindexes that assembly. Uninstalling/clearing app data removes the index with it.

**D3 — Model.** Embedding model is a single global `AISettings.EmbeddingModel` (default `text-embedding-3-small`), used with whichever provider is active. Capability is a catalog property: `openai`, `ollama`, `custom` support embeddings (`SupportsEmbeddings = true`, defaults `text-embedding-3-small`, `nomic-embed-text`, `text-embedding-3-small`); `anthropic` does not (no embeddings API) and the UI says so. "Local model" from the roadmap is satisfied by Ollama profiles (local process, loopback HTTP, no cost); shipping an in-process ONNX runtime is explicitly out of scope. Vectors are little-endian float32 blobs; dimension is discovered from the first response and stored per assembly. A dimensions or model mismatch at query time is a stale index: fall back to the heuristic and schedule reindex, never mix vectors across models.

**D4 — Evaluation.** Three layers, all automated and network-free: (1) unit — deterministic fake provider (keyword→axis vectors) proves end-to-end ranking, e.g. query "send http request" ranks `HttpClientSender.SendAsync` above `Tokenizer.NextToken`; (2) integration — index a real small fixture assembly through the full pipeline with the fake provider, persist to a temp SQLite file, reopen, query, assert stable top-k (golden top-1); (3) invariants — dims mismatch falls back, model change invalidates, partial index (entity cap exceeded) is labeled and searchable. Real-provider quality evaluation is a documented manual follow-up, not a shipped test.

**Other locked decisions:**

- One `SearchMode.Semantic` mode; the engine is chosen automatically per query (remote when enabled+ready+indexed, local heuristic otherwise). The picker label drops "(local heuristic)" and becomes "Semantic Search"; the pane status line names the engine that ran.
- `SemanticSearchStrategy.SearchLocal` keeps the exact current behavior as the permanent fallback; it is not deleted.
- Query ranking is top-k with no cosine cutoff (roadmap wording); threshold tuning is deferred to evaluation follow-up.
- Candidate enumeration (`types + methods`) is extracted to one internal helper shared by both AI search strategies and the indexer; dedup rules unchanged (`FullName`, ordinal, first wins).
- Cost/rate safety: batches of 32 inputs, at most 2 concurrent embedding requests, 2 retries with backoff on 429/5xx, 5000-entity cap per assembly (excess marks the index `partial` and is reported).
- Decompilation failures on malformed metadata degrade to name-only text for that entity (guard mirrors `ContextBuilder.IsRecoverableMetadataException`); they never abort a run.

**Out of scope:** vector DBs / sqlite-vec extensions (exhaustive in-memory cosine is sufficient at ≤5000×1536 per assembly), ANN indexes, in-process ONNX models, LLM re-ranking of top-k (roadmap marks it optional), embedding individual fields/properties/events, per-profile embedding models, cross-assembly index sharing.

## 4. Alternatives considered

**A. Roadmap implementation with heuristic fallback (chosen).** Provider embeddings + SQLite + background index + cosine, heuristic retained as automatic fallback. Delivers every §4.6 criterion; failure modes degrade gracefully; opt-in protects cost/privacy. Risk: new native ADO dependency (`Microsoft.Data.Sqlite` + e_sqlite3 native, ~1.5 MB per RID) added to the ILSpyX package. Accepted because ILSpyX already ships third-party deps (Markdig, YamlDotNet, K4os.Compression.LZ4) and the roadmap names SQLite explicitly.

**B. Local-only embeddings (in-process ONNX).** No network or cost, but adds a large native runtime plus a model-download/distribution problem, and does not satisfy the stated gap ("remote embedding-backed"). Revisit only if users report cost/privacy blockers with A.

**C. In-memory index, no persistence.** Smallest change, but re-embeds the whole assembly list every session — strictly worse cost and latency than today for remote providers, and fails the roadmap's cache criterion. Rejected.

## 5. Database schema

Created idempotently on first open (`InitializeAsync`); `schema_info` carries `schema_version = 1` for future migrations. Roadmap's literal sketch (`embeddings(token TEXT PRIMARY KEY, vector BLOB)`) is adapted to a composite key because FQNs collide across assemblies and tokens are only unique per module; the token+vector BLOB spirit is preserved.

```sql
CREATE TABLE IF NOT EXISTS schema_info (key TEXT PRIMARY KEY, value TEXT NOT NULL);
CREATE TABLE IF NOT EXISTS assemblies (
  assembly_id     TEXT PRIMARY KEY,   -- MVID guid string ("N" format)
  file_name       TEXT NOT NULL,
  mvid            TEXT NOT NULL,
  file_length     INTEGER NOT NULL,
  last_write_ticks INTEGER NOT NULL,
  embedding_model TEXT NOT NULL,
  dimensions      INTEGER NOT NULL,
  entity_count    INTEGER NOT NULL,
  status          TEXT NOT NULL,      -- 'complete' | 'partial'
  indexed_at_utc  TEXT NOT NULL       -- ISO 8601
);
CREATE TABLE IF NOT EXISTS embeddings (
  assembly_id  TEXT NOT NULL,
  entity_token INTEGER NOT NULL,      -- raw metadata token (System.Reflection.Metadata)
  full_name    TEXT NOT NULL,
  kind         INTEGER NOT NULL,      -- SemanticIndexEntityKind: 0=Type, 1=Method
  vector       BLOB NOT NULL,         -- float32[dimensions], little-endian
  PRIMARY KEY (assembly_id, entity_token)
) WITHOUT ROWID;
```

## 6. File structure

New files (all in `ICSharpCode.ILSpyX` unless prefixed):

| File | Responsibility |
|---|---|
| `AI/Embeddings/IEmbeddingProvider.cs` | Wire-neutral embedding contract + result types |
| `AI/Embeddings/OpenAIEmbeddingProvider.cs` | OpenAI-compatible `/v1/embeddings` client (OpenAI, Ollama, custom) |
| `AI/SemanticIndex/ISemanticIndexStore.cs` | Persistence contract + records (`SemanticAssemblyFingerprint`, `SemanticAssemblyIndexInfo`, `SemanticVectorEntry`, `SemanticIndexEntityKind`) |
| `AI/SemanticIndex/SqliteSemanticIndexStore.cs` | `Microsoft.Data.Sqlite` implementation, WAL, schema init, BLOB codec |
| `AI/SemanticIndex/SemanticIndexService.cs` | Orchestrator: gates, debounce, fingerprint checks, batching, retries, progress, cancellation |
| `AI/SemanticIndex/EntityTextExtractor.cs` | Entity → embedding text (signature + guarded, truncated decompiled body) |
| `AI/SemanticIndex/SemanticIndexHost.cs` | App bridge supplying the DB path (mirrors `AISelectionHost`) |
| `Search/SearchCandidateEnumerator.cs` | Shared internal candidate enumeration (types + methods) |
| `ILSpy/AI/SemanticIndexHostImpl.cs` | App-layer host: `%APPDATA%/ICSharpCode/semantic-index.db` |
| `ILSpy/AI/SemanticIndexServiceExporter.cs` | MEF export wiring + `CurrentAssemblyListChangedEventArgs` subscription |
| Tests: `ICSharpCode.ILSpyX.Tests/AI/OpenAIEmbeddingProviderTests.cs`, `SqliteSemanticIndexStoreTests.cs`, `SemanticIndexServiceTests.cs`, `EntityTextExtractorTests.cs`, `SemanticSearchRemoteTests.cs`; `ILSpy.Tests/Search/SemanticPaneTests.cs` | Per-phase test cycles |

Modified files:

| File | Change |
|---|---|
| `AI/AIProviderCatalog.cs` | Descriptor gains `SupportsEmbeddings`, `DefaultEmbeddingModel` |
| `AI/AIProviderFactory.cs` | `IAIProviderFactory` + impl gain `CreateEmbeddingProviderAsync` |
| `Settings/AISettings.cs` | `SemanticIndexingEnabled`, `EmbeddingModel` (+ XML round-trip) |
| `Search/SemanticSearchStrategy.cs` | Add `SearchAsync` with engine selection; rename `Search` → `SearchLocal` (repo-internal only caller is `SearchPaneModel`) |
| `Search/AISearchStrategy.cs` | Use shared `SearchCandidateEnumerator` |
| `ILSpy/Search/SearchPaneModel.cs` | Async semantic path w/ snapshot, status line, fallback wiring |
| `ILSpy/Options/AISettingsViewModel.cs`, `AISettingsPanel.axaml` | Semantic group: toggle, model, index list, delete actions |
| `Directory.Packages.props` | `Microsoft.Data.Sqlite` central version |
| `doc/plans/enhancements/ai-search-architecture-decision.md`, `doc/plans/AI-INTEGRATION-ROADMAP.md` | Status reconciliation (phase 6) |

---

## Phase 0 — Baseline

No production changes. Purpose: reproducible baseline and tool verification.

- [ ] **0.1** Run and record outcomes:

```bash
rtk dotnet build ILSpy.Desktop.slnf --no-restore --verbosity minimal
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~AI|FullyQualifiedName~Embedding' --verbosity minimal
rtk dotnet test ILSpy.Tests/ILSpy.Tests.csproj --no-restore --filter 'FullyQualifiedName~Search' --verbosity minimal
```

- [ ] **0.2** Record pre-existing failures verbatim (expected: none per `ai-gap-closure-implementation-plan.md` §14 verification record; any failure is a baseline note, not something this plan fixes).

**Gate:** build green or baseline failures recorded; `rtk` availability confirmed (else substitute plain `dotnet` for every command below).

## Phase 1 — Embedding capability, provider client, settings

**Files:**
- Modify: `AI/AIProviderCatalog.cs`, `AI/AIProviderFactory.cs`, `Settings/AISettings.cs`
- Create: `AI/Embeddings/IEmbeddingProvider.cs`, `AI/Embeddings/OpenAIEmbeddingProvider.cs`
- Test: `ICSharpCode.ILSpyX.Tests/AI/OpenAIEmbeddingProviderTests.cs`, `ICSharpCode.ILSpyX.Tests/AI/AIProviderCatalogTests.cs` (extend), `ICSharpCode.ILSpyX.Tests/AI/AIProviderFactorySnapshotTests.cs` (extend), `ICSharpCode.ILSpyX.Tests/Settings/AISettingsTests.cs` (extend or create alongside existing settings tests)

**Interfaces produced:**

```csharp
namespace ICSharpCode.ILSpyX.AI
{
    /// <summary>Embeds batches of text into a fixed-dimension vector space; the model and
    /// endpoint are fixed at construction from one snapshot. Order of results matches inputs.</summary>
    public interface IEmbeddingProvider
    {
        Task<IReadOnlyList<ReadOnlyMemory<float>>> EmbedAsync(
            IReadOnlyList<string> inputs, CancellationToken cancellationToken = default);
    }
}

// AIProviderDescriptor gains two parameters (append after Implementation):
public sealed record AIProviderDescriptor(
    string Id, string Label, string DefaultBaseUrl, string DefaultModel,
    AIProviderKeyRequirement KeyRequirement, AIProviderImplementation Implementation,
    bool SupportsEmbeddings, string DefaultEmbeddingModel);

// IAIProviderFactory gains:
/// <summary>Creates the embedding provider for an immutable resolved target; throws
/// AIConfigurationException when the provider type has no embeddings API.</summary>
Task<IEmbeddingProvider> CreateEmbeddingProviderAsync(
    AISelectionSnapshot snapshot, string? embeddingModel, CancellationToken cancellationToken = default);

// AISettings gains (XML element names match property names, round-tripped like SendIL):
public bool SemanticIndexingEnabled { get; set; }   // default false
public string EmbeddingModel { get; set; }          // default "text-embedding-3-small", trimmed, never null/empty
```

Catalog values: `openai` → `(true, "text-embedding-3-small")`; `ollama` → `(true, "nomic-embed-text")`; `custom` → `(true, "text-embedding-3-small")`; `anthropic` → `(false, "")`.

`OpenAIEmbeddingProvider` wire behavior (single class serves all three OpenAI-compatible providers, mirroring `OpenAIProvider` conventions):
- Ctor `(string baseUrl, string? apiKey, string model, HttpClient httpClient)`; endpoint building identical to `OpenAIProvider.cs:51-55` but appending `/embeddings` instead of `/chat/completions`; same absolute-HTTP(S) + loopback-only validation; same query/fragment rejection.
- Request: `POST { "model": model, "input": [ ... ] }`, `Authorization: Bearer` when key present. Non-streaming; `Accept: application/json`.
- Response: parse `data` array; normalize via each element's `index` field into input order; validate `embedding` is a non-empty number array; validate every input index present exactly once; else `HttpRequestException` naming the problem.
- Client-side guards: `inputs.Count` in 1..`EmbeddingBatchSize` (32), each input non-empty after trim; model non-empty.
- Error body handling: reuse the bounded 4096-byte read + API-key redaction pattern from `OpenAIProvider.ReadErrorBodyAsync`/`RedactApiKey` (duplicate the small helpers into this class; both providers stay independent).
- Constants `EmbeddingBatchSize = 32` and `MaxConcurrentEmbeddingRequests = 2` live in `IEmbeddingProvider.cs` as `public const int` on a static class `EmbeddingLimits` (single declaration point; indexer enforces the first).

Factory behavior: reuse existing validation, then `if (!descriptor.SupportsEmbeddings) throw new AIConfigurationException($"Provider '{snapshot.ProviderType}' does not offer an embedding API. Use an OpenAI-compatible or Ollama profile for semantic indexing.");`; resolve model = `embeddingModel` when non-whitespace else `descriptor.DefaultEmbeddingModel`; return `new OpenAIEmbeddingProvider(...)` sharing the factory's `HttpClient`.

- [ ] **1.1 Write failing catalog tests** — every descriptor carries the two new values; `anthropic.SupportsEmbeddings` false; all others true; defaults non-empty for supporters.

```csharp
[Test]
public void Anthropic_DoesNotSupportEmbeddings()
{
    var descriptor = AIProviderCatalog.Get("anthropic");
    Assert.That(descriptor.SupportsEmbeddings, Is.False);
    Assert.That(descriptor.DefaultEmbeddingModel, Is.Empty);
}

[Test]
public void Ollama_SupportsEmbeddings_WithLocalDefaultModel()
{
    var descriptor = AIProviderCatalog.Get("ollama");
    Assert.That(descriptor.SupportsEmbeddings, Is.True);
    Assert.That(descriptor.DefaultEmbeddingModel, Is.EqualTo("nomic-embed-text"));
}
```

- [ ] **1.2** Run: `rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~AIProviderCatalog' --verbosity minimal` — expect compile failure (record signature error as the failing state).
- [ ] **1.3** Extend the descriptor record + catalog entries; fix compile; tests pass.
- [ ] **1.4 Write failing provider client tests** using a stub `HttpMessageHandler` (new file follows the stub pattern in `ICSharpCode.ILSpyX.Tests/AI/Providers/` if one exists there, else local class):

```csharp
[Test]
public async Task EmbedAsync_MapsResponseIndexToInputOrder([Values] bool shuffle)
{
    string Body(bool s) => s
        ? """{"data":[{"object":"embedding","index":1,"embedding":[0.5,0.5]},{"object":"embedding","index":0,"embedding":[0.25,0.75]}],"model":"m","usage":{"prompt_tokens":1,"total_tokens":1}}"""
        : """{"data":[{"object":"embedding","index":0,"embedding":[0.25,0.75]},{"object":"embedding","index":1,"embedding":[0.5,0.5]}],"model":"m","usage":{"prompt_tokens":1,"total_tokens":1}}""";
    var handler = new StubHandler(Body(shuffle));
    var provider = new OpenAIEmbeddingProvider("https://api.openai.com", "key", "m", new HttpClient(handler));
    var vectors = await provider.EmbedAsync(new[] { "alpha", "beta" });
    Assert.That(vectors[0].ToArray(), Is.EqualTo(new[] { 0.25f, 0.75f }));
    Assert.That(vectors[1].ToArray(), Is.EqualTo(new[] { 0.5f, 0.5f }));
}
```

Required cases (one test each): shuffled-index normalization; 429 → `HttpRequestException` with status in message (backoff lives in phase 3, not here); missing index 1 of 2 → throws; empty `embedding` array → throws; batch of 33 → throws referencing the limit; request JSON contains `"input":["..."]` and `Authorization` header when key set (assert via captured request in stub); non-loopback HTTP base URL rejected in ctor; API key redacted from error body text.

- [ ] **1.5** Run provider tests — fail, then implement `IEmbeddingProvider.cs` + `OpenAIEmbeddingProvider.cs` + `EmbeddingLimits`; tests pass.
- [ ] **1.6 Write failing factory tests**: snapshot with `ProviderType = "anthropic"` → `AIConfigurationException` mentioning embedding; snapshot openai + null model → provider constructed with `text-embedding-3-small`; openai + `"custom-model"` → uses it. Implement `CreateEmbeddingProviderAsync`; pass.
- [ ] **1.7 Write failing settings tests**: default `SemanticIndexingEnabled == false`; default `EmbeddingModel == "text-embedding-3-small"`; XML round-trip `<SemanticIndexingEnabled>true</SemanticIndexingEnabled><EmbeddingModel>nomic-embed-text</EmbeddingModel>` survives save/load; missing elements → defaults; empty model element → default. Implement; pass.

- [ ] **1.8 Gate + commit**

```bash
rtk dotnet build ILSpy.Desktop.slnf --no-restore --verbosity minimal
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~AIProviderCatalog|FullyQualifiedName~OpenAIEmbeddingProvider|FullyQualifiedName~AIProviderFactory|FullyQualifiedName~AISettings' --verbosity minimal
git add -A && git commit -m "feat(ai): embedding provider contract, OpenAI-compatible client, catalog capability, settings"
```

## Phase 2 — SQLite persistence

**Files:**
- Modify: `Directory.Packages.props` (add `Microsoft.Data.Sqlite` central version; add `PackageReference` to `ICSharpCode.ILSpyX.csproj` beside `Markdig`), `ICSharpCode.ILSpyX/ILSpyX.csproj`
- Create: `AI/SemanticIndex/ISemanticIndexStore.cs`, `AI/SemanticIndex/SqliteSemanticIndexStore.cs`
- Test: `ICSharpCode.ILSpyX.Tests/AI/SqliteSemanticIndexStoreTests.cs`

**Interfaces produced:**

```csharp
namespace ICSharpCode.ILSpyX.AI
{
    public enum SemanticIndexEntityKind { Type = 0, Method = 1 }

    public sealed record SemanticAssemblyFingerprint(
        string AssemblyId, string FileName, string Mvid, long FileLength, long LastWriteTicks);

    public sealed record SemanticAssemblyIndexInfo(
        SemanticAssemblyFingerprint Fingerprint, string EmbeddingModel, int Dimensions,
        int EntityCount, bool Complete, DateTime IndexedAtUtc);

    public sealed record SemanticVectorEntry(
        int EntityToken, string FullName, SemanticIndexEntityKind Kind, float[] Vector);

    public interface ISemanticIndexStore : IDisposable
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
        Task<SemanticAssemblyIndexInfo?> TryGetAssemblyInfoAsync(string assemblyId, CancellationToken cancellationToken = default);
        /// <summary>Atomically replaces any existing rows for the assembly.</summary>
        Task ReplaceAssemblyAsync(SemanticAssemblyIndexInfo info, IReadOnlyList<SemanticVectorEntry> vectors, CancellationToken cancellationToken = default);
        /// <summary>Returns vectors for fingerprint/model/dims-matched assemblies only; silently skips stale or unknown ids.</summary>
        Task<IReadOnlyList<SemanticVectorEntry>> LoadVectorsAsync(IReadOnlyList<SemanticAssemblyFingerprint> fingerprints, string embeddingModel, int expectedDimensions, CancellationToken cancellationToken = default);
        Task DeleteAssemblyAsync(string assemblyId, CancellationToken cancellationToken = default);
        Task DeleteAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SemanticAssemblyIndexInfo>> ListAssembliesAsync(CancellationToken cancellationToken = default);
    }
}
```

Implementation notes: open connections with `Cache=Shared`, `Mode=ReadWriteCreate`; execute `PRAGMA journal_mode=WAL;` once in `InitializeAsync` alongside schema creation (idempotent, `CREATE TABLE IF NOT EXISTS`). All SQLite calls run inside `Task.Run(..., cancellationToken)` — the ADO API is synchronous and must never touch the UI thread. Vector codec: `MemoryMarshal.Cast<float, byte>` both directions, little-endian (net10 is LE on all supported RIDs; assert `BitConverter.IsLittleEndian` once in the codec and throw `NotSupportedException` otherwise). `ReplaceAssemblyAsync` runs `BEGIN`/delete-by-id/insert-batch/`COMMIT` in one transaction. `LoadVectorsAsync` validates per assembly: info exists, embedding model matches, dimensions match, all fingerprint fields match — completeness is deliberately NOT required (`partial` indexes are searchable; they simply cover fewer entities). Mismatches are skipped, not errors.

- [ ] **2.1** Add the package (choose the current stable version at implementation time; record it here); build.
- [ ] **2.2 Write failing store tests** (each test gets a fresh temp file via `Path.Combine(Path.GetTempPath(), $"semantic-index-{Guid.NewGuid():N}.db")`, deleted in teardown):

```csharp
[Test]
public async Task ReplaceAssembly_ThenLoad_RoundTripsVectorsExactly()
{
    string path = NewTempDbPath();
    await using var store = new SqliteSemanticIndexStore(path);
    await store.InitializeAsync();
    var fingerprint = new SemanticAssemblyFingerprint("mvid-1", "A.dll", "mvid-1", 1024, 638712345678901234L);
    var info = new SemanticAssemblyIndexInfo(fingerprint, "text-embedding-3-small", 2, 2, Complete: true, DateTime.UtcNow);
    await store.ReplaceAssemblyAsync(info, new[] {
        new SemanticVectorEntry(0x02000002, "Ns.HttpClientSender", SemanticIndexEntityKind.Type, new[] { 0.25f, 0.75f }),
        new SemanticVectorEntry(0x06000001, "Ns.HttpClientSender.SendAsync", SemanticIndexEntityKind.Method, new[] { 0.5f, 0.5f }),
    });

    var loaded = await store.LoadVectorsAsync(new[] { fingerprint }, "text-embedding-3-small", 2);

    Assert.That(loaded.Count, Is.EqualTo(2));
    Assert.That(loaded[0].Vector, Is.EqualTo(new[] { 0.25f, 0.75f }));
    Assert.That(loaded[0].EntityToken, Is.EqualTo(0x02000002));
}
```

Required cases: round-trip exactness (above); reopen-dispose-reopen persistence survives process boundary; replace is atomic — second `ReplaceAssemblyAsync` for same id leaves exactly the new rows; stale model → `LoadVectorsAsync` returns empty; stale dims → empty; stale last-write ticks → empty; unknown assembly id → empty; `DeleteAssemblyAsync` removes info + vectors; `DeleteAllAsync` empties both tables but keeps schema; `ListAssembliesAsync` ordered by `FileName` ordinal; simultaneous `LoadVectorsAsync` during `ReplaceAssemblyAsync` on second connection returns either old-or-new complete set (WAL check, may use `Task.WhenAll`).

- [ ] **2.3** Run → fail → implement store → pass.
- [ ] **2.4 Gate + commit**

```bash
rtk dotnet build ILSpy.Desktop.slnf --no-restore --verbosity minimal
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~SqliteSemanticIndexStore' --verbosity minimal
git add -A && git commit -m "feat(ai): SQLite semantic index store with fingerprint-scoped loads"
```

## Phase 3 — Indexing pipeline

**Files:**
- Create: `AI/SemanticIndex/EntityTextExtractor.cs`, `AI/SemanticIndex/SemanticIndexService.cs`, `Search/SearchCandidateEnumerator.cs` (internal static, `GetCandidates(MetadataFile)` — body moved verbatim from `SemanticSearchStrategy.GetCandidates`)
- Modify: `Search/SemanticSearchStrategy.cs`, `Search/AISearchStrategy.cs` (both delegate to the enumerator; no behavior change)
- Test: `ICSharpCode.ILSpyX.Tests/AI/EntityTextExtractorTests.cs`, `ICSharpCode.ILSpyX.Tests/AI/SemanticIndexServiceTests.cs`, extend `ICSharpCode.ILSpyX.Tests/AI/EmbeddingStoreTests.cs` untouched (heuristic contract stays)

**Interfaces produced:**

```csharp
// EntityTextExtractor.cs
public static class EntityTextExtractor
{
    public const int MaxEmbeddingTextLength = 2048;
    /// <summary>"{FullName}\n{signature line}\n{decompiled body truncated to budget}".
    /// Decompilation failure (recoverable metadata exceptions) degrades to name+signature only.</summary>
    public static string Extract(IEntity entity, CSharpDecompiler decompiler);
}

// SemanticIndexService.cs
public sealed record SemanticIndexProgress(
    string State, // "disabled" | "idle" | "indexing" | "complete" | "failed" | "canceled"
    string? AssemblyFileName, int CompletedEntities, int TotalEntities, string? Error);

public sealed class SemanticIndexService : IDisposable
{
    public event Action<SemanticIndexProgress>? ProgressChanged;
    public SemanticIndexService(AISelectionService selectionService, IAIProviderFactory providerFactory, ISemanticIndexStore store, AISettings settings);
    /// <summary>Entry point for assembly-list churn. Debounced by IndexDebounceDelay; cancels any superseded run; cached assemblies are skipped.</summary>
    public void OnAssembliesChanged(IReadOnlyList<(MetadataFile Module, SemanticAssemblyFingerprint Fingerprint)> assemblies);
    /// <summary>Immediate, non-debounced run used by settings' rebuild action and tests.</summary>
    public Task ReindexAsync(IReadOnlyList<(MetadataFile Module, SemanticAssemblyFingerprint Fingerprint)> assemblies, AISelectionSnapshot snapshot, string embeddingModel, IProgress<string>? progress, CancellationToken cancellationToken);
    public void Cancel();
}
```

`IndexDebounceDelay = TimeSpan.FromSeconds(2)` and `MaxEntitiesPerAssembly = 5000` are `public static readonly` on `SemanticIndexService`.

Pipeline (`ReindexAsync`, all `ConfigureAwait(false)`):
1. Gate locally (defense in depth; callers gate too): `settings.SemanticIndexingEnabled` and `settings.PrivacyConsentAccepted`, else publish `disabled` and return.
2. Create provider once via `factory.CreateEmbeddingProviderAsync(snapshot, embeddingModel, ct)`.
3. For each assembly (sequentially): fingerprint check — `(TryGetAssemblyInfoAsync)` hit with matching model + dims + length + ticks → skip, count as complete. Miss → delete stale row, enumerate candidates via `SearchCandidateEnumerator`, cap at `MaxEntitiesPerAssembly` (overflow → `Complete: false` in stored info), extract texts, chunk into `EmbeddingBatchSize` batches, embed with `MaxConcurrentEmbeddingRequests` semaphore, retry each batch twice on 429/5xx with 500 ms then 2 s delay (honor `Retry-After` seconds header when present and ≤ 60), one `ReplaceAssemblyAsync` per assembly, progress every batch (`indexing`, file, done/total).
4. Recoverable decompile failures produce name-only text and continue; provider failures after retries fail the run with `failed` + error text (no partial row persisted for that assembly); `OperationCanceledException` → `canceled`, no row.

Fingerprint computation helper (internal static on the service, tested): `Mvid = file.Metadata.GetModuleDefinition().Mvid.ToString("N")`, `AssemblyId = Mvid`, `FileLength = new FileInfo(file.FileName).Length`, `LastWriteTicks = File.GetLastWriteTimeUtc(file.FileName).Ticks`; file-system failure → length 0 / ticks 0 (still indexed; MVID remains the identity).

`EntityTextExtractor.Extract`: reuse the decompile calls from `ContextBuilder.Decompile` (`DecompileTypeAsString` / `DecompileAsString`) but do NOT send IL/call-graph (those are chat-context features; embeddings use source text only). Guard with the same recoverable-metadata exception set — extract `ContextBuilder.IsRecoverableMetadataException` to an internal static helper `AI/ReadableMetadataGuard.cs` used by both classes (pure refactor, `ContextBuilderTests` must stay green). Truncate the composed string to `MaxEmbeddingTextLength` characters.

- [ ] **3.1** Extract `SearchCandidateEnumerator`; retarget both strategies; extend `EmbeddingStoreTests`-level smoke via existing strategy tests; run focused search/AI suites → green (refactor is behavior-neutral).
- [ ] **3.2 Write failing extractor tests** against a fixture assembly (follow `ContextBuilderTests` fixture conventions): type entity text starts with full name and contains `class`/interface keyword from decompiled output; method entity contains its signature; enormous decompilation is truncated to exactly `MaxEmbeddingTextLength`; recoverable-failure fixture yields name-only text without throwing (fixture per `ContextBuilder` malformed-metadata tests; if none exists, assert the guard path via a stubbed decompiler throwing the recoverable set — take the stub route if `ContextBuilderTests` has no such fixture to copy).
- [ ] **3.3** Implement extractor → pass.
- [ ] **3.4 Write failing service tests** with fake provider (`FakeEmbeddingProvider : IEmbeddingProvider` recording calls, returning deterministic unit vectors e.g. first input char selects an axis), temp SQLite store, and `AISettings` with `SemanticIndexingEnabled = true`, `PrivacyConsentAccepted = true`:

```csharp
[Test]
public async Task Reindex_IndexesAndPersists_ThenSecondRunSkipsCachedAssembly()
{
    // arrange: one module (use a small fixture dll already referenced by the test project),
    // snapshot for provider "openai", fake provider counting batches
    await service.ReindexAsync(inputs, snapshot, "text-embedding-3-small", progress: null, ct: default);
    int batchesAfterFirst = fake.BatchCallCount;
    await service.ReindexAsync(inputs, snapshot, "text-embedding-3-small", progress: null, ct: default);

    Assert.That(fake.BatchCallCount, Is.EqualTo(batchesAfterFirst), "second run must be fully cache-served");
    Assert.That(store.ListAssembliesAsync(...).Result.Count, Is.EqualTo(1));
}
```

Required cases: disabled setting → zero provider calls, state `disabled`; consent missing → same; cache hit skip (above); model change reindexes (second run with `"nomic-embed-text"` calls provider again and stores new model); entity cap marks `Complete: false` and caps persisted rows at 5000; batch retry: fake throws 429 once then succeeds → run completes, exactly 2 attempts for that batch; fake always-429 → state `failed`, no rows persisted, error mentions status; cancellation mid-run → state `canceled`, no rows; progress sequence starts `(indexing, file, 0, N)` and ends `(complete, null, N, N)`; provider-not-supporting-embeddings snapshot → `AIConfigurationException` surfaces as `failed` with its message.

- [ ] **3.5** Implement service → pass.
- [ ] **3.6 Gate + commit**

```bash
rtk dotnet build ILSpy.Desktop.slnf --no-restore --verbosity minimal
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~SemanticIndex|FullyQualifiedName~EntityTextExtractor|FullyQualifiedName~ContextBuilder|FullyQualifiedName~Search' --verbosity minimal
git add -A && git commit -m "feat(ai): background semantic indexing pipeline with SQLite cache and retries"
```

## Phase 4 — Query path and search-pane integration

**Files:**
- Modify: `Search/SemanticSearchStrategy.cs`, `ILSpy/Search/SearchPaneModel.cs`, `ILSpy/Search/SearchPane.axaml`
- Create: `ILSpy/AI/SemanticIndexHostImpl.cs`, `ILSpy/AI/SemanticIndexServiceExporter.cs`
- Test: `ICSharpCode.ILSpyX.Tests/AI/SemanticSearchRemoteTests.cs`, `ILSpy.Tests/Search/SearchPaneModelTests.cs` (extend; also touch `SearchPrefixParsingTests.cs` if the `semantic:` prefix assertions live there)

**Interfaces produced:**

```csharp
// SemanticSearchStrategy.cs (Search stays, renamed callers only)
public static class SemanticSearchStrategy
{
    /// <summary>Remote cosine search over the persistent index when enabled, ready, and
    /// fingerprint-matched; otherwise the local hash-vector heuristic. Never throws for
    /// engine unavailability — falls back and reports which engine ran.</summary>
    public static async Task<(IReadOnlyList<IEntity> Results, string Engine)> SearchAsync(
        IEnumerable<MetadataFile> modules, string query, AISelectionSnapshot snapshot,
        IAIProviderFactory providerFactory, ISemanticIndexStore store, AISettings settings,
        int limit = 20, CancellationToken cancellationToken = default);
    // Engine values: "remote-embeddings" | "local-heuristic"
    public static IReadOnlyList<IEntity> SearchLocal(IEnumerable<MetadataFile> modules, string query, int limit = 20); // unchanged body
}
```

Engine-selection algorithm (order matters):
1. If `!settings.SemanticIndexingEnabled` → `SearchLocal`, engine `local-heuristic`.
2. If catalog says provider lacks embeddings → `SearchLocal`.
3. Fingerprint each module; `LoadVectorsAsync` with `snapshot`-resolved embedding model (`settings.EmbeddingModel` non-blank → it, else catalog default). Zero usable assemblies → `SearchLocal`.
4. Embed the query (single input). Response dims ≠ any stored dims → `SearchLocal` (stale; reindex will fix).
5. Cosine over all loaded entries (exhaustive; reuse `EmbeddingStore.Cosine` math as a shared internal static `CosineSimilarity(float[], float[])` — move the two-dim-guard implementation once, `EmbeddingStore` delegates to it). Top-k by score, k = `limit`, ties broken by `FullName` ordinal for determinism.
6. Resolve tokens: group by module; `compilation.MainModule.ResolveEntity(MetadataTokens.EntityHandle(entry.EntityToken))` (compilation via `GetTypeSystemWithDecompilerSettingsOrNull(new DecompilerSettings())`); unresolved/nil → drop silently. Engine `remote-embeddings`.
7. Any provider `HttpRequestException`/`AIConfigurationException` in 3-6 → `SearchLocal` (failure is a fallback, not a user error).

**Pane wiring** (`SearchPaneModel`): semantic mode now resolves a snapshot exactly like AI mode (hoist the `mode is SearchMode.AI or SearchMode.Semantic` branch to resolve for both; semantic still doesn't *require* it — resolution failure falls back to local heuristic, not to literal search). Pass an exported `SemanticIndexService`/store handle via `AppComposition.TryGetExport`. Add one bound property `[ObservableProperty] public partial string SpecialSearchStatus { get; set; } = string.Empty;` — set `"Semantic search: remote embeddings"` or `"Semantic search: local heuristic (index disabled or unavailable)"` on the dispatcher after `SearchAsync` returns, cleared when a non-special run starts. Render as a one-line `TextBlock` under the mode picker in `ILSpy/Search/SearchPane.axaml` (minimal layout change, `TextBlock.IsVisible` bound to non-empty status). The existing fallback (`StartFallbackSearch`) fires only when `SearchAsync` itself throws (unexpected) — engine fallback is handled inside the strategy, so no-results from a healthy engine must NOT trigger literal fallback (preserve current AI-mode empty-result behavior exactly; only semantic behavior changes).

**App composition** (`SemanticIndexServiceExporter.cs`): MEF export of `SemanticIndexHost` impl (`Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ICSharpCode", "semantic-index.db")`, create directory on first use); constructs the shared `SqliteSemanticIndexStore` + `SemanticIndexService` (`[Shared]`), subscribes `Util.MessageBus<Util.CurrentAssemblyListChangedEventArgs>` — on change, when `SemanticIndexingEnabled && consent`, resolve snapshot once, map loaded assemblies (`GetAssemblies()` → `GetMetadataFileOrNull()` → fingerprint) and call `OnAssembliesChanged`; skip auto-loaded-only adds exactly like `SearchPaneModel.OnAssemblyListChanged` (issue #3734 loop guard — extract or replicate that check). Dispose on app shutdown (`DockWorkspace` lifecycle neighbors show the pattern).

- [ ] **4.1 Write failing strategy tests** (fake provider + temp store, fixture module):

```csharp
[Test]
public async Task SearchAsync_RanksSemanticMatchesOverUnrelated_WithFakeEmbeddings()
{
    // FakeEmbeddingProvider: "http"/"send" tokens → +x axis; "parse"/"token" → +y axis.
    // Index a fixture containing HttpClientSender.SendAsync (method) and Tokenizer.NextToken.
    var (results, engine) = await SemanticSearchStrategy.SearchAsync(
        modules, "send http request", snapshot, factory, store, enabledSettings);

    Assert.That(engine, Is.EqualTo("remote-embeddings"));
    Assert.That(results[0].FullName, Does.Contain("SendAsync"));
}
```

Required cases: disabled → engine `local-heuristic`, zero provider calls; enabled+indexed → `remote-embeddings` + correct order (above); index present but query embedding fails (fake throws) → `local-heuristic`, no throw; dims mismatch → `local-heuristic`; stored token unresolvable in current module → dropped, remaining results intact; deterministic tie-break; `SearchLocal` output identical to pre-rename `Search` for the same inputs (regression pin — copy the current expectation from existing semantic tests).

- [ ] **4.2** Implement strategy changes → pass.
- [ ] **4.3 Write failing pane tests** (follow existing `ILSpy.Tests/Search/` pane-test conventions; where dispatcher is involved use the established test harness pattern from chat pane tests): semantic query with enabled settings calls `SearchAsync` (observable via a seam — pane takes `Func<...>`/interface defaulting to the static strategy, constructor-injectable fake); snapshot resolution failure → heuristic runs, no literal fallback; status text set on dispatcher; `semantic:` prefix still routes to semantic mode; AI-mode behavior unchanged (snapshot, empty-result → literal fallback preserved exactly).
- [ ] **4.4** Implement pane + exporter wiring → pass.
- [ ] **4.5 Gate + commit**

```bash
rtk dotnet build ILSpy.Desktop.slnf --no-restore --verbosity minimal
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~SemanticSearch|FullyQualifiedName~EmbeddingStore' --verbosity minimal
rtk dotnet test ILSpy.Tests/ILSpy.Tests.csproj --no-restore --filter 'FullyQualifiedName~Search' --verbosity minimal
git add -A && git commit -m "feat(search): remote cosine semantic search with heuristic fallback and pane status"
```

## Phase 5 — Settings UI: opt-in, model, index management

**Files:**
- Modify: `ILSpy/Options/AISettingsViewModel.cs`, `ILSpy/Options/AISettingsPanel.axaml`
- Test: extend `ILSpy.Tests/Options/AISettingsViewModelTests.cs` (the existing settings view-model test file)

UI (one new group "Semantic search" in the AI panel, below privacy):
- Toggle `Enable semantic indexing` — `IsEnabled` bound to `PrivacyConsentAccepted`; enabling sets `SemanticIndexingEnabled`; warning `TextBlock` with the D1 copy verbatim, visible when toggle is on.
- `Embedding model` TextBox bound to `EmbeddingModel`, placeholder = active provider's `DefaultEmbeddingModel`, watermarked helper text "Applies to OpenAI-compatible and Ollama profiles. Anthropic does not offer embeddings." Active provider shown via existing selection-state exposure in the view model.
- Index list `ListBox` (assembly file name, model, entity count, indexed date, `partial` badge) bound to `ListAssembliesAsync` results loaded when the panel opens; per-row Delete button (`DeleteAssemblyAsync` + refresh), global "Delete entire index" button with the panel's existing confirmation pattern; "Rebuild now" button → resolves snapshot once, calls `ReindexAsync` with a progress string bound under the list; disabled with reason when not ready/not supported.

View-model tests: toggle disabled until consent; enabling persists `SemanticIndexingEnabled`; warning visibility toggles; delete calls store and refreshes list; rebuild resolves exactly one snapshot and surfaces `AIConfigurationException` as actionable message with zero provider calls; Anthropic active profile disables rebuild with the no-embeddings message.

- [ ] **5.1** Write failing view-model tests → **5.2** implement view model + axaml → pass.
- [ ] **5.3 Gate + commit**

```bash
rtk dotnet build ILSpy.Desktop.slnf --no-restore --verbosity minimal
rtk dotnet test ILSpy.Tests/ILSpy.Tests.csproj --no-restore --filter 'FullyQualifiedName~AISettings|FullyQualifiedName~Options' --verbosity minimal
git add -A && git commit -m "feat(ai): semantic indexing settings with consent gate and index management"
```

## Phase 6 — Documentation reconciliation and full verification

**Files:**
- Modify: `doc/plans/enhancements/ai-search-architecture-decision.md`, `doc/plans/AI-INTEGRATION-ROADMAP.md` (§4.6 status), `doc/plans/enhancements/ai-gap-closure-implementation-plan.md` (§13 cross-reference note only), `ICSharpCode.ILSpyX/Search/SemanticSearchStrategy.cs` XML docs, this file's status header

- [ ] **6.1** Decision doc: replace the descope sentence (lines 19-22) with the implemented design — one paragraph summarizing D1–D4 and linking this plan; keep the `SearchPaneModel` special-case decision unchanged. Roadmap §4.6: mark implemented with deviations (composite key schema; opt-in indexing; top-k without threshold; ONNX descoped) recorded. Update strategy XML docs and the pane mode label ("Semantic Search").
- [ ] **6.2** Full verification:

```bash
rtk dotnet build ILSpy.Desktop.slnf --no-restore --verbosity minimal
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~AI|FullyQualifiedName~Embedding|FullyQualifiedName~Semantic|FullyQualifiedName~Search|FullyQualifiedName~ContextBuilder' --verbosity minimal
rtk dotnet test ILSpy.Tests/ILSpy.Tests.csproj --no-restore --filter 'FullyQualifiedName~AI|FullyQualifiedName~Search|FullyQualifiedName~Options' --verbosity minimal
rtk git diff --check
rtk git status --short
```

(Note the known full-solution timeout limitation recorded in `ai-gap-closure-implementation-plan.md` §14; do not re-attempt it without instruction.)

- [ ] **6.3** Manual regression checklist (record outcomes in this file):
  1. Fresh profile, consent accepted, indexing OFF → semantic query uses heuristic; status says so; zero network calls to `/embeddings` (verify via provider dashboard or local proxy).
  2. Enable indexing with Ollama profile → assemblies index in background with progress; re-query → `remote-embeddings` engine; results navigate on click.
  3. Reload unchanged assemblies → no re-embedding (log/batch count); rebuild assembly → reindex only that one.
  4. Switch profile to Anthropic → semantic query degrades to heuristic with clear status; settings show "no embeddings API".
  5. Delete index from settings → immediate heuristic fallback; DB file shrinks/empties.
  6. Cancel mid-index (close pane/exit app) → no partial rows; app exit is clean (no WAL lock errors on next open).

- [ ] **6.4** Update this plan's status header with date + verification record; commit: `git add -A && git commit -m "docs(ai): record remote embedding semantic search implementation"`.

## Rollback and implementation safety

- Every phase is independently buildable and revertible; revert by phase commit.
- SQLite is additive: the store file is created lazily on first enabled use; removing the feature or toggling off leaves at worst an inert `.db` file — deleting it is always safe (it is a cache, never source of truth).
- The heuristic path (`SearchLocal`) must remain callable at every commit; any phase that breaks it must be fixed before proceeding (regression pin in 4.1 guards this).
- Never persist partial assemblies: a failed/canceled run writes no rows for that assembly (re-run is the recovery).
- If `Microsoft.Data.Sqlite` proves unacceptable for the ILSpyX package (e.g., native RID matrix problems in phase 2), stop and surface the decision — the fallback is a length-prefixed binary file implementing the same `ISemanticIndexStore`; do not silently swap storage engines mid-plan.
