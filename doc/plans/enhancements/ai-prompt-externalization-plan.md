# AI Prompt Externalization Implementation Plan

Status: Implemented (Phase 4 completed 2026-08-20)
Created: 2026-08-19  
Audience: Implementer with access to the ILSpy codebase  
Prerequisites: Completion of prompt enhancement audit (all 6 system prompts evaluated and rewritten)

## 1. Executive Summary

Currently, all AI system prompts are hardcoded as `const string` literals embedded directly in their consuming classes. This plan externalizes them to a structured file format, enabling:

- **Easy editing** without C# compilation
- **Version control** of prompt evolution independent of code changes
- **Multi-model tuning** by swapping prompt variants per provider
- **Rapid iteration** during prompt engineering cycles
- **Centralized review** of all AI behavior definitions in one place

The implementation preserves existing API surfaces, maintains backward compatibility through embedded fallbacks, and adds zero runtime overhead when the external file is absent.

## 2. File Format Evaluation and Recommendation

### Option A: JSON (Recommended)

**Pros:**
- Native .NET deserialization via `System.Text.Json`
- Schema validation support
- Readable diffs in version control
- Multiline string support via escape sequences or arrays
- No additional dependencies

**Cons:**
- Multiline strings require escaping or array-of-lines encoding
- Comments require workaround (e.g., `"_comment"` keys)

**Structure:**
```json
{
  "$schema": "./ai-prompts-schema.json",
  "version": 1,
  "prompts": {
    "explanation": {
      "system": "You are an expert .NET reverse engineer...",
      "description": "Core code explanation prompt for AIExplanationService"
    },
    "rename": {
      "system": "You are an expert .NET reverse engineer...",
      "description": "Rename suggestion prompt for RenameSuggester"
    }
  }
}
```

### Option B: XML

**Pros:**
- CDATA for clean multiline strings
- Built-in comment support
- Schema validation via XSD

**Cons:**
- More verbose than JSON
- Less common in .NET configuration
- Requires `System.Xml.Linq` or manual parsing

### Option C: INI/TOML

**Pros:**
- Human-readable for simple key-value
- Native multiline in TOML

**Cons:**
- No nested structure (flat)
- TOML requires third-party library
- Poor fit for complex prompt metadata

**Decision: JSON** — balances simplicity, .NET native support, and version-control readability.

## 3. Task 1: Generate Enhanced System Prompts

### 3.1 Prompt Inventory

Six system prompts identified during audit:

| Prompt ID | Source File | Line | Current Constant | Character Count |
|-----------|-------------|------|------------------|-----------------|
| `explanation` | `ICSharpCode.ILSpyX/AI/AIExplanationService.cs` | 18 | `SystemPrompt` | ~80 (current) |
| `rename` | `ICSharpCode.ILSpyX/AI/RenameSuggester.cs` | 35 | `SystemPrompt` | ~220 (current) |
| `chat` | `ILSpy/AI/AIChatPaneModel.cs` | 37 | `SystemPrompt` | ~90 (current) |
| `security` | `ICSharpCode.ILSpyX/Analyzers/Builtin/AISecurityAnalyzer.cs` | 25 | `SystemPrompt` | ~280 (current) |
| `search` | `ICSharpCode.ILSpyX/Search/AISearchStrategy.cs` | 38 | (inline literal) | ~120 (current) |
| `assembly_summary` | `ILSpy/AI/AssemblySummaryContextMenuEntry.cs` | 64 | (inline literal) | ~140 (current) |

### 3.2 Enhanced Prompt Template Structure

Each enhanced prompt follows the established pattern from the audit:

```
[ROLE DECLARATION]
You are an expert .NET reverse engineer with deep knowledge of ECMA-335 CLI specification, CIL instruction set, .NET metadata tables, and CLR type system.

[DOMAIN EXPERTISE ENUMERATION]
You understand:
- CIL opcodes and their semantics
- Metadata token resolution
- Decompilation artifacts (state machines, closures, lambda lifting)
- Obfuscation patterns
- [domain-specific items per prompt]

[BEHAVIORAL RULES]
- State uncertainty when context is incomplete
- Never invent types, methods, or IL that are not in the provided context
- Distinguish compiler-generated artifacts from source code
- [task-specific rules]

[OUTPUT FORMAT]
[Original structured output requirements preserved exactly]
```

