// Copyright (c) 2019 Siegfried Pammer
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
using System.Reflection.Metadata;
using System.Xml.Linq;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

using ILSpy.BamlDecompiler.Xaml;

namespace ILSpy.BamlDecompiler.Rewrite
{
	internal class ConnectionIdRewritePass : IRewritePass
	{
		static readonly TopLevelTypeName componentConnectorTypeName = new TopLevelTypeName("System.Windows.Markup", "IComponentConnector");

		public void Run(XamlContext ctx, XDocument document)
		{
			var mappings = DecompileEventMappings(ctx, document);
			ProcessConnectionIds(document.Root, mappings);
		}

		static void ProcessConnectionIds(XElement element, List<(LongSet key, EventRegistration[] value)> eventMappings)
		{
			foreach (var child in element.Elements())
				ProcessConnectionIds(child, eventMappings);

			foreach (var annotation in element.Annotations<BamlConnectionId>())
			{
				int index;
				if ((index = eventMappings.FindIndex(item => item.key.Contains(annotation.Id))) > -1)
				{
					foreach (var entry in eventMappings[index].value)
					{
						string xmlns = ""; // TODO : implement xmlns resolver!
						element.Add(new XAttribute(xmlns + entry.EventName, entry.MethodName));
					}
				}
			}
		}

		List<(LongSet, EventRegistration[])> DecompileEventMappings(XamlContext ctx, XDocument document)
		{
			var result = new List<(LongSet, EventRegistration[])>();

			var xClass = document.Root.Elements().First().Attribute(ctx.GetKnownNamespace("Class", XamlContext.KnownNamespace_Xaml));
			if (xClass == null)
				return result;

			var type = ctx.TypeSystem.FindType(new FullTypeName(xClass.Value)).GetDefinition();
			if (type == null)
				return result;

			var connectorInterface = ctx.TypeSystem.FindType(componentConnectorTypeName).GetDefinition();
			if (connectorInterface == null)
				return result;
			var connect = connectorInterface.GetMethods(m => m.Name == "Connect").SingleOrDefault();

			IMethod method = null;
			MethodDefinition metadataEntry = default;
			var module = ctx.TypeSystem.MainModule.PEFile;

			foreach (IMethod m in type.Methods)
			{
				if (m.ExplicitlyImplementedInterfaceMembers.Any(md => md.MemberDefinition.Equals(connect)))
				{
					method = m;
					metadataEntry = module.Metadata.GetMethodDefinition((MethodDefinitionHandle)method.MetadataToken);
					break;
				}
			}

			if (method == null || metadataEntry.RelativeVirtualAddress <= 0)
				return result;

			var body = module.Reader.GetMethodBody(metadataEntry.RelativeVirtualAddress);
			var genericContext = new GenericContext(
				classTypeParameters: method.DeclaringType?.TypeParameters,
				methodTypeParameters: method.TypeParameters);

			// decompile method and optimize the switch
			var ilReader = new ILReader(ctx.TypeSystem.MainModule);
			var function = ilReader.ReadIL((MethodDefinitionHandle)method.MetadataToken, body, genericContext, ILFunctionKind.TopLevelFunction, ctx.CancellationToken);

			var context = new ILTransformContext(function, ctx.TypeSystem, null) {
				CancellationToken = ctx.CancellationToken
			};
			function.RunTransforms(CSharpDecompiler.GetILTransforms(), context);

			var block = function.Body.Children.OfType<Block>().First();
			var ilSwitch = block.Descendants.OfType<SwitchInstruction>().FirstOrDefault();

			if (ilSwitch != null)
			{
				foreach (var section in ilSwitch.Sections)
				{
					var events = FindEvents(section.Body);
					result.Add((section.Labels, events));
				}
			}
			else
			{
				foreach (var ifInst in function.Descendants.OfType<IfInstruction>())
				{
					if (!(ifInst.Condition is Comp comp))
						continue;
					if (comp.Kind != ComparisonKind.Inequality && comp.Kind != ComparisonKind.Equality)
						continue;
					if (!comp.Right.MatchLdcI4(out int id))
						continue;
					var events = FindEvents(comp.Kind == ComparisonKind.Inequality ? ifInst.FalseInst : ifInst.TrueInst);
					result.Add((new LongSet(id), events));
				}
			}
			return result;
		}

		EventRegistration[] FindEvents(ILInstruction inst)
		{
			var events = new List<EventRegistration>();

			switch (inst)
			{
				case Block _:
					foreach (var node in ((Block)inst).Instructions)
					{
						FindEvents(node, events);
					}
					FindEvents(((Block)inst).FinalInstruction, events);
					break;
				case Branch br:
					return FindEvents(br.TargetBlock);
				default:
					FindEvents(inst, events);
					break;
			}
			return events.ToArray();
		}

		void FindEvents(ILInstruction inst, List<EventRegistration> events)
		{
			CallInstruction call = inst as CallInstruction;
			if (call == null || call.OpCode == OpCode.NewObj)
				return;

			if (IsAddEvent(call, out string eventName, out string handlerName) || IsAddAttachedEvent(call, out eventName, out handlerName))
				events.Add(new EventRegistration { EventName = eventName, MethodName = handlerName });
		}

		bool IsAddAttachedEvent(CallInstruction call, out string eventName, out string handlerName)
		{
			eventName = "";
			handlerName = "";

			if (call.Arguments.Count == 3)
			{
				var addMethod = call.Method;
				if (addMethod.Name != "AddHandler" || addMethod.Parameters.Count != 2)
					return false;
				if (!call.Arguments[1].MatchLdsFld(out IField field))
					return false;
				eventName = field.DeclaringType.Name + "." + field.Name;
				if (eventName.EndsWith("Event", StringComparison.Ordinal) && eventName.Length > "Event".Length)
					eventName = eventName.Remove(eventName.Length - "Event".Length);
				var newObj = call.Arguments[2] as NewObj;
				if (newObj == null || newObj.Arguments.Count != 2)
					return false;
				var ldftn = newObj.Arguments[1];
				if (ldftn.OpCode != OpCode.LdFtn && ldftn.OpCode != OpCode.LdVirtFtn)
					return false;
				handlerName = ((IInstructionWithMethodOperand)ldftn).Method.Name;
				return true;
			}

			return false;
		}

		bool IsAddEvent(CallInstruction call, out string eventName, out string handlerName)
		{
			eventName = "";
			handlerName = "";

			if (call.Arguments.Count == 2)
			{
				var addMethod = call.Method;
				if (!addMethod.Name.StartsWith("add_", StringComparison.Ordinal) || addMethod.Parameters.Count != 1)
					return false;
				eventName = addMethod.Name.Substring("add_".Length);
				var newObj = call.Arguments[1] as NewObj;
				if (newObj == null || newObj.Arguments.Count != 2)
					return false;
				var ldftn = newObj.Arguments[1];
				if (ldftn.OpCode != OpCode.LdFtn && ldftn.OpCode != OpCode.LdVirtFtn)
					return false;
				handlerName = ((IInstructionWithMethodOperand)ldftn).Method.Name;
				handlerName = XamlUtils.EscapeName(handlerName);
				return true;
			}

			return false;
		}
	}
}
