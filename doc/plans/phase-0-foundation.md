# Phase 0: Foundation - Detailed Implementation Plan

**Goal:** Build the shared infrastructure that all AI features depend on  
**Estimated effort:** 2-3 weeks  
**Dependencies:** None  
**Completion criteria:** All foundation components are unit-tested and ready for Phase 1 consumption

---

## Task 0.1: Token Counter Utility ⭐

**Files to create:**
- `ICSharpCode.ILSpyX/AI/TokenCounter.cs`
- `ICSharpCode.ILSpyX.Tests/AI/TokenCounterTests.cs`

### Implementation Details

```csharp
// ICSharpCode.ILSpyX/AI/TokenCounter.cs
namespace ICSharpCode.ILSpyX.AI
{
    /// <summary>
    /// Provides approximate token counting for LLM context budget management.
    /// Uses heuristics (4 chars ≈ 1 token for English prose, 3 chars ≈ 1 token for code).
    /// Accuracy is sufficient for budget decisions; not meant to match tiktoken exactly.
    /// </summary>
    public static class TokenCounter
    {
        /// <summary>
        /// Estimates token count for the given text.
        /// </summary>
        /// <param name="text">The text to count tokens for.</param>
        /// <param name="isCode">True if text is code (uses 3:1 ratio), false for prose (4:1 ratio).</param>
        /// <returns>Approximate token count.</returns>
        public static int CountTokens(string text, bool isCode = true)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
            
            int charCount = text.Length;
            int divisor = isCode ? 3 : 4;
            
            // Add extra tokens for newlines (each line boundary ~= 1 token)
            int lineCount = text.Count(c => c == '\n') + 1;
            
            return (charCount / divisor) + lineCount;
        }
        
        /// <summary>
        /// Truncates text to fit within target token budget, breaking at statement boundaries.
        /// </summary>
        /// <param name="text">The text to truncate.</param>
        /// <param name="maxTokens">Maximum token budget.</param>
        /// <param name="isCode">True if text is code.</param>
        /// <returns>Truncated text that fits within budget, with "..." suffix if truncated.</returns>
        public static string TruncateToTokenBudget(string text, int maxTokens, bool isCode = true)
        {
            if (CountTokens(text, isCode) <= maxTokens)
                return text;
            
            // Binary search for largest prefix that fits
            int low = 0;
            int high = text.Length;
            int bestLength = 0;
            
            while (low <= high)
            {
                int mid = (low + high) / 2;
                string candidate = text.Substring(0, mid);
                
                if (CountTokens(candidate, isCode) <= maxTokens)
                {
                    bestLength = mid;
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            
            // Break at last statement boundary (newline) before bestLength
            int lastNewline = text.LastIndexOf('\n', bestLength - 1);
            if (lastNewline > 0)
                bestLength = lastNewline;
            
            return text.Substring(0, bestLength) + "\n...";
        }
    }
}
```

### Unit Tests

```csharp
// ICSharpCode.ILSpyX.Tests/AI/TokenCounterTests.cs
[TestFixture]
public class TokenCounterTests
{
    [Test]
    public void CountTokens_EmptyString_ReturnsZero()
    {
        Assert.AreEqual(0, TokenCounter.CountTokens(""));
        Assert.AreEqual(0, TokenCounter.CountTokens(null));
    }
    
    [Test]
    public void CountTokens_Code_Uses3To1Ratio()
    {
        // 30 chars of code ≈ 10 tokens + 1 for line = 11
        string code = "public void Method() { }";
        int tokens = TokenCounter.CountTokens(code, isCode: true);
        Assert.That(tokens, Is.InRange(8, 12));
    }
    
    [Test]
    public void CountTokens_Prose_Uses4To1Ratio()
    {
        // 40 chars of prose ≈ 10 tokens + 1 for line = 11
        string prose = "This is a sample sentence for testing.";
        int tokens = TokenCounter.CountTokens(prose, isCode: false);
        Assert.That(tokens, Is.InRange(9, 13));
    }
    
    [Test]
    public void TruncateToTokenBudget_FitsWithinBudget_ReturnsOriginal()
    {
        string text = "short";
        string result = TokenCounter.TruncateToTokenBudget(text, maxTokens: 1000);
        Assert.AreEqual("short", result);
    }
    
    [Test]
    public void TruncateToTokenBudget_ExceedsBudget_TruncatesAtNewline()
    {
        string text = "line1\nline2\nline3\nline4\n";
        string result = TokenCounter.TruncateToTokenBudget(text, maxTokens: 5);
        Assert.That(result, Does.Contain("line1"));
        Assert.That(result, Does.EndWith("..."));
        Assert.That(result, Does.Not.Contain("line4"));
    }
}
```

