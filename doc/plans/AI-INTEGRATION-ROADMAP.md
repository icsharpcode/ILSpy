# ILSpy AI Integration - Implementation Roadmap

This document outlines the phased implementation plan for AI/LLM integration into ILSpy.

## Overview

**Goal:** Enable BYOK (Bring Your Own Key) AI assistance for decompilation workflows, focusing on deobfuscation, code understanding, and security analysis.

**Supported Providers:** OpenAI, Anthropic, Ollama, any OpenAI-compatible endpoint

**Architecture Principle:** All AI features share a common foundation (settings, provider abstraction, context builder). Build the foundation once, then add features incrementally.

---

## Phase 0: Foundation (Prerequisites)

**Estimated effort:** 2-3 weeks  
**Dependencies:** None  
**Goal:** Build the shared infrastructure that all AI features depend on  
**Status:** Implemented; cross-platform credential-store smoke tests and the repository's pinned .NET 11 test run remain validation gates

### Tasks (ordered easy → hard)

#### 0.1 Token Counter Utility
**Difficulty:** ⭐ Easy  
**Files:** `ICSharpCode.ILSpyX/AI/TokenCounter.cs`  
**Description:** Implement approximate token counting for context budget management  
**Acceptance criteria:**
- Heuristic-based (4 chars ≈ 1 token for English, 3 for code)
- `int CountTokens(string text)` method
- Good enough for budget decisions, doesn't need tiktoken accuracy
- Unit tests with known examples

#### 0.2 AI Settings Data Model
**Difficulty:** ⭐ Easy  
**Files:** `ICSharpCode.ILSpyX/Settings/AISettings.cs`  
**Description:** Define the settings class that holds API keys, model selection, provider config  
**Acceptance criteria:**
- Properties: Provider, ApiKey, BaseUrl, Model, MaxContextTokens, StreamResponses, SendIL, SendCallGraph, PrivacyConsentAccepted
- XML serialization/deserialization compatible with `ILSpySettings`
- Default values set appropriately
- No UI yet (Phase 1)

#### 0.3 Secure API Key Storage
**Difficulty:** ⭐⭐ Medium  
**Files:** `ICSharpCode.ILSpyX/AI/SecureKeyStorage.cs`  
**Description:** Platform-specific credential storage (DPAPI/Keychain/libsecret)  
**Acceptance criteria:**
- Never store API key in plain XML
- Windows: DPAPI (`ProtectedData.Protect`)
- macOS: Keychain via P/Invoke or `security` CLI
- Linux: Secret Service via `secret-tool`; report storage as unavailable when no secure service exists
- Never fall back to an application-managed file without a platform-protected encryption key
- Async `SaveKeyAsync`, `LoadKeyAsync`, `TryLoadKeyAsync`, and `DeleteKeyAsync` operations
- Distinguish a missing key from an unavailable credential store

#### 0.4 LLM Provider Interface
**Difficulty:** ⭐ Easy  
**Files:** `ICSharpCode.ILSpyX/AI/ILLMProvider.cs`, `ICSharpCode.ILSpyX/AI/LLMRequest.cs`  
**Description:** Define the abstraction for all LLM providers  
**Acceptance criteria:**
```csharp
public interface ILLMProvider
{
    IAsyncEnumerable<string> CompleteAsync(LLMRequest request, CancellationToken ct);
}

public record LLMRequest(string SystemPrompt, IReadOnlyList<LLMMessage> Messages, int MaxTokens);
public record LLMMessage(string Role, string Content); // "user" | "assistant" | "system"
```

#### 0.5 OpenAI Provider Implementation
**Difficulty:** ⭐⭐ Medium  
**Files:** `ICSharpCode.ILSpyX/AI/Providers/OpenAIProvider.cs`  
**Description:** Implement OpenAI-compatible chat completion API (covers OpenAI, Ollama, and custom endpoints)  
**Acceptance criteria:**
- HTTP POST to `{baseUrl}/v1/chat/completions`
- JSON payload: `{"model": "...", "messages": [...], "max_tokens": ..., "stream": true}`
- Parse SSE (Server-Sent Events) stream for `data: {..."delta":{"content":"..."}...}`
- Handle rate limits (429), auth errors (401), model not found (404)
- Unit tests with mock HTTP responses

