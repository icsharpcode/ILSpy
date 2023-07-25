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
#ifndef __WIL_COM_INCLUDED
#define __WIL_COM_INCLUDED

#include <WeakReference.h>
#include <combaseapi.h>
#include "result.h"
#include "resource.h" // last to ensure _COMBASEAPI_H_ protected definitions are available

#if __has_include(<tuple>)
#include <tuple>
#endif
#if __has_include(<type_traits>)
#include <type_traits>
#endif

// Forward declaration within WIL (see https://msdn.microsoft.com/en-us/library/br244983.aspx)
/// @cond
namespace Microsoft
{
    namespace WRL
    {
        template <typename T>
        class ComPtr;
    }
}
/// @endcond

namespace wil
{
    /// @cond
    namespace details
    {
        // We can't directly use wistd::is_convertible as it returns TRUE for an ambiguous conversion.
        // Adding is_abstract to the mix, enables us to allow conversion for interfaces, but deny it for
        // classes (where the multiple inheritance causes ambiguity).
        // NOTE:    I've reached out to vcsig on this topic and it turns out that __is_convertible_to should NEVER
        //          return true for ambiguous conversions.  This was a bug in our compiler that has since been fixed.
        //          Eventually, once that fix propagates we can move to a more efficient __is_convertible_to without
        //          the added complexity.
        template <class TFrom, class TTo>
        struct is_com_convertible :
            wistd::bool_constant<__is_convertible_to(TFrom, TTo) && (__is_abstract(TFrom) || wistd::is_same<TFrom, TTo>::value)>
        {
        };

        using tag_com_query = wistd::integral_constant<char, 0>;
        using tag_try_com_query = wistd::integral_constant<char, 1>;
        using tag_com_copy = wistd::integral_constant<char, 2>;
        using tag_try_com_copy = wistd::integral_constant<char, 3>;

        class default_query_policy
        {
        public:
            template <typename T>
            inline static HRESULT query(_In_ T* ptr, REFIID riid, _COM_Outptr_ void** result)
            {
                return ptr->QueryInterface(riid, result);
            }

            template <typename T, typename TResult>
            inline static HRESULT query(_In_ T* ptr, _COM_Outptr_ TResult** result)
            {
                return query_dispatch(ptr, typename details::is_com_convertible<T*, TResult*>::type(), result);
            }

        private:
            template <typename T, typename TResult>
            inline static HRESULT query_dispatch(_In_ T* ptr, wistd::true_type, _COM_Outptr_ TResult** result)     // convertible
            {
                *result = ptr;
                (*result)->AddRef();
                return S_OK;
            }

            template <typename T, typename TResult>
            inline static HRESULT query_dispatch(_In_ T* ptr, wistd::false_type, _COM_Outptr_ TResult** result)    // not convertible
            {
                auto hr = ptr->QueryInterface(IID_PPV_ARGS(result));
                __analysis_assume(SUCCEEDED(hr) || (*result == nullptr));
                return hr;
            }
        };

        template <typename T>
        struct query_policy_helper
        {
            using type = default_query_policy;
        };

        class weak_query_policy
        {
        public:
            inline static HRESULT query(_In_ IWeakReference* ptr, REFIID riid, _COM_Outptr_ void** result)
            {
                WI_ASSERT_MSG(riid != __uuidof(IWeakReference), "Cannot resolve a weak reference to IWeakReference");
                *result = nullptr;

                IInspectable* temp;
                HRESULT hr = ptr->Resolve(__uuidof(IInspectable), &temp);
                if (SUCCEEDED(hr))
                {
                    if (temp == nullptr)
                    {
                        return E_NOT_SET;
                    }
                    hr = temp->QueryInterface(riid, result);
                    __analysis_assume(SUCCEEDED(hr) || (*result == nullptr));
                    temp->Release();
                }

                return hr;
            }

            template <typename TResult>
            inline static HRESULT query(_In_ IWeakReference* ptr, _COM_Outptr_ TResult** result)
            {
                static_assert(!wistd::is_same<IWeakReference, TResult>::value, "Cannot resolve a weak reference to IWeakReference");
                return query_dispatch(ptr, wistd::is_base_of<IInspectable, TResult>(), result);
            }

        private:
            template <typename TResult>
            static HRESULT query_dispatch(_In_ IWeakReference* ptr, wistd::true_type, _COM_Outptr_ TResult** result)
            {
                auto hr = ptr->Resolve(__uuidof(TResult), reinterpret_cast<IInspectable**>(result));
                if (SUCCEEDED(hr) && (*result == nullptr))
                {
                    hr = E_NOT_SET;
                }
                __analysis_assume(SUCCEEDED(hr) || (*result == nullptr));
                return hr;
            }

            template <typename TResult>
            static HRESULT query_dispatch(_In_ IWeakReference* ptr, wistd::false_type, _COM_Outptr_ TResult** result)
            {
                return query(ptr, IID_PPV_ARGS(result));
            }
        };

        template <>
        struct query_policy_helper<IWeakReference>
        {
            using type = weak_query_policy;
        };

#if (NTDDI_VERSION >= NTDDI_WINBLUE)
        class agile_query_policy
        {
        public:
            inline static HRESULT query(_In_ IAgileReference* ptr, REFIID riid, _COM_Outptr_ void** result)
            {
                WI_ASSERT_MSG(riid != __uuidof(IAgileReference), "Cannot resolve a agile reference to IAgileReference");
                auto hr = ptr->Resolve(riid, result);
                __analysis_assume(SUCCEEDED(hr) || (*result == nullptr));       // IAgileReference::Resolve not annotated correctly
                return hr;
            }

            template <typename TResult>
            static HRESULT query(_In_ IAgileReference* ptr, _COM_Outptr_ TResult** result)
            {
                static_assert(!wistd::is_same<IAgileReference, TResult>::value, "Cannot resolve a agile reference to IAgileReference");
                return query(ptr, __uuidof(TResult), reinterpret_cast<void**>(result));
            }
        };

        template <>
        struct query_policy_helper<IAgileReference>
        {
            using type = agile_query_policy;
        };
#endif

        template <typename T>
        using query_policy_t = typename query_policy_helper<typename wistd::remove_pointer<T>::type>::type;

    } // details
    /// @endcond

    //! Represents the base template type that implements com_ptr, com_weak_ref, and com_agile_ref.
    //! See @ref page_comptr for more background.  See @ref page_query for more information on querying with WIL.
    //! @tparam T               Represents the type being held by the com_ptr_t.
    //!                         For com_ptr, this will always be the interface being represented.  For com_weak_ref, this will always be
    //!                         IWeakReference.  For com_agile_ref, this will always be IAgileReference.
    //! @tparam err_policy      Represents the error policy for the class (error codes, exceptions, or fail fast; see @ref page_errors)
    template <typename T, typename err_policy = err_exception_policy>
    class com_ptr_t
    {
    private:
        using element_type_reference = typename wistd::add_lvalue_reference<T>::type;
        using query_policy = details::query_policy_t<T>;
    public:
        //! The function return result (HRESULT or void) for the given err_policy (see @ref page_errors).
        using result = typename err_policy::result;
        //! The template type `T` being held by the com_ptr_t.
        using element_type = T;
        //! A pointer to the template type `T` being held by the com_ptr_t (what `get()` returns).
        using pointer = T*;

        //! @name Constructors
        //! @{

        //! Default constructor (holds nullptr).
        com_ptr_t() WI_NOEXCEPT :
            m_ptr(nullptr)
        {
        }

        //! Implicit construction from nullptr_t (holds nullptr).
        com_ptr_t(wistd::nullptr_t) WI_NOEXCEPT :
            com_ptr_t()
        {
        }

        //! Implicit construction from a compatible raw interface pointer (AddRef's the parameter).
        com_ptr_t(pointer ptr) WI_NOEXCEPT :
            m_ptr(ptr)
        {
            if (m_ptr)
            {
                m_ptr->AddRef();
            }
        }

        //! Copy-construction from a like `com_ptr_t` (copies and AddRef's the parameter).
        com_ptr_t(const com_ptr_t& other) WI_NOEXCEPT :
            com_ptr_t(other.get())
        {
        }

        //! Copy-construction from a convertible `com_ptr_t` (copies and AddRef's the parameter).
        template <class U, typename err, class = wistd::enable_if_t<__is_convertible_to(U*, pointer)>>
        com_ptr_t(const com_ptr_t<U, err>& other) WI_NOEXCEPT :
            com_ptr_t(static_cast<pointer>(other.get()))
        {
        }

        //! Move construction from a like `com_ptr_t` (avoids AddRef/Release by moving from the parameter).
        com_ptr_t(com_ptr_t&& other) WI_NOEXCEPT :
            m_ptr(other.detach())
        {
        }

        //! Move construction from a compatible `com_ptr_t` (avoids AddRef/Release by moving from the parameter).
        template <class U, typename err, class = wistd::enable_if_t<__is_convertible_to(U*, pointer)>>
        com_ptr_t(com_ptr_t<U, err>&& other) WI_NOEXCEPT :
            m_ptr(other.detach())
        {
        }
        //! @}

        //! Destructor (releases the pointer).
        ~com_ptr_t() WI_NOEXCEPT
        {
            if (m_ptr)
            {
                m_ptr->Release();
            }
        }

        //! @name Assignment operators
        //! @{

        //! Assign to nullptr (releases the current pointer, holds nullptr).
        com_ptr_t& operator=(wistd::nullptr_t) WI_NOEXCEPT
        {
            reset();
            return *this;
        }

        //! Assign a compatible raw interface pointer (releases current pointer, copies and AddRef's the parameter).
        com_ptr_t& operator=(pointer other) WI_NOEXCEPT
        {
            auto ptr = m_ptr;
            m_ptr = other;
            if (m_ptr)
            {
                m_ptr->AddRef();
            }
            if (ptr)
            {
                ptr->Release();
            }
            return *this;
        }

        //! Assign a like `com_ptr_t` (releases current pointer, copies and AddRef's the parameter).
        com_ptr_t& operator=(const com_ptr_t& other) WI_NOEXCEPT
        {
            return operator=(other.get());
        }

        //! Assign a convertible `com_ptr_t` (releases current pointer, copies and AddRef's the parameter).
        template <class U, typename err, class = wistd::enable_if_t<__is_convertible_to(U*, pointer)>>
        com_ptr_t& operator=(const com_ptr_t<U, err>& other) WI_NOEXCEPT
        {
            return operator=(static_cast<pointer>(other.get()));
        }

        //! Move assign from a like `com_ptr_t` (releases current pointer, avoids AddRef/Release by moving the parameter).
        com_ptr_t& operator=(com_ptr_t&& other) WI_NOEXCEPT
        {
            attach(other.detach());
            return *this;
        }

        //! Move assignment from a compatible `com_ptr_t` (releases current pointer, avoids AddRef/Release by moving from the parameter).
        template <class U, typename err, class = wistd::enable_if_t<__is_convertible_to(U*, pointer)>>
        com_ptr_t& operator=(com_ptr_t<U, err>&& other) WI_NOEXCEPT
        {
            attach(other.detach());
            return *this;
        }
        //! @}

        //! @name Modifiers
        //! @{

        //! Swap pointers with an another named com_ptr_t object.
        template <typename err>
        void swap(com_ptr_t<T, err>& other) WI_NOEXCEPT
        {
            auto ptr = m_ptr;
            m_ptr = other.m_ptr;
            other.m_ptr = ptr;
        }

        //! Swap pointers with a rvalue reference to another com_ptr_t object.
        template <typename err>
        void swap(com_ptr_t<T, err>&& other) WI_NOEXCEPT
        {
            swap(other);
        }

        //! Releases the pointer and sets it to nullptr.
        void reset() WI_NOEXCEPT
        {
            auto ptr = m_ptr;
            m_ptr = nullptr;
            if (ptr)
            {
                ptr->Release();
            }
        }

        //! Releases the pointer and sets it to nullptr.
        void reset(wistd::nullptr_t) WI_NOEXCEPT
        {
            reset();
        }

        //! Takes ownership of a compatible raw interface pointer (releases pointer, copies but DOES NOT AddRef the parameter).
        void attach(pointer other) WI_NOEXCEPT
        {
            auto ptr = m_ptr;
            m_ptr = other;
            if (ptr)
            {
                ULONG ref = ptr->Release();
                WI_ASSERT_MSG(((other != ptr) || (ref > 0)), "Bug: Attaching the same already assigned, destructed pointer");
            }
        }

        //! Relinquishes ownership and returns the internal interface pointer (DOES NOT release the detached pointer, sets class pointer to null).
        WI_NODISCARD pointer detach() WI_NOEXCEPT
        {
            auto temp = m_ptr;
            m_ptr = nullptr;
            return temp;
        }

        //! Returns the address of the internal pointer (releases ownership of the pointer BEFORE returning the address).
        //! The pointer is explicitly released to prevent accidental leaks of the pointer.  Coding standards generally indicate that
        //! there is little valid `_Inout_` use of `IInterface**`, making this safe to do under typical use.
        //! @see addressof
        //! ~~~~
        //! STDAPI GetMuffin(IMuffin **muffin);
        //! wil::com_ptr<IMuffin> myMuffin;
        //! THROW_IF_FAILED(GetMuffin(myMuffin.put()));
        //! ~~~~
        pointer* put() WI_NOEXCEPT
        {
            reset();
            return &m_ptr;
        }

        //! Returns the address of the internal pointer casted to void** (releases ownership of the pointer BEFORE returning the address).
        //! @see put
        void** put_void() WI_NOEXCEPT
        {
            return reinterpret_cast<void**>(put());
        }

        //! Returns the address of the internal pointer casted to IUnknown** (releases ownership of the pointer BEFORE returning the address).
        //! @see put
        ::IUnknown** put_unknown() WI_NOEXCEPT
        {
            return reinterpret_cast<::IUnknown**>(put());
        }

        //! Returns the address of the internal pointer (releases ownership of the pointer BEFORE returning the address).
        //! The pointer is explicitly released to prevent accidental leaks of the pointer.  Coding standards generally indicate that
        //! there is little valid `_Inout_` use of `IInterface**`, making this safe to do under typical use.  Since this behavior is not always immediately
        //! apparent, prefer to scope variables as close to use as possible (generally avoiding use of the same com_ptr variable in successive calls to
        //! receive an output interface).
        //! @see addressof
        pointer* operator&() WI_NOEXCEPT
        {
            return put();
        }

        //! Returns the address of the internal pointer (does not release the pointer; should not be used for `_Out_` parameters)
        pointer* addressof() WI_NOEXCEPT
        {
            return &m_ptr;
        }
        //! @}

        //! @name Inspection
        //! @{

        //! Returns the address of the const internal pointer (does not release the pointer)
        WI_NODISCARD const pointer* addressof() const WI_NOEXCEPT
        {
            return &m_ptr;
        }

        //! Returns 'true' if the pointer is assigned (NOT nullptr)
        WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
        {
            return (m_ptr != nullptr);
        }

        //! Returns the pointer
        WI_NODISCARD pointer get() const WI_NOEXCEPT
        {
            return m_ptr;
        }

        //! Allows direct calls against the pointer (AV on internal nullptr)
        WI_NODISCARD pointer operator->() const WI_NOEXCEPT
        {
            return m_ptr;
        }

