//! The C# syntax tree of a whole request, as the round trip of seam 2 carries it between the
//! host and the engine: generic nodes whose kinds, slots and scalars `syntax_schema` describes.
//! Annotations travel as handles into a table the host keeps; the engine reads only those it
//! decodes and keeps the rest attached to their nodes.
//!
//! Wire format (unsigned LEB128 `uvar`, signed values zigzag-encoded):
//! a node is its kind, then each scalar of the kind in schema order, then its annotation
//! handles (count, handles), its leading and trailing trivia (count, nodes, each), then each slot
//! in schema order: a single slot as a presence byte and the node, a collection as a count and
//! the nodes. Before the root, the engine's answer lists the annotations it created (count, then
//! each as a kind byte, a length and the payload), which handles from the host's handle count
//! upward refer to; the host's tree has no such list. Scalars: `Boolean` a byte; `String` a count of UTF-16 code units and the units;
//! (its length plus one, 0 for null); `Object` (a literal value) a handle; `TextLocation` line and column; any other (`Int32`,
//! an enum) a zigzag-encoded integer.

use super::syntax_schema::{KINDS, KindSchema};

/// A node of the tree, by its index in `SyntaxTree::nodes`.
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
pub struct SyntaxId(pub u32);

/// The value of a scalar property.
#[derive(Clone, Debug, PartialEq, Eq)]
pub enum Scalar {
    Bool(bool),
    /// An `Int32` or an enum, by its numeric value.
    Int(i64),
    /// A string as UTF-16 code units (identifiers and comments need not be valid UTF-16).
    Str(Vec<u16>),
    /// A string scalar that is null (an interpolation without format suffix, for one).
    NullStr,
    /// A literal value the host keeps, by its handle.
    Object(u32),
    /// A `TextLocation` (line, column).
    Location(u64, u64),
}

impl Scalar {
    pub fn as_bool(&self) -> Option<bool> {
        match *self {
            Scalar::Bool(b) => Some(b),
            _ => None,
        }
    }

    pub fn as_int(&self) -> Option<i64> {
        match *self {
            Scalar::Int(v) => Some(v),
            _ => None,
        }
    }

    pub fn as_str(&self) -> Option<String> {
        match self {
            Scalar::Str(s) => Some(String::from_utf16_lossy(s)),
            _ => None,
        }
    }
}

/// The children of one slot.
#[derive(Clone, Debug, PartialEq, Eq)]
pub enum SlotValue {
    Single(Option<SyntaxId>),
    Many(Vec<SyntaxId>),
}

#[derive(Clone, Debug)]
pub struct SyntaxNode {
    pub kind: u16,
    pub scalars: Vec<Scalar>,
    /// Handles of the host's annotation objects, in the node's order.
    pub annotations: Vec<u32>,
    pub leading_trivia: Vec<SyntaxId>,
    pub trailing_trivia: Vec<SyntaxId>,
    /// One value per slot of the kind, in schema order.
    pub slots: Vec<SlotValue>,
    pub parent: Option<SyntaxId>,
}

impl SyntaxNode {
    pub fn schema(&self) -> &'static KindSchema {
        &KINDS[self.kind as usize]
    }
}

/// A syntax tree in an arena; nodes that a transform detaches stay in the arena unreferenced.
#[derive(Clone, Debug, Default)]
pub struct SyntaxTree {
    pub nodes: Vec<SyntaxNode>,
    pub root: Option<SyntaxId>,
    /// The number of objects the host keeps for the tree's handles.
    pub host_handles: u32,
    /// The annotations the engine created, as the kind of host object the host builds for each
    /// and its payload; the handle of the `i`th is `host_handles + i`. A transform that creates
    /// a new kind adds it here and in the host's `SyntaxRoundTrip.CreateAnnotation`.
    pub new_annotations: Vec<(u8, Vec<u8>)>,
}

impl SyntaxTree {
    pub fn node(&self, id: SyntaxId) -> &SyntaxNode {
        &self.nodes[id.0 as usize]
    }

    pub fn node_mut(&mut self, id: SyntaxId) -> &mut SyntaxNode {
        &mut self.nodes[id.0 as usize]
    }

    pub fn kind(&self, id: SyntaxId) -> u16 {
        self.node(id).kind
    }

