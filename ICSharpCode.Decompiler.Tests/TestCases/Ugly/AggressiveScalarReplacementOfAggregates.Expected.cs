using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	public class DisplayClass
	{
		public Program thisField;
		public int field1;
		public string field2;
	}

	public class NestedDisplayClass
	{
		public DisplayClass field3;
		public int field1;
		public string field2;
	}

	public class Program
	{
		public int Rand()
		{
			throw new NotImplementedException();
		}
		
		public void Test1()
		{
			int field1 = 42;
			string field2 = "Hello World!";
			Console.WriteLine("{0} {1}", field1, field2);
		}
		
		public void Test2()
		{
			DisplayClass displayClass = new DisplayClass {
				field1 = 42,
				field2 = "Hello World!"
			};
			Console.WriteLine("{0} {1}", displayClass.field1, displayClass.GetHashCode());
		}

		public void Test3()
		{
			DisplayClass displayClass = new DisplayClass {
				field1 = 42,
				field2 = "Hello World!"
			};
			Console.WriteLine("{0} {1}", displayClass.field1, displayClass);
		}

		public void Test4()
		{
			DisplayClass displayClass = new DisplayClass {
				thisField = this,
				field1 = 42,
				field2 = "Hello World!"
			};
			int field1 = 4711;
			string field2 = "ILSpy";
			DisplayClass field3;
			if (displayClass.field1 > 100) {
				field3 = displayClass;
			} else {
				field3 = null;
			}
			Console.WriteLine("{0} {1}", displayClass, field3);
		}

		public void Test5()
		{
			DisplayClass displayClass = new DisplayClass {
				thisField = this,
				field1 = 42,
				field2 = "Hello World!"
			};
			int field1 = 4711;
			string field2 = "ILSpy";
			DisplayClass field3;
			if (displayClass.field1 > 100) {
				field3 = displayClass;
			} else {
				field3 = null;
			}
			Console.WriteLine("{0} {1}", field2 + field1, field3);
		}
		
		public void Issue1898(int i)
		{
			DisplayClass displayClass = new DisplayClass {
				thisField = this,
				field1 = i
			};
			int field1 = default(int);
			string field2 = default(string);
			DisplayClass field3 = default(DisplayClass);
			while (true) {
				switch (Rand()) {
					case 1:
						field1 = Rand();
						continue;
					case 2:
						field2 = Rand().ToString();
						continue;
					case 3:
						field3 = displayClass;
						continue;
				}
				Console.WriteLine(field1);
				Console.WriteLine(field2);
				Console.WriteLine(field3);
			}
		}

		public void Test6(int i)
		{
			int field1 = i;
			string field2 = "Hello World!";
			if (i < 0) {
				i = -i;
			}
			Console.WriteLine("{0} {1}", field1, field2);
		}

		public void Test6b(int i)
		{
			int num = i;
			int field1 = num;
			string field2 = "Hello World!";
			if (num < 0) {
				num = -num;
			}
			Console.WriteLine("{0} {1}", field1, field2);
		}

		public void Test7(int i)
		{
			int field1 = i;
			string field2 = "Hello World!";
			Console.WriteLine("{0} {1} {2}", field1++, field2, i);
		}

		public void Test8(int i)
		{
			int field1 = i;
			string field2 = "Hello World!";
			i = 42;
			Console.WriteLine("{0} {1}", field1, field2);
		}

		public void Test8b(int i)
		{
			int num = i;
			int field1 = num;
			string field2 = "Hello World!";
			num = 42;
			Console.WriteLine("{0} {1}", field1, field2);
		}

//		public void Test9()
//		{
//			Program thisField = this;
//			int field1 = 1;
//			string field2 = "Hello World!";
//			thisField = new Program();
//			Console.WriteLine("{0} {1}", this, thisField);
//		}
	}
}
