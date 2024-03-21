// Copyright (c) 2024 Yuriy Zatuchnyy
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

using System.Linq;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Analyzers;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers
{
	[TestFixture]
	public class AnalyzerScopeTests
	{
		public class TestClass
		{

		}


		[Test]
		public void WhenPublicNestedClass_ThenNotInfiniteLoop()
		{
			// Given
			ILSpyX.AssemblyList assemblyList = new ILSpyX.AssemblyList();
			var file = new PEFile(this.GetType().Assembly.Location);
			var td = file.Metadata.TypeDefinitions.First(td => td.GetFullTypeName(file.Metadata).Name == nameof(TestClass));

			Decompiler.Metadata.IAssemblyResolver assemblyResolver = new UniversalAssemblyResolver(null, false, null);
			ICompilation compilation = new DecompilerTypeSystem(file, assemblyResolver);
			ITypeResolveContext context = new CSharpResolver(compilation);
			var module = ((IModuleReference)file).Resolve(context) as MetadataModule;
			IEntity entity = module.GetDefinition(td);

			// When 
			var task = Task.Run(() => {
				var target = new AnalyzerScope(assemblyList, entity);
			});

			var result = Task.WaitAny(new[] { task, Task.Delay(500) }); // 0.5 seconds

			// Then
			Assert.That(result == 0, "The constructor should complete in less than 10 seconds");
		}

	}
}
