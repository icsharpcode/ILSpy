using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using Mono.Cecil;

namespace ICSharpCode.ILSpy
{
	[Export(typeof(Language))]
	class CSharpILMixedLanguage : Language
	{
		private readonly bool detectControlStructure = true;

		public override string Name => "IL with C#";

		public override string FileExtension => ".il";

		CSharpDecompiler CreateDecompiler(ModuleDefinition module, DecompilationOptions options)
		{
			CSharpDecompiler decompiler = new CSharpDecompiler(module, options.DecompilerSettings);
			decompiler.CancellationToken = options.CancellationToken;
			return decompiler;
		}

		void WriteCode(TextWriter output, DecompilerSettings settings, SyntaxTree syntaxTree, IDecompilerTypeSystem typeSystem)
		{
			syntaxTree.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
			TokenWriter tokenWriter = new TextWriterTokenWriter(output);
			tokenWriter = TokenWriter.WrapInWriterThatSetsLocationsInAST(tokenWriter);
			syntaxTree.AcceptVisitor(new CSharpOutputVisitor(tokenWriter, settings.CSharpFormattingOptions));
		}

		public override void DecompileMethod(MethodDefinition method, ITextOutput output, DecompilationOptions options)
		{
			//AddReferenceWarningMessage(method.Module.Assembly, output);
			var csharpOutput = new StringWriter();
			CSharpDecompiler decompiler = CreateDecompiler(method.Module, options);
			var st = decompiler.Decompile(method);
			WriteCode(csharpOutput, options.DecompilerSettings, st, decompiler.TypeSystem);
			var sequencePoints = (IList<SequencePoint>)decompiler.CreateSequencePoints(st).FirstOrDefault(kvp => kvp.Key.CecilMethod == method).Value ?? EmptyList<SequencePoint>.Instance;
			var codeLines = csharpOutput.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			WriteCommentLine(output, TypeToString(method.DeclaringType, includeNamespace: true));
			var methodDisassembler = new MixedMethodBodyDisassembler(output, codeLines, sequencePoints, detectControlStructure, options.CancellationToken);
			var dis = new ReflectionDisassembler(output, methodDisassembler, options.CancellationToken);
			dis.DisassembleMethod(method);
		}

		class MixedMethodBodyDisassembler : MethodBodyDisassembler
		{
			// list sorted by IL offset
			IList<SequencePoint> sequencePoints;
			// lines of raw c# source code
			string[] codeLines;

			public MixedMethodBodyDisassembler(ITextOutput output, string[] codeLines, IList<SequencePoint> sequencePoints, bool detectControlStructure, CancellationToken cancellationToken)
				: base(output, detectControlStructure, cancellationToken)
			{
				if (codeLines == null)
					throw new ArgumentNullException(nameof(codeLines));
				if (sequencePoints == null)
					throw new ArgumentNullException(nameof(sequencePoints));

				this.codeLines = codeLines;
				this.sequencePoints = sequencePoints;
			}

			protected override void WriteInstruction(ITextOutput output, Mono.Cecil.Cil.Instruction instruction)
			{
				int index = sequencePoints.BinarySearch(instruction.Offset, seq => seq.Offset);
				if (index >= 0) {
					var info = sequencePoints[index];
					if (!info.IsHidden) {
						for (int line = info.StartLine; line <= info.EndLine; line++) {
							if (output is ISmartTextOutput highlightingOutput) {
								string text = codeLines[line - 1];
								int startColumn = 1;
								int endColumn = text.Length + 1;
								if (line == info.StartLine)
									startColumn = info.StartColumn;
								if (line == info.EndLine)
									endColumn = info.EndColumn;
								WriteHighlightedCommentLine(highlightingOutput, text, startColumn - 1, endColumn - 1);
							} else
								WriteCommentLine(output, codeLines[line - 1]);
						}
					} else {
						WriteCommentLine(output, "no code");
					}
				}
				base.WriteInstruction(output, instruction);
			}

			HighlightingColor gray = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.DarkGray) };
			HighlightingColor black = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.Black) };

			void WriteHighlightedCommentLine(ISmartTextOutput output, string text, int startColumn, int endColumn)
			{
				output.BeginSpan(gray);
				output.Write("// " + text.Substring(0, startColumn));
				output.BeginSpan(black);
				output.Write(text.Substring(startColumn, endColumn - startColumn));
				output.EndSpan();
				output.Write(text.Substring(endColumn));
				output.EndSpan();
				output.WriteLine();
			}

			void WriteCommentLine(ITextOutput output, string text)
			{
				output.WriteLine("// " + text);
			}
		}
	}
}
