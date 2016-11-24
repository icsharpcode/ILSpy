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
		/// Variable created for exception handler
		/// </summary>
		Exception,
		/// <summary>
		/// Variable created from stack slot.
		/// </summary>
		StackSlot
	}

	public class ILVariable
	{
		public readonly VariableKind Kind;
		public readonly StackType StackType;
		
		IType type;
		public IType Type {
			get {
				return type;
			}
			internal set {
				if (value.GetStackType() != StackType)
					throw new ArgumentException($"Expected stack-type: {StackType} may not be changed. Found: {value.GetStackType()}");
				type = value;
			}
		}
		
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
		public int LoadCount { get; internal set; }
		
		/// <summary>
		/// Number of store instructions referencing this variable.
		/// 
		/// Stores are:
		/// <list type="item">
		/// <item>stloc</item>
		/// <item>TryCatchHandler (assigning the exception variable)</item>
		/// <item>PinnedRegion (assigning the pointer variable)</item>
		/// <item>initial values (<see cref="HasInitialValue"/>)</item>
		/// </list>
		/// </summary>
		/// <remarks>
		/// This variable is automatically updated when adding/removing stores instructions from the ILAst.
		/// </remarks>
		public int StoreCount { get; internal set; }
		
		/// <summary>
		/// Number of ldloca instructions referencing this variable.
		/// </summary>
		/// <remarks>
		/// This variable is automatically updated when adding/removing ldloca instructions from the ILAst.
		/// </remarks>
		public int AddressCount { get; internal set; }

		bool hasInitialValue;
		
		/// <summary>
		/// Gets/Sets whether the variable has an initial value.
		/// This is always <c>true</c> for parameters (incl. <c>this</c>).
		/// 
		/// Normal variables have an initial value if the function uses ".locals init"
		/// and that initialization is not a dead store.
		/// </summary>
		/// <remarks>
		/// An initial value is counted as a store (adds 1 to StoreCount)
		/// </remarks>
		public bool HasInitialValue {
			get { return hasInitialValue; }
			set {
				if (Kind == VariableKind.Parameter && !value)
					throw new InvalidOperationException("Cannot remove HasInitialValue from parameters");
				if (hasInitialValue) {
					StoreCount--;
				}
				hasInitialValue = value;
				if (value) {
					StoreCount++;
				}
			}
		}
		
		public bool IsSingleDefinition {
			get {
				return StoreCount == 1 && AddressCount == 0;
			}
		}
		
		public ILVariable(VariableKind kind, IType type, int index)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			this.Kind = kind;
			this.type = type;
			this.StackType = type.GetStackType();
			this.Index = index;
			if (kind == VariableKind.Parameter)
				this.HasInitialValue = true;
		}
		
		public ILVariable(VariableKind kind, IType type, StackType stackType, int index)
		{
			this.Kind = kind;
			this.type = type;
			this.StackType = stackType;
			this.Index = index;
			if (kind == VariableKind.Parameter)
				this.HasInitialValue = true;
		}
		
		public override string ToString()
		{
			return Name;
		}
		
		internal void WriteDefinitionTo(ITextOutput output)
		{
			switch (Kind) {
				case VariableKind.Local:
					output.Write("local ");
					break;
				case VariableKind.PinnedLocal:
					output.Write("pinned local ");
					break;
				case VariableKind.Parameter:
					output.Write("param ");
					break;
				case VariableKind.Exception:
					output.Write("exception ");
					break;
				case VariableKind.StackSlot:
					output.Write("stack ");
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			output.WriteDefinition(this.Name, this, isLocal: true);
			output.Write(" : ");
			Type.WriteTo(output);
			output.Write('(');
			if (Kind == VariableKind.Parameter || Kind == VariableKind.Local || Kind == VariableKind.PinnedLocal) {
				output.Write("Index={0}, ", Index);
			}
			output.Write("LoadCount={0}, AddressCount={1}, StoreCount={2})", LoadCount, AddressCount, StoreCount);
			if (hasInitialValue && Kind != VariableKind.Parameter) {
				output.Write(" init");
			}
		}
		
		internal void WriteTo(ITextOutput output)
		{
			output.WriteReference(this.Name, this, isLocal: true);
		}
	}
	
	public interface IInstructionWithVariableOperand
	{
		ILVariable Variable { get; set; }
	}
}