### Acceptance Criteria
- ✅ `CountTokens(string, bool)` returns reasonable estimates
- ✅ `TruncateToTokenBudget` breaks at line boundaries
- ✅ Unit tests pass
- ✅ No external dependencies

---

## Task 0.2: AI Settings Data Model ⭐

**Files to create:**
- `ICSharpCode.ILSpyX/Settings/AISettings.cs`
- `ICSharpCode.ILSpyX.Tests/Settings/AISettingsTests.cs`

### Implementation Details

```csharp
// ICSharpCode.ILSpyX/Settings/AISettings.cs
using System;
using System.ComponentModel;
using System.Xml.Linq;

namespace ICSharpCode.ILSpyX.Settings
{
    /// <summary>
    /// Settings for AI/LLM integration (BYOK - Bring Your Own Key).
    /// </summary>
    public class AISettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        /// <summary>
        /// LLM provider: "openai", "anthropic", "ollama", "custom"
        /// </summary>
        public string Provider { get; set; } = "openai";
        
        /// <summary>
        /// API key reference (stored securely, not in XML).
        /// This field stores only a placeholder; actual key retrieved via SecureKeyStorage.
        /// </summary>
        public string ApiKeyPlaceholder { get; set; } = "";
        
        /// <summary>
        /// Base URL for API calls. 
        /// OpenAI: https://api.openai.com (default)
        /// Anthropic: https://api.anthropic.com (default)
        /// Ollama: http://localhost:11434 (default)
        /// Custom: user-specified
        /// </summary>
        public string BaseUrl { get; set; } = "https://api.openai.com";
        
        /// <summary>
        /// Model identifier (e.g., "gpt-4o", "claude-opus-4-8", "llama3:70b").
        /// </summary>
        public string Model { get; set; } = "gpt-4o";
        
        /// <summary>
        /// Maximum tokens to send as context (budget per request).
        /// </summary>
        public int MaxContextTokens { get; set; } = 32000;
        
        /// <summary>
        /// Enable streaming responses (typewriter effect).
        /// </summary>
        public bool StreamResponses { get; set; } = true;
        
        /// <summary>
        /// Opt-in: send IL bytecode alongside decompiled C# for richer context.
        /// </summary>
        public bool SendIL { get; set; } = false;
        
        /// <summary>
        /// Opt-in: send callers and callees in context for better rename suggestions.
        /// </summary>
        public bool SendCallGraph { get; set; } = false;
        
        public AISettings()
        {
        }
        
        public void Load(XElement element)
        {
            Provider = (string?)element.Element(nameof(Provider)) ?? "openai";
            ApiKeyPlaceholder = (string?)element.Element(nameof(ApiKeyPlaceholder)) ?? "";
            BaseUrl = (string?)element.Element(nameof(BaseUrl)) ?? GetDefaultBaseUrl(Provider);
            Model = (string?)element.Element(nameof(Model)) ?? GetDefaultModel(Provider);
            MaxContextTokens = (int?)element.Element(nameof(MaxContextTokens)) ?? 32000;
            StreamResponses = (bool?)element.Element(nameof(StreamResponses)) ?? true;
            SendIL = (bool?)element.Element(nameof(SendIL)) ?? false;
            SendCallGraph = (bool?)element.Element(nameof(SendCallGraph)) ?? false;
        }
        
        public XElement Save()
        {
            return new XElement(
                "AISettings",
                new XElement(nameof(Provider), Provider),
                new XElement(nameof(ApiKeyPlaceholder), ApiKeyPlaceholder),
                new XElement(nameof(BaseUrl), BaseUrl),
                new XElement(nameof(Model), Model),
                new XElement(nameof(MaxContextTokens), MaxContextTokens),
                new XElement(nameof(StreamResponses), StreamResponses),
                new XElement(nameof(SendIL), SendIL),
                new XElement(nameof(SendCallGraph), SendCallGraph)
            );
        }
        
        private static string GetDefaultBaseUrl(string provider)
        {
            return provider.ToLowerInvariant() switch
            {
                "openai" => "https://api.openai.com",
                "anthropic" => "https://api.anthropic.com",
                "ollama" => "http://localhost:11434",
                _ => "https://api.openai.com"
            };
        }
        
        private static string GetDefaultModel(string provider)
        {
            return provider.ToLowerInvariant() switch
            {
                "openai" => "gpt-4o",
                "anthropic" => "claude-opus-4-8",
                "ollama" => "llama3:70b",
                _ => "gpt-4o"
            };
        }
    }
}
```

### Unit Tests

