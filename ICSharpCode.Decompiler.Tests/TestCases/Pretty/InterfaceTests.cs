// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
	internal class InterfaceTests
	{
		public interface IA
		{
#if CS80 && !NET40
			static int Field;
#endif
			int Property1 { get; }
			int Property2 { set; }
			int Property3 { get; set; }

			event EventHandler MyEvent;
			void Method();

#if CS80 && !NET40
			static IA()
			{

			}

			void DefaultMethod()
			{
				Method();
				PrivateMethod();
			}

			private void PrivateMethod()
			{
				Method();
			}

			internal void InternalMethod()
			{
				Method();
			}

			sealed void SealedMethod()
			{
				Method();
			}

			static void StaticMethod()
			{

			}
#endif
		}
		public interface IA2 : IA
		{
#if CS80 && !NET40
			int IA.Property3 {
				get {
					return 0;
				}
				set {
				}
			}

			event EventHandler IA.MyEvent {
				add {
				}
				remove {
				}
			}

			new event EventHandler MyEvent {
				add {
				}
				remove {
				}
			}

			void IA.InternalMethod()
			{
			}

			new void Method()
			{
			}
#endif
		}
		public interface IB
		{
		}
#if CS80 && !NET40
		public interface IProtectedMembers
		{
			protected void ProtectedMethod();

			protected internal void ProtectedInternalMethod()
			{
				ProtectedMethod();
			}
		}
		public interface IStaticMembers
		{
			static int StaticProperty { get; set; }

			static event EventHandler StaticEvent;
		}
		public interface IGenericWithDefaultImpl<T>
		{
			T Value { get; }

			T DefaultGet<U>(U key) where U : T
			{
				return Value;
			}
		}
#endif
		public class C : IA2, IA, IB
		{
			int IA.Property1 {
				get {
					throw new NotImplementedException();
				}
			}
			int IA.Property2 {
				set {
					throw new NotImplementedException();
				}
			}
			int IA.Property3 {
				get {
					throw new NotImplementedException();
				}
				set {
					throw new NotImplementedException();
				}
			}

			event EventHandler IA.MyEvent {
				add {
				}
				remove {
				}
			}
			public int Finalize()
			{
				return 0;
			}
			void IA.Method()
			{
				throw new NotImplementedException();
			}
		}

		internal interface IInterfacesCannotDeclareDtors
		{
			int Finalize();
		}

#if ROSLYN
		private class Issue3230_F : Issue3230_F.IFoo
		{
			protected interface IFoo
			{
				void Foo();
			}

			void IFoo.Foo()
			{
				Console.WriteLine("F");
			}

			public void Bar()
			{
				((IFoo)this).Foo();
			}
		}

		private class Issue3230_SubF : Issue3230_F, Issue3230_SubF.ISubFoo
		{
			protected interface ISubFoo : IFoo
			{
			}

			void IFoo.Foo()
			{
				Console.WriteLine("SubF");
			}
		}

		private class Issue3230_Priv : Issue3230_Priv.IPriv
		{
			private interface IPriv
			{
			}
		}

		private class Issue3230_ProtInt : Issue3230_ProtInt.IFoo
		{
			protected internal interface IFoo
			{
			}
		}

		private class Issue3230_SubProtInt : Issue3230_ProtInt, Issue3230_ProtInt.IFoo
		{
		}

#if CS72
		private class Issue3230_PrivProt : Issue3230_PrivProt.IFoo
		{
			private protected interface IFoo
			{
			}
		}

		private class Issue3230_SubPrivProt : Issue3230_PrivProt, Issue3230_SubPrivProt.ISub
		{
			private protected interface ISub : IFoo
			{
			}
		}
#endif

		private class Issue3230_Outer
		{
			protected interface IP
			{
			}

			private class Inner : IP
			{
			}
		}

		private class Issue3230_Outer2 : Issue3230_Outer
		{
			private class Inner2 : IP
			{
			}
		}

		private class Issue3230_G<T> : Issue3230_G<T>.INested
		{
			protected interface INested
			{
			}
		}

		private class Issue3230_SubG : Issue3230_G<int>, Issue3230_SubG.ISub
		{
			protected interface ISub : INested
			{
			}
		}

		private interface IWrap3230<T>
		{
		}

		private class Issue3230_TypeArg : Issue3230_TypeArg.IFoo, IWrap3230<Issue3230_TypeArg.IFoo>
		{
			protected interface IFoo
			{
			}
		}

		private class Issue3230_SubTypeArg : Issue3230_TypeArg, Issue3230_SubTypeArg.IS
		{
			protected interface IS : IWrap3230<IFoo>
			{
			}
		}

		private class Issue3230_DeclChain
		{
			protected class Nest
			{
				public interface I
				{
				}
			}

			internal class NestInt
			{
				public interface I
				{
				}
			}
		}

		private class Issue3230_SubDeclChain : Issue3230_DeclChain, Issue3230_SubDeclChain.IMine
		{
			protected interface IMine : Nest.I
			{
			}
		}

		private class Issue3230_SubDeclChainKeep : Issue3230_DeclChain, Issue3230_SubDeclChainKeep.IMine, Issue3230_DeclChain.NestInt.I
		{
			protected interface IMine : NestInt.I
			{
			}
		}
#endif
	}
}
