



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly TypeMemberTests
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module TypeMemberTests.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.IndexerWithGetOnly
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance int32 
          get_Item(int32 i) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method IndexerWithGetOnly::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method IndexerWithGetOnly::.ctor

  .property instance int32 Item(int32)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.IndexerWithGetOnly::get_Item(int32)
  } // end of property IndexerWithGetOnly::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IndexerWithGetOnly

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.IndexerWithSetOnly
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance void 
          set_Item(int32 i,
                   int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method IndexerWithSetOnly::set_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method IndexerWithSetOnly::.ctor

  .property instance int32 Item(int32)
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.IndexerWithSetOnly::set_Item(int32,
                                                                                                  int32)
  } // end of property IndexerWithSetOnly::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IndexerWithSetOnly

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.IndexerWithMoreParameters
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance int32 
          get_Item(int32 i,
                   string s,
                   class [mscorlib]System.Type t) cil managed
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
  } // end of method IndexerWithMoreParameters::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method IndexerWithMoreParameters::.ctor

  .property instance int32 Item(int32,
                                string,
                                class [mscorlib]System.Type)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.IndexerWithMoreParameters::get_Item(int32,
                                                                                                          string,
                                                                                                          class [mscorlib]System.Type)
  } // end of property IndexerWithMoreParameters::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IndexerWithMoreParameters

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.IndexerInGenericClass`1<T>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance int32 
          get_Item(!T t) cil managed
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
  } // end of method IndexerInGenericClass`1::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method IndexerInGenericClass`1::.ctor

  .property instance int32 Item(!T)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.IndexerInGenericClass`1::get_Item(!T)
  } // end of property IndexerInGenericClass`1::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IndexerInGenericClass`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.OverloadedIndexer
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance int32 
          get_Item(int32 t) cil managed
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
  } // end of method OverloadedIndexer::get_Item

  .method public hidebysig specialname instance int32 
          get_Item(string s) cil managed
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
  } // end of method OverloadedIndexer::get_Item

  .method public hidebysig specialname instance void 
          set_Item(string s,
                   int32 'value') cil managed
  {
    // Code size       25 (0x19)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  box        [mscorlib]System.Int32
    IL_0007:  ldstr      " "
    IL_000c:  ldarg.1
    IL_000d:  call       string [mscorlib]System.String::Concat(object,
                                                                object,
                                                                object)
    IL_0012:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0017:  nop
    IL_0018:  ret
  } // end of method OverloadedIndexer::set_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method OverloadedIndexer::.ctor

  .property instance int32 Item(int32)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.OverloadedIndexer::get_Item(int32)
  } // end of property OverloadedIndexer::Item
  .property instance int32 Item(string)
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.OverloadedIndexer::set_Item(string,
                                                                                                 int32)
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.OverloadedIndexer::get_Item(string)
  } // end of property OverloadedIndexer::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.OverloadedIndexer

.class interface public abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.IIndexerInInterface
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig newslot specialname abstract virtual 
          instance void  set_Item(string s,
                                  string s2,
                                  int32 'value') cil managed
  {
  } // end of method IIndexerInInterface::set_Item

  .property instance int32 Item(string,
                                string)
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.IIndexerInInterface::set_Item(string,
                                                                                                   string,
                                                                                                   int32)
  } // end of property IIndexerInInterface::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IIndexerInInterface

.class interface public abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_IndexerInterfaceExplicitImplementation
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig newslot specialname abstract virtual 
          instance int32  get_Item(string s) cil managed
  {
  } // end of method IMyInterface_IndexerInterfaceExplicitImplementation::get_Item

  .property instance int32 Item(string)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_IndexerInterfaceExplicitImplementation::get_Item(string)
  } // end of property IMyInterface_IndexerInterfaceExplicitImplementation::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_IndexerInterfaceExplicitImplementation

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerInterfaceExplicitImplementation
       extends [mscorlib]System.Object
       implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_IndexerInterfaceExplicitImplementation
{
  .method private hidebysig newslot specialname virtual final 
          instance int32  ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_IndexerInterfaceExplicitImplementation.get_Item(string s) cil managed
  {
    .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_IndexerInterfaceExplicitImplementation::get_Item
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method MyClass_IndexerInterfaceExplicitImplementation::ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_IndexerInterfaceExplicitImplementation.get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass_IndexerInterfaceExplicitImplementation::.ctor

  .property instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_IndexerInterfaceExplicitImplementation.Item(string)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerInterfaceExplicitImplementation::ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_IndexerInterfaceExplicitImplementation.get_Item(string)
  } // end of property MyClass_IndexerInterfaceExplicitImplementation::ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_IndexerInterfaceExplicitImplementation.Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerInterfaceExplicitImplementation

.class interface public abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_IndexerInterfaceImplementation
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig newslot specialname abstract virtual 
          instance int32  get_Item(string s) cil managed
  {
  } // end of method IMyInterface_IndexerInterfaceImplementation::get_Item

  .property instance int32 Item(string)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_IndexerInterfaceImplementation::get_Item(string)
  } // end of property IMyInterface_IndexerInterfaceImplementation::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_IndexerInterfaceImplementation

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerInterfaceImplementation
       extends [mscorlib]System.Object
       implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_IndexerInterfaceImplementation
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig newslot specialname virtual final 
          instance int32  get_Item(string s) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method MyClass_IndexerInterfaceImplementation::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass_IndexerInterfaceImplementation::.ctor

  .property instance int32 Item(string)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerInterfaceImplementation::get_Item(string)
  } // end of property MyClass_IndexerInterfaceImplementation::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerInterfaceImplementation

.class public abstract auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerAbstract
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig newslot specialname abstract virtual 
          instance void  set_Item(string s,
                                  string s2,
                                  int32 'value') cil managed
  {
  } // end of method MyClass_IndexerAbstract::set_Item

  .method family hidebysig newslot specialname abstract virtual 
          instance string  get_Item(int32 index) cil managed
  {
  } // end of method MyClass_IndexerAbstract::get_Item

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass_IndexerAbstract::.ctor

  .property instance int32 Item(string,
                                string)
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerAbstract::set_Item(string,
                                                                                                       string,
                                                                                                       int32)
  } // end of property MyClass_IndexerAbstract::Item
  .property instance string Item(int32)
  {
    .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerAbstract::get_Item(int32)
  } // end of property MyClass_IndexerAbstract::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerAbstract

.class interface public abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodExplicit
{
  .method public hidebysig newslot abstract virtual 
          instance void  MyMethod() cil managed
  {
  } // end of method IMyInterface_MethodExplicit::MyMethod

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodExplicit

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_MethodExplicit
       extends [mscorlib]System.Object
       implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodExplicit
{
  .method private hidebysig newslot virtual final 
          instance void  ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodExplicit.MyMethod() cil managed
  {
    .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodExplicit::MyMethod
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method MyClass_MethodExplicit::ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodExplicit.MyMethod

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass_MethodExplicit::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_MethodExplicit

.class interface public abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodFromInterfaceVirtual
{
  .method public hidebysig newslot abstract virtual 
          instance void  MyMethod() cil managed
  {
  } // end of method IMyInterface_MethodFromInterfaceVirtual::MyMethod

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodFromInterfaceVirtual

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
       extends [mscorlib]System.Object
       implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodFromInterfaceVirtual
{
  .method public hidebysig newslot virtual 
          instance void  MyMethod() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method MyClass::MyMethod

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

.class interface public abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodFromInterface
{
  .method public hidebysig newslot abstract virtual 
          instance void  MyMethod() cil managed
  {
  } // end of method IMyInterface_MethodFromInterface::MyMethod

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodFromInterface

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_MethodFromInterface
       extends [mscorlib]System.Object
       implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodFromInterface
{
  .method public hidebysig newslot virtual final 
          instance void  MyMethod() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method MyClass_MethodFromInterface::MyMethod

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass_MethodFromInterface::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_MethodFromInterface

.class interface public abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodFromInterfaceAbstract
{
  .method public hidebysig newslot abstract virtual 
          instance void  MyMethod() cil managed
  {
  } // end of method IMyInterface_MethodFromInterfaceAbstract::MyMethod

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodFromInterfaceAbstract

.class public abstract auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_MethodFromInterfaceAbstract
       extends [mscorlib]System.Object
       implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_MethodFromInterfaceAbstract
{
  .method public hidebysig newslot abstract virtual 
          instance void  MyMethod() cil managed
  {
  } // end of method MyClass_MethodFromInterfaceAbstract::MyMethod

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass_MethodFromInterfaceAbstract::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_MethodFromInterfaceAbstract

.class interface public abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterface
{
  .method public hidebysig newslot specialname abstract virtual 
          instance int32  get_MyProperty() cil managed
  {
  } // end of method IMyInterface_PropertyInterface::get_MyProperty

  .method public hidebysig newslot specialname abstract virtual 
          instance void  set_MyProperty(int32 'value') cil managed
  {
  } // end of method IMyInterface_PropertyInterface::set_MyProperty

  .property instance int32 MyProperty()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterface::get_MyProperty()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterface::set_MyProperty(int32)
  } // end of property IMyInterface_PropertyInterface::MyProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterface

.class interface public abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation
{
  .method public hidebysig newslot specialname abstract virtual 
          instance int32  get_MyProperty() cil managed
  {
  } // end of method IMyInterface_PropertyInterfaceExplicitImplementation::get_MyProperty

  .method public hidebysig newslot specialname abstract virtual 
          instance void  set_MyProperty(int32 'value') cil managed
  {
  } // end of method IMyInterface_PropertyInterfaceExplicitImplementation::set_MyProperty

  .property instance int32 MyProperty()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation::get_MyProperty()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation::set_MyProperty(int32)
  } // end of property IMyInterface_PropertyInterfaceExplicitImplementation::MyProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyInterfaceExplicitImplementation
       extends [mscorlib]System.Object
       implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation
{
  .method private hidebysig newslot specialname virtual final 
          instance int32  ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation.get_MyProperty() cil managed
  {
    .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation::get_MyProperty
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method MyClass_PropertyInterfaceExplicitImplementation::ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation.get_MyProperty

  .method private hidebysig newslot specialname virtual final 
          instance void  ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation.set_MyProperty(int32 'value') cil managed
  {
    .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation::set_MyProperty
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method MyClass_PropertyInterfaceExplicitImplementation::ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation.set_MyProperty

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass_PropertyInterfaceExplicitImplementation::.ctor

  .property instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation.MyProperty()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyInterfaceExplicitImplementation::ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation.get_MyProperty()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyInterfaceExplicitImplementation::ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation.set_MyProperty(int32)
  } // end of property MyClass_PropertyInterfaceExplicitImplementation::ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceExplicitImplementation.MyProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyInterfaceExplicitImplementation

.class interface public abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceImplementation
{
  .method public hidebysig newslot specialname abstract virtual 
          instance int32  get_MyProperty() cil managed
  {
  } // end of method IMyInterface_PropertyInterfaceImplementation::get_MyProperty

  .method public hidebysig newslot specialname abstract virtual 
          instance void  set_MyProperty(int32 'value') cil managed
  {
  } // end of method IMyInterface_PropertyInterfaceImplementation::set_MyProperty

  .property instance int32 MyProperty()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceImplementation::get_MyProperty()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceImplementation::set_MyProperty(int32)
  } // end of property IMyInterface_PropertyInterfaceImplementation::MyProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceImplementation

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyInterfaceImplementation
       extends [mscorlib]System.Object
       implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.IMyInterface_PropertyInterfaceImplementation
{
  .method public hidebysig newslot specialname virtual final 
          instance int32  get_MyProperty() cil managed
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
  } // end of method MyClass_PropertyInterfaceImplementation::get_MyProperty

  .method public hidebysig newslot specialname virtual final 
          instance void  set_MyProperty(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method MyClass_PropertyInterfaceImplementation::set_MyProperty

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass_PropertyInterfaceImplementation::.ctor

  .property instance int32 MyProperty()
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyInterfaceImplementation::set_MyProperty(int32)
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyInterfaceImplementation::get_MyProperty()
  } // end of property MyClass_PropertyInterfaceImplementation::MyProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyInterfaceImplementation

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyPrivateGetPublicSet
       extends [mscorlib]System.Object
{
  .method private hidebysig specialname instance int32 
          get_MyProperty() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method MyClass_PropertyPrivateGetPublicSet::get_MyProperty

  .method public hidebysig specialname instance void 
          set_MyProperty(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method MyClass_PropertyPrivateGetPublicSet::set_MyProperty

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass_PropertyPrivateGetPublicSet::.ctor

  .property instance int32 MyProperty()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyPrivateGetPublicSet::get_MyProperty()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyPrivateGetPublicSet::set_MyProperty(int32)
  } // end of property MyClass_PropertyPrivateGetPublicSet::MyProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyPrivateGetPublicSet

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyPublicGetProtectedSet
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname instance int32 
          get_MyProperty() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method MyClass_PropertyPublicGetProtectedSet::get_MyProperty

  .method family hidebysig specialname instance void 
          set_MyProperty(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method MyClass_PropertyPublicGetProtectedSet::set_MyProperty

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass_PropertyPublicGetProtectedSet::.ctor

  .property instance int32 MyProperty()
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyPublicGetProtectedSet::set_MyProperty(int32)
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyPublicGetProtectedSet::get_MyProperty()
  } // end of property MyClass_PropertyPublicGetProtectedSet::MyProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyPublicGetProtectedSet

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideDefaultAccessorOnly
       extends [mscorlib]System.Object
{
  .method public hidebysig newslot specialname virtual 
          instance int32  get_MyProperty() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method MyClass_PropertyOverrideDefaultAccessorOnly::get_MyProperty

  .method family hidebysig newslot specialname virtual 
          instance void  set_MyProperty(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method MyClass_PropertyOverrideDefaultAccessorOnly::set_MyProperty

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass_PropertyOverrideDefaultAccessorOnly::.ctor

  .property instance int32 MyProperty()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideDefaultAccessorOnly::get_MyProperty()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideDefaultAccessorOnly::set_MyProperty(int32)
  } // end of property MyClass_PropertyOverrideDefaultAccessorOnly::MyProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideDefaultAccessorOnly

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Derived_PropertyOverrideDefaultAccessorOnly
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideDefaultAccessorOnly
{
  .method public hidebysig specialname virtual 
          instance int32  get_MyProperty() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.4
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method Derived_PropertyOverrideDefaultAccessorOnly::get_MyProperty

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideDefaultAccessorOnly::.ctor()
    IL_0006:  ret
  } // end of method Derived_PropertyOverrideDefaultAccessorOnly::.ctor

  .property instance int32 MyProperty()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Derived_PropertyOverrideDefaultAccessorOnly::get_MyProperty()
  } // end of property Derived_PropertyOverrideDefaultAccessorOnly::MyProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Derived_PropertyOverrideDefaultAccessorOnly

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideRestrictedAccessorOnly
       extends [mscorlib]System.Object
{
  .method public hidebysig newslot specialname virtual 
          instance int32  get_MyProperty() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method MyClass_PropertyOverrideRestrictedAccessorOnly::get_MyProperty

  .method family hidebysig newslot specialname virtual 
          instance void  set_MyProperty(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method MyClass_PropertyOverrideRestrictedAccessorOnly::set_MyProperty

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass_PropertyOverrideRestrictedAccessorOnly::.ctor

  .property instance int32 MyProperty()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideRestrictedAccessorOnly::get_MyProperty()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideRestrictedAccessorOnly::set_MyProperty(int32)
  } // end of property MyClass_PropertyOverrideRestrictedAccessorOnly::MyProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideRestrictedAccessorOnly

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Derived_PropertyOverrideRestrictedAccessorOnly
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideRestrictedAccessorOnly
{
  .method family hidebysig specialname virtual 
          instance void  set_MyProperty(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method Derived_PropertyOverrideRestrictedAccessorOnly::set_MyProperty

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideRestrictedAccessorOnly::.ctor()
    IL_0006:  ret
  } // end of method Derived_PropertyOverrideRestrictedAccessorOnly::.ctor

  .property instance int32 MyProperty()
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Derived_PropertyOverrideRestrictedAccessorOnly::set_MyProperty(int32)
  } // end of property Derived_PropertyOverrideRestrictedAccessorOnly::MyProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Derived_PropertyOverrideRestrictedAccessorOnly

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideOneAccessor
       extends [mscorlib]System.Object
{
  .method famorassem hidebysig newslot specialname virtual 
          instance int32  get_MyProperty() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method MyClass_PropertyOverrideOneAccessor::get_MyProperty

  .method family hidebysig newslot specialname virtual 
          instance void  set_MyProperty(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method MyClass_PropertyOverrideOneAccessor::set_MyProperty

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass_PropertyOverrideOneAccessor::.ctor

  .property instance int32 MyProperty()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideOneAccessor::get_MyProperty()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideOneAccessor::set_MyProperty(int32)
  } // end of property MyClass_PropertyOverrideOneAccessor::MyProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideOneAccessor

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.DerivedNew_PropertyOverrideOneAccessor
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideOneAccessor
{
  .method public hidebysig newslot specialname virtual 
          instance void  set_MyProperty(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method DerivedNew_PropertyOverrideOneAccessor::set_MyProperty

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_PropertyOverrideOneAccessor::.ctor()
    IL_0006:  ret
  } // end of method DerivedNew_PropertyOverrideOneAccessor::.ctor

  .property instance int32 MyProperty()
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DerivedNew_PropertyOverrideOneAccessor::set_MyProperty(int32)
  } // end of property DerivedNew_PropertyOverrideOneAccessor::MyProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DerivedNew_PropertyOverrideOneAccessor

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.DerivedOverride_PropertyOverrideOneAccessor
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.DerivedNew_PropertyOverrideOneAccessor
{
  .method public hidebysig specialname virtual 
          instance void  set_MyProperty(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method DerivedOverride_PropertyOverrideOneAccessor::set_MyProperty

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DerivedNew_PropertyOverrideOneAccessor::.ctor()
    IL_0006:  ret
  } // end of method DerivedOverride_PropertyOverrideOneAccessor::.ctor

  .property instance int32 MyProperty()
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DerivedOverride_PropertyOverrideOneAccessor::set_MyProperty(int32)
  } // end of property DerivedOverride_PropertyOverrideOneAccessor::MyProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DerivedOverride_PropertyOverrideOneAccessor

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerOverrideRestrictedAccessorOnly
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig newslot specialname virtual 
          instance int32  get_Item(string s) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method MyClass_IndexerOverrideRestrictedAccessorOnly::get_Item

  .method family hidebysig newslot specialname virtual 
          instance void  set_Item(string s,
                                  int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method MyClass_IndexerOverrideRestrictedAccessorOnly::set_Item

  .method family hidebysig newslot specialname virtual 
          instance int32  get_Item(int32 i) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method MyClass_IndexerOverrideRestrictedAccessorOnly::get_Item

  .method famorassem hidebysig newslot specialname virtual 
          instance void  set_Item(int32 i,
                                  int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method MyClass_IndexerOverrideRestrictedAccessorOnly::set_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass_IndexerOverrideRestrictedAccessorOnly::.ctor

  .property instance int32 Item(string)
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerOverrideRestrictedAccessorOnly::set_Item(string,
                                                                                                                             int32)
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerOverrideRestrictedAccessorOnly::get_Item(string)
  } // end of property MyClass_IndexerOverrideRestrictedAccessorOnly::Item
  .property instance int32 Item(int32)
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerOverrideRestrictedAccessorOnly::set_Item(int32,
                                                                                                                             int32)
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerOverrideRestrictedAccessorOnly::get_Item(int32)
  } // end of property MyClass_IndexerOverrideRestrictedAccessorOnly::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerOverrideRestrictedAccessorOnly

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Derived_IndexerOverrideRestrictedAccessorOnly
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerOverrideRestrictedAccessorOnly
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method family hidebysig specialname virtual 
          instance int32  get_Item(int32 i) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.4
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method Derived_IndexerOverrideRestrictedAccessorOnly::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass_IndexerOverrideRestrictedAccessorOnly::.ctor()
    IL_0006:  ret
  } // end of method Derived_IndexerOverrideRestrictedAccessorOnly::.ctor

  .property instance int32 Item(int32)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Derived_IndexerOverrideRestrictedAccessorOnly::get_Item(int32)
  } // end of property Derived_IndexerOverrideRestrictedAccessorOnly::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Derived_IndexerOverrideRestrictedAccessorOnly

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideProperty
       extends [mscorlib]System.Object
{
  .method public hidebysig newslot specialname virtual 
          instance int32  get_P() cil managed
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
  } // end of method A_HideProperty::get_P

  .method public hidebysig newslot specialname virtual 
          instance void  set_P(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method A_HideProperty::set_P

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideProperty::.ctor

  .property instance int32 P()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideProperty::get_P()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideProperty::set_P(int32)
  } // end of property A_HideProperty::P
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideProperty

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideProperty
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideProperty
{
  .method private hidebysig specialname instance int32 
          get_P() cil managed
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
  } // end of method B_HideProperty::get_P

  .method private hidebysig specialname instance void 
          set_P(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HideProperty::set_P

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideProperty::.ctor()
    IL_0006:  ret
  } // end of method B_HideProperty::.ctor

  .property instance int32 P()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideProperty::get_P()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideProperty::set_P(int32)
  } // end of property B_HideProperty::P
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideProperty

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideProperty
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideProperty
{
  .method public hidebysig specialname virtual 
          instance void  set_P(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method C_HideProperty::set_P

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideProperty::.ctor()
    IL_0006:  ret
  } // end of method C_HideProperty::.ctor

  .property instance int32 P()
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideProperty::set_P(int32)
  } // end of property C_HideProperty::P
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideProperty

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers
       extends [mscorlib]System.Object
{
  .field public int32 F
  .method public hidebysig specialname instance int32 
          get_Prop() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method A_HideMembers::get_Prop

  .method public hidebysig specialname instance int32 
          get_G() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method A_HideMembers::get_G

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideMembers::.ctor

  .property instance int32 Prop()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers::get_Prop()
  } // end of property A_HideMembers::Prop
  .property instance int32 G()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers::get_G()
  } // end of property A_HideMembers::G
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMembers
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers
{
  .method public hidebysig specialname instance int32 
          get_F() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method B_HideMembers::get_F

  .method public hidebysig specialname instance string 
          get_Prop() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "a"
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method B_HideMembers::get_Prop

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers::.ctor()
    IL_0006:  ret
  } // end of method B_HideMembers::.ctor

  .property instance int32 F()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMembers::get_F()
  } // end of property B_HideMembers::F
  .property instance string Prop()
  {
    .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMembers::get_Prop()
  } // end of property B_HideMembers::Prop
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMembers

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMembers
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers
{
  .field public int32 G
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers::.ctor()
    IL_0006:  ret
  } // end of method C_HideMembers::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMembers

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.D_HideMembers
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers
{
  .method public hidebysig instance void 
          F() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method D_HideMembers::F

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers::.ctor()
    IL_0006:  ret
  } // end of method D_HideMembers::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.D_HideMembers

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.D1_HideMembers
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.D_HideMembers
{
  .field public int32 F
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.D_HideMembers::.ctor()
    IL_0006:  ret
  } // end of method D1_HideMembers::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.D1_HideMembers

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.E_HideMembers
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers
{
  .class auto ansi nested private beforefieldinit F
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
    } // end of method F::.ctor

  } // end of class F

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers::.ctor()
    IL_0006:  ret
  } // end of method E_HideMembers::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.E_HideMembers

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.G_HideMembers2
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname instance int32 
          get_Item() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method G_HideMembers2::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method G_HideMembers2::.ctor

  .property instance int32 Item()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.G_HideMembers2::get_Item()
  } // end of property G_HideMembers2::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.G_HideMembers2

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.G2_HideMembers2
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.G_HideMembers2
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance int32 
          get_Item(int32 i) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method G2_HideMembers2::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.G_HideMembers2::.ctor()
    IL_0006:  ret
  } // end of method G2_HideMembers2::.ctor

  .property instance int32 Item(int32)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.G2_HideMembers2::get_Item(int32)
  } // end of property G2_HideMembers2::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.G2_HideMembers2

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.G3_HideMembers2
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.G2_HideMembers2
{
  .method public hidebysig specialname instance int32 
          get_Item() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.4
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method G3_HideMembers2::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.G2_HideMembers2::.ctor()
    IL_0006:  ret
  } // end of method G3_HideMembers2::.ctor

  .property instance int32 Item()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.G3_HideMembers2::get_Item()
  } // end of property G3_HideMembers2::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.G3_HideMembers2

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.H_HideMembers2
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance int32 
          get_Item(int32 j) cil managed
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
  } // end of method H_HideMembers2::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method H_HideMembers2::.ctor

  .property instance int32 Item(int32)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.H_HideMembers2::get_Item(int32)
  } // end of property H_HideMembers2::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.H_HideMembers2

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.H2_HideMembers2
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.H_HideMembers2
{
  .method public hidebysig specialname instance int32 
          get_Item() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method H2_HideMembers2::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.H_HideMembers2::.ctor()
    IL_0006:  ret
  } // end of method H2_HideMembers2::.ctor

  .property instance int32 Item()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.H2_HideMembers2::get_Item()
  } // end of property H2_HideMembers2::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.H2_HideMembers2

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.H3_HideMembers2
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.H2_HideMembers2
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance string 
          get_Item(int32 j) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method H3_HideMembers2::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.H2_HideMembers2::.ctor()
    IL_0006:  ret
  } // end of method H3_HideMembers2::.ctor

  .property instance string Item(int32)
  {
    .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.H3_HideMembers2::get_Item(int32)
  } // end of property H3_HideMembers2::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.H3_HideMembers2

