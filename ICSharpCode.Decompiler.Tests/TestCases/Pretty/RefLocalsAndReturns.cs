using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class RefLocalsAndReturns
	{
		public delegate ref T RefFunc<T>();
		public delegate ref readonly T ReadOnlyRefFunc<T>();

		public ref struct RefStruct
		{
			private int dummy;
		}

		public readonly ref struct ReadOnlyRefStruct
		{
			private readonly int dummy;
		}

		public struct NormalStruct
		{
			private readonly int dummy;

			public void Method()
			{
			}
		}

		public readonly struct ReadOnlyStruct
		{
			private readonly int dummy;

			public void Method()
			{
			}
		}

		public static ref T GetRef<T>()
		{
			throw new NotImplementedException();
		}

		public static ref readonly T GetReadonlyRef<T>()
		{
			throw new NotImplementedException();
		}

		public void CallOnRefReturn()
		{
			// Both direct calls:
			GetRef<NormalStruct>().Method();
			GetRef<ReadOnlyStruct>().Method();

			// call on a copy, not the original ref:
			NormalStruct @ref = GetRef<NormalStruct>();
			@ref.Method();

			ReadOnlyStruct ref2 = GetRef<ReadOnlyStruct>();
			ref2.Method();
		}

		public void CallOnReadOnlyRefReturn()
		{
			// uses implicit temporary:
			GetReadonlyRef<NormalStruct>().Method();
			// direct call:
			GetReadonlyRef<ReadOnlyStruct>().Method();
			// call on a copy, not the original ref:
			ReadOnlyStruct readonlyRef = GetReadonlyRef<ReadOnlyStruct>();
			readonlyRef.Method();
		}

		public void CallOnInParam(in NormalStruct ns, in ReadOnlyStruct rs)
		{
			// uses implicit temporary:
			ns.Method();
			// direct call:
			rs.Method();
			// call on a copy, not the original ref:
			ReadOnlyStruct readOnlyStruct = rs;
			readOnlyStruct.Method();
		}
	}
}