```csharp
[TestFixture]
public class AISettingsTests
{
    [Test]
    public void DefaultValues_AreCorrect()
    {
        var settings = new AISettings();
        Assert.AreEqual("openai", settings.Provider);
        Assert.AreEqual(32000, settings.MaxContextTokens);
        Assert.IsTrue(settings.StreamResponses);
        Assert.IsFalse(settings.SendIL);
        Assert.IsFalse(settings.SendCallGraph);
    }
    
    [Test]
    public void SaveAndLoad_RoundTrip()
    {
        var original = new AISettings
        {
            Provider = "anthropic",
            Model = "claude-opus-4-8",
            MaxContextTokens = 16000,
            SendIL = true
        };
        
        XElement xml = original.Save();
        var loaded = new AISettings();
        loaded.Load(xml);
        
        Assert.AreEqual("anthropic", loaded.Provider);
        Assert.AreEqual("claude-opus-4-8", loaded.Model);
        Assert.AreEqual(16000, loaded.MaxContextTokens);
        Assert.IsTrue(loaded.SendIL);
    }
    
    [Test]
    public void Load_MissingElements_UsesDefaults()
    {
        var xml = new XElement("AISettings");
        var settings = new AISettings();
        settings.Load(xml);
        
        Assert.AreEqual("openai", settings.Provider);
        Assert.AreEqual(32000, settings.MaxContextTokens);
    }
}
```

### Integration with ILSpySettings

Add to `ILSpySettings.cs`:

```csharp
public AISettings AISettings {
    get {
        var section = this["AISettings"];
        var settings = new AISettings();
        settings.Load(section);
        return settings;
    }
}
```

### Acceptance Criteria
- ✅ All properties have sensible defaults
- ✅ XML serialization round-trips correctly
- ✅ API key is NOT stored in XML (only placeholder)
- ✅ Provider-specific defaults applied
- ✅ Unit tests pass

---

## Task 0.3: Secure API Key Storage ⭐⭐

**Files to create:**
- `ICSharpCode.ILSpyX/AI/SecureKeyStorage.cs`
- `ICSharpCode.ILSpyX.Tests/AI/SecureKeyStorageTests.cs`

### Implementation Details

