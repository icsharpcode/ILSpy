// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Tests.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Semantics
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class OverloadResolutionTests
	{
		ICompilation compilation;

		[OneTimeSetUp]
		public void SetUp()
		{
			compilation = new SimpleCompilation(TypeSystemLoaderTests.TestAssembly,
				TypeSystemLoaderTests.Mscorlib,
				TypeSystemLoaderTests.SystemCore);
		}

		ResolveResult[] MakeArgumentList(params Type[] argumentTypes)
		{
			return argumentTypes.Select(t => new ResolveResult(compilation.FindType(t))).ToArray();
		}

		IMethod MakeMethod(params object[] parameterTypesOrDefaultValues)
		{
			var context = new SimpleTypeResolveContext(compilation.MainModule);
			var m = new FakeMethod(compilation, SymbolKind.Method);
			m.Name = "Method";
			var parameters = new List<IParameter>();
			foreach (var typeOrDefaultValue in parameterTypesOrDefaultValues)
			{
				Type type = typeOrDefaultValue as Type;
				if (type != null)
					parameters.Add(new DefaultParameter(compilation.FindType(type), string.Empty, owner: m));
				else if (Type.GetTypeCode(typeOrDefaultValue.GetType()) > TypeCode.Object)
					parameters.Add(new DefaultParameter(compilation.FindType(typeOrDefaultValue.GetType()), string.Empty,
						owner: m, isOptional: true, defaultValue: typeOrDefaultValue));
				else
					throw new ArgumentException(typeOrDefaultValue.ToString());
			}
			m.Parameters = parameters;
			return m;
		}

		IMethod MakeParamsMethod(params object[] parameterTypesOrDefaultValues)
		{
			var m = (FakeMethod)MakeMethod(parameterTypesOrDefaultValues);
			var parameters = m.Parameters.ToList();
			parameters[parameters.Count - 1] = new DefaultParameter(
				parameters.Last().Type, parameters.Last().Name,
				isParams: true);
			m.Parameters = parameters;
			return m;
		}

		[Test]
		public void PreferIntOverUInt()
		{
			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList(typeof(ushort)));
			var c1 = MakeMethod(typeof(int));
			Assert.That(r.AddCandidate(c1), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.AddCandidate(MakeMethod(typeof(uint))), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(!r.IsAmbiguous);
			Assert.That(r.BestCandidate, Is.SameAs(c1));
		}

		[Test]
		public void PreferUIntOverLong_FromIntLiteral()
		{
			ResolveResult[] args = { new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), 1) };
			OverloadResolution r = new OverloadResolution(compilation, args);
			var c1 = MakeMethod(typeof(uint));
			Assert.That(r.AddCandidate(c1), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.AddCandidate(MakeMethod(typeof(long))), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(!r.IsAmbiguous);
			Assert.That(r.BestCandidate, Is.SameAs(c1));
		}

		[Test]
		public void NullableIntAndNullableUIntIsAmbiguous()
		{
			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList(typeof(ushort?)));
			Assert.That(r.AddCandidate(MakeMethod(typeof(int?))), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.AddCandidate(MakeMethod(typeof(uint?))), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.BestCandidateErrors, Is.EqualTo(OverloadResolutionErrors.AmbiguousMatch));

			// then adding a matching overload solves the ambiguity:
			Assert.That(r.AddCandidate(MakeMethod(typeof(ushort?))), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.BestCandidateErrors, Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.BestCandidateAmbiguousWith, Is.Null);
		}

		[Test]
		public void ParamsMethodMatchesEmptyArgumentList()
		{
			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList());
			Assert.That(r.AddCandidate(MakeParamsMethod(typeof(int[]))), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.BestCandidateIsExpandedForm);
		}

		[Test]
		public void ParamsMethodMatchesOneArgumentInExpandedForm()
		{
			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList(typeof(int)));
			Assert.That(r.AddCandidate(MakeParamsMethod(typeof(int[]))), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.BestCandidateIsExpandedForm);
		}

		[Test]
		public void ParamsMethodMatchesInUnexpandedForm()
		{
			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList(typeof(int[])));
			Assert.That(r.AddCandidate(MakeParamsMethod(typeof(int[]))), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(!r.BestCandidateIsExpandedForm);
		}

		[Test]
		public void LessArgumentsPassedToParamsIsBetter()
		{
			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList(typeof(int), typeof(int), typeof(int)));
			Assert.That(r.AddCandidate(MakeParamsMethod(typeof(int[]))), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.AddCandidate(MakeParamsMethod(typeof(int), typeof(int[]))), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(!r.IsAmbiguous);
			Assert.That(r.BestCandidate.Parameters.Count, Is.EqualTo(2));
		}

		[Test]
		public void CallInvalidParamsDeclaration()
		{
			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList(typeof(int[,])));
			Assert.That(r.AddCandidate(MakeParamsMethod(typeof(int))), Is.EqualTo(OverloadResolutionErrors.ArgumentTypeMismatch));
			Assert.That(!r.BestCandidateIsExpandedForm);
		}

		[Test]
		public void PreferMethodWithoutOptionalParameters()
		{
			var m1 = MakeMethod();
			var m2 = MakeMethod(1);

			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList());
			Assert.That(r.AddCandidate(m1), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.AddCandidate(m2), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(!r.IsAmbiguous);
			Assert.That(r.BestCandidate, Is.SameAs(m1));
		}

		[Test]
		public void SkeetEvilOverloadResolution()
		{
			// http://msmvps.com/blogs/jon_skeet/archive/2010/11/02/evil-code-overload-resolution-workaround.aspx

			var container = compilation.FindType(typeof(SkeetEvilOverloadResolutionTestCase)).GetDefinition();
			IMethod resolvedM1 = container.GetMethods(m => m.Name == "Foo").First();
			IMethod resolvedM2 = container.GetMethods(m => m.Name == "Foo").Skip(1).First();
			IMethod resolvedM3 = container.GetMethods(m => m.Name == "Foo").Skip(2).First();

			// Call: Foo<int>();
			OverloadResolution o;
			o = new OverloadResolution(compilation, new ResolveResult[0], typeArguments: new[] { compilation.FindType(typeof(int)) });
			Assert.That(o.AddCandidate(resolvedM1), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(o.AddCandidate(resolvedM2), Is.EqualTo(OverloadResolutionErrors.ConstructedTypeDoesNotSatisfyConstraint));
			Assert.That(o.BestCandidate, Is.SameAs(resolvedM1));

			// Call: Foo<string>();
			o = new OverloadResolution(compilation, new ResolveResult[0], typeArguments: new[] { compilation.FindType(typeof(string)) });
			Assert.That(o.AddCandidate(resolvedM1), Is.EqualTo(OverloadResolutionErrors.ConstructedTypeDoesNotSatisfyConstraint));
			Assert.That(o.AddCandidate(resolvedM2), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(o.BestCandidate, Is.SameAs(resolvedM2));

			// Call: Foo<int?>();
			o = new OverloadResolution(compilation, new ResolveResult[0], typeArguments: new[] { compilation.FindType(typeof(int?)) });
			Assert.That(o.AddCandidate(resolvedM1), Is.EqualTo(OverloadResolutionErrors.ConstructedTypeDoesNotSatisfyConstraint));
			Assert.That(o.AddCandidate(resolvedM2), Is.EqualTo(OverloadResolutionErrors.ConstructedTypeDoesNotSatisfyConstraint));
			Assert.That(o.AddCandidate(resolvedM3), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(o.BestCandidate, Is.SameAs(resolvedM3));
		}

		class SkeetEvilOverloadResolutionTestCase
		{
			class ClassConstraint<T> where T : class { }
			static void Foo<T>(T? ignored = default(T?)) where T : struct { }
			static void Foo<T>(ClassConstraint<T> ignored = default(ClassConstraint<T>)) where T : class { }
			static void Foo<T>() { }
		}

		/// <summary>
		/// A lambda of the form "() => default(returnType)"
		/// </summary>
		class MockLambda : LambdaResolveResult
		{
			readonly IType inferredReturnType;
			internal readonly List<IParameter> parameters = new List<IParameter>();

			public MockLambda(IType returnType)
			{
				this.inferredReturnType = returnType;
			}

			public override IReadOnlyList<IParameter> Parameters {
				get { return parameters; }
			}

			public override Conversion IsValid(IType[] parameterTypes, IType returnType, CSharpConversions conversions)
			{
				return conversions.ImplicitConversion(inferredReturnType, returnType);
			}

			public override bool IsImplicitlyTyped {
				get { return false; }
			}

			public override bool IsAnonymousMethod {
				get { return false; }
			}

			public override bool HasParameterList {
				get { return true; }
			}

			public override bool IsAsync {
				get { return false; }
			}

			public override ResolveResult Body {
				get { throw new NotImplementedException(); }
			}

			public override IType ReturnType {
				get { throw new NotImplementedException(); }
			}

			public override IType GetInferredReturnType(IType[] parameterTypes)
			{
				return inferredReturnType;
			}
		}

		[Test]
		public void BetterConversionByLambdaReturnValue()
		{
			var m1 = MakeMethod(typeof(Func<long>));
			var m2 = MakeMethod(typeof(Func<int>));

			// M(() => default(byte));
			ResolveResult[] args = {
				new MockLambda(compilation.FindType(KnownTypeCode.Byte))
			};

			OverloadResolution r = new OverloadResolution(compilation, args);
			Assert.That(r.AddCandidate(m1), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.AddCandidate(m2), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.BestCandidate, Is.SameAs(m2));
			Assert.That(r.BestCandidateErrors, Is.EqualTo(OverloadResolutionErrors.None));
		}

		[Test]
		public void BetterConversionByLambdaReturnValue_ExpressionTree()
		{
			var m1 = MakeMethod(typeof(Func<long>));
			var m2 = MakeMethod(typeof(Expression<Func<int>>));

			// M(() => default(byte));
			ResolveResult[] args = {
				new MockLambda(compilation.FindType(KnownTypeCode.Byte))
			};

			OverloadResolution r = new OverloadResolution(compilation, args);
			Assert.That(r.AddCandidate(m1), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.AddCandidate(m2), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.BestCandidate, Is.SameAs(m2));
			Assert.That(r.BestCandidateErrors, Is.EqualTo(OverloadResolutionErrors.None));
		}

		[Test]
		public void Lambda_DelegateAndExpressionTreeOverloadsAreAmbiguous()
		{
			var m1 = MakeMethod(typeof(Func<int>));
			var m2 = MakeMethod(typeof(Expression<Func<int>>));

			// M(() => default(int));
			ResolveResult[] args = {
				new MockLambda(compilation.FindType(KnownTypeCode.Int32))
			};

			OverloadResolution r = new OverloadResolution(compilation, args);
			Assert.That(r.AddCandidate(m1), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.AddCandidate(m2), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.BestCandidateErrors, Is.EqualTo(OverloadResolutionErrors.AmbiguousMatch));
		}

		[Test, Ignore("Overload Resolution bug")]
		public void BetterFunctionMemberIsNotTransitive()
		{
			var container = compilation.FindType(typeof(BetterFunctionMemberIsNotTransitiveTestCase)).GetDefinition();

			var args = new ResolveResult[] {
				new MockLambda(compilation.FindType(KnownTypeCode.String)) { parameters = { new DefaultParameter(SpecialType.UnknownType, "arg") } }
			};

			OverloadResolution r = new OverloadResolution(compilation, args);
			foreach (var method in container.GetMethods(m => m.Name == "Method"))
			{
				Assert.That(r.AddCandidate(method), Is.EqualTo(OverloadResolutionErrors.None));
			}

			Assert.That(r.BestCandidate, Is.EqualTo(container.GetMethods(m => m.Name == "Method").Last()));
		}

		class BetterFunctionMemberIsNotTransitiveTestCase
		{
			static void Method(Action<string> a) { }
			static void Method<T>(Func<string, T> a) { }
			static void Method(Action<object> a) { }
			static void Method<T>(Func<object, T> a) { }

			public static void Main(string[] args)
			{
				Method(a => a.ToString());
			}
		}

		#region First-class span betterness

		public struct ConvertibleToBothReadOnlySpans
		{
			public static implicit operator ReadOnlySpan<int>(ConvertibleToBothReadOnlySpans c)
			{
				return default;
			}

			public static implicit operator ReadOnlySpan<long>(ConvertibleToBothReadOnlySpans c)
			{
				return default;
			}
		}

		static IMethod MakeMethodIn(ICompilation c, params Type[] parameterTypes)
		{
			var m = new FakeMethod(c, SymbolKind.Method);
			m.Name = "Method";
			m.Parameters = parameterTypes
				.Select(t => (IParameter)new DefaultParameter(c.FindType(t), string.Empty, owner: m))
				.ToList();
			return m;
		}

		[Test]
		public void ReadOnlySpanOverloadsWithUnrelatedElementTypesAreAmbiguous()
		{
			// C# 14 spec, 12.6.4.7: ReadOnlySpan<E1> is a better conversion target than
			// ReadOnlySpan<E2> only if an implicit conversion exists from ReadOnlySpan<E1>
			// to ReadOnlySpan<E2> - the span types, not the element types. No span conversion
			// relates ReadOnlySpan<int> and ReadOnlySpan<long>, so neither target is better
			// and the call is ambiguous; Roslyn reports CS0121.
			var c = RefAssemblyCompilation.Instance;
			var r = new OverloadResolution(c, new[] {
				new ResolveResult(c.FindType(typeof(ConvertibleToBothReadOnlySpans)))
			});
			Assert.That(r.AddCandidate(MakeMethodIn(c, typeof(ReadOnlySpan<int>))), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.AddCandidate(MakeMethodIn(c, typeof(ReadOnlySpan<long>))), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.IsAmbiguous);
		}

		static IMethod MakeByRefMethodIn(ICompilation c, Type parameterType, ReferenceKind kind)
		{
			var m = new FakeMethod(c, SymbolKind.Method);
			m.Name = "Method";
			m.Parameters = new List<IParameter> {
				new DefaultParameter(new ByReferenceType(c.FindType(parameterType)), string.Empty,
					owner: m, referenceKind: kind)
			};
			return m;
		}

		static IMethod MakeInMethodIn(ICompilation c, Type parameterType)
			=> MakeByRefMethodIn(c, parameterType, ReferenceKind.In);

		[Test]
		public void InReadOnlySpanParameter_BindsAnArrayWithoutInButNotWithIn()
		{
			// Roslyn: OnlyIn(arr) compiles - a value argument may bind to an 'in' parameter
			// through the implicit span conversion (a temporary is created). OnlyIn(in arr)
			// is CS1503: an explicit 'in' argument must have the parameter's own type.
			var c = RefAssemblyCompilation.Instance;
			var arrayArg = new ResolveResult(new ArrayType(c, c.FindType(KnownTypeCode.Int32)));

			var implicitIn = new OverloadResolution(c, new[] { arrayArg });
			Assert.That(implicitIn.AddCandidate(MakeInMethodIn(c, typeof(ReadOnlySpan<int>))),
				Is.EqualTo(OverloadResolutionErrors.None));

			var explicitIn = new OverloadResolution(c, new[] {
				new ByReferenceResolveResult(arrayArg, ReferenceKind.In)
			});
			Assert.That(explicitIn.AddCandidate(MakeInMethodIn(c, typeof(ReadOnlySpan<int>))),
				Is.Not.EqualTo(OverloadResolutionErrors.None));
		}

		[Test]
		public void InReadOnlySpanParameter_BindsAnIdentityArgumentWithAndWithoutIn()
		{
			var c = RefAssemblyCompilation.Instance;
			var rosArg = new ResolveResult(c.FindType(typeof(ReadOnlySpan<int>)));

			var implicitIn = new OverloadResolution(c, new[] { rosArg });
			Assert.That(implicitIn.AddCandidate(MakeInMethodIn(c, typeof(ReadOnlySpan<int>))),
				Is.EqualTo(OverloadResolutionErrors.None));

			var explicitIn = new OverloadResolution(c, new[] {
				new ByReferenceResolveResult(rosArg, ReferenceKind.In)
			});
			Assert.That(explicitIn.AddCandidate(MakeInMethodIn(c, typeof(ReadOnlySpan<int>))),
				Is.EqualTo(OverloadResolutionErrors.None));
		}

		[Test]
		public void ByValueOverloadPreferredOverInOverload_WithoutInAtTheCall()
		{
			// Roslyn: for F(ReadOnlySpan<int>) vs F(in ReadOnlySpan<int>), a call without 'in'
			// picks the by-value overload - both for an identity argument and through the
			// span conversion from int[].
			var c = RefAssemblyCompilation.Instance;
			foreach (var arg in new[] {
				new ResolveResult(c.FindType(typeof(ReadOnlySpan<int>))),
				new ResolveResult(new ArrayType(c, c.FindType(KnownTypeCode.Int32)))
			})
			{
				var r = new OverloadResolution(c, new[] { arg });
				var byValue = MakeMethodIn(c, typeof(ReadOnlySpan<int>));
				Assert.That(r.AddCandidate(byValue), Is.EqualTo(OverloadResolutionErrors.None));
				Assert.That(r.AddCandidate(MakeInMethodIn(c, typeof(ReadOnlySpan<int>))),
					Is.EqualTo(OverloadResolutionErrors.None));
				Assert.That(!r.IsAmbiguous, $"argument {arg.Type}");
				Assert.That(r.BestCandidate, Is.SameAs(byValue), $"argument {arg.Type}");
			}
		}

		[Test]
		public void RefAndOutParametersNeverBindThroughASpanConversion()
		{
			// Roslyn: CS1503 for both 'M(ref arr)' against 'ref ReadOnlySpan<int>' and
			// 'M(out arr)' against 'out ReadOnlySpan<int>' - ref and out demand the
			// parameter's own type; the span conversion does not apply. A value argument
			// without the keyword is a passing-mode mismatch regardless of conversions.
			var c = RefAssemblyCompilation.Instance;
			var arrayArg = new ResolveResult(new ArrayType(c, c.FindType(KnownTypeCode.Int32)));
			foreach (var kind in new[] { ReferenceKind.Ref, ReferenceKind.Out })
			{
				var byRefArgument = new OverloadResolution(c, new[] {
					new ByReferenceResolveResult(arrayArg, kind)
				});
				Assert.That(byRefArgument.AddCandidate(MakeByRefMethodIn(c, typeof(ReadOnlySpan<int>), kind)),
					Is.Not.EqualTo(OverloadResolutionErrors.None), kind.ToString());

				var valueArgument = new OverloadResolution(c, new[] { arrayArg });
				Assert.That(valueArgument.AddCandidate(MakeByRefMethodIn(c, typeof(ReadOnlySpan<int>), kind)),
					Is.Not.EqualTo(OverloadResolutionErrors.None), kind.ToString());
			}
		}

		[Test]
		public void InOverloadIsTheOnlyCandidateWithInAtTheCall()
		{
			// Roslyn: F(in ros) picks the in-overload; the by-value overload cannot take an
			// 'in' argument.
			var c = RefAssemblyCompilation.Instance;
			var r = new OverloadResolution(c, new[] {
				new ByReferenceResolveResult(new ResolveResult(c.FindType(typeof(ReadOnlySpan<int>))), ReferenceKind.In)
			});
			Assert.That(r.AddCandidate(MakeMethodIn(c, typeof(ReadOnlySpan<int>))),
				Is.Not.EqualTo(OverloadResolutionErrors.None));
			var inOverload = MakeInMethodIn(c, typeof(ReadOnlySpan<int>));
			Assert.That(r.AddCandidate(inOverload), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.BestCandidate, Is.SameAs(inOverload));
		}

		[Test]
		public void ReadOnlySpanOfStringPreferredOverReadOnlySpanOfObject()
		{
			// The positive direction of the same rule: string[] converts to both targets, and
			// the covariant span conversion ReadOnlySpan<string> -> ReadOnlySpan<object>
			// exists, so ReadOnlySpan<string> is the better target.
			var c = RefAssemblyCompilation.Instance;
			var r = new OverloadResolution(c, new[] {
				new ResolveResult(new ArrayType(c, c.FindType(KnownTypeCode.String)))
			});
			var better = MakeMethodIn(c, typeof(ReadOnlySpan<string>));
			Assert.That(r.AddCandidate(better), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(r.AddCandidate(MakeMethodIn(c, typeof(ReadOnlySpan<object>))), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(!r.IsAmbiguous);
			Assert.That(r.BestCandidate, Is.SameAs(better));
		}

		#endregion
	}
}
