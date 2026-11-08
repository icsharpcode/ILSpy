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

    /// The bit at an index inside the capacity.
    pub fn bit(&self, index: usize) -> bool {
        self.words[index >> 6] & (1u64 << (index & 63)) != 0
    }

    pub fn is_subset_of(&self, other: &BitSet) -> bool {
        self.words
            .iter()
            .zip(&other.words)
            .all(|(a, b)| a & !b == 0)
    }

    pub fn union_with(&mut self, other: &BitSet) {
        for (a, b) in self.words.iter_mut().zip(&other.words) {
            *a |= b;
        }
    }

    pub fn intersect_with(&mut self, other: &BitSet) {
        for (a, b) in self.words.iter_mut().zip(&other.words) {
            *a &= b;
        }
    }

    /// Whether any bit is set.
    pub fn any(&self) -> bool {
        self.words.iter().any(|&w| w != 0)
    }

    pub fn replace_with(&mut self, other: &BitSet) {
        self.words.copy_from_slice(&other.words);
    }

    pub fn clear_all(&mut self) {
        self.words.fill(0);
    }

    /// Clears the bits `start..end`.
    pub fn clear_range(&mut self, start: usize, end: usize) {
        for i in start..end {
            self.words[i >> 6] &= !(1u64 << (i & 63));
        }
    }

    /// The first set bit in `start..end` (`NextSetBit`, which returns -1 for none).
    pub fn next_set_bit(&self, start: usize, end: usize) -> Option<usize> {
        (start..end).find(|&i| self.bit(i))
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

#[cfg(test)]
mod set_tests {
    use super::BitSet;

    #[test]
    fn set_operations_match_the_managed_bitset() {
        let mut a = BitSet::new(130);
        a.set(1);
        a.set(129);
        let mut b = a.clone();
        b.set(64);
        assert!(a.is_subset_of(&b) && !b.is_subset_of(&a));
        a.union_with(&b);
        assert!(a.bit(64));
        assert_eq!(a.next_set_bit(2, 130), Some(64));
        a.clear_range(60, 129);
        assert_eq!(a.next_set_bit(2, 130), Some(129));
        assert_eq!(a.next_set_bit(2, 129), None);
        a.clear_all();
        assert!(!a.bit(1));
        a.replace_with(&b);
        assert!(a.bit(1) && a.bit(64) && a.bit(129));
    }
}
