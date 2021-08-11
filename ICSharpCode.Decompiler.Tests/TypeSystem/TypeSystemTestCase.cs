// Copyright (c) 2010-2018 AlphaSierraPapa for the SharpDevelop Team
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: ICSharpCode.Decompiler.Tests.TypeSystem.TypeTestAttribute(
	42, typeof(System.Action<>), typeof(IDictionary<string, IList<NUnit.Framework.TestAttribute>>))]

[assembly: TypeForwardedTo(typeof(Func<,>))]

namespace ICSharpCode.Decompiler.Tests.TypeSystem
{
	public delegate S GenericDelegate<in T, out S>(T input) where T : S where S : class;

	public class SimplePublicClass
	{
		public void Method() { }

		public SimplePublicClass() { }
		[Double(1)]
		~SimplePublicClass() { }
	}

	public class TypeTestAttribute : Attribute
	{
		public TypeTestAttribute(int a1, Type a2, Type a3) { }

#pragma warning disable CS0465
		private void Finalize()
		{

		}
#pragma warning restore CS0465
	}

	[Params(1, StringComparison.CurrentCulture, null, 4.0, "Test")]
	public class ParamsAttribute : Attribute
	{
		public ParamsAttribute(params object[] x) { }

		[Params(Property = new string[] { "a", "b" })]
		public string[] Property {
			[return: Params("Attribute on return type of getter")]
			get { return null; }
			set { }
		}
	}

	[Double(1)]
	public class DoubleAttribute : Attribute
	{
		public DoubleAttribute(double val) { }
	}

	public unsafe class DynamicTest
	{
		public dynamic DynamicField;
		public dynamic SimpleProperty { get; set; }

		public List<dynamic> DynamicGenerics1(Action<object, dynamic[], object> param) { return null; }
		public void DynamicGenerics2(Action<object, dynamic, object> param) { }
		public void DynamicGenerics3(Action<int, dynamic, object> param) { }
		public void DynamicGenerics4(Action<int[], dynamic, object> param) { }
		public void DynamicGenerics5(Action<int*[], dynamic, object> param) { }
		public void DynamicGenerics6(ref Action<object, dynamic, object> param) { }
		public void DynamicGenerics7(Action<int[,][], dynamic, object> param) { }
	}

	public class GenericClass<A, B> where A : B
	{
		public void TestMethod<K, V>(string param) where V : K where K : IComparable<V> { }
		public void GetIndex<T>(T element) where T : IEquatable<T> { }

		public NestedEnum EnumField;

		public A Property { get; set; }

		public enum NestedEnum
		{
			EnumMember
		}
	}

	public class PropertyTest
	{
		public int PropertyWithProtectedSetter { get; protected set; }

		public object PropertyWithPrivateSetter { get; private set; }

		public object PropertyWithoutSetter { get { return null; } }

		public object PropertyWithPrivateGetter { private get; set; }

		public string this[int index] { get { return "Test"; } set { } }
	}

	public enum MyEnum : short
	{
		First,
		Second,
		Flag1 = 0x10,
		Flag2 = 0x20,
		CombinedFlags = Flag1 | Flag2
	}

	public class Base<T>
	{
		public class Nested<X> { }

		~Base() { }

		public virtual void GenericMethodWithConstraints<X>(T a) where X : IComparer<T>, new() { }
	}
	public class Derived<A, B> : Base<B>
	{
		~Derived() { }
		public override void GenericMethodWithConstraints<Y>(B a) { }
	}

	public struct MyStructWithCtor
	{
		public MyStructWithCtor(int a) { }
	}

	public class MyClassWithCtor
	{
		private MyClassWithCtor(int a) { }
	}

	[Serializable]
	public class NonCustomAttributes
	{
		[SpecialName]
		public class SpecialNameClass
		{
		}

		[SpecialName]
		public struct SpecialNameStruct
		{
		}

		[NonSerialized]
		public readonly int NonSerializedField;

		[SpecialName]
		public readonly int SpecialNameField;

		[SpecialName]
		public event EventHandler SpecialNameEvent;

		[SpecialName]
		public int SpecialNameProperty { get; set; }

