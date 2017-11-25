
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



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
.assembly extern System.Xml
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly tngduxjw
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module tngduxjw.dll
// MVID: {416BD71B-B506-4355-8604-9F8C6A2FDEE6}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x04A60000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
       extends [mscorlib]System.Object
{
  .class auto ansi nested private beforefieldinit GenericClass`1<X>
         extends [mscorlib]System.Object
  {
    .field public static !X StaticField
    .field public !X InstanceField
    .field private static !X '<StaticProperty>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private !X '<InstanceProperty>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname static 
            !X  get_StaticProperty() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<!X>::'<StaticProperty>k__BackingField'
      IL_0005:  ret
    } // end of method GenericClass`1::get_StaticProperty

    .method public hidebysig specialname static 
            void  set_StaticProperty(!X 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  stsfld     !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<!X>::'<StaticProperty>k__BackingField'
      IL_0006:  ret
    } // end of method GenericClass`1::set_StaticProperty

    .method public hidebysig specialname 
            instance !X  get_InstanceProperty() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<!X>::'<InstanceProperty>k__BackingField'
      IL_0006:  ret
    } // end of method GenericClass`1::get_InstanceProperty

    .method public hidebysig specialname 
            instance void  set_InstanceProperty(!X 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<!X>::'<InstanceProperty>k__BackingField'
      IL_0007:  ret
    } // end of method GenericClass`1::set_InstanceProperty

    .method public hidebysig static bool 
            GenericMethod<Y>() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } // end of method GenericClass`1::GenericMethod

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method GenericClass`1::.ctor

    .property !X StaticProperty()
    {
      .get !X ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1::get_StaticProperty()
      .set void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1::set_StaticProperty(!X)
    } // end of property GenericClass`1::StaticProperty
    .property instance !X InstanceProperty()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1::set_InstanceProperty(!X)
      .get instance !X ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1::get_InstanceProperty()
    } // end of property GenericClass`1::InstanceProperty
  } // end of class GenericClass`1

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public bool a
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass0'::.ctor

  } // end of class '<>c__DisplayClass0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass2'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public bool a
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass2'::.ctor

  } // end of class '<>c__DisplayClass2'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass4'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 x
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass4'::.ctor

  } // end of class '<>c__DisplayClass4'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass8'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> dict
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass8'::.ctor

  } // end of class '<>c__DisplayClass8'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClassa'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 i
    .field public string x
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClassa'::.ctor

  } // end of class '<>c__DisplayClassa'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClassc'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public uint8 z
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClassc'::.ctor

  } // end of class '<>c__DisplayClassc'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass10'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [System.Core]System.Collections.Generic.HashSet`1<int32> set
    .field public class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> sink
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass10'::.ctor

  } // end of class '<>c__DisplayClass10'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass14'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> 'call'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass14'::.ctor

  } // end of class '<>c__DisplayClass14'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass16'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public bool x
    .field public int32 y
    .field public uint8 z
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass16'::.ctor

  } // end of class '<>c__DisplayClass16'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass19'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [System.Xml]System.Xml.XmlReaderSettings s
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass19'::.ctor

  } // end of class '<>c__DisplayClass19'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass1b'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 i
    .field public string x
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass1b'::.ctor

  } // end of class '<>c__DisplayClass1b'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass82'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 captured
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass82'::.ctor

    .method public hidebysig instance int32 
            '<Capturing>b__81'() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass82'::captured
      IL_0006:  ret
    } // end of method '<>c__DisplayClass82'::'<Capturing>b__81'

  } // end of class '<>c__DisplayClass82'

  .field private int32 'field'
  .field private static class [mscorlib]System.Func`2<int32,string> 'CS$<>9__CachedAnonymousMethodDelegate7'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> 'CS$<>9__CachedAnonymousMethodDelegatef'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> 'CS$<>9__CachedAnonymousMethodDelegate13'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<int32[],int32> 'CS$<>9__CachedAnonymousMethodDelegate22'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int32[],int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate23'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<int32[0...,0...],int32> 'CS$<>9__CachedAnonymousMethodDelegate24'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int32[0...,0...],int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate25'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int32[][],int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate26'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<int32[],int32> 'CS$<>9__CachedAnonymousMethodDelegate29'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<int32> 'CS$<>9__CachedAnonymousMethodDelegate2a'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<object> 'CS$<>9__CachedAnonymousMethodDelegate32'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<object> 'CS$<>9__CachedAnonymousMethodDelegate33'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<object> 'CS$<>9__CachedAnonymousMethodDelegate34'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<object> 'CS$<>9__CachedAnonymousMethodDelegate35'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<object> 'CS$<>9__CachedAnonymousMethodDelegate36'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<object> 'CS$<>9__CachedAnonymousMethodDelegate37'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<object> 'CS$<>9__CachedAnonymousMethodDelegate38'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<class [mscorlib]System.Type> 'CS$<>9__CachedAnonymousMethodDelegate3e'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<class [mscorlib]System.Type> 'CS$<>9__CachedAnonymousMethodDelegate3f'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<class [mscorlib]System.Type> 'CS$<>9__CachedAnonymousMethodDelegate40'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<class [mscorlib]System.Type> 'CS$<>9__CachedAnonymousMethodDelegate41'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<class [mscorlib]System.Type> 'CS$<>9__CachedAnonymousMethodDelegate42'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass> 'CS$<>9__CachedAnonymousMethodDelegate45'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>> 'CS$<>9__CachedAnonymousMethodDelegate46'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<object,bool> 'CS$<>9__CachedAnonymousMethodDelegate48'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<bool,bool> 'CS$<>9__CachedAnonymousMethodDelegate4a'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate4d'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate4e'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int32,int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate5e'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int32,int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate5f'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int32,int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate60'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int32,int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate61'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int32,int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate62'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int64,int32,int64> 'CS$<>9__CachedAnonymousMethodDelegate63'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int64,int32,int64> 'CS$<>9__CachedAnonymousMethodDelegate64'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int64,int32,int64> 'CS$<>9__CachedAnonymousMethodDelegate65'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int64,int32,int64> 'CS$<>9__CachedAnonymousMethodDelegate66'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int64,int32,int64> 'CS$<>9__CachedAnonymousMethodDelegate67'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int16,int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate68'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int32,int16,int32> 'CS$<>9__CachedAnonymousMethodDelegate69'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int16,int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate6a'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int32,int16,int32> 'CS$<>9__CachedAnonymousMethodDelegate6b'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int16,int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate6c'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate71'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int32,int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate72'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int32,int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate73'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<int32,int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate74'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate79'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate7a'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<int64,int64> 'CS$<>9__CachedAnonymousMethodDelegate7b'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<int64,int64> 'CS$<>9__CachedAnonymousMethodDelegate7c'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<int32> 'CS$<>9__CachedAnonymousMethodDelegate7f'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<int32,int32> 'CS$<>9__CachedAnonymousMethodDelegate80'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<string,string> 'CS$<>9__CachedAnonymousMethodDelegate88'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<int32,string> 'CS$<>9__CachedAnonymousMethodDelegate89'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<string,char[]> 'CS$<>9__CachedAnonymousMethodDelegate8a'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<bool> 'CS$<>9__CachedAnonymousMethodDelegate8b'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<bool> 'CS$<>9__CachedAnonymousMethodDelegate8d'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<int32[]> 'CS$<>9__CachedAnonymousMethodDelegate93'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<int32[]> 'CS$<>9__CachedAnonymousMethodDelegate94'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<int32[0...,0...]> 'CS$<>9__CachedAnonymousMethodDelegate95'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<int32[][]> 'CS$<>9__CachedAnonymousMethodDelegate96'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<int32[][]> 'CS$<>9__CachedAnonymousMethodDelegate97'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`1<object> 'CS$<>9__CachedAnonymousMethodDelegate99'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private hidebysig static object 
          ToCode<R>(object x,
                    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Action`1<!!R>> expr) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          ToCode<R>(object x,
                    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!R>> expr) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          ToCode<T,R>(object x,
                      class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!T,!!R>> expr) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          ToCode<T,T2,R>(object x,
                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!T,!!T2,!!R>> expr) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          X() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ret
  } // end of method ExpressionTrees::X

  .method public hidebysig instance void 
          Parameter(bool a) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass0'::a
    IL_000d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0012:  ldloc.0
    IL_0013:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0018:  ldtoken    field bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass0'::a
    IL_001d:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0022:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0027:  ldc.i4.0
    IL_0028:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0032:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0037:  pop
    IL_0038:  ret
  } // end of method ExpressionTrees::Parameter

  .method public hidebysig instance void 
          LocalVariable() cil managed
  {
    // Code size       57 (0x39)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass2' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass2'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass2'::a
    IL_000d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0012:  ldloc.0
    IL_0013:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0018:  ldtoken    field bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass2'::a
    IL_001d:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0022:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0027:  ldc.i4.0
    IL_0028:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0032:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0037:  pop
    IL_0038:  ret
  } // end of method ExpressionTrees::LocalVariable

  .method public hidebysig instance void 
          LambdaParameter() cil managed
  {
    // Code size       51 (0x33)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Boolean
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldstr      "a"
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0021:  stloc.1
    IL_0022:  ldloc.1
    IL_0023:  ldc.i4.0
    IL_0024:  ldloc.0
    IL_0025:  stelem.ref
    IL_0026:  ldloc.1
    IL_0027:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<bool,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_002c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool,bool>(object,
                                                                                                                 class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_0031:  pop
    IL_0032:  ret
  } // end of method ExpressionTrees::LambdaParameter

  .method public hidebysig instance void 
          AddOperator(int32 x) cil managed
  {
    // Code size       109 (0x6d)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass4' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass4'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass4'::x
    IL_000d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0012:  ldc.i4.1
    IL_0013:  box        [mscorlib]System.Int32
    IL_0018:  ldtoken    [mscorlib]System.Int32
    IL_001d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0022:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0027:  ldloc.0
    IL_0028:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_002d:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass4'::x
    IL_0032:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0037:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_003c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0041:  ldc.i4.2
    IL_0042:  box        [mscorlib]System.Int32
    IL_0047:  ldtoken    [mscorlib]System.Int32
    IL_004c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0051:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0056:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_005b:  ldc.i4.0
    IL_005c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0061:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0066:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006b:  pop
    IL_006c:  ret
  } // end of method ExpressionTrees::AddOperator

  .method public hidebysig instance void 
          AnonymousClasses() cil managed
  {
    // Code size       157 (0x9d)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [mscorlib]System.Reflection.MethodInfo[] V_1)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void class '<>f__AnonymousType0`2'<int32,string>::.ctor(!0,
                                                                                                 !1)
    IL_000a:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_000f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0019:  ldc.i4.2
    IL_001a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001f:  stloc.0
    IL_0020:  ldloc.0
    IL_0021:  ldc.i4.0
    IL_0022:  ldc.i4.3
    IL_0023:  box        [mscorlib]System.Int32
    IL_0028:  ldtoken    [mscorlib]System.Int32
    IL_002d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0032:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0037:  stelem.ref
    IL_0038:  ldloc.0
    IL_0039:  ldc.i4.1
    IL_003a:  ldstr      "a"
    IL_003f:  ldtoken    [mscorlib]System.String
    IL_0044:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0049:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004e:  stelem.ref
    IL_004f:  ldloc.0
    IL_0050:  ldc.i4.2
    IL_0051:  newarr     [mscorlib]System.Reflection.MethodInfo
    IL_0056:  stloc.1
    IL_0057:  ldloc.1
    IL_0058:  ldc.i4.0
    IL_0059:  ldtoken    method instance !0 class '<>f__AnonymousType0`2'<int32,string>::get_X()
    IL_005e:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_0063:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0068:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006d:  stelem.ref
    IL_006e:  ldloc.1
    IL_006f:  ldc.i4.1
    IL_0070:  ldtoken    method instance !1 class '<>f__AnonymousType0`2'<int32,string>::get_A()
    IL_0075:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_007a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0084:  stelem.ref
    IL_0085:  ldloc.1
    IL_0086:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_008b:  ldc.i4.0
    IL_008c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0091:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class '<>f__AnonymousType0`2'<int32,string>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0096:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class '<>f__AnonymousType0`2'<int32,string>>(object,
                                                                                                                                                   class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_009b:  pop
    IL_009c:  ret
  } // end of method ExpressionTrees::AnonymousClasses

  .method public hidebysig instance void 
          ArrayIndex() cil managed
  {
    // Code size       232 (0xe8)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldc.i4.3
    IL_0010:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.0
    IL_0018:  ldc.i4.3
    IL_0019:  box        [mscorlib]System.Int32
    IL_001e:  ldtoken    [mscorlib]System.Int32
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002d:  stelem.ref
    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.1
    IL_0030:  ldc.i4.4
    IL_0031:  box        [mscorlib]System.Int32
    IL_0036:  ldtoken    [mscorlib]System.Int32
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0045:  stelem.ref
    IL_0046:  ldloc.0
    IL_0047:  ldc.i4.2
    IL_0048:  ldc.i4.5
    IL_0049:  box        [mscorlib]System.Int32
    IL_004e:  ldtoken    [mscorlib]System.Int32
    IL_0053:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0058:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005d:  stelem.ref
    IL_005e:  ldloc.0
    IL_005f:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0064:  ldc.i4.0
    IL_0065:  box        [mscorlib]System.Int32
    IL_006a:  ldtoken    [mscorlib]System.Int32
    IL_006f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0074:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0079:  ldnull
    IL_007a:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_007f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0084:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0089:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_008e:  ldtoken    method instance int64 [mscorlib]System.DateTime::get_Ticks()
    IL_0093:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0098:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00a2:  ldc.i4.3
    IL_00a3:  conv.i8
    IL_00a4:  box        [mscorlib]System.Int64
    IL_00a9:  ldtoken    [mscorlib]System.Int64
    IL_00ae:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b8:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_00bd:  ldtoken    [mscorlib]System.Int32
    IL_00c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00cc:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_00d1:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_00d6:  ldc.i4.0
    IL_00d7:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00dc:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e1:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00e6:  pop
    IL_00e7:  ret
  } // end of method ExpressionTrees::ArrayIndex

  .method public hidebysig instance void 
          ArrayLengthAndDoubles() cil managed
  {
    // Code size       301 (0x12d)
    .maxstack  14
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method !!0[] [System.Core]System.Linq.Enumerable::ToArray<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.1
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldnull
    IL_001f:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Concat<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0024:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0029:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002e:  ldc.i4.2
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  stloc.1
    IL_0035:  ldloc.1
    IL_0036:  ldc.i4.0
    IL_0037:  ldtoken    [mscorlib]System.Double
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  ldc.i4.3
    IL_0042:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0047:  stloc.2
    IL_0048:  ldloc.2
    IL_0049:  ldc.i4.0
    IL_004a:  ldc.r8     1.
    IL_0053:  box        [mscorlib]System.Double
    IL_0058:  ldtoken    [mscorlib]System.Double
    IL_005d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0062:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0067:  stelem.ref
    IL_0068:  ldloc.2
    IL_0069:  ldc.i4.1
    IL_006a:  ldc.r8     2.0099999999999998
    IL_0073:  box        [mscorlib]System.Double
    IL_0078:  ldtoken    [mscorlib]System.Double
    IL_007d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0082:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0087:  stelem.ref
    IL_0088:  ldloc.2
    IL_0089:  ldc.i4.2
    IL_008a:  ldc.r8     3.5
    IL_0093:  box        [mscorlib]System.Double
    IL_0098:  ldtoken    [mscorlib]System.Double
    IL_009d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a7:  stelem.ref
    IL_00a8:  ldloc.2
    IL_00a9:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ae:  stelem.ref
    IL_00af:  ldloc.1
    IL_00b0:  ldc.i4.1
    IL_00b1:  ldtoken    [mscorlib]System.Double
    IL_00b6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bb:  ldc.i4.2
    IL_00bc:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00c1:  stloc.3
    IL_00c2:  ldloc.3
    IL_00c3:  ldc.i4.0
    IL_00c4:  ldc.r8     1.
    IL_00cd:  box        [mscorlib]System.Double
    IL_00d2:  ldtoken    [mscorlib]System.Double
    IL_00d7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e1:  stelem.ref
    IL_00e2:  ldloc.3
    IL_00e3:  ldc.i4.1
    IL_00e4:  ldc.r8     2.
    IL_00ed:  box        [mscorlib]System.Double
    IL_00f2:  ldtoken    [mscorlib]System.Double
    IL_00f7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0101:  stelem.ref
    IL_0102:  ldloc.3
    IL_0103:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0108:  stelem.ref
    IL_0109:  ldloc.1
    IL_010a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_010f:  stelem.ref
    IL_0110:  ldloc.0
    IL_0111:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0116:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayLength(class [System.Core]System.Linq.Expressions.Expression)
    IL_011b:  ldc.i4.0
    IL_011c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0121:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0126:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_012b:  pop
    IL_012c:  ret
  } // end of method ExpressionTrees::ArrayLengthAndDoubles

  .method public hidebysig instance void 
          AsOperator() cil managed
  {
    // Code size       64 (0x40)
    .maxstack  3
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0014:  ldc.i4.0
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001a:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_001f:  ldtoken    [mscorlib]System.String
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::TypeAs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Type)
    IL_002e:  ldc.i4.0
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0034:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0039:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_003e:  pop
    IL_003f:  ret
  } // end of method ExpressionTrees::AsOperator

  .method public hidebysig instance void 
          ComplexGenericName() cil managed
  {
    // Code size       140 (0x8c)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldstr      "x"
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.0
    IL_001c:  box        [mscorlib]System.Int32
    IL_0021:  ldtoken    [mscorlib]System.Int32
    IL_0026:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0030:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0035:  ldc.i4.1
    IL_0036:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003b:  stloc.1
    IL_003c:  ldloc.1
    IL_003d:  ldc.i4.0
    IL_003e:  ldloc.0
    IL_003f:  stelem.ref
    IL_0040:  ldloc.1
    IL_0041:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0046:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_004b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0050:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0055:  ldc.i4.1
    IL_0056:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_005b:  stloc.2
    IL_005c:  ldloc.2
    IL_005d:  ldc.i4.0
    IL_005e:  ldc.i4.0
    IL_005f:  box        [mscorlib]System.Int32
    IL_0064:  ldtoken    [mscorlib]System.Int32
    IL_0069:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0073:  stelem.ref
    IL_0074:  ldloc.2
    IL_0075:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_007a:  ldc.i4.0
    IL_007b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0080:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0085:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008a:  pop
    IL_008b:  ret
  } // end of method ExpressionTrees::ComplexGenericName

  .method public hidebysig instance void 
          DefaultValue() cil managed
  {
    // Code size       173 (0xad)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             valuetype [mscorlib]System.TimeSpan V_1)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void [mscorlib]System.TimeSpan::.ctor(int32,
                                                                               int32,
                                                                               int32)
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0014:  ldc.i4.3
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.0
    IL_001d:  ldc.i4.1
    IL_001e:  box        [mscorlib]System.Int32
    IL_0023:  ldtoken    [mscorlib]System.Int32
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  stelem.ref
    IL_0033:  ldloc.0
    IL_0034:  ldc.i4.1
    IL_0035:  ldc.i4.2
    IL_0036:  box        [mscorlib]System.Int32
    IL_003b:  ldtoken    [mscorlib]System.Int32
    IL_0040:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0045:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004a:  stelem.ref
    IL_004b:  ldloc.0
    IL_004c:  ldc.i4.2
    IL_004d:  ldc.i4.3
    IL_004e:  box        [mscorlib]System.Int32
    IL_0053:  ldtoken    [mscorlib]System.Int32
    IL_0058:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0062:  stelem.ref
    IL_0063:  ldloc.0
    IL_0064:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0069:  ldloca.s   V_1
    IL_006b:  initobj    [mscorlib]System.TimeSpan
    IL_0071:  ldloc.1
    IL_0072:  box        [mscorlib]System.TimeSpan
    IL_0077:  ldtoken    [mscorlib]System.TimeSpan
    IL_007c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0081:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0086:  ldc.i4.0
    IL_0087:  ldtoken    method bool [mscorlib]System.TimeSpan::op_Equality(valuetype [mscorlib]System.TimeSpan,
                                                                            valuetype [mscorlib]System.TimeSpan)
    IL_008c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0091:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0096:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_009b:  ldc.i4.0
    IL_009c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a6:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ab:  pop
    IL_00ac:  ret
  } // end of method ExpressionTrees::DefaultValue

  .method public hidebysig instance void 
          EnumConstant() cil managed
  {
    // Code size       116 (0x74)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0014:  ldc.i4.0
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001a:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_001f:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_0024:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0029:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002e:  ldc.i4.1
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.0
    IL_0037:  ldc.i4.0
    IL_0038:  box        [mscorlib]System.MidpointRounding
    IL_003d:  ldtoken    [mscorlib]System.MidpointRounding
    IL_0042:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0047:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004c:  ldtoken    [mscorlib]System.Object
    IL_0051:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0056:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_005b:  stelem.ref
    IL_005c:  ldloc.0
    IL_005d:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0062:  ldc.i4.0
    IL_0063:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0068:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0072:  pop
    IL_0073:  ret
  } // end of method ExpressionTrees::EnumConstant

  .method public hidebysig instance void 
          IndexerAccess() cil managed
  {
    // Code size       180 (0xb4)
    .maxstack  7
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8' V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  ldc.i4.s   20
    IL_000a:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [System.Core]System.Linq.Enumerable::Range(int32,
                                                                                                                                    int32)
    IL_000f:  ldsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_0014:  brtrue.s   IL_0027

    IL_0016:  ldnull
    IL_0017:  ldftn      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<IndexerAccess>b__6'(int32)
    IL_001d:  newobj     instance void class [mscorlib]System.Func`2<int32,string>::.ctor(object,
                                                                                          native int)
    IL_0022:  stsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_0027:  ldsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_002c:  call       class [mscorlib]System.Collections.Generic.Dictionary`2<!!1,!!0> [System.Core]System.Linq.Enumerable::ToDictionary<int32,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                          class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0031:  stfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8'::dict
    IL_0036:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_003b:  ldloc.0
    IL_003c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0041:  ldtoken    field class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8'::dict
    IL_0046:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0050:  ldtoken    method instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::get_Item(!0)
    IL_0055:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>
    IL_005a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0064:  ldc.i4.1
    IL_0065:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_006a:  stloc.1
    IL_006b:  ldloc.1
    IL_006c:  ldc.i4.0
    IL_006d:  ldstr      "3"
    IL_0072:  ldtoken    [mscorlib]System.String
    IL_0077:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0081:  stelem.ref
    IL_0082:  ldloc.1
    IL_0083:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0088:  ldc.i4.3
    IL_0089:  box        [mscorlib]System.Int32
    IL_008e:  ldtoken    [mscorlib]System.Int32
    IL_0093:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0098:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00a2:  ldc.i4.0
    IL_00a3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ad:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b2:  pop
    IL_00b3:  ret
  } // end of method ExpressionTrees::IndexerAccess

  .method public hidebysig instance void 
          IsOperator() cil managed
  {
    // Code size       64 (0x40)
    .maxstack  3
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0014:  ldc.i4.0
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001a:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_001f:  ldtoken    [mscorlib]System.String
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  call       class [System.Core]System.Linq.Expressions.TypeBinaryExpression [System.Core]System.Linq.Expressions.Expression::TypeIs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002e:  ldc.i4.0
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0034:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0039:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_003e:  pop
    IL_003f:  ret
  } // end of method ExpressionTrees::IsOperator

  .method public hidebysig instance void 
          ListInitializer() cil managed
  {
    // Code size       370 (0x172)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.ElementInit[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::.ctor()
    IL_000a:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_000f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0019:  ldc.i4.0
    IL_001a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0024:  ldc.i4.3
    IL_0025:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_002a:  stloc.0
    IL_002b:  ldloc.0
    IL_002c:  ldc.i4.0
    IL_002d:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_0032:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0037:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0041:  ldc.i4.2
    IL_0042:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0047:  stloc.1
    IL_0048:  ldloc.1
    IL_0049:  ldc.i4.0
    IL_004a:  ldc.i4.1
    IL_004b:  box        [mscorlib]System.Int32
    IL_0050:  ldtoken    [mscorlib]System.Int32
    IL_0055:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005f:  stelem.ref
    IL_0060:  ldloc.1
    IL_0061:  ldc.i4.1
    IL_0062:  ldc.i4.1
    IL_0063:  box        [mscorlib]System.Int32
    IL_0068:  ldtoken    [mscorlib]System.Int32
    IL_006d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0072:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0077:  stelem.ref
    IL_0078:  ldloc.1
    IL_0079:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_007e:  stelem.ref
    IL_007f:  ldloc.0
    IL_0080:  ldc.i4.1
    IL_0081:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_0086:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_008b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0090:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0095:  ldc.i4.2
    IL_0096:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_009b:  stloc.2
    IL_009c:  ldloc.2
    IL_009d:  ldc.i4.0
    IL_009e:  ldc.i4.2
    IL_009f:  box        [mscorlib]System.Int32
    IL_00a4:  ldtoken    [mscorlib]System.Int32
    IL_00a9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ae:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b3:  stelem.ref
    IL_00b4:  ldloc.2
    IL_00b5:  ldc.i4.1
    IL_00b6:  ldc.i4.2
    IL_00b7:  box        [mscorlib]System.Int32
    IL_00bc:  ldtoken    [mscorlib]System.Int32
    IL_00c1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cb:  stelem.ref
    IL_00cc:  ldloc.2
    IL_00cd:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00d2:  stelem.ref
    IL_00d3:  ldloc.0
    IL_00d4:  ldc.i4.2
    IL_00d5:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_00da:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_00df:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e4:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e9:  ldc.i4.2
    IL_00ea:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ef:  stloc.3
    IL_00f0:  ldloc.3
    IL_00f1:  ldc.i4.0
    IL_00f2:  ldc.i4.3
    IL_00f3:  box        [mscorlib]System.Int32
    IL_00f8:  ldtoken    [mscorlib]System.Int32
    IL_00fd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0102:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0107:  stelem.ref
    IL_0108:  ldloc.3
    IL_0109:  ldc.i4.1
    IL_010a:  ldc.i4.4
    IL_010b:  box        [mscorlib]System.Int32
    IL_0110:  ldtoken    [mscorlib]System.Int32
    IL_0115:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_011f:  stelem.ref
    IL_0120:  ldloc.3
    IL_0121:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0126:  stelem.ref
    IL_0127:  ldloc.0
    IL_0128:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_012d:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::get_Count()
    IL_0132:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0137:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0141:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0146:  ldc.i4.3
    IL_0147:  box        [mscorlib]System.Int32
    IL_014c:  ldtoken    [mscorlib]System.Int32
    IL_0151:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0156:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_015b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0160:  ldc.i4.0
    IL_0161:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0166:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_016b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0170:  pop
    IL_0171:  ret
  } // end of method ExpressionTrees::ListInitializer

  .method public hidebysig instance void 
          ListInitializer2() cil managed
  {
    // Code size       328 (0x148)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.ElementInit[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3,
             class [System.Core]System.Linq.Expressions.Expression[] V_4)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor(int32)
    IL_000a:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_000f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0019:  ldc.i4.1
    IL_001a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001f:  stloc.0
    IL_0020:  ldloc.0
    IL_0021:  ldc.i4.0
    IL_0022:  ldc.i4.s   50
    IL_0024:  box        [mscorlib]System.Int32
    IL_0029:  ldtoken    [mscorlib]System.Int32
    IL_002e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0038:  stelem.ref
    IL_0039:  ldloc.0
    IL_003a:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_003f:  ldc.i4.3
    IL_0040:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_0045:  stloc.1
    IL_0046:  ldloc.1
    IL_0047:  ldc.i4.0
    IL_0048:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_004d:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0052:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005c:  ldc.i4.1
    IL_005d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0062:  stloc.2
    IL_0063:  ldloc.2
    IL_0064:  ldc.i4.0
    IL_0065:  ldc.i4.1
    IL_0066:  box        [mscorlib]System.Int32
    IL_006b:  ldtoken    [mscorlib]System.Int32
    IL_0070:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0075:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007a:  stelem.ref
    IL_007b:  ldloc.2
    IL_007c:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0081:  stelem.ref
    IL_0082:  ldloc.1
    IL_0083:  ldc.i4.1
    IL_0084:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0089:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_008e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0093:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0098:  ldc.i4.1
    IL_0099:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_009e:  stloc.3
    IL_009f:  ldloc.3
    IL_00a0:  ldc.i4.0
    IL_00a1:  ldc.i4.2
    IL_00a2:  box        [mscorlib]System.Int32
    IL_00a7:  ldtoken    [mscorlib]System.Int32
    IL_00ac:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b6:  stelem.ref
    IL_00b7:  ldloc.3
    IL_00b8:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00bd:  stelem.ref
    IL_00be:  ldloc.1
    IL_00bf:  ldc.i4.2
    IL_00c0:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00c5:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00ca:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cf:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00d4:  ldc.i4.1
    IL_00d5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00da:  stloc.s    V_4
    IL_00dc:  ldloc.s    V_4
    IL_00de:  ldc.i4.0
    IL_00df:  ldc.i4.3
    IL_00e0:  box        [mscorlib]System.Int32
    IL_00e5:  ldtoken    [mscorlib]System.Int32
    IL_00ea:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ef:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00f4:  stelem.ref
    IL_00f5:  ldloc.s    V_4
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00fc:  stelem.ref
    IL_00fd:  ldloc.1
    IL_00fe:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_0103:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_0108:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_010d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0112:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0117:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_011c:  ldc.i4.3
    IL_011d:  box        [mscorlib]System.Int32
    IL_0122:  ldtoken    [mscorlib]System.Int32
    IL_0127:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_012c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0131:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0136:  ldc.i4.0
    IL_0137:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_013c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0141:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0146:  pop
    IL_0147:  ret
  } // end of method ExpressionTrees::ListInitializer2

  .method public hidebysig instance void 
          ListInitializer3() cil managed
  {
    // Code size       298 (0x12a)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.ElementInit[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_000a:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_000f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0019:  ldc.i4.0
    IL_001a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0024:  ldc.i4.3
    IL_0025:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_002a:  stloc.0
    IL_002b:  ldloc.0
    IL_002c:  ldc.i4.0
    IL_002d:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0032:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0037:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0041:  ldc.i4.1
    IL_0042:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0047:  stloc.1
    IL_0048:  ldloc.1
    IL_0049:  ldc.i4.0
    IL_004a:  ldc.i4.1
    IL_004b:  box        [mscorlib]System.Int32
    IL_0050:  ldtoken    [mscorlib]System.Int32
    IL_0055:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005f:  stelem.ref
    IL_0060:  ldloc.1
    IL_0061:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0066:  stelem.ref
    IL_0067:  ldloc.0
    IL_0068:  ldc.i4.1
    IL_0069:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_006e:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0073:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0078:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007d:  ldc.i4.1
    IL_007e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0083:  stloc.2
    IL_0084:  ldloc.2
    IL_0085:  ldc.i4.0
    IL_0086:  ldc.i4.2
    IL_0087:  box        [mscorlib]System.Int32
    IL_008c:  ldtoken    [mscorlib]System.Int32
    IL_0091:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0096:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009b:  stelem.ref
    IL_009c:  ldloc.2
    IL_009d:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00a2:  stelem.ref
    IL_00a3:  ldloc.0
    IL_00a4:  ldc.i4.2
    IL_00a5:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00aa:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00af:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b4:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00b9:  ldc.i4.1
    IL_00ba:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00bf:  stloc.3
    IL_00c0:  ldloc.3
    IL_00c1:  ldc.i4.0
    IL_00c2:  ldc.i4.3
    IL_00c3:  box        [mscorlib]System.Int32
    IL_00c8:  ldtoken    [mscorlib]System.Int32
    IL_00cd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d7:  stelem.ref
    IL_00d8:  ldloc.3
    IL_00d9:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00de:  stelem.ref
    IL_00df:  ldloc.0
    IL_00e0:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_00e5:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_00ea:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00ef:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f4:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00f9:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00fe:  ldc.i4.3
    IL_00ff:  box        [mscorlib]System.Int32
    IL_0104:  ldtoken    [mscorlib]System.Int32
    IL_0109:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0113:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0118:  ldc.i4.0
    IL_0119:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_011e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0123:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0128:  pop
    IL_0129:  ret
  } // end of method ExpressionTrees::ListInitializer3

  .method public hidebysig instance void 
          LiteralCharAndProperty() cil managed
  {
    // Code size       146 (0x92)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void [mscorlib]System.String::.ctor(char,
                                                                             int32)
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0014:  ldc.i4.2
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.0
    IL_001d:  ldc.i4.s   32
    IL_001f:  box        [mscorlib]System.Char
    IL_0024:  ldtoken    [mscorlib]System.Char
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0033:  stelem.ref
    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.1
    IL_0036:  ldc.i4.3
    IL_0037:  box        [mscorlib]System.Int32
    IL_003c:  ldtoken    [mscorlib]System.Int32
    IL_0041:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004b:  stelem.ref
    IL_004c:  ldloc.0
    IL_004d:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0052:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0057:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_005c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0061:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0066:  ldc.i4.1
    IL_0067:  box        [mscorlib]System.Int32
    IL_006c:  ldtoken    [mscorlib]System.Int32
    IL_0071:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0076:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0080:  ldc.i4.0
    IL_0081:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0086:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0090:  pop
    IL_0091:  ret
  } // end of method ExpressionTrees::LiteralCharAndProperty

  .method public hidebysig instance void 
          CharNoCast() cil managed
  {
    // Code size       137 (0x89)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldstr      "abc"
    IL_000a:  ldtoken    [mscorlib]System.String
    IL_000f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0019:  ldtoken    method instance char [mscorlib]System.String::get_Chars(int32)
    IL_001e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0023:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0028:  ldc.i4.1
    IL_0029:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002e:  stloc.0
    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.0
    IL_0031:  ldc.i4.1
    IL_0032:  box        [mscorlib]System.Int32
    IL_0037:  ldtoken    [mscorlib]System.Int32
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0046:  stelem.ref
    IL_0047:  ldloc.0
    IL_0048:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004d:  ldtoken    [mscorlib]System.Int32
    IL_0052:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_005c:  ldc.i4.s   98
    IL_005e:  box        [mscorlib]System.Int32
    IL_0063:  ldtoken    [mscorlib]System.Int32
    IL_0068:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0072:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0077:  ldc.i4.0
    IL_0078:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0082:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0087:  pop
    IL_0088:  ret
  } // end of method ExpressionTrees::CharNoCast

  .method public hidebysig instance void 
          StringsImplicitCast() cil managed
  {
    // Code size       376 (0x178)
    .maxstack  8
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassa' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassa'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassa'::i
    IL_000d:  ldloc.0
    IL_000e:  ldstr      "X"
    IL_0013:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassa'::x
    IL_0018:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_001d:  ldstr      "a\n\\b"
    IL_0022:  ldtoken    [mscorlib]System.String
    IL_0027:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0031:  ldloc.0
    IL_0032:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0037:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassa'::x
    IL_003c:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_004b:  ldloc.0
    IL_004c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0051:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassa'::x
    IL_0056:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_005b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0060:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0065:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_006a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0074:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0079:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0083:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0088:  ldc.i4.2
    IL_0089:  box        [mscorlib]System.Int32
    IL_008e:  ldtoken    [mscorlib]System.Int32
    IL_0093:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0098:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00a2:  ldc.i4.0
    IL_00a3:  box        [mscorlib]System.Boolean
    IL_00a8:  ldtoken    [mscorlib]System.Boolean
    IL_00ad:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b7:  ldc.i4.1
    IL_00b8:  box        [mscorlib]System.Boolean
    IL_00bd:  ldtoken    [mscorlib]System.Boolean
    IL_00c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cc:  ldc.i4.1
    IL_00cd:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_00d2:  box        [mscorlib]System.Decimal
    IL_00d7:  ldtoken    [mscorlib]System.Decimal
    IL_00dc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e6:  ldloc.0
    IL_00e7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00ec:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassa'::i
    IL_00f1:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f6:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00fb:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_0100:  ldtoken    [mscorlib]System.Decimal
    IL_0105:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010a:  ldtoken    method valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
    IL_010f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0114:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0119:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type,
                                                                                                                                             class [mscorlib]System.Reflection.MethodInfo)
    IL_011e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0123:  ldc.i4.0
    IL_0124:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0129:  box        [mscorlib]System.Decimal
    IL_012e:  ldtoken    [mscorlib]System.Decimal
    IL_0133:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0138:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_013d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0142:  ldc.i4.0
    IL_0143:  box        [mscorlib]System.Boolean
    IL_0148:  ldtoken    [mscorlib]System.Boolean
    IL_014d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0152:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0157:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_015c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0161:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0166:  ldc.i4.0
    IL_0167:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_016c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0171:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0176:  pop
    IL_0177:  ret
  } // end of method ExpressionTrees::StringsImplicitCast

  .method public hidebysig instance void 
          NotImplicitCast() cil managed
  {
    // Code size       104 (0x68)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassc' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassc'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.s   42
    IL_0009:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassc'::z
    IL_000e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0013:  ldloc.0
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0019:  ldtoken    field uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassc'::z
    IL_001e:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0023:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0028:  ldtoken    [mscorlib]System.Int32
    IL_002d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0032:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0037:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_003c:  ldc.i4.0
    IL_003d:  box        [mscorlib]System.Int32
    IL_0042:  ldtoken    [mscorlib]System.Int32
    IL_0047:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0051:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0056:  ldc.i4.0
    IL_0057:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_005c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0061:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0066:  pop
    IL_0067:  ret
  } // end of method ExpressionTrees::NotImplicitCast

  .method public hidebysig instance void 
          MembersBuiltin() cil managed
  {
    // Code size       405 (0x195)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldc.i4.s   123
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.2
    IL_000b:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0010:  box        [mscorlib]System.Decimal
    IL_0015:  ldtoken    [mscorlib]System.Decimal
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0024:  ldtoken    method instance string [mscorlib]System.Decimal::ToString()
    IL_0029:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0033:  ldc.i4.0
    IL_0034:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0039:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_003e:  ldc.i4.0
    IL_003f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0044:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0049:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_004e:  pop
    IL_004f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0054:  ldc.i4     0x7fff
    IL_0059:  box        [mscorlib]System.AttributeTargets
    IL_005e:  ldtoken    [mscorlib]System.AttributeTargets
    IL_0063:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0068:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006d:  ldtoken    method instance bool [mscorlib]System.Enum::HasFlag(class [mscorlib]System.Enum)
    IL_0072:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0077:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007c:  ldc.i4.1
    IL_007d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0082:  stloc.0
    IL_0083:  ldloc.0
    IL_0084:  ldc.i4.0
    IL_0085:  ldc.i4.1
    IL_0086:  box        [mscorlib]System.AttributeTargets
    IL_008b:  ldtoken    [mscorlib]System.AttributeTargets
    IL_0090:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0095:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009a:  ldtoken    [mscorlib]System.Enum
    IL_009f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00a9:  stelem.ref
    IL_00aa:  ldloc.0
    IL_00ab:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00b0:  ldc.i4.0
    IL_00b1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00bb:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00c0:  pop
    IL_00c1:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00c6:  ldstr      "abc"
    IL_00cb:  ldtoken    [mscorlib]System.String
    IL_00d0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00da:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_00df:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00e4:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e9:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00ee:  ldc.i4.3
    IL_00ef:  box        [mscorlib]System.Int32
    IL_00f4:  ldtoken    [mscorlib]System.Int32
    IL_00f9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fe:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0103:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0108:  ldc.i4.0
    IL_0109:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_010e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0113:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0118:  pop
    IL_0119:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_011e:  ldc.i4.s   97
    IL_0120:  box        [mscorlib]System.Char
    IL_0125:  ldtoken    [mscorlib]System.Char
    IL_012a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_012f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0134:  ldtoken    method instance int32 [mscorlib]System.Char::CompareTo(char)
    IL_0139:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_013e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0143:  ldc.i4.1
    IL_0144:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0149:  stloc.1
    IL_014a:  ldloc.1
    IL_014b:  ldc.i4.0
    IL_014c:  ldc.i4.s   98
    IL_014e:  box        [mscorlib]System.Char
    IL_0153:  ldtoken    [mscorlib]System.Char
    IL_0158:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_015d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0162:  stelem.ref
    IL_0163:  ldloc.1
    IL_0164:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0169:  ldc.i4.0
    IL_016a:  box        [mscorlib]System.Int32
    IL_016f:  ldtoken    [mscorlib]System.Int32
    IL_0174:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0179:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_017e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0183:  ldc.i4.0
    IL_0184:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0189:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_018e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0193:  pop
    IL_0194:  ret
  } // end of method ExpressionTrees::MembersBuiltin

  .method public hidebysig instance void 
          MembersDefault() cil managed
  {
    // Code size       588 (0x24c)
    .maxstack  7
    .locals init (valuetype [mscorlib]System.DateTime V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldloca.s   V_0
    IL_0007:  initobj    [mscorlib]System.DateTime
    IL_000d:  ldloc.0
    IL_000e:  box        [mscorlib]System.DateTime
    IL_0013:  ldtoken    [mscorlib]System.DateTime
    IL_0018:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0022:  ldtoken    method instance int64 [mscorlib]System.DateTime::get_Ticks()
    IL_0027:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0031:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0036:  ldc.i4.0
    IL_0037:  conv.i8
    IL_0038:  box        [mscorlib]System.Int64
    IL_003d:  ldtoken    [mscorlib]System.Int64
    IL_0042:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0047:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0051:  ldc.i4.0
    IL_0052:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0057:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0061:  pop
    IL_0062:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0067:  ldnull
    IL_0068:  box        [mscorlib]System.Array
    IL_006d:  ldtoken    [mscorlib]System.Array
    IL_0072:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0077:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007c:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_0081:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0086:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_008b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0090:  ldc.i4.0
    IL_0091:  box        [mscorlib]System.Int32
    IL_0096:  ldtoken    [mscorlib]System.Int32
    IL_009b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a5:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00aa:  ldc.i4.0
    IL_00ab:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b5:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ba:  pop
    IL_00bb:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00c0:  ldnull
    IL_00c1:  box        [mscorlib]System.Type
    IL_00c6:  ldtoken    [mscorlib]System.Type
    IL_00cb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d5:  ldtoken    method instance bool [mscorlib]System.Type::get_IsLayoutSequential()
    IL_00da:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00df:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e4:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00e9:  ldc.i4.0
    IL_00ea:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00ef:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00f4:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00f9:  pop
    IL_00fa:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00ff:  ldnull
    IL_0100:  box        class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0105:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_010a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0114:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_0119:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_011e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0123:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0128:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_012d:  ldc.i4.0
    IL_012e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0133:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0138:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_013d:  pop
    IL_013e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0143:  ldnull
    IL_0144:  box        [mscorlib]System.Array
    IL_0149:  ldtoken    [mscorlib]System.Array
    IL_014e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0153:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0158:  ldtoken    method instance object [mscorlib]System.Array::Clone()
    IL_015d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0162:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0167:  ldc.i4.0
    IL_0168:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_016d:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0172:  ldnull
    IL_0173:  box        [mscorlib]System.Object
    IL_0178:  ldtoken    [mscorlib]System.Object
    IL_017d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0182:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0187:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_018c:  ldc.i4.0
    IL_018d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0192:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0197:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_019c:  pop
    IL_019d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_01a2:  ldnull
    IL_01a3:  box        [mscorlib]System.Type
    IL_01a8:  ldtoken    [mscorlib]System.Type
    IL_01ad:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01b7:  ldtoken    method instance bool [mscorlib]System.Type::IsInstanceOfType(object)
    IL_01bc:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01c1:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01c6:  ldc.i4.1
    IL_01c7:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01cc:  stloc.1
    IL_01cd:  ldloc.1
    IL_01ce:  ldc.i4.0
    IL_01cf:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_01d4:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01d9:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_01de:  ldc.i4.0
    IL_01df:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01e4:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_01e9:  stelem.ref
    IL_01ea:  ldloc.1
    IL_01eb:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01f0:  ldc.i4.0
    IL_01f1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01f6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01fb:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0200:  pop
    IL_0201:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0206:  ldnull
    IL_0207:  box        class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_020c:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0211:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0216:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_021b:  ldtoken    method instance class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::AsReadOnly()
    IL_0220:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0225:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_022a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_022f:  ldc.i4.0
    IL_0230:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0235:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_023a:  ldc.i4.0
    IL_023b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0240:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0245:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<int32>>(object,
                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_024a:  pop
    IL_024b:  ret
  } // end of method ExpressionTrees::MembersDefault

  .method public hidebysig instance void 
          DoAssert() cil managed
  {
    // Code size       398 (0x18e)
    .maxstack  8
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldarg.0
    IL_0006:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_000b:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0010:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001a:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'field'
    IL_001f:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0024:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0029:  ldarg.0
    IL_002a:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_002f:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0034:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0039:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_003e:  ldtoken    method instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::C()
    IL_0043:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0048:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_004d:  ldc.i4.0
    IL_004e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0053:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0058:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_005d:  ldc.i4.0
    IL_005e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0063:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0068:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006d:  pop
    IL_006e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0073:  ldnull
    IL_0074:  ldtoken    method bool [mscorlib]System.Object::ReferenceEquals(object,
                                                                              object)
    IL_0079:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0083:  ldc.i4.2
    IL_0084:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0089:  stloc.0
    IL_008a:  ldloc.0
    IL_008b:  ldc.i4.0
    IL_008c:  ldarg.0
    IL_008d:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0092:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0097:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a1:  stelem.ref
    IL_00a2:  ldloc.0
    IL_00a3:  ldc.i4.1
    IL_00a4:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::.ctor()
    IL_00a9:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00ae:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_00b3:  ldc.i4.0
    IL_00b4:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00b9:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_00be:  stelem.ref
    IL_00bf:  ldloc.0
    IL_00c0:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00c5:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00ca:  ldc.i4.0
    IL_00cb:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00d0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00d5:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00da:  pop
    IL_00db:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00e0:  ldarg.0
    IL_00e1:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_00e6:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_00eb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00f5:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::MyEquals(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees)
    IL_00fa:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00ff:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0104:  ldc.i4.1
    IL_0105:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_010a:  stloc.1
    IL_010b:  ldloc.1
    IL_010c:  ldc.i4.0
    IL_010d:  ldarg.0
    IL_010e:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0113:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0118:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0122:  stelem.ref
    IL_0123:  ldloc.1
    IL_0124:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0129:  ldarg.0
    IL_012a:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_012f:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0134:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0139:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_013e:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::MyEquals(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees)
    IL_0143:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0148:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_014d:  ldc.i4.1
    IL_014e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0153:  stloc.2
    IL_0154:  ldloc.2
    IL_0155:  ldc.i4.0
    IL_0156:  ldnull
    IL_0157:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_015c:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0161:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0166:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_016b:  stelem.ref
    IL_016c:  ldloc.2
    IL_016d:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0172:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0177:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_017c:  ldc.i4.0
    IL_017d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0182:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0187:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_018c:  pop
    IL_018d:  ret
  } // end of method ExpressionTrees::DoAssert

  .method private hidebysig instance int32 
          C() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method ExpressionTrees::C

  .method private hidebysig instance bool 
          MyEquals(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees other) cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method ExpressionTrees::MyEquals

  .method public hidebysig instance void 
          MethodGroupAsExtensionMethod() cil managed
  {
    // Code size       287 (0x11f)
    .maxstack  10
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method bool [System.Core]System.Linq.Enumerable::Any<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0014:  box        [mscorlib]System.Reflection.MethodInfo
    IL_0019:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_001e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0023:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0028:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_002d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0032:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0037:  ldc.i4.2
    IL_0038:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_003d:  stloc.0
    IL_003e:  ldloc.0
    IL_003f:  ldc.i4.0
    IL_0040:  ldtoken    class [mscorlib]System.Func`1<bool>
    IL_0045:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004a:  box        [mscorlib]System.Type
    IL_004f:  ldtoken    [mscorlib]System.Type
    IL_0054:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0059:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005e:  stelem.ref
    IL_005f:  ldloc.0
    IL_0060:  ldc.i4.1
    IL_0061:  ldtoken    [mscorlib]System.Int32
    IL_0066:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006b:  ldc.i4.4
    IL_006c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0071:  stloc.1
    IL_0072:  ldloc.1
    IL_0073:  ldc.i4.0
    IL_0074:  ldc.i4     0x7d0
    IL_0079:  box        [mscorlib]System.Int32
    IL_007e:  ldtoken    [mscorlib]System.Int32
    IL_0083:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0088:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008d:  stelem.ref
    IL_008e:  ldloc.1
    IL_008f:  ldc.i4.1
    IL_0090:  ldc.i4     0x7d4
    IL_0095:  box        [mscorlib]System.Int32
    IL_009a:  ldtoken    [mscorlib]System.Int32
    IL_009f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a9:  stelem.ref
    IL_00aa:  ldloc.1
    IL_00ab:  ldc.i4.2
    IL_00ac:  ldc.i4     0x7d8
    IL_00b1:  box        [mscorlib]System.Int32
    IL_00b6:  ldtoken    [mscorlib]System.Int32
    IL_00bb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c5:  stelem.ref
    IL_00c6:  ldloc.1
    IL_00c7:  ldc.i4.3
    IL_00c8:  ldc.i4     0x7dc
    IL_00cd:  box        [mscorlib]System.Int32
    IL_00d2:  ldtoken    [mscorlib]System.Int32
    IL_00d7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e1:  stelem.ref
    IL_00e2:  ldloc.1
    IL_00e3:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00e8:  ldtoken    class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_00ed:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f2:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00f7:  stelem.ref
    IL_00f8:  ldloc.0
    IL_00f9:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00fe:  ldtoken    class [mscorlib]System.Func`1<bool>
    IL_0103:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0108:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_010d:  ldc.i4.0
    IL_010e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0113:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Func`1<bool>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0118:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Func`1<bool>>(object,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_011d:  pop
    IL_011e:  ret
  } // end of method ExpressionTrees::MethodGroupAsExtensionMethod

  .method public hidebysig instance void 
          MethodGroupConstant() cil managed
  {
    // Code size       917 (0x395)
    .maxstack  11
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10' V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3,
             class [System.Core]System.Linq.Expressions.Expression[] V_4,
             class [System.Core]System.Linq.Expressions.Expression[] V_5,
             class [System.Core]System.Linq.Expressions.Expression[] V_6,
             class [System.Core]System.Linq.Expressions.Expression[] V_7,
             class [System.Core]System.Linq.Expressions.Expression[] V_8)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_000b:  ldnull
    IL_000c:  ldtoken    method bool [mscorlib]System.Array::TrueForAll<int32>(!!0[],
                                                                               class [mscorlib]System.Predicate`1<!!0>)
    IL_0011:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0016:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_001b:  ldc.i4.2
    IL_001c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0021:  stloc.1
    IL_0022:  ldloc.1
    IL_0023:  ldc.i4.0
    IL_0024:  ldtoken    [mscorlib]System.Int32
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  ldc.i4.4
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  stloc.2
    IL_0035:  ldloc.2
    IL_0036:  ldc.i4.0
    IL_0037:  ldc.i4     0x7d0
    IL_003c:  box        [mscorlib]System.Int32
    IL_0041:  ldtoken    [mscorlib]System.Int32
    IL_0046:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0050:  stelem.ref
    IL_0051:  ldloc.2
    IL_0052:  ldc.i4.1
    IL_0053:  ldc.i4     0x7d4
    IL_0058:  box        [mscorlib]System.Int32
    IL_005d:  ldtoken    [mscorlib]System.Int32
    IL_0062:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0067:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006c:  stelem.ref
    IL_006d:  ldloc.2
    IL_006e:  ldc.i4.2
    IL_006f:  ldc.i4     0x7d8
    IL_0074:  box        [mscorlib]System.Int32
    IL_0079:  ldtoken    [mscorlib]System.Int32
    IL_007e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0083:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0088:  stelem.ref
    IL_0089:  ldloc.2
    IL_008a:  ldc.i4.3
    IL_008b:  ldc.i4     0x7dc
    IL_0090:  box        [mscorlib]System.Int32
    IL_0095:  ldtoken    [mscorlib]System.Int32
    IL_009a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a4:  stelem.ref
    IL_00a5:  ldloc.2
    IL_00a6:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ab:  stelem.ref
    IL_00ac:  ldloc.1
    IL_00ad:  ldc.i4.1
    IL_00ae:  ldtoken    method bool [mscorlib]System.DateTime::IsLeapYear(int32)
    IL_00b3:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00b8:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00bd:  box        [mscorlib]System.Reflection.MethodInfo
    IL_00c2:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_00c7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d1:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_00d6:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00db:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e0:  ldc.i4.2
    IL_00e1:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00e6:  stloc.3
    IL_00e7:  ldloc.3
    IL_00e8:  ldc.i4.0
    IL_00e9:  ldtoken    class [mscorlib]System.Predicate`1<int32>
    IL_00ee:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f3:  box        [mscorlib]System.Type
    IL_00f8:  ldtoken    [mscorlib]System.Type
    IL_00fd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0102:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0107:  stelem.ref
    IL_0108:  ldloc.3
    IL_0109:  ldc.i4.1
    IL_010a:  ldnull
    IL_010b:  ldtoken    [mscorlib]System.Object
    IL_0110:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0115:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_011a:  stelem.ref
    IL_011b:  ldloc.3
    IL_011c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0121:  ldtoken    class [mscorlib]System.Predicate`1<int32>
    IL_0126:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_012b:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0130:  stelem.ref
    IL_0131:  ldloc.1
    IL_0132:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0137:  ldc.i4.0
    IL_0138:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_013d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0142:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0147:  pop
    IL_0148:  ldloc.0
    IL_0149:  newobj     instance void class [System.Core]System.Collections.Generic.HashSet`1<int32>::.ctor()
    IL_014e:  stfld      class [System.Core]System.Collections.Generic.HashSet`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10'::set
    IL_0153:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0158:  ldnull
    IL_0159:  ldtoken    method bool [System.Core]System.Linq.Enumerable::All<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                     class [mscorlib]System.Func`2<!!0,bool>)
    IL_015e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0163:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0168:  ldc.i4.2
    IL_0169:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_016e:  stloc.s    V_4
    IL_0170:  ldloc.s    V_4
    IL_0172:  ldc.i4.0
    IL_0173:  ldtoken    [mscorlib]System.Int32
    IL_0178:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_017d:  ldc.i4.4
    IL_017e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0183:  stloc.s    V_5
    IL_0185:  ldloc.s    V_5
    IL_0187:  ldc.i4.0
    IL_0188:  ldc.i4     0x7d0
    IL_018d:  box        [mscorlib]System.Int32
    IL_0192:  ldtoken    [mscorlib]System.Int32
    IL_0197:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_019c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01a1:  stelem.ref
    IL_01a2:  ldloc.s    V_5
    IL_01a4:  ldc.i4.1
    IL_01a5:  ldc.i4     0x7d4
    IL_01aa:  box        [mscorlib]System.Int32
    IL_01af:  ldtoken    [mscorlib]System.Int32
    IL_01b4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b9:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01be:  stelem.ref
    IL_01bf:  ldloc.s    V_5
    IL_01c1:  ldc.i4.2
    IL_01c2:  ldc.i4     0x7d8
    IL_01c7:  box        [mscorlib]System.Int32
    IL_01cc:  ldtoken    [mscorlib]System.Int32
    IL_01d1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01d6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01db:  stelem.ref
    IL_01dc:  ldloc.s    V_5
    IL_01de:  ldc.i4.3
    IL_01df:  ldc.i4     0x7dc
    IL_01e4:  box        [mscorlib]System.Int32
    IL_01e9:  ldtoken    [mscorlib]System.Int32
    IL_01ee:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01f8:  stelem.ref
    IL_01f9:  ldloc.s    V_5
    IL_01fb:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0200:  stelem.ref
    IL_0201:  ldloc.s    V_4
    IL_0203:  ldc.i4.1
    IL_0204:  ldtoken    method instance bool class [System.Core]System.Collections.Generic.HashSet`1<int32>::Add(!0)
    IL_0209:  ldtoken    class [System.Core]System.Collections.Generic.HashSet`1<int32>
    IL_020e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0213:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0218:  box        [mscorlib]System.Reflection.MethodInfo
    IL_021d:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_0222:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0227:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_022c:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_0231:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0236:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_023b:  ldc.i4.2
    IL_023c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0241:  stloc.s    V_6
    IL_0243:  ldloc.s    V_6
    IL_0245:  ldc.i4.0
    IL_0246:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_024b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0250:  box        [mscorlib]System.Type
    IL_0255:  ldtoken    [mscorlib]System.Type
    IL_025a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_025f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0264:  stelem.ref
    IL_0265:  ldloc.s    V_6
    IL_0267:  ldc.i4.1
    IL_0268:  ldloc.0
    IL_0269:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_026e:  ldtoken    field class [System.Core]System.Collections.Generic.HashSet`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10'::set
    IL_0273:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0278:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_027d:  stelem.ref
    IL_027e:  ldloc.s    V_6
    IL_0280:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0285:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_028a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_028f:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0294:  stelem.ref
    IL_0295:  ldloc.s    V_4
    IL_0297:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_029c:  ldc.i4.0
    IL_029d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_02a2:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_02a7:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_02ac:  pop
    IL_02ad:  ldloc.0
    IL_02ae:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_02b3:  brtrue.s   IL_02c6

    IL_02b5:  ldnull
    IL_02b6:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<MethodGroupConstant>b__e'(class [mscorlib]System.Func`3<object,object,bool>)
    IL_02bc:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool>::.ctor(object,
                                                                                                                                    native int)
    IL_02c1:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_02c6:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_02cb:  stfld      class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10'::sink
    IL_02d0:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_02d5:  ldloc.0
    IL_02d6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_02db:  ldtoken    field class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10'::sink
    IL_02e0:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_02e5:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_02ea:  ldc.i4.1
    IL_02eb:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_02f0:  stloc.s    V_7
    IL_02f2:  ldloc.s    V_7
    IL_02f4:  ldc.i4.0
    IL_02f5:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_02fa:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_02ff:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0304:  box        [mscorlib]System.Reflection.MethodInfo
    IL_0309:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_030e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0313:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0318:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_031d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0322:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0327:  ldc.i4.2
    IL_0328:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_032d:  stloc.s    V_8
    IL_032f:  ldloc.s    V_8
    IL_0331:  ldc.i4.0
    IL_0332:  ldtoken    class [mscorlib]System.Func`3<object,object,bool>
    IL_0337:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_033c:  box        [mscorlib]System.Type
    IL_0341:  ldtoken    [mscorlib]System.Type
    IL_0346:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_034b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0350:  stelem.ref
    IL_0351:  ldloc.s    V_8
    IL_0353:  ldc.i4.1
    IL_0354:  ldnull
    IL_0355:  ldtoken    [mscorlib]System.Object
    IL_035a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_035f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0364:  stelem.ref
    IL_0365:  ldloc.s    V_8
    IL_0367:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_036c:  ldtoken    class [mscorlib]System.Func`3<object,object,bool>
    IL_0371:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0376:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_037b:  stelem.ref
    IL_037c:  ldloc.s    V_7
    IL_037e:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_0383:  ldc.i4.0
    IL_0384:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0389:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_038e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0393:  pop
    IL_0394:  ret
  } // end of method ExpressionTrees::MethodGroupConstant

  .method public hidebysig instance void 
          MultipleCasts() cil managed
  {
    // Code size       100 (0x64)
    .maxstack  4
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldc.i4.1
    IL_0006:  box        [mscorlib]System.Int32
    IL_000b:  ldtoken    [mscorlib]System.Int32
    IL_0010:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001a:  ldc.i4.1
    IL_001b:  box        [mscorlib]System.Int32
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002f:  ldtoken    [mscorlib]System.Object
    IL_0034:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0039:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_003e:  ldtoken    [mscorlib]System.Int32
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_004d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0052:  ldc.i4.0
    IL_0053:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0058:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0062:  pop
    IL_0063:  ret
  } // end of method ExpressionTrees::MultipleCasts

  .method public hidebysig instance void 
          MultipleDots() cil managed
  {
    // Code size       142 (0x8e)
    .maxstack  4
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldc.i4.3
    IL_0006:  box        [mscorlib]System.Int32
    IL_000b:  ldtoken    [mscorlib]System.Int32
    IL_0010:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001a:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_001f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0024:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0029:  ldc.i4.0
    IL_002a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002f:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0034:  ldtoken    method instance string [mscorlib]System.Object::ToString()
    IL_0039:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0043:  ldc.i4.0
    IL_0044:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0049:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004e:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0053:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0058:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0062:  ldc.i4.0
    IL_0063:  box        [mscorlib]System.Int32
    IL_0068:  ldtoken    [mscorlib]System.Int32
    IL_006d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0072:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0077:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_007c:  ldc.i4.0
    IL_007d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0082:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0087:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008c:  pop
    IL_008d:  ret
  } // end of method ExpressionTrees::MultipleDots

  .method public hidebysig instance void 
          NestedLambda() cil managed
  {
    // Code size       572 (0x23c)
    .maxstack  10
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass14' V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_5,
             class [System.Core]System.Linq.Expressions.Expression[] V_6,
             class [System.Core]System.Linq.Expressions.Expression[] V_7,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_8,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_9,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_10)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass14'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate13'
    IL_000c:  brtrue.s   IL_001f

    IL_000e:  ldnull
    IL_000f:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NestedLambda>b__12'(class [mscorlib]System.Func`1<int32>)
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32>::.ctor(object,
                                                                                                                        native int)
    IL_001a:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate13'
    IL_001f:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate13'
    IL_0024:  stfld      class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass14'::'call'
    IL_0029:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_002e:  ldloc.0
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0034:  ldtoken    field class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass14'::'call'
    IL_0039:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_003e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0043:  ldc.i4.1
    IL_0044:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0049:  stloc.1
    IL_004a:  ldloc.1
    IL_004b:  ldc.i4.0
    IL_004c:  ldc.i4.s   42
    IL_004e:  box        [mscorlib]System.Int32
    IL_0053:  ldtoken    [mscorlib]System.Int32
    IL_0058:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0062:  ldc.i4.0
    IL_0063:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0068:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006d:  stelem.ref
    IL_006e:  ldloc.1
    IL_006f:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_0074:  ldc.i4.0
    IL_0075:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0084:  pop
    IL_0085:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_008a:  ldnull
    IL_008b:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<int32,int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                       class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0090:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0095:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009a:  ldc.i4.2
    IL_009b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00a0:  stloc.2
    IL_00a1:  ldloc.2
    IL_00a2:  ldc.i4.0
    IL_00a3:  ldtoken    [mscorlib]System.Int32
    IL_00a8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ad:  ldc.i4.2
    IL_00ae:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00b3:  stloc.3
    IL_00b4:  ldloc.3
    IL_00b5:  ldc.i4.0
    IL_00b6:  ldc.i4.s   37
    IL_00b8:  box        [mscorlib]System.Int32
    IL_00bd:  ldtoken    [mscorlib]System.Int32
    IL_00c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cc:  stelem.ref
    IL_00cd:  ldloc.3
    IL_00ce:  ldc.i4.1
    IL_00cf:  ldc.i4.s   42
    IL_00d1:  box        [mscorlib]System.Int32
    IL_00d6:  ldtoken    [mscorlib]System.Int32
    IL_00db:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e5:  stelem.ref
    IL_00e6:  ldloc.3
    IL_00e7:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ec:  stelem.ref
    IL_00ed:  ldloc.2
    IL_00ee:  ldc.i4.1
    IL_00ef:  ldtoken    [mscorlib]System.Int32
    IL_00f4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f9:  ldstr      "x"
    IL_00fe:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0103:  stloc.s    V_4
    IL_0105:  ldloc.s    V_4
    IL_0107:  ldc.i4.2
    IL_0108:  box        [mscorlib]System.Int32
    IL_010d:  ldtoken    [mscorlib]System.Int32
    IL_0112:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0117:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_011c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0121:  ldc.i4.1
    IL_0122:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0127:  stloc.s    V_5
    IL_0129:  ldloc.s    V_5
    IL_012b:  ldc.i4.0
    IL_012c:  ldloc.s    V_4
    IL_012e:  stelem.ref
    IL_012f:  ldloc.s    V_5
    IL_0131:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0136:  stelem.ref
    IL_0137:  ldloc.2
    IL_0138:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_013d:  ldc.i4.0
    IL_013e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0143:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                              class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0148:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>(object,
                                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_014d:  pop
    IL_014e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0153:  ldnull
    IL_0154:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<int32,int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                       class [mscorlib]System.Func`3<!!0,int32,!!1>)
    IL_0159:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_015e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0163:  ldc.i4.2
    IL_0164:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0169:  stloc.s    V_6
    IL_016b:  ldloc.s    V_6
    IL_016d:  ldc.i4.0
    IL_016e:  ldtoken    [mscorlib]System.Int32
    IL_0173:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0178:  ldc.i4.2
    IL_0179:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_017e:  stloc.s    V_7
    IL_0180:  ldloc.s    V_7
    IL_0182:  ldc.i4.0
    IL_0183:  ldc.i4.s   37
    IL_0185:  box        [mscorlib]System.Int32
    IL_018a:  ldtoken    [mscorlib]System.Int32
    IL_018f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0194:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0199:  stelem.ref
    IL_019a:  ldloc.s    V_7
    IL_019c:  ldc.i4.1
    IL_019d:  ldc.i4.s   42
    IL_019f:  box        [mscorlib]System.Int32
    IL_01a4:  ldtoken    [mscorlib]System.Int32
    IL_01a9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ae:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01b3:  stelem.ref
    IL_01b4:  ldloc.s    V_7
    IL_01b6:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_01bb:  stelem.ref
    IL_01bc:  ldloc.s    V_6
    IL_01be:  ldc.i4.1
    IL_01bf:  ldtoken    [mscorlib]System.Int32
    IL_01c4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01c9:  ldstr      "x"
    IL_01ce:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01d3:  stloc.s    V_8
    IL_01d5:  ldtoken    [mscorlib]System.Int32
    IL_01da:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01df:  ldstr      "i"
    IL_01e4:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01e9:  stloc.s    V_9
    IL_01eb:  ldloc.s    V_8
    IL_01ed:  ldc.i4.2
    IL_01ee:  box        [mscorlib]System.Int32
    IL_01f3:  ldtoken    [mscorlib]System.Int32
    IL_01f8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01fd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0202:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0207:  ldc.i4.2
    IL_0208:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_020d:  stloc.s    V_10
    IL_020f:  ldloc.s    V_10
    IL_0211:  ldc.i4.0
    IL_0212:  ldloc.s    V_8
    IL_0214:  stelem.ref
    IL_0215:  ldloc.s    V_10
    IL_0217:  ldc.i4.1
    IL_0218:  ldloc.s    V_9
    IL_021a:  stelem.ref
    IL_021b:  ldloc.s    V_10
    IL_021d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0222:  stelem.ref
    IL_0223:  ldloc.s    V_6
    IL_0225:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_022a:  ldc.i4.0
    IL_022b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0230:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                              class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0235:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>(object,
                                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_023a:  pop
    IL_023b:  ret
  } // end of method ExpressionTrees::NestedLambda

  .method public hidebysig instance void 
          CurriedLambda() cil managed
  {
    // Code size       145 (0x91)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_5)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldstr      "a"
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0019:  stloc.0
    IL_001a:  ldtoken    [mscorlib]System.Int32
    IL_001f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0024:  ldstr      "b"
    IL_0029:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002e:  stloc.1
    IL_002f:  ldtoken    [mscorlib]System.Int32
    IL_0034:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0039:  ldstr      "c"
    IL_003e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0043:  stloc.2
    IL_0044:  ldloc.0
    IL_0045:  ldloc.1
    IL_0046:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_004b:  ldloc.2
    IL_004c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0051:  ldc.i4.1
    IL_0052:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0057:  stloc.3
    IL_0058:  ldloc.3
    IL_0059:  ldc.i4.0
    IL_005a:  ldloc.2
    IL_005b:  stelem.ref
    IL_005c:  ldloc.3
    IL_005d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0062:  ldc.i4.1
    IL_0063:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0068:  stloc.s    V_4
    IL_006a:  ldloc.s    V_4
    IL_006c:  ldc.i4.0
    IL_006d:  ldloc.1
    IL_006e:  stelem.ref
    IL_006f:  ldloc.s    V_4
    IL_0071:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0076:  ldc.i4.1
    IL_0077:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007c:  stloc.s    V_5
    IL_007e:  ldloc.s    V_5
    IL_0080:  ldc.i4.0
    IL_0081:  ldloc.0
    IL_0082:  stelem.ref
    IL_0083:  ldloc.s    V_5
    IL_0085:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>(object,
                                                                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_008f:  pop
    IL_0090:  ret
  } // end of method ExpressionTrees::CurriedLambda

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<int32,bool> a) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.s   42
    IL_0003:  callvirt   instance !1 class [mscorlib]System.Func`2<int32,bool>::Invoke(!0)
    IL_0008:  ret
  } // end of method ExpressionTrees::Fizz

  .method private hidebysig instance bool 
          Buzz(class [mscorlib]System.Func`2<int32,bool> a) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.s   42
    IL_0003:  callvirt   instance !1 class [mscorlib]System.Func`2<int32,bool>::Invoke(!0)
    IL_0008:  ret
  } // end of method ExpressionTrees::Buzz

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<string,bool> a) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldstr      "42"
    IL_0006:  callvirt   instance !1 class [mscorlib]System.Func`2<string,bool>::Invoke(!0)
    IL_000b:  ret
  } // end of method ExpressionTrees::Fizz

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool> a) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>::Invoke(!0)
    IL_0007:  ret
  } // end of method ExpressionTrees::Fizz

  .method public hidebysig instance void 
          NestedLambda2() cil managed
  {
    // Code size       1310 (0x51e)
    .maxstack  12
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_5,
             class [System.Core]System.Linq.Expressions.Expression[] V_6,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_7,
             class [System.Core]System.Linq.Expressions.Expression[] V_8,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_9,
             class [System.Core]System.Linq.Expressions.Expression[] V_10,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_11,
             class [System.Core]System.Linq.Expressions.Expression[] V_12,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_13,
             class [System.Core]System.Linq.Expressions.Expression[] V_14,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_15,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_16,
             class [System.Core]System.Linq.Expressions.Expression[] V_17,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_18,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_19,
             class [System.Core]System.Linq.Expressions.Expression[] V_20,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_21,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_22)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldarg.0
    IL_0006:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_000b:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0010:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001a:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<string,bool>)
    IL_001f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0024:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0029:  ldc.i4.1
    IL_002a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002f:  stloc.0
    IL_0030:  ldloc.0
    IL_0031:  ldc.i4.0
    IL_0032:  ldtoken    [mscorlib]System.String
    IL_0037:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  ldstr      "x"
    IL_0041:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0046:  stloc.1
    IL_0047:  ldloc.1
    IL_0048:  ldstr      "a"
    IL_004d:  ldtoken    [mscorlib]System.String
    IL_0052:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005c:  ldc.i4.0
    IL_005d:  ldtoken    method bool [mscorlib]System.String::op_Equality(string,
                                                                          string)
    IL_0062:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0067:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_0071:  ldc.i4.1
    IL_0072:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0077:  stloc.2
    IL_0078:  ldloc.2
    IL_0079:  ldc.i4.0
    IL_007a:  ldloc.1
    IL_007b:  stelem.ref
    IL_007c:  ldloc.2
    IL_007d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0082:  stelem.ref
    IL_0083:  ldloc.0
    IL_0084:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0089:  ldc.i4.0
    IL_008a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_008f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0094:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0099:  pop
    IL_009a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_009f:  ldarg.0
    IL_00a0:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_00a5:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_00aa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00af:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b4:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<string,bool>)
    IL_00b9:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00be:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00c3:  ldc.i4.1
    IL_00c4:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00c9:  stloc.3
    IL_00ca:  ldloc.3
    IL_00cb:  ldc.i4.0
    IL_00cc:  ldtoken    [mscorlib]System.String
    IL_00d1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d6:  ldstr      "x"
    IL_00db:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00e0:  stloc.s    V_4
    IL_00e2:  ldloc.s    V_4
    IL_00e4:  ldstr      "a"
    IL_00e9:  ldtoken    [mscorlib]System.String
    IL_00ee:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00f8:  ldc.i4.0
    IL_00f9:  ldtoken    method bool [mscorlib]System.String::op_Inequality(string,
                                                                            string)
    IL_00fe:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0103:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0108:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               bool,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_010d:  ldc.i4.1
    IL_010e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0113:  stloc.s    V_5
    IL_0115:  ldloc.s    V_5
    IL_0117:  ldc.i4.0
    IL_0118:  ldloc.s    V_4
    IL_011a:  stelem.ref
    IL_011b:  ldloc.s    V_5
    IL_011d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0122:  stelem.ref
    IL_0123:  ldloc.3
    IL_0124:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0129:  ldc.i4.0
    IL_012a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_012f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0134:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0139:  pop
    IL_013a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_013f:  ldarg.0
    IL_0140:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0145:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_014a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_014f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0154:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>)
    IL_0159:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_015e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0163:  ldc.i4.1
    IL_0164:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0169:  stloc.s    V_6
    IL_016b:  ldloc.s    V_6
    IL_016d:  ldc.i4.0
    IL_016e:  ldtoken    [mscorlib]System.Action
    IL_0173:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0178:  ldstr      "x"
    IL_017d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0182:  stloc.s    V_7
    IL_0184:  ldloc.s    V_7
    IL_0186:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::NestedLambda2()
    IL_018b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0190:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0195:  box        [mscorlib]System.Reflection.MethodInfo
    IL_019a:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_019f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01a9:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_01ae:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01b3:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01b8:  ldc.i4.2
    IL_01b9:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01be:  stloc.s    V_8
    IL_01c0:  ldloc.s    V_8
    IL_01c2:  ldc.i4.0
    IL_01c3:  ldtoken    [mscorlib]System.Action
    IL_01c8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01cd:  box        [mscorlib]System.Type
    IL_01d2:  ldtoken    [mscorlib]System.Type
    IL_01d7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01dc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01e1:  stelem.ref
    IL_01e2:  ldloc.s    V_8
    IL_01e4:  ldc.i4.1
    IL_01e5:  ldarg.0
    IL_01e6:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_01eb:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_01f0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01fa:  stelem.ref
    IL_01fb:  ldloc.s    V_8
    IL_01fd:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0202:  ldtoken    [mscorlib]System.Action
    IL_0207:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_020c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0211:  ldc.i4.0
    IL_0212:  ldtoken    method bool [mscorlib]System.Delegate::op_Equality(class [mscorlib]System.Delegate,
                                                                            class [mscorlib]System.Delegate)
    IL_0217:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_021c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0221:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_0226:  ldc.i4.1
    IL_0227:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_022c:  stloc.s    V_9
    IL_022e:  ldloc.s    V_9
    IL_0230:  ldc.i4.0
    IL_0231:  ldloc.s    V_7
    IL_0233:  stelem.ref
    IL_0234:  ldloc.s    V_9
    IL_0236:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_023b:  stelem.ref
    IL_023c:  ldloc.s    V_6
    IL_023e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0243:  ldc.i4.0
    IL_0244:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0249:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_024e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0253:  pop
    IL_0254:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0259:  ldarg.0
    IL_025a:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_025f:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0264:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0269:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_026e:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>)
    IL_0273:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0278:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_027d:  ldc.i4.1
    IL_027e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0283:  stloc.s    V_10
    IL_0285:  ldloc.s    V_10
    IL_0287:  ldc.i4.0
    IL_0288:  ldtoken    [mscorlib]System.Action
    IL_028d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0292:  ldstr      "x"
    IL_0297:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_029c:  stloc.s    V_11
    IL_029e:  ldloc.s    V_11
    IL_02a0:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::NestedLambda2()
    IL_02a5:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_02aa:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_02af:  box        [mscorlib]System.Reflection.MethodInfo
    IL_02b4:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_02b9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02be:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02c3:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_02c8:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_02cd:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_02d2:  ldc.i4.2
    IL_02d3:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_02d8:  stloc.s    V_12
    IL_02da:  ldloc.s    V_12
    IL_02dc:  ldc.i4.0
    IL_02dd:  ldtoken    [mscorlib]System.Action
    IL_02e2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02e7:  box        [mscorlib]System.Type
    IL_02ec:  ldtoken    [mscorlib]System.Type
    IL_02f1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02f6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02fb:  stelem.ref
    IL_02fc:  ldloc.s    V_12
    IL_02fe:  ldc.i4.1
    IL_02ff:  ldarg.0
    IL_0300:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0305:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_030a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_030f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0314:  stelem.ref
    IL_0315:  ldloc.s    V_12
    IL_0317:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_031c:  ldtoken    [mscorlib]System.Action
    IL_0321:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0326:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_032b:  ldc.i4.0
    IL_032c:  ldtoken    method bool [mscorlib]System.Delegate::op_Inequality(class [mscorlib]System.Delegate,
                                                                              class [mscorlib]System.Delegate)
    IL_0331:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0336:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_033b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               bool,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0340:  ldc.i4.1
    IL_0341:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0346:  stloc.s    V_13
    IL_0348:  ldloc.s    V_13
    IL_034a:  ldc.i4.0
    IL_034b:  ldloc.s    V_11
    IL_034d:  stelem.ref
    IL_034e:  ldloc.s    V_13
    IL_0350:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0355:  stelem.ref
    IL_0356:  ldloc.s    V_10
    IL_0358:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_035d:  ldc.i4.0
    IL_035e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0363:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0368:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_036d:  pop
    IL_036e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0373:  ldarg.0
    IL_0374:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0379:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_037e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0383:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0388:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<int32,bool>)
    IL_038d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0392:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0397:  ldc.i4.1
    IL_0398:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_039d:  stloc.s    V_14
    IL_039f:  ldloc.s    V_14
    IL_03a1:  ldc.i4.0
    IL_03a2:  ldtoken    [mscorlib]System.Int32
    IL_03a7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03ac:  ldstr      "x"
    IL_03b1:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03b6:  stloc.s    V_15
    IL_03b8:  ldloc.s    V_15
    IL_03ba:  ldc.i4.s   37
    IL_03bc:  box        [mscorlib]System.Int32
    IL_03c1:  ldtoken    [mscorlib]System.Int32
    IL_03c6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03cb:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_03d0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_03d5:  ldc.i4.1
    IL_03d6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03db:  stloc.s    V_16
    IL_03dd:  ldloc.s    V_16
    IL_03df:  ldc.i4.0
    IL_03e0:  ldloc.s    V_15
    IL_03e2:  stelem.ref
    IL_03e3:  ldloc.s    V_16
    IL_03e5:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03ea:  stelem.ref
    IL_03eb:  ldloc.s    V_14
    IL_03ed:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_03f2:  ldc.i4.0
    IL_03f3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03f8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03fd:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0402:  pop
    IL_0403:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0408:  ldarg.0
    IL_0409:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_040e:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0413:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0418:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_041d:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<int32,bool>)
    IL_0422:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0427:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_042c:  ldc.i4.1
    IL_042d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0432:  stloc.s    V_17
    IL_0434:  ldloc.s    V_17
    IL_0436:  ldc.i4.0
    IL_0437:  ldtoken    [mscorlib]System.Int32
    IL_043c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0441:  ldstr      "x"
    IL_0446:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_044b:  stloc.s    V_18
    IL_044d:  ldc.i4.1
    IL_044e:  box        [mscorlib]System.Boolean
    IL_0453:  ldtoken    [mscorlib]System.Boolean
    IL_0458:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_045d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0462:  ldc.i4.1
    IL_0463:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0468:  stloc.s    V_19
    IL_046a:  ldloc.s    V_19
    IL_046c:  ldc.i4.0
    IL_046d:  ldloc.s    V_18
    IL_046f:  stelem.ref
    IL_0470:  ldloc.s    V_19
    IL_0472:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0477:  stelem.ref
    IL_0478:  ldloc.s    V_17
    IL_047a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_047f:  ldc.i4.0
    IL_0480:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0485:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_048a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_048f:  pop
    IL_0490:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0495:  ldarg.0
    IL_0496:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_049b:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_04a0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04a5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_04aa:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Buzz(class [mscorlib]System.Func`2<int32,bool>)
    IL_04af:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_04b4:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_04b9:  ldc.i4.1
    IL_04ba:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_04bf:  stloc.s    V_20
    IL_04c1:  ldloc.s    V_20
    IL_04c3:  ldc.i4.0
    IL_04c4:  ldtoken    [mscorlib]System.Int32
    IL_04c9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04ce:  ldstr      "x"
    IL_04d3:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04d8:  stloc.s    V_21
    IL_04da:  ldc.i4.1
    IL_04db:  box        [mscorlib]System.Boolean
    IL_04e0:  ldtoken    [mscorlib]System.Boolean
    IL_04e5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04ea:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_04ef:  ldc.i4.1
    IL_04f0:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_04f5:  stloc.s    V_22
    IL_04f7:  ldloc.s    V_22
    IL_04f9:  ldc.i4.0
    IL_04fa:  ldloc.s    V_21
    IL_04fc:  stelem.ref
    IL_04fd:  ldloc.s    V_22
    IL_04ff:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0504:  stelem.ref
    IL_0505:  ldloc.s    V_20
    IL_0507:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_050c:  ldc.i4.0
    IL_050d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0512:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0517:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_051c:  pop
    IL_051d:  ret
  } // end of method ExpressionTrees::NestedLambda2

  .method public hidebysig instance void 
          NewArrayAndExtensionMethod() cil managed
  {
    // Code size       296 (0x128)
    .maxstack  10
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool [System.Core]System.Linq.Enumerable::SequenceEqual<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldtoken    [mscorlib]System.Double
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  ldc.i4.3
    IL_0029:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002e:  stloc.1
    IL_002f:  ldloc.1
    IL_0030:  ldc.i4.0
    IL_0031:  ldc.r8     1.
    IL_003a:  box        [mscorlib]System.Double
    IL_003f:  ldtoken    [mscorlib]System.Double
    IL_0044:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0049:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004e:  stelem.ref
    IL_004f:  ldloc.1
    IL_0050:  ldc.i4.1
    IL_0051:  ldc.r8     2.0099999999999998
    IL_005a:  box        [mscorlib]System.Double
    IL_005f:  ldtoken    [mscorlib]System.Double
    IL_0064:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0069:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006e:  stelem.ref
    IL_006f:  ldloc.1
    IL_0070:  ldc.i4.2
    IL_0071:  ldc.r8     3.5
    IL_007a:  box        [mscorlib]System.Double
    IL_007f:  ldtoken    [mscorlib]System.Double
    IL_0084:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0089:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008e:  stelem.ref
    IL_008f:  ldloc.1
    IL_0090:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0095:  stelem.ref
    IL_0096:  ldloc.0
    IL_0097:  ldc.i4.1
    IL_0098:  ldtoken    [mscorlib]System.Double
    IL_009d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a2:  ldc.i4.3
    IL_00a3:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00a8:  stloc.2
    IL_00a9:  ldloc.2
    IL_00aa:  ldc.i4.0
    IL_00ab:  ldc.r8     1.
    IL_00b4:  box        [mscorlib]System.Double
    IL_00b9:  ldtoken    [mscorlib]System.Double
    IL_00be:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c8:  stelem.ref
    IL_00c9:  ldloc.2
    IL_00ca:  ldc.i4.1
    IL_00cb:  ldc.r8     2.0099999999999998
    IL_00d4:  box        [mscorlib]System.Double
    IL_00d9:  ldtoken    [mscorlib]System.Double
    IL_00de:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e8:  stelem.ref
    IL_00e9:  ldloc.2
    IL_00ea:  ldc.i4.2
    IL_00eb:  ldc.r8     3.5
    IL_00f4:  box        [mscorlib]System.Double
    IL_00f9:  ldtoken    [mscorlib]System.Double
    IL_00fe:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0103:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0108:  stelem.ref
    IL_0109:  ldloc.2
    IL_010a:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_010f:  stelem.ref
    IL_0110:  ldloc.0
    IL_0111:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0116:  ldc.i4.0
    IL_0117:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_011c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0121:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0126:  pop
    IL_0127:  ret
  } // end of method ExpressionTrees::NewArrayAndExtensionMethod

  .method public hidebysig instance void 
          NewMultiDimArray() cil managed
  {
    // Code size       140 (0x8c)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldc.i4.2
    IL_0010:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.0
    IL_0018:  ldc.i4.3
    IL_0019:  box        [mscorlib]System.Int32
    IL_001e:  ldtoken    [mscorlib]System.Int32
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002d:  stelem.ref
    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.1
    IL_0030:  ldc.i4.4
    IL_0031:  box        [mscorlib]System.Int32
    IL_0036:  ldtoken    [mscorlib]System.Int32
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0045:  stelem.ref
    IL_0046:  ldloc.0
    IL_0047:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_004c:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_0051:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0056:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0060:  ldc.i4.1
    IL_0061:  box        [mscorlib]System.Int32
    IL_0066:  ldtoken    [mscorlib]System.Int32
    IL_006b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0070:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0075:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_007a:  ldc.i4.0
    IL_007b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0080:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0085:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008a:  pop
    IL_008b:  ret
  } // end of method ExpressionTrees::NewMultiDimArray

  .method public hidebysig instance void 
          NewObject() cil managed
  {
    // Code size       80 (0x50)
    .maxstack  4
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0014:  ldc.i4.0
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001a:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_001f:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_0024:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0029:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_002e:  ldc.i4.0
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0039:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_003e:  ldc.i4.0
    IL_003f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0044:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0049:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_004e:  pop
    IL_004f:  ret
  } // end of method ExpressionTrees::NewObject

  .method public hidebysig instance void 
          NotOperator() cil managed
  {
    // Code size       240 (0xf0)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16'::x
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.3
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16'::y
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.s   42
    IL_0017:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16'::z
    IL_001c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0021:  ldloc.0
    IL_0022:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0027:  ldtoken    field uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16'::z
    IL_002c:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0031:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0036:  ldtoken    [mscorlib]System.Int32
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0045:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_004a:  ldc.i4.0
    IL_004b:  box        [mscorlib]System.Int32
    IL_0050:  ldtoken    [mscorlib]System.Int32
    IL_0055:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0064:  ldc.i4.0
    IL_0065:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_006a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0074:  pop
    IL_0075:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_007a:  ldloc.0
    IL_007b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0080:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16'::y
    IL_0085:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_008a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_008f:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0094:  ldc.i4.0
    IL_0095:  box        [mscorlib]System.Int32
    IL_009a:  ldtoken    [mscorlib]System.Int32
    IL_009f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a9:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00ae:  ldc.i4.0
    IL_00af:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b9:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00be:  pop
    IL_00bf:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00c4:  ldloc.0
    IL_00c5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00ca:  ldtoken    field bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16'::x
    IL_00cf:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00d4:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00d9:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00de:  ldc.i4.0
    IL_00df:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e9:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ee:  pop
    IL_00ef:  ret
  } // end of method ExpressionTrees::NotOperator

  .method public hidebysig instance void 
          ObjectInitializers() cil managed
  {
    // Code size       275 (0x113)
    .maxstack  7
    .locals init (class [System.Xml]System.Xml.XmlReaderSettings V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass19' V_1,
             class [System.Core]System.Linq.Expressions.MemberBinding[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass19'::.ctor()
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  newobj     instance void [System.Xml]System.Xml.XmlReaderSettings::.ctor()
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.0
    IL_000f:  callvirt   instance void [System.Xml]System.Xml.XmlReaderSettings::set_CloseInput(bool)
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.0
    IL_0016:  callvirt   instance void [System.Xml]System.Xml.XmlReaderSettings::set_CheckCharacters(bool)
    IL_001b:  ldloc.0
    IL_001c:  stfld      class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass19'::s
    IL_0021:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0026:  ldtoken    method instance void [System.Xml]System.Xml.XmlReaderSettings::.ctor()
    IL_002b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0030:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0035:  ldc.i4.0
    IL_0036:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_003b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0040:  ldc.i4.2
    IL_0041:  newarr     [System.Core]System.Linq.Expressions.MemberBinding
    IL_0046:  stloc.2
    IL_0047:  ldloc.2
    IL_0048:  ldc.i4.0
    IL_0049:  ldtoken    method instance void [System.Xml]System.Xml.XmlReaderSettings::set_CloseInput(bool)
    IL_004e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0053:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0058:  ldloc.1
    IL_0059:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_005e:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass19'::s
    IL_0063:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0068:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_006d:  ldtoken    method instance bool [System.Xml]System.Xml.XmlReaderSettings::get_CloseInput()
    IL_0072:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0077:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0081:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_0086:  stelem.ref
    IL_0087:  ldloc.2
    IL_0088:  ldc.i4.1
    IL_0089:  ldtoken    method instance void [System.Xml]System.Xml.XmlReaderSettings::set_CheckCharacters(bool)
    IL_008e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0093:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0098:  ldloc.1
    IL_0099:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_009e:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass19'::s
    IL_00a3:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00a8:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00ad:  ldtoken    method instance bool [System.Xml]System.Xml.XmlReaderSettings::get_CheckCharacters()
    IL_00b2:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00b7:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00bc:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00c1:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_00c6:  stelem.ref
    IL_00c7:  ldloc.2
    IL_00c8:  call       class [System.Core]System.Linq.Expressions.MemberInitExpression [System.Core]System.Linq.Expressions.Expression::MemberInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.MemberBinding[])
    IL_00cd:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_00d2:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00d7:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00dc:  ldc.i4.1
    IL_00dd:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00e2:  stloc.3
    IL_00e3:  ldloc.3
    IL_00e4:  ldc.i4.0
    IL_00e5:  ldloc.1
    IL_00e6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00eb:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass19'::s
    IL_00f0:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f5:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00fa:  stelem.ref
    IL_00fb:  ldloc.3
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0101:  ldc.i4.0
    IL_0102:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0107:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_010c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0111:  pop
    IL_0112:  ret
  } // end of method ExpressionTrees::ObjectInitializers

  .method public hidebysig instance void 
          Quoted() cil managed
  {
    // Code size       180 (0xb4)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldstr      "n"
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0019:  stloc.0
    IL_001a:  ldtoken    [mscorlib]System.String
    IL_001f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0024:  ldstr      "s"
    IL_0029:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002e:  stloc.1
    IL_002f:  ldloc.1
    IL_0030:  ldloc.0
    IL_0031:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_0036:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0040:  ldc.i4.0
    IL_0041:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0046:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004b:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0050:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0055:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_005f:  ldc.i4.2
    IL_0060:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0065:  stloc.2
    IL_0066:  ldloc.2
    IL_0067:  ldc.i4.0
    IL_0068:  ldloc.0
    IL_0069:  stelem.ref
    IL_006a:  ldloc.2
    IL_006b:  ldc.i4.1
    IL_006c:  ldloc.1
    IL_006d:  stelem.ref
    IL_006e:  ldloc.2
    IL_006f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0074:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_0079:  ldtoken    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<int32,string,string>>
    IL_007e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0083:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0088:  ldnull
    IL_0089:  box        [mscorlib]System.Object
    IL_008e:  ldtoken    [mscorlib]System.Object
    IL_0093:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0098:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00a2:  ldc.i4.0
    IL_00a3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ad:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b2:  pop
    IL_00b3:  ret
  } // end of method ExpressionTrees::Quoted

  .method public hidebysig instance void 
          Quoted2() cil managed
  {
    // Code size       174 (0xae)
    .maxstack  8
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                                   class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldnull
    IL_001f:  ldtoken    method object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0024:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0029:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002e:  ldc.i4.0
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0039:  stelem.ref
    IL_003a:  ldloc.0
    IL_003b:  ldc.i4.1
    IL_003c:  ldc.i4.1
    IL_003d:  box        [mscorlib]System.Boolean
    IL_0042:  ldtoken    [mscorlib]System.Boolean
    IL_0047:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0051:  ldc.i4.0
    IL_0052:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0057:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_0061:  stelem.ref
    IL_0062:  ldloc.0
    IL_0063:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0068:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_006d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0072:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0077:  ldc.i4.1
    IL_0078:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_007d:  stloc.1
    IL_007e:  ldloc.1
    IL_007f:  ldc.i4.0
    IL_0080:  ldnull
    IL_0081:  box        [mscorlib]System.Object
    IL_0086:  ldtoken    [mscorlib]System.Object
    IL_008b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0090:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0095:  stelem.ref
    IL_0096:  ldloc.1
    IL_0097:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_009c:  ldc.i4.0
    IL_009d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a2:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a7:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ac:  pop
    IL_00ad:  ret
  } // end of method ExpressionTrees::Quoted2

  .method public hidebysig instance void 
          QuotedWithAnonymous() cil managed
  {
    // Code size       370 (0x172)
    .maxstack  18
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3,
             class [mscorlib]System.Reflection.MethodInfo[] V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_5,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_6)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method !!0 [System.Core]System.Linq.Enumerable::Single<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.1
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldnull
    IL_001f:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType1`2'<string,string>,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                               class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0024:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0029:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002e:  ldc.i4.2
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  stloc.1
    IL_0035:  ldloc.1
    IL_0036:  ldc.i4.0
    IL_0037:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  ldc.i4.1
    IL_0042:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0047:  stloc.2
    IL_0048:  ldloc.2
    IL_0049:  ldc.i4.0
    IL_004a:  ldtoken    method instance void class '<>f__AnonymousType1`2'<string,string>::.ctor(!0,
                                                                                                  !1)
    IL_004f:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_0054:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0059:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_005e:  ldc.i4.2
    IL_005f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0064:  stloc.3
    IL_0065:  ldloc.3
    IL_0066:  ldc.i4.0
    IL_0067:  ldstr      "a"
    IL_006c:  ldtoken    [mscorlib]System.String
    IL_0071:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0076:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007b:  stelem.ref
    IL_007c:  ldloc.3
    IL_007d:  ldc.i4.1
    IL_007e:  ldstr      "b"
    IL_0083:  ldtoken    [mscorlib]System.String
    IL_0088:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0092:  stelem.ref
    IL_0093:  ldloc.3
    IL_0094:  ldc.i4.2
    IL_0095:  newarr     [mscorlib]System.Reflection.MethodInfo
    IL_009a:  stloc.s    V_4
    IL_009c:  ldloc.s    V_4
    IL_009e:  ldc.i4.0
    IL_009f:  ldtoken    method instance !0 class '<>f__AnonymousType1`2'<string,string>::get_X()
    IL_00a4:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00a9:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ae:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00b3:  stelem.ref
    IL_00b4:  ldloc.s    V_4
    IL_00b6:  ldc.i4.1
    IL_00b7:  ldtoken    method instance !1 class '<>f__AnonymousType1`2'<string,string>::get_Y()
    IL_00bc:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00c1:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c6:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00cb:  stelem.ref
    IL_00cc:  ldloc.s    V_4
    IL_00ce:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_00d3:  stelem.ref
    IL_00d4:  ldloc.2
    IL_00d5:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00da:  stelem.ref
    IL_00db:  ldloc.1
    IL_00dc:  ldc.i4.1
    IL_00dd:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00e2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e7:  ldstr      "o"
    IL_00ec:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00f1:  stloc.s    V_5
    IL_00f3:  ldloc.s    V_5
    IL_00f5:  ldtoken    method instance !0 class '<>f__AnonymousType1`2'<string,string>::get_X()
    IL_00fa:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00ff:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0104:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0109:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_010e:  ldloc.s    V_5
    IL_0110:  ldtoken    method instance !1 class '<>f__AnonymousType1`2'<string,string>::get_Y()
    IL_0115:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_011a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0124:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0129:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_012e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0133:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0138:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_013d:  ldc.i4.1
    IL_013e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0143:  stloc.s    V_6
    IL_0145:  ldloc.s    V_6
    IL_0147:  ldc.i4.0
    IL_0148:  ldloc.s    V_5
    IL_014a:  stelem.ref
    IL_014b:  ldloc.s    V_6
    IL_014d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<string,string>,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0152:  stelem.ref
    IL_0153:  ldloc.1
    IL_0154:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0159:  stelem.ref
    IL_015a:  ldloc.0
    IL_015b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0160:  ldc.i4.0
    IL_0161:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0166:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_016b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0170:  pop
    IL_0171:  ret
  } // end of method ExpressionTrees::QuotedWithAnonymous

  .method public hidebysig instance void 
          StaticCall() cil managed
  {
    // Code size       130 (0x82)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldc.i4.3
    IL_001f:  box        [mscorlib]System.Int32
    IL_0024:  ldtoken    [mscorlib]System.Int32
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0033:  ldtoken    [mscorlib]System.Object
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0042:  stelem.ref
    IL_0043:  ldloc.0
    IL_0044:  ldc.i4.1
    IL_0045:  ldc.i4.0
    IL_0046:  box        [mscorlib]System.Int32
    IL_004b:  ldtoken    [mscorlib]System.Int32
    IL_0050:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0055:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005a:  ldtoken    [mscorlib]System.Object
    IL_005f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0064:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0069:  stelem.ref
    IL_006a:  ldloc.0
    IL_006b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0070:  ldc.i4.0
    IL_0071:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0076:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0080:  pop
    IL_0081:  ret
  } // end of method ExpressionTrees::StaticCall

  .method public hidebysig instance void 
          ThisCall() cil managed
  {
    // Code size       116 (0x74)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldarg.0
    IL_0006:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_000b:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0010:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001a:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_001f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0024:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0029:  ldc.i4.1
    IL_002a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002f:  stloc.0
    IL_0030:  ldloc.0
    IL_0031:  ldc.i4.0
    IL_0032:  ldc.i4.3
    IL_0033:  box        [mscorlib]System.Int32
    IL_0038:  ldtoken    [mscorlib]System.Int32
    IL_003d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0047:  ldtoken    [mscorlib]System.Object
    IL_004c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0051:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0056:  stelem.ref
    IL_0057:  ldloc.0
    IL_0058:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_005d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0062:  ldc.i4.0
    IL_0063:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0068:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0072:  pop
    IL_0073:  ret
  } // end of method ExpressionTrees::ThisCall

  .method public hidebysig instance void 
          ThisExplicit() cil managed
  {
    // Code size       115 (0x73)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldarg.0
    IL_001f:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0024:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0033:  stelem.ref
    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.1
    IL_0036:  ldc.i4.3
    IL_0037:  box        [mscorlib]System.Int32
    IL_003c:  ldtoken    [mscorlib]System.Int32
    IL_0041:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004b:  ldtoken    [mscorlib]System.Object
    IL_0050:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0055:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_005a:  stelem.ref
    IL_005b:  ldloc.0
    IL_005c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0061:  ldc.i4.0
    IL_0062:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0067:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0071:  pop
    IL_0072:  ret
  } // end of method ExpressionTrees::ThisExplicit

  .method public hidebysig instance void 
          TypedConstant() cil managed
  {
    // Code size       112 (0x70)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Type
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldc.i4.2
    IL_0010:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.0
    IL_0018:  ldtoken    [mscorlib]System.Int32
    IL_001d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0022:  box        [mscorlib]System.Type
    IL_0027:  ldtoken    [mscorlib]System.Type
    IL_002c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0031:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0036:  stelem.ref
    IL_0037:  ldloc.0
    IL_0038:  ldc.i4.1
    IL_0039:  ldtoken    [mscorlib]System.String
    IL_003e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0043:  box        [mscorlib]System.Type
    IL_0048:  ldtoken    [mscorlib]System.Type
    IL_004d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0052:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0057:  stelem.ref
    IL_0058:  ldloc.0
    IL_0059:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_005e:  ldc.i4.0
    IL_005f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0064:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0069:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Type[]>(object,
                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006e:  pop
    IL_006f:  ret
  } // end of method ExpressionTrees::TypedConstant

  .method public hidebysig instance void 
          StaticCallImplicitCast() cil managed
  {
    // Code size       130 (0x82)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldc.i4.3
    IL_001f:  box        [mscorlib]System.Int32
    IL_0024:  ldtoken    [mscorlib]System.Int32
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0033:  ldtoken    [mscorlib]System.Object
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0042:  stelem.ref
    IL_0043:  ldloc.0
    IL_0044:  ldc.i4.1
    IL_0045:  ldc.i4.0
    IL_0046:  box        [mscorlib]System.Int32
    IL_004b:  ldtoken    [mscorlib]System.Int32
    IL_0050:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0055:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005a:  ldtoken    [mscorlib]System.Object
    IL_005f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0064:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0069:  stelem.ref
    IL_006a:  ldloc.0
    IL_006b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0070:  ldc.i4.0
    IL_0071:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0076:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0080:  pop
    IL_0081:  ret
  } // end of method ExpressionTrees::StaticCallImplicitCast

  .method public hidebysig instance void 
          StaticMembers() cil managed
  {
    // Code size       234 (0xea)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_001a:  ldnull
    IL_001b:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0020:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0025:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_002f:  ldnull
    IL_0030:  ldtoken    method valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromMilliseconds(float64)
    IL_0035:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_003f:  ldc.i4.1
    IL_0040:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0045:  stloc.0
    IL_0046:  ldloc.0
    IL_0047:  ldc.i4.0
    IL_0048:  ldc.r8     10.000999999999999
    IL_0051:  box        [mscorlib]System.Double
    IL_0056:  ldtoken    [mscorlib]System.Double
    IL_005b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0060:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0065:  stelem.ref
    IL_0066:  ldloc.0
    IL_0067:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_006c:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::op_Addition(valuetype [mscorlib]System.DateTime,
                                                                                                           valuetype [mscorlib]System.TimeSpan)
    IL_0071:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0076:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0080:  ldc.i4.0
    IL_0081:  ldtoken    method bool [mscorlib]System.DateTime::op_GreaterThan(valuetype [mscorlib]System.DateTime,
                                                                               valuetype [mscorlib]System.DateTime)
    IL_0086:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_008b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0090:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  bool,
                                                                                                                                                  class [mscorlib]System.Reflection.MethodInfo)
    IL_0095:  ldtoken    method instance string [mscorlib]System.Boolean::ToString()
    IL_009a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_009f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a4:  ldc.i4.0
    IL_00a5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00aa:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00af:  ldstr      "False"
    IL_00b4:  ldtoken    [mscorlib]System.String
    IL_00b9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00be:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c3:  ldc.i4.0
    IL_00c4:  ldtoken    method bool [mscorlib]System.String::op_Equality(string,
                                                                          string)
    IL_00c9:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00ce:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00d3:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_00d8:  ldc.i4.0
    IL_00d9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00de:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e3:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00e8:  pop
    IL_00e9:  ret
  } // end of method ExpressionTrees::StaticMembers

  .method public hidebysig instance void 
          Strings() cil managed
  {
    // Code size       376 (0x178)
    .maxstack  8
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass1b' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass1b'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass1b'::i
    IL_000d:  ldloc.0
    IL_000e:  ldstr      "X"
    IL_0013:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass1b'::x
    IL_0018:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_001d:  ldstr      "a\n\\b"
    IL_0022:  ldtoken    [mscorlib]System.String
    IL_0027:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0031:  ldloc.0
    IL_0032:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0037:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass1b'::x
    IL_003c:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_004b:  ldloc.0
    IL_004c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0051:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass1b'::x
    IL_0056:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_005b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0060:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0065:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_006a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0074:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0079:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0083:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0088:  ldc.i4.2
    IL_0089:  box        [mscorlib]System.Int32
    IL_008e:  ldtoken    [mscorlib]System.Int32
    IL_0093:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0098:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00a2:  ldc.i4.0
    IL_00a3:  box        [mscorlib]System.Boolean
    IL_00a8:  ldtoken    [mscorlib]System.Boolean
    IL_00ad:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b7:  ldc.i4.1
    IL_00b8:  box        [mscorlib]System.Boolean
    IL_00bd:  ldtoken    [mscorlib]System.Boolean
    IL_00c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cc:  ldc.i4.1
    IL_00cd:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_00d2:  box        [mscorlib]System.Decimal
    IL_00d7:  ldtoken    [mscorlib]System.Decimal
    IL_00dc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e6:  ldloc.0
    IL_00e7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00ec:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass1b'::i
    IL_00f1:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f6:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00fb:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_0100:  ldtoken    [mscorlib]System.Decimal
    IL_0105:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010a:  ldtoken    method valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
    IL_010f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0114:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0119:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type,
                                                                                                                                             class [mscorlib]System.Reflection.MethodInfo)
    IL_011e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0123:  ldc.i4.0
    IL_0124:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0129:  box        [mscorlib]System.Decimal
    IL_012e:  ldtoken    [mscorlib]System.Decimal
    IL_0133:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0138:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_013d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0142:  ldc.i4.0
    IL_0143:  box        [mscorlib]System.Boolean
    IL_0148:  ldtoken    [mscorlib]System.Boolean
    IL_014d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0152:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0157:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_015c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0161:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0166:  ldc.i4.0
    IL_0167:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_016c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0171:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0176:  pop
    IL_0177:  ret
  } // end of method ExpressionTrees::Strings

  .method public hidebysig instance void 
          GenericClassInstance() cil managed
  {
    // Code size       150 (0x96)
    .maxstack  5
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::.ctor()
    IL_000a:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_000f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0019:  ldc.i4.0
    IL_001a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0024:  ldtoken    field !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::InstanceField
    IL_0029:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_002e:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle,
                                                                                                                               valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0038:  ldtoken    [mscorlib]System.Double
    IL_003d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0047:  ldtoken    method instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>::.ctor()
    IL_004c:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>
    IL_0051:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0056:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_005b:  ldc.i4.0
    IL_005c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0061:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0066:  ldtoken    method instance !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>::get_InstanceProperty()
    IL_006b:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>
    IL_0070:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0075:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_007f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0084:  ldc.i4.0
    IL_0085:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_008a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<float64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<float64>(object,
                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0094:  pop
    IL_0095:  ret
  } // end of method ExpressionTrees::GenericClassInstance

  .method public hidebysig instance void 
          GenericClassStatic() cil managed
  {
    // Code size       90 (0x5a)
    .maxstack  5
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    field !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::StaticField
    IL_000b:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_0010:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle,
                                                                                                                               valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_001a:  ldtoken    [mscorlib]System.Double
    IL_001f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0024:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0029:  ldnull
    IL_002a:  ldtoken    method !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>::get_StaticProperty()
    IL_002f:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>
    IL_0034:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0039:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_003e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0043:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0048:  ldc.i4.0
    IL_0049:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<float64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0053:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<float64>(object,
                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0058:  pop
    IL_0059:  ret
  } // end of method ExpressionTrees::GenericClassStatic

  .method public hidebysig instance void 
          InvokeGenericMethod() cil managed
  {
    // Code size       55 (0x37)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::GenericMethod<float64>()
    IL_000b:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_001a:  ldc.i4.0
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0025:  ldc.i4.0
    IL_0026:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0030:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0035:  pop
    IL_0036:  ret
  } // end of method ExpressionTrees::InvokeGenericMethod

  .method private hidebysig static void  Test<T>(!!T delegateExpression,
                                                 class [System.Core]System.Linq.Expressions.Expression`1<!!T> expressionTree) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method ExpressionTrees::Test

  .method public hidebysig static void  ArrayIndexer() cil managed
  {
    // Code size       645 (0x285)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_5,
             class [System.Core]System.Linq.Expressions.Expression[] V_6,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_7,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_8,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_9,
             class [System.Core]System.Linq.Expressions.Expression[] V_10,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_11,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_12,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_13,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_14)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate22'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayIndexer>b__1d'(int32[])
    IL_000e:  newobj     instance void class [mscorlib]System.Func`2<int32[],int32>::.ctor(object,
                                                                                           native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate22'
    IL_0018:  ldsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate22'
    IL_001d:  ldtoken    int32[]
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  ldstr      "array"
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0031:  stloc.0
    IL_0032:  ldloc.0
    IL_0033:  ldc.i4.0
    IL_0034:  box        [mscorlib]System.Int32
    IL_0039:  ldtoken    [mscorlib]System.Int32
    IL_003e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0043:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_004d:  ldc.i4.1
    IL_004e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0053:  stloc.1
    IL_0054:  ldloc.1
    IL_0055:  ldc.i4.0
    IL_0056:  ldloc.0
    IL_0057:  stelem.ref
    IL_0058:  ldloc.1
    IL_0059:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32[],int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32[],int32>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0063:  ldsfld     class [mscorlib]System.Func`3<int32[],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate23'
    IL_0068:  brtrue.s   IL_007b

    IL_006a:  ldnull
    IL_006b:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayIndexer>b__1e'(int32[],
                                                                                                                    int32)
    IL_0071:  newobj     instance void class [mscorlib]System.Func`3<int32[],int32,int32>::.ctor(object,
                                                                                                 native int)
    IL_0076:  stsfld     class [mscorlib]System.Func`3<int32[],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate23'
    IL_007b:  ldsfld     class [mscorlib]System.Func`3<int32[],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate23'
    IL_0080:  ldtoken    int32[]
    IL_0085:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008a:  ldstr      "array"
    IL_008f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0094:  stloc.2
    IL_0095:  ldtoken    [mscorlib]System.Int32
    IL_009a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009f:  ldstr      "index"
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00a9:  stloc.3
    IL_00aa:  ldloc.2
    IL_00ab:  ldloc.3
    IL_00ac:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_00b1:  ldc.i4.2
    IL_00b2:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b7:  stloc.s    V_4
    IL_00b9:  ldloc.s    V_4
    IL_00bb:  ldc.i4.0
    IL_00bc:  ldloc.2
    IL_00bd:  stelem.ref
    IL_00be:  ldloc.s    V_4
    IL_00c0:  ldc.i4.1
    IL_00c1:  ldloc.3
    IL_00c2:  stelem.ref
    IL_00c3:  ldloc.s    V_4
    IL_00c5:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32[],int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ca:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32[],int32,int32>>(!!0,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00cf:  ldsfld     class [mscorlib]System.Func`2<int32[0...,0...],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate24'
    IL_00d4:  brtrue.s   IL_00e7

    IL_00d6:  ldnull
    IL_00d7:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayIndexer>b__1f'(int32[0...,0...])
    IL_00dd:  newobj     instance void class [mscorlib]System.Func`2<int32[0...,0...],int32>::.ctor(object,
                                                                                                    native int)
    IL_00e2:  stsfld     class [mscorlib]System.Func`2<int32[0...,0...],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate24'
    IL_00e7:  ldsfld     class [mscorlib]System.Func`2<int32[0...,0...],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate24'
    IL_00ec:  ldtoken    int32[0...,0...]
    IL_00f1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f6:  ldstr      "array"
    IL_00fb:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0100:  stloc.s    V_5
    IL_0102:  ldloc.s    V_5
    IL_0104:  ldc.i4.2
    IL_0105:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_010a:  stloc.s    V_6
    IL_010c:  ldloc.s    V_6
    IL_010e:  ldc.i4.0
    IL_010f:  ldc.i4.0
    IL_0110:  box        [mscorlib]System.Int32
    IL_0115:  ldtoken    [mscorlib]System.Int32
    IL_011a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0124:  stelem.ref
    IL_0125:  ldloc.s    V_6
    IL_0127:  ldc.i4.1
    IL_0128:  ldc.i4.5
    IL_0129:  box        [mscorlib]System.Int32
    IL_012e:  ldtoken    [mscorlib]System.Int32
    IL_0133:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0138:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_013d:  stelem.ref
    IL_013e:  ldloc.s    V_6
    IL_0140:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0145:  ldc.i4.1
    IL_0146:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_014b:  stloc.s    V_7
    IL_014d:  ldloc.s    V_7
    IL_014f:  ldc.i4.0
    IL_0150:  ldloc.s    V_5
    IL_0152:  stelem.ref
    IL_0153:  ldloc.s    V_7
    IL_0155:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32[0...,0...],int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_015a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32[0...,0...],int32>>(!!0,
                                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_015f:  ldsfld     class [mscorlib]System.Func`3<int32[0...,0...],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate25'
    IL_0164:  brtrue.s   IL_0177

    IL_0166:  ldnull
    IL_0167:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayIndexer>b__20'(int32[0...,0...],
                                                                                                                    int32)
    IL_016d:  newobj     instance void class [mscorlib]System.Func`3<int32[0...,0...],int32,int32>::.ctor(object,
                                                                                                          native int)
    IL_0172:  stsfld     class [mscorlib]System.Func`3<int32[0...,0...],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate25'
    IL_0177:  ldsfld     class [mscorlib]System.Func`3<int32[0...,0...],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate25'
    IL_017c:  ldtoken    int32[0...,0...]
    IL_0181:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0186:  ldstr      "array"
    IL_018b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0190:  stloc.s    V_8
    IL_0192:  ldtoken    [mscorlib]System.Int32
    IL_0197:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_019c:  ldstr      "index"
    IL_01a1:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01a6:  stloc.s    V_9
    IL_01a8:  ldloc.s    V_8
    IL_01aa:  ldc.i4.2
    IL_01ab:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01b0:  stloc.s    V_10
    IL_01b2:  ldloc.s    V_10
    IL_01b4:  ldc.i4.0
    IL_01b5:  ldloc.s    V_9
    IL_01b7:  stelem.ref
    IL_01b8:  ldloc.s    V_10
    IL_01ba:  ldc.i4.1
    IL_01bb:  ldc.i4.7
    IL_01bc:  box        [mscorlib]System.Int32
    IL_01c1:  ldtoken    [mscorlib]System.Int32
    IL_01c6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01cb:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01d0:  stelem.ref
    IL_01d1:  ldloc.s    V_10
    IL_01d3:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_01d8:  ldc.i4.2
    IL_01d9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01de:  stloc.s    V_11
    IL_01e0:  ldloc.s    V_11
    IL_01e2:  ldc.i4.0
    IL_01e3:  ldloc.s    V_8
    IL_01e5:  stelem.ref
    IL_01e6:  ldloc.s    V_11
    IL_01e8:  ldc.i4.1
    IL_01e9:  ldloc.s    V_9
    IL_01eb:  stelem.ref
    IL_01ec:  ldloc.s    V_11
    IL_01ee:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32[0...,0...],int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                           class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01f3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32[0...,0...],int32,int32>>(!!0,
                                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01f8:  ldsfld     class [mscorlib]System.Func`3<int32[][],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate26'
    IL_01fd:  brtrue.s   IL_0210

    IL_01ff:  ldnull
    IL_0200:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayIndexer>b__21'(int32[][],
                                                                                                                    int32)
    IL_0206:  newobj     instance void class [mscorlib]System.Func`3<int32[][],int32,int32>::.ctor(object,
                                                                                                   native int)
    IL_020b:  stsfld     class [mscorlib]System.Func`3<int32[][],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate26'
    IL_0210:  ldsfld     class [mscorlib]System.Func`3<int32[][],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate26'
    IL_0215:  ldtoken    int32[][]
    IL_021a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_021f:  ldstr      "array"
    IL_0224:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0229:  stloc.s    V_12
    IL_022b:  ldtoken    [mscorlib]System.Int32
    IL_0230:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0235:  ldstr      "index"
    IL_023a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_023f:  stloc.s    V_13
    IL_0241:  ldloc.s    V_12
    IL_0243:  ldloc.s    V_13
    IL_0245:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_024a:  ldc.i4.7
    IL_024b:  box        [mscorlib]System.Int32
    IL_0250:  ldtoken    [mscorlib]System.Int32
    IL_0255:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_025a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_025f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_0264:  ldc.i4.2
    IL_0265:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_026a:  stloc.s    V_14
    IL_026c:  ldloc.s    V_14
    IL_026e:  ldc.i4.0
    IL_026f:  ldloc.s    V_12
    IL_0271:  stelem.ref
    IL_0272:  ldloc.s    V_14
    IL_0274:  ldc.i4.1
    IL_0275:  ldloc.s    V_13
    IL_0277:  stelem.ref
    IL_0278:  ldloc.s    V_14
    IL_027a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32[][],int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_027f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32[][],int32,int32>>(!!0,
                                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0284:  ret
  } // end of method ExpressionTrees::ArrayIndexer

  .method public hidebysig static void  ArrayLength() cil managed
  {
    // Code size       165 (0xa5)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate29'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayLength>b__27'(int32[])
    IL_000e:  newobj     instance void class [mscorlib]System.Func`2<int32[],int32>::.ctor(object,
                                                                                           native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate29'
    IL_0018:  ldsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate29'
    IL_001d:  ldtoken    int32[]
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  ldstr      "array"
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0031:  stloc.0
    IL_0032:  ldloc.0
    IL_0033:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayLength(class [System.Core]System.Linq.Expressions.Expression)
    IL_0038:  ldc.i4.1
    IL_0039:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003e:  stloc.1
    IL_003f:  ldloc.1
    IL_0040:  ldc.i4.0
    IL_0041:  ldloc.0
    IL_0042:  stelem.ref
    IL_0043:  ldloc.1
    IL_0044:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32[],int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0049:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32[],int32>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_004e:  ldsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate2a'
    IL_0053:  brtrue.s   IL_0066

    IL_0055:  ldnull
    IL_0056:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayLength>b__28'()
    IL_005c:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0061:  stsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate2a'
    IL_0066:  ldsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate2a'
    IL_006b:  ldnull
    IL_006c:  box        [mscorlib]System.Array
    IL_0071:  ldtoken    [mscorlib]System.Array
    IL_0076:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0080:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_0085:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_008a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_008f:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0094:  ldc.i4.0
    IL_0095:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_009a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_009f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32>>(!!0,
                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00a4:  ret
  } // end of method ExpressionTrees::ArrayLength

  .method public hidebysig static void  NewObj() cil managed
  {
    // Code size       591 (0x24f)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2)
    IL_0000:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate32'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NewObj>b__2b'()
    IL_000e:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate32'
    IL_0018:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate32'
    IL_001d:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::.ctor()
    IL_0022:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0027:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_002c:  ldc.i4.0
    IL_002d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0032:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0037:  ldc.i4.0
    IL_0038:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0042:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0047:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate33'
    IL_004c:  brtrue.s   IL_005f

    IL_004e:  ldnull
    IL_004f:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NewObj>b__2c'()
    IL_0055:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_005a:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate33'
    IL_005f:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate33'
    IL_0064:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithCtor::.ctor(int32)
    IL_0069:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_006e:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0073:  ldc.i4.1
    IL_0074:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0079:  stloc.0
    IL_007a:  ldloc.0
    IL_007b:  ldc.i4.0
    IL_007c:  ldc.i4.5
    IL_007d:  box        [mscorlib]System.Int32
    IL_0082:  ldtoken    [mscorlib]System.Int32
    IL_0087:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0091:  stelem.ref
    IL_0092:  ldloc.0
    IL_0093:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0098:  ldc.i4.0
    IL_0099:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_009e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00a8:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate34'
    IL_00ad:  brtrue.s   IL_00c0

    IL_00af:  ldnull
    IL_00b0:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NewObj>b__2d'()
    IL_00b6:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_00bb:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate34'
    IL_00c0:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate34'
    IL_00c5:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors::.ctor()
    IL_00ca:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00cf:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_00d4:  ldc.i4.0
    IL_00d5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00da:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_00df:  ldc.i4.0
    IL_00e0:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e5:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ea:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00ef:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate35'
    IL_00f4:  brtrue.s   IL_0107

    IL_00f6:  ldnull
    IL_00f7:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NewObj>b__2e'()
    IL_00fd:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_0102:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate35'
    IL_0107:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate35'
    IL_010c:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors::.ctor(int32)
    IL_0111:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0116:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_011b:  ldc.i4.1
    IL_011c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0121:  stloc.1
    IL_0122:  ldloc.1
    IL_0123:  ldc.i4.0
    IL_0124:  ldc.i4.5
    IL_0125:  box        [mscorlib]System.Int32
    IL_012a:  ldtoken    [mscorlib]System.Int32
    IL_012f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0134:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0139:  stelem.ref
    IL_013a:  ldloc.1
    IL_013b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0140:  ldc.i4.0
    IL_0141:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0146:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_014b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0150:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate36'
    IL_0155:  brtrue.s   IL_0168

    IL_0157:  ldnull
    IL_0158:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NewObj>b__2f'()
    IL_015e:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_0163:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate36'
    IL_0168:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate36'
    IL_016d:  ldtoken    method instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::.ctor()
    IL_0172:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_0177:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_017c:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0181:  ldc.i4.0
    IL_0182:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0187:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_018c:  ldc.i4.0
    IL_018d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0192:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0197:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_019c:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate37'
    IL_01a1:  brtrue.s   IL_01b4

    IL_01a3:  ldnull
    IL_01a4:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NewObj>b__30'()
    IL_01aa:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_01af:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate37'
    IL_01b4:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate37'
    IL_01b9:  ldtoken    method instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1<int32>::.ctor()
    IL_01be:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1<int32>
    IL_01c3:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01c8:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_01cd:  ldc.i4.0
    IL_01ce:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01d3:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_01d8:  ldc.i4.0
    IL_01d9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01de:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01e3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01e8:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate38'
    IL_01ed:  brtrue.s   IL_0200

    IL_01ef:  ldnull
    IL_01f0:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NewObj>b__31'()
    IL_01f6:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_01fb:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate38'
    IL_0200:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate38'
    IL_0205:  ldtoken    method instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1<int32>::.ctor(int32)
    IL_020a:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1<int32>
    IL_020f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0214:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0219:  ldc.i4.1
    IL_021a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_021f:  stloc.2
    IL_0220:  ldloc.2
    IL_0221:  ldc.i4.0
    IL_0222:  ldc.i4.5
    IL_0223:  box        [mscorlib]System.Int32
    IL_0228:  ldtoken    [mscorlib]System.Int32
    IL_022d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0232:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0237:  stelem.ref
    IL_0238:  ldloc.2
    IL_0239:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_023e:  ldc.i4.0
    IL_023f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0244:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0249:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_024e:  ret
  } // end of method ExpressionTrees::NewObj

  .method public hidebysig static void  TypeOfExpr() cil managed
  {
    // Code size       376 (0x178)
    .maxstack  3
    IL_0000:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate3e'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<TypeOfExpr>b__39'()
    IL_000e:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate3e'
    IL_0018:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate3e'
    IL_001d:  ldtoken    [mscorlib]System.Int32
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  box        [mscorlib]System.Type
    IL_002c:  ldtoken    [mscorlib]System.Type
    IL_0031:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0036:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_003b:  ldc.i4.0
    IL_003c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0041:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0046:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_004b:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate3f'
    IL_0050:  brtrue.s   IL_0063

    IL_0052:  ldnull
    IL_0053:  ldftn      class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<TypeOfExpr>b__3a'()
    IL_0059:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_005e:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate3f'
    IL_0063:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate3f'
    IL_0068:  ldtoken    [mscorlib]System.Object
    IL_006d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0072:  box        [mscorlib]System.Type
    IL_0077:  ldtoken    [mscorlib]System.Type
    IL_007c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0081:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0086:  ldc.i4.0
    IL_0087:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_008c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0091:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0096:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate40'
    IL_009b:  brtrue.s   IL_00ae

    IL_009d:  ldnull
    IL_009e:  ldftn      class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<TypeOfExpr>b__3b'()
    IL_00a4:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_00a9:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate40'
    IL_00ae:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate40'
    IL_00b3:  ldtoken    [mscorlib]System.Collections.Generic.List`1
    IL_00b8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bd:  box        [mscorlib]System.Type
    IL_00c2:  ldtoken    [mscorlib]System.Type
    IL_00c7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d1:  ldc.i4.0
    IL_00d2:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00d7:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00dc:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00e1:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate41'
    IL_00e6:  brtrue.s   IL_00f9

    IL_00e8:  ldnull
    IL_00e9:  ldftn      class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<TypeOfExpr>b__3c'()
    IL_00ef:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_00f4:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate41'
    IL_00f9:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate41'
    IL_00fe:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0103:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0108:  box        [mscorlib]System.Type
    IL_010d:  ldtoken    [mscorlib]System.Type
    IL_0112:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0117:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_011c:  ldc.i4.0
    IL_011d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0122:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0127:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_012c:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate42'
    IL_0131:  brtrue.s   IL_0144

    IL_0133:  ldnull
    IL_0134:  ldftn      class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<TypeOfExpr>b__3d'()
    IL_013a:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_013f:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate42'
    IL_0144:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate42'
    IL_0149:  ldtoken    int32*
    IL_014e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0153:  box        [mscorlib]System.Type
    IL_0158:  ldtoken    [mscorlib]System.Type
    IL_015d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0162:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0167:  ldc.i4.0
    IL_0168:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_016d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0172:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0177:  ret
  } // end of method ExpressionTrees::TypeOfExpr

  .method public hidebysig static void  AsTypeExpr() cil managed
  {
    // Code size       177 (0xb1)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_3)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate45'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<AsTypeExpr>b__43'(object)
    IL_000e:  newobj     instance void class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass>::.ctor(object,
                                                                                                                                                native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate45'
    IL_0018:  ldsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate45'
    IL_001d:  ldtoken    [mscorlib]System.Object
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  ldstr      "obj"
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0031:  stloc.0
    IL_0032:  ldloc.0
    IL_0033:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::TypeAs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Type)
    IL_0042:  ldc.i4.1
    IL_0043:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0048:  stloc.1
    IL_0049:  ldloc.1
    IL_004a:  ldc.i4.0
    IL_004b:  ldloc.0
    IL_004c:  stelem.ref
    IL_004d:  ldloc.1
    IL_004e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0053:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass>>(!!0,
                                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0058:  ldsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate46'
    IL_005d:  brtrue.s   IL_0070

    IL_005f:  ldnull
    IL_0060:  ldftn      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<AsTypeExpr>b__44'(object)
    IL_0066:  newobj     instance void class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>>::.ctor(object,
                                                                                                                                                                               native int)
    IL_006b:  stsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate46'
    IL_0070:  ldsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate46'
    IL_0075:  ldtoken    [mscorlib]System.Object
    IL_007a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007f:  ldstr      "obj"
    IL_0084:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0089:  stloc.2
    IL_008a:  ldloc.2
    IL_008b:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>
    IL_0090:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0095:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::TypeAs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Type)
    IL_009a:  ldc.i4.1
    IL_009b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a0:  stloc.3
    IL_00a1:  ldloc.3
    IL_00a2:  ldc.i4.0
    IL_00a3:  ldloc.2
    IL_00a4:  stelem.ref
    IL_00a5:  ldloc.3
    IL_00a6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ab:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>>>(!!0,
                                                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00b0:  ret
  } // end of method ExpressionTrees::AsTypeExpr

  .method public hidebysig static void  IsTypeExpr() cil managed
  {
    // Code size       89 (0x59)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<object,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate48'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<IsTypeExpr>b__47'(object)
    IL_000e:  newobj     instance void class [mscorlib]System.Func`2<object,bool>::.ctor(object,
                                                                                         native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`2<object,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate48'
    IL_0018:  ldsfld     class [mscorlib]System.Func`2<object,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate48'
    IL_001d:  ldtoken    [mscorlib]System.Object
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  ldstr      "obj"
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0031:  stloc.0
    IL_0032:  ldloc.0
    IL_0033:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.TypeBinaryExpression [System.Core]System.Linq.Expressions.Expression::TypeIs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0042:  ldc.i4.1
    IL_0043:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0048:  stloc.1
    IL_0049:  ldloc.1
    IL_004a:  ldc.i4.0
    IL_004b:  ldloc.0
    IL_004c:  stelem.ref
    IL_004d:  ldloc.1
    IL_004e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0053:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<object,bool>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0058:  ret
  } // end of method ExpressionTrees::IsTypeExpr

  .method public hidebysig static void  UnaryLogicalOperators() cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<bool,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4a'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<UnaryLogicalOperators>b__49'(bool)
    IL_000e:  newobj     instance void class [mscorlib]System.Func`2<bool,bool>::.ctor(object,
                                                                                       native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`2<bool,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4a'
    IL_0018:  ldsfld     class [mscorlib]System.Func`2<bool,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4a'
    IL_001d:  ldtoken    [mscorlib]System.Boolean
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  ldstr      "a"
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0031:  stloc.0
    IL_0032:  ldloc.0
    IL_0033:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0038:  ldc.i4.1
    IL_0039:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003e:  stloc.1
    IL_003f:  ldloc.1
    IL_0040:  ldc.i4.0
    IL_0041:  ldloc.0
    IL_0042:  stelem.ref
    IL_0043:  ldloc.1
    IL_0044:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<bool,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0049:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<bool,bool>>(!!0,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_004e:  ret
  } // end of method ExpressionTrees::UnaryLogicalOperators

  .method public hidebysig static void  ConditionalOperator() cil managed
  {
    // Code size       172 (0xac)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_3)
    IL_0000:  ldnull
    IL_0001:  ldtoken    [mscorlib]System.Boolean
    IL_0006:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000b:  ldstr      "a"
    IL_0010:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.5
    IL_0018:  box        [mscorlib]System.Int32
    IL_001d:  ldtoken    [mscorlib]System.Int32
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002c:  ldc.i4.s   10
    IL_002e:  box        [mscorlib]System.Int32
    IL_0033:  ldtoken    [mscorlib]System.Int32
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0047:  ldc.i4.1
    IL_0048:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004d:  stloc.1
    IL_004e:  ldloc.1
    IL_004f:  ldc.i4.0
    IL_0050:  ldloc.0
    IL_0051:  stelem.ref
    IL_0052:  ldloc.1
    IL_0053:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<bool,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0058:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool,int32>(object,
                                                                                                                  class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_005d:  pop
    IL_005e:  ldnull
    IL_005f:  ldtoken    [mscorlib]System.Object
    IL_0064:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0069:  ldstr      "a"
    IL_006e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0073:  stloc.2
    IL_0074:  ldloc.2
    IL_0075:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass::.ctor()
    IL_007a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0084:  ldc.i4.0
    IL_0085:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_008a:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_008f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0094:  ldc.i4.1
    IL_0095:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_009a:  stloc.3
    IL_009b:  ldloc.3
    IL_009c:  ldc.i4.0
    IL_009d:  ldloc.2
    IL_009e:  stelem.ref
    IL_009f:  ldloc.3
    IL_00a0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a5:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<object,object>(object,
                                                                                                                     class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_00aa:  pop
    IL_00ab:  ret
  } // end of method ExpressionTrees::ConditionalOperator

  .method public hidebysig static void  BinaryLogicalOperators() cil managed
  {
    // Code size       1795 (0x703)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_5,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_6,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_7,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_8,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_9,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_10,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_11,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_12,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_13,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_14,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_15,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_16,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_17,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_18,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_19,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_20,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_21,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_22,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_23,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_24,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_25,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_26,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_27,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_28,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_29,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_30,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_31,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_32,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_33,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_34,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_35,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_36,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_37,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_38,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_39,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_40,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_41,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_42,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_43,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_44,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_45,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_46,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_47,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_48,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_49,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_50)
    IL_0000:  ldnull
    IL_0001:  ldtoken    [mscorlib]System.Int32
    IL_0006:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000b:  ldstr      "a"
    IL_0010:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0015:  stloc.0
    IL_0016:  ldtoken    [mscorlib]System.Int32
    IL_001b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0020:  ldstr      "b"
    IL_0025:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldloc.1
    IL_002d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0032:  ldc.i4.2
    IL_0033:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0038:  stloc.2
    IL_0039:  ldloc.2
    IL_003a:  ldc.i4.0
    IL_003b:  ldloc.0
    IL_003c:  stelem.ref
    IL_003d:  ldloc.2
    IL_003e:  ldc.i4.1
    IL_003f:  ldloc.1
    IL_0040:  stelem.ref
    IL_0041:  ldloc.2
    IL_0042:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0047:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_004c:  pop
    IL_004d:  ldnull
    IL_004e:  ldtoken    [mscorlib]System.Int32
    IL_0053:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0058:  ldstr      "a"
    IL_005d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0062:  stloc.3
    IL_0063:  ldtoken    [mscorlib]System.Int32
    IL_0068:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006d:  ldstr      "b"
    IL_0072:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0077:  stloc.s    V_4
    IL_0079:  ldloc.3
    IL_007a:  ldloc.s    V_4
    IL_007c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0081:  ldc.i4.2
    IL_0082:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0087:  stloc.s    V_5
    IL_0089:  ldloc.s    V_5
    IL_008b:  ldc.i4.0
    IL_008c:  ldloc.3
    IL_008d:  stelem.ref
    IL_008e:  ldloc.s    V_5
    IL_0090:  ldc.i4.1
    IL_0091:  ldloc.s    V_4
    IL_0093:  stelem.ref
    IL_0094:  ldloc.s    V_5
    IL_0096:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_009b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_00a0:  pop
    IL_00a1:  ldnull
    IL_00a2:  ldtoken    [mscorlib]System.Int32
    IL_00a7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ac:  ldstr      "a"
    IL_00b1:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00b6:  stloc.s    V_6
    IL_00b8:  ldtoken    [mscorlib]System.Int32
    IL_00bd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c2:  ldstr      "b"
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00cc:  stloc.s    V_7
    IL_00ce:  ldloc.s    V_6
    IL_00d0:  ldloc.s    V_7
    IL_00d2:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00d7:  ldc.i4.2
    IL_00d8:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00dd:  stloc.s    V_8
    IL_00df:  ldloc.s    V_8
    IL_00e1:  ldc.i4.0
    IL_00e2:  ldloc.s    V_6
    IL_00e4:  stelem.ref
    IL_00e5:  ldloc.s    V_8
    IL_00e7:  ldc.i4.1
    IL_00e8:  ldloc.s    V_7
    IL_00ea:  stelem.ref
    IL_00eb:  ldloc.s    V_8
    IL_00ed:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00f2:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_00f7:  pop
    IL_00f8:  ldnull
    IL_00f9:  ldtoken    [mscorlib]System.Int32
    IL_00fe:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0103:  ldstr      "a"
    IL_0108:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_010d:  stloc.s    V_9
    IL_010f:  ldtoken    [mscorlib]System.Int32
    IL_0114:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0119:  ldstr      "b"
    IL_011e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0123:  stloc.s    V_10
    IL_0125:  ldloc.s    V_9
    IL_0127:  ldloc.s    V_10
    IL_0129:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression)
    IL_012e:  ldc.i4.2
    IL_012f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0134:  stloc.s    V_11
    IL_0136:  ldloc.s    V_11
    IL_0138:  ldc.i4.0
    IL_0139:  ldloc.s    V_9
    IL_013b:  stelem.ref
    IL_013c:  ldloc.s    V_11
    IL_013e:  ldc.i4.1
    IL_013f:  ldloc.s    V_10
    IL_0141:  stelem.ref
    IL_0142:  ldloc.s    V_11
    IL_0144:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0149:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_014e:  pop
    IL_014f:  ldnull
    IL_0150:  ldtoken    [mscorlib]System.Int32
    IL_0155:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_015a:  ldstr      "a"
    IL_015f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0164:  stloc.s    V_12
    IL_0166:  ldtoken    [mscorlib]System.Int32
    IL_016b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0170:  ldstr      "b"
    IL_0175:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_017a:  stloc.s    V_13
    IL_017c:  ldloc.s    V_12
    IL_017e:  ldloc.s    V_13
    IL_0180:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0185:  ldc.i4.2
    IL_0186:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_018b:  stloc.s    V_14
    IL_018d:  ldloc.s    V_14
    IL_018f:  ldc.i4.0
    IL_0190:  ldloc.s    V_12
    IL_0192:  stelem.ref
    IL_0193:  ldloc.s    V_14
    IL_0195:  ldc.i4.1
    IL_0196:  ldloc.s    V_13
    IL_0198:  stelem.ref
    IL_0199:  ldloc.s    V_14
    IL_019b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01a0:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_01a5:  pop
    IL_01a6:  ldnull
    IL_01a7:  ldtoken    [mscorlib]System.Int32
    IL_01ac:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b1:  ldstr      "a"
    IL_01b6:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01bb:  stloc.s    V_15
    IL_01bd:  ldtoken    [mscorlib]System.Int32
    IL_01c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01c7:  ldstr      "b"
    IL_01cc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01d1:  stloc.s    V_16
    IL_01d3:  ldloc.s    V_15
    IL_01d5:  ldloc.s    V_16
    IL_01d7:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression)
    IL_01dc:  ldc.i4.2
    IL_01dd:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01e2:  stloc.s    V_17
    IL_01e4:  ldloc.s    V_17
    IL_01e6:  ldc.i4.0
    IL_01e7:  ldloc.s    V_15
    IL_01e9:  stelem.ref
    IL_01ea:  ldloc.s    V_17
    IL_01ec:  ldc.i4.1
    IL_01ed:  ldloc.s    V_16
    IL_01ef:  stelem.ref
    IL_01f0:  ldloc.s    V_17
    IL_01f2:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01f7:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_01fc:  pop
    IL_01fd:  ldnull
    IL_01fe:  ldtoken    [mscorlib]System.Int32
    IL_0203:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0208:  ldstr      "a"
    IL_020d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0212:  stloc.s    V_18
    IL_0214:  ldtoken    [mscorlib]System.Int32
    IL_0219:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_021e:  ldstr      "b"
    IL_0223:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0228:  stloc.s    V_19
    IL_022a:  ldloc.s    V_18
    IL_022c:  ldc.i4.1
    IL_022d:  box        [mscorlib]System.Int32
    IL_0232:  ldtoken    [mscorlib]System.Int32
    IL_0237:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_023c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0241:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0246:  ldloc.s    V_19
    IL_0248:  ldc.i4.2
    IL_0249:  box        [mscorlib]System.Int32
    IL_024e:  ldtoken    [mscorlib]System.Int32
    IL_0253:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0258:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_025d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0262:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0267:  ldc.i4.2
    IL_0268:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_026d:  stloc.s    V_20
    IL_026f:  ldloc.s    V_20
    IL_0271:  ldc.i4.0
    IL_0272:  ldloc.s    V_18
    IL_0274:  stelem.ref
    IL_0275:  ldloc.s    V_20
    IL_0277:  ldc.i4.1
    IL_0278:  ldloc.s    V_19
    IL_027a:  stelem.ref
    IL_027b:  ldloc.s    V_20
    IL_027d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0282:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0287:  pop
    IL_0288:  ldnull
    IL_0289:  ldtoken    [mscorlib]System.Int32
    IL_028e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0293:  ldstr      "a"
    IL_0298:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_029d:  stloc.s    V_21
    IL_029f:  ldtoken    [mscorlib]System.Int32
    IL_02a4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02a9:  ldstr      "b"
    IL_02ae:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02b3:  stloc.s    V_22
    IL_02b5:  ldloc.s    V_21
    IL_02b7:  ldc.i4.1
    IL_02b8:  box        [mscorlib]System.Int32
    IL_02bd:  ldtoken    [mscorlib]System.Int32
    IL_02c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02c7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02cc:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_02d1:  ldloc.s    V_22
    IL_02d3:  ldc.i4.2
    IL_02d4:  box        [mscorlib]System.Int32
    IL_02d9:  ldtoken    [mscorlib]System.Int32
    IL_02de:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02e3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02e8:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_02ed:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_02f2:  ldc.i4.2
    IL_02f3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_02f8:  stloc.s    V_23
    IL_02fa:  ldloc.s    V_23
    IL_02fc:  ldc.i4.0
    IL_02fd:  ldloc.s    V_21
    IL_02ff:  stelem.ref
    IL_0300:  ldloc.s    V_23
    IL_0302:  ldc.i4.1
    IL_0303:  ldloc.s    V_22
    IL_0305:  stelem.ref
    IL_0306:  ldloc.s    V_23
    IL_0308:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_030d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0312:  pop
    IL_0313:  ldnull
    IL_0314:  ldtoken    [mscorlib]System.Int32
    IL_0319:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_031e:  ldstr      "a"
    IL_0323:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0328:  stloc.s    V_24
    IL_032a:  ldtoken    [mscorlib]System.Int16
    IL_032f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0334:  ldstr      "b"
    IL_0339:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_033e:  stloc.s    V_25
    IL_0340:  ldloc.s    V_24
    IL_0342:  ldloc.s    V_25
    IL_0344:  ldtoken    [mscorlib]System.Int32
    IL_0349:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_034e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0353:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0358:  ldc.i4.2
    IL_0359:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_035e:  stloc.s    V_26
    IL_0360:  ldloc.s    V_26
    IL_0362:  ldc.i4.0
    IL_0363:  ldloc.s    V_24
    IL_0365:  stelem.ref
    IL_0366:  ldloc.s    V_26
    IL_0368:  ldc.i4.1
    IL_0369:  ldloc.s    V_25
    IL_036b:  stelem.ref
    IL_036c:  ldloc.s    V_26
    IL_036e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int16,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0373:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int16,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0378:  pop
    IL_0379:  ldnull
    IL_037a:  ldtoken    [mscorlib]System.UInt16
    IL_037f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0384:  ldstr      "a"
    IL_0389:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_038e:  stloc.s    V_27
    IL_0390:  ldtoken    [mscorlib]System.Int32
    IL_0395:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_039a:  ldstr      "b"
    IL_039f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03a4:  stloc.s    V_28
    IL_03a6:  ldloc.s    V_27
    IL_03a8:  ldtoken    [mscorlib]System.Int32
    IL_03ad:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03b2:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_03b7:  ldloc.s    V_28
    IL_03b9:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_03be:  ldc.i4.2
    IL_03bf:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03c4:  stloc.s    V_29
    IL_03c6:  ldloc.s    V_29
    IL_03c8:  ldc.i4.0
    IL_03c9:  ldloc.s    V_27
    IL_03cb:  stelem.ref
    IL_03cc:  ldloc.s    V_29
    IL_03ce:  ldc.i4.1
    IL_03cf:  ldloc.s    V_28
    IL_03d1:  stelem.ref
    IL_03d2:  ldloc.s    V_29
    IL_03d4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<uint16,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03d9:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<uint16,int32,bool>(object,
                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_03de:  pop
    IL_03df:  ldnull
    IL_03e0:  ldtoken    [mscorlib]System.Int32
    IL_03e5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03ea:  ldstr      "a"
    IL_03ef:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03f4:  stloc.s    V_30
    IL_03f6:  ldtoken    [mscorlib]System.Int64
    IL_03fb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0400:  ldstr      "b"
    IL_0405:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_040a:  stloc.s    V_31
    IL_040c:  ldloc.s    V_30
    IL_040e:  ldtoken    [mscorlib]System.Int64
    IL_0413:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0418:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_041d:  ldloc.s    V_31
    IL_041f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0424:  ldc.i4.2
    IL_0425:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_042a:  stloc.s    V_32
    IL_042c:  ldloc.s    V_32
    IL_042e:  ldc.i4.0
    IL_042f:  ldloc.s    V_30
    IL_0431:  stelem.ref
    IL_0432:  ldloc.s    V_32
    IL_0434:  ldc.i4.1
    IL_0435:  ldloc.s    V_31
    IL_0437:  stelem.ref
    IL_0438:  ldloc.s    V_32
    IL_043a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int64,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_043f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int64,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0444:  pop
    IL_0445:  ldnull
    IL_0446:  ldtoken    [mscorlib]System.UInt64
    IL_044b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0450:  ldstr      "a"
    IL_0455:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_045a:  stloc.s    V_33
    IL_045c:  ldtoken    [mscorlib]System.UInt32
    IL_0461:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0466:  ldstr      "b"
    IL_046b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0470:  stloc.s    V_34
    IL_0472:  ldloc.s    V_33
    IL_0474:  ldloc.s    V_34
    IL_0476:  ldtoken    [mscorlib]System.UInt64
    IL_047b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0480:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0485:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression)
    IL_048a:  ldc.i4.2
    IL_048b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0490:  stloc.s    V_35
    IL_0492:  ldloc.s    V_35
    IL_0494:  ldc.i4.0
    IL_0495:  ldloc.s    V_33
    IL_0497:  stelem.ref
    IL_0498:  ldloc.s    V_35
    IL_049a:  ldc.i4.1
    IL_049b:  ldloc.s    V_34
    IL_049d:  stelem.ref
    IL_049e:  ldloc.s    V_35
    IL_04a0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<uint64,uint32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_04a5:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<uint64,uint32,bool>(object,
                                                                                                                          class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_04aa:  pop
    IL_04ab:  ldnull
    IL_04ac:  ldtoken    [mscorlib]System.Int32
    IL_04b1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04b6:  ldstr      "a"
    IL_04bb:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04c0:  stloc.s    V_36
    IL_04c2:  ldtoken    [mscorlib]System.UInt32
    IL_04c7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04cc:  ldstr      "b"
    IL_04d1:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04d6:  stloc.s    V_37
    IL_04d8:  ldloc.s    V_36
    IL_04da:  ldtoken    [mscorlib]System.Int64
    IL_04df:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04e4:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_04e9:  ldloc.s    V_37
    IL_04eb:  ldtoken    [mscorlib]System.Int64
    IL_04f0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04f5:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_04fa:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression)
    IL_04ff:  ldc.i4.2
    IL_0500:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0505:  stloc.s    V_38
    IL_0507:  ldloc.s    V_38
    IL_0509:  ldc.i4.0
    IL_050a:  ldloc.s    V_36
    IL_050c:  stelem.ref
    IL_050d:  ldloc.s    V_38
    IL_050f:  ldc.i4.1
    IL_0510:  ldloc.s    V_37
    IL_0512:  stelem.ref
    IL_0513:  ldloc.s    V_38
    IL_0515:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,uint32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_051a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,uint32,bool>(object,
                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_051f:  pop
    IL_0520:  ldnull
    IL_0521:  ldtoken    [mscorlib]System.Int32
    IL_0526:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_052b:  ldstr      "a"
    IL_0530:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0535:  stloc.s    V_39
    IL_0537:  ldtoken    [mscorlib]System.Int64
    IL_053c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0541:  ldstr      "b"
    IL_0546:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_054b:  stloc.s    V_40
    IL_054d:  ldloc.s    V_39
    IL_054f:  ldtoken    [mscorlib]System.Int64
    IL_0554:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0559:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_055e:  ldloc.s    V_40
    IL_0560:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0565:  ldc.i4.2
    IL_0566:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_056b:  stloc.s    V_41
    IL_056d:  ldloc.s    V_41
    IL_056f:  ldc.i4.0
    IL_0570:  ldloc.s    V_39
    IL_0572:  stelem.ref
    IL_0573:  ldloc.s    V_41
    IL_0575:  ldc.i4.1
    IL_0576:  ldloc.s    V_40
    IL_0578:  stelem.ref
    IL_0579:  ldloc.s    V_41
    IL_057b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int64,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0580:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int64,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0585:  pop
    IL_0586:  ldnull
    IL_0587:  ldtoken    [mscorlib]System.Int16
    IL_058c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0591:  ldstr      "a"
    IL_0596:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_059b:  stloc.s    V_42
    IL_059d:  ldtoken    [mscorlib]System.Int64
    IL_05a2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05a7:  ldstr      "b"
    IL_05ac:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_05b1:  stloc.s    V_43
    IL_05b3:  ldloc.s    V_42
    IL_05b5:  ldtoken    [mscorlib]System.Int64
    IL_05ba:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05bf:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_05c4:  ldloc.s    V_43
    IL_05c6:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression)
    IL_05cb:  ldc.i4.2
    IL_05cc:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_05d1:  stloc.s    V_44
    IL_05d3:  ldloc.s    V_44
    IL_05d5:  ldc.i4.0
    IL_05d6:  ldloc.s    V_42
    IL_05d8:  stelem.ref
    IL_05d9:  ldloc.s    V_44
    IL_05db:  ldc.i4.1
    IL_05dc:  ldloc.s    V_43
    IL_05de:  stelem.ref
    IL_05df:  ldloc.s    V_44
    IL_05e1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int64,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_05e6:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int16,int64,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_05eb:  pop
    IL_05ec:  ldnull
    IL_05ed:  ldtoken    [mscorlib]System.Int32
    IL_05f2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05f7:  ldstr      "a"
    IL_05fc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0601:  stloc.s    V_45
    IL_0603:  ldtoken    [mscorlib]System.Int32
    IL_0608:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_060d:  ldstr      "b"
    IL_0612:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0617:  stloc.s    V_46
    IL_0619:  ldloc.s    V_45
    IL_061b:  ldc.i4.1
    IL_061c:  box        [mscorlib]System.Int32
    IL_0621:  ldtoken    [mscorlib]System.Int32
    IL_0626:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_062b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0630:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0635:  ldloc.s    V_46
    IL_0637:  ldc.i4.2
    IL_0638:  box        [mscorlib]System.Int32
    IL_063d:  ldtoken    [mscorlib]System.Int32
    IL_0642:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0647:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_064c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0651:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0656:  ldc.i4.2
    IL_0657:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_065c:  stloc.s    V_47
    IL_065e:  ldloc.s    V_47
    IL_0660:  ldc.i4.0
    IL_0661:  ldloc.s    V_45
    IL_0663:  stelem.ref
    IL_0664:  ldloc.s    V_47
    IL_0666:  ldc.i4.1
    IL_0667:  ldloc.s    V_46
    IL_0669:  stelem.ref
    IL_066a:  ldloc.s    V_47
    IL_066c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0671:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0676:  pop
    IL_0677:  ldnull
    IL_0678:  ldtoken    [mscorlib]System.Int32
    IL_067d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0682:  ldstr      "a"
    IL_0687:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_068c:  stloc.s    V_48
    IL_068e:  ldtoken    [mscorlib]System.Int32
    IL_0693:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0698:  ldstr      "b"
    IL_069d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_06a2:  stloc.s    V_49
    IL_06a4:  ldloc.s    V_48
    IL_06a6:  ldc.i4.1
    IL_06a7:  box        [mscorlib]System.Int32
    IL_06ac:  ldtoken    [mscorlib]System.Int32
    IL_06b1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_06b6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_06bb:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_06c0:  ldloc.s    V_49
    IL_06c2:  ldc.i4.2
    IL_06c3:  box        [mscorlib]System.Int32
    IL_06c8:  ldtoken    [mscorlib]System.Int32
    IL_06cd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_06d2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_06d7:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_06dc:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_06e1:  ldc.i4.2
    IL_06e2:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_06e7:  stloc.s    V_50
    IL_06e9:  ldloc.s    V_50
    IL_06eb:  ldc.i4.0
    IL_06ec:  ldloc.s    V_48
    IL_06ee:  stelem.ref
    IL_06ef:  ldloc.s    V_50
    IL_06f1:  ldc.i4.1
    IL_06f2:  ldloc.s    V_49
    IL_06f4:  stelem.ref
    IL_06f5:  ldloc.s    V_50
    IL_06f7:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_06fc:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0701:  pop
    IL_0702:  ret
  } // end of method ExpressionTrees::BinaryLogicalOperators

  .method public hidebysig static void  UnaryArithmeticOperators() cil managed
  {
    // Code size       152 (0x98)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_3)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4d'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<UnaryArithmeticOperators>b__4b'(int32)
    IL_000e:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4d'
    IL_0018:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4d'
    IL_001d:  ldtoken    [mscorlib]System.Int32
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  ldstr      "a"
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0031:  stloc.0
    IL_0032:  ldloc.0
    IL_0033:  ldc.i4.1
    IL_0034:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0039:  stloc.1
    IL_003a:  ldloc.1
    IL_003b:  ldc.i4.0
    IL_003c:  ldloc.0
    IL_003d:  stelem.ref
    IL_003e:  ldloc.1
    IL_003f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0044:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0049:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4e'
    IL_004e:  brtrue.s   IL_0061

    IL_0050:  ldnull
    IL_0051:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<UnaryArithmeticOperators>b__4c'(int32)
    IL_0057:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_005c:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4e'
    IL_0061:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4e'
    IL_0066:  ldtoken    [mscorlib]System.Int32
    IL_006b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0070:  ldstr      "a"
    IL_0075:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_007a:  stloc.2
    IL_007b:  ldloc.2
    IL_007c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_0081:  ldc.i4.1
    IL_0082:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0087:  stloc.3
    IL_0088:  ldloc.3
    IL_0089:  ldc.i4.0
    IL_008a:  ldloc.2
    IL_008b:  stelem.ref
    IL_008c:  ldloc.3
    IL_008d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0092:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0097:  ret
  } // end of method ExpressionTrees::UnaryArithmeticOperators

  .method public hidebysig static void  BinaryArithmeticOperators() cil managed
  {
    // Code size       1848 (0x738)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_5,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_6,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_7,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_8,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_9,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_10,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_11,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_12,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_13,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_14,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_15,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_16,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_17,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_18,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_19,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_20,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_21,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_22,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_23,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_24,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_25,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_26,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_27,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_28,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_29,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_30,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_31,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_32,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_33,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_34,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_35,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_36,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_37,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_38,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_39,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_40,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_41,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_42,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_43,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_44)
    IL_0000:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate5e'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__4f'(int32,
                                                                                                                                 int32)
    IL_000e:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate5e'
    IL_0018:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate5e'
    IL_001d:  ldtoken    [mscorlib]System.Int32
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  ldstr      "a"
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0031:  stloc.0
    IL_0032:  ldtoken    [mscorlib]System.Int32
    IL_0037:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  ldstr      "b"
    IL_0041:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0046:  stloc.1
    IL_0047:  ldloc.0
    IL_0048:  ldloc.1
    IL_0049:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_004e:  ldc.i4.2
    IL_004f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0054:  stloc.2
    IL_0055:  ldloc.2
    IL_0056:  ldc.i4.0
    IL_0057:  ldloc.0
    IL_0058:  stelem.ref
    IL_0059:  ldloc.2
    IL_005a:  ldc.i4.1
    IL_005b:  ldloc.1
    IL_005c:  stelem.ref
    IL_005d:  ldloc.2
    IL_005e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0063:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0068:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate5f'
    IL_006d:  brtrue.s   IL_0080

    IL_006f:  ldnull
    IL_0070:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__50'(int32,
                                                                                                                                 int32)
    IL_0076:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_007b:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate5f'
    IL_0080:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate5f'
    IL_0085:  ldtoken    [mscorlib]System.Int32
    IL_008a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008f:  ldstr      "a"
    IL_0094:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0099:  stloc.3
    IL_009a:  ldtoken    [mscorlib]System.Int32
    IL_009f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a4:  ldstr      "b"
    IL_00a9:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00ae:  stloc.s    V_4
    IL_00b0:  ldloc.3
    IL_00b1:  ldloc.s    V_4
    IL_00b3:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Subtract(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00b8:  ldc.i4.2
    IL_00b9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00be:  stloc.s    V_5
    IL_00c0:  ldloc.s    V_5
    IL_00c2:  ldc.i4.0
    IL_00c3:  ldloc.3
    IL_00c4:  stelem.ref
    IL_00c5:  ldloc.s    V_5
    IL_00c7:  ldc.i4.1
    IL_00c8:  ldloc.s    V_4
    IL_00ca:  stelem.ref
    IL_00cb:  ldloc.s    V_5
    IL_00cd:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00d2:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00d7:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate60'
    IL_00dc:  brtrue.s   IL_00ef

    IL_00de:  ldnull
    IL_00df:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__51'(int32,
                                                                                                                                 int32)
    IL_00e5:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_00ea:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate60'
    IL_00ef:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate60'
    IL_00f4:  ldtoken    [mscorlib]System.Int32
    IL_00f9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fe:  ldstr      "a"
    IL_0103:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0108:  stloc.s    V_6
    IL_010a:  ldtoken    [mscorlib]System.Int32
    IL_010f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0114:  ldstr      "b"
    IL_0119:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_011e:  stloc.s    V_7
    IL_0120:  ldloc.s    V_6
    IL_0122:  ldloc.s    V_7
    IL_0124:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0129:  ldc.i4.2
    IL_012a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_012f:  stloc.s    V_8
    IL_0131:  ldloc.s    V_8
    IL_0133:  ldc.i4.0
    IL_0134:  ldloc.s    V_6
    IL_0136:  stelem.ref
    IL_0137:  ldloc.s    V_8
    IL_0139:  ldc.i4.1
    IL_013a:  ldloc.s    V_7
    IL_013c:  stelem.ref
    IL_013d:  ldloc.s    V_8
    IL_013f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0144:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0149:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate61'
    IL_014e:  brtrue.s   IL_0161

    IL_0150:  ldnull
    IL_0151:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__52'(int32,
                                                                                                                                 int32)
    IL_0157:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_015c:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate61'
    IL_0161:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate61'
    IL_0166:  ldtoken    [mscorlib]System.Int32
    IL_016b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0170:  ldstr      "a"
    IL_0175:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_017a:  stloc.s    V_9
    IL_017c:  ldtoken    [mscorlib]System.Int32
    IL_0181:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0186:  ldstr      "b"
    IL_018b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0190:  stloc.s    V_10
    IL_0192:  ldloc.s    V_9
    IL_0194:  ldloc.s    V_10
    IL_0196:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Divide(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_019b:  ldc.i4.2
    IL_019c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01a1:  stloc.s    V_11
    IL_01a3:  ldloc.s    V_11
    IL_01a5:  ldc.i4.0
    IL_01a6:  ldloc.s    V_9
    IL_01a8:  stelem.ref
    IL_01a9:  ldloc.s    V_11
    IL_01ab:  ldc.i4.1
    IL_01ac:  ldloc.s    V_10
    IL_01ae:  stelem.ref
    IL_01af:  ldloc.s    V_11
    IL_01b1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01b6:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01bb:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate62'
    IL_01c0:  brtrue.s   IL_01d3

    IL_01c2:  ldnull
    IL_01c3:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__53'(int32,
                                                                                                                                 int32)
    IL_01c9:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_01ce:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate62'
    IL_01d3:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate62'
    IL_01d8:  ldtoken    [mscorlib]System.Int32
    IL_01dd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01e2:  ldstr      "a"
    IL_01e7:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01ec:  stloc.s    V_12
    IL_01ee:  ldtoken    [mscorlib]System.Int32
    IL_01f3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f8:  ldstr      "b"
    IL_01fd:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0202:  stloc.s    V_13
    IL_0204:  ldloc.s    V_12
    IL_0206:  ldloc.s    V_13
    IL_0208:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_020d:  ldc.i4.2
    IL_020e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0213:  stloc.s    V_14
    IL_0215:  ldloc.s    V_14
    IL_0217:  ldc.i4.0
    IL_0218:  ldloc.s    V_12
    IL_021a:  stelem.ref
    IL_021b:  ldloc.s    V_14
    IL_021d:  ldc.i4.1
    IL_021e:  ldloc.s    V_13
    IL_0220:  stelem.ref
    IL_0221:  ldloc.s    V_14
    IL_0223:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0228:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_022d:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate63'
    IL_0232:  brtrue.s   IL_0245

    IL_0234:  ldnull
    IL_0235:  ldftn      int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__54'(int64,
                                                                                                                                 int32)
    IL_023b:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_0240:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate63'
    IL_0245:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate63'
    IL_024a:  ldtoken    [mscorlib]System.Int64
    IL_024f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0254:  ldstr      "a"
    IL_0259:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_025e:  stloc.s    V_15
    IL_0260:  ldtoken    [mscorlib]System.Int32
    IL_0265:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_026a:  ldstr      "b"
    IL_026f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0274:  stloc.s    V_16
    IL_0276:  ldloc.s    V_15
    IL_0278:  ldloc.s    V_16
    IL_027a:  ldtoken    [mscorlib]System.Int64
    IL_027f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0284:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0289:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_028e:  ldc.i4.2
    IL_028f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0294:  stloc.s    V_17
    IL_0296:  ldloc.s    V_17
    IL_0298:  ldc.i4.0
    IL_0299:  ldloc.s    V_15
    IL_029b:  stelem.ref
    IL_029c:  ldloc.s    V_17
    IL_029e:  ldc.i4.1
    IL_029f:  ldloc.s    V_16
    IL_02a1:  stelem.ref
    IL_02a2:  ldloc.s    V_17
    IL_02a4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_02a9:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_02ae:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate64'
    IL_02b3:  brtrue.s   IL_02c6

    IL_02b5:  ldnull
    IL_02b6:  ldftn      int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__55'(int64,
                                                                                                                                 int32)
    IL_02bc:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_02c1:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate64'
    IL_02c6:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate64'
    IL_02cb:  ldtoken    [mscorlib]System.Int64
    IL_02d0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02d5:  ldstr      "a"
    IL_02da:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02df:  stloc.s    V_18
    IL_02e1:  ldtoken    [mscorlib]System.Int32
    IL_02e6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02eb:  ldstr      "b"
    IL_02f0:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02f5:  stloc.s    V_19
    IL_02f7:  ldloc.s    V_18
    IL_02f9:  ldloc.s    V_19
    IL_02fb:  ldtoken    [mscorlib]System.Int64
    IL_0300:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0305:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_030a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Subtract(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_030f:  ldc.i4.2
    IL_0310:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0315:  stloc.s    V_20
    IL_0317:  ldloc.s    V_20
    IL_0319:  ldc.i4.0
    IL_031a:  ldloc.s    V_18
    IL_031c:  stelem.ref
    IL_031d:  ldloc.s    V_20
    IL_031f:  ldc.i4.1
    IL_0320:  ldloc.s    V_19
    IL_0322:  stelem.ref
    IL_0323:  ldloc.s    V_20
    IL_0325:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_032a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_032f:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate65'
    IL_0334:  brtrue.s   IL_0347

    IL_0336:  ldnull
    IL_0337:  ldftn      int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__56'(int64,
                                                                                                                                 int32)
    IL_033d:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_0342:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate65'
    IL_0347:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate65'
    IL_034c:  ldtoken    [mscorlib]System.Int64
    IL_0351:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0356:  ldstr      "a"
    IL_035b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0360:  stloc.s    V_21
    IL_0362:  ldtoken    [mscorlib]System.Int32
    IL_0367:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_036c:  ldstr      "b"
    IL_0371:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0376:  stloc.s    V_22
    IL_0378:  ldloc.s    V_21
    IL_037a:  ldloc.s    V_22
    IL_037c:  ldtoken    [mscorlib]System.Int64
    IL_0381:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0386:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_038b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0390:  ldc.i4.2
    IL_0391:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0396:  stloc.s    V_23
    IL_0398:  ldloc.s    V_23
    IL_039a:  ldc.i4.0
    IL_039b:  ldloc.s    V_21
    IL_039d:  stelem.ref
    IL_039e:  ldloc.s    V_23
    IL_03a0:  ldc.i4.1
    IL_03a1:  ldloc.s    V_22
    IL_03a3:  stelem.ref
    IL_03a4:  ldloc.s    V_23
    IL_03a6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03ab:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_03b0:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate66'
    IL_03b5:  brtrue.s   IL_03c8

    IL_03b7:  ldnull
    IL_03b8:  ldftn      int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__57'(int64,
                                                                                                                                 int32)
    IL_03be:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_03c3:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate66'
    IL_03c8:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate66'
    IL_03cd:  ldtoken    [mscorlib]System.Int64
    IL_03d2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03d7:  ldstr      "a"
    IL_03dc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03e1:  stloc.s    V_24
    IL_03e3:  ldtoken    [mscorlib]System.Int32
    IL_03e8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03ed:  ldstr      "b"
    IL_03f2:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03f7:  stloc.s    V_25
    IL_03f9:  ldloc.s    V_24
    IL_03fb:  ldloc.s    V_25
    IL_03fd:  ldtoken    [mscorlib]System.Int64
    IL_0402:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0407:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_040c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Divide(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_0411:  ldc.i4.2
    IL_0412:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0417:  stloc.s    V_26
    IL_0419:  ldloc.s    V_26
    IL_041b:  ldc.i4.0
    IL_041c:  ldloc.s    V_24
    IL_041e:  stelem.ref
    IL_041f:  ldloc.s    V_26
    IL_0421:  ldc.i4.1
    IL_0422:  ldloc.s    V_25
    IL_0424:  stelem.ref
    IL_0425:  ldloc.s    V_26
    IL_0427:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_042c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0431:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate67'
    IL_0436:  brtrue.s   IL_0449

    IL_0438:  ldnull
    IL_0439:  ldftn      int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__58'(int64,
                                                                                                                                 int32)
    IL_043f:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_0444:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate67'
    IL_0449:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate67'
    IL_044e:  ldtoken    [mscorlib]System.Int64
    IL_0453:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0458:  ldstr      "a"
    IL_045d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0462:  stloc.s    V_27
    IL_0464:  ldtoken    [mscorlib]System.Int32
    IL_0469:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_046e:  ldstr      "b"
    IL_0473:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0478:  stloc.s    V_28
    IL_047a:  ldloc.s    V_27
    IL_047c:  ldloc.s    V_28
    IL_047e:  ldtoken    [mscorlib]System.Int64
    IL_0483:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0488:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_048d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_0492:  ldc.i4.2
    IL_0493:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0498:  stloc.s    V_29
    IL_049a:  ldloc.s    V_29
    IL_049c:  ldc.i4.0
    IL_049d:  ldloc.s    V_27
    IL_049f:  stelem.ref
    IL_04a0:  ldloc.s    V_29
    IL_04a2:  ldc.i4.1
    IL_04a3:  ldloc.s    V_28
    IL_04a5:  stelem.ref
    IL_04a6:  ldloc.s    V_29
    IL_04a8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_04ad:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_04b2:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate68'
    IL_04b7:  brtrue.s   IL_04ca

    IL_04b9:  ldnull
    IL_04ba:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__59'(int16,
                                                                                                                                 int32)
    IL_04c0:  newobj     instance void class [mscorlib]System.Func`3<int16,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_04c5:  stsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate68'
    IL_04ca:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate68'
    IL_04cf:  ldtoken    [mscorlib]System.Int16
    IL_04d4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04d9:  ldstr      "a"
    IL_04de:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04e3:  stloc.s    V_30
    IL_04e5:  ldtoken    [mscorlib]System.Int32
    IL_04ea:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04ef:  ldstr      "b"
    IL_04f4:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04f9:  stloc.s    V_31
    IL_04fb:  ldloc.s    V_30
    IL_04fd:  ldtoken    [mscorlib]System.Int32
    IL_0502:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0507:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_050c:  ldloc.s    V_31
    IL_050e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0513:  ldc.i4.2
    IL_0514:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0519:  stloc.s    V_32
    IL_051b:  ldloc.s    V_32
    IL_051d:  ldc.i4.0
    IL_051e:  ldloc.s    V_30
    IL_0520:  stelem.ref
    IL_0521:  ldloc.s    V_32
    IL_0523:  ldc.i4.1
    IL_0524:  ldloc.s    V_31
    IL_0526:  stelem.ref
    IL_0527:  ldloc.s    V_32
    IL_0529:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_052e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int16,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0533:  ldsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate69'
    IL_0538:  brtrue.s   IL_054b

    IL_053a:  ldnull
    IL_053b:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__5a'(int32,
                                                                                                                                 int16)
    IL_0541:  newobj     instance void class [mscorlib]System.Func`3<int32,int16,int32>::.ctor(object,
                                                                                               native int)
    IL_0546:  stsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate69'
    IL_054b:  ldsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate69'
    IL_0550:  ldtoken    [mscorlib]System.Int32
    IL_0555:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_055a:  ldstr      "a"
    IL_055f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0564:  stloc.s    V_33
    IL_0566:  ldtoken    [mscorlib]System.Int16
    IL_056b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0570:  ldstr      "b"
    IL_0575:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_057a:  stloc.s    V_34
    IL_057c:  ldloc.s    V_33
    IL_057e:  ldloc.s    V_34
    IL_0580:  ldtoken    [mscorlib]System.Int32
    IL_0585:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_058a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_058f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Subtract(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0594:  ldc.i4.2
    IL_0595:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_059a:  stloc.s    V_35
    IL_059c:  ldloc.s    V_35
    IL_059e:  ldc.i4.0
    IL_059f:  ldloc.s    V_33
    IL_05a1:  stelem.ref
    IL_05a2:  ldloc.s    V_35
    IL_05a4:  ldc.i4.1
    IL_05a5:  ldloc.s    V_34
    IL_05a7:  stelem.ref
    IL_05a8:  ldloc.s    V_35
    IL_05aa:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int16,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_05af:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int16,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_05b4:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6a'
    IL_05b9:  brtrue.s   IL_05cc

    IL_05bb:  ldnull
    IL_05bc:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__5b'(int16,
                                                                                                                                 int32)
    IL_05c2:  newobj     instance void class [mscorlib]System.Func`3<int16,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_05c7:  stsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6a'
    IL_05cc:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6a'
    IL_05d1:  ldtoken    [mscorlib]System.Int16
    IL_05d6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05db:  ldstr      "a"
    IL_05e0:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_05e5:  stloc.s    V_36
    IL_05e7:  ldtoken    [mscorlib]System.Int32
    IL_05ec:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05f1:  ldstr      "b"
    IL_05f6:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_05fb:  stloc.s    V_37
    IL_05fd:  ldloc.s    V_36
    IL_05ff:  ldtoken    [mscorlib]System.Int32
    IL_0604:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0609:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_060e:  ldloc.s    V_37
    IL_0610:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0615:  ldc.i4.2
    IL_0616:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_061b:  stloc.s    V_38
    IL_061d:  ldloc.s    V_38
    IL_061f:  ldc.i4.0
    IL_0620:  ldloc.s    V_36
    IL_0622:  stelem.ref
    IL_0623:  ldloc.s    V_38
    IL_0625:  ldc.i4.1
    IL_0626:  ldloc.s    V_37
    IL_0628:  stelem.ref
    IL_0629:  ldloc.s    V_38
    IL_062b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0630:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int16,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0635:  ldsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6b'
    IL_063a:  brtrue.s   IL_064d

    IL_063c:  ldnull
    IL_063d:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__5c'(int32,
                                                                                                                                 int16)
    IL_0643:  newobj     instance void class [mscorlib]System.Func`3<int32,int16,int32>::.ctor(object,
                                                                                               native int)
    IL_0648:  stsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6b'
    IL_064d:  ldsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6b'
    IL_0652:  ldtoken    [mscorlib]System.Int32
    IL_0657:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_065c:  ldstr      "a"
    IL_0661:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0666:  stloc.s    V_39
    IL_0668:  ldtoken    [mscorlib]System.Int16
    IL_066d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0672:  ldstr      "b"
    IL_0677:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_067c:  stloc.s    V_40
    IL_067e:  ldloc.s    V_39
    IL_0680:  ldloc.s    V_40
    IL_0682:  ldtoken    [mscorlib]System.Int32
    IL_0687:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_068c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0691:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Divide(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_0696:  ldc.i4.2
    IL_0697:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_069c:  stloc.s    V_41
    IL_069e:  ldloc.s    V_41
    IL_06a0:  ldc.i4.0
    IL_06a1:  ldloc.s    V_39
    IL_06a3:  stelem.ref
    IL_06a4:  ldloc.s    V_41
    IL_06a6:  ldc.i4.1
    IL_06a7:  ldloc.s    V_40
    IL_06a9:  stelem.ref
    IL_06aa:  ldloc.s    V_41
    IL_06ac:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int16,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_06b1:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int16,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_06b6:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6c'
    IL_06bb:  brtrue.s   IL_06ce

    IL_06bd:  ldnull
    IL_06be:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__5d'(int16,
                                                                                                                                 int32)
    IL_06c4:  newobj     instance void class [mscorlib]System.Func`3<int16,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_06c9:  stsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6c'
    IL_06ce:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6c'
    IL_06d3:  ldtoken    [mscorlib]System.Int16
    IL_06d8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_06dd:  ldstr      "a"
    IL_06e2:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_06e7:  stloc.s    V_42
    IL_06e9:  ldtoken    [mscorlib]System.Int32
    IL_06ee:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_06f3:  ldstr      "b"
    IL_06f8:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_06fd:  stloc.s    V_43
    IL_06ff:  ldloc.s    V_42
    IL_0701:  ldtoken    [mscorlib]System.Int32
    IL_0706:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_070b:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0710:  ldloc.s    V_43
    IL_0712:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_0717:  ldc.i4.2
    IL_0718:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_071d:  stloc.s    V_44
    IL_071f:  ldloc.s    V_44
    IL_0721:  ldc.i4.0
    IL_0722:  ldloc.s    V_42
    IL_0724:  stelem.ref
    IL_0725:  ldloc.s    V_44
    IL_0727:  ldc.i4.1
    IL_0728:  ldloc.s    V_43
    IL_072a:  stelem.ref
    IL_072b:  ldloc.s    V_44
    IL_072d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0732:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int16,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0737:  ret
  } // end of method ExpressionTrees::BinaryArithmeticOperators

  .method public hidebysig static void  BitOperators() cil managed
  {
    // Code size       415 (0x19f)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_5,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_6,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_7,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_8,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_9,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_10)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate71'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BitOperators>b__6d'(int32)
    IL_000e:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate71'
    IL_0018:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate71'
    IL_001d:  ldtoken    [mscorlib]System.Int32
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  ldstr      "a"
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0031:  stloc.0
    IL_0032:  ldloc.0
    IL_0033:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0038:  ldc.i4.1
    IL_0039:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003e:  stloc.1
    IL_003f:  ldloc.1
    IL_0040:  ldc.i4.0
    IL_0041:  ldloc.0
    IL_0042:  stelem.ref
    IL_0043:  ldloc.1
    IL_0044:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0049:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_004e:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate72'
    IL_0053:  brtrue.s   IL_0066

    IL_0055:  ldnull
    IL_0056:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BitOperators>b__6e'(int32,
                                                                                                                    int32)
    IL_005c:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0061:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate72'
    IL_0066:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate72'
    IL_006b:  ldtoken    [mscorlib]System.Int32
    IL_0070:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0075:  ldstr      "a"
    IL_007a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_007f:  stloc.2
    IL_0080:  ldtoken    [mscorlib]System.Int32
    IL_0085:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008a:  ldstr      "b"
    IL_008f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0094:  stloc.3
    IL_0095:  ldloc.2
    IL_0096:  ldloc.3
    IL_0097:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::And(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_009c:  ldc.i4.2
    IL_009d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a2:  stloc.s    V_4
    IL_00a4:  ldloc.s    V_4
    IL_00a6:  ldc.i4.0
    IL_00a7:  ldloc.2
    IL_00a8:  stelem.ref
    IL_00a9:  ldloc.s    V_4
    IL_00ab:  ldc.i4.1
    IL_00ac:  ldloc.3
    IL_00ad:  stelem.ref
    IL_00ae:  ldloc.s    V_4
    IL_00b0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b5:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00ba:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate73'
    IL_00bf:  brtrue.s   IL_00d2

    IL_00c1:  ldnull
    IL_00c2:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BitOperators>b__6f'(int32,
                                                                                                                    int32)
    IL_00c8:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_00cd:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate73'
    IL_00d2:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate73'
    IL_00d7:  ldtoken    [mscorlib]System.Int32
    IL_00dc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e1:  ldstr      "a"
    IL_00e6:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00eb:  stloc.s    V_5
    IL_00ed:  ldtoken    [mscorlib]System.Int32
    IL_00f2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f7:  ldstr      "b"
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0101:  stloc.s    V_6
    IL_0103:  ldloc.s    V_5
    IL_0105:  ldloc.s    V_6
    IL_0107:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Or(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression)
    IL_010c:  ldc.i4.2
    IL_010d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0112:  stloc.s    V_7
    IL_0114:  ldloc.s    V_7
    IL_0116:  ldc.i4.0
    IL_0117:  ldloc.s    V_5
    IL_0119:  stelem.ref
    IL_011a:  ldloc.s    V_7
    IL_011c:  ldc.i4.1
    IL_011d:  ldloc.s    V_6
    IL_011f:  stelem.ref
    IL_0120:  ldloc.s    V_7
    IL_0122:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0127:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_012c:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate74'
    IL_0131:  brtrue.s   IL_0144

    IL_0133:  ldnull
    IL_0134:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BitOperators>b__70'(int32,
                                                                                                                    int32)
    IL_013a:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_013f:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate74'
    IL_0144:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate74'
    IL_0149:  ldtoken    [mscorlib]System.Int32
    IL_014e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0153:  ldstr      "a"
    IL_0158:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_015d:  stloc.s    V_8
    IL_015f:  ldtoken    [mscorlib]System.Int32
    IL_0164:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0169:  ldstr      "b"
    IL_016e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0173:  stloc.s    V_9
    IL_0175:  ldloc.s    V_8
    IL_0177:  ldloc.s    V_9
    IL_0179:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ExclusiveOr(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_017e:  ldc.i4.2
    IL_017f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0184:  stloc.s    V_10
    IL_0186:  ldloc.s    V_10
    IL_0188:  ldc.i4.0
    IL_0189:  ldloc.s    V_8
    IL_018b:  stelem.ref
    IL_018c:  ldloc.s    V_10
    IL_018e:  ldc.i4.1
    IL_018f:  ldloc.s    V_9
    IL_0191:  stelem.ref
    IL_0192:  ldloc.s    V_10
    IL_0194:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0199:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_019e:  ret
  } // end of method ExpressionTrees::BitOperators

  .method public hidebysig static void  ShiftOperators() cil managed
  {
    // Code size       409 (0x199)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_5,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_6,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_7)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate79'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ShiftOperators>b__75'(int32)
    IL_000e:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate79'
    IL_0018:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate79'
    IL_001d:  ldtoken    [mscorlib]System.Int32
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  ldstr      "a"
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0031:  stloc.0
    IL_0032:  ldloc.0
    IL_0033:  ldc.i4.2
    IL_0034:  box        [mscorlib]System.Int32
    IL_0039:  ldtoken    [mscorlib]System.Int32
    IL_003e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0043:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::RightShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_004d:  ldc.i4.1
    IL_004e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0053:  stloc.1
    IL_0054:  ldloc.1
    IL_0055:  ldc.i4.0
    IL_0056:  ldloc.0
    IL_0057:  stelem.ref
    IL_0058:  ldloc.1
    IL_0059:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0063:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7a'
    IL_0068:  brtrue.s   IL_007b

    IL_006a:  ldnull
    IL_006b:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ShiftOperators>b__76'(int32)
    IL_0071:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0076:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7a'
    IL_007b:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7a'
    IL_0080:  ldtoken    [mscorlib]System.Int32
    IL_0085:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008a:  ldstr      "a"
    IL_008f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0094:  stloc.2
    IL_0095:  ldloc.2
    IL_0096:  ldc.i4.2
    IL_0097:  box        [mscorlib]System.Int32
    IL_009c:  ldtoken    [mscorlib]System.Int32
    IL_00a1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00ab:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LeftShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression)
    IL_00b0:  ldc.i4.1
    IL_00b1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b6:  stloc.3
    IL_00b7:  ldloc.3
    IL_00b8:  ldc.i4.0
    IL_00b9:  ldloc.2
    IL_00ba:  stelem.ref
    IL_00bb:  ldloc.3
    IL_00bc:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00c1:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00c6:  ldsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7b'
    IL_00cb:  brtrue.s   IL_00de

    IL_00cd:  ldnull
    IL_00ce:  ldftn      int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ShiftOperators>b__77'(int64)
    IL_00d4:  newobj     instance void class [mscorlib]System.Func`2<int64,int64>::.ctor(object,
                                                                                         native int)
    IL_00d9:  stsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7b'
    IL_00de:  ldsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7b'
    IL_00e3:  ldtoken    [mscorlib]System.Int64
    IL_00e8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ed:  ldstr      "a"
    IL_00f2:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00f7:  stloc.s    V_4
    IL_00f9:  ldloc.s    V_4
    IL_00fb:  ldc.i4.2
    IL_00fc:  box        [mscorlib]System.Int32
    IL_0101:  ldtoken    [mscorlib]System.Int32
    IL_0106:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0110:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::RightShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_0115:  ldc.i4.1
    IL_0116:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_011b:  stloc.s    V_5
    IL_011d:  ldloc.s    V_5
    IL_011f:  ldc.i4.0
    IL_0120:  ldloc.s    V_4
    IL_0122:  stelem.ref
    IL_0123:  ldloc.s    V_5
    IL_0125:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int64,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_012a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int64,int64>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_012f:  ldsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7c'
    IL_0134:  brtrue.s   IL_0147

    IL_0136:  ldnull
    IL_0137:  ldftn      int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ShiftOperators>b__78'(int64)
    IL_013d:  newobj     instance void class [mscorlib]System.Func`2<int64,int64>::.ctor(object,
                                                                                         native int)
    IL_0142:  stsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7c'
    IL_0147:  ldsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7c'
    IL_014c:  ldtoken    [mscorlib]System.Int64
    IL_0151:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0156:  ldstr      "a"
    IL_015b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0160:  stloc.s    V_6
    IL_0162:  ldloc.s    V_6
    IL_0164:  ldc.i4.2
    IL_0165:  box        [mscorlib]System.Int32
    IL_016a:  ldtoken    [mscorlib]System.Int32
    IL_016f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0174:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0179:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LeftShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression)
    IL_017e:  ldc.i4.1
    IL_017f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0184:  stloc.s    V_7
    IL_0186:  ldloc.s    V_7
    IL_0188:  ldc.i4.0
    IL_0189:  ldloc.s    V_6
    IL_018b:  stelem.ref
    IL_018c:  ldloc.s    V_7
    IL_018e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int64,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0193:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int64,int64>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0198:  ret
  } // end of method ExpressionTrees::ShiftOperators

  .method public hidebysig static void  SimpleExpressions() cil managed
  {
    // Code size       140 (0x8c)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7f'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<SimpleExpressions>b__7d'()
    IL_000e:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7f'
    IL_0018:  ldsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7f'
    IL_001d:  ldc.i4.0
    IL_001e:  box        [mscorlib]System.Int32
    IL_0023:  ldtoken    [mscorlib]System.Int32
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  ldc.i4.0
    IL_0033:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0038:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_003d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32>>(!!0,
                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0042:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate80'
    IL_0047:  brtrue.s   IL_005a

    IL_0049:  ldnull
    IL_004a:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<SimpleExpressions>b__7e'(int32)
    IL_0050:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0055:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate80'
    IL_005a:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate80'
    IL_005f:  ldtoken    [mscorlib]System.Int32
    IL_0064:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0069:  ldstr      "a"
    IL_006e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0073:  stloc.0
    IL_0074:  ldloc.0
    IL_0075:  ldc.i4.1
    IL_0076:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007b:  stloc.1
    IL_007c:  ldloc.1
    IL_007d:  ldc.i4.0
    IL_007e:  ldloc.0
    IL_007f:  stelem.ref
    IL_0080:  ldloc.1
    IL_0081:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0086:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_008b:  ret
  } // end of method ExpressionTrees::SimpleExpressions

  .method public hidebysig static void  Capturing() cil managed
  {
    // Code size       63 (0x3f)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass82' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass82'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.5
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass82'::captured
    IL_000d:  ldloc.0
    IL_000e:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass82'::'<Capturing>b__81'()
    IL_0014:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0019:  ldloc.0
    IL_001a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_001f:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass82'::captured
    IL_0024:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0029:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_002e:  ldc.i4.0
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0034:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0039:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32>>(!!0,
                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_003e:  ret
  } // end of method ExpressionTrees::Capturing

  .method public hidebysig static void  FieldAndPropertyAccess() cil managed
  {
    // Code size       452 (0x1c4)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_5,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_6,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_7)
    IL_0000:  ldnull
    IL_0001:  ldc.i4.1
    IL_0002:  box        [mscorlib]System.Int32
    IL_0007:  ldtoken    [mscorlib]System.Int32
    IL_000c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0011:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0016:  ldc.i4.0
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_001c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0021:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0026:  pop
    IL_0027:  ldnull
    IL_0028:  ldnull
    IL_0029:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::StaticField
    IL_002e:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0038:  ldc.i4.0
    IL_0039:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0043:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0048:  pop
    IL_0049:  ldnull
    IL_004a:  ldnull
    IL_004b:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::StaticReadonlyField
    IL_0050:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0055:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_005a:  ldc.i4.0
    IL_005b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0060:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0065:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006a:  pop
    IL_006b:  ldnull
    IL_006c:  ldnull
    IL_006d:  ldtoken    method int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_StaticProperty()
    IL_0072:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0077:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0081:  ldc.i4.0
    IL_0082:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0087:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0091:  pop
    IL_0092:  ldnull
    IL_0093:  ldnull
    IL_0094:  ldtoken    method int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_StaticReadonlyProperty()
    IL_0099:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_009e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a3:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00a8:  ldc.i4.0
    IL_00a9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00ae:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b3:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b8:  pop
    IL_00b9:  ldnull
    IL_00ba:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_00bf:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c4:  ldstr      "a"
    IL_00c9:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00ce:  stloc.0
    IL_00cf:  ldloc.0
    IL_00d0:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::Field
    IL_00d5:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00da:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00df:  ldc.i4.1
    IL_00e0:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e5:  stloc.1
    IL_00e6:  ldloc.1
    IL_00e7:  ldc.i4.0
    IL_00e8:  ldloc.0
    IL_00e9:  stelem.ref
    IL_00ea:  ldloc.1
    IL_00eb:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00f0:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_00f5:  pop
    IL_00f6:  ldnull
    IL_00f7:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_00fc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0101:  ldstr      "a"
    IL_0106:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_010b:  stloc.2
    IL_010c:  ldloc.2
    IL_010d:  ldtoken    method instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_Property()
    IL_0112:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0117:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_011c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0121:  ldc.i4.1
    IL_0122:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0127:  stloc.3
    IL_0128:  ldloc.3
    IL_0129:  ldc.i4.0
    IL_012a:  ldloc.2
    IL_012b:  stelem.ref
    IL_012c:  ldloc.3
    IL_012d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0132:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_0137:  pop
    IL_0138:  ldnull
    IL_0139:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_013e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0143:  ldstr      "a"
    IL_0148:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_014d:  stloc.s    V_4
    IL_014f:  ldloc.s    V_4
    IL_0151:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::ReadonlyField
    IL_0156:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_015b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0160:  ldc.i4.1
    IL_0161:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0166:  stloc.s    V_5
    IL_0168:  ldloc.s    V_5
    IL_016a:  ldc.i4.0
    IL_016b:  ldloc.s    V_4
    IL_016d:  stelem.ref
    IL_016e:  ldloc.s    V_5
    IL_0170:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0175:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_017a:  pop
    IL_017b:  ldnull
    IL_017c:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_0181:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0186:  ldstr      "a"
    IL_018b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0190:  stloc.s    V_6
    IL_0192:  ldloc.s    V_6
    IL_0194:  ldtoken    method instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_ReadonlyProperty()
    IL_0199:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_019e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01a3:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_01a8:  ldc.i4.1
    IL_01a9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01ae:  stloc.s    V_7
    IL_01b0:  ldloc.s    V_7
    IL_01b2:  ldc.i4.0
    IL_01b3:  ldloc.s    V_6
    IL_01b5:  stelem.ref
    IL_01b6:  ldloc.s    V_7
    IL_01b8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01bd:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_01c2:  pop
    IL_01c3:  ret
  } // end of method ExpressionTrees::FieldAndPropertyAccess

  .method public hidebysig static void  Call() cil managed
  {
    // Code size       549 (0x225)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_5,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_6,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_7,
             class [System.Core]System.Linq.Expressions.Expression[] V_8,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_9,
             class [System.Core]System.Linq.Expressions.Expression[] V_10)
    IL_0000:  ldnull
    IL_0001:  ldtoken    [mscorlib]System.String
    IL_0006:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000b:  ldstr      "a"
    IL_0010:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0015:  stloc.0
    IL_0016:  ldnull
    IL_0017:  ldtoken    method void [mscorlib]System.Console::WriteLine(string)
    IL_001c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0021:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0026:  ldc.i4.1
    IL_0027:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002c:  stloc.1
    IL_002d:  ldloc.1
    IL_002e:  ldc.i4.0
    IL_002f:  ldloc.0
    IL_0030:  stelem.ref
    IL_0031:  ldloc.1
    IL_0032:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0037:  ldc.i4.1
    IL_0038:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003d:  stloc.2
    IL_003e:  ldloc.2
    IL_003f:  ldc.i4.0
    IL_0040:  ldloc.0
    IL_0041:  stelem.ref
    IL_0042:  ldloc.2
    IL_0043:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Action`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                       class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0048:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Action`1<!!0>>)
    IL_004d:  pop
    IL_004e:  ldsfld     class [mscorlib]System.Func`2<string,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate88'
    IL_0053:  brtrue.s   IL_0066

    IL_0055:  ldnull
    IL_0056:  ldftn      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<Call>b__84'(string)
    IL_005c:  newobj     instance void class [mscorlib]System.Func`2<string,string>::.ctor(object,
                                                                                           native int)
    IL_0061:  stsfld     class [mscorlib]System.Func`2<string,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate88'
    IL_0066:  ldsfld     class [mscorlib]System.Func`2<string,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate88'
    IL_006b:  ldtoken    [mscorlib]System.String
    IL_0070:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0075:  ldstr      "a"
    IL_007a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_007f:  stloc.3
    IL_0080:  ldloc.3
    IL_0081:  ldtoken    method instance string [mscorlib]System.Object::ToString()
    IL_0086:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_008b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0090:  ldc.i4.0
    IL_0091:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0096:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_009b:  ldc.i4.1
    IL_009c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a1:  stloc.s    V_4
    IL_00a3:  ldloc.s    V_4
    IL_00a5:  ldc.i4.0
    IL_00a6:  ldloc.3
    IL_00a7:  stelem.ref
    IL_00a8:  ldloc.s    V_4
    IL_00aa:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00af:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<string,string>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00b4:  ldsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate89'
    IL_00b9:  brtrue.s   IL_00cc

    IL_00bb:  ldnull
    IL_00bc:  ldftn      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<Call>b__85'(int32)
    IL_00c2:  newobj     instance void class [mscorlib]System.Func`2<int32,string>::.ctor(object,
                                                                                          native int)
    IL_00c7:  stsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate89'
    IL_00cc:  ldsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate89'
    IL_00d1:  ldtoken    [mscorlib]System.Int32
    IL_00d6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00db:  ldstr      "a"
    IL_00e0:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00e5:  stloc.s    V_5
    IL_00e7:  ldloc.s    V_5
    IL_00e9:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_00ee:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00f3:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00f8:  ldc.i4.0
    IL_00f9:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00fe:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0103:  ldc.i4.1
    IL_0104:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0109:  stloc.s    V_6
    IL_010b:  ldloc.s    V_6
    IL_010d:  ldc.i4.0
    IL_010e:  ldloc.s    V_5
    IL_0110:  stelem.ref
    IL_0111:  ldloc.s    V_6
    IL_0113:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                           class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0118:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,string>>(!!0,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_011d:  ldsfld     class [mscorlib]System.Func`2<string,char[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8a'
    IL_0122:  brtrue.s   IL_0135

    IL_0124:  ldnull
    IL_0125:  ldftn      char[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<Call>b__86'(string)
    IL_012b:  newobj     instance void class [mscorlib]System.Func`2<string,char[]>::.ctor(object,
                                                                                           native int)
    IL_0130:  stsfld     class [mscorlib]System.Func`2<string,char[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8a'
    IL_0135:  ldsfld     class [mscorlib]System.Func`2<string,char[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8a'
    IL_013a:  ldtoken    [mscorlib]System.String
    IL_013f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0144:  ldstr      "a"
    IL_0149:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_014e:  stloc.s    V_7
    IL_0150:  ldnull
    IL_0151:  ldtoken    method !!0[] [System.Core]System.Linq.Enumerable::ToArray<char>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0156:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_015b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0160:  ldc.i4.1
    IL_0161:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0166:  stloc.s    V_8
    IL_0168:  ldloc.s    V_8
    IL_016a:  ldc.i4.0
    IL_016b:  ldloc.s    V_7
    IL_016d:  stelem.ref
    IL_016e:  ldloc.s    V_8
    IL_0170:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0175:  ldc.i4.1
    IL_0176:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_017b:  stloc.s    V_9
    IL_017d:  ldloc.s    V_9
    IL_017f:  ldc.i4.0
    IL_0180:  ldloc.s    V_7
    IL_0182:  stelem.ref
    IL_0183:  ldloc.s    V_9
    IL_0185:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,char[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_018a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<string,char[]>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_018f:  ldsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8b'
    IL_0194:  brtrue.s   IL_01a7

    IL_0196:  ldnull
    IL_0197:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<Call>b__87'()
    IL_019d:  newobj     instance void class [mscorlib]System.Func`1<bool>::.ctor(object,
                                                                                  native int)
    IL_01a2:  stsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8b'
    IL_01a7:  ldsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8b'
    IL_01ac:  ldc.i4.s   97
    IL_01ae:  box        [mscorlib]System.Char
    IL_01b3:  ldtoken    [mscorlib]System.Char
    IL_01b8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01bd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01c2:  ldtoken    method instance int32 [mscorlib]System.Char::CompareTo(char)
    IL_01c7:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01cc:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01d1:  ldc.i4.1
    IL_01d2:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01d7:  stloc.s    V_10
    IL_01d9:  ldloc.s    V_10
    IL_01db:  ldc.i4.0
    IL_01dc:  ldc.i4.s   98
    IL_01de:  box        [mscorlib]System.Char
    IL_01e3:  ldtoken    [mscorlib]System.Char
    IL_01e8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ed:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01f2:  stelem.ref
    IL_01f3:  ldloc.s    V_10
    IL_01f5:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01fa:  ldc.i4.0
    IL_01fb:  box        [mscorlib]System.Int32
    IL_0200:  ldtoken    [mscorlib]System.Int32
    IL_0205:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_020a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_020f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0214:  ldc.i4.0
    IL_0215:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_021a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_021f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<bool>>(!!0,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0224:  ret
  } // end of method ExpressionTrees::Call

  .method public hidebysig static void  Quote() cil managed
  {
    // Code size       203 (0xcb)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2)
    IL_0000:  ldsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8d'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<Quote>b__8c'()
    IL_000e:  newobj     instance void class [mscorlib]System.Func`1<bool>::.ctor(object,
                                                                                  native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8d'
    IL_0018:  ldsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8d'
    IL_001d:  ldtoken    [mscorlib]System.Int32
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  ldstr      "n"
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0031:  stloc.0
    IL_0032:  ldtoken    [mscorlib]System.String
    IL_0037:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  ldstr      "s"
    IL_0041:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0046:  stloc.1
    IL_0047:  ldloc.1
    IL_0048:  ldloc.0
    IL_0049:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_004e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0053:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0058:  ldc.i4.0
    IL_0059:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_005e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0063:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0068:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_006d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0072:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0077:  ldc.i4.2
    IL_0078:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007d:  stloc.2
    IL_007e:  ldloc.2
    IL_007f:  ldc.i4.0
    IL_0080:  ldloc.0
    IL_0081:  stelem.ref
    IL_0082:  ldloc.2
    IL_0083:  ldc.i4.1
    IL_0084:  ldloc.1
    IL_0085:  stelem.ref
    IL_0086:  ldloc.2
    IL_0087:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_0091:  ldtoken    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<int32,string,string>>
    IL_0096:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009b:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00a0:  ldnull
    IL_00a1:  box        [mscorlib]System.Object
    IL_00a6:  ldtoken    [mscorlib]System.Object
    IL_00ab:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b5:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00ba:  ldc.i4.0
    IL_00bb:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00c0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00c5:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<bool>>(!!0,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00ca:  ret
  } // end of method ExpressionTrees::Quote

  .method public hidebysig static void  ArrayInitializer() cil managed
  {
    // Code size       615 (0x267)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3,
             class [System.Core]System.Linq.Expressions.Expression[] V_4,
             class [System.Core]System.Linq.Expressions.Expression[] V_5)
    IL_0000:  ldsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate93'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayInitializer>b__8e'()
    IL_000e:  newobj     instance void class [mscorlib]System.Func`1<int32[]>::.ctor(object,
                                                                                     native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate93'
    IL_0018:  ldsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate93'
    IL_001d:  ldtoken    [mscorlib]System.Int32
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  ldc.i4.3
    IL_0028:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002d:  stloc.0
    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.0
    IL_0030:  ldc.i4.1
    IL_0031:  box        [mscorlib]System.Int32
    IL_0036:  ldtoken    [mscorlib]System.Int32
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0045:  stelem.ref
    IL_0046:  ldloc.0
    IL_0047:  ldc.i4.1
    IL_0048:  ldc.i4.2
    IL_0049:  box        [mscorlib]System.Int32
    IL_004e:  ldtoken    [mscorlib]System.Int32
    IL_0053:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0058:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005d:  stelem.ref
    IL_005e:  ldloc.0
    IL_005f:  ldc.i4.2
    IL_0060:  ldc.i4.3
    IL_0061:  box        [mscorlib]System.Int32
    IL_0066:  ldtoken    [mscorlib]System.Int32
    IL_006b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0070:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0075:  stelem.ref
    IL_0076:  ldloc.0
    IL_0077:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_007c:  ldc.i4.0
    IL_007d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0082:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0087:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[]>>(!!0,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_008c:  ldsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate94'
    IL_0091:  brtrue.s   IL_00a4

    IL_0093:  ldnull
    IL_0094:  ldftn      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayInitializer>b__8f'()
    IL_009a:  newobj     instance void class [mscorlib]System.Func`1<int32[]>::.ctor(object,
                                                                                     native int)
    IL_009f:  stsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate94'
    IL_00a4:  ldsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate94'
    IL_00a9:  ldtoken    [mscorlib]System.Int32
    IL_00ae:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b3:  ldc.i4.1
    IL_00b4:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00b9:  stloc.1
    IL_00ba:  ldloc.1
    IL_00bb:  ldc.i4.0
    IL_00bc:  ldc.i4.3
    IL_00bd:  box        [mscorlib]System.Int32
    IL_00c2:  ldtoken    [mscorlib]System.Int32
    IL_00c7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d1:  stelem.ref
    IL_00d2:  ldloc.1
    IL_00d3:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_00d8:  ldc.i4.0
    IL_00d9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00de:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[]>>(!!0,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00e8:  ldsfld     class [mscorlib]System.Func`1<int32[0...,0...]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate95'
    IL_00ed:  brtrue.s   IL_0100

    IL_00ef:  ldnull
    IL_00f0:  ldftn      int32[0...,0...] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayInitializer>b__90'()
    IL_00f6:  newobj     instance void class [mscorlib]System.Func`1<int32[0...,0...]>::.ctor(object,
                                                                                              native int)
    IL_00fb:  stsfld     class [mscorlib]System.Func`1<int32[0...,0...]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate95'
    IL_0100:  ldsfld     class [mscorlib]System.Func`1<int32[0...,0...]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate95'
    IL_0105:  ldtoken    [mscorlib]System.Int32
    IL_010a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010f:  ldc.i4.2
    IL_0110:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0115:  stloc.2
    IL_0116:  ldloc.2
    IL_0117:  ldc.i4.0
    IL_0118:  ldc.i4.3
    IL_0119:  box        [mscorlib]System.Int32
    IL_011e:  ldtoken    [mscorlib]System.Int32
    IL_0123:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0128:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_012d:  stelem.ref
    IL_012e:  ldloc.2
    IL_012f:  ldc.i4.1
    IL_0130:  ldc.i4.5
    IL_0131:  box        [mscorlib]System.Int32
    IL_0136:  ldtoken    [mscorlib]System.Int32
    IL_013b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0140:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0145:  stelem.ref
    IL_0146:  ldloc.2
    IL_0147:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_014c:  ldc.i4.0
    IL_014d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0152:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[0...,0...]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0157:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[0...,0...]>>(!!0,
                                                                                                                                                   class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_015c:  ldsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate96'
    IL_0161:  brtrue.s   IL_0174

    IL_0163:  ldnull
    IL_0164:  ldftn      int32[][] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayInitializer>b__91'()
    IL_016a:  newobj     instance void class [mscorlib]System.Func`1<int32[][]>::.ctor(object,
                                                                                       native int)
    IL_016f:  stsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate96'
    IL_0174:  ldsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate96'
    IL_0179:  ldtoken    int32[]
    IL_017e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0183:  ldc.i4.1
    IL_0184:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0189:  stloc.3
    IL_018a:  ldloc.3
    IL_018b:  ldc.i4.0
    IL_018c:  ldc.i4.3
    IL_018d:  box        [mscorlib]System.Int32
    IL_0192:  ldtoken    [mscorlib]System.Int32
    IL_0197:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_019c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01a1:  stelem.ref
    IL_01a2:  ldloc.3
    IL_01a3:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_01a8:  ldc.i4.0
    IL_01a9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01ae:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[][]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01b3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[][]>>(!!0,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01b8:  ldsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate97'
    IL_01bd:  brtrue.s   IL_01d0

    IL_01bf:  ldnull
    IL_01c0:  ldftn      int32[][] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayInitializer>b__92'()
    IL_01c6:  newobj     instance void class [mscorlib]System.Func`1<int32[][]>::.ctor(object,
                                                                                       native int)
    IL_01cb:  stsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate97'
    IL_01d0:  ldsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate97'
    IL_01d5:  ldtoken    int32[]
    IL_01da:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01df:  ldc.i4.1
    IL_01e0:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01e5:  stloc.s    V_4
    IL_01e7:  ldloc.s    V_4
    IL_01e9:  ldc.i4.0
    IL_01ea:  ldtoken    [mscorlib]System.Int32
    IL_01ef:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f4:  ldc.i4.3
    IL_01f5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01fa:  stloc.s    V_5
    IL_01fc:  ldloc.s    V_5
    IL_01fe:  ldc.i4.0
    IL_01ff:  ldc.i4.1
    IL_0200:  box        [mscorlib]System.Int32
    IL_0205:  ldtoken    [mscorlib]System.Int32
    IL_020a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_020f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0214:  stelem.ref
    IL_0215:  ldloc.s    V_5
    IL_0217:  ldc.i4.1
    IL_0218:  ldc.i4.2
    IL_0219:  box        [mscorlib]System.Int32
    IL_021e:  ldtoken    [mscorlib]System.Int32
    IL_0223:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0228:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_022d:  stelem.ref
    IL_022e:  ldloc.s    V_5
    IL_0230:  ldc.i4.2
    IL_0231:  ldc.i4.3
    IL_0232:  box        [mscorlib]System.Int32
    IL_0237:  ldtoken    [mscorlib]System.Int32
    IL_023c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0241:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0246:  stelem.ref
    IL_0247:  ldloc.s    V_5
    IL_0249:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_024e:  stelem.ref
    IL_024f:  ldloc.s    V_4
    IL_0251:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0256:  ldc.i4.0
    IL_0257:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_025c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[][]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0261:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[][]>>(!!0,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0266:  ret
  } // end of method ExpressionTrees::ArrayInitializer

  .method public hidebysig static void  AnonymousTypes() cil managed
  {
    // Code size       180 (0xb4)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [mscorlib]System.Reflection.MethodInfo[] V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate99'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<AnonymousTypes>b__98'()
    IL_000e:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_0013:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate99'
    IL_0018:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate99'
    IL_001d:  ldtoken    method instance void class '<>f__AnonymousType2`2'<int32,string>::.ctor(!0,
                                                                                                 !1)
    IL_0022:  ldtoken    class '<>f__AnonymousType2`2'<int32,string>
    IL_0027:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0031:  ldc.i4.2
    IL_0032:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i4.0
    IL_003a:  ldc.i4.5
    IL_003b:  box        [mscorlib]System.Int32
    IL_0040:  ldtoken    [mscorlib]System.Int32
    IL_0045:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004f:  stelem.ref
    IL_0050:  ldloc.0
    IL_0051:  ldc.i4.1
    IL_0052:  ldstr      "Test"
    IL_0057:  ldtoken    [mscorlib]System.String
    IL_005c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0061:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0066:  stelem.ref
    IL_0067:  ldloc.0
    IL_0068:  ldc.i4.2
    IL_0069:  newarr     [mscorlib]System.Reflection.MethodInfo
    IL_006e:  stloc.1
    IL_006f:  ldloc.1
    IL_0070:  ldc.i4.0
    IL_0071:  ldtoken    method instance !0 class '<>f__AnonymousType2`2'<int32,string>::get_A()
    IL_0076:  ldtoken    class '<>f__AnonymousType2`2'<int32,string>
    IL_007b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0080:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0085:  stelem.ref
    IL_0086:  ldloc.1
    IL_0087:  ldc.i4.1
    IL_0088:  ldtoken    method instance !1 class '<>f__AnonymousType2`2'<int32,string>::get_B()
    IL_008d:  ldtoken    class '<>f__AnonymousType2`2'<int32,string>
    IL_0092:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0097:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009c:  stelem.ref
    IL_009d:  ldloc.1
    IL_009e:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_00a3:  ldc.i4.0
    IL_00a4:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a9:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ae:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00b3:  ret
  } // end of method ExpressionTrees::AnonymousTypes

  .method public hidebysig static void  ObjectInit() cil managed
  {
    // Code size       141 (0x8d)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.MemberBinding[] V_0)
    IL_0000:  ldnull
    IL_0001:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::.ctor()
    IL_0006:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000b:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0010:  ldc.i4.0
    IL_0011:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0016:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_001b:  ldc.i4.2
    IL_001c:  newarr     [System.Core]System.Linq.Expressions.MemberBinding
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  ldc.i4.0
    IL_0024:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::set_Property(int32)
    IL_0029:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0033:  ldc.i4.4
    IL_0034:  box        [mscorlib]System.Int32
    IL_0039:  ldtoken    [mscorlib]System.Int32
    IL_003e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0043:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_004d:  stelem.ref
    IL_004e:  ldloc.0
    IL_004f:  ldc.i4.1
    IL_0050:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::Field
    IL_0055:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_005a:  ldc.i4.3
    IL_005b:  box        [mscorlib]System.Int32
    IL_0060:  ldtoken    [mscorlib]System.Int32
    IL_0065:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006f:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MemberInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_0074:  stelem.ref
    IL_0075:  ldloc.0
    IL_0076:  call       class [System.Core]System.Linq.Expressions.MemberInitExpression [System.Core]System.Linq.Expressions.Expression::MemberInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.MemberBinding[])
    IL_007b:  ldc.i4.0
    IL_007c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0081:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                             class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0086:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType>(object,
                                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008b:  pop
    IL_008c:  ret
  } // end of method ExpressionTrees::ObjectInit

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ExpressionTrees::.ctor

  .method private hidebysig static string 
          '<IndexerAccess>b__6'(int32 n) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarga.s   n
    IL_0002:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0007:  ret
  } // end of method ExpressionTrees::'<IndexerAccess>b__6'

  .method private hidebysig static bool  '<MethodGroupConstant>b__e'(class [mscorlib]System.Func`3<object,object,bool> f) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !2 class [mscorlib]System.Func`3<object,object,bool>::Invoke(!0,
                                                                                               !1)
    IL_0008:  ret
  } // end of method ExpressionTrees::'<MethodGroupConstant>b__e'

  .method private hidebysig static int32 
          '<NestedLambda>b__12'(class [mscorlib]System.Func`1<int32> f) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0006:  ret
  } // end of method ExpressionTrees::'<NestedLambda>b__12'

  .method private hidebysig static int32 
          '<ArrayIndexer>b__1d'(int32[] 'array') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldelem.i4
    IL_0003:  ret
  } // end of method ExpressionTrees::'<ArrayIndexer>b__1d'

  .method private hidebysig static int32 
          '<ArrayIndexer>b__1e'(int32[] 'array',
                                int32 index) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldelem.i4
    IL_0003:  ret
  } // end of method ExpressionTrees::'<ArrayIndexer>b__1e'

  .method private hidebysig static int32 
          '<ArrayIndexer>b__1f'(int32[0...,0...] 'array') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.5
    IL_0003:  call       instance int32 int32[0...,0...]::Get(int32,
                                                              int32)
    IL_0008:  ret
  } // end of method ExpressionTrees::'<ArrayIndexer>b__1f'

  .method private hidebysig static int32 
          '<ArrayIndexer>b__20'(int32[0...,0...] 'array',
                                int32 index) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.7
    IL_0003:  call       instance int32 int32[0...,0...]::Get(int32,
                                                              int32)
    IL_0008:  ret
  } // end of method ExpressionTrees::'<ArrayIndexer>b__20'

  .method private hidebysig static int32 
          '<ArrayIndexer>b__21'(int32[][] 'array',
                                int32 index) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldelem.ref
    IL_0003:  ldc.i4.7
    IL_0004:  ldelem.i4
    IL_0005:  ret
  } // end of method ExpressionTrees::'<ArrayIndexer>b__21'

  .method private hidebysig static int32 
          '<ArrayLength>b__27'(int32[] 'array') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldlen
    IL_0002:  conv.i4
    IL_0003:  ret
  } // end of method ExpressionTrees::'<ArrayLength>b__27'

  .method private hidebysig static int32 
          '<ArrayLength>b__28'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  callvirt   instance int32 [mscorlib]System.Array::get_Length()
    IL_0006:  ret
  } // end of method ExpressionTrees::'<ArrayLength>b__28'

  .method private hidebysig static object 
          '<NewObj>b__2b'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::.ctor()
    IL_0005:  ret
  } // end of method ExpressionTrees::'<NewObj>b__2b'

  .method private hidebysig static object 
          '<NewObj>b__2c'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldc.i4.5
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithCtor::.ctor(int32)
    IL_0006:  ret
  } // end of method ExpressionTrees::'<NewObj>b__2c'

  .method private hidebysig static object 
          '<NewObj>b__2d'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors::.ctor()
    IL_0005:  ret
  } // end of method ExpressionTrees::'<NewObj>b__2d'

  .method private hidebysig static object 
          '<NewObj>b__2e'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldc.i4.5
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors::.ctor(int32)
    IL_0006:  ret
  } // end of method ExpressionTrees::'<NewObj>b__2e'

  .method private hidebysig static object 
          '<NewObj>b__2f'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::.ctor()
    IL_0005:  ret
  } // end of method ExpressionTrees::'<NewObj>b__2f'

  .method private hidebysig static object 
          '<NewObj>b__30'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1<int32>::.ctor()
    IL_0005:  ret
  } // end of method ExpressionTrees::'<NewObj>b__30'

  .method private hidebysig static object 
          '<NewObj>b__31'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldc.i4.5
    IL_0001:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1<int32>::.ctor(int32)
    IL_0006:  ret
  } // end of method ExpressionTrees::'<NewObj>b__31'

  .method private hidebysig static class [mscorlib]System.Type 
          '<TypeOfExpr>b__39'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  8
    IL_0000:  ldtoken    [mscorlib]System.Int32
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  ret
  } // end of method ExpressionTrees::'<TypeOfExpr>b__39'

  .method private hidebysig static class [mscorlib]System.Type 
          '<TypeOfExpr>b__3a'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  8
    IL_0000:  ldtoken    [mscorlib]System.Object
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  ret
  } // end of method ExpressionTrees::'<TypeOfExpr>b__3a'

  .method private hidebysig static class [mscorlib]System.Type 
          '<TypeOfExpr>b__3b'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  8
    IL_0000:  ldtoken    [mscorlib]System.Collections.Generic.List`1
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  ret
  } // end of method ExpressionTrees::'<TypeOfExpr>b__3b'

  .method private hidebysig static class [mscorlib]System.Type 
          '<TypeOfExpr>b__3c'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  8
    IL_0000:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  ret
  } // end of method ExpressionTrees::'<TypeOfExpr>b__3c'

  .method private hidebysig static class [mscorlib]System.Type 
          '<TypeOfExpr>b__3d'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  8
    IL_0000:  ldtoken    int32*
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  ret
  } // end of method ExpressionTrees::'<TypeOfExpr>b__3d'

  .method private hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass 
          '<AsTypeExpr>b__43'(object obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  isinst     ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
    IL_0006:  ret
  } // end of method ExpressionTrees::'<AsTypeExpr>b__43'

  .method private hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object> 
          '<AsTypeExpr>b__44'(object obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  isinst     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>
    IL_0006:  ret
  } // end of method ExpressionTrees::'<AsTypeExpr>b__44'

  .method private hidebysig static bool  '<IsTypeExpr>b__47'(object obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  isinst     ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
    IL_0006:  ldnull
    IL_0007:  cgt.un
    IL_0009:  ret
  } // end of method ExpressionTrees::'<IsTypeExpr>b__47'

  .method private hidebysig static bool  '<UnaryLogicalOperators>b__49'(bool a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ceq
    IL_0004:  ret
  } // end of method ExpressionTrees::'<UnaryLogicalOperators>b__49'

  .method private hidebysig static int32 
          '<UnaryArithmeticOperators>b__4b'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  } // end of method ExpressionTrees::'<UnaryArithmeticOperators>b__4b'

  .method private hidebysig static int32 
          '<UnaryArithmeticOperators>b__4c'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  neg
    IL_0002:  ret
  } // end of method ExpressionTrees::'<UnaryArithmeticOperators>b__4c'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__4f'(int32 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__4f'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__50'(int32 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  sub
    IL_0003:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__50'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__51'(int32 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  mul
    IL_0003:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__51'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__52'(int32 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  div
    IL_0003:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__52'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__53'(int32 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  rem
    IL_0003:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__53'

  .method private hidebysig static int64 
          '<BinaryArithmeticOperators>b__54'(int64 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  conv.i8
    IL_0003:  add
    IL_0004:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__54'

  .method private hidebysig static int64 
          '<BinaryArithmeticOperators>b__55'(int64 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  conv.i8
    IL_0003:  sub
    IL_0004:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__55'

  .method private hidebysig static int64 
          '<BinaryArithmeticOperators>b__56'(int64 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  conv.i8
    IL_0003:  mul
    IL_0004:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__56'

  .method private hidebysig static int64 
          '<BinaryArithmeticOperators>b__57'(int64 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  conv.i8
    IL_0003:  div
    IL_0004:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__57'

  .method private hidebysig static int64 
          '<BinaryArithmeticOperators>b__58'(int64 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  conv.i8
    IL_0003:  rem
    IL_0004:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__58'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__59'(int16 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__59'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__5a'(int32 a,
                                             int16 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  sub
    IL_0003:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__5a'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__5b'(int16 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  mul
    IL_0003:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__5b'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__5c'(int32 a,
                                             int16 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  div
    IL_0003:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__5c'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__5d'(int16 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  rem
    IL_0003:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__5d'

  .method private hidebysig static int32 
          '<BitOperators>b__6d'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  not
    IL_0002:  ret
  } // end of method ExpressionTrees::'<BitOperators>b__6d'

  .method private hidebysig static int32 
          '<BitOperators>b__6e'(int32 a,
                                int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  and
    IL_0003:  ret
  } // end of method ExpressionTrees::'<BitOperators>b__6e'

  .method private hidebysig static int32 
          '<BitOperators>b__6f'(int32 a,
                                int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  or
    IL_0003:  ret
  } // end of method ExpressionTrees::'<BitOperators>b__6f'

  .method private hidebysig static int32 
          '<BitOperators>b__70'(int32 a,
                                int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  xor
    IL_0003:  ret
  } // end of method ExpressionTrees::'<BitOperators>b__70'

  .method private hidebysig static int32 
          '<ShiftOperators>b__75'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  shr
    IL_0003:  ret
  } // end of method ExpressionTrees::'<ShiftOperators>b__75'

  .method private hidebysig static int32 
          '<ShiftOperators>b__76'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  shl
    IL_0003:  ret
  } // end of method ExpressionTrees::'<ShiftOperators>b__76'

  .method private hidebysig static int64 
          '<ShiftOperators>b__77'(int64 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  shr
    IL_0003:  ret
  } // end of method ExpressionTrees::'<ShiftOperators>b__77'

  .method private hidebysig static int64 
          '<ShiftOperators>b__78'(int64 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  shl
    IL_0003:  ret
  } // end of method ExpressionTrees::'<ShiftOperators>b__78'

  .method private hidebysig static int32 
          '<SimpleExpressions>b__7d'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  } // end of method ExpressionTrees::'<SimpleExpressions>b__7d'

  .method private hidebysig static int32 
          '<SimpleExpressions>b__7e'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  } // end of method ExpressionTrees::'<SimpleExpressions>b__7e'

  .method private hidebysig static string 
          '<Call>b__84'(string a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0006:  ret
  } // end of method ExpressionTrees::'<Call>b__84'

  .method private hidebysig static string 
          '<Call>b__85'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarga.s   a
    IL_0002:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0007:  ret
  } // end of method ExpressionTrees::'<Call>b__85'

  .method private hidebysig static char[] 
          '<Call>b__86'(string a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<char>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0006:  ret
  } // end of method ExpressionTrees::'<Call>b__86'

  .method private hidebysig static bool  '<Call>b__87'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       16 (0x10)
    .maxstack  2
    .locals init (char V_0)
    IL_0000:  ldc.i4.s   97
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  ldc.i4.s   98
    IL_0007:  call       instance int32 [mscorlib]System.Char::CompareTo(char)
    IL_000c:  ldc.i4.0
    IL_000d:  clt
    IL_000f:  ret
  } // end of method ExpressionTrees::'<Call>b__87'

  .method private hidebysig static bool  '<Quote>b__8c'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       118 (0x76)
    .maxstack  4
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2)
    IL_0000:  ldtoken    [mscorlib]System.Int32
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  ldstr      "n"
    IL_000f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0014:  stloc.0
    IL_0015:  ldtoken    [mscorlib]System.String
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  ldstr      "s"
    IL_0024:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0029:  stloc.1
    IL_002a:  ldloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_0031:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0036:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_003b:  ldc.i4.0
    IL_003c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0041:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0046:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_004b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0050:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0055:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_005a:  ldc.i4.2
    IL_005b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0060:  stloc.2
    IL_0061:  ldloc.2
    IL_0062:  ldc.i4.0
    IL_0063:  ldloc.0
    IL_0064:  stelem.ref
    IL_0065:  ldloc.2
    IL_0066:  ldc.i4.1
    IL_0067:  ldloc.1
    IL_0068:  stelem.ref
    IL_0069:  ldloc.2
    IL_006a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006f:  ldnull
    IL_0070:  ceq
    IL_0072:  ldc.i4.0
    IL_0073:  ceq
    IL_0075:  ret
  } // end of method ExpressionTrees::'<Quote>b__8c'

  .method private hidebysig static int32[] 
          '<ArrayInitializer>b__8e'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  newarr     [mscorlib]System.Int32
    IL_0006:  dup
    IL_0007:  ldtoken    field valuetype '<PrivateImplementationDetails>{416BD71B-B506-4355-8604-9F8C6A2FDEE6}'/'__StaticArrayInitTypeSize=12' '<PrivateImplementationDetails>{416BD71B-B506-4355-8604-9F8C6A2FDEE6}'::'$$method0x60000bc-1'
    IL_000c:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0011:  ret
  } // end of method ExpressionTrees::'<ArrayInitializer>b__8e'

  .method private hidebysig static int32[] 
          '<ArrayInitializer>b__8f'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  newarr     [mscorlib]System.Int32
    IL_0006:  ret
  } // end of method ExpressionTrees::'<ArrayInitializer>b__8f'

  .method private hidebysig static int32[0...,0...] 
          '<ArrayInitializer>b__90'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  ldc.i4.5
    IL_0002:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_0007:  ret
  } // end of method ExpressionTrees::'<ArrayInitializer>b__90'

  .method private hidebysig static int32[][] 
          '<ArrayInitializer>b__91'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  newarr     int32[]
    IL_0006:  ret
  } // end of method ExpressionTrees::'<ArrayInitializer>b__91'

  .method private hidebysig static int32[][] 
          '<ArrayInitializer>b__92'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       29 (0x1d)
    .maxstack  5
    .locals init (int32[][] V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  newarr     int32[]
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.3
    IL_000a:  newarr     [mscorlib]System.Int32
    IL_000f:  dup
    IL_0010:  ldtoken    field valuetype '<PrivateImplementationDetails>{416BD71B-B506-4355-8604-9F8C6A2FDEE6}'/'__StaticArrayInitTypeSize=12' '<PrivateImplementationDetails>{416BD71B-B506-4355-8604-9F8C6A2FDEE6}'::'$$method0x60000c0-1'
    IL_0015:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_001a:  stelem.ref
    IL_001b:  ldloc.0
    IL_001c:  ret
  } // end of method ExpressionTrees::'<ArrayInitializer>b__92'

  .method private hidebysig static object 
          '<AnonymousTypes>b__98'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldc.i4.5
    IL_0001:  ldstr      "Test"
    IL_0006:  newobj     instance void class '<>f__AnonymousType2`2'<int32,string>::.ctor(!0,
                                                                                          !1)
    IL_000b:  ret
  } // end of method ExpressionTrees::'<AnonymousTypes>b__98'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname static 
          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass 
          op_Addition(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass a,
                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass b) cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass::.ctor()
    IL_0005:  ret
  } // end of method MyClass::op_Addition

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
       extends [mscorlib]System.Object
{
  .field public static literal int32 ConstField = int32(0x00000001)
  .field public static initonly int32 StaticReadonlyField
  .field public static int32 StaticField
  .field public initonly int32 ReadonlyField
  .field public int32 Field
  .field private static int32 '<StaticProperty>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private int32 '<Property>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname static 
          int32  get_StaticReadonlyProperty() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  } // end of method SimpleType::get_StaticReadonlyProperty

  .method public hidebysig specialname static 
          int32  get_StaticProperty() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::'<StaticProperty>k__BackingField'
    IL_0005:  ret
  } // end of method SimpleType::get_StaticProperty

  .method public hidebysig specialname static 
          void  set_StaticProperty(int32 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::'<StaticProperty>k__BackingField'
    IL_0006:  ret
  } // end of method SimpleType::set_StaticProperty

  .method public hidebysig specialname instance int32 
          get_ReadonlyProperty() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  } // end of method SimpleType::get_ReadonlyProperty

  .method public hidebysig specialname instance int32 
          get_Property() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::'<Property>k__BackingField'
    IL_0006:  ret
  } // end of method SimpleType::get_Property

  .method public hidebysig specialname instance void 
          set_Property(int32 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::'<Property>k__BackingField'
    IL_0007:  ret
  } // end of method SimpleType::set_Property

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::ReadonlyField
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.3
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::Field
    IL_000e:  ldarg.0
    IL_000f:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0014:  ret
  } // end of method SimpleType::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldc.i4.2
    IL_0001:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::StaticReadonlyField
    IL_0006:  ldc.i4.3
    IL_0007:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::StaticField
    IL_000c:  ret
  } // end of method SimpleType::.cctor

  .property int32 StaticReadonlyProperty()
  {
    .get int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_StaticReadonlyProperty()
  } // end of property SimpleType::StaticReadonlyProperty
  .property int32 StaticProperty()
  {
    .get int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_StaticProperty()
    .set void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::set_StaticProperty(int32)
  } // end of property SimpleType::StaticProperty
  .property instance int32 ReadonlyProperty()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_ReadonlyProperty()
  } // end of property SimpleType::ReadonlyProperty
  .property instance int32 Property()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_Property()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::set_Property(int32)
  } // end of property SimpleType::Property
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithCtor
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 i) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method SimpleTypeWithCtor::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithCtor

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors
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
  } // end of method SimpleTypeWithMultipleCtors::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 i) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method SimpleTypeWithMultipleCtors::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1<T>
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
  } // end of method GenericClassWithCtor`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1<T>
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
  } // end of method GenericClassWithMultipleCtors`1::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 x) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method GenericClassWithMultipleCtors`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClass`1<T>
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
  } // end of method GenericClass`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClass`1

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType0`2'<'<X>j__TPar','<A>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<X>j__TPar' '<X>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<A>j__TPar' '<A>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<X>j__TPar' X,
                               !'<A>j__TPar' A) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType0`2'::.ctor

  .method public hidebysig specialname instance !'<X>j__TPar' 
          get_X() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0`2'::get_X

  .method public hidebysig specialname instance !'<A>j__TPar' 
          get_A() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0`2'::get_A

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ X = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0019:  box        !'<X>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", A = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0037:  box        !'<A>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousType0`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType0`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0xed16d9c
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousType0`2'::GetHashCode

  .property instance !'<X>j__TPar' X()
  {
    .get instance !'<X>j__TPar' '<>f__AnonymousType0`2'::get_X()
  } // end of property '<>f__AnonymousType0`2'::X
  .property instance !'<A>j__TPar' A()
  {
    .get instance !'<A>j__TPar' '<>f__AnonymousType0`2'::get_A()
  } // end of property '<>f__AnonymousType0`2'::A
} // end of class '<>f__AnonymousType0`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType1`2'<'<X>j__TPar','<Y>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<X>j__TPar' '<X>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Y>j__TPar' '<Y>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<X>j__TPar' X,
                               !'<Y>j__TPar' Y) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType1`2'::.ctor

  .method public hidebysig specialname instance !'<X>j__TPar' 
          get_X() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType1`2'::get_X

  .method public hidebysig specialname instance !'<Y>j__TPar' 
          get_Y() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType1`2'::get_Y

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ X = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0019:  box        !'<X>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", Y = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0037:  box        !'<Y>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousType1`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType1`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0xc18f39dd
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousType1`2'::GetHashCode

  .property instance !'<X>j__TPar' X()
  {
    .get instance !'<X>j__TPar' '<>f__AnonymousType1`2'::get_X()
  } // end of property '<>f__AnonymousType1`2'::X
  .property instance !'<Y>j__TPar' Y()
  {
    .get instance !'<Y>j__TPar' '<>f__AnonymousType1`2'::get_Y()
  } // end of property '<>f__AnonymousType1`2'::Y
} // end of class '<>f__AnonymousType1`2'

