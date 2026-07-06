using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class LambdaParameterModifiers
	{
		public delegate void RefAction(ref int x);

		public delegate bool TryParseDelegate(string s, out int result);

		public delegate int InFunc(in DateTime d);

		public delegate int RefReadonlyFunc(ref readonly int x);

		public delegate void RefGenericAction<T>(ref T item);

		public delegate int ScopedSpanFunc(scoped Span<int> s);

		public delegate Span<int> ScopedMixedFunc(scoped ref int x, ref int y);

		public delegate void ScopedRefAction(scoped ref int x);

		private int instanceField;

		public RefAction RefBlockBody()
		{
			return delegate (ref int x) {
				x++;
			};
		}

		public RefAction RefExpressionBody()
		{
#if EXPECTED_OUTPUT
			return delegate (ref int x) {
				x *= 2;
			};
#elif CS140
			// C# 14 simple lambda parameter with modifiers: no type needed
			return (ref x) => x *= 2;
#else
			return (ref int x) => x *= 2;
#endif
		}

		public TryParseDelegate OutLambda()
		{
#if EXPECTED_OUTPUT
			return delegate (string s, out int result) {
				return int.TryParse(s, out result);
			};
#elif CS140
			// C# 14 simple lambda parameter with modifiers: no type needed
			return (s, out result) => int.TryParse(s, out result);
#else
			return (string s, out int result) => int.TryParse(s, out result);
#endif
		}

		public InFunc InLambda()
		{
			return delegate (in DateTime d) {
				return d.Year;
			};
		}

		public RefReadonlyFunc RefReadonlyLambda()
		{
			return delegate (ref readonly int x) {
				return x + 1;
			};
		}

		public RefGenericAction<string> GenericRefLambda()
		{
			return delegate (ref string item) {
				item += "!";
			};
		}

		public RefAction CaptureAlongsideRefParameter(int offset)
		{
			return delegate (ref int x) {
				x += offset + instanceField;
			};
		}

		public ScopedSpanFunc ScopedSpanLambda()
		{
			return (scoped Span<int> s) => s.Length;
		}

		public ScopedMixedFunc ScopedMixedLambda()
		{
			return delegate (scoped ref int x, ref int y) {
				x += y;
				return default(Span<int>);
			};
		}

		public ScopedRefAction ScopedRefLambda()
		{
			return delegate (scoped ref int x) {
				x++;
			};
		}

		public RefAction AttributedRefLambda()
		{
#if EXPECTED_OUTPUT
			return ([ParamMod] ref int x) => {
				x += 10;
			};
#elif CS140
			// C# 14 also allows attributes on simple lambda parameters
			return ([ParamMod] ref x) => x += 10;
#else
			return ([ParamMod] ref int x) => x += 10;
#endif
		}

		public RefAction UnusedRefParameter()
		{
			return delegate {
			};
		}

		public RefAction LocalFunctionWithRef()
		{
			return LocalRef;
			static void LocalRef(ref int x)
			{
				x -= 3;
			}
		}

		public int InvokeAll()
		{
			int x = 1;
			RefBlockBody()(ref x);
			RefExpressionBody()(ref x);
			OutLambda()("42", out var result);
			DateTime d = DateTime.MinValue;
			int num = InLambda()(in d) + RefReadonlyLambda()(in x);
			string item = "a";
			GenericRefLambda()(ref item);
			CaptureAlongsideRefParameter(2)(ref x);
			LocalFunctionWithRef()(ref x);
			return x + result + num + item.Length;
		}
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	internal class ParamModAttribute : Attribute
	{
	}
}
