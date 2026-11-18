//! The engine's ports of the built-in AST transforms (`CSharp/Transforms`), each registered in
//! `csharp::ast_transforms::TRANSFORMS` under its index in the built-in list.

pub mod add_checked_blocks;
pub mod add_xml_documentation;
pub mod combine_query_expressions;
pub mod declare_variables;
pub mod field_and_constructor_initializers;
pub mod fix_name_collisions;
pub mod flatten_switch_blocks;
pub mod introduce_query_expressions;
pub mod introduce_unsafe_modifier;
pub mod introduce_using_declarations;
pub mod normalize_block_statements;
pub mod pattern_statement_transform;
pub mod prettify_assignments;
pub mod rename_visual_basic_anonymous_types;
pub mod replace_method_calls_with_operators;
pub mod rewrite_visual_basic_with_events;
