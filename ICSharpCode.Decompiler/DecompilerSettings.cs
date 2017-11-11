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
		bool anonymousMethods = true;

		/// <summary>
		/// Decompile anonymous methods/lambdas.
		/// </summary>
		public bool AnonymousMethods {
			get { return anonymousMethods; }
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
			get { return anonymousTypes; }
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
			get { return expressionTrees; }
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
			get { return yieldReturn; }
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
			get { return asyncAwait; }
			set {
				if (asyncAwait != value) {
					asyncAwait = value;
					OnPropertyChanged();
				}
			}
		}

		bool fixedBuffers = true;

		/// <summary>
		/// Decompile C# 1.0 'public unsafe fixed int arr[10];' members.
		/// </summary>
		public bool FixedBuffers {
			get { return fixedBuffers; }
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
			get { return liftNullables; }
			set {
				if (liftNullables != value) {
					liftNullables = value;
					OnPropertyChanged();
				}
			}
		}

		bool automaticProperties = true;

		/// <summary>
		/// Decompile automatic properties
		/// </summary>
		public bool AutomaticProperties {
			get { return automaticProperties; }
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
			get { return automaticEvents; }
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
			get { return usingStatement; }
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
			get { return alwaysUseBraces; }
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
			get { return forEachStatement; }
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
			get { return lockStatement; }
			set {
				if (lockStatement != value) {
					lockStatement = value;
					OnPropertyChanged();
				}
			}
		}

		bool switchStatementOnString = true;

		public bool SwitchStatementOnString {
			get { return switchStatementOnString; }
			set {
				if (switchStatementOnString != value) {
					switchStatementOnString = value;
					OnPropertyChanged();
				}
			}
		}

		bool usingDeclarations = true;

		public bool UsingDeclarations {
			get { return usingDeclarations; }
			set {
				if (usingDeclarations != value) {
					usingDeclarations = value;
					OnPropertyChanged();
				}
			}
		}

		bool queryExpressions = true;

		public bool QueryExpressions {
			get { return queryExpressions; }
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
			get { return useImplicitMethodGroupConversion; }
			set {
				if (useImplicitMethodGroupConversion != value) {
					useImplicitMethodGroupConversion = value;
					OnPropertyChanged();
				}
			}
		}

		bool fullyQualifyAmbiguousTypeNames = true;

		public bool FullyQualifyAmbiguousTypeNames {
			get { return fullyQualifyAmbiguousTypeNames; }
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
			get { return useDebugSymbols; }
			set {
				if (useDebugSymbols != value) {
					useDebugSymbols = value;
					OnPropertyChanged();
				}
			}
		}

		bool objectCollectionInitializers = true;

		/// <summary>
		/// Gets/Sets whether to use C# 3.0 object/collection initializers
		/// </summary>
		public bool ObjectOrCollectionInitializers {
			get { return objectCollectionInitializers; }
			set {
				if (objectCollectionInitializers != value) {
					objectCollectionInitializers = value;
					OnPropertyChanged();
				}
			}
		}

		bool showXmlDocumentation = true;

		/// <summary>
		/// Gets/Sets whether to include XML documentation comments in the decompiled code
		/// </summary>
		public bool ShowXmlDocumentation {
			get { return showXmlDocumentation; }
			set {
				if (showXmlDocumentation != value) {
					showXmlDocumentation = value;
					OnPropertyChanged();
				}
			}
		}

		bool foldBraces = false;

		public bool FoldBraces {
			get { return foldBraces; }
			set {
				if (foldBraces != value) {
					foldBraces = value;
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
			get { return introduceIncrementAndDecrement; }
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
			get { return makeAssignmentExpressions; }
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
			get { return alwaysGenerateExceptionVariableForCatchBlocks; }
			set {
				if (alwaysGenerateExceptionVariableForCatchBlocks != value) {
					alwaysGenerateExceptionVariableForCatchBlocks = value;
					OnPropertyChanged();
				}
			}
		}

		bool showDebugInfo;

		public bool ShowDebugInfo {
			get { return showDebugInfo; }
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
			get { return removeDeadCode; }
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
			get { return loadInMemory; }
			set {
				if (loadInMemory != value) {
					loadInMemory = value;
					OnPropertyChanged();
				}
			}
		}

		bool throwOnAssemblyResolveErrors = true;

		public bool ThrowOnAssemblyResolveErrors {
			get { return throwOnAssemblyResolveErrors; }
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
			DecompilerSettings settings = (DecompilerSettings)MemberwiseClone();
			if (csharpFormattingOptions != null)
				settings.csharpFormattingOptions = csharpFormattingOptions.Clone();
			settings.PropertyChanged = null;
			return settings;
		}
	}
}
