//! The engine's ports of the built-in AST transforms (`CSharp/Transforms`), each registered in
//! `csharp::ast_transforms::TRANSFORMS` under its index in the built-in list.

pub mod add_checked_blocks;
pub mod declare_variables;
pub mod replace_method_calls_with_operators;
