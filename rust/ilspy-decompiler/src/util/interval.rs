//! `Interval` and `LongInterval` (`ICSharpCode.Decompiler.Util`): half-open ranges whose
//! exclusive end wraps around to the minimum to include the maximum value.

use std::fmt;

/// `[start, end)` of IL offsets.
#[derive(Clone, Copy, Debug, Default, PartialEq, Eq, Hash)]
pub struct Interval {
    pub start: i32,
    pub end: i32,
}

impl Interval {
    /// # Panics
    /// When `end` lies before `start` (the managed constructor throws).
    pub fn new(start: i32, end: i32) -> Self {
        assert!(
            start <= end.wrapping_sub(1) || start == end,
            "The end must be after the start"
        );
        Interval { start, end }
    }

    pub fn inclusive_end(self) -> i32 {
        self.end.wrapping_sub(1)
    }

    pub fn is_empty(self) -> bool {
        self.start > self.inclusive_end()
    }

    pub fn contains(self, val: i32) -> bool {
        self.start <= val && val <= self.inclusive_end()
    }

    pub fn intersect(self, other: Interval) -> Interval {
        let start = self.start.max(other.start);
        let inclusive_end = self.inclusive_end().min(other.inclusive_end());
        if start <= inclusive_end {
            Interval::new(start, inclusive_end.wrapping_add(1))
        } else {
            Interval::default()
        }
    }
}

impl fmt::Display for Interval {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        if self.end == i32::MIN {
            write!(f, "[{}..int.MaxValue]", self.start)
        } else {
            write!(f, "[{}..{})", self.start, self.end)
        }
    }
}

/// `[start, end)` of 64-bit values.
#[derive(Clone, Copy, Debug, Default, PartialEq, Eq, Hash)]
pub struct LongInterval {
    pub start: i64,
    pub end: i64,
}

impl LongInterval {
    /// # Panics
    /// When `end` lies before `start` (the managed constructor throws).
    pub fn new(start: i64, end: i64) -> Self {
        assert!(
            start <= end.wrapping_sub(1) || start == end,
            "The end must be after the start"
        );
        LongInterval { start, end }
    }

    /// # Panics
    /// When `inclusive_end < start`.
    pub fn inclusive(start: i64, inclusive_end: i64) -> Self {
        assert!(start <= inclusive_end, "The end must be after the start");
        LongInterval::new(start, inclusive_end.wrapping_add(1))
    }

    pub fn inclusive_end(self) -> i64 {
        self.end.wrapping_sub(1)
    }

    pub fn is_empty(self) -> bool {
        self.start > self.inclusive_end()
    }

    pub fn contains(self, val: i64) -> bool {
        self.start <= val && val <= self.inclusive_end()
    }

    pub fn intersect(self, other: LongInterval) -> LongInterval {
        let start = self.start.max(other.start);
        let inclusive_end = self.inclusive_end().min(other.inclusive_end());
        if start <= inclusive_end {
            LongInterval::new(start, inclusive_end.wrapping_add(1))
        } else {
            LongInterval::default()
        }
    }

    /// `Range()`: the values in order.
    pub fn range(self) -> impl Iterator<Item = i64> {
        let mut next = (!self.is_empty()).then_some(self.start);
        let last = self.inclusive_end();
        std::iter::from_fn(move || {
            let current = next?;
            next = (current != last).then(|| current + 1);
            Some(current)
        })
    }
}

impl fmt::Display for LongInterval {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match (self.start == i64::MIN, self.end == i64::MIN) {
            (true, true) => write!(f, "[long.MinValue..long.MaxValue]"),
            (false, true) => write!(f, "[{}..long.MaxValue]", self.start),
            (true, false) => write!(f, "[long.MinValue..{})", self.end),
            (false, false) => write!(f, "[{}..{})", self.start, self.end),
        }
    }
}

/// `ICSharpCode.Decompiler.Tests/Util/IntervalTests.cs`.
#[cfg(test)]
mod tests {
    use super::Interval;

    #[test]
    fn default_is_empty() {
        let d = Interval::default();
        assert!(d.is_empty());
        assert!(!d.contains(-1) && !d.contains(0) && !d.contains(1));
    }

    #[test]
    fn empty_at_1() {
        let i = Interval::new(1, 1);
        assert!(i.is_empty());
        assert!(!i.contains(0) && !i.contains(1) && !i.contains(2));
    }

    #[test]
    fn one_to_three() {
        let i = Interval::new(1, 3);
        assert!(!i.is_empty());
        assert!(!i.contains(0) && i.contains(1) && i.contains(2) && !i.contains(3));
    }

    #[test]
    fn full_interval() {
        let full = Interval::new(i32::MIN, i32::MIN);
        assert!(!full.is_empty());
        assert!(full.contains(i32::MIN) && full.contains(0) && full.contains(i32::MAX));
    }

    #[test]
    fn non_negative_integers() {
        let i = Interval::new(0, i32::MIN);
        assert!(!i.is_empty());
        assert!(i.contains(0) && i.contains(1000) && i.contains(i32::MAX));
        assert!(!i.contains(-1) && !i.contains(-1000) && !i.contains(i32::MIN));
    }

    #[test]
    fn intersection() {
        let empty = Interval::new(0, 0);
        let zero = Interval::new(0, 1);
        let full = Interval::new(i32::MIN, i32::MIN);
        let nonneg = Interval::new(0, i32::MIN);
        let nonpos = Interval::new(i32::MIN, 1);
        let maxval = Interval::new(i32::MAX, i32::MIN);
        assert_eq!(full.intersect(nonneg), nonneg);
        assert_eq!(nonneg.intersect(full), nonneg);
        assert_eq!(nonneg.intersect(zero), zero);
        assert_eq!(nonneg.intersect(nonpos), zero);
        assert_eq!(nonneg.intersect(maxval), maxval);
        assert_eq!(nonpos.intersect(maxval), empty);
    }
}
