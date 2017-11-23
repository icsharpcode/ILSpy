
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Loops
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
.module Loops.dll
// MVID: {F07742D5-57A8-4CEC-B130-8CFD8D8E0AFC}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x04780000


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
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumerator::get_Current

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumerator::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumerator::Reset

    .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator 
            GetEnumerator() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ret
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
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumerator::get_Current

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumerator::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumerator::Reset

    .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumerator 
            GetEnumerator() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumerator
      IL_0006:  ret
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
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumerator`1::get_Current

    .method public hidebysig instance void 
            Dispose() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumerator`1::Dispose

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumerator`1::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumerator`1::Reset

    .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!T> 
            GetEnumerator() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ret
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
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumerator`1::get_Current

    .method public hidebysig instance void 
            Dispose() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumerator`1::Dispose

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumerator`1::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumerator`1::Reset

    .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumerator`1<!T> 
            GetEnumerator() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldobj      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumerator`1<!T>
      IL_0006:  ret
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
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumeratorWithIDisposable::get_Current

    .method public hidebysig newslot virtual final 
            instance void  Dispose() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumeratorWithIDisposable::Dispose

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumeratorWithIDisposable::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumeratorWithIDisposable::Reset

    .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable 
            GetEnumerator() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ret
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
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumeratorWithIDisposable::get_Current

    .method public hidebysig newslot virtual final 
            instance void  Dispose() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumeratorWithIDisposable::Dispose

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumeratorWithIDisposable::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumeratorWithIDisposable::Reset

    .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable 
            GetEnumerator() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable
      IL_0006:  ret
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
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumeratorWithIDisposable`1::get_Current

    .method public hidebysig newslot virtual final 
            instance void  Dispose() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumeratorWithIDisposable`1::Dispose

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumeratorWithIDisposable`1::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomClassEnumeratorWithIDisposable`1::Reset

    .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!T> 
            GetEnumerator() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ret
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
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumeratorWithIDisposable`1::get_Current

    .method public hidebysig newslot virtual final 
            instance void  Dispose() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumeratorWithIDisposable`1::Dispose

    .method public hidebysig instance bool 
            MoveNext() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumeratorWithIDisposable`1::MoveNext

    .method public hidebysig instance void 
            Reset() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomStructEnumeratorWithIDisposable`1::Reset

    .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!T> 
            GetEnumerator() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldobj      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!T>
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem::'<Property>k__BackingField'
      IL_0006:  ret
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
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
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

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass29_0'
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
    } // end of method '<>c__DisplayClass29_0'::.ctor

    .method assembly hidebysig instance bool 
            '<ForeachWithCapturedVariable>b__0'() cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/'<>c__DisplayClass29_0'::c
      IL_0006:  ldc.i4.5
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method '<>c__DisplayClass29_0'::'<ForeachWithCapturedVariable>b__0'

  } // end of class '<>c__DisplayClass29_0'

  .field private class [mscorlib]System.Collections.Generic.IEnumerable`1<string> alternatives
  .field private object someObject
  .method private hidebysig instance void 
          TryGetItem(int32 id,
                     [out] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/Item& item) cil managed
  {
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.2
    IL_0001:  ldnull
    IL_0002:  stind.ref
    IL_0003:  ret
  } // end of method Loops::TryGetItem

  .method private hidebysig static void  Operation(int32& item) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method Loops::Operation

  .method private hidebysig static void  Operation(class [mscorlib]System.Func`1<bool> f) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method Loops::Operation

  .method public hidebysig instance void 
          ForEachOnField() cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  1
    .locals init (class [mscorlib]System.Collections.Generic.IEnumerator`1<string> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::alternatives
    IL_0006:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
    IL_000b:  stloc.0
    .try
    {
      IL_000c:  br.s       IL_001a

      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0014:  callvirt   instance string [mscorlib]System.String::ToLower()
      IL_0019:  pop
      IL_001a:  ldloc.0
      IL_001b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0020:  brtrue.s   IL_000e

      IL_0022:  leave.s    IL_002e

    }  // end .try
    finally
    {
      IL_0024:  ldloc.0
      IL_0025:  brfalse.s  IL_002d

      IL_0027:  ldloc.0
      IL_0028:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002d:  endfinally
    }  // end handler
    IL_002e:  ret
  } // end of method Loops::ForEachOnField

  .method public hidebysig instance void 
          ForEach(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> alternatives) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  1
    .locals init (class [mscorlib]System.Collections.Generic.IEnumerator`1<string> V_0)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_0015

      IL_0009:  ldloc.0
      IL_000a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_000f:  callvirt   instance string [mscorlib]System.String::ToLower()
      IL_0014:  pop
      IL_0015:  ldloc.0
      IL_0016:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_001b:  brtrue.s   IL_0009

      IL_001d:  leave.s    IL_0029

    }  // end .try
    finally
    {
      IL_001f:  ldloc.0
      IL_0020:  brfalse.s  IL_0028

      IL_0022:  ldloc.0
      IL_0023:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0028:  endfinally
    }  // end handler
    IL_0029:  ret
  } // end of method Loops::ForEach

  .method public hidebysig instance void 
          ForEachOverList(class [mscorlib]System.Collections.Generic.List`1<string> list) cil managed
  {
    // Code size       48 (0x30)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string> V_0)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<string>::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_0016

      IL_0009:  ldloca.s   V_0
      IL_000b:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::get_Current()
      IL_0010:  callvirt   instance string [mscorlib]System.String::ToLower()
      IL_0015:  pop
      IL_0016:  ldloca.s   V_0
      IL_0018:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::MoveNext()
      IL_001d:  brtrue.s   IL_0009

      IL_001f:  leave.s    IL_002f

    }  // end .try
    finally
    {
      IL_0021:  ldloca.s   V_0
      IL_0023:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>
      IL_0029:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002e:  endfinally
    }  // end handler
    IL_002f:  ret
  } // end of method Loops::ForEachOverList

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerable(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       49 (0x31)
    .maxstack  1
    .locals init (class [mscorlib]System.Collections.IEnumerator V_0,
             class [mscorlib]System.IDisposable V_1)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_0015

      IL_0009:  ldloc.0
      IL_000a:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_000f:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0014:  pop
      IL_0015:  ldloc.0
      IL_0016:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_001b:  brtrue.s   IL_0009

      IL_001d:  leave.s    IL_0030

    }  // end .try
    finally
    {
      IL_001f:  ldloc.0
      IL_0020:  isinst     [mscorlib]System.IDisposable
      IL_0025:  stloc.1
      IL_0026:  ldloc.1
      IL_0027:  brfalse.s  IL_002f

      IL_0029:  ldloc.1
      IL_002a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002f:  endfinally
    }  // end handler
    IL_0030:  ret
  } // end of method Loops::ForEachOverNonGenericEnumerable

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerableWithAutomaticCastValueType(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  1
    .locals init (class [mscorlib]System.Collections.IEnumerator V_0,
             int32 V_1,
             class [mscorlib]System.IDisposable V_2)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_001d

      IL_0009:  ldloc.0
      IL_000a:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_000f:  unbox.any  [mscorlib]System.Int32
      IL_0014:  stloc.1
      IL_0015:  ldloca.s   V_1
      IL_0017:  call       instance string [mscorlib]System.Int32::ToString()
      IL_001c:  pop
      IL_001d:  ldloc.0
      IL_001e:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0023:  brtrue.s   IL_0009

      IL_0025:  leave.s    IL_0038

    }  // end .try
    finally
    {
      IL_0027:  ldloc.0
      IL_0028:  isinst     [mscorlib]System.IDisposable
      IL_002d:  stloc.2
      IL_002e:  ldloc.2
      IL_002f:  brfalse.s  IL_0037

      IL_0031:  ldloc.2
      IL_0032:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0037:  endfinally
    }  // end handler
    IL_0038:  ret
  } // end of method Loops::ForEachOverNonGenericEnumerableWithAutomaticCastValueType

  .method public hidebysig instance void 
          ForEachOverNonGenericEnumerableWithAutomaticCastRefType(class [mscorlib]System.Collections.IEnumerable enumerable) cil managed
  {
    // Code size       53 (0x35)
    .maxstack  1
    .locals init (class [mscorlib]System.Collections.IEnumerator V_0,
             class [mscorlib]System.IDisposable V_1)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_0019

      IL_0009:  ldloc.0
      IL_000a:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_000f:  castclass  [mscorlib]System.String
      IL_0014:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0019:  ldloc.0
      IL_001a:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_001f:  brtrue.s   IL_0009

      IL_0021:  leave.s    IL_0034

    }  // end .try
    finally
    {
      IL_0023:  ldloc.0
      IL_0024:  isinst     [mscorlib]System.IDisposable
      IL_0029:  stloc.1
      IL_002a:  ldloc.1
      IL_002b:  brfalse.s  IL_0033

      IL_002d:  ldloc.1
      IL_002e:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0033:  endfinally
    }  // end handler
    IL_0034:  ret
  } // end of method Loops::ForEachOverNonGenericEnumerableWithAutomaticCastRefType

  .method public hidebysig instance void 
          ForEachOnCustomClassEnumerator(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator e) cil managed
  {
    // Code size       48 (0x30)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator V_0,
             class [mscorlib]System.IDisposable V_1)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_0014

      IL_0009:  ldloc.0
      IL_000a:  callvirt   instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator::get_Current()
      IL_000f:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_0014:  ldloc.0
      IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator::MoveNext()
      IL_001a:  brtrue.s   IL_0009

      IL_001c:  leave.s    IL_002f

    }  // end .try
    finally
    {
      IL_001e:  ldloc.0
      IL_001f:  isinst     [mscorlib]System.IDisposable
      IL_0024:  stloc.1
      IL_0025:  ldloc.1
      IL_0026:  brfalse.s  IL_002e

      IL_0028:  ldloc.1
      IL_0029:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002e:  endfinally
    }  // end handler
    IL_002f:  ret
  } // end of method Loops::ForEachOnCustomClassEnumerator

  .method public hidebysig instance void 
          ForEachOnGenericCustomClassEnumerator<T>(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!!T> e) cil managed
  {
    // Code size       53 (0x35)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!!T> V_0,
             class [mscorlib]System.IDisposable V_1)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!0> class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!!T>::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_0019

      IL_0009:  ldloc.0
      IL_000a:  callvirt   instance !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!!T>::get_Current()
      IL_000f:  box        !!T
      IL_0014:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_0019:  ldloc.0
      IL_001a:  callvirt   instance bool class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumerator`1<!!T>::MoveNext()
      IL_001f:  brtrue.s   IL_0009

      IL_0021:  leave.s    IL_0034

    }  // end .try
    finally
    {
      IL_0023:  ldloc.0
      IL_0024:  isinst     [mscorlib]System.IDisposable
      IL_0029:  stloc.1
      IL_002a:  ldloc.1
      IL_002b:  brfalse.s  IL_0033

      IL_002d:  ldloc.1
      IL_002e:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0033:  endfinally
    }  // end handler
    IL_0034:  ret
  } // end of method Loops::ForEachOnGenericCustomClassEnumerator

  .method public hidebysig instance void 
          ForEachOnCustomClassEnumeratorWithIDisposable(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable e) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable V_0)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_0014

      IL_0009:  ldloc.0
      IL_000a:  callvirt   instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable::get_Current()
      IL_000f:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_0014:  ldloc.0
      IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable::MoveNext()
      IL_001a:  brtrue.s   IL_0009

      IL_001c:  leave.s    IL_0028

    }  // end .try
    finally
    {
      IL_001e:  ldloc.0
      IL_001f:  brfalse.s  IL_0027

      IL_0021:  ldloc.0
      IL_0022:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0027:  endfinally
    }  // end handler
    IL_0028:  ret
  } // end of method Loops::ForEachOnCustomClassEnumeratorWithIDisposable

  .method public hidebysig instance void 
          ForEachOnCustomStructEnumeratorWithIDisposable(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable e) cil managed
  {
    // Code size       48 (0x30)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable V_0)
    IL_0000:  ldarga.s   e
    IL_0002:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable::GetEnumerator()
    IL_0007:  stloc.0
    .try
    {
      IL_0008:  br.s       IL_0016

      IL_000a:  ldloca.s   V_0
      IL_000c:  call       instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable::get_Current()
      IL_0011:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_0016:  ldloca.s   V_0
      IL_0018:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable::MoveNext()
      IL_001d:  brtrue.s   IL_000a

      IL_001f:  leave.s    IL_002f

    }  // end .try
    finally
    {
      IL_0021:  ldloca.s   V_0
      IL_0023:  constrained. ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable
      IL_0029:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002e:  endfinally
    }  // end handler
    IL_002f:  ret
  } // end of method Loops::ForEachOnCustomStructEnumeratorWithIDisposable

  .method public hidebysig instance void 
          ForEachOnGenericCustomClassEnumeratorWithIDisposable<T>(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!!T> e) cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!!T> V_0)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!0> class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!!T>::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_0019

      IL_0009:  ldloc.0
      IL_000a:  callvirt   instance !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!!T>::get_Current()
      IL_000f:  box        !!T
      IL_0014:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_0019:  ldloc.0
      IL_001a:  callvirt   instance bool class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomClassEnumeratorWithIDisposable`1<!!T>::MoveNext()
      IL_001f:  brtrue.s   IL_0009

      IL_0021:  leave.s    IL_002d

    }  // end .try
    finally
    {
      IL_0023:  ldloc.0
      IL_0024:  brfalse.s  IL_002c

      IL_0026:  ldloc.0
      IL_0027:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002c:  endfinally
    }  // end handler
    IL_002d:  ret
  } // end of method Loops::ForEachOnGenericCustomClassEnumeratorWithIDisposable

  .method public hidebysig instance void 
          ForEachOnGenericCustomStructEnumeratorWithIDisposable<T>(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!!T> e) cil managed
  {
    // Code size       53 (0x35)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!!T> V_0)
    IL_0000:  ldarga.s   e
    IL_0002:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!0> valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!!T>::GetEnumerator()
    IL_0007:  stloc.0
    .try
    {
      IL_0008:  br.s       IL_001b

      IL_000a:  ldloca.s   V_0
      IL_000c:  call       instance !0 valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!!T>::get_Current()
      IL_0011:  box        !!T
      IL_0016:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_001b:  ldloca.s   V_0
      IL_001d:  call       instance bool valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!!T>::MoveNext()
      IL_0022:  brtrue.s   IL_000a

      IL_0024:  leave.s    IL_0034

    }  // end .try
    finally
    {
      IL_0026:  ldloca.s   V_0
      IL_0028:  constrained. valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/CustomStructEnumeratorWithIDisposable`1<!!T>
      IL_002e:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0033:  endfinally
    }  // end handler
    IL_0034:  ret
  } // end of method Loops::ForEachOnGenericCustomStructEnumeratorWithIDisposable

  .method public hidebysig static void  NonGenericForeachWithReturnFallbackTest(class [mscorlib]System.Collections.IEnumerable e) cil managed
  {
    // Code size       88 (0x58)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.IEnumerator V_0,
             object V_1,
             class [mscorlib]System.IDisposable V_2)
    IL_0000:  ldstr      "NonGenericForeachWithReturnFallback:"
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000a:  ldarg.0
    IL_000b:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_0010:  stloc.0
    .try
    {
      IL_0011:  ldstr      "MoveNext"
      IL_0016:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001b:  ldloc.0
      IL_001c:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0021:  brfalse.s  IL_003a

      IL_0023:  ldloc.0
      IL_0024:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_0029:  stloc.1
      IL_002a:  ldstr      "current: "
      IL_002f:  ldloc.1
      IL_0030:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_0035:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_003a:  leave.s    IL_004d

    }  // end .try
    finally
    {
      IL_003c:  ldloc.0
      IL_003d:  isinst     [mscorlib]System.IDisposable
      IL_0042:  stloc.2
      IL_0043:  ldloc.2
      IL_0044:  brfalse.s  IL_004c

      IL_0046:  ldloc.2
      IL_0047:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_004c:  endfinally
    }  // end handler
    IL_004d:  ldstr      "After finally!"
    IL_0052:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0057:  ret
  } // end of method Loops::NonGenericForeachWithReturnFallbackTest

  .method public hidebysig static void  ForeachWithRefUsage(class [mscorlib]System.Collections.Generic.List`1<int32> items) cil managed
  {
    // Code size       50 (0x32)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32> V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_0018

      IL_0009:  ldloca.s   V_0
      IL_000b:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::get_Current()
      IL_0010:  stloc.1
      IL_0011:  ldloca.s   V_1
      IL_0013:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Operation(int32&)
      IL_0018:  ldloca.s   V_0
      IL_001a:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::MoveNext()
      IL_001f:  brtrue.s   IL_0009

      IL_0021:  leave.s    IL_0031

    }  // end .try
    finally
    {
      IL_0023:  ldloca.s   V_0
      IL_0025:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>
      IL_002b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0030:  endfinally
    }  // end handler
    IL_0031:  ret
  } // end of method Loops::ForeachWithRefUsage

  .method public hidebysig static void  ForeachWithCapturedVariable(class [mscorlib]System.Collections.Generic.List`1<int32> items) cil managed
  {
    // Code size       71 (0x47)
    .maxstack  3
    .locals init (valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32> V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_002d

      IL_0009:  ldloca.s   V_0
      IL_000b:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::get_Current()
      IL_0010:  stloc.1
      IL_0011:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/'<>c__DisplayClass29_0'::.ctor()
      IL_0016:  dup
      IL_0017:  ldloc.1
      IL_0018:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/'<>c__DisplayClass29_0'::c
      IL_001d:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/'<>c__DisplayClass29_0'::'<ForeachWithCapturedVariable>b__0'()
      IL_0023:  newobj     instance void class [mscorlib]System.Func`1<bool>::.ctor(object,
                                                                                    native int)
      IL_0028:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Operation(class [mscorlib]System.Func`1<bool>)
      IL_002d:  ldloca.s   V_0
      IL_002f:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::MoveNext()
      IL_0034:  brtrue.s   IL_0009

      IL_0036:  leave.s    IL_0046

    }  // end .try
    finally
    {
      IL_0038:  ldloca.s   V_0
      IL_003a:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>
      IL_0040:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0045:  endfinally
    }  // end handler
    IL_0046:  ret
  } // end of method Loops::ForeachWithCapturedVariable

  .method public hidebysig static !!T  LastOrDefault<T>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!T> items) cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  1
    .locals init (!!T V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<!!T> V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  initobj    !!T
    IL_0008:  ldarg.0
    IL_0009:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!!T>::GetEnumerator()
    IL_000e:  stloc.1
    .try
    {
      IL_000f:  br.s       IL_0018

      IL_0011:  ldloc.1
      IL_0012:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!!T>::get_Current()
      IL_0017:  stloc.0
      IL_0018:  ldloc.1
      IL_0019:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_001e:  brtrue.s   IL_0011

      IL_0020:  leave.s    IL_002c

    }  // end .try
    finally
    {
      IL_0022:  ldloc.1
      IL_0023:  brfalse.s  IL_002b

      IL_0025:  ldloc.1
      IL_0026:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002b:  endfinally
    }  // end handler
    IL_002c:  ldloc.0
    IL_002d:  ret
  } // end of method Loops::LastOrDefault

  .method public hidebysig instance void 
          ForEachOverArray(string[] 'array') cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  2
    .locals init (string[] V_0,
             int32 V_1,
             string V_2)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0024

    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem.ref
    IL_0009:  stloc.2
    IL_000a:  ldloc.2
    IL_000b:  callvirt   instance string [mscorlib]System.String::ToLower()
    IL_0010:  ldloc.2
    IL_0011:  callvirt   instance string [mscorlib]System.String::ToUpper()
    IL_0016:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stloc.1
    IL_0024:  ldloc.1
    IL_0025:  ldloc.0
    IL_0026:  ldlen
    IL_0027:  conv.i4
    IL_0028:  blt.s      IL_0006

    IL_002a:  ret
  } // end of method Loops::ForEachOverArray

  .method public hidebysig instance void 
          ForEachOverArrayOfPointers(int32*[] 'array') cil managed
  {
    // Code size       51 (0x33)
    .maxstack  2
    .locals init (int32*[] V_0,
             int32 V_1)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_002c

    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem.i
    IL_0009:  dup
    IL_000a:  newobj     instance void [mscorlib]System.IntPtr::.ctor(void*)
    IL_000f:  box        [mscorlib]System.IntPtr
    IL_0014:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0019:  newobj     instance void [mscorlib]System.IntPtr::.ctor(void*)
    IL_001e:  box        [mscorlib]System.IntPtr
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.1
    IL_002a:  add
    IL_002b:  stloc.1
    IL_002c:  ldloc.1
    IL_002d:  ldloc.0
    IL_002e:  ldlen
    IL_002f:  conv.i4
    IL_0030:  blt.s      IL_0006

    IL_0032:  ret
  } // end of method Loops::ForEachOverArrayOfPointers

  .method public hidebysig instance void 
          ForEachBreakWhenFound(string name,
                                valuetype [mscorlib]System.StringComparison& output) cil managed
  {
    // Code size       89 (0x59)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.IEnumerator V_0,
             valuetype [mscorlib]System.StringComparison V_1,
             class [mscorlib]System.IDisposable V_2)
    IL_0000:  ldtoken    [mscorlib]System.StringComparison
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  call       class [mscorlib]System.Array [mscorlib]System.Enum::GetValues(class [mscorlib]System.Type)
    IL_000f:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Array::GetEnumerator()
    IL_0014:  stloc.0
    .try
    {
      IL_0015:  br.s       IL_003d

      IL_0017:  ldloc.0
      IL_0018:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_001d:  unbox.any  [mscorlib]System.StringComparison
      IL_0022:  stloc.1
      IL_0023:  ldloca.s   V_1
      IL_0025:  constrained. [mscorlib]System.StringComparison
      IL_002b:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0030:  ldarg.1
      IL_0031:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_0036:  brfalse.s  IL_003d

      IL_0038:  ldarg.2
      IL_0039:  ldloc.1
      IL_003a:  stind.i4
      IL_003b:  leave.s    IL_0058

      IL_003d:  ldloc.0
      IL_003e:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0043:  brtrue.s   IL_0017

      IL_0045:  leave.s    IL_0058

    }  // end .try
    finally
    {
      IL_0047:  ldloc.0
      IL_0048:  isinst     [mscorlib]System.IDisposable
      IL_004d:  stloc.2
      IL_004e:  ldloc.2
      IL_004f:  brfalse.s  IL_0057

      IL_0051:  ldloc.2
      IL_0052:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0057:  endfinally
    }  // end handler
    IL_0058:  ret
  } // end of method Loops::ForEachBreakWhenFound

  .method public hidebysig instance void 
          ForEachOverListOfStruct(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem> items,
                                  int32 'value') cil managed
  {
    // Code size       51 (0x33)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem> V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem V_1)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_0019

      IL_0009:  ldloca.s   V_0
      IL_000b:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::get_Current()
      IL_0010:  stloc.1
      IL_0011:  ldloca.s   V_1
      IL_0013:  ldarg.2
      IL_0014:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem::set_Property(int32)
      IL_0019:  ldloca.s   V_0
      IL_001b:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::MoveNext()
      IL_0020:  brtrue.s   IL_0009

      IL_0022:  leave.s    IL_0032

    }  // end .try
    finally
    {
      IL_0024:  ldloca.s   V_0
      IL_0026:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>
      IL_002c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0031:  endfinally
    }  // end handler
    IL_0032:  ret
  } // end of method Loops::ForEachOverListOfStruct

  .method public hidebysig instance void 
          ForEachOverListOfStruct2(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem> items,
                                   int32 'value') cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem> V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem V_1)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_0020

      IL_0009:  ldloca.s   V_0
      IL_000b:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::get_Current()
      IL_0010:  stloc.1
      IL_0011:  ldloca.s   V_1
      IL_0013:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem::TestCall()
      IL_0018:  ldloca.s   V_1
      IL_001a:  ldarg.2
      IL_001b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem::set_Property(int32)
      IL_0020:  ldloca.s   V_0
      IL_0022:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::MoveNext()
      IL_0027:  brtrue.s   IL_0009

      IL_0029:  leave.s    IL_0039

    }  // end .try
    finally
    {
      IL_002b:  ldloca.s   V_0
      IL_002d:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>
      IL_0033:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0038:  endfinally
    }  // end handler
    IL_0039:  ret
  } // end of method Loops::ForEachOverListOfStruct2

  .method public hidebysig instance void 
          ForEachOverListOfStruct3(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem> items,
                                   int32 'value') cil managed
  {
    // Code size       50 (0x32)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem> V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem V_1)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_0018

      IL_0009:  ldloca.s   V_0
      IL_000b:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::get_Current()
      IL_0010:  stloc.1
      IL_0011:  ldloca.s   V_1
      IL_0013:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem::TestCall()
      IL_0018:  ldloca.s   V_0
      IL_001a:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>::MoveNext()
      IL_001f:  brtrue.s   IL_0009

      IL_0021:  leave.s    IL_0031

    }  // end .try
    finally
    {
      IL_0023:  ldloca.s   V_0
      IL_0025:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/DataItem>
      IL_002b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0030:  endfinally
    }  // end handler
    IL_0031:  ret
  } // end of method Loops::ForEachOverListOfStruct3

  .method public hidebysig instance void 
          ForEachOverMultiDimArray(int32[0...,0...] items) cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  3
    .locals init (int32[0...,0...] V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  ldc.i4.0
    IL_0004:  callvirt   instance int32 [mscorlib]System.Array::GetUpperBound(int32)
    IL_0009:  stloc.1
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance int32 [mscorlib]System.Array::GetUpperBound(int32)
    IL_0011:  stloc.2
    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.0
    IL_0014:  callvirt   instance int32 [mscorlib]System.Array::GetLowerBound(int32)
    IL_0019:  stloc.3
    IL_001a:  br.s       IL_004a

    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.1
    IL_001e:  callvirt   instance int32 [mscorlib]System.Array::GetLowerBound(int32)
    IL_0023:  stloc.s    V_4
    IL_0025:  br.s       IL_0041

    IL_0027:  ldloc.0
    IL_0028:  ldloc.3
    IL_0029:  ldloc.s    V_4
    IL_002b:  call       instance int32 int32[0...,0...]::Get(int32,
                                                              int32)
    IL_0030:  dup
    IL_0031:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003b:  ldloc.s    V_4
    IL_003d:  ldc.i4.1
    IL_003e:  add
    IL_003f:  stloc.s    V_4
    IL_0041:  ldloc.s    V_4
    IL_0043:  ldloc.2
    IL_0044:  ble.s      IL_0027

    IL_0046:  ldloc.3
    IL_0047:  ldc.i4.1
    IL_0048:  add
    IL_0049:  stloc.3
    IL_004a:  ldloc.3
    IL_004b:  ldloc.1
    IL_004c:  ble.s      IL_001c

    IL_004e:  ret
  } // end of method Loops::ForEachOverMultiDimArray

  .method public hidebysig instance void 
          ForEachOverMultiDimArray2(int32[0...,0...,0...] items) cil managed
  {
    // Code size       116 (0x74)
    .maxstack  4
    .locals init (int32[0...,0...,0...] V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             int32 V_6)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  ldc.i4.0
    IL_0004:  callvirt   instance int32 [mscorlib]System.Array::GetUpperBound(int32)
    IL_0009:  stloc.1
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance int32 [mscorlib]System.Array::GetUpperBound(int32)
    IL_0011:  stloc.2
    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.2
    IL_0014:  callvirt   instance int32 [mscorlib]System.Array::GetUpperBound(int32)
    IL_0019:  stloc.3
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.0
    IL_001c:  callvirt   instance int32 [mscorlib]System.Array::GetLowerBound(int32)
    IL_0021:  stloc.s    V_4
    IL_0023:  br.s       IL_006e

    IL_0025:  ldloc.0
    IL_0026:  ldc.i4.1
    IL_0027:  callvirt   instance int32 [mscorlib]System.Array::GetLowerBound(int32)
    IL_002c:  stloc.s    V_5
    IL_002e:  br.s       IL_0063

    IL_0030:  ldloc.0
    IL_0031:  ldc.i4.2
    IL_0032:  callvirt   instance int32 [mscorlib]System.Array::GetLowerBound(int32)
    IL_0037:  stloc.s    V_6
    IL_0039:  br.s       IL_0058

    IL_003b:  ldloc.0
    IL_003c:  ldloc.s    V_4
    IL_003e:  ldloc.s    V_5
    IL_0040:  ldloc.s    V_6
    IL_0042:  call       instance int32 int32[0...,0...,0...]::Get(int32,
                                                                   int32,
                                                                   int32)
    IL_0047:  dup
    IL_0048:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_004d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0052:  ldloc.s    V_6
    IL_0054:  ldc.i4.1
    IL_0055:  add
    IL_0056:  stloc.s    V_6
    IL_0058:  ldloc.s    V_6
    IL_005a:  ldloc.3
    IL_005b:  ble.s      IL_003b

    IL_005d:  ldloc.s    V_5
    IL_005f:  ldc.i4.1
    IL_0060:  add
    IL_0061:  stloc.s    V_5
    IL_0063:  ldloc.s    V_5
    IL_0065:  ldloc.2
    IL_0066:  ble.s      IL_0030

    IL_0068:  ldloc.s    V_4
    IL_006a:  ldc.i4.1
    IL_006b:  add
    IL_006c:  stloc.s    V_4
    IL_006e:  ldloc.s    V_4
    IL_0070:  ldloc.1
    IL_0071:  ble.s      IL_0025

    IL_0073:  ret
  } // end of method Loops::ForEachOverMultiDimArray2

  .method public hidebysig instance void 
          ForEachOverMultiDimArray3(int32*[0...,0...] items) cil managed
  {
    // Code size       81 (0x51)
    .maxstack  3
    .locals init (int32*[0...,0...] V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  ldc.i4.0
    IL_0004:  callvirt   instance int32 [mscorlib]System.Array::GetUpperBound(int32)
    IL_0009:  stloc.1
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance int32 [mscorlib]System.Array::GetUpperBound(int32)
    IL_0011:  stloc.2
    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.0
    IL_0014:  callvirt   instance int32 [mscorlib]System.Array::GetLowerBound(int32)
    IL_0019:  stloc.3
    IL_001a:  br.s       IL_004c

    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.1
    IL_001e:  callvirt   instance int32 [mscorlib]System.Array::GetLowerBound(int32)
    IL_0023:  stloc.s    V_4
    IL_0025:  br.s       IL_0043

    IL_0027:  ldloc.0
    IL_0028:  ldloc.3
    IL_0029:  ldloc.s    V_4
    IL_002b:  call       instance int32* int32*[0...,0...]::Get(int32,
                                                                int32)
    IL_0030:  dup
    IL_0031:  ldind.i4
    IL_0032:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0037:  ldind.i4
    IL_0038:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003d:  ldloc.s    V_4
    IL_003f:  ldc.i4.1
    IL_0040:  add
    IL_0041:  stloc.s    V_4
    IL_0043:  ldloc.s    V_4
    IL_0045:  ldloc.2
    IL_0046:  ble.s      IL_0027

    IL_0048:  ldloc.3
    IL_0049:  ldc.i4.1
    IL_004a:  add
    IL_004b:  stloc.3
    IL_004c:  ldloc.3
    IL_004d:  ldloc.1
    IL_004e:  ble.s      IL_001c

    IL_0050:  ret
  } // end of method Loops::ForEachOverMultiDimArray3

  .method public hidebysig instance void 
          ForOverArray(string[] 'array') cil managed
  {
    // Code size       24 (0x18)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0011

    IL_0004:  ldarg.1
    IL_0005:  ldloc.0
    IL_0006:  ldelem.ref
    IL_0007:  callvirt   instance string [mscorlib]System.String::ToLower()
    IL_000c:  pop
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.1
    IL_000f:  add
    IL_0010:  stloc.0
    IL_0011:  ldloc.0
    IL_0012:  ldarg.1
    IL_0013:  ldlen
    IL_0014:  conv.i4
    IL_0015:  blt.s      IL_0004

    IL_0017:  ret
  } // end of method Loops::ForOverArray

  .method public hidebysig instance void 
          NoForeachOverArray(string[] 'array') cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  2
    .locals init (int32 V_0,
             string V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0017

    IL_0004:  ldarg.1
    IL_0005:  ldloc.0
    IL_0006:  ldelem.ref
    IL_0007:  stloc.1
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.5
    IL_000a:  rem
    IL_000b:  brtrue.s   IL_0013

    IL_000d:  ldloc.1
    IL_000e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.1
    IL_0015:  add
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldarg.1
    IL_0019:  ldlen
    IL_001a:  conv.i4
    IL_001b:  blt.s      IL_0004

    IL_001d:  ret
  } // end of method Loops::NoForeachOverArray

  .method public hidebysig instance void 
          NestedLoops() cil managed
  {
    // Code size       53 (0x35)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_002f

    IL_0004:  ldloc.0
    IL_0005:  ldc.i4.2
    IL_0006:  rem
    IL_0007:  brtrue.s   IL_0021

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.1
    IL_000b:  br.s       IL_001b

    IL_000d:  ldstr      "Y"
    IL_0012:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0017:  ldloc.1
    IL_0018:  ldc.i4.1
    IL_0019:  add
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.5
    IL_001d:  blt.s      IL_000d

    IL_001f:  br.s       IL_002b

    IL_0021:  ldstr      "X"
    IL_0026:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002b:  ldloc.0
    IL_002c:  ldc.i4.1
    IL_002d:  add
    IL_002e:  stloc.0
    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.s   10
    IL_0032:  blt.s      IL_0004

    IL_0034:  ret
  } // end of method Loops::NestedLoops

  .method private hidebysig instance bool 
          Condition(string arg) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldstr      "Condition: "
    IL_0005:  ldarg.1
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldc.i4.0
    IL_0011:  ret
  } // end of method Loops::Condition

  .method public hidebysig instance void 
          WhileLoop() cil managed
  {
    // Code size       118 (0x76)
    .maxstack  2
    IL_0000:  ldstr      "Initial"
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000a:  ldarg.0
    IL_000b:  ldstr      "if"
    IL_0010:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0015:  brfalse.s  IL_006b

    IL_0017:  br.s       IL_0054

    IL_0019:  ldstr      "Loop Body"
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0023:  ldarg.0
    IL_0024:  ldstr      "test"
    IL_0029:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_002e:  brfalse.s  IL_004a

    IL_0030:  ldarg.0
    IL_0031:  ldstr      "continue"
    IL_0036:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_003b:  brtrue.s   IL_0054

    IL_003d:  ldarg.0
    IL_003e:  ldstr      "break"
    IL_0043:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0048:  brfalse.s  IL_0061

    IL_004a:  ldstr      "End of loop body"
    IL_004f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0054:  ldarg.0
    IL_0055:  ldstr      "while"
    IL_005a:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_005f:  brtrue.s   IL_0019

    IL_0061:  ldstr      "After loop"
    IL_0066:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006b:  ldstr      "End of method"
    IL_0070:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0075:  ret
  } // end of method Loops::WhileLoop

  .method public hidebysig instance void 
          ForLoop() cil managed
  {
    // Code size       124 (0x7c)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldstr      "Initial"
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000a:  ldarg.0
    IL_000b:  ldstr      "if"
    IL_0010:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0015:  brfalse.s  IL_0071

    IL_0017:  ldc.i4.0
    IL_0018:  stloc.0
    IL_0019:  br.s       IL_005a

    IL_001b:  ldstr      "Loop Body"
    IL_0020:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0025:  ldarg.0
    IL_0026:  ldstr      "test"
    IL_002b:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0030:  brfalse.s  IL_004c

    IL_0032:  ldarg.0
    IL_0033:  ldstr      "continue"
    IL_0038:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_003d:  brtrue.s   IL_0056

    IL_003f:  ldarg.0
    IL_0040:  ldstr      "not-break"
    IL_0045:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_004a:  brfalse.s  IL_0067

    IL_004c:  ldstr      "End of loop body"
    IL_0051:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0056:  ldloc.0
    IL_0057:  ldc.i4.1
    IL_0058:  add
    IL_0059:  stloc.0
    IL_005a:  ldarg.0
    IL_005b:  ldstr      "for"
    IL_0060:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
    IL_0065:  brtrue.s   IL_001b

    IL_0067:  ldstr      "After loop"
    IL_006c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0071:  ldstr      "End of method"
    IL_0076:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007b:  ret
  } // end of method Loops::ForLoop

  .method public hidebysig instance void 
          ReturnFromDoWhileInTryFinally() cil managed
  {
    // Code size       50 (0x32)
    .maxstack  2
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldstr      "return"
      IL_0006:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
      IL_000b:  brfalse.s  IL_000f

      IL_000d:  leave.s    IL_0031

      IL_000f:  ldarg.0
      IL_0010:  ldstr      "repeat"
      IL_0015:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::Condition(string)
      IL_001a:  brtrue.s   IL_0000

      IL_001c:  call       string[] [mscorlib]System.Environment::GetCommandLineArgs()
      IL_0021:  pop
      IL_0022:  leave.s    IL_002b

    }  // end .try
    finally
    {
      IL_0024:  call       string[] [mscorlib]System.Environment::GetCommandLineArgs()
      IL_0029:  pop
      IL_002a:  endfinally
    }  // end handler
    IL_002b:  call       string[] [mscorlib]System.Environment::GetCommandLineArgs()
    IL_0030:  pop
    IL_0031:  ret
  } // end of method Loops::ReturnFromDoWhileInTryFinally

  .method public hidebysig instance void 
          ForLoopWithEarlyReturn(int32[] ids) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  3
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/Item V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0018

    IL_0004:  ldnull
    IL_0005:  stloc.1
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  ldloc.0
    IL_0009:  ldelem.i4
    IL_000a:  ldloca.s   V_1
    IL_000c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::TryGetItem(int32,
                                                                                                       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops/Item&)
    IL_0011:  ldloc.1
    IL_0012:  brfalse.s  IL_001e

    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  ldarg.1
    IL_001a:  ldlen
    IL_001b:  conv.i4
    IL_001c:  blt.s      IL_0004

    IL_001e:  ret
  } // end of method Loops::ForLoopWithEarlyReturn

  .method public hidebysig instance void 
          ForeachLoopWithEarlyReturn(class [mscorlib]System.Collections.Generic.List`1<object> items) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  3
    .locals init (valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<object> V_0,
             object V_1,
             object V_2)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<object>::GetEnumerator()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  br.s       IL_001f

      IL_0009:  ldloca.s   V_0
      IL_000b:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<object>::get_Current()
      IL_0010:  stloc.1
      IL_0011:  ldarg.0
      IL_0012:  ldloc.1
      IL_0013:  dup
      IL_0014:  stloc.2
      IL_0015:  stfld      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Loops::someObject
      IL_001a:  ldloc.2
      IL_001b:  brtrue.s   IL_001f

      IL_001d:  leave.s    IL_0038

      IL_001f:  ldloca.s   V_0
      IL_0021:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<object>::MoveNext()
      IL_0026:  brtrue.s   IL_0009

      IL_0028:  leave.s    IL_0038

    }  // end .try
    finally
    {
      IL_002a:  ldloca.s   V_0
      IL_002c:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<object>
      IL_0032:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0037:  endfinally
    }  // end handler
    IL_0038:  ret
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
