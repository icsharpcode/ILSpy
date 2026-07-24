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
using System.Linq;
using System.Linq.Expressions;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
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

		OverloadResolution ResolveWithDefaultLiteralArgument(params IMethod[] candidates)
		{
			var or = new OverloadResolution(compilation, new ResolveResult[] { new DefaultLiteralResolveResult() });
			foreach (var candidate in candidates)
			{
				Assert.That(or.AddCandidate(candidate), Is.EqualTo(OverloadResolutionErrors.None));
			}
			return or;
		}

		[Test]
		public void DefaultLiteralArgumentPrefersBetterConversionTarget()
		{
			// csc resolves M(default) by the better conversion target:
			// M(int)/M(long) -> int; M(object)/M(string) -> string; M(int)/M(int?) -> int
			var mInt = MakeMethod(typeof(int));
			var or = ResolveWithDefaultLiteralArgument(mInt, MakeMethod(typeof(long)));
			Assert.That(!or.IsAmbiguous);
			Assert.That(or.BestCandidate, Is.SameAs(mInt));

			var mString = MakeMethod(typeof(string));
			or = ResolveWithDefaultLiteralArgument(MakeMethod(typeof(object)), mString);
			Assert.That(!or.IsAmbiguous);
			Assert.That(or.BestCandidate, Is.SameAs(mString));

			mInt = MakeMethod(typeof(int));
			or = ResolveWithDefaultLiteralArgument(mInt, MakeMethod(typeof(int?)));
			Assert.That(!or.IsAmbiguous);
			Assert.That(or.BestCandidate, Is.SameAs(mInt));
		}

		[Test]
		public void DefaultLiteralArgumentWithoutBetterConversionTargetIsAmbiguous()
		{
			// csc: M(default) with M(int)/M(string) is CS0121
			var or = ResolveWithDefaultLiteralArgument(MakeMethod(typeof(int)), MakeMethod(typeof(string)));
			Assert.That(or.IsAmbiguous);
		}

		[Test]
		public void DefaultLiteralArgumentDoesNotParticipateInTypeInference()
		{
			// csc: G(default) with G<T>(T) is CS0411 (cannot infer the type argument)
			var m = compilation.FindType(typeof(DefaultLiteralGenericTestCase)).GetMethods(m2 => m2.Name == "G").Single();
			var or = new OverloadResolution(compilation, new ResolveResult[] { new DefaultLiteralResolveResult() });
			Assert.That(or.AddCandidate(m), Is.EqualTo(OverloadResolutionErrors.TypeInferenceFailed));
			Assert.That(!or.FoundApplicableCandidate);
		}

		class DefaultLiteralGenericTestCase
		{
			public static T G<T>(T x)
			{
				return x;
			}

			public static T H<T>(T a, T b)
			{
				return a;
			}
		}

		[Test]
		public void DefaultLiteralArgumentRidesAlongInTypeInference()
		{
			// csc: F("x", default) with F<T>(T, T) infers T=string;
			// F(default, default) is CS0411.
			var m = compilation.FindType(typeof(DefaultLiteralGenericTestCase)).GetMethods(m2 => m2.Name == "H").Single();
			var or = new OverloadResolution(compilation, new ResolveResult[] {
				new ResolveResult(compilation.FindType(KnownTypeCode.String)),
				new DefaultLiteralResolveResult()
			});
			Assert.That(or.AddCandidate(m), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(or.FoundApplicableCandidate);
			Assert.That(((IMethod)or.GetBestCandidateWithSubstitutedTypeArguments()).TypeArguments.Single().IsKnownType(KnownTypeCode.String));

			or = new OverloadResolution(compilation, new ResolveResult[] {
				new DefaultLiteralResolveResult(),
				new DefaultLiteralResolveResult()
			});
			Assert.That(or.AddCandidate(m), Is.EqualTo(OverloadResolutionErrors.TypeInferenceFailed));
		}

		[Test]
		public void BinaryOperatorWithDefaultLiteralOperand()
		{
			// csc: "i == default" uses the int equality operator, "s == default" the string one;
			// "default == default" is an error (CS8315).
			var resolver = new CSharpResolver(compilation);
			var i = new ResolveResult(compilation.FindType(KnownTypeCode.Int32));
			var s = new ResolveResult(compilation.FindType(KnownTypeCode.String));

			var rr = resolver.ResolveBinaryOperator(BinaryOperatorType.Equality, i, new DefaultLiteralResolveResult());
			Assert.That(!rr.IsError);
			Assert.That(rr.Type.IsKnownType(KnownTypeCode.Boolean));

			rr = resolver.ResolveBinaryOperator(BinaryOperatorType.Equality, s, new DefaultLiteralResolveResult());
			Assert.That(!rr.IsError);
			Assert.That(rr.Type.IsKnownType(KnownTypeCode.Boolean));

			rr = resolver.ResolveBinaryOperator(BinaryOperatorType.Equality, new DefaultLiteralResolveResult(), new DefaultLiteralResolveResult());
			Assert.That(rr.IsError);
		}
	}
}