    /// The single child in slot `slot` of `id`.
    pub fn child(&self, id: SyntaxId, slot: usize) -> Option<SyntaxId> {
        match &self.node(id).slots[slot] {
            SlotValue::Single(c) => *c,
            SlotValue::Many(_) => panic!(
                "slot {slot} of {} is a collection",
                self.node(id).schema().name
            ),
        }
    }

    /// The children in the collection slot `slot` of `id`.
    pub fn children(&self, id: SyntaxId, slot: usize) -> &[SyntaxId] {
        match &self.node(id).slots[slot] {
            SlotValue::Many(c) => c,
            SlotValue::Single(_) => {
                panic!("slot {slot} of {} is single", self.node(id).schema().name)
            }
        }
    }

    /// All children of `id` in child order (slot by slot), without trivia.
    pub fn all_children(&self, id: SyntaxId) -> Vec<SyntaxId> {
        let mut result = Vec::new();
        for slot in &self.node(id).slots {
            match slot {
                SlotValue::Single(Some(c)) => result.push(*c),
                SlotValue::Single(None) => {}
                SlotValue::Many(cs) => result.extend_from_slice(cs),
            }
        }
        result
    }

    /// A new node of `kind` with default scalars and empty slots, without parent.
    pub fn new_node(&mut self, kind: u16) -> SyntaxId {
        let schema = &KINDS[kind as usize];
        let node = SyntaxNode {
            kind,
            scalars: schema
                .scalars
                .iter()
                .map(|s| match s.ty {
                    "Boolean" => Scalar::Bool(false),
                    "String" => Scalar::Str(Vec::new()),
                    "TextLocation" => Scalar::Location(0, 0),
                    _ => Scalar::Int(0),
                })
                .collect(),
            annotations: Vec::new(),
            leading_trivia: Vec::new(),
            trailing_trivia: Vec::new(),
            slots: schema
                .slots
                .iter()
                .map(|s| {
                    if s.collection {
                        SlotValue::Many(Vec::new())
                    } else {
                        SlotValue::Single(None)
                    }
                })
                .collect(),
            parent: None,
        };
        self.nodes.push(node);
        SyntaxId(self.nodes.len() as u32 - 1)
    }

    /// Puts `child` (detached first) into the single slot `slot` of `id`, detaching what was
    /// there.
    pub fn set_child(&mut self, id: SyntaxId, slot: usize, child: Option<SyntaxId>) {
        if let SlotValue::Single(Some(old)) = self.node(id).slots[slot] {
            self.node_mut(old).parent = None;
        }
        if let Some(c) = child {
            self.detach(c);
            self.node_mut(c).parent = Some(id);
        }
        self.node_mut(id).slots[slot] = SlotValue::Single(child);
    }

    /// Inserts `child` (detached first) at `index` of the collection slot `slot` of `id`.
    pub fn insert_child(&mut self, id: SyntaxId, slot: usize, index: usize, child: SyntaxId) {
        self.detach(child);
        self.node_mut(child).parent = Some(id);
        match &mut self.node_mut(id).slots[slot] {
            SlotValue::Many(cs) => cs.insert(index, child),
            SlotValue::Single(_) => panic!("slot {slot} is single"),
        }
    }

    /// Removes `id` from its parent's slot (or trivia list), if it has a parent.
    pub fn detach(&mut self, id: SyntaxId) {
        let Some(parent) = self.node(id).parent else {
            return;
        };
        let p = self.node_mut(parent);
        for slot in &mut p.slots {
            match slot {
                SlotValue::Single(c) if *c == Some(id) => *c = None,
                SlotValue::Many(cs) => cs.retain(|&c| c != id),
                _ => {}
            }
        }
        p.leading_trivia.retain(|&c| c != id);
        p.trailing_trivia.retain(|&c| c != id);
        self.node_mut(id).parent = None;
    }

    /// Replaces `id` in its parent by `replacement` (detached first).
    pub fn replace(&mut self, id: SyntaxId, replacement: SyntaxId) {
        let Some(parent) = self.node(id).parent else {
            return;
        };
        self.detach(replacement);
        let p = self.node_mut(parent);
        for slot in &mut p.slots {
            match slot {
                SlotValue::Single(c) if *c == Some(id) => *c = Some(replacement),
                SlotValue::Many(cs) => {
                    for c in cs.iter_mut() {
                        if *c == id {
                            *c = replacement;
                        }
                    }
                }
                _ => {}
            }
        }
        self.node_mut(replacement).parent = Some(parent);
        self.node_mut(id).parent = None;
    }

