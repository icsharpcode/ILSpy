



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly TupleTests
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module TupleTests.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests
       extends [mscorlib]System.Object
{
  .class abstract auto ansi nested private beforefieldinit OverloadResolution
         extends [mscorlib]System.Object
  {
    .method public hidebysig newslot abstract virtual 
            instance void  M1(valuetype [mscorlib]System.ValueTuple`2<int64,int64> a) cil managed
    {
    } // end of method OverloadResolution::M1

    .method public hidebysig newslot abstract virtual 
            instance void  M1(object a) cil managed
    {
    } // end of method OverloadResolution::M1

    .method public hidebysig instance void 
            UseM1(valuetype [mscorlib]System.ValueTuple`2<int32,int32> a) cil managed
    {
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  box        valuetype [mscorlib]System.ValueTuple`2<int32,int32>
      IL_0008:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/OverloadResolution::M1(object)
      IL_000d:  nop
      IL_000e:  ret
    } // end of method OverloadResolution::UseM1

    .method family hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method OverloadResolution::.ctor

  } // end of class OverloadResolution

  .class auto ansi serializable sealed nested private beforefieldinit '<>c'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static initonly class ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c' '<>9'
    .field public static class [mscorlib]System.Func`2<valuetype [mscorlib]System.ValueTuple`2<int32,string>,bool> '<>9__22_0'
    .field public static class [mscorlib]System.Action '<>9__48_0'
    .field public static class [mscorlib]System.Action '<>9__50_0'
    .method private hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c'::.ctor()
      IL_0005:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c'::'<>9'
      IL_000a:  ret
    } // end of method '<>c'::.cctor

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<>c'::.ctor

    .method assembly hidebysig instance bool 
            '<get_HasItems>b__22_0'(valuetype [mscorlib]System.ValueTuple`2<int32,string> a) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      !0 valuetype [mscorlib]System.ValueTuple`2<int32,string>::Item1
      IL_0006:  ldc.i4.0
      IL_0007:  cgt
      IL_0009:  ret
    } // end of method '<>c'::'<get_HasItems>b__22_0'

    .method assembly hidebysig instance void 
            '<get_TargetTyping>b__48_0'() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method '<>c'::'<get_TargetTyping>b__48_0'

    .method assembly hidebysig instance void 
            '<get_NotTargetTyping>b__50_0'() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method '<>c'::'<get_NotTargetTyping>b__50_0'

  } // end of class '<>c'

  .field public valuetype [mscorlib]System.ValueTuple VT0
  .field public valuetype [mscorlib]System.ValueTuple`1<int32> VT1
  .field public valuetype [mscorlib]System.ValueTuple`8<int32,int32,int32,int32,int32,int32,int32,valuetype [mscorlib]System.ValueTuple> VT7EmptyRest
  .field public valuetype [mscorlib]System.ValueTuple`2<int32,uint32> Unnamed2
  .field public valuetype [mscorlib]System.ValueTuple`3<int32,int32,int32> Unnamed3
  .field public valuetype [mscorlib]System.ValueTuple`4<int32,int32,int32,int32> Unnamed4
  .field public valuetype [mscorlib]System.ValueTuple`5<int32,int32,int32,int32,int32> Unnamed5
  .field public valuetype [mscorlib]System.ValueTuple`6<int32,int32,int32,int32,int32,int32> Unnamed6
  .field public valuetype [mscorlib]System.ValueTuple`7<int32,int32,int32,int32,int32,int32,int32> Unnamed7
  .field public valuetype [mscorlib]System.ValueTuple`8<int32,int32,int32,int32,int32,int32,int32,valuetype [mscorlib]System.ValueTuple`1<int32>> Unnamed8
  .field public valuetype [mscorlib]System.ValueTuple`2<int32,uint32> Named2
  .custom instance void [mscorlib]System.Runtime.CompilerServices.TupleElementNamesAttribute::.ctor(string[]) = ( 01 00 02 00 00 00 01 61 01 62 00 00 )             // .......a.b..
  .field public valuetype [mscorlib]System.ValueTuple`2<int32,uint32>[] Named2Array
  .custom instance void [mscorlib]System.Runtime.CompilerServices.TupleElementNamesAttribute::.ctor(string[]) = ( 01 00 02 00 00 00 01 61 01 62 00 00 )             // .......a.b..
  .field public valuetype [mscorlib]System.ValueTuple`8<int32,int32,int32,int32,int32,int32,int32,valuetype [mscorlib]System.ValueTuple`1<int32>> Named8
  .custom instance void [mscorlib]System.Runtime.CompilerServices.TupleElementNamesAttribute::.ctor(string[]) = ( 01 00 09 00 00 00 01 61 01 62 01 63 01 64 01 65   // .......a.b.c.d.e
                                                                                                                  01 66 01 67 01 68 FF 00 00 )                      // .f.g.h...
  .field public valuetype [mscorlib]System.ValueTuple`5<int32,int32,int32,int32,int32> PartiallyNamed
  .custom instance void [mscorlib]System.Runtime.CompilerServices.TupleElementNamesAttribute::.ctor(string[]) = ( 01 00 05 00 00 00 FF 01 61 FF 01 62 FF 00 00 )    // ........a..b...
  .field public valuetype [mscorlib]System.ValueTuple`3<valuetype [mscorlib]System.ValueTuple`2<int32,int32>,valuetype [mscorlib]System.ValueTuple`2<int32,int32>,valuetype [mscorlib]System.ValueTuple`2<int32,int32>> Nested1
  .custom instance void [mscorlib]System.Runtime.CompilerServices.TupleElementNamesAttribute::.ctor(string[]) = ( 01 00 09 00 00 00 01 78 01 79 01 7A 01 61 01 62   // .......x.y.z.a.b
                                                                                                                  FF FF 01 63 01 64 00 00 )                         // ...c.d..
  .field public valuetype [mscorlib]System.ValueTuple`3<valuetype [mscorlib]System.ValueTuple`2<object,object>,object,valuetype [mscorlib]System.ValueTuple`2<object,object>> Nested2
  .custom instance void [System.Core]System.Runtime.CompilerServices.DynamicAttribute::.ctor(bool[]) = ( 01 00 08 00 00 00 00 00 00 01 01 00 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.TupleElementNamesAttribute::.ctor(string[]) = ( 01 00 07 00 00 00 FF FF FF 01 61 01 62 01 63 01   // ..........a.b.c.
                                                                                                                  64 00 00 )                                        // d..
  .field public valuetype [mscorlib]System.ValueTuple`5<valuetype [mscorlib]System.ValueTuple,valuetype [mscorlib]System.ValueTuple`2<int32,int32>,valuetype [mscorlib]System.ValueTuple`1<int32>,valuetype [mscorlib]System.ValueTuple`2<int32,int32>,valuetype [mscorlib]System.ValueTuple`2<int32,int32>> Nested3
  .custom instance void [mscorlib]System.Runtime.CompilerServices.TupleElementNamesAttribute::.ctor(string[]) = ( 01 00 0C 00 00 00 01 61 FF 01 62 FF 01 63 02 78   // .......a..b..c.x
                                                                                                                  31 02 78 32 FF 02 79 31 02 79 32 FF FF 00 00 )    // 1.x2..y1.y2....
  .field public valuetype [mscorlib]System.ValueTuple`8<int32,int32,int32,int32,int32,int32,int32,valuetype [mscorlib]System.ValueTuple`2<int32,valuetype [mscorlib]System.ValueTuple`2<int32,int32>>> Nested4
  .custom instance void [mscorlib]System.Runtime.CompilerServices.TupleElementNamesAttribute::.ctor(string[]) = ( 01 00 0D 00 00 00 01 61 01 62 01 63 01 64 01 65   // .......a.b.c.d.e
                                                                                                                  01 66 01 67 01 68 FF FF FF 01 69 01 6A 00 00 )    // .f.g.h....i.j..
  .field public class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype [mscorlib]System.ValueTuple`2<int32,string>,valuetype [mscorlib]System.ValueTuple`2<string,int32>> TupleDict
  .custom instance void [mscorlib]System.Runtime.CompilerServices.TupleElementNamesAttribute::.ctor(string[]) = ( 01 00 04 00 00 00 01 61 01 62 01 63 01 64 00 00 ) // .......a.b.c.d..
  .field public class [mscorlib]System.Collections.Generic.List`1<valuetype [mscorlib]System.ValueTuple`2<int32,string>> List
  .method public hidebysig specialname instance bool 
          get_HasItems() cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype [mscorlib]System.ValueTuple`2<int32,string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::List
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<valuetype [mscorlib]System.ValueTuple`2<int32,string>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c'::'<>9__22_0'
    IL_000b:  dup
    IL_000c:  brtrue.s   IL_0025

    IL_000e:  pop
    IL_000f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c'::'<>9'
    IL_0014:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c'::'<get_HasItems>b__22_0'(valuetype [mscorlib]System.ValueTuple`2<int32,string>)
    IL_001a:  newobj     instance void class [mscorlib]System.Func`2<valuetype [mscorlib]System.ValueTuple`2<int32,string>,bool>::.ctor(object,
                                                                                                                                        native int)
    IL_001f:  dup
    IL_0020:  stsfld     class [mscorlib]System.Func`2<valuetype [mscorlib]System.ValueTuple`2<int32,string>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c'::'<>9__22_0'
    IL_0025:  call       bool [System.Core]System.Linq.Enumerable::Any<valuetype [mscorlib]System.ValueTuple`2<int32,string>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                              class [mscorlib]System.Func`2<!!0,bool>)
    IL_002a:  ret
  } // end of method TupleTests::get_HasItems

  .method public hidebysig specialname instance int32 
          get_VT1Member() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldflda     valuetype [mscorlib]System.ValueTuple`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::VT1
    IL_0006:  ldfld      !0 valuetype [mscorlib]System.ValueTuple`1<int32>::Item1
    IL_000b:  ret
  } // end of method TupleTests::get_VT1Member

  .method public hidebysig specialname instance int32 
          get_AccessUnnamed8() cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldflda     valuetype [mscorlib]System.ValueTuple`8<int32,int32,int32,int32,int32,int32,int32,valuetype [mscorlib]System.ValueTuple`1<int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::Unnamed8
    IL_0006:  ldflda     !7 valuetype [mscorlib]System.ValueTuple`8<int32,int32,int32,int32,int32,int32,int32,valuetype [mscorlib]System.ValueTuple`1<int32>>::Rest
    IL_000b:  ldfld      !0 valuetype [mscorlib]System.ValueTuple`1<int32>::Item1
    IL_0010:  ret
  } // end of method TupleTests::get_AccessUnnamed8

  .method public hidebysig specialname instance int32 
          get_AccessNamed8() cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldflda     valuetype [mscorlib]System.ValueTuple`8<int32,int32,int32,int32,int32,int32,int32,valuetype [mscorlib]System.ValueTuple`1<int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::Named8
    IL_0006:  ldflda     !7 valuetype [mscorlib]System.ValueTuple`8<int32,int32,int32,int32,int32,int32,int32,valuetype [mscorlib]System.ValueTuple`1<int32>>::Rest
    IL_000b:  ldfld      !0 valuetype [mscorlib]System.ValueTuple`1<int32>::Item1
    IL_0010:  ret
  } // end of method TupleTests::get_AccessNamed8

  .method public hidebysig specialname instance int32 
          get_AccessPartiallyNamed() cil managed
  {
    // Code size       24 (0x18)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldflda     valuetype [mscorlib]System.ValueTuple`5<int32,int32,int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::PartiallyNamed
    IL_0006:  ldfld      !1 valuetype [mscorlib]System.ValueTuple`5<int32,int32,int32,int32,int32>::Item2
    IL_000b:  ldarg.0
    IL_000c:  ldflda     valuetype [mscorlib]System.ValueTuple`5<int32,int32,int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::PartiallyNamed
    IL_0011:  ldfld      !2 valuetype [mscorlib]System.ValueTuple`5<int32,int32,int32,int32,int32>::Item3
    IL_0016:  add
    IL_0017:  ret
  } // end of method TupleTests::get_AccessPartiallyNamed

  .method public hidebysig specialname instance valuetype [mscorlib]System.ValueTuple`1<int32> 
          get_NewTuple1() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  newobj     instance void valuetype [mscorlib]System.ValueTuple`1<int32>::.ctor(!0)
    IL_0006:  ret
  } // end of method TupleTests::get_NewTuple1

  .method public hidebysig specialname instance valuetype [mscorlib]System.ValueTuple`2<int32,int32> 
          get_NewTuple2() cil managed
  {
    .param [0]
    .custom instance void [mscorlib]System.Runtime.CompilerServices.TupleElementNamesAttribute::.ctor(string[]) = ( 01 00 02 00 00 00 01 61 01 62 00 00 )             // .......a.b..
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  newobj     instance void valuetype [mscorlib]System.ValueTuple`2<int32,int32>::.ctor(!0,
                                                                                                   !1)
    IL_0007:  ret
  } // end of method TupleTests::get_NewTuple2

  .method public hidebysig specialname instance object 
          get_BoxedTuple10() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  10
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  ldc.i4.4
    IL_0004:  ldc.i4.5
    IL_0005:  ldc.i4.6
    IL_0006:  ldc.i4.7
    IL_0007:  ldc.i4.8
    IL_0008:  ldc.i4.s   9
    IL_000a:  ldc.i4.s   10
    IL_000c:  newobj     instance void valuetype [mscorlib]System.ValueTuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                         !1,
                                                                                                         !2)
    IL_0011:  newobj     instance void valuetype [mscorlib]System.ValueTuple`8<int32,int32,int32,int32,int32,int32,int32,valuetype [mscorlib]System.ValueTuple`3<int32,int32,int32>>::.ctor(!0,
                                                                                                                                                                                            !1,
                                                                                                                                                                                            !2,
                                                                                                                                                                                            !3,
                                                                                                                                                                                            !4,
                                                                                                                                                                                            !5,
                                                                                                                                                                                            !6,
                                                                                                                                                                                            !7)
    IL_0016:  box        valuetype [mscorlib]System.ValueTuple`8<int32,int32,int32,int32,int32,int32,int32,valuetype [mscorlib]System.ValueTuple`3<int32,int32,int32>>
    IL_001b:  ret
  } // end of method TupleTests::get_BoxedTuple10

  .method public hidebysig specialname instance valuetype [mscorlib]System.ValueTuple`2<uint32,int32> 
          get_SwapUnnamed() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldflda     valuetype [mscorlib]System.ValueTuple`2<int32,uint32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::Unnamed2
    IL_0006:  ldfld      !1 valuetype [mscorlib]System.ValueTuple`2<int32,uint32>::Item2
    IL_000b:  ldarg.0
    IL_000c:  ldflda     valuetype [mscorlib]System.ValueTuple`2<int32,uint32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::Unnamed2
    IL_0011:  ldfld      !0 valuetype [mscorlib]System.ValueTuple`2<int32,uint32>::Item1
    IL_0016:  newobj     instance void valuetype [mscorlib]System.ValueTuple`2<uint32,int32>::.ctor(!0,
                                                                                                    !1)
    IL_001b:  ret
  } // end of method TupleTests::get_SwapUnnamed

  .method public hidebysig specialname instance valuetype [mscorlib]System.ValueTuple`2<uint32,int32> 
          get_SwapNamed2() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldflda     valuetype [mscorlib]System.ValueTuple`2<int32,uint32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::Named2
    IL_0006:  ldfld      !1 valuetype [mscorlib]System.ValueTuple`2<int32,uint32>::Item2
    IL_000b:  ldarg.0
    IL_000c:  ldflda     valuetype [mscorlib]System.ValueTuple`2<int32,uint32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::Named2
    IL_0011:  ldfld      !0 valuetype [mscorlib]System.ValueTuple`2<int32,uint32>::Item1
    IL_0016:  newobj     instance void valuetype [mscorlib]System.ValueTuple`2<uint32,int32>::.ctor(!0,
                                                                                                    !1)
    IL_001b:  ret
  } // end of method TupleTests::get_SwapNamed2

  .method public hidebysig specialname instance int32 
          get_TupleHash() cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init (valuetype [mscorlib]System.ValueTuple`3<int32,int32,int32> V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  newobj     instance void valuetype [mscorlib]System.ValueTuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                         !1,
                                                                                                         !2)
    IL_0008:  stloc.0
    IL_0009:  ldloca.s   V_0
    IL_000b:  constrained. valuetype [mscorlib]System.ValueTuple`3<int32,int32,int32>
    IL_0011:  callvirt   instance int32 [mscorlib]System.Object::GetHashCode()
    IL_0016:  ret
  } // end of method TupleTests::get_TupleHash

  .method public hidebysig specialname instance int32 
          get_TupleHash2() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldflda     valuetype [mscorlib]System.ValueTuple`2<int32,uint32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::Named2
    IL_0006:  constrained. valuetype [mscorlib]System.ValueTuple`2<int32,uint32>
    IL_000c:  callvirt   instance int32 [mscorlib]System.Object::GetHashCode()
    IL_0011:  ret
  } // end of method TupleTests::get_TupleHash2

  .method public hidebysig specialname instance valuetype [mscorlib]System.ValueTuple`2<int32,int32> 
          get_AccessRest() cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  9
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  ldc.i4.4
    IL_0004:  ldc.i4.5
    IL_0005:  ldc.i4.6
    IL_0006:  ldc.i4.7
    IL_0007:  ldc.i4.8
    IL_0008:  ldc.i4.s   9
    IL_000a:  newobj     instance void valuetype [mscorlib]System.ValueTuple`2<int32,int32>::.ctor(!0,
                                                                                                   !1)
    IL_000f:  newobj     instance void valuetype [mscorlib]System.ValueTuple`8<int32,int32,int32,int32,int32,int32,int32,valuetype [mscorlib]System.ValueTuple`2<int32,int32>>::.ctor(!0,
                                                                                                                                                                                      !1,
                                                                                                                                                                                      !2,
                                                                                                                                                                                      !3,
                                                                                                                                                                                      !4,
                                                                                                                                                                                      !5,
                                                                                                                                                                                      !6,
                                                                                                                                                                                      !7)
    IL_0014:  ldfld      !7 valuetype [mscorlib]System.ValueTuple`8<int32,int32,int32,int32,int32,int32,int32,valuetype [mscorlib]System.ValueTuple`2<int32,int32>>::Rest
    IL_0019:  ret
  } // end of method TupleTests::get_AccessRest

  .method public hidebysig specialname instance valuetype [mscorlib]System.ValueTuple`3<string,object,class [mscorlib]System.Action> 
          get_TargetTyping() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldc.i4.1
    IL_0002:  box        [mscorlib]System.Int32
    IL_0007:  ldsfld     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c'::'<>9__48_0'
    IL_000c:  dup
    IL_000d:  brtrue.s   IL_0026

    IL_000f:  pop
    IL_0010:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c'::'<>9'
    IL_0015:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c'::'<get_TargetTyping>b__48_0'()
    IL_001b:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                      native int)
    IL_0020:  dup
    IL_0021:  stsfld     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c'::'<>9__48_0'
    IL_0026:  newobj     instance void valuetype [mscorlib]System.ValueTuple`3<string,object,class [mscorlib]System.Action>::.ctor(!0,
                                                                                                                                   !1,
                                                                                                                                   !2)
    IL_002b:  ret
  } // end of method TupleTests::get_TargetTyping

  .method public hidebysig specialname instance object 
          get_NotTargetTyping() cil managed
  {
    // Code size       49 (0x31)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldc.i4.1
    IL_0002:  box        [mscorlib]System.Int32
    IL_0007:  ldsfld     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c'::'<>9__50_0'
    IL_000c:  dup
    IL_000d:  brtrue.s   IL_0026

    IL_000f:  pop
    IL_0010:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c'::'<>9'
    IL_0015:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c'::'<get_NotTargetTyping>b__50_0'()
    IL_001b:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                      native int)
    IL_0020:  dup
    IL_0021:  stsfld     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests/'<>c'::'<>9__50_0'
    IL_0026:  newobj     instance void valuetype [mscorlib]System.ValueTuple`3<string,object,class [mscorlib]System.Action>::.ctor(!0,
                                                                                                                                   !1,
                                                                                                                                   !2)
    IL_002b:  box        valuetype [mscorlib]System.ValueTuple`3<string,object,class [mscorlib]System.Action>
    IL_0030:  ret
  } // end of method TupleTests::get_NotTargetTyping

  .method public hidebysig instance void 
          UseDict() cil managed
  {
    // Code size       109 (0x6d)
    .maxstack  3
    .locals init (string V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype [mscorlib]System.ValueTuple`2<int32,string>,valuetype [mscorlib]System.ValueTuple`2<string,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::TupleDict
    IL_0007:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype [mscorlib]System.ValueTuple`2<int32,string>,valuetype [mscorlib]System.ValueTuple`2<string,int32>>::get_Count()
    IL_000c:  ldc.i4.s   10
    IL_000e:  cgt
    IL_0010:  stloc.1
    IL_0011:  ldloc.1
    IL_0012:  brfalse.s  IL_0022

    IL_0014:  nop
    IL_0015:  ldarg.0
    IL_0016:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype [mscorlib]System.ValueTuple`2<int32,string>,valuetype [mscorlib]System.ValueTuple`2<string,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::TupleDict
    IL_001b:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype [mscorlib]System.ValueTuple`2<int32,string>,valuetype [mscorlib]System.ValueTuple`2<string,int32>>::Clear()
    IL_0020:  nop
    IL_0021:  nop
    IL_0022:  ldarg.0
    IL_0023:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype [mscorlib]System.ValueTuple`2<int32,string>,valuetype [mscorlib]System.ValueTuple`2<string,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::TupleDict
    IL_0028:  ldc.i4.1
    IL_0029:  ldstr      "abc"
    IL_002e:  newobj     instance void valuetype [mscorlib]System.ValueTuple`2<int32,string>::.ctor(!0,
                                                                                                    !1)
    IL_0033:  callvirt   instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype [mscorlib]System.ValueTuple`2<int32,string>,valuetype [mscorlib]System.ValueTuple`2<string,int32>>::get_Item(!0)
    IL_0038:  ldfld      !0 valuetype [mscorlib]System.ValueTuple`2<string,int32>::Item1
    IL_003d:  stloc.0
    IL_003e:  ldloc.0
    IL_003f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0044:  nop
    IL_0045:  ldloc.0
    IL_0046:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004b:  nop
    IL_004c:  ldarg.0
    IL_004d:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype [mscorlib]System.ValueTuple`2<int32,string>,valuetype [mscorlib]System.ValueTuple`2<string,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::TupleDict
    IL_0052:  callvirt   instance class [mscorlib]System.Collections.Generic.Dictionary`2/ValueCollection<!0,!1> class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype [mscorlib]System.ValueTuple`2<int32,string>,valuetype [mscorlib]System.ValueTuple`2<string,int32>>::get_Values()
    IL_0057:  call       class [mscorlib]System.Collections.Generic.List`1<!!0> [System.Core]System.Linq.Enumerable::ToList<valuetype [mscorlib]System.ValueTuple`2<string,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_005c:  call       !!0 [System.Core]System.Linq.Enumerable::First<valuetype [mscorlib]System.ValueTuple`2<string,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0061:  ldfld      !1 valuetype [mscorlib]System.ValueTuple`2<string,int32>::Item2
    IL_0066:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_006b:  nop
    IL_006c:  ret
  } // end of method TupleTests::UseDict

  .method public hidebysig instance void 
          Issue1174() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  3
    .locals init (valuetype [mscorlib]System.ValueTuple`3<int32,int32,int32> V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.2
    IL_0003:  ldc.i4.3
    IL_0004:  newobj     instance void valuetype [mscorlib]System.ValueTuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                         !1,
                                                                                                         !2)
    IL_0009:  stloc.0
    IL_000a:  ldloca.s   V_0
    IL_000c:  constrained. valuetype [mscorlib]System.ValueTuple`3<int32,int32,int32>
    IL_0012:  callvirt   instance int32 [mscorlib]System.Object::GetHashCode()
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001c:  nop
    IL_001d:  ret
  } // end of method TupleTests::Issue1174

  .method public hidebysig instance void 
          LocalVariables(valuetype [mscorlib]System.ValueTuple`2<int32,int32> a) cil managed
  {
    // Code size       76 (0x4c)
    .maxstack  4
    .locals init (valuetype [mscorlib]System.ValueTuple`2<int32,int32> V_0)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  ldarg.1
    IL_0004:  ldfld      !0 valuetype [mscorlib]System.ValueTuple`2<int32,int32>::Item1
    IL_0009:  ldarg.1
    IL_000a:  ldfld      !1 valuetype [mscorlib]System.ValueTuple`2<int32,int32>::Item2
    IL_000f:  add
    IL_0010:  ldarg.1
    IL_0011:  ldfld      !0 valuetype [mscorlib]System.ValueTuple`2<int32,int32>::Item1
    IL_0016:  ldarg.1
    IL_0017:  ldfld      !1 valuetype [mscorlib]System.ValueTuple`2<int32,int32>::Item2
    IL_001c:  mul
    IL_001d:  call       instance void valuetype [mscorlib]System.ValueTuple`2<int32,int32>::.ctor(!0,
                                                                                                   !1)
    IL_0022:  ldloca.s   V_0
    IL_0024:  constrained. valuetype [mscorlib]System.ValueTuple`2<int32,int32>
    IL_002a:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0034:  nop
    IL_0035:  ldloc.0
    IL_0036:  box        valuetype [mscorlib]System.ValueTuple`2<int32,int32>
    IL_003b:  call       instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
    IL_0040:  callvirt   instance string [mscorlib]System.Type::get_FullName()
    IL_0045:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004a:  nop
    IL_004b:  ret
  } // end of method TupleTests::LocalVariables

  .method public hidebysig instance void 
          Foreach(class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.ValueTuple`2<int32,string>> input) cil managed
  {
    // Code size       75 (0x4b)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.IEnumerator`1<valuetype [mscorlib]System.ValueTuple`2<int32,string>> V_0,
             valuetype [mscorlib]System.ValueTuple`2<int32,string> V_1)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.ValueTuple`2<int32,string>>::GetEnumerator()
    IL_0008:  stloc.0
    .try
    {
      IL_0009:  br.s       IL_0035

      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<valuetype [mscorlib]System.ValueTuple`2<int32,string>>::get_Current()
      IL_0011:  stloc.1
      IL_0012:  nop
      IL_0013:  ldstr      "{0}: {1}"
      IL_0018:  ldloc.1
      IL_0019:  ldfld      !0 valuetype [mscorlib]System.ValueTuple`2<int32,string>::Item1
      IL_001e:  box        [mscorlib]System.Int32
      IL_0023:  ldloc.1
      IL_0024:  ldfld      !1 valuetype [mscorlib]System.ValueTuple`2<int32,string>::Item2
      IL_0029:  call       string [mscorlib]System.String::Format(string,
                                                                  object,
                                                                  object)
      IL_002e:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0033:  nop
      IL_0034:  nop
      IL_0035:  ldloc.0
      IL_0036:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_003b:  brtrue.s   IL_000b

      IL_003d:  leave.s    IL_004a

    }  // end .try
    finally
    {
      IL_003f:  ldloc.0
      IL_0040:  brfalse.s  IL_0049

      IL_0042:  ldloc.0
      IL_0043:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0048:  nop
      IL_0049:  endfinally
    }  // end handler
    IL_004a:  ret
  } // end of method TupleTests::Foreach

  .method public hidebysig instance void 
          ForeachNamedElements(class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.ValueTuple`2<int32,string>> input) cil managed
  {
    .param [1]
    .custom instance void [mscorlib]System.Runtime.CompilerServices.TupleElementNamesAttribute::.ctor(string[]) = ( 01 00 02 00 00 00 05 49 6E 64 65 78 04 44 61 74   // .......Index.Dat
                                                                                                                    61 00 00 )                                        // a..
    // Code size       75 (0x4b)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.IEnumerator`1<valuetype [mscorlib]System.ValueTuple`2<int32,string>> V_0,
             valuetype [mscorlib]System.ValueTuple`2<int32,string> V_1)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.ValueTuple`2<int32,string>>::GetEnumerator()
    IL_0008:  stloc.0
    .try
    {
      IL_0009:  br.s       IL_0035

      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<valuetype [mscorlib]System.ValueTuple`2<int32,string>>::get_Current()
      IL_0011:  stloc.1
      IL_0012:  nop
      IL_0013:  ldstr      "{0}: {1}"
      IL_0018:  ldloc.1
      IL_0019:  ldfld      !0 valuetype [mscorlib]System.ValueTuple`2<int32,string>::Item1
      IL_001e:  box        [mscorlib]System.Int32
      IL_0023:  ldloc.1
      IL_0024:  ldfld      !1 valuetype [mscorlib]System.ValueTuple`2<int32,string>::Item2
      IL_0029:  call       string [mscorlib]System.String::Format(string,
                                                                  object,
                                                                  object)
      IL_002e:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0033:  nop
      IL_0034:  nop
      IL_0035:  ldloc.0
      IL_0036:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_003b:  brtrue.s   IL_000b

      IL_003d:  leave.s    IL_004a

    }  // end .try
    finally
    {
      IL_003f:  ldloc.0
      IL_0040:  brfalse.s  IL_0049

      IL_0042:  ldloc.0
      IL_0043:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0048:  nop
      IL_0049:  endfinally
    }  // end handler
    IL_004a:  ret
  } // end of method TupleTests::ForeachNamedElements

  .method public hidebysig instance void 
          NonGenericForeach(class [mscorlib]System.Collections.IEnumerable input) cil managed
  {
    // Code size       87 (0x57)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.IEnumerator V_0,
             valuetype [mscorlib]System.ValueTuple`2<string,int32> V_1,
             class [mscorlib]System.IDisposable V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0008:  stloc.0
    .try
    {
      IL_0009:  br.s       IL_003a

      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0011:  unbox.any  valuetype [mscorlib]System.ValueTuple`2<string,int32>
      IL_0016:  stloc.1
      IL_0017:  nop
      IL_0018:  ldstr      "{0}: {1}"
      IL_001d:  ldloc.1
      IL_001e:  ldfld      !0 valuetype [mscorlib]System.ValueTuple`2<string,int32>::Item1
      IL_0023:  ldloc.1
      IL_0024:  ldfld      !1 valuetype [mscorlib]System.ValueTuple`2<string,int32>::Item2
      IL_0029:  box        [mscorlib]System.Int32
      IL_002e:  call       string [mscorlib]System.String::Format(string,
                                                                  object,
                                                                  object)
      IL_0033:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0038:  nop
      IL_0039:  nop
      IL_003a:  ldloc.0
      IL_003b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0040:  brtrue.s   IL_000b

      IL_0042:  leave.s    IL_0056

    }  // end .try
    finally
    {
      IL_0044:  ldloc.0
      IL_0045:  isinst     [mscorlib]System.IDisposable
      IL_004a:  stloc.2
      IL_004b:  ldloc.2
      IL_004c:  brfalse.s  IL_0055

      IL_004e:  ldloc.2
      IL_004f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0054:  nop
      IL_0055:  endfinally
    }  // end handler
    IL_0056:  ret
  } // end of method TupleTests::NonGenericForeach

  .method public hidebysig instance void 
          CallForeach() cil managed
  {
    // Code size       50 (0x32)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [mscorlib]System.ValueTuple`2<int32,string>>::.ctor()
    IL_0007:  dup
    IL_0008:  ldc.i4.1
    IL_0009:  ldstr      "a"
    IL_000e:  newobj     instance void valuetype [mscorlib]System.ValueTuple`2<int32,string>::.ctor(!0,
                                                                                                    !1)
    IL_0013:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [mscorlib]System.ValueTuple`2<int32,string>>::Add(!0)
    IL_0018:  nop
    IL_0019:  dup
    IL_001a:  ldc.i4.2
    IL_001b:  ldstr      "b"
    IL_0020:  newobj     instance void valuetype [mscorlib]System.ValueTuple`2<int32,string>::.ctor(!0,
                                                                                                    !1)
    IL_0025:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [mscorlib]System.ValueTuple`2<int32,string>>::Add(!0)
    IL_002a:  nop
    IL_002b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::Foreach(class [mscorlib]System.Collections.Generic.IEnumerable`1<valuetype [mscorlib]System.ValueTuple`2<int32,string>>)
    IL_0030:  nop
    IL_0031:  ret
  } // end of method TupleTests::CallForeach

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method TupleTests::.ctor

  .property instance bool HasItems()
  {
    .get instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_HasItems()
  } // end of property TupleTests::HasItems
  .property instance int32 VT1Member()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_VT1Member()
  } // end of property TupleTests::VT1Member
  .property instance int32 AccessUnnamed8()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_AccessUnnamed8()
  } // end of property TupleTests::AccessUnnamed8
  .property instance int32 AccessNamed8()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_AccessNamed8()
  } // end of property TupleTests::AccessNamed8
  .property instance int32 AccessPartiallyNamed()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_AccessPartiallyNamed()
  } // end of property TupleTests::AccessPartiallyNamed
  .property instance valuetype [mscorlib]System.ValueTuple`1<int32>
          NewTuple1()
  {
    .get instance valuetype [mscorlib]System.ValueTuple`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_NewTuple1()
  } // end of property TupleTests::NewTuple1
  .property instance valuetype [mscorlib]System.ValueTuple`2<int32,int32>
          NewTuple2()
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.TupleElementNamesAttribute::.ctor(string[]) = ( 01 00 02 00 00 00 01 61 01 62 00 00 )             // .......a.b..
    .get instance valuetype [mscorlib]System.ValueTuple`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_NewTuple2()
  } // end of property TupleTests::NewTuple2
  .property instance object BoxedTuple10()
  {
    .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_BoxedTuple10()
  } // end of property TupleTests::BoxedTuple10
  .property instance valuetype [mscorlib]System.ValueTuple`2<uint32,int32>
          SwapUnnamed()
  {
    .get instance valuetype [mscorlib]System.ValueTuple`2<uint32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_SwapUnnamed()
  } // end of property TupleTests::SwapUnnamed
  .property instance valuetype [mscorlib]System.ValueTuple`2<uint32,int32>
          SwapNamed2()
  {
    .get instance valuetype [mscorlib]System.ValueTuple`2<uint32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_SwapNamed2()
  } // end of property TupleTests::SwapNamed2
  .property instance int32 TupleHash()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_TupleHash()
  } // end of property TupleTests::TupleHash
  .property instance int32 TupleHash2()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_TupleHash2()
  } // end of property TupleTests::TupleHash2
  .property instance valuetype [mscorlib]System.ValueTuple`2<int32,int32>
          AccessRest()
  {
    .get instance valuetype [mscorlib]System.ValueTuple`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_AccessRest()
  } // end of property TupleTests::AccessRest
  .property instance valuetype [mscorlib]System.ValueTuple`3<string,object,class [mscorlib]System.Action>
          TargetTyping()
  {
    .get instance valuetype [mscorlib]System.ValueTuple`3<string,object,class [mscorlib]System.Action> ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_TargetTyping()
  } // end of property TupleTests::TargetTyping
  .property instance object NotTargetTyping()
  {
    .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests::get_NotTargetTyping()
  } // end of property TupleTests::NotTargetTyping
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTests


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
