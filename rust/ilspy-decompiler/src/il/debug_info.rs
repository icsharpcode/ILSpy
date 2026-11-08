//! `DebugInfo/IDebugInfoProvider.cs`, as far as the IL reader uses it. The engine has no PDB
//! reader: the host implements this (the FFI crate bridges the managed provider).

/// `PdbExtraTypeInfo`: the dynamic and tuple element name information a PDB records for a
/// local, in the order `ApplyAttributeTypeVisitor` consumes it.
#[derive(Clone, Debug, Default, PartialEq, Eq)]
pub struct ExtraTypeInfo {
    pub tuple_element_names: Option<Vec<Option<String>>>,
    pub dynamic_flags: Option<Vec<bool>>,
}

/// Debug information of the main module. Methods are identified by their MethodDef token.
pub trait DebugInfo {
    /// `TryGetName`: the name of local `index`.
    fn local_name(&self, method_token: u32, index: i32) -> Option<String>;
    /// `TryGetExtraTypeInfo` for local `index`.
    fn extra_type_info(&self, method_token: u32, index: i32) -> Option<ExtraTypeInfo>;
}
