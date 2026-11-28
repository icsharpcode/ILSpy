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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.TypeSystem
{
	/// <summary>
	/// An overriding property with a single accessor copies its accessibility from the property that
	/// introduced it. The search for that property used to ask every override on the way for its
	/// accessibility, which started the same search again. The tree view of ILSpy uses an uncached
	/// type system, where every lookup creates new entities, so no state kept in them ends the recursion.
	/// </summary>
	[TestFixture(TypeSystemOptions.Default)]
	[TestFixture(TypeSystemOptions.Default | TypeSystemOptions.Uncached)]
	public class OverridingPropertyAccessibilityTests(TypeSystemOptions options)
	{
		[Test]
		public void CyclicInheritance()
		{
			var compilation = CreateCompilation((BaseType: 1, Getter: 0), (BaseType: 0, Getter: 1));

			Assert.That(GetProperty(compilation, "C0").Accessibility, Is.EqualTo(Accessibility.Protected));
			Assert.That(GetProperty(compilation, "C1").Accessibility, Is.EqualTo(Accessibility.Protected));
		}

		[Test]
		public void GetterDeclaredInDerivedClass()
		{
			var compilation = CreateCompilation((BaseType: -1, Getter: 1), (BaseType: 0, Getter: -1));

			var property = GetProperty(compilation, "C0");
			Assert.That(property.DeclaringTypeDefinition.Name, Is.EqualTo("C1"));
			Assert.That(property.Accessibility, Is.EqualTo(Accessibility.Protected));
		}

		[Test]
		public void GenericConstructedBaseType()
		{
			var compilation = CreateGenericCompilation();
			var derivedType = compilation.MainModule.TopLevelTypeDefinitions.Single(type => type.Name == "Derived");
			var constructedBaseType = derivedType.GetNonInterfaceBaseTypes().Single(type => type.Name == "Base");

			Assert.That(constructedBaseType, Is.InstanceOf<ParameterizedType>());
			Assert.That(derivedType.Equals(constructedBaseType), Is.False);
			Assert.That(GetProperty(compilation, "Derived").Accessibility, Is.EqualTo(Accessibility.ProtectedOrInternal));
		}

		[Test]
		public void MissingBaseReferencePreservesAccessorAccessibility()
		{
			var compilation = CreateCompilation((BaseType: -2, Getter: 0));

			Assert.That(GetProperty(compilation, "C0").Accessibility, Is.EqualTo(Accessibility.Protected));
		}

		[Test]
		public void LongChainOfOverrides()
		{
			var compilation = CreateCompilation(Enumerable.Range(0, 64).Select(i => (BaseType: i - 1, Getter: i)).ToArray());

			Assert.That(GetProperty(compilation, "C0").Accessibility, Is.EqualTo(Accessibility.ProtectedOrInternal));
			Assert.That(GetProperty(compilation, "C63").Getter.Accessibility, Is.EqualTo(Accessibility.Protected));
			Assert.That(GetProperty(compilation, "C63").Accessibility, Is.EqualTo(Accessibility.ProtectedOrInternal));
		}

		static IProperty GetProperty(ICompilation compilation, string className)
		{
			return compilation.MainModule.TopLevelTypeDefinitions.Single(t => t.Name == className).Properties.Single();
		}

		/// <summary>
		/// Builds a minimal in-memory assembly containing one class per element of <paramref name="classes"/>.
		/// The class with index i is named "Ci". BaseType is -1 for System.Object, -2 for an unresolved base type,
		/// or the index of another class. Each class declares "get_P" and, unless Getter is -1, lists property "P"
		/// with the "get_P" method from the class at index Getter. The getter is protected internal virtual and newslot
		/// when BaseType is -1; otherwise it is protected and overrides the base getter.
		/// </summary>
		ICompilation CreateCompilation(params (int BaseType, int Getter)[] classes)
		{
			var metadata = new MetadataBuilder();
			metadata.AddModule(0, metadata.GetOrAddString("test.dll"),
				metadata.GetOrAddGuid(Guid.NewGuid()), default, default);
			metadata.AddAssembly(metadata.GetOrAddString("test"), new Version(1, 0, 0, 0),
				default, default, 0, AssemblyHashAlgorithm.None);
			var mscorlib = metadata.AddAssemblyReference(metadata.GetOrAddString("mscorlib"),
				new Version(4, 0, 0, 0), default, default, 0, default);
			var objectType = metadata.AddTypeReference(mscorlib,
				metadata.GetOrAddString("System"), metadata.GetOrAddString("Object"));
			var missingAssembly = metadata.AddAssemblyReference(metadata.GetOrAddString("MissingAssembly"),
				new Version(1, 0, 0, 0), default, default, 0, default);
			var missingBaseType = metadata.AddTypeReference(missingAssembly,
				metadata.GetOrAddString("Missing"), metadata.GetOrAddString("Base"));

			var getterSignature = new BlobBuilder();
			new BlobEncoder(getterSignature).MethodSignature(isInstanceMethod: true)
				.Parameters(0, returnType => returnType.Type().Int32(), parameters => { });
			var propertySignature = new BlobBuilder();
			new BlobEncoder(propertySignature).PropertySignature(isInstanceProperty: true)
				.Parameters(0, returnType => returnType.Type().Int32(), parameters => { });

			metadata.AddTypeDefinition(default, default,
				metadata.GetOrAddString("<Module>"), baseType: default,
				fieldList: MetadataTokens.FieldDefinitionHandle(1), methodList: MetadataTokens.MethodDefinitionHandle(1));
			for (int i = 0; i < classes.Length; i++)
			{
				var (baseType, getter) = classes[i];
				var method = metadata.AddMethodDefinition(
					MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual
						| (baseType == -1 ? MethodAttributes.FamORAssem | MethodAttributes.NewSlot : MethodAttributes.Family),
					MethodImplAttributes.IL, metadata.GetOrAddString("get_P"), metadata.GetOrAddBlob(getterSignature),
					bodyOffset: -1, parameterList: MetadataTokens.ParameterHandle(1));
				// The row 1 is <Module>.
				var typeDef = metadata.AddTypeDefinition(TypeAttributes.Public, default, metadata.GetOrAddString("C" + i),
					baseType switch {
						-1 => objectType,
						-2 => missingBaseType,
						_ => MetadataTokens.TypeDefinitionHandle(baseType + 2)
					},
					fieldList: MetadataTokens.FieldDefinitionHandle(1), methodList: method);
				if (getter >= 0)
				{
					var property = metadata.AddProperty(PropertyAttributes.None,
						metadata.GetOrAddString("P"), metadata.GetOrAddBlob(propertySignature));
					metadata.AddPropertyMap(typeDef, property);
					metadata.AddMethodSemantics(property, MethodSemanticsAttributes.Getter,
						MetadataTokens.MethodDefinitionHandle(getter + 1));
				}
			}

			var peBlob = new BlobBuilder();
			new ManagedPEBuilder(PEHeaderBuilder.CreateLibraryHeader(),
				new MetadataRootBuilder(metadata), ilStream: new BlobBuilder()).Serialize(peBlob);
			var peFile = new PEFile("test.dll", new MemoryStream(peBlob.ToArray()));
			return new SimpleCompilation(peFile.WithOptions(options), MinimalCorlib.Instance);
		}

		ICompilation CreateGenericCompilation()
		{
			var metadata = new MetadataBuilder();
			metadata.AddModule(0, metadata.GetOrAddString("test.dll"),
				metadata.GetOrAddGuid(Guid.NewGuid()), default, default);
			metadata.AddAssembly(metadata.GetOrAddString("test"), new Version(1, 0, 0, 0),
				default, default, 0, AssemblyHashAlgorithm.None);
			var mscorlib = metadata.AddAssemblyReference(metadata.GetOrAddString("mscorlib"),
				new Version(4, 0, 0, 0), default, default, 0, default);
			var objectType = metadata.AddTypeReference(mscorlib,
				metadata.GetOrAddString("System"), metadata.GetOrAddString("Object"));

			var getterSignature = new BlobBuilder();
			new BlobEncoder(getterSignature).MethodSignature(isInstanceMethod: true)
				.Parameters(0, returnType => returnType.Type().Int32(), parameters => { });
			var propertySignature = new BlobBuilder();
			new BlobEncoder(propertySignature).PropertySignature(isInstanceProperty: true)
				.Parameters(0, returnType => returnType.Type().Int32(), parameters => { });

			metadata.AddTypeDefinition(default, default,
				metadata.GetOrAddString("<Module>"), baseType: default,
				fieldList: MetadataTokens.FieldDefinitionHandle(1), methodList: MetadataTokens.MethodDefinitionHandle(1));

			var baseMethod = metadata.AddMethodDefinition(
				MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual
					| MethodAttributes.FamORAssem | MethodAttributes.NewSlot,
				MethodImplAttributes.IL, metadata.GetOrAddString("get_P"), metadata.GetOrAddBlob(getterSignature),
				bodyOffset: -1, parameterList: MetadataTokens.ParameterHandle(1));
			var baseType = metadata.AddTypeDefinition(TypeAttributes.Public, default, metadata.GetOrAddString("Base`1"),
				objectType, MetadataTokens.FieldDefinitionHandle(1), baseMethod);
			var baseProperty = metadata.AddProperty(PropertyAttributes.None,
				metadata.GetOrAddString("P"), metadata.GetOrAddBlob(propertySignature));
			metadata.AddPropertyMap(baseType, baseProperty);
			metadata.AddMethodSemantics(baseProperty, MethodSemanticsAttributes.Getter, baseMethod);

			var genericBaseSignature = new BlobBuilder();
			new BlobEncoder(genericBaseSignature).TypeSpecificationSignature()
				.GenericInstantiation(baseType, 1, isValueType: false)
				.AddArgument().GenericTypeParameter(0);
			var genericBase = metadata.AddTypeSpecification(metadata.GetOrAddBlob(genericBaseSignature));
			var derivedMethod = metadata.AddMethodDefinition(
				MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual | MethodAttributes.Family,
				MethodImplAttributes.IL, metadata.GetOrAddString("get_P"), metadata.GetOrAddBlob(getterSignature),
				bodyOffset: -1, parameterList: MetadataTokens.ParameterHandle(1));
			var derivedType = metadata.AddTypeDefinition(TypeAttributes.Public, default, metadata.GetOrAddString("Derived`1"),
				genericBase, MetadataTokens.FieldDefinitionHandle(1), derivedMethod);
			var derivedProperty = metadata.AddProperty(PropertyAttributes.None,
				metadata.GetOrAddString("P"), metadata.GetOrAddBlob(propertySignature));
			metadata.AddPropertyMap(derivedType, derivedProperty);
			metadata.AddMethodSemantics(derivedProperty, MethodSemanticsAttributes.Getter, derivedMethod);

			metadata.AddGenericParameter(baseType, GenericParameterAttributes.None,
				metadata.GetOrAddString("T"), 0);
			metadata.AddGenericParameter(derivedType, GenericParameterAttributes.None,
				metadata.GetOrAddString("T"), 0);

			var peBlob = new BlobBuilder();
			new ManagedPEBuilder(PEHeaderBuilder.CreateLibraryHeader(),
				new MetadataRootBuilder(metadata), ilStream: new BlobBuilder()).Serialize(peBlob);
			var peFile = new PEFile("test.dll", new MemoryStream(peBlob.ToArray()));
			return new SimpleCompilation(peFile.WithOptions(options), MinimalCorlib.Instance);
		}
	}
}
