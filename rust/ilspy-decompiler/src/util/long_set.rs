//! `LongSet` (`ICSharpCode.Decompiler.Util`): a set of 64-bit values as sorted, non-adjacent,
//! non-empty intervals.

use std::fmt;

use super::interval::LongInterval;

#[derive(Clone, Debug, Default)]
pub struct LongSet {
    pub intervals: Vec<LongInterval>,
}

/// `LongSet.MergeOverlapping`: `input` sorted by start, empty intervals removed.
fn merge_overlapping(input: impl IntoIterator<Item = LongInterval>) -> Vec<LongInterval> {
    let mut result = Vec::new();
    let (mut start, mut end) = (i64::MIN, i64::MIN);
    let mut empty = true;
    for element in input {
        if !empty && element.start <= end {
            end = if element.end == i64::MIN {
                i64::MIN
            } else {
                end.max(element.end)
            };
        } else {
            if !empty {
                result.push(LongInterval::new(start, end));
            } else {
                empty = false;
            }
            start = element.start;
            end = element.end;
        }
        if end == i64::MIN {
            break;
        }
    }
    if !empty {
        result.push(LongInterval::new(start, end));
    }
    result
}

impl LongSet {
    pub fn empty() -> Self {
        LongSet {
            intervals: Vec::new(),
        }
    }

    pub fn universe() -> Self {
        LongSet::from_interval(LongInterval::inclusive(i64::MIN, i64::MAX))
    }

    pub fn single(value: i64) -> Self {
        LongSet {
            intervals: vec![LongInterval::inclusive(value, value)],
        }
    }

    pub fn from_interval(interval: LongInterval) -> Self {
        LongSet {
            intervals: if interval.is_empty() {
                Vec::new()
            } else {
                vec![interval]
            },
        }
    }

    pub fn from_intervals(intervals: impl IntoIterator<Item = LongInterval>) -> Self {
        let mut sorted: Vec<LongInterval> =
            intervals.into_iter().filter(|i| !i.is_empty()).collect();
        sorted.sort_by_key(|i| i.start);
        LongSet {
            intervals: merge_overlapping(sorted),
        }
    }

    pub fn is_empty(&self) -> bool {
        self.intervals.is_empty()
    }

    /// `Count()`: the number of values, `u64::MAX` for the universe (whose count overflows).
    pub fn count(&self) -> u64 {
        let count = self.intervals.iter().fold(0u64, |c, i| {
            c.wrapping_add(i.end.wrapping_sub(i.start) as u64)
        });
        if count == 0 && !self.intervals.is_empty() {
            u64::MAX
        } else {
            count
        }
    }

    fn intersections<'a>(&'a self, other: &'a LongSet) -> impl Iterator<Item = LongInterval> + 'a {
        let (mut a, mut b) = (
            self.intervals.iter().peekable(),
            other.intervals.iter().peekable(),
        );
        std::iter::from_fn(move || {
            while let (Some(&&x), Some(&&y)) = (a.peek(), b.peek()) {
                let intersection = x.intersect(y);
                if x.inclusive_end() < y.inclusive_end() {
                    a.next();
                } else {
                    b.next();
                }
                if !intersection.is_empty() {
                    return Some(intersection);
                }
            }
            None
        })
    }

    pub fn overlaps(&self, other: &LongSet) -> bool {
        self.intersections(other).next().is_some()
    }

    pub fn intersect_with(&self, other: &LongSet) -> LongSet {
        LongSet {
            intervals: self.intersections(other).collect(),
        }
    }

    pub fn union_with(&self, other: &LongSet) -> LongSet {
        let mut merged = Vec::with_capacity(self.intervals.len() + other.intervals.len());
        let (mut a, mut b) = (
            self.intervals.iter().peekable(),
            other.intervals.iter().peekable(),
        );
        loop {
            match (a.peek(), b.peek()) {
                (Some(x), Some(y)) if x.start <= y.start => merged.push(*a.next().unwrap()),
                (Some(_), Some(_)) | (None, Some(_)) => merged.push(*b.next().unwrap()),
                (Some(_), None) => merged.push(*a.next().unwrap()),
                (None, None) => break,
            }
        }
        LongSet {
            intervals: merge_overlapping(merged),
        }
    }

