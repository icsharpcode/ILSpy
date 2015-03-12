// Copyright (c) 2015 Daniel Grunwald
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
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	/// <summary>
	/// Helper class for building NRefactory type systems in test cases.
	/// </summary>
	public static class TypeSystem
	{
		static readonly Lazy<DecompilerTypeSystem> decompilerTypeSystem = new Lazy<DecompilerTypeSystem>(
			delegate {
				var module = ModuleDefinition.ReadModule(typeof(TypeSystem).Module.FullyQualifiedName);
				return new DecompilerTypeSystem(module);
			});
		
		public static IAssembly FromReflection(Assembly assembly)
		{
			return decompilerTypeSystem.Value.Compilation.Assemblies.Single(asm => asm.AssemblyName == assembly.GetName().Name);
		}
		
		public static IType FromReflection(Type type)
		{
			return decompilerTypeSystem.Value.Compilation.FindType(type);
		}
	}
}
