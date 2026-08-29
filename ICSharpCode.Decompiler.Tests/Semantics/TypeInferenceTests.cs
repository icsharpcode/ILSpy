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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Tests.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Semantics
{
	[TestFixture]
	public class TypeInferenceTests
	{
		public interface ICo<out T> { }
		public interface IContra<in T> { }
		public interface IInv<T> { }
		public class DoubleImpl : IInv<int>, IInv<string> { }

		public struct ConvertibleToString
		{
			public static implicit operator string(ConvertibleToString s)
			{
				return "a";
			}
		}

		public class MyConvertible
		{
			public static implicit operator MyConvertible(int number)
			{
				return null;
			}

			public static implicit operator int(MyConvertible obj)
			{
				return 0;
			}
		}

		ICompilation compilation;
		TypeInference ti;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			compilation = new SimpleCompilation(TypeSystemLoaderTests.TestAssembly,
				TypeSystemLoaderTests.Mscorlib,
				TypeSystemLoaderTests.SystemCore);
		}

		[SetUp]
		public void Setup()
		{
			ti = new TypeInference(compilation);
		}

		#region Type Inference
		[Test]
		public void ArrayToEnumerable()
		{
			ITypeParameter tp = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			IType stringType = compilation.FindType(KnownTypeCode.String);
			ITypeDefinition enumerableType = compilation.FindType(KnownTypeCode.IEnumerableOfT).GetDefinition();

			bool success;
			Assert.That(
				ti.InferTypeArguments(new[] { tp },
					new[] { new ResolveResult(new ArrayType(compilation, stringType)) },
					new IType[] { new ParameterizedType(enumerableType, new[] { tp }) },
					out success),
				Is.EqualTo(new[] { stringType }));
			Assert.That(success);
		}

		[Test]
		public void ArrayToReadOnlyList()
		{
			ITypeParameter tp = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			IType stringType = compilation.FindType(KnownTypeCode.String);
			ITypeDefinition readOnlyListType = compilation.FindType(KnownTypeCode.IReadOnlyListOfT).GetDefinition();

			bool success;
			Assert.That(
				ti.InferTypeArguments(new[] { tp },
					new[] { new ResolveResult(new ArrayType(compilation, stringType)) },
					new IType[] { new ParameterizedType(readOnlyListType, new[] { tp }) },
					out success),
				Is.EqualTo(new[] { stringType }));
			Assert.That(success);
		}

		[Test]
		public void EnumerableToArrayInContravariantType()
		{
			ITypeParameter tp = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			IType stringType = compilation.FindType(KnownTypeCode.String);
			ITypeDefinition enumerableType = compilation.FindType(typeof(IEnumerable<>)).GetDefinition();
			ITypeDefinition comparerType = compilation.FindType(typeof(IComparer<>)).GetDefinition();

			var comparerOfIEnumerableOfString = new ParameterizedType(comparerType, new IType[] { new ParameterizedType(enumerableType, new[] { stringType }) });
			var comparerOfTpArray = new ParameterizedType(comparerType, new IType[] { new ArrayType(compilation, tp) });

			bool success;
			Assert.That(
				ti.InferTypeArguments(new[] { tp },
					new[] { new ResolveResult(comparerOfIEnumerableOfString) },
					new IType[] { comparerOfTpArray },
					out success),
				Is.EqualTo(new[] { stringType }));
			Assert.That(success);
		}

		[Test]
		public void InferFromObjectAndFromNullLiteral()
		{
			// M<T>(T a, T b);
			ITypeParameter tp = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");

			// M(new object(), null);
			bool success;
			Assert.That(
				ti.InferTypeArguments(new[] { tp },
					new[] { new ResolveResult(compilation.FindType(KnownTypeCode.Object)), new ResolveResult(SpecialType.NullType) },
					new IType[] { tp, tp },
					out success),
				Is.EqualTo(new[] { compilation.FindType(KnownTypeCode.Object) }));
			Assert.That(success);
		}

		[Test]
		public void ArrayToListWithArrayCovariance()
		{
			ITypeParameter tp = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			IType objectType = compilation.FindType(KnownTypeCode.Object);
			IType stringType = compilation.FindType(KnownTypeCode.String);
			ITypeDefinition listType = compilation.FindType(KnownTypeCode.IListOfT).GetDefinition();

			// void M<T>(IList<T> a, T b);
			// M(new string[0], new object());

			bool success;
			Assert.That(
				ti.InferTypeArguments(
					new[] { tp },
					new[] { new ResolveResult(new ArrayType(compilation, stringType)), new ResolveResult(objectType) },
					new IType[] { new ParameterizedType(listType, new[] { tp }), tp },
					out success),
				Is.EqualTo(new[] { objectType }));
			Assert.That(success);
		}

		[Test]
		public void IEnumerableCovarianceWithDynamic()
		{
			ITypeParameter tp = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			var enumerableType = compilation.FindType(typeof(IEnumerable<>)).GetDefinition();
			var ienumerableOfT = new ParameterizedType(enumerableType, new[] { tp });
			var ienumerableOfString = new ParameterizedType(enumerableType, new[] { compilation.FindType(KnownTypeCode.String) });
			var ienumerableOfDynamic = new ParameterizedType(enumerableType, new[] { SpecialType.Dynamic });

			// static T M<T>(IEnumerable<T> x, IEnumerable<T> y) {}
			// M(IEnumerable<dynamic>, IEnumerable<string>); -> should infer T=dynamic, no ambiguity
			// See http://blogs.msdn.com/b/cburrows/archive/2010/04/01/errata-dynamic-conversions-and-overload-resolution.aspx
			// for details.

			bool success;
			Assert.That(
				ti.InferTypeArguments(
					new[] { tp },
					new[] { new ResolveResult(ienumerableOfDynamic), new ResolveResult(ienumerableOfString) },
					new IType[] { ienumerableOfT, ienumerableOfT },
					out success),
				Is.EqualTo(new[] { SpecialType.Dynamic }));
			Assert.That(success);
		}
		#endregion

		#region Inference with Method Groups
		[Test]
		public void CannotInferFromMethodParameterTypes()
		{
			// static void M<A, B>(Func<A, B> f) {}
			// M(int.Parse); // type inference fails
			var A = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "A");
			var B = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "B");

			IType declType = compilation.FindType(typeof(int));
			var methods = new MethodListWithDeclaringType(declType, declType.GetMethods(m => m.Name == "Parse"));
			var argument = new MethodGroupResolveResult(new TypeResolveResult(declType), "Parse", new[] { methods }, new IType[0]);

			bool success;
			ti.InferTypeArguments(new ITypeParameter[] { A, B }, new ResolveResult[] { argument },
				new IType[] { new ParameterizedType(compilation.FindType(typeof(Func<,>)).GetDefinition(), new IType[] { A, B }) },
				out success);
			Assert.That(!success);
		}

		[Test]
		public void InferFromMethodReturnType()
		{
			// static void M<T>(Func<T> f) {}
			// M(Console.ReadKey); // type inference produces ConsoleKeyInfo

			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");

			IType declType = compilation.FindType(typeof(Console));
			var methods = new MethodListWithDeclaringType(declType, declType.GetMethods(m => m.Name == "ReadKey"));
			var argument = new MethodGroupResolveResult(new TypeResolveResult(declType), "ReadKey", new[] { methods }, new IType[0]);

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T }, new ResolveResult[] { argument },
					new IType[] { new ParameterizedType(compilation.FindType(typeof(Func<>)).GetDefinition(), new IType[] { T }) },
					out success),
				Is.EqualTo(new[] { compilation.FindType(typeof(ConsoleKeyInfo)) }));
			Assert.That(success);
		}
		#endregion

		#region Inference with Lambda
		#region MockImplicitLambda
		sealed class MockImplicitLambda : LambdaResolveResult
		{
			IType[] expectedParameterTypes;
			IType inferredReturnType;
			IParameter[] parameters;
			bool isAsync;

			public MockImplicitLambda(IType[] expectedParameterTypes, IType inferredReturnType, bool isAsync = false)
			{
				this.expectedParameterTypes = expectedParameterTypes;
				this.inferredReturnType = inferredReturnType;
				this.isAsync = isAsync;
				this.parameters = new IParameter[expectedParameterTypes.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					// UnknownType because this lambda is implicitly typed
					parameters[i] = new DefaultParameter(SpecialType.UnknownType, "X" + i);
				}
			}

			public override IReadOnlyList<IParameter> Parameters {
				get { return parameters; }
			}

			public override Conversion IsValid(IType[] parameterTypes, IType returnType, CSharpConversions conversions)
			{
				Assert.That(parameterTypes, Is.EqualTo(expectedParameterTypes));
				return conversions.ImplicitConversion(inferredReturnType, returnType);
			}

			public override bool IsImplicitlyTyped {
				get { return true; }
			}

			public override bool IsAnonymousMethod {
				get { return false; }
			}

			public override bool HasParameterList {
				get { return true; }
			}

			public override bool IsAsync {
				get { return isAsync; }
			}

			public override ResolveResult Body {
				get { throw new NotImplementedException(); }
			}

			public override IType ReturnType {
				get { return SpecialType.UnknownType; }
			}

			public override IType GetInferredReturnType(IType[] parameterTypes)
			{
				Assert.That(parameterTypes, Is.EqualTo(expectedParameterTypes), "Parameters types passed to " + this);
				return inferredReturnType;
			}

			public override string ToString()
			{
				return "[MockImplicitLambda (" + string.Join<IType>(", ", expectedParameterTypes) + ") => " + inferredReturnType + "]";
			}
		}

		sealed class MockExplicitLambda : LambdaResolveResult
		{
			IType inferredReturnType;
			IParameter[] parameters;
			bool isAsync;

			public MockExplicitLambda(IType[] parameterTypes, IType inferredReturnType, bool isAsync = false)
			{
				this.inferredReturnType = inferredReturnType;
				this.isAsync = isAsync;
				this.parameters = new IParameter[parameterTypes.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					parameters[i] = new DefaultParameter(parameterTypes[i], "X" + i);
				}
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
				get { return isAsync; }
			}

			public override ResolveResult Body {
				get { throw new NotImplementedException(); }
			}

			public override IType ReturnType {
				get { return inferredReturnType; }
			}

			public override IType GetInferredReturnType(IType[] parameterTypes)
			{
				return inferredReturnType;
			}

			public override string ToString()
			{
				return "[MockExplicitLambda (" + string.Join<IParameter>(", ", parameters) + ") => " + inferredReturnType + "]";
			}
		}
		#endregion

		[Test]
		public void TestLambdaInference()
		{
			ITypeParameter[] typeParameters = {
				new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "X"),
				new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "Y"),
				new DefaultTypeParameter(compilation, SymbolKind.Method, 2, "Z")
			};
			IType[] parameterTypes = {
				typeParameters[0],
				new ParameterizedType(compilation.FindType(typeof(Func<,>)).GetDefinition(), new IType[] { typeParameters[0], typeParameters[1] }),
				new ParameterizedType(compilation.FindType(typeof(Func<,>)).GetDefinition(), new IType[] { typeParameters[1], typeParameters[2] })
			};
			// Signature:  M<X,Y,Z>(X x, Func<X,Y> y, Func<Y,Z> z) {}
			// Invocation: M(default(string), s => default(int), t => default(float));
			ResolveResult[] arguments = {
				new ResolveResult(compilation.FindType(KnownTypeCode.String)),
				new MockImplicitLambda(new[] { compilation.FindType(KnownTypeCode.String) }, compilation.FindType(KnownTypeCode.Int32)),
				new MockImplicitLambda(new[] { compilation.FindType(KnownTypeCode.Int32) }, compilation.FindType(KnownTypeCode.Single))
			};
			bool success;
			Assert.That(
				ti.InferTypeArguments(typeParameters, arguments, parameterTypes, out success),
				Is.EqualTo(new[] {
					compilation.FindType(KnownTypeCode.String),
					compilation.FindType(KnownTypeCode.Int32),
					compilation.FindType(KnownTypeCode.Single)
				}));
			Assert.That(success);
		}

		[Test]
		public void ConvertAllLambdaInference()
		{
			ITypeParameter[] classTypeParameters = { new DefaultTypeParameter(compilation, SymbolKind.TypeDefinition, 0, "T") };
			ITypeParameter[] methodTypeParameters = { new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "R") };

			IType[] parameterTypes = {
				new ParameterizedType(compilation.FindType(typeof(Converter<,>)).GetDefinition(),
					new IType[] { classTypeParameters[0], methodTypeParameters[0] })
			};

			// Signature:  List<T>.ConvertAll<R>(Converter<T, R> converter);
			// Invocation: listOfString.ConvertAll(s => default(int));
			ResolveResult[] arguments = {
				new MockImplicitLambda(new[] { compilation.FindType(KnownTypeCode.String) }, compilation.FindType(KnownTypeCode.Int32))
			};
			IType[] classTypeArguments = {
				compilation.FindType(KnownTypeCode.String)
			};

			bool success;
			Assert.That(
				ti.InferTypeArguments(methodTypeParameters, arguments, parameterTypes, out success, classTypeArguments),
				Is.EqualTo(new[] { compilation.FindType(KnownTypeCode.Int32) }));
			Assert.That(success);
		}

		[Test]
		public void InferFromImplicitAsyncLambda()
		{
			// Signature:  M<T>(Func<int, Task<T>> f)
			// Invocation: M(async x => x + 1);
			// An async lambda's inferred return type is already wrapped in Task<>,
			// so lower-bound inference of Task<int> against Task<T> yields T = int.
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			IType intType = compilation.FindType(KnownTypeCode.Int32);
			IType taskOfInt = new ParameterizedType(compilation.FindType(typeof(System.Threading.Tasks.Task<>)).GetDefinition(), new[] { intType });
			IType[] parameterTypes = {
				new ParameterizedType(compilation.FindType(typeof(Func<,>)).GetDefinition(),
					new IType[] { intType, new ParameterizedType(compilation.FindType(typeof(System.Threading.Tasks.Task<>)).GetDefinition(), new[] { T }) })
			};
			ResolveResult[] arguments = {
				new MockImplicitLambda(new[] { intType }, taskOfInt, isAsync: true)
			};

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T }, arguments, parameterTypes, out success),
				Is.EqualTo(new[] { intType }));
			Assert.That(success);
		}

		[Test]
		public void InferFromExplicitAsyncLambda()
		{
			// Signature:  M<T>(Func<int, Task<T>> f)
			// Invocation: M(async (int x) => x + 1);
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			IType intType = compilation.FindType(KnownTypeCode.Int32);
			IType taskOfInt = new ParameterizedType(compilation.FindType(typeof(System.Threading.Tasks.Task<>)).GetDefinition(), new[] { intType });
			IType[] parameterTypes = {
				new ParameterizedType(compilation.FindType(typeof(Func<,>)).GetDefinition(),
					new IType[] { intType, new ParameterizedType(compilation.FindType(typeof(System.Threading.Tasks.Task<>)).GetDefinition(), new[] { T }) })
			};
			ResolveResult[] arguments = {
				new MockExplicitLambda(new[] { intType }, taskOfInt, isAsync: true)
			};

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T }, arguments, parameterTypes, out success),
				Is.EqualTo(new[] { intType }));
			Assert.That(success);
		}
		#endregion

		[Test]
		public void NullablePick()
		{
			// Signature:  Pick<T>(T? a, T? b)
			// Invocation: Pick(default(int?), default(long?)); -> infers T = long
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeDefinition nullableType = compilation.FindType(KnownTypeCode.NullableOfT).GetDefinition();
			var nullableOfT = new ParameterizedType(nullableType, new[] { T });

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T },
					new[] { new ResolveResult(compilation.FindType(typeof(int?))), new ResolveResult(compilation.FindType(typeof(long?))) },
					new IType[] { nullableOfT, nullableOfT },
					out success),
				Is.EqualTo(new[] { compilation.FindType(KnownTypeCode.Int64) }));
			Assert.That(success);
		}

		[Test]
		public void CoContraPick()
		{
			// Signature:  Pick<T>(ICo<T> a, IContra<T> b)
			// Invocation: Pick(default(ICo<string>), default(IContra<object>));
			//
			// String and Object are both valid choices; and csc ends up picking object,
			// even though the C# specification says it should pick string:
			// 7.5.2.11 Fixing - both string and object are in the candidate set;
			// string has a conversion to object (the other candidate),
			// object doesn't have that; so string should be chosen as the result.
			//
			// We follow the csc behavior.
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeDefinition coType = compilation.FindType(typeof(ICo<>)).GetDefinition();
			ITypeDefinition contraType = compilation.FindType(typeof(IContra<>)).GetDefinition();

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T },
					new[] {
						new ResolveResult(compilation.FindType(typeof(ICo<string>))),
						new ResolveResult(compilation.FindType(typeof(IContra<object>)))
					},
					new IType[] {
						new ParameterizedType(coType, new[] { T }),
						new ParameterizedType(contraType, new[] { T })
					},
					out success),
				Is.EqualTo(new[] { compilation.FindType(KnownTypeCode.Object) }));
			Assert.That(success);
		}

		/// <summary>
		/// Bug 9300 - Unknown Resolve Error
		/// </summary>
		[Test]
		public void TestBug9300()
		{
			// Signature:  Foo<T>(T a, IContra<T> b)
			// Invocation: Foo(new ConvertibleToString(), default(IContra<string>));
			// The lower bound ConvertibleToString and the upper bound string can both
			// only be satisfied by string, via the user-defined implicit conversion.
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeDefinition contraType = compilation.FindType(typeof(IContra<>)).GetDefinition();

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T },
					new[] {
						new ResolveResult(compilation.FindType(typeof(ConvertibleToString))),
						new ResolveResult(compilation.FindType(typeof(IContra<string>)))
					},
					new IType[] {
						T,
						new ParameterizedType(contraType, new[] { T })
					},
					out success),
				Is.EqualTo(new[] { compilation.FindType(KnownTypeCode.String) }));
			Assert.That(success);
		}

		[Test]
		public void GenericArgumentImplicitlyConvertibleToAndFromAnotherTypeList()
		{
			// Signature:  F<K>(IList<K> a, K b)
			// Invocation: F(new List<MyConvertible>(), 1);
			// IList<K> is invariant, so the first argument gives the exact bound
			// MyConvertible; the lower bound int is compatible with it through the
			// user-defined implicit conversion, so inference succeeds.
			var K = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "K");
			ITypeDefinition listType = compilation.FindType(KnownTypeCode.IListOfT).GetDefinition();

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { K },
					new[] {
						new ResolveResult(compilation.FindType(typeof(List<MyConvertible>))),
						new ResolveResult(compilation.FindType(KnownTypeCode.Int32))
					},
					new IType[] {
						new ParameterizedType(listType, new[] { K }),
						K
					},
					out success),
				Is.EqualTo(new[] { compilation.FindType(typeof(MyConvertible)) }));
			Assert.That(success);
		}

		[Test]
		public void GenericArgumentImplicitlyConvertibleToAndFromAnotherTypeIEnumerable()
		{
			// Signature:  F<K>(IEnumerable<K> a, K b)
			// Invocation: F(new List<MyConvertible>(), 1);
			// With the covariant IEnumerable<K> there is no exact bound, only the two
			// lower bounds MyConvertible and int. Since both are implicitly convertible
			// to each other, neither candidate is better and inference fails.
			var K = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "K");
			ITypeDefinition enumerableType = compilation.FindType(KnownTypeCode.IEnumerableOfT).GetDefinition();

			bool success;
			ti.InferTypeArguments(new ITypeParameter[] { K },
				new[] {
					new ResolveResult(compilation.FindType(typeof(List<MyConvertible>))),
					new ResolveResult(compilation.FindType(KnownTypeCode.Int32))
				},
				new IType[] {
					new ParameterizedType(enumerableType, new[] { K }),
					K
				},
				out success);
			Assert.That(!success);
		}

		#region Input type inferences (spec 12.6.3.7)
		[Test]
		public void RefParameterUsesExactInference()
		{
			// Signature:  M<T>(ref List<T> x)
			// Invocation: M(ref listOfString);
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeDefinition listType = compilation.FindType(typeof(List<>)).GetDefinition();

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T },
					new[] { new ByReferenceResolveResult(new ResolveResult(compilation.FindType(typeof(List<string>))), ReferenceKind.Ref) },
					new IType[] { new ByReferenceType(new ParameterizedType(listType, new[] { T })) },
					out success),
				Is.EqualTo(new[] { compilation.FindType(KnownTypeCode.String) }));
			Assert.That(success);
		}

		[Test]
		public void RefParameterDoesNotUseLowerBoundInference()
		{
			// Signature:  M<T>(ref IList<T> x)
			// Invocation: M(ref listOfString); with a List<string> variable
			// A reference parameter requires an exact inference, so the base-type walk
			// of lower-bound inference must not apply and no bound is found for T.
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeDefinition ilistType = compilation.FindType(KnownTypeCode.IListOfT).GetDefinition();

			bool success;
			ti.InferTypeArguments(new ITypeParameter[] { T },
				new[] { new ByReferenceResolveResult(new ResolveResult(compilation.FindType(typeof(List<string>))), ReferenceKind.Ref) },
				new IType[] { new ByReferenceType(new ParameterizedType(ilistType, new[] { T })) },
				out success);
			Assert.That(!success);
		}

		[Test]
		[Ignore("Not implemented: a value argument passed to an 'in' parameter must produce a lower-bound inference (spec 12.6.3.7); currently no bound at all is inferred because every by-reference parameter takes the exact-inference path.")]
		public void InParameterWithValueArgumentUsesLowerBoundInference()
		{
			// Signature:  M<T>(in T x)
			// Invocation: M(5); -> rvalue argument, T = int
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T },
					new[] { new ResolveResult(compilation.FindType(KnownTypeCode.Int32)) },
					new IType[] { new ByReferenceType(T) },
					out success),
				Is.EqualTo(new[] { compilation.FindType(KnownTypeCode.Int32) }));
			Assert.That(success);
		}
		#endregion

		#region Tuple literal inferences (spec 12.6.3.7)
		[Test]
		[Ignore("Not implemented: elementwise input type inference from a tuple literal (spec 12.6.3.7); the literal is currently inferred through its tuple type, which makes conflicting exact element bounds instead of elementwise lower-bound inferences.")]
		public void TupleLiteralInputTypeInference()
		{
			// Signature:  M<T>((T, T) t)
			// Invocation: M((1, 2L)); -> csc infers T = long
			// A tuple literal infers elementwise: a lower-bound inference is made from
			// each element to the corresponding element type, giving the bounds
			// { int, long } and the fixed type long. Treating the literal like a value
			// of type (int, long) would instead produce conflicting exact bounds.
			var comp = RefAssemblyCompilation.Instance;
			var inference = new TypeInference(comp);
			var T = new DefaultTypeParameter(comp, SymbolKind.Method, 0, "T");
			var tupleOfTT = new TupleType(comp, ImmutableArray.Create<IType>(T, T));
			var literal = new TupleResolveResult(comp, ImmutableArray.Create<ResolveResult>(
				new ResolveResult(comp.FindType(KnownTypeCode.Int32)),
				new ResolveResult(comp.FindType(KnownTypeCode.Int64))));

			bool success;
			Assert.That(
				inference.InferTypeArguments(new ITypeParameter[] { T },
					new ResolveResult[] { literal },
					new IType[] { tupleOfTT },
					out success),
				Is.EqualTo(new[] { comp.FindType(KnownTypeCode.Int64) }));
			Assert.That(success);
		}
		#endregion

		#region Tuple element name merging
		// The C# standard does not mention tuple element names in type inference;
		// csc merges names when bounds differ only by them: names are kept where all
		// bounds agree and dropped where they conflict (MergeTupleNames in Roslyn's
		// MethodTypeInference.cs).

		TupleType MakeTupleType(ICompilation comp, params string[] elementNames)
		{
			return new TupleType(comp,
				ImmutableArray.Create(comp.FindType(KnownTypeCode.Int32), comp.FindType(KnownTypeCode.String)),
				ImmutableArray.CreateRange(elementNames));
		}

		[Test]
		public void BestCommonTypeMergesTupleElementNames()
		{
			// var m = cond ? (a: 1, b: "x") : (a: 2, c: "y"); -> (int a, string)
			var comp = RefAssemblyCompilation.Instance;
			var inference = new TypeInference(comp);

			bool success;
			Assert.That(
				inference.GetBestCommonType(new[] {
					new ResolveResult(MakeTupleType(comp, "a", "b")),
					new ResolveResult(MakeTupleType(comp, "a", "c"))
				}, out success),
				Is.EqualTo(MakeTupleType(comp, "a", null)));
			Assert.That(success);
		}

		[Test]
		public void FixingMergesTupleElementNamesOfExactAndLowerBounds()
		{
			// Signature:  M<T>(IList<T> x, T y)
			// Invocation: M(listOfAB, valueAC); -> T = (int a, string)
			var comp = RefAssemblyCompilation.Instance;
			var inference = new TypeInference(comp);
			var T = new DefaultTypeParameter(comp, SymbolKind.Method, 0, "T");
			ITypeDefinition listType = comp.FindType(KnownTypeCode.IListOfT).GetDefinition();

			bool success;
			Assert.That(
				inference.InferTypeArguments(new ITypeParameter[] { T },
					new[] {
						new ResolveResult(new ParameterizedType(listType, new[] { MakeTupleType(comp, "a", "b") })),
						new ResolveResult(MakeTupleType(comp, "a", "c"))
					},
					new IType[] {
						new ParameterizedType(listType, new[] { T }),
						T
					},
					out success),
				Is.EqualTo(new[] { MakeTupleType(comp, "a", null) }));
			Assert.That(success);
		}

		[Test]
		public void FixingMergesNestedTupleElementNames()
		{
			// Signature:  M<T>(T x, T y)
			// Invocation: M(listOfAB, listOfAC);   -> T = IList<(int a, string)>
			//             M(arrayOfAB, arrayOfAC); -> T = (int a, string)[]
			var comp = RefAssemblyCompilation.Instance;
			ITypeDefinition listType = comp.FindType(KnownTypeCode.IListOfT).GetDefinition();

			IType InferSingle(IType argType1, IType argType2)
			{
				var T = new DefaultTypeParameter(comp, SymbolKind.Method, 0, "T");
				var result = new TypeInference(comp).InferTypeArguments(new ITypeParameter[] { T },
					new[] { new ResolveResult(argType1), new ResolveResult(argType2) },
					new IType[] { T, T },
					out bool success);
				Assert.That(success);
				return result.Single();
			}

			Assert.That(
				InferSingle(
					new ParameterizedType(listType, new[] { MakeTupleType(comp, "a", "b") }),
					new ParameterizedType(listType, new[] { MakeTupleType(comp, "a", "c") })),
				Is.EqualTo(new ParameterizedType(listType, new[] { MakeTupleType(comp, "a", null) })));

			Assert.That(
				InferSingle(
					new ArrayType(comp, MakeTupleType(comp, "a", "b")),
					new ArrayType(comp, MakeTupleType(comp, "a", "c"))),
				Is.EqualTo(new ArrayType(comp, MakeTupleType(comp, "a", null))));
		}

		[Test]
		public void FixingMergesTupleElementNamesAcrossLowerAndUpperBounds()
		{
			// Signature:  M<T>(T x, Action<T> y)
			// Invocation: M(listOfAB, actionOfListOfAC); -> T = IList<(int a, string)>
			// Action<in T> is contravariant, so the second argument produces an upper bound
			// while the first produces a lower bound.
			var comp = RefAssemblyCompilation.Instance;
			var inference = new TypeInference(comp);
			var T = new DefaultTypeParameter(comp, SymbolKind.Method, 0, "T");
			ITypeDefinition listType = comp.FindType(KnownTypeCode.IListOfT).GetDefinition();
			ITypeDefinition actionType = comp.FindType(typeof(Action<>)).GetDefinition();
			IType listOfAC = new ParameterizedType(listType, new[] { MakeTupleType(comp, "a", "c") });

			bool success;
			Assert.That(
				inference.InferTypeArguments(new ITypeParameter[] { T },
					new[] {
						new ResolveResult(new ParameterizedType(listType, new[] { MakeTupleType(comp, "a", "b") })),
						new ResolveResult(new ParameterizedType(actionType, new[] { listOfAC }))
					},
					new IType[] {
						T,
						new ParameterizedType(actionType, new IType[] { T })
					},
					out success),
				Is.EqualTo(new[] { new ParameterizedType(listType, new[] { MakeTupleType(comp, "a", null) }) }));
			Assert.That(success);
		}

		[Test]
		public void FixingMergesTupleElementNamesThroughEqualNullabilityAnnotations()
		{
			// Signature:  M<T>(T x, T y)
			// Invocation: M(nullableListOfAB, nullableListOfAC); -> T = IList<(int a, string)>?
			//             M(nullableArrayOfAB, nullableArrayOfAC); -> T = (int a, string)[]?
			var comp = RefAssemblyCompilation.Instance;
			ITypeDefinition listType = comp.FindType(KnownTypeCode.IListOfT).GetDefinition();

			IType InferSingle(IType argType1, IType argType2)
			{
				var T = new DefaultTypeParameter(comp, SymbolKind.Method, 0, "T");
				var result = new TypeInference(comp).InferTypeArguments(new ITypeParameter[] { T },
					new[] { new ResolveResult(argType1), new ResolveResult(argType2) },
					new IType[] { T, T },
					out bool success);
				Assert.That(success);
				return result.Single();
			}

			IType NullableListOf(TupleType elementType)
				=> new ParameterizedType(listType, new[] { elementType }).ChangeNullability(Nullability.Nullable);
			IType NullableArrayOf(TupleType elementType)
				=> new ArrayType(comp, elementType, 1, Nullability.Nullable);

			Assert.That(
				InferSingle(NullableListOf(MakeTupleType(comp, "a", "b")), NullableListOf(MakeTupleType(comp, "a", "c"))),
				Is.EqualTo(NullableListOf(MakeTupleType(comp, "a", null))));

			Assert.That(
				InferSingle(NullableArrayOf(MakeTupleType(comp, "a", "b")), NullableArrayOf(MakeTupleType(comp, "a", "c"))),
				Is.EqualTo(NullableArrayOf(MakeTupleType(comp, "a", null))));
		}

		[Test]
		public void FixingDoesNotMergeBoundsThatDifferInNullability()
		{
			// Signature:  M<T>(T x, T y)
			// Invocation: M(nullableArrayOfString, arrayOfString);
			// Merging nullability requires the variance of the position, which this
			// implementation does not track, so such bounds stay distinct and fixing fails
			// (csc infers string[]?).
			var comp = RefAssemblyCompilation.Instance;
			var T = new DefaultTypeParameter(comp, SymbolKind.Method, 0, "T");
			IType stringType = comp.FindType(KnownTypeCode.String);

			new TypeInference(comp).InferTypeArguments(new ITypeParameter[] { T },
				new[] {
					new ResolveResult(new ArrayType(comp, stringType, 1, Nullability.Nullable)),
					new ResolveResult(new ArrayType(comp, stringType))
				},
				new IType[] { T, T },
				out bool success);
			Assert.That(success, Is.False);
		}

		[Test]
		public void FixingMergesTupleElementNamesOfMultipleExactBounds()
		{
			// Signature:  M<T>(ref T x, ref T y)
			// Invocation: M(ref ab, ref ac); -> T = (int a, string)
			var comp = RefAssemblyCompilation.Instance;
			var inference = new TypeInference(comp);
			var T = new DefaultTypeParameter(comp, SymbolKind.Method, 0, "T");

			bool success;
			Assert.That(
				inference.InferTypeArguments(new ITypeParameter[] { T },
					new[] {
						new ByReferenceResolveResult(new ResolveResult(MakeTupleType(comp, "a", "b")), ReferenceKind.Ref),
						new ByReferenceResolveResult(new ResolveResult(MakeTupleType(comp, "a", "c")), ReferenceKind.Ref)
					},
					new IType[] {
						new ByReferenceType(T),
						new ByReferenceType(T)
					},
					out success),
				Is.EqualTo(new[] { MakeTupleType(comp, "a", null) }));
			Assert.That(success);
		}
		#endregion

		#region Explicit parameter type inferences (spec 12.6.3.9)
		[Test]
		public void ExplicitLambdaParameterTypesGiveExactBounds()
		{
			// Signature:  M<T>(Func<T, bool> f)
			// Invocation: M((string s) => true);
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			IType stringType = compilation.FindType(KnownTypeCode.String);
			IType boolType = compilation.FindType(KnownTypeCode.Boolean);
			IType[] parameterTypes = {
				new ParameterizedType(compilation.FindType(typeof(Func<,>)).GetDefinition(),
					new IType[] { T, boolType })
			};
			ResolveResult[] arguments = {
				new MockExplicitLambda(new[] { stringType }, boolType)
			};

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T }, arguments, parameterTypes, out success),
				Is.EqualTo(new[] { stringType }));
			Assert.That(success);
		}
		#endregion

		#region Exact inferences (spec 12.6.3.10)
		[Test]
		public void ExactInferenceUnwrapsNullable()
		{
			// Signature:  M<T>(ref T? x)
			// Invocation: M(ref nullableInt); -> T = int
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeDefinition nullableType = compilation.FindType(KnownTypeCode.NullableOfT).GetDefinition();

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T },
					new[] { new ByReferenceResolveResult(new ResolveResult(compilation.FindType(typeof(int?))), ReferenceKind.Ref) },
					new IType[] { new ByReferenceType(new ParameterizedType(nullableType, new[] { T })) },
					out success),
				Is.EqualTo(new[] { compilation.FindType(KnownTypeCode.Int32) }));
			Assert.That(success);
		}

		[Test]
		public void ExactInferenceOnArrayElements()
		{
			// Signature:  M<T>(ref T[] x)
			// Invocation: M(ref stringArray); -> T = string
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			IType stringType = compilation.FindType(KnownTypeCode.String);

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T },
					new[] { new ByReferenceResolveResult(new ResolveResult(new ArrayType(compilation, stringType)), ReferenceKind.Ref) },
					new IType[] { new ByReferenceType(new ArrayType(compilation, T)) },
					out success),
				Is.EqualTo(new[] { stringType }));
			Assert.That(success);
		}
		#endregion

		#region Lower-bound inferences (spec 12.6.3.11)
		[Test]
		public void ArrayToCollection()
		{
			ITypeParameter tp = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			IType stringType = compilation.FindType(KnownTypeCode.String);
			ITypeDefinition collectionType = compilation.FindType(KnownTypeCode.ICollectionOfT).GetDefinition();

			bool success;
			Assert.That(
				ti.InferTypeArguments(new[] { tp },
					new[] { new ResolveResult(new ArrayType(compilation, stringType)) },
					new IType[] { new ParameterizedType(collectionType, new[] { tp }) },
					out success),
				Is.EqualTo(new[] { stringType }));
			Assert.That(success);
		}

		[Test]
		public void ArrayToReadOnlyCollection()
		{
			ITypeParameter tp = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			IType stringType = compilation.FindType(KnownTypeCode.String);
			ITypeDefinition rocType = compilation.FindType(KnownTypeCode.IReadOnlyCollectionOfT).GetDefinition();

			bool success;
			Assert.That(
				ti.InferTypeArguments(new[] { tp },
					new[] { new ResolveResult(new ArrayType(compilation, stringType)) },
					new IType[] { new ParameterizedType(rocType, new[] { tp }) },
					out success),
				Is.EqualTo(new[] { stringType }));
			Assert.That(success);
		}

		[Test]
		public void LowerBoundInferenceRequiresUniqueBaseType()
		{
			// Signature:  M<T>(IInv<T> x)
			// Invocation: M(new DoubleImpl()); with DoubleImpl : IInv<int>, IInv<string>
			// No inference is made because the implemented IInv<> instantiation is
			// not unique, so T has no bounds and inference fails.
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeDefinition invType = compilation.FindType(typeof(IInv<>)).GetDefinition();

			bool success;
			ti.InferTypeArguments(new ITypeParameter[] { T },
				new[] { new ResolveResult(compilation.FindType(typeof(DoubleImpl))) },
				new IType[] { new ParameterizedType(invType, new[] { T }) },
				out success);
			Assert.That(!success);
		}

		[Test]
		public void LowerBoundInferenceValueTypeElementIsExact()
		{
			// Signature:  M<T>(IEnumerable<T> a, T b)
			// Invocation: M(intSequence, 2L);
			// Even though IEnumerable<T> is covariant, the element type int is a value
			// type, so an exact inference is made for it. The lower bound long is not
			// implicitly convertible to the exact bound int, so inference fails.
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeDefinition enumerableType = compilation.FindType(KnownTypeCode.IEnumerableOfT).GetDefinition();

			bool success;
			ti.InferTypeArguments(new ITypeParameter[] { T },
				new[] {
					new ResolveResult(compilation.FindType(typeof(IEnumerable<int>))),
					new ResolveResult(compilation.FindType(KnownTypeCode.Int64))
				},
				new IType[] {
					new ParameterizedType(enumerableType, new[] { T }),
					T
				},
				out success);
			Assert.That(!success);
		}
		#endregion

		#region Upper-bound inferences (spec 12.6.3.12)
		[Test]
		public void UpperBoundInferenceKeepsDirectionForCovariance()
		{
			// Signature:  M<T>(IContra<ICo<T>> x)
			// Invocation: M(default(IContra<ICo<string>>)); -> T = string
			// The contravariant outer interface turns the element inference into an
			// upper-bound inference from ICo<string> to ICo<T>; the covariant inner
			// interface keeps the upper-bound direction for T.
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeDefinition contraType = compilation.FindType(typeof(IContra<>)).GetDefinition();
			ITypeDefinition coType = compilation.FindType(typeof(ICo<>)).GetDefinition();

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T },
					new[] { new ResolveResult(compilation.FindType(typeof(IContra<ICo<string>>))) },
					new IType[] { new ParameterizedType(contraType, new IType[] { new ParameterizedType(coType, new[] { T }) }) },
					out success),
				Is.EqualTo(new[] { compilation.FindType(KnownTypeCode.String) }));
			Assert.That(success);
		}

		[Test]
		public void UpperBoundInferenceFlipsToLowerBoundForContravariance()
		{
			// Signature:  M<T>(IContra<IContra<T>> x)
			// Invocation: M(default(IContra<IContra<string>>)); -> T = string
			// Two levels of contravariance: the upper-bound inference from
			// IContra<string> to IContra<T> flips back to a lower-bound inference
			// from string to T.
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeDefinition contraType = compilation.FindType(typeof(IContra<>)).GetDefinition();

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T },
					new[] { new ResolveResult(compilation.FindType(typeof(IContra<IContra<string>>))) },
					new IType[] { new ParameterizedType(contraType, new IType[] { new ParameterizedType(contraType, new[] { T }) }) },
					out success),
				Is.EqualTo(new[] { compilation.FindType(KnownTypeCode.String) }));
			Assert.That(success);
		}

		[Test]
		public void UpperBoundInferenceOnArrayElements()
		{
			// Signature:  M<T>(IContra<T[]> x)
			// Invocation: M(default(IContra<string[]>)); -> T = string
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeDefinition contraType = compilation.FindType(typeof(IContra<>)).GetDefinition();

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T },
					new[] { new ResolveResult(compilation.FindType(typeof(IContra<string[]>))) },
					new IType[] { new ParameterizedType(contraType, new IType[] { new ArrayType(compilation, T) }) },
					out success),
				Is.EqualTo(new[] { compilation.FindType(KnownTypeCode.String) }));
			Assert.That(success);
		}

		[Test]
		public void UpperBoundInferenceFromArrayInterfaceToArray()
		{
			// Signature:  M<T>(IContra<T[]> x)
			// Invocation: M(default(IContra<IEnumerable<string>>)); -> T = string
			// Upper-bound inference from IEnumerable<string> to T[] uses the
			// array-interface rule elementwise.
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeDefinition contraType = compilation.FindType(typeof(IContra<>)).GetDefinition();

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T },
					new[] { new ResolveResult(compilation.FindType(typeof(IContra<IEnumerable<string>>))) },
					new IType[] { new ParameterizedType(contraType, new IType[] { new ArrayType(compilation, T) }) },
					out success),
				Is.EqualTo(new[] { compilation.FindType(KnownTypeCode.String) }));
			Assert.That(success);
		}

		[Test]
		public void UpperBoundInferenceUnwrapsNullable()
		{
			// Signature:  M<T>(IContra<T?> x)
			// Invocation: M(default(IContra<int?>)); -> T = int
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeDefinition contraType = compilation.FindType(typeof(IContra<>)).GetDefinition();
			ITypeDefinition nullableType = compilation.FindType(KnownTypeCode.NullableOfT).GetDefinition();

			bool success;
			Assert.That(
				ti.InferTypeArguments(new ITypeParameter[] { T },
					new[] { new ResolveResult(compilation.FindType(typeof(IContra<int?>))) },
					new IType[] { new ParameterizedType(contraType, new IType[] { new ParameterizedType(nullableType, new[] { T }) }) },
					out success),
				Is.EqualTo(new[] { compilation.FindType(KnownTypeCode.Int32) }));
			Assert.That(success);
		}
		#endregion

		#region Fixing (spec 12.6.3.13)
		[Test]
		public void FixingFailsOnConflictingExactBounds()
		{
			// Signature:  M<T>(ref List<T> a, ref List<T> b)
			// Invocation: M(ref listOfString, ref listOfObject);
			var T = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeDefinition listType = compilation.FindType(typeof(List<>)).GetDefinition();
			var refListOfT = new ByReferenceType(new ParameterizedType(listType, new[] { T }));

			bool success;
			ti.InferTypeArguments(new ITypeParameter[] { T },
				new[] {
					new ByReferenceResolveResult(new ResolveResult(compilation.FindType(typeof(List<string>))), ReferenceKind.Ref),
					new ByReferenceResolveResult(new ResolveResult(compilation.FindType(typeof(List<object>))), ReferenceKind.Ref)
				},
				new IType[] { refListOfT, refListOfT },
				out success);
			Assert.That(!success);
		}
		#endregion

		#region Best common type (spec 12.6.3.17)
		[Test]
		public void BestCommonTypeIntAndShort()
		{
			bool success;
			Assert.That(
				ti.GetBestCommonType(new[] {
					new ResolveResult(compilation.FindType(KnownTypeCode.Int16)),
					new ResolveResult(compilation.FindType(KnownTypeCode.Int32))
				}, out success),
				Is.EqualTo(compilation.FindType(KnownTypeCode.Int32)));
			Assert.That(success);
		}

		[Test]
		public void BestCommonTypeNullAndString()
		{
			bool success;
			Assert.That(
				ti.GetBestCommonType(new[] {
					new ResolveResult(SpecialType.NullType),
					new ResolveResult(compilation.FindType(KnownTypeCode.String))
				}, out success),
				Is.EqualTo(compilation.FindType(KnownTypeCode.String)));
			Assert.That(success);
		}

		[Test]
		public void BestCommonTypeNullAndInt()
		{
			Assert.That(
				ti.GetBestCommonType(new[] {
					new ResolveResult(SpecialType.NullType),
					new ResolveResult(compilation.FindType(KnownTypeCode.Int32))
				}, out bool success),
				Is.EqualTo(compilation.FindType(KnownTypeCode.Int32)));
			// By my read of the C# spec, the best common type is really the non-nullable `int`.
			// It's only a following step that will report an error if the argument expressions
			// are not convertible to the common type.
			Assert.That(success);
		}

		[Test]
		public void BestCommonTypeStringAndObject()
		{
			bool success;
			Assert.That(
				ti.GetBestCommonType(new[] {
					new ResolveResult(compilation.FindType(KnownTypeCode.String)),
					new ResolveResult(compilation.FindType(KnownTypeCode.Object))
				}, out success),
				Is.EqualTo(compilation.FindType(KnownTypeCode.Object)));
			Assert.That(success);
		}

		[Test]
		public void BestCommonTypeStringAndDynamic()
		{
			Assert.That(
				ti.GetBestCommonType(new[] {
					new ResolveResult(compilation.FindType(KnownTypeCode.String)),
					new ResolveResult(SpecialType.Dynamic)
				}, out bool success),
				Is.EqualTo(SpecialType.Dynamic));
			Assert.That(success);
		}

		[Test]
		public void BestCommonTypeObjectAndDynamic()
		{
			Assert.That(
				ti.GetBestCommonType(new[] {
					new ResolveResult(compilation.FindType(KnownTypeCode.Object)),
					new ResolveResult(SpecialType.Dynamic)
				}, out bool success),
				Is.EqualTo(SpecialType.Dynamic));
			Assert.That(success);
		}

		[Test]
		public void BestCommonTypeDynamicAndObject()
		{
			Assert.That(
				ti.GetBestCommonType(new[] {
					new ResolveResult(SpecialType.Dynamic),
					new ResolveResult(compilation.FindType(KnownTypeCode.Object))
				}, out bool success),
				Is.EqualTo(SpecialType.Dynamic));
			Assert.That(success);
		}
		#endregion

		#region FindTypeInBounds
		IType[] Resolve(params Type[] types)
		{
			IType[] r = new IType[types.Length];
			for (int i = 0; i < types.Length; i++)
			{
				r[i] = compilation.FindType(types[i]);
				Assert.That(r[i], Is.Not.SameAs(SpecialType.UnknownType));
			}
			Array.Sort(r, (a, b) => a.ReflectionName.CompareTo(b.ReflectionName));
			return r;
		}

		IType[] FindAllTypesInBounds(IReadOnlyList<IType> lowerBounds, IReadOnlyList<IType> upperBounds = null)
		{
			ti.Algorithm = TypeInferenceAlgorithm.ImprovedReturnAllResults;
			IType type = ti.FindTypeInBounds(lowerBounds, upperBounds ?? new IType[0]);
			return ExpandIntersections(type).OrderBy(t => t.ReflectionName).ToArray();
		}

		static IEnumerable<IType> ExpandIntersections(IType type)
		{
			if (type is IntersectionType it)
			{
				return it.Types.SelectMany(t => ExpandIntersections(t));
			}
			if (type is ParameterizedType pt)
			{
				IType[][] typeArguments = new IType[pt.TypeArguments.Count][];
				for (int i = 0; i < typeArguments.Length; i++)
				{
					typeArguments[i] = ExpandIntersections(pt.TypeArguments[i]).ToArray();
				}
				return AllCombinations(typeArguments).Select(ta => new ParameterizedType(pt.GetDefinition(), ta));
			}
			return new[] { type };
		}

		/// <summary>
		/// Performs the combinatorial explosion.
		/// </summary>
		static IEnumerable<IType[]> AllCombinations(IType[][] typeArguments)
		{
			int[] index = new int[typeArguments.Length];
			index[typeArguments.Length - 1] = -1;
			while (true)
			{
				int i;
				for (i = index.Length - 1; i >= 0; i--)
				{
					if (++index[i] == typeArguments[i].Length)
						index[i] = 0;
					else
						break;
				}
				if (i < 0)
					break;
				IType[] r = new IType[typeArguments.Length];
				for (i = 0; i < r.Length; i++)
				{
					r[i] = typeArguments[i][index[i]];
				}
				yield return r;
			}
		}

		[Test]
		public void ListOfShortAndInt()
		{
			Assert.That(
				FindAllTypesInBounds(Resolve(typeof(List<short>), typeof(List<int>))),
				Is.EqualTo(Resolve(typeof(IList))));
		}

		[Test]
		public void ListOfStringAndObject()
		{
			// The covariant IReadOnlyList<object> (added in .NET 4.5) is more specific than
			// IEnumerable<object>, so it replaces it in the result set.
			Assert.That(
				FindAllTypesInBounds(Resolve(typeof(List<string>), typeof(List<object>))),
				Is.EqualTo(Resolve(typeof(IList), typeof(IReadOnlyList<object>))));
		}

		[Test]
		public void ListOfListOfStringAndObject()
		{
			// As in ListOfStringAndObject, the covariant IReadOnlyList<T> replaces IEnumerable<T>
			// on both nesting levels.
			Assert.That(
				FindAllTypesInBounds(Resolve(typeof(List<List<string>>), typeof(List<List<object>>))),
				Is.EqualTo(Resolve(typeof(IList), typeof(IReadOnlyList<IList>), typeof(IReadOnlyList<IReadOnlyList<object>>))));
		}

		[Test]
		public void ShortAndInt()
		{
			Assert.That(
				FindAllTypesInBounds(Resolve(typeof(short), typeof(int))),
				Is.EqualTo(Resolve(typeof(int))));
		}

		[Test]
		public void StringAndVersion()
		{
			Assert.That(
				FindAllTypesInBounds(Resolve(typeof(string), typeof(Version))),
				Is.EqualTo(Resolve(typeof(ICloneable), typeof(IComparable))));
		}

		[Test]
		public void CommonSubTypeClonableComparable()
		{
			Assert.That(
				FindAllTypesInBounds(Resolve(), Resolve(typeof(ICloneable), typeof(IComparable))),
				Is.EqualTo(Resolve(typeof(string), typeof(Version))));
		}

		[Test]
		public void EnumerableOfStringAndVersion()
		{
			Assert.That(
				FindAllTypesInBounds(Resolve(typeof(IList<string>), typeof(IList<Version>))),
				Is.EqualTo(Resolve(typeof(IEnumerable<ICloneable>), typeof(IEnumerable<IComparable>))));
		}

		[Test]
		public void CommonSubTypeIEnumerableClonableIEnumerableComparable()
		{
			Assert.That(
				FindAllTypesInBounds(Resolve(), Resolve(typeof(IEnumerable<ICloneable>), typeof(IEnumerable<IComparable>))),
				Is.EqualTo(Resolve(typeof(IEnumerable<string>), typeof(IEnumerable<Version>))));
		}

		[Test]
		public void CommonSubTypeIEnumerableClonableIEnumerableComparableList()
		{
			// ReadOnlyCollectionBuilder<T> appears because the test compilation includes
			// System.Core, which declares it as another public implementation of both
			// IList and IList<T>.
			Assert.That(
				FindAllTypesInBounds(Resolve(), Resolve(typeof(IEnumerable<ICloneable>), typeof(IEnumerable<IComparable>), typeof(IList))),
				Is.EqualTo(Resolve(typeof(List<string>), typeof(List<Version>), typeof(Collection<string>), typeof(Collection<Version>), typeof(ReadOnlyCollection<string>), typeof(ReadOnlyCollection<Version>), typeof(System.Runtime.CompilerServices.ReadOnlyCollectionBuilder<string>), typeof(System.Runtime.CompilerServices.ReadOnlyCollectionBuilder<Version>))));
		}
		#endregion

		#region First-class span type inference
		IType[] InferSpan(Func<ICompilation, ITypeParameter, IType[]> parameterTypes,
			Func<ICompilation, ResolveResult[]> arguments, out bool success)
		{
			var c = RefAssemblyCompilation.Instance;
			var inference = new TypeInference(c);
			ITypeParameter tp = new DefaultTypeParameter(c, SymbolKind.Method, 0, "T");
			return inference.InferTypeArguments(new[] { tp }, arguments(c), parameterTypes(c, tp), out success);
		}

		static ParameterizedType SpanOf(ICompilation c, IType element)
			=> new ParameterizedType(c.FindType(KnownTypeCode.SpanOfT).GetDefinition(), new[] { element });

		static ParameterizedType ReadOnlySpanOf(ICompilation c, IType element)
			=> new ParameterizedType(c.FindType(KnownTypeCode.ReadOnlySpanOfT).GetDefinition(), new[] { element });

		[Test]
		public void SpanArgumentAloneInfersItsElementType()
		{
			bool success;
			Assert.That(
				InferSpan(
					(c, tp) => new IType[] { SpanOf(c, tp) },
					c => new[] { new ResolveResult(SpanOf(c, c.FindType(KnownTypeCode.String))) },
					out success),
				Is.EqualTo(new[] { RefAssemblyCompilation.Instance.FindType(KnownTypeCode.String) }));
			Assert.That(success);
		}

		[Test]
		public void SpanArgumentGivesAnExactBound_ConflictingLowerBoundFailsInference()
		{
			// M<T>(Span<T>, T) called with (Span<string>, object): Span<T> is invariant, so the
			// span argument contributes an EXACT bound (C# 14 spec, 12.6.3.10: "If V is a
			// Span<V1>, then an exact inference is made"). The conflicting lower bound object
			// must fail inference; Roslyn reports CS0411 for this call.
			bool success;
			InferSpan(
				(c, tp) => new IType[] { SpanOf(c, tp), tp },
				c => new[] {
					new ResolveResult(SpanOf(c, c.FindType(KnownTypeCode.String))),
					new ResolveResult(c.FindType(KnownTypeCode.Object))
				},
				out success);
			Assert.That(success, Is.False);
		}

		[Test]
		public void ArrayArgumentForSpanParameterGivesAnExactBound_ConflictingLowerBoundFailsInference()
		{
			// Same as above with a string[] argument: the array-to-Span conversion requires
			// identity element types, so the bound is exact. Roslyn reports CS0411.
			bool success;
			InferSpan(
				(c, tp) => new IType[] { SpanOf(c, tp), tp },
				c => new[] {
					new ResolveResult(new ArrayType(c, c.FindType(KnownTypeCode.String))),
					new ResolveResult(c.FindType(KnownTypeCode.Object))
				},
				out success);
			Assert.That(success, Is.False);
		}

		[Test]
		public void SpanArgumentForReadOnlySpanParameterGivesALowerBound()
		{
			// M<T>(ReadOnlySpan<T>, T) called with (Span<string>, object): ReadOnlySpan is
			// covariance-convertible, the span argument contributes a LOWER bound, and T=object
			// wins. Roslyn compiles this with T=object.
			bool success;
			Assert.That(
				InferSpan(
					(c, tp) => new IType[] { ReadOnlySpanOf(c, tp), tp },
					c => new[] {
						new ResolveResult(SpanOf(c, c.FindType(KnownTypeCode.String))),
						new ResolveResult(c.FindType(KnownTypeCode.Object))
					},
					out success),
				Is.EqualTo(new[] { RefAssemblyCompilation.Instance.FindType(KnownTypeCode.Object) }));
			Assert.That(success);
		}

		[Test]
		public void ArrayArgumentForReadOnlySpanParameterGivesALowerBound()
		{
			bool success;
			Assert.That(
				InferSpan(
					(c, tp) => new IType[] { ReadOnlySpanOf(c, tp), tp },
					c => new[] {
						new ResolveResult(new ArrayType(c, c.FindType(KnownTypeCode.String))),
						new ResolveResult(c.FindType(KnownTypeCode.Object))
					},
					out success),
				Is.EqualTo(new[] { RefAssemblyCompilation.Instance.FindType(KnownTypeCode.Object) }));
			Assert.That(success);
		}
		#endregion
	}
}
