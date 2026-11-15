// Copyright (c) 2026 Franck
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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using Redecomp.Core.Naming;

namespace Redecomp.Core.Reconstruction;

/// <summary>The ILSpy project decompiler with the Redecomp transforms added to its pipeline.</summary>
sealed class RedecompProjectDecompiler(DecompilerSettings settings, IAssemblyResolver resolver, IDebugInfoProvider? debugInfo, ReconstructionOptions options)
	: WholeProjectDecompiler(settings, resolver, null, null, debugInfo)
{
	protected override CSharpDecompiler CreateDecompiler(DecompilerTypeSystem ts)
	{
		var decompiler = base.CreateDecompiler(ts);
		if (!options.DeterministicNaming)
			return decompiler;
		var transforms = decompiler.ILTransforms;
		int index = transforms.IndexOf(transforms.OfType<AssignVariableNames>().Single());
		transforms.Insert(index, new DeterministicVariableNames());
		return decompiler;
	}
}
