The files in this folder are prettiness tests for the decompiler.

The NUnit class running these tests is ../PrettyTestRunner.cs.
It uses pre-defined IL files in order to avoid test failures in cause of compiler changes.
We test different C# compiler versions as well (in future).

Each test consists of a C# file for comparing the resulting code and a source IL file used for assembling/decompiling.

We:
* assemble a test case (call the result "executable 1")
* decompile "executable 1" to C# ("decompiled.cs")
* compare "decompiled.cs" to "source.cs"

The tests pass if the code looks exactly the same as the input code, ignoring comments, empty lines and preprocessor directives.

Note: If you delete an .il file, it will be re-created on the next test run.
This can be helpful when modifying the test case; but it also might have unexpected results when your C# compiler differs
from the compiler previously used to create the .il file.
