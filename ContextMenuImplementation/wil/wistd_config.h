// -*- C++ -*-
//===--------------------------- __config ---------------------------------===//
//
//                     The LLVM Compiler Infrastructure
//
// This file is dual licensed under the MIT and the University of Illinois Open
// Source Licenses. See LICENSE.TXT for details.
//
//===----------------------------------------------------------------------===//

// STL common functionality
//
// Some aspects of STL are core language concepts that should be used from all C++ code, regardless
// of whether exceptions are enabled in the component.  Common library code that expects to be used
// from exception-free components want these concepts, but including STL headers directly introduces
// friction as it requires components not using STL to declare their STL version.  Doing so creates
// ambiguity around whether STL use is safe in a particular component and implicitly brings in
// a long list of headers (including <new>) which can create further ambiguity around throwing new
// support (some routines pulled in may expect it).  Secondarily, pulling in these headers also has
// the potential to create naming conflicts or other implied dependencies.
//
// To promote the use of these core language concepts outside of STL-based binaries, this file is
// selectively pulling those concepts *directly* from corresponding STL headers.  The corresponding
// "std::" namespace STL functions and types should be preferred over these in code that is bound to
// STL.  The implementation and naming of all functions are taken directly from STL, instead using
// "wistd" (Windows Implementation std) as the namespace.
//
// Routines in this namespace should always be considered a reflection of the *current* STL implementation
// of those routines.  Updates from STL should be taken, but no "bugs" should be fixed here.
//
// New, exception-based code should not use this namespace, but instead should prefer the std:: implementation.
// Only code that is not exception-based and libraries that expect to be utilized across both exception
// and non-exception based code should utilize this functionality.

// This header mimics libc++'s '__config' header to the extent necessary to get the wistd::* definitions compiling. Note
// that this has a few key differences since libc++'s MSVC compatability is currently not functional and a bit behind

#ifndef _WISTD_CONFIG_H_
#define _WISTD_CONFIG_H_

// DO NOT add *any* additional includes to this file -- there should be no dependencies from its usage
#include <cstddef> // For size_t and other necessary types

/// @cond
#if defined(_MSC_VER) && !defined(__clang__)
#  if !defined(__WI_LIBCPP_HAS_NO_PRAGMA_SYSTEM_HEADER)
#    define __WI_LIBCPP_HAS_NO_PRAGMA_SYSTEM_HEADER
#  endif
#endif

#ifndef __WI_LIBCPP_HAS_NO_PRAGMA_SYSTEM_HEADER
#pragma GCC system_header
#endif

#ifdef __GNUC__
#  define __WI_GNUC_VER (__GNUC__ * 100 + __GNUC_MINOR__)
// The __WI_GNUC_VER_NEW macro better represents the new GCC versioning scheme
// introduced in GCC 5.0.
#  define __WI_GNUC_VER_NEW (__WI_GNUC_VER * 10 + __GNUC_PATCHLEVEL__)
#else
#  define __WI_GNUC_VER 0
#  define __WI_GNUC_VER_NEW 0
#endif

// _MSVC_LANG is the more accurate way to get the C++ version in MSVC
#if defined(_MSVC_LANG) && (_MSVC_LANG > __cplusplus)
#define __WI_CPLUSPLUS _MSVC_LANG
#else
#define __WI_CPLUSPLUS __cplusplus
#endif

#ifndef __WI_LIBCPP_STD_VER
#  if  __WI_CPLUSPLUS <= 201103L
#    define __WI_LIBCPP_STD_VER 11
#  elif __WI_CPLUSPLUS <= 201402L
#    define __WI_LIBCPP_STD_VER 14
#  elif __WI_CPLUSPLUS <= 201703L
#    define __WI_LIBCPP_STD_VER 17
#  else
#    define __WI_LIBCPP_STD_VER 18  // current year, or date of c++2a ratification
#  endif
#endif  // __WI_LIBCPP_STD_VER

#if __WI_CPLUSPLUS < 201103L
#define __WI_LIBCPP_CXX03_LANG
#endif

#if defined(__ELF__)
#  define __WI_LIBCPP_OBJECT_FORMAT_ELF   1
#elif defined(__MACH__)
#  define __WI_LIBCPP_OBJECT_FORMAT_MACHO 1
#elif defined(_WIN32)
#  define __WI_LIBCPP_OBJECT_FORMAT_COFF  1
#elif defined(__wasm__)
#  define __WI_LIBCPP_OBJECT_FORMAT_WASM  1
#else
#  error Unknown object file format
#endif

#if defined(__clang__)
#  define __WI_LIBCPP_COMPILER_CLANG
#elif defined(__GNUC__)
#  define __WI_LIBCPP_COMPILER_GCC
#elif defined(_MSC_VER)
#  define __WI_LIBCPP_COMPILER_MSVC
#elif defined(__IBMCPP__)
#  define __WI_LIBCPP_COMPILER_IBM
#endif

#if defined(__WI_LIBCPP_COMPILER_MSVC)
#define __WI_PUSH_WARNINGS  __pragma(warning(push))
#define __WI_POP_WARNINGS   __pragma(warning(pop))
#elif defined(__WI_LIBCPP_COMPILER_CLANG)
#define __WI_PUSH_WARNINGS  __pragma(clang diagnostic push)
#define __WI_POP_WARNINGS   __pragma(clang diagnostic pop)
#else
#define __WI_PUSH_WARNINGS
#define __WI_POP_WARNINGS
#endif

#ifdef __WI_LIBCPP_COMPILER_MSVC
#define __WI_MSVC_DISABLE_WARNING(id)   __pragma(warning(disable: id))
#else
#define __WI_MSVC_DISABLE_WARNING(id)
#endif

#ifdef __WI_LIBCPP_COMPILER_CLANG
#define __WI_CLANG_DISABLE_WARNING(warning) __pragma(clang diagnostic ignored #warning)
#else
#define __WI_CLANG_DISABLE_WARNING(warning)
#endif

// NOTE: MSVC, which is what we primarily target, is severly underrepresented in libc++ and checks such as
// __has_feature(...) are always false for MSVC, even when the feature being tested _is_ present in MSVC. Therefore, we
// instead modify all checks to be __WI_HAS_FEATURE_IS_UNION, etc., which provides the correct value for MSVC and falls
// back to the __has_feature(...), etc. value otherwise. We intentionally leave '__has_feature', etc. undefined for MSVC
// so that we don't accidentally use the incorrect behavior
#ifndef __WI_LIBCPP_COMPILER_MSVC

#ifndef __has_feature
#define __has_feature(__x) 0
#endif

// '__is_identifier' returns '0' if '__x' is a reserved identifier provided by
// the compiler and '1' otherwise.
#ifndef __is_identifier
#define __is_identifier(__x) 1
#endif

#ifndef __has_cpp_attribute
#define __has_cpp_attribute(__x) 0
#endif

#ifndef __has_attribute
#define __has_attribute(__x) 0
#endif

#ifndef __has_builtin
#define __has_builtin(__x) 0
#endif

#if __has_feature(cxx_alignas)
#  define __WI_ALIGNAS_TYPE(x) alignas(x)
#  define __WI_ALIGNAS(x) alignas(x)
#else
#  define __WI_ALIGNAS_TYPE(x) __attribute__((__aligned__(__alignof(x))))
#  define __WI_ALIGNAS(x) __attribute__((__aligned__(x)))
#endif

#if __has_feature(cxx_explicit_conversions) || defined(__IBMCPP__) || \
    (!defined(__WI_LIBCPP_CXX03_LANG) && defined(__GNUC__)) // All supported GCC versions
#  define __WI_LIBCPP_EXPLICIT explicit
#else
#  define __WI_LIBCPP_EXPLICIT
#endif

#if __has_feature(cxx_attributes)
#  define __WI_LIBCPP_NORETURN [[noreturn]]
#else
#  define __WI_LIBCPP_NORETURN __attribute__ ((noreturn))
#endif

#define __WI_LIBCPP_SUPPRESS_NONINIT_ANALYSIS
#define __WI_LIBCPP_SUPPRESS_NOEXCEPT_ANALYSIS

// The __WI_LIBCPP_NODISCARD_ATTRIBUTE should only be used to define other
// NODISCARD macros to the correct attribute.
#if __has_cpp_attribute(nodiscard)
#  define __WI_LIBCPP_NODISCARD_ATTRIBUTE [[nodiscard]]
#elif defined(__WI_LIBCPP_COMPILER_CLANG) && !defined(__WI_LIBCPP_CXX03_LANG)
#  define __WI_LIBCPP_NODISCARD_ATTRIBUTE [[clang::warn_unused_result]]
#else
// We can't use GCC's [[gnu::warn_unused_result]] and
// __attribute__((warn_unused_result)), because GCC does not silence them via
// (void) cast.
#  define __WI_LIBCPP_NODISCARD_ATTRIBUTE
#endif

#define __WI_HAS_FEATURE_IS_UNION __has_feature(is_union)
#define __WI_HAS_FEATURE_IS_CLASS __has_feature(is_class)
#define __WI_HAS_FEATURE_IS_ENUM __has_feature(is_enum)
#define __WI_HAS_FEATURE_IS_CONVERTIBLE_TO __has_feature(is_convertible_to)
#define __WI_HAS_FEATURE_IS_EMPTY __has_feature(is_empty)
#define __WI_HAS_FEATURE_IS_POLYMORPHIC __has_feature(is_polymorphic)
#define __WI_HAS_FEATURE_HAS_VIRTUAL_DESTRUCTOR __has_feature(has_virtual_destructor)
#define __WI_HAS_FEATURE_REFERENCE_QUALIFIED_FUNCTIONS __has_feature(cxx_reference_qualified_functions)
#define __WI_HAS_FEATURE_IS_CONSTRUCTIBLE __has_feature(is_constructible)
#define __WI_HAS_FEATURE_IS_TRIVIALLY_CONSTRUCTIBLE __has_feature(is_trivially_constructible)
#define __WI_HAS_FEATURE_IS_TRIVIALLY_ASSIGNABLE __has_feature(is_trivially_assignable)
#define __WI_HAS_FEATURE_HAS_TRIVIAL_DESTRUCTOR __has_feature(has_trivial_destructor)
#define __WI_HAS_FEATURE_NOEXCEPT __has_feature(cxx_noexcept)
#define __WI_HAS_FEATURE_IS_POD __has_feature(is_pod)
#define __WI_HAS_FEATURE_IS_STANDARD_LAYOUT __has_feature(is_standard_layout)
#define __WI_HAS_FEATURE_IS_TRIVIALLY_COPYABLE __has_feature(is_trivially_copyable)
#define __WI_HAS_FEATURE_IS_TRIVIAL __has_feature(is_trivial)
#define __WI_HAS_FEATURE_HAS_TRIVIAL_CONSTRUCTOR __has_feature(has_trivial_constructor) || (__WI_GNUC_VER >= 403)
#define __WI_HAS_FEATURE_HAS_NOTHROW_CONSTRUCTOR __has_feature(has_nothrow_constructor) || (__WI_GNUC_VER >= 403)
#define __WI_HAS_FEATURE_HAS_NOTHROW_COPY __has_feature(has_nothrow_copy) || (__WI_GNUC_VER >= 403)
#define __WI_HAS_FEATURE_HAS_NOTHROW_ASSIGN __has_feature(has_nothrow_assign) || (__WI_GNUC_VER >= 403)

#if !(__has_feature(cxx_noexcept))
#define __WI_LIBCPP_HAS_NO_NOEXCEPT
#endif

#if !__is_identifier(__has_unique_object_representations) || __WI_GNUC_VER >= 700
#define __WI_LIBCPP_HAS_UNIQUE_OBJECT_REPRESENTATIONS
#endif

#if !(__has_feature(cxx_variadic_templates))
#define __WI_LIBCPP_HAS_NO_VARIADICS
#endif

#if __has_feature(is_literal) || __WI_GNUC_VER >= 407
#define __WI_LIBCPP_IS_LITERAL(T) __is_literal(T)
#endif

#if __has_feature(underlying_type) || __WI_GNUC_VER >= 407
#define __WI_LIBCPP_UNDERLYING_TYPE(T) __underlying_type(T)
#endif

#if __has_feature(is_final) || __WI_GNUC_VER >= 407
#define __WI_LIBCPP_HAS_IS_FINAL
#endif

#if __has_feature(is_base_of) || defined(__GNUC__) && __WI_GNUC_VER >= 403
#define __WI_LIBCPP_HAS_IS_BASE_OF
#endif

#if __is_identifier(__is_aggregate) && (__WI_GNUC_VER_NEW < 7001)
#define __WI_LIBCPP_HAS_NO_IS_AGGREGATE
#endif

#if !(__has_feature(cxx_rtti)) && !defined(__WI_LIBCPP_NO_RTTI)
#define __WI_LIBCPP_NO_RTTI
#endif

#if !(__has_feature(cxx_variable_templates))
#define __WI_LIBCPP_HAS_NO_VARIABLE_TEMPLATES
#endif

#if !(__has_feature(cxx_relaxed_constexpr))
#define __WI_LIBCPP_HAS_NO_CXX14_CONSTEXPR
#endif

#if !__has_builtin(__builtin_addressof) && _GNUC_VER < 700
#define __WI_LIBCPP_HAS_NO_BUILTIN_ADDRESSOF
#endif

#if __has_attribute(__no_sanitize__) && !defined(__WI_LIBCPP_COMPILER_GCC)
#  define __WI_LIBCPP_NO_CFI __attribute__((__no_sanitize__("cfi")))
#else
#  define __WI_LIBCPP_NO_CFI
#endif

#define __WI_LIBCPP_ALWAYS_INLINE __attribute__ ((__always_inline__))

#if __has_attribute(internal_linkage)
#  define __WI_LIBCPP_INTERNAL_LINKAGE __attribute__ ((internal_linkage))
#else
#  define __WI_LIBCPP_INTERNAL_LINKAGE __WI_LIBCPP_ALWAYS_INLINE
#endif

#else

// NOTE: Much of the following assumes a decently recent version of MSVC. Past versions can be supported, but will need
//       to be updated to contain the proper _MSC_VER check
#define __WI_ALIGNAS_TYPE(x) alignas(x)
#define __WI_ALIGNAS(x) alignas(x)
#define __alignof__ __alignof

#define __WI_LIBCPP_EXPLICIT explicit
#define __WI_LIBCPP_NORETURN [[noreturn]]
#define __WI_LIBCPP_SUPPRESS_NONINIT_ANALYSIS __pragma(warning(suppress:26495))
#define __WI_LIBCPP_SUPPRESS_NOEXCEPT_ANALYSIS __pragma(warning(suppress:26439))


#if __WI_LIBCPP_STD_VER > 14
#define __WI_LIBCPP_NODISCARD_ATTRIBUTE [[nodiscard]]
#else
#define __WI_LIBCPP_NODISCARD_ATTRIBUTE _Check_return_
#endif

