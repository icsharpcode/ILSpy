
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
.assembly '3qhjq0hu'
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module '3qhjq0hu.dll'
// MVID: {426C7905-90B3-4EA8-8585-F907E00CBDB7}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02FC0000


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
      // Code size       10 (0xa)
      .maxstack  1
      .locals init (!X V_0)
      IL_0000:  ldsfld     !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<!X>::'<StaticProperty>k__BackingField'
      IL_0005:  stloc.0
      IL_0006:  br.s       IL_0008

      IL_0008:  ldloc.0
      IL_0009:  ret
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
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (!X V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<!X>::'<InstanceProperty>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (bool V_0)
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
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
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass82'::captured
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          ToCode<R>(object x,
                    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!R>> expr) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          ToCode<T,R>(object x,
                      class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!T,!!R>> expr) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          ToCode<T,T2,R>(object x,
                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!T,!!T2,!!R>> expr) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          X() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ExpressionTrees::X

  .method public hidebysig instance void 
          Parameter(bool a) cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass0'::a
    IL_000d:  nop
    IL_000e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0013:  ldloc.0
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0019:  ldtoken    field bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass0'::a
    IL_001e:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0023:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0028:  ldc.i4.0
    IL_0029:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0033:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0038:  pop
    IL_0039:  nop
    IL_003a:  ret
  } // end of method ExpressionTrees::Parameter

  .method public hidebysig instance void 
          LocalVariable() cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass2' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass2'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass2'::a
    IL_000e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0013:  ldloc.0
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0019:  ldtoken    field bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass2'::a
    IL_001e:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0023:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0028:  ldc.i4.0
    IL_0029:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0033:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0038:  pop
    IL_0039:  nop
    IL_003a:  ret
  } // end of method ExpressionTrees::LocalVariable

  .method public hidebysig instance void 
          LambdaParameter() cil managed
  {
    // Code size       52 (0x34)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Boolean
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldstr      "a"
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.1
    IL_001d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0022:  stloc.1
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.0
    IL_0025:  ldloc.0
    IL_0026:  stelem.ref
    IL_0027:  ldloc.1
    IL_0028:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<bool,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_002d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool,bool>(object,
                                                                                                                 class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_0032:  pop
    IL_0033:  ret
  } // end of method ExpressionTrees::LambdaParameter

  .method public hidebysig instance void 
          AddOperator(int32 x) cil managed
  {
    // Code size       111 (0x6f)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass4' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass4'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass4'::x
    IL_000d:  nop
    IL_000e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0013:  ldc.i4.1
    IL_0014:  box        [mscorlib]System.Int32
    IL_0019:  ldtoken    [mscorlib]System.Int32
    IL_001e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0023:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0028:  ldloc.0
    IL_0029:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_002e:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass4'::x
    IL_0033:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0038:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0042:  ldc.i4.2
    IL_0043:  box        [mscorlib]System.Int32
    IL_0048:  ldtoken    [mscorlib]System.Int32
    IL_004d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0052:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0057:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_005c:  ldc.i4.0
    IL_005d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0062:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0067:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006c:  pop
    IL_006d:  nop
    IL_006e:  ret
  } // end of method ExpressionTrees::AddOperator

  .method public hidebysig instance void 
          AnonymousClasses() cil managed
  {
    // Code size       158 (0x9e)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [mscorlib]System.Reflection.MethodInfo[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void class '<>f__AnonymousType0`2'<int32,string>::.ctor(!0,
                                                                                                 !1)
    IL_000b:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_001a:  ldc.i4.2
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  stloc.0
    IL_0021:  ldloc.0
    IL_0022:  ldc.i4.0
    IL_0023:  ldc.i4.3
    IL_0024:  box        [mscorlib]System.Int32
    IL_0029:  ldtoken    [mscorlib]System.Int32
    IL_002e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0038:  stelem.ref
    IL_0039:  ldloc.0
    IL_003a:  ldc.i4.1
    IL_003b:  ldstr      "a"
    IL_0040:  ldtoken    [mscorlib]System.String
    IL_0045:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004f:  stelem.ref
    IL_0050:  ldloc.0
    IL_0051:  ldc.i4.2
    IL_0052:  newarr     [mscorlib]System.Reflection.MethodInfo
    IL_0057:  stloc.1
    IL_0058:  ldloc.1
    IL_0059:  ldc.i4.0
    IL_005a:  ldtoken    method instance !0 class '<>f__AnonymousType0`2'<int32,string>::get_X()
    IL_005f:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_0064:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0069:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006e:  stelem.ref
    IL_006f:  ldloc.1
    IL_0070:  ldc.i4.1
    IL_0071:  ldtoken    method instance !1 class '<>f__AnonymousType0`2'<int32,string>::get_A()
    IL_0076:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_007b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0080:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0085:  stelem.ref
    IL_0086:  ldloc.1
    IL_0087:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_008c:  ldc.i4.0
    IL_008d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0092:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class '<>f__AnonymousType0`2'<int32,string>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0097:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class '<>f__AnonymousType0`2'<int32,string>>(object,
                                                                                                                                                   class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_009c:  pop
    IL_009d:  ret
  } // end of method ExpressionTrees::AnonymousClasses

  .method public hidebysig instance void 
          ArrayIndex() cil managed
  {
    // Code size       233 (0xe9)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldc.i4.3
    IL_0011:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.0
    IL_0019:  ldc.i4.3
    IL_001a:  box        [mscorlib]System.Int32
    IL_001f:  ldtoken    [mscorlib]System.Int32
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002e:  stelem.ref
    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.1
    IL_0031:  ldc.i4.4
    IL_0032:  box        [mscorlib]System.Int32
    IL_0037:  ldtoken    [mscorlib]System.Int32
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0046:  stelem.ref
    IL_0047:  ldloc.0
    IL_0048:  ldc.i4.2
    IL_0049:  ldc.i4.5
    IL_004a:  box        [mscorlib]System.Int32
    IL_004f:  ldtoken    [mscorlib]System.Int32
    IL_0054:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0059:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005e:  stelem.ref
    IL_005f:  ldloc.0
    IL_0060:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0065:  ldc.i4.0
    IL_0066:  box        [mscorlib]System.Int32
    IL_006b:  ldtoken    [mscorlib]System.Int32
    IL_0070:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0075:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007a:  ldnull
    IL_007b:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0080:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0085:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_008a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_008f:  ldtoken    method instance int64 [mscorlib]System.DateTime::get_Ticks()
    IL_0094:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0099:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00a3:  ldc.i4.3
    IL_00a4:  conv.i8
    IL_00a5:  box        [mscorlib]System.Int64
    IL_00aa:  ldtoken    [mscorlib]System.Int64
    IL_00af:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b9:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_00be:  ldtoken    [mscorlib]System.Int32
    IL_00c3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c8:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00cd:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_00d2:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_00d7:  ldc.i4.0
    IL_00d8:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00dd:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e2:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00e7:  pop
    IL_00e8:  ret
  } // end of method ExpressionTrees::ArrayIndex

  .method public hidebysig instance void 
          ArrayLengthAndDoubles() cil managed
  {
    // Code size       302 (0x12e)
    .maxstack  14
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method !!0[] [System.Core]System.Linq.Enumerable::ToArray<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.1
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  ldnull
    IL_0020:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Concat<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0025:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002f:  ldc.i4.2
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0035:  stloc.1
    IL_0036:  ldloc.1
    IL_0037:  ldc.i4.0
    IL_0038:  ldtoken    [mscorlib]System.Double
    IL_003d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0042:  ldc.i4.3
    IL_0043:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0048:  stloc.2
    IL_0049:  ldloc.2
    IL_004a:  ldc.i4.0
    IL_004b:  ldc.r8     1.
    IL_0054:  box        [mscorlib]System.Double
    IL_0059:  ldtoken    [mscorlib]System.Double
    IL_005e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0063:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0068:  stelem.ref
    IL_0069:  ldloc.2
    IL_006a:  ldc.i4.1
    IL_006b:  ldc.r8     2.0099999999999998
    IL_0074:  box        [mscorlib]System.Double
    IL_0079:  ldtoken    [mscorlib]System.Double
    IL_007e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0083:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0088:  stelem.ref
    IL_0089:  ldloc.2
    IL_008a:  ldc.i4.2
    IL_008b:  ldc.r8     3.5
    IL_0094:  box        [mscorlib]System.Double
    IL_0099:  ldtoken    [mscorlib]System.Double
    IL_009e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a8:  stelem.ref
    IL_00a9:  ldloc.2
    IL_00aa:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00af:  stelem.ref
    IL_00b0:  ldloc.1
    IL_00b1:  ldc.i4.1
    IL_00b2:  ldtoken    [mscorlib]System.Double
    IL_00b7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bc:  ldc.i4.2
    IL_00bd:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00c2:  stloc.2
    IL_00c3:  ldloc.2
    IL_00c4:  ldc.i4.0
    IL_00c5:  ldc.r8     1.
    IL_00ce:  box        [mscorlib]System.Double
    IL_00d3:  ldtoken    [mscorlib]System.Double
    IL_00d8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e2:  stelem.ref
    IL_00e3:  ldloc.2
    IL_00e4:  ldc.i4.1
    IL_00e5:  ldc.r8     2.
    IL_00ee:  box        [mscorlib]System.Double
    IL_00f3:  ldtoken    [mscorlib]System.Double
    IL_00f8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0102:  stelem.ref
    IL_0103:  ldloc.2
    IL_0104:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0109:  stelem.ref
    IL_010a:  ldloc.1
    IL_010b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0110:  stelem.ref
    IL_0111:  ldloc.0
    IL_0112:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0117:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayLength(class [System.Core]System.Linq.Expressions.Expression)
    IL_011c:  ldc.i4.0
    IL_011d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0122:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0127:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_012c:  pop
    IL_012d:  ret
  } // end of method ExpressionTrees::ArrayLengthAndDoubles

  .method public hidebysig instance void 
          AsOperator() cil managed
  {
    // Code size       65 (0x41)
    .maxstack  3
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0015:  ldc.i4.0
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0020:  ldtoken    [mscorlib]System.String
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::TypeAs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Type)
    IL_002f:  ldc.i4.0
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0035:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_003a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_003f:  pop
    IL_0040:  ret
  } // end of method ExpressionTrees::AsOperator

  .method public hidebysig instance void 
          ComplexGenericName() cil managed
  {
    // Code size       141 (0x8d)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldstr      "x"
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.0
    IL_001d:  box        [mscorlib]System.Int32
    IL_0022:  ldtoken    [mscorlib]System.Int32
    IL_0027:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0031:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0036:  ldc.i4.1
    IL_0037:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003c:  stloc.1
    IL_003d:  ldloc.1
    IL_003e:  ldc.i4.0
    IL_003f:  ldloc.0
    IL_0040:  stelem.ref
    IL_0041:  ldloc.1
    IL_0042:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0047:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_004c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0051:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0056:  ldc.i4.1
    IL_0057:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_005c:  stloc.2
    IL_005d:  ldloc.2
    IL_005e:  ldc.i4.0
    IL_005f:  ldc.i4.0
    IL_0060:  box        [mscorlib]System.Int32
    IL_0065:  ldtoken    [mscorlib]System.Int32
    IL_006a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0074:  stelem.ref
    IL_0075:  ldloc.2
    IL_0076:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_007b:  ldc.i4.0
    IL_007c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0081:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0086:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008b:  pop
    IL_008c:  ret
  } // end of method ExpressionTrees::ComplexGenericName

  .method public hidebysig instance void 
          DefaultValue() cil managed
  {
    // Code size       174 (0xae)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             valuetype [mscorlib]System.TimeSpan V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void [mscorlib]System.TimeSpan::.ctor(int32,
                                                                               int32,
                                                                               int32)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0015:  ldc.i4.3
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldc.i4.1
    IL_001f:  box        [mscorlib]System.Int32
    IL_0024:  ldtoken    [mscorlib]System.Int32
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0033:  stelem.ref
    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.1
    IL_0036:  ldc.i4.2
    IL_0037:  box        [mscorlib]System.Int32
    IL_003c:  ldtoken    [mscorlib]System.Int32
    IL_0041:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004b:  stelem.ref
    IL_004c:  ldloc.0
    IL_004d:  ldc.i4.2
    IL_004e:  ldc.i4.3
    IL_004f:  box        [mscorlib]System.Int32
    IL_0054:  ldtoken    [mscorlib]System.Int32
    IL_0059:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0063:  stelem.ref
    IL_0064:  ldloc.0
    IL_0065:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_006a:  ldloca.s   V_1
    IL_006c:  initobj    [mscorlib]System.TimeSpan
    IL_0072:  ldloc.1
    IL_0073:  box        [mscorlib]System.TimeSpan
    IL_0078:  ldtoken    [mscorlib]System.TimeSpan
    IL_007d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0082:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0087:  ldc.i4.0
    IL_0088:  ldtoken    method bool [mscorlib]System.TimeSpan::op_Equality(valuetype [mscorlib]System.TimeSpan,
                                                                            valuetype [mscorlib]System.TimeSpan)
    IL_008d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0092:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0097:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_009c:  ldc.i4.0
    IL_009d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a2:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a7:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ac:  pop
    IL_00ad:  ret
  } // end of method ExpressionTrees::DefaultValue

  .method public hidebysig instance void 
          EnumConstant() cil managed
  {
    // Code size       117 (0x75)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0015:  ldc.i4.0
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0020:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_0025:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002f:  ldc.i4.1
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0035:  stloc.0
    IL_0036:  ldloc.0
    IL_0037:  ldc.i4.0
    IL_0038:  ldc.i4.0
    IL_0039:  box        [mscorlib]System.MidpointRounding
    IL_003e:  ldtoken    [mscorlib]System.MidpointRounding
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004d:  ldtoken    [mscorlib]System.Object
    IL_0052:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_005c:  stelem.ref
    IL_005d:  ldloc.0
    IL_005e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0063:  ldc.i4.0
    IL_0064:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0069:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0073:  pop
    IL_0074:  ret
  } // end of method ExpressionTrees::EnumConstant

  .method public hidebysig instance void 
          IndexerAccess() cil managed
  {
    // Code size       184 (0xb8)
    .maxstack  7
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8' V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  ldc.i4.s   20
    IL_000b:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [System.Core]System.Linq.Enumerable::Range(int32,
                                                                                                                                    int32)
    IL_0010:  ldsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_0015:  brtrue.s   IL_002a

    IL_0017:  ldnull
    IL_0018:  ldftn      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<IndexerAccess>b__6'(int32)
    IL_001e:  newobj     instance void class [mscorlib]System.Func`2<int32,string>::.ctor(object,
                                                                                          native int)
    IL_0023:  stsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_0028:  br.s       IL_002a

    IL_002a:  ldsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_002f:  call       class [mscorlib]System.Collections.Generic.Dictionary`2<!!1,!!0> [System.Core]System.Linq.Enumerable::ToDictionary<int32,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                          class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0034:  stfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8'::dict
    IL_0039:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_003e:  ldloc.0
    IL_003f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0044:  ldtoken    field class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8'::dict
    IL_0049:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0053:  ldtoken    method instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::get_Item(!0)
    IL_0058:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>
    IL_005d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0062:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0067:  ldc.i4.1
    IL_0068:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_006d:  stloc.1
    IL_006e:  ldloc.1
    IL_006f:  ldc.i4.0
    IL_0070:  ldstr      "3"
    IL_0075:  ldtoken    [mscorlib]System.String
    IL_007a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0084:  stelem.ref
    IL_0085:  ldloc.1
    IL_0086:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_008b:  ldc.i4.3
    IL_008c:  box        [mscorlib]System.Int32
    IL_0091:  ldtoken    [mscorlib]System.Int32
    IL_0096:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00a5:  ldc.i4.0
    IL_00a6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00ab:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b0:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b5:  pop
    IL_00b6:  nop
    IL_00b7:  ret
  } // end of method ExpressionTrees::IndexerAccess

  .method public hidebysig instance void 
          IsOperator() cil managed
  {
    // Code size       65 (0x41)
    .maxstack  3
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0015:  ldc.i4.0
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0020:  ldtoken    [mscorlib]System.String
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  call       class [System.Core]System.Linq.Expressions.TypeBinaryExpression [System.Core]System.Linq.Expressions.Expression::TypeIs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002f:  ldc.i4.0
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0035:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_003a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_003f:  pop
    IL_0040:  ret
  } // end of method ExpressionTrees::IsOperator

  .method public hidebysig instance void 
          ListInitializer() cil managed
  {
    // Code size       371 (0x173)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.ElementInit[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::.ctor()
    IL_000b:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_001a:  ldc.i4.0
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0025:  ldc.i4.3
    IL_0026:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_002b:  stloc.0
    IL_002c:  ldloc.0
    IL_002d:  ldc.i4.0
    IL_002e:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_0033:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0038:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0042:  ldc.i4.2
    IL_0043:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0048:  stloc.1
    IL_0049:  ldloc.1
    IL_004a:  ldc.i4.0
    IL_004b:  ldc.i4.1
    IL_004c:  box        [mscorlib]System.Int32
    IL_0051:  ldtoken    [mscorlib]System.Int32
    IL_0056:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0060:  stelem.ref
    IL_0061:  ldloc.1
    IL_0062:  ldc.i4.1
    IL_0063:  ldc.i4.1
    IL_0064:  box        [mscorlib]System.Int32
    IL_0069:  ldtoken    [mscorlib]System.Int32
    IL_006e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0073:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0078:  stelem.ref
    IL_0079:  ldloc.1
    IL_007a:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_007f:  stelem.ref
    IL_0080:  ldloc.0
    IL_0081:  ldc.i4.1
    IL_0082:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_0087:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_008c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0091:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0096:  ldc.i4.2
    IL_0097:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_009c:  stloc.1
    IL_009d:  ldloc.1
    IL_009e:  ldc.i4.0
    IL_009f:  ldc.i4.2
    IL_00a0:  box        [mscorlib]System.Int32
    IL_00a5:  ldtoken    [mscorlib]System.Int32
    IL_00aa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00af:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b4:  stelem.ref
    IL_00b5:  ldloc.1
    IL_00b6:  ldc.i4.1
    IL_00b7:  ldc.i4.2
    IL_00b8:  box        [mscorlib]System.Int32
    IL_00bd:  ldtoken    [mscorlib]System.Int32
    IL_00c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cc:  stelem.ref
    IL_00cd:  ldloc.1
    IL_00ce:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00d3:  stelem.ref
    IL_00d4:  ldloc.0
    IL_00d5:  ldc.i4.2
    IL_00d6:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_00db:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_00e0:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e5:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00ea:  ldc.i4.2
    IL_00eb:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00f0:  stloc.1
    IL_00f1:  ldloc.1
    IL_00f2:  ldc.i4.0
    IL_00f3:  ldc.i4.3
    IL_00f4:  box        [mscorlib]System.Int32
    IL_00f9:  ldtoken    [mscorlib]System.Int32
    IL_00fe:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0103:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0108:  stelem.ref
    IL_0109:  ldloc.1
    IL_010a:  ldc.i4.1
    IL_010b:  ldc.i4.4
    IL_010c:  box        [mscorlib]System.Int32
    IL_0111:  ldtoken    [mscorlib]System.Int32
    IL_0116:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0120:  stelem.ref
    IL_0121:  ldloc.1
    IL_0122:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0127:  stelem.ref
    IL_0128:  ldloc.0
    IL_0129:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_012e:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::get_Count()
    IL_0133:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0138:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0142:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0147:  ldc.i4.3
    IL_0148:  box        [mscorlib]System.Int32
    IL_014d:  ldtoken    [mscorlib]System.Int32
    IL_0152:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0157:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_015c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0161:  ldc.i4.0
    IL_0162:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0167:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_016c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0171:  pop
    IL_0172:  ret
  } // end of method ExpressionTrees::ListInitializer

  .method public hidebysig instance void 
          ListInitializer2() cil managed
  {
    // Code size       326 (0x146)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.ElementInit[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor(int32)
    IL_000b:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_001a:  ldc.i4.1
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  stloc.0
    IL_0021:  ldloc.0
    IL_0022:  ldc.i4.0
    IL_0023:  ldc.i4.s   50
    IL_0025:  box        [mscorlib]System.Int32
    IL_002a:  ldtoken    [mscorlib]System.Int32
    IL_002f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0034:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0039:  stelem.ref
    IL_003a:  ldloc.0
    IL_003b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0040:  ldc.i4.3
    IL_0041:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_0046:  stloc.1
    IL_0047:  ldloc.1
    IL_0048:  ldc.i4.0
    IL_0049:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_004e:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0053:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0058:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005d:  ldc.i4.1
    IL_005e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0063:  stloc.0
    IL_0064:  ldloc.0
    IL_0065:  ldc.i4.0
    IL_0066:  ldc.i4.1
    IL_0067:  box        [mscorlib]System.Int32
    IL_006c:  ldtoken    [mscorlib]System.Int32
    IL_0071:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0076:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007b:  stelem.ref
    IL_007c:  ldloc.0
    IL_007d:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0082:  stelem.ref
    IL_0083:  ldloc.1
    IL_0084:  ldc.i4.1
    IL_0085:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_008a:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_008f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0094:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0099:  ldc.i4.1
    IL_009a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_009f:  stloc.0
    IL_00a0:  ldloc.0
    IL_00a1:  ldc.i4.0
    IL_00a2:  ldc.i4.2
    IL_00a3:  box        [mscorlib]System.Int32
    IL_00a8:  ldtoken    [mscorlib]System.Int32
    IL_00ad:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b7:  stelem.ref
    IL_00b8:  ldloc.0
    IL_00b9:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00be:  stelem.ref
    IL_00bf:  ldloc.1
    IL_00c0:  ldc.i4.2
    IL_00c1:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00c6:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00cb:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d0:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00d5:  ldc.i4.1
    IL_00d6:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00db:  stloc.0
    IL_00dc:  ldloc.0
    IL_00dd:  ldc.i4.0
    IL_00de:  ldc.i4.3
    IL_00df:  box        [mscorlib]System.Int32
    IL_00e4:  ldtoken    [mscorlib]System.Int32
    IL_00e9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ee:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00f3:  stelem.ref
    IL_00f4:  ldloc.0
    IL_00f5:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00fa:  stelem.ref
    IL_00fb:  ldloc.1
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_0101:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_0106:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_010b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0110:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0115:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_011a:  ldc.i4.3
    IL_011b:  box        [mscorlib]System.Int32
    IL_0120:  ldtoken    [mscorlib]System.Int32
    IL_0125:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_012a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_012f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0134:  ldc.i4.0
    IL_0135:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_013a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_013f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0144:  pop
    IL_0145:  ret
  } // end of method ExpressionTrees::ListInitializer2

  .method public hidebysig instance void 
          ListInitializer3() cil managed
  {
    // Code size       299 (0x12b)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.ElementInit[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_000b:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_001a:  ldc.i4.0
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0025:  ldc.i4.3
    IL_0026:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_002b:  stloc.0
    IL_002c:  ldloc.0
    IL_002d:  ldc.i4.0
    IL_002e:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0033:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0038:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0042:  ldc.i4.1
    IL_0043:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0048:  stloc.1
    IL_0049:  ldloc.1
    IL_004a:  ldc.i4.0
    IL_004b:  ldc.i4.1
    IL_004c:  box        [mscorlib]System.Int32
    IL_0051:  ldtoken    [mscorlib]System.Int32
    IL_0056:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0060:  stelem.ref
    IL_0061:  ldloc.1
    IL_0062:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0067:  stelem.ref
    IL_0068:  ldloc.0
    IL_0069:  ldc.i4.1
    IL_006a:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_006f:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0074:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0079:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007e:  ldc.i4.1
    IL_007f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0084:  stloc.1
    IL_0085:  ldloc.1
    IL_0086:  ldc.i4.0
    IL_0087:  ldc.i4.2
    IL_0088:  box        [mscorlib]System.Int32
    IL_008d:  ldtoken    [mscorlib]System.Int32
    IL_0092:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0097:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009c:  stelem.ref
    IL_009d:  ldloc.1
    IL_009e:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00a3:  stelem.ref
    IL_00a4:  ldloc.0
    IL_00a5:  ldc.i4.2
    IL_00a6:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00ab:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00b0:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b5:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00ba:  ldc.i4.1
    IL_00bb:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00c0:  stloc.1
    IL_00c1:  ldloc.1
    IL_00c2:  ldc.i4.0
    IL_00c3:  ldc.i4.3
    IL_00c4:  box        [mscorlib]System.Int32
    IL_00c9:  ldtoken    [mscorlib]System.Int32
    IL_00ce:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d8:  stelem.ref
    IL_00d9:  ldloc.1
    IL_00da:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00df:  stelem.ref
    IL_00e0:  ldloc.0
    IL_00e1:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_00e6:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_00eb:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00f0:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f5:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00fa:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00ff:  ldc.i4.3
    IL_0100:  box        [mscorlib]System.Int32
    IL_0105:  ldtoken    [mscorlib]System.Int32
    IL_010a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0114:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0119:  ldc.i4.0
    IL_011a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_011f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0124:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0129:  pop
    IL_012a:  ret
  } // end of method ExpressionTrees::ListInitializer3

  .method public hidebysig instance void 
          LiteralCharAndProperty() cil managed
  {
    // Code size       147 (0x93)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void [mscorlib]System.String::.ctor(char,
                                                                             int32)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldc.i4.s   32
    IL_0020:  box        [mscorlib]System.Char
    IL_0025:  ldtoken    [mscorlib]System.Char
    IL_002a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0034:  stelem.ref
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.1
    IL_0037:  ldc.i4.3
    IL_0038:  box        [mscorlib]System.Int32
    IL_003d:  ldtoken    [mscorlib]System.Int32
    IL_0042:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0047:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004c:  stelem.ref
    IL_004d:  ldloc.0
    IL_004e:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0053:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0058:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_005d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0062:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0067:  ldc.i4.1
    IL_0068:  box        [mscorlib]System.Int32
    IL_006d:  ldtoken    [mscorlib]System.Int32
    IL_0072:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0077:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0081:  ldc.i4.0
    IL_0082:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0087:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0091:  pop
    IL_0092:  ret
  } // end of method ExpressionTrees::LiteralCharAndProperty

  .method public hidebysig instance void 
          CharNoCast() cil managed
  {
    // Code size       138 (0x8a)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldstr      "abc"
    IL_000b:  ldtoken    [mscorlib]System.String
    IL_0010:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001a:  ldtoken    method instance char [mscorlib]System.String::get_Chars(int32)
    IL_001f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0024:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0029:  ldc.i4.1
    IL_002a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002f:  stloc.0
    IL_0030:  ldloc.0
    IL_0031:  ldc.i4.0
    IL_0032:  ldc.i4.1
    IL_0033:  box        [mscorlib]System.Int32
    IL_0038:  ldtoken    [mscorlib]System.Int32
    IL_003d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0047:  stelem.ref
    IL_0048:  ldloc.0
    IL_0049:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004e:  ldtoken    [mscorlib]System.Int32
    IL_0053:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0058:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_005d:  ldc.i4.s   98
    IL_005f:  box        [mscorlib]System.Int32
    IL_0064:  ldtoken    [mscorlib]System.Int32
    IL_0069:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0073:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0078:  ldc.i4.0
    IL_0079:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0083:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0088:  pop
    IL_0089:  ret
  } // end of method ExpressionTrees::CharNoCast

  .method public hidebysig instance void 
          StringsImplicitCast() cil managed
  {
    // Code size       378 (0x17a)
    .maxstack  8
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassa' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassa'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassa'::i
    IL_000e:  ldloc.0
    IL_000f:  ldstr      "X"
    IL_0014:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassa'::x
    IL_0019:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_001e:  ldstr      "a\n\\b"
    IL_0023:  ldtoken    [mscorlib]System.String
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  ldloc.0
    IL_0033:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0038:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassa'::x
    IL_003d:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0047:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_004c:  ldloc.0
    IL_004d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0052:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassa'::x
    IL_0057:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_005c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0061:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0066:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_006b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0070:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0075:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_007a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0084:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0089:  ldc.i4.2
    IL_008a:  box        [mscorlib]System.Int32
    IL_008f:  ldtoken    [mscorlib]System.Int32
    IL_0094:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0099:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00a3:  ldc.i4.0
    IL_00a4:  box        [mscorlib]System.Boolean
    IL_00a9:  ldtoken    [mscorlib]System.Boolean
    IL_00ae:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b8:  ldc.i4.1
    IL_00b9:  box        [mscorlib]System.Boolean
    IL_00be:  ldtoken    [mscorlib]System.Boolean
    IL_00c3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cd:  ldc.i4.1
    IL_00ce:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_00d3:  box        [mscorlib]System.Decimal
    IL_00d8:  ldtoken    [mscorlib]System.Decimal
    IL_00dd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e7:  ldloc.0
    IL_00e8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00ed:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassa'::i
    IL_00f2:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_0101:  ldtoken    [mscorlib]System.Decimal
    IL_0106:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010b:  ldtoken    method valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
    IL_0110:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0115:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_011a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type,
                                                                                                                                             class [mscorlib]System.Reflection.MethodInfo)
    IL_011f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0124:  ldc.i4.0
    IL_0125:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_012a:  box        [mscorlib]System.Decimal
    IL_012f:  ldtoken    [mscorlib]System.Decimal
    IL_0134:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0139:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_013e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0143:  ldc.i4.0
    IL_0144:  box        [mscorlib]System.Boolean
    IL_0149:  ldtoken    [mscorlib]System.Boolean
    IL_014e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0153:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0158:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_015d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0162:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0167:  ldc.i4.0
    IL_0168:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_016d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0172:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0177:  pop
    IL_0178:  nop
    IL_0179:  ret
  } // end of method ExpressionTrees::StringsImplicitCast

  .method public hidebysig instance void 
          NotImplicitCast() cil managed
  {
    // Code size       106 (0x6a)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassc' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassc'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.s   42
    IL_000a:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassc'::z
    IL_000f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0014:  ldloc.0
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_001a:  ldtoken    field uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClassc'::z
    IL_001f:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0024:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0029:  ldtoken    [mscorlib]System.Int32
    IL_002e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0038:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_003d:  ldc.i4.0
    IL_003e:  box        [mscorlib]System.Int32
    IL_0043:  ldtoken    [mscorlib]System.Int32
    IL_0048:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0052:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0057:  ldc.i4.0
    IL_0058:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_005d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0062:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0067:  pop
    IL_0068:  nop
    IL_0069:  ret
  } // end of method ExpressionTrees::NotImplicitCast

  .method public hidebysig instance void 
          MembersBuiltin() cil managed
  {
    // Code size       406 (0x196)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldc.i4.s   123
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.0
    IL_000b:  ldc.i4.2
    IL_000c:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0011:  box        [mscorlib]System.Decimal
    IL_0016:  ldtoken    [mscorlib]System.Decimal
    IL_001b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0020:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0025:  ldtoken    method instance string [mscorlib]System.Decimal::ToString()
    IL_002a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0034:  ldc.i4.0
    IL_0035:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_003a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_003f:  ldc.i4.0
    IL_0040:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0045:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_004a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_004f:  pop
    IL_0050:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0055:  ldc.i4     0x7fff
    IL_005a:  box        [mscorlib]System.AttributeTargets
    IL_005f:  ldtoken    [mscorlib]System.AttributeTargets
    IL_0064:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0069:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006e:  ldtoken    method instance bool [mscorlib]System.Enum::HasFlag(class [mscorlib]System.Enum)
    IL_0073:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0078:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007d:  ldc.i4.1
    IL_007e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0083:  stloc.0
    IL_0084:  ldloc.0
    IL_0085:  ldc.i4.0
    IL_0086:  ldc.i4.1
    IL_0087:  box        [mscorlib]System.AttributeTargets
    IL_008c:  ldtoken    [mscorlib]System.AttributeTargets
    IL_0091:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0096:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009b:  ldtoken    [mscorlib]System.Enum
    IL_00a0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a5:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00aa:  stelem.ref
    IL_00ab:  ldloc.0
    IL_00ac:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00b1:  ldc.i4.0
    IL_00b2:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b7:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00bc:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00c1:  pop
    IL_00c2:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00c7:  ldstr      "abc"
    IL_00cc:  ldtoken    [mscorlib]System.String
    IL_00d1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00db:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_00e0:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00e5:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00ea:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00ef:  ldc.i4.3
    IL_00f0:  box        [mscorlib]System.Int32
    IL_00f5:  ldtoken    [mscorlib]System.Int32
    IL_00fa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ff:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0104:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0109:  ldc.i4.0
    IL_010a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_010f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0114:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0119:  pop
    IL_011a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_011f:  ldc.i4.s   97
    IL_0121:  box        [mscorlib]System.Char
    IL_0126:  ldtoken    [mscorlib]System.Char
    IL_012b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0130:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0135:  ldtoken    method instance int32 [mscorlib]System.Char::CompareTo(char)
    IL_013a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_013f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0144:  ldc.i4.1
    IL_0145:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_014a:  stloc.0
    IL_014b:  ldloc.0
    IL_014c:  ldc.i4.0
    IL_014d:  ldc.i4.s   98
    IL_014f:  box        [mscorlib]System.Char
    IL_0154:  ldtoken    [mscorlib]System.Char
    IL_0159:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_015e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0163:  stelem.ref
    IL_0164:  ldloc.0
    IL_0165:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_016a:  ldc.i4.0
    IL_016b:  box        [mscorlib]System.Int32
    IL_0170:  ldtoken    [mscorlib]System.Int32
    IL_0175:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_017a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_017f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0184:  ldc.i4.0
    IL_0185:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_018a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_018f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0194:  pop
    IL_0195:  ret
  } // end of method ExpressionTrees::MembersBuiltin

  .method public hidebysig instance void 
          MembersDefault() cil managed
  {
    // Code size       589 (0x24d)
    .maxstack  7
    .locals init (valuetype [mscorlib]System.DateTime V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldloca.s   V_0
    IL_0008:  initobj    [mscorlib]System.DateTime
    IL_000e:  ldloc.0
    IL_000f:  box        [mscorlib]System.DateTime
    IL_0014:  ldtoken    [mscorlib]System.DateTime
    IL_0019:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0023:  ldtoken    method instance int64 [mscorlib]System.DateTime::get_Ticks()
    IL_0028:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0032:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0037:  ldc.i4.0
    IL_0038:  conv.i8
    IL_0039:  box        [mscorlib]System.Int64
    IL_003e:  ldtoken    [mscorlib]System.Int64
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
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
    IL_0063:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0068:  ldnull
    IL_0069:  box        [mscorlib]System.Array
    IL_006e:  ldtoken    [mscorlib]System.Array
    IL_0073:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0078:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007d:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_0082:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0087:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_008c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0091:  ldc.i4.0
    IL_0092:  box        [mscorlib]System.Int32
    IL_0097:  ldtoken    [mscorlib]System.Int32
    IL_009c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a6:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00ab:  ldc.i4.0
    IL_00ac:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b6:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00bb:  pop
    IL_00bc:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00c1:  ldnull
    IL_00c2:  box        [mscorlib]System.Type
    IL_00c7:  ldtoken    [mscorlib]System.Type
    IL_00cc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d6:  ldtoken    method instance bool [mscorlib]System.Type::get_IsLayoutSequential()
    IL_00db:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00e0:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e5:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00ea:  ldc.i4.0
    IL_00eb:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00f0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00f5:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00fa:  pop
    IL_00fb:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0100:  ldnull
    IL_0101:  box        class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0106:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_010b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0110:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0115:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_011a:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_011f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0124:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0129:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_012e:  ldc.i4.0
    IL_012f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0134:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0139:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_013e:  pop
    IL_013f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0144:  ldnull
    IL_0145:  box        [mscorlib]System.Array
    IL_014a:  ldtoken    [mscorlib]System.Array
    IL_014f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0154:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0159:  ldtoken    method instance object [mscorlib]System.Array::Clone()
    IL_015e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0163:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0168:  ldc.i4.0
    IL_0169:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_016e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0173:  ldnull
    IL_0174:  box        [mscorlib]System.Object
    IL_0179:  ldtoken    [mscorlib]System.Object
    IL_017e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0183:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0188:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_018d:  ldc.i4.0
    IL_018e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0193:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0198:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_019d:  pop
    IL_019e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_01a3:  ldnull
    IL_01a4:  box        [mscorlib]System.Type
    IL_01a9:  ldtoken    [mscorlib]System.Type
    IL_01ae:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01b8:  ldtoken    method instance bool [mscorlib]System.Type::IsInstanceOfType(object)
    IL_01bd:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01c2:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01c7:  ldc.i4.1
    IL_01c8:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01cd:  stloc.1
    IL_01ce:  ldloc.1
    IL_01cf:  ldc.i4.0
    IL_01d0:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_01d5:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01da:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_01df:  ldc.i4.0
    IL_01e0:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01e5:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_01ea:  stelem.ref
    IL_01eb:  ldloc.1
    IL_01ec:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01f1:  ldc.i4.0
    IL_01f2:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01f7:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01fc:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0201:  pop
    IL_0202:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0207:  ldnull
    IL_0208:  box        class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_020d:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0212:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0217:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_021c:  ldtoken    method instance class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::AsReadOnly()
    IL_0221:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0226:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_022b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0230:  ldc.i4.0
    IL_0231:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0236:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_023b:  ldc.i4.0
    IL_023c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0241:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0246:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<int32>>(object,
                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_024b:  pop
    IL_024c:  ret
  } // end of method ExpressionTrees::MembersDefault

  .method public hidebysig instance void 
          DoAssert() cil managed
  {
    // Code size       399 (0x18f)
    .maxstack  8
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldarg.0
    IL_0007:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_000c:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0011:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001b:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'field'
    IL_0020:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0025:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_002a:  ldarg.0
    IL_002b:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0030:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0035:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_003f:  ldtoken    method instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::C()
    IL_0044:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0049:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_004e:  ldc.i4.0
    IL_004f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0054:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0059:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_005e:  ldc.i4.0
    IL_005f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0064:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0069:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006e:  pop
    IL_006f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0074:  ldnull
    IL_0075:  ldtoken    method bool [mscorlib]System.Object::ReferenceEquals(object,
                                                                              object)
    IL_007a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0084:  ldc.i4.2
    IL_0085:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_008a:  stloc.0
    IL_008b:  ldloc.0
    IL_008c:  ldc.i4.0
    IL_008d:  ldarg.0
    IL_008e:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0093:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0098:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a2:  stelem.ref
    IL_00a3:  ldloc.0
    IL_00a4:  ldc.i4.1
    IL_00a5:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::.ctor()
    IL_00aa:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00af:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_00b4:  ldc.i4.0
    IL_00b5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ba:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_00bf:  stelem.ref
    IL_00c0:  ldloc.0
    IL_00c1:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00c6:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00cb:  ldc.i4.0
    IL_00cc:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00d1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00d6:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00db:  pop
    IL_00dc:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00e1:  ldarg.0
    IL_00e2:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_00e7:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_00ec:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00f6:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::MyEquals(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees)
    IL_00fb:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0100:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0105:  ldc.i4.1
    IL_0106:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_010b:  stloc.0
    IL_010c:  ldloc.0
    IL_010d:  ldc.i4.0
    IL_010e:  ldarg.0
    IL_010f:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0114:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0119:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0123:  stelem.ref
    IL_0124:  ldloc.0
    IL_0125:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_012a:  ldarg.0
    IL_012b:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0130:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0135:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_013f:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::MyEquals(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees)
    IL_0144:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0149:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_014e:  ldc.i4.1
    IL_014f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0154:  stloc.0
    IL_0155:  ldloc.0
    IL_0156:  ldc.i4.0
    IL_0157:  ldnull
    IL_0158:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_015d:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0162:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0167:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_016c:  stelem.ref
    IL_016d:  ldloc.0
    IL_016e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0173:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0178:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_017d:  ldc.i4.0
    IL_017e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0183:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0188:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_018d:  pop
    IL_018e:  ret
  } // end of method ExpressionTrees::DoAssert

  .method private hidebysig instance int32 
          C() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method ExpressionTrees::C

  .method private hidebysig instance bool 
          MyEquals(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees other) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method ExpressionTrees::MyEquals

  .method public hidebysig instance void 
          MethodGroupAsExtensionMethod() cil managed
  {
    // Code size       288 (0x120)
    .maxstack  10
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method bool [System.Core]System.Linq.Enumerable::Any<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  box        [mscorlib]System.Reflection.MethodInfo
    IL_001a:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_001f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0024:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0029:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_002e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0033:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0038:  ldc.i4.2
    IL_0039:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_003e:  stloc.0
    IL_003f:  ldloc.0
    IL_0040:  ldc.i4.0
    IL_0041:  ldtoken    class [mscorlib]System.Func`1<bool>
    IL_0046:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004b:  box        [mscorlib]System.Type
    IL_0050:  ldtoken    [mscorlib]System.Type
    IL_0055:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005f:  stelem.ref
    IL_0060:  ldloc.0
    IL_0061:  ldc.i4.1
    IL_0062:  ldtoken    [mscorlib]System.Int32
    IL_0067:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006c:  ldc.i4.4
    IL_006d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0072:  stloc.1
    IL_0073:  ldloc.1
    IL_0074:  ldc.i4.0
    IL_0075:  ldc.i4     0x7d0
    IL_007a:  box        [mscorlib]System.Int32
    IL_007f:  ldtoken    [mscorlib]System.Int32
    IL_0084:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0089:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008e:  stelem.ref
    IL_008f:  ldloc.1
    IL_0090:  ldc.i4.1
    IL_0091:  ldc.i4     0x7d4
    IL_0096:  box        [mscorlib]System.Int32
    IL_009b:  ldtoken    [mscorlib]System.Int32
    IL_00a0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00aa:  stelem.ref
    IL_00ab:  ldloc.1
    IL_00ac:  ldc.i4.2
    IL_00ad:  ldc.i4     0x7d8
    IL_00b2:  box        [mscorlib]System.Int32
    IL_00b7:  ldtoken    [mscorlib]System.Int32
    IL_00bc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c6:  stelem.ref
    IL_00c7:  ldloc.1
    IL_00c8:  ldc.i4.3
    IL_00c9:  ldc.i4     0x7dc
    IL_00ce:  box        [mscorlib]System.Int32
    IL_00d3:  ldtoken    [mscorlib]System.Int32
    IL_00d8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e2:  stelem.ref
    IL_00e3:  ldloc.1
    IL_00e4:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00e9:  ldtoken    class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_00ee:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f3:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00f8:  stelem.ref
    IL_00f9:  ldloc.0
    IL_00fa:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ff:  ldtoken    class [mscorlib]System.Func`1<bool>
    IL_0104:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0109:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_010e:  ldc.i4.0
    IL_010f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0114:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Func`1<bool>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0119:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Func`1<bool>>(object,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_011e:  pop
    IL_011f:  ret
  } // end of method ExpressionTrees::MethodGroupAsExtensionMethod

  .method public hidebysig instance void 
          MethodGroupConstant() cil managed
  {
    // Code size       900 (0x384)
    .maxstack  11
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10' V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_000c:  ldnull
    IL_000d:  ldtoken    method bool [mscorlib]System.Array::TrueForAll<int32>(!!0[],
                                                                               class [mscorlib]System.Predicate`1<!!0>)
    IL_0012:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0017:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_001c:  ldc.i4.2
    IL_001d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0022:  stloc.1
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.0
    IL_0025:  ldtoken    [mscorlib]System.Int32
    IL_002a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  ldc.i4.4
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0035:  stloc.2
    IL_0036:  ldloc.2
    IL_0037:  ldc.i4.0
    IL_0038:  ldc.i4     0x7d0
    IL_003d:  box        [mscorlib]System.Int32
    IL_0042:  ldtoken    [mscorlib]System.Int32
    IL_0047:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0051:  stelem.ref
    IL_0052:  ldloc.2
    IL_0053:  ldc.i4.1
    IL_0054:  ldc.i4     0x7d4
    IL_0059:  box        [mscorlib]System.Int32
    IL_005e:  ldtoken    [mscorlib]System.Int32
    IL_0063:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0068:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006d:  stelem.ref
    IL_006e:  ldloc.2
    IL_006f:  ldc.i4.2
    IL_0070:  ldc.i4     0x7d8
    IL_0075:  box        [mscorlib]System.Int32
    IL_007a:  ldtoken    [mscorlib]System.Int32
    IL_007f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0084:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0089:  stelem.ref
    IL_008a:  ldloc.2
    IL_008b:  ldc.i4.3
    IL_008c:  ldc.i4     0x7dc
    IL_0091:  box        [mscorlib]System.Int32
    IL_0096:  ldtoken    [mscorlib]System.Int32
    IL_009b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a5:  stelem.ref
    IL_00a6:  ldloc.2
    IL_00a7:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ac:  stelem.ref
    IL_00ad:  ldloc.1
    IL_00ae:  ldc.i4.1
    IL_00af:  ldtoken    method bool [mscorlib]System.DateTime::IsLeapYear(int32)
    IL_00b4:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00b9:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00be:  box        [mscorlib]System.Reflection.MethodInfo
    IL_00c3:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_00c8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d2:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_00d7:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00dc:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e1:  ldc.i4.2
    IL_00e2:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00e7:  stloc.2
    IL_00e8:  ldloc.2
    IL_00e9:  ldc.i4.0
    IL_00ea:  ldtoken    class [mscorlib]System.Predicate`1<int32>
    IL_00ef:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f4:  box        [mscorlib]System.Type
    IL_00f9:  ldtoken    [mscorlib]System.Type
    IL_00fe:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0103:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0108:  stelem.ref
    IL_0109:  ldloc.2
    IL_010a:  ldc.i4.1
    IL_010b:  ldnull
    IL_010c:  ldtoken    [mscorlib]System.Object
    IL_0111:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0116:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_011b:  stelem.ref
    IL_011c:  ldloc.2
    IL_011d:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0122:  ldtoken    class [mscorlib]System.Predicate`1<int32>
    IL_0127:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_012c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0131:  stelem.ref
    IL_0132:  ldloc.1
    IL_0133:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0138:  ldc.i4.0
    IL_0139:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_013e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0143:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0148:  pop
    IL_0149:  ldloc.0
    IL_014a:  newobj     instance void class [System.Core]System.Collections.Generic.HashSet`1<int32>::.ctor()
    IL_014f:  stfld      class [System.Core]System.Collections.Generic.HashSet`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10'::set
    IL_0154:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0159:  ldnull
    IL_015a:  ldtoken    method bool [System.Core]System.Linq.Enumerable::All<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                     class [mscorlib]System.Func`2<!!0,bool>)
    IL_015f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0164:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0169:  ldc.i4.2
    IL_016a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_016f:  stloc.1
    IL_0170:  ldloc.1
    IL_0171:  ldc.i4.0
    IL_0172:  ldtoken    [mscorlib]System.Int32
    IL_0177:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_017c:  ldc.i4.4
    IL_017d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0182:  stloc.2
    IL_0183:  ldloc.2
    IL_0184:  ldc.i4.0
    IL_0185:  ldc.i4     0x7d0
    IL_018a:  box        [mscorlib]System.Int32
    IL_018f:  ldtoken    [mscorlib]System.Int32
    IL_0194:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0199:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_019e:  stelem.ref
    IL_019f:  ldloc.2
    IL_01a0:  ldc.i4.1
    IL_01a1:  ldc.i4     0x7d4
    IL_01a6:  box        [mscorlib]System.Int32
    IL_01ab:  ldtoken    [mscorlib]System.Int32
    IL_01b0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01ba:  stelem.ref
    IL_01bb:  ldloc.2
    IL_01bc:  ldc.i4.2
    IL_01bd:  ldc.i4     0x7d8
    IL_01c2:  box        [mscorlib]System.Int32
    IL_01c7:  ldtoken    [mscorlib]System.Int32
    IL_01cc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01d1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01d6:  stelem.ref
    IL_01d7:  ldloc.2
    IL_01d8:  ldc.i4.3
    IL_01d9:  ldc.i4     0x7dc
    IL_01de:  box        [mscorlib]System.Int32
    IL_01e3:  ldtoken    [mscorlib]System.Int32
    IL_01e8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ed:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01f2:  stelem.ref
    IL_01f3:  ldloc.2
    IL_01f4:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_01f9:  stelem.ref
    IL_01fa:  ldloc.1
    IL_01fb:  ldc.i4.1
    IL_01fc:  ldtoken    method instance bool class [System.Core]System.Collections.Generic.HashSet`1<int32>::Add(!0)
    IL_0201:  ldtoken    class [System.Core]System.Collections.Generic.HashSet`1<int32>
    IL_0206:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_020b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0210:  box        [mscorlib]System.Reflection.MethodInfo
    IL_0215:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_021a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_021f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0224:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_0229:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_022e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0233:  ldc.i4.2
    IL_0234:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0239:  stloc.2
    IL_023a:  ldloc.2
    IL_023b:  ldc.i4.0
    IL_023c:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_0241:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0246:  box        [mscorlib]System.Type
    IL_024b:  ldtoken    [mscorlib]System.Type
    IL_0250:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0255:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_025a:  stelem.ref
    IL_025b:  ldloc.2
    IL_025c:  ldc.i4.1
    IL_025d:  ldloc.0
    IL_025e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0263:  ldtoken    field class [System.Core]System.Collections.Generic.HashSet`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10'::set
    IL_0268:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_026d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0272:  stelem.ref
    IL_0273:  ldloc.2
    IL_0274:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0279:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_027e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0283:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0288:  stelem.ref
    IL_0289:  ldloc.1
    IL_028a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_028f:  ldc.i4.0
    IL_0290:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0295:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_029a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_029f:  pop
    IL_02a0:  ldloc.0
    IL_02a1:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_02a6:  brtrue.s   IL_02bb

    IL_02a8:  ldnull
    IL_02a9:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<MethodGroupConstant>b__e'(class [mscorlib]System.Func`3<object,object,bool>)
    IL_02af:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool>::.ctor(object,
                                                                                                                                    native int)
    IL_02b4:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_02b9:  br.s       IL_02bb

    IL_02bb:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_02c0:  stfld      class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10'::sink
    IL_02c5:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_02ca:  ldloc.0
    IL_02cb:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_02d0:  ldtoken    field class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10'::sink
    IL_02d5:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_02da:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_02df:  ldc.i4.1
    IL_02e0:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_02e5:  stloc.1
    IL_02e6:  ldloc.1
    IL_02e7:  ldc.i4.0
    IL_02e8:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_02ed:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_02f2:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_02f7:  box        [mscorlib]System.Reflection.MethodInfo
    IL_02fc:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_0301:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0306:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_030b:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_0310:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0315:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_031a:  ldc.i4.2
    IL_031b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0320:  stloc.2
    IL_0321:  ldloc.2
    IL_0322:  ldc.i4.0
    IL_0323:  ldtoken    class [mscorlib]System.Func`3<object,object,bool>
    IL_0328:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_032d:  box        [mscorlib]System.Type
    IL_0332:  ldtoken    [mscorlib]System.Type
    IL_0337:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_033c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0341:  stelem.ref
    IL_0342:  ldloc.2
    IL_0343:  ldc.i4.1
    IL_0344:  ldnull
    IL_0345:  ldtoken    [mscorlib]System.Object
    IL_034a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_034f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0354:  stelem.ref
    IL_0355:  ldloc.2
    IL_0356:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_035b:  ldtoken    class [mscorlib]System.Func`3<object,object,bool>
    IL_0360:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0365:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_036a:  stelem.ref
    IL_036b:  ldloc.1
    IL_036c:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_0371:  ldc.i4.0
    IL_0372:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0377:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_037c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0381:  pop
    IL_0382:  nop
    IL_0383:  ret
  } // end of method ExpressionTrees::MethodGroupConstant

  .method public hidebysig instance void 
          MultipleCasts() cil managed
  {
    // Code size       101 (0x65)
    .maxstack  4
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldc.i4.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  ldtoken    [mscorlib]System.Int32
    IL_0011:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001b:  ldc.i4.1
    IL_001c:  box        [mscorlib]System.Int32
    IL_0021:  ldtoken    [mscorlib]System.Int32
    IL_0026:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0030:  ldtoken    [mscorlib]System.Object
    IL_0035:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_003f:  ldtoken    [mscorlib]System.Int32
    IL_0044:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0049:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_004e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0053:  ldc.i4.0
    IL_0054:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0059:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0063:  pop
    IL_0064:  ret
  } // end of method ExpressionTrees::MultipleCasts

  .method public hidebysig instance void 
          MultipleDots() cil managed
  {
    // Code size       143 (0x8f)
    .maxstack  4
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldc.i4.3
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  ldtoken    [mscorlib]System.Int32
    IL_0011:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001b:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_0020:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0025:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002a:  ldc.i4.0
    IL_002b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0030:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0035:  ldtoken    method instance string [mscorlib]System.Object::ToString()
    IL_003a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0044:  ldc.i4.0
    IL_0045:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_004a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004f:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0054:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0059:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0063:  ldc.i4.0
    IL_0064:  box        [mscorlib]System.Int32
    IL_0069:  ldtoken    [mscorlib]System.Int32
    IL_006e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0073:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0078:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_007d:  ldc.i4.0
    IL_007e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0083:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0088:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008d:  pop
    IL_008e:  ret
  } // end of method ExpressionTrees::MultipleDots

  .method public hidebysig instance void 
          NestedLambda() cil managed
  {
    // Code size       562 (0x232)
    .maxstack  10
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass14' V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_5)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass14'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate13'
    IL_000d:  brtrue.s   IL_0022

    IL_000f:  ldnull
    IL_0010:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NestedLambda>b__12'(class [mscorlib]System.Func`1<int32>)
    IL_0016:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32>::.ctor(object,
                                                                                                                        native int)
    IL_001b:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate13'
    IL_0020:  br.s       IL_0022

    IL_0022:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate13'
    IL_0027:  stfld      class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass14'::'call'
    IL_002c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0031:  ldloc.0
    IL_0032:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0037:  ldtoken    field class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass14'::'call'
    IL_003c:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0046:  ldc.i4.1
    IL_0047:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_004c:  stloc.1
    IL_004d:  ldloc.1
    IL_004e:  ldc.i4.0
    IL_004f:  ldc.i4.s   42
    IL_0051:  box        [mscorlib]System.Int32
    IL_0056:  ldtoken    [mscorlib]System.Int32
    IL_005b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0060:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0065:  ldc.i4.0
    IL_0066:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_006b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0070:  stelem.ref
    IL_0071:  ldloc.1
    IL_0072:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_0077:  ldc.i4.0
    IL_0078:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0082:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0087:  pop
    IL_0088:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_008d:  ldnull
    IL_008e:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<int32,int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                       class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0093:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0098:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009d:  ldc.i4.2
    IL_009e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00a3:  stloc.1
    IL_00a4:  ldloc.1
    IL_00a5:  ldc.i4.0
    IL_00a6:  ldtoken    [mscorlib]System.Int32
    IL_00ab:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b0:  ldc.i4.2
    IL_00b1:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00b6:  stloc.2
    IL_00b7:  ldloc.2
    IL_00b8:  ldc.i4.0
    IL_00b9:  ldc.i4.s   37
    IL_00bb:  box        [mscorlib]System.Int32
    IL_00c0:  ldtoken    [mscorlib]System.Int32
    IL_00c5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ca:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cf:  stelem.ref
    IL_00d0:  ldloc.2
    IL_00d1:  ldc.i4.1
    IL_00d2:  ldc.i4.s   42
    IL_00d4:  box        [mscorlib]System.Int32
    IL_00d9:  ldtoken    [mscorlib]System.Int32
    IL_00de:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e8:  stelem.ref
    IL_00e9:  ldloc.2
    IL_00ea:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ef:  stelem.ref
    IL_00f0:  ldloc.1
    IL_00f1:  ldc.i4.1
    IL_00f2:  ldtoken    [mscorlib]System.Int32
    IL_00f7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fc:  ldstr      "x"
    IL_0101:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0106:  stloc.3
    IL_0107:  ldloc.3
    IL_0108:  ldc.i4.2
    IL_0109:  box        [mscorlib]System.Int32
    IL_010e:  ldtoken    [mscorlib]System.Int32
    IL_0113:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0118:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_011d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0122:  ldc.i4.1
    IL_0123:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0128:  stloc.s    V_4
    IL_012a:  ldloc.s    V_4
    IL_012c:  ldc.i4.0
    IL_012d:  ldloc.3
    IL_012e:  stelem.ref
    IL_012f:  ldloc.s    V_4
    IL_0131:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0136:  stelem.ref
    IL_0137:  ldloc.1
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
    IL_0169:  stloc.1
    IL_016a:  ldloc.1
    IL_016b:  ldc.i4.0
    IL_016c:  ldtoken    [mscorlib]System.Int32
    IL_0171:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0176:  ldc.i4.2
    IL_0177:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_017c:  stloc.2
    IL_017d:  ldloc.2
    IL_017e:  ldc.i4.0
    IL_017f:  ldc.i4.s   37
    IL_0181:  box        [mscorlib]System.Int32
    IL_0186:  ldtoken    [mscorlib]System.Int32
    IL_018b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0190:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0195:  stelem.ref
    IL_0196:  ldloc.2
    IL_0197:  ldc.i4.1
    IL_0198:  ldc.i4.s   42
    IL_019a:  box        [mscorlib]System.Int32
    IL_019f:  ldtoken    [mscorlib]System.Int32
    IL_01a4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a9:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01ae:  stelem.ref
    IL_01af:  ldloc.2
    IL_01b0:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_01b5:  stelem.ref
    IL_01b6:  ldloc.1
    IL_01b7:  ldc.i4.1
    IL_01b8:  ldtoken    [mscorlib]System.Int32
    IL_01bd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01c2:  ldstr      "x"
    IL_01c7:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01cc:  stloc.3
    IL_01cd:  ldtoken    [mscorlib]System.Int32
    IL_01d2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01d7:  ldstr      "i"
    IL_01dc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01e1:  stloc.s    V_5
    IL_01e3:  ldloc.3
    IL_01e4:  ldc.i4.2
    IL_01e5:  box        [mscorlib]System.Int32
    IL_01ea:  ldtoken    [mscorlib]System.Int32
    IL_01ef:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01f9:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_01fe:  ldc.i4.2
    IL_01ff:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0204:  stloc.s    V_4
    IL_0206:  ldloc.s    V_4
    IL_0208:  ldc.i4.0
    IL_0209:  ldloc.3
    IL_020a:  stelem.ref
    IL_020b:  ldloc.s    V_4
    IL_020d:  ldc.i4.1
    IL_020e:  ldloc.s    V_5
    IL_0210:  stelem.ref
    IL_0211:  ldloc.s    V_4
    IL_0213:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0218:  stelem.ref
    IL_0219:  ldloc.1
    IL_021a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_021f:  ldc.i4.0
    IL_0220:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0225:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                              class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_022a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>(object,
                                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_022f:  pop
    IL_0230:  nop
    IL_0231:  ret
  } // end of method ExpressionTrees::NestedLambda

  .method public hidebysig instance void 
          CurriedLambda() cil managed
  {
    // Code size       140 (0x8c)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_3)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldstr      "a"
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_001a:  stloc.0
    IL_001b:  ldtoken    [mscorlib]System.Int32
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  ldstr      "b"
    IL_002a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002f:  stloc.1
    IL_0030:  ldtoken    [mscorlib]System.Int32
    IL_0035:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003a:  ldstr      "c"
    IL_003f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0044:  stloc.2
    IL_0045:  ldloc.0
    IL_0046:  ldloc.1
    IL_0047:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_004c:  ldloc.2
    IL_004d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0052:  ldc.i4.1
    IL_0053:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0058:  stloc.3
    IL_0059:  ldloc.3
    IL_005a:  ldc.i4.0
    IL_005b:  ldloc.2
    IL_005c:  stelem.ref
    IL_005d:  ldloc.3
    IL_005e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0063:  ldc.i4.1
    IL_0064:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0069:  stloc.3
    IL_006a:  ldloc.3
    IL_006b:  ldc.i4.0
    IL_006c:  ldloc.1
    IL_006d:  stelem.ref
    IL_006e:  ldloc.3
    IL_006f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0074:  ldc.i4.1
    IL_0075:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007a:  stloc.3
    IL_007b:  ldloc.3
    IL_007c:  ldc.i4.0
    IL_007d:  ldloc.0
    IL_007e:  stelem.ref
    IL_007f:  ldloc.3
    IL_0080:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0085:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>(object,
                                                                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_008a:  pop
    IL_008b:  ret
  } // end of method ExpressionTrees::CurriedLambda

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<int32,bool> a) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.s   42
    IL_0004:  callvirt   instance !1 class [mscorlib]System.Func`2<int32,bool>::Invoke(!0)
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method ExpressionTrees::Fizz

  .method private hidebysig instance bool 
          Buzz(class [mscorlib]System.Func`2<int32,bool> a) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.s   42
    IL_0004:  callvirt   instance !1 class [mscorlib]System.Func`2<int32,bool>::Invoke(!0)
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method ExpressionTrees::Buzz

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<string,bool> a) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldstr      "42"
    IL_0007:  callvirt   instance !1 class [mscorlib]System.Func`2<string,bool>::Invoke(!0)
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method ExpressionTrees::Fizz

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool> a) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>::Invoke(!0)
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method ExpressionTrees::Fizz

  .method public hidebysig instance void 
          NestedLambda2() cil managed
  {
    // Code size       1254 (0x4e6)
    .maxstack  12
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldarg.0
    IL_0007:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_000c:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0011:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001b:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<string,bool>)
    IL_0020:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0025:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002a:  ldc.i4.1
    IL_002b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0030:  stloc.0
    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.0
    IL_0033:  ldtoken    [mscorlib]System.String
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  ldstr      "x"
    IL_0042:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0047:  stloc.1
    IL_0048:  ldloc.1
    IL_0049:  ldstr      "a"
    IL_004e:  ldtoken    [mscorlib]System.String
    IL_0053:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0058:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005d:  ldc.i4.0
    IL_005e:  ldtoken    method bool [mscorlib]System.String::op_Equality(string,
                                                                          string)
    IL_0063:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0068:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_0072:  ldc.i4.1
    IL_0073:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0078:  stloc.2
    IL_0079:  ldloc.2
    IL_007a:  ldc.i4.0
    IL_007b:  ldloc.1
    IL_007c:  stelem.ref
    IL_007d:  ldloc.2
    IL_007e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0083:  stelem.ref
    IL_0084:  ldloc.0
    IL_0085:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_008a:  ldc.i4.0
    IL_008b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0090:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0095:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_009a:  pop
    IL_009b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00a0:  ldarg.0
    IL_00a1:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_00a6:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_00ab:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b5:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<string,bool>)
    IL_00ba:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00bf:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00c4:  ldc.i4.1
    IL_00c5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ca:  stloc.0
    IL_00cb:  ldloc.0
    IL_00cc:  ldc.i4.0
    IL_00cd:  ldtoken    [mscorlib]System.String
    IL_00d2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d7:  ldstr      "x"
    IL_00dc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00e1:  stloc.1
    IL_00e2:  ldloc.1
    IL_00e3:  ldstr      "a"
    IL_00e8:  ldtoken    [mscorlib]System.String
    IL_00ed:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00f7:  ldc.i4.0
    IL_00f8:  ldtoken    method bool [mscorlib]System.String::op_Inequality(string,
                                                                            string)
    IL_00fd:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0102:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0107:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               bool,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_010c:  ldc.i4.1
    IL_010d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0112:  stloc.2
    IL_0113:  ldloc.2
    IL_0114:  ldc.i4.0
    IL_0115:  ldloc.1
    IL_0116:  stelem.ref
    IL_0117:  ldloc.2
    IL_0118:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_011d:  stelem.ref
    IL_011e:  ldloc.0
    IL_011f:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0124:  ldc.i4.0
    IL_0125:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_012a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_012f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0134:  pop
    IL_0135:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_013a:  ldarg.0
    IL_013b:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0140:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0145:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_014a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_014f:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>)
    IL_0154:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0159:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_015e:  ldc.i4.1
    IL_015f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0164:  stloc.0
    IL_0165:  ldloc.0
    IL_0166:  ldc.i4.0
    IL_0167:  ldtoken    [mscorlib]System.Action
    IL_016c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0171:  ldstr      "x"
    IL_0176:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_017b:  stloc.1
    IL_017c:  ldloc.1
    IL_017d:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::NestedLambda2()
    IL_0182:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0187:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_018c:  box        [mscorlib]System.Reflection.MethodInfo
    IL_0191:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_0196:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_019b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01a0:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_01a5:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01aa:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01af:  ldc.i4.2
    IL_01b0:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01b5:  stloc.3
    IL_01b6:  ldloc.3
    IL_01b7:  ldc.i4.0
    IL_01b8:  ldtoken    [mscorlib]System.Action
    IL_01bd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01c2:  box        [mscorlib]System.Type
    IL_01c7:  ldtoken    [mscorlib]System.Type
    IL_01cc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01d1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01d6:  stelem.ref
    IL_01d7:  ldloc.3
    IL_01d8:  ldc.i4.1
    IL_01d9:  ldarg.0
    IL_01da:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_01df:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_01e4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01e9:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01ee:  stelem.ref
    IL_01ef:  ldloc.3
    IL_01f0:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01f5:  ldtoken    [mscorlib]System.Action
    IL_01fa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ff:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0204:  ldc.i4.0
    IL_0205:  ldtoken    method bool [mscorlib]System.Delegate::op_Equality(class [mscorlib]System.Delegate,
                                                                            class [mscorlib]System.Delegate)
    IL_020a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_020f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0214:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_0219:  ldc.i4.1
    IL_021a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_021f:  stloc.2
    IL_0220:  ldloc.2
    IL_0221:  ldc.i4.0
    IL_0222:  ldloc.1
    IL_0223:  stelem.ref
    IL_0224:  ldloc.2
    IL_0225:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_022a:  stelem.ref
    IL_022b:  ldloc.0
    IL_022c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0231:  ldc.i4.0
    IL_0232:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0237:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_023c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0241:  pop
    IL_0242:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0247:  ldarg.0
    IL_0248:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_024d:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0252:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0257:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_025c:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>)
    IL_0261:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0266:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_026b:  ldc.i4.1
    IL_026c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0271:  stloc.0
    IL_0272:  ldloc.0
    IL_0273:  ldc.i4.0
    IL_0274:  ldtoken    [mscorlib]System.Action
    IL_0279:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_027e:  ldstr      "x"
    IL_0283:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0288:  stloc.1
    IL_0289:  ldloc.1
    IL_028a:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::NestedLambda2()
    IL_028f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0294:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0299:  box        [mscorlib]System.Reflection.MethodInfo
    IL_029e:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_02a3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02a8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02ad:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_02b2:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_02b7:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_02bc:  ldc.i4.2
    IL_02bd:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_02c2:  stloc.3
    IL_02c3:  ldloc.3
    IL_02c4:  ldc.i4.0
    IL_02c5:  ldtoken    [mscorlib]System.Action
    IL_02ca:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02cf:  box        [mscorlib]System.Type
    IL_02d4:  ldtoken    [mscorlib]System.Type
    IL_02d9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02de:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02e3:  stelem.ref
    IL_02e4:  ldloc.3
    IL_02e5:  ldc.i4.1
    IL_02e6:  ldarg.0
    IL_02e7:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_02ec:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_02f1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02f6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02fb:  stelem.ref
    IL_02fc:  ldloc.3
    IL_02fd:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0302:  ldtoken    [mscorlib]System.Action
    IL_0307:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_030c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0311:  ldc.i4.0
    IL_0312:  ldtoken    method bool [mscorlib]System.Delegate::op_Inequality(class [mscorlib]System.Delegate,
                                                                              class [mscorlib]System.Delegate)
    IL_0317:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_031c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0321:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               bool,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0326:  ldc.i4.1
    IL_0327:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_032c:  stloc.2
    IL_032d:  ldloc.2
    IL_032e:  ldc.i4.0
    IL_032f:  ldloc.1
    IL_0330:  stelem.ref
    IL_0331:  ldloc.2
    IL_0332:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0337:  stelem.ref
    IL_0338:  ldloc.0
    IL_0339:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_033e:  ldc.i4.0
    IL_033f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0344:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0349:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_034e:  pop
    IL_034f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0354:  ldarg.0
    IL_0355:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_035a:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_035f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0364:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0369:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<int32,bool>)
    IL_036e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0373:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0378:  ldc.i4.1
    IL_0379:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_037e:  stloc.0
    IL_037f:  ldloc.0
    IL_0380:  ldc.i4.0
    IL_0381:  ldtoken    [mscorlib]System.Int32
    IL_0386:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_038b:  ldstr      "x"
    IL_0390:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0395:  stloc.1
    IL_0396:  ldloc.1
    IL_0397:  ldc.i4.s   37
    IL_0399:  box        [mscorlib]System.Int32
    IL_039e:  ldtoken    [mscorlib]System.Int32
    IL_03a3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03a8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_03ad:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_03b2:  ldc.i4.1
    IL_03b3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03b8:  stloc.2
    IL_03b9:  ldloc.2
    IL_03ba:  ldc.i4.0
    IL_03bb:  ldloc.1
    IL_03bc:  stelem.ref
    IL_03bd:  ldloc.2
    IL_03be:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03c3:  stelem.ref
    IL_03c4:  ldloc.0
    IL_03c5:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_03ca:  ldc.i4.0
    IL_03cb:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03d0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03d5:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_03da:  pop
    IL_03db:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_03e0:  ldarg.0
    IL_03e1:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_03e6:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_03eb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03f0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_03f5:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<int32,bool>)
    IL_03fa:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_03ff:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0404:  ldc.i4.1
    IL_0405:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_040a:  stloc.0
    IL_040b:  ldloc.0
    IL_040c:  ldc.i4.0
    IL_040d:  ldtoken    [mscorlib]System.Int32
    IL_0412:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0417:  ldstr      "x"
    IL_041c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0421:  stloc.1
    IL_0422:  ldc.i4.1
    IL_0423:  box        [mscorlib]System.Boolean
    IL_0428:  ldtoken    [mscorlib]System.Boolean
    IL_042d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0432:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0437:  ldc.i4.1
    IL_0438:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_043d:  stloc.2
    IL_043e:  ldloc.2
    IL_043f:  ldc.i4.0
    IL_0440:  ldloc.1
    IL_0441:  stelem.ref
    IL_0442:  ldloc.2
    IL_0443:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0448:  stelem.ref
    IL_0449:  ldloc.0
    IL_044a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_044f:  ldc.i4.0
    IL_0450:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0455:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_045a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_045f:  pop
    IL_0460:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0465:  ldarg.0
    IL_0466:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_046b:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0470:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0475:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_047a:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Buzz(class [mscorlib]System.Func`2<int32,bool>)
    IL_047f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0484:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0489:  ldc.i4.1
    IL_048a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_048f:  stloc.0
    IL_0490:  ldloc.0
    IL_0491:  ldc.i4.0
    IL_0492:  ldtoken    [mscorlib]System.Int32
    IL_0497:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_049c:  ldstr      "x"
    IL_04a1:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04a6:  stloc.1
    IL_04a7:  ldc.i4.1
    IL_04a8:  box        [mscorlib]System.Boolean
    IL_04ad:  ldtoken    [mscorlib]System.Boolean
    IL_04b2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04b7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_04bc:  ldc.i4.1
    IL_04bd:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_04c2:  stloc.2
    IL_04c3:  ldloc.2
    IL_04c4:  ldc.i4.0
    IL_04c5:  ldloc.1
    IL_04c6:  stelem.ref
    IL_04c7:  ldloc.2
    IL_04c8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_04cd:  stelem.ref
    IL_04ce:  ldloc.0
    IL_04cf:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_04d4:  ldc.i4.0
    IL_04d5:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_04da:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_04df:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_04e4:  pop
    IL_04e5:  ret
  } // end of method ExpressionTrees::NestedLambda2

  .method public hidebysig instance void 
          NewArrayAndExtensionMethod() cil managed
  {
    // Code size       297 (0x129)
    .maxstack  10
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool [System.Core]System.Linq.Enumerable::SequenceEqual<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  ldtoken    [mscorlib]System.Double
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  ldc.i4.3
    IL_002a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  ldc.i4.0
    IL_0032:  ldc.r8     1.
    IL_003b:  box        [mscorlib]System.Double
    IL_0040:  ldtoken    [mscorlib]System.Double
    IL_0045:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004f:  stelem.ref
    IL_0050:  ldloc.1
    IL_0051:  ldc.i4.1
    IL_0052:  ldc.r8     2.0099999999999998
    IL_005b:  box        [mscorlib]System.Double
    IL_0060:  ldtoken    [mscorlib]System.Double
    IL_0065:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006f:  stelem.ref
    IL_0070:  ldloc.1
    IL_0071:  ldc.i4.2
    IL_0072:  ldc.r8     3.5
    IL_007b:  box        [mscorlib]System.Double
    IL_0080:  ldtoken    [mscorlib]System.Double
    IL_0085:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008f:  stelem.ref
    IL_0090:  ldloc.1
    IL_0091:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0096:  stelem.ref
    IL_0097:  ldloc.0
    IL_0098:  ldc.i4.1
    IL_0099:  ldtoken    [mscorlib]System.Double
    IL_009e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a3:  ldc.i4.3
    IL_00a4:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00a9:  stloc.1
    IL_00aa:  ldloc.1
    IL_00ab:  ldc.i4.0
    IL_00ac:  ldc.r8     1.
    IL_00b5:  box        [mscorlib]System.Double
    IL_00ba:  ldtoken    [mscorlib]System.Double
    IL_00bf:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c9:  stelem.ref
    IL_00ca:  ldloc.1
    IL_00cb:  ldc.i4.1
    IL_00cc:  ldc.r8     2.0099999999999998
    IL_00d5:  box        [mscorlib]System.Double
    IL_00da:  ldtoken    [mscorlib]System.Double
    IL_00df:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e9:  stelem.ref
    IL_00ea:  ldloc.1
    IL_00eb:  ldc.i4.2
    IL_00ec:  ldc.r8     3.5
    IL_00f5:  box        [mscorlib]System.Double
    IL_00fa:  ldtoken    [mscorlib]System.Double
    IL_00ff:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0104:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0109:  stelem.ref
    IL_010a:  ldloc.1
    IL_010b:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0110:  stelem.ref
    IL_0111:  ldloc.0
    IL_0112:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0117:  ldc.i4.0
    IL_0118:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_011d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0122:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0127:  pop
    IL_0128:  ret
  } // end of method ExpressionTrees::NewArrayAndExtensionMethod

  .method public hidebysig instance void 
          NewMultiDimArray() cil managed
  {
    // Code size       141 (0x8d)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldc.i4.2
    IL_0011:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.0
    IL_0019:  ldc.i4.3
    IL_001a:  box        [mscorlib]System.Int32
    IL_001f:  ldtoken    [mscorlib]System.Int32
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002e:  stelem.ref
    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.1
    IL_0031:  ldc.i4.4
    IL_0032:  box        [mscorlib]System.Int32
    IL_0037:  ldtoken    [mscorlib]System.Int32
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0046:  stelem.ref
    IL_0047:  ldloc.0
    IL_0048:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_004d:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_0052:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0057:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0061:  ldc.i4.1
    IL_0062:  box        [mscorlib]System.Int32
    IL_0067:  ldtoken    [mscorlib]System.Int32
    IL_006c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0071:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0076:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_007b:  ldc.i4.0
    IL_007c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0081:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0086:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008b:  pop
    IL_008c:  ret
  } // end of method ExpressionTrees::NewMultiDimArray

  .method public hidebysig instance void 
          NewObject() cil managed
  {
    // Code size       81 (0x51)
    .maxstack  4
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0015:  ldc.i4.0
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0020:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_0025:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002a:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_002f:  ldc.i4.0
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0035:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_003a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_003f:  ldc.i4.0
    IL_0040:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0045:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_004a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_004f:  pop
    IL_0050:  ret
  } // end of method ExpressionTrees::NewObject

  .method public hidebysig instance void 
          NotOperator() cil managed
  {
    // Code size       242 (0xf2)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16'::x
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.3
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16'::y
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.s   42
    IL_0018:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16'::z
    IL_001d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0022:  ldloc.0
    IL_0023:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0028:  ldtoken    field uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16'::z
    IL_002d:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0032:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0037:  ldtoken    [mscorlib]System.Int32
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_004b:  ldc.i4.0
    IL_004c:  box        [mscorlib]System.Int32
    IL_0051:  ldtoken    [mscorlib]System.Int32
    IL_0056:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0060:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0065:  ldc.i4.0
    IL_0066:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_006b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0070:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0075:  pop
    IL_0076:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_007b:  ldloc.0
    IL_007c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0081:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16'::y
    IL_0086:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_008b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0090:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0095:  ldc.i4.0
    IL_0096:  box        [mscorlib]System.Int32
    IL_009b:  ldtoken    [mscorlib]System.Int32
    IL_00a0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00aa:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00af:  ldc.i4.0
    IL_00b0:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b5:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ba:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00bf:  pop
    IL_00c0:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00c5:  ldloc.0
    IL_00c6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00cb:  ldtoken    field bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass16'::x
    IL_00d0:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00d5:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00da:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00df:  ldc.i4.0
    IL_00e0:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e5:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ea:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ef:  pop
    IL_00f0:  nop
    IL_00f1:  ret
  } // end of method ExpressionTrees::NotOperator

  .method public hidebysig instance void 
          ObjectInitializers() cil managed
  {
    // Code size       279 (0x117)
    .maxstack  7
    .locals init (class [System.Xml]System.Xml.XmlReaderSettings V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass19' V_1,
             class [System.Core]System.Linq.Expressions.MemberBinding[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass19'::.ctor()
    IL_0005:  stloc.1
    IL_0006:  nop
    IL_0007:  ldloc.1
    IL_0008:  newobj     instance void [System.Xml]System.Xml.XmlReaderSettings::.ctor()
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.0
    IL_0010:  callvirt   instance void [System.Xml]System.Xml.XmlReaderSettings::set_CloseInput(bool)
    IL_0015:  nop
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.0
    IL_0018:  callvirt   instance void [System.Xml]System.Xml.XmlReaderSettings::set_CheckCharacters(bool)
    IL_001d:  nop
    IL_001e:  ldloc.0
    IL_001f:  stfld      class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass19'::s
    IL_0024:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0029:  ldtoken    method instance void [System.Xml]System.Xml.XmlReaderSettings::.ctor()
    IL_002e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0033:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0038:  ldc.i4.0
    IL_0039:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_003e:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0043:  ldc.i4.2
    IL_0044:  newarr     [System.Core]System.Linq.Expressions.MemberBinding
    IL_0049:  stloc.2
    IL_004a:  ldloc.2
    IL_004b:  ldc.i4.0
    IL_004c:  ldtoken    method instance void [System.Xml]System.Xml.XmlReaderSettings::set_CloseInput(bool)
    IL_0051:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0056:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005b:  ldloc.1
    IL_005c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0061:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass19'::s
    IL_0066:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_006b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0070:  ldtoken    method instance bool [System.Xml]System.Xml.XmlReaderSettings::get_CloseInput()
    IL_0075:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007f:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0084:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_0089:  stelem.ref
    IL_008a:  ldloc.2
    IL_008b:  ldc.i4.1
    IL_008c:  ldtoken    method instance void [System.Xml]System.Xml.XmlReaderSettings::set_CheckCharacters(bool)
    IL_0091:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0096:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009b:  ldloc.1
    IL_009c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00a1:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass19'::s
    IL_00a6:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00ab:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00b0:  ldtoken    method instance bool [System.Xml]System.Xml.XmlReaderSettings::get_CheckCharacters()
    IL_00b5:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00ba:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00bf:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00c4:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_00c9:  stelem.ref
    IL_00ca:  ldloc.2
    IL_00cb:  call       class [System.Core]System.Linq.Expressions.MemberInitExpression [System.Core]System.Linq.Expressions.Expression::MemberInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.MemberBinding[])
    IL_00d0:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_00d5:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00da:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00df:  ldc.i4.1
    IL_00e0:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00e5:  stloc.3
    IL_00e6:  ldloc.3
    IL_00e7:  ldc.i4.0
    IL_00e8:  ldloc.1
    IL_00e9:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00ee:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass19'::s
    IL_00f3:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f8:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00fd:  stelem.ref
    IL_00fe:  ldloc.3
    IL_00ff:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0104:  ldc.i4.0
    IL_0105:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_010a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_010f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0114:  pop
    IL_0115:  nop
    IL_0116:  ret
  } // end of method ExpressionTrees::ObjectInitializers

  .method public hidebysig instance void 
          Quoted() cil managed
  {
    // Code size       181 (0xb5)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldstr      "n"
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_001a:  stloc.0
    IL_001b:  ldtoken    [mscorlib]System.String
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  ldstr      "s"
    IL_002a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  ldloc.0
    IL_0032:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_0037:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0041:  ldc.i4.0
    IL_0042:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0047:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004c:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0051:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0056:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0060:  ldc.i4.2
    IL_0061:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0066:  stloc.2
    IL_0067:  ldloc.2
    IL_0068:  ldc.i4.0
    IL_0069:  ldloc.0
    IL_006a:  stelem.ref
    IL_006b:  ldloc.2
    IL_006c:  ldc.i4.1
    IL_006d:  ldloc.1
    IL_006e:  stelem.ref
    IL_006f:  ldloc.2
    IL_0070:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0075:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_007a:  ldtoken    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<int32,string,string>>
    IL_007f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0084:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0089:  ldnull
    IL_008a:  box        [mscorlib]System.Object
    IL_008f:  ldtoken    [mscorlib]System.Object
    IL_0094:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0099:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00a3:  ldc.i4.0
    IL_00a4:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a9:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ae:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b3:  pop
    IL_00b4:  ret
  } // end of method ExpressionTrees::Quoted

  .method public hidebysig instance void 
          Quoted2() cil managed
  {
    // Code size       175 (0xaf)
    .maxstack  8
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                                   class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  ldnull
    IL_0020:  ldtoken    method object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0025:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002f:  ldc.i4.0
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0035:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_003a:  stelem.ref
    IL_003b:  ldloc.0
    IL_003c:  ldc.i4.1
    IL_003d:  ldc.i4.1
    IL_003e:  box        [mscorlib]System.Boolean
    IL_0043:  ldtoken    [mscorlib]System.Boolean
    IL_0048:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0052:  ldc.i4.0
    IL_0053:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0058:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_0062:  stelem.ref
    IL_0063:  ldloc.0
    IL_0064:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0069:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_006e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0073:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0078:  ldc.i4.1
    IL_0079:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_007e:  stloc.0
    IL_007f:  ldloc.0
    IL_0080:  ldc.i4.0
    IL_0081:  ldnull
    IL_0082:  box        [mscorlib]System.Object
    IL_0087:  ldtoken    [mscorlib]System.Object
    IL_008c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0091:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0096:  stelem.ref
    IL_0097:  ldloc.0
    IL_0098:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_009d:  ldc.i4.0
    IL_009e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a8:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ad:  pop
    IL_00ae:  ret
  } // end of method ExpressionTrees::Quoted2

  .method public hidebysig instance void 
          QuotedWithAnonymous() cil managed
  {
    // Code size       371 (0x173)
    .maxstack  18
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3,
             class [mscorlib]System.Reflection.MethodInfo[] V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_5,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_6)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method !!0 [System.Core]System.Linq.Enumerable::Single<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.1
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  ldnull
    IL_0020:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType1`2'<string,string>,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                               class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0025:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002f:  ldc.i4.2
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0035:  stloc.1
    IL_0036:  ldloc.1
    IL_0037:  ldc.i4.0
    IL_0038:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_003d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0042:  ldc.i4.1
    IL_0043:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0048:  stloc.2
    IL_0049:  ldloc.2
    IL_004a:  ldc.i4.0
    IL_004b:  ldtoken    method instance void class '<>f__AnonymousType1`2'<string,string>::.ctor(!0,
                                                                                                  !1)
    IL_0050:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_0055:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005a:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_005f:  ldc.i4.2
    IL_0060:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0065:  stloc.3
    IL_0066:  ldloc.3
    IL_0067:  ldc.i4.0
    IL_0068:  ldstr      "a"
    IL_006d:  ldtoken    [mscorlib]System.String
    IL_0072:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0077:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007c:  stelem.ref
    IL_007d:  ldloc.3
    IL_007e:  ldc.i4.1
    IL_007f:  ldstr      "b"
    IL_0084:  ldtoken    [mscorlib]System.String
    IL_0089:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0093:  stelem.ref
    IL_0094:  ldloc.3
    IL_0095:  ldc.i4.2
    IL_0096:  newarr     [mscorlib]System.Reflection.MethodInfo
    IL_009b:  stloc.s    V_4
    IL_009d:  ldloc.s    V_4
    IL_009f:  ldc.i4.0
    IL_00a0:  ldtoken    method instance !0 class '<>f__AnonymousType1`2'<string,string>::get_X()
    IL_00a5:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00aa:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00af:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00b4:  stelem.ref
    IL_00b5:  ldloc.s    V_4
    IL_00b7:  ldc.i4.1
    IL_00b8:  ldtoken    method instance !1 class '<>f__AnonymousType1`2'<string,string>::get_Y()
    IL_00bd:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00c2:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00cc:  stelem.ref
    IL_00cd:  ldloc.s    V_4
    IL_00cf:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_00d4:  stelem.ref
    IL_00d5:  ldloc.2
    IL_00d6:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00db:  stelem.ref
    IL_00dc:  ldloc.1
    IL_00dd:  ldc.i4.1
    IL_00de:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00e3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e8:  ldstr      "o"
    IL_00ed:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00f2:  stloc.s    V_5
    IL_00f4:  ldloc.s    V_5
    IL_00f6:  ldtoken    method instance !0 class '<>f__AnonymousType1`2'<string,string>::get_X()
    IL_00fb:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_0100:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0105:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_010a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_010f:  ldloc.s    V_5
    IL_0111:  ldtoken    method instance !1 class '<>f__AnonymousType1`2'<string,string>::get_Y()
    IL_0116:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_011b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0120:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0125:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_012a:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_012f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0134:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0139:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_013e:  ldc.i4.1
    IL_013f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0144:  stloc.s    V_6
    IL_0146:  ldloc.s    V_6
    IL_0148:  ldc.i4.0
    IL_0149:  ldloc.s    V_5
    IL_014b:  stelem.ref
    IL_014c:  ldloc.s    V_6
    IL_014e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<string,string>,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0153:  stelem.ref
    IL_0154:  ldloc.1
    IL_0155:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_015a:  stelem.ref
    IL_015b:  ldloc.0
    IL_015c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0161:  ldc.i4.0
    IL_0162:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0167:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_016c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0171:  pop
    IL_0172:  ret
  } // end of method ExpressionTrees::QuotedWithAnonymous

  .method public hidebysig instance void 
          StaticCall() cil managed
  {
    // Code size       131 (0x83)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  ldc.i4.3
    IL_0020:  box        [mscorlib]System.Int32
    IL_0025:  ldtoken    [mscorlib]System.Int32
    IL_002a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0034:  ldtoken    [mscorlib]System.Object
    IL_0039:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0043:  stelem.ref
    IL_0044:  ldloc.0
    IL_0045:  ldc.i4.1
    IL_0046:  ldc.i4.0
    IL_0047:  box        [mscorlib]System.Int32
    IL_004c:  ldtoken    [mscorlib]System.Int32
    IL_0051:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0056:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005b:  ldtoken    [mscorlib]System.Object
    IL_0060:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0065:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_006a:  stelem.ref
    IL_006b:  ldloc.0
    IL_006c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0071:  ldc.i4.0
    IL_0072:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0077:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0081:  pop
    IL_0082:  ret
  } // end of method ExpressionTrees::StaticCall

  .method public hidebysig instance void 
          ThisCall() cil managed
  {
    // Code size       117 (0x75)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldarg.0
    IL_0007:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_000c:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0011:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001b:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_0020:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0025:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002a:  ldc.i4.1
    IL_002b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0030:  stloc.0
    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.0
    IL_0033:  ldc.i4.3
    IL_0034:  box        [mscorlib]System.Int32
    IL_0039:  ldtoken    [mscorlib]System.Int32
    IL_003e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0043:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0048:  ldtoken    [mscorlib]System.Object
    IL_004d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0052:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0057:  stelem.ref
    IL_0058:  ldloc.0
    IL_0059:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_005e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0063:  ldc.i4.0
    IL_0064:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0069:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0073:  pop
    IL_0074:  ret
  } // end of method ExpressionTrees::ThisCall

  .method public hidebysig instance void 
          ThisExplicit() cil managed
  {
    // Code size       116 (0x74)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  ldarg.0
    IL_0020:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0025:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_002a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0034:  stelem.ref
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.1
    IL_0037:  ldc.i4.3
    IL_0038:  box        [mscorlib]System.Int32
    IL_003d:  ldtoken    [mscorlib]System.Int32
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
  } // end of method ExpressionTrees::ThisExplicit

  .method public hidebysig instance void 
          TypedConstant() cil managed
  {
    // Code size       113 (0x71)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Type
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldc.i4.2
    IL_0011:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.0
    IL_0019:  ldtoken    [mscorlib]System.Int32
    IL_001e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0023:  box        [mscorlib]System.Type
    IL_0028:  ldtoken    [mscorlib]System.Type
    IL_002d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0032:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0037:  stelem.ref
    IL_0038:  ldloc.0
    IL_0039:  ldc.i4.1
    IL_003a:  ldtoken    [mscorlib]System.String
    IL_003f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0044:  box        [mscorlib]System.Type
    IL_0049:  ldtoken    [mscorlib]System.Type
    IL_004e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0053:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0058:  stelem.ref
    IL_0059:  ldloc.0
    IL_005a:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_005f:  ldc.i4.0
    IL_0060:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0065:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Type[]>(object,
                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006f:  pop
    IL_0070:  ret
  } // end of method ExpressionTrees::TypedConstant

  .method public hidebysig instance void 
          StaticCallImplicitCast() cil managed
  {
    // Code size       131 (0x83)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  ldc.i4.3
    IL_0020:  box        [mscorlib]System.Int32
    IL_0025:  ldtoken    [mscorlib]System.Int32
    IL_002a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0034:  ldtoken    [mscorlib]System.Object
    IL_0039:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0043:  stelem.ref
    IL_0044:  ldloc.0
    IL_0045:  ldc.i4.1
    IL_0046:  ldc.i4.0
    IL_0047:  box        [mscorlib]System.Int32
    IL_004c:  ldtoken    [mscorlib]System.Int32
    IL_0051:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0056:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005b:  ldtoken    [mscorlib]System.Object
    IL_0060:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0065:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_006a:  stelem.ref
    IL_006b:  ldloc.0
    IL_006c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0071:  ldc.i4.0
    IL_0072:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0077:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0081:  pop
    IL_0082:  ret
  } // end of method ExpressionTrees::StaticCallImplicitCast

  .method public hidebysig instance void 
          StaticMembers() cil managed
  {
    // Code size       235 (0xeb)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_001b:  ldnull
    IL_001c:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0021:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0026:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0030:  ldnull
    IL_0031:  ldtoken    method valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromMilliseconds(float64)
    IL_0036:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0040:  ldc.i4.1
    IL_0041:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0046:  stloc.0
    IL_0047:  ldloc.0
    IL_0048:  ldc.i4.0
    IL_0049:  ldc.r8     10.000999999999999
    IL_0052:  box        [mscorlib]System.Double
    IL_0057:  ldtoken    [mscorlib]System.Double
    IL_005c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0061:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0066:  stelem.ref
    IL_0067:  ldloc.0
    IL_0068:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_006d:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::op_Addition(valuetype [mscorlib]System.DateTime,
                                                                                                           valuetype [mscorlib]System.TimeSpan)
    IL_0072:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0077:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0081:  ldc.i4.0
    IL_0082:  ldtoken    method bool [mscorlib]System.DateTime::op_GreaterThan(valuetype [mscorlib]System.DateTime,
                                                                               valuetype [mscorlib]System.DateTime)
    IL_0087:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_008c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0091:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  bool,
                                                                                                                                                  class [mscorlib]System.Reflection.MethodInfo)
    IL_0096:  ldtoken    method instance string [mscorlib]System.Boolean::ToString()
    IL_009b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00a0:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a5:  ldc.i4.0
    IL_00a6:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ab:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00b0:  ldstr      "False"
    IL_00b5:  ldtoken    [mscorlib]System.String
    IL_00ba:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bf:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c4:  ldc.i4.0
    IL_00c5:  ldtoken    method bool [mscorlib]System.String::op_Equality(string,
                                                                          string)
    IL_00ca:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00cf:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00d4:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_00d9:  ldc.i4.0
    IL_00da:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00df:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e4:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00e9:  pop
    IL_00ea:  ret
  } // end of method ExpressionTrees::StaticMembers

  .method public hidebysig instance void 
          Strings() cil managed
  {
    // Code size       378 (0x17a)
    .maxstack  8
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass1b' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass1b'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass1b'::i
    IL_000e:  ldloc.0
    IL_000f:  ldstr      "X"
    IL_0014:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass1b'::x
    IL_0019:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_001e:  ldstr      "a\n\\b"
    IL_0023:  ldtoken    [mscorlib]System.String
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  ldloc.0
    IL_0033:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0038:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass1b'::x
    IL_003d:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0047:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_004c:  ldloc.0
    IL_004d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0052:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass1b'::x
    IL_0057:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_005c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0061:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0066:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_006b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0070:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0075:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_007a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0084:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0089:  ldc.i4.2
    IL_008a:  box        [mscorlib]System.Int32
    IL_008f:  ldtoken    [mscorlib]System.Int32
    IL_0094:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0099:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00a3:  ldc.i4.0
    IL_00a4:  box        [mscorlib]System.Boolean
    IL_00a9:  ldtoken    [mscorlib]System.Boolean
    IL_00ae:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b8:  ldc.i4.1
    IL_00b9:  box        [mscorlib]System.Boolean
    IL_00be:  ldtoken    [mscorlib]System.Boolean
    IL_00c3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cd:  ldc.i4.1
    IL_00ce:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_00d3:  box        [mscorlib]System.Decimal
    IL_00d8:  ldtoken    [mscorlib]System.Decimal
    IL_00dd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e7:  ldloc.0
    IL_00e8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00ed:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass1b'::i
    IL_00f2:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_0101:  ldtoken    [mscorlib]System.Decimal
    IL_0106:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010b:  ldtoken    method valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
    IL_0110:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0115:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_011a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type,
                                                                                                                                             class [mscorlib]System.Reflection.MethodInfo)
    IL_011f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0124:  ldc.i4.0
    IL_0125:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_012a:  box        [mscorlib]System.Decimal
    IL_012f:  ldtoken    [mscorlib]System.Decimal
    IL_0134:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0139:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_013e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0143:  ldc.i4.0
    IL_0144:  box        [mscorlib]System.Boolean
    IL_0149:  ldtoken    [mscorlib]System.Boolean
    IL_014e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0153:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0158:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_015d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0162:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0167:  ldc.i4.0
    IL_0168:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_016d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0172:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0177:  pop
    IL_0178:  nop
    IL_0179:  ret
  } // end of method ExpressionTrees::Strings

  .method public hidebysig instance void 
          GenericClassInstance() cil managed
  {
    // Code size       151 (0x97)
    .maxstack  5
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::.ctor()
    IL_000b:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_001a:  ldc.i4.0
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0025:  ldtoken    field !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::InstanceField
    IL_002a:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_002f:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle,
                                                                                                                               valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0034:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0039:  ldtoken    [mscorlib]System.Double
    IL_003e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0043:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0048:  ldtoken    method instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>::.ctor()
    IL_004d:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>
    IL_0052:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_005c:  ldc.i4.0
    IL_005d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0062:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0067:  ldtoken    method instance !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>::get_InstanceProperty()
    IL_006c:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>
    IL_0071:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0076:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0080:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0085:  ldc.i4.0
    IL_0086:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_008b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<float64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0090:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<float64>(object,
                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0095:  pop
    IL_0096:  ret
  } // end of method ExpressionTrees::GenericClassInstance

  .method public hidebysig instance void 
          GenericClassStatic() cil managed
  {
    // Code size       91 (0x5b)
    .maxstack  5
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    field !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::StaticField
    IL_000c:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_0011:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle,
                                                                                                                               valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_001b:  ldtoken    [mscorlib]System.Double
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_002a:  ldnull
    IL_002b:  ldtoken    method !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>::get_StaticProperty()
    IL_0030:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>
    IL_0035:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_003f:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0044:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0049:  ldc.i4.0
    IL_004a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<float64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0054:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<float64>(object,
                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0059:  pop
    IL_005a:  ret
  } // end of method ExpressionTrees::GenericClassStatic

  .method public hidebysig instance void 
          InvokeGenericMethod() cil managed
  {
    // Code size       56 (0x38)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::GenericMethod<float64>()
    IL_000c:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_0011:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_001b:  ldc.i4.0
    IL_001c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0021:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0026:  ldc.i4.0
    IL_0027:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0031:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0036:  pop
    IL_0037:  ret
  } // end of method ExpressionTrees::InvokeGenericMethod

  .method private hidebysig static void  Test<T>(!!T delegateExpression,
                                                 class [System.Core]System.Linq.Expressions.Expression`1<!!T> expressionTree) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method ExpressionTrees::Test

  .method public hidebysig static void  ArrayIndexer() cil managed
  {
    // Code size       623 (0x26f)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate22'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayIndexer>b__1d'(int32[])
    IL_000f:  newobj     instance void class [mscorlib]System.Func`2<int32[],int32>::.ctor(object,
                                                                                           native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate22'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate22'
    IL_0020:  ldtoken    int32[]
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "array"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.0
    IL_0037:  box        [mscorlib]System.Int32
    IL_003c:  ldtoken    [mscorlib]System.Int32
    IL_0041:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_0050:  ldc.i4.1
    IL_0051:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0056:  stloc.1
    IL_0057:  ldloc.1
    IL_0058:  ldc.i4.0
    IL_0059:  ldloc.0
    IL_005a:  stelem.ref
    IL_005b:  ldloc.1
    IL_005c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32[],int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0061:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32[],int32>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0066:  nop
    IL_0067:  ldsfld     class [mscorlib]System.Func`3<int32[],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate23'
    IL_006c:  brtrue.s   IL_0081

    IL_006e:  ldnull
    IL_006f:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayIndexer>b__1e'(int32[],
                                                                                                                    int32)
    IL_0075:  newobj     instance void class [mscorlib]System.Func`3<int32[],int32,int32>::.ctor(object,
                                                                                                 native int)
    IL_007a:  stsfld     class [mscorlib]System.Func`3<int32[],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate23'
    IL_007f:  br.s       IL_0081

    IL_0081:  ldsfld     class [mscorlib]System.Func`3<int32[],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate23'
    IL_0086:  ldtoken    int32[]
    IL_008b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0090:  ldstr      "array"
    IL_0095:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_009a:  stloc.0
    IL_009b:  ldtoken    [mscorlib]System.Int32
    IL_00a0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a5:  ldstr      "index"
    IL_00aa:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00af:  stloc.2
    IL_00b0:  ldloc.0
    IL_00b1:  ldloc.2
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_00b7:  ldc.i4.2
    IL_00b8:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00bd:  stloc.1
    IL_00be:  ldloc.1
    IL_00bf:  ldc.i4.0
    IL_00c0:  ldloc.0
    IL_00c1:  stelem.ref
    IL_00c2:  ldloc.1
    IL_00c3:  ldc.i4.1
    IL_00c4:  ldloc.2
    IL_00c5:  stelem.ref
    IL_00c6:  ldloc.1
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32[],int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00cc:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32[],int32,int32>>(!!0,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00d1:  nop
    IL_00d2:  ldsfld     class [mscorlib]System.Func`2<int32[0...,0...],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate24'
    IL_00d7:  brtrue.s   IL_00ec

    IL_00d9:  ldnull
    IL_00da:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayIndexer>b__1f'(int32[0...,0...])
    IL_00e0:  newobj     instance void class [mscorlib]System.Func`2<int32[0...,0...],int32>::.ctor(object,
                                                                                                    native int)
    IL_00e5:  stsfld     class [mscorlib]System.Func`2<int32[0...,0...],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate24'
    IL_00ea:  br.s       IL_00ec

    IL_00ec:  ldsfld     class [mscorlib]System.Func`2<int32[0...,0...],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate24'
    IL_00f1:  ldtoken    int32[0...,0...]
    IL_00f6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fb:  ldstr      "array"
    IL_0100:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0105:  stloc.0
    IL_0106:  ldloc.0
    IL_0107:  ldc.i4.2
    IL_0108:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_010d:  stloc.3
    IL_010e:  ldloc.3
    IL_010f:  ldc.i4.0
    IL_0110:  ldc.i4.0
    IL_0111:  box        [mscorlib]System.Int32
    IL_0116:  ldtoken    [mscorlib]System.Int32
    IL_011b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0120:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0125:  stelem.ref
    IL_0126:  ldloc.3
    IL_0127:  ldc.i4.1
    IL_0128:  ldc.i4.5
    IL_0129:  box        [mscorlib]System.Int32
    IL_012e:  ldtoken    [mscorlib]System.Int32
    IL_0133:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0138:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_013d:  stelem.ref
    IL_013e:  ldloc.3
    IL_013f:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0144:  ldc.i4.1
    IL_0145:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_014a:  stloc.1
    IL_014b:  ldloc.1
    IL_014c:  ldc.i4.0
    IL_014d:  ldloc.0
    IL_014e:  stelem.ref
    IL_014f:  ldloc.1
    IL_0150:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32[0...,0...],int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0155:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32[0...,0...],int32>>(!!0,
                                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_015a:  nop
    IL_015b:  ldsfld     class [mscorlib]System.Func`3<int32[0...,0...],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate25'
    IL_0160:  brtrue.s   IL_0175

    IL_0162:  ldnull
    IL_0163:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayIndexer>b__20'(int32[0...,0...],
                                                                                                                    int32)
    IL_0169:  newobj     instance void class [mscorlib]System.Func`3<int32[0...,0...],int32,int32>::.ctor(object,
                                                                                                          native int)
    IL_016e:  stsfld     class [mscorlib]System.Func`3<int32[0...,0...],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate25'
    IL_0173:  br.s       IL_0175

    IL_0175:  ldsfld     class [mscorlib]System.Func`3<int32[0...,0...],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate25'
    IL_017a:  ldtoken    int32[0...,0...]
    IL_017f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0184:  ldstr      "array"
    IL_0189:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_018e:  stloc.0
    IL_018f:  ldtoken    [mscorlib]System.Int32
    IL_0194:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0199:  ldstr      "index"
    IL_019e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01a3:  stloc.2
    IL_01a4:  ldloc.0
    IL_01a5:  ldc.i4.2
    IL_01a6:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01ab:  stloc.3
    IL_01ac:  ldloc.3
    IL_01ad:  ldc.i4.0
    IL_01ae:  ldloc.2
    IL_01af:  stelem.ref
    IL_01b0:  ldloc.3
    IL_01b1:  ldc.i4.1
    IL_01b2:  ldc.i4.7
    IL_01b3:  box        [mscorlib]System.Int32
    IL_01b8:  ldtoken    [mscorlib]System.Int32
    IL_01bd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01c2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01c7:  stelem.ref
    IL_01c8:  ldloc.3
    IL_01c9:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_01ce:  ldc.i4.2
    IL_01cf:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01d4:  stloc.1
    IL_01d5:  ldloc.1
    IL_01d6:  ldc.i4.0
    IL_01d7:  ldloc.0
    IL_01d8:  stelem.ref
    IL_01d9:  ldloc.1
    IL_01da:  ldc.i4.1
    IL_01db:  ldloc.2
    IL_01dc:  stelem.ref
    IL_01dd:  ldloc.1
    IL_01de:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32[0...,0...],int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                           class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01e3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32[0...,0...],int32,int32>>(!!0,
                                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01e8:  nop
    IL_01e9:  ldsfld     class [mscorlib]System.Func`3<int32[][],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate26'
    IL_01ee:  brtrue.s   IL_0203

    IL_01f0:  ldnull
    IL_01f1:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayIndexer>b__21'(int32[][],
                                                                                                                    int32)
    IL_01f7:  newobj     instance void class [mscorlib]System.Func`3<int32[][],int32,int32>::.ctor(object,
                                                                                                   native int)
    IL_01fc:  stsfld     class [mscorlib]System.Func`3<int32[][],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate26'
    IL_0201:  br.s       IL_0203

    IL_0203:  ldsfld     class [mscorlib]System.Func`3<int32[][],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate26'
    IL_0208:  ldtoken    int32[][]
    IL_020d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0212:  ldstr      "array"
    IL_0217:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_021c:  stloc.0
    IL_021d:  ldtoken    [mscorlib]System.Int32
    IL_0222:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0227:  ldstr      "index"
    IL_022c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0231:  stloc.2
    IL_0232:  ldloc.0
    IL_0233:  ldloc.2
    IL_0234:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_0239:  ldc.i4.7
    IL_023a:  box        [mscorlib]System.Int32
    IL_023f:  ldtoken    [mscorlib]System.Int32
    IL_0244:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0249:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_024e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_0253:  ldc.i4.2
    IL_0254:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0259:  stloc.1
    IL_025a:  ldloc.1
    IL_025b:  ldc.i4.0
    IL_025c:  ldloc.0
    IL_025d:  stelem.ref
    IL_025e:  ldloc.1
    IL_025f:  ldc.i4.1
    IL_0260:  ldloc.2
    IL_0261:  stelem.ref
    IL_0262:  ldloc.1
    IL_0263:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32[][],int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0268:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32[][],int32,int32>>(!!0,
                                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_026d:  nop
    IL_026e:  ret
  } // end of method ExpressionTrees::ArrayIndexer

  .method public hidebysig static void  ArrayLength() cil managed
  {
    // Code size       172 (0xac)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate29'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayLength>b__27'(int32[])
    IL_000f:  newobj     instance void class [mscorlib]System.Func`2<int32[],int32>::.ctor(object,
                                                                                           native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate29'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate29'
    IL_0020:  ldtoken    int32[]
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "array"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayLength(class [System.Core]System.Linq.Expressions.Expression)
    IL_003b:  ldc.i4.1
    IL_003c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0041:  stloc.1
    IL_0042:  ldloc.1
    IL_0043:  ldc.i4.0
    IL_0044:  ldloc.0
    IL_0045:  stelem.ref
    IL_0046:  ldloc.1
    IL_0047:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32[],int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_004c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32[],int32>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0051:  nop
    IL_0052:  ldsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate2a'
    IL_0057:  brtrue.s   IL_006c

    IL_0059:  ldnull
    IL_005a:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayLength>b__28'()
    IL_0060:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0065:  stsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate2a'
    IL_006a:  br.s       IL_006c

    IL_006c:  ldsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate2a'
    IL_0071:  ldnull
    IL_0072:  box        [mscorlib]System.Array
    IL_0077:  ldtoken    [mscorlib]System.Array
    IL_007c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0081:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0086:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_008b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0090:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0095:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_009a:  ldc.i4.0
    IL_009b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a5:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32>>(!!0,
                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00aa:  nop
    IL_00ab:  ret
  } // end of method ExpressionTrees::ArrayLength

  .method public hidebysig static void  NewObj() cil managed
  {
    // Code size       613 (0x265)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate32'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NewObj>b__2b'()
    IL_000f:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate32'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate32'
    IL_0020:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::.ctor()
    IL_0025:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002a:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_002f:  ldc.i4.0
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0035:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_003a:  ldc.i4.0
    IL_003b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0040:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0045:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_004a:  nop
    IL_004b:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate33'
    IL_0050:  brtrue.s   IL_0065

    IL_0052:  ldnull
    IL_0053:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NewObj>b__2c'()
    IL_0059:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_005e:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate33'
    IL_0063:  br.s       IL_0065

    IL_0065:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate33'
    IL_006a:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithCtor::.ctor(int32)
    IL_006f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0074:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0079:  ldc.i4.1
    IL_007a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_007f:  stloc.0
    IL_0080:  ldloc.0
    IL_0081:  ldc.i4.0
    IL_0082:  ldc.i4.5
    IL_0083:  box        [mscorlib]System.Int32
    IL_0088:  ldtoken    [mscorlib]System.Int32
    IL_008d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0092:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0097:  stelem.ref
    IL_0098:  ldloc.0
    IL_0099:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_009e:  ldc.i4.0
    IL_009f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a9:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00ae:  nop
    IL_00af:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate34'
    IL_00b4:  brtrue.s   IL_00c9

    IL_00b6:  ldnull
    IL_00b7:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NewObj>b__2d'()
    IL_00bd:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_00c2:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate34'
    IL_00c7:  br.s       IL_00c9

    IL_00c9:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate34'
    IL_00ce:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors::.ctor()
    IL_00d3:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00d8:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_00dd:  ldc.i4.0
    IL_00de:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00e3:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_00e8:  ldc.i4.0
    IL_00e9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00ee:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00f3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00f8:  nop
    IL_00f9:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate35'
    IL_00fe:  brtrue.s   IL_0113

    IL_0100:  ldnull
    IL_0101:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NewObj>b__2e'()
    IL_0107:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_010c:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate35'
    IL_0111:  br.s       IL_0113

    IL_0113:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate35'
    IL_0118:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors::.ctor(int32)
    IL_011d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0122:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0127:  ldc.i4.1
    IL_0128:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_012d:  stloc.0
    IL_012e:  ldloc.0
    IL_012f:  ldc.i4.0
    IL_0130:  ldc.i4.5
    IL_0131:  box        [mscorlib]System.Int32
    IL_0136:  ldtoken    [mscorlib]System.Int32
    IL_013b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0140:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0145:  stelem.ref
    IL_0146:  ldloc.0
    IL_0147:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_014c:  ldc.i4.0
    IL_014d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0152:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0157:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_015c:  nop
    IL_015d:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate36'
    IL_0162:  brtrue.s   IL_0177

    IL_0164:  ldnull
    IL_0165:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NewObj>b__2f'()
    IL_016b:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_0170:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate36'
    IL_0175:  br.s       IL_0177

    IL_0177:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate36'
    IL_017c:  ldtoken    method instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::.ctor()
    IL_0181:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_0186:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_018b:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0190:  ldc.i4.0
    IL_0191:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0196:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_019b:  ldc.i4.0
    IL_019c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01a1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01a6:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01ab:  nop
    IL_01ac:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate37'
    IL_01b1:  brtrue.s   IL_01c6

    IL_01b3:  ldnull
    IL_01b4:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NewObj>b__30'()
    IL_01ba:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_01bf:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate37'
    IL_01c4:  br.s       IL_01c6

    IL_01c6:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate37'
    IL_01cb:  ldtoken    method instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1<int32>::.ctor()
    IL_01d0:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1<int32>
    IL_01d5:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01da:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_01df:  ldc.i4.0
    IL_01e0:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01e5:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_01ea:  ldc.i4.0
    IL_01eb:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01f0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01f5:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01fa:  nop
    IL_01fb:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate38'
    IL_0200:  brtrue.s   IL_0215

    IL_0202:  ldnull
    IL_0203:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<NewObj>b__31'()
    IL_0209:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_020e:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate38'
    IL_0213:  br.s       IL_0215

    IL_0215:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate38'
    IL_021a:  ldtoken    method instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1<int32>::.ctor(int32)
    IL_021f:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1<int32>
    IL_0224:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0229:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_022e:  ldc.i4.1
    IL_022f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0234:  stloc.0
    IL_0235:  ldloc.0
    IL_0236:  ldc.i4.0
    IL_0237:  ldc.i4.5
    IL_0238:  box        [mscorlib]System.Int32
    IL_023d:  ldtoken    [mscorlib]System.Int32
    IL_0242:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0247:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_024c:  stelem.ref
    IL_024d:  ldloc.0
    IL_024e:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0253:  ldc.i4.0
    IL_0254:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0259:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_025e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0263:  nop
    IL_0264:  ret
  } // end of method ExpressionTrees::NewObj

  .method public hidebysig static void  TypeOfExpr() cil managed
  {
    // Code size       392 (0x188)
    .maxstack  3
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate3e'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<TypeOfExpr>b__39'()
    IL_000f:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate3e'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate3e'
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  box        [mscorlib]System.Type
    IL_002f:  ldtoken    [mscorlib]System.Type
    IL_0034:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0039:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_003e:  ldc.i4.0
    IL_003f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0044:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0049:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_004e:  nop
    IL_004f:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate3f'
    IL_0054:  brtrue.s   IL_0069

    IL_0056:  ldnull
    IL_0057:  ldftn      class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<TypeOfExpr>b__3a'()
    IL_005d:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_0062:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate3f'
    IL_0067:  br.s       IL_0069

    IL_0069:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate3f'
    IL_006e:  ldtoken    [mscorlib]System.Object
    IL_0073:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0078:  box        [mscorlib]System.Type
    IL_007d:  ldtoken    [mscorlib]System.Type
    IL_0082:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0087:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008c:  ldc.i4.0
    IL_008d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0092:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0097:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_009c:  nop
    IL_009d:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate40'
    IL_00a2:  brtrue.s   IL_00b7

    IL_00a4:  ldnull
    IL_00a5:  ldftn      class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<TypeOfExpr>b__3b'()
    IL_00ab:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_00b0:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate40'
    IL_00b5:  br.s       IL_00b7

    IL_00b7:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate40'
    IL_00bc:  ldtoken    [mscorlib]System.Collections.Generic.List`1
    IL_00c1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c6:  box        [mscorlib]System.Type
    IL_00cb:  ldtoken    [mscorlib]System.Type
    IL_00d0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00da:  ldc.i4.0
    IL_00db:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e5:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00ea:  nop
    IL_00eb:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate41'
    IL_00f0:  brtrue.s   IL_0105

    IL_00f2:  ldnull
    IL_00f3:  ldftn      class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<TypeOfExpr>b__3c'()
    IL_00f9:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_00fe:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate41'
    IL_0103:  br.s       IL_0105

    IL_0105:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate41'
    IL_010a:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_010f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0114:  box        [mscorlib]System.Type
    IL_0119:  ldtoken    [mscorlib]System.Type
    IL_011e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0123:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0128:  ldc.i4.0
    IL_0129:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_012e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0133:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0138:  nop
    IL_0139:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate42'
    IL_013e:  brtrue.s   IL_0153

    IL_0140:  ldnull
    IL_0141:  ldftn      class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<TypeOfExpr>b__3d'()
    IL_0147:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_014c:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate42'
    IL_0151:  br.s       IL_0153

    IL_0153:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate42'
    IL_0158:  ldtoken    int32*
    IL_015d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0162:  box        [mscorlib]System.Type
    IL_0167:  ldtoken    [mscorlib]System.Type
    IL_016c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0171:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0176:  ldc.i4.0
    IL_0177:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_017c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0181:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0186:  nop
    IL_0187:  ret
  } // end of method ExpressionTrees::TypeOfExpr

  .method public hidebysig static void  AsTypeExpr() cil managed
  {
    // Code size       184 (0xb8)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate45'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<AsTypeExpr>b__43'(object)
    IL_000f:  newobj     instance void class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass>::.ctor(object,
                                                                                                                                                native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate45'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate45'
    IL_0020:  ldtoken    [mscorlib]System.Object
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "obj"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::TypeAs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Type)
    IL_0045:  ldc.i4.1
    IL_0046:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004b:  stloc.1
    IL_004c:  ldloc.1
    IL_004d:  ldc.i4.0
    IL_004e:  ldloc.0
    IL_004f:  stelem.ref
    IL_0050:  ldloc.1
    IL_0051:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0056:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass>>(!!0,
                                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_005b:  nop
    IL_005c:  ldsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate46'
    IL_0061:  brtrue.s   IL_0076

    IL_0063:  ldnull
    IL_0064:  ldftn      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<AsTypeExpr>b__44'(object)
    IL_006a:  newobj     instance void class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>>::.ctor(object,
                                                                                                                                                                               native int)
    IL_006f:  stsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate46'
    IL_0074:  br.s       IL_0076

    IL_0076:  ldsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate46'
    IL_007b:  ldtoken    [mscorlib]System.Object
    IL_0080:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0085:  ldstr      "obj"
    IL_008a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_008f:  stloc.0
    IL_0090:  ldloc.0
    IL_0091:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>
    IL_0096:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009b:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::TypeAs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Type)
    IL_00a0:  ldc.i4.1
    IL_00a1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a6:  stloc.1
    IL_00a7:  ldloc.1
    IL_00a8:  ldc.i4.0
    IL_00a9:  ldloc.0
    IL_00aa:  stelem.ref
    IL_00ab:  ldloc.1
    IL_00ac:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b1:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>>>(!!0,
                                                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00b6:  nop
    IL_00b7:  ret
  } // end of method ExpressionTrees::AsTypeExpr

  .method public hidebysig static void  IsTypeExpr() cil managed
  {
    // Code size       93 (0x5d)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<object,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate48'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<IsTypeExpr>b__47'(object)
    IL_000f:  newobj     instance void class [mscorlib]System.Func`2<object,bool>::.ctor(object,
                                                                                         native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`2<object,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate48'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`2<object,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate48'
    IL_0020:  ldtoken    [mscorlib]System.Object
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "obj"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.TypeBinaryExpression [System.Core]System.Linq.Expressions.Expression::TypeIs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0045:  ldc.i4.1
    IL_0046:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004b:  stloc.1
    IL_004c:  ldloc.1
    IL_004d:  ldc.i4.0
    IL_004e:  ldloc.0
    IL_004f:  stelem.ref
    IL_0050:  ldloc.1
    IL_0051:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0056:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<object,bool>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_005b:  nop
    IL_005c:  ret
  } // end of method ExpressionTrees::IsTypeExpr

  .method public hidebysig static void  UnaryLogicalOperators() cil managed
  {
    // Code size       83 (0x53)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<bool,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4a'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<UnaryLogicalOperators>b__49'(bool)
    IL_000f:  newobj     instance void class [mscorlib]System.Func`2<bool,bool>::.ctor(object,
                                                                                       native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`2<bool,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4a'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`2<bool,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4a'
    IL_0020:  ldtoken    [mscorlib]System.Boolean
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "a"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_003b:  ldc.i4.1
    IL_003c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0041:  stloc.1
    IL_0042:  ldloc.1
    IL_0043:  ldc.i4.0
    IL_0044:  ldloc.0
    IL_0045:  stelem.ref
    IL_0046:  ldloc.1
    IL_0047:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<bool,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_004c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<bool,bool>>(!!0,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0051:  nop
    IL_0052:  ret
  } // end of method ExpressionTrees::UnaryLogicalOperators

  .method public hidebysig static void  ConditionalOperator() cil managed
  {
    // Code size       173 (0xad)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldtoken    [mscorlib]System.Boolean
    IL_0007:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000c:  ldstr      "a"
    IL_0011:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.5
    IL_0019:  box        [mscorlib]System.Int32
    IL_001e:  ldtoken    [mscorlib]System.Int32
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002d:  ldc.i4.s   10
    IL_002f:  box        [mscorlib]System.Int32
    IL_0034:  ldtoken    [mscorlib]System.Int32
    IL_0039:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0043:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0048:  ldc.i4.1
    IL_0049:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004e:  stloc.1
    IL_004f:  ldloc.1
    IL_0050:  ldc.i4.0
    IL_0051:  ldloc.0
    IL_0052:  stelem.ref
    IL_0053:  ldloc.1
    IL_0054:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<bool,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0059:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool,int32>(object,
                                                                                                                  class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_005e:  pop
    IL_005f:  ldnull
    IL_0060:  ldtoken    [mscorlib]System.Object
    IL_0065:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006a:  ldstr      "a"
    IL_006f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0074:  stloc.0
    IL_0075:  ldloc.0
    IL_0076:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass::.ctor()
    IL_007b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0080:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0085:  ldc.i4.0
    IL_0086:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_008b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0090:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0095:  ldc.i4.1
    IL_0096:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_009b:  stloc.1
    IL_009c:  ldloc.1
    IL_009d:  ldc.i4.0
    IL_009e:  ldloc.0
    IL_009f:  stelem.ref
    IL_00a0:  ldloc.1
    IL_00a1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a6:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<object,object>(object,
                                                                                                                     class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_00ab:  pop
    IL_00ac:  ret
  } // end of method ExpressionTrees::ConditionalOperator

  .method public hidebysig static void  BinaryLogicalOperators() cil managed
  {
    // Code size       1639 (0x667)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldtoken    [mscorlib]System.Int32
    IL_0007:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000c:  ldstr      "a"
    IL_0011:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0016:  stloc.0
    IL_0017:  ldtoken    [mscorlib]System.Int32
    IL_001c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0021:  ldstr      "b"
    IL_0026:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002b:  stloc.1
    IL_002c:  ldloc.0
    IL_002d:  ldloc.1
    IL_002e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0033:  ldc.i4.2
    IL_0034:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0039:  stloc.2
    IL_003a:  ldloc.2
    IL_003b:  ldc.i4.0
    IL_003c:  ldloc.0
    IL_003d:  stelem.ref
    IL_003e:  ldloc.2
    IL_003f:  ldc.i4.1
    IL_0040:  ldloc.1
    IL_0041:  stelem.ref
    IL_0042:  ldloc.2
    IL_0043:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0048:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_004d:  pop
    IL_004e:  ldnull
    IL_004f:  ldtoken    [mscorlib]System.Int32
    IL_0054:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0059:  ldstr      "a"
    IL_005e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0063:  stloc.0
    IL_0064:  ldtoken    [mscorlib]System.Int32
    IL_0069:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006e:  ldstr      "b"
    IL_0073:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0078:  stloc.1
    IL_0079:  ldloc.0
    IL_007a:  ldloc.1
    IL_007b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0080:  ldc.i4.2
    IL_0081:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0086:  stloc.2
    IL_0087:  ldloc.2
    IL_0088:  ldc.i4.0
    IL_0089:  ldloc.0
    IL_008a:  stelem.ref
    IL_008b:  ldloc.2
    IL_008c:  ldc.i4.1
    IL_008d:  ldloc.1
    IL_008e:  stelem.ref
    IL_008f:  ldloc.2
    IL_0090:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0095:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_009a:  pop
    IL_009b:  ldnull
    IL_009c:  ldtoken    [mscorlib]System.Int32
    IL_00a1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a6:  ldstr      "a"
    IL_00ab:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00b0:  stloc.0
    IL_00b1:  ldtoken    [mscorlib]System.Int32
    IL_00b6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bb:  ldstr      "b"
    IL_00c0:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00c5:  stloc.1
    IL_00c6:  ldloc.0
    IL_00c7:  ldloc.1
    IL_00c8:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00cd:  ldc.i4.2
    IL_00ce:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00d3:  stloc.2
    IL_00d4:  ldloc.2
    IL_00d5:  ldc.i4.0
    IL_00d6:  ldloc.0
    IL_00d7:  stelem.ref
    IL_00d8:  ldloc.2
    IL_00d9:  ldc.i4.1
    IL_00da:  ldloc.1
    IL_00db:  stelem.ref
    IL_00dc:  ldloc.2
    IL_00dd:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e2:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_00e7:  pop
    IL_00e8:  ldnull
    IL_00e9:  ldtoken    [mscorlib]System.Int32
    IL_00ee:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f3:  ldstr      "a"
    IL_00f8:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00fd:  stloc.0
    IL_00fe:  ldtoken    [mscorlib]System.Int32
    IL_0103:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0108:  ldstr      "b"
    IL_010d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0112:  stloc.1
    IL_0113:  ldloc.0
    IL_0114:  ldloc.1
    IL_0115:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression)
    IL_011a:  ldc.i4.2
    IL_011b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0120:  stloc.2
    IL_0121:  ldloc.2
    IL_0122:  ldc.i4.0
    IL_0123:  ldloc.0
    IL_0124:  stelem.ref
    IL_0125:  ldloc.2
    IL_0126:  ldc.i4.1
    IL_0127:  ldloc.1
    IL_0128:  stelem.ref
    IL_0129:  ldloc.2
    IL_012a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_012f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0134:  pop
    IL_0135:  ldnull
    IL_0136:  ldtoken    [mscorlib]System.Int32
    IL_013b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0140:  ldstr      "a"
    IL_0145:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_014a:  stloc.0
    IL_014b:  ldtoken    [mscorlib]System.Int32
    IL_0150:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0155:  ldstr      "b"
    IL_015a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_015f:  stloc.1
    IL_0160:  ldloc.0
    IL_0161:  ldloc.1
    IL_0162:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0167:  ldc.i4.2
    IL_0168:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_016d:  stloc.2
    IL_016e:  ldloc.2
    IL_016f:  ldc.i4.0
    IL_0170:  ldloc.0
    IL_0171:  stelem.ref
    IL_0172:  ldloc.2
    IL_0173:  ldc.i4.1
    IL_0174:  ldloc.1
    IL_0175:  stelem.ref
    IL_0176:  ldloc.2
    IL_0177:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_017c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0181:  pop
    IL_0182:  ldnull
    IL_0183:  ldtoken    [mscorlib]System.Int32
    IL_0188:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_018d:  ldstr      "a"
    IL_0192:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0197:  stloc.0
    IL_0198:  ldtoken    [mscorlib]System.Int32
    IL_019d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a2:  ldstr      "b"
    IL_01a7:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01ac:  stloc.1
    IL_01ad:  ldloc.0
    IL_01ae:  ldloc.1
    IL_01af:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression)
    IL_01b4:  ldc.i4.2
    IL_01b5:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01ba:  stloc.2
    IL_01bb:  ldloc.2
    IL_01bc:  ldc.i4.0
    IL_01bd:  ldloc.0
    IL_01be:  stelem.ref
    IL_01bf:  ldloc.2
    IL_01c0:  ldc.i4.1
    IL_01c1:  ldloc.1
    IL_01c2:  stelem.ref
    IL_01c3:  ldloc.2
    IL_01c4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01c9:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_01ce:  pop
    IL_01cf:  ldnull
    IL_01d0:  ldtoken    [mscorlib]System.Int32
    IL_01d5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01da:  ldstr      "a"
    IL_01df:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01e4:  stloc.0
    IL_01e5:  ldtoken    [mscorlib]System.Int32
    IL_01ea:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ef:  ldstr      "b"
    IL_01f4:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01f9:  stloc.1
    IL_01fa:  ldloc.0
    IL_01fb:  ldc.i4.1
    IL_01fc:  box        [mscorlib]System.Int32
    IL_0201:  ldtoken    [mscorlib]System.Int32
    IL_0206:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_020b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0210:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0215:  ldloc.1
    IL_0216:  ldc.i4.2
    IL_0217:  box        [mscorlib]System.Int32
    IL_021c:  ldtoken    [mscorlib]System.Int32
    IL_0221:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0226:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_022b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0230:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0235:  ldc.i4.2
    IL_0236:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_023b:  stloc.2
    IL_023c:  ldloc.2
    IL_023d:  ldc.i4.0
    IL_023e:  ldloc.0
    IL_023f:  stelem.ref
    IL_0240:  ldloc.2
    IL_0241:  ldc.i4.1
    IL_0242:  ldloc.1
    IL_0243:  stelem.ref
    IL_0244:  ldloc.2
    IL_0245:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_024a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_024f:  pop
    IL_0250:  ldnull
    IL_0251:  ldtoken    [mscorlib]System.Int32
    IL_0256:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_025b:  ldstr      "a"
    IL_0260:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0265:  stloc.0
    IL_0266:  ldtoken    [mscorlib]System.Int32
    IL_026b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0270:  ldstr      "b"
    IL_0275:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_027a:  stloc.1
    IL_027b:  ldloc.0
    IL_027c:  ldc.i4.1
    IL_027d:  box        [mscorlib]System.Int32
    IL_0282:  ldtoken    [mscorlib]System.Int32
    IL_0287:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_028c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0291:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0296:  ldloc.1
    IL_0297:  ldc.i4.2
    IL_0298:  box        [mscorlib]System.Int32
    IL_029d:  ldtoken    [mscorlib]System.Int32
    IL_02a2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02a7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02ac:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_02b1:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_02b6:  ldc.i4.2
    IL_02b7:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_02bc:  stloc.2
    IL_02bd:  ldloc.2
    IL_02be:  ldc.i4.0
    IL_02bf:  ldloc.0
    IL_02c0:  stelem.ref
    IL_02c1:  ldloc.2
    IL_02c2:  ldc.i4.1
    IL_02c3:  ldloc.1
    IL_02c4:  stelem.ref
    IL_02c5:  ldloc.2
    IL_02c6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_02cb:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_02d0:  pop
    IL_02d1:  ldnull
    IL_02d2:  ldtoken    [mscorlib]System.Int32
    IL_02d7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02dc:  ldstr      "a"
    IL_02e1:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02e6:  stloc.0
    IL_02e7:  ldtoken    [mscorlib]System.Int16
    IL_02ec:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02f1:  ldstr      "b"
    IL_02f6:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02fb:  stloc.1
    IL_02fc:  ldloc.0
    IL_02fd:  ldloc.1
    IL_02fe:  ldtoken    [mscorlib]System.Int32
    IL_0303:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0308:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_030d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0312:  ldc.i4.2
    IL_0313:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0318:  stloc.2
    IL_0319:  ldloc.2
    IL_031a:  ldc.i4.0
    IL_031b:  ldloc.0
    IL_031c:  stelem.ref
    IL_031d:  ldloc.2
    IL_031e:  ldc.i4.1
    IL_031f:  ldloc.1
    IL_0320:  stelem.ref
    IL_0321:  ldloc.2
    IL_0322:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int16,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0327:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int16,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_032c:  pop
    IL_032d:  ldnull
    IL_032e:  ldtoken    [mscorlib]System.UInt16
    IL_0333:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0338:  ldstr      "a"
    IL_033d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0342:  stloc.0
    IL_0343:  ldtoken    [mscorlib]System.Int32
    IL_0348:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_034d:  ldstr      "b"
    IL_0352:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0357:  stloc.1
    IL_0358:  ldloc.0
    IL_0359:  ldtoken    [mscorlib]System.Int32
    IL_035e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0363:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0368:  ldloc.1
    IL_0369:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_036e:  ldc.i4.2
    IL_036f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0374:  stloc.2
    IL_0375:  ldloc.2
    IL_0376:  ldc.i4.0
    IL_0377:  ldloc.0
    IL_0378:  stelem.ref
    IL_0379:  ldloc.2
    IL_037a:  ldc.i4.1
    IL_037b:  ldloc.1
    IL_037c:  stelem.ref
    IL_037d:  ldloc.2
    IL_037e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<uint16,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0383:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<uint16,int32,bool>(object,
                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0388:  pop
    IL_0389:  ldnull
    IL_038a:  ldtoken    [mscorlib]System.Int32
    IL_038f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0394:  ldstr      "a"
    IL_0399:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_039e:  stloc.0
    IL_039f:  ldtoken    [mscorlib]System.Int64
    IL_03a4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03a9:  ldstr      "b"
    IL_03ae:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03b3:  stloc.1
    IL_03b4:  ldloc.0
    IL_03b5:  ldtoken    [mscorlib]System.Int64
    IL_03ba:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03bf:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_03c4:  ldloc.1
    IL_03c5:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_03ca:  ldc.i4.2
    IL_03cb:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03d0:  stloc.2
    IL_03d1:  ldloc.2
    IL_03d2:  ldc.i4.0
    IL_03d3:  ldloc.0
    IL_03d4:  stelem.ref
    IL_03d5:  ldloc.2
    IL_03d6:  ldc.i4.1
    IL_03d7:  ldloc.1
    IL_03d8:  stelem.ref
    IL_03d9:  ldloc.2
    IL_03da:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int64,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03df:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int64,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_03e4:  pop
    IL_03e5:  ldnull
    IL_03e6:  ldtoken    [mscorlib]System.UInt64
    IL_03eb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03f0:  ldstr      "a"
    IL_03f5:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03fa:  stloc.0
    IL_03fb:  ldtoken    [mscorlib]System.UInt32
    IL_0400:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0405:  ldstr      "b"
    IL_040a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_040f:  stloc.1
    IL_0410:  ldloc.0
    IL_0411:  ldloc.1
    IL_0412:  ldtoken    [mscorlib]System.UInt64
    IL_0417:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_041c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0421:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression)
    IL_0426:  ldc.i4.2
    IL_0427:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_042c:  stloc.2
    IL_042d:  ldloc.2
    IL_042e:  ldc.i4.0
    IL_042f:  ldloc.0
    IL_0430:  stelem.ref
    IL_0431:  ldloc.2
    IL_0432:  ldc.i4.1
    IL_0433:  ldloc.1
    IL_0434:  stelem.ref
    IL_0435:  ldloc.2
    IL_0436:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<uint64,uint32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_043b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<uint64,uint32,bool>(object,
                                                                                                                          class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0440:  pop
    IL_0441:  ldnull
    IL_0442:  ldtoken    [mscorlib]System.Int32
    IL_0447:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_044c:  ldstr      "a"
    IL_0451:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0456:  stloc.0
    IL_0457:  ldtoken    [mscorlib]System.UInt32
    IL_045c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0461:  ldstr      "b"
    IL_0466:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_046b:  stloc.1
    IL_046c:  ldloc.0
    IL_046d:  ldtoken    [mscorlib]System.Int64
    IL_0472:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0477:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_047c:  ldloc.1
    IL_047d:  ldtoken    [mscorlib]System.Int64
    IL_0482:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0487:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_048c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression)
    IL_0491:  ldc.i4.2
    IL_0492:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0497:  stloc.2
    IL_0498:  ldloc.2
    IL_0499:  ldc.i4.0
    IL_049a:  ldloc.0
    IL_049b:  stelem.ref
    IL_049c:  ldloc.2
    IL_049d:  ldc.i4.1
    IL_049e:  ldloc.1
    IL_049f:  stelem.ref
    IL_04a0:  ldloc.2
    IL_04a1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,uint32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_04a6:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,uint32,bool>(object,
                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_04ab:  pop
    IL_04ac:  ldnull
    IL_04ad:  ldtoken    [mscorlib]System.Int32
    IL_04b2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04b7:  ldstr      "a"
    IL_04bc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04c1:  stloc.0
    IL_04c2:  ldtoken    [mscorlib]System.Int64
    IL_04c7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04cc:  ldstr      "b"
    IL_04d1:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04d6:  stloc.1
    IL_04d7:  ldloc.0
    IL_04d8:  ldtoken    [mscorlib]System.Int64
    IL_04dd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04e2:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_04e7:  ldloc.1
    IL_04e8:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_04ed:  ldc.i4.2
    IL_04ee:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_04f3:  stloc.2
    IL_04f4:  ldloc.2
    IL_04f5:  ldc.i4.0
    IL_04f6:  ldloc.0
    IL_04f7:  stelem.ref
    IL_04f8:  ldloc.2
    IL_04f9:  ldc.i4.1
    IL_04fa:  ldloc.1
    IL_04fb:  stelem.ref
    IL_04fc:  ldloc.2
    IL_04fd:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int64,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0502:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int64,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0507:  pop
    IL_0508:  ldnull
    IL_0509:  ldtoken    [mscorlib]System.Int16
    IL_050e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0513:  ldstr      "a"
    IL_0518:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_051d:  stloc.0
    IL_051e:  ldtoken    [mscorlib]System.Int64
    IL_0523:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0528:  ldstr      "b"
    IL_052d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0532:  stloc.1
    IL_0533:  ldloc.0
    IL_0534:  ldtoken    [mscorlib]System.Int64
    IL_0539:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_053e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0543:  ldloc.1
    IL_0544:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression)
    IL_0549:  ldc.i4.2
    IL_054a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_054f:  stloc.2
    IL_0550:  ldloc.2
    IL_0551:  ldc.i4.0
    IL_0552:  ldloc.0
    IL_0553:  stelem.ref
    IL_0554:  ldloc.2
    IL_0555:  ldc.i4.1
    IL_0556:  ldloc.1
    IL_0557:  stelem.ref
    IL_0558:  ldloc.2
    IL_0559:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int64,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_055e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int16,int64,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0563:  pop
    IL_0564:  ldnull
    IL_0565:  ldtoken    [mscorlib]System.Int32
    IL_056a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_056f:  ldstr      "a"
    IL_0574:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0579:  stloc.0
    IL_057a:  ldtoken    [mscorlib]System.Int32
    IL_057f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0584:  ldstr      "b"
    IL_0589:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_058e:  stloc.1
    IL_058f:  ldloc.0
    IL_0590:  ldc.i4.1
    IL_0591:  box        [mscorlib]System.Int32
    IL_0596:  ldtoken    [mscorlib]System.Int32
    IL_059b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05a0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_05a5:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_05aa:  ldloc.1
    IL_05ab:  ldc.i4.2
    IL_05ac:  box        [mscorlib]System.Int32
    IL_05b1:  ldtoken    [mscorlib]System.Int32
    IL_05b6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05bb:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_05c0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_05c5:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_05ca:  ldc.i4.2
    IL_05cb:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_05d0:  stloc.2
    IL_05d1:  ldloc.2
    IL_05d2:  ldc.i4.0
    IL_05d3:  ldloc.0
    IL_05d4:  stelem.ref
    IL_05d5:  ldloc.2
    IL_05d6:  ldc.i4.1
    IL_05d7:  ldloc.1
    IL_05d8:  stelem.ref
    IL_05d9:  ldloc.2
    IL_05da:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_05df:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_05e4:  pop
    IL_05e5:  ldnull
    IL_05e6:  ldtoken    [mscorlib]System.Int32
    IL_05eb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05f0:  ldstr      "a"
    IL_05f5:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_05fa:  stloc.0
    IL_05fb:  ldtoken    [mscorlib]System.Int32
    IL_0600:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0605:  ldstr      "b"
    IL_060a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_060f:  stloc.1
    IL_0610:  ldloc.0
    IL_0611:  ldc.i4.1
    IL_0612:  box        [mscorlib]System.Int32
    IL_0617:  ldtoken    [mscorlib]System.Int32
    IL_061c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0621:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0626:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_062b:  ldloc.1
    IL_062c:  ldc.i4.2
    IL_062d:  box        [mscorlib]System.Int32
    IL_0632:  ldtoken    [mscorlib]System.Int32
    IL_0637:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_063c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0641:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0646:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_064b:  ldc.i4.2
    IL_064c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0651:  stloc.2
    IL_0652:  ldloc.2
    IL_0653:  ldc.i4.0
    IL_0654:  ldloc.0
    IL_0655:  stelem.ref
    IL_0656:  ldloc.2
    IL_0657:  ldc.i4.1
    IL_0658:  ldloc.1
    IL_0659:  stelem.ref
    IL_065a:  ldloc.2
    IL_065b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0660:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0665:  pop
    IL_0666:  ret
  } // end of method ExpressionTrees::BinaryLogicalOperators

  .method public hidebysig static void  UnaryArithmeticOperators() cil managed
  {
    // Code size       159 (0x9f)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4d'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<UnaryArithmeticOperators>b__4b'(int32)
    IL_000f:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4d'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4d'
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "a"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.1
    IL_0037:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003c:  stloc.1
    IL_003d:  ldloc.1
    IL_003e:  ldc.i4.0
    IL_003f:  ldloc.0
    IL_0040:  stelem.ref
    IL_0041:  ldloc.1
    IL_0042:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0047:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_004c:  nop
    IL_004d:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4e'
    IL_0052:  brtrue.s   IL_0067

    IL_0054:  ldnull
    IL_0055:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<UnaryArithmeticOperators>b__4c'(int32)
    IL_005b:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0060:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4e'
    IL_0065:  br.s       IL_0067

    IL_0067:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate4e'
    IL_006c:  ldtoken    [mscorlib]System.Int32
    IL_0071:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0076:  ldstr      "a"
    IL_007b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0080:  stloc.0
    IL_0081:  ldloc.0
    IL_0082:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_0087:  ldc.i4.1
    IL_0088:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_008d:  stloc.1
    IL_008e:  ldloc.1
    IL_008f:  ldc.i4.0
    IL_0090:  ldloc.0
    IL_0091:  stelem.ref
    IL_0092:  ldloc.1
    IL_0093:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0098:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_009d:  nop
    IL_009e:  ret
  } // end of method ExpressionTrees::UnaryArithmeticOperators

  .method public hidebysig static void  BinaryArithmeticOperators() cil managed
  {
    // Code size       1757 (0x6dd)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate5e'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__4f'(int32,
                                                                                                                                 int32)
    IL_000f:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate5e'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate5e'
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "a"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldtoken    [mscorlib]System.Int32
    IL_003a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003f:  ldstr      "b"
    IL_0044:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0049:  stloc.1
    IL_004a:  ldloc.0
    IL_004b:  ldloc.1
    IL_004c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0051:  ldc.i4.2
    IL_0052:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0057:  stloc.2
    IL_0058:  ldloc.2
    IL_0059:  ldc.i4.0
    IL_005a:  ldloc.0
    IL_005b:  stelem.ref
    IL_005c:  ldloc.2
    IL_005d:  ldc.i4.1
    IL_005e:  ldloc.1
    IL_005f:  stelem.ref
    IL_0060:  ldloc.2
    IL_0061:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0066:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_006b:  nop
    IL_006c:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate5f'
    IL_0071:  brtrue.s   IL_0086

    IL_0073:  ldnull
    IL_0074:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__50'(int32,
                                                                                                                                 int32)
    IL_007a:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_007f:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate5f'
    IL_0084:  br.s       IL_0086

    IL_0086:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate5f'
    IL_008b:  ldtoken    [mscorlib]System.Int32
    IL_0090:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0095:  ldstr      "a"
    IL_009a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_009f:  stloc.0
    IL_00a0:  ldtoken    [mscorlib]System.Int32
    IL_00a5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00aa:  ldstr      "b"
    IL_00af:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00b4:  stloc.1
    IL_00b5:  ldloc.0
    IL_00b6:  ldloc.1
    IL_00b7:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Subtract(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00bc:  ldc.i4.2
    IL_00bd:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00c2:  stloc.2
    IL_00c3:  ldloc.2
    IL_00c4:  ldc.i4.0
    IL_00c5:  ldloc.0
    IL_00c6:  stelem.ref
    IL_00c7:  ldloc.2
    IL_00c8:  ldc.i4.1
    IL_00c9:  ldloc.1
    IL_00ca:  stelem.ref
    IL_00cb:  ldloc.2
    IL_00cc:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00d1:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00d6:  nop
    IL_00d7:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate60'
    IL_00dc:  brtrue.s   IL_00f1

    IL_00de:  ldnull
    IL_00df:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__51'(int32,
                                                                                                                                 int32)
    IL_00e5:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_00ea:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate60'
    IL_00ef:  br.s       IL_00f1

    IL_00f1:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate60'
    IL_00f6:  ldtoken    [mscorlib]System.Int32
    IL_00fb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0100:  ldstr      "a"
    IL_0105:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_010a:  stloc.0
    IL_010b:  ldtoken    [mscorlib]System.Int32
    IL_0110:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0115:  ldstr      "b"
    IL_011a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_011f:  stloc.1
    IL_0120:  ldloc.0
    IL_0121:  ldloc.1
    IL_0122:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0127:  ldc.i4.2
    IL_0128:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_012d:  stloc.2
    IL_012e:  ldloc.2
    IL_012f:  ldc.i4.0
    IL_0130:  ldloc.0
    IL_0131:  stelem.ref
    IL_0132:  ldloc.2
    IL_0133:  ldc.i4.1
    IL_0134:  ldloc.1
    IL_0135:  stelem.ref
    IL_0136:  ldloc.2
    IL_0137:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_013c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0141:  nop
    IL_0142:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate61'
    IL_0147:  brtrue.s   IL_015c

    IL_0149:  ldnull
    IL_014a:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__52'(int32,
                                                                                                                                 int32)
    IL_0150:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0155:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate61'
    IL_015a:  br.s       IL_015c

    IL_015c:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate61'
    IL_0161:  ldtoken    [mscorlib]System.Int32
    IL_0166:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_016b:  ldstr      "a"
    IL_0170:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0175:  stloc.0
    IL_0176:  ldtoken    [mscorlib]System.Int32
    IL_017b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0180:  ldstr      "b"
    IL_0185:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_018a:  stloc.1
    IL_018b:  ldloc.0
    IL_018c:  ldloc.1
    IL_018d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Divide(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_0192:  ldc.i4.2
    IL_0193:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0198:  stloc.2
    IL_0199:  ldloc.2
    IL_019a:  ldc.i4.0
    IL_019b:  ldloc.0
    IL_019c:  stelem.ref
    IL_019d:  ldloc.2
    IL_019e:  ldc.i4.1
    IL_019f:  ldloc.1
    IL_01a0:  stelem.ref
    IL_01a1:  ldloc.2
    IL_01a2:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01a7:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01ac:  nop
    IL_01ad:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate62'
    IL_01b2:  brtrue.s   IL_01c7

    IL_01b4:  ldnull
    IL_01b5:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__53'(int32,
                                                                                                                                 int32)
    IL_01bb:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_01c0:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate62'
    IL_01c5:  br.s       IL_01c7

    IL_01c7:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate62'
    IL_01cc:  ldtoken    [mscorlib]System.Int32
    IL_01d1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01d6:  ldstr      "a"
    IL_01db:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01e0:  stloc.0
    IL_01e1:  ldtoken    [mscorlib]System.Int32
    IL_01e6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01eb:  ldstr      "b"
    IL_01f0:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01f5:  stloc.1
    IL_01f6:  ldloc.0
    IL_01f7:  ldloc.1
    IL_01f8:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_01fd:  ldc.i4.2
    IL_01fe:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0203:  stloc.2
    IL_0204:  ldloc.2
    IL_0205:  ldc.i4.0
    IL_0206:  ldloc.0
    IL_0207:  stelem.ref
    IL_0208:  ldloc.2
    IL_0209:  ldc.i4.1
    IL_020a:  ldloc.1
    IL_020b:  stelem.ref
    IL_020c:  ldloc.2
    IL_020d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0212:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0217:  nop
    IL_0218:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate63'
    IL_021d:  brtrue.s   IL_0232

    IL_021f:  ldnull
    IL_0220:  ldftn      int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__54'(int64,
                                                                                                                                 int32)
    IL_0226:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_022b:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate63'
    IL_0230:  br.s       IL_0232

    IL_0232:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate63'
    IL_0237:  ldtoken    [mscorlib]System.Int64
    IL_023c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0241:  ldstr      "a"
    IL_0246:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_024b:  stloc.0
    IL_024c:  ldtoken    [mscorlib]System.Int32
    IL_0251:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0256:  ldstr      "b"
    IL_025b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0260:  stloc.1
    IL_0261:  ldloc.0
    IL_0262:  ldloc.1
    IL_0263:  ldtoken    [mscorlib]System.Int64
    IL_0268:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_026d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0272:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0277:  ldc.i4.2
    IL_0278:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_027d:  stloc.2
    IL_027e:  ldloc.2
    IL_027f:  ldc.i4.0
    IL_0280:  ldloc.0
    IL_0281:  stelem.ref
    IL_0282:  ldloc.2
    IL_0283:  ldc.i4.1
    IL_0284:  ldloc.1
    IL_0285:  stelem.ref
    IL_0286:  ldloc.2
    IL_0287:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_028c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0291:  nop
    IL_0292:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate64'
    IL_0297:  brtrue.s   IL_02ac

    IL_0299:  ldnull
    IL_029a:  ldftn      int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__55'(int64,
                                                                                                                                 int32)
    IL_02a0:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_02a5:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate64'
    IL_02aa:  br.s       IL_02ac

    IL_02ac:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate64'
    IL_02b1:  ldtoken    [mscorlib]System.Int64
    IL_02b6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02bb:  ldstr      "a"
    IL_02c0:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02c5:  stloc.0
    IL_02c6:  ldtoken    [mscorlib]System.Int32
    IL_02cb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02d0:  ldstr      "b"
    IL_02d5:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02da:  stloc.1
    IL_02db:  ldloc.0
    IL_02dc:  ldloc.1
    IL_02dd:  ldtoken    [mscorlib]System.Int64
    IL_02e2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02e7:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_02ec:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Subtract(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_02f1:  ldc.i4.2
    IL_02f2:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_02f7:  stloc.2
    IL_02f8:  ldloc.2
    IL_02f9:  ldc.i4.0
    IL_02fa:  ldloc.0
    IL_02fb:  stelem.ref
    IL_02fc:  ldloc.2
    IL_02fd:  ldc.i4.1
    IL_02fe:  ldloc.1
    IL_02ff:  stelem.ref
    IL_0300:  ldloc.2
    IL_0301:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0306:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_030b:  nop
    IL_030c:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate65'
    IL_0311:  brtrue.s   IL_0326

    IL_0313:  ldnull
    IL_0314:  ldftn      int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__56'(int64,
                                                                                                                                 int32)
    IL_031a:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_031f:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate65'
    IL_0324:  br.s       IL_0326

    IL_0326:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate65'
    IL_032b:  ldtoken    [mscorlib]System.Int64
    IL_0330:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0335:  ldstr      "a"
    IL_033a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_033f:  stloc.0
    IL_0340:  ldtoken    [mscorlib]System.Int32
    IL_0345:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_034a:  ldstr      "b"
    IL_034f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0354:  stloc.1
    IL_0355:  ldloc.0
    IL_0356:  ldloc.1
    IL_0357:  ldtoken    [mscorlib]System.Int64
    IL_035c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0361:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0366:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_036b:  ldc.i4.2
    IL_036c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0371:  stloc.2
    IL_0372:  ldloc.2
    IL_0373:  ldc.i4.0
    IL_0374:  ldloc.0
    IL_0375:  stelem.ref
    IL_0376:  ldloc.2
    IL_0377:  ldc.i4.1
    IL_0378:  ldloc.1
    IL_0379:  stelem.ref
    IL_037a:  ldloc.2
    IL_037b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0380:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0385:  nop
    IL_0386:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate66'
    IL_038b:  brtrue.s   IL_03a0

    IL_038d:  ldnull
    IL_038e:  ldftn      int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__57'(int64,
                                                                                                                                 int32)
    IL_0394:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_0399:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate66'
    IL_039e:  br.s       IL_03a0

    IL_03a0:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate66'
    IL_03a5:  ldtoken    [mscorlib]System.Int64
    IL_03aa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03af:  ldstr      "a"
    IL_03b4:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03b9:  stloc.0
    IL_03ba:  ldtoken    [mscorlib]System.Int32
    IL_03bf:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03c4:  ldstr      "b"
    IL_03c9:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03ce:  stloc.1
    IL_03cf:  ldloc.0
    IL_03d0:  ldloc.1
    IL_03d1:  ldtoken    [mscorlib]System.Int64
    IL_03d6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03db:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_03e0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Divide(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_03e5:  ldc.i4.2
    IL_03e6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03eb:  stloc.2
    IL_03ec:  ldloc.2
    IL_03ed:  ldc.i4.0
    IL_03ee:  ldloc.0
    IL_03ef:  stelem.ref
    IL_03f0:  ldloc.2
    IL_03f1:  ldc.i4.1
    IL_03f2:  ldloc.1
    IL_03f3:  stelem.ref
    IL_03f4:  ldloc.2
    IL_03f5:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03fa:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_03ff:  nop
    IL_0400:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate67'
    IL_0405:  brtrue.s   IL_041a

    IL_0407:  ldnull
    IL_0408:  ldftn      int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__58'(int64,
                                                                                                                                 int32)
    IL_040e:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_0413:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate67'
    IL_0418:  br.s       IL_041a

    IL_041a:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate67'
    IL_041f:  ldtoken    [mscorlib]System.Int64
    IL_0424:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0429:  ldstr      "a"
    IL_042e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0433:  stloc.0
    IL_0434:  ldtoken    [mscorlib]System.Int32
    IL_0439:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_043e:  ldstr      "b"
    IL_0443:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0448:  stloc.1
    IL_0449:  ldloc.0
    IL_044a:  ldloc.1
    IL_044b:  ldtoken    [mscorlib]System.Int64
    IL_0450:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0455:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_045a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_045f:  ldc.i4.2
    IL_0460:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0465:  stloc.2
    IL_0466:  ldloc.2
    IL_0467:  ldc.i4.0
    IL_0468:  ldloc.0
    IL_0469:  stelem.ref
    IL_046a:  ldloc.2
    IL_046b:  ldc.i4.1
    IL_046c:  ldloc.1
    IL_046d:  stelem.ref
    IL_046e:  ldloc.2
    IL_046f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0474:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0479:  nop
    IL_047a:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate68'
    IL_047f:  brtrue.s   IL_0494

    IL_0481:  ldnull
    IL_0482:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__59'(int16,
                                                                                                                                 int32)
    IL_0488:  newobj     instance void class [mscorlib]System.Func`3<int16,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_048d:  stsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate68'
    IL_0492:  br.s       IL_0494

    IL_0494:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate68'
    IL_0499:  ldtoken    [mscorlib]System.Int16
    IL_049e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04a3:  ldstr      "a"
    IL_04a8:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04ad:  stloc.0
    IL_04ae:  ldtoken    [mscorlib]System.Int32
    IL_04b3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04b8:  ldstr      "b"
    IL_04bd:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04c2:  stloc.1
    IL_04c3:  ldloc.0
    IL_04c4:  ldtoken    [mscorlib]System.Int32
    IL_04c9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04ce:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_04d3:  ldloc.1
    IL_04d4:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_04d9:  ldc.i4.2
    IL_04da:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_04df:  stloc.2
    IL_04e0:  ldloc.2
    IL_04e1:  ldc.i4.0
    IL_04e2:  ldloc.0
    IL_04e3:  stelem.ref
    IL_04e4:  ldloc.2
    IL_04e5:  ldc.i4.1
    IL_04e6:  ldloc.1
    IL_04e7:  stelem.ref
    IL_04e8:  ldloc.2
    IL_04e9:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_04ee:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int16,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_04f3:  nop
    IL_04f4:  ldsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate69'
    IL_04f9:  brtrue.s   IL_050e

    IL_04fb:  ldnull
    IL_04fc:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__5a'(int32,
                                                                                                                                 int16)
    IL_0502:  newobj     instance void class [mscorlib]System.Func`3<int32,int16,int32>::.ctor(object,
                                                                                               native int)
    IL_0507:  stsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate69'
    IL_050c:  br.s       IL_050e

    IL_050e:  ldsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate69'
    IL_0513:  ldtoken    [mscorlib]System.Int32
    IL_0518:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_051d:  ldstr      "a"
    IL_0522:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0527:  stloc.0
    IL_0528:  ldtoken    [mscorlib]System.Int16
    IL_052d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0532:  ldstr      "b"
    IL_0537:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_053c:  stloc.1
    IL_053d:  ldloc.0
    IL_053e:  ldloc.1
    IL_053f:  ldtoken    [mscorlib]System.Int32
    IL_0544:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0549:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_054e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Subtract(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0553:  ldc.i4.2
    IL_0554:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0559:  stloc.2
    IL_055a:  ldloc.2
    IL_055b:  ldc.i4.0
    IL_055c:  ldloc.0
    IL_055d:  stelem.ref
    IL_055e:  ldloc.2
    IL_055f:  ldc.i4.1
    IL_0560:  ldloc.1
    IL_0561:  stelem.ref
    IL_0562:  ldloc.2
    IL_0563:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int16,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0568:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int16,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_056d:  nop
    IL_056e:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6a'
    IL_0573:  brtrue.s   IL_0588

    IL_0575:  ldnull
    IL_0576:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__5b'(int16,
                                                                                                                                 int32)
    IL_057c:  newobj     instance void class [mscorlib]System.Func`3<int16,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0581:  stsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6a'
    IL_0586:  br.s       IL_0588

    IL_0588:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6a'
    IL_058d:  ldtoken    [mscorlib]System.Int16
    IL_0592:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0597:  ldstr      "a"
    IL_059c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_05a1:  stloc.0
    IL_05a2:  ldtoken    [mscorlib]System.Int32
    IL_05a7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05ac:  ldstr      "b"
    IL_05b1:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_05b6:  stloc.1
    IL_05b7:  ldloc.0
    IL_05b8:  ldtoken    [mscorlib]System.Int32
    IL_05bd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05c2:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_05c7:  ldloc.1
    IL_05c8:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_05cd:  ldc.i4.2
    IL_05ce:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_05d3:  stloc.2
    IL_05d4:  ldloc.2
    IL_05d5:  ldc.i4.0
    IL_05d6:  ldloc.0
    IL_05d7:  stelem.ref
    IL_05d8:  ldloc.2
    IL_05d9:  ldc.i4.1
    IL_05da:  ldloc.1
    IL_05db:  stelem.ref
    IL_05dc:  ldloc.2
    IL_05dd:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_05e2:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int16,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_05e7:  nop
    IL_05e8:  ldsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6b'
    IL_05ed:  brtrue.s   IL_0602

    IL_05ef:  ldnull
    IL_05f0:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__5c'(int32,
                                                                                                                                 int16)
    IL_05f6:  newobj     instance void class [mscorlib]System.Func`3<int32,int16,int32>::.ctor(object,
                                                                                               native int)
    IL_05fb:  stsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6b'
    IL_0600:  br.s       IL_0602

    IL_0602:  ldsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6b'
    IL_0607:  ldtoken    [mscorlib]System.Int32
    IL_060c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0611:  ldstr      "a"
    IL_0616:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_061b:  stloc.0
    IL_061c:  ldtoken    [mscorlib]System.Int16
    IL_0621:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0626:  ldstr      "b"
    IL_062b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0630:  stloc.1
    IL_0631:  ldloc.0
    IL_0632:  ldloc.1
    IL_0633:  ldtoken    [mscorlib]System.Int32
    IL_0638:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_063d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0642:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Divide(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_0647:  ldc.i4.2
    IL_0648:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_064d:  stloc.2
    IL_064e:  ldloc.2
    IL_064f:  ldc.i4.0
    IL_0650:  ldloc.0
    IL_0651:  stelem.ref
    IL_0652:  ldloc.2
    IL_0653:  ldc.i4.1
    IL_0654:  ldloc.1
    IL_0655:  stelem.ref
    IL_0656:  ldloc.2
    IL_0657:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int16,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_065c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int16,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0661:  nop
    IL_0662:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6c'
    IL_0667:  brtrue.s   IL_067c

    IL_0669:  ldnull
    IL_066a:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BinaryArithmeticOperators>b__5d'(int16,
                                                                                                                                 int32)
    IL_0670:  newobj     instance void class [mscorlib]System.Func`3<int16,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0675:  stsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6c'
    IL_067a:  br.s       IL_067c

    IL_067c:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate6c'
    IL_0681:  ldtoken    [mscorlib]System.Int16
    IL_0686:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_068b:  ldstr      "a"
    IL_0690:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0695:  stloc.0
    IL_0696:  ldtoken    [mscorlib]System.Int32
    IL_069b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_06a0:  ldstr      "b"
    IL_06a5:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_06aa:  stloc.1
    IL_06ab:  ldloc.0
    IL_06ac:  ldtoken    [mscorlib]System.Int32
    IL_06b1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_06b6:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_06bb:  ldloc.1
    IL_06bc:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_06c1:  ldc.i4.2
    IL_06c2:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_06c7:  stloc.2
    IL_06c8:  ldloc.2
    IL_06c9:  ldc.i4.0
    IL_06ca:  ldloc.0
    IL_06cb:  stelem.ref
    IL_06cc:  ldloc.2
    IL_06cd:  ldc.i4.1
    IL_06ce:  ldloc.1
    IL_06cf:  stelem.ref
    IL_06d0:  ldloc.2
    IL_06d1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_06d6:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int16,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_06db:  nop
    IL_06dc:  ret
  } // end of method ExpressionTrees::BinaryArithmeticOperators

  .method public hidebysig static void  BitOperators() cil managed
  {
    // Code size       404 (0x194)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate71'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BitOperators>b__6d'(int32)
    IL_000f:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate71'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate71'
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "a"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_003b:  ldc.i4.1
    IL_003c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0041:  stloc.1
    IL_0042:  ldloc.1
    IL_0043:  ldc.i4.0
    IL_0044:  ldloc.0
    IL_0045:  stelem.ref
    IL_0046:  ldloc.1
    IL_0047:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_004c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0051:  nop
    IL_0052:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate72'
    IL_0057:  brtrue.s   IL_006c

    IL_0059:  ldnull
    IL_005a:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BitOperators>b__6e'(int32,
                                                                                                                    int32)
    IL_0060:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0065:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate72'
    IL_006a:  br.s       IL_006c

    IL_006c:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate72'
    IL_0071:  ldtoken    [mscorlib]System.Int32
    IL_0076:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007b:  ldstr      "a"
    IL_0080:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0085:  stloc.0
    IL_0086:  ldtoken    [mscorlib]System.Int32
    IL_008b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0090:  ldstr      "b"
    IL_0095:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_009a:  stloc.2
    IL_009b:  ldloc.0
    IL_009c:  ldloc.2
    IL_009d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::And(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_00a2:  ldc.i4.2
    IL_00a3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a8:  stloc.1
    IL_00a9:  ldloc.1
    IL_00aa:  ldc.i4.0
    IL_00ab:  ldloc.0
    IL_00ac:  stelem.ref
    IL_00ad:  ldloc.1
    IL_00ae:  ldc.i4.1
    IL_00af:  ldloc.2
    IL_00b0:  stelem.ref
    IL_00b1:  ldloc.1
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b7:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00bc:  nop
    IL_00bd:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate73'
    IL_00c2:  brtrue.s   IL_00d7

    IL_00c4:  ldnull
    IL_00c5:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BitOperators>b__6f'(int32,
                                                                                                                    int32)
    IL_00cb:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_00d0:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate73'
    IL_00d5:  br.s       IL_00d7

    IL_00d7:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate73'
    IL_00dc:  ldtoken    [mscorlib]System.Int32
    IL_00e1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e6:  ldstr      "a"
    IL_00eb:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00f0:  stloc.0
    IL_00f1:  ldtoken    [mscorlib]System.Int32
    IL_00f6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fb:  ldstr      "b"
    IL_0100:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0105:  stloc.2
    IL_0106:  ldloc.0
    IL_0107:  ldloc.2
    IL_0108:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Or(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression)
    IL_010d:  ldc.i4.2
    IL_010e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0113:  stloc.1
    IL_0114:  ldloc.1
    IL_0115:  ldc.i4.0
    IL_0116:  ldloc.0
    IL_0117:  stelem.ref
    IL_0118:  ldloc.1
    IL_0119:  ldc.i4.1
    IL_011a:  ldloc.2
    IL_011b:  stelem.ref
    IL_011c:  ldloc.1
    IL_011d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0122:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0127:  nop
    IL_0128:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate74'
    IL_012d:  brtrue.s   IL_0142

    IL_012f:  ldnull
    IL_0130:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<BitOperators>b__70'(int32,
                                                                                                                    int32)
    IL_0136:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_013b:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate74'
    IL_0140:  br.s       IL_0142

    IL_0142:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate74'
    IL_0147:  ldtoken    [mscorlib]System.Int32
    IL_014c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0151:  ldstr      "a"
    IL_0156:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_015b:  stloc.0
    IL_015c:  ldtoken    [mscorlib]System.Int32
    IL_0161:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0166:  ldstr      "b"
    IL_016b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0170:  stloc.2
    IL_0171:  ldloc.0
    IL_0172:  ldloc.2
    IL_0173:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ExclusiveOr(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0178:  ldc.i4.2
    IL_0179:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_017e:  stloc.1
    IL_017f:  ldloc.1
    IL_0180:  ldc.i4.0
    IL_0181:  ldloc.0
    IL_0182:  stelem.ref
    IL_0183:  ldloc.1
    IL_0184:  ldc.i4.1
    IL_0185:  ldloc.2
    IL_0186:  stelem.ref
    IL_0187:  ldloc.1
    IL_0188:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_018d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0192:  nop
    IL_0193:  ret
  } // end of method ExpressionTrees::BitOperators

  .method public hidebysig static void  ShiftOperators() cil managed
  {
    // Code size       410 (0x19a)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate79'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ShiftOperators>b__75'(int32)
    IL_000f:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate79'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate79'
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "a"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.2
    IL_0037:  box        [mscorlib]System.Int32
    IL_003c:  ldtoken    [mscorlib]System.Int32
    IL_0041:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::RightShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_0050:  ldc.i4.1
    IL_0051:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0056:  stloc.1
    IL_0057:  ldloc.1
    IL_0058:  ldc.i4.0
    IL_0059:  ldloc.0
    IL_005a:  stelem.ref
    IL_005b:  ldloc.1
    IL_005c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0061:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0066:  nop
    IL_0067:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7a'
    IL_006c:  brtrue.s   IL_0081

    IL_006e:  ldnull
    IL_006f:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ShiftOperators>b__76'(int32)
    IL_0075:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_007a:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7a'
    IL_007f:  br.s       IL_0081

    IL_0081:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7a'
    IL_0086:  ldtoken    [mscorlib]System.Int32
    IL_008b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0090:  ldstr      "a"
    IL_0095:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_009a:  stloc.0
    IL_009b:  ldloc.0
    IL_009c:  ldc.i4.2
    IL_009d:  box        [mscorlib]System.Int32
    IL_00a2:  ldtoken    [mscorlib]System.Int32
    IL_00a7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ac:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b1:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LeftShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression)
    IL_00b6:  ldc.i4.1
    IL_00b7:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00bc:  stloc.1
    IL_00bd:  ldloc.1
    IL_00be:  ldc.i4.0
    IL_00bf:  ldloc.0
    IL_00c0:  stelem.ref
    IL_00c1:  ldloc.1
    IL_00c2:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00c7:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00cc:  nop
    IL_00cd:  ldsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7b'
    IL_00d2:  brtrue.s   IL_00e7

    IL_00d4:  ldnull
    IL_00d5:  ldftn      int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ShiftOperators>b__77'(int64)
    IL_00db:  newobj     instance void class [mscorlib]System.Func`2<int64,int64>::.ctor(object,
                                                                                         native int)
    IL_00e0:  stsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7b'
    IL_00e5:  br.s       IL_00e7

    IL_00e7:  ldsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7b'
    IL_00ec:  ldtoken    [mscorlib]System.Int64
    IL_00f1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f6:  ldstr      "a"
    IL_00fb:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0100:  stloc.0
    IL_0101:  ldloc.0
    IL_0102:  ldc.i4.2
    IL_0103:  box        [mscorlib]System.Int32
    IL_0108:  ldtoken    [mscorlib]System.Int32
    IL_010d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0112:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0117:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::RightShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_011c:  ldc.i4.1
    IL_011d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0122:  stloc.1
    IL_0123:  ldloc.1
    IL_0124:  ldc.i4.0
    IL_0125:  ldloc.0
    IL_0126:  stelem.ref
    IL_0127:  ldloc.1
    IL_0128:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int64,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_012d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int64,int64>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0132:  nop
    IL_0133:  ldsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7c'
    IL_0138:  brtrue.s   IL_014d

    IL_013a:  ldnull
    IL_013b:  ldftn      int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ShiftOperators>b__78'(int64)
    IL_0141:  newobj     instance void class [mscorlib]System.Func`2<int64,int64>::.ctor(object,
                                                                                         native int)
    IL_0146:  stsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7c'
    IL_014b:  br.s       IL_014d

    IL_014d:  ldsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7c'
    IL_0152:  ldtoken    [mscorlib]System.Int64
    IL_0157:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_015c:  ldstr      "a"
    IL_0161:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0166:  stloc.0
    IL_0167:  ldloc.0
    IL_0168:  ldc.i4.2
    IL_0169:  box        [mscorlib]System.Int32
    IL_016e:  ldtoken    [mscorlib]System.Int32
    IL_0173:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0178:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_017d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LeftShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression)
    IL_0182:  ldc.i4.1
    IL_0183:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0188:  stloc.1
    IL_0189:  ldloc.1
    IL_018a:  ldc.i4.0
    IL_018b:  ldloc.0
    IL_018c:  stelem.ref
    IL_018d:  ldloc.1
    IL_018e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int64,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0193:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int64,int64>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0198:  nop
    IL_0199:  ret
  } // end of method ExpressionTrees::ShiftOperators

  .method public hidebysig static void  SimpleExpressions() cil managed
  {
    // Code size       147 (0x93)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7f'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<SimpleExpressions>b__7d'()
    IL_000f:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7f'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7f'
    IL_0020:  ldc.i4.0
    IL_0021:  box        [mscorlib]System.Int32
    IL_0026:  ldtoken    [mscorlib]System.Int32
    IL_002b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0030:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0035:  ldc.i4.0
    IL_0036:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0040:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32>>(!!0,
                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0045:  nop
    IL_0046:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate80'
    IL_004b:  brtrue.s   IL_0060

    IL_004d:  ldnull
    IL_004e:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<SimpleExpressions>b__7e'(int32)
    IL_0054:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0059:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate80'
    IL_005e:  br.s       IL_0060

    IL_0060:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate80'
    IL_0065:  ldtoken    [mscorlib]System.Int32
    IL_006a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006f:  ldstr      "a"
    IL_0074:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0079:  stloc.0
    IL_007a:  ldloc.0
    IL_007b:  ldc.i4.1
    IL_007c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0081:  stloc.1
    IL_0082:  ldloc.1
    IL_0083:  ldc.i4.0
    IL_0084:  ldloc.0
    IL_0085:  stelem.ref
    IL_0086:  ldloc.1
    IL_0087:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0091:  nop
    IL_0092:  ret
  } // end of method ExpressionTrees::SimpleExpressions

  .method public hidebysig static void  Capturing() cil managed
  {
    // Code size       66 (0x42)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass82' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass82'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.5
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass82'::captured
    IL_000e:  ldloc.0
    IL_000f:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass82'::'<Capturing>b__81'()
    IL_0015:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_001a:  ldloc.0
    IL_001b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0020:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass82'::captured
    IL_0025:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_002a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_002f:  ldc.i4.0
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0035:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_003a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32>>(!!0,
                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_003f:  nop
    IL_0040:  nop
    IL_0041:  ret
  } // end of method ExpressionTrees::Capturing

  .method public hidebysig static void  FieldAndPropertyAccess() cil managed
  {
    // Code size       441 (0x1b9)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldc.i4.1
    IL_0003:  box        [mscorlib]System.Int32
    IL_0008:  ldtoken    [mscorlib]System.Int32
    IL_000d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0012:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0017:  ldc.i4.0
    IL_0018:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_001d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0022:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0027:  pop
    IL_0028:  ldnull
    IL_0029:  ldnull
    IL_002a:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::StaticField
    IL_002f:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0034:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0039:  ldc.i4.0
    IL_003a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0044:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0049:  pop
    IL_004a:  ldnull
    IL_004b:  ldnull
    IL_004c:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::StaticReadonlyField
    IL_0051:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0056:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_005b:  ldc.i4.0
    IL_005c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0061:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0066:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006b:  pop
    IL_006c:  ldnull
    IL_006d:  ldnull
    IL_006e:  ldtoken    method int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_StaticProperty()
    IL_0073:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0078:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0082:  ldc.i4.0
    IL_0083:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0088:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0092:  pop
    IL_0093:  ldnull
    IL_0094:  ldnull
    IL_0095:  ldtoken    method int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_StaticReadonlyProperty()
    IL_009a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_009f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00a9:  ldc.i4.0
    IL_00aa:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00af:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b4:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b9:  pop
    IL_00ba:  ldnull
    IL_00bb:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_00c0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c5:  ldstr      "a"
    IL_00ca:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00cf:  stloc.0
    IL_00d0:  ldloc.0
    IL_00d1:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::Field
    IL_00d6:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00db:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00e0:  ldc.i4.1
    IL_00e1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e6:  stloc.1
    IL_00e7:  ldloc.1
    IL_00e8:  ldc.i4.0
    IL_00e9:  ldloc.0
    IL_00ea:  stelem.ref
    IL_00eb:  ldloc.1
    IL_00ec:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00f1:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_00f6:  pop
    IL_00f7:  ldnull
    IL_00f8:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_00fd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0102:  ldstr      "a"
    IL_0107:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_010c:  stloc.0
    IL_010d:  ldloc.0
    IL_010e:  ldtoken    method instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_Property()
    IL_0113:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0118:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_011d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0122:  ldc.i4.1
    IL_0123:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0128:  stloc.1
    IL_0129:  ldloc.1
    IL_012a:  ldc.i4.0
    IL_012b:  ldloc.0
    IL_012c:  stelem.ref
    IL_012d:  ldloc.1
    IL_012e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0133:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_0138:  pop
    IL_0139:  ldnull
    IL_013a:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_013f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0144:  ldstr      "a"
    IL_0149:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_014e:  stloc.0
    IL_014f:  ldloc.0
    IL_0150:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::ReadonlyField
    IL_0155:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_015a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_015f:  ldc.i4.1
    IL_0160:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0165:  stloc.1
    IL_0166:  ldloc.1
    IL_0167:  ldc.i4.0
    IL_0168:  ldloc.0
    IL_0169:  stelem.ref
    IL_016a:  ldloc.1
    IL_016b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0170:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_0175:  pop
    IL_0176:  ldnull
    IL_0177:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_017c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0181:  ldstr      "a"
    IL_0186:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_018b:  stloc.0
    IL_018c:  ldloc.0
    IL_018d:  ldtoken    method instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_ReadonlyProperty()
    IL_0192:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0197:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_019c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_01a1:  ldc.i4.1
    IL_01a2:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01a7:  stloc.1
    IL_01a8:  ldloc.1
    IL_01a9:  ldc.i4.0
    IL_01aa:  ldloc.0
    IL_01ab:  stelem.ref
    IL_01ac:  ldloc.1
    IL_01ad:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01b2:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_01b7:  pop
    IL_01b8:  ret
  } // end of method ExpressionTrees::FieldAndPropertyAccess

  .method public hidebysig static void  Call() cil managed
  {
    // Code size       541 (0x21d)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldtoken    [mscorlib]System.String
    IL_0007:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000c:  ldstr      "a"
    IL_0011:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0016:  stloc.0
    IL_0017:  ldnull
    IL_0018:  ldtoken    method void [mscorlib]System.Console::WriteLine(string)
    IL_001d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0022:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0027:  ldc.i4.1
    IL_0028:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002d:  stloc.1
    IL_002e:  ldloc.1
    IL_002f:  ldc.i4.0
    IL_0030:  ldloc.0
    IL_0031:  stelem.ref
    IL_0032:  ldloc.1
    IL_0033:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0038:  ldc.i4.1
    IL_0039:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003e:  stloc.2
    IL_003f:  ldloc.2
    IL_0040:  ldc.i4.0
    IL_0041:  ldloc.0
    IL_0042:  stelem.ref
    IL_0043:  ldloc.2
    IL_0044:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Action`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                       class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0049:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Action`1<!!0>>)
    IL_004e:  pop
    IL_004f:  ldsfld     class [mscorlib]System.Func`2<string,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate88'
    IL_0054:  brtrue.s   IL_0069

    IL_0056:  ldnull
    IL_0057:  ldftn      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<Call>b__84'(string)
    IL_005d:  newobj     instance void class [mscorlib]System.Func`2<string,string>::.ctor(object,
                                                                                           native int)
    IL_0062:  stsfld     class [mscorlib]System.Func`2<string,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate88'
    IL_0067:  br.s       IL_0069

    IL_0069:  ldsfld     class [mscorlib]System.Func`2<string,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate88'
    IL_006e:  ldtoken    [mscorlib]System.String
    IL_0073:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0078:  ldstr      "a"
    IL_007d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0082:  stloc.0
    IL_0083:  ldloc.0
    IL_0084:  ldtoken    method instance string [mscorlib]System.Object::ToString()
    IL_0089:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_008e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0093:  ldc.i4.0
    IL_0094:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0099:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_009e:  ldc.i4.1
    IL_009f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a4:  stloc.2
    IL_00a5:  ldloc.2
    IL_00a6:  ldc.i4.0
    IL_00a7:  ldloc.0
    IL_00a8:  stelem.ref
    IL_00a9:  ldloc.2
    IL_00aa:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00af:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<string,string>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00b4:  nop
    IL_00b5:  ldsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate89'
    IL_00ba:  brtrue.s   IL_00cf

    IL_00bc:  ldnull
    IL_00bd:  ldftn      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<Call>b__85'(int32)
    IL_00c3:  newobj     instance void class [mscorlib]System.Func`2<int32,string>::.ctor(object,
                                                                                          native int)
    IL_00c8:  stsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate89'
    IL_00cd:  br.s       IL_00cf

    IL_00cf:  ldsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate89'
    IL_00d4:  ldtoken    [mscorlib]System.Int32
    IL_00d9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00de:  ldstr      "a"
    IL_00e3:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00e8:  stloc.0
    IL_00e9:  ldloc.0
    IL_00ea:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_00ef:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00f4:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00f9:  ldc.i4.0
    IL_00fa:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ff:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0104:  ldc.i4.1
    IL_0105:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_010a:  stloc.2
    IL_010b:  ldloc.2
    IL_010c:  ldc.i4.0
    IL_010d:  ldloc.0
    IL_010e:  stelem.ref
    IL_010f:  ldloc.2
    IL_0110:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                           class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0115:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,string>>(!!0,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_011a:  nop
    IL_011b:  ldsfld     class [mscorlib]System.Func`2<string,char[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8a'
    IL_0120:  brtrue.s   IL_0135

    IL_0122:  ldnull
    IL_0123:  ldftn      char[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<Call>b__86'(string)
    IL_0129:  newobj     instance void class [mscorlib]System.Func`2<string,char[]>::.ctor(object,
                                                                                           native int)
    IL_012e:  stsfld     class [mscorlib]System.Func`2<string,char[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8a'
    IL_0133:  br.s       IL_0135

    IL_0135:  ldsfld     class [mscorlib]System.Func`2<string,char[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8a'
    IL_013a:  ldtoken    [mscorlib]System.String
    IL_013f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0144:  ldstr      "a"
    IL_0149:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_014e:  stloc.0
    IL_014f:  ldnull
    IL_0150:  ldtoken    method !!0[] [System.Core]System.Linq.Enumerable::ToArray<char>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0155:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_015a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_015f:  ldc.i4.1
    IL_0160:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0165:  stloc.1
    IL_0166:  ldloc.1
    IL_0167:  ldc.i4.0
    IL_0168:  ldloc.0
    IL_0169:  stelem.ref
    IL_016a:  ldloc.1
    IL_016b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0170:  ldc.i4.1
    IL_0171:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0176:  stloc.2
    IL_0177:  ldloc.2
    IL_0178:  ldc.i4.0
    IL_0179:  ldloc.0
    IL_017a:  stelem.ref
    IL_017b:  ldloc.2
    IL_017c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,char[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0181:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<string,char[]>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0186:  nop
    IL_0187:  ldsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8b'
    IL_018c:  brtrue.s   IL_01a1

    IL_018e:  ldnull
    IL_018f:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<Call>b__87'()
    IL_0195:  newobj     instance void class [mscorlib]System.Func`1<bool>::.ctor(object,
                                                                                  native int)
    IL_019a:  stsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8b'
    IL_019f:  br.s       IL_01a1

    IL_01a1:  ldsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8b'
    IL_01a6:  ldc.i4.s   97
    IL_01a8:  box        [mscorlib]System.Char
    IL_01ad:  ldtoken    [mscorlib]System.Char
    IL_01b2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01bc:  ldtoken    method instance int32 [mscorlib]System.Char::CompareTo(char)
    IL_01c1:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01c6:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01cb:  ldc.i4.1
    IL_01cc:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01d1:  stloc.1
    IL_01d2:  ldloc.1
    IL_01d3:  ldc.i4.0
    IL_01d4:  ldc.i4.s   98
    IL_01d6:  box        [mscorlib]System.Char
    IL_01db:  ldtoken    [mscorlib]System.Char
    IL_01e0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01e5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01ea:  stelem.ref
    IL_01eb:  ldloc.1
    IL_01ec:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01f1:  ldc.i4.0
    IL_01f2:  box        [mscorlib]System.Int32
    IL_01f7:  ldtoken    [mscorlib]System.Int32
    IL_01fc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0201:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0206:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_020b:  ldc.i4.0
    IL_020c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0211:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0216:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<bool>>(!!0,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_021b:  nop
    IL_021c:  ret
  } // end of method ExpressionTrees::Call

  .method public hidebysig static void  Quote() cil managed
  {
    // Code size       207 (0xcf)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8d'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<Quote>b__8c'()
    IL_000f:  newobj     instance void class [mscorlib]System.Func`1<bool>::.ctor(object,
                                                                                  native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8d'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate8d'
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "n"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldtoken    [mscorlib]System.String
    IL_003a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003f:  ldstr      "s"
    IL_0044:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0049:  stloc.1
    IL_004a:  ldloc.1
    IL_004b:  ldloc.0
    IL_004c:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_0051:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0056:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005b:  ldc.i4.0
    IL_005c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0061:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0066:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_006b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0070:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0075:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_007a:  ldc.i4.2
    IL_007b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0080:  stloc.2
    IL_0081:  ldloc.2
    IL_0082:  ldc.i4.0
    IL_0083:  ldloc.0
    IL_0084:  stelem.ref
    IL_0085:  ldloc.2
    IL_0086:  ldc.i4.1
    IL_0087:  ldloc.1
    IL_0088:  stelem.ref
    IL_0089:  ldloc.2
    IL_008a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008f:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_0094:  ldtoken    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<int32,string,string>>
    IL_0099:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00a3:  ldnull
    IL_00a4:  box        [mscorlib]System.Object
    IL_00a9:  ldtoken    [mscorlib]System.Object
    IL_00ae:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b8:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00bd:  ldc.i4.0
    IL_00be:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00c3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00c8:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<bool>>(!!0,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00cd:  nop
    IL_00ce:  ret
  } // end of method ExpressionTrees::Quote

  .method public hidebysig static void  ArrayInitializer() cil managed
  {
    // Code size       623 (0x26f)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate93'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayInitializer>b__8e'()
    IL_000f:  newobj     instance void class [mscorlib]System.Func`1<int32[]>::.ctor(object,
                                                                                     native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate93'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate93'
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldc.i4.3
    IL_002b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0030:  stloc.0
    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.0
    IL_0033:  ldc.i4.1
    IL_0034:  box        [mscorlib]System.Int32
    IL_0039:  ldtoken    [mscorlib]System.Int32
    IL_003e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0043:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0048:  stelem.ref
    IL_0049:  ldloc.0
    IL_004a:  ldc.i4.1
    IL_004b:  ldc.i4.2
    IL_004c:  box        [mscorlib]System.Int32
    IL_0051:  ldtoken    [mscorlib]System.Int32
    IL_0056:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0060:  stelem.ref
    IL_0061:  ldloc.0
    IL_0062:  ldc.i4.2
    IL_0063:  ldc.i4.3
    IL_0064:  box        [mscorlib]System.Int32
    IL_0069:  ldtoken    [mscorlib]System.Int32
    IL_006e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0073:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0078:  stelem.ref
    IL_0079:  ldloc.0
    IL_007a:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_007f:  ldc.i4.0
    IL_0080:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0085:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[]>>(!!0,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_008f:  nop
    IL_0090:  ldsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate94'
    IL_0095:  brtrue.s   IL_00aa

    IL_0097:  ldnull
    IL_0098:  ldftn      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayInitializer>b__8f'()
    IL_009e:  newobj     instance void class [mscorlib]System.Func`1<int32[]>::.ctor(object,
                                                                                     native int)
    IL_00a3:  stsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate94'
    IL_00a8:  br.s       IL_00aa

    IL_00aa:  ldsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate94'
    IL_00af:  ldtoken    [mscorlib]System.Int32
    IL_00b4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b9:  ldc.i4.1
    IL_00ba:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00bf:  stloc.0
    IL_00c0:  ldloc.0
    IL_00c1:  ldc.i4.0
    IL_00c2:  ldc.i4.3
    IL_00c3:  box        [mscorlib]System.Int32
    IL_00c8:  ldtoken    [mscorlib]System.Int32
    IL_00cd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d7:  stelem.ref
    IL_00d8:  ldloc.0
    IL_00d9:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_00de:  ldc.i4.0
    IL_00df:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e9:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[]>>(!!0,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00ee:  nop
    IL_00ef:  ldsfld     class [mscorlib]System.Func`1<int32[0...,0...]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate95'
    IL_00f4:  brtrue.s   IL_0109

    IL_00f6:  ldnull
    IL_00f7:  ldftn      int32[0...,0...] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayInitializer>b__90'()
    IL_00fd:  newobj     instance void class [mscorlib]System.Func`1<int32[0...,0...]>::.ctor(object,
                                                                                              native int)
    IL_0102:  stsfld     class [mscorlib]System.Func`1<int32[0...,0...]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate95'
    IL_0107:  br.s       IL_0109

    IL_0109:  ldsfld     class [mscorlib]System.Func`1<int32[0...,0...]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate95'
    IL_010e:  ldtoken    [mscorlib]System.Int32
    IL_0113:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0118:  ldc.i4.2
    IL_0119:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_011e:  stloc.0
    IL_011f:  ldloc.0
    IL_0120:  ldc.i4.0
    IL_0121:  ldc.i4.3
    IL_0122:  box        [mscorlib]System.Int32
    IL_0127:  ldtoken    [mscorlib]System.Int32
    IL_012c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0131:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0136:  stelem.ref
    IL_0137:  ldloc.0
    IL_0138:  ldc.i4.1
    IL_0139:  ldc.i4.5
    IL_013a:  box        [mscorlib]System.Int32
    IL_013f:  ldtoken    [mscorlib]System.Int32
    IL_0144:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0149:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_014e:  stelem.ref
    IL_014f:  ldloc.0
    IL_0150:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0155:  ldc.i4.0
    IL_0156:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_015b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[0...,0...]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0160:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[0...,0...]>>(!!0,
                                                                                                                                                   class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0165:  nop
    IL_0166:  ldsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate96'
    IL_016b:  brtrue.s   IL_0180

    IL_016d:  ldnull
    IL_016e:  ldftn      int32[][] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayInitializer>b__91'()
    IL_0174:  newobj     instance void class [mscorlib]System.Func`1<int32[][]>::.ctor(object,
                                                                                       native int)
    IL_0179:  stsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate96'
    IL_017e:  br.s       IL_0180

    IL_0180:  ldsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate96'
    IL_0185:  ldtoken    int32[]
    IL_018a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_018f:  ldc.i4.1
    IL_0190:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0195:  stloc.0
    IL_0196:  ldloc.0
    IL_0197:  ldc.i4.0
    IL_0198:  ldc.i4.3
    IL_0199:  box        [mscorlib]System.Int32
    IL_019e:  ldtoken    [mscorlib]System.Int32
    IL_01a3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01ad:  stelem.ref
    IL_01ae:  ldloc.0
    IL_01af:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_01b4:  ldc.i4.0
    IL_01b5:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01ba:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[][]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01bf:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[][]>>(!!0,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01c4:  nop
    IL_01c5:  ldsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate97'
    IL_01ca:  brtrue.s   IL_01df

    IL_01cc:  ldnull
    IL_01cd:  ldftn      int32[][] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<ArrayInitializer>b__92'()
    IL_01d3:  newobj     instance void class [mscorlib]System.Func`1<int32[][]>::.ctor(object,
                                                                                       native int)
    IL_01d8:  stsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate97'
    IL_01dd:  br.s       IL_01df

    IL_01df:  ldsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate97'
    IL_01e4:  ldtoken    int32[]
    IL_01e9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ee:  ldc.i4.1
    IL_01ef:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01f4:  stloc.0
    IL_01f5:  ldloc.0
    IL_01f6:  ldc.i4.0
    IL_01f7:  ldtoken    [mscorlib]System.Int32
    IL_01fc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0201:  ldc.i4.3
    IL_0202:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0207:  stloc.1
    IL_0208:  ldloc.1
    IL_0209:  ldc.i4.0
    IL_020a:  ldc.i4.1
    IL_020b:  box        [mscorlib]System.Int32
    IL_0210:  ldtoken    [mscorlib]System.Int32
    IL_0215:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_021a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_021f:  stelem.ref
    IL_0220:  ldloc.1
    IL_0221:  ldc.i4.1
    IL_0222:  ldc.i4.2
    IL_0223:  box        [mscorlib]System.Int32
    IL_0228:  ldtoken    [mscorlib]System.Int32
    IL_022d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0232:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0237:  stelem.ref
    IL_0238:  ldloc.1
    IL_0239:  ldc.i4.2
    IL_023a:  ldc.i4.3
    IL_023b:  box        [mscorlib]System.Int32
    IL_0240:  ldtoken    [mscorlib]System.Int32
    IL_0245:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_024a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_024f:  stelem.ref
    IL_0250:  ldloc.1
    IL_0251:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0256:  stelem.ref
    IL_0257:  ldloc.0
    IL_0258:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_025d:  ldc.i4.0
    IL_025e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0263:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[][]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0268:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[][]>>(!!0,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_026d:  nop
    IL_026e:  ret
  } // end of method ExpressionTrees::ArrayInitializer

  .method public hidebysig static void  AnonymousTypes() cil managed
  {
    // Code size       184 (0xb8)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [mscorlib]System.Reflection.MethodInfo[] V_1)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate99'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'<AnonymousTypes>b__98'()
    IL_000f:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_0014:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate99'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate99'
    IL_0020:  ldtoken    method instance void class '<>f__AnonymousType2`2'<int32,string>::.ctor(!0,
                                                                                                 !1)
    IL_0025:  ldtoken    class '<>f__AnonymousType2`2'<int32,string>
    IL_002a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0034:  ldc.i4.2
    IL_0035:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_003a:  stloc.0
    IL_003b:  ldloc.0
    IL_003c:  ldc.i4.0
    IL_003d:  ldc.i4.5
    IL_003e:  box        [mscorlib]System.Int32
    IL_0043:  ldtoken    [mscorlib]System.Int32
    IL_0048:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0052:  stelem.ref
    IL_0053:  ldloc.0
    IL_0054:  ldc.i4.1
    IL_0055:  ldstr      "Test"
    IL_005a:  ldtoken    [mscorlib]System.String
    IL_005f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0064:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0069:  stelem.ref
    IL_006a:  ldloc.0
    IL_006b:  ldc.i4.2
    IL_006c:  newarr     [mscorlib]System.Reflection.MethodInfo
    IL_0071:  stloc.1
    IL_0072:  ldloc.1
    IL_0073:  ldc.i4.0
    IL_0074:  ldtoken    method instance !0 class '<>f__AnonymousType2`2'<int32,string>::get_A()
    IL_0079:  ldtoken    class '<>f__AnonymousType2`2'<int32,string>
    IL_007e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0083:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0088:  stelem.ref
    IL_0089:  ldloc.1
    IL_008a:  ldc.i4.1
    IL_008b:  ldtoken    method instance !1 class '<>f__AnonymousType2`2'<int32,string>::get_B()
    IL_0090:  ldtoken    class '<>f__AnonymousType2`2'<int32,string>
    IL_0095:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009f:  stelem.ref
    IL_00a0:  ldloc.1
    IL_00a1:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_00a6:  ldc.i4.0
    IL_00a7:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00ac:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b1:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00b6:  nop
    IL_00b7:  ret
  } // end of method ExpressionTrees::AnonymousTypes

  .method public hidebysig static void  ObjectInit() cil managed
  {
    // Code size       142 (0x8e)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.MemberBinding[] V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::.ctor()
    IL_0007:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000c:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0011:  ldc.i4.0
    IL_0012:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0017:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_001c:  ldc.i4.2
    IL_001d:  newarr     [System.Core]System.Linq.Expressions.MemberBinding
    IL_0022:  stloc.0
    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.0
    IL_0025:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::set_Property(int32)
    IL_002a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0034:  ldc.i4.4
    IL_0035:  box        [mscorlib]System.Int32
    IL_003a:  ldtoken    [mscorlib]System.Int32
    IL_003f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0044:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0049:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_004e:  stelem.ref
    IL_004f:  ldloc.0
    IL_0050:  ldc.i4.1
    IL_0051:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::Field
    IL_0056:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_005b:  ldc.i4.3
    IL_005c:  box        [mscorlib]System.Int32
    IL_0061:  ldtoken    [mscorlib]System.Int32
    IL_0066:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0070:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MemberInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_0075:  stelem.ref
    IL_0076:  ldloc.0
    IL_0077:  call       class [System.Core]System.Linq.Expressions.MemberInitExpression [System.Core]System.Linq.Expressions.Expression::MemberInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.MemberBinding[])
    IL_007c:  ldc.i4.0
    IL_007d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0082:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                             class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0087:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType>(object,
                                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008c:  pop
    IL_008d:  ret
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
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  ldarga.s   n
    IL_0002:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method ExpressionTrees::'<IndexerAccess>b__6'

  .method private hidebysig static bool  '<MethodGroupConstant>b__e'(class [mscorlib]System.Func`3<object,object,bool> f) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       13 (0xd)
    .maxstack  3
    .locals init (bool V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !2 class [mscorlib]System.Func`3<object,object,bool>::Invoke(!0,
                                                                                               !1)
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method ExpressionTrees::'<MethodGroupConstant>b__e'

  .method private hidebysig static int32 
          '<NestedLambda>b__12'(class [mscorlib]System.Func`1<int32> f) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method ExpressionTrees::'<NestedLambda>b__12'

  .method private hidebysig static int32 
          '<ArrayIndexer>b__1d'(int32[] 'array') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldelem.i4
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<ArrayIndexer>b__1d'

  .method private hidebysig static int32 
          '<ArrayIndexer>b__1e'(int32[] 'array',
                                int32 index) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldelem.i4
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<ArrayIndexer>b__1e'

  .method private hidebysig static int32 
          '<ArrayIndexer>b__1f'(int32[0...,0...] 'array') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       13 (0xd)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.5
    IL_0003:  call       instance int32 int32[0...,0...]::Get(int32,
                                                              int32)
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method ExpressionTrees::'<ArrayIndexer>b__1f'

  .method private hidebysig static int32 
          '<ArrayIndexer>b__20'(int32[0...,0...] 'array',
                                int32 index) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       13 (0xd)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.7
    IL_0003:  call       instance int32 int32[0...,0...]::Get(int32,
                                                              int32)
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method ExpressionTrees::'<ArrayIndexer>b__20'

  .method private hidebysig static int32 
          '<ArrayIndexer>b__21'(int32[][] 'array',
                                int32 index) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       10 (0xa)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldelem.ref
    IL_0003:  ldc.i4.7
    IL_0004:  ldelem.i4
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method ExpressionTrees::'<ArrayIndexer>b__21'

  .method private hidebysig static int32 
          '<ArrayLength>b__27'(int32[] 'array') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldlen
    IL_0002:  conv.i4
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<ArrayLength>b__27'

  .method private hidebysig static int32 
          '<ArrayLength>b__28'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldnull
    IL_0001:  callvirt   instance int32 [mscorlib]System.Array::get_Length()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method ExpressionTrees::'<ArrayLength>b__28'

  .method private hidebysig static object 
          '<NewObj>b__2b'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::.ctor()
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method ExpressionTrees::'<NewObj>b__2b'

  .method private hidebysig static object 
          '<NewObj>b__2c'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  ldc.i4.5
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithCtor::.ctor(int32)
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method ExpressionTrees::'<NewObj>b__2c'

  .method private hidebysig static object 
          '<NewObj>b__2d'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors::.ctor()
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method ExpressionTrees::'<NewObj>b__2d'

  .method private hidebysig static object 
          '<NewObj>b__2e'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  ldc.i4.5
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors::.ctor(int32)
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method ExpressionTrees::'<NewObj>b__2e'

  .method private hidebysig static object 
          '<NewObj>b__2f'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method ExpressionTrees::'<NewObj>b__2f'

  .method private hidebysig static object 
          '<NewObj>b__30'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1<int32>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method ExpressionTrees::'<NewObj>b__30'

  .method private hidebysig static object 
          '<NewObj>b__31'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  ldc.i4.5
    IL_0001:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1<int32>::.ctor(int32)
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method ExpressionTrees::'<NewObj>b__31'

  .method private hidebysig static class [mscorlib]System.Type 
          '<TypeOfExpr>b__39'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       15 (0xf)
    .maxstack  1
    .locals init (class [mscorlib]System.Type V_0)
    IL_0000:  ldtoken    [mscorlib]System.Int32
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method ExpressionTrees::'<TypeOfExpr>b__39'

  .method private hidebysig static class [mscorlib]System.Type 
          '<TypeOfExpr>b__3a'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       15 (0xf)
    .maxstack  1
    .locals init (class [mscorlib]System.Type V_0)
    IL_0000:  ldtoken    [mscorlib]System.Object
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method ExpressionTrees::'<TypeOfExpr>b__3a'

  .method private hidebysig static class [mscorlib]System.Type 
          '<TypeOfExpr>b__3b'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       15 (0xf)
    .maxstack  1
    .locals init (class [mscorlib]System.Type V_0)
    IL_0000:  ldtoken    [mscorlib]System.Collections.Generic.List`1
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method ExpressionTrees::'<TypeOfExpr>b__3b'

  .method private hidebysig static class [mscorlib]System.Type 
          '<TypeOfExpr>b__3c'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       15 (0xf)
    .maxstack  1
    .locals init (class [mscorlib]System.Type V_0)
    IL_0000:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method ExpressionTrees::'<TypeOfExpr>b__3c'

  .method private hidebysig static class [mscorlib]System.Type 
          '<TypeOfExpr>b__3d'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       15 (0xf)
    .maxstack  1
    .locals init (class [mscorlib]System.Type V_0)
    IL_0000:  ldtoken    int32*
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method ExpressionTrees::'<TypeOfExpr>b__3d'

  .method private hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass 
          '<AsTypeExpr>b__43'(object obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass V_0)
    IL_0000:  ldarg.0
    IL_0001:  isinst     ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method ExpressionTrees::'<AsTypeExpr>b__43'

  .method private hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object> 
          '<AsTypeExpr>b__44'(object obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object> V_0)
    IL_0000:  ldarg.0
    IL_0001:  isinst     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method ExpressionTrees::'<AsTypeExpr>b__44'

  .method private hidebysig static bool  '<IsTypeExpr>b__47'(object obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       14 (0xe)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  ldarg.0
    IL_0001:  isinst     ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
    IL_0006:  ldnull
    IL_0007:  cgt.un
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method ExpressionTrees::'<IsTypeExpr>b__47'

  .method private hidebysig static bool  '<UnaryLogicalOperators>b__49'(bool a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ceq
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method ExpressionTrees::'<UnaryLogicalOperators>b__49'

  .method private hidebysig static int32 
          '<UnaryArithmeticOperators>b__4b'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0004

    IL_0004:  ldloc.0
    IL_0005:  ret
  } // end of method ExpressionTrees::'<UnaryArithmeticOperators>b__4b'

  .method private hidebysig static int32 
          '<UnaryArithmeticOperators>b__4c'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  neg
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ExpressionTrees::'<UnaryArithmeticOperators>b__4c'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__4f'(int32 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__4f'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__50'(int32 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  sub
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__50'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__51'(int32 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  mul
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__51'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__52'(int32 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  div
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__52'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__53'(int32 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  rem
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__53'

  .method private hidebysig static int64 
          '<BinaryArithmeticOperators>b__54'(int64 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (int64 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  conv.i8
    IL_0003:  add
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__54'

  .method private hidebysig static int64 
          '<BinaryArithmeticOperators>b__55'(int64 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (int64 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  conv.i8
    IL_0003:  sub
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__55'

  .method private hidebysig static int64 
          '<BinaryArithmeticOperators>b__56'(int64 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (int64 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  conv.i8
    IL_0003:  mul
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__56'

  .method private hidebysig static int64 
          '<BinaryArithmeticOperators>b__57'(int64 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (int64 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  conv.i8
    IL_0003:  div
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__57'

  .method private hidebysig static int64 
          '<BinaryArithmeticOperators>b__58'(int64 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (int64 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  conv.i8
    IL_0003:  rem
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__58'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__59'(int16 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__59'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__5a'(int32 a,
                                             int16 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  sub
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__5a'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__5b'(int16 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  mul
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__5b'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__5c'(int32 a,
                                             int16 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  div
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__5c'

  .method private hidebysig static int32 
          '<BinaryArithmeticOperators>b__5d'(int16 a,
                                             int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  rem
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<BinaryArithmeticOperators>b__5d'

  .method private hidebysig static int32 
          '<BitOperators>b__6d'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  not
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ExpressionTrees::'<BitOperators>b__6d'

  .method private hidebysig static int32 
          '<BitOperators>b__6e'(int32 a,
                                int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  and
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<BitOperators>b__6e'

  .method private hidebysig static int32 
          '<BitOperators>b__6f'(int32 a,
                                int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  or
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<BitOperators>b__6f'

  .method private hidebysig static int32 
          '<BitOperators>b__70'(int32 a,
                                int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  xor
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<BitOperators>b__70'

  .method private hidebysig static int32 
          '<ShiftOperators>b__75'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  shr
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<ShiftOperators>b__75'

  .method private hidebysig static int32 
          '<ShiftOperators>b__76'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  shl
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<ShiftOperators>b__76'

  .method private hidebysig static int64 
          '<ShiftOperators>b__77'(int64 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int64 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  shr
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<ShiftOperators>b__77'

  .method private hidebysig static int64 
          '<ShiftOperators>b__78'(int64 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  2
    .locals init (int64 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  shl
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method ExpressionTrees::'<ShiftOperators>b__78'

  .method private hidebysig static int32 
          '<SimpleExpressions>b__7d'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0004

    IL_0004:  ldloc.0
    IL_0005:  ret
  } // end of method ExpressionTrees::'<SimpleExpressions>b__7d'

  .method private hidebysig static int32 
          '<SimpleExpressions>b__7e'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0004

    IL_0004:  ldloc.0
    IL_0005:  ret
  } // end of method ExpressionTrees::'<SimpleExpressions>b__7e'

  .method private hidebysig static string 
          '<Call>b__84'(string a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method ExpressionTrees::'<Call>b__84'

  .method private hidebysig static string 
          '<Call>b__85'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  ldarga.s   a
    IL_0002:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method ExpressionTrees::'<Call>b__85'

  .method private hidebysig static char[] 
          '<Call>b__86'(string a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (char[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<char>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method ExpressionTrees::'<Call>b__86'

  .method private hidebysig static bool  '<Call>b__87'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       20 (0x14)
    .maxstack  2
    .locals init (bool V_0,
             char V_1)
    IL_0000:  ldc.i4.s   97
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  ldc.i4.s   98
    IL_0007:  call       instance int32 [mscorlib]System.Char::CompareTo(char)
    IL_000c:  ldc.i4.0
    IL_000d:  clt
    IL_000f:  stloc.0
    IL_0010:  br.s       IL_0012

    IL_0012:  ldloc.0
    IL_0013:  ret
  } // end of method ExpressionTrees::'<Call>b__87'

  .method private hidebysig static bool  '<Quote>b__8c'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       122 (0x7a)
    .maxstack  4
    .locals init (bool V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_3)
    IL_0000:  ldtoken    [mscorlib]System.Int32
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  ldstr      "n"
    IL_000f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0014:  stloc.1
    IL_0015:  ldtoken    [mscorlib]System.String
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  ldstr      "s"
    IL_0024:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0029:  stloc.2
    IL_002a:  ldloc.2
    IL_002b:  ldloc.1
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
    IL_0060:  stloc.3
    IL_0061:  ldloc.3
    IL_0062:  ldc.i4.0
    IL_0063:  ldloc.1
    IL_0064:  stelem.ref
    IL_0065:  ldloc.3
    IL_0066:  ldc.i4.1
    IL_0067:  ldloc.2
    IL_0068:  stelem.ref
    IL_0069:  ldloc.3
    IL_006a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006f:  ldnull
    IL_0070:  ceq
    IL_0072:  ldc.i4.0
    IL_0073:  ceq
    IL_0075:  stloc.0
    IL_0076:  br.s       IL_0078

    IL_0078:  ldloc.0
    IL_0079:  ret
  } // end of method ExpressionTrees::'<Quote>b__8c'

  .method private hidebysig static int32[] 
          '<ArrayInitializer>b__8e'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       22 (0x16)
    .maxstack  3
    .locals init (int32[] V_0)
    IL_0000:  ldc.i4.3
    IL_0001:  newarr     [mscorlib]System.Int32
    IL_0006:  dup
    IL_0007:  ldtoken    field valuetype '<PrivateImplementationDetails>{426C7905-90B3-4EA8-8585-F907E00CBDB7}'/'__StaticArrayInitTypeSize=12' '<PrivateImplementationDetails>{426C7905-90B3-4EA8-8585-F907E00CBDB7}'::'$$method0x60000bc-1'
    IL_000c:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_0014

    IL_0014:  ldloc.0
    IL_0015:  ret
  } // end of method ExpressionTrees::'<ArrayInitializer>b__8e'

  .method private hidebysig static int32[] 
          '<ArrayInitializer>b__8f'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (int32[] V_0)
    IL_0000:  ldc.i4.3
    IL_0001:  newarr     [mscorlib]System.Int32
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method ExpressionTrees::'<ArrayInitializer>b__8f'

  .method private hidebysig static int32[0...,0...] 
          '<ArrayInitializer>b__90'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       12 (0xc)
    .maxstack  2
    .locals init (int32[0...,0...] V_0)
    IL_0000:  ldc.i4.3
    IL_0001:  ldc.i4.5
    IL_0002:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method ExpressionTrees::'<ArrayInitializer>b__90'

  .method private hidebysig static int32[][] 
          '<ArrayInitializer>b__91'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (int32[][] V_0)
    IL_0000:  ldc.i4.3
    IL_0001:  newarr     int32[]
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method ExpressionTrees::'<ArrayInitializer>b__91'

  .method private hidebysig static int32[][] 
          '<ArrayInitializer>b__92'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       33 (0x21)
    .maxstack  5
    .locals init (int32[][] V_0,
             int32[][] V_1)
    IL_0000:  ldc.i4.1
    IL_0001:  newarr     int32[]
    IL_0006:  stloc.1
    IL_0007:  ldloc.1
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.3
    IL_000a:  newarr     [mscorlib]System.Int32
    IL_000f:  dup
    IL_0010:  ldtoken    field valuetype '<PrivateImplementationDetails>{426C7905-90B3-4EA8-8585-F907E00CBDB7}'/'__StaticArrayInitTypeSize=12' '<PrivateImplementationDetails>{426C7905-90B3-4EA8-8585-F907E00CBDB7}'::'$$method0x60000c0-1'
    IL_0015:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_001a:  stelem.ref
    IL_001b:  ldloc.1
    IL_001c:  stloc.0
    IL_001d:  br.s       IL_001f

    IL_001f:  ldloc.0
    IL_0020:  ret
  } // end of method ExpressionTrees::'<ArrayInitializer>b__92'

  .method private hidebysig static object 
          '<AnonymousTypes>b__98'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       16 (0x10)
    .maxstack  2
    .locals init (object V_0)
    IL_0000:  ldc.i4.5
    IL_0001:  ldstr      "Test"
    IL_0006:  newobj     instance void class '<>f__AnonymousType2`2'<int32,string>::.ctor(!0,
                                                                                          !1)
    IL_000b:  stloc.0
    IL_000c:  br.s       IL_000e

    IL_000e:  ldloc.0
    IL_000f:  ret
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
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass::.ctor()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
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
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method SimpleType::get_StaticReadonlyProperty

  .method public hidebysig specialname static 
          int32  get_StaticProperty() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::'<StaticProperty>k__BackingField'
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
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
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method SimpleType::get_ReadonlyProperty

  .method public hidebysig specialname instance int32 
          get_Property() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::'<Property>k__BackingField'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
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
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::ReadonlyField
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.3
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::Field
    IL_000e:  ldarg.0
    IL_000f:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0014:  nop
    IL_0015:  ret
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
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  nop
    IL_0009:  ret
  } // end of method SimpleTypeWithCtor::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithCtor

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  nop
    IL_0009:  ret
  } // end of method SimpleTypeWithMultipleCtors::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 i) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  nop
    IL_0009:  ret
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
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  nop
    IL_0009:  ret
  } // end of method GenericClassWithMultipleCtors`1::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 x) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  nop
    IL_0009:  ret
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
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<X>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType0`2'::get_X

  .method public hidebysig specialname instance !'<A>j__TPar' 
          get_A() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<A>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType0`2'::get_A

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       89 (0x59)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0,
             string V_1)
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
    IL_0054:  stloc.1
    IL_0055:  br.s       IL_0057

    IL_0057:  ldloc.1
    IL_0058:  ret
  } // end of method '<>f__AnonymousType0`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       65 (0x41)
    .maxstack  3
    .locals init (class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'> V_0,
             bool V_1)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_003a

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_003a

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  br.s       IL_003b

    IL_003a:  ldc.i4.0
    IL_003b:  nop
    IL_003c:  stloc.1
    IL_003d:  br.s       IL_003f

    IL_003f:  ldloc.1
    IL_0040:  ret
  } // end of method '<>f__AnonymousType0`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       62 (0x3e)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
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
    IL_0039:  stloc.1
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.1
    IL_003d:  ret
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
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<X>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType1`2'::get_X

  .method public hidebysig specialname instance !'<Y>j__TPar' 
          get_Y() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<Y>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType1`2'::get_Y

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       89 (0x59)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0,
             string V_1)
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
    IL_0054:  stloc.1
    IL_0055:  br.s       IL_0057

    IL_0057:  ldloc.1
    IL_0058:  ret
  } // end of method '<>f__AnonymousType1`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       65 (0x41)
    .maxstack  3
    .locals init (class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'> V_0,
             bool V_1)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_003a

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_003a

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  br.s       IL_003b

    IL_003a:  ldc.i4.0
    IL_003b:  nop
    IL_003c:  stloc.1
    IL_003d:  br.s       IL_003f

    IL_003f:  ldloc.1
    IL_0040:  ret
  } // end of method '<>f__AnonymousType1`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       62 (0x3e)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
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
    IL_0039:  stloc.1
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.1
    IL_003d:  ret
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

.class private auto ansi '<PrivateImplementationDetails>{426C7905-90B3-4EA8-8585-F907E00CBDB7}'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=12'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 12
  } // end of class '__StaticArrayInitTypeSize=12'

  .field static assembly valuetype '<PrivateImplementationDetails>{426C7905-90B3-4EA8-8585-F907E00CBDB7}'/'__StaticArrayInitTypeSize=12' '$$method0x60000bc-1' at I_00007A38
  .field static assembly valuetype '<PrivateImplementationDetails>{426C7905-90B3-4EA8-8585-F907E00CBDB7}'/'__StaticArrayInitTypeSize=12' '$$method0x60000c0-1' at I_00007AB0
} // end of class '<PrivateImplementationDetails>{426C7905-90B3-4EA8-8585-F907E00CBDB7}'

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
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<A>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType2`2'::get_A

  .method public hidebysig specialname instance !'<B>j__TPar' 
          get_B() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<B>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType2`2'::get_B

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       89 (0x59)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0,
             string V_1)
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
    IL_0054:  stloc.1
    IL_0055:  br.s       IL_0057

    IL_0057:  ldloc.1
    IL_0058:  ret
  } // end of method '<>f__AnonymousType2`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       65 (0x41)
    .maxstack  3
    .locals init (class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0,
             bool V_1)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_003a

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_003a

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<B>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<B>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  br.s       IL_003b

    IL_003a:  ldc.i4.0
    IL_003b:  nop
    IL_003c:  stloc.1
    IL_003d:  br.s       IL_003f

    IL_003f:  ldloc.1
    IL_0040:  ret
  } // end of method '<>f__AnonymousType2`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       62 (0x3e)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
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
    IL_0039:  stloc.1
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.1
    IL_003d:  ret
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

.data cil I_00007A38 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00) 
.data cil I_00007A44 = int8[12]
.data cil I_00007AB0 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00) 
// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\ExpressionTrees.res