    pub fn add_offset(&self, val: i64) -> LongSet {
        if val == 0 {
            return self.clone();
        }
        let mut intervals = Vec::with_capacity(self.intervals.len() + 1);
        for element in &self.intervals {
            let start = element.start.wrapping_add(val);
            let inclusive_end = element.inclusive_end().wrapping_add(val);
            if start <= inclusive_end {
                intervals.push(LongInterval::inclusive(start, inclusive_end));
            } else {
                intervals.push(LongInterval::inclusive(start, i64::MAX));
                intervals.push(LongInterval::inclusive(i64::MIN, inclusive_end));
            }
        }
        intervals.sort_by_key(|i| i.start);
        LongSet {
            intervals: merge_overlapping(intervals),
        }
    }

    pub fn except_with(&self, other: &LongSet) -> LongSet {
        self.intersect_with(&other.invert())
    }

    pub fn invert(&self) -> LongSet {
        if self.is_empty() {
            return LongSet::universe();
        }
        let mut intervals = Vec::with_capacity(self.intervals.len() + 1);
        let mut prev_end = i64::MIN;
        for interval in &self.intervals {
            if interval.start > prev_end {
                intervals.push(LongInterval::new(prev_end, interval.start));
            }
            prev_end = interval.end;
        }
        if prev_end != i64::MIN {
            intervals.push(LongInterval::new(prev_end, i64::MIN));
        }
        LongSet { intervals }
    }

    pub fn is_subset_of(&self, other: &LongSet) -> bool {
        self.union_with(other).set_equals(other)
    }

    pub fn is_superset_of(&self, other: &LongSet) -> bool {
        other.is_subset_of(self)
    }

    pub fn is_proper_subset_of(&self, other: &LongSet) -> bool {
        self.is_subset_of(other) && !self.set_equals(other)
    }

    pub fn is_proper_superset_of(&self, other: &LongSet) -> bool {
        self.is_superset_of(other) && !self.set_equals(other)
    }

    /// `upper_bound` (compares against the exclusive end, as the managed code does).
    fn upper_bound(&self, val: i64) -> usize {
        let (mut min, mut max) = (0isize, self.intervals.len() as isize - 1);
        while max >= min {
            let m = min + (max - min) / 2;
            let i = self.intervals[m as usize];
            if val < i.start {
                max = m - 1;
                continue;
            }
            if val > i.end {
                min = m + 1;
                continue;
            }
            return m as usize + 1;
        }
        min as usize
    }

    pub fn contains(&self, val: i64) -> bool {
        let index = self.upper_bound(val);
        index > 0 && self.intervals[index - 1].contains(val)
    }

    pub fn values(&self) -> impl Iterator<Item = i64> + '_ {
        self.intervals.iter().flat_map(|i| i.range())
    }

    pub fn containing_interval(&self) -> LongInterval {
        match (self.intervals.first(), self.intervals.last()) {
            (Some(first), Some(last)) => LongInterval::new(first.start, last.end),
            _ => LongInterval::default(),
        }
    }

    pub fn set_equals(&self, other: &LongSet) -> bool {
        self.intervals == other.intervals
    }
}

impl fmt::Display for LongSet {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        let parts: Vec<String> = self.intervals.iter().map(|i| i.to_string()).collect();
        write!(f, "{}", parts.join(","))
    }
}

/// `ICSharpCode.Decompiler.Tests/Util/LongSetTests.cs`.
#[cfg(test)]
mod tests {
    use super::{LongInterval, LongSet};

    fn set(intervals: &[(i64, i64)]) -> LongSet {
        LongSet {
            intervals: intervals
                .iter()
                .map(|&(s, e)| LongInterval::new(s, e))
                .collect(),
        }
    }