```csharp
// ICSharpCode.ILSpyX/AI/SecureKeyStorage.cs
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace ICSharpCode.ILSpyX.AI
{
    /// <summary>
    /// Platform-specific secure storage for API keys.
    /// Windows: DPAPI (ProtectedData)
    /// macOS: Keychain via 'security' CLI
    /// Linux: libsecret via 'secret-tool' CLI, fallback to encrypted file
    /// </summary>
    public static class SecureKeyStorage
    {
        private const string KeychainService = "ILSpy";
        private const string KeychainAccount = "AI_API_Key";
        
        /// <summary>
        /// Saves an API key for the given provider.
        /// </summary>
        public static void SaveKey(string provider, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentException("Provider cannot be empty", nameof(provider));
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API key cannot be empty", nameof(apiKey));
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SaveKeyWindows(provider, apiKey);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                SaveKeyMacOS(provider, apiKey);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                SaveKeyLinux(provider, apiKey);
            }
            else
            {
                throw new PlatformNotSupportedException("Secure key storage not supported on this platform");
            }
        }
        
        /// <summary>
        /// Loads an API key for the given provider. Returns null if not found.
        /// </summary>
        public static string? LoadKey(string provider)
        {
            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentException("Provider cannot be empty", nameof(provider));
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return LoadKeyWindows(provider);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return LoadKeyMacOS(provider);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return LoadKeyLinux(provider);
            }
            else
            {
                throw new PlatformNotSupportedException("Secure key storage not supported on this platform");
            }
        }
        
        /// <summary>
        /// Deletes an API key for the given provider.
        /// </summary>
        public static void DeleteKey(string provider)
        {
            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentException("Provider cannot be empty", nameof(provider));
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                DeleteKeyWindows(provider);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                DeleteKeyMacOS(provider);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                DeleteKeyLinux(provider);
            }
        }
        
        // Windows: DPAPI
        private static void SaveKeyWindows(string provider, string apiKey)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(apiKey);
            byte[] encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
            
            string keyFilePath = GetKeyFilePath(provider);
            Directory.CreateDirectory(Path.GetDirectoryName(keyFilePath)!);
            File.WriteAllBytes(keyFilePath, encryptedBytes);
        }
        
        private static string? LoadKeyWindows(string provider)
        {
            string keyFilePath = GetKeyFilePath(provider);
            if (!File.Exists(keyFilePath))
                return null;
            
            try
            {
                byte[] encryptedBytes = File.ReadAllBytes(keyFilePath);
                byte[] plainBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (CryptographicException)
            {
                // Key was encrypted by different user or machine
                return null;
            }
        }
        
        private static void DeleteKeyWindows(string provider)
        {
            string keyFilePath = GetKeyFilePath(provider);
            if (File.Exists(keyFilePath))
                File.Delete(keyFilePath);
        }
        
        // macOS: Keychain via 'security' CLI
        private static void SaveKeyMacOS(string provider, string apiKey)
        {
            string account = $"{KeychainAccount}_{provider}";
            
            // Delete existing entry first
            DeleteKeyMacOS(provider);
            
            // Add new entry
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "security",
                Arguments = $"add-generic-password -s {KeychainService} -a {account} -w {apiKey}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var process = System.Diagnostics.Process.Start(psi);
            process?.WaitForExit();
        }
        
        private static string? LoadKeyMacOS(string provider)
        {
            string account = $"{KeychainAccount}_{provider}";
            
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "security",
                Arguments = $"find-generic-password -s {KeychainService} -a {account} -w",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var process = System.Diagnostics.Process.Start(psi);
            if (process == null)
                return null;
            
            string output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            
            return process.ExitCode == 0 ? output : null;
        }
        
        private static void DeleteKeyMacOS(string provider)
        {
            string account = $"{KeychainAccount}_{provider}";
            
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "security",
                Arguments = $"delete-generic-password -s {KeychainService} -a {account}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var process = System.Diagnostics.Process.Start(psi);
            process?.WaitForExit();
        }
        
        // Linux: secret-tool CLI (fallback to encrypted file)
        private static void SaveKeyLinux(string provider, string apiKey)
        {
            string account = $"{KeychainAccount}_{provider}";
            
            // Try secret-tool first
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "secret-tool",
                Arguments = $"store --label='ILSpy AI API Key' service {KeychainService} account {account}",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            try
            {
                using var process = System.Diagnostics.Process.Start(psi);
                if (process != null)
                {
                    process.StandardInput.Write(apiKey);
                    process.StandardInput.Close();
                    process.WaitForExit();
                    
                    if (process.ExitCode == 0)
                        return;
                }
            }
            catch
            {
                // secret-tool not available, fall through to file storage
            }
            
            // Fallback: encrypted file (not ideal, but better than plain text)
            SaveKeyWindows(provider, apiKey); // Uses DPAPI-style encryption
        }
        
        private static string? LoadKeyLinux(string provider)
        {
            string account = $"{KeychainAccount}_{provider}";
            
            // Try secret-tool first
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "secret-tool",
                Arguments = $"lookup service {KeychainService} account {account}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            try
            {
                using var process = System.Diagnostics.Process.Start(psi);
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();
                    
                    if (process.ExitCode == 0)
                        return output;
                }
            }
            catch
            {
                // secret-tool not available, fall through to file storage
            }
            
            // Fallback: encrypted file
            return LoadKeyWindows(provider);
        }
        
        private static void DeleteKeyLinux(string provider)
        {
            string account = $"{KeychainAccount}_{provider}";
            
            // Try secret-tool first
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "secret-tool",
                Arguments = $"clear service {KeychainService} account {account}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            try
            {
                using var process = System.Diagnostics.Process.Start(psi);
                process?.WaitForExit();
            }
            catch
            {
                // Ignore
            }
            
            // Also delete file fallback
            DeleteKeyWindows(provider);
        }
        
        private static string GetKeyFilePath(string provider)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appDataPath, "ICSharpCode", "ILSpy", "AI", $"{provider}_key.dat");
        }
    }
}
```

### Unit Tests

```csharp
[TestFixture]
public class SecureKeyStorageTests
{
    private const string TestProvider = "test_provider";
    private const string TestKey = "sk-test-1234567890abcdef";
    
    [TearDown]
    public void Cleanup()
    {
        SecureKeyStorage.DeleteKey(TestProvider);
    }
    
    [Test]
    public void SaveAndLoad_RoundTrip()
    {
        SecureKeyStorage.SaveKey(TestProvider, TestKey);
        string? loaded = SecureKeyStorage.LoadKey(TestProvider);
        
        Assert.AreEqual(TestKey, loaded);
    }
    
    [Test]
    public void LoadKey_NotFound_ReturnsNull()
    {
        string? loaded = SecureKeyStorage.LoadKey("nonexistent_provider");
        Assert.IsNull(loaded);
    }
    
    [Test]
    public void DeleteKey_RemovesKey()
    {
        SecureKeyStorage.SaveKey(TestProvider, TestKey);
        SecureKeyStorage.DeleteKey(TestProvider);
        
        string? loaded = SecureKeyStorage.LoadKey(TestProvider);
        Assert.IsNull(loaded);
    }
    
    [Test]
    public void SaveKey_EmptyProvider_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => SecureKeyStorage.SaveKey("", TestKey));
    }
    
    [Test]
    public void SaveKey_EmptyKey_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => SecureKeyStorage.SaveKey(TestProvider, ""));
    }
}
```

