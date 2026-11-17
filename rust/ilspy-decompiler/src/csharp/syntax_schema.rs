//! The shape of the C# syntax tree the round trip of seam 2 sends: every node kind with its
//! child slots (in child order) and scalar properties. Generated from the node classes by
//! `SyntaxSchemaTests` (`Interop/SyntaxSchema.cs`); do not edit.

pub struct SlotSchema {
    pub name: &'static str,
    pub kind: &'static str,
    pub child: &'static str,
    pub collection: bool,
    pub optional: bool,
}

pub struct ScalarSchema {
    pub name: &'static str,
    pub ty: &'static str,
}

pub struct KindSchema {
    pub name: &'static str,
    pub slots: &'static [SlotSchema],
    pub scalars: &'static [ScalarSchema],
}

pub const KINDS: &[KindSchema] = &[
    KindSchema {
        name: "Accessor",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "Body", kind: "Body", child: "BlockStatement", collection: false, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
            ScalarSchema { name: "Kind", ty: "AccessorKind" },
            ScalarSchema { name: "IsInitOnly", ty: "Boolean" },
        ],
    },
    KindSchema {
        name: "AnonymousMethodExpression",
        slots: &[
            SlotSchema { name: "Parameters", kind: "Parameter", child: "ParameterDeclaration", collection: true, optional: true },
            SlotSchema { name: "Body", kind: "Body", child: "BlockStatement", collection: false, optional: false },
        ],
        scalars: &[
            ScalarSchema { name: "IsAsync", ty: "Boolean" },
        ],
    },
    KindSchema {
        name: "AnonymousTypeCreateExpression",
        slots: &[
            SlotSchema { name: "Initializers", kind: "Expression", child: "Expression", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "ArrayCreateExpression",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: true },
            SlotSchema { name: "Arguments", kind: "Argument", child: "Expression", collection: true, optional: true },
            SlotSchema { name: "AdditionalArraySpecifiers", kind: "AdditionalArraySpecifier", child: "ArraySpecifier", collection: true, optional: true },
            SlotSchema { name: "Initializer", kind: "Initializer", child: "ArrayInitializerExpression", collection: false, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "ArrayInitializerExpression",
        slots: &[
            SlotSchema { name: "Elements", kind: "Expression", child: "Expression", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "ArraySpecifier",
        slots: &[
        ],
        scalars: &[
            ScalarSchema { name: "Dimensions", ty: "Int32" },
        ],
    },
    KindSchema {
        name: "AsExpression",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "AssignmentExpression",
        slots: &[
            SlotSchema { name: "Left", kind: "Left", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "Right", kind: "Right", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
            ScalarSchema { name: "Operator", ty: "AssignmentOperatorType" },
        ],
    },
    KindSchema {
        name: "Attribute",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "Arguments", kind: "Argument", child: "Expression", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "HasArgumentList", ty: "Boolean" },
        ],
    },
    KindSchema {
        name: "AttributeSection",
        slots: &[
            SlotSchema { name: "AttributeTargetToken", kind: "Identifier", child: "Identifier", collection: false, optional: true },
            SlotSchema { name: "Attributes", kind: "Attribute", child: "Attribute", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "BaseReferenceExpression",
        slots: &[
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "BinaryOperatorExpression",
        slots: &[
            SlotSchema { name: "Left", kind: "Left", child: "Expression", collection: false, optional: true },
            SlotSchema { name: "Right", kind: "Right", child: "Expression", collection: false, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Operator", ty: "BinaryOperatorType" },
        ],
    },
    KindSchema {
        name: "BlockStatement",
        slots: &[
            SlotSchema { name: "Statements", kind: "Statement", child: "Statement", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "BreakStatement",
        slots: &[
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "CaseLabel",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "CastExpression",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "CatchClause",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: true },
            SlotSchema { name: "VariableNameToken", kind: "Identifier", child: "Identifier", collection: false, optional: true },
            SlotSchema { name: "Condition", kind: "Condition", child: "Expression", collection: false, optional: true },
            SlotSchema { name: "Body", kind: "Body", child: "BlockStatement", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "CheckedExpression",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "CheckedStatement",
        slots: &[
            SlotSchema { name: "Body", kind: "Body", child: "BlockStatement", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "Comment",
        slots: &[
        ],
        scalars: &[
            ScalarSchema { name: "CommentType", ty: "CommentType" },
            ScalarSchema { name: "Content", ty: "String" },
        ],
    },
    KindSchema {
        name: "ComposedType",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "BaseType", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "ArraySpecifiers", kind: "ArraySpecifier", child: "ArraySpecifier", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "HasRefSpecifier", ty: "Boolean" },
            ScalarSchema { name: "HasReadOnlySpecifier", ty: "Boolean" },
            ScalarSchema { name: "HasNullableSpecifier", ty: "Boolean" },
            ScalarSchema { name: "PointerRank", ty: "Int32" },
        ],
    },
    KindSchema {
        name: "ConditionalExpression",
        slots: &[
            SlotSchema { name: "Condition", kind: "Condition", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "TrueExpression", kind: "True", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "FalseExpression", kind: "False", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "Constraint",
        slots: &[
            SlotSchema { name: "TypeParameter", kind: "ConstraintTypeParameter", child: "SimpleType", collection: false, optional: false },
            SlotSchema { name: "BaseTypes", kind: "BaseType", child: "AstType", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "ConstructorDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "Parameters", kind: "Parameter", child: "ParameterDeclaration", collection: true, optional: true },
            SlotSchema { name: "Initializer", kind: "ConstructorInitializer", child: "ConstructorInitializer", collection: false, optional: true },
            SlotSchema { name: "Body", kind: "Body", child: "BlockStatement", collection: false, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
        ],
    },
    KindSchema {
        name: "ConstructorInitializer",
        slots: &[
            SlotSchema { name: "Arguments", kind: "Argument", child: "Expression", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "ConstructorInitializerType", ty: "ConstructorInitializerType" },
        ],
    },
    KindSchema {
        name: "ContinueStatement",
        slots: &[
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "CustomEventDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "ReturnType", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "PrivateImplementationType", kind: "PrivateImplementationType", child: "AstType", collection: false, optional: true },
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "AddAccessor", kind: "AddAccessor", child: "Accessor", collection: false, optional: true },
            SlotSchema { name: "RemoveAccessor", kind: "RemoveAccessor", child: "Accessor", collection: false, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
        ],
    },
    KindSchema {
        name: "DeclarationExpression",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "Designation", kind: "VariableDesignation", child: "VariableDesignation", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "DefaultValueExpression",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "DelegateDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "ReturnType", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "TypeParameters", kind: "TypeParameter", child: "TypeParameterDeclaration", collection: true, optional: true },
            SlotSchema { name: "Parameters", kind: "Parameter", child: "ParameterDeclaration", collection: true, optional: true },
            SlotSchema { name: "Constraints", kind: "Constraint", child: "Constraint", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
        ],
    },
    KindSchema {
        name: "DestructorDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "Body", kind: "Body", child: "BlockStatement", collection: false, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
        ],
    },
    KindSchema {
        name: "DirectionExpression",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
            ScalarSchema { name: "FieldDirection", ty: "FieldDirection" },
        ],
    },
    KindSchema {
        name: "DoWhileStatement",
        slots: &[
            SlotSchema { name: "EmbeddedStatement", kind: "EmbeddedStatement", child: "Statement", collection: false, optional: false },
            SlotSchema { name: "Condition", kind: "Condition", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "DocumentationReference",
        slots: &[
            SlotSchema { name: "DeclaringType", kind: "DeclaringType", child: "AstType", collection: false, optional: true },
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "ConversionOperatorReturnType", kind: "ConversionOperatorReturnType", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "TypeArguments", kind: "TypeArgument", child: "AstType", collection: true, optional: true },
            SlotSchema { name: "Parameters", kind: "Parameter", child: "ParameterDeclaration", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "SymbolKind", ty: "SymbolKind" },
            ScalarSchema { name: "OperatorType", ty: "OperatorType" },
            ScalarSchema { name: "HasParameterList", ty: "Boolean" },
        ],
    },
    KindSchema {
        name: "EmptyStatement",
        slots: &[
        ],
        scalars: &[
            ScalarSchema { name: "Location", ty: "TextLocation" },
        ],
    },
    KindSchema {
        name: "EnumMemberDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "Initializer", kind: "EnumMemberInitializer", child: "Expression", collection: false, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
        ],
    },
    KindSchema {
        name: "ErrorExpression",
        slots: &[
        ],
        scalars: &[
            ScalarSchema { name: "Location", ty: "TextLocation" },
        ],
    },
    KindSchema {
        name: "EventDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "ReturnType", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "Variables", kind: "Variable", child: "VariableInitializer", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
        ],
    },
    KindSchema {
        name: "ExpressionStatement",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "ExtensionDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "TypeParameters", kind: "TypeParameter", child: "TypeParameterDeclaration", collection: true, optional: true },
            SlotSchema { name: "ReceiverParameters", kind: "Parameter", child: "ParameterDeclaration", collection: true, optional: true },
            SlotSchema { name: "Constraints", kind: "Constraint", child: "Constraint", collection: true, optional: true },
            SlotSchema { name: "Members", kind: "TypeMember", child: "EntityDeclaration", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
        ],
    },
    KindSchema {
        name: "ExternAliasDeclaration",
        slots: &[
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "FieldDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "ReturnType", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "Variables", kind: "Variable", child: "VariableInitializer", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
        ],
    },
    KindSchema {
        name: "FixedFieldDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "ReturnType", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "Variables", kind: "FixedVariable", child: "FixedVariableInitializer", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
        ],
    },
    KindSchema {
        name: "FixedStatement",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "Variables", kind: "Variable", child: "VariableInitializer", collection: true, optional: true },
            SlotSchema { name: "EmbeddedStatement", kind: "EmbeddedStatement", child: "Statement", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "FixedVariableInitializer",
        slots: &[
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "CountExpression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "ForStatement",
        slots: &[
            SlotSchema { name: "Initializers", kind: "ForInitializer", child: "Statement", collection: true, optional: true },
            SlotSchema { name: "Condition", kind: "Condition", child: "Expression", collection: false, optional: true },
            SlotSchema { name: "Iterators", kind: "Iterator", child: "Statement", collection: true, optional: true },
            SlotSchema { name: "EmbeddedStatement", kind: "EmbeddedStatement", child: "Statement", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "ForeachStatement",
        slots: &[
            SlotSchema { name: "VariableType", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "VariableDesignation", kind: "VariableDesignation", child: "VariableDesignation", collection: false, optional: false },
            SlotSchema { name: "InExpression", kind: "Expression", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "EmbeddedStatement", kind: "EmbeddedStatement", child: "Statement", collection: false, optional: false },
        ],
        scalars: &[
            ScalarSchema { name: "IsAsync", ty: "Boolean" },
        ],
    },
    KindSchema {
        name: "FunctionPointerAstType",
        slots: &[
            SlotSchema { name: "CallingConventions", kind: "CallingConvention", child: "AstType", collection: true, optional: true },
            SlotSchema { name: "Parameters", kind: "Parameter", child: "ParameterDeclaration", collection: true, optional: true },
            SlotSchema { name: "ReturnType", kind: "Type", child: "AstType", collection: false, optional: false },
        ],
        scalars: &[
            ScalarSchema { name: "HasUnmanagedCallingConvention", ty: "Boolean" },
        ],
    },
    KindSchema {
        name: "GotoCaseStatement",
        slots: &[
            SlotSchema { name: "LabelExpression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "GotoDefaultStatement",
        slots: &[
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "GotoStatement",
        slots: &[
            SlotSchema { name: "LabelToken", kind: "Identifier", child: "Identifier", collection: false, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "Identifier",
        slots: &[
        ],
        scalars: &[
            ScalarSchema { name: "Name", ty: "String" },
            ScalarSchema { name: "IsVerbatim", ty: "Boolean" },
        ],
    },
    KindSchema {
        name: "IdentifierExpression",
        slots: &[
            SlotSchema { name: "IdentifierToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "TypeArguments", kind: "TypeArgument", child: "AstType", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "IfElseStatement",
        slots: &[
            SlotSchema { name: "Condition", kind: "Condition", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "TrueStatement", kind: "TrueStatement", child: "Statement", collection: false, optional: false },
            SlotSchema { name: "FalseStatement", kind: "FalseStatement", child: "Statement", collection: false, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "IndexerDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "ReturnType", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "PrivateImplementationType", kind: "PrivateImplementationType", child: "AstType", collection: false, optional: true },
            SlotSchema { name: "Parameters", kind: "Parameter", child: "ParameterDeclaration", collection: true, optional: true },
            SlotSchema { name: "Getter", kind: "Getter", child: "Accessor", collection: false, optional: true },
            SlotSchema { name: "Setter", kind: "Setter", child: "Accessor", collection: false, optional: true },
            SlotSchema { name: "ExpressionBody", kind: "ExpressionBody", child: "Expression", collection: false, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
        ],
    },
    KindSchema {
        name: "IndexerExpression",
        slots: &[
            SlotSchema { name: "Target", kind: "TargetExpression", child: "Expression", collection: false, optional: true },
            SlotSchema { name: "Arguments", kind: "Argument", child: "Expression", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "InterpolatedStringExpression",
        slots: &[
            SlotSchema { name: "Content", kind: "Content", child: "InterpolatedStringContent", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "InterpolatedStringText",
        slots: &[
        ],
        scalars: &[
            ScalarSchema { name: "Text", ty: "String" },
        ],
    },
    KindSchema {
        name: "Interpolation",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
            ScalarSchema { name: "Alignment", ty: "Int32" },
            ScalarSchema { name: "Suffix", ty: "String" },
        ],
    },
    KindSchema {
        name: "InvocationAstType",
        slots: &[
            SlotSchema { name: "Arguments", kind: "Expression", child: "Expression", collection: true, optional: true },
            SlotSchema { name: "BaseType", kind: "Type", child: "AstType", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "InvocationExpression",
        slots: &[
            SlotSchema { name: "Target", kind: "TargetExpression", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "Arguments", kind: "Argument", child: "Expression", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "IsExpression",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "LabelStatement",
        slots: &[
            SlotSchema { name: "LabelToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "LambdaExpression",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "Parameters", kind: "Parameter", child: "ParameterDeclaration", collection: true, optional: true },
            SlotSchema { name: "Body", kind: "LambdaBody", child: "AstNode", collection: false, optional: false },
        ],
        scalars: &[
            ScalarSchema { name: "IsAsync", ty: "Boolean" },
        ],
    },
    KindSchema {
        name: "LinePreprocessorDirective",
        slots: &[
        ],
        scalars: &[
            ScalarSchema { name: "Type", ty: "PreProcessorDirectiveType" },
            ScalarSchema { name: "Argument", ty: "String" },
        ],
    },
    KindSchema {
        name: "LocalFunctionDeclarationStatement",
        slots: &[
            SlotSchema { name: "Declaration", kind: "MethodDeclaration", child: "MethodDeclaration", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "LockStatement",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "EmbeddedStatement", kind: "EmbeddedStatement", child: "Statement", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "MemberReferenceExpression",
        slots: &[
            SlotSchema { name: "Target", kind: "TargetExpression", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "MemberNameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "TypeArguments", kind: "TypeArgument", child: "AstType", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "MemberType",
        slots: &[
            SlotSchema { name: "Target", kind: "Target", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "MemberNameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "TypeArguments", kind: "TypeArgument", child: "AstType", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "IsDoubleColon", ty: "Boolean" },
        ],
    },
    KindSchema {
        name: "MethodDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "ReturnType", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "PrivateImplementationType", kind: "PrivateImplementationType", child: "AstType", collection: false, optional: true },
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "TypeParameters", kind: "TypeParameter", child: "TypeParameterDeclaration", collection: true, optional: true },
            SlotSchema { name: "Parameters", kind: "Parameter", child: "ParameterDeclaration", collection: true, optional: true },
            SlotSchema { name: "Constraints", kind: "Constraint", child: "Constraint", collection: true, optional: true },
            SlotSchema { name: "Body", kind: "Body", child: "BlockStatement", collection: false, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
        ],
    },
    KindSchema {
        name: "NamedArgumentExpression",
        slots: &[
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "NamedExpression",
        slots: &[
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "NamespaceDeclaration",
        slots: &[
            SlotSchema { name: "NamespaceName", kind: "NamespaceName", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "Members", kind: "Member", child: "AstNode", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "IsFileScoped", ty: "Boolean" },
        ],
    },
    KindSchema {
        name: "NullReferenceExpression",
        slots: &[
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "ObjectCreateExpression",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "Arguments", kind: "Argument", child: "Expression", collection: true, optional: true },
            SlotSchema { name: "Initializer", kind: "Initializer", child: "ArrayInitializerExpression", collection: false, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "OperatorDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "ReturnType", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "PrivateImplementationType", kind: "PrivateImplementationType", child: "AstType", collection: false, optional: true },
            SlotSchema { name: "Parameters", kind: "Parameter", child: "ParameterDeclaration", collection: true, optional: true },
            SlotSchema { name: "Body", kind: "Body", child: "BlockStatement", collection: false, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
            ScalarSchema { name: "OperatorType", ty: "OperatorType" },
        ],
    },
    KindSchema {
        name: "OutVarDeclarationExpression",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "Variable", kind: "Variable", child: "VariableInitializer", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "ParameterDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: true },
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: true },
            SlotSchema { name: "DefaultExpression", kind: "Expression", child: "Expression", collection: false, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "HasThisModifier", ty: "Boolean" },
            ScalarSchema { name: "IsParams", ty: "Boolean" },
            ScalarSchema { name: "IsScopedRef", ty: "Boolean" },
            ScalarSchema { name: "ParameterModifier", ty: "ReferenceKind" },
        ],
    },
    KindSchema {
        name: "ParenthesizedExpression",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "ParenthesizedVariableDesignation",
        slots: &[
            SlotSchema { name: "VariableDesignations", kind: "VariableDesignation", child: "VariableDesignation", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "PointerReferenceExpression",
        slots: &[
            SlotSchema { name: "Target", kind: "TargetExpression", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "MemberNameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "TypeArguments", kind: "TypeArgument", child: "AstType", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "PragmaWarningPreprocessorDirective",
        slots: &[
            SlotSchema { name: "Warnings", kind: "Warning", child: "PrimitiveExpression", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Type", ty: "PreProcessorDirectiveType" },
            ScalarSchema { name: "Argument", ty: "String" },
        ],
    },
    KindSchema {
        name: "PreProcessorDirective",
        slots: &[
        ],
        scalars: &[
            ScalarSchema { name: "Type", ty: "PreProcessorDirectiveType" },
            ScalarSchema { name: "Argument", ty: "String" },
        ],
    },
    KindSchema {
        name: "PrimitiveExpression",
        slots: &[
        ],
        scalars: &[
            ScalarSchema { name: "Value", ty: "Object" },
            ScalarSchema { name: "Format", ty: "LiteralFormat" },
        ],
    },
    KindSchema {
        name: "PrimitiveType",
        slots: &[
        ],
        scalars: &[
            ScalarSchema { name: "Keyword", ty: "String" },
        ],
    },
    KindSchema {
        name: "PropertyDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "ReturnType", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "PrivateImplementationType", kind: "PrivateImplementationType", child: "AstType", collection: false, optional: true },
            SlotSchema { name: "Getter", kind: "Getter", child: "Accessor", collection: false, optional: true },
            SlotSchema { name: "Setter", kind: "Setter", child: "Accessor", collection: false, optional: true },
            SlotSchema { name: "Initializer", kind: "Expression", child: "Expression", collection: false, optional: true },
            SlotSchema { name: "ExpressionBody", kind: "ExpressionBody", child: "Expression", collection: false, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
        ],
    },
    KindSchema {
        name: "QueryContinuationClause",
        slots: &[
            SlotSchema { name: "PrecedingQuery", kind: "PrecedingQuery", child: "QueryExpression", collection: false, optional: false },
            SlotSchema { name: "IdentifierToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "QueryExpression",
        slots: &[
            SlotSchema { name: "Clauses", kind: "Clause", child: "QueryClause", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "QueryFromClause",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: true },
            SlotSchema { name: "IdentifierToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "QueryGroupClause",
        slots: &[
            SlotSchema { name: "Projection", kind: "Projection", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "Key", kind: "Key", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "QueryJoinClause",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: true },
            SlotSchema { name: "JoinIdentifierToken", kind: "JoinIdentifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "InExpression", kind: "InExpression", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "OnExpression", kind: "OnExpression", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "EqualsExpression", kind: "EqualsExpression", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "IntoIdentifierToken", kind: "IntoIdentifier", child: "Identifier", collection: false, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "QueryLetClause",
        slots: &[
            SlotSchema { name: "IdentifierToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "QueryOrderClause",
        slots: &[
            SlotSchema { name: "Orderings", kind: "Ordering", child: "QueryOrdering", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "QueryOrdering",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
            ScalarSchema { name: "Direction", ty: "QueryOrderingDirection" },
        ],
    },
    KindSchema {
        name: "QuerySelectClause",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "QueryWhereClause",
        slots: &[
            SlotSchema { name: "Condition", kind: "Condition", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "RecursivePatternExpression",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: true },
            SlotSchema { name: "SubPatterns", kind: "SubPattern", child: "Expression", collection: true, optional: true },
            SlotSchema { name: "Designation", kind: "VariableDesignation", child: "VariableDesignation", collection: false, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "IsPositional", ty: "Boolean" },
        ],
    },
    KindSchema {
        name: "ReturnStatement",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "SimpleType",
        slots: &[
            SlotSchema { name: "IdentifierToken", kind: "Identifier", child: "Identifier", collection: false, optional: true },
            SlotSchema { name: "TypeArguments", kind: "TypeArgument", child: "AstType", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "SingleVariableDesignation",
        slots: &[
            SlotSchema { name: "IdentifierToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "SizeOfExpression",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "StackAllocExpression",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: true },
            SlotSchema { name: "CountExpression", kind: "Expression", child: "Expression", collection: false, optional: true },
            SlotSchema { name: "Initializer", kind: "Initializer", child: "ArrayInitializerExpression", collection: false, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "SwitchExpression",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "SwitchSections", kind: "SwitchExpressionSection", child: "SwitchExpressionSection", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "SwitchExpressionSection",
        slots: &[
            SlotSchema { name: "Pattern", kind: "Pattern", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "Body", kind: "SwitchExpressionBody", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "SwitchSection",
        slots: &[
            SlotSchema { name: "CaseLabels", kind: "CaseLabel", child: "CaseLabel", collection: true, optional: true },
            SlotSchema { name: "Statements", kind: "EmbeddedStatement", child: "Statement", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "SwitchStatement",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "SwitchSections", kind: "SwitchSection", child: "SwitchSection", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "SyntaxTree",
        slots: &[
            SlotSchema { name: "Members", kind: "Member", child: "AstNode", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "ThisReferenceExpression",
        slots: &[
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "ThrowExpression",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "ThrowStatement",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "TryCatchStatement",
        slots: &[
            SlotSchema { name: "TryBlock", kind: "TryBlock", child: "BlockStatement", collection: false, optional: false },
            SlotSchema { name: "CatchClauses", kind: "CatchClause", child: "CatchClause", collection: true, optional: true },
            SlotSchema { name: "FinallyBlock", kind: "FinallyBlock", child: "BlockStatement", collection: false, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "TupleAstType",
        slots: &[
            SlotSchema { name: "Elements", kind: "Element", child: "TupleTypeElement", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "TupleExpression",
        slots: &[
            SlotSchema { name: "Elements", kind: "Expression", child: "Expression", collection: true, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "TupleTypeElement",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "TypeDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "TypeParameters", kind: "TypeParameter", child: "TypeParameterDeclaration", collection: true, optional: true },
            SlotSchema { name: "PrimaryConstructorParameters", kind: "Parameter", child: "ParameterDeclaration", collection: true, optional: true },
            SlotSchema { name: "BaseTypes", kind: "BaseType", child: "AstType", collection: true, optional: true },
            SlotSchema { name: "Constraints", kind: "Constraint", child: "Constraint", collection: true, optional: true },
            SlotSchema { name: "Members", kind: "TypeMember", child: "EntityDeclaration", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
            ScalarSchema { name: "ClassType", ty: "ClassType" },
            ScalarSchema { name: "HasPrimaryConstructor", ty: "Boolean" },
        ],
    },
    KindSchema {
        name: "TypeOfExpression",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "TypeParameterDeclaration",
        slots: &[
            SlotSchema { name: "Attributes", kind: "AttributeSection", child: "AttributeSection", collection: true, optional: true },
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
        ],
        scalars: &[
            ScalarSchema { name: "Variance", ty: "VarianceModifier" },
        ],
    },
    KindSchema {
        name: "TypeReferenceExpression",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "UnaryOperatorExpression",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
            ScalarSchema { name: "Operator", ty: "UnaryOperatorType" },
        ],
    },
    KindSchema {
        name: "UncheckedExpression",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "UncheckedStatement",
        slots: &[
            SlotSchema { name: "Body", kind: "Body", child: "BlockStatement", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "UndocumentedExpression",
        slots: &[
            SlotSchema { name: "Arguments", kind: "Argument", child: "Expression", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "UndocumentedExpressionType", ty: "UndocumentedExpressionType" },
        ],
    },
    KindSchema {
        name: "UnsafeStatement",
        slots: &[
            SlotSchema { name: "Body", kind: "Body", child: "BlockStatement", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "UsingAliasDeclaration",
        slots: &[
            SlotSchema { name: "AliasToken", kind: "Alias", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "Import", kind: "Import", child: "AstType", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "UsingDeclaration",
        slots: &[
            SlotSchema { name: "Import", kind: "Import", child: "AstType", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "UsingStatement",
        slots: &[
            SlotSchema { name: "ResourceAcquisition", kind: "ResourceAcquisition", child: "AstNode", collection: false, optional: false },
            SlotSchema { name: "EmbeddedStatement", kind: "EmbeddedStatement", child: "Statement", collection: false, optional: false },
        ],
        scalars: &[
            ScalarSchema { name: "IsAsync", ty: "Boolean" },
            ScalarSchema { name: "IsEnhanced", ty: "Boolean" },
        ],
    },
    KindSchema {
        name: "VariableDeclarationStatement",
        slots: &[
            SlotSchema { name: "Type", kind: "Type", child: "AstType", collection: false, optional: false },
            SlotSchema { name: "Variables", kind: "Variable", child: "VariableInitializer", collection: true, optional: true },
        ],
        scalars: &[
            ScalarSchema { name: "Modifiers", ty: "Modifiers" },
        ],
    },
    KindSchema {
        name: "VariableInitializer",
        slots: &[
            SlotSchema { name: "NameToken", kind: "Identifier", child: "Identifier", collection: false, optional: false },
            SlotSchema { name: "Initializer", kind: "Expression", child: "Expression", collection: false, optional: true },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "WhileStatement",
        slots: &[
            SlotSchema { name: "Condition", kind: "Condition", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "EmbeddedStatement", kind: "EmbeddedStatement", child: "Statement", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "WithInitializerExpression",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
            SlotSchema { name: "Initializer", kind: "Initializer", child: "ArrayInitializerExpression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "YieldBreakStatement",
        slots: &[
        ],
        scalars: &[
        ],
    },
    KindSchema {
        name: "YieldReturnStatement",
        slots: &[
            SlotSchema { name: "Expression", kind: "Expression", child: "Expression", collection: false, optional: false },
        ],
        scalars: &[
        ],
    },
];

/// The kind index of each node class.
pub mod kind {
    pub const ACCESSOR: u16 = 0;
    pub const ANONYMOUS_METHOD_EXPRESSION: u16 = 1;
    pub const ANONYMOUS_TYPE_CREATE_EXPRESSION: u16 = 2;
    pub const ARRAY_CREATE_EXPRESSION: u16 = 3;
    pub const ARRAY_INITIALIZER_EXPRESSION: u16 = 4;
    pub const ARRAY_SPECIFIER: u16 = 5;
    pub const AS_EXPRESSION: u16 = 6;
    pub const ASSIGNMENT_EXPRESSION: u16 = 7;
    pub const ATTRIBUTE: u16 = 8;
    pub const ATTRIBUTE_SECTION: u16 = 9;
    pub const BASE_REFERENCE_EXPRESSION: u16 = 10;
    pub const BINARY_OPERATOR_EXPRESSION: u16 = 11;
    pub const BLOCK_STATEMENT: u16 = 12;
    pub const BREAK_STATEMENT: u16 = 13;
    pub const CASE_LABEL: u16 = 14;
    pub const CAST_EXPRESSION: u16 = 15;
    pub const CATCH_CLAUSE: u16 = 16;
    pub const CHECKED_EXPRESSION: u16 = 17;
    pub const CHECKED_STATEMENT: u16 = 18;
    pub const COMMENT: u16 = 19;
    pub const COMPOSED_TYPE: u16 = 20;
    pub const CONDITIONAL_EXPRESSION: u16 = 21;
    pub const CONSTRAINT: u16 = 22;
    pub const CONSTRUCTOR_DECLARATION: u16 = 23;
    pub const CONSTRUCTOR_INITIALIZER: u16 = 24;
    pub const CONTINUE_STATEMENT: u16 = 25;
    pub const CUSTOM_EVENT_DECLARATION: u16 = 26;
    pub const DECLARATION_EXPRESSION: u16 = 27;
    pub const DEFAULT_VALUE_EXPRESSION: u16 = 28;
    pub const DELEGATE_DECLARATION: u16 = 29;
    pub const DESTRUCTOR_DECLARATION: u16 = 30;
    pub const DIRECTION_EXPRESSION: u16 = 31;
    pub const DO_WHILE_STATEMENT: u16 = 32;
    pub const DOCUMENTATION_REFERENCE: u16 = 33;
    pub const EMPTY_STATEMENT: u16 = 34;
    pub const ENUM_MEMBER_DECLARATION: u16 = 35;
    pub const ERROR_EXPRESSION: u16 = 36;
    pub const EVENT_DECLARATION: u16 = 37;
    pub const EXPRESSION_STATEMENT: u16 = 38;
    pub const EXTENSION_DECLARATION: u16 = 39;
    pub const EXTERN_ALIAS_DECLARATION: u16 = 40;
    pub const FIELD_DECLARATION: u16 = 41;
    pub const FIXED_FIELD_DECLARATION: u16 = 42;
    pub const FIXED_STATEMENT: u16 = 43;
    pub const FIXED_VARIABLE_INITIALIZER: u16 = 44;
    pub const FOR_STATEMENT: u16 = 45;
    pub const FOREACH_STATEMENT: u16 = 46;
    pub const FUNCTION_POINTER_AST_TYPE: u16 = 47;
    pub const GOTO_CASE_STATEMENT: u16 = 48;
    pub const GOTO_DEFAULT_STATEMENT: u16 = 49;
    pub const GOTO_STATEMENT: u16 = 50;
    pub const IDENTIFIER: u16 = 51;
    pub const IDENTIFIER_EXPRESSION: u16 = 52;
    pub const IF_ELSE_STATEMENT: u16 = 53;
    pub const INDEXER_DECLARATION: u16 = 54;
    pub const INDEXER_EXPRESSION: u16 = 55;
    pub const INTERPOLATED_STRING_EXPRESSION: u16 = 56;
    pub const INTERPOLATED_STRING_TEXT: u16 = 57;
    pub const INTERPOLATION: u16 = 58;
    pub const INVOCATION_AST_TYPE: u16 = 59;
    pub const INVOCATION_EXPRESSION: u16 = 60;
    pub const IS_EXPRESSION: u16 = 61;
    pub const LABEL_STATEMENT: u16 = 62;
    pub const LAMBDA_EXPRESSION: u16 = 63;
    pub const LINE_PREPROCESSOR_DIRECTIVE: u16 = 64;
    pub const LOCAL_FUNCTION_DECLARATION_STATEMENT: u16 = 65;
    pub const LOCK_STATEMENT: u16 = 66;
    pub const MEMBER_REFERENCE_EXPRESSION: u16 = 67;
    pub const MEMBER_TYPE: u16 = 68;
    pub const METHOD_DECLARATION: u16 = 69;
    pub const NAMED_ARGUMENT_EXPRESSION: u16 = 70;
    pub const NAMED_EXPRESSION: u16 = 71;
    pub const NAMESPACE_DECLARATION: u16 = 72;
    pub const NULL_REFERENCE_EXPRESSION: u16 = 73;
    pub const OBJECT_CREATE_EXPRESSION: u16 = 74;
    pub const OPERATOR_DECLARATION: u16 = 75;
    pub const OUT_VAR_DECLARATION_EXPRESSION: u16 = 76;
    pub const PARAMETER_DECLARATION: u16 = 77;
    pub const PARENTHESIZED_EXPRESSION: u16 = 78;
    pub const PARENTHESIZED_VARIABLE_DESIGNATION: u16 = 79;
    pub const POINTER_REFERENCE_EXPRESSION: u16 = 80;
    pub const PRAGMA_WARNING_PREPROCESSOR_DIRECTIVE: u16 = 81;
    pub const PRE_PROCESSOR_DIRECTIVE: u16 = 82;
    pub const PRIMITIVE_EXPRESSION: u16 = 83;
    pub const PRIMITIVE_TYPE: u16 = 84;
    pub const PROPERTY_DECLARATION: u16 = 85;
    pub const QUERY_CONTINUATION_CLAUSE: u16 = 86;
    pub const QUERY_EXPRESSION: u16 = 87;
    pub const QUERY_FROM_CLAUSE: u16 = 88;
    pub const QUERY_GROUP_CLAUSE: u16 = 89;
    pub const QUERY_JOIN_CLAUSE: u16 = 90;
    pub const QUERY_LET_CLAUSE: u16 = 91;
    pub const QUERY_ORDER_CLAUSE: u16 = 92;
    pub const QUERY_ORDERING: u16 = 93;
    pub const QUERY_SELECT_CLAUSE: u16 = 94;
    pub const QUERY_WHERE_CLAUSE: u16 = 95;
    pub const RECURSIVE_PATTERN_EXPRESSION: u16 = 96;
    pub const RETURN_STATEMENT: u16 = 97;
    pub const SIMPLE_TYPE: u16 = 98;
    pub const SINGLE_VARIABLE_DESIGNATION: u16 = 99;
    pub const SIZE_OF_EXPRESSION: u16 = 100;
    pub const STACK_ALLOC_EXPRESSION: u16 = 101;
    pub const SWITCH_EXPRESSION: u16 = 102;
    pub const SWITCH_EXPRESSION_SECTION: u16 = 103;
    pub const SWITCH_SECTION: u16 = 104;
    pub const SWITCH_STATEMENT: u16 = 105;
    pub const SYNTAX_TREE: u16 = 106;
    pub const THIS_REFERENCE_EXPRESSION: u16 = 107;
    pub const THROW_EXPRESSION: u16 = 108;
    pub const THROW_STATEMENT: u16 = 109;
    pub const TRY_CATCH_STATEMENT: u16 = 110;
    pub const TUPLE_AST_TYPE: u16 = 111;
    pub const TUPLE_EXPRESSION: u16 = 112;
    pub const TUPLE_TYPE_ELEMENT: u16 = 113;
    pub const TYPE_DECLARATION: u16 = 114;
    pub const TYPE_OF_EXPRESSION: u16 = 115;
    pub const TYPE_PARAMETER_DECLARATION: u16 = 116;
    pub const TYPE_REFERENCE_EXPRESSION: u16 = 117;
    pub const UNARY_OPERATOR_EXPRESSION: u16 = 118;
    pub const UNCHECKED_EXPRESSION: u16 = 119;
    pub const UNCHECKED_STATEMENT: u16 = 120;
    pub const UNDOCUMENTED_EXPRESSION: u16 = 121;
    pub const UNSAFE_STATEMENT: u16 = 122;
    pub const USING_ALIAS_DECLARATION: u16 = 123;
    pub const USING_DECLARATION: u16 = 124;
    pub const USING_STATEMENT: u16 = 125;
    pub const VARIABLE_DECLARATION_STATEMENT: u16 = 126;
    pub const VARIABLE_INITIALIZER: u16 = 127;
    pub const WHILE_STATEMENT: u16 = 128;
    pub const WITH_INITIALIZER_EXPRESSION: u16 = 129;
    pub const YIELD_BREAK_STATEMENT: u16 = 130;
    pub const YIELD_RETURN_STATEMENT: u16 = 131;
}

/// The slot and scalar indices of `Accessor`.
pub mod accessor {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const BODY: usize = 1;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
        pub const KIND: usize = 1;
        pub const IS_INIT_ONLY: usize = 2;
    }
}

/// The slot and scalar indices of `AnonymousMethodExpression`.
pub mod anonymous_method_expression {
    pub mod slot {
        pub const PARAMETERS: usize = 0;
        pub const BODY: usize = 1;
    }
    pub mod scalar {
        pub const IS_ASYNC: usize = 0;
    }
}

/// The slot and scalar indices of `AnonymousTypeCreateExpression`.
pub mod anonymous_type_create_expression {
    pub mod slot {
        pub const INITIALIZERS: usize = 0;
    }
}

/// The slot and scalar indices of `ArrayCreateExpression`.
pub mod array_create_expression {
    pub mod slot {
        pub const TYPE: usize = 0;
        pub const ARGUMENTS: usize = 1;
        pub const ADDITIONAL_ARRAY_SPECIFIERS: usize = 2;
        pub const INITIALIZER: usize = 3;
    }
}

/// The slot and scalar indices of `ArrayInitializerExpression`.
pub mod array_initializer_expression {
    pub mod slot {
        pub const ELEMENTS: usize = 0;
    }
}

/// The slot and scalar indices of `ArraySpecifier`.
pub mod array_specifier {
    pub mod scalar {
        pub const DIMENSIONS: usize = 0;
    }
}

/// The slot and scalar indices of `AsExpression`.
pub mod as_expression {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
        pub const TYPE: usize = 1;
    }
}

/// The slot and scalar indices of `AssignmentExpression`.
pub mod assignment_expression {
    pub mod slot {
        pub const LEFT: usize = 0;
        pub const RIGHT: usize = 1;
    }
    pub mod scalar {
        pub const OPERATOR: usize = 0;
    }
}

/// The slot and scalar indices of `Attribute`.
pub mod attribute {
    pub mod slot {
        pub const TYPE: usize = 0;
        pub const ARGUMENTS: usize = 1;
    }
    pub mod scalar {
        pub const HAS_ARGUMENT_LIST: usize = 0;
    }
}

/// The slot and scalar indices of `AttributeSection`.
pub mod attribute_section {
    pub mod slot {
        pub const ATTRIBUTE_TARGET_TOKEN: usize = 0;
        pub const ATTRIBUTES: usize = 1;
    }
}

/// The slot and scalar indices of `BinaryOperatorExpression`.
pub mod binary_operator_expression {
    pub mod slot {
        pub const LEFT: usize = 0;
        pub const RIGHT: usize = 1;
    }
    pub mod scalar {
        pub const OPERATOR: usize = 0;
    }
}

/// The slot and scalar indices of `BlockStatement`.
pub mod block_statement {
    pub mod slot {
        pub const STATEMENTS: usize = 0;
    }
}

/// The slot and scalar indices of `CaseLabel`.
pub mod case_label {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
    }
}

/// The slot and scalar indices of `CastExpression`.
pub mod cast_expression {
    pub mod slot {
        pub const TYPE: usize = 0;
        pub const EXPRESSION: usize = 1;
    }
}

/// The slot and scalar indices of `CatchClause`.
pub mod catch_clause {
    pub mod slot {
        pub const TYPE: usize = 0;
        pub const VARIABLE_NAME_TOKEN: usize = 1;
        pub const CONDITION: usize = 2;
        pub const BODY: usize = 3;
    }
}

/// The slot and scalar indices of `CheckedExpression`.
pub mod checked_expression {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
    }
}

/// The slot and scalar indices of `CheckedStatement`.
pub mod checked_statement {
    pub mod slot {
        pub const BODY: usize = 0;
    }
}

/// The slot and scalar indices of `Comment`.
pub mod comment {
    pub mod scalar {
        pub const COMMENT_TYPE: usize = 0;
        pub const CONTENT: usize = 1;
    }
}

/// The slot and scalar indices of `ComposedType`.
pub mod composed_type {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const BASE_TYPE: usize = 1;
        pub const ARRAY_SPECIFIERS: usize = 2;
    }
    pub mod scalar {
        pub const HAS_REF_SPECIFIER: usize = 0;
        pub const HAS_READ_ONLY_SPECIFIER: usize = 1;
        pub const HAS_NULLABLE_SPECIFIER: usize = 2;
        pub const POINTER_RANK: usize = 3;
    }
}

/// The slot and scalar indices of `ConditionalExpression`.
pub mod conditional_expression {
    pub mod slot {
        pub const CONDITION: usize = 0;
        pub const TRUE_EXPRESSION: usize = 1;
        pub const FALSE_EXPRESSION: usize = 2;
    }
}

/// The slot and scalar indices of `Constraint`.
pub mod constraint {
    pub mod slot {
        pub const TYPE_PARAMETER: usize = 0;
        pub const BASE_TYPES: usize = 1;
    }
}

/// The slot and scalar indices of `ConstructorDeclaration`.
pub mod constructor_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const NAME_TOKEN: usize = 1;
        pub const PARAMETERS: usize = 2;
        pub const INITIALIZER: usize = 3;
        pub const BODY: usize = 4;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
    }
}

/// The slot and scalar indices of `ConstructorInitializer`.
pub mod constructor_initializer {
    pub mod slot {
        pub const ARGUMENTS: usize = 0;
    }
    pub mod scalar {
        pub const CONSTRUCTOR_INITIALIZER_TYPE: usize = 0;
    }
}

/// The slot and scalar indices of `CustomEventDeclaration`.
pub mod custom_event_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const RETURN_TYPE: usize = 1;
        pub const PRIVATE_IMPLEMENTATION_TYPE: usize = 2;
        pub const NAME_TOKEN: usize = 3;
        pub const ADD_ACCESSOR: usize = 4;
        pub const REMOVE_ACCESSOR: usize = 5;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
    }
}

/// The slot and scalar indices of `DeclarationExpression`.
pub mod declaration_expression {
    pub mod slot {
        pub const TYPE: usize = 0;
        pub const DESIGNATION: usize = 1;
    }
}

/// The slot and scalar indices of `DefaultValueExpression`.
pub mod default_value_expression {
    pub mod slot {
        pub const TYPE: usize = 0;
    }
}

/// The slot and scalar indices of `DelegateDeclaration`.
pub mod delegate_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const RETURN_TYPE: usize = 1;
        pub const NAME_TOKEN: usize = 2;
        pub const TYPE_PARAMETERS: usize = 3;
        pub const PARAMETERS: usize = 4;
        pub const CONSTRAINTS: usize = 5;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
    }
}

/// The slot and scalar indices of `DestructorDeclaration`.
pub mod destructor_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const NAME_TOKEN: usize = 1;
        pub const BODY: usize = 2;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
    }
}

/// The slot and scalar indices of `DirectionExpression`.
pub mod direction_expression {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
    }
    pub mod scalar {
        pub const FIELD_DIRECTION: usize = 0;
    }
}

/// The slot and scalar indices of `DoWhileStatement`.
pub mod do_while_statement {
    pub mod slot {
        pub const EMBEDDED_STATEMENT: usize = 0;
        pub const CONDITION: usize = 1;
    }
}

/// The slot and scalar indices of `DocumentationReference`.
pub mod documentation_reference {
    pub mod slot {
        pub const DECLARING_TYPE: usize = 0;
        pub const NAME_TOKEN: usize = 1;
        pub const CONVERSION_OPERATOR_RETURN_TYPE: usize = 2;
        pub const TYPE_ARGUMENTS: usize = 3;
        pub const PARAMETERS: usize = 4;
    }
    pub mod scalar {
        pub const SYMBOL_KIND: usize = 0;
        pub const OPERATOR_TYPE: usize = 1;
        pub const HAS_PARAMETER_LIST: usize = 2;
    }
}

/// The slot and scalar indices of `EmptyStatement`.
pub mod empty_statement {
    pub mod scalar {
        pub const LOCATION: usize = 0;
    }
}

/// The slot and scalar indices of `EnumMemberDeclaration`.
pub mod enum_member_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const NAME_TOKEN: usize = 1;
        pub const INITIALIZER: usize = 2;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
    }
}

/// The slot and scalar indices of `ErrorExpression`.
pub mod error_expression {
    pub mod scalar {
        pub const LOCATION: usize = 0;
    }
}

/// The slot and scalar indices of `EventDeclaration`.
pub mod event_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const RETURN_TYPE: usize = 1;
        pub const VARIABLES: usize = 2;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
    }
}

/// The slot and scalar indices of `ExpressionStatement`.
pub mod expression_statement {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
    }
}

/// The slot and scalar indices of `ExtensionDeclaration`.
pub mod extension_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const TYPE_PARAMETERS: usize = 1;
        pub const RECEIVER_PARAMETERS: usize = 2;
        pub const CONSTRAINTS: usize = 3;
        pub const MEMBERS: usize = 4;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
    }
}

/// The slot and scalar indices of `ExternAliasDeclaration`.
pub mod extern_alias_declaration {
    pub mod slot {
        pub const NAME_TOKEN: usize = 0;
    }
}

/// The slot and scalar indices of `FieldDeclaration`.
pub mod field_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const RETURN_TYPE: usize = 1;
        pub const VARIABLES: usize = 2;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
    }
}

/// The slot and scalar indices of `FixedFieldDeclaration`.
pub mod fixed_field_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const RETURN_TYPE: usize = 1;
        pub const VARIABLES: usize = 2;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
    }
}

/// The slot and scalar indices of `FixedStatement`.
pub mod fixed_statement {
    pub mod slot {
        pub const TYPE: usize = 0;
        pub const VARIABLES: usize = 1;
        pub const EMBEDDED_STATEMENT: usize = 2;
    }
}

/// The slot and scalar indices of `FixedVariableInitializer`.
pub mod fixed_variable_initializer {
    pub mod slot {
        pub const NAME_TOKEN: usize = 0;
        pub const COUNT_EXPRESSION: usize = 1;
    }
}

/// The slot and scalar indices of `ForStatement`.
pub mod for_statement {
    pub mod slot {
        pub const INITIALIZERS: usize = 0;
        pub const CONDITION: usize = 1;
        pub const ITERATORS: usize = 2;
        pub const EMBEDDED_STATEMENT: usize = 3;
    }
}

/// The slot and scalar indices of `ForeachStatement`.
pub mod foreach_statement {
    pub mod slot {
        pub const VARIABLE_TYPE: usize = 0;
        pub const VARIABLE_DESIGNATION: usize = 1;
        pub const IN_EXPRESSION: usize = 2;
        pub const EMBEDDED_STATEMENT: usize = 3;
    }
    pub mod scalar {
        pub const IS_ASYNC: usize = 0;
    }
}

/// The slot and scalar indices of `FunctionPointerAstType`.
pub mod function_pointer_ast_type {
    pub mod slot {
        pub const CALLING_CONVENTIONS: usize = 0;
        pub const PARAMETERS: usize = 1;
        pub const RETURN_TYPE: usize = 2;
    }
    pub mod scalar {
        pub const HAS_UNMANAGED_CALLING_CONVENTION: usize = 0;
    }
}

/// The slot and scalar indices of `GotoCaseStatement`.
pub mod goto_case_statement {
    pub mod slot {
        pub const LABEL_EXPRESSION: usize = 0;
    }
}

/// The slot and scalar indices of `GotoStatement`.
pub mod goto_statement {
    pub mod slot {
        pub const LABEL_TOKEN: usize = 0;
    }
}

/// The slot and scalar indices of `Identifier`.
pub mod identifier {
    pub mod scalar {
        pub const NAME: usize = 0;
        pub const IS_VERBATIM: usize = 1;
    }
}

/// The slot and scalar indices of `IdentifierExpression`.
pub mod identifier_expression {
    pub mod slot {
        pub const IDENTIFIER_TOKEN: usize = 0;
        pub const TYPE_ARGUMENTS: usize = 1;
    }
}

/// The slot and scalar indices of `IfElseStatement`.
pub mod if_else_statement {
    pub mod slot {
        pub const CONDITION: usize = 0;
        pub const TRUE_STATEMENT: usize = 1;
        pub const FALSE_STATEMENT: usize = 2;
    }
}

/// The slot and scalar indices of `IndexerDeclaration`.
pub mod indexer_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const RETURN_TYPE: usize = 1;
        pub const PRIVATE_IMPLEMENTATION_TYPE: usize = 2;
        pub const PARAMETERS: usize = 3;
        pub const GETTER: usize = 4;
        pub const SETTER: usize = 5;
        pub const EXPRESSION_BODY: usize = 6;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
    }
}

/// The slot and scalar indices of `IndexerExpression`.
pub mod indexer_expression {
    pub mod slot {
        pub const TARGET: usize = 0;
        pub const ARGUMENTS: usize = 1;
    }
}

/// The slot and scalar indices of `InterpolatedStringExpression`.
pub mod interpolated_string_expression {
    pub mod slot {
        pub const CONTENT: usize = 0;
    }
}

/// The slot and scalar indices of `InterpolatedStringText`.
pub mod interpolated_string_text {
    pub mod scalar {
        pub const TEXT: usize = 0;
    }
}

/// The slot and scalar indices of `Interpolation`.
pub mod interpolation {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
    }
    pub mod scalar {
        pub const ALIGNMENT: usize = 0;
        pub const SUFFIX: usize = 1;
    }
}

/// The slot and scalar indices of `InvocationAstType`.
pub mod invocation_ast_type {
    pub mod slot {
        pub const ARGUMENTS: usize = 0;
        pub const BASE_TYPE: usize = 1;
    }
}

/// The slot and scalar indices of `InvocationExpression`.
pub mod invocation_expression {
    pub mod slot {
        pub const TARGET: usize = 0;
        pub const ARGUMENTS: usize = 1;
    }
}

/// The slot and scalar indices of `IsExpression`.
pub mod is_expression {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
        pub const TYPE: usize = 1;
    }
}

/// The slot and scalar indices of `LabelStatement`.
pub mod label_statement {
    pub mod slot {
        pub const LABEL_TOKEN: usize = 0;
    }
}

/// The slot and scalar indices of `LambdaExpression`.
pub mod lambda_expression {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const PARAMETERS: usize = 1;
        pub const BODY: usize = 2;
    }
    pub mod scalar {
        pub const IS_ASYNC: usize = 0;
    }
}

/// The slot and scalar indices of `LinePreprocessorDirective`.
pub mod line_preprocessor_directive {
    pub mod scalar {
        pub const TYPE: usize = 0;
        pub const ARGUMENT: usize = 1;
    }
}

/// The slot and scalar indices of `LocalFunctionDeclarationStatement`.
pub mod local_function_declaration_statement {
    pub mod slot {
        pub const DECLARATION: usize = 0;
    }
}

/// The slot and scalar indices of `LockStatement`.
pub mod lock_statement {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
        pub const EMBEDDED_STATEMENT: usize = 1;
    }
}

/// The slot and scalar indices of `MemberReferenceExpression`.
pub mod member_reference_expression {
    pub mod slot {
        pub const TARGET: usize = 0;
        pub const MEMBER_NAME_TOKEN: usize = 1;
        pub const TYPE_ARGUMENTS: usize = 2;
    }
}

/// The slot and scalar indices of `MemberType`.
pub mod member_type {
    pub mod slot {
        pub const TARGET: usize = 0;
        pub const MEMBER_NAME_TOKEN: usize = 1;
        pub const TYPE_ARGUMENTS: usize = 2;
    }
    pub mod scalar {
        pub const IS_DOUBLE_COLON: usize = 0;
    }
}

/// The slot and scalar indices of `MethodDeclaration`.
pub mod method_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const RETURN_TYPE: usize = 1;
        pub const PRIVATE_IMPLEMENTATION_TYPE: usize = 2;
        pub const NAME_TOKEN: usize = 3;
        pub const TYPE_PARAMETERS: usize = 4;
        pub const PARAMETERS: usize = 5;
        pub const CONSTRAINTS: usize = 6;
        pub const BODY: usize = 7;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
    }
}

/// The slot and scalar indices of `NamedArgumentExpression`.
pub mod named_argument_expression {
    pub mod slot {
        pub const NAME_TOKEN: usize = 0;
        pub const EXPRESSION: usize = 1;
    }
}

/// The slot and scalar indices of `NamedExpression`.
pub mod named_expression {
    pub mod slot {
        pub const NAME_TOKEN: usize = 0;
        pub const EXPRESSION: usize = 1;
    }
}

/// The slot and scalar indices of `NamespaceDeclaration`.
pub mod namespace_declaration {
    pub mod slot {
        pub const NAMESPACE_NAME: usize = 0;
        pub const MEMBERS: usize = 1;
    }
    pub mod scalar {
        pub const IS_FILE_SCOPED: usize = 0;
    }
}

/// The slot and scalar indices of `ObjectCreateExpression`.
pub mod object_create_expression {
    pub mod slot {
        pub const TYPE: usize = 0;
        pub const ARGUMENTS: usize = 1;
        pub const INITIALIZER: usize = 2;
    }
}

/// The slot and scalar indices of `OperatorDeclaration`.
pub mod operator_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const RETURN_TYPE: usize = 1;
        pub const PRIVATE_IMPLEMENTATION_TYPE: usize = 2;
        pub const PARAMETERS: usize = 3;
        pub const BODY: usize = 4;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
        pub const OPERATOR_TYPE: usize = 1;
    }
}

/// The slot and scalar indices of `OutVarDeclarationExpression`.
pub mod out_var_declaration_expression {
    pub mod slot {
        pub const TYPE: usize = 0;
        pub const VARIABLE: usize = 1;
    }
}

/// The slot and scalar indices of `ParameterDeclaration`.
pub mod parameter_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const TYPE: usize = 1;
        pub const NAME_TOKEN: usize = 2;
        pub const DEFAULT_EXPRESSION: usize = 3;
    }
    pub mod scalar {
        pub const HAS_THIS_MODIFIER: usize = 0;
        pub const IS_PARAMS: usize = 1;
        pub const IS_SCOPED_REF: usize = 2;
        pub const PARAMETER_MODIFIER: usize = 3;
    }
}

/// The slot and scalar indices of `ParenthesizedExpression`.
pub mod parenthesized_expression {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
    }
}

/// The slot and scalar indices of `ParenthesizedVariableDesignation`.
pub mod parenthesized_variable_designation {
    pub mod slot {
        pub const VARIABLE_DESIGNATIONS: usize = 0;
    }
}

/// The slot and scalar indices of `PointerReferenceExpression`.
pub mod pointer_reference_expression {
    pub mod slot {
        pub const TARGET: usize = 0;
        pub const MEMBER_NAME_TOKEN: usize = 1;
        pub const TYPE_ARGUMENTS: usize = 2;
    }
}

/// The slot and scalar indices of `PragmaWarningPreprocessorDirective`.
pub mod pragma_warning_preprocessor_directive {
    pub mod slot {
        pub const WARNINGS: usize = 0;
    }
    pub mod scalar {
        pub const TYPE: usize = 0;
        pub const ARGUMENT: usize = 1;
    }
}

/// The slot and scalar indices of `PreProcessorDirective`.
pub mod pre_processor_directive {
    pub mod scalar {
        pub const TYPE: usize = 0;
        pub const ARGUMENT: usize = 1;
    }
}

/// The slot and scalar indices of `PrimitiveExpression`.
pub mod primitive_expression {
    pub mod scalar {
        pub const VALUE: usize = 0;
        pub const FORMAT: usize = 1;
    }
}

/// The slot and scalar indices of `PrimitiveType`.
pub mod primitive_type {
    pub mod scalar {
        pub const KEYWORD: usize = 0;
    }
}

/// The slot and scalar indices of `PropertyDeclaration`.
pub mod property_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const RETURN_TYPE: usize = 1;
        pub const NAME_TOKEN: usize = 2;
        pub const PRIVATE_IMPLEMENTATION_TYPE: usize = 3;
        pub const GETTER: usize = 4;
        pub const SETTER: usize = 5;
        pub const INITIALIZER: usize = 6;
        pub const EXPRESSION_BODY: usize = 7;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
    }
}

/// The slot and scalar indices of `QueryContinuationClause`.
pub mod query_continuation_clause {
    pub mod slot {
        pub const PRECEDING_QUERY: usize = 0;
        pub const IDENTIFIER_TOKEN: usize = 1;
    }
}

/// The slot and scalar indices of `QueryExpression`.
pub mod query_expression {
    pub mod slot {
        pub const CLAUSES: usize = 0;
    }
}

/// The slot and scalar indices of `QueryFromClause`.
pub mod query_from_clause {
    pub mod slot {
        pub const TYPE: usize = 0;
        pub const IDENTIFIER_TOKEN: usize = 1;
        pub const EXPRESSION: usize = 2;
    }
}

/// The slot and scalar indices of `QueryGroupClause`.
pub mod query_group_clause {
    pub mod slot {
        pub const PROJECTION: usize = 0;
        pub const KEY: usize = 1;
    }
}

/// The slot and scalar indices of `QueryJoinClause`.
pub mod query_join_clause {
    pub mod slot {
        pub const TYPE: usize = 0;
        pub const JOIN_IDENTIFIER_TOKEN: usize = 1;
        pub const IN_EXPRESSION: usize = 2;
        pub const ON_EXPRESSION: usize = 3;
        pub const EQUALS_EXPRESSION: usize = 4;
        pub const INTO_IDENTIFIER_TOKEN: usize = 5;
    }
}

/// The slot and scalar indices of `QueryLetClause`.
pub mod query_let_clause {
    pub mod slot {
        pub const IDENTIFIER_TOKEN: usize = 0;
        pub const EXPRESSION: usize = 1;
    }
}

/// The slot and scalar indices of `QueryOrderClause`.
pub mod query_order_clause {
    pub mod slot {
        pub const ORDERINGS: usize = 0;
    }
}

/// The slot and scalar indices of `QueryOrdering`.
pub mod query_ordering {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
    }
    pub mod scalar {
        pub const DIRECTION: usize = 0;
    }
}

/// The slot and scalar indices of `QuerySelectClause`.
pub mod query_select_clause {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
    }
}

/// The slot and scalar indices of `QueryWhereClause`.
pub mod query_where_clause {
    pub mod slot {
        pub const CONDITION: usize = 0;
    }
}

/// The slot and scalar indices of `RecursivePatternExpression`.
pub mod recursive_pattern_expression {
    pub mod slot {
        pub const TYPE: usize = 0;
        pub const SUB_PATTERNS: usize = 1;
        pub const DESIGNATION: usize = 2;
    }
    pub mod scalar {
        pub const IS_POSITIONAL: usize = 0;
    }
}

/// The slot and scalar indices of `ReturnStatement`.
pub mod return_statement {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
    }
}

/// The slot and scalar indices of `SimpleType`.
pub mod simple_type {
    pub mod slot {
        pub const IDENTIFIER_TOKEN: usize = 0;
        pub const TYPE_ARGUMENTS: usize = 1;
    }
}

/// The slot and scalar indices of `SingleVariableDesignation`.
pub mod single_variable_designation {
    pub mod slot {
        pub const IDENTIFIER_TOKEN: usize = 0;
    }
}

/// The slot and scalar indices of `SizeOfExpression`.
pub mod size_of_expression {
    pub mod slot {
        pub const TYPE: usize = 0;
    }
}

/// The slot and scalar indices of `StackAllocExpression`.
pub mod stack_alloc_expression {
    pub mod slot {
        pub const TYPE: usize = 0;
        pub const COUNT_EXPRESSION: usize = 1;
        pub const INITIALIZER: usize = 2;
    }
}

/// The slot and scalar indices of `SwitchExpression`.
pub mod switch_expression {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
        pub const SWITCH_SECTIONS: usize = 1;
    }
}

/// The slot and scalar indices of `SwitchExpressionSection`.
pub mod switch_expression_section {
    pub mod slot {
        pub const PATTERN: usize = 0;
        pub const BODY: usize = 1;
    }
}

/// The slot and scalar indices of `SwitchSection`.
pub mod switch_section {
    pub mod slot {
        pub const CASE_LABELS: usize = 0;
        pub const STATEMENTS: usize = 1;
    }
}

/// The slot and scalar indices of `SwitchStatement`.
pub mod switch_statement {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
        pub const SWITCH_SECTIONS: usize = 1;
    }
}

/// The slot and scalar indices of `SyntaxTree`.
pub mod syntax_tree {
    pub mod slot {
        pub const MEMBERS: usize = 0;
    }
}

/// The slot and scalar indices of `ThrowExpression`.
pub mod throw_expression {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
    }
}

/// The slot and scalar indices of `ThrowStatement`.
pub mod throw_statement {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
    }
}

/// The slot and scalar indices of `TryCatchStatement`.
pub mod try_catch_statement {
    pub mod slot {
        pub const TRY_BLOCK: usize = 0;
        pub const CATCH_CLAUSES: usize = 1;
        pub const FINALLY_BLOCK: usize = 2;
    }
}

/// The slot and scalar indices of `TupleAstType`.
pub mod tuple_ast_type {
    pub mod slot {
        pub const ELEMENTS: usize = 0;
    }
}

/// The slot and scalar indices of `TupleExpression`.
pub mod tuple_expression {
    pub mod slot {
        pub const ELEMENTS: usize = 0;
    }
}

/// The slot and scalar indices of `TupleTypeElement`.
pub mod tuple_type_element {
    pub mod slot {
        pub const TYPE: usize = 0;
        pub const NAME_TOKEN: usize = 1;
    }
}

/// The slot and scalar indices of `TypeDeclaration`.
pub mod type_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const NAME_TOKEN: usize = 1;
        pub const TYPE_PARAMETERS: usize = 2;
        pub const PRIMARY_CONSTRUCTOR_PARAMETERS: usize = 3;
        pub const BASE_TYPES: usize = 4;
        pub const CONSTRAINTS: usize = 5;
        pub const MEMBERS: usize = 6;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
        pub const CLASS_TYPE: usize = 1;
        pub const HAS_PRIMARY_CONSTRUCTOR: usize = 2;
    }
}

/// The slot and scalar indices of `TypeOfExpression`.
pub mod type_of_expression {
    pub mod slot {
        pub const TYPE: usize = 0;
    }
}

/// The slot and scalar indices of `TypeParameterDeclaration`.
pub mod type_parameter_declaration {
    pub mod slot {
        pub const ATTRIBUTES: usize = 0;
        pub const NAME_TOKEN: usize = 1;
    }
    pub mod scalar {
        pub const VARIANCE: usize = 0;
    }
}

/// The slot and scalar indices of `TypeReferenceExpression`.
pub mod type_reference_expression {
    pub mod slot {
        pub const TYPE: usize = 0;
    }
}

/// The slot and scalar indices of `UnaryOperatorExpression`.
pub mod unary_operator_expression {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
    }
    pub mod scalar {
        pub const OPERATOR: usize = 0;
    }
}

/// The slot and scalar indices of `UncheckedExpression`.
pub mod unchecked_expression {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
    }
}

/// The slot and scalar indices of `UncheckedStatement`.
pub mod unchecked_statement {
    pub mod slot {
        pub const BODY: usize = 0;
    }
}

/// The slot and scalar indices of `UndocumentedExpression`.
pub mod undocumented_expression {
    pub mod slot {
        pub const ARGUMENTS: usize = 0;
    }
    pub mod scalar {
        pub const UNDOCUMENTED_EXPRESSION_TYPE: usize = 0;
    }
}

/// The slot and scalar indices of `UnsafeStatement`.
pub mod unsafe_statement {
    pub mod slot {
        pub const BODY: usize = 0;
    }
}

/// The slot and scalar indices of `UsingAliasDeclaration`.
pub mod using_alias_declaration {
    pub mod slot {
        pub const ALIAS_TOKEN: usize = 0;
        pub const IMPORT: usize = 1;
    }
}

/// The slot and scalar indices of `UsingDeclaration`.
pub mod using_declaration {
    pub mod slot {
        pub const IMPORT: usize = 0;
    }
}

/// The slot and scalar indices of `UsingStatement`.
pub mod using_statement {
    pub mod slot {
        pub const RESOURCE_ACQUISITION: usize = 0;
        pub const EMBEDDED_STATEMENT: usize = 1;
    }
    pub mod scalar {
        pub const IS_ASYNC: usize = 0;
        pub const IS_ENHANCED: usize = 1;
    }
}

/// The slot and scalar indices of `VariableDeclarationStatement`.
pub mod variable_declaration_statement {
    pub mod slot {
        pub const TYPE: usize = 0;
        pub const VARIABLES: usize = 1;
    }
    pub mod scalar {
        pub const MODIFIERS: usize = 0;
    }
}

/// The slot and scalar indices of `VariableInitializer`.
pub mod variable_initializer {
    pub mod slot {
        pub const NAME_TOKEN: usize = 0;
        pub const INITIALIZER: usize = 1;
    }
}

/// The slot and scalar indices of `WhileStatement`.
pub mod while_statement {
    pub mod slot {
        pub const CONDITION: usize = 0;
        pub const EMBEDDED_STATEMENT: usize = 1;
    }
}

/// The slot and scalar indices of `WithInitializerExpression`.
pub mod with_initializer_expression {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
        pub const INITIALIZER: usize = 1;
    }
}

/// The slot and scalar indices of `YieldReturnStatement`.
pub mod yield_return_statement {
    pub mod slot {
        pub const EXPRESSION: usize = 0;
    }
}
