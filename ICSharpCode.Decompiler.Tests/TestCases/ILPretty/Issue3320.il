.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
  .ver 4:0:0:0
}
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
  .ver 4:0:0:0
}
.assembly extern Microsoft.CSharp
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver 4:0:0:0
}
.assembly Issue3320
{
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module Issue3320.dll
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY

.class interface public abstract auto ansi IDbConnection
{
}

.class interface public abstract auto ansi IDbTransaction
{
}

.class interface public abstract auto ansi IDapperAsyncImplementor
{
	.method public hidebysig newslot abstract virtual
		instance class [mscorlib]System.Threading.Tasks.Task`1<!!T> GetAsync<class T> (
			class IDbConnection connection,
			object id,
			class IDbTransaction transaction,
			valuetype [mscorlib]System.Nullable`1<int32> commandTimeout
		) cil managed
	{
	}
}

.class public abstract auto ansi sealed beforefieldinit Issue3320
	extends [mscorlib]System.Object
{
	.method public hidebysig specialname static
		class IDapperAsyncImplementor get_Instance () cil managed
	{
		.maxstack 8

		IL_0000: ldnull
		IL_0001: ret
	}

	.property class IDapperAsyncImplementor Instance()
	{
		.get class IDapperAsyncImplementor Issue3320::get_Instance()
	}

	.class nested private auto ansi abstract sealed beforefieldinit '<GetAsync>o__SiteContainer6`1'<T>
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>> '<>p__Site7'
		.field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`7<class [System.Core]System.Runtime.CompilerServices.CallSite, class IDapperAsyncImplementor, class IDbConnection, object, class IDbTransaction, valuetype [mscorlib]System.Nullable`1<int32>, object>> '<>p__Site8'
		.field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> '<>p__Site9'
		.field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, bool>> '<>p__Sitea'
		.field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> '<>p__Siteb'
		.field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> '<>p__Sitec'

	} // end of class <GetAsync>o__SiteContainer6`1

	.class nested private auto ansi sealed beforefieldinit '<GetAsync>d__d`1'<class T>
		extends [mscorlib]System.ValueType
		implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public int32 '<>1__state'
		.field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!T> '<>t__builder'
		.field public class IDbConnection connection
		.field public object id
		.field public class IDbTransaction transaction
		.field public valuetype [mscorlib]System.Nullable`1<int32> commandTimeout
		.field private object '<>u__$awaitere'
		.field private object '<>t__stack'

		// Methods
		.method private final hidebysig newslot virtual 
			instance void MoveNext () cil managed 
		{
			.override method instance void [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext()
			// Method begins at RVA 0x7188
			// Header size: 12
			// Code size: 788 (0x314)
			.maxstack 11
			.locals init (
				[0] bool,
				[1] !T,
				[2] class [mscorlib]System.Exception,
				[3] int32,
				[4] class [mscorlib]System.Type[],
				[5] class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[],
				[6] object,
				[7] class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[],
				[8] object,
				[9] object,
				[10] class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[],
				[11] class [mscorlib]System.Tuple`2<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>, class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>>>,
				[12] object,
				[13] class [mscorlib]System.Runtime.CompilerServices.ICriticalNotifyCompletion,
				[14] object,
				[15] class [mscorlib]System.Runtime.CompilerServices.INotifyCompletion,
				[16] class [mscorlib]System.Tuple`2<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>, class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>>>,
				[17] object,
				[18] class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[]
			)

			.try
			{
				IL_0000: ldc.i4.1
				IL_0001: stloc.0
				IL_0002: ldarg.0
				IL_0003: ldfld int32 valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>1__state'
				IL_0008: stloc.3
				IL_0009: ldloc.3
				IL_000a: ldc.i4.0
				IL_000b: beq IL_024a

				IL_0010: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !0>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Site7'
				IL_0015: brtrue.s IL_003b

				IL_0017: ldc.i4.0
				IL_0018: ldtoken !T
				IL_001d: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
				IL_0022: ldtoken Issue3320
				IL_0027: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
				IL_002c: call class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::Convert(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags, class [mscorlib]System.Type, class [mscorlib]System.Type)
				IL_0031: call class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
				IL_0036: stsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !0>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Site7'

				IL_003b: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !0>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Site7'
				IL_0040: ldfld !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>>::Target
				IL_0045: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !0>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Site7'
				IL_004a: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`7<class [System.Core]System.Runtime.CompilerServices.CallSite, class IDapperAsyncImplementor, class IDbConnection, object, class IDbTransaction, valuetype [mscorlib]System.Nullable`1<int32>, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Site8'
				IL_004f: brtrue.s IL_00c9

				IL_0051: ldc.i4.0
				IL_0052: ldstr "GetAsync"
				IL_0057: ldc.i4.1
				IL_0058: newarr [mscorlib]System.Type
				IL_005d: stloc.s 4
				IL_005f: ldloc.s 4
				IL_0061: ldc.i4.0
				IL_0062: ldtoken !T
				IL_0067: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
				IL_006c: stelem.ref
				IL_006d: ldloc.s 4
				IL_006f: ldtoken Issue3320
				IL_0074: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
				IL_0079: ldc.i4.5
				IL_007a: newarr [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
				IL_007f: stloc.s 5
				IL_0081: ldloc.s 5
				IL_0083: ldc.i4.0
				IL_0084: ldc.i4.1
				IL_0085: ldnull
				IL_0086: call class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags, string)
				IL_008b: stelem.ref
				IL_008c: ldloc.s 5
				IL_008e: ldc.i4.1
				IL_008f: ldc.i4.1
				IL_0090: ldnull
				IL_0091: call class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags, string)
				IL_0096: stelem.ref
				IL_0097: ldloc.s 5
				IL_0099: ldc.i4.2
				IL_009a: ldc.i4.0
				IL_009b: ldnull
				IL_009c: call class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags, string)
				IL_00a1: stelem.ref
				IL_00a2: ldloc.s 5
				IL_00a4: ldc.i4.3
				IL_00a5: ldc.i4.1
				IL_00a6: ldnull
				IL_00a7: call class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags, string)
				IL_00ac: stelem.ref
				IL_00ad: ldloc.s 5
				IL_00af: ldc.i4.4
				IL_00b0: ldc.i4.1
				IL_00b1: ldnull
				IL_00b2: call class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags, string)
				IL_00b7: stelem.ref
				IL_00b8: ldloc.s 5
				IL_00ba: call class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags, string, class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>, class [mscorlib]System.Type, class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
				IL_00bf: call class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`7<class [System.Core]System.Runtime.CompilerServices.CallSite, class IDapperAsyncImplementor, class IDbConnection, object, class IDbTransaction, valuetype [mscorlib]System.Nullable`1<int32>, object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
				IL_00c4: stsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`7<class [System.Core]System.Runtime.CompilerServices.CallSite, class IDapperAsyncImplementor, class IDbConnection, object, class IDbTransaction, valuetype [mscorlib]System.Nullable`1<int32>, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Site8'

				IL_00c9: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`7<class [System.Core]System.Runtime.CompilerServices.CallSite, class IDapperAsyncImplementor, class IDbConnection, object, class IDbTransaction, valuetype [mscorlib]System.Nullable`1<int32>, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Site8'
				IL_00ce: ldfld !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`7<class [System.Core]System.Runtime.CompilerServices.CallSite, class IDapperAsyncImplementor, class IDbConnection, object, class IDbTransaction, valuetype [mscorlib]System.Nullable`1<int32>, object>>::Target
				IL_00d3: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`7<class [System.Core]System.Runtime.CompilerServices.CallSite, class IDapperAsyncImplementor, class IDbConnection, object, class IDbTransaction, valuetype [mscorlib]System.Nullable`1<int32>, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Site8'
				IL_00d8: call class IDapperAsyncImplementor Issue3320::get_Instance()
				IL_00dd: ldarg.0
				IL_00de: ldfld class IDbConnection valuetype Issue3320/'<GetAsync>d__d`1'<!T>::connection
				IL_00e3: ldarg.0
				IL_00e4: ldfld object valuetype Issue3320/'<GetAsync>d__d`1'<!T>::id
				IL_00e9: ldarg.0
				IL_00ea: ldfld class IDbTransaction valuetype Issue3320/'<GetAsync>d__d`1'<!T>::transaction
				IL_00ef: ldarg.0
				IL_00f0: ldfld valuetype [mscorlib]System.Nullable`1<int32> valuetype Issue3320/'<GetAsync>d__d`1'<!T>::commandTimeout
				IL_00f5: callvirt instance !6 class [mscorlib]System.Func`7<class [System.Core]System.Runtime.CompilerServices.CallSite, class IDapperAsyncImplementor, class IDbConnection, object, class IDbTransaction, valuetype [mscorlib]System.Nullable`1<int32>, object>::Invoke(!0, !1, !2, !3, !4, !5)
				IL_00fa: stloc.s 6
				IL_00fc: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Site9'
				IL_0101: brtrue.s IL_0138

				IL_0103: ldc.i4.0
				IL_0104: ldstr "GetAwaiter"
				IL_0109: ldnull
				IL_010a: ldtoken Issue3320
				IL_010f: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
				IL_0114: ldc.i4.1
				IL_0115: newarr [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
				IL_011a: stloc.s 7
				IL_011c: ldloc.s 7
				IL_011e: ldc.i4.0
				IL_011f: ldc.i4.0
				IL_0120: ldnull
				IL_0121: call class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags, string)
				IL_0126: stelem.ref
				IL_0127: ldloc.s 7
				IL_0129: call class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags, string, class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>, class [mscorlib]System.Type, class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
				IL_012e: call class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
				IL_0133: stsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Site9'

				IL_0138: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Site9'
				IL_013d: ldfld !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>>::Target
				IL_0142: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Site9'
				IL_0147: ldloc.s 6
				IL_0149: callvirt instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>::Invoke(!0, !1)
				IL_014e: stloc.s 8
				IL_0150: ldloc.s 8
				IL_0152: stloc.s 9
				IL_0154: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, bool>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Sitea'
				IL_0159: brtrue.s IL_017f

				IL_015b: ldc.i4.0
				IL_015c: ldtoken [mscorlib]System.Boolean
				IL_0161: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
				IL_0166: ldtoken Issue3320
				IL_016b: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
				IL_0170: call class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::Convert(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags, class [mscorlib]System.Type, class [mscorlib]System.Type)
				IL_0175: call class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, bool>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
				IL_017a: stsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, bool>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Sitea'

				IL_017f: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, bool>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Sitea'
				IL_0184: ldfld !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, bool>>::Target
				IL_0189: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, bool>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Sitea'
				IL_018e: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Siteb'
				IL_0193: brtrue.s IL_01c9

				IL_0195: ldc.i4.0
				IL_0196: ldstr "IsCompleted"
				IL_019b: ldtoken Issue3320
				IL_01a0: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
				IL_01a5: ldc.i4.1
				IL_01a6: newarr [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
				IL_01ab: stloc.s 10
				IL_01ad: ldloc.s 10
				IL_01af: ldc.i4.0
				IL_01b0: ldc.i4.0
				IL_01b1: ldnull
				IL_01b2: call class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags, string)
				IL_01b7: stelem.ref
				IL_01b8: ldloc.s 10
				IL_01ba: call class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags, string, class [mscorlib]System.Type, class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
				IL_01bf: call class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
				IL_01c4: stsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Siteb'

				IL_01c9: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Siteb'
				IL_01ce: ldfld !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>>::Target
				IL_01d3: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Siteb'
				IL_01d8: ldloc.s 9
				IL_01da: callvirt instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>::Invoke(!0, !1)
				IL_01df: callvirt instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, bool>::Invoke(!0, !1)
				IL_01e4: brtrue IL_0287

				IL_01e9: newobj instance void class [mscorlib]System.Tuple`2<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>, class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>>>::.ctor(!0, !1)
				IL_01ee: stloc.s 11
				IL_01f0: ldarg.0
				IL_01f1: ldloc.s 11
				IL_01f3: stfld object valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>t__stack'
				IL_01f8: ldarg.0
				IL_01f9: ldc.i4.0
				IL_01fa: stfld int32 valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>1__state'
				IL_01ff: ldarg.0
				IL_0200: ldloc.s 8
				IL_0202: stfld object valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>u__$awaitere'
				IL_0207: ldloc.s 8
				IL_0209: stloc.s 12
				IL_020b: ldloc.s 12
				IL_020d: isinst [mscorlib]System.Runtime.CompilerServices.ICriticalNotifyCompletion
				IL_0212: stloc.s 13
				IL_0214: ldloc.s 13
				IL_0216: brtrue.s IL_0235

				IL_0218: ldloc.s 8
				IL_021a: stloc.s 14
				IL_021c: ldloc.s 14
				IL_021e: castclass [mscorlib]System.Runtime.CompilerServices.INotifyCompletion
				IL_0223: stloc.s 15
				IL_0225: ldarg.0
				IL_0226: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>t__builder'
				IL_022b: ldloca.s 15
				IL_022d: ldarg.0
				IL_022e: call instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!T>::AwaitOnCompleted<class [mscorlib]System.Runtime.CompilerServices.INotifyCompletion, valuetype Issue3320/'<GetAsync>d__d`1'<!T>>(!!0&, !!1&)
				IL_0233: br.s IL_0243

				IL_0235: ldarg.0
				IL_0236: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>t__builder'
				IL_023b: ldloca.s 13
				IL_023d: ldarg.0
				IL_023e: call instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!T>::AwaitUnsafeOnCompleted<class [mscorlib]System.Runtime.CompilerServices.ICriticalNotifyCompletion, valuetype Issue3320/'<GetAsync>d__d`1'<!T>>(!!0&, !!1&)

				IL_0243: ldc.i4.0
				IL_0244: stloc.0
				IL_0245: leave IL_0313

				IL_024a: ldarg.0
				IL_024b: ldfld object valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>t__stack'
				IL_0250: unbox.any class [mscorlib]System.Tuple`2<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>, class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>>>
				IL_0255: stloc.s 16
				IL_0257: ldloc.s 16
				IL_0259: callvirt instance !0 class [mscorlib]System.Tuple`2<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>, class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>>>::get_Item1()
				IL_025e: ldloc.s 16
				IL_0260: callvirt instance !1 class [mscorlib]System.Tuple`2<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>, class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>>>::get_Item2()
				IL_0265: ldarg.0
				IL_0266: ldnull
				IL_0267: stfld object valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>t__stack'
				IL_026c: ldarg.0
				IL_026d: ldfld object valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>u__$awaitere'
				IL_0272: unbox.any [mscorlib]System.Object
				IL_0277: stloc.s 8
				IL_0279: ldarg.0
				IL_027a: ldnull
				IL_027b: stfld object valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>u__$awaitere'
				IL_0280: ldarg.0
				IL_0281: ldc.i4.m1
				IL_0282: stfld int32 valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>1__state'

				IL_0287: ldloc.s 8
				IL_0289: stloc.s 17
				IL_028b: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Sitec'
				IL_0290: brtrue.s IL_02c7

				IL_0292: ldc.i4.0
				IL_0293: ldstr "GetResult"
				IL_0298: ldnull
				IL_0299: ldtoken Issue3320
				IL_029e: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
				IL_02a3: ldc.i4.1
				IL_02a4: newarr [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
				IL_02a9: stloc.s 18
				IL_02ab: ldloc.s 18
				IL_02ad: ldc.i4.0
				IL_02ae: ldc.i4.0
				IL_02af: ldnull
				IL_02b0: call class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags, string)
				IL_02b5: stelem.ref
				IL_02b6: ldloc.s 18
				IL_02b8: call class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags, string, class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>, class [mscorlib]System.Type, class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
				IL_02bd: call class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
				IL_02c2: stsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Sitec'

				IL_02c7: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Sitec'
				IL_02cc: ldfld !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>>::Target
				IL_02d1: ldsfld class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>> class Issue3320/'<GetAsync>o__SiteContainer6`1'<!T>::'<>p__Sitec'
				IL_02d6: ldloc.s 17
				IL_02d8: callvirt instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, object>::Invoke(!0, !1)
				IL_02dd: ldnull
				IL_02de: stloc.s 8
				IL_02e0: callvirt instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite, object, !T>::Invoke(!0, !1)
				IL_02e5: stloc.1
				IL_02e6: leave.s IL_02ff
			} // end .try
			catch [mscorlib]System.Exception
			{
				IL_02e8: stloc.2
				IL_02e9: ldarg.0
				IL_02ea: ldc.i4.s -2
				IL_02ec: stfld int32 valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>1__state'
				IL_02f1: ldarg.0
				IL_02f2: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>t__builder'
				IL_02f7: ldloc.2
				IL_02f8: call instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!T>::SetException(class [mscorlib]System.Exception)
				IL_02fd: leave.s IL_0313
			} // end handler

			IL_02ff: ldarg.0
			IL_0300: ldc.i4.s -2
			IL_0302: stfld int32 valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>1__state'
			IL_0307: ldarg.0
			IL_0308: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>t__builder'
			IL_030d: ldloc.1
			IL_030e: call instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!T>::SetResult(!0)

			IL_0313: ret
		} // end of method '<GetAsync>d__d`1'::MoveNext

		.method private final hidebysig newslot virtual 
			instance void SetStateMachine (
				class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine param0
			) cil managed 
		{
			.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
				01 00 00 00
			)
			.override method instance void [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
			// Method begins at RVA 0x74c4
			// Header size: 1
			// Code size: 13 (0xd)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype Issue3320/'<GetAsync>d__d`1'<!T>::'<>t__builder'
			IL_0006: ldarg.1
			IL_0007: call instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!T>::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
			IL_000c: ret
		} // end of method '<GetAsync>d__d`1'::SetStateMachine

	} // end of class <GetAsync>d__d`1


	.method public hidebysig static 
		class [mscorlib]System.Threading.Tasks.Task`1<!!T> GetAsync<class T> (
			class IDbConnection connection,
			object id,
			[opt] class IDbTransaction transaction,
			[opt] valuetype [mscorlib]System.Nullable`1<int32> commandTimeout
		) cil managed 
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = (
			01 00 00 00
		)
		.custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = (
			01 00 37 44 61 70 70 65 72 45 78 74 65 6e 73 69
			6f 6e 73 2e 44 61 70 70 65 72 41 73 79 6e 63 45
			78 74 65 6e 73 69 6f 6e 73 2b 3c 47 65 74 41 73
			79 6e 63 3e 64 5f 5f 64 60 31 00 00
		)
		.custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = (
			01 00 00 00
		)
		.param [2]
			.custom instance void [System.Core]System.Runtime.CompilerServices.DynamicAttribute::.ctor() = (
				01 00 00 00
			)
		.param [3] = nullref
		.param [4] = nullref
		// Method begins at RVA 0x74d4
		// Header size: 12
		// Code size: 82 (0x52)
		.maxstack 2
		.locals init (
			[0] valuetype Issue3320/'<GetAsync>d__d`1'<!!T>,
			[1] valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!!T>
		)

		IL_0000: ldloca.s 0
		IL_0002: ldarg.0
		IL_0003: stfld class IDbConnection valuetype Issue3320/'<GetAsync>d__d`1'<!!T>::connection
		IL_0008: ldloca.s 0
		IL_000a: ldarg.1
		IL_000b: stfld object valuetype Issue3320/'<GetAsync>d__d`1'<!!T>::id
		IL_0010: ldloca.s 0
		IL_0012: ldarg.2
		IL_0013: stfld class IDbTransaction valuetype Issue3320/'<GetAsync>d__d`1'<!!T>::transaction
		IL_0018: ldloca.s 0
		IL_001a: ldarg.3
		IL_001b: stfld valuetype [mscorlib]System.Nullable`1<int32> valuetype Issue3320/'<GetAsync>d__d`1'<!!T>::commandTimeout
		IL_0020: ldloca.s 0
		IL_0022: call valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!!T>::Create()
		IL_0027: stfld valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype Issue3320/'<GetAsync>d__d`1'<!!T>::'<>t__builder'
		IL_002c: ldloca.s 0
		IL_002e: ldc.i4.m1
		IL_002f: stfld int32 valuetype Issue3320/'<GetAsync>d__d`1'<!!T>::'<>1__state'
		IL_0034: ldloca.s 0
		IL_0036: ldfld valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype Issue3320/'<GetAsync>d__d`1'<!!T>::'<>t__builder'
		IL_003b: stloc.1
		IL_003c: ldloca.s 1
		IL_003e: ldloca.s 0
		IL_0040: call instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!!T>::Start<valuetype Issue3320/'<GetAsync>d__d`1'<!!T>>(!!0&)
		IL_0045: ldloca.s 0
		IL_0047: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype Issue3320/'<GetAsync>d__d`1'<!!T>::'<>t__builder'
		IL_004c: call instance class [mscorlib]System.Threading.Tasks.Task`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!!T>::get_Task()
		IL_0051: ret
	} // end of method Issue3320::GetAsync
}
