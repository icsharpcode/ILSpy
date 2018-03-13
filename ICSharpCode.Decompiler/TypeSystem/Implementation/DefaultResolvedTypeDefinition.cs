// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation of <see cref="ITypeDefinition"/>.
	/// </summary>
	public class DefaultResolvedTypeDefinition : ITypeDefinition
	{
		readonly ITypeResolveContext parentContext;
		readonly IUnresolvedTypeDefinition[] parts;

		public DefaultResolvedTypeDefinition(ITypeResolveContext parentContext, params IUnresolvedTypeDefinition[] parts)
		{
			if (parentContext == null || parentContext.CurrentAssembly == null)
				throw new ArgumentException("Parent context does not specify any assembly", "parentContext");
			if (parts == null || parts.Length == 0)
				throw new ArgumentException("No parts were specified", "parts");
			this.parentContext = parentContext;
			this.parts = parts;
			
			foreach (var part in parts) {
				IsAbstract  |= part.IsAbstract;
				IsSealed    |= part.IsSealed;
				IsShadowing |= part.IsShadowing;
				IsSynthetic &= part.IsSynthetic; // true if all parts are synthetic
				
				// internal is the default, so use another part's accessibility until we find a non-internal accessibility
				if (Accessibility == Accessibility.Internal)
					Accessibility = part.Accessibility;
			}
		}

		IReadOnlyList<ITypeParameter> typeParameters;
		
		public IReadOnlyList<ITypeParameter> TypeParameters {
			get {
				var result = LazyInit.VolatileRead(ref this.typeParameters);
				if (result != null) {
					return result;
				}
				var contextForTypeParameters = parts[0].CreateResolveContext(parentContext);
				contextForTypeParameters = contextForTypeParameters.WithCurrentTypeDefinition(this);
				if (parentContext.CurrentTypeDefinition == null || parentContext.CurrentTypeDefinition.TypeParameterCount == 0) {
					result = parts[0].TypeParameters.CreateResolvedTypeParameters(contextForTypeParameters);
				} else {
					// This is a nested class inside a generic class; copy type parameters from outer class if we can:
					var outerClass = parentContext.CurrentTypeDefinition;
					var typeParameters = new ITypeParameter[parts[0].TypeParameters.Count];
					for (var i = 0; i < typeParameters.Length; i++) {
						var unresolvedTP = parts[0].TypeParameters[i];
						if (i < outerClass.TypeParameterCount && outerClass.TypeParameters[i].Name == unresolvedTP.Name)
							typeParameters[i] = outerClass.TypeParameters[i];
						else
							typeParameters[i] = unresolvedTP.CreateResolvedTypeParameter(contextForTypeParameters);
					}
					result = typeParameters;
				}
				return LazyInit.GetOrSet(ref this.typeParameters, result);
			}
		}

		IReadOnlyList<IAttribute> attributes;
		
		public IReadOnlyList<IAttribute> Attributes {
			get {
				var result = LazyInit.VolatileRead(ref this.attributes);
				if (result != null) {
					return result;
				}
				var newResult = new List<IAttribute>();
				var context = parentContext.WithCurrentTypeDefinition(this);
				foreach (var part in parts) {
					var parentContextForPart = part.CreateResolveContext(context);
					foreach (var attr in part.Attributes) {
						newResult.Add(attr.CreateResolvedAttribute(parentContextForPart));
					}
				}
				if (newResult.Count == 0)
					result = EmptyList<IAttribute>.Instance;
				else
					result = newResult;
				return LazyInit.GetOrSet(ref this.attributes, result);
			}
		}
		
		public IReadOnlyList<IUnresolvedTypeDefinition> Parts => parts;

		public SymbolKind SymbolKind => parts[0].SymbolKind;

		public virtual TypeKind Kind => parts[0].Kind;

		#region NestedTypes
		IReadOnlyList<ITypeDefinition> nestedTypes;
		
		public IReadOnlyList<ITypeDefinition> NestedTypes {
			get {
				var result = LazyInit.VolatileRead(ref this.nestedTypes);
				if (result != null) {
					return result;
				} else {
					result = (
						from part in parts
						from nestedTypeRef in part.NestedTypes
						group nestedTypeRef by new { nestedTypeRef.Name, nestedTypeRef.TypeParameters.Count } into g
						select new DefaultResolvedTypeDefinition(new SimpleTypeResolveContext(this), g.ToArray())
					).ToList<ITypeDefinition>();
					return LazyInit.GetOrSet(ref this.nestedTypes, result);
				}
			}
		}
		#endregion
		
		#region Members
		sealed class MemberList : IReadOnlyList<IMember>
		{
			internal readonly ITypeResolveContext[] contextPerMember;
			internal readonly IUnresolvedMember[] unresolvedMembers;
			internal readonly IMember[] resolvedMembers;
			internal readonly int NonPartialMemberCount;
			
			public MemberList(List<ITypeResolveContext> contextPerMember, List<IUnresolvedMember> unresolvedNonPartialMembers, List<PartialMethodInfo> partialMethodInfos)
			{
				this.NonPartialMemberCount = unresolvedNonPartialMembers.Count;
				this.contextPerMember = contextPerMember.ToArray();
				this.unresolvedMembers = unresolvedNonPartialMembers.ToArray();
				if (partialMethodInfos == null) {
					this.resolvedMembers = new IMember[unresolvedNonPartialMembers.Count];
				} else {
					this.resolvedMembers = new IMember[unresolvedNonPartialMembers.Count + partialMethodInfos.Count];
					for (var i = 0; i < partialMethodInfos.Count; i++) {
						var info = partialMethodInfos[i];
						var memberIndex = NonPartialMemberCount + i;
						resolvedMembers[memberIndex] = DefaultResolvedMethod.CreateFromMultipleParts(
							info.Parts.ToArray(), info.Contexts.ToArray (), false);
					}
				}
			}
			
			public IMember this[int index] {
				get {
					var output = LazyInit.VolatileRead(ref resolvedMembers[index]);
					if (output != null) {
						return output;
					}
					return LazyInit.GetOrSet(ref resolvedMembers[index], unresolvedMembers[index].CreateResolved(contextPerMember[index]));
				}
			}
			
			public int Count => resolvedMembers.Length;

			public int IndexOf(IMember item)
			{
				for (var i = 0; i < this.Count; i++) {
					if (this[i].Equals(item))
						return i;
				}
				return -1;
			}
			
			public IEnumerator<IMember> GetEnumerator()
			{
				for (var i = 0; i < this.Count; i++) {
					yield return this[i];
				}
			}
			
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
		
		sealed class PartialMethodInfo
		{
			public readonly string Name;
			public readonly int TypeParameterCount;
			public readonly IReadOnlyList<IParameter> Parameters;
			public readonly List<IUnresolvedMethod> Parts = new List<IUnresolvedMethod>();
			public readonly List<ITypeResolveContext> Contexts = new List<ITypeResolveContext>();

			public PartialMethodInfo(IUnresolvedMethod method, ITypeResolveContext context)
			{
				this.Name = method.Name;
				this.TypeParameterCount = method.TypeParameters.Count;
				this.Parameters = method.Parameters.CreateResolvedParameters(context);
				this.Parts.Add(method);
				this.Contexts.Add (context);
			}
			
			public void AddPart(IUnresolvedMethod method, ITypeResolveContext context)
			{
				if (method.HasBody) {
					// make the implementation the primary part
					this.Parts.Insert(0, method);
					this.Contexts.Insert (0, context);
				} else {
					this.Parts.Add(method);
					this.Contexts.Add (context);
				}
			}
			
			public bool IsSameSignature(PartialMethodInfo other, StringComparer nameComparer)
			{
				return nameComparer.Equals(this.Name, other.Name)
					&& this.TypeParameterCount == other.TypeParameterCount
					&& ParameterListComparer.Instance.Equals(this.Parameters, other.Parameters);
			}
		}
		
		MemberList memberList;
		
		MemberList GetMemberList()
		{
			var result = LazyInit.VolatileRead(ref this.memberList);
			if (result != null) {
				return result;
			}
			var unresolvedMembers = new List<IUnresolvedMember>();
			var contextPerMember = new List<ITypeResolveContext>();
			List<PartialMethodInfo> partialMethodInfos = null;
			var addDefaultConstructorIfRequired = false;
			foreach (var part in parts) {
				var parentContextForPart = part.CreateResolveContext(parentContext);
				var contextForPart = parentContextForPart.WithCurrentTypeDefinition(this);
				foreach (var member in part.Members) {
					var method = member as IUnresolvedMethod;
					if (method != null && method.IsPartial) {
						// Merge partial method declaration and implementation
						if (partialMethodInfos == null)
							partialMethodInfos = new List<PartialMethodInfo>();
						var newInfo = new PartialMethodInfo(method, contextForPart);
						PartialMethodInfo existingInfo = null;
						foreach (var info in partialMethodInfos) {
							if (newInfo.IsSameSignature(info, Compilation.NameComparer)) {
								existingInfo = info;
								break;
							}
						}
						if (existingInfo != null) {
							// Add the unresolved method to the PartialMethodInfo:
							existingInfo.AddPart(method, contextForPart);
						} else {
							partialMethodInfos.Add(newInfo);
						}
					} else {
						unresolvedMembers.Add(member);
						contextPerMember.Add(contextForPart);
					}
				}
				
				addDefaultConstructorIfRequired |= part.AddDefaultConstructorIfRequired;
			}
			if (addDefaultConstructorIfRequired) {
				var kind = this.Kind;
				if (kind == TypeKind.Class && !this.IsStatic && !unresolvedMembers.Any(m => m.SymbolKind == SymbolKind.Constructor && !m.IsStatic)
				    || kind == TypeKind.Enum || kind == TypeKind.Struct)
				{
					contextPerMember.Add(parts[0].CreateResolveContext(parentContext).WithCurrentTypeDefinition(this));
					unresolvedMembers.Add(DefaultUnresolvedMethod.CreateDefaultConstructor(parts[0]));
				}
			}
			result = new MemberList(contextPerMember, unresolvedMembers, partialMethodInfos);
			return LazyInit.GetOrSet(ref this.memberList, result);
		}
		
		public IReadOnlyList<IMember> Members => GetMemberList();

		public IEnumerable<IField> Fields {
			get {
				var members = GetMemberList();
				for (var i = 0; i < members.unresolvedMembers.Length; i++) {
					if (members.unresolvedMembers[i].SymbolKind == SymbolKind.Field)
						yield return (IField)members[i];
				}
			}
		}
		
		public IEnumerable<IMethod> Methods {
			get {
				var members = GetMemberList();
				for (var i = 0; i < members.unresolvedMembers.Length; i++) {
					if (members.unresolvedMembers[i] is IUnresolvedMethod)
						yield return (IMethod)members[i];
				}
				for (var i = members.unresolvedMembers.Length; i < members.Count; i++) {
					yield return (IMethod)members[i];
				}
			}
		}
		
		public IEnumerable<IProperty> Properties {
			get {
				var members = GetMemberList();
				for (var i = 0; i < members.unresolvedMembers.Length; i++) {
					switch (members.unresolvedMembers[i].SymbolKind) {
						case SymbolKind.Property:
						case SymbolKind.Indexer:
							yield return (IProperty)members[i];
							break;
					}
				}
			}
		}
		
		public IEnumerable<IEvent> Events {
			get {
				var members = GetMemberList();
				for (var i = 0; i < members.unresolvedMembers.Length; i++) {
					if (members.unresolvedMembers[i].SymbolKind == SymbolKind.Event)
						yield return (IEvent)members[i];
				}
			}
		}
		#endregion
		
		volatile KnownTypeCode knownTypeCode = (KnownTypeCode)(-1);
		
		public KnownTypeCode KnownTypeCode {
			get {
				var result = this.knownTypeCode;
				if (result == (KnownTypeCode)(-1)) {
					result = KnownTypeCode.None;
					var compilation = this.Compilation;
					for (var i = 0; i < KnownTypeReference.KnownTypeCodeCount; i++) {
						if (compilation.FindType((KnownTypeCode)i) == this) {
							result = (KnownTypeCode)i;
							break;
						}
					}
					this.knownTypeCode = result;
				}
				return result;
			}
		}
		
		volatile IType enumUnderlyingType;
		
		public IType EnumUnderlyingType {
			get {
				var result = this.enumUnderlyingType;
				if (result == null) {
					if (this.Kind == TypeKind.Enum) {
						result = CalculateEnumUnderlyingType();
					} else {
						result = SpecialType.UnknownType;
					}
					this.enumUnderlyingType = result;
				}
				return result;
			}
		}
		
		IType CalculateEnumUnderlyingType()
		{
			foreach (var part in parts) {
				var context = part.CreateResolveContext(parentContext).WithCurrentTypeDefinition(this);
				foreach (var baseTypeRef in part.BaseTypes) {
					var type = baseTypeRef.Resolve(context);
					if (type.Kind != TypeKind.Unknown)
						return type;
				}
			}
			return this.Compilation.FindType(KnownTypeCode.Int32);
		}
		
		volatile byte hasExtensionMethods; // 0 = unknown, 1 = true, 2 = false
		
		public bool HasExtensionMethods {
			get {
				var val = this.hasExtensionMethods;
				if (val == 0) {
					if (CalculateHasExtensionMethods())
						val = 1;
					else
						val = 2;
					this.hasExtensionMethods = val;
				}
				return val == 1;
			}
		}
		
		bool CalculateHasExtensionMethods()
		{
			var noExtensionMethods = true;
			foreach (var part in parts) {
				// Return true if any part has extension methods
				if (part.HasExtensionMethods == true)
					return true;
				if (part.HasExtensionMethods == null)
					noExtensionMethods = false;
			}
			// Return false if all parts are known to have no extension methods
			if (noExtensionMethods)
				return false;
			// If unsure, look at the resolved methods.
			return Methods.Any(m => m.IsExtensionMethod);
		}
		
		public bool IsPartial => parts.Length > 1 || parts[0].IsPartial;

		public bool? IsReferenceType {
			get {
				switch (this.Kind) {
					case TypeKind.Class:
					case TypeKind.Interface:
					case TypeKind.Module:
					case TypeKind.Delegate:
						return true;
					case TypeKind.Struct:
					case TypeKind.Enum:
					case TypeKind.Void:
						return false;
					default:
						throw new InvalidOperationException("Invalid value for TypeKind");
				}
			}
		}
		
		public int TypeParameterCount => parts[0].TypeParameters.Count;

		public IReadOnlyList<IType> TypeArguments => TypeParameters;

		#region DirectBaseTypes
		IList<IType> directBaseTypes;
		
		public IEnumerable<IType> DirectBaseTypes {
			get {
				var result = LazyInit.VolatileRead(ref this.directBaseTypes);
				if (result != null) {
					return result;
				}
				using (var busyLock = BusyManager.Enter(this)) {
					if (busyLock.Success) {
						result = CalculateDirectBaseTypes();
						return LazyInit.GetOrSet(ref this.directBaseTypes, result);
					} else {
						// This can happen for "class Test : $Test.Base$ { public class Base {} }"
						// and also for the valid code
						// "class Test : Base<Test.Inner> { public class Inner {} }"
						
						// Don't cache the error!
						return EmptyList<IType>.Instance;
					}
				}
			}
		}
		
		IList<IType> CalculateDirectBaseTypes()
		{
			var result = new List<IType>();
			var hasNonInterface = false;
			if (this.Kind != TypeKind.Enum) {
				foreach (var part in parts) {
					var context = part.CreateResolveContext(parentContext).WithCurrentTypeDefinition(this);
					foreach (var baseTypeRef in part.BaseTypes) {
						var baseType = baseTypeRef.Resolve(context);
						if (!(baseType.Kind == TypeKind.Unknown || result.Contains(baseType))) {
							result.Add(baseType);
							if (baseType.Kind != TypeKind.Interface)
								hasNonInterface = true;
						}
					}
				}
			}
			if (!hasNonInterface && !(this.Name == "Object" && this.Namespace == "System" && this.TypeParameterCount == 0)) {
				KnownTypeCode primitiveBaseType;
				switch (this.Kind) {
					case TypeKind.Enum:
						primitiveBaseType = KnownTypeCode.Enum;
						break;
					case TypeKind.Struct:
					case TypeKind.Void:
						primitiveBaseType = KnownTypeCode.ValueType;
						break;
					case TypeKind.Delegate:
						primitiveBaseType = KnownTypeCode.Delegate;
						break;
					default:
						primitiveBaseType = KnownTypeCode.Object;
						break;
				}
				var t = parentContext.Compilation.FindType(primitiveBaseType);
				if (t.Kind != TypeKind.Unknown)
					result.Add(t);
			}
			return result;
		}
		#endregion
		
		public string FullName => parts[0].FullName;

		public string Name => parts[0].Name;

		public string ReflectionName => parts[0].ReflectionName;

		public string Namespace => parts[0].Namespace;

		public FullTypeName FullTypeName => parts[0].FullTypeName;

		public ITypeDefinition DeclaringTypeDefinition => parentContext.CurrentTypeDefinition;

		public IType DeclaringType => parentContext.CurrentTypeDefinition;

		public IAssembly ParentAssembly => parentContext.CurrentAssembly;

		public ICompilation Compilation => parentContext.Compilation;

		#region Modifiers
		public bool IsStatic => IsAbstract && IsSealed;
		public bool IsAbstract { get; }
		public bool IsSealed { get; }
		public bool IsShadowing { get; }
		public bool IsSynthetic { get; } = true;

		public Accessibility Accessibility { get; } = Accessibility.Internal;

		bool IHasAccessibility.IsPrivate => Accessibility == Accessibility.Private;

		bool IHasAccessibility.IsPublic => Accessibility == Accessibility.Public;

		bool IHasAccessibility.IsProtected => Accessibility == Accessibility.Protected;

		bool IHasAccessibility.IsInternal => Accessibility == Accessibility.Internal;

		bool IHasAccessibility.IsProtectedOrInternal => Accessibility == Accessibility.ProtectedOrInternal;

		bool IHasAccessibility.IsProtectedAndInternal => Accessibility == Accessibility.ProtectedAndInternal;

		#endregion
		
		ITypeDefinition IType.GetDefinition()
		{
			return this;
		}
		
		IType IType.AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitTypeDefinition(this);
		}
		
		IType IType.VisitChildren(TypeVisitor visitor)
		{
			return this;
		}
		
		public ITypeReference ToTypeReference()
		{
			var declTypeDef = this.DeclaringTypeDefinition;
			if (declTypeDef != null) {
				return new NestedTypeReference(declTypeDef.ToTypeReference(),
					this.Name, this.TypeParameterCount - declTypeDef.TypeParameterCount,
					this.IsReferenceType);
			} else {
				var asm = this.ParentAssembly;
				IAssemblyReference asmRef;
				if (asm != null)
					asmRef = new DefaultAssemblyReference(asm.AssemblyName);
				else
					asmRef = null;
				return new GetClassTypeReference(asmRef, this.Namespace, this.Name, this.TypeParameterCount, this.IsReferenceType);
			}
		}
		
		ISymbolReference ISymbol.ToReference()
		{
			return (ISymbolReference)ToTypeReference();
		}
		
		public IEnumerable<IType> GetNestedTypes(Predicate<ITypeDefinition> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			const GetMemberOptions opt = GetMemberOptions.IgnoreInheritedMembers | GetMemberOptions.ReturnMemberDefinitions;
			if ((options & opt) == opt) {
				if (filter == null)
					return this.NestedTypes;
				else
					return GetNestedTypesImpl(filter);
			} else {
				return GetMembersHelper.GetNestedTypes(this, filter, options);
			}
		}
		
		IEnumerable<IType> GetNestedTypesImpl(Predicate<ITypeDefinition> filter)
		{
			foreach (var nestedType in this.NestedTypes) {
				if (filter(nestedType))
					yield return nestedType;
			}
		}
		
		public IEnumerable<IType> GetNestedTypes(IReadOnlyList<IType> typeArguments, Predicate<ITypeDefinition> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return GetMembersHelper.GetNestedTypes(this, typeArguments, filter, options);
		}
		
		#region GetMembers()
		IEnumerable<IMember> GetFilteredMembers(Predicate<IUnresolvedMember> filter)
		{
			var members = GetMemberList();
			for (var i = 0; i < members.unresolvedMembers.Length; i++) {
				if (filter == null || filter(members.unresolvedMembers[i])) {
					yield return members[i];
				}
			}
			for (var i = members.unresolvedMembers.Length; i < members.Count; i++) {
				var method = (IMethod)members[i];
				var ok = false;
				foreach (var part in method.Parts) {
					if (filter == null || filter(part)) {
						ok = true;
						break;
					}
				}
				if (ok)
					yield return method;
			}
		}
		
		IEnumerable<IMethod> GetFilteredMethods(Predicate<IUnresolvedMethod> filter)
		{
			var members = GetMemberList();
			for (var i = 0; i < members.unresolvedMembers.Length; i++) {
				var unresolved = members.unresolvedMembers[i] as IUnresolvedMethod;
				if (unresolved != null && (filter == null || filter(unresolved))) {
					yield return (IMethod)members[i];
				}
			}
			for (var i = members.unresolvedMembers.Length; i < members.Count; i++) {
				var method = (IMethod)members[i];
				var ok = false;
				foreach (var part in method.Parts) {
					if (filter == null || filter(part)) {
						ok = true;
						break;
					}
				}
				if (ok)
					yield return method;
			}
		}
		
		IEnumerable<TResolved> GetFilteredNonMethods<TUnresolved, TResolved>(Predicate<TUnresolved> filter) where TUnresolved : class, IUnresolvedMember where TResolved : class, IMember
		{
			var members = GetMemberList();
			for (var i = 0; i < members.unresolvedMembers.Length; i++) {
				var unresolved = members.unresolvedMembers[i] as TUnresolved;
				if (unresolved != null && (filter == null || filter(unresolved))) {
					yield return (TResolved)members[i];
				}
			}
		}
		
		public virtual IEnumerable<IMethod> GetMethods(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				return GetFilteredMethods(ExtensionMethods.And(m => !m.IsConstructor, filter));
			} else {
				return GetMembersHelper.GetMethods(this, filter, options);
			}
		}
		
		public virtual IEnumerable<IMethod> GetMethods(IReadOnlyList<IType> typeArguments, Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return GetMembersHelper.GetMethods(this, typeArguments, filter, options);
		}
		
		public virtual IEnumerable<IMethod> GetConstructors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.IgnoreInheritedMembers)
		{
			if (ComHelper.IsComImport(this)) {
				var coClass = ComHelper.GetCoClass(this);
				using (var busyLock = BusyManager.Enter(this)) {
					if (busyLock.Success) {
						return coClass.GetConstructors(filter, options)
							.Select(m => new SpecializedMethod(m, m.Substitution) { DeclaringType = this });
					}
				}
				return EmptyList<IMethod>.Instance;
			}
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				return GetFilteredMethods(ExtensionMethods.And(m => m.IsConstructor && !m.IsStatic, filter));
			} else {
				return GetMembersHelper.GetConstructors(this, filter, options);
			}
		}
		
		public virtual IEnumerable<IProperty> GetProperties(Predicate<IUnresolvedProperty> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				return GetFilteredNonMethods<IUnresolvedProperty, IProperty>(filter);
			} else {
				return GetMembersHelper.GetProperties(this, filter, options);
			}
		}
		
		public virtual IEnumerable<IField> GetFields(Predicate<IUnresolvedField> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				return GetFilteredNonMethods<IUnresolvedField, IField>(filter);
			} else {
				return GetMembersHelper.GetFields(this, filter, options);
			}
		}
		
		public virtual IEnumerable<IEvent> GetEvents(Predicate<IUnresolvedEvent> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				return GetFilteredNonMethods<IUnresolvedEvent, IEvent>(filter);
			} else {
				return GetMembersHelper.GetEvents(this, filter, options);
			}
		}
		
		public virtual IEnumerable<IMember> GetMembers(Predicate<IUnresolvedMember> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				return GetFilteredMembers(filter);
			} else {
				return GetMembersHelper.GetMembers(this, filter, options);
			}
		}
		
		public virtual IEnumerable<IMethod> GetAccessors(Predicate<IUnresolvedMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				return GetFilteredAccessors(filter);
			} else {
				return GetMembersHelper.GetAccessors(this, filter, options);
			}
		}
		
		IEnumerable<IMethod> GetFilteredAccessors(Predicate<IUnresolvedMethod> filter)
		{
			var members = GetMemberList();
			for (var i = 0; i < members.unresolvedMembers.Length; i++) {
				var unresolved = members.unresolvedMembers[i];
				var unresolvedProperty = unresolved as IUnresolvedProperty;
				var unresolvedEvent = unresolved as IUnresolvedEvent;
				if (unresolvedProperty != null) {
					if (unresolvedProperty.CanGet && (filter == null || filter(unresolvedProperty.Getter)))
						yield return ((IProperty)members[i]).Getter;
					if (unresolvedProperty.CanSet && (filter == null || filter(unresolvedProperty.Setter)))
						yield return ((IProperty)members[i]).Setter;
				} else if (unresolvedEvent != null) {
					if (unresolvedEvent.CanAdd && (filter == null || filter(unresolvedEvent.AddAccessor)))
						yield return ((IEvent)members[i]).AddAccessor;
					if (unresolvedEvent.CanRemove && (filter == null || filter(unresolvedEvent.RemoveAccessor)))
						yield return ((IEvent)members[i]).RemoveAccessor;
					if (unresolvedEvent.CanInvoke && (filter == null || filter(unresolvedEvent.InvokeAccessor)))
						yield return ((IEvent)members[i]).InvokeAccessor;
				}
			}
		}
		#endregion
		
		#region GetInterfaceImplementation
		public IMember GetInterfaceImplementation(IMember interfaceMember)
		{
			return GetInterfaceImplementation(new[] { interfaceMember })[0];
		}
		
		public IReadOnlyList<IMember> GetInterfaceImplementation(IReadOnlyList<IMember> interfaceMembers)
		{
			// TODO: review the subtle rules for interface reimplementation,
			// write tests and fix this method.
			// Also virtual/override is going to be tricky -
			// I think we'll need to consider the 'virtual' method first for
			// reimplemenatation purposes, but then actually return the 'override'
			// (as that's the method that ends up getting called)
			
			interfaceMembers = interfaceMembers.ToList(); // avoid evaluating more than once
			
			var result = new IMember[interfaceMembers.Count];
			var signatureToIndexDict = new MultiDictionary<IMember, int>(SignatureComparer.Ordinal);
			for (var i = 0; i < interfaceMembers.Count; i++) {
				signatureToIndexDict.Add(interfaceMembers[i], i);
			}
			foreach (var member in GetMembers(m => !m.IsExplicitInterfaceImplementation)) {
				foreach (var interfaceMemberIndex in signatureToIndexDict[member]) {
					result[interfaceMemberIndex] = member;
				}
			}
			foreach (var explicitImpl in GetMembers(m => m.IsExplicitInterfaceImplementation)) {
				foreach (var interfaceMember in explicitImpl.ImplementedInterfaceMembers) {
					foreach (var potentialMatchingIndex in signatureToIndexDict[interfaceMember]) {
						if (interfaceMember.Equals(interfaceMembers[potentialMatchingIndex])) {
							result[potentialMatchingIndex] = explicitImpl;
						}
					}
				}
			}
			return result;
		}
		#endregion
		
		public TypeParameterSubstitution GetSubstitution()
		{
			return TypeParameterSubstitution.Identity;
		}
		
		public TypeParameterSubstitution GetSubstitution(IReadOnlyList<IType> methodTypeArguments)
		{
			return TypeParameterSubstitution.Identity;
		}

		public bool Equals(IType other)
		{
			return this == other;
		}
		
		public override string ToString()
		{
			return this.ReflectionName;
		}
	}
}