		[DllImport("unmanaged.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DllMethod([In, Out] ref int p);

		[DllImport("unmanaged.dll", PreserveSig = false)]
		public static extern bool DoNotPreserveSig();

		[PreserveSig]
		public static void PreserveSigAsAttribute()
		{
		}

		[SpecialName]
		public static void SpecialNameMethod()
		{
		}
	}

	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode, Pack = 8)]
	public struct ExplicitFieldLayoutStruct
	{
		[FieldOffset(0)]
		public int Field0;

		[FieldOffset(100)]
		public int Field100;
	}

	public class ParameterTests
	{
		public void MethodWithOutParameter(out int x) { x = 0; }
		public void MethodWithParamsArray(params object[] x) { }
		public void MethodWithOptionalParameter(int x = 4) { }
		public void MethodWithExplicitOptionalParameter([Optional] int x) { }
		public void MethodWithRefParameter(ref int x) { }
		public void MethodWithInParameter(in int x) { }
		public void MethodWithEnumOptionalParameter(StringComparison x = StringComparison.OrdinalIgnoreCase) { }
		public void MethodWithOptionalNullableParameter(int? x = null) { }
		public void MethodWithOptionalLongParameter(long x = 1) { }
		public void MethodWithOptionalNullableLongParameter(long? x = 1) { }
		public void MethodWithOptionalDecimalParameter(decimal x = 1) { }
		public void VarArgsMethod(__arglist) { }
	}

	public class VarArgsCtor
	{
		public VarArgsCtor(__arglist) { }
	}

