//! The ILAst: the IL value model, the instruction tree, the IL reader and its block builder.

pub mod ast;
mod block_builder;
pub mod control_flow;
pub mod debug_info;
pub mod opcodes;
mod patterns;
pub mod reader;
mod refs;
pub mod transforms;
pub mod types;
