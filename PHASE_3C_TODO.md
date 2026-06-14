# Phase 3c -- storage flip (linked list -> slot fields). Fresh-session execution checklist.

> **UPDATE 2026-06-14: Phases 4 (token drop) + 5 (comments->trivia) are now DONE (20 commits on
> `ast-source-generator-rebased`, byte-identical, unpushed) -- they were reordered AHEAD of 3c.
> So this checklist is now SIMPLER than written: CSharpTokenNode/CSharpModifierToken are deleted
> (no token children), InsertSpecialsDecorator is deleted, and there are NO positional comment
> children or `Roles.Comma` trailing-comma markers any more -- IGNORE the "Risks/gotchas" notes
> about comments/commas/token positional children below; they no longer exist. The only
> comment-as-node forms are ErrorExpression + comment-bearing EmptyStatement (normal nodes).
> StartLocation/EndLocation are ALREADY stored fields set at print (not computed) -- the location
> part of 3c is done. The GetNodeAt-family nav methods are dead -> just DELETE them. Launch from
> current clean HEAD (not `20a04825e`). The ILSpy-tests submodule is now fully checked out in the
> worktree (net40 configs run; no nuget-symlink workaround needed).**

Atomic change (corner C1): the build does NOT compile again until ALL of A+B+C land together.
Spec: `ROLES_FREE_SLOT_AST_DESIGN.md` Part I.1 + Phase 3c.

Build/test (every run): prefix `OPENSSL_ENABLE_SHA1_SIGNATURES=1`; build with
`-p:RestoreEnablePackagePruning=false`; suite is
`dotnet test ICSharpCode.Decompiler.Tests/ICSharpCode.Decompiler.Tests.csproj --report-trx`
(needs the `ILSpy-tests/nuget` symlink -> `/home/siegfried/Projects/ILSpy/ILSpy-tests/nuget`,
recreate if absent or every test Assert.Fails at fixture setup). After each commit discard any
pruned `*packages.lock.json`. Let the pre-commit format hook run (it touches DecompilerVersionInfo.cs
harmlessly). Use `Assisted-by:` trailer, never Co-Authored-By.

## 0. Optional pre-flip cleanups (cheap, do first or skip)
- [ ] Squash `f914d57b3` (tried slot-order reorder) + `7fea03be7` (reverted it) -- they cancel; branch
      is local-only so an interactive rebase is safe. Optional, cosmetic.
- [ ] `UsingAliasDeclaration` alias: `AliasRole` holds an Identifier child but is exposed only as a
      string wrapper (no Identifier property). Add `[Slot("AliasRole")] public partial Identifier
      AliasToken { get; set; }` so the schema is complete (or handle inside the flip).