        //! Dereferences the pointer (AV on internal nullptr)
        WI_NODISCARD element_type_reference operator*() const WI_NOEXCEPT
        {
            return *m_ptr;
        }
        //! @}

        //! @name Query helpers
        //! * Retrieves the requested interface
        //! * AV if the pointer is null
        //! * Produce an error if the requested interface is unsupported
        //!
        //! See @ref page_query for more information
        //! @{

        //! Query and return a smart pointer matching the interface specified by 'U':  `auto foo = m_ptr.query<IFoo>();`.
        //! See @ref page_query for more information.
        //!
        //! This method is the primary method that should be used to query a com_ptr in exception-based or fail-fast based code.
        //! Error-code returning code should use @ref query_to so that the returned HRESULT can be examined.  In the following
        //! examples, `m_ptr` is an exception-based or fail-fast based com_ptr, com_weak_ref, or com_agile_ref:
        //! ~~~~
        //! auto foo = ptr.query<IFoo>();
        //! foo->Method1();
        //! foo->Method2();
        //! ~~~~
        //! For simple single-method calls, this method allows removing the temporary that holds the com_ptr:
        //! ~~~~
        //! ptr.query<IFoo>()->Method1();
        //! ~~~~
        //! @tparam U Represents the interface being queried
        //! @return A `com_ptr_t` pointer to the given interface `U`.  The pointer is guaranteed not null.  The returned
        //!         `com_ptr_t` type will be @ref com_ptr or @ref com_ptr_failfast (matching the error handling form of the
        //!         pointer being queried (exception based or fail-fast).
        template <class U>
        WI_NODISCARD inline com_ptr_t<U, err_policy> query() const
        {
            static_assert(wistd::is_same<void, result>::value, "query requires exceptions or fail fast; use try_query or query_to");
            return com_ptr_t<U, err_policy>(m_ptr, details::tag_com_query());
        }

        //! Query for the interface of the given out parameter `U`:  `ptr.query_to(&foo);`.
        //! See @ref page_query for more information.
        //!
        //! For fail-fast and exception-based behavior this routine should primarily be used to write to out parameters and @ref query should
        //! be used to perform most queries.  For error-code based code, this routine is the primary method that should be used to query a com_ptr.
        //!
        //! Error-code based samples:
        //! ~~~~
        //! // class member being queried:
        //! wil::com_ptr_nothrow<IUnknown> m_ptr;
        //!
        //! // simple query example:
        //! wil::com_ptr_nothrow<IFoo> foo;
        //! RETURN_IF_FAILED(m_ptr.query_to(&foo));
        //! foo->FooMethod1();
        //!
        //! // output parameter example:
        //! HRESULT GetFoo(_COM_Outptr_ IFoo** fooPtr)
        //! {
        //!     RETURN_IF_FAILED(m_ptr.query_to(fooPtr));
        //!     return S_OK;
        //! }
        //! ~~~~
        //! Exception or fail-fast samples:
        //! ~~~~
        //! // class member being queried
        //! wil::com_ptr<IUnknown> m_ptr;
        //!
        //! void GetFoo(_COM_Outptr_ IFoo** fooPtr)
        //! {
        //!     m_ptr.query_to(fooPtr);
        //! }
        //! ~~~~
        //! @tparam U           Represents the interface being queried (type of the output parameter).  This interface does not need to
        //!                     be specified directly.  Rely upon template type deduction to pick up the type from the output parameter.
        //! @param ptrResult    The output pointer that will receive the newly queried interface.  This pointer will be assigned null on failure.
        //! @return             For the nothrow (error code-based) classes (@ref com_ptr_nothrow, @ref com_weak_ref_nothrow, @ref com_agile_ref_nothrow) this
        //!                     method returns an `HRESULT` indicating whether the query was successful.  Exception-based and fail-fast based classes
        //!                     do not return a value (void).
        template <class U>
        result query_to(_COM_Outptr_ U** ptrResult) const
        {
            // Prefast cannot see through the error policy + query_policy mapping and as a result fires 6388 and 28196 for this function.
            // Suppression is also not working. Wrapping this entire function in #pragma warning(disable: 6388 28196) does not stop all of the prefast errors
            // from being emitted.
#if defined(_PREFAST_)
            *ptrResult = nullptr;
            return err_policy::HResult(E_NOINTERFACE);
#else
            return err_policy::HResult(query_policy::query(m_ptr, ptrResult));
#endif
        }

        //! Query for the requested interface using the iid, ppv pattern:  `ptr.query_to(riid, ptr);`.
        //! See @ref page_query for more information.
        //!
        //! This method is built to implement an API boundary that exposes a returned pointer to a caller through the REFIID and void** pointer
        //! pattern (like QueryInterface).  This pattern should not be used outside of that pattern (through IID_PPV_ARGS) as it is less efficient
        //! than the typed version of @ref query_to which can elide the QueryInterface in favor of AddRef when the types are convertible.
        //! ~~~~
        //! // class member being queried:
        //! wil::com_ptr_nothrow<IUnknown> m_ptr;
        //!
        //! // output parameter example:
        //! HRESULT GetFoo(REFIID riid, _COM_Outptr_ void** ptrResult)
        //! {
        //!     RETURN_IF_FAILED(m_ptr.query_to(riid, ptrResult));
        //!     return S_OK;
        //! }
        //! ~~~~
        //! @param riid         The interface to query for.
        //! @param ptrResult    The output pointer that will receive the newly queried interface.  This pointer will be assigned null on failure.
        //! @return             For the nothrow (error code-based) classes (@ref com_ptr_nothrow, @ref com_weak_ref_nothrow, @ref com_agile_ref_nothrow) this
        //!                     method returns an `HRESULT` indicating whether the query was successful.  Exception-based and fail-fast based classes
        //!                     do not return a value (void).
        result query_to(REFIID riid, _COM_Outptr_ void** ptrResult) const
        {
            // Prefast cannot see through the error policy + query_policy mapping and as a result and as a result fires 6388 and 28196 for this function.
            // Suppression is also not working. Wrapping this entire function in #pragma warning(disable: 6388 28196) does not stop the prefast errors
            // from being emitted.
#if defined(_PREFAST_)
            *ptrResult = nullptr;
            return err_policy::HResult(E_NOINTERFACE);
#else
            return err_policy::HResult(query_policy::query(m_ptr, riid, ptrResult));
#endif
        }
        //! @}

        //! @name Try query helpers
        //! * Attempts to retrieves the requested interface
        //! * AV if the pointer is null
        //! * Produce null if the requested interface is unsupported
        //! * bool returns 'true' when query was successful
        //!
        //! See @ref page_query for more information.
        //! @{

        //! Attempt a query and return a smart pointer matching the interface specified by 'U':  `auto foo = m_ptr.try_query<IFoo>();` (null result when interface is unsupported).
        //! See @ref page_query for more information.
        //!
        //! This method can be used to query a com_ptr for an interface when it's known that support for that interface is
        //! optional (failing the query should not produce an error).  The caller must examine the returned pointer to see
        //! if it's null before using it:
        //! ~~~~
        //! auto foo = ptr.try_query<IFoo>();
        //! if (foo)
        //! {
        //!     foo->Method1();
        //!     foo->Method2();
        //! }
        //! ~~~~
        //! @tparam U   Represents the interface being queried
        //! @return     A `com_ptr_t` pointer to the given interface `U`.  The returned pointer will be null if the interface is
        //!             not supported.  The returned `com_ptr_t` will have the same error handling policy (exceptions, failfast or error codes) as
        //!             the pointer being queried.
        template <class U>
        WI_NODISCARD inline com_ptr_t<U, err_policy> try_query() const
        {
            return com_ptr_t<U, err_policy>(m_ptr, details::tag_try_com_query());
        }

        //! Attempts to query for the interface matching the given output parameter; returns a bool indicating if the query was successful (non-null).
        //! See @ref page_query for more information.
        //!
        //! This method can be used to perform a query against a non-null interface when it's known that support for that interface is
        //! optional (failing the query should not produce an error).  The caller must examine the returned bool before using the returned pointer.
        //! ~~~~
        //! wil::com_ptr_nothrow<IFoo> foo;
        //! if (ptr.try_query_to(&foo))
        //! {
        //!     foo->Method1();
        //!     foo->Method2();
        //! }
        //! ~~~~
        //! @param ptrResult    The pointer to query for.  The interface to query is deduced from the type of this out parameter; do not specify
        //!                     the type directly to the template.
        //! @return             A `bool` indicating `true` of the query was successful (the returned parameter is non-null).
        template <class U>
        _Success_return_ bool try_query_to(_COM_Outptr_ U** ptrResult) const
        {
            return SUCCEEDED(query_policy::query(m_ptr, ptrResult));
        }

        //! Attempts a query for the requested interface using the iid, ppv pattern:  `ptr.try_query_to(riid, ptr);`.
        //! See @ref page_query for more information.
        //!
        //! This method is built to implement an API boundary that exposes a returned pointer to a caller through the REFIID and void** pointer
        //! pattern (like QueryInterface).  The key distinction is that this routine does not produce an error if the request isn't fulfilled, so
        //! it's appropriate for `_COM_Outptr_result_maybenull_` cases.  This pattern should not be used outside of that pattern (through IID_PPV_ARGS) as
        //! it is less efficient than the typed version of @ref try_query_to which can elide the QueryInterface in favor of AddRef when the types are convertible.
        //! The caller must examine the returned bool before using the returned pointer.
        //! ~~~~
        //! // class member being queried:
        //! wil::com_ptr_nothrow<IUnknown> m_ptr;
        //!
        //! // output parameter example (result may be null):
        //! HRESULT GetFoo(REFIID riid, _COM_Outptr_result_maybenull_ void** ptrResult)
        //! {
        //!     m_ptr.try_query_to(riid, ptrResult);
        //!     return S_OK;
        //! }
        //! ~~~~
        //! @param riid         The interface to query for.
        //! @param ptrResult    The output pointer that will receive the newly queried interface.  This pointer will be assigned null on failure.
        //! @return             A `bool` indicating `true` of the query was successful (the returned parameter is non-null).
        _Success_return_ bool try_query_to(REFIID riid, _COM_Outptr_ void** ptrResult) const
        {
            return SUCCEEDED(query_policy::query(m_ptr, riid, ptrResult));
        }
        //! @}

        //! @name Copy helpers
        //! * Retrieves the requested interface
        //! * Succeeds with null if the pointer is null
        //! * Produce an error if the requested interface is unsupported
        //!
        //! See @ref page_query for more information.
        //! @{

        //! Query and return a smart pointer matching the interface specified by 'U':  `auto foo = m_ptr.copy<IFoo>();` (succeeds and returns a null ptr if the queried pointer is null).
        //! See @ref page_query for more information.
        //!
        //! This method is identical to @ref query with the exception that it can be used when the pointer is null.  When used
        //! against a null pointer, the returned pointer will always be null and an error will not be produced.  Like query it will
        //! produce an error for a non-null pointer that does not support the requested interface.
        //! @tparam U Represents the interface being queried
        //! @return A `com_ptr_t` pointer to the given interface `U`.  The pointer will be null ONLY if the pointer being queried is null.  The returned
        //!         `com_ptr_t` type will be @ref com_ptr or @ref com_ptr_failfast (matching the error handling form of the
        //!         pointer being queried (exception based or fail-fast).
        template <class U>
        WI_NODISCARD inline com_ptr_t<U, err_policy> copy() const
        {
            static_assert(wistd::is_same<void, result>::value, "copy requires exceptions or fail fast; use the try_copy or copy_to method");
            return com_ptr_t<U, err_policy>(m_ptr, details::tag_com_copy());
        }

        //! Query for the interface of the given out parameter `U`:  `ptr.copy_to(&foo);` (succeeds and returns null ptr if the queried pointer is null).
        //! See @ref page_query for more information.
        //!
        //! This method is identical to @ref query_to with the exception that it can be used when the pointer is null.  When used
        //! against a null pointer, the returned pointer will always be null and an error will not be produced.  Like query_to it will
        //! produce an error for a non-null pointer that does not support the requested interface.
        //! @tparam U           Represents the interface being queried (type of the output parameter).  This interface does not need to
        //!                     be specified directly.  Rely upon template type deduction to pick up the type from the output parameter.
        //! @param ptrResult    The output pointer that will receive the newly queried interface.  This pointer will be assigned null on failure OR assigned null
        //!                     when the source pointer is null.
        //! @return             For the nothrow (error code-based) classes (@ref com_ptr_nothrow, @ref com_weak_ref_nothrow, @ref com_agile_ref_nothrow) this
        //!                     method returns an `HRESULT` indicating whether the query was successful.  Copying a null value is considered success. Exception-based
        //!                     and fail-fast based classes do not return a value (void).
        template <class U>
        result copy_to(_COM_Outptr_result_maybenull_ U** ptrResult) const
        {
            if (m_ptr)
            {
                // Prefast cannot see through the error policy + query_policy mapping and as a result and as a result fires 6388 and 28196 for this function.
                // Suppression is also not working. Wrapping this entire function in #pragma warning(disable: 6388 28196) does not stop the prefast errors
                // from being emitted.
#if defined(_PREFAST_)
                *ptrResult = nullptr;
                return err_policy::HResult(E_NOINTERFACE);
#else
                return err_policy::HResult(query_policy::query(m_ptr, ptrResult));
#endif
            }
            *ptrResult = nullptr;
            return err_policy::OK();
        }

        //! Query for the requested interface using the iid, ppv pattern:  `ptr.copy_to(riid, ptr);`. (succeeds and returns null ptr if the queried pointer is null).
        //! See @ref page_query for more information.
        //!
        //! Identical to the corresponding @ref query_to method with the exception that it can be used when the pointer is null.  When used
        //! against a null pointer, the returned pointer will always be null and an error will not be produced.  Like query_to it will
        //! produce an error for a non-null pointer that does not support the requested interface.
        //! @param riid         The interface to query for.
        //! @param ptrResult    The output pointer that will receive the newly queried interface.  This pointer will be assigned null on failure OR assigned null
        //!                     when the source pointer is null.
        //! @return             For the nothrow (error code-based) classes (@ref com_ptr_nothrow, @ref com_weak_ref_nothrow, @ref com_agile_ref_nothrow) this
        //!                     method returns an `HRESULT` indicating whether the query was successful.  Copying a null value is considered success.  Exception-based
        //!                     and fail-fast based classes do not return a value (void).
        result copy_to(REFIID riid, _COM_Outptr_result_maybenull_ void** ptrResult) const
        {
            if (m_ptr)
            {
                // Prefast cannot see through the error policy + query_policy mapping and as a result and as a result fires 6388 and 28196 for this function.
                // Suppression is also not working. Wrapping this entire function in #pragma warning(disable: 6388 28196) does not stop the prefast errors
                // from being emitted.
#if defined(_PREFAST_)
                *ptrResult = nullptr;
                return err_policy::HResult(E_NOINTERFACE);
#else
                return err_policy::HResult(query_policy::query(m_ptr, riid, ptrResult));
#endif
            }
            *ptrResult = nullptr;
            return err_policy::OK();
        }
        //! @}