.class private auto ansi '<PrivateImplementationDetails>{416BD71B-B506-4355-8604-9F8C6A2FDEE6}'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=12'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 12
  } // end of class '__StaticArrayInitTypeSize=12'

  .field static assembly valuetype '<PrivateImplementationDetails>{416BD71B-B506-4355-8604-9F8C6A2FDEE6}'/'__StaticArrayInitTypeSize=12' '$$method0x60000bc-1' at I_00007670
  .field static assembly valuetype '<PrivateImplementationDetails>{416BD71B-B506-4355-8604-9F8C6A2FDEE6}'/'__StaticArrayInitTypeSize=12' '$$method0x60000c0-1' at I_000076A8
} // end of class '<PrivateImplementationDetails>{416BD71B-B506-4355-8604-9F8C6A2FDEE6}'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType2`2'<'<A>j__TPar','<B>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<A>j__TPar' '<A>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<B>j__TPar' '<B>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<A>j__TPar' A,
                               !'<B>j__TPar' B) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType2`2'::.ctor

  .method public hidebysig specialname instance !'<A>j__TPar' 
          get_A() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType2`2'::get_A

  .method public hidebysig specialname instance !'<B>j__TPar' 
          get_B() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType2`2'::get_B

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ A = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_0019:  box        !'<A>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", B = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_0037:  box        !'<B>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousType2`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<B>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<B>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType2`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0xbc6464e2
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<B>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<B>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousType2`2'::GetHashCode

  .property instance !'<A>j__TPar' A()
  {
    .get instance !'<A>j__TPar' '<>f__AnonymousType2`2'::get_A()
  } // end of property '<>f__AnonymousType2`2'::A
  .property instance !'<B>j__TPar' B()
  {
    .get instance !'<B>j__TPar' '<>f__AnonymousType2`2'::get_B()
  } // end of property '<>f__AnonymousType2`2'::B
} // end of class '<>f__AnonymousType2`2'


// =============================================================

.data cil I_00007670 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00) 
.data cil I_0000767C = int8[4]
.data cil I_000076A8 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00) 
// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\ExpressionTrees.opt.res
