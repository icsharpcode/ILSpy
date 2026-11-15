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

using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

namespace Redecomp.Core.Naming;

/// <summary>
/// Names local variables from facts read in the code, for variables that have no name from the
/// debug symbols. It runs just before <see cref="AssignVariableNames"/>, which keeps the names set
/// here and only resolves conflicts between them. Rules, by decreasing precedence:
/// <list type="number">
/// <item>the variable (or its <c>ToString()</c>) is returned: name derived from the method name;</item>
/// <item>the variable is assigned the result of a method of the decompiled assembly: name derived from that method;</item>
/// <item>the variable is assigned a LINQ aggregate: aggregate name (Sum gives "total") plus the selected member or the source collection.</item>
/// </list>
/// Variables that no rule covers keep the names the decompiler generates.
/// </summary>
public sealed class DeterministicVariableNames : IILTransform
{
	static readonly HashSet<string> Aggregates = ["Sum", "Count", "LongCount", "Max", "Min", "Average"];

	static readonly HashSet<string> CSharpKeywords = [
		"abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class",
		"const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event",
		"explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if",
		"implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null",
		"object", "operator", "out", "override", "params", "private", "protected", "public", "readonly",
		"ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct",
		"switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
		"ushort", "using", "virtual", "void", "volatile", "while",
	];

	public void Run(ILFunction function, ILTransformContext context)
	{
		var mainModule = context.TypeSystem.MainModule;
		foreach (var f in function.Descendants.OfType<ILFunction>())
		{
			foreach (var v in f.Variables)
			{
				if (!v.HasGeneratedName || v.Kind is not (VariableKind.Local or VariableKind.StackSlot))
					continue;
				string? name = NameFromReturn(f, v) ?? NameFromCallResult(v, mainModule) ?? NameFromAggregate(v);
				if (name == null || !IsValidIdentifier(name))
					continue;
				v.Name = name;
				v.HasGeneratedName = false;
			}
		}
	}

	static string? NameFromReturn(ILFunction function, ILVariable v)
	{
		if (function.Kind != ILFunctionKind.TopLevelFunction || function.Method == null)
			return null;
		bool returned = v.LoadInstructions.Any(load => load.Parent is Leave { IsLeavingFunction: true }
			|| (load.Parent is CallInstruction { Method.Name: "ToString", Arguments.Count: 1 } toString
				&& toString.Parent is Leave { IsLeavingFunction: true }));
		if (!returned)
			return null;
		string methodName = function.Method.Name;
		if (methodName.StartsWith("get_", StringComparison.Ordinal))
			methodName = methodName[4..];
		else if (function.Method.IsAccessor || methodName.Contains('<') || methodName.StartsWith('.'))
			return null;
		return MethodNameRules.NameForResultOf(methodName, v.Type.IsKnownType(KnownTypeCode.Boolean));
	}

	static string? NameFromCallResult(ILVariable v, IModule mainModule)
	{
		var names = new HashSet<string?>();
		foreach (var store in v.StoreInstructions)
		{
			if (store is not StLoc stloc)
				return null;
			var value = stloc.Value is Await await ? await.Value : stloc.Value;
			if (value is not CallInstruction call || call is NewObj
				|| call.Method.ParentModule != mainModule || call.Method.IsAccessor || call.Method.Name.Contains('<'))
				return null;
			names.Add(MethodNameRules.NameForResultOf(call.Method.Name, v.Type.IsKnownType(KnownTypeCode.Boolean)));
		}
		return names.Count == 1 ? names.Single() : null;
	}

	static string? NameFromAggregate(ILVariable v)
	{
		if (v.StoreInstructions.Count != 1 || v.StoreInstructions[0] is not StLoc stloc)
			return null;
		if (stloc.Value is not CallInstruction call || call.Arguments.Count == 0
			|| call.Method.DeclaringType.FullName != "System.Linq.Enumerable" || !Aggregates.Contains(call.Method.Name))
			return null;
		string op = call.Method.Name switch { "Sum" => "total", "LongCount" => "count", var n => n.ToLowerInvariant() };
		string? subject = call.Arguments.Count > 1 ? SelectedMember(call.Arguments[1]) : null;
		subject ??= MemberName(call.Arguments[0]);
		return subject == null ? null : op + Capitalize(subject.TrimStart('_'));
	}

	/// <summary>The member a selector lambda returns, as in <c>a =&gt; a.Quantite</c>.</summary>
	static string? SelectedMember(ILInstruction selector)
	{
		var lambda = selector.Descendants.OfType<ILFunction>().FirstOrDefault();
		if (lambda?.Body is not BlockContainer body)
			return null;
		var leaves = body.Descendants.OfType<Leave>().Where(l => l.IsLeavingFunction).ToList();
		if (leaves.Count != 1)
			return null;
		var value = leaves[0].Value;
		bool onParameter = value is CallInstruction { Arguments.Count: 1 } c && c.Arguments[0].MatchLdLoc(out var p) && p.Kind == VariableKind.Parameter
			|| value is LdObj { Target: LdFlda { Target: var t } } && t.MatchLdLoc(out var q) && q.Kind == VariableKind.Parameter;
		return onParameter ? MemberName(value) : null;
	}

	static string? MemberName(ILInstruction inst) => inst switch {
		CallInstruction { Method: { Name: var n } } when n.StartsWith("get_", StringComparison.Ordinal) => n[4..],
		LdObj { Target: LdFlda ldflda } => ldflda.Field.Name,
		LdObj { Target: LdsFlda ldsflda } => ldsflda.Field.Name,
		LdLoc { Variable: { HasGeneratedName: false } local } => local.Name,
		_ => null,
	};

	static string Capitalize(string s) => s.Length == 0 ? s : char.ToUpperInvariant(s[0]) + s[1..];

	static bool IsValidIdentifier(string name)
		=> name.Length > 0 && (char.IsLetter(name[0]) || name[0] == '_') && name.All(c => char.IsLetterOrDigit(c) || c == '_')
			&& !CSharpKeywords.Contains(name);
}
