use std::fmt;

/// Why a metadata structure could not be read.
#[derive(Debug, Clone, PartialEq, Eq)]
pub enum Error {
    /// The input is not a PE image.
    NotPe(&'static str),
    /// A PE image without a CLI header.
    NotManaged,
    /// A read ran past the end of its region.
    Truncated,
    /// The data violates ECMA-335.
    Malformed(&'static str),
    /// Valid, but not handled by this reader.
    Unsupported(&'static str),
}

pub type Result<T> = std::result::Result<T, Error>;

impl fmt::Display for Error {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            Error::NotPe(why) => write!(f, "not a PE image: {why}"),
            Error::NotManaged => f.write_str("PE image has no CLI header"),
            Error::Truncated => f.write_str("unexpected end of data"),
            Error::Malformed(why) => write!(f, "malformed metadata: {why}"),
            Error::Unsupported(why) => write!(f, "unsupported metadata: {why}"),
        }
    }
}

impl std::error::Error for Error {}
