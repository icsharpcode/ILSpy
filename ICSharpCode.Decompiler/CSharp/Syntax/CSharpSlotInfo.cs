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

#nullable enable

using System;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Describes one child slot of a node type: its name, the declared child type, and whether it is a
	/// collection. Each slot has a single generated static instance (e.g.
	/// <c>BinaryOperatorExpression.LeftSlot</c>); <c>node.Slot</c> returns the slot a node occupies in
	/// its parent, so <c>node.Slot == X.YSlot</c> identifies a node's position by object identity (the
	/// successor to <c>node.Role == X.YRole</c>).
	/// </summary>
	public sealed class CSharpSlotInfo
	{
		/// <summary>The slot's property name, e.g. "Left", "Body", "Parameters".</summary>
		public string Name { get; }

		/// <summary>The declared child type (the element type for a collection slot).</summary>
		public Type ChildType { get; }

		/// <summary>Whether the slot holds a collection of children rather than a single child.</summary>
		public bool IsCollection { get; }

		/// <summary>
		/// Whether the slot is syntactically optional. Populated accurately once the null-object model
		/// is replaced by nullable reference types; currently informational.
		/// </summary>
		public bool IsOptional { get; }

		/// <summary>
		/// The slot's kind, shared across node types (so <c>node.Slot.Kind == SlotKind.X</c> identifies a
		/// node's position the way <c>node.Role == Roles.X</c> did, including polymorphically).
		/// </summary>
		public SlotKind Kind { get; }

		internal CSharpSlotInfo(string name, Type childType, bool isCollection, SlotKind kind, bool isOptional = false)
		{
			Name = name;
			ChildType = childType;
			IsCollection = isCollection;
			Kind = kind;
			IsOptional = isOptional;
		}

		public override string ToString() => Name;
	}
}
