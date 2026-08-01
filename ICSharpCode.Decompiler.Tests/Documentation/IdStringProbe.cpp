// Probe assembly for MSVC's xml doc ID string generator (the ECMA-372-style
// dialect ILSpy's IdStringProvider must reproduce for C++/CLI assemblies).
// Every documented member below lands in the generated .xml; the members are
// independent, so if one fails to compile, delete it and rebuild.
//
// Build (VS Developer Command Prompt):
//   msbuild IdStringProbe.vcxproj -p:Configuration=Debug -p:Platform=x64
// Return: x64\Debug\IdStringProbe.dll and x64\Debug\IdStringProbe.xml

using namespace System;
using namespace System::Collections::Generic;

/// <summary>Probe class.</summary>
public ref class IdProbe
{
public:
	/// <summary>const int parameter; known baseline: System.Int32!System.Runtime.CompilerServices.IsConst</summary>
	static int ConstValue(const int x) { return x; }

	/// <summary>long parameter; modopt(IsLong) on Int32</summary>
	static void TakesLong(long x) { (void)x; }

	/// <summary>unsigned long parameter; modopt(IsLong) on UInt32</summary>
	static void TakesULong(unsigned long x) { (void)x; }

	/// <summary>char pointer parameter; modopt(IsSignUnspecifiedByte) under a pointer</summary>
	static void TakesCharPtr(char* p) { (void)p; }

	/// <summary>const char pointer parameter; IsConst and IsSignUnspecifiedByte together (modifier ordering)</summary>
	static void TakesConstCharPtr(const char* p) { (void)p; }

	/// <summary>volatile int pointer parameter; modreq(IsVolatile): the open question is whether MSVC renders '|', '!', or omits it</summary>
	static void TakesVolatilePtr(volatile int* p) { (void)p; }

	/// <summary>const volatile int pointer parameter; modreq and modopt mixed on one type</summary>
	static void TakesConstVolatilePtr(const volatile int* p) { (void)p; }

	/// <summary>tracking reference parameter; expected System.Int32@</summary>
	static void TakesTrackingRef(int% r) { r = 0; }

	/// <summary>const tracking reference parameter; modifier placement relative to the '@'</summary>
	static void TakesConstTrackingRef(const int% r) { (void)r; }

	/// <summary>native reference parameter under /clr; representation evidence</summary>
	static void TakesNativeRef(int& r) { r = 0; }

	/// <summary>two-dimensional managed array parameter; expected System.Int32[0:,0:]</summary>
	static void TakesArray2(array<int, 2>^ a) { (void)a; }

	/// <summary>nested type of a generic instantiation; how does MSVC distribute the type arguments?</summary>
	static void TakesEnumerator(List<int>::Enumerator e) { (void)e; }

	/// <summary>generic method; expected arity marker and grave-accent parameter encoding</summary>
	generic<typename T> static void Gen(T t) { (void)t; }

	/// <summary>conversion operator with a by-value ref-class parameter; the MSVC docs show IdProbe!IsByValue and a '~' return type</summary>
	static explicit operator int(IdProbe x) { (void)x; return 0; }

	/// <summary>indexed property with a long parameter; modopt inside the indexer parentheses</summary>
	property int default[long]
	{
		int get(long i) { return (int)i; }
		void set(long i, int value) { (void)i; (void)value; }
	}
};

/// <summary>Generic ref class; arity in the type ID.</summary>
generic<typename T> public ref class GBox
{
public:
	/// <summary>method on a generic type taking T; expected grave-accent zero</summary>
	void Hold(T item) { (void)item; }
};

/// <summary>Consumer of an instantiated generic type.</summary>
public ref class GBoxUser
{
public:
	/// <summary>generic instantiation in a signature; expected GBox{System.Int32}</summary>
	static void Use(GBox<int>^ b) { (void)b; }
};
