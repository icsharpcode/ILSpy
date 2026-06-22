# The C# syntax tree and its source generator

The classes in this folder make up the C# abstract syntax tree (AST) that the decompiler builds and
then pretty-prints. Each node is a **slot-based** tree node: its children live in numbered *slots*,
the same model the IL AST (`ICSharpCode.Decompiler/IL`) uses.

Almost all of the mechanical per-node code -- the visitor plumbing, structural pattern matching, the
child-index API, constructors, and slot metadata -- is **generated** by a Roslyn source generator
(`ICSharpCode.Decompiler.Generators/DecompilerSyntaxTreeGenerator.cs`). You declare a node and its
children with a few attributes and `partial` properties; the generator fills in the rest.

## A node at a glance

```csharp
[DecompilerAstNode]
public sealed partial class BinaryOperatorExpression : Expression
{
    [Slot("Left")]
    public partial Expression? Left { get; set; }     // a child slot (optional: nullable)

    public BinaryOperatorType Operator { get; set; }  // a scalar -- a plain auto-property

    [Slot("Right")]
    public partial Expression? Right { get; set; }    // another child slot
}
```

From this the generator emits the `AcceptVisitor` overloads, `DoMatch`, the backing fields and bodies
for `Left`/`Right`, the flattened child-index dispatch (`GetChildCount`/`GetChild`/`SetChild`/...),
constructors (`new BinaryOperatorExpression(left, right)`), the `LeftSlot`/`RightSlot` metadata, and
the clone logic. The hand-written file only carries what is genuinely node-specific.

## The attributes

### `[DecompilerAstNode]`

Marks a class for generation. The class **must be `partial`** (the generator emits the other half).

- `[DecompilerAstNode]` -- a normal node.
- `[DecompilerAstNode(hasPatternPlaceholder: true)]` -- in addition, emit the pattern-placeholder
  machinery so the type can appear in a pattern (the `implicit operator` from `Pattern` and the nested
  `PatternPlaceholder`). Used on the bases that pattern matching substitutes into (`AstNode`,
  `Expression`, `Statement`, `AstType`, ...).

A concrete node whose direct base is abstract also gets the three `AcceptVisitor` overloads and a
`Visit<NodeName>` entry on the generated `IAstVisitor` interfaces.

### `[Slot("Kind")]`