#### 0.6 Decompilation Context Builder (Basic)
**Difficulty:** ⭐⭐ Medium  
**Files:** `ICSharpCode.ILSpyX/AI/DecompilationContext.cs`, `ICSharpCode.ILSpyX/AI/ContextBuilder.cs`  
**Description:** Extract relevant metadata from ILSpy's type system and build LLM-ready context  
**Acceptance criteria:**
- `DecompilationContext` record with: DecompiledCSharp, FullyQualifiedName, AssemblyName, TargetFramework
- `ContextBuilder(AISettings settings).Build(IEntity entity, CSharpDecompiler decompiler)` method
- Token budget enforcement (truncate at statement boundaries if over limit)
- Serialize to compact Markdown format for LLM consumption
- Unit tests with sample `ITypeDefinition` mocks

**Phase 0 Deliverable:** No user-facing features yet, but all core infrastructure is testable and ready for Phase 1 features.

---

## Phase 1: First User-Facing Features (Easy Wins)

**Estimated effort:** 2-3 weeks  
**Dependencies:** Phase 0 complete  
**Goal:** Ship the first usable AI feature and validate the foundation

### Tasks (ordered easy → hard)

#### 1.1 AI Settings UI Panel
**Difficulty:** ⭐⭐ Medium  
**Files:** `ILSpy/Options/AISettingsViewModel.cs`, `ILSpy/Options/AISettingsPanel.axaml`, `ILSpy/Options/AISettingsPanel.axaml.cs`  
**Description:** Create settings UI panel using existing `IOptionPage` MEF contract  
**Acceptance criteria:**
- Export with `[ExportOptionPage(Title = "AI Assistant", Order = 100)]`
- Dropdown for Provider (OpenAI, Anthropic, Ollama, Custom)
- TextBox for API Key (masked with PasswordChar, "●●●●●●")
- TextBox for Base URL (visible only when Provider = Custom or Ollama)
- TextBox for Model (with common presets in dropdown: gpt-4o, claude-opus-4-8, etc.)
- Slider for MaxContextTokens (4k-128k range, default 32k)
- Checkboxes for SendIL, SendCallGraph (off by default)
- "Test Connection" button that sends a simple "Hello" prompt and shows success/error
- Prominent privacy notice: "Your API key is stored securely. Decompiled code is sent to the provider."
- Required consent checkbox bound to `PrivacyConsentAccepted`; keep all AI actions disabled until checked
- Bind to `AISettings` instance loaded from `SettingsService`

#### 1.2 Simple Explanation Dialog (Blocking)
**Difficulty:** ⭐⭐ Medium  
**Files:** `ILSpy/AI/ExplainContextMenuEntry.cs`, `ILSpy/AI/ExplainDialog.axaml`, `ILSpy/AI/ExplainDialog.axaml.cs`  
**Description:** Right-click any symbol → "Explain with AI" → shows modal dialog with explanation  
**Acceptance criteria:**
- `[ExportContextMenuEntry(Category = "AI", Order = 1000)]`
- Context menu enabled only if `AISettings.ApiKey` is set and `PrivacyConsentAccepted` is true
- Modal dialog with "Explaining..." spinner while waiting
- Display full response in scrollable TextBox
- Non-streaming (wait for full response, easier to implement)
- Error handling: show error message in dialog if API call fails
- Cancel button to abort request
- Works on: methods, types, properties, fields

#### 1.3 Explanation to Clipboard
**Difficulty:** ⭐ Easy  
**Files:** Update `ExplainDialog.axaml` from 1.2  
**Description:** Add "Copy to Clipboard" button to explanation dialog  
**Acceptance criteria:**
- Button below explanation text
- Copies full response to system clipboard
- Shows brief "Copied!" confirmation