#define __WI_HAS_FEATURE_IS_UNION 1
#define __WI_HAS_FEATURE_IS_CLASS 1
#define __WI_HAS_FEATURE_IS_ENUM 1
#define __WI_HAS_FEATURE_IS_CONVERTIBLE_TO 1
#define __WI_HAS_FEATURE_IS_EMPTY 1
#define __WI_HAS_FEATURE_IS_POLYMORPHIC 1
#define __WI_HAS_FEATURE_HAS_VIRTUAL_DESTRUCTOR 1
#define __WI_LIBCPP_HAS_UNIQUE_OBJECT_REPRESENTATIONS 1
#define __WI_HAS_FEATURE_REFERENCE_QUALIFIED_FUNCTIONS 1
#define __WI_HAS_FEATURE_IS_CONSTRUCTIBLE 1
#define __WI_HAS_FEATURE_IS_TRIVIALLY_CONSTRUCTIBLE 1
#define __WI_HAS_FEATURE_IS_TRIVIALLY_ASSIGNABLE 1
#define __WI_HAS_FEATURE_HAS_TRIVIAL_DESTRUCTOR 1
#define __WI_HAS_FEATURE_NOEXCEPT 1
#define __WI_HAS_FEATURE_IS_POD 1
#define __WI_HAS_FEATURE_IS_STANDARD_LAYOUT 1
#define __WI_HAS_FEATURE_IS_TRIVIALLY_COPYABLE 1
#define __WI_HAS_FEATURE_IS_TRIVIAL 1
#define __WI_HAS_FEATURE_HAS_TRIVIAL_CONSTRUCTOR 1
#define __WI_HAS_FEATURE_HAS_NOTHROW_CONSTRUCTOR 1
#define __WI_HAS_FEATURE_HAS_NOTHROW_COPY 1
#define __WI_HAS_FEATURE_HAS_NOTHROW_ASSIGN 1
#define __WI_HAS_FEATURE_IS_DESTRUCTIBLE 1

#if !defined(_CPPRTTI) && !defined(__WI_LIBCPP_NO_RTTI)
#define __WI_LIBCPP_NO_RTTI
#endif

#define __WI_LIBCPP_IS_LITERAL(T) __is_literal_type(T)
#define __WI_LIBCPP_UNDERLYING_TYPE(T) __underlying_type(T)
#define __WI_LIBCPP_HAS_IS_FINAL
#define __WI_LIBCPP_HAS_IS_BASE_OF

#if __WI_LIBCPP_STD_VER < 14
#define __WI_LIBCPP_HAS_NO_VARIABLE_TEMPLATES
#endif

#define __WI_LIBCPP_HAS_NO_BUILTIN_ADDRESSOF
#define __WI_LIBCPP_NO_CFI

#define __WI_LIBCPP_ALWAYS_INLINE __forceinline
#define __WI_LIBCPP_INTERNAL_LINKAGE

#endif

#ifndef _WIN32

#ifdef __LITTLE_ENDIAN__
#  if __LITTLE_ENDIAN__
#    define __WI_LIBCPP_LITTLE_ENDIAN
#  endif  // __LITTLE_ENDIAN__
#endif  // __LITTLE_ENDIAN__

#ifdef __BIG_ENDIAN__
#  if __BIG_ENDIAN__
#    define __WI_LIBCPP_BIG_ENDIAN
#  endif  // __BIG_ENDIAN__
#endif  // __BIG_ENDIAN__

#ifdef __BYTE_ORDER__
#  if __BYTE_ORDER__ == __ORDER_LITTLE_ENDIAN__
#    define __WI_LIBCPP_LITTLE_ENDIAN
#  elif __BYTE_ORDER__ == __ORDER_BIG_ENDIAN__
#    define __WI_LIBCPP_BIG_ENDIAN
#  endif // __BYTE_ORDER__ == __ORDER_BIG_ENDIAN__
#endif // __BYTE_ORDER__

#if !defined(__WI_LIBCPP_LITTLE_ENDIAN) && !defined(__WI_LIBCPP_BIG_ENDIAN)
#  include <endian.h>
#  if __BYTE_ORDER == __LITTLE_ENDIAN
#    define __WI_LIBCPP_LITTLE_ENDIAN
#  elif __BYTE_ORDER == __BIG_ENDIAN
#    define __WI_LIBCPP_BIG_ENDIAN
#  else  // __BYTE_ORDER == __BIG_ENDIAN
#    error unable to determine endian
#  endif
#endif  // !defined(__WI_LIBCPP_LITTLE_ENDIAN) && !defined(__WI_LIBCPP_BIG_ENDIAN)

#else // _WIN32

#define __WI_LIBCPP_LITTLE_ENDIAN

#endif // _WIN32

#ifdef __WI_LIBCPP_HAS_NO_CONSTEXPR
#  define __WI_LIBCPP_CONSTEXPR
#else
#  define __WI_LIBCPP_CONSTEXPR constexpr
#endif

#if __WI_LIBCPP_STD_VER > 11 && !defined(__WI_LIBCPP_HAS_NO_CXX14_CONSTEXPR)
#  define __WI_LIBCPP_CONSTEXPR_AFTER_CXX11 constexpr
#else
#  define __WI_LIBCPP_CONSTEXPR_AFTER_CXX11
#endif

#if __WI_LIBCPP_STD_VER > 14 && !defined(__WI_LIBCPP_HAS_NO_CXX14_CONSTEXPR)
#  define __WI_LIBCPP_CONSTEXPR_AFTER_CXX14 constexpr
#else
#  define __WI_LIBCPP_CONSTEXPR_AFTER_CXX14
#endif

#if __WI_LIBCPP_STD_VER > 17 && !defined(__WI_LIBCPP_HAS_NO_CXX14_CONSTEXPR)
#  define __WI_LIBCPP_CONSTEXPR_AFTER_CXX17 constexpr
#else
#  define __WI_LIBCPP_CONSTEXPR_AFTER_CXX17
#endif

#if !defined(__WI_LIBCPP_DISABLE_NODISCARD_AFTER_CXX17) && \
    (__WI_LIBCPP_STD_VER > 17 || defined(__WI_LIBCPP_ENABLE_NODISCARD))
#  define __WI_LIBCPP_NODISCARD_AFTER_CXX17 __WI_LIBCPP_NODISCARD_ATTRIBUTE
#else
#  define __WI_LIBCPP_NODISCARD_AFTER_CXX17
#endif

#if __WI_LIBCPP_STD_VER > 14 && defined(__cpp_inline_variables) && (__cpp_inline_variables >= 201606L)
#  define __WI_LIBCPP_INLINE_VAR inline
#else
#  define __WI_LIBCPP_INLINE_VAR
#endif

#ifdef __WI_LIBCPP_CXX03_LANG
#define __WI_LIBCPP_HAS_NO_UNICODE_CHARS
#define __WI_LIBCPP_HAS_NO_RVALUE_REFERENCES
#endif

#ifndef __SIZEOF_INT128__
#define __WI_LIBCPP_HAS_NO_INT128
#endif

#if !__WI_HAS_FEATURE_NOEXCEPT && !defined(__WI_LIBCPP_HAS_NO_NOEXCEPT)
#define __WI_LIBCPP_HAS_NO_NOEXCEPT
#endif

#ifndef __WI_LIBCPP_HAS_NO_NOEXCEPT
#  define WI_NOEXCEPT noexcept
#  define __WI_NOEXCEPT_(x) noexcept(x)
#else
#  define WI_NOEXCEPT throw()
#  define __WI_NOEXCEPT_(x)
#endif

#if defined(__WI_LIBCPP_OBJECT_FORMAT_COFF)
#define __WI_LIBCPP_HIDDEN
#define __WI_LIBCPP_TEMPLATE_VIS
#endif // defined(__WI_LIBCPP_OBJECT_FORMAT_COFF)

#ifndef __WI_LIBCPP_HIDDEN
#  if !defined(__WI_LIBCPP_DISABLE_VISIBILITY_ANNOTATIONS)
#    define __WI_LIBCPP_HIDDEN __attribute__ ((__visibility__("hidden")))
#  else
#    define __WI_LIBCPP_HIDDEN
#  endif
#endif

#ifndef __WI_LIBCPP_TEMPLATE_VIS
#  if !defined(__WI_LIBCPP_DISABLE_VISIBILITY_ANNOTATIONS) && !defined(__WI_LIBCPP_COMPILER_MSVC)
#    if __has_attribute(__type_visibility__)
#      define __WI_LIBCPP_TEMPLATE_VIS __attribute__ ((__type_visibility__("default")))
#    else
#      define __WI_LIBCPP_TEMPLATE_VIS __attribute__ ((__visibility__("default")))
#    endif
#  else
#    define __WI_LIBCPP_TEMPLATE_VIS
#  endif
#endif

