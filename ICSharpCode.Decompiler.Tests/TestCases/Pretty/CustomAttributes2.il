



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly CustomAttributes2
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module CustomAttributes2.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit CustomAttributes2.CustomAtributes
       extends [mscorlib]System.Object
{
  .class auto ansi sealed nested public EnumWithFlag
         extends [mscorlib]System.Enum
  {
    .custom instance void [mscorlib]System.FlagsAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype CustomAttributes2.CustomAtributes/EnumWithFlag All = int32(0x0000000F)
    .field public static literal valuetype CustomAttributes2.CustomAtributes/EnumWithFlag None = int32(0x00000000)
    .field public static literal valuetype CustomAttributes2.CustomAtributes/EnumWithFlag Item1 = int32(0x00000001)
    .field public static literal valuetype CustomAttributes2.CustomAtributes/EnumWithFlag Item2 = int32(0x00000002)
    .field public static literal valuetype CustomAttributes2.CustomAtributes/EnumWithFlag Item3 = int32(0x00000004)
    .field public static literal valuetype CustomAttributes2.CustomAtributes/EnumWithFlag Item4 = int32(0x00000008)
  } // end of class EnumWithFlag

  .class auto ansi nested public beforefieldinit MyAttribute
         extends [mscorlib]System.Attribute
  {
    .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 FF 7F 00 00 00 00 ) 
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(valuetype CustomAttributes2.CustomAtributes/EnumWithFlag en) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
      IL_0006:  nop
      IL_0007:  nop
      IL_0008:  nop
      IL_0009:  ret
    } // end of method MyAttribute::.ctor

  } // end of class MyAttribute

  .field private static int32 'field'
  .custom instance void CustomAttributes2.CustomAtributes/MyAttribute::.ctor(valuetype CustomAttributes2.CustomAtributes/EnumWithFlag) = ( 01 00 03 00 00 00 00 00 ) 
  .method public hidebysig specialname static 
          string  get_Property() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "aa"
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method CustomAtributes::get_Property

  .method public hidebysig specialname static 
          string  get_GetterOnlyPropertyWithAttributeOnGetter() cil managed
  {
    .custom instance void CustomAttributes2.CustomAtributes/MyAttribute::.ctor(valuetype CustomAttributes2.CustomAtributes/EnumWithFlag) = ( 01 00 01 00 00 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "aa"
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method CustomAtributes::get_GetterOnlyPropertyWithAttributeOnGetter

  .method public hidebysig specialname static 
          string  get_GetterOnlyPropertyWithAttributeOnGetter2() cil managed
  {
    .custom instance void CustomAttributes2.CustomAtributes/MyAttribute::.ctor(valuetype CustomAttributes2.CustomAtributes/EnumWithFlag) = ( 01 00 01 00 00 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "aa"
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method CustomAtributes::get_GetterOnlyPropertyWithAttributeOnGetter2

  .method public hidebysig static void  ObsoletedMethod() cil managed
  {
    .custom instance void [mscorlib]System.ObsoleteAttribute::.ctor(string) = ( 01 00 0C 73 6F 6D 65 20 6D 65 73 73 61 67 65 00   // ...some message.
                                                                                00 ) 
    // Code size       66 (0x42)
    .maxstack  3
    .locals init (valuetype [mscorlib]System.AttributeTargets V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "{0} $$$ {1}"
    IL_0006:  ldc.i4     0x400
    IL_000b:  box        [mscorlib]System.AttributeTargets
    IL_0010:  ldc.i4     0x180
    IL_0015:  box        [mscorlib]System.AttributeTargets
    IL_001a:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_001f:  nop
    IL_0020:  ldc.i4     0x180
    IL_0025:  stloc.0
    IL_0026:  ldstr      "{0} $$$ {1}"
    IL_002b:  ldc.i4     0x400
    IL_0030:  box        [mscorlib]System.AttributeTargets
    IL_0035:  ldloc.0
    IL_0036:  box        [mscorlib]System.AttributeTargets
    IL_003b:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object,
                                                                  object)
    IL_0040:  nop
    IL_0041:  ret
  } // end of method CustomAtributes::ObsoletedMethod

  .property string Property()
  {
    .custom instance void CustomAttributes2.CustomAtributes/MyAttribute::.ctor(valuetype CustomAttributes2.CustomAtributes/EnumWithFlag) = ( 01 00 0F 00 00 00 00 00 ) 
    .get string CustomAttributes2.CustomAtributes::get_Property()
  } // end of property CustomAtributes::Property
  .property string GetterOnlyPropertyWithAttributeOnGetter()
  {
    .get string CustomAttributes2.CustomAtributes::get_GetterOnlyPropertyWithAttributeOnGetter()
  } // end of property CustomAtributes::GetterOnlyPropertyWithAttributeOnGetter
  .property string GetterOnlyPropertyWithAttributeOnGetter2()
  {
    .custom instance void CustomAttributes2.CustomAtributes/MyAttribute::.ctor(valuetype CustomAttributes2.CustomAtributes/EnumWithFlag) = ( 01 00 0F 00 00 00 00 00 ) 
    .get string CustomAttributes2.CustomAtributes::get_GetterOnlyPropertyWithAttributeOnGetter2()
  } // end of property CustomAtributes::GetterOnlyPropertyWithAttributeOnGetter2
} // end of class CustomAttributes2.CustomAtributes


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