**Phase 1 Deliverable:** Users can configure AI settings and get explanations via right-click. First end-to-end validation of the architecture.

---

## Phase 2: Streaming & Enhanced Context

**Estimated effort:** 2-3 weeks  
**Dependencies:** Phase 1 complete  
**Goal:** Improve UX with streaming and richer context

### Tasks (ordered easy → hard)

#### 2.1 Streaming Response Infrastructure
**Difficulty:** ⭐⭐⭐ Hard (Avalonia threading)  
**Files:** Update `OpenAIProvider.cs`, create `ILSpy/AI/StreamingTextControl.axaml`  
**Description:** Wire up `IAsyncEnumerable<string>` from provider to UI updates  
**Acceptance criteria:**
- Background thread consumes `IAsyncEnumerable<string>` from `CompleteAsync`
- Each chunk dispatched to UI thread via `Dispatcher.UIThread.InvokeAsync`
- TextBox content appends chunk-by-chunk (typewriter effect)
- No UI freeze during streaming
- Cancel button aborts enumeration and HTTP request
- Unit test with mock provider that yields chunks with delays

#### 2.2 AI Output Tool Pane (Dockable)
**Difficulty:** ⭐⭐ Medium  
**Files:** `ILSpy/AI/AIOutputPaneModel.cs`, `ILSpy/AI/AIOutputPane.axaml`  
**Description:** Replace modal dialog with dockable pane (like Analyzer pane)  
**Acceptance criteria:**
- Implement `ToolPaneModel` and export with `[ExportToolPane]`
- Shows in View menu → "AI Output"
- Dockable below/right of decompiler text view
- Shows streaming response with typewriter effect
- Header shows: symbol name being explained
- Clear button to reset content
- Copy button (from Phase 1.3)
- Persists dock position in session settings

#### 2.3 Enhanced Context Builder
**Difficulty:** ⭐⭐⭐ Hard  
**Files:** Update `ContextBuilder.cs` from Phase 0.6  
**Description:** Add optional IL, callers, callees, attributes, string literals  
**Acceptance criteria:**
- If `AISettings.SendIL == true`, include IL decompilation
- Extract string literals from method body (walk `SyntaxTree`)
- Extract attributes from symbol (`IEntity.GetAttributes()`)
- If `AISettings.SendCallGraph == true`:
  - Find callers using `MethodUsedByAnalyzer`
  - Find callees by walking method invocations
  - Limit to 10 each (most relevant)
- Token budget respects hierarchy: trim IL first, then callees, then callers, then literals, finally C# (at statement boundaries)
- Serialize all to structured Markdown with sections
- Unit tests with realistic type system mocks

#### 2.4 Assembly Summary Feature
**Difficulty:** ⭐⭐ Medium  
**Files:** `ILSpy/AI/AssemblySummaryContextMenuEntry.cs`  
**Description:** Right-click assembly node → "Summarize Assembly with AI"  
**Acceptance criteria:**
- Context menu on assembly tree nodes only
- Context includes: assembly name/version, top-level namespaces, count of public types, entry point (if any), assembly attributes, TFM
- Sample 5-10 largest public types (names + base classes)
- Response displayed in AI Output pane
- System prompt: "You are analyzing a .NET assembly. Provide a 2-3 paragraph summary: what it is, what framework it targets, what it's probably used for."

**Phase 2 Deliverable:** Users get streaming responses in a dockable pane, with richer context sent to the LLM. Assembly-level summaries available.

---

## Phase 3: Power Features (Rename & Security)

**Estimated effort:** 3-4 weeks  
**Dependencies:** Phase 2 complete  
**Goal:** Deliver the highest-value unique features (deobfuscation assistance)

### Tasks (ordered easy → hard)