### Acceptance Criteria
- ✅ Windows: Uses DPAPI, keys stored in `%APPDATA%\ICSharpCode\ILSpy\AI\`
- ✅ macOS: Uses Keychain via `security` CLI
- ✅ Linux: Uses libsecret via `secret-tool`, falls back to encrypted file
- ✅ Round-trip works on all platforms
- ✅ Graceful fallback if platform API unavailable
- ✅ Unit tests pass

---

## Task 0.4: LLM Provider Interface ⭐

**Files to create:**
- `ICSharpCode.ILSpyX/AI/ILLMProvider.cs`
- `ICSharpCode.ILSpyX/AI/LLMRequest.cs`
- `ICSharpCode.ILSpyX/AI/LLMMessage.cs`

### Implementation Details

```csharp
// ICSharpCode.ILSpyX/AI/ILLMProvider.cs
using System.Collections.Generic;
using System.Threading;

namespace ICSharpCode.ILSpyX.AI
{
    /// <summary>
    /// Abstraction for LLM providers (OpenAI, Anthropic, Ollama, custom).
    /// </summary>
    public interface ILLMProvider
    {
        /// <summary>
        /// Sends a completion request and yields response chunks as they arrive (streaming).
        /// </summary>
        /// <param name="request">The LLM request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Async enumerable of response text chunks.</returns>
        IAsyncEnumerable<string> CompleteAsync(LLMRequest request, CancellationToken cancellationToken);
        
        /// <summary>
        /// Tests the connection with a simple "Hello" prompt.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if connection successful, false otherwise.</returns>
        Task<bool> TestConnectionAsync(CancellationToken cancellationToken);
    }
}
```

```csharp
// ICSharpCode.ILSpyX/AI/LLMRequest.cs
using System.Collections.Generic;

