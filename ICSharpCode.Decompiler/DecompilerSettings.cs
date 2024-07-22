﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
	public class DecompilerSettings : INotifyPropertyChanged
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
		/// Deactivates all language features from versions newer than <paramref name="languageVersion"/>.
		/// </summary>
		public void SetLanguageVersion(CSharp.LanguageVersion languageVersion)
		{
			// By default, all decompiler features are enabled.
			// Disable some of them based on language version:
			if (languageVersion < CSharp.LanguageVersion.CSharp2)
			{
				anonymousMethods = false;
				liftNullables = false;
				yieldReturn = false;
				useImplicitMethodGroupConversion = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp3)
			{
				anonymousTypes = false;
				useLambdaSyntax = false;
				objectCollectionInitializers = false;
				automaticProperties = false;
				extensionMethods = false;
				queryExpressions = false;
				expressionTrees = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp4)
			{
				dynamic = false;
				namedArguments = false;
				optionalArguments = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp5)
			{
				asyncAwait = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp6)
			{
				awaitInCatchFinally = false;
				useExpressionBodyForCalculatedGetterOnlyProperties = false;
				nullPropagation = false;
				stringInterpolation = false;
				dictionaryInitializers = false;
				extensionMethodsInCollectionInitializers = false;
				useRefLocalsForAccurateOrderOfEvaluation = false;
				getterOnlyAutomaticProperties = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp7)
			{
				outVariables = false;
				throwExpressions = false;
				tupleTypes = false;
				tupleConversions = false;
				discards = false;
				localFunctions = false;
				deconstruction = false;
				patternMatching = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp7_2)
			{
				introduceReadonlyAndInModifiers = false;
				introduceRefModifiersOnStructs = false;
				nonTrailingNamedArguments = false;
				refExtensionMethods = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp7_3)
			{
				introduceUnmanagedConstraint = false;
				stackAllocInitializers = false;
				tupleComparisons = false;
				patternBasedFixedStatement = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp8_0)
			{
				nullableReferenceTypes = false;
				readOnlyMethods = false;
				asyncUsingAndForEachStatement = false;
				asyncEnumerator = false;
				useEnhancedUsing = false;
				staticLocalFunctions = false;
				ranges = false;
				switchExpressions = false;
				recursivePatternMatching = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp9_0)
			{
				nativeIntegers = false;
				initAccessors = false;
				functionPointers = false;
				forEachWithGetEnumeratorExtension = false;
				recordClasses = false;
				withExpressions = false;
				usePrimaryConstructorSyntax = false;
				covariantReturns = false;
				relationalPatterns = false;
				patternCombinators = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp10_0)
			{
				fileScopedNamespaces = false;
				recordStructs = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp11_0)
			{
				scopedRef = false;
				requiredMembers = false;
				numericIntPtr = false;
				utf8StringLiterals = false;
				unsignedRightShift = false;
				checkedOperators = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp12_0)
			{
				refReadOnlyParameters = false;
			}
		}

		public CSharp.LanguageVersion GetMinimumRequiredVersion()
		{
			if (refReadOnlyParameters)
				return CSharp.LanguageVersion.CSharp12_0;
			if (scopedRef || requiredMembers || numericIntPtr || utf8StringLiterals || unsignedRightShift || checkedOperators)
				return CSharp.LanguageVersion.CSharp11_0;
			if (fileScopedNamespaces || recordStructs)
				return CSharp.LanguageVersion.CSharp10_0;
			if (nativeIntegers || initAccessors || functionPointers || forEachWithGetEnumeratorExtension
				|| recordClasses || withExpressions || usePrimaryConstructorSyntax || covariantReturns
				|| relationalPatterns || patternCombinators)
				return CSharp.LanguageVersion.CSharp9_0;
			if (nullableReferenceTypes || readOnlyMethods || asyncEnumerator || asyncUsingAndForEachStatement
				|| staticLocalFunctions || ranges || switchExpressions || recursivePatternMatching)
				return CSharp.LanguageVersion.CSharp8_0;
			if (introduceUnmanagedConstraint || tupleComparisons || stackAllocInitializers
				|| patternBasedFixedStatement)
				return CSharp.LanguageVersion.CSharp7_3;
			if (introduceRefModifiersOnStructs || introduceReadonlyAndInModifiers
				|| nonTrailingNamedArguments || refExtensionMethods)
				return CSharp.LanguageVersion.CSharp7_2;
			// C# 7.1 missing
			if (outVariables || throwExpressions || tupleTypes || tupleConversions
				|| discards || localFunctions || deconstruction || patternMatching)
				return CSharp.LanguageVersion.CSharp7;
			if (awaitInCatchFinally || useExpressionBodyForCalculatedGetterOnlyProperties || nullPropagation
				|| stringInterpolation || dictionaryInitializers || extensionMethodsInCollectionInitializers
				|| useRefLocalsForAccurateOrderOfEvaluation || getterOnlyAutomaticProperties)
				return CSharp.LanguageVersion.CSharp6;
			if (asyncAwait)
				return CSharp.LanguageVersion.CSharp5;
			if (dynamic || namedArguments || optionalArguments)
				return CSharp.LanguageVersion.CSharp4;
			if (anonymousTypes || objectCollectionInitializers || automaticProperties
				|| queryExpressions || expressionTrees)
				return CSharp.LanguageVersion.CSharp3;
			if (anonymousMethods || liftNullables || yieldReturn || useImplicitMethodGroupConversion)
				return CSharp.LanguageVersion.CSharp2;
			return CSharp.LanguageVersion.CSharp1;
		}

		bool nativeIntegers = true;

		/// <summary>
		/// Use C# 9 <c>nint</c>/<c>nuint</c> types.
		/// </summary>
		[Category("C# 9.0 / VS 2019.8")]
		[Description("DecompilerSettings.NativeIntegers")]
		public bool NativeIntegers {
			get { return nativeIntegers; }
			set {
				if (nativeIntegers != value)
				{
					nativeIntegers = value;
					OnPropertyChanged();
				}
			}
		}

		bool numericIntPtr = true;

		/// <summary>
		/// Treat <c>IntPtr</c>/<c>UIntPtr</c> as <c>nint</c>/<c>nuint</c>.
		/// </summary>
		[Category("C# 11.0 / VS 2022.4")]
		[Description("DecompilerSettings.NumericIntPtr")]
		public bool NumericIntPtr {
			get { return numericIntPtr; }
			set {
				if (numericIntPtr != value)
				{
					numericIntPtr = value;
					OnPropertyChanged();
				}
			}
		}

		bool covariantReturns = true;

		/// <summary>
		/// Decompile C# 9 covariant return types.
		/// </summary>
		[Category("C# 9.0 / VS 2019.8")]
		[Description("DecompilerSettings.CovariantReturns")]
		public bool CovariantReturns {
			get { return covariantReturns; }
			set {
				if (covariantReturns != value)
				{
					covariantReturns = value;
					OnPropertyChanged();
				}
			}
		}

		bool initAccessors = true;

		/// <summary>
		/// Use C# 9 <c>init;</c> property accessors.
		/// </summary>
		[Category("C# 9.0 / VS 2019.8")]
		[Description("DecompilerSettings.InitAccessors")]
		public bool InitAccessors {
			get { return initAccessors; }
			set {
				if (initAccessors != value)
				{
					initAccessors = value;
					OnPropertyChanged();
				}
			}
		}

		bool recordClasses = true;

		/// <summary>
		/// Use C# 9 <c>record</c> classes.
		/// </summary>
		[Category("C# 9.0 / VS 2019.8")]
		[Description("DecompilerSettings.RecordClasses")]
		public bool RecordClasses {
			get { return recordClasses; }
			set {
				if (recordClasses != value)
				{
					recordClasses = value;
					OnPropertyChanged();
				}
			}
		}

		bool recordStructs = true;

		/// <summary>
		/// Use C# 10 <c>record</c> structs.
		/// </summary>
		[Category("C# 10.0 / VS 2022")]
		[Description("DecompilerSettings.RecordStructs")]
		public bool RecordStructs {
			get { return recordStructs; }
			set {
				if (recordStructs != value)
				{
					recordStructs = value;
					OnPropertyChanged();
				}
			}
		}

		bool withExpressions = true;

		/// <summary>
		/// Use C# 9 <c>with</c> initializer expressions.
		/// </summary>
		[Category("C# 9.0 / VS 2019.8")]
		[Description("DecompilerSettings.WithExpressions")]
		public bool WithExpressions {
			get { return withExpressions; }
			set {
				if (withExpressions != value)
				{
					withExpressions = value;
					OnPropertyChanged();
				}
			}
		}

		bool usePrimaryConstructorSyntax = true;

		/// <summary>
		/// Use primary constructor syntax with records.
		/// </summary>
		[Category("C# 9.0 / VS 2019.8")]
		[Description("DecompilerSettings.UsePrimaryConstructorSyntax")]
		public bool UsePrimaryConstructorSyntax {
			get { return usePrimaryConstructorSyntax; }
			set {
				if (usePrimaryConstructorSyntax != value)
				{
					usePrimaryConstructorSyntax = value;
					OnPropertyChanged();
				}
			}
		}

		bool functionPointers = true;

		/// <summary>
		/// Use C# 9 <c>delegate* unmanaged</c> types.
		/// If this option is disabled, function pointers will instead be decompiled with type `IntPtr`.
		/// </summary>
		[Category("C# 9.0 / VS 2019.8")]
		[Description("DecompilerSettings.FunctionPointers")]
		public bool FunctionPointers {
			get { return functionPointers; }
			set {
				if (functionPointers != value)
				{
					functionPointers = value;
					OnPropertyChanged();
				}
			}
		}

		bool scopedRef = true;

		/// <summary>
		/// Use C# 11 <c>scoped</c> modifier.
		/// </summary>
		[Category("C# 11.0 / VS 2022.4")]
		[Description("DecompilerSettings.ScopedRef")]
		public bool ScopedRef {
			get { return scopedRef; }
			set {
				if (scopedRef != value)
				{
					scopedRef = value;
					OnPropertyChanged();
				}
			}
		}

		[Obsolete("Renamed to ScopedRef. This property will be removed in a future version of the decompiler.")]
		[Browsable(false)]
		public bool LifetimeAnnotations {
			get { return ScopedRef; }
			set { ScopedRef = value; }
		}

		bool requiredMembers = true;

		/// <summary>
		/// Use C# 11 <c>required</c> modifier.
		/// </summary>
		[Category("C# 11.0 / VS 2022.4")]
		[Description("DecompilerSettings.RequiredMembers")]
		public bool RequiredMembers {
			get { return requiredMembers; }
			set {
				if (requiredMembers != value)
				{
					requiredMembers = value;
					OnPropertyChanged();
				}
			}
		}

		bool switchExpressions = true;

		/// <summary>
		/// Use C# 8 switch expressions.
		/// </summary>
		[Category("C# 8.0 / VS 2019")]
		[Description("DecompilerSettings.SwitchExpressions")]
		public bool SwitchExpressions {
			get { return switchExpressions; }
			set {
				if (switchExpressions != value)
				{
					switchExpressions = value;
					OnPropertyChanged();
				}
			}
		}

		bool fileScopedNamespaces = true;

		/// <summary>
		/// Use C# 10 file-scoped namespaces.
		/// </summary>
		[Category("C# 10.0 / VS 2022")]
		[Description("DecompilerSettings.FileScopedNamespaces")]
		public bool FileScopedNamespaces {
			get { return fileScopedNamespaces; }
			set {
				if (fileScopedNamespaces != value)
				{
					fileScopedNamespaces = value;
					OnPropertyChanged();
				}
			}
		}

		bool anonymousMethods = true;

		/// <summary>
		/// Decompile anonymous methods/lambdas.
		/// </summary>
		[Category("C# 2.0 / VS 2005")]
		[Description("DecompilerSettings.DecompileAnonymousMethodsLambdas")]
		public bool AnonymousMethods {
			get { return anonymousMethods; }
			set {
				if (anonymousMethods != value)
				{
					anonymousMethods = value;
					OnPropertyChanged();
				}
			}
		}

		bool anonymousTypes = true;

		/// <summary>
		/// Decompile anonymous types.
		/// </summary>
		[Category("C# 3.0 / VS 2008")]
		[Description("DecompilerSettings.DecompileAnonymousTypes")]
		public bool AnonymousTypes {
			get { return anonymousTypes; }
			set {
				if (anonymousTypes != value)
				{
					anonymousTypes = value;
					OnPropertyChanged();
				}
			}
		}

		bool useLambdaSyntax = true;

		/// <summary>
		/// Use C# 3 lambda syntax if possible.
		/// </summary>
		[Category("C# 3.0 / VS 2008")]
		[Description("DecompilerSettings.UseLambdaSyntaxIfPossible")]
		public bool UseLambdaSyntax {
			get { return useLambdaSyntax; }
			set {
				if (useLambdaSyntax != value)
				{
					useLambdaSyntax = value;
					OnPropertyChanged();
				}
			}
		}

		bool expressionTrees = true;

		/// <summary>
		/// Decompile expression trees.
		/// </summary>
		[Category("C# 3.0 / VS 2008")]
		[Description("DecompilerSettings.DecompileExpressionTrees")]
		public bool ExpressionTrees {
			get { return expressionTrees; }
			set {
				if (expressionTrees != value)
				{
					expressionTrees = value;
					OnPropertyChanged();
				}
			}
		}

		bool yieldReturn = true;

		/// <summary>
		/// Decompile enumerators.
		/// </summary>
		[Category("C# 2.0 / VS 2005")]
		[Description("DecompilerSettings.DecompileEnumeratorsYieldReturn")]
		public bool YieldReturn {
			get { return yieldReturn; }
			set {
				if (yieldReturn != value)
				{
					yieldReturn = value;
					OnPropertyChanged();
				}
			}
		}

		bool dynamic = true;

		/// <summary>
		/// Decompile use of the 'dynamic' type.
		/// </summary>
		[Category("C# 4.0 / VS 2010")]
		[Description("DecompilerSettings.DecompileUseOfTheDynamicType")]
		public bool Dynamic {
			get { return dynamic; }
			set {
				if (dynamic != value)
				{
					dynamic = value;
					OnPropertyChanged();
				}
			}
		}

		bool asyncAwait = true;

		/// <summary>
		/// Decompile async methods.
		/// </summary>
		[Category("C# 5.0 / VS 2012")]
		[Description("DecompilerSettings.DecompileAsyncMethods")]
		public bool AsyncAwait {
			get { return asyncAwait; }
			set {
				if (asyncAwait != value)
				{
					asyncAwait = value;
					OnPropertyChanged();
				}
			}
		}

		bool awaitInCatchFinally = true;

		/// <summary>
		/// Decompile await in catch/finally blocks.
		/// Only has an effect if <see cref="AsyncAwait"/> is enabled.
		/// </summary>
		[Category("C# 6.0 / VS 2015")]
		[Description("DecompilerSettings.DecompileAwaitInCatchFinallyBlocks")]
		public bool AwaitInCatchFinally {
			get { return awaitInCatchFinally; }
			set {
				if (awaitInCatchFinally != value)
				{
					awaitInCatchFinally = value;
					OnPropertyChanged();
				}
			}
		}

		bool asyncEnumerator = true;

		/// <summary>
		/// Decompile IAsyncEnumerator/IAsyncEnumerable.
		/// Only has an effect if <see cref="AsyncAwait"/> is enabled.
		/// </summary>
		[Category("C# 8.0 / VS 2019")]
		[Description("DecompilerSettings.AsyncEnumerator")]
		public bool AsyncEnumerator {
			get { return asyncEnumerator; }
			set {
				if (asyncEnumerator != value)
				{
					asyncEnumerator = value;
					OnPropertyChanged();
				}
			}
		}

		bool decimalConstants = true;

		/// <summary>
		/// Decompile [DecimalConstant(...)] as simple literal values.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DecompileDecimalConstantAsSimpleLiteralValues")]
		public bool DecimalConstants {
			get { return decimalConstants; }
			set {
				if (decimalConstants != value)
				{
					decimalConstants = value;
					OnPropertyChanged();
				}
			}
		}

		bool fixedBuffers = true;

		/// <summary>
		/// Decompile C# 1.0 'public unsafe fixed int arr[10];' members.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DecompileC10PublicUnsafeFixedIntArr10Members")]
		public bool FixedBuffers {
			get { return fixedBuffers; }
			set {
				if (fixedBuffers != value)
				{
					fixedBuffers = value;
					OnPropertyChanged();
				}
			}
		}

		bool stringConcat = true;

		/// <summary>
		/// Decompile 'string.Concat(a, b)' calls into 'a + b'.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.StringConcat")]
		public bool StringConcat {
			get { return stringConcat; }
			set {
				if (stringConcat != value)
				{
					stringConcat = value;
					OnPropertyChanged();
				}
			}
		}

		bool liftNullables = true;

		/// <summary>
		/// Use lifted operators for nullables.
		/// </summary>
		[Category("C# 2.0 / VS 2005")]
		[Description("DecompilerSettings.UseLiftedOperatorsForNullables")]
		public bool LiftNullables {
			get { return liftNullables; }
			set {
				if (liftNullables != value)
				{
					liftNullables = value;
					OnPropertyChanged();
				}
			}
		}

		bool nullPropagation = true;

		/// <summary>
		/// Decompile C# 6 ?. and ?[] operators.
		/// </summary>
		[Category("C# 6.0 / VS 2015")]
		[Description("DecompilerSettings.NullPropagation")]
		public bool NullPropagation {
			get { return nullPropagation; }
			set {
				if (nullPropagation != value)
				{
					nullPropagation = value;
					OnPropertyChanged();
				}
			}
		}

		bool automaticProperties = true;

		/// <summary>
		/// Decompile automatic properties
		/// </summary>
		[Category("C# 3.0 / VS 2008")]
		[Description("DecompilerSettings.DecompileAutomaticProperties")]
		public bool AutomaticProperties {
			get { return automaticProperties; }
			set {
				if (automaticProperties != value)
				{
					automaticProperties = value;
					OnPropertyChanged();
				}
			}
		}

		bool getterOnlyAutomaticProperties = true;

		/// <summary>
		/// Decompile getter-only automatic properties
		/// </summary>
		[Category("C# 6.0 / VS 2015")]
		[Description("DecompilerSettings.GetterOnlyAutomaticProperties")]
		public bool GetterOnlyAutomaticProperties {
			get { return getterOnlyAutomaticProperties; }
			set {
				if (getterOnlyAutomaticProperties != value)
				{
					getterOnlyAutomaticProperties = value;
					OnPropertyChanged();
				}
			}
		}

		bool automaticEvents = true;

		/// <summary>
		/// Decompile automatic events
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DecompileAutomaticEvents")]
		public bool AutomaticEvents {
			get { return automaticEvents; }
			set {
				if (automaticEvents != value)
				{
					automaticEvents = value;
					OnPropertyChanged();
				}
			}
		}

		bool usingStatement = true;

		/// <summary>
		/// Decompile using statements.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DetectUsingStatements")]
		public bool UsingStatement {
			get { return usingStatement; }
			set {
				if (usingStatement != value)
				{
					usingStatement = value;
					OnPropertyChanged();
				}
			}
		}

		bool useEnhancedUsing = true;

		/// <summary>
		/// Use enhanced using statements.
		/// </summary>
		[Category("C# 8.0 / VS 2019")]
		[Description("DecompilerSettings.UseEnhancedUsing")]
		public bool UseEnhancedUsing {
			get { return useEnhancedUsing; }
			set {
				if (useEnhancedUsing != value)
				{
					useEnhancedUsing = value;
					OnPropertyChanged();
				}
			}
		}

		bool alwaysUseBraces = true;

		/// <summary>
		/// Gets/Sets whether to use braces for single-statement-blocks. 
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.AlwaysUseBraces")]
		public bool AlwaysUseBraces {
			get { return alwaysUseBraces; }
			set {
				if (alwaysUseBraces != value)
				{
					alwaysUseBraces = value;
					OnPropertyChanged();
				}
			}
		}

		bool forEachStatement = true;

		/// <summary>
		/// Decompile foreach statements.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DetectForeachStatements")]
		public bool ForEachStatement {
			get { return forEachStatement; }
			set {
				if (forEachStatement != value)
				{
					forEachStatement = value;
					OnPropertyChanged();
				}
			}
		}

		bool forEachWithGetEnumeratorExtension = true;

		/// <summary>
		/// Support GetEnumerator extension methods in foreach.
		/// </summary>
		[Category("C# 9.0 / VS 2019.8")]
		[Description("DecompilerSettings.DecompileForEachWithGetEnumeratorExtension")]
		public bool ForEachWithGetEnumeratorExtension {
			get { return forEachWithGetEnumeratorExtension; }
			set {
				if (forEachWithGetEnumeratorExtension != value)
				{
					forEachWithGetEnumeratorExtension = value;
					OnPropertyChanged();
				}
			}
		}

		bool lockStatement = true;

		/// <summary>
		/// Decompile lock statements.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DetectLockStatements")]
		public bool LockStatement {
			get { return lockStatement; }
			set {
				if (lockStatement != value)
				{
					lockStatement = value;
					OnPropertyChanged();
				}
			}
		}

		bool switchStatementOnString = true;

		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DetectSwitchOnString")]
		public bool SwitchStatementOnString {
			get { return switchStatementOnString; }
			set {
				if (switchStatementOnString != value)
				{
					switchStatementOnString = value;
					OnPropertyChanged();
				}
			}
		}

		bool sparseIntegerSwitch = true;

		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.SparseIntegerSwitch")]
		public bool SparseIntegerSwitch {
			get { return sparseIntegerSwitch; }
			set {
				if (sparseIntegerSwitch != value)
				{
					sparseIntegerSwitch = value;
					OnPropertyChanged();
				}
			}
		}

		bool usingDeclarations = true;

		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.InsertUsingDeclarations")]
		public bool UsingDeclarations {
			get { return usingDeclarations; }
			set {
				if (usingDeclarations != value)
				{
					usingDeclarations = value;
					OnPropertyChanged();
				}
			}
		}

		bool extensionMethods = true;

		[Category("C# 3.0 / VS 2008")]
		[Description("DecompilerSettings.UseExtensionMethodSyntax")]
		public bool ExtensionMethods {
			get { return extensionMethods; }
			set {
				if (extensionMethods != value)
				{
					extensionMethods = value;
					OnPropertyChanged();
				}
			}
		}

		bool queryExpressions = true;

		[Category("C# 3.0 / VS 2008")]
		[Description("DecompilerSettings.UseLINQExpressionSyntax")]
		public bool QueryExpressions {
			get { return queryExpressions; }
			set {
				if (queryExpressions != value)
				{
					queryExpressions = value;
					OnPropertyChanged();
				}
			}
		}

		bool useImplicitMethodGroupConversion = true;

		/// <summary>
		/// Gets/Sets whether to use C# 2.0 method group conversions.
		/// true: <c>EventHandler h = this.OnClick;</c>
		/// false: <c>EventHandler h = new EventHandler(this.OnClick);</c>
		/// </summary>
		[Category("C# 2.0 / VS 2005")]
		[Description("DecompilerSettings.UseImplicitMethodGroupConversions")]
		public bool UseImplicitMethodGroupConversion {
			get { return useImplicitMethodGroupConversion; }
			set {
				if (useImplicitMethodGroupConversion != value)
				{
					useImplicitMethodGroupConversion = value;
					OnPropertyChanged();
				}
			}
		}

		bool alwaysCastTargetsOfExplicitInterfaceImplementationCalls = false;

		/// <summary>
		/// Gets/Sets whether to always cast targets to explicitly implemented methods.
		/// true: <c>((ISupportInitialize)pictureBox1).BeginInit();</c>
		/// false: <c>pictureBox1.BeginInit();</c>
		/// default: false
		/// </summary>
		[Category("Other")]
		[Description("DecompilerSettings.AlwaysCastTargetsOfExplicitInterfaceImplementationCalls")]
		public bool AlwaysCastTargetsOfExplicitInterfaceImplementationCalls {
			get { return alwaysCastTargetsOfExplicitInterfaceImplementationCalls; }
			set {
				if (alwaysCastTargetsOfExplicitInterfaceImplementationCalls != value)
				{
					alwaysCastTargetsOfExplicitInterfaceImplementationCalls = value;
					OnPropertyChanged();
				}
			}
		}

		bool alwaysQualifyMemberReferences = false;

		/// <summary>
		/// Gets/Sets whether to always qualify member references.
		/// true: <c>this.DoSomething();</c>
		/// false: <c>DoSomething();</c>
		/// default: false
		/// </summary>
		[Category("Other")]
		[Description("DecompilerSettings.AlwaysQualifyMemberReferences")]
		public bool AlwaysQualifyMemberReferences {
			get { return alwaysQualifyMemberReferences; }
			set {
				if (alwaysQualifyMemberReferences != value)
				{
					alwaysQualifyMemberReferences = value;
					OnPropertyChanged();
				}
			}
		}

		bool alwaysShowEnumMemberValues = false;

		/// <summary>
		/// Gets/Sets whether to always show enum member values.
		/// true: <c>enum Kind { A = 0, B = 1, C = 5 }</c>
		/// false: <c>enum Kind { A, B, C = 5 }</c>
		/// default: false
		/// </summary>
		[Category("Other")]
		[Description("DecompilerSettings.AlwaysShowEnumMemberValues")]
		public bool AlwaysShowEnumMemberValues {
			get { return alwaysShowEnumMemberValues; }
			set {
				if (alwaysShowEnumMemberValues != value)
				{
					alwaysShowEnumMemberValues = value;
					OnPropertyChanged();
				}
			}
		}

		bool useDebugSymbols = true;

		/// <summary>
		/// Gets/Sets whether to use variable names from debug symbols, if available.
		/// </summary>
		[Category("Other")]
		[Description("DecompilerSettings.UseVariableNamesFromDebugSymbolsIfAvailable")]
		public bool UseDebugSymbols {
			get { return useDebugSymbols; }
			set {
				if (useDebugSymbols != value)
				{
					useDebugSymbols = value;
					OnPropertyChanged();
				}
			}
		}

		bool arrayInitializers = true;

		/// <summary>
		/// Gets/Sets whether to use array initializers.
		/// If set to false, might produce non-compilable code.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.ArrayInitializerExpressions")]
		public bool ArrayInitializers {
			get { return arrayInitializers; }
			set {
				if (arrayInitializers != value)
				{
					arrayInitializers = value;
					OnPropertyChanged();
				}
			}
		}

		bool objectCollectionInitializers = true;

		/// <summary>
		/// Gets/Sets whether to use C# 3.0 object/collection initializers.
		/// </summary>
		[Category("C# 3.0 / VS 2008")]
		[Description("DecompilerSettings.ObjectCollectionInitializerExpressions")]
		public bool ObjectOrCollectionInitializers {
			get { return objectCollectionInitializers; }
			set {
				if (objectCollectionInitializers != value)
				{
					objectCollectionInitializers = value;
					OnPropertyChanged();
				}
			}
		}

		bool dictionaryInitializers = true;

		/// <summary>
		/// Gets/Sets whether to use C# 6.0 dictionary initializers.
		/// Only has an effect if ObjectOrCollectionInitializers is enabled.
		/// </summary>
		[Category("C# 6.0 / VS 2015")]
		[Description("DecompilerSettings.DictionaryInitializerExpressions")]
		public bool DictionaryInitializers {
			get { return dictionaryInitializers; }
			set {
				if (dictionaryInitializers != value)
				{
					dictionaryInitializers = value;
					OnPropertyChanged();
				}
			}
		}

		bool extensionMethodsInCollectionInitializers = true;

		/// <summary>
		/// Gets/Sets whether to use C# 6.0 Extension Add methods in collection initializers.
		/// Only has an effect if ObjectOrCollectionInitializers is enabled.
		/// </summary>
		[Category("C# 6.0 / VS 2015")]
		[Description("DecompilerSettings.AllowExtensionAddMethodsInCollectionInitializerExpressions")]
		public bool ExtensionMethodsInCollectionInitializers {
			get { return extensionMethodsInCollectionInitializers; }
			set {
				if (extensionMethodsInCollectionInitializers != value)
				{
					extensionMethodsInCollectionInitializers = value;
					OnPropertyChanged();
				}
			}
		}

		bool useRefLocalsForAccurateOrderOfEvaluation = true;

		/// <summary>
		/// Gets/Sets whether to use local ref variables in cases where this is necessary
		/// for re-compilation with a modern C# compiler to reproduce the same behavior
		/// as the original assembly produced with an old C# compiler that used an incorrect
		/// order of evaluation.
		/// See https://github.com/icsharpcode/ILSpy/issues/2050
		/// </summary>
		[Category("C# 6.0 / VS 2015")]
		[Description("DecompilerSettings.UseRefLocalsForAccurateOrderOfEvaluation")]
		public bool UseRefLocalsForAccurateOrderOfEvaluation {
			get { return useRefLocalsForAccurateOrderOfEvaluation; }
			set {
				if (useRefLocalsForAccurateOrderOfEvaluation != value)
				{
					useRefLocalsForAccurateOrderOfEvaluation = value;
					OnPropertyChanged();
				}
			}
		}

		bool refExtensionMethods = true;

		/// <summary>
		/// Gets/Sets whether to use C# 7.2 'ref' extension methods.
		/// </summary>
		[Category("C# 7.2 / VS 2017.4")]
		[Description("DecompilerSettings.AllowExtensionMethodSyntaxOnRef")]
		public bool RefExtensionMethods {
			get { return refExtensionMethods; }
			set {
				if (refExtensionMethods != value)
				{
					refExtensionMethods = value;
					OnPropertyChanged();
				}
			}
		}

		bool stringInterpolation = true;

		/// <summary>
		/// Gets/Sets whether to use C# 6.0 string interpolation
		/// </summary>
		[Category("C# 6.0 / VS 2015")]
		[Description("DecompilerSettings.UseStringInterpolation")]
		public bool StringInterpolation {
			get { return stringInterpolation; }
			set {
				if (stringInterpolation != value)
				{
					stringInterpolation = value;
					OnPropertyChanged();
				}
			}
		}

		bool utf8StringLiterals = true;

		/// <summary>
		/// Gets/Sets whether to use C# 11.0 UTF-8 string literals
		/// </summary>
		[Category("C# 11.0 / VS 2022.4")]
		[Description("DecompilerSettings.Utf8StringLiterals")]
		public bool Utf8StringLiterals {
			get { return utf8StringLiterals; }
			set {
				if (utf8StringLiterals != value)
				{
					utf8StringLiterals = value;
					OnPropertyChanged();
				}
			}
		}

		bool switchOnReadOnlySpanChar = true;

		/// <summary>
		/// Gets/Sets whether to use C# 11.0 switch on (ReadOnly)Span&lt;char&gt;
		/// </summary>
		[Category("C# 11.0 / VS 2022.4")]
		[Description("DecompilerSettings.SwitchOnReadOnlySpanChar")]
		public bool SwitchOnReadOnlySpanChar {
			get { return switchOnReadOnlySpanChar; }
			set {
				if (switchOnReadOnlySpanChar != value)
				{
					switchOnReadOnlySpanChar = value;
					OnPropertyChanged();
				}
			}
		}

		bool unsignedRightShift = true;

		/// <summary>
		/// Gets/Sets whether to use C# 11.0 unsigned right shift operator.
		/// </summary>
		[Category("C# 11.0 / VS 2022.4")]
		[Description("DecompilerSettings.UnsignedRightShift")]
		public bool UnsignedRightShift {
			get { return unsignedRightShift; }
			set {
				if (unsignedRightShift != value)
				{
					unsignedRightShift = value;
					OnPropertyChanged();
				}
			}
		}

		bool checkedOperators = true;

		/// <summary>
		/// Gets/Sets whether to use C# 11.0 user-defined checked operators.
		/// </summary>
		[Category("C# 11.0 / VS 2022.4")]
		[Description("DecompilerSettings.CheckedOperators")]
		public bool CheckedOperators {
			get { return checkedOperators; }
			set {
				if (checkedOperators != value)
				{
					checkedOperators = value;
					OnPropertyChanged();
				}
			}
		}

		bool showXmlDocumentation = true;

		/// <summary>
		/// Gets/Sets whether to include XML documentation comments in the decompiled code.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.IncludeXMLDocumentationCommentsInTheDecompiledCode")]
		public bool ShowXmlDocumentation {
			get { return showXmlDocumentation; }
			set {
				if (showXmlDocumentation != value)
				{
					showXmlDocumentation = value;
					OnPropertyChanged();
				}
			}
		}

		bool foldBraces = false;

		[Browsable(false)]
		public bool FoldBraces {
			get { return foldBraces; }
			set {
				if (foldBraces != value)
				{
					foldBraces = value;
					OnPropertyChanged();
				}
			}
		}

		bool expandMemberDefinitions = false;

		[Browsable(false)]
		public bool ExpandMemberDefinitions {
			get { return expandMemberDefinitions; }
			set {
				if (expandMemberDefinitions != value)
				{
					expandMemberDefinitions = value;
					OnPropertyChanged();
				}
			}
		}

		bool expandUsingDeclarations = false;

		[Browsable(false)]
		public bool ExpandUsingDeclarations {
			get { return expandUsingDeclarations; }
			set {
				if (expandUsingDeclarations != value)
				{
					expandUsingDeclarations = value;
					OnPropertyChanged();
				}
			}
		}

		bool decompileMemberBodies = true;

		/// <summary>
		/// Gets/Sets whether member bodies should be decompiled.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Browsable(false)]
		public bool DecompileMemberBodies {
			get { return decompileMemberBodies; }
			set {
				if (decompileMemberBodies != value)
				{
					decompileMemberBodies = value;
					OnPropertyChanged();
				}
			}
		}

		bool useExpressionBodyForCalculatedGetterOnlyProperties = true;

		/// <summary>
		/// Gets/Sets whether simple calculated getter-only property declarations
		/// should use expression body syntax.
		/// </summary>
		[Category("C# 6.0 / VS 2015")]
		[Description("DecompilerSettings.UseExpressionBodiedMemberSyntaxForGetOnlyProperties")]
		public bool UseExpressionBodyForCalculatedGetterOnlyProperties {
			get { return useExpressionBodyForCalculatedGetterOnlyProperties; }
			set {
				if (useExpressionBodyForCalculatedGetterOnlyProperties != value)
				{
					useExpressionBodyForCalculatedGetterOnlyProperties = value;
					OnPropertyChanged();
				}
			}
		}

		bool outVariables = true;

		/// <summary>
		/// Gets/Sets whether out variable declarations should be used when possible.
		/// </summary>
		[Category("C# 7.0 / VS 2017")]
		[Description("DecompilerSettings.UseOutVariableDeclarations")]
		public bool OutVariables {
			get { return outVariables; }
			set {
				if (outVariables != value)
				{
					outVariables = value;
					OnPropertyChanged();
				}
			}
		}

		bool discards = true;

		/// <summary>
		/// Gets/Sets whether discards should be used when possible.
		/// Only has an effect if <see cref="OutVariables"/> is enabled.
		/// </summary>
		[Category("C# 7.0 / VS 2017")]
		[Description("DecompilerSettings.UseDiscards")]
		public bool Discards {
			get { return discards; }
			set {
				if (discards != value)
				{
					discards = value;
					OnPropertyChanged();
				}
			}
		}

		bool introduceRefModifiersOnStructs = true;

		/// <summary>
		/// Gets/Sets whether IsByRefLikeAttribute should be replaced with 'ref' modifiers on structs.
		/// </summary>
		[Category("C# 7.2 / VS 2017.4")]
		[Description("DecompilerSettings.IsByRefLikeAttributeShouldBeReplacedWithRefModifiersOnStructs")]
		public bool IntroduceRefModifiersOnStructs {
			get { return introduceRefModifiersOnStructs; }
			set {
				if (introduceRefModifiersOnStructs != value)
				{
					introduceRefModifiersOnStructs = value;
					OnPropertyChanged();
				}
			}
		}

		bool introduceReadonlyAndInModifiers = true;

		/// <summary>
		/// Gets/Sets whether IsReadOnlyAttribute should be replaced with 'readonly' modifiers on structs
		/// and with the 'in' modifier on parameters.
		/// </summary>
		[Category("C# 7.2 / VS 2017.4")]
		[Description("DecompilerSettings." +
			"IsReadOnlyAttributeShouldBeReplacedWithReadonlyInModifiersOnStructsParameters")]
		public bool IntroduceReadonlyAndInModifiers {
			get { return introduceReadonlyAndInModifiers; }
			set {
				if (introduceReadonlyAndInModifiers != value)
				{
					introduceReadonlyAndInModifiers = value;
					OnPropertyChanged();
				}
			}
		}

		bool readOnlyMethods = true;

		[Category("C# 8.0 / VS 2019")]
		[Description("DecompilerSettings.ReadOnlyMethods")]
		public bool ReadOnlyMethods {
			get { return readOnlyMethods; }
			set {
				if (readOnlyMethods != value)
				{
					readOnlyMethods = value;
					OnPropertyChanged();
				}
			}
		}

		bool asyncUsingAndForEachStatement = true;

		[Category("C# 8.0 / VS 2019")]
		[Description("DecompilerSettings.DetectAsyncUsingAndForeachStatements")]
		public bool AsyncUsingAndForEachStatement {
			get { return asyncUsingAndForEachStatement; }
			set {
				if (asyncUsingAndForEachStatement != value)
				{
					asyncUsingAndForEachStatement = value;
					OnPropertyChanged();
				}
			}
		}

		bool introduceUnmanagedConstraint = true;

		/// <summary>
		/// If this option is active, [IsUnmanagedAttribute] on type parameters
		/// is replaced with "T : unmanaged" constraints.
		/// </summary>
		[Category("C# 7.3 / VS 2017.7")]
		[Description("DecompilerSettings." +
			"IsUnmanagedAttributeOnTypeParametersShouldBeReplacedWithUnmanagedConstraints")]
		public bool IntroduceUnmanagedConstraint {
			get { return introduceUnmanagedConstraint; }
			set {
				if (introduceUnmanagedConstraint != value)
				{
					introduceUnmanagedConstraint = value;
					OnPropertyChanged();
				}
			}
		}

		bool stackAllocInitializers = true;

		/// <summary>
		/// Gets/Sets whether C# 7.3 stackalloc initializers should be used.
		/// </summary>
		[Category("C# 7.3 / VS 2017.7")]
		[Description("DecompilerSettings.UseStackallocInitializerSyntax")]
		public bool StackAllocInitializers {
			get { return stackAllocInitializers; }
			set {
				if (stackAllocInitializers != value)
				{
					stackAllocInitializers = value;
					OnPropertyChanged();
				}
			}
		}

		bool patternBasedFixedStatement = true;

		/// <summary>
		/// Gets/Sets whether C# 7.3 pattern based fixed statement should be used.
		/// </summary>
		[Category("C# 7.3 / VS 2017.7")]
		[Description("DecompilerSettings.UsePatternBasedFixedStatement")]
		public bool PatternBasedFixedStatement {
			get { return patternBasedFixedStatement; }
			set {
				if (patternBasedFixedStatement != value)
				{
					patternBasedFixedStatement = value;
					OnPropertyChanged();
				}
			}
		}

		bool tupleTypes = true;

		/// <summary>
		/// Gets/Sets whether tuple type syntax <c>(int, string)</c>
		/// should be used for <c>System.ValueTuple</c>.
		/// </summary>
		[Category("C# 7.0 / VS 2017")]
		[Description("DecompilerSettings.UseTupleTypeSyntax")]
		public bool TupleTypes {
			get { return tupleTypes; }
			set {
				if (tupleTypes != value)
				{
					tupleTypes = value;
					OnPropertyChanged();
				}
			}
		}

		bool throwExpressions = true;

		/// <summary>
		/// Gets/Sets whether throw expressions should be used.
		/// </summary>
		[Category("C# 7.0 / VS 2017")]
		[Description("DecompilerSettings.UseThrowExpressions")]
		public bool ThrowExpressions {
			get { return throwExpressions; }
			set {
				if (throwExpressions != value)
				{
					throwExpressions = value;
					OnPropertyChanged();
				}
			}
		}

		bool tupleConversions = true;

		/// <summary>
		/// Gets/Sets whether implicit conversions between tuples
		/// should be used in the decompiled output.
		/// </summary>
		[Category("C# 7.0 / VS 2017")]
		[Description("DecompilerSettings.UseImplicitConversionsBetweenTupleTypes")]
		public bool TupleConversions {
			get { return tupleConversions; }
			set {
				if (tupleConversions != value)
				{
					tupleConversions = value;
					OnPropertyChanged();
				}
			}
		}

		bool tupleComparisons = true;

		/// <summary>
		/// Gets/Sets whether tuple comparisons should be detected.
		/// </summary>
		[Category("C# 7.3 / VS 2017.7")]
		[Description("DecompilerSettings.DetectTupleComparisons")]
		public bool TupleComparisons {
			get { return tupleComparisons; }
			set {
				if (tupleComparisons != value)
				{
					tupleComparisons = value;
					OnPropertyChanged();
				}
			}
		}

		bool namedArguments = true;

		/// <summary>
		/// Gets/Sets whether named arguments should be used.
		/// </summary>
		[Category("C# 4.0 / VS 2010")]
		[Description("DecompilerSettings.UseNamedArguments")]
		public bool NamedArguments {
			get { return namedArguments; }
			set {
				if (namedArguments != value)
				{
					namedArguments = value;
					OnPropertyChanged();
				}
			}
		}

		bool nonTrailingNamedArguments = true;

		/// <summary>
		/// Gets/Sets whether C# 7.2 non-trailing named arguments should be used.
		/// </summary>
		[Category("C# 7.2 / VS 2017.4")]
		[Description("DecompilerSettings.UseNonTrailingNamedArguments")]
		public bool NonTrailingNamedArguments {
			get { return nonTrailingNamedArguments; }
			set {
				if (nonTrailingNamedArguments != value)
				{
					nonTrailingNamedArguments = value;
					OnPropertyChanged();
				}
			}
		}

		bool optionalArguments = true;

		/// <summary>
		/// Gets/Sets whether optional arguments should be removed, if possible.
		/// </summary>
		[Category("C# 4.0 / VS 2010")]
		[Description("DecompilerSettings.RemoveOptionalArgumentsIfPossible")]
		public bool OptionalArguments {
			get { return optionalArguments; }
			set {
				if (optionalArguments != value)
				{
					optionalArguments = value;
					OnPropertyChanged();
				}
			}
		}

		bool localFunctions = true;

		/// <summary>
		/// Gets/Sets whether C# 7.0 local functions should be transformed.
		/// </summary>
		[Category("C# 7.0 / VS 2017")]
		[Description("DecompilerSettings.IntroduceLocalFunctions")]
		public bool LocalFunctions {
			get { return localFunctions; }
			set {
				if (localFunctions != value)
				{
					localFunctions = value;
					OnPropertyChanged();
				}
			}
		}

		bool deconstruction = true;

		/// <summary>
		/// Gets/Sets whether C# 7.0 deconstruction should be detected.
		/// </summary>
		[Category("C# 7.0 / VS 2017")]
		[Description("DecompilerSettings.Deconstruction")]
		public bool Deconstruction {
			get { return deconstruction; }
			set {
				if (deconstruction != value)
				{
					deconstruction = value;
					OnPropertyChanged();
				}
			}
		}

		bool patternMatching = true;

		/// <summary>
		/// Gets/Sets whether C# 7.0 pattern matching should be detected.
		/// </summary>
		[Category("C# 7.0 / VS 2017")]
		[Description("DecompilerSettings.PatternMatching")]
		public bool PatternMatching {
			get { return patternMatching; }
			set {
				if (patternMatching != value)
				{
					patternMatching = value;
					OnPropertyChanged();
				}
			}
		}

		bool recursivePatternMatching = true;

		/// <summary>
		/// Gets/Sets whether C# 8.0 recursive patterns should be detected.
		/// </summary>
		[Category("C# 8.0 / VS 2019")]
		[Description("DecompilerSettings.RecursivePatternMatching")]
		public bool RecursivePatternMatching {
			get { return recursivePatternMatching; }
			set {
				if (recursivePatternMatching != value)
				{
					recursivePatternMatching = value;
					OnPropertyChanged();
				}
			}
		}

		bool patternCombinators = true;

		/// <summary>
		/// Gets/Sets whether C# 9.0 and, or, not patterns should be detected.
		/// </summary>
		[Category("C# 9.0 / VS 2019.8")]
		[Description("DecompilerSettings.PatternCombinators")]
		public bool PatternCombinators {
			get { return patternCombinators; }
			set {
				if (patternCombinators != value)
				{
					patternCombinators = value;
					OnPropertyChanged();
				}
			}
		}

		bool relationalPatterns = true;

		/// <summary>
		/// Gets/Sets whether C# 9.0 relational patterns should be detected.
		/// </summary>
		[Category("C# 9.0 / VS 2019.8")]
		[Description("DecompilerSettings.RelationalPatterns")]
		public bool RelationalPatterns {
			get { return relationalPatterns; }
			set {
				if (relationalPatterns != value)
				{
					relationalPatterns = value;
					OnPropertyChanged();
				}
			}
		}

		bool staticLocalFunctions = true;

		/// <summary>
		/// Gets/Sets whether C# 8.0 static local functions should be transformed.
		/// </summary>
		[Category("C# 8.0 / VS 2019")]
		[Description("DecompilerSettings.IntroduceStaticLocalFunctions")]
		public bool StaticLocalFunctions {
			get { return staticLocalFunctions; }
			set {
				if (staticLocalFunctions != value)
				{
					staticLocalFunctions = value;
					OnPropertyChanged();
				}
			}
		}

		bool ranges = true;

		/// <summary>
		/// Gets/Sets whether C# 8.0 index and range syntax should be used.
		/// </summary>
		[Category("C# 8.0 / VS 2019")]
		[Description("DecompilerSettings.Ranges")]
		public bool Ranges {
			get { return ranges; }
			set {
				if (ranges != value)
				{
					ranges = value;
					OnPropertyChanged();
				}
			}
		}

		bool nullableReferenceTypes = true;

		/// <summary>
		/// Gets/Sets whether C# 8.0 nullable reference types are enabled.
		/// </summary>
		[Category("C# 8.0 / VS 2019")]
		[Description("DecompilerSettings.NullableReferenceTypes")]
		public bool NullableReferenceTypes {
			get { return nullableReferenceTypes; }
			set {
				if (nullableReferenceTypes != value)
				{
					nullableReferenceTypes = value;
					OnPropertyChanged();
				}
			}
		}

		bool showDebugInfo;

		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.ShowInfoFromDebugSymbolsIfAvailable")]
		[Browsable(false)]
		public bool ShowDebugInfo {
			get { return showDebugInfo; }
			set {
				if (showDebugInfo != value)
				{
					showDebugInfo = value;
					OnPropertyChanged();
				}
			}
		}

		#region Options to aid VB decompilation
		bool assumeArrayLengthFitsIntoInt32 = true;

		/// <summary>
		/// Gets/Sets whether the decompiler can assume that 'ldlen; conv.i4.ovf'
		/// does not throw an overflow exception.
		/// </summary>
		[Category("DecompilerSettings.VBSpecificOptions")]
		[Browsable(false)]
		public bool AssumeArrayLengthFitsIntoInt32 {
			get { return assumeArrayLengthFitsIntoInt32; }
			set {
				if (assumeArrayLengthFitsIntoInt32 != value)
				{
					assumeArrayLengthFitsIntoInt32 = value;
					OnPropertyChanged();
				}
			}
		}

		bool introduceIncrementAndDecrement = true;

		/// <summary>
		/// Gets/Sets whether to use increment and decrement operators
		/// </summary>
		[Category("DecompilerSettings.VBSpecificOptions")]
		[Browsable(false)]
		public bool IntroduceIncrementAndDecrement {
			get { return introduceIncrementAndDecrement; }
			set {
				if (introduceIncrementAndDecrement != value)
				{
					introduceIncrementAndDecrement = value;
					OnPropertyChanged();
				}
			}
		}

		bool makeAssignmentExpressions = true;

		/// <summary>
		/// Gets/Sets whether to use assignment expressions such as in while ((count = Do()) != 0) ;
		/// </summary>
		[Category("DecompilerSettings.VBSpecificOptions")]
		[Browsable(false)]
		public bool MakeAssignmentExpressions {
			get { return makeAssignmentExpressions; }
			set {
				if (makeAssignmentExpressions != value)
				{
					makeAssignmentExpressions = value;
					OnPropertyChanged();
				}
			}
		}

		#endregion

		#region Options to aid F# decompilation
		bool removeDeadCode = false;

		[Category("DecompilerSettings.FSpecificOptions")]
		[Description("DecompilerSettings.RemoveDeadAndSideEffectFreeCodeUseWithCaution")]
		public bool RemoveDeadCode {
			get { return removeDeadCode; }
			set {
				if (removeDeadCode != value)
				{
					removeDeadCode = value;
					OnPropertyChanged();
				}
			}
		}

		bool removeDeadStores = false;

		[Category("DecompilerSettings.FSpecificOptions")]
		[Description("DecompilerSettings.RemoveDeadStores")]
		public bool RemoveDeadStores {
			get { return removeDeadStores; }
			set {
				if (removeDeadStores != value)
				{
					removeDeadStores = value;
					OnPropertyChanged();
				}
			}
		}
		#endregion

		#region Assembly Load and Resolve options

		bool loadInMemory = false;

		[Browsable(false)]
		public bool LoadInMemory {
			get { return loadInMemory; }
			set {
				if (loadInMemory != value)
				{
					loadInMemory = value;
					OnPropertyChanged();
				}
			}
		}

		bool throwOnAssemblyResolveErrors = true;

		[Browsable(false)]
		public bool ThrowOnAssemblyResolveErrors {
			get { return throwOnAssemblyResolveErrors; }
			set {
				if (throwOnAssemblyResolveErrors != value)
				{
					throwOnAssemblyResolveErrors = value;
					OnPropertyChanged();
				}
			}
		}

		bool applyWindowsRuntimeProjections = true;

		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.ApplyWindowsRuntimeProjectionsOnLoadedAssemblies")]
		public bool ApplyWindowsRuntimeProjections {
			get { return applyWindowsRuntimeProjections; }
			set {
				if (applyWindowsRuntimeProjections != value)
				{
					applyWindowsRuntimeProjections = value;
					OnPropertyChanged();
				}
			}
		}

		bool autoLoadAssemblyReferences = true;

		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.AutoLoadAssemblyReferences")]
		public bool AutoLoadAssemblyReferences {
			get { return autoLoadAssemblyReferences; }
			set {
				if (autoLoadAssemblyReferences != value)
				{
					autoLoadAssemblyReferences = value;
					OnPropertyChanged();
				}
			}
		}

		#endregion

		bool forStatement = true;

		/// <summary>
		/// Gets/sets whether the decompiler should produce for loops.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.ForStatement")]
		public bool ForStatement {
			get { return forStatement; }
			set {
				if (forStatement != value)
				{
					forStatement = value;
					OnPropertyChanged();
				}
			}
		}

		bool doWhileStatement = true;

		/// <summary>
		/// Gets/sets whether the decompiler should produce do-while loops.
		/// </summary>
		[Category("C# 1.0 / VS .NET")]
		[Description("DecompilerSettings.DoWhileStatement")]
		public bool DoWhileStatement {
			get { return doWhileStatement; }
			set {
				if (doWhileStatement != value)
				{
					doWhileStatement = value;
					OnPropertyChanged();
				}
			}
		}

		bool refReadOnlyParameters = true;

		/// <summary>
		/// Gets/sets whether RequiresLocationAttribute on parameters should be replaced with 'ref readonly' modifiers.
		/// </summary>
		[Category("C# 12.0 / VS 2022.8")]
		[Description("DecompilerSettings.RefReadOnlyParameters")]
		public bool RefReadOnlyParameters {
			get { return refReadOnlyParameters; }
			set {
				if (refReadOnlyParameters != value)
				{
					refReadOnlyParameters = value;
					OnPropertyChanged();
				}
			}
		}

		bool separateLocalVariableDeclarations = false;

		/// <summary>
		/// Gets/sets whether the decompiler should separate local variable declarations
		/// from their initialization.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.SeparateLocalVariableDeclarations")]
		public bool SeparateLocalVariableDeclarations {
			get { return separateLocalVariableDeclarations; }
			set {
				if (separateLocalVariableDeclarations != value)
				{
					separateLocalVariableDeclarations = value;
					OnPropertyChanged();
				}
			}
		}

		bool useSdkStyleProjectFormat = true;

		/// <summary>
		/// Gets or sets a value indicating whether the new SDK style format
		/// shall be used for the generated project files.
		/// </summary>
		[Category("DecompilerSettings.ProjectExport")]
		[Description("DecompilerSettings.UseSdkStyleProjectFormat")]
		public bool UseSdkStyleProjectFormat {
			get { return useSdkStyleProjectFormat; }
			set {
				if (useSdkStyleProjectFormat != value)
				{
					useSdkStyleProjectFormat = value;
					OnPropertyChanged();
				}
			}
		}

		bool useNestedDirectoriesForNamespaces;

		/// <summary>
		/// Gets/sets whether namespaces and namespace-like identifiers should be split at '.'
		/// and each part should produce a new level of nesting in the output directory structure. 
		/// </summary>
		[Category("DecompilerSettings.ProjectExport")]
		[Description("DecompilerSettings.UseNestedDirectoriesForNamespaces")]
		public bool UseNestedDirectoriesForNamespaces {
			get { return useNestedDirectoriesForNamespaces; }
			set {
				if (useNestedDirectoriesForNamespaces != value)
				{
					useNestedDirectoriesForNamespaces = value;
					OnPropertyChanged();
				}
			}
		}

		bool aggressiveScalarReplacementOfAggregates = false;

		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.AggressiveScalarReplacementOfAggregates")]
		// TODO : Remove once https://github.com/icsharpcode/ILSpy/issues/2032 is fixed.
