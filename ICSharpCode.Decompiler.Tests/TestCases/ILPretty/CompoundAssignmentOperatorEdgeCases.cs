using System.Runtime.CompilerServices;

public class BadArityTarget
{
	public int Value;

	// C# 14 compound assignment operators take exactly one parameter and the instance
	// increment operators none; any other arity stays a plain method.
	[SpecialName]
	public void op_AdditionAssignment(int a, int b)
	{
	}

	[SpecialName]
	public void op_SubtractionAssignment()
	{
	}

	[SpecialName]
	public void op_IncrementAssignment(int a)
	{
	}
}
public class BadParameterShapes
{
	// C# allows only value, "in" and "ref readonly" parameters on a compound assignment
	// operator; ref and params stay plain methods.
	[SpecialName]
	public void op_SubtractionAssignment(ref int rhs)
	{
	}

	[SpecialName]
	public void op_MultiplicationAssignment(params int[] rhs)
	{
	}
}
public class CompoundTarget
{
	public int Value;

	public void operator +=(int rhs)
	{
		Value += rhs;
	}

	// Not "operator -=": C# 14 only recognizes void-returning instance methods as compound
	// assignment operators, so a value-returning one stays a plain method.
	[SpecialName]
	public CompoundTarget op_SubtractionAssignment(int rhs)
	{
		return this;
	}

	[SpecialName]
	public static CompoundTarget op_MultiplicationAssignment(CompoundTarget lhs, int rhs)
	{
		return lhs;
	}
}
public class CovariantBase
{
	public int Value;

	public static CovariantDerived operator ++(CovariantBase x)
	{
		return (CovariantDerived)x;
	}
}
public class CovariantDerived : CovariantBase
{
	public void operator ++()
	{
	}
}
public class DerivedCompoundTarget : CompoundTarget
{
	public void CallBaseOperator(int n)
	{
		// "this" cannot be the target of "x op= y", so the receiver keeps a copy of it;
		// the copy binds the same inherited operator the IL calls.
		DerivedCompoundTarget derivedCompoundTarget = this;
		derivedCompoundTarget += n;
	}
}
public class EdgeCases
{
	public static int Sink;

	public static ShadowTarget Shared;

	public ShadowTarget mutableField;

	// A statement-level "x++" considers the instance operator first; the only form that always
	// binds the static operator is a postfix increment whose result is used, so the result goes
	// to a discard.
	public static void StatementLevelPostIncrement()
	{
		_ = Shared++;
	}

	public ShadowTarget ReassignField()
	{
		mutableField = new ShadowTarget();
		return null;
	}

	public static void UseTwo(object o, ShadowTarget t)
	{
	}

	// The receiver read must not be hoisted above side effects already pending on the
	// expression stack: ReassignField() replaces the field the increment then applies to.
	public void IncrementAfterPendingSideEffect()
	{
		ShadowTarget o = ReassignField();
		mutableField++;
		UseTwo(o, mutableField);
	}

	// A pre-increment whose result is used prefers the instance operator too, so the increment
	// becomes a statement of its own before the uses of the new value.
	public static void ValueUsedPreIncrement()
	{
		ShadowTarget shared = Shared;
		_ = shared++;
		UseValue(Shared = shared);
	}

	public static void UseValue(ShadowTarget t)
	{
	}

	// The same holds for the shape that stores the old value inside the operator call.
	public static void InlineStorePostIncrement()
	{
		_ = Shared++;
	}

	// An array element is a variable, so a statement-level "++arr[0]" would bind the instance
	// operator too.
	public static void ArrayElementIncrement(ShadowTarget[] arr)
	{
		ShadowTarget shadowTarget = arr[0];
		_ = shadowTarget++;
		arr[0] = shadowTarget;
	}

	public static CompoundTarget GetTarget()
	{
		return new CompoundTarget();
	}

	public static void NonVariableReceiver(int n)
	{
		CompoundTarget target = GetTarget();
		target += n;
	}

	public static void UnconstrainedGenericReceiver<T>(T x, int n) where T : IOther
	{
		ICompound compound = (ICompound)(object)x;
		compound += n;
	}

	// Not "x += n": C# requires a user-defined operator to be public, so an operator that
	// is not cannot be bound by the operator form and the call has to stay a call.
	public static void InaccessibleOperator(InaccessibleOperatorTarget x, int n)
	{
		x.op_AdditionAssignment(n);
	}

	public static void MismatchedReceiverType(object o, int n)
	{
		((CompoundTarget)o).op_AdditionAssignment(n);
	}

	// A prefix increment prefers the instance "operator ++()" even where its result is used, so
	// the increment is a statement with a discarded postfix result; the copy keeps the
	// parameter unchanged, as in the IL.
	public static ShadowTarget ShadowedIncrement(ShadowTarget x)
	{
		ShadowTarget result = x;
		_ = result++;
		return result;
	}

	public static void CallNonCSharpOperators(CompoundTarget t, int n)
	{
		t.op_SubtractionAssignment(n);
		CompoundTarget.op_MultiplicationAssignment(t, n);
	}

	// The static operator's covariant return lets the result be stored in a derived-typed
	// local, and on that type "++x" would bind the instance operator; a postfix increment with
	// its result discarded binds the static one.
	public static CovariantDerived CovariantIncrement(CovariantDerived d)
	{
		CovariantDerived covariantDerived = d;
		_ = covariantDerived++;
		Sink = covariantDerived.Value;
		return covariantDerived;
	}

	// Not "x += n": the "in" overload is what the IL calls, but "x += n" binds the by-value
	// overload, so the call has to stay a call.
	public static void CallInOverload(InOverloads x, int n)
	{
		int rhs = n;
		x.op_AdditionAssignment(in rhs);
	}
}
public interface ICompound
{
	void operator +=(int rhs);
}
public class InaccessibleOperatorTarget
{
	public int Value;

	// Not "operator +=": a non-public operator is not declarable as one in C#.
	private void op_AdditionAssignment(int rhs)
	{
		Value += rhs;
	}
}
public class InOverloads
{
	public int Value;

	public void operator +=(int rhs)
	{
	}

	public void operator +=(in int rhs)
	{
	}
}
public interface IOther
{
	void M();
}
public class ShadowTarget
{
	public int Value;

	public static ShadowTarget operator ++(ShadowTarget x)
	{
		return x;
	}

	public void operator ++()
	{
		Value++;
	}
}
public static class StaticClassOps
{
	// a static class cannot contain operators
	[SpecialName]
	public void op_AdditionAssignment(int rhs)
	{
	}
}
