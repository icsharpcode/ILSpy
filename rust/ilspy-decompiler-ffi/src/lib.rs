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
use std::sync::{Once, OnceLock};

use ilspy_decompiler::Session;
use ilspy_decompiler::il::debug_info::{DebugInfo, ExtraTypeInfo};
use ilspy_decompiler::il::reader::ReaderOptions;
use ilspy_decompiler::il::transforms::{
    ExtensionMethodQuery, FieldKind, Resolver, RootNames, SETTING_NAMES, Settings,
};
use ilspy_decompiler::typesystem::compilation::{Compilation, ModuleId, ModuleInput};
use ilspy_decompiler::typesystem::members::MethodDefId;
use ilspy_decompiler::typesystem::types::TypeSystem;

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

/// The host's C# resolver: answers the `resolver_query` message at `query` (`len` bytes). A
/// yes/no question is answered with 1 for yes and 0 for no; a question with data in its answer
/// (`wire::ilast::method_group_query`) writes up to `answer_capacity` bytes to `answer` and
/// returns the full answer's length, so a longer answer is asked for again with room for it. A
/// negative value is a failure, which the engine takes as no (or no data) and the host reports
/// after the call. It must not retain `query` or `answer`.
pub type ResolverCallback = unsafe extern "C" fn(
    ctx: *mut c_void,
    query: *const u8,
    len: usize,
    answer: *mut u8,
    answer_capacity: usize,
) -> i32;

struct HostResolver {
    ctx: *mut c_void,
    callback: ResolverCallback,
}

impl HostResolver {
    /// A yes/no or integer question.
    fn ask(&self, query: &[u8]) -> i32 {
        // SAFETY: the host contract above; `query` outlives the call and no answer is read.
        unsafe {
            (self.callback)(
                self.ctx,
                query.as_ptr(),
                query.len(),
                std::ptr::null_mut(),
                0,
            )
        }
    }

    /// A question answered with data; `None` when the host failed.
    fn ask_data(&self, query: &[u8]) -> Option<Vec<u8>> {
        let mut answer = vec![0u8; 256];
        loop {
            // SAFETY: the host contract above; `query` and `answer` outlive the call and the
            // host writes at most `answer.len()` bytes.
            let n = unsafe {
                (self.callback)(
                    self.ctx,
                    query.as_ptr(),
                    query.len(),
                    answer.as_mut_ptr(),
                    answer.len(),
                )
            };
            let n = usize::try_from(n).ok()?;
            if n <= answer.len() {
                answer.truncate(n);
                return Some(answer);
            }
            answer = vec![0u8; n];
        }
    }
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
            compilation: OnceLock::new(),
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

/// The `ModuleInput`s an export receives as an `FfiModule` array.
///
/// # Safety
/// `modules` must point to `count` valid `FfiModule`s whose pointers outlive the result.
unsafe fn module_inputs<'a>(
    modules: *const FfiModule,
    count: usize,
    options: u32,
) -> Vec<ModuleInput<'a>> {
    // SAFETY: the caller guarantees the module array and everything it points to.
    unsafe { slice(modules, count) }
        .iter()
        .map(|m| unsafe {
            ModuleInput {
                image: slice(m.image, m.image_len),
                assembly_name: String::from_utf8_lossy(slice(m.assembly_name, m.assembly_name_len))
                    .into_owned(),
                assembly_refs: targets(slice(m.assembly_refs, m.assembly_ref_count)),
                module_refs: targets(slice(m.module_refs, m.module_ref_count)),
                options,
            }
        })
        .collect()
}

