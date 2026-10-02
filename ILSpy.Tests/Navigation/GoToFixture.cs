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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Emits a small inheritance hierarchy for the go-to navigation tests:
/// <code>
/// interface IShape { double Area(); }
/// abstract class ShapeBase : IShape { public abstract double Area(); }
/// class Circle : ShapeBase { public override double Area(); }
/// class Square : ShapeBase { public override double Area(); }
/// class Lonely { public void Solo(); }
/// </code>
/// </summary>
public static class GoToFixture
{
	public const string Name = "GoToFixture";

	public static string Emit()
	{
		var ab = new PersistedAssemblyBuilder(new AssemblyName(Name), typeof(object).Assembly);
		var module = ab.DefineDynamicModule(Name);

		var shape = module.DefineType($"{Name}.IShape",
			TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);
		shape.DefineMethod("Area",
			MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual
			| MethodAttributes.HideBySig | MethodAttributes.NewSlot,
			typeof(double), Type.EmptyTypes);
		shape.CreateType();

		var shapeBase = module.DefineType($"{Name}.ShapeBase",
			TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Class, typeof(object));
		shapeBase.AddInterfaceImplementation(shape);
		shapeBase.DefineMethod("Area",
			MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual
			| MethodAttributes.HideBySig | MethodAttributes.NewSlot,
			typeof(double), Type.EmptyTypes);
		shapeBase.DefineDefaultConstructor(MethodAttributes.Family);
		shapeBase.CreateType();

		DefineShape(module, shapeBase, "Circle", 3.14);
		DefineShape(module, shapeBase, "Square", 4.0);

		var lonely = module.DefineType($"{Name}.Lonely", TypeAttributes.Public | TypeAttributes.Class);
		var solo = lonely.DefineMethod("Solo", MethodAttributes.Public | MethodAttributes.HideBySig,
			typeof(void), Type.EmptyTypes);
		solo.GetILGenerator().Emit(OpCodes.Ret);
		lonely.CreateType();

		var dir = Path.Combine(Path.GetTempPath(), $"ILSpyGoToFixture_{Guid.NewGuid():N}");
		Directory.CreateDirectory(dir);
		var path = Path.Combine(dir, $"{Name}.dll");
		ab.Save(path);
		return path;
	}

	static void DefineShape(ModuleBuilder module, TypeBuilder shapeBase, string name, double area)
	{
		var type = module.DefineType($"{Name}.{name}", TypeAttributes.Public | TypeAttributes.Class, shapeBase);
		var method = type.DefineMethod("Area",
			MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
			typeof(double), Type.EmptyTypes);
		var il = method.GetILGenerator();
		il.Emit(OpCodes.Ldc_R8, area);
		il.Emit(OpCodes.Ret);
		type.DefineDefaultConstructor(MethodAttributes.Public);
		type.CreateType();
	}

	public static async Task OpenAsync(MainWindowViewModel vm)
	{
		await vm.OpenAssemblyAsync(Emit());
	}

	public static TypeTreeNode TypeNode(MainWindowViewModel vm, string typeName)
		=> vm.AssemblyTreeModel.FindNode<TypeTreeNode>(Name, Name, $"{Name}.{typeName}");

	public static MethodTreeNode MethodNode(MainWindowViewModel vm, string typeName, string methodName)
	{
		var typeNode = TypeNode(vm, typeName);
		typeNode.IsExpanded = true;
		return typeNode.Children.OfType<MethodTreeNode>().Single(m => m.MethodDefinition.Name == methodName);
	}

	public static ITypeDefinition TypeDef(MainWindowViewModel vm, string typeName)
		=> TypeNode(vm, typeName).TypeDefinition;

	public static IMethod Method(MainWindowViewModel vm, string typeName, string methodName)
		=> MethodNode(vm, typeName, methodName).MethodDefinition;

	/// <summary>"Type.Member" or "Type" for a navigation target, for order-insensitive assertions.</summary>
	public static string Describe(IEntity entity)
		=> entity is ITypeDefinition td ? td.Name : $"{entity.DeclaringTypeDefinition?.Name}.{entity.Name}";
}
