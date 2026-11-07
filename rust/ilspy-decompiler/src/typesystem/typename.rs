//! Serialized type names (`System.Reflection.Metadata.TypeName.TryParse`, used by
//! `ReflectionHelper.ParseReflectionName`), including the assembly name grammar it validates.
//!
//! Ported from the net11 shared-framework System.Reflection.Metadata, behavior for behavior:
//! the raw text (escapes included) is the full name of a simple name, nested names share the
//! text of their outermost name, decorators wrap in order, and the default node limit is 20.

const MAX_NODES: usize = 20;
const SZ_ARRAY: i32 = -1;
const POINTER: i32 = -2;
const BY_REF: i32 = -3;
const DELIMITERS: &[char] = &['[', ']', '&', '*', ',', '+', '\\'];

/// A parsed type name.
#[derive(Clone, Debug, PartialEq, Eq)]
pub struct TypeName {
    /// The text of a simple (possibly nested) name; `None` for constructed names.
    text: Option<String>,
    /// For a declaring type: the length of its part of `text`.
    nested_length: Option<usize>,
    element_or_generic: Option<Box<TypeName>>,
    declaring: Option<Box<TypeName>>,
    generic_args: Vec<TypeName>,
    rank_or_modifier: i32,
    /// The simple assembly name, when the name was assembly-qualified.
    pub assembly: Option<String>,
}

impl TypeName {
    fn simple(
        text: String,
        assembly: Option<String>,
        declaring: Option<TypeName>,
        nested_length: Option<usize>,
    ) -> Self {
        TypeName {
            text: Some(text),
            nested_length,
            element_or_generic: None,
            declaring: declaring.map(Box::new),
            generic_args: Vec::new(),
            rank_or_modifier: 0,
            assembly,
        }
    }

    pub fn is_array(&self) -> bool {
        self.rank_or_modifier == SZ_ARRAY || self.rank_or_modifier > 0
    }

    pub fn is_by_ref(&self) -> bool {
        self.rank_or_modifier == BY_REF
    }

    pub fn is_pointer(&self) -> bool {
        self.rank_or_modifier == POINTER
    }

    pub fn is_nested(&self) -> bool {
        self.declaring.is_some()
    }

    pub fn is_constructed_generic(&self) -> bool {
        !self.generic_args.is_empty()
    }

    /// `GetArrayRank`.
    pub fn array_rank(&self) -> u32 {
        if self.rank_or_modifier == SZ_ARRAY {
            1
        } else {
            self.rank_or_modifier as u32
        }
    }

    pub fn element_type(&self) -> &TypeName {
        self.element_or_generic
            .as_deref()
            .expect("decorated type name")
    }

    pub fn generic_definition(&self) -> &TypeName {
        self.element_or_generic
            .as_deref()
            .expect("constructed type name")
    }

    pub fn generic_args(&self) -> &[TypeName] {
        &self.generic_args
    }

    pub fn declaring_type(&self) -> &TypeName {
        self.declaring.as_deref().expect("nested type name")
    }

    /// `TypeName.FullName`.
    pub fn full_name(&self) -> String {
        let mut s = String::new();
        self.append_full_name(&mut s);
        s
    }

    fn append_full_name(&self, s: &mut String) {
        match &self.text {
            Some(text) => match self.nested_length {
                Some(n) if text.chars().count() > n => s.extend(text.chars().take(n)),
                _ => s.push_str(text),
            },
            None if self.is_constructed_generic() => {
                self.generic_definition().append_full_name(s);
                s.push('[');
                for (i, arg) in self.generic_args.iter().enumerate() {
                    if i > 0 {
                        s.push(',');
                    }
                    s.push('[');
                    arg.append_full_name(s);
                    if let Some(asm) = &arg.assembly {
                        s.push_str(", ");
                        s.push_str(asm);
                    }
                    s.push(']');
                }
                s.push(']');
            }
            None => {
                self.element_type().append_full_name(s);
                s.push_str(&match self.rank_or_modifier {
                    BY_REF => "&".to_owned(),
                    POINTER => "*".to_owned(),
                    SZ_ARRAY => "[]".to_owned(),
                    1 => "[*]".to_owned(),
                    r => format!("[{}]", ",".repeat(r as usize - 1)),
                });
            }
        }
    }