.class interface public abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.IA_HideMembers2a
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig newslot specialname abstract virtual 
          instance int32  get_Item(int32 i) cil managed
  {
  } // end of method IA_HideMembers2a::get_Item

  .property instance int32 Item(int32)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.IA_HideMembers2a::get_Item(int32)
  } // end of property IA_HideMembers2a::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.IA_HideMembers2a

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers2a
       extends [mscorlib]System.Object
       implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.IA_HideMembers2a
{
  .method private hidebysig newslot specialname virtual final 
          instance int32  ICSharpCode.Decompiler.Tests.TestCases.Pretty.IA_HideMembers2a.get_Item(int32 i) cil managed
  {
    .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.IA_HideMembers2a::get_Item
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method A_HideMembers2a::ICSharpCode.Decompiler.Tests.TestCases.Pretty.IA_HideMembers2a.get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideMembers2a::.ctor

  .property instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.IA_HideMembers2a.Item(int32)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers2a::ICSharpCode.Decompiler.Tests.TestCases.Pretty.IA_HideMembers2a.get_Item(int32)
  } // end of property A_HideMembers2a::ICSharpCode.Decompiler.Tests.TestCases.Pretty.IA_HideMembers2a.Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers2a

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A1_HideMembers2a
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers2a
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance int32 
          get_Item(int32 i) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method A1_HideMembers2a::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers2a::.ctor()
    IL_0006:  ret
  } // end of method A1_HideMembers2a::.ctor

  .property instance int32 Item(int32)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.A1_HideMembers2a::get_Item(int32)
  } // end of property A1_HideMembers2a::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A1_HideMembers2a

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.G_HideMembers3`1<T>
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void 
          M1(!T p) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method G_HideMembers3`1::M1

  .method public hidebysig instance int32 
          M2(int32 t) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method G_HideMembers3`1::M2

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method G_HideMembers3`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.G_HideMembers3`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.G1_HideMembers3`1<T>
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.G_HideMembers3`1<int32>
{
  .method public hidebysig instance int32 
          M1(int32 i) cil managed
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
  } // end of method G1_HideMembers3`1::M1

  .method public hidebysig instance int32 
          M2(!T i) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method G1_HideMembers3`1::M2

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.G_HideMembers3`1<int32>::.ctor()
    IL_0006:  ret
  } // end of method G1_HideMembers3`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.G1_HideMembers3`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.G2_HideMembers3`1<T>
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.G_HideMembers3`1<int32>
{
  .method public hidebysig instance int32 
          M1(!T p) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.4
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method G2_HideMembers3`1::M1

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.G_HideMembers3`1<int32>::.ctor()
    IL_0006:  ret
  } // end of method G2_HideMembers3`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.G2_HideMembers3`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.J_HideMembers3
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname instance int32 
          get_P() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method J_HideMembers3::get_P

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method J_HideMembers3::.ctor

  .property instance int32 P()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.J_HideMembers3::get_P()
  } // end of property J_HideMembers3::P
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.J_HideMembers3

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.J2_HideMembers3
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.J_HideMembers3
{
  .field public int32 get_P
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.J_HideMembers3::.ctor()
    IL_0006:  ret
  } // end of method J2_HideMembers3::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.J2_HideMembers3

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers4
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void 
          M<T>(!!T t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method A_HideMembers4::M

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideMembers4::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers4

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A1_HideMembers4
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers4
{
  .method public hidebysig instance void 
          M<K>(!!K t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method A1_HideMembers4::M

  .method public hidebysig instance void 
          M(int32 t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method A1_HideMembers4::M

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers4::.ctor()
    IL_0006:  ret
  } // end of method A1_HideMembers4::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A1_HideMembers4

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMembers4
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void 
          M<T>() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HideMembers4::M

  .method public hidebysig instance void 
          M1<T>() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HideMembers4::M1

  .method public hidebysig instance void 
          M2<T>(!!T t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HideMembers4::M2

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method B_HideMembers4::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMembers4

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B1_HideMembers4
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMembers4
{
  .method public hidebysig instance void 
          M<T1,T2>() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B1_HideMembers4::M

  .method public hidebysig instance void 
          M1<R>() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B1_HideMembers4::M1

  .method public hidebysig instance void 
          M2<R>(!!R r) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B1_HideMembers4::M2

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMembers4::.ctor()
    IL_0006:  ret
  } // end of method B1_HideMembers4::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B1_HideMembers4

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMembers4`1<T>
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void 
          M<TT>(!T t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method C_HideMembers4`1::M

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method C_HideMembers4`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMembers4`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.C1_HideMembers4`1<K>
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMembers4`1<!K>
{
  .method public hidebysig instance void 
          M<TT>(!!TT t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method C1_HideMembers4`1::M

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMembers4`1<!K>::.ctor()
    IL_0006:  ret
  } // end of method C1_HideMembers4`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C1_HideMembers4`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers5
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void 
          M(int32 t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method A_HideMembers5::M

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideMembers5::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers5

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A1_HideMembers5
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers5
{
  .method public hidebysig instance void 
          M(int32& t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method A1_HideMembers5::M

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMembers5::.ctor()
    IL_0006:  ret
  } // end of method A1_HideMembers5::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A1_HideMembers5

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMembers5
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void 
          M(int32& l) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HideMembers5::M

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method B_HideMembers5::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMembers5

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B1_HideMembers5
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMembers5
{
  .method public hidebysig instance void 
          M([out] int32& l) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.2
    IL_0003:  stind.i4
    IL_0004:  ret
  } // end of method B1_HideMembers5::M

  .method public hidebysig instance void 
          M(int64& l) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B1_HideMembers5::M

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMembers5::.ctor()
    IL_0006:  ret
  } // end of method B1_HideMembers5::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B1_HideMembers5

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMemberSkipNotVisible
       extends [mscorlib]System.Object
{
  .field family int32 F
  .method family hidebysig specialname instance string 
          get_P() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method A_HideMemberSkipNotVisible::get_P

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideMemberSkipNotVisible::.ctor

  .property instance string P()
  {
    .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMemberSkipNotVisible::get_P()
  } // end of property A_HideMemberSkipNotVisible::P
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMemberSkipNotVisible

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMemberSkipNotVisible
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMemberSkipNotVisible
{
  .field private string F
  .method private hidebysig specialname instance void 
          set_P(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HideMemberSkipNotVisible::set_P

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMemberSkipNotVisible::.ctor()
    IL_0006:  ret
  } // end of method B_HideMemberSkipNotVisible::.ctor

  .property instance int32 P()
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMemberSkipNotVisible::set_P(int32)
  } // end of property B_HideMemberSkipNotVisible::P
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMemberSkipNotVisible

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideNestedClass
       extends [mscorlib]System.Object
{
  .class auto ansi nested public beforefieldinit N1
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
    } // end of method N1::.ctor

  } // end of class N1

  .class auto ansi nested family beforefieldinit N2
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
    } // end of method N2::.ctor

  } // end of class N2

  .class auto ansi nested private beforefieldinit N3
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
    } // end of method N3::.ctor

  } // end of class N3

  .class auto ansi nested assembly beforefieldinit N4
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
    } // end of method N4::.ctor

  } // end of class N4

  .class auto ansi nested famorassem beforefieldinit N5
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
    } // end of method N5::.ctor

  } // end of class N5

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideNestedClass::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideNestedClass

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideNestedClass
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideNestedClass
{
  .field public int32 N1
  .field public int32 N2
  .field public int32 N3
  .field public int32 N4
  .field public int32 N5
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideNestedClass::.ctor()
    IL_0006:  ret
  } // end of method B_HideNestedClass::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideNestedClass

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HidePropertyReservedMethod
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname instance int32 
          get_P() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method A_HidePropertyReservedMethod::get_P

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HidePropertyReservedMethod::.ctor

  .property instance int32 P()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HidePropertyReservedMethod::get_P()
  } // end of property A_HidePropertyReservedMethod::P
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HidePropertyReservedMethod

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HidePropertyReservedMethod
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HidePropertyReservedMethod
{
  .method public hidebysig instance int32 
          get_P() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method B_HidePropertyReservedMethod::get_P

  .method public hidebysig instance void 
          set_P(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HidePropertyReservedMethod::set_P

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HidePropertyReservedMethod::.ctor()
    IL_0006:  ret
  } // end of method B_HidePropertyReservedMethod::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HidePropertyReservedMethod

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideIndexerDiffAccessor
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance int32 
          get_Item(int32 i) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method A_HideIndexerDiffAccessor::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideIndexerDiffAccessor::.ctor

  .property instance int32 Item(int32)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideIndexerDiffAccessor::get_Item(int32)
  } // end of property A_HideIndexerDiffAccessor::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideIndexerDiffAccessor

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideIndexerDiffAccessor
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideIndexerDiffAccessor
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance void 
          set_Item(int32 j,
                   int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HideIndexerDiffAccessor::set_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideIndexerDiffAccessor::.ctor()
    IL_0006:  ret
  } // end of method B_HideIndexerDiffAccessor::.ctor

  .property instance int32 Item(int32)
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideIndexerDiffAccessor::set_Item(int32,
                                                                                                         int32)
  } // end of property B_HideIndexerDiffAccessor::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideIndexerDiffAccessor

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideIndexerGeneric`1<T>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig newslot specialname virtual 
          instance int32  get_Item(!T r) cil managed
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
  } // end of method A_HideIndexerGeneric`1::get_Item

  .method public hidebysig newslot specialname virtual 
          instance void  set_Item(!T r,
                                  int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method A_HideIndexerGeneric`1::set_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideIndexerGeneric`1::.ctor

  .property instance int32 Item(!T)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideIndexerGeneric`1::get_Item(!T)
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideIndexerGeneric`1::set_Item(!T,
                                                                                                      int32)
  } // end of property A_HideIndexerGeneric`1::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideIndexerGeneric`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideIndexerGeneric
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideIndexerGeneric`1<int32>
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method private hidebysig specialname instance int32 
          get_Item(int32 k) cil managed
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
  } // end of method B_HideIndexerGeneric::get_Item

  .method private hidebysig specialname instance void 
          set_Item(int32 k,
                   int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HideIndexerGeneric::set_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideIndexerGeneric`1<int32>::.ctor()
    IL_0006:  ret
  } // end of method B_HideIndexerGeneric::.ctor

  .property instance int32 Item(int32)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideIndexerGeneric::get_Item(int32)
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideIndexerGeneric::set_Item(int32,
                                                                                                    int32)
  } // end of property B_HideIndexerGeneric::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideIndexerGeneric

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideIndexerGeneric`1<T>
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideIndexerGeneric`1<!T>
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname virtual 
          instance void  set_Item(!T s,
                                  int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method C_HideIndexerGeneric`1::set_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideIndexerGeneric`1<!T>::.ctor()
    IL_0006:  ret
  } // end of method C_HideIndexerGeneric`1::.ctor

  .property instance int32 Item(!T)
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideIndexerGeneric`1::set_Item(!T,
                                                                                                      int32)
  } // end of property C_HideIndexerGeneric`1::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideIndexerGeneric`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.D_HideIndexerGeneric`1<T>
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideIndexerGeneric`1<!T>
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig newslot specialname virtual 
          instance void  set_Item(!T s,
                                  int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method D_HideIndexerGeneric`1::set_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideIndexerGeneric`1<!T>::.ctor()
    IL_0006:  ret
  } // end of method D_HideIndexerGeneric`1::.ctor

  .property instance int32 Item(!T)
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.D_HideIndexerGeneric`1::set_Item(!T,
                                                                                                      int32)
  } // end of property D_HideIndexerGeneric`1::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.D_HideIndexerGeneric`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethod
       extends [mscorlib]System.Object
{
  .method public hidebysig newslot virtual 
          instance void  F() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method A_HideMethod::F

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideMethod::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethod

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethod
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethod
{
  .method private hidebysig instance void 
          F() cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethod::F()
    IL_0007:  nop
    IL_0008:  ret
  } // end of method B_HideMethod::F

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethod::.ctor()
    IL_0006:  ret
  } // end of method B_HideMethod::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethod

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMethod
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethod
{
  .method public hidebysig virtual instance void 
          F() cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethod::F()
    IL_0007:  nop
    IL_0008:  ret
  } // end of method C_HideMethod::F

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethod::.ctor()
    IL_0006:  ret
  } // end of method C_HideMethod::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMethod

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodGeneric`1<T>
       extends [mscorlib]System.Object
{
  .method public hidebysig newslot virtual 
          instance void  F(!T s) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method A_HideMethodGeneric`1::F

  .method public hidebysig static bool  Equals(object o1,
                                               object o2) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method A_HideMethodGeneric`1::Equals

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideMethodGeneric`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodGeneric`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodGeneric
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodGeneric`1<string>
{
  .method private hidebysig instance void 
          F(string k) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HideMethodGeneric::F

  .method public hidebysig instance void 
          F(int32 i) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HideMethodGeneric::F

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodGeneric`1<string>::.ctor()
    IL_0006:  ret
  } // end of method B_HideMethodGeneric::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodGeneric

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMethodGeneric`1<T>
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodGeneric`1<!T>
{
  .method public hidebysig virtual instance void 
          F(!T r) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method C_HideMethodGeneric`1::F

  .method public hidebysig instance void 
          G(!T t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method C_HideMethodGeneric`1::G

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodGeneric`1<!T>::.ctor()
    IL_0006:  ret
  } // end of method C_HideMethodGeneric`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMethodGeneric`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.D_HideMethodGeneric`1<T1>
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMethodGeneric`1<!T1>
{
  .method public hidebysig newslot virtual 
          instance void  F(!T1 k) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method D_HideMethodGeneric`1::F

  .method public hidebysig newslot virtual 
          instance void  F<T2>(!!T2 k) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method D_HideMethodGeneric`1::F

  .method public hidebysig newslot virtual 
          instance void  G<T2>(!!T2 t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method D_HideMethodGeneric`1::G

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMethodGeneric`1<!T1>::.ctor()
    IL_0006:  ret
  } // end of method D_HideMethodGeneric`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.D_HideMethodGeneric`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodGenericSkipPrivate`1<T>
       extends [mscorlib]System.Object
{
  .method public hidebysig newslot virtual 
          instance void  F(!T t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method A_HideMethodGenericSkipPrivate`1::F

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideMethodGenericSkipPrivate`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodGenericSkipPrivate`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodGenericSkipPrivate`1<T>
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodGenericSkipPrivate`1<!T>
{
  .method private hidebysig instance void 
          F(!T t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HideMethodGenericSkipPrivate`1::F

  .method private hidebysig instance void 
          K() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HideMethodGenericSkipPrivate`1::K

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodGenericSkipPrivate`1<!T>::.ctor()
    IL_0006:  ret
  } // end of method B_HideMethodGenericSkipPrivate`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodGenericSkipPrivate`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMethodGenericSkipPrivate`1<T>
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodGenericSkipPrivate`1<!T>
{
  .method public hidebysig virtual instance void 
          F(!T tt) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method C_HideMethodGenericSkipPrivate`1::F

  .method public hidebysig instance void 
          K() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method C_HideMethodGenericSkipPrivate`1::K

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodGenericSkipPrivate`1<!T>::.ctor()
    IL_0006:  ret
  } // end of method C_HideMethodGenericSkipPrivate`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMethodGenericSkipPrivate`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.D_HideMethodGenericSkipPrivate
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodGenericSkipPrivate`1<int32>
{
  .method public hidebysig virtual instance void 
          F(int32 t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method D_HideMethodGenericSkipPrivate::F

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodGenericSkipPrivate`1<int32>::.ctor()
    IL_0006:  ret
  } // end of method D_HideMethodGenericSkipPrivate::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.D_HideMethodGenericSkipPrivate

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodGeneric2
       extends [mscorlib]System.Object
{
  .method public hidebysig newslot virtual 
          instance void  F(int32 i) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method A_HideMethodGeneric2::F

  .method public hidebysig instance void 
          K() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method A_HideMethodGeneric2::K

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideMethodGeneric2::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodGeneric2

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodGeneric2`1<T>
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodGeneric2
{
  .method family hidebysig newslot virtual 
          instance void  F(!T t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HideMethodGeneric2`1::F

  .method public hidebysig instance void 
          K<T2>() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method B_HideMethodGeneric2`1::K

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodGeneric2::.ctor()
    IL_0006:  ret
  } // end of method B_HideMethodGeneric2`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodGeneric2`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMethodGeneric2
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodGeneric2`1<int32>
{
  .method family hidebysig virtual instance void 
          F(int32 k) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method C_HideMethodGeneric2::F

  .method public hidebysig instance void 
          K<T3>() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method C_HideMethodGeneric2::K

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodGeneric2`1<int32>::.ctor()
    IL_0006:  ret
  } // end of method C_HideMethodGeneric2::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideMethodGeneric2

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.D_HideMethodGeneric2
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodGeneric2`1<string>
{
  .method public hidebysig virtual instance void 
          F(int32 k) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method D_HideMethodGeneric2::F

  .method public hidebysig instance void 
          L<T4>() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method D_HideMethodGeneric2::L

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodGeneric2`1<string>::.ctor()
    IL_0006:  ret
  } // end of method D_HideMethodGeneric2::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.D_HideMethodGeneric2

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.E_HideMethodGeneric2`1<T>
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void 
          M<T2>(!T t,
                !!T2 t2) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method E_HideMethodGeneric2`1::M

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method E_HideMethodGeneric2`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.E_HideMethodGeneric2`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.F_HideMethodGeneric2`1<T>
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.E_HideMethodGeneric2`1<!T>
{
  .method public hidebysig instance void 
          M(!T t1,
            !T t2) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method F_HideMethodGeneric2`1::M

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.E_HideMethodGeneric2`1<!T>::.ctor()
    IL_0006:  ret
  } // end of method F_HideMethodGeneric2`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.F_HideMethodGeneric2`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.C1_HideMethodDiffSignatures`1<T>
       extends [mscorlib]System.Object
{
  .method public hidebysig newslot virtual 
          instance void  M(!T arg) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method C1_HideMethodDiffSignatures`1::M

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method C1_HideMethodDiffSignatures`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C1_HideMethodDiffSignatures`1

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.C2_HideMethodDiffSignatures`2<T1,T2>
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C1_HideMethodDiffSignatures`1<!T2>
{
  .method public hidebysig newslot virtual 
          instance void  M(!T2 arg) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method C2_HideMethodDiffSignatures`2::M

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C1_HideMethodDiffSignatures`1<!T2>::.ctor()
    IL_0006:  ret
  } // end of method C2_HideMethodDiffSignatures`2::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C2_HideMethodDiffSignatures`2

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.C3_HideMethodDiffSignatures
       extends class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C2_HideMethodDiffSignatures`2<int32,bool>
{
  .method public hidebysig newslot virtual 
          instance void  M(bool arg) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method C3_HideMethodDiffSignatures::M

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C2_HideMethodDiffSignatures`2<int32,bool>::.ctor()
    IL_0006:  ret
  } // end of method C3_HideMethodDiffSignatures::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C3_HideMethodDiffSignatures

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodStatic
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname instance int32 
          get_N() cil managed
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
  } // end of method A_HideMethodStatic::get_N

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideMethodStatic::.ctor

  .property instance int32 N()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodStatic::get_N()
  } // end of property A_HideMethodStatic::N
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideMethodStatic

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodStatic
       extends [mscorlib]System.Object
{
  .method public hidebysig instance int32 
          N() cil managed
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
  } // end of method B_HideMethodStatic::N

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method B_HideMethodStatic::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideMethodStatic

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent
       extends [mscorlib]System.Object
{
  .field private class [mscorlib]System.EventHandler E
  .field private class [mscorlib]System.EventHandler F
  .method public hidebysig newslot specialname virtual 
          instance void  add_E(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       48 (0x30)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2,
             bool V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent::E
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate,
                                                                                            class [mscorlib]System.Delegate)
    IL_0010:  castclass  [mscorlib]System.EventHandler
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent::E
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                          !!0,
                                                                                                                          !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  ceq
    IL_0028:  ldc.i4.0
    IL_0029:  ceq
    IL_002b:  stloc.3
    IL_002c:  ldloc.3
    IL_002d:  brtrue.s   IL_0007

    IL_002f:  ret
  } // end of method A_HideEvent::add_E

  .method public hidebysig newslot specialname virtual 
          instance void  remove_E(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       48 (0x30)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2,
             bool V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent::E
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate,
                                                                                           class [mscorlib]System.Delegate)
    IL_0010:  castclass  [mscorlib]System.EventHandler
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent::E
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                          !!0,
                                                                                                                          !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  ceq
    IL_0028:  ldc.i4.0
    IL_0029:  ceq
    IL_002b:  stloc.3
    IL_002c:  ldloc.3
    IL_002d:  brtrue.s   IL_0007

    IL_002f:  ret
  } // end of method A_HideEvent::remove_E

  .method public hidebysig specialname instance void 
          add_F(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       48 (0x30)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2,
             bool V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent::F
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate,
                                                                                            class [mscorlib]System.Delegate)
    IL_0010:  castclass  [mscorlib]System.EventHandler
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent::F
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                          !!0,
                                                                                                                          !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  ceq
    IL_0028:  ldc.i4.0
    IL_0029:  ceq
    IL_002b:  stloc.3
    IL_002c:  ldloc.3
    IL_002d:  brtrue.s   IL_0007

    IL_002f:  ret
  } // end of method A_HideEvent::add_F

  .method public hidebysig specialname instance void 
          remove_F(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       48 (0x30)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2,
             bool V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent::F
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate,
                                                                                           class [mscorlib]System.Delegate)
    IL_0010:  castclass  [mscorlib]System.EventHandler
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent::F
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                          !!0,
                                                                                                                          !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  ceq
    IL_0028:  ldc.i4.0
    IL_0029:  ceq
    IL_002b:  stloc.3
    IL_002c:  ldloc.3
    IL_002d:  brtrue.s   IL_0007

    IL_002f:  ret
  } // end of method A_HideEvent::remove_F

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A_HideEvent::.ctor

  .event [mscorlib]System.EventHandler E
  {
    .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent::remove_E(class [mscorlib]System.EventHandler)
    .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent::add_E(class [mscorlib]System.EventHandler)
  } // end of event A_HideEvent::E
  .event [mscorlib]System.EventHandler F
  {
    .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent::remove_F(class [mscorlib]System.EventHandler)
    .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent::add_F(class [mscorlib]System.EventHandler)
  } // end of event A_HideEvent::F
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent
{
  .field private class [mscorlib]System.EventHandler E
  .field private class [mscorlib]System.EventHandler F
  .method public hidebysig newslot specialname virtual 
          instance void  add_E(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       48 (0x30)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2,
             bool V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent::E
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate,
                                                                                            class [mscorlib]System.Delegate)
    IL_0010:  castclass  [mscorlib]System.EventHandler
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent::E
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                          !!0,
                                                                                                                          !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  ceq
    IL_0028:  ldc.i4.0
    IL_0029:  ceq
    IL_002b:  stloc.3
    IL_002c:  ldloc.3
    IL_002d:  brtrue.s   IL_0007

    IL_002f:  ret
  } // end of method B_HideEvent::add_E

  .method public hidebysig newslot specialname virtual 
          instance void  remove_E(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       48 (0x30)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2,
             bool V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent::E
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate,
                                                                                           class [mscorlib]System.Delegate)
    IL_0010:  castclass  [mscorlib]System.EventHandler
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent::E
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                          !!0,
                                                                                                                          !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  ceq
    IL_0028:  ldc.i4.0
    IL_0029:  ceq
    IL_002b:  stloc.3
    IL_002c:  ldloc.3
    IL_002d:  brtrue.s   IL_0007

    IL_002f:  ret
  } // end of method B_HideEvent::remove_E

  .method public hidebysig specialname instance void 
          add_F(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       48 (0x30)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2,
             bool V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent::F
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate,
                                                                                            class [mscorlib]System.Delegate)
    IL_0010:  castclass  [mscorlib]System.EventHandler
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent::F
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                          !!0,
                                                                                                                          !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  ceq
    IL_0028:  ldc.i4.0
    IL_0029:  ceq
    IL_002b:  stloc.3
    IL_002c:  ldloc.3
    IL_002d:  brtrue.s   IL_0007

    IL_002f:  ret
  } // end of method B_HideEvent::add_F

  .method public hidebysig specialname instance void 
          remove_F(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       48 (0x30)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2,
             bool V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent::F
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate,
                                                                                           class [mscorlib]System.Delegate)
    IL_0010:  castclass  [mscorlib]System.EventHandler
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent::F
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                          !!0,
                                                                                                                          !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  ceq
    IL_0028:  ldc.i4.0
    IL_0029:  ceq
    IL_002b:  stloc.3
    IL_002c:  ldloc.3
    IL_002d:  brtrue.s   IL_0007

    IL_002f:  ret
  } // end of method B_HideEvent::remove_F

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.A_HideEvent::.ctor()
    IL_0006:  ret
  } // end of method B_HideEvent::.ctor

  .event [mscorlib]System.EventHandler E
  {
    .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent::add_E(class [mscorlib]System.EventHandler)
    .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent::remove_E(class [mscorlib]System.EventHandler)
  } // end of event B_HideEvent::E
  .event [mscorlib]System.EventHandler F
  {
    .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent::add_F(class [mscorlib]System.EventHandler)
    .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent::remove_F(class [mscorlib]System.EventHandler)
  } // end of event B_HideEvent::F
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideEvent
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent
{
  .field private class [mscorlib]System.EventHandler E
  .method public hidebysig specialname virtual 
          instance void  add_E(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       48 (0x30)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2,
             bool V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideEvent::E
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate,
                                                                                            class [mscorlib]System.Delegate)
    IL_0010:  castclass  [mscorlib]System.EventHandler
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideEvent::E
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                          !!0,
                                                                                                                          !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  ceq
    IL_0028:  ldc.i4.0
    IL_0029:  ceq
    IL_002b:  stloc.3
    IL_002c:  ldloc.3
    IL_002d:  brtrue.s   IL_0007

    IL_002f:  ret
  } // end of method C_HideEvent::add_E

  .method public hidebysig specialname virtual 
          instance void  remove_E(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       48 (0x30)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2,
             bool V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideEvent::E
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldarg.1
    IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate,
                                                                                           class [mscorlib]System.Delegate)
    IL_0010:  castclass  [mscorlib]System.EventHandler
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideEvent::E
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                          !!0,
                                                                                                                          !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  ceq
    IL_0028:  ldc.i4.0
    IL_0029:  ceq
    IL_002b:  stloc.3
    IL_002c:  ldloc.3
    IL_002d:  brtrue.s   IL_0007

    IL_002f:  ret
  } // end of method C_HideEvent::remove_E

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.B_HideEvent::.ctor()
    IL_0006:  ret
  } // end of method C_HideEvent::.ctor

  .event [mscorlib]System.EventHandler E
  {
    .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideEvent::add_E(class [mscorlib]System.EventHandler)
    .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideEvent::remove_E(class [mscorlib]System.EventHandler)
  } // end of event C_HideEvent::E
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.C_HideEvent


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
