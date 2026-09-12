using System;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class DynamicAwait
	{
		private static Task DoAsync(object o)
		{
			return Task.CompletedTask;
		}

		private static dynamic GetDynamic()
		{
			return null;
		}

		private static Task<int> GetIntAsync(object o)
		{
			return Task.FromResult(0);
		}

		private static Task<object> GetObjAsync()
		{
			return Task.FromResult<object>(null);
		}

		// --- dynamic operations around a normal (real-Task) await ---

		private static async Task SetMemberBeforeAwait(dynamic d)
		{
			d.Before = 1;
			await DoAsync(null);
		}

		private static async Task SetMemberAfterAwait(dynamic d)
		{
			await DoAsync(null);
			d.After = 1;
		}

		private static async Task SetMemberAroundAwait(dynamic d)
		{
			d.Before = 1;
			await DoAsync(null);
			d.After = 2;
		}

		private static async Task InvokeBeforeAwait(dynamic d)
		{
			d.Before();
			await DoAsync(null);
		}

		private static async Task InvokeAfterAwait(dynamic d)
		{
			await DoAsync(null);
			d.After();
		}

		private static async Task DynamicInLoopWithAwait(dynamic d)
		{
			for (int i = 0; i < 10; i++)
			{
				d.Add(i);
				await DoAsync(null);
			}
		}

		private static async Task DynamicWhileConditionWithAwait(dynamic d)
		{
			while ((bool)d.HasNext)
			{
				await DoAsync(null);
			}
		}

		private static async Task TwoAwaitsWithDynamicBetween(dynamic d)
		{
			await DoAsync(null);
			d.Middle = 1;
			await DoAsync(null);
		}

		private static async Task DynamicConditionGuardingAwait(dynamic d)
		{
			if ((bool)d.Flag)
			{
				await DoAsync(null);
			}
		}

		private static async Task DynamicAwaitInTryCatch(dynamic d)
		{
			// Optimized pre-Roslyn state machines keep the bookkeeping of an await inside a try
			// block in locals that survive decompilation as dead stores; a plain await on a Task
			// produces the same three stores.
#if EXPECTED_OUTPUT && OPT && LEGACY_CSC
			int num = default(int);
			int num2 = num;
			int num3 = 0;
#endif
			try
			{
				d.Before = 1;
				await DoAsync(null);
			}
			catch
			{
			}
		}

		// --- dynamic feeding the awaited expression (awaiter stays a real Task) ---

		private static async Task DynamicConvertedArgumentToAwaitedCall(dynamic d)
		{
			await DoAsync((string)d.Value);
		}

		private static async Task AwaitTaskFromDynamicCast(dynamic d)
		{
			await (Task)d.GetTask();
		}

		private static async Task<int> AwaitIntTaskFromDynamicCast(dynamic d)
		{
			return await (Task<int>)d.GetIntTask();
		}

		// --- awaited result feeding a dynamic operation ---

		private static async Task StoreAwaitResultInDynamicMember(dynamic d)
		{
			d.Result = await GetIntAsync(null);
		}

		private static async Task PassAwaitResultToDynamicInvoke(dynamic d)
		{
			d.Consume(await GetObjAsync());
		}

		private static async Task DynamicIndexerWithAwaitedIndex(dynamic d)
		{
			d[await GetIntAsync(null)] = 1;
		}

		private static async Task<object> DynamicBinaryOpWithAwaitedOperand(dynamic d)
		{
			return d + await GetIntAsync(null);
		}

		private static async Task<int> DynamicConvertOfAwaitedResult()
		{
			return (int)(dynamic)(await GetObjAsync());
		}

		// --- real-world shapes from the referenced issues ---

		// awaited dynamic stored in a dynamic local, then used (#1388)
		private static async Task<dynamic> AwaitDynamicIntoUsedLocal()
		{
			// The pre-Roslyn compiler copies the awaited value into its own local first.
#if LEGACY_CSC
			dynamic dynamic = GetDynamic();
			dynamic val = await dynamic;
#else
			dynamic val = await GetDynamic();
#endif
			val.UseMe();
			return val;
		}

		// dynamic-dispatched awaited call with a dynamic-converted result (#1928)
		private static async Task<string> AwaitDynamicConvertResult(dynamic d)
		{
#if LEGACY_CSC
			dynamic val = d.RunAsync();
			return (string)(await val);
#else
			return (string)(await d.RunAsync());
#endif
		}

		// --- static-type call target spilled across an await ---

		private static void StaticMember(dynamic d, int value)
		{
		}

		private static async Task StaticTargetWithAwaitedArgument(object o)
		{
			DynamicAwait.StaticMember((dynamic)o, await GetIntAsync(null));
		}

		// --- fully dynamic awaiter (the awaitable itself is dynamic) ---

		private static async Task AwaitDynamicValue(dynamic d)
		{
			await d;
		}

		private static async Task AwaitDynamicMethodResult(dynamic d)
		{
#if LEGACY_CSC
			dynamic val = d.RunAsync();
			await val;
#else
			await d.RunAsync();
#endif
		}

		private static async Task AwaitDynamicLocalWithStatementBefore()
		{
			// Optimized builds drop the local's debug name, so the decompiler regenerates one from
			// the type; the pre-Roslyn compiler never emits the name in the first place.
#if OPT || LEGACY_CSC
			dynamic dynamic = GetDynamic();
			Console.WriteLine("before await");
			await dynamic;
#else
			dynamic x = GetDynamic();
			Console.WriteLine("before await");
			await x;
#endif
		}
	}
}