/// Runs `f` on the compilation of `modules` and writes its bytes to `out`.
///
/// # Safety
/// As `module_inputs`; `out` must be writable.
unsafe fn with_compilation(
    modules: *const FfiModule,
    count: usize,
    options: u32,
    out: *mut Buf,
    f: impl FnOnce(&Compilation<'_>) -> Vec<u8>,
) -> Status {
    // SAFETY: the caller guarantees `out` is writable.
    unsafe { out.write(Buf::EMPTY) };
    match run(|| {
        // SAFETY: the caller guarantees the module array.
        let inputs = unsafe { module_inputs(modules, count, options) };
        let comp = Compilation::new(inputs).map_err(|e| e.to_string())?;
        Ok(f(&comp))
    }) {
        Ok(bytes) => {
            // SAFETY: as above.
            unsafe { out.write(Buf::from_vec(bytes)) };
            Status::Ok
        }
        Err(status) => status,
    }
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
    // SAFETY: forwarded from the caller.
    unsafe {
        with_compilation(modules, count, options, out, |comp| {
            ilspy_decompiler::typesystem::listing::listing(comp).into_bytes()
        })
    }
}

/// The host's `IDebugInfoProvider` for the main module. `kind` 0 asks for `TryGetName`
/// (UTF-8), 1 for `TryGetExtraTypeInfo` (see `decode_extra_type_info`). The host writes up to
/// `cap` bytes to `buf` and returns the full length, or -1 when it has no answer; a length
/// above `cap` makes the engine call again with a larger buffer.
pub type DebugInfoCallback = unsafe extern "C" fn(
    ctx: *mut c_void,
    method_token: u32,
    kind: u32,
    index: i32,
    buf: *mut u8,
    cap: usize,
) -> isize;

const DEBUG_INFO_NAME: u32 = 0;
const DEBUG_INFO_EXTRA_TYPE_INFO: u32 = 1;

struct HostDebugInfo {
    ctx: *mut c_void,
    callback: DebugInfoCallback,
}

impl HostDebugInfo {
    fn fetch(&self, method_token: u32, kind: u32, index: i32) -> Option<Vec<u8>> {
        let mut buf = vec![0u8; 256];
        loop {
            // SAFETY: the host contract above; `buf` is writable for its length.
            let n = unsafe {
                (self.callback)(
                    self.ctx,
                    method_token,
                    kind,
                    index,
                    buf.as_mut_ptr(),
                    buf.len(),
                )
            };
            let n = usize::try_from(n).ok()?;
            if n <= buf.len() {
                buf.truncate(n);
                return Some(buf);
            }
            buf.resize(n, 0);
        }
    }
}

impl DebugInfo for HostDebugInfo {
    fn local_name(&self, method_token: u32, index: i32) -> Option<String> {
        self.fetch(method_token, DEBUG_INFO_NAME, index)
            .map(|b| String::from_utf8_lossy(&b).into_owned())
    }

    fn extra_type_info(&self, method_token: u32, index: i32) -> Option<ExtraTypeInfo> {
        decode_extra_type_info(&self.fetch(method_token, DEBUG_INFO_EXTRA_TYPE_INFO, index)?)
    }
}

/// `PdbExtraTypeInfo` as `Interop/RustDebugInfo.cs` writes it, little-endian: the dynamic flag
/// count (-1 for none) and one byte per flag, then the tuple element name count (-1 for none)
/// and per name its UTF-8 length (-1 for null) and bytes. Malformed input yields `None`.
fn decode_extra_type_info(mut b: &[u8]) -> Option<ExtraTypeInfo> {
    fn count(b: &mut &[u8]) -> Option<Option<usize>> {
        let (head, rest) = b.split_first_chunk::<4>()?;
        *b = rest;
        let n = i32::from_le_bytes(*head);
        Some(usize::try_from(n).ok())
    }
    fn bytes<'a>(b: &mut &'a [u8], n: usize) -> Option<&'a [u8]> {
        let (head, rest) = b.split_at_checked(n)?;
        *b = rest;
        Some(head)
    }
    let dynamic_flags = match count(&mut b)? {
        Some(n) => Some(bytes(&mut b, n)?.iter().map(|&x| x != 0).collect()),
        None => None,
    };
    let tuple_element_names = match count(&mut b)? {
        Some(n) => Some(
            (0..n)
                .map(|_| {
                    Some(match count(&mut b)? {
                        Some(len) => {
                            Some(String::from_utf8_lossy(bytes(&mut b, len)?).into_owned())
                        }
                        None => None,
                    })
                })
                .collect::<Option<Vec<_>>>()?,
        ),
        None => None,
    };
    b.is_empty().then_some(ExtraTypeInfo {
        tuple_element_names,
        dynamic_flags,
    })
}

impl Resolver for HostResolver {
    fn can_transform_to_extension_method_call(
        &self,
        ts: &TypeSystem<'_, '_>,
        query: &ExtensionMethodQuery<'_>,
    ) -> bool {
        let Ok(bytes) = ilspy_decompiler::wire::ilast::resolver_query(ts, query) else {
            return false;
        };
        self.ask(&bytes) == 1
    }

