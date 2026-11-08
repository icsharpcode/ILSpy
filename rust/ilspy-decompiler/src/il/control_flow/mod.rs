//! `IL/ControlFlow/`: the control flow transforms.

pub mod control_flow_simplification;
pub mod detect_pinned_regions;
pub mod state_range_analysis;
pub mod switch_analysis;
pub mod switch_detection;
pub mod symbolic_execution;
pub mod yield_return_decompiler;
