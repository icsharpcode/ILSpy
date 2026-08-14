// Reproducer for issue #3729:
// Decompilation of a struct constructor call where the struct's defining
// assembly (Library1) cannot be resolved at decompile time.
// Mirrors what csc emits for `new Library1.MyStruct(4)` followed by
// string concat with the struct value.

.assembly extern Library1
{
}
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver 4:0:0:0
}
.assembly extern System.Console
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver 4:0:0:0
}
.assembly Issue3729
{
  .ver 1:0:0:0
}
.module Issue3729.dll

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3729
    extends [System.Runtime]System.Object
{
    .field private valuetype [Library1]Library1.MyStruct structField

    // single-int-arg struct ctor with subsequent constrained.callvirt ToString()
    .method public hidebysig
        instance void TestSingleArgCtor () cil managed
    {
        .maxstack 2
        .locals init (
            [0] valuetype [Library1]Library1.MyStruct
        )

        IL_0000: ldloca.s 0
        IL_0002: ldc.i4.4
        IL_0003: call instance void [Library1]Library1.MyStruct::.ctor(int32)
        IL_0008: ldstr "Test new struct: "
        IL_000d: ldloca.s 0
        IL_000f: constrained. [Library1]Library1.MyStruct
        IL_0015: callvirt instance string [System.Runtime]System.Object::ToString()
        IL_001a: call string [System.Runtime]System.String::Concat(string, string)
        IL_001f: call void [System.Console]System.Console::WriteLine(string)
        IL_0024: ret
    }

    // explicit parameterless struct ctor (C# 10+ feature in source)
    // Single-argument ctor: the receiver sits directly below the argument.
    .method public hidebysig
        instance void TestParameterlessCtor () cil managed
    {
        .maxstack 1
        .locals init (
            [0] valuetype [Library1]Library1.MyEmptyStruct
        )

        IL_0000: ldloca.s 0
        IL_0002: call instance void [Library1]Library1.MyEmptyStruct::.ctor()
        IL_0007: ldloc.0
        IL_0008: box [Library1]Library1.MyEmptyStruct
        IL_000d: call void [System.Console]System.Console::WriteLine(object)
        IL_0012: ret
    }

    // Multi-argument ctor with mixed parameter types: the receiver sits below three arguments
    .method public hidebysig
        instance void TestMultiArgCtor () cil managed
    {
        .maxstack 5
        .locals init (
            [0] valuetype [Library1]Library1.MyBigStruct
        )

        IL_0000: ldloca.s 0
        IL_0002: ldc.i4.1
        IL_0003: ldstr "hello"
        IL_0008: ldc.r8 3.14
        IL_0011: call instance void [Library1]Library1.MyBigStruct::.ctor(int32, string, float64)
        IL_0016: ldloc.0
        IL_0017: box [Library1]Library1.MyBigStruct
        IL_001c: call void [System.Console]System.Console::WriteLine(object)
        IL_0021: ret
    }

    // struct field init via `ldflda + call .ctor`. Receiver is the field
    // address (StackType.Ref) - same shape as `ldloca`, just sourced from
    // an instance field instead of a local.
    .method public hidebysig
        instance void TestFieldCtor () cil managed
    {
        .maxstack 2

        IL_0000: ldarg.0
        IL_0001: ldflda valuetype [Library1]Library1.MyStruct ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3729::structField
        IL_0006: ldc.i4.5
        IL_0007: call instance void [Library1]Library1.MyStruct::.ctor(int32)
        IL_000c: ret
    }

    // Unsafe code passes the target as a pointer, so the receiver is a native
    // pointer (StackType.I) rather than a managed reference.
    .method public hidebysig static
        void TestPointerCtor (
            void* ptr
        ) cil managed
    {
        .maxstack 2

        IL_0000: ldarg.0
        IL_0001: ldc.i4.6
        IL_0002: call instance void [Library1]Library1.MyStruct::.ctor(int32)
        IL_0007: ret
    }

    // struct array element init via `ldelema + call .ctor`. Receiver is the
    // element address (StackType.Ref) - again the same shape.
    .method public hidebysig
        instance void TestArrayElemCtor () cil managed
    {
        .maxstack 4
        .locals init (
            [0] valuetype [Library1]Library1.MyStruct[]
        )

        IL_0000: ldc.i4.1
        IL_0001: newarr [Library1]Library1.MyStruct
        IL_0006: stloc.0
        IL_0007: ldloc.0
        IL_0008: ldc.i4.0
        IL_0009: ldelema [Library1]Library1.MyStruct
        IL_000e: ldc.i4.7
        IL_000f: call instance void [Library1]Library1.MyStruct::.ctor(int32)
        IL_0014: ret
    }

    // generic struct instantiation `new MyGenericStruct<int>(4)`. Declaring
    // type at the call site is a ParameterizedType wrapping the unresolved
    // generic def - its Kind delegates to the underlying UnknownType.Kind,
    // so a constructed generic type is unresolved in the same way.
    .method public hidebysig
        instance void TestGenericStructCtor () cil managed
    {
        .maxstack 2
        .locals init (
            [0] valuetype [Library1]Library1.MyGenericStruct`1<int32>
        )

        IL_0000: ldloca.s 0
        IL_0002: ldc.i4.4
        IL_0003: call instance void valuetype [Library1]Library1.MyGenericStruct`1<int32>::.ctor(!0)
        IL_0008: ldloc.0
        IL_0009: box valuetype [Library1]Library1.MyGenericStruct`1<int32>
        IL_000e: call void [System.Console]System.Console::WriteLine(object)
        IL_0013: ret
    }

    // newobj on an unresolved reference type - uses OpCode.NewObj, a different
    // IL path that already produces correct C# (HandleConstructorCall in CallBuilder).
    // Included as a control case for the value-type ctor handling above.
    .method public hidebysig
        instance void TestRefTypeNewobj () cil managed
    {
        .maxstack 1
        .locals init (
            [0] class [Library1]Library1.MyClass
        )

        IL_0000: newobj instance void [Library1]Library1.MyClass::.ctor()
        IL_0005: stloc.0
        IL_0006: ldloc.0
        IL_0007: call void [System.Console]System.Console::WriteLine(object)
        IL_000c: ret
    }

    .method public hidebysig specialname rtspecialname
        instance void .ctor () cil managed
    {
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Object::.ctor()
        IL_0006: ret
    }
}

// Classes deriving from an unresolved base class. The synthesized .ctor
// calls `[Library1]Library1.MissingBase::.ctor(...)` with `this`
// (StackType.O) as the receiver - exercises the StackType.Ref guard on
// a reference-typed receiver, which has to keep falling through to the
// default call path so the C# decompiler can render it as a normal
// `: base(...)` constructor initializer.
.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3729_DerivedFromUnknown
    extends [Library1]Library1.MissingBase
{
    // parameterless base ctor - the implicit ctor would normally be elided.
    .method public hidebysig specialname rtspecialname
        instance void .ctor () cil managed
    {
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [Library1]Library1.MissingBase::.ctor()
        IL_0006: ret
    }
}

// Same as above but the base ctor takes an argument - receiver is at
// depth 1 (one parameter on the stack above it). Output should render
// the explicit `: base(<arg>)` initializer.
.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3729_DerivedFromUnknownWithArgs
    extends [Library1]Library1.MissingBase
{
    .method public hidebysig specialname rtspecialname
        instance void .ctor () cil managed
    {
        .maxstack 2

        IL_0000: ldarg.0
        IL_0001: ldc.i4.s 42
        IL_0003: call instance void [Library1]Library1.MissingBase::.ctor(int32)
        IL_0008: ret
    }
}
