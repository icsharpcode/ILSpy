using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Semantics
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class OverloadResolutionTests
	{
		ICompilation compilation;

		[SetUp]
		public void SetUp()
		{
			var cecilLoader = new MetadataLoader() { IncludeInternalMembers = true };
			var mscorlib = cecilLoader.LoadAssemblyFile(typeof(object).Assembly.Location);
			var systemCore = cecilLoader.LoadAssemblyFile(typeof(System.Linq.Enumerable).Assembly.Location);
			compilation = new SimpleCompilation(cecilLoader.LoadAssemblyFile(typeof(OverloadResolutionTests).Assembly.Location), mscorlib, systemCore);
		}

		ResolveResult[] MakeArgumentList(params Type[] argumentTypes)
		{
			return argumentTypes.Select(t => new ResolveResult(compilation.FindType(t))).ToArray();
		}

		IMethod MakeMethod(params object[] parameterTypesOrDefaultValues)
		{
			var context = new SimpleTypeResolveContext(compilation.MainAssembly);
			return (IMethod)MakeUnresolvedMethod(parameterTypesOrDefaultValues).CreateResolved(context);
		}

		DefaultUnresolvedMethod MakeUnresolvedMethod(params object[] parameterTypesOrDefaultValues)
		{
			var m = new DefaultUnresolvedMethod();
			m.Name = "Method";
			foreach (var typeOrDefaultValue in parameterTypesOrDefaultValues) {
				Type type = typeOrDefaultValue as Type;
				if (type != null)
					m.Parameters.Add(new DefaultUnresolvedParameter(type.ToTypeReference(), string.Empty));
				else if (Type.GetTypeCode(typeOrDefaultValue.GetType()) > TypeCode.Object)
					m.Parameters.Add(new DefaultUnresolvedParameter(typeOrDefaultValue.GetType().ToTypeReference(), string.Empty) {
						DefaultValue = new SimpleConstantValue(typeOrDefaultValue.GetType().ToTypeReference(), typeOrDefaultValue)
					});
				else
					throw new ArgumentException(typeOrDefaultValue.ToString());
			}
			return m;
		}

		IMethod MakeParamsMethod(params object[] parameterTypesOrDefaultValues)
		{
			var m = MakeUnresolvedMethod(parameterTypesOrDefaultValues);
			((DefaultUnresolvedParameter)m.Parameters.Last()).IsParams = true;
			var context = new SimpleTypeResolveContext(compilation.MainAssembly);
			return (IMethod)m.CreateResolved(context);
		}

		IUnresolvedParameter MakeOptionalParameter(ITypeReference type, string name)
		{
			return new DefaultUnresolvedParameter(type, name) {
				DefaultValue = new SimpleConstantValue(type, null)
			};
		}

		[Test]
		public void PreferIntOverUInt()
		{
			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList(typeof(ushort)));
			var c1 = MakeMethod(typeof(int));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(c1));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeMethod(typeof(uint))));
			Assert.IsFalse(r.IsAmbiguous);
			Assert.AreSame(c1, r.BestCandidate);
		}

		[Test]
		public void PreferUIntOverLong_FromIntLiteral()
		{
			ResolveResult[] args = { new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), 1) };
			OverloadResolution r = new OverloadResolution(compilation, args);
			var c1 = MakeMethod(typeof(uint));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(c1));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeMethod(typeof(long))));
			Assert.IsFalse(r.IsAmbiguous);
			Assert.AreSame(c1, r.BestCandidate);
		}

		[Test, Ignore("Broken after migration to ICS.Decompiler")]
		public void NullableIntAndNullableUIntIsAmbiguous()
		{
			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList(typeof(ushort?)));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeMethod(typeof(int?))));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeMethod(typeof(uint?))));
			Assert.AreEqual(OverloadResolutionErrors.AmbiguousMatch, r.BestCandidateErrors);

			// then adding a matching overload solves the ambiguity:
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeMethod(typeof(ushort?))));
			Assert.AreEqual(OverloadResolutionErrors.None, r.BestCandidateErrors);
			Assert.IsNull(r.BestCandidateAmbiguousWith);
		}

		[Test]
		public void ParamsMethodMatchesEmptyArgumentList()
		{
			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList());
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeParamsMethod(typeof(int[]))));
			Assert.IsTrue(r.BestCandidateIsExpandedForm);
		}

		[Test]
		public void ParamsMethodMatchesOneArgumentInExpandedForm()
		{
			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList(typeof(int)));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeParamsMethod(typeof(int[]))));
			Assert.IsTrue(r.BestCandidateIsExpandedForm);
		}

		[Test]
		public void ParamsMethodMatchesInUnexpandedForm()
		{
			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList(typeof(int[])));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeParamsMethod(typeof(int[]))));
			Assert.IsFalse(r.BestCandidateIsExpandedForm);
		}

		[Test]
		public void LessArgumentsPassedToParamsIsBetter()
		{
			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList(typeof(int), typeof(int), typeof(int)));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeParamsMethod(typeof(int[]))));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(MakeParamsMethod(typeof(int), typeof(int[]))));
			Assert.IsFalse(r.IsAmbiguous);
			Assert.AreEqual(2, r.BestCandidate.Parameters.Count);
		}

		[Test]
		public void CallInvalidParamsDeclaration()
		{
			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList(typeof(int[,])));
			Assert.AreEqual(OverloadResolutionErrors.ArgumentTypeMismatch, r.AddCandidate(MakeParamsMethod(typeof(int))));
			Assert.IsFalse(r.BestCandidateIsExpandedForm);
		}

		[Test]
		public void PreferMethodWithoutOptionalParameters()
		{
			var m1 = MakeMethod();
			var m2 = MakeMethod(1);

			OverloadResolution r = new OverloadResolution(compilation, MakeArgumentList());
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(m1));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(m2));
			Assert.IsFalse(r.IsAmbiguous);
			Assert.AreSame(m1, r.BestCandidate);
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
			Assert.AreEqual(OverloadResolutionErrors.None, o.AddCandidate(resolvedM1));
			Assert.AreEqual(OverloadResolutionErrors.ConstructedTypeDoesNotSatisfyConstraint, o.AddCandidate(resolvedM2));
			Assert.AreSame(resolvedM1, o.BestCandidate);

			// Call: Foo<string>();
			o = new OverloadResolution(compilation, new ResolveResult[0], typeArguments: new[] { compilation.FindType(typeof(string)) });
			Assert.AreEqual(OverloadResolutionErrors.ConstructedTypeDoesNotSatisfyConstraint, o.AddCandidate(resolvedM1));
			Assert.AreEqual(OverloadResolutionErrors.None, o.AddCandidate(resolvedM2));
			Assert.AreSame(resolvedM2, o.BestCandidate);

			// Call: Foo<int?>();
			o = new OverloadResolution(compilation, new ResolveResult[0], typeArguments: new[] { compilation.FindType(typeof(int?)) });
			Assert.AreEqual(OverloadResolutionErrors.ConstructedTypeDoesNotSatisfyConstraint, o.AddCandidate(resolvedM1));
			Assert.AreEqual(OverloadResolutionErrors.ConstructedTypeDoesNotSatisfyConstraint, o.AddCandidate(resolvedM2));
			Assert.AreEqual(OverloadResolutionErrors.None, o.AddCandidate(resolvedM3));
			Assert.AreSame(resolvedM3, o.BestCandidate);
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
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(m1));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(m2));
			Assert.AreSame(m2, r.BestCandidate);
			Assert.AreEqual(OverloadResolutionErrors.None, r.BestCandidateErrors);
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
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(m1));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(m2));
			Assert.AreSame(m2, r.BestCandidate);
			Assert.AreEqual(OverloadResolutionErrors.None, r.BestCandidateErrors);
		}

		[Test, Ignore("Broken on SRM branch???")]
		public void Lambda_DelegateAndExpressionTreeOverloadsAreAmbiguous()
		{
			var m1 = MakeMethod(typeof(Func<int>));
			var m2 = MakeMethod(typeof(Expression<Func<int>>));

			// M(() => default(int));
			ResolveResult[] args = {
				new MockLambda(compilation.FindType(KnownTypeCode.Int32))
			};

			OverloadResolution r = new OverloadResolution(compilation, args);
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(m1));
			Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(m2));
			Assert.AreEqual(OverloadResolutionErrors.AmbiguousMatch, r.BestCandidateErrors);
		}

		[Test, Ignore("Overload Resolution bug")]
		public void BetterFunctionMemberIsNotTransitive()
		{
			var container = compilation.FindType(typeof(BetterFunctionMemberIsNotTransitiveTestCase)).GetDefinition();

			var args = new ResolveResult[] {
				new MockLambda(compilation.FindType(KnownTypeCode.String)) { parameters = { new DefaultParameter(SpecialType.UnknownType, "arg") } } 
			};

			OverloadResolution r = new OverloadResolution(compilation, args);
			foreach (var method in container.GetMethods(m => m.Name == "Method")) {
				Assert.AreEqual(OverloadResolutionErrors.None, r.AddCandidate(method));
			}

			Assert.AreEqual(container.GetMethods(m => m.Name == "Method").Last(), r.BestCandidate); 
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
	}
}
