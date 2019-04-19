
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:2:1:0
}
.assembly extern System.Diagnostics.Debug
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:1:1:0
}
.assembly extern System.IO.FileSystem
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:1:1:0
}
.assembly Issue1325
{
  .custom instance void Microsoft.VisualBasic.Embedded::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                                   63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

  .custom instance void [System.Runtime]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 18 2E 4E 45 54 43 6F 72 65 41 70 70 2C 56   // ....NETCoreApp,V
                                                                                                              65 72 73 69 6F 6E 3D 76 32 2E 31 01 00 54 0E 14   // ersion=v2.1..T..
                                                                                                              46 72 61 6D 65 77 6F 72 6B 44 69 73 70 6C 61 79   // FrameworkDisplay
                                                                                                              4E 61 6D 65 00 )                                  // Name.
  .custom instance void [System.Runtime]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 05 44 65 62 75 67 00 00 )                   // ...Debug..
  .custom instance void [System.Runtime]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..
  .custom instance void [System.Runtime]System.Reflection.AssemblyInformationalVersionAttribute::.ctor(string) = ( 01 00 05 31 2E 30 2E 30 00 00 )                   // ...1.0.0..
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module Issue1325.dll
// MVID: {CE79A917-9454-47A4-8FF9-0348706BB573}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01240000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi sealed Issue1325.Program
       extends [System.Runtime]System.Object
{
  .custom instance void Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  Main(string[] args) cil managed
  {
    .entrypoint
    .custom instance void [System.Runtime]System.STAThreadAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method Program::Main

  .method public static void  TestCode(class Issue1325.Test t,
                                       int32 i) cil managed
  {
    // Code size       64 (0x40)
    .maxstack  3
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldstr      ""
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldstr      "Test.txt"
    IL_000d:  call       string [System.IO.FileSystem]System.IO.File::ReadAllText(string)
    IL_0012:  call       string [System.Runtime]System.String::Concat(string,
                                                                      string)
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  ldstr      "asdf"
    IL_001e:  call       string [System.Runtime]System.String::Concat(string,
                                                                      string)
    IL_0023:  stloc.0
    IL_0024:  ldarg.0
    IL_0025:  ldarg.1
    IL_0026:  ldloc.0
    IL_0027:  callvirt   instance void Issue1325.Test::set_Parameterized(int32,
                                                                            string)
    IL_002c:  nop
    IL_002d:  ldarg.0
    IL_002e:  ldloc.0
    IL_002f:  ldstr      "asdf"
    IL_0034:  call       string [System.Runtime]System.String::Concat(string,
                                                                      string)
    IL_0039:  callvirt   instance void Issue1325.Test::set_Unparameterized(string)
    IL_003e:  nop
    IL_003f:  ret
  } // end of method Program::TestCode

} // end of class Issue1325.Program

.class private auto ansi Issue1325.Test
       extends [System.Runtime]System.Object
{
  .field private string _Unparameterized
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Diagnostics.Debug]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Diagnostics.Debug]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Test::.ctor

  .method public specialname instance string 
          get_Parameterized(int32 i) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void [System.Runtime]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method Test::get_Parameterized

  .method public specialname instance void 
          set_Parameterized(int32 i,
                            string 'value') cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [System.Runtime]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method Test::set_Parameterized

  .method public specialname instance string 
          get_Unparameterized() cil managed
  {
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string Issue1325.Test::_Unparameterized
    IL_0006:  br.s       IL_0008

    IL_0008:  ret
  } // end of method Test::get_Unparameterized

  .method public specialname instance void 
          set_Unparameterized(string AutoPropertyValue) cil managed
  {
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      string Issue1325.Test::_Unparameterized
    IL_0007:  ret
  } // end of method Test::set_Unparameterized

  .property instance string Parameterized(int32)
  {
    .get instance string Issue1325.Test::get_Parameterized(int32)
    .set instance void Issue1325.Test::set_Parameterized(int32,
                                                            string)
  } // end of property Test::Parameterized
  .property instance string Unparameterized()
  {
    .get instance string Issue1325.Test::get_Unparameterized()
    .set instance void Issue1325.Test::set_Unparameterized(string)
  } // end of property Test::Unparameterized
} // end of class Issue1325.Test

.class private auto ansi sealed Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute
       extends [System.Runtime]System.Attribute
{
  .custom instance void Microsoft.VisualBasic.Embedded::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.AttributeUsageAttribute::.ctor(valuetype [System.Runtime]System.AttributeTargets) = ( 01 00 04 00 00 00 01 00 54 02 09 49 6E 68 65 72   // ........T..Inher
                                                                                                                                     69 74 65 64 00 )                                  // ited.
  .custom instance void [System.Runtime]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System.Runtime]System.ComponentModel.EditorBrowsableState) = ( 01 00 01 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method StandardModuleAttribute::.ctor

} // end of class Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute

.class private auto ansi sealed Microsoft.VisualBasic.Embedded
       extends [System.Runtime]System.Attribute
{
  .custom instance void Microsoft.VisualBasic.Embedded::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.AttributeUsageAttribute::.ctor(valuetype [System.Runtime]System.AttributeTargets) = ( 01 00 07 00 00 00 01 00 54 02 09 49 6E 68 65 72   // ........T..Inher
                                                                                                                                     69 74 65 64 00 )                                  // ited.
  .custom instance void [System.Runtime]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System.Runtime]System.ComponentModel.EditorBrowsableState) = ( 01 00 01 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method Embedded::.ctor

} // end of class Microsoft.VisualBasic.Embedded