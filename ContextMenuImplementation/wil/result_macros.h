//*********************************************************
//
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
//
//*********************************************************
#ifndef __WIL_RESULTMACROS_INCLUDED
#define __WIL_RESULTMACROS_INCLUDED

// WARNING:
// Code within this scope must satisfy both C99 and C++

#include "common.h"

#if !defined(__WIL_MIN_KERNEL) && !defined(WIL_KERNEL_MODE)
#include <Windows.h>
#endif

// Setup the debug behavior. For kernel-mode, we ignore NDEBUG because that gets set automatically
// for driver projects. We mimic the behavior of NT_ASSERT which checks only for DBG.
// RESULT_NO_DEBUG is provided as an opt-out mechanism.
#ifndef RESULT_DEBUG
#if (DBG || defined(DEBUG) || defined(_DEBUG)) && !defined(RESULT_NO_DEBUG) && (defined(WIL_KERNEL_MODE) || !defined(NDEBUG))
#define RESULT_DEBUG
#endif
#endif

/// @cond
#if defined(_PREFAST_)
#define __WI_ANALYSIS_ASSUME(_exp)                          _Analysis_assume_(_exp)
#else
#ifdef RESULT_DEBUG
#define __WI_ANALYSIS_ASSUME(_exp)                          ((void) 0)
#else
// NOTE: Clang does not currently handle __noop correctly and will fail to compile if the argument is not copy
//       constructible. Therefore, use 'sizeof' for syntax validation. We don't do this universally for all compilers
//       since lambdas are not allowed in unevaluated contexts prior to C++20, which does not appear to affect __noop
#if !defined(_MSC_VER) || defined(__clang__)
#define __WI_ANALYSIS_ASSUME(_exp)                          ((void)sizeof(!(_exp))) // Validate syntax on non-debug builds
#else
#define __WI_ANALYSIS_ASSUME(_exp)                          __noop(_exp)
#endif
#endif
#endif // _PREFAST_

//*****************************************************************************
// Assert Macros
//*****************************************************************************

#ifdef RESULT_DEBUG
#if defined(__clang__) && defined(_WIN32)
// Clang currently mis-handles '__annotation' for 32-bit - https://bugs.llvm.org/show_bug.cgi?id=41890
#define __WI_ASSERT_FAIL_ANNOTATION(msg) (void)0
#else
#define __WI_ASSERT_FAIL_ANNOTATION(msg) __annotation(L"Debug", L"AssertFail", msg)
#endif

#define WI_ASSERT(condition)                                (__WI_ANALYSIS_ASSUME(condition), ((!(condition)) ? (__WI_ASSERT_FAIL_ANNOTATION(L"" #condition), DbgRaiseAssertionFailure(), FALSE) : TRUE))
#define WI_ASSERT_MSG(condition, msg)                       (__WI_ANALYSIS_ASSUME(condition), ((!(condition)) ? (__WI_ASSERT_FAIL_ANNOTATION(L##msg), DbgRaiseAssertionFailure(), FALSE) : TRUE))
#define WI_ASSERT_NOASSUME                                  WI_ASSERT
#define WI_ASSERT_MSG_NOASSUME                              WI_ASSERT_MSG
#define WI_VERIFY                                           WI_ASSERT
#define WI_VERIFY_MSG                                       WI_ASSERT_MSG
#define WI_VERIFY_SUCCEEDED(condition)                      WI_ASSERT(SUCCEEDED(condition))
#else
#define WI_ASSERT(condition)                                (__WI_ANALYSIS_ASSUME(condition), 0)
#define WI_ASSERT_MSG(condition, msg)                       (__WI_ANALYSIS_ASSUME(condition), 0)
#define WI_ASSERT_NOASSUME(condition)                       ((void) 0)
#define WI_ASSERT_MSG_NOASSUME(condition, msg)              ((void) 0)
#define WI_VERIFY(condition)                                (__WI_ANALYSIS_ASSUME(condition), ((condition) ? TRUE : FALSE))
#define WI_VERIFY_MSG(condition, msg)                       (__WI_ANALYSIS_ASSUME(condition), ((condition) ? TRUE : FALSE))
#define WI_VERIFY_SUCCEEDED(condition)                      (__WI_ANALYSIS_ASSUME(SUCCEEDED(condition)), ((SUCCEEDED(condition)) ? TRUE : FALSE))
#endif // RESULT_DEBUG

#if !defined(_NTDEF_)
typedef _Return_type_success_(return >= 0) LONG NTSTATUS;
#endif
#ifndef STATUS_SUCCESS
#define STATUS_SUCCESS              ((NTSTATUS)0x00000000L)
#endif
#ifndef STATUS_UNSUCCESSFUL
#define STATUS_UNSUCCESSFUL         ((NTSTATUS)0xC0000001L)
#endif
#ifndef __NTSTATUS_FROM_WIN32
#define __NTSTATUS_FROM_WIN32(x) ((NTSTATUS)(x) <= 0 ? ((NTSTATUS)(x)) : ((NTSTATUS) (((x) & 0x0000FFFF) | (FACILITY_WIN32 << 16) | ERROR_SEVERITY_ERROR)))
#endif

#ifndef WIL_AllocateMemory
#ifdef _KERNEL_MODE
#define WIL_AllocateMemory(SIZE)    ExAllocatePoolWithTag(NonPagedPoolNx, SIZE, 'LIW')
WI_ODR_PRAGMA("WIL_AllocateMemory", "2")
#else
#define WIL_AllocateMemory(SIZE)    HeapAlloc(GetProcessHeap(), 0, SIZE)
WI_ODR_PRAGMA("WIL_AllocateMemory", "1")
#endif
#else
WI_ODR_PRAGMA("WIL_AllocateMemory", "0")
#endif

#ifndef WIL_FreeMemory
#ifdef _KERNEL_MODE
#define WIL_FreeMemory(MEM)         ExFreePoolWithTag(MEM, 'LIW')
WI_ODR_PRAGMA("WIL_FreeMemory", "2")
#else
#define WIL_FreeMemory(MEM)         HeapFree(GetProcessHeap(), 0, MEM)
WI_ODR_PRAGMA("WIL_FreeMemory", "1")
#endif
#else
WI_ODR_PRAGMA("WIL_FreeMemory", "0")
#endif

// It would appear as though the C++17 "noexcept is part of the type system" update in MSVC has "infected" the behavior
// when compiling with C++14 (the default...), however the updated behavior for decltype understanding noexcept is _not_
// present... So, work around it
#if __WI_LIBCPP_STD_VER >= 17
#define WI_PFN_NOEXCEPT WI_NOEXCEPT
#else
#define WI_PFN_NOEXCEPT
#endif
/// @endcond

#if defined(__cplusplus) && !defined(__WIL_MIN_KERNEL) && !defined(WIL_KERNEL_MODE)

#include <strsafe.h>
#include <intrin.h>     // provides the _ReturnAddress() intrinsic
#include <new.h>        // provides 'operator new', 'std::nothrow', etc.
#if defined(WIL_ENABLE_EXCEPTIONS) && !defined(WIL_SUPPRESS_NEW)
#include <new>          // provides std::bad_alloc in the windows and public CRT headers
#endif

#pragma warning(push)
#pragma warning(disable:4714 6262)    // __forceinline not honored, stack size

//*****************************************************************************
// Behavioral setup (error handling macro configuration)
//*****************************************************************************
// Set any of the following macros to the values given below before including Result.h to
// control the error handling macro's trade-offs between diagnostics and performance

// RESULT_DIAGNOSTICS_LEVEL
// This define controls the level of diagnostic instrumentation that is built into the binary as a
// byproduct of using the macros.  The amount of diagnostic instrumentation that is supplied is
// a trade-off between diagnosibility of issues and code size and performance.  The modes are:
//      0   - No diagnostics, smallest & fastest (subject to tail-merge)
//      1   - No diagnostics, unique call sites for each macro (defeat's tail-merge)
//      2   - Line number
//      3   - Line number + source filename
//      4   - Line number + source filename + function name
//      5   - Line number + source filename + function name + code within the macro
// By default, mode 3 is used in free builds and mode 5 is used in checked builds.  Note that the
// _ReturnAddress() will always be available through all modes when possible.

// RESULT_INCLUDE_CALLER_RETURNADDRESS
// This controls whether or not the _ReturnAddress() of the function that includes the macro will
// be reported to telemetry.  Note that this is in addition to the _ReturnAddress() of the actual
// macro position (which is always reported).  The values are:
//      0   - The address is not included
//      1   - The address is included
// The default value is '1'.

// RESULT_INLINE_ERROR_TESTS
// For conditional macros (other than RETURN_XXX), this controls whether branches will be evaluated
// within the call containing the macro or will be forced into the function called by the macros.
// Pushing branching into the called function reduces code size and the number of unique branches
// evaluated, but increases the instruction count executed per macro.
//      0   - Branching will not happen inline to the macros
//      1   - Branching is pushed into the calling function via __forceinline
// The default value is '1'.  Note that XXX_MSG functions are always effectively mode '0' due to the
// compiler's unwillingness to inline var-arg functions.

// RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST
// RESULT_INCLUDE_CALLER_RETURNADDRESS_FAIL_FAST
// RESULT_INLINE_ERROR_TESTS_FAIL_FAST
// These defines are identical to those above in form/function, but only applicable to fail fast error
// handling allowing a process to have different diagnostic information and performance characteristics
// for fail fast than for other error handling given the different reporting infrastructure (Watson
// vs Telemetry).

// Set the default diagnostic mode
// Note that RESULT_DEBUG_INFO and RESULT_SUPPRESS_DEBUG_INFO are older deprecated models of controlling mode
#ifndef RESULT_DIAGNOSTICS_LEVEL
#if (defined(RESULT_DEBUG) || defined(RESULT_DEBUG_INFO)) && !defined(RESULT_SUPPRESS_DEBUG_INFO)
#define RESULT_DIAGNOSTICS_LEVEL 5
#else
#define RESULT_DIAGNOSTICS_LEVEL 3
#endif
#endif
#ifndef RESULT_INCLUDE_CALLER_RETURNADDRESS
#define RESULT_INCLUDE_CALLER_RETURNADDRESS 1
#endif
#ifndef RESULT_INLINE_ERROR_TESTS
#define RESULT_INLINE_ERROR_TESTS 1
#endif
#ifndef RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST
#define RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST RESULT_DIAGNOSTICS_LEVEL
#endif
#ifndef RESULT_INCLUDE_CALLER_RETURNADDRESS_FAIL_FAST
#define RESULT_INCLUDE_CALLER_RETURNADDRESS_FAIL_FAST RESULT_INCLUDE_CALLER_RETURNADDRESS
#endif
#ifndef RESULT_INLINE_ERROR_TESTS_FAIL_FAST
#define RESULT_INLINE_ERROR_TESTS_FAIL_FAST RESULT_INLINE_ERROR_TESTS
#endif


//*****************************************************************************
// Win32 specific error macros
//*****************************************************************************

#define FAILED_WIN32(win32err)                              ((win32err) != 0)
#define SUCCEEDED_WIN32(win32err)                           ((win32err) == 0)


//*****************************************************************************
// NT_STATUS specific error macros
//*****************************************************************************

#define FAILED_NTSTATUS(status)                             (((NTSTATUS)(status)) < 0)
#define SUCCEEDED_NTSTATUS(status)                          (((NTSTATUS)(status)) >= 0)


//*****************************************************************************
// Testing helpers - redefine to run unit tests against fail fast
//*****************************************************************************

#ifndef RESULT_NORETURN
#define RESULT_NORETURN                                     __declspec(noreturn)
#endif
#ifndef RESULT_NORETURN_NULL
#define RESULT_NORETURN_NULL                                _Ret_notnull_
#endif
#ifndef RESULT_NORETURN_RESULT
#define RESULT_NORETURN_RESULT(expr)                        (void)(expr);
#endif

//*****************************************************************************
// Helpers to setup the macros and functions used below... do not directly use.
//*****************************************************************************

/// @cond
#define __R_DIAGNOSTICS(diagnostics)                        diagnostics.returnAddress, diagnostics.line, diagnostics.file, nullptr, nullptr
#define __R_DIAGNOSTICS_RA(diagnostics, address)            diagnostics.returnAddress, diagnostics.line, diagnostics.file, nullptr, nullptr, address
#define __R_FN_PARAMS_FULL                                  _In_opt_ void* callerReturnAddress, unsigned int lineNumber, _In_opt_ PCSTR fileName, _In_opt_ PCSTR functionName, _In_opt_ PCSTR code, void* returnAddress
#define __R_FN_LOCALS_FULL_RA                               void* callerReturnAddress = nullptr; unsigned int lineNumber = 0; PCSTR fileName = nullptr; PCSTR functionName = nullptr; PCSTR code = nullptr; void* returnAddress = _ReturnAddress();
// NOTE: This BEGINs the common macro handling (__R_ prefix) for non-fail fast handled cases
//       This entire section will be repeated below for fail fast (__RFF_ prefix).
#define __R_COMMA ,
#define __R_FN_CALL_FULL                                    callerReturnAddress, lineNumber, fileName, functionName, code, returnAddress
#define __R_FN_CALL_FULL_RA                                 callerReturnAddress, lineNumber, fileName, functionName, code, _ReturnAddress()
// The following macros assemble the varying amount of data we want to collect from the macros, treating it uniformly
#if (RESULT_DIAGNOSTICS_LEVEL >= 2)  // line number
#define __R_IF_LINE(term) term
#define __R_IF_NOT_LINE(term)
#define __R_IF_COMMA ,
#define __R_LINE_VALUE static_cast<unsigned short>(__LINE__)
#else
#define __R_IF_LINE(term)
#define __R_IF_NOT_LINE(term) term
#define __R_IF_COMMA
#define __R_LINE_VALUE static_cast<unsigned short>(0)
#endif
#if (RESULT_DIAGNOSTICS_LEVEL >= 3) // line number + file name
#define __R_IF_FILE(term) term
#define __R_IF_NOT_FILE(term)
#define __R_FILE_VALUE __FILE__
#else
#define __R_IF_FILE(term)
#define __R_IF_NOT_FILE(term) term
#define __R_FILE_VALUE nullptr
#endif
#if (RESULT_DIAGNOSTICS_LEVEL >= 4) // line number + file name + function name
#define __R_IF_FUNCTION(term) term
#define __R_IF_NOT_FUNCTION(term)
#else
#define __R_IF_FUNCTION(term)
#define __R_IF_NOT_FUNCTION(term) term
#endif
#if (RESULT_DIAGNOSTICS_LEVEL >= 5) // line number + file name + function name + macro code
#define __R_IF_CODE(term) term
#define __R_IF_NOT_CODE(term)
#else
#define __R_IF_CODE(term)
#define __R_IF_NOT_CODE(term) term
#endif
#if (RESULT_INCLUDE_CALLER_RETURNADDRESS == 1)
#define __R_IF_CALLERADDRESS(term) term
#define __R_IF_NOT_CALLERADDRESS(term)
#define __R_CALLERADDRESS_VALUE _ReturnAddress()
#else
#define __R_IF_CALLERADDRESS(term)
#define __R_IF_NOT_CALLERADDRESS(term) term
#define __R_CALLERADDRESS_VALUE nullptr
#endif
#if (RESULT_INCLUDE_CALLER_RETURNADDRESS == 1) || (RESULT_DIAGNOSTICS_LEVEL >= 2)
#define __R_IF_TRAIL_COMMA ,
#else
#define __R_IF_TRAIL_COMMA
#endif
// Assemble the varying amounts of data into a single macro
#define __R_INFO_ONLY(CODE)                                 __R_IF_CALLERADDRESS(_ReturnAddress() __R_IF_COMMA) __R_IF_LINE(__R_LINE_VALUE) __R_IF_FILE(__R_COMMA __R_FILE_VALUE) __R_IF_FUNCTION(__R_COMMA __FUNCTION__) __R_IF_CODE(__R_COMMA CODE)
#define __R_INFO(CODE)                                      __R_INFO_ONLY(CODE) __R_IF_TRAIL_COMMA
#define __R_INFO_NOFILE_ONLY(CODE)                          __R_IF_CALLERADDRESS(_ReturnAddress() __R_IF_COMMA) __R_IF_LINE(__R_LINE_VALUE) __R_IF_FILE(__R_COMMA "wil") __R_IF_FUNCTION(__R_COMMA __FUNCTION__) __R_IF_CODE(__R_COMMA CODE)
#define __R_INFO_NOFILE(CODE)                               __R_INFO_NOFILE_ONLY(CODE) __R_IF_TRAIL_COMMA
#define __R_FN_PARAMS_ONLY                                  __R_IF_CALLERADDRESS(void* callerReturnAddress __R_IF_COMMA) __R_IF_LINE(unsigned int lineNumber) __R_IF_FILE(__R_COMMA _In_opt_ PCSTR fileName) __R_IF_FUNCTION(__R_COMMA _In_opt_ PCSTR functionName) __R_IF_CODE(__R_COMMA _In_opt_ PCSTR code)
#define __R_FN_PARAMS                                       __R_FN_PARAMS_ONLY __R_IF_TRAIL_COMMA
#define __R_FN_CALL_ONLY                                    __R_IF_CALLERADDRESS(callerReturnAddress __R_IF_COMMA) __R_IF_LINE(lineNumber) __R_IF_FILE(__R_COMMA fileName) __R_IF_FUNCTION(__R_COMMA functionName) __R_IF_CODE(__R_COMMA code)
#define __R_FN_CALL                                         __R_FN_CALL_ONLY __R_IF_TRAIL_COMMA
#define __R_FN_LOCALS                                       __R_IF_NOT_CALLERADDRESS(void* callerReturnAddress = nullptr;) __R_IF_NOT_LINE(unsigned int lineNumber = 0;) __R_IF_NOT_FILE(PCSTR fileName = nullptr;) __R_IF_NOT_FUNCTION(PCSTR functionName = nullptr;) __R_IF_NOT_CODE(PCSTR code = nullptr;)
#define __R_FN_LOCALS_RA                                    __R_IF_NOT_CALLERADDRESS(void* callerReturnAddress = nullptr;) __R_IF_NOT_LINE(unsigned int lineNumber = 0;) __R_IF_NOT_FILE(PCSTR fileName = nullptr;) __R_IF_NOT_FUNCTION(PCSTR functionName = nullptr;) __R_IF_NOT_CODE(PCSTR code = nullptr;) void* returnAddress = _ReturnAddress();
#define __R_FN_UNREFERENCED                                 __R_IF_CALLERADDRESS((void)callerReturnAddress;) __R_IF_LINE((void)lineNumber;) __R_IF_FILE((void)fileName;) __R_IF_FUNCTION((void)functionName;) __R_IF_CODE((void)code;)
// 1) Direct Methods
//      * Called Directly by Macros
//      * Always noinline
//      * May be template-driven to create unique call sites if (RESULT_DIAGNOSTICS_LEVEL == 1)
#if (RESULT_DIAGNOSTICS_LEVEL == 1)
#define __R_DIRECT_METHOD(RetType, MethodName)              template <unsigned int optimizerCounter> inline __declspec(noinline) RetType MethodName
#define __R_DIRECT_NORET_METHOD(RetType, MethodName)        template <unsigned int optimizerCounter> inline __declspec(noinline) RESULT_NORETURN RetType MethodName
#else
#define __R_DIRECT_METHOD(RetType, MethodName)              inline __declspec(noinline) RetType MethodName
#define __R_DIRECT_NORET_METHOD(RetType, MethodName)        inline __declspec(noinline) RESULT_NORETURN RetType MethodName
#endif
#define __R_DIRECT_FN_PARAMS                                __R_FN_PARAMS
#define __R_DIRECT_FN_PARAMS_ONLY                           __R_FN_PARAMS_ONLY
#define __R_DIRECT_FN_CALL                                  __R_FN_CALL_FULL_RA __R_COMMA
#define __R_DIRECT_FN_CALL_ONLY                             __R_FN_CALL_FULL_RA
// 2) Internal Methods
//      * Only called by Conditional routines
//      * 'inline' when (RESULT_INLINE_ERROR_TESTS = 0 and RESULT_DIAGNOSTICS_LEVEL != 1), otherwise noinline (directly called by code when branching is forceinlined)
//      * May be template-driven to create unique call sites if (RESULT_DIAGNOSTICS_LEVEL == 1 and RESULT_INLINE_ERROR_TESTS = 1)
#if (RESULT_DIAGNOSTICS_LEVEL == 1)
#define __R_INTERNAL_NOINLINE_METHOD(MethodName)            inline __declspec(noinline) void MethodName
#define __R_INTERNAL_NOINLINE_NORET_METHOD(MethodName)      inline __declspec(noinline) RESULT_NORETURN void MethodName
#define __R_INTERNAL_INLINE_METHOD(MethodName)              template <unsigned int optimizerCounter> inline __declspec(noinline) void MethodName
#define __R_INTERNAL_INLINE_NORET_METHOD(MethodName)        template <unsigned int optimizerCounter> inline __declspec(noinline) RESULT_NORETURN void MethodName
#define __R_CALL_INTERNAL_INLINE_METHOD(MethodName)         MethodName <optimizerCounter>
#else
#define __R_INTERNAL_NOINLINE_METHOD(MethodName)            inline void MethodName
#define __R_INTERNAL_NOINLINE_NORET_METHOD(MethodName)      inline RESULT_NORETURN void MethodName
#define __R_INTERNAL_INLINE_METHOD(MethodName)              inline __declspec(noinline) void MethodName
#define __R_INTERNAL_INLINE_NORET_METHOD(MethodName)        inline __declspec(noinline) RESULT_NORETURN void MethodName
#define __R_CALL_INTERNAL_INLINE_METHOD(MethodName)         MethodName
#endif
#define __R_CALL_INTERNAL_NOINLINE_METHOD(MethodName)       MethodName
#define __R_INTERNAL_NOINLINE_FN_PARAMS                     __R_FN_PARAMS void* returnAddress __R_COMMA
#define __R_INTERNAL_NOINLINE_FN_PARAMS_ONLY                __R_FN_PARAMS void* returnAddress
#define __R_INTERNAL_NOINLINE_FN_CALL                       __R_FN_CALL_FULL __R_COMMA
#define __R_INTERNAL_NOINLINE_FN_CALL_ONLY                  __R_FN_CALL_FULL
#define __R_INTERNAL_INLINE_FN_PARAMS                       __R_FN_PARAMS
#define __R_INTERNAL_INLINE_FN_PARAMS_ONLY                  __R_FN_PARAMS_ONLY
#define __R_INTERNAL_INLINE_FN_CALL                         __R_FN_CALL_FULL_RA __R_COMMA
#define __R_INTERNAL_INLINE_FN_CALL_ONLY                    __R_FN_CALL_FULL_RA
#if (RESULT_INLINE_ERROR_TESTS == 0)
#define __R_INTERNAL_METHOD                                 __R_INTERNAL_NOINLINE_METHOD
#define __R_INTERNAL_NORET_METHOD                           __R_INTERNAL_NOINLINE_NORET_METHOD
#define __R_CALL_INTERNAL_METHOD                            __R_CALL_INTERNAL_NOINLINE_METHOD
#define __R_INTERNAL_FN_PARAMS                              __R_INTERNAL_NOINLINE_FN_PARAMS
#define __R_INTERNAL_FN_PARAMS_ONLY                         __R_INTERNAL_NOINLINE_FN_PARAMS_ONLY
#define __R_INTERNAL_FN_CALL                                __R_INTERNAL_NOINLINE_FN_CALL
#define __R_INTERNAL_FN_CALL_ONLY                           __R_INTERNAL_NOINLINE_FN_CALL_ONLY
#else
#define __R_INTERNAL_METHOD                                 __R_INTERNAL_INLINE_METHOD
#define __R_INTERNAL_NORET_METHOD                           __R_INTERNAL_INLINE_NORET_METHOD
#define __R_CALL_INTERNAL_METHOD                            __R_CALL_INTERNAL_INLINE_METHOD
#define __R_INTERNAL_FN_PARAMS                              __R_INTERNAL_INLINE_FN_PARAMS
#define __R_INTERNAL_FN_PARAMS_ONLY                         __R_INTERNAL_INLINE_FN_PARAMS_ONLY
#define __R_INTERNAL_FN_CALL                                __R_INTERNAL_INLINE_FN_CALL
#define __R_INTERNAL_FN_CALL_ONLY                           __R_INTERNAL_INLINE_FN_CALL_ONLY
#endif
// 3) Conditional Methods
//      * Called Directly by Macros
//      * May be noinline or __forceinline depending upon (RESULT_INLINE_ERROR_TESTS)
//      * May be template-driven to create unique call sites if (RESULT_DIAGNOSTICS_LEVEL == 1)
#if (RESULT_DIAGNOSTICS_LEVEL == 1)
#define __R_CONDITIONAL_NOINLINE_METHOD(RetType, MethodName)            template <unsigned int optimizerCounter> inline __declspec(noinline) RetType MethodName
#define __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(RetType, MethodName)   inline __declspec(noinline) RetType MethodName
#define __R_CONDITIONAL_INLINE_METHOD(RetType, MethodName)              template <unsigned int optimizerCounter> __forceinline RetType MethodName
#define __R_CONDITIONAL_INLINE_TEMPLATE_METHOD(RetType, MethodName)     __forceinline RetType MethodName
#define __R_CONDITIONAL_PARTIAL_TEMPLATE                                unsigned int optimizerCounter __R_COMMA
#else
#define __R_CONDITIONAL_NOINLINE_METHOD(RetType, MethodName)            inline __declspec(noinline) RetType MethodName
#define __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(RetType, MethodName)   inline __declspec(noinline) RetType MethodName
#define __R_CONDITIONAL_INLINE_METHOD(RetType, MethodName)              __forceinline RetType MethodName
#define __R_CONDITIONAL_INLINE_TEMPLATE_METHOD(RetType, MethodName)     __forceinline RetType MethodName
#define __R_CONDITIONAL_PARTIAL_TEMPLATE
#endif
#define __R_CONDITIONAL_NOINLINE_FN_CALL                    __R_FN_CALL _ReturnAddress() __R_COMMA
#define __R_CONDITIONAL_NOINLINE_FN_CALL_ONLY               __R_FN_CALL _ReturnAddress()
#define __R_CONDITIONAL_INLINE_FN_CALL                      __R_FN_CALL
#define __R_CONDITIONAL_INLINE_FN_CALL_ONLY                 __R_FN_CALL_ONLY
#if (RESULT_INLINE_ERROR_TESTS == 0)
#define __R_CONDITIONAL_METHOD                              __R_CONDITIONAL_NOINLINE_METHOD
#define __R_CONDITIONAL_TEMPLATE_METHOD                     __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD
#define __R_CONDITIONAL_FN_CALL                             __R_CONDITIONAL_NOINLINE_FN_CALL
#define __R_CONDITIONAL_FN_CALL_ONLY                        __R_CONDITIONAL_NOINLINE_FN_CALL_ONLY
#else
#define __R_CONDITIONAL_METHOD                              __R_CONDITIONAL_INLINE_METHOD
#define __R_CONDITIONAL_TEMPLATE_METHOD                     __R_CONDITIONAL_INLINE_TEMPLATE_METHOD
#define __R_CONDITIONAL_FN_CALL                             __R_CONDITIONAL_INLINE_FN_CALL
#define __R_CONDITIONAL_FN_CALL_ONLY                        __R_CONDITIONAL_INLINE_FN_CALL_ONLY
#endif
#define __R_CONDITIONAL_FN_PARAMS                           __R_FN_PARAMS
#define __R_CONDITIONAL_FN_PARAMS_ONLY                      __R_FN_PARAMS_ONLY
// Macro call-site helpers
#define __R_NS_ASSEMBLE2(ri, rd)                            in##ri##diag##rd                // Differing internal namespaces eliminate ODR violations between modes
#define __R_NS_ASSEMBLE(ri, rd)                             __R_NS_ASSEMBLE2(ri, rd)
#define __R_NS_NAME                                         __R_NS_ASSEMBLE(RESULT_INLINE_ERROR_TESTS, RESULT_DIAGNOSTICS_LEVEL)
#define __R_NS wil::details::__R_NS_NAME
#if (RESULT_DIAGNOSTICS_LEVEL == 1)
#define __R_FN(MethodName)                                  __R_NS:: MethodName <__COUNTER__>
#else
#define __R_FN(MethodName)                                  __R_NS:: MethodName
#endif
// NOTE: This ENDs the common macro handling (__R_ prefix) for non-fail fast handled cases
//       This entire section is repeated below for fail fast (__RFF_ prefix).  For ease of editing this section, the
//       process is to copy/paste, and search and replace (__R_ -> __RFF_), (RESULT_DIAGNOSTICS_LEVEL -> RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST),
//       (RESULT_INLINE_ERROR_TESTS -> RESULT_INLINE_ERROR_TESTS_FAIL_FAST) and (RESULT_INCLUDE_CALLER_RETURNADDRESS -> RESULT_INCLUDE_CALLER_RETURNADDRESS_FAIL_FAST)
#define __RFF_COMMA ,
#define __RFF_FN_CALL_FULL                                    callerReturnAddress, lineNumber, fileName, functionName, code, returnAddress
#define __RFF_FN_CALL_FULL_RA                                 callerReturnAddress, lineNumber, fileName, functionName, code, _ReturnAddress()
// The following macros assemble the varying amount of data we want to collect from the macros, treating it uniformly
#if (RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST >= 2)  // line number
#define __RFF_IF_LINE(term) term
#define __RFF_IF_NOT_LINE(term)
#define __RFF_IF_COMMA ,
#else
#define __RFF_IF_LINE(term)
#define __RFF_IF_NOT_LINE(term) term
#define __RFF_IF_COMMA
#endif
#if (RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST >= 3) // line number + file name
#define __RFF_IF_FILE(term) term
#define __RFF_IF_NOT_FILE(term)
#else
#define __RFF_IF_FILE(term)
#define __RFF_IF_NOT_FILE(term) term
#endif
#if (RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST >= 4) // line number + file name + function name
#define __RFF_IF_FUNCTION(term) term
#define __RFF_IF_NOT_FUNCTION(term)
#else
#define __RFF_IF_FUNCTION(term)
#define __RFF_IF_NOT_FUNCTION(term) term
#endif
#if (RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST >= 5) // line number + file name + function name + macro code
#define __RFF_IF_CODE(term) term
#define __RFF_IF_NOT_CODE(term)
#else
#define __RFF_IF_CODE(term)
#define __RFF_IF_NOT_CODE(term) term
#endif
#if (RESULT_INCLUDE_CALLER_RETURNADDRESS_FAIL_FAST == 1)
#define __RFF_IF_CALLERADDRESS(term) term
#define __RFF_IF_NOT_CALLERADDRESS(term)
#else
#define __RFF_IF_CALLERADDRESS(term)
#define __RFF_IF_NOT_CALLERADDRESS(term) term
#endif
#if (RESULT_INCLUDE_CALLER_RETURNADDRESS_FAIL_FAST == 1) || (RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST >= 2)
#define __RFF_IF_TRAIL_COMMA ,
#else
#define __RFF_IF_TRAIL_COMMA
#endif
// Assemble the varying amounts of data into a single macro
#define __RFF_INFO_ONLY(CODE)                                 __RFF_IF_CALLERADDRESS(_ReturnAddress() __RFF_IF_COMMA) __RFF_IF_LINE(__R_LINE_VALUE) __RFF_IF_FILE(__RFF_COMMA __R_FILE_VALUE) __RFF_IF_FUNCTION(__RFF_COMMA __FUNCTION__) __RFF_IF_CODE(__RFF_COMMA CODE)
#define __RFF_INFO(CODE)                                      __RFF_INFO_ONLY(CODE) __RFF_IF_TRAIL_COMMA
#define __RFF_INFO_NOFILE_ONLY(CODE)                          __RFF_IF_CALLERADDRESS(_ReturnAddress() __RFF_IF_COMMA) __RFF_IF_LINE(__R_LINE_VALUE) __RFF_IF_FILE(__RFF_COMMA "wil") __RFF_IF_FUNCTION(__RFF_COMMA __FUNCTION__) __RFF_IF_CODE(__RFF_COMMA CODE)
#define __RFF_INFO_NOFILE(CODE)                               __RFF_INFO_NOFILE_ONLY(CODE) __RFF_IF_TRAIL_COMMA
#define __RFF_FN_PARAMS_ONLY                                  __RFF_IF_CALLERADDRESS(void* callerReturnAddress __RFF_IF_COMMA) __RFF_IF_LINE(unsigned int lineNumber) __RFF_IF_FILE(__RFF_COMMA _In_opt_ PCSTR fileName) __RFF_IF_FUNCTION(__RFF_COMMA _In_opt_ PCSTR functionName) __RFF_IF_CODE(__RFF_COMMA _In_opt_ PCSTR code)
#define __RFF_FN_PARAMS                                       __RFF_FN_PARAMS_ONLY __RFF_IF_TRAIL_COMMA
#define __RFF_FN_CALL_ONLY                                    __RFF_IF_CALLERADDRESS(callerReturnAddress __RFF_IF_COMMA) __RFF_IF_LINE(lineNumber) __RFF_IF_FILE(__RFF_COMMA fileName) __RFF_IF_FUNCTION(__RFF_COMMA functionName) __RFF_IF_CODE(__RFF_COMMA code)
#define __RFF_FN_CALL                                         __RFF_FN_CALL_ONLY __RFF_IF_TRAIL_COMMA
#define __RFF_FN_LOCALS                                       __RFF_IF_NOT_CALLERADDRESS(void* callerReturnAddress = nullptr;) __RFF_IF_NOT_LINE(unsigned int lineNumber = 0;) __RFF_IF_NOT_FILE(PCSTR fileName = nullptr;) __RFF_IF_NOT_FUNCTION(PCSTR functionName = nullptr;) __RFF_IF_NOT_CODE(PCSTR code = nullptr;)
#define __RFF_FN_UNREFERENCED                                 __RFF_IF_CALLERADDRESS(callerReturnAddress;) __RFF_IF_LINE(lineNumber;) __RFF_IF_FILE(fileName;) __RFF_IF_FUNCTION(functionName;) __RFF_IF_CODE(code;)
// 1) Direct Methods
//      * Called Directly by Macros
//      * Always noinline
//      * May be template-driven to create unique call sites if (RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST == 1)
#if (RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST == 1)
#define __RFF_DIRECT_METHOD(RetType, MethodName)              template <unsigned int optimizerCounter> inline __declspec(noinline) RetType MethodName
#define __RFF_DIRECT_NORET_METHOD(RetType, MethodName)        template <unsigned int optimizerCounter> inline __declspec(noinline) RESULT_NORETURN RetType MethodName
#else
#define __RFF_DIRECT_METHOD(RetType, MethodName)              inline __declspec(noinline) RetType MethodName
#define __RFF_DIRECT_NORET_METHOD(RetType, MethodName)        inline __declspec(noinline) RESULT_NORETURN RetType MethodName
#endif
#define __RFF_DIRECT_FN_PARAMS                                __RFF_FN_PARAMS
#define __RFF_DIRECT_FN_PARAMS_ONLY                           __RFF_FN_PARAMS_ONLY
#define __RFF_DIRECT_FN_CALL                                  __RFF_FN_CALL_FULL_RA __RFF_COMMA
#define __RFF_DIRECT_FN_CALL_ONLY                             __RFF_FN_CALL_FULL_RA
// 2) Internal Methods
//      * Only called by Conditional routines
//      * 'inline' when (RESULT_INLINE_ERROR_TESTS_FAIL_FAST = 0 and RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST != 1), otherwise noinline (directly called by code when branching is forceinlined)
//      * May be template-driven to create unique call sites if (RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST == 1 and RESULT_INLINE_ERROR_TESTS_FAIL_FAST = 1)
#if (RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST == 1)
#define __RFF_INTERNAL_NOINLINE_METHOD(MethodName)            inline __declspec(noinline) void MethodName
#define __RFF_INTERNAL_NOINLINE_NORET_METHOD(MethodName)      inline __declspec(noinline) RESULT_NORETURN void MethodName
#define __RFF_INTERNAL_INLINE_METHOD(MethodName)              template <unsigned int optimizerCounter> inline __declspec(noinline) void MethodName
#define __RFF_INTERNAL_INLINE_NORET_METHOD(MethodName)        template <unsigned int optimizerCounter> inline __declspec(noinline) RESULT_NORETURN void MethodName
#define __RFF_CALL_INTERNAL_INLINE_METHOD(MethodName)         MethodName <optimizerCounter>
#else
#define __RFF_INTERNAL_NOINLINE_METHOD(MethodName)            inline void MethodName
#define __RFF_INTERNAL_NOINLINE_NORET_METHOD(MethodName)      inline RESULT_NORETURN void MethodName
#define __RFF_INTERNAL_INLINE_METHOD(MethodName)              inline __declspec(noinline) void MethodName
#define __RFF_INTERNAL_INLINE_NORET_METHOD(MethodName)        inline __declspec(noinline) RESULT_NORETURN void MethodName
#define __RFF_CALL_INTERNAL_INLINE_METHOD(MethodName)         MethodName
#endif
#define __RFF_CALL_INTERNAL_NOINLINE_METHOD(MethodName)       MethodName
#define __RFF_INTERNAL_NOINLINE_FN_PARAMS                     __RFF_FN_PARAMS void* returnAddress __RFF_COMMA
#define __RFF_INTERNAL_NOINLINE_FN_PARAMS_ONLY                __RFF_FN_PARAMS void* returnAddress
#define __RFF_INTERNAL_NOINLINE_FN_CALL                       __RFF_FN_CALL_FULL __RFF_COMMA
#define __RFF_INTERNAL_NOINLINE_FN_CALL_ONLY                  __RFF_FN_CALL_FULL
#define __RFF_INTERNAL_INLINE_FN_PARAMS                       __RFF_FN_PARAMS
#define __RFF_INTERNAL_INLINE_FN_PARAMS_ONLY                  __RFF_FN_PARAMS_ONLY
#define __RFF_INTERNAL_INLINE_FN_CALL                         __RFF_FN_CALL_FULL_RA __RFF_COMMA
#define __RFF_INTERNAL_INLINE_FN_CALL_ONLY                    __RFF_FN_CALL_FULL_RA
#if (RESULT_INLINE_ERROR_TESTS_FAIL_FAST == 0)
#define __RFF_INTERNAL_METHOD                                 __RFF_INTERNAL_NOINLINE_METHOD
#define __RFF_INTERNAL_NORET_METHOD                           __RFF_INTERNAL_NOINLINE_NORET_METHOD
#define __RFF_CALL_INTERNAL_METHOD                            __RFF_CALL_INTERNAL_NOINLINE_METHOD
#define __RFF_INTERNAL_FN_PARAMS                              __RFF_INTERNAL_NOINLINE_FN_PARAMS
#define __RFF_INTERNAL_FN_PARAMS_ONLY                         __RFF_INTERNAL_NOINLINE_FN_PARAMS_ONLY
#define __RFF_INTERNAL_FN_CALL                                __RFF_INTERNAL_NOINLINE_FN_CALL
#define __RFF_INTERNAL_FN_CALL_ONLY                           __RFF_INTERNAL_NOINLINE_FN_CALL_ONLY
#else
#define __RFF_INTERNAL_METHOD                                 __RFF_INTERNAL_INLINE_METHOD
#define __RFF_INTERNAL_NORET_METHOD                           __RFF_INTERNAL_INLINE_NORET_METHOD
#define __RFF_CALL_INTERNAL_METHOD                            __RFF_CALL_INTERNAL_INLINE_METHOD
#define __RFF_INTERNAL_FN_PARAMS                              __RFF_INTERNAL_INLINE_FN_PARAMS
#define __RFF_INTERNAL_FN_PARAMS_ONLY                         __RFF_INTERNAL_INLINE_FN_PARAMS_ONLY
#define __RFF_INTERNAL_FN_CALL                                __RFF_INTERNAL_INLINE_FN_CALL
#define __RFF_INTERNAL_FN_CALL_ONLY                           __RFF_INTERNAL_INLINE_FN_CALL_ONLY
#endif
// 3) Conditional Methods
//      * Called Directly by Macros
//      * May be noinline or __forceinline depending upon (RESULT_INLINE_ERROR_TESTS_FAIL_FAST)
//      * May be template-driven to create unique call sites if (RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST == 1)
#if (RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST == 1)
#define __RFF_CONDITIONAL_NOINLINE_METHOD(RetType, MethodName)            template <unsigned int optimizerCounter> inline __declspec(noinline) RetType MethodName
#define __RFF_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(RetType, MethodName)   inline __declspec(noinline) RetType MethodName
#define __RFF_CONDITIONAL_INLINE_METHOD(RetType, MethodName)              template <unsigned int optimizerCounter> __forceinline RetType MethodName
#define __RFF_CONDITIONAL_INLINE_TEMPLATE_METHOD(RetType, MethodName)     __forceinline RetType MethodName
#define __RFF_CONDITIONAL_PARTIAL_TEMPLATE                                unsigned int optimizerCounter __RFF_COMMA
#else
#define __RFF_CONDITIONAL_NOINLINE_METHOD(RetType, MethodName)            inline __declspec(noinline) RetType MethodName
#define __RFF_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(RetType, MethodName)   inline __declspec(noinline) RetType MethodName
#define __RFF_CONDITIONAL_INLINE_METHOD(RetType, MethodName)              __forceinline RetType MethodName
#define __RFF_CONDITIONAL_INLINE_TEMPLATE_METHOD(RetType, MethodName)     __forceinline RetType MethodName
#define __RFF_CONDITIONAL_PARTIAL_TEMPLATE
#endif
#define __RFF_CONDITIONAL_NOINLINE_FN_CALL                    __RFF_FN_CALL _ReturnAddress() __RFF_COMMA
#define __RFF_CONDITIONAL_NOINLINE_FN_CALL_ONLY               __RFF_FN_CALL _ReturnAddress()
#define __RFF_CONDITIONAL_INLINE_FN_CALL                      __RFF_FN_CALL
#define __RFF_CONDITIONAL_INLINE_FN_CALL_ONLY                 __RFF_FN_CALL_ONLY
#if (RESULT_INLINE_ERROR_TESTS_FAIL_FAST == 0)
#define __RFF_CONDITIONAL_METHOD                              __RFF_CONDITIONAL_NOINLINE_METHOD
#define __RFF_CONDITIONAL_TEMPLATE_METHOD                     __RFF_CONDITIONAL_NOINLINE_TEMPLATE_METHOD
#define __RFF_CONDITIONAL_FN_CALL                             __RFF_CONDITIONAL_NOINLINE_FN_CALL
#define __RFF_CONDITIONAL_FN_CALL_ONLY                        __RFF_CONDITIONAL_NOINLINE_FN_CALL_ONLY
#else
#define __RFF_CONDITIONAL_METHOD                              __RFF_CONDITIONAL_INLINE_METHOD
#define __RFF_CONDITIONAL_TEMPLATE_METHOD                     __RFF_CONDITIONAL_INLINE_TEMPLATE_METHOD
#define __RFF_CONDITIONAL_FN_CALL                             __RFF_CONDITIONAL_INLINE_FN_CALL
#define __RFF_CONDITIONAL_FN_CALL_ONLY                        __RFF_CONDITIONAL_INLINE_FN_CALL_ONLY
#endif
#define __RFF_CONDITIONAL_FN_PARAMS                           __RFF_FN_PARAMS
#define __RFF_CONDITIONAL_FN_PARAMS_ONLY                      __RFF_FN_PARAMS_ONLY
// Macro call-site helpers
#define __RFF_NS_ASSEMBLE2(ri, rd)                            in##ri##diag##rd                // Differing internal namespaces eliminate ODR violations between modes
#define __RFF_NS_ASSEMBLE(ri, rd)                             __RFF_NS_ASSEMBLE2(ri, rd)
#define __RFF_NS_NAME                                         __RFF_NS_ASSEMBLE(RESULT_INLINE_ERROR_TESTS_FAIL_FAST, RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST)
#define __RFF_NS wil::details::__RFF_NS_NAME
#if (RESULT_DIAGNOSTICS_LEVEL_FAIL_FAST == 1)
#define __RFF_FN(MethodName)                                  __RFF_NS:: MethodName <__COUNTER__>
#else
#define __RFF_FN(MethodName)                                  __RFF_NS:: MethodName
#endif
// end-of-repeated fail-fast handling macros

// Helpers for return macros
#define __RETURN_HR_MSG(hr, str, fmt, ...)                   __WI_SUPPRESS_4127_S do { const HRESULT __hr = (hr); if (FAILED(__hr)) { __R_FN(Return_HrMsg)(__R_INFO(str) __hr, fmt, ##__VA_ARGS__); } return __hr; } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __RETURN_HR_MSG_FAIL(hr, str, fmt, ...)              __WI_SUPPRESS_4127_S do { const HRESULT __hr = (hr); __R_FN(Return_HrMsg)(__R_INFO(str) __hr, fmt, ##__VA_ARGS__); return __hr; } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __RETURN_WIN32_MSG(err, str, fmt, ...)               __WI_SUPPRESS_4127_S do { const DWORD __err = (err); if (FAILED_WIN32(__err)) { return __R_FN(Return_Win32Msg)(__R_INFO(str) __err, fmt, ##__VA_ARGS__); } return S_OK; } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __RETURN_WIN32_MSG_FAIL(err, str, fmt, ...)          __WI_SUPPRESS_4127_S do { const DWORD __err = (err); return __R_FN(Return_Win32Msg)(__R_INFO(str) __err, fmt, ##__VA_ARGS__); } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __RETURN_GLE_MSG_FAIL(str, fmt, ...)                 return __R_FN(Return_GetLastErrorMsg)(__R_INFO(str) fmt, ##__VA_ARGS__)
#define __RETURN_NTSTATUS_MSG(status, str, fmt, ...)         __WI_SUPPRESS_4127_S do { const NTSTATUS __status = (status); if  (FAILED_NTSTATUS(__status)) { return __R_FN(Return_NtStatusMsg)(__R_INFO(str) __status, fmt, ##__VA_ARGS__); } return S_OK; } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __RETURN_NTSTATUS_MSG_FAIL(status, str, fmt, ...)    __WI_SUPPRESS_4127_S do { const NTSTATUS __status = (status); return __R_FN(Return_NtStatusMsg)(__R_INFO(str) __status, fmt, ##__VA_ARGS__); } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __RETURN_HR(hr, str)                                 __WI_SUPPRESS_4127_S do { const HRESULT __hr = (hr); if (FAILED(__hr)) { __R_FN(Return_Hr)(__R_INFO(str) __hr); } return __hr; } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __RETURN_HR_NOFILE(hr, str)                          __WI_SUPPRESS_4127_S do { const HRESULT __hr = (hr); if (FAILED(__hr)) { __R_FN(Return_Hr)(__R_INFO_NOFILE(str) __hr); } return __hr; } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __RETURN_HR_FAIL(hr, str)                            __WI_SUPPRESS_4127_S do { const HRESULT __hr = (hr); __R_FN(Return_Hr)(__R_INFO(str) __hr); return __hr; } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __RETURN_HR_FAIL_NOFILE(hr, str)                     __WI_SUPPRESS_4127_S do { const HRESULT __hr = (hr); __R_FN(Return_Hr)(__R_INFO_NOFILE(str) __hr); return __hr; } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __RETURN_WIN32(err, str)                             __WI_SUPPRESS_4127_S do { const DWORD __err = (err); if (FAILED_WIN32(__err)) { return __R_FN(Return_Win32)(__R_INFO(str) __err); } return S_OK; } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __RETURN_WIN32_FAIL(err, str)                        __WI_SUPPRESS_4127_S do { const DWORD __err = (err); return __R_FN(Return_Win32)(__R_INFO(str) __err); } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __RETURN_GLE_FAIL(str)                               return __R_FN(Return_GetLastError)(__R_INFO_ONLY(str))
#define __RETURN_GLE_FAIL_NOFILE(str)                        return __R_FN(Return_GetLastError)(__R_INFO_NOFILE_ONLY(str))
#define __RETURN_NTSTATUS(status, str)                       __WI_SUPPRESS_4127_S do { const NTSTATUS __status = (status); if (FAILED_NTSTATUS(__status)) { return __R_FN(Return_NtStatus)(__R_INFO(str) __status); } return S_OK; } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __RETURN_NTSTATUS_FAIL(status, str)                  __WI_SUPPRESS_4127_S do { const NTSTATUS __status = (status); return __R_FN(Return_NtStatus)(__R_INFO(str) __status); } __WI_SUPPRESS_4127_E while ((void)0, 0)
/// @endcond

//*****************************************************************************
// Macros for returning failures as HRESULTs
//*****************************************************************************

// Always returns a known result (HRESULT) - always logs failures
#define RETURN_HR(hr)                                           __RETURN_HR(wil::verify_hresult(hr), #hr)
#define RETURN_LAST_ERROR()                                     __RETURN_GLE_FAIL(nullptr)
#define RETURN_WIN32(win32err)                                  __RETURN_WIN32(win32err, #win32err)
#define RETURN_NTSTATUS(status)                                 __RETURN_NTSTATUS(status, #status)

// Conditionally returns failures (HRESULT) - always logs failures
#define RETURN_IF_FAILED(hr)                                    __WI_SUPPRESS_4127_S do { const auto __hrRet = wil::verify_hresult(hr); if (FAILED(__hrRet)) { __RETURN_HR_FAIL(__hrRet, #hr); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define RETURN_IF_WIN32_BOOL_FALSE(win32BOOL)                   __WI_SUPPRESS_4127_S do { const auto __boolRet = wil::verify_BOOL(win32BOOL); if (!__boolRet) { __RETURN_GLE_FAIL(#win32BOOL); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define RETURN_IF_WIN32_ERROR(win32err)                         __WI_SUPPRESS_4127_S do { const DWORD __errRet = (win32err); if (FAILED_WIN32(__errRet)) { __RETURN_WIN32_FAIL(__errRet, #win32err); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define RETURN_IF_NULL_ALLOC(ptr)                               __WI_SUPPRESS_4127_S do { if ((ptr) == nullptr) { __RETURN_HR_FAIL(E_OUTOFMEMORY, #ptr); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define RETURN_HR_IF(hr, condition)                             __WI_SUPPRESS_4127_S do { if (wil::verify_bool(condition)) { __RETURN_HR(wil::verify_hresult(hr), #condition); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define RETURN_HR_IF_NULL(hr, ptr)                              __WI_SUPPRESS_4127_S do { if ((ptr) == nullptr) { __RETURN_HR(wil::verify_hresult(hr), #ptr); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define RETURN_LAST_ERROR_IF(condition)                         __WI_SUPPRESS_4127_S do { if (wil::verify_bool(condition)) { __RETURN_GLE_FAIL(#condition); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define RETURN_LAST_ERROR_IF_NULL(ptr)                          __WI_SUPPRESS_4127_S do { if ((ptr) == nullptr) { __RETURN_GLE_FAIL(#ptr); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define RETURN_IF_NTSTATUS_FAILED(status)                       __WI_SUPPRESS_4127_S do { const NTSTATUS __statusRet = (status); if (FAILED_NTSTATUS(__statusRet)) { __RETURN_NTSTATUS_FAIL(__statusRet, #status); }} __WI_SUPPRESS_4127_E while ((void)0, 0)

// Always returns a known failure (HRESULT) - always logs a var-arg message on failure
#define RETURN_HR_MSG(hr, fmt, ...)                             __RETURN_HR_MSG(wil::verify_hresult(hr), #hr, fmt, ##__VA_ARGS__)
#define RETURN_LAST_ERROR_MSG(fmt, ...)                         __RETURN_GLE_MSG_FAIL(nullptr, fmt, ##__VA_ARGS__)
#define RETURN_WIN32_MSG(win32err, fmt, ...)                    __RETURN_WIN32_MSG(win32err, #win32err, fmt, ##__VA_ARGS__)
#define RETURN_NTSTATUS_MSG(status, fmt, ...)                   __RETURN_NTSTATUS_MSG(status, #status, fmt, ##__VA_ARGS__)

// Conditionally returns failures (HRESULT) - always logs a var-arg message on failure
#define RETURN_IF_FAILED_MSG(hr, fmt, ...)                      __WI_SUPPRESS_4127_S do { const auto __hrRet = wil::verify_hresult(hr); if (FAILED(__hrRet)) { __RETURN_HR_MSG_FAIL(__hrRet, #hr, fmt, ##__VA_ARGS__); }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_IF_WIN32_BOOL_FALSE_MSG(win32BOOL, fmt, ...)     __WI_SUPPRESS_4127_S do { if (!wil::verify_BOOL(win32BOOL)) { __RETURN_GLE_MSG_FAIL(#win32BOOL, fmt, ##__VA_ARGS__); }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_IF_WIN32_ERROR_MSG(win32err, fmt, ...)           __WI_SUPPRESS_4127_S do { const DWORD __errRet = (win32err); if (FAILED_WIN32(__errRet)) { __RETURN_WIN32_MSG_FAIL(__errRet, #win32err, fmt, ##__VA_ARGS__); }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_IF_NULL_ALLOC_MSG(ptr, fmt, ...)                 __WI_SUPPRESS_4127_S do { if ((ptr) == nullptr) { __RETURN_HR_MSG_FAIL(E_OUTOFMEMORY, #ptr, fmt, ##__VA_ARGS__); }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_HR_IF_MSG(hr, condition, fmt, ...)               __WI_SUPPRESS_4127_S do { if (wil::verify_bool(condition)) { __RETURN_HR_MSG(wil::verify_hresult(hr), #condition, fmt, ##__VA_ARGS__); }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_HR_IF_NULL_MSG(hr, ptr, fmt, ...)                __WI_SUPPRESS_4127_S do { if ((ptr) == nullptr) { __RETURN_HR_MSG(wil::verify_hresult(hr), #ptr, fmt, ##__VA_ARGS__); }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_LAST_ERROR_IF_MSG(condition, fmt, ...)           __WI_SUPPRESS_4127_S do { if (wil::verify_bool(condition)) { __RETURN_GLE_MSG_FAIL(#condition, fmt, ##__VA_ARGS__); }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_LAST_ERROR_IF_NULL_MSG(ptr, fmt, ...)            __WI_SUPPRESS_4127_S do { if ((ptr) == nullptr) { __RETURN_GLE_MSG_FAIL(#ptr, fmt, ##__VA_ARGS__); }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_IF_NTSTATUS_FAILED_MSG(status, fmt, ...)         __WI_SUPPRESS_4127_S do { const NTSTATUS __statusRet = (status); if (FAILED_NTSTATUS(__statusRet)) { __RETURN_NTSTATUS_MSG_FAIL(__statusRet, #status, fmt, ##__VA_ARGS__); }} __WI_SUPPRESS_4127_E while((void)0, 0)

// Conditionally returns failures (HRESULT) - use for failures that are expected in common use - failures are not logged - macros are only for control flow pattern
#define RETURN_IF_FAILED_EXPECTED(hr)                           __WI_SUPPRESS_4127_S do { const auto __hrRet = wil::verify_hresult(hr); if (FAILED(__hrRet)) { return __hrRet; }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define RETURN_IF_WIN32_BOOL_FALSE_EXPECTED(win32BOOL)          __WI_SUPPRESS_4127_S do { if (!wil::verify_BOOL(win32BOOL)) { return wil::details::GetLastErrorFailHr(); }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_IF_WIN32_ERROR_EXPECTED(win32err)                __WI_SUPPRESS_4127_S do { const DWORD __errRet = (win32err); if (FAILED_WIN32(__errRet)) { return __HRESULT_FROM_WIN32(__errRet); }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_IF_NULL_ALLOC_EXPECTED(ptr)                      __WI_SUPPRESS_4127_S do { if ((ptr) == nullptr) { return E_OUTOFMEMORY; }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_HR_IF_EXPECTED(hr, condition)                    __WI_SUPPRESS_4127_S do { if (wil::verify_bool(condition)) { return wil::verify_hresult(hr); }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_HR_IF_NULL_EXPECTED(hr, ptr)                     __WI_SUPPRESS_4127_S do { if ((ptr) == nullptr) { return wil::verify_hresult(hr); }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_LAST_ERROR_IF_EXPECTED(condition)                __WI_SUPPRESS_4127_S do { if (wil::verify_bool(condition)) { return wil::details::GetLastErrorFailHr(); }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_LAST_ERROR_IF_NULL_EXPECTED(ptr)                 __WI_SUPPRESS_4127_S do { if ((ptr) == nullptr) { return wil::details::GetLastErrorFailHr(); }} __WI_SUPPRESS_4127_E while((void)0, 0)
#define RETURN_IF_NTSTATUS_FAILED_EXPECTED(status)              __WI_SUPPRESS_4127_S do { const NTSTATUS __statusRet = (status); if (FAILED_NTSTATUS(__statusRet)) { return wil::details::NtStatusToHr(__statusRet); }} __WI_SUPPRESS_4127_E while((void)0, 0)

#define __WI_OR_IS_EXPECTED_HRESULT(e) || (__hrRet == wil::verify_hresult(e))
#define RETURN_IF_FAILED_WITH_EXPECTED(hr, hrExpected, ...) \
    do \
    { \
        const auto __hrRet = wil::verify_hresult(hr); \
        if (FAILED(__hrRet)) \
        { \
            if ((__hrRet == wil::verify_hresult(hrExpected)) WI_FOREACH(__WI_OR_IS_EXPECTED_HRESULT, ##__VA_ARGS__)) \
            { \
                return __hrRet; \
            } \
            __RETURN_HR_FAIL(__hrRet, #hr); \
        } \
    } \
    while ((void)0, 0)

//*****************************************************************************
// Macros for logging failures (ignore or pass-through)
//*****************************************************************************

// Always logs a known failure
#define LOG_HR(hr)                                              __R_FN(Log_Hr)(__R_INFO(#hr) wil::verify_hresult(hr))
#define LOG_LAST_ERROR()                                        __R_FN(Log_GetLastError)(__R_INFO_ONLY(nullptr))
#define LOG_WIN32(win32err)                                     __R_FN(Log_Win32)(__R_INFO(#win32err) win32err)
#define LOG_NTSTATUS(status)                                    __R_FN(Log_NtStatus)(__R_INFO(#status) status)

// Conditionally logs failures - returns parameter value
#define LOG_IF_FAILED(hr)                                       __R_FN(Log_IfFailed)(__R_INFO(#hr) wil::verify_hresult(hr))
#define LOG_IF_WIN32_BOOL_FALSE(win32BOOL)                      __R_FN(Log_IfWin32BoolFalse)(__R_INFO(#win32BOOL) wil::verify_BOOL(win32BOOL))
#define LOG_IF_WIN32_ERROR(win32err)                            __R_FN(Log_IfWin32Error)(__R_INFO(#win32err) win32err)
#define LOG_IF_NULL_ALLOC(ptr)                                  __R_FN(Log_IfNullAlloc)(__R_INFO(#ptr) ptr)
#define LOG_HR_IF(hr, condition)                                __R_FN(Log_HrIf)(__R_INFO(#condition) wil::verify_hresult(hr), wil::verify_bool(condition))
#define LOG_HR_IF_NULL(hr, ptr)                                 __R_FN(Log_HrIfNull)(__R_INFO(#ptr) wil::verify_hresult(hr), ptr)
#define LOG_LAST_ERROR_IF(condition)                            __R_FN(Log_GetLastErrorIf)(__R_INFO(#condition) wil::verify_bool(condition))
#define LOG_LAST_ERROR_IF_NULL(ptr)                             __R_FN(Log_GetLastErrorIfNull)(__R_INFO(#ptr) ptr)
#define LOG_IF_NTSTATUS_FAILED(status)                          __R_FN(Log_IfNtStatusFailed)(__R_INFO(#status) status)

// Alternatives for SUCCEEDED(hr) and FAILED(hr) that conditionally log failures
#define SUCCEEDED_LOG(hr)                                       SUCCEEDED(LOG_IF_FAILED(hr))
#define FAILED_LOG(hr)                                          FAILED(LOG_IF_FAILED(hr))
#define SUCCEEDED_WIN32_LOG(win32err)                           SUCCEEDED_WIN32(LOG_IF_WIN32_ERROR(win32err))
#define FAILED_WIN32_LOG(win32err)                              FAILED_WIN32(LOG_IF_WIN32_ERROR(win32err))
#define SUCCEEDED_NTSTATUS_LOG(status)                          SUCCEEDED_NTSTATUS(LOG_IF_NTSTATUS_FAILED(status))
#define FAILED_NTSTATUS_LOG(status)                             FAILED_NTSTATUS(LOG_IF_NTSTATUS_FAILED(status))

// Alternatives for NT_SUCCESS(x) that conditionally logs failures
#define NT_SUCCESS_LOG(status)                                  NT_SUCCESS(LOG_IF_NTSTATUS_FAILED(status))

// Always logs a known failure - logs a var-arg message on failure
#define LOG_HR_MSG(hr, fmt, ...)                                __R_FN(Log_HrMsg)(__R_INFO(#hr) wil::verify_hresult(hr), fmt, ##__VA_ARGS__)
#define LOG_LAST_ERROR_MSG(fmt, ...)                            __R_FN(Log_GetLastErrorMsg)(__R_INFO(nullptr) fmt, ##__VA_ARGS__)
#define LOG_WIN32_MSG(win32err, fmt, ...)                       __R_FN(Log_Win32Msg)(__R_INFO(#win32err) win32err, fmt, ##__VA_ARGS__)
#define LOG_NTSTATUS_MSG(status, fmt, ...)                      __R_FN(Log_NtStatusMsg)(__R_INFO(#status) status, fmt, ##__VA_ARGS__)

// Conditionally logs failures - returns parameter value - logs a var-arg message on failure
#define LOG_IF_FAILED_MSG(hr, fmt, ...)                         __R_FN(Log_IfFailedMsg)(__R_INFO(#hr) wil::verify_hresult(hr), fmt, ##__VA_ARGS__)
#define LOG_IF_WIN32_BOOL_FALSE_MSG(win32BOOL, fmt, ...)        __R_FN(Log_IfWin32BoolFalseMsg)(__R_INFO(#win32BOOL) wil::verify_BOOL(win32BOOL), fmt, ##__VA_ARGS__)
#define LOG_IF_WIN32_ERROR_MSG(win32err, fmt, ...)              __R_FN(Log_IfWin32ErrorMsg)(__R_INFO(#win32err) win32err, fmt, ##__VA_ARGS__)
#define LOG_IF_NULL_ALLOC_MSG(ptr, fmt, ...)                    __R_FN(Log_IfNullAllocMsg)(__R_INFO(#ptr) ptr, fmt, ##__VA_ARGS__)
#define LOG_HR_IF_MSG(hr, condition, fmt, ...)                  __R_FN(Log_HrIfMsg)(__R_INFO(#condition) wil::verify_hresult(hr), wil::verify_bool(condition), fmt, ##__VA_ARGS__)
#define LOG_HR_IF_NULL_MSG(hr, ptr, fmt, ...)                   __R_FN(Log_HrIfNullMsg)(__R_INFO(#ptr) wil::verify_hresult(hr), ptr, fmt, ##__VA_ARGS__)
#define LOG_LAST_ERROR_IF_MSG(condition, fmt, ...)              __R_FN(Log_GetLastErrorIfMsg)(__R_INFO(#condition) wil::verify_bool(condition), fmt, ##__VA_ARGS__)
#define LOG_LAST_ERROR_IF_NULL_MSG(ptr, fmt, ...)               __R_FN(Log_GetLastErrorIfNullMsg)(__R_INFO(#ptr) ptr, fmt, ##__VA_ARGS__)
#define LOG_IF_NTSTATUS_FAILED_MSG(status, fmt, ...)            __R_FN(Log_IfNtStatusFailedMsg)(__R_INFO(#status) status, fmt, ##__VA_ARGS__)

#define __WI_COMMA_EXPECTED_HRESULT(e) , wil::verify_hresult(e)
#define LOG_IF_FAILED_WITH_EXPECTED(hr, hrExpected, ...)        __R_FN(Log_IfFailedWithExpected)(__R_INFO(#hr) wil::verify_hresult(hr), WI_ARGS_COUNT(__VA_ARGS__) + 1, wil::verify_hresult(hrExpected) WI_FOREACH(__WI_COMMA_EXPECTED_HRESULT, ##__VA_ARGS__))

//*****************************************************************************
// Macros to fail fast the process on failures
//*****************************************************************************

// Always fail fast a known failure
#define FAIL_FAST_HR(hr)                                        __RFF_FN(FailFast_Hr)(__RFF_INFO(#hr) wil::verify_hresult(hr))
#define FAIL_FAST_LAST_ERROR()                                  __RFF_FN(FailFast_GetLastError)(__RFF_INFO_ONLY(nullptr))
#define FAIL_FAST_WIN32(win32err)                               __RFF_FN(FailFast_Win32)(__RFF_INFO(#win32err) win32err)
#define FAIL_FAST_NTSTATUS(status)                              __RFF_FN(FailFast_NtStatus)(__RFF_INFO(#status) status)

// Conditionally fail fast failures - returns parameter value
#define FAIL_FAST_IF_FAILED(hr)                                 __RFF_FN(FailFast_IfFailed)(__RFF_INFO(#hr) wil::verify_hresult(hr))
#define FAIL_FAST_IF_WIN32_BOOL_FALSE(win32BOOL)                __RFF_FN(FailFast_IfWin32BoolFalse)(__RFF_INFO(#win32BOOL) wil::verify_BOOL(win32BOOL))
#define FAIL_FAST_IF_WIN32_ERROR(win32err)                      __RFF_FN(FailFast_IfWin32Error)(__RFF_INFO(#win32err) win32err)
#define FAIL_FAST_IF_NULL_ALLOC(ptr)                            __RFF_FN(FailFast_IfNullAlloc)(__RFF_INFO(#ptr) ptr)
#define FAIL_FAST_HR_IF(hr, condition)                          __RFF_FN(FailFast_HrIf)(__RFF_INFO(#condition) wil::verify_hresult(hr), wil::verify_bool(condition))
#define FAIL_FAST_HR_IF_NULL(hr, ptr)                           __RFF_FN(FailFast_HrIfNull)(__RFF_INFO(#ptr) wil::verify_hresult(hr), ptr)
#define FAIL_FAST_LAST_ERROR_IF(condition)                      __RFF_FN(FailFast_GetLastErrorIf)(__RFF_INFO(#condition) wil::verify_bool(condition))
#define FAIL_FAST_LAST_ERROR_IF_NULL(ptr)                       __RFF_FN(FailFast_GetLastErrorIfNull)(__RFF_INFO(#ptr) ptr)
#define FAIL_FAST_IF_NTSTATUS_FAILED(status)                    __RFF_FN(FailFast_IfNtStatusFailed)(__RFF_INFO(#status) status)

// Always fail fast a known failure - fail fast a var-arg message on failure
#define FAIL_FAST_HR_MSG(hr, fmt, ...)                          __RFF_FN(FailFast_HrMsg)(__RFF_INFO(#hr) wil::verify_hresult(hr), fmt, ##__VA_ARGS__)
#define FAIL_FAST_LAST_ERROR_MSG(fmt, ...)                      __RFF_FN(FailFast_GetLastErrorMsg)(__RFF_INFO(nullptr) fmt, ##__VA_ARGS__)
#define FAIL_FAST_WIN32_MSG(win32err, fmt, ...)                 __RFF_FN(FailFast_Win32Msg)(__RFF_INFO(#win32err) win32err, fmt, ##__VA_ARGS__)
#define FAIL_FAST_NTSTATUS_MSG(status, fmt, ...)                __RFF_FN(FailFast_NtStatusMsg)(__RFF_INFO(#status) status, fmt, ##__VA_ARGS__)

// Conditionally fail fast failures - returns parameter value - fail fast a var-arg message on failure
#define FAIL_FAST_IF_FAILED_MSG(hr, fmt, ...)                   __RFF_FN(FailFast_IfFailedMsg)(__RFF_INFO(#hr) wil::verify_hresult(hr), fmt, ##__VA_ARGS__)
#define FAIL_FAST_IF_WIN32_BOOL_FALSE_MSG(win32BOOL, fmt, ...)  __RFF_FN(FailFast_IfWin32BoolFalseMsg)(__RFF_INFO(#win32BOOL) wil::verify_BOOL(win32BOOL), fmt, ##__VA_ARGS__)
#define FAIL_FAST_IF_WIN32_ERROR_MSG(win32err, fmt, ...)        __RFF_FN(FailFast_IfWin32ErrorMsg)(__RFF_INFO(#win32err) win32err, fmt, ##__VA_ARGS__)
#define FAIL_FAST_IF_NULL_ALLOC_MSG(ptr, fmt, ...)              __RFF_FN(FailFast_IfNullAllocMsg)(__RFF_INFO(#ptr) ptr, fmt, ##__VA_ARGS__)
#define FAIL_FAST_HR_IF_MSG(hr, condition, fmt, ...)            __RFF_FN(FailFast_HrIfMsg)(__RFF_INFO(#condition) wil::verify_hresult(hr), wil::verify_bool(condition), fmt, ##__VA_ARGS__)
#define FAIL_FAST_HR_IF_NULL_MSG(hr, ptr, fmt, ...)             __RFF_FN(FailFast_HrIfNullMsg)(__RFF_INFO(#ptr) wil::verify_hresult(hr), ptr, fmt, ##__VA_ARGS__)
#define FAIL_FAST_LAST_ERROR_IF_MSG(condition, fmt, ...)        __RFF_FN(FailFast_GetLastErrorIfMsg)(__RFF_INFO(#condition) wil::verify_bool(condition), fmt, ##__VA_ARGS__)
#define FAIL_FAST_LAST_ERROR_IF_NULL_MSG(ptr, fmt, ...)         __RFF_FN(FailFast_GetLastErrorIfNullMsg)(__RFF_INFO(#ptr) ptr, fmt, ##__VA_ARGS__)
#define FAIL_FAST_IF_NTSTATUS_FAILED_MSG(status, fmt, ...)      __RFF_FN(FailFast_IfNtStatusFailedMsg)(__RFF_INFO(#status) status, fmt, ##__VA_ARGS__)

// Always fail fast a known failure
#ifndef FAIL_FAST
#define FAIL_FAST()                                             __RFF_FN(FailFast_Unexpected)(__RFF_INFO_ONLY(nullptr))
#endif

// Conditionally fail fast failures - returns parameter value
#define FAIL_FAST_IF(condition)                                 __RFF_FN(FailFast_If)(__RFF_INFO(#condition) wil::verify_bool(condition))
#define FAIL_FAST_IF_NULL(ptr)                                  __RFF_FN(FailFast_IfNull)(__RFF_INFO(#ptr) ptr)

// Always fail fast a known failure - fail fast a var-arg message on failure
#define FAIL_FAST_MSG(fmt, ...)                                 __RFF_FN(FailFast_UnexpectedMsg)(__RFF_INFO(nullptr) fmt, ##__VA_ARGS__)

// Conditionally fail fast failures - returns parameter value - fail fast a var-arg message on failure
#define FAIL_FAST_IF_MSG(condition, fmt, ...)                   __RFF_FN(FailFast_IfMsg)(__RFF_INFO(#condition) wil::verify_bool(condition), fmt, ##__VA_ARGS__)
#define FAIL_FAST_IF_NULL_MSG(ptr, fmt, ...)                    __RFF_FN(FailFast_IfNullMsg)(__RFF_INFO(#ptr) ptr, fmt, ##__VA_ARGS__)

// Immediate fail fast (no telemetry - use rarely / only when *already* in an undefined state)
#define FAIL_FAST_IMMEDIATE()                                   __RFF_FN(FailFastImmediate_Unexpected)()

// Conditional immediate fail fast (no telemetry - use rarely / only when *already* in an undefined state)
#define FAIL_FAST_IMMEDIATE_IF_FAILED(hr)                       __RFF_FN(FailFastImmediate_IfFailed)(wil::verify_hresult(hr))
#define FAIL_FAST_IMMEDIATE_IF(condition)                       __RFF_FN(FailFastImmediate_If)(wil::verify_bool(condition))
#define FAIL_FAST_IMMEDIATE_IF_NULL(ptr)                        __RFF_FN(FailFastImmediate_IfNull)(ptr)
#define FAIL_FAST_IMMEDIATE_IF_NTSTATUS_FAILED(status)          __RFF_FN(FailFastImmediate_IfNtStatusFailed)(status)

// Specializations
#define FAIL_FAST_IMMEDIATE_IF_IN_LOADER_CALLOUT()              do { if (wil::details::g_pfnFailFastInLoaderCallout != nullptr) { wil::details::g_pfnFailFastInLoaderCallout(); } } while ((void)0, 0)


//*****************************************************************************
// Macros to throw exceptions on failure
//*****************************************************************************

#ifdef WIL_ENABLE_EXCEPTIONS

// Always throw a known failure
#define THROW_HR(hr)                                            __R_FN(Throw_Hr)(__R_INFO(#hr) wil::verify_hresult(hr))
#define THROW_LAST_ERROR()                                      __R_FN(Throw_GetLastError)(__R_INFO_ONLY(nullptr))
#define THROW_WIN32(win32err)                                   __R_FN(Throw_Win32)(__R_INFO(#win32err) win32err)
#define THROW_EXCEPTION(exception)                              wil::details::ReportFailure_CustomException(__R_INFO(#exception) exception)
#define THROW_NTSTATUS(status)                                  __R_FN(Throw_NtStatus)(__R_INFO(#status) status)

// Conditionally throw failures - returns parameter value
#define THROW_IF_FAILED(hr)                                     __R_FN(Throw_IfFailed)(__R_INFO(#hr) wil::verify_hresult(hr))
#define THROW_IF_WIN32_BOOL_FALSE(win32BOOL)                    __R_FN(Throw_IfWin32BoolFalse)(__R_INFO(#win32BOOL) wil::verify_BOOL(win32BOOL))
#define THROW_IF_WIN32_ERROR(win32err)                          __R_FN(Throw_IfWin32Error)(__R_INFO(#win32err) win32err)
#define THROW_IF_NULL_ALLOC(ptr)                                __R_FN(Throw_IfNullAlloc)(__R_INFO(#ptr) ptr)
#define THROW_HR_IF(hr, condition)                              __R_FN(Throw_HrIf)(__R_INFO(#condition) wil::verify_hresult(hr), wil::verify_bool(condition))
#define THROW_HR_IF_NULL(hr, ptr)                               __R_FN(Throw_HrIfNull)(__R_INFO(#ptr) wil::verify_hresult(hr), ptr)
#define THROW_WIN32_IF(win32err, condition)                     __R_FN(Throw_Win32If)(__R_INFO(#condition) wil::verify_win32(win32err), wil::verify_bool(condition))
#define THROW_LAST_ERROR_IF(condition)                          __R_FN(Throw_GetLastErrorIf)(__R_INFO(#condition) wil::verify_bool(condition))
#define THROW_LAST_ERROR_IF_NULL(ptr)                           __R_FN(Throw_GetLastErrorIfNull)(__R_INFO(#ptr) ptr)
#define THROW_IF_NTSTATUS_FAILED(status)                        __R_FN(Throw_IfNtStatusFailed)(__R_INFO(#status) status)

// Always throw a known failure - throw a var-arg message on failure
#define THROW_HR_MSG(hr, fmt, ...)                              __R_FN(Throw_HrMsg)(__R_INFO(#hr) wil::verify_hresult(hr), fmt, ##__VA_ARGS__)
#define THROW_LAST_ERROR_MSG(fmt, ...)                          __R_FN(Throw_GetLastErrorMsg)(__R_INFO(nullptr) fmt, ##__VA_ARGS__)
#define THROW_WIN32_MSG(win32err, fmt, ...)                     __R_FN(Throw_Win32Msg)(__R_INFO(#win32err) win32err, fmt, ##__VA_ARGS__)
#define THROW_EXCEPTION_MSG(exception, fmt, ...)                wil::details::ReportFailure_CustomExceptionMsg(__R_INFO(#exception) exception, fmt, ##__VA_ARGS__)
#define THROW_NTSTATUS_MSG(status, fmt, ...)                    __R_FN(Throw_NtStatusMsg)(__R_INFO(#status) status, fmt, ##__VA_ARGS__)

// Conditionally throw failures - returns parameter value - throw a var-arg message on failure
#define THROW_IF_FAILED_MSG(hr, fmt, ...)                       __R_FN(Throw_IfFailedMsg)(__R_INFO(#hr) wil::verify_hresult(hr), fmt, ##__VA_ARGS__)
#define THROW_IF_WIN32_BOOL_FALSE_MSG(win32BOOL, fmt, ...)      __R_FN(Throw_IfWin32BoolFalseMsg)(__R_INFO(#win32BOOL) wil::verify_BOOL(win32BOOL), fmt, ##__VA_ARGS__)
#define THROW_IF_WIN32_ERROR_MSG(win32err, fmt, ...)            __R_FN(Throw_IfWin32ErrorMsg)(__R_INFO(#win32err) win32err, fmt, ##__VA_ARGS__)
#define THROW_IF_NULL_ALLOC_MSG(ptr, fmt, ...)                  __R_FN(Throw_IfNullAllocMsg)(__R_INFO(#ptr) ptr, fmt, ##__VA_ARGS__)
#define THROW_HR_IF_MSG(hr, condition, fmt, ...)                __R_FN(Throw_HrIfMsg)(__R_INFO(#condition) wil::verify_hresult(hr), wil::verify_bool(condition), fmt, ##__VA_ARGS__)
#define THROW_HR_IF_NULL_MSG(hr, ptr, fmt, ...)                 __R_FN(Throw_HrIfNullMsg)(__R_INFO(#ptr) wil::verify_hresult(hr), ptr, fmt, ##__VA_ARGS__)
#define THROW_WIN32_IF_MSG(win32err, condition, fmt, ...)       __R_FN(Throw_Win32IfMsg)(__R_INFO(#condition) wil::verify_win32(win32err), wil::verify_bool(condition), fmt, ##__VA_ARGS__)
#define THROW_LAST_ERROR_IF_MSG(condition, fmt, ...)            __R_FN(Throw_GetLastErrorIfMsg)(__R_INFO(#condition) wil::verify_bool(condition), fmt, ##__VA_ARGS__)
#define THROW_LAST_ERROR_IF_NULL_MSG(ptr, fmt, ...)             __R_FN(Throw_GetLastErrorIfNullMsg)(__R_INFO(#ptr) ptr, fmt, ##__VA_ARGS__)
#define THROW_IF_NTSTATUS_FAILED_MSG(status, fmt, ...)          __R_FN(Throw_IfNtStatusFailedMsg)(__R_INFO(#status) status, fmt, ##__VA_ARGS__)


//*****************************************************************************
// Macros to catch and convert exceptions on failure
//*****************************************************************************

// Use these macros *within* a catch (...) block to handle exceptions
#define RETURN_CAUGHT_EXCEPTION()                               return __R_FN(Return_CaughtException)(__R_INFO_ONLY(nullptr))
#define RETURN_CAUGHT_EXCEPTION_MSG(fmt, ...)                   return __R_FN(Return_CaughtExceptionMsg)(__R_INFO(nullptr) fmt, ##__VA_ARGS__)
#define RETURN_CAUGHT_EXCEPTION_EXPECTED()                      return wil::ResultFromCaughtException()
#define LOG_CAUGHT_EXCEPTION()                                  __R_FN(Log_CaughtException)(__R_INFO_ONLY(nullptr))
#define LOG_CAUGHT_EXCEPTION_MSG(fmt, ...)                      __R_FN(Log_CaughtExceptionMsg)(__R_INFO(nullptr) fmt, ##__VA_ARGS__)
#define FAIL_FAST_CAUGHT_EXCEPTION()                            __R_FN(FailFast_CaughtException)(__R_INFO_ONLY(nullptr))
#define FAIL_FAST_CAUGHT_EXCEPTION_MSG(fmt, ...)                __R_FN(FailFast_CaughtExceptionMsg)(__R_INFO(nullptr) fmt, ##__VA_ARGS__)
#define THROW_NORMALIZED_CAUGHT_EXCEPTION()                     __R_FN(Throw_CaughtException)(__R_INFO_ONLY(nullptr))
#define THROW_NORMALIZED_CAUGHT_EXCEPTION_MSG(fmt, ...)         __R_FN(Throw_CaughtExceptionMsg)(__R_INFO(nullptr) fmt, ##__VA_ARGS__)

// Use these macros in place of a catch block to handle exceptions
#define CATCH_RETURN()                                          catch (...) { RETURN_CAUGHT_EXCEPTION(); }
#define CATCH_RETURN_MSG(fmt, ...)                              catch (...) { RETURN_CAUGHT_EXCEPTION_MSG(fmt, ##__VA_ARGS__); }
#define CATCH_RETURN_EXPECTED()                                 catch (...) { RETURN_CAUGHT_EXCEPTION_EXPECTED(); }
#define CATCH_LOG()                                             catch (...) { LOG_CAUGHT_EXCEPTION(); }
// Use CATCH_LOG_RETURN instead of CATCH_LOG in a function-try block around a destructor.  CATCH_LOG in this specific case has an implicit throw at the end of scope.
// Due to a bug (DevDiv 441931), Warning 4297 (function marked noexcept throws exception) is detected even when the throwing code is unreachable, such as the end of scope after a return, in function-level catch.
#define CATCH_LOG_RETURN()                                      catch (...) { __pragma(warning(suppress : 4297)); LOG_CAUGHT_EXCEPTION(); return; }
#define CATCH_LOG_MSG(fmt, ...)                                 catch (...) { LOG_CAUGHT_EXCEPTION_MSG(fmt, ##__VA_ARGS__); }
// Likewise use CATCH_LOG_RETURN_MSG instead of CATCH_LOG_MSG in function-try blocks around destructors.
#define CATCH_LOG_RETURN_MSG(fmt, ...)                          catch (...) { __pragma(warning(suppress : 4297)); LOG_CAUGHT_EXCEPTION_MSG(fmt, ##__VA_ARGS__); return; }
#define CATCH_FAIL_FAST()                                       catch (...) { FAIL_FAST_CAUGHT_EXCEPTION(); }
#define CATCH_FAIL_FAST_MSG(fmt, ...)                           catch (...) { FAIL_FAST_CAUGHT_EXCEPTION_MSG(fmt, ##__VA_ARGS__); }
#define CATCH_THROW_NORMALIZED()                                catch (...) { THROW_NORMALIZED_CAUGHT_EXCEPTION(); }
#define CATCH_THROW_NORMALIZED_MSG(fmt, ...)                    catch (...) { THROW_NORMALIZED_CAUGHT_EXCEPTION_MSG(fmt, ##__VA_ARGS__); }
#define CATCH_LOG_RETURN_HR(hr)                                 catch (...) { LOG_CAUGHT_EXCEPTION(); return hr; }

#endif  // WIL_ENABLE_EXCEPTIONS

// Use this macro to supply diagnostics information to wil::ResultFromException
#define WI_DIAGNOSTICS_INFO                                     wil::DiagnosticsInfo(__R_CALLERADDRESS_VALUE, __R_LINE_VALUE, __R_FILE_VALUE)
#define WI_DIAGNOSTICS_NAME(name)                               wil::DiagnosticsInfo(__R_CALLERADDRESS_VALUE, __R_LINE_VALUE, __R_FILE_VALUE, name)



//*****************************************************************************
// Usage Error Macros
//*****************************************************************************

#ifndef WI_USAGE_ASSERT_STOP
#define WI_USAGE_ASSERT_STOP(condition)                     WI_ASSERT(condition)
#endif
#ifdef RESULT_DEBUG
#define WI_USAGE_ERROR(msg, ...)                            do { LOG_HR_MSG(HRESULT_FROM_WIN32(ERROR_ASSERTION_FAILURE), msg, ##__VA_ARGS__); WI_USAGE_ASSERT_STOP(false); } while ((void)0, 0)
#define WI_USAGE_ERROR_FORWARD(msg, ...)                    do { ReportFailure_ReplaceMsg<FailureType::Log>(__R_FN_CALL_FULL, HRESULT_FROM_WIN32(ERROR_ASSERTION_FAILURE), msg, ##__VA_ARGS__); WI_USAGE_ASSERT_STOP(false); } while ((void)0, 0)
#else
#define WI_USAGE_ERROR(msg, ...)                            do { LOG_HR(HRESULT_FROM_WIN32(ERROR_ASSERTION_FAILURE)); WI_USAGE_ASSERT_STOP(false); } while ((void)0, 0)
#define WI_USAGE_ERROR_FORWARD(msg, ...)                    do { ReportFailure_Hr<FailureType::Log>(__R_FN_CALL_FULL, HRESULT_FROM_WIN32(ERROR_ASSERTION_FAILURE)); WI_USAGE_ASSERT_STOP(false); } while ((void)0, 0)
#endif
#define WI_USAGE_VERIFY(condition, msg, ...)                do { const auto __passed = wil::verify_bool(condition); if (!__passed) { WI_USAGE_ERROR(msg, ##__VA_ARGS__); }} while ((void)0, 0)
#define WI_USAGE_VERIFY_FORWARD(condition, msg, ...)        do { const auto __passed = wil::verify_bool(condition); if (!__passed) { WI_USAGE_ERROR_FORWARD(msg, ##__VA_ARGS__); }} while ((void)0, 0)
#ifdef RESULT_DEBUG
#define WI_USAGE_ASSERT(condition, msg, ...)                WI_USAGE_VERIFY(condition, msg, ##__VA_ARGS__)
#else
#define WI_USAGE_ASSERT(condition, msg, ...)
#endif

//*****************************************************************************
// Internal Error Macros - DO NOT USE - these are for internal WIL use only to reduce sizes of binaries that use WIL
//*****************************************************************************
#ifdef RESULT_DEBUG
#define __WIL_PRIVATE_RETURN_IF_FAILED(hr)                   RETURN_IF_FAILED(hr)
#define __WIL_PRIVATE_RETURN_HR_IF(hr, cond)                 RETURN_HR_IF(hr, cond)
#define __WIL_PRIVATE_RETURN_LAST_ERROR_IF(cond)             RETURN_LAST_ERROR_IF(cond)
#define __WIL_PRIVATE_RETURN_IF_WIN32_BOOL_FALSE(win32BOOL)  RETURN_IF_WIN32_BOOL_FALSE(win32BOOL)
#define __WIL_PRIVATE_RETURN_LAST_ERROR_IF_NULL(ptr)         RETURN_LAST_ERROR_IF_NULL(ptr)
#define __WIL_PRIVATE_RETURN_IF_NULL_ALLOC(ptr)              RETURN_IF_NULL_ALLOC(ptr)
#define __WIL_PRIVATE_RETURN_LAST_ERROR()                    RETURN_LAST_ERROR()
#define __WIL_PRIVATE_FAIL_FAST_HR_IF(hr, condition)         FAIL_FAST_HR_IF(hr, condition)
#define __WIL_PRIVATE_FAIL_FAST_HR(hr)                       FAIL_FAST_HR(hr)
#define __WIL_PRIVATE_LOG_HR(hr)                             LOG_HR(hr)
#else
#define __WIL_PRIVATE_RETURN_IF_FAILED(hr)                   do { const auto __hrRet = wil::verify_hresult(hr); if (FAILED(__hrRet)) { __RETURN_HR_FAIL_NOFILE(__hrRet, #hr); }} while ((void)0, 0)
#define __WIL_PRIVATE_RETURN_HR_IF(hr, cond)                 do { if (wil::verify_bool(cond)) { __RETURN_HR_NOFILE(wil::verify_hresult(hr), #cond); }} while ((void)0, 0)
#define __WIL_PRIVATE_RETURN_LAST_ERROR_IF(cond)             do { if (wil::verify_bool(cond)) { __RETURN_GLE_FAIL_NOFILE(#cond); }} while ((void)0, 0)
#define __WIL_PRIVATE_RETURN_IF_WIN32_BOOL_FALSE(win32BOOL)  do { const BOOL __boolRet = wil::verify_BOOL(win32BOOL); if (!__boolRet) { __RETURN_GLE_FAIL_NOFILE(#win32BOOL); }} while ((void)0, 0)
#define __WIL_PRIVATE_RETURN_LAST_ERROR_IF_NULL(ptr)         do { if ((ptr) == nullptr) { __RETURN_GLE_FAIL_NOFILE(#ptr); }} while ((void)0, 0)
#define __WIL_PRIVATE_RETURN_IF_NULL_ALLOC(ptr)              do { if ((ptr) == nullptr) { __RETURN_HR_FAIL_NOFILE(E_OUTOFMEMORY, #ptr); }} while ((void)0, 0)
#define __WIL_PRIVATE_RETURN_LAST_ERROR()                    __RETURN_GLE_FAIL_NOFILE(nullptr)
#define __WIL_PRIVATE_FAIL_FAST_HR_IF(hr, condition)         __RFF_FN(FailFast_HrIf)(__RFF_INFO_NOFILE(#condition) wil::verify_hresult(hr), wil::verify_bool(condition))
#define __WIL_PRIVATE_FAIL_FAST_HR(hr)                       __RFF_FN(FailFast_Hr)(__RFF_INFO_NOFILE(#hr) wil::verify_hresult(hr))
#define __WIL_PRIVATE_LOG_HR(hr)                             __R_FN(Log_Hr)(__R_INFO_NOFILE(#hr) wil::verify_hresult(hr))
#endif

namespace wil
{
    // Indicates the kind of message / failure type that was used to produce a given error
    enum class FailureType
    {
        Exception,          // THROW_...
        Return,             // RETURN_..._LOG or RETURN_..._MSG
        Log,                // LOG_...
        FailFast            // FAIL_FAST_...
    };

    enum class FailureFlags
    {
        None                     = 0x00,
        RequestFailFast          = 0x01,
        RequestSuppressTelemetry = 0x02,
        RequestDebugBreak        = 0x04,
        NtStatus                 = 0x08,
    };
    DEFINE_ENUM_FLAG_OPERATORS(FailureFlags);

    /** Use with functions and macros that allow customizing which kinds of exceptions are handled.
    This is used with methods like wil::ResultFromException and wil::ResultFromExceptionDebug. */
    enum class SupportedExceptions
    {
        Default,        //!< [Default] all well known exceptions (honors g_fResultFailFastUnknownExceptions).
        Known,          //!< [Known] all well known exceptions (including std::exception).
        All,            //!< [All] all exceptions, known or otherwise.
        None,           //!< [None] no exceptions at all, an exception will fail-fast where thrown.
        Thrown,         //!< [Thrown] exceptions thrown by wil only (Platform::Exception^ or ResultException).
        ThrownOrAlloc   //!< [ThrownOrAlloc] exceptions thrown by wil (Platform::Exception^ or ResultException) or std::bad_alloc.
    };

    // Represents the call context information about a given failure
    // No constructors, destructors or virtual members should be contained within
    struct CallContextInfo
    {
        long contextId;                         // incrementing ID for this call context (unique across an individual module load within process)
        PCSTR contextName;                      // the explicit name given to this context
        PCWSTR contextMessage;                  // [optional] Message that can be associated with the call context
    };

    // Represents all context information about a given failure
    // No constructors, destructors or virtual members should be contained within
    struct FailureInfo
    {
        FailureType type;
        FailureFlags flags;
        HRESULT hr;
        NTSTATUS status;
        long failureId;                         // incrementing ID for this specific failure (unique across an individual module load within process)
        PCWSTR pszMessage;                      // Message is only present for _MSG logging (it's the Sprintf message)
        DWORD threadId;                         // the thread this failure was originally encountered on
        PCSTR pszCode;                          // [debug only] Capture code from the macro
        PCSTR pszFunction;                      // [debug only] The function name
        PCSTR pszFile;
        unsigned int uLineNumber;
        int cFailureCount;                      // How many failures of 'type' have been reported in this module so far
        PCSTR pszCallContext;                   // General breakdown of the call context stack that generated this failure
        CallContextInfo callContextOriginating; // The outermost (first seen) call context
        CallContextInfo callContextCurrent;     // The most recently seen call context
        PCSTR pszModule;                        // The module where the failure originated
        void* returnAddress;                    // The return address to the point that called the macro
        void* callerReturnAddress;              // The return address of the function that includes the macro
    };

    //! Created automatically from using WI_DIAGNOSTICS_INFO to provide diagnostics to functions.
    //! Note that typically wil hides diagnostics from users under the covers by passing them automatically to functions as
    //! parameters hidden behind a macro.  In some cases, the user needs to directly supply these, so this class provides
    //! the mechanism for that.  We only use this for user-passed content as it can't be directly controlled by RESULT_DIAGNOSTICS_LEVEL
    //! to ensure there are no ODR violations (though that variable still controls what parameters within this structure would be available).
    struct DiagnosticsInfo
    {
        void* returnAddress = nullptr;
        PCSTR file = nullptr;
        PCSTR name = nullptr;
        unsigned short line = 0;

        DiagnosticsInfo() = default;

        __forceinline DiagnosticsInfo(void* returnAddress_, unsigned short line_, PCSTR file_) :
            returnAddress(returnAddress_),
            file(file_),
            line(line_)
        {
        }

        __forceinline DiagnosticsInfo(void* returnAddress_, unsigned short line_, PCSTR file_, PCSTR name_) :
            returnAddress(returnAddress_),
            file(file_),
            name(name_),
            line(line_)
        {
        }
    };

    enum class ErrorReturn
    {
        Auto,
        None
    };

    // [optionally] Plug in error logging
    // Note:  This callback is deprecated.  Please use SetResultTelemetryFallback for telemetry or
    // SetResultLoggingCallback for observation.
    extern "C" __declspec(selectany) void(__stdcall *g_pfnResultLoggingCallback)(_Inout_ wil::FailureInfo *pFailure, _Inout_updates_opt_z_(cchDebugMessage) PWSTR pszDebugMessage, _Pre_satisfies_(cchDebugMessage > 0) size_t cchDebugMessage) WI_PFN_NOEXCEPT = nullptr;

    // [optional]
    // This can be explicitly set to control whether or not error messages will be output to OutputDebugString.  It can also
    // be set directly from within the debugger to force console logging for debugging purposes.
    __declspec(selectany) bool g_fResultOutputDebugString = true;

    // [optionally] Allows application to specify a debugger to detect whether a debugger is present.
    // Useful for processes that can only be debugged under kernel debuggers where IsDebuggerPresent returns
    // false.
    __declspec(selectany) bool(__stdcall *g_pfnIsDebuggerPresent)() WI_PFN_NOEXCEPT = nullptr;

    // [optionally] Allows forcing WIL to believe a debugger is present. Useful for when a kernel debugger is attached and ::IsDebuggerPresent returns false
    __declspec(selectany) bool g_fIsDebuggerPresent = false;

    // [optionally] Plug in additional exception-type support (return S_OK when *unable* to remap the exception)
    __declspec(selectany) HRESULT(__stdcall *g_pfnResultFromCaughtException)() WI_PFN_NOEXCEPT = nullptr;

    // [optionally] Use to configure fast fail of unknown exceptions (turn them off).
    __declspec(selectany) bool g_fResultFailFastUnknownExceptions = true;

    // [optionally] Set to false to a configure all THROW_XXX macros in C++/CX to throw ResultException rather than Platform::Exception^
    __declspec(selectany) bool g_fResultThrowPlatformException = true;

    // [optionally] Set to false to a configure all CATCH_ and CAUGHT_ macros to NOT support (fail-fast) std::exception based exceptions (other than std::bad_alloc and wil::ResultException)
    __declspec(selectany) bool g_fResultSupportStdException = true;

    // [optionally] Set to true to cause a debug break to occur on a result failure
    __declspec(selectany) bool g_fBreakOnFailure = false;

    // [optionally] customize failfast behavior
    __declspec(selectany) bool(__stdcall *g_pfnWilFailFast)(const wil::FailureInfo& info) WI_PFN_NOEXCEPT = nullptr;

    /// @cond
    namespace details
    {
        // True if g_pfnResultLoggingCallback is set (allows cutting off backwards compat calls to the function)
        __declspec(selectany) bool g_resultMessageCallbackSet = false;

        // On Desktop/System WINAPI family: convert NTSTATUS error codes to friendly name strings.
        __declspec(selectany) void(__stdcall *g_pfnFormatNtStatusMsg)(NTSTATUS, PWSTR, DWORD) = nullptr;

        _Success_(true) _Ret_range_(dest, destEnd)
        inline PWSTR LogStringPrintf(_Out_writes_to_ptr_(destEnd) _Always_(_Post_z_) PWSTR dest, _Pre_satisfies_(destEnd >= dest) PCWSTR destEnd, _In_ _Printf_format_string_ PCWSTR format, ...)
        {
            va_list argList;
            va_start(argList, format);
            StringCchVPrintfW(dest, (destEnd - dest), format, argList);
            return (destEnd == dest) ? dest : (dest + wcslen(dest));
        }
    }
    /// @endcond

    // This call generates the default logging string that makes its way to OutputDebugString for
    // any particular failure.  This string is also used to associate a failure with a PlatformException^ which
    // only allows a single string to be associated with the exception.
    inline HRESULT GetFailureLogString(_Out_writes_(cchDest) _Always_(_Post_z_) PWSTR pszDest, _Pre_satisfies_(cchDest > 0) _In_ size_t cchDest, _In_ FailureInfo const &failure) WI_NOEXCEPT
    {
        // This function was lenient to empty strings at one point and some callers became dependent on this beahvior
        if ((cchDest == 0) || (pszDest == nullptr))
        {
            return S_OK;
        }

        pszDest[0] = L'\0';

        // Call the logging callback (if present) to allow them to generate the debug string that will be pushed to the console
        // or the platform exception object if the caller desires it.
        if ((g_pfnResultLoggingCallback != nullptr) && details::g_resultMessageCallbackSet)
        {
            // older-form callback was a non-const FailureInfo*; conceptually this is const as callers should not be modifying
            g_pfnResultLoggingCallback(const_cast<FailureInfo*>(&failure), pszDest, cchDest);
        }

        // The callback only optionally needs to supply the debug string -- if the callback didn't populate it, yet we still want
        // it for OutputDebugString or exception message, then generate the default string.
        if (pszDest[0] == L'\0')
        {
            PCSTR pszType = "";
            switch (failure.type)
            {
            case FailureType::Exception:
                pszType = "Exception";
                break;
            case FailureType::Return:
                if (WI_IsFlagSet(failure.flags, FailureFlags::NtStatus))
                {
                    pszType = "ReturnNt";
                }
                else
                {
                    pszType = "ReturnHr";
                }
                break;
            case FailureType::Log:
                if (WI_IsFlagSet(failure.flags, FailureFlags::NtStatus))
                {
                    pszType = "LogNt";
                }
                else
                {
                    pszType = "LogHr";
                }
                break;
            case FailureType::FailFast:
                pszType = "FailFast";
                break;
            }

            wchar_t szErrorText[256]{};
            szErrorText[0] = L'\0';
            LONG errorCode = 0;

            if (WI_IsFlagSet(failure.flags, FailureFlags::NtStatus))
            {
                errorCode = failure.status;
                if (wil::details::g_pfnFormatNtStatusMsg)
                {
                    wil::details::g_pfnFormatNtStatusMsg(failure.status, szErrorText, ARRAYSIZE(szErrorText));
                }
            }
            else
            {
                errorCode = failure.hr;
                FormatMessageW(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS, nullptr, failure.hr, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), szErrorText, ARRAYSIZE(szErrorText), nullptr);
            }

            // %FILENAME(%LINE): %TYPE(%count) tid(%threadid) %HRESULT %SystemMessage
            //     %Caller_MSG [%CODE(%FUNCTION)]

            PWSTR dest = pszDest;
            PCWSTR destEnd = (pszDest + cchDest);

            if (failure.pszFile != nullptr)
            {
                dest = details::LogStringPrintf(dest, destEnd, L"%hs(%u)\\%hs!%p: ", failure.pszFile, failure.uLineNumber, failure.pszModule, failure.returnAddress);
            }
            else
            {
                dest = details::LogStringPrintf(dest, destEnd, L"%hs!%p: ", failure.pszModule, failure.returnAddress);
            }

            if (failure.callerReturnAddress != nullptr)
            {
                dest = details::LogStringPrintf(dest, destEnd, L"(caller: %p) ", failure.callerReturnAddress);
            }

            dest = details::LogStringPrintf(dest, destEnd, L"%hs(%d) tid(%x) %08X %ws", pszType, failure.cFailureCount, ::GetCurrentThreadId(), errorCode, szErrorText);

            if ((failure.pszMessage != nullptr) || (failure.pszCallContext != nullptr) || (failure.pszFunction != nullptr))
            {
                dest = details::LogStringPrintf(dest, destEnd, L"    ");
                if (failure.pszMessage != nullptr)
                {
                    dest = details::LogStringPrintf(dest, destEnd, L"Msg:[%ws] ", failure.pszMessage);
                }
                if (failure.pszCallContext != nullptr)
                {
                    dest = details::LogStringPrintf(dest, destEnd, L"CallContext:[%hs] ", failure.pszCallContext);
                }

                if (failure.pszCode != nullptr)
                {
                    dest = details::LogStringPrintf(dest, destEnd, L"[%hs(%hs)]\n", failure.pszFunction, failure.pszCode);
                }
                else if (failure.pszFunction != nullptr)
                {
                    dest = details::LogStringPrintf(dest, destEnd, L"[%hs]\n", failure.pszFunction);
                }
                else
                {
                    dest = details::LogStringPrintf(dest, destEnd, L"\n");
                }
            }
        }

        // Explicitly choosing to return success in the event of truncation... Current callers
        // depend upon it or it would be eliminated.
        return S_OK;
    }

    /// @cond
    namespace details
    {
        //! Interface used to wrap up code (generally a lambda or other functor) to run in an exception-managed context where
        //! exceptions or errors can be observed and logged.
        struct IFunctor
        {
            virtual HRESULT Run() = 0;
        };

        //! Used to provide custom behavior when an exception is encountered while executing IFunctor
        struct IFunctorHost
        {
            virtual HRESULT Run(IFunctor& functor) = 0;
            virtual HRESULT ExceptionThrown(void* returnAddress) = 0;
        };

        __declspec(noinline) inline HRESULT NtStatusToHr(NTSTATUS status) WI_NOEXCEPT;
        __declspec(noinline) inline NTSTATUS HrToNtStatus(HRESULT) WI_NOEXCEPT;

        struct ResultStatus
        {
            enum Kind : unsigned int { HResult, NtStatus };

            static ResultStatus FromResult(const HRESULT _hr)
            {
                return { _hr, wil::details::HrToNtStatus(_hr), Kind::HResult };
            }
            static ResultStatus FromStatus(const NTSTATUS _status)
            {
                return { wil::details::NtStatusToHr(_status), _status, Kind::NtStatus };
            }
            static ResultStatus FromFailureInfo(const FailureInfo& _failure)
            {
                return { _failure.hr, _failure.status, WI_IsFlagSet(_failure.flags, FailureFlags::NtStatus) ? Kind::NtStatus : Kind::HResult };
            }
            HRESULT hr = S_OK;
            NTSTATUS status = STATUS_SUCCESS;
            Kind kind = Kind::NtStatus;
        };

        // Fallback telemetry provider callback (set with wil::SetResultTelemetryFallback)
        __declspec(selectany) void(__stdcall *g_pfnTelemetryCallback)(bool alreadyReported, wil::FailureInfo const &failure) WI_PFN_NOEXCEPT = nullptr;

        // Result.h plug-in (WIL use only)
        __declspec(selectany) void(__stdcall* g_pfnNotifyFailure)(_Inout_ FailureInfo* pFailure) WI_PFN_NOEXCEPT = nullptr;
        __declspec(selectany) void(__stdcall *g_pfnGetContextAndNotifyFailure)(_Inout_ FailureInfo *pFailure, _Out_writes_(callContextStringLength) _Post_z_ PSTR callContextString, _Pre_satisfies_(callContextStringLength > 0) size_t callContextStringLength) WI_PFN_NOEXCEPT = nullptr;

        // Observe all errors flowing through the system with this callback (set with wil::SetResultLoggingCallback); use with custom logging
        __declspec(selectany) void(__stdcall *g_pfnLoggingCallback)(wil::FailureInfo const &failure) WI_PFN_NOEXCEPT = nullptr;

        // Desktop/System Only:  Module fetch function (automatically setup)
        __declspec(selectany) PCSTR(__stdcall *g_pfnGetModuleName)() WI_PFN_NOEXCEPT = nullptr;

        // Desktop/System Only:  Retrieve address offset and modulename
        __declspec(selectany) bool(__stdcall *g_pfnGetModuleInformation)(void* address, _Out_opt_ unsigned int* addressOffset, _Out_writes_bytes_opt_(size) char* name, size_t size) WI_PFN_NOEXCEPT = nullptr;

        // Called with the expectation that the program will terminate when called inside of a loader callout.
        // Desktop/System Only: Automatically setup when building Windows (BUILD_WINDOWS defined)
        __declspec(selectany) void(__stdcall *g_pfnFailFastInLoaderCallout)() WI_PFN_NOEXCEPT = nullptr;

        // Called to translate an NTSTATUS value to a Win32 error code
        // Desktop/System Only: Automatically setup when building Windows (BUILD_WINDOWS defined)
        __declspec(selectany) ULONG(__stdcall *g_pfnRtlNtStatusToDosErrorNoTeb)(NTSTATUS) WI_PFN_NOEXCEPT = nullptr;

        // Desktop/System Only: Call to DebugBreak
        __declspec(selectany) void(__stdcall *g_pfnDebugBreak)() WI_PFN_NOEXCEPT = nullptr;

        // Called to determine whether or not termination is happening
        // Desktop/System Only: Automatically setup when building Windows (BUILD_WINDOWS defined)
        __declspec(selectany) BOOLEAN(__stdcall *g_pfnDllShutdownInProgress)() WI_PFN_NOEXCEPT = nullptr;
        __declspec(selectany) bool g_processShutdownInProgress = false;

        // On Desktop/System WINAPI family: dynalink RaiseFailFastException because we may encounter modules
        // that do not have RaiseFailFastException in kernelbase.  UWP apps will directly link.
        __declspec(selectany) void (__stdcall *g_pfnRaiseFailFastException)(PEXCEPTION_RECORD,PCONTEXT,DWORD) = nullptr;

        // Exception-based compiled additions
        __declspec(selectany) HRESULT(__stdcall *g_pfnRunFunctorWithExceptionFilter)(IFunctor& functor, IFunctorHost& host, void* returnAddress) = nullptr;
        __declspec(selectany) void(__stdcall *g_pfnRethrow)() = nullptr;
        __declspec(selectany) void(__stdcall *g_pfnThrowResultException)(const FailureInfo& failure) = nullptr;
        extern "C" __declspec(selectany) ResultStatus(__stdcall *g_pfnResultFromCaughtExceptionInternal)(_Out_writes_opt_(debugStringChars) PWSTR debugString, _When_(debugString != nullptr, _Pre_satisfies_(debugStringChars > 0)) size_t debugStringChars, _Out_ bool* isNormalized) WI_PFN_NOEXCEPT = nullptr;

        // C++/WinRT additions
        extern "C" __declspec(selectany) HRESULT(__stdcall *g_pfnResultFromCaughtException_CppWinRt)(_Out_writes_opt_(debugStringChars) PWSTR debugString, _When_(debugString != nullptr, _Pre_satisfies_(debugStringChars > 0)) size_t debugStringChars, _Out_ bool* isNormalized) WI_PFN_NOEXCEPT = nullptr;

        // C++/cx compiled additions
        extern "C" __declspec(selectany) void(__stdcall *g_pfnThrowPlatformException)(FailureInfo const &failure, PCWSTR debugString) = nullptr;
        extern "C" __declspec(selectany) _Always_(_Post_satisfies_(return < 0)) HRESULT(__stdcall *g_pfnResultFromCaughtException_WinRt)(_Inout_updates_opt_(debugStringChars) PWSTR debugString, _When_(debugString != nullptr, _Pre_satisfies_(debugStringChars > 0)) size_t debugStringChars, _Out_ bool* isNormalized) WI_PFN_NOEXCEPT = nullptr;
        __declspec(selectany) _Always_(_Post_satisfies_(return < 0)) HRESULT(__stdcall *g_pfnResultFromKnownExceptions_WinRt)(const DiagnosticsInfo& diagnostics, void* returnAddress, SupportedExceptions supported, IFunctor& functor) = nullptr;

        // Plugin to call RoOriginateError (WIL use only)
        __declspec(selectany) void(__stdcall *g_pfnOriginateCallback)(wil::FailureInfo const& failure) WI_PFN_NOEXCEPT = nullptr;

        // Plugin to call RoFailFastWithErrorContext (WIL use only)
        __declspec(selectany) void(__stdcall* g_pfnFailfastWithContextCallback)(wil::FailureInfo const& failure) WI_PFN_NOEXCEPT = nullptr;


        // Allocate and disown the allocation so that Appverifier does not complain about a false leak
        inline PVOID ProcessHeapAlloc(_In_ DWORD flags, _In_ size_t size) WI_NOEXCEPT
        {
            const HANDLE processHeap = ::GetProcessHeap();
            const PVOID allocation = ::HeapAlloc(processHeap, flags, size);

            static bool fetchedRtlDisownModuleHeapAllocation = false;
            static NTSTATUS (__stdcall *pfnRtlDisownModuleHeapAllocation)(HANDLE, PVOID) WI_PFN_NOEXCEPT = nullptr;

            if (pfnRtlDisownModuleHeapAllocation)
            {
                (void)pfnRtlDisownModuleHeapAllocation(processHeap, allocation);
            }
            else if (!fetchedRtlDisownModuleHeapAllocation)
            {
                if (auto ntdllModule = ::GetModuleHandleW(L"ntdll.dll"))
                {
                    pfnRtlDisownModuleHeapAllocation = reinterpret_cast<decltype(pfnRtlDisownModuleHeapAllocation)>(::GetProcAddress(ntdllModule, "RtlDisownModuleHeapAllocation"));
                }
                fetchedRtlDisownModuleHeapAllocation = true;

                if (pfnRtlDisownModuleHeapAllocation)
                {
                    (void)pfnRtlDisownModuleHeapAllocation(processHeap, allocation);
                }
            }

            return allocation;
        }

        enum class ReportFailureOptions
        {
            None                    = 0x00,
            ForcePlatformException  = 0x01,
            MayRethrow              = 0x02,
        };
        DEFINE_ENUM_FLAG_OPERATORS(ReportFailureOptions);

        template <typename TFunctor>
        using functor_return_type = decltype((*static_cast<TFunctor*>(nullptr))());

        template <typename TFunctor>
        struct functor_wrapper_void : public IFunctor
        {
            TFunctor&& functor;
            functor_wrapper_void(TFunctor&& functor_) : functor(wistd::forward<TFunctor>(functor_)) { }
            #pragma warning(push)
            #pragma warning(disable:4702) // https://github.com/Microsoft/wil/issues/2
            HRESULT Run() override
            {
                functor();
                return S_OK;
            }
            #pragma warning(pop)
        };

        template <typename TFunctor>
        struct functor_wrapper_HRESULT : public IFunctor
        {
            TFunctor&& functor;
            functor_wrapper_HRESULT(TFunctor& functor_) : functor(wistd::forward<TFunctor>(functor_)) { }
            HRESULT Run() override
            {
                return functor();
            }
        };

        template <typename TFunctor, typename TReturn>
        struct functor_wrapper_other : public IFunctor
        {
            TFunctor&& functor;
            TReturn& retVal;
            functor_wrapper_other(TFunctor& functor_, TReturn& retval_) : functor(wistd::forward<TFunctor>(functor_)), retVal(retval_) { }
            #pragma warning(push)
            #pragma warning(disable:4702) // https://github.com/Microsoft/wil/issues/2
            HRESULT Run() override
            {
                retVal = functor();
                return S_OK;
            }
            #pragma warning(pop)
        };

        struct tag_return_void : public wistd::integral_constant<size_t, 0>
        {
            template <typename TFunctor>
            using functor_wrapper = functor_wrapper_void<TFunctor>;
        };

        struct tag_return_HRESULT : public wistd::integral_constant<size_t, 1>
        {
            template <typename TFunctor>
            using functor_wrapper = functor_wrapper_HRESULT<TFunctor>;
        };

        struct tag_return_other : public wistd::integral_constant<size_t, 2>
        {
            template <typename TFunctor, typename TReturn>
            using functor_wrapper = functor_wrapper_other<TFunctor, TReturn>;
        };

        // type-trait to help discover the return type of a functor for tag/dispatch.

        template <ErrorReturn errorReturn, typename T>
        struct return_type
        {
            using type = tag_return_other;
        };

        template <>
        struct return_type<ErrorReturn::Auto, HRESULT>
        {
            using type = tag_return_HRESULT;
        };

        template <>
        struct return_type<ErrorReturn::Auto, void>
        {
            using type = tag_return_void;
        };

        template <>
        struct return_type<ErrorReturn::None, void>
        {
            using type = tag_return_void;
        };

        template <ErrorReturn errorReturn, typename Functor>
        using functor_tag = typename return_type<errorReturn, functor_return_type<Functor>>::type;

        // Forward declarations to enable use of fail fast and reporting internally...
        namespace __R_NS_NAME
        {
            _Post_satisfies_(return == hr) __R_DIRECT_METHOD(HRESULT, Log_Hr)(__R_DIRECT_FN_PARAMS HRESULT hr) WI_NOEXCEPT;
            _Post_satisfies_(return == hr) __R_DIRECT_METHOD(HRESULT, Log_HrMsg)(__R_DIRECT_FN_PARAMS HRESULT hr, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT;
            _Post_satisfies_(return == err) __R_DIRECT_METHOD(DWORD, Log_Win32Msg)(__R_DIRECT_FN_PARAMS DWORD err, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT;
        }
        namespace __RFF_NS_NAME
        {
            __RFF_DIRECT_NORET_METHOD(void, FailFast_Unexpected)(__RFF_DIRECT_FN_PARAMS_ONLY) WI_NOEXCEPT;
            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_) __RFF_CONDITIONAL_METHOD(bool, FailFast_If)(__RFF_CONDITIONAL_FN_PARAMS bool condition) WI_NOEXCEPT;
            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_) __RFF_CONDITIONAL_METHOD(bool, FailFast_HrIf)(__RFF_CONDITIONAL_FN_PARAMS HRESULT hr, bool condition) WI_NOEXCEPT;
            _Post_satisfies_(return == condition) _When_(!condition, _Analysis_noreturn_) __RFF_CONDITIONAL_METHOD(bool, FailFast_IfFalse)(__RFF_CONDITIONAL_FN_PARAMS bool condition) WI_NOEXCEPT;
            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_) __RFF_CONDITIONAL_METHOD(bool, FailFastImmediate_If)(bool condition) WI_NOEXCEPT;
        }

        RESULT_NORETURN inline void __stdcall WilFailFast(const FailureInfo& info);
        inline void LogFailure(__R_FN_PARAMS_FULL, FailureType type, const ResultStatus& resultPair, _In_opt_ PCWSTR message,
                               bool fWantDebugString, _Out_writes_(debugStringSizeChars) _Post_z_ PWSTR debugString, _Pre_satisfies_(debugStringSizeChars > 0) size_t debugStringSizeChars,
                               _Out_writes_(callContextStringSizeChars) _Post_z_ PSTR callContextString, _Pre_satisfies_(callContextStringSizeChars > 0) size_t callContextStringSizeChars,
                               _Out_ FailureInfo *failure) WI_NOEXCEPT;

        __declspec(noinline) inline void ReportFailure(__R_FN_PARAMS_FULL, FailureType type, const ResultStatus& resultPair, _In_opt_ PCWSTR message = nullptr, ReportFailureOptions options = ReportFailureOptions::None);
        template<FailureType, bool = false>
        __declspec(noinline) inline void ReportFailure_Base(__R_FN_PARAMS_FULL, const ResultStatus& resultPair, _In_opt_ PCWSTR message = nullptr, ReportFailureOptions options = ReportFailureOptions::None);
        template<FailureType>
        inline void ReportFailure_ReplaceMsg(__R_FN_PARAMS_FULL, HRESULT hr, _Printf_format_string_ PCSTR formatString, ...);
        __declspec(noinline) inline void ReportFailure_Hr(__R_FN_PARAMS_FULL, FailureType type, HRESULT hr);
        template<FailureType>
        __declspec(noinline) inline void ReportFailure_Hr(__R_FN_PARAMS_FULL, HRESULT hr);
        template<FailureType>
        __declspec(noinline) inline HRESULT ReportFailure_CaughtException(__R_FN_PARAMS_FULL, SupportedExceptions supported = SupportedExceptions::Default);

        //*****************************************************************************
        // Fail fast helpers (for use only internally to WIL)
        //*****************************************************************************

        /// @cond
        #define __FAIL_FAST_ASSERT__(condition)                         do { if (!(condition)) { __RFF_FN(FailFast_Unexpected)(__RFF_INFO_ONLY(#condition)); } } while ((void)0, 0)
        #define __FAIL_FAST_IMMEDIATE_ASSERT__(condition)               do { if (!(condition)) { wil::FailureInfo failure {}; wil::details::WilFailFast(failure); } } while ((void)0, 0)
        #define __FAIL_FAST_ASSERT_WIN32_BOOL_FALSE__(condition)        __RFF_FN(FailFast_IfWin32BoolFalse)(__RFF_INFO(#condition) wil::verify_BOOL(condition))

        // A simple ref-counted buffer class.  The interface is very similar to shared_ptr<>, only it manages
        // an allocated buffer and maintains the size.

        class shared_buffer
        {
        public:
            shared_buffer() WI_NOEXCEPT : m_pCopy(nullptr), m_size(0)
            {
            }

            shared_buffer(shared_buffer const &other) WI_NOEXCEPT : m_pCopy(nullptr), m_size(0)
            {
                assign(other.m_pCopy, other.m_size);
            }

            shared_buffer(shared_buffer &&other) WI_NOEXCEPT :
                m_pCopy(other.m_pCopy),
                m_size(other.m_size)
            {
                other.m_pCopy = nullptr;
                other.m_size = 0;
            }

            ~shared_buffer() WI_NOEXCEPT
            {
                reset();
            }

            shared_buffer& operator=(shared_buffer const &other) WI_NOEXCEPT
            {
                if (this != wistd::addressof(other))
                {
                    assign(other.m_pCopy, other.m_size);
                }
                return *this;
            }

            shared_buffer& operator=(shared_buffer &&other) WI_NOEXCEPT
            {
                if (this != wistd::addressof(other))
                {
                    reset();
                    m_pCopy = other.m_pCopy;
                    m_size = other.m_size;
                    other.m_pCopy = nullptr;
                    other.m_size = 0;
                }
                return *this;
            }

            void reset() WI_NOEXCEPT
            {
                if (m_pCopy != nullptr)
                {
                    if (0 == ::InterlockedDecrementRelease(m_pCopy))
                    {
                        WIL_FreeMemory(m_pCopy);
                    }
                    m_pCopy = nullptr;
                    m_size = 0;
                }
            }

            bool create(_In_reads_bytes_opt_(cbData) void const *pData, size_t cbData) WI_NOEXCEPT
            {
                if (cbData == 0)
                {
                    reset();
                    return true;
                }

                long *pCopyRefCount = reinterpret_cast<long *>(WIL_AllocateMemory(sizeof(long)+cbData));
                if (pCopyRefCount == nullptr)
                {
                    return false;
                }

                *pCopyRefCount = 0;
                if (pData != nullptr)
                {
                    memcpy_s(pCopyRefCount + 1, cbData, pData, cbData); // +1 to advance past sizeof(long) counter
                }
                assign(pCopyRefCount, cbData);
                return true;
            }

            bool create(size_t cbData) WI_NOEXCEPT
            {
                return create(nullptr, cbData);
            }

            WI_NODISCARD void* get(_Out_opt_ size_t *pSize = nullptr) const WI_NOEXCEPT
            {
                if (pSize != nullptr)
                {
                    *pSize = m_size;
                }
                return (m_pCopy == nullptr) ? nullptr : (m_pCopy + 1);
            }

            WI_NODISCARD size_t size() const WI_NOEXCEPT
            {
                return m_size;
            }

            WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
            {
                return (m_pCopy != nullptr);
            }

            WI_NODISCARD bool unique() const WI_NOEXCEPT
            {
                return ((m_pCopy != nullptr) && (*m_pCopy == 1));
            }

        private:
            long *m_pCopy;      // pointer to allocation: refcount + data
            size_t m_size;      // size of the data from m_pCopy

            void assign(_In_opt_ long *pCopy, size_t cbSize) WI_NOEXCEPT
            {
                reset();
                if (pCopy != nullptr)
                {
                    m_pCopy = pCopy;
                    m_size = cbSize;
                    ::InterlockedIncrementNoFence(m_pCopy);
                }
            }
        };

        inline shared_buffer make_shared_buffer_nothrow(_In_reads_bytes_opt_(countBytes) void *pData, size_t countBytes) WI_NOEXCEPT
        {
            shared_buffer buffer;
            buffer.create(pData, countBytes);
            return buffer;
        }

        inline shared_buffer make_shared_buffer_nothrow(size_t countBytes) WI_NOEXCEPT
        {
            shared_buffer buffer;
            buffer.create(countBytes);
            return buffer;
        }

        // A small mimic of the STL shared_ptr class, but unlike shared_ptr, a pointer is not attached to the class, but is
        // always simply contained within (it cannot be attached or detached).

        template <typename object_t>
        class shared_object
        {
        public:
            shared_object() WI_NOEXCEPT : m_pCopy(nullptr)
            {
            }

            shared_object(shared_object const &other) WI_NOEXCEPT :
                m_pCopy(other.m_pCopy)
            {
                    if (m_pCopy != nullptr)
                    {
                        ::InterlockedIncrementNoFence(&m_pCopy->m_refCount);
                    }
                }

            shared_object(shared_object &&other) WI_NOEXCEPT :
            m_pCopy(other.m_pCopy)
            {
                other.m_pCopy = nullptr;
            }

            ~shared_object() WI_NOEXCEPT
            {
                reset();
            }

            shared_object& operator=(shared_object const &other) WI_NOEXCEPT
            {
                if (this != wistd::addressof(other))
                {
                    reset();
                    m_pCopy = other.m_pCopy;
                    if (m_pCopy != nullptr)
                    {
                        ::InterlockedIncrementNoFence(&m_pCopy->m_refCount);
                    }
                }
                return *this;
            }

            shared_object& operator=(shared_object &&other) WI_NOEXCEPT
            {
                if (this != wistd::addressof(other))
                {
                    reset();
                    m_pCopy = other.m_pCopy;
                    other.m_pCopy = nullptr;
                }
                return *this;
            }

            void reset() WI_NOEXCEPT
            {
                if (m_pCopy != nullptr)
                {
                    if (0 == ::InterlockedDecrementRelease(&m_pCopy->m_refCount))
                    {
                        delete m_pCopy;
                    }
                    m_pCopy = nullptr;
                }
            }

            bool create()
            {
                RefAndObject *pObject = new(std::nothrow) RefAndObject();
                if (pObject == nullptr)
                {
                    return false;
                }
                reset();
                m_pCopy = pObject;
                return true;
            }

            template <typename param_t>
            bool create(param_t &&param1)
            {
                RefAndObject *pObject = new(std::nothrow) RefAndObject(wistd::forward<param_t>(param1));
                if (pObject == nullptr)
                {
                    return false;
                }
                reset();
                m_pCopy = pObject;
                return true;
            }

            WI_NODISCARD object_t* get() const WI_NOEXCEPT
            {
                return (m_pCopy == nullptr) ? nullptr : &m_pCopy->m_object;
            }

            WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
            {
                return (m_pCopy != nullptr);
            }

            WI_NODISCARD bool unique() const WI_NOEXCEPT
            {
                return ((m_pCopy != nullptr) && (m_pCopy->m_refCount == 1));
            }

            WI_NODISCARD object_t* operator->() const WI_NOEXCEPT
            {
                return get();
            }

        private:
            struct RefAndObject
            {
                long m_refCount;
                object_t m_object;

                RefAndObject() :
                    m_refCount(1),
                    m_object()
                {
                }

                template <typename param_t>
                RefAndObject(param_t &&param1) :
                    m_refCount(1),
                    m_object(wistd::forward<param_t>(param1))
                {
                }
            };

            RefAndObject *m_pCopy;
        };

        // The following functions are basically the same, but are kept separated to:
        // 1) Provide a unique count and last error code per-type
        // 2) Avoid merging the types to allow easy debugging (breakpoints, conditional breakpoints based
        //      upon count of errors from a particular type, etc)
__WI_PUSH_WARNINGS
#if __clang_major__ >= 13
__WI_CLANG_DISABLE_WARNING(-Wunused-but-set-variable) // s_hrErrorLast used for debugging. We intentionally only assign to it
#endif
        __declspec(noinline) inline int RecordException(HRESULT hr) WI_NOEXCEPT
        {
            static HRESULT volatile s_hrErrorLast = S_OK;
            static long volatile s_cErrorCount = 0;
            s_hrErrorLast = hr;
            return ::InterlockedIncrementNoFence(&s_cErrorCount);
        }

        __declspec(noinline) inline int RecordReturn(HRESULT hr) WI_NOEXCEPT
        {
            static HRESULT volatile s_hrErrorLast = S_OK;
            static long volatile s_cErrorCount = 0;
            s_hrErrorLast = hr;
            return ::InterlockedIncrementNoFence(&s_cErrorCount);
        }

        __declspec(noinline) inline int RecordLog(HRESULT hr) WI_NOEXCEPT
        {
            static HRESULT volatile s_hrErrorLast = S_OK;
            static long volatile s_cErrorCount = 0;
            s_hrErrorLast = hr;
            return ::InterlockedIncrementNoFence(&s_cErrorCount);
        }

        __declspec(noinline) inline int RecordFailFast(HRESULT hr) WI_NOEXCEPT
        {
            static HRESULT volatile s_hrErrorLast = S_OK;
            s_hrErrorLast = hr;
            return 1;
        }
__WI_POP_WARNINGS

        inline RESULT_NORETURN void __stdcall WilRaiseFailFastException(_In_ PEXCEPTION_RECORD er, _In_opt_ PCONTEXT cr, _In_ DWORD flags)
        {
            // if we managed to load the pointer either through WilDynamicRaiseFailFastException (PARTITION_DESKTOP etc.)
            // or via direct linkage (e.g. UWP apps), then use it.
            if (g_pfnRaiseFailFastException)
            {
                g_pfnRaiseFailFastException(er, cr, flags);
            }
            // if not, as a best effort, we are just going to call the intrinsic.
            __fastfail(FAST_FAIL_FATAL_APP_EXIT);
        }

#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_SYSTEM)
        inline bool __stdcall GetModuleInformation(_In_opt_ void* address, _Out_opt_ unsigned int* addressOffset, _Out_writes_bytes_opt_(size) char* name, size_t size) WI_NOEXCEPT
        {
            HMODULE hModule = nullptr;
            if (address && !GetModuleHandleExW(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT, reinterpret_cast<PCWSTR>(address), &hModule))
            {
                assign_to_opt_param(addressOffset, 0U);
                return false;
            }
            if (addressOffset)
            {
                *addressOffset = address ? static_cast<unsigned int>(static_cast<unsigned char*>(address) - reinterpret_cast<unsigned char *>(hModule)) : 0;
            }
            if (name)
            {
                char modulePath[MAX_PATH];
                if (!GetModuleFileNameA(hModule, modulePath, ARRAYSIZE(modulePath)))
                {
                    return false;
                }

                PCSTR start = modulePath + strlen(modulePath);
                while ((start > modulePath) && (*(start - 1) != '\\'))
                {
                    start--;
                }
                StringCchCopyA(name, size, start);
            }
            return true;
        }

        inline PCSTR __stdcall GetCurrentModuleName() WI_NOEXCEPT
        {
            static char s_szModule[64] = {};
            static volatile bool s_fModuleValid = false;
            if (!s_fModuleValid)    // Races are acceptable
            {
                GetModuleInformation(reinterpret_cast<void*>(&RecordFailFast), nullptr, s_szModule, ARRAYSIZE(s_szModule));
                s_fModuleValid = true;
            }
            return s_szModule;
        }

        inline void __stdcall DebugBreak() WI_NOEXCEPT
        {
            ::DebugBreak();
        }

        inline void __stdcall WilDynamicLoadRaiseFailFastException(_In_ PEXCEPTION_RECORD er, _In_ PCONTEXT cr, _In_ DWORD flags)
        {
            auto k32handle = GetModuleHandleW(L"kernelbase.dll");
            _Analysis_assume_(k32handle != nullptr);
            auto pfnRaiseFailFastException = reinterpret_cast<decltype(WilDynamicLoadRaiseFailFastException)*>(GetProcAddress(k32handle, "RaiseFailFastException"));
            if (pfnRaiseFailFastException)
            {
                pfnRaiseFailFastException(er, cr, flags);
            }
        }
#endif  // WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_SYSTEM)

        inline bool __stdcall GetModuleInformationFromAddress(_In_opt_ void* address, _Out_opt_ unsigned int* addressOffset, _Out_writes_bytes_opt_(size) char* buffer, size_t size) WI_NOEXCEPT
        {
            if (size > 0)
            {
                assign_to_opt_param(buffer, '\0');
            }
            if (addressOffset)
            {
                *addressOffset = 0;
            }
            if (g_pfnGetModuleInformation)
            {
                return g_pfnGetModuleInformation(address, addressOffset, buffer, size);
            }
            return false;
        }

        __declspec(noinline) inline HRESULT NtStatusToHr(NTSTATUS status) WI_NOEXCEPT
        {
            // The following conversions are the only known incorrect mappings in RtlNtStatusToDosErrorNoTeb
            if (SUCCEEDED_NTSTATUS(status))
            {
                // All successful status codes have only one hresult equivalent, S_OK
                return S_OK;
            }
            if (status == static_cast<NTSTATUS>(STATUS_NO_MEMORY))
            {
                // RtlNtStatusToDosErrorNoTeb maps STATUS_NO_MEMORY to the less popular of two Win32 no memory error codes resulting in an unexpected mapping
                return E_OUTOFMEMORY;
            }

            if (g_pfnRtlNtStatusToDosErrorNoTeb != nullptr)
            {
                DWORD err = g_pfnRtlNtStatusToDosErrorNoTeb(status);

                // ERROR_MR_MID_NOT_FOUND indicates a bug in the originator of the error (failure to add a mapping to the Win32 error codes).
                // There are known instances of this bug which are unlikely to be fixed soon, and it's always possible that additional instances
                // could be added in the future. In these cases, it's better to use HRESULT_FROM_NT rather than returning a meaningless error.
                if ((err != 0) && (err != ERROR_MR_MID_NOT_FOUND))
                {
                    return __HRESULT_FROM_WIN32(err);
                }
            }

            return HRESULT_FROM_NT(status);
        }

        __declspec(noinline) inline NTSTATUS HrToNtStatus(HRESULT hr) WI_NOEXCEPT
        {
            // Constants taken from ntstatus.h
            static constexpr NTSTATUS WIL_STATUS_INVALID_PARAMETER = 0xC000000D;
            static constexpr NTSTATUS WIL_STATUS_INTERNAL_ERROR = 0xC00000E5;
            static constexpr NTSTATUS WIL_STATUS_INTEGER_OVERFLOW = 0xC0000095;
            static constexpr NTSTATUS WIL_STATUS_OBJECT_PATH_NOT_FOUND = 0xC000003A;
            static constexpr NTSTATUS WIL_STATUS_OBJECT_NAME_NOT_FOUND = 0xC0000034;
            static constexpr NTSTATUS WIL_STATUS_NOT_IMPLEMENTED = 0xC0000002;
            static constexpr NTSTATUS WIL_STATUS_BUFFER_OVERFLOW = 0x80000005;
            static constexpr NTSTATUS WIL_STATUS_IMPLEMENTATION_LIMIT = 0xC000042B;
            static constexpr NTSTATUS WIL_STATUS_NO_MORE_MATCHES = 0xC0000273;
            static constexpr NTSTATUS WIL_STATUS_ILLEGAL_CHARACTER = 0xC0000161;
            static constexpr NTSTATUS WIL_STATUS_UNDEFINED_CHARACTER = 0xC0000163;
            static constexpr NTSTATUS WIL_STATUS_BUFFER_TOO_SMALL = 0xC0000023;
            static constexpr NTSTATUS WIL_STATUS_DISK_FULL = 0xC000007F;
            static constexpr NTSTATUS WIL_STATUS_OBJECT_NAME_INVALID = 0xC0000033;
            static constexpr NTSTATUS WIL_STATUS_DLL_NOT_FOUND = 0xC0000135;
            static constexpr NTSTATUS WIL_STATUS_REVISION_MISMATCH = 0xC0000059;
            static constexpr NTSTATUS WIL_STATUS_XML_PARSE_ERROR = 0xC000A083;
            static constexpr HRESULT WIL_E_FAIL = 0x80004005;

            NTSTATUS status = STATUS_SUCCESS;

            switch (hr)
            {
            case S_OK:
                status = STATUS_SUCCESS;
                break;
            case E_INVALIDARG:
                status = WIL_STATUS_INVALID_PARAMETER;
                break;
            case __HRESULT_FROM_WIN32(ERROR_INTERNAL_ERROR):
                status = WIL_STATUS_INTERNAL_ERROR;
                break;
            case E_OUTOFMEMORY:
                status = STATUS_NO_MEMORY;
                break;
            case __HRESULT_FROM_WIN32(ERROR_ARITHMETIC_OVERFLOW):
                status = WIL_STATUS_INTEGER_OVERFLOW;
                break;
            case __HRESULT_FROM_WIN32(ERROR_PATH_NOT_FOUND):
                status = WIL_STATUS_OBJECT_PATH_NOT_FOUND;
                break;
            case __HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND):
                status = WIL_STATUS_OBJECT_NAME_NOT_FOUND;
                break;
            case __HRESULT_FROM_WIN32(ERROR_INVALID_FUNCTION):
                status = WIL_STATUS_NOT_IMPLEMENTED;
                break;
            case __HRESULT_FROM_WIN32(ERROR_MORE_DATA):
                status = WIL_STATUS_BUFFER_OVERFLOW;
                break;
            case __HRESULT_FROM_WIN32(ERROR_IMPLEMENTATION_LIMIT):
                status = WIL_STATUS_IMPLEMENTATION_LIMIT;
                break;
            case __HRESULT_FROM_WIN32(ERROR_NO_MORE_MATCHES):
                status = WIL_STATUS_NO_MORE_MATCHES;
                break;
            case __HRESULT_FROM_WIN32(ERROR_ILLEGAL_CHARACTER):
                status = WIL_STATUS_ILLEGAL_CHARACTER;
                break;
            case __HRESULT_FROM_WIN32(ERROR_UNDEFINED_CHARACTER):
                status = WIL_STATUS_UNDEFINED_CHARACTER;
                break;
            case __HRESULT_FROM_WIN32(ERROR_INSUFFICIENT_BUFFER):
                status = WIL_STATUS_BUFFER_TOO_SMALL;
                break;
            case __HRESULT_FROM_WIN32(ERROR_DISK_FULL):
                status = WIL_STATUS_DISK_FULL;
                break;
            case __HRESULT_FROM_WIN32(ERROR_INVALID_NAME):
                status = WIL_STATUS_OBJECT_NAME_INVALID;
                break;
            case __HRESULT_FROM_WIN32(ERROR_MOD_NOT_FOUND):
                status = WIL_STATUS_DLL_NOT_FOUND;
                break;
            case __HRESULT_FROM_WIN32(ERROR_OLD_WIN_VERSION):
                status = WIL_STATUS_REVISION_MISMATCH;
                break;
            case WIL_E_FAIL:
                status = STATUS_UNSUCCESSFUL;
                break;
            case __HRESULT_FROM_WIN32(ERROR_XML_PARSE_ERROR):
                status = WIL_STATUS_XML_PARSE_ERROR;
                break;
            case __HRESULT_FROM_WIN32(ERROR_UNHANDLED_EXCEPTION):
                status = STATUS_NONCONTINUABLE_EXCEPTION;
                break;
            default:
                if ((hr & FACILITY_NT_BIT) != 0)
                {
                    status = (hr & ~FACILITY_NT_BIT);
                }
                else if (HRESULT_FACILITY(hr) == FACILITY_WIN32)
                {
                    status = __NTSTATUS_FROM_WIN32(HRESULT_CODE(hr));
                }
                else if (HRESULT_FACILITY(hr) == FACILITY_SSPI)
                {
                    status = ((NTSTATUS)(hr) <= 0 ? ((NTSTATUS)(hr)) : ((NTSTATUS)(((hr) & 0x0000FFFF) | (FACILITY_SSPI << 16) | ERROR_SEVERITY_ERROR)));
                }
                else
                {
                    status = WIL_STATUS_INTERNAL_ERROR;
                }
                break;
            }
            return status;
        }

        // The following set of functions all differ only based upon number of arguments.  They are unified in their handling
        // of data from each of the various error-handling types (fast fail, exceptions, etc.).
        _Post_equals_last_error_
        inline DWORD GetLastErrorFail(__R_FN_PARAMS_FULL) WI_NOEXCEPT
        {
            __R_FN_UNREFERENCED;
            auto err = ::GetLastError();
            if (SUCCEEDED_WIN32(err))
            {
                // This function should only be called when GetLastError() is set to a FAILURE.
                // If you hit this assert (or are reviewing this failure telemetry), then there are one of three issues:
                //  1) Your code is using a macro (such as RETURN_IF_WIN32_BOOL_FALSE()) on a function that does not actually
                //      set the last error (consult MSDN).
                //  2) Your macro check against the error is not immediately after the API call.  Pushing it later can result
                //      in another API call between the previous one and the check resetting the last error.
                //  3) The API you're calling has a bug in it and does not accurately set the last error (there are a few
                //      examples here, such as SendMessageTimeout() that don't accurately set the last error).  For these,
                //      please send mail to 'wildisc' when found and work-around with win32errorhelpers.

                WI_USAGE_ERROR_FORWARD("CALLER BUG: Macro usage error detected.  GetLastError() does not have an error.");
                return ERROR_ASSERTION_FAILURE;
            }
            return err;
        }

        inline __declspec(noinline) DWORD GetLastErrorFail() WI_NOEXCEPT
        {
            __R_FN_LOCALS_FULL_RA;
            return GetLastErrorFail(__R_FN_CALL_FULL);
        }

        _Translates_last_error_to_HRESULT_
        inline HRESULT GetLastErrorFailHr(__R_FN_PARAMS_FULL) WI_NOEXCEPT
        {
            return HRESULT_FROM_WIN32(GetLastErrorFail(__R_FN_CALL_FULL));
        }

        _Translates_last_error_to_HRESULT_
        inline __declspec(noinline) HRESULT GetLastErrorFailHr() WI_NOEXCEPT
        {
            __R_FN_LOCALS_FULL_RA;
            return GetLastErrorFailHr(__R_FN_CALL_FULL);
        }

        inline void PrintLoggingMessage(_Out_writes_(cchDest) _Post_z_ PWSTR pszDest, _Pre_satisfies_(cchDest > 0) size_t cchDest, _In_opt_ _Printf_format_string_ PCSTR formatString, _In_opt_ va_list argList) WI_NOEXCEPT
        {
            if (formatString == nullptr)
            {
                pszDest[0] = L'\0';
            }
            else if (argList == nullptr)
            {
                StringCchPrintfW(pszDest, cchDest, L"%hs", formatString);
            }
            else
            {
                wchar_t szFormatWide[2048];
                StringCchPrintfW(szFormatWide, ARRAYSIZE(szFormatWide), L"%hs", formatString);
                StringCchVPrintfW(pszDest, cchDest, szFormatWide, argList);
            }
        }

#pragma warning(push)
#pragma warning(disable:__WARNING_RETURNING_BAD_RESULT)
        // NOTE: The following two functions are unfortunate copies of strsafe.h functions that have been copied to reduce the friction associated with using
        // Result.h and ResultException.h in a build that does not have WINAPI_PARTITION_DESKTOP defined (where these are conditionally enabled).

        static STRSAFEAPI WilStringLengthWorkerA(_In_reads_or_z_(cchMax) STRSAFE_PCNZCH psz, _In_ _In_range_(<= , STRSAFE_MAX_CCH) size_t cchMax, _Out_opt_ _Deref_out_range_(< , cchMax) _Deref_out_range_(<= , _String_length_(psz)) size_t* pcchLength)
        {
            HRESULT hr = S_OK;
            size_t cchOriginalMax = cchMax;
            while (cchMax && (*psz != '\0'))
            {
                psz++;
                cchMax--;
            }
            if (cchMax == 0)
            {
                // the string is longer than cchMax
                hr = STRSAFE_E_INVALID_PARAMETER;
            }
            if (pcchLength)
            {
                if (SUCCEEDED(hr))
                {
                    *pcchLength = cchOriginalMax - cchMax;
                }
                else
                {
                    *pcchLength = 0;
                }
            }
            return hr;
        }

        _Must_inspect_result_ STRSAFEAPI StringCchLengthA(_In_reads_or_z_(cchMax) STRSAFE_PCNZCH psz, _In_ _In_range_(1, STRSAFE_MAX_CCH) size_t cchMax, _Out_opt_ _Deref_out_range_(<, cchMax) _Deref_out_range_(<= , _String_length_(psz)) size_t* pcchLength)
        {
            HRESULT hr = S_OK;
            if ((psz == nullptr) || (cchMax > STRSAFE_MAX_CCH))
            {
                hr = STRSAFE_E_INVALID_PARAMETER;
            }
            else
            {
                hr = WilStringLengthWorkerA(psz, cchMax, pcchLength);
            }
            if (FAILED(hr) && pcchLength)
            {
                *pcchLength = 0;
            }
            return hr;
        }
#pragma warning(pop)

        _Post_satisfies_(cchDest > 0 && cchDest <= cchMax) static STRSAFEAPI WilStringValidateDestA(_In_reads_opt_(cchDest) STRSAFE_PCNZCH /*pszDest*/, _In_ size_t cchDest, _In_ const size_t cchMax)
        {
            HRESULT hr = S_OK;
            if ((cchDest == 0) || (cchDest > cchMax))
            {
                hr = STRSAFE_E_INVALID_PARAMETER;
            }
            return hr;
        }

        static STRSAFEAPI WilStringVPrintfWorkerA(_Out_writes_(cchDest) _Always_(_Post_z_) STRSAFE_LPSTR pszDest, _In_ _In_range_(1, STRSAFE_MAX_CCH) size_t cchDest, _Always_(_Out_opt_ _Deref_out_range_(<=, cchDest - 1)) size_t* pcchNewDestLength, _In_ _Printf_format_string_ STRSAFE_LPCSTR pszFormat, _In_ va_list argList)
        {
            HRESULT hr = S_OK;
            int iRet{};

            // leave the last space for the null terminator
            size_t cchMax = cchDest - 1;
            size_t cchNewDestLength = 0;
#undef STRSAFE_USE_SECURE_CRT
#define STRSAFE_USE_SECURE_CRT 1
        #if (STRSAFE_USE_SECURE_CRT == 1) && !defined(STRSAFE_LIB_IMPL)
            iRet = _vsnprintf_s(pszDest, cchDest, cchMax, pszFormat, argList);
        #else
        #pragma warning(push)
        #pragma warning(disable: __WARNING_BANNED_API_USAGE)// "STRSAFE not included"
            iRet = _vsnprintf(pszDest, cchMax, pszFormat, argList);
        #pragma warning(pop)
        #endif
            // ASSERT((iRet < 0) || (((size_t)iRet) <= cchMax));

            if ((iRet < 0) || (((size_t)iRet) > cchMax))
            {
                // need to null terminate the string
                pszDest += cchMax;
                *pszDest = '\0';

                cchNewDestLength = cchMax;

                // we have truncated pszDest
                hr = STRSAFE_E_INSUFFICIENT_BUFFER;
            }
            else if (((size_t)iRet) == cchMax)
            {
                // need to null terminate the string
                pszDest += cchMax;
                *pszDest = '\0';

                cchNewDestLength = cchMax;
            }
            else
            {
                cchNewDestLength = (size_t)iRet;
            }

            if (pcchNewDestLength)
            {
                *pcchNewDestLength = cchNewDestLength;
            }

            return hr;
        }

        __inline HRESULT StringCchPrintfA( _Out_writes_(cchDest) _Always_(_Post_z_) STRSAFE_LPSTR pszDest, _In_ size_t cchDest, _In_ _Printf_format_string_ STRSAFE_LPCSTR pszFormat, ...)
        {
            HRESULT hr;
            hr = wil::details::WilStringValidateDestA(pszDest, cchDest, STRSAFE_MAX_CCH);
            if (SUCCEEDED(hr))
            {
                va_list argList;
                va_start(argList, pszFormat);
                hr = wil::details::WilStringVPrintfWorkerA(pszDest, cchDest, nullptr, pszFormat, argList);
                va_end(argList);
            }
            else if (cchDest > 0)
            {
                *pszDest = '\0';
            }
            return hr;
        }

        _Ret_range_(sizeof(char), (psz == nullptr) ? sizeof(char) : (_String_length_(psz) + sizeof(char)))
        inline size_t ResultStringSize(_In_opt_ PCSTR psz)
            { return (psz == nullptr) ? sizeof(char) : (strlen(psz) + sizeof(char)); }

        _Ret_range_(sizeof(wchar_t), (psz == nullptr) ? sizeof(wchar_t) : ((_String_length_(psz) + 1) * sizeof(wchar_t)))
        inline size_t ResultStringSize(_In_opt_ PCWSTR psz)
            { return (psz == nullptr) ? sizeof(wchar_t) : (wcslen(psz) + 1) * sizeof(wchar_t); }

        template<typename TString>
        _Ret_range_(pStart, pEnd) inline unsigned char* WriteResultString(
            _Pre_satisfies_(pStart <= pEnd)
            _When_((pStart == pEnd) || (pszString == nullptr) || (pszString[0] == 0), _In_opt_)
            _When_((pStart != pEnd) && (pszString != nullptr) && (pszString[0] != 0), _Out_writes_bytes_opt_(_String_length_(pszString) * sizeof(pszString[0])))
            unsigned char* pStart, _Pre_satisfies_(pEnd >= pStart) unsigned char* pEnd, _In_opt_z_ TString pszString, _Outptr_result_maybenull_z_ TString* ppszBufferString)
        {
            // No space? Null string? Do nothing.
            if ((pStart == pEnd) || !pszString || !*pszString)
            {
                assign_null_to_opt_param(ppszBufferString);
                return pStart;
            }

            // Treats the range pStart--pEnd as a memory buffer into which pszString is copied. A pointer to
            // the start of the copied string is placed into ppszStringBuffer. If the buffer isn't big enough,
            // do nothing, and tell the caller nothing was written.
            size_t const stringSize = ResultStringSize(pszString);
            size_t const bufferSize = pEnd - pStart;
            if (bufferSize < stringSize)
            {
                assign_null_to_opt_param(ppszBufferString);
                return pStart;
            }

            memcpy_s(pStart, bufferSize, pszString, stringSize);
            assign_to_opt_param(ppszBufferString, reinterpret_cast<TString>(pStart));// lgtm[cpp/incorrect-string-type-conversion] False positive - The query is misinterpreting a buffer (char *) with a MBS string, the cast to TString is expected.
            return pStart + stringSize;
        }

        _Ret_range_(0, (cchMax > 0) ? cchMax - 1 : 0) inline size_t UntrustedStringLength(_In_ PCSTR psz, _In_ size_t cchMax)    { size_t cbLength; return SUCCEEDED(wil::details::StringCchLengthA(psz, cchMax, &cbLength)) ? cbLength : 0; }
        _Ret_range_(0, (cchMax > 0) ? cchMax - 1 : 0) inline size_t UntrustedStringLength(_In_ PCWSTR psz, _In_ size_t cchMax)   { size_t cbLength; return SUCCEEDED(::StringCchLengthW(psz, cchMax, &cbLength)) ? cbLength : 0; }

        template<typename TString>
        _Ret_range_(pStart, pEnd) inline unsigned char *GetResultString(_In_reads_to_ptr_opt_(pEnd) unsigned char *pStart, _Pre_satisfies_(pEnd >= pStart) unsigned char *pEnd, _Out_ TString *ppszBufferString)
        {
            size_t cchLen = UntrustedStringLength(reinterpret_cast<TString>(pStart), (pEnd - pStart) / sizeof((*ppszBufferString)[0]));
            *ppszBufferString = (cchLen > 0) ? reinterpret_cast<TString>(pStart) : nullptr;
            auto pReturn = (wistd::min)(pEnd, pStart + ((cchLen + 1) * sizeof((*ppszBufferString)[0])));
            __analysis_assume((pReturn >= pStart) && (pReturn <= pEnd));
            return pReturn;
        }
    } // details namespace
    /// @endcond

    //*****************************************************************************
    // WIL result handling initializers
    //
    // Generally, callers do not need to manually initialize WIL. This header creates
    // the appropriate .CRT init section pieces through global objects to ensure that
    // WilInitialize... is called before DllMain or main().
    //
    // Certain binaries do not link with the CRT or do not support .CRT-section based
    // initializers. Those binaries must link only with other static libraries that
    // also set RESULT_SUPPRESS_STATIC_INITIALIZERS to ensure no .CRT inits are left,
    // and they should call one of the WilInitialize_ResultMacros_??? methods during
    // their initialization phase.  Skipping this initialization path is OK as well,
    // but results in a slightly degraded experience with result reporting.
    //
    // Calling WilInitialize_ResultMacros_DesktopOrSystem_SuppressPrivateApiUse provides:
    // - The name of the current module in wil::FailureInfo::pszModule
    // - The name of the returning-to module during wil\staging.h failures
    //*****************************************************************************

#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_SYSTEM)
    //! Call this method to initialize WIL manually in a module where RESULT_SUPPRESS_STATIC_INITIALIZERS is required. WIL will
    //! only use publicly documented APIs.
    inline void WilInitialize_ResultMacros_DesktopOrSystem_SuppressPrivateApiUse()
    {
        details::g_pfnGetModuleName        = details::GetCurrentModuleName;
        details::g_pfnGetModuleInformation = details::GetModuleInformation;
        details::g_pfnDebugBreak           = details::DebugBreak;
        details::g_pfnRaiseFailFastException = wil::details::WilDynamicLoadRaiseFailFastException;
    }

    /// @cond
    namespace details
    {
#ifndef RESULT_SUPPRESS_STATIC_INITIALIZERS
#if !defined(BUILD_WINDOWS) || defined(WIL_SUPPRESS_PRIVATE_API_USE)
        WI_HEADER_INITITALIZATION_FUNCTION(WilInitialize_ResultMacros_DesktopOrSystem_SuppressPrivateApiUse, []
        {
            ::wil::WilInitialize_ResultMacros_DesktopOrSystem_SuppressPrivateApiUse();
            return 1;
        });
#endif
#endif
    }
    /// @endcond
#else // !WINAPI_PARTITION_DESKTOP, !WINAPI_PARTITION_SYSTEM, explicitly assume these modules can direct link
    namespace details
    {
        WI_HEADER_INITITALIZATION_FUNCTION(WilInitialize_ResultMacros_AppOnly, []
        {
            g_pfnRaiseFailFastException = ::RaiseFailFastException;
            return 1;
        });
    }
#endif // WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_SYSTEM)

    //*****************************************************************************
    // Public Error Handling Helpers
    //*****************************************************************************

    //! Call this method to determine if process shutdown is in progress (allows avoiding work during dll unload).
    inline bool ProcessShutdownInProgress()
    {
        return (details::g_processShutdownInProgress || (details::g_pfnDllShutdownInProgress ? details::g_pfnDllShutdownInProgress() : false));
    }

    /** Use this object to wrap an object that wants to prevent its destructor from being run when the process is shutting down,
    but the hosting DLL doesn't support CRT initializers (such as kernelbase.dll).  The hosting DLL is responsible for calling
    Construct() and Destroy() to manually run the constructor and destructor during DLL load & unload.
    Upon process shutdown a method (ProcessShutdown()) is called that must be implemented on the object, otherwise the destructor is
    called as is typical. */
    template<class T>
    class manually_managed_shutdown_aware_object
    {
    public:
        manually_managed_shutdown_aware_object() = default;
        manually_managed_shutdown_aware_object(manually_managed_shutdown_aware_object const&) = delete;
        void operator=(manually_managed_shutdown_aware_object const&) = delete;

        void construct()
        {
            void* var = &m_raw;
            ::new(var) T();
        }

        void destroy()
        {
            if (ProcessShutdownInProgress())
            {
                get().ProcessShutdown();
            }
            else
            {
                (&get())->~T();
            }
        }

        //! Retrieves a reference to the contained object
        T& get() WI_NOEXCEPT
        {
            return *reinterpret_cast<T*>(&m_raw);
        }

    private:
        alignas(T) unsigned char m_raw[sizeof(T)];
    };

    /** Use this object to wrap an object that wants to prevent its destructor from being run when the process is shutting down.
    Upon process shutdown a method (ProcessShutdown()) is called that must be implemented on the object, otherwise the destructor is
    called as is typical. */
    template<class T>
    class shutdown_aware_object
    {
    public:
        shutdown_aware_object()
        {
            m_object.construct();
        }

        ~shutdown_aware_object()
        {
            m_object.destroy();
        }

        shutdown_aware_object(shutdown_aware_object const&) = delete;
        void operator=(shutdown_aware_object const&) = delete;

        //! Retrieves a reference to the contained object
        T& get() WI_NOEXCEPT
        {
            return m_object.get();
        }

    private:
        manually_managed_shutdown_aware_object<T> m_object;
    };

    /** Use this object to wrap an object that wants to prevent its destructor from being run when the process is shutting down. */
    template<class T>
    class object_without_destructor_on_shutdown
    {
    public:
        object_without_destructor_on_shutdown()
        {
            void* var = &m_raw;
            ::new(var) T();
        }

        ~object_without_destructor_on_shutdown()
        {
            if (!ProcessShutdownInProgress())
            {
                get().~T();
            }
        }

        object_without_destructor_on_shutdown(object_without_destructor_on_shutdown const&) = delete;
        void operator=(object_without_destructor_on_shutdown const&) = delete;

        //! Retrieves a reference to the contained object
        T& get() WI_NOEXCEPT
        {
            return *reinterpret_cast<T*>(&m_raw);
        }

    private:
        alignas(T) unsigned char m_raw[sizeof(T)]{};
    };

    /** Forward your DLLMain to this function so that WIL can have visibility into whether a DLL unload is because
    of termination or normal unload.  Note that when g_pfnDllShutdownInProgress is set, WIL attempts to make this
    determination on its own without this callback.  Suppressing private APIs requires use of this. */
    inline void DLLMain(HINSTANCE, DWORD reason, _In_opt_ LPVOID reserved)
    {
        if (!details::g_processShutdownInProgress)
        {
            if ((reason == DLL_PROCESS_DETACH) && (reserved != nullptr))
            {
                details::g_processShutdownInProgress = true;
            }
        }
    }

    // [optionally] Plug in fallback telemetry reporting
    // Normally, the callback is owned by including ResultLogging.h in the including module.  Alternatively a module
    // could re-route fallback telemetry to any ONE specific provider by calling this method.
    inline void SetResultTelemetryFallback(_In_opt_ decltype(details::g_pfnTelemetryCallback) callbackFunction)
    {
        // Only ONE telemetry provider can own the fallback telemetry callback.
        __FAIL_FAST_IMMEDIATE_ASSERT__((details::g_pfnTelemetryCallback == nullptr) || (callbackFunction == nullptr) || (details::g_pfnTelemetryCallback == callbackFunction));
        details::g_pfnTelemetryCallback = callbackFunction;
    }

    // [optionally] Plug in result logging (do not use for telemetry)
    // This provides the ability for a module to hook all failures flowing through the system for inspection
    // and/or logging.
    inline void SetResultLoggingCallback(_In_opt_ decltype(details::g_pfnLoggingCallback) callbackFunction)
    {
        // Only ONE function can own the result logging callback
        __FAIL_FAST_IMMEDIATE_ASSERT__((details::g_pfnLoggingCallback == nullptr) || (callbackFunction == nullptr) || (details::g_pfnLoggingCallback == callbackFunction));
        details::g_pfnLoggingCallback = callbackFunction;
    }

    // [optionally] Plug in custom result messages
    // There are some purposes that require translating the full information that is known about a failure
    // into a message to be logged (either through the console for debugging OR as the message attached
    // to a Platform::Exception^).  This callback allows a module to format the string itself away from the
    // default.
    inline void SetResultMessageCallback(_In_opt_ decltype(wil::g_pfnResultLoggingCallback) callbackFunction)
    {
        // Only ONE function can own the result message callback
        __FAIL_FAST_IMMEDIATE_ASSERT__((g_pfnResultLoggingCallback == nullptr) || (callbackFunction == nullptr) || (g_pfnResultLoggingCallback == callbackFunction));
        details::g_resultMessageCallbackSet = true;
        g_pfnResultLoggingCallback = callbackFunction;
    }

    // [optionally] Plug in exception remapping
    // A module can plug a callback in using this function to setup custom exception handling to allow any
    // exception type to be converted into an HRESULT from exception barriers.
    inline void SetResultFromCaughtExceptionCallback(_In_opt_ decltype(wil::g_pfnResultFromCaughtException) callbackFunction)
    {
        // Only ONE function can own the exception conversion
        __FAIL_FAST_IMMEDIATE_ASSERT__((g_pfnResultFromCaughtException == nullptr) || (callbackFunction == nullptr) || (g_pfnResultFromCaughtException == callbackFunction));
        g_pfnResultFromCaughtException = callbackFunction;
    }

    // [optionally] Plug in exception remapping
    // This provides the ability for a module to call RoOriginateError in case of a failure.
    // Normally, the callback is owned by including result_originate.h in the including module.  Alternatively a module
    // could re-route error origination callback to its own implementation.
    inline void SetOriginateErrorCallback(_In_opt_ decltype(details::g_pfnOriginateCallback) callbackFunction)
    {
        // Only ONE function can own the error origination callback
        __FAIL_FAST_IMMEDIATE_ASSERT__((details::g_pfnOriginateCallback == nullptr) || (callbackFunction == nullptr) || (details::g_pfnOriginateCallback == callbackFunction));
        details::g_pfnOriginateCallback = callbackFunction;
    }

    // [optionally] Plug in failfast callback
    // This provides the ability for a module to call RoFailFastWithErrorContext in the failfast handler -if- there is stowed
    // exception data available.  Normally, the callback is owned by including result_originate.h in the including module.
    // Alternatively a module could re-route to its own implementation.
    inline void SetFailfastWithContextCallback(_In_opt_ decltype(details::g_pfnFailfastWithContextCallback) callbackFunction)
    {
        // Only ONE function can own the failfast with context callback
        __FAIL_FAST_IMMEDIATE_ASSERT__((details::g_pfnFailfastWithContextCallback == nullptr) || (callbackFunction == nullptr) || (details::g_pfnFailfastWithContextCallback == callbackFunction));
        details::g_pfnFailfastWithContextCallback = callbackFunction;
    }

    // A RAII wrapper around the storage of a FailureInfo struct (which is normally meant to be consumed
    // on the stack or from the caller).  The storage of FailureInfo needs to copy some data internally
    // for lifetime purposes.

    class StoredFailureInfo
    {
    public:
        StoredFailureInfo() WI_NOEXCEPT
        {
            ::ZeroMemory(&m_failureInfo, sizeof(m_failureInfo));
        }

        StoredFailureInfo(FailureInfo const &other) WI_NOEXCEPT
        {
            SetFailureInfo(other);
        }

        WI_NODISCARD FailureInfo const& GetFailureInfo() const WI_NOEXCEPT
        {
            return m_failureInfo;
        }

        void SetFailureInfo(FailureInfo const &failure) WI_NOEXCEPT
        {
            m_failureInfo = failure;

            size_t const cbNeed = details::ResultStringSize(failure.pszMessage) +
                                  details::ResultStringSize(failure.pszCode) +
                                  details::ResultStringSize(failure.pszFunction) +
                                  details::ResultStringSize(failure.pszFile) +
                                  details::ResultStringSize(failure.pszCallContext) +
                                  details::ResultStringSize(failure.pszModule) +
                                  details::ResultStringSize(failure.callContextCurrent.contextName) +
                                  details::ResultStringSize(failure.callContextCurrent.contextMessage) +
                                  details::ResultStringSize(failure.callContextOriginating.contextName) +
                                  details::ResultStringSize(failure.callContextOriginating.contextMessage);

            if (!m_spStrings.unique() || (m_spStrings.size() < cbNeed))
            {
                m_spStrings.reset();
                m_spStrings.create(cbNeed);
            }

            size_t cbAlloc;
            unsigned char *pBuffer = static_cast<unsigned char *>(m_spStrings.get(&cbAlloc));
            unsigned char *pBufferEnd = (pBuffer != nullptr) ? pBuffer + cbAlloc : nullptr;

            if (pBuffer)
            {
                pBuffer = details::WriteResultString(pBuffer, pBufferEnd, failure.pszMessage, &m_failureInfo.pszMessage);
                pBuffer = details::WriteResultString(pBuffer, pBufferEnd, failure.pszCode, &m_failureInfo.pszCode);
                pBuffer = details::WriteResultString(pBuffer, pBufferEnd, failure.pszFunction, &m_failureInfo.pszFunction);
                pBuffer = details::WriteResultString(pBuffer, pBufferEnd, failure.pszFile, &m_failureInfo.pszFile);
                pBuffer = details::WriteResultString(pBuffer, pBufferEnd, failure.pszCallContext, &m_failureInfo.pszCallContext);
                pBuffer = details::WriteResultString(pBuffer, pBufferEnd, failure.pszModule, &m_failureInfo.pszModule);
                pBuffer = details::WriteResultString(pBuffer, pBufferEnd, failure.callContextCurrent.contextName, &m_failureInfo.callContextCurrent.contextName);
                pBuffer = details::WriteResultString(pBuffer, pBufferEnd, failure.callContextCurrent.contextMessage, &m_failureInfo.callContextCurrent.contextMessage);
                pBuffer = details::WriteResultString(pBuffer, pBufferEnd, failure.callContextOriginating.contextName, &m_failureInfo.callContextOriginating.contextName);
                pBuffer = details::WriteResultString(pBuffer, pBufferEnd, failure.callContextOriginating.contextMessage, &m_failureInfo.callContextOriginating.contextMessage);
                ZeroMemory(pBuffer, pBufferEnd - pBuffer);
            }
        }

        // Relies upon generated copy constructor and assignment operator

    protected:
        FailureInfo m_failureInfo;
        details::shared_buffer m_spStrings;
    };

#if defined(WIL_ENABLE_EXCEPTIONS) || defined(WIL_FORCE_INCLUDE_RESULT_EXCEPTION)

    //! This is WIL's default exception class thrown from all THROW_XXX macros (outside of c++/cx).
    //! This class stores all of the FailureInfo context that is available when the exception is thrown.  It's also caught by
    //! exception guards for automatic conversion to HRESULT.
    //!
    //! In c++/cx, Platform::Exception^ is used instead of this class (unless @ref wil::g_fResultThrowPlatformException has been changed).
    class ResultException : public std::exception
    {
    public:
        //! Constructs a new ResultException from an existing FailureInfo.
        ResultException(const FailureInfo& failure) WI_NOEXCEPT :
            m_failure(failure)
        {
        }

        //! Constructs a new exception type from a given HRESULT (use only for constructing custom exception types).
        ResultException(_Pre_satisfies_(hr < 0) HRESULT hr) WI_NOEXCEPT :
            m_failure(CustomExceptionFailureInfo(hr))
        {
        }

        //! Returns the failed HRESULT that this exception represents.
        _Always_(_Post_satisfies_(return < 0)) WI_NODISCARD HRESULT GetErrorCode() const WI_NOEXCEPT
        {
            HRESULT const hr = m_failure.GetFailureInfo().hr;
            __analysis_assume(hr < 0);
            return hr;
        }

        //! Returns the failed NTSTATUS that this exception represents.
        _Always_(_Post_satisfies_(return < 0)) WI_NODISCARD NTSTATUS GetStatusCode() const WI_NOEXCEPT
        {
            NTSTATUS const status = m_failure.GetFailureInfo().status;
            __analysis_assume(status < 0);
            return status;
        }

        //! Get a reference to the stored FailureInfo.
        WI_NODISCARD FailureInfo const& GetFailureInfo() const WI_NOEXCEPT
        {
            return m_failure.GetFailureInfo();
        }

        //! Sets the stored FailureInfo (use primarily only when constructing custom exception types).
        void SetFailureInfo(FailureInfo const &failure) WI_NOEXCEPT
        {
            m_failure.SetFailureInfo(failure);
        }

        //! Provides a string representing the FailureInfo from this exception.
        WI_NODISCARD inline const char* __CLR_OR_THIS_CALL what() const WI_NOEXCEPT override
        {
            if (!m_what)
            {
                wchar_t message[2048];
                GetFailureLogString(message, ARRAYSIZE(message), m_failure.GetFailureInfo());

                char messageA[1024];
                wil::details::StringCchPrintfA(messageA, ARRAYSIZE(messageA), "%ws", message);
                m_what.create(messageA, strlen(messageA) + sizeof(*messageA));
            }
            return static_cast<const char *>(m_what.get());
        }

        // Relies upon auto-generated copy constructor and assignment operator
    protected:
        StoredFailureInfo m_failure;                //!< The failure information for this exception
        mutable details::shared_buffer m_what;      //!< The on-demand generated what() string

        //! Use to produce a custom FailureInfo from an HRESULT (use only when constructing custom exception types).
        static FailureInfo CustomExceptionFailureInfo(HRESULT hr) WI_NOEXCEPT
        {
            FailureInfo fi = {};
            fi.type = FailureType::Exception;
            fi.hr = hr;
            return fi;
        }
    };
#endif


    //*****************************************************************************
    // Public Helpers that catch -- mostly only enabled when exceptions are enabled
    //*****************************************************************************

    // ResultFromCaughtException is a function that is meant to be called from within a catch(...) block.  Internally
    // it re-throws and catches the exception to convert it to an HRESULT.  If an exception is of an unrecognized type
    // the function will fail fast.
    //
    // try
    // {
    //     // Code
    // }
    // catch (...)
    // {
    //     hr = wil::ResultFromCaughtException();
    // }
    _Always_(_Post_satisfies_(return < 0))
    __declspec(noinline) inline HRESULT ResultFromCaughtException() WI_NOEXCEPT
    {
        bool isNormalized = false;
        HRESULT hr = S_OK;
        if (details::g_pfnResultFromCaughtExceptionInternal)
        {
            hr = details::g_pfnResultFromCaughtExceptionInternal(nullptr, 0, &isNormalized).hr;
        }
        if (FAILED(hr))
        {
            return hr;
        }

        // Caller bug: an unknown exception was thrown
        __WIL_PRIVATE_FAIL_FAST_HR_IF(__HRESULT_FROM_WIN32(ERROR_UNHANDLED_EXCEPTION), g_fResultFailFastUnknownExceptions);
        return __HRESULT_FROM_WIN32(ERROR_UNHANDLED_EXCEPTION);
    }

    //! Identical to 'throw;', but can be called from error-code neutral code to rethrow in code that *may* be running under an exception context
    inline void RethrowCaughtException()
    {
        // We always want to rethrow the exception under normal circumstances.  Ordinarily, we could actually guarantee
        // this as we should be able to rethrow if we caught an exception, but if we got here in the middle of running
        // dynamic initializers, then it's possible that we haven't yet setup the rethrow function pointer, thus the
        // runtime check without the noreturn annotation.

        if (details::g_pfnRethrow)
        {
            details::g_pfnRethrow();
        }
    }

    //! Identical to 'throw ResultException(failure);', but can be referenced from error-code neutral code
    inline void ThrowResultException(const FailureInfo& failure)
    {
        if (details::g_pfnThrowResultException)
        {
            details::g_pfnThrowResultException(failure);
        }
    }

    //! @cond
    namespace details
    {
#ifdef WIL_ENABLE_EXCEPTIONS
        //*****************************************************************************
        // Private helpers to catch and propagate exceptions
        //*****************************************************************************

        RESULT_NORETURN inline void TerminateAndReportError(_In_opt_ PEXCEPTION_POINTERS)
        {
            // This is an intentional fail-fast that was caught by an exception guard with WIL.  Look back up the callstack to determine
            // the source of the actual exception being thrown.  The exception guard used by the calling code did not expect this
            // exception type to be thrown or is specifically requesting fail-fast for this class of exception.

            FailureInfo failure{};
            WilFailFast(failure);
        }

        inline void MaybeGetExceptionString(const ResultException& exception, _Out_writes_opt_(debugStringChars) PWSTR debugString, _When_(debugString != nullptr, _Pre_satisfies_(debugStringChars > 0)) size_t debugStringChars)
        {
            if (debugString)
            {
                GetFailureLogString(debugString, debugStringChars, exception.GetFailureInfo());
            }
        }

        inline void MaybeGetExceptionString(const std::exception& exception, _Out_writes_opt_(debugStringChars) PWSTR debugString, _When_(debugString != nullptr, _Pre_satisfies_(debugStringChars > 0)) size_t debugStringChars)
        {
            if (debugString)
            {
                StringCchPrintfW(debugString, debugStringChars, L"std::exception: %hs", exception.what());
            }
        }

        inline HRESULT ResultFromKnownException(const ResultException& exception, const DiagnosticsInfo& diagnostics, void* returnAddress)
        {
            wchar_t message[2048]{};
            message[0] = L'\0';
            MaybeGetExceptionString(exception, message, ARRAYSIZE(message));
            auto hr = exception.GetErrorCode();
            wil::details::ReportFailure_Base<FailureType::Log>(__R_DIAGNOSTICS_RA(diagnostics, returnAddress), ResultStatus::FromResult(hr), message);
            return hr;
        }

        inline HRESULT ResultFromKnownException(const std::bad_alloc& exception, const DiagnosticsInfo& diagnostics, void* returnAddress)
        {
            wchar_t message[2048]{};
            message[0] = L'\0';
            MaybeGetExceptionString(exception, message, ARRAYSIZE(message));
            constexpr auto hr = E_OUTOFMEMORY;
            wil::details::ReportFailure_Base<FailureType::Log>(__R_DIAGNOSTICS_RA(diagnostics, returnAddress), ResultStatus::FromResult(hr), message);
            return hr;
        }

        inline HRESULT ResultFromKnownException(const std::exception& exception, const DiagnosticsInfo& diagnostics, void* returnAddress)
        {
            wchar_t message[2048]{};
            message[0] = L'\0';
            MaybeGetExceptionString(exception, message, ARRAYSIZE(message));
            constexpr auto hr = __HRESULT_FROM_WIN32(ERROR_UNHANDLED_EXCEPTION);
            ReportFailure_Base<FailureType::Log>(__R_DIAGNOSTICS_RA(diagnostics, returnAddress), ResultStatus::FromResult(hr), message);
            return hr;
        }

        inline HRESULT ResultFromKnownException_CppWinRT(const DiagnosticsInfo& diagnostics, void* returnAddress)
        {
            if (g_pfnResultFromCaughtException_CppWinRt)
            {
                wchar_t message[2048]{};
                message[0] = L'\0';
                bool ignored;
                auto hr = g_pfnResultFromCaughtException_CppWinRt(message, ARRAYSIZE(message), &ignored);
                if (FAILED(hr))
                {
                    ReportFailure_Base<FailureType::Log>(__R_DIAGNOSTICS_RA(diagnostics, returnAddress), ResultStatus::FromResult(hr), message);
                    return hr;
                }
            }

            // Indicate that this either isn't a C++/WinRT exception or a handler isn't configured by returning success
            return S_OK;
        }

        inline HRESULT RecognizeCaughtExceptionFromCallback(_Inout_updates_opt_(debugStringChars) PWSTR debugString, _When_(debugString != nullptr, _Pre_satisfies_(debugStringChars > 0)) size_t debugStringChars)
        {
            HRESULT hr = g_pfnResultFromCaughtException();

            // If we still don't know the error -- or we would like to get the debug string for the error (if possible) we
            // rethrow and catch std::exception.

            if (SUCCEEDED(hr) || debugString)
            {
                try
                {
                    throw;
                }
                catch (std::exception& exception)
                {
                    MaybeGetExceptionString(exception, debugString, debugStringChars);
                    if (SUCCEEDED(hr))
                    {
                        hr = __HRESULT_FROM_WIN32(ERROR_UNHANDLED_EXCEPTION);
                    }
                }
                catch (...)
                {
                    // Fall through to returning 'hr' below
                }
            }

            return hr;
        }

#ifdef __cplusplus_winrt
        inline Platform::String^ GetPlatformExceptionMessage(Platform::Exception^ exception)
        {
            struct RawExceptionData_Partial
            {
                PCWSTR description;
                PCWSTR restrictedErrorString;
            };

            auto exceptionPtr = reinterpret_cast<void*>(static_cast<::Platform::Object^>(exception));
            auto exceptionInfoPtr = reinterpret_cast<ULONG_PTR*>(exceptionPtr) - 1;
            auto partial = reinterpret_cast<RawExceptionData_Partial*>(*exceptionInfoPtr);

            Platform::String^ message = exception->Message;

            PCWSTR errorString = partial->restrictedErrorString;
            PCWSTR messageString = reinterpret_cast<PCWSTR>(message ? message->Data() : nullptr);

            // An old Platform::Exception^ bug that did not actually expose the error string out of the exception
            // message.  We do it by hand here if the message associated with the strong does not contain the
            // message that was originally attached to the string (in the fixed version it will).

            if ((errorString && *errorString && messageString) &&
                (wcsstr(messageString, errorString) == nullptr))
            {
                return ref new Platform::String(reinterpret_cast<_Null_terminated_ const __wchar_t *>(errorString));
            }
            return message;
        }

        inline void MaybeGetExceptionString(_In_ Platform::Exception^ exception, _Out_writes_opt_(debugStringChars) PWSTR debugString, _When_(debugString != nullptr, _Pre_satisfies_(debugStringChars > 0)) size_t debugStringChars)
        {
            if (debugString)
            {
                auto message = GetPlatformExceptionMessage(exception);
                auto messageString = !message ? L"(null Message)" : reinterpret_cast<PCWSTR>(message->Data());
                StringCchPrintfW(debugString, debugStringChars, L"Platform::Exception^: %ws", messageString);
            }
        }

        inline HRESULT ResultFromKnownException(Platform::Exception^ exception, const DiagnosticsInfo& diagnostics, void* returnAddress)
        {
            wchar_t message[2048];
            message[0] = L'\0';
            MaybeGetExceptionString(exception, message, ARRAYSIZE(message));
            auto hr = exception->HResult;
            wil::details::ReportFailure_Base<FailureType::Log>(__R_DIAGNOSTICS_RA(diagnostics, returnAddress), ResultStatus::FromResult(hr), message);
            return hr;
        }

        inline HRESULT __stdcall ResultFromCaughtException_WinRt(_Inout_updates_opt_(debugStringChars) PWSTR debugString, _When_(debugString != nullptr, _Pre_satisfies_(debugStringChars > 0)) size_t debugStringChars, _Inout_ bool* isNormalized) WI_NOEXCEPT
        {
            if (g_pfnResultFromCaughtException)
            {
                try
                {
                    throw;
                }
                catch (const ResultException& exception)
                {
                    MaybeGetExceptionString(exception, debugString, debugStringChars);
                    return exception.GetErrorCode();
                }
                catch (Platform::Exception^ exception)
                {
                    *isNormalized = true;
                    // We need to call __abi_translateCurrentException so that the CX runtime will pull the originated error information
                    // out of the exception object and place it back into thread-local storage.
                    __abi_translateCurrentException(false);
                    MaybeGetExceptionString(exception, debugString, debugStringChars);
                    return exception->HResult;
                }
                catch (const std::bad_alloc& exception)
                {
                    MaybeGetExceptionString(exception, debugString, debugStringChars);
                    return E_OUTOFMEMORY;
                }
                catch (...)
                {
                    auto hr = RecognizeCaughtExceptionFromCallback(debugString, debugStringChars);
                    if (FAILED(hr))
                    {
                        return hr;
                    }
                }
            }
            else
            {
                try
                {
                    throw;
                }
                catch (const ResultException& exception)
                {
                    MaybeGetExceptionString(exception, debugString, debugStringChars);
                    return exception.GetErrorCode();
                }
                catch (Platform::Exception^ exception)
                {
                    *isNormalized = true;
                    // We need to call __abi_translateCurrentException so that the CX runtime will pull the originated error information
                    // out of the exception object and place it back into thread-local storage.
                    __abi_translateCurrentException(false);
                    MaybeGetExceptionString(exception, debugString, debugStringChars);
                    return exception->HResult;
                }
                catch (const std::bad_alloc& exception)
                {
                    MaybeGetExceptionString(exception, debugString, debugStringChars);
                    return E_OUTOFMEMORY;
                }
                catch (std::exception& exception)
                {
                    MaybeGetExceptionString(exception, debugString, debugStringChars);
                    return HRESULT_FROM_WIN32(ERROR_UNHANDLED_EXCEPTION);
                }
                catch (...)
                {
                    // Fall through to returning 'S_OK' below
                }
            }

            // Tell the caller that we were unable to map the exception by succeeding...
            return S_OK;
        }

        // WinRT supporting version to execute a functor and catch known exceptions.
        inline HRESULT __stdcall ResultFromKnownExceptions_WinRt(const DiagnosticsInfo& diagnostics, void* returnAddress, SupportedExceptions supported, IFunctor& functor)
        {
            WI_ASSERT(supported != SupportedExceptions::Default);

            switch (supported)
            {
            case SupportedExceptions::Known:
                try
                {
                    return functor.Run();
                }
                catch (const ResultException& exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }
                catch (Platform::Exception^ exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }
                catch (const std::bad_alloc& exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }
                catch (std::exception& exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }
                catch (...)
                {
                    auto hr = ResultFromKnownException_CppWinRT(diagnostics, returnAddress);
                    if (FAILED(hr))
                    {
                        return hr;
                    }

                    // Unknown exception
                    throw;
                }
                break;

            case SupportedExceptions::ThrownOrAlloc:
                try
                {
                    return functor.Run();
                }
                catch (const ResultException& exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }
                catch (Platform::Exception^ exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }
                catch (const std::bad_alloc& exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }
                break;

            case SupportedExceptions::Thrown:
                try
                {
                    return functor.Run();
                }
                catch (const ResultException& exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }
                catch (Platform::Exception^ exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }
                break;
            }

            WI_ASSERT(false);
            return S_OK;
        }

        inline void __stdcall ThrowPlatformException(FailureInfo const &failure, LPCWSTR debugString)
        {
            throw Platform::Exception::CreateException(failure.hr, ref new Platform::String(reinterpret_cast<_Null_terminated_ const __wchar_t *>(debugString)));
        }

#if !defined(RESULT_SUPPRESS_STATIC_INITIALIZERS)
        WI_HEADER_INITITALIZATION_FUNCTION(InitializeWinRt, []
        {
            g_pfnResultFromCaughtException_WinRt = ResultFromCaughtException_WinRt;
            g_pfnResultFromKnownExceptions_WinRt = ResultFromKnownExceptions_WinRt;
            g_pfnThrowPlatformException = ThrowPlatformException;
            return 1;
        });
#endif
#endif

        inline void __stdcall Rethrow()
        {
            throw;
        }

        inline void __stdcall ThrowResultExceptionInternal(const FailureInfo& failure)
        {
            throw ResultException(failure);
        }

        __declspec(noinline) inline ResultStatus __stdcall ResultFromCaughtExceptionInternal(_Out_writes_opt_(debugStringChars) PWSTR debugString, _When_(debugString != nullptr, _Pre_satisfies_(debugStringChars > 0)) size_t debugStringChars, _Out_ bool* isNormalized) WI_NOEXCEPT
        {
            if (debugString)
            {
                *debugString = L'\0';
            }
            *isNormalized = false;

            if (details::g_pfnResultFromCaughtException_CppWinRt != nullptr)
            {
                const auto hr = details::g_pfnResultFromCaughtException_CppWinRt(debugString, debugStringChars, isNormalized);
                if (FAILED(hr))
                {
                    return ResultStatus::FromResult(hr);
                }
            }

            if (details::g_pfnResultFromCaughtException_WinRt != nullptr)
            {
                const auto hr = details::g_pfnResultFromCaughtException_WinRt(debugString, debugStringChars, isNormalized);
                return ResultStatus::FromResult(hr);
            }

            if (g_pfnResultFromCaughtException)
            {
                try
                {
                    throw;
                }
                catch (const ResultException& exception)
                {
                    *isNormalized = true;
                    MaybeGetExceptionString(exception, debugString, debugStringChars);
                    return ResultStatus::FromFailureInfo(exception.GetFailureInfo());
                }
                catch (const std::bad_alloc& exception)
                {
                    MaybeGetExceptionString(exception, debugString, debugStringChars);
                    return ResultStatus::FromResult(E_OUTOFMEMORY);
                }
                catch (...)
                {
                    auto hr = RecognizeCaughtExceptionFromCallback(debugString, debugStringChars);
                    if (FAILED(hr))
                    {
                        return ResultStatus::FromResult(hr);
                    }
                }
            }
            else
            {
                try
                {
                    throw;
                }
                catch (const ResultException& exception)
                {
                    *isNormalized = true;
                    MaybeGetExceptionString(exception, debugString, debugStringChars);
                    return ResultStatus::FromFailureInfo(exception.GetFailureInfo());
                }
                catch (const std::bad_alloc& exception)
                {
                    MaybeGetExceptionString(exception, debugString, debugStringChars);
                    return ResultStatus::FromResult(E_OUTOFMEMORY);
                }
                catch (std::exception& exception)
                {
                    MaybeGetExceptionString(exception, debugString, debugStringChars);
                    return ResultStatus::FromResult(__HRESULT_FROM_WIN32(ERROR_UNHANDLED_EXCEPTION));
                }
                catch (...)
                {
                    // Fall through to returning 'S_OK' below
                }
            }

            // Tell the caller that we were unable to map the exception by succeeding...
            return ResultStatus::FromResult(S_OK);
        }

        // Runs the given functor, converting any exceptions of the supported types that are known to HRESULTs and returning
        // that HRESULT.  Does NOT attempt to catch unknown exceptions (which propagate).  Primarily used by SEH exception
        // handling techniques to stop at the point the exception is thrown.
        inline HRESULT ResultFromKnownExceptions(const DiagnosticsInfo& diagnostics, void* returnAddress, SupportedExceptions supported, IFunctor& functor)
        {
            if (supported == SupportedExceptions::Default)
            {
                supported = g_fResultSupportStdException ? SupportedExceptions::Known : SupportedExceptions::ThrownOrAlloc;
            }

            if ((details::g_pfnResultFromKnownExceptions_WinRt != nullptr) &&
                ((supported == SupportedExceptions::Known) || (supported == SupportedExceptions::Thrown) || (supported == SupportedExceptions::ThrownOrAlloc)))
            {
                return details::g_pfnResultFromKnownExceptions_WinRt(diagnostics, returnAddress, supported, functor);
            }

            switch (supported)
            {
            case SupportedExceptions::Known:
                try
                {
                    return functor.Run();
                }
                catch (const ResultException& exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }
                catch (const std::bad_alloc& exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }
                catch (std::exception& exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }
                catch (...)
                {
                    auto hr = ResultFromKnownException_CppWinRT(diagnostics, returnAddress);
                    if (FAILED(hr))
                    {
                        return hr;
                    }

                    // Unknown exception
                    throw;
                }

            case SupportedExceptions::ThrownOrAlloc:
                try
                {
                    return functor.Run();
                }
                catch (const ResultException& exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }
                catch (const std::bad_alloc& exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }

            case SupportedExceptions::Thrown:
                try
                {
                    return functor.Run();
                }
                catch (const ResultException& exception)
                {
                    return ResultFromKnownException(exception, diagnostics, returnAddress);
                }

            case SupportedExceptions::All:
                try
                {
                    return functor.Run();
                }
                catch (...)
                {
                    return wil::details::ReportFailure_CaughtException<FailureType::Log>(__R_DIAGNOSTICS_RA(diagnostics, returnAddress), supported);
                }

            case SupportedExceptions::None:
                return functor.Run();

            case SupportedExceptions::Default:
                WI_ASSERT(false);
            }

            WI_ASSERT(false);
            return S_OK;
        }

        inline HRESULT ResultFromExceptionSeh(const DiagnosticsInfo& diagnostics, void* returnAddress, SupportedExceptions supported, IFunctor& functor) WI_NOEXCEPT
        {
            __try
            {
                return wil::details::ResultFromKnownExceptions(diagnostics, returnAddress, supported, functor);
            }
            __except (wil::details::TerminateAndReportError(GetExceptionInformation()), EXCEPTION_CONTINUE_SEARCH)
            {
                WI_ASSERT(false);
                RESULT_NORETURN_RESULT(HRESULT_FROM_WIN32(ERROR_UNHANDLED_EXCEPTION));
            }
        }

        __declspec(noinline) inline HRESULT ResultFromException(const DiagnosticsInfo& diagnostics, SupportedExceptions supported, IFunctor& functor) WI_NOEXCEPT
        {
#ifdef RESULT_DEBUG
            // We can't do debug SEH handling if the caller also wants a shot at mapping the exceptions
            // themselves or if the caller doesn't want to fail-fast unknown exceptions
            if ((g_pfnResultFromCaughtException == nullptr) && g_fResultFailFastUnknownExceptions)
            {
                return wil::details::ResultFromExceptionSeh(diagnostics, _ReturnAddress(), supported, functor);
            }
#endif
            try
            {
                return functor.Run();
            }
            catch (...)
            {
                return wil::details::ReportFailure_CaughtException<FailureType::Log>(__R_DIAGNOSTICS(diagnostics), _ReturnAddress(), supported);
            }
        }

        __declspec(noinline) inline HRESULT ResultFromExceptionDebug(const DiagnosticsInfo& diagnostics, SupportedExceptions supported, IFunctor& functor) WI_NOEXCEPT
        {
            return wil::details::ResultFromExceptionSeh(diagnostics, _ReturnAddress(), supported, functor);
        }

        // Exception guard -- catch exceptions and log them (or handle them with a custom callback)
        // WARNING: may throw an exception...
        inline HRESULT __stdcall RunFunctorWithExceptionFilter(IFunctor& functor, IFunctorHost& host, void* returnAddress)
        {
            try
            {
                return host.Run(functor);
            }
            catch (...)
            {
                // Note that the host may choose to re-throw, throw a normalized exception, return S_OK and eat the exception or
                // return the remapped failure.
                return host.ExceptionThrown(returnAddress);
            }
        }

        WI_HEADER_INITITALIZATION_FUNCTION(InitializeResultExceptions, []
        {
            g_pfnRunFunctorWithExceptionFilter = RunFunctorWithExceptionFilter;
            g_pfnRethrow = Rethrow;
            g_pfnThrowResultException = ThrowResultExceptionInternal;
            g_pfnResultFromCaughtExceptionInternal = ResultFromCaughtExceptionInternal;
            return 1;
        });

    }

    //! A lambda-based exception guard that can vary the supported exception types.
    //! This function accepts a lambda and diagnostics information as its parameters and executes that lambda
    //! under a try/catch(...) block.  All exceptions are caught and the function reports the exception information
    //! and diagnostics to telemetry on failure.  An HRESULT is returned that maps to the exception.
    //!
    //! Note that an overload exists that does not report failures to telemetry at all.  This version should be preferred
    //! to that version.  Also note that neither of these versions are preferred over using try catch blocks to accomplish
    //! the same thing as they will be more efficient.
    //!
    //! See @ref page_exception_guards for more information and examples on exception guards.
    //! ~~~~
    //! return wil::ResultFromException(WI_DIAGNOSTICS_INFO, [&]
    //! {
    //!     // exception-based code
    //!     // telemetry is reported with full exception information
    //! });
    //! ~~~~
    //! @param diagnostics  Always pass WI_DIAGNOSTICS_INFO as the first parameter
    //! @param supported    What kind of exceptions you want to support
    //! @param functor      A lambda that accepts no parameters; any return value is ignored
    //! @return             S_OK on success (no exception thrown) or an error based upon the exception thrown
    template <typename Functor>
    __forceinline HRESULT ResultFromException(const DiagnosticsInfo& diagnostics, SupportedExceptions supported, Functor&& functor) WI_NOEXCEPT
    {
        static_assert(details::functor_tag<ErrorReturn::None, Functor>::value != details::tag_return_other::value, "Functor must return void or HRESULT");
        typename details::functor_tag<ErrorReturn::None, Functor>::template functor_wrapper<Functor> functorObject(wistd::forward<Functor>(functor));

        return wil::details::ResultFromException(diagnostics, supported, functorObject);
    }

    //! A lambda-based exception guard.
    //! This overload uses SupportedExceptions::Known by default.  See @ref ResultFromException for more detailed information.
    template <typename Functor>
    __forceinline HRESULT ResultFromException(const DiagnosticsInfo& diagnostics, Functor&& functor) WI_NOEXCEPT
    {
        return ResultFromException(diagnostics, SupportedExceptions::Known, wistd::forward<Functor>(functor));
    }

    //! A lambda-based exception guard that does not report failures to telemetry.
    //! This function accepts a lambda as it's only parameter and executes that lambda under a try/catch(...) block.
    //! All exceptions are caught and the function returns an HRESULT mapping to the exception.
    //!
    //! This version (taking only a lambda) does not report failures to telemetry.  An overload with the same name
    //! can be utilized by passing `WI_DIAGNOSTICS_INFO` as the first parameter and the lambda as the second parameter
    //! to report failure information to telemetry.
    //!
    //! See @ref page_exception_guards for more information and examples on exception guards.
    //! ~~~~
    //! hr = wil::ResultFromException([&]
    //! {
    //!     // exception-based code
    //!     // the conversion of exception to HRESULT doesn't report telemetry
    //! });
    //!
    //! hr = wil::ResultFromException(WI_DIAGNOSTICS_INFO, [&]
    //! {
    //!     // exception-based code
    //!     // telemetry is reported with full exception information
    //! });
    //! ~~~~
    //! @param functor  A lambda that accepts no parameters; any return value is ignored
    //! @return         S_OK on success (no exception thrown) or an error based upon the exception thrown
    template <typename Functor>
    inline HRESULT ResultFromException(Functor&& functor) WI_NOEXCEPT try
    {
        static_assert(details::functor_tag<ErrorReturn::None, Functor>::value == details::tag_return_void::value, "Functor must return void");
        typename details::functor_tag<ErrorReturn::None, Functor>::template functor_wrapper<Functor> functorObject(wistd::forward<Functor>(functor));

        functorObject.Run();
        return S_OK;
    }
    catch (...)
    {
        return ResultFromCaughtException();
    }


    //! A lambda-based exception guard that can identify the origin of unknown exceptions and can vary the supported exception types.
    //! Functionally this is nearly identical to the corresponding @ref ResultFromException function with the exception
    //! that it utilizes structured exception handling internally to be able to terminate at the point where a unknown
    //! exception is thrown, rather than after that unknown exception has been unwound.  Though less efficient, this leads
    //! to a better debugging experience when analyzing unknown exceptions.
    //!
    //! For example:
    //! ~~~~
    //! hr = wil::ResultFromExceptionDebug(WI_DIAGNOSTICS_INFO, [&]
    //! {
    //!     FunctionWhichMayThrow();
    //! });
    //! ~~~~
    //! Assume FunctionWhichMayThrow() has a bug in it where it accidentally does a `throw E_INVALIDARG;`.  This ends up
    //! throwing a `long` as an exception object which is not what the caller intended.  The normal @ref ResultFromException
    //! would fail-fast when this is encountered, but it would do so AFTER FunctionWhichMayThrow() is already off of the
    //! stack and has been unwound.  Because SEH is used for ResultFromExceptionDebug, the fail-fast occurs with everything
    //! leading up to and including the `throw INVALIDARG;` still on the stack (and easily debuggable).
    //!
    //! The penalty paid for using this, however, is efficiency.  It's far less efficient as a general pattern than either
    //! using ResultFromException directly or especially using try with CATCH_ macros directly.  Still it's helpful to deploy
    //! selectively to isolate issues a component may be having with unknown/unhandled exceptions.
    //!
    //! The ability to vary the SupportedExceptions that this routine provides adds the ability to track down unexpected
    //! exceptions not falling into the supported category easily through fail-fast.  For example, by not supporting any
    //! exception, you can use this function to quickly add an exception guard that will fail-fast any exception at the point
    //! the exception occurs (the throw) in a codepath where the origination of unknown exceptions need to be tracked down.
    //!
    //! Also see @ref ResultFromExceptionDebugNoStdException.  It functions almost identically, but also will fail-fast and stop
    //! on std::exception based exceptions (but not Platform::Exception^ or wil::ResultException).  Using this can help isolate
    //! where an unexpected exception is being generated from.
    //! @param diagnostics  Always pass WI_DIAGNOSTICS_INFO as the first parameter
    //! @param supported    What kind of exceptions you want to support
    //! @param functor      A lambda that accepts no parameters; any return value is ignored
    //! @return             S_OK on success (no exception thrown) or an error based upon the exception thrown
    template <typename Functor>
    __forceinline HRESULT ResultFromExceptionDebug(const DiagnosticsInfo& diagnostics, SupportedExceptions supported, Functor&& functor) WI_NOEXCEPT
    {
        static_assert(details::functor_tag<ErrorReturn::None, Functor>::value == details::tag_return_void::value, "Functor must return void");
        typename details::functor_tag<ErrorReturn::None, Functor>::template functor_wrapper<Functor> functorObject(wistd::forward<Functor>(functor));

        return wil::details::ResultFromExceptionDebug(diagnostics, supported, functorObject);
    }

    //! A lambda-based exception guard that can identify the origin of unknown exceptions.
    //! This overload uses SupportedExceptions::Known by default.  See @ref ResultFromExceptionDebug for more detailed information.
    template <typename Functor>
    __forceinline HRESULT ResultFromExceptionDebug(const DiagnosticsInfo& diagnostics, Functor&& functor) WI_NOEXCEPT
    {
        static_assert(details::functor_tag<ErrorReturn::None, Functor>::value == details::tag_return_void::value, "Functor must return void");
        typename details::functor_tag<ErrorReturn::None, Functor>::template functor_wrapper<Functor> functorObject(wistd::forward<Functor>(functor));

        return wil::details::ResultFromExceptionDebug(diagnostics, SupportedExceptions::Known, functorObject);
    }

    //! A fail-fast based exception guard.
    //! Technically this is an overload of @ref ResultFromExceptionDebug that uses SupportedExceptions::None by default.  Any uncaught
    //! exception that makes it back to this guard would result in a fail-fast at the point the exception is thrown.
    template <typename Functor>
    __forceinline void FailFastException(const DiagnosticsInfo& diagnostics, Functor&& functor) WI_NOEXCEPT
    {
        static_assert(details::functor_tag<ErrorReturn::None, Functor>::value == details::tag_return_void::value, "Functor must return void");
        typename details::functor_tag<ErrorReturn::None, Functor>::template functor_wrapper<Functor> functorObject(wistd::forward<Functor>(functor));

        wil::details::ResultFromExceptionDebug(diagnostics, SupportedExceptions::None, functorObject);
    }

    namespace details {

#endif  // WIL_ENABLE_EXCEPTIONS

        // Exception guard -- catch exceptions and log them (or handle them with a custom callback)
        // WARNING: may throw an exception...
        inline __declspec(noinline) HRESULT RunFunctor(IFunctor& functor, IFunctorHost& host)
        {
            if (g_pfnRunFunctorWithExceptionFilter)
            {
                return g_pfnRunFunctorWithExceptionFilter(functor, host, _ReturnAddress());
            }

            return host.Run(functor);
        }

        // Returns true if a debugger should be considered to be connected.
        // Modules can force this on through setting g_fIsDebuggerPresent explicitly (useful for live debugging),
        // they can provide a callback function by setting g_pfnIsDebuggerPresent (useful for kernel debbugging),
        // and finally the user-mode check (IsDebuggerPrsent) is checked. IsDebuggerPresent is a fast call
        inline bool IsDebuggerPresent()
        {
            return g_fIsDebuggerPresent || ((g_pfnIsDebuggerPresent != nullptr) ? g_pfnIsDebuggerPresent() : (::IsDebuggerPresent() != FALSE));
        }

        //*****************************************************************************
        // Shared Reporting -- all reporting macros bubble up through this codepath
        //*****************************************************************************

        inline void LogFailure(__R_FN_PARAMS_FULL, FailureType type, const ResultStatus& resultPair, _In_opt_ PCWSTR message,
            bool fWantDebugString, _Out_writes_(debugStringSizeChars) _Post_z_ PWSTR debugString, _Pre_satisfies_(debugStringSizeChars > 0) size_t debugStringSizeChars,
            _Out_writes_(callContextStringSizeChars) _Post_z_ PSTR callContextString, _Pre_satisfies_(callContextStringSizeChars > 0) size_t callContextStringSizeChars,
            _Out_ FailureInfo *failure) WI_NOEXCEPT
        {
            debugString[0] = L'\0';
            callContextString[0] = L'\0';

            static long volatile s_failureId = 0;

            failure->hr = resultPair.hr;
            failure->status = resultPair.status;

            int failureCount = 0;
            switch (type)
            {
            case FailureType::Exception:
                failureCount = RecordException(failure->hr);
                break;
            case FailureType::Return:
                failureCount = RecordReturn(failure->hr);
                break;
            case FailureType::Log:
                if (SUCCEEDED(failure->hr))
                {
                    // If you hit this assert (or are reviewing this failure telemetry), then most likely you are trying to log success
                    // using one of the WIL macros.  Example:
                    //      LOG_HR(S_OK);
                    // Instead, use one of the forms that conditionally logs based upon the error condition:
                    //      LOG_IF_FAILED(hr);

                    WI_USAGE_ERROR_FORWARD("CALLER BUG: Macro usage error detected.  Do not LOG_XXX success.");
                    failure->hr = __HRESULT_FROM_WIN32(ERROR_ASSERTION_FAILURE);
                    failure->status = wil::details::HrToNtStatus(failure->hr);
                }
                failureCount = RecordLog(failure->hr);
                break;
            case FailureType::FailFast:
                failureCount = RecordFailFast(failure->hr);
                break;
            };

            failure->type = type;
            failure->flags = FailureFlags::None;
            WI_SetFlagIf(failure->flags, FailureFlags::NtStatus, resultPair.kind == ResultStatus::Kind::NtStatus);
            failure->failureId = ::InterlockedIncrementNoFence(&s_failureId);
            failure->pszMessage = ((message != nullptr) && (message[0] != L'\0')) ? message : nullptr;
            failure->threadId = ::GetCurrentThreadId();
            failure->pszFile = fileName;
            failure->uLineNumber = lineNumber;
            failure->cFailureCount = failureCount;
            failure->pszCode = code;
            failure->pszFunction = functionName;
            failure->returnAddress = returnAddress;
            failure->callerReturnAddress = callerReturnAddress;
            failure->pszCallContext = nullptr;
            ::ZeroMemory(&failure->callContextCurrent, sizeof(failure->callContextCurrent));
            ::ZeroMemory(&failure->callContextOriginating, sizeof(failure->callContextOriginating));
            failure->pszModule = (g_pfnGetModuleName != nullptr) ? g_pfnGetModuleName() : nullptr;

            // Process failure notification / adjustments
            if (details::g_pfnNotifyFailure)
            {
                details::g_pfnNotifyFailure(failure);
            }

            // Completes filling out failure, notifies thread-based callbacks and the telemetry callback
            if (details::g_pfnGetContextAndNotifyFailure)
            {
                details::g_pfnGetContextAndNotifyFailure(failure, callContextString, callContextStringSizeChars);
            }

            // Allow hooks to inspect the failure before acting upon it
            if (details::g_pfnLoggingCallback)
            {
                details::g_pfnLoggingCallback(*failure);
            }

            // If the hook is enabled then it will be given the opportunity to call RoOriginateError to greatly improve the diagnostic experience
            // for uncaught exceptions.  In cases where we will be throwing a C++/CX Platform::Exception we should avoid originating because the
            // CX runtime will be doing that for us.  fWantDebugString is only set to true when the caller will be throwing a Platform::Exception.
            if (details::g_pfnOriginateCallback && !fWantDebugString && WI_IsFlagClear(failure->flags, FailureFlags::RequestSuppressTelemetry))
            {
                details::g_pfnOriginateCallback(*failure);
            }

            if (SUCCEEDED(failure->hr))
            {
                // Caller bug: Leaking a success code into a failure-only function
                FAIL_FAST_IMMEDIATE_IF(type != FailureType::FailFast);
                failure->hr = E_UNEXPECTED;
                failure->status = wil::details::HrToNtStatus(failure->hr);
            }

            bool const fUseOutputDebugString = IsDebuggerPresent() && g_fResultOutputDebugString && WI_IsFlagClear(failure->flags, FailureFlags::RequestSuppressTelemetry);

            // We need to generate the logging message if:
            // * We're logging to OutputDebugString
            // * OR the caller asked us to (generally for attaching to a C++/CX exception)
            if (fWantDebugString || fUseOutputDebugString)
            {
                // Call the logging callback (if present) to allow them to generate the debug string that will be pushed to the console
                // or the platform exception object if the caller desires it.
                if ((g_pfnResultLoggingCallback != nullptr) && !g_resultMessageCallbackSet)
                {
                    g_pfnResultLoggingCallback(failure, debugString, debugStringSizeChars);
                }

                // The callback only optionally needs to supply the debug string -- if the callback didn't populate it, yet we still want
                // it for OutputDebugString or exception message, then generate the default string.
                if (debugString[0] == L'\0')
                {
                    GetFailureLogString(debugString, debugStringSizeChars, *failure);
                }

                if (fUseOutputDebugString)
                {
                    ::OutputDebugStringW(debugString);
                }
            }
            else
            {
                // [deprecated behavior]
                // This callback was at one point *always* called for all failures, so we continue to call it for failures even when we don't
                // need to generate the debug string information (when the callback was supplied directly).  We can avoid this if the caller
                // used the explicit function (through g_resultMessageCallbackSet)
                if ((g_pfnResultLoggingCallback != nullptr) && !g_resultMessageCallbackSet)
                {
                    g_pfnResultLoggingCallback(failure, nullptr, 0);
                }
            }

            if ((WI_IsFlagSet(failure->flags, FailureFlags::RequestDebugBreak) || g_fBreakOnFailure) && (g_pfnDebugBreak != nullptr))
            {
                g_pfnDebugBreak();
            }
        }

        inline RESULT_NORETURN void __stdcall WilFailFast(const wil::FailureInfo& failure)
        {
            if (g_pfnWilFailFast)
            {
                g_pfnWilFailFast(failure);
            }

#ifdef RESULT_RAISE_FAST_FAIL_EXCEPTION
            // Use of this macro is an ODR violation - use the callback instead.  This will be removed soon.
            RESULT_RAISE_FAST_FAIL_EXCEPTION;
#endif

            // Before we fail fast in this method, give the [optional] RoFailFastWithErrorContext a try.
            if (g_pfnFailfastWithContextCallback)
            {
                g_pfnFailfastWithContextCallback(failure);
            }

            // parameter 0 is the !analyze code (FAST_FAIL_FATAL_APP_EXIT)
            EXCEPTION_RECORD er{};
            er.NumberParameters = 1;            // default to be safe, see below
            er.ExceptionCode = static_cast<DWORD>(STATUS_STACK_BUFFER_OVERRUN); // 0xC0000409
            er.ExceptionFlags = EXCEPTION_NONCONTINUABLE;
            er.ExceptionInformation[0] = FAST_FAIL_FATAL_APP_EXIT; // see winnt.h, generated from minkernel\published\base\ntrtl_x.w
            if (failure.returnAddress == nullptr)                     // FailureInfo does not have _ReturnAddress, have RaiseFailFastException generate it
            {
                // passing ExceptionCode 0xC0000409 and one param with FAST_FAIL_APP_EXIT will use existing
                // !analyze functionality to crawl the stack looking for the HRESULT
                // don't pass a 0 HRESULT in param 1 because that will result in worse bucketing.
                WilRaiseFailFastException(&er, nullptr, FAIL_FAST_GENERATE_EXCEPTION_ADDRESS);
            }
            else                                                // use FailureInfo caller address
            {
                // parameter 1 is the failing HRESULT
                // parameter 2 is the line number.  This is never used for bucketing (due to code churn causing re-bucketing) but is available in the dump's
                // exception record to aid in failure locality. Putting it here prevents it from being poisoned in triage dumps.
                er.NumberParameters = 3;
                er.ExceptionInformation[1] = failure.hr;
                er.ExceptionInformation[2] = failure.uLineNumber;
                er.ExceptionAddress = failure.returnAddress;
                WilRaiseFailFastException(&er, nullptr, 0 /* do not generate exception address */);
            }
        }

        template<FailureType T>
        inline __declspec(noinline) void ReportFailure_Return(__R_FN_PARAMS_FULL, const ResultStatus& resultPair, PCWSTR message, ReportFailureOptions options)
        {
            bool needPlatformException = ((T == FailureType::Exception) &&
                WI_IsFlagClear(options, ReportFailureOptions::MayRethrow) &&
                (g_pfnThrowPlatformException != nullptr) &&
                (g_fResultThrowPlatformException || WI_IsFlagSet(options, ReportFailureOptions::ForcePlatformException)));

            FailureInfo failure;
            wchar_t debugString[2048];
            char callContextString[1024];

            LogFailure(__R_FN_CALL_FULL, T, resultPair, message, needPlatformException,
                debugString, ARRAYSIZE(debugString), callContextString, ARRAYSIZE(callContextString), &failure);

            if (WI_IsFlagSet(failure.flags, FailureFlags::RequestFailFast))
            {
                WilFailFast(failure);
            }
        }

        template<FailureType T, bool SuppressAction>
        inline __declspec(noinline) void ReportFailure_Base(__R_FN_PARAMS_FULL, const ResultStatus& resultPair, PCWSTR message, ReportFailureOptions options)
        {
            ReportFailure_Return<T>(__R_FN_CALL_FULL, resultPair, message, options);
        }

        template<FailureType T>
        inline __declspec(noinline) RESULT_NORETURN void ReportFailure_NoReturn(__R_FN_PARAMS_FULL, const ResultStatus& resultPair, PCWSTR message, ReportFailureOptions options)
        {
            bool needPlatformException = ((T == FailureType::Exception) &&
                WI_IsFlagClear(options, ReportFailureOptions::MayRethrow) &&
                (g_pfnThrowPlatformException != nullptr) &&
                (g_fResultThrowPlatformException || WI_IsFlagSet(options, ReportFailureOptions::ForcePlatformException)));

            FailureInfo failure;
            wchar_t debugString[2048];
            char callContextString[1024];

            LogFailure(__R_FN_CALL_FULL, T, resultPair, message, needPlatformException,
                debugString, ARRAYSIZE(debugString), callContextString, ARRAYSIZE(callContextString), &failure);
__WI_SUPPRESS_4127_S
            if ((T == FailureType::FailFast) || WI_IsFlagSet(failure.flags, FailureFlags::RequestFailFast))
            {
                WilFailFast(const_cast<FailureInfo&>(failure));
            }
            else
            {
                if (needPlatformException)
                {
                    g_pfnThrowPlatformException(failure, debugString);
                }

                if (WI_IsFlagSet(options, ReportFailureOptions::MayRethrow))
                {
                    RethrowCaughtException();
                }

                ThrowResultException(failure);

                // Wil was instructed to throw, but doesn't have any capability to do so (global function pointers are not setup)
                WilFailFast(const_cast<FailureInfo&>(failure));
            }
__WI_SUPPRESS_4127_E
        }

        template<>
        inline __declspec(noinline) RESULT_NORETURN void ReportFailure_Base<FailureType::FailFast, false>(__R_FN_PARAMS_FULL, const ResultStatus& resultPair, PCWSTR message, ReportFailureOptions options)
        {
            ReportFailure_NoReturn<FailureType::FailFast>(__R_FN_CALL_FULL, resultPair, message, options);
        }

        template<>
        inline __declspec(noinline) RESULT_NORETURN void ReportFailure_Base<FailureType::Exception, false>(__R_FN_PARAMS_FULL, const ResultStatus& resultPair, PCWSTR message, ReportFailureOptions options)
        {
            ReportFailure_NoReturn<FailureType::Exception>(__R_FN_CALL_FULL, resultPair, message, options);
        }

        __declspec(noinline) inline void ReportFailure(__R_FN_PARAMS_FULL, FailureType type, const ResultStatus& resultPair, _In_opt_ PCWSTR message, ReportFailureOptions options)
        {
            switch(type)
            {
            case FailureType::Exception:
                ReportFailure_Base<FailureType::Exception>(__R_FN_CALL_FULL, resultPair, message, options);
                break;
            case FailureType::FailFast:
                ReportFailure_Base<FailureType::FailFast>(__R_FN_CALL_FULL, resultPair, message, options);
                break;
            case FailureType::Log:
                ReportFailure_Base<FailureType::Log>(__R_FN_CALL_FULL, resultPair, message, options);
                break;
            case FailureType::Return:
                ReportFailure_Base<FailureType::Return>(__R_FN_CALL_FULL, resultPair, message, options);
                break;
            }
        }

        template<FailureType T>
        inline ResultStatus ReportFailure_CaughtExceptionCommon(__R_FN_PARAMS_FULL, _Inout_updates_(debugStringChars) PWSTR debugString, _Pre_satisfies_(debugStringChars > 0) size_t debugStringChars, SupportedExceptions supported)
        {
            bool isNormalized = false;
            auto length = wcslen(debugString);
            WI_ASSERT(length < debugStringChars);
            ResultStatus resultPair;
            if (details::g_pfnResultFromCaughtExceptionInternal)
            {
                resultPair = details::g_pfnResultFromCaughtExceptionInternal(debugString + length, debugStringChars - length, &isNormalized);
            }

            const bool known = (FAILED(resultPair.hr));
            if (!known)
            {
                resultPair = ResultStatus::FromResult(__HRESULT_FROM_WIN32(ERROR_UNHANDLED_EXCEPTION));
            }

            ReportFailureOptions options = ReportFailureOptions::ForcePlatformException;
            WI_SetFlagIf(options, ReportFailureOptions::MayRethrow, isNormalized);

            if ((supported == SupportedExceptions::None) ||
                ((supported == SupportedExceptions::Known) && !known) ||
                ((supported == SupportedExceptions::Thrown) && !isNormalized) ||
                ((supported == SupportedExceptions::Default) && !known && g_fResultFailFastUnknownExceptions))
            {
                // By default WIL will issue a fail fast for unrecognized exception types.  Wil recognizes any std::exception or wil::ResultException based
                // types and Platform::Exception^, so there aren't too many valid exception types which could cause this.  Those that are valid, should be handled
                // by remapping the exception callback.  Those that are not valid should be found and fixed (meaningless accidents like 'throw hr;').
                // The caller may also be requesting non-default behavior to fail-fast more frequently (primarily for debugging unknown exceptions).
                ReportFailure_Base<FailureType::FailFast>(__R_FN_CALL_FULL, resultPair, debugString, options);
            }
            else
            {
                ReportFailure_Base<T>(__R_FN_CALL_FULL, resultPair, debugString, options);
            }

            return resultPair;
        }

        template<FailureType T>
        inline ResultStatus RESULT_NORETURN ReportFailure_CaughtExceptionCommonNoReturnBase(__R_FN_PARAMS_FULL, _Inout_updates_(debugStringChars) PWSTR debugString, _Pre_satisfies_(debugStringChars > 0) size_t debugStringChars, SupportedExceptions supported)
        {
            bool isNormalized = false;
            const auto length = wcslen(debugString);
            WI_ASSERT(length < debugStringChars);
            ResultStatus resultPair;
            if (details::g_pfnResultFromCaughtExceptionInternal)
            {
                resultPair = details::g_pfnResultFromCaughtExceptionInternal(debugString + length, debugStringChars - length, &isNormalized);
            }

            const bool known = (FAILED(resultPair.hr));
            if (!known)
            {
                resultPair = ResultStatus::FromResult(__HRESULT_FROM_WIN32(ERROR_UNHANDLED_EXCEPTION));
            }

            ReportFailureOptions options = ReportFailureOptions::ForcePlatformException;
            WI_SetFlagIf(options, ReportFailureOptions::MayRethrow, isNormalized);

            if ((supported == SupportedExceptions::None) ||
                ((supported == SupportedExceptions::Known) && !known) ||
                ((supported == SupportedExceptions::Thrown) && !isNormalized) ||
                ((supported == SupportedExceptions::Default) && !known && g_fResultFailFastUnknownExceptions))
            {
                // By default WIL will issue a fail fast for unrecognized exception types.  Wil recognizes any std::exception or wil::ResultException based
                // types and Platform::Exception^, so there aren't too many valid exception types which could cause this.  Those that are valid, should be handled
                // by remapping the exception callback.  Those that are not valid should be found and fixed (meaningless accidents like 'throw hr;').
                // The caller may also be requesting non-default behavior to fail-fast more frequently (primarily for debugging unknown exceptions).
                ReportFailure_Base<FailureType::FailFast>(__R_FN_CALL_FULL, resultPair, debugString, options);
            }
            else
            {
                ReportFailure_Base<T>(__R_FN_CALL_FULL, resultPair, debugString, options);
            }

            RESULT_NORETURN_RESULT(resultPair);
        }

        template<>
        inline RESULT_NORETURN ResultStatus ReportFailure_CaughtExceptionCommon<FailureType::FailFast>(__R_FN_PARAMS_FULL, _Inout_updates_(debugStringChars) PWSTR debugString, _Pre_satisfies_(debugStringChars > 0) size_t debugStringChars, SupportedExceptions supported)
        {
            RESULT_NORETURN_RESULT(ReportFailure_CaughtExceptionCommonNoReturnBase<FailureType::FailFast>(__R_FN_CALL_FULL, debugString, debugStringChars, supported));
        }

        template<>
        inline RESULT_NORETURN ResultStatus ReportFailure_CaughtExceptionCommon<FailureType::Exception>(__R_FN_PARAMS_FULL, _Inout_updates_(debugStringChars) PWSTR debugString, _Pre_satisfies_(debugStringChars > 0) size_t debugStringChars, SupportedExceptions supported)
        {
            RESULT_NORETURN_RESULT(ReportFailure_CaughtExceptionCommonNoReturnBase<FailureType::Exception>(__R_FN_CALL_FULL, debugString, debugStringChars, supported));
        }

        template<FailureType T>
        inline void ReportFailure_Msg(__R_FN_PARAMS_FULL, const ResultStatus& resultPair, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            wchar_t message[2048];
            PrintLoggingMessage(message, ARRAYSIZE(message), formatString, argList);
            ReportFailure_Base<T>(__R_FN_CALL_FULL, resultPair, message);
        }

        template<>
        inline RESULT_NORETURN void ReportFailure_Msg<FailureType::FailFast>(__R_FN_PARAMS_FULL, const ResultStatus& resultPair, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            wchar_t message[2048];
            PrintLoggingMessage(message, ARRAYSIZE(message), formatString, argList);
            ReportFailure_Base<FailureType::FailFast>(__R_FN_CALL_FULL, resultPair, message);
        }

        template<>
        inline RESULT_NORETURN void ReportFailure_Msg<FailureType::Exception>(__R_FN_PARAMS_FULL, const ResultStatus& resultPair, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            wchar_t message[2048];
            PrintLoggingMessage(message, ARRAYSIZE(message), formatString, argList);
            ReportFailure_Base<FailureType::Exception>(__R_FN_CALL_FULL, resultPair, message);
        }

        template <FailureType T>
        inline void ReportFailure_ReplaceMsg(__R_FN_PARAMS_FULL, HRESULT hr, PCSTR formatString, ...)
        {
            va_list argList;
            va_start(argList, formatString);
            ReportFailure_Msg<T>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr), formatString, argList);
        }

        template<FailureType T>
        __declspec(noinline) inline void ReportFailure_Hr(__R_FN_PARAMS_FULL, HRESULT hr)
        {
            ReportFailure_Base<T>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr));
        }

        template<>
        __declspec(noinline) inline RESULT_NORETURN void ReportFailure_Hr<FailureType::FailFast>(__R_FN_PARAMS_FULL, HRESULT hr)
        {
            ReportFailure_Base<FailureType::FailFast>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr));
        }

        template<>
        __declspec(noinline) inline RESULT_NORETURN void ReportFailure_Hr<FailureType::Exception>(__R_FN_PARAMS_FULL, HRESULT hr)
        {
            ReportFailure_Base<FailureType::Exception>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr));
        }

        __declspec(noinline) inline void ReportFailure_Hr(__R_FN_PARAMS_FULL, FailureType type, HRESULT hr)
        {
            switch(type)
            {
            case FailureType::Exception:
                ReportFailure_Hr<FailureType::Exception>(__R_FN_CALL_FULL, hr);
                break;
            case FailureType::FailFast:
                ReportFailure_Hr<FailureType::FailFast>(__R_FN_CALL_FULL, hr);
                break;
            case FailureType::Log:
                ReportFailure_Hr<FailureType::Log>(__R_FN_CALL_FULL, hr);
                break;
            case FailureType::Return:
                ReportFailure_Hr<FailureType::Return>(__R_FN_CALL_FULL, hr);
                break;
            }
        }

        template<FailureType T>
        _Success_(true)
        _Translates_Win32_to_HRESULT_(err)
        __declspec(noinline) inline HRESULT ReportFailure_Win32(__R_FN_PARAMS_FULL, DWORD err)
        {
            const auto hr = __HRESULT_FROM_WIN32(err);
            ReportFailure_Base<T>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr));
            return hr;
        }

        template<>
        _Success_(true)
        _Translates_Win32_to_HRESULT_(err)
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_Win32<FailureType::FailFast>(__R_FN_PARAMS_FULL, DWORD err)
        {
            const auto hr = __HRESULT_FROM_WIN32(err);
            ReportFailure_Base<FailureType::FailFast>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr));
            RESULT_NORETURN_RESULT(hr);
        }

        template<>
        _Success_(true)
        _Translates_Win32_to_HRESULT_(err)
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_Win32<FailureType::Exception>(__R_FN_PARAMS_FULL, DWORD err)
        {
            const auto hr = __HRESULT_FROM_WIN32(err);
            ReportFailure_Base<FailureType::Exception>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr));
            RESULT_NORETURN_RESULT(hr);
        }

        template<FailureType T>
        __declspec(noinline) inline DWORD ReportFailure_GetLastError(__R_FN_PARAMS_FULL)
        {
            const auto err = GetLastErrorFail(__R_FN_CALL_FULL);
            const auto hr = __HRESULT_FROM_WIN32(err);
            ReportFailure_Base<T>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr));
            ::SetLastError(err);
            return err;
        }

        template<>
        __declspec(noinline) inline RESULT_NORETURN DWORD ReportFailure_GetLastError<FailureType::FailFast>(__R_FN_PARAMS_FULL)
        {
            const auto err = GetLastErrorFail(__R_FN_CALL_FULL);
            const auto hr = __HRESULT_FROM_WIN32(err);
            ReportFailure_Base<FailureType::FailFast>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr));
            RESULT_NORETURN_RESULT(err);
        }

        template<>
        __declspec(noinline) inline RESULT_NORETURN DWORD ReportFailure_GetLastError<FailureType::Exception>(__R_FN_PARAMS_FULL)
        {
            const auto err = GetLastErrorFail(__R_FN_CALL_FULL);
            const auto hr = __HRESULT_FROM_WIN32(err);
            ReportFailure_Base<FailureType::Exception>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr));
            RESULT_NORETURN_RESULT(err);
        }

        template<FailureType T>
        _Success_(true)
        _Translates_last_error_to_HRESULT_
        __declspec(noinline) inline HRESULT ReportFailure_GetLastErrorHr(__R_FN_PARAMS_FULL)
        {
            const auto hr = GetLastErrorFailHr(__R_FN_CALL_FULL);
            ReportFailure_Base<T>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr));
            return hr;
        }

        template<>
        _Success_(true)
        _Translates_last_error_to_HRESULT_
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_GetLastErrorHr<FailureType::FailFast>(__R_FN_PARAMS_FULL)
        {
            const auto hr = GetLastErrorFailHr(__R_FN_CALL_FULL);
            ReportFailure_Base<FailureType::FailFast>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr));
            RESULT_NORETURN_RESULT(hr);
        }

        template<>
        _Success_(true)
        _Translates_last_error_to_HRESULT_
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_GetLastErrorHr<FailureType::Exception>(__R_FN_PARAMS_FULL)
        {
            const auto hr = GetLastErrorFailHr(__R_FN_CALL_FULL);
            ReportFailure_Base<FailureType::Exception>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr));
            RESULT_NORETURN_RESULT(hr);
        }

        template<FailureType T>
        _Success_(true)
        _Translates_NTSTATUS_to_HRESULT_(status)
        __declspec(noinline) inline HRESULT ReportFailure_NtStatus(__R_FN_PARAMS_FULL, NTSTATUS status)
        {
            const auto resultPair = ResultStatus::FromStatus(status);
            ReportFailure_Base<T>(__R_FN_CALL_FULL, resultPair);
            return resultPair.hr;
        }

        template<>
        _Success_(true)
        _Translates_NTSTATUS_to_HRESULT_(status)
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_NtStatus<FailureType::FailFast>(__R_FN_PARAMS_FULL, NTSTATUS status)
        {
            const auto resultPair = ResultStatus::FromStatus(status);
            ReportFailure_Base<FailureType::FailFast>(__R_FN_CALL_FULL, resultPair);
            RESULT_NORETURN_RESULT(resultPair.hr);
        }

        template<>
        _Success_(true)
        _Translates_NTSTATUS_to_HRESULT_(status)
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_NtStatus<FailureType::Exception>(__R_FN_PARAMS_FULL, NTSTATUS status)
        {
            const auto resultPair = ResultStatus::FromStatus(status);
            ReportFailure_Base<FailureType::Exception>(__R_FN_CALL_FULL, resultPair);
            RESULT_NORETURN_RESULT(resultPair.hr);
        }

        template<FailureType T>
        __declspec(noinline) inline HRESULT ReportFailure_CaughtException(__R_FN_PARAMS_FULL, SupportedExceptions supported)
        {
            wchar_t message[2048]{};
            message[0] = L'\0';
            return ReportFailure_CaughtExceptionCommon<T>(__R_FN_CALL_FULL, message, ARRAYSIZE(message), supported).hr;
        }

        template<>
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_CaughtException<FailureType::FailFast>(__R_FN_PARAMS_FULL, SupportedExceptions supported)
        {
            wchar_t message[2048]{};
            message[0] = L'\0';
            RESULT_NORETURN_RESULT(ReportFailure_CaughtExceptionCommon<FailureType::FailFast>(__R_FN_CALL_FULL, message, ARRAYSIZE(message), supported).hr);
        }

        template<>
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_CaughtException<FailureType::Exception>(__R_FN_PARAMS_FULL, SupportedExceptions supported)
        {
            wchar_t message[2048]{};
            message[0] = L'\0';
            RESULT_NORETURN_RESULT(ReportFailure_CaughtExceptionCommon<FailureType::Exception>(__R_FN_CALL_FULL, message, ARRAYSIZE(message), supported).hr);
        }

        template<FailureType T>
        __declspec(noinline) inline void ReportFailure_HrMsg(__R_FN_PARAMS_FULL, HRESULT hr, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            ReportFailure_Msg<T>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr), formatString, argList);
        }

        template<>
        __declspec(noinline) inline RESULT_NORETURN void ReportFailure_HrMsg<FailureType::FailFast>(__R_FN_PARAMS_FULL, HRESULT hr, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            ReportFailure_Msg<FailureType::FailFast>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr), formatString, argList);
        }

        template<>
        __declspec(noinline) inline RESULT_NORETURN void ReportFailure_HrMsg<FailureType::Exception>(__R_FN_PARAMS_FULL, HRESULT hr, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            ReportFailure_Msg<FailureType::Exception>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr), formatString, argList);
        }

        template<FailureType T>
        _Success_(true)
        _Translates_Win32_to_HRESULT_(err)
        __declspec(noinline) inline HRESULT ReportFailure_Win32Msg(__R_FN_PARAMS_FULL, DWORD err, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            auto hr = __HRESULT_FROM_WIN32(err);
            ReportFailure_Msg<T>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr), formatString, argList);
            return hr;
        }

        template<>
        _Success_(true)
        _Translates_Win32_to_HRESULT_(err)
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_Win32Msg<FailureType::FailFast>(__R_FN_PARAMS_FULL, DWORD err, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            auto hr = __HRESULT_FROM_WIN32(err);
            ReportFailure_Msg<FailureType::FailFast>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr), formatString, argList);
            RESULT_NORETURN_RESULT(hr);
        }

        template<>
        _Success_(true)
        _Translates_Win32_to_HRESULT_(err)
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_Win32Msg<FailureType::Exception>(__R_FN_PARAMS_FULL, DWORD err, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            auto hr = __HRESULT_FROM_WIN32(err);
            ReportFailure_Msg<FailureType::Exception>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr), formatString, argList);
            RESULT_NORETURN_RESULT(hr);
        }

        template<FailureType T>
        __declspec(noinline) inline DWORD ReportFailure_GetLastErrorMsg(__R_FN_PARAMS_FULL, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            auto err = GetLastErrorFail(__R_FN_CALL_FULL);
            auto hr = __HRESULT_FROM_WIN32(err);
            ReportFailure_Msg<T>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr), formatString, argList);
            ::SetLastError(err);
            return err;
        }

        template<>
        __declspec(noinline) inline RESULT_NORETURN DWORD ReportFailure_GetLastErrorMsg<FailureType::FailFast>(__R_FN_PARAMS_FULL, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            auto err = GetLastErrorFail(__R_FN_CALL_FULL);
            auto hr = __HRESULT_FROM_WIN32(err);
            ReportFailure_Msg<FailureType::FailFast>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr), formatString, argList);
            RESULT_NORETURN_RESULT(err);
        }

        template<>
        __declspec(noinline) inline RESULT_NORETURN DWORD ReportFailure_GetLastErrorMsg<FailureType::Exception>(__R_FN_PARAMS_FULL, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            auto err = GetLastErrorFail(__R_FN_CALL_FULL);
            auto hr = __HRESULT_FROM_WIN32(err);
            ReportFailure_Msg<FailureType::Exception>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr), formatString, argList);
            RESULT_NORETURN_RESULT(err);
        }

        template<FailureType T>
        _Success_(true)
        _Translates_last_error_to_HRESULT_
        __declspec(noinline) inline HRESULT ReportFailure_GetLastErrorHrMsg(__R_FN_PARAMS_FULL, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            auto hr = GetLastErrorFailHr(__R_FN_CALL_FULL);
            ReportFailure_Msg<T>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr), formatString, argList);
            return hr;
        }

        template<>
        _Success_(true)
        _Translates_last_error_to_HRESULT_
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_GetLastErrorHrMsg<FailureType::FailFast>(__R_FN_PARAMS_FULL, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            auto hr = GetLastErrorFailHr(__R_FN_CALL_FULL);
            ReportFailure_Msg<FailureType::FailFast>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr), formatString, argList);
            RESULT_NORETURN_RESULT(hr);
        }

        template<>
        _Success_(true)
        _Translates_last_error_to_HRESULT_
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_GetLastErrorHrMsg<FailureType::Exception>(__R_FN_PARAMS_FULL, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            auto hr = GetLastErrorFailHr(__R_FN_CALL_FULL);
            ReportFailure_Msg<FailureType::Exception>(__R_FN_CALL_FULL, ResultStatus::FromResult(hr), formatString, argList);
            RESULT_NORETURN_RESULT(hr);
        }

        template<FailureType T>
        _Success_(true)
        _Translates_NTSTATUS_to_HRESULT_(status)
        __declspec(noinline) inline HRESULT ReportFailure_NtStatusMsg(__R_FN_PARAMS_FULL, NTSTATUS status, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            const auto resultPair = ResultStatus::FromStatus(status);
            ReportFailure_Msg<T>(__R_FN_CALL_FULL, resultPair, formatString, argList);
            return resultPair.hr;
        }

        template<>
        _Success_(true)
        _Translates_NTSTATUS_to_HRESULT_(status)
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_NtStatusMsg<FailureType::FailFast>(__R_FN_PARAMS_FULL, NTSTATUS status, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            const auto resultPair = ResultStatus::FromStatus(status);
            ReportFailure_Msg<FailureType::FailFast>(__R_FN_CALL_FULL, resultPair, formatString, argList);
            RESULT_NORETURN_RESULT(resultPair.hr);
        }

        template<>
        _Success_(true)
        _Translates_NTSTATUS_to_HRESULT_(status)
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_NtStatusMsg<FailureType::Exception>(__R_FN_PARAMS_FULL, NTSTATUS status, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            const auto resultPair = ResultStatus::FromStatus(status);
            ReportFailure_Msg<FailureType::Exception>(__R_FN_CALL_FULL, resultPair, formatString, argList);
            RESULT_NORETURN_RESULT(resultPair.hr);
        }

        template<FailureType T>
        __declspec(noinline) inline HRESULT ReportFailure_CaughtExceptionMsg(__R_FN_PARAMS_FULL, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            // Pre-populate the buffer with our message, the exception message will be added to it...
            wchar_t message[2048];
            PrintLoggingMessage(message, ARRAYSIZE(message), formatString, argList);
            StringCchCatW(message, ARRAYSIZE(message), L" -- ");
            return ReportFailure_CaughtExceptionCommon<T>(__R_FN_CALL_FULL, message, ARRAYSIZE(message), SupportedExceptions::Default).hr;
        }

        template<>
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_CaughtExceptionMsg<FailureType::FailFast>(__R_FN_PARAMS_FULL, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            // Pre-populate the buffer with our message, the exception message will be added to it...
            wchar_t message[2048];
            PrintLoggingMessage(message, ARRAYSIZE(message), formatString, argList);
            StringCchCatW(message, ARRAYSIZE(message), L" -- ");
            RESULT_NORETURN_RESULT(ReportFailure_CaughtExceptionCommon<FailureType::FailFast>(__R_FN_CALL_FULL, message, ARRAYSIZE(message), SupportedExceptions::Default).hr);
        }

        template<>
        __declspec(noinline) inline RESULT_NORETURN HRESULT ReportFailure_CaughtExceptionMsg<FailureType::Exception>(__R_FN_PARAMS_FULL, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            // Pre-populate the buffer with our message, the exception message will be added to it...
            wchar_t message[2048];
            PrintLoggingMessage(message, ARRAYSIZE(message), formatString, argList);
            StringCchCatW(message, ARRAYSIZE(message), L" -- ");
            RESULT_NORETURN_RESULT(ReportFailure_CaughtExceptionCommon<FailureType::Exception>(__R_FN_CALL_FULL, message, ARRAYSIZE(message), SupportedExceptions::Default).hr);
        }


        //*****************************************************************************
        // Support for throwing custom exception types
        //*****************************************************************************

#ifdef WIL_ENABLE_EXCEPTIONS
        inline HRESULT GetErrorCode(_In_ ResultException &exception) WI_NOEXCEPT
        {
            return exception.GetErrorCode();
        }

        inline void SetFailureInfo(_In_ FailureInfo const &failure, _Inout_ ResultException &exception) WI_NOEXCEPT
        {
            return exception.SetFailureInfo(failure);
        }

#ifdef __cplusplus_winrt
        inline HRESULT GetErrorCode(_In_ Platform::Exception^ exception) WI_NOEXCEPT
        {
            return exception->HResult;
        }

        inline void SetFailureInfo(_In_ FailureInfo const &, _Inout_ Platform::Exception^ exception) WI_NOEXCEPT
        {
            // no-op -- once a PlatformException^ is created, we can't modify the message, but this function must
            // exist to distinguish this from ResultException
        }
#endif

        template <typename T>
        RESULT_NORETURN inline void ReportFailure_CustomExceptionHelper(_Inout_ T &exception, __R_FN_PARAMS_FULL, _In_opt_ PCWSTR message = nullptr)
        {
            // When seeing the error: "cannot convert parameter 1 from 'XXX' to 'wil::ResultException &'"
            // Custom exceptions must be based upon either ResultException or Platform::Exception^ to be used with ResultException.h.
            // This compilation error indicates an attempt to throw an incompatible exception type.
            const HRESULT hr = GetErrorCode(exception);

            FailureInfo failure;
            wchar_t debugString[2048];
            char callContextString[1024];

            LogFailure(__R_FN_CALL_FULL, FailureType::Exception, ResultStatus::FromResult(hr), message, false,     // false = does not need debug string
                       debugString, ARRAYSIZE(debugString), callContextString, ARRAYSIZE(callContextString), &failure);

            if (WI_IsFlagSet(failure.flags, FailureFlags::RequestFailFast))
            {
                WilFailFast(failure);
            }

            // push the failure info context into the custom exception class
            SetFailureInfo(failure, exception);

            throw exception;
        }

        template <typename T>
        __declspec(noreturn, noinline) inline void ReportFailure_CustomException(__R_FN_PARAMS _In_ T exception)
        {
            __R_FN_LOCALS_RA;
            ReportFailure_CustomExceptionHelper(exception, __R_FN_CALL_FULL);
        }

        template <typename T>
        __declspec(noreturn, noinline) inline void ReportFailure_CustomExceptionMsg(__R_FN_PARAMS _In_ T exception, _In_ _Printf_format_string_ PCSTR formatString, ...)
        {
            va_list argList;
            va_start(argList, formatString);
            wchar_t message[2048];
            PrintLoggingMessage(message, ARRAYSIZE(message), formatString, argList);

            __R_FN_LOCALS_RA;
            ReportFailure_CustomExceptionHelper(exception, __R_FN_CALL_FULL, message);
        }
#endif

        namespace __R_NS_NAME
        {
            //*****************************************************************************
            // Return Macros
            //*****************************************************************************

            __R_DIRECT_METHOD(void, Return_Hr)(__R_DIRECT_FN_PARAMS HRESULT hr) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_Hr<FailureType::Return>(__R_DIRECT_FN_CALL hr);
            }

            _Success_(true)
            _Translates_Win32_to_HRESULT_(err)
            __R_DIRECT_METHOD(HRESULT, Return_Win32)(__R_DIRECT_FN_PARAMS DWORD err) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                return wil::details::ReportFailure_Win32<FailureType::Return>(__R_DIRECT_FN_CALL err);
            }

            _Success_(true)
            _Translates_last_error_to_HRESULT_
            __R_DIRECT_METHOD(HRESULT, Return_GetLastError)(__R_DIRECT_FN_PARAMS_ONLY) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                return wil::details::ReportFailure_GetLastErrorHr<FailureType::Return>(__R_DIRECT_FN_CALL_ONLY);
            }

            _Success_(true)
            _Translates_NTSTATUS_to_HRESULT_(status)
            __R_DIRECT_METHOD(HRESULT, Return_NtStatus)(__R_DIRECT_FN_PARAMS NTSTATUS status) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                return wil::details::ReportFailure_NtStatus<FailureType::Return>(__R_DIRECT_FN_CALL status);
            }

#ifdef WIL_ENABLE_EXCEPTIONS
            __R_DIRECT_METHOD(HRESULT, Return_CaughtException)(__R_DIRECT_FN_PARAMS_ONLY) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                return wil::details::ReportFailure_CaughtException<FailureType::Return>(__R_DIRECT_FN_CALL_ONLY);
            }
#endif

            __R_DIRECT_METHOD(void, Return_HrMsg)(__R_DIRECT_FN_PARAMS HRESULT hr, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                wil::details::ReportFailure_HrMsg<FailureType::Return>(__R_DIRECT_FN_CALL hr, formatString, argList);
            }

            _Success_(true)
            _Translates_Win32_to_HRESULT_(err)
            __R_DIRECT_METHOD(HRESULT, Return_Win32Msg)(__R_DIRECT_FN_PARAMS DWORD err, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                return wil::details::ReportFailure_Win32Msg<FailureType::Return>(__R_DIRECT_FN_CALL err, formatString, argList);
            }

            _Success_(true)
            _Translates_last_error_to_HRESULT_
            __R_DIRECT_METHOD(HRESULT, Return_GetLastErrorMsg)(__R_DIRECT_FN_PARAMS _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                return wil::details::ReportFailure_GetLastErrorHrMsg<FailureType::Return>(__R_DIRECT_FN_CALL formatString, argList);
            }

            _Success_(true)
            _Translates_NTSTATUS_to_HRESULT_(status)
            __R_DIRECT_METHOD(HRESULT, Return_NtStatusMsg)(__R_DIRECT_FN_PARAMS NTSTATUS status, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                return wil::details::ReportFailure_NtStatusMsg<FailureType::Return>(__R_DIRECT_FN_CALL status, formatString, argList);
            }

#ifdef WIL_ENABLE_EXCEPTIONS
            __R_DIRECT_METHOD(HRESULT, Return_CaughtExceptionMsg)(__R_DIRECT_FN_PARAMS _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                return wil::details::ReportFailure_CaughtExceptionMsg<FailureType::Return>(__R_DIRECT_FN_CALL formatString, argList);
            }
#endif

            //*****************************************************************************
            // Log Macros
            //*****************************************************************************

            _Post_satisfies_(return == hr)
            __R_DIRECT_METHOD(HRESULT, Log_Hr)(__R_DIRECT_FN_PARAMS HRESULT hr) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_Hr<FailureType::Log>(__R_DIRECT_FN_CALL hr);
                return hr;
            }

            _Post_satisfies_(return == err)
            __R_DIRECT_METHOD(DWORD, Log_Win32)(__R_DIRECT_FN_PARAMS DWORD err) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_Win32<FailureType::Log>(__R_DIRECT_FN_CALL err);
                return err;
            }

            __R_DIRECT_METHOD(DWORD, Log_GetLastError)(__R_DIRECT_FN_PARAMS_ONLY) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                return wil::details::ReportFailure_GetLastError<FailureType::Log>(__R_DIRECT_FN_CALL_ONLY);
            }

            _Post_satisfies_(return == status)
            __R_DIRECT_METHOD(NTSTATUS, Log_NtStatus)(__R_DIRECT_FN_PARAMS NTSTATUS status) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_NtStatus<FailureType::Log>(__R_DIRECT_FN_CALL status);
                return status;
            }

#ifdef WIL_ENABLE_EXCEPTIONS
            __R_DIRECT_METHOD(HRESULT, Log_CaughtException)(__R_DIRECT_FN_PARAMS_ONLY) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                return wil::details::ReportFailure_CaughtException<FailureType::Log>(__R_DIRECT_FN_CALL_ONLY);
            }
#endif

            __R_INTERNAL_METHOD(_Log_Hr)(__R_INTERNAL_FN_PARAMS HRESULT hr) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_Hr<FailureType::Log>(__R_INTERNAL_FN_CALL hr);
            }

            __R_INTERNAL_METHOD(_Log_GetLastError)(__R_INTERNAL_FN_PARAMS_ONLY) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_GetLastError<FailureType::Log>(__R_INTERNAL_FN_CALL_ONLY);
            }

            __R_INTERNAL_METHOD(_Log_Win32)(__R_INTERNAL_FN_PARAMS DWORD err) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_Win32<FailureType::Log>(__R_INTERNAL_FN_CALL err);
            }

            __R_INTERNAL_METHOD(_Log_NullAlloc)(__R_INTERNAL_FN_PARAMS_ONLY) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_Hr<FailureType::Log>(__R_INTERNAL_FN_CALL E_OUTOFMEMORY);
            }

            __R_INTERNAL_METHOD(_Log_NtStatus)(__R_INTERNAL_FN_PARAMS NTSTATUS status) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_NtStatus<FailureType::Log>(__R_INTERNAL_FN_CALL status);
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            _Post_satisfies_(return == hr)
            __R_CONDITIONAL_METHOD(HRESULT, Log_IfFailed)(__R_CONDITIONAL_FN_PARAMS HRESULT hr)
            {
                if (FAILED(hr))
                {
                    __R_CALL_INTERNAL_METHOD(_Log_Hr)(__R_CONDITIONAL_FN_CALL hr);
                }
                return hr;
            }

            _Post_satisfies_(return == hr)
            __R_CONDITIONAL_NOINLINE_METHOD(HRESULT, Log_IfFailedWithExpected)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, unsigned int expectedCount, ...) WI_NOEXCEPT
            {
                va_list args;
                va_start(args, expectedCount);

                if (FAILED(hr))
                {
                    unsigned int expectedIndex;
                    for (expectedIndex = 0; expectedIndex < expectedCount; ++expectedIndex)
                    {
                        if (hr == va_arg(args, HRESULT))
                        {
                            break;
                        }
                    }

                    if (expectedIndex == expectedCount)
                    {
                        __R_CALL_INTERNAL_METHOD(_Log_Hr)(__R_CONDITIONAL_FN_CALL hr);
                    }
                }

                va_end(args);
                return hr;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            _Post_satisfies_(return == ret)
            __R_CONDITIONAL_METHOD(BOOL, Log_IfWin32BoolFalse)(__R_CONDITIONAL_FN_PARAMS BOOL ret)
            {
                if (!ret)
                {
                    __R_CALL_INTERNAL_METHOD(_Log_GetLastError)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
                return ret;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            _Post_satisfies_(return == err)
            __R_CONDITIONAL_METHOD(DWORD, Log_IfWin32Error)(__R_CONDITIONAL_FN_PARAMS DWORD err)
            {
                if (FAILED_WIN32(err))
                {
                    __R_CALL_INTERNAL_METHOD(_Log_Win32)(__R_CONDITIONAL_FN_CALL err);
                }
                return err;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            _Post_satisfies_(return == handle)
            __R_CONDITIONAL_METHOD(HANDLE, Log_IfHandleInvalid)(__R_CONDITIONAL_FN_PARAMS HANDLE handle)
            {
                if (handle == INVALID_HANDLE_VALUE)
                {
                    __R_CALL_INTERNAL_METHOD(_Log_GetLastError)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
                return handle;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            _Post_satisfies_(return == handle)
            __R_CONDITIONAL_METHOD(HANDLE, Log_IfHandleNull)(__R_CONDITIONAL_FN_PARAMS HANDLE handle)
            {
                if (handle == nullptr)
                {
                    __R_CALL_INTERNAL_METHOD(_Log_GetLastError)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
                return handle;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer)
            __R_CONDITIONAL_TEMPLATE_METHOD(PointerT, Log_IfNullAlloc)(__R_CONDITIONAL_FN_PARAMS _Pre_maybenull_ PointerT pointer) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    __R_CALL_INTERNAL_METHOD(_Log_NullAlloc)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
                return pointer;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __R_CONDITIONAL_TEMPLATE_METHOD(void, Log_IfNullAlloc)(__R_CONDITIONAL_FN_PARAMS const PointerT& pointer) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    __R_CALL_INTERNAL_METHOD(_Log_NullAlloc)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            _Post_satisfies_(return == condition)
            __R_CONDITIONAL_METHOD(bool, Log_HrIf)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, bool condition)
            {
                if (condition)
                {
                    __R_CALL_INTERNAL_METHOD(_Log_Hr)(__R_CONDITIONAL_FN_CALL hr);
                }
                return condition;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            _Post_satisfies_(return == condition)
            __R_CONDITIONAL_METHOD(bool, Log_HrIfFalse)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, bool condition)
            {
                if (!condition)
                {
                    __R_CALL_INTERNAL_METHOD(_Log_Hr)(__R_CONDITIONAL_FN_CALL hr);
                }
                return condition;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer)
            __R_CONDITIONAL_TEMPLATE_METHOD(PointerT, Log_HrIfNull)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, _Pre_maybenull_ PointerT pointer) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    __R_CALL_INTERNAL_METHOD(_Log_Hr)(__R_CONDITIONAL_FN_CALL hr);
                }
                return pointer;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __R_CONDITIONAL_TEMPLATE_METHOD(void, Log_HrIfNull)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, _In_opt_ const PointerT& pointer) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    __R_CALL_INTERNAL_METHOD(_Log_Hr)(__R_CONDITIONAL_FN_CALL hr);
                }
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            _Post_satisfies_(return == condition)
            __R_CONDITIONAL_METHOD(bool, Log_GetLastErrorIf)(__R_CONDITIONAL_FN_PARAMS bool condition)
            {
                if (condition)
                {
                    __R_CALL_INTERNAL_METHOD(_Log_GetLastError)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
                return condition;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            _Post_satisfies_(return == condition)
            __R_CONDITIONAL_METHOD(bool, Log_GetLastErrorIfFalse)(__R_CONDITIONAL_FN_PARAMS bool condition)
            {
                if (!condition)
                {
                    __R_CALL_INTERNAL_METHOD(_Log_GetLastError)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
                return condition;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer)
            __R_CONDITIONAL_TEMPLATE_METHOD(PointerT, Log_GetLastErrorIfNull)(__R_CONDITIONAL_FN_PARAMS _Pre_maybenull_ PointerT pointer) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    __R_CALL_INTERNAL_METHOD(_Log_GetLastError)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
                return pointer;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __R_CONDITIONAL_TEMPLATE_METHOD(void, Log_GetLastErrorIfNull)(__R_CONDITIONAL_FN_PARAMS _In_opt_ const PointerT& pointer) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    __R_CALL_INTERNAL_METHOD(_Log_GetLastError)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            _Post_satisfies_(return == status)
            __R_CONDITIONAL_METHOD(NTSTATUS, Log_IfNtStatusFailed)(__R_CONDITIONAL_FN_PARAMS NTSTATUS status)
            {
                if (FAILED_NTSTATUS(status))
                {
                    __R_CALL_INTERNAL_METHOD(_Log_NtStatus)(__R_CONDITIONAL_FN_CALL status);
                }
                return status;
            }

            _Post_satisfies_(return == hr)
            __R_DIRECT_METHOD(HRESULT, Log_HrMsg)(__R_DIRECT_FN_PARAMS HRESULT hr, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                wil::details::ReportFailure_HrMsg<FailureType::Log>(__R_DIRECT_FN_CALL hr, formatString, argList);
                return hr;
            }

            _Post_satisfies_(return == err)
            __R_DIRECT_METHOD(DWORD, Log_Win32Msg)(__R_DIRECT_FN_PARAMS DWORD err, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                wil::details::ReportFailure_Win32Msg<FailureType::Log>(__R_DIRECT_FN_CALL err, formatString, argList);
                return err;
            }

            __R_DIRECT_METHOD(DWORD, Log_GetLastErrorMsg)(__R_DIRECT_FN_PARAMS _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                return wil::details::ReportFailure_GetLastErrorMsg<FailureType::Log>(__R_DIRECT_FN_CALL formatString, argList);
            }

            _Post_satisfies_(return == status)
            __R_DIRECT_METHOD(NTSTATUS, Log_NtStatusMsg)(__R_DIRECT_FN_PARAMS NTSTATUS status, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                wil::details::ReportFailure_NtStatusMsg<FailureType::Log>(__R_DIRECT_FN_CALL status, formatString, argList);
                return status;
            }

#ifdef WIL_ENABLE_EXCEPTIONS
            __R_DIRECT_METHOD(HRESULT, Log_CaughtExceptionMsg)(__R_DIRECT_FN_PARAMS _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                return wil::details::ReportFailure_CaughtExceptionMsg<FailureType::Log>(__R_DIRECT_FN_CALL formatString, argList);
            }
#endif

            __R_INTERNAL_NOINLINE_METHOD(_Log_HrMsg)(__R_INTERNAL_NOINLINE_FN_PARAMS HRESULT hr, _Printf_format_string_ PCSTR formatString, va_list argList) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_HrMsg<FailureType::Log>(__R_INTERNAL_NOINLINE_FN_CALL hr, formatString, argList);
            }

            __R_INTERNAL_NOINLINE_METHOD(_Log_GetLastErrorMsg)(__R_INTERNAL_NOINLINE_FN_PARAMS _Printf_format_string_ PCSTR formatString, va_list argList) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_GetLastErrorMsg<FailureType::Log>(__R_INTERNAL_NOINLINE_FN_CALL formatString, argList);
            }

            __R_INTERNAL_NOINLINE_METHOD(_Log_Win32Msg)(__R_INTERNAL_NOINLINE_FN_PARAMS DWORD err, _Printf_format_string_ PCSTR formatString, va_list argList) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_Win32Msg<FailureType::Log>(__R_INTERNAL_NOINLINE_FN_CALL err, formatString, argList);
            }

            __R_INTERNAL_NOINLINE_METHOD(_Log_NullAllocMsg)(__R_INTERNAL_NOINLINE_FN_PARAMS _Printf_format_string_ PCSTR formatString, va_list argList) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_HrMsg<FailureType::Log>(__R_INTERNAL_NOINLINE_FN_CALL E_OUTOFMEMORY, formatString, argList);
            }

            __R_INTERNAL_NOINLINE_METHOD(_Log_NtStatusMsg)(__R_INTERNAL_NOINLINE_FN_PARAMS NTSTATUS status, _Printf_format_string_ PCSTR formatString, va_list argList) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_NtStatusMsg<FailureType::Log>(__R_INTERNAL_NOINLINE_FN_CALL status, formatString, argList);
            }

            _Post_satisfies_(return == hr)
            __R_CONDITIONAL_NOINLINE_METHOD(HRESULT, Log_IfFailedMsg)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (FAILED(hr))
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_HrMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
                return hr;
            }

            _Post_satisfies_(return == ret)
            __R_CONDITIONAL_NOINLINE_METHOD(BOOL, Log_IfWin32BoolFalseMsg)(__R_CONDITIONAL_FN_PARAMS BOOL ret, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (!ret)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_GetLastErrorMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return ret;
            }

            _Post_satisfies_(return == err)
            __R_CONDITIONAL_NOINLINE_METHOD(DWORD, Log_IfWin32ErrorMsg)(__R_CONDITIONAL_FN_PARAMS DWORD err, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (FAILED_WIN32(err))
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_Win32Msg)(__R_CONDITIONAL_NOINLINE_FN_CALL err, formatString, argList);
                }
                return err;
            }

            _Post_satisfies_(return == handle)
            __R_CONDITIONAL_NOINLINE_METHOD(HANDLE, Log_IfHandleInvalidMsg)(__R_CONDITIONAL_FN_PARAMS HANDLE handle, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (handle == INVALID_HANDLE_VALUE)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_GetLastErrorMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return handle;
            }

            _Post_satisfies_(return == handle)
            __R_CONDITIONAL_NOINLINE_METHOD(HANDLE, Log_IfHandleNullMsg)(__R_CONDITIONAL_FN_PARAMS HANDLE handle, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (handle == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_GetLastErrorMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return handle;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer)
            __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(PointerT, Log_IfNullAllocMsg)(__R_CONDITIONAL_FN_PARAMS _Pre_maybenull_ PointerT pointer, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_NullAllocMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL_ONLY, formatString, argList);
                }
                return pointer;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(void, Log_IfNullAllocMsg)(__R_CONDITIONAL_FN_PARAMS const PointerT& pointer, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_NullAllocMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL_ONLY, formatString, argList);
                }
            }

            _Post_satisfies_(return == condition)
            __R_CONDITIONAL_NOINLINE_METHOD(bool, Log_HrIfMsg)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, bool condition, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_HrMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
                return condition;
            }

            _Post_satisfies_(return == condition)
            __R_CONDITIONAL_NOINLINE_METHOD(bool, Log_HrIfFalseMsg)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, bool condition, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (!condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_HrMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
                return condition;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer)
            __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(PointerT, Log_HrIfNullMsg)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, _Pre_maybenull_ PointerT pointer, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_HrMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
                return pointer;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(void, Log_HrIfNullMsg)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, _In_opt_ const PointerT& pointer, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_HrMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
            }

            _Post_satisfies_(return == condition)
            __R_CONDITIONAL_NOINLINE_METHOD(bool, Log_GetLastErrorIfMsg)(__R_CONDITIONAL_FN_PARAMS bool condition, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_GetLastErrorMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return condition;
            }

            _Post_satisfies_(return == condition)
            __R_CONDITIONAL_NOINLINE_METHOD(bool, Log_GetLastErrorIfFalseMsg)(__R_CONDITIONAL_FN_PARAMS bool condition, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (!condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_GetLastErrorMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return condition;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer)
            __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(PointerT, Log_GetLastErrorIfNullMsg)(__R_CONDITIONAL_FN_PARAMS _Pre_maybenull_ PointerT pointer, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_GetLastErrorMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return pointer;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(void, Log_GetLastErrorIfNullMsg)(__R_CONDITIONAL_FN_PARAMS _In_opt_ const PointerT& pointer, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_GetLastErrorMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
            }

            _Post_satisfies_(return == status)
            __R_CONDITIONAL_NOINLINE_METHOD(NTSTATUS, Log_IfNtStatusFailedMsg)(__R_CONDITIONAL_FN_PARAMS NTSTATUS status, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (FAILED_NTSTATUS(status))
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Log_NtStatusMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL status, formatString, argList);
                }
                return status;
            }
        } // namespace __R_NS_NAME

        namespace __RFF_NS_NAME
        {
            //*****************************************************************************
            // FailFast Macros
            //*****************************************************************************

            __RFF_DIRECT_NORET_METHOD(void, FailFast_Hr)(__RFF_DIRECT_FN_PARAMS HRESULT hr) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_Hr<FailureType::FailFast>(__RFF_DIRECT_FN_CALL hr);
            }

            __RFF_DIRECT_NORET_METHOD(void, FailFast_Win32)(__RFF_DIRECT_FN_PARAMS DWORD err) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_Win32<FailureType::FailFast>(__RFF_DIRECT_FN_CALL err);
            }

            __RFF_DIRECT_NORET_METHOD(void, FailFast_GetLastError)(__RFF_DIRECT_FN_PARAMS_ONLY) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_GetLastError<FailureType::FailFast>(__RFF_DIRECT_FN_CALL_ONLY);
            }

            __RFF_DIRECT_NORET_METHOD(void, FailFast_NtStatus)(__RFF_DIRECT_FN_PARAMS NTSTATUS status) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_NtStatus<FailureType::FailFast>(__RFF_DIRECT_FN_CALL status);
            }

#ifdef WIL_ENABLE_EXCEPTIONS
            __RFF_DIRECT_NORET_METHOD(void, FailFast_CaughtException)(__RFF_DIRECT_FN_PARAMS_ONLY) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_CaughtException<FailureType::FailFast>(__RFF_DIRECT_FN_CALL_ONLY);
            }
#endif

            __RFF_INTERNAL_NORET_METHOD(_FailFast_Hr)(__RFF_INTERNAL_FN_PARAMS HRESULT hr) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_Hr<FailureType::FailFast>(__RFF_INTERNAL_FN_CALL hr);
            }

            __RFF_INTERNAL_NORET_METHOD(_FailFast_GetLastError)(__RFF_INTERNAL_FN_PARAMS_ONLY) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_GetLastError<FailureType::FailFast>(__RFF_INTERNAL_FN_CALL_ONLY);
            }

            __RFF_INTERNAL_NORET_METHOD(_FailFast_Win32)(__RFF_INTERNAL_FN_PARAMS DWORD err) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_Win32<FailureType::FailFast>(__RFF_INTERNAL_FN_CALL err);
            }

            __RFF_INTERNAL_NORET_METHOD(_FailFast_NullAlloc)(__RFF_INTERNAL_FN_PARAMS_ONLY) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_Hr<FailureType::FailFast>(__RFF_INTERNAL_FN_CALL E_OUTOFMEMORY);
            }

            __RFF_INTERNAL_NORET_METHOD(_FailFast_NtStatus)(__RFF_INTERNAL_FN_PARAMS NTSTATUS status) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_NtStatus<FailureType::FailFast>(__RFF_INTERNAL_FN_CALL status);
            }

            _Post_satisfies_(return == hr) _When_(FAILED(hr), _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(HRESULT, FailFast_IfFailed)(__RFF_CONDITIONAL_FN_PARAMS HRESULT hr) WI_NOEXCEPT
            {
                if (FAILED(hr))
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_Hr)(__RFF_CONDITIONAL_FN_CALL hr);
                }
                return hr;
            }

            _Post_satisfies_(return == ret) _When_(!ret, _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(BOOL, FailFast_IfWin32BoolFalse)(__RFF_CONDITIONAL_FN_PARAMS BOOL ret) WI_NOEXCEPT
            {
                if (!ret)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_GetLastError)(__RFF_CONDITIONAL_FN_CALL_ONLY);
                }
                return ret;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            _Post_satisfies_(return == err) _When_(FAILED_WIN32(err), _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(DWORD, FailFast_IfWin32Error)(__RFF_CONDITIONAL_FN_PARAMS DWORD err)
            {
                if (FAILED_WIN32(err))
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_Win32)(__RFF_CONDITIONAL_FN_CALL err);
                }
                return err;
            }

            _Post_satisfies_(return == handle) _When_(handle == INVALID_HANDLE_VALUE, _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(HANDLE, FailFast_IfHandleInvalid)(__RFF_CONDITIONAL_FN_PARAMS HANDLE handle) WI_NOEXCEPT
            {
                if (handle == INVALID_HANDLE_VALUE)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_GetLastError)(__RFF_CONDITIONAL_FN_CALL_ONLY);
                }
                return handle;
            }

            _Post_satisfies_(return == handle) _When_(handle == nullptr, _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(RESULT_NORETURN_NULL HANDLE, FailFast_IfHandleNull)(__RFF_CONDITIONAL_FN_PARAMS HANDLE handle) WI_NOEXCEPT
            {
                if (handle == nullptr)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_GetLastError)(__RFF_CONDITIONAL_FN_CALL_ONLY);
                }
                return handle;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __RFF_CONDITIONAL_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, FailFast_IfNullAlloc)(__RFF_CONDITIONAL_FN_PARAMS _Pre_maybenull_ PointerT pointer)
            {
                if (pointer == nullptr)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_NullAlloc)(__RFF_CONDITIONAL_FN_CALL_ONLY);
                }
                return pointer;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __RFF_CONDITIONAL_TEMPLATE_METHOD(void, FailFast_IfNullAlloc)(__RFF_CONDITIONAL_FN_PARAMS const PointerT& pointer)
            {
                if (pointer == nullptr)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_NullAlloc)(__RFF_CONDITIONAL_FN_CALL_ONLY);
                }
            }

            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(bool, FailFast_HrIf)(__RFF_CONDITIONAL_FN_PARAMS HRESULT hr, bool condition) WI_NOEXCEPT
            {
                if (condition)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_Hr)(__RFF_CONDITIONAL_FN_CALL hr);
                }
                return condition;
            }

            _Post_satisfies_(return == condition) _When_(!condition, _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(bool, FailFast_HrIfFalse)(__RFF_CONDITIONAL_FN_PARAMS HRESULT hr, bool condition) WI_NOEXCEPT
            {
                if (!condition)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_Hr)(__RFF_CONDITIONAL_FN_CALL hr);
                }
                return condition;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __RFF_CONDITIONAL_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, FailFast_HrIfNull)(__RFF_CONDITIONAL_FN_PARAMS HRESULT hr, _Pre_maybenull_ PointerT pointer)
            {
                if (pointer == nullptr)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_Hr)(__RFF_CONDITIONAL_FN_CALL hr);
                }
                return pointer;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __RFF_CONDITIONAL_TEMPLATE_METHOD(void, FailFast_HrIfNull)(__RFF_CONDITIONAL_FN_PARAMS HRESULT hr, _In_opt_ const PointerT& pointer)
            {
                if (pointer == nullptr)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_Hr)(__RFF_CONDITIONAL_FN_CALL hr);
                }
            }

            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(bool, FailFast_GetLastErrorIf)(__RFF_CONDITIONAL_FN_PARAMS bool condition) WI_NOEXCEPT
            {
                if (condition)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_GetLastError)(__RFF_CONDITIONAL_FN_CALL_ONLY);
                }
                return condition;
            }

            _Post_satisfies_(return == condition) _When_(!condition, _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(bool, FailFast_GetLastErrorIfFalse)(__RFF_CONDITIONAL_FN_PARAMS bool condition) WI_NOEXCEPT
            {
                if (!condition)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_GetLastError)(__RFF_CONDITIONAL_FN_CALL_ONLY);
                }
                return condition;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __RFF_CONDITIONAL_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, FailFast_GetLastErrorIfNull)(__RFF_CONDITIONAL_FN_PARAMS _Pre_maybenull_ PointerT pointer)
            {
                if (pointer == nullptr)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_GetLastError)(__RFF_CONDITIONAL_FN_CALL_ONLY);
                }
                return pointer;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __RFF_CONDITIONAL_TEMPLATE_METHOD(void, FailFast_GetLastErrorIfNull)(__RFF_CONDITIONAL_FN_PARAMS _In_opt_ const PointerT& pointer)
            {
                if (pointer == nullptr)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_GetLastError)(__RFF_CONDITIONAL_FN_CALL_ONLY);
                }
            }

            _Post_satisfies_(return == status) _When_(FAILED_NTSTATUS(status), _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(NTSTATUS, FailFast_IfNtStatusFailed)(__RFF_CONDITIONAL_FN_PARAMS NTSTATUS status) WI_NOEXCEPT
            {
                if (FAILED_NTSTATUS(status))
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_NtStatus)(__RFF_CONDITIONAL_FN_CALL status);
                }
                return status;
            }

            __RFF_DIRECT_NORET_METHOD(void, FailFast_HrMsg)(__RFF_DIRECT_FN_PARAMS HRESULT hr, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_HrMsg<FailureType::FailFast>(__RFF_DIRECT_FN_CALL hr, formatString, argList);
            }

            __RFF_DIRECT_NORET_METHOD(void, FailFast_Win32Msg)(__RFF_DIRECT_FN_PARAMS DWORD err, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_Win32Msg<FailureType::FailFast>(__RFF_DIRECT_FN_CALL err, formatString, argList);
            }

            __RFF_DIRECT_NORET_METHOD(void, FailFast_GetLastErrorMsg)(__RFF_DIRECT_FN_PARAMS _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_GetLastErrorMsg<FailureType::FailFast>(__RFF_DIRECT_FN_CALL formatString, argList);
            }

            __RFF_DIRECT_NORET_METHOD(void, FailFast_NtStatusMsg)(__RFF_DIRECT_FN_PARAMS NTSTATUS status, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_NtStatusMsg<FailureType::FailFast>(__RFF_DIRECT_FN_CALL status, formatString, argList);
            }

#ifdef WIL_ENABLE_EXCEPTIONS
            __RFF_DIRECT_NORET_METHOD(void, FailFast_CaughtExceptionMsg)(__RFF_DIRECT_FN_PARAMS _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_CaughtExceptionMsg<FailureType::FailFast>(__RFF_DIRECT_FN_CALL formatString, argList);
            }
#endif

            __RFF_INTERNAL_NOINLINE_NORET_METHOD(_FailFast_HrMsg)(__RFF_INTERNAL_NOINLINE_FN_PARAMS HRESULT hr, _Printf_format_string_ PCSTR formatString, va_list argList) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_HrMsg<FailureType::FailFast>(__RFF_INTERNAL_NOINLINE_FN_CALL hr, formatString, argList);
            }

            __RFF_INTERNAL_NOINLINE_NORET_METHOD(_FailFast_GetLastErrorMsg)(__RFF_INTERNAL_NOINLINE_FN_PARAMS _Printf_format_string_ PCSTR formatString, va_list argList) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_GetLastErrorMsg<FailureType::FailFast>(__RFF_INTERNAL_NOINLINE_FN_CALL formatString, argList);
            }

            __RFF_INTERNAL_NOINLINE_NORET_METHOD(_FailFast_Win32Msg)(__RFF_INTERNAL_NOINLINE_FN_PARAMS DWORD err, _Printf_format_string_ PCSTR formatString, va_list argList) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_Win32Msg<FailureType::FailFast>(__RFF_INTERNAL_NOINLINE_FN_CALL err, formatString, argList);
            }

            __RFF_INTERNAL_NOINLINE_NORET_METHOD(_FailFast_NullAllocMsg)(__RFF_INTERNAL_NOINLINE_FN_PARAMS _Printf_format_string_ PCSTR formatString, va_list argList) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_HrMsg<FailureType::FailFast>(__RFF_INTERNAL_NOINLINE_FN_CALL E_OUTOFMEMORY, formatString, argList);
            }

            __RFF_INTERNAL_NOINLINE_NORET_METHOD(_FailFast_NtStatusMsg)(__RFF_INTERNAL_NOINLINE_FN_PARAMS NTSTATUS status, _Printf_format_string_ PCSTR formatString, va_list argList) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_NtStatusMsg<FailureType::FailFast>(__RFF_INTERNAL_NOINLINE_FN_CALL status, formatString, argList);
            }

            _Post_satisfies_(return == hr) _When_(FAILED(hr), _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_METHOD(HRESULT, FailFast_IfFailedMsg)(__RFF_CONDITIONAL_FN_PARAMS HRESULT hr, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (FAILED(hr))
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_HrMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
                return hr;
            }

            _Post_satisfies_(return == ret) _When_(!ret, _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_METHOD(BOOL, FailFast_IfWin32BoolFalseMsg)(__RFF_CONDITIONAL_FN_PARAMS BOOL ret, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (!ret)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_GetLastErrorMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return ret;
            }

            _Post_satisfies_(return == err) _When_(FAILED_WIN32(err), _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_METHOD(DWORD, FailFast_IfWin32ErrorMsg)(__RFF_CONDITIONAL_FN_PARAMS DWORD err, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (FAILED_WIN32(err))
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_Win32Msg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL err, formatString, argList);
                }
                return err;
            }

            _Post_satisfies_(return == handle) _When_(handle == INVALID_HANDLE_VALUE, _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_METHOD(HANDLE, FailFast_IfHandleInvalidMsg)(__RFF_CONDITIONAL_FN_PARAMS HANDLE handle, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (handle == INVALID_HANDLE_VALUE)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_GetLastErrorMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return handle;
            }

            _Post_satisfies_(return == handle) _When_(handle == nullptr, _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_METHOD(RESULT_NORETURN_NULL HANDLE, FailFast_IfHandleNullMsg)(__RFF_CONDITIONAL_FN_PARAMS HANDLE handle, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (handle == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_GetLastErrorMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return handle;
            }

            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, FailFast_IfNullAllocMsg)(__RFF_CONDITIONAL_FN_PARAMS _Pre_maybenull_ PointerT pointer, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_NullAllocMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL_ONLY, formatString, argList);
                }
                return pointer;
            }

            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __RFF_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(void, FailFast_IfNullAllocMsg)(__RFF_CONDITIONAL_FN_PARAMS const PointerT& pointer, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_NullAllocMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL_ONLY, formatString, argList);
                }
            }

            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_METHOD(bool, FailFast_HrIfMsg)(__RFF_CONDITIONAL_FN_PARAMS HRESULT hr, bool condition, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_HrMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
                return condition;
            }

            _Post_satisfies_(return == condition) _When_(!condition, _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_METHOD(bool, FailFast_HrIfFalseMsg)(__RFF_CONDITIONAL_FN_PARAMS HRESULT hr, bool condition, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (!condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_HrMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
                return condition;
            }

            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, FailFast_HrIfNullMsg)(__RFF_CONDITIONAL_FN_PARAMS HRESULT hr, _Pre_maybenull_ PointerT pointer, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_HrMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
                return pointer;
            }

            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __RFF_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(void, FailFast_HrIfNullMsg)(__RFF_CONDITIONAL_FN_PARAMS HRESULT hr, _In_opt_ const PointerT& pointer, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_HrMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
            }

            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_METHOD(bool, FailFast_GetLastErrorIfMsg)(__RFF_CONDITIONAL_FN_PARAMS bool condition, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_GetLastErrorMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return condition;
            }

            _Post_satisfies_(return == condition) _When_(!condition, _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_METHOD(bool, FailFast_GetLastErrorIfFalseMsg)(__RFF_CONDITIONAL_FN_PARAMS bool condition, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (!condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_GetLastErrorMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return condition;
            }

            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, FailFast_GetLastErrorIfNullMsg)(__RFF_CONDITIONAL_FN_PARAMS _Pre_maybenull_ PointerT pointer, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_GetLastErrorMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return pointer;
            }

            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __RFF_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(void, FailFast_GetLastErrorIfNullMsg)(__RFF_CONDITIONAL_FN_PARAMS _In_opt_ const PointerT& pointer, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_GetLastErrorMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
            }

            _Post_satisfies_(return == status) _When_(FAILED_NTSTATUS(status), _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_METHOD(NTSTATUS, FailFast_IfNtStatusFailedMsg)(__RFF_CONDITIONAL_FN_PARAMS NTSTATUS status, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (FAILED_NTSTATUS(status))
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_NtStatusMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL status, formatString, argList);
                }
                return status;
            }

            __RFF_DIRECT_NORET_METHOD(void, FailFast_Unexpected)(__RFF_DIRECT_FN_PARAMS_ONLY) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_Hr<FailureType::FailFast>(__RFF_DIRECT_FN_CALL E_UNEXPECTED);
            }

            __RFF_INTERNAL_NORET_METHOD(_FailFast_Unexpected)(__RFF_INTERNAL_FN_PARAMS_ONLY) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_Hr<FailureType::FailFast>(__RFF_INTERNAL_FN_CALL E_UNEXPECTED);
            }

            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(bool, FailFast_If)(__RFF_CONDITIONAL_FN_PARAMS bool condition) WI_NOEXCEPT
            {
                if (condition)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_Unexpected)(__RFF_CONDITIONAL_FN_CALL_ONLY);
                }
                return condition;
            }

            _Post_satisfies_(return == condition) _When_(!condition, _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(bool, FailFast_IfFalse)(__RFF_CONDITIONAL_FN_PARAMS bool condition) WI_NOEXCEPT
            {
                if (!condition)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_Unexpected)(__RFF_CONDITIONAL_FN_CALL_ONLY);
                }
                return condition;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            __WI_SUPPRESS_NULLPTR_ANALYSIS
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __RFF_CONDITIONAL_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, FailFast_IfNull)(__RFF_CONDITIONAL_FN_PARAMS _Pre_maybenull_ PointerT pointer)
            {
                if (pointer == nullptr)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_Unexpected)(__RFF_CONDITIONAL_FN_CALL_ONLY);
                }
                return pointer;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __WI_SUPPRESS_NULLPTR_ANALYSIS
            __RFF_CONDITIONAL_TEMPLATE_METHOD(void, FailFast_IfNull)(__RFF_CONDITIONAL_FN_PARAMS _In_opt_ const PointerT& pointer)
            {
                if (pointer == nullptr)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFast_Unexpected)(__RFF_CONDITIONAL_FN_CALL_ONLY);
                }
            }

            __RFF_DIRECT_NORET_METHOD(void, FailFast_UnexpectedMsg)(__RFF_DIRECT_FN_PARAMS _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_HrMsg<FailureType::FailFast>(__RFF_DIRECT_FN_CALL E_UNEXPECTED, formatString, argList);
            }

            __RFF_INTERNAL_NOINLINE_NORET_METHOD(_FailFast_UnexpectedMsg)(__RFF_INTERNAL_NOINLINE_FN_PARAMS _Printf_format_string_ PCSTR formatString, va_list argList) WI_NOEXCEPT
            {
                __RFF_FN_LOCALS;
                wil::details::ReportFailure_HrMsg<FailureType::FailFast>(__RFF_INTERNAL_NOINLINE_FN_CALL E_UNEXPECTED, formatString, argList);
            }

            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_METHOD(bool, FailFast_IfMsg)(__RFF_CONDITIONAL_FN_PARAMS bool condition, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_UnexpectedMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return condition;
            }

            _Post_satisfies_(return == condition) _When_(!condition, _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_METHOD(bool, FailFast_IfFalseMsg)(__RFF_CONDITIONAL_FN_PARAMS bool condition, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (!condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_UnexpectedMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return condition;
            }

            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __RFF_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, FailFast_IfNullMsg)(__RFF_CONDITIONAL_FN_PARAMS _Pre_maybenull_ PointerT pointer, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_UnexpectedMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return pointer;
            }

            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __RFF_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(void, FailFast_IfNullMsg)(__RFF_CONDITIONAL_FN_PARAMS _In_opt_ const PointerT& pointer, _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __RFF_CALL_INTERNAL_NOINLINE_METHOD(_FailFast_UnexpectedMsg)(__RFF_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
            }

            //*****************************************************************************
            // FailFast Immediate Macros
            //*****************************************************************************

            __RFF_DIRECT_NORET_METHOD(void, FailFastImmediate_Unexpected)() WI_NOEXCEPT
            {
                __fastfail(FAST_FAIL_FATAL_APP_EXIT);
            }

            __RFF_INTERNAL_NORET_METHOD(_FailFastImmediate_Unexpected)() WI_NOEXCEPT
            {
                __fastfail(FAST_FAIL_FATAL_APP_EXIT);
            }

            _Post_satisfies_(return == hr) _When_(FAILED(hr), _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(HRESULT, FailFastImmediate_IfFailed)(HRESULT hr) WI_NOEXCEPT
            {
                if (FAILED(hr))
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFastImmediate_Unexpected)();
                }
                return hr;
            }

            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(bool, FailFastImmediate_If)(bool condition) WI_NOEXCEPT
            {
                if (condition)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFastImmediate_Unexpected)();
                }
                return condition;
            }

            _Post_satisfies_(return == condition) _When_(!condition, _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(bool, FailFastImmediate_IfFalse)(bool condition) WI_NOEXCEPT
            {
                if (!condition)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFastImmediate_Unexpected)();
                }
                return condition;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __RFF_CONDITIONAL_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, FailFastImmediate_IfNull)(_Pre_maybenull_ PointerT pointer)
            {
                if (pointer == nullptr)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFastImmediate_Unexpected)();
                }
                return pointer;
            }

            // Should be decorated WI_NOEXCEPT, but conflicts with forceinline.
            template <__RFF_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __RFF_CONDITIONAL_TEMPLATE_METHOD(void, FailFastImmediate_IfNull)(_In_opt_ const PointerT& pointer)
            {
                if (pointer == nullptr)
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFastImmediate_Unexpected)();
                }
            }

            _Post_satisfies_(return == status) _When_(FAILED_NTSTATUS(status), _Analysis_noreturn_)
            __RFF_CONDITIONAL_METHOD(NTSTATUS, FailFastImmediate_IfNtStatusFailed)(NTSTATUS status) WI_NOEXCEPT
            {
                if (FAILED_NTSTATUS(status))
                {
                    __RFF_CALL_INTERNAL_METHOD(_FailFastImmediate_Unexpected)();
                }
                return status;
            }
        } // namespace __RFF_NS_NAME

        namespace __R_NS_NAME
        {
            //*****************************************************************************
            // Exception Macros
            //*****************************************************************************

#ifdef WIL_ENABLE_EXCEPTIONS
            __R_DIRECT_NORET_METHOD(void, Throw_Hr)(__R_DIRECT_FN_PARAMS HRESULT hr)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_Hr<FailureType::Exception>(__R_DIRECT_FN_CALL hr);
            }

            __R_DIRECT_NORET_METHOD(void, Throw_Win32)(__R_DIRECT_FN_PARAMS DWORD err)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_Win32<FailureType::Exception>(__R_DIRECT_FN_CALL err);
            }

            __R_DIRECT_NORET_METHOD(void, Throw_GetLastError)(__R_DIRECT_FN_PARAMS_ONLY)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_GetLastError<FailureType::Exception>(__R_DIRECT_FN_CALL_ONLY);
            }

            __R_DIRECT_NORET_METHOD(void, Throw_NtStatus)(__R_DIRECT_FN_PARAMS NTSTATUS status)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_NtStatus<FailureType::Exception>(__R_DIRECT_FN_CALL status);
            }

            __R_DIRECT_NORET_METHOD(void, Throw_CaughtException)(__R_DIRECT_FN_PARAMS_ONLY)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_CaughtException<FailureType::Exception>(__R_DIRECT_FN_CALL_ONLY);
            }

            __R_INTERNAL_NORET_METHOD(_Throw_Hr)(__R_INTERNAL_FN_PARAMS HRESULT hr)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_Hr<FailureType::Exception>(__R_INTERNAL_FN_CALL hr);
            }

            __R_INTERNAL_NORET_METHOD(_Throw_GetLastError)(__R_INTERNAL_FN_PARAMS_ONLY)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_GetLastError<FailureType::Exception>(__R_INTERNAL_FN_CALL_ONLY);
            }

            __R_INTERNAL_NORET_METHOD(_Throw_Win32)(__R_INTERNAL_FN_PARAMS DWORD err)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_Win32<FailureType::Exception>(__R_INTERNAL_FN_CALL err);
            }

            __R_INTERNAL_NORET_METHOD(_Throw_NullAlloc)(__R_INTERNAL_FN_PARAMS_ONLY)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_Hr<FailureType::Exception>(__R_INTERNAL_FN_CALL E_OUTOFMEMORY);
            }

            __R_INTERNAL_NORET_METHOD(_Throw_NtStatus)(__R_INTERNAL_FN_PARAMS NTSTATUS status)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_NtStatus<FailureType::Exception>(__R_INTERNAL_FN_CALL status);
            }

            _Post_satisfies_(return == hr) _When_(FAILED(hr), _Analysis_noreturn_)
            __R_CONDITIONAL_METHOD(HRESULT, Throw_IfFailed)(__R_CONDITIONAL_FN_PARAMS HRESULT hr)
            {
                if (FAILED(hr))
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_Hr)(__R_CONDITIONAL_FN_CALL hr);
                }
                return hr;
            }

            _Post_satisfies_(return == ret) _When_(!ret, _Analysis_noreturn_)
            __R_CONDITIONAL_METHOD(BOOL, Throw_IfWin32BoolFalse)(__R_CONDITIONAL_FN_PARAMS BOOL ret)
            {
                if (!ret)
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_GetLastError)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
                return ret;
            }

            _Post_satisfies_(return == err) _When_(FAILED_WIN32(err), _Analysis_noreturn_)
            __R_CONDITIONAL_METHOD(DWORD, Throw_IfWin32Error)(__R_CONDITIONAL_FN_PARAMS DWORD err)
            {
                if (FAILED_WIN32(err))
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_Win32)(__R_CONDITIONAL_FN_CALL err);
                }
                return err;
            }

            _Post_satisfies_(return == handle) _When_(handle == INVALID_HANDLE_VALUE, _Analysis_noreturn_)
            __R_CONDITIONAL_METHOD(HANDLE, Throw_IfHandleInvalid)(__R_CONDITIONAL_FN_PARAMS HANDLE handle)
            {
                if (handle == INVALID_HANDLE_VALUE)
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_GetLastError)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
                return handle;
            }

            _Post_satisfies_(return == handle) _When_(handle == nullptr, _Analysis_noreturn_)
            __R_CONDITIONAL_METHOD(RESULT_NORETURN_NULL HANDLE, Throw_IfHandleNull)(__R_CONDITIONAL_FN_PARAMS HANDLE handle)
            {
                if (handle == nullptr)
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_GetLastError)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
                return handle;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __R_CONDITIONAL_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, Throw_IfNullAlloc)(__R_CONDITIONAL_FN_PARAMS _Pre_maybenull_ PointerT pointer)
            {
                if (pointer == nullptr)
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_NullAlloc)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
                return pointer;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __R_CONDITIONAL_TEMPLATE_METHOD(void, Throw_IfNullAlloc)(__R_CONDITIONAL_FN_PARAMS const PointerT& pointer)
            {
                if (pointer == nullptr)
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_NullAlloc)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
            }

            _Post_satisfies_(return == condition)
            _When_(condition, _Analysis_noreturn_)
            __R_CONDITIONAL_METHOD(bool, Throw_HrIf)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, bool condition)
            {
                if (condition)
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_Hr)(__R_CONDITIONAL_FN_CALL hr);
                }
                return condition;
            }

            _Post_satisfies_(return == condition)
            _When_(!condition, _Analysis_noreturn_)
            __R_CONDITIONAL_METHOD(bool, Throw_HrIfFalse)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, bool condition)
            {
                if (!condition)
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_Hr)(__R_CONDITIONAL_FN_CALL hr);
                }
                return condition;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __R_CONDITIONAL_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, Throw_HrIfNull)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, _Pre_maybenull_ PointerT pointer)
            {
                if (pointer == nullptr)
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_Hr)(__R_CONDITIONAL_FN_CALL hr);
                }
                return pointer;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __R_CONDITIONAL_TEMPLATE_METHOD(void, Throw_HrIfNull)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, _In_opt_ const PointerT& pointer)
            {
                if (pointer == nullptr)
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_Hr)(__R_CONDITIONAL_FN_CALL hr);
                }
            }

            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_)
            __R_CONDITIONAL_METHOD(bool, Throw_Win32If)(__R_CONDITIONAL_FN_PARAMS DWORD err, bool condition)
            {
                if (condition)
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_Win32)(__R_CONDITIONAL_FN_CALL err);
                }
                return condition;
            }

            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_)
            __R_CONDITIONAL_METHOD(bool, Throw_GetLastErrorIf)(__R_CONDITIONAL_FN_PARAMS bool condition)
            {
                if (condition)
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_GetLastError)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
                return condition;
            }

            _Post_satisfies_(return == condition) _When_(!condition, _Analysis_noreturn_)
            __R_CONDITIONAL_METHOD(bool, Throw_GetLastErrorIfFalse)(__R_CONDITIONAL_FN_PARAMS bool condition)
            {
                if (!condition)
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_GetLastError)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
                return condition;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __R_CONDITIONAL_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, Throw_GetLastErrorIfNull)(__R_CONDITIONAL_FN_PARAMS _Pre_maybenull_ PointerT pointer)
            {
                if (pointer == nullptr)
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_GetLastError)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
                return pointer;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __R_CONDITIONAL_TEMPLATE_METHOD(void, Throw_GetLastErrorIfNull)(__R_CONDITIONAL_FN_PARAMS _In_opt_ const PointerT& pointer)
            {
                if (pointer == nullptr)
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_GetLastError)(__R_CONDITIONAL_FN_CALL_ONLY);
                }
            }

            _Post_satisfies_(return == status)
            _When_(FAILED_NTSTATUS(status), _Analysis_noreturn_)
            __R_CONDITIONAL_METHOD(NTSTATUS, Throw_IfNtStatusFailed)(__R_CONDITIONAL_FN_PARAMS NTSTATUS status)
            {
                if (FAILED_NTSTATUS(status))
                {
                    __R_CALL_INTERNAL_METHOD(_Throw_NtStatus)(__R_CONDITIONAL_FN_CALL status);
                }
                return status;
            }

            __R_DIRECT_NORET_METHOD(void, Throw_HrMsg)(__R_DIRECT_FN_PARAMS HRESULT hr, _Printf_format_string_ PCSTR formatString, ...)
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                wil::details::ReportFailure_HrMsg<FailureType::Exception>(__R_DIRECT_FN_CALL hr, formatString, argList);
            }

            __R_DIRECT_NORET_METHOD(void, Throw_Win32Msg)(__R_DIRECT_FN_PARAMS DWORD err, _Printf_format_string_ PCSTR formatString, ...)
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                wil::details::ReportFailure_Win32Msg<FailureType::Exception>(__R_DIRECT_FN_CALL err, formatString, argList);
            }

            __R_DIRECT_NORET_METHOD(void, Throw_GetLastErrorMsg)(__R_DIRECT_FN_PARAMS _Printf_format_string_ PCSTR formatString, ...)
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                wil::details::ReportFailure_GetLastErrorMsg<FailureType::Exception>(__R_DIRECT_FN_CALL formatString, argList);
            }

            __R_DIRECT_NORET_METHOD(void, Throw_NtStatusMsg)(__R_DIRECT_FN_PARAMS NTSTATUS status, _Printf_format_string_ PCSTR formatString, ...)
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                wil::details::ReportFailure_NtStatusMsg<FailureType::Exception>(__R_DIRECT_FN_CALL status, formatString, argList);
            }

            __R_DIRECT_NORET_METHOD(void, Throw_CaughtExceptionMsg)(__R_DIRECT_FN_PARAMS _Printf_format_string_ PCSTR formatString, ...)
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                wil::details::ReportFailure_CaughtExceptionMsg<FailureType::Exception>(__R_DIRECT_FN_CALL formatString, argList);
            }

            __R_INTERNAL_NOINLINE_NORET_METHOD(_Throw_HrMsg)(__R_INTERNAL_NOINLINE_FN_PARAMS HRESULT hr, _Printf_format_string_ PCSTR formatString, va_list argList)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_HrMsg<FailureType::Exception>(__R_INTERNAL_NOINLINE_FN_CALL hr, formatString, argList);
            }

            __R_INTERNAL_NOINLINE_NORET_METHOD(_Throw_GetLastErrorMsg)(__R_INTERNAL_NOINLINE_FN_PARAMS _Printf_format_string_ PCSTR formatString, va_list argList)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_GetLastErrorMsg<FailureType::Exception>(__R_INTERNAL_NOINLINE_FN_CALL formatString, argList);
            }

            __R_INTERNAL_NOINLINE_NORET_METHOD(_Throw_Win32Msg)(__R_INTERNAL_NOINLINE_FN_PARAMS DWORD err, _Printf_format_string_ PCSTR formatString, va_list argList)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_Win32Msg<FailureType::Exception>(__R_INTERNAL_NOINLINE_FN_CALL err, formatString, argList);
            }

            __R_INTERNAL_NOINLINE_NORET_METHOD(_Throw_NullAllocMsg)(__R_INTERNAL_NOINLINE_FN_PARAMS _Printf_format_string_ PCSTR formatString, va_list argList)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_HrMsg<FailureType::Exception>(__R_INTERNAL_NOINLINE_FN_CALL E_OUTOFMEMORY, formatString, argList);
            }

            __R_INTERNAL_NOINLINE_NORET_METHOD(_Throw_NtStatusMsg)(__R_INTERNAL_NOINLINE_FN_PARAMS NTSTATUS status, _Printf_format_string_ PCSTR formatString, va_list argList)
            {
                __R_FN_LOCALS;
                wil::details::ReportFailure_NtStatusMsg<FailureType::Exception>(__R_INTERNAL_NOINLINE_FN_CALL status, formatString, argList);
            }

            _Post_satisfies_(return == hr) _When_(FAILED(hr), _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_METHOD(HRESULT, Throw_IfFailedMsg)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (FAILED(hr))
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_HrMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
                return hr;
            }

            _Post_satisfies_(return == ret) _When_(!ret, _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_METHOD(BOOL, Throw_IfWin32BoolFalseMsg)(__R_CONDITIONAL_FN_PARAMS BOOL ret, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (!ret)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_GetLastErrorMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return ret;
            }

            _Post_satisfies_(return == err) _When_(FAILED_WIN32(err), _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_METHOD(DWORD, Throw_IfWin32ErrorMsg)(__R_CONDITIONAL_FN_PARAMS DWORD err, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (FAILED_WIN32(err))
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_Win32Msg)(__R_CONDITIONAL_NOINLINE_FN_CALL err, formatString, argList);
                }
                return err;
            }

            _Post_satisfies_(return == handle) _When_(handle == INVALID_HANDLE_VALUE, _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_METHOD(HANDLE, Throw_IfHandleInvalidMsg)(__R_CONDITIONAL_FN_PARAMS HANDLE handle, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (handle == INVALID_HANDLE_VALUE)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_GetLastErrorMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return handle;
            }

            _Post_satisfies_(return == handle) _When_(handle == 0, _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_METHOD(RESULT_NORETURN_NULL HANDLE, Throw_IfHandleNullMsg)(__R_CONDITIONAL_FN_PARAMS HANDLE handle, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (handle == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_GetLastErrorMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return handle;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, Throw_IfNullAllocMsg)(__R_CONDITIONAL_FN_PARAMS _Pre_maybenull_ PointerT pointer, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_NullAllocMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL_ONLY, formatString, argList);
                }
                return pointer;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __WI_SUPPRESS_NULLPTR_ANALYSIS
            _When_(pointer == nullptr, _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(void, Throw_IfNullAllocMsg)(__R_CONDITIONAL_FN_PARAMS const PointerT& pointer, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_NullAllocMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL_ONLY, formatString, argList);
                }
            }

            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_METHOD(bool, Throw_HrIfMsg)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, bool condition, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_HrMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
                return condition;
            }

            _Post_satisfies_(return == condition) _When_(!condition, _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_METHOD(bool, Throw_HrIfFalseMsg)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, bool condition, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (!condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_HrMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
                return condition;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            __WI_SUPPRESS_NULLPTR_ANALYSIS
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, Throw_HrIfNullMsg)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, _Pre_maybenull_ PointerT pointer, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_HrMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
                return pointer;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __WI_SUPPRESS_NULLPTR_ANALYSIS
            _When_(pointer == nullptr, _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(void, Throw_HrIfNullMsg)(__R_CONDITIONAL_FN_PARAMS HRESULT hr, _In_opt_ const PointerT& pointer, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_HrMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL hr, formatString, argList);
                }
            }

            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_METHOD(bool, Throw_Win32IfMsg)(__R_CONDITIONAL_FN_PARAMS DWORD err, bool condition, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_Win32Msg)(__R_CONDITIONAL_NOINLINE_FN_CALL err, formatString, argList);
                }
                return condition;
            }

            _Post_satisfies_(return == condition) _When_(condition, _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_METHOD(bool, Throw_GetLastErrorIfMsg)(__R_CONDITIONAL_FN_PARAMS bool condition, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_GetLastErrorMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return condition;
            }

            _Post_satisfies_(return == condition) _When_(!condition, _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_METHOD(bool, Throw_GetLastErrorIfFalseMsg)(__R_CONDITIONAL_FN_PARAMS bool condition, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (!condition)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_GetLastErrorMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return condition;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_NOT_CLASS(PointerT)>
            _Post_satisfies_(return == pointer) _When_(pointer == nullptr, _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(RESULT_NORETURN_NULL PointerT, Throw_GetLastErrorIfNullMsg)(__R_CONDITIONAL_FN_PARAMS _Pre_maybenull_ PointerT pointer, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_GetLastErrorMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
                return pointer;
            }

            template <__R_CONDITIONAL_PARTIAL_TEMPLATE typename PointerT, __R_ENABLE_IF_IS_CLASS(PointerT)>
            __WI_SUPPRESS_NULLPTR_ANALYSIS
            _When_(pointer == nullptr, _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_TEMPLATE_METHOD(void, Throw_GetLastErrorIfNullMsg)(__R_CONDITIONAL_FN_PARAMS _In_opt_ const PointerT& pointer, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (pointer == nullptr)
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_GetLastErrorMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL formatString, argList);
                }
            }

            _Post_satisfies_(return == status) _When_(FAILED_NTSTATUS(status), _Analysis_noreturn_)
            __R_CONDITIONAL_NOINLINE_METHOD(NTSTATUS, Throw_IfNtStatusFailedMsg)(__R_CONDITIONAL_FN_PARAMS NTSTATUS status, _Printf_format_string_ PCSTR formatString, ...)
            {
                if (FAILED_NTSTATUS(status))
                {
                    va_list argList;
                    va_start(argList, formatString);
                    __R_CALL_INTERNAL_NOINLINE_METHOD(_Throw_NtStatusMsg)(__R_CONDITIONAL_NOINLINE_FN_CALL status, formatString, argList);
                }
                return status;
            }
#endif // WIL_ENABLE_EXCEPTIONS

        }   // __R_NS_NAME namespace
    }   // details namespace
    /// @endcond


    //*****************************************************************************
    // Error Handling Policies to switch between error-handling style
    //*****************************************************************************
    // The following policies are used as template policies for components that can support exception, fail-fast, and
    // error-code based modes.

    // Use for classes which should return HRESULTs as their error-handling policy
    // Intentionally removed logging from this policy as logging is more useful at the caller.
    struct err_returncode_policy
    {
        using result = HRESULT;

        __forceinline static HRESULT Win32BOOL(BOOL fReturn) { RETURN_IF_WIN32_BOOL_FALSE_EXPECTED(fReturn); return S_OK; }
        __forceinline static HRESULT Win32Handle(HANDLE h, _Out_ HANDLE *ph) { *ph = h; RETURN_LAST_ERROR_IF_NULL_EXPECTED(h); return S_OK; }
        _Post_satisfies_(return == hr)
        __forceinline static HRESULT HResult(HRESULT hr) { return hr; }
        __forceinline static HRESULT LastError() { return wil::details::GetLastErrorFailHr(); }
        __forceinline static HRESULT LastErrorIfFalse(bool condition) { RETURN_LAST_ERROR_IF_EXPECTED(!condition); return S_OK; }
        _Post_satisfies_(return == S_OK)
        __forceinline static HRESULT OK() { return S_OK; }
    };

    // Use for classes which fail-fast on errors
    struct err_failfast_policy
    {
        typedef _Return_type_success_(true) void result;
        __forceinline static result Win32BOOL(BOOL fReturn) { FAIL_FAST_IF_WIN32_BOOL_FALSE(fReturn); }
        __forceinline static result Win32Handle(HANDLE h, _Out_ HANDLE *ph) { *ph = h; FAIL_FAST_LAST_ERROR_IF_NULL(h); }
        _When_(FAILED(hr), _Analysis_noreturn_)
        __forceinline static result HResult(HRESULT hr) { FAIL_FAST_IF_FAILED(hr); }
        __forceinline static result LastError() { FAIL_FAST_LAST_ERROR(); }
        __forceinline static result LastErrorIfFalse(bool condition) { if (!condition) { FAIL_FAST_LAST_ERROR(); } }
        __forceinline static result OK() {}
    };

#ifdef WIL_ENABLE_EXCEPTIONS
    // Use for classes which should return through exceptions as their error-handling policy
    struct err_exception_policy
    {
        typedef _Return_type_success_(true) void result;
        __forceinline static result Win32BOOL(BOOL fReturn) { THROW_IF_WIN32_BOOL_FALSE(fReturn); }
        __forceinline static result Win32Handle(HANDLE h, _Out_ HANDLE *ph) { *ph = h; THROW_LAST_ERROR_IF_NULL(h); }
        _When_(FAILED(hr), _Analysis_noreturn_)
        __forceinline static result HResult(HRESULT hr) { THROW_IF_FAILED(hr); }
        __forceinline static result LastError() { THROW_LAST_ERROR(); }
        __forceinline static result LastErrorIfFalse(bool condition) { if (!condition) { THROW_LAST_ERROR(); } }
        __forceinline static result OK() {}
    };
#else
    // NOTE: A lot of types use 'err_exception_policy' as a default template argument and therefore it must be defined
    // (MSVC is permissive about this, but other compilers are not). This will still cause compilation errors at
    // template instantiation time since this type lacks required member functions. An alternative would be to have some
    // 'default_err_policy' alias that would be something like 'err_failfast_policy' when exceptions are not available,
    // but that may have unexpected side effects when compiling code that expects to be using exceptions
    struct err_exception_policy
    {
    };
#endif

} // namespace wil

#pragma warning(pop)

#endif // defined(__cplusplus) && !defined(__WIL_MIN_KERNEL) && !defined(WIL_KERNEL_MODE)
#endif // __WIL_RESULTMACROS_INCLUDED
