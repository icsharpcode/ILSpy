// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;

namespace ICSharpCode.Decompiler
{
	/// <summary>
	/// Settings for the decompiler.
	/// </summary>
	public partial class DecompilerSettings : INotifyPropertyChanged
	{
		/// <summary>
		/// Equivalent to <c>new DecompilerSettings(LanguageVersion.Latest)</c>
		/// </summary>
		public DecompilerSettings()
		{
		}

		/// <summary>
		/// Creates a new DecompilerSettings instance with initial settings
		/// appropriate for the specified language version.
		/// </summary>
		/// <remarks>
		/// This does not imply that the resulting code strictly uses only language features from
		/// that version. Language constructs like generics or ref locals cannot be removed from
		/// the compiled code.
		/// </remarks>
		public DecompilerSettings(CSharp.LanguageVersion languageVersion)
		{
			SetLanguageVersion(languageVersion);
		}

		/// <summary>
		/// Use C# 9 <c>nint</c>/<c>nuint</c> types.
		/// </summary>
		[Description("DecompilerSettings.NativeIntegers")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp9_0)]
		public partial bool NativeIntegers { get; set; }

		/// <summary>
		/// Treat <c>IntPtr</c>/<c>UIntPtr</c> as <c>nint</c>/<c>nuint</c>.
		/// </summary>
		[Description("DecompilerSettings.NumericIntPtr")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp11_0)]
		public partial bool NumericIntPtr { get; set; }

		/// <summary>
		/// Decompile C# 9 covariant return types.
		/// </summary>
		[Description("DecompilerSettings.CovariantReturns")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp9_0)]
		public partial bool CovariantReturns { get; set; }

		/// <summary>
		/// Use C# 9 <c>init;</c> property accessors.
		/// </summary>
		[Description("DecompilerSettings.InitAccessors")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp9_0)]
		public partial bool InitAccessors { get; set; }

		/// <summary>
		/// Use C# 9 <c>record</c> classes.
		/// </summary>
		[Description("DecompilerSettings.RecordClasses")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp9_0)]
		public partial bool RecordClasses { get; set; }

		/// <summary>
		/// Use C# 10 <c>record</c> structs.
		/// </summary>
		[Description("DecompilerSettings.RecordStructs")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp10_0)]
		public partial bool RecordStructs { get; set; }

		/// <summary>
		/// Use field initializers in structs.
		/// </summary>
		[Description("DecompilerSettings.StructDefaultConstructorsAndFieldInitializers")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp10_0)]
		public partial bool StructDefaultConstructorsAndFieldInitializers { get; set; }

		/// <summary>
		/// Use C# 9 <c>with</c> initializer expressions.
		/// </summary>
		[Description("DecompilerSettings.WithExpressions")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp9_0)]
		public partial bool WithExpressions { get; set; }

		/// <summary>
		/// Use primary constructor syntax with records.
		/// </summary>
		[Description("DecompilerSettings.UsePrimaryConstructorSyntax")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp9_0)]
		public partial bool UsePrimaryConstructorSyntax { get; set; }

		/// <summary>
		/// Use C# 9 <c>delegate* unmanaged</c> types.
		/// If this option is disabled, function pointers will instead be decompiled with type `IntPtr`.
		/// </summary>
		[Description("DecompilerSettings.FunctionPointers")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp9_0)]
		public partial bool FunctionPointers { get; set; }

		/// <summary>
		/// Use C# 11 <c>scoped</c> modifier.
		/// </summary>
		[Description("DecompilerSettings.ScopedRef")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp11_0)]
		public partial bool ScopedRef { get; set; }

		[Obsolete("Renamed to ScopedRef. This property will be removed in a future version of the decompiler.")]
		[Browsable(false)]
		public bool LifetimeAnnotations {
			get { return ScopedRef; }
			set { ScopedRef = value; }
		}

		/// <summary>
		/// Use C# 11 <c>required</c> modifier.
		/// </summary>
		[Description("DecompilerSettings.RequiredMembers")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp11_0)]
		public partial bool RequiredMembers { get; set; }

		/// <summary>
		/// Use C# 8 switch expressions.
		/// </summary>
		[Description("DecompilerSettings.SwitchExpressions")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp8_0)]
		public partial bool SwitchExpressions { get; set; }

		/// <summary>
		/// Use C# 10 file-scoped namespaces.
		/// </summary>
		[Description("DecompilerSettings.FileScopedNamespaces")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp10_0)]
		public partial bool FileScopedNamespaces { get; set; }

		/// <summary>
		/// Decompile anonymous methods/lambdas.
		/// </summary>
		[Description("DecompilerSettings.DecompileAnonymousMethodsLambdas")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp2)]
		public partial bool AnonymousMethods { get; set; }

		/// <summary>
		/// Decompile anonymous types.
		/// </summary>
		[Description("DecompilerSettings.DecompileAnonymousTypes")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp3)]
		public partial bool AnonymousTypes { get; set; }

		/// <summary>
		/// Use C# 3 lambda syntax if possible.
		/// </summary>
		[Description("DecompilerSettings.UseLambdaSyntaxIfPossible")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp3)]
		public partial bool UseLambdaSyntax { get; set; }

		/// <summary>
		/// Decompile expression trees.
		/// </summary>
		[Description("DecompilerSettings.DecompileExpressionTrees")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp3)]
		public partial bool ExpressionTrees { get; set; }

		/// <summary>
		/// Decompile enumerators.
		/// </summary>
		[Description("DecompilerSettings.DecompileEnumeratorsYieldReturn")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp2)]
		public partial bool YieldReturn { get; set; }

		/// <summary>
		/// Decompile use of the 'dynamic' type.
		/// </summary>
		[Description("DecompilerSettings.DecompileUseOfTheDynamicType")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp4)]
		public partial bool Dynamic { get; set; }

		/// <summary>
		/// Decompile async methods.
		/// </summary>
		[Description("DecompilerSettings.DecompileAsyncMethods")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp5)]
		public partial bool AsyncAwait { get; set; }

		/// <summary>
		/// Decompile await in catch/finally blocks.
		/// Only has an effect if <see cref="AsyncAwait"/> is enabled.
		/// </summary>
		[Description("DecompilerSettings.DecompileAwaitInCatchFinallyBlocks")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp6)]
		public partial bool AwaitInCatchFinally { get; set; }

		/// <summary>
		/// Decompile IAsyncEnumerator/IAsyncEnumerable.
		/// Only has an effect if <see cref="AsyncAwait"/> is enabled.
		/// </summary>
		[Description("DecompilerSettings.AsyncEnumerator")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp8_0)]
		public partial bool AsyncEnumerator { get; set; }

		/// <summary>
		/// Decompile [DecimalConstant(...)] as simple literal values.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DecompileDecimalConstantAsSimpleLiteralValues")]
		[DecompilerSetting]
		public partial bool DecimalConstants { get; set; }

		/// <summary>
		/// Decompile C# 1.0 'public unsafe fixed int arr[10];' members.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DecompileC10PublicUnsafeFixedIntArr10Members")]
		[DecompilerSetting]
		public partial bool FixedBuffers { get; set; }

		/// <summary>
		/// Decompile 'string.Concat(a, b)' calls into 'a + b'.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.StringConcat")]
		[DecompilerSetting]
		public partial bool StringConcat { get; set; }

		/// <summary>
		/// Use lifted operators for nullables.
		/// </summary>
		[Description("DecompilerSettings.UseLiftedOperatorsForNullables")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp2)]
		public partial bool LiftNullables { get; set; }

		/// <summary>
		/// Decompile C# 6 ?. and ?[] operators.
		/// </summary>
		[Description("DecompilerSettings.NullPropagation")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp6)]
		public partial bool NullPropagation { get; set; }

		/// <summary>
		/// Decompile automatic properties
		/// </summary>
		[Description("DecompilerSettings.DecompileAutomaticProperties")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp3)]
		public partial bool AutomaticProperties { get; set; }

		/// <summary>
		/// Decompile getter-only automatic properties
		/// </summary>
		[Description("DecompilerSettings.GetterOnlyAutomaticProperties")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp6)]
		public partial bool GetterOnlyAutomaticProperties { get; set; }

		/// <summary>
		/// Decompile automatic events
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DecompileAutomaticEvents")]
		[DecompilerSetting]
		public partial bool AutomaticEvents { get; set; }

		/// <summary>
		/// Decompile using statements.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DetectUsingStatements")]
		[DecompilerSetting]
		public partial bool UsingStatement { get; set; }

		/// <summary>
		/// Use enhanced using statements.
		/// </summary>
		[Description("DecompilerSettings.UseEnhancedUsing")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp8_0)]
		public partial bool UseEnhancedUsing { get; set; }

		/// <summary>
		/// Gets/Sets whether to use braces for single-statement-blocks. 
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.AlwaysUseBraces")]
		[DecompilerSetting]
		public partial bool AlwaysUseBraces { get; set; }

		/// <summary>
		/// Decompile foreach statements.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DetectForeachStatements")]
		[DecompilerSetting]
		public partial bool ForEachStatement { get; set; }

		/// <summary>
		/// Support GetEnumerator extension methods in foreach.
		/// </summary>
		[Description("DecompilerSettings.DecompileForEachWithGetEnumeratorExtension")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp9_0)]
		public partial bool ForEachWithGetEnumeratorExtension { get; set; }

		/// <summary>
		/// Support params collections.
		/// </summary>
		[Description("DecompilerSettings.DecompileParamsCollections")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp13_0)]
		public partial bool ParamsCollections { get; set; }

		/// <summary>
		/// Decompile lock statements.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DetectLockStatements")]
		[DecompilerSetting]
		public partial bool LockStatement { get; set; }

		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DetectSwitchOnString")]
		[DecompilerSetting]
		public partial bool SwitchStatementOnString { get; set; }

		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.SparseIntegerSwitch")]
		[DecompilerSetting]
		public partial bool SparseIntegerSwitch { get; set; }

		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.InsertUsingDeclarations")]
		[DecompilerSetting]
		public partial bool UsingDeclarations { get; set; }

		[Description("DecompilerSettings.UseExtensionMethodSyntax")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp3)]
		public partial bool ExtensionMethods { get; set; }

		[Description("DecompilerSettings.UseLINQExpressionSyntax")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp3)]
		public partial bool QueryExpressions { get; set; }

		/// <summary>
		/// Gets/Sets whether to use C# 2.0 method group conversions.
		/// true: <c>EventHandler h = this.OnClick;</c>
		/// false: <c>EventHandler h = new EventHandler(this.OnClick);</c>
		/// </summary>
		[Description("DecompilerSettings.UseImplicitMethodGroupConversions")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp2)]
		public partial bool UseImplicitMethodGroupConversion { get; set; }

		/// <summary>
		/// Gets/Sets whether to use object creation expressions for generic types with <c>new()</c> constraint.
		/// true: <c>T t = new T();</c>
		/// false: <c>T t = Activator.CreateInstance&lt;T&gt;()</c>
		/// </summary>
		[Description("DecompilerSettings.UseObjectCreationOfGenericTypeParameter")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp2)]
		public partial bool UseObjectCreationOfGenericTypeParameter { get; set; }

		/// <summary>
		/// Gets/Sets whether to always cast targets to explicitly implemented methods.
		/// true: <c>((ISupportInitialize)pictureBox1).BeginInit();</c>
		/// false: <c>pictureBox1.BeginInit();</c>
		/// default: false
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.AlwaysCastTargetsOfExplicitInterfaceImplementationCalls")]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool AlwaysCastTargetsOfExplicitInterfaceImplementationCalls { get; set; }

		/// <summary>
		/// Gets/Sets whether to always qualify member references.
		/// true: <c>this.DoSomething();</c>
		/// false: <c>DoSomething();</c>
		/// default: false
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.AlwaysQualifyMemberReferences")]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool AlwaysQualifyMemberReferences { get; set; }

		/// <summary>
		/// Gets/Sets whether to always show enum member values.
		/// true: <c>enum Kind { A = 0, B = 1, C = 5 }</c>
		/// false: <c>enum Kind { A, B, C = 5 }</c>
		/// default: false
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.AlwaysShowEnumMemberValues")]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool AlwaysShowEnumMemberValues { get; set; }

		/// <summary>
		/// Gets/Sets whether to use variable names from debug symbols, if available.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.UseVariableNamesFromDebugSymbolsIfAvailable")]
		[DecompilerSetting]
		public partial bool UseDebugSymbols { get; set; }

		/// <summary>
		/// Gets/Sets whether to use array initializers.
		/// If set to false, might produce non-compilable code.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.ArrayInitializerExpressions")]
		[DecompilerSetting]
		public partial bool ArrayInitializers { get; set; }

		/// <summary>
		/// Gets/Sets whether to use C# 3.0 object/collection initializers.
		/// </summary>
		[Description("DecompilerSettings.ObjectCollectionInitializerExpressions")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp3)]
		public partial bool ObjectOrCollectionInitializers { get; set; }

		/// <summary>
		/// Gets/Sets whether to use C# 6.0 dictionary initializers.
		/// Only has an effect if ObjectOrCollectionInitializers is enabled.
		/// </summary>
		[Description("DecompilerSettings.DictionaryInitializerExpressions")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp6)]
		public partial bool DictionaryInitializers { get; set; }

		/// <summary>
		/// Gets/Sets whether to use C# 6.0 Extension Add methods in collection initializers.
		/// Only has an effect if ObjectOrCollectionInitializers is enabled.
		/// </summary>
		[Description("DecompilerSettings.AllowExtensionAddMethodsInCollectionInitializerExpressions")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp6)]
		public partial bool ExtensionMethodsInCollectionInitializers { get; set; }

		/// <summary>
		/// Gets/Sets whether to use local ref variables in cases where this is necessary
		/// for re-compilation with a modern C# compiler to reproduce the same behavior
		/// as the original assembly produced with an old C# compiler that used an incorrect
		/// order of evaluation.
		/// See https://github.com/icsharpcode/ILSpy/issues/2050
		/// </summary>
		[Description("DecompilerSettings.UseRefLocalsForAccurateOrderOfEvaluation")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7)]
		public partial bool UseRefLocalsForAccurateOrderOfEvaluation { get; set; }

		/// <summary>
		/// Gets/Sets whether to use C# 7.2 'ref' extension methods.
		/// </summary>
		[Description("DecompilerSettings.AllowExtensionMethodSyntaxOnRef")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7_2)]
		public partial bool RefExtensionMethods { get; set; }

		/// <summary>
		/// Gets/Sets whether to use C# 6.0 string interpolation
		/// </summary>
		[Description("DecompilerSettings.UseStringInterpolation")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp6)]
		public partial bool StringInterpolation { get; set; }

		/// <summary>
		/// Gets/Sets whether to use C# 11.0 UTF-8 string literals
		/// </summary>
		[Description("DecompilerSettings.Utf8StringLiterals")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp11_0)]
		public partial bool Utf8StringLiterals { get; set; }

		/// <summary>
		/// Gets/Sets whether to use C# 11.0 switch on (ReadOnly)Span&lt;char&gt;
		/// </summary>
		[Description("DecompilerSettings.SwitchOnReadOnlySpanChar")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp11_0)]
		public partial bool SwitchOnReadOnlySpanChar { get; set; }

		/// <summary>
		/// Gets/Sets whether to use C# 11.0 unsigned right shift operator.
		/// </summary>
		[Description("DecompilerSettings.UnsignedRightShift")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp11_0)]
		public partial bool UnsignedRightShift { get; set; }

		/// <summary>
		/// Gets/Sets whether to use C# 11.0 user-defined checked operators.
		/// </summary>
		[Description("DecompilerSettings.CheckedOperators")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp11_0)]
		public partial bool CheckedOperators { get; set; }

		/// <summary>
		/// Gets/Sets whether to include XML documentation comments in the decompiled code.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.IncludeXMLDocumentationCommentsInTheDecompiledCode")]
		[DecompilerSetting]
		public partial bool ShowXmlDocumentation { get; set; }

		[Browsable(false)]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool FoldBraces { get; set; }

		[Browsable(false)]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool ExpandXmlDocumentationComments { get; set; }

		[Browsable(false)]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool ExpandMemberDefinitions { get; set; }

		[Browsable(false)]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool ExpandUsingDeclarations { get; set; }

		/// <summary>
		/// Gets/Sets whether member bodies should be decompiled.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Browsable(false)]
		[DecompilerSetting]
		public partial bool DecompileMemberBodies { get; set; }

		/// <summary>
		/// Gets/Sets whether simple calculated getter-only property declarations
		/// should use expression body syntax.
		/// </summary>
		[Description("DecompilerSettings.UseExpressionBodiedMemberSyntaxForGetOnlyProperties")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp6)]
		public partial bool UseExpressionBodyForCalculatedGetterOnlyProperties { get; set; }

		/// <summary>
		/// Gets/Sets whether out variable declarations should be used when possible.
		/// </summary>
		[Description("DecompilerSettings.UseOutVariableDeclarations")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7)]
		public partial bool OutVariables { get; set; }

		/// <summary>
		/// Gets/Sets whether discards should be used when possible.
		/// Only has an effect if <see cref="OutVariables"/> is enabled.
		/// </summary>
		[Description("DecompilerSettings.UseDiscards")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7)]
		public partial bool Discards { get; set; }

		/// <summary>
		/// Gets/Sets whether IsByRefLikeAttribute should be replaced with 'ref' modifiers on structs.
		/// </summary>
		[Description("DecompilerSettings.IsByRefLikeAttributeShouldBeReplacedWithRefModifiersOnStructs")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7_2)]
		public partial bool IntroduceRefModifiersOnStructs { get; set; }

		/// <summary>
		/// Gets/Sets whether IsReadOnlyAttribute should be replaced with 'readonly' modifiers on structs
		/// and with the 'in' modifier on parameters.
		/// </summary>
		[Description("DecompilerSettings." +
			"IsReadOnlyAttributeShouldBeReplacedWithReadonlyInModifiersOnStructsParameters")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7_2)]
		public partial bool IntroduceReadonlyAndInModifiers { get; set; }

		/// <summary>
		/// Gets/Sets whether "private protected" should be used.
		/// </summary>
		[Description("DecompilerSettings.IntroducePrivateProtectedAccessibility")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7_2)]
		public partial bool IntroducePrivateProtectedAccessibility { get; set; }

		[Description("DecompilerSettings.ReadOnlyMethods")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp8_0)]
		public partial bool ReadOnlyMethods { get; set; }

		[Description("DecompilerSettings.DetectAsyncUsingAndForeachStatements")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp8_0)]
		public partial bool AsyncUsingAndForEachStatement { get; set; }

		/// <summary>
		/// If this option is active, [IsUnmanagedAttribute] on type parameters
		/// is replaced with "T : unmanaged" constraints.
		/// </summary>
		[Description("DecompilerSettings." +
			"IsUnmanagedAttributeOnTypeParametersShouldBeReplacedWithUnmanagedConstraints")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7_3)]
		public partial bool IntroduceUnmanagedConstraint { get; set; }

		/// <summary>
		/// Gets/Sets whether C# 7.3 stackalloc initializers should be used.
		/// </summary>
		[Description("DecompilerSettings.UseStackallocInitializerSyntax")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7_3)]
		public partial bool StackAllocInitializers { get; set; }

		/// <summary>
		/// Gets/Sets whether C# 7.3 pattern based fixed statement should be used.
		/// </summary>
		[Description("DecompilerSettings.UsePatternBasedFixedStatement")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7_3)]
		public partial bool PatternBasedFixedStatement { get; set; }

		/// <summary>
		/// Gets/Sets whether tuple type syntax <c>(int, string)</c>
		/// should be used for <c>System.ValueTuple</c>.
		/// </summary>
		[Description("DecompilerSettings.UseTupleTypeSyntax")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7)]
		public partial bool TupleTypes { get; set; }

		/// <summary>
		/// Gets/Sets whether throw expressions should be used.
		/// </summary>
		[Description("DecompilerSettings.UseThrowExpressions")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7)]
		public partial bool ThrowExpressions { get; set; }

		/// <summary>
		/// Gets/Sets whether implicit conversions between tuples
		/// should be used in the decompiled output.
		/// </summary>
		[Description("DecompilerSettings.UseImplicitConversionsBetweenTupleTypes")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7)]
		public partial bool TupleConversions { get; set; }

		/// <summary>
		/// Gets/Sets whether tuple comparisons should be detected.
		/// </summary>
		[Description("DecompilerSettings.DetectTupleComparisons")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7_3)]
		public partial bool TupleComparisons { get; set; }

		/// <summary>
		/// Gets/Sets whether named arguments should be used.
		/// </summary>
		[Description("DecompilerSettings.UseNamedArguments")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp4)]
		public partial bool NamedArguments { get; set; }

		/// <summary>
		/// Gets/Sets whether C# 7.2 non-trailing named arguments should be used.
		/// </summary>
		[Description("DecompilerSettings.UseNonTrailingNamedArguments")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7_2)]
		public partial bool NonTrailingNamedArguments { get; set; }

		/// <summary>
		/// Gets/Sets whether optional arguments should be removed, if possible.
		/// </summary>
		[Description("DecompilerSettings.RemoveOptionalArgumentsIfPossible")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp4)]
		public partial bool OptionalArguments { get; set; }

		/// <summary>
		/// Gets/Sets whether to expand <c>params</c> arguments by replacing explicit array creation
		/// with individual values in method calls.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.ExpandParamsArguments")]
		[DecompilerSetting]
		public partial bool ExpandParamsArguments { get; set; }

		/// <summary>
		/// Gets/Sets whether C# 7.0 local functions should be transformed.
		/// </summary>
		[Description("DecompilerSettings.IntroduceLocalFunctions")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7)]
		public partial bool LocalFunctions { get; set; }

		/// <summary>
		/// Gets/Sets whether C# 7.0 deconstruction should be detected.
		/// </summary>
		[Description("DecompilerSettings.Deconstruction")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7)]
		public partial bool Deconstruction { get; set; }

		/// <summary>
		/// Gets/Sets whether C# 7.0 pattern matching should be detected.
		/// </summary>
		[Description("DecompilerSettings.PatternMatching")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp7)]
		public partial bool PatternMatching { get; set; }

		/// <summary>
		/// Gets/Sets whether C# 8.0 recursive patterns should be detected.
		/// </summary>
		[Description("DecompilerSettings.RecursivePatternMatching")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp8_0)]
		public partial bool RecursivePatternMatching { get; set; }

		/// <summary>
		/// Gets/Sets whether C# 9.0 and, or, not patterns should be detected.
		/// </summary>
		[Description("DecompilerSettings.PatternCombinators")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp9_0)]
		public partial bool PatternCombinators { get; set; }

		/// <summary>
		/// Gets/Sets whether C# 9.0 relational patterns should be detected.
		/// </summary>
		[Description("DecompilerSettings.RelationalPatterns")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp9_0)]
		public partial bool RelationalPatterns { get; set; }

		/// <summary>
		/// Gets/Sets whether C# 8.0 static local functions should be transformed.
		/// </summary>
		[Description("DecompilerSettings.IntroduceStaticLocalFunctions")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp8_0)]
		public partial bool StaticLocalFunctions { get; set; }

		/// <summary>
		/// Gets/Sets whether C# 8.0 index and range syntax should be used.
		/// </summary>
		[Description("DecompilerSettings.Ranges")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp8_0)]
		public partial bool Ranges { get; set; }

		/// <summary>
		/// Gets/Sets whether C# 8.0 nullable reference types are enabled.
		/// </summary>
		[Description("DecompilerSettings.NullableReferenceTypes")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp8_0)]
		public partial bool NullableReferenceTypes { get; set; }

		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.ShowInfoFromDebugSymbolsIfAvailable")]
		[Browsable(false)]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool ShowDebugInfo { get; set; }
		#region Options to aid VB decompilation

		/// <summary>
		/// Gets/Sets whether the decompiler can assume that 'ldlen; conv.i4.ovf'
		/// does not throw an overflow exception.
		/// </summary>
		[Category("DecompilerSettings.VBSpecificOptions")]
		[Browsable(false)]
		[DecompilerSetting]
		public partial bool AssumeArrayLengthFitsIntoInt32 { get; set; }

		/// <summary>
		/// Gets/Sets whether to use increment and decrement operators
		/// </summary>
		[Category("DecompilerSettings.VBSpecificOptions")]
		[Browsable(false)]
		[DecompilerSetting]
		public partial bool IntroduceIncrementAndDecrement { get; set; }

		/// <summary>
		/// Gets/Sets whether to use assignment expressions such as in while ((count = Do()) != 0) ;
		/// </summary>
		[Category("DecompilerSettings.VBSpecificOptions")]
		[Browsable(false)]
		[DecompilerSetting]
		public partial bool MakeAssignmentExpressions { get; set; }
		#endregion
		#region Options to aid F# decompilation

		[Category("DecompilerSettings.FSpecificOptions")]
		[Description("DecompilerSettings.RemoveDeadAndSideEffectFreeCodeUseWithCaution")]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool RemoveDeadCode { get; set; }

		[Category("DecompilerSettings.FSpecificOptions")]
		[Description("DecompilerSettings.RemoveDeadStores")]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool RemoveDeadStores { get; set; }
		#endregion
		#region Assembly Load and Resolve options

		[Browsable(false)]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool LoadInMemory { get; set; }

		[Browsable(false)]
		[DecompilerSetting]
		public partial bool ThrowOnAssemblyResolveErrors { get; set; }

		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.ApplyWindowsRuntimeProjectionsOnLoadedAssemblies")]
		[DecompilerSetting]
		public partial bool ApplyWindowsRuntimeProjections { get; set; }

		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.AutoLoadAssemblyReferences")]
		[DecompilerSetting]
		public partial bool AutoLoadAssemblyReferences { get; set; }
		#endregion

		/// <summary>
		/// Gets/sets whether the decompiler should produce for loops.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.ForStatement")]
		[DecompilerSetting]
		public partial bool ForStatement { get; set; }

		/// <summary>
		/// Gets/sets whether the decompiler should produce do-while loops.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DoWhileStatement")]
		[DecompilerSetting]
		public partial bool DoWhileStatement { get; set; }

		/// <summary>
		/// Gets/sets whether RequiresLocationAttribute on parameters should be replaced with 'ref readonly' modifiers.
		/// </summary>
		[Description("DecompilerSettings.RefReadOnlyParameters")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp12_0)]
		public partial bool RefReadOnlyParameters { get; set; }

		/// <summary>
		/// Use primary constructor syntax with classes and structs.
		/// </summary>
		[Description("DecompilerSettings.UsePrimaryConstructorSyntaxForNonRecordTypes")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp12_0)]
		public partial bool UsePrimaryConstructorSyntaxForNonRecordTypes { get; set; }

		/// <summary>
		/// Gets/Sets whether C# 12.0 inline array uses should be transformed.
		/// </summary>
		[Description("DecompilerSettings.InlineArrays")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp12_0)]
		public partial bool InlineArrays { get; set; }

		/// <summary>
		/// Gets/Sets whether C# 14.0 extension members should be transformed.
		/// </summary>
		[Description("DecompilerSettings.ExtensionMembers")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp14_0)]
		public partial bool ExtensionMembers { get; set; }

		/// <summary>
		/// Gets/Sets whether (ReadOnly)Span&lt;T&gt; should be treated like built-in types.
		/// </summary>
		[Description("DecompilerSettings.FirstClassSpanTypes")]
		[DecompilerSetting(CSharp.LanguageVersion.CSharp14_0)]
		public partial bool FirstClassSpanTypes { get; set; }

		/// <summary>
		/// Gets/sets whether the decompiler should separate local variable declarations
		/// from their initialization.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.SeparateLocalVariableDeclarations")]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool SeparateLocalVariableDeclarations { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the new SDK style format
		/// shall be used for the generated project files.
		/// </summary>
		[Category("DecompilerSettings.ProjectExport")]
		[Description("DecompilerSettings.UseSdkStyleProjectFormat")]
		[DecompilerSetting]
		public partial bool UseSdkStyleProjectFormat { get; set; }

		/// <summary>
		/// Gets/sets whether namespaces and namespace-like identifiers should be split at '.'
		/// and each part should produce a new level of nesting in the output directory structure. 
		/// </summary>
		[Category("DecompilerSettings.ProjectExport")]
		[Description("DecompilerSettings.UseNestedDirectoriesForNamespaces")]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool UseNestedDirectoriesForNamespaces { get; set; }

		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.AggressiveScalarReplacementOfAggregates")]
		// TODO : Remove once https://github.com/icsharpcode/ILSpy/issues/2032 is fixed.
