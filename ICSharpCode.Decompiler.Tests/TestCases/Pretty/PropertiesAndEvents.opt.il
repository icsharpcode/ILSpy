
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly sagvhpmx
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module sagvhpmx.dll
// MVID: {01D434AD-9489-4A19-81AE-3BC2177A09C3}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02850000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents
       extends [mscorlib]System.Object
{
  .class interface abstract auto ansi nested private IBase
  {
    .method public hidebysig newslot specialname abstract virtual 
            instance void  add_Event(class [mscorlib]System.Action 'value') cil managed
    {
    } // end of method IBase::add_Event

    .method public hidebysig newslot specialname abstract virtual 
            instance void  remove_Event(class [mscorlib]System.Action 'value') cil managed
    {
    } // end of method IBase::remove_Event

    .method public hidebysig newslot specialname abstract virtual 
            instance int32  get_Test() cil managed
    {
    } // end of method IBase::get_Test

    .method public hidebysig newslot specialname abstract virtual 
            instance void  set_Test(int32 'value') cil managed
    {
    } // end of method IBase::set_Test

    .event [mscorlib]System.Action Event
    {
      .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::add_Event(class [mscorlib]System.Action)
      .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::remove_Event(class [mscorlib]System.Action)
    } // end of event IBase::Event
    .property instance int32 Test()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::set_Test(int32)
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::get_Test()
    } // end of property IBase::Test
  } // end of class IBase

  .class auto ansi nested private beforefieldinit Impl
         extends [mscorlib]System.Object
         implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase
  {
    .method private hidebysig newslot specialname virtual final 
            instance void  ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.add_Event(class [mscorlib]System.Action 'value') cil managed
    {
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::add_Event
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Impl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.add_Event

    .method private hidebysig newslot specialname virtual final 
            instance void  ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.remove_Event(class [mscorlib]System.Action 'value') cil managed
    {
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::remove_Event
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Impl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.remove_Event

    .method private hidebysig newslot specialname virtual final 
            instance int32  ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.get_Test() cil managed
    {
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::get_Test
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method Impl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.get_Test

    .method private hidebysig newslot specialname virtual final 
            instance void  ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.set_Test(int32 'value') cil managed
    {
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::set_Test
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Impl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.set_Test

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method Impl::.ctor

    .event [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.Event
    {
      .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Impl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.add_Event(class [mscorlib]System.Action)
      .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Impl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.remove_Event(class [mscorlib]System.Action)
    } // end of event Impl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.Event
    .property instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.Test()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Impl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.get_Test()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Impl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.set_Test(int32)
    } // end of property Impl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.Test
  } // end of class Impl

  .field private class [mscorlib]System.EventHandler AutomaticEvent
  .field private notserialized class [mscorlib]System.EventHandler AutomaticEventWithInitializer
  .field private int32 '<Value>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.EventHandler 'CS$<>9__CachedAnonymousMethodDelegate1'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname instance int32 
          get_Value() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::'<Value>k__BackingField'
    IL_0006:  ret
  } // end of method PropertiesAndEvents::get_Value

  .method private hidebysig specialname instance void 
          set_Value(int32 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::'<Value>k__BackingField'
    IL_0007:  ret
  } // end of method PropertiesAndEvents::set_Value

  .method public hidebysig specialname instance void 
          add_AutomaticEvent(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       41 (0x29)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::AutomaticEvent
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
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::AutomaticEvent
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
  } // end of method PropertiesAndEvents::add_AutomaticEvent

  .method public hidebysig specialname instance void 
          remove_AutomaticEvent(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       41 (0x29)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::AutomaticEvent
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
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::AutomaticEvent
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
  } // end of method PropertiesAndEvents::remove_AutomaticEvent

  .method public hidebysig specialname instance void 
          add_AutomaticEventWithInitializer(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       41 (0x29)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::AutomaticEventWithInitializer
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
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::AutomaticEventWithInitializer
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
  } // end of method PropertiesAndEvents::add_AutomaticEventWithInitializer

  .method public hidebysig specialname instance void 
          remove_AutomaticEventWithInitializer(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       41 (0x29)
    .maxstack  3
    .locals init (class [mscorlib]System.EventHandler V_0,
             class [mscorlib]System.EventHandler V_1,
             class [mscorlib]System.EventHandler V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::AutomaticEventWithInitializer
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
    IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::AutomaticEventWithInitializer
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
  } // end of method PropertiesAndEvents::remove_AutomaticEventWithInitializer

  .method public hidebysig specialname instance void 
          add_CustomEvent(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::add_AutomaticEvent(class [mscorlib]System.EventHandler)
    IL_0007:  ret
  } // end of method PropertiesAndEvents::add_CustomEvent

  .method public hidebysig specialname instance void 
          remove_CustomEvent(class [mscorlib]System.EventHandler 'value') cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::remove_AutomaticEvent(class [mscorlib]System.EventHandler)
    IL_0007:  ret
  } // end of method PropertiesAndEvents::remove_CustomEvent

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::'CS$<>9__CachedAnonymousMethodDelegate1'
    IL_0006:  brtrue.s   IL_0019

    IL_0008:  ldnull
    IL_0009:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::'<.ctor>b__0'(object,
                                                                                                               class [mscorlib]System.EventArgs)
    IL_000f:  newobj     instance void [mscorlib]System.EventHandler::.ctor(object,
                                                                            native int)
    IL_0014:  stsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::'CS$<>9__CachedAnonymousMethodDelegate1'
    IL_0019:  ldsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::'CS$<>9__CachedAnonymousMethodDelegate1'
    IL_001e:  stfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::AutomaticEventWithInitializer
    IL_0023:  ldarg.0
    IL_0024:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0029:  ret
  } // end of method PropertiesAndEvents::.ctor

  .method private hidebysig static void  '<.ctor>b__0'(object sender,
                                                       class [mscorlib]System.EventArgs e) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method PropertiesAndEvents::'<.ctor>b__0'

  .event [mscorlib]System.EventHandler AutomaticEvent
  {
    .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::remove_AutomaticEvent(class [mscorlib]System.EventHandler)
    .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::add_AutomaticEvent(class [mscorlib]System.EventHandler)
  } // end of event PropertiesAndEvents::AutomaticEvent
  .event [mscorlib]System.EventHandler AutomaticEventWithInitializer
  {
    .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::add_AutomaticEventWithInitializer(class [mscorlib]System.EventHandler)
    .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::remove_AutomaticEventWithInitializer(class [mscorlib]System.EventHandler)
  } // end of event PropertiesAndEvents::AutomaticEventWithInitializer
  .event [mscorlib]System.EventHandler CustomEvent
  {
    .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::remove_CustomEvent(class [mscorlib]System.EventHandler)
    .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::add_CustomEvent(class [mscorlib]System.EventHandler)
  } // end of event PropertiesAndEvents::CustomEvent
  .property instance int32 Value()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::get_Value()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::set_Value(int32)
  } // end of property PropertiesAndEvents::Value
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\PropertiesAndEvents.opt.res
