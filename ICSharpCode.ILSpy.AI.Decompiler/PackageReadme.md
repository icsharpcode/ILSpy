## About

ICSharpCode.ILSpy.AI.Decompiler holds the decompiler-aware half of ILSpy's AI integration: context building from decompiled entities, LLM-backed explanation and rename suggestion, AI/semantic search strategies, and security analysis. It depends only on `ICSharpCode.ILSpy.AI` (providers, configuration, prompts) and `ICSharpCode.Decompiler`.

The portable provider/configuration/credential half lives in `ICSharpCode.ILSpy.AI`; the Avalonia desktop UI stays inside ILSpy itself.