namespace ICSharpCode.ILSpyX.AI
{
    /// <summary>
    /// Represents a request to an LLM provider.
    /// </summary>
    public record LLMRequest(
        string SystemPrompt,
        IReadOnlyList<LLMMessage> Messages,
        int MaxTokens,
        double Temperature = 0.7
    );
}
```

```csharp
// ICSharpCode.ILSpyX/AI/LLMMessage.cs
namespace ICSharpCode.ILSpyX.AI
{
    /// <summary>
    /// A single message in an LLM conversation.
    /// </summary>
    public record LLMMessage(
        string Role,    // "user", "assistant", "system"
        string Content
    );
}
```

### Acceptance Criteria
- ✅ Clean interface definition
- ✅ Supports streaming via `IAsyncEnumerable<string>`
- ✅ Simple records for request/message
- ✅ No implementation yet (Phase 0.5)

---

## Task 0.5: OpenAI Provider Implementation ⭐⭐

**Files to create:**
- `ICSharpCode.ILSpyX/AI/Providers/OpenAIProvider.cs`
- `ICSharpCode.ILSpyX.Tests/AI/Providers/OpenAIProviderTests.cs`

### Implementation Details

```csharp
// ICSharpCode.ILSpyX/AI/Providers/OpenAIProvider.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpyX.AI.Providers
{
    /// <summary>
    /// OpenAI-compatible API provider (supports OpenAI, Ollama, and custom endpoints).
    /// </summary>
    public class OpenAIProvider : ILLMProvider
    {
        private readonly string baseUrl;
        private readonly string apiKey;
        private readonly string model;
        private readonly HttpClient httpClient;
        
        public OpenAIProvider(string baseUrl, string apiKey, string model)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL cannot be empty", nameof(baseUrl));
            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Model cannot be empty", nameof(model));
            
            this.baseUrl = baseUrl.TrimEnd('/');
            this.apiKey = apiKey ?? ""; // Ollama doesn't require a key
            this.model = model;
            this.httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(120);
        }
        
        public async IAsyncEnumerable<string> CompleteAsync(
            LLMRequest request,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            string url = $"{baseUrl}/v1/chat/completions";
            
            // Build messages array (system prompt + conversation)
            var messages = new List<object>();
            if (!string.IsNullOrWhiteSpace(request.SystemPrompt))
            {
                messages.Add(new { role = "system", content = request.SystemPrompt });
            }
            foreach (var msg in request.Messages)
            {
                messages.Add(new { role = msg.Role, content = msg.Content });
            }
            
            var payload = new
            {
                model = model,
                messages = messages,
                max_tokens = request.MaxTokens,
                temperature = request.Temperature,
                stream = true
            };
            
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }
            
            using var response = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"API request failed: {response.StatusCode} - {errorBody}");
            }
            
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new System.IO.StreamReader(stream);
            
            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                string? line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                
                if (!line.StartsWith("data: "))
                    continue;
                
                string data = line.Substring(6).Trim();
                if (data == "[DONE]")
                    break;
                
                JsonDocument json;
                try
                {
                    json = JsonDocument.Parse(data);
                }
                catch (JsonException)
                {
                    continue; // Skip malformed JSON
                }
                
                using (json)
                {
                    var root = json.RootElement;
                    if (root.TryGetProperty("choices", out var choices) &&
                        choices.GetArrayLength() > 0)
                    {
                        var choice = choices[0];
                        if (choice.TryGetProperty("delta", out var delta) &&
                            delta.TryGetProperty("content", out var contentProp))
                        {
                            string chunk = contentProp.GetString() ?? "";
                            if (!string.IsNullOrEmpty(chunk))
                            {
                                yield return chunk;
                            }
                        }
                    }
                }
            }
        }
        
        public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken)
        {
            try
            {
                var testRequest = new LLMRequest(
                    SystemPrompt: "You are a test assistant.",
                    Messages: new[] { new LLMMessage("user", "Say 'Hello'") },
                    MaxTokens: 10
                );
                
                bool gotResponse = false;
                await foreach (var chunk in CompleteAsync(testRequest, cancellationToken))
                {
                    gotResponse = true;
                    break; // Just need to verify we can get one chunk
                }
                
                return gotResponse;
            }
            catch
            {
                return false;
            }
        }
    }
}
```

### Unit Tests (with mock HTTP)

```csharp
[TestFixture]
public class OpenAIProviderTests
{
    [Test]
    public void Constructor_EmptyBaseUrl_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new OpenAIProvider("", "key", "model"));
    }
    
    [Test]
    public void Constructor_EmptyModel_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new OpenAIProvider("https://api.openai.com", "key", ""));
    }
    
    // Note: Full integration tests require mock HTTP handler
    // For Phase 0, focus on construction and basic validation
    // Real streaming tests come in Phase 2.1
}
```

### Acceptance Criteria
- ✅ Constructor validates inputs
- ✅ Builds correct JSON payload
- ✅ Sends authorization header if API key present
- ✅ Parses SSE stream (`data: ...` lines)
- ✅ Yields content chunks from `delta.content`
- ✅ Handles `[DONE]` terminator
- ✅ Handles errors (401, 404, 429, 500)
- ✅ `TestConnectionAsync` verifies connectivity

---

## Task 0.6: Decompilation Context Builder (Basic) ⭐⭐

**Files to create:**
- `ICSharpCode.ILSpyX/AI/DecompilationContext.cs`
- `ICSharpCode.ILSpyX/AI/ContextBuilder.cs`
- `ICSharpCode.ILSpyX.Tests/AI/ContextBuilderTests.cs`

### Implementation Details

```csharp
// ICSharpCode.ILSpyX/AI/DecompilationContext.cs
namespace ICSharpCode.ILSpyX.AI
{
    /// <summary>
    /// Encapsulates all context about a decompiled symbol for LLM consumption.
    /// </summary>
    public record DecompilationContext
    {
        public string DecompiledCSharp { get; init; } = "";
        public string? IL { get; init; }
        public string FullyQualifiedName { get; init; } = "";
        public string AssemblyName { get; init; } = "";
        public string TargetFramework { get; init; } = "";
        public IReadOnlyList<string> Callers { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> Callees { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> ImplementedInterfaces { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> Attributes { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> StringLiterals { get; init; } = Array.Empty<string>();
        public int ApproximateTokenCount { get; init; }
        
        /// <summary>
        /// Serializes this context to a compact Markdown format for LLM consumption.
        /// </summary>
        public string ToMarkdown()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"# {FullyQualifiedName}");
            sb.AppendLine();
            sb.AppendLine($"**Assembly:** {AssemblyName}");
            if (!string.IsNullOrEmpty(TargetFramework))
                sb.AppendLine($"**Target Framework:** {TargetFramework}");
            sb.AppendLine();
            
            if (Attributes.Count > 0)
            {
                sb.AppendLine("**Attributes:**");
                foreach (var attr in Attributes)
                    sb.AppendLine($"- {attr}");
                sb.AppendLine();
            }
            
            if (ImplementedInterfaces.Count > 0)
            {
                sb.AppendLine("**Implements:**");
                foreach (var iface in ImplementedInterfaces)
                    sb.AppendLine($"- {iface}");
                sb.AppendLine();
            }
            
            sb.AppendLine("## Decompiled Code");
            sb.AppendLine();
            sb.AppendLine("```csharp");
            sb.AppendLine(DecompiledCSharp);
            sb.AppendLine("```");
            sb.AppendLine();
            
            if (!string.IsNullOrEmpty(IL))
            {
                sb.AppendLine("## IL Bytecode");
                sb.AppendLine();
                sb.AppendLine("```il");
                sb.AppendLine(IL);
                sb.AppendLine("```");
                sb.AppendLine();
            }
            
            if (StringLiterals.Count > 0)
            {
                sb.AppendLine("**String Literals:**");
                foreach (var lit in StringLiterals.Take(20))
                    sb.AppendLine($"- \"{lit}\"");
                if (StringLiterals.Count > 20)
                    sb.AppendLine($"- ... and {StringLiterals.Count - 20} more");
                sb.AppendLine();
            }
            
            if (Callers.Count > 0)
            {
                sb.AppendLine("**Called By:**");
                foreach (var caller in Callers.Take(10))
                    sb.AppendLine($"- {caller}");
                if (Callers.Count > 10)
                    sb.AppendLine($"- ... and {Callers.Count - 10} more");
                sb.AppendLine();
            }
            
            if (Callees.Count > 0)
            {
                sb.AppendLine("**Calls:**");
                foreach (var callee in Callees.Take(10))
                    sb.AppendLine($"- {callee}");
                if (Callees.Count > 10)
                    sb.AppendLine($"- ... and {Callees.Count - 10} more");
                sb.AppendLine();
            }
            
            return sb.ToString();
        }
    }
}
```

```csharp
// ICSharpCode.ILSpyX/AI/ContextBuilder.cs
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using System.IO;
using System.Text;

namespace ICSharpCode.ILSpyX.AI
{
    /// <summary>
    /// Builds DecompilationContext from ILSpy's type system.
    /// </summary>
    public class ContextBuilder
    {
        private readonly AISettings settings;
        