    /// `TypeName.Name` of a simple or nested name: the last nested segment, or the part after
    /// the namespace.
    pub fn name(&self) -> String {
        let full: Vec<char> = match &self.text {
            Some(text) => {
                let chars: Vec<char> = text.chars().collect();
                match self.nested_length {
                    Some(n) => chars[..n.min(chars.len())].to_vec(),
                    None => chars,
                }
            }
            None => {
                return self
                    .element_or_generic
                    .as_ref()
                    .map_or_else(String::new, |e| e.name());
            }
        };
        if let Some(declaring) = &self.declaring {
            let start = declaring.nested_length.unwrap_or(0) + 1;
            full[start.min(full.len())..].iter().collect()
        } else {
            let s: String = full.iter().collect();
            match namespace_delimiter(&s) {
                Some(i) => s[i + 1..].to_owned(),
                None => s,
            }
        }
    }
}

/// `IndexOfNamespaceDelimiter`.
fn namespace_delimiter(full: &str) -> Option<usize> {
    let i = full.rfind('.')?;
    if i > 0 && full.as_bytes()[i - 1] == b'.' {
        Some(i - 1)
    } else {
        Some(i)
    }
}

fn trim_start(s: &[char]) -> &[char] {
    let n = s.iter().take_while(|c| c.is_whitespace()).count();
    &s[n..]
}

fn strip(span: &mut &[char], c: char) -> bool {
    if span.first() == Some(&c) {
        *span = trim_start(&span[1..]);
        true
    } else {
        false
    }
}

/// `GetFullTypeNameLength`: `None` for an invalid escape.
fn full_type_name_length(input: &[char]) -> (Option<usize>, bool) {
    let Some(mut i) = input.iter().position(|c| DELIMITERS.contains(c)) else {
        return (Some(input.len()), false);
    };
    if input[i] == '\\' {
        loop {
            if i >= input.len() {
                break;
            }
            let c = input[i];
            if c == '\\' {
                i += 1;
                if i == input.len() || !DELIMITERS.contains(&input[i]) {
                    return (None, false);
                }
            } else if DELIMITERS.contains(&c) {
                break;
            }
            i += 1;
        }
    }
    let nested = i > 0 && i < input.len() && input[i] == '+';
    (Some(i), nested)
}

fn parse_decorator(input: &mut &[char]) -> Option<i32> {
    let saved = *input;
    if strip(input, '*') {
        return Some(POINTER);
    }
    if strip(input, '&') {
        return Some(BY_REF);
    }
    if strip(input, '[') {
        let mut rank = 1i32;
        let mut star = false;
        loop {
            if strip(input, ']') {
                return Some(if rank == 1 && !star { SZ_ARRAY } else { rank });
            }
            if star {
                break;
            }
            if rank == 1 && strip(input, '*') {
                star = true;
                continue;
            }
            if !strip(input, ',') {
                break;
            }
            rank += 1;
        }
    }
    *input = saved;
    None
}

struct Parser<'s> {
    input: &'s [char],
    depth: usize,
}

impl Parser<'_> {
    fn dive(&mut self) -> bool {
        self.depth += 1;
        self.depth <= MAX_NODES
    }

    fn next_type_name(&mut self, allow_qualified: bool) -> Option<TypeName> {
        if !self.dive() {
            return None;
        }
        let mut nested_lengths = Vec::new();
        let mut total = 0;
        loop {
            let (len, nested) = full_type_name_length(&self.input[total..]);
            let len = len.filter(|&l| l > 0)?;
            if nested {
                if !self.dive() {
                    return None;
                }
                nested_lengths.push(len);
                total += 1;
            }
            total += len;
            if !nested {
                break;
            }
        }
        let text: String = self.input[..total].iter().collect();
        self.input = &self.input[total..];
        let after_name = self.input;
        let mut args = Vec::new();
        if let Some(mut double) = self.beginning_of_generic_args() {
            loop {
                let arg = self.next_type_name(double)?;
                if double && !strip(&mut self.input, ']') {
                    return None;
                }
                args.push(arg);
                if !strip(&mut self.input, ',') {
                    break;
                }
                double = strip(&mut self.input, '[');
            }
            if !strip(&mut self.input, ']') {
                return None;
            }
        }
        if args.is_empty() {
            self.input = after_name;
        } else if !self.dive() {
            return None;
        }
        let decorators_start = self.input;
        let mut has_decorators = false;
        while parse_decorator(&mut self.input).is_some() {
            if !self.dive() {
                return None;
            }
            has_decorators = true;
        }
        let assembly = if allow_qualified {
            self.assembly_name()?
        } else {
            None
        };
        let declaring = declaring_type(&text, &nested_lengths, &assembly);
        let mut name = TypeName::simple(text, assembly.clone(), declaring.clone(), None);
        if !args.is_empty() {
            name = TypeName {
                text: None,
                nested_length: None,
                element_or_generic: Some(Box::new(name)),
                declaring: declaring.map(Box::new),
                generic_args: args,
                rank_or_modifier: 0,
                assembly: assembly.clone(),
            };
        }
        if has_decorators {
            let mut input = decorators_start;
            while let Some(r) = parse_decorator(&mut input) {
                name = TypeName {
                    text: None,
                    nested_length: None,
                    element_or_generic: Some(Box::new(name)),
                    declaring: None,
                    generic_args: Vec::new(),
                    rank_or_modifier: r,
                    assembly: assembly.clone(),
                };
            }
        }
        Some(name)
    }

    /// `IsBeginningOfGenericArgs`: `Some(double_brackets)`.
    fn beginning_of_generic_args(&mut self) -> Option<bool> {
        if self.input.first() != Some(&'[') {
            return None;
        }
        let rest = trim_start(&self.input[1..]);
        match rest.first() {
            Some('[') => {
                self.input = trim_start(&rest[1..]);
                Some(true)
            }
            Some(c) if !matches!(c, '*' | ',' | ']') => {
                self.input = rest;
                Some(false)
            }
            _ => None,
        }
    }

    /// `TryParseAssemblyName`: `None` on failure, `Some(None)` without one.
    fn assembly_name(&mut self) -> Option<Option<String>> {
        let saved = self.input;
        if !strip(&mut self.input, ',') {
            return Some(None);
        }
        if self.input.is_empty() {
            self.input = saved;
            return None;
        }
        let candidate_len = assembly_name_candidate(self.input);
        let candidate: String = self.input[..candidate_len].iter().collect();
        let name = parse_assembly_name(&candidate)?;
        self.input = &self.input[candidate_len..];
        Some(Some(name))
    }
}

fn declaring_type(
    text: &str,
    nested_lengths: &[usize],
    assembly: &Option<String>,
) -> Option<TypeName> {
    let mut declaring: Option<TypeName> = None;
    let mut offset = 0;
    for len in nested_lengths {
        let length = offset + len;
        declaring = Some(TypeName::simple(
            text.to_owned(),
            assembly.clone(),
            declaring,
            Some(length),
        ));
        offset += len + 1;
    }
    declaring
}

/// `GetAssemblyNameCandidate`: up to the first `]` not preceded by `\`.
fn assembly_name_candidate(input: &[char]) -> usize {
    match input.iter().position(|&c| c == ']') {
        Some(i) if i > 0 && input[i - 1] == '\\' => {
            let mut j = i;
            while j < input.len() && (input[j] != ']' || input[j - 1] == '\\') {
                j += 1;
            }
            j
        }
        Some(i) => i,
        None => input.len(),
    }
}

/// `TypeName.TryParse` with default options.
pub fn parse(name: &str) -> Option<TypeName> {
    let chars: Vec<char> = name.chars().collect();
    let input = trim_start(&chars);
    if input.is_empty() {
        return None;
    }
    let mut parser = Parser { input, depth: 0 };
    let result = parser.next_type_name(true)?;
    parser.input.is_empty().then_some(result)
}

#[derive(PartialEq)]
enum Token {
    Equals,
    Comma,
    String(String),
    End,
}

struct AssemblyNameTokens<'s> {
    input: &'s [char],
    index: usize,
}

impl AssemblyNameTokens<'_> {
    /// `TryGetNextChar`: `None` for an embedded NUL; `'\0'` at the end.
    fn next_char(&mut self) -> Option<char> {
        if self.index < self.input.len() {
            let c = self.input[self.index];
            self.index += 1;
            if c == '\0' { None } else { Some(c) }
        } else {
            Some('\0')
        }
    }

    fn next_token(&mut self) -> Option<Token> {
        let is_space = |c: char| matches!(c, '\t' | '\n' | '\r' | ' ');
        let mut c;
        loop {
            c = self.next_char()?;
            match c {
                ',' => return Some(Token::Comma),
                '=' => return Some(Token::Equals),
                '\0' => return Some(Token::End),
                _ => {}
            }
            if !is_space(c) {
                break;
            }
        }
        let mut s = String::new();
        let quote = if c == '"' || c == '\'' {
            let q = c;
            c = self.next_char()?;
            Some(q)
        } else {
            None
        };
        loop {
            if c == '\0' {
                if quote.is_some() {
                    return None;
                }
                break;
            }
            if quote == Some(c) {
                break;
            }
            if quote.is_none() && (c == ',' || c == '=') {
                self.index -= 1;
                break;
            }
            if quote.is_none() && (c == '"' || c == '\'') {
                return None;
            }
            if c == '\\' {
                c = self.next_char()?;
                match c {
                    '"' | '\'' | ',' | '=' | '\\' => s.push(c),
                    't' => s.push('\t'),
                    'r' => s.push('\r'),
                    'n' => s.push('\n'),
                    _ => return None,
                }
            } else {
                s.push(c);
            }
            c = self.next_char()?;
        }
        if quote.is_none() {
            while s.ends_with(is_space) {
                s.pop();
            }
        }
        Some(Token::String(s))
    }
}

