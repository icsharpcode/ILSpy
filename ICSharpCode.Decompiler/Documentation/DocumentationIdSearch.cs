// Copyright (c) 2026 Siegfried Pammer
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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Text;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Documentation
{
	/// <summary>
	/// Finds the entities a hand-written documentation id was aiming at.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The id grammar is exact, and <see cref="IdStringProvider"/> implements it exactly: that is
	/// what keeps cref-following honest, because an id in a documentation file is machine-written
	/// and means one member. This is the other half - the ids people and tools type by hand at a
	/// command line, where being made to spell out a parameter list or a generic arity means
	/// knowing the answer before asking the question.
	/// </para>
	/// <para>
	/// Matching is a ladder, loosening one thing at a time and stopping at the first rung that
	/// matches anything:
	/// </para>
	/// <list type="number">
	///   <item>the exact id, as the grammar defines it;</item>
	///   <item>the id without its parameter list, naming a member group;</item>
	///   <item>the id with generic arities left off the declaring type, the member, or both.</item>
	/// </list>
	/// <para>
	/// A rung can match several entities, and every one of them is returned: which to present is
	/// the caller's decision, and hiding the others would hide that the id was ambiguous. An id
	/// that does spell out a signature never leaves the first rung, so stating a signature no
	/// member has still finds nothing rather than drifting to a same-named sibling.
	/// </para>
	/// </remarks>
	public static class DocumentationIdSearch
	{
		/// <summary>
		/// Finds every entity the given id names, in the first module that matches at all.
		/// Returns an empty group if no module does.
		/// </summary>
		public static (MetadataFile Module, ImmutableArray<EntityHandle> Handles) Find(string idString, IReadOnlyList<MetadataFile> modules)
		{
			if (idString == null)
				throw new ArgumentNullException(nameof(idString));
			if (modules == null)
				throw new ArgumentNullException(nameof(modules));

			var (exactModule, exactHandle) = TryExact(idString, modules);
			if (!exactHandle.IsNil)
				return (exactModule, ImmutableArray.Create(exactHandle));

			var queries = Parse(idString);
			if (queries.Count == 0)
				return (null, ImmutableArray<EntityHandle>.Empty);

			// Rung 2 requires the id to name the declaring type in full; rung 3 lets it name only
			// the tail of that path, so "Dictionary.Add" finds the member without the namespace.
			foreach (bool suffixMatch in new[] { false, true })
			{
				foreach (var module in modules)
				{
					if (module == null)
						continue;
					var matches = ImmutableArray.CreateBuilder<EntityHandle>();
					foreach (var query in queries)
						CollectInModule(module, query, suffixMatch, matches);
					if (matches.Count > 0)
						return (module, Distinct(matches));
				}
			}

			return (null, ImmutableArray<EntityHandle>.Empty);
		}

		static (MetadataFile, EntityHandle) TryExact(string idString, IReadOnlyList<MetadataFile> modules)
		{
			// Only a well-formed id can be exact, and IdStringProvider throws rather than
			// returning nothing for one that is not.
			if (idString.Length < 2 || idString[1] != ':')
				return (null, default);
			try
			{
				return IdStringProvider.FindEntity(idString, modules);
			}
			catch (ReflectionNameParseException)
			{
				return (null, default);
			}
		}

		static ImmutableArray<EntityHandle> Distinct(ImmutableArray<EntityHandle>.Builder matches)
		{
			var seen = new HashSet<EntityHandle>();
			var result = ImmutableArray.CreateBuilder<EntityHandle>(matches.Count);
			foreach (var handle in matches)
			{
				if (seen.Add(handle))
					result.Add(handle);
			}
			return result.ToImmutable();
		}

		/// <summary>A name with the generic arity the id stated for it, or -1 for none.</summary>
		readonly struct Part
		{
			public Part(string name, int arity)
			{
				Name = name;
				Arity = arity;
			}

			public string Name { get; }
			public int Arity { get; }
		}

		struct Query
		{
			public char Kind { get; set; }
			public Part[] TypePath { get; set; }
			public string MemberName { get; set; }
			public int MemberArity { get; set; }
		}

		/// <summary>
		/// Turns an id into the queries it could plausibly mean. A missing "X:" prefix leaves the
		/// kind open, and the final dot is then ambiguous between a nested type name and a member
		/// name - both readings are returned, and both are searched.
		/// </summary>
		static List<Query> Parse(string idString)
		{
			var queries = new List<Query>();
			string rest = idString;
			char kind = '\0';
			if (rest.Length > 2 && rest[1] == ':')
			{
				kind = rest[0];
				if (kind is not ('T' or 'M' or 'P' or 'F' or 'E'))
					return queries;
				rest = rest.Substring(2);
			}

			// An id that spells out a signature is exact or nothing, and exact has been tried.
			if (rest.IndexOf('(') >= 0 || rest.IndexOf('~') >= 0)
				return queries;

			var path = SplitPath(rest);
			if (path.Count == 0)
				return queries;

			if (kind is '\0' or 'T')
				queries.Add(new Query { Kind = 'T', TypePath = path.ToArray() });

			if (kind != 'T' && path.Count >= 2)
			{
				var member = path[path.Count - 1];
				var declaring = path.GetRange(0, path.Count - 1).ToArray();
				// '#ctor'/'#cctor' are the id spelling of the metadata names '.ctor'/'.cctor'.
				string memberName = member.Name.Replace('#', '.');
				foreach (char memberKind in kind == '\0' ? new[] { 'M', 'P', 'F', 'E' } : new[] { kind })
				{
					queries.Add(new Query {
						Kind = memberKind,
						TypePath = declaring,
						MemberName = memberName,
						MemberArity = member.Arity,
					});
				}
			}

			return queries;
		}

		/// <summary>
		/// Splits a dotted name into parts, reading the generic arity of each from whichever
		/// spelling it was given: the id form (<c>Dictionary`2</c>, <c>M``1</c>) or the cref and
		/// C# forms (<c>Dictionary{TKey,TValue}</c>, <c>Dictionary&lt;TKey,TValue&gt;</c>). The
		/// bracketed forms are what a person reaches for, and unlike a backtick they survive being
		/// typed at a shell prompt.
		/// </summary>
		static List<Part> SplitPath(string text)
		{
			var parts = new List<Part>();
			int i = 0;
			while (i <= text.Length)
			{
				int start = i;
				int arity = -1;
				var name = new StringBuilder();
				while (i < text.Length && text[i] != '.')
				{
					char c = text[i];
					if (c == '`')
					{
						int digits = i + 1;
						while (digits < text.Length && text[digits] == '`')
							digits++;
						int numberStart = digits;
						while (digits < text.Length && char.IsDigit(text[digits]))
							digits++;
						if (digits == numberStart)
							return new List<Part>();
						arity = int.Parse(text.Substring(numberStart, digits - numberStart));
						i = digits;
						continue;
					}
					if (c is '{' or '<')
					{
						int close = MatchingBracket(text, i);
						if (close < 0)
							return new List<Part>();
						arity = CountArguments(text, i + 1, close);
						i = close + 1;
						continue;
					}
					name.Append(c);
					i++;
				}
				if (name.Length == 0 && start == i)
					return new List<Part>();
				parts.Add(new Part(name.ToString(), arity));
				if (i >= text.Length)
					break;
				i++;  // the '.'
			}
			return parts;
		}

		static int MatchingBracket(string text, int open)
		{
			int depth = 0;
			for (int i = open; i < text.Length; i++)
			{
				if (text[i] is '{' or '<')
					depth++;
				else if (text[i] is '}' or '>' && --depth == 0)
					return i;
			}
			return -1;
		}

		static int CountArguments(string text, int start, int end)
		{
			if (start >= end)
				return 0;
			int depth = 0, count = 1;
			for (int i = start; i < end; i++)
			{
				if (text[i] is '{' or '<')
					depth++;
				else if (text[i] is '}' or '>')
					depth--;
				else if (text[i] == ',' && depth == 0)
					count++;
			}
			return count;
		}

		static void CollectInModule(MetadataFile module, Query query, bool suffixMatch, ImmutableArray<EntityHandle>.Builder matches)
		{
			var metadata = module.Metadata;
			foreach (var typeHandle in metadata.TypeDefinitions)
			{
				var typeDef = metadata.GetTypeDefinition(typeHandle);
				if (!TypeMatches(metadata, typeDef, query.TypePath, suffixMatch))
					continue;
				if (query.Kind == 'T')
					matches.Add(typeHandle);
				else
					CollectMembers(metadata, typeDef, query, matches);
			}
		}

		/// <summary>
		/// Compares a type's namespace-and-nesting path against the id's, part by part. A part
		/// that states an arity must match it; one that leaves it off matches any, which is what
		/// lets "Dictionary" find "Dictionary`2". With <paramref name="suffixMatch"/> the id need
		/// only name the tail of the path, so a namespace may be shortened or dropped.
		/// </summary>
		static bool TypeMatches(MetadataReader metadata, TypeDefinition typeDef, Part[] wanted, bool suffixMatch)
		{
			var actual = new List<Part>();
			var current = typeDef;
			while (true)
			{
				actual.Insert(0, SplitArity(metadata.GetString(current.Name)));
				var declaring = current.GetDeclaringType();
				if (declaring.IsNil)
					break;
				current = metadata.GetTypeDefinition(declaring);
			}
			string ns = metadata.GetString(current.Namespace);
			if (ns.Length > 0)
			{
				var namespaceParts = ns.Split('.');
				for (int i = namespaceParts.Length - 1; i >= 0; i--)
					actual.Insert(0, new Part(namespaceParts[i], -1));
			}

			if (suffixMatch ? actual.Count < wanted.Length : actual.Count != wanted.Length)
				return false;
			int offset = actual.Count - wanted.Length;
			for (int i = 0; i < wanted.Length; i++)
			{
				var a = actual[offset + i];
				if (a.Name != wanted[i].Name)
					return false;
				if (wanted[i].Arity >= 0 && a.Arity != wanted[i].Arity)
					return false;
			}
			return true;
		}

		static Part SplitArity(string metadataName)
		{
			int tick = metadataName.IndexOf('`');
			if (tick < 0)
				return new Part(metadataName, 0);
			return int.TryParse(metadataName.Substring(tick + 1), out int arity)
				? new Part(metadataName.Substring(0, tick), arity)
				: new Part(metadataName, 0);
		}

		static void CollectMembers(MetadataReader metadata, TypeDefinition typeDef, Query query, ImmutableArray<EntityHandle>.Builder matches)
		{
			bool NameMatches(StringHandle candidate) => metadata.StringComparer.Equals(candidate, query.MemberName);

			switch (query.Kind)
			{
				case 'F':
					foreach (var handle in typeDef.GetFields())
					{
						if (NameMatches(metadata.GetFieldDefinition(handle).Name))
							matches.Add(handle);
					}
					break;
				case 'M':
					foreach (var handle in typeDef.GetMethods())
					{
						var method = metadata.GetMethodDefinition(handle);
						if (!NameMatches(method.Name))
							continue;
						if (query.MemberArity >= 0 && method.GetGenericParameters().Count != query.MemberArity)
							continue;
						matches.Add(handle);
					}
					break;
				case 'P':
					foreach (var handle in typeDef.GetProperties())
					{
						if (NameMatches(metadata.GetPropertyDefinition(handle).Name))
							matches.Add(handle);
					}
					break;
				case 'E':
					foreach (var handle in typeDef.GetEvents())
					{
						if (NameMatches(metadata.GetEventDefinition(handle).Name))
							matches.Add(handle);
					}
					break;
			}
		}
	}
}
