using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	// C# 7.3 "improved overload candidates": overload resolution prunes candidates with a
	// static/instance mismatch (decided by the receiver form), generic candidates whose
	// constraints are violated, and (for method group conversions) candidates whose return
	// type does not match the delegate. The call sites below bind unambiguously only under
	// these rules; the decompiler keeps them unambiguous under pre-7.3 rules as well by
	// inserting explicit argument casts, so this output re-resolves to the original members
	// in every language version.
	public class CS73_ImprovedOverloadCandidates
	{
		public class StaticFromStaticContext
		{
			public static string M(object o)
			{
				return "static M(object)";
			}

			public string M(string s)
			{
				return "instance M(string)";
			}

			public static string CallFromStaticContext()
			{
				// C# 7.3 binds M("x") to the static overload because instance members are
				// removed in a static context; the cast keeps the binding explicit.
				return M((object)"x");
			}
		}

		public class InstanceReceiver
		{
			public static string M(string s)
			{
				return "static M(string)";
			}

			public string M(object o)
			{
				return "instance M(object)";
			}

			public string CallViaThis()
			{
				// The original source is this.M("x"): C# 7.3 removes the static overload
				// because the receiver is an instance.
				return M((object)"x");
			}

			public string CallSimpleName()
			{
				// A simple name does not fix the receiver form, so no pruning happens and
				// the better-matching static overload wins in all language versions.
				return M("x");
			}
		}

		public class TypeNameReceiver
		{
			public string M(string s)
			{
				return "instance M(string)";
			}

			public static string M(object o)
			{
				return "static M(object)";
			}

			public static string CallViaTypeName()
			{
				// The original source is TypeNameReceiver.M("x"): C# 7.3 removes the
				// instance overload because the receiver is a type.
				return M((object)"x");
			}
		}

		public class CombinedPruning
		{
			public string M(string s)
			{
				return "instance M(string)";
			}

			public static string M<T>(T t) where T : struct
			{
				return "static M<T>";
			}

			public static string M(object o)
			{
				return "static M(object)";
			}

			public static string Call()
			{
				// Both prunings at once: the instance overload is removed in the static
				// context and the generic overload is removed because T=string violates
				// the struct constraint.
				return M((object)"x");
			}
		}

		public static class ConstraintPruning
		{
			public static string StructConstraint<T>(T t) where T : struct
			{
				return "StructConstraint<T>";
			}

			public static string StructConstraint(object o)
			{
				return "StructConstraint(object)";
			}

			public static string ClassConstraint<T>(T t) where T : class
			{
				return "ClassConstraint<T>";
			}

			public static string ClassConstraint(long l)
			{
				return "ClassConstraint(long)";
			}

			public static string BaseTypeConstraint<T>(T t) where T : Exception
			{
				return "BaseTypeConstraint<T>";
			}

			public static string BaseTypeConstraint(object o)
			{
				return "BaseTypeConstraint(object)";
			}

			public static string UnmanagedConstraint<T>(T t) where T : unmanaged
			{
				return "UnmanagedConstraint<T>";
			}

			public static string UnmanagedConstraint(object o)
			{
				return "UnmanagedConstraint(object)";
			}

			public static string CallStructConstraintViolated()
			{
				return StructConstraint((object)"x");
			}

			public static string CallStructConstraintViolatedByNullable(int? value)
			{
				// Nullable<int> does not satisfy the struct constraint either.
				return StructConstraint((object)value);
			}

			public static string CallStructConstraintSatisfied()
			{
				return StructConstraint(42);
			}

			public static string CallClassConstraintViolated()
			{
				return ClassConstraint(42L);
			}

			public static string CallBaseTypeConstraintViolated()
			{
				return BaseTypeConstraint((object)"x");
			}

			public static string CallBaseTypeConstraintSatisfied()
			{
				return BaseTypeConstraint(new InvalidOperationException());
			}

			public static string CallUnmanagedConstraintViolated()
			{
				return UnmanagedConstraint((object)"x");
			}
		}

		public static class MethodGroupReturnType
		{
			public static void M(string s)
			{
			}

			public static int M(object o)
			{
				return 42;
			}

			public static Func<string, int> Get()
			{
				// Binds to M(object): void M(string) is removed because its return type
				// does not match the delegate. This method group requires the C# 7.3
				// rules to compile at all.
				return M;
			}
		}

		public class MethodGroupStaticInstance
		{
			public static string M(string s)
			{
				return "static M(string)";
			}

			public string M(object o)
			{
				return "instance M(object)";
			}

			public static Func<string, string> ViaTypeName()
			{
				// Binds to the static overload; it is also the better parameter match, so
				// the binding is the same in all language versions.
				return M;
			}
		}

		public static class MethodGroupConstraint
		{
			public static string G<T>(T t) where T : struct
			{
				return "G<T>";
			}

			public static string G(object o)
			{
				return "G(object)";
			}

			public static Func<string, string> Get()
			{
				// Binds to G(object): G<string> is removed because the struct constraint
				// is violated. This method group requires the C# 7.3 rules to compile.
				return G;
			}
		}
	}
}
