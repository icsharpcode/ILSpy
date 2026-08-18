---
phase: 0
slug: foundation
status: partial
nyquist_compliant: false
wave_0_complete: true
created: 2026-08-18
---

# Phase 0 - Validation Strategy

Phase 0 foundation behavior is covered by the existing NUnit suite plus
platform-gated native credential-store smoke tests.

## Test Infrastructure

| Property | Value |
|----------|-------|
| Framework | NUnit 4.6.1 with Microsoft Testing Platform |
| Config file | global.json |
| Quick run command | dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --filter "FullyQualifiedName~AI" |
| Supplemental AI command | dotnet test ILSpy.Tests/ILSpy.Tests.csproj --filter "FullyQualifiedName~AI" |
| Pinned SDK | 11.0.100-preview.7.26381.103 |

## Per-Task Verification Map

| Task ID | Requirement | Test File | Command | Status |
|---------|-------------|-----------|---------|--------|
| 0.1 | Token counting and budget truncation | ICSharpCode.ILSpyX.Tests/AI/TokenCounterTests.cs | AI test project command | Green |
| 0.2 | AI settings defaults and XML round-trip | ICSharpCode.ILSpyX.Tests/Settings/AISettingsTests.cs | AI test project command | Green |
| 0.3 | Secure storage validation, cancellation, unavailable-store handling | ICSharpCode.ILSpyX.Tests/AI/SecureKeyStorageTests.cs | AI test project command | Green |
| 0.3 | Native credential-store round trip | ICSharpCode.ILSpyX.Tests/AI/SecureKeyStorageSmokeTests.cs | AI test project command on the matching OS | macOS green; Windows/Linux pending |
| 0.4 | LLM request/message contract | ICSharpCode.ILSpyX.Tests/AI/Providers/OpenAIProviderTests.cs | AI test project command | Green |
| 0.5 | OpenAI-compatible HTTP, SSE, validation, and error mapping | ICSharpCode.ILSpyX.Tests/AI/Providers/OpenAIProviderTests.cs | AI test project command | Green |
| 0.6 | Context metadata, markdown serialization, and budget enforcement | ICSharpCode.ILSpyX.Tests/AI/ContextBuilderTests.cs | AI test project command | Green |

## Validation Results

| Check | Result |
|-------|--------|
| ICSharpCode.ILSpyX.Tests AI filter | 138 passed, 0 failed, 2 platform-skipped |
| ILSpy.Tests AI filter | 4 passed, 0 failed, 1 environment-skipped |
| macOS Keychain native smoke | Passed |
| Exact solution-wide command from the phase plan | Not runnable on macOS because Windows-only test assemblies require Microsoft.WindowsDesktop.App; use the Windows CI leg for that command |

## Manual-Only Verifications

| Behavior | Why Manual | Instructions |
|----------|------------|--------------|
| Windows DPAPI round trip | Requires a Windows host and user profile | Run SecureKeyStorageSmokeTests.RoundTrip_WorksOnWindows on the Windows CI or a Windows developer machine |
| Linux Secret Service round trip | Requires Linux Secret Service, secret-tool, and an unlocked user session | Run SecureKeyStorageSmokeTests.RoundTrip_WorksOnLinux on a Linux desktop session with secret-tool available |

## Validation Sign-Off

- [x] All Phase 0 requirements have automated unit coverage.
- [x] macOS native credential-store smoke coverage passes.
- [x] Pinned SDK is installed and the phase AI test project is green.
- [ ] Windows native credential-store smoke has run on Windows.
- [ ] Linux native credential-store smoke has run on Linux.
- [ ] Exact solution-wide command has run on a Windows host.
- [ ] nyquist_compliant: true

Approval: pending platform validation