        //! @name Try copy helpers
        //! * Attempts to retrieves the requested interface
        //! * Successfully produces null if the queried pointer is already null
        //! * Produce null if the requested interface is unsupported
        //! * bool returns 'false' ONLY when the queried pointer is not null and the requested interface is unsupported
        //!
        //! See @ref page_query for more information.
        //! @{

        //! Attempt a query and return a smart pointer matching the interface specified by 'U':  `auto foo = m_ptr.try_query<IFoo>();` (null result when interface is unsupported or queried pointer is null).
        //! See @ref page_query for more information.
        //!
        //! Identical to the corresponding @ref try_query method with the exception that it can be used when the pointer is null.  When used
        //! against a null pointer, the returned pointer will always be null and an error will not be produced.
        //! @tparam U   Represents the interface being queried
        //! @return     A `com_ptr_t` pointer to the given interface `U`.  The returned pointer will be null if the interface was
        //!             not supported or the pointer being queried is null.  The returned `com_ptr_t` will have the same error handling
        //!             policy (exceptions, failfast or error codes) as the pointer being queried.
        template <class U>
        WI_NODISCARD inline com_ptr_t<U, err_policy> try_copy() const
        {
            return com_ptr_t<U, err_policy>(m_ptr, details::tag_try_com_copy());
        }

        //! Attempts to query for the interface matching the given output parameter; returns a bool indicating if the query was successful (returns `false` if the pointer is null).
        //! See @ref page_query for more information.
        //!
        //! Identical to the corresponding @ref try_query_to method with the exception that it can be used when the pointer is null.  When used
        //! against a null pointer, the returned pointer will be null and the return value will be `false`.
        //! @param ptrResult    The pointer to query for.  The interface to query is deduced from the type of this out parameter; do not specify
        //!                     the type directly to the template.
        //! @return             A `bool` indicating `true` of the query was successful (the returned parameter is non-null).
        template <class U>
        _Success_return_ bool try_copy_to(_COM_Outptr_result_maybenull_ U** ptrResult) const
        {
            if (m_ptr)
            {
                return SUCCEEDED(query_policy::query(m_ptr, ptrResult));
            }
            *ptrResult = nullptr;
            return false;
        }

        //! Attempts a query for the requested interface using the iid, ppv pattern:  `ptr.try_query_to(riid, ptr);` (returns `false` if the pointer is null)
        //! See @ref page_query for more information.
        //!
        //! Identical to the corresponding @ref try_query_to method with the exception that it can be used when the pointer is null.  When used
        //! against a null pointer, the returned pointer will be null and the return value will be `false`.
        //! @param riid         The interface to query for.
        //! @param ptrResult    The output pointer that will receive the newly queried interface.  This pointer will be assigned null on failure or
        //!                     if the source pointer being queried is null.
        //! @return             A `bool` indicating `true` of the query was successful (the returned parameter is non-null).  Querying a null
        //!                     pointer will return `false` with a null result.
        _Success_return_ bool try_copy_to(REFIID riid, _COM_Outptr_result_maybenull_ void** ptrResult) const
        {
            if (m_ptr)
            {
                return SUCCEEDED(query_policy::query(m_ptr, riid, ptrResult));
            }
            *ptrResult = nullptr;
            return false;
        }
        //! @}

        //! @name WRL compatibility
        //! @{

        //! Copy construct from a compatible WRL ComPtr<T>.
        template <class U, class = wistd::enable_if_t<__is_convertible_to(U*, pointer)>>
        com_ptr_t(const Microsoft::WRL::ComPtr<U>& other) WI_NOEXCEPT :
            com_ptr_t(static_cast<pointer>(other.Get()))
        {
        }

        //! Move construct from a compatible WRL ComPtr<T>.
        template <class U, class = wistd::enable_if_t<__is_convertible_to(U*, pointer)>>
        com_ptr_t(Microsoft::WRL::ComPtr<U>&& other) WI_NOEXCEPT :
            m_ptr(other.Detach())
        {
        }

        //! Assign from a compatible WRL ComPtr<T>.
        template <class U, class = wistd::enable_if_t<__is_convertible_to(U*, pointer)>>
        com_ptr_t& operator=(const Microsoft::WRL::ComPtr<U>& other) WI_NOEXCEPT
        {
            return operator=(static_cast<pointer>(other.Get()));
        }

        //! Move assign from a compatible WRL ComPtr<T>.
        template <class U, class = wistd::enable_if_t<__is_convertible_to(U*, pointer)>>
        com_ptr_t& operator=(Microsoft::WRL::ComPtr<U>&& other) WI_NOEXCEPT
        {
            attach(other.Detach());
            return *this;
        }

        //! Swap pointers with a WRL ComPtr<T> to the same interface.
        void swap(Microsoft::WRL::ComPtr<T>& other) WI_NOEXCEPT
        {
            auto ptr = m_ptr;
            m_ptr = other.Detach();
            other.Attach(ptr);
        }

        //! Swap pointers with a rvalue reference to a WRL ComPtr<T> to the same interface.
        void swap(Microsoft::WRL::ComPtr<T>&& other) WI_NOEXCEPT
        {
            swap(other);
        }
        //! @}  // WRL compatibility

    public:
        // Internal Helpers
        /// @cond
        template <class U>
        inline com_ptr_t(_In_ U* ptr, details::tag_com_query) : m_ptr(nullptr)
        {
            err_policy::HResult(details::query_policy_t<U>::query(ptr, &m_ptr));
        }

        template <class U>
        inline com_ptr_t(_In_ U* ptr, details::tag_try_com_query) WI_NOEXCEPT : m_ptr(nullptr)
        {
            details::query_policy_t<U>::query(ptr, &m_ptr);
        }

        template <class U>
        inline com_ptr_t(_In_opt_ U* ptr, details::tag_com_copy) : m_ptr(nullptr)
        {
            if (ptr)
            {
                err_policy::HResult(details::query_policy_t<U>::query(ptr, &m_ptr));
            }
        }

        template <class U>
        inline com_ptr_t(_In_opt_ U* ptr, details::tag_try_com_copy) WI_NOEXCEPT : m_ptr(nullptr)
        {
            if (ptr)
            {
                details::query_policy_t<U>::query(ptr, &m_ptr);
            }
        }
        /// @endcond

    private:
        pointer m_ptr;
    };

    // Error-policy driven forms of com_ptr

#ifdef WIL_ENABLE_EXCEPTIONS
    //! COM pointer, errors throw exceptions (see @ref com_ptr_t for details)
    template <typename T>
    using com_ptr = com_ptr_t<T, err_exception_policy>;
#endif

    //! COM pointer, errors return error codes (see @ref com_ptr_t for details)
    template <typename T>
    using com_ptr_nothrow = com_ptr_t<T, err_returncode_policy>;

    //! COM pointer, errors fail-fast (see @ref com_ptr_t for details)
    template <typename T>
    using com_ptr_failfast = com_ptr_t<T, err_failfast_policy>;


    // Global operators / swap

    //! Swaps the given com pointers that have different error handling.
    //! Note that there are also corresponding versions to allow you to swap any wil com_ptr<T> with a WRL ComPtr<T>.
    template <typename T, typename ErrLeft, typename ErrRight>
    inline void swap(com_ptr_t<T, ErrLeft>& left, com_ptr_t<T, ErrRight>& right) WI_NOEXCEPT
    {
        left.swap(right);
    }

    //! Swaps the given com pointers that have the same error handling.
    template <typename T, typename Err>
    inline void swap(com_ptr_t<T, Err>& left, com_ptr_t<T, Err>& right) WI_NOEXCEPT
    {
        left.swap(right);
    }

