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

	// Not "x++": the type also declares the C# 14 instance "operator ++()", which "x++" binds
	// in preference to the static one this call names.
	public static ShadowTarget ShadowedIncrement(ShadowTarget x)
	{
		return ShadowTarget.op_Increment(x);
	}

	public static void CallNonCSharpOperators(CompoundTarget t, int n)
	{
		t.op_SubtractionAssignment(n);
		CompoundTarget.op_MultiplicationAssignment(t, n);
	}

	// Not "++x": the static operator's covariant return lets the result be stored in a
	// derived-typed local, and on that type "++x" would bind the instance operator.
	public static CovariantDerived CovariantIncrement(CovariantDerived d)
	{
		CovariantDerived covariantDerived = CovariantBase.op_Increment(d);
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
