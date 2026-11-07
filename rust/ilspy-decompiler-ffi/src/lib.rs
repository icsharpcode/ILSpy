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
use ilspy_decompiler::typesystem::compilation::{Compilation, ModuleId, ModuleInput};

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

/// Result of an export that does work. The host maps it to fallback, cancellation or an
/// exception (see the lecture, "FFI contract").
#[repr(i32)]
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum Status {
    Ok = 0,
    Unsupported = 1,
    Cancelled = 2,
    Error = 3,
    Panic = 4,
}

/// Runs `f`; an `Err` or a panic becomes `Status::Error` / `Status::Panic` plus a last-error
/// message.
fn run<T>(f: impl FnOnce() -> Result<T, String>) -> Result<T, Status> {
    install_panic_hook();
    match panic::catch_unwind(AssertUnwindSafe(f)) {
        Ok(Ok(value)) => Ok(value),
        Ok(Err(message)) => {
            set_last_error(message);
            Err(Status::Error)
        }
        Err(_) => {
            let report = PANIC_REPORT
                .with(|slot| slot.borrow_mut().take())
                .unwrap_or_else(|| "panic in the decompiler engine".to_owned());
            set_last_error(format!("panic: {report}"));
            Err(Status::Panic)
        }
    }
}

/// Runs `f`, turning an `Err` or a panic into `fallback` plus a last-error message.
fn guard<T>(fallback: T, f: impl FnOnce() -> Result<T, String>) -> T {
    run(f).unwrap_or(fallback)
}

/// An engine-allocated byte buffer handed to the host, which copies it and then calls
/// `ilspy_buf_free` exactly once.
#[repr(C)]
pub struct Buf {
    pub ptr: *mut u8,
    pub len: usize,
    pub cap: usize,
}

impl Buf {
    const EMPTY: Buf = Buf {
        ptr: ptr::null_mut(),
        len: 0,
        cap: 0,
    };

    fn from_vec(v: Vec<u8>) -> Buf {
        let mut v = std::mem::ManuallyDrop::new(v);
        Buf {
            ptr: v.as_mut_ptr(),
            len: v.len(),
            cap: v.capacity(),
        }
    }
}

/// Frees a buffer returned by the engine. A null buffer is ignored.
///
/// # Safety
/// `buf` must come from the engine and not have been freed yet.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn ilspy_buf_free(buf: Buf) {
    guard((), || {
        if !buf.ptr.is_null() {
            // SAFETY: `buf` was produced by `Buf::from_vec` and is freed once.
            drop(unsafe { Vec::from_raw_parts(buf.ptr, buf.len, buf.cap) });
        }
        Ok(())
    })
}

/// Debug export for the shim's `MetadataListingEquality` test: the canonical metadata listing
/// (`metadata::listing`) of a PE image, as UTF-8 in `*out`.
///
/// # Safety
/// `image` must point to `len` readable bytes; `out` must be writable.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn ilspy_debug_list_members(
    image: *const u8,
    len: usize,
    out: *mut Buf,
) -> Status {
    // SAFETY: the caller guarantees `out` is writable.
    unsafe { out.write(Buf::EMPTY) };
    match run(|| {
        if image.is_null() {
            return Err("null image".to_owned());
        }
        // SAFETY: the caller guarantees `image` points to `len` bytes for this call.
        let image = unsafe { std::slice::from_raw_parts(image, len) };
        Ok(ilspy_decompiler::metadata::listing::listing(image).into_bytes())
    }) {
        Ok(bytes) => {
            // SAFETY: as above.
            unsafe { out.write(Buf::from_vec(bytes)) };
            Status::Ok
        }
        Err(status) => status,
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

/// One module of a compilation as the host passes it. Reference tables hold, per AssemblyRef
/// or ModuleRef row, the index of the module it resolves to, or -1.
#[repr(C)]
pub struct FfiModule {
    pub image: *const u8,
    pub image_len: usize,
    pub assembly_name: *const u8,
    pub assembly_name_len: usize,
    pub assembly_refs: *const i32,
    pub assembly_ref_count: usize,
    pub module_refs: *const i32,
    pub module_ref_count: usize,
}

/// # Safety
/// `ptr` must point to `len` readable items, or `len` must be 0.
unsafe fn slice<'a, T>(ptr: *const T, len: usize) -> &'a [T] {
    if len == 0 {
        &[]
    } else {
        // SAFETY: guaranteed by the caller.
        unsafe { std::slice::from_raw_parts(ptr, len) }
    }
}

fn targets(raw: &[i32]) -> Vec<Option<ModuleId>> {
    raw.iter()
        .map(|&i| u32::try_from(i).ok().map(ModuleId))
        .collect()
}

/// Debug export for the shim's `TypeSystemListingEquality` test: the type-system listing
/// (`typesystem::listing`) of module 0 of the given compilation under `TypeSystemOptions`
/// `options`, as UTF-8 in `*out`.
///
/// # Safety
/// `modules` must point to `count` valid `FfiModule`s whose pointers are valid for the call;
/// `out` must be writable.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn ilspy_debug_type_listing(
    modules: *const FfiModule,
    count: usize,
    options: u32,
    out: *mut Buf,
) -> Status {
    // SAFETY: the caller guarantees `out` is writable.
    unsafe { out.write(Buf::EMPTY) };
    match run(|| {
        // SAFETY: the caller guarantees the module array and everything it points to.
        let inputs = unsafe { slice(modules, count) }
            .iter()
            .map(|m| unsafe {
                ModuleInput {
                    image: slice(m.image, m.image_len),
                    assembly_name: String::from_utf8_lossy(slice(
                        m.assembly_name,
                        m.assembly_name_len,
                    ))
                    .into_owned(),
                    assembly_refs: targets(slice(m.assembly_refs, m.assembly_ref_count)),
                    module_refs: targets(slice(m.module_refs, m.module_ref_count)),
                }
            })
            .collect();
        let comp = Compilation::new(inputs, options).map_err(|e| e.to_string())?;
        Ok(ilspy_decompiler::typesystem::listing::listing(&comp).into_bytes())
    }) {
        Ok(bytes) => {
            // SAFETY: as above.
            unsafe { out.write(Buf::from_vec(bytes)) };
            Status::Ok
        }
        Err(status) => status,
    }
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

#[cfg(test)]
mod buffer_tests {
    use super::*;

    #[test]
    fn listing_of_a_non_pe_is_returned_and_freed() {
        let mut buf = Buf::EMPTY;
        let image = b"not a PE";
        // SAFETY: `image` and `buf` are valid for the call.
        let status = unsafe { ilspy_debug_list_members(image.as_ptr(), image.len(), &mut buf) };
        assert_eq!(status, Status::Ok);
        // SAFETY: the engine wrote `len` initialized bytes.
        let text = unsafe { std::slice::from_raw_parts(buf.ptr, buf.len) };
        assert_eq!(text, b"BAD IMAGE\n");
        // SAFETY: freed once.
        unsafe { ilspy_buf_free(buf) };
    }
}
