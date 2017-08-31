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
using System.Linq;
using System.Reflection;
using ICSharpCode.Decompiler.TypeSystem;
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
				using (var module = ModuleDefinition.ReadModule(typeof(TypeSystem).Module.FullyQualifiedName)) {
					return new DecompilerTypeSystem(module);
				}
			});
		
		public static DecompilerTypeSystem Instance {
			get { return decompilerTypeSystem.Value; }
		}
		
		public static IAssembly FromReflection(Assembly assembly)
		{
			return decompilerTypeSystem.Value.Compilation.Assemblies.Single(asm => asm.AssemblyName == assembly.GetName().Name);
		}
		
		public static IType FromReflection(Type type)
		{
			return decompilerTypeSystem.Value.Compilation.FindType(type);
		}
		
		/// <summary>
		/// Retrieves a static method that returns void and takes the specified parameter types.
		/// </summary>
		public static IMethod Action<T1>()
		{
			return Action(typeof(T1));
		}
		
		/// <summary>
		/// Retrieves a static method that returns void and takes the specified parameter types.
		/// </summary>
		public static IMethod Action<T1, T2>()
		{
			return Action(typeof(T1), typeof(T2));
		}
		
		/// <summary>
		/// Retrieves a static method that returns void and takes the specified parameter types.
		/// </summary>
		public static IMethod Action<T1, T2, T3>()
		{
			return Action(typeof(T1), typeof(T2), typeof(T3));
		}
		
		/// <summary>
		/// Retrieves a static method that returns void and takes the specified parameter types.
		/// </summary>
		public static IMethod Action<T1, T2, T3, T4>()
		{
			return Action(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
		}
		
		/// <summary>
		/// Retrieves a static method that returns void and takes the specified parameter types.
		/// </summary>
		public static IMethod Action(params Type[] paramTypes)
		{
			return Action(paramTypes.Select(FromReflection).ToArray());
		}
		
		/// <summary>
		/// Retrieves a static method that returns void and takes the specified parameter types.
		/// </summary>
		public static IMethod Action(params IType[] paramTypes)
		{
			return FromReflection(typeof(Actions)).GetMethods(
				m => m.Name == "Action" && m.TypeParameters.Count == paramTypes.Length && m.Parameters.Count == paramTypes.Length)
				.Single();
		}
	}
	
	static class Actions
	{
		public static void Action() {}
		public static void Action<T1>(T1 a1) {}
		public static void Action<T1, T2>(T1 a1, T2 a2) {}
		public static void Action<T1, T2, T3>(T1 a1, T2 a2, T3 a3) {}
		public static void Action<T1, T2, T3, T4>(T1 a1, T2 a2, T3 a3, T4 a4) {}
	}
}
