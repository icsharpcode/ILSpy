# AI Prompt Externalization - Detailed Implementation Plan

**Status**: Implemented (Phase 3 completed 2026-08-20)  
**Created**: 2026-08-19  
**Author**: Dr. Masroor Ehsan  
**Target**: Less-capable model execution

## Overview

This document provides step-by-step implementation instructions for externalizing AI system prompts from hardcoded C# constants to `.prompt` files with YAML frontmatter. The design is locked down in `ai-prompt-externalization-plan.md`.

## Prerequisites

- All 9 base prompt files already created in `ICSharpCode.ILSpyX/AI/prompts/`:
  - `explanation.prompt`
  - `rename.prompt`
  - `chat.prompt`
  - `security.prompt` (updated to v2 with confidence field)
  - `security_audit.prompt` (new)
  - `generate_docs.prompt` (new)
  - `search.prompt`
  - `assembly_summary.prompt`
  - `README.md` (updated with new prompt IDs)

## Phase 1: Infrastructure (Tasks 1-3)

### Task 1: Create AIPromptMetadata.cs

**File**: `ICSharpCode.ILSpyX/AI/AIPromptMetadata.cs`

**Instructions**:
1. Create new file with MIT X11 license header (copy from existing `.cs` file)
2. Copyright line: `// Copyright (c) 2026 Dr. Masroor Ehsan`
3. Add `using YamlDotNet.Serialization;`
4. Create `public sealed class AIPromptMetadata` in namespace `ICSharpCode.ILSpyX.AI`
5. Add 7 properties with YamlDotNet attributes:

```csharp
[YamlMember(Alias = "description")]
public string Description { get; set; } = string.Empty;

[YamlMember(Alias = "applies_to_models")]
public List<string>? ApplesToModels { get; set; }

[YamlMember(Alias = "author")]
public string? Author { get; set; }

[YamlMember(Alias = "updated_at")]
public string? UpdatedAt { get; set; }

[YamlMember(Alias = "temperature_hint")]
public double? TemperatureHint { get; set; }

[YamlMember(Alias = "max_tokens_hint")]
public int? MaxTokensHint { get; set; }

[YamlMember(Alias = "version")]
public int? Version { get; set; }
```

**Validation**: File compiles with no errors.

---

### Task 2: Create AIPromptProvider.cs

**File**: `ICSharpCode.ILSpyX/AI/AIPromptProvider.cs`

**Instructions**:

1. Create new file with MIT X11 license header
2. Copyright line: `// Copyright (c) 2026 Dr. Masroor Ehsan`
3. Add usings:
```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
```

4. Create `public sealed class AIPromptProvider` in namespace `ICSharpCode.ILSpyX.AI`

5. Add singleton pattern:
```csharp
private static readonly Lazy<AIPromptProvider> _instance = new Lazy<AIPromptProvider>(() => new AIPromptProvider());
public static AIPromptProvider Instance => _instance.Value;
```

6. Add fields:
```csharp
private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
private readonly string _promptsDirectory;
```

7. Implement private constructor:
```csharp
private AIPromptProvider()
{
    var assembly = typeof(AIPromptProvider).Assembly;
    var assemblyLocation = assembly.Location;
    if (!string.IsNullOrEmpty(assemblyLocation))
    {
        var assemblyDir = Path.GetDirectoryName(assemblyLocation);
        _promptsDirectory = Path.Combine(assemblyDir!, "AI", "prompts");
    }
    else
    {
        _promptsDirectory = Path.Combine(AppContext.BaseDirectory, "AI", "prompts");
    }
}
```

