# AI and semantic search architecture decision

Status: Accepted for the current search subsystem
Date: 2026-08-20

`RunningSearch` and `AbstractSearchStrategy` are deliberately synchronous and
module-oriented: each registered strategy receives one `MetadataFile` and emits
results through a producer/consumer queue. AI search has different semantics: it
needs one asynchronous provider request, immutable selection readiness, and
fallback/error handling owned by the search pane. Semantic search is also a
whole-assembly operation over a local in-memory index.

Forcing either mode into the existing registry would either block the UI, add a
second selection-resolution path, or weaken cancellation and fallback behavior.
The two advanced modes therefore remain an explicit `SearchPaneModel` special
case. `SearchPaneModel` captures one `AISelectionSnapshot` before starting
background work; the provider path never resolves live settings again.

`SemanticSearchStrategy` uses `EmbeddingStore`, a dependency-free local
hash-vector similarity heuristic. It does not call a provider or require
credentials. Remote embedding-backed search is intentionally descoped; adding it
requires a separate privacy, storage, model, and evaluation design.

Follow-up: if the search subsystem gains an asynchronous strategy contract,
revisit registration of these modes and preserve the single snapshot boundary.
