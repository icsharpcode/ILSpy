



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ValueTypes.opt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ValueTypes.opt.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes
       extends [mscorlib]System.Object
{
  .class sequential ansi sealed nested public beforefieldinit S
         extends [mscorlib]System.ValueType
  {
    .field public int32 Field
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(int32 'field') cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::Field
      IL_0007:  ret
    } // end of method S::.ctor

    .method public hidebysig instance void 
            SetField() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.5
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::Field
      IL_0007:  ret
    } // end of method S::SetField

    .method public hidebysig instance void 
            MethodCalls() cil managed
    {
      // Code size       24 (0x18)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::SetField()
      IL_0006:  ldarg.0
      IL_0007:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
      IL_000c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::Test(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S)
      IL_0011:  ldarg.0
      IL_0012:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::Test(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S&)
      IL_0017:  ret
    } // end of method S::MethodCalls

    .method private hidebysig static void 
            Test(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S byVal) cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method S::Test

    .method private hidebysig static void 
            Test(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& byRef) cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method S::Test

  } // end of class S

  .field private static initonly valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S ReadOnlyS
  .field private static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S MutableS
  .field private static int32 modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) VolatileInt
  .method public hidebysig static void  CallMethodViaField() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_1)
    IL_0000:  ldsfld     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes::ReadOnlyS
    IL_0005:  stloc.1
    IL_0006:  ldloca.s   V_1
    IL_0008:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::SetField()
    IL_000d:  ldsflda    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes::MutableS
    IL_0012:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::SetField()
    IL_0017:  ldsfld     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes::MutableS
    IL_001c:  stloc.0
    IL_001d:  ldloca.s   V_0
    IL_001f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::SetField()
    IL_0024:  ret
  } // end of method ValueTypes::CallMethodViaField

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S 
          InitObj1() cil managed
  {
    // Code size       16 (0x10)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0)
    IL_0000:  ldloca.s   V_0
    IL_0002:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0008:  call       int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes::MakeArray()
    IL_000d:  pop
    IL_000e:  ldloc.0
    IL_000f:  ret
  } // end of method ValueTypes::InitObj1

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S 
          InitObj2() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0)
    IL_0000:  ldloca.s   V_0
    IL_0002:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method ValueTypes::InitObj2

  .method public hidebysig static void  InitObj3([out] valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& p) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0007:  ret
  } // end of method ValueTypes::InitObj3

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S 
          CallValueTypeCtor1() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.s   10
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::.ctor(int32)
    IL_0007:  ret
  } // end of method ValueTypes::CallValueTypeCtor1

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S 
          CallValueTypeCtor2() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldc.i4.s   10
    IL_0004:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::.ctor(int32)
    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method ValueTypes::CallValueTypeCtor2

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S 
          Copy1(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S p) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  } // end of method ValueTypes::Copy1

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S 
          Copy2(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& p) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0006:  ret
  } // end of method ValueTypes::Copy2

  .method public hidebysig static void  Copy3(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S p,
                                              [out] valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& o) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  stobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0007:  ret
  } // end of method ValueTypes::Copy3

  .method public hidebysig static void  Copy4(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& p,
                                              [out] valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& o) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0007:  stobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_000c:  ret
  } // end of method ValueTypes::Copy4

  .method public hidebysig static void  Copy4b(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& p,
                                               [out] valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& o) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes::Copy4(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S&,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S&)
    IL_0007:  ret
  } // end of method ValueTypes::Copy4b

  .method public hidebysig static void  Issue56(int32 i,
                                                [out] string& str) cil managed
  {
    // Code size       24 (0x18)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldstr      "qq"
    IL_0006:  stind.ref
    IL_0007:  ldarg.1
    IL_0008:  dup
    IL_0009:  ldind.ref
    IL_000a:  ldarga.s   i
    IL_000c:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0011:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0016:  stind.ref
    IL_0017:  ret
  } // end of method ValueTypes::Issue56

  .method public hidebysig static void  CopyAroundAndModifyField(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S s) cil managed
  {
    // Code size       22 (0x16)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  dup
    IL_0005:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::Field
    IL_000a:  ldc.i4.s   10
    IL_000c:  add
    IL_000d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::Field
    IL_0012:  ldloc.0
    IL_0013:  starg.s    s
    IL_0015:  ret
  } // end of method ValueTypes::CopyAroundAndModifyField

  .method private hidebysig static int32[] 
          MakeArray() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ret
  } // end of method ValueTypes::MakeArray

  .method public hidebysig static void  IncrementArrayLocation() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  8
    IL_0000:  call       int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes::MakeArray()
    IL_0005:  call       int32 [mscorlib]System.Environment::get_TickCount()
    IL_000a:  ldelema    [mscorlib]System.Int32
    IL_000f:  dup
    IL_0010:  ldobj      [mscorlib]System.Int32
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stobj      [mscorlib]System.Int32
    IL_001c:  ret
  } // end of method ValueTypes::IncrementArrayLocation

  .method public hidebysig static bool  Is(object obj) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  isinst     ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0006:  ldnull
    IL_0007:  cgt.un
    IL_0009:  ret
  } // end of method ValueTypes::Is

  .method public hidebysig static bool  IsNullable(object obj) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  isinst     valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S>
    IL_0006:  ldnull
    IL_0007:  cgt.un
    IL_0009:  ret
  } // end of method ValueTypes::IsNullable

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S> 
          As(object obj) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  isinst     valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S>
    IL_0006:  unbox.any  valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S>
    IL_000b:  ret
  } // end of method ValueTypes::As

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S 
          OnlyChangeTheCopy(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S p) cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::SetField()
    IL_0009:  ldarg.0
    IL_000a:  ret
  } // end of method ValueTypes::OnlyChangeTheCopy

  .method public hidebysig static void  UseRefBoolInCondition(bool& x) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldind.i1
    IL_0002:  brfalse.s  IL_000e

    IL_0004:  ldstr      "true"
    IL_0009:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000e:  ret
  } // end of method ValueTypes::UseRefBoolInCondition

  .method public hidebysig static void  CompareNotEqual0IsReallyNotEqual(class [mscorlib]System.IComparable`1<int32> a) cil managed
  {
    // Code size       20 (0x14)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance int32 class [mscorlib]System.IComparable`1<int32>::CompareTo(!0)
    IL_0007:  brfalse.s  IL_0013

    IL_0009:  ldstr      "true"
    IL_000e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0013:  ret
  } // end of method ValueTypes::CompareNotEqual0IsReallyNotEqual

  .method public hidebysig static void  CompareEqual0IsReallyEqual(class [mscorlib]System.IComparable`1<int32> a) cil managed
  {
    // Code size       20 (0x14)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance int32 class [mscorlib]System.IComparable`1<int32>::CompareTo(!0)
    IL_0007:  brtrue.s   IL_0013

    IL_0009:  ldstr      "true"
    IL_000e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0013:  ret
  } // end of method ValueTypes::CompareEqual0IsReallyEqual

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       23 (0x17)
    .maxstack  8
    IL_0000:  ldsflda    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes::ReadOnlyS
    IL_0005:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_000b:  ldsflda    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes::MutableS
    IL_0010:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0016:  ret
  } // end of method ValueTypes::.cctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