8. Implement public API method:
```csharp
/// <summary>
/// Gets the system prompt for the specified prompt ID and optional model ID.
/// </summary>
/// <param name="promptId">Prompt identifier (e.g., "explanation", "rename").</param>
/// <param name="modelId">Optional model ID for variant selection (e.g., "claude-opus-5").</param>
/// <returns>System prompt text, or embedded fallback if directory/file is missing.</returns>
public string GetSystemPrompt(string promptId, string? modelId = null)
{
    var cacheKey = $"{promptId}:{modelId ?? "<null>"}";
    
    if (_cache.TryGetValue(cacheKey, out var cached))
    {
        return cached;
    }
    
    string prompt;
    
    if (Directory.Exists(_promptsDirectory))
    {
        prompt = LoadFromDirectory(promptId, modelId);
    }
    else
    {
        prompt = GetEmbeddedFallback(promptId);
    }
    
    _cache[cacheKey] = prompt;
    return prompt;
}
```

9. Implement `LoadFromDirectory` private method:
```csharp
private string LoadFromDirectory(string promptId, string? modelId)
{
    var baseName = $"{promptId}.prompt";
    var pattern = $"{promptId}.*.prompt";
    
    // Get all matching files and base file, sort lexicographically
    var allFiles = Directory.GetFiles(_promptsDirectory, pattern, SearchOption.TopDirectoryOnly)
        .Concat(new[] { Path.Combine(_promptsDirectory, baseName) })
        .Where(File.Exists)
        .OrderBy(f => Path.GetFileName(f), StringComparer.Ordinal)
        .ToList();
    
    // If modelId specified, scan variations first
    if (!string.IsNullOrEmpty(modelId))
    {
        foreach (var file in allFiles)
        {
            // Skip base file
            if (Path.GetFileName(file) == baseName)
                continue;
            
            var (metadata, _) = ParsePromptFile(file);
            if (metadata?.ApplesToModels != null && metadata.ApplesToModels.Contains(modelId, StringComparer.Ordinal))
            {
                var (_, promptText) = ParsePromptFile(file);
                return promptText;
            }
        }
    }
    
    // Fallback to base file
    var baseFile = Path.Combine(_promptsDirectory, baseName);
    if (File.Exists(baseFile))
    {
        var (_, promptText) = ParsePromptFile(baseFile);
        return promptText;
    }
    
    // Final fallback to embedded
    return GetEmbeddedFallback(promptId);
}
```

10. Implement `ParsePromptFile` private method:
```csharp
private (AIPromptMetadata? metadata, string promptText) ParsePromptFile(string filePath)
{
    try
    {
        var content = File.ReadAllText(filePath, Encoding.UTF8);
        
        // Strip UTF-8 BOM if present
        if (content.StartsWith("﻿"))
        {
            content = content.Substring(1);
        }
        
        // Normalize line endings
        content = content.Replace("\r\n", "\n");
        
        // Find first --- separator
        var firstSeparator = content.IndexOf("\n---\n", StringComparison.Ordinal);
        if (firstSeparator < 0)
        {
            // No separator found, treat entire content as prompt
            return (null, content.Trim());
        }
        
        var yamlBlock = content.Substring(0, firstSeparator);
        var promptText = content.Substring(firstSeparator + 5); // Skip "\n---\n"
        
        // Check if YAML block starts with ---
        if (!yamlBlock.StartsWith("---\n", StringComparison.Ordinal))
        {
            // Malformed, treat entire content as prompt
            return (null, content.Trim());
        }
        
        // Strip leading ---
        yamlBlock = yamlBlock.Substring(4);
        
        // Parse YAML
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
        
        var metadata = deserializer.Deserialize<AIPromptMetadata>(yamlBlock);
        return (metadata, promptText.Trim());
    }
    catch
    {
        // On any parse error, fall back to embedded
        return (null, GetEmbeddedFallback(Path.GetFileNameWithoutExtension(filePath)));
    }
}
```

11. Implement stub for `GetEmbeddedFallback`:
```csharp
private string GetEmbeddedFallback(string promptId)
{
    return EmbeddedPrompts.Get(promptId);
}
```

**Validation**: File compiles. `EmbeddedPrompts.Get` will error until Task 3 is complete.

---

### Task 3: Create EmbeddedPrompts.g.cs

**File**: `ICSharpCode.ILSpyX/AI/EmbeddedPrompts.g.cs`

