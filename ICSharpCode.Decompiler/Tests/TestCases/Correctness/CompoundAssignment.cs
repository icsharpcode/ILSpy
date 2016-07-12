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
using System.Collections.Generic;

class CompoundAssignment
{
	static void Main()
	{
		PreIncrementProperty();
	}
	
	static void Test(int a, int b)
	{
		Console.WriteLine("{0} {1}", a, b);
	}
	
	static int x;
	
	static int X()
	{
		Console.Write("X ");
		return ++x;
	}
	
	int instanceField;
	
	public int InstanceProperty {
		get {
			Console.WriteLine("In get_InstanceProperty");
			return instanceField;
		}
		set {
			Console.WriteLine("In set_InstanceProperty, value=" + value);
			instanceField = value;
		}
	}
	
	static int staticField;
	
	public static int StaticProperty {
		get {
			Console.WriteLine("In get_StaticProperty");
			return staticField;
		}
		set {
			Console.WriteLine("In set_StaticProperty, value=" + value);
			staticField = value;
		}
	}
	
	public static Dictionary<string, int> GetDict()
	{
		Console.WriteLine("In GetDict()");
		return new Dictionary<string, int>();
	}
	
	static string GetString()
	{
		Console.WriteLine("In GetString()");
		return "the string";
	}
	
	static void PreIncrementProperty()
	{
		Console.WriteLine("PreIncrementProperty:");
		Test(X(), ++new CompoundAssignment().InstanceProperty);
		Test(X(), ++StaticProperty);
	}
	
	static void PreIncrementIndexer()
	{
		Console.WriteLine("PreIncrementIndexer:");
		Test(X(), ++GetDict()[GetString()]);
	}
}
