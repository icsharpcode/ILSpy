



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
.assembly QualifierTests
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
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
.module QualifierTests.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.Pretty.QualifierTests
       extends [mscorlib]System.Object
{
  .class sequential ansi sealed nested private beforefieldinit Test
         extends [mscorlib]System.ValueType
  {
    .field private int32 dummy
    .method private hidebysig instance void 
            DeclaringType(class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests 'instance') cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::NoParameters()
      IL_0006:  ret
    } // end of method Test::DeclaringType

    .method private hidebysig instance void 
            DeclaringType() cil managed
    {
      // Code size       25 (0x19)
      .maxstack  8
      IL_0000:  call       void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::StaticNoParameteres()
      IL_0005:  ldarg.0
      IL_0006:  ldnull
      IL_0007:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::Parameter(class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests)
      IL_000c:  ldnull
      IL_000d:  call       void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::StaticParameter(class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests)
      IL_0012:  ldnull
      IL_0013:  call       void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::StaticParameter(object)
      IL_0018:  ret
    } // end of method Test::DeclaringType

    .method private hidebysig instance void 
            Parameter(object o) cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Test::Parameter

    .method private hidebysig static void 
            StaticParameter(object o) cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Test::StaticParameter

    .method private hidebysig instance void 
            Parameter(class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests test) cil managed
    {
      // Code size       83 (0x53)
      .maxstack  3
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  ldobj      ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test
      IL_0007:  box        ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test
      IL_000c:  ldftn      instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::Parameter(object)
      IL_0012:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                        native int)
      IL_0017:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::Delegate(class [mscorlib]System.Action`1<object>)
      IL_001c:  ldarg.0
      IL_001d:  ldnull
      IL_001e:  ldftn      void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::StaticParameter(object)
      IL_0024:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                        native int)
      IL_0029:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::Delegate(class [mscorlib]System.Action`1<object>)
      IL_002e:  ldarg.0
      IL_002f:  ldarg.1
      IL_0030:  ldftn      instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Parameter(object)
      IL_0036:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                        native int)
      IL_003b:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::Delegate(class [mscorlib]System.Action`1<object>)
      IL_0040:  ldarg.0
      IL_0041:  ldnull
      IL_0042:  ldftn      void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::StaticParameter(object)
      IL_0048:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                        native int)
      IL_004d:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::Delegate(class [mscorlib]System.Action`1<object>)
      IL_0052:  ret
    } // end of method Test::Parameter

    .method private hidebysig static void 
            StaticParameter(class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests test) cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Test::StaticParameter

    .method private hidebysig static void 
            DeclaringTypeStatic() cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Test::DeclaringTypeStatic

    .method private hidebysig instance void 
            DeclaringTypeConflict(class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests 'instance') cil managed
    {
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::DeclaringType()
      IL_0006:  ldarg.1
      IL_0007:  callvirt   instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::DeclaringType()
      IL_000c:  ldarg.0
      IL_000d:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::fieldConflict()
      IL_0012:  ldarg.1
      IL_0013:  ldc.i4.5
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::fieldConflict
      IL_0019:  ret
    } // end of method Test::DeclaringTypeConflict

    .method private hidebysig instance void 
            DeclaringTypeConflict() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  call       void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::DeclaringTypeStatic()
      IL_0005:  call       void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::DeclaringTypeStatic()
      IL_000a:  ret
    } // end of method Test::DeclaringTypeConflict

    .method private hidebysig instance void 
            fieldConflict() cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Test::fieldConflict

    .method private hidebysig instance void 
            Delegate(class [mscorlib]System.Action`1<object> action) cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Test::Delegate

  } // end of class Test

  .class auto ansi nested assembly beforefieldinit Parent
         extends [mscorlib]System.Object
  {
    .method public hidebysig newslot virtual 
            instance void  Virtual() cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Parent::Virtual

    .method public hidebysig newslot virtual 
            instance void  NewVirtual() cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Parent::NewVirtual

    .method public hidebysig instance void 
            New() cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Parent::New

    .method public hidebysig instance void 
            BaseOnly() cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Parent::BaseOnly

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method Parent::.ctor

  } // end of class Parent

  .class auto ansi nested assembly beforefieldinit Child
         extends ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent
  {
    .method public hidebysig virtual instance void 
            Virtual() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::Virtual()
      IL_0006:  ret
    } // end of method Child::Virtual

    .method public hidebysig instance void 
            NewVirtual() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::NewVirtual()
      IL_0006:  ret
    } // end of method Child::NewVirtual

    .method public hidebysig instance void 
            New() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::New()
      IL_0006:  ret
    } // end of method Child::New

    .method public hidebysig instance void 
            BaseQualifiers() cil managed
    {
      // Code size       43 (0x2b)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::Virtual()
      IL_0006:  ldarg.0
      IL_0007:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::Virtual()
      IL_000c:  ldarg.0
      IL_000d:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Child::NewVirtual()
      IL_0012:  ldarg.0
      IL_0013:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::NewVirtual()
      IL_0018:  ldarg.0
      IL_0019:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Child::New()
      IL_001e:  ldarg.0
      IL_001f:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::New()
      IL_0024:  ldarg.0
      IL_0025:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::BaseOnly()
      IL_002a:  ret
    } // end of method Child::BaseQualifiers

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::.ctor()
      IL_0006:  ret
    } // end of method Child::.ctor

  } // end of class Child

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass14_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 conflictWithVariable
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass14_0'::.ctor

    .method assembly hidebysig instance int32 
            '<Conflicts>b__0'() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass14_0'::conflictWithVariable
      IL_0006:  ret
    } // end of method '<>c__DisplayClass14_0'::'<Conflicts>b__0'

  } // end of class '<>c__DisplayClass14_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass15_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests '<>4__this'
    .field public int32 fieldConflict
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass15_0'::.ctor

    .method assembly hidebysig instance int32 
            '<Capturing>b__0'() cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0'::'<>4__this'
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::fieldConflict
      IL_000b:  ldarg.0
      IL_000c:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0'::fieldConflict
      IL_0011:  add
      IL_0012:  ret
    } // end of method '<>c__DisplayClass15_0'::'<Capturing>b__0'

    .method assembly hidebysig instance int32 
            '<Capturing>b__1'() cil managed
    {
      // Code size       63 (0x3f)
      .maxstack  4
      .locals init (class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_1' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_1'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0' ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_1'::'CS$<>8__locals1'
      IL_000d:  ldloc.0
      IL_000e:  ldc.i4.5
      IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_1'::innerConflict
      IL_0014:  ldarg.0
      IL_0015:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0'::'<>4__this'
      IL_001a:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::fieldConflict
      IL_001f:  ldarg.0
      IL_0020:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0'::fieldConflict
      IL_0025:  add
      IL_0026:  ldarg.0
      IL_0027:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0'::'<>4__this'
      IL_002c:  ldloc.0
      IL_002d:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_1'::'<Capturing>b__2'()
      IL_0033:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                     native int)
      IL_0038:  call       instance int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Capturer2(class [mscorlib]System.Func`1<int32>)
      IL_003d:  add
      IL_003e:  ret
    } // end of method '<>c__DisplayClass15_0'::'<Capturing>b__1'

  } // end of class '<>c__DisplayClass15_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass15_1'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 innerConflict
    .field public class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0' 'CS$<>8__locals1'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass15_1'::.ctor

    .method assembly hidebysig instance int32 
            '<Capturing>b__2'() cil managed
    {
      // Code size       53 (0x35)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0' ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_1'::'CS$<>8__locals1'
      IL_0006:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0'::'<>4__this'
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::innerConflict
      IL_0010:  ldarg.0
      IL_0011:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_1'::innerConflict
      IL_0016:  add
      IL_0017:  ldarg.0
      IL_0018:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0' ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_1'::'CS$<>8__locals1'
      IL_001d:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0'::'<>4__this'
      IL_0022:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::fieldConflict
      IL_0027:  add
      IL_0028:  ldarg.0
      IL_0029:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0' ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_1'::'CS$<>8__locals1'
      IL_002e:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0'::fieldConflict
      IL_0033:  add
      IL_0034:  ret
    } // end of method '<>c__DisplayClass15_1'::'<Capturing>b__2'

  } // end of class '<>c__DisplayClass15_1'

  .field private int32 fieldConflict
  .field private int32 innerConflict
  .method private hidebysig instance void 
          NoParameters() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldftn      instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Parameter(object)
    IL_0008:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                      native int)
    IL_000d:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Delegate(class [mscorlib]System.Action`1<object>)
    IL_0012:  ldarg.0
    IL_0013:  ldnull
    IL_0014:  ldftn      void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::StaticParameter(object)
    IL_001a:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                      native int)
    IL_001f:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Delegate(class [mscorlib]System.Action`1<object>)
    IL_0024:  ret
  } // end of method QualifierTests::NoParameters

  .method private hidebysig static void  StaticNoParameteres() cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method QualifierTests::StaticNoParameteres

  .method private hidebysig instance void 
          Parameter(object o) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method QualifierTests::Parameter

  .method private hidebysig static void  StaticParameter(object o) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method QualifierTests::StaticParameter

  .method private hidebysig instance void 
          DeclaringType() cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method QualifierTests::DeclaringType

  .method private hidebysig static void  DeclaringTypeStatic() cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method QualifierTests::DeclaringTypeStatic

  .method private hidebysig instance void 
          conflictWithParameter() cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method QualifierTests::conflictWithParameter

  .method private hidebysig instance void 
          conflictWithVariable(int32 val) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method QualifierTests::conflictWithVariable

  .method private hidebysig instance void 
          Conflicts(int32 conflictWithParameter) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::conflictWithParameter()
    IL_0006:  ret
  } // end of method QualifierTests::Conflicts

  .method private hidebysig instance void 
          Conflicts() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass14_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass14_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.5
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass14_0'::conflictWithVariable
    IL_000d:  ldarg.0
    IL_000e:  ldloc.0
    IL_000f:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass14_0'::conflictWithVariable
    IL_0014:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::conflictWithVariable(int32)
    IL_0019:  ldarg.0
    IL_001a:  ldloc.0
    IL_001b:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass14_0'::'<Conflicts>b__0'()
    IL_0021:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0026:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Capturer(class [mscorlib]System.Func`1<int32>)
    IL_002b:  ret
  } // end of method QualifierTests::Conflicts

  .method private hidebysig instance void 
          Capturing() cil managed
  {
    // Code size       57 (0x39)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.5
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0'::fieldConflict
    IL_0014:  ldarg.0
    IL_0015:  ldloc.0
    IL_0016:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0'::'<Capturing>b__0'()
    IL_001c:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0021:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Capturer(class [mscorlib]System.Func`1<int32>)
    IL_0026:  ldarg.0
    IL_0027:  ldloc.0
    IL_0028:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass15_0'::'<Capturing>b__1'()
    IL_002e:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0033:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Capturer(class [mscorlib]System.Func`1<int32>)
    IL_0038:  ret
  } // end of method QualifierTests::Capturing

  .method private hidebysig instance void 
          Capturer(class [mscorlib]System.Func`1<int32> func) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method QualifierTests::Capturer

  .method private hidebysig instance int32 
          Capturer2(class [mscorlib]System.Func`1<int32> func) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  } // end of method QualifierTests::Capturer2

  .method private hidebysig instance void 
          Delegate(class [mscorlib]System.Action`1<object> action) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method QualifierTests::Delegate

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method QualifierTests::.ctor

} // end of class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests

.class private abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.Pretty.Ext
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig static void  Do(int32 test) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method Ext::Do

  .method public hidebysig static void  Do(object test) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method Ext::Do

  .method public hidebysig static void  Do2(int32 test) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       20 (0x14)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       void ICSharpCode.Decompiler.Tests.Pretty.Ext::Do(int32)
    IL_0006:  ldnull
    IL_0007:  call       bool [System.Core]System.Linq.Enumerable::Any<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  pop
    IL_000d:  ldnull
    IL_000e:  call       void ICSharpCode.Decompiler.Tests.Pretty.Ext::Do(object)
    IL_0013:  ret
  } // end of method Ext::Do2

} // end of class ICSharpCode.Decompiler.Tests.Pretty.Ext


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
