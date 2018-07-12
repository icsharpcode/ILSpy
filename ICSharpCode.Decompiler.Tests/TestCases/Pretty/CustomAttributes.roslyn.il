



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly CustomAttributes
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
.module CustomAttributes.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit CustomAttributes.CustomAttributes
       extends [mscorlib]System.Object
{
  .class auto ansi sealed nested public EnumWithFlag
         extends [mscorlib]System.Enum
  {
    .custom instance void [mscorlib]System.FlagsAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype CustomAttributes.CustomAttributes/EnumWithFlag All = int32(0x0000000F)
    .field public static literal valuetype CustomAttributes.CustomAttributes/EnumWithFlag None = int32(0x00000000)
    .field public static literal valuetype CustomAttributes.CustomAttributes/EnumWithFlag Item1 = int32(0x00000001)
    .field public static literal valuetype CustomAttributes.CustomAttributes/EnumWithFlag Item2 = int32(0x00000002)
    .field public static literal valuetype CustomAttributes.CustomAttributes/EnumWithFlag Item3 = int32(0x00000004)
    .field public static literal valuetype CustomAttributes.CustomAttributes/EnumWithFlag Item4 = int32(0x00000008)
  } // end of class EnumWithFlag

  .class auto ansi nested public beforefieldinit MyAttribute
         extends [mscorlib]System.Attribute
  {
    .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 FF 7F 00 00 00 00 ) 
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(object val) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
      IL_0006:  nop
      IL_0007:  nop
      IL_0008:  ret
    } // end of method MyAttribute::.ctor

  } // end of class MyAttribute

  .class auto ansi sealed nested public ULongEnum
         extends [mscorlib]System.Enum
  {
    .custom instance void CustomAttributes.CustomAttributes/MyAttribute::.ctor(object) = ( 01 00 55 2B 43 75 73 74 6F 6D 41 74 74 72 69 62   // ..U+CustomAttrib
                                                                                           75 74 65 73 2E 43 75 73 74 6F 6D 41 74 74 72 69   // utes.CustomAttri
                                                                                           62 75 74 65 73 2B 55 4C 6F 6E 67 45 6E 75 6D FF   // butes+ULongEnum.
                                                                                           FF FF FF FF FF FF FF 00 00 ) 
    .field public specialname rtspecialname uint64 value__
    .field public static literal valuetype CustomAttributes.CustomAttributes/ULongEnum MaxUInt64 = uint64(0xFFFFFFFFFFFFFFFF)
    .custom instance void CustomAttributes.CustomAttributes/MyAttribute::.ctor(object) = ( 01 00 0E FF 00 00 ) 
  } // end of class ULongEnum

  .class auto ansi nested public beforefieldinit TypesAttribute
         extends [mscorlib]System.Attribute
  {
    .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 00 01 00 00 00 00 ) 
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(class [mscorlib]System.Type 'type') cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
      IL_0006:  nop
      IL_0007:  nop
      IL_0008:  ret
    } // end of method TypesAttribute::.ctor

  } // end of class TypesAttribute

  .class auto ansi nested private beforefieldinit SomeType`1<T>
         extends [mscorlib]System.Object
  {
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method SomeType`1::.ctor

  } // end of class SomeType`1

  .class auto ansi nested private beforefieldinit SomeType`2<K,V>
         extends [mscorlib]System.Object
  {
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method SomeType`2::.ctor

  } // end of class SomeType`2

  .class sequential ansi sealed nested private beforefieldinit DataType
         extends [mscorlib]System.ValueType
  {
    .field private int32 i
  } // end of class DataType

  .field private static int32 typeattr_int
  .custom instance void CustomAttributes.CustomAttributes/TypesAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 59 53 79 73 74 65 6D 2E 49 6E 74 33 32 2C   // ..YSystem.Int32,
                                                                                                                 20 6D 73 63 6F 72 6C 69 62 2C 20 56 65 72 73 69   //  mscorlib, Versi
                                                                                                                 6F 6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75 6C 74   // on=4.0.0.0, Cult
                                                                                                                 75 72 65 3D 6E 65 75 74 72 61 6C 2C 20 50 75 62   // ure=neutral, Pub
                                                                                                                 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61   // licKeyToken=b77a
                                                                                                                 35 63 35 36 31 39 33 34 65 30 38 39 00 00 )       // 5c561934e089..
  .field private static int32 typeattr_null
  .custom instance void CustomAttributes.CustomAttributes/TypesAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 FF 00 00 ) 
  .field private static int32 typeattr_list_of_int
  .custom instance void CustomAttributes.CustomAttributes/TypesAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 80 CB 53 79 73 74 65 6D 2E 43 6F 6C 6C 65   // ....System.Colle
                                                                                                                 63 74 69 6F 6E 73 2E 47 65 6E 65 72 69 63 2E 4C   // ctions.Generic.L
                                                                                                                 69 73 74 60 31 5B 5B 53 79 73 74 65 6D 2E 49 6E   // ist`1[[System.In
                                                                                                                 74 33 32 2C 20 6D 73 63 6F 72 6C 69 62 2C 20 56   // t32, mscorlib, V
                                                                                                                 65 72 73 69 6F 6E 3D 34 2E 30 2E 30 2E 30 2C 20   // ersion=4.0.0.0, 
                                                                                                                 43 75 6C 74 75 72 65 3D 6E 65 75 74 72 61 6C 2C   // Culture=neutral,
                                                                                                                 20 50 75 62 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D   //  PublicKeyToken=
                                                                                                                 62 37 37 61 35 63 35 36 31 39 33 34 65 30 38 39   // b77a5c561934e089
                                                                                                                 5D 5D 2C 20 6D 73 63 6F 72 6C 69 62 2C 20 56 65   // ]], mscorlib, Ve
                                                                                                                 72 73 69 6F 6E 3D 34 2E 30 2E 30 2E 30 2C 20 43   // rsion=4.0.0.0, C
                                                                                                                 75 6C 74 75 72 65 3D 6E 65 75 74 72 61 6C 2C 20   // ulture=neutral, 
                                                                                                                 50 75 62 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D 62   // PublicKeyToken=b
                                                                                                                 37 37 61 35 63 35 36 31 39 33 34 65 30 38 39 00   // 77a5c561934e089.
                                                                                                                 00 ) 
  .field private static int32 typeattr_list_unbound
  .custom instance void CustomAttributes.CustomAttributes/TypesAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 6E 53 79 73 74 65 6D 2E 43 6F 6C 6C 65 63   // ..nSystem.Collec
                                                                                                                 74 69 6F 6E 73 2E 47 65 6E 65 72 69 63 2E 4C 69   // tions.Generic.Li
                                                                                                                 73 74 60 31 2C 20 6D 73 63 6F 72 6C 69 62 2C 20   // st`1, mscorlib, 
                                                                                                                 56 65 72 73 69 6F 6E 3D 34 2E 30 2E 30 2E 30 2C   // Version=4.0.0.0,
                                                                                                                 20 43 75 6C 74 75 72 65 3D 6E 65 75 74 72 61 6C   //  Culture=neutral
                                                                                                                 2C 20 50 75 62 6C 69 63 4B 65 79 54 6F 6B 65 6E   // , PublicKeyToken
                                                                                                                 3D 62 37 37 61 35 63 35 36 31 39 33 34 65 30 38   // =b77a5c561934e08
                                                                                                                 39 00 00 )                                        // 9..
  .field private static int32 typeattr_sometype_of_datatype
  .custom instance void CustomAttributes.CustomAttributes/TypesAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 58 43 75 73 74 6F 6D 41 74 74 72 69 62 75   // ..XCustomAttribu
                                                                                                                 74 65 73 2E 43 75 73 74 6F 6D 41 74 74 72 69 62   // tes.CustomAttrib
                                                                                                                 75 74 65 73 2B 53 6F 6D 65 54 79 70 65 60 31 5B   // utes+SomeType`1[
                                                                                                                 43 75 73 74 6F 6D 41 74 74 72 69 62 75 74 65 73   // CustomAttributes
                                                                                                                 2E 43 75 73 74 6F 6D 41 74 74 72 69 62 75 74 65   // .CustomAttribute
                                                                                                                 73 2B 44 61 74 61 54 79 70 65 5D 00 00 )          // s+DataType]..
  .field private static int32 typeattr_sometype_of_datatype2
  .custom instance void CustomAttributes.CustomAttributes/TypesAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 80 83 43 75 73 74 6F 6D 41 74 74 72 69 62   // ....CustomAttrib
                                                                                                                 75 74 65 73 2E 43 75 73 74 6F 6D 41 74 74 72 69   // utes.CustomAttri
                                                                                                                 62 75 74 65 73 2B 53 6F 6D 65 54 79 70 65 60 32   // butes+SomeType`2
                                                                                                                 5B 43 75 73 74 6F 6D 41 74 74 72 69 62 75 74 65   // [CustomAttribute
                                                                                                                 73 2E 43 75 73 74 6F 6D 41 74 74 72 69 62 75 74   // s.CustomAttribut
                                                                                                                 65 73 2B 44 61 74 61 54 79 70 65 2C 43 75 73 74   // es+DataType,Cust
                                                                                                                 6F 6D 41 74 74 72 69 62 75 74 65 73 2E 43 75 73   // omAttributes.Cus
                                                                                                                 74 6F 6D 41 74 74 72 69 62 75 74 65 73 2B 44 61   // tomAttributes+Da
                                                                                                                 74 61 54 79 70 65 5D 00 00 )                      // taType]..
  .field private static int32 typeattr_sometype_of_datatype_and_int
  .custom instance void CustomAttributes.CustomAttributes/TypesAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 80 B4 43 75 73 74 6F 6D 41 74 74 72 69 62   // ....CustomAttrib
                                                                                                                 75 74 65 73 2E 43 75 73 74 6F 6D 41 74 74 72 69   // utes.CustomAttri
                                                                                                                 62 75 74 65 73 2B 53 6F 6D 65 54 79 70 65 60 32   // butes+SomeType`2
                                                                                                                 5B 43 75 73 74 6F 6D 41 74 74 72 69 62 75 74 65   // [CustomAttribute
                                                                                                                 73 2E 43 75 73 74 6F 6D 41 74 74 72 69 62 75 74   // s.CustomAttribut
                                                                                                                 65 73 2B 44 61 74 61 54 79 70 65 2C 5B 53 79 73   // es+DataType,[Sys
                                                                                                                 74 65 6D 2E 49 6E 74 33 32 2C 20 6D 73 63 6F 72   // tem.Int32, mscor
                                                                                                                 6C 69 62 2C 20 56 65 72 73 69 6F 6E 3D 34 2E 30   // lib, Version=4.0
                                                                                                                 2E 30 2E 30 2C 20 43 75 6C 74 75 72 65 3D 6E 65   // .0.0, Culture=ne
                                                                                                                 75 74 72 61 6C 2C 20 50 75 62 6C 69 63 4B 65 79   // utral, PublicKey
                                                                                                                 54 6F 6B 65 6E 3D 62 37 37 61 35 63 35 36 31 39   // Token=b77a5c5619
                                                                                                                 33 34 65 30 38 39 5D 5D 00 00 )                   // 34e089]]..
  .field private static int32 typeattr_sometype_of_datatype_array_and_int
  .custom instance void CustomAttributes.CustomAttributes/TypesAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 80 B6 43 75 73 74 6F 6D 41 74 74 72 69 62   // ....CustomAttrib
                                                                                                                 75 74 65 73 2E 43 75 73 74 6F 6D 41 74 74 72 69   // utes.CustomAttri
                                                                                                                 62 75 74 65 73 2B 53 6F 6D 65 54 79 70 65 60 32   // butes+SomeType`2
                                                                                                                 5B 43 75 73 74 6F 6D 41 74 74 72 69 62 75 74 65   // [CustomAttribute
                                                                                                                 73 2E 43 75 73 74 6F 6D 41 74 74 72 69 62 75 74   // s.CustomAttribut
                                                                                                                 65 73 2B 44 61 74 61 54 79 70 65 5B 5D 2C 5B 53   // es+DataType[],[S
                                                                                                                 79 73 74 65 6D 2E 49 6E 74 33 32 2C 20 6D 73 63   // ystem.Int32, msc
                                                                                                                 6F 72 6C 69 62 2C 20 56 65 72 73 69 6F 6E 3D 34   // orlib, Version=4
                                                                                                                 2E 30 2E 30 2E 30 2C 20 43 75 6C 74 75 72 65 3D   // .0.0.0, Culture=
                                                                                                                 6E 65 75 74 72 61 6C 2C 20 50 75 62 6C 69 63 4B   // neutral, PublicK
                                                                                                                 65 79 54 6F 6B 65 6E 3D 62 37 37 61 35 63 35 36   // eyToken=b77a5c56
                                                                                                                 31 39 33 34 65 30 38 39 5D 5D 00 00 )             // 1934e089]]..
  .field private static int32 typeattr_sometype_of_nested_sometype
  .custom instance void CustomAttributes.CustomAttributes/TypesAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 80 E2 43 75 73 74 6F 6D 41 74 74 72 69 62   // ....CustomAttrib
                                                                                                                 75 74 65 73 2E 43 75 73 74 6F 6D 41 74 74 72 69   // utes.CustomAttri
                                                                                                                 62 75 74 65 73 2B 53 6F 6D 65 54 79 70 65 60 32   // butes+SomeType`2
                                                                                                                 5B 43 75 73 74 6F 6D 41 74 74 72 69 62 75 74 65   // [CustomAttribute
                                                                                                                 73 2E 43 75 73 74 6F 6D 41 74 74 72 69 62 75 74   // s.CustomAttribut
                                                                                                                 65 73 2B 53 6F 6D 65 54 79 70 65 60 31 5B 43 75   // es+SomeType`1[Cu
                                                                                                                 73 74 6F 6D 41 74 74 72 69 62 75 74 65 73 2E 43   // stomAttributes.C
                                                                                                                 75 73 74 6F 6D 41 74 74 72 69 62 75 74 65 73 2B   // ustomAttributes+
                                                                                                                 44 61 74 61 54 79 70 65 5D 2C 5B 53 79 73 74 65   // DataType],[Syste
                                                                                                                 6D 2E 49 6E 74 33 32 2C 20 6D 73 63 6F 72 6C 69   // m.Int32, mscorli
                                                                                                                 62 2C 20 56 65 72 73 69 6F 6E 3D 34 2E 30 2E 30   // b, Version=4.0.0
                                                                                                                 2E 30 2C 20 43 75 6C 74 75 72 65 3D 6E 65 75 74   // .0, Culture=neut
                                                                                                                 72 61 6C 2C 20 50 75 62 6C 69 63 4B 65 79 54 6F   // ral, PublicKeyTo
                                                                                                                 6B 65 6E 3D 62 37 37 61 35 63 35 36 31 39 33 34   // ken=b77a5c561934
                                                                                                                 65 30 38 39 5D 5D 00 00 )                         // e089]]..
  .field private static int32 typeattr_sometype_of_int_and_datatype
  .custom instance void CustomAttributes.CustomAttributes/TypesAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 80 B4 43 75 73 74 6F 6D 41 74 74 72 69 62   // ....CustomAttrib
                                                                                                                 75 74 65 73 2E 43 75 73 74 6F 6D 41 74 74 72 69   // utes.CustomAttri
                                                                                                                 62 75 74 65 73 2B 53 6F 6D 65 54 79 70 65 60 32   // butes+SomeType`2
                                                                                                                 5B 5B 53 79 73 74 65 6D 2E 49 6E 74 33 32 2C 20   // [[System.Int32, 
                                                                                                                 6D 73 63 6F 72 6C 69 62 2C 20 56 65 72 73 69 6F   // mscorlib, Versio
                                                                                                                 6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75 6C 74 75   // n=4.0.0.0, Cultu
                                                                                                                 72 65 3D 6E 65 75 74 72 61 6C 2C 20 50 75 62 6C   // re=neutral, Publ
                                                                                                                 69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61 35   // icKeyToken=b77a5
                                                                                                                 63 35 36 31 39 33 34 65 30 38 39 5D 2C 43 75 73   // c561934e089],Cus
                                                                                                                 74 6F 6D 41 74 74 72 69 62 75 74 65 73 2E 43 75   // tomAttributes.Cu
                                                                                                                 73 74 6F 6D 41 74 74 72 69 62 75 74 65 73 2B 44   // stomAttributes+D
                                                                                                                 61 74 61 54 79 70 65 5D 00 00 )                   // ataType]..
  .field private static int32 typeattr_array_of_int
  .custom instance void CustomAttributes.CustomAttributes/TypesAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 5B 53 79 73 74 65 6D 2E 49 6E 74 33 32 5B   // ..[System.Int32[
                                                                                                                 5D 2C 20 6D 73 63 6F 72 6C 69 62 2C 20 56 65 72   // ], mscorlib, Ver
                                                                                                                 73 69 6F 6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75   // sion=4.0.0.0, Cu
                                                                                                                 6C 74 75 72 65 3D 6E 65 75 74 72 61 6C 2C 20 50   // lture=neutral, P
                                                                                                                 75 62 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37   // ublicKeyToken=b7
                                                                                                                 37 61 35 63 35 36 31 39 33 34 65 30 38 39 00 00 ) // 7a5c561934e089..
  .field private static int32 typeattr_multidim_array_of_int
  .custom instance void CustomAttributes.CustomAttributes/TypesAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 61 53 79 73 74 65 6D 2E 49 6E 74 33 32 5B   // ..aSystem.Int32[
                                                                                                                 2C 5D 5B 2C 2C 2C 5D 2C 20 6D 73 63 6F 72 6C 69   // ,][,,,], mscorli
                                                                                                                 62 2C 20 56 65 72 73 69 6F 6E 3D 34 2E 30 2E 30   // b, Version=4.0.0
                                                                                                                 2E 30 2C 20 43 75 6C 74 75 72 65 3D 6E 65 75 74   // .0, Culture=neut
                                                                                                                 72 61 6C 2C 20 50 75 62 6C 69 63 4B 65 79 54 6F   // ral, PublicKeyTo
                                                                                                                 6B 65 6E 3D 62 37 37 61 35 63 35 36 31 39 33 34   // ken=b77a5c561934
                                                                                                                 65 30 38 39 00 00 )                               // e089..
  .field private static int32 'field'
  .custom instance void CustomAttributes.CustomAttributes/MyAttribute::.ctor(object) = ( 01 00 55 2E 43 75 73 74 6F 6D 41 74 74 72 69 62   // ..U.CustomAttrib
                                                                                         75 74 65 73 2E 43 75 73 74 6F 6D 41 74 74 72 69   // utes.CustomAttri
                                                                                         62 75 74 65 73 2B 45 6E 75 6D 57 69 74 68 46 6C   // butes+EnumWithFl
                                                                                         61 67 03 00 00 00 00 00 )                         // ag......
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
  } // end of method CustomAttributes::get_Property

  .method public hidebysig static void  ObsoletedMethod() cil managed
  {
    .custom instance void [mscorlib]System.ObsoleteAttribute::.ctor(string) = ( 01 00 0C 73 6F 6D 65 20 6D 65 73 73 61 67 65 00   // ...some message.
                                                                                00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method CustomAttributes::ObsoletedMethod

  .method public hidebysig static void  EnumArray() cil managed
  {
    .custom instance void CustomAttributes.CustomAttributes/MyAttribute::.ctor(object) = ( 01 00 1D 55 64 53 79 73 74 65 6D 2E 53 74 72 69   // ...UdSystem.Stri
                                                                                           6E 67 43 6F 6D 70 61 72 69 73 6F 6E 2C 20 6D 73   // ngComparison, ms
                                                                                           63 6F 72 6C 69 62 2C 20 56 65 72 73 69 6F 6E 3D   // corlib, Version=
                                                                                           34 2E 30 2E 30 2E 30 2C 20 43 75 6C 74 75 72 65   // 4.0.0.0, Culture
                                                                                           3D 6E 65 75 74 72 61 6C 2C 20 50 75 62 6C 69 63   // =neutral, Public
                                                                                           4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61 35 63 35   // KeyToken=b77a5c5
                                                                                           36 31 39 33 34 65 30 38 39 02 00 00 00 04 00 00   // 61934e089.......
                                                                                           00 00 00 00 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method CustomAttributes::EnumArray

  .method public hidebysig static void  BoxedEnumArray() cil managed
  {
    .custom instance void CustomAttributes.CustomAttributes/MyAttribute::.ctor(object) = ( 01 00 1D 51 02 00 00 00 55 64 53 79 73 74 65 6D   // ...Q....UdSystem
                                                                                           2E 53 74 72 69 6E 67 43 6F 6D 70 61 72 69 73 6F   // .StringCompariso
                                                                                           6E 2C 20 6D 73 63 6F 72 6C 69 62 2C 20 56 65 72   // n, mscorlib, Ver
                                                                                           73 69 6F 6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75   // sion=4.0.0.0, Cu
                                                                                           6C 74 75 72 65 3D 6E 65 75 74 72 61 6C 2C 20 50   // lture=neutral, P
                                                                                           75 62 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37   // ublicKeyToken=b7
                                                                                           37 61 35 63 35 36 31 39 33 34 65 30 38 39 04 00   // 7a5c561934e089..
                                                                                           00 00 55 64 53 79 73 74 65 6D 2E 53 74 72 69 6E   // ..UdSystem.Strin
                                                                                           67 43 6F 6D 70 61 72 69 73 6F 6E 2C 20 6D 73 63   // gComparison, msc
                                                                                           6F 72 6C 69 62 2C 20 56 65 72 73 69 6F 6E 3D 34   // orlib, Version=4
                                                                                           2E 30 2E 30 2E 30 2C 20 43 75 6C 74 75 72 65 3D   // .0.0.0, Culture=
                                                                                           6E 65 75 74 72 61 6C 2C 20 50 75 62 6C 69 63 4B   // neutral, PublicK
                                                                                           65 79 54 6F 6B 65 6E 3D 62 37 37 61 35 63 35 36   // eyToken=b77a5c56
                                                                                           31 39 33 34 65 30 38 39 00 00 00 00 00 00 )       // 1934e089......
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method CustomAttributes::BoxedEnumArray

  .method public hidebysig static void  BoxedLiteralsArray() cil managed
  {
    .custom instance void CustomAttributes.CustomAttributes/MyAttribute::.ctor(object) = ( 01 00 1D 51 13 00 00 00 08 01 00 00 00 09 02 00   // ...Q............
                                                                                           00 00 0A 03 00 00 00 00 00 00 00 0B 04 00 00 00 
                                                                                           00 00 00 00 06 05 00 07 06 00 05 07 04 08 03 61   // ...............a
                                                                                           00 03 00 00 03 FF FE 03 FF FF 0C 00 00 80 3F 0D   // ..............?.
                                                                                           00 00 00 00 00 00 00 40 0E 04 74 65 78 74 0E FF   // .......@..text..
                                                                                           50 59 53 79 73 74 65 6D 2E 49 6E 74 33 32 2C 20   // PYSystem.Int32, 
                                                                                           6D 73 63 6F 72 6C 69 62 2C 20 56 65 72 73 69 6F   // mscorlib, Versio
                                                                                           6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75 6C 74 75   // n=4.0.0.0, Cultu
                                                                                           72 65 3D 6E 65 75 74 72 61 6C 2C 20 50 75 62 6C   // re=neutral, Publ
                                                                                           69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61 35   // icKeyToken=b77a5
                                                                                           63 35 36 31 39 33 34 65 30 38 39 1D 51 01 00 00   // c561934e089.Q...
                                                                                           00 08 01 00 00 00 1D 08 01 00 00 00 01 00 00 00 
                                                                                           00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method CustomAttributes::BoxedLiteralsArray

  .property string Property()
  {
    .custom instance void CustomAttributes.CustomAttributes/MyAttribute::.ctor(object) = ( 01 00 55 2E 43 75 73 74 6F 6D 41 74 74 72 69 62   // ..U.CustomAttrib
                                                                                           75 74 65 73 2E 43 75 73 74 6F 6D 41 74 74 72 69   // utes.CustomAttri
                                                                                           62 75 74 65 73 2B 45 6E 75 6D 57 69 74 68 46 6C   // butes+EnumWithFl
                                                                                           61 67 0F 00 00 00 00 00 )                         // ag......
    .get string CustomAttributes.CustomAttributes::get_Property()
  } // end of property CustomAttributes::Property
} // end of class CustomAttributes.CustomAttributes


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
