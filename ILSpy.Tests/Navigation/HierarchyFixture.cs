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
using System.Threading.Tasks;

using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Emits a small assembly with a known inheritance graph for the type-hierarchy and
/// file-structure tests:
/// <code>
/// interface IShape { double Area(); }
/// abstract class ShapeBase : IShape { abstract double Area(); }
/// class Circle : ShapeBase { double radius; double Area(); double Diameter(); }
/// class BigCircle : Circle { }
/// class Square : ShapeBase { double Area(); }
/// </code>
/// All types live in the <see cref="Namespace"/> namespace of an assembly with the same name.
/// </summary>
static class HierarchyFixture
{
	public const string Namespace = "Shapes";

	public static string Emit()
	{
		var ab = new PersistedAssemblyBuilder(new AssemblyName(Namespace), typeof(object).Assembly);
		var module = ab.DefineDynamicModule(Namespace);
		var objectCtor = typeof(object).GetConstructor(Type.EmptyTypes)!;

		var shape = module.DefineType($"{Namespace}.IShape",
			TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);
		shape.DefineMethod("Area",
			MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual
			| MethodAttributes.HideBySig | MethodAttributes.NewSlot,
			typeof(double), Type.EmptyTypes);
		shape.CreateType();

		var shapeBase = module.DefineType($"{Namespace}.ShapeBase",
			TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract, typeof(object), [shape]);
		shapeBase.DefineMethod("Area",
			MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual
			| MethodAttributes.HideBySig | MethodAttributes.NewSlot,
			typeof(double), Type.EmptyTypes);
		var shapeBaseCtor = DefineCtor(shapeBase, objectCtor, MethodAttributes.Family);
		shapeBase.CreateType();

		var circle = module.DefineType($"{Namespace}.Circle", TypeAttributes.Public | TypeAttributes.Class, shapeBase);
		var radius = circle.DefineField("radius", typeof(double), FieldAttributes.Private);
		DefineAreaOverride(circle, 3.0);
		var diameter = circle.DefineMethod("Diameter",
			MethodAttributes.Public | MethodAttributes.HideBySig, typeof(double), Type.EmptyTypes);
		var il = diameter.GetILGenerator();
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Ldfld, radius);
		il.Emit(OpCodes.Ldc_R8, 2.0);
		il.Emit(OpCodes.Mul);
		il.Emit(OpCodes.Ret);
		var circleCtor = DefineCtor(circle, shapeBaseCtor, MethodAttributes.Public);
		circle.CreateType();

		var bigCircle = module.DefineType($"{Namespace}.BigCircle", TypeAttributes.Public | TypeAttributes.Class, circle);
		DefineCtor(bigCircle, circleCtor, MethodAttributes.Public);
		bigCircle.CreateType();

		var square = module.DefineType($"{Namespace}.Square", TypeAttributes.Public | TypeAttributes.Class, shapeBase);
		DefineAreaOverride(square, 4.0);
		DefineCtor(square, shapeBaseCtor, MethodAttributes.Public);
		square.CreateType();

		var dir = Path.Combine(Path.GetTempPath(), $"ILSpyHierarchyFixture_{Guid.NewGuid():N}");
		Directory.CreateDirectory(dir);
		var path = Path.Combine(dir, $"{Namespace}.dll");
		ab.Save(path);
		return path;
	}

	static ConstructorInfo DefineCtor(TypeBuilder type, ConstructorInfo baseCtor, MethodAttributes access)
	{
		var ctor = type.DefineConstructor(access | MethodAttributes.HideBySig | MethodAttributes.SpecialName
			| MethodAttributes.RTSpecialName, CallingConventions.Standard, Type.EmptyTypes);
		var il = ctor.GetILGenerator();
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Call, baseCtor);
		il.Emit(OpCodes.Ret);
		return ctor;
	}

	static void DefineAreaOverride(TypeBuilder type, double value)
	{
		var area = type.DefineMethod("Area",
			MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
			typeof(double), Type.EmptyTypes);
		var il = area.GetILGenerator();
		il.Emit(OpCodes.Ldc_R8, value);
		il.Emit(OpCodes.Ret);
	}

	/// <summary>Opens the fixture through the production Open command.</summary>
	public static Task OpenAsync(MainWindowViewModel vm) => vm.OpenAssemblyAsync(Emit());

	/// <summary>The assembly-tree node of the fixture type named <paramref name="name"/>.</summary>
	public static TypeTreeNode FindType(MainWindowViewModel vm, string name)
		=> vm.AssemblyTreeModel.FindNode<TypeTreeNode>(Namespace, Namespace, $"{Namespace}.{name}");
}
