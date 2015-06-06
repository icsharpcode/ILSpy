// Copyright (c) 2014 Daniel Grunwald
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
using Mono.Cecil;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;

namespace ICSharpCode.Decompiler.CSharp
{
	public class CSharpDecompiler
	{
		readonly DecompilerTypeSystem typeSystem;

		List<IILTransform> ilTransforms = new List<IILTransform> {
			new OptimizingTransform(),
			new LoopDetection(),
			new ControlFlowSimplification(),
			new ILInlining(),
			new TransformingVisitor(),
			new TransformValueTypeInitializers(),
			new TransformArrayInitializers()
		};

		List<IAstTransform> astTransforms = new List<IAstTransform> {
			//new PushNegation(),
			//new DelegateConstruction(context),
			//new PatternStatementTransform(context),
			new ReplaceMethodCallsWithOperators(),
			new IntroduceUnsafeModifier(),
			new AddCheckedBlocks(),
			//new DeclareVariables(context), // should run after most transforms that modify statements
			new ConvertConstructorCallIntoInitializer(), // must run after DeclareVariables
			//new DecimalConstantTransform(),
			new IntroduceUsingDeclarations(),
			//new IntroduceExtensionMethods(context), // must run after IntroduceUsingDeclarations
			//new IntroduceQueryExpressions(context), // must run after IntroduceExtensionMethods
			//new CombineQueryExpressions(context),
			//new FlattenSwitchBlocks(),
		};

		public CancellationToken CancellationToken { get; set; }

		/// <summary>
		/// IL transforms.
		/// </summary>
		public IList<IILTransform> ILTransforms {
			get { return ilTransforms; }
		}

		/// <summary>
		/// C# AST transforms.
		/// </summary>
		public IList<IAstTransform> AstTransforms {
			get { return astTransforms; }
		}

		public CSharpDecompiler(ModuleDefinition module)
			: this(new DecompilerTypeSystem(module))
		{
		}

		public CSharpDecompiler(DecompilerTypeSystem typeSystem)
		{
			if (typeSystem == null)
				throw new ArgumentNullException("typeSystem");
			this.typeSystem = typeSystem;
		}
		
		TypeSystemAstBuilder CreateAstBuilder(ITypeResolveContext decompilationContext)
		{
			var typeSystemAstBuilder = new TypeSystemAstBuilder();
			typeSystemAstBuilder.ShowAttributes = true;
			typeSystemAstBuilder.AlwaysUseShortTypeNames = true;
			typeSystemAstBuilder.AddResolveResultAnnotations = true;
			return typeSystemAstBuilder;
		}
		
		void RunTransforms(AstNode rootNode, ITypeResolveContext decompilationContext)
		{
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			var context = new TransformContext(typeSystem, decompilationContext, typeSystemAstBuilder, CancellationToken);
			foreach (var transform in astTransforms) {
				CancellationToken.ThrowIfCancellationRequested();
				transform.Run(rootNode, context);
			}
			rootNode.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
		}
		
		public SyntaxTree DecompileWholeModuleAsSingleFile()
		{
			var decompilationContext = new SimpleTypeResolveContext(typeSystem.MainAssembly);
			SyntaxTree syntaxTree = new SyntaxTree();
			foreach (var a in typeSystem.Compilation.MainAssembly.AssemblyAttributes)
			{
				var astBuilder = CreateAstBuilder(decompilationContext);
				var attrSection = new AttributeSection(astBuilder.ConvertAttribute(a));
				attrSection.AttributeTarget = "assembly";
				syntaxTree.AddChild(attrSection, SyntaxTree.MemberRole);
			}
			string currentNamespace = null;
			AstNode groupNode = null;
			foreach (var cecilType in typeSystem.ModuleDefinition.Types) {
				var typeDef = typeSystem.Resolve(cecilType).GetDefinition();
				if (typeDef.Name == "<Module>" && typeDef.Members.Count == 0)
					continue;
				if(string.IsNullOrEmpty(cecilType.Namespace)) {
					groupNode = syntaxTree;
				} else {
					if (currentNamespace != cecilType.Namespace)
					{
						groupNode = new NamespaceDeclaration(cecilType.Namespace);
						syntaxTree.AddChild(groupNode, SyntaxTree.MemberRole);
					}
				}
				currentNamespace = cecilType.Namespace;
				var typeDecl = DoDecompile(typeDef, decompilationContext.WithCurrentTypeDefinition(typeDef));
				groupNode.AddChild(typeDecl, SyntaxTree.MemberRole);
			}
			RunTransforms(syntaxTree, decompilationContext);
			return syntaxTree;
		}
		
