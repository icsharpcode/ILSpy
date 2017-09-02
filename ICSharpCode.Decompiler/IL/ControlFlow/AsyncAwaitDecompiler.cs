using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	/// <summary>
	/// Decompiler step for C# 5 async/await.
	/// </summary>
	class AsyncAwaitDecompiler : IILTransform
	{
		/*
		public static bool IsCompilerGeneratedStateMachine(TypeDefinition type)
		{
			if (!(type.DeclaringType != null && type.IsCompilerGenerated()))
				return false;
			foreach (TypeReference i in type.Interfaces) {
				if (i.Namespace == "System.Runtime.CompilerServices" && i.Name == "IAsyncStateMachine")
					return true;
			}
			return false;
		}
		*/

		enum AsyncMethodType
		{
			Void,
			Task,
			TaskOfT
		}

		ILTransformContext context;

		// These fields are set by MatchTaskCreationPattern()
		IType taskType; // return type of the async method
		IType underlyingReturnType; // return type of the method (only the "T" for Task{T})
		AsyncMethodType methodType;
		ITypeDefinition stateMachineType;
		ITypeDefinition builderType;
		IField builderField;
		IField stateField;
		int initialState;
		Dictionary<IField, ILVariable> fieldToParameterMap = new Dictionary<IField, ILVariable>();

		// These fields are set by AnalyzeMoveNext():
		ILFunction moveNextFunction;
		ILVariable cachedStateVar; // variable in MoveNext that caches the stateField.
		TryCatch mainTryCatch;
		Block setResultAndExitBlock; // block that is jumped to for return statements
		int finalState;       // final state after the setResultAndExitBlock
		ILVariable resultVar; // the variable that gets returned by the setResultAndExitBlock

		public void Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.AsyncAwait)
				return; // abort if async/await decompilation is disabled
			this.context = context;
			fieldToParameterMap.Clear();
			if (!MatchTaskCreationPattern(function))
				return;
			try {
				AnalyzeMoveNext();
				ValidateCatchBlock();
				InlineBodyOfMoveNext(function);
			} catch (SymbolicAnalysisFailedException) {
				return;
			}
		}

		#region MatchTaskCreationPattern
		bool MatchTaskCreationPattern(ILFunction function)
		{
			if (!(function.Body is BlockContainer blockContainer))
				return false;
			if (blockContainer.Blocks.Count != 1)
				return false;
			var body = blockContainer.EntryPoint.Instructions;
			if (body.Count < 5)
				return false;
			/* Example:
			    V_0 is an instance of the compiler-generated struct/class,
				V_1 is an instance of the builder struct/class
				Block IL_0000 (incoming: 1)  {
					stobj System.Runtime.CompilerServices.AsyncVoidMethodBuilder(ldflda [Field ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async+<AwaitYield>d__3.<>t__builder](ldloca V_0), call Create())
					stobj System.Int32(ldflda [Field ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async+<AwaitYield>d__3.<>1__state](ldloca V_0), ldc.i4 -1)
					stloc V_1(ldobj System.Runtime.CompilerServices.AsyncVoidMethodBuilder(ldflda [Field ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async+<AwaitYield>d__3.<>t__builder](ldloc V_0)))
					call Start(ldloca V_1, ldloca V_0)
					leave IL_0000 (or ret for non-void async methods)
				}
			*/

			// Check the second-to-last instruction (the start call) first, as we can get the most information from that
			if (!(body[body.Count - 2] is Call startCall))
				return false;
			if (startCall.Method.Name != "Start")
				return false;
			taskType = context.TypeSystem.Resolve(function.Method.ReturnType);
			builderType = startCall.Method.DeclaringTypeDefinition;
			const string ns = "System.Runtime.CompilerServices";
			if (taskType.IsKnownType(KnownTypeCode.Void)) {
				methodType = AsyncMethodType.Void;
				underlyingReturnType = taskType;
				if (builderType?.FullTypeName != new TopLevelTypeName(ns, "AsyncVoidMethodBuilder"))
					return false;
			} else if (taskType.IsKnownType(KnownTypeCode.Task)) {
				methodType = AsyncMethodType.Task;
				underlyingReturnType = context.TypeSystem.Compilation.FindType(KnownTypeCode.Void);
				if (builderType?.FullTypeName != new TopLevelTypeName(ns, "AsyncTaskMethodBuilder", 0))
					return false;
			} else if (taskType.IsKnownType(KnownTypeCode.TaskOfT)) {
				methodType = AsyncMethodType.TaskOfT;
				underlyingReturnType = TaskType.UnpackTask(context.TypeSystem.Compilation, taskType);
				if (builderType?.FullTypeName != new TopLevelTypeName(ns, "AsyncTaskMethodBuilder", 1))
					return false;
			} else {
				return false; // TODO: generalized async return type
			}
			if (startCall.Arguments.Count != 2)
				return false;
			if (!startCall.Arguments[0].MatchLdLocRef(out ILVariable builderVar))
				return false;
			if (!startCall.Arguments[1].MatchLdLoca(out ILVariable stateMachineVar))
				return false;
			stateMachineType = stateMachineVar.Type.GetDefinition();
			if (stateMachineType == null)
				return false;

			// Check third-to-last instruction (copy of builder)
			// stloc builder(ldfld StateMachine::<>t__builder(ldloc stateMachine))
			if (!body[body.Count - 3].MatchStLoc(builderVar, out var loadBuilderExpr))
				return false;
			if (!loadBuilderExpr.MatchLdFld(out var loadStateMachineForBuilderExpr, out builderField))
				return false;
			builderField = (IField)builderField.MemberDefinition;
			if (!(loadStateMachineForBuilderExpr.MatchLdLocRef(stateMachineVar) || loadStateMachineForBuilderExpr.MatchLdLoc(stateMachineVar)))
				return false;

			// Check the last instruction (ret)
			if (methodType == AsyncMethodType.Void) {
				if (!body.Last().MatchLeave(blockContainer))
					return false;
			} else {
				// ret(call(AsyncTaskMethodBuilder::get_Task, ldflda(StateMachine::<>t__builder, ldloca(stateMachine))))
				if (!body.Last().MatchReturn(out var returnValue))
					return false;
				if (!MatchCall(returnValue, "get_Task", out var getTaskArgs) || getTaskArgs.Count != 1)
					return false;
				ILInstruction target;
				IField builderField2;
				if (builderType.IsReferenceType == true) {
					if (!getTaskArgs[0].MatchLdFld(out target, out builderField2))
						return false;
				} else {
					if (!getTaskArgs[0].MatchLdFlda(out target, out builderField2))
						return false;
				}
				if (builderField2.MemberDefinition != builderField)
					return false;
				if (!target.MatchLdLocRef(stateMachineVar))
					return false;
			}

			// Check the last field assignment - this should be the state field
			// stfld <>1__state(ldloca stateField, ldc.i4 -1)
			if (!MatchStFld(body[body.Count - 4], stateMachineVar, out stateField, out var initialStateExpr))
				return false;
			if (!initialStateExpr.MatchLdcI4(out initialState))
				return false;
			if (initialState != -1)
				return false;

			// Check the second-to-last field assignment - this should be the builder field
			// stfld StateMachine.builder(ldloca stateMachine, call Create())
			if (!MatchStFld(body[body.Count - 5], stateMachineVar, out var builderField3, out var builderInitialization))
				return false;
			if (builderField3 != builderField)
				return false;
			if (!(builderInitialization is Call createCall))
				return false;
			if (createCall.Method.Name != "Create" || createCall.Arguments.Count != 0)
				return false;

			int pos = 0;
			if (stateMachineType.Kind == TypeKind.Class) {
				// If state machine is a class, the first instruction creates an instance:
				// stloc stateMachine(newobj StateMachine.ctor())
				if (!body[pos].MatchStLoc(stateMachineVar, out var init))
					return false;
				if (!(init is NewObj newobj && newobj.Arguments.Count == 0 && newobj.Method.DeclaringTypeDefinition == stateMachineType))
					return false;
				pos++;
			}
			for (; pos < body.Count - 5; pos++) {
				// stfld StateMachine.field(ldloca stateMachine, ldvar(param))
				if (!MatchStFld(body[pos], stateMachineVar, out var field, out var fieldInit))
					return false;
				if (!fieldInit.MatchLdLoc(out var v))
					return false;
				if (v.Kind != VariableKind.Parameter)
					return false;
				fieldToParameterMap[field] = v;
			}

			return true;
		}

		/// <summary>
		/// Matches a (potentially virtual) instance method call.
		/// </summary>
		static bool MatchCall(ILInstruction inst, string name, out InstructionCollection<ILInstruction> args)
		{
			if (inst is CallInstruction call && (call.OpCode == OpCode.Call || call.OpCode == OpCode.CallVirt)
				&& call.Method.Name == name && !call.Method.IsStatic)
			{
				args = call.Arguments;
				return args.Count > 0;
			}
			args = null;
			return false;
		}

		/// <summary>
		/// Matches a store to the state machine.
		/// </summary>
		static bool MatchStFld(ILInstruction stfld, ILVariable stateMachineVar, out IField field, out ILInstruction value)
		{
			if (!stfld.MatchStFld(out var target, out field, out value))
				return false;
			field = field.MemberDefinition as IField;
			return field != null && target.MatchLdLocRef(stateMachineVar);
		}
		#endregion

		#region AnalyzeMoveNext
		/// <summary>
		/// First peek into MoveNext(); analyzes everything outside the big try-catch.
		/// </summary>
		void AnalyzeMoveNext()
		{
			var moveNextMethod = context.TypeSystem.GetCecil(stateMachineType)?.Methods.FirstOrDefault(f => f.Name == "MoveNext");
			if (moveNextMethod == null)
				throw new SymbolicAnalysisFailedException();
			moveNextFunction = YieldReturnDecompiler.CreateILAst(moveNextMethod, context);
			if (!(moveNextFunction.Body is BlockContainer blockContainer))
				throw new SymbolicAnalysisFailedException();
			if (blockContainer.Blocks.Count != 2)
				throw new SymbolicAnalysisFailedException();
			if (blockContainer.EntryPoint.IncomingEdgeCount != 1)
				throw new SymbolicAnalysisFailedException();
			int pos = 0;
			if (blockContainer.EntryPoint.Instructions[0].MatchStLoc(out cachedStateVar, out var cachedStateInit)) {
				// stloc(cachedState, ldfld(valuetype StateMachineStruct::<>1__state, ldloc(this)))
				if (!cachedStateInit.MatchLdFld(out var target, out var loadedField))
					throw new SymbolicAnalysisFailedException();
				if (!target.MatchLdThis())
					throw new SymbolicAnalysisFailedException();
				if (loadedField.MemberDefinition != stateField)
					throw new SymbolicAnalysisFailedException();
				++pos;
			}
			mainTryCatch = blockContainer.EntryPoint.Instructions[pos] as TryCatch;
			// TryCatch will be validated in ValidateCatchBlock()

			setResultAndExitBlock = blockContainer.Blocks[1];
			// stobj System.Int32(ldflda [Field ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async+<SimpleBoolTaskMethod>d__7.<>1__state](ldloc this), ldc.i4 -2)
			// call SetResult(ldflda [Field ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async+<SimpleBoolTaskMethod>d__7.<>t__builder](ldloc this), ldloc result)
			// leave IL_0000
			if (setResultAndExitBlock.Instructions.Count != 3)
				throw new SymbolicAnalysisFailedException();
			if (!MatchStateAssignment(setResultAndExitBlock.Instructions[0], out finalState))
				throw new SymbolicAnalysisFailedException();
			if (!MatchCall(setResultAndExitBlock.Instructions[1], "SetResult", out var args))
				throw new SymbolicAnalysisFailedException();
			if (!IsBuilderFieldOnThis(args[0]))
				throw new SymbolicAnalysisFailedException();
			if (methodType == AsyncMethodType.TaskOfT) {
				if (args.Count != 2)
					throw new SymbolicAnalysisFailedException();
				if (!args[1].MatchLdLoc(out resultVar))
					throw new SymbolicAnalysisFailedException();
			} else {
				resultVar = null;
				if (args.Count != 1)
					throw new SymbolicAnalysisFailedException();
			}
			if (!setResultAndExitBlock.Instructions[2].MatchLeave(blockContainer))
				throw new SymbolicAnalysisFailedException();
		}

		void ValidateCatchBlock()
		{
			// catch E_143 : System.Exception if (ldc.i4 1) BlockContainer {
			// 	Block IL_008f (incoming: 1)  {
			// 		stloc exception(ldloc E_143)
			// 		stfld <>1__state(ldloc this, ldc.i4 -2)
			// 		call SetException(ldfld <>t__builder(ldloc this), ldloc exception)
			// 		leave IL_0000
			// 	}
			// }
			if (mainTryCatch?.Handlers.Count != 1)
				throw new SymbolicAnalysisFailedException();
			var handler = mainTryCatch.Handlers[0];
			if (!handler.Variable.Type.IsKnownType(KnownTypeCode.Exception))
				throw new SymbolicAnalysisFailedException();
			if (!handler.Filter.MatchLdcI4(1))
				throw new SymbolicAnalysisFailedException();
			var catchBlock = YieldReturnDecompiler.SingleBlock(handler.Body);
			if (catchBlock?.Instructions.Count != 4)
				throw new SymbolicAnalysisFailedException();
			// stloc exception(ldloc E_143)
			if (!(catchBlock.Instructions[0] is StLoc stloc))
				throw new SymbolicAnalysisFailedException();
			if (!stloc.Value.MatchLdLoc(handler.Variable))
				throw new SymbolicAnalysisFailedException();
			// stfld <>1__state(ldloc this, ldc.i4 -2)
			if (!MatchStateAssignment(catchBlock.Instructions[1], out int newState) || newState != finalState)
				throw new SymbolicAnalysisFailedException();
			// call SetException(ldfld <>t__builder(ldloc this), ldloc exception)
			if (!MatchCall(catchBlock.Instructions[2], "SetException", out var args))
				throw new SymbolicAnalysisFailedException();
			if (args.Count != 2)
				throw new SymbolicAnalysisFailedException();
			if (!IsBuilderFieldOnThis(args[0]))
				throw new SymbolicAnalysisFailedException();
			if (!args[1].MatchLdLoc(stloc.Variable))
				throw new SymbolicAnalysisFailedException();
			// leave IL_0000
			if (!catchBlock.Instructions[3].MatchLeave((BlockContainer)moveNextFunction.Body))
				throw new SymbolicAnalysisFailedException();
		}

		bool IsBuilderFieldOnThis(ILInstruction inst)
		{
			IField field;
			ILInstruction target;
			if (builderType.IsReferenceType == true) {
				// ldfld(StateMachine::<>t__builder, ldloc(this))
				if (!inst.MatchLdFld(out target, out field))
					return false;
			} else {
				// ldflda(StateMachine::<>t__builder, ldloc(this))
				if (!inst.MatchLdFlda(out target, out field))
					return false;
			}
			return target.MatchLdThis() && field.MemberDefinition == builderField;
		}

		bool MatchStateAssignment(ILInstruction inst, out int newState)
		{
			// stfld(StateMachine::<>1__state, ldloc(this), ldc.i4(stateId))
			if (inst.MatchStFld(out var target, out var field, out var value)
				&& target.MatchLdThis()
				&& field.MemberDefinition == stateField
				&& value.MatchLdcI4(out newState))
			{
				return true;
			}
			newState = 0;
			return false;
		}
		#endregion

		#region InlineBodyOfMoveNext
		void InlineBodyOfMoveNext(ILFunction function)
		{
			context.Step("Inline body of MoveNext()", function);
			function.Body = mainTryCatch.TryBlock;
			function.AsyncReturnType = underlyingReturnType;
			moveNextFunction.Variables.Clear();
			moveNextFunction.ReleaseRef();
			foreach (var branch in function.Descendants.OfType<Branch>()) {
				if (branch.TargetBlock == setResultAndExitBlock) {
					if (resultVar != null)
						branch.ReplaceWith(new Return(new LdLoc(resultVar)) { ILRange = branch.ILRange });
					else
						branch.ReplaceWith(new Leave((BlockContainer)function.Body) { ILRange = branch.ILRange });
				}
			}
			foreach (var leave in function.Descendants.OfType<Leave>()) {
				if (leave.TargetContainer == moveNextFunction.Body) {
					leave.ReplaceWith(new InvalidBranch {
						Message = " leave MoveNext - await not detected correctly ",
						ILRange = leave.ILRange
					});
				}
			}
			function.Variables.AddRange(function.Descendants.OfType<IInstructionWithVariableOperand>().Select(inst => inst.Variable).Distinct());
			function.Variables.RemoveDead();
		}
		#endregion
	}
}
