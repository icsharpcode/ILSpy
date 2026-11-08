//! `UnionFind<T>` (`ICSharpCode.Decompiler.Util`): union by rank with path compression.

use std::collections::HashMap;
use std::hash::Hash;

pub struct UnionFind<T> {
    parent: HashMap<T, T>,
    rank: HashMap<T, u32>,
}

impl<T> Default for UnionFind<T> {
    fn default() -> Self {
        UnionFind {
            parent: HashMap::new(),
            rank: HashMap::new(),
        }
    }
}

impl<T: Copy + Eq + Hash> UnionFind<T> {
    pub fn find(&mut self, v: T) -> T {
        let p = *self.parent.get(&v).unwrap_or(&v);
        if p == v {
            return v;
        }
        let root = self.find(p);
        self.parent.insert(v, root);
        root
    }

    /// Merges the sets of `a` and `b`; on equal ranks `a`'s representative wins, as in the
    /// managed code.
    pub fn merge(&mut self, a: T, b: T) {
        let (ra, rb) = (self.find(a), self.find(b));
        if ra == rb {
            return;
        }
        let (ka, kb) = (
            *self.rank.get(&ra).unwrap_or(&0),
            *self.rank.get(&rb).unwrap_or(&0),
        );
        if ka < kb {
            self.parent.insert(ra, rb);
        } else if ka > kb {
            self.parent.insert(rb, ra);
        } else {
            self.parent.insert(rb, ra);
            self.rank.insert(ra, ka + 1);
        }
    }
}

#[cfg(test)]
mod tests {
    use super::UnionFind;

    #[test]
    fn merged_elements_share_a_representative() {
        let mut u = UnionFind::default();
        u.merge(1, 2);
        u.merge(3, 4);
        assert_eq!(u.find(1), u.find(2));
        assert_ne!(u.find(1), u.find(3));
        u.merge(2, 4);
        assert_eq!(u.find(3), u.find(1));
        assert_eq!(u.find(5), 5);
    }
}