/// `AssemblyNameInfo.TryParse`: the simple name of a valid assembly name.
pub fn parse_assembly_name(input: &str) -> Option<String> {
    let chars: Vec<char> = input.chars().collect();
    if chars.is_empty() {
        return None;
    }
    let mut t = AssemblyNameTokens {
        input: &chars,
        index: 0,
    };
    let Some(Token::String(name)) = t.next_token() else {
        return None;
    };
    if name.is_empty() {
        return None;
    }
    let mut seen = std::collections::HashSet::new();
    let mut token = t.next_token()?;
    loop {
        match token {
            Token::End => return Some(name),
            Token::Comma => {
                let Some(Token::String(key)) = t.next_token() else {
                    return None;
                };
                if t.next_token()? != Token::Equals {
                    return None;
                }
                let Some(Token::String(value)) = t.next_token() else {
                    return None;
                };
                if key.is_empty() {
                    return None;
                }
                let key_lower = key.to_ascii_lowercase();
                let kind = match key_lower.as_str() {
                    "publickey" => "publickeytoken",
                    k => k,
                };
                let known = matches!(
                    kind,
                    "version"
                        | "culture"
                        | "publickeytoken"
                        | "processorarchitecture"
                        | "retargetable"
                        | "contenttype"
                );
                if known && !seen.insert(kind.to_owned()) {
                    return None;
                }
                let valid = match key_lower.as_str() {
                    "version" => valid_version(&value),
                    "publickeytoken" => valid_hex(&value, true),
                    "publickey" => valid_hex(&value, false),
                    "processorarchitecture" => ["msil", "x86", "ia64", "amd64", "arm"]
                        .iter()
                        .any(|a| value.eq_ignore_ascii_case(a)),
                    "retargetable" => {
                        value.eq_ignore_ascii_case("yes") || value.eq_ignore_ascii_case("no")
                    }
                    "contenttype" => value.eq_ignore_ascii_case("windowsruntime"),
                    _ => true,
                };
                if !valid {
                    return None;
                }
            }
            _ => return None,
        }
        token = t.next_token()?;
    }
}

fn valid_version(v: &str) -> bool {
    let parts: Vec<&str> = v.split('.').collect();
    if !(2..=4).contains(&parts.len()) {
        return false;
    }
    parts.iter().enumerate().all(|(i, p)| {
        !p.is_empty()
            && p.bytes().all(|b| b.is_ascii_digit())
            && p.parse::<u16>().is_ok_and(|n| i >= 2 || n != u16::MAX)
    })
}

fn valid_hex(v: &str, is_token: bool) -> bool {
    if v.eq_ignore_ascii_case("null") || v.is_empty() {
        return true;
    }
    v.len().is_multiple_of(2)
        && (!is_token || v.len() == 16)
        && v.bytes().all(|b| b.is_ascii_hexdigit())
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn parses_like_system_reflection_metadata() {
        let t = parse(
            "System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.Int32]][]",
        )
        .unwrap();
        assert!(t.is_array());
        let g = t.element_type();
        assert!(g.is_constructed_generic());
        assert_eq!(g.generic_args()[0].assembly.as_deref(), Some("mscorlib"));
        assert_eq!(
            g.generic_definition().full_name(),
            "System.Collections.Generic.Dictionary`2"
        );

        let n = parse(
            "A.B+C+D, Asm, Version=1.2.3.4, Culture=neutral, PublicKeyToken=b77a5c561934e089",
        )
        .unwrap();
        assert!(n.is_nested());
        assert_eq!(n.name(), "D");
        assert_eq!(n.declaring_type().name(), "C");
        assert_eq!(n.declaring_type().declaring_type().full_name(), "A.B");
        assert_eq!(n.assembly.as_deref(), Some("Asm"));

        assert!(parse("").is_none());
        assert!(parse("A[").is_none());
        assert!(parse("A, Asm, Version=1").is_none());
        assert!(parse("A, Asm, PublicKeyToken=12").is_none());
        assert!(parse("A*&").is_some_and(|t| t.is_by_ref() && t.element_type().is_pointer()));
        assert!(parse("A[*]").is_some_and(|t| t.is_array() && t.array_rank() == 1));
    }
}
