# ILSpy AI Features - Detailed Specification

**Version:** 1.0  
**Last Updated:** 2026-08-17  
**Status:** Design Document; Phase 0 foundation implemented, validation pending

---

## Table of Contents

1. [Overview](#overview)
2. [Core Principles](#core-principles)
3. [Feature Details](#feature-details)
   - [Smart Rename Assistant](#smart-rename-assistant)
   - [Code Explanation](#code-explanation)
   - [Assembly Summary](#assembly-summary)
   - [Security Audit Analyzer](#security-audit-analyzer)
   - [AI Chat Assistant](#ai-chat-assistant)
   - [Natural Language Search](#natural-language-search)
   - [Documentation Generator](#documentation-generator)
   - [Code Comparison Intelligence](#code-comparison-intelligence)
4. [User Workflows](#user-workflows)
5. [Privacy & Security](#privacy--security)
6. [Performance Considerations](#performance-considerations)

---

## Overview

ILSpy AI integration brings LLM-powered assistance to .NET decompilation workflows. All features are **BYOK (Bring Your Own Key)** — users provide their own API keys for OpenAI, Anthropic, Ollama, or custom endpoints. No data is sent to any service without explicit user configuration and consent.

**Supported Providers:**
- OpenAI (GPT-4o, GPT-4 Turbo, etc.)
- Anthropic (Claude Opus, Claude Sonnet)
- Ollama (local models: Llama 3, Mistral, etc.)
- Any OpenAI-compatible API endpoint

**Key Differentiator:** Focus on deobfuscation and reverse-engineering assistance — capabilities that require semantic understanding and can't be achieved with static analysis alone.

---

## Core Principles

1. **Privacy-First:** All AI features are opt-in. Users control exactly what context is sent (IL, call graph, string literals).
2. **No Lock-In:** Works with multiple providers. Prefer open weights models where possible.
3. **Offline-Capable:** Ollama support enables fully local operation with no internet connectivity.
4. **Transparent:** Users always know what data is being sent and when.
5. **Augmentation, Not Replacement:** AI assists human understanding; it doesn't automate away the human.
6. **Display-Only:** AI-suggested changes (renames, annotations) are display-time transformations. The original assembly is never modified.

---

## Feature Details

### Smart Rename Assistant (Deobfuscation)

**Problem:** Obfuscated assemblies use meaningless names (`a`, `b`, `method_47`, `Class1234`). Renaming them manually is tedious and requires deep understanding of each symbol's purpose.

**Solution:** AI analyzes the symbol's implementation, usage patterns, and surrounding context to suggest meaningful names.

#### User Experience

**Single Symbol Rename:**
1. User right-clicks an obfuscated method/class/field in the decompiler view or assembly tree
2. Selects "Suggest Name with AI" from context menu
3. A dialog appears showing:
   - Current name: `a`
   - Analyzing spinner (2-5 seconds)
   - Ranked list of suggestions with confidence scores and reasoning:
     ```
     ○ ProcessPayment (95% confident)
       Reason: Calls PaymentGateway.Charge, validates card number, logs transaction
     
     ○ HandleTransaction (82% confident)
       Reason: Contains transaction validation and database commit logic
     
     ○ ValidateAndCharge (78% confident)
       Reason: Performs validation before charging
     ```
4. User selects one and clicks "Apply"
5. The decompiler view refreshes, showing the new name highlighted in a distinct color (e.g., light blue background) with a tooltip indicator: "AI-suggested name"
6. All references to that symbol throughout the assembly also show the new name

**Batch Rename:**
1. User right-clicks a class with many obfuscated members
2. Selects "Batch Rename All Members with AI"
3. A progress dialog shows:
   ```
   Analyzing class MyClass (47 members)
   
   [■■■■■■■■■□□□□□□□□□□□] 45%
   
   Processing: method_23 → ValidateInput
   Completed: 21/47
   
   [Cancel]
   ```
4. When complete, shows a review dialog with all suggestions:
   ```
   ☑ field_1 → paymentGateway (95%)
   ☑ field_2 → connectionString (88%)
   ☑ method_1 → Initialize (92%)
   ☑ method_2 → Dispose (89%)
   ☐ method_3 → ProcessData (62%)  [low confidence, unchecked by default]
   
   [Select All] [Select None] [Apply Selected (42)] [Cancel]
   ```
5. User can review, toggle individual suggestions, then apply all selected at once

#### Context Sent to LLM

For a method being renamed:
- **Method signature:** Return type, parameter types and names, visibility
- **Decompiled body:** Full C# code (truncated if exceeds token budget)
- **String literals:** All string constants used in the method
- **Attributes:** `[Obsolete]`, `[HttpGet]`, `[Serializable]`, etc.
- **Interfaces implemented:** If method implements an interface method
- **Callers:** Names of up to 10 methods that call this one
- **Callees:** Names of up to 10 methods that this one calls
- **Return type usage:** What the return value is used for in calling code

Example prompt:
```
You are analyzing obfuscated .NET code. Suggest a meaningful name for this method.

**Current name:** a
**Signature:** public bool a(string b, decimal c)
**Implements:** IPaymentProcessor.ProcessPayment

**Decompiled code:**
```csharp
public bool a(string b, decimal c)
{
    if (string.IsNullOrEmpty(b) || c <= 0m)
        return false;
    
    try
    {
        PaymentGateway gateway = new PaymentGateway();
        TransactionResult result = gateway.Charge(b, c);
        Logger.Log("Transaction completed: " + result.TransactionId);
        return result.Success;
    }
    catch (Exception ex)
    {
        Logger.LogError("Payment failed", ex);
        return false;
    }
}
```

**Called by:** CheckoutController.FinalizeOrder, OrderService.ProcessOrder
**Calls:** PaymentGateway.Charge, Logger.Log, Logger.LogError
**String literals:** "Transaction completed: ", "Payment failed"

Return 3-5 name suggestions in JSON format:
[
  {"name": "...", "confidence": 0.0-1.0, "reasoning": "..."},
  ...
]
```

#### Annotation Storage

Renamed symbols are stored in a sidecar file alongside the assembly:

**File:** `MyAssembly.dll.ilspy-annotations.json`

```json
{
  "assemblyHash": "sha256:a1b2c3d4...",
  "version": "1.0",
  "annotations": [
    {
      "token": "0x06000042",
      "kind": "method",
      "originalName": "a",
      "suggestedName": "ProcessPayment",
      "confidence": 0.95,
      "approvedBy": "user",
      "timestamp": "2026-08-17T14:30:00Z"
    },
    {
      "token": "0x04000015",
      "kind": "field",
      "originalName": "field_1",
      "suggestedName": "paymentGateway",
      "confidence": 0.88,
      "approvedBy": "user",
      "timestamp": "2026-08-17T14:32:15Z"
    }
  ]
}
```

- **assemblyHash:** SHA256 of the assembly file. If hash mismatches, annotations are ignored (prevents applying renames to wrong version).
- **token:** Metadata token (unique identifier that survives reassembly).
- **approvedBy:** "user" (manually approved), "ai-auto" (batch-applied above threshold).

#### Display-Time Application

Renames are applied during decompilation:
1. When `CSharpDecompiler` generates output for an entity, it checks the annotation store
2. If an annotation exists for that token, the suggested name is used instead of the original
3. Visual indicator added: light blue highlight + tooltip "AI-suggested: ProcessPayment (95% confident)"
4. All references throughout the codebase also use the suggested name
5. Original assembly file is never modified

#### Edge Cases

- **Name collisions:** If suggested name already exists in scope, append suffix: `ProcessPayment_2`
- **Invalid names:** If LLM suggests invalid C# identifier, sanitize or fall back to `OriginalName_Suggested`
- **Low confidence:** Suggestions below 60% confidence are marked "Review Required" in batch mode
- **Context window overflow:** For huge methods (>100k tokens), truncate body and note: "Context truncated due to size"

---

### Code Explanation

**Problem:** Decompiled code can be cryptic, especially with obfuscation, unfamiliar libraries, or complex algorithms. Understanding what a method does requires tracing through call sites, reading docs, and mental execution.

**Solution:** AI generates plain-English explanations of code purpose, algorithm, and gotchas.

#### User Experience

1. User right-clicks any symbol (method, class, property, field) in the decompiler view or assembly tree
2. Selects "Explain with AI"
3. The AI Output pane slides open (or gains focus if already open) at the bottom of the window
4. Explanation streams in with typewriter effect (shows first sentence in ~500ms):
   ```
   Explaining: MyClass.ProcessPayment
   
   ▼ Purpose
   This method processes a payment transaction by validating the input, 
   calling an external payment gateway, and logging the result.
   
   ▼ Algorithm
   1. Validates that the card number is non-empty and amount is positive
   2. Creates a PaymentGateway instance and calls its Charge method
   3. Logs the transaction ID on success
   4. Returns true if charge succeeds, false otherwise
   5. Catches exceptions and logs errors
   
   ▼ Key Details
   • Uses PaymentGateway.Charge for actual processing
   • No retry logic - single attempt only
   • Logs errors but does not rethrow exceptions
   • Returns false for both validation failures and exceptions
   
   ▼ Potential Issues
   ⚠ No retry on transient failures (network timeout, gateway busy)
   ⚠ Swallows exceptions - caller cannot distinguish validation vs. gateway failure
   ⚠ Creates new PaymentGateway instance per call (no connection pooling)
   
   [Copy to Clipboard] [Ask Follow-Up]
   ```

4. User can:
   - Copy the entire explanation to clipboard
   - Click "Ask Follow-Up" to open the chat pane with context pre-loaded
   - Close the pane and continue browsing

#### Context Sent to LLM

Same as rename assistant context, but optimized for explanation rather than naming:
- Decompiled C# code
- Method signature
- Attributes
- Interfaces implemented
- Callers/callees (optional, if `SendCallGraph` enabled)
- String literals (optional)
- IL bytecode (optional, if `SendIL` enabled)

Example prompt:
```
You are explaining decompiled .NET code to a reverse engineer. Provide a clear, 
structured explanation.

**Code:**
```csharp
public bool ProcessPayment(string cardNumber, decimal amount)
{
    if (string.IsNullOrEmpty(cardNumber) || amount <= 0m)
        return false;
    
    try
    {
        PaymentGateway gateway = new PaymentGateway();
        TransactionResult result = gateway.Charge(cardNumber, amount);
        Logger.Log("Transaction completed: " + result.TransactionId);
        return result.Success;
    }
    catch (Exception ex)
    {
        Logger.LogError("Payment failed", ex);
        return false;
    }
}
```

**Called by:** CheckoutController.FinalizeOrder (2 call sites)
**Calls:** PaymentGateway.Charge, Logger.Log, Logger.LogError

Format your response as:

# Purpose
[One sentence summary]

# Algorithm
[Step-by-step what the code does]

# Key Details
[Important implementation notes]

# Potential Issues
[Security concerns, bugs, performance problems, or gotchas]
```

#### UI Components

**AI Output Pane:**
- Dockable panel (like Analyzers pane)
- Position: below decompiler text view by default
- Header: "AI Assistant" with symbol name being explained
- Body: Markdown-formatted response with syntax highlighting for code blocks
- Footer: [Copy] [Clear] [Ask Follow-Up] buttons
- Persists dock position in session settings

**Streaming Display:**
- Text appears chunk-by-chunk (typewriter effect)
- User can scroll while streaming continues
- Cancel button visible during streaming
- After completion, [Copy] and [Ask Follow-Up] buttons appear

---

### Assembly Summary

**Problem:** When opening an unknown assembly, engineers spend 10-30 minutes browsing namespaces, reading type names, and inferring purpose before understanding what it does.

**Solution:** AI generates a 2-3 paragraph summary by analyzing assembly metadata, public API surface, and entry points.

#### User Experience

1. User right-clicks an assembly node in the tree
2. Selects "Summarize Assembly with AI"
3. AI Output pane opens, shows:
   ```
   Analyzing assembly: MyCompany.PaymentService.dll
   
   [■■■■■■■■■■] Analyzing...
   
   ▼ Assembly Information
   Name: MyCompany.PaymentService
   Version: 2.3.1.0
   Target Framework: .NET 6.0
   
   ▼ Summary
   This assembly implements a payment processing service for e-commerce applications.
   It provides integration with multiple payment gateways (Stripe, PayPal, Square) 
   through a unified IPaymentProcessor interface. The main entry point is 
   PaymentService.ProcessTransaction, which routes payments to the appropriate 
   gateway based on configuration.
   
   The assembly includes three main subsystems:
   1. Payment processing (MyCompany.PaymentService.Processors namespace)
   2. Transaction logging and audit (MyCompany.PaymentService.Audit)
   3. Configuration and gateway management (MyCompany.PaymentService.Config)
   
   Notable dependencies include Newtonsoft.Json for serialization, 
   System.Net.Http for API calls, and Entity Framework Core for database access.
   The assembly appears to target enterprise use cases, with extensive logging,
   retry logic, and support for both synchronous and asynchronous workflows.
   
   ▼ Public API Surface
   • 3 public classes
   • 12 public methods
   • 2 interfaces: IPaymentProcessor, ITransactionLogger
   • Entry point: PaymentService.ProcessTransaction
   
   ▼ Key Types
   • PaymentService - Main orchestrator
   • StripeProcessor, PayPalProcessor, SquareProcessor - Gateway implementations
   • TransactionLogger - Audit logging
   • PaymentConfiguration - Settings management
   
   [Copy Summary] [Explore Types]
   ```

#### Context Sent to LLM

- **Assembly metadata:** Name, version, target framework, culture, public key token
- **Assembly attributes:** `[assembly: Description]`, `[assembly: Product]`, `[assembly: Company]`, etc.
- **Top-level namespaces:** List of root namespaces (e.g., `MyCompany.PaymentService.*`)
- **Public types:** Names of all public classes, interfaces, enums (up to 50)
- **Type relationships:** Base classes and interfaces for public types
- **Entry point:** If executable, the `Main` method signature
- **Referenced assemblies:** Direct dependencies (name + version)
- **Top 10 largest methods:** By IL size (often reveals core functionality)
- **Embedded resources:** Count and types (e.g., "5 .resx files, 2 .png images")

Example prompt:
```
You are analyzing a .NET assembly. Provide a concise summary of what this assembly 
does, its architecture, and its likely use case.

**Assembly:** MyCompany.PaymentService, Version=2.3.1.0
**Target Framework:** net6.0
**Attributes:** 
- Product: "Payment Service"
- Company: "MyCompany"
- Description: "Multi-gateway payment processing"

**Namespaces:**
- MyCompany.PaymentService
- MyCompany.PaymentService.Processors
- MyCompany.PaymentService.Audit
- MyCompany.PaymentService.Config

**Public Types (12):**
- PaymentService : IPaymentProcessor
- StripeProcessor : IPaymentProcessor
- PayPalProcessor : IPaymentProcessor
- SquareProcessor : IPaymentProcessor
- TransactionLogger : ITransactionLogger
- PaymentConfiguration
- TransactionResult
- PaymentException : Exception
- ... (4 more)

**Entry Point:** None (class library)

**Dependencies:**
- Newtonsoft.Json 13.0.1
- System.Net.Http 6.0.0
- Microsoft.EntityFrameworkCore 6.0.8

**Largest Methods:**
1. PaymentService.ProcessTransaction (482 IL bytes)
2. StripeProcessor.Charge (318 IL bytes)
3. TransactionLogger.LogTransaction (256 IL bytes)

Provide:
1. A 2-3 paragraph summary of the assembly's purpose and architecture
2. What it's likely used for
3. Key subsystems or components
```

---

### Security Audit Analyzer

**Problem:** Manual security review of decompiled assemblies is time-consuming and error-prone. Common vulnerabilities (SQL injection, hardcoded credentials, weak crypto) are tedious to find by hand.

**Solution:** AI-powered analyzer that scans for security issues and displays them in the Analyzer pane.

#### User Experience

1. User selects an assembly or type in the tree
2. Right-clicks → "Analyze" → "Security Risks (AI)"
3. Analyzer pane populates with a tree structure:
   ```
   ▼ Security Risks (AI) - 12 issues found
     ▼ Critical (2)
       ▶ SQL Injection Risk - CustomerRepository.GetByName
       ▶ Hardcoded Credential - DatabaseConfig.ConnectionString
     ▼ High (5)
       ▶ Weak Cryptography (MD5) - PasswordHasher.Hash
       ▶ Deserialization of Untrusted Data - ApiController.ProcessRequest
       ▶ Path Traversal Risk - FileManager.LoadFile
       ▶ Insecure Random - TokenGenerator.GenerateToken
       ▶ Dangerous P/Invoke - NativeMethods.ExecuteCommand
     ▼ Medium (5)
       ▶ Missing Input Validation - PaymentService.ProcessPayment
       ▶ Information Disclosure - ErrorHandler.DisplayError
       ...
   ```
4. User clicks on an issue → decompiler view jumps to the offending code
5. The specific line is highlighted with a red squiggly underline
6. Hover shows tooltip:
   ```
   ⚠ Security Risk: SQL Injection
   
   This method concatenates user input directly into a SQL query without 
   parameterization. An attacker can inject arbitrary SQL commands.
   
   Vulnerable line:
   string sql = "SELECT * FROM Customers WHERE Name = '" + userName + "'";
   
   Recommendation:
   Use parameterized queries or an ORM like Entity Framework to prevent injection.
   ```

#### Patterns Detected

**Critical Severity:**
- SQL injection (string concatenation in SQL queries)
- Command injection (user input to `Process.Start`)
- Hardcoded credentials (literal strings matching password patterns)
- Deserialization of untrusted data (`BinaryFormatter`, `NetDataContractSerializer` on external input)
- Path traversal (user input in file paths without validation)

**High Severity:**
- Weak cryptography (MD5, SHA1 for passwords; DES, RC4; ECB mode)
- Insecure random (`Random` class for security-sensitive values)
- Dangerous P/Invoke (kernel32 APIs, unmanaged memory operations)
- Missing authentication checks (public endpoints without `[Authorize]`)
- Information disclosure (exception details in responses, connection strings in logs)

**Medium Severity:**
- Missing input validation (no null checks, no length limits)
- Weak TLS (TLS 1.0/1.1, `ServicePointManager.SecurityProtocol` set incorrectly)
- Insecure defaults (HTTP instead of HTTPS, permissive CORS)
- Missing rate limiting (no throttling on authentication endpoints)

**Low Severity:**
- Missing XML comment warnings
- Unused code (dead methods detected by AI)
- Performance anti-patterns (N+1 queries, boxing in hot loops)

#### Context Sent to LLM

Per-type analysis:
- Decompiled C# for all methods in the type
- String literals (especially important for SQL/command injection detection)
- Method calls to known-dangerous APIs (Process.Start, BinaryFormatter, etc.)
- Attributes (looking for `[AllowAnonymous]`, `[HttpGet]`, etc.)

Batch processing:
- Analyze one type at a time (avoid token limit blow-up)
- Stream results as they arrive
- User can cancel mid-analysis

Example prompt:
```
You are a security auditor reviewing decompiled .NET code. Identify security 
vulnerabilities and return them in JSON format.

**Type:** CustomerRepository

**Code:**
```csharp
public class CustomerRepository
{
    private readonly string connectionString = "Server=...;Password=Admin123;";
    
    public Customer GetByName(string userName)
    {
        string sql = "SELECT * FROM Customers WHERE Name = '" + userName + "'";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            // ... read results
        }
    }
    
    public void HashPassword(string password)
    {
        MD5 md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }
}
```

Return JSON array:
[
  {
    "type": "CustomerRepository",
    "method": "GetByName",
    "issue": "SQL Injection",
    "severity": "Critical",
    "line": "string sql = ...",
    "description": "...",
    "recommendation": "..."
  },
  ...
]
```

#### Implementation Notes

- Implements `IAnalyzer` interface (MEF-exported)
- Shows in Analyzer pane alongside existing analyzers
- Results are `SearchResult` objects that navigate to code on click
- Severity levels map to icon colors: 🔴 Critical, 🟠 High, 🟡 Medium, 🔵 Low
- Confidence threshold: only emit issues above 70% confidence

---

### AI Chat Assistant

**Problem:** Explaining and analyzing code is often iterative. A single explanation may lead to follow-up questions: "Why does it use this algorithm?", "What happens if X is null?", "Is this thread-safe?"

**Solution:** A persistent chat interface where users can ask questions about the currently-viewed assembly, with full conversation history.

#### User Experience

1. User opens AI chat pane via View → AI Assistant (or keyboard shortcut)
2. Pane shows:
   ```
   ┌─────────────────────────────────────────────┐
   │ AI Assistant                          [×][⚙] │
   ├─────────────────────────────────────────────┤
   │ Currently viewing: PaymentService.dll       │
   │                                             │
   │ ┌─────────────────────────────────────────┐ │
   │ │ 🤖 Hi! I can help you understand this  │ │
   │ │    assembly. Ask me anything about the │ │
   │ │    code you're viewing.                │ │
   │ │                                         │ │
   │ │    Try: /explain PaymentService.Process│ │
   │ │         /audit                          │ │
   │ │         /summary                        │ │
   │ └─────────────────────────────────────────┘ │
   │                                             │
   │ [Type a message...              ] [Send ⮕] │
   └─────────────────────────────────────────────┘
   ```

3. User types question: "What does PaymentService.ProcessTransaction do?"
4. Message appears as user bubble:
   ```
   │ ┌─────────────────────────────────────────┐ │
   │ │ You: What does PaymentService.Process   │ │
   │ │      Transaction do?                    │ │
   │ └─────────────────────────────────────────┘ │
   ```

5. Response streams in:
   ```
   │ ┌─────────────────────────────────────────┐ │
   │ │ 🤖 PaymentService.ProcessTransaction is │ │
   │ │    the main entry point for processing  │ │
   │ │    payments. It:                        │ │
   │ │    1. Validates the payment request     │ │
   │ │    2. Routes to the appropriate gateway │ │
   │ │    3. Logs the transaction              │ │
   │ │    4. Returns the result                │ │
   │ │                                         │ │
   │ │    It supports Stripe, PayPal, and     │ │
   │ │    Square as payment gateways.          │ │
   │ └─────────────────────────────────────────┘ │
   ```

6. User asks follow-up: "How does it choose which gateway to use?"
7. AI responds with context from previous messages:
   ```
   │ ┌─────────────────────────────────────────┐ │
   │ │ 🤖 It reads the gateway type from       │ │
   │ │    PaymentConfiguration.DefaultGateway. │ │
   │ │    The code checks:                     │ │
   │ │    ```csharp                            │ │
   │ │    switch (config.DefaultGateway) {     │ │
   │ │        case "Stripe": ...               │ │
   │ │        case "PayPal": ...               │ │
   │ │        ...                              │ │
   │ │    }                                    │ │
   │ │    ```                                  │ │
   │ └─────────────────────────────────────────┘ │
   ```

#### Slash Commands

Quick shortcuts for common operations:
- `/explain <symbol>` - Explain a type/method
- `/rename <symbol>` - Suggest rename for a symbol
- `/audit` - Run security audit on current assembly
- `/summary` - Generate assembly summary
- `/clear` - Clear conversation history
- `/help` - Show available commands

Example:
```
You: /explain PaymentService.ProcessTransaction

🤖 [Fetching code...]

🤖 PaymentService.ProcessTransaction is the main entry point...
   [full explanation as in Code Explanation feature]

You: Is it thread-safe?

🤖 No, ProcessTransaction is not thread-safe. It creates a new 
   PaymentGateway instance per call, which is safe, but the 
   TransactionLogger it uses may have shared state...
```

#### Context Auto-Injection

Every user message automatically includes:
- **Currently viewed symbol:** If user has a method/type selected in decompiler view
- **Assembly name:** Always included
- **Recent navigation:** Last 3 symbols the user viewed (helps with "it" and "that" references)

Example auto-injected context:
```
[System context - not shown to user]
Currently viewing: PaymentService.ProcessTransaction (method)
Assembly: MyCompany.PaymentService.dll
Recent navigation: 
  - PaymentService.ProcessTransaction
  - StripeProcessor.Charge
  - TransactionLogger.LogTransaction

User message: "Is it thread-safe?"
```

#### Conversation Persistence

- Conversation history stored per assembly in `.ilspy-chat-history.json`
- Loaded when assembly opens
- Saved on app exit or when conversation cleared
- Max 100 messages (oldest pruned when limit exceeded)
- Export conversation to Markdown file via [⚙] menu → "Export Conversation"

Format:
```json
{
  "assemblyHash": "sha256:a1b2c3d4...",
  "messages": [
    {
      "timestamp": "2026-08-17T14:30:00Z",
      "role": "user",
      "content": "What does PaymentService.ProcessTransaction do?"
    },
    {
      "timestamp": "2026-08-17T14:30:15Z",
      "role": "assistant",
      "content": "PaymentService.ProcessTransaction is..."
    }
  ]
}
```

---

### Natural Language Search

**Problem:** ILSpy's search requires knowing method/type names. Finding "methods that call the database" or "code that sends HTTP requests" requires manual browsing or complex regex patterns.

**Solution:** Natural language search that understands intent and returns semantically relevant results.

#### User Experience

1. User opens search pane (Ctrl+F as usual)
2. New toggle appears: `[○ Literal] [● AI Search]`
3. User types natural language query:
   ```
   ┌─────────────────────────────────────────────┐
   │ Search: methods that send HTTP requests     │
   │ [○ Literal] [● AI Search]            [Go]   │
   └─────────────────────────────────────────────┘
   ```

4. Search results populate with confidence scores:
   ```
   ▼ AI Search Results (8 matches)
     PaymentService.ProcessTransaction (95%)
       └─ Calls HttpClient.PostAsync to send payment data
     
     ApiClient.SendRequest (92%)
       └─ Primary HTTP client wrapper, uses HttpClient
     
     WebhookManager.NotifyEndpoint (87%)
       └─ Sends POST requests to webhook URLs
     
     HealthChecker.PingService (81%)
       └─ Performs HTTP GET to check service availability
     
     ...
   ```

5. Click a result → navigates to that method in decompiler view
6. Result includes snippet highlighting why it matched

#### How It Works

**Approach 1: LLM-Based (Phase 4.5)**
1. User types query
2. Context builder samples ~50 random methods as "vocabulary"
3. LLM receives: query + vocabulary sample + full list of method signatures
4. LLM returns: JSON array of FQNs + confidence scores + reasoning
5. UI displays results ranked by confidence

**Approach 2: Embedding-Based (Phase 4.6)**
1. On assembly load, decompile all methods (background thread)
2. Compute embeddings for each method (via OpenAI `text-embedding-3-small` or local model)
3. Store in SQLite: `embeddings(token TEXT PRIMARY KEY, vector BLOB)`
4. User types query
5. Embed query, compute cosine similarity against all stored vectors
6. Return top-k results
7. Optional: re-rank top-k with LLM for better precision

Example prompt (Approach 1):
```
You are analyzing a .NET assembly. The user wants to find methods matching this 
natural language query.

**Query:** "methods that send HTTP requests"

**Assembly:** MyCompany.PaymentService.dll

**Sample methods (50 random):**
- PaymentService.ProcessTransaction
- StripeProcessor.Charge
- PayPalProcessor.Charge
- TransactionLogger.LogTransaction
- ApiClient.SendRequest
- ...

**All method signatures (300 total):**
- PaymentService.ProcessTransaction(PaymentRequest) : TransactionResult
- StripeProcessor.Charge(string, decimal) : bool
- ApiClient.SendRequest(HttpMethod, string, object) : HttpResponseMessage
- ...

Return JSON array of matches with confidence:
[
  {
    "fullyQualifiedName": "ApiClient.SendRequest",
    "confidence": 0.95,
    "reasoning": "Directly sends HTTP requests via HttpClient"
  },
  ...
]
```

---

### Documentation Generator

**Problem:** Decompiled code has no XML documentation comments. When building wrappers or understanding complex APIs, engineers must write their own docs from scratch.

**Solution:** AI generates XML documentation comments (`<summary>`, `<param>`, `<returns>`, `<exception>`) based on implementation.

#### User Experience

1. User right-clicks a type or method
2. Selects "Generate XML Documentation"
3. AI Output pane shows generated docs:
   ```
   Generated documentation for: PaymentService.ProcessTransaction
   
   /// <summary>
   /// Processes a payment transaction through the configured payment gateway.
   /// Validates the request, routes to the appropriate gateway (Stripe, PayPal, 
   /// or Square), logs the transaction, and returns the result.
   /// </summary>
   /// <param name="request">The payment request containing card details and amount.</param>
   /// <returns>
   /// A <see cref="TransactionResult"/> indicating success or failure, including
   /// the transaction ID and any error messages.
   /// </returns>
   /// <exception cref="ArgumentNullException">
   /// Thrown when <paramref name="request"/> is null.
   /// </exception>
   /// <exception cref="InvalidOperationException">
   /// Thrown when no payment gateway is configured.
   /// </exception>
   /// <remarks>
   /// This method is thread-safe and can be called concurrently. Each call creates
   /// a new gateway instance to avoid shared state issues.
   /// </remarks>
   
   [Copy Documentation] [Insert as Comment]
   ```

4. User clicks "Copy Documentation" → XML comments copied to clipboard
5. User clicks "Insert as Comment" → comments appear above the method in decompiler view (display-only, not saved to assembly)

#### Context Sent to LLM

- Method signature (return type, parameters with types)
- Decompiled body
- Exceptions thrown (detected by scanning for `throw` statements)
- Return statements (what values are returned under what conditions)
- Attributes (e.g., `[Obsolete("reason")]`)

Example prompt:
```
Generate XML documentation comments for this C# method.

```csharp
public TransactionResult ProcessTransaction(PaymentRequest request)
{
    if (request == null)
        throw new ArgumentNullException(nameof(request));
    
    if (config.DefaultGateway == null)
        throw new InvalidOperationException("No gateway configured");
    
    IPaymentProcessor processor = GetProcessor(config.DefaultGateway);
    TransactionResult result = processor.Charge(request.CardNumber, request.Amount);
    
    logger.LogTransaction(result);
    
    return result;
}
```

Return only the XML documentation in this format:
/// <summary>...</summary>
/// <param name="...">...</param>
/// <returns>...</returns>
/// <exception cref="...">...</exception>
/// <remarks>...</remarks>
```

---

### Code Comparison Intelligence

**Problem:** Comparing two versions of an assembly (e.g., v1.2 vs v1.3) to understand what changed requires manual diff navigation and interpretation.

**Solution:** AI generates a high-level summary of differences between two assembly versions.

#### User Experience

1. User loads two versions of an assembly (e.g., drag both DLLs into ILSpy)
2. Selects both in the assembly tree (Ctrl+Click)
3. Right-clicks → "Compare Assemblies with AI"
4. AI Output pane shows:
   ```
   Comparing: PaymentService v2.3.0 → v2.4.0
   
   [■■■■■■■■■■] Analyzing differences...
   
   ▼ Summary of Changes
   
   Version 2.4.0 introduces improved error handling and adds support for a new
   payment gateway (Square). Key changes:
   
   ▼ New Features (3)
   • Added SquareProcessor class implementing IPaymentProcessor
   • Added retry logic to PaymentService.ProcessTransaction (max 3 attempts)
   • Added TransactionResult.RetryCount property
   
   ▼ Breaking Changes (1)
   ⚠ PaymentService.ProcessTransaction signature changed:
     Old: ProcessTransaction(PaymentRequest request)
     New: ProcessTransaction(PaymentRequest request, RetryPolicy policy)
   
   ▼ Bug Fixes (2)
   • Fixed null reference exception in StripeProcessor.Charge when card number is empty
   • Fixed connection leak in TransactionLogger (now uses 'using' statement)
   
   ▼ Internal Changes (5)
   • Refactored PaymentConfiguration to use Options pattern
   • Improved logging with structured logging (Serilog)
   • Updated Entity Framework Core from 6.0.8 to 6.0.15
   • Removed deprecated PayPalLegacyProcessor class
   • Performance optimization in TransactionLogger (bulk inserts)
   
   ▼ Unchanged (8 classes)
   • ApiClient
   • ErrorHandler
   • ...
   
   [Export Report] [View Detailed Diff]
   ```

#### How It Works

1. Decompile both assemblies with ILSpy's existing diff-friendly mode (normalized IL via `ILAstLanguage`)
2. Compute structural diff:
   - Types added/removed/changed
   - Members added/removed/changed
   - Signature changes
   - IL-level differences
3. Send diff summary to LLM (not full code, just change list)
4. LLM categorizes changes and generates narrative summary

Example context sent:
```
Compare two versions of a .NET assembly and summarize the changes.

**Version 1:** PaymentService 2.3.0
**Version 2:** PaymentService 2.4.0

**Types Added:**
- SquareProcessor : IPaymentProcessor

**Types Removed:**
- PayPalLegacyProcessor

**Types Changed:**
- PaymentService
  - Method changed: ProcessTransaction
    - Old signature: ProcessTransaction(PaymentRequest)
    - New signature: ProcessTransaction(PaymentRequest, RetryPolicy)
    - IL size: 482 bytes → 618 bytes (28% larger)
  - Method added: RetryWithBackoff(Func<TransactionResult>, RetryPolicy)

- StripeProcessor
  - Method changed: Charge
    - Added null check at beginning (6 new IL instructions)

- TransactionLogger
  - Method changed: LogTransaction
    - Now uses 'using' statement (IDisposable pattern)

**Dependencies Changed:**
- Entity Framework Core: 6.0.8 → 6.0.15

Provide:
1. High-level summary of changes (2-3 sentences)
2. Categorized list: New Features, Breaking Changes, Bug Fixes, Internal Changes
3. Note any potential compatibility issues
```

---

## User Workflows

### Workflow 1: Reverse Engineering an Obfuscated Assembly

1. **Load assembly** → ILSpy shows obfuscated names everywhere
2. **Generate summary** → Right-click assembly → "Summarize with AI" → understand overall purpose
3. **Find entry point** → AI summary tells you the main class/method
4. **Explain entry point** → Right-click entry method → "Explain with AI" → understand what it does
5. **Batch rename** → Right-click main class → "Batch Rename All Members with AI" → get meaningful names
6. **Trace dependencies** → Click through renamed methods, use AI explanations for complex ones
7. **Security audit** → Right-click assembly → "Analyze Security Risks" → identify vulnerabilities
8. **Export annotations** → Save `.ilspy-annotations.json` for team members to use

### Workflow 2: Understanding a Third-Party Library

1. **Load library** → Open NuGet package DLL
2. **Generate summary** → Understand what the library does and its architecture
3. **Explore API** → Browse public types, explain key classes with AI
4. **Generate docs** → Right-click public methods → "Generate XML Documentation" → copy for wrapper code
5. **Ask questions** → Open chat pane, ask "How do I initialize this library?" or "What's the recommended pattern for X?"

### Workflow 3: Security Review

1. **Load suspicious assembly** → Client sends DLL for analysis
2. **Security audit** → Right-click assembly → "Analyze Security Risks (AI)" → get initial findings
3. **Review critical issues** → Click through each critical/high-severity finding
4. **Explain context** → For each finding, explain the surrounding code to understand exploitability
5. **Chat for details** → Ask follow-up questions: "Can this SQL injection be exploited if user input is validated elsewhere?"
6. **Export report** → Copy security findings + AI explanations into report for client

### Workflow 4: Comparing Versions

1. **Load both versions** → Drag v1.2.dll and v1.3.dll into ILSpy
2. **Compare** → Select both, right-click → "Compare Assemblies with AI"
3. **Review breaking changes** → Identify any signature changes that affect your code
4. **Understand new features** → Explain new methods to decide if worth upgrading
5. **Export comparison** → Save AI comparison summary for team discussion

---

## Privacy & Security

### What Gets Sent to LLM Providers

**Always sent (when using AI features):**
- Decompiled C# code of selected symbols
- Method signatures, type names
- Assembly metadata (name, version, target framework)

**Opt-in (user configures in settings):**
- IL bytecode (`SendIL` setting)
- Call graph (callers/callees) (`SendCallGraph` setting)
- String literals from method bodies (implicitly included in C# code)

**Never sent:**
- Full assemblies (only selected symbols)
- User's file paths or directory structure
- User's API key to us (stored locally, sent directly to provider)
- Any data when AI features are not in use

### Data Retention by Providers

ILSpy has no control over how third-party providers (OpenAI, Anthropic) store or use data. Users should:
- Review their chosen provider's data retention policy
- Use Ollama for fully local, offline operation if confidentiality is critical
- Avoid using AI features on highly sensitive proprietary code without legal review

### Settings Transparency

The AI settings panel prominently displays:
```
⚠ PRIVACY NOTICE

When you use AI features, decompiled code is sent to your chosen provider
(OpenAI, Anthropic, or custom endpoint). 

What is sent:
✓ Decompiled C# code of the symbol you're analyzing
✓ Method signatures and assembly metadata
✓ IL bytecode (if enabled below)
✓ Call graph data (if enabled below)

What is NOT sent:
✗ Full assemblies
✗ Your file paths
✗ Any data when AI features are not in use

Your API key is stored securely on your device and never sent to ILSpy developers.

[ ] I understand and accept these privacy terms
```

Checkbox must be checked before AI features activate. The persisted foundation setting is `AISettings.PrivacyConsentAccepted`, defaults to `false`, and must gate every user-facing AI action.

### Secure Key Storage

- Windows: DPAPI (`ProtectedData.Protect`) with `CurrentUser` scope
- macOS: Keychain via native Security framework APIs
- Linux: Secret Service (`secret-tool`); if unavailable, report secure storage as unavailable
- No application-managed file fallback unless a future design provides a platform-protected encryption key
- API keys never stored in plain text
- Keys never appear in logs or error messages

---

## Performance Considerations

### Token Budget Management

- Default context window: 32k tokens
- Allocation: 2k system prompt + 4k conversation history + 8k code context + 2k response budget
- Trimming order when over budget: IL → callees → callers → string literals → C# body (truncated at statement boundaries)

### Rate Limiting

- ILSpy does not enforce rate limits (provider's responsibility)
- Users are warned about costs in settings: "AI requests cost money. OpenAI GPT-4o: ~$0.01 per explanation."
- Batch operations (batch rename, security audit) process one item at a time with progress dialog (user can cancel)

### Caching

- **Explanation cache:** Explanations cached in memory for current session (keyed by metadata token + settings hash)
- **Embedding cache:** Pre-computed embeddings stored in SQLite, recomputed only if assembly changes
- **Annotation cache:** Renames loaded once per assembly, applied during decompilation

### Background Processing

- Security audit runs on background thread (does not block UI)
- Embedding computation runs on background thread with progress notification
- Streaming responses arrive on background thread, dispatched to UI thread in chunks

### Offline Operation

- All features except AI work without API key configured
- Ollama models run fully offline (no internet required)
- Annotation files and chat history stored locally, work offline

---

## Future Enhancements (Post Phase 4)

### Smart Bookmarks
- When user bookmarks a method, AI auto-generates 5-word label
- Bookmarks become searchable by semantic meaning

### License Scanner
- Detect copied code patterns from known open-source libraries
- Flag GPL-licensed code in commercial assemblies

### Performance Pattern Detector
- Spot allocations in hot loops, repeated LINQ queries, synchronous I/O in async methods
- Analyzer-style results with severity levels

### Code Smells Analyzer
- Detect anti-patterns: God classes, long methods, feature envy, etc.
- Display as analyzer results

### Cross-Assembly Analysis
- "Find all assemblies that depend on this type"
- "Show me everywhere this vulnerability pattern appears across all loaded assemblies"

### AI-Assisted Debugging
- Explain why an exception might be thrown at a specific line
- Trace data flow backward from a crash site

---

**Document Version:** 1.0  
**Last Updated:** 2026-08-17  
**Status:** Design Document - Phase 0 implemented; later phases subject to change