## A. AstNodeCollection<T> -> list-backed  (CSharp/Syntax/AstNodeCollection.cs)
Currently a *view* over the linked list (iterates parent.firstChild filtering by role). Rewrite:
- [ ] Back with `List<T>`; hold `AstNode parent` + `int slotIndex` (the collection's slot).
- [ ] Keep the used surface (already enumerated in design Part I.1): Add, AddRange, Insert,
      InsertBefore(item)/InsertAfter(item), Remove, Clear, ReplaceWith(IEnumerable), MoveTo, Detach,
      AcceptVisitor, indexer/Count/Contains/CopyTo/GetEnumerator, FirstOrNullObject/LastOrNullObject.
- [ ] On every mutation, set added child's parent + role + childIndex, detach old, and **renumber
      following children's childIndex** (insert/remove shift). Strict-tree invariant.
- [ ] Implement IReadOnlyList<T> AND explicit IReadOnlyList<INode> (for the matcher, Phase 6).

## B. AstNode base rewrite  (CSharp/Syntax/AstNode.cs)
- [ ] Remove `firstChild/lastChild/prevSibling/nextSibling`. Add `int childIndex = -1`. Keep `parent`,
      keep `flags` (role index stays until Phase 7 -- a child's role is set when placed in a slot).
- [ ] Make abstract (generator implements per leaf): `int GetChildCount()`, `AstNode? GetChild(int)`,
      `void SetChild(int, AstNode?)`, `CSharpSlotInfo GetChildSlot(int)`. (Replaces the bridge's
      SlotCount/GetSlotRole/IsCollectionSlot/GetChild/SetChild.)
- [ ] `SetChildNode<T>(ref T field, T value, int slotIndex)`: frozen-free; validate not-parented /
      not-in-another-tree / type-compatible; detach old; write field; set value.parent + childIndex.
- [ ] Re-express the linked-list API over slots (flattened index, Part I.4 -- single slot = width 1
      even when empty; collection slot = contiguous run of its count; offsets after a collection shift
      by its length):
      - FirstChild/LastChild/NextSibling/PrevSibling: computed from (parent, childIndex), skipping
        empty optional single slots.
      - Children / HasChildren: iterate flattened slots in order.
      - Descendants/GetDescendantsImpl, GetNodeAt/GetNodesBetween/Contains: now go through Children
        (already do) -- verify they work once Children is slot-based. (These location-nav methods are
        NRefactory-inherited and currently have no in-tree callers, but the API is kept; location
        correctness itself is covered by LocationsInAstTests' text-slice assertion, not via GetNodeAt.)
- [ ] Re-express the **role API** (still used by consumers until Phase 7) over slots:
      - GetChildByRole<T>(role): scan slots for GetChildSlot(i).Role == role; single -> (T)GetChild(i)
        (role null object if empty); collection role -> first element.
      - GetChildrenByRole<T>(role): return the collection for that role. NB the base needs to reach the
        collection object generically -- emit a generator hook (e.g. virtual AstNodeCollection
        GetCollection(int slot)) or have GetChildrenByRole map role->slot and return the field.
      - SetChildByRole, AddChild(child, role), AddChildWithExistingRole, InsertChildBefore/After(sibling),
        Remove(), Detach, ReplaceWith(node) = parent.SetChild(childIndex, node).
- [ ] StartLocation/EndLocation: still compute from first/last child (over slots) for now -- the
      stored-field-at-print version is coupled to the Phase-4 token drop, NOT this phase.
- [ ] Clone: MemberwiseClone; reset parent/childIndex; per-slot deep clone (loop GetChildCount/GetChild
      or generator-emitted Clone) + CloneAnnotations (+ trivia, once that lands in Phase 5).

## C. Generator  (ICSharpCode.Decompiler.Generators/DecompilerSyntaxTreeGenerator.cs)
- [ ] From the existing [Slot] declarations, emit: backing fields (single `T f;` / collection
      `readonly AstNodeCollection<T> f`); property bodies single `get => f; set => SetChildNode(ref f,
      value, <slot>)`, collection `get => f`; the flat GetChildCount/GetChild/SetChild/GetChildSlot
      with collection range arithmetic; CSharpSlotInfo statics; per-slot Clone.
- [ ] Inherited (override) slots already declared on leaves (flatten) -- emit their field+body on the
      leaf (the field lives on the leaf, the base contract member becomes abstract or stays virtual).
- [ ] Drop/replace the transitional SlotCount/GetSlotRole/IsCollectionSlot bridge emission.

## D. Iterate to green
- [ ] Build; fix the cascade of compile errors (internal users of firstChild/nextSibling, any direct
      linked-list pokes, AstNodeCollection ctor signature changes). Expect many.
- [ ] Full suite -> must return to 1195/984 baseline, byte-identical, 0 diffs.
- [ ] ilspycmd smoke-decompile a couple assemblies; perf re-check vs the ~33s CoreLib baseline (E2).

## E. Owed at the flip (tracked tasks #15, #16)
- [ ] #16: re-introduce the slot-order invariant (now holds by construction -- children stored by slot)
      as a DEBUG CheckInvariant; wire into the IAstTransform pipeline (design Part I.13).
- [ ] #15: revert the one bridge-era decompiler change -- `IntroduceExtensionMethods` ReplaceWith
      (commit 6869ef7a7) -- now unnecessary; audit for any other decompiler-transform changes.

## Risks / gotchas
- The role API reimplementation (single vs collection, role->slot lookup) is the fiddly core -- get
  GetChildByRole/GetChildrenByRole exactly right; lots of consumers depend on them.
- Pattern matcher: `AstNodeCollection<T>` is used by the matcher; keep IReadOnlyList<INode> working
  (full matcher port is Phase 6, but don't break it now).
- Comments/trivia are still ordinary `Roles.Comment` children in the linked-list model; after the flip
  they're collection/role children too -- verify comment output unchanged (Phase 5 moves them to the
  trivia channel later).
- This delivers the O(1) access + is where perf finally arrives (the bridge had neither).
