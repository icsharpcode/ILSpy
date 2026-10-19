# ICSharpCode.Decompiler.AotTest

Native AOT gate for the `ICSharpCode.Decompiler` NuGet package. It answers two questions about the
shipped netstandard2.0 assembly:

1. Does ILC report any trim or AOT warning for it? The publish roots the whole assembly
   (`TrimmerRootAssembly`), reports warnings per call site (`TrimmerSingleWarn=false`), keeps the
   `DecompilerEventSource` call sites alive (`EventSourceSupport=true`) and treats every warning as
   an error, so a new reflection pattern anywhere in the decompiler fails the publish.
2. Does the native binary behave like the managed one? Both are run on the same inputs and their
   decompiled output is compared byte for byte; the binary also wraps an exception thrown from a
   nested helper in a `DecompilerException` and exits non-zero if the helper's name is missing
   from the trace. A second native publish with `RootDecompiler=false` leaves the decompiler
   trimmed the way it is in a consumer's app and is compared the same way. The native binaries
   run in invariant globalization mode (the project sets `InvariantGlobalization`), the managed
   build is made with `InvariantGlobalization=false` and runs on ICU, so output that depends on the
   culture or on the globalization mode shows up as a difference.

The project is in no solution (a plain solution restore would otherwise pull the ILCompiler packs on
every build) and runs in CI on Linux (`Decompiler Native AOT gate` in `build-ilspy.yml`).

Locally (Linux/macOS with clang installed; on Windows the MSVC linker must be discoverable):

```
dotnet publish ICSharpCode.Decompiler.AotTest -c Release -r linux-x64 -p:RestoreEnablePackagePruning=false -o aotgate/native
dotnet publish ICSharpCode.Decompiler.AotTest -c Release -r linux-x64 -p:RootDecompiler=false -p:RestoreEnablePackagePruning=false -o aotgate/native-trimmed
dotnet build ICSharpCode.Decompiler.AotTest -c Release -p:PublishAot=false -p:InvariantGlobalization=false -p:RestoreEnablePackagePruning=false -o aotgate/managed
aotgate/native/ICSharpCode.Decompiler.AotTest aotgate/managed/ICSharpCode.Decompiler.dll aot.cs
aotgate/managed/ICSharpCode.Decompiler.AotTest aotgate/managed/ICSharpCode.Decompiler.dll managed.cs
cmp aot.cs managed.cs
```

The managed build is framework-dependent on purpose: a self-contained host carries the whole
framework in its own directory and resolves references there, which a native host cannot, so the
two would differ for reasons that have nothing to do with the decompiler.

The RID-specific restore rewrites `ICSharpCode.Decompiler.Generators/packages.lock.json`; discard that
change with `git checkout`.
