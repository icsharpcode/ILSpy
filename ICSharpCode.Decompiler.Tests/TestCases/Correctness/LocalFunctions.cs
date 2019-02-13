using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalFunctions
{
	class LocalFunctions
	{
		int field;

		public static void Main(string[] args)
		{
			StaticContextNoCapture(10);
			StaticContextSimpleCapture(10);
			StaticContextCaptureInForLoop(10);
			var inst = new LocalFunctions() { field = 10 };
			inst.ContextNoCapture();
			inst.ContextSimpleCapture();
			inst.ContextCaptureInForLoop();
		}

		public static void StaticContextNoCapture(int length)
		{
			for (int i = 0; i < length; i++) {
				LocalWrite("Hello " + i);
			}

			void LocalWrite(string s) => Console.WriteLine(s);
		}

		public static void StaticContextSimpleCapture(int length)
		{
			for (int i = 0; i < length; i++) {
				LocalWrite();
			}

			void LocalWrite() => Console.WriteLine("Hello " + length);
		}

		public static void StaticContextCaptureInForLoop(int length)
		{
			for (int i = 0; i < length; i++) {
				void LocalWrite() => Console.WriteLine("Hello " + i + "/" + length);
				LocalWrite();
			}
		}

		public void ContextNoCapture()
		{
			for (int i = 0; i < field; i++) {
				LocalWrite("Hello " + i);
			}

			void LocalWrite(string s) => Console.WriteLine(s);
		}

		public void ContextSimpleCapture()
		{
			for (int i = 0; i < field; i++) {
				LocalWrite();
			}

			void LocalWrite() => Console.WriteLine("Hello " + field);
		}

		public void ContextCaptureInForLoop()
		{
			for (int i = 0; i < field; i++) {
				void LocalWrite() => Console.WriteLine("Hello " + i + "/" + field);
				LocalWrite();
			}
		}
	}
}
