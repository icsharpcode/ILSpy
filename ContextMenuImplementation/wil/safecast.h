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
#ifndef __WIL_SAFECAST_INCLUDED
#define __WIL_SAFECAST_INCLUDED

#include "result_macros.h"
#include <intsafe.h>
#include "wistd_config.h"
#include "wistd_type_traits.h"

namespace wil
{
    namespace details
    {
        // Default error case for undefined conversions in intsafe.h
        template<typename OldT, typename NewT> constexpr wistd::nullptr_t intsafe_conversion = nullptr;

        // is_known_safe_static_cast_v determines if a conversion is known to be safe or not. Known
        // safe conversions can be handled by static_cast, this includes conversions between the same
        // type, when the new type is larger than the old type but is not a signed to unsigned
        // conversion, and when the two types are the same size and signed/unsigned. All other
        // conversions will be assumed to be potentially unsafe, and the conversion must be handled
        // by intsafe and checked.
        template <typename NewT, typename OldT>
        constexpr bool is_known_safe_static_cast_v =
            (sizeof(NewT) > sizeof(OldT) && !(wistd::is_signed_v<OldT> && wistd::is_unsigned_v<NewT>)) ||
            (sizeof(NewT) == sizeof(OldT) && ((wistd::is_signed_v<NewT> && wistd::is_signed_v<OldT>) || (wistd::is_unsigned_v<NewT> && wistd::is_unsigned_v<OldT>)));

        // Helper template to determine that NewT and OldT are both integral types. The safe_cast
        // operation only supports conversions between integral types.
        template <typename NewT, typename OldT>
        constexpr bool both_integral_v = wistd::is_integral<NewT>::value && wistd::is_integral<OldT>::value;

        // Note on native wchar_t (__wchar_t):
        //      Intsafe.h does not currently handle native wchar_t. When compiling with /Zc:wchar_t-, this is fine as wchar_t is
        //      typedef'd to unsigned short. However, when compiling with /Zc:wchar_t or wchar_t as a native type, the lack of
        //      support for native wchar_t in intsafe.h becomes an issue. To work around this, we treat native wchar_t as an
        //      unsigned short when passing it to intsafe.h, because the two on the Windows platform are the same size and
        //      share the same range according to MSDN. If the cast is to a native wchar_t, the result from intsafe.h is cast
        //      to a native wchar_t.

        // Intsafe does not have a defined conversion for native wchar_t
        template <typename NewT, typename OldT>
        constexpr bool neither_native_wchar_v = !wistd::is_same<NewT, __wchar_t>::value && !wistd::is_same<OldT, __wchar_t>::value;

        // Check to see if the cast is a conversion to native wchar_t
        template <typename NewT, typename OldT>
        constexpr bool is_cast_to_wchar_v = wistd::is_same<NewT, __wchar_t>::value && !wistd::is_same<OldT, __wchar_t>::value;

        // Check to see if the cast is a conversion from native wchar_t
        template <typename NewT, typename OldT>
        constexpr bool is_cast_from_wchar_v = !wistd::is_same<NewT, __wchar_t>::value && wistd::is_same<OldT, __wchar_t>::value;

        // Validate the conversion to be performed has a defined mapping to an intsafe conversion
        template <typename NewT, typename OldT>
        constexpr bool is_supported_intsafe_cast_v = intsafe_conversion<OldT, NewT> != nullptr;

        // True when the conversion is between integral types and can be handled by static_cast
        template <typename NewT, typename OldT>
        constexpr bool is_supported_safe_static_cast_v = both_integral_v<NewT, OldT> && is_known_safe_static_cast_v<NewT, OldT>;

        // True when the conversion is between integral types, does not involve native wchar, has
        // a mapped intsafe conversion, and is unsafe.
        template <typename NewT, typename OldT>
        constexpr bool is_supported_unsafe_cast_no_wchar_v =
            both_integral_v<NewT, OldT> &&
            !is_known_safe_static_cast_v<NewT, OldT> &&
            neither_native_wchar_v<NewT, OldT> &&
            is_supported_intsafe_cast_v<NewT, OldT>;

