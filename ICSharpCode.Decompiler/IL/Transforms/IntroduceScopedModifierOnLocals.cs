// Copyright (c) 2026 Sebastien Lebreton
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

#nullable enable

using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Infers the C# <c>scoped</c> modifier on locals whose initializer has a wider
	/// ref-safety context than a later assignment.
	/// </summary>
	/// <remarks>
	/// The modifier is erased from IL and PDBs, so recovery compares the ref-safe-context
	/// (for ref locals) or safe-context (for ref-struct locals) of the declaration initializer
	/// with each later store. The three contexts are ordered from narrowest to widest:
	/// current method, return-only, calling method. A narrower later store proves that the
	/// original declaration required <c>scoped</c>.
	/// </remarks>
	class IntroduceScopedModifierOnLocals : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.ScopedRef)
				return;

			foreach (var nestedFunction in function.Descendants.OfType<ILFunction>())
			{
				if (nestedFunction.Method is not {
					ParentModule: MetadataModule { UseUpdatedEscapeRules: true }
				} method)
				{
					continue;
				}

				new Analyzer(nestedFunction, method, context).Run();
			}
		}

		enum SafeContext : byte
		{
			CurrentMethod,
			ReturnOnly,
			CallingMethod,
		}

		readonly struct LocalScopes(
			SafeContext? refEscape,
			SafeContext? valEscape,
			bool isScoped,
			bool isScopedWithoutInitializer)
		{
			public SafeContext? RefEscape { get; } = refEscape;
			public SafeContext? ValEscape { get; } = valEscape;
			public bool IsScoped { get; } = isScoped;
			public bool IsScopedWithoutInitializer { get; } = isScopedWithoutInitializer;
		}

		sealed class Analyzer(ILFunction function, IMethod method, ILTransformContext context)
		{
			readonly Dictionary<ILVariable, StLoc[]> _stores = [];
			readonly Dictionary<ILVariable, List<StObj>> _fieldStores = [];
			readonly Dictionary<ILVariable, List<CallInstruction>> _receiverCalls = [];
			Dictionary<ILVariable, LocalScopes> _scopes = [];

			public void Run()
			{
				foreach (var variable in function.Variables)
				{
					variable.IsScoped = false;
					variable.IsScopedWithoutInitializer = false;
					if (variable.Kind is not (VariableKind.Local or VariableKind.StackSlot)
						|| !variable.Type.IsRefLikeOrAllowsRefLikeType()
						|| variable.UsesInitialValue)
					{
						continue;
					}

					var stores = variable.StoreInstructions.OfType<StLoc>().ToArray();
					if (stores.Length == 0 || stores.Any(store => store.Parent == null))
						continue;

					_stores.Add(variable, stores);
					_scopes.Add(variable, default);
				}

				foreach (var store in function.Descendants.OfType<StObj>())
				{
					if (!TryGetStorageRoot(store.Target, out var variable)
						|| !_stores.ContainsKey(variable))
					{
						continue;
					}

					if (!_fieldStores.TryGetValue(variable, out var stores))
					{
						stores = [];
						_fieldStores.Add(variable, stores);
					}
					stores.Add(store);
				}

				foreach (var call in function.Descendants.OfType<CallInstruction>())
				{
					bool isRefStructReceiver = call.IsInstanceCall
						&& call.Method.DeclaringType.IsRefLikeOrAllowsRefLikeType()
						&& !call.Method.ThisIsRefReadOnly;
					if (!(isRefStructReceiver || IsRefStructExtensionReceiver(call))
						|| call.Arguments.Count == 0
						|| !TryGetStorageRoot(call.Arguments[0], out var variable)
						|| !_stores.ContainsKey(variable))
					{
						continue;
					}

					if (!_receiverCalls.TryGetValue(variable, out var calls))
					{
						calls = [];
						_receiverCalls.Add(variable, calls);
					}
					calls.Add(call);
				}

				for (int iteration = 0; iteration <= _stores.Count * 3; iteration++)
				{
					context.CancellationToken.ThrowIfCancellationRequested();
					bool changed = false;
					var nextScopes = new Dictionary<ILVariable, LocalScopes>(_scopes);
					foreach (var pair in _stores)
					{
						ILVariable variable = pair.Key;
						StLoc[] stores = pair.Value;
						var initializer = GetFirstStore(stores);
						LocalScopes scopes = CalculateLocalScopes(variable, initializer, stores);
						LocalScopes previous = _scopes[variable];
						if (scopes.RefEscape != previous.RefEscape
							|| scopes.ValEscape != previous.ValEscape
							|| scopes.IsScoped != previous.IsScoped
							|| scopes.IsScopedWithoutInitializer != previous.IsScopedWithoutInitializer)
						{
							nextScopes[variable] = scopes;
							changed = true;
						}
					}

					_scopes = nextScopes;
					if (!changed)
						break;
				}

				foreach (var pair in _scopes)
				{
					pair.Key.IsScoped = pair.Value.IsScoped;
					pair.Key.IsScopedWithoutInitializer = pair.Value.IsScopedWithoutInitializer;
				}
			}

			LocalScopes CalculateLocalScopes(ILVariable variable, StLoc initializer, IReadOnlyList<StLoc> stores)
			{
				bool isRefLocal = variable.Type.Kind == TypeKind.ByReference;
				SafeContext? initializerRefEscape = isRefLocal
					? GetRefEscape(initializer.Value)
					: SafeContext.CurrentMethod;
				SafeContext? initializerValEscape = GetVariableValueType(variable).IsRefLikeOrAllowsRefLikeType()
					? GetValEscape(initializer.Value)
					: SafeContext.CallingMethod;

				if (initializerRefEscape == null || initializerValEscape == null)
					return default;

				bool requiresScoped = false;
				SafeContext initializerEscape = isRefLocal
					? initializerRefEscape.Value
					: initializerValEscape.Value;
				bool requiresScopedWithoutInitializer = initializerEscape < SafeContext.CallingMethod;
				foreach (var store in stores)
				{
					if (store == initializer)
						continue;

					SafeContext? storeEscape = isRefLocal
						? GetRefEscape(store.Value)
						: GetValEscape(store.Value);
					if (storeEscape is not { } scope)
						continue;

					if (scope < SafeContext.CallingMethod)
						requiresScopedWithoutInitializer = true;
					if (scope < initializerEscape)
					{
						requiresScoped = true;
					}
				}

				if (!requiresScoped && _fieldStores.TryGetValue(variable, out var fieldStores))
				{
					foreach (var store in fieldStores)
					{
						if (!initializer.IsBefore(store))
							continue;

						SafeContext? storeEscape = store.Type.Kind == TypeKind.ByReference
							? GetRefEscape(store.Value)
							: GetValEscape(store.Value);
						if (storeEscape is not { } scope)
							continue;

						if (scope < SafeContext.CallingMethod)
							requiresScopedWithoutInitializer = true;
						if (scope < initializerValEscape.Value)
						{
							requiresScoped = true;
						}
					}
				}

				if (!requiresScoped && _receiverCalls.TryGetValue(variable, out var receiverCalls))
				{
					foreach (var call in receiverCalls)
					{
						if (!initializer.IsBefore(call))
							continue;

						SafeContext? callEscape = GetInvocationEscapeToReceiver(call);
						if (callEscape is not { } scope)
							continue;

						if (scope < SafeContext.CallingMethod)
							requiresScopedWithoutInitializer = true;
						if (scope < initializerValEscape.Value)
						{
							requiresScoped = true;
						}
					}
				}

				if (!requiresScoped)
				{
					return new LocalScopes(
						initializerRefEscape,
						initializerValEscape,
						isScoped: false,
						isScopedWithoutInitializer: requiresScopedWithoutInitializer);
				}

				return isRefLocal
					? new LocalScopes(
						SafeContext.CurrentMethod,
						SafeContext.CallingMethod,
						isScoped: true,
						isScopedWithoutInitializer: true)
					: new LocalScopes(
						SafeContext.CurrentMethod,
						SafeContext.CurrentMethod,
						isScoped: true,
						isScopedWithoutInitializer: true);
			}

			/// <summary>
			/// Gets the declaration-initializer candidate: the first assignment in the final
			/// ILAst traversal order.
			/// </summary>
			static StLoc GetFirstStore(IReadOnlyList<StLoc> stores)
			{
				StLoc first = stores[0];
				for (int i = 1; i < stores.Count; i++)
				{
					if (stores[i].IsBefore(first))
						first = stores[i];
				}

				return first;
			}

			static bool IsRefStructExtensionReceiver(CallInstruction call)
			{
				if (!call.Method.IsExtensionMethod || call.Arguments.Count == 0)
					return false;

				IParameter? receiver = call.GetParameter(0);
				return receiver is { ReferenceKind: ReferenceKind.Ref }
					&& GetValueType(receiver.Type).IsRefLikeOrAllowsRefLikeType();
			}

			static bool TryGetStorageRoot(ILInstruction target, out ILVariable variable)
			{
				while (target is LdFlda fieldAddress)
				{
					target = fieldAddress.Target;
				}

				if (target is LdLoca ldloca)
				{
					variable = ldloca.Variable;
					return true;
				}

				variable = null!;
				return false;
			}

			SafeContext? GetRefEscape(ILInstruction value)
			{
				switch (value)
				{
					case LdLoc ldloc:
						return GetVariableScopes(ldloc.Variable).RefEscape;
					case LdLoca:
					case LocAlloc:
					case LocAllocSpan:
						return SafeContext.CurrentMethod;
					case LdsFlda:
					case LdElema:
						return SafeContext.CallingMethod;
					case LdElemaInlineArray inlineArray:
						return GetRefEscape(inlineArray.Array);
					case LdFlda fieldAddress:
						return GetFieldRefEscape(fieldAddress);
					case LdObjIfRef load:
						return GetRefEscape(load.Target);
					case CallInstruction call:
						return GetInvocationEscape(call, isRefEscape: true);
					case IfInstruction conditional:
						return Intersect(GetRefEscape(conditional.TrueInst), GetRefEscape(conditional.FalseInst));
					case NullCoalescingInstruction coalescing:
						return Intersect(GetRefEscape(coalescing.ValueInst), GetRefEscape(coalescing.FallbackInst));
					case Block block:
						return block.Kind == BlockKind.StackAllocInitializer
							? SafeContext.CurrentMethod
							: GetRefEscape(block.FinalInstruction);
					default:
						return null;
				}
			}

			SafeContext? GetValEscape(ILInstruction value)
			{
				switch (value)
				{
					case DefaultValue:
					case LdNull:
					case NewArr:
						return SafeContext.CallingMethod;
					case LocAlloc:
					case LocAllocSpan:
						return SafeContext.CurrentMethod;
					case LdLoc ldloc:
						return GetVariableScopes(ldloc.Variable).ValEscape;
					case LdLoca ldloca:
						return GetVariableScopes(ldloca.Variable).ValEscape;
					case LdsFlda:
					case LdElema:
						return SafeContext.CallingMethod;
					case LdElemaInlineArray inlineArray:
						return GetValEscape(inlineArray.Array);
					case LdFlda fieldAddress:
						return GetFieldValEscape(fieldAddress);
					case LdObj load:
						return GetValEscape(load.Target);
					case LdObjIfRef load:
						return GetValEscape(load.Target);
					case CallInstruction call:
						return GetInvocationEscape(call, isRefEscape: false);
					case IfInstruction conditional:
						return Intersect(GetValEscape(conditional.TrueInst), GetValEscape(conditional.FalseInst));
					case NullCoalescingInstruction coalescing:
						return Intersect(GetValEscape(coalescing.ValueInst), GetValEscape(coalescing.FallbackInst));
					case Block block:
						return block.Kind == BlockKind.StackAllocInitializer
							? SafeContext.CurrentMethod
							: GetValEscape(block.FinalInstruction);
					default:
						return null;
				}
			}

			SafeContext? GetFieldRefEscape(LdFlda fieldAddress)
			{
				if (fieldAddress.Field.IsStatic || fieldAddress.Field.DeclaringType.IsReferenceType == true)
					return SafeContext.CallingMethod;

				if (fieldAddress.Field.ReturnType.Kind == TypeKind.ByReference)
					return GetValEscape(fieldAddress.Target);

				return GetRefEscape(fieldAddress.Target);
			}

			SafeContext? GetFieldValEscape(LdFlda fieldAddress)
			{
				if (fieldAddress.Field.IsStatic
					|| !fieldAddress.Field.DeclaringType.IsRefLikeOrAllowsRefLikeType())
				{
					return SafeContext.CallingMethod;
				}

				return GetValEscape(fieldAddress.Target);
			}

			SafeContext? GetInvocationEscape(CallInstruction call, bool isRefEscape)
			{
				IType returnType = call is NewObj ? call.Method.DeclaringType : call.Method.ReturnType;
				bool hasRefLikeReturn = GetValueType(returnType).IsRefLikeOrAllowsRefLikeType();
				if (!isRefEscape && !hasRefLikeReturn)
					return SafeContext.CallingMethod;

				if (returnType.Kind == TypeKind.Unknown)
					return null;

				if (call.Method.ParentModule is not MetadataModule module)
					return null;

				return module.UseUpdatedEscapeRules
					? GetInvocationEscapeWithUpdatedRules(call, isRefEscape, returnType)
					: GetInvocationEscapeWithOldRules(call, isRefEscape);
			}

			SafeContext? GetInvocationEscapeWithUpdatedRules(
				CallInstruction call, bool isRefEscape, IType returnType)
			{
				SafeContext? result = SafeContext.CallingMethod;
				bool returnsRefToRefStruct = returnType is ByReferenceType { ElementType: var elementType }
					&& elementType.IsRefLikeOrAllowsRefLikeType();

				for (int i = 0; i < call.Arguments.Count; i++)
				{
					if (i == 0 && call.IsInstanceCall)
					{
						IType receiverType = call.Method.DeclaringType;
						if (receiverType.IsRefLikeOrAllowsRefLikeType())
						{
							result = AddInvocationEscape(
								result, GetValEscape(call.Arguments[i]), false,
								isRefEscape, returnsRefToRefStruct, isRefToRefStruct: true);
						}

						if (receiverType.IsReferenceType == false
							&& call.Method.GetThisParameterScope() != ScopedKind.ScopedRef)
						{
							result = AddInvocationEscape(
								result, GetRefEscape(call.Arguments[i]), true,
								isRefEscape, returnsRefToRefStruct,
								isRefToRefStruct: receiverType.IsRefLikeOrAllowsRefLikeType());
						}
						continue;
					}

					IParameter? parameter = call.GetParameter(i);
					if (parameter == null)
						return null;

					IType parameterType = GetValueType(parameter.Type);
					bool isRefToRefStruct = parameter.ReferenceKind != ReferenceKind.None
						&& parameterType.IsRefLikeOrAllowsRefLikeType();
					if (parameterType.IsRefLikeOrAllowsRefLikeType()
						&& parameter.ReferenceKind != ReferenceKind.Out
						&& GetParameterValEscape(parameter) != SafeContext.CurrentMethod)
					{
						result = AddInvocationEscape(
							result, GetValEscape(call.Arguments[i]), false,
							isRefEscape, returnsRefToRefStruct, isRefToRefStruct);
					}

					if (parameter.ReferenceKind != ReferenceKind.None
						&& GetParameterRefEscape(parameter) != SafeContext.CurrentMethod)
					{
						result = AddInvocationEscape(
							result, GetRefEscape(call.Arguments[i]), true,
							isRefEscape, returnsRefToRefStruct, isRefToRefStruct);
					}
				}

				return result;
			}

			SafeContext? GetInvocationEscapeWithOldRules(CallInstruction call, bool isRefEscape)
			{
				SafeContext? result = SafeContext.CallingMethod;
				for (int i = 0; i < call.Arguments.Count; i++)
				{
					if (i == 0 && call.IsInstanceCall)
					{
						if (call.Method.DeclaringType.IsRefLikeOrAllowsRefLikeType())
							result = Intersect(result, GetValEscape(call.Arguments[i]));
						continue;
					}

					IParameter? parameter = call.GetParameter(i);
					if (parameter == null)
						return null;

					if (!isRefEscape && GetValueType(parameter.Type).IsRefLikeOrAllowsRefLikeType())
						result = Intersect(result, GetValEscape(call.Arguments[i]));

					if (isRefEscape && parameter.ReferenceKind != ReferenceKind.None)
						result = Intersect(result, GetRefEscape(call.Arguments[i]));
				}

				return result;
			}

			SafeContext? GetInvocationEscapeToReceiver(CallInstruction call)
			{
				if (call.Method.ParentModule is not MetadataModule { UseUpdatedEscapeRules: true })
					return null;

				SafeContext? result = SafeContext.CallingMethod;
				for (int i = 1; i < call.Arguments.Count; i++)
				{
					IParameter? parameter = call.GetParameter(i);
					if (parameter == null)
						return null;

					IType parameterType = GetValueType(parameter.Type);
					if (parameterType.IsRefLikeOrAllowsRefLikeType()
						&& parameter.ReferenceKind != ReferenceKind.Out
						&& GetParameterValEscape(parameter) == SafeContext.CallingMethod)
					{
						result = Intersect(result, GetValEscape(call.Arguments[i]));
					}

					if (parameter.ReferenceKind != ReferenceKind.None
						&& GetParameterRefEscape(parameter) == SafeContext.CallingMethod)
					{
						result = Intersect(result, GetRefEscape(call.Arguments[i]));
					}
				}

				return result;
			}

			static SafeContext? AddInvocationEscape(
				SafeContext? current,
				SafeContext? argument,
				bool argumentIsRefEscape,
				bool invocationIsRefEscape,
				bool returnsRefToRefStruct,
				bool isRefToRefStruct)
			{
				if (returnsRefToRefStruct
					&& (!isRefToRefStruct || argumentIsRefEscape != invocationIsRefEscape))
				{
					return current;
				}

				return Intersect(current, argument);
			}

			LocalScopes GetVariableScopes(ILVariable variable)
			{
				if (_scopes.TryGetValue(variable, out var scopes))
					return scopes;

				if (variable.Kind != VariableKind.Parameter)
					return default;

				if (variable.Index is not int index)
					return default;

				if (index < 0)
					return GetThisParameterScopes();

				if (index >= method.Parameters.Count)
					return default;

				IParameter parameter = method.Parameters[index];
				return new LocalScopes(
					GetParameterRefEscape(parameter),
					GetParameterValEscape(parameter),
					isScoped: false,
					isScopedWithoutInitializer: false);
			}

			LocalScopes GetThisParameterScopes()
			{
				if (method.DeclaringTypeDefinition?.Kind != TypeKind.Struct)
				{
					return new LocalScopes(
						SafeContext.CallingMethod,
						SafeContext.CallingMethod,
						isScoped: false,
						isScopedWithoutInitializer: false);
				}

				SafeContext refEscape = method.GetThisParameterScope() == ScopedKind.ScopedRef
					? SafeContext.CurrentMethod
					: SafeContext.ReturnOnly;
				SafeContext valEscape = method.IsConstructor
					? SafeContext.ReturnOnly
					: SafeContext.CallingMethod;
				return new LocalScopes(
					refEscape,
					valEscape,
					isScoped: false,
					isScopedWithoutInitializer: false);
			}

			static SafeContext GetParameterRefEscape(IParameter parameter)
			{
				if (parameter.ReferenceKind == ReferenceKind.None)
					return SafeContext.CurrentMethod;
				if (parameter.GetEffectiveScope() == ScopedKind.ScopedRef)
					return SafeContext.CurrentMethod;

				bool useUpdatedEscapeRules = parameter.Owner?.ParentModule is MetadataModule {
					UseUpdatedEscapeRules: true
				};
				if (parameter.Lifetime.HasUnscopedRefAttribute
					&& useUpdatedEscapeRules
					&& parameter.ReferenceKind == ReferenceKind.Out)
				{
					return SafeContext.ReturnOnly;
				}
				if (parameter.Lifetime.HasUnscopedRefAttribute && useUpdatedEscapeRules)
					return SafeContext.CallingMethod;

				return SafeContext.ReturnOnly;
			}

			static SafeContext GetParameterValEscape(IParameter parameter)
			{
				if (parameter.GetEffectiveScope() == ScopedKind.ScopedValue)
					return SafeContext.CurrentMethod;
				if (parameter.ReferenceKind == ReferenceKind.Out
					&& parameter.Owner?.ParentModule is MetadataModule { UseUpdatedEscapeRules: true })
				{
					return SafeContext.ReturnOnly;
				}

				return SafeContext.CallingMethod;
			}

			static IType GetVariableValueType(ILVariable variable)
			{
				return GetValueType(variable.Type);
			}

			static IType GetValueType(IType type)
			{
				return type is ByReferenceType byReference ? byReference.ElementType : type;
			}

			static SafeContext? Intersect(SafeContext? left, SafeContext? right)
			{
				if (left == null || right == null)
					return null;
				return left < right ? left : right;
			}
		}
	}
}