#if !DEBUG
		[Browsable(false)]
#endif
		public bool AggressiveScalarReplacementOfAggregates {
			get { return aggressiveScalarReplacementOfAggregates; }
			set {
				if (aggressiveScalarReplacementOfAggregates != value)
				{
					aggressiveScalarReplacementOfAggregates = value;
					OnPropertyChanged();
				}
			}
		}

		bool aggressiveInlining = false;

		/// <summary>
		/// If set to false (the default), the decompiler will inline local variables only when they occur
		/// in a context where the C# compiler is known to emit compiler-generated locals.
		/// If set to true, the decompiler will inline local variables whenever possible.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.AggressiveInlining")]
		public bool AggressiveInlining {
			get { return aggressiveInlining; }
			set {
				if (aggressiveInlining != value)
				{
					aggressiveInlining = value;
					OnPropertyChanged();
				}
			}
		}

		bool alwaysUseGlobal = false;

		/// <summary>
		/// Always fully qualify namespaces using the "global::" prefix.
		/// </summary>
		[Category("DecompilerSettings.Other")]
		[Description("DecompilerSettings.AlwaysUseGlobal")]
		public bool AlwaysUseGlobal {
			get { return alwaysUseGlobal; }
			set {
				if (alwaysUseGlobal != value)
				{
					alwaysUseGlobal = value;
					OnPropertyChanged();
				}
			}
		}

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

		public DecompilerSettings Clone()
		{
			DecompilerSettings settings = (DecompilerSettings)MemberwiseClone();
			if (csharpFormattingOptions != null)
				settings.csharpFormattingOptions = csharpFormattingOptions.Clone();
			settings.PropertyChanged = null;
			return settings;
		}
	}
}
