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
			DisplayClass displayClass = new DisplayClass {
				field1 = 42,
				field2 = "Hello World!"
			};
			Console.WriteLine("{0} {1}", displayClass.field1, displayClass.field2);
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
			NestedDisplayClass nested = new NestedDisplayClass {
				field1 = 4711,
				field2 = "ILSpy"
			};
			if (displayClass.field1 > 100) {
				nested.field3 = displayClass;
			} else {
				nested.field3 = null;
			}
			Console.WriteLine("{0} {1}", displayClass, nested.field3);
		}

		public void Test5()
		{
			DisplayClass displayClass = new DisplayClass {
				thisField = this,
				field1 = 42,
				field2 = "Hello World!"
			};
			NestedDisplayClass nested = new NestedDisplayClass {
				field1 = 4711,
				field2 = "ILSpy"
			};
			if (displayClass.field1 > 100) {
				nested.field3 = displayClass;
			} else {
				nested.field3 = null;
			}
			Console.WriteLine("{0} {1}", nested.field2 + nested.field1, nested.field3);
		}
		
		public void Issue1898(int i)
		{
			DisplayClass displayClass = new DisplayClass {
				thisField = this,
				field1 = i
			};
			NestedDisplayClass nested = new NestedDisplayClass();
			while (true) {
				switch (Rand()) {
					case 1:
						nested.field1 = Rand();
						break;
					case 2:
						nested.field2 = Rand().ToString();
						break;
					case 3:
						nested.field3 = displayClass;
						break;
					default:
						Console.WriteLine(nested.field1);
						Console.WriteLine(nested.field2);
						Console.WriteLine(nested.field3);
						break;
				}
			}
		}

		public void Test6(int i)
		{
			DisplayClass displayClass = new DisplayClass {
				field1 = i,
				field2 = "Hello World!"
			};
			if (i < 0) {
				i = -i;
			}
			Console.WriteLine("{0} {1}", displayClass.field1, displayClass.field2);
		}
		
		public void Test6b(int i)
		{
			int num = i;
			DisplayClass displayClass = new DisplayClass {
				field1 = num,
				field2 = "Hello World!"
			};
			if (num < 0) {
				num = -num;
			}
			Console.WriteLine("{0} {1}", displayClass.field1, displayClass.field2);
		}

		public void Test7(int i)
		{
			DisplayClass displayClass = new DisplayClass {
				field1 = i,
				field2 = "Hello World!"
			};
			Console.WriteLine("{0} {1} {2}", displayClass.field1++, displayClass.field2, i);
		}

		public void Test8(int i)
		{
			DisplayClass displayClass = new DisplayClass {
				field1 = i,
				field2 = "Hello World!"
			};
			i = 42;
			Console.WriteLine("{0} {1}", displayClass.field1, displayClass.field2);
		}

		public void Test8b(int i)
		{
			int num = i;
			DisplayClass displayClass = new DisplayClass {
				field1 = num,
				field2 = "Hello World!"
			};
			num = 42;
			Console.WriteLine("{0} {1}", displayClass.field1, displayClass.field2);
		}

//		public void Test9()
//		{
//			DisplayClass displayClass = new DisplayClass {
//				thisField = this,
//				field1 = 1,
//				field2 = "Hello World!"
//			};
//			displayClass.thisField = new Program();
//			Console.WriteLine("{0} {1}", this, displayClass.thisField);
//		}
	}
}
