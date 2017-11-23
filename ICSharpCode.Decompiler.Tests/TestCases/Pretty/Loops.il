
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly '50bjvac5'
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module '50bjvac5.dll'
// MVID: {7EA9F0A8-B896-4F84-BCD8-AC7B4D44BBC0}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03450000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops
       extends [mscorlib]System.Object
{
  .class auto ansi nested public beforefieldinit CustomClassEnumerator
         extends [mscorlib]System.Object
  {
    .method public hidebysig specialname 
            instance object  get_Current() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumerator::get_Current

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumerator::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumerator::Reset

    .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator 
            GetEnumerator() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
    } // end of method CustomClassEnumerator::GetEnumerator

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method CustomClassEnumerator::.ctor

    .property instance object Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator::get_Current()
    } // end of property CustomClassEnumerator::Current
  } // end of class CustomClassEnumerator

  .class sequential ansi sealed nested public beforefieldinit CustomStructEnumerator
         extends [mscorlib]System.ValueType
  {
    .pack 0
    .size 1
    .method public hidebysig specialname 
            instance object  get_Current() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumerator::get_Current

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumerator::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumerator::Reset

    .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumerator 
            GetEnumerator() cil managed
    {
      // Code size       12 (0xc)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumerator V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumerator
      IL_0007:  stloc.0
      IL_0008:  br.s       IL_000a

      IL_000a:  ldloc.0
      IL_000b:  ret
    } // end of method CustomStructEnumerator::GetEnumerator

    .property instance object Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumerator::get_Current()
    } // end of property CustomStructEnumerator::Current
  } // end of class CustomStructEnumerator

  .class auto ansi nested public beforefieldinit CustomClassEnumerator`1<T>
         extends [mscorlib]System.Object
  {
    .method public hidebysig specialname 
            instance !T  get_Current() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumerator`1::get_Current

    .method public hidebysig instance void 
            Dispose() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumerator`1::Dispose

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumerator`1::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumerator`1::Reset

    .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!T> 
            GetEnumerator() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!T> V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
    } // end of method CustomClassEnumerator`1::GetEnumerator

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method CustomClassEnumerator`1::.ctor

    .property instance !T Current()
    {
      .get instance !T ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1::get_Current()
    } // end of property CustomClassEnumerator`1::Current
  } // end of class CustomClassEnumerator`1

  .class sequential ansi sealed nested public beforefieldinit CustomStructEnumerator`1<T>
         extends [mscorlib]System.ValueType
  {
    .pack 0
    .size 1
    .method public hidebysig specialname 
            instance !T  get_Current() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumerator`1::get_Current

    .method public hidebysig instance void 
            Dispose() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumerator`1::Dispose

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumerator`1::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumerator`1::Reset

    .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumerator`1<!T> 
            GetEnumerator() cil managed
    {
      // Code size       12 (0xc)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumerator`1<!T> V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldobj      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumerator`1<!T>
      IL_0007:  stloc.0
      IL_0008:  br.s       IL_000a

      IL_000a:  ldloc.0
      IL_000b:  ret
    } // end of method CustomStructEnumerator`1::GetEnumerator

    .property instance !T Current()
    {
      .get instance !T ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumerator`1::get_Current()
    } // end of property CustomStructEnumerator`1::Current
  } // end of class CustomStructEnumerator`1

  .class auto ansi nested public beforefieldinit CustomClassEnumeratorWithIDisposable
         extends [mscorlib]System.Object
         implements [mscorlib]System.IDisposable
  {
    .method public hidebysig specialname 
            instance object  get_Current() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumeratorWithIDisposable::get_Current

    .method public hidebysig newslot virtual final 
            instance void  Dispose() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumeratorWithIDisposable::Dispose

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumeratorWithIDisposable::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumeratorWithIDisposable::Reset

    .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable 
            GetEnumerator() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
    } // end of method CustomClassEnumeratorWithIDisposable::GetEnumerator

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method CustomClassEnumeratorWithIDisposable::.ctor

    .property instance object Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable::get_Current()
    } // end of property CustomClassEnumeratorWithIDisposable::Current
  } // end of class CustomClassEnumeratorWithIDisposable

  .class sequential ansi sealed nested public beforefieldinit CustomStructEnumeratorWithIDisposable
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.IDisposable
  {
    .pack 0
    .size 1
    .method public hidebysig specialname 
            instance object  get_Current() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumeratorWithIDisposable::get_Current

    .method public hidebysig newslot virtual final 
            instance void  Dispose() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumeratorWithIDisposable::Dispose

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumeratorWithIDisposable::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumeratorWithIDisposable::Reset

    .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable 
            GetEnumerator() cil managed
    {
      // Code size       12 (0xc)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable
      IL_0007:  stloc.0
      IL_0008:  br.s       IL_000a

      IL_000a:  ldloc.0
      IL_000b:  ret
    } // end of method CustomStructEnumeratorWithIDisposable::GetEnumerator

    .property instance object Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable::get_Current()
    } // end of property CustomStructEnumeratorWithIDisposable::Current
  } // end of class CustomStructEnumeratorWithIDisposable

  .class auto ansi nested public beforefieldinit CustomClassEnumeratorWithIDisposable`1<T>
         extends [mscorlib]System.Object
         implements [mscorlib]System.IDisposable
  {
    .method public hidebysig specialname 
            instance !T  get_Current() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumeratorWithIDisposable`1::get_Current

    .method public hidebysig newslot virtual final 
            instance void  Dispose() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumeratorWithIDisposable`1::Dispose

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumeratorWithIDisposable`1::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomClassEnumeratorWithIDisposable`1::Reset

    .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!T> 
            GetEnumerator() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!T> V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
    } // end of method CustomClassEnumeratorWithIDisposable`1::GetEnumerator

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method CustomClassEnumeratorWithIDisposable`1::.ctor

    .property instance !T Current()
    {
      .get instance !T ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1::get_Current()
    } // end of property CustomClassEnumeratorWithIDisposable`1::Current
  } // end of class CustomClassEnumeratorWithIDisposable`1

  .class sequential ansi sealed nested public beforefieldinit CustomStructEnumeratorWithIDisposable`1<T>
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.IDisposable
  {
    .pack 0
    .size 1
    .method public hidebysig specialname 
            instance !T  get_Current() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumeratorWithIDisposable`1::get_Current

    .method public hidebysig newslot virtual final 
            instance void  Dispose() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumeratorWithIDisposable`1::Dispose

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumeratorWithIDisposable`1::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomStructEnumeratorWithIDisposable`1::Reset

    .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!T> 
            GetEnumerator() cil managed
    {
      // Code size       12 (0xc)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!T> V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldobj      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!T>
      IL_0007:  stloc.0
      IL_0008:  br.s       IL_000a

      IL_000a:  ldloc.0
      IL_000b:  ret
    } // end of method CustomStructEnumeratorWithIDisposable`1::GetEnumerator

    .property instance !T Current()
    {
      .get instance !T ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1::get_Current()
    } // end of property CustomStructEnumeratorWithIDisposable`1::Current
  } // end of class CustomStructEnumeratorWithIDisposable`1

  .class sequential ansi sealed nested public beforefieldinit DataItem
         extends [mscorlib]System.ValueType
  {
    .field private int32 '<Property>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname 
            instance int32  get_Property() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem::'<Property>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method DataItem::get_Property

    .method public hidebysig specialname 
            instance void  set_Property(int32 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem::'<Property>k__BackingField'
      IL_0007:  ret
    } // end of method DataItem::set_Property

    .method public hidebysig instance void 
            TestCall() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method DataItem::TestCall

    .property instance int32 Property()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem::get_Property()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem::set_Property(int32)
    } // end of property DataItem::Property
  } // end of class DataItem

  .class auto ansi nested public beforefieldinit Item
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
    } // end of method Item::.ctor

  } // end of class Item

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass1'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 c
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass1'::.ctor

    .method public hidebysig instance bool 
            '<ForeachWithCapturedVariable>b__0'() cil managed
    {
      // Code size       14 (0xe)
      .maxstack  2
      .locals init (bool V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/'<>c__DisplayClass1'::c
      IL_0006:  ldc.i4.5
      IL_0007:  ceq
      IL_0009:  stloc.0
      IL_000a:  br.s       IL_000c

      IL_000c:  ldloc.0
      IL_000d:  ret
    } // end of method '<>c__DisplayClass1'::'<ForeachWithCapturedVariable>b__0'

  } // end of class '<>c__DisplayClass1'

  .field private class [mscorlib]System.Collections.Generic.IEnumerable`1<string> alternatives
  .field private object someObject
  .method private hidebysig instance void 
          TryGetItem(int32 id,
                     [out] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/Item& item) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  ldnull
    IL_0003:  stind.ref
    IL_0004:  ret
  } // end of method Loops::TryGetItem

  .method private hidebysig static void  Operation(int32& item) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method Loops::Operation

  .method private hidebysig static void  Operation(class [mscorlib]System.Func`1<bool> f) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method Loops::Operation

  .method public hidebysig instance void 
          ForEachOnField() cil managed
  {
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (string V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<string> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.0
    IL_0003:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::alternatives
    IL_0008:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
    IL_000d:  stloc.1
    .try
    {
      IL_000e:  br.s       IL_0020

      IL_0010:  ldloc.1
      IL_0011:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0016:  stloc.0
      IL_0017:  nop
      IL_0018:  ldloc.0
      IL_0019:  callvirt   instance string [mscorlib]System.String::ToLower()
      IL_001e:  pop
      IL_001f:  nop
      IL_0020:  ldloc.1
      IL_0021:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0026:  stloc.2
      IL_0027:  ldloc.2
      IL_0028:  brtrue.s   IL_0010

      IL_002a:  leave.s    IL_003c

    }  // end .try
    finally
    {
      IL_002c:  ldloc.1
      IL_002d:  ldnull
      IL_002e:  ceq
      IL_0030:  stloc.2
      IL_0031:  ldloc.2
      IL_0032:  brtrue.s   IL_003b

      IL_0034:  ldloc.1
      IL_0035:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_003a:  nop
      IL_003b:  endfinally
    }  // end handler
    IL_003c:  nop
    IL_003d:  ret
  } // end of method Loops::ForEachOnField

  .method public hidebysig instance void 
          ForEach(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> alternatives) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  2
    .locals init (string V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<string> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_001b

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0011:  stloc.0
      IL_0012:  nop
      IL_0013:  ldloc.0
      IL_0014:  callvirt   instance string [mscorlib]System.String::ToLower()
      IL_0019:  pop
      IL_001a:  nop
      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0021:  stloc.2
      IL_0022:  ldloc.2
      IL_0023:  brtrue.s   IL_000b

      IL_0025:  leave.s    IL_0037

    }  // end .try
    finally
    {
      IL_0027:  ldloc.1
      IL_0028:  ldnull
      IL_0029:  ceq
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  brtrue.s   IL_0036

      IL_002f:  ldloc.1
      IL_0030:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0035:  nop
      IL_0036:  endfinally
    }  // end handler
    IL_0037:  nop
    IL_0038:  ret
  } // end of method Loops::ForEach

  .method public hidebysig instance void 
          ForEachOverList(class [mscorlib]System.Collections.Generic.List`1<string> list) cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  1
    .locals init (string V_0,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<string>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_001c

      IL_000b:  ldloca.s   V_1
      IL_000d:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::get_Current()
      IL_0012:  stloc.0
      IL_0013:  nop
      IL_0014:  ldloc.0
      IL_0015:  callvirt   instance string [mscorlib]System.String::ToLower()
      IL_001a:  pop
      IL_001b:  nop
      IL_001c:  ldloca.s   V_1
      IL_001e:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::MoveNext()
      IL_0023:  stloc.2
      IL_0024:  ldloc.2
      IL_0025:  brtrue.s   IL_000b

      IL_0027:  leave.s    IL_0038

    }  // end .try
    finally
    {
      IL_0029:  ldloca.s   V_1
      IL_002b:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>
      IL_0031:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0036:  nop
      IL_0037:  endfinally
    }  // end handler
    IL_0038:  nop
    IL_0039:  ret
  } // end of method Loops::ForEachOverList

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerable(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       64 (0x40)
    .maxstack  2
    .locals init (object V_0,
             class [mscorlib]System.Collections.IEnumerator V_1,
             bool V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_001b

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0011:  stloc.0
      IL_0012:  nop
      IL_0013:  ldloc.0
      IL_0014:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0019:  pop
      IL_001a:  nop
      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0021:  stloc.2
      IL_0022:  ldloc.2
      IL_0023:  brtrue.s   IL_000b

      IL_0025:  leave.s    IL_003e

    }  // end .try
    finally
    {
      IL_0027:  ldloc.1
      IL_0028:  isinst     [mscorlib]System.IDisposable
      IL_002d:  stloc.3
      IL_002e:  ldloc.3
      IL_002f:  ldnull
      IL_0030:  ceq
      IL_0032:  stloc.2
      IL_0033:  ldloc.2
      IL_0034:  brtrue.s   IL_003d

      IL_0036:  ldloc.3
      IL_0037:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_003c:  nop
      IL_003d:  endfinally
    }  // end handler
    IL_003e:  nop
    IL_003f:  ret
  } // end of method Loops::ForEachOverNonGenericEnumerable

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerableWithAutomaticCastValueType(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       70 (0x46)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.IEnumerator V_1,
             bool V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_0021

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0011:  unbox.any  [mscorlib]System.Int32
      IL_0016:  stloc.0
      IL_0017:  nop
      IL_0018:  ldloca.s   V_0
      IL_001a:  call       instance string [mscorlib]System.Int32::ToString()
      IL_001f:  pop
      IL_0020:  nop
      IL_0021:  ldloc.1
      IL_0022:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0027:  stloc.2
      IL_0028:  ldloc.2
      IL_0029:  brtrue.s   IL_000b

      IL_002b:  leave.s    IL_0044

    }  // end .try
    finally
    {
      IL_002d:  ldloc.1
      IL_002e:  isinst     [mscorlib]System.IDisposable
      IL_0033:  stloc.3
      IL_0034:  ldloc.3
      IL_0035:  ldnull
      IL_0036:  ceq
      IL_0038:  stloc.2
      IL_0039:  ldloc.2
      IL_003a:  brtrue.s   IL_0043

      IL_003c:  ldloc.3
      IL_003d:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0042:  nop
      IL_0043:  endfinally
    }  // end handler
    IL_0044:  nop
    IL_0045:  ret
  } // end of method Loops::ForEachOverNonGenericEnumerableWithAutomaticCastValueType

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerableWithAutomaticCastRefType(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       69 (0x45)
    .maxstack  2
    .locals init (string V_0,
             class [mscorlib]System.Collections.IEnumerator V_1,
             bool V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_0020

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0011:  castclass  [mscorlib]System.String
      IL_0016:  stloc.0
      IL_0017:  nop
      IL_0018:  ldloc.0
      IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001e:  nop
      IL_001f:  nop
      IL_0020:  ldloc.1
      IL_0021:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0026:  stloc.2
      IL_0027:  ldloc.2
      IL_0028:  brtrue.s   IL_000b

      IL_002a:  leave.s    IL_0043

    }  // end .try
    finally
    {
      IL_002c:  ldloc.1
      IL_002d:  isinst     [mscorlib]System.IDisposable
      IL_0032:  stloc.3
      IL_0033:  ldloc.3
      IL_0034:  ldnull
      IL_0035:  ceq
      IL_0037:  stloc.2
      IL_0038:  ldloc.2
      IL_0039:  brtrue.s   IL_0042

      IL_003b:  ldloc.3
      IL_003c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0041:  nop
      IL_0042:  endfinally
    }  // end handler
    IL_0043:  nop
    IL_0044:  ret
  } // end of method Loops::ForEachOverNonGenericEnumerableWithAutomaticCastRefType

  .method public hidebysig instance void 
          ForEachOnCustomClassEnumerator(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator e) cil managed
  {
    // Code size       64 (0x40)
    .maxstack  2
    .locals init (object V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator V_1,
             bool V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_001b

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator::get_Current()
      IL_0011:  stloc.0
      IL_0012:  nop
      IL_0013:  ldloc.0
      IL_0014:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_0019:  nop
      IL_001a:  nop
      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator::MoveNext()
      IL_0021:  stloc.2
      IL_0022:  ldloc.2
      IL_0023:  brtrue.s   IL_000b

      IL_0025:  leave.s    IL_003e

    }  // end .try
    finally
    {
      IL_0027:  ldloc.1
      IL_0028:  isinst     [mscorlib]System.IDisposable
      IL_002d:  stloc.3
      IL_002e:  ldloc.3
      IL_002f:  ldnull
      IL_0030:  ceq
      IL_0032:  stloc.2
      IL_0033:  ldloc.2
      IL_0034:  brtrue.s   IL_003d

      IL_0036:  ldloc.3
      IL_0037:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_003c:  nop
      IL_003d:  endfinally
    }  // end handler
    IL_003e:  nop
    IL_003f:  ret
  } // end of method Loops::ForEachOnCustomClassEnumerator

  .method public hidebysig instance void 
          ForEachOnGenericCustomClassEnumerator<T>(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!!T> e) cil managed
  {
    // Code size       69 (0x45)
    .maxstack  2
    .locals init (!!T V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!!T> V_1,
             bool V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!0> class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!!T>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_0020

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!!T>::get_Current()
      IL_0011:  stloc.0
      IL_0012:  nop
      IL_0013:  ldloc.0
      IL_0014:  box        !!T
      IL_0019:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_001e:  nop
      IL_001f:  nop
      IL_0020:  ldloc.1
      IL_0021:  callvirt   instance bool class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!!T>::MoveNext()
      IL_0026:  stloc.2
      IL_0027:  ldloc.2
      IL_0028:  brtrue.s   IL_000b

      IL_002a:  leave.s    IL_0043

    }  // end .try
    finally
    {
      IL_002c:  ldloc.1
      IL_002d:  isinst     [mscorlib]System.IDisposable
      IL_0032:  stloc.3
      IL_0033:  ldloc.3
      IL_0034:  ldnull
      IL_0035:  ceq
      IL_0037:  stloc.2
      IL_0038:  ldloc.2
      IL_0039:  brtrue.s   IL_0042

      IL_003b:  ldloc.3
      IL_003c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0041:  nop
      IL_0042:  endfinally
    }  // end handler
    IL_0043:  nop
    IL_0044:  ret
  } // end of method Loops::ForEachOnGenericCustomClassEnumerator

  .method public hidebysig instance void 
          ForEachOnCustomClassEnumeratorWithIDisposable(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable e) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  2
    .locals init (object V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_001b

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable::get_Current()
      IL_0011:  stloc.0
      IL_0012:  nop
      IL_0013:  ldloc.0
      IL_0014:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_0019:  nop
      IL_001a:  nop
      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable::MoveNext()
      IL_0021:  stloc.2
      IL_0022:  ldloc.2
      IL_0023:  brtrue.s   IL_000b

      IL_0025:  leave.s    IL_0037

    }  // end .try
    finally
    {
      IL_0027:  ldloc.1
      IL_0028:  ldnull
      IL_0029:  ceq
      IL_002b:  stloc.2
      IL_002c:  ldloc.2
      IL_002d:  brtrue.s   IL_0036

      IL_002f:  ldloc.1
      IL_0030:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0035:  nop
      IL_0036:  endfinally
    }  // end handler
    IL_0037:  nop
    IL_0038:  ret
  } // end of method Loops::ForEachOnCustomClassEnumeratorWithIDisposable

  .method public hidebysig instance void 
          ForEachOnCustomStructEnumeratorWithIDisposable(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable e) cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  1
    .locals init (object V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarga.s   e
    IL_0004:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable::GetEnumerator()
    IL_0009:  stloc.1
    .try
    {
      IL_000a:  br.s       IL_001d

      IL_000c:  ldloca.s   V_1
      IL_000e:  call       instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable::get_Current()
      IL_0013:  stloc.0
      IL_0014:  nop
      IL_0015:  ldloc.0
      IL_0016:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_001b:  nop
      IL_001c:  nop
      IL_001d:  ldloca.s   V_1
      IL_001f:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable::MoveNext()
      IL_0024:  stloc.2
      IL_0025:  ldloc.2
      IL_0026:  brtrue.s   IL_000c

      IL_0028:  leave.s    IL_0039

    }  // end .try
    finally
    {
      IL_002a:  ldloca.s   V_1
      IL_002c:  constrained. ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable
      IL_0032:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0037:  nop
      IL_0038:  endfinally
    }  // end handler
    IL_0039:  nop
    IL_003a:  ret
  } // end of method Loops::ForEachOnCustomStructEnumeratorWithIDisposable

  .method public hidebysig instance void 
          ForEachOnGenericCustomClassEnumeratorWithIDisposable<T>(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!!T> e) cil managed
  {
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (!!T V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!!T> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!0> class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!!T>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_0020

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!!T>::get_Current()
      IL_0011:  stloc.0
      IL_0012:  nop
      IL_0013:  ldloc.0
      IL_0014:  box        !!T
      IL_0019:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_001e:  nop
      IL_001f:  nop
      IL_0020:  ldloc.1
      IL_0021:  callvirt   instance bool class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!!T>::MoveNext()
      IL_0026:  stloc.2
      IL_0027:  ldloc.2
      IL_0028:  brtrue.s   IL_000b

      IL_002a:  leave.s    IL_003c

    }  // end .try
    finally
    {
      IL_002c:  ldloc.1
      IL_002d:  ldnull
      IL_002e:  ceq
      IL_0030:  stloc.2
      IL_0031:  ldloc.2
      IL_0032:  brtrue.s   IL_003b

      IL_0034:  ldloc.1
      IL_0035:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_003a:  nop
      IL_003b:  endfinally
    }  // end handler
    IL_003c:  nop
    IL_003d:  ret
  } // end of method Loops::ForEachOnGenericCustomClassEnumeratorWithIDisposable

  .method public hidebysig instance void 
          ForEachOnGenericCustomStructEnumeratorWithIDisposable<T>(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!!T> e) cil managed
  {
    // Code size       64 (0x40)
    .maxstack  1
    .locals init (!!T V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!!T> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarga.s   e
    IL_0004:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!0> valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!!T>::GetEnumerator()
    IL_0009:  stloc.1
    .try
    {
      IL_000a:  br.s       IL_0022

      IL_000c:  ldloca.s   V_1
      IL_000e:  call       instance !0 valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!!T>::get_Current()
      IL_0013:  stloc.0
      IL_0014:  nop
      IL_0015:  ldloc.0
      IL_0016:  box        !!T
      IL_001b:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_0020:  nop
      IL_0021:  nop
      IL_0022:  ldloca.s   V_1
      IL_0024:  call       instance bool valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!!T>::MoveNext()
      IL_0029:  stloc.2
      IL_002a:  ldloc.2
      IL_002b:  brtrue.s   IL_000c

      IL_002d:  leave.s    IL_003e

    }  // end .try
    finally
    {
      IL_002f:  ldloca.s   V_1
      IL_0031:  constrained. valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!!T>
      IL_0037:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_003c:  nop
      IL_003d:  endfinally
    }  // end handler
    IL_003e:  nop
    IL_003f:  ret
  } // end of method Loops::ForEachOnGenericCustomStructEnumeratorWithIDisposable

  .method public hidebysig static void  NonGenericForeachWithReturnFallbackTest(class [mscorlib]System.Collections.IEnumerable e) cil managed
  {
    // Code size       113 (0x71)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.IEnumerator V_0,
             object V_1,
             class [mscorlib]System.IDisposable V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  ldstr      "NonGenericForeachWithReturnFallback:"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ldarg.0
    IL_000d:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0012:  stloc.0
    .try
    {
      IL_0013:  nop
      IL_0014:  ldstr      "MoveNext"
      IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001e:  nop
      IL_001f:  ldloc.0
      IL_0020:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0025:  ldc.i4.0
      IL_0026:  ceq
      IL_0028:  stloc.3
      IL_0029:  ldloc.3
      IL_002a:  brtrue.s   IL_0046

      IL_002c:  nop
      IL_002d:  ldloc.0
      IL_002e:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0033:  stloc.1
      IL_0034:  ldstr      "current: "
      IL_0039:  ldloc.1
      IL_003a:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_003f:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0044:  nop
      IL_0045:  nop
      IL_0046:  nop
      IL_0047:  leave.s    IL_0064

    }  // end .try
    finally
    {
      IL_0049:  nop
      IL_004a:  ldloc.0
      IL_004b:  isinst     [mscorlib]System.IDisposable
      IL_0050:  stloc.2
      IL_0051:  ldloc.2
      IL_0052:  ldnull
      IL_0053:  ceq
      IL_0055:  stloc.3
      IL_0056:  ldloc.3
      IL_0057:  brtrue.s   IL_0062

      IL_0059:  nop
      IL_005a:  ldloc.2
      IL_005b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0060:  nop
      IL_0061:  nop
      IL_0062:  nop
      IL_0063:  endfinally
    }  // end handler
    IL_0064:  nop
    IL_0065:  ldstr      "After finally!"
    IL_006a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006f:  nop
    IL_0070:  ret
  } // end of method Loops::NonGenericForeachWithReturnFallbackTest

  .method public hidebysig static void  ForeachWithRefUsage(class [mscorlib]System.Collections.Generic.List`1<int32> items) cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  1
    .locals init (int32 V_0,
             int32 V_1,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32> V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.0
    IL_0003:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::GetEnumerator()
    IL_0008:  stloc.2
    .try
    {
      IL_0009:  br.s       IL_001f

      IL_000b:  ldloca.s   V_2
      IL_000d:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::get_Current()
      IL_0012:  stloc.0
      IL_0013:  nop
      IL_0014:  ldloc.0
      IL_0015:  stloc.1
      IL_0016:  ldloca.s   V_1
      IL_0018:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Operation(int32&)
      IL_001d:  nop
      IL_001e:  nop
      IL_001f:  ldloca.s   V_2
      IL_0021:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::MoveNext()
      IL_0026:  stloc.3
      IL_0027:  ldloc.3
      IL_0028:  brtrue.s   IL_000b

      IL_002a:  leave.s    IL_003b

    }  // end .try
    finally
    {
      IL_002c:  ldloca.s   V_2
      IL_002e:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>
      IL_0034:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0039:  nop
      IL_003a:  endfinally
    }  // end handler
    IL_003b:  nop
    IL_003c:  ret
  } // end of method Loops::ForeachWithRefUsage

  .method public hidebysig static void  ForeachWithCapturedVariable(class [mscorlib]System.Collections.Generic.List`1<int32> items) cil managed
  {
    // Code size       82 (0x52)
    .maxstack  2
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/'<>c__DisplayClass1' V_1,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32> V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.0
    IL_0003:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::GetEnumerator()
    IL_0008:  stloc.2
    .try
    {
      IL_0009:  br.s       IL_0034

      IL_000b:  ldloca.s   V_2
      IL_000d:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::get_Current()
      IL_0012:  stloc.0
      IL_0013:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/'<>c__DisplayClass1'::.ctor()
      IL_0018:  stloc.1
      IL_0019:  nop
      IL_001a:  ldloc.1
      IL_001b:  ldloc.0
      IL_001c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/'<>c__DisplayClass1'::c
      IL_0021:  ldloc.1
      IL_0022:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/'<>c__DisplayClass1'::'<ForeachWithCapturedVariable>b__0'()
      IL_0028:  newobj     instance void class [mscorlib]System.Func`1<bool>::.ctor(object,
                                                                                    native int)
      IL_002d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Operation(class [mscorlib]System.Func`1<bool>)
      IL_0032:  nop
      IL_0033:  nop
      IL_0034:  ldloca.s   V_2
      IL_0036:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::MoveNext()
      IL_003b:  stloc.3
      IL_003c:  ldloc.3
      IL_003d:  brtrue.s   IL_000b

      IL_003f:  leave.s    IL_0050

    }  // end .try
    finally
    {
      IL_0041:  ldloca.s   V_2
      IL_0043:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>
      IL_0049:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_004e:  nop
      IL_004f:  endfinally
    }  // end handler
    IL_0050:  nop
    IL_0051:  ret
  } // end of method Loops::ForeachWithCapturedVariable

  .method public hidebysig static !!T  LastOrDefault<T>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!T> items) cil managed
  {
    // Code size       69 (0x45)
    .maxstack  2
    .locals init (!!T V_0,
             !!T V_1,
             !!T V_2,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<!!T> V_3,
             bool V_4)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  initobj    !!T
    IL_0009:  nop
    IL_000a:  ldarg.0
    IL_000b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!!T>::GetEnumerator()
    IL_0010:  stloc.3
    .try
    {
      IL_0011:  br.s       IL_001e

      IL_0013:  ldloc.3
      IL_0014:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!!T>::get_Current()
      IL_0019:  stloc.1
      IL_001a:  nop
      IL_001b:  ldloc.1
      IL_001c:  stloc.0
      IL_001d:  nop
      IL_001e:  ldloc.3
      IL_001f:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0024:  stloc.s    V_4
      IL_0026:  ldloc.s    V_4
      IL_0028:  brtrue.s   IL_0013

      IL_002a:  leave.s    IL_003e

    }  // end .try
    finally
    {
      IL_002c:  ldloc.3
      IL_002d:  ldnull
      IL_002e:  ceq
      IL_0030:  stloc.s    V_4
      IL_0032:  ldloc.s    V_4
      IL_0034:  brtrue.s   IL_003d

      IL_0036:  ldloc.3
      IL_0037:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_003c:  nop
      IL_003d:  endfinally
    }  // end handler
    IL_003e:  nop
    IL_003f:  ldloc.0
    IL_0040:  stloc.2
    IL_0041:  br.s       IL_0043

    IL_0043:  ldloc.2
    IL_0044:  ret
  } // end of method Loops::LastOrDefault

  .method public hidebysig instance void 
          ForEachOverArray(string[] 'array') cil managed
  {
    // Code size       52 (0x34)
    .maxstack  2
    .locals init (string V_0,
             string[] V_1,
             int32 V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldc.i4.0
    IL_0005:  stloc.2
    IL_0006:  br.s       IL_0029

    IL_0008:  ldloc.1
    IL_0009:  ldloc.2
    IL_000a:  ldelem.ref
    IL_000b:  stloc.0
    IL_000c:  nop
    IL_000d:  ldloc.0
    IL_000e:  callvirt   instance string [mscorlib]System.String::ToLower()
    IL_0013:  ldloc.0
    IL_0014:  callvirt   instance string [mscorlib]System.String::ToUpper()
    IL_0019:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0023:  nop
    IL_0024:  nop
    IL_0025:  ldloc.2
    IL_0026:  ldc.i4.1
    IL_0027:  add
    IL_0028:  stloc.2
    IL_0029:  ldloc.2
    IL_002a:  ldloc.1
    IL_002b:  ldlen
    IL_002c:  conv.i4
    IL_002d:  clt
    IL_002f:  stloc.3
    IL_0030:  ldloc.3
    IL_0031:  brtrue.s   IL_0008

    IL_0033:  ret
  } // end of method Loops::ForEachOverArray

  .method public hidebysig instance void 
          ForEachOverArrayOfPointers(int32*[] 'array') cil managed
  {
    // Code size       63 (0x3f)
    .maxstack  2
    .locals init (int32* V_0,
             int32*[] V_1,
             int32 V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldc.i4.0
    IL_0005:  stloc.2
    IL_0006:  br.s       IL_0034

    IL_0008:  ldloc.1
    IL_0009:  ldloc.2
    IL_000a:  ldelem.i
    IL_000b:  stloc.0
    IL_000c:  nop
    IL_000d:  ldloc.0
    IL_000e:  newobj     instance void [mscorlib]System.IntPtr::.ctor(void*)
    IL_0013:  box        [mscorlib]System.IntPtr
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_001d:  nop
    IL_001e:  ldloc.0
    IL_001f:  newobj     instance void [mscorlib]System.IntPtr::.ctor(void*)
    IL_0024:  box        [mscorlib]System.IntPtr
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_002e:  nop
    IL_002f:  nop
    IL_0030:  ldloc.2
    IL_0031:  ldc.i4.1
    IL_0032:  add
    IL_0033:  stloc.2
    IL_0034:  ldloc.2
    IL_0035:  ldloc.1
    IL_0036:  ldlen
    IL_0037:  conv.i4
    IL_0038:  clt
    IL_003a:  stloc.3
    IL_003b:  ldloc.3
    IL_003c:  brtrue.s   IL_0008

    IL_003e:  ret
  } // end of method Loops::ForEachOverArrayOfPointers

  .method public hidebysig instance void 
          ForEachBreakWhenFound(string name,
                                valuetype [mscorlib]System.StringComparison& output) cil managed
  {
    // Code size       106 (0x6a)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.StringComparison V_0,
             class [mscorlib]System.Collections.IEnumerator V_1,
             bool V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldtoken    [mscorlib]System.StringComparison
    IL_0007:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000c:  call       class [mscorlib]System.Array [mscorlib]System.Enum::GetValues(class [mscorlib]System.Type)
    IL_0011:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Array::GetEnumerator()
    IL_0016:  stloc.1
    .try
    {
      IL_0017:  br.s       IL_0045

      IL_0019:  ldloc.1
      IL_001a:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_001f:  unbox.any  [mscorlib]System.StringComparison
      IL_0024:  stloc.0
      IL_0025:  nop
      IL_0026:  ldloc.0
      IL_0027:  box        [mscorlib]System.StringComparison
      IL_002c:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0031:  ldarg.1
      IL_0032:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_0037:  ldc.i4.0
      IL_0038:  ceq
      IL_003a:  stloc.2
      IL_003b:  ldloc.2
      IL_003c:  brtrue.s   IL_0044

      IL_003e:  nop
      IL_003f:  ldarg.2
      IL_0040:  ldloc.0
      IL_0041:  stind.i4
      IL_0042:  br.s       IL_004f

      IL_0044:  nop
      IL_0045:  ldloc.1
      IL_0046:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_004b:  stloc.2
      IL_004c:  ldloc.2
      IL_004d:  brtrue.s   IL_0019

      IL_004f:  leave.s    IL_0068

    }  // end .try
    finally
    {
      IL_0051:  ldloc.1
      IL_0052:  isinst     [mscorlib]System.IDisposable
      IL_0057:  stloc.3
      IL_0058:  ldloc.3
      IL_0059:  ldnull
      IL_005a:  ceq
      IL_005c:  stloc.2
      IL_005d:  ldloc.2
      IL_005e:  brtrue.s   IL_0067

      IL_0060:  ldloc.3
      IL_0061:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0066:  nop
      IL_0067:  endfinally
    }  // end handler
    IL_0068:  nop
    IL_0069:  ret
  } // end of method Loops::ForEachBreakWhenFound

  .method public hidebysig instance void 
          ForEachOverListOfStruct(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem> items,
                                  int32 'value') cil managed
  {
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem V_1,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem> V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::GetEnumerator()
    IL_0008:  stloc.2
    .try
    {
      IL_0009:  br.s       IL_0020

      IL_000b:  ldloca.s   V_2
      IL_000d:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::get_Current()
      IL_0012:  stloc.0
      IL_0013:  nop
      IL_0014:  ldloc.0
      IL_0015:  stloc.1
      IL_0016:  ldloca.s   V_1
      IL_0018:  ldarg.2
      IL_0019:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem::set_Property(int32)
      IL_001e:  nop
      IL_001f:  nop
      IL_0020:  ldloca.s   V_2
      IL_0022:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::MoveNext()
      IL_0027:  stloc.3
      IL_0028:  ldloc.3
      IL_0029:  brtrue.s   IL_000b

      IL_002b:  leave.s    IL_003c

    }  // end .try
    finally
    {
      IL_002d:  ldloca.s   V_2
      IL_002f:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>
      IL_0035:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_003a:  nop
      IL_003b:  endfinally
    }  // end handler
    IL_003c:  nop
    IL_003d:  ret
  } // end of method Loops::ForEachOverListOfStruct

  .method public hidebysig instance void 
          ForEachOverListOfStruct2(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem> items,
                                   int32 'value') cil managed
  {
    // Code size       70 (0x46)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem V_1,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem> V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::GetEnumerator()
    IL_0008:  stloc.2
    .try
    {
      IL_0009:  br.s       IL_0028

      IL_000b:  ldloca.s   V_2
      IL_000d:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::get_Current()
      IL_0012:  stloc.0
      IL_0013:  nop
      IL_0014:  ldloc.0
      IL_0015:  stloc.1
      IL_0016:  ldloca.s   V_1
      IL_0018:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem::TestCall()
      IL_001d:  nop
      IL_001e:  ldloca.s   V_1
      IL_0020:  ldarg.2
      IL_0021:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem::set_Property(int32)
      IL_0026:  nop
      IL_0027:  nop
      IL_0028:  ldloca.s   V_2
      IL_002a:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::MoveNext()
      IL_002f:  stloc.3
      IL_0030:  ldloc.3
      IL_0031:  brtrue.s   IL_000b

      IL_0033:  leave.s    IL_0044

    }  // end .try
    finally
    {
      IL_0035:  ldloca.s   V_2
      IL_0037:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>
      IL_003d:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0042:  nop
      IL_0043:  endfinally
    }  // end handler
    IL_0044:  nop
    IL_0045:  ret
  } // end of method Loops::ForEachOverListOfStruct2

  .method public hidebysig instance void 
          ForEachOverListOfStruct3(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem> items,
                                   int32 'value') cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem V_0,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_001d

      IL_000b:  ldloca.s   V_1
      IL_000d:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::get_Current()
      IL_0012:  stloc.0
      IL_0013:  nop
      IL_0014:  ldloca.s   V_0
      IL_0016:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem::TestCall()
      IL_001b:  nop
      IL_001c:  nop
      IL_001d:  ldloca.s   V_1
      IL_001f:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::MoveNext()
      IL_0024:  stloc.2
      IL_0025:  ldloc.2
      IL_0026:  brtrue.s   IL_000b

      IL_0028:  leave.s    IL_0039

    }  // end .try
    finally
    {
      IL_002a:  ldloca.s   V_1
      IL_002c:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>
      IL_0032:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0037:  nop
      IL_0038:  endfinally
    }  // end handler
    IL_0039:  nop
    IL_003a:  ret
  } // end of method Loops::ForEachOverListOfStruct3

  .method public hidebysig instance void 
          ForEachOverMultiDimArray(int32[0...,0...] items) cil managed
  {
    // Code size       110 (0x6e)
    .maxstack  3
    .locals init (int32 V_0,
             int32[0...,0...] V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             bool V_6)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloc.1
    IL_0005:  ldc.i4.0
    IL_0006:  callvirt   instance int32 [mscorlib]System.Array::GetUpperBound(int32)
    IL_000b:  stloc.2
    IL_000c:  ldloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  callvirt   instance int32 [mscorlib]System.Array::GetUpperBound(int32)
    IL_0013:  stloc.3
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.0
    IL_0016:  callvirt   instance int32 [mscorlib]System.Array::GetLowerBound(int32)
    IL_001b:  stloc.s    V_4
    IL_001d:  br.s       IL_005f

    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.1
    IL_0021:  callvirt   instance int32 [mscorlib]System.Array::GetLowerBound(int32)
    IL_0026:  stloc.s    V_5
    IL_0028:  br.s       IL_004b

    IL_002a:  ldloc.1
    IL_002b:  ldloc.s    V_4
    IL_002d:  ldloc.s    V_5
    IL_002f:  call       instance int32 int32[0...,0...]::Get(int32,
                                                              int32)
    IL_0034:  stloc.0
    IL_0035:  nop
    IL_0036:  ldloc.0
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003c:  nop
    IL_003d:  ldloc.0
    IL_003e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0043:  nop
    IL_0044:  nop
    IL_0045:  ldloc.s    V_5
    IL_0047:  ldc.i4.1
    IL_0048:  add
    IL_0049:  stloc.s    V_5
    IL_004b:  ldloc.s    V_5
    IL_004d:  ldloc.3
    IL_004e:  cgt
    IL_0050:  ldc.i4.0
    IL_0051:  ceq
    IL_0053:  stloc.s    V_6
    IL_0055:  ldloc.s    V_6
    IL_0057:  brtrue.s   IL_002a

    IL_0059:  ldloc.s    V_4
    IL_005b:  ldc.i4.1
    IL_005c:  add
    IL_005d:  stloc.s    V_4
    IL_005f:  ldloc.s    V_4
    IL_0061:  ldloc.2
    IL_0062:  cgt
    IL_0064:  ldc.i4.0
    IL_0065:  ceq
    IL_0067:  stloc.s    V_6
    IL_0069:  ldloc.s    V_6
    IL_006b:  brtrue.s   IL_001f

    IL_006d:  ret
  } // end of method Loops::ForEachOverMultiDimArray

  .method public hidebysig instance void 
          ForEachOverMultiDimArray2(int32[0...,0...,0...] items) cil managed
  {
    // Code size       153 (0x99)
    .maxstack  4
    .locals init (int32 V_0,
             int32[0...,0...,0...] V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             int32 V_6,
             int32 V_7,
             bool V_8)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloc.1
    IL_0005:  ldc.i4.0
    IL_0006:  callvirt   instance int32 [mscorlib]System.Array::GetUpperBound(int32)
    IL_000b:  stloc.2
    IL_000c:  ldloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  callvirt   instance int32 [mscorlib]System.Array::GetUpperBound(int32)
    IL_0013:  stloc.3
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.2
    IL_0016:  callvirt   instance int32 [mscorlib]System.Array::GetUpperBound(int32)
    IL_001b:  stloc.s    V_4
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4.0
    IL_001f:  callvirt   instance int32 [mscorlib]System.Array::GetLowerBound(int32)
    IL_0024:  stloc.s    V_5
    IL_0026:  br.s       IL_008a

    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.1
    IL_002a:  callvirt   instance int32 [mscorlib]System.Array::GetLowerBound(int32)
    IL_002f:  stloc.s    V_6
    IL_0031:  br.s       IL_0076

    IL_0033:  ldloc.1
    IL_0034:  ldc.i4.2
    IL_0035:  callvirt   instance int32 [mscorlib]System.Array::GetLowerBound(int32)
    IL_003a:  stloc.s    V_7
    IL_003c:  br.s       IL_0061

    IL_003e:  ldloc.1
    IL_003f:  ldloc.s    V_5
    IL_0041:  ldloc.s    V_6
    IL_0043:  ldloc.s    V_7
    IL_0045:  call       instance int32 int32[0...,0...,0...]::Get(int32,
                                                                   int32,
                                                                   int32)
    IL_004a:  stloc.0
    IL_004b:  nop
    IL_004c:  ldloc.0
    IL_004d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0052:  nop
    IL_0053:  ldloc.0
    IL_0054:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0059:  nop
    IL_005a:  nop
    IL_005b:  ldloc.s    V_7
    IL_005d:  ldc.i4.1
    IL_005e:  add
    IL_005f:  stloc.s    V_7
    IL_0061:  ldloc.s    V_7
    IL_0063:  ldloc.s    V_4
    IL_0065:  cgt
    IL_0067:  ldc.i4.0
    IL_0068:  ceq
    IL_006a:  stloc.s    V_8
    IL_006c:  ldloc.s    V_8
    IL_006e:  brtrue.s   IL_003e

    IL_0070:  ldloc.s    V_6
    IL_0072:  ldc.i4.1
    IL_0073:  add
    IL_0074:  stloc.s    V_6
    IL_0076:  ldloc.s    V_6
    IL_0078:  ldloc.3
    IL_0079:  cgt
    IL_007b:  ldc.i4.0
    IL_007c:  ceq
    IL_007e:  stloc.s    V_8
    IL_0080:  ldloc.s    V_8
    IL_0082:  brtrue.s   IL_0033

    IL_0084:  ldloc.s    V_5
    IL_0086:  ldc.i4.1
    IL_0087:  add
    IL_0088:  stloc.s    V_5
    IL_008a:  ldloc.s    V_5
    IL_008c:  ldloc.2
    IL_008d:  cgt
    IL_008f:  ldc.i4.0
    IL_0090:  ceq
    IL_0092:  stloc.s    V_8
    IL_0094:  ldloc.s    V_8
    IL_0096:  brtrue.s   IL_0028

    IL_0098:  ret
  } // end of method Loops::ForEachOverMultiDimArray2

  .method public hidebysig instance void 
          ForEachOverMultiDimArray3(int32*[0...,0...] items) cil managed
  {
    // Code size       112 (0x70)
    .maxstack  3
    .locals init (int32* V_0,
             int32*[0...,0...] V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             bool V_6)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloc.1
    IL_0005:  ldc.i4.0
    IL_0006:  callvirt   instance int32 [mscorlib]System.Array::GetUpperBound(int32)
    IL_000b:  stloc.2
    IL_000c:  ldloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  callvirt   instance int32 [mscorlib]System.Array::GetUpperBound(int32)
    IL_0013:  stloc.3
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.0
    IL_0016:  callvirt   instance int32 [mscorlib]System.Array::GetLowerBound(int32)
    IL_001b:  stloc.s    V_4
    IL_001d:  br.s       IL_0061

    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.1
    IL_0021:  callvirt   instance int32 [mscorlib]System.Array::GetLowerBound(int32)
    IL_0026:  stloc.s    V_5
    IL_0028:  br.s       IL_004d

    IL_002a:  ldloc.1
    IL_002b:  ldloc.s    V_4
    IL_002d:  ldloc.s    V_5
    IL_002f:  call       instance int32* int32*[0...,0...]::Get(int32,
                                                                int32)
    IL_0034:  stloc.0
    IL_0035:  nop
    IL_0036:  ldloc.0
    IL_0037:  ldind.i4
    IL_0038:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003d:  nop
    IL_003e:  ldloc.0
    IL_003f:  ldind.i4
    IL_0040:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0045:  nop
    IL_0046:  nop
    IL_0047:  ldloc.s    V_5
    IL_0049:  ldc.i4.1
    IL_004a:  add
    IL_004b:  stloc.s    V_5
    IL_004d:  ldloc.s    V_5
    IL_004f:  ldloc.3
    IL_0050:  cgt
    IL_0052:  ldc.i4.0
    IL_0053:  ceq
    IL_0055:  stloc.s    V_6
    IL_0057:  ldloc.s    V_6
    IL_0059:  brtrue.s   IL_002a

    IL_005b:  ldloc.s    V_4
    IL_005d:  ldc.i4.1
    IL_005e:  add
    IL_005f:  stloc.s    V_4
    IL_0061:  ldloc.s    V_4
    IL_0063:  ldloc.2
    IL_0064:  cgt
    IL_0066:  ldc.i4.0
    IL_0067:  ceq
    IL_0069:  stloc.s    V_6
    IL_006b:  ldloc.s    V_6
    IL_006d:  brtrue.s   IL_001f

    IL_006f:  ret
  } // end of method Loops::ForEachOverMultiDimArray3

  .method public hidebysig instance void 
          ForOverArray(string[] 'array') cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0014

    IL_0005:  nop
    IL_0006:  ldarg.1
    IL_0007:  ldloc.0
    IL_0008:  ldelem.ref
    IL_0009:  callvirt   instance string [mscorlib]System.String::ToLower()
    IL_000e:  pop
    IL_000f:  nop
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  ldarg.1
    IL_0016:  ldlen
    IL_0017:  conv.i4
    IL_0018:  clt
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  brtrue.s   IL_0005

    IL_001e:  ret
  } // end of method Loops::ForOverArray

  .method public hidebysig instance void 
          NoForeachOverArray(string[] 'array') cil managed
  {
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (int32 V_0,
             string V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0025

    IL_0005:  nop
    IL_0006:  ldarg.1
    IL_0007:  ldloc.0
    IL_0008:  ldelem.ref
    IL_0009:  stloc.1
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.5
    IL_000c:  rem
    IL_000d:  ldc.i4.0
    IL_000e:  ceq
    IL_0010:  ldc.i4.0
    IL_0011:  ceq
    IL_0013:  stloc.2
    IL_0014:  ldloc.2
    IL_0015:  brtrue.s   IL_0020

    IL_0017:  nop
    IL_0018:  ldloc.1
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001e:  nop
    IL_001f:  nop
    IL_0020:  nop
    IL_0021:  ldloc.0
    IL_0022:  ldc.i4.1
    IL_0023:  add
    IL_0024:  stloc.0
    IL_0025:  ldloc.0
    IL_0026:  ldarg.1
    IL_0027:  ldlen
    IL_0028:  conv.i4
    IL_0029:  clt
    IL_002b:  stloc.2
    IL_002c:  ldloc.2
    IL_002d:  brtrue.s   IL_0005

    IL_002f:  ret
  } // end of method Loops::NoForeachOverArray

  .method public hidebysig instance void 
          NestedLoops() cil managed
  {
    // Code size       80 (0x50)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0046

    IL_0005:  nop
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.2
    IL_0008:  rem
    IL_0009:  ldc.i4.0
    IL_000a:  ceq
    IL_000c:  ldc.i4.0
    IL_000d:  ceq
    IL_000f:  stloc.2
    IL_0010:  ldloc.2
    IL_0011:  brtrue.s   IL_0034

    IL_0013:  nop
    IL_0014:  ldc.i4.0
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0029

    IL_0018:  nop
    IL_0019:  ldstr      "Y"
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0023:  nop
    IL_0024:  nop
    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.1
    IL_0027:  add
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.5
    IL_002b:  clt
    IL_002d:  stloc.2
    IL_002e:  ldloc.2
    IL_002f:  brtrue.s   IL_0018

    IL_0031:  nop
    IL_0032:  br.s       IL_0041

    IL_0034:  nop
    IL_0035:  ldstr      "X"
    IL_003a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003f:  nop
    IL_0040:  nop
    IL_0041:  nop
    IL_0042:  ldloc.0
    IL_0043:  ldc.i4.1
    IL_0044:  add
    IL_0045:  stloc.0
    IL_0046:  ldloc.0
    IL_0047:  ldc.i4.s   10
    IL_0049:  clt
    IL_004b:  stloc.2
    IL_004c:  ldloc.2
    IL_004d:  brtrue.s   IL_0005

    IL_004f:  ret
  } // end of method Loops::NestedLoops

  .method private hidebysig instance bool 
          Condition(string arg) cil managed
  {
    // Code size       24 (0x18)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "Condition: "
    IL_0006:  ldarg.1
    IL_0007:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0011:  nop
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.0
    IL_0014:  br.s       IL_0016

    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method Loops::Condition

  .method public hidebysig instance void 
          WhileLoop() cil managed
  {
    // Code size       155 (0x9b)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "Initial"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ldarg.0
    IL_000d:  ldstr      "if"
    IL_0012:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0017:  ldc.i4.0
    IL_0018:  ceq
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  brtrue.s   IL_008f

    IL_001e:  nop
    IL_001f:  br.s       IL_0074

    IL_0021:  nop
    IL_0022:  ldstr      "Loop Body"
    IL_0027:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002c:  nop
    IL_002d:  ldarg.0
    IL_002e:  ldstr      "test"
    IL_0033:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0038:  ldc.i4.0
    IL_0039:  ceq
    IL_003b:  stloc.0
    IL_003c:  ldloc.0
    IL_003d:  brtrue.s   IL_0068

    IL_003f:  nop
    IL_0040:  ldarg.0
    IL_0041:  ldstr      "continue"
    IL_0046:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_004b:  ldc.i4.0
    IL_004c:  ceq
    IL_004e:  stloc.0
    IL_004f:  ldloc.0
    IL_0050:  brtrue.s   IL_0055

    IL_0052:  nop
    IL_0053:  br.s       IL_0074

    IL_0055:  ldarg.0
    IL_0056:  ldstr      "break"
    IL_005b:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0060:  stloc.0
    IL_0061:  ldloc.0
    IL_0062:  brtrue.s   IL_0067

    IL_0064:  nop
    IL_0065:  br.s       IL_0083

    IL_0067:  nop
    IL_0068:  ldstr      "End of loop body"
    IL_006d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0072:  nop
    IL_0073:  nop
    IL_0074:  ldarg.0
    IL_0075:  ldstr      "while"
    IL_007a:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_007f:  stloc.0
    IL_0080:  ldloc.0
    IL_0081:  brtrue.s   IL_0021

    IL_0083:  ldstr      "After loop"
    IL_0088:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_008d:  nop
    IL_008e:  nop
    IL_008f:  ldstr      "End of method"
    IL_0094:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0099:  nop
    IL_009a:  ret
  } // end of method Loops::WhileLoop

  .method public hidebysig instance void 
          ForLoop() cil managed
  {
    // Code size       161 (0xa1)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldstr      "Initial"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ldarg.0
    IL_000d:  ldstr      "if"
    IL_0012:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0017:  ldc.i4.0
    IL_0018:  ceq
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  brtrue.s   IL_0095

    IL_001e:  nop
    IL_001f:  ldc.i4.0
    IL_0020:  stloc.0
    IL_0021:  br.s       IL_007a

    IL_0023:  nop
    IL_0024:  ldstr      "Loop Body"
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002e:  nop
    IL_002f:  ldarg.0
    IL_0030:  ldstr      "test"
    IL_0035:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_003a:  ldc.i4.0
    IL_003b:  ceq
    IL_003d:  stloc.1
    IL_003e:  ldloc.1
    IL_003f:  brtrue.s   IL_006a

    IL_0041:  nop
    IL_0042:  ldarg.0
    IL_0043:  ldstr      "continue"
    IL_0048:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_004d:  ldc.i4.0
    IL_004e:  ceq
    IL_0050:  stloc.1
    IL_0051:  ldloc.1
    IL_0052:  brtrue.s   IL_0057

    IL_0054:  nop
    IL_0055:  br.s       IL_0076

    IL_0057:  ldarg.0
    IL_0058:  ldstr      "not-break"
    IL_005d:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0062:  stloc.1
    IL_0063:  ldloc.1
    IL_0064:  brtrue.s   IL_0069

    IL_0066:  nop
    IL_0067:  br.s       IL_0089

    IL_0069:  nop
    IL_006a:  ldstr      "End of loop body"
    IL_006f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0074:  nop
    IL_0075:  nop
    IL_0076:  ldloc.0
    IL_0077:  ldc.i4.1
    IL_0078:  add
    IL_0079:  stloc.0
    IL_007a:  ldarg.0
    IL_007b:  ldstr      "for"
    IL_0080:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0085:  stloc.1
    IL_0086:  ldloc.1
    IL_0087:  brtrue.s   IL_0023

    IL_0089:  ldstr      "After loop"
    IL_008e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0093:  nop
    IL_0094:  nop
    IL_0095:  ldstr      "End of method"
    IL_009a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_009f:  nop
    IL_00a0:  ret
  } // end of method Loops::ForLoop

  .method public hidebysig instance void 
          ReturnFromDoWhileInTryFinally() cil managed
  {
    // Code size       67 (0x43)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  nop
      IL_0003:  ldarg.0
      IL_0004:  ldstr      "return"
      IL_0009:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
      IL_000e:  ldc.i4.0
      IL_000f:  ceq
      IL_0011:  stloc.0
      IL_0012:  ldloc.0
      IL_0013:  brtrue.s   IL_0018

      IL_0015:  nop
      IL_0016:  leave.s    IL_0041

      IL_0018:  nop
      IL_0019:  ldarg.0
      IL_001a:  ldstr      "repeat"
      IL_001f:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
      IL_0024:  stloc.0
      IL_0025:  ldloc.0
      IL_0026:  brtrue.s   IL_0002

      IL_0028:  call       string[] [mscorlib]System.Environment::GetCommandLineArgs()
      IL_002d:  pop
      IL_002e:  nop
      IL_002f:  leave.s    IL_003a

    }  // end .try
    finally
    {
      IL_0031:  nop
      IL_0032:  call       string[] [mscorlib]System.Environment::GetCommandLineArgs()
      IL_0037:  pop
      IL_0038:  nop
      IL_0039:  endfinally
    }  // end handler
    IL_003a:  nop
    IL_003b:  call       string[] [mscorlib]System.Environment::GetCommandLineArgs()
    IL_0040:  pop
    IL_0041:  nop
    IL_0042:  ret
  } // end of method Loops::ReturnFromDoWhileInTryFinally

  .method public hidebysig instance void 
          ForLoopWithEarlyReturn(int32[] ids) cil managed
  {
    // Code size       50 (0x32)
    .maxstack  3
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/Item V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0027

    IL_0005:  nop
    IL_0006:  ldnull
    IL_0007:  stloc.1
    IL_0008:  ldarg.0
    IL_0009:  ldarg.1
    IL_000a:  ldloc.0
    IL_000b:  ldelem.i4
    IL_000c:  ldloca.s   V_1
    IL_000e:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::TryGetItem(int32,
                                                                                                       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/Item&)
    IL_0013:  nop
    IL_0014:  ldloc.1
    IL_0015:  ldnull
    IL_0016:  ceq
    IL_0018:  ldc.i4.0
    IL_0019:  ceq
    IL_001b:  stloc.2
    IL_001c:  ldloc.2
    IL_001d:  brtrue.s   IL_0022

    IL_001f:  nop
    IL_0020:  br.s       IL_0031

    IL_0022:  nop
    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.1
    IL_0025:  add
    IL_0026:  stloc.0
    IL_0027:  ldloc.0
    IL_0028:  ldarg.1
    IL_0029:  ldlen
    IL_002a:  conv.i4
    IL_002b:  clt
    IL_002d:  stloc.2
    IL_002e:  ldloc.2
    IL_002f:  brtrue.s   IL_0005

    IL_0031:  ret
  } // end of method Loops::ForLoopWithEarlyReturn

  .method public hidebysig instance void 
          ForeachLoopWithEarlyReturn(class [mscorlib]System.Collections.Generic.List`1<object> items) cil managed
  {
    // Code size       74 (0x4a)
    .maxstack  3
    .locals init (object V_0,
             valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<object> V_1,
             object V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<object>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_002c

      IL_000b:  ldloca.s   V_1
      IL_000d:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<object>::get_Current()
      IL_0012:  stloc.0
      IL_0013:  nop
      IL_0014:  ldarg.0
      IL_0015:  ldloc.0
      IL_0016:  dup
      IL_0017:  stloc.2
      IL_0018:  stfld      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::someObject
      IL_001d:  ldloc.2
      IL_001e:  ldnull
      IL_001f:  ceq
      IL_0021:  ldc.i4.0
      IL_0022:  ceq
      IL_0024:  stloc.3
      IL_0025:  ldloc.3
      IL_0026:  brtrue.s   IL_002b

      IL_0028:  nop
      IL_0029:  br.s       IL_0037

      IL_002b:  nop
      IL_002c:  ldloca.s   V_1
      IL_002e:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<object>::MoveNext()
      IL_0033:  stloc.3
      IL_0034:  ldloc.3
      IL_0035:  brtrue.s   IL_000b

      IL_0037:  leave.s    IL_0048

    }  // end .try
    finally
    {
      IL_0039:  ldloca.s   V_1
      IL_003b:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<object>
      IL_0041:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0046:  nop
      IL_0047:  endfinally
    }  // end handler
    IL_0048:  nop
    IL_0049:  ret
  } // end of method Loops::ForeachLoopWithEarlyReturn

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Loops::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\Loops.res
