//! C ABI of the ILSpy decompiler engine.
//!
//! Every export catches panics, so no unwind ever crosses the boundary. Failures are reported
//! through return values plus `ilspy_last_error`, which reads a thread-local slot: errors of
//! `ilspy_session_new` have no session to live in, and concurrent requests on one session must
//! not overwrite each other's message.
#![allow(unsafe_code)]

use std::backtrace::Backtrace;
use std::cell::RefCell;
use std::ffi::{CString, c_char, c_void};
use std::panic::{self, AssertUnwindSafe};
use std::ptr;
use std::sync::Once;

use ilspy_decompiler::Session;

static VERSION_C: &str = concat!(env!("CARGO_PKG_VERSION"), "\0");

thread_local! {
    static LAST_ERROR: RefCell<Option<CString>> = const { RefCell::new(None) };
    static PANIC_REPORT: RefCell<Option<String>> = const { RefCell::new(None) };
}

fn set_last_error(message: String) {
    // Interior NULs would truncate the message on the C side; replace them instead of failing.
    let message = CString::new(message.replace('\0', "\u{FFFD}")).unwrap_or_default();
    LAST_ERROR.with(|slot| *slot.borrow_mut() = Some(message));
}

fn install_panic_hook() {
    static HOOK: Once = Once::new();
    HOOK.call_once(|| {
        panic::set_hook(Box::new(|info| {
            let report = format!("{info}\n{}", Backtrace::force_capture());
            PANIC_REPORT.with(|slot| *slot.borrow_mut() = Some(report));
        }));
    });
}

/// Runs `f`, turning an `Err` or a panic into `fallback` plus a last-error message.
fn guard<T>(fallback: T, f: impl FnOnce() -> Result<T, String>) -> T {
    install_panic_hook();
    match panic::catch_unwind(AssertUnwindSafe(f)) {
        Ok(Ok(value)) => value,
        Ok(Err(message)) => {
            set_last_error(message);
            fallback
        }
        Err(_) => {
            let report = PANIC_REPORT
                .with(|slot| slot.borrow_mut().take())
                .unwrap_or_else(|| "panic in the decompiler engine".to_owned());
            set_last_error(format!("panic: {report}"));
            fallback
        }
    }
}

/// The engine version as a static NUL-terminated UTF-8 string. Never free it.
#[unsafe(no_mangle)]
pub extern "C" fn ilspy_version() -> *const c_char {
    VERSION_C.as_ptr().cast()
}

/// Creates a session. `ctx` is passed back to host callbacks unchanged. Returns null on
/// failure; `ilspy_last_error` then holds the reason.
#[unsafe(no_mangle)]
pub extern "C" fn ilspy_session_new(ctx: *mut c_void) -> *mut FfiSession {
    guard(ptr::null_mut(), || {
        Ok(Box::into_raw(Box::new(FfiSession {
            _ctx: ctx,
            _session: Session::new(),
        })))
    })
}

/// Frees a session created by `ilspy_session_new`. Null is ignored.
///
/// # Safety
/// `session` must be null or a pointer returned by `ilspy_session_new` that was not freed yet.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn ilspy_session_free(session: *mut FfiSession) {
    guard((), || {
        if !session.is_null() {
            // SAFETY: the caller guarantees `session` came from `Box::into_raw` and is live.
            drop(unsafe { Box::from_raw(session) });
        }
        Ok(())
    })
}

/// The last error reported on the calling thread, or null. The pointer stays valid until the
/// next export call on the same thread.
#[unsafe(no_mangle)]
pub extern "C" fn ilspy_last_error() -> *const c_char {
    LAST_ERROR.with(|slot| slot.borrow().as_ref().map_or(ptr::null(), |s| s.as_ptr()))
}

/// The session as the host sees it: an opaque pointer.
pub struct FfiSession {
    _ctx: *mut c_void,
    _session: Session,
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::ffi::CStr;

    fn last_error() -> Option<String> {
        let p = ilspy_last_error();
        // SAFETY: a non-null result points into this thread's LAST_ERROR slot.
        (!p.is_null()).then(|| unsafe { CStr::from_ptr(p) }.to_string_lossy().into_owned())
    }

    #[test]
    fn version_matches_manifest() {
        // SAFETY: VERSION_C is a static NUL-terminated string.
        let v = unsafe { CStr::from_ptr(ilspy_version()) };
        assert_eq!(v.to_str().unwrap(), ilspy_decompiler::VERSION);
    }

    #[test]
    fn session_roundtrip() {
        let s = ilspy_session_new(ptr::null_mut());
        assert!(!s.is_null());
        // SAFETY: `s` was just created and is freed exactly once.
        unsafe { ilspy_session_free(s) };
        // SAFETY: null is explicitly allowed.
        unsafe { ilspy_session_free(ptr::null_mut()) };
    }

    #[test]
    fn guard_reports_errors_and_panics() {
        assert_eq!(guard(7, || Err("bad input".to_owned())), 7);
        assert_eq!(last_error().as_deref(), Some("bad input"));

        assert_eq!(guard(8, || panic!("boom")), 8);
        let report = last_error().unwrap();
        assert!(report.starts_with("panic: "), "{report}");
        assert!(report.contains("boom"), "{report}");
    }
}
