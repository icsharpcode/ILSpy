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
#ifndef __WIL_TOKEN_HELPERS_INCLUDED
#define __WIL_TOKEN_HELPERS_INCLUDED

#ifdef _KERNEL_MODE
#error This header is not supported in kernel-mode.
#endif

#include "resource.h"
#include <new>
#include <lmcons.h>         // for UNLEN and DNLEN
#include <processthreadsapi.h>

// for GetUserNameEx()
#ifndef SECURITY_WIN32
#define SECURITY_WIN32
#endif
#include <Security.h>

namespace wil
{
    /// @cond
    namespace details
    {
        // Template specialization for TOKEN_INFORMATION_CLASS, add more mappings here as needed
        // TODO: The mapping should be reversed to be MapTokenInfoClassToStruct since there may
        // be an info class value that uses the same structure. That is the case for the file
        // system information.
        template<typename T> struct MapTokenStructToInfoClass;
        template<> struct MapTokenStructToInfoClass<TOKEN_ACCESS_INFORMATION> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenAccessInformation; static constexpr bool FixedSize = false; };
        template<> struct MapTokenStructToInfoClass<TOKEN_APPCONTAINER_INFORMATION> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenAppContainerSid; static constexpr bool FixedSize = false; };
        template<> struct MapTokenStructToInfoClass<TOKEN_DEFAULT_DACL> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenDefaultDacl; static constexpr bool FixedSize = false; };
        template<> struct MapTokenStructToInfoClass<TOKEN_GROUPS_AND_PRIVILEGES> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenGroupsAndPrivileges; static constexpr bool FixedSize = false; };
        template<> struct MapTokenStructToInfoClass<TOKEN_MANDATORY_LABEL> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenIntegrityLevel; static constexpr bool FixedSize = false; };
        template<> struct MapTokenStructToInfoClass<TOKEN_OWNER> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenOwner; static constexpr bool FixedSize = false;  };
        template<> struct MapTokenStructToInfoClass<TOKEN_PRIMARY_GROUP> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenPrimaryGroup; static constexpr bool FixedSize = false;  };
        template<> struct MapTokenStructToInfoClass<TOKEN_PRIVILEGES> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenPrivileges; static constexpr bool FixedSize = false;  };
        template<> struct MapTokenStructToInfoClass<TOKEN_USER> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenUser; static constexpr bool FixedSize = false;  };

        // fixed size cases
        template<> struct MapTokenStructToInfoClass<TOKEN_ELEVATION_TYPE> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenElevationType; static constexpr bool FixedSize = true; };
        template<> struct MapTokenStructToInfoClass<TOKEN_MANDATORY_POLICY> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenMandatoryPolicy; static constexpr bool FixedSize = true; };
        template<> struct MapTokenStructToInfoClass<TOKEN_ORIGIN> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenOrigin; static constexpr bool FixedSize = true; };
        template<> struct MapTokenStructToInfoClass<TOKEN_SOURCE> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenSource; static constexpr bool FixedSize = true; };
        template<> struct MapTokenStructToInfoClass<TOKEN_STATISTICS> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenStatistics; static constexpr bool FixedSize = true; };
        template<> struct MapTokenStructToInfoClass<TOKEN_TYPE> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenType; static constexpr bool FixedSize = true; };
        template<> struct MapTokenStructToInfoClass<SECURITY_IMPERSONATION_LEVEL> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenImpersonationLevel;  static constexpr bool FixedSize = true; };
        template<> struct MapTokenStructToInfoClass<TOKEN_ELEVATION> { static constexpr TOKEN_INFORMATION_CLASS infoClass = TokenElevation; static constexpr bool FixedSize = true; };
    
        struct token_info_deleter
        {
            template<typename T> void operator()(T* p) const
            {
                static_assert(wistd::is_trivially_destructible_v<T>, "do not use with nontrivial types");
                ::operator delete(p);
            }
        };
    }
    /// @endcond

    enum class OpenThreadTokenAs
    {
        Current,
        Self
    };

    /** Open the active token.
    Opens either the current thread token (if impersonating) or the current process token. Returns a token the caller
    can use with methods like get_token_information<> below. By default, the token is opened for TOKEN_QUERY and as the
    effective user.

    Consider using GetCurrentThreadEffectiveToken() instead of this method when eventually calling get_token_information.
    This method returns a real handle to the effective token, but GetCurrentThreadEffectiveToken() is a Pseudo-handle
    and much easier to manage.
    ~~~~
    wil::unique_handle theToken;
    RETURN_IF_FAILED(wil::open_current_access_token_nothrow(&theToken));
    ~~~~
    Callers who want more access to the token (such as to duplicate or modify the token) can pass
    any mask of the token rights.
    ~~~~
    wil::unique_handle theToken;
    RETURN_IF_FAILED(wil::open_current_access_token_nothrow(&theToken, TOKEN_QUERY | TOKEN_ADJUST_PRIVILEGES));
    ~~~~
    Services impersonating their clients may need to request that the active token is opened on the
    behalf of the service process to perform certain operations. Opening a token for impersonation access
    or privilege-adjustment are examples of uses.
    ~~~~
    wil::unique_handle callerToken;
    RETURN_IF_FAILED(wil::open_current_access_token_nothrow(&theToken, TOKEN_QUERY | TOKEN_IMPERSONATE, OpenThreadTokenAs::Self));
    ~~~~
    @param tokenHandle Receives the token opened during the operation. Must be CloseHandle'd by the caller, or
                (preferably) stored in a wil::unique_handle
    @param access Bits from the TOKEN_* access mask which are passed to OpenThreadToken/OpenProcessToken
    @param openAs Current to use current thread security context, or Self to use process security context.
    */
    inline HRESULT open_current_access_token_nothrow(_Out_ HANDLE* tokenHandle, unsigned long access = TOKEN_QUERY, OpenThreadTokenAs openAs = OpenThreadTokenAs::Current)
    {
        HRESULT hr = (OpenThreadToken(GetCurrentThread(), access, (openAs == OpenThreadTokenAs::Self), tokenHandle) ? S_OK : HRESULT_FROM_WIN32(::GetLastError()));
        if (hr == HRESULT_FROM_WIN32(ERROR_NO_TOKEN))
        {
            hr = (OpenProcessToken(GetCurrentProcess(), access, tokenHandle) ? S_OK : HRESULT_FROM_WIN32(::GetLastError()));
        }
        return hr;
    }

    //! Current thread or process token, consider using GetCurrentThreadEffectiveToken() instead.
    inline wil::unique_handle open_current_access_token_failfast(unsigned long access = TOKEN_QUERY, OpenThreadTokenAs openAs = OpenThreadTokenAs::Current)
    {
        HANDLE rawTokenHandle;
        FAIL_FAST_IF_FAILED(open_current_access_token_nothrow(&rawTokenHandle, access, openAs));
        return wil::unique_handle(rawTokenHandle);
    }

