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
		/// This does not imply that the resulting
		/// </remarks>
		public DecompilerSettings(CSharp.LanguageVersion languageVersion)
		{
			// By default, all decompiler features are enabled.
			// Disable some of them based on language version:
			if (languageVersion < CSharp.LanguageVersion.CSharp2) {
				anonymousMethods = false;
				liftNullables = false;
				yieldReturn = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp3) {
				anonymousTypes = false;
				objectCollectionInitializers = false;
				automaticProperties = false;
				queryExpressions = false;
				expressionTrees = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp4) {
				// * dynamic (not supported yet)
				// * named and optional arguments (not supported yet)
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp5) {
				asyncAwait = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp6) {
				awaitInCatchFinally = false;
				useExpressionBodyForCalculatedGetterOnlyProperties = false;
				nullPropagation = false;
				stringInterpolation = false;
				dictionaryInitializers = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp7) {
				outVariables = false;
				discards = false;
			}
			if (languageVersion < CSharp.LanguageVersion.CSharp7_2) {
				introduceRefAndReadonlyModifiersOnStructs = false;
			}
		}

		bool anonymousMethods = true;

		/// <summary>
		/// Decompile anonymous methods/lambdas.
		/// </summary>
		public bool AnonymousMethods {
			get => anonymousMethods;
			set {
				if (anonymousMethods != value) {
					anonymousMethods = value;
					OnPropertyChanged();
				}
			}
		}

		bool anonymousTypes = true;

		/// <summary>
		/// Decompile anonymous types.
		/// </summary>
		public bool AnonymousTypes {
			get => anonymousTypes;
			set {
				if (anonymousTypes != value) {
					anonymousTypes = value;
					OnPropertyChanged();
				}
			}
		}

		bool expressionTrees = true;

		/// <summary>
		/// Decompile expression trees.
		/// </summary>
		public bool ExpressionTrees {
			get => expressionTrees;
			set {
				if (expressionTrees != value) {
					expressionTrees = value;
					OnPropertyChanged();
				}
			}
		}

		bool yieldReturn = true;

		/// <summary>
		/// Decompile enumerators.
		/// </summary>
		public bool YieldReturn {
			get => yieldReturn;
			set {
				if (yieldReturn != value) {
					yieldReturn = value;
					OnPropertyChanged();
				}
			}
		}

		bool asyncAwait = true;

		/// <summary>
		/// Decompile async methods.
		/// </summary>
		public bool AsyncAwait {
			get => asyncAwait;
			set {
				if (asyncAwait != value) {
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
		public bool AwaitInCatchFinally {
			get => awaitInCatchFinally;
			set {
				if (awaitInCatchFinally != value) {
					awaitInCatchFinally = value;
					OnPropertyChanged();
				}
			}
		}

		bool fixedBuffers = true;

		/// <summary>
		/// Decompile C# 1.0 'public unsafe fixed int arr[10];' members.
		/// </summary>
		public bool FixedBuffers {
			get => fixedBuffers;
			set {
				if (fixedBuffers != value) {
					fixedBuffers = value;
					OnPropertyChanged();
				}
			}
		}

		bool liftNullables = true;

		/// <summary>
		/// Use lifted operators for nullables.
		/// </summary>
		public bool LiftNullables {
			get => liftNullables;
			set {
				if (liftNullables != value) {
					liftNullables = value;
					OnPropertyChanged();
				}
			}
		}

		bool nullPropagation = true;

		/// <summary>
		/// Decompile C# 6 ?. and ?[] operators.
		/// </summary>
		public bool NullPropagation {
			get => nullPropagation;
			set {
				if (nullPropagation != value) {
					nullPropagation = value;
					OnPropertyChanged();
				}
			}
		}

		bool automaticProperties = true;

		/// <summary>
		/// Decompile automatic properties
		/// </summary>
		public bool AutomaticProperties {
			get => automaticProperties;
			set {
				if (automaticProperties != value) {
					automaticProperties = value;
					OnPropertyChanged();
				}
			}
		}

		bool automaticEvents = true;

		/// <summary>
		/// Decompile automatic events
		/// </summary>
		public bool AutomaticEvents {
			get => automaticEvents;
			set {
				if (automaticEvents != value) {
					automaticEvents = value;
					OnPropertyChanged();
				}
			}
		}

		bool usingStatement = true;

		/// <summary>
		/// Decompile using statements.
		/// </summary>
		public bool UsingStatement {
			get => usingStatement;
			set {
				if (usingStatement != value) {
					usingStatement = value;
					OnPropertyChanged();
				}
			}
		}

		bool alwaysUseBraces = true;

		/// <summary>
		/// Gets/Sets whether to use braces for single-statement-blocks. 
		/// </summary>
		public bool AlwaysUseBraces {
			get => alwaysUseBraces;
			set {
				if (alwaysUseBraces != value) {
					alwaysUseBraces = value;
					OnPropertyChanged();
				}
			}
		}

		bool forEachStatement = true;

		/// <summary>
		/// Decompile foreach statements.
		/// </summary>
		public bool ForEachStatement {
			get => forEachStatement;
			set {
				if (forEachStatement != value) {
					forEachStatement = value;
					OnPropertyChanged();
				}
			}
		}

		bool lockStatement = true;

		/// <summary>
		/// Decompile lock statements.
		/// </summary>
		public bool LockStatement {
			get => lockStatement;
			set {
				if (lockStatement != value) {
					lockStatement = value;
					OnPropertyChanged();
				}
			}
		}

		bool switchStatementOnString = true;

		public bool SwitchStatementOnString {
			get => switchStatementOnString;
			set {
				if (switchStatementOnString != value) {
					switchStatementOnString = value;
					OnPropertyChanged();
				}
			}
		}

		bool usingDeclarations = true;

		public bool UsingDeclarations {
			get => usingDeclarations;
			set {
				if (usingDeclarations != value) {
					usingDeclarations = value;
					OnPropertyChanged();
				}
			}
		}

		bool queryExpressions = true;

		public bool QueryExpressions {
			get => queryExpressions;
			set {
				if (queryExpressions != value) {
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
		public bool UseImplicitMethodGroupConversion {
			get => useImplicitMethodGroupConversion;
			set {
				if (useImplicitMethodGroupConversion != value) {
					useImplicitMethodGroupConversion = value;
					OnPropertyChanged();
				}
			}
		}

		bool fullyQualifyAmbiguousTypeNames = true;

		public bool FullyQualifyAmbiguousTypeNames {
			get => fullyQualifyAmbiguousTypeNames;
			set {
				if (fullyQualifyAmbiguousTypeNames != value) {
					fullyQualifyAmbiguousTypeNames = value;
					OnPropertyChanged();
				}
			}
		}

		bool useDebugSymbols = true;

		/// <summary>
		/// Gets/Sets whether to use variable names from debug symbols, if available.
		/// </summary>
		public bool UseDebugSymbols {
			get => useDebugSymbols;
			set {
				if (useDebugSymbols != value) {
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
		public bool ArrayInitializers
		{
			get => arrayInitializers;
			set
			{
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
		public bool ObjectOrCollectionInitializers {
			get => objectCollectionInitializers;
			set {
				if (objectCollectionInitializers != value) {
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
		public bool DictionaryInitializers {
			get => dictionaryInitializers;
			set {
				if (dictionaryInitializers != value) {
					dictionaryInitializers = value;
					OnPropertyChanged();
				}
			}
		}

		bool stringInterpolation = true;

		/// <summary>
		/// Gets/Sets whether to use C# 6.0 string interpolation
		/// </summary>
		public bool StringInterpolation {
			get => stringInterpolation;
			set {
				if (stringInterpolation != value) {
					stringInterpolation = value;
					OnPropertyChanged();
				}
			}
		}

		bool showXmlDocumentation = true;

		/// <summary>
		/// Gets/Sets whether to include XML documentation comments in the decompiled code.
		/// </summary>
		public bool ShowXmlDocumentation {
			get => showXmlDocumentation;
			set {
				if (showXmlDocumentation != value) {
					showXmlDocumentation = value;
					OnPropertyChanged();
				}
			}
		}

		bool foldBraces = false;

		public bool FoldBraces {
			get => foldBraces;
			set {
				if (foldBraces != value) {
					foldBraces = value;
					OnPropertyChanged();
				}
			}
		}

		bool expandMemberDefinitions = false;

		public bool ExpandMemberDefinitions {
			get => expandMemberDefinitions;
			set {
				if (expandMemberDefinitions != value) {
					expandMemberDefinitions = value;
					OnPropertyChanged();
				}
			}
		}

		bool decompileMemberBodies = true;

		/// <summary>
		/// Gets/Sets whether member bodies should be decompiled.
		/// </summary>
		public bool DecompileMemberBodies {
			get => decompileMemberBodies;
			set {
				if (decompileMemberBodies != value) {
					decompileMemberBodies = value;
					OnPropertyChanged();
				}
			}
		}

		bool useExpressionBodyForCalculatedGetterOnlyProperties = true;

		/// <summary>
		/// Gets/Sets whether simple calculated getter-only property declarations should use expression body syntax.
		/// </summary>
		public bool UseExpressionBodyForCalculatedGetterOnlyProperties {
			get => useExpressionBodyForCalculatedGetterOnlyProperties;
			set {
				if (useExpressionBodyForCalculatedGetterOnlyProperties != value) {
					useExpressionBodyForCalculatedGetterOnlyProperties = value;
					OnPropertyChanged();
				}
			}
		}

		bool outVariables = true;

		/// <summary>
		/// Gets/Sets whether out variable declarations should be used when possible.
		/// </summary>
		public bool OutVariables {
			get => outVariables;
			set {
				if (outVariables != value) {
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
		public bool Discards {
			get => discards;
			set {
				if (discards != value) {
					discards = value;
					OnPropertyChanged();
				}
			}
		}

		bool introduceRefAndReadonlyModifiersOnStructs = true;

		/// <summary>
		/// Gets/Sets whether IsByRefLikeAttribute and IsReadOnlyAttribute should be replaced with 'ref' and 'readonly' modifiers on structs.
		/// </summary>
		public bool IntroduceRefAndReadonlyModifiersOnStructs {
			get => introduceRefAndReadonlyModifiersOnStructs;
			set {
				if (introduceRefAndReadonlyModifiersOnStructs != value) {
					introduceRefAndReadonlyModifiersOnStructs = value;
					OnPropertyChanged();
				}
			}
		}

		#region Options to aid VB decompilation
		bool introduceIncrementAndDecrement = true;

		/// <summary>
		/// Gets/Sets whether to use increment and decrement operators
		/// </summary>
		public bool IntroduceIncrementAndDecrement {
			get => introduceIncrementAndDecrement;
			set {
				if (introduceIncrementAndDecrement != value) {
					introduceIncrementAndDecrement = value;
					OnPropertyChanged();
				}
			}
		}

		bool makeAssignmentExpressions = true;

		/// <summary>
		/// Gets/Sets whether to use assignment expressions such as in while ((count = Do()) != 0) ;
		/// </summary>
		public bool MakeAssignmentExpressions {
			get => makeAssignmentExpressions;
			set {
				if (makeAssignmentExpressions != value) {
					makeAssignmentExpressions = value;
					OnPropertyChanged();
				}
			}
		}

		bool alwaysGenerateExceptionVariableForCatchBlocks = false;

		/// <summary>
		/// Gets/Sets whether to always generate exception variables in catch blocks
		/// </summary>
		public bool AlwaysGenerateExceptionVariableForCatchBlocks {
			get => alwaysGenerateExceptionVariableForCatchBlocks;
			set {
				if (alwaysGenerateExceptionVariableForCatchBlocks != value) {
					alwaysGenerateExceptionVariableForCatchBlocks = value;
					OnPropertyChanged();
				}
			}
		}

		bool showDebugInfo;

		public bool ShowDebugInfo {
			get => showDebugInfo;
			set {
				if (showDebugInfo != value) {
					showDebugInfo = value;
					OnPropertyChanged();
				}
			}
		}

		#endregion

		#region Options to aid F# decompilation
		bool removeDeadCode = false;

		public bool RemoveDeadCode {
			get => removeDeadCode;
			set {
				if (removeDeadCode != value) {
					removeDeadCode = value;
					OnPropertyChanged();
				}
			}
		}
		#endregion

		#region Assembly Load and Resolve options

		bool loadInMemory = false;

		public bool LoadInMemory {
			get => loadInMemory;
			set {
				if (loadInMemory != value) {
					loadInMemory = value;
					OnPropertyChanged();
				}
			}
		}

		bool throwOnAssemblyResolveErrors = true;

		public bool ThrowOnAssemblyResolveErrors {
			get => throwOnAssemblyResolveErrors;
			set {
				if (throwOnAssemblyResolveErrors != value) {
					throwOnAssemblyResolveErrors = value;
					OnPropertyChanged();
				}
			}
		}

		#endregion

		CSharpFormattingOptions csharpFormattingOptions;

		public CSharpFormattingOptions CSharpFormattingOptions {
			get {
				if (csharpFormattingOptions == null) {
					csharpFormattingOptions = FormattingOptionsFactory.CreateAllman();
					csharpFormattingOptions.IndentSwitchBody = false;
					csharpFormattingOptions.ArrayInitializerWrapping = Wrapping.WrapAlways;
				}
				return csharpFormattingOptions;
			}
			set {
				if (value == null)
					throw new ArgumentNullException();
				if (csharpFormattingOptions != value) {
					csharpFormattingOptions = value;
					OnPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public DecompilerSettings Clone()
		{
			var settings = (DecompilerSettings)MemberwiseClone();
			if (csharpFormattingOptions != null)
				settings.csharpFormattingOptions = csharpFormattingOptions.Clone();
			settings.PropertyChanged = null;
			return settings;
		}
	}
}
