// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 11:0:0:0
}
.assembly CompoundAssignmentOperatorEdgeCases
{
  .ver 1:0:0:0
}
.module CompoundAssignmentOperatorEdgeCases.dll
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY

.class public auto ansi beforefieldinit CompoundTarget
       extends [System.Runtime]System.Object
{
  .field public int32 Value

  // A C# 14 user-defined compound assignment operator.
  .method public hidebysig specialname instance void
          op_AdditionAssignment(int32 rhs) cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32 CompoundTarget::Value
    IL_0007:  ldarg.1
    IL_0008:  add
    IL_0009:  stfld      int32 CompoundTarget::Value
    IL_000e:  ret
  }

  // C++/CLI emits value-returning instance operators; C# has no syntax for those.
  .method public hidebysig specialname instance class CompoundTarget
          op_SubtractionAssignment(int32 rhs) cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  }

  // F# mangles "static member (*=)" to a static, value-returning op_MultiplicationAssignment.
  .method public hidebysig specialname static class CompoundTarget
          op_MultiplicationAssignment(class CompoundTarget lhs,
                                      int32 rhs) cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  }

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  ret
  }
}

.class public auto ansi beforefieldinit InaccessibleOperatorTarget
       extends [System.Runtime]System.Object
{
  .field public int32 Value

  // C# requires a user-defined operator to be public, so no C# expression can bind to
  // this one and it cannot be declared as an operator either.
  .method private hidebysig specialname instance void
          op_AdditionAssignment(int32 rhs) cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32 InaccessibleOperatorTarget::Value
    IL_0007:  ldarg.1
    IL_0008:  add
    IL_0009:  stfld      int32 InaccessibleOperatorTarget::Value
    IL_000e:  ret
  }

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  ret
  }
}

.class interface public abstract auto ansi beforefieldinit ICompound
{
  .method public hidebysig newslot specialname abstract virtual
          instance void  op_AdditionAssignment(int32 rhs) cil managed
  {
  }
}

.class interface public abstract auto ansi beforefieldinit IOther
{
  .method public hidebysig newslot abstract virtual
          instance void  M() cil managed
  {
  }
}

.class public auto ansi beforefieldinit DerivedCompoundTarget
       extends CompoundTarget
{
  .method public hidebysig instance void  CallBaseOperator(int32 n) cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void CompoundTarget::op_AdditionAssignment(int32)
    IL_0007:  ret
  }

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void CompoundTarget::.ctor()
    IL_0006:  ret
  }
}

.class public auto ansi beforefieldinit ShadowTarget
       extends [System.Runtime]System.Object
{
  .field public int32 Value

  // A static increment operator on a type that also declares the C# 14 instance one.
  .method public hidebysig specialname static class ShadowTarget
          op_Increment(class ShadowTarget x) cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  }

  .method public hidebysig specialname instance void
          op_IncrementAssignment() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32 ShadowTarget::Value
    IL_0007:  ldc.i4.1
    IL_0008:  add
    IL_0009:  stfld      int32 ShadowTarget::Value
    IL_000e:  ret
  }

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  ret
  }
}

