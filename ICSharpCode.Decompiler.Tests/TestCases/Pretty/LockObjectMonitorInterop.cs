using System;
using System.Threading;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class LockObjectMonitorInterop
	{
		private readonly Lock lockObj = new Lock();

		private object untypedLock = new object();

		public void MonitorLockOnObjectField()
		{
			lock (untypedLock)
			{
				Console.WriteLine("object field");
			}
		}

		public void MonitorLockOnStoredLock()
		{
#pragma warning disable 9216
			untypedLock = lockObj;
#pragma warning restore 9216
			lock (untypedLock)
			{
				Console.WriteLine("stored lock");
			}
		}

		public void LockOnGenericParameter<T>(T x) where T : class
		{
			lock (x)
			{
				Console.WriteLine("generic");
			}
		}

		public bool TryEnterExplicit()
		{
			if (lockObj.TryEnter())
			{
				try
				{
					Console.WriteLine("entered");
					return true;
				}
				finally
				{
					lockObj.Exit();
				}
			}
			return false;
		}

		public void EnterExitExplicit()
		{
			lockObj.Enter();
			try
			{
				Console.WriteLine("explicit");
			}
			finally
			{
				lockObj.Exit();
			}
		}
	}
}
