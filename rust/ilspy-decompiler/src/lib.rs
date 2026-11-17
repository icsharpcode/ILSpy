//! The ILSpy decompiler engine.
//!
//! Hosted by the C# shim `ICSharpCode.Decompiler.Rust` through the `ilspy-decompiler-ffi`
//! crate. See `RustPort_HumanLecture.md` at the repository root for the architecture.

pub mod csharp;
pub mod flow_analysis;
pub mod il;
pub mod metadata;
pub mod semantics;
pub mod typesystem;
pub mod util;
pub mod wire;

/// How many IL transforms of `CSharpDecompiler.GetILTransforms()`, flattened in pipeline
/// order, the engine runs before it hands a function to the host (seam 1).
pub const IL_PREFIX: u32 = 59;

/// The number of built-in AST transforms (`CSharpDecompiler.GetAstTransforms`) the engine runs
/// on the syntax tree the host sends at seam 2 (`csharp::ast_transforms`).
pub const AST_PREFIX: u32 = 0;

/// The engine version, as declared in the workspace manifest.
pub const VERSION: &str = env!("CARGO_PKG_VERSION");

/// One host-side decompiler instance. Requests on a session may run on any thread.
#[derive(Debug, Default)]
pub struct Session {}

impl Session {
    pub fn new() -> Self {
        Self::default()
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn session_is_send_and_sync() {
        fn assert_send_sync<T: Send + Sync>() {}
        assert_send_sync::<Session>();
    }
}
