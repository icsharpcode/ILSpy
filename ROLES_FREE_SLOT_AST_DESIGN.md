# Roles-free slot-based C# AST -- complete design + implementation plan

Branch: `ast-source-generator-rebased`. Authoritative, self-contained design + step list for
moving the C# AST from roles + doubly-linked-list children to a slot-based model. Companion:
line-by-line pattern-matcher equivalence proofs in
`~/.claude/plans/rustling-bubbling-oasis-agent-ae8f0184ec5b19d68.md`.

## Context (why)

ILSpy's C# AST (`ICSharpCode.Decompiler/CSharp/Syntax/`) is NRefactory-derived: children live
in a per-node doubly-linked list, each tagged with a `Role` (9-bit index packed in `flags`).
Consequences: `GetChildByRole` is a linear scan; child order is insertion order, not schema
order (forcing hacks like the `InsertChildAfter` splice in
`TransformFieldAndConstructorInitializers.RemoveImplicitConstructor`); every property
read/write walks/relinks the list. The ILAst (`IL/Instructions/*`) already uses the better
model -- fixed *slots* with O(1) access and schema-ordered children -- generated from a
template. We are porting that model onto the C# AST, and using the move to shed other
NRefactory full-fidelity baggage the decompiler does not need.

Foundation already in place on this branch: a Roslyn incremental generator
(`ICSharpCode.Decompiler.Generators/DecompilerSyntaxTreeGenerator.cs`) that, from a
`[DecompilerAstNode]` attribute + reflection over each node's properties, generates the
mechanical members -- `AcceptVisitor` x3, the three `IAstVisitor` interfaces, `DoMatch`, null
nodes, and pattern placeholders. Storage is still role + linked-list (handwritten
`GetChildByRole` properties). This plan replaces that storage with slots and removes roles.

## Decisions locked with the user
- Roslyn incremental source generator (in place).
- Node declaration = **partial properties, source-order slots** (no explicit indices).
- **Delete `Role` entirely** (and `Role<T>`, `Roles`, `TokenRole`, `GetChildByRole`,
  `SetChildByRole`, `GetChildrenByRole`, `node.Role`); rewrite all consumers; accepted public-API
  break. **Keep `AstNode` and `AstNodeCollection<T>` names** -- reimplement the collection over slots
  rather than rename it (avoids a gratuitous API break and keeps the `AstNode`/`AstNodeCollection`/
  `AstType` name family; the IL parallel is implementation, not naming).
- **Drop token nodes** (`CSharpTokenNode`/`CSharpModifierToken` deleted); their meaning -> scalars.
- **Comments and preprocessor directives = node trivia** over a `Trivia` base
  (`Comment`/`PreProcessorDirective` derive from it). Stored in the existing annotation channel as a
  lazily-created `NodeTrivia` holder -- **no per-node fields**, so the ~99.99% no-trivia nodes cost
  nothing (`CloneAnnotations` copies it for free). `AddLeadingTrivia`/`AddTrailingTrivia`, printed
  before/after the node. `#define` (the only directive emitted; no bracketing) becomes leading trivia
  on the `SyntaxTree`. Doc comments = a leading `Documentation` slot (structural, separate).
- **The AST models structure + content, never formatting/whitespace.** Whitespace, indentation,
  spacing, blank lines, brace style are print-time, driven by `CSharpFormattingOptions` (independent
  of the AST); `Trivia` is content (comments+directives), not whitespace.
- **Drop `IFreezable`/`Freeze`/`IsFrozen`** -- dead NRefactory baggage (no consumer ever freezes;
  not even the null singletons).
- **Locations** = stored `StartLocation`/`EndLocation` (`TextLocation` each), assigned at print time;
  no `Span` tuple, no compute-from-children recursion.
- **Inheritance** = abstract contract on bases + per-leaf ordered slots, flat `sealed` dispatch.
- **Pattern matcher** = faithful port to list-based collection matching; slim `INode`; tests first.
- **Null-object -> NRT**: backing field uniformly nullable; *syntactically required* slot =
  non-null accessor (the static contract), optional slot = nullable accessor; delete the null
  objects. `CheckInvariant` enforces required-non-null at between-transform checkpoints.
- `Identifier` stays a node (single fixed slot). `CSharpSlotInfo` is positional. Annotations
  unchanged.

==========================================================================================
# PART I -- THE COMPLETE DESIGN
==========================================================================================

## 1. Native model

