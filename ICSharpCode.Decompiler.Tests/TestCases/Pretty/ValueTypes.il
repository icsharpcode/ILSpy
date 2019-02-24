



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ValueTypes
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ValueTypes.dll
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
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::Field
      IL_0008:  ret
    } // end of method S::.ctor

    .method public hidebysig instance void 
            SetField() cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.5
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::Field
      IL_0008:  ret
    } // end of method S::SetField

    .method public hidebysig instance void 
            MethodCalls() cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::SetField()
      IL_0007:  nop
      IL_0008:  ldarg.0
      IL_0009:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
      IL_000e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::Test(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S)
      IL_0013:  nop
      IL_0014:  ldarg.0
      IL_0015:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::Test(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S&)
      IL_001a:  nop
      IL_001b:  ret
    } // end of method S::MethodCalls

    .method private hidebysig static void 
            Test(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S byVal) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method S::Test

    .method private hidebysig static void 
            Test(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& byRef) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method S::Test

  } // end of class S

  .field private static initonly valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S ReadOnlyS
  .field private static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S MutableS
  .field private static int32 modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) VolatileInt
  .method public hidebysig static void  CallMethodViaField() cil managed
  {
    // Code size       41 (0x29)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_1)
    IL_0000:  nop
    IL_0001:  ldsfld     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes::ReadOnlyS
    IL_0006:  stloc.1
    IL_0007:  ldloca.s   V_1
    IL_0009:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::SetField()
    IL_000e:  nop
    IL_000f:  ldsflda    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes::MutableS
    IL_0014:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::SetField()
    IL_0019:  nop
    IL_001a:  ldsfld     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes::MutableS
    IL_001f:  stloc.0
    IL_0020:  ldloca.s   V_0
    IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::SetField()
    IL_0027:  nop
    IL_0028:  ret
  } // end of method ValueTypes::CallMethodViaField

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S 
          InitObj1() cil managed
  {
    // Code size       21 (0x15)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_1)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0009:  call       int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes::MakeArray()
    IL_000e:  pop
    IL_000f:  ldloc.0
    IL_0010:  stloc.1
    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.1
    IL_0014:  ret
  } // end of method ValueTypes::InitObj1

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S 
          InitObj2() cil managed
  {
    // Code size       15 (0xf)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_1)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_1
    IL_0003:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0009:  ldloc.1
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method ValueTypes::InitObj2

  .method public hidebysig static void  InitObj3([out] valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& p) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0008:  ret
  } // end of method ValueTypes::InitObj3

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S 
          CallValueTypeCtor1() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   10
    IL_0003:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::.ctor(int32)
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method ValueTypes::CallValueTypeCtor1

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S 
          CallValueTypeCtor2() cil managed
  {
    // Code size       17 (0x11)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_1)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  ldc.i4.s   10
    IL_0005:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::.ctor(int32)
    IL_000a:  nop
    IL_000b:  ldloc.0
    IL_000c:  stloc.1
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.1
    IL_0010:  ret
  } // end of method ValueTypes::CallValueTypeCtor2

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S 
          Copy1(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S p) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ValueTypes::Copy1

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S 
          Copy2(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& p) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method ValueTypes::Copy2

  .method public hidebysig static void  Copy3(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S p,
                                              [out] valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& o) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.0
    IL_0003:  stobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0008:  ret
  } // end of method ValueTypes::Copy3

  .method public hidebysig static void  Copy4(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& p,
                                              [out] valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& o) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.0
    IL_0003:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0008:  stobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_000d:  ret
  } // end of method ValueTypes::Copy4

  .method public hidebysig static void  Copy4b(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& p,
                                               [out] valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S& o) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes::Copy4(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S&,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S&)
    IL_0008:  nop
    IL_0009:  ret
  } // end of method ValueTypes::Copy4b

  .method public hidebysig static void  Issue56(int32 i,
                                                [out] string& str) cil managed
  {
    // Code size       25 (0x19)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldstr      "qq"
    IL_0007:  stind.ref
    IL_0008:  ldarg.1
    IL_0009:  dup
    IL_000a:  ldind.ref
    IL_000b:  ldarga.s   i
    IL_000d:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0012:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0017:  stind.ref
    IL_0018:  ret
  } // end of method ValueTypes::Issue56

  .method public hidebysig static void  CopyAroundAndModifyField(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S s) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  dup
    IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::Field
    IL_000b:  ldc.i4.s   10
    IL_000d:  add
    IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::Field
    IL_0013:  ldloc.0
    IL_0014:  starg.s    s
    IL_0016:  ret
  } // end of method ValueTypes::CopyAroundAndModifyField

  .method private hidebysig static int32[] 
          MakeArray() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32[] V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ValueTypes::MakeArray

  .method public hidebysig static void  IncrementArrayLocation() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes::MakeArray()
    IL_0006:  call       int32 [mscorlib]System.Environment::get_TickCount()
    IL_000b:  ldelema    [mscorlib]System.Int32
    IL_0010:  dup
    IL_0011:  ldobj      [mscorlib]System.Int32
    IL_0016:  ldc.i4.1
    IL_0017:  add
    IL_0018:  stobj      [mscorlib]System.Int32
    IL_001d:  ret
  } // end of method ValueTypes::IncrementArrayLocation

  .method public hidebysig static bool  Is(object obj) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  isinst     ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S
    IL_0007:  ldnull
    IL_0008:  cgt.un
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method ValueTypes::Is

  .method public hidebysig static bool  IsNullable(object obj) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  isinst     valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S>
    IL_0007:  ldnull
    IL_0008:  cgt.un
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method ValueTypes::IsNullable

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S> 
          As(object obj) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S> V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  isinst     valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S>
    IL_0007:  unbox.any  valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S>
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method ValueTypes::As

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S 
          OnlyChangeTheCopy(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S p) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ValueTypes/S::SetField()
    IL_000a:  nop
    IL_000b:  ldarg.0
    IL_000c:  stloc.1
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.1
    IL_0010:  ret
  } // end of method ValueTypes::OnlyChangeTheCopy

  .method public hidebysig static void  UseRefBoolInCondition(bool& x) cil managed
  {
    // Code size       24 (0x18)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldind.i1
    IL_0003:  ldc.i4.0
    IL_0004:  ceq
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brtrue.s   IL_0017

    IL_000a:  nop
    IL_000b:  ldstr      "true"
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0015:  nop
    IL_0016:  nop
    IL_0017:  ret
  } // end of method ValueTypes::UseRefBoolInCondition

  .method public hidebysig static void  CompareNotEqual0IsReallyNotEqual(class [mscorlib]System.IComparable`1<int32> a) cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance int32 class [mscorlib]System.IComparable`1<int32>::CompareTo(!0)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  brtrue.s   IL_001c

    IL_000f:  nop
    IL_0010:  ldstr      "true"
    IL_0015:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001a:  nop
    IL_001b:  nop
    IL_001c:  ret
  } // end of method ValueTypes::CompareNotEqual0IsReallyNotEqual

  .method public hidebysig static void  CompareEqual0IsReallyEqual(class [mscorlib]System.IComparable`1<int32> a) cil managed
  {
    // Code size       32 (0x20)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance int32 class [mscorlib]System.IComparable`1<int32>::CompareTo(!0)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  ldc.i4.0
    IL_000c:  ceq
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  brtrue.s   IL_001f

    IL_0012:  nop
    IL_0013:  ldstr      "true"
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001d:  nop
    IL_001e:  nop
    IL_001f:  ret
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