        public ContextBuilder(AISettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
        
        /// <summary>
        /// Builds context for the given entity (type, method, field, property).
        /// </summary>
        public DecompilationContext Build(IEntity entity, CSharpDecompiler decompiler)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (decompiler == null)
                throw new ArgumentNullException(nameof(decompiler));
            
            // Decompile to C#
            string csharpCode = DecompileEntity(entity, decompiler);
            
            // Extract metadata
            string fqn = entity.FullName;
            string assemblyName = entity.ParentModule?.AssemblyName ?? "";
            string targetFramework = GetTargetFramework(entity.ParentModule);
            
            var attributes = ExtractAttributes(entity);
            var interfaces = ExtractInterfaces(entity);
            var stringLiterals = ExtractStringLiterals(csharpCode);
            
            // Optional: IL
            string? il = null;
            if (settings.SendIL)
            {
                il = DecompileToIL(entity, decompiler);
            }
            
            // Optional: call graph (Phase 2.3)
            var callers = new List<string>();
            var callees = new List<string>();
            
            // Build context
            var context = new DecompilationContext
            {
                DecompiledCSharp = csharpCode,
                IL = il,
                FullyQualifiedName = fqn,
                AssemblyName = assemblyName,
                TargetFramework = targetFramework,
                Callers = callers,
                Callees = callees,
                ImplementedInterfaces = interfaces,
                Attributes = attributes,
                StringLiterals = stringLiterals
            };
            
            // Enforce token budget
            context = EnforceTokenBudget(context);
            
            return context;
        }
        
        private string DecompileEntity(IEntity entity, CSharpDecompiler decompiler)
        {
            var output = new StringWriter();
            var textOutput = new PlainTextOutput(output);
            
            if (entity is ITypeDefinition type)
            {
                decompiler.DecompileType(type.FullTypeName, textOutput);
            }
            else if (entity is IMember member)
            {
                decompiler.DecompileMember(member, textOutput);
            }
            
            return output.ToString();
        }
        
        private string? DecompileToIL(IEntity entity, CSharpDecompiler decompiler)
        {
            // Simplified: would use ILLanguage.Instance
            // For Phase 0, return placeholder
            return null;
        }
        
        private IReadOnlyList<string> ExtractAttributes(IEntity entity)
        {
            var attrs = new List<string>();
            foreach (var attr in entity.GetAttributes())
            {
                attrs.Add(attr.AttributeType.Name);
            }
            return attrs;
        }
        
        private IReadOnlyList<string> ExtractInterfaces(IEntity entity)
        {
            if (entity is ITypeDefinition typeDef)
            {
                return typeDef.DirectBaseTypes
                    .Where(t => t.Kind == TypeKind.Interface)
                    .Select(t => t.FullName)
                    .ToList();
            }
            return Array.Empty<string>();
        }
        
        private IReadOnlyList<string> ExtractStringLiterals(string csharpCode)
        {
            // Simplified: regex-based extraction
            // For Phase 0, return empty (Phase 2.3 will walk SyntaxTree)
            return Array.Empty<string>();
        }
        