    fn eq(a: &LongSet, b: &LongSet) {
        assert!(a.set_equals(b), "{a} != {b}");
    }

    #[test]
    fn upper_bound() {
        let s = set(&[(1, 5), (6, 7)]);
        assert_eq!(s.upper_bound(0), 0);
        for i in 1..=5 {
            assert_eq!(s.upper_bound(i), 1);
        }
        for i in 6..=10 {
            assert_eq!(s.upper_bound(i), 2);
        }
    }

    #[test]
    fn universe_contains_all() {
        let u = LongSet::universe();
        assert!(u.contains(i64::MIN) && u.contains(1) && u.contains(i64::MAX));
        assert!(!u.is_empty());
    }

    #[test]
    fn intersect_universe() {
        let u = LongSet::universe();
        eq(&u.intersect_with(&u), &u);
        eq(&u.intersect_with(&LongSet::empty()), &LongSet::empty());
        eq(
            &u.intersect_with(&LongSet::single(i64::MAX)),
            &LongSet::single(i64::MAX),
        );
        let s = set(&[(1, 5), (6, 7)]);
        eq(&s.intersect_with(&u), &s);
    }

    #[test]
    fn union_universe() {
        let u = LongSet::universe();
        eq(&u.union_with(&u), &u);
        eq(&u.union_with(&LongSet::empty()), &u);
        eq(&u.union_with(&LongSet::single(i64::MAX)), &u);
        eq(&set(&[(1, 5), (6, 7)]).union_with(&u), &u);
    }

    #[test]
    fn except_with_universe() {
        let (u, e) = (LongSet::universe(), LongSet::empty());
        eq(&u.except_with(&e), &u);
        eq(&u.except_with(&u), &e);
        eq(&e.except_with(&u), &e);
        eq(&e.except_with(&e), &e);
    }

    #[test]
    fn union_with() {
        eq(
            &LongSet::single(0).union_with(&LongSet::single(1)),
            &LongSet::from_interval(LongInterval::new(0, 2)),
        );
        eq(
            &LongSet::single(0).invert().union_with(&LongSet::single(0)),
            &LongSet::universe(),
        );
    }

    fn assert_add_to(input: &LongSet, constant: i64) {
        let mut expected: Vec<i64> = input.values().map(|e| e.wrapping_add(constant)).collect();
        expected.sort();
        let actual: Vec<i64> = input.add_offset(constant).values().collect();
        assert_eq!(actual, expected);
    }

    #[test]
    fn add_to() {
        eq(&LongSet::single(0).add_offset(1), &LongSet::single(1));
        eq(
            &LongSet::single(i64::MAX).add_offset(1),
            &LongSet::single(i64::MIN),
        );
        assert_add_to(&LongSet::from_interval(LongInterval::new(-10, 10)), 5);
        assert_add_to(
            &LongSet::from_interval(LongInterval::new(-10, 10)),
            i64::MAX,
        );
        eq(
            &LongSet::single(0).invert().add_offset(10),
            &LongSet::single(10).invert(),
        );
        eq(
            &LongSet::single(30).invert().add_offset(-10),
            &LongSet::single(20).invert(),
        );
    }

    #[test]
    fn values() {
        assert!(LongSet::empty().values().next().is_none());
        assert!(LongSet::universe().values().next().is_some());
        let v: Vec<i64> = LongSet::from_interval(LongInterval::inclusive(1, 3))
            .values()
            .collect();
        assert_eq!(v, [1, 2, 3]);
    }

    #[test]
    fn value_count() {
        assert_eq!(LongSet::empty().count(), 0);
        assert_eq!(LongSet::single(3).invert().count(), u64::MAX);
        assert_eq!(LongSet::universe().count(), u64::MAX);
        assert_eq!(
            LongSet::from_interval(LongInterval::inclusive(-1, i64::MAX)).count(),
            i64::MAX as u64 + 2
        );
    }
}
