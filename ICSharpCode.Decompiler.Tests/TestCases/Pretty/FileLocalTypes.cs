using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	file class DerivedFileClass : SimpleFileClass, IFileInterface
	{
		public static readonly SimpleFileClass SharedInstance = new SimpleFileClass();

		public SimpleFileClass Sibling { get; set; }

		public string Describe()
		{
			return "derived";
		}

		public SimpleFileClass[] MakeArray(int size)
		{
			return new SimpleFileClass[size];
		}
	}
	file abstract class FileAbstractBase
	{
		public abstract int Compute(int input);
	}
	file sealed class FileConcrete : FileAbstractBase
	{
		public override int Compute(int input)
		{
			return input + 1;
		}
	}
	file delegate int FileDelegate(int argument);
	file enum FileEnum
	{
		None,
		Some
	}
	file static class FileExtensions
	{
		public static int Doubled(this int value)
		{
			return value * 2;
		}

		public static void PrintDescription(this IFileInterface helper)
		{
			Console.WriteLine(helper.Describe());
		}
	}
	file static class FileFactory
	{
		public static IFileInterface CreateHelper()
		{
			return new DerivedFileClass();
		}

		public static FileAbstractBase CreateBase()
		{
			return new FileConcrete();
		}

		public static FileEnum MakeEnum()
		{
			return FileEnum.Some;
		}
	}
	file class FileGeneric<T> where T : class
	{
		public T Item { get; set; }

		public List<T> ToList()
		{
			return new List<T> { Item };
		}
	}
	file sealed class FileNestedHost
	{
		public class Nested
		{
			public int Number;
		}

		private readonly Nested nested = new Nested();

		public int Read()
		{
			return nested.Number;
		}
	}
	file struct FileStruct
	{
		public int X;

		public int Y;

		public readonly int Sum()
		{
			return X + Y;
		}
	}
	file interface IFileInterface
	{
		string Describe();
	}
	file class SimpleFileClass
	{
		public int Value;

		public int GetValue()
		{
			return Value;
		}
	}
	public class FileLocalTypes
	{
		public void Declarations()
		{
			SimpleFileClass simpleFileClass = new SimpleFileClass();
			simpleFileClass.Value = 42;
			Console.WriteLine(simpleFileClass.GetValue());
			DerivedFileClass derivedFileClass = new DerivedFileClass();
			Console.WriteLine(derivedFileClass.Describe());
			derivedFileClass.Sibling = simpleFileClass;
			Console.WriteLine(derivedFileClass.Sibling.Value);
		}

		public void InterfacesAndInheritance()
		{
			IFileInterface fileInterface = FileFactory.CreateHelper();
			Console.WriteLine(fileInterface.Describe());
			fileInterface.PrintDescription();
			SimpleFileClass sharedInstance = DerivedFileClass.SharedInstance;
			Console.WriteLine(sharedInstance.GetValue());
			Console.WriteLine(sharedInstance.Value);
			FileAbstractBase fileAbstractBase = FileFactory.CreateBase();
			Console.WriteLine(fileAbstractBase.Compute(1));
			Console.WriteLine(fileAbstractBase.Compute(2));
		}

		public void StructsAndEnums()
		{
			FileStruct fileStruct = new FileStruct {
				X = 1,
				Y = 2
			};
			Console.WriteLine(fileStruct.Sum());
			FileEnum fileEnum = FileFactory.MakeEnum();
			Console.WriteLine(fileEnum);
			Console.WriteLine(fileEnum == FileEnum.None);
		}

		public void DelegatesAndExtensions()
		{
			FileDelegate fileDelegate = (int argument) => argument + 1;
			Console.WriteLine(fileDelegate(1) + fileDelegate(2));
			Console.WriteLine(3.Doubled());
		}

		public void GenericsAndArrays()
		{
			FileGeneric<string> fileGeneric = new FileGeneric<string>();
			Console.WriteLine(fileGeneric.ToList().Count);
			fileGeneric.Item = "hi";
			Console.WriteLine(fileGeneric.Item);
			SimpleFileClass[] array = new DerivedFileClass().MakeArray(3);
			array[0] = new SimpleFileClass();
			Console.WriteLine(array.Length);
			List<SimpleFileClass> list = new List<SimpleFileClass>();
			Console.WriteLine(list.Count);
			list.Add(new SimpleFileClass());
			Console.WriteLine(list.Count);
		}

		public void NestedTypes()
		{
			FileNestedHost fileNestedHost = new FileNestedHost();
			Console.WriteLine(fileNestedHost.Read());
			FileNestedHost.Nested nested = new FileNestedHost.Nested();
			Console.WriteLine(nested.Number);
			nested.Number = 7 + fileNestedHost.Read();
			Console.WriteLine(nested.Number);
		}
	}
}
