// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using Mono.Cecil;

namespace ILSpy.BamlDecompiler
{
	/// <summary>
	/// Represents an event registration of a XAML code-behind class.
	/// </summary>
	sealed class EventRegistration
	{
		public string EventName, MethodName;
	}
	
	/// <summary>
	/// Decompiles event and name mappings of XAML code-behind classes.
	/// </summary>
	sealed class ConnectMethodDecompiler
	{
		AssemblyDefinition assembly;
		
		public ConnectMethodDecompiler(AssemblyDefinition assembly)
		{
			this.assembly = assembly;
		}
		
		public List<(LongSet, EventRegistration[])> DecompileEventMappings(string fullTypeName, CancellationToken cancellationToken)
		{
			var result = new List<(LongSet, EventRegistration[])>();
			TypeDefinition type = this.assembly.MainModule.GetType(fullTypeName);
			
			if (type == null)
				return result;
			
			MethodDefinition method = null;
			
			foreach (var m in type.Methods) {
				if (m.Name == "System.Windows.Markup.IComponentConnector.Connect") {
					method = m;
					break;
				}
			}
			
			if (method == null)
				return result;
			
			// decompile method and optimize the switch
			var typeSystem = new DecompilerTypeSystem(method.Module);
			var ilReader = new ILReader(typeSystem);
			var function = ilReader.ReadIL(method.Body, cancellationToken);

			var context = new ILTransformContext(function, typeSystem) {
				CancellationToken = cancellationToken
			};
			function.RunTransforms(CSharpDecompiler.GetILTransforms(), context);
			
			var block = function.Body.Children.OfType<Block>().First();
			var ilSwitch = block.Descendants.OfType<SwitchInstruction>().FirstOrDefault();
			
			if (ilSwitch != null) {
				foreach (var section in ilSwitch.Sections) {
					var events = FindEvents(section.Body);
					result.Add((section.Labels, events));
				}
			} else {
				foreach (var ifInst in function.Descendants.OfType<IfInstruction>()) {
					var comp = ifInst.Condition as Comp;
					if (comp.Kind != ComparisonKind.Inequality && comp.Kind != ComparisonKind.Equality)
						continue;
					int id;
					if (!comp.Right.MatchLdcI4(out id))
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
			
			switch (inst) {
				case Block b:
					foreach (var node in ((Block)inst).Instructions) {
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
			
			string eventName, handlerName;
			if (IsAddEvent(call, out eventName, out handlerName) || IsAddAttachedEvent(call, out eventName, out handlerName))
				events.Add(new EventRegistration { EventName = eventName, MethodName = handlerName });
		}
		
		bool IsAddAttachedEvent(CallInstruction call, out string eventName, out string handlerName)
		{
			eventName = "";
			handlerName = "";
			
			if (call.Arguments.Count == 3) {
				var addMethod = call.Method;
				if (addMethod.Name != "AddHandler" || addMethod.Parameters.Count != 2)
					return false;
				IField field;
				if (!call.Arguments[1].MatchLdsFld(out field))
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
			
			if (call.Arguments.Count == 2) {
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
				return true;
			}
			
			return false;
		}
	}
}
