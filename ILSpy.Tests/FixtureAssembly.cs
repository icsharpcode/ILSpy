// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Threading.Tasks;

using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Emits a tiny throwaway assembly to a temp file so the project-export / save-code / compare
/// tests can exercise the full pipeline without decompiling a multi-hundred-type framework
/// assembly. The default assembly list loads System.Linq, CoreLib and the Uri assembly; targeting
/// those made each of these tests run for ~10s purely on the type count. A handful of public
/// members is enough to produce a non-empty .cs, a real namespace subtree and a resolvable
/// .csproj, while keeping each test well under a second.
/// </summary>
public static class FixtureAssembly
{
	/// <summary>Short name of the single public type a caller can assert on / navigate to.</summary>
	public const string TypeName = "Greeter";

	/// <summary>Name of a member of <see cref="TypeName"/> a caller can assert is decompiled.</summary>
	public const string MemberName = "Hello";

	/// <summary>
	/// Emits a fresh fixture assembly named <paramref name="name"/> to a temp file and returns its
	/// path. Each call produces a distinct file so callers can build solutions / comparisons out of
	/// several fixtures.
	/// </summary>
	public static string Emit(string name)
	{
		var ab = new PersistedAssemblyBuilder(new AssemblyName(name), typeof(object).Assembly);
		var module = ab.DefineDynamicModule(name);

		var greeter = module.DefineType($"{name}.{TypeName}", TypeAttributes.Public | TypeAttributes.Class);
		var hello = greeter.DefineMethod(
			MemberName, MethodAttributes.Public | MethodAttributes.Static, typeof(string), Type.EmptyTypes);
		var il = hello.GetILGenerator();
		il.Emit(OpCodes.Ldstr, $"hello from {name}");
		il.Emit(OpCodes.Ret);
		greeter.CreateType();

		// A second type in a nested namespace so the namespace subtree and the project layout are
		// not entirely trivial.
		var widget = module.DefineType($"{name}.Widgets.Widget", TypeAttributes.Public | TypeAttributes.Class);
		var add = widget.DefineMethod(
			"Add", MethodAttributes.Public | MethodAttributes.Static, typeof(int), [typeof(int), typeof(int)]);
		var il2 = add.GetILGenerator();
		il2.Emit(OpCodes.Ldarg_0);
		il2.Emit(OpCodes.Ldarg_1);
		il2.Emit(OpCodes.Add);
		il2.Emit(OpCodes.Ret);
		widget.CreateType();

		// Unique directory (not filename) so the file is "<name>.dll": the assembly's ShortName then
		// equals <name> and matches the "<name>" namespace prefix the types use, keeping FindNode
		// paths simple.
		var dir = Path.Combine(Path.GetTempPath(), $"ILSpyFixture_{Guid.NewGuid():N}");
		Directory.CreateDirectory(dir);
		var path = Path.Combine(dir, $"{name}.dll");
		ab.Save(path);
		return path;
	}

	/// <summary>
	/// Emits a fixture assembly and opens it through the production Open command, returning the
	/// loaded assembly once its load result is available.
	/// </summary>
	public static Task<LoadedAssembly> OpenFixtureAsync(this MainWindowViewModel vm, string name = "Fixture")
	{
		ArgumentNullException.ThrowIfNull(vm);
		return vm.OpenAssemblyAsync(Emit(name));
	}
}