        // True when the conversion is between integral types, is a cast to native wchar_t, has
        // a mapped intsafe conversion, and is unsafe.
        template <typename NewT, typename OldT>
        constexpr bool is_supported_unsafe_cast_to_wchar_v =
            both_integral_v<NewT, OldT> &&
            !is_known_safe_static_cast_v<NewT, OldT> &&
            is_cast_to_wchar_v<NewT, OldT> &&
            is_supported_intsafe_cast_v<unsigned short, OldT>;

        // True when the conversion is between integral types, is a cast from native wchar_t, has
        // a mapped intsafe conversion, and is unsafe.
        template <typename NewT, typename OldT>
        constexpr bool is_supported_unsafe_cast_from_wchar_v =
            both_integral_v<NewT, OldT> &&
            !is_known_safe_static_cast_v<NewT, OldT> &&
            is_cast_from_wchar_v<NewT, OldT> &&
            is_supported_intsafe_cast_v<NewT, unsigned short>;

        // True when the conversion is supported and unsafe, and may or may not involve
        // native wchar_t.
        template <typename NewT, typename OldT>
        constexpr bool is_supported_unsafe_cast_v =
            is_supported_unsafe_cast_no_wchar_v<NewT, OldT> ||
            is_supported_unsafe_cast_to_wchar_v<NewT, OldT> ||
            is_supported_unsafe_cast_from_wchar_v<NewT, OldT>;

        // True when T is any one of the primitive types that the variably sized types are defined as.
        template <typename T>
        constexpr bool is_potentially_variably_sized_type_v =
            wistd::is_same<T, int>::value ||
            wistd::is_same<T, unsigned int>::value ||
            wistd::is_same<T, long>::value ||
            wistd::is_same<T, unsigned long>::value ||
            wistd::is_same<T, __int64>::value ||
            wistd::is_same<T, unsigned __int64>::value;

        // True when either type is potentialy variably sized (e.g. size_t, ptrdiff_t)
        template <typename OldT, typename NewT>
        constexpr bool is_potentially_variably_sized_cast_v =
            is_potentially_variably_sized_type_v<OldT> ||
            is_potentially_variably_sized_type_v<NewT>;

        // Mappings of all conversions defined in intsafe.h to intsafe_conversion
        // Note: Uppercase types (UINT, DWORD, SIZE_T, etc) and architecture dependent types resolve
        // to the base types. The base types are used since they do not vary based on architecture.
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<__int64, char> = LongLongToChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<__int64, int> = LongLongToInt;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<__int64, long> = LongLongToLong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<__int64, short> = LongLongToShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<__int64, signed char> = LongLongToInt8;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<__int64, unsigned __int64> = LongLongToULongLong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<__int64, unsigned char> = LongLongToUChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<__int64, unsigned int> = LongLongToUInt;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<__int64, unsigned long> = LongLongToULong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<__int64, unsigned short> = LongLongToUShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<int, char> = IntToChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<int, short> = IntToShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<int, signed char> = IntToInt8;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<int, unsigned __int64> = IntToULongLong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<int, unsigned char> = IntToUChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<int, unsigned int> = IntToUInt;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<int, unsigned long> = IntToULong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<int, unsigned short> = IntToUShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<long, char> = LongToChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<long, int> = LongToInt;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<long, short> = LongToShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<long, signed char> = LongToInt8;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<long, unsigned __int64> = LongToULongLong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<long, unsigned char> = LongToUChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<long, unsigned int> = LongToUInt;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<long, unsigned long> = LongToULong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<long, unsigned short> = LongToUShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<short, char> = ShortToChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<short, signed char> = ShortToInt8;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<short, unsigned __int64> = ShortToULongLong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<short, unsigned char> = ShortToUChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<short, unsigned int> = ShortToUInt;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<short, unsigned long> = ShortToULong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<short, unsigned short> = ShortToUShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<signed char, unsigned __int64> = Int8ToULongLong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<signed char, unsigned char> = Int8ToUChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<signed char, unsigned int> = Int8ToUInt;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<signed char, unsigned long> = Int8ToULong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<signed char, unsigned short> = Int8ToUShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned __int64, __int64> = ULongLongToLongLong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned __int64, char> = ULongLongToChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned __int64, int> = ULongLongToInt;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned __int64, long> = ULongLongToLong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned __int64, short> = ULongLongToShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned __int64, signed char> = ULongLongToInt8;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned __int64, unsigned char> = ULongLongToUChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned __int64, unsigned int> = ULongLongToUInt;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned __int64, unsigned long> = ULongLongToULong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned __int64, unsigned short> = ULongLongToUShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned char, char> = UInt8ToChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned char, signed char> = UIntToInt8;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned int, char> = UIntToChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned int, int> = UIntToInt;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned int, long> = UIntToLong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned int, short> = UIntToShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned int, signed char> = UIntToInt8;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned int, unsigned char> = UIntToUChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned int, unsigned short> = UIntToUShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned long, char> = ULongToChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned long, int> = ULongToInt;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned long, long> = ULongToLong;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned long, short> = ULongToShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned long, signed char> = ULongToInt8;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned long, unsigned char> = ULongToUChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned long, unsigned int> = ULongToUInt;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned long, unsigned short> = ULongToUShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned short, char> = UShortToChar;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned short, short> = UShortToShort;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned short, signed char> = UShortToInt8;
        template<> __WI_LIBCPP_INLINE_VAR constexpr auto intsafe_conversion<unsigned short, unsigned char> = UShortToUChar;
    }

    // Unsafe conversion where failure results in fail fast.
    template <
        typename NewT,
        typename OldT,
        wistd::enable_if_t<details::is_supported_unsafe_cast_no_wchar_v<NewT, OldT>, int> = 0
    >
    NewT safe_cast_failfast(const OldT var)
    {
        NewT newVar;
        FAIL_FAST_IF_FAILED((details::intsafe_conversion<OldT, NewT>(var, &newVar)));
        return newVar;
    }

    // Unsafe conversion where failure results in fail fast.
    template <
        typename NewT,
        typename OldT,
        wistd::enable_if_t<details::is_supported_unsafe_cast_from_wchar_v<NewT, OldT>, int> = 0
    >
    NewT safe_cast_failfast(const OldT var)
    {
        NewT newVar;
        FAIL_FAST_IF_FAILED((details::intsafe_conversion<unsigned short, NewT>(static_cast<unsigned short>(var), &newVar)));
        return newVar;
    }

    // Unsafe conversion where failure results in fail fast.
    template <
        typename NewT,
        typename OldT,
        wistd::enable_if_t<details::is_supported_unsafe_cast_to_wchar_v<NewT, OldT>, int> = 0
    >
    NewT safe_cast_failfast(const OldT var)
    {
        unsigned short newVar;
        FAIL_FAST_IF_FAILED((details::intsafe_conversion<OldT, unsigned short>(var, &newVar)));
        return static_cast<__wchar_t>(newVar);
    }

    // This conversion is always safe, therefore a static_cast is fine.
    template <
        typename NewT,
        typename OldT,
        wistd::enable_if_t<details::is_supported_safe_static_cast_v<NewT, OldT>, int> = 0
    >
    NewT safe_cast_failfast(const OldT var)
    {
        return static_cast<NewT>(var);
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    // Unsafe conversion where failure results in a thrown exception.
    template <
        typename NewT,
        typename OldT,
        wistd::enable_if_t<details::is_supported_unsafe_cast_no_wchar_v<NewT, OldT>, int> = 0
    >
    NewT safe_cast(const OldT var)
    {
        NewT newVar;
        THROW_IF_FAILED((details::intsafe_conversion<OldT, NewT>(var, &newVar)));
        return newVar;
    }

    // Unsafe conversion where failure results in a thrown exception.
    template <
        typename NewT,
        typename OldT,
        wistd::enable_if_t<details::is_supported_unsafe_cast_from_wchar_v<NewT, OldT>, int> = 0
    >
    NewT safe_cast(const OldT var)
    {
        NewT newVar;
        THROW_IF_FAILED((details::intsafe_conversion<unsigned short, NewT>(static_cast<unsigned short>(var), &newVar)));
        return newVar;
    }

    // Unsafe conversion where failure results in a thrown exception.
    template <
        typename NewT,
        typename OldT,
        wistd::enable_if_t<details::is_supported_unsafe_cast_to_wchar_v<NewT, OldT>, int> = 0
    >
    NewT safe_cast(const OldT var)
    {
        unsigned short newVar;
        THROW_IF_FAILED((details::intsafe_conversion<OldT, unsigned short>(var, &newVar)));
        return static_cast<__wchar_t>(newVar);
    }

    // This conversion is always safe, therefore a static_cast is fine.
    template <
        typename NewT,
        typename OldT,
        wistd::enable_if_t<details::is_supported_safe_static_cast_v<NewT, OldT>, int> = 0
    >
    NewT safe_cast(const OldT var)
    {
        return static_cast<NewT>(var);
    }
#endif

    // This conversion is unsafe, therefore the two parameter version of safe_cast_nothrow must be used
    template <
        typename NewT,
        typename OldT,
        wistd::enable_if_t<details::is_supported_unsafe_cast_v<NewT, OldT>, int> = 0
    >
    NewT safe_cast_nothrow(const OldT /*var*/)
    {
        static_assert(!wistd::is_same_v<NewT, NewT>, "This cast has the potential to fail, use the two parameter safe_cast_nothrow instead");
    }

    // This conversion is always safe, therefore a static_cast is fine.
    template <
        typename NewT,
        typename OldT,
        wistd::enable_if_t<details::is_supported_safe_static_cast_v<NewT, OldT>, int> = 0
    >
    NewT safe_cast_nothrow(const OldT var)
    {
        return static_cast<NewT>(var);
    }

    // Unsafe conversion where an HRESULT is returned. It is up to the callee to check and handle the HRESULT
    template <
        typename NewT,
        typename OldT,
        wistd::enable_if_t<details::is_supported_unsafe_cast_no_wchar_v<NewT, OldT>, int> = 0
    >
    HRESULT safe_cast_nothrow(const OldT var, NewT* newTResult)
    {
        return details::intsafe_conversion<OldT, NewT>(var, newTResult);
    }

    // Unsafe conversion where an HRESULT is returned. It is up to the callee to check and handle the HRESULT
    template <
        typename NewT,
        typename OldT,
        wistd::enable_if_t<details::is_supported_unsafe_cast_from_wchar_v<NewT, OldT>, int> = 0
    >
    HRESULT safe_cast_nothrow(const OldT var, NewT* newTResult)
    {
        return details::intsafe_conversion<unsigned short, NewT>(static_cast<unsigned short>(var), newTResult);
    }

    // Unsafe conversion where an HRESULT is returned. It is up to the callee to check and handle the HRESULT
    template <
        typename NewT,
        typename OldT,
        wistd::enable_if_t<details::is_supported_unsafe_cast_to_wchar_v<NewT, OldT>, int> = 0
    >
    HRESULT safe_cast_nothrow(const OldT var, NewT* newTResult)
    {
        return details::intsafe_conversion<OldT, unsigned short>(var, reinterpret_cast<unsigned short *>(newTResult));
    }

    // This conversion is always safe, therefore a static_cast is fine. If it can be determined the conversion
    // does not involve a variably sized type, then the compilation will fail and say the single parameter version
    // of safe_cast_nothrow should be used instead.
    template <
        typename NewT,
        typename OldT,
        wistd::enable_if_t<details::is_supported_safe_static_cast_v<NewT, OldT>, int> = 0
    >
    HRESULT safe_cast_nothrow(const OldT var, NewT* newTResult)
    {
        static_assert(details::is_potentially_variably_sized_cast_v<OldT, NewT>, "This cast is always safe; use safe_cast_nothrow<T>(value) to avoid unnecessary error handling.");
        *newTResult = static_cast<NewT>(var);
        return S_OK;
    }
}

#endif // __WIL_SAFECAST_INCLUDED