// Exception based function to open current thread/process access token and acquire pointer to it
#ifdef WIL_ENABLE_EXCEPTIONS
    //! Current thread or process token, consider using GetCurrentThreadEffectiveToken() instead.
    inline wil::unique_handle open_current_access_token(unsigned long access = TOKEN_QUERY, OpenThreadTokenAs openAs = OpenThreadTokenAs::Current)
    {
        HANDLE rawTokenHandle;
        THROW_IF_FAILED(open_current_access_token_nothrow(&rawTokenHandle, access, openAs));
        return wil::unique_handle(rawTokenHandle);
    }
#endif // WIL_ENABLE_EXCEPTIONS

#if (_WIN32_WINNT >= _WIN32_WINNT_WIN8)

    // Returns tokenHandle or the effective thread token if tokenHandle is null.
    // Note, this returns an token handle who's lifetime is managed independently
    // and it may be a pseudo token, don't free it!
    inline HANDLE GetCurrentThreadEffectiveTokenWithOverride(HANDLE tokenHandle)
    {
        return tokenHandle ? tokenHandle : GetCurrentThreadEffectiveToken();
    }

    /** Fetches information about a token.
    See GetTokenInformation on MSDN for what this method can return. For variable sized structs the information
    is returned to the caller as a wil::unique_tokeninfo_ptr<T> (like TOKEN_ORIGIN, TOKEN_USER, TOKEN_ELEVATION, etc.). For
    fixed sized, the struct is returned directly.
    The caller must have access to read the information from the provided token. This method works with both real
    (e.g. OpenCurrentAccessToken) and pseudo (e.g. GetCurrentThreadToken) token handles.
    ~~~~
    // Retrieve the TOKEN_USER structure for the current process
    wil::unique_tokeninfo_ptr<TOKEN_USER> user;
    RETURN_IF_FAILED(wil::get_token_information_nothrow(user, GetCurrentProcessToken()));
    RETURN_IF_FAILED(ConsumeSid(user->User.Sid));
    ~~~~
    Not specifying the token handle is the same as specifying 'nullptr' and retrieves information about the effective token.
    ~~~~
    wil::unique_tokeninfo_ptr<TOKEN_PRIVILEGES> privileges;
    RETURN_IF_FAILED(wil::get_token_information_nothrow(privileges));
    for (auto const& privilege : wil::GetRange(privileges->Privileges, privileges->PrivilegeCount))
    {
        RETURN_IF_FAILED(ConsumePrivilege(privilege));
    }
    ~~~~
    @param tokenInfo Receives a pointer to a structure containing the results of GetTokenInformation for the requested
            type. The type of <T> selects which TOKEN_INFORMATION_CLASS will be used.
    @param tokenHandle Specifies which token will be queried. When nullptr, the thread's effective current token is used.
    @return S_OK on success, a FAILED hresult containing the win32 error from querying the token otherwise.
    */

    template <typename Q> using unique_tokeninfo_ptr = wistd::unique_ptr<Q, details::token_info_deleter>;

    template <typename T, wistd::enable_if_t<!details::MapTokenStructToInfoClass<T>::FixedSize>* = nullptr>
    inline HRESULT get_token_information_nothrow(unique_tokeninfo_ptr<T>& tokenInfo, HANDLE tokenHandle = nullptr)
    {
        tokenInfo.reset();
        tokenHandle = GetCurrentThreadEffectiveTokenWithOverride(tokenHandle);

        DWORD tokenInfoSize = 0;
        const auto infoClass = details::MapTokenStructToInfoClass<T>::infoClass;
        RETURN_LAST_ERROR_IF(!((!GetTokenInformation(tokenHandle, infoClass, nullptr, 0, &tokenInfoSize)) &&
            (::GetLastError() == ERROR_INSUFFICIENT_BUFFER)));
        unique_tokeninfo_ptr<T> tokenInfoClose{ static_cast<T*>(::operator new(tokenInfoSize, std::nothrow)) };
        RETURN_IF_NULL_ALLOC(tokenInfoClose);
        RETURN_IF_WIN32_BOOL_FALSE(GetTokenInformation(tokenHandle, infoClass, tokenInfoClose.get(), tokenInfoSize, &tokenInfoSize));
        tokenInfo = wistd::move(tokenInfoClose);

        return S_OK;
    }

    template <typename T, wistd::enable_if_t<details::MapTokenStructToInfoClass<T>::FixedSize>* = nullptr>
    inline HRESULT get_token_information_nothrow(_Out_ T* tokenInfo, HANDLE tokenHandle = nullptr)
    {
        *tokenInfo = {};
        tokenHandle = GetCurrentThreadEffectiveTokenWithOverride(tokenHandle);

        DWORD tokenInfoSize = sizeof(T);
        const auto infoClass = details::MapTokenStructToInfoClass<T>::infoClass;
        RETURN_IF_WIN32_BOOL_FALSE(GetTokenInformation(tokenHandle, infoClass, tokenInfo, tokenInfoSize, &tokenInfoSize));

        return S_OK;
    }

    namespace details
    {
        template<typename T, typename policy, wistd::enable_if_t<!details::MapTokenStructToInfoClass<T>::FixedSize>* = nullptr>
        unique_tokeninfo_ptr<T> GetTokenInfoWrap(HANDLE token = nullptr)
        {
            unique_tokeninfo_ptr<T> temp;
            policy::HResult(get_token_information_nothrow(temp, token));
            return temp;
        }

        template<typename T, typename policy, wistd::enable_if_t<details::MapTokenStructToInfoClass<T>::FixedSize>* = nullptr>
        T GetTokenInfoWrap(HANDLE token = nullptr)
        {
            T temp{};
            policy::HResult(get_token_information_nothrow(&temp, token));
            return temp;
        }
    }

    //! A variant of get_token_information<T> that fails-fast on errors retrieving the token
    template <typename T>
    inline auto get_token_information_failfast(HANDLE token = nullptr)
    {
        return details::GetTokenInfoWrap<T, err_failfast_policy>(token);
    }

    //! Overload of GetTokenInformationNoThrow that retrieves a token linked from the provided token
    inline HRESULT get_token_information_nothrow(unique_token_linked_token& tokenInfo, HANDLE tokenHandle = nullptr)
    {
        static_assert(sizeof(tokenInfo) == sizeof(TOKEN_LINKED_TOKEN), "confusing size mismatch");
        tokenHandle = GetCurrentThreadEffectiveTokenWithOverride(tokenHandle);

        DWORD tokenInfoSize = 0;
        RETURN_IF_WIN32_BOOL_FALSE(::GetTokenInformation(tokenHandle, TokenLinkedToken,
            tokenInfo.reset_and_addressof(), sizeof(tokenInfo), &tokenInfoSize));
        return S_OK;
    }

    /** Retrieves the linked-token information for a token.
    Fails-fast if the link information cannot be retrieved.
    ~~~~
    auto link = get_linked_token_information_failfast(GetCurrentThreadToken());
    auto tokenUser = get_token_information<TOKEN_USER>(link.LinkedToken);
    ~~~~
    @param token Specifies the token to query. Pass nullptr to use the current effective thread token
    @return unique_token_linked_token containing a handle to the linked token
    */
    inline unique_token_linked_token get_linked_token_information_failfast(HANDLE token = nullptr)
    {
        unique_token_linked_token tokenInfo;
        FAIL_FAST_IF_FAILED(get_token_information_nothrow(tokenInfo, token));
        return tokenInfo;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    /** Fetches information about a token.
    See get_token_information_nothrow for full details.
    ~~~~
    auto user = wil::get_token_information<TOKEN_USER>(GetCurrentProcessToken());
    ConsumeSid(user->User.Sid);
    ~~~~
    Pass 'nullptr' (or omit the parameter) as tokenHandle to retrieve information about the effective token.
    ~~~~
    auto privs = wil::get_token_information<TOKEN_PRIVILEGES>(privileges);
    for (auto& priv : wil::make_range(privs->Privileges, privs->Privilieges + privs->PrivilegeCount))
    {
        if (priv.Attributes & SE_PRIVILEGE_ENABLED)
        {
            // ...
        }
    }
    ~~~~
    @return A pointer to a structure containing the results of GetTokenInformation for the requested  type. The type of
                <T> selects which TOKEN_INFORMATION_CLASS will be used.
    @param token Specifies which token will be queried. When nullptr or not set, the thread's effective current token is used.
    */
    template <typename T>
    inline auto get_token_information(HANDLE token = nullptr)
    {
        return details::GetTokenInfoWrap<T, err_exception_policy>(token);
    }

    /** Retrieves the linked-token information for a token.
    Throws an exception if the link information cannot be retrieved.
    ~~~~
    auto link = get_linked_token_information(GetCurrentThreadToken());
    auto tokenUser = get_token_information<TOKEN_USER>(link.LinkedToken);
    ~~~~
    @param token Specifies the token to query. Pass nullptr to use the current effective thread token
    @return unique_token_linked_token containing a handle to the linked token
    */
    inline unique_token_linked_token get_linked_token_information(HANDLE token = nullptr)
    {
        unique_token_linked_token tokenInfo;
        THROW_IF_FAILED(get_token_information_nothrow(tokenInfo, token));
        return tokenInfo;
    }
#endif
#endif // _WIN32_WINNT >= _WIN32_WINNT_WIN8

    /// @cond
    namespace details
    {
        inline void RevertImpersonateToken(_In_ _Post_ptr_invalid_ HANDLE oldToken)
        {
            FAIL_FAST_IMMEDIATE_IF(!::SetThreadToken(nullptr, oldToken));

            if (oldToken)
            {
                ::CloseHandle(oldToken);
            }
        }
    }
    /// @endcond

    using unique_token_reverter = wil::unique_any<
        HANDLE,
        decltype(&details::RevertImpersonateToken),
        details::RevertImpersonateToken,
        details::pointer_access_none,
        HANDLE,
        INT_PTR,
        -1,
        HANDLE>;

    /** Temporarily impersonates a token on this thread.
    This method sets a new token on a thread, restoring the current token when the returned object
    is destroyed. Useful for impersonating other tokens or running as 'self,' especially in services.
    ~~~~
    HRESULT OpenFileAsSessionuser(PCWSTR filePath, DWORD session, _Out_ HANDLE* opened)
    {
        wil::unique_handle userToken;
        RETURN_IF_WIN32_BOOL_FALSE(QueryUserToken(session, &userToken));

        wil::unique_token_reverter reverter;
        RETURN_IF_FAILED(wil::impersonate_token_nothrow(userToken.get(), reverter));

        wil::unique_hfile userFile(::CreateFile(filePath, ...));
        RETURN_LAST_ERROR_IF(!userFile && (::GetLastError() != ERROR_FILE_NOT_FOUND));

        *opened = userFile.release();
        return S_OK;
    }
    ~~~~
    @param token A token to impersonate, or 'nullptr' to run as the process identity.
    */
    inline HRESULT impersonate_token_nothrow(HANDLE token, unique_token_reverter& reverter)
    {
        wil::unique_handle currentToken;

        // Get the token for the current thread. If there wasn't one, the reset will clear it as well
        if (!OpenThreadToken(GetCurrentThread(), TOKEN_ALL_ACCESS, TRUE, &currentToken))
        {
            RETURN_LAST_ERROR_IF(::GetLastError() != ERROR_NO_TOKEN);
        }

        // Update the current token
        RETURN_IF_WIN32_BOOL_FALSE(::SetThreadToken(nullptr, token));

        reverter.reset(currentToken.release()); // Ownership passed
        return S_OK;
    }

    /** Temporarily clears any impersonation on this thread.
    This method resets the current thread's token to nullptr, indicating that it is not impersonating
    any user. Useful for elevating to whatever identity a service or higher-privilege process might
    be capable of running under.
    ~~~~
    HRESULT DeleteFileRetryAsSelf(PCWSTR filePath)
    {
        if (!::DeleteFile(filePath))
        {
            RETURN_LAST_ERROR_IF(::GetLastError() != ERROR_ACCESS_DENIED);
            wil::unique_token_reverter reverter;
            RETURN_IF_FAILED(wil::run_as_self_nothrow(reverter));
            RETURN_IF_FAILED(TakeOwnershipOfFile(filePath));
            RETURN_IF_FAILED(GrantDeleteAccess(filePath));
            RETURN_IF_WIN32_BOOL_FALSE(::DeleteFile(filePath));
        }
        return S_OK;
    }
    ~~~~
    */
    inline HRESULT run_as_self_nothrow(unique_token_reverter& reverter)
    {
        return impersonate_token_nothrow(nullptr, reverter);
    }

    inline unique_token_reverter impersonate_token_failfast(HANDLE token)
    {
        unique_token_reverter oldToken;
        FAIL_FAST_IF_FAILED(impersonate_token_nothrow(token, oldToken));
        return oldToken;
    }

    inline unique_token_reverter run_as_self_failfast()
    {
        return impersonate_token_failfast(nullptr);
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    /** Temporarily impersonates a token on this thread.
    This method sets a new token on a thread, restoring the current token when the returned object
    is destroyed. Useful for impersonating other tokens or running as 'self,' especially in services.
    ~~~~
    wil::unique_hfile OpenFileAsSessionuser(_In_z_ const wchar_t* filePath, DWORD session)
    {
        wil::unique_handle userToken;
        THROW_IF_WIN32_BOOL_FALSE(QueryUserToken(session, &userToken));

        auto priorToken = wil::impersonate_token(userToken.get());

        wil::unique_hfile userFile(::CreateFile(filePath, ...));
        THROW_LAST_ERROR_IF(::GetLastError() != ERROR_FILE_NOT_FOUND);

        return userFile;
    }
    ~~~~
    @param token A token to impersonate, or 'nullptr' to run as the process identity.
    */
    inline unique_token_reverter impersonate_token(HANDLE token = nullptr)
    {
        unique_token_reverter oldToken;
        THROW_IF_FAILED(impersonate_token_nothrow(token, oldToken));
        return oldToken;
    }

    /** Temporarily clears any impersonation on this thread.
    This method resets the current thread's token to nullptr, indicating that it is not impersonating
    any user. Useful for elevating to whatever identity a service or higher-privilege process might
    be capable of running under.
    ~~~~
    void DeleteFileRetryAsSelf(_In_z_ const wchar_t* filePath)
    {
        if (!::DeleteFile(filePath) && (::GetLastError() == ERROR_ACCESS_DENIED))
        {
            auto priorToken = wil::run_as_self();
            TakeOwnershipOfFile(filePath);
            GrantDeleteAccess(filePath);
            ::DeleteFile(filePath);
        }
    }
    ~~~~
    */
    inline unique_token_reverter run_as_self()
    {
        return impersonate_token(nullptr);
    }
#endif // WIL_ENABLE_EXCEPTIONS

    namespace details
    {
        template<size_t AuthorityCount> struct static_sid_t
        {
            BYTE Revision;
            BYTE SubAuthorityCount;
            SID_IDENTIFIER_AUTHORITY IdentifierAuthority;
            DWORD SubAuthority[AuthorityCount];

            PSID get()
            {
                return reinterpret_cast<PSID>(this);
            }

            template<size_t other> static_sid_t& operator=(const static_sid_t<other>& source)
            {
                static_assert(other <= AuthorityCount, "Cannot assign from a larger static sid to a smaller one");

                if (&this->Revision != &source.Revision)
                {
                    memcpy(this, &source, sizeof(source));
                }

                return *this;
            }
        };
    }

    /** Returns a structure containing a Revision 1 SID initialized with the authorities provided
    Replaces AllocateAndInitializeSid by constructing a structure laid out like a PSID, but
    returned like a value. The resulting object is suitable for use with any method taking PSID,
    passed by "&the_sid" or via "the_sid.get()"
    ~~~~
    // Change the owner of the key to administrators
    auto systemSid = wil::make_static_sid(SECURITY_NT_AUTHORITY, SECURITY_BUILTIN_DOMAIN_RID, DOMAIN_ALIAS_RID_ADMINS);
    RETURN_IF_WIN32_ERROR(SetNamedSecurityInfo(keyPath, SE_REGISTRY_KEY, OWNER_SECURITY_INFORMATION, &systemSid, nullptr, nullptr, nullptr));
    ~~~~
    */
    template<typename... Ts> constexpr auto make_static_sid(const SID_IDENTIFIER_AUTHORITY& authority, Ts&&... subAuthorities)
    {
        using sid_t = details::static_sid_t<sizeof...(subAuthorities)>;

        static_assert(sizeof...(subAuthorities) <= SID_MAX_SUB_AUTHORITIES, "too many sub authorities");
        static_assert(offsetof(sid_t, Revision) == offsetof(_SID, Revision), "layout mismatch");
        static_assert(offsetof(sid_t, SubAuthorityCount) == offsetof(_SID, SubAuthorityCount), "layout mismatch");
        static_assert(offsetof(sid_t, IdentifierAuthority) == offsetof(_SID, IdentifierAuthority), "layout mismatch");
        static_assert(offsetof(sid_t, SubAuthority) == offsetof(_SID, SubAuthority), "layout mismatch");

        return sid_t { SID_REVISION, sizeof...(subAuthorities), authority, { static_cast<DWORD>(subAuthorities)... } };
    }

    //! Variant of static_sid that defaults to the NT authority
    template<typename... Ts> constexpr auto make_static_nt_sid(Ts&& ... subAuthorities)
    {
        return make_static_sid(SECURITY_NT_AUTHORITY, wistd::forward<Ts>(subAuthorities)...);
    }

    /** Determines whether a specified security identifier (SID) is enabled in an access token.
    This function determines whether a security identifier, described by a given set of subauthorities, is enabled
    in the given access token. Note that only up to eight subauthorities can be passed to this function.
    ~~~~
    bool IsGuest()
    {
        return wil::test_token_membership(nullptr, SECURITY_NT_AUTHORITY, SECURITY_BUILTIN_DOMAIN_RID, DOMAIN_ALIAS_RID_GUESTS));
    }
    ~~~~
    @param result This will be set to true if and only if a security identifier described by the given set of subauthorities is enabled in the given access token.
    @param token A handle to an access token. The handle must have TOKEN_QUERY access to the token, and must be an impersonation token. If token is nullptr, test_token_membership
           uses the impersonation token of the calling thread. If the thread is not impersonating, the function duplicates the thread's primary token to create an impersonation token.
    @param sidAuthority A reference to a SID_IDENTIFIER_AUTHORITY structure. This structure provides the top-level identifier authority value to set in the SID.
    @param subAuthorities Up to 15 subauthority values to place in the SID (this is a systemwide limit)
    @return S_OK on success, a FAILED hresult containing the win32 error from creating the SID or querying the token otherwise.
    */
    template<typename... Ts> HRESULT test_token_membership_nothrow(_Out_ bool* result, _In_opt_ HANDLE token,
        const SID_IDENTIFIER_AUTHORITY& sidAuthority, Ts&&... subAuthorities)
    {
        *result = false;
        auto tempSid = make_static_sid(sidAuthority, wistd::forward<Ts>(subAuthorities)...);
        BOOL isMember;
        RETURN_IF_WIN32_BOOL_FALSE(CheckTokenMembership(token, &tempSid, &isMember));

        *result = (isMember != FALSE);

        return S_OK;
    }

#if (_WIN32_WINNT >= _WIN32_WINNT_WIN8)
    /** Determine whether a token represents an app container
    This method uses the passed in token and emits a boolean indicating that
    whether TokenIsAppContainer is true.
    ~~~~
    HRESULT OnlyIfAppContainer()
    {
    bool isAppContainer;
    RETURN_IF_FAILED(wil::get_token_is_app_container_nothrow(nullptr, isAppContainer));
    RETURN_HR_IF(E_ACCESSDENIED, !isAppContainer);
    RETURN_HR(...);
    }
    ~~~~
    @param token A token to get info about, or 'nullptr' to run as the current thread.
    */
    inline HRESULT get_token_is_app_container_nothrow(_In_opt_ HANDLE token, bool& value)
    {
        DWORD isAppContainer = 0;
        DWORD returnLength = 0;
        RETURN_IF_WIN32_BOOL_FALSE(::GetTokenInformation(
            token ? token : GetCurrentThreadEffectiveToken(),
            TokenIsAppContainer,
            &isAppContainer,
            sizeof(isAppContainer),
            &returnLength));

        value = (isAppContainer != 0);

        return S_OK;
    }

    //! A variant of get_token_is_app_container_nothrow that fails-fast on errors retrieving the token information
    inline bool get_token_is_app_container_failfast(HANDLE token = nullptr)
    {
        bool value = false;
        FAIL_FAST_IF_FAILED(get_token_is_app_container_nothrow(token, value));

        return value;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    //! A variant of get_token_is_app_container_nothrow that throws on errors retrieving the token information
    inline bool get_token_is_app_container(HANDLE token = nullptr)
    {
        bool value = false;
        THROW_IF_FAILED(get_token_is_app_container_nothrow(token, value));

        return value;
    }
#endif // WIL_ENABLE_EXCEPTIONS
#endif // _WIN32_WINNT >= _WIN32_WINNT_WIN8

    template<typename... Ts> bool test_token_membership_failfast(_In_opt_ HANDLE token,
        const SID_IDENTIFIER_AUTHORITY& sidAuthority, Ts&&... subAuthorities)
    {
        bool result;
        FAIL_FAST_IF_FAILED(test_token_membership_nothrow(&result, token, sidAuthority, wistd::forward<Ts>(subAuthorities)...));
        return result;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    template<typename... Ts> bool test_token_membership(_In_opt_ HANDLE token, const SID_IDENTIFIER_AUTHORITY& sidAuthority,
        Ts&&... subAuthorities)
    {
        bool result;
        THROW_IF_FAILED(test_token_membership_nothrow(&result, token, sidAuthority, wistd::forward<Ts>(subAuthorities)...));
        return result;
    }
#endif

} //namespace wil

#endif // __WIL_TOKEN_HELPERS_INCLUDED