.class public auto ansi beforefieldinit EdgeCases
       extends [System.Runtime]System.Object
{
  .field public static int32 Sink
  .field public static class ShadowTarget Shared

  // The post-increment shape legacy csc emits: the old value goes to a temp that nothing reads,
  // so the increment is written as a statement, where C# considers the instance operator first.
  .method public hidebysig static void  StatementLevelPostIncrement() cil managed
  {
    .maxstack  2
    .locals init (class ShadowTarget V_0)
    IL_0000:  ldsfld     class ShadowTarget EdgeCases::Shared
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  call       class ShadowTarget ShadowTarget::op_Increment(class ShadowTarget)
    IL_000c:  stsfld     class ShadowTarget EdgeCases::Shared
    IL_0011:  ret
  }

  // A pre-increment whose result is used: a prefix increment would bind the instance operator,
  // so the increment has to become a statement of its own before the uses of the new value.
  .method public hidebysig static void  ValueUsedPreIncrement() cil managed
  {
    .maxstack  2
    IL_0000:  ldsfld     class ShadowTarget EdgeCases::Shared
    IL_0005:  call       class ShadowTarget ShadowTarget::op_Increment(class ShadowTarget)
    IL_000a:  dup
    IL_000b:  stsfld     class ShadowTarget EdgeCases::Shared
    IL_0010:  call       void EdgeCases::UseValue(class ShadowTarget)
    IL_0015:  ret
  }

  .method public hidebysig static void  UseValue(class ShadowTarget t) cil managed
  {
    .maxstack  8
    IL_0000:  ret
  }

  // The other legacy post-increment shape, storing the old value inside the operator call: it is
  // written as a statement too where nothing reads that value.
  .method public hidebysig static void  InlineStorePostIncrement() cil managed
  {
    .maxstack  3
    .locals init (class ShadowTarget V_0)
    IL_0000:  ldsfld     class ShadowTarget EdgeCases::Shared
    IL_0005:  dup
    IL_0006:  stloc.0
    IL_0007:  call       class ShadowTarget ShadowTarget::op_Increment(class ShadowTarget)
    IL_000c:  stsfld     class ShadowTarget EdgeCases::Shared
    IL_0011:  ret
  }

  // An array element is a variable, and "ldelem.ref" leaves the reader with the stack type
  // rather than with the element type.
  .method public hidebysig static void  ArrayElementIncrement(class ShadowTarget[] arr) cil managed
  {
    .maxstack  3
    .locals init (class ShadowTarget V_0)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  ldc.i4.0
    IL_0003:  ldelem.ref
    IL_0004:  call       class ShadowTarget ShadowTarget::op_Increment(class ShadowTarget)
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  ldloc.0
    IL_000c:  stelem.ref
    IL_000d:  ret
  }

  .method public hidebysig static class CompoundTarget
          GetTarget() cil managed
  {
    .maxstack  8
    IL_0000:  newobj     instance void CompoundTarget::.ctor()
    IL_0005:  ret
  }

  // The receiver is not a variable, so it has to be given one before it can become the
  // target of "x += n".
  .method public hidebysig static void  NonVariableReceiver(int32 n) cil managed
  {
    .maxstack  8
    IL_0000:  call       class CompoundTarget EdgeCases::GetTarget()
    IL_0005:  ldarg.0
    IL_0006:  callvirt   instance void CompoundTarget::op_AdditionAssignment(int32)
    IL_000b:  ret
  }

  // T is not constrained to ICompound, so the cast selects the operator and has to stay; it
  // is the variable holding the cast value that becomes the assignment target.
  .method public hidebysig static void  UnconstrainedGenericReceiver<(IOther) T>(!!T x,
                                                                                 int32 n) cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  box        !!T
    IL_0006:  castclass  ICompound
    IL_000b:  ldarg.1
    IL_000c:  callvirt   instance void ICompound::op_AdditionAssignment(int32)
    IL_0011:  ret
  }

  // The operator is not public, so the call cannot become "x += n".
  .method public hidebysig static void  InaccessibleOperator(class InaccessibleOperatorTarget x,
                                                             int32 n) cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance void InaccessibleOperatorTarget::op_AdditionAssignment(int32)
    IL_0007:  ret
  }

  // The receiver's static type does not declare the operator, so overload resolution has no
  // candidate to pick.
  .method public hidebysig static void  MismatchedReceiverType(object o,
                                                               int32 n) cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance void CompoundTarget::op_AdditionAssignment(int32)
    IL_0007:  ret
  }

  // "x++" would bind the instance operator, so the call to the static one has to stay a call.
  .method public hidebysig static class ShadowTarget
          ShadowedIncrement(class ShadowTarget x) cil managed
  {
    .maxstack  8
    .locals init (class ShadowTarget V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       class ShadowTarget ShadowTarget::op_Increment(class ShadowTarget)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ret
  }

  .method public hidebysig static void  CallNonCSharpOperators(class CompoundTarget t,
                                                               int32 n) cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance class CompoundTarget CompoundTarget::op_SubtractionAssignment(int32)
    IL_0007:  pop
    IL_0008:  ldarg.0
    IL_0009:  ldarg.1
    IL_000a:  call       class CompoundTarget CompoundTarget::op_MultiplicationAssignment(class CompoundTarget,
                                                                                          int32)
    IL_000f:  pop
    IL_0010:  ret
  }

  // Not "++x": the static operator's covariant return lets the result be stored in a
  // derived-typed local, and on that type "++x" would bind the instance operator.
  .method public hidebysig static class CovariantDerived
          CovariantIncrement(class CovariantDerived d) cil managed
  {
    .maxstack  8
    .locals init (class CovariantDerived V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       class CovariantDerived CovariantBase::op_Increment(class CovariantBase)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldfld      int32 CovariantBase::Value
    IL_000d:  stsfld     int32 EdgeCases::Sink
    IL_0012:  ldloc.0
    IL_0013:  ret
  }

  // Not "x += n": the "in" overload is what the IL calls, but "x += n" binds the by-value
  // overload, so the call has to stay a call.
  .method public hidebysig static void  CallInOverload(class InOverloads x, int32 n) cil managed
  {
    .maxstack  8
    .locals init (int32 V_0)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  callvirt   instance void InOverloads::op_AdditionAssignment(int32&)
    IL_000a:  ret
  }

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  ret
  }
}

