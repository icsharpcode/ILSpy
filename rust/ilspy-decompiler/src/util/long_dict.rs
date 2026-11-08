//! `LongDict<T>` (`ICSharpCode.Decompiler.Util`): a map from disjoint `i64` intervals to
//! values, built from `(set, value)` entries where earlier entries win.

use super::interval::LongInterval;
use super::long_set::LongSet;

pub struct LongDict<T> {
    keys: Vec<LongInterval>,
    values: Vec<T>,
}

impl<T: Clone> LongDict<T> {
    pub fn new(entries: impl IntoIterator<Item = (LongSet, T)>) -> Self {
        let mut available = LongSet::universe();
        let mut pairs = Vec::new();
        for (key, value) in entries {
            for &interval in key.intersect_with(&available).intervals() {
                pairs.push((interval, value.clone()));
            }
            available = available.except_with(&key);
        }
        // `Array.Sort` by start; the intervals are disjoint, so the order is total.
        pairs.sort_by_key(|(k, _)| k.start);
        let (keys, values) = pairs.into_iter().unzip();
        LongDict { keys, values }
    }

    pub fn get(&self, key: i64) -> Option<&T> {
        let pos = self.keys.partition_point(|k| k.start <= key);
        let i = pos.checked_sub(1)?;
        self.keys[i].contains(key).then(|| &self.values[i])
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn earlier_entries_win_and_lookups_hit_their_interval() {
        let d = LongDict::new([
            (LongSet::from_interval(LongInterval::inclusive(0, 10)), "a"),
            (LongSet::from_interval(LongInterval::inclusive(5, 20)), "b"),
        ]);
        assert_eq!(d.get(7), Some(&"a"));
        assert_eq!(d.get(15), Some(&"b"));
        assert_eq!(d.get(21), None);
        assert_eq!(d.get(-1), None);
    }
}
