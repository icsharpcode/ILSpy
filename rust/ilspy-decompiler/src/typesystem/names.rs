//! Type names as the managed `TopLevelTypeName` / `FullTypeName` define them.

use std::fmt::Write;

#[derive(Clone, Debug, PartialEq, Eq, Hash)]
pub struct TopLevelTypeName {
    pub ns: String,
    pub name: String,
    pub tpc: u32,
}

impl TopLevelTypeName {
    pub fn new(ns: &str, name: &str, tpc: u32) -> Self {
        Self {
            ns: ns.to_owned(),
            name: name.to_owned(),
            tpc,
        }
    }
}

/// A top-level name plus nested type names with their additional type parameter counts.
#[derive(Clone, Debug, PartialEq, Eq, Hash)]
pub struct FullTypeName {
    pub top: TopLevelTypeName,
    pub nested: Vec<(String, u32)>,
}

impl FullTypeName {
    pub fn top_level(top: TopLevelTypeName) -> Self {
        Self {
            top,
            nested: Vec::new(),
        }
    }

    pub fn nested_type(mut self, name: String, tpc: u32) -> Self {
        self.nested.push((name, tpc));
        self
    }

    /// The simple name of the innermost type.
    pub fn name(&self) -> &str {
        self.nested.last().map_or(&self.top.name, |(n, _)| n)
    }

    pub fn type_parameter_count(&self) -> u32 {
        self.top.tpc + self.nested.iter().map(|(_, t)| t).sum::<u32>()
    }

    /// `Ns.Outer`1+Inner`1`.
    pub fn reflection_name(&self) -> String {
        let mut s = String::new();
        if !self.top.ns.is_empty() {
            s.push_str(&self.top.ns);
            s.push('.');
        }
        s.push_str(&self.top.name);
        if self.top.tpc > 0 {
            let _ = write!(s, "`{}", self.top.tpc);
        }
        for (name, tpc) in &self.nested {
            s.push('+');
            s.push_str(name);
            if *tpc > 0 {
                let _ = write!(s, "`{tpc}");
            }
        }
        s
    }
}

/// Splits a trailing `` `N `` (ASCII digits only, at most `i32::MAX`) off a metadata name,
/// like `ReflectionHelper.SplitTypeParameterCountFromReflectionName`.
pub fn split_type_parameter_count(name: &str) -> (&str, u32) {
    if let Some(pos) = name.rfind('`') {
        let digits = &name[pos + 1..];
        if !digits.is_empty()
            && digits.bytes().all(|b| b.is_ascii_digit())
            && let Ok(n) = digits.parse::<u64>()
            && n <= i32::MAX as u64
        {
            return (&name[..pos], n as u32);
        }
    }
    (name, 0)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn splits_like_reflection_helper() {
        assert_eq!(split_type_parameter_count("List`1"), ("List", 1));
        assert_eq!(split_type_parameter_count("List`01"), ("List", 1));
        assert_eq!(split_type_parameter_count("A`b`2"), ("A`b", 2));
        assert_eq!(split_type_parameter_count("List`"), ("List`", 0));
        assert_eq!(split_type_parameter_count("List`x"), ("List`x", 0));
        assert_eq!(
            split_type_parameter_count("L`99999999999"),
            ("L`99999999999", 0)
        );
    }

    #[test]
    fn reflection_names() {
        let n = FullTypeName::top_level(TopLevelTypeName::new("N", "Outer", 1))
            .nested_type("Inner".into(), 2)
            .nested_type("Leaf".into(), 0);
        assert_eq!(n.reflection_name(), "N.Outer`1+Inner`2+Leaf");
        assert_eq!(n.type_parameter_count(), 3);
    }
}