#### 3.1 Anthropic Provider Implementation
**Difficulty:** ⭐⭐ Medium  
**Files:** `ICSharpCode.ILSpyX/AI/Providers/AnthropicProvider.cs`  
**Description:** Implement Anthropic Messages API (different JSON schema than OpenAI)  
**Acceptance criteria:**
- HTTP POST to `https://api.anthropic.com/v1/messages`
- JSON payload: `{"model": "...", "messages": [...], "max_tokens": ..., "system": "...", "stream": true}`
- Parse SSE stream for `event: content_block_delta` → `delta.text`
- Handle Anthropic-specific headers (`anthropic-version`, `x-api-key`)
- Rate limit handling (same as OpenAI)
- Unit tests with mock HTTP responses

#### 3.2 Rename Assistant (Single Symbol)
**Difficulty:** ⭐⭐⭐⭐ Very Hard  
**Files:** `ILSpy/AI/RenameAssistantContextMenuEntry.cs`, `ILSpy/AI/RenameDialog.axaml`, `ICSharpCode.ILSpyX/AI/RenameSuggester.cs`  
**Description:** Right-click obfuscated symbol → "Suggest Name with AI" → ranked list of name candidates  
**Acceptance criteria:**
- Context menu visible on methods, types, fields, properties
- Detect likely obfuscation: short names (1-2 chars), all-numeric, random-looking (e.g. `a`, `b1`, `method_47`)
- Context sent: method signature, decompiled body, return type, param types, string literals, attributes, implemented interfaces, callers/callees (names only)
- System prompt: "Suggest 3-5 meaningful names for this obfuscated symbol. Return JSON: [{name, confidence, reasoning}]"
- Parse JSON response
- Show dialog with ranked list (radio buttons), each showing confidence % and reasoning
- "Apply" button (for now: just copies selected name to clipboard with message "Annotation system not yet implemented")
- Error handling: if response is not valid JSON, show raw text

#### 3.3 Rename Annotation Storage (Sidecar File)
**Difficulty:** ⭐⭐⭐ Hard  
**Files:** `ICSharpCode.ILSpyX/Annotations/RenameAnnotations.cs`, `ICSharpCode.ILSpyX/Annotations/RenameAnnotationManager.cs`  
**Description:** Persist user-approved renames in `.ilspy-annotations.json` beside the assembly  
**Acceptance criteria:**
- JSON format: `{"assemblyHash": "...", "renames": [{"token": "0x06000042", "newName": "ProcessPayment"}]}`
- `assemblyHash` = SHA256 of assembly file (detect mismatches)
- `token` = metadata token (unique, survives reassembly)
- Load annotations when assembly is opened
- Save when user applies a rename
- Thread-safe (multiple renames may happen concurrently)
- Unit tests with in-memory JSON

#### 3.4 Display-Time Rename Application
**Difficulty:** ⭐⭐⭐⭐ Very Hard  
**Files:** Update `CSharpDecompiler`, `ITextOutput`, or create a `IDecompilerOutputFilter`  
**Description:** Apply stored renames during decompilation text generation  
**Acceptance criteria:**
- When decompiling, check if entity has a rename annotation
- Replace generated name with annotation name in output
- Preserve all references (call sites, field accesses, type references)
- Visual indicator (e.g. color, tooltip) that name is AI-suggested
- Does NOT modify the assembly file (display-only)
- Unit tests with mock annotations and known decompiler output

#### 3.5 Batch Rename (Whole Class)
**Difficulty:** ⭐⭐⭐⭐⭐ Hardest  
**Files:** Update `RenameAssistantContextMenuEntry.cs`, create `ILSpy/AI/BatchRenameDialog.axaml`  
**Description:** Right-click type → "Batch Rename All Members with AI" → renames all methods/fields in dependency order  
**Acceptance criteria:**
- Progress dialog showing current symbol being processed
- Process members in dependency order (fields → properties → methods, callees before callers)
- Context for each member includes previously-renamed symbols (growing context)
- Token budget per-member (may need chunking for huge classes)
- User can review all suggestions before applying
- Show diff-style view: `oldName → newName` with confidence
- "Apply All" or selective checkboxes
- Cancelable mid-batch
- Save all to annotation file

