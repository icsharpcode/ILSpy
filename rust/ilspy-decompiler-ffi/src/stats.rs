//! Process-wide counters for performance measurement (`ilspy_stats`): a counting global
//! allocator over the system allocator, and the wall time spent inside the exports that do
//! work, minus the time their callbacks spend in the host.
//!
//! Every counter is a relaxed atomic, so the bookkeeping costs a few uncontended atomic adds
//! per allocation and two clock reads per export call whether or not anybody reads it.

use std::alloc::{GlobalAlloc, Layout, System};
use std::sync::atomic::{AtomicU64, Ordering::Relaxed};
use std::time::Instant;

static ALLOCATED: AtomicU64 = AtomicU64::new(0);
static ALLOCATIONS: AtomicU64 = AtomicU64::new(0);
static LIVE: AtomicU64 = AtomicU64::new(0);
static PEAK: AtomicU64 = AtomicU64::new(0);
static ELAPSED_NS: AtomicU64 = AtomicU64::new(0);
static CALLBACK_NS: AtomicU64 = AtomicU64::new(0);
static CALLS: AtomicU64 = AtomicU64::new(0);

/// The system allocator, counting bytes allocated, bytes live and the peak of the latter.
pub struct CountingAllocator;

fn on_alloc(size: usize) {
    let size = size as u64;
    ALLOCATED.fetch_add(size, Relaxed);
    ALLOCATIONS.fetch_add(1, Relaxed);
    let live = LIVE.fetch_add(size, Relaxed) + size;
    // Read first: the peak changes rarely, so most allocations skip the read-modify-write.
    if live > PEAK.load(Relaxed) {
        PEAK.fetch_max(live, Relaxed);
    }
}

fn on_dealloc(size: usize) {
    LIVE.fetch_sub(size as u64, Relaxed);
}

// SAFETY: every method forwards to `System` with the caller's arguments unchanged; the
// counters do not touch the memory.
unsafe impl GlobalAlloc for CountingAllocator {
    unsafe fn alloc(&self, layout: Layout) -> *mut u8 {
        // SAFETY: forwarded under the caller's contract.
        let p = unsafe { System.alloc(layout) };
        if !p.is_null() {
            on_alloc(layout.size());
        }
        p
    }

    unsafe fn alloc_zeroed(&self, layout: Layout) -> *mut u8 {
        // SAFETY: forwarded under the caller's contract.
        let p = unsafe { System.alloc_zeroed(layout) };
        if !p.is_null() {
            on_alloc(layout.size());
        }
        p
    }

    unsafe fn dealloc(&self, ptr: *mut u8, layout: Layout) {
        // SAFETY: forwarded under the caller's contract.
        unsafe { System.dealloc(ptr, layout) };
        on_dealloc(layout.size());
    }

    unsafe fn realloc(&self, ptr: *mut u8, layout: Layout, new_size: usize) -> *mut u8 {
        // SAFETY: forwarded under the caller's contract.
        let p = unsafe { System.realloc(ptr, layout, new_size) };
        if !p.is_null() {
            on_dealloc(layout.size());
            on_alloc(new_size);
            // A grown block is one allocation, not two.
            ALLOCATIONS.fetch_sub(1, Relaxed);
        }
        p
    }
}

/// Runs `f` as one export call: its wall time counts as engine time.
pub fn time_call<T>(f: impl FnOnce() -> T) -> T {
    let start = Instant::now();
    let result = f();
    ELAPSED_NS.fetch_add(start.elapsed().as_nanos() as u64, Relaxed);
    CALLS.fetch_add(1, Relaxed);
    result
}

/// Runs `f` as a call into the host: its wall time is subtracted from the engine time.
pub fn time_callback<T>(f: impl FnOnce() -> T) -> T {
    let start = Instant::now();
    let result = f();
    CALLBACK_NS.fetch_add(start.elapsed().as_nanos() as u64, Relaxed);
    result
}

/// The counters as `ilspy_stats` hands them to the host; `Interop/RustStats.cs` mirrors it.
#[repr(C)]
#[derive(Clone, Copy, Debug, Default)]
pub struct Stats {
    /// Bytes allocated since the process started.
    pub allocated_bytes: u64,
    /// Allocations since the process started (a reallocation counts once).
    pub allocations: u64,
    /// Bytes allocated and not yet freed.
    pub live_bytes: u64,
    /// The highest `live_bytes` since the process started or the last peak reset.
    pub peak_live_bytes: u64,
    /// Wall time inside the exports, callbacks into the host included.
    pub rust_elapsed_ns: u64,
    /// Wall time of the callbacks into the host made from inside the exports.
    pub callback_ns: u64,
    /// Export calls timed in `rust_elapsed_ns`.
    pub calls: u64,
}

pub fn snapshot() -> Stats {
    Stats {
        allocated_bytes: ALLOCATED.load(Relaxed),
        allocations: ALLOCATIONS.load(Relaxed),
        live_bytes: LIVE.load(Relaxed),
        peak_live_bytes: PEAK.load(Relaxed),
        rust_elapsed_ns: ELAPSED_NS.load(Relaxed),
        callback_ns: CALLBACK_NS.load(Relaxed),
        calls: CALLS.load(Relaxed),
    }
}

/// Starts a new peak at the bytes live now.
pub fn reset_peak() {
    PEAK.store(LIVE.load(Relaxed), Relaxed);
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn allocations_are_counted() {
        let before = snapshot();
        let v = vec![0u8; 1 << 20];
        let during = snapshot();
        drop(v);
        assert!(during.allocated_bytes - before.allocated_bytes >= 1 << 20);
        assert!(during.peak_live_bytes >= 1 << 20);
    }

    #[test]
    fn callbacks_are_timed_apart() {
        let before = snapshot();
        time_call(|| time_callback(|| std::thread::sleep(std::time::Duration::from_millis(2))));
        let after = snapshot();
        assert!(after.calls > before.calls);
        assert!(after.callback_ns - before.callback_ns >= 2_000_000);
        assert!(after.rust_elapsed_ns - before.rust_elapsed_ns >= 2_000_000);
    }
}