#if !DEBUG
		[Browsable(false)]
#endif
		[DecompilerSetting(DefaultValue = false)]
		public partial bool AggressiveScalarReplacementOfAggregates { get; set; }

		/// <summary>
		/// If set to false (the default), the decompiler will inline local variables only when they occur
		/// in a context where the C# compiler is known to emit compiler-generated locals.
		/// If set to true, the decompiler will inline local variables whenever possible.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.AggressiveInlining")]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool AggressiveInlining { get; set; }

		/// <summary>
		/// Always fully qualify namespaces using the "global::" prefix.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.AlwaysUseGlobal")]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool AlwaysUseGlobal { get; set; }

		/// <summary>
		/// If set to false (the default), the decompiler will move field initializers at the start of constructors
		/// to their respective field declarations (TransformFieldAndConstructorInitializers) only when the declaring
		/// type has BeforeFieldInit or the member IsConst.
		/// If set true, the decompiler will always move them regardless of the flags.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.AlwaysMoveInitializer")]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool AlwaysMoveInitializer { get; set; }

		/// <summary>
		/// Sort custom attributes.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.SortCustomAttributes")]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool SortCustomAttributes { get; set; }

		/// <summary>
		/// Sort switch sections by their label value instead of by IL offset.
		/// Useful when diffing decompiler output across rebuilds of obfuscated assemblies,
		/// where IL block layout is unstable but the case-to-value mapping is not.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.SortSwitchSections")]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool SortSwitchSections { get; set; }

		/// <summary>
		/// Check for overflow and underflow in operators.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.CheckForOverflowUnderflow")]
		[DecompilerSetting(DefaultValue = false)]
		public partial bool CheckForOverflowUnderflow { get; set; }

		CSharpFormattingOptions csharpFormattingOptions;

		[Browsable(false)]
		public CSharpFormattingOptions CSharpFormattingOptions {
			get {
				if (csharpFormattingOptions == null)
				{
					csharpFormattingOptions = FormattingOptionsFactory.CreateAllman();
					csharpFormattingOptions.IndentSwitchBody = false;
					csharpFormattingOptions.ArrayInitializerWrapping = Wrapping.WrapIfTooLong;
					csharpFormattingOptions.AutoPropertyFormatting = PropertyFormatting.SingleLine;
				}
				return csharpFormattingOptions;
			}
			set {
				if (value == null)
					throw new ArgumentNullException();
				if (csharpFormattingOptions != value)
				{
					csharpFormattingOptions = value;
					OnPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public virtual DecompilerSettings Clone()
		{
			DecompilerSettings settings = (DecompilerSettings)MemberwiseClone();
			if (csharpFormattingOptions != null)
				settings.csharpFormattingOptions = csharpFormattingOptions.Clone();
			settings.PropertyChanged = null;
			return settings;
		}
	}
}