    fn field_reference_binds(
        &self,
        ts: &TypeSystem<'_, '_>,
        field: &ilspy_decompiler::il::ast::FieldOperand,
        target: Option<ilspy_decompiler::il::transforms::LookupTarget>,
    ) -> bool {
        let Ok(bytes) = ilspy_decompiler::wire::ilast::field_reference_query(ts, field, target)
        else {
            return false;
        };
        self.ask(&bytes) == 1
    }

    /// The answer is 0 for an ordinary field, 1 for the `field` keyword, 3 for a field named
    /// `field` in an accessor, `0x2000_0000 | row` for the backing field of the automatic
    /// property at Property `row` and `0x1000_0000 | row` for that of the automatic event at
    /// Event `row`; a failed callback answers as for an ordinary field (the host rethrows).
    fn field_kind(
        &self,
        ts: &TypeSystem<'_, '_>,
        field: &ilspy_decompiler::il::ast::FieldOperand,
        target_is_this: bool,
    ) -> FieldKind {
        let Ok(bytes) = ilspy_decompiler::wire::ilast::field_kind_query(ts, field, target_is_this)
        else {
            return FieldKind::Ordinary;
        };
        let answer = self.ask(&bytes);
        match answer {
            1 => FieldKind::FieldKeyword,
            3 => FieldKind::NamedFieldInAccessor,
            a if a & 0x3000_0000 == 0x2000_0000 => FieldKind::AutoPropertyBackingField {
                property_row: (a & 0x00FF_FFFF) as u32,
            },
            a if a & 0x3000_0000 == 0x1000_0000 => FieldKind::AutoEventBackingField {
                event_row: (a & 0x00FF_FFFF) as u32,
            },
            _ => FieldKind::Ordinary,
        }
    }

    fn method_group(
        &self,
        ts: &TypeSystem<'_, '_>,
        name: &str,
        target: Option<ilspy_decompiler::il::transforms::LookupTarget>,
        type_arguments: &[ilspy_decompiler::typesystem::types::TypeId],
    ) -> Option<Vec<ilspy_decompiler::il::transforms::MethodGroupList>> {
        let bytes =
            ilspy_decompiler::wire::ilast::method_group_query(ts, name, target, type_arguments)
                .ok()?;
        let answer = self.ask_data(&bytes)?;
        ilspy_decompiler::wire::ilast::method_group_answer(&answer)
    }

    fn accessor_reference(
        &self,
        ts: &TypeSystem<'_, '_>,
        accessor: &ilspy_decompiler::il::ast::MethodOperand,
        target: Option<ilspy_decompiler::il::transforms::LookupTarget>,
        callvirt: bool,
    ) -> ilspy_decompiler::il::transforms::AccessorBinding {
        use ilspy_decompiler::il::transforms::AccessorBinding;
        let Ok(bytes) =
            ilspy_decompiler::wire::ilast::accessor_reference_query(ts, accessor, target, callvirt)
        else {
            return AccessorBinding::None;
        };
        match self.ask(&bytes) {
            1 => AccessorBinding::Owner,
            2 => AccessorBinding::Other,
            _ => AccessorBinding::None,
        }
    }

