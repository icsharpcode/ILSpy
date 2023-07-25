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
#ifndef __WIL_COMMON_INCLUDED
#define __WIL_COMMON_INCLUDED

#if defined(_KERNEL_MODE ) && !defined(__WIL_MIN_KERNEL)
// This define indicates that the WIL usage is in a kernel mode context where
// a high degree of WIL functionality is desired.
//
// Use (sparingly) to change behavior based on whether WIL is being used in kernel
// mode or user mode.
#define WIL_KERNEL_MODE
#endif

// Defining WIL_HIDE_DEPRECATED will hide everything deprecated.
// Each wave of deprecation will add a new WIL_HIDE_DEPRECATED_YYMM number that can be used to lock deprecation at
// a particular point, allowing components to avoid backslide and catch up to the current independently.
#ifdef WIL_HIDE_DEPRECATED
#define WIL_HIDE_DEPRECATED_1809
#endif
#ifdef WIL_HIDE_DEPRECATED_1809
#define WIL_HIDE_DEPRECATED_1612
#endif
#ifdef WIL_HIDE_DEPRECATED_1612
#define WIL_HIDE_DEPRECATED_1611
#endif

// Implementation side note: ideally the deprecation would be done with the function-level declspec
// as it allows you to utter the error text when used.  The declspec works, but doing it selectively with
// a macro makes intellisense deprecation comments not work.  So we just use the #pragma deprecation.
#ifdef WIL_WARN_DEPRECATED
#define WIL_WARN_DEPRECATED_1809
#endif
#ifdef WIL_WARN_DEPRECATED_1809
#define WIL_WARN_DEPRECATED_1612
#endif
#ifdef WIL_WARN_DEPRECATED_1612
#define WIL_WARN_DEPRECATED_1611
#endif
#ifdef WIL_WARN_DEPRECATED_1809
#define WIL_WARN_DEPRECATED_1809_PRAGMA(...) __pragma(deprecated(__VA_ARGS__))
#else
#define WIL_WARN_DEPRECATED_1809_PRAGMA(...)
#endif
#ifdef WIL_WARN_DEPRECATED_1611
#define WIL_WARN_DEPRECATED_1611_PRAGMA(...) __pragma(deprecated(__VA_ARGS__))
#else
#define WIL_WARN_DEPRECATED_1611_PRAGMA(...)
#endif
#ifdef WIL_WARN_DEPRECATED_1612
#define WIL_WARN_DEPRECATED_1612_PRAGMA(...) __pragma(deprecated(__VA_ARGS__))
#else
#define WIL_WARN_DEPRECATED_1612_PRAGMA(...)
#endif

#if defined(_MSVC_LANG)
#define __WI_SUPPRESS_4127_S __pragma(warning(push)) __pragma(warning(disable:4127)) __pragma(warning(disable:26498)) __pragma(warning(disable:4245))
#define __WI_SUPPRESS_4127_E __pragma(warning(pop))
#define __WI_SUPPRESS_NULLPTR_ANALYSIS __pragma(warning(suppress:28285)) __pragma(warning(suppress:6504))
#define __WI_SUPPRESS_NONINIT_ANALYSIS __pragma(warning(suppress:26495))
#define __WI_SUPPRESS_NOEXCEPT_ANALYSIS __pragma(warning(suppress:26439))
#else
#define __WI_SUPPRESS_4127_S
#define __WI_SUPPRESS_4127_E
#define __WI_SUPPRESS_NULLPTR_ANALYSIS
#define __WI_SUPPRESS_NONINIT_ANALYSIS
#define __WI_SUPPRESS_NOEXCEPT_ANALYSIS
#endif

#include <sal.h>

// Some SAL remapping / decoration to better support Doxygen.  Macros that look like function calls can
// confuse Doxygen when they are used to decorate a function or variable.  We simplify some of these to
// basic macros without the function for common use cases.
/// @cond
#define _Success_return_ _Success_(return)
#define _Success_true_ _Success_(true)
#define __declspec_noinline_ __declspec(noinline)
#define __declspec_selectany_ __declspec(selectany)
/// @endcond

//! @defgroup macrobuilding Macro Composition
//! The following macros are building blocks primarily intended for authoring other macros.
//! @{

//! Re-state a macro value (indirection for composition)
#define WI_FLATTEN(...)                     __VA_ARGS__

/// @cond
#define __WI_PASTE_imp(a, b)                a##b
/// @endcond

//! This macro is for use in other macros to paste two tokens together, such as a constant and the __LINE__ macro.
#define WI_PASTE(a, b)                      __WI_PASTE_imp(a, b)

/// @cond
#define __WI_HAS_VA_OPT_IMPL(F, T, ...) T
#define __WI_HAS_VA_OPT_(...) __WI_HAS_VA_OPT_IMPL(__VA_OPT__(0,) 1, 0)
/// @endcond

//! Evaluates to '1' when support for '__VA_OPT__' is available, else '0'
#define WI_HAS_VA_OPT __WI_HAS_VA_OPT_(unused)

/// @cond
#define __WI_ARGS_COUNT1(A0, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18, A19, A20, A21, A22, A23, A24, A25, A26, A27, A28, A29, \
                         A30, A31, A32, A33, A34, A35, A36, A37, A38, A39, A40, A41, A42, A43, A44, A45, A46, A47, A48, A49, A50, A51, A52, A53, A54, A55, A56, A57, A58, A59, \
                         A60, A61, A62, A63, A64, A65, A66, A67, A68, A69, A70, A71, A72, A73, A74, A75, A76, A77, A78, A79, A80, A81, A82, A83, A84, A85, A86, A87, A88, A89, \
                         A90, A91, A92, A93, A94, A95, A96, A97, A98, A99, count, ...) count
#define __WI_ARGS_COUNT0(...) WI_FLATTEN(__WI_ARGS_COUNT1(__VA_ARGS__, 99, 98, 97, 96, 95, 94, 93, 92, 91, 90, 89, 88, 87, 86, 85, 84, 83, 82, 81, 80, \
                         79, 78, 77, 76, 75, 74, 73, 72, 71, 70, 69, 68, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50,  49, 48, 47, 46, 45, 44, 43, 42, 41, 40, \
                         39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0))
#define __WI_ARGS_COUNT_PREFIX(...) 0, __VA_ARGS__
/// @endcond

//! This variadic macro returns the number of arguments passed to it (up to 99).
#if WI_HAS_VA_OPT
#define WI_ARGS_COUNT(...) __WI_ARGS_COUNT0(0 __VA_OPT__(, __VA_ARGS__))
#else
#define WI_ARGS_COUNT(...) __WI_ARGS_COUNT0(__WI_ARGS_COUNT_PREFIX(__VA_ARGS__))
#endif

/// @cond
#define __WI_FOR_imp0( fn)
#define __WI_FOR_imp1( fn, arg)      fn(arg)
#define __WI_FOR_imp2( fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp1(fn, __VA_ARGS__))
#define __WI_FOR_imp3( fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp2(fn, __VA_ARGS__))
#define __WI_FOR_imp4( fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp3(fn, __VA_ARGS__))
#define __WI_FOR_imp5( fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp4(fn, __VA_ARGS__))
#define __WI_FOR_imp6( fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp5(fn, __VA_ARGS__))
#define __WI_FOR_imp7( fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp6(fn, __VA_ARGS__))
#define __WI_FOR_imp8( fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp7(fn, __VA_ARGS__))
#define __WI_FOR_imp9( fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp8(fn, __VA_ARGS__))
#define __WI_FOR_imp10(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp9(fn, __VA_ARGS__))
#define __WI_FOR_imp11(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp10(fn, __VA_ARGS__))
#define __WI_FOR_imp12(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp11(fn, __VA_ARGS__))
#define __WI_FOR_imp13(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp12(fn, __VA_ARGS__))
#define __WI_FOR_imp14(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp13(fn, __VA_ARGS__))
#define __WI_FOR_imp15(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp14(fn, __VA_ARGS__))
#define __WI_FOR_imp16(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp15(fn, __VA_ARGS__))
#define __WI_FOR_imp17(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp16(fn, __VA_ARGS__))
#define __WI_FOR_imp18(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp17(fn, __VA_ARGS__))
#define __WI_FOR_imp19(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp18(fn, __VA_ARGS__))
#define __WI_FOR_imp20(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp19(fn, __VA_ARGS__))
#define __WI_FOR_imp21(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp20(fn, __VA_ARGS__))
#define __WI_FOR_imp22(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp21(fn, __VA_ARGS__))
#define __WI_FOR_imp23(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp22(fn, __VA_ARGS__))
#define __WI_FOR_imp24(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp23(fn, __VA_ARGS__))
#define __WI_FOR_imp25(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp24(fn, __VA_ARGS__))
#define __WI_FOR_imp26(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp25(fn, __VA_ARGS__))
#define __WI_FOR_imp27(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp26(fn, __VA_ARGS__))
#define __WI_FOR_imp28(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp27(fn, __VA_ARGS__))
#define __WI_FOR_imp29(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp28(fn, __VA_ARGS__))
#define __WI_FOR_imp30(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp29(fn, __VA_ARGS__))
#define __WI_FOR_imp31(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp30(fn, __VA_ARGS__))
#define __WI_FOR_imp32(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp31(fn, __VA_ARGS__))
#define __WI_FOR_imp33(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp32(fn, __VA_ARGS__))
#define __WI_FOR_imp34(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp33(fn, __VA_ARGS__))
#define __WI_FOR_imp35(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp34(fn, __VA_ARGS__))
#define __WI_FOR_imp36(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp35(fn, __VA_ARGS__))
#define __WI_FOR_imp37(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp36(fn, __VA_ARGS__))
#define __WI_FOR_imp38(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp37(fn, __VA_ARGS__))
#define __WI_FOR_imp39(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp38(fn, __VA_ARGS__))
#define __WI_FOR_imp40(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp39(fn, __VA_ARGS__))
#define __WI_FOR_imp41(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp40(fn, __VA_ARGS__))
#define __WI_FOR_imp42(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp41(fn, __VA_ARGS__))
#define __WI_FOR_imp43(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp42(fn, __VA_ARGS__))
#define __WI_FOR_imp44(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp43(fn, __VA_ARGS__))
#define __WI_FOR_imp45(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp44(fn, __VA_ARGS__))
#define __WI_FOR_imp46(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp45(fn, __VA_ARGS__))
#define __WI_FOR_imp47(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp46(fn, __VA_ARGS__))
#define __WI_FOR_imp48(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp47(fn, __VA_ARGS__))
#define __WI_FOR_imp49(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp48(fn, __VA_ARGS__))
#define __WI_FOR_imp50(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp49(fn, __VA_ARGS__))
#define __WI_FOR_imp51(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp50(fn, __VA_ARGS__))
#define __WI_FOR_imp52(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp51(fn, __VA_ARGS__))
#define __WI_FOR_imp53(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp52(fn, __VA_ARGS__))
#define __WI_FOR_imp54(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp53(fn, __VA_ARGS__))
#define __WI_FOR_imp55(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp54(fn, __VA_ARGS__))
#define __WI_FOR_imp56(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp55(fn, __VA_ARGS__))
#define __WI_FOR_imp57(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp56(fn, __VA_ARGS__))
#define __WI_FOR_imp58(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp57(fn, __VA_ARGS__))
#define __WI_FOR_imp59(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp58(fn, __VA_ARGS__))
#define __WI_FOR_imp60(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp59(fn, __VA_ARGS__))
#define __WI_FOR_imp61(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp60(fn, __VA_ARGS__))
#define __WI_FOR_imp62(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp61(fn, __VA_ARGS__))
#define __WI_FOR_imp63(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp62(fn, __VA_ARGS__))
#define __WI_FOR_imp64(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp63(fn, __VA_ARGS__))
#define __WI_FOR_imp65(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp64(fn, __VA_ARGS__))
#define __WI_FOR_imp66(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp65(fn, __VA_ARGS__))
#define __WI_FOR_imp67(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp66(fn, __VA_ARGS__))
#define __WI_FOR_imp68(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp67(fn, __VA_ARGS__))
#define __WI_FOR_imp69(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp68(fn, __VA_ARGS__))
#define __WI_FOR_imp70(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp69(fn, __VA_ARGS__))
#define __WI_FOR_imp71(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp70(fn, __VA_ARGS__))
#define __WI_FOR_imp72(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp71(fn, __VA_ARGS__))
#define __WI_FOR_imp73(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp72(fn, __VA_ARGS__))
#define __WI_FOR_imp74(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp73(fn, __VA_ARGS__))
#define __WI_FOR_imp75(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp74(fn, __VA_ARGS__))
#define __WI_FOR_imp76(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp75(fn, __VA_ARGS__))
#define __WI_FOR_imp77(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp76(fn, __VA_ARGS__))
#define __WI_FOR_imp78(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp77(fn, __VA_ARGS__))
#define __WI_FOR_imp79(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp78(fn, __VA_ARGS__))
#define __WI_FOR_imp80(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp79(fn, __VA_ARGS__))
#define __WI_FOR_imp81(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp80(fn, __VA_ARGS__))
#define __WI_FOR_imp82(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp81(fn, __VA_ARGS__))
#define __WI_FOR_imp83(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp82(fn, __VA_ARGS__))
#define __WI_FOR_imp84(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp83(fn, __VA_ARGS__))
#define __WI_FOR_imp85(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp84(fn, __VA_ARGS__))
#define __WI_FOR_imp86(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp85(fn, __VA_ARGS__))
#define __WI_FOR_imp87(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp86(fn, __VA_ARGS__))
#define __WI_FOR_imp88(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp87(fn, __VA_ARGS__))
#define __WI_FOR_imp89(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp88(fn, __VA_ARGS__))
#define __WI_FOR_imp90(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp89(fn, __VA_ARGS__))
#define __WI_FOR_imp91(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp90(fn, __VA_ARGS__))
#define __WI_FOR_imp92(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp91(fn, __VA_ARGS__))
#define __WI_FOR_imp93(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp92(fn, __VA_ARGS__))
#define __WI_FOR_imp94(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp93(fn, __VA_ARGS__))
#define __WI_FOR_imp95(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp94(fn, __VA_ARGS__))
#define __WI_FOR_imp96(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp95(fn, __VA_ARGS__))
#define __WI_FOR_imp97(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp96(fn, __VA_ARGS__))
#define __WI_FOR_imp98(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp97(fn, __VA_ARGS__))
#define __WI_FOR_imp99(fn, arg, ...) fn(arg) WI_FLATTEN(__WI_FOR_imp98(fn, __VA_ARGS__))

#define __WI_FOR_imp(n, fnAndArgs)  WI_PASTE(__WI_FOR_imp, n) fnAndArgs
/// @endcond

//! Iterates through each of the given arguments invoking the specified macro against each one.
#define WI_FOREACH(fn, ...) __WI_FOR_imp(WI_ARGS_COUNT(__VA_ARGS__), (fn, ##__VA_ARGS__))

//! Dispatches a single macro name to separate macros based on the number of arguments passed to it.
#define WI_MACRO_DISPATCH(name, ...) WI_PASTE(WI_PASTE(name, WI_ARGS_COUNT(__VA_ARGS__)), (__VA_ARGS__))

//! @} // Macro composition helpers

#if !defined(__cplusplus) || defined(__WIL_MIN_KERNEL)

#define WI_ODR_PRAGMA(NAME, TOKEN)
#define WI_NOEXCEPT

#else
#pragma warning(push)
#pragma warning(disable:4714)    // __forceinline not honored

// DO NOT add *any* further includes to this file -- there should be no dependencies from its usage
#include "wistd_type_traits.h"

//! This macro inserts ODR violation protection; the macro allows it to be compatible with straight "C" code
#define WI_ODR_PRAGMA(NAME, TOKEN)  __pragma(detect_mismatch("ODR_violation_" NAME "_mismatch", TOKEN))

#ifdef WIL_KERNEL_MODE
WI_ODR_PRAGMA("WIL_KERNEL_MODE", "1")
#else
WI_ODR_PRAGMA("WIL_KERNEL_MODE", "0")
#endif

#if (defined(_CPPUNWIND) || defined(__EXCEPTIONS)) && !defined(WIL_SUPPRESS_EXCEPTIONS)
/** This define is automatically set when exceptions are enabled within wil.
It is automatically defined when your code is compiled with exceptions enabled (via checking for the built-in
_CPPUNWIND or __EXCEPTIONS flag) unless you explicitly define WIL_SUPPRESS_EXCEPTIONS ahead of including your first wil
header.  All exception-based WIL methods and classes are included behind:
~~~~
#ifdef WIL_ENABLE_EXCEPTIONS
// code
#endif
~~~~
This enables exception-free code to directly include WIL headers without worrying about exception-based
routines suddenly becoming available. */
#define WIL_ENABLE_EXCEPTIONS
#endif
/// @endcond

/// @cond
#if defined(WIL_EXCEPTION_MODE)
static_assert(WIL_EXCEPTION_MODE <= 2, "Invalid exception mode");
#elif !defined(WIL_LOCK_EXCEPTION_MODE)
#define WIL_EXCEPTION_MODE 0            // default, can link exception-based and non-exception based libraries together
#pragma detect_mismatch("ODR_violation_WIL_EXCEPTION_MODE_mismatch", "0")
#elif defined(WIL_ENABLE_EXCEPTIONS)
#define WIL_EXCEPTION_MODE 1            // new code optimization:  ONLY support linking libraries together that have exceptions enabled
#pragma detect_mismatch("ODR_violation_WIL_EXCEPTION_MODE_mismatch", "1")
#else
#define WIL_EXCEPTION_MODE 2            // old code optimization:  ONLY support linking libraries that are NOT using exceptions
#pragma detect_mismatch("ODR_violation_WIL_EXCEPTION_MODE_mismatch", "2")
#endif

#if WIL_EXCEPTION_MODE == 1 && !defined(WIL_ENABLE_EXCEPTIONS)
#error Must enable exceptions when WIL_EXCEPTION_MODE == 1
#endif

// block for documentation only
#if defined(WIL_DOXYGEN)
/** This define can be explicitly set to disable exception usage within wil.
Normally this define is never needed as the WIL_ENABLE_EXCEPTIONS macro is enabled automatically by looking
at _CPPUNWIND.  If your code compiles with exceptions enabled, but does not want to enable the exception-based
classes and methods from WIL, define this macro ahead of including the first WIL header. */
#define WIL_SUPPRESS_EXCEPTIONS

/** This define can be explicitly set to lock the process exception mode to WIL_ENABLE_EXCEPTIONS.
Locking the exception mode provides optimizations to exception barriers, staging hooks and DLL load costs as it eliminates the need to
do copy-on-write initialization of various function pointers and the necessary indirection that's done within WIL to avoid ODR violations
when linking libraries together with different exception handling semantics. */
#define WIL_LOCK_EXCEPTION_MODE

/** This define explicit sets the exception mode for the process to control optimizations.
Three exception modes are available:
0)  This is the default.  This enables a binary to link both exception-based and non-exception based libraries together that
    use WIL.  This adds overhead to exception barriers, DLL copy on write pages and indirection through function pointers to avoid ODR
    violations when linking libraries together with different exception handling semantics.
1)  Prefer this setting when it can be used.  This locks the binary to only supporting libraries which were built with exceptions enabled.
2)  This locks the binary to libraries built without exceptions. */
#define WIL_EXCEPTION_MODE
#endif

#if (__cplusplus >= 201703) || (_MSVC_LANG >= 201703)
#define WIL_HAS_CXX_17 1
#else
#define WIL_HAS_CXX_17 0
#endif

// Until we'll have C++17 enabled in our code base, we're falling back to SAL
#define WI_NODISCARD __WI_LIBCPP_NODISCARD_ATTRIBUTE

#define __R_ENABLE_IF_IS_CLASS(ptrType)                     wistd::enable_if_t<wistd::is_class<ptrType>::value, void*> = nullptr
#define __R_ENABLE_IF_IS_NOT_CLASS(ptrType)                 wistd::enable_if_t<!wistd::is_class<ptrType>::value, void*> = nullptr

//! @defgroup bitwise Bitwise Inspection and Manipulation
//! Bitwise helpers to improve readability and reduce the error rate of bitwise operations.
//! Several macros have been constructed to assist with bitwise inspection and manipulation.  These macros exist
//! for two primary purposes:
//!
//! 1. To improve the readability of bitwise comparisons and manipulation.
//!
//!    The macro names are the more concise, readable form of what's being done and do not require that any flags
//!    or variables be specified multiple times for the comparisons.
//!
//! 2. To reduce the error rate associated with bitwise operations.
//!
//!    The readability improvements naturally lend themselves to this by cutting down the number of concepts.
//!    Using `WI_IsFlagSet(var, MyEnum::Flag)` rather than `((var & MyEnum::Flag) == MyEnum::Flag)` removes the comparison
//!    operator and repetition in the flag value.
//!
//!    Additionally, these macros separate single flag operations (which tend to be the most common) from multi-flag
//!    operations so that compile-time errors are generated for bitwise operations which are likely incorrect,
//!    such as:  `WI_IsFlagSet(var, MyEnum::None)` or `WI_IsFlagSet(var, MyEnum::ValidMask)`.
//!
//! Note that the single flag helpers should be used when a compile-time constant single flag is being manipulated.  These
//! helpers provide compile-time errors on misuse and should be preferred over the multi-flag helpers.  The multi-flag helpers
//! should be used when multiple flags are being used simultaneously or when the flag values are not compile-time constants.
//!
//! Common example usage (manipulation of flag variables):
//! ~~~~
//! WI_SetFlag(m_flags, MyFlags::Foo);                              // Set a single flag in the given variable
//! WI_SetAllFlags(m_flags, MyFlags::Foo | MyFlags::Bar);           // Set one or more flags
//! WI_ClearFlagIf(m_flags, MyFlags::Bar, isBarClosed);             // Conditionally clear a single flag based upon a bool
//! WI_ClearAllFlags(m_flags, MyFlags::Foo | MyFlags::Bar);         // Clear one or more flags from the given variable
//! WI_ToggleFlag(m_flags, MyFlags::Foo);                           // Toggle (change to the opposite value) a single flag
//! WI_UpdateFlag(m_flags, MyFlags::Bar, isBarClosed);              // Sets or Clears a single flag from the given variable based upon a bool value
//! WI_UpdateFlagsInMask(m_flags, flagsMask, newFlagValues);        // Sets or Clears the flags in flagsMask to the masked values from newFlagValues
//! ~~~~
//! Common example usage (inspection of flag variables):
//! ~~~~
//! if (WI_IsFlagSet(m_flags, MyFlags::Foo))                        // Is a single flag set in the given variable?
//! if (WI_IsAnyFlagSet(m_flags, MyFlags::Foo | MyFlags::Bar))      // Is at least one flag from the given mask set?
//! if (WI_AreAllFlagsClear(m_flags, MyFlags::Foo | MyFlags::Bar))  // Are all flags in the given list clear?
//! if (WI_IsSingleFlagSet(m_flags))                                // Is *exactly* one flag set in the given variable?
//! ~~~~
//! @{

//! Returns the unsigned type of the same width and numeric value as the given enum
#define WI_EnumValue(val)                                   static_cast<::wil::integral_from_enum<decltype(val)>>(val)
//! Validates that exactly ONE bit is set in compile-time constant `flag`
#define WI_StaticAssertSingleBitSet(flag)                   static_cast<decltype(flag)>(::wil::details::verify_single_flag_helper<static_cast<unsigned long long>(WI_EnumValue(flag))>::value)

//! @name Bitwise manipulation macros
//! @{

//! Set zero or more bitflags specified by `flags` in the variable `var`.
#define WI_SetAllFlags(var, flags)                          ((var) |= (flags))
//! Set a single compile-time constant `flag` in the variable `var`.
#define WI_SetFlag(var, flag)                               WI_SetAllFlags(var, WI_StaticAssertSingleBitSet(flag))
//! Conditionally sets a single compile-time constant `flag` in the variable `var` only if `condition` is true.
#define WI_SetFlagIf(var, flag, condition)                  do { if (wil::verify_bool(condition)) { WI_SetFlag(var, flag); } } while ((void)0, 0)

//! Clear zero or more bitflags specified by `flags` from the variable `var`.
#define WI_ClearAllFlags(var, flags)                        ((var) &= ~(flags))
//! Clear a single compile-time constant `flag` from the variable `var`.
#define WI_ClearFlag(var, flag)                             WI_ClearAllFlags(var, WI_StaticAssertSingleBitSet(flag))
//! Conditionally clear a single compile-time constant `flag` in the variable `var` only if `condition` is true.
#define WI_ClearFlagIf(var, flag, condition)                do { if (wil::verify_bool(condition)) { WI_ClearFlag(var, flag); } } while ((void)0, 0)

//! Changes a single compile-time constant `flag` in the variable `var` to be set if `isFlagSet` is true or cleared if `isFlagSet` is false.
#define WI_UpdateFlag(var, flag, isFlagSet)                 (wil::verify_bool(isFlagSet) ? WI_SetFlag(var, flag) : WI_ClearFlag(var, flag))
//! Changes only the flags specified by `flagsMask` in the variable `var` to match the corresponding flags in `newFlags`.
#define WI_UpdateFlagsInMask(var, flagsMask, newFlags)      wil::details::UpdateFlagsInMaskHelper(var, flagsMask, newFlags)

//! Toggles (XOR the value) of multiple bitflags specified by `flags` in the variable `var`.
#define WI_ToggleAllFlags(var, flags)                       ((var) ^= (flags))
//! Toggles (XOR the value) of a single compile-time constant `flag` in the variable `var`.
#define WI_ToggleFlag(var, flag)                            WI_ToggleAllFlags(var, WI_StaticAssertSingleBitSet(flag))
//! @}      // bitwise manipulation macros

//! @name Bitwise inspection macros
//! @{

//! Evaluates as true if every bitflag specified in `flags` is set within `val`.
#define WI_AreAllFlagsSet(val, flags)                       wil::details::AreAllFlagsSetHelper(val, flags)
//! Evaluates as true if one or more bitflags specified in `flags` are set within `val`.
#define WI_IsAnyFlagSet(val, flags)                         (static_cast<decltype((val) & (flags))>(WI_EnumValue(val) & WI_EnumValue(flags)) != static_cast<decltype((val) & (flags))>(0))
//! Evaluates as true if a single compile-time constant `flag` is set within `val`.
#define WI_IsFlagSet(val, flag)                             WI_IsAnyFlagSet(val, WI_StaticAssertSingleBitSet(flag))

//! Evaluates as true if every bitflag specified in `flags` is clear within `val`.
#define WI_AreAllFlagsClear(val, flags)                     (static_cast<decltype((val) & (flags))>(WI_EnumValue(val) & WI_EnumValue(flags)) == static_cast<decltype((val) & (flags))>(0))
//! Evaluates as true if one or more bitflags specified in `flags` are clear within `val`.
#define WI_IsAnyFlagClear(val, flags)                       (!wil::details::AreAllFlagsSetHelper(val, flags))
//! Evaluates as true if a single compile-time constant `flag` is clear within `val`.
#define WI_IsFlagClear(val, flag)                           WI_AreAllFlagsClear(val, WI_StaticAssertSingleBitSet(flag))

//! Evaluates as true if exactly one bit (any bit) is set within `val`.
#define WI_IsSingleFlagSet(val)                             wil::details::IsSingleFlagSetHelper(val)
//! Evaluates as true if exactly one bit from within the specified `mask` is set within `val`.
#define WI_IsSingleFlagSetInMask(val, mask)                 wil::details::IsSingleFlagSetHelper((val) & (mask))
//! Evaluates as true if exactly one bit (any bit) is set within `val` or if there are no bits set within `val`.
#define WI_IsClearOrSingleFlagSet(val)                      wil::details::IsClearOrSingleFlagSetHelper(val)
//! Evaluates as true if exactly one bit from within the specified `mask` is set within `val` or if there are no bits from `mask` set within `val`.
#define WI_IsClearOrSingleFlagSetInMask(val, mask)          wil::details::IsClearOrSingleFlagSetHelper((val) & (mask))
//! @}

#if defined(WIL_DOXYGEN)
/** This macro provides a C++ header with a guaranteed initialization function.
Normally, were a global object's constructor used for this purpose, the optimizer/linker might throw
the object away if it's unreferenced (which throws away the side-effects that the initialization function
was trying to achieve).  Using this macro forces linker inclusion of a variable that's initialized by the
provided function to elide that optimization.
//!
This functionality is primarily provided as a building block for header-based libraries (such as WIL)
to be able to layer additional functionality into other libraries by their mere inclusion.  Alternative models
of initialization should be used whenever they are available.
~~~~
#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
WI_HEADER_INITITALIZATION_FUNCTION(InitializeDesktopFamilyApis, []
{
    g_pfnGetModuleName              = GetCurrentModuleName;
    g_pfnFailFastInLoaderCallout    = FailFastInLoaderCallout;
    return 1;
});
#endif
~~~~
The above example is used within WIL to decide whether or not the library containing WIL is allowed to use
desktop APIs.  Building this functionality as #IFDEFs within functions would create ODR violations, whereas
doing it with global function pointers and header initialization allows a runtime determination. */
#define WI_HEADER_INITITALIZATION_FUNCTION(name, fn)
#elif defined(_M_IX86)
#define WI_HEADER_INITITALIZATION_FUNCTION(name, fn) \
    extern "C" { __declspec(selectany) unsigned char g_header_init_ ## name = static_cast<unsigned char>(fn()); } \
    __pragma(comment(linker, "/INCLUDE:_g_header_init_" #name))
#elif defined(_M_IA64) || defined(_M_AMD64) || defined(_M_ARM) || defined(_M_ARM64)
#define WI_HEADER_INITITALIZATION_FUNCTION(name, fn) \
    extern "C" { __declspec(selectany) unsigned char g_header_init_ ## name = static_cast<unsigned char>(fn()); } \
    __pragma(comment(linker, "/INCLUDE:g_header_init_" #name))
#else
    #error linker pragma must include g_header_init variation
#endif


/** All Windows Implementation Library classes and functions are located within the "wil" namespace.
The 'wil' namespace is an intentionally short name as the intent is for code to be able to reference
the namespace directly (example: `wil::srwlock lock;`) without a using statement.  Resist adding a using
statement for wil to avoid introducing potential name collisions between wil and other namespaces. */
namespace wil
{
    /// @cond
    namespace details
    {
        template <typename T>
        class pointer_range
        {
        public:
            pointer_range(T begin_, T end_) : m_begin(begin_), m_end(end_) {}
            WI_NODISCARD T begin() const  { return m_begin; }
            WI_NODISCARD T end() const    { return m_end; }
        private:
            T m_begin;
            T m_end;
        };
    }
    /// @endcond

    /** Enables using range-based for between a begin and end object pointer.
    ~~~~
    for (auto& obj : make_range(objPointerBegin, objPointerEnd)) { }
    ~~~~ */
    template <typename T>
    details::pointer_range<T> make_range(T begin, T end)
    {
        return details::pointer_range<T>(begin, end);
    }

    /** Enables using range-based for on a range when given the base pointer and the number of objects in the range.
    ~~~~
    for (auto& obj : make_range(objPointer, objCount)) { }
    ~~~~ */
    template <typename T>
    details::pointer_range<T> make_range(T begin, size_t count)
    {
        return details::pointer_range<T>(begin, begin + count);
    }


    //! @defgroup outparam Output Parameters
    //! Improve the conciseness of assigning values to optional output parameters.
    //! @{

    /** Assign the given value to an optional output parameter.
    Makes code more concise by removing trivial `if (outParam)` blocks. */
    template <typename T>
    inline void assign_to_opt_param(_Out_opt_ T *outParam, T val)
    {
        if (outParam != nullptr)
        {
            *outParam = val;
        }
    }

    /** Assign NULL to an optional output pointer parameter.
    Makes code more concise by removing trivial `if (outParam)` blocks. */
    template <typename T>
    inline void assign_null_to_opt_param(_Out_opt_ T *outParam)
    {
        if (outParam != nullptr)
        {
            *outParam = nullptr;
        }
    }
    //! @}      // end output parameter helpers

    /** Performs a logical or of the given variadic template parameters allowing indirect compile-time boolean evaluation.
    Example usage:
    ~~~~
    template <unsigned int... Rest>
    struct FeatureRequiredBy
    {
        static const bool enabled = wil::variadic_logical_or<WilFeature<Rest>::enabled...>::value;
    };
    ~~~~ */
    template <bool...> struct variadic_logical_or;
    /// @cond
    template <> struct variadic_logical_or<> : wistd::false_type { };
    template <bool... Rest> struct variadic_logical_or<true, Rest...> : wistd::true_type { };
    template <bool... Rest> struct variadic_logical_or<false, Rest...> : variadic_logical_or<Rest...>::type { };
    /// @endcond

    /// @cond
    namespace details
    {
        template <unsigned long long flag>
        struct verify_single_flag_helper
        {
            static_assert((flag != 0) && ((flag & (flag - 1)) == 0), "Single flag expected, zero or multiple flags found");
            static const unsigned long long value = flag;
        };
    }
    /// @endcond


    //! @defgroup typesafety Type Validation
    //! Helpers to validate variable types to prevent accidental, but allowed type conversions.
    //! These helpers are most useful when building macros that accept a particular type.  Putting these functions around the types accepted
    //! prior to pushing that type through to a function (or using it within the macro) allows the macro to add an additional layer of type
    //! safety that would ordinarily be stripped away by C++ implicit conversions.  This system is extensively used in the error handling helper
    //! macros to validate the types given to various macro parameters.
    //! @{

    /** Verify that `val` can be evaluated as a logical bool.
    Other types will generate an intentional compilation error.  Allowed types for a logical bool are bool, BOOL,
    boolean, BOOLEAN, and classes with an explicit bool cast.
    @param val The logical bool expression
    @return A C++ bool representing the evaluation of `val`. */
    template <typename T, __R_ENABLE_IF_IS_CLASS(T)>
    _Post_satisfies_(return == static_cast<bool>(val))
    __forceinline constexpr bool verify_bool(const T& val)
    {
        return static_cast<bool>(val);
    }

    template <typename T, __R_ENABLE_IF_IS_NOT_CLASS(T)>
    __forceinline constexpr bool verify_bool(T /*val*/)
    {
        static_assert(!wistd::is_same<T, T>::value, "Wrong Type: bool/BOOL/BOOLEAN/boolean expected");
        return false;
    }

    template <>
    _Post_satisfies_(return == val)
    __forceinline constexpr bool verify_bool<bool>(bool val)
    {
        return val;
    }

    template <>
    _Post_satisfies_(return == (val != 0))
    __forceinline constexpr bool verify_bool<int>(int val)
    {
        return (val != 0);
    }

    template <>
    _Post_satisfies_(return == (val != 0))
    __forceinline constexpr bool verify_bool<unsigned char>(unsigned char val)
    {
        return (val != 0);
    }

    /** Verify that `val` is a Win32 BOOL value.
    Other types (including other logical bool expressions) will generate an intentional compilation error.  Note that this will
    accept any `int` value as long as that is the underlying typedef behind `BOOL`.
    @param val The Win32 BOOL returning expression
    @return A Win32 BOOL representing the evaluation of `val`. */
    template <typename T>
    _Post_satisfies_(return == val)
    __forceinline constexpr int verify_BOOL(T val)
    {
        // Note: Written in terms of 'int' as BOOL is actually:  typedef int BOOL;
        static_assert((wistd::is_same<T, int>::value), "Wrong Type: BOOL expected");
        return val;
    }

    /** Verify that `hr` is an HRESULT value.
    Other types will generate an intentional compilation error.  Note that this will accept any `long` value as that is the
    underlying typedef behind HRESULT.
    //!
    Note that occasionally you might run into an HRESULT which is directly defined with a #define, such as:
    ~~~~
    #define UIA_E_NOTSUPPORTED   0x80040204
    ~~~~
    Though this looks like an `HRESULT`, this is actually an `unsigned long` (the hex specification forces this).  When
    these are encountered and they are NOT in the public SDK (have not yet shipped to the public), then you should change
    their definition to match the manner in which `HRESULT` constants are defined in winerror.h:
    ~~~~
    #define E_NOTIMPL            _HRESULT_TYPEDEF_(0x80004001L)
    ~~~~
    When these are encountered in the public SDK, their type should not be changed and you should use a static_cast
    to use this value in a macro that utilizes `verify_hresult`, for example:
    ~~~~
    RETURN_HR_IF(static_cast<HRESULT>(UIA_E_NOTSUPPORTED), (patternId != UIA_DragPatternId));
    ~~~~
    @param hr The HRESULT returning expression
    @return An HRESULT representing the evaluation of `val`. */
    template <typename T>
    _Post_satisfies_(return == hr)
    inline constexpr long verify_hresult(T hr)
    {
        // Note: Written in terms of 'long' as HRESULT is actually:  typedef _Return_type_success_(return >= 0) long HRESULT
        static_assert(wistd::is_same<T, long>::value, "Wrong Type: HRESULT expected");
        return hr;
    }

    /** Verify that `status` is an NTSTATUS value.
    Other types will generate an intentional compilation error.  Note that this will accept any `long` value as that is the
    underlying typedef behind NTSTATUS.
    //!
    Note that occasionally you might run into an NTSTATUS which is directly defined with a #define, such as:
    ~~~~
    #define STATUS_NOT_SUPPORTED             0x1
    ~~~~
    Though this looks like an `NTSTATUS`, this is actually an `unsigned long` (the hex specification forces this).  When
    these are encountered and they are NOT in the public SDK (have not yet shipped to the public), then you should change
    their definition to match the manner in which `NTSTATUS` constants are defined in ntstatus.h:
    ~~~~
    #define STATUS_NOT_SUPPORTED             ((NTSTATUS)0xC00000BBL)
    ~~~~
    When these are encountered in the public SDK, their type should not be changed and you should use a static_cast
    to use this value in a macro that utilizes `verify_ntstatus`, for example:
    ~~~~
    NT_RETURN_IF_FALSE(static_cast<NTSTATUS>(STATUS_NOT_SUPPORTED), (dispatch->Version == HKE_V1_0));
    ~~~~
    @param status The NTSTATUS returning expression
    @return An NTSTATUS representing the evaluation of `val`. */
    template <typename T>
    _Post_satisfies_(return == status)
    inline long verify_ntstatus(T status)
    {
        // Note: Written in terms of 'long' as NTSTATUS is actually:  typedef _Return_type_success_(return >= 0) long NTSTATUS
        static_assert(wistd::is_same<T, long>::value, "Wrong Type: NTSTATUS expected");
        return status;
    }

    /** Verify that `error` is a Win32 error code.
    Other types will generate an intentional compilation error. Note that this will accept any `long` value as that is
    the underlying type used for WIN32 error codes, as well as any `DWORD` (`unsigned long`) value since this is the type
    commonly used when manipulating Win32 error codes.
    @param error The Win32 error code returning expression
    @return An Win32 error code representing the evaluation of `error`. */
    template <typename T>
    _Post_satisfies_(return == error)
    inline T verify_win32(T error)
    {
        // Note: Win32 error code are defined as 'long' (#define ERROR_SUCCESS 0L), but are more frequently used as DWORD (unsigned long).
        // This accept both types.
        static_assert(wistd::is_same<T, long>::value || wistd::is_same<T, unsigned long>::value, "Wrong Type: Win32 error code (long / unsigned long) expected");
        return error;
    }
    /// @}      // end type validation routines

    /// @cond
    // Implementation details for macros and helper functions... do not use directly.
    namespace details
    {
        // Use size-specific casts to avoid sign extending numbers -- avoid warning C4310: cast truncates constant value
        #define __WI_MAKE_UNSIGNED(val) \
            (__pragma(warning(push)) __pragma(warning(disable: 4310 4309)) (sizeof(val) == 1 ? static_cast<unsigned char>(val) : \
                                                                            sizeof(val) == 2 ? static_cast<unsigned short>(val) : \
                                                                            sizeof(val) == 4 ? static_cast<unsigned long>(val) :  \
                                                                            static_cast<unsigned long long>(val)) __pragma(warning(pop)))
        #define __WI_IS_UNSIGNED_SINGLE_FLAG_SET(val) ((val) && !((val) & ((val) - 1)))
        #define __WI_IS_SINGLE_FLAG_SET(val) __WI_IS_UNSIGNED_SINGLE_FLAG_SET(__WI_MAKE_UNSIGNED(val))

        template <typename TVal, typename TFlags>
        __forceinline constexpr bool AreAllFlagsSetHelper(TVal val, TFlags flags)
        {
            return ((val & flags) == static_cast<decltype(val & flags)>(flags));
        }

        template <typename TVal>
        __forceinline constexpr bool IsSingleFlagSetHelper(TVal val)
        {
            return __WI_IS_SINGLE_FLAG_SET(val);
        }

        template <typename TVal>
        __forceinline constexpr bool IsClearOrSingleFlagSetHelper(TVal val)
        {
            return ((val == static_cast<wistd::remove_reference_t<TVal>>(0)) || IsSingleFlagSetHelper(val));
        }

        template <typename TVal, typename TMask, typename TFlags>
        __forceinline constexpr void UpdateFlagsInMaskHelper(_Inout_ TVal& val, TMask mask, TFlags flags)
        {
            val = static_cast<wistd::remove_reference_t<TVal>>((val & ~mask) | (flags & mask));
        }

        template <long>
        struct variable_size;

        template <>
        struct variable_size<1>
        {
            using type = unsigned char;
        };

        template <>
        struct variable_size<2>
        {
            using type = unsigned short;
        };

        template <>
        struct variable_size<4>
        {
            using type = unsigned long;
        };

        template <>
        struct variable_size<8>
        {
            using type = unsigned long long;
        };

        template <typename T>
        struct variable_size_mapping
        {
            using type = typename variable_size<sizeof(T)>::type;
        };
    } // details
    /// @endcond

    /** Defines the unsigned type of the same width (1, 2, 4, or 8 bytes) as the given type.
    This allows code to generically convert any enum class to it's corresponding underlying type. */
    template <typename T>
    using integral_from_enum = typename details::variable_size_mapping<T>::type;

    //! Declares a name that intentionally hides a name from an outer scope.
    //! Use this to prevent accidental use of a parameter or lambda captured variable.
    using hide_name = void(struct hidden_name);
} // wil

#pragma warning(pop)

#endif // __cplusplus
#endif // __WIL_COMMON_INCLUDED