#define __WI_LIBCPP_INLINE_VISIBILITY __WI_LIBCPP_HIDDEN __WI_LIBCPP_INTERNAL_LINKAGE

namespace wistd     // ("Windows Implementation" std)
{
     using nullptr_t = decltype(__nullptr);

     template <class _T1, class _T2 = _T1>
     struct __less
     {
     __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY __WI_LIBCPP_CONSTEXPR_AFTER_CXX11
     bool operator()(const _T1& __x, const _T1& __y) const {return __x < __y;}

     __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY __WI_LIBCPP_CONSTEXPR_AFTER_CXX11
     bool operator()(const _T1& __x, const _T2& __y) const {return __x < __y;}

     __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY __WI_LIBCPP_CONSTEXPR_AFTER_CXX11
     bool operator()(const _T2& __x, const _T1& __y) const {return __x < __y;}

     __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY __WI_LIBCPP_CONSTEXPR_AFTER_CXX11
     bool operator()(const _T2& __x, const _T2& __y) const {return __x < __y;}
     };

     template <class _T1>
     struct __less<_T1, _T1>
     {
     __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY __WI_LIBCPP_CONSTEXPR_AFTER_CXX11
     bool operator()(const _T1& __x, const _T1& __y) const {return __x < __y;}
     };

     template <class _T1>
     struct __less<const _T1, _T1>
     {
     __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY __WI_LIBCPP_CONSTEXPR_AFTER_CXX11
     bool operator()(const _T1& __x, const _T1& __y) const {return __x < __y;}
     };

     template <class _T1>
     struct __less<_T1, const _T1>
     {
     __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY __WI_LIBCPP_CONSTEXPR_AFTER_CXX11
     bool operator()(const _T1& __x, const _T1& __y) const {return __x < __y;}
     };

     // These are added to wistd to enable use of min/max without having to use the windows.h min/max
     // macros that some clients might not have access to. Note: the STL versions of these have debug
     // checking for the less than operator and support for iterators that these implementations lack.
     // Use the STL versions when you require use of those features.

     // min

     template <class _Tp, class _Compare>
     inline __WI_LIBCPP_INLINE_VISIBILITY __WI_LIBCPP_CONSTEXPR_AFTER_CXX11
     const _Tp&
     (min)(const _Tp& __a, const _Tp& __b, _Compare __comp)
     {
     return __comp(__b, __a) ? __b : __a;
     }

     template <class _Tp>
     inline __WI_LIBCPP_INLINE_VISIBILITY __WI_LIBCPP_CONSTEXPR_AFTER_CXX11
     const _Tp&
     (min)(const _Tp& __a, const _Tp& __b)
     {
     return (min)(__a, __b, __less<_Tp>());
     }

     // max

     template <class _Tp, class _Compare>
     inline __WI_LIBCPP_INLINE_VISIBILITY __WI_LIBCPP_CONSTEXPR_AFTER_CXX11
     const _Tp&
     (max)(const _Tp& __a, const _Tp& __b, _Compare __comp)
     {
     return __comp(__a, __b) ? __b : __a;
     }

     template <class _Tp>
     inline __WI_LIBCPP_INLINE_VISIBILITY __WI_LIBCPP_CONSTEXPR_AFTER_CXX11
     const _Tp&
     (max)(const _Tp& __a, const _Tp& __b)
     {
     return (max)(__a, __b, __less<_Tp>());
     }

    template <class _Arg, class _Result>
    struct __WI_LIBCPP_TEMPLATE_VIS unary_function
    {
        using argument_type = _Arg;
        using result_type = _Result;
    };

    template <class _Arg1, class _Arg2, class _Result>
    struct __WI_LIBCPP_TEMPLATE_VIS binary_function
    {
        using first_argument_type = _Arg1;
        using second_argument_type = _Arg2;
        using result_type = _Result;
    };
}
/// @endcond

#endif // _WISTD_CONFIG_H_