		public EntityDeclaration Decompile(TypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
				throw new ArgumentNullException("typeDefinition");
			ITypeDefinition typeDef = typeSystem.Resolve(typeDefinition).GetDefinition();
			if (typeDef == null)
				throw new InvalidOperationException("Could not find type definition in NR type system");
			var decompilationContext = new SimpleTypeResolveContext(typeDef);
			var decl = DoDecompile(typeDef, decompilationContext);
			RunTransforms(decl, decompilationContext);
			return decl;
		}
		
		EntityDeclaration DoDecompile(ITypeDefinition typeDef, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentTypeDefinition == typeDef);
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			var entityDecl = typeSystemAstBuilder.ConvertEntity(typeDef);
			var typeDecl = entityDecl as TypeDeclaration;
			if (typeDecl == null) {
				// e.g. DelegateDeclaration
				return entityDecl;
			}
			foreach (var type in typeDef.NestedTypes) {
				typeDecl.Members.Add(DoDecompile(type, decompilationContext.WithCurrentTypeDefinition(type)));
			}
			foreach (var field in typeDef.Fields) {
				var fieldDef = typeSystem.GetCecil(field) as FieldDefinition;
				if (fieldDef != null) {
					var memberDecl = DoDecompile(fieldDef, field, decompilationContext.WithCurrentMember(field));
					typeDecl.Members.Add(memberDecl);
				}
			}
			foreach (var property in typeDef.Properties) {
				var propDef = typeSystem.GetCecil(property) as PropertyDefinition;
				if (propDef != null) {
					var propDecl = DoDecompile(propDef, property, decompilationContext.WithCurrentMember(property));
					typeDecl.Members.Add(propDecl);
				}
			}
			foreach (var @event in typeDef.Events) {
				var eventDef = typeSystem.GetCecil(@event) as EventDefinition;
				if (eventDef != null) {
					var eventDecl = DoDecompile(eventDef, @event, decompilationContext.WithCurrentMember(@event));
					typeDecl.Members.Add(eventDecl);
				}
			}
			foreach (var method in typeDef.Methods) {
				var methodDef = typeSystem.GetCecil(method) as MethodDefinition;
				if (methodDef != null) {
					var memberDecl = DoDecompile(methodDef, method, decompilationContext.WithCurrentMember(method));
					typeDecl.Members.Add(memberDecl);
				}
			}
			if (typeDecl.Members.OfType<IndexerDeclaration>().Any(idx => idx.PrivateImplementationType.IsNull)) {
				// Remove the [DefaultMember] attribute if the class contains indexers
				foreach (AttributeSection section in typeDecl.Attributes) {
					foreach (var attr in section.Attributes) {
						var tr = attr.Type.GetResolveResult().Type;
						if (tr.Name == "DefaultMemberAttribute" && tr.Namespace == "System.Reflection") {
							attr.Remove();
						}
					}
					if (section.Attributes.Count == 0)
						section.Remove();
				}
			}
			
			return typeDecl;
		}
		
		public EntityDeclaration Decompile(MethodDefinition methodDefinition)
		{
			if (methodDefinition == null)
				throw new ArgumentNullException("methodDefinition");
			var method = typeSystem.Resolve(methodDefinition);
			if (method == null)
				throw new InvalidOperationException("Could not find method in NR type system");
			var decompilationContext = new SimpleTypeResolveContext(method);
			var decl = DoDecompile(methodDefinition, method, decompilationContext);
			RunTransforms(decl, decompilationContext);
			return decl;
		}

