//! `BitSet` (`ICSharpCode.Decompiler.Util`), with the managed word count, so an index the
//! managed indexer rejects is rejected here too.

#[derive(Clone, Debug)]
pub struct BitSet {
    words: Vec<u64>,
}

impl BitSet {
    pub fn new(capacity: usize) -> Self {
        BitSet {
            words: vec![0; ((capacity + 63) >> 6).max(1)],
        }
    }

    /// The bit at `index`, `None` where the managed indexer throws.
    pub fn get(&self, index: usize) -> Option<bool> {
        let word = self.words.get(index >> 6)?;
        Some(word & (1u64 << (index & 63)) != 0)
    }

    /// # Panics
    /// When `index` is beyond the capacity rounded up to a whole word.
    pub fn set(&mut self, index: usize) {
        self.words[index >> 6] |= 1u64 << (index & 63);
    }
}

#[cfg(test)]
mod tests {
    use super::BitSet;

    #[test]
    fn capacity_rounds_up_to_words() {
        let mut b = BitSet::new(64);
        b.set(63);
        assert_eq!(b.get(63), Some(true));
        assert_eq!(b.get(62), Some(false));
        assert_eq!(b.get(64), None);
        assert_eq!(BitSet::new(65).get(64), Some(false));
        assert_eq!(BitSet::new(0).get(0), Some(false));
    }
}
