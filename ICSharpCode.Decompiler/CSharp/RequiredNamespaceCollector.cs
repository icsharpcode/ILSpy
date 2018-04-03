using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

using static ICSharpCode.Decompiler.Metadata.ILOpCodeExtensions;

namespace ICSharpCode.Decompiler.CSharp
{
	class RequiredNamespaceCollector
	{
		public HashSet<string> Namespaces { get; }

		HashSet<EntityHandle> visited = new HashSet<EntityHandle>();
		DecompilerTypeSystem typeSystem;

		public static void CollectNamespaces(DecompilerTypeSystem typeSystem, HashSet<string> namespaces)
		{
			foreach (var type in typeSystem.MainAssembly.GetAllTypeDefinitions()) {
				namespaces.Add(type.Namespace);
				HandleAttributes(type.Attributes, namespaces);

				foreach (var field in type.Fields) {
					HandleAttributes(field.Attributes, namespaces);
				}

				foreach (var property in type.Properties) {
					HandleAttributes(property.Attributes, namespaces);
				}

				foreach (var @event in type.Events) {
					HandleAttributes(@event.Attributes, namespaces);
				}

				foreach (var method in type.Methods) {
					HandleAttributes(method.Attributes, namespaces);
				}
			}
		}

		public static void CollectNamespaces(IEntity entity, DecompilerTypeSystem typeSystem, HashSet<string> namespaces)
		{
		}

		public static void CollectNamespaces(EntityHandle entity, DecompilerTypeSystem typeSystem, HashSet<string> namespaces)
		{
		}


		static void HandleAttributes(IEnumerable<IAttribute> attributes, HashSet<string> namespaces)
		{
			foreach (var attr in attributes) {
				namespaces.Add(attr.AttributeType.Namespace);
				foreach (var arg in attr.PositionalArguments) {
					namespaces.Add(arg.Type.Namespace);
					if (arg is TypeOfResolveResult torr)
						namespaces.Add(torr.ReferencedType.Namespace);
				}
				foreach (var arg in attr.NamedArguments) {
					namespaces.Add(arg.Value.Type.Namespace);
					if (arg.Value is TypeOfResolveResult torr)
						namespaces.Add(torr.ReferencedType.Namespace);
				}
			}
		}

		void CollectNamespacesFromMethodBody(MethodBodyBlock method, PEReader reader)
		{
			var instructions = method.GetILReader();
			var metadata = reader.GetMetadataReader();
			while (instructions.RemainingBytes > 0) {
				var opCode = instructions.DecodeOpCode();
				switch (opCode.GetOperandType()) {
					case Metadata.OperandType.Field:
					case Metadata.OperandType.Method:
					case Metadata.OperandType.Sig:
					case Metadata.OperandType.Tok:
					case Metadata.OperandType.Type:
						var handle = MetadataTokens.EntityHandle(instructions.ReadInt32());
						//CollectNamespaces(handle, metadata);
						break;
					default:
						instructions.SkipOperand(opCode);
						break;
				}
			}
		}
	}
}
