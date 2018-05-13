



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
.assembly TupleTypes
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
.module TupleTypes.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTypes
       extends [mscorlib]System.Object
{
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
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method TupleTypes::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.TupleTypes


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