    //! Compare two com pointers.
    //! Compares the two raw com pointers for equivalence.  Does NOT compare object identity with a QI for IUnknown.
    //!
    //! Note that documentation for all of the various comparators has not been generated to reduce global function
    //! clutter, but ALL standard comparison operators are supported between wil com_ptr<T> objects, nullptr_t, and
    //! WRL ComPtr<T>.
    template <typename TLeft, typename ErrLeft, typename TRight, typename ErrRight>
    inline bool operator==(const com_ptr_t<TLeft, ErrLeft>& left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
    {
        static_assert(__is_convertible_to(TLeft*, TRight*) || __is_convertible_to(TRight*, TLeft*), "comparison operator requires left and right pointers to be compatible");
        return (left.get() == right.get());
    }

    // We don't document all of the global comparison operators (reduce clutter)
    /// @cond
    template <typename TLeft, typename ErrLeft, typename TRight, typename ErrRight>
    inline bool operator<(const com_ptr_t<TLeft, ErrLeft>& left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
    {
        static_assert(__is_convertible_to(TLeft*, TRight*) || __is_convertible_to(TRight*, TLeft*), "comparison operator requires left and right pointers to be compatible");
        return (left.get() < right.get());
    }

    template <typename TLeft, typename ErrLeft>
    inline bool operator==(const com_ptr_t<TLeft, ErrLeft>& left, wistd::nullptr_t) WI_NOEXCEPT
    {
        return (left.get() == nullptr);
    }

    template <typename TLeft, typename ErrLeft, typename TRight, typename ErrRight>
    inline bool operator!=(const com_ptr_t<TLeft, ErrLeft>& left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
        { return (!(left == right)); }

    template <typename TLeft, typename ErrLeft, typename TRight, typename ErrRight>
    inline bool operator>=(const com_ptr_t<TLeft, ErrLeft>& left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
        { return (!(left < right)); }

    template <typename TLeft, typename ErrLeft, typename TRight, typename ErrRight>
    inline bool operator>(const com_ptr_t<TLeft, ErrLeft>& left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
        { return (right < left); }

    template <typename TLeft, typename ErrLeft, typename TRight, typename ErrRight>
    inline bool operator<=(const com_ptr_t<TLeft, ErrLeft>& left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
        { return (!(right < left)); }

    template <typename TRight, typename ErrRight>
    inline bool operator==(wistd::nullptr_t, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
    {
        return (right.get() == nullptr);
    }

    template <typename TLeft, typename ErrLeft>
    inline bool operator!=(const com_ptr_t<TLeft, ErrLeft>& left, wistd::nullptr_t) WI_NOEXCEPT
        { return (!(left == nullptr)); }

    template <typename TRight, typename ErrRight>
    inline bool operator!=(wistd::nullptr_t, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
        { return (!(right == nullptr)); }

    // WRL ComPtr support

    template <typename T, typename ErrLeft>
    inline void swap(com_ptr_t<T, ErrLeft>& left, Microsoft::WRL::ComPtr<T>& right) WI_NOEXCEPT
    {
        left.swap(right);
    }

    template <typename TLeft, typename ErrLeft, typename TRight>
    inline bool operator==(const com_ptr_t<TLeft, ErrLeft>& left, const Microsoft::WRL::ComPtr<TRight>& right) WI_NOEXCEPT
    {
        static_assert(__is_convertible_to(TLeft*, TRight*) || __is_convertible_to(TRight*, TLeft*), "comparison operator requires left and right pointers to be compatible");
        return (left.get() == right.Get());
    }

    template <typename TLeft, typename ErrLeft, typename TRight>
    inline bool operator<(const com_ptr_t<TLeft, ErrLeft>& left, const Microsoft::WRL::ComPtr<TRight>& right) WI_NOEXCEPT
    {
        static_assert(__is_convertible_to(TLeft*, TRight*) || __is_convertible_to(TRight*, TLeft*), "comparison operator requires left and right pointers to be compatible");
        return (left.get() < right.Get());
    }

    template <typename TLeft, typename ErrLeft, typename TRight>
    inline bool operator!=(const com_ptr_t<TLeft, ErrLeft>& left, const Microsoft::WRL::ComPtr<TRight>& right) WI_NOEXCEPT
        { return (!(left == right)); }

    template <typename TLeft, typename ErrLeft, typename TRight>
    inline bool operator>=(const com_ptr_t<TLeft, ErrLeft>& left, const Microsoft::WRL::ComPtr<TRight>& right) WI_NOEXCEPT
        { return (!(left < right)); }

    template <typename TLeft, typename ErrLeft, typename TRight>
    inline bool operator>(const com_ptr_t<TLeft, ErrLeft>& left, const Microsoft::WRL::ComPtr<TRight>& right) WI_NOEXCEPT
        { return (right < left); }

    template <typename TLeft, typename ErrLeft, typename TRight>
    inline bool operator<=(const com_ptr_t<TLeft, ErrLeft>& left, const Microsoft::WRL::ComPtr<TRight>& right) WI_NOEXCEPT
        { return (!(right < left)); }

    template <typename T, typename ErrRight>
    inline void swap(Microsoft::WRL::ComPtr<T>& left, com_ptr_t<T, ErrRight>& right) WI_NOEXCEPT
    {
        right.swap(left);
    }

    template <typename TLeft, typename TRight, typename ErrRight>
    inline bool operator==(const Microsoft::WRL::ComPtr<TLeft>& left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
    {
        static_assert(__is_convertible_to(TLeft*, TRight*) || __is_convertible_to(TRight*, TLeft*), "comparison operator requires left and right pointers to be compatible");
        return (left.Get() == right.get());
    }

    template <typename TLeft, typename TRight, typename ErrRight>
    inline bool operator<(const Microsoft::WRL::ComPtr<TLeft>& left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
    {
        static_assert(__is_convertible_to(TLeft*, TRight*) || __is_convertible_to(TRight*, TLeft*), "comparison operator requires left and right pointers to be compatible");
        return (left.Get() < right.get());
    }

    template <typename TLeft, typename TRight, typename ErrRight>
    inline bool operator!=(const Microsoft::WRL::ComPtr<TLeft>& left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
        { return (!(left == right)); }

    template <typename TLeft, typename TRight, typename ErrRight>
    inline bool operator>=(const Microsoft::WRL::ComPtr<TLeft>& left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
        { return (!(left < right)); }

    template <typename TLeft, typename TRight, typename ErrRight>
    inline bool operator>(const Microsoft::WRL::ComPtr<TLeft>& left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
        { return (right < left); }

    template <typename TLeft, typename TRight, typename ErrRight>
    inline bool operator<=(const Microsoft::WRL::ComPtr<TLeft>& left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
        { return (!(right < left)); }

    // raw COM pointer support
    //
    // Use these for convenience and to avoid unnecessary AddRef/Release cyles when using raw
    // pointers to access STL containers. Specify std::less<> to benefit from operator<.
    //
    // Example: std::set<wil::com_ptr<IUnknown>, std::less<>> set;

    template <typename TLeft, typename ErrLeft, typename TRight>
    inline bool operator==(const com_ptr_t<TLeft, ErrLeft>& left, TRight* right) WI_NOEXCEPT
    {
        static_assert(__is_convertible_to(TLeft*, TRight*) || __is_convertible_to(TRight*, TLeft*), "comparison operator requires left and right pointers to be compatible");
        return (left.get() == right);
    }

    template <typename TLeft, typename ErrLeft, typename TRight>
    inline bool operator<(const com_ptr_t<TLeft, ErrLeft>& left, TRight* right) WI_NOEXCEPT
    {
        static_assert(__is_convertible_to(TLeft*, TRight*) || __is_convertible_to(TRight*, TLeft*), "comparison operator requires left and right pointers to be compatible");
        return (left.get() < right);
    }

    template <typename TLeft, typename ErrLeft, typename TRight>
    inline bool operator!=(const com_ptr_t<TLeft, ErrLeft>& left, TRight* right) WI_NOEXCEPT
        { return (!(left == right)); }

    template <typename TLeft, typename ErrLeft, typename TRight>
    inline bool operator>=(const com_ptr_t<TLeft, ErrLeft>& left, TRight* right) WI_NOEXCEPT
        { return (!(left < right)); }

    template <typename TLeft, typename ErrLeft, typename TRight>
    inline bool operator>(const com_ptr_t<TLeft, ErrLeft>& left, TRight* right) WI_NOEXCEPT
        { return (right < left); }

    template <typename TLeft, typename ErrLeft, typename TRight>
    inline bool operator<=(const com_ptr_t<TLeft, ErrLeft>& left, TRight* right) WI_NOEXCEPT
        { return (!(right < left)); }

    template <typename TLeft, typename TRight, typename ErrRight>
    inline bool operator==(TLeft* left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
    {
        static_assert(__is_convertible_to(TLeft*, TRight*) || __is_convertible_to(TRight*, TLeft*), "comparison operator requires left and right pointers to be compatible");
        return (left == right.get());
    }

    template <typename TLeft, typename TRight, typename ErrRight>
    inline bool operator<(TLeft* left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
    {
        static_assert(__is_convertible_to(TLeft*, TRight*) || __is_convertible_to(TRight*, TLeft*), "comparison operator requires left and right pointers to be compatible");
        return (left < right.get());
    }

    template <typename TLeft, typename TRight, typename ErrRight>
    inline bool operator!=(TLeft* left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
        { return (!(left == right)); }

    template <typename TLeft, typename TRight, typename ErrRight>
    inline bool operator>=(TLeft* left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
        { return (!(left < right)); }

    template <typename TLeft, typename TRight, typename ErrRight>
    inline bool operator>(TLeft* left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
        { return (right < left); }

    template <typename TLeft, typename TRight, typename ErrRight>
    inline bool operator<=(TLeft* left, const com_ptr_t<TRight, ErrRight>& right) WI_NOEXCEPT
        { return (!(right < left)); }

    // suppress documentation of every single comparison operator
    /// @endcond


    //! An overloaded function that retrieves the raw com pointer from a raw pointer, wil::com_ptr_t<T>, WRL ComPtr<T>, or Platform::Object^.
    //! This function is primarily useful by library or helper code.  It allows code to be written to accept a forwarding reference
    //! template that can be used as an input com pointer.  That input com pointer is allowed to be any of:
    //! * Raw Pointer:  `T* com_raw_ptr(T* ptr)`
    //! * Wil com_ptr:  `T* com_raw_ptr(const wil::com_ptr_t<T, err>& ptr)`
    //! * WRL ComPtr:   `T* com_raw_ptr(const Microsoft::WRL::ComPtr<T>& ptr)`
    //! * C++/CX hat:   `IInspectable* com_raw_ptr(Platform::Object^ ptr)`
    //!
    //! Which in turn allows code like the following to be written:
    //! ~~~~
    //! template <typename U, typename T>
    //! void com_query_to(T&& ptrSource, _COM_Outptr_ U** ptrResult)
    //! {
    //!     auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
    //!     // decltype(raw) has the type of the inner pointer and raw is guaranteed to be a raw com pointer
    //! ~~~~
    template <typename T>
    T* com_raw_ptr(T* ptr)
    {
        return ptr;
    }

    /// @cond
    template <typename T, typename err>
    T* com_raw_ptr(const wil::com_ptr_t<T, err>& ptr)
    {
        return ptr.get();
    }

    template <typename T>
    T* com_raw_ptr(const Microsoft::WRL::ComPtr<T>& ptr)
    {
        return ptr.Get();
    }

#ifdef __cplusplus_winrt

    template <typename T>
    inline IInspectable* com_raw_ptr(T^ ptr)
    {
        return reinterpret_cast<IInspectable*>(static_cast<::Platform::Object^>(ptr));
    }

#endif
    /// @endcond

#ifdef WIL_ENABLE_EXCEPTIONS
    //! Constructs a `com_ptr` from a raw pointer.
    //! This avoids having to restate the interface in pre-C++20.
    //! Starting in C++20, you can write `wil::com_ptr(p)` directly.
    //! ~~~
    //! void example(ILongNamedThing* thing)
    //! {
    //!    callback([thing = wil::make_com_ptr(thing)] { /* do something */ });
    //! }
    //! ~~~
    template <typename T>
    com_ptr<T> make_com_ptr(T* p) { return p; }
#endif

    //! Constructs a `com_ptr_nothrow` from a raw pointer.
    //! This avoids having to restate the interface in pre-C++20.
    //! Starting in C++20, you can write `wil::com_ptr_nothrow(p)` directly.
    //! ~~~
    //! void example(ILongNamedThing* thing)
    //! {
    //!    callback([thing = wil::make_com_ptr_nothrow(thing)] { /* do something */ });
    //! }
    //! ~~~
    template <typename T>
    com_ptr_nothrow<T> make_com_ptr_nothrow(T* p) { return p; }

    //! Constructs a `com_ptr_failfast` from a raw pointer.
    //! This avoids having to restate the interface in pre-C++20.
    //! Starting in C++20, you can write `wil::com_ptr_failfast(p)` directly.
    //! ~~~
    //! void example(ILongNamedThing* thing)
    //! {
    //!    callback([thing = wil::make_com_ptr_failfast(thing)] { /* do something */ });
    //! }
    //! ~~~
    template <typename T>
    com_ptr_failfast<T> make_com_ptr_failfast(T* p) { return p; }

    //! @name Stand-alone query helpers
    //! * Source pointer can be raw interface pointer, any wil com_ptr, or WRL ComPtr
    //! * Retrieves the requested interface
    //! * AV if the source pointer is null
    //! * Produce an error if the requested interface is unsupported
    //!
    //! See @ref page_query for more information
    //! @{

#ifdef WIL_ENABLE_EXCEPTIONS
    //! Queries for the specified interface and returns an exception-based wil::com_ptr to that interface (exception if unsupported).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), should not be null
    //! @tparam U           Represents the interface being queried
    //! @return             A `wil::com_ptr<U>` pointer to the given interface `U`.  The returned pointer is guaranteed not null.
    template <typename U, typename T>
    inline com_ptr<U> com_query(T&& ptrSource)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        return com_ptr<U>(raw, details::tag_com_query());
    }
#endif

    //! Queries for the specified interface and returns a fail-fast-based wil::com_ptr_failfast to that interface (fail-fast if unsupported).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), should not be null
    //! @tparam U           Represents the interface being queried
    //! @return             A `wil::com_ptr<U>` pointer to the given interface `U`.  The returned pointer is guaranteed not null.
    template <typename U, typename T>
    inline com_ptr_failfast<U> com_query_failfast(T&& ptrSource)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        return com_ptr_failfast<U>(raw, details::tag_com_query());
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    //! Queries for the interface specified by the type of the output parameter (throws an exception if unsupported).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), should not be null
    //! @param ptrResult    Represents the output pointer to populate.  The returned pointer is guaranteed not null.
    template <typename U, typename T>
    _Success_true_ void com_query_to(T&& ptrSource, _COM_Outptr_ U** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        THROW_IF_FAILED(details::query_policy_t<decltype(raw)>::query(raw, ptrResult));
        __analysis_assume(*ptrResult != nullptr);
    }
#endif

    //! Queries for the interface specified by the type of the output parameter (fail-fast if unsupported).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), should not be null
    //! @param ptrResult    Represents the output pointer to populate.  The returned pointer is guaranteed not null.
    template <typename U, typename T>
    _Success_true_ void com_query_to_failfast(T&& ptrSource, _COM_Outptr_ U** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        FAIL_FAST_IF_FAILED(details::query_policy_t<decltype(raw)>::query(raw, ptrResult));
        __analysis_assume(*ptrResult != nullptr);
    }

    //! Queries for the interface specified by the type of the output parameter (returns an error if unsupported).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), should not be null
    //! @param ptrResult    Represents the output pointer to populate.  The returned pointer will be null upon failure.
    //! @return             Returns an HRESULT representing whether the query succeeded.
    template <typename U, typename T>
    HRESULT com_query_to_nothrow(T&& ptrSource, _COM_Outptr_ U** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        auto hr = details::query_policy_t<decltype(raw)>::query(raw, ptrResult);
        __analysis_assume(SUCCEEDED(hr) || (*ptrResult == nullptr));
        return hr;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    //! Queries for the interface specified by the given REFIID parameter (throws an exception if unsupported).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), should not be null
    //! @param riid         The interface to query for
    //! @param ptrResult    Represents the output pointer to populate.  The returned pointer is guaranteed not null.
    template <typename T>
    _Success_true_ void com_query_to(T&& ptrSource, REFIID riid, _COM_Outptr_ void** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        THROW_IF_FAILED(details::query_policy_t<decltype(raw)>::query(raw, riid, ptrResult));
        __analysis_assume(*ptrResult != nullptr);
    }
#endif

    //! Queries for the interface specified by the given REFIID parameter (fail-fast if unsupported).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), should not be null
    //! @param riid         The interface to query for
    //! @param ptrResult    Represents the output pointer to populate.  The returned pointer is guaranteed not null.
    template <typename T>
    _Success_true_ void com_query_to_failfast(T&& ptrSource, REFIID riid, _COM_Outptr_ void** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        FAIL_FAST_IF_FAILED(details::query_policy_t<decltype(raw)>::query(raw, riid, ptrResult));
        __analysis_assume(*ptrResult != nullptr);
    }

    //! Queries for the interface specified by the given REFIID parameter (returns an error if unsupported).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), should not be null
    //! @param riid         The interface to query for
    //! @param ptrResult    Represents the output pointer to populate.  The returned pointer will be null upon failure.
    template <typename T>
    HRESULT com_query_to_nothrow(T&& ptrSource, REFIID riid, _COM_Outptr_ void** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        auto hr = details::query_policy_t<decltype(raw)>::query(raw, riid, ptrResult);
        __analysis_assume(SUCCEEDED(hr) || (*ptrResult == nullptr));
        return hr;
    }
    //! @}

    //! @name Stand-alone try query helpers
    //! * Source pointer can be raw interface pointer, any wil com_ptr, or WRL ComPtr
    //! * Attempts to retrieves the requested interface
    //! * AV if the source pointer is null
    //! * Produce null if the requested interface is unsupported
    //! * bool returns 'true' when query was successful (non-null return result)
    //!
    //! See @ref page_query for more information.
    //! @{

#ifdef WIL_ENABLE_EXCEPTIONS
    //! Attempts a query for the specified interface and returns an exception-based wil::com_ptr to that interface (returns null if unsupported).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), should not be null
    //! @tparam U           Represents the interface being queried
    //! @return             A `wil::com_ptr<U>` pointer to the given interface `U`.  The returned pointer is null if the requested interface was not supported.
    template <class U, typename T>
    inline com_ptr<U> try_com_query(T&& ptrSource)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        return com_ptr<U>(raw, details::tag_try_com_query());
    }
#endif

    //! Attempts a query for the specified interface and returns an fail-fast wil::com_ptr_failfast to that interface (returns null if unsupported).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), should not be null
    //! @tparam U           Represents the interface being queried
    //! @return             A `wil::com_ptr_failfast<U>` pointer to the given interface `U`.  The returned pointer is null if the requested interface was not supported.
    template <class U, typename T>
    inline com_ptr_failfast<U> try_com_query_failfast(T&& ptrSource)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        return com_ptr_failfast<U>(raw, details::tag_try_com_query());
    }

    //! Attempts a query for the specified interface and returns an error-code-based wil::com_ptr_nothrow to that interface (returns null if unsupported).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), should not be null
    //! @tparam U           Represents the interface being queried
    //! @return             A `wil::com_ptr_nothrow<U>` pointer to the given interface `U`.  The returned pointer is null if the requested interface was not supported.
    template <class U, typename T>
    inline com_ptr_nothrow<U> try_com_query_nothrow(T&& ptrSource)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        return com_ptr_nothrow<U>(raw, details::tag_try_com_query());
    }

    //! Attempts a query for the interface specified by the type of the output parameter (returns `false` if unsupported).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), should not be null.
    //! @param ptrResult    Represents the output pointer to populate.  If the interface is unsupported, the returned pointer will be null.
    //! @return             A bool value representing whether the query was successful (non-null return result).
    template <typename U, typename T>
    _Success_return_ bool try_com_query_to(T&& ptrSource, _COM_Outptr_ U** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        return (SUCCEEDED(details::query_policy_t<decltype(raw)>::query(raw, ptrResult)));
    }

    //! Attempts a query for the interface specified by the type of the output parameter (returns `false` if unsupported).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), should not be null.
    //! @param riid         The interface to query for
    //! @param ptrResult    Represents the output pointer to populate.  If the interface is unsupported, the returned pointer will be null.
    //! @return             A bool value representing whether the query was successful (non-null return result).
    template <typename T>
    _Success_return_ bool try_com_query_to(T&& ptrSource, REFIID riid, _COM_Outptr_ void** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        return (SUCCEEDED(details::query_policy_t<decltype(raw)>::query(raw, riid, ptrResult)));
    }
    //! @}


    //! @name Stand-alone copy helpers
    //! * Source pointer can be raw interface pointer, any wil com_ptr, or WRL ComPtr
    //! * Retrieves the requested interface
    //! * Succeeds with null if the source pointer is null
    //! * Produce an error if the requested interface is unsupported
    //!
    //! See @ref page_query for more information
    //! @{

#ifdef WIL_ENABLE_EXCEPTIONS
    //! Queries for the specified interface and returns an exception-based wil::com_ptr to that interface (exception if unsupported, preserves null).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), may be null
    //! @tparam U           Represents the interface being queried
    //! @return             A `wil::com_ptr<U>` pointer to the given interface `U`.  The returned pointer will be null only if the source is null.
    template <class U, typename T>
    inline com_ptr<U> com_copy(T&& ptrSource)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        return com_ptr<U>(raw, details::tag_com_copy());
    }
#endif

    //! Queries for the specified interface and returns a fail-fast-based wil::com_ptr_failfast to that interface (fail-fast if unsupported, preserves null).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), may be null
    //! @tparam U           Represents the interface being queried
    //! @return             A `wil::com_ptr<U>` pointer to the given interface `U`.  The returned pointer will be null only if the source is null.
    template <class U, typename T>
    inline com_ptr_failfast<U> com_copy_failfast(T&& ptrSource)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        return com_ptr_failfast<U>(raw, details::tag_com_copy());
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    //! Queries for the interface specified by the type of the output parameter (throws an exception if unsupported, preserves null).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), may be null
    //! @param ptrResult    Represents the output pointer to populate.  The returned pointer will be null only if the source is null.
    template <typename U, typename T>
    _Success_true_ void com_copy_to(T&& ptrSource, _COM_Outptr_result_maybenull_ U** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        if (raw)
        {
            THROW_IF_FAILED(details::query_policy_t<decltype(raw)>::query(raw, ptrResult));
            return;
        }
        *ptrResult = nullptr;
    }
#endif

    //! Queries for the interface specified by the type of the output parameter (fail-fast if unsupported, preserves null).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), may be null
    //! @param ptrResult    Represents the output pointer to populate.  The returned pointer will be null only if the source is null.
    template <typename U, typename T>
    _Success_true_ void com_copy_to_failfast(T&& ptrSource, _COM_Outptr_result_maybenull_ U** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        if (raw)
        {
            FAIL_FAST_IF_FAILED(details::query_policy_t<decltype(raw)>::query(raw, ptrResult));
            return;
        }
        *ptrResult = nullptr;
    }

    //! Queries for the interface specified by the type of the output parameter (returns an error if unsupported, preserves null).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), may be null
    //! @param ptrResult    Represents the output pointer to populate.  The returned pointer will be null upon failure or if the source is null.
    //! @return             Returns an HRESULT representing whether the query succeeded (returns S_OK if the source is null).
    template <typename U, typename T>
    HRESULT com_copy_to_nothrow(T&& ptrSource, _COM_Outptr_result_maybenull_ U** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        if (raw)
        {
            RETURN_HR(details::query_policy_t<decltype(raw)>::query(raw, ptrResult));
        }
        *ptrResult = nullptr;
        return S_OK;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    //! Queries for the interface specified by the given REFIID parameter (throws an exception if unsupported, preserves null).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), may be null
    //! @param riid         The interface to query for
    //! @param ptrResult    Represents the output pointer to populate.  The returned pointer will be null only if the source is null.
    template <typename T>
    _Success_true_ void com_copy_to(T&& ptrSource, REFIID riid, _COM_Outptr_result_maybenull_ void** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        if (raw)
        {
            THROW_IF_FAILED(details::query_policy_t<decltype(raw)>::query(raw, riid, ptrResult));
            return;
        }
        *ptrResult = nullptr;
    }
#endif

    //! Queries for the interface specified by the given REFIID parameter (fail-fast if unsupported, preserves null).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), may be null
    //! @param riid         The interface to query for
    //! @param ptrResult    Represents the output pointer to populate.  The returned pointer will be null only if the source is null.
    template <typename T>
    _Success_true_ void com_copy_to_failfast(T&& ptrSource, REFIID riid, _COM_Outptr_result_maybenull_ void** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        if (raw)
        {
            FAIL_FAST_IF_FAILED(details::query_policy_t<decltype(raw)>::query(raw, riid, ptrResult));
            return;
        }
        *ptrResult = nullptr;
    }

    //! Queries for the interface specified by the given REFIID parameter (returns an error if unsupported, preserves null).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), may be null
    //! @param riid         The interface to query for
    //! @param ptrResult    Represents the output pointer to populate.  The returned pointer will be null upon failure or if the source is null.
    //! @return             Returns an HRESULT representing whether the query succeeded (returns S_OK if the source is null).
    template <typename T>
    HRESULT com_copy_to_nothrow(T&& ptrSource, REFIID riid, _COM_Outptr_result_maybenull_ void** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        if (raw)
        {
            RETURN_HR(details::query_policy_t<decltype(raw)>::query(raw, riid, ptrResult));
        }
        *ptrResult = nullptr;
        return S_OK;
    }
    //! @}


    //! @name Stand-alone try copy helpers
    //! * Source pointer can be raw interface pointer, any wil com_ptr, or WRL ComPtr
    //! * Attempts to retrieves the requested interface
    //! * Succeeds with null if the source pointer is null
    //! * Produce null if the requested interface is unsupported
    //! * bool returns 'true' when query was successful (non-null return result)
    //!
    //! See @ref page_query for more information.
    //! @{

#ifdef WIL_ENABLE_EXCEPTIONS
    //! Attempts a query for the specified interface and returns an exception-based wil::com_ptr to that interface (returns null if unsupported, preserves null).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), may be null
    //! @tparam U           Represents the interface being queried
    //! @return             A `wil::com_ptr<U>` pointer to the given interface `U`.  The returned pointer is null if the requested interface was not supported.
    template <class U, typename T>
    inline com_ptr<U> try_com_copy(T&& ptrSource)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        return com_ptr<U>(raw, details::tag_try_com_copy());
    }
#endif

    //! Attempts a query for the specified interface and returns an fail-fast wil::com_ptr_failfast to that interface (returns null if unsupported, preserves null).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), may be null
    //! @tparam U           Represents the interface being queried
    //! @return             A `wil::com_ptr_failfast<U>` pointer to the given interface `U`.  The returned pointer is null if the requested interface was not supported.
    template <class U, typename T>
    inline com_ptr_failfast<U> try_com_copy_failfast(T&& ptrSource)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        return com_ptr_failfast<U>(raw, details::tag_try_com_copy());
    }

    //! Attempts a query for the specified interface and returns an error-code-based wil::com_ptr_nothrow to that interface (returns null if unsupported, preserves null).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), may be null
    //! @tparam U           Represents the interface being queried
    //! @return             A `wil::com_ptr_nothrow<U>` pointer to the given interface `U`.  The returned pointer is null if the requested interface was not supported.
    template <class U, typename T>
    inline com_ptr_nothrow<U> try_com_copy_nothrow(T&& ptrSource)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        return com_ptr_nothrow<U>(raw, details::tag_try_com_copy());
    }

    //! Attempts a query for the interface specified by the type of the output parameter (returns `false` if unsupported, preserves null).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), may be null.
    //! @param ptrResult    Represents the output pointer to populate.  If the interface is unsupported, the returned pointer will be null.
    //! @return             A bool value representing whether the query was successful (non-null return result).
    template <typename U, typename T>
    _Success_return_ bool try_com_copy_to(T&& ptrSource, _COM_Outptr_result_maybenull_ U** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        if (raw)
        {
            return SUCCEEDED(details::query_policy_t<decltype(raw)>::query(raw, ptrResult));
        }
        *ptrResult = nullptr;
        return false;
    }

    //! Attempts a query for the interface specified by the type of the output parameter (returns `false` if unsupported, preserves null).
    //! See @ref page_query for more information.
    //! @param ptrSource    The pointer to query (may be a raw interface pointer, wil com_ptr, or WRL ComPtr), may be null.
    //! @param riid         The interface to query for
    //! @param ptrResult    Represents the output pointer to populate.  If the interface is unsupported, the returned pointer will be null.
    //! @return             A bool value representing whether the query was successful (non-null return result).
    template <typename T>
    _Success_return_ bool try_com_copy_to(T&& ptrSource, REFIID riid, _COM_Outptr_result_maybenull_ void** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        if (raw)
        {
            return SUCCEEDED(details::query_policy_t<decltype(raw)>::query(raw, riid, ptrResult));
        }
        *ptrResult = nullptr;
        return false;
    }
    //! @}

#ifdef __cplusplus_winrt
    //! @name Stand-alone helpers to query for CX ref ("hat") types from ABI COM types.
    //! * Source pointer can be raw interface pointer, any wil com_ptr, or WRL ComPtr
    //! * Retrieves the requested C++/CX interface or ref class.
    //! * Preserves null if the source pointer is null
    //! * Produce an error if the requested interface is unsupported
    //!
    //! See @ref page_query for more information
    //! @{

    template <typename T>
    ::Platform::Object^ cx_object_from_abi(T&& ptr) WI_NOEXCEPT
    {
        IInspectable* const inspectable = com_raw_ptr(wistd::forward<T>(ptr));
        return reinterpret_cast<::Platform::Object^>(inspectable);
    }

    template <typename U, typename T>
    inline U^ cx_safe_cast(T&& ptrSource)
    {
        return safe_cast<U^>(cx_object_from_abi(wistd::forward<T>(ptrSource)));
    }

    template <typename U, typename T>
    inline U^ cx_dynamic_cast(T&& ptrSource) WI_NOEXCEPT
    {
        return dynamic_cast<U^>(cx_object_from_abi(wistd::forward<T>(ptrSource)));
    }
    //! @}
#endif


    //*****************************************************************************
    // Agile References
    //*****************************************************************************

#if (NTDDI_VERSION >= NTDDI_WINBLUE)
#ifdef WIL_ENABLE_EXCEPTIONS
    //! Agile reference to a COM interface, errors throw exceptions (see @ref com_ptr_t and @ref com_agile_query for details)
    using com_agile_ref = com_ptr<IAgileReference>;
#endif
    //! Agile reference to a COM interface, errors return error codes (see @ref com_ptr_t and @ref com_agile_query_nothrow for details)
    using com_agile_ref_nothrow = com_ptr_nothrow<IAgileReference>;
    //! Agile reference to a COM interface, errors fail fast (see @ref com_ptr_t and @ref com_agile_query_failfast for details)
    using com_agile_ref_failfast = com_ptr_failfast<IAgileReference>;

    //! @name Create agile reference helpers
    //! * Attempts to retrieve an agile reference to the requested interface (see [RoGetAgileReference](https://msdn.microsoft.com/en-us/library/dn269839.aspx))
    //! * Source pointer can be raw interface pointer, any wil com_ptr, or WRL ComPtr
    //! * `query` methods AV if the source pointer is null
    //! * `copy` methods succeed with null if the source pointer is null
    //! * Accept optional [AgileReferenceOptions](https://msdn.microsoft.com/en-us/library/dn269836.aspx)
    //!
    //! See @ref page_query for more information on resolving an agile ref
    //! @{

#ifdef WIL_ENABLE_EXCEPTIONS
    //! return a com_agile_ref representing the given source pointer (throws an exception on failure)
    template <typename T>
    com_agile_ref com_agile_query(T&& ptrSource, AgileReferenceOptions options = AGILEREFERENCE_DEFAULT)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        com_agile_ref agileRef;
        THROW_IF_FAILED(::RoGetAgileReference(options, __uuidof(raw), raw, &agileRef));
        return agileRef;
    }
#endif

    //! return a com_agile_ref_failfast representing the given source pointer (fail-fast on failure)
    template <typename T>
    com_agile_ref_failfast com_agile_query_failfast(T&& ptrSource, AgileReferenceOptions options = AGILEREFERENCE_DEFAULT)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        com_agile_ref_failfast agileRef;
        FAIL_FAST_IF_FAILED(::RoGetAgileReference(options, __uuidof(raw), raw, &agileRef));
        return agileRef;
    }

    //! return a com_agile_ref_nothrow representing the given source pointer (returns an HRESULT on failure)
    template <typename T>
    HRESULT com_agile_query_nothrow(T&& ptrSource, _COM_Outptr_ IAgileReference** ptrResult, AgileReferenceOptions options = AGILEREFERENCE_DEFAULT)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        auto hr = ::RoGetAgileReference(options, __uuidof(raw), raw, ptrResult);
        __analysis_assume(SUCCEEDED(hr) || (*ptrResult == nullptr));
        return hr;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    //! return a com_agile_ref representing the given source pointer (throws an exception on failure, source maybe null)
    template <typename T>
    com_agile_ref com_agile_copy(T&& ptrSource, AgileReferenceOptions options = AGILEREFERENCE_DEFAULT)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        com_agile_ref agileRef;
        if (raw)
        {
            THROW_IF_FAILED(::RoGetAgileReference(options, __uuidof(raw), raw, &agileRef));
        }
        return agileRef;
    }
#endif

    //! return a com_agile_ref_failfast representing the given source pointer (fail-fast on failure, source maybe null)
    template <typename T>
    com_agile_ref_failfast com_agile_copy_failfast(T&& ptrSource, AgileReferenceOptions options = AGILEREFERENCE_DEFAULT)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        com_agile_ref_failfast agileRef;
        if (raw)
        {
            FAIL_FAST_IF_FAILED(::RoGetAgileReference(options, __uuidof(raw), raw, &agileRef));
        }
        return agileRef;
    }

    //! return an agile ref (com_agile_ref_XXX or other representation) representing the given source pointer (return error on failure, source maybe null)
    template <typename T>
    HRESULT com_agile_copy_nothrow(T&& ptrSource, _COM_Outptr_result_maybenull_ IAgileReference** ptrResult, AgileReferenceOptions options = AGILEREFERENCE_DEFAULT)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        if (raw)
        {
            RETURN_HR(::RoGetAgileReference(options, __uuidof(raw), raw, ptrResult));
        }
        *ptrResult = nullptr;
        return S_OK;
    }
    //! @}
#endif

    //*****************************************************************************
    // Weak References
    //*****************************************************************************

    namespace details
    {
        template <typename T>
        HRESULT GetWeakReference(T* ptr, _COM_Outptr_ IWeakReference** weakReference)
        {
            static_assert(!wistd::is_same<IWeakReference, T>::value, "Cannot get an IWeakReference to an IWeakReference");

            *weakReference = nullptr;
            com_ptr_nothrow<IWeakReferenceSource> source;
            HRESULT hr = ptr->QueryInterface(IID_PPV_ARGS(&source));
            if (SUCCEEDED(hr))
            {
                hr = source->GetWeakReference(weakReference);
            }
            return hr;
        }
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    //! Weak reference to a COM interface, errors throw exceptions (see @ref com_ptr_t and @ref com_weak_query for details)
    using com_weak_ref = com_ptr<IWeakReference>;
#endif
    //! Weak reference to a COM interface, errors return error codes (see @ref com_ptr_t and @ref com_weak_query_nothrow for details)
    using com_weak_ref_nothrow = com_ptr_nothrow<IWeakReference>;
    //! Weak reference to a COM interface, errors fail fast (see @ref com_ptr_t and @ref com_weak_query_failfast for details)
    using com_weak_ref_failfast = com_ptr_failfast<IWeakReference>;

    //! @name Create weak reference helpers
    //! * Attempts to retrieve a weak reference to the requested interface (see WRL's similar [WeakRef](https://msdn.microsoft.com/en-us/library/br244853.aspx))
    //! * Source pointer can be raw interface pointer, any wil com_ptr, or WRL ComPtr
    //! * `query` methods AV if the source pointer is null
    //! * `copy` methods succeed with null if the source pointer is null
    //!
    //! See @ref page_query for more information on resolving a weak ref
    //! @{

#ifdef WIL_ENABLE_EXCEPTIONS
    //! return a com_weak_ref representing the given source pointer (throws an exception on failure)
    template <typename T>
    com_weak_ref com_weak_query(T&& ptrSource)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        com_weak_ref weakRef;
        THROW_IF_FAILED(details::GetWeakReference(raw, &weakRef));
        return weakRef;
    }
#endif

    //! return a com_weak_ref_failfast representing the given source pointer (fail-fast on failure)
    template <typename T>
    com_weak_ref_failfast com_weak_query_failfast(T&& ptrSource)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        com_weak_ref_failfast weakRef;
        FAIL_FAST_IF_FAILED(details::GetWeakReference(raw, &weakRef));
        return weakRef;
    }

    //! return a com_weak_ref_nothrow representing the given source pointer (returns an HRESULT on failure)
    template <typename T>
    HRESULT com_weak_query_nothrow(T&& ptrSource, _COM_Outptr_ IWeakReference** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        auto hr = details::GetWeakReference(raw, ptrResult);
        __analysis_assume(SUCCEEDED(hr) || (*ptrResult == nullptr));
        return hr;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    //! return a com_weak_ref representing the given source pointer (throws an exception on failure, source maybe null)
    template <typename T>
    com_weak_ref com_weak_copy(T&& ptrSource)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        com_weak_ref weakRef;
        if (raw)
        {
            THROW_IF_FAILED(details::GetWeakReference(raw, &weakRef));
        }
        return weakRef;
    }
#endif

    //! return a com_weak_ref_failfast representing the given source pointer (fail-fast on failure, source maybe null)
    template <typename T>
    com_weak_ref_failfast com_weak_copy_failfast(T&& ptrSource)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        com_weak_ref_failfast weakRef;
        if (raw)
        {
            FAIL_FAST_IF_FAILED(details::GetWeakReference(raw, &weakRef));
        }
        return weakRef;
    }

    //! return a com_weak_ref_failfast representing the given source pointer (fail-fast on failure, source maybe null)
    template <typename T>
    HRESULT com_weak_copy_nothrow(T&& ptrSource, _COM_Outptr_result_maybenull_ IWeakReference** ptrResult)
    {
        auto raw = com_raw_ptr(wistd::forward<T>(ptrSource));
        if (raw)
        {
            RETURN_HR(details::GetWeakReference(raw, ptrResult));
        }
        *ptrResult = nullptr;
        return S_OK;
    }

#pragma region COM Object Helpers

    template <typename T>
    inline bool is_agile(T&& ptrSource)
    {
        wil::com_ptr_nothrow<IAgileObject> agileObject;
        return SUCCEEDED(com_raw_ptr(wistd::forward<T>(ptrSource))->QueryInterface(IID_PPV_ARGS(&agileObject)));
    }

    /** constructs a COM object using an CLSID on a specific interface or IUnknown.*/
    template<typename Interface = IUnknown, typename error_policy = err_exception_policy>
    wil::com_ptr_t<Interface, error_policy> CoCreateInstance(REFCLSID rclsid, DWORD dwClsContext = CLSCTX_INPROC_SERVER)
    {
        wil::com_ptr_t<Interface, error_policy> result;
        error_policy::HResult(::CoCreateInstance(rclsid, nullptr, dwClsContext, IID_PPV_ARGS(&result)));
        return result;
    }

    /** constructs a COM object using the class as the identifier (that has an associated CLSID) on a specific interface or IUnknown. */
    template<typename Class, typename Interface = IUnknown, typename error_policy = err_exception_policy>
    wil::com_ptr_t<Interface, error_policy> CoCreateInstance(DWORD dwClsContext = CLSCTX_INPROC_SERVER)
    {
        return CoCreateInstance<Interface, error_policy>(__uuidof(Class), dwClsContext);
    }

    /** constructs a COM object using an CLSID on a specific interface or IUnknown. */
    template<typename Interface = IUnknown>
    wil::com_ptr_failfast<Interface> CoCreateInstanceFailFast(REFCLSID rclsid, DWORD dwClsContext = CLSCTX_INPROC_SERVER) WI_NOEXCEPT
    {
        return CoCreateInstance<Interface, err_failfast_policy>(rclsid, dwClsContext);
    }

    /** constructs a COM object using the class as the identifier (that has an associated CLSID) on a specific interface or IUnknown. */
    template<typename Class, typename Interface = IUnknown>
    wil::com_ptr_failfast<Interface> CoCreateInstanceFailFast(DWORD dwClsContext = CLSCTX_INPROC_SERVER) WI_NOEXCEPT
    {
        return CoCreateInstanceFailFast<Interface>(__uuidof(Class), dwClsContext);
    }

    /** constructs a COM object using an CLSID on a specific interface or IUnknown.
    Note, failures are reported as a null result, the HRESULT is lost. */
    template<typename Interface = IUnknown>
    wil::com_ptr_nothrow<Interface> CoCreateInstanceNoThrow(REFCLSID rclsid, DWORD dwClsContext = CLSCTX_INPROC_SERVER) WI_NOEXCEPT
    {
        return CoCreateInstance<Interface, err_returncode_policy>(rclsid, dwClsContext);
    }

    /** constructs a COM object using the class as the identifier (that has an associated CLSID) on a specific interface or IUnknown.
    Note, failures are reported as a null result, the HRESULT is lost. */
    template<typename Class, typename Interface = IUnknown>
    wil::com_ptr_nothrow<Interface> CoCreateInstanceNoThrow(DWORD dwClsContext = CLSCTX_INPROC_SERVER) WI_NOEXCEPT
    {
        return CoCreateInstanceNoThrow<Interface>(__uuidof(Class), dwClsContext);
    }

    /** constructs a COM object class factory using an CLSID on IClassFactory or a specific interface. */
    template<typename Interface = IClassFactory, typename error_policy = err_exception_policy>
    wil::com_ptr_t<Interface, error_policy> CoGetClassObject(REFCLSID rclsid, DWORD dwClsContext = CLSCTX_INPROC_SERVER)
    {
        wil::com_ptr_t<Interface, error_policy> result;
        error_policy::HResult(CoGetClassObject(rclsid, dwClsContext, nullptr, IID_PPV_ARGS(&result)));
        return result;
    }

    /** constructs a COM object class factory using the class as the identifier (that has an associated CLSID)
    on IClassFactory or a specific interface. */
    template<typename Class, typename Interface = IClassFactory, typename error_policy = err_exception_policy>
    wil::com_ptr_t<Interface, error_policy> CoGetClassObject(DWORD dwClsContext = CLSCTX_INPROC_SERVER)
    {
        return CoGetClassObject<Interface, error_policy>(__uuidof(Class), dwClsContext);
    }

    /** constructs a COM object class factory using an CLSID on IClassFactory or a specific interface. */
    template<typename Interface = IClassFactory>
    wil::com_ptr_failfast<Interface> CoGetClassObjectFailFast(REFCLSID rclsid, DWORD dwClsContext = CLSCTX_INPROC_SERVER)
    {
        return CoGetClassObject<Interface, err_failfast_policy>(rclsid, dwClsContext);
    }

    /** constructs a COM object class factory using the class as the identifier (that has an associated CLSID)
    on IClassFactory or a specific interface. */
    template<typename Class, typename Interface = IClassFactory>
    wil::com_ptr_failfast<Interface> CoGetClassObjectFailFast(DWORD dwClsContext = CLSCTX_INPROC_SERVER)
    {
        return CoGetClassObjectFailFast<Interface>(__uuidof(Class), dwClsContext);
    }

    /** constructs a COM object class factory using an CLSID on IClassFactory or a specific interface.
    Note, failures are reported as a null result, the HRESULT is lost. */
    template<typename Interface = IClassFactory>
    wil::com_ptr_nothrow<Interface> CoGetClassObjectNoThrow(REFCLSID rclsid, DWORD dwClsContext = CLSCTX_INPROC_SERVER)
    {
        return CoGetClassObject<Interface, err_returncode_policy>(rclsid, dwClsContext);
    }

    /** constructs a COM object class factory using the class as the identifier (that has an associated CLSID)
    on IClassFactory or a specific interface.
    Note, failures are reported as a null result, the HRESULT is lost. */
    template<typename Class, typename Interface = IClassFactory>
    wil::com_ptr_nothrow<Interface> CoGetClassObjectNoThrow(DWORD dwClsContext = CLSCTX_INPROC_SERVER)
    {
        return CoGetClassObjectNoThrow<Interface>(__uuidof(Class), dwClsContext);
    }

#if __cpp_lib_apply && __has_include(<type_traits>)
    namespace details
    {
        template <typename error_policy, typename... Results>
        auto CoCreateInstanceEx(REFCLSID clsid, CLSCTX clsCtx) noexcept
        {
            MULTI_QI multiQis[sizeof...(Results)]{};
            const IID* iids[sizeof...(Results)]{ &__uuidof(Results)... };

            static_assert(sizeof...(Results) > 0);

            for (auto i = 0U; i < sizeof...(Results); ++i)
            {
                multiQis[i].pIID = iids[i];
            }

            const auto hr = CoCreateInstanceEx(clsid, nullptr, clsCtx, nullptr,
                ARRAYSIZE(multiQis), multiQis);

            std::tuple<wil::com_ptr_t<Results, error_policy>...> resultTuple;

            std::apply([i = 0, &multiQis](auto&... a) mutable
            {
                (a.attach(reinterpret_cast<typename std::remove_reference<decltype(a)>::type::pointer>(multiQis[i++].pItf)), ...);
            }, resultTuple);
            return std::tuple<HRESULT, decltype(resultTuple)>(hr, std::move(resultTuple));
        }

        template<typename error_policy, typename... Results>
        auto com_multi_query(IUnknown* obj)
        {
            MULTI_QI multiQis[sizeof...(Results)]{};
            const IID* iids[sizeof...(Results)]{ &__uuidof(Results)... };

            static_assert(sizeof...(Results) > 0);

            for (auto i = 0U; i < sizeof...(Results); ++i)
            {
                multiQis[i].pIID = iids[i];
            }

            std::tuple<wil::com_ptr_t<Results, error_policy>...> resultTuple{};

            wil::com_ptr_nothrow<IMultiQI> multiQi;
            auto hr = obj->QueryInterface(IID_PPV_ARGS(&multiQi));
            if (SUCCEEDED(hr))
            {
                hr = multiQi->QueryMultipleInterfaces(ARRAYSIZE(multiQis), multiQis);
                std::apply([i = 0, &multiQis](auto&... a) mutable
                {
                    (a.attach(reinterpret_cast<typename std::remove_reference<decltype(a)>::type::pointer>(multiQis[i++].pItf)), ...);
                }, resultTuple);
            }
            return std::tuple<HRESULT, decltype(resultTuple)>{hr, std::move(resultTuple)};
        }
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    // CoCreateInstanceEx can be used to improve performance by requesting multiple interfaces
    // from an object at create time. This is most useful for out of process (OOP) servers, saving
    // and RPC per extra interface requested.
    template <typename... Results>
    auto CoCreateInstanceEx(REFCLSID clsid, CLSCTX clsCtx = CLSCTX_LOCAL_SERVER)
    {
        auto [error, result] = details::CoCreateInstanceEx<err_exception_policy, Results...>(clsid, clsCtx);
        THROW_IF_FAILED(error);
        THROW_HR_IF(E_NOINTERFACE, error == CO_S_NOTALLINTERFACES);
        return result;
    }

    template <typename... Results>
    auto TryCoCreateInstanceEx(REFCLSID clsid, CLSCTX clsCtx = CLSCTX_LOCAL_SERVER)
    {
        auto [error, result] = details::CoCreateInstanceEx<err_exception_policy, Results...>(clsid, clsCtx);
        return result;
    }
#endif

    // Returns [error, result] where result is a tuple with each of the requested interfaces.
    template <typename... Results>
    auto CoCreateInstanceExNoThrow(REFCLSID clsid, CLSCTX clsCtx = CLSCTX_LOCAL_SERVER) noexcept
    {
        auto [error, result] = details::CoCreateInstanceEx<err_returncode_policy, Results...>(clsid, clsCtx);
        if (SUCCEEDED(error) && (error == CO_S_NOTALLINTERFACES))
        {
            return std::tuple<HRESULT, decltype(result)>{E_NOINTERFACE, {}};
        }
        return std::tuple<HRESULT, decltype(result)>{error, result};
    }

    template <typename... Results>
    auto TryCoCreateInstanceExNoThrow(REFCLSID clsid, CLSCTX clsCtx = CLSCTX_LOCAL_SERVER) noexcept
    {
        auto [error, result] = details::CoCreateInstanceEx<err_returncode_policy, Results...>(clsid, clsCtx);
        return result;
    }

    template <typename... Results>
    auto CoCreateInstanceExFailFast(REFCLSID clsid, CLSCTX clsCtx = CLSCTX_LOCAL_SERVER) noexcept
    {
        auto [error, result] = details::CoCreateInstanceEx<err_failfast_policy, Results...>(clsid, clsCtx);
        FAIL_FAST_IF_FAILED(error);
        FAIL_FAST_HR_IF(E_NOINTERFACE, error == CO_S_NOTALLINTERFACES);
        return result;
    }

    template <typename... Results>
    auto TryCoCreateInstanceExFailFast(REFCLSID clsid, CLSCTX clsCtx = CLSCTX_LOCAL_SERVER) noexcept
    {
        auto [error, result] = details::CoCreateInstanceEx<err_failfast_policy, Results...>(clsid, clsCtx);
        return result;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    template<typename... Results>
    auto com_multi_query(IUnknown* obj)
    {
        auto [error, result] = details::com_multi_query<err_exception_policy, Results...>(obj);
        THROW_IF_FAILED(error);
        THROW_HR_IF(E_NOINTERFACE, error == S_FALSE);
        return result;
    }

    template<typename... Results>
    auto try_com_multi_query(IUnknown* obj)
    {
        auto [error, result] = details::com_multi_query<err_exception_policy, Results...>(obj);
        return result;
    }
#endif

#endif // __cpp_lib_apply && __has_include(<type_traits>)

#pragma endregion

#pragma region Stream helpers

    /** Read data from a stream into a buffer.
    Reads up to a certain number of bytes into a buffer. Returns the amount of data written, which
    may be less than the amount requested if the stream ran out.
    ~~~~
    IStream* source = // ...
    ULONG dataBlob = 0;
    size_t read = 0;
    RETURN_IF_FAILED(wil::stream_read_partial_nothrow(source, &dataBlob, sizeof(dataBlob), &read));
    if (read != sizeof(dataBlob))
    {
        // end of stream, probably
    }
    else if (dataBlob == 0x8675309)
    {
        DoThing(dataBlob);
    }
    ~~~~
    @param stream The stream from which to read at most `size` bytes.
    @param data A buffer into which up to `size` bytes will be read
    @param size The size, in bytes, of the buffer pointed to by `data`
    @param wrote The amount, in bytes, of data read from `stream` into `data`
    */
    inline HRESULT stream_read_partial_nothrow(_In_ ISequentialStream* stream, _Out_writes_bytes_to_(size, *wrote) void* data, unsigned long size, unsigned long *wrote)
    {
        RETURN_HR(stream->Read(data, size, wrote));
    }

    /** Read an exact number of bytes from a stream into a buffer.
    Fails if the stream didn't read all the bytes requested.
    ~~~~
    IStream* source = // ...
    ULONG dataBlob = 0;
    RETURN_IF_FAILED(wil::stream_read_nothrow(source, &dataBlob, sizeof(dataBlob)));
    if (dataBlob == 0x8675309)
    {
        DoThing(dataBlob);
    }
    ~~~~
    @param stream The stream from which to read at most `size` bytes.
    @param data A buffer into which up to `size` bytes will be read
    @param size The size, in bytes, of the buffer pointed to by `data`
    @return The underlying stream read result, or HRESULT_FROM_WIN32(ERROR_INVALID_DATA) if the stream
        did not read the complete buffer.
    */
    inline HRESULT stream_read_nothrow(_In_ ISequentialStream* stream, _Out_writes_bytes_all_(size) void* data, unsigned long size)
    {
        unsigned long didRead;
        RETURN_IF_FAILED(stream_read_partial_nothrow(stream, data, size, &didRead));
        RETURN_HR_IF(HRESULT_FROM_WIN32(ERROR_INVALID_DATA), didRead != size);

        return S_OK;
    }

    /** Read from a stream into a POD type.
    Fails if the stream didn't have enough bytes.
    ~~~~
    IStream* source = // ...
    MY_HEADER header{};
    RETURN_IF_FAILED(wil::stream_read_nothrow(source, &header));
    if (header.Version == 0x8675309)
    {
        ConsumeOldHeader(stream, header);
    }
    ~~~~
    @param stream The stream from which to read at most `size` bytes.
    @param pThing The POD data type to read from the stream.
    @return The underlying stream read result, or HRESULT_FROM_WIN32(ERROR_INVALID_DATA) if the stream
        did not read the complete buffer.
    */
    template<typename T> HRESULT stream_read_nothrow(_In_ ISequentialStream* stream, _Out_ T* pThing)
    {
        static_assert(__is_pod(T), "Type must be POD.");
        return stream_read_nothrow(stream, pThing, sizeof(T));
    }

    /** Write an exact number of bytes to a stream from a buffer.
    Fails if the stream didn't read write the bytes requested.
    ~~~~
    IStream* source = // ...
    ULONG dataBlob = 0x8675309;
    RETURN_IF_FAILED(wil::stream_write_nothrow(source, &dataBlob, sizeof(dataBlob)));
    ~~~~
    @param stream The stream to which to write at most `size` bytes.
    @param data A buffer from which up to `size` bytes will be read
    @param size The size, in bytes, of the buffer pointed to by `data`
    */
    inline HRESULT stream_write_nothrow(_In_ ISequentialStream* stream, _In_reads_bytes_(size) const void* data, unsigned long size)
    {
        unsigned long wrote;
        RETURN_IF_FAILED(stream->Write(data, size, &wrote));
        RETURN_HR_IF(HRESULT_FROM_WIN32(ERROR_INVALID_DATA), wrote != size);

        return S_OK;
    }

    /** Write a POD type to a stream.
    Fails if not all the bytes were written.
    ~~~~
    IStream* source = // ...
    MY_HEADER header { 0x8675309, HEADER_FLAG_1 | HEADER_FLAG_2 };
    RETURN_IF_FAILED(wil::stream_write_nothrow(source, header));

    ULONGLONG value = 16;
    RETURN_IF_FAILED(wil::stream_write_nothrow(source, value));
    ~~~~
    @param stream The stream to which to write `thing`
    @param thing The POD data type to write to the stream.
    */
    template<typename T> inline HRESULT stream_write_nothrow(_In_ ISequentialStream* stream, const T& thing)
    {
        return stream_write_nothrow(stream, wistd::addressof(thing), sizeof(thing));
    }

    /** Retrieve the size of this stream, in bytes
    ~~~~
    IStream* source = // ...
    ULONGLONG size;
    RETURN_IF_FAILED(wil::stream_size_nothrow(source, &size));
    RETURN_HR_IF(E_INVALIDARG, size > ULONG_MAX);
    ~~~~
    @param stream The stream whose size is to be returned in `value`
    @param value The size, in bytes, reported by `stream`
    */
    inline HRESULT stream_size_nothrow(_In_ IStream* stream, _Out_ unsigned long long* value)
    {
        STATSTG st{};
        RETURN_IF_FAILED(stream->Stat(&st, STATFLAG_NONAME));
        *value = st.cbSize.QuadPart;

        return S_OK;
    }

    /** Seek a stream to a relative offset or absolute position
    ~~~~
    IStream* source = // ...
    unsigned long long landed;
    RETURN_IF_FAILED(wil::stream_seek_nothrow(source, 16, STREAM_SEEK_CUR, &landed));
    RETURN_IF_FAILED(wil::stream_seek_nothrow(source, -5, STREAM_SEEK_END));
    RETURN_IF_FAILED(wil::stream_seek_nothrow(source, LLONG_MAX, STREAM_SEEK_CUR));
    ~~~~
    @param stream The stream to seek
    @param offset The position, in bytes from the current position, to seek
    @param from The starting point from which to seek, from the STREAM_SEEK_* set of values
    @param value Optionally recieves the new absolute position from the stream
    */
    inline HRESULT stream_seek_nothrow(_In_ IStream* stream, long long offset, unsigned long from, _Out_opt_ unsigned long long* value = nullptr)
    {
        LARGE_INTEGER amount{};
        ULARGE_INTEGER landed{};
        amount.QuadPart = offset;
        RETURN_IF_FAILED(stream->Seek(amount, from, value ? &landed : nullptr));
        assign_to_opt_param(value, landed.QuadPart);

        return S_OK;
    }

    /** Seek a stream to an absolute offset
    ~~~~
    IStream* source = // ...
    RETURN_HR(wil::stream_set_position_nothrow(source, 16));
    ~~~~
    @param stream The stream whose size is to be returned in `value`
    @param offset The position, in bytes from the start of the stream, to seek to
    @param value Optionally recieves the new absolute position from the stream
    */
    inline HRESULT stream_set_position_nothrow(_In_ IStream* stream, unsigned long long offset, _Out_opt_ unsigned long long* value = nullptr)
    {
        // IStream::Seek(..., _SET) interprets the first parameter as an unsigned value.
        return stream_seek_nothrow(stream, static_cast<long long>(offset), STREAM_SEEK_SET, value);
    }

    /** Seek a relative amount in a stream
    ~~~~
    IStream* source = // ...
    RETURN_IF_FAILED(wil::stream_seek_from_current_position_nothrow(source, -16));

    ULONGLONG newPosition;
    RETURN_IF_FAILED(wil::stream_seek_from_current_position_nothrow(source, 16, &newPosition));
    ~~~~
    @param stream The stream whose location is to be moved
    @param amount The offset, in bytes, to seek the stream.
    @param value Set to the new absolute steam position, in bytes
    */
    inline HRESULT stream_seek_from_current_position_nothrow(_In_ IStream* stream, long long amount, _Out_opt_ unsigned long long* value = nullptr)
    {
        return stream_seek_nothrow(stream, amount, STREAM_SEEK_CUR, value);
    }

    /** Determine the current byte position in the stream
    ~~~~
    IStream* source = // ...
    ULONGLONG currentPos;
    RETURN_IF_FAILED(wil::stream_get_position_nothrow(source, &currentPos));
    ~~~~
    @param stream The stream whose location is to be moved
    @param position Set to the current absolute steam position, in bytes
    */
    inline HRESULT stream_get_position_nothrow(_In_ IStream* stream, _Out_ unsigned long long* position)
    {
        return stream_seek_from_current_position_nothrow(stream, 0, position);
    }

    /** Moves the stream to absolute position 0
    ~~~~
    IStream* source = // ...
    RETURN_IF_FAILED(wil::stream_reset_nothrow(source));
    ~~~~
    @param stream The stream whose location is to be moved
    */
    inline HRESULT stream_reset_nothrow(_In_ IStream* stream)
    {
        return stream_set_position_nothrow(stream, 0);
    }

    /** Copy data from one stream to another, returning the final amount copied.
    ~~~~
    IStream* source = // ...
    IStream* target = // ...
    ULONGLONG copied;
    RETURN_IF_FAILED(wil::stream_copy_bytes_nothrow(source, target, sizeof(MyType), &copied));
    if (copied < sizeof(MyType))
    {
        DoSomethingAboutPartialCopy();
    }
    ~~~~
    @param source The stream from which to copy at most `amount` bytes
    @param target The steam to which to copy at most `amount` bytes
    @param amount The maximum number of bytes to copy from `source` to `target`
    @param pCopied If non-null, set to the number of bytes copied between the two.
    */
    inline HRESULT stream_copy_bytes_nothrow(_In_ IStream* source, _In_ IStream* target, unsigned long long amount, _Out_opt_ unsigned long long* pCopied = nullptr)
    {
        ULARGE_INTEGER toCopy{};
        ULARGE_INTEGER copied{};
        toCopy.QuadPart = amount;
        RETURN_IF_FAILED(source->CopyTo(target, toCopy, nullptr, &copied));
        assign_to_opt_param(pCopied, copied.QuadPart);

        return S_OK;
    }

    /** Copy all data from one stream to another, returning the final amount copied.
    ~~~~
    IStream* source = // ...
    IStream* target = // ...
    ULONGLONG copied;
    RETURN_IF_FAILED(wil::stream_copy_all_nothrow(source, target, &copied));
    if (copied < 8)
    {
       DoSomethingAboutPartialCopy();
    }
    ~~~~
    @param source The stream from which to copy all content
    @param target The steam to which to copy all content
    @param pCopied If non-null, set to the number of bytes copied between the two.
    */
    inline HRESULT stream_copy_all_nothrow(_In_ IStream* source, _In_ IStream* target, _Out_opt_ unsigned long long* pCopied = nullptr)
    {
        return stream_copy_bytes_nothrow(source, target, ULLONG_MAX, pCopied);
    }

    /** Copies an exact amount of data from one stream to another, failing otherwise
    ~~~~
    IStream* source = // ...
    IStream* target = // ...
    RETURN_IF_FAILED(wil::stream_copy_all_nothrow(source, target, 16));
    ~~~~
    @param source The stream from which to copy at most `amount` bytes
    @param target The steam to which to copy at most `amount` bytes
    @param amount The number of bytes to copy from `source` to `target`
    */
    inline HRESULT stream_copy_exact_nothrow(_In_ IStream* source, _In_ IStream* target, unsigned long long amount)
    {
        unsigned long long copied;
        RETURN_IF_FAILED(stream_copy_bytes_nothrow(source, target, ULLONG_MAX, &copied));
        RETURN_HR_IF(HRESULT_FROM_WIN32(ERROR_INVALID_DATA), copied != amount);

        return S_OK;
    }

    //! Controls behavior when reading a zero-length string from a stream
    enum class empty_string_options
    {
        //! Zero-length strings are returned as nullptr
        returns_null,

        //! Zero-length strings are allocated and returned with zero characters
        returns_empty,
    };

#ifdef __WIL_OBJBASE_H_

    /** Read a string from a stream and returns an allocated copy
    Deserializes strings in streams written by both IStream_WriteStr and wil::stream_write_string[_nothrow]. The format
    is a single 16-bit quantity, followed by that many wchar_ts. The returned string is allocated with CoTaskMemAlloc.
    Returns a zero-length (but non-null) string if the stream contained a zero-length string.
    ~~~~
    IStream* source = // ...
    wil::unique_cotaskmem_string content;
    RETURN_IF_FAILED(wil::stream_read_string_nothrow(source, &content));
    if (wcscmp(content.get(), L"waffles") == 0)
    {
        // Waffles!
    }
    ~~~~
    @param source The stream from which to read a string
    @param value Set to point to the allocated result of reading a string from `source`
    */
    inline HRESULT stream_read_string_nothrow(
        _In_ ISequentialStream* source,
        _When_(options == empty_string_options::returns_empty, _Outptr_result_z_) _When_(options == empty_string_options::returns_null, _Outptr_result_maybenull_z_) wchar_t** value,
        empty_string_options options = empty_string_options::returns_empty)
    {
        unsigned short cch;
        RETURN_IF_FAILED(stream_read_nothrow(source, &cch));

        if ((cch == 0) && (options == empty_string_options::returns_null))
        {
            *value = nullptr;
        }
        else
        {
            auto allocated = make_unique_cotaskmem_nothrow<wchar_t[]>(static_cast<size_t>(cch) + 1);
            RETURN_IF_NULL_ALLOC(allocated);
            RETURN_IF_FAILED(stream_read_nothrow(source, allocated.get(), static_cast<unsigned long>(cch) * sizeof(wchar_t)));
            allocated[cch] = 0;

            *value = allocated.release();
        }

        return S_OK;
    }

#endif // __WIL_OBJBASE_H

    /** Write a string to a stream
    Serializes a string into a stream by putting its length and then the wchar_ts in the string
    into the stream.  Zero-length strings have their length but no data written. This is the
    form expected by IStream_ReadStr and wil::string_read_stream.
    ~~~~
    IStream* target = // ...
    RETURN_IF_FAILED(wil::stream_write_string_nothrow(target, L"Waffles", 3));
    // Produces wchar_t[] { 0x3, L'W', L'a', L'f' };
    ~~~~
    @param target The stream to which to write a string
    @param source The string to write. Can be null if `writeLength` is zero
    @param writeLength The number of characters to write from source into `target`
    */
    inline HRESULT stream_write_string_nothrow(_In_ ISequentialStream* target, _In_reads_opt_(writeLength) const wchar_t*  source, _In_ size_t writeLength)
    {
        FAIL_FAST_IF(writeLength > USHRT_MAX);

        RETURN_IF_FAILED(stream_write_nothrow(target, static_cast<unsigned short>(writeLength)));

        if (writeLength > 0)
        {
            RETURN_IF_FAILED(stream_write_nothrow(target, source, static_cast<unsigned short>(writeLength) * sizeof(wchar_t)));
        }

        return S_OK;
    }

    /** Write a string to a stream
    Serializes a string into a stream by putting its length and then the wchar_ts in the string
    into the stream.  Zero-length strings have their length but no data written. This is the
    form expected by IStream_ReadStr and wil::string_read_stream.
    ~~~~
    IStream* target = // ...
    RETURN_IF_FAILED(wil::stream_write_string_nothrow(target, L"Waffles"));
    // Produces wchar_t[] { 0x3, L'W', L'a', L'f', L'f', L'l', L'e', L's' };
    ~~~~
    @param target The stream to which to write a string
    @param source The string to write. When nullptr, a zero-length string is written.
    */
    inline HRESULT stream_write_string_nothrow(_In_ ISequentialStream* target, _In_opt_z_ const wchar_t*  source)
    {
        return stream_write_string_nothrow(target, source, source ? wcslen(source) : 0);
    }

#ifdef WIL_ENABLE_EXCEPTIONS

    /** Read data from a stream into a buffer.
    ~~~~
    IStream* source = // ...
    ULONG dataBlob = 0;
    auto read = wil::stream_read_partial(source, &dataBlob, sizeof(dataBlob));
    if (read != sizeof(dataBlob))
    {
        // end of stream, probably
    }
    else if (dataBlob == 0x8675309)
    {
        DoThing(dataBlob);
    }
    ~~~~
    @param stream The stream from which to read at most `size` bytes.
    @param data A buffer into which up to `size` bytes will be read
    @param size The size, in bytes, of the buffer pointed to by `data`
    @return The amount, in bytes, of data read from `stream` into `data`
    */
    inline unsigned long stream_read_partial(_In_ ISequentialStream* stream, _Out_writes_bytes_to_(size, return) void* data, unsigned long size)
    {
        unsigned long didRead;
        THROW_IF_FAILED(stream_read_partial_nothrow(stream, data, size, &didRead));

        return didRead;
    }

    /** Read an exact number of bytes from a stream into a buffer.
    Fails if the stream didn't read all the bytes requested by throwing HRESULT_FROM_WIN32(ERROR_INVALID_DATA).
    ~~~~
    IStream* source = // ...
    ULONG dataBlob = 0;
    wil::stream_read(source, &dataBlob, sizeof(dataBlob));
    if (dataBlob == 0x8675309)
    {
        DoThing(dataBlob);
    }
    ~~~~
    @param stream The stream from which to read at most `size` bytes.
    @param data A buffer into which up to `size` bytes will be read
    @param size The size, in bytes, of the buffer pointed to by `data`
    */
    inline void stream_read(_In_ ISequentialStream* stream, _Out_writes_bytes_all_(size) void* data, unsigned long size)
    {
        THROW_HR_IF(HRESULT_FROM_WIN32(ERROR_INVALID_DATA), stream_read_partial(stream, data, size) != size);
    }

    /** Read from a stream into a POD type.
    Fails if the stream didn't have enough bytes by throwing HRESULT_FROM_WIN32(ERROR_INVALID_DATA).
    ~~~~
    IStream* source = // ...
    MY_HEADER header = wil::stream_read<MY_HEADER>(source);
    if (header.Version == 0x8675309)
    {
        ConsumeOldHeader(stream, header);
    }
    ~~~~
    @param stream The stream from which to read at most `sizeof(T)` bytes.
    @return An instance of `T` read from the stream
    */
    template<typename T> T stream_read(_In_ ISequentialStream* stream)
    {
        static_assert(__is_pod(T), "Read type must be POD");
        T temp{};
        stream_read(stream, &temp, sizeof(temp));

        return temp;
    }

    /** Write an exact number of bytes to a stream from a buffer.
    Fails if the stream didn't read write the bytes requested.
    ~~~~
    IStream* source = // ...
    ULONG dataBlob = 0;
    wil::stream_write(source, dataBlob, sizeof(dataBlob));
    ~~~~
    @param stream The stream to which to write at most `size` bytes.
    @param data A buffer from which up to `size` bytes will be read
    @param size The size, in bytes, of the buffer pointed to by `data`
    */
    inline void stream_write(_In_ ISequentialStream* stream, _In_reads_bytes_(size) const void* data, unsigned long size)
    {
        THROW_IF_FAILED(stream_write_nothrow(stream, data, size));
    }

    /** Write a POD type to a stream.
    Fails if the stream didn't accept the entire size.
    ~~~~
    IStream* target = // ...

    MY_HEADER header { 0x8675309, HEADER_FLAG_1 | HEADER_FLAG_2 };
    wil::stream_write(target, header)

    wil::stream_write<ULONGLONG>(target, 16);
    ~~~~
    @param stream The stream to which to write `thing`
    @param thing The POD data type to write to the stream.
    */
    template<typename T> inline void stream_write(_In_ ISequentialStream* stream, const T& thing)
    {
        stream_write(stream, wistd::addressof(thing), sizeof(thing));
    }

    /** Retrieve the size of this stream, in bytes
    ~~~~
    IStream* source = // ...
    ULONGLONG size = wil::stream_size(source);
    ~~~~
    @param stream The stream whose size is to be returned in `value`
    @return The size, in bytes, reported by `stream`
    */
    inline unsigned long long stream_size(_In_ IStream* stream)
    {
        unsigned long long size;
        THROW_IF_FAILED(stream_size_nothrow(stream, &size));

        return size;
    }

    /** Seek a stream to an absolute offset
    ~~~~
    IStream* source = // ...
    wil::stream_set_position(source, sizeof(HEADER));
    ~~~~
    @param stream The stream whose size is to be returned in `value`
    @param offset The offset, in bytes, to seek the stream.
    @return The new absolute stream position, in bytes
    */
    inline unsigned long long stream_set_position(_In_ IStream* stream, unsigned long long offset)
    {
        unsigned long long landed;
        THROW_IF_FAILED(stream_set_position_nothrow(stream, offset, &landed));
        return landed;
    }

    /** Seek a relative amount in a stream
    ~~~~
    IStream* source = // ...
    ULONGLONG newPosition = wil::stream_seek_from_current_position(source, 16);
    ~~~~
    @param stream The stream whose location is to be moved
    @param amount The offset, in bytes, to seek the stream.
    @return The new absolute stream position, in bytes
    */
    inline unsigned long long stream_seek_from_current_position(_In_ IStream* stream, long long amount)
    {
        unsigned long long landed;
        THROW_IF_FAILED(stream_seek_from_current_position_nothrow(stream, amount, &landed));

        return landed;
    }

    /** Determine the current byte position in the stream
    ~~~~
    IStream* source = // ...
    ULONGLONG currentPos = wil::stream_get_position(source);
    ~~~~
    @param stream The stream whose location is to be moved
    @return The current position reported by `stream`
    */
    inline unsigned long long stream_get_position(_In_ IStream* stream)
    {
        return stream_seek_from_current_position(stream, 0);
    }

    /** Moves the stream to absolute position 0
    ~~~~
    IStream* source = // ...
    wil::stream_reset(source);
    ASSERT(wil::stream_get_position(source) == 0);
    ~~~~
    @param stream The stream whose location is to be moved
    */
    inline void stream_reset(_In_ IStream* stream)
    {
        stream_set_position(stream, 0);
    }

    /** Copy data from one stream to another
    ~~~~
    IStream* source = // ...
    IStream* target = // ...
    ULONGLONG copied = ;
    if (wil::stream_copy_bytes(source, target, sizeof(Header)) < sizeof(Header))
    {
       DoSomethingAboutPartialCopy();
    }
    ~~~~
    @param source The stream from which to copy at most `amount` bytes
    @param target The steam to which to copy at most `amount` bytes
    @param amount The maximum number of bytes to copy from `source` to `target`
    @return The number of bytes copied between the two streams
    */
    inline unsigned long long stream_copy_bytes(_In_ IStream* source, _In_ IStream* target, unsigned long long amount)
    {
        unsigned long long copied;
        THROW_IF_FAILED(stream_copy_bytes_nothrow(source, target, amount, &copied));

        return copied;
    }

    /** Copy all data from one stream to another
    ~~~~
    IStream* source = // ...
    IStream* target = // ...
    ULONGLONG copied = wil::stream_copy_all(source, target);
    ~~~~
    @param source The stream from which to copy all content
    @param target The steam to which to copy all content
    @return The number of bytes copied between the two.
    */
    inline unsigned long long stream_copy_all(_In_ IStream* source, _In_ IStream* target)
    {
        return stream_copy_bytes(source, target, ULLONG_MAX);
    }

    /** Copies an exact amount of data from one stream to another, failing otherwise
    ~~~~
    IStream* source = // ...
    IStream* target = // ...
    wil::stream_copy_all_nothrow(source, target, sizeof(SOMETHING));
    ~~~~
    @param source The stream from which to copy at most `amount` bytes
    @param target The steam to which to copy at most `amount` bytes
    @param amount The number of bytes to copy from `source` to `target`
    */
    inline void stream_copy_exact(_In_ IStream* source, _In_ IStream* target, unsigned long long amount)
    {
        THROW_HR_IF(HRESULT_FROM_WIN32(ERROR_INVALID_DATA), stream_copy_bytes(source, target, amount) != amount);
    }

#ifdef __WIL_OBJBASE_H_

    /** Read a string from a stream and returns an allocated copy
    Deserializes strings in streams written by both IStream_WriteStr and wil::stream_write_string[_nothrow]. The format
    is a single 16-bit quantity, followed by that many wchar_ts. The returned string is allocated with CoTaskMemAlloc.
    Returns a zero-length (but non-null) string if the stream contained a zero-length string.
    ~~~~
    IStream* source = // ...
    wil::unique_cotaskmem_string content = wil::stream_read_string(source);
    if (wcscmp(content.get(), L"waffles") == 0)
    {
        // Waffles!
    }
    ~~~~
    @param source The stream from which to read a string
    @return An non-null string (but possibly zero lengh) string read from `source`
    */
    inline wil::unique_cotaskmem_string stream_read_string(_In_ ISequentialStream* source, empty_string_options options = empty_string_options::returns_empty)
    {
        wil::unique_cotaskmem_string result;
        THROW_IF_FAILED(stream_read_string_nothrow(source, &result, options));

        return result;
    }

#endif // __WIL_OBJBASE_H

    /** Write a string to a stream
    Serializes a string into a stream by putting its length and then the wchar_ts in the string
    into the stream.  Zero-length strings have their length but no data written. This is the
    form expected by IStream_ReadStr and wil::string_read_stream.
    ~~~~
    IStream* target = // ...
    wil::stream_write_string(target, L"Waffles", 3);
    ~~~~
    @param target The stream to which to write a string
    @param source The string to write. Can be null if `toWriteCch` is zero
    @param toWriteCch The number of characters to write from source into `target`
    */
    inline void stream_write_string(_In_ ISequentialStream* target, _In_reads_opt_(toWriteCch) const wchar_t*  source, _In_ size_t toWriteCch)
    {
        THROW_IF_FAILED(stream_write_string_nothrow(target, source, toWriteCch));
    }

    /** Write a string to a stream
    Serializes a string into a stream by putting its length and then the wchar_ts in the string
    into the stream.  Zero-length strings have their length but no data written.This is the
    form expected by IStream_ReadStr and wil::string_read_stream.
    ~~~~
    IStream* target = // ...
    wil::stream_write_string(target, L"Waffles");
    ~~~~
    @param target The stream to which to write a string
    @param source The string to write. When nullptr, a zero-length string is written.
    */
    inline void stream_write_string(_In_ ISequentialStream* target, _In_opt_z_ const wchar_t*  source)
    {
        THROW_IF_FAILED(stream_write_string_nothrow(target, source, source ? wcslen(source) : 0));
    }

    /** Saves and restores the position of a stream
    Useful for potentially reading data from a stream, or being able to read ahead, then reset
    back to where one left off, such as conditionally reading content from a stream.
    ~~~~
    void MaybeConsumeStream(IStream* stream)
    {
        // On error, reset the read position in the stream to where we left off
        auto saver = wil::stream_position_saver(stream);
        auto header = wil::stream_read<MY_HEADER>(stream);
        for (ULONG i = 0; i < header.Count; ++i)
        {
            ProcessElement(wil::stream_read<MY_ELEMENT>(stream));
        }
    }
    ~~~~
    */
    class stream_position_saver
    {
    public:
        //! Constructs a saver from the current position of this stream
        //! @param stream The stream instance whose position is to be saved.
        explicit stream_position_saver(_In_opt_ IStream* stream) :
            m_stream(stream),
            m_position(stream ? stream_get_position(stream) : 0)
        {
        }

        ~stream_position_saver()
        {
            if (m_stream)
            {
                LOG_IF_FAILED(stream_set_position_nothrow(m_stream.get(), m_position));
            }
        }

        /** Updates the current position in the stream
        ~~~~
        // Read a size marker from the stream, then advance that much.
        IStream* stream1 = // ...
        auto saver = wil::stream_position_saver(stream1);
        auto size = wil::stream_read<long>(stream1);
        wil::stream_seek_from_current_position(stream, size);
        saver.update();
        ~~~~
        */
        void update()
        {
            m_position = stream_get_position(m_stream.get());
        }

        //! Returns the current position being saved for the stream
        //! @returns The position, in bytes, being saved for the stream
        WI_NODISCARD unsigned long long position() const
        {
            return m_position;
        }

        /** Resets the position saver to manage a new stream
        Reverts the position of any stream this saver is currently holding a place for.
        ~~~~
        IStream* stream1 = // ...
        IStream* stream2 = // ...
        auto saver = wil::stream_position_saver(stream1);
        if (wil::stream_read<MyType>(stream1).Flags != 0)
        {
            saver.reset(stream2); // position in stream1 is reverted, now holding stream2
        }
        ~~~~
        @param stream The stream whose position is to be saved
        */
        void reset(_In_ IStream* stream)
        {
            reset();

            m_stream = stream;
            m_position = wil::stream_get_position(m_stream.get());
        }

        /** Resets the position of the stream
        ~~~~
        IStream* stream1 = // ...
        auto saver = wil::stream_position_saver(stream1);
        MyType mt = wil::stream_read<MyType>(stream1);
        if (mt.Flags & MyTypeFlags::Extended)
        {
            saver.reset();
            ProcessExtended(stream1, wil::stream_read<MyTypeExtended>(stream1));
        }
        else
        {
            ProcessStandard(stream1, mt);
        }
        ~~~~
        */
        void reset()
        {
            if (m_stream)
            {
                wil::stream_set_position(m_stream.get(), m_position);
            }
        }

        /** Stops saving the position of the stream
        ~~~~
        // The stream has either a standard or extended header, followed by interesting content.
        // Read either one, leaving the stream after the headers have been read off. On failure,
        // the stream's position is restored.
        std::pair<MyType, MyTypeExtended> get_headers(_In_ IStream* source)
        {
            auto saver = wil::stream_position_saver(stream1);
            MyType mt = wil::stream_read<MyType>(stream1);
            MyTypeExtended mte{};
            if (mt.Flags & MyTypeFlags::Extended)
            {
                mte = wil::stream_read<MyTypeExtended>(stream1);
            }
            saver.dismiss();
            return { mt, mte };
        }
        ~~~~
        */
        void dismiss()
        {
            m_stream.reset();
        }

        stream_position_saver(stream_position_saver&&) = default;
        stream_position_saver& operator=(stream_position_saver&&) = default;

        stream_position_saver(const stream_position_saver&) = delete;
        void operator=(const stream_position_saver&) = delete;

    private:
        com_ptr<IStream> m_stream;
        unsigned long long m_position;
    };
#endif // WIL_ENABLE_EXCEPTIONS
#pragma endregion // stream helpers

#if defined(__IObjectWithSite_INTERFACE_DEFINED__)
    /// @cond
    namespace details
    {
        inline void __stdcall SetSiteNull(IObjectWithSite* objWithSite)
        {
            objWithSite->SetSite(nullptr); // break the cycle
        }
    } // details
    /// @endcond

    using unique_set_site_null_call = wil::unique_com_call<IObjectWithSite, decltype(details::SetSiteNull), details::SetSiteNull>;

    /** RAII support for managing the site chain. This function sets the site pointer on an object and return an object
    that resets it on destruction to break the cycle.
    Note, this does not preserve the existing site if there is one (an uncommon case) so only use this when that is not required.
    ~~~
    auto cleanup = wil::com_set_site(execCommand.get(), serviceProvider->GetAsSite());
    ~~~
    Include ocidl.h before wil\com.h to use this.
    */
    WI_NODISCARD inline unique_set_site_null_call com_set_site(_In_opt_ IUnknown* obj, _In_opt_ IUnknown* site)
    {
        wil::com_ptr_nothrow<IObjectWithSite> objWithSite;
        if (site && wil::try_com_copy_to(obj, &objWithSite))
        {
            objWithSite->SetSite(site);
        }
        return unique_set_site_null_call(objWithSite.get());
    }

    /** Iterate over each object in a site chain. Useful for debugging site issues, here is sample use.
    ~~~
    void OutputDebugSiteChainWatchWindowText(IUnknown* site)
    {
        OutputDebugStringW(L"Copy and paste these entries into the Visual Studio Watch Window\n");
        wil::for_each_site(site, [](IUnknown* site)
        {
            wchar_t msg[64];
            StringCchPrintfW(msg, ARRAYSIZE(msg), L"((IUnknown*)0x%p)->__vfptr[0]\n", site);
            OutputDebugStringW(msg);
        });
    }
    */

    template<typename TLambda>
    void for_each_site(_In_opt_ IUnknown* siteInput, TLambda&& callback)
    {
        wil::com_ptr_nothrow<IUnknown> site(siteInput);
        while (site)
        {
            callback(site.get());
            auto objWithSite = site.try_query<IObjectWithSite>();
            site.reset();
            if (objWithSite)
            {
                objWithSite->GetSite(IID_PPV_ARGS(&site));
            }
        }
    }

#endif // __IObjectWithSite_INTERFACE_DEFINED__

} // wil

#endif
