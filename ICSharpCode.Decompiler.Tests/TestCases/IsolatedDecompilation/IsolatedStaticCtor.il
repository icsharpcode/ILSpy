#define CORE_ASSEMBLY "System.Runtime"

// Regression fixture for #3774. Static field initializers compile into the type's static
// constructor, and the decompiler normally moves them onto the field declarations. When a single
// .cctor is decompiled in isolation (as the "IL with C#" view does, via CSharpDecompiler.Decompile
// of one method handle) those declarations are absent from the partial syntax tree, so the
// assignments must stay in the constructor body instead of being dropped. The type is
// 'beforefieldinit' -- the configuration under which the decompiler treats the .cctor stores as
// movable field initializers, which is what triggered the dropped statement.

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver 4:0:0:0
}

.assembly IsolatedStaticCtor { }

.class public auto ansi beforefieldinit C
    extends [CORE_ASSEMBLY]System.Object
{
    .field public static initonly int32 Number
    .field public static initonly string Text

    .method private hidebysig specialname rtspecialname static
        void .cctor () cil managed
    {
        .maxstack 8

        ldc.i4.s 42
        stsfld int32 C::Number
        ldstr "hello"
        stsfld string C::Text
        ret
    }
}
