// Copyright (c) 2014 Daniel Grunwald
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

using ICSharpCode.NRefactory.TypeSystem;
using System;
using System.Linq;

namespace ICSharpCode.Decompiler.IL
{
	public enum VariableKind
	{
		/// <summary>
		/// A local variable.
		/// </summary>
		Local,
		/// <summary>
		/// A pinned local variable
		/// </summary>
		PinnedLocal,
		/// <summary>
		/// A parameter.
		/// </summary>
		Parameter,
		/// <summary>
		/// The 'this' parameter
		/// </summary>
		This,
		/// <summary>
		/// Variable created for exception handler
		/// </summary>
		Exception,
		StackSlot
	}

	public class ILVariable
	{
		public readonly VariableKind Kind;
		public readonly IType Type;
		public readonly StackType StackType;
		
		/// <summary>
		/// The index of the local variable or parameter (depending on Kind)
		/// </summary>
		public readonly int Index;
		
		public string Name { get; set; }

		/// <summary>
		/// Gets the scope in which this variable is declared.
		/// </summary>
		/// <remarks>
		/// This property is set automatically when the variable is added to the <c>VariableScope.Variables</c> collection.
		/// </remarks>
		public ILVariableScope Scope { get; internal set; }
		
		/// <summary>
		/// Gets the index of this variable within the <c>VariableScope.Variables</c> collection.
		/// </summary>
		/// <remarks>
		/// This property is set automatically when the variable is added to the <c>VariableScope.Variables</c> collection.
		/// It may change if an item with a lower index is removed from the collection.
		/// </remarks>
		public int IndexInScope { get; internal set; }
		
		/// <summary>
		/// Number of ldloc instructions referencing this variable.
		/// </summary>
		/// <remarks>
		/// This variable is automatically updated when adding/removing ldloc instructions from the ILAst.
		/// </remarks>
		public int LoadCount;
		
		/// <summary>
		/// Number of stloc instructions referencing this variable.
		/// </summary>
		/// <remarks>
		/// This variable is automatically updated when adding/removing stloc instructions from the ILAst.
		/// </remarks>
		public int StoreCount;
		
		/// <summary>
		/// Number of ldloca instructions referencing this variable.
		/// </summary>
		/// <remarks>
		/// This variable is automatically updated when adding/removing ldloca instructions from the ILAst.
		/// </remarks>
		public int AddressCount;

		public bool IsSingleDefinition {
			get {
				return StoreCount == 1 && AddressCount == 0;
			}
		}
		
		public ILVariable(VariableKind kind, IType type, int index)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			this.Kind = kind;
			this.Type = type;
			this.StackType = type.GetStackType();
			this.Index = index;
		}
		
		public ILVariable(VariableKind kind, IType type, StackType stackType, int index)
		{
			this.Kind = kind;
			this.Type = type;
			this.StackType = stackType;
			this.Index = index;
		}
		
		public override string ToString()
		{
			return Name;
		}
		
		internal void WriteDefinitionTo(ITextOutput output)
		{
			output.WriteDefinition(this.Name, this, isLocal: true);
			output.Write(" : ");
			if (Kind == VariableKind.PinnedLocal)
				output.Write("pinned ");
			Type.WriteTo(output);
			output.Write("({0} ldloc, {1} ldloca, {2} stloc)", LoadCount, AddressCount, StoreCount);
		}
		
		internal void WriteTo(ITextOutput output)
		{
			output.WriteReference(this.Name, this, isLocal: true);
		}
	}
}