**Instructions**:

1. Create new file with MIT X11 license header
2. Copyright line: `// Copyright (c) 2026 Dr. Masroor Ehsan`
3. Add comment: `/// <summary>`
4. Add comment: `/// Generated embedded fallback prompts. DO NOT EDIT - regenerate with BuildTools/PromptEmbedder.`
5. Add comment: `/// </summary>`
6. Create `internal static class EmbeddedPrompts` in namespace `ICSharpCode.ILSpyX.AI`
7. Add dictionary field and `Get` method:

```csharp
private static readonly Dictionary<string, string> _prompts = new Dictionary<string, string>(StringComparer.Ordinal)
{
    ["explanation"] = "You explain decompiled .NET code concisely. State uncertainty when context is incomplete. Never instruct the user to execute code.",
    ["rename"] = "You suggest meaningful C# names for obfuscated .NET symbols. Return only valid JSON: [{\"name\": string, \"confidence\": number, \"reasoning\": string}]. Return 3 to 5 distinct PascalCase or camelCase candidates. Do not include markdown fences or extra text.",
    ["chat"] = "You are an assistant for .NET decompilation. Answer questions about the code clearly and concisely.",
    ["security"] = "You identify security vulnerabilities in decompiled .NET code. Return only valid JSON: [{\"type\": string, \"method\": string, \"issue\": string, \"severity\": \"Critical\"|\"High\"|\"Medium\"|\"Low\", \"line\": number}]. Report only plausible SQL injection, hardcoded credentials, weak cryptography, path traversal, unsafe deserialization, dangerous P/Invoke, or equivalent issues. Do not invent issues.",
    ["search"] = "Given these method and type signatures, which ones match the query? Return only a JSON array of fully-qualified names.",
    ["assembly_summary"] = "You are analyzing a .NET assembly. Provide a 2-3 paragraph summary: what it is, what framework it targets, what it is probably used for.",
};

public static string Get(string promptId)
{
    if (_prompts.TryGetValue(promptId, out var prompt))
    {
        return prompt;
    }
    
    throw new ArgumentException($"Unknown prompt ID: {promptId}", nameof(promptId));
}
```

**Note**: This file will be code-generated in Phase 3. For now, create it manually.

**Validation**: Run `dotnet build ICSharpCode.ILSpyX/ICSharpCode.ILSpyX.csproj --no-restore`. Should compile with errors about missing YamlDotNet (fixed in Task 4).

---

## Phase 2: Dependencies and Build Integration (Tasks 4-5)

### Task 4: Add YamlDotNet Package Reference

**File**: `Directory.Packages.props` (repo root)

**Instructions**:

1. Open `Directory.Packages.props`
2. Locate the `<ItemGroup>` containing `<PackageVersion>` elements
3. Add new entry in alphabetical order:
```xml
<PackageVersion Include="YamlDotNet" Version="16.1.3" />
```

**File**: `ICSharpCode.ILSpyX/ICSharpCode.ILSpyX.csproj`

**Instructions**:

1. Open `ICSharpCode.ILSpyX/ICSharpCode.ILSpyX.csproj`
2. Locate the `<ItemGroup>` containing `<PackageReference>` elements
3. Add new entry in alphabetical order:
```xml
<PackageReference Include="YamlDotNet" />
```

**Validation**: Run `pwsh updatedeps.ps1` from repo root to regenerate `packages.lock.json` files.

---

### Task 5: Add Build Integration for .prompt Files

**File**: `ICSharpCode.ILSpyX/ICSharpCode.ILSpyX.csproj`

**Instructions**:

1. Open `ICSharpCode.ILSpyX/ICSharpCode.ILSpyX.csproj`
2. Add a new `<ItemGroup>` after existing `<ItemGroup>` sections:
```xml
<ItemGroup>
  <Content Include="AI\prompts\*.prompt">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
  <Content Include="AI\prompts\README.md">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

**Validation**: Run `pwsh build.ps1 --no-restore`. Check `ICSharpCode.ILSpyX/bin/Debug/net10.0/AI/prompts/` contains all 7 files.

---

## Phase 3: Consumer Migration (Tasks 6-11)

### Task 6: Migrate AIExplanationService

**File**: `ICSharpCode.ILSpyX/AI/AIExplanationService.cs`

**Instructions**:

1. Open file
2. Delete line 18: `public const string SystemPrompt = "You explain decompiled .NET code concisely. State uncertainty when context is incomplete. Never instruct the user to execute code.";`
3. Locate method `ExplainContextStreamingAsync` (around line 55-64)
4. Replace the body to compute system prompt dynamically:

**OLD**:
```csharp
public IAsyncEnumerable<string> ExplainContextStreamingAsync(
    DecompilationContext context,
    CancellationToken cancellationToken = default)
{
    ArgumentNullException.ThrowIfNull(context);
    return CompleteStreamingAsync(
        SystemPrompt,
        "Explain this selected symbol:\n\n" + context.ToMarkdown(),
        cancellationToken);
}
```

**NEW**:
```csharp
public IAsyncEnumerable<string> ExplainContextStreamingAsync(
    DecompilationContext context,
    CancellationToken cancellationToken = default)
{
    ArgumentNullException.ThrowIfNull(context);
    string systemPrompt = AIPromptProvider.Instance.GetSystemPrompt("explanation", snapshot.ModelId);
    return CompleteStreamingAsync(
        systemPrompt,
        "Explain this selected symbol:\n\n" + context.ToMarkdown(),
        cancellationToken);
}
```

**Validation**: Build compiles. Run existing tests to ensure no regression.

---

### Task 7: Migrate RenameSuggester

**File**: `ICSharpCode.ILSpyX/AI/RenameSuggester.cs`

**Instructions**:

1. Open file
2. Locate the `SystemPrompt` constant (around line 35)
3. Delete it
4. Locate method `SuggestNamesAsync` (around line 57-78)
5. Replace first line of method body to compute system prompt dynamically:

**OLD**:
```csharp
public async Task<List<RenameCandidate>> SuggestNamesAsync(...)
{
    var request = new LLMRequest(SystemPrompt, messages, maxTokens: 512, temperature: 0.3);
    ...
}
```

**NEW**:
```csharp
public async Task<List<RenameCandidate>> SuggestNamesAsync(...)
{
    string systemPrompt = AIPromptProvider.Instance.GetSystemPrompt("rename", snapshot.ModelId);
    var request = new LLMRequest(systemPrompt, messages, maxTokens: 512, temperature: 0.3);
    ...
}
```

**Validation**: Build compiles. Run existing tests.

---

### Task 9: Migrate AISecurityAnalyzer

**File**: `ICSharpCode.ILSpyX/Analyzers/Builtin/AISecurityAnalyzer.cs`

**Instructions**:

1. Open file
2. Locate the `SystemPrompt` constant (around line 25)
3. Delete it
4. Locate method `AnalyzeAsync` (around line 45-88)
5. Find where `SystemPrompt` is used
6. Replace with dynamic lookup:

**OLD**:
```csharp
var request = new LLMRequest(SystemPrompt, new[] { new LLMMessage("user", userPrompt) }, ...);
```

**NEW**:
```csharp
string systemPrompt = AIPromptProvider.Instance.GetSystemPrompt("security", snapshot.ModelId);
var request = new LLMRequest(systemPrompt, new[] { new LLMMessage("user", userPrompt) }, ...);
```

**Validation**: Build compiles. Run `ICSharpCode.ILSpyX.Tests/Analyzers/AISecurityAnalyzerTests.cs` if it exists.

---

### Task 10: Migrate AISecurityAuditService

**File**: `ICSharpCode.ILSpyX/Analyzers/Builtin/AISecurityAuditService.cs`

**Instructions**:

1. Open file
2. Locate method `AnalyzeTypeAsync` (around line 79-89)
3. Find line 86 where inline system prompt is used
4. Replace with dynamic lookup:

**OLD**:
```csharp
await foreach (string chunk in service.CompleteStreamingAsync("You identify security vulnerabilities in decompiled .NET code. Return only valid JSON with type, method, issue, severity, line, and numeric confidence from 0 to 1. Report only plausible issues.", prompt, cancellationToken).ConfigureAwait(false))
```

**NEW**:
```csharp
string systemPrompt = AIPromptProvider.Instance.GetSystemPrompt("security_audit", snapshot.ModelId);
await foreach (string chunk in service.CompleteStreamingAsync(systemPrompt, prompt, cancellationToken).ConfigureAwait(false))
```

**Validation**: Build compiles. Run security audit tests if available.

---

### Task 11: Migrate AISearchStrategy

**File**: `ICSharpCode.ILSpyX/Search/AISearchStrategy.cs`

**Instructions**:

**OLD**:
```csharp
var request = new LLMRequest(SystemPrompt, new[] { new LLMMessage("user", userPrompt) }, ...);
```

**NEW**:
```csharp
string systemPrompt = AIPromptProvider.Instance.GetSystemPrompt("security", snapshot.ModelId);
var request = new LLMRequest(systemPrompt, new[] { new LLMMessage("user", userPrompt) }, ...);
```

**Validation**: Build compiles. Run `ICSharpCode.ILSpyX.Tests/Analyzers/AISecurityAnalyzerTests.cs` if it exists.

---

### Task 11: Migrate AISearchStrategy

**File**: `ICSharpCode.ILSpyX/Search/AISearchStrategy.cs`

**Instructions**:

1. Open file
2. Locate the `SystemPrompt` constant (around line 38)
3. Delete it
4. Locate method `SearchAsync` (around line 66-113)
5. Find where `SystemPrompt` is used
6. Replace with dynamic lookup:

**OLD**:
```csharp
var request = new LLMRequest(SystemPrompt, new[] { new LLMMessage("user", userPrompt) }, ...);
```

**NEW**:
```csharp
string systemPrompt = AIPromptProvider.Instance.GetSystemPrompt("search", snapshot.ModelId);
var request = new LLMRequest(systemPrompt, new[] { new LLMMessage("user", userPrompt) }, ...);
```

**Validation**: Build compiles. Test AI search feature if possible.

---

### Task 12: Migrate AssemblySummaryContextMenuEntry

**File**: `ILSpy/AI/AssemblySummaryContextMenuEntry.cs`

**Instructions**:

1. Open file
2. Locate the `SystemPrompt` constant (around line 69)
3. Delete it
4. Locate method `GenerateSummaryAsync` (around line 85-120)
5. Find where `SystemPrompt` is used
6. Replace with dynamic lookup:

**OLD**:
```csharp
var request = new LLMRequest(SystemPrompt, new[] { new LLMMessage("user", userPrompt) }, ...);
```

**NEW**:
```csharp
string systemPrompt = AIPromptProvider.Instance.GetSystemPrompt("assembly_summary", snapshot.ModelId);
var request = new LLMRequest(systemPrompt, new[] { new LLMMessage("user", userPrompt) }, ...);
```

**Note**: This is in the `ILSpy` project.

**Validation**: Build compiles. Test assembly summary feature if possible.

---

### Task 13: Migrate GenerateDocsContextMenuEntry

**File**: `ILSpy/AI/GenerateDocsContextMenuEntry.cs`

**Instructions**:

1. Open file
2. Locate method `GenerateAsync` (around line 66-77)
3. Find line 71 where inline system prompt is used
4. Replace with dynamic lookup:

**OLD**:
```csharp
await foreach (var chunk in service.CompleteStreamingAsync(
    "Generate XML documentation comments. Return only the XML, no explanation.",
    "Generate <summary>, <param>, <returns>, and exception documentation for this symbol:\n\n" + context.ToMarkdown(), cancellationToken).ConfigureAwait(false))
```

**NEW**:
```csharp
string systemPrompt = AIPromptProvider.Instance.GetSystemPrompt("generate_docs", snapshot.ModelId);
await foreach (var chunk in service.CompleteStreamingAsync(
    systemPrompt,
    "Generate <summary>, <param>, <returns>, and exception documentation for this symbol:\n\n" + context.ToMarkdown(), cancellationToken).ConfigureAwait(false))
```

**Note**: This is in the `ILSpy` project.

**Validation**: Build compiles. Test doc generation feature if possible.

---

## Phase 4: Testing (Task 14)

### Task 14: Create AIPromptProviderTests.cs

1. Open file
2. Locate the `SystemPrompt` constant (around line 38)
3. Delete it
4. Locate method `SearchAsync` (around line 66-113)
5. Find where `SystemPrompt` is used
6. Replace with dynamic lookup:

**OLD**:
```csharp
var request = new LLMRequest(SystemPrompt, new[] { new LLMMessage("user", userPrompt) }, ...);
```

**NEW**:
```csharp
string systemPrompt = AIPromptProvider.Instance.GetSystemPrompt("search", snapshot.ModelId);
var request = new LLMRequest(systemPrompt, new[] { new LLMMessage("user", userPrompt) }, ...);
```

**Validation**: Build compiles. Test AI search feature if possible.

---

### Task 11: Migrate AssemblySummaryContextMenuEntry

**File**: `ILSpy/AI/AssemblySummaryContextMenuEntry.cs`

**Instructions**:

1. Open file
2. Locate the `SystemPrompt` constant (around line 69)
3. Delete it
4. Locate method `GenerateSummaryAsync` (around line 85-120)
5. Find where `SystemPrompt` is used
6. Replace with dynamic lookup:

**OLD**:
```csharp
var request = new LLMRequest(SystemPrompt, new[] { new LLMMessage("user", userPrompt) }, ...);
```

**NEW**:
```csharp
string systemPrompt = AIPromptProvider.Instance.GetSystemPrompt("assembly_summary", snapshot.ModelId);
var request = new LLMRequest(systemPrompt, new[] { new LLMMessage("user", userPrompt) }, ...);
```

**Note**: This is in the `ILSpy` project.

**Validation**: Build compiles. Test assembly summary feature if possible.

---

## Phase 4: Testing (Task 14)

### Task 14: Create AIPromptProviderTests.cs

**File**: `ICSharpCode.ILSpyX.Tests/AI/AIPromptProviderTests.cs`

**Instructions**:

1. Create new file with MIT X11 license header
2. Copyright line: `// Copyright (c) 2026 Dr. Masroor Ehsan`
3. Add usings:
```csharp
using NUnit.Framework;
using ICSharpCode.ILSpyX.AI;
using System.IO;
using System;
```

