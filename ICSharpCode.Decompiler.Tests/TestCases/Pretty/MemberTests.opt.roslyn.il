



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly MemberTests
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module MemberTests.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MemberTests
       extends [mscorlib]System.Object
{
  .class auto ansi nested public beforefieldinit IndexerNonDefaultName
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 03 46 6F 6F 00 00 )                         // ...Foo..
    .method public hidebysig specialname 
            instance int32  get_Foo(int32 index) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } // end of method IndexerNonDefaultName::get_Foo

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method IndexerNonDefaultName::.ctor

    .property instance int32 Foo(int32)
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.MemberTests/IndexerNonDefaultName::get_Foo(int32)
    } // end of property IndexerNonDefaultName::Foo
  } // end of class IndexerNonDefaultName

  .class auto ansi nested public beforefieldinit NoDefaultMember
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 03 42 61 72 00 00 )                         // ...Bar..
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method NoDefaultMember::.ctor

  } // end of class NoDefaultMember

  .field public static literal int32 IntConstant = int32(0x00000001)
  .field public static initonly valuetype [mscorlib]System.Decimal DecimalConstant
  .custom instance void [mscorlib]System.Runtime.CompilerServices.DecimalConstantAttribute::.ctor(uint8,
                                                                                                  uint8,
                                                                                                  uint32,
                                                                                                  uint32,
                                                                                                  uint32) = ( 01 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 
                                                                                                              00 00 ) 
  .field private int32 modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) volatileField
  .field private static int32 modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) staticVolatileField
  .method public hidebysig instance void 
          UseVolatileFields() cil managed
  {
    // Code size       56 (0x38)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  volatile.
    IL_0003:  ldfld      int32 modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) ICSharpCode.Decompiler.Tests.TestCases.Pretty.MemberTests::volatileField
    IL_0008:  volatile.
    IL_000a:  ldsfld     int32 modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) ICSharpCode.Decompiler.Tests.TestCases.Pretty.MemberTests::staticVolatileField
    IL_000f:  add
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0015:  ldarg.0
    IL_0016:  ldarg.0
    IL_0017:  volatile.
    IL_0019:  ldfld      int32 modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) ICSharpCode.Decompiler.Tests.TestCases.Pretty.MemberTests::volatileField
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  volatile.
    IL_0022:  stfld      int32 modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) ICSharpCode.Decompiler.Tests.TestCases.Pretty.MemberTests::volatileField
    IL_0027:  volatile.
    IL_0029:  ldsfld     int32 modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) ICSharpCode.Decompiler.Tests.TestCases.Pretty.MemberTests::staticVolatileField
    IL_002e:  ldc.i4.1
    IL_002f:  add
    IL_0030:  volatile.
    IL_0032:  stsfld     int32 modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) ICSharpCode.Decompiler.Tests.TestCases.Pretty.MemberTests::staticVolatileField
    IL_0037:  ret
  } // end of method MemberTests::UseVolatileFields

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       16 (0x10)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.3
    IL_0002:  volatile.
    IL_0004:  stfld      int32 modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) ICSharpCode.Decompiler.Tests.TestCases.Pretty.MemberTests::volatileField
    IL_0009:  ldarg.0
    IL_000a:  call       instance void [mscorlib]System.Object::.ctor()
    IL_000f:  ret
  } // end of method MemberTests::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       20 (0x14)
    .maxstack  8
    IL_0000:  ldc.i4.2
    IL_0001:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0006:  stsfld     valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.MemberTests::DecimalConstant
    IL_000b:  ldc.i4.4
    IL_000c:  volatile.
    IL_000e:  stsfld     int32 modreq([mscorlib]System.Runtime.CompilerServices.IsVolatile) ICSharpCode.Decompiler.Tests.TestCases.Pretty.MemberTests::staticVolatileField
    IL_0013:  ret
  } // end of method MemberTests::.cctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MemberTests


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
