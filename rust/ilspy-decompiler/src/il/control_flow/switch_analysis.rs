//! `IL/ControlFlow/SwitchAnalysis.cs`: the comparison helpers. The analysis of switch-like
//! condition chains itself belongs to the SwitchDetection step.

use crate::il::ast::ComparisonKind;
use crate::il::types::Sign;
use crate::util::interval::LongInterval;
use crate::util::long_set::LongSet;

/// `MakeSetWhereComparisonIsTrue`: the values `x` for which `x kind val` holds.
pub fn make_set_where_comparison_is_true(kind: ComparisonKind, val: i64, sign: Sign) -> LongSet {
    match kind {
        ComparisonKind::Equality => LongSet::single(val),
        ComparisonKind::Inequality => LongSet::single(val).invert(),
        ComparisonKind::LessThan => greater_than_or_equal(val, sign).invert(),
        ComparisonKind::LessThanOrEqual => less_than_or_equal(val, sign),
        ComparisonKind::GreaterThan => less_than_or_equal(val, sign).invert(),
        ComparisonKind::GreaterThanOrEqual => greater_than_or_equal(val, sign),
    }
}

fn greater_than_or_equal(val: i64, sign: Sign) -> LongSet {
    if sign == Sign::Signed {
        return LongSet::from_interval(LongInterval::inclusive(val, i64::MAX));
    }
    if val >= 0 {
        // val..=ulong.MaxValue as signed longs is two ranges.
        LongSet::from_interval(LongInterval::inclusive(val, i64::MAX))
            .union_with(&LongSet::from_interval(LongInterval::new(i64::MIN, 0)))
    } else {
        LongSet::from_interval(LongInterval::new(val, 0))
    }
}

fn less_than_or_equal(val: i64, sign: Sign) -> LongSet {
    if sign == Sign::Signed {
        return LongSet::from_interval(LongInterval::inclusive(i64::MIN, val));
    }
    if val >= 0 {
        LongSet::from_interval(LongInterval::inclusive(0, val))
    } else {
        // 0..=(ulong)val as signed longs is two ranges.
        LongSet::from_interval(LongInterval::inclusive(0, i64::MAX)).union_with(
            &LongSet::from_interval(LongInterval::inclusive(i64::MIN, val)),
        )
    }
}