4. Create test class:
```csharp
[TestFixture]
public class AIPromptProviderTests
{
    [Test]
    public void GetSystemPrompt_ReturnsNonEmptyString_ForKnownPromptIds()
    {
        var promptIds = new[] { "explanation", "rename", "chat", "security", "security_audit", "generate_docs", "search", "assembly_summary" };
        
        foreach (var promptId in promptIds)
        {
            var prompt = AIPromptProvider.Instance.GetSystemPrompt(promptId);
            Assert.That(prompt, Is.Not.Null);
            Assert.That(prompt, Is.Not.Empty);
        }
    }
    
    [Test]
    public void GetSystemPrompt_ThrowsArgumentException_ForUnknownPromptId()
    {
        Assert.Throws<ArgumentException>(() => AIPromptProvider.Instance.GetSystemPrompt("nonexistent"));
    }
    
    [Test]
    public void GetSystemPrompt_ReturnsSameInstance_OnRepeatedCalls()
    {
        var prompt1 = AIPromptProvider.Instance.GetSystemPrompt("explanation");
        var prompt2 = AIPromptProvider.Instance.GetSystemPrompt("explanation");
        Assert.That(ReferenceEquals(prompt1, prompt2), Is.True, "Caching should return same string instance");
    }
    
    [Test]
    public void GetSystemPrompt_WithModelId_ReturnsPrompt()
    {
        var prompt = AIPromptProvider.Instance.GetSystemPrompt("explanation", "claude-opus-5");
        Assert.That(prompt, Is.Not.Null);
        Assert.That(prompt, Is.Not.Empty);
    }
}
```

