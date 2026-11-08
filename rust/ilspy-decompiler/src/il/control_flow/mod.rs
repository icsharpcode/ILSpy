//! `IL/ControlFlow/`: the control flow transforms.

pub mod async_await_decompiler;
pub mod await_in_catch_transform;
pub mod await_in_finally_transform;
pub mod control_flow_graph;
pub mod control_flow_simplification;
pub mod detect_pinned_regions;
pub mod runtime_async;
pub mod state_range_analysis;
pub mod switch_analysis;
pub mod switch_detection;
pub mod symbolic_execution;
pub mod yield_return_decompiler;