        private string GetTargetFramework(IModule? module)
        {
            // Simplified: would inspect TargetFrameworkAttribute
            return module?.AssemblyName ?? "";
        }
        
        private DecompilationContext EnforceTokenBudget(DecompilationContext context)
        {
            int maxTokens = settings.MaxContextTokens;
            string markdown = context.ToMarkdown();
            int currentTokens = TokenCounter.CountTokens(markdown, isCode: true);
            
            if (currentTokens <= maxTokens)
                return context;
            
            // Trim in order: IL -> callees -> callers -> literals -> C# body
            if (context.IL != null)
            {
                context = context with { IL = null };
                markdown = context.ToMarkdown();
                currentTokens = TokenCounter.CountTokens(markdown, isCode: true);
                if (currentTokens <= maxTokens)
                    return context;
            }
            
            if (context.Callees.Count > 0)
            {
                context = context with { Callees = Array.Empty<string>() };
                markdown = context.ToMarkdown();
                currentTokens = TokenCounter.CountTokens(markdown, isCode: true);
                if (currentTokens <= maxTokens)
                    return context;
            }
            
            if (context.Callers.Count > 0)
            {
                context = context with { Callers = Array.Empty<string>() };
                markdown = context.ToMarkdown();
                currentTokens = TokenCounter.CountTokens(markdown, isCode: true);
                if (currentTokens <= maxTokens)
                    return context;
            }
            
            if (context.StringLiterals.Count > 0)
            {
                context = context with { StringLiterals = Array.Empty<string>() };
                markdown = context.ToMarkdown();
                currentTokens = TokenCounter.CountTokens(markdown, isCode: true);
                if (currentTokens <= maxTokens)
                    return context;
            }
            
            // Final resort: truncate C# body
            string truncated = TokenCounter.TruncateToTokenBudget(context.DecompiledCSharp, maxTokens / 2, isCode: true);
            context = context with { DecompiledCSharp = truncated };
            
            return context;
        }
    }
}
```

### Unit Tests

```csharp
[TestFixture]
public class ContextBuilderTests
{
    [Test]
    public void Build_ValidEntity_ReturnsContext()
    {
        // This test requires mock IEntity and CSharpDecompiler
        // For Phase 0, focus on ToMarkdown format
        
        var context = new DecompilationContext
        {
            DecompiledCSharp = "public void Method() { }",
            FullyQualifiedName = "MyClass.Method",
            AssemblyName = "MyAssembly",
            TargetFramework = "net10.0",
            Attributes = new[] { "Obsolete" },
            StringLiterals = new[] { "hello", "world" }
        };
        
        string markdown = context.ToMarkdown();
        
        Assert.That(markdown, Does.Contain("# MyClass.Method"));
        Assert.That(markdown, Does.Contain("**Assembly:** MyAssembly"));
        Assert.That(markdown, Does.Contain("```csharp"));
        Assert.That(markdown, Does.Contain("public void Method()"));
        Assert.That(markdown, Does.Contain("**String Literals:**"));
    }
    
    [Test]
    public void EnforceTokenBudget_ExceedsBudget_TrimsILFirst()
    {
        // Mock test: verify trimming order
        // Full implementation in Phase 2.3
    }
}
```

### Acceptance Criteria
- ✅ `Build` method takes `IEntity` + `CSharpDecompiler`, returns `DecompilationContext`
- ✅ Extracts: C# code, FQN, assembly name, attributes, interfaces
- ✅ `ToMarkdown()` produces clean, structured Markdown
- ✅ Token budget enforcement trims in correct order
- ✅ String literal extraction placeholder (full impl in Phase 2.3)
- ✅ Call graph placeholder (full impl in Phase 2.3)

---

## Phase 0 Completion Checklist

- [ ] 0.1 Token Counter implemented and tested
- [ ] 0.2 AI Settings model defined, XML round-trip tested
- [ ] 0.3 Secure key storage works on Windows/macOS/Linux
- [ ] 0.4 LLM provider interface defined
- [ ] 0.5 OpenAI provider implemented, basic connectivity tested
- [ ] 0.6 Context builder extracts basic metadata and decompiled code
- [ ] All unit tests pass (`dotnet test --solution ILSpy.sln --report-trx --filter "FullyQualifiedName~AI"`)
- [ ] Code reviewed for copyright headers (see CLAUDE.md conventions)
- [ ] Pre-commit hook passes (formatting)

---

## Next Steps

After Phase 0 is complete and validated:
1. Create `doc/plans/phase-1-first-features.md`
2. Implement Phase 1 (settings UI, first explanation feature)
3. Get user feedback on the foundation before proceeding to Phase 2

---

**Document Version:** 1.0  
**Last Updated:** 2026-08-17
