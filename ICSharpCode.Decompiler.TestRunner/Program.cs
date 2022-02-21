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





