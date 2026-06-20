// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly IssueIEnumeratorPatchWrapper
{
  .ver 1:0:0:0
}
.module IssueIEnumeratorPatchWrapper.dll

.namespace IFix.Core
{
  .class public auto ansi beforefieldinit XIDAttribute
         extends [mscorlib]System.Attribute
  {
    .field private int32 id
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(int32 id) cil managed
    {
      .maxstack  8
      ldarg.0
      call       instance void [mscorlib]System.Attribute::.ctor()
      ldarg.0
      ldarg.1
      stfld      int32 IFix.Core.XIDAttribute::id
      ret
    }
  }
}

.namespace IFix
{
  .class public auto ansi beforefieldinit WrappersManagerImpl
         extends [mscorlib]System.Object
  {
    .method public static hidebysig bool  IsPatched(int32 id) cil managed
    {
      .maxstack  8
      ldc.i4.0
      ret
    }

    .method public static hidebysig class IFix.ILFixDynamicMethodWrapper  GetPatch(int32 id) cil managed
    {
      .maxstack  8
      ldnull
      ret
    }

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .maxstack  8
      ldarg.0
      call       instance void [mscorlib]System.Object::.ctor()
      ret
    }
  }

  .class public auto ansi beforefieldinit ILFixDynamicMethodWrapper
         extends [mscorlib]System.Object
  {
    .method public hidebysig instance class [mscorlib]System.Collections.IEnumerator 
            __Gen_Wrap_44(object inst) cil managed
    {
      .maxstack  8
      ldnull
      ret
    }

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .maxstack  8
      ldarg.0
      call       instance void [mscorlib]System.Object::.ctor()
      ret
    }
  }
}

.namespace XUtliPoolLib
{
  .class public auto ansi beforefieldinit XVersionNewData
         extends [mscorlib]System.Object
  {
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .maxstack  8
      ldarg.0
      call       instance void [mscorlib]System.Object::.ctor()
      ret
    }
  }

  .class interface public abstract auto ansi IXVideo
  {
    .method public newslot abstract virtual instance void  Play(bool A_1) cil managed {}
    .method public newslot abstract virtual instance bool  get_isPlaying() cil managed {}
    .property instance bool isPlaying()
    {
      .get instance bool XUtliPoolLib.IXVideo::get_isPlaying()
    }
  }
}

.namespace XUpdater
{
  .class public auto ansi beforefieldinit XUpdater
         extends [mscorlib]System.Object
  {
    .field public class XUtliPoolLib.XVersionNewData _server
    .field public string _version
    .field public class XUtliPoolLib.XVersionNewData _client
    .field public class XUtliPoolLib.XVersionNewData _buildin
    .field public bool _need_play_cg
    .field public class XUtliPoolLib.IXVideo _video

    .class nested private auto ansi sealed beforefieldinit '<Finish>d__82'
           extends [mscorlib]System.Object
           implements class [mscorlib]System.Collections.Generic.IEnumerator`1<object>,
                      [mscorlib]System.IDisposable,
                      [mscorlib]System.Collections.IEnumerator
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .field private int32 '<>1__state'
      .field private object '<>2__current'
      .field public class XUpdater.XUpdater '<>4__this'

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor(int32 '<>1__state') cil managed
      {
        .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [IssueIEnumeratorPatchWrapper]IFix.Core.XIDAttribute::.ctor(int32) = ( 01 00 76 01 00 00 00 00 )
        .maxstack  8
        ldarg.0
        call       instance void [mscorlib]System.Object::.ctor()
        ldarg.0
        ldarg.1
        stfld      int32 XUpdater.XUpdater/'<Finish>d__82'::'<>1__state'
        ret
      }

      .method private hidebysig newslot virtual final 
              instance void  'System.IDisposable.Dispose'() cil managed
      {
        .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [IssueIEnumeratorPatchWrapper]IFix.Core.XIDAttribute::.ctor(int32) = ( 01 00 77 01 00 00 00 00 )
        .override [mscorlib]System.IDisposable::Dispose
        ret
      }

      .method private hidebysig newslot virtual final 
              instance bool  MoveNext() cil managed
      {
        .custom instance void [IssueIEnumeratorPatchWrapper]IFix.Core.XIDAttribute::.ctor(int32) = ( 01 00 78 01 00 00 00 00 )
        .override [mscorlib]System.Collections.IEnumerator::MoveNext
        .maxstack  2
        .locals init (int32 V_0, class XUpdater.XUpdater V_1)
        ldarg.0
        ldfld      int32 XUpdater.XUpdater/'<Finish>d__82'::'<>1__state'
        stloc.0
        ldarg.0
        ldfld      class XUpdater.XUpdater XUpdater.XUpdater/'<Finish>d__82'::'<>4__this'
        stloc.1
        ldloc.0
        switch     (IL_0022, IL_0083, IL_009c)

        ldc.i4.0
        ret

        IL_0022:
        ldarg.0
        ldc.i4.m1
        stfld      int32 XUpdater.XUpdater/'<Finish>d__82'::'<>1__state'
        ldloc.1
        ldfld      class XUtliPoolLib.XVersionNewData XUpdater.XUpdater::_server
        brfalse.s  IL_0042

        ldloc.1
        ldloc.1
        ldfld      class XUtliPoolLib.XVersionNewData XUpdater.XUpdater::_server
        callvirt   instance string [mscorlib]System.Object::ToString()
        stfld      string XUpdater.XUpdater::_version

        IL_0042:
        ldloc.1
        ldnull
        stfld      class XUtliPoolLib.XVersionNewData XUpdater.XUpdater::_server
        ldloc.1
        ldnull
        stfld      class XUtliPoolLib.XVersionNewData XUpdater.XUpdater::_client
        ldloc.1
        ldnull
        stfld      class XUtliPoolLib.XVersionNewData XUpdater.XUpdater::_buildin
        ldloc.1
        ldfld      bool XUpdater.XUpdater::_need_play_cg
        brfalse.s  IL_00b0

        ldloc.1
        ldfld      class XUtliPoolLib.IXVideo XUpdater.XUpdater::_video
        brfalse.s  IL_00b0

        ldloc.1
        ldfld      class XUtliPoolLib.IXVideo XUpdater.XUpdater::_video
        ldc.i4.0
        callvirt   instance void XUtliPoolLib.IXVideo::Play(bool)
        ldarg.0
        ldnull
        stfld      object XUpdater.XUpdater/'<Finish>d__82'::'<>2__current'
        ldarg.0
        ldc.i4.1
        stfld      int32 XUpdater.XUpdater/'<Finish>d__82'::'<>1__state'
        ldc.i4.1
        ret

        IL_0083:
        ldarg.0
        ldc.i4.m1
        stfld      int32 XUpdater.XUpdater/'<Finish>d__82'::'<>1__state'
        br.s       IL_00a3

        IL_008c:
        ldarg.0
        ldnull
        stfld      object XUpdater.XUpdater/'<Finish>d__82'::'<>2__current'
        ldarg.0
        ldc.i4.2
        stfld      int32 XUpdater.XUpdater/'<Finish>d__82'::'<>1__state'
        ldc.i4.1
        ret

        IL_009c:
        ldarg.0
        ldc.i4.m1
        stfld      int32 XUpdater.XUpdater/'<Finish>d__82'::'<>1__state'

        IL_00a3:
        ldloc.1
        ldfld      class XUtliPoolLib.IXVideo XUpdater.XUpdater::_video
        callvirt   instance bool XUtliPoolLib.IXVideo::get_isPlaying()
        brtrue.s   IL_008c

        IL_00b0:
        ldc.i4.0
        ret
      }

      .method private hidebysig specialname rtspecialname newslot virtual final 
              instance object  'System.Collections.Generic.IEnumerator<System.Object>.get_Current'() cil managed
      {
        .custom instance void [IssueIEnumeratorPatchWrapper]IFix.Core.XIDAttribute::.ctor(int32) = ( 01 00 79 01 00 00 00 00 )
        .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 )
        .override class [mscorlib]System.Collections.Generic.IEnumerator`1<object>::get_Current
        ldarg.0
        ldfld      object XUpdater.XUpdater/'<Finish>d__82'::'<>2__current'
        ret
      }

      .method private hidebysig newslot virtual final 
              instance void  'System.Collections.IEnumerator.Reset'() cil managed
      {
        .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [IssueIEnumeratorPatchWrapper]IFix.Core.XIDAttribute::.ctor(int32) = ( 01 00 7a 01 00 00 00 00 )
        .override [mscorlib]System.Collections.IEnumerator::Reset
        newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
        throw
      }

      .method private hidebysig specialname rtspecialname newslot virtual final 
              instance object  'System.Collections.IEnumerator.get_Current'() cil managed
      {
        .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [IssueIEnumeratorPatchWrapper]IFix.Core.XIDAttribute::.ctor(int32) = ( 01 00 7b 01 00 00 00 00 )
        .override [mscorlib]System.Collections.IEnumerator::get_Current
        ldarg.0
        ldfld      object XUpdater.XUpdater/'<Finish>d__82'::'<>2__current'
        ret
      }

      .property instance object 'System.Collections.Generic.IEnumerator<System.Object>.Current'()
      {
        .get instance object XUpdater.XUpdater/'<Finish>d__82'::'System.Collections.Generic.IEnumerator<System.Object>.get_Current'()
      }
      .property instance object 'System.Collections.IEnumerator.Current'()
      {
        .get instance object XUpdater.XUpdater/'<Finish>d__82'::'System.Collections.IEnumerator.get_Current'()
      }
    } // end of class '<Finish>d__82'

    .method private hidebysig 
            instance class [mscorlib]System.Collections.IEnumerator  Finish() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = (
        01 00 1f 58 55 70 64 61 74 65 72 2e 58 55 70 64
        61 74 65 72 2b 3c 46 69 6e 69 73 68 3e 64 5f 5f
        38 32 00 00
      )
      .custom instance void [IssueIEnumeratorPatchWrapper]IFix.Core.XIDAttribute::.ctor(int32) = ( 01 00 75 01 00 00 00 00 )
      .maxstack  8
      ldc.i4     373
      call       bool IFix.WrappersManagerImpl::IsPatched(int32)
      brfalse.s  IL_0020

      ldc.i4     373
      call       class IFix.ILFixDynamicMethodWrapper IFix.WrappersManagerImpl::GetPatch(int32)
      ldarg.0
      callvirt   instance class [mscorlib]System.Collections.IEnumerator IFix.ILFixDynamicMethodWrapper::__Gen_Wrap_44(object)
      ret

      IL_0020:
      ldc.i4.0
      newobj     instance void XUpdater.XUpdater/'<Finish>d__82'::.ctor(int32)
      dup
      ldarg.0
      stfld      class XUpdater.XUpdater XUpdater.XUpdater/'<Finish>d__82'::'<>4__this'
      ret
    }

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .maxstack  8
      ldarg.0
      call       instance void [mscorlib]System.Object::.ctor()
      ret
    }
  }
}