#### 3.6 Security Analyzer (IAnalyzer)
**Difficulty:** ⭐⭐⭐ Hard  
**Files:** `ICSharpCode.ILSpyX/Analyzers/Builtin/AISecurityAnalyzer.cs`  
**Description:** Implement `IAnalyzer` that uses AI to find security issues  
**Acceptance criteria:**
- Export with `[ExportAnalyzer(Header = "Security Risks (AI)", Order = 1000)]`
- Analyze entire assembly when invoked
- Process types one at a time (avoid token limit blow-up)
- System prompt: "Identify security vulnerabilities. Return JSON array: [{type, method, issue, severity, line}]"
- Parse JSON, create `SearchResult` for each hit
- Display in Analyzer pane tree (clickable → navigates to code)
- Severity levels: Critical, High, Medium, Low (color-coded icons)
- Patterns to detect: SQL injection, hardcoded credentials, weak crypto, path traversal, deserialization risks, dangerous P/Invoke
- Unit tests with known-vulnerable code samples

**Phase 3 Deliverable:** Users can get AI-suggested renames for obfuscated code and see them applied in the decompiler view. Security analyzer identifies potential issues.

---

## Phase 4: Advanced Features (Chat & Search)

**Estimated effort:** 3-4 weeks  
**Dependencies:** Phase 3 complete  
**Goal:** Add conversational AI and semantic search capabilities

### Tasks (ordered easy → hard)

#### 4.1 Documentation Generator
**Difficulty:** ⭐⭐ Medium  
**Files:** `ILSpy/AI/GenerateDocsContextMenuEntry.cs`  
**Description:** Right-click type/method → "Generate XML Documentation" → produces `<summary>`, `<param>`, `<returns>` comments  
**Acceptance criteria:**
- Context menu on types and methods
- System prompt: "Generate XML documentation comments. Return only the XML, no explanation."
- Parse response, format as C# comment block (`/// <summary>...`)
- Insert at top of decompiled text (display-only, not written to assembly)
- Copy button to copy just the doc comments
- Handles generic types, async methods, exceptions thrown

#### 4.2 AI Chat Pane (No History)
**Difficulty:** ⭐⭐⭐ Hard  
**Files:** `ILSpy/AI/AIChatPaneModel.cs`, `ILSpy/AI/AIChatPane.axaml`  
**Description:** New dockable pane with chat input, send button, scrollable message history  
**Acceptance criteria:**
- Implement `ToolPaneModel` and export
- Chat UI: messages list (user/assistant bubbles), input TextBox, Send button
- Auto-inject context: "Currently viewing: {symbol name in active decompiler tab}"
- System prompt: "You are an assistant for .NET decompilation. Answer questions about the code."
- Display streaming responses in assistant bubbles
- Clear conversation button
- No persistence yet (memory-only for session)

#### 4.3 Chat History Persistence
**Difficulty:** ⭐⭐ Medium  
**Files:** Update `AIChatPaneModel.cs`, create `ICSharpCode.ILSpyX/AI/ChatHistory.cs`  
**Description:** Save/load conversation history per assembly  
**Acceptance criteria:**
- Store in `{assembly-dir}/.ilspy-chat-history.json`
- JSON array of messages with timestamps
- Load when assembly opens, save on app exit or conversation clear
- Max history length (e.g. 100 messages) to avoid unbounded growth
- Export conversation to Markdown file (with "Export" button)

#### 4.4 Chat Slash Commands
**Difficulty:** ⭐⭐ Medium  
**Files:** Update `AIChatPane.axaml.cs`  
**Description:** Type `/explain`, `/rename`, `/audit` in chat input for shortcuts  
**Acceptance criteria:**
- `/explain` → trigger explanation for currently-selected symbol
- `/rename {symbolName}` → trigger rename assistant
- `/audit` → trigger security analyzer on entire assembly
- `/summary` → trigger assembly summary
- Auto-complete suggestions as user types `/`
- Response appears in chat as assistant message