    /// An annotation the engine creates, of the host object kind `kind` with `payload`; its
    /// handle.
    pub fn new_annotation(&mut self, kind: u8, payload: Vec<u8>) -> u32 {
        self.new_annotations.push((kind, payload));
        self.host_handles + self.new_annotations.len() as u32 - 1
    }

    /// Reads a tree the host wrote, whose handles refer to `host_handles` objects.
    pub fn decode(bytes: &[u8], host_handles: u32) -> Result<SyntaxTree, String> {
        let mut r = Reader { bytes, pos: 0 };
        let mut tree = SyntaxTree {
            host_handles,
            ..SyntaxTree::default()
        };
        let root = decode_node(&mut r, &mut tree, None)?;
        if r.pos != bytes.len() {
            return Err(format!(
                "{} bytes after the syntax tree",
                bytes.len() - r.pos
            ));
        }
        tree.root = Some(root);
        Ok(tree)
    }

    /// Writes the tree for the host, from the root.
    pub fn encode(&self) -> Vec<u8> {
        let mut out = Vec::new();
        uvar(&mut out, self.new_annotations.len() as u64);
        for (kind, payload) in &self.new_annotations {
            out.push(*kind);
            uvar(&mut out, payload.len() as u64);
            out.extend_from_slice(payload);
        }
        if let Some(root) = self.root {
            self.encode_node(root, &mut out);
        }
        out
    }

    fn encode_node(&self, id: SyntaxId, out: &mut Vec<u8>) {
        let node = self.node(id);
        uvar(out, u64::from(node.kind));
        for s in &node.scalars {
            match s {
                Scalar::Bool(b) => out.push(u8::from(*b)),
                Scalar::Int(v) => uvar(out, zigzag(*v)),
                Scalar::NullStr => uvar(out, 0),
                Scalar::Str(units) => {
                    uvar(out, units.len() as u64 + 1);
                    for u in units {
                        out.extend_from_slice(&u.to_le_bytes());
                    }
                }
                Scalar::Object(h) => uvar(out, u64::from(*h)),
                Scalar::Location(l, c) => {
                    uvar(out, *l);
                    uvar(out, *c);
                }
            }
        }
        uvar(out, node.annotations.len() as u64);
        for a in &node.annotations {
            uvar(out, u64::from(*a));
        }
        for list in [&node.leading_trivia, &node.trailing_trivia] {
            uvar(out, list.len() as u64);
            for &t in list {
                self.encode_node(t, out);
            }
        }
        for slot in &node.slots {
            match slot {
                SlotValue::Single(None) => out.push(0),
                SlotValue::Single(Some(c)) => {
                    out.push(1);
                    self.encode_node(*c, out);
                }
                SlotValue::Many(cs) => {
                    uvar(out, cs.len() as u64);
                    for &c in cs {
                        self.encode_node(c, out);
                    }
                }
            }
        }
    }
}

