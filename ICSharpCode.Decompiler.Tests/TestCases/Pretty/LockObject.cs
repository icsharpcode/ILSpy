using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class LockObject
	{
		private class GenericClassWithLock<T>
		{
			private readonly Lock lockObj = new Lock();

			public void LockInGenericClass(T value)
			{
				lock (lockObj)
				{
					Console.WriteLine(value);
				}
			}
		}

		private readonly Lock lockObj = new Lock();

		private static readonly Lock staticLock = new Lock();

		public void LockOnField()
		{
			lock (lockObj)
			{
				Console.WriteLine("field");
			}
		}

		public static void LockOnStaticField()
		{
			lock (staticLock)
			{
				Console.WriteLine("static field");
			}
		}

		public void LockOnParameter(Lock l)
		{
			lock (l)
			{
				Console.WriteLine("parameter");
			}
		}

		public void LockOnLocal()
		{
			Lock obj = new Lock();
			lock (obj)
			{
				Console.WriteLine("local");
			}
			Console.WriteLine(obj.IsHeldByCurrentThread);
		}

		public void LockOnMethodCallResult()
		{
			lock (GetLock())
			{
				Console.WriteLine("method call result");
			}
		}

		public void NestedLocks(Lock inner)
		{
			lock (lockObj)
			{
				Console.WriteLine("outer");
				lock (inner)
				{
					Console.WriteLine("inner");
				}
			}
		}

		public int EarlyReturnInsideLock(int x)
		{
			lock (lockObj)
			{
				if (x > 0)
				{
					return x;
				}
				Console.WriteLine("not positive");
			}
			return -1;
		}

		public async Task LockBetweenAwaits()
		{
			await Task.Yield();
			lock (lockObj)
			{
				Console.WriteLine("between awaits");
			}
			await Task.Yield();
		}

		public void LockInGenericMethod<T>(Lock l, T value)
		{
			lock (l)
			{
				Console.WriteLine(value);
			}
		}

		public Action LockInDelegate()
		{
			return delegate {
				lock (lockObj)
				{
					Console.WriteLine("delegate");
				}
			};
		}

		public void MonitorLockViaCast()
		{
#pragma warning disable 9216
			lock ((object)lockObj)
			{
				Console.WriteLine("monitor semantics");
			}
#pragma warning restore 9216
		}

		private Lock GetLock()
		{
			return lockObj;
		}
	}
}
