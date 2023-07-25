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
#ifndef __WIL_STL_INCLUDED
#define __WIL_STL_INCLUDED

#include "common.h"
#include "resource.h"
#include <memory>
#include <string>
#include <vector>
#include <utility>
#if _HAS_CXX17
#include <string_view>
#endif

#ifndef WI_STL_FAIL_FAST_IF
#define WI_STL_FAIL_FAST_IF FAIL_FAST_IF
#endif

#if defined(WIL_ENABLE_EXCEPTIONS)

namespace wil
{
    /** Secure allocator for STL containers.
    The `wil::secure_allocator` allocator calls `SecureZeroMemory` before deallocating
    memory. This provides a mechanism for secure STL containers such as `wil::secure_vector`,
    `wil::secure_string`, and `wil::secure_wstring`. */
    template <typename T>
    struct secure_allocator
        : public std::allocator<T>
    {
        template<typename Other>
        struct rebind
        {
            using other = secure_allocator<Other>;
        };

        secure_allocator()
            : std::allocator<T>()
        {
        }

        ~secure_allocator() = default;

        secure_allocator(const secure_allocator& a)
            : std::allocator<T>(a)
        {
        }

        template <class U>
        secure_allocator(const secure_allocator<U>& a)
            : std::allocator<T>(a)
        {
        }

        T* allocate(size_t n)
        {
            return std::allocator<T>::allocate(n);
        }

        void deallocate(T* p, size_t n)
        {
            SecureZeroMemory(p, sizeof(T) * n);
            std::allocator<T>::deallocate(p, n);
        }
    };

    //! `wil::secure_vector` will be securely zeroed before deallocation.
    template <typename Type>
    using secure_vector = std::vector<Type, secure_allocator<Type>>;
    //! `wil::secure_wstring` will be securely zeroed before deallocation.
    using secure_wstring = std::basic_string<wchar_t, std::char_traits<wchar_t>, wil::secure_allocator<wchar_t>>;
    //! `wil::secure_string` will be securely zeroed before deallocation.
    using secure_string = std::basic_string<char, std::char_traits<char>, wil::secure_allocator<char>>;

    /// @cond
    namespace details
    {
        template<> struct string_maker<std::wstring>
        {
            HRESULT make(_In_reads_opt_(length) PCWSTR source, size_t length) WI_NOEXCEPT try
            {
                m_value = source ? std::wstring(source, length) : std::wstring(length, L'\0');
                return S_OK;
            }
            catch (...)
            {
                return E_OUTOFMEMORY;
            }

            wchar_t* buffer() { return &m_value[0]; }

            HRESULT trim_at_existing_null(size_t length) { m_value.erase(length); return S_OK; }

            std::wstring release() { return std::wstring(std::move(m_value)); }

            static PCWSTR get(const std::wstring& value) { return value.c_str(); }

        private:
            std::wstring m_value;
        };
    }
    /// @endcond

    // str_raw_ptr is an overloaded function that retrieves a const pointer to the first character in a string's buffer.
    // This is the overload for std::wstring.  Other overloads available in resource.h.
    inline PCWSTR str_raw_ptr(const std::wstring& str)
    {
        return str.c_str();
    }

#if _HAS_CXX17
    /**
        zstring_view. A zstring_view is identical to a std::string_view except it is always nul-terminated (unless empty).
        * zstring_view can be used for storing string literals without "forgetting" the length or that it is nul-terminated.
        * A zstring_view can be converted implicitly to a std::string_view because it is always safe to use a nul-terminated
          string_view as a plain string view.
        * A zstring_view can be constructed from a std::string because the data in std::string is nul-terminated.
    */
    template<class TChar>
    class basic_zstring_view : public std::basic_string_view<TChar>
    {
        using size_type = typename std::basic_string_view<TChar>::size_type;

    public:
        constexpr basic_zstring_view() noexcept = default;
        constexpr basic_zstring_view(const basic_zstring_view&) noexcept = default;
        constexpr basic_zstring_view& operator=(const basic_zstring_view&) noexcept = default;

        constexpr basic_zstring_view(const TChar* pStringData, size_type stringLength) noexcept
            : std::basic_string_view<TChar>(pStringData, stringLength)
        {
            if (pStringData[stringLength] != 0) { WI_STL_FAIL_FAST_IF(true); }
        }

        template<size_t stringArrayLength>
        constexpr basic_zstring_view(const TChar(&stringArray)[stringArrayLength]) noexcept
            : std::basic_string_view<TChar>(&stringArray[0], length_n(&stringArray[0], stringArrayLength))
        {
        }

        // Construct from nul-terminated char ptr. To prevent this from overshadowing array construction,
        // we disable this constructor if the value is an array (including string literal).
        template<typename TPtr, std::enable_if_t<
            std::is_convertible<TPtr, const TChar*>::value && !std::is_array<TPtr>::value>* = nullptr>
        constexpr basic_zstring_view(TPtr&& pStr) noexcept
            : std::basic_string_view<TChar>(std::forward<TPtr>(pStr)) {}

        constexpr basic_zstring_view(const std::basic_string<TChar>& str) noexcept
            : std::basic_string_view<TChar>(&str[0], str.size()) {}

        // basic_string_view [] precondition won't let us read view[view.size()]; so we define our own.
        WI_NODISCARD constexpr const TChar& operator[](size_type idx) const noexcept
        {
            WI_ASSERT(idx <= this->size() && this->data() != nullptr);
            return this->data()[idx];
        }

        WI_NODISCARD constexpr const TChar* c_str() const noexcept
        {
            WI_ASSERT(this->data() == nullptr || this->data()[this->size()] == 0);
            return this->data();
        }

    private:
        // Bounds-checked version of char_traits::length, like strnlen. Requires that the input contains a null terminator.
        static constexpr size_type length_n(_In_reads_opt_(buf_size) const TChar* str, size_type buf_size) noexcept
        {
            const std::basic_string_view<TChar> view(str, buf_size);
            auto pos = view.find_first_of(TChar());
            if (pos == view.npos) { WI_STL_FAIL_FAST_IF(true); }
            return pos;
        }

        // The following basic_string_view methods must not be allowed because they break the nul-termination.
        using std::basic_string_view<TChar>::swap;
        using std::basic_string_view<TChar>::remove_suffix;
    };

    using zstring_view = basic_zstring_view<char>;
    using zwstring_view = basic_zstring_view<wchar_t>;
#endif // _HAS_CXX17

} // namespace wil

#endif // WIL_ENABLE_EXCEPTIONS

#endif // __WIL_STL_INCLUDED
