



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly PropertiesAndEvents.opt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module PropertiesAndEvents.opt.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents
       extends [mscorlib]System.Object
{
  .class interface abstract auto ansi nested private IBase
  {
    .method public hidebysig newslot specialname abstract virtual 
            instance int32  get_GetterOnly() cil managed
    {
    } // end of method IBase::get_GetterOnly

    .method public hidebysig newslot specialname abstract virtual 
            instance void  set_SetterOnly(int32 'value') cil managed
    {
    } // end of method IBase::set_SetterOnly

    .method public hidebysig newslot specialname abstract virtual 
            instance int32  get_Test() cil managed
    {
    } // end of method IBase::get_Test

    .method public hidebysig newslot specialname abstract virtual 
            instance void  set_Test(int32 'value') cil managed
    {
    } // end of method IBase::set_Test

    .method public hidebysig newslot specialname abstract virtual 
            instance void  add_Event(class [mscorlib]System.Action 'value') cil managed
    {
    } // end of method IBase::add_Event

    .method public hidebysig newslot specialname abstract virtual 
            instance void  remove_Event(class [mscorlib]System.Action 'value') cil managed
    {
    } // end of method IBase::remove_Event

    .event [mscorlib]System.Action Event
    {
      .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::add_Event(class [mscorlib]System.Action)
      .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::remove_Event(class [mscorlib]System.Action)
    } // end of event IBase::Event
    .property instance int32 GetterOnly()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::get_GetterOnly()
    } // end of property IBase::GetterOnly
    .property instance int32 SetterOnly()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::set_SetterOnly(int32)
    } // end of property IBase::SetterOnly
    .property instance int32 Test()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::get_Test()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::set_Test(int32)
    } // end of property IBase::Test
  } // end of class IBase

  .class abstract auto ansi nested private beforefieldinit BaseClass
         extends [mscorlib]System.Object
  {
    .method public hidebysig newslot specialname abstract virtual 
            instance void  add_ThisIsAnAbstractEvent(class [mscorlib]System.EventHandler 'value') cil managed
    {
    } // end of method BaseClass::add_ThisIsAnAbstractEvent

    .method public hidebysig newslot specialname abstract virtual 
            instance void  remove_ThisIsAnAbstractEvent(class [mscorlib]System.EventHandler 'value') cil managed
    {
    } // end of method BaseClass::remove_ThisIsAnAbstractEvent

    .method family hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method BaseClass::.ctor

    .event [mscorlib]System.EventHandler ThisIsAnAbstractEvent
    {
      .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/BaseClass::remove_ThisIsAnAbstractEvent(class [mscorlib]System.EventHandler)
      .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/BaseClass::add_ThisIsAnAbstractEvent(class [mscorlib]System.EventHandler)
    } // end of event BaseClass::ThisIsAnAbstractEvent
  } // end of class BaseClass

  .class auto ansi nested private beforefieldinit OtherClass
         extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/BaseClass
  {
    .field private class [mscorlib]System.EventHandler ThisIsAnAbstractEvent
    .method public hidebysig specialname virtual 
            instance void  add_ThisIsAnAbstractEvent(class [mscorlib]System.EventHandler 'value') cil managed
    {
      // Code size       41 (0x29)
      .maxstack  3
      .locals init (class [mscorlib]System.EventHandler V_0,
               class [mscorlib]System.EventHandler V_1,
               class [mscorlib]System.EventHandler V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/OtherClass::ThisIsAnAbstractEvent
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
      IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/OtherClass::ThisIsAnAbstractEvent
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
    } // end of method OtherClass::add_ThisIsAnAbstractEvent

    .method public hidebysig specialname virtual 
            instance void  remove_ThisIsAnAbstractEvent(class [mscorlib]System.EventHandler 'value') cil managed
    {
      // Code size       41 (0x29)
      .maxstack  3
      .locals init (class [mscorlib]System.EventHandler V_0,
               class [mscorlib]System.EventHandler V_1,
               class [mscorlib]System.EventHandler V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/OtherClass::ThisIsAnAbstractEvent
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
      IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/OtherClass::ThisIsAnAbstractEvent
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
    } // end of method OtherClass::remove_ThisIsAnAbstractEvent

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/BaseClass::.ctor()
      IL_0006:  ret
    } // end of method OtherClass::.ctor

    .event [mscorlib]System.EventHandler ThisIsAnAbstractEvent
    {
      .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/OtherClass::add_ThisIsAnAbstractEvent(class [mscorlib]System.EventHandler)
      .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/OtherClass::remove_ThisIsAnAbstractEvent(class [mscorlib]System.EventHandler)
    } // end of event OtherClass::ThisIsAnAbstractEvent
  } // end of class OtherClass

  .class auto ansi nested private beforefieldinit ExplicitImpl
         extends [mscorlib]System.Object
         implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase
  {
    .method private hidebysig newslot specialname virtual final 
            instance int32  ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.get_Test() cil managed
    {
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::get_Test
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.get_Test

    .method private hidebysig newslot specialname virtual final 
            instance void  ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.set_Test(int32 'value') cil managed
    {
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::set_Test
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.set_Test

    .method private hidebysig newslot specialname virtual final 
            instance int32  ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.get_GetterOnly() cil managed
    {
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::get_GetterOnly
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.get_GetterOnly

    .method private hidebysig newslot specialname virtual final 
            instance void  ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.set_SetterOnly(int32 'value') cil managed
    {
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::set_SetterOnly
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.set_SetterOnly

    .method private hidebysig newslot specialname virtual final 
            instance void  ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.add_Event(class [mscorlib]System.Action 'value') cil managed
    {
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::add_Event
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.add_Event

    .method private hidebysig newslot specialname virtual final 
            instance void  ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.remove_Event(class [mscorlib]System.Action 'value') cil managed
    {
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase::remove_Event
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.remove_Event

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method ExplicitImpl::.ctor

    .event [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.Event
    {
      .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.add_Event(class [mscorlib]System.Action)
      .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.remove_Event(class [mscorlib]System.Action)
    } // end of event ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.Event
    .property instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.Test()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.set_Test(int32)
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.get_Test()
    } // end of property ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.Test
    .property instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.GetterOnly()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.get_GetterOnly()
    } // end of property ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.GetterOnly
    .property instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.SetterOnly()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.set_SetterOnly(int32)
    } // end of property ExplicitImpl::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IBase.SetterOnly
  } // end of class ExplicitImpl

  .class auto ansi nested private beforefieldinit Impl
         extends [mscorlib]System.Object
         implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IBase
  {
    .field private class [mscorlib]System.Action Event
    .method public hidebysig newslot specialname virtual final 
            instance int32  get_GetterOnly() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method Impl::get_GetterOnly

    .method public hidebysig newslot specialname virtual final 
            instance void  set_SetterOnly(int32 'value') cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method Impl::set_SetterOnly

    .method public hidebysig newslot specialname virtual final 
            instance int32  get_Test() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method Impl::get_Test

    .method public hidebysig newslot specialname virtual final 
            instance void  set_Test(int32 'value') cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method Impl::set_Test

    .method public hidebysig newslot specialname virtual final 
            instance void  add_Event(class [mscorlib]System.Action 'value') cil managed
    {
      // Code size       41 (0x29)
      .maxstack  3
      .locals init (class [mscorlib]System.Action V_0,
               class [mscorlib]System.Action V_1,
               class [mscorlib]System.Action V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Impl::Event
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  stloc.1
      IL_0009:  ldloc.1
      IL_000a:  ldarg.1
      IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate,
                                                                                              class [mscorlib]System.Delegate)
      IL_0010:  castclass  [mscorlib]System.Action
      IL_0015:  stloc.2
      IL_0016:  ldarg.0
      IL_0017:  ldflda     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Impl::Event
      IL_001c:  ldloc.2
      IL_001d:  ldloc.1
      IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.Action>(!!0&,
                                                                                                                      !!0,
                                                                                                                      !!0)
      IL_0023:  stloc.0
      IL_0024:  ldloc.0
      IL_0025:  ldloc.1
      IL_0026:  bne.un.s   IL_0007

      IL_0028:  ret
    } // end of method Impl::add_Event

    .method public hidebysig newslot specialname virtual final 
            instance void  remove_Event(class [mscorlib]System.Action 'value') cil managed
    {
      // Code size       41 (0x29)
      .maxstack  3
      .locals init (class [mscorlib]System.Action V_0,
               class [mscorlib]System.Action V_1,
               class [mscorlib]System.Action V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Impl::Event
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  stloc.1
      IL_0009:  ldloc.1
      IL_000a:  ldarg.1
      IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate,
                                                                                             class [mscorlib]System.Delegate)
      IL_0010:  castclass  [mscorlib]System.Action
      IL_0015:  stloc.2
      IL_0016:  ldarg.0
      IL_0017:  ldflda     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Impl::Event
      IL_001c:  ldloc.2
      IL_001d:  ldloc.1
      IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.Action>(!!0&,
                                                                                                                      !!0,
                                                                                                                      !!0)
      IL_0023:  stloc.0
      IL_0024:  ldloc.0
      IL_0025:  ldloc.1
      IL_0026:  bne.un.s   IL_0007

      IL_0028:  ret
    } // end of method Impl::remove_Event

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method Impl::.ctor

    .event [mscorlib]System.Action Event
    {
      .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Impl::remove_Event(class [mscorlib]System.Action)
      .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Impl::add_Event(class [mscorlib]System.Action)
    } // end of event Impl::Event
    .property instance int32 GetterOnly()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Impl::get_GetterOnly()
    } // end of property Impl::GetterOnly
    .property instance int32 SetterOnly()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Impl::set_SetterOnly(int32)
    } // end of property Impl::SetterOnly
    .property instance int32 Test()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Impl::set_Test(int32)
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Impl::get_Test()
    } // end of property Impl::Test
  } // end of class Impl

  .class interface abstract auto ansi nested private IChange
  {
    .method public hidebysig newslot specialname abstract virtual 
            instance int32  get_Property() cil managed
    {
    } // end of method IChange::get_Property

    .method public hidebysig newslot specialname abstract virtual 
            instance void  set_Property(int32 'value') cil managed
    {
    } // end of method IChange::set_Property

    .method public hidebysig newslot specialname abstract virtual 
            instance void  add_Changed(class [mscorlib]System.EventHandler 'value') cil managed
    {
    } // end of method IChange::add_Changed

    .method public hidebysig newslot specialname abstract virtual 
            instance void  remove_Changed(class [mscorlib]System.EventHandler 'value') cil managed
    {
    } // end of method IChange::remove_Changed

    .event [mscorlib]System.EventHandler Changed
    {
      .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IChange::add_Changed(class [mscorlib]System.EventHandler)
      .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IChange::remove_Changed(class [mscorlib]System.EventHandler)
    } // end of event IChange::Changed
    .property instance int32 Property()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IChange::set_Property(int32)
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IChange::get_Property()
    } // end of property IChange::Property
  } // end of class IChange

  .class auto ansi nested private beforefieldinit Change
         extends [mscorlib]System.Object
         implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IChange
  {
    .field private class [mscorlib]System.EventHandler Changed
    .field private int32 '<ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.Property>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method private hidebysig newslot specialname virtual final 
            instance int32  ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.get_Property() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IChange::get_Property
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Change::'<ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.Property>k__BackingField'
      IL_0006:  ret
    } // end of method Change::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.get_Property

    .method private hidebysig newslot specialname virtual final 
            instance void  ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.set_Property(int32 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IChange::set_Property
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Change::'<ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.Property>k__BackingField'
      IL_0007:  ret
    } // end of method Change::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.set_Property

    .method private hidebysig newslot specialname virtual final 
            instance void  ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.add_Changed(class [mscorlib]System.EventHandler 'value') cil managed
    {
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IChange::add_Changed
      // Code size       24 (0x18)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Change::Changed
      IL_0007:  ldarg.1
      IL_0008:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate,
                                                                                              class [mscorlib]System.Delegate)
      IL_000d:  castclass  [mscorlib]System.EventHandler
      IL_0012:  stfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Change::Changed
      IL_0017:  ret
    } // end of method Change::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.add_Changed

    .method private hidebysig newslot specialname virtual final 
            instance void  ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.remove_Changed(class [mscorlib]System.EventHandler 'value') cil managed
    {
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/IChange::remove_Changed
      // Code size       24 (0x18)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Change::Changed
      IL_0007:  ldarg.1
      IL_0008:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate,
                                                                                             class [mscorlib]System.Delegate)
      IL_000d:  castclass  [mscorlib]System.EventHandler
      IL_0012:  stfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Change::Changed
      IL_0017:  ret
    } // end of method Change::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.remove_Changed

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method Change::.ctor

    .event [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.Changed
    {
      .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Change::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.remove_Changed(class [mscorlib]System.EventHandler)
      .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Change::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.add_Changed(class [mscorlib]System.EventHandler)
    } // end of event Change::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.Changed
    .property instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.Property()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Change::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.get_Property()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents/Change::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.set_Property(int32)
    } // end of property Change::ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents.IChange.Property
  } // end of class Change

  .field private notserialized int32 someField
  .field private object issue1221
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

  .method private hidebysig specialname instance void 
          set_Issue1221(object 'value') cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::issue1221
    IL_0007:  ret
  } // end of method PropertiesAndEvents::set_Issue1221

  .method public hidebysig specialname instance object 
          get_Item() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ret
  } // end of method PropertiesAndEvents::get_Item

  .method public hidebysig specialname instance void 
          set_Item(object 'value') cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method PropertiesAndEvents::set_Item

  .method public hidebysig specialname instance int32 
          get_NotAnAutoProperty() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::someField
    IL_0006:  ret
  } // end of method PropertiesAndEvents::get_NotAnAutoProperty

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

  .method private hidebysig static void  '<.ctor>b__0'(object param0,
                                                       class [mscorlib]System.EventArgs param1) cil managed
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
    .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::add_CustomEvent(class [mscorlib]System.EventHandler)
    .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::remove_CustomEvent(class [mscorlib]System.EventHandler)
  } // end of event PropertiesAndEvents::CustomEvent
  .property instance int32 Value()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::get_Value()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::set_Value(int32)
  } // end of property PropertiesAndEvents::Value
  .property instance object Issue1221()
  {
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::set_Issue1221(object)
  } // end of property PropertiesAndEvents::Issue1221
  .property instance object Item()
  {
    .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::get_Item()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::set_Item(object)
  } // end of property PropertiesAndEvents::Item
  .property instance int32 NotAnAutoProperty()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents::get_NotAnAutoProperty()
  } // end of property PropertiesAndEvents::NotAnAutoProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.PropertiesAndEvents


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
