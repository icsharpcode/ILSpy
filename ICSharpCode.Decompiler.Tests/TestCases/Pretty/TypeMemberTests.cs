// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class T01_IndexerWithGetOnly
	{
#if ROSLYN
		public int this[int i] => i;
#else
		public int this[int i] {
			get {
				return i;
			}
		}
#endif
	}
	public class T02_IndexerWithSetOnly
	{
		public int this[int i] {
			set {
			}
		}
	}

	public class T03_IndexerWithMoreParameters
	{
#if ROSLYN
		public int this[int i, string s, Type t] => 0;
#else
		public int this[int i, string s, Type t] {
			get {
				return 0;
			}
		}
#endif
	}

	public class T04_IndexerInGenericClass<T>
	{
#if ROSLYN
		public int this[T t] => 0;
#else
		public int this[T t] {
			get {
				return 0;
			}
		}
#endif
	}
	public class T05_OverloadedIndexer
	{
#if ROSLYN
		public int this[int t] => 0;
#else
		public int this[int t] {
			get {
				return 0;
			}
		}
#endif
		public int this[string s] {
			get {
				return 0;
			}
			set {
				Console.WriteLine(value + " " + s);
			}
		}
	}
	public interface T06_IIndexerInInterface
	{
		int this[string s, string s2] {
			set;
		}
	}
	public interface T07_IMyInterface_IndexerInterfaceExplicitImplementation
	{
		int this[string s] {
			get;
		}
	}
	public class T07_MyClass_IndexerInterfaceExplicitImplementation : T07_IMyInterface_IndexerInterfaceExplicitImplementation
	{
#if ROSLYN
		int T07_IMyInterface_IndexerInterfaceExplicitImplementation.this[string s] => 3;
#else
		int T07_IMyInterface_IndexerInterfaceExplicitImplementation.this[string s] {
			get {
				return 3;
			}
		}
#endif
	}
	public interface T08_IMyInterface_IndexerInterfaceImplementation
	{
		int this[string s] {
			get;
		}
	}
	public class T08_MyClass_IndexerInterfaceImplementation : T08_IMyInterface_IndexerInterfaceImplementation
	{
#if ROSLYN
		public int this[string s] => 3;
#else
		public int this[string s] {
			get {
				return 3;
			}
		}
#endif
	}

	public interface T09_IMyInterface_MethodExplicit
	{
		void MyMethod();
	}
	public abstract class T09_MyClass_IndexerAbstract
	{
		public abstract int this[string s, string s2] {
			set;
		}
		protected abstract string this[int index] {
			get;
		}
	}
	public class T09_MyClass_MethodExplicit : T09_IMyInterface_MethodExplicit
	{
		void T09_IMyInterface_MethodExplicit.MyMethod()
		{
		}
	}

	public interface T10_IMyInterface_MethodFromInterfaceVirtual
	{
		void MyMethod();
	}
	public class T10_MyClass : T10_IMyInterface_MethodFromInterfaceVirtual
	{
		public virtual void MyMethod()
		{
		}
	}
	public interface T11_IMyInterface_MethodFromInterface
	{
		void MyMethod();
	}
	public class T11_MyClass_MethodFromInterface : T11_IMyInterface_MethodFromInterface
	{
		public void MyMethod()
		{
		}
	}
	public interface T12_IMyInterface_MethodFromInterfaceAbstract
	{
		void MyMethod();
	}
	public abstract class T12_MyClass_MethodFromInterfaceAbstract : T12_IMyInterface_MethodFromInterfaceAbstract
	{
		public abstract void MyMethod();
	}
	public interface T13_IMyInterface_PropertyInterface
	{
		int MyProperty {
			get;
			set;
		}
	}
	public interface T14_IMyInterface_PropertyInterfaceExplicitImplementation
	{
		int MyProperty {
			get;
			set;
		}
	}
	public class T14_MyClass_PropertyInterfaceExplicitImplementation : T14_IMyInterface_PropertyInterfaceExplicitImplementation
	{
		int T14_IMyInterface_PropertyInterfaceExplicitImplementation.MyProperty {
			get {
				return 0;
			}
			set {
			}
		}
	}
	public interface T15_IMyInterface_PropertyInterfaceImplementation
		{
		int MyProperty {
			get;
			set;
		}
	}
	public class T15_MyClass_PropertyInterfaceImplementation : T15_IMyInterface_PropertyInterfaceImplementation
	{
		public int MyProperty {
			get {
				return 0;
			}
			set {
			}
		}
	}
	public class T16_MyClass_PropertyPrivateGetPublicSet
	{
		public int MyProperty {
			private get {
				return 3;
			}
			set {
			}
		}
	}
	public class T17_MyClass_PropertyPublicGetProtectedSet
	{
		public int MyProperty {
			get {
				return 3;
			}
			protected set {
			}
		}
	}
	public class T18_Base_PropertyOverrideDefaultAccessorOnly
	{
		public virtual int MyProperty {
			get {
				return 3;
			}
			protected set {
			}
		}
	}
	public class T18_Derived_PropertyOverrideDefaultAccessorOnly : T18_Base_PropertyOverrideDefaultAccessorOnly
	{
#if ROSLYN
		public override int MyProperty => 4;
#else
		public override int MyProperty {
			get {
				return 4;
			}
		}
#endif
	}
	public class T19_Base_PropertyOverrideRestrictedAccessorOnly
	{
		public virtual int MyProperty {
			get {
				return 3;
			}
			protected set {
			}
		}
	}
	public class T19_Derived_PropertyOverrideRestrictedAccessorOnly : T19_Base_PropertyOverrideRestrictedAccessorOnly
	{
		public override int MyProperty {
			protected set {
			}
		}
	}
	public class T20_Base_PropertyOverrideOneAccessor
	{
		protected internal virtual int MyProperty {
			get {
				return 3;
			}
			protected set {
			}
		}
	}
	public class T20_DerivedNew_PropertyOverrideOneAccessor : T20_Base_PropertyOverrideOneAccessor
	{
		public new virtual int MyProperty {
			set {
			}
		}
	}
	public class T20_DerivedOverride_PropertyOverrideOneAccessor : T20_DerivedNew_PropertyOverrideOneAccessor
	{
		public override int MyProperty {
			set {
			}
		}
	}
	public class T21_Base_IndexerOverrideRestrictedAccessorOnly
	{
		public virtual int this[string s] {
			get {
				return 3;
			}
			protected set {
			}
		}
		protected internal virtual int this[int i] {
			protected get {
				return 2;
			}
			set {
			}
		}
	}
	public class T21_Derived_IndexerOverrideRestrictedAccessorOnly : T21_Base_IndexerOverrideRestrictedAccessorOnly
	{
		protected internal override int this[int i] {
			protected get {
				return 4;
			}
		}
	}
	public class T22_A_HideProperty
	{
		public virtual int P {
			get {
				return 0;
			}
			set {
			}
		}
	}
	public class T22_B_HideProperty : T22_A_HideProperty
	{
		private new int P {
			get {
				return 0;
			}
			set {
			}
		}
	}
	public class T22_C_HideProperty : T22_B_HideProperty
	{
		public override int P {
			set {
			}
		}
	}
	public class T23_A_HideMembers
	{
		public int F;
#if ROSLYN
		public int Prop => 3;
		public int G => 3;
#else
		public int Prop {
			get {
				return 3;
			}
		}
		public int G {
			get {
				return 3;
			}
		}
#endif
	}
	public class T23_B_HideMembers : T23_A_HideMembers
	{
#if ROSLYN
		public new int F => 3;
		public new string Prop => "a";
#else
		public new int F {
			get {
				return 3;
			}
		}
		public new string Prop {
			get {
				return "a";
			}
		}
#endif
	}
	public class T23_C_HideMembers : T23_A_HideMembers
	{
		public new int G;
	}
	public class T23_D_HideMembers : T23_A_HideMembers
	{
		public new void F()
		{
		}
	}
	public class T23_D1_HideMembers : T23_D_HideMembers
	{
		public new int F;
	}
	public class T23_E_HideMembers : T23_A_HideMembers
	{
		private new class F
		{
		}
	}
	public class T23_G_HideMembers2
	{
#if ROSLYN
		public int Item => 1;
#else
		public int Item {
			get {
				return 1;
			}
		}
#endif
	}
	public class T23_G2_HideMembers2 : T23_G_HideMembers2
	{
#if ROSLYN
		public int this[int i] => 2;
#else
		public int this[int i] {
			get {
				return 2;
			}
		}
#endif
	}
	public class T23_G3_HideMembers2 : T23_G2_HideMembers2
	{
#if ROSLYN
		public new int Item => 4;
#else
		public new int Item {
			get {
				return 4;
			}
		}
#endif
	}
	public class T23_H_HideMembers2
	{
#if ROSLYN
		public int this[int j] => 0;
#else
		public int this[int j] {
			get {
				return 0;
			}
		}
#endif
	}
	public class T23_H2_HideMembers2 : T23_H_HideMembers2
	{
#if ROSLYN
		public int Item => 2;
#else
		public int Item {
			get {
				return 2;
			}
		}
#endif
	}
	public class T23_H3_HideMembers2 : T23_H2_HideMembers2
	{
#if ROSLYN
		public new string this[int j] => null;
#else
		public new string this[int j] {
			get {
				return null;
			}
		}
#endif
	}

	public class T24_A_HideMembers2a : T24_IA_HideMembers2a
	{
		int T24_IA_HideMembers2a.this[int i] {
			get {
				throw new NotImplementedException();
			}
		}
	}
	public class T24_A1_HideMembers2a : T24_A_HideMembers2a
	{
#if ROSLYN
		public int this[int i] => 3;
#else
		public int this[int i] {
			get {
				return 3;
			}
		}
#endif
	}
	public interface T24_IA_HideMembers2a
	{
		int this[int i] {
			get;
		}
	}

	public class T25_G_HideMembers3<T>
	{
		public void M1(T p)
		{
		}
		public int M2(int t)
		{
			return 3;
		}
	}
	public class T25_G1_HideMembers3<T> : T25_G_HideMembers3<int>
	{
		public new int M1(int i)
		{
			return 0;
		}
		public int M2(T i)
		{
			return 2;
		}
	}
	public class T25_G2_HideMembers3<T> : T25_G_HideMembers3<int>
	{
		public int M1(T p)
		{
			return 4;
		}
	}
	public class T25_J_HideMembers3
	{
#if ROSLYN
		public int P => 2;
#else
		public int P {
			get {
				return 2;
			}
		}
#endif
	}
	public class T25_J2_HideMembers3 : T25_J_HideMembers3
	{
#pragma warning disable 0108
		// Deliberate bad code for test case
		public int get_P;
#pragma warning restore 0108
	}
	public class T26_A_HideMembers4
	{
		public void M<T>(T t)
		{
		}
	}
	public class T26_A1_HideMembers4 : T26_A_HideMembers4
	{
		public new void M<K>(K t)
		{
		}
		public void M(int t)
		{
		}
	}
	public class T26_B_HideMembers4
	{
		public void M<T>()
		{
		}
		public void M1<T>()
		{
		}
		public void M2<T>(T t)
		{
		}
	}
	public class T26_B1_HideMembers4 : T26_B_HideMembers4
	{
		public void M<T1, T2>()
		{
		}
		public new void M1<R>()
		{
		}
		public new void M2<R>(R r)
		{
		}
	}
	public class T26_C_HideMembers4<T>
	{
		public void M<TT>(T t)
		{
		}
	}
	public class T26_C1_HideMembers4<K> : T26_C_HideMembers4<K>
	{
		public void M<TT>(TT t)
		{
		}
	}
	public class T27_A_HideMembers5
	{
		public void M(int t)
		{
		}
	}
	public class T27_A1_HideMembers5 : T27_A_HideMembers5
	{
		public void M(ref int t)
		{
		}
	}
	public class T27_B_HideMembers5
	{
		public void M(ref int l)
		{
		}
	}
	public class T27_B1_HideMembers5 : T27_B_HideMembers5
	{
		public void M(out int l)
		{
			l = 2;
		}
		public void M(ref long l)
		{
		}
	}
	public class T28_A_HideMemberSkipNotVisible
	{
		protected int F;
#if ROSLYN
		protected string P => null;
#else
		protected string P {
			get {
				return null;
			}
		}
#endif
	}
	public class T28_B_HideMemberSkipNotVisible : T28_A_HideMemberSkipNotVisible
	{
		private new string F;
		private new int P {
			set {
			}
		}
	}
	public class T29_A_HideNestedClass
	{
		public class N1
		{
		}
		protected class N2
		{
		}
		private class N3
		{
		}
		internal class N4
		{
		}
		protected internal class N5
		{
		}
	}
	public class T29_B_HideNestedClass : T29_A_HideNestedClass
	{
		public new int N1;
		public new int N2;
		public int N3;
		public new int N4;
		public new int N5;
	}
	public class T30_A_HidePropertyReservedMethod
	{
#if ROSLYN
		public int P => 1;
#else
		public int P {
			get {
				return 1;
			}
		}
#endif
	}
	public class T30_B_HidePropertyReservedMethod : T30_A_HidePropertyReservedMethod
	{
		public int get_P()
		{
			return 2;
		}
		public void set_P(int value)
		{
		}
	}
	public class T31_A_HideIndexerDiffAccessor
	{
#if ROSLYN
		public int this[int i] => 2;
#else
		public int this[int i] {
			get {
				return 2;
			}
		}
#endif
	}
	public class T31_B_HideIndexerDiffAccessor : T31_A_HideIndexerDiffAccessor
	{
		public new int this[int j] {
			set {
			}
		}
	}
	public class T32_A_HideIndexerGeneric<T>
	{
		public virtual int this[T r] {
			get {
				return 0;
			}
			set {
			}
		}
	}
	public class T32_B_HideIndexerGeneric : T32_A_HideIndexerGeneric<int>
	{
		private new int this[int k] {
			get {
				return 0;
			}
			set {
			}
		}
	}
	public class T32_C_HideIndexerGeneric<T> : T32_A_HideIndexerGeneric<T>
	{
		public override int this[T s] {
			set {
			}
		}
	}
	public class T32_D_HideIndexerGeneric<T> : T32_C_HideIndexerGeneric<T>
	{
		public new virtual int this[T s] {
			set {
			}
		}
	}
	public class T33_A_HideMethod
	{
		public virtual void F()
		{
		}
	}
	public class T33_B_HideMethod : T33_A_HideMethod
	{
		private new void F()
		{
			base.F();
		}
	}
	public class T33_C_HideMethod : T33_B_HideMethod
	{
		public override void F()
		{
			base.F();
		}
	}
	public class T34_A_HideMethodGeneric<T>
	{
		public virtual void F(T s)
		{
		}
		public new static bool Equals(object o1, object o2)
		{
			return true;
		}
	}
	public class T34_B_HideMethodGeneric : T34_A_HideMethodGeneric<string>
	{
		private new void F(string k)
		{
		}
		public void F(int i)
		{
		}
	}
	public class T34_C_HideMethodGeneric<T> : T34_A_HideMethodGeneric<T>
	{
		public override void F(T r)
		{
		}
		public void G(T t)
		{
		}
	}
	public class T34_D_HideMethodGeneric<T1> : T34_C_HideMethodGeneric<T1>
	{
		public new virtual void F(T1 k)
		{
		}
		public virtual void F<T2>(T2 k)
		{
		}
		public virtual void G<T2>(T2 t)
		{
		}
	}
	public class T35_A_HideMethodGenericSkipPrivate<T>
	{
		public virtual void F(T t)
		{
		}
	}
	public class T35_B_HideMethodGenericSkipPrivate<T> : T35_A_HideMethodGenericSkipPrivate<T>
	{
		private new void F(T t)
		{
		}
		private void K()
		{
		}
	}
	public class T35_C_HideMethodGenericSkipPrivate<T> : T35_B_HideMethodGenericSkipPrivate<T>
	{
		public override void F(T tt)
		{
		}
		public void K()
		{
		}
	}
	public class T35_D_HideMethodGenericSkipPrivate : T35_B_HideMethodGenericSkipPrivate<int>
	{
		public override void F(int t)
		{
		}
	}
	public class T36_A_HideMethodGeneric2
	{
		public virtual void F(int i)
		{
		}
		public void K()
		{
		}
	}
	public class T36_B_HideMethodGeneric2<T> : T36_A_HideMethodGeneric2
	{
		protected virtual void F(T t)
		{
		}
		public void K<T2>()
		{
		}
	}
	public class T36_C_HideMethodGeneric2 : T36_B_HideMethodGeneric2<int>
	{
		protected override void F(int k)
		{
		}
		public new void K<T3>()
		{
		}
	}
	public class T36_D_HideMethodGeneric2 : T36_B_HideMethodGeneric2<string>
	{
		public override void F(int k)
		{
		}
		public void L<T4>()
		{
		}
	}
	public class T36_E_HideMethodGeneric2<T>
	{
		public void M<T2>(T t, T2 t2)
		{
		}
	}
	public class T36_F_HideMethodGeneric2<T> : T36_E_HideMethodGeneric2<T>
	{
		public void M(T t1, T t2)
		{
		}
	}
	public class T37_C1_HideMethodDiffSignatures<T>
	{
		public virtual void M(T arg)
		{
		}
	}
	public class T37_C2_HideMethodDiffSignatures<T1, T2> : T37_C1_HideMethodDiffSignatures<T2>
	{
		public new virtual void M(T2 arg)
		{
		}
	}
	public class T37_C3_HideMethodDiffSignatures : T37_C2_HideMethodDiffSignatures<int, bool>
	{
		public new virtual void M(bool arg)
		{
		}
	}
	public class T38_A_HideMethodStatic
	{
#if ROSLYN
		public int N => 0;
#else
		public int N {
			get {
				return 0;
			}
		}
#endif
	}
	public class T38_B_HideMethodStatic
	{
		public int N()
		{
			return 0;
		}
	}
	public class T39_A_HideEvent
	{
		public virtual event EventHandler E;
		public event EventHandler F;
	}
	public class T39_B_HideEvent : T39_A_HideEvent
	{
		public new virtual event EventHandler E;
		public new event EventHandler F;
	}
	public class T39_C_HideEvent : T39_B_HideEvent
	{
		public override event EventHandler E;
	}
}
