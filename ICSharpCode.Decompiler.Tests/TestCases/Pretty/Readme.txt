The files in this folder are prettiness tests for the decompiler.

The NUnit class running these tests is ../PrettyTestRunner.cs.

Each test case is a C# file.
The test runner will:
 1. Compile the file into an .exe/.dll
 2. Decompile the .exe/.dll
 3. Compare the resulting code with the original input code.

The tests pass if the code looks exactly the same as the input code, ignoring comments, empty lines and preprocessor directives.
It also ignores disabled preprocessors sections (e.g. "#if ROSLYN") when the test runs with a compiler that does not set this symbol.
See Tester.GetPreprocessorSymbols() for the available symbols.
