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
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Resolver;
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
	}
}