    fn can_use_extension_method_syntax(
        &self,
        ts: &TypeSystem<'_, '_>,
        method: &ilspy_decompiler::il::ast::MethodOperand,
        method_args: Option<&[ilspy_decompiler::typesystem::types::TypeId]>,
        type_arguments: &[ilspy_decompiler::typesystem::types::TypeId],
        target: &ilspy_decompiler::semantics::ResolveResult,
        arguments: &[ilspy_decompiler::semantics::ResolveResult],
        method_groups: &[Option<ilspy_decompiler::il::transforms::MethodGroupOperand<'_>>],
        names: Option<&[Option<String>]>,
    ) -> Option<bool> {
        let bytes = ilspy_decompiler::wire::ilast::extension_method_syntax_query(
            ts,
            method,
            method_args,
            type_arguments,
            target,
            arguments,
            method_groups,
            names,
        )
        .ok()?;
        // Bit 0 is the answer; bit 1 + i whether the host's resolution tried to convert the
        // i-th lambda argument to a delegate of other parameter types.
        let answer = self.ask(&bytes);
        if answer < 0 {
            return None;
        }
        let lambdas = arguments.iter().filter_map(|a| a.as_lambda()?.decompiled());
        for (i, l) in lambdas.enumerate() {
            if answer & (2 << i) != 0 {
                l.attempted_conversion_with_type_mismatch.set(true);
            }
        }
        Some(answer & 1 != 0)
    }

    fn constructors(
        &self,
        ts: &TypeSystem<'_, '_>,
        constructor: &ilspy_decompiler::il::ast::MethodOperand,
    ) -> Option<Vec<ilspy_decompiler::il::transforms::MethodGroupList>> {
        let bytes = ilspy_decompiler::wire::ilast::constructors_query(ts, constructor).ok()?;
        let answer = self.ask_data(&bytes)?;
        ilspy_decompiler::wire::ilast::method_group_answer(&answer)
    }

    fn method_group_reference(
        &self,
        ts: &TypeSystem<'_, '_>,
        name: &str,
        target: Option<ilspy_decompiler::il::transforms::LookupTarget>,
        type_arguments: &[ilspy_decompiler::typesystem::types::TypeId],
    ) -> Option<Vec<ilspy_decompiler::il::transforms::MethodGroupList>> {
        let bytes = ilspy_decompiler::wire::ilast::method_group_reference_query(
            ts,
            name,
            target,
            type_arguments,
        )
        .ok()?;
        let answer = self.ask_data(&bytes)?;
        ilspy_decompiler::wire::ilast::method_group_answer(&answer)
    }

    fn indexers(
        &self,
        ts: &TypeSystem<'_, '_>,
        target: ilspy_decompiler::il::transforms::LookupTarget,
    ) -> Option<Vec<ilspy_decompiler::il::transforms::MethodGroupList>> {
        let bytes = ilspy_decompiler::wire::ilast::indexers_query(ts, target).ok()?;
        let answer = self.ask_data(&bytes)?;
        ilspy_decompiler::wire::ilast::method_group_answer(&answer)
    }

    fn types_accessible(
        &self,
        ts: &TypeSystem<'_, '_>,
        types: &[ilspy_decompiler::typesystem::types::TypeId],
    ) -> bool {
        let Ok(bytes) = ilspy_decompiler::wire::ilast::types_accessible_query(ts, types) else {
            return false;
        };
        self.ask(&bytes) == 1
    }

    fn singularize(&self, ts: &TypeSystem<'_, '_>, word: &str) -> Option<String> {
        let bytes = ilspy_decompiler::wire::ilast::singularize_query(ts, word).ok()?;
        String::from_utf8(self.ask_data(&bytes)?).ok()
    }
}

/// The names AssignVariableNames' root scope collects, as `RustEngine.RootNames` writes them:
/// two lists, each a count and length-prefixed UTF-8 strings, as unsigned LEB128.
fn decode_root_names(bytes: &[u8]) -> Option<RootNames> {
    let mut pos = 0;
    let uvar = |pos: &mut usize| -> Option<usize> {
        let mut value = 0usize;
        let mut shift = 0;
        loop {
            let b = *bytes.get(*pos)?;
            *pos += 1;
            value |= usize::from(b & 0x7F).checked_shl(shift)?;
            if b & 0x80 == 0 {
                return Some(value);
            }
            shift += 7;
        }
    };
    let mut lists = [Vec::new(), Vec::new()];
    for list in &mut lists {
        for _ in 0..uvar(&mut pos)? {
            let len = uvar(&mut pos)?;
            let text = bytes.get(pos..pos.checked_add(len)?)?;
            pos += len;
            list.push(String::from_utf8_lossy(text).into_owned());
        }
    }
    let [member_names, reserved_names] = lists;
    Some(RootNames {
        member_names,
        reserved_names,
    })
}

/// Debug export for the shim's `IlAstReadEquality` test: the IL reader's function for every
/// method body of module 0, as wire v2 (`wire::ilast`). Bit 0 of `reader_flags` is
/// `UseRefLocalsForAccurateOrderOfEvaluation`; a non-null `debug_info` is called with
/// `debug_ctx` as the reader's debug symbols.
///
/// # Safety
/// As `ilspy_debug_type_listing`; `debug_info` must honor the `DebugInfoCallback` contract.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn ilspy_debug_read_il(
    modules: *const FfiModule,
    count: usize,
    options: u32,
    reader_flags: u32,
    debug_ctx: *mut c_void,
    debug_info: Option<DebugInfoCallback>,
    out: *mut Buf,
) -> Status {
    let host = debug_info.map(|callback| HostDebugInfo {
        ctx: debug_ctx,
        callback,
    });
    let reader = ReaderOptions {
        use_ref_locals_for_accurate_order_of_evaluation: reader_flags & 1 != 0,
        debug_info: host.as_ref().map(|h| h as &dyn DebugInfo),
    };
    // SAFETY: forwarded from the caller.
    unsafe {
        with_compilation(modules, count, options, out, |comp| {
            let mut ts = TypeSystem::new(comp);
            ilspy_decompiler::wire::ilast::read_module(&mut ts, ModuleId(0), reader)
        })
    }
}

