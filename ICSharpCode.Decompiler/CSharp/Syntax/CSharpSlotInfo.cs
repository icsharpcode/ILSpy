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
	/// its parent, so <c>node.Slot == X.YSlot</c> identifies a node's position by object identity.
	/// </summary>
	public class CSharpSlotInfo
	{
		/// <summary>The slot's property name, e.g. "Left", "Body", "Parameters".</summary>
		public string Name { get; }

		/// <summary>The declared child type (the element type for a collection slot).</summary>
		public Type ChildType { get; }

		/// <summary>Whether the slot holds a collection of children rather than a single child.</summary>
		public bool IsCollection { get; }

		/// <summary>
		/// The slot's kind: the canonical shared slot for this child position, identifying it across node
		/// types by object identity (so <c>node.Slot.Kind == Slots.X</c> identifies a node's position
		/// polymorphically). A per-node slot points at its
		/// shared <c>Slots</c> constant; a <c>Slots</c> constant is itself the kind, so its own
		/// <see cref="Kind"/> is null (and is never read -- only per-node slots are asked for their kind).
		/// </summary>
		public CSharpSlotInfo? Kind { get; }

		/// <summary>
		/// Whether the slot may be empty: a single slot whose child is nullable, or any collection slot
		/// (a collection may hold zero children). A non-optional single slot must always hold a child in a
		/// well-formed tree; <see cref="AstNode.CheckInvariant"/> asserts this.
		/// </summary>
		public bool IsOptional { get; }

		internal CSharpSlotInfo(string name, Type childType, bool isCollection, CSharpSlotInfo? kind, bool isOptional)
		{
			Name = name;
			ChildType = childType;
			IsCollection = isCollection;
			Kind = kind;
			IsOptional = isOptional;
		}

		public override string ToString() => Name;
	}

	/// <summary>
	/// A <see cref="CSharpSlotInfo"/> that carries its child type as a type parameter, so the typed
	/// child accessors (<c>node.GetChild(SomeNode.XSlot)</c>) infer the result type from the slot
	/// instead of needing an explicit type argument. <typeparamref name="T"/> is the child type (the
	/// element type for a collection slot).
	/// </summary>
	public sealed class CSharpSlotInfo<T> : CSharpSlotInfo where T : AstNode
	{
		internal CSharpSlotInfo(string name, bool isCollection, CSharpSlotInfo? kind, bool isOptional)
			: base(name, typeof(T), isCollection, kind, isOptional)
		{
		}
	}
}
