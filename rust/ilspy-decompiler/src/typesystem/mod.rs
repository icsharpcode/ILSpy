//! The type system: a compilation of modules, type definitions resolved across them, and
//! interned types built from signatures.

pub mod attrs;
pub mod attrvalue;
pub mod compilation;
pub mod flags;
pub mod fnptr;
pub mod known;
pub mod listing;
pub mod members;
pub mod names;
pub mod options;
pub mod reflection;
pub mod resolve;
pub mod typename;
pub mod types;
