



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly CustomAttributeSamples
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
.module CustomAttributeSamples.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi sealed ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.AppliedToDelegate
       extends [mscorlib]System.MulticastDelegate
{
  .custom instance void [mscorlib]System.ObsoleteAttribute::.ctor(string) = ( 01 00 06 72 65 61 73 6F 6E 00 00 )                // ...reason..
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(object 'object',
                               native int 'method') runtime managed
  {
  } // end of method AppliedToDelegate::.ctor

  .method public hidebysig newslot virtual 
          instance int32  Invoke() runtime managed
  {
  } // end of method AppliedToDelegate::Invoke

  .method public hidebysig newslot virtual 
          instance class [mscorlib]System.IAsyncResult 
          BeginInvoke(class [mscorlib]System.AsyncCallback callback,
                      object 'object') runtime managed
  {
  } // end of method AppliedToDelegate::BeginInvoke

  .method public hidebysig newslot virtual 
          instance int32  EndInvoke(class [mscorlib]System.IAsyncResult result) runtime managed
  {
  } // end of method AppliedToDelegate::EndInvoke

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.AppliedToDelegate

.class interface public abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.AppliedToInterface
{
  .custom instance void [mscorlib]System.ObsoleteAttribute::.ctor(string) = ( 01 00 06 72 65 61 73 6F 6E 00 00 )                // ...reason..
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.AppliedToInterface

.class public sequential ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.AppliedToStruct
       extends [mscorlib]System.ValueType
{
  .custom instance void [mscorlib]System.ObsoleteAttribute::.ctor(string) = ( 01 00 06 72 65 61 73 6F 6E 00 00 )                // ...reason..
  .field public int32 Field
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.AppliedToStruct

.class public auto ansi sealed ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.EnumWithFlagsAttribute
       extends [mscorlib]System.Enum
{
  .custom instance void [mscorlib]System.FlagsAttribute::.ctor() = ( 01 00 00 00 ) 
  .field public specialname rtspecialname int32 value__
  .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.EnumWithFlagsAttribute None = int32(0x00000000)
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.EnumWithFlagsAttribute

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 FF 7F 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyAttributeAttribute::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyMethodOrInterfaceAttributeAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 40 04 00 00 00 00 )                         // ..@.....
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyMethodOrInterfaceAttributeAttribute::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyMethodOrInterfaceAttributeAttribute

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.ObsoleteClass
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.ObsoleteAttribute::.ctor(string) = ( 01 00 07 6D 65 73 73 61 67 65 00 00 )             // ...message..
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ObsoleteClass::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.ObsoleteClass

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyTypeAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 FF 7F 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(class [mscorlib]System.Type t) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyTypeAttribute::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyTypeAttribute

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.SomeClass
       extends [mscorlib]System.Object
{
  .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyTypeAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 5D 53 79 73 74 65 6D 2E 41 74 74 72 69 62   // ..]System.Attrib
                                                                                                                                                     75 74 65 2C 20 6D 73 63 6F 72 6C 69 62 2C 20 56   // ute, mscorlib, V
                                                                                                                                                     65 72 73 69 6F 6E 3D 34 2E 30 2E 30 2E 30 2C 20   // ersion=4.0.0.0, 
                                                                                                                                                     43 75 6C 74 75 72 65 3D 6E 65 75 74 72 61 6C 2C   // Culture=neutral,
                                                                                                                                                     20 50 75 62 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D   //  PublicKeyToken=
                                                                                                                                                     62 37 37 61 35 63 35 36 31 39 33 34 65 30 38 39   // b77a5c561934e089
                                                                                                                                                     00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method SomeClass::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.SomeClass

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .field public int32 Field
  .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private class [mscorlib]System.EventHandler MyEvent
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private class [mscorlib]System.EventHandler MyEvent2
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname instance int32 
          get_Property() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  } // end of method TestClass::get_Property

  .method public hidebysig specialname instance int32 
          get_PropertyAttributeOnGetter() cil managed
  {
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  } // end of method TestClass::get_PropertyAttributeOnGetter

  .method public hidebysig specialname instance int32 
          get_PropertyAttributeOnSetter() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  ret
  } // end of method TestClass::get_PropertyAttributeOnSetter

  .method public hidebysig specialname instance void 
          set_PropertyAttributeOnSetter(int32 'value') cil managed
  {
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TestClass::set_PropertyAttributeOnSetter

  .method public hidebysig specialname instance int32 
          get_Item(int32 i) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  } // end of method TestClass::get_Item

  .method public hidebysig specialname instance void 
          add_MyEvent(class [mscorlib]System.EventHandler 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       41 (0x29)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::MyEvent
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
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::MyEvent
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                          !!0,
                                                                                                                          !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  bne.un.s   IL_0007

    IL_0028:  ret
  } // end of method TestClass::add_MyEvent

  .method public hidebysig specialname instance void 
          remove_MyEvent(class [mscorlib]System.EventHandler 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       41 (0x29)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::MyEvent
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
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::MyEvent
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                          !!0,
                                                                                                                          !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  bne.un.s   IL_0007

    IL_0028:  ret
  } // end of method TestClass::remove_MyEvent

  .method public hidebysig specialname instance void 
          add_MyEvent2(class [mscorlib]System.EventHandler 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       41 (0x29)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::MyEvent2
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
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::MyEvent2
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                          !!0,
                                                                                                                          !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  bne.un.s   IL_0007

    IL_0028:  ret
  } // end of method TestClass::add_MyEvent2

  .method public hidebysig specialname instance void 
          remove_MyEvent2(class [mscorlib]System.EventHandler 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       41 (0x29)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::MyEvent2
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
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::MyEvent2
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                          !!0,
                                                                                                                          !!0)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  bne.un.s   IL_0007

    IL_0028:  ret
  } // end of method TestClass::remove_MyEvent2

  .method public hidebysig instance void 
          Method() cil managed
  {
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TestClass::Method

  .method public hidebysig instance void 
          Method(int32 val) cil managed
  {
    .param [1]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TestClass::Method

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method TestClass::.ctor

  .event [mscorlib]System.EventHandler MyEvent
  {
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::add_MyEvent(class [mscorlib]System.EventHandler)
    .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::remove_MyEvent(class [mscorlib]System.EventHandler)
  } // end of event TestClass::MyEvent
  .event [mscorlib]System.EventHandler MyEvent2
  {
    .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::add_MyEvent2(class [mscorlib]System.EventHandler)
    .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::remove_MyEvent2(class [mscorlib]System.EventHandler)
  } // end of event TestClass::MyEvent2
  .property instance int32 Property()
  {
    .custom instance void [mscorlib]System.ObsoleteAttribute::.ctor(string) = ( 01 00 06 72 65 61 73 6F 6E 00 00 )                // ...reason..
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::get_Property()
  } // end of property TestClass::Property
  .property instance int32 PropertyAttributeOnGetter()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::get_PropertyAttributeOnGetter()
  } // end of property TestClass::PropertyAttributeOnGetter
  .property instance int32 PropertyAttributeOnSetter()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::get_PropertyAttributeOnSetter()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::set_PropertyAttributeOnSetter(int32)
  } // end of property TestClass::PropertyAttributeOnSetter
  .property instance int32 Item(int32)
  {
    .custom instance void [mscorlib]System.ObsoleteAttribute::.ctor(string) = ( 01 00 06 72 65 61 73 6F 6E 00 00 )                // ...reason..
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass::get_Item(int32)
  } // end of property TestClass::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.TestClass

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeWithNamedArgumentAppliedAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 40 00 00 00 01 00 54 02 0D 41 6C 6C 6F 77   // ..@.....T..Allow
                                                                                                                         4D 75 6C 74 69 70 6C 65 01 )                      // Multiple.
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyAttributeWithNamedArgumentAppliedAttribute::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeWithNamedArgumentAppliedAttribute

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeWithCustomPropertyAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 FF 7F 00 00 00 00 ) 
  .method public hidebysig specialname instance string 
          get_Prop() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldstr      ""
    IL_0005:  ret
  } // end of method MyAttributeWithCustomPropertyAttribute::get_Prop

  .method public hidebysig specialname instance void 
          set_Prop(string 'value') cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method MyAttributeWithCustomPropertyAttribute::set_Prop

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyAttributeWithCustomPropertyAttribute::.ctor

  .property instance string Prop()
  {
    .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeWithCustomPropertyAttribute::get_Prop()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeWithCustomPropertyAttribute::set_Prop(string)
  } // end of property MyAttributeWithCustomPropertyAttribute::Prop
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeWithCustomPropertyAttribute

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass
       extends [mscorlib]System.Object
{
  .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeWithCustomPropertyAttribute::.ctor() = ( 01 00 01 00 54 0E 04 50 72 6F 70 05 76 61 6C 75   // ....T..Prop.valu
                                                                                                                                                 65 )                                              // e
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeWithNamedInitializerPropertyTypeAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 FF 7F 00 00 00 00 ) 
  .method public hidebysig specialname instance class [mscorlib]System.Type 
          get_Prop() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ret
  } // end of method MyAttributeWithNamedInitializerPropertyTypeAttribute::get_Prop

  .method public hidebysig specialname instance void 
          set_Prop(class [mscorlib]System.Type 'value') cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method MyAttributeWithNamedInitializerPropertyTypeAttribute::set_Prop

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyAttributeWithNamedInitializerPropertyTypeAttribute::.ctor

  .property instance class [mscorlib]System.Type
          Prop()
  {
    .get instance class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeWithNamedInitializerPropertyTypeAttribute::get_Prop()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeWithNamedInitializerPropertyTypeAttribute::set_Prop(class [mscorlib]System.Type)
  } // end of property MyAttributeWithNamedInitializerPropertyTypeAttribute::Prop
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeWithNamedInitializerPropertyTypeAttribute

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass2
       extends [mscorlib]System.Object
{
  .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeWithNamedInitializerPropertyTypeAttribute::.ctor() = ( 01 00 01 00 54 50 04 50 72 6F 70 58 53 79 73 74   // ....TP.PropXSyst
                                                                                                                                                               65 6D 2E 45 6E 75 6D 2C 20 6D 73 63 6F 72 6C 69   // em.Enum, mscorli
                                                                                                                                                               62 2C 20 56 65 72 73 69 6F 6E 3D 34 2E 30 2E 30   // b, Version=4.0.0
                                                                                                                                                               2E 30 2C 20 43 75 6C 74 75 72 65 3D 6E 65 75 74   // .0, Culture=neut
                                                                                                                                                               72 61 6C 2C 20 50 75 62 6C 69 63 4B 65 79 54 6F   // ral, PublicKeyTo
                                                                                                                                                               6B 65 6E 3D 62 37 37 61 35 63 35 36 31 39 33 34   // ken=b77a5c561934
                                                                                                                                                               65 30 38 39 )                                     // e089
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass2::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass2

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeNamedInitializerPropertyEnumAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 FF 7F 00 00 00 00 ) 
  .method public hidebysig specialname instance valuetype [mscorlib]System.AttributeTargets 
          get_Prop() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldc.i4     0x7fff
    IL_0005:  ret
  } // end of method MyAttributeNamedInitializerPropertyEnumAttribute::get_Prop

  .method public hidebysig specialname instance void 
          set_Prop(valuetype [mscorlib]System.AttributeTargets 'value') cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method MyAttributeNamedInitializerPropertyEnumAttribute::set_Prop

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyAttributeNamedInitializerPropertyEnumAttribute::.ctor

  .property instance valuetype [mscorlib]System.AttributeTargets
          Prop()
  {
    .get instance valuetype [mscorlib]System.AttributeTargets ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeNamedInitializerPropertyEnumAttribute::get_Prop()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeNamedInitializerPropertyEnumAttribute::set_Prop(valuetype [mscorlib]System.AttributeTargets)
  } // end of property MyAttributeNamedInitializerPropertyEnumAttribute::Prop
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeNamedInitializerPropertyEnumAttribute

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass3
       extends [mscorlib]System.Object
{
  .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeNamedInitializerPropertyEnumAttribute::.ctor() = ( 01 00 01 00 54 55 64 53 79 73 74 65 6D 2E 41 74   // ....TUdSystem.At
                                                                                                                                                           74 72 69 62 75 74 65 54 61 72 67 65 74 73 2C 20   // tributeTargets, 
                                                                                                                                                           6D 73 63 6F 72 6C 69 62 2C 20 56 65 72 73 69 6F   // mscorlib, Versio
                                                                                                                                                           6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75 6C 74 75   // n=4.0.0.0, Cultu
                                                                                                                                                           72 65 3D 6E 65 75 74 72 61 6C 2C 20 50 75 62 6C   // re=neutral, Publ
                                                                                                                                                           69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61 35   // icKeyToken=b77a5
                                                                                                                                                           63 35 36 31 39 33 34 65 30 38 39 04 50 72 6F 70   // c561934e089.Prop
                                                                                                                                                           44 00 00 00 )                                     // D...
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass3::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass3

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeNamedInitializerFieldEnumAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 FF 7F 00 00 00 00 ) 
  .field public valuetype [mscorlib]System.AttributeTargets Field
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyAttributeNamedInitializerFieldEnumAttribute::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeNamedInitializerFieldEnumAttribute

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass4
       extends [mscorlib]System.Object
{
  .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeNamedInitializerFieldEnumAttribute::.ctor() = ( 01 00 01 00 53 55 64 53 79 73 74 65 6D 2E 41 74   // ....SUdSystem.At
                                                                                                                                                        74 72 69 62 75 74 65 54 61 72 67 65 74 73 2C 20   // tributeTargets, 
                                                                                                                                                        6D 73 63 6F 72 6C 69 62 2C 20 56 65 72 73 69 6F   // mscorlib, Versio
                                                                                                                                                        6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75 6C 74 75   // n=4.0.0.0, Cultu
                                                                                                                                                        72 65 3D 6E 65 75 74 72 61 6C 2C 20 50 75 62 6C   // re=neutral, Publ
                                                                                                                                                        69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61 35   // icKeyToken=b77a5
                                                                                                                                                        63 35 36 31 39 33 34 65 30 38 39 05 46 69 65 6C   // c561934e089.Fiel
                                                                                                                                                        64 44 00 00 00 )                                  // dD...
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass4::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass4

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass5
       extends [mscorlib]System.Object
{
  .method public hidebysig instance int32 
          MyMethod() cil managed
  {
    .param [0]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.5
    IL_0001:  ret
  } // end of method MyClass5::MyMethod

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass5::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass5

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass6
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname instance int32 
          get_Prop() cil managed
  {
    .param [0]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  ret
  } // end of method MyClass6::get_Prop

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass6::.ctor

  .property instance int32 Prop()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass6::get_Prop()
  } // end of property MyClass6::Prop
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass6

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass7
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname instance void 
          set_Prop(int32 'value') cil managed
  {
    .param [1]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method MyClass7::set_Prop

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass7::.ctor

  .property instance int32 Prop()
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass7::set_Prop(int32)
  } // end of property MyClass7::Prop
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass7

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass8
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname instance int32 
          get_Prop() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  ret
  } // end of method MyClass8::get_Prop

  .method public hidebysig specialname instance void 
          set_Prop(int32 'value') cil managed
  {
    .param [0]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method MyClass8::set_Prop

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass8::.ctor

  .property instance int32 Prop()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass8::get_Prop()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass8::set_Prop(int32)
  } // end of property MyClass8::Prop
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass8

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass9
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance int32 
          get_Item(string s) cil managed
  {
    .param [0]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  ret
  } // end of method MyClass9::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass9::.ctor

  .property instance int32 Item(string)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass9::get_Item(string)
  } // end of property MyClass9::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass9

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass10
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance void 
          set_Item(string s,
                   int32 'value') cil managed
  {
    .param [1]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method MyClass10::set_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass10::.ctor

  .property instance int32 Item(string)
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass10::set_Item(string,
                                                                                                                int32)
  } // end of property MyClass10::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass10

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass11
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance int32 
          get_Item(string s) cil managed
  {
    .param [1]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  ret
  } // end of method MyClass11::get_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass11::.ctor

  .property instance int32 Item(string)
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass11::get_Item(string)
  } // end of property MyClass11::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass11

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass12
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance string 
          get_Item(int32 index) cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldstr      ""
    IL_0005:  ret
  } // end of method MyClass12::get_Item

  .method public hidebysig specialname instance void 
          set_Item(int32 index,
                   string 'value') cil managed
  {
    .param [0]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method MyClass12::set_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass12::.ctor

  .property instance string Item(int32)
  {
    .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass12::get_Item(int32)
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass12::set_Item(int32,
                                                                                                                string)
  } // end of property MyClass12::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass12

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeTargetPropertyIndexSetMultiParamAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 FF 7F 00 00 00 00 ) 
  .field public int32 Field
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyAttributeTargetPropertyIndexSetMultiParamAttribute::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeTargetPropertyIndexSetMultiParamAttribute

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass13
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .method public hidebysig specialname instance string 
          get_Item(int32 index1,
                   int32 index2) cil managed
  {
    .param [1]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeTargetPropertyIndexSetMultiParamAttribute::.ctor() = ( 01 00 01 00 53 08 05 46 69 65 6C 64 02 00 00 00 ) // ....S..Field....
    .param [2]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeTargetPropertyIndexSetMultiParamAttribute::.ctor() = ( 01 00 01 00 53 08 05 46 69 65 6C 64 03 00 00 00 ) // ....S..Field....
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldstr      ""
    IL_0005:  ret
  } // end of method MyClass13::get_Item

  .method public hidebysig specialname instance void 
          set_Item(int32 index1,
                   int32 index2,
                   string 'value') cil managed
  {
    .param [1]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeTargetPropertyIndexSetMultiParamAttribute::.ctor() = ( 01 00 01 00 53 08 05 46 69 65 6C 64 02 00 00 00 ) // ....S..Field....
    .param [2]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeTargetPropertyIndexSetMultiParamAttribute::.ctor() = ( 01 00 01 00 53 08 05 46 69 65 6C 64 03 00 00 00 ) // ....S..Field....
    .param [3]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method MyClass13::set_Item

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass13::.ctor

  .property instance string Item(int32,
                                 int32)
  {
    .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass13::get_Item(int32,
                                                                                                                  int32)
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass13::set_Item(int32,
                                                                                                                int32,
                                                                                                                string)
  } // end of property MyClass13::Item
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass13

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeOnReturnTypeOfDelegateAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 FF 7F 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyAttributeOnReturnTypeOfDelegateAttribute::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeOnReturnTypeOfDelegateAttribute

.class public auto ansi sealed ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.Test
       extends [mscorlib]System.MulticastDelegate
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(object 'object',
                               native int 'method') runtime managed
  {
  } // end of method Test::.ctor

  .method public hidebysig newslot virtual 
          instance void  Invoke() runtime managed
  {
    .param [0]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeOnReturnTypeOfDelegateAttribute::.ctor() = ( 01 00 00 00 ) 
  } // end of method Test::Invoke

  .method public hidebysig newslot virtual 
          instance class [mscorlib]System.IAsyncResult 
          BeginInvoke(class [mscorlib]System.AsyncCallback callback,
                      object 'object') runtime managed
  {
  } // end of method Test::BeginInvoke

  .method public hidebysig newslot virtual 
          instance void  EndInvoke(class [mscorlib]System.IAsyncResult result) runtime managed
  {
    .param [0]
    .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyAttributeOnReturnTypeOfDelegateAttribute::.ctor() = ( 01 00 00 00 ) 
  } // end of method Test::EndInvoke

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.Test

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClassAttributeOnTypeParameterAttribute
       extends [mscorlib]System.Attribute
{
  .custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 FF 7F 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method MyClassAttributeOnTypeParameterAttribute::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClassAttributeOnTypeParameterAttribute

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass`1<T>
       extends [mscorlib]System.Object
{
  .param type T 
  .custom instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClassAttributeOnTypeParameterAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomAttributeSamples.MyClass`1


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