**Validation**: Run `dotnet test --solution ILSpy.sln --report-trx --filter "FullyQualifiedName~AIPromptProviderTests"`. All 4 tests should pass.

---

## Phase 5: Build-Time Code Generation (Future Enhancement)

**Status**: Deferred to future milestone

**File**: `BuildTools/PromptEmbedder/Program.cs` (to be created)

**Scope**:
- Create console app that reads all `.prompt` files from `ICSharpCode.ILSpyX/AI/prompts/`
- Generate `ICSharpCode.ILSpyX/AI/EmbeddedPrompts.g.cs` with embedded prompts
- Integrate into build via MSBuild `<Exec>` task or pre-build event

**Why Deferred**: Manual `EmbeddedPrompts.g.cs` is sufficient for MVP. Code generation adds build complexity and is not blocking.

---

## Verification Checklist

After completing all tasks:

- [ ] Run `pwsh build.ps1` from repo root — should succeed with no errors
- [ ] Run `dotnet test --solution ILSpy.sln --report-trx` — all tests pass
- [ ] Check `ICSharpCode.ILSpyX/bin/Debug/net10.0/AI/prompts/` contains 7 files
- [ ] Manually test "Explain Code" feature in ILSpy UI
- [ ] Manually test "Rename Symbol" feature
- [ ] Manually test AI chat pane
- [ ] Manually test assembly summary
- [ ] Create a test variation file `explanation.opus.prompt` with `applies_to_models: [claude-opus-5]` and different prompt text, verify it's used when `snapshot.ModelId == "claude-opus-5"`

---

## Common Pitfalls

1. **Lock file pruning**: Always use `pwsh restore.ps1` / `pwsh build.ps1`, never bare `dotnet restore` / `dotnet build`
2. **File headers**: Copy MIT X11 header from existing file — do not invent or paraphrase
3. **Line endings**: On Windows, save new `.cs` files as CRLF. On Linux/macOS, leave as LF. Never force CRLF on Linux/macOS.
4. **YamlDotNet version**: Use exactly version `16.1.3` as specified
5. **StringComparer**: Always use `StringComparer.Ordinal` for filename sorting and model ID matching — never `StringComparison.OrdinalIgnoreCase`
6. **Caching**: `AIPromptProvider` caches by `(promptId, modelId)` tuple — do not bypass cache
7. **Model ID source**: `snapshot.ModelId` is the authoritative source — do not invent or hardcode model IDs

---

## Success Criteria

- All 6 consumer classes migrated to use `AIPromptProvider.Instance.GetSystemPrompt()`
- No hardcoded system prompt constants remain in consumer classes
- All existing tests pass
- New `AIPromptProviderTests` pass (4 tests)
- Build completes with no warnings or errors
- `.prompt` files are copied to output directory
- Manual smoke test of each AI feature succeeds

---

## Rollback Plan

If implementation fails or introduces regressions:

1. `git checkout -- <file>` for each modified consumer class
2. `git clean -fd ICSharpCode.ILSpyX/AI/` to remove new infrastructure files
3. `git checkout -- Directory.Packages.props ICSharpCode.ILSpyX/ICSharpCode.ILSpyX.csproj` to revert dependency changes
4. `pwsh restore.ps1 && pwsh build.ps1` to rebuild clean state

---

## Post-Implementation

After successful implementation:

1. Update `ai-prompt-externalization-plan.md` status to "Implemented"
2. Create example model-specific variation (e.g., `explanation.opus.prompt`) as documentation
3. Consider creating `BuildTools/PromptEmbedder/` code generator (Phase 5)
4. Update contributor documentation if needed
