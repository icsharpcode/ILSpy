## About

ICSharpCode.Decompiler is the decompiler engine used in [ILSpy](https://github.com/icsharpcode/ILSpy/).

You can learn how to use it from the following samples/real-world applications:

* **decompiler-nuget-demos.verso** in the [ILSpy repository](https://github.com/icsharpcode/ILSpy/). This shows the basics of using the NuGet (Verso extension for VSCode needed).
* **ILSpyCmd** project in the [ILSpy repository](https://github.com/icsharpcode/ILSpy/tree/master/ICSharpCode.ILSpyCmd). This dotnet tool shows automation of whole assembly decompilation.
* **ILSpy.Backend** in the [ILSpy Visual Studio Code Extension repository](https://github.com/icsharpcode/ilspy-vscode/tree/master/backend). This LSP shows listing / decompiling members.
* **ILSpy** itself. Complex, recommended only for advanced users and definitely **not** the place to get started.

## Trimming and Native AOT

The assembly is annotated as trim- and AOT-compatible (`IsTrimmable` and `IsAotCompatible` assembly metadata), and every build publishes it through the ILCompiler with the whole assembly rooted and warnings as errors, then compares the native binary's output with the managed build's (`ICSharpCode.Decompiler.AotTest` in the repository). Two things to know when hosting the decompiler in a trimmed, single-file or Native AOT app:

* `DecompilerEventSource` (the `ICSharpCode.Decompiler` tracing provider) stays silent in `PublishAot` apps unless the app sets `<EventSourceSupport>true</EventSourceSupport>`: the ILCompiler defaults that feature switch to false.
* Single-file and Native AOT hosts have no framework assemblies next to the running runtime. The resolver then takes framework facades (mscorlib, netstandard, System.*) for assemblies targeting other frameworks from the newest shared framework of the `dotnet` on `PATH`. A host without an installed .NET runtime has to pass a `UniversalAssemblyResolver` with the reference directories added.
