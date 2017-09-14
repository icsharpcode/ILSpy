The files in this folder are correctness tests for the decompiler.

The NUnit class running these tests is ../../CorrectnessTestRunner.cs.

We:
* compile/assemble a test case (call the result "executable 1")
* decompile "executable 1" to C# ("decompiled.cs")
* compile decompiled.cs, resulting in "executable 2"
* run both executable and compare their output (exit code, stdout, stderr)

We repeat the steps above with a few different compiler options (/o+ or not; /debug or not).

The tests pass if the code compiles without error and produces the same output.
The tests do not care at all if the resulting code is pretty, or if any high-level constructs like closures were detected.
