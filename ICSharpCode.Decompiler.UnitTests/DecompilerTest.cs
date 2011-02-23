using Decompiler;
using Decompiler.Transforms;
using ICSharpCode.Decompiler;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ICSharpCode.Decompiler
{
    public abstract class DecompilerTest
    {
        AssemblyDefinition targetAssembly;

        /// <summary>
        ///   Gets the assembly that will be used for decompilation.
        /// </summary>
        /// <value>
        ///   The default <see cref="AssemblyDefinition"/> is the Decompile.UnitTests assembly.
        /// </value>
        public AssemblyDefinition TargetAssembly
        {
            get
            {
                if (targetAssembly == null)
                    targetAssembly = AssemblyDefinition.ReadAssembly(Assembly.GetExecutingAssembly().Location);
                return targetAssembly;
            }
            set
            {
                targetAssembly = value;
            }
        }

        public string DecompileMethod(Type @class, string methodName)
        {
            var type = TargetAssembly.MainModule.GetType(@class.FullName.Replace('+', '/'));
            if (type == null)
                throw new Exception(string.Format("Can not locate type '{0}' in assembly '{1}'", @class.FullName, TargetAssembly.FullName));
            var method = type.Methods.FirstOrDefault(m => m.Name == methodName);
            if (method == null)
                throw new Exception(string.Format("Can not locate method '{0}' in type '{1}'", methodName, type.FullName));

            StringWriter source = new StringWriter();
            AstBuilder decompiler = new AstBuilder(new DecompilerContext() { CurrentType = method.DeclaringType });
            decompiler.AddMethod(method);
            decompiler.GenerateCode(new ConciseTextOutput(source));

            return source.ToString();
        }

        /// <summary>
        ///   Don't generate newlines and indentation.  This is much easier for Assert.AreEqual(...)
        /// </summary>
        public sealed class ConciseTextOutput : ITextOutput
        {
            readonly TextWriter writer;

            public ConciseTextOutput(TextWriter writer)
            {
                this.writer = writer;
            }


            public void Indent()
            {
            }

            public void Unindent()
            {
            }

            void WriteIndent()
            {
            }

            public void Write(char ch)
            {
                writer.Write(ch);
            }

            public void Write(string text)
            {
                writer.Write(text);
            }

            public void WriteLine()
            {
            }

            public void WriteDefinition(string text, object definition)
            {
                Write(text);
            }

            public void WriteReference(string text, object reference)
            {
                Write(text);
            }

            void ITextOutput.MarkFoldStart(string collapsedText, bool defaultCollapsed)
            {
            }

            void ITextOutput.MarkFoldEnd()
            {
            }
        }
    }
}
