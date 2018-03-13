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
using System.Text;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp.Resolver
{
	sealed class CSharpOperators
	{
		readonly ICompilation compilation;
		
		private CSharpOperators(ICompilation compilation)
		{
			this.compilation = compilation;
			InitParameterArrays();
		}
		
		/// <summary>
		/// Gets the CSharpOperators instance for the specified <see cref="ICompilation"/>.
		/// This will make use of the context's cache manager (if available) to reuse the CSharpOperators instance.
		/// </summary>
		public static CSharpOperators Get(ICompilation compilation)
		{
			var cache = compilation.CacheManager;
			var operators = (CSharpOperators)cache.GetShared(typeof(CSharpOperators));
			if (operators == null) {
				operators = (CSharpOperators)cache.GetOrAddShared(typeof(CSharpOperators), new CSharpOperators(compilation));
			}
			return operators;
		}
		
		#region class OperatorMethod
		OperatorMethod[] Lift(params OperatorMethod[] methods)
		{
			var result = new List<OperatorMethod>(methods);
			foreach (var method in methods) {
				var lifted = method.Lift(this);
				if (lifted != null)
					result.Add(lifted);
			}
			return result.ToArray();
		}

		readonly IParameter[] normalParameters = new IParameter[(int)(TypeCode.String + 1 - TypeCode.Object)];
		readonly IParameter[] nullableParameters = new IParameter[(int)(TypeCode.Decimal + 1 - TypeCode.Boolean)];
		
		void InitParameterArrays()
		{
			for (var i = TypeCode.Object; i <= TypeCode.String; i++) {
				normalParameters[i - TypeCode.Object] = new DefaultParameter(compilation.FindType(i), string.Empty);
			}
			for (var i = TypeCode.Boolean; i <= TypeCode.Decimal; i++) {
				var type = NullableType.Create(compilation, compilation.FindType(i));
				nullableParameters[i - TypeCode.Boolean] = new DefaultParameter(type, string.Empty);
			}
		}
		
		IParameter MakeParameter(TypeCode code)
		{
			return normalParameters[code - TypeCode.Object];
		}
		
		IParameter MakeNullableParameter(IParameter normalParameter)
		{
			for (var i = TypeCode.Boolean; i <= TypeCode.Decimal; i++) {
				if (normalParameter == normalParameters[i - TypeCode.Object])
					return nullableParameters[i - TypeCode.Boolean];
			}
			throw new ArgumentException();
		}
		
		internal class OperatorMethod : IParameterizedMember
		{
			internal readonly List<IParameter> parameters = new List<IParameter>();
			
			protected OperatorMethod(ICompilation compilation)
			{
				this.Compilation = compilation;
			}
			
			public IReadOnlyList<IParameter> Parameters => parameters;

			public IType ReturnType { get; internal set; }
			
			public ICompilation Compilation { get; }

			public virtual OperatorMethod Lift(CSharpOperators operators)
			{
				return null;
			}
			
			ITypeDefinition IEntity.DeclaringTypeDefinition => null;

			IType IEntity.DeclaringType => null;

			IMember IMember.MemberDefinition => this;

			IUnresolvedMember IMember.UnresolvedMember => null;

			IReadOnlyList<IMember> IMember.ImplementedInterfaceMembers => EmptyList<IMember>.Instance;

			bool IMember.IsVirtual => false;

			bool IMember.IsOverride => false;

			bool IMember.IsOverridable => false;

			SymbolKind ISymbol.SymbolKind => SymbolKind.Operator;

			IReadOnlyList<IAttribute> IEntity.Attributes => EmptyList<IAttribute>.Instance;

			Accessibility IHasAccessibility.Accessibility => Accessibility.Public;

			bool IEntity.IsStatic => true;

			bool IEntity.IsAbstract => false;

			bool IEntity.IsSealed => false;

			bool IEntity.IsShadowing => false;

			bool IEntity.IsSynthetic => true;

			bool IHasAccessibility.IsPrivate => false;

			bool IHasAccessibility.IsPublic => true;

			bool IHasAccessibility.IsProtected => false;

			bool IHasAccessibility.IsInternal => false;

			bool IHasAccessibility.IsProtectedOrInternal => false;

			bool IHasAccessibility.IsProtectedAndInternal => false;

			bool IMember.IsExplicitInterfaceImplementation => false;

			IAssembly IEntity.ParentAssembly => Compilation.MainAssembly;

			ISymbolReference ISymbol.ToReference()
			{
				throw new NotSupportedException();
			}
			
			IMemberReference IMember.ToReference()
			{
				throw new NotSupportedException();
			}

			TypeParameterSubstitution IMember.Substitution => TypeParameterSubstitution.Identity;

			IMember IMember.Specialize(TypeParameterSubstitution substitution)
			{
				if (TypeParameterSubstitution.Identity.Equals(substitution))
					return this;
				throw new NotSupportedException();
			}

			string INamedElement.FullName => "operator";

			public string Name => "operator";

			string INamedElement.Namespace => string.Empty;

			string INamedElement.ReflectionName => "operator";

			public override string ToString()
			{
				var b = new StringBuilder();
				b.Append(ReturnType + " operator(");
				for (var i = 0; i < parameters.Count; i++) {
					if (i > 0)
						b.Append(", ");
					b.Append(parameters[i].Type);
				}
				b.Append(')');
				return b.ToString();
			}
		}
		#endregion
		
		#region Unary operator class definitions
		internal class UnaryOperatorMethod : OperatorMethod
		{
			public virtual bool CanEvaluateAtCompileTime => false;

			public virtual object Invoke(CSharpResolver resolver, object input)
			{
				throw new NotSupportedException();
			}
			
			public UnaryOperatorMethod(ICompilation compilaton) : base(compilaton)
			{
			}
		}
		
		sealed class LambdaUnaryOperatorMethod<T> : UnaryOperatorMethod
		{
			readonly Func<T, T> func;
			
			public LambdaUnaryOperatorMethod(CSharpOperators operators, Func<T, T> func)
				: base(operators.compilation)
			{
				var typeCode = Type.GetTypeCode(typeof(T));
				this.ReturnType = operators.compilation.FindType(typeCode);
				parameters.Add(operators.MakeParameter(typeCode));
				this.func = func;
			}
			
			public override bool CanEvaluateAtCompileTime => true;

			public override object Invoke(CSharpResolver resolver, object input)
			{
				if (input == null)
					return null;
				return func((T)resolver.CSharpPrimitiveCast(Type.GetTypeCode(typeof(T)), input));
			}
			
			public override OperatorMethod Lift(CSharpOperators operators)
			{
				return new LiftedUnaryOperatorMethod(operators, this);
			}
		}
		
		sealed class LiftedUnaryOperatorMethod : UnaryOperatorMethod, ILiftedOperator
		{
			readonly UnaryOperatorMethod baseMethod;
			
			public LiftedUnaryOperatorMethod(CSharpOperators operators, UnaryOperatorMethod baseMethod) : base(operators.compilation)
			{
				this.baseMethod = baseMethod;
				this.ReturnType = NullableType.Create(baseMethod.Compilation, baseMethod.ReturnType);
				parameters.Add(operators.MakeNullableParameter(baseMethod.Parameters[0]));
			}

			public IReadOnlyList<IParameter> NonLiftedParameters => baseMethod.Parameters;
			public IType NonLiftedReturnType => baseMethod.ReturnType;
		}
		#endregion
		
		#region Unary operator definitions
		// C# 4.0 spec: §7.7.1 Unary plus operator
		OperatorMethod[] unaryPlusOperators;
		
		public OperatorMethod[] UnaryPlusOperators {
			get {
				var ops = LazyInit.VolatileRead(ref unaryPlusOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref unaryPlusOperators, Lift(
						new LambdaUnaryOperatorMethod<int>    (this, i => +i),
						new LambdaUnaryOperatorMethod<uint>   (this, i => +i),
						new LambdaUnaryOperatorMethod<long>   (this, i => +i),
						new LambdaUnaryOperatorMethod<ulong>  (this, i => +i),
						new LambdaUnaryOperatorMethod<float>  (this, i => +i),
						new LambdaUnaryOperatorMethod<double> (this, i => +i),
						new LambdaUnaryOperatorMethod<decimal>(this, i => +i)
					));
				}
			}
		}
		
		// C# 4.0 spec: §7.7.2 Unary minus operator
		OperatorMethod[] uncheckedUnaryMinusOperators;
		
		public OperatorMethod[] UncheckedUnaryMinusOperators {
			get {
				var ops = LazyInit.VolatileRead(ref uncheckedUnaryMinusOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref uncheckedUnaryMinusOperators, Lift(
						new LambdaUnaryOperatorMethod<int>    (this, i => unchecked(-i)),
						new LambdaUnaryOperatorMethod<long>   (this, i => unchecked(-i)),
						new LambdaUnaryOperatorMethod<float>  (this, i => unchecked(-i)),
						new LambdaUnaryOperatorMethod<double> (this, i => unchecked(-i)),
						new LambdaUnaryOperatorMethod<decimal>(this, i => unchecked(-i))
					));
				}
			}
		}
		
		OperatorMethod[] checkedUnaryMinusOperators;
		
		public OperatorMethod[] CheckedUnaryMinusOperators {
			get {
				var ops = LazyInit.VolatileRead(ref checkedUnaryMinusOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref checkedUnaryMinusOperators, Lift(
						new LambdaUnaryOperatorMethod<int>    (this, i => checked(-i)),
						new LambdaUnaryOperatorMethod<long>   (this, i => checked(-i)),
						new LambdaUnaryOperatorMethod<float>  (this, i => checked(-i)),
						new LambdaUnaryOperatorMethod<double> (this, i => checked(-i)),
						new LambdaUnaryOperatorMethod<decimal>(this, i => checked(-i))
					));
				}
			}
		}
		
		// C# 4.0 spec: §7.7.3 Logical negation operator
		OperatorMethod[] logicalNegationOperators;
		
		public OperatorMethod[] LogicalNegationOperators {
			get {
				var ops = LazyInit.VolatileRead(ref logicalNegationOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref logicalNegationOperators, Lift(
						new LambdaUnaryOperatorMethod<bool>(this, b => !b)
					));
				}
			}
		}
		
		// C# 4.0 spec: §7.7.4 Bitwise complement operator
		OperatorMethod[] bitwiseComplementOperators;
		
		public OperatorMethod[] BitwiseComplementOperators {
			get {
				var ops = LazyInit.VolatileRead(ref bitwiseComplementOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref bitwiseComplementOperators, Lift(
						new LambdaUnaryOperatorMethod<int>  (this, i => ~i),
						new LambdaUnaryOperatorMethod<uint> (this, i => ~i),
						new LambdaUnaryOperatorMethod<long> (this, i => ~i),
						new LambdaUnaryOperatorMethod<ulong>(this, i => ~i)
					));
				}
			}
		}
		#endregion
		
		#region Binary operator class definitions
		internal class BinaryOperatorMethod : OperatorMethod
		{
			public virtual bool CanEvaluateAtCompileTime => false;

			public virtual object Invoke(CSharpResolver resolver, object lhs, object rhs) {
				throw new NotSupportedException();
			}
			
			public BinaryOperatorMethod(ICompilation compilation) : base(compilation) {}
		}
		
		sealed class LambdaBinaryOperatorMethod<T1, T2> : BinaryOperatorMethod
		{
			readonly Func<T1, T2, T1> checkedFunc;
			readonly Func<T1, T2, T1> uncheckedFunc;
			
			public LambdaBinaryOperatorMethod(CSharpOperators operators, Func<T1, T2, T1> func)
				: this(operators, func, func)
			{
			}
			
			public LambdaBinaryOperatorMethod(CSharpOperators operators, Func<T1, T2, T1> checkedFunc, Func<T1, T2, T1> uncheckedFunc)
				: base(operators.compilation)
			{
				var t1 = Type.GetTypeCode(typeof(T1));
				this.ReturnType = operators.compilation.FindType(t1);
				parameters.Add(operators.MakeParameter(t1));
				parameters.Add(operators.MakeParameter(Type.GetTypeCode(typeof(T2))));
				this.checkedFunc = checkedFunc;
				this.uncheckedFunc = uncheckedFunc;
			}
			
			public override bool CanEvaluateAtCompileTime => true;

			public override object Invoke(CSharpResolver resolver, object lhs, object rhs)
			{
				if (lhs == null || rhs == null)
					return null;
				var func = resolver.CheckForOverflow ? checkedFunc : uncheckedFunc;
				return func((T1)resolver.CSharpPrimitiveCast(Type.GetTypeCode(typeof(T1)), lhs),
				            (T2)resolver.CSharpPrimitiveCast(Type.GetTypeCode(typeof(T2)), rhs));
			}
			
			public override OperatorMethod Lift(CSharpOperators operators)
			{
				return new LiftedBinaryOperatorMethod(operators, this);
			}
		}
		
		sealed class LiftedBinaryOperatorMethod : BinaryOperatorMethod, ILiftedOperator
		{
			readonly BinaryOperatorMethod baseMethod;
			
			public LiftedBinaryOperatorMethod(CSharpOperators operators, BinaryOperatorMethod baseMethod)
				: base(operators.compilation)
			{
				this.baseMethod = baseMethod;
				this.ReturnType = NullableType.Create(operators.compilation, baseMethod.ReturnType);
				parameters.Add(operators.MakeNullableParameter(baseMethod.Parameters[0]));
				parameters.Add(operators.MakeNullableParameter(baseMethod.Parameters[1]));
			}

			public IReadOnlyList<IParameter> NonLiftedParameters => baseMethod.Parameters;
			public IType NonLiftedReturnType => baseMethod.ReturnType;
		}
		#endregion
		
		#region Arithmetic operators
		// C# 4.0 spec: §7.8.1 Multiplication operator
		
		OperatorMethod[] multiplicationOperators;
		
		public OperatorMethod[] MultiplicationOperators {
			get {
				var ops = LazyInit.VolatileRead(ref multiplicationOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref multiplicationOperators, Lift(
						new LambdaBinaryOperatorMethod<int,     int>    (this, (a, b) => checked(a * b), (a, b) => unchecked(a * b)),
						new LambdaBinaryOperatorMethod<uint,    uint>   (this, (a, b) => checked(a * b), (a, b) => unchecked(a * b)),
						new LambdaBinaryOperatorMethod<long,    long>   (this, (a, b) => checked(a * b), (a, b) => unchecked(a * b)),
						new LambdaBinaryOperatorMethod<ulong,   ulong>  (this, (a, b) => checked(a * b), (a, b) => unchecked(a * b)),
						new LambdaBinaryOperatorMethod<float,   float>  (this, (a, b) => checked(a * b), (a, b) => unchecked(a * b)),
						new LambdaBinaryOperatorMethod<double,  double> (this, (a, b) => checked(a * b), (a, b) => unchecked(a * b)),
						new LambdaBinaryOperatorMethod<decimal, decimal>(this, (a, b) => checked(a * b), (a, b) => unchecked(a * b))
					));
				}
			}
		}
		
		// C# 4.0 spec: §7.8.2 Division operator
		OperatorMethod[] divisionOperators;
		
		public OperatorMethod[] DivisionOperators {
			get {
				var ops = LazyInit.VolatileRead(ref divisionOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref divisionOperators, Lift(
						new LambdaBinaryOperatorMethod<int,     int>    (this, (a, b) => checked(a / b), (a, b) => unchecked(a / b)),
						new LambdaBinaryOperatorMethod<uint,    uint>   (this, (a, b) => checked(a / b), (a, b) => unchecked(a / b)),
						new LambdaBinaryOperatorMethod<long,    long>   (this, (a, b) => checked(a / b), (a, b) => unchecked(a / b)),
						new LambdaBinaryOperatorMethod<ulong,   ulong>  (this, (a, b) => checked(a / b), (a, b) => unchecked(a / b)),
						new LambdaBinaryOperatorMethod<float,   float>  (this, (a, b) => checked(a / b), (a, b) => unchecked(a / b)),
						new LambdaBinaryOperatorMethod<double,  double> (this, (a, b) => checked(a / b), (a, b) => unchecked(a / b)),
						new LambdaBinaryOperatorMethod<decimal, decimal>(this, (a, b) => checked(a / b), (a, b) => unchecked(a / b))
					));
				}
			}
		}
		
		// C# 4.0 spec: §7.8.3 Remainder operator
		OperatorMethod[] remainderOperators;
		
		public OperatorMethod[] RemainderOperators {
			get {
				var ops = LazyInit.VolatileRead(ref remainderOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref remainderOperators, Lift(
						new LambdaBinaryOperatorMethod<int,     int>    (this, (a, b) => checked(a % b), (a, b) => unchecked(a % b)),
						new LambdaBinaryOperatorMethod<uint,    uint>   (this, (a, b) => checked(a % b), (a, b) => unchecked(a % b)),
						new LambdaBinaryOperatorMethod<long,    long>   (this, (a, b) => checked(a % b), (a, b) => unchecked(a % b)),
						new LambdaBinaryOperatorMethod<ulong,   ulong>  (this, (a, b) => checked(a % b), (a, b) => unchecked(a % b)),
						new LambdaBinaryOperatorMethod<float,   float>  (this, (a, b) => checked(a % b), (a, b) => unchecked(a % b)),
						new LambdaBinaryOperatorMethod<double,  double> (this, (a, b) => checked(a % b), (a, b) => unchecked(a % b)),
						new LambdaBinaryOperatorMethod<decimal, decimal>(this, (a, b) => checked(a % b), (a, b) => unchecked(a % b))
					));
				}
			}
		}
		
		// C# 4.0 spec: §7.8.3 Addition operator
		OperatorMethod[] additionOperators;
		
		public OperatorMethod[] AdditionOperators {
			get {
				var ops = LazyInit.VolatileRead(ref additionOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref additionOperators, Lift(
						new LambdaBinaryOperatorMethod<int,     int>    (this, (a, b) => checked(a + b), (a, b) => unchecked(a + b)),
						new LambdaBinaryOperatorMethod<uint,    uint>   (this, (a, b) => checked(a + b), (a, b) => unchecked(a + b)),
						new LambdaBinaryOperatorMethod<long,    long>   (this, (a, b) => checked(a + b), (a, b) => unchecked(a + b)),
						new LambdaBinaryOperatorMethod<ulong,   ulong>  (this, (a, b) => checked(a + b), (a, b) => unchecked(a + b)),
						new LambdaBinaryOperatorMethod<float,   float>  (this, (a, b) => checked(a + b), (a, b) => unchecked(a + b)),
						new LambdaBinaryOperatorMethod<double,  double> (this, (a, b) => checked(a + b), (a, b) => unchecked(a + b)),
						new LambdaBinaryOperatorMethod<decimal, decimal>(this, (a, b) => checked(a + b), (a, b) => unchecked(a + b)),
						new StringConcatenation(this, TypeCode.String, TypeCode.String),
						new StringConcatenation(this, TypeCode.String, TypeCode.Object),
						new StringConcatenation(this, TypeCode.Object, TypeCode.String)
					));
				}
			}
		}
		
		// not in this list, but handled manually: enum addition, delegate combination
		sealed class StringConcatenation : BinaryOperatorMethod
		{
			public StringConcatenation(CSharpOperators operators, TypeCode p1, TypeCode p2)
				: base(operators.compilation)
			{
				this.CanEvaluateAtCompileTime = p1 == TypeCode.String && p2 == TypeCode.String;
				this.ReturnType = operators.compilation.FindType(KnownTypeCode.String);
				parameters.Add(operators.MakeParameter(p1));
				parameters.Add(operators.MakeParameter(p2));
			}
			
			public override bool CanEvaluateAtCompileTime { get; }

			public override object Invoke(CSharpResolver resolver, object lhs, object rhs)
			{
				return string.Concat(lhs, rhs);
			}
		}
		
		// C# 4.0 spec: §7.8.4 Subtraction operator
		OperatorMethod[] subtractionOperators;
		
		public OperatorMethod[] SubtractionOperators {
			get {
				var ops = LazyInit.VolatileRead(ref subtractionOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref subtractionOperators, Lift(
						new LambdaBinaryOperatorMethod<int,     int>    (this, (a, b) => checked(a - b), (a, b) => unchecked(a - b)),
						new LambdaBinaryOperatorMethod<uint,    uint>   (this, (a, b) => checked(a - b), (a, b) => unchecked(a - b)),
						new LambdaBinaryOperatorMethod<long,    long>   (this, (a, b) => checked(a - b), (a, b) => unchecked(a - b)),
						new LambdaBinaryOperatorMethod<ulong,   ulong>  (this, (a, b) => checked(a - b), (a, b) => unchecked(a - b)),
						new LambdaBinaryOperatorMethod<float,   float>  (this, (a, b) => checked(a - b), (a, b) => unchecked(a - b)),
						new LambdaBinaryOperatorMethod<double,  double> (this, (a, b) => checked(a - b), (a, b) => unchecked(a - b)),
						new LambdaBinaryOperatorMethod<decimal, decimal>(this, (a, b) => checked(a - b), (a, b) => unchecked(a - b))
					));
				}
			}
		}
		
		// C# 4.0 spec: §7.8.5 Shift operators
		OperatorMethod[] shiftLeftOperators;
		
		public OperatorMethod[] ShiftLeftOperators {
			get {
				var ops = LazyInit.VolatileRead(ref shiftLeftOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref shiftLeftOperators, Lift(
						new LambdaBinaryOperatorMethod<int,   int>(this, (a, b) => a << b),
						new LambdaBinaryOperatorMethod<uint,  int>(this, (a, b) => a << b),
						new LambdaBinaryOperatorMethod<long,  int>(this, (a, b) => a << b),
						new LambdaBinaryOperatorMethod<ulong, int>(this, (a, b) => a << b)
					));
				}
			}
		}
		
		OperatorMethod[] shiftRightOperators;
		
		public OperatorMethod[] ShiftRightOperators {
			get {
				var ops = LazyInit.VolatileRead(ref shiftRightOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref shiftRightOperators, Lift(
						new LambdaBinaryOperatorMethod<int,   int>(this, (a, b) => a >> b),
						new LambdaBinaryOperatorMethod<uint,  int>(this, (a, b) => a >> b),
						new LambdaBinaryOperatorMethod<long,  int>(this, (a, b) => a >> b),
						new LambdaBinaryOperatorMethod<ulong, int>(this, (a, b) => a >> b)
					));
				}
			}
		}
		#endregion
		
		#region Equality operators
		sealed class EqualityOperatorMethod : BinaryOperatorMethod
		{
			public readonly TypeCode Type;
			public readonly bool Negate;
			
			public EqualityOperatorMethod(CSharpOperators operators, TypeCode type, bool negate)
				: base(operators.compilation)
			{
				this.Negate = negate;
				this.Type = type;
				this.ReturnType = operators.compilation.FindType(KnownTypeCode.Boolean);
				parameters.Add(operators.MakeParameter(type));
				parameters.Add(operators.MakeParameter(type));
			}
			
			public override bool CanEvaluateAtCompileTime => Type != TypeCode.Object;

			public override object Invoke(CSharpResolver resolver, object lhs, object rhs)
			{
				if (lhs == null && rhs == null)
					return !Negate; // ==: true; !=: false
				if (lhs == null || rhs == null)
					return Negate; // ==: false; !=: true
				lhs = resolver.CSharpPrimitiveCast(Type, lhs);
				rhs = resolver.CSharpPrimitiveCast(Type, rhs);
				bool equal;
				if (Type == TypeCode.Single) {
					equal = (float)lhs == (float)rhs;
				} else if (Type == TypeCode.Double) {
					equal = (double)lhs == (double)rhs;
				} else {
					equal = object.Equals(lhs, rhs);
				}
				return equal ^ Negate;
			}
			
			public override OperatorMethod Lift(CSharpOperators operators)
			{
				if (Type == TypeCode.Object || Type == TypeCode.String)
					return null;
				else
					return new LiftedEqualityOperatorMethod(operators, this);
			}
		}
		
		sealed class LiftedEqualityOperatorMethod : BinaryOperatorMethod, ILiftedOperator
		{
			readonly EqualityOperatorMethod baseMethod;
			
			public LiftedEqualityOperatorMethod(CSharpOperators operators, EqualityOperatorMethod baseMethod)
				: base(operators.compilation)
			{
				this.baseMethod = baseMethod;
				this.ReturnType = baseMethod.ReturnType;
				var p = operators.MakeNullableParameter(baseMethod.Parameters[0]);
				parameters.Add(p);
				parameters.Add(p);
			}
			
			public override bool CanEvaluateAtCompileTime => baseMethod.CanEvaluateAtCompileTime;

			public override object Invoke(CSharpResolver resolver, object lhs, object rhs)
			{
				return baseMethod.Invoke(resolver, lhs, rhs);
			}

			public IReadOnlyList<IParameter> NonLiftedParameters => baseMethod.Parameters;
			public IType NonLiftedReturnType => baseMethod.ReturnType;
		}
		
		// C# 4.0 spec: §7.10 Relational and type-testing operators
		static readonly TypeCode[] valueEqualityOperatorsFor = {
			TypeCode.Int32, TypeCode.UInt32,
			TypeCode.Int64, TypeCode.UInt64,
			TypeCode.Single, TypeCode.Double,
			TypeCode.Decimal,
			TypeCode.Boolean
		};
		
		OperatorMethod[] valueEqualityOperators;
		
		public OperatorMethod[] ValueEqualityOperators {
			get {
				var ops = LazyInit.VolatileRead(ref valueEqualityOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref valueEqualityOperators, Lift(
						valueEqualityOperatorsFor.Select(c => new EqualityOperatorMethod(this, c, false)).ToArray()
					));
				}
			}
		}
		
		OperatorMethod[] valueInequalityOperators;
		
		public OperatorMethod[] ValueInequalityOperators {
			get {
				var ops = LazyInit.VolatileRead(ref valueInequalityOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref valueInequalityOperators, Lift(
						valueEqualityOperatorsFor.Select(c => new EqualityOperatorMethod(this, c, true)).ToArray()
					));
				}
			}
		}
		
		OperatorMethod[] referenceEqualityOperators;
		
		public OperatorMethod[] ReferenceEqualityOperators {
			get {
				var ops = LazyInit.VolatileRead(ref referenceEqualityOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref referenceEqualityOperators, Lift(
						new EqualityOperatorMethod(this, TypeCode.Object, false),
						new EqualityOperatorMethod(this, TypeCode.String, false)
					));
				}
			}
		}
		
		OperatorMethod[] referenceInequalityOperators;
		
		public OperatorMethod[] ReferenceInequalityOperators {
			get {
				var ops = LazyInit.VolatileRead(ref referenceInequalityOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref referenceInequalityOperators, Lift(
						new EqualityOperatorMethod(this, TypeCode.Object, true),
						new EqualityOperatorMethod(this, TypeCode.String, true)
					));
				}
			}
		}
		#endregion
		
		#region Relational Operators
		sealed class RelationalOperatorMethod<T1, T2> : BinaryOperatorMethod
		{
			readonly Func<T1, T2, bool> func;
			
			public RelationalOperatorMethod(CSharpOperators operators, Func<T1, T2, bool> func)
				: base(operators.compilation)
			{
				this.ReturnType = operators.compilation.FindType(KnownTypeCode.Boolean);
				parameters.Add(operators.MakeParameter(Type.GetTypeCode(typeof(T1))));
				parameters.Add(operators.MakeParameter(Type.GetTypeCode(typeof(T2))));
				this.func = func;
			}
			
			public override bool CanEvaluateAtCompileTime => true;

			public override object Invoke(CSharpResolver resolver, object lhs, object rhs)
			{
				if (lhs == null || rhs == null)
					return null;
				return func((T1)resolver.CSharpPrimitiveCast(Type.GetTypeCode(typeof(T1)), lhs),
				            (T2)resolver.CSharpPrimitiveCast(Type.GetTypeCode(typeof(T2)), rhs));
			}
			
			public override OperatorMethod Lift(CSharpOperators operators)
			{
				var lifted = new LiftedBinaryOperatorMethod(operators, this);
				lifted.ReturnType = this.ReturnType; // don't lift the return type for relational operators
				return lifted;
			}
		}
		
		OperatorMethod[] lessThanOperators;
		
		public OperatorMethod[] LessThanOperators {
			get {
				var ops = LazyInit.VolatileRead(ref lessThanOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref lessThanOperators, Lift(
						new RelationalOperatorMethod<int, int>        (this, (a, b) => a < b),
						new RelationalOperatorMethod<uint, uint>      (this, (a, b) => a < b),
						new RelationalOperatorMethod<long, long>      (this, (a, b) => a < b),
						new RelationalOperatorMethod<ulong, ulong>    (this, (a, b) => a < b),
						new RelationalOperatorMethod<float, float>    (this, (a, b) => a < b),
						new RelationalOperatorMethod<double, double>  (this, (a, b) => a < b),
						new RelationalOperatorMethod<decimal, decimal>(this, (a, b) => a < b)
					));
				}
			}
		}
		
		OperatorMethod[] lessThanOrEqualOperators;
		
		public OperatorMethod[] LessThanOrEqualOperators {
			get {
				var ops = LazyInit.VolatileRead(ref lessThanOrEqualOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref lessThanOrEqualOperators, Lift(
						new RelationalOperatorMethod<int, int>        (this, (a, b) => a <= b),
						new RelationalOperatorMethod<uint, uint>      (this, (a, b) => a <= b),
						new RelationalOperatorMethod<long, long>      (this, (a, b) => a <= b),
						new RelationalOperatorMethod<ulong, ulong>    (this, (a, b) => a <= b),
						new RelationalOperatorMethod<float, float>    (this, (a, b) => a <= b),
						new RelationalOperatorMethod<double, double>  (this, (a, b) => a <= b),
						new RelationalOperatorMethod<decimal, decimal>(this, (a, b) => a <= b)
					));
				}
			}
		}
		
		OperatorMethod[] greaterThanOperators;
		
		public OperatorMethod[] GreaterThanOperators {
			get {
				var ops = LazyInit.VolatileRead(ref greaterThanOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref greaterThanOperators, Lift(
						new RelationalOperatorMethod<int, int>        (this, (a, b) => a > b),
						new RelationalOperatorMethod<uint, uint>      (this, (a, b) => a > b),
						new RelationalOperatorMethod<long, long>      (this, (a, b) => a > b),
						new RelationalOperatorMethod<ulong, ulong>    (this, (a, b) => a > b),
						new RelationalOperatorMethod<float, float>    (this, (a, b) => a > b),
						new RelationalOperatorMethod<double, double>  (this, (a, b) => a > b),
						new RelationalOperatorMethod<decimal, decimal>(this, (a, b) => a > b)
					));
				}
			}
		}
		
		OperatorMethod[] greaterThanOrEqualOperators;
		
		public OperatorMethod[] GreaterThanOrEqualOperators {
			get {
				var ops = LazyInit.VolatileRead(ref greaterThanOrEqualOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref greaterThanOrEqualOperators, Lift(
						new RelationalOperatorMethod<int, int>        (this, (a, b) => a >= b),
						new RelationalOperatorMethod<uint, uint>      (this, (a, b) => a >= b),
						new RelationalOperatorMethod<long, long>      (this, (a, b) => a >= b),
						new RelationalOperatorMethod<ulong, ulong>    (this, (a, b) => a >= b),
						new RelationalOperatorMethod<float, float>    (this, (a, b) => a >= b),
						new RelationalOperatorMethod<double, double>  (this, (a, b) => a >= b),
						new RelationalOperatorMethod<decimal, decimal>(this, (a, b) => a >= b)
					));
				}
			}
		}
		#endregion
		
		#region Bitwise operators
		OperatorMethod[] logicalAndOperators;
		
		public OperatorMethod[] LogicalAndOperators {
			get {
				var ops = LazyInit.VolatileRead(ref logicalAndOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref logicalAndOperators, new OperatorMethod[] {
					                         	new LambdaBinaryOperatorMethod<bool, bool>(this, (a, b) => a & b)
					                         });
				}
			}
		}
		
		
		OperatorMethod[] bitwiseAndOperators;
		
		public OperatorMethod[] BitwiseAndOperators {
			get {
				var ops = LazyInit.VolatileRead(ref bitwiseAndOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref bitwiseAndOperators, Lift(
						new LambdaBinaryOperatorMethod<int, int>    (this, (a, b) => a & b),
						new LambdaBinaryOperatorMethod<uint, uint>  (this, (a, b) => a & b),
						new LambdaBinaryOperatorMethod<long, long>  (this, (a, b) => a & b),
						new LambdaBinaryOperatorMethod<ulong, ulong>(this, (a, b) => a & b),
						this.LogicalAndOperators[0]
					));
				}
			}
		}
		
		
		OperatorMethod[] logicalOrOperators;
		
		public OperatorMethod[] LogicalOrOperators {
			get {
				var ops = LazyInit.VolatileRead(ref logicalOrOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref logicalOrOperators, new OperatorMethod[] {
					                         	new LambdaBinaryOperatorMethod<bool, bool>(this, (a, b) => a | b)
					                         });
				}
			}
		}
		
		OperatorMethod[] bitwiseOrOperators;
		
		public OperatorMethod[] BitwiseOrOperators {
			get {
				var ops = LazyInit.VolatileRead(ref bitwiseOrOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref bitwiseOrOperators, Lift(
						new LambdaBinaryOperatorMethod<int, int>    (this, (a, b) => a | b),
						new LambdaBinaryOperatorMethod<uint, uint>  (this, (a, b) => a | b),
						new LambdaBinaryOperatorMethod<long, long>  (this, (a, b) => a | b),
						new LambdaBinaryOperatorMethod<ulong, ulong>(this, (a, b) => a | b),
						this.LogicalOrOperators[0]
					));
				}
			}
		}
		
		// Note: the logic for the lifted bool? bitwise operators is wrong;
		// we produce "true | null" = "null" when it should be true. However, this is irrelevant
		// because bool? cannot be a compile-time type.
		
		OperatorMethod[] bitwiseXorOperators;

		public OperatorMethod[] BitwiseXorOperators {
			get {
				var ops = LazyInit.VolatileRead(ref bitwiseXorOperators);
				if (ops != null) {
					return ops;
				} else {
					return LazyInit.GetOrSet(ref bitwiseXorOperators, Lift(
						new LambdaBinaryOperatorMethod<int, int>    (this, (a, b) => a ^ b),
						new LambdaBinaryOperatorMethod<uint, uint>  (this, (a, b) => a ^ b),
						new LambdaBinaryOperatorMethod<long, long>  (this, (a, b) => a ^ b),
						new LambdaBinaryOperatorMethod<ulong, ulong>(this, (a, b) => a ^ b),
						new LambdaBinaryOperatorMethod<bool, bool>  (this, (a, b) => a ^ b)
					));
				}
			}
		}
		#endregion

		#region User-defined operators
		public static IMethod LiftUserDefinedOperator(IMethod m)
		{
			if (IsComparisonOperator(m)) {
				if (!m.ReturnType.IsKnownType(KnownTypeCode.Boolean))
					return null; // cannot lift this operator
			} else {
				if (!NullableType.IsNonNullableValueType(m.ReturnType))
					return null; // cannot lift this operator
			}
			for (var i = 0; i < m.Parameters.Count; i++) {
				if (!NullableType.IsNonNullableValueType(m.Parameters[i].Type))
					return null; // cannot lift this operator
			}
			return new LiftedUserDefinedOperator(m);
		}

		internal static bool IsComparisonOperator(IMethod m)
		{
			return m.IsOperator && m.Parameters.Count == 2
				&& (OperatorDeclaration.GetOperatorType(m.Name)?.IsComparisonOperator() ?? false);
		}

		sealed class LiftedUserDefinedOperator : SpecializedMethod, ILiftedOperator
		{
			internal readonly IParameterizedMember nonLiftedOperator;

			public LiftedUserDefinedOperator(IMethod nonLiftedMethod)
				: base((IMethod)nonLiftedMethod.MemberDefinition, nonLiftedMethod.Substitution)
			{
				this.nonLiftedOperator = nonLiftedMethod;
				var substitution = new MakeNullableVisitor(nonLiftedMethod.Compilation, nonLiftedMethod.Substitution);
				this.Parameters = base.CreateParameters(substitution);
				// Comparison operators keep the 'bool' return type even when lifted.
				if (IsComparisonOperator(nonLiftedMethod))
					this.ReturnType = nonLiftedMethod.ReturnType;
				else
					this.ReturnType = nonLiftedMethod.ReturnType.AcceptVisitor(substitution);
			}

			public IReadOnlyList<IParameter> NonLiftedParameters => nonLiftedOperator.Parameters;
			public IType NonLiftedReturnType => nonLiftedOperator.ReturnType;

			public override bool Equals(object obj)
			{
				var op = obj as LiftedUserDefinedOperator;
				return op != null && this.nonLiftedOperator.Equals(op.nonLiftedOperator);
			}

			public override int GetHashCode()
			{
				return nonLiftedOperator.GetHashCode() ^ 0x7191254;
			}
		}

		sealed class MakeNullableVisitor : TypeVisitor
		{
			readonly ICompilation compilation;
			readonly TypeParameterSubstitution typeParameterSubstitution;

			public MakeNullableVisitor(ICompilation compilation, TypeParameterSubstitution typeParameterSubstitution)
			{
				this.compilation = compilation;
				this.typeParameterSubstitution = typeParameterSubstitution;
			}

			public override IType VisitTypeDefinition(ITypeDefinition type)
			{
				return NullableType.Create(compilation, type.AcceptVisitor(typeParameterSubstitution));
			}

			public override IType VisitTypeParameter(ITypeParameter type)
			{
				return NullableType.Create(compilation, type.AcceptVisitor(typeParameterSubstitution));
			}

			public override IType VisitParameterizedType(ParameterizedType type)
			{
				return NullableType.Create(compilation, type.AcceptVisitor(typeParameterSubstitution));
			}

			public override IType VisitOtherType(IType type)
			{
				return NullableType.Create(compilation, type.AcceptVisitor(typeParameterSubstitution));
			}
		}
		#endregion
	}

	/// <summary>
	/// Implement this interface to give overload resolution a hint that the member represents a lifted operator,
	/// which is used in the tie-breaking rules.
	/// </summary>
	public interface ILiftedOperator : IParameterizedMember
	{
		IType NonLiftedReturnType { get; }
		IReadOnlyList<IParameter> NonLiftedParameters { get; }
	}
}
