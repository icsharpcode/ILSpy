using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Search
{
	abstract class AbstractEntitySearchStrategy : AbstractSearchStrategy
	{
		protected readonly Language language;
		protected readonly ApiVisibility apiVisibility;

		protected AbstractEntitySearchStrategy(Language language, ApiVisibility apiVisibility, IProducerConsumerCollection<SearchResult> resultQueue, params string[] terms)
			: base(resultQueue, terms)
		{
			this.language = language;
			this.apiVisibility = apiVisibility;
		}

		protected bool CheckVisibility(IEntity entity)
		{
			if (apiVisibility == ApiVisibility.All)
				return true;

			do
			{
				if (apiVisibility == ApiVisibility.PublicOnly)
				{
					if (!(entity.Accessibility == Accessibility.Public ||
						entity.Accessibility == Accessibility.Protected ||
						entity.Accessibility == Accessibility.ProtectedOrInternal))
						return false;
				}
				else if (apiVisibility == ApiVisibility.PublicAndInternal)
				{
					if (!language.ShowMember(entity))
						return false;
				}
				entity = entity.DeclaringTypeDefinition;
			}
			while (entity != null);

			return true;
		}

		protected void OnFoundResult(IEntity entity)
		{
			var result = ResultFromEntity(entity);
			OnFoundResult(result);
		}

		SearchResult ResultFromEntity(IEntity item)
		{
			var declaringType = item.DeclaringTypeDefinition;
			return new MemberSearchResult {
				Member = item,
				Fitness = CalculateFitness(item),
				Name = GetLanguageSpecificName(item),
				Location = declaringType != null ? language.TypeToString(declaringType, includeNamespace: true) : item.Namespace,
				Assembly = item.ParentModule.FullAssemblyName,
				ToolTip = item.ParentModule.PEFile?.FileName
			};
		}

		float CalculateFitness(IEntity member)
		{
			string text = member.Name;

			// Probably compiler generated types without meaningful names, show them last
			if (text.StartsWith("<"))
			{
				return 0;
			}

			// Constructors and destructors always have the same name in IL:
			// Use type name instead
			if (member.SymbolKind == SymbolKind.Constructor || member.SymbolKind == SymbolKind.Destructor)
			{
				text = member.DeclaringType.Name;
			}

			// Ignore generic arguments, it not possible to search based on them either
			text = ReflectionHelper.SplitTypeParameterCountFromReflectionName(text);

			return 1.0f / text.Length;
		}

		string GetLanguageSpecificName(IEntity member)
		{
			switch (member)
			{
				case ITypeDefinition t:
					return language.TypeToString(t, false);
				case IField f:
					return language.FieldToString(f, true, false, false);
				case IProperty p:
					return language.PropertyToString(p, true, false, false);
				case IMethod m:
					return language.MethodToString(m, true, false, false);
				case IEvent e:
					return language.EventToString(e, true, false, false);
				default:
					throw new NotSupportedException(member?.GetType() + " not supported!");
			}
		}

		static internal ImageSource GetIcon(IEntity member)
		{
			switch (member)
			{
				case ITypeDefinition t:
					return TypeTreeNode.GetIcon(t);
				case IField f:
					return FieldTreeNode.GetIcon(f);
				case IProperty p:
					return PropertyTreeNode.GetIcon(p);
				case IMethod m:
					return MethodTreeNode.GetIcon(m);
				case IEvent e:
					return EventTreeNode.GetIcon(e);
				default:
					throw new NotSupportedException(member?.GetType() + " not supported!");
			}
		}
	}
}