/// The session as the host sees it: an opaque pointer.
pub struct FfiSession {
    _ctx: *mut c_void,
    _session: Session,
    /// Set once by `ilspy_session_load`. The images are borrowed from the host, which keeps
    /// them alive for the session's lifetime; everything else was copied.
    compilation: OnceLock<Compilation<'static>>,
}

/// `ilspy_il_prefix`: the number of IL transforms the engine serves (`IL_PREFIX`).
#[unsafe(no_mangle)]
pub extern "C" fn ilspy_il_prefix() -> u32 {
    ilspy_decompiler::IL_PREFIX
}

/// The `DecompilerSettings` property names behind the bits of the `settings` argument of
/// `ilspy_session_read_function`, in bit order, separated by `\n`, as a static NUL-terminated
/// UTF-8 string. Never free it.
#[unsafe(no_mangle)]
pub extern "C" fn ilspy_setting_names() -> *const c_char {
    static NAMES: OnceLock<CString> = OnceLock::new();
    NAMES
        .get_or_init(|| CString::new(SETTING_NAMES.join("\n")).expect("no NUL in names"))
        .as_ptr()
}

/// Gives the session its compilation: the modules as for `ilspy_debug_type_listing`, main
/// module first, under `TypeSystemOptions` `options`. Only the images must stay valid after
/// the call, until the session is freed. A session is loaded at most once.
///
/// # Safety
/// `session` must be a live session; `modules` must point to `count` valid `FfiModule`s
/// whose images outlive the session.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn ilspy_session_load(
    session: *mut FfiSession,
    modules: *const FfiModule,
    count: usize,
    options: u32,
) -> Status {
    match run(|| {
        // SAFETY: the caller guarantees a live session.
        let session = unsafe { session.as_ref() }.ok_or("null session")?;
        // SAFETY: the caller guarantees the module array; the images outlive the session.
        let inputs = unsafe { module_inputs(modules, count, options) };
        let comp = Compilation::new(inputs).map_err(|e| e.to_string())?;
        session
            .compilation
            .set(comp)
            .map_err(|_| "the session is already loaded".to_owned())
    }) {
        Ok(()) => Status::Ok,
        Err(status) => status,
    }
}

/// Seam 1: the function of the main module's method `method_token` after the first
/// `il_prefix` IL transforms, as a one-entry wire v2 message (`wire::ilast::read_function`).
/// `Unsupported` when `il_prefix` exceeds `ilspy_il_prefix()`. `settings` has bit `i` set when
/// the setting `ilspy_setting_names()` lists at `i` is on. `reader_flags`, `debug_ctx` and
/// `debug_info` as for `ilspy_debug_read_il`; bit 1 of `reader_flags` reads for analysis
/// (`wire::ilast::read_function`); bit 2 adds the C# translation of the body (`wire::ast`).
/// `resolver` answers the C# resolver questions
/// (`resolver_ctx` is passed back); without it the engine finds no extension methods.
/// `root_names` (`root_names_len` bytes, or null) are the names AssignVariableNames' root
/// scope collects (`decode_root_names`). Safe to call from several threads at once.
///
/// # Safety
/// `session` must be a live, loaded session; `out` must be writable; `debug_info` must honor
/// the `DebugInfoCallback` contract and `resolver` the `ResolverCallback` one. `root_names` must
/// be readable for `root_names_len` bytes unless it is null.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn ilspy_session_read_function(
    session: *const FfiSession,
    method_token: u32,
    il_prefix: u32,
    reader_flags: u32,
    settings: u64,
    settings_high: u64,
    debug_ctx: *mut c_void,
    debug_info: Option<DebugInfoCallback>,
    resolver_ctx: *mut c_void,
    resolver: Option<ResolverCallback>,
    root_names: *const u8,
    root_names_len: usize,
    out: *mut Buf,
) -> Status {
    // SAFETY: the caller guarantees `out` is writable.
    unsafe { out.write(Buf::EMPTY) };
    if il_prefix > ilspy_decompiler::IL_PREFIX {
        set_last_error(format!(
            "IL prefix {il_prefix} is beyond the engine's {}",
            ilspy_decompiler::IL_PREFIX
        ));
        return Status::Unsupported;
    }
    let host = debug_info.map(|callback| HostDebugInfo {
        ctx: debug_ctx,
        callback,
    });
    let resolver = resolver.map(|callback| HostResolver {
        ctx: resolver_ctx,
        callback,
    });
    // SAFETY: the caller passes `root_names_len` readable bytes (or none).
    let root_names = (!root_names.is_null() && root_names_len > 0)
        .then(|| unsafe { std::slice::from_raw_parts(root_names, root_names_len) })
        .and_then(decode_root_names);
    let reader = ReaderOptions {
        use_ref_locals_for_accurate_order_of_evaluation: reader_flags & 1 != 0,
        debug_info: host.as_ref().map(|h| h as &dyn DebugInfo),
    };
    match run(|| {
        // SAFETY: the caller guarantees a live session.
        let session = unsafe { session.as_ref() }.ok_or("null session")?;
        let comp = session
            .compilation
            .get()
            .ok_or("the session has no compilation")?;
        if method_token >> 24 != 0x06 {
            return Err(format!("0x{method_token:08x} is not a MethodDef token"));
        }
        let method = MethodDefId {
            module: ModuleId(0),
            row: method_token & 0x00FF_FFFF,
        };
        let mut ts = TypeSystem::new(comp);
        ilspy_decompiler::wire::ilast::read_function(
            &mut ts,
            method,
            reader,
            il_prefix,
            Settings::from_bits(settings, settings_high),
            resolver.as_ref().map(|r| r as &dyn Resolver),
            root_names.as_ref(),
            reader_flags & 2 != 0,
            reader_flags & 4 != 0,
        )
    }) {
        Ok(bytes) => {
            // SAFETY: as above.
            unsafe { out.write(Buf::from_vec(bytes)) };
            Status::Ok
        }
        Err(status) => status,
    }
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

#[cfg(test)]
mod debug_info_tests {
    use super::*;

    fn le(n: i32) -> [u8; 4] {
        n.to_le_bytes()
    }

    #[test]
    fn extra_type_info_decodes_flags_and_names() {
        let mut b = Vec::new();
        b.extend(le(2));
        b.extend([0, 1]);
        b.extend(le(2));
        b.extend(le(-1));
        b.extend(le(1));
        b.push(b'a');
        assert_eq!(
            decode_extra_type_info(&b),
            Some(ExtraTypeInfo {
                dynamic_flags: Some(vec![false, true]),
                tuple_element_names: Some(vec![None, Some("a".to_owned())]),
            })
        );
        let mut none = Vec::new();
        none.extend(le(-1));
        none.extend(le(-1));
        assert_eq!(
            decode_extra_type_info(&none),
            Some(ExtraTypeInfo::default())
        );
        assert_eq!(decode_extra_type_info(&b[..b.len() - 1]), None);
        b.push(0);
        assert_eq!(decode_extra_type_info(&b), None);
    }

    /// Answers names longer than the first buffer, so the engine has to call again.
    unsafe extern "C" fn long_names(
        _: *mut c_void,
        method_token: u32,
        kind: u32,
        index: i32,
        buf: *mut u8,
        cap: usize,
    ) -> isize {
        if kind != DEBUG_INFO_NAME || index < 0 {
            return -1;
        }
        let name = format!("{method_token:x}").repeat(100) + &index.to_string();
        if name.len() <= cap {
            // SAFETY: `buf` is writable for `cap` bytes.
            unsafe { ptr::copy_nonoverlapping(name.as_ptr(), buf, name.len()) };
        }
        name.len() as isize
    }

    #[test]
    fn host_debug_info_grows_the_buffer() {
        let host = HostDebugInfo {
            ctx: ptr::null_mut(),
            callback: long_names,
        };
        assert_eq!(
            host.local_name(0x0600_0001, 3),
            Some("6000001".repeat(100) + "3")
        );
        assert_eq!(host.local_name(0x0600_0001, -1), None);
        assert_eq!(host.extra_type_info(0x0600_0001, 0), None);
    }
}
