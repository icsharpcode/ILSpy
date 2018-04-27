



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
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
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
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::NoParameters()
      IL_0007:  nop
      IL_0008:  ret
    } // end of method Test::DeclaringType

    .method private hidebysig instance void 
            DeclaringType() cil managed
    {
      // Code size       30 (0x1e)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  call       void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::StaticNoParameteres()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldnull
      IL_0009:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::Parameter(class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests)
      IL_000e:  nop
      IL_000f:  ldnull
      IL_0010:  call       void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::StaticParameter(class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests)
      IL_0015:  nop
      IL_0016:  ldnull
      IL_0017:  call       void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::StaticParameter(object)
      IL_001c:  nop
      IL_001d:  ret
    } // end of method Test::DeclaringType

    .method private hidebysig instance void 
            Parameter(object o) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method Test::Parameter

    .method private hidebysig static void 
            StaticParameter(object o) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method Test::StaticParameter

    .method private hidebysig instance void 
            Parameter(class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests test) cil managed
    {
      // Code size       88 (0x58)
      .maxstack  3
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.0
      IL_0003:  ldobj      ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test
      IL_0008:  box        ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test
      IL_000d:  ldftn      instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::Parameter(object)
      IL_0013:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                        native int)
      IL_0018:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::Delegate(class [mscorlib]System.Action`1<object>)
      IL_001d:  nop
      IL_001e:  ldarg.0
      IL_001f:  ldnull
      IL_0020:  ldftn      void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::StaticParameter(object)
      IL_0026:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                        native int)
      IL_002b:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::Delegate(class [mscorlib]System.Action`1<object>)
      IL_0030:  nop
      IL_0031:  ldarg.0
      IL_0032:  ldarg.1
      IL_0033:  ldftn      instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Parameter(object)
      IL_0039:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                        native int)
      IL_003e:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::Delegate(class [mscorlib]System.Action`1<object>)
      IL_0043:  nop
      IL_0044:  ldarg.0
      IL_0045:  ldnull
      IL_0046:  ldftn      void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::StaticParameter(object)
      IL_004c:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                        native int)
      IL_0051:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::Delegate(class [mscorlib]System.Action`1<object>)
      IL_0056:  nop
      IL_0057:  ret
    } // end of method Test::Parameter

    .method private hidebysig static void 
            StaticParameter(class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests test) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method Test::StaticParameter

    .method private hidebysig static void 
            DeclaringTypeStatic() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method Test::DeclaringTypeStatic

    .method private hidebysig instance void 
            DeclaringTypeConflict(class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests 'instance') cil managed
    {
      // Code size       30 (0x1e)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::DeclaringType()
      IL_0007:  nop
      IL_0008:  ldarg.1
      IL_0009:  callvirt   instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::DeclaringType()
      IL_000e:  nop
      IL_000f:  ldarg.0
      IL_0010:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::fieldConflict()
      IL_0015:  nop
      IL_0016:  ldarg.1
      IL_0017:  ldc.i4.5
      IL_0018:  stfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::fieldConflict
      IL_001d:  ret
    } // end of method Test::DeclaringTypeConflict

    .method private hidebysig instance void 
            DeclaringTypeConflict() cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  call       void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Test::DeclaringTypeStatic()
      IL_0006:  nop
      IL_0007:  call       void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::DeclaringTypeStatic()
      IL_000c:  nop
      IL_000d:  ret
    } // end of method Test::DeclaringTypeConflict

    .method private hidebysig instance void 
            fieldConflict() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method Test::fieldConflict

    .method private hidebysig instance void 
            Delegate(class [mscorlib]System.Action`1<object> action) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method Test::Delegate

  } // end of class Test

  .class auto ansi nested assembly beforefieldinit Parent
         extends [mscorlib]System.Object
  {
    .method public hidebysig newslot virtual 
            instance void  Virtual() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method Parent::Virtual

    .method public hidebysig newslot virtual 
            instance void  NewVirtual() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method Parent::NewVirtual

    .method public hidebysig instance void 
            New() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method Parent::New

    .method public hidebysig instance void 
            BaseOnly() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
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
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::Virtual()
      IL_0007:  nop
      IL_0008:  ret
    } // end of method Child::Virtual

    .method public hidebysig instance void 
            NewVirtual() cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::NewVirtual()
      IL_0007:  nop
      IL_0008:  ret
    } // end of method Child::NewVirtual

    .method public hidebysig instance void 
            New() cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::New()
      IL_0007:  nop
      IL_0008:  ret
    } // end of method Child::New

    .method public hidebysig instance void 
            BaseQualifiers() cil managed
    {
      // Code size       51 (0x33)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  callvirt   instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::Virtual()
      IL_0007:  nop
      IL_0008:  ldarg.0
      IL_0009:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::Virtual()
      IL_000e:  nop
      IL_000f:  ldarg.0
      IL_0010:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Child::NewVirtual()
      IL_0015:  nop
      IL_0016:  ldarg.0
      IL_0017:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::NewVirtual()
      IL_001c:  nop
      IL_001d:  ldarg.0
      IL_001e:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Child::New()
      IL_0023:  nop
      IL_0024:  ldarg.0
      IL_0025:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::New()
      IL_002a:  nop
      IL_002b:  ldarg.0
      IL_002c:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/Parent::BaseOnly()
      IL_0031:  nop
      IL_0032:  ret
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

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass1'
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
    } // end of method '<>c__DisplayClass1'::.ctor

    .method public hidebysig instance int32 
            '<Conflicts>b__0'() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass1'::conflictWithVariable
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<>c__DisplayClass1'::'<Conflicts>b__0'

  } // end of class '<>c__DisplayClass1'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass6'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass8'
           extends [mscorlib]System.Object
    {
      .field public class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6' 'CS$<>8__locals7'
      .field public int32 innerConflict
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass8'::.ctor

      .method public hidebysig instance int32 
              '<Capturing>b__5'() cil managed
      {
        // Code size       57 (0x39)
        .maxstack  2
        .locals init (int32 V_0)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6' ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'/'<>c__DisplayClass8'::'CS$<>8__locals7'
        IL_0006:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'::'<>4__this'
        IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::innerConflict
        IL_0010:  ldarg.0
        IL_0011:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'/'<>c__DisplayClass8'::innerConflict
        IL_0016:  add
        IL_0017:  ldarg.0
        IL_0018:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6' ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'/'<>c__DisplayClass8'::'CS$<>8__locals7'
        IL_001d:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'::'<>4__this'
        IL_0022:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::fieldConflict
        IL_0027:  add
        IL_0028:  ldarg.0
        IL_0029:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6' ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'/'<>c__DisplayClass8'::'CS$<>8__locals7'
        IL_002e:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'::fieldConflict
        IL_0033:  add
        IL_0034:  stloc.0
        IL_0035:  br.s       IL_0037

        IL_0037:  ldloc.0
        IL_0038:  ret
      } // end of method '<>c__DisplayClass8'::'<Capturing>b__5'

    } // end of class '<>c__DisplayClass8'

    .field public int32 fieldConflict
    .field public class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests '<>4__this'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass6'::.ctor

    .method public hidebysig instance int32 
            '<Capturing>b__3'() cil managed
    {
      // Code size       23 (0x17)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'::'<>4__this'
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::fieldConflict
      IL_000b:  ldarg.0
      IL_000c:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'::fieldConflict
      IL_0011:  add
      IL_0012:  stloc.0
      IL_0013:  br.s       IL_0015

      IL_0015:  ldloc.0
      IL_0016:  ret
    } // end of method '<>c__DisplayClass6'::'<Capturing>b__3'

    .method public hidebysig instance int32 
            '<Capturing>b__4'() cil managed
    {
      // Code size       68 (0x44)
      .maxstack  4
      .locals init (class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'/'<>c__DisplayClass8' V_0,
               int32 V_1)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'/'<>c__DisplayClass8'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6' ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'/'<>c__DisplayClass8'::'CS$<>8__locals7'
      IL_000d:  nop
      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.5
      IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'/'<>c__DisplayClass8'::innerConflict
      IL_0015:  ldarg.0
      IL_0016:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'::'<>4__this'
      IL_001b:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::fieldConflict
      IL_0020:  ldarg.0
      IL_0021:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'::fieldConflict
      IL_0026:  add
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'::'<>4__this'
      IL_002d:  ldloc.0
      IL_002e:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'/'<>c__DisplayClass8'::'<Capturing>b__5'()
      IL_0034:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                     native int)
      IL_0039:  call       instance int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Capturer2(class [mscorlib]System.Func`1<int32>)
      IL_003e:  add
      IL_003f:  stloc.1
      IL_0040:  br.s       IL_0042

      IL_0042:  ldloc.1
      IL_0043:  ret
    } // end of method '<>c__DisplayClass6'::'<Capturing>b__4'

  } // end of class '<>c__DisplayClass6'

  .field private int32 fieldConflict
  .field private int32 innerConflict
  .method private hidebysig instance void 
          NoParameters() cil managed
  {
    // Code size       40 (0x28)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.0
    IL_0003:  ldftn      instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Parameter(object)
    IL_0009:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                      native int)
    IL_000e:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Delegate(class [mscorlib]System.Action`1<object>)
    IL_0013:  nop
    IL_0014:  ldarg.0
    IL_0015:  ldnull
    IL_0016:  ldftn      void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::StaticParameter(object)
    IL_001c:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                      native int)
    IL_0021:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Delegate(class [mscorlib]System.Action`1<object>)
    IL_0026:  nop
    IL_0027:  ret
  } // end of method QualifierTests::NoParameters

  .method private hidebysig static void  StaticNoParameteres() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method QualifierTests::StaticNoParameteres

  .method private hidebysig instance void 
          Parameter(object o) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method QualifierTests::Parameter

  .method private hidebysig static void  StaticParameter(object o) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method QualifierTests::StaticParameter

  .method private hidebysig instance void 
          DeclaringType() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method QualifierTests::DeclaringType

  .method private hidebysig static void  DeclaringTypeStatic() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method QualifierTests::DeclaringTypeStatic

  .method private hidebysig instance void 
          conflictWithParameter() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method QualifierTests::conflictWithParameter

  .method private hidebysig instance void 
          conflictWithVariable(int32 val) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method QualifierTests::conflictWithVariable

  .method private hidebysig instance void 
          Conflicts(int32 conflictWithParameter) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::conflictWithParameter()
    IL_0007:  nop
    IL_0008:  ret
  } // end of method QualifierTests::Conflicts

  .method private hidebysig instance void 
          Conflicts() cil managed
  {
    // Code size       48 (0x30)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass1' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass1'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.5
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass1'::conflictWithVariable
    IL_000e:  ldarg.0
    IL_000f:  ldloc.0
    IL_0010:  ldfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass1'::conflictWithVariable
    IL_0015:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::conflictWithVariable(int32)
    IL_001a:  nop
    IL_001b:  ldarg.0
    IL_001c:  ldloc.0
    IL_001d:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass1'::'<Conflicts>b__0'()
    IL_0023:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0028:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Capturer(class [mscorlib]System.Func`1<int32>)
    IL_002d:  nop
    IL_002e:  nop
    IL_002f:  ret
  } // end of method QualifierTests::Conflicts

  .method private hidebysig instance void 
          Capturing() cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.Pretty.QualifierTests ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'::'<>4__this'
    IL_000d:  nop
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.5
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'::fieldConflict
    IL_0015:  ldarg.0
    IL_0016:  ldloc.0
    IL_0017:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'::'<Capturing>b__3'()
    IL_001d:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Capturer(class [mscorlib]System.Func`1<int32>)
    IL_0027:  nop
    IL_0028:  ldarg.0
    IL_0029:  ldloc.0
    IL_002a:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.Pretty.QualifierTests/'<>c__DisplayClass6'::'<Capturing>b__4'()
    IL_0030:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0035:  call       instance void ICSharpCode.Decompiler.Tests.Pretty.QualifierTests::Capturer(class [mscorlib]System.Func`1<int32>)
    IL_003a:  nop
    IL_003b:  nop
    IL_003c:  ret
  } // end of method QualifierTests::Capturing

  .method private hidebysig instance void 
          Capturer(class [mscorlib]System.Func`1<int32> func) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method QualifierTests::Capturer

  .method private hidebysig instance int32 
          Capturer2(class [mscorlib]System.Func`1<int32> func) cil managed
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
  } // end of method QualifierTests::Capturer2

  .method private hidebysig instance void 
          Delegate(class [mscorlib]System.Action`1<object> action) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
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
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method Ext::Do

  .method public hidebysig static void  Do(object test) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method Ext::Do

  .method public hidebysig static void  Do2(int32 test) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       23 (0x17)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       void ICSharpCode.Decompiler.Tests.Pretty.Ext::Do(int32)
    IL_0007:  nop
    IL_0008:  ldnull
    IL_0009:  call       bool [System.Core]System.Linq.Enumerable::Any<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000e:  pop
    IL_000f:  ldnull
    IL_0010:  call       void ICSharpCode.Decompiler.Tests.Pretty.Ext::Do(object)
    IL_0015:  nop
    IL_0016:  ret
  } // end of method Ext::Do2

} // end of class ICSharpCode.Decompiler.Tests.Pretty.Ext


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
