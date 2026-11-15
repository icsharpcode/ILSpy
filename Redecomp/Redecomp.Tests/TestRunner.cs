// Copyright (c) 2026 Franck
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Reflection;

namespace Redecomp.Tests;

[AttributeUsage(AttributeTargets.Method)]
public sealed class TestAttribute : Attribute;

public sealed class AssertionException(string message) : Exception(message);

public static class Assert
{
	public static void That(bool condition, string message)
	{
		if (!condition)
			throw new AssertionException(message);
	}

	public static void AreEqual<T>(T expected, T actual, string what)
	{
		if (!EqualityComparer<T>.Default.Equals(expected, actual))
			throw new AssertionException($"{what}: expected <{expected}> but was <{actual}>");
	}
}

/// <summary>
/// Runs every public static method marked with <see cref="TestAttribute"/>.
/// An optional argument filters tests by substring of their full name.
/// </summary>
public static class Program
{
	public static int Main(string[] args)
	{
		string? filter = args.FirstOrDefault();
		var tests = typeof(Program).Assembly.GetTypes()
			.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
			.Where(m => m.GetCustomAttribute<TestAttribute>() != null)
			.Select(m => (Method: m, Name: $"{m.DeclaringType!.Name}.{m.Name}"))
			.Where(t => filter == null || t.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
			.OrderBy(t => t.Name)
			.ToList();
		int failed = 0;
		foreach (var (method, name) in tests)
		{
			try
			{
				method.Invoke(null, null);
				Console.WriteLine($"PASS {name}");
			}
			catch (TargetInvocationException ex)
			{
				failed++;
				Console.WriteLine($"FAIL {name}: {ex.InnerException?.GetType().Name}: {ex.InnerException?.Message}");
			}
		}
		Console.WriteLine($"{tests.Count - failed}/{tests.Count} passed");
		return failed == 0 && tests.Count > 0 ? 0 : 1;
	}
}
