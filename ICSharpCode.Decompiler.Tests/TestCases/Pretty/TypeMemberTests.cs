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
	public class IndexerWithGetOnly
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
	public class IndexerWithSetOnly
	{
		public int this[int i] {
			set {
			}
		}
	}

	public class IndexerWithMoreParameters
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

	public class IndexerInGenericClass<T>
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
	public class OverloadedIndexer
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
	public interface IIndexerInInterface
	{
		int this[string s, string s2] {
			set;
		}
	}
	public interface IMyInterface_IndexerInterfaceExplicitImplementation
	{
		int this[string s] {
			get;
		}
	}
	public class MyClass_IndexerInterfaceExplicitImplementation : IMyInterface_IndexerInterfaceExplicitImplementation
	{
#if ROSLYN
		int IMyInterface_IndexerInterfaceExplicitImplementation.this[string s] => 3;
#else
		int IMyInterface_IndexerInterfaceExplicitImplementation.this[string s] {
			get {
				return 3;
			}
		}
#endif
	}
	public interface IMyInterface_IndexerInterfaceImplementation
	{
		int this[string s] {
			get;
		}
	}
	public class MyClass_IndexerInterfaceImplementation : IMyInterface_IndexerInterfaceImplementation
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
	public abstract class MyClass_IndexerAbstract
	{
		public abstract int this[string s, string s2] {
			set;
		}
		protected abstract string this[int index] {
			get;
		}
	}
	public interface IMyInterface_MethodExplicit
	{
		void MyMethod();
	}
	public class MyClass_MethodExplicit : IMyInterface_MethodExplicit
	{
		void IMyInterface_MethodExplicit.MyMethod()
		{
		}
	}
	public interface IMyInterface_MethodFromInterfaceVirtual
	{
		void MyMethod();
	}
	public class MyClass : IMyInterface_MethodFromInterfaceVirtual
	{
		public virtual void MyMethod()
		{
		}
	}
	public interface IMyInterface_MethodFromInterface
	{
		void MyMethod();
	}
	public class MyClass_MethodFromInterface : IMyInterface_MethodFromInterface
	{
		public void MyMethod()
		{
		}
	}
	public interface IMyInterface_MethodFromInterfaceAbstract
	{
		void MyMethod();
	}
	public abstract class MyClass_MethodFromInterfaceAbstract : IMyInterface_MethodFromInterfaceAbstract
	{
		public abstract void MyMethod();
	}
	public interface IMyInterface_PropertyInterface
	{
		int MyProperty {
			get;
			set;
		}
	}
	public interface IMyInterface_PropertyInterfaceExplicitImplementation
	{
		int MyProperty {
			get;
			set;
		}
	}
	public class MyClass_PropertyInterfaceExplicitImplementation : IMyInterface_PropertyInterfaceExplicitImplementation
	{
		int IMyInterface_PropertyInterfaceExplicitImplementation.MyProperty {
			get {
				return 0;
			}
			set {
			}
		}
	}
	public interface IMyInterface_PropertyInterfaceImplementation
		{
		int MyProperty {
			get;
			set;
		}
	}
	public class MyClass_PropertyInterfaceImplementation : IMyInterface_PropertyInterfaceImplementation
		{
		public int MyProperty {
			get {
				return 0;
			}
			set {
			}
		}
	}
	public class MyClass_PropertyPrivateGetPublicSet
	{
		public int MyProperty {
			private get {
				return 3;
			}
			set {
			}
		}
	}
	public class MyClass_PropertyPublicGetProtectedSet
	{
		public int MyProperty {
			get {
				return 3;
			}
			protected set {
			}
		}
	}
	public class MyClass_PropertyOverrideDefaultAccessorOnly
	{
		public virtual int MyProperty {
			get {
				return 3;
			}
			protected set {
			}
		}
	}
	public class Derived_PropertyOverrideDefaultAccessorOnly : MyClass_PropertyOverrideDefaultAccessorOnly
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
	public class MyClass_PropertyOverrideRestrictedAccessorOnly
	{
		public virtual int MyProperty {
			get {
				return 3;
			}
			protected set {
			}
		}
	}
	public class Derived_PropertyOverrideRestrictedAccessorOnly : MyClass_PropertyOverrideRestrictedAccessorOnly
	{
		public override int MyProperty {
			protected set {
			}
		}
	}
	public class MyClass_PropertyOverrideOneAccessor
	{
		protected internal virtual int MyProperty {
			get {
				return 3;
			}
			protected set {
			}
		}
	}
	public class DerivedNew_PropertyOverrideOneAccessor : MyClass_PropertyOverrideOneAccessor
	{
		public new virtual int MyProperty {
			set {
			}
		}
	}
	public class DerivedOverride_PropertyOverrideOneAccessor : DerivedNew_PropertyOverrideOneAccessor
	{
		public override int MyProperty {
			set {
			}
		}
	}
	public class MyClass_IndexerOverrideRestrictedAccessorOnly
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
	public class Derived_IndexerOverrideRestrictedAccessorOnly : MyClass_IndexerOverrideRestrictedAccessorOnly
	{
		protected internal override int this[int i] {
			protected get {
				return 4;
			}
		}
	}
	public class A_HideProperty
	{
		public virtual int P {
			get {
				return 0;
			}
			set {
			}
		}
	}
	public class B_HideProperty : A_HideProperty
	{
		private new int P {
			get {
				return 0;
			}
			set {
			}
		}
	}
	public class C_HideProperty : B_HideProperty
	{
		public override int P {
			set {
			}
		}
	}
	public class A_HideMembers
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
	public class B_HideMembers : A_HideMembers
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
	public class C_HideMembers : A_HideMembers
	{
		public new int G;
	}
	public class D_HideMembers : A_HideMembers
	{
		public new void F()
		{
		}
	}
	public class D1_HideMembers : D_HideMembers
	{
		public new int F;
	}
	public class E_HideMembers : A_HideMembers
	{
		private new class F
		{
		}
	}
	public class G_HideMembers2
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
	public class G2_HideMembers2 : G_HideMembers2
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
	public class G3_HideMembers2 : G2_HideMembers2
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
	public class H_HideMembers2
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
	public class H2_HideMembers2 : H_HideMembers2
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
	public class H3_HideMembers2 : H2_HideMembers2
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
	public interface IA_HideMembers2a
	{
		int this[int i] {
			get;
		}
	}
	public class A_HideMembers2a : IA_HideMembers2a
	{
		int IA_HideMembers2a.this[int i] {
			get {
				throw new NotImplementedException();
			}
		}
	}
	public class A1_HideMembers2a : A_HideMembers2a
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
	public class G_HideMembers3<T>
	{
		public void M1(T p)
		{
		}
		public int M2(int t)
		{
			return 3;
		}
	}
	public class G1_HideMembers3<T> : G_HideMembers3<int>
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
	public class G2_HideMembers3<T> : G_HideMembers3<int>
	{
		public int M1(T p)
		{
			return 4;
		}
	}
	public class J_HideMembers3
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
	public class J2_HideMembers3 : J_HideMembers3
	{
#pragma warning disable 0108
		// Deliberate bad code for test case
		public int get_P;
#pragma warning restore 0108
	}
	public class A_HideMembers4
	{
		public void M<T>(T t)
		{
		}
	}
	public class A1_HideMembers4 : A_HideMembers4
	{
		public new void M<K>(K t)
		{
		}
		public void M(int t)
		{
		}
	}
	public class B_HideMembers4
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
	public class B1_HideMembers4 : B_HideMembers4
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
	public class C_HideMembers4<T>
	{
		public void M<TT>(T t)
		{
		}
	}
	public class C1_HideMembers4<K> : C_HideMembers4<K>
	{
		public void M<TT>(TT t)
		{
		}
	}
	public class A_HideMembers5
	{
		public void M(int t)
		{
		}
	}
	public class A1_HideMembers5 : A_HideMembers5
	{
		public void M(ref int t)
		{
		}
	}
	public class B_HideMembers5
	{
		public void M(ref int l)
		{
		}
	}
	public class B1_HideMembers5 : B_HideMembers5
	{
		public void M(out int l)
		{
			l = 2;
		}
		public void M(ref long l)
		{
		}
	}
	public class A_HideMemberSkipNotVisible
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
	public class B_HideMemberSkipNotVisible : A_HideMemberSkipNotVisible
	{
		private new string F;
		private new int P {
			set {
			}
		}
	}
	public class A_HideNestedClass
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
	public class B_HideNestedClass : A_HideNestedClass
	{
		public new int N1;
		public new int N2;
		public int N3;
		public new int N4;
		public new int N5;
	}
	public class A_HidePropertyReservedMethod
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
	public class B_HidePropertyReservedMethod : A_HidePropertyReservedMethod
	{
		public int get_P()
		{
			return 2;
		}
		public void set_P(int value)
		{
		}
	}
	public class A_HideIndexerDiffAccessor
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
	public class B_HideIndexerDiffAccessor : A_HideIndexerDiffAccessor
	{
		public new int this[int j] {
			set {
			}
		}
	}
	public class A_HideIndexerGeneric<T>
	{
		public virtual int this[T r] {
			get {
				return 0;
			}
			set {
			}
		}
	}
	public class B_HideIndexerGeneric : A_HideIndexerGeneric<int>
	{
		private new int this[int k] {
			get {
				return 0;
			}
			set {
			}
		}
	}
	public class C_HideIndexerGeneric<T> : A_HideIndexerGeneric<T>
	{
		public override int this[T s] {
			set {
			}
		}
	}
	public class D_HideIndexerGeneric<T> : C_HideIndexerGeneric<T>
	{
		public new virtual int this[T s] {
			set {
			}
		}
	}
	public class A_HideMethod
	{
		public virtual void F()
		{
		}
	}
	public class B_HideMethod : A_HideMethod
	{
		private new void F()
		{
			base.F();
		}
	}
	public class C_HideMethod : B_HideMethod
	{
		public override void F()
		{
			base.F();
		}
	}
	public class A_HideMethodGeneric<T>
	{
		public virtual void F(T s)
		{
		}
		public new static bool Equals(object o1, object o2)
		{
			return true;
		}
	}
	public class B_HideMethodGeneric : A_HideMethodGeneric<string>
	{
		private new void F(string k)
		{
		}
		public void F(int i)
		{
		}
	}
	public class C_HideMethodGeneric<T> : A_HideMethodGeneric<T>
	{
		public override void F(T r)
		{
		}
		public void G(T t)
		{
		}
	}
	public class D_HideMethodGeneric<T1> : C_HideMethodGeneric<T1>
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
	public class A_HideMethodGenericSkipPrivate<T>
	{
		public virtual void F(T t)
		{
		}
	}
	public class B_HideMethodGenericSkipPrivate<T> : A_HideMethodGenericSkipPrivate<T>
	{
		private new void F(T t)
		{
		}
		private void K()
		{
		}
	}
	public class C_HideMethodGenericSkipPrivate<T> : B_HideMethodGenericSkipPrivate<T>
	{
		public override void F(T tt)
		{
		}
		public void K()
		{
		}
	}
	public class D_HideMethodGenericSkipPrivate : B_HideMethodGenericSkipPrivate<int>
	{
		public override void F(int t)
		{
		}
	}
	public class A_HideMethodGeneric2
	{
		public virtual void F(int i)
		{
		}
		public void K()
		{
		}
	}
	public class B_HideMethodGeneric2<T> : A_HideMethodGeneric2
	{
		protected virtual void F(T t)
		{
		}
		public void K<T2>()
		{
		}
	}
	public class C_HideMethodGeneric2 : B_HideMethodGeneric2<int>
	{
		protected override void F(int k)
		{
		}
		public new void K<T3>()
		{
		}
	}
	public class D_HideMethodGeneric2 : B_HideMethodGeneric2<string>
	{
		public override void F(int k)
		{
		}
		public void L<T4>()
		{
		}
	}
	public class E_HideMethodGeneric2<T>
	{
		public void M<T2>(T t, T2 t2)
		{
		}
	}
	public class F_HideMethodGeneric2<T> : E_HideMethodGeneric2<T>
	{
		public void M(T t1, T t2)
		{
		}
	}
	public class C1_HideMethodDiffSignatures<T>
	{
		public virtual void M(T arg)
		{
		}
	}
	public class C2_HideMethodDiffSignatures<T1, T2> : C1_HideMethodDiffSignatures<T2>
	{
		public new virtual void M(T2 arg)
		{
		}
	}
	public class C3_HideMethodDiffSignatures : C2_HideMethodDiffSignatures<int, bool>
	{
		public new virtual void M(bool arg)
		{
		}
	}
	public class A_HideMethodStatic
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
	public class B_HideMethodStatic
	{
		public int N()
		{
			return 0;
		}
	}
	public class A_HideEvent
	{
		public virtual event EventHandler E;
		public event EventHandler F;
	}
	public class B_HideEvent : A_HideEvent
	{
		public new virtual event EventHandler E;
		public new event EventHandler F;
	}
	public class C_HideEvent : B_HideEvent
	{
		public override event EventHandler E;
	}
}
