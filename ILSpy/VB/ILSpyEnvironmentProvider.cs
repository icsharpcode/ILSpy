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
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.VB.Visitors;
using Mono.Cecil;

namespace ICSharpCode.ILSpy.VB
{
	public class ILSpyEnvironmentProvider : IEnvironmentProvider
	{
		public string RootNamespace {
			get {
				return "";
			}
		}

		//readonly CecilLoader loader = new CecilLoader();
		
		public string GetTypeNameForAttribute(NRefactory.CSharp.Attribute attribute)
		{
			return attribute.Type.Annotations
				.OfType<Mono.Cecil.MemberReference>()
				.First()
				.FullName;
		}
		
		public IType ResolveType(NRefactory.VB.Ast.AstType type, NRefactory.VB.Ast.TypeDeclaration entity = null)
		{
			/*
			var annotation = type.Annotation<TypeReference>();
			if (annotation == null )
				return null;
			
			IEntity current = null;
			if (entity != null) {
				var typeInfo = entity.Annotation<TypeReference>();
				current = loader.ReadTypeReference(typeInfo).Resolve(context).GetDefinition();
			}
			
			return loader.ReadTypeReference(annotation, entity: current).Resolve(context);*/
			return SpecialType.UnknownType;
		}
		
		public TypeKind GetTypeKindForAstType(NRefactory.CSharp.AstType type)
		{
			var annotation = type.Annotation<TypeReference>();
			if (annotation == null)
				return TypeKind.Unknown;
			
			var definition = annotation.ResolveOrThrow();
			if (definition.IsClass)
				return TypeKind.Class;
			if (definition.IsInterface)
				return TypeKind.Interface;
			if (definition.IsEnum)
				return TypeKind.Enum;
			if (definition.IsFunctionPointer)
				return TypeKind.Delegate;
			if (definition.IsValueType)
				return TypeKind.Struct;
			
			return TypeKind.Unknown;
		}
		
		public TypeCode ResolveExpression(NRefactory.CSharp.Expression expression)
		{
			var annotation = expression.Annotations.OfType<TypeInformation>().FirstOrDefault();
			
			if (annotation == null || annotation.InferredType == null)
				return TypeCode.Object;
			
			var definition = annotation.InferredType.Resolve();
			
			if (definition == null)
				return TypeCode.Object;
			
			switch (definition.FullName) {
				case "System.String":
					return TypeCode.String;
				default:
					break;
			}
			
			return TypeCode.Object;
		}
		
		public bool? IsReferenceType(NRefactory.CSharp.Expression expression)
		{
			if (expression is NRefactory.CSharp.NullReferenceExpression)
				return true;
			
			var annotation = expression.Annotations.OfType<TypeInformation>().FirstOrDefault();
			
			if (annotation == null || annotation.InferredType == null)
				return null;
			
			var definition = annotation.InferredType.Resolve();
			
			if (definition == null)
				return null;
			
			return !definition.IsValueType;
		}
		
		public IEnumerable<NRefactory.VB.Ast.InterfaceMemberSpecifier> CreateMemberSpecifiersForInterfaces(IEnumerable<NRefactory.VB.Ast.AstType> interfaces)
		{
			foreach (var type in interfaces) {
				var def = type.Annotation<TypeReference>().Resolve();
				if (def == null) continue;
				foreach (var method in def.Methods.Where(m => !m.Name.StartsWith("get_", StringComparison.Ordinal) && !m.Name.StartsWith("set_", StringComparison.Ordinal))) {
					yield return new NRefactory.VB.Ast.InterfaceMemberSpecifier((NRefactory.VB.Ast.AstType)type.Clone(), method.Name);
				}
				
				foreach (var property in def.Properties) {
					yield return new NRefactory.VB.Ast.InterfaceMemberSpecifier((NRefactory.VB.Ast.AstType)type.Clone(), property.Name);
				}
			}
		}
		
		public bool HasEvent(NRefactory.VB.Ast.Expression expression)
		{
			return expression.Annotation<EventDefinition>() != null;
		}
		
		public bool IsMethodGroup(NRefactory.CSharp.Expression expression)
		{
			var methodInfo = expression.Annotation<MethodReference>()?.Resolve();
			if (methodInfo != null) {
				return !methodInfo.IsGetter && !methodInfo.IsSetter && !methodInfo.IsAddOn && !methodInfo.IsRemoveOn;
			}
			
			return false;
		}

		public NRefactory.CSharp.ParameterDeclaration[] GetParametersForProperty(NRefactory.CSharp.PropertyDeclaration property)
		{
			var propInfo = property.Annotation<PropertyReference>();

			if (propInfo == null)
				return new NRefactory.CSharp.ParameterDeclaration[0];

			return propInfo.Parameters.Select(p => new NRefactory.CSharp.ParameterDeclaration(AstBuilder.ConvertType(p.ParameterType), p.Name, GetModifiers(p))).ToArray();
		}

		NRefactory.CSharp.ParameterModifier GetModifiers(ParameterDefinition p)
		{
			if (p.IsOut && p.IsIn)
				return NRefactory.CSharp.ParameterModifier.Ref;
			if (p.IsOut)
				return NRefactory.CSharp.ParameterModifier.Out;

			return NRefactory.CSharp.ParameterModifier.None;
		}
	}
}
