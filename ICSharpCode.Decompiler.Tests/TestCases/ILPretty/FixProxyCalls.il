// C:\Users\M.Hedtke\Documents\FixProxyCalls\FixProxyCalls\bin\Debug\netstandard2.0\FixProxyCalls.dll

.assembly extern netstandard
{
	.publickeytoken = (
		cc 7b 13 ff cd 2d dd 51
	)
	.ver 2:0:0:0
}
.assembly FixProxyCalls
{
	.hash algorithm 0x00008004 // SHA1
	.ver 1:0:0:0
}

.module FixProxyCalls.dll
// MVID: {1C9C9ED2-FBF8-44D2-9699-305E3CFD7A94}
.corflags 0x00000001 // ILOnly


.class private auto ansi '<Module>'
{
} // end of class <Module>

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls
	extends [netstandard]System.Object
{
	// Nested Types
	.class nested public auto ansi beforefieldinit TestHandler
		extends [netstandard]System.Net.Http.DelegatingHandler
	{
		// Nested Types
		.class nested private auto ansi sealed beforefieldinit '<SendAsync>d__0'
			extends [netstandard]System.Object
			implements [netstandard]System.Runtime.CompilerServices.IAsyncStateMachine
		{
			.custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
				01 00 00 00
			)
			// Fields
			.field public int32 '<>1__state'
			.field public valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [netstandard]System.Net.Http.HttpResponseMessage> '<>t__builder'
			.field public class [netstandard]System.Net.Http.HttpRequestMessage r
			.field public valuetype [netstandard]System.Threading.CancellationToken c
			.field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler '<>4__this'
			.field private class [netstandard]System.Net.Http.HttpResponseMessage '<>s__1'
			.field private valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<class [netstandard]System.Net.Http.HttpResponseMessage> '<>u__1'

			// Methods
			.method public hidebysig specialname rtspecialname 
				instance void .ctor () cil managed 
			{
				// Method begins at RVA 0x20c4
				// Code size 8 (0x8)
				.maxstack 8

				IL_0000: ldarg.0
				IL_0001: call instance void [netstandard]System.Object::.ctor()
				IL_0006: nop
				IL_0007: ret
			} // end of method '<SendAsync>d__0'::.ctor

			.method private final hidebysig newslot virtual 
				instance void MoveNext () cil managed 
			{
				.override method instance void [netstandard]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext()
				// Method begins at RVA 0x20d0
				// Code size 187 (0xbb)
				.maxstack 3
				.locals init (
					[0] int32,
					[1] class [netstandard]System.Net.Http.HttpResponseMessage,
					[2] valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<class [netstandard]System.Net.Http.HttpResponseMessage>,
					[3] class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0',
					[4] class [netstandard]System.Exception
				)

				IL_0000: ldarg.0
				IL_0001: ldfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>1__state'
				IL_0006: stloc.0
				.try
				{
					IL_0007: ldloc.0
					IL_0008: brfalse.s IL_000c

					IL_000a: br.s IL_000e

					IL_000c: br.s IL_0059

					IL_000e: nop
					IL_000f: ldarg.0
					IL_0010: ldfld class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>4__this'
					IL_0015: ldarg.0
					IL_0016: ldfld class [netstandard]System.Net.Http.HttpRequestMessage ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::r
					IL_001b: ldarg.0
					IL_001c: ldfld valuetype [netstandard]System.Threading.CancellationToken ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::c
					IL_0021: call instance class [netstandard]System.Threading.Tasks.Task`1<class [netstandard]System.Net.Http.HttpResponseMessage> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler::'<>n__0'(class [netstandard]System.Net.Http.HttpRequestMessage, valuetype [netstandard]System.Threading.CancellationToken)
					IL_0026: callvirt instance valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [netstandard]System.Threading.Tasks.Task`1<class [netstandard]System.Net.Http.HttpResponseMessage>::GetAwaiter()
					IL_002b: stloc.2
					IL_002c: ldloca.s 2
					IL_002e: call instance bool valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<class [netstandard]System.Net.Http.HttpResponseMessage>::get_IsCompleted()
					IL_0033: brtrue.s IL_0075

					IL_0035: ldarg.0
					IL_0036: ldc.i4.0
					IL_0037: dup
					IL_0038: stloc.0
					IL_0039: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>1__state'
					IL_003e: ldarg.0
					IL_003f: ldloc.2
					IL_0040: stfld valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<class [netstandard]System.Net.Http.HttpResponseMessage> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>u__1'
					IL_0045: ldarg.0
					IL_0046: stloc.3
					IL_0047: ldarg.0
					IL_0048: ldflda valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [netstandard]System.Net.Http.HttpResponseMessage> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>t__builder'
					IL_004d: ldloca.s 2
					IL_004f: ldloca.s 3
					IL_0051: call instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [netstandard]System.Net.Http.HttpResponseMessage>::AwaitUnsafeOnCompleted<valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<class [netstandard]System.Net.Http.HttpResponseMessage>, class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'>(!!0&, !!1&)
					IL_0056: nop
					IL_0057: leave.s IL_00ba

					IL_0059: ldarg.0
					IL_005a: ldfld valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<class [netstandard]System.Net.Http.HttpResponseMessage> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>u__1'
					IL_005f: stloc.2
					IL_0060: ldarg.0
					IL_0061: ldflda valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<class [netstandard]System.Net.Http.HttpResponseMessage> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>u__1'
					IL_0066: initobj valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<class [netstandard]System.Net.Http.HttpResponseMessage>
					IL_006c: ldarg.0
					IL_006d: ldc.i4.m1
					IL_006e: dup
					IL_006f: stloc.0
					IL_0070: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>1__state'

					IL_0075: ldarg.0
					IL_0076: ldloca.s 2
					IL_0078: call instance !0 valuetype [netstandard]System.Runtime.CompilerServices.TaskAwaiter`1<class [netstandard]System.Net.Http.HttpResponseMessage>::GetResult()
					IL_007d: stfld class [netstandard]System.Net.Http.HttpResponseMessage ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>s__1'
					IL_0082: ldarg.0
					IL_0083: ldfld class [netstandard]System.Net.Http.HttpResponseMessage ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>s__1'
					IL_0088: stloc.1
					IL_0089: leave.s IL_00a5
				} // end .try
				catch [netstandard]System.Exception
				{
					IL_008b: stloc.s 4
					IL_008d: ldarg.0
					IL_008e: ldc.i4.s -2
					IL_0090: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>1__state'
					IL_0095: ldarg.0
					IL_0096: ldflda valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [netstandard]System.Net.Http.HttpResponseMessage> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>t__builder'
					IL_009b: ldloc.s 4
					IL_009d: call instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [netstandard]System.Net.Http.HttpResponseMessage>::SetException(class [netstandard]System.Exception)
					IL_00a2: nop
					IL_00a3: leave.s IL_00ba
				} // end handler

				IL_00a5: ldarg.0
				IL_00a6: ldc.i4.s -2
				IL_00a8: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>1__state'
				IL_00ad: ldarg.0
				IL_00ae: ldflda valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [netstandard]System.Net.Http.HttpResponseMessage> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>t__builder'
				IL_00b3: ldloc.1
				IL_00b4: call instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [netstandard]System.Net.Http.HttpResponseMessage>::SetResult(!0)
				IL_00b9: nop

				IL_00ba: ret
			} // end of method '<SendAsync>d__0'::MoveNext

			.method private final hidebysig newslot virtual 
				instance void SetStateMachine (
					class [netstandard]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine
				) cil managed 
			{
				.custom instance void [netstandard]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
					01 00 00 00
				)
				.override method instance void [netstandard]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine(class [netstandard]System.Runtime.CompilerServices.IAsyncStateMachine)
				// Method begins at RVA 0x21a8
				// Code size 1 (0x1)
				.maxstack 8

				IL_0000: ret
			} // end of method '<SendAsync>d__0'::SetStateMachine

		} // end of class <SendAsync>d__0


		// Methods
		.method family hidebysig virtual 
			instance class [netstandard]System.Threading.Tasks.Task`1<class [netstandard]System.Net.Http.HttpResponseMessage> SendAsync (
				class [netstandard]System.Net.Http.HttpRequestMessage r,
				valuetype [netstandard]System.Threading.CancellationToken c
			) cil managed 
		{
			.custom instance void [netstandard]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [netstandard]System.Type) = (
				01 00 59 49 43 53 68 61 72 70 43 6f 64 65 2e 44
				65 63 6f 6d 70 69 6c 65 72 2e 54 65 73 74 73 2e
				54 65 73 74 43 61 73 65 73 2e 49 4c 50 72 65 74
				74 79 2e 46 69 78 50 72 6f 78 79 43 61 6c 6c 73
				2b 54 65 73 74 48 61 6e 64 6c 65 72 2b 3c 53 65
				6e 64 41 73 79 6e 63 3e 64 5f 5f 30 00 00
			)
			.custom instance void [netstandard]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = (
				01 00 00 00
			)
			// Method begins at RVA 0x205c
			// Code size 73 (0x49)
			.maxstack 2
			.locals init (
				[0] class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0',
				[1] valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [netstandard]System.Net.Http.HttpResponseMessage>
			)

			IL_0000: newobj instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::.ctor()
			IL_0005: stloc.0
			IL_0006: ldloc.0
			IL_0007: ldarg.0
			IL_0008: stfld class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>4__this'
			IL_000d: ldloc.0
			IL_000e: ldarg.1
			IL_000f: stfld class [netstandard]System.Net.Http.HttpRequestMessage ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::r
			IL_0014: ldloc.0
			IL_0015: ldarg.2
			IL_0016: stfld valuetype [netstandard]System.Threading.CancellationToken ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::c
			IL_001b: ldloc.0
			IL_001c: call valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [netstandard]System.Net.Http.HttpResponseMessage>::Create()
			IL_0021: stfld valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [netstandard]System.Net.Http.HttpResponseMessage> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>t__builder'
			IL_0026: ldloc.0
			IL_0027: ldc.i4.m1
			IL_0028: stfld int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>1__state'
			IL_002d: ldloc.0
			IL_002e: ldfld valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [netstandard]System.Net.Http.HttpResponseMessage> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>t__builder'
			IL_0033: stloc.1
			IL_0034: ldloca.s 1
			IL_0036: ldloca.s 0
			IL_0038: call instance void valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [netstandard]System.Net.Http.HttpResponseMessage>::Start<class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'>(!!0&)
			IL_003d: ldloc.0
			IL_003e: ldflda valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [netstandard]System.Net.Http.HttpResponseMessage> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls/TestHandler/'<SendAsync>d__0'::'<>t__builder'
			IL_0043: call instance class [netstandard]System.Threading.Tasks.Task`1<!0> valuetype [netstandard]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<class [netstandard]System.Net.Http.HttpResponseMessage>::get_Task()
			IL_0048: ret
		} // end of method TestHandler::SendAsync

		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x20b1
			// Code size 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [netstandard]System.Net.Http.DelegatingHandler::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method TestHandler::.ctor

		.method private hidebysig 
			instance class [netstandard]System.Threading.Tasks.Task`1<class [netstandard]System.Net.Http.HttpResponseMessage> '<>n__0' (
				class [netstandard]System.Net.Http.HttpRequestMessage 'request',
				valuetype [netstandard]System.Threading.CancellationToken cancellationToken
			) cil managed 
		{
			.custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
				01 00 00 00
			)
			.custom instance void [netstandard]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
				01 00 00 00
			)
			// Method begins at RVA 0x20ba
			// Code size 9 (0x9)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: ldarg.1
			IL_0002: ldarg.2
			IL_0003: call instance class [netstandard]System.Threading.Tasks.Task`1<class [netstandard]System.Net.Http.HttpResponseMessage> [netstandard]System.Net.Http.DelegatingHandler::SendAsync(class [netstandard]System.Net.Http.HttpRequestMessage, valuetype [netstandard]System.Threading.CancellationToken)
			IL_0008: ret
		} // end of method TestHandler::'<>n__0'

	} // end of class TestHandler


	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [netstandard]System.Object::.ctor()
		IL_0006: nop
		IL_0007: ret
	} // end of method FixProxyCalls::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.FixProxyCalls