		EntityDeclaration DoDecompile(MethodDefinition methodDefinition, IMethod method, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == method);
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			var entityDecl = typeSystemAstBuilder.ConvertEntity(method);
			if (methodDefinition.HasBody) {
				DecompileBody(methodDefinition, method, entityDecl, decompilationContext, typeSystemAstBuilder);
			}
			return entityDecl;
		}

		IDecompilerTypeSystem GetSpecializingTypeSystem(ITypeResolveContext decompilationContext)
		{
			IList<IType> classTypeParameters = null;
			IList<IType> methodTypeParameters = null;
			
			if (decompilationContext.CurrentTypeDefinition != null)
				classTypeParameters = decompilationContext.CurrentTypeDefinition.TypeArguments;
			IMethod method = decompilationContext.CurrentMember as IMethod;
			if (method != null)
				methodTypeParameters = method.TypeArguments;
			
			if ((classTypeParameters != null && classTypeParameters.Count > 0) || (methodTypeParameters != null && methodTypeParameters.Count > 0))
				return new SpecializingDecompilerTypeSystem(typeSystem, new TypeParameterSubstitution(classTypeParameters, methodTypeParameters));
			else
				return typeSystem;
		}
		
		void DecompileBody(MethodDefinition methodDefinition, IMethod method, EntityDeclaration entityDecl, ITypeResolveContext decompilationContext, TypeSystemAstBuilder typeSystemAstBuilder)
		{
			var specializingTypeSystem = GetSpecializingTypeSystem(decompilationContext);
			var ilReader = new ILReader(specializingTypeSystem);
			var function = ilReader.ReadIL(methodDefinition.Body, CancellationToken);
			function.CheckInvariant();
			var context = new ILTransformContext { TypeSystem = specializingTypeSystem, CancellationToken = CancellationToken };
			foreach (var transform in ilTransforms) {
				CancellationToken.ThrowIfCancellationRequested();
				transform.Run(function, context);
				function.CheckInvariant();
			}
			var statementBuilder = new StatementBuilder(decompilationContext, method);
			var body = statementBuilder.ConvertAsBlock(function.Body);
			
			// insert variables at start of body
			Statement prevVarDecl = null;
			foreach (var v in function.Variables) {
				if (v.LoadCount == 0 && v.StoreCount == 0 && v.AddressCount == 0)
					continue;
				if (v.Kind == VariableKind.Local || v.Kind == VariableKind.StackSlot) {
					var type = typeSystemAstBuilder.ConvertType(v.Type);
					var varDecl = new VariableDeclarationStatement(type, v.Name);
					body.Statements.InsertAfter(prevVarDecl, varDecl);
					prevVarDecl = varDecl;
				}
			}

			entityDecl.AddChild(body, Roles.Body);
		}

		EntityDeclaration DoDecompile(FieldDefinition fieldDefinition, IField field, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == field);
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			if (decompilationContext.CurrentTypeDefinition.Kind == TypeKind.Enum) {
				var enumDec = new EnumMemberDeclaration() {
					Name = field.Name,
					Initializer = typeSystemAstBuilder.ConvertConstantValue(decompilationContext.CurrentTypeDefinition.EnumUnderlyingType, field.ConstantValue),
				};
				enumDec.Attributes.AddRange(field.Attributes.Select(a => new AttributeSection(typeSystemAstBuilder.ConvertAttribute(a))));
				return enumDec;
			}
			return typeSystemAstBuilder.ConvertEntity(field);
		}

		EntityDeclaration DoDecompile(PropertyDefinition propertyDefinition, IProperty property, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == property);
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			EntityDeclaration entityDecl = typeSystemAstBuilder.ConvertEntity(property);
			Accessor getter, setter;
			if (entityDecl is PropertyDeclaration) {
				getter = ((PropertyDeclaration)entityDecl).Getter;
				setter = ((PropertyDeclaration)entityDecl).Setter;
			} else {
				getter = ((IndexerDeclaration)entityDecl).Getter;
				setter = ((IndexerDeclaration)entityDecl).Setter;
			}
			if (property.CanGet && property.Getter.HasBody) {
				DecompileBody(propertyDefinition.GetMethod, property.Getter, getter, decompilationContext, typeSystemAstBuilder);
			}
			if (property.CanSet && property.Setter.HasBody) {
				DecompileBody(propertyDefinition.SetMethod, property.Setter, setter, decompilationContext, typeSystemAstBuilder);
			}
			return entityDecl;
		}
		
		EntityDeclaration DoDecompile(EventDefinition propertyDefinition, IEvent ev, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == ev);
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			typeSystemAstBuilder.UseCustomEvents = true;
			var eventDecl = (CustomEventDeclaration)typeSystemAstBuilder.ConvertEntity(ev);
			if (propertyDefinition.AddMethod != null && propertyDefinition.AddMethod.HasBody) {
				DecompileBody(propertyDefinition.AddMethod, ev.AddAccessor, eventDecl.AddAccessor, decompilationContext, typeSystemAstBuilder);
			}
			if (propertyDefinition.RemoveMethod != null && propertyDefinition.RemoveMethod.HasBody) {
				DecompileBody(propertyDefinition.RemoveMethod, ev.RemoveAccessor, eventDecl.RemoveAccessor, decompilationContext, typeSystemAstBuilder);
			}
			return eventDecl;
		}
	}
}
