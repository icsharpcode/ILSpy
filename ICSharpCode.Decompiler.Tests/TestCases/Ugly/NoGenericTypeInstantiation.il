
//  .NET IL Disassembler.  Version 9.0.4



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly tmppi3wbt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module tmppi3wbt.tmp
// MVID: {4130db71-37be-40af-9724-d988f7189866}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoGenericTypeInstantiation`1<.ctor TOnType>
       extends [mscorlib]System.Object
{
  .method public hidebysig static !TOnType 
          CreateTOnType() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  1
    .locals init (!TOnType V_0,
             !TOnType V_1)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_1
    IL_0003:  initobj    !TOnType
    IL_0009:  ldloc.1
    IL_000a:  box        !TOnType
    IL_000f:  brfalse.s  IL_001c

    IL_0011:  ldloca.s   V_1
    IL_0013:  initobj    !TOnType
    IL_0019:  ldloc.1
    IL_001a:  br.s       IL_0021

    IL_001c:  call       !!0 [mscorlib]System.Activator::CreateInstance<!TOnType>()
    IL_0021:  nop
    IL_0022:  stloc.0
    IL_0023:  br.s       IL_0025

    IL_0025:  ldloc.0
    IL_0026:  ret
  } // end of method NoGenericTypeInstantiation`1::CreateTOnType

  .method public hidebysig static !!TOnMethod 
          CreateTOnMethod<.ctor TOnMethod>() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  1
    .locals init (!!TOnMethod V_0,
             !!TOnMethod V_1)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_1
    IL_0003:  initobj    !!TOnMethod
    IL_0009:  ldloc.1
    IL_000a:  box        !!TOnMethod
    IL_000f:  brfalse.s  IL_001c

    IL_0011:  ldloca.s   V_1
    IL_0013:  initobj    !!TOnMethod
    IL_0019:  ldloc.1
    IL_001a:  br.s       IL_0021

    IL_001c:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0021:  nop
    IL_0022:  stloc.0
    IL_0023:  br.s       IL_0025

    IL_0025:  ldloc.0
    IL_0026:  ret
  } // end of method NoGenericTypeInstantiation`1::CreateTOnMethod

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method NoGenericTypeInstantiation`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoGenericTypeInstantiation`1


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