#### 4.5 Natural Language Search (LLM-based)
**Difficulty:** ⭐⭐⭐⭐ Very Hard  
**Files:** `ICSharpCode.ILSpyX/Search/AISearchStrategy.cs`  
**Description:** Implement `ISearchStrategy` that uses AI to interpret natural language queries  
**Acceptance criteria:**
- Registered in search pane, toggled with "AI Search" checkbox
- User types: "methods that call the database"
- Context builder samples ~50 random methods as "vocabulary"
- System prompt: "Given these method signatures, which ones match the query? Return JSON array of fully-qualified names."
- Parse response, resolve to `IEntity` objects, return as `SearchResult[]`
- Display in existing search results pane
- Show confidence scores if available
- Fallback to literal search if AI call fails

#### 4.6 Embedding-Based Semantic Search
**Difficulty:** ⭐⭐⭐⭐⭐ Hardest  
**Files:** `ICSharpCode.ILSpyX/AI/EmbeddingStore.cs`, `ICSharpCode.ILSpyX/AI/SemanticSearchStrategy.cs`  
**Description:** Pre-compute embeddings for all methods, store locally, do vector similarity search  
**Acceptance criteria:**
- On assembly load, decompile all methods, compute embeddings (OpenAI `text-embedding-3-small` or local model)
- Store in SQLite: `CREATE TABLE embeddings (token TEXT PRIMARY KEY, vector BLOB)`
- Vector = 1536-dim float array, serialized as binary
- Search: embed query, compute cosine similarity against all stored vectors
- Return top-k results
- Background indexing (don't block UI)
- Cache embeddings (recompute only if assembly changes)
- Optional dependency: can skip if user doesn't want to pay for embedding API calls

**Phase 4 Deliverable:** Users can chat with AI about assemblies, use natural language search, and get semantic search results from pre-computed embeddings.

---

## Summary by Difficulty

**Easy (⭐):**
- 0.1 Token Counter
- 0.2 AI Settings Data Model
- 0.4 LLM Provider Interface
- 1.3 Explanation to Clipboard

**Medium (⭐⭐):**
- 0.3 Secure API Key Storage
- 0.5 OpenAI Provider Implementation
- 0.6 Context Builder (Basic)
- 1.1 AI Settings UI Panel
- 1.2 Simple Explanation Dialog
- 2.2 AI Output Tool Pane
- 2.4 Assembly Summary Feature
- 3.1 Anthropic Provider
- 3.3 Rename Annotation Storage
- 4.1 Documentation Generator
- 4.2 AI Chat Pane (No History)
- 4.3 Chat History Persistence
- 4.4 Chat Slash Commands

**Hard (⭐⭐⭐):**
- 2.1 Streaming Response Infrastructure
- 2.3 Enhanced Context Builder
- 3.6 Security Analyzer

**Very Hard (⭐⭐⭐⭐):**
- 3.2 Rename Assistant (Single Symbol)
- 3.4 Display-Time Rename Application
- 4.5 Natural Language Search

**Hardest (⭐⭐⭐⭐⭐):**
- 3.5 Batch Rename (Whole Class)
- 4.6 Embedding-Based Semantic Search

---

## Dependencies Graph

```
Phase 0 (Foundation)
  └─→ Phase 1 (First Features)
        └─→ Phase 2 (Streaming & Context)
              ├─→ Phase 3 (Rename & Security)
              └─→ Phase 4 (Chat & Search)
```

Within each phase, tasks are independent unless noted.

**Key bottlenecks:**
- 2.1 (Streaming) blocks most of Phase 3 & 4 UX improvements
- 3.3 + 3.4 (Annotation system) blocks 3.5 (Batch Rename)
- 0.5 (OpenAI Provider) blocks everything user-facing

---

## Next Steps

1. Complete Phase 0 validation on the pinned .NET 11 SDK
2. Smoke-test secure key storage on Windows, macOS, and Linux with Secret Service available
3. Create `doc/plans/phase-1-first-features.md`
4. Implement Phase 1 with privacy-consent gating

---

**Document Version:** 1.0  
**Last Updated:** 2026-08-17
