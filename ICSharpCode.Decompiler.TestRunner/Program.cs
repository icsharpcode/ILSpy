// Copyright (c) 2022 Siegfried Pammer
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

using System;
using System.Reflection;
using System.Runtime.Loader;

namespace ICSharpCode.Decompiler.TestRunner;

public static class Program
{
	static int Main(string[] args)
	{
		AssemblyLoadContext context = new("TestRunner", isCollectible: true);
		context.Resolving += ContextResolving;

		try
		{
			var mainAssembly = context.LoadFromAssemblyPath(args[0]);
			int paramCount = mainAssembly.EntryPoint!.GetParameters().Length;
			object? result = mainAssembly.EntryPoint!.Invoke(null, paramCount == 0 ? new object[0] : new object[1] { new string[0] });
			return result is int i ? i : 0;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("TestRunner crashed:");
			Console.Error.WriteLine(ex.ToString());
			return -1;
		}
		finally
		{
			context.Unload();
			context.Resolving -= ContextResolving;
		}
	}

	private static Assembly? ContextResolving(AssemblyLoadContext context, AssemblyName name)
	{
		return null;
	}
}