fn decode_node(
    r: &mut Reader<'_>,
    tree: &mut SyntaxTree,
    parent: Option<SyntaxId>,
) -> Result<SyntaxId, String> {
    let kind = r.uvar()?;
    let schema = KINDS
        .get(kind as usize)
        .ok_or_else(|| format!("unknown syntax kind {kind}"))?;
    let mut scalars = Vec::with_capacity(schema.scalars.len());
    for s in schema.scalars {
        scalars.push(match s.ty {
            "Boolean" => Scalar::Bool(r.u8()? != 0),
            "String" => match r.uvar()? as usize {
                0 => Scalar::NullStr,
                n => {
                    let mut units = Vec::with_capacity(n - 1);
                    for _ in 1..n {
                        let lo = r.u8()?;
                        let hi = r.u8()?;
                        units.push(u16::from_le_bytes([lo, hi]));
                    }
                    Scalar::Str(units)
                }
            },
            "Object" => Scalar::Object(r.uvar()? as u32),
            "TextLocation" => Scalar::Location(r.uvar()?, r.uvar()?),
            _ => Scalar::Int(unzigzag(r.uvar()?)),
        });
    }
    let n = r.uvar()? as usize;
    let mut annotations = Vec::with_capacity(n);
    for _ in 0..n {
        annotations.push(r.uvar()? as u32);
    }
    tree.nodes.push(SyntaxNode {
        kind: kind as u16,
        scalars,
        annotations,
        leading_trivia: Vec::new(),
        trailing_trivia: Vec::new(),
        slots: Vec::with_capacity(schema.slots.len()),
        parent,
    });
    let id = SyntaxId(tree.nodes.len() as u32 - 1);
    for leading in [true, false] {
        let n = r.uvar()? as usize;
        let mut list = Vec::with_capacity(n);
        for _ in 0..n {
            list.push(decode_node(r, tree, Some(id))?);
        }
        if leading {
            tree.node_mut(id).leading_trivia = list;
        } else {
            tree.node_mut(id).trailing_trivia = list;
        }
    }
    for s in schema.slots {
        let value = if s.collection {
            let n = r.uvar()? as usize;
            let mut cs = Vec::with_capacity(n);
            for _ in 0..n {
                cs.push(decode_node(r, tree, Some(id))?);
            }
            SlotValue::Many(cs)
        } else if r.u8()? != 0 {
            SlotValue::Single(Some(decode_node(r, tree, Some(id))?))
        } else {
            SlotValue::Single(None)
        };
        tree.node_mut(id).slots.push(value);
    }
    Ok(id)
}

struct Reader<'a> {
    bytes: &'a [u8],
    pos: usize,
}

impl Reader<'_> {
    fn u8(&mut self) -> Result<u8, String> {
        let b = *self
            .bytes
            .get(self.pos)
            .ok_or("the syntax tree ends early")?;
        self.pos += 1;
        Ok(b)
    }

    fn uvar(&mut self) -> Result<u64, String> {
        let mut v = 0u64;
        let mut shift = 0;
        loop {
            let b = self.u8()?;
            v |= u64::from(b & 0x7F) << shift;
            if b & 0x80 == 0 {
                return Ok(v);
            }
            shift += 7;
            if shift > 63 {
                return Err("an overlong integer in the syntax tree".to_owned());
            }
        }
    }
}

fn uvar(out: &mut Vec<u8>, mut v: u64) {
    loop {
        let b = (v & 0x7F) as u8;
        v >>= 7;
        if v == 0 {
            out.push(b);
            return;
        }
        out.push(b | 0x80);
    }
}

fn zigzag(v: i64) -> u64 {
    ((v << 1) ^ (v >> 63)) as u64
}

fn unzigzag(v: u64) -> i64 {
    ((v >> 1) as i64) ^ -((v & 1) as i64)
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::csharp::syntax_schema::{binary_operator_expression, kind, primitive_expression};

    #[test]
    fn a_tree_survives_encoding() {
        let mut tree = SyntaxTree::default();
        let binary = tree.new_node(kind::BINARY_OPERATOR_EXPRESSION);
        tree.node_mut(binary).scalars[binary_operator_expression::scalar::OPERATOR] =
            Scalar::Int(-3);
        let left = tree.new_node(kind::PRIMITIVE_EXPRESSION);
        tree.node_mut(left).scalars[primitive_expression::scalar::VALUE] = Scalar::Object(7);
        tree.node_mut(left).annotations = vec![1, 2];
        tree.set_child(binary, binary_operator_expression::slot::LEFT, Some(left));
        tree.root = Some(binary);
        let bytes = tree.encode();
        // The host sends the tree without the list of new annotations, which opens the answer.
        let back = SyntaxTree::decode(&bytes[1..], 8).unwrap();
        assert_eq!(back.encode(), bytes);
        let root = back.root.unwrap();
        let l = back
            .child(root, binary_operator_expression::slot::LEFT)
            .unwrap();
        assert_eq!(back.node(l).annotations, vec![1, 2]);
        assert_eq!(back.node(l).parent, Some(root));
        assert_eq!(
            back.child(root, binary_operator_expression::slot::RIGHT),
            None
        );
        assert_eq!(unzigzag(zigzag(i64::MIN)), i64::MIN);
    }
}
