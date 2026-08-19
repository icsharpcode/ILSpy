# AI Prompt File Format

This directory contains system prompts for ILSpy's AI features in externalized `.prompt` files. The format allows easy editing without C# compilation and supports model-specific prompt variations.

## File Format Specification

Each `.prompt` file has two sections separated by exactly one `---` line:

```yaml
---
description: Human-readable purpose
applies_to_models: [claude-opus-5, deepseek-v4]
author: Dr. Masroor Ehsan
updated_at: 2026-08-19
temperature_hint: 0.3
max_tokens_hint: 2000
version: 1
---
Prompt text starts here.
Can contain --- sequences (only the first --- is the separator).
Multiple lines are preserved exactly as written.
```

### Metadata Block (YAML)

The metadata block is parsed as YAML. All fields except `description` are optional.

**Required fields:**
- `description` (string) — Human-readable purpose of this prompt, used for diagnostics

**Optional fields:**
- `applies_to_models` (array of strings) — Model IDs this prompt applies to (see Model-Specific Variations below)
- `author` (string) — Contributor who wrote this prompt
- `updated_at` (string) — Last modification date (ISO 8601 recommended but not enforced)
- `temperature_hint` (number) — Suggested temperature (0.0-2.0)
- `max_tokens_hint` (integer) — Suggested max tokens
- `version` (integer) — Prompt version for tracking evolution

### Prompt Text

Everything after the first `---` separator is the prompt text. It is preserved exactly as written, including:
- Additional `---` sequences (they are part of the prompt, not separators)
- Leading/trailing whitespace
- Blank lines

### Encoding

Files must be UTF-8 encoded. The loader strips UTF-8 BOM if present and normalizes line endings (CRLF → LF) before parsing.

## File Naming Conventions

### Base Prompts (Universal)

A base prompt applies to all models unless a model-specific variation exists:

```
{prompt-id}.prompt
```

Example: `explanation.prompt`

**Requirements:**
- Must NOT have an `applies_to_models` field in metadata (or it should be absent/null)
- Serves as the fallback when no model-specific variation matches

### Model-Specific Variations

Variations target specific models:

```
{prompt-id}.{arbitrary-suffix}.prompt
```

Examples:
- `explanation.opus-tuned.prompt`
- `explanation.01.prompt` (numeric priority, zero-padded)
- `rename.fast-model.prompt`

**Requirements:**
- Must have an `applies_to_models` field with at least one model ID
- The `{arbitrary-suffix}` is cosmetic (for human reference only) and not used for matching

### Variation Selection Algorithm

At runtime, when requesting a prompt for a specific model ID:

1. Find all files matching `{prompt-id}.*.prompt` in this directory
2. Sort files **lexicographically by filename** (ordinal, case-sensitive, using `StringComparer.Ordinal`)
3. For each file in order:
   - Parse metadata block
   - If `applies_to_models` contains the requested model ID (exact, case-sensitive match), return this prompt
4. If no variation matches, return the base prompt (`{prompt-id}.prompt`)
5. If base prompt is missing, fall back to embedded constant in `EmbeddedPrompts.g.cs`

**Important:** Because files are sorted lexicographically, numeric suffixes must be **zero-padded** to assert priority:

```
explanation.01.prompt   (loads before 02)
explanation.02.prompt
explanation.10.prompt   (loads after 02)
```

Without zero-padding, `explanation.10.prompt` would load before `explanation.2.prompt` (lexicographic: "1" < "2").

### Model ID Matching Rules

- **Exact match only** — `claude-opus-5` matches `claude-opus-5` but NOT `claude-opus-5.1` or `claude-opus`
- **Case-sensitive** — `claude-opus-5` does NOT match `CLAUDE-OPUS-5`
- **No wildcards** — `claude-opus-*` is treated as a literal string, not a pattern
- **No whitespace trimming** — `" claude-opus-5 "` (with spaces) will not match `claude-opus-5`

## Current Prompt IDs

| Prompt ID | Consumer Class | Purpose |
|-----------|----------------|---------|
| `explanation` | `AIExplanationService` | Explains decompiled code with reverse-engineering context |
| `rename` | `RenameSuggester` | Suggests meaningful names for obfuscated symbols |
| `chat` | `AIChatPaneModel` | Multi-turn conversational decompilation assistant |
| `security` | `AISecurityAnalyzer` | Identifies security vulnerabilities in decompiled assemblies |
| `search` | `AISearchStrategy` | Natural-language search over assembly symbol vocabulary |
| `assembly_summary` | `AssemblySummaryContextMenuEntry` | Summarizes entire assemblies from metadata |

## Example: Creating a Model-Specific Variation

To create an Opus-optimized explanation prompt:

1. Create `explanation.opus.prompt`:

```yaml
---
description: Explanation prompt optimized for Claude Opus models
applies_to_models: [claude-opus-5, claude-opus-4.8]
author: Your Name
updated_at: 2026-08-19
temperature_hint: 0.2
version: 1
---
You are an expert .NET reverse engineer with deep knowledge of ECMA-335...
[Enhanced prompt text here]
```

2. The base `explanation.prompt` remains as the universal fallback
3. At runtime, when the user selects `claude-opus-5`, the variation is used
4. For any other model (e.g., `deepseek-v4`), the base prompt is used

## Validation and Error Handling

The loader logs warnings for:
- Missing base prompt (falls back to embedded constant)
- Corrupt YAML in metadata block (skips file)
- Invalid metadata values (e.g., `temperature_hint` out of range)
- Empty `applies_to_models` array in a variation file
- Missing `applies_to_models` in a variation file
- `applies_to_models` present in a base file

These are **non-fatal** — the loader continues with warnings and falls back gracefully.

## Editing Workflow

1. Edit `.prompt` files directly in this directory
2. Restart ILSpy to load changes (no hot-reload currently)
3. Verify changes with the target AI feature (Explain Code, Rename Symbol, etc.)
4. Commit changes to version control

## Build Integration

These files are copied to the output directory during build:

```xml
<Content Include="AI\prompts\*.prompt">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</Content>
```

If the directory is missing at runtime, ILSpy falls back to embedded constants in `EmbeddedPrompts.g.cs` (generated at build time from these files).

## Contributing

When adding a new AI feature that requires a system prompt:

1. Add a new base prompt file: `{new-prompt-id}.prompt`
2. Document the prompt ID and consumer class in the table above
3. Update `AIPromptProvider` to recognize the new prompt ID
4. Update the build-time code generator to include the new prompt in `EmbeddedPrompts.g.cs`
5. Add tests in `ICSharpCode.ILSpyX.Tests/AI/AIPromptProviderTests.cs`

## Schema Reference

The metadata block follows standard YAML syntax. Key naming convention is `snake_case` (matching common YAML practice). Arrays use bracket notation: `[item1, item2]` or YAML block style:

```yaml
applies_to_models:
  - claude-opus-5
  - deepseek-v4
```

Both formats are equivalent and supported by YamlDotNet.