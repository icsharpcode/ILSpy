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
#ifndef __WIL_REGISTRY_HELPERS_INCLUDED
#define __WIL_REGISTRY_HELPERS_INCLUDED

#if defined(_STRING_) || defined (_VECTOR_) || (defined (__cpp_lib_optional) && defined (_OPTIONAL_))
#include <functional>
#include <iterator>
#endif

#include <stdint.h>
#include <Windows.h>
#include "resource.h"

#ifdef _KERNEL_MODE
#error This header is not supported in kernel-mode.
#endif

namespace wil
{
    namespace reg
    {
        /**
         * \brief Helper function to translate registry return values if the value was not found
         * \param hr HRESULT to test from registry APIs
         * \return boolean if the HRESULT indicates the registry value was not found
         */
        constexpr bool is_registry_not_found(HRESULT hr) WI_NOEXCEPT
        {
            return (hr == __HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND)) ||
                (hr == __HRESULT_FROM_WIN32(ERROR_PATH_NOT_FOUND));
        }

        /**
         * \brief Helper function to translate registry return values if the buffer was too small
         * \param hr HRESULT to test from registry APIs
         * \return boolean if the HRESULT indicates the buffer was too small for the value being read
         */
        constexpr bool is_registry_buffer_too_small(HRESULT hr) WI_NOEXCEPT
        {
            return hr == __HRESULT_FROM_WIN32(ERROR_MORE_DATA);
        }

        // Access rights for opening registry keys. See https://learn.microsoft.com/en-us/windows/win32/sysinfo/registry-key-security-and-access-rights.
        enum class key_access
        {
            // Open key for reading.
            read,

            // Open key for reading and writing. Equivalent to KEY_ALL_ACCESS.
            readwrite,
        };

        namespace reg_view_details
        {
            constexpr DWORD get_value_flags_from_value_type(DWORD type) WI_NOEXCEPT
            {
                switch (type)
                {
                case REG_DWORD:
                    return RRF_RT_REG_DWORD;
                case REG_QWORD:
                    return RRF_RT_REG_QWORD;
                case REG_SZ:
                    return RRF_RT_REG_SZ | RRF_RT_REG_EXPAND_SZ | RRF_NOEXPAND;
                case REG_EXPAND_SZ:
                    return RRF_RT_REG_SZ | RRF_RT_REG_EXPAND_SZ;
                case REG_MULTI_SZ:
                    return RRF_RT_REG_MULTI_SZ;
                case REG_BINARY:
                    return RRF_RT_REG_BINARY;
                    // the caller can directly specify their own flags if they need to
                default:
                    return type;
                }
            }

            constexpr DWORD get_access_flags(key_access access) WI_NOEXCEPT
            {
                switch (access)
                {
                case key_access::read:
                    return KEY_READ;
                case key_access::readwrite:
                    return KEY_ALL_ACCESS;
                }
                FAIL_FAST();
            }

            /**
             * \brief A utility function that walks a contigous wchar_t container looking for strings within a multi-string
             * \tparam InputIt An iterator type that reference a container that holds wchar_t characters to translate into individual strings
             * \tparam Fn A callback function to be called each time a string is found - given the [begin, end] iterators referencing the found string
             * \param first An iterator referencing to the beginning of the target container (like a std::begin iterator)
             * \param last An iterator referencing one-past-the-end of the target container (like a std::end iterator)
             * \param func A callback function to be called each time a string is found - given the [begin, end] iterators referencing the found string
             */
            template <class InputIt, class Fn>
            void walk_multistring(const InputIt& first, const InputIt& last, Fn func)
            {
                auto current = first;
                const auto end_iterator = last;
                const auto last_null = (end_iterator - 1);
                while (current != end_iterator)
                {
                    // hand rolling ::std::find(current, end_iterator, L'\0');
                    // as this may be called when <algorithm> isn't available
                    auto next = current;
                    while (next != end_iterator && *next != L'\0')
                    {
                        ++next;
                    }

                    if (next != end_iterator)
                    {
                        // don't add an empty string for the final 2nd-null-terminator
                        if (next != last_null)
                        {
                            // call the function provided with the [begin, end] pair referencing a string found
                            func(current, next);
                        }
                        current = next + 1;
                    }
                    else
                    {
                        current = next;
                    }
                }
            }

#if defined(_VECTOR_) && defined(_STRING_) && defined(WIL_ENABLE_EXCEPTIONS)
            /**
             * \brief A translation function taking iterators referencing std::wstring objects and returns a corresponding std::vector<wchar_t> to be written to a MULTI_SZ registry value
             *        The translation follows the rules for how MULTI_SZ registry values should be formatted, notably how null characters should be embedded within the returned vector
             * \tparam InputIt An iterator type that references a container that holds std::wstring objects to translate into a wchar_t buffer
             * \param first An iterator referencing to the beginning of the target container (like a std::begin iterator)
             * \param last An iterator referencing one-past-the-end of the target container (like a std::end iterator)
             * \return A std::vector<wchar_t> with the raw wchar_t buffer of bytes prepared to write to a MULTI_SZ registry value
             */
            template <class InputIt>
            ::std::vector<wchar_t> get_multistring_from_wstrings(const InputIt& first, const InputIt& last)
            {
                ::std::vector<wchar_t> multistring;

                if (first == last)
                {
                    multistring.push_back(L'\0');
                    multistring.push_back(L'\0');
                    return multistring;
                }

                for (const auto& wstr : ::wil::make_range(first, last))
                {
                    multistring.insert(multistring.end(), ::std::begin(wstr), ::std::end(wstr));
                    multistring.push_back(L'\0');
                }

                // double-null-terminate the last string
                multistring.push_back(L'\0');
                return multistring;
            }

            /**
             * \brief A translation function taking iterators referencing wchar_t characters and returns extracted individual std::wstring objects
             *        The translation follows the rules for how MULTI_SZ registry value can be formatted, notably with embedded null characters
             *        Note that this conversion avoids returning empty std::wstring objects even though the input may contain contiguous null wchar_t values
             * \tparam InputIt An iterator type that reference a container that holds wchar_t characters to translate into individual strings
             * \param first An iterator referencing to the beginning of the target container (like a std::begin iterator)
             * \param last An iterator referencing one-past-the-end of the target container (like a std::end iterator)
             * \return A std::vector<std::wstring> of the extracted strings from the input container of wchar_t characters
             */
            template <class InputIt>
            ::std::vector<::std::wstring> get_wstring_vector_from_multistring(const InputIt& first, const InputIt& last)
            {
                if (last - first < 3)
                {
                    // it doesn't have the required 2 terminating null characters - return an empty string
                    return ::std::vector<::std::wstring>(1);
                }

                ::std::vector<::std::wstring> strings;
                walk_multistring(first, last, [&](const InputIt& string_first, const InputIt& string_last)
                    {
                        strings.emplace_back(string_first, string_last);
                    }
                );
                return strings;
            }
#endif // #if defined(_VECTOR_) && defined(_STRING_) && defined(WIL_ENABLE_EXCEPTIONS)

#if defined(__WIL_OBJBASE_H_)
            template <size_t C>
            void get_multistring_bytearray_from_strings_nothrow(const PCWSTR data[C], ::wil::unique_cotaskmem_array_ptr<BYTE>& multistring) WI_NOEXCEPT
            {
                constexpr uint8_t nullTermination[2]{ 0x00, 0x00 };

                size_t total_array_length_bytes = 0;
                for (size_t i = 0; i < C; ++i)
                {
                    total_array_length_bytes += wcslen(data[i]) * sizeof(wchar_t);
                    total_array_length_bytes += sizeof(wchar_t); // plus one for the null-terminator
                }
                total_array_length_bytes += sizeof(wchar_t); // plus one for the ending double-null-terminator

                *multistring.addressof() = static_cast<uint8_t*>(::CoTaskMemAlloc(total_array_length_bytes));
                if (!multistring.get())
                {
                    multistring.reset();
                    return;
                }
                *multistring.size_address() = total_array_length_bytes;

                size_t array_offset = 0;
                for (size_t i = 0; i < C; ++i)
                {
                    const auto string_length_bytes = wcslen(data[i]) * sizeof(wchar_t);
                    memcpy(multistring.get() + array_offset, data[i], string_length_bytes);
                    array_offset += string_length_bytes;

                    static_assert(sizeof(nullTermination) == sizeof(wchar_t), "null terminator must be a wchar");
                    memcpy(multistring.get() + array_offset, nullTermination, sizeof(nullTermination));
                    array_offset += sizeof(nullTermination);
                }

                // double-null-terminate the last string
                memcpy(multistring.get() + array_offset, nullTermination, sizeof(nullTermination));
            }

            /**
             * \brief A translation function taking iterators referencing wchar_t characters and returns extracted individual wil::unique_cotaskmem_string objects
             *        The translation follows the rules for how MULTI_SZ registry value can be formatted, notably with embedded null characters
             *        Note that this conversion avoids returning empty wil::unique_cotaskmem_string objects even though the input may contain contiguous null wchar_t values
             * \tparam InputIt An iterator type that reference a container that holds wchar_t characters to translate into individual strings
             * \param first An iterator referencing to the beginning of the target container (like a std::begin iterator)
             * \param last An iterator referencing one-past-the-end of the target container (like a std::end iterator)
             * \param cotaskmem_array The [out] wil::unique_cotaskmem_array_ptr<wil::unique_cotaskmem_string> to contain the array of strings
             * \return A wil::unique_cotaskmem_array_ptr<wil::unique_cotaskmem_string> of the extracted strings from the input container of wchar_t characters
             *         An empty wil::unique_cotaskmem_array_ptr should be translated as out-of-memory as there should always be at least one wil::unique_cotaskmem_string
             */
            template <class InputIt>
            void get_cotaskmemstring_array_from_multistring_nothrow(const InputIt& first, const InputIt& last, ::wil::unique_cotaskmem_array_ptr<::wil::unique_cotaskmem_string>& cotaskmem_array) WI_NOEXCEPT
            {
                if (last - first < 3)
                {
                    // it doesn't have the required 2 terminating null characters - return an empty string
                    *cotaskmem_array.addressof() = static_cast<PWSTR*>(::CoTaskMemAlloc(sizeof(PWSTR) * 1));
                    if (cotaskmem_array)
                    {
                        auto new_string = ::wil::make_cotaskmem_string_nothrow(L"");
                        if (new_string)
                        {
                            *cotaskmem_array.size_address() = 1;
                            cotaskmem_array[0] = new_string.release();
                        }
                        else
                        {
                            // oom will return an empty array
                            cotaskmem_array.reset();
                        }
                    }
                    else
                    {
                        // oom will return an empty array
                        cotaskmem_array.reset();
                    }
                    return;
                }

                // we must first count the # of strings for the array
                size_t arraySize = 0;
                walk_multistring(first, last, [&](const InputIt&, const InputIt&)
                    {
                        ++arraySize;
                    }
                );

                // allocate the array size necessary to hold all the unique_cotaskmem_strings
                *cotaskmem_array.addressof() = static_cast<PWSTR*>(::CoTaskMemAlloc(sizeof(PWSTR) * arraySize));
                if (!cotaskmem_array)
                {
                    // oom will return an empty array
                    cotaskmem_array.reset();
                    return;
                }

                *cotaskmem_array.size_address() = arraySize;
                ZeroMemory(cotaskmem_array.data(), sizeof(PWSTR) * arraySize);

                size_t arrayOffset = 0;
                walk_multistring(first, last, [&](const InputIt& string_first, const InputIt& string_last)
                    {
                        FAIL_FAST_IF(arrayOffset >= arraySize);
                        const auto stringSize = string_last - string_first;
                        auto new_string = ::wil::make_cotaskmem_string_nothrow(&(*string_first), stringSize);
                        if (!new_string)
                        {
                            // oom will return an empty array
                            cotaskmem_array.reset();
                            return;
                        }
                        cotaskmem_array[arrayOffset] = new_string.release();
                        ++arrayOffset;
                    }
                );
            }
#endif // #if defined(__WIL_OBJBASE_H_)

            namespace reg_value_type_info
            {
                // supports_prepare_buffer is used to determine if the input buffer to read a registry value should be prepared
                // before the first call to the registry read API
                template <typename T>
                constexpr bool supports_prepare_buffer() WI_NOEXCEPT
                {
                    return false;
                }
                template <typename T>
                HRESULT prepare_buffer(T&) WI_NOEXCEPT
                {
                    // no-op in the default case
                    return S_OK;
                }

                // supports_resize_buffer is used to determine if the input buffer to read a registry value can be resized
                // for those cases if the error from the registry read API indicates it needs a larger buffer
                template <typename T>
                constexpr bool supports_resize_buffer() WI_NOEXCEPT
                {
                    return false;
                }
                template <typename T>
                constexpr HRESULT resize_buffer(T&, DWORD) WI_NOEXCEPT
                {
                    return E_NOTIMPL;
                }

                // supports_trim_buffer is used to determine if the input buffer to read a registry value must be trimmed
                // after the registry read API has successfully written into the supplied buffer
                // note that currently only std::wstring requires this as it cannot have embedded nulls
                template <typename T>
                constexpr bool supports_trim_buffer() WI_NOEXCEPT
                {
                    return false;
                }
                template <typename T>
                constexpr void trim_buffer(T&) WI_NOEXCEPT
                {
                }

                constexpr void* get_buffer(const int32_t& value) WI_NOEXCEPT
                {
                    return const_cast<int32_t*>(&value);
                }

                constexpr DWORD get_buffer_size_bytes(int32_t) WI_NOEXCEPT
                {
                    return static_cast<DWORD>(sizeof(int32_t));
                }

                constexpr void* get_buffer(const uint32_t& value) WI_NOEXCEPT
                {
                    return const_cast<uint32_t*>(&value);
                }

                constexpr DWORD get_buffer_size_bytes(uint32_t) WI_NOEXCEPT
                {
                    return static_cast<DWORD>(sizeof(uint32_t));
                }

                constexpr void* get_buffer(const long& value) WI_NOEXCEPT
                {
                    return const_cast<long*>(&value);
                }

                constexpr DWORD get_buffer_size_bytes(long) WI_NOEXCEPT
                {
                    return static_cast<DWORD>(sizeof(long));
                }

                constexpr void* get_buffer(const unsigned long& value) WI_NOEXCEPT
                {
                    return const_cast<unsigned long*>(&value);
                }

                constexpr DWORD get_buffer_size_bytes(unsigned long) WI_NOEXCEPT
                {
                    return static_cast<DWORD>(sizeof(unsigned long));
                }

                constexpr void* get_buffer(const int64_t& value) WI_NOEXCEPT
                {
                    return const_cast<int64_t*>(&value);
                }

                constexpr DWORD get_buffer_size_bytes(int64_t) WI_NOEXCEPT
                {
                    return static_cast<DWORD>(sizeof(int64_t));
                }

                constexpr void* get_buffer(const uint64_t& value) WI_NOEXCEPT
                {
                    return const_cast<uint64_t*>(&value);
                }

                constexpr DWORD get_buffer_size_bytes(uint64_t) WI_NOEXCEPT
                {
                    return static_cast<DWORD>(sizeof(uint64_t));
                }

                constexpr void* get_buffer(PCWSTR value) WI_NOEXCEPT
                {
                    return const_cast<wchar_t*>(value);
                }

                inline DWORD get_buffer_size_bytes(PCWSTR value) WI_NOEXCEPT
                {
                    if (!value)
                    {
                        return 0;
                    }
                    // including the last null buffer space in the returned buffer-size-bytes
                    // as the registry API we call guarantees null termination
                    return static_cast<DWORD>((::wcslen(value) + 1) * sizeof(wchar_t));
                }

#if defined(_VECTOR_) && defined(WIL_ENABLE_EXCEPTIONS)
                inline void* get_buffer(const ::std::vector<uint8_t>& buffer) WI_NOEXCEPT
                {
                    return const_cast<uint8_t*>(buffer.data());
                }

                inline DWORD get_buffer_size_bytes(const ::std::vector<uint8_t>& value) WI_NOEXCEPT
                {
                    return static_cast<DWORD>(value.size());
                }

                template <>
                constexpr bool supports_prepare_buffer<::std::vector<uint8_t>>() WI_NOEXCEPT
                {
                    return true;
                }
                inline HRESULT prepare_buffer(::std::vector<uint8_t>& value) WI_NOEXCEPT try
                {
                    // resize the initial vector to at least 1 byte
                    // this is needed so we can detect when the registry value exists
                    // but the value has zero-bytes
                    if (value.empty())
                    {
                        value.resize(1);
                    }
                    // zero out the buffer if pre-allocated
                    for (auto& string_char : value)
                    {
                        string_char = 0x00;
                    }
                    return S_OK;
                }
                CATCH_RETURN();

                template <>
                constexpr bool supports_resize_buffer<::std::vector<uint8_t>>() WI_NOEXCEPT
                {
                    return true;
                }
                inline HRESULT resize_buffer(::std::vector<uint8_t>& buffer, DWORD byteSize) WI_NOEXCEPT try
                {
                    buffer.resize(byteSize);
                    return S_OK;
                }
                CATCH_RETURN();

                // std::vector<wchar_t> does not implement resize_buffer
                // because these support functions are only needed for set_value
                // from the return of get_multistring_from_wstrings
                inline void* get_buffer(const ::std::vector<wchar_t>& value) WI_NOEXCEPT
                {
                    return const_cast<wchar_t*>(value.data());
                }

                inline DWORD get_buffer_size_bytes(const ::std::vector<wchar_t>& value) WI_NOEXCEPT
                {
                    return static_cast<DWORD>(value.size()) * sizeof(wchar_t);
                }

                template <>
                constexpr bool supports_prepare_buffer<::std::vector<wchar_t>>() WI_NOEXCEPT
                {
                    return true;
                }
                inline HRESULT prepare_buffer(::std::vector<wchar_t>& value) WI_NOEXCEPT
                {
                    // zero out the buffer if pre-allocated
                    for (auto& string_char : value)
                    {
                        string_char = L'\0';
                    }
                    return S_OK;
                }
#endif // #if defined(_VECTOR_) && defined(WIL_ENABLE_EXCEPTIONS)

#if defined(_STRING_) && defined(WIL_ENABLE_EXCEPTIONS)
                inline void* get_buffer(const ::std::wstring& string) WI_NOEXCEPT
                {
                    return const_cast<wchar_t*>(string.data());
                }

                inline DWORD get_buffer_size_bytes(const ::std::wstring& string) WI_NOEXCEPT
                {
                    // including the last null buffer space in the returned buffer-size-bytes
                    // as the registry API we call guarantees null termination
                    return static_cast<DWORD>((string.size() + 1) * sizeof(wchar_t));
                }

                template <>
                constexpr bool supports_prepare_buffer<::std::wstring>() WI_NOEXCEPT
                {
                    return true;
                }
                inline HRESULT prepare_buffer(::std::wstring& string) WI_NOEXCEPT
                {
                    // zero out the buffer if pre-allocated
                    for (auto& string_char : string)
                    {
                        string_char = L'\0';
                    }
                    return S_OK;
                }

                template <>
                constexpr bool supports_resize_buffer<::std::wstring>() WI_NOEXCEPT
                {
                    return true;
                }
                inline HRESULT resize_buffer(::std::wstring& string, DWORD byteSize) WI_NOEXCEPT try
                {
                    string.resize(byteSize / sizeof(wchar_t));
                    return S_OK;
                }
                CATCH_RETURN();

                template <>
                constexpr bool supports_trim_buffer<::std::wstring>() WI_NOEXCEPT
                {
                    return true;
                }
                inline void trim_buffer(::std::wstring& buffer) WI_NOEXCEPT
                {
                    // remove any embedded null characters
                    const auto offset = buffer.find_first_of(L'\0');
                    if (offset != ::std::wstring::npos)
                    {
                        buffer.resize(offset);
                    }
                }
#endif // #if defined(_STRING_) && defined(WIL_ENABLE_EXCEPTIONS)

#if defined(__WIL_OLEAUTO_H_)
                inline void* get_buffer(const BSTR& value) WI_NOEXCEPT
                {
                    return value;
                }

                inline DWORD get_buffer_size_bytes(const BSTR& value) WI_NOEXCEPT
                {
                    auto length = ::SysStringLen(value);
                    if (length > 0)
                    {
                        // SysStringLen does not count the null-terminator
                        // including the last null buffer space in the returned buffer-size-bytes
                        // as the registry API we call guarantees null termination
                        length += 1;
                    }
                    return length * sizeof(wchar_t);
                }

                template <>
                constexpr bool supports_prepare_buffer<BSTR>() WI_NOEXCEPT
                {
                    return true;
                }
                inline HRESULT prepare_buffer(BSTR& value) WI_NOEXCEPT
                {
                    if (value)
                    {
                        // zero out the buffer if pre-allocated
                        for (auto& string_char : ::wil::make_range(value, get_buffer_size_bytes(value) / sizeof(WCHAR)))
                        {
                            string_char = L'\0';
                        }
                    }
                    return S_OK;
                }

                template <>
                constexpr bool supports_resize_buffer<BSTR>() WI_NOEXCEPT
                {
                    return true;
                }
                // transferringOwnership is only set to false if this is a 'shallow' copy of the BSTR
                // and the caller maintained ownership of the original BSTR.
                inline HRESULT resize_buffer(BSTR& string, DWORD byteSize, bool transferringOwnership = true) WI_NOEXCEPT
                {
                    // convert bytes to length (number of WCHARs)
                    DWORD length = byteSize / sizeof(WCHAR);
                    // SysAllocStringLen adds a null, so subtract a wchar_t from the input length
                    length = length > 0 ? length - 1 : length;
                    const BSTR new_bstr{ ::SysAllocStringLen(string, length) };
                    RETURN_IF_NULL_ALLOC(new_bstr);

                    // if not transferring ownership, the caller will still own the original BSTR
                    if (transferringOwnership)
                    {
                        ::SysFreeString(string);
                    }

                    string = new_bstr;
                    return S_OK;
                }

                inline void* get_buffer(const ::wil::unique_bstr& value) WI_NOEXCEPT
                {
                    return value.get();
                }

                inline DWORD get_buffer_size_bytes(const ::wil::unique_bstr& value) WI_NOEXCEPT
                {
                    return get_buffer_size_bytes(value.get());
                }

                template <>
                constexpr bool supports_prepare_buffer<::wil::unique_bstr>() WI_NOEXCEPT
                {
                    return true;
                }
                inline HRESULT prepare_buffer(::wil::unique_bstr& value) WI_NOEXCEPT
                {
                    if (value)
                    {
                        // zero out the buffer if pre-allocated
                        for (auto& string_char : ::wil::make_range(value.get(), get_buffer_size_bytes(value) / sizeof(WCHAR)))
                        {
                            string_char = L'\0';
                        }
                    }
                    return S_OK;
                }

                template<>
                constexpr bool supports_resize_buffer<::wil::unique_bstr>() WI_NOEXCEPT
                {
                    return true;
                }
                inline HRESULT resize_buffer(::wil::unique_bstr& string, DWORD byteSize) WI_NOEXCEPT
                {
                    BSTR temp_bstr = string.get();

                    // not transferring ownership of the BSTR within 'string' to resize_buffer()
                    // resize_buffer() will overwrite temp_bstr with a newly-allocated BSTR
                    constexpr bool transferringOwnership = false;
                    RETURN_IF_FAILED(resize_buffer(temp_bstr, byteSize, transferringOwnership));

                    // if succeeded in creating a new BSTR, move ownership of the new BSTR into string
                    string.reset(temp_bstr);
                    return S_OK;
                }
#endif // #if defined(__WIL_OLEAUTO_H_)

#if defined(__WIL_OLEAUTO_H_STL)
                inline void* get_buffer(const ::wil::shared_bstr& value) WI_NOEXCEPT
                {
                    return value.get();
                }

                inline DWORD get_buffer_size_bytes(const ::wil::shared_bstr& value) WI_NOEXCEPT
                {
                    return get_buffer_size_bytes(value.get());
                }

                template <>
                constexpr bool supports_prepare_buffer<::wil::shared_bstr>() WI_NOEXCEPT
                {
                    return true;
                }
                inline HRESULT prepare_buffer(::wil::shared_bstr& value) WI_NOEXCEPT
                {
                    if (value)
                    {
                        // zero out the buffer if pre-allocated
                        for (auto& string_char : ::wil::make_range(value.get(), get_buffer_size_bytes(value) / sizeof(WCHAR)))
                        {
                            string_char = L'\0';
                        }
                    }
                    return S_OK;
                }

                template<>
                constexpr bool supports_resize_buffer<::wil::shared_bstr>() WI_NOEXCEPT
                {
                    return true;
                }
                inline HRESULT resize_buffer(::wil::shared_bstr& string, DWORD byteSize) WI_NOEXCEPT
                {
                    BSTR temp_bstr = string.get();

                    // not transferring ownership of the BSTR within 'string' to resize_buffer()
                    // resize_buffer() will overwrite temp_bstr with a newly-allocated BSTR
                    constexpr bool transferringOwnership = false;
                    RETURN_IF_FAILED(resize_buffer(temp_bstr, byteSize, transferringOwnership));

                    // if succeeded in creating a new BSTR, move ownership of the new BSTR into string
                    string.reset(temp_bstr);
                    return S_OK;
                }
#endif // #if defined(__WIL_OLEAUTO_H_STL)

#if defined(__WIL_OBJBASE_H_)
                inline void* get_buffer(const ::wil::unique_cotaskmem_string& value) WI_NOEXCEPT
                {
                    return value.get();
                }

                constexpr DWORD get_buffer_size_bytes(const ::wil::unique_cotaskmem_string&) WI_NOEXCEPT
                {
                    // wil::unique_cotaskmem_string does not intrinsically track its internal buffer size
                    // thus the caller must track the buffer size it requested to be allocated
                    return 0;
                }

                template<>
                constexpr bool supports_resize_buffer<::wil::unique_cotaskmem_string>() WI_NOEXCEPT
                {
                    return true;
                }
                inline HRESULT resize_buffer(::wil::unique_cotaskmem_string& string, DWORD byteSize) WI_NOEXCEPT
                {
                    // convert bytes to length (number of WCHARs)
                    size_t length = byteSize / sizeof(wchar_t);
                    // ::wil::make_unique_string_nothrow adds one to the length when it allocates, so subtracting 1 from the input length
                    length = length > 0 ? length - 1 : length;
                    auto new_string = ::wil::make_unique_string_nothrow<::wil::unique_cotaskmem_string>(string.get(), length);
                    RETURN_IF_NULL_ALLOC(new_string.get());

                    string = ::wistd::move(new_string);
                    return S_OK;
                }

                inline void* get_buffer(const ::wil::unique_cotaskmem_array_ptr<uint8_t>& value) WI_NOEXCEPT
                {
                    return value.get();
                }

                inline DWORD get_buffer_size_bytes(const ::wil::unique_cotaskmem_array_ptr<uint8_t>& value) WI_NOEXCEPT
                {
                    return static_cast<DWORD>(value.size());
                }

                template<>
                constexpr bool supports_resize_buffer<::wil::unique_cotaskmem_array_ptr<uint8_t>>() WI_NOEXCEPT
                {
                    return true;
                }

                inline HRESULT resize_buffer(::wil::unique_cotaskmem_array_ptr<uint8_t>& arrayValue, DWORD byteSize) WI_NOEXCEPT
                {
                    ::wil::unique_cotaskmem_array_ptr<uint8_t> tempValue;
                    *tempValue.addressof() = static_cast<uint8_t*>(::CoTaskMemAlloc(byteSize));
                    RETURN_IF_NULL_ALLOC(tempValue.get());
                    *tempValue.size_address() = byteSize;

                    const auto bytesToCopy = arrayValue.size() < byteSize ? arrayValue.size() : byteSize;
                    CopyMemory(tempValue.get(), arrayValue.get(), bytesToCopy);

                    arrayValue = ::wistd::move(tempValue);
                    return S_OK;
                }
#endif // #if defined(__WIL_OBJBASE_H_)

#if defined(__WIL_OBJBASE_H_STL)
                inline void* get_buffer(const ::wil::shared_cotaskmem_string& value) WI_NOEXCEPT
                {
                    return value.get();
                }

                constexpr DWORD get_buffer_size_bytes(const ::wil::shared_cotaskmem_string&) WI_NOEXCEPT
                {
                    // wil::shared_cotaskmem_string does not intrinsically track its internal buffer size
                    // thus the caller must track the buffer size it requested to be allocated
                    return 0;
                }

                template<>
                constexpr bool supports_resize_buffer<::wil::shared_cotaskmem_string>() WI_NOEXCEPT
                {
                    return true;
                }
                inline HRESULT resize_buffer(::wil::shared_cotaskmem_string& string, DWORD byteSize) WI_NOEXCEPT try
                {
                    // convert bytes to length (number of WCHARs)
                    size_t length = byteSize / sizeof(wchar_t);
                    // ::wil::make_unique_string_nothrow adds one to the length when it allocates, so subtracting 1 from the input length
                    length = length > 0 ? length - 1 : length;
                    auto new_string = ::wil::make_unique_string_nothrow<::wil::unique_cotaskmem_string>(string.get(), length);
                    RETURN_IF_NULL_ALLOC(new_string.get());

                    string = ::wistd::move(new_string);
                    return S_OK;
                }
                CATCH_RETURN();
#endif // #if defined(__WIL_OBJBASE_H_STL)

                // constexpr expressions to determing the get* and set* registry value types
                // for all supported types T to read/write values
                template <typename T>
                DWORD get_value_type() WI_NOEXCEPT
                {
                    static_assert(sizeof(T) != sizeof(T), "Unsupported type for get_value_type");
                }

                template <typename T>
                DWORD set_value_type() WI_NOEXCEPT
                {
                    static_assert(sizeof(T) != sizeof(T), "Unsupported type for set_value_type");
                }

                template <>
                constexpr DWORD get_value_type<int32_t>() WI_NOEXCEPT
                {
                    return get_value_flags_from_value_type(REG_DWORD);
                }
                template <>
                constexpr DWORD set_value_type<int32_t>() WI_NOEXCEPT
                {
                    return REG_DWORD;
                }

                template <>
                constexpr DWORD get_value_type<uint32_t>() WI_NOEXCEPT
                {
                    return get_value_flags_from_value_type(REG_DWORD);
                }
                template <>
                constexpr DWORD set_value_type<uint32_t>() WI_NOEXCEPT
                {
                    return REG_DWORD;
                }

                template <>
                constexpr DWORD get_value_type<long>() WI_NOEXCEPT
                {
                    return get_value_flags_from_value_type(REG_DWORD);
                }
                template <>
                constexpr DWORD set_value_type<long>() WI_NOEXCEPT
                {
                    return REG_DWORD;
                }

                template <>
                constexpr DWORD get_value_type<unsigned long>() WI_NOEXCEPT
                {
                    return get_value_flags_from_value_type(REG_DWORD);
                }
                template <>
                constexpr DWORD set_value_type<unsigned long>() WI_NOEXCEPT
                {
                    return REG_DWORD;
                }

                template <>
                constexpr DWORD get_value_type<int64_t>() WI_NOEXCEPT
                {
                    return get_value_flags_from_value_type(REG_QWORD);
                }
                template <>
                constexpr DWORD set_value_type<int64_t>() WI_NOEXCEPT
                {
                    return REG_QWORD;
                }

                template <>
                constexpr DWORD get_value_type<uint64_t>() WI_NOEXCEPT
                {
                    return get_value_flags_from_value_type(REG_QWORD);
                }
                template <>
                constexpr DWORD set_value_type<uint64_t>() WI_NOEXCEPT
                {
                    return REG_QWORD;
                }

                template <>
                constexpr DWORD get_value_type<PCWSTR>() WI_NOEXCEPT
                {
                    return get_value_flags_from_value_type(REG_SZ);
                }
                template <>
                constexpr DWORD set_value_type<PCWSTR>() WI_NOEXCEPT
                {
                    return REG_SZ;
                }

#if defined(_STRING_) && defined(WIL_ENABLE_EXCEPTIONS)
                template <>
                constexpr DWORD get_value_type<::std::wstring>() WI_NOEXCEPT
                {
                    return get_value_flags_from_value_type(REG_SZ);
                }

                template <>
                constexpr DWORD set_value_type<const ::std::wstring>() WI_NOEXCEPT
                {
                    return REG_SZ;
                }
#endif // #if defined(_STRING_) && defined(WIL_ENABLE_EXCEPTIONS)

#if defined(__WIL_OLEAUTO_H_)
                template <>
                constexpr DWORD get_value_type<BSTR>() WI_NOEXCEPT
                {
                    return get_value_flags_from_value_type(REG_SZ);
                }
                template <>
                constexpr DWORD get_value_type<::wil::unique_bstr>() WI_NOEXCEPT
                {
                    return get_value_flags_from_value_type(REG_SZ);
                }

                template <>
                constexpr DWORD set_value_type<const BSTR>() WI_NOEXCEPT
                {
                    return REG_SZ;
                }

                template <>
                constexpr DWORD set_value_type<const ::wil::unique_bstr>() WI_NOEXCEPT
                {
                    return REG_SZ;
                }
#endif // #if defined(__WIL_OLEAUTO_H_)

#if defined(__WIL_OLEAUTO_H_STL)

                template <>
                constexpr DWORD get_value_type<::wil::shared_bstr>() WI_NOEXCEPT
                {
                    return get_value_flags_from_value_type(REG_SZ);
                }

                template <>
                constexpr DWORD set_value_type<const ::wil::shared_bstr>() WI_NOEXCEPT
                {
                    return REG_SZ;
                }
#endif // #if defined(__WIL_OLEAUTO_H_STL)

#if defined(__WIL_OBJBASE_H_)
                template <>
                constexpr DWORD get_value_type<::wil::unique_cotaskmem_string>() WI_NOEXCEPT
                {
                    return get_value_flags_from_value_type(REG_SZ);
                }

                template <>
                constexpr DWORD set_value_type<const ::wil::unique_cotaskmem_string>() WI_NOEXCEPT
                {
                    return REG_SZ;
                }
#endif // defined(__WIL_OBJBASE_H_)

#if defined(__WIL_OBJBASE_H_STL)
                template <>
                constexpr DWORD get_value_type<::wil::shared_cotaskmem_string>() WI_NOEXCEPT
                {
                    return get_value_flags_from_value_type(REG_SZ);
                }

                template <>
                constexpr DWORD set_value_type<const ::wil::shared_cotaskmem_string>() WI_NOEXCEPT
                {
                    return REG_SZ;
                }
#endif // #if defined(__WIL_OBJBASE_H_STL)
            }

            template <typename err_policy = ::wil::err_exception_policy>
            class reg_view_t
            {
            public:
                explicit reg_view_t(HKEY key) WI_NOEXCEPT : m_key(key)
                {
                }
                ~reg_view_t() = default;
                reg_view_t(const reg_view_t&) = delete;
                reg_view_t& operator=(const reg_view_t&) = delete;
                reg_view_t(reg_view_t&&) = delete;
                reg_view_t& operator=(reg_view_t&&) = delete;

                typename err_policy::result open_key(_In_opt_ _In_opt_ PCWSTR subKey, _Out_ HKEY* hkey, ::wil::reg::key_access access = ::wil::reg::key_access::read) const
                {
                    constexpr DWORD zero_options{ 0 };
                    return err_policy::HResult(HRESULT_FROM_WIN32(::RegOpenKeyExW(m_key, subKey, zero_options, get_access_flags(access), hkey)));
                }

                typename err_policy::result create_key(PCWSTR subKey, _Out_ HKEY* hkey, ::wil::reg::key_access access = ::wil::reg::key_access::read) const
                {
                    *hkey = nullptr;

                    constexpr DWORD zero_reserved{ 0 };
                    constexpr PWSTR null_class{ nullptr };
                    constexpr DWORD zero_options{ 0 };
                    constexpr SECURITY_ATTRIBUTES* null_security_attributes{ nullptr };
                    DWORD disposition{ 0 };
                    return err_policy::HResult(HRESULT_FROM_WIN32(
                        ::RegCreateKeyExW(m_key, subKey, zero_reserved, null_class, zero_options, get_access_flags(access), null_security_attributes, hkey, &disposition)));
                }

                typename err_policy::result delete_tree(_In_opt_ PCWSTR sub_key) const
                {
                    auto hr = HRESULT_FROM_WIN32(::RegDeleteTreeW(m_key, sub_key));
                    if (::wil::reg::is_registry_not_found(hr))
                    {
                        hr = S_OK;
                    }
                    return err_policy::HResult(hr);
                }

                typename err_policy::result delete_value(_In_opt_ PCWSTR value_name) const
                {
                    return err_policy::HResult(HRESULT_FROM_WIN32(::RegDeleteValueW(m_key, value_name)));
                }

                template <typename R>
                typename err_policy::result get_value(_In_opt_ PCWSTR subkey, _In_opt_ PCWSTR value_name, R& return_value, DWORD type = reg_value_type_info::get_value_type<R>()) const
                {
                    return get_value_with_type(subkey, value_name, return_value, type);
                }

                // typename D supports unsigned 32-bit values; i.e. allows the caller to pass a DWORD* as well as uint32_t*
                template <size_t Length, typename DwordType,
                    wistd::enable_if_t<wistd::is_same_v<DwordType, uint32_t> || wistd::is_same_v<DwordType, unsigned long>>* = nullptr>
                typename err_policy::result get_value_char_array(_In_opt_ PCWSTR subkey, _In_opt_ PCWSTR value_name, WCHAR(&return_value)[Length], DWORD type, _Out_opt_ DwordType * requiredBytes) const
                {
                    constexpr DwordType zero_value{ 0ul };
                    ::wil::assign_to_opt_param(requiredBytes, zero_value);
                    DWORD data_size_bytes{ Length * sizeof(WCHAR) };
                    const auto hr = HRESULT_FROM_WIN32(
                        ::RegGetValueW(m_key, subkey, value_name, ::wil::reg::reg_view_details::get_value_flags_from_value_type(type), nullptr, return_value, &data_size_bytes));
                    if (SUCCEEDED(hr) || ::wil::reg::is_registry_buffer_too_small(hr))
                    {
                        const DwordType updated_value{ data_size_bytes };
                        ::wil::assign_to_opt_param(requiredBytes, updated_value);
                    }
                    return err_policy::HResult(hr);
                }

#if defined (_OPTIONAL_) && defined(__cpp_lib_optional)
                // intended for err_exception_policy as err_returncode_policy will not get an error code
                template <typename R>
                ::std::optional<R> try_get_value(_In_opt_ PCWSTR subkey, _In_opt_ PCWSTR value_name, DWORD type = reg_value_type_info::get_value_type<R>()) const
                {
                    R value{};
                    const auto hr = get_value_with_type<R, ::wil::err_returncode_policy>(subkey, value_name, value, type);
                    if (SUCCEEDED(hr))
                    {
                        return ::std::optional(::wistd::move(value));
                    }

                    if (::wil::reg::is_registry_not_found(hr))
                    {
                        return ::std::nullopt;
                    }

                    // throw if exception policy
                    err_policy::HResult(hr);
                    return ::std::nullopt;
                }
#endif // #if defined (_OPTIONAL_) && defined(__cpp_lib_optional)

                template <typename R>
                typename err_policy::result set_value(_In_opt_ PCWSTR subkey, _In_opt_ PCWSTR value_name, const R& value, DWORD type = reg_value_type_info::set_value_type<R>()) const
                {
                    return set_value_with_type(subkey, value_name, value, type);
                }

            private:
                const HKEY m_key{};

                template <typename R>
                typename err_policy::result set_value_with_type(_In_opt_ PCWSTR subkey, _In_opt_ PCWSTR value_name, const R& value, DWORD type) const
                {
                    return err_policy::HResult(HRESULT_FROM_WIN32(
                        ::RegSetKeyValueW(
                            m_key,
                            subkey,
                            value_name,
                            type,
                            static_cast<uint8_t*>(reg_value_type_info::get_buffer(value)),
                            reg_value_type_info::get_buffer_size_bytes(value))));
                }

                template <typename R, typename get_value_with_type_policy = err_policy>
                typename get_value_with_type_policy::result get_value_with_type(_In_opt_ PCWSTR subkey, _In_opt_ PCWSTR value_name, R& return_value, DWORD type = reg_value_type_info::get_value_type<R>()) const
                {
                    if
#if defined(__cpp_if_constexpr)
                        constexpr
#endif
                        (reg_value_type_info::supports_prepare_buffer<R>())

                    {
                        const auto prepare_buffer_hr = reg_value_type_info::prepare_buffer(return_value);
                        if (FAILED(prepare_buffer_hr))
                        {
                            return get_value_with_type_policy::HResult(prepare_buffer_hr);
                        }
                    }

                    // get_buffer_size_bytes should include the null terminator when used for strings.
                    DWORD bytes_allocated{ reg_value_type_info::get_buffer_size_bytes(return_value) };
                    HRESULT get_value_hresult = S_OK;
                    for (;;)
                    {
                        constexpr DWORD* null_type{ nullptr };
                        DWORD data_size_bytes{ bytes_allocated };
                        get_value_hresult = HRESULT_FROM_WIN32(::RegGetValueW(
                            m_key,
                            subkey,
                            value_name,
                            get_value_flags_from_value_type(type),
                            null_type,
                            reg_value_type_info::get_buffer(return_value),
                            &data_size_bytes));

                        // some return types we can grow as needed - e.g. when writing to a std::wstring
                        // only compile and resize_buffer for those types that support dynamically growing the buffer
                        if
#if defined(__cpp_if_constexpr)
                            constexpr
#endif
                            (reg_value_type_info::supports_resize_buffer<R>())
                        {
                            // Attempt to grow the buffer with the data_size_bytes returned from GetRegValueW
                            // GetRegValueW will indicate the caller allocate the returned number of bytes in one of two cases:
                            // 1. returns ERROR_MORE_DATA
                            // 2. returns ERROR_SUCCESS when we gave it a nullptr for the out buffer
                            const bool shouldReallocate =
                                (::wil::reg::is_registry_buffer_too_small(get_value_hresult)) ||
                                (SUCCEEDED(get_value_hresult) && (reg_value_type_info::get_buffer(return_value) == nullptr) && (data_size_bytes > 0));
                            if (shouldReallocate)
                            {
                                // verify if resize_buffer succeeded allocation
                                const auto resize_buffer_hr = reg_value_type_info::resize_buffer(return_value, data_size_bytes);
                                if (FAILED(resize_buffer_hr))
                                {
                                    // if resize fails, return this error back to the caller
                                    return get_value_with_type_policy::HResult(resize_buffer_hr);
                                }

                                // if it resize succeeds, continue the for loop to try again
                                bytes_allocated = data_size_bytes;
                                continue;
                            }

                            // if the RegGetValueW call succeeded with a non-null [out] param,
                            // and the type supports resize_buffer
                            // and the bytes we allocated don't match data_size_bytes returned from RegGetValueW
                            // resize the buffer to match what RegGetValueW returned
                            if (SUCCEEDED(get_value_hresult))
                            {
                                const auto current_byte_size = reg_value_type_info::get_buffer_size_bytes(return_value);
                                if (current_byte_size != data_size_bytes)
                                {
                                    // verify if resize_buffer succeeded allocation
                                    const auto resize_buffer_hr = reg_value_type_info::resize_buffer(return_value, data_size_bytes);
                                    if (FAILED(resize_buffer_hr))
                                    {
                                        // if resize fails, return this error back to the caller
                                        return get_value_with_type_policy::HResult(resize_buffer_hr);
                                    }
                                }
                            }
                        }

                        // we don't need to reallocate and retry the call to RegGetValueW so breaking out of the loop
                        break;
                    }

                    // some types (generally string types) require trimming its internal buffer after RegGetValueW successfully wrote into its buffer
                    if
#if defined(__cpp_if_constexpr)
                        constexpr
#endif
                        (reg_value_type_info::supports_trim_buffer<R>())

                    {
                        if (SUCCEEDED(get_value_hresult))
                        {
                            reg_value_type_info::trim_buffer(return_value);
                        }
                    }

                    return get_value_with_type_policy::HResult(get_value_hresult);
                }
            };

            using reg_view_nothrow = ::wil::reg::reg_view_details::reg_view_t<::wil::err_returncode_policy>;
#if defined(WIL_ENABLE_EXCEPTIONS)
            using reg_view = ::wil::reg::reg_view_details::reg_view_t<::wil::err_exception_policy>;
#endif // #if defined(WIL_ENABLE_EXCEPTIONS)
        }

    } // namespace reg
} // namespace wil
#endif // __WIL_REGISTRY_HELPERS_INCLUDED