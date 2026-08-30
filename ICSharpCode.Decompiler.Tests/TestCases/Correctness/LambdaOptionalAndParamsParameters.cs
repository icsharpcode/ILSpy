#pragma warning disable CS9099, CS9100
using System;
using System.Reflection;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class LambdaOptionalAndParamsParameters
	{
		static void Main()
		{
#if CS120 && !NET40
			ReportMetadata();
			ReportCalls();
#endif
		}

#if CS120 && !NET40
		delegate int OptionalFunc(int x = 5);

		delegate int PlainFunc(int x);

		delegate void ParamsAction(params int[] xs);

		delegate void PlainAction(int[] xs);

		// A lambda's parameter list does not have to repeat what the delegate declares: it may
		// state a different default, one the delegate does not have, or none where the delegate
		// has one, and the same for the params modifier. Metadata records what the lambda itself
		// declared, and reflection reports that rather than the delegate's, so the decompiled
		// lambda has to carry the lambda's own list back.
		static void Report(string name, Delegate d)
		{
			ParameterInfo p = d.Method.GetParameters()[0];
			Console.WriteLine("{0}: hasDefault={1} default={2} params={3}",
				name,
				p.HasDefaultValue,
				p.HasDefaultValue ? p.DefaultValue : "none",
				p.IsDefined(typeof(ParamArrayAttribute), inherit: false));
		}

		static void ReportMetadata()
		{
			Report("DefaultOnlyInDelegate", (OptionalFunc)((int x) => x * 2));
			Report("DefaultOnlyInLambda", (PlainFunc)((int x = 3) => x * 2));
			Report("DefaultDiffersFromDelegate", (OptionalFunc)((int x = 7) => x * 2));
			Report("DefaultAgreesWithDelegate", (OptionalFunc)((int x = 5) => x * 2));
			Report("ParamsOnlyInDelegate", (ParamsAction)((int[] xs) => Console.WriteLine(xs.Length)));
			Report("ParamsOnlyInLambda", (PlainAction)((params int[] xs) => Console.WriteLine(xs.Length)));
		}

		// The value a caller gets for an omitted argument comes from the delegate, never from the
		// lambda, so these stay the same whatever the lambda declared.
		static void ReportCalls()
		{
			OptionalFunc noDefault = (int x) => x * 2;
			OptionalFunc otherDefault = (int x = 7) => x * 2;
			Console.WriteLine(noDefault());
			Console.WriteLine(otherDefault());
			Console.WriteLine(otherDefault(1));

			ParamsAction expandedByDelegate = (int[] xs) => Console.WriteLine(xs.Length);
			expandedByDelegate(1, 2, 3);
			PlainAction notExpanded = (params int[] xs) => Console.WriteLine(xs.Length);
			notExpanded(new int[2]);
		}
#endif
	}
}
