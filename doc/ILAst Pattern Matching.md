ILAst Pattern Matching
=================

Some IL instructions are classified as "patterns".
```c#
abstract class PatternMatchILInstruction : IStoreInstruction {
   public ILInstruction TestedOperand { get; }
   public ILVariable Variable { get; } // variable that receives the match result; may also be a temporary
}

public bool IsPattern(this ILInstruction inst, out ILInstruction testedOperand) => inst switch {
    PatternMatchILInstruction pm => testedOperand = pm.TestedOperand; return true;
    LogicNot logicNot => IsPattern(logicNot.Operand, out testedOperand),
    Comp comp => testedOperand = comp.Left; return IsConstant(comp.Right);
}
```
Every `match.*` instruction has the following properties:
  * The `TestedOperand` specifies what gets matched against the pattern.
  * The `Variable` stores the value of `TestedOperand` (after converting to the matched type, if appropriate).
    * If this variable is also used outside the `match.*` node, it corresponds to the C# `single_variable_designation`.
    * Otherwise it's a temporary used for pattern matching.
    * I think in both cases it should have VariableKind.PatternVar
  * The match instruction evaluates to `StackType.I4`: 0 if the pattern was matched, 1 otherwise.

Some `match` instructions have a body with `List<ILInstruction> nestedPatterns`. Here every nested pattern must be a pattern according to `IsPattern()`, and the `testedOperand` of each must be a member of the `Variable` of the parent pattern. (members are: field, property, or deconstruction.result).
(exception: `match.and`/`match.or`, these instead require the `testedOperand` to be exactly the `Variable` of the parent pattern)


Examples
--------
1) `expr is var x`
    =>
    `match.var(x = expr)`
    =>
    ```
    Block (VarPattern) {
	    stloc x(expr)		// single eval expr
	    final: ldc.i4 1		// match always
    }
    ```
2) `expr is T x`
	=>
    `match.type T(x = expr) {}`
    =>
	```
    Block (TypePattern) {
        stloc x(isinst T(expr))
        final: x != null
    }
	```
3) `expr is C { A: var x } z`
	=>
    ```
    match.type C(z = expr) {
       match.var(x = z.A)
    }
    ```
    =>
	```
    Block (TypePattern) {
        stloc z(isinst T(expr))
        final: (z != null)
            && Block(VarPattern) {
                 stloc x(z.A)
                 final: ldc.i4 1
               }
    }
	```
4) `expr is C { A: var x, B: 42, C: { A: 4 } } z`
    =>
    ```
    match.type C(z = expr) {
        match.var (x = z.A),
        comp (z.B == 42),
        match.recursive (temp2 = z.C) {
            comp (temp2.A == 4)
        }
    }
    ```
	=>
	```
    Block (TypePattern) {
        stloc z(isinst C(expr))
        final: (z != null)
            && Block(VarPattern) {
                 stloc x(z.A)
                 final: ldc.i4 1
               }
            && comp (z.B == 42)
            && Block(RecursivePattern) {
                 stloc temp2(z.C)
                 final: (temp2 != null)
                     && comp (temp2.A == 4)
               }
    }
	```
5) `expr is C(var x, var y, <4) { ... }`
   =>
   ```
   match.recursive.type.deconstruct(C tmp1 = expr) {
       match.var(x = deconstruct.result0(tmp1)),
       match.var(y = deconstruct.result1(tmp1)),
       comp(deconstruct.result2(tmp1) < 4),
   }
   ```

6) `expr is C(1, D(2, 3))`
    =>
    ```
    match.type.deconstruct(C c = expr) {
        comp(deconstruct.result0(c) == 1),
        match.type.deconstruct(D d = deconstruct.result1(c)) {
            comp(deconstruct.result0(d) == 2),
            comp(deconstruct.result1(d) == 2),
        }
    }
    ```
    
7) `x is >= 0 and var y and <= 100`
    ```
    match.and(tmp1 = x) {
        comp(tmp1 >= 0),
        match.var(y = tmp1),
        comp(tmp1 <= 100)
    }
    ```

8) `x is not C _`
    =>
    ```
    logic.not(
        match.type(C tmp1 = x) {}
    )
    ```

9) `expr is (var a, var b)` (when expr is object)
    =>
    ```
    match.type.deconstruct(ITuple tmp = expr) {
        match.var(a = deconstruct.result0(tmp)),
        match.var(b = deconstruct.result1(tmp)),
    }
    ```
    
10) `expr is (var a, var b)` (when expr is ValueTuple<int, int>)
    =>
    ```
    match.recursive(tmp = expr) {
        match.var(a = tmp.Item1),
        match.var(b = tmp.Item2),
    }
    ```