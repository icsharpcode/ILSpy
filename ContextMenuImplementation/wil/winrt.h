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
#ifndef __WIL_WINRT_INCLUDED
#define __WIL_WINRT_INCLUDED

#include <hstring.h>
#include <wrl\client.h>
#include <wrl\implements.h>
#include <wrl\async.h>
#include <wrl\wrappers\corewrappers.h>
#include "result.h"
#include "com.h"
#include "resource.h"
#include <windows.foundation.h>
#include <windows.foundation.collections.h>

#ifdef __cplusplus_winrt
#include <collection.h> // bring in the CRT iterator for support for C++ CX code
#endif

/// @cond
#if defined(WIL_ENABLE_EXCEPTIONS) && !defined(__WI_HAS_STD_LESS)
#ifdef __has_include
#if __has_include(<functional>)
#define __WI_HAS_STD_LESS 1
#include <functional>
#endif // Otherwise, not using STL; don't specialize std::less
#else
// Fall back to the old way of forward declaring std::less
#define __WI_HAS_STD_LESS 1
#pragma warning(push)
#pragma warning(disable:4643) // Forward declaring '...' in namespace std is not permitted by the C++ Standard.
namespace std
{
    template<class _Ty>
    struct less;
}
#pragma warning(pop)
#endif
#endif
#if defined(WIL_ENABLE_EXCEPTIONS) && defined(__has_include)
#if __has_include(<vector>)
#define __WI_HAS_STD_VECTOR 1
#include <vector>
#endif
#endif
/// @endcond

// This enables this code to be used in code that uses the ABI prefix or not.
// Code using the public SDK and C++ CX code has the ABI prefix, windows internal
// is built in a way that does not.
#if !defined(MIDL_NS_PREFIX) && !defined(____x_ABI_CWindows_CFoundation_CIClosable_FWD_DEFINED__)
// Internal .idl files use the namespace without the ABI prefix. Macro out ABI for that case
#pragma push_macro("ABI")
#undef ABI
#define ABI
#endif

namespace wil
{
    // time_t is the number of 1 - second intervals since January 1, 1970.
    constexpr long long SecondsToStartOf1970 = 0x2b6109100;
    constexpr long long HundredNanoSecondsInSecond = 10000000LL;

    inline __time64_t DateTime_to_time_t(ABI::Windows::Foundation::DateTime dateTime)
    {
        // DateTime is the number of 100 - nanosecond intervals since January 1, 1601.
        return (dateTime.UniversalTime / HundredNanoSecondsInSecond - SecondsToStartOf1970);
    }

    inline ABI::Windows::Foundation::DateTime time_t_to_DateTime(__time64_t timeT)
    {
        ABI::Windows::Foundation::DateTime dateTime;
        dateTime.UniversalTime = (timeT + SecondsToStartOf1970) * HundredNanoSecondsInSecond;
        return dateTime;
    }

#pragma region HSTRING Helpers
    /// @cond
    namespace details
    {
        // hstring_compare is used to assist in HSTRING comparison of two potentially non-similar string types. E.g.
        // comparing a raw HSTRING with WRL's HString/HStringReference/etc. The consumer can optionally inhibit the
        // deduction of array sizes by providing 'true' for the 'InhibitStringArrays' template argument. This is
        // generally done in scenarios where the consumer cannot guarantee that the input argument types are perfectly
        // preserved from end-to-end. E.g. if a single function in the execution path captures an array as const T&,
        // then it is impossible to differentiate const arrays (where we generally do want to deduce length) from
        // non-const arrays (where we generally do not want to deduce length). The consumer can also optionally choose
        // to perform case-insensitive comparison by providing 'true' for the 'IgnoreCase' template argument.
        template <bool InhibitStringArrays, bool IgnoreCase>
        struct hstring_compare
        {
            // get_buffer returns the string buffer and length for the supported string types
            static const wchar_t* get_buffer(HSTRING hstr, UINT32* length) WI_NOEXCEPT
            {
                return ::WindowsGetStringRawBuffer(hstr, length);
            }

            static const wchar_t* get_buffer(const Microsoft::WRL::Wrappers::HString& hstr, UINT32* length) WI_NOEXCEPT
            {
                return hstr.GetRawBuffer(length);
            }

            static const wchar_t* get_buffer(
                const Microsoft::WRL::Wrappers::HStringReference& hstr,
                UINT32* length) WI_NOEXCEPT
            {
                return hstr.GetRawBuffer(length);
            }

            static const wchar_t* get_buffer(const unique_hstring& str, UINT32* length) WI_NOEXCEPT
            {
                return ::WindowsGetStringRawBuffer(str.get(), length);
            }

            template <bool..., bool Enable = InhibitStringArrays>
            static wistd::enable_if_t<Enable, const wchar_t*> get_buffer(const wchar_t* str, UINT32* length) WI_NOEXCEPT
            {
                str = (str != nullptr) ? str : L"";
                *length = static_cast<UINT32>(wcslen(str));
                return str;
            }

            template <typename StringT, bool..., bool Enable = !InhibitStringArrays>
            static wistd::enable_if_t<
                wistd::conjunction<
                    wistd::is_pointer<StringT>,
                    wistd::is_same<wistd::decay_t<wistd::remove_pointer_t<StringT>>, wchar_t>,
                    wistd::bool_constant<Enable>
                >::value,
            const wchar_t*> get_buffer(StringT str, UINT32* length) WI_NOEXCEPT
            {
                str = (str != nullptr) ? str : L"";
                *length = static_cast<UINT32>(wcslen(str));
                return str;
            }

            template <size_t Size, bool..., bool Enable = !InhibitStringArrays>
            static wistd::enable_if_t<Enable, const wchar_t*> get_buffer(
                const wchar_t (&str)[Size],
                UINT32* length) WI_NOEXCEPT
            {
                *length = Size - 1;
                return str;
            }

            template <size_t Size, bool..., bool Enable = !InhibitStringArrays>
            static wistd::enable_if_t<Enable, const wchar_t*> get_buffer(wchar_t (&str)[Size], UINT32* length) WI_NOEXCEPT
            {
                *length = static_cast<UINT32>(wcslen(str));
                return str;
            }

            // Overload for std::wstring, or at least things that behave like std::wstring, without adding a dependency
            // on STL headers
            template <typename StringT>
            static wistd::enable_if_t<wistd::conjunction_v<
                wistd::is_same<const wchar_t*, decltype(wistd::declval<StringT>().c_str())>,
                wistd::is_same<typename StringT::size_type, decltype(wistd::declval<StringT>().length())>>,
            const wchar_t*> get_buffer(const StringT& str, UINT32* length) WI_NOEXCEPT
            {
                *length = static_cast<UINT32>(str.length());
                return str.c_str();
            }

            template <typename LhsT, typename RhsT>
            static auto compare(LhsT&& lhs, RhsT&& rhs) ->
                decltype(get_buffer(lhs, wistd::declval<UINT32*>()), get_buffer(rhs, wistd::declval<UINT32*>()), int())
            {
                UINT32 lhsLength;
                UINT32 rhsLength;
                auto lhsBuffer = get_buffer(wistd::forward<LhsT>(lhs), &lhsLength);
                auto rhsBuffer = get_buffer(wistd::forward<RhsT>(rhs), &rhsLength);

                const auto result = ::CompareStringOrdinal(
                    lhsBuffer,
                    lhsLength,
                    rhsBuffer,
                    rhsLength,
                    IgnoreCase ? TRUE : FALSE);
                WI_ASSERT(result != 0);

                return result;
            }

            template <typename LhsT, typename RhsT>
            static auto equals(LhsT&& lhs, RhsT&& rhs) WI_NOEXCEPT ->
                decltype(compare(wistd::forward<LhsT>(lhs), wistd::forward<RhsT>(rhs)), bool())
            {
                return compare(wistd::forward<LhsT>(lhs), wistd::forward<RhsT>(rhs)) == CSTR_EQUAL;
            }

            template <typename LhsT, typename RhsT>
            static auto not_equals(LhsT&& lhs, RhsT&& rhs) WI_NOEXCEPT ->
                decltype(compare(wistd::forward<LhsT>(lhs), wistd::forward<RhsT>(rhs)), bool())
            {
                return compare(wistd::forward<LhsT>(lhs), wistd::forward<RhsT>(rhs)) != CSTR_EQUAL;
            }

            template <typename LhsT, typename RhsT>
            static auto less(LhsT&& lhs, RhsT&& rhs) WI_NOEXCEPT ->
                decltype(compare(wistd::forward<LhsT>(lhs), wistd::forward<RhsT>(rhs)), bool())
            {
                return compare(wistd::forward<LhsT>(lhs), wistd::forward<RhsT>(rhs)) == CSTR_LESS_THAN;
            }

            template <typename LhsT, typename RhsT>
            static auto less_equals(LhsT&& lhs, RhsT&& rhs) WI_NOEXCEPT ->
                decltype(compare(wistd::forward<LhsT>(lhs), wistd::forward<RhsT>(rhs)), bool())
            {
                return compare(wistd::forward<LhsT>(lhs), wistd::forward<RhsT>(rhs)) != CSTR_GREATER_THAN;
            }

            template <typename LhsT, typename RhsT>
            static auto greater(LhsT&& lhs, RhsT&& rhs) WI_NOEXCEPT ->
                decltype(compare(wistd::forward<LhsT>(lhs), wistd::forward<RhsT>(rhs)), bool())
            {
                return compare(wistd::forward<LhsT>(lhs), wistd::forward<RhsT>(rhs)) == CSTR_GREATER_THAN;
            }

            template <typename LhsT, typename RhsT>
            static auto greater_equals(LhsT&& lhs, RhsT&& rhs) WI_NOEXCEPT ->
                decltype(compare(wistd::forward<LhsT>(lhs), wistd::forward<RhsT>(rhs)), bool())
            {
                return compare(wistd::forward<LhsT>(lhs), wistd::forward<RhsT>(rhs)) != CSTR_LESS_THAN;
            }
        };
    }
    /// @endcond

    //! Detects if one or more embedded null is present in an HSTRING.
    inline bool HasEmbeddedNull(_In_opt_ HSTRING value)
    {
        BOOL hasEmbeddedNull = FALSE;
        (void)WindowsStringHasEmbeddedNull(value, &hasEmbeddedNull);
        return hasEmbeddedNull != FALSE;
    }

    /** TwoPhaseHStringConstructor help using the 2 phase constructor pattern for HSTRINGs.
    ~~~
    auto stringConstructor = wil::TwoPhaseHStringConstructor::Preallocate(size);
    RETURN_IF_NULL_ALLOC(stringConstructor.Get());

    RETURN_IF_FAILED(stream->Read(stringConstructor.Get(), stringConstructor.ByteSize(), &bytesRead));

    // Validate stream contents, sizes must match, string must be null terminated.
    RETURN_IF_FAILED(stringConstructor.Validate(bytesRead));

    wil::unique_hstring string { stringConstructor.Promote() };
    ~~~

    See also wil::unique_hstring_buffer.
    */
    struct TwoPhaseHStringConstructor
    {
        TwoPhaseHStringConstructor() = delete;
        TwoPhaseHStringConstructor(const TwoPhaseHStringConstructor&) = delete;
        void operator=(const TwoPhaseHStringConstructor&) = delete;

        TwoPhaseHStringConstructor(TwoPhaseHStringConstructor&& other) WI_NOEXCEPT
        {
            m_characterLength = other.m_characterLength;
            other.m_characterLength = 0;
            m_maker = wistd::move(other.m_maker);
        }

        static TwoPhaseHStringConstructor Preallocate(UINT32 characterLength)
        {
            return TwoPhaseHStringConstructor{ characterLength };
        }

        //! Returns the HSTRING after it has been populated like Detatch() or release(); be sure to put this in a RAII type to manage its lifetime.
        HSTRING Promote()
        {
            m_characterLength = 0;
            return m_maker.release().release();
        }

        ~TwoPhaseHStringConstructor() = default;

        WI_NODISCARD explicit operator PCWSTR() const
        {
            // This is set by WindowsPromoteStringBuffer() which must be called to
            // construct this object via the static method Preallocate().
            return m_maker.buffer();
        }

        //! Returns a pointer for the buffer so it can be populated
        WI_NODISCARD wchar_t* Get() const { return const_cast<wchar_t*>(m_maker.buffer()); }
        //! Used to validate range of buffer when populating.
        WI_NODISCARD ULONG ByteSize() const { return m_characterLength * sizeof(wchar_t); }

        /** Ensure that the size of the data provided is consistent with the pre-allocated buffer.
        It seems that WindowsPreallocateStringBuffer() provides the null terminator in the buffer
        (based on testing) so this can be called before populating the buffer.
        */
        WI_NODISCARD HRESULT Validate(ULONG bytesRead) const
        {
            // Null termination is required for the buffer before calling WindowsPromoteStringBuffer().
            RETURN_HR_IF(HRESULT_FROM_WIN32(ERROR_INVALID_DATA),
                (bytesRead != ByteSize()) ||
                (Get()[m_characterLength] != L'\0'));
            return S_OK;
        }

    private:
        TwoPhaseHStringConstructor(UINT32 characterLength) : m_characterLength(characterLength)
        {
            (void)m_maker.make(nullptr, characterLength);
        }

        UINT32 m_characterLength;
        details::string_maker<unique_hstring> m_maker;
    };

    //! A transparent less-than comparison function object that enables comparison of various string types intended for
    //! use with associative containers (such as `std::set`, `std::map`, etc.) that use
    //! `Microsoft::WRL::Wrappers::HString` as the key type. This removes the need for the consumer to explicitly
    //! create an `HString` object when using lookup functions such as `find`, `lower_bound`, etc. For example, the
    //! following scenarios would all work exactly as you would expect them to:
    //! ~~~
    //! std::map<HString, int, wil::hstring_less> map;
    //! const wchar_t constArray[] = L"foo";
    //! wchar_t nonConstArray[MAX_PATH] = L"foo";
    //!
    //! HString key;
    //! THROW_IF_FAILED(key.Set(constArray));
    //! map.emplace(std::move(key), 42);
    //!
    //! HString str;
    //! wil::unique_hstring uniqueStr;
    //! THROW_IF_FAILED(str.Set(L"foo"));
    //! THROW_IF_FAILED(str.CopyTo(&uniqueStr));
    //!
    //! // All of the following return an iterator to the pair { L"foo", 42 }
    //! map.find(str);
    //! map.find(str.Get());
    //! map.find(HStringReference(constArray));
    //! map.find(uniqueStr);
    //! map.find(std::wstring(constArray));
    //! map.find(constArray);
    //! map.find(nonConstArray);
    //! map.find(static_cast<const wchar_t*>(constArray));
    //! ~~~
    //! The first four calls in the example above use `WindowsGetStringRawBuffer` (or equivalent) to get the string
    //! buffer and length for the comparison. The fifth example uses `std::wstring::c_str` and `std::wstring::length`
    //! for getting these two values. The remaining three examples use only the string buffer and call `wcslen` for the
    //! length. That is, the length is *not* deduced for either array. This is because argument types are not always
    //! perfectly preserved by container functions and in fact are often captured as const references making it
    //! impossible to differentiate const arrays - where we can safely deduce length - from non const arrays - where we
    //! cannot safely deduce length since the buffer may be larger than actually needed (e.g. creating a
    //! `char[MAX_PATH]` array, but only filling it with 10 characters). The implications of this behavior is that
    //! string literals that contain embedded null characters will only include the part of the buffer up to the first
    //! null character. For example, the following example will result in all calls to `find` returning an end
    //! iterator.
    //! ~~~
    //! std::map<HString, int, wil::hstring_less> map;
    //! const wchar_t constArray[] = L"foo\0bar";
    //! wchar_t nonConstArray[MAX_PATH] = L"foo\0bar";
    //!
    //! // Create the key with the embedded null character
    //! HString key;
    //! THROW_IF_FAILED(key.Set(constArray));
    //! map.emplace(std::move(key), 42);
    //!
    //! // All of the following return map.end() since they look for the string "foo"
    //! map.find(constArray);
    //! map.find(nonConstArray);
    //! map.find(static_cast<const wchar_t*>(constArray));
    //! ~~~
    //! In order to search using a string literal that contains embedded null characters, a simple alternative is to
    //! first create an `HStringReference` and use that for the function call:
    //! ~~~
    //! // HStringReference's constructor *will* deduce the length of const arrays
    //! map.find(HStringReference(constArray));
    //! ~~~
    struct hstring_less
    {
        using is_transparent = void;

        template <typename LhsT, typename RhsT>
        WI_NODISCARD auto operator()(const LhsT& lhs, const RhsT& rhs) const WI_NOEXCEPT ->
            decltype(details::hstring_compare<true, false>::less(lhs, rhs))
        {
            return details::hstring_compare<true, false>::less(lhs, rhs);
        }
    };

    //! A transparent less-than comparison function object whose behavior is equivalent to that of @ref hstring_less
    //! with the one difference that comparisons are case-insensitive. That is, the following example will correctly
    //! find the inserted value:
    //! ~~~
    //! std::map<HString, int, wil::hstring_insensitive_less> map;
    //!
    //! HString key;
    //! THROW_IF_FAILED(key.Set(L"foo"));
    //! map.emplace(std::move(key), 42);
    //!
    //! // All of the following return an iterator to the pair { L"foo", 42 }
    //! map.find(L"FOo");
    //! map.find(HStringReference(L"fOo"));
    //! map.find(HStringReference(L"fOO").Get());
    //! ~~~
    struct hstring_insensitive_less
    {
        using is_transparent = void;

        template <typename LhsT, typename RhsT>
        WI_NODISCARD auto operator()(const LhsT& lhs, const RhsT& rhs) const WI_NOEXCEPT ->
            decltype(details::hstring_compare<true, true>::less(lhs, rhs))
        {
            return details::hstring_compare<true, true>::less(lhs, rhs);
        }
    };

#pragma endregion

    /// @cond
    namespace details
    {
        // MapToSmartType<T>::type is used to map a raw type into an RAII expression
        // of it. This is needed when lifetime management of the type is needed, for example
        // when holding them as a value produced in an iterator.
        // This type has a common set of methods used to abstract the access to the value
        // that is similar to ComPtr<> and the WRL Wrappers: Get(), GetAddressOf() and other operators.
        // Clients of the smart type must use those to access the value.

        // TODO: Having the base definition defined will result in creating leaks if a type
        // that needs resource management (e.g. PROPVARIANT) that has not specialized is used.
        //
        // One fix is to use std::is_enum to cover that case and leave the base definition undefined.
        // That base should use static_assert to inform clients how to fix the lack of specialization.
        template<typename T, typename Enable = void> struct MapToSmartType
        {
            #pragma warning(push)
            #pragma warning(disable:4702) // https://github.com/Microsoft/wil/issues/2
            struct type // T holder
            {
                type() = default;
                type(T&& value) : m_value(wistd::forward<T>(value)) {}
                WI_NODISCARD operator T() const { return m_value; }
                type& operator=(T&& value) { m_value = wistd::forward<T>(value); return *this; }
                WI_NODISCARD T Get() const { return m_value; }

                // Returning T&& to support move only types
                // In case of absence of T::operator=(T&&) a call to T::operator=(const T&) will happen
                T&& Get()          { return wistd::move(m_value); }

                WI_NODISCARD HRESULT CopyTo(T* result) const { *result = m_value; return S_OK; }
                T* GetAddressOf()  { return &m_value; }
                T* ReleaseAndGetAddressOf() { return &m_value; }
                T* operator&()     { return &m_value; }
                T m_value{};
            };
            #pragma warning(pop)
        };

        // IUnknown * derived -> Microsoft::WRL::ComPtr<>
        template <typename T>
        struct MapToSmartType<T, typename wistd::enable_if<wistd::is_base_of<IUnknown, typename wistd::remove_pointer<T>::type>::value>::type>
        {
            typedef Microsoft::WRL::ComPtr<typename wistd::remove_pointer<T>::type> type;
        };

        // HSTRING -> Microsoft::WRL::Wrappers::HString
        template <> struct MapToSmartType<HSTRING, void>
        {
            class HStringWithRelease : public Microsoft::WRL::Wrappers::HString
            {
            public:
                // Unlike all other WRL types HString does not have ReleaseAndGetAddressOf and
                // GetAddressOf() has non-standard behavior, calling Release().
                HSTRING* ReleaseAndGetAddressOf() WI_NOEXCEPT
                {
                    Release();
                    return &hstr_;
                }
            };
            typedef HStringWithRelease type;
        };

        // WinRT interfaces like IVector<>, IAsyncOperation<> and IIterable<> can be templated
        // on a runtime class (instead of an interface or primitive type). In these cases the objects
        // produced by those interfaces implement an interface defined by the runtime class default interface.
        //
        // These templates deduce the type of the produced interface or pass through
        // the type unmodified in the non runtime class case.
        //
        // for example:
        //      IAsyncOperation<StorageFile*> -> IAsyncOperation<IStorageFile*>

        // For IVector<T>, IVectorView<T>.
        template<typename VectorType> struct MapVectorResultType
        {
            template<typename TVector, typename TResult>
            static TResult PeekGetAtType(HRESULT(STDMETHODCALLTYPE TVector::*)(unsigned, TResult*));
            typedef decltype(PeekGetAtType(&VectorType::GetAt)) type;
        };

        // For IIterator<T>.
        template<typename T> struct MapIteratorResultType
        {
            template<typename TIterable, typename TResult>
            static TResult PeekCurrentType(HRESULT(STDMETHODCALLTYPE TIterable::*)(TResult*));
            typedef decltype(PeekCurrentType(&ABI::Windows::Foundation::Collections::IIterator<T>::get_Current)) type;
        };

        // For IAsyncOperation<T>.
        template<typename T> struct MapAsyncOpResultType
        {
            template<typename TAsyncOperation, typename TResult>
            static TResult PeekGetResultsType(HRESULT(STDMETHODCALLTYPE TAsyncOperation::*)(TResult*));
            typedef decltype(PeekGetResultsType(&ABI::Windows::Foundation::IAsyncOperation<T>::GetResults)) type;
        };

        // For IAsyncOperationWithProgress<T, P>.
        template<typename T, typename P> struct MapAsyncOpProgressResultType
        {
            template<typename TAsyncOperation, typename TResult>
            static TResult PeekGetResultsType(HRESULT(STDMETHODCALLTYPE TAsyncOperation::*)(TResult*));
            typedef decltype(PeekGetResultsType(&ABI::Windows::Foundation::IAsyncOperationWithProgress<T, P>::GetResults)) type;
        };

        // No support for IAsyncActionWithProgress<P> none of these (currently) use
        // a runtime class for the progress type.
    }
    /// @endcond
#pragma region C++ iterators for WinRT collections for use with range based for and STL algorithms

    /** Range base for and STL algorithms support for WinRT ABI collection types, IVector<T>, IVectorView<T>, IIterable<T>
    similar to support provided by <collection.h> for C++ CX. Three error handling policies are supported.
    ~~~
    ComPtr<CollectionType> collection = GetCollection(); // can be IVector<HSTRING>, IVectorView<HSTRING> or IIterable<HSTRING>

    for (auto const& element : wil::get_range(collection.Get()))                // exceptions
    for (auto const& element : wil::get_range_nothrow(collection.Get(), &hr))   // error code
    for (auto const& element : wil::get_range_failfast(collection.Get()))       // fail fast
    {
       // use element
    }
    ~~~
    Standard algorithm example:
    ~~~
    ComPtr<IVectorView<StorageFile*>> files = GetFiles();
    auto fileRange = wil::get_range_nothrow(files.Get());
    auto itFound = std::find_if(fileRange.begin(), fileRange.end(), [](ComPtr<IStorageFile> file) -> bool
    {
         return true; // first element in range
    });
    ~~~
    */
#pragma region exception and fail fast based IVector<>/IVectorView<>

    template <typename VectorType, typename err_policy = err_exception_policy>
    class vector_range
    {
    public:
        typedef typename details::MapVectorResultType<VectorType>::type TResult;
        typedef typename details::MapToSmartType<TResult>::type TSmart;

        vector_range() = delete;

        explicit vector_range(_In_ VectorType *vector) : m_v(vector)
        {
        }

        class vector_iterator
        {
        public:
#ifdef _XUTILITY_
            // could be random_access_iterator_tag but missing some features
            typedef ::std::bidirectional_iterator_tag iterator_category;
#endif
            typedef TSmart value_type;
            typedef ptrdiff_t difference_type;
            typedef const TSmart* pointer;
            typedef const TSmart& reference;

            // for begin()
            vector_iterator(VectorType* v, unsigned int pos)
                : m_v(v), m_i(pos)
            {
            }

            // for end()
            vector_iterator() : m_v(nullptr), m_i(-1) {}

            vector_iterator(const vector_iterator& other)
            {
                m_v = other.m_v;
                m_i = other.m_i;
                err_policy::HResult(other.m_element.CopyTo(m_element.GetAddressOf()));
            }

            vector_iterator& operator=(const vector_iterator& other)
            {
                if (this != wistd::addressof(other))
                {
                    m_v = other.m_v;
                    m_i = other.m_i;
                    err_policy::HResult(other.m_element.CopyTo(m_element.ReleaseAndGetAddressOf()));
                }
                return *this;
            }

            reference operator*()
            {
                err_policy::HResult(m_v->GetAt(m_i, m_element.ReleaseAndGetAddressOf()));
                return m_element;
            }

            pointer operator->()
            {
                err_policy::HResult(m_v->GetAt(m_i, m_element.ReleaseAndGetAddressOf()));
                return wistd::addressof(m_element);
            }

            vector_iterator& operator++()
            {
                ++m_i;
                return *this;
            }

            vector_iterator& operator--()
            {
                --m_i;
                return *this;
            }

            vector_iterator operator++(int)
            {
                vector_iterator old(*this);
                ++*this;
                return old;
            }

            vector_iterator operator--(int)
            {
                vector_iterator old(*this);
                --*this;
                return old;
            }

            vector_iterator& operator+=(int n)
            {
                m_i += n;
                return *this;
            }

            vector_iterator& operator-=(int n)
            {
                m_i -= n;
                return *this;
            }

            WI_NODISCARD vector_iterator operator+(int n) const
            {
                vector_iterator ret(*this);
                ret += n;
                return ret;
            }

            WI_NODISCARD vector_iterator operator-(int n) const
            {
                vector_iterator ret(*this);
                ret -= n;
                return ret;
            }

            WI_NODISCARD ptrdiff_t operator-(const vector_iterator& other) const
            {
                return m_i - other.m_i;
            }

            WI_NODISCARD bool operator==(const vector_iterator& other) const
            {
                return m_i == other.m_i;
            }

            WI_NODISCARD bool operator!=(const vector_iterator& other) const
            {
                return m_i != other.m_i;
            }

            WI_NODISCARD bool operator<(const vector_iterator& other) const
            {
                return m_i < other.m_i;
            }

            WI_NODISCARD bool operator>(const vector_iterator& other) const
            {
                return m_i > other.m_i;
            }

            WI_NODISCARD bool operator<=(const vector_iterator& other) const
            {
                return m_i <= other.m_i;
            }

            WI_NODISCARD bool operator>=(const vector_iterator& other) const
            {
                return m_i >= other.m_i;
            }

        private:
            VectorType* m_v; // weak, collection must outlive iterators.
            unsigned int m_i;
            TSmart m_element;
        };

        vector_iterator begin()
        {
            return vector_iterator(m_v, 0);
        }

        vector_iterator end()
        {
            unsigned int size;
            err_policy::HResult(m_v->get_Size(&size));
            return vector_iterator(m_v, size);
        }
    private:
        VectorType* m_v; // weak, collection must outlive iterators.
    };
#pragma endregion

#pragma region error code based IVector<>/IVectorView<>

    template <typename VectorType>
    class vector_range_nothrow
    {
    public:
        typedef typename details::MapVectorResultType<VectorType>::type TResult;
        typedef typename details::MapToSmartType<TResult>::type TSmart;

        vector_range_nothrow() = delete;
        vector_range_nothrow(const vector_range_nothrow&) = delete;
        vector_range_nothrow& operator=(const vector_range_nothrow&) = delete;

        vector_range_nothrow(vector_range_nothrow&& other) WI_NOEXCEPT :
            m_v(other.m_v), m_size(other.m_size), m_result(other.m_result), m_resultStorage(other.m_resultStorage),
            m_currentElement(wistd::move(other.m_currentElement))
        {
        }

        vector_range_nothrow(_In_ VectorType *vector, HRESULT* result = nullptr)
            : m_v(vector), m_result(result ? result : &m_resultStorage)
        {
            *m_result = m_v->get_Size(&m_size);
        }

        class vector_iterator_nothrow
        {
        public:
#ifdef _XUTILITY_
            // must be input_iterator_tag as use (via ++, --, etc.) of one invalidates the other.
            typedef ::std::input_iterator_tag iterator_category;
#endif
            typedef TSmart value_type;
            typedef ptrdiff_t difference_type;
            typedef const TSmart* pointer;
            typedef const TSmart& reference;

            vector_iterator_nothrow() = delete;
            vector_iterator_nothrow(vector_range_nothrow<VectorType>* range, unsigned int pos)
                : m_range(range), m_i(pos)
            {
            }

            WI_NODISCARD reference operator*() const
            {
                return m_range->m_currentElement;
            }

            WI_NODISCARD pointer operator->() const
            {
                return wistd::addressof(m_range->m_currentElement);
            }

            vector_iterator_nothrow& operator++()
            {
                ++m_i;
                m_range->get_at_current(m_i);
                return *this;
            }

            vector_iterator_nothrow& operator--()
            {
                --m_i;
                m_range->get_at_current(m_i);
                return *this;
            }

            vector_iterator_nothrow operator++(int)
            {
                vector_iterator_nothrow old(*this);
                ++*this;
                return old;
            }

            vector_iterator_nothrow operator--(int)
            {
                vector_iterator_nothrow old(*this);
                --*this;
                return old;
            }

            vector_iterator_nothrow& operator+=(int n)
            {
                m_i += n;
                m_range->get_at_current(m_i);
                return *this;
            }

            vector_iterator_nothrow& operator-=(int n)
            {
                m_i -= n;
                m_range->get_at_current(m_i);
                return *this;
            }

            WI_NODISCARD bool operator==(vector_iterator_nothrow const& other) const
            {
                return FAILED(*m_range->m_result) || (m_i == other.m_i);
            }

            WI_NODISCARD bool operator!=(vector_iterator_nothrow const& other) const
            {
                return !operator==(other);
            }

        private:
            vector_range_nothrow<VectorType>* m_range;
            unsigned int m_i = 0;
        };

        vector_iterator_nothrow begin()
        {
            get_at_current(0);
            return vector_iterator_nothrow(this, 0);
        }

        vector_iterator_nothrow end()
        {
            return vector_iterator_nothrow(this, m_size);
        }

        // Note, the error code is observed in operator!= and operator==, it always
        // returns "equal" in the failed state to force the compare to the end
        // iterator to return false and stop the loop.
        //
        // Is this ok for the general case?
        void get_at_current(unsigned int i)
        {
            if (SUCCEEDED(*m_result) && (i < m_size))
            {
                *m_result = m_v->GetAt(i, m_currentElement.ReleaseAndGetAddressOf());
            }
        }

    private:
        VectorType* m_v; // weak, collection must outlive iterators.
        unsigned int m_size;

        // This state is shared by vector_iterator_nothrow instances. this means
        // use of one iterator invalidates the other.
        HRESULT* m_result;
        HRESULT m_resultStorage = S_OK; // for the case where the caller does not provide the location to store the result
        TSmart m_currentElement;
    };

#pragma endregion

#pragma region exception and fail fast based IIterable<>

    template <typename T, typename err_policy = err_exception_policy>
    class iterable_range
    {
    public:
        typedef typename details::MapIteratorResultType<T>::type TResult;
        typedef typename details::MapToSmartType<TResult>::type TSmart;

        explicit iterable_range(_In_ ABI::Windows::Foundation::Collections::IIterable<T>* iterable)
            : m_iterable(iterable)
        {
        }

        class iterable_iterator
        {
        public:
#ifdef _XUTILITY_
            typedef ::std::forward_iterator_tag iterator_category;
#endif
            typedef TSmart value_type;
            typedef ptrdiff_t difference_type;
            typedef const TSmart* pointer;
            typedef const TSmart& reference;

            iterable_iterator() : m_i(-1) {}

            // for begin()
            explicit iterable_iterator(_In_ ABI::Windows::Foundation::Collections::IIterable<T>* iterable)
            {
                err_policy::HResult(iterable->First(&m_iterator));
                boolean hasCurrent;
                err_policy::HResult(m_iterator->get_HasCurrent(&hasCurrent));
                m_i = hasCurrent ? 0 : -1;
            }

            // for end()
            iterable_iterator(int /*currentIndex*/) : m_i(-1)
            {
            }

            iterable_iterator(const iterable_iterator& other)
            {
                m_iterator = other.m_iterator;
                m_i = other.m_i;
                err_policy::HResult(other.m_element.CopyTo(m_element.GetAddressOf()));
            }

            iterable_iterator& operator=(const iterable_iterator& other)
            {
                m_iterator = other.m_iterator;
                m_i = other.m_i;
                err_policy::HResult(other.m_element.CopyTo(m_element.ReleaseAndGetAddressOf()));
                return *this;
            }

            WI_NODISCARD bool operator==(iterable_iterator const& other) const
            {
                return m_i == other.m_i;
            }

            WI_NODISCARD bool operator!=(iterable_iterator const& other) const
            {
                return !operator==(other);
            }

            reference operator*()
            {
                err_policy::HResult(m_iterator->get_Current(m_element.ReleaseAndGetAddressOf()));
                return m_element;
            }

            pointer operator->()
            {
                err_policy::HResult(m_iterator->get_Current(m_element.ReleaseAndGetAddressOf()));
                return wistd::addressof(m_element);
            }

            iterable_iterator& operator++()
            {
                boolean hasCurrent;
                err_policy::HResult(m_iterator->MoveNext(&hasCurrent));
                if (hasCurrent)
                {
                    m_i++;
                }
                else
                {
                    m_i = -1;
                }
                return *this;
            }

            iterable_iterator operator++(int)
            {
                iterable_iterator old(*this);
                ++*this;
                return old;
            }

        private:
            Microsoft::WRL::ComPtr<ABI::Windows::Foundation::Collections::IIterator<T>> m_iterator;
            int m_i;
            TSmart m_element;
        };

        iterable_iterator begin()
        {
            return iterable_iterator(m_iterable);
        }

        iterable_iterator end()
        {
            return iterable_iterator();
        }
    private:
        // weak, collection must outlive iterators.
        ABI::Windows::Foundation::Collections::IIterable<T>* m_iterable;
    };
#pragma endregion

#if defined(__WI_HAS_STD_VECTOR)
    /** Converts WinRT vectors to std::vector by requesting the collection's data in a single
    operation. This can be more efficient in terms of IPC cost than iteratively processing it.
    ~~~
    ComPtr<IVector<IPropertyValue*>> values = GetValues();
    std::vector<ComPtr<IPropertyValue>> allData = wil::to_vector(values);
    for (ComPtr<IPropertyValue> const& item : allData)
    {
        // use item
    }
    Can be used for ABI::Windows::Foundation::Collections::IVector<T> and
    ABI::Windows::Foundation::Collections::IVectorView<T>
    */
    template<typename VectorType> auto to_vector(VectorType* src)
    {
        using TResult = typename details::MapVectorResultType<VectorType>::type;
        using TSmart = typename details::MapToSmartType<TResult>::type;
        static_assert(sizeof(TResult) == sizeof(TSmart), "result and smart sizes are different");
        std::vector<TSmart> output;
        UINT32 expected = 0;
        THROW_IF_FAILED(src->get_Size(&expected));
        if (expected > 0)
        {
            output.resize(expected + 1);
            UINT32 fetched = 0;
            THROW_IF_FAILED(src->GetMany(0, static_cast<UINT32>(output.size()), reinterpret_cast<TResult*>(output.data()), &fetched));
            THROW_HR_IF(E_CHANGED_STATE, fetched > expected);
            output.resize(fetched);
        }
        return output;
    }
#endif

#pragma region error code base IIterable<>
    template <typename T>
    class iterable_range_nothrow
    {
    public:
        typedef typename details::MapIteratorResultType<T>::type TResult;
        typedef typename details::MapToSmartType<TResult>::type TSmart;

        iterable_range_nothrow() = delete;
        iterable_range_nothrow(const iterable_range_nothrow&) = delete;
        iterable_range_nothrow& operator=(const iterable_range_nothrow&) = delete;
        iterable_range_nothrow& operator=(iterable_range_nothrow &&) = delete;

        iterable_range_nothrow(iterable_range_nothrow&& other) WI_NOEXCEPT :
            m_iterator(wistd::move(other.m_iterator)), m_element(wistd::move(other.m_element)),
            m_resultStorage(other.m_resultStorage)
        {
            if (other.m_result == &other.m_resultStorage)
            {
                m_result = &m_resultStorage;
            }
            else
            {
                m_result = other.m_result;
            }
        }

        iterable_range_nothrow(_In_ ABI::Windows::Foundation::Collections::IIterable<T>* iterable, HRESULT* result = nullptr)
            : m_result(result ? result : &m_resultStorage)
        {
            *m_result = iterable->First(&m_iterator);
            if (SUCCEEDED(*m_result))
            {
                boolean hasCurrent;
                *m_result = m_iterator->get_HasCurrent(&hasCurrent);
                if (SUCCEEDED(*m_result) && hasCurrent)
                {
                    *m_result = m_iterator->get_Current(m_element.ReleaseAndGetAddressOf());
                    if (FAILED(*m_result))
                    {
                        m_iterator = nullptr; // release the iterator if no elements are found
                    }
                }
                else
                {
                    m_iterator = nullptr; // release the iterator if no elements are found
                }
            }
        }

        class iterable_iterator_nothrow
        {
        public:
#ifdef _XUTILITY_
            // muse be input_iterator_tag as use of one instance invalidates the other.
            typedef ::std::input_iterator_tag iterator_category;
#endif
            typedef TSmart value_type;
            typedef ptrdiff_t difference_type;
            typedef const TSmart* pointer;
            typedef const TSmart& reference;

            iterable_iterator_nothrow(_In_ iterable_range_nothrow* range, int currentIndex) :
                m_range(range), m_i(currentIndex)
            {
            }

            WI_NODISCARD bool operator==(iterable_iterator_nothrow const& other) const
            {
                return FAILED(*m_range->m_result) || (m_i == other.m_i);
            }

            WI_NODISCARD bool operator!=(iterable_iterator_nothrow const& other) const
            {
                return !operator==(other);
            }

            WI_NODISCARD reference operator*() const WI_NOEXCEPT
            {
                return m_range->m_element;
            }

            WI_NODISCARD pointer operator->() const WI_NOEXCEPT
            {
                return wistd::addressof(m_range->m_element);
            }

            iterable_iterator_nothrow& operator++()
            {
                boolean hasCurrent;
                *m_range->m_result = m_range->m_iterator->MoveNext(&hasCurrent);
                if (SUCCEEDED(*m_range->m_result) && hasCurrent)
                {
                    *m_range->m_result = m_range->m_iterator->get_Current(m_range->m_element.ReleaseAndGetAddressOf());
                    if (SUCCEEDED(*m_range->m_result))
                    {
                        m_i++;
                    }
                    else
                    {
                        m_i = -1;
                    }
                }
                else
                {
                    m_i = -1;
                }
                return *this;
            }

            iterable_range_nothrow operator++(int)
            {
                iterable_range_nothrow old(*this);
                ++*this;
                return old;
            }

        private:
            iterable_range_nothrow* m_range;
            int m_i;
        };

        iterable_iterator_nothrow begin()
        {
            return iterable_iterator_nothrow(this, this->m_iterator ? 0 : -1);
        }

        iterable_iterator_nothrow end()
        {
            return iterable_iterator_nothrow(this, -1);
        }

    private:
        Microsoft::WRL::ComPtr<ABI::Windows::Foundation::Collections::IIterator<T>> m_iterator;
        // This state is shared by all iterator instances
        // so use of one iterator can invalidate another's ability to dereference
        // that is allowed for input iterators.
        TSmart m_element;
        HRESULT* m_result;
        HRESULT m_resultStorage = S_OK;
    };

#pragma endregion

#ifdef WIL_ENABLE_EXCEPTIONS
    template <typename T> vector_range<ABI::Windows::Foundation::Collections::IVector<T>> get_range(ABI::Windows::Foundation::Collections::IVector<T> *v)
    {
        return vector_range<ABI::Windows::Foundation::Collections::IVector<T>>(v);
    }

    template <typename T> vector_range<ABI::Windows::Foundation::Collections::IVectorView<T>> get_range(ABI::Windows::Foundation::Collections::IVectorView<T> *v)
    {
        return vector_range<ABI::Windows::Foundation::Collections::IVectorView<T>>(v);
    }
#endif // WIL_ENABLE_EXCEPTIONS

    template <typename T> vector_range<ABI::Windows::Foundation::Collections::IVector<T>, err_failfast_policy> get_range_failfast(ABI::Windows::Foundation::Collections::IVector<T> *v)
    {
        return vector_range<ABI::Windows::Foundation::Collections::IVector<T>, err_failfast_policy>(v);
    }

    template <typename T> vector_range<ABI::Windows::Foundation::Collections::IVectorView<T>, err_failfast_policy> get_range_failfast(ABI::Windows::Foundation::Collections::IVectorView<T> *v)
    {
        return vector_range<ABI::Windows::Foundation::Collections::IVectorView<T>, err_failfast_policy>(v);
    }

    template <typename T> vector_range_nothrow<ABI::Windows::Foundation::Collections::IVector<T>> get_range_nothrow(ABI::Windows::Foundation::Collections::IVector<T> *v, HRESULT* result = nullptr)
    {
        return vector_range_nothrow<ABI::Windows::Foundation::Collections::IVector<T>>(v, result);
    }

    template <typename T> vector_range_nothrow<ABI::Windows::Foundation::Collections::IVectorView<T>> get_range_nothrow(ABI::Windows::Foundation::Collections::IVectorView<T> *v, HRESULT* result = nullptr)
    {
        return vector_range_nothrow<ABI::Windows::Foundation::Collections::IVectorView<T>>(v, result);
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    template <typename T> iterable_range<T> get_range(ABI::Windows::Foundation::Collections::IIterable<T> *v)
    {
        return iterable_range<T>(v);
    }
#endif // WIL_ENABLE_EXCEPTIONS

    template <typename T> iterable_range<T, err_failfast_policy> get_range_failfast(ABI::Windows::Foundation::Collections::IIterable<T> *v)
    {
        return iterable_range<T, err_failfast_policy>(v);
    }

    template <typename T> iterable_range_nothrow<T> get_range_nothrow(ABI::Windows::Foundation::Collections::IIterable<T> *v, HRESULT* result = nullptr)
    {
        return iterable_range_nothrow<T>(v, result);
    }
}

#pragma endregion

#ifdef WIL_ENABLE_EXCEPTIONS

#pragma region Global operator functions
#if defined(MIDL_NS_PREFIX) || defined(____x_ABI_CWindows_CFoundation_CIClosable_FWD_DEFINED__)
namespace ABI {
#endif
    namespace Windows {
        namespace Foundation {
            namespace Collections {
                template <typename X> typename wil::vector_range<IVector<X>>::vector_iterator begin(IVector<X>* v)
                {
                    return typename wil::vector_range<IVector<X>>::vector_iterator(v, 0);
                }

                template <typename X> typename wil::vector_range<IVector<X>>::vector_iterator end(IVector<X>* v)
                {
                    unsigned int size;
                    THROW_IF_FAILED(v->get_Size(&size));
                    return typename wil::vector_range<IVector<X>>::vector_iterator(v, size);
                }

                template <typename X> typename wil::vector_range<IVectorView<X>>::vector_iterator begin(IVectorView<X>* v)
                {
                    return typename wil::vector_range<IVectorView<X>>::vector_iterator(v, 0);
                }

                template <typename X> typename wil::vector_range<IVectorView<X>>::vector_iterator end(IVectorView<X>* v)
                {
                    unsigned int size;
                    THROW_IF_FAILED(v->get_Size(&size));
                    return typename wil::vector_range<IVectorView<X>>::vector_iterator(v, size);
                }

                template <typename X> typename wil::iterable_range<X>::iterable_iterator begin(IIterable<X>* i)
                {
                    return typename wil::iterable_range<X>::iterable_iterator(i);
                }

                template <typename X> typename wil::iterable_range<X>::iterable_iterator end(IIterable<X>*)
                {
                    return typename wil::iterable_range<X>::iterable_iterator();
                }
            } // namespace Collections
        } // namespace Foundation
    } // namespace Windows
#if defined(MIDL_NS_PREFIX) || defined(____x_ABI_CWindows_CFoundation_CIClosable_FWD_DEFINED__)
} // namespace ABI
#endif

#endif // WIL_ENABLE_EXCEPTIONS

#pragma endregion

namespace wil
{
#pragma region WinRT Async API helpers

/// @cond
namespace details
{
    template <typename TResult, typename TFunc, typename ...Args,
        typename wistd::enable_if<wistd::is_same<HRESULT, TResult>::value, int>::type = 0>
        HRESULT CallAndHandleErrorsWithReturnType(TFunc&& func, Args&&... args)
    {
        return wistd::forward<TFunc>(func)(wistd::forward<Args>(args)...);
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    template <typename TResult, typename TFunc, typename ...Args,
        typename wistd::enable_if<wistd::is_same<void, TResult>::value, int>::type = 0>
        HRESULT CallAndHandleErrorsWithReturnType(TFunc&& func, Args&&... args)
    {
        try
        {
            wistd::forward<TFunc>(func)(wistd::forward<Args>(args)...);
        }
        CATCH_RETURN();
        return S_OK;
    }
#endif

    template <typename TFunc, typename ...Args>
    HRESULT CallAndHandleErrors(TFunc&& func, Args&&... args)
    {
        return CallAndHandleErrorsWithReturnType<decltype(wistd::forward<TFunc>(func)(wistd::forward<Args>(args)...))>(
            wistd::forward<TFunc>(func), wistd::forward<Args>(args)...);
    }

    // Get the last type of a template parameter pack.
    // usage:
    //     LastType<int, bool>::type boolValue;
    template <typename... Ts> struct LastType
    {
        template<typename T, typename... OtherTs> struct LastTypeOfTs
        {
            typedef typename LastTypeOfTs<OtherTs...>::type type;
        };

        template<typename T> struct LastTypeOfTs<T>
        {
            typedef T type;
        };

        template<typename... OtherTs>
        static typename LastTypeOfTs<OtherTs...>::type LastTypeOfTsFunc() {}
        typedef decltype(LastTypeOfTsFunc<Ts...>()) type;
    };

    // Takes a member function that has an out param like F(..., IAsyncAction**) or F(..., IAsyncOperation<bool>**)
    // and returns IAsyncAction* or IAsyncOperation<bool>*.
    template<typename I, typename ...P>
    typename wistd::remove_pointer<typename LastType<P...>::type>::type GetReturnParamPointerType(HRESULT(STDMETHODCALLTYPE I::*)(P...));

    // Use to determine the result type of the async action/operation interfaces or example
    // decltype(GetAsyncResultType(action.get())) returns void
    void GetAsyncResultType(ABI::Windows::Foundation::IAsyncAction*);
    template <typename P> void GetAsyncResultType(ABI::Windows::Foundation::IAsyncActionWithProgress<P>*);
    template <typename T> typename wil::details::MapAsyncOpResultType<T>::type GetAsyncResultType(ABI::Windows::Foundation::IAsyncOperation<T>*);
    template <typename T, typename P> typename wil::details::MapAsyncOpProgressResultType<T, P>::type GetAsyncResultType(ABI::Windows::Foundation::IAsyncOperationWithProgress<T, P>*);

    // Use to determine the result type of the async action/operation interfaces or example
    // decltype(GetAsyncDelegateType(action.get())) returns void
    ABI::Windows::Foundation::IAsyncActionCompletedHandler* GetAsyncDelegateType(ABI::Windows::Foundation::IAsyncAction*);
    template <typename P> ABI::Windows::Foundation::IAsyncActionWithProgressCompletedHandler<P>* GetAsyncDelegateType(ABI::Windows::Foundation::IAsyncActionWithProgress<P>*);
    template <typename T> ABI::Windows::Foundation::IAsyncOperationCompletedHandler<T>* GetAsyncDelegateType(ABI::Windows::Foundation::IAsyncOperation<T>*);
    template <typename T, typename P> ABI::Windows::Foundation::IAsyncOperationWithProgressCompletedHandler<T, P>* GetAsyncDelegateType(ABI::Windows::Foundation::IAsyncOperationWithProgress<T, P>*);

    template <typename TBaseAgility, typename TIOperation, typename TFunction>
    HRESULT RunWhenCompleteAction(_In_ TIOperation operation, TFunction&& func) WI_NOEXCEPT
    {
        using namespace Microsoft::WRL;
        typedef wistd::remove_pointer_t<decltype(GetAsyncDelegateType(operation))> TIDelegate;

        auto callback = Callback<Implements<RuntimeClassFlags<ClassicCom>, TIDelegate, TBaseAgility>>(
            [func = wistd::forward<TFunction>(func)](TIOperation operation, ABI::Windows::Foundation::AsyncStatus status) mutable -> HRESULT
        {
            HRESULT hr = S_OK;
            if (status != ABI::Windows::Foundation::AsyncStatus::Completed)   // avoid a potentially costly marshaled QI / call if we completed successfully
            {
                // QI to the IAsyncInfo interface.  While all operations implement this, it is
                // possible that the stub has disconnected, causing the QI to fail.
                ComPtr<ABI::Windows::Foundation::IAsyncInfo> asyncInfo;
                hr = operation->QueryInterface(IID_PPV_ARGS(&asyncInfo));
                if (SUCCEEDED(hr))
                {
                    // Save the error code result in a temporary variable to allow us
                    // to also retrieve the result of the COM call.  If the stub has
                    // disconnected, this call may fail.
                    HRESULT errorCode = E_UNEXPECTED;
                    hr = asyncInfo->get_ErrorCode(&errorCode);
                    if (SUCCEEDED(hr))
                    {
                        // Return the operations error code to the caller.
                        hr = errorCode;
                    }
                }
            }

            return CallAndHandleErrors(func, hr);
        });
        RETURN_IF_NULL_ALLOC(callback);
        return operation->put_Completed(callback.Get());
    }

    template <typename TBaseAgility, typename TIOperation, typename TFunction>
    HRESULT RunWhenComplete(_In_ TIOperation operation, TFunction&& func) WI_NOEXCEPT
    {
        using namespace Microsoft::WRL;
        using namespace ABI::Windows::Foundation::Internal;

        typedef wistd::remove_pointer_t<decltype(GetAsyncDelegateType(operation))> TIDelegate;

        auto callback = Callback<Implements<RuntimeClassFlags<ClassicCom>, TIDelegate, TBaseAgility>>(
            [func = wistd::forward<TFunction>(func)](TIOperation operation, ABI::Windows::Foundation::AsyncStatus status) mutable -> HRESULT
        {
            typename details::MapToSmartType<typename GetAbiType<typename wistd::remove_pointer<TIOperation>::type::TResult_complex>::type>::type result;

            HRESULT hr = S_OK;
            // avoid a potentially costly marshaled QI / call if we completed successfully
            if (status == ABI::Windows::Foundation::AsyncStatus::Completed)
            {
                hr = operation->GetResults(result.GetAddressOf());
            }
            else
            {
                // QI to the IAsyncInfo interface.  While all operations implement this, it is
                // possible that the stub has disconnected, causing the QI to fail.
                ComPtr<ABI::Windows::Foundation::IAsyncInfo> asyncInfo;
                hr = operation->QueryInterface(IID_PPV_ARGS(&asyncInfo));
                if (SUCCEEDED(hr))
                {
                    // Save the error code result in a temporary variable to allow us
                    // to also retrieve the result of the COM call.  If the stub has
                    // disconnected, this call may fail.
                    HRESULT errorCode = E_UNEXPECTED;
                    hr = asyncInfo->get_ErrorCode(&errorCode);
                    if (SUCCEEDED(hr))
                    {
                        // Return the operations error code to the caller.
                        hr = errorCode;
                    }
                }
            }

            return CallAndHandleErrors(func, hr, result.Get());
        });
        RETURN_IF_NULL_ALLOC(callback);
        return operation->put_Completed(callback.Get());
    }

#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_SYSTEM)
    template <typename TIOperation>
    HRESULT WaitForCompletion(_In_ TIOperation operation, COWAIT_FLAGS flags, DWORD timeoutValue, _Out_opt_ bool* timedOut) WI_NOEXCEPT
    {
        typedef wistd::remove_pointer_t<decltype(GetAsyncDelegateType(operation))> TIDelegate;

        class CompletionDelegate : public Microsoft::WRL::RuntimeClass<Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::RuntimeClassType::Delegate>,
            TIDelegate, Microsoft::WRL::FtmBase>
        {
        public:
            HRESULT RuntimeClassInitialize()
            {
                RETURN_HR(m_completedEventHandle.create());
            }

            HRESULT STDMETHODCALLTYPE Invoke(_In_ TIOperation, ABI::Windows::Foundation::AsyncStatus status) override
            {
                m_status = status;
                m_completedEventHandle.SetEvent();
                return S_OK;
            }

            WI_NODISCARD HANDLE GetEvent() const
            {
                return m_completedEventHandle.get();
            }

            WI_NODISCARD ABI::Windows::Foundation::AsyncStatus GetStatus() const
            {
                return m_status;
            }

        private:
            volatile ABI::Windows::Foundation::AsyncStatus m_status = ABI::Windows::Foundation::AsyncStatus::Started;
            wil::unique_event_nothrow m_completedEventHandle;
        };

        WI_ASSERT(timedOut || (timeoutValue == INFINITE));
        assign_to_opt_param(timedOut, false);

        Microsoft::WRL::ComPtr<CompletionDelegate> completedDelegate;
        RETURN_IF_FAILED(Microsoft::WRL::MakeAndInitialize<CompletionDelegate>(&completedDelegate));
        RETURN_IF_FAILED(operation->put_Completed(completedDelegate.Get()));

        HANDLE handles[] = { completedDelegate->GetEvent() };
        DWORD dwHandleIndex;
        HRESULT hr = CoWaitForMultipleHandles(flags, timeoutValue, ARRAYSIZE(handles), handles, &dwHandleIndex);

        // If the caller is listening for timedOut, and we actually timed out, set the bool and return S_OK. Otherwise, fail.
        if (timedOut && (hr == RPC_S_CALLPENDING))
        {
            *timedOut = true;
            return S_OK;
        }
        RETURN_IF_FAILED(hr);

        if (completedDelegate->GetStatus() != ABI::Windows::Foundation::AsyncStatus::Completed)
        {
            // QI to the IAsyncInfo interface.  While all operations implement this, it is
            // possible that the stub has disconnected, causing the QI to fail.
            Microsoft::WRL::ComPtr<ABI::Windows::Foundation::IAsyncInfo> asyncInfo;
            hr = operation->QueryInterface(IID_PPV_ARGS(&asyncInfo));
            if (SUCCEEDED(hr))
            {
                // Save the error code result in a temporary variable to allow us
                // to also retrieve the result of the COM call.  If the stub has
                // disconnected, this call may fail.
                HRESULT errorCode = E_UNEXPECTED;
                hr = asyncInfo->get_ErrorCode(&errorCode);
                if (SUCCEEDED(hr))
                {
                    // Return the operations error code to the caller.
                    hr = errorCode;
                }
            }
            return hr; // leave it to the caller to log failures.
        }
        return S_OK;
    }

    template <typename TIOperation, typename TIResults>
    HRESULT WaitForCompletion(_In_ TIOperation operation, _Out_ TIResults result, COWAIT_FLAGS flags,
        DWORD timeoutValue, _Out_opt_ bool* timedOut) WI_NOEXCEPT
    {
        RETURN_IF_FAILED_EXPECTED(details::WaitForCompletion(operation, flags, timeoutValue, timedOut));
        return operation->GetResults(result);
    }
#endif // WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_SYSTEM)
}
/// @endcond

/** Set the completion callback for an async operation to run a caller provided function.
Once complete the function is called with the error code result of the operation
and the async operation result (if applicable).
The function parameter list must be (HRESULT hr) for actions,
(HRESULT hr, IResultInterface* object) for operations that produce interfaces,
and (HRESULT hr, TResult value) for operations that produce value types.
~~~
run_when_complete(getFileOp.Get(), [](HRESULT hr, IStorageFile* file) -> void
{

});
~~~
for an agile callback use Microsoft::WRL::FtmBase
~~~
run_when_complete<FtmBase>(getFileOp.Get(), [](HRESULT hr, IStorageFile* file) -> void
{

});
~~~
Using the non throwing form:
~~~
hr = run_when_complete_nothrow<StorageFile*>(getFileOp.Get(), [](HRESULT hr, IStorageFile* file) -> HRESULT
{

});
~~~
*/

//! Run a fuction when an async operation completes. Use Microsoft::WRL::FtmBase for TAgility to make the completion handler agile and run on the async thread.
template<typename TAgility = IUnknown, typename TFunc>
HRESULT run_when_complete_nothrow(_In_ ABI::Windows::Foundation::IAsyncAction* operation, TFunc&& func) WI_NOEXCEPT
{
    return details::RunWhenCompleteAction<TAgility>(operation, wistd::forward<TFunc>(func));
}

template<typename TAgility = IUnknown, typename TResult, typename TFunc, typename TAsyncResult = typename wil::details::MapAsyncOpResultType<TResult>::type>
HRESULT run_when_complete_nothrow(_In_ ABI::Windows::Foundation::IAsyncOperation<TResult>* operation, TFunc&& func) WI_NOEXCEPT
{
    return details::RunWhenComplete<TAgility>(operation, wistd::forward<TFunc>(func));
}

template<typename TAgility = IUnknown, typename TResult, typename TProgress, typename TFunc, typename TAsyncResult = typename wil::details::MapAsyncOpProgressResultType<TResult, TProgress>::type>
HRESULT run_when_complete_nothrow(_In_ ABI::Windows::Foundation::IAsyncOperationWithProgress<TResult, TProgress>* operation, TFunc&& func) WI_NOEXCEPT
{
    return details::RunWhenComplete<TAgility>(operation, wistd::forward<TFunc>(func));
}

template<typename TAgility = IUnknown, typename TProgress, typename TFunc>
HRESULT run_when_complete_nothrow(_In_ ABI::Windows::Foundation::IAsyncActionWithProgress<TProgress>* operation, TFunc&& func) WI_NOEXCEPT
{
    return details::RunWhenCompleteAction<TAgility>(operation, wistd::forward<TFunc>(func));
}

#ifdef WIL_ENABLE_EXCEPTIONS
//! Run a fuction when an async operation completes. Use Microsoft::WRL::FtmBase for TAgility to make the completion handler agile and run on the async thread.
template<typename TAgility = IUnknown, typename TFunc>
void run_when_complete(_In_ ABI::Windows::Foundation::IAsyncAction* operation, TFunc&& func)
{
    THROW_IF_FAILED((details::RunWhenCompleteAction<TAgility>(operation, wistd::forward<TFunc>(func))));
}

template<typename TAgility = IUnknown, typename TResult, typename TFunc, typename TAsyncResult = typename wil::details::MapAsyncOpResultType<TResult>::type>
void run_when_complete(_In_ ABI::Windows::Foundation::IAsyncOperation<TResult>* operation, TFunc&& func)
{
    THROW_IF_FAILED((details::RunWhenComplete<TAgility>(operation, wistd::forward<TFunc>(func))));
}

template<typename TAgility = IUnknown, typename TResult, typename TProgress, typename TFunc, typename TAsyncResult = typename wil::details::MapAsyncOpProgressResultType<TResult, TProgress>::type>
void run_when_complete(_In_ ABI::Windows::Foundation::IAsyncOperationWithProgress<TResult, TProgress>* operation, TFunc&& func)
{
    THROW_IF_FAILED((details::RunWhenComplete<TAgility>(operation, wistd::forward<TFunc>(func))));
}

template<typename TAgility = IUnknown, typename TProgress, typename TFunc>
void run_when_complete(_In_ ABI::Windows::Foundation::IAsyncActionWithProgress<TProgress>* operation, TFunc&& func)
{
    THROW_IF_FAILED((details::RunWhenCompleteAction<TAgility>(operation, wistd::forward<TFunc>(func))));
}
#endif

#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_SYSTEM)
/** Wait for an asynchronous operation to complete (or be canceled).
Use to synchronously wait on async operations on background threads.
Do not call from UI threads or STA threads as reentrancy will result.
~~~
ComPtr<IAsyncOperation<StorageFile*>> op;
THROW_IF_FAILED(storageFileStatics->GetFileFromPathAsync(HStringReference(path).Get(), &op));
auto file = wil::wait_for_completion(op.Get());
~~~
*/
template <typename TAsync = ABI::Windows::Foundation::IAsyncAction>
inline HRESULT wait_for_completion_nothrow(_In_ TAsync* operation, COWAIT_FLAGS flags = COWAIT_DISPATCH_CALLS) WI_NOEXCEPT
{
    return details::WaitForCompletion(operation, flags, INFINITE, nullptr);
}

// These forms return the result from the async operation

template <typename TResult>
HRESULT wait_for_completion_nothrow(_In_ ABI::Windows::Foundation::IAsyncOperation<TResult>* operation,
    _Out_ typename wil::details::MapAsyncOpResultType<TResult>::type* result,
    COWAIT_FLAGS flags = COWAIT_DISPATCH_CALLS) WI_NOEXCEPT
{
    return details::WaitForCompletion(operation, result, flags, INFINITE, nullptr);
}

template <typename TResult, typename TProgress>
HRESULT wait_for_completion_nothrow(_In_ ABI::Windows::Foundation::IAsyncOperationWithProgress<TResult, TProgress>* operation,
    _Out_ typename wil::details::MapAsyncOpProgressResultType<TResult, TProgress>::type* result,
    COWAIT_FLAGS flags = COWAIT_DISPATCH_CALLS) WI_NOEXCEPT
{
    return details::WaitForCompletion(operation, result, flags, INFINITE, nullptr);
}

// Same as above, but allows caller to specify a timeout value.
// On timeout, S_OK is returned, with timedOut set to true.

template <typename TAsync = ABI::Windows::Foundation::IAsyncAction>
inline HRESULT wait_for_completion_or_timeout_nothrow(_In_ TAsync* operation,
    DWORD timeoutValue, _Out_ bool* timedOut, COWAIT_FLAGS flags = COWAIT_DISPATCH_CALLS) WI_NOEXCEPT
{
    return details::WaitForCompletion(operation, flags, timeoutValue, timedOut);
}

template <typename TResult>
HRESULT wait_for_completion_or_timeout_nothrow(_In_ ABI::Windows::Foundation::IAsyncOperation<TResult>* operation,
    _Out_ typename wil::details::MapAsyncOpResultType<TResult>::type* result,
    DWORD timeoutValue, _Out_ bool* timedOut, COWAIT_FLAGS flags = COWAIT_DISPATCH_CALLS) WI_NOEXCEPT
{
    return details::WaitForCompletion(operation, result, flags, timeoutValue, timedOut);
}

template <typename TResult, typename TProgress>
HRESULT wait_for_completion_or_timeout_nothrow(_In_ ABI::Windows::Foundation::IAsyncOperationWithProgress<TResult, TProgress>* operation,
    _Out_ typename wil::details::MapAsyncOpProgressResultType<TResult, TProgress>::type* result,
    DWORD timeoutValue, _Out_ bool* timedOut, COWAIT_FLAGS flags = COWAIT_DISPATCH_CALLS) WI_NOEXCEPT
{
    return details::WaitForCompletion(operation, result, flags, timeoutValue, timedOut);
}

#ifdef WIL_ENABLE_EXCEPTIONS
//! Wait for an asynchronous operation to complete (or be canceled).
template <typename TAsync = ABI::Windows::Foundation::IAsyncAction>
inline void wait_for_completion(_In_ TAsync* operation, COWAIT_FLAGS flags = COWAIT_DISPATCH_CALLS)
{
    THROW_IF_FAILED(details::WaitForCompletion(operation, flags, INFINITE, nullptr));
}

template <typename TResult, typename TReturn = typename wil::details::MapToSmartType<typename wil::details::MapAsyncOpResultType<TResult>::type>::type>
TReturn
wait_for_completion(_In_ ABI::Windows::Foundation::IAsyncOperation<TResult>* operation, COWAIT_FLAGS flags = COWAIT_DISPATCH_CALLS)
{
    TReturn result;
    THROW_IF_FAILED(details::WaitForCompletion(operation, result.GetAddressOf(), flags, INFINITE, nullptr));
    return result;
}

template <typename TResult, typename TProgress, typename TReturn = typename wil::details::MapToSmartType<typename wil::details::MapAsyncOpProgressResultType<TResult, TProgress>::type>::type>
TReturn
wait_for_completion(_In_ ABI::Windows::Foundation::IAsyncOperationWithProgress<TResult, TProgress>* operation, COWAIT_FLAGS flags = COWAIT_DISPATCH_CALLS)
{
    TReturn result;
    THROW_IF_FAILED(details::WaitForCompletion(operation, result.GetAddressOf(), flags, INFINITE, nullptr));
    return result;
}

/** Similar to WaitForCompletion but this function encapsulates the creation of the async operation
making usage simpler.
~~~
ComPtr<ILauncherStatics> launcher; // inited somewhere
auto result = call_and_wait_for_completion(launcher.Get(), &ILauncherStatics::LaunchUriAsync, uri.Get());
~~~
*/
template<typename I, typename ...P, typename ...Args>
auto call_and_wait_for_completion(I* object, HRESULT(STDMETHODCALLTYPE I::*func)(P...), Args&&... args)
{
    Microsoft::WRL::ComPtr<typename wistd::remove_pointer<typename wistd::remove_pointer<typename details::LastType<P...>::type>::type>::type> op;
    THROW_IF_FAILED((object->*func)(wistd::forward<Args>(args)..., &op));
    return wil::wait_for_completion(op.Get());
}
#endif
#endif // WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_SYSTEM)

#pragma endregion

#pragma region WinRT object construction
#ifdef WIL_ENABLE_EXCEPTIONS
//! Get a WinRT activation factory object, usually using a IXXXStatics interface.
template <typename TInterface>
com_ptr<TInterface> GetActivationFactory(PCWSTR runtimeClass)
{
    com_ptr<TInterface> result;
    THROW_IF_FAILED(RoGetActivationFactory(Microsoft::WRL::Wrappers::HStringReference(runtimeClass).Get(), IID_PPV_ARGS(&result)));
    return result;
}

//! Get a WinRT object.
template <typename TInterface>
com_ptr<TInterface> ActivateInstance(PCWSTR runtimeClass)
{
    com_ptr<IInspectable> result;
    THROW_IF_FAILED(RoActivateInstance(Microsoft::WRL::Wrappers::HStringReference(runtimeClass).Get(), &result));
    return result.query<TInterface>();
}
#endif
#pragma endregion

#pragma region Async production helpers

/// @cond
namespace details
{
    template <typename TResult>
    class SyncAsyncOp WrlFinal : public Microsoft::WRL::RuntimeClass<ABI::Windows::Foundation::IAsyncOperation<TResult>,
        Microsoft::WRL::AsyncBase<ABI::Windows::Foundation::IAsyncOperationCompletedHandler<TResult>>>
    {
        // typedef typename MapToSmartType<TResult>::type TSmart;
        using RuntimeClassT = typename SyncAsyncOp::RuntimeClassT;
        InspectableClass(__super::z_get_rc_name_impl(), TrustLevel::BaseTrust);
    public:
        HRESULT RuntimeClassInitialize(const TResult& op)
        {
            m_result = op;
            return S_OK;
        }

        IFACEMETHODIMP put_Completed(ABI::Windows::Foundation::IAsyncOperationCompletedHandler<TResult>* competed) override
        {
            competed->Invoke(this, ABI::Windows::Foundation::AsyncStatus::Completed);
            return S_OK;
        }

        IFACEMETHODIMP get_Completed(ABI::Windows::Foundation::IAsyncOperationCompletedHandler<TResult>** competed) override
        {
            *competed = nullptr;
            return S_OK;
        }

        IFACEMETHODIMP GetResults(TResult* result) override
        {
            *result = m_result;
            return S_OK;
        }

        HRESULT OnStart() override { return S_OK; }
        void OnClose() override { }
        void OnCancel() override { }
    private:
        // needs to be MapToSmartType<TResult>::type to hold non trial types
        TResult m_result{};
    };

    extern const __declspec(selectany) wchar_t SyncAsyncActionName[] = L"SyncActionAction";

    class SyncAsyncActionOp WrlFinal : public Microsoft::WRL::RuntimeClass<ABI::Windows::Foundation::IAsyncAction,
        Microsoft::WRL::AsyncBase<ABI::Windows::Foundation::IAsyncActionCompletedHandler,
        Microsoft::WRL::Details::Nil,
        Microsoft::WRL::AsyncResultType::SingleResult
#ifndef _WRL_DISABLE_CAUSALITY_
        ,Microsoft::WRL::AsyncCausalityOptions<SyncAsyncActionName>
#endif
        >>
    {
        InspectableClass(InterfaceName_Windows_Foundation_IAsyncAction, TrustLevel::BaseTrust);
    public:
        IFACEMETHODIMP put_Completed(ABI::Windows::Foundation::IAsyncActionCompletedHandler* competed) override
        {
            competed->Invoke(this, ABI::Windows::Foundation::AsyncStatus::Completed);
            return S_OK;
        }

        IFACEMETHODIMP get_Completed(ABI::Windows::Foundation::IAsyncActionCompletedHandler** competed) override
        {
            *competed = nullptr;
            return S_OK;
        }

        IFACEMETHODIMP GetResults() override
        {
            return S_OK;
        }

        HRESULT OnStart() override { return S_OK; }
        void OnClose() override { }
        void OnCancel() override { }
    };
}

/// @endcond
//! Creates a WinRT async operation object that implements IAsyncOperation<TResult>. Use mostly for testing and for mocking APIs.
template <typename TResult>
HRESULT make_synchronous_async_operation_nothrow(ABI::Windows::Foundation::IAsyncOperation<TResult>** result, const TResult& value)
{
    return Microsoft::WRL::MakeAndInitialize<details::SyncAsyncOp<TResult>>(result, value);
}

//! Creates a WinRT async operation object that implements IAsyncAction. Use mostly for testing and for mocking APIs.
inline HRESULT make_synchronous_async_action_nothrow(ABI::Windows::Foundation::IAsyncAction** result)
{
    return Microsoft::WRL::MakeAndInitialize<details::SyncAsyncActionOp>(result);
}

#ifdef WIL_ENABLE_EXCEPTIONS
//! Creates a WinRT async operation object that implements IAsyncOperation<TResult>. Use mostly for testing and for mocking APIs.
// TODO: map TRealResult and TSmartResult into SyncAsyncOp.
template <typename TResult, typename TRealResult = typename details::MapAsyncOpResultType<TResult>::type, typename TSmartResult = typename details::MapToSmartType<TRealResult>::type>
void make_synchronous_async_operation(ABI::Windows::Foundation::IAsyncOperation<TResult>** result, const TResult& value)
{
    THROW_IF_FAILED((Microsoft::WRL::MakeAndInitialize<details::SyncAsyncOp<TResult>>(result, value)));
}

//! Creates a WinRT async operation object that implements IAsyncAction. Use mostly for testing and for mocking APIs.
inline void make_synchronous_async_action(ABI::Windows::Foundation::IAsyncAction** result)
{
    THROW_IF_FAILED((Microsoft::WRL::MakeAndInitialize<details::SyncAsyncActionOp>(result)));
}
#endif
#pragma endregion

#pragma region EventRegistrationToken RAII wrapper

// unique_winrt_event_token[_cx] is an RAII wrapper around EventRegistrationToken. When the unique_winrt_event_token[_cx] is
// destroyed, the event is automatically unregistered. Declare a wil::unique_winrt_event_token[_cx]<T> at the scope the event
// should be registered for (often they are tied to object lifetime), where T is the type of the event sender
//     wil::unique_winrt_event_token_cx<Windows::UI::Xaml::Controls::Button> m_token;
//
// Macros have been defined to register for handling the event and then returning an unique_winrt_event_token[_cx]. These
// macros simply hide the function references for adding and removing the event.
//     C++/CX  m_token = WI_MakeUniqueWinRtEventTokenCx(ExampleEventName, sender, handler);
//     ABI     m_token = WI_MakeUniqueWinRtEventToken(ExampleEventName, sender, handler, &m_token);                 // Exception and failfast
//     ABI     RETURN_IF_FAILED(WI_MakeUniqueWinRtEventTokenNoThrow(ExampleEventName, sender, handler, &m_token));  // No throw variant
//
// When the wrapper is destroyed, the handler will be unregistered. You can explicitly unregister the handler prior.
//     m_token.reset();
//
// You can release the EventRegistrationToken from being managed by the wrapper by calling .release()
//     m_token.release();  // DANGER: no longer being managed
//
// If you just need the value of the EventRegistrationToken you can call .get()
//     m_token.get();
//
// See "onecore\shell\tests\wil\UniqueWinRTEventTokenTests.cpp" for more examples of usage in ABI and C++/CX.

#ifdef __cplusplus_winrt
namespace details
{
    template<typename T> struct remove_reference { typedef T type; };
    template<typename T> struct remove_reference<T^> { typedef T type; };
}

template<typename T>
class unique_winrt_event_token_cx
{
    using removal_func = void(T::*)(Windows::Foundation::EventRegistrationToken);
    using static_removal_func = void(__cdecl *)(Windows::Foundation::EventRegistrationToken);

public:
    unique_winrt_event_token_cx() = default;

    unique_winrt_event_token_cx(Windows::Foundation::EventRegistrationToken token, T^ sender, removal_func removalFunction) WI_NOEXCEPT :
        m_token(token),
        m_weakSender(sender),
        m_removalFunction(removalFunction)
    {}

    unique_winrt_event_token_cx(Windows::Foundation::EventRegistrationToken token, static_removal_func removalFunction) WI_NOEXCEPT :
        m_token(token),
        m_staticRemovalFunction(removalFunction)
    {}

    unique_winrt_event_token_cx(const unique_winrt_event_token_cx&) = delete;
    unique_winrt_event_token_cx& operator=(const unique_winrt_event_token_cx&) = delete;

    unique_winrt_event_token_cx(unique_winrt_event_token_cx&& other) WI_NOEXCEPT :
        m_token(other.m_token),
        m_weakSender(wistd::move(other.m_weakSender)),
        m_removalFunction(other.m_removalFunction),
        m_staticRemovalFunction(other.m_staticRemovalFunction)
    {
        other.m_token = {};
        other.m_weakSender = nullptr;
        other.m_removalFunction = nullptr;
        other.m_staticRemovalFunction = nullptr;
    }

    unique_winrt_event_token_cx& operator=(unique_winrt_event_token_cx&& other) WI_NOEXCEPT
    {
        if (this != wistd::addressof(other))
        {
            reset();

            wistd::swap_wil(m_token, other.m_token);
            wistd::swap_wil(m_weakSender, other.m_weakSender);
            wistd::swap_wil(m_removalFunction, other.m_removalFunction);
            wistd::swap_wil(m_staticRemovalFunction, other.m_staticRemovalFunction);
        }

        return *this;
    }

    ~unique_winrt_event_token_cx() WI_NOEXCEPT
    {
        reset();
    }

    WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
    {
        return (m_token.Value != 0);
    }

    WI_NODISCARD Windows::Foundation::EventRegistrationToken get() const WI_NOEXCEPT
    {
        return m_token;
    }

    void reset() noexcept
    {
        if (m_token.Value != 0)
        {
            if (m_staticRemovalFunction)
            {
                (*m_staticRemovalFunction)(m_token);
            }
            else
            {
                auto resolvedSender = [&]()
                {
                    try
                    {
                        return m_weakSender.Resolve<T>();
                    }
                    catch (...)
                    {
                        // Ignore RPC or other failures that are unavoidable in some cases
                        // matching wil::unique_winrt_event_token and winrt::event_revoker
                        return static_cast<T^>(nullptr);
                    }
                }();
                if (resolvedSender)
                {
                    (resolvedSender->*m_removalFunction)(m_token);
                }
            }
            release();
        }
    }

    // Stops the wrapper from managing resource and returns the EventRegistrationToken.
    Windows::Foundation::EventRegistrationToken release() WI_NOEXCEPT
    {
        auto token = m_token;
        m_token = {};
        m_weakSender = nullptr;
        m_removalFunction = nullptr;
        m_staticRemovalFunction = nullptr;
        return token;
    }

private:
    Windows::Foundation::EventRegistrationToken m_token = {};
    Platform::WeakReference m_weakSender;
    removal_func m_removalFunction = nullptr;
    static_removal_func m_staticRemovalFunction = nullptr;
};

#endif

template<typename T>
class unique_winrt_event_token
{
    using removal_func = HRESULT(__stdcall T::*)(::EventRegistrationToken);

public:
    unique_winrt_event_token() = default;

    unique_winrt_event_token(::EventRegistrationToken token, T* sender, removal_func removalFunction) WI_NOEXCEPT :
        m_token(token),
        m_removalFunction(removalFunction)
    {
        m_weakSender = wil::com_weak_query_failfast(sender);
    }

    unique_winrt_event_token(const unique_winrt_event_token&) = delete;
    unique_winrt_event_token& operator=(const unique_winrt_event_token&) = delete;

    unique_winrt_event_token(unique_winrt_event_token&& other) WI_NOEXCEPT :
        m_token(other.m_token),
        m_weakSender(wistd::move(other.m_weakSender)),
        m_removalFunction(other.m_removalFunction)
    {
        other.m_token = {};
        other.m_removalFunction = nullptr;
    }

    unique_winrt_event_token& operator=(unique_winrt_event_token&& other) WI_NOEXCEPT
    {
        if (this != wistd::addressof(other))
        {
            reset();

            wistd::swap_wil(m_token, other.m_token);
            wistd::swap_wil(m_weakSender, other.m_weakSender);
            wistd::swap_wil(m_removalFunction, other.m_removalFunction);
        }

        return *this;
    }

    ~unique_winrt_event_token() WI_NOEXCEPT
    {
        reset();
    }

    WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
    {
        return (m_token.value != 0);
    }

    WI_NODISCARD ::EventRegistrationToken get() const WI_NOEXCEPT
    {
        return m_token;
    }

    void reset() WI_NOEXCEPT
    {
        if (m_token.value != 0)
        {
            // If T cannot be QI'ed from the weak object then T is not a COM interface.
            auto resolvedSender = m_weakSender.try_query<T>();
            if (resolvedSender)
            {
                FAIL_FAST_IF_FAILED((resolvedSender.get()->*m_removalFunction)(m_token));
            }
            release();
        }
    }

    // Stops the wrapper from managing resource and returns the EventRegistrationToken.
    ::EventRegistrationToken release() WI_NOEXCEPT
    {
        auto token = m_token;
        m_token = {};
        m_weakSender = nullptr;
        m_removalFunction = nullptr;
        return token;
    }

private:
    ::EventRegistrationToken m_token = {};
    wil::com_weak_ref_failfast m_weakSender;
    removal_func m_removalFunction = nullptr;
};

namespace details
{
#ifdef __cplusplus_winrt

    // Handles registration of the event handler to the subscribing object and then wraps the EventRegistrationToken in unique_winrt_event_token.
    // Not intended to be directly called. Use the WI_MakeUniqueWinRtEventTokenCx macro to abstract away specifying the functions that handle addition and removal.
    template<typename T, typename addition_func, typename removal_func, typename handler>
    inline wil::unique_winrt_event_token_cx<T> make_unique_winrt_event_token_cx(T^ sender, addition_func additionFunc, removal_func removalFunc, handler^ h)
    {
        auto rawToken = (sender->*additionFunc)(h);
        wil::unique_winrt_event_token_cx<T> temp(rawToken, sender, removalFunc);
        return temp;
    }

    template<typename T, typename addition_func, typename removal_func, typename handler>
    inline wil::unique_winrt_event_token_cx<T> make_unique_winrt_static_event_token_cx(addition_func additionFunc, removal_func removalFunc, handler^ h)
    {
        auto rawToken = (*additionFunc)(h);
        wil::unique_winrt_event_token_cx<T> temp(rawToken, removalFunc);
        return temp;
    }

#endif // __cplusplus_winrt

    // Handles registration of the event handler to the subscribing object and then wraps the EventRegistrationToken in unique_winrt_event_token.
    // Not intended to be directly called. Use the WI_MakeUniqueWinRtEventToken macro to abstract away specifying the functions that handle addition and removal.
    template<typename err_policy = wil::err_returncode_policy, typename T, typename addition_func, typename removal_func, typename handler>
    inline auto make_unique_winrt_event_token(T* sender, addition_func additionFunc, removal_func removalFunc, handler h, wil::unique_winrt_event_token<T>* token_reference)
    {
        ::EventRegistrationToken rawToken;
        err_policy::HResult((sender->*additionFunc)(h, &rawToken));
        *token_reference = wil::unique_winrt_event_token<T>(rawToken, sender, removalFunc);
        return err_policy::OK();
    }

    // Overload make function to allow for returning the constructed object when not using HRESULT based code.
    template<typename err_policy = wil::err_returncode_policy, typename T, typename addition_func, typename removal_func, typename handler>
    inline typename wistd::enable_if<!wistd::is_same<err_policy, wil::err_returncode_policy>::value, wil::unique_winrt_event_token<T>>::type
    make_unique_winrt_event_token(T* sender, addition_func additionFunc, removal_func removalFunc, handler h)
    {
        ::EventRegistrationToken rawToken;
        err_policy::HResult((sender->*additionFunc)(h, &rawToken));
        return wil::unique_winrt_event_token<T>(rawToken, sender, removalFunc);
    }

} // namespace details

// Helper macros to abstract function names for event addition and removal.
#ifdef __cplusplus_winrt

#define WI_MakeUniqueWinRtEventTokenCx(_event, _object, _handler) \
    wil::details::make_unique_winrt_event_token_cx( \
        _object, \
        &wil::details::remove_reference<decltype(_object)>::type::##_event##::add, \
        &wil::details::remove_reference<decltype(_object)>::type::##_event##::remove, \
        _handler)

#define WI_MakeUniqueWinRtStaticEventTokenCx(_event, _baseType, _handler) \
    wil::details::make_unique_winrt_static_event_token_cx<_baseType>( \
        &##_baseType##::##_event##::add, \
        &##_baseType##::##_event##::remove, \
        _handler)

#endif // __cplusplus_winrt

#ifdef WIL_ENABLE_EXCEPTIONS

#define WI_MakeUniqueWinRtEventToken(_event, _object, _handler) \
    wil::details::make_unique_winrt_event_token<wil::err_exception_policy>( \
        _object, \
        &wistd::remove_pointer<decltype(_object)>::type::add_##_event, \
        &wistd::remove_pointer<decltype(_object)>::type::remove_##_event, \
        _handler)

#endif // WIL_ENABLE_EXCEPTIONS

#define WI_MakeUniqueWinRtEventTokenNoThrow(_event, _object, _handler, _token_reference) \
    wil::details::make_unique_winrt_event_token( \
        _object, \
        &wistd::remove_pointer<decltype(_object)>::type::add_##_event, \
        &wistd::remove_pointer<decltype(_object)>::type::remove_##_event, \
        _handler, \
        _token_reference)

#define WI_MakeUniqueWinRtEventTokenFailFast(_event, _object, _handler) \
    wil::details::make_unique_winrt_event_token<wil::err_failfast_policy>( \
        _object, \
        &wistd::remove_pointer<decltype(_object)>::type::add_##_event, \
        &wistd::remove_pointer<decltype(_object)>::type::remove_##_event, \
        _handler)

#pragma endregion // EventRegistrationToken RAII wrapper

} // namespace wil

#if (NTDDI_VERSION >= NTDDI_WINBLUE)

template <>
struct ABI::Windows::Foundation::IAsyncOperation<ABI::Windows::Foundation::IAsyncAction*> :
    ABI::Windows::Foundation::IAsyncOperation_impl<ABI::Windows::Foundation::IAsyncAction*>
{
    static const wchar_t* z_get_rc_name_impl()
    {
        return L"IAsyncOperation<IAsyncAction*>";
    }
};

template <typename P>
struct ABI::Windows::Foundation::IAsyncOperationWithProgress<ABI::Windows::Foundation::IAsyncAction*,P> :
    ABI::Windows::Foundation::IAsyncOperationWithProgress_impl<ABI::Windows::Foundation::IAsyncAction*, P>
{
    static const wchar_t* z_get_rc_name_impl()
    {
        return L"IAsyncOperationWithProgress<IAsyncAction*,P>";
    }
};

template <typename T>
struct ABI::Windows::Foundation::IAsyncOperation<ABI::Windows::Foundation::IAsyncOperation<T>*> :
    ABI::Windows::Foundation::IAsyncOperation_impl<ABI::Windows::Foundation::IAsyncOperation<T>*>
{
    static const wchar_t* z_get_rc_name_impl()
    {
        return L"IAsyncOperation<IAsyncOperation<T>*>";
    }
};

template <typename T, typename P>
struct ABI::Windows::Foundation::IAsyncOperationWithProgress<ABI::Windows::Foundation::IAsyncOperation<T>*, P> :
    ABI::Windows::Foundation::IAsyncOperationWithProgress_impl<ABI::Windows::Foundation::IAsyncOperation<T>*, P>
{
    static const wchar_t* z_get_rc_name_impl()
    {
        return L"IAsyncOperationWithProgress<IAsyncOperation<T>*,P>";
    }
};

template <typename T, typename P>
struct ABI::Windows::Foundation::IAsyncOperation<ABI::Windows::Foundation::IAsyncOperationWithProgress<T, P>*> :
    ABI::Windows::Foundation::IAsyncOperation_impl<ABI::Windows::Foundation::IAsyncOperationWithProgress<T, P>*>
{
    static const wchar_t* z_get_rc_name_impl()
    {
        return L"IAsyncOperation<IAsyncOperationWithProgress<T,P>*>";
    }
};

template <typename T, typename P, typename Z>
struct ABI::Windows::Foundation::IAsyncOperationWithProgress<ABI::Windows::Foundation::IAsyncOperationWithProgress<T,P>*, Z> :
    ABI::Windows::Foundation::IAsyncOperationWithProgress_impl<ABI::Windows::Foundation::IAsyncOperationWithProgress<T,P>*, Z>
{
    static const wchar_t* z_get_rc_name_impl()
    {
        return L"IAsyncOperationWithProgress<IAsyncOperationWithProgress<T,P>*,Z>";
    }
};

template <>
struct ABI::Windows::Foundation::IAsyncOperationCompletedHandler<ABI::Windows::Foundation::IAsyncAction*> :
    ABI::Windows::Foundation::IAsyncOperationCompletedHandler_impl<ABI::Windows::Foundation::IAsyncAction*>
{
    static const wchar_t* z_get_rc_name_impl()
    {
        return L"IAsyncOperationCompletedHandler<IAsyncAction*>";
    }
};

template <typename P>
struct ABI::Windows::Foundation::IAsyncOperationWithProgressCompletedHandler<ABI::Windows::Foundation::IAsyncAction*, P> :
    ABI::Windows::Foundation::IAsyncOperationWithProgressCompletedHandler_impl<ABI::Windows::Foundation::IAsyncAction*, P>
{
    static const wchar_t* z_get_rc_name_impl()
    {
        return L"IAsyncOperationWithProgressCompletedHandler<IAsyncAction*,P>";
    }
};

template <typename T>
struct ABI::Windows::Foundation::IAsyncOperationCompletedHandler<ABI::Windows::Foundation::IAsyncOperation<T>*> :
    ABI::Windows::Foundation::IAsyncOperationCompletedHandler_impl<ABI::Windows::Foundation::IAsyncOperation<T>*>
{
    static const wchar_t* z_get_rc_name_impl()
    {
        return L"IAsyncOperationCompletedHandler<IAsyncOperation<T>*>";
    }
};

template <typename T, typename P>
struct ABI::Windows::Foundation::IAsyncOperationWithProgressCompletedHandler<ABI::Windows::Foundation::IAsyncOperation<T>*, P> :
    ABI::Windows::Foundation::IAsyncOperationWithProgressCompletedHandler_impl<ABI::Windows::Foundation::IAsyncOperation<T>*, P>
{
    static const wchar_t* z_get_rc_name_impl()
    {
        return L"IAsyncOperationWithProgressCompletedHandler<IAsyncOperation<T>*,P>";
    }
};

template <typename T, typename P>
struct ABI::Windows::Foundation::IAsyncOperationCompletedHandler<ABI::Windows::Foundation::IAsyncOperationWithProgress<T, P>*> :
    ABI::Windows::Foundation::IAsyncOperationCompletedHandler_impl<ABI::Windows::Foundation::IAsyncOperationWithProgress<T, P>*>
{
    static const wchar_t* z_get_rc_name_impl()
    {
        return L"IAsyncOperationCompletedHandler<IAsyncOperationWithProgress<T>*>";
    }
};

template <typename T, typename P, typename Z>
struct ABI::Windows::Foundation::IAsyncOperationWithProgressCompletedHandler<ABI::Windows::Foundation::IAsyncOperationWithProgress<T, P>*, Z> :
    ABI::Windows::Foundation::IAsyncOperationWithProgressCompletedHandler_impl<ABI::Windows::Foundation::IAsyncOperationWithProgress<T, P>*, Z>
{
    static const wchar_t* z_get_rc_name_impl()
    {
        return L"IAsyncOperationWithProgressCompletedHandler<IAsyncOperationWithProgress<T,P>*,Z>";
    }
};
#endif // NTDDI_VERSION >= NTDDI_WINBLUE

#if !defined(MIDL_NS_PREFIX) && !defined(____x_ABI_CWindows_CFoundation_CIClosable_FWD_DEFINED__)
// Internal .idl files use the namespace without the ABI prefix. Macro out ABI for that case
#pragma pop_macro("ABI")
#endif

#if __WI_HAS_STD_LESS

namespace std
{
    //! Specialization of `std::less` for `Microsoft::WRL::Wrappers::HString` that uses `hstring_less` for the
    //! comparison function object.
    template <>
    struct less<Microsoft::WRL::Wrappers::HString> :
        public wil::hstring_less
    {
    };

    //! Specialization of `std::less` for `wil::unique_hstring` that uses `hstring_less` for the comparison function
    //! object.
    template <>
    struct less<wil::unique_hstring> :
        public wil::hstring_less
    {
    };
}

#endif

#endif // __WIL_WINRT_INCLUDED
