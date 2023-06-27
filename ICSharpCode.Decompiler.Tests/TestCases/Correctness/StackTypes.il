.assembly extern mscorlib
{
  .publickeytoken = ( b7 7a 5c 56 19 34 e0 89 )
  .ver 4:0:0:0
}

.assembly 'StackTypes'
{
  .ver 0:0:0:0
}

.module StackTypes.exe
.corflags 0x00000001 // ILOnly

.class private auto ansi abstract sealed beforefieldinit Program
extends [mscorlib]System.Object
{
  .method public hidebysig static void Main (string[] args) cil managed 
  {
    .maxstack 8
    .entrypoint

    call void Program::InlineAssignByte()
    call void Program::Int32OrNativeTests()
    call void Program::ByRefInstanceCallWithTypeMismatchTests()

    ret
  } // end of method Main

  .method public static void InlineAssignByte()
  {
    .locals init (
      int8 local
    )
    ldstr "InlineAssignByte: WriteLine(local = {0})"
    ldc.i4 300
    dup
    br pointless // this pointless branch is a workaround for https://github.com/dotnet/coreclr/issues/14784
    // it doesn't have any effect on ILSpy as TransformAssignment runs after control-flow reconstruction
pointless:
    // This assignment cannot be turned into a C# inline assignment, because doing so would truncate to 8 bits.
    stloc.0
    box int32
    call void [mscorlib]System.Console::WriteLine(string, object)
    ldstr "InlineAssignByte: local is {0}"
    ldloc.0
    box int32
    call void [mscorlib]System.Console::WriteLine(string, object)
    ret
  }

  .method public static void Int32OrNativeTests()
  {
    ldstr "Int32OrNative(0x7fffffff, false) = {0}"
    ldc.i4 0x7fffffff
    ldc.i4 0
    call native int Program::Int32OrNative(int32, bool)
    box native int
    call void [mscorlib]System.Console::WriteLine(string, object)

    ldstr "Int32OrNative(0x7fffffff, true) = {0}"
    ldc.i4 0x7fffffff
    ldc.i4 1
    call native int Program::Int32OrNative(int32, bool)
    box native int
    call void [mscorlib]System.Console::WriteLine(string, object)

    ldstr "Int32OrNative(-1, false) = {0}"
    ldc.i4.m1
    ldc.i4 0
    call native int Program::Int32OrNative(int32, bool)
    box native int
    call void [mscorlib]System.Console::WriteLine(string, object)

    ldstr "Int32OrNative(-1, true) = {0}"
    ldc.i4.m1
    ldc.i4 1
    call native int Program::Int32OrNative(int32, bool)
    box native int
    call void [mscorlib]System.Console::WriteLine(string, object)

    ldstr "Int32OrNativeReordered(0x7fffffff, false) = {0}"
    ldc.i4 0x7fffffff
    ldc.i4 0
    call native int Program::Int32OrNativeReordered(int32, bool)
    box native int
    call void [mscorlib]System.Console::WriteLine(string, object)

    ldstr "Int32OrNativeReordered(0x7fffffff, true) = {0}"
    ldc.i4 0x7fffffff
    ldc.i4 1
    call native int Program::Int32OrNativeReordered(int32, bool)
    box native int
    call void [mscorlib]System.Console::WriteLine(string, object)

    ldstr "Int32OrNativeReordered(-1, false) = {0}"
    ldc.i4.m1
    ldc.i4 0
    call native int Program::Int32OrNativeReordered(int32, bool)
    box native int
    call void [mscorlib]System.Console::WriteLine(string, object)

    ldstr "Int32OrNativeReordered(-1, true) = {0}"
    ldc.i4.m1
    ldc.i4 1
    call native int Program::Int32OrNativeReordered(int32, bool)
    box native int
    call void [mscorlib]System.Console::WriteLine(string, object)

    ldstr "Int32OrNativeLoopStyle(0x7fffffff):"
    call void [mscorlib]System.Console::WriteLine(string)
    ldc.i4 0x7fffffff
    call void Program::Int32OrNativeLoopStyle(int32)

    ldstr "Int32OrNativeLoopStyle(-1):"
    call void [mscorlib]System.Console::WriteLine(string)
    ldc.i4.m1
    call void Program::Int32OrNativeLoopStyle(int32)

    ldstr "Int32OrNativeDeadCode(0x7fffffff) = {0}"
    ldc.i4 0x7fffffff
    call native int Program::Int32OrNativeDeadCode(int32)
    box native int
    call void [mscorlib]System.Console::WriteLine(string, object)

    ldstr "Int32OrNativeDeadCode(-1) = {0}"
    ldc.i4.m1
    call native int Program::Int32OrNativeDeadCode(int32)
    box native int
    call void [mscorlib]System.Console::WriteLine(string, object)

    ldc.i4 0x7fffffff
    call void Program::RunInt32OrNativeMultiUse(int32)
    ldc.i4.m1
    call void Program::RunInt32OrNativeMultiUse(int32)

    ret
  }
  .method public static native int Int32OrNative(int32 val, bool use_native)
  {
    ldarg.1
    brtrue use_native_int
  use_i4:
    ldarg.0
    br after_if
  use_native_int:
    ldarg.0
    conv.u
    br after_if
  after_if:
    ldc.i4.1
    add
    ret
  }
  
  .method public static native int Int32OrNativeReordered(int32 val, bool use_native)
  {
    // The spec is ambiguous whether the addition will be in 32-bits or native size.
    // The microsoft runtime picks native size.
    ldarg.1
    brtrue use_native_int
  use_i4:
    ldarg.0
    br after_if
  after_if:
    ldc.i4.1
    add
    ret
  use_native_int:
    ldarg.0
    conv.u
    br after_if
  }
  
  .method public static void Int32OrNativeLoopStyle(int32 val)
  {
    // The spec is ambiguous whether the addition will be in 32-bits or native size.
    // The microsoft runtime picks native size.
    .locals init (
      int32 i
    )
    ldarg.0
  loop:
    ldc.i4.1
    add
    call void Program::Print(native int)
    ldloc.0
    brtrue end

    ldc.i4.1
    stloc.0
    ldarg.0
    conv.u
    br loop
  end:
    ret
  }

  .method public static native int Int32OrNativeDeadCode(int32 val)
  {
    // The spec is ambiguous whether the addition will be in 32-bits or native size.
    // The microsoft runtime picks 32-bits.
  use_i4:
    ldarg.0
    br after_if
  after_if:
    ldc.i4.1
    add
    ret
  use_native_int: // dead code
    ldarg.0
    conv.u
    br after_if
  }

  .method public static void RunInt32OrNativeMultiUse(int32 val)
  {
    ldstr "RunInt32OrNativeMultiUse({0}, push_i: false, use2: false) = {1}"
    ldarg val
    box int32
    ldarg val
    ldc.i4 0 // push_i
    ldc.i4 0 // use2
    call native int Program::Int32OrNativeMultiUse(int32 val, bool push_i, bool use2)
    box native int
    call void [mscorlib]System.Console::WriteLine(string, object, object)

    ldstr "RunInt32OrNativeMultiUse({0}, push_i: false, use2: true) = {1}"
    ldarg val
    box int32
    ldarg val
    ldc.i4 0 // push_i
    ldc.i4 1 // use2
    call native int Program::Int32OrNativeMultiUse(int32 val, bool push_i, bool use2)
    box native int
    call void [mscorlib]System.Console::WriteLine(string, object, object)

    ldstr "RunInt32OrNativeMultiUse({0}, push_i: true, use2: false) = {1}"
    ldarg val
    box int32
    ldarg val
    ldc.i4 1 // push_i
    ldc.i4 0 // use2
    call native int Program::Int32OrNativeMultiUse(int32 val, bool push_i, bool use2)
    box native int
    call void [mscorlib]System.Console::WriteLine(string, object, object)

    ldstr "RunInt32OrNativeMultiUse({0}, push_i: true, use2: true) = {1}"
    ldarg val
    box int32
    ldarg val
    ldc.i4 1 // push_i
    ldc.i4 1 // use2
    call native int Program::Int32OrNativeMultiUse(int32 val, bool push_i, bool use2)
    box native int
    call void [mscorlib]System.Console::WriteLine(string, object, object)
    ret
  }

  .method public static native int Int32OrNativeMultiUse(int32 val, bool push_i, bool use2)
  {
    ldarg.1
    brtrue push_i
    br push_i4
  push_i4:
    ldarg.0
    ldarg.2
    brtrue use2
    br use1
  push_i:
    ldarg.0
    conv.u
    br use1
  use1:
    ldc.i4.1
    add
    ret
  use2:
    ldc.i4.2
    add
    ret
  }

  .method public static void RefOrNull(uint8& r, bool use_null)
  {
    // Not quite valid IL due to ref<->I mismatch.
    // https://github.com/icsharpcode/ILSpy/issues/981
    ldarg.1
    brtrue push_null
    ldarg.0
    br done
  push_null:
    ldc.i4.0
    conv.u
  done:
    call void Program::UseRef(uint8&)
    ret
  }

  .method public static void UseRef(uint8& r)
  {
    ret
  }

  .method public static void Print(native int val)
  {
    ldarg.0
    box native int
    call void [mscorlib]System.Console::WriteLine(object)
    ret
  }

  .method public static void ByRefInstanceCallWithTypeMismatchTests()
  {
    ldstr "ByRefInstanceCallWithTypeMismatch(0) = {0}"
    ldc.i4.0
    call string Program::ByRefInstanceCallWithTypeMismatch(int32)
    call void [mscorlib]System.Console::WriteLine(string, object)

    ldstr "ByRefInstanceCallWithTypeMismatch(1) = {0}"
    ldc.i4.1
    call string Program::ByRefInstanceCallWithTypeMismatch(int32)
    call void [mscorlib]System.Console::WriteLine(string, object)

    ldstr "Issue1333() = {0}"
    call string Program::Issue1333()
    call void [mscorlib]System.Console::WriteLine(string, object)

    ret
  }
  
  .method public hidebysig static string ByRefInstanceCallWithTypeMismatch(int32 val) cil managed
  {
    ldarga val
    constrained. [mscorlib]System.Boolean
    callvirt instance string [mscorlib]System.Object::ToString()
    ret
  }
  
  .method public hidebysig static string Issue1333() cil managed
  {
    .locals (bool)
    ldc.i4.0
    stloc.0
    ldloca.s 0
    constrained. [mscorlib]System.Boolean
    callvirt instance string [mscorlib]System.Object::ToString()
    ret
  }
}
