using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
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
using Mono.Cecil.Cil;

namespace ICSharpCode.ILSpy
{
	[Export(typeof(Language))]
	class CSharpILMixedLanguage : ILLanguage
	{
		public override string Name => "IL with C#";

		protected override ReflectionDisassembler CreateDisassembler(ITextOutput output, DecompilationOptions options)
		{
			return new ReflectionDisassembler(output, 
				new MixedMethodBodyDisassembler(output, options) {
					DetectControlStructure = detectControlStructure,
					ShowSequencePoints = options.DecompilerSettings.ShowDebugInfo
				},
				options.CancellationToken);
		}

		static CSharpDecompiler CreateDecompiler(ModuleDefinition module, DecompilationOptions options)
		{
			CSharpDecompiler decompiler = new CSharpDecompiler(module, options.DecompilerSettings);
			decompiler.CancellationToken = options.CancellationToken;
			return decompiler;
		}

		static void WriteCode(TextWriter output, DecompilerSettings settings, SyntaxTree syntaxTree, IDecompilerTypeSystem typeSystem)
		{
			syntaxTree.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
			TokenWriter tokenWriter = new TextWriterTokenWriter(output);
			tokenWriter = TokenWriter.WrapInWriterThatSetsLocationsInAST(tokenWriter);
			syntaxTree.AcceptVisitor(new CSharpOutputVisitor(tokenWriter, settings.CSharpFormattingOptions));
		}
		
		class MixedMethodBodyDisassembler : MethodBodyDisassembler
		{
			readonly DecompilationOptions options;
			// list sorted by IL offset
			IList<Decompiler.IL.SequencePoint> sequencePoints;
			// lines of raw c# source code
			string[] codeLines;

			public MixedMethodBodyDisassembler(ITextOutput output, DecompilationOptions options)
				: base(output, options.CancellationToken)
			{
				this.options = options;
			}

			public override void Disassemble(MethodBody body)
			{
				var method = body.Method;
				try {
					var csharpOutput = new StringWriter();
					CSharpDecompiler decompiler = CreateDecompiler(method.Module, options);
					var st = decompiler.Decompile(method);
					WriteCode(csharpOutput, options.DecompilerSettings, st, decompiler.TypeSystem);
					var mapping = decompiler.CreateSequencePoints(st).FirstOrDefault(kvp => kvp.Key.CecilMethod == method);
					this.sequencePoints = mapping.Value ?? (IList<Decompiler.IL.SequencePoint>)EmptyList<Decompiler.IL.SequencePoint>.Instance;
					this.codeLines = csharpOutput.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
					base.Disassemble(body);
				} finally {
					this.sequencePoints = null;
					this.codeLines = null;
				}
			}

			protected override void WriteInstruction(ITextOutput output, Instruction instruction)
			{
				int index = sequencePoints.BinarySearch(instruction.Offset, seq => seq.Offset);
				if (index >= 0) {
					var info = sequencePoints[index];
					var highlightingOutput = output as ISmartTextOutput;
					if (!info.IsHidden) {
						for (int line = info.StartLine; line <= info.EndLine; line++) {
							if (highlightingOutput != null) {
								string text = codeLines[line - 1];
								int startColumn = 1;
								int endColumn = text.Length + 1;
								if (line == info.StartLine)
									startColumn = info.StartColumn;
								if (line == info.EndLine)
									endColumn = info.EndColumn;
								WriteHighlightedCommentLine(highlightingOutput, text, startColumn - 1, endColumn - 1, info.StartLine == info.EndLine);
							} else
								WriteCommentLine(output, codeLines[line - 1]);
						}
					} else {
						output.Write("// ");
						highlightingOutput?.BeginSpan(gray);
						output.WriteLine("(no C# code)");
						highlightingOutput?.EndSpan();
					}
				}
				base.WriteInstruction(output, instruction);
			}

			HighlightingColor gray = new HighlightingColor { Foreground = new SimpleHighlightingBrush(Colors.DarkGray) };

			void WriteHighlightedCommentLine(ISmartTextOutput output, string text, int startColumn, int endColumn, bool isSingleLine)
			{
				if (startColumn > text.Length) {
					Debug.Fail("startColumn is invalid");
					startColumn = text.Length;
				}
				if (endColumn > text.Length) {
					Debug.Fail("endColumn is invalid");
					endColumn = text.Length;
				}
				output.Write("// ");
				output.BeginSpan(gray);
				if (isSingleLine)
					output.Write(text.Substring(0, startColumn).TrimStart());
				else
					output.Write(text.Substring(0, startColumn));
				output.EndSpan();
				output.Write(text.Substring(startColumn, endColumn - startColumn));
				output.BeginSpan(gray);
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