### 3.3 File Location and Structure

**Target file:** `ICSharpCode.ILSpyX/AI/ai-prompts.json`

**Rationale:**
- Lives in `ICSharpCode.ILSpyX` (cross-platform shared library)
- Alongside existing AI infrastructure
- Accessible to both ILSpy (Avalonia UI) and ILSpyCmd (CLI)
- Included in NuGet package build

**Schema file:** `ICSharpCode.ILSpyX/AI/ai-prompts-schema.json`

### 3.4 JSON Schema Design

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "type": "object",
  "required": ["version", "prompts"],
  "properties": {
    "version": {
      "type": "integer",
      "const": 1,
      "description": "Schema version for future migrations"
    },
    "prompts": {
      "type": "object",
      "patternProperties": {
        "^[a-z_]+$": {
          "type": "object",
          "required": ["system"],
          "properties": {
            "system": {
              "type": "string",
              "minLength": 10,
              "description": "System prompt text"
            },
            "description": {
              "type": "string",
              "description": "Human-readable purpose"
            },
            "model_hints": {
              "type": "object",
              "description": "Optional model-specific overrides",
              "additionalProperties": {"type": "string"}
            }
          }
        }
      }
    }
  }
}
```

### 3.5 Initial ai-prompts.json Content

```json
{
  "$schema": "./ai-prompts-schema.json",
  "version": 1,
  "prompts": {
    "explanation": {
      "system": "[Enhanced 2000-character prompt from audit]",
      "description": "Explains decompiled .NET code with reverse-engineering expertise"
    },
    "rename": {
      "system": "[Enhanced 1800-character prompt from audit]",
      "description": "Suggests meaningful names for obfuscated symbols"
    },
    "chat": {
      "system": "[Enhanced 1600-character prompt from audit]",
      "description": "Multi-turn decompilation assistance chat"
    },
    "security": {
      "system": "[Enhanced 2200-character prompt from audit]",
      "description": "Identifies security vulnerabilities in decompiled code"
    },
    "search": {
      "system": "[Enhanced 1400-character prompt from audit]",
      "description": "Natural-language search over assembly symbols"
    },
    "assembly_summary": {
      "system": "[Enhanced 1700-character prompt from audit]",
      "description": "Summarizes assemblies from metadata analysis"
    }
  }
}
```

**Deliverable:** `ai-prompts.json` with all 6 enhanced prompts from the audit, validated against schema.

## 4. Task 2: On-Demand Prompt Loading

### 4.1 Architecture Overview

```
┌─────────────────────────────────────────┐
│   AIPromptProvider (new singleton)      │
│   - Load ai-prompts.json on first use  │
│   - Cache in memory (immutable)         │
│   - Fallback to embedded constants      │
└─────────────┬───────────────────────────┘
              │
              ├─ GetPrompt("explanation")
              ├─ GetPrompt("rename")
              ├─ GetPrompt("chat")
              ├─ GetPrompt("security")
              ├─ GetPrompt("search")
              └─ GetPrompt("assembly_summary")
                        │
              ┌─────────┴─────────────────┐
              │ Consumer Classes (6 total)│
              │ - AIExplanationService    │
              │ - RenameSuggester         │
              │ - AIChatPaneModel         │
              │ - AISecurityAnalyzer      │
              │ - AISearchStrategy        │
              │ - AssemblySummary...Entry │
              └───────────────────────────┘
```

### 4.2 New Class: AIPromptProvider

**Location:** `ICSharpCode.ILSpyX/AI/AIPromptProvider.cs`

**Responsibilities:**
1. Load `ai-prompts.json` from assembly directory on first access
2. Deserialize and cache prompt dictionary
3. Return fallback embedded prompts if file absent/invalid
4. Thread-safe lazy initialization

**API Surface:**
```csharp
namespace ICSharpCode.ILSpyX.AI
{
    public sealed class AIPromptProvider
    {
        // Singleton access
        public static AIPromptProvider Instance { get; }

        // Primary API
        public string GetSystemPrompt(string promptId);
        
        // Optional: diagnostics
        public bool IsExternalFileLoaded { get; }
        public string ExternalFilePath { get; }
    }
}
```

**Implementation Sketch:**
```csharp
sealed class AIPromptProvider
{
    static readonly Lazy<AIPromptProvider> LazyInstance = 
        new(() => new AIPromptProvider(), isThreadSafe: true);
    
    public static AIPromptProvider Instance => LazyInstance.Value;
    
    readonly IReadOnlyDictionary<string, string> prompts;
    readonly bool isExternalFileLoaded;
    
    AIPromptProvider()
    {
        string jsonPath = Path.Combine(
            AppContext.BaseDirectory,
            "ai-prompts.json"
        );
        
        if (File.Exists(jsonPath))
        {
            try
            {
                var root = JsonSerializer.Deserialize<PromptRoot>(
                    File.ReadAllText(jsonPath)
                );
                prompts = root.Prompts.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.System,
                    StringComparer.Ordinal
                );
                isExternalFileLoaded = true;
                return;
            }
            catch (JsonException)
            {
                // Fall through to embedded defaults
            }
        }
        
        // Embedded fallback
        prompts = CreateEmbeddedDefaults();
        isExternalFileLoaded = false;
    }
    
    public string GetSystemPrompt(string promptId)
    {
        if (prompts.TryGetValue(promptId, out string? prompt))
            return prompt;
        
        throw new ArgumentException(
            $"Unknown prompt ID: {promptId}",
            nameof(promptId)
        );
    }
    
    static IReadOnlyDictionary<string, string> CreateEmbeddedDefaults()
    {
        return new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["explanation"] = EmbeddedPrompts.Explanation,
            ["rename"] = EmbeddedPrompts.Rename,
            ["chat"] = EmbeddedPrompts.Chat,
            ["security"] = EmbeddedPrompts.Security,
            ["search"] = EmbeddedPrompts.Search,
            ["assembly_summary"] = EmbeddedPrompts.AssemblySummary,
        };
    }
    
    // DTOs for deserialization
    sealed class PromptRoot
    {
        public int Version { get; set; }
        public Dictionary<string, PromptEntry> Prompts { get; set; } = new();
    }
    
    sealed class PromptEntry
    {
        public string System { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
```

### 4.3 New Class: EmbeddedPrompts (Fallback Constants)

**Location:** `ICSharpCode.ILSpyX/AI/EmbeddedPrompts.cs`

**Purpose:** Preserve current embedded prompts as fallback when `ai-prompts.json` is absent or invalid.

**Structure:**
```csharp
namespace ICSharpCode.ILSpyX.AI
{
    /// <summary>
    /// Embedded fallback prompts when ai-prompts.json is unavailable.
    /// These are the original pre-enhancement prompts for compatibility.
    /// </summary>
    static class EmbeddedPrompts
    {
        public const string Explanation = 
            "You explain decompiled .NET code concisely. State uncertainty when context is incomplete. Never instruct the user to execute code.";
        
        public const string Rename = 
            "You suggest meaningful C# names for obfuscated .NET symbols. Return only valid JSON: [{\"name\": string, \"confidence\": number, \"reasoning\": string}]. Return 3 to 5 distinct PascalCase or camelCase candidates. Do not include markdown fences or extra text.";
        
        public const string Chat = 
            "You are an assistant for .NET decompilation. Answer questions about the code clearly and concisely.";
        
        public const string Security = 
            "You identify security vulnerabilities in decompiled .NET code. Return only valid JSON: [{\"type\": string, \"method\": string, \"issue\": string, \"severity\": \"Critical\"|\"High\"|\"Medium\"|\"Low\", \"line\": number}]. Report only plausible SQL injection, hardcoded credentials, weak cryptography, path traversal, unsafe deserialization, dangerous P/Invoke, or equivalent issues. Do not invent issues.";
        
        public const string Search = 
            "Given these method and type signatures, which ones match the query? Return only a JSON array of fully-qualified names.";
        
        public const string AssemblySummary = 
            "You are analyzing a .NET assembly. Provide a 2-3 paragraph summary: what it is, what framework it targets, what it is probably used for.";
    }
}
```

### 4.4 Consumer Migration

Six files require modification to replace hardcoded constants with `AIPromptProvider.Instance.GetSystemPrompt(...)` calls.

#### 4.4.1 AIExplanationService.cs

**Before:**
```csharp
public const string SystemPrompt = "You explain decompiled .NET code concisely...";
```

**After:**
```csharp
public static string SystemPrompt => 
    AIPromptProvider.Instance.GetSystemPrompt("explanation");
```

**Additional changes:** None (all references use `SystemPrompt` property/constant).

#### 4.4.2 RenameSuggester.cs

**Before:**
```csharp
public const string SystemPrompt = "You suggest meaningful C# names...";
```

**After:**
```csharp
public static string SystemPrompt => 
    AIPromptProvider.Instance.GetSystemPrompt("rename");
```

#### 4.4.3 AIChatPaneModel.cs

**Before:**
```csharp
const string SystemPrompt = "You are an assistant for .NET decompilation...";
```

**After:**
```csharp
static string SystemPrompt => 
    AIPromptProvider.Instance.GetSystemPrompt("chat");
```

#### 4.4.4 AISecurityAnalyzer.cs

**Before:**
```csharp
const string SystemPrompt = "You identify security vulnerabilities...";
```

**After:**
```csharp
static string SystemPrompt => 
    AIPromptProvider.Instance.GetSystemPrompt("security");
```

#### 4.4.5 AISearchStrategy.cs

**Before:**
```csharp
var request = new LLMRequest(
    "Given these method and type signatures, which ones match the query?...",
    ...
);
```

**After:**
```csharp
string systemPrompt = AIPromptProvider.Instance.GetSystemPrompt("search");
var request = new LLMRequest(systemPrompt, ...);
```

#### 4.4.6 AssemblySummaryContextMenuEntry.cs

**Before:**
```csharp
var request = new LLMRequest(
    "You are analyzing a .NET assembly. Provide a 2-3 paragraph summary...",
    ...
);
```

**After:**
```csharp
string systemPrompt = AIPromptProvider.Instance.GetSystemPrompt("assembly_summary");
var request = new LLMRequest(systemPrompt, ...);
```

### 4.5 Build Integration

**File deployment:** `ai-prompts.json` must be copied to output directory.

**ILSpyX.csproj modification:**
```xml
<ItemGroup>
  <Content Include="AI\ai-prompts.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
  <Content Include="AI\ai-prompts-schema.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

**NuGet package inclusion:**
- Ensure `ai-prompts.json` is included in `contentFiles` or `build` directory for consumers.
- Test that `ILSpyCmd` CLI can resolve prompts from its output directory.

## 5. Migration Strategy and Backward Compatibility

### 5.1 Phased Rollout

**Phase 1: Infrastructure (no behavior change)**
1. Create `AIPromptProvider` class
2. Create `EmbeddedPrompts` class with current prompts
3. Add `ai-prompts.json` with current (pre-enhancement) prompts
4. Verify file deployment in build output
5. No consumer changes yet

**Phase 2: Consumer Migration**
1. Migrate all 6 consumers to call `AIPromptProvider`
2. Run full test suite
3. Verify behavior unchanged (prompts identical)

**Phase 3: Enhancement**
1. Replace `ai-prompts.json` content with enhanced prompts
2. Update `EmbeddedPrompts` to match (so fallback has same quality)
3. Document prompt enhancement in release notes

### 5.2 Fallback Behavior

| Scenario | Behavior |
|----------|----------|
| `ai-prompts.json` present and valid | Use external file |
| File absent | Use `EmbeddedPrompts` fallback |
| File present but invalid JSON | Log warning, use `EmbeddedPrompts` |
| File present but missing prompt ID | Throw `ArgumentException` (fail-fast) |
| File present but wrong schema version | Log warning, use `EmbeddedPrompts` |

**No silent degradation:** A malformed external file logs a warning but falls back gracefully. A missing prompt ID throws immediately (developer error, not user error).

### 5.3 Version Migration

Future schema changes (e.g., adding model-specific overrides):

```json
{
  "version": 2,
  "prompts": {
    "explanation": {
      "system": "...",
      "model_overrides": {
        "gpt-4": "Adjusted prompt for GPT-4...",
        "claude-opus-5": "Adjusted prompt for Claude Opus 5..."
      }
    }
  }
}
```

`AIPromptProvider` checks `version` field:
- Version 1: Load as-is
- Version 2+: Load with extended logic or migrate on-the-fly
- Unknown version: Fall back to embedded

## 6. Testing Approach

### 6.1 Unit Tests

**New test file:** `ICSharpCode.ILSpyX.Tests/AI/AIPromptProviderTests.cs`

**Test cases:**
1. `LoadValidExternalFile_ReturnsPrompts`
2. `MissingFile_FallsBackToEmbedded`
3. `InvalidJson_FallsBackToEmbedded`
4. `MissingPromptId_ThrowsArgumentException`
5. `ThreadSafeLazyInitialization_SingleInstance`
6. `ExternalFileLoaded_PropertyReturnsTrue`
7. `FallbackUsed_PropertyReturnsFalse`

**Test resources:**
- `TestData/ai-prompts-valid.json`
- `TestData/ai-prompts-invalid.json`
- `TestData/ai-prompts-missing-key.json`

### 6.2 Integration Tests

**File:** `ICSharpCode.ILSpyX.Tests/AI/PromptConsumerIntegrationTests.cs`

**Test cases:**
1. `AIExplanationService_UsesExternalPrompt`
2. `RenameSuggester_UsesExternalPrompt`
3. `AIChatPaneModel_UsesExternalPrompt`
4. `AISecurityAnalyzer_UsesExternalPrompt`
5. `AISearchStrategy_UsesExternalPrompt`
6. `AssemblySummary_UsesExternalPrompt`

**Verification:** Mock file system or deploy test `ai-prompts.json`, verify each consumer receives expected prompt text.

### 6.3 Manual Testing

**Checklist:**
- [ ] Build ILSpy, verify `ai-prompts.json` in output directory
- [ ] Delete `ai-prompts.json`, verify fallback behavior
- [ ] Corrupt `ai-prompts.json`, verify warning logged and fallback used
- [ ] Modify prompt in `ai-prompts.json`, restart app, verify new prompt used
- [ ] Run all 6 AI features (explain, rename, chat, security, search, summary)
- [ ] Check ILSpyCmd CLI resolves prompts correctly

## 7. Rollout Plan

### 7.1 Pre-Implementation

- [ ] Review plan with maintainers
- [ ] Approve JSON schema design
- [ ] Approve prompt IDs (`explanation`, `rename`, etc.)
- [ ] Confirm build integration approach

### 7.2 Implementation Order

1. **PR 1: Infrastructure**
   - `AIPromptProvider.cs`
   - `EmbeddedPrompts.cs`
   - `ai-prompts.json` (current prompts)
   - `ai-prompts-schema.json`
   - Build integration
   - Unit tests

2. **PR 2: Consumer Migration**
   - Migrate all 6 consumers
   - Integration tests
   - Verify no behavior change

3. **PR 3: Prompt Enhancement**
   - Replace `ai-prompts.json` with enhanced prompts
   - Update `EmbeddedPrompts` to match
   - Document in `CHANGELOG.md`

### 7.3 Post-Rollout

- Monitor issue tracker for prompt-related reports
- Document prompt editing workflow in `README.md` or `CONTRIBUTING.md`
- Consider adding `/ai-prompts reload` command for live editing during development

## 8. Success Criteria

### 8.1 Functional Requirements

- [ ] All 6 AI features use `AIPromptProvider`
- [ ] External `ai-prompts.json` loads correctly
- [ ] Fallback to embedded prompts when file absent
- [ ] No runtime exceptions on file load failure
- [ ] Prompt modifications in `ai-prompts.json` take effect on restart

### 8.2 Code Quality

- [ ] Zero hardcoded prompt strings in consumer classes
- [ ] All tests pass (existing + new)
- [ ] No performance regression (lazy load, cached)
- [ ] Code follows ILSpy conventions (headers, formatting, naming)

### 8.3 Documentation

- [ ] `ai-prompts.json` documented in README or wiki
- [ ] JSON schema published
- [ ] Prompt editing workflow documented for contributors
- [ ] CHANGELOG entry describes enhancement

### 8.4 User Impact

- [ ] AI features continue working identically
- [ ] Enhanced prompts deliver measurable quality improvement (fewer hallucinations, more accurate rename suggestions, better security findings)
- [ ] No breaking changes for existing workflows

## 9. Risks and Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Prompt file deleted by user | AI features fail | Embedded fallback ensures continuity |
| Corrupted JSON after manual edit | AI features fail | Validation + fallback + clear error message |
| Prompt IDs typo in consumer code | Runtime exception | Unit tests cover all 6 IDs |
| File not deployed in NuGet package | Consumers get fallback only | Test NuGet package installation |
| Performance hit from file I/O | Slower startup | Lazy singleton + cache (load once) |
| Schema version incompatibility | Future migration complexity | Version field + migration logic |

## 10. Future Enhancements (Out of Scope)

- **Live reload:** Watch `ai-prompts.json` for changes and reload without restart
- **Model-specific prompts:** `model_overrides` per prompt ID
- **Prompt versioning:** Git-tracked prompt history with A/B testing
- **UI editor:** In-app prompt editor with validation
- **Telemetry:** Track which prompts are used, success rates, token consumption
- **Internationalization:** Localized prompts for non-English decompilation

## 11. References

- **Audit document:** (output from previous AI prompt audit task)
- **JSON Schema spec:** https://json-schema.org/draft-07/schema
- **System.Text.Json docs:** https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/
- **ILSpy CLAUDE.md:** `/Volumes/OSCOO1TB/repos/ILSpy/CLAUDE.md`
- **ILSpy build scripts:** `restore.ps1`, `build.ps1`

---

**Appendix A: Prompt ID Reference**

| Prompt ID | Consumer Class | Purpose |
|-----------|----------------|---------|
| `explanation` | `AIExplanationService` | Explains decompiled code with reverse-engineering context |
| `rename` | `RenameSuggester` | Suggests meaningful names for obfuscated symbols |
| `chat` | `AIChatPaneModel` | Multi-turn conversational decompilation assistant |
| `security` | `AISecurityAnalyzer` | Identifies security vulnerabilities in decompiled assemblies |
| `search` | `AISearchStrategy` | Natural-language search over assembly symbol vocabulary |
| `assembly_summary` | `AssemblySummaryContextMenuEntry` | Summarizes entire assemblies from metadata |

**Appendix B: Example Enhanced Prompt (explanation)**

```
You are an expert .NET reverse engineer with deep knowledge of the ECMA-335 CLI specification, CIL instruction set, .NET metadata tables, and CLR type system.

You understand:
- CIL opcodes (ldloc, stfld, callvirt, etc.) and their stack behavior
- Metadata token resolution (TypeRef, TypeDef, MemberRef, MethodDef)
- Type system semantics (variance, constraints, generic instantiation)
- Decompilation artifacts: async/await state machines (<>1__state, MoveNext), iterator blocks (IEnumerable<T> lowering), closures (captured variables in DisplayClass), lambda lifting, and compiler-generated types/members
- Common obfuscation techniques: control-flow flattening, string encryption, type/member renaming, proxy methods, and constant folding
- PDB symbol information and its absence in obfuscated assemblies
- .NET Framework vs .NET Core vs .NET 5+ API differences

When explaining decompiled code:
- State uncertainty when context is incomplete or symbols are missing
- Never invent types, methods, namespaces, or IL instructions that are not present in the provided decompiled output
- Distinguish compiler-generated artifacts (marked with <>, [CompilerGenerated], or predictable patterns) from developer-written code
- When encountering obfuscated names (single characters, ​‌ sequences, numeric-only), acknowledge them as obfuscated rather than inventing plausible names
- Explain control flow in terms of the decompiled C# structure, not the underlying CIL (unless CIL details are specifically relevant)
- If the code appears incomplete or truncated, explicitly state what is missing

Never instruct the user to execute code. Focus on static analysis and explanation of what the code does, not how to run it.
```

---

**End of Implementation Plan**