.class public auto ansi beforefieldinit BadArityTarget
       extends [System.Runtime]System.Object
{
  .field public int32 Value

  // C# 14 compound assignment operators take exactly one parameter and the instance
  // increment operators none; any other arity stays a plain method.
  .method public hidebysig specialname instance void
          op_AdditionAssignment(int32 a,
                                int32 b) cil managed
  {
    .maxstack  8
    IL_0000:  ret
  }

  .method public hidebysig specialname instance void
          op_SubtractionAssignment() cil managed
  {
    .maxstack  8
    IL_0000:  ret
  }

  .method public hidebysig specialname instance void
          op_IncrementAssignment(int32 a) cil managed
  {
    .maxstack  8
    IL_0000:  ret
  }

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  ret
  }
}

.class public auto ansi beforefieldinit CovariantBase
       extends [System.Runtime]System.Object
{
  .field public int32 Value

  .method public hidebysig specialname static class CovariantDerived
          op_Increment(class CovariantBase x) cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  castclass  CovariantDerived
    IL_0006:  ret
  }

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  ret
  }
}

.class public auto ansi beforefieldinit CovariantDerived
       extends CovariantBase
{
  .method public hidebysig specialname instance void
          op_IncrementAssignment() cil managed
  {
    .maxstack  8
    IL_0000:  ret
  }

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void CovariantBase::.ctor()
    IL_0006:  ret
  }
}

.class public auto ansi beforefieldinit BadParameterShapes
       extends [System.Runtime]System.Object
{
  // C# allows only value, "in" and "ref readonly" parameters on a compound assignment
  // operator; ref and params stay plain methods.
  .method public hidebysig specialname instance void
          op_SubtractionAssignment(int32& rhs) cil managed
  {
    .maxstack 8
    ret
  }

  .method public hidebysig specialname instance void
          op_MultiplicationAssignment(int32[] rhs) cil managed
  {
    .param [1]
    .custom instance void [System.Runtime]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 )
    .maxstack 8
    ret
  }

  .method public hidebysig specialname rtspecialname instance void .ctor() cil managed
  {
    .maxstack 8
    ldarg.0
    call instance void [System.Runtime]System.Object::.ctor()
    ret
  }
}

// a static class cannot contain operators
.class public abstract auto ansi sealed beforefieldinit StaticClassOps
       extends [System.Runtime]System.Object
{
  .method public hidebysig specialname instance void
          op_AdditionAssignment(int32 rhs) cil managed
  {
    .maxstack 8
    ret
  }
}

.class public auto ansi beforefieldinit InOverloads
       extends [System.Runtime]System.Object
{
  .field public int32 Value

  .method public hidebysig specialname instance void
          op_AdditionAssignment(int32 rhs) cil managed
  {
    .maxstack  8
    IL_0000:  ret
  }

  .method public hidebysig specialname instance void
          op_AdditionAssignment([in] int32& rhs) cil managed
  {
    .param [1]
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 )
    .maxstack  8
    IL_0000:  ret
  }

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  ret
  }
}