`AstNode` base (handwritten):
```csharp
public abstract class AstNode : AbstractAnnotatable, ICloneable, PatternMatching.INode {
    AstNode? parent;
    int childIndex = -1;            // flattened index within parent
    TextLocation startLocation, endLocation;   // set at print time
    // flags: per-class bits only (e.g. Identifier.IsVerbatim); no role index, no frozen bit

    public AstNode? Parent { get; }
    public int ChildIndex { get; }
    public CSharpSlotInfo? Slot => parent?.GetChildSlot(childIndex);     // replaces node.Role
    public TextLocation StartLocation { get; }   // returns startLocation (no recursion)
    public TextLocation EndLocation { get; }     // returns endLocation
    internal void SetLocations(TextLocation start, TextLocation end);    // print-time writer

    // generated per concrete leaf (flat, sealed):
    protected abstract int GetChildCount();
    protected abstract AstNode? GetChild(int index);   // null = empty optional slot
    protected abstract void SetChild(int index, AstNode? value);
    protected abstract CSharpSlotInfo GetChildSlot(int index);

    public ChildrenCollection Children { get; }        // document order (struct view)
    public void ReplaceWith(AstNode n) => parent!.SetChild(childIndex, n);
    // NextSibling/PrevSibling/FirstChild/LastChild computed from (parent, childIndex),
    // skipping empty optional slots.
    protected void SetChildNode<T>(ref T field, T value, int index) where T : AstNode?; // see below

    // Trivia (comments + preprocessor directives), Trivia = base of Comment/PreProcessorDirective.
    // NO per-node trivia fields: stored in the existing AbstractAnnotatable annotation channel as a
    // lazily-created NodeTrivia holder {leading, trailing}, so a node WITHOUT trivia (the ~99.99%
    // common case) pays nothing extra. CloneAnnotations copies it for free (Part 6/8).
    public void AddLeadingTrivia(Trivia t);   // get-or-create the NodeTrivia annotation, append
    public void AddTrailingTrivia(Trivia t);
    public IEnumerable<Trivia> LeadingTrivia { get; }   // empty (no alloc) when the annotation is absent
    public IEnumerable<Trivia> TrailingTrivia { get; }
}
```
`SetChildNode` (analog of IL's `SetChildInstruction`): validate value not parented / not in another
tree / type-compatible with slot; write field; set value.parent/childIndex; detach old. Strict-tree
invariant (old node detached immediately); no IL "stale positions". (No frozen check -- `IFreezable`
is dropped.)

`CSharpSlotInfo` (the `SlotInfo` analog; positional):
```csharp
public sealed class CSharpSlotInfo {
    public string Name { get; }        // "Left", "Body", "Parameters"
    public Type ChildType { get; }     // declared child type
    public bool IsOptional { get; }    // syntactically optional => nullable accessor (Part 11);
                                        // required => non-null accessor. Drives CheckInvariant.
    public bool IsCollection { get; }
}
```
Per-slot static instances generated per class (`BinaryOperatorExpression.LeftSlot`).
`node.Slot == X.YSlot` replaces `node.Role == X.YRole` (~43 consumer sites).

`AstNodeCollection<T>` (kept; reimplemented over slots, analog of `InstructionCollection<T>`):
List-backed, knows parent + slot, implements `IReadOnlyList<T>` and explicitly
`IReadOnlyList<INode>` (for the matcher). Insert/remove renumbers following children's `childIndex`.
Must preserve the actually-used surface: `Add`, `AddRange`, `Insert`, `InsertBefore(item,...)`/
`InsertAfter(item,...)` (by reference node), `Remove`, `Clear`, `ReplaceWith(IEnumerable<T>)`,
`MoveTo(targetCollection)` (cross-collection move -- corner C6), `Detach`, `AcceptVisitor`,
indexer/`Count`/`Contains`/`CopyTo`/`GetEnumerator`. `FirstOrNullObject`/`LastOrNullObject` become
`FirstOrDefault`/`LastOrDefault` (returning `null`) once null objects are gone (Part 11).

## 2. Node declaration (author writes)
```csharp
[DecompilerAstNode]
public partial class BinaryOperatorExpression : Expression {
    public partial Expression Left { get; set; }
    public partial Expression Right { get; set; }
    public BinaryOperatorType Operator { get; set; }   // operator text derives from this
}
```
The generator emits the other half: backing fields, property bodies
(`get => left; set => SetChildNode(ref left, value, <n>)`), the `CSharpSlotInfo` statics,
`GetChildCount/GetChild/SetChild/GetChildSlot`, `Clone`, and (as today) AcceptVisitor / DoMatch
/ null node (until Part 11) / pattern placeholder. Slot order = source declaration order
(Roslyn preserves member order).

Declaration mechanism (how the generator reads each slot):
- Single child slot: `public partial T Name { get; set; }` (T : AstNode). Read-only `{ get; }`
  for a printer-derived child (e.g. a computed token replacement) -- rare after the token drop.
- Collection slot: `public partial AstNodeCollection<T> Name { get; }` (get-only; generator owns the
  instance).
- **Optionality (Part 11 NRT): the declared nullability IS the slot's `IsOptional`** --
  `T Name` => *syntactically required* (non-null accessor = the static contract; getter returns
  `field!`), `T? Name` => optional (nullable accessor, `null` when empty). The **backing field is
  uniformly nullable either way** (so transforms may transiently empty any slot); only the accessor
  signature and `IsOptional` differ. Pre-NRT phases keep non-null accessors with null-object
  substitution; the nullability annotations are applied in Phase 8.
- Inherited contract members are re-declared `override partial` in the leaf at their ordered
  position (Part 3).

## 3. Inheritance composition
Base classes are the shared **contract** (for polymorphic consumption); each concrete node owns
its **implementation and order**. Base declares shared *child-slot* members `abstract`
(`EntityDeclaration.Attributes`); shared *scalars* (`Modifiers`, `SymbolKind`) stay concrete on
the base. Each leaf declares its full slot set as partial properties in document order,
`override`-implementing the contract members where they belong; the generator emits one flat
`sealed` dispatch per leaf. No base-delegation, no offset arithmetic (this is the divergence
from ILAst, which forbids split children via `sealed`; C# AST splits, so we use the
abstract-contract approach instead).
Confirmed by spike E4 (prototype `352d1eb3c`): base-delegation is not merely heavier but
*structurally incorrect* here -- for TypeMembers the inherited shared slots (`ReturnType`/
`NameToken`) are NON-CONTIGUOUS in source order (`NameToken` is emitted after the leaf-only
`PrivateImplementationType`, `CSharpOutputVisitor.cs:2577-2582`), so base-delegation, which
front-pins the inherited block to indices `0..BaseSlotCount-1`, cannot produce correct document
order. Flatten is required, not merely preferred; it is also smaller in total generated LOC (family
158 vs 171) and avoids a base virtual call + offset math on the hot child-access path. The one
flatten risk (a leaf silently mis-ordering/omitting a re-declared contract slot) is
generator-controllable: the generator owns ordering and emits a per-leaf contract-coverage
self-check.

## 4. Collections & flattened indexing
A node's flattened child index space = slots in declaration order. A **single slot is always
width-1** -- it occupies its flattened index even when empty (`GetChild` returns `null` at that
fixed position); only **collection** slots vary their width, occupying a contiguous run of the
collection's current length. So the index space is stable across the Phase 11 null->nullable
transition (an emptied single slot does not shift the offsets of later slots). Generated
`GetChild/SetChild/GetChildSlot` use range arithmetic (single slots at fixed offsets; offsets after
a collection shift by its count). `AstNodeCollection<T>` insert/remove renumbers following children.
This removes the `TransformFieldAndConstructorInitializers` reordering hack (a param added to
`TypeDeclaration.Parameters` lands at its schema position regardless of insertion time).
**Renumber scheme (decided by spike E2, prototype `dd58586b8`): plain eager renumber-on-mutation.**
It beats the linked list on every realistic op mix (positional Get/IndexOf on a list are O(1) vs the
list's O(n) scans); the O(n^2) front-insert-no-read worst case is not a decompiler workload and is
negligible in absolute terms (14 ms at 5000 elements vs a ~33 s CoreLib decompile). A lazy
dirty-watermark scheme (childIndex recomputed on first ordinal read, ~20 LOC, no API change) is the
documented fallback, applied only if profiling ever flags a specific oversized collection.
Open sub-point (from E4): when a leaf does not store a contract slot (e.g. `FieldDeclaration` has no
`PrivateImplementationType`/`NameToken` storage), decide whether that slot occupies a flattened index
or is contract-only-not-a-slot -- nail this down with the width-1 rule above.

## 5. Drop token nodes
The ~85 `CSharpTokenNode` child slots are removed (the printer's `WriteToken(Role, text)` is
handed the text; it never reads a token child). `CSharpTokenNode` and `CSharpModifierToken`
types are deleted. Meaning -> scalars (most already exist): operators ->
`BinaryOperatorType`/`AssignmentOperatorType`/`UnaryOperatorType`; accessor keyword ->
`AccessorKind`; member modifiers -> `Modifiers` flags (flip storage from `CSharpModifierToken`
children to a field); `async`/`await` presence -> `bool IsAsync` on `UsingStatement`/
`ForeachStatement`/`LambdaExpression`/`AnonymousMethodExpression`; attribute target -> an
`AttributeTarget` enum (complete value set: `None, Assembly, Module, Type, Method, Property, Field,
Event, Param, Return` -- note `Property` is used today, `TransformFieldAndConstructorInitializers`
sets `"property"`/`"field"`); punctuation -> pure output.
Verified by spike E3 (prototype `6e29b23ec`): the decompile-to-text printer never reads these token
children -- dropping `OperatorToken` and `ReturnToken`/`SemicolonToken` produced byte-identical
Pretty output (667/667). `VisitBinaryOperatorExpression` derives operator text from the `Operator`
enum; `ComposedType` reads `HasRefSpecifier`/`HasNullableSpecifier`/`PointerRank`, not its token
children; `InsertRequiredSpacesDecorator` branches on token TEXT, never the Role; and
`InsertParenthesesVisitor` is type/precedence-driven (zero paren drift). The genuine coupling is
`InsertMissingTokensDecorator` (rebuilds tokens + assigns Roles on the `CreateWriterThatSetsLocationsInAST`
location path, NOT in the Pretty path) and `InsertSpecialsDecorator`'s role-keyed trivia placement --
see Phase 4.

## 6. Comments, doc comments, directives
- Free comments = **node trivia**. Today a `Comment` attaches via `Roles.Comment` to almost any
  node (the `InsertSpecialsDecorator` interleaves at print) -- six receiver categories in use:
  block statement-list, an `Expression`, a non-block `Statement`, between `TypeDeclaration.Members`,
  an `AstType`, a `VariableInitializer`, an `Attribute`, an `Accessor`. Two comment-node types
  cannot reproduce that. Instead, every comment is re-homed onto **the node it annotates** as
  leading or trailing trivia:
  - Storage (memory-efficient -- comments/directives are the absolute outlier in decompiler output,
    so the no-trivia case must cost ZERO): NO per-node trivia fields. Trivia lives in the existing
    `AbstractAnnotatable` annotation channel (the one `object annotations` field every node already
    has) as a lazily-created `NodeTrivia` holder `{leading, trailing}`, added only when a node first
    gets trivia. A node without trivia (the ~99.99%) pays nothing -- no new field, no allocation. This
    keeps trivia outside the flattened child-index space (no matcher interaction, `CheckInvariant`'s
    child-range arithmetic stays clean).
  - API: `AddLeadingTrivia(Trivia)` / `AddTrailingTrivia(Trivia)` (the distinction is the method
    called, not a caller-set flag) get-or-create the `NodeTrivia` annotation and append; read-back via
    `LeadingTrivia`/`TrailingTrivia` (empty, no allocation, when absent).
  - Print: output visitor emits leading trivia at `StartNode` (before the node's children) and
    trailing at `EndNode` (after), insertion order per group -- replacing the `InsertSpecialsDecorator`
    `Roles.Comment` path. The "between members" case becomes a *leading* comment on the following
    member (re-homed at the call site).
  - `Clone` is automatic: trivia is an annotation, so the existing `CloneAnnotations()` copies it --
    no special trivia-copy code.
  - Guard: assert no site needs an *interior* comment between two real children where neither is the
    annotated node (the enumeration shows none -- all are first/last/leading-on-member).
- **Preprocessor directives are trivia too**, on the SAME channel. `Comment` and
  `PreProcessorDirective` both derive from a `Trivia` base, so leading/trailing trivia is a list of
  `Trivia`. `Trivia` is **content the printer must emit verbatim (comments + directives), NOT
  whitespace** -- whitespace/blank-lines are never modeled in the AST (Part 7). This unifies what
  were two mechanisms (trivia channel + "marker nodes in the collection") into one, and takes
  directives off the child-index space (a `#define` is file-level trivia, not a semantic child).
  `#define` becomes **leading trivia on the `SyntaxTree`** (top-of-file); `IntroduceUsingDeclarations`
  reads `syntaxTree.LeadingTrivia` instead of `Children`. `PreProcessorDirective` carries a
  `DirectiveKind` + payload. Grounded: only `#define` is emitted today (`CSharpDecompiler.cs:2263`)
  and there is no bracketing (region/if/endif/pragma never constructed); if any are added later they
  attach as leading/trailing trivia on the relevant node (the natural model).
- Doc comments: a leading `Documentation` slot on `EntityDeclaration` (a `DocumentationComment`
  node, multi-line text + span); rides with its member. Replaces today's fragile preceding
  `Comment(Documentation)` siblings (`AddXmlDocumentationTransform`). Kept separate from the trivia
  channel (it is structural -- a slot that rides with the member -- not free-floating trivia).

## 7. Source locations & the AST/formatting boundary
**Principle: the AST models structure + content, never formatting or whitespace.** Whitespace,
indentation, spacing, blank lines, and brace style are *print-time* concerns -- the output visitor
produces them when turning the AST into text, driven by `CSharpFormattingOptions`, which stays an
**independent** object passed to the printer, not state on the nodes. The AST carries no whitespace
nodes and no formatting flags. (This is why locations are assigned at print time, and why `Trivia`
is comments+directives only, not whitespace.) Consequence for the rewrite: do not migrate any
formatting concept into a node or slot; if a current node carries an incidental formatting hint, it
belongs in the printer + options, not the AST.

Locations: keep the existing `StartLocation`/`EndLocation` properties (`TextLocation` each), the primary
consumer being PDB generation (`DebugInfo.SequencePoint` is `StartLine/StartColumn/EndLine/EndColumn`;
`SequencePointBuilder` reads `node.StartLocation`/`EndLocation`) plus `AstNode`'s own tree-nav
(`GetNodeAt`/`GetNodesBetween`/`Contains`). **No `Span` tuple** -- keeping the two property names is
the facade that spares the ~33 readers any churn. Today these are *virtual, computed by recursion*
to the token leaves (`AstNode.cs:183-199`); the token drop removes those leaves, so the new model
**stores** the values in two fields and **drops the recursion**. Assigned by the existing mechanism:
the output visitor brackets every node with `StartNode`/`EndNode`, and `WrapInWriterThatSetsLocationsInAST`
reads the writer's tracked `Location` and calls the internal `SetLocations`. No computed fallback is
needed: locations are only meaningful post-format today too (token-leaf locations are `Empty` until
`InsertMissingTokensDecorator` runs), and all ~33 readers run post-format. Token-drop consequence:
`SequencePointBuilder`'s `{`/`}` breakpoints move from brace-token locations to the
`BlockStatement` start/end.

## 8. Annotations
Unchanged. `AstNode : AbstractAnnotatable` stays; annotations are orthogonal to the child model
(keyed by node identity, not slots), and core to the decompiler (124 `.Annotation<...>` reads,
incl. the `ILInstruction`-per-node mapping `SequencePointBuilder` uses). Generated `Clone` must
keep calling `CloneAnnotations()` -- which also copies comment/directive trivia for free, since
Part 6 stores the `NodeTrivia` holder as an annotation (same node-identity-keyed channel).

## 9. Role deletion & node.Slot
`Role`/`Role<T>`/`Roles`/`TokenRole`/`GetChildByRole`/`SetChildByRole`/`GetChildrenByRole` deleted.
`AstNodeCollection<T>` is **kept** (reimplemented over slots, Part 1) -- not deleted/renamed.
`node.Role` -> `node.Slot` (`CSharpSlotInfo`). Consumer rewrites: `GetChildByRole(R)` -> typed
property or `GetChild(i)` (~14 external; ~439 internal vanish into generated bodies);
`SetChildByRole(R, v)` -> typed property set; `node.Role == X.YRole` -> `node.Slot == X.YSlot` (~43);
`GetChildrenByRole` -> the typed `AstNodeCollection<T>` property; `AddChild(c, R)`/
`InsertChildBefore/After` -> `collection.Add`/`Insert`; `node.Role.IsValid(...)` -> slot/type check
on `CSharpSlotInfo.ChildType`.

## 10. Pattern-matching engine (faithful list-based port)
Role-based collection matching becomes list-vs-list (each `AstNodeCollection<T>` already *is* the
per-role sub-list). See Part II Phase 6 for the full design (slim `INode`; central matcher over
`IReadOnlyList<INode>`; `Repeat`/`OptionalNode` over index cursor; `AstNodeCollection<T>.DoMatch`;
generator `PatternPlaceholder` signature; `IdentifierExpressionBackreference` rewrite). Semantics
preserved 1:1; unit tests added first.

## 11. Null-object -> NRT
**Backing field is uniformly nullable**; the accessor encodes the contract. Optional slot ->
nullable accessor (`Expression? Body`, returns the field, `null` when empty); *syntactically
required* slot -> non-null accessor (`Expression Left`, getter returns `field!`). The nullable field
lets a transform transiently empty any slot ("optional during a transform"); the non-null accessor
is the static contract that a finished tree has it filled, enforced by `CheckInvariant` at
between-transform checkpoints (Part 13). Reading a transiently-empty required child throws at the
bug site; an unrefilled one is caught at the next checkpoint -- so no special `Detach`/`Remove`
rule is needed.
Classification = *syntactically required* (the C# grammar is the oracle; mechanical proxy: a slot is
required iff `CSharpOutputVisitor` dereferences it unconditionally). `default-to-optional` only as a
tie-breaker. Drop the 18 generated null-node classes, `AstNode.IsNull`, and the `.Null` singletons.
Consumer sweep: ~91 `.IsNull` -> `== null` (dead on required slots), ~99 `.Null` -> `null`; output
visitor + `DepthFirstAstVisitor` drop `VisitNullNode` and null-check before recursing into optional
children. Simplifies the matcher: `INode.IsNull` is then removed (empty = `null`).
Verify before locking: no path emits a finished tree with a required slot empty (the decompiler
emits an `ErrorExpression`/comment instead) -- a false-required there would throw where today it
printed nothing.

## 12. What the generator emits (end state)
Per `[DecompilerAstNode]` node, from its partial-property declarations: backing fields; slot
property bodies; `CSharpSlotInfo` statics; `GetChildCount/GetChild/SetChild/GetChildSlot` (flat,
sealed, range arithmetic for collections); `Clone` (per-slot + `CloneAnnotations`); `AcceptVisitor`
x3; the `IAstVisitor` interfaces; `DoMatch`; pattern placeholder (`DoMatchCollection` list-based).
For required slots the getter returns `field!` (the static non-null contract over a nullable field);
optional slots return the nullable field. No null-node classes after Part 11. Optionally generate
`SymbolKind` from the model (replacing 14 handwritten overrides) -- deferred nicety.

## 13. AST invariants (CheckInvariant)
The slot model makes structural invariants *checkable* (the role + linked-list model could not
validate a `childIndex` because there was none). Add a DEBUG-only recursive `CheckInvariant` on
`AstNode`, the analog of `ILInstruction.CheckInvariant`, asserting per child:
- `child.Parent == this` AND `this.GetChild(child.ChildIndex) == child` (parent/index round-trip);
- child's runtime type is assignable to `GetChildSlot(child.ChildIndex).ChildType` (slot typing);
- collection slots: elements occupy the slot's contiguous index range, in order, with matching
  `childIndex`;
- strict tree: every node reachable exactly once (no shared subtrees);
- (post-NRT, Part 11) every slot with `CSharpSlotInfo.IsOptional == false` is non-null -- this is
  the enforcement of "syntactically required" at the checkpoint (transforms may transiently empty it
  between checkpoints; here it must be filled).
Recurse into all children. Wire it into the C# transform pipeline to run after every
`IAstTransform` in DEBUG (mirroring the IL pipeline's per-transform `CheckInvariant`). This is
both a permanent correctness guard for transforms ("we want every transform to behave") and a
migration accelerator -- a transform or a mis-generated slot that corrupts the tree fails at the
exact transform, not as a downstream decompilation diff. Directly guards corners C1/C6.

==========================================================================================
# PART II -- IMPLEMENTATION STEPS
==========================================================================================

Guiding constraints: every phase ends green on the decompiler suites (`dotnet test --solution
ILSpy.sln --report-trx`, prefixed `OPENSSL_ENABLE_SHA1_SIGNATURES=1`); the round-trip + Pretty
suites are the real behavioral gate for the transforms/printer. The base-storage flip is the one
unavoidably coordinated change; everything else is staged.

**EXECUTION ORDER (revised 2026-06-13): Phase 4 -> Phase 5 -> Phase 3c.** The storage flip (3c) was
originally sequenced before token-drop (4) and comments-to-trivia (5), but at the flip the still-live
positional non-slot children (`Roles.Comment` / `Roles.Comma` / `Roles.PreProcessorDirective`,
interleaved in document order by `InsertSpecialsDecorator` and attached to many no-collection nodes:
CastExpression, ExpressionStatement, VariableInitializer, Attribute, AstType, ErrorExpression) have
nowhere to live in a pure-slot tree. Landing Phase 4 + Phase 5 first removes every non-slot child, so
the 3c flip becomes a clean pure-slot conversion with no interim sidecar. Trade-off accepted: the
O(1)/perf payoff is deferred to 3c, and Phase 4's `InsertMissingTokensDecorator` location-path gating
work is front-loaded. Phases 6-9 follow 3c as before.

## Phase 0 -- Prerequisite (DONE)
Reconciled `ast-source-generator` onto master; generator builds and runs; visitor/DoMatch/null/
pattern generation in place. (Branch `ast-source-generator-rebased`, pushed.)

## Phase S -- De-risking spikes (DONE)
Ran E1-E4 in isolated worktrees (off the warmup commit `76a965ae2`); all four returned
high-confidence verdicts. Prototype commits kept for reference (not landed):
- **E1 migration mechanic -> BRIDGE** (commit `3050aee76`): slot accessors over the linked list,
  family-by-family conversion, single later storage flip. Pretty output byte-identical to baseline
  (667/667); DEBUG slot-order==document-order invariant held across all 667 fixtures. Prerequisites
  surfaced: `Role.NullObjectUntyped`; collection slots need a separate design (ride to the flip on
  `AstNodeCollection`); bridge gives the API but not O(1) perf (that arrives at the flip).
- **E2 collection perf -> PLAIN EAGER renumber** (commit `dd58586b8`): the O(n^2) trap is
  pathological (front-insert with no ordinal reads) only; on realistic op mixes eager List-backed
  beats the linked list (whose positional Get/IndexOf are O(n)). Lazy dirty-watermark kept as a
  documented ~20-LOC fallback. No smarter scheme up front.
- **E3 token-drop -> LOW blast radius** (commit `6e29b23ec`): dropping token slots is byte-neutral
  for the decompile-to-text printer (already scalar-driven). Real risk re-pointed to
  `InsertMissingTokensDecorator` on the AST-with-locations (GUI) path, which Pretty does NOT cover.
- **E4 inheritance -> KEEP FLATTEN** (commit `352d1eb3c`): base-delegation is structurally
  incorrect -- inherited slots are non-contiguous in source order (`NameToken` after
  `PrivateImplementationType`), which a front-pinned base block cannot represent. Flatten is also
  smaller overall (family 158 vs 171 LOC).
CheckInvariant (Phase 1) is brought up first so the spikes ran under it (the E1 invariant IS the
Phase 3a slot-order guard).

## Phase 1 -- Slot base infrastructure + CheckInvariant (handwritten)
Reimplement `AstNodeCollection<T>` over slots (List-backed, parent+slot, renumbering,
`IReadOnlyList<T>`+explicit `IReadOnlyList<INode>`; preserve the used surface per Part I.1); add
`CSharpSlotInfo`. Add to `AstNode`: `childIndex`, `Slot`, stored `StartLocation`/`EndLocation` +
internal `SetLocations`, abstract `GetChildCount/GetChild/SetChild/GetChildSlot`, `Children` struct
view, `SetChildNode<T>`, computed sibling nav, slot-based `Clone`, the `AddLeading/TrailingTrivia`
side-channel. Drop `IFreezable`/`Freeze`/`IsFrozen`. Implement `CheckInvariant`
(Part I.13) and wire it into the C# `IAstTransform` pipeline (the transform runner in
`CSharpDecompiler`) under DEBUG, so it guards every later phase and the spikes. Keep the
linked-list API temporarily for unconverted nodes (bridge), or build on a sub-branch -- see
Phase 3 / spike E1.
Gate: compiles; CheckInvariant runs green on current (pre-migration) output; no node migrated yet.

## Phase 2 -- Generator: slot storage emission
Extend the generator to emit, from partial-property declarations: backing fields, property
bodies, `CSharpSlotInfo` statics, and `GetChildCount/GetChild/SetChild/GetChildSlot` with the
flat per-leaf dispatch + collection range arithmetic; adapt `Clone` to per-slot. Add a
`SLOTAST` diagnostic for malformed declarations (mixed ordering, unsupported slot type).
Gate: generator unit-builds; emits correct storage for a single pilot node.

## Phase 3 -- Family-by-family node migration to slots
Convert node declarations from handwritten `GetChildByRole` properties to partial properties,
one family per step, regenerating storage. Order (smallest/most-isolated first):
1. Expressions (leaf-heavy, few inter-family deps).
2. Statements.
3. TypeMembers (EntityDeclaration contract + leaves; exercises inheritance composition).
4. GeneralScope + types (AstType hierarchy, NamespaceDeclaration, etc.).
5. Remaining (SyntaxTree, VariableDesignation, etc.).
Mechanic: **BRIDGE** (decided by spike E1, prototype `3050aee76`; the coordinated flip is a
non-reviewable big-bang because all nodes share `AstNode`'s firstChild/lastChild traversal and
`AstNodeCollection` is hardwired to it). Three sub-steps:
- **3a.** Land the `AstNode` slot layer (`SlotCount`/`GetSlotRole(i)`/`IsCollectionSlot(i)`,
  `GetChild(i)`/`SetChild(i)` over the linked list) PLUS the DEBUG slot-order==document-order
  invariant FIRST, as its own commit, wired into `CSharpOutputVisitor.StartNode` so it runs the
  whole suite on every PR. Prerequisite: add `Role.NullObjectUntyped` (`Role<T>` invariance blocks
  `(Role<AstNode>)role`).
- **3b.** Convert node declarations to slot accessors family-by-family (Expressions -> Statements ->
  TypeMembers -> GeneralScope+types -> remaining). Each commit is small, mechanical, individually
  revertible, and gated by the assert + byte-identical Pretty suite. Extend the slot-order assert to
  optional/reordered-child families (modifiers, attributes, the EntityDeclaration interleave from E4)
  as they convert -- it is proven only for simple Expressions so far.
- **3c.** Flip storage to real backing fields as ONE coordinated change once all families route
  through `GetChild`/`SetChild` -- linked list, `AstNodeCollection`, `Clone`, `StartLocation`/
  `EndLocation`, and sibling nav flip together. **Collection slots** are not covered by the
  single-slot bridge accessors (they stay on `GetChildrenByRole`/`AstNodeCollection` and move here) --
  they need their own design step. The bridge yields the slot API but NOT O(1) access; perf only
  arrives at 3c.
Bridge is output-neutral in practice: E1 proved byte-identical Pretty output across all 667
decompiled fixtures with the slot-order assert never tripping. The one known insertion-vs-schema
divergence is the `TransformFieldAndConstructorInitializers` reordering hack; the assert is the tool
that catches any other.
Gate per family: full decompiler suite green; the slot-order==document-order assert holds for the
converted family.

## Phase 4 -- Drop tokens
Convert token-carried meaning to scalars (modifiers field, `AccessorKind`, `IsAsync`,
`AttributeTarget`, operator enums already present); remove token child slots from declarations;
delete `CSharpTokenNode`/`CSharpModifierToken`; move `SequencePointBuilder` brace points to
`BlockStatement` start/end.
**Effort re-pointed by spike E3** (the decompile-to-text printer is already scalar-driven, so the
spacing/parens rework the design feared is near-zero -- E3 dropped `OperatorToken` +
`ReturnToken`/`SemicolonToken` with byte-identical Pretty output, and `InsertRequiredSpacesDecorator`
branches on token TEXT not Role). The real, Pretty-INVISIBLE coupling is the gating work:
- **(1, gating)** `InsertMissingTokensDecorator` reconstructs `CSharpTokenNode`/`CSharpModifierToken`
  from the print stream and assigns Roles on the `CreateWriterThatSetsLocationsInAST` path (GUI
  navigation / source maps), NOT exercised by Pretty. Build regression coverage for that path and run
  it against the token-slot drop BEFORE committing the drop.
- **(2)** `InsertSpecialsDecorator` role-keyed comment/preprocessor placement (low-risk: missing role
  just doesn't advance) -- add comment/directive-bearing fixtures.
- `InsertParenthesesVisitor` / `InsertRequiredSpacesDecorator` / spacing: verified inert by E3, no
  change expected.
Gate: Pretty + round-trip suites green; PDB sequence-point tests green; AND the
AST-with-locations / token-navigation path green (Pretty alone does not exercise the at-risk
component).

## Phase 5 -- Comments + directives (trivia), doc comments
Add the `Trivia` base (`Comment : Trivia`, `PreProcessorDirective : Trivia`) and the
`AddLeading/TrailingTrivia` side-channel on `AstNode` (Part I.6); add `DocumentationComment` + the
`Documentation` slot on `EntityDeclaration`. Migrate the 6 comment-receiver categories to
`AddLeadingTrivia`/`AddTrailingTrivia` on the annotated node (re-home the between-members comments
onto the following member); add the interior-comment assert. Move `#define` to leading trivia on the
`SyntaxTree` and rewrite `IntroduceUsingDeclarations` to read `syntaxTree.LeadingTrivia`. Rewrite
`AddXmlDocumentationTransform` to set the `Documentation` slot; update the printer to emit leading
trivia at `StartNode` / trailing at `EndNode` + the doc node; remove the old `Comment`/
`Roles.Comment` interleaving + `InsertSpecialsDecorator` trivia path.
Gate: Pretty suite green (comment/doc/define output identical).

## Phase 6 -- Pattern-matching engine port (TESTS FIRST)
6.1 Add `ICSharpCode.Decompiler.Tests/PatternMatching/PatternMatchingTests.cs` (~18 NUnit cases,
    substrate-agnostic; green on the current engine first). Cases: single-node + captures,
    AnyNode/OrNull, NamedNode retrieval, Choice checkpoint-restore, Repeat min/max/greedy/
    backtrack-to-satisfy-trailing-fixed, Optional present/absent/at-end, Backreference,
    IdentifierExpressionBackreference (+ TypeArguments rejection), empty collections, nesting, and
    real `forPattern` + trimmed `forOnArrayPattern` from PatternStatementTransform.
6.2 Slim `INode`: keep `IsNull`+`DoMatch`; drop `Role`/`FirstChild`/`NextSibling`; new hook
    `DoMatchCollection(IReadOnlyList<INode> other, int pos, Match, BacktrackingInfo)`.
6.3 `Pattern.cs`: `PossibleMatch.NextOther` -> `int NextOtherIndex`; port the static core to
    list+index (delete role-skip loops; `null` -> `pos>=Count`; `NextSibling` -> `+1`; preserve
    the `stack.Count==patternStack.Count` invariant); new default instance hook
    `DoMatch(pos<Count? other[pos] : null, match)`; drop dead INode explicit members.
6.4 `Repeat`/`OptionalNode` `DoMatchCollection` over index cursor (`DoMatch` overloads unchanged).
6.5 `AstNodeCollection<T>.DoMatch(other, match) => Pattern.DoMatchCollection(this, other, match)`
    via the explicit `IReadOnlyList<INode>` view.
6.6 `AstNode` INode impl: new list hook; remove `NextSibling`/`FirstChild`/old `DoMatchCollection`.
6.7 Generator: `PatternPlaceholder.DoMatchCollection` new signature (member-type detection stays
    `"AstNodeCollection`1"` -- the type name is unchanged).
6.8 `IdentifierExpressionBackreference`: replace `referenced.GetChildByRole(Roles.Identifier).Name`
    with `((IdentifierExpression)referenced).Identifier` guarded by `is`.
Gate: 6.1 suite green after each sub-step; full decompiler suite green (transforms unchanged at
the API level).

## Phase 7 -- Delete Role + consumer rewrites
Delete `Role`/`Role<T>`/`Roles`/`TokenRole`/`GetChildByRole`/`SetChildByRole`/
`GetChildrenByRole`/`node.Role`/`AddChild(role)`/`InsertChildBefore/After` (KEEP
`AstNodeCollection<T>` -- reimplemented over slots, not deleted). Rewrite the ~14
external `GetChildByRole`, ~43 `.Role` reads (-> `.Slot`), and the `AddChild`/`InsertChild*`/
`MoveTo` sites (-> collection ops; delete the reordering hack). Remove the role bridge from
Phase 1/3.
Gate: compiles with no role references; full suite green; public-API diff reviewed (this is the
intended break).

## Phase 8 -- Null-object -> NRT
**Required/optional classification is NEW information** (the uniformly-null-object model records no
such distinction), but the rule is objective: a slot is *syntactically required* iff the C# grammar
cannot omit it -- mechanical proxy: `CSharpOutputVisitor` dereferences it unconditionally (no
`if (x != null)` guard / alternate-token branch). Read the printer rather than judging per slot;
`default-to-optional` only as a tie-breaker. Model: **backing field uniformly nullable**; required
slot -> non-null accessor (`field!`, the static contract), optional -> nullable accessor; transforms
may transiently empty any slot, and `CheckInvariant` enforces required-non-null at the
between-transform checkpoints (so no special `Detach`/`Remove` rule).
Generator: emit required vs optional accessors from the declared nullability (= `IsOptional`); stop
emitting null-node classes + `hasNullNode`; `DoMatch`/matcher use `== null`. Delete the 18 `.Null`
singletons + `AstNode.IsNull` + `INode.IsNull` + `VisitNullNode`. Consumer sweep: ~91 `.IsNull` ->
`== null` (dead on required slots), ~99 `.Null` -> `null`/empty; output visitor +
`DepthFirstAstVisitor` null-guards.
**Classification rule, refined (verified):** "printer dereferences the slot unconditionally" is
*necessary but not sufficient* for required -- the null-object's silent `VisitNullNode` no-op can
mask a legitimately-empty slot. **Also check construction sites.** Confirmed landmine:
`IndexerExpression.Target` is visited unconditionally (`CSharpOutputVisitor.cs:1012`) yet is
intentionally empty in finished trees -- object initializers `new C { [i] = v }`
(`CallBuilder.cs:1690`) and property-pattern index paths (`ExpressionBuilder.cs:3585`). So
`Target` is **optional** (`Expression? Target`), and `VisitIndexerExpression` must guard it (or
`Target?.AcceptVisitor`) once the null-object no-op is gone. Verified safe: `ErrorExpression` is
always a complete top-level node (never a stand-in child, so error paths never empty a required
slot); the truly-required slots (binary/assignment/cast/unary/condition/body) are always filled;
the printer's explicit `IsNull` guards already mark the known optionals (`ReturnStatement.Expression`,
`CaseLabel.Expression`, `ThrowStatement.Expression`, `IfElseStatement.FalseStatement`).
`IndexerExpression.Target` is the **sole** null-`Target` case (verified: of ~17 construction sites
only `CallBuilder.cs:1690` and `ExpressionBuilder.cs:3585` pass null; `MemberReferenceExpression`/
`InvocationExpression` `Target` are never null-constructed -- `this.A` is an MRE with a
`ThisReferenceExpression` target, bare `A` is an `IdentifierExpression`, so there is no implicit-`this`
null-target). It exists only because an implicit-target indexer (dictionary/object initializer, and
the property-pattern path) is *modeled* as `IndexerExpression` with a null target. Phase-8 choice:
(A) classify `IndexerExpression.Target` optional and guard it in `VisitIndexerExpression` (cheap,
honest); (B) give the implicit-target indexer its own node and keep `Target` required (removes the
wart, adds a node type). Recommend (A).
Gate: builds under `#nullable enable` with no new AST nullable warnings; full suite green;
spot-check optional-child output (abstract methods, expression-bodied members, non-await
using/foreach).

## Phase 9 -- Cleanup & niceties
**Decided, deferred (generator-internal, zero public-API impact): bit-pack declared scalars into
`flags`** (ILAst-style). The generator backs each declared `bool` (1 bit) and *narrow non-`[Flags]`
enum* (`BinaryOperatorType`/`UnaryOperatorType`/`AssignmentOperatorType` ~5-6 bits, `AccessorKind`,
`VarianceModifier`, ...) with a mask+shift over `flags` instead of a field, allocating bit ranges
per class above the base's used bits (generalize `AstNodeFlagsUsedBits` to a generated per-class
`const`), with an overflow diagnostic if a node exceeds the word. **Carve-out: `[Flags]` enums stay
their own field** -- `Modifiers` is ~19 flag bits, already a bitset, and would overflow the word
(esp. while the 9-bit role index is still present pre-Phase-7). The accessor signature is unchanged
(`public bool IsAsync { get; set; }`), so this is a pure storage swap doable any time after Phase 2.
Delete dead decorators/visitor methods; optionally generate `SymbolKind` from the model; re-add
the `Generators` project to `ILSpy.sln` (already done); regenerate lock files; final
public-API-diff pass; update docs.

## Files (representative, not exhaustive)
- Base/infra: `CSharp/Syntax/AstNode.cs`, new `CSharpSlotInfo.cs`, `AstNodeCollection.cs`
  (reimplemented over slots, not deleted), delete `Role.cs`/`Roles.cs`/`TokenRole.cs`/
  `CSharpTokenNode.cs`/`CSharpModifierToken.cs`.
- Nodes: every file under `CSharp/Syntax/Expressions|Statements|TypeMembers|GeneralScope` +
  the type/`*AstType` files (partial-property conversion, generator-assisted).
- Generator: `ICSharpCode.Decompiler.Generators/DecompilerSyntaxTreeGenerator.cs`.
- Matcher: `CSharp/Syntax/PatternMatching/*` (per Phase 6).
- Printer/debug: `CSharp/OutputVisitor/CSharpOutputVisitor.cs`, `TextWriterTokenWriter.cs`,
  `InsertMissingTokensDecorator.cs`, `InsertParenthesesVisitor.cs`, `InsertSpecialsDecorator.cs`,
  `SequencePointBuilder.cs`, `DepthFirstAstVisitor.cs`.
- Transforms: `AddXmlDocumentationTransform.cs`, `IntroduceUsingDeclarations.cs`,
  `TransformFieldAndConstructorInitializers.cs` (drop reordering hack), `PatternStatementTransform.cs`
  (no API change), plus the ~57 role/`GetChildByRole` consumer sites across `CSharp/`.
- Tests: new `ICSharpCode.Decompiler.Tests/PatternMatching/PatternMatchingTests.cs`.

## Verification (end to end)
- Per phase: full decompiler suite green (`dotnet test --solution ILSpy.sln --report-trx`,
  `OPENSSL_ENABLE_SHA1_SIGNATURES=1`); round-trip + Pretty are the behavioral gate; PDB
  sequence-point tests guard locations after the token drop.
- Pattern engine: the new unit suite (Phase 6.1) green throughout.
- Final: public-API-diff review (intended break: roles/tokens/null-objects removed); build clean
  under `#nullable enable`; ilspycmd smoke decompile of a few representative assemblies.

## Weird corners (honest risk list) & de-risking spikes

C1. **The base-storage flip is not cleanly incremental** (highest risk). All ~110 nodes share
   `AstNode`, so storage cannot be half-slots/half-linked-list at runtime. The "bridge" (slot
   accessors implemented over the linked list, then flip to fields) is awkward precisely where
   the models differ most: `childIndex` is not naturally stored in a linked list, and sibling
   nav is computed differently. The "coordinated flip" avoids the awkward bridge but is a big-
   bang change. This single choice shapes the entire migration. -> SPIKE E1.
   RESOLVED (E1, `3050aee76`): BRIDGE. Byte-neutral on the tested families (667/667 Pretty
   identical, slot-order assert never tripped). Sibling nav stays on the linked list during the
   bridge; `childIndex`/O(1) access + `AstNodeCollection`/`Clone`/sibling-nav all flip together at
   3c. Prerequisite `Role.NullObjectUntyped`.
C2. **Collection insert/remove renumbering is potentially O(n^2)** where the linked list was
   O(1). `AstNodeCollection<T>` renumbers following children's `childIndex` on every insert/remove.
   Large collections (a `SyntaxTree`'s members, a big `BlockStatement`) under insert-heavy
   transforms could regress decompile time. IL's `InstructionCollection` has the same model and
   is fine, but C# AST collections can be larger. -> SPIKE E2 (benchmark).
   RESOLVED (E2, `dd58586b8`) -- and the framing was inverted: the linked list is the *slower*
   model for realistic transforms (O(n) positional Get/IndexOf); plain eager renumber wins on every
   real op mix. The O(n^2) trap is front-insert-with-no-reads only and negligible in absolute terms.
   Decision: plain eager renumber, lazy dirty-watermark as a documented fallback.
C3. **Printer fidelity after the token drop.** The spacing/parenthesis machinery
   (`InsertRequiredSpacesDecorator`, `InsertParenthesesVisitor`, `InsertMissingTokensDecorator`)
   may branch on token roles/presence; dropping token nodes could shift output whitespace or
   parens subtly. Pretty tests are exact-text, so any drift fails loudly -- good, but the blast
   radius is unknown until tried. -> SPIKE E3 (drop tokens on a slice, diff Pretty).
   RESOLVED (E3, `6e29b23ec`): the spacing/parens concern is inert -- the printer is already
   scalar-driven; dropping tokens was byte-neutral. The residual risk MOVED to
   `InsertMissingTokensDecorator` on the AST-with-locations (GUI) path, which Pretty does not
   exercise -- that is now the Phase 4 gating item.
C4. **Inheritance re-declaration.** The chosen flatten model makes each of ~30 declarations
   re-declare inherited contract slots (`override partial ... Attributes`). Repetitive, and a
   mistake (wrong order/omission) is a silent document-order bug. The base-delegation
   alternative trades this for offset arithmetic + non-sealed dispatch. -> SPIKE E4 (compare on
   TypeMembers).
   RESOLVED (E4, `352d1eb3c`): KEEP FLATTEN. Base-delegation is structurally incorrect here
   (inherited slots are non-contiguous in source order -- `NameToken` after
   `PrivateImplementationType` -- which a front-pinned base block cannot represent). The flatten
   mis-order risk is generator-controlled (owns ordering + per-leaf contract-coverage self-check).
C5. **Pattern placeholders in collection-initializer syntax** (`Statements = { new Repeat(...) }`)
   must implicitly convert `Pattern -> Statement -> PatternPlaceholder` and `Add` to a
   `AstNodeCollection<T>`. Lower risk -- the Phase 6 tests-first approach exercises it directly.
C6. **Transform idioms**: `Detach`/`MoveTo`/`ReplaceWith`/cross-collection moves get subtle
   slot-model rewrites; the renumbering must stay correct under move-then-readd. Covered by the
   suite, but a source of fiddly bugs.
C7. **NRT required/optional classification** (Phase 8): the current null-object model records no
   such distinction. RESOLVED via R7 -- the rule is *syntactically required* (the C# grammar is the
   objective oracle; the mechanical proxy is "`CSharpOutputVisitor` dereferences the slot
   unconditionally" -- necessary but not sufficient: also check construction sites, since the
   null-object no-op masks intentionally-empty slots, e.g. `IndexerExpression.Target`). Required ->
   non-null accessor (the static contract); optional -> `T?`. Backing
   fields are uniformly nullable (transform-friendly); `CheckInvariant` enforces required-non-null at
   between-transform checkpoints. default-to-optional only as tie-breaker. Lowered from "main NRT
   risk" to mechanical work.

De-risking spikes (each isolated, parallel-worktree-friendly):
- **E1 -- migration mechanic**: implement the Expressions family both ways (bridge over the
  linked list vs coordinated flip to fields) and compare pain + reviewability. Decides Phase 3.
- **E2 -- collection perf**: micro-benchmark `AstNodeCollection<T>` insert/remove vs the linked list
  on a large method/file; decompile a big assembly and compare wall-clock. Decides whether
  renumbering needs a smarter scheme (e.g. lazy/index-on-demand).
- **E3 -- token-drop printer probe**: drop tokens on one expression + one statement, run the
  Pretty suite, inspect the diff to size the spacing/parens rework.
- **E4 -- inheritance shape**: flatten vs base-delegation on TypeMembers; compare generated-code
  volume + ergonomics; confirm or revisit the flatten decision.

## Open items
- Phase 3 mechanic: RESOLVED to BRIDGE by spike E1 (see Phase 3 / Phase S).
- Collection-renumber scheme: RESOLVED to plain eager + documented lazy fallback by spike E2.
- `SymbolKind` generation (Phase 9) is optional; flags bit-packing (Phase 9) decided + deferred.
- Remaining within-plan probes (not decisions): extend the slot-order assert to optional/reordered-
  child families before the 3c flip (E1/N2); add regression coverage for the
  `InsertMissingTokensDecorator` location path before the token drop (E3/N1); re-time a CoreLib
  decompile after 3c against the ~33 s baseline (E2).
- See PART III for the full review-findings backlog (the items below were folded in there).
- **Post-rewrite (future): audit the AST against Roslyn's model and remove warts.** The slot model
  + trivia channel + required/optional nullability already converge on Roslyn's shape, so a parity
  pass becomes tractable once the rewrite lands. Known seed: `IndexerExpression` with null `Target`
  for implicit-target indexers (Roslyn: a distinct `ImplicitElementAccess`). Other candidates to
  check: object-initializer member names (bare `IdentifierExpression` vs an assignment shape),
  node-kind granularity where one node + a flag collapses distinct C# constructs. Gated on the
  rewrite being done; not a near-term task.

==========================================================================================
# PART III -- REVIEW FINDINGS & RESOLUTIONS (added 2026-06-13)
==========================================================================================

A code-grounded review of PARTS I/II surfaced the following holes, gaps, and decisions.
Each is a tracked task; the "Amends" line names the section to update when the task is handled.
Ordered as we will handle them.

## R0 (task #4) -- Delete `IFreezable`/`Freeze`/`IsFrozen` [LEAD, decided]
**Finding (verified):** `Freeze`/`IsFrozen`/`IFreezable` is entirely confined to `AstNode.cs`.
No transform, builder, or consumer ever calls `Freeze()` (the only `.Freeze()` in the tree is an
unrelated Avalonia brush in `CSharpHighlightingTokenWriter.cs`). The generated null-node singletons
do not freeze themselves either, so every `IsFrozen` guard in `SetChild`/`AddChild` only ever
evaluates false. The decompiler is single-threaded; nothing shares or freezes nodes.
**Decision:** Drop the whole `IFreezable` apparatus in this rewrite (it is exactly the NRefactory
full-fidelity baggage the Context section says to shed). Consequences to fold in:
- `AstNode` no longer implements `IFreezable`; remove `Freeze`/`IsFrozen`/`frozenBit`.
- `SetChildNode` (Part I.1) drops its frozen check.
- `CheckInvariant` (Part I.13) drops the "frozen consistency" clause.
- The freed flag bit is moot -- the `flags` word loses the role index anyway.
**Amends:** Part I.1, Part I.13.

## R1 (task #1) -- Comment carriers beyond statement/expression [base-model; before Phase 1/5]
**Finding (verified, full enumeration):** today a `Comment` attaches via `Roles.Comment` to almost
any node (the `InsertSpecialsDecorator` interleaves them at print time). `CommentStatement` +
`CommentedExpression` (Part I.6) cover only 2 of the **6** receiver categories in use. (Doc
comments -- `CommentType.Documentation` from `AddXmlDocumentationTransform` -- are separate and go
to the `Documentation` slot.)
Covered: block statement-list (-> `CommentStatement`: `StatementBuilder.cs:494,1362,1415`,
`CSharpDecompiler.cs:1436,2049,2103`, `ExpressionBuilder.cs:2445,2623`); on an `Expression`
(-> `CommentedExpression`: `CallBuilder.cs:567`, `ErrorExpression.cs:57`,
`ExpressionBuilder.cs:2065,4967`).
**Not covered -- six gaps:**
- A. trailing comment on a **non-block `Statement`** -- `StatementBuilder.cs:140,1526,1544`.
- B. **`TypeDeclaration.Members`** (sibling of `EntityDeclaration`) -- `CSharpDecompiler.cs:1698,
  1727` ("error: enumerator has no value", "nested types are not permitted"). A `CommentStatement`
  is a `Statement`, so it cannot live in a `Members` collection -- the one position that cannot be
  faked with the two proposed nodes. (Corrects an earlier mistaken "never between members" claim.)
- C. on an **`AstType`** -- `DeclareVariables.cs:616` ("pinned"), `TypeSystemAstBuilder.cs:414`.
- D. on a **`VariableInitializer`** -- `CSharpDecompiler.cs:2322` (the "could not decompile" field
  message).
- E. on an **`Attribute`** -- `TypeSystemAstBuilder.cs:813` ("Could not decode attribute
  arguments.").
- F. on an **`Accessor`** (an `EntityDeclaration`, printed inline) -- `TypeSystemAstBuilder.cs:2123`
  ("init").
**RESOLVED -- comments become node trivia (leading/trailing), off the child model.**
(LATER REFINED -- see authoritative Part I.6: the element type is a `Trivia` base covering BOTH
`Comment` AND `PreProcessorDirective` (directives folded in, not separate marker nodes); storage is
the existing annotation channel via a lazy `NodeTrivia` holder -- NO per-node fields -- so `Clone`
is automatic via `CloneAnnotations`. The original sketch below predates those two refinements.)
- **Storage:** a single per-node trivia collection on `AstNode`, a side-channel like annotations --
  *outside* the flattened child-index space (so no `Comments` slot bloats every leaf's
  `GetChild/SetChild` dispatch, no pattern-matcher interaction, `CheckInvariant`'s child-range
  arithmetic stays clean). Each entry is tagged leading or trailing.
- **Public API:** `AddLeadingTrivia(Trivia)` / `AddTrailingTrivia(Trivia)` -- the
  leading/trailing distinction is encoded by which method is called; callers never set a flag.
  Read-back via filtered `LeadingTrivia` / `TrailingTrivia` views.
- **Print mechanic:** the output visitor emits leading trivia at `StartNode` (before the node's
  children) and trailing at `EndNode` (after), preserving insertion order within each group. No
  interleaving into the sibling sequence, no anchor index. Replaces `InsertSpecialsDecorator`'s
  `Roles.Comment` interleaving + the `AstNode.cs` comment-skip walks.
- **`Clone`** copies trivia alongside `CloneAnnotations()`; `CheckInvariant` treats trivia as
  outside the child range (parent set, no `ChildIndex`).
- **Doc comments** keep the dedicated `Documentation` slot (structural, rides with the member);
  they are not folded into this trivia channel.
- **Migration mapping** (the six categories -> the node each comment annotates):
  A `StatementBuilder.cs:140` -> `AddTrailingTrivia`; `:1526,1544` -> `AddLeadingTrivia`.
  B `CSharpDecompiler.cs:1698,1727` -> re-home from `typeDecl.InsertChildBefore(member,...)` to
  `member.AddLeadingTrivia(...)`.
  C `DeclareVariables.cs:616`, `TypeSystemAstBuilder.cs:414` -> `AddLeadingTrivia` on the `AstType`.
  D `CSharpDecompiler.cs:2322` -> `AddTrailingTrivia` on the `VariableInitializer`.
  E `TypeSystemAstBuilder.cs:813` -> `AddTrailingTrivia` on the `Attribute`.
  F `TypeSystemAstBuilder.cs:2123` -> `AddTrailingTrivia` on the `Accessor`.
- **Implementation guard:** assert no migrated site needs an *interior* comment (between two real
  children of the same node where neither is the annotated node); the enumeration shows all are
  first/last/leading-on-member, so the model is sufficient -- the assert catches a regression.
**Amends:** Part I.6 (rewrite "comments as slot children" -> trivia channel), Phase 5 (drop
`InsertSpecialsDecorator` trivia path; add the `AddLeading/TrailingTrivia` migration).

## R2 (task #2) -- Locations: keep `StartLocation`/`EndLocation`, drop the `Span` tuple [RESOLVED]
**Finding (verified, `AstNode.cs:183-199`):** today `StartLocation`/`EndLocation` are *virtual,
computed by recursion* -- a non-leaf returns `firstChild.StartLocation` / `lastChild.EndLocation`,
bottoming out at the **token leaves** (`CSharpTokenNode` etc.), which store the real location
written at print time by `InsertMissingTokensDecorator` (`node.Location = ...`). The Phase-4 token
drop removes those leaves, so the recursion loses its base case -- *that* is the only reason
Part I.7 proposed explicit per-node storage. The stored values are still exactly
`(TextLocation start, TextLocation end)`.
**Decision -- no `Span` tuple; keep the two existing properties:**
- **Public API:** keep `StartLocation` / `EndLocation` (each `TextLocation`). This *is* the facade
  resolution -- zero churn across the ~33 readers (`SequencePointBuilder` + the `GetNodeAt`/
  `GetNodesBetween`/`Contains` tree-nav in `AstNode` itself), and consistent with #11. A `Span`
  tuple would force rewriting all 33 sites for no semantic gain. `Span` is removed from the design.
- **Storage:** two backing `TextLocation` fields (a private struct is an impl detail, not exposed)
  with an `internal` setter / `SetLocations(start, end)`, written by the existing `StartNode`/
  `EndNode` + `WrapInWriterThatSetsLocationsInAST` path Part I.7 already keeps.
- **Drop the `virtual` compute-from-children recursion:** every printed node is bracketed by
  `StartNode`/`EndNode` and stores its own span directly; nothing needs to derive from children
  once leaves no longer carry locations.
- **Timing -- no computed fallback needed:** locations are only meaningful after the format pass
  *today too* (token-leaf locations are `Empty` until `InsertMissingTokensDecorator` runs). All
  ~33 readers are post-format (`SequencePointBuilder` on formatted output; UI nav on the printed
  tree); no mid-pipeline reader exists.
- Token-drop consequence (already in Part I.7): `SequencePointBuilder`'s brace breakpoints move
  from `LBraceToken`/`RBraceToken` locations to `BlockStatement` start/end.
**Amends:** Part I.1 (replace the `Span` property with stored `StartLocation`/`EndLocation` +
internal setter; drop the `virtual` recursion), Part I.7 (s/`Span`/`StartLocation`+`EndLocation`/).

## R3 (task #3) -- Full `NodeCollection<T>` API surface [before Phase 1]
**Finding (verified):** Parts I.1/I.4 mention only Add/Insert/remove. The actually-used
`AstNodeCollection<T>` surface a replacement must cover: `MoveTo` (cross-collection move -- the C6
case), `ReplaceWith(IEnumerable)`, `AddRange`, `InsertBefore`/`InsertAfter(item,...)` (by reference
node, **not** index), `Detach`, `AcceptVisitor`, indexer/`Count`/`Contains`/`CopyTo`, and
`FirstOrNullObject`/`LastOrNullObject`. The last two are entangled with Phase 8 -- they become
`FirstOrDefault`-returning-`null` once null objects die.
**Amends:** Part I.1.

## R4 (task #5) -- `AttributeTarget` enum completeness [before Phase 4]
**Finding (verified):** Part I.5 converts the string target to an enum, but the old commented-out
enum `{None,Assembly,Module,Type,Param,Field,Return,Method}` omits `property`, which
`TransformFieldAndConstructorInitializers.cs` actually sets (`= "property"`).
**Action:** enumerate the complete set (assembly, module, type, param, field, return, method,
property, event -- verify `event`) before generating.
**Amends:** Part I.5.

## R5 (task #6) -- Add `SetChildByRole` to the deletion list [Phase 7]
**Finding (verified):** Part I.9 lists `GetChildByRole`/`GetChildrenByRole`/`AddChild`/
`InsertChild*` but omits `SetChildByRole` (used in `AttributeSection.AttributeTarget` setter, etc.).
**Amends:** Part I.9, Phase 7.

## R6 (task #7) -- Empty single-slot index occupancy [doc clarity; Phase 1/8]
**Finding:** For O(1) fixed-slot access plus the Part I.13 round-trip invariant
(`GetChild(child.ChildIndex) == child`), an empty optional **single** slot must still occupy its
flattened index (return `null` at a fixed position); only **collection** slots vary the offset.
Part I.4 only describes collections shifting offsets, leaving two readings.
**Action:** state that single slots are always width-1, so the flattened index space is stable
across the Phase 8 null->nullable transition.
**Amends:** Part I.4.

## R7 (task #8) -- Required/optional model: nullable field, required accessor, checkpoint-enforced [RESOLVED]
**Question raised:** are any slots actually "required"? Yes -- binary operands, `IfStatement.Condition`/
`TrueStatement`, loop condition/body, `CastExpression.Type`/`Expression`, assignment sides,
`MemberReferenceExpression`/`InvocationExpression.Target`, etc. are always filled in a finished tree
and an empty one is a bug. (Genuinely optional: `ReturnStatement.Expression`, `MethodDeclaration.Body`,
`IfStatement.FalseStatement`, property accessors.) But every node has a parameterless ctor + settable
slots and the decompiler builds incrementally (`BinaryOperatorExpression.cs:63-91`), so a required
field is transiently empty during construction -- "required" cannot mean non-null for the node's whole
lifetime.
**Principle (from the user):** during a transform every slot is effectively optional (may be empty);
`CheckInvariant` -- which runs *between* transforms -- guarantees the required ones exist at those
checkpoints.
**Resolution -- split storage from contract:**
- **Backing field is always nullable.** This is "optional during a transform": any slot may be
  detached/left empty mid-pipeline; transforms never fight the type system; construction stays
  incremental; no `= null!` lie. (Note: this means single optional/required slots both store a
  nullable field -- the field is uniformly nullable; only the *accessor* differs.)
- **Accessor nullability IS the required marker** (single source of truth, per Part I.2: declared
  nullability = `IsOptional`). Required -> non-null accessor `Expression Left` (getter returns
  `field!` -- throws if read while empty); optional -> `Expression? Body`.
- **`CheckInvariant` reads `CSharpSlotInfo.IsOptional`** and asserts required slots non-null at each
  post-transform checkpoint -- "make sure the nodes exist," at the stable boundaries, not every instant.
- **Contract:** between transforms the tree is whole (required filled); within a transform anything
  goes (field tolerates null). Reading a transiently-empty required child throws at the bug site;
  forgetting to refill is caught by `CheckInvariant` at the boundary.
**This closes #8:** no special `Detach()`/`Remove()` rule -- transient-null is allowed by the field,
existence is enforced at checkpoints, misuse throws at read.
**Payoff preserved:** ergonomic non-null reads at the hundreds of `node.Left.AcceptVisitor(...)`
sites (no `T?` warning flood -- the only reason null objects existed); still deletes the 18 null-node
classes + `IsNull`/`VisitNullNode` plumbing.
**Classification rule -- "syntactically required" (objective oracle, replaces per-slot judgment):**
a slot is required (non-null accessor = the static contract) iff the **C# grammar** cannot omit it
(`BinaryOperatorExpression.Left`/`Right`, `IfStatement.Condition`/`TrueStatement`, loop condition+
body, `CastExpression.Type`/`Expression`, assignment sides, `...Target`); optional iff the grammar
allows it absent (`ReturnStatement.Expression`, `MethodDeclaration.Body`, `IfStatement.FalseStatement`,
`VariableInitializer` initializer, property accessors, attribute args). **Mechanical proxy:** a slot
is required iff `CSharpOutputVisitor` dereferences it *unconditionally* (no `if (x != null)` guard /
no alternate-token branch). The printer is ground truth -- it is the consumer that would NRE, and its
access pattern necessarily matches the grammar -- so Phase 8 reads the output visitor rather than
guessing. This de-risks C7. `default-to-optional` survives only as the tie-breaker for a genuinely
ambiguous slot.
**Residual:** classification is still per-slot work, but now objective and single-homed (accessor
nullability feeds both the type and `CheckInvariant`).
**Verified (and rule refined):** the "unconditional-deref" proxy is necessary but not sufficient --
the null-object no-op can mask a legitimately-empty slot, so construction sites must also be checked.
Landmine found: `IndexerExpression.Target` is visited unconditionally yet intentionally empty in
object initializers (`CallBuilder.cs:1690`) and property-pattern index paths
(`ExpressionBuilder.cs:3585`) -> classify **optional**, and guard it in `VisitIndexerExpression`.
This is the **sole** null-`Target` case: `MemberReferenceExpression`/`InvocationExpression` `Target`
are never null-constructed (`this.A` is an MRE with a `ThisReferenceExpression` target, bare `A` is
an `IdentifierExpression` -- no implicit-`this` null-target), so both stay required. `ErrorExpression`
is always a complete top-level node (never a stand-in child), so error paths never empty a required
slot. The IndexerExpression null-target is a modeling wart (implicit-target indexer as
`IndexerExpression`); Phase-8 may either make `Target` optional (recommended) or give the
implicit-target indexer its own node.
**Amends:** Part I.11 (nullable field + required-accessor model; drop "required slot = non-null
field"), C7, Part I.13 (the "required slots non-null" clause sources `IsOptional`), Phase 8.

## R8 (task #9) -- The bridge is not output-neutral [Phase 3]
**Finding:** The Phase 3a bridge maps `GetChild(i)` to "the i-th role-ordered child," but
linked-list children are in **insertion** order while slot order is **schema** order. The moment a
family converts, the printer (walking `Children`) sees schema order. Today these coincide *except*
the `TransformFieldAndConstructorInitializers` reordering hack -- so the bridge should be neutral,
but any other latent insertion-vs-schema divergence surfaces as a Pretty diff at **bridge** time,
not flip time. The per-family gate ("slot accessors agree with old role accessors") checks per-role
identity, not document order.
**Action:** lean on the Pretty suite at bridge time, or add a document-order assertion to the gate.
**CONFIRMED by E1 (`3050aee76`):** across all 667 decompiled fixtures the bridge produced
byte-identical Pretty output and the document-order assert never tripped for the tested families --
so the gate IS the slot-order==document-order invariant of Phase 3a, prototyped and proven for
simple Expressions. Still to do: extend the assert to optional/reordered-child families
(modifiers/attributes, the EntityDeclaration interleave from E4) before the 3c flip.
**Amends:** C1, Phase 3.

## R10 (task #11) -- Keep original type names; drop the `NodeCollection` rename [decided]
**Finding:** The design is asymmetric -- it keeps `AstNode` but renames `AstNodeCollection<T>` ->
`NodeCollection<T>` to mirror IL's `InstructionCollection<T>` (Parts I.1, I.9, I.10, Phase
6.5/6.7, Phase 7, Files, R3).
**Decision:** Keep **both** original names. Reimplement `AstNodeCollection<T>` over slots
(list-backed, parent+slot, renumbering) rather than delete+rename it. Rationale: (1) renaming a
heavily-referenced public type is a gratuitous API break on top of the necessary roles/tokens/
nulls removal, against `reduce-api-diff` and noisier in the Phase 7/9 API-diff gate; (2) `AstNode`/
`AstNodeCollection<T>`/`AstType` are a name family -- keeping `AstNode` while renaming the
collection breaks the symmetry; (3) the IL parallel is an implementation detail, not a name.
**New public names that DO stay:** `CSharpSlotInfo` (no prior type) and `node.Slot` (replaces
`node.Role`).
**Sweep:** replace every `NodeCollection<T>` with `AstNodeCollection<T>` and change "delete
`AstNodeCollection<T>`" to "reimplement over slots" in Parts I.1, I.9, I.10, Phase 6.5/6.7,
Phase 7, the Files list, and R3 above.
**Amends:** Parts I.1, I.9, I.10, Phase 6, Phase 7, Files.

## R9 (task #10) -- Resolved open items (recorded)
- **Directives: CONFIRMED no bracketing.** Only `#define` is emitted today; region/if/endif/pragma
  are never constructed (the printer's generic `WritePreProcessorDirective` *can* emit them, but no
  site does). (LATER REFINED -- see Part I.6: directives are NOT separate marker nodes; they are
  folded into the leading/trailing `Trivia` channel alongside comments. `#define` -> leading trivia
  on the `SyntaxTree`. The no-bracketing finding is what makes trivia sufficient.)
- **Partial properties / LangVersion: non-issue.** `ICSharpCode.Decompiler` is single-target
  `netstandard2.0` with `<LangVersion>14</LangVersion>`; partial properties (C# 13+) are
  compile-time and available. (The lib is *not* multi-targeted, contrary to the CLAUDE.md framing;
  the `init`-breakage constraint is a `netstandard2.0`-runtime `IsExternalInit` concern, orthogonal
  to this.)
- **Companion proof file** (`~/.claude/plans/rustling-bubbling-oasis-...md`) is load-bearing for
  Phase 6 -- confirm it still exists and matches this revision before starting Phase 6.
