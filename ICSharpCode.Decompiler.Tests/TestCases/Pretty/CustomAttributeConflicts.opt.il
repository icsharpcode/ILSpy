



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly CustomAttributeConflicts.opt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module CustomAttributeConflicts.opt.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit CustomAttributeConflicts.AttributeWithSameNameAsNormalType
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method AttributeWithSameNameAsNormalType::.ctor

} // end of class CustomAttributeConflicts.AttributeWithSameNameAsNormalType

.class private auto ansi beforefieldinit CustomAttributeConflicts.TestClass
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void 
          Test1() cil managed
  {
    .custom instance void CustomAttributeConflicts.NS1.OtherAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TestClass::Test1

  .method public hidebysig instance void 
          Test2() cil managed
  {
    .custom instance void CustomAttributeConflicts.NS1.SimpleAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TestClass::Test2

  .method public hidebysig instance void 
          Test3() cil managed
  {
    .custom instance void CustomAttributeConflicts.NS2.SimpleAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TestClass::Test3

  .method public hidebysig instance void 
          Test4() cil managed
  {
    .custom instance void CustomAttributeConflicts.NS1.AttributeWithSameNameAsNormalType::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TestClass::Test4

  .method public hidebysig instance void 
          Test5() cil managed
  {
    .custom instance void CustomAttributeConflicts.NSWithConflictingTypes.My::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TestClass::Test5

  .method public hidebysig instance void 
          Test6() cil managed
  {
    .custom instance void CustomAttributeConflicts.NSWithConflictingTypes.MyAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TestClass::Test6

  .method public hidebysig instance void 
          Test7() cil managed
  {
    .custom instance void CustomAttributeConflicts.NSWithConflictingTypes2.MyOther::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TestClass::Test7

  .method public hidebysig instance void 
          Test8() cil managed
  {
    .custom instance void CustomAttributeConflicts.NSWithConflictingTypes2.MyOtherAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TestClass::Test8

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method TestClass::.ctor

} // end of class CustomAttributeConflicts.TestClass

.class private auto ansi beforefieldinit CustomAttributeConflicts.NS1.OtherAttribute
       extends [mscorlib]System.Attribute
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method OtherAttribute::.ctor

} // end of class CustomAttributeConflicts.NS1.OtherAttribute

.class private auto ansi beforefieldinit CustomAttributeConflicts.NS1.SimpleAttribute
       extends [mscorlib]System.Attribute
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method SimpleAttribute::.ctor

} // end of class CustomAttributeConflicts.NS1.SimpleAttribute

.class private auto ansi beforefieldinit CustomAttributeConflicts.NS2.SimpleAttribute
       extends [mscorlib]System.Attribute
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method SimpleAttribute::.ctor

} // end of class CustomAttributeConflicts.NS2.SimpleAttribute

.class private auto ansi beforefieldinit CustomAttributeConflicts.NS1.AttributeWithSameNameAsNormalType
       extends [mscorlib]System.Attribute
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method AttributeWithSameNameAsNormalType::.ctor

} // end of class CustomAttributeConflicts.NS1.AttributeWithSameNameAsNormalType

.class private auto ansi beforefieldinit CustomAttributeConflicts.NSWithConflictingTypes.My
       extends [mscorlib]System.Attribute
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method My::.ctor

} // end of class CustomAttributeConflicts.NSWithConflictingTypes.My

.class private auto ansi beforefieldinit CustomAttributeConflicts.NSWithConflictingTypes.MyAttribute
       extends [mscorlib]System.Attribute
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyAttribute::.ctor

} // end of class CustomAttributeConflicts.NSWithConflictingTypes.MyAttribute

.class private auto ansi beforefieldinit CustomAttributeConflicts.NSWithConflictingTypes2.MyOther
       extends [mscorlib]System.Attribute
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyOther::.ctor

} // end of class CustomAttributeConflicts.NSWithConflictingTypes2.MyOther

.class private auto ansi beforefieldinit CustomAttributeConflicts.NSWithConflictingTypes2.MyOtherAttribute
       extends [mscorlib]System.Attribute
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyOtherAttribute::.ctor

} // end of class CustomAttributeConflicts.NSWithConflictingTypes2.MyOtherAttribute

.class private auto ansi beforefieldinit CustomAttributeConflicts.NSWithConflictingTypes.MyAttributeAttribute
       extends [mscorlib]System.Attribute
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyAttributeAttribute::.ctor

} // end of class CustomAttributeConflicts.NSWithConflictingTypes.MyAttributeAttribute

.class private auto ansi beforefieldinit CustomAttributeConflicts.NSWithConflictingTypes.MyOther
       extends [mscorlib]System.Attribute
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyOther::.ctor

} // end of class CustomAttributeConflicts.NSWithConflictingTypes.MyOther

.class private auto ansi beforefieldinit CustomAttributeConflicts.NSWithConflictingTypes.MyOtherAttribute
       extends [mscorlib]System.Attribute
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyOtherAttribute::.ctor

} // end of class CustomAttributeConflicts.NSWithConflictingTypes.MyOtherAttribute

.class private auto ansi beforefieldinit CustomAttributeConflicts.NSWithConflictingTypes.MyOtherAttributeAttribute
       extends [mscorlib]System.Attribute
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyOtherAttributeAttribute::.ctor

} // end of class CustomAttributeConflicts.NSWithConflictingTypes.MyOtherAttributeAttribute

.class private auto ansi beforefieldinit CustomAttributeConflicts.NSWithConflictingTypes2.MyOtherAttributeAttribute
       extends [mscorlib]System.Attribute
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyOtherAttributeAttribute::.ctor

} // end of class CustomAttributeConflicts.NSWithConflictingTypes2.MyOtherAttributeAttribute


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
