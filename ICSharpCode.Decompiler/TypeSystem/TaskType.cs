// Copyright (c) 2010-2014 AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Helper class for dealing with System.Threading.Tasks.Task.
	/// </summary>
	public static class TaskType
	{
		/// <summary>
		/// Gets the T in Task&lt;T&gt;.
		/// Returns void for non-generic Task.
		/// Any other type is returned unmodified.
		/// </summary>
		public static IType UnpackTask(ICompilation compilation, IType type)
		{
			if (!IsTask(type))
				return type;
			if (type.TypeParameterCount == 0)
				return compilation.FindType(KnownTypeCode.Void);
			else
				return type.TypeArguments[0];
		}

		/// <summary>
		/// Gets whether the specified type is Task or Task&lt;T&gt;.
		/// </summary>
		public static bool IsTask(IType type)
		{
			ITypeDefinition def = type.GetDefinition();
			if (def != null)
			{
				if (def.KnownTypeCode == KnownTypeCode.Task)
					return true;
				if (def.KnownTypeCode == KnownTypeCode.TaskOfT)
					return type is ParameterizedType;
			}
			return false;
		}

		/// <summary>
		/// Gets whether the specified type is a Task-like type.
		/// </summary>
		public static bool IsCustomTask(IType type, out IType builderType)
		{
			builderType = null;
			ITypeDefinition def = type.GetDefinition();
			if (def != null)
			{
				if (def.TypeParameterCount > 1)
					return false;
				var attribute = def.GetAttribute(KnownAttribute.AsyncMethodBuilder);
				if (attribute == null || attribute.FixedArguments.Length != 1)
					return false;
				var arg = attribute.FixedArguments[0];
				if (!arg.Type.IsKnownType(KnownTypeCode.Type))
					return false;
				builderType = (IType)arg.Value;
				return true;
			}
			return false;
		}

		const string ns = "System.Runtime.CompilerServices";

		/// <summary>
		/// Gets whether the specified type is a non-generic Task-like type.
		/// </summary>
		/// <param name="builderTypeName">Returns the full type-name of the builder type, if successful.</param>
		public static bool IsNonGenericTaskType(IType task, out FullTypeName builderTypeName)
		{
			if (task.IsKnownType(KnownTypeCode.Task))
			{
				builderTypeName = new TopLevelTypeName(ns, "AsyncTaskMethodBuilder");
				return true;
			}
			if (IsCustomTask(task, out var builderType))
			{
				builderTypeName = new FullTypeName(builderType.ReflectionName);
				return builderTypeName.TypeParameterCount == 0;
			}
			builderTypeName = default;
			return false;
		}

		/// <summary>
		/// Gets whether the specified type is a generic Task-like type.
		/// </summary>
		/// <param name="builderTypeName">Returns the full type-name of the builder type, if successful.</param>
		public static bool IsGenericTaskType(IType task, out FullTypeName builderTypeName)
		{
			if (task.IsKnownType(KnownTypeCode.TaskOfT))
			{
				builderTypeName = new TopLevelTypeName(ns, "AsyncTaskMethodBuilder", 1);
				return true;
			}
			if (IsCustomTask(task, out var builderType))
			{
				builderTypeName = new FullTypeName(builderType.ReflectionName);
				return builderTypeName.TypeParameterCount == 1;
			}
			builderTypeName = default;
			return false;
		}

		/// <summary>
		/// Creates a task type.
		/// </summary>
		public static IType Create(ICompilation compilation, IType elementType)
		{
			if (compilation == null)
				throw new ArgumentNullException(nameof(compilation));
			if (elementType == null)
				throw new ArgumentNullException(nameof(elementType));

			if (elementType.Kind == TypeKind.Void)
				return compilation.FindType(KnownTypeCode.Task);
			IType taskType = compilation.FindType(KnownTypeCode.TaskOfT);
			ITypeDefinition taskTypeDef = taskType.GetDefinition();
			if (taskTypeDef != null)
				return new ParameterizedType(taskTypeDef, new[] { elementType });
			else
				return taskType;
		}
	}
}