	[ComImport(), Guid("21B8916C-F28E-11D2-A473-00C04F8EF448"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAssemblyEnum
	{
		[PreserveSig()]
		int GetNextAssembly(uint dwFlags);
	}

	public class OuterGeneric<X>
	{
		public class Inner
		{
			public OuterGeneric<X> referenceToOuter;
			public Inner(OuterGeneric<X> referenceToOuter) { }
		}

		public OuterGeneric<X>.Inner Field1;
		public Inner Field2;
		public OuterGeneric<OuterGeneric<X>.Inner>.Inner Field3;
	}

	public class ExplicitDisposableImplementation : IDisposable
	{
		void IDisposable.Dispose() { }
	}

	public interface IGenericInterface<T>
	{
		void Test<S>(T a, S b) where S : T;
		void Test<S>(T a, ref S b);
	}

	public class ExplicitGenericInterfaceImplementation : IGenericInterface<string>
	{
		void IGenericInterface<string>.Test<T>(string a, T b) { }
		void IGenericInterface<string>.Test<T>(string a, ref T b) { }
	}

	public interface IGenericInterfaceWithUnifiableMethods<T, S>
	{
		void Test(T a);
		void Test(S a);
	}

	public class ImplementationOfUnifiedMethods : IGenericInterfaceWithUnifiableMethods<int, int>
	{
		public void Test(int a) { }
	}

	public class ExplicitGenericInterfaceImplementationWithUnifiableMethods<T, S> : IGenericInterfaceWithUnifiableMethods<T, S>
	{
		void IGenericInterfaceWithUnifiableMethods<T, S>.Test(T a) { }
		void IGenericInterfaceWithUnifiableMethods<T, S>.Test(S a) { }
	}

	public partial class PartialClass
	{
		partial void PartialMethodWithImplementation(int a);

		partial void PartialMethodWithImplementation(System.Int32 a)
		{
		}

		partial void PartialMethodWithImplementation(string a);

		partial void PartialMethodWithImplementation(System.String a)
		{
		}

		partial void PartialMethodWithoutImplementation();
	}

	public class ClassWithStaticAndNonStaticMembers
	{
		public static event System.EventHandler Event1 { add { } remove { } }
		public event System.EventHandler Event2 { add { } remove { } }
#pragma warning disable 67
		public static event System.EventHandler Event3;
		public event System.EventHandler Event4;

		public static int Prop1 { get { return 0; } set { } }
		public int Prop2 { get { return 0; } set { } }
		public static int Prop3 { get; set; }
		public int Prop4 { get; set; }
	}

	public interface IInterfaceWithProperty
	{
		int Prop { get; set; }
	}

	public interface IBase1
	{
		int Prop { get; set; }
	}

	public interface IBase2
	{
		int Prop { get; set; }
	}

	public interface IDerived : IBase1, IBase2
	{
		new int Prop { get; set; }
	}

	public class ClassWithVirtualProperty
	{
		public virtual int Prop { get; protected set; }
	}

	public class ClassThatOverridesAndSealsVirtualProperty : ClassWithVirtualProperty
	{
		public sealed override int Prop { get; protected set; }
	}

	public class ClassThatOverridesGetterOnly : ClassWithVirtualProperty
	{
		public override int Prop { get { return 1; } }
	}

	public class ClassThatOverridesSetterOnly : ClassThatOverridesGetterOnly
	{
		public override int Prop { protected set { } }
	}

	public class ClassThatImplementsProperty : IInterfaceWithProperty
	{
		public int Prop { get; set; }
	}

	public class ClassThatImplementsPropertyExplicitly : IInterfaceWithProperty
	{
		int IInterfaceWithProperty.Prop { get; set; }
	}

	public interface IInterfaceWithIndexers
	{
		int this[int x] { get; set; }
		int this[string x] { get; set; }
		int this[int x, int y] { get; set; }
	}

	public interface IGenericInterfaceWithIndexer<T>
	{
		int this[T x] { get; set; }
	}

	public interface IInterfaceWithRenamedIndexer
	{
		[IndexerName("NewName")]
		int this[int x] { get; set; }
	}

	public class ClassThatImplementsIndexers : IInterfaceWithIndexers, IGenericInterfaceWithIndexer<int>
	{
		public int this[int x] { get { return 0; } set { } }
		public int this[string x] { get { return 0; } set { } }
		public int this[int x, int y] { get { return 0; } set { } }
	}

	public class ClassThatImplementsIndexersExplicitly : IInterfaceWithIndexers, IGenericInterfaceWithIndexer<int>, IInterfaceWithRenamedIndexer
	{
		int IInterfaceWithIndexers.this[int x] { get { return 0; } set { } }
		int IGenericInterfaceWithIndexer<int>.this[int x] { get { return 0; } set { } }
		int IInterfaceWithIndexers.this[string x] { get { return 0; } set { } }
		int IInterfaceWithIndexers.this[int x, int y] { get { return 0; } set { } }
		int IInterfaceWithRenamedIndexer.this[int x] { get { return 0; } set { } }
	}

	public interface IHasEvent
	{
		event EventHandler Event;
	}

	public class ClassThatImplementsEvent : IHasEvent
	{
		public event EventHandler Event;
	}

	public class ClassThatImplementsEventWithCustomAccessors : IHasEvent
	{
		public event EventHandler Event { add { } remove { } }
	}

	public class ClassThatImplementsEventExplicitly : IHasEvent
	{
		event EventHandler IHasEvent.Event { add { } remove { } }
	}

	public interface IShadowTestBase
	{
		void Method();
		int this[int i] { get; set; }
		int Prop { get; set; }
		event EventHandler Evt;
	}

	public interface IShadowTestDerived : IShadowTestBase
	{
		new void Method();
		new int this[int i] { get; set; }
		new int Prop { get; set; }
		new event EventHandler Evt;
	}

	public static class StaticClass
	{
		public static void Extension(this object inst) { }
	}

	public abstract class AbstractClass { }

	public class IndexerNonDefaultName
	{
		[IndexerName("Foo")]
		public int this[int index] {
			get { return 0; }
		}
	}

	public class ClassWithMethodThatHasNullableDefaultParameter
	{
		public void Foo(int? bar = 42) { }
	}

	public class AccessibilityTest
	{
		public void Public() { }
		internal void Internal() { }
		protected internal void ProtectedInternal() { }
		internal protected void InternalProtected() { }
		protected void Protected() { }
		private void Private() { }
		void None() { }
	}

	public class ConstantFieldTest
	{
		public const byte Cb = 42;
		public const sbyte Csb = 42;
		public const char Cc = '\x42';
		public const short Cs = 42;
		public const ushort Cus = 42;
		public const int Ci = 42;
		public const uint Cui = 42;
		public const long Cl = 42;
		public const ulong Cul = 42;
		public const double Cd = 42;
		public const float Cf = 42;
		public const decimal Cm = 42;
		public const string S = "hello, world";
		public const string NullString = null;

		public const MyEnum EnumFromThisAssembly = MyEnum.Second;
		public const StringComparison EnumFromAnotherAssembly = StringComparison.OrdinalIgnoreCase;
		public const MyEnum DefaultOfEnum = default(MyEnum);

		public const int SOsb = sizeof(sbyte);
		public const int SOb = sizeof(byte);
		public const int SOs = sizeof(short);
		public const int SOus = sizeof(ushort);
		public const int SOi = sizeof(int);
		public const int SOui = sizeof(uint);
		public const int SOl = sizeof(long);
		public const int SOul = sizeof(ulong);
		public const int SOc = sizeof(char);
		public const int SOf = sizeof(float);
		public const int SOd = sizeof(double);
		public const int SObl = sizeof(bool);
		public const int SOe = sizeof(MyEnum);


		public const byte CNewb = new byte();
		public const sbyte CNewsb = new sbyte();
		public const char CNewc = new char();
		public const short CNews = new short();
		public const ushort CNewus = new ushort();
		public const int CNewi = new int();
		public const uint CNewui = new uint();
		public const long CNewl = new long();
		public const ulong CNewul = new ulong();
		public const double CNewd = new double();
		public const float CNewf = new float();
		public const decimal CNewm = new decimal();
	}

	public interface IExplicitImplementationTests
	{
		void M(int a);
		int P { get; set; }
		event Action E;
		int this[int x] { get; set; }
	}

	public class ExplicitImplementationTests : IExplicitImplementationTests
	{
		public void M(int a) { }
		public int P { get; set; }
		public event Action E;
		public int this[int x] { get { return 0; } set { } }

		void IExplicitImplementationTests.M(int a) { }
		int IExplicitImplementationTests.P { get; set; }
		event Action IExplicitImplementationTests.E { add { } remove { } }
		int IExplicitImplementationTests.this[int x] { get { return 0; } set { } }
	}

	[TypeTest(C, typeof(Inner), typeof(int)), My]
	public class ClassWithAttributesUsingNestedMembers
	{
		sealed class MyAttribute : Attribute { }

		const int C = 42;
		class Inner
		{
		}

		[TypeTest(C, typeof(Inner), typeof(int)), My]
		public int P { get; set; }

		[TypeTest(C, typeof(Inner), typeof(int)), My]
		class AttributedInner
		{
		}

		[TypeTest(C, typeof(Inner), typeof(int)), My]
		class AttributedInner2
		{
			sealed class MyAttribute : Attribute { }

			const int C = 43;
			class Inner { }
		}
	}

	public class ClassWithAttributeOnTypeParameter<[Double(2)] T> { }

	[Guid("790C6E0B-9194-4cc9-9426-A48A63185696"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[ComImport]
	public interface IMarshalAsTests
	{
		[DispId(48)]
		void AliasComponent([MarshalAs(UnmanagedType.BStr)][In] string bstrSrcApplicationIDOrName, [MarshalAs(UnmanagedType.BStr)][In] string bstrCLSIDOrProgID, [MarshalAs(UnmanagedType.BStr)][In] string bstrDestApplicationIDOrName, [MarshalAs(UnmanagedType.BStr)][In] string bstrNewProgId, [MarshalAs(UnmanagedType.BStr)][In] string bstrNewClsid);

		[DispId(33)]
		[return: MarshalAs(UnmanagedType.VariantBool)]
		bool AreApplicationInstancesPaused([MarshalAs(UnmanagedType.LPStruct)][In] object pVarApplicationInstanceID);

		[DispId(19)]
		void BackupREGDB([MarshalAs(UnmanagedType.BStr)][In] string bstrBackupFilePath);

		[DispId(2)]
		[return: MarshalAs(UnmanagedType.Interface)]
		object Connect([MarshalAs(UnmanagedType.BStr)][In] string connectStr);

		[DispId(45)]
		void CopyApplications([MarshalAs(UnmanagedType.BStr)][In] string bstrSourcePartitionIDOrName, [MarshalAs(UnmanagedType.LPStruct)][In] object pVarApplicationID, [MarshalAs(UnmanagedType.BStr)][In] string bstrDestinationPartitionIDOrName);

		[DispId(46)]
		void CopyComponents([MarshalAs(UnmanagedType.BStr)][In] string bstrSourceApplicationIDOrName, [MarshalAs(UnmanagedType.LPStruct)][In] object pVarCLSIDOrProgID, [MarshalAs(UnmanagedType.BStr)][In] string bstrDestinationApplicationIDOrName);

		[DispId(36)]
		void CreateServiceForApplication([MarshalAs(UnmanagedType.BStr)][In] string bstrApplicationIDOrName, [MarshalAs(UnmanagedType.BStr)][In] string bstrServiceName, [MarshalAs(UnmanagedType.BStr)][In] string bstrStartType, [MarshalAs(UnmanagedType.BStr)][In] string bstrErrorControl, [MarshalAs(UnmanagedType.BStr)][In] string bstrDependencies, [MarshalAs(UnmanagedType.BStr)][In] string bstrRunAs, [MarshalAs(UnmanagedType.BStr)][In] string bstrPassword, [MarshalAs(UnmanagedType.VariantBool)][In] bool bDesktopOk);

		[DispId(40)]
		void CurrentPartition([MarshalAs(UnmanagedType.BStr)][In] string bstrPartitionIDOrName);

		[DispId(41)]
		[return: MarshalAs(UnmanagedType.BStr)]
		string CurrentPartitionID();

		[DispId(42)]
		[return: MarshalAs(UnmanagedType.BStr)]
		string CurrentPartitionName();

		[DispId(37)]
		void DeleteServiceForApplication([MarshalAs(UnmanagedType.BStr)][In] string bstrApplicationIDOrName);

		[DispId(34)]
		[return: MarshalAs(UnmanagedType.BStr)]
		string DumpApplicationInstance([MarshalAs(UnmanagedType.BStr)][In] string bstrApplicationInstanceID, [MarshalAs(UnmanagedType.BStr)][In] string bstrDirectory, [MarshalAs(UnmanagedType.I4)][In] int lMaxImages);

		[DispId(9)]
		void ExportApplication([MarshalAs(UnmanagedType.BStr)][In] string bstrApplIdOrName, [MarshalAs(UnmanagedType.BStr)][In] string bstrApplicationFile, [In] int lOptions);

		[DispId(54)]
		void ExportPartition([MarshalAs(UnmanagedType.BStr)][In] string bstrPartitionIDOrName, [MarshalAs(UnmanagedType.BStr)][In] string bstrPartitionFileName, [MarshalAs(UnmanagedType.I4)][In] int lOptions);

		[DispId(44)]
		void FlushPartitionCache();

		[DispId(28)]
		[return: MarshalAs(UnmanagedType.BStr)]
		string GetApplicationInstanceIDFromProcessID([MarshalAs(UnmanagedType.I4)][In] int lProcessID);

		[DispId(1)]
		[return: MarshalAs(UnmanagedType.Interface)]
		object GetCollection([MarshalAs(UnmanagedType.BStr)][In] string bstrCollName);

		[DispId(5)]
		[return: MarshalAs(UnmanagedType.Interface)]
		object GetCollectionByQuery([MarshalAs(UnmanagedType.BStr)][In] string collName, [MarshalAs(UnmanagedType.SafeArray)][In] ref object[] aQuery);

		[DispId(27)]
		[return: MarshalAs(UnmanagedType.Interface)]
		object GetCollectionByQuery2([MarshalAs(UnmanagedType.BStr)][In] string bstrCollectionName, [MarshalAs(UnmanagedType.LPStruct)][In] object pVarQueryStrings);

		[DispId(57)]
		[return: MarshalAs(UnmanagedType.I4)]
		int GetComponentVersionCount([MarshalAs(UnmanagedType.BStr)][In] string bstrCLSIDOrProgID);

		[DispId(26)]
		void GetEventClassesForIID([In] string bstrIID, [MarshalAs(UnmanagedType.SafeArray)][In][Out] ref object[] varCLSIDS, [MarshalAs(UnmanagedType.SafeArray)][In][Out] ref object[] varProgIDs, [MarshalAs(UnmanagedType.SafeArray)][In][Out] ref object[] varDescriptions);

		[DispId(17)]
		void GetMultipleComponentsInfo([MarshalAs(UnmanagedType.BStr)][In] string bstrApplIdOrName, [In] object varFileNames, [MarshalAs(UnmanagedType.SafeArray)] out object[] varCLSIDS, [MarshalAs(UnmanagedType.SafeArray)] out object[] varClassNames, [MarshalAs(UnmanagedType.SafeArray)] out object[] varFileFlags, [MarshalAs(UnmanagedType.SafeArray)] out object[] varComponentFlags);

		[DispId(38)]
		[return: MarshalAs(UnmanagedType.BStr)]
		string GetPartitionID([MarshalAs(UnmanagedType.BStr)][In] string bstrApplicationIDOrName);

		[DispId(39)]
		[return: MarshalAs(UnmanagedType.BStr)]
		string GetPartitionName([MarshalAs(UnmanagedType.BStr)][In] string bstrApplicationIDOrName);

		[DispId(43)]
		[return: MarshalAs(UnmanagedType.BStr)]
		string GlobalPartitionID();

		[DispId(6)]
		void ImportComponent([MarshalAs(UnmanagedType.BStr)][In] string bstrApplIdOrName, [MarshalAs(UnmanagedType.BStr)][In] string bstrCLSIDOrProgId);

		[DispId(52)]
		void ImportComponents([MarshalAs(UnmanagedType.BStr)][In] string bstrApplicationIDOrName, [MarshalAs(UnmanagedType.LPStruct)][In] object pVarCLSIDOrProgID, [MarshalAs(UnmanagedType.LPStruct)][In] object pVarComponentType);

		[DispId(50)]
		void ImportUnconfiguredComponents([MarshalAs(UnmanagedType.BStr)][In] string bstrApplicationIDOrName, [MarshalAs(UnmanagedType.LPStruct)][In] object pVarCLSIDOrProgID, [MarshalAs(UnmanagedType.LPStruct)][In] object pVarComponentType);

		[DispId(10)]
		void InstallApplication([MarshalAs(UnmanagedType.BStr)][In] string bstrApplicationFile, [MarshalAs(UnmanagedType.BStr)][In] string bstrDestinationDirectory, [In] int lOptions, [MarshalAs(UnmanagedType.BStr)][In] string bstrUserId, [MarshalAs(UnmanagedType.BStr)][In] string bstrPassword, [MarshalAs(UnmanagedType.BStr)][In] string bstrRSN);

		[DispId(7)]
		void InstallComponent([MarshalAs(UnmanagedType.BStr)][In] string bstrApplIdOrName, [MarshalAs(UnmanagedType.BStr)][In] string bstrDLL, [MarshalAs(UnmanagedType.BStr)][In] string bstrTLB, [MarshalAs(UnmanagedType.BStr)][In] string bstrPSDLL);

		[DispId(25)]
		void InstallEventClass([MarshalAs(UnmanagedType.BStr)][In] string bstrApplIdOrName, [MarshalAs(UnmanagedType.BStr)][In] string bstrDLL, [MarshalAs(UnmanagedType.BStr)][In] string bstrTLB, [MarshalAs(UnmanagedType.BStr)][In] string bstrPSDLL);

		[DispId(16)]
		void InstallMultipleComponents([MarshalAs(UnmanagedType.BStr)][In] string bstrApplIdOrName, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)][In] ref object[] fileNames, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)][In] ref object[] CLSIDS);

		[DispId(24)]
		void InstallMultipleEventClasses([MarshalAs(UnmanagedType.BStr)][In] string bstrApplIdOrName, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)][In] ref object[] fileNames, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)][In] ref object[] CLSIDS);

		[DispId(55)]
		void InstallPartition([MarshalAs(UnmanagedType.BStr)][In] string bstrFileName, [MarshalAs(UnmanagedType.BStr)][In] string bstrDestDirectory, [MarshalAs(UnmanagedType.I4)][In] int lOptions, [MarshalAs(UnmanagedType.BStr)][In] string bstrUserID, [MarshalAs(UnmanagedType.BStr)][In] string bstrPassword, [MarshalAs(UnmanagedType.BStr)][In] string bstrRSN);

		[DispId(53)]
		[return: MarshalAs(UnmanagedType.VariantBool)]
		bool Is64BitCatalogServer();

		[DispId(35)]
		[return: MarshalAs(UnmanagedType.VariantBool)]
		bool IsApplicationInstanceDumpSupported();

		[DispId(49)]
		[return: MarshalAs(UnmanagedType.Interface)]
		object IsSafeToDelete([MarshalAs(UnmanagedType.BStr)][In] string bstrDllName);

		[DispId(3)]
		int MajorVersion();

		[DispId(4)]
		int MinorVersion();

		[DispId(47)]
		void MoveComponents([MarshalAs(UnmanagedType.BStr)][In] string bstrSourceApplicationIDOrName, [MarshalAs(UnmanagedType.LPStruct)][In] object pVarCLSIDOrProgID, [MarshalAs(UnmanagedType.BStr)][In] string bstrDestinationApplicationIDOrName);

		[DispId(30)]
		void PauseApplicationInstances([MarshalAs(UnmanagedType.LPStruct)][In] object pVarApplicationInstanceID);

		[DispId(51)]
		void PromoteUnconfiguredComponents([MarshalAs(UnmanagedType.BStr)][In] string bstrApplicationIDOrName, [MarshalAs(UnmanagedType.LPStruct)][In] object pVarCLSIDOrProgID, [MarshalAs(UnmanagedType.LPStruct)][In] object pVarComponentType);

		[DispId(21)]
		void QueryApplicationFile([MarshalAs(UnmanagedType.BStr)][In] string bstrApplicationFile, [MarshalAs(UnmanagedType.BStr)] out string bstrApplicationName, [MarshalAs(UnmanagedType.BStr)] out string bstrApplicationDescription, [MarshalAs(UnmanagedType.VariantBool)] out bool bHasUsers, [MarshalAs(UnmanagedType.VariantBool)] out bool bIsProxy, [MarshalAs(UnmanagedType.SafeArray)] out object[] varFileNames);

		[DispId(56)]
		[return: MarshalAs(UnmanagedType.IDispatch)]
		object QueryApplicationFile2([MarshalAs(UnmanagedType.BStr)][In] string bstrApplicationFile);

		[DispId(32)]
		void RecycleApplicationInstances([MarshalAs(UnmanagedType.LPStruct)][In] object pVarApplicationInstanceID, [MarshalAs(UnmanagedType.I4)][In] int lReasonCode);

		[DispId(18)]
		void RefreshComponents();

		[DispId(12)]
		void RefreshRouter();

		[DispId(14)]
		void Reserved1();

		[DispId(15)]
		void Reserved2();

		[DispId(20)]
		void RestoreREGDB([MarshalAs(UnmanagedType.BStr)][In] string bstrBackupFilePath);

		[DispId(31)]
		void ResumeApplicationInstances([MarshalAs(UnmanagedType.LPStruct)][In] object pVarApplicationInstanceID);

		[DispId(23)]
		int ServiceCheck([In] int lService);

		[DispId(8)]
		void ShutdownApplication([MarshalAs(UnmanagedType.BStr)][In] string bstrApplIdOrName);

		[DispId(29)]
		void ShutdownApplicationInstances([MarshalAs(UnmanagedType.LPStruct)][In] object pVarApplicationInstanceID);

		[DispId(22)]
		void StartApplication([MarshalAs(UnmanagedType.BStr)][In] string bstrApplIdOrName);

		[DispId(13)]
		void StartRouter();

		[DispId(11)]
		void StopRouter();
	}
}