Declares one child position. Put it on a `partial` property; the **property type** decides the shape,
and the **string** is the slot *kind* (see [Slots and kinds](#slots-and-kinds)):

| Property type | Meaning |
|---|---|
| `Expression Foo` (any `AstNode` subtype) | a single required child |
| `Expression? Foo` | a single **optional** child (may be empty / null) |
| `AstNodeCollection<Parameter> Foo { get; }` | a child **collection** (get-only; the generator owns the body) |
| `string Name` | a **name**: a convenience string over a generated backing `Identifier` token slot |
| `string? Name` | an **optional** name (reads as `null` when absent) |

The kind string is shared across node types: every slot tagged `[Slot("Expression")]` has the same
kind, so `node.Slot.Kind == Slots.Expression` works polymorphically. Pick an existing kind name when
the position is the same logical role *and* child type as on other nodes; pick a new one when either
differs. A kind must map to a **single child type** across every node that uses it: the generator
emits one typed `Slots.X` constant per kind, and a kind spanning two child types would have to widen
that constant to `AstNode`, leaving the typed accessors (`GetChildren` especially) unable to recover
the real element type. The generator enforces this at build time (`DSTG001`). A position that is
genuinely an expression *or* a statement -- a lambda body -- is still a single declared type,
`AstNode`, and is fine.

### `[ExcludeFromMatch]`

Drops a property from the generated `DoMatch`. Use it where structural equality should ignore a slot
-- e.g. a constructor's `NameToken` (its name is just the declaring type's name, matched elsewhere).

## Slots and kinds

A slot is described at runtime by **`CSharpSlotInfo`** (`Name`, `ChildType`, `IsCollection`,
`IsOptional`, and `Kind`). The generic `CSharpSlotInfo<T>` additionally carries the child type as a
type parameter so the typed accessors can infer it.

There are two flavours of `CSharpSlotInfo` instance, both generated:

- **Per-node slots** -- `BinaryOperatorExpression.LeftSlot`, etc. One per slot per node type; carries
  that node's `IsOptional`/`ChildType`. `node.GetChildSlotInfo(i)` returns these.
- **Canonical kinds** -- the `Slots` holder (`Slots.Left`, `Slots.Expression`, ...). One per distinct
  kind across the whole AST. These are the shared identity, matched by reference.

A per-node slot's `Kind` points at its canonical `Slots` constant; a canonical constant is its own
kind. A kind *is* an object -- the `Slots` constant -- matched by reference identity:

```csharp
node.GetChild(Slots.Left)              // typed access; T inferred from Slots.Left (-> Expression?)
node.GetChildren(Slots.Parameter)      // a collection, element type inferred
node.Slot.Kind == Slots.Initializer    // "is this node in an Initializer-kind slot?", polymorphic
```

## Scalars and constructors

A settable property that is **not** a `[Slot]` -- a `bool`, an enum, etc. -- is just a normal
auto-property and is **not** a child. A settable enum-typed scalar is treated as part of construction,
so it becomes a constructor parameter alongside the child slots, in source-declaration order.

If a scalar has an invariant (e.g. it must be non-negative), express it as a `CheckInvariant` override
instead of a throwing setter (see below). Pure-value nodes with no `[Slot]` (e.g.
`PrimitiveExpression`, whose state is an opaque literal) keep hand-written constructors.

## `CheckInvariant`

`AstNode.CheckInvariant()` (DEBUG only) runs **before the transform pipeline and after every
transform** (`CSharpDecompiler.RunTransforms`), the analog of `ILInstruction.CheckInvariant`. The base
implementation recursively checks the slot structure: required (non-optional) single slots are filled,
each child's `Parent` points back, the flattened index matches, and the runtime type fits the slot.

A node type **overrides** it (calling `base`) to assert its own scalar invariants -- constraints on a
scalar property that the slot structure cannot express:

```csharp
internal override void CheckInvariant()
{
    base.CheckInvariant();
    Debug.Assert(PointerRank >= 0, "ComposedType.PointerRank must not be negative");
}
```

So a transform that corrupts the tree fails at the exact transform, not as a downstream output diff.

## Adding a new node

1. Create `Foo.cs` in the appropriate sub-folder, with the license header and `#nullable enable`.
2. Declare `[DecompilerAstNode] public sealed partial class Foo : <Base>` (or `abstract`, with
   `hasPatternPlaceholder: true` if it's a pattern-matchable base).
3. Add a `[Slot("Kind")] public partial <Type> Child { get; set; }` for each child (or
   `AstNodeCollection<T> Children { get; }` for a collection; `string Name` for a name). Mark optional
   children nullable. Declare them in the order they print.
4. Add any scalar auto-properties. Add a `Visit Foo` to the hand-written visitor base classes if your
   node needs custom visiting beyond the generated dispatch.
5. Build. Inspect the generated half if needed by building with
   `-p:EmitCompilerGeneratedFiles=true` (output under `obj/.../generated/`).
6. Pretty-print it in `CSharpOutputVisitor`, add a `VisitFoo`, and add a fixture under
   `ICSharpCode.Decompiler.Tests/TestCases/Pretty`.

## Conventions and gotchas

- The `[Slot]` string is the **bare kind name** ("Body", "Expression"), not a dotted role expression.
- A kind maps to exactly one child type; reusing one name for two different child types is a build
  error (`DSTG001`). When two positions would share a name but differ in child type, give each its own
  kind name -- as the declaration-level attribute slot (`[Slot("AttributeSection")]`) does versus an
  attribute section's own `[Slot("Attribute")]`, or the for-statement initializer
  (`[Slot("ForInitializer")]`, a `Statement` collection) versus an object/array initializer
  (`[Slot("Initializer")]`, an `ArrayInitializerExpression`).
- Optional **names** read back as `null` (not an empty string) and a `null`/empty assignment clears the
  backing token.
- Don't hand-write what the generator emits (visitors, `DoMatch`, child dispatch, constructors, slot
  metadata, clone). If you find yourself doing so, the node is probably missing a `[Slot]`.

## Working on the generator (notes for agents and tooling)

A few things that are easy to get wrong when editing the generator or the nodes automatically:

- **Build the way the repo expects.** Use the root pwsh scripts, and the OpenSSL env var is required:
  `OPENSSL_ENABLE_SHA1_SIGNATURES=1 pwsh ./build.ps1 -Configuration Debug --no-restore`. A bare
  `dotnet build` prunes the lock files (see the root `CLAUDE.md`).
- **The generator is a cached analyzer assembly.** Editing a **node** file re-runs the generator (node
  files are its input). Editing **the generator's own source** may *not* re-run it on an incremental
  build -- the consuming project keeps using the cached analyzer. Force a fresh run by clearing the
  generator's outputs: `rm -rf ICSharpCode.Decompiler.Generators/{bin,obj}/Debug` and rebuild.
- **Don't verify generation by reading `obj/.../generated/*.g.cs`.** Those files are written only when
  the build sets `-p:EmitCompilerGeneratedFiles=true`, so they are usually a *stale* snapshot and will
  mislead you. To confirm the generator emits what you expect, either build with that flag, or just
  reference the expected symbol (e.g. `Slots.Foo`) in code and let the build fail or pass.
- **Validate with the DEBUG test run.** `CheckInvariant` is `[Conditional("DEBUG")]` and the test host
  runs DEBUG, so after any AST or transform change run the Pretty suite in DEBUG:
  `OPENSSL_ENABLE_SHA1_SIGNATURES=1 dotnet test --solution ILSpy.sln --report-trx --no-build -c Debug
  --filter "FullyQualifiedName~PrettyTestRunner"`. It checks the output is byte-identical **and**
  exercises `CheckInvariant` across every node shape -- a green run is the real signal, not a green
  build.
- **A `required slot '<Name>' on <Node> must not be empty` assertion** means a slot is declared
  non-nullable but is legitimately empty at runtime. The fix is almost always to make that `[Slot]`
  property nullable (the position is optional), not to relax the check.
