// Copyright (c) 2026 Piero Viano
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
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;

namespace ICSharpCode.ILSpy.Tests.Dependencies;

/// <summary>
/// Emits small library/consumer assembly pairs for the dependency features. The library defines
/// <c>Api.Compute(int)</c>; the consumer calls it from a method body, which gives the consumer an
/// assembly reference to the library.
/// </summary>
static class DependencyFixtures
{
	/// <summary>A fresh, empty temp directory.</summary>
	public static string NewDirectory()
	{
		var dir = Path.Combine(Path.GetTempPath(), $"ILSpyDependencyFixture_{Guid.NewGuid():N}");
		Directory.CreateDirectory(dir);
		return dir;
	}

	/// <summary>A unique simple assembly name with the given prefix.</summary>
	public static string UniqueName(string prefix) => prefix + Guid.NewGuid().ToString("N").Substring(0, 8);

	/// <summary>Emits the library <c>&lt;directory&gt;/&lt;name&gt;.dll</c> and returns its path.</summary>
	public static string EmitLibrary(string directory, string name, Version? version = null)
	{
		var assemblyName = new AssemblyName(name) { Version = version ?? new Version(1, 0, 0, 0) };
		var ab = new PersistedAssemblyBuilder(assemblyName, typeof(object).Assembly);
		var module = ab.DefineDynamicModule(name);

		var api = module.DefineType($"{name}.Api", TypeAttributes.Public | TypeAttributes.Class);
		var compute = api.DefineMethod("Compute", MethodAttributes.Public | MethodAttributes.Static, typeof(int), [typeof(int)]);
		var il = compute.GetILGenerator();
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Ret);
		api.CreateType();

		var path = Path.Combine(directory, name + ".dll");
		ab.Save(path);
		return path;
	}

	/// <summary>
	/// Emits the consumer <c>&lt;directory&gt;/&lt;name&gt;.dll</c> compiled against the library at
	/// <paramref name="libraryPath"/> and returns its path. The library is read into a collectible
	/// load context from memory, so its file is neither locked nor kept loaded.
	/// </summary>
	public static string EmitConsumer(string directory, string name, string libraryPath)
	{
		var context = new AssemblyLoadContext(name, isCollectible: true);
		try
		{
			using var stream = new MemoryStream(File.ReadAllBytes(libraryPath));
			var library = context.LoadFromStream(stream);
			string libraryName = library.GetName().Name!;
			var apiType = library.GetType($"{libraryName}.Api", throwOnError: true)!;

			var ab = new PersistedAssemblyBuilder(new AssemblyName(name) { Version = new Version(1, 0, 0, 0) }, typeof(object).Assembly);
			var module = ab.DefineDynamicModule(name);

			var bodyUser = module.DefineType($"{name}.UsesInBody", TypeAttributes.Public | TypeAttributes.Class);
			var run = bodyUser.DefineMethod("Run", MethodAttributes.Public | MethodAttributes.Static, typeof(int), Type.EmptyTypes);
			var il = run.GetILGenerator();
			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Call, apiType.GetMethod("Compute")!);
			il.Emit(OpCodes.Ret);
			bodyUser.CreateType();

			var path = Path.Combine(directory, name + ".dll");
			ab.Save(path);
			return path;
		}
		finally
		{
			context.Unload();
		}
	}

	/// <summary>
	/// Emits a library and a consumer next to each other in a fresh directory, so the consumer's
	/// reference resolves from its own folder.
	/// </summary>
	public static (string LibraryPath, string ConsumerPath, string LibraryName, string ConsumerName) EmitPair(string prefix = "Dep")
	{
		var dir = NewDirectory();
		var libraryName = UniqueName(prefix + "Lib");
		var consumerName = UniqueName(prefix + "App");
		var library = EmitLibrary(dir, libraryName);
		var consumer = EmitConsumer(dir, consumerName, library);
		return (library, consumer, libraryName, consumerName);
	}

	/// <summary>
	/// Emits a consumer whose library reference cannot be resolved: the library is built in a
	/// separate directory that is deleted afterwards.
	/// </summary>
	public static (string ConsumerPath, string MissingLibraryName) EmitConsumerWithMissingReference(string prefix = "Dep")
	{
		var libraryDir = NewDirectory();
		var consumerDir = NewDirectory();
		var libraryName = UniqueName(prefix + "Missing");
		var library = EmitLibrary(libraryDir, libraryName);
		var consumer = EmitConsumer(consumerDir, UniqueName(prefix + "Orphan"), library);
		Directory.Delete(libraryDir, recursive: true);
		return (consumer, libraryName);
	}
}
