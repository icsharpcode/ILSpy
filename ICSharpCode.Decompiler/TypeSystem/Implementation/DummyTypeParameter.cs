// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.Threading;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	public sealed class DummyTypeParameter : AbstractType, ITypeParameter
	{
		static ITypeParameter[] methodTypeParameters = { new DummyTypeParameter(SymbolKind.Method, 0) };
		static ITypeParameter[] classTypeParameters = { new DummyTypeParameter(SymbolKind.TypeDefinition, 0) };
		static IReadOnlyList<ITypeParameter>[] classTypeParameterLists = { EmptyList<ITypeParameter>.Instance };
		
		public static ITypeParameter GetMethodTypeParameter(int index)
		{
			return GetTypeParameter(ref methodTypeParameters, SymbolKind.Method, index);
		}
		
		public static ITypeParameter GetClassTypeParameter(int index)
		{
			return GetTypeParameter(ref classTypeParameters, SymbolKind.TypeDefinition, index);
		}
		
		static ITypeParameter GetTypeParameter(ref ITypeParameter[] typeParameters, SymbolKind symbolKind, int index)
		{
			ITypeParameter[] tps = typeParameters;
			while (index >= tps.Length) {
				// We don't have a normal type parameter for this index, so we need to extend our array.
				// Because the array can be used concurrently from multiple threads, we have to use
				// Interlocked.CompareExchange.
				ITypeParameter[] newTps = new ITypeParameter[index + 1];
				tps.CopyTo(newTps, 0);
				for (int i = tps.Length; i < newTps.Length; i++) {
					newTps[i] = new DummyTypeParameter(symbolKind, i);
				}
				ITypeParameter[] oldTps = Interlocked.CompareExchange(ref typeParameters, newTps, tps);
				if (oldTps == tps) {
					// exchange successful
					tps = newTps;
				} else {
					// exchange not successful
					tps = oldTps;
				}
			}
			return tps[index];
		}

		/// <summary>
		/// Gets a list filled with dummy type parameters.
		/// </summary>
		internal static IReadOnlyList<ITypeParameter> GetClassTypeParameterList(int length)
		{
			IReadOnlyList<ITypeParameter>[] tps = classTypeParameterLists;
			while (length >= tps.Length) {
				// We don't have a normal type parameter for this index, so we need to extend our array.
				// Because the array can be used concurrently from multiple threads, we have to use
				// Interlocked.CompareExchange.
				IReadOnlyList<ITypeParameter>[] newTps = new IReadOnlyList<ITypeParameter>[length + 1];
				tps.CopyTo(newTps, 0);
				for (int i = tps.Length; i < newTps.Length; i++) {
					var newList = new ITypeParameter[i];
					for (int j = 0; j < newList.Length; j++) {
						newList[j] = GetClassTypeParameter(j);
					}
					newTps[i] = newList;
				}
				var oldTps = Interlocked.CompareExchange(ref classTypeParameterLists, newTps, tps);
				if (oldTps == tps) {
					// exchange successful
					tps = newTps;
				} else {
					// exchange not successful
					tps = oldTps;
				}
			}
			return tps[length];
		}
		
		readonly SymbolKind ownerType;
		readonly int index;
		
		private DummyTypeParameter(SymbolKind ownerType, int index)
		{
			this.ownerType = ownerType;
			this.index = index;
		}
		
		SymbolKind ISymbol.SymbolKind {
			get { return SymbolKind.TypeParameter; }
		}
		
		public override string Name {
			get {
				return (ownerType == SymbolKind.Method ? "!!" : "!") + index;
			}
		}
		
		public override string ReflectionName {
			get {
				return (ownerType == SymbolKind.Method ? "``" : "`") + index;
			}
		}
		
		public override string ToString()
		{
			return ReflectionName + " (dummy)";
		}
		
		public override bool? IsReferenceType {
			get { return null; }
		}
		
		public override TypeKind Kind {
			get { return TypeKind.TypeParameter; }
		}
		
		public override IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitTypeParameter(this);
		}
		
		public int Index {
			get { return index; }
		}
		
		IEnumerable<IAttribute> ITypeParameter.GetAttributes() =>EmptyList<IAttribute>.Instance;
		
		SymbolKind ITypeParameter.OwnerType {
			get { return ownerType; }
		}
		
		VarianceModifier ITypeParameter.Variance {
			get { return VarianceModifier.Invariant; }
		}
		
		IEntity ITypeParameter.Owner {
			get { return null; }
		}
		
		IType ITypeParameter.EffectiveBaseClass {
			get { return SpecialType.UnknownType; }
		}
		
		IReadOnlyCollection<IType> ITypeParameter.EffectiveInterfaceSet {
			get { return EmptyList<IType>.Instance; }
		}

		bool ITypeParameter.HasDefaultConstructorConstraint => false;
		bool ITypeParameter.HasReferenceTypeConstraint => false;
		bool ITypeParameter.HasValueTypeConstraint => false;
		bool ITypeParameter.HasUnmanagedConstraint => false;
		Nullability ITypeParameter.NullabilityConstraint => Nullability.Oblivious;

		IReadOnlyList<TypeConstraint> ITypeParameter.TypeConstraints => EmptyList<TypeConstraint>.Instance;

		public override IType ChangeNullability(Nullability nullability)
		{
			if (nullability == Nullability.Oblivious) {
				return this;
			} else {
				return new NullabilityAnnotatedTypeParameter(this, nullability);
			}
		}
	}
}
