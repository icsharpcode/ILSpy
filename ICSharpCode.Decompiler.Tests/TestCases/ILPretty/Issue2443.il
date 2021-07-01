
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern ClassLibrary1
{
  .ver 1:0:0:0
}
.assembly ConsoleApp8
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .custom instance void [mscorlib]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 1C 2E 4E 45 54 46 72 61 6D 65 77 6F 72 6B   // ....NETFramework
                                                                                                        2C 56 65 72 73 69 6F 6E 3D 76 34 2E 37 2E 32 01   // ,Version=v4.7.2.
                                                                                                        00 54 0E 14 46 72 61 6D 65 77 6F 72 6B 44 69 73   // .T..FrameworkDis
                                                                                                        70 6C 61 79 4E 61 6D 65 14 2E 4E 45 54 20 46 72   // playName..NET Fr
                                                                                                        61 6D 65 77 6F 72 6B 20 34 2E 37 2E 32 )          // amework 4.7.2
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 0B 43 6F 6E 73 6F 6C 65 41 70 70 38 00 00 ) // ...ConsoleApp8..
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 07 52 65 6C 65 61 73 65 00 00 )             // ...Release..
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..
  .custom instance void [mscorlib]System.Reflection.AssemblyInformationalVersionAttribute::.ctor(string) = ( 01 00 05 31 2E 30 2E 30 00 00 )                   // ...1.0.0..
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 0B 43 6F 6E 73 6F 6C 65 41 70 70 38 00 00 ) // ...ConsoleApp8..
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 0B 43 6F 6E 73 6F 6C 65 41 70 70 38 00 00 ) // ...ConsoleApp8..
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module ConsoleApp8.exe
// MVID: {C71BB5CC-A98C-4CF4-B8A4-000055F94187}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02890000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ConsoleApp8.Program
       extends [ClassLibrary1]ClassLibrary1.Class1
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(string _) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void [ClassLibrary1]ClassLibrary1.Class1::.ctor(string)
    IL_0007:  ldstr      "Class1(string)<-Program(string)"
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0011:  ret
  } // end of method Program::.ctor

  .method private hidebysig static void  Main(string[] args) cil managed
  {
    .entrypoint
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldstr      ""
    IL_0005:  newobj     instance void ConsoleApp8.Program::.ctor(string)
    IL_000a:  pop
    IL_000b:  ldstr      "Hello World!"
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0015:  ret
  } // end of method Program::Main

} // end of class ConsoleApp8.Program


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file Issue2443.res
