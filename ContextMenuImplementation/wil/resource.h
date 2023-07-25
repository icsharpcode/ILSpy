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

#include "result_macros.h"
#include "wistd_functional.h"
#include "wistd_memory.h"

#pragma warning(push)
#pragma warning(disable:26135 26110)    // Missing locking annotation, Caller failing to hold lock
#pragma warning(disable:4714)           // __forceinline not honored

#ifndef __WIL_RESOURCE
#define __WIL_RESOURCE

// stdint.h and intsafe.h have conflicting definitions, so it's not safe to include either to pick up our dependencies,
// so the definitions we need are copied below
#ifdef _WIN64
#define __WI_SIZE_MAX   0xffffffffffffffffui64 // UINT64_MAX
#else /* _WIN64 */
#define __WI_SIZE_MAX   0xffffffffui32 // UINT32_MAX
#endif /* _WIN64 */

// Forward declaration
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
    //! This type copies the current value of GetLastError at construction and resets the last error
    //! to that value when it is destroyed.
    //!
    //! This is useful in library code that runs during a value's destructor. If the library code could
    //! inadvertently change the value of GetLastError (by calling a Win32 API or similar), it should
    //! instantiate a value of this type before calling the library function in order to preserve the
    //! GetLastError value the user would expect.
    //!
    //! This construct exists to hide kernel mode/user mode differences in wil library code.
    //!
    //! Example usage:
    //!
    //!     if (!CreateFile(...))
    //!     {
    //!         auto lastError = wil::last_error_context();
    //!         WriteFile(g_hlog, logdata);
    //!     }
    //!
    class last_error_context
    {
#ifndef WIL_KERNEL_MODE
        bool m_dismissed = false;
        DWORD m_error = 0;
    public:
        last_error_context() WI_NOEXCEPT : last_error_context(::GetLastError())
        {
        }

        explicit last_error_context(DWORD error) WI_NOEXCEPT :
            m_error(error)
        {
        }

        last_error_context(last_error_context&& other) WI_NOEXCEPT
        {
            operator=(wistd::move(other));
        }

        last_error_context& operator=(last_error_context&& other) WI_NOEXCEPT
        {
            m_dismissed = wistd::exchange(other.m_dismissed, true);
            m_error = other.m_error;

            return *this;
        }

        ~last_error_context() WI_NOEXCEPT
        {
            if (!m_dismissed)
            {
                ::SetLastError(m_error);
            }
        }

        //! last_error_context doesn't own a concrete resource, so therefore
        //! it just disarms its destructor and returns void.
        void release() WI_NOEXCEPT
        {
            WI_ASSERT(!m_dismissed);
            m_dismissed = true;
        }

        WI_NODISCARD auto value() const WI_NOEXCEPT
        {
            return m_error;
        }
#else
    public:
        void release() WI_NOEXCEPT { }
#endif // WIL_KERNEL_MODE
    };

    /// @cond
    namespace details
    {
        typedef wistd::integral_constant<size_t, 0> pointer_access_all;             // get(), release(), addressof(), and '&' are available
        typedef wistd::integral_constant<size_t, 1> pointer_access_noaddress;       // get() and release() are available
        typedef wistd::integral_constant<size_t, 2> pointer_access_none;            // the raw pointer is not available

        template<bool is_fn_ptr, typename close_fn_t, close_fn_t close_fn, typename pointer_storage_t> struct close_invoke_helper
        {
            __forceinline static void close(pointer_storage_t value) WI_NOEXCEPT { wistd::invoke(close_fn, value); }
            inline static void close_reset(pointer_storage_t value) WI_NOEXCEPT
            {
                auto preserveError = last_error_context();
                wistd::invoke(close_fn, value);
            }
        };

        template<typename close_fn_t, close_fn_t close_fn, typename pointer_storage_t> struct close_invoke_helper<true, close_fn_t, close_fn, pointer_storage_t>
        {
            __forceinline static void close(pointer_storage_t value) WI_NOEXCEPT { close_fn(value); }
            inline static void close_reset(pointer_storage_t value) WI_NOEXCEPT
            {
                auto preserveError = last_error_context();
                close_fn(value);
            }
        };

        template<typename close_fn_t, close_fn_t close_fn, typename pointer_storage_t> using close_invoker =
            close_invoke_helper<wistd::is_pointer_v<close_fn_t> ? wistd::is_function_v<wistd::remove_pointer_t<close_fn_t>> : false, close_fn_t, close_fn, pointer_storage_t>;

        template <typename pointer_t,                                         // The handle type
            typename close_fn_t,                                              // The handle close function type
            close_fn_t close_fn,                                              //      * and function pointer
            typename pointer_access_t = pointer_access_all,                   // all, noaddress or none to control pointer method access
            typename pointer_storage_t = pointer_t,                           // The type used to store the handle (usually the same as the handle itself)
            typename invalid_t = pointer_t,                                   // The invalid handle value type
            invalid_t invalid = invalid_t{},                                  //      * and its value (default ZERO value)
            typename pointer_invalid_t = wistd::nullptr_t>                    // nullptr_t if the invalid handle value is compatible with nullptr, otherwise pointer
            struct resource_policy : close_invoker<close_fn_t, close_fn, pointer_storage_t>
        {
            typedef pointer_storage_t pointer_storage;
            typedef pointer_t pointer;
            typedef pointer_invalid_t pointer_invalid;
            typedef pointer_access_t pointer_access;
            __forceinline static pointer_storage invalid_value() { return (pointer)invalid; }
            __forceinline static bool is_valid(pointer_storage value) WI_NOEXCEPT { return (static_cast<pointer>(value) != (pointer)invalid); }
        };


        // This class provides the pointer storage behind the implementation of unique_any_t utilizing the given
        // resource_policy.  It is separate from unique_any_t to allow a type-specific specialization class to plug
        // into the inheritance chain between unique_any_t and unique_storage.  This allows classes like unique_event
        // to be a unique_any formed class, but also expose methods like SetEvent directly.

        template <typename Policy>
        class unique_storage
        {
        protected:
            typedef Policy policy;
            typedef typename policy::pointer_storage pointer_storage;
            typedef typename policy::pointer pointer;
            typedef unique_storage<policy> base_storage;

        public:
            unique_storage() WI_NOEXCEPT :
            m_ptr(policy::invalid_value())
            {
            }

            explicit unique_storage(pointer_storage ptr) WI_NOEXCEPT :
                m_ptr(ptr)
            {
            }

            unique_storage(unique_storage &&other) WI_NOEXCEPT :
                m_ptr(wistd::move(other.m_ptr))
            {
                other.m_ptr = policy::invalid_value();
            }

            ~unique_storage() WI_NOEXCEPT
            {
                if (policy::is_valid(m_ptr))
                {
                    policy::close(m_ptr);
                }
            }

            WI_NODISCARD bool is_valid() const WI_NOEXCEPT
            {
                return policy::is_valid(m_ptr);
            }

            void reset(pointer_storage ptr = policy::invalid_value()) WI_NOEXCEPT
            {
                if (policy::is_valid(m_ptr))
                {
                    policy::close_reset(m_ptr);
                }
                m_ptr = ptr;
            }

            void reset(wistd::nullptr_t) WI_NOEXCEPT
            {
                static_assert(wistd::is_same<typename policy::pointer_invalid, wistd::nullptr_t>::value, "reset(nullptr): valid only for handle types using nullptr as the invalid value");
                reset();
            }

            WI_NODISCARD pointer get() const WI_NOEXCEPT
            {
                return static_cast<pointer>(m_ptr);
            }

            pointer_storage release() WI_NOEXCEPT
            {
                static_assert(!wistd::is_same<typename policy::pointer_access, pointer_access_none>::value, "release(): the raw handle value is not available for this resource class");
                auto ptr = m_ptr;
                m_ptr = policy::invalid_value();
                return ptr;
            }

            pointer_storage *addressof() WI_NOEXCEPT
            {
                static_assert(wistd::is_same<typename policy::pointer_access, pointer_access_all>::value, "addressof(): the address of the raw handle is not available for this resource class");
                return &m_ptr;
            }

        protected:
            void replace(unique_storage &&other) WI_NOEXCEPT
            {
                reset(other.m_ptr);
                other.m_ptr = policy::invalid_value();
            }

        private:
            pointer_storage m_ptr;
        };
    } // details
      /// @endcond


      // This class when paired with unique_storage and an optional type-specific specialization class implements
      // the same interface as STL's unique_ptr<> for resource handle types.  It is a non-copyable, yet movable class
      // supporting attach (reset), detach (release), retrieval (get()).

    template <typename storage_t>
    class unique_any_t : public storage_t
    {
    public:
        typedef typename storage_t::policy policy;
        typedef typename policy::pointer_storage pointer_storage;
        typedef typename policy::pointer pointer;

        unique_any_t(unique_any_t const &) = delete;
        unique_any_t& operator=(unique_any_t const &) = delete;

        // Note that the default constructor really shouldn't be needed (taken care of by the forwarding constructor below), but
        // the forwarding constructor causes an internal compiler error when the class is used in a C++ array.  Defining the default
        // constructor independent of the forwarding constructor removes the compiler limitation.
        unique_any_t() = default;

        // forwarding constructor: forwards all 'explicit' and multi-arg constructors to the base class
        template <typename arg1, typename... args_t>
        explicit unique_any_t(arg1 && first, args_t&&... args)
            __WI_NOEXCEPT_((wistd::is_nothrow_constructible_v<storage_t, arg1, args_t...>)) :
            storage_t(wistd::forward<arg1>(first), wistd::forward<args_t>(args)...)
        {
            static_assert(wistd::is_same<typename policy::pointer_access, details::pointer_access_none>::value ||
                wistd::is_same<typename policy::pointer_access, details::pointer_access_all>::value ||
                wistd::is_same<typename policy::pointer_access, details::pointer_access_noaddress>::value, "pointer_access policy must be a known pointer_access* integral type");
        }

        unique_any_t(wistd::nullptr_t) WI_NOEXCEPT
        {
            static_assert(wistd::is_same<typename policy::pointer_invalid, wistd::nullptr_t>::value, "nullptr constructor: valid only for handle types using nullptr as the invalid value");
        }

        unique_any_t(unique_any_t &&other) WI_NOEXCEPT :
        storage_t(wistd::move(other))
        {
        }

        unique_any_t& operator=(unique_any_t &&other) WI_NOEXCEPT
        {
            if (this != wistd::addressof(other))
            {
                // cast to base_storage to 'skip' calling the (optional) specialization class that provides handle-specific functionality
                storage_t::replace(wistd::move(static_cast<typename storage_t::base_storage &>(other)));
            }
            return (*this);
        }

        unique_any_t& operator=(wistd::nullptr_t) WI_NOEXCEPT
        {
            static_assert(wistd::is_same<typename policy::pointer_invalid, wistd::nullptr_t>::value, "nullptr assignment: valid only for handle types using nullptr as the invalid value");
            storage_t::reset();
            return (*this);
        }

        void swap(unique_any_t &other) WI_NOEXCEPT
        {
            unique_any_t self(wistd::move(*this));
            operator=(wistd::move(other));
            other = wistd::move(self);
        }

        WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
        {
            return storage_t::is_valid();
        }

        //! ~~~~
        //! BOOL OpenOrCreateWaffle(PCWSTR name, HWAFFLE* handle);
        //! wil::unique_any<HWAFFLE, decltype(&::CloseWaffle), ::CloseWaffle> waffle;
        //! RETURN_IF_WIN32_BOOL_FALSE(OpenOrCreateWaffle(L"tasty.yum", waffle.put()));
        //! ~~~~
        pointer_storage *put() WI_NOEXCEPT
        {
            static_assert(wistd::is_same<typename policy::pointer_access, details::pointer_access_all>::value, "operator & is not available for this handle");
            storage_t::reset();
            return storage_t::addressof();
        }

        pointer_storage *operator&() WI_NOEXCEPT
        {
            return put();
        }

        WI_NODISCARD pointer get() const WI_NOEXCEPT
        {
            static_assert(!wistd::is_same<typename policy::pointer_access, details::pointer_access_none>::value, "get(): the raw handle value is not available for this resource class");
            return storage_t::get();
        }

        // The following functions are publicly exposed by their inclusion in the unique_storage base class

        // explicit unique_any_t(pointer_storage ptr) WI_NOEXCEPT
        // void reset(pointer_storage ptr = policy::invalid_value()) WI_NOEXCEPT
        // void reset(wistd::nullptr_t) WI_NOEXCEPT
        // pointer_storage release() WI_NOEXCEPT                                        // not exposed for some resource types
        // pointer_storage *addressof() WI_NOEXCEPT                                     // not exposed for some resource types
    };

    template <typename policy>
    void swap(unique_any_t<policy>& left, unique_any_t<policy>& right) WI_NOEXCEPT
    {
        left.swap(right);
    }

    template <typename policy>
    bool operator==(const unique_any_t<policy>& left, const unique_any_t<policy>& right) WI_NOEXCEPT
    {
        return (left.get() == right.get());
    }

    template <typename policy>
    bool operator==(const unique_any_t<policy>& left, wistd::nullptr_t) WI_NOEXCEPT
    {
        static_assert(wistd::is_same<typename unique_any_t<policy>::policy::pointer_invalid, wistd::nullptr_t>::value, "the resource class does not use nullptr as an invalid value");
        return !left;
    }

    template <typename policy>
    bool operator==(wistd::nullptr_t, const unique_any_t<policy>& right) WI_NOEXCEPT
    {
        static_assert(wistd::is_same<typename unique_any_t<policy>::policy::pointer_invalid, wistd::nullptr_t>::value, "the resource class does not use nullptr as an invalid value");
        return !right;
    }

    template <typename policy>
    bool operator!=(const unique_any_t<policy>& left, const unique_any_t<policy>& right) WI_NOEXCEPT
    {
        return (!(left.get() == right.get()));
    }

    template <typename policy>
    bool operator!=(const unique_any_t<policy>& left, wistd::nullptr_t) WI_NOEXCEPT
    {
        static_assert(wistd::is_same<typename unique_any_t<policy>::policy::pointer_invalid, wistd::nullptr_t>::value, "the resource class does not use nullptr as an invalid value");
        return !!left;
    }

    template <typename policy>
    bool operator!=(wistd::nullptr_t, const unique_any_t<policy>& right) WI_NOEXCEPT
    {
        static_assert(wistd::is_same<typename unique_any_t<policy>::policy::pointer_invalid, wistd::nullptr_t>::value, "the resource class does not use nullptr as an invalid value");
        return !!right;
    }

    template <typename policy>
    bool operator<(const unique_any_t<policy>& left, const unique_any_t<policy>& right) WI_NOEXCEPT
    {
        return (left.get() < right.get());
    }

    template <typename policy>
    bool operator>=(const unique_any_t<policy>& left, const unique_any_t<policy>& right) WI_NOEXCEPT
    {
        return (!(left < right));
    }

    template <typename policy>
    bool operator>(const unique_any_t<policy>& left, const unique_any_t<policy>& right) WI_NOEXCEPT
    {
        return (right < left);
    }

    template <typename policy>
    bool operator<=(const unique_any_t<policy>& left, const unique_any_t<policy>& right) WI_NOEXCEPT
    {
        return (!(right < left));
    }

    // unique_any provides a template alias for easily building a unique_any_t from a unique_storage class with the given
    // template parameters for resource_policy.

    template <typename pointer,                                   // The handle type
        typename close_fn_t,                                      // The handle close function type
        close_fn_t close_fn,                                      //      * and function pointer
        typename pointer_access = details::pointer_access_all,    // all, noaddress or none to control pointer method access
        typename pointer_storage = pointer,                       // The type used to store the handle (usually the same as the handle itself)
        typename invalid_t = pointer,                             // The invalid handle value type
        invalid_t invalid = invalid_t{},                          //      * and its value (default ZERO value)
        typename pointer_invalid = wistd::nullptr_t>              // nullptr_t if the invalid handle value is compatible with nullptr, otherwise pointer
        using unique_any = unique_any_t<details::unique_storage<details::resource_policy<pointer, close_fn_t, close_fn, pointer_access, pointer_storage, invalid_t, invalid, pointer_invalid>>>;

    /// @cond
    namespace details
    {
        template <typename TLambda>
        class lambda_call
        {
        public:
            lambda_call(const lambda_call&) = delete;
            lambda_call& operator=(const lambda_call&) = delete;
            lambda_call& operator=(lambda_call&& other) = delete;

            explicit lambda_call(TLambda&& lambda) WI_NOEXCEPT : m_lambda(wistd::move(lambda))
            {
                static_assert(wistd::is_same<decltype(lambda()), void>::value, "scope_exit lambdas must not have a return value");
                static_assert(!wistd::is_lvalue_reference<TLambda>::value && !wistd::is_rvalue_reference<TLambda>::value,
                    "scope_exit should only be directly used with a lambda");
            }

            lambda_call(lambda_call&& other) WI_NOEXCEPT : m_lambda(wistd::move(other.m_lambda)), m_call(other.m_call)
            {
                other.m_call = false;
            }

            ~lambda_call() WI_NOEXCEPT
            {
                reset();
            }

            // Ensures the scope_exit lambda will not be called
            void release() WI_NOEXCEPT
            {
                m_call = false;
            }

            // Executes the scope_exit lambda immediately if not yet run; ensures it will not run again
            void reset() WI_NOEXCEPT
            {
                if (m_call)
                {
                    m_call = false;
                    m_lambda();
                }
            }

            // Returns true if the scope_exit lambda is still going to be executed
            WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
            {
                return m_call;
            }

        protected:
            TLambda m_lambda;
            bool m_call = true;
        };

#ifdef WIL_ENABLE_EXCEPTIONS
        template <typename TLambda>
        class lambda_call_log
        {
        public:
            lambda_call_log(const lambda_call_log&) = delete;
            lambda_call_log& operator=(const lambda_call_log&) = delete;
            lambda_call_log& operator=(lambda_call_log&& other) = delete;

            explicit lambda_call_log(void* address, const DiagnosticsInfo& info, TLambda&& lambda) WI_NOEXCEPT :
            m_address(address), m_info(info), m_lambda(wistd::move(lambda))
            {
                static_assert(wistd::is_same<decltype(lambda()), void>::value, "scope_exit lambdas must return 'void'");
                static_assert(!wistd::is_lvalue_reference<TLambda>::value && !wistd::is_rvalue_reference<TLambda>::value,
                    "scope_exit should only be directly used with a lambda");
            }

            lambda_call_log(lambda_call_log&& other) WI_NOEXCEPT :
            m_address(other.m_address), m_info(other.m_info), m_lambda(wistd::move(other.m_lambda)), m_call(other.m_call)
            {
                other.m_call = false;
            }

            ~lambda_call_log() WI_NOEXCEPT
            {
                reset();
            }

            // Ensures the scope_exit lambda will not be called
            void release() WI_NOEXCEPT
            {
                m_call = false;
            }

            // Executes the scope_exit lambda immediately if not yet run; ensures it will not run again
            void reset() WI_NOEXCEPT
            {
                if (m_call)
                {
                    m_call = false;
                    try
                    {
                        m_lambda();
                    }
                    catch (...)
                    {
                        ReportFailure_CaughtException<FailureType::Log>(__R_DIAGNOSTICS(m_info), m_address);
                    }
                }
            }

            // Returns true if the scope_exit lambda is still going to be executed
            WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
            {
                return m_call;
            }

        private:
            void* m_address;
            DiagnosticsInfo m_info;
            TLambda m_lambda;
            bool m_call = true;
        };
#endif  // WIL_ENABLE_EXCEPTIONS
    }
    /// @endcond

    /** Returns an object that executes the given lambda when destroyed.
    Capture the object with 'auto'; use reset() to execute the lambda early or release() to avoid
    execution.  Exceptions thrown in the lambda will fail-fast; use scope_exit_log to avoid. */
    template <typename TLambda>
    WI_NODISCARD inline auto scope_exit(TLambda&& lambda) WI_NOEXCEPT
    {
        return details::lambda_call<TLambda>(wistd::forward<TLambda>(lambda));
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    /** Returns an object that executes the given lambda when destroyed; logs exceptions.
    Capture the object with 'auto'; use reset() to execute the lambda early or release() to avoid
    execution.  Exceptions thrown in the lambda will be caught and logged without being propagated. */
    template <typename TLambda>
    WI_NODISCARD inline __declspec(noinline) auto scope_exit_log(const DiagnosticsInfo& diagnostics, TLambda&& lambda) WI_NOEXCEPT
    {
        return details::lambda_call_log<TLambda>(_ReturnAddress(), diagnostics, wistd::forward<TLambda>(lambda));
    }
#endif

    // Forward declaration...
    template <typename T, typename err_policy>
    class com_ptr_t;

    //! Type traits class that identifies the inner type of any smart pointer.
    template <typename Ptr>
    struct smart_pointer_details
    {
        typedef typename Ptr::pointer pointer;
    };

    /// @cond
    template <typename T>
    struct smart_pointer_details<Microsoft::WRL::ComPtr<T>>
    {
        typedef T* pointer;
    };
    /// @endcond

    /** Generically detaches a raw pointer from any smart pointer.
    Caller takes ownership of the returned raw pointer; calls the correct release(), detach(),
    or Detach() method based on the smart pointer type */
    template <typename TSmartPointer>
    WI_NODISCARD typename TSmartPointer::pointer detach_from_smart_pointer(TSmartPointer& smartPtr)
    {
        return smartPtr.release();
    }

    /// @cond
    // Generically detaches a raw pointer from any smart pointer
    template <typename T, typename err>
    WI_NODISCARD T* detach_from_smart_pointer(wil::com_ptr_t<T, err>& smartPtr)
    {
        return smartPtr.detach();
    }

    // Generically detaches a raw pointer from any smart pointer
    template <typename T>
    WI_NODISCARD T* detach_from_smart_pointer(Microsoft::WRL::ComPtr<T>& smartPtr)
    {
        return smartPtr.Detach();
    }

    template<typename T, typename err> class com_ptr_t; // forward
    namespace details
    {
        // The first two attach_to_smart_pointer() overloads are ambiguous when passed a com_ptr_t.
        // To solve that use this functions return type to elminate the reset form for com_ptr_t.
        template <typename T, typename err> wistd::false_type use_reset(wil::com_ptr_t<T, err>*) { return wistd::false_type(); }
        template <typename T> wistd::true_type use_reset(T*) { return wistd::true_type(); }
    }
    /// @endcond

    /** Generically attach a raw pointer to a compatible smart pointer.
    Calls the correct reset(), attach(), or Attach() method based on samrt pointer type. */
    template <typename TSmartPointer, typename EnableResetForm = wistd::enable_if_t<decltype(details::use_reset(static_cast<TSmartPointer*>(nullptr)))::value>>
    void attach_to_smart_pointer(TSmartPointer& smartPtr, typename TSmartPointer::pointer rawPtr)
    {
        smartPtr.reset(rawPtr);
    }

    /// @cond

    // Generically attach a raw pointer to a compatible smart pointer.
    template <typename T, typename err>
    void attach_to_smart_pointer(wil::com_ptr_t<T, err>& smartPtr, T* rawPtr)
    {
        smartPtr.attach(rawPtr);
    }

    // Generically attach a raw pointer to a compatible smart pointer.
    template <typename T>
    void attach_to_smart_pointer(Microsoft::WRL::ComPtr<T>& smartPtr, T* rawPtr)
    {
        smartPtr.Attach(rawPtr);
    }
    /// @endcond

    //! @ingroup outparam
    /** Detach a smart pointer resource to an optional output pointer parameter.
    Avoids cluttering code with nullptr tests; works generically for any smart pointer */
    template <typename T, typename TSmartPointer>
    inline void detach_to_opt_param(_Out_opt_ T* outParam, TSmartPointer&& smartPtr)
    {
        if (outParam)
        {
            *outParam = detach_from_smart_pointer(smartPtr);
        }
    }

    /// @cond
    namespace details
    {
        template <typename T>
        struct out_param_t
        {
            typedef typename wil::smart_pointer_details<T>::pointer pointer;
            T &wrapper;
            pointer pRaw;
            bool replace = true;

            out_param_t(_Inout_ T &output) :
                wrapper(output),
                pRaw(nullptr)
            {
            }

            out_param_t(out_param_t&& other) WI_NOEXCEPT :
                wrapper(other.wrapper),
                pRaw(other.pRaw)
            {
                WI_ASSERT(other.replace);
                other.replace = false;
            }

            operator pointer*()
            {
                WI_ASSERT(replace);
                return &pRaw;
            }

            ~out_param_t()
            {
                if (replace)
                {
                    attach_to_smart_pointer(wrapper, pRaw);
                }
            }

            out_param_t(out_param_t const &other) = delete;
            out_param_t &operator=(out_param_t const &other) = delete;
        };

        template <typename Tcast, typename T>
        struct out_param_ptr_t
        {
            typedef typename wil::smart_pointer_details<T>::pointer pointer;
            T &wrapper;
            pointer pRaw;
            bool replace = true;

            out_param_ptr_t(_Inout_ T &output) :
                wrapper(output),
                pRaw(nullptr)
            {
            }

            out_param_ptr_t(out_param_ptr_t&& other) WI_NOEXCEPT :
                wrapper(other.wrapper),
                pRaw(other.pRaw)
            {
                WI_ASSERT(other.replace);
                other.replace = false;
            }

            operator Tcast()
            {
                WI_ASSERT(replace);
                return reinterpret_cast<Tcast>(&pRaw);
            }

            ~out_param_ptr_t()
            {
                if (replace)
                {
                    attach_to_smart_pointer(wrapper, pRaw);
                }
            }

            out_param_ptr_t(out_param_ptr_t const &other) = delete;
            out_param_ptr_t &operator=(out_param_ptr_t const &other) = delete;
        };
    } // details
      /// @endcond

      /** Use to retrieve raw out parameter pointers into smart pointers that do not support the '&' operator.
      This avoids multi-step handling of a raw resource to establish the smart pointer.
      Example: `GetFoo(out_param(foo));` */
    template <typename T>
    details::out_param_t<T> out_param(T& p)
    {
        return details::out_param_t<T>(p);
    }

    /** Use to retrieve raw out parameter pointers (with a required cast) into smart pointers that do not support the '&' operator.
    Use only when the smart pointer's &handle is not equal to the output type a function requires, necessitating a cast.
    Example: `wil::out_param_ptr<PSECURITY_DESCRIPTOR*>(securityDescriptor)` */
    template <typename Tcast, typename T>
    details::out_param_ptr_t<Tcast, T> out_param_ptr(T& p)
    {
        return details::out_param_ptr_t<Tcast, T>(p);
    }

    /** Use unique_struct to define an RAII type for a trivial struct that references resources that must be cleaned up.
    Unique_struct wraps a trivial struct using a custom clean up function and, optionally, custom initializer function. If no custom initialier function is defined in the template
    then ZeroMemory is used.
    Unique_struct is modeled off of std::unique_ptr. However, unique_struct inherits from the defined type instead of managing the struct through a private member variable.

    If the type you're wrapping is a system type, you can share the code by declaring it in this file (Resource.h). Send requests to wildisc.
    Otherwise, if the type is local to your project, declare it locally.
    @tparam struct_t The struct you want to manage
    @tparam close_fn_t The type of the function to clean up the struct. Takes one parameter: a pointer of struct_t. Return values are ignored.
    @tparam close_fn The function of type close_fn_t. This is called in the destructor and reset functions.
    @tparam init_fn_t Optional:The type of the function to initialize the struct.  Takes one parameter: a pointer of struct_t. Return values are ignored.
    @tparam init_fn Optional:The function of type init_fn_t. This is called in the constructor, reset, and release functions. The default is ZeroMemory to initialize the struct.

    Defined using the default zero memory initializer
    ~~~
    typedef wil::unique_struct<PROPVARIANT, decltype(&::PropVariantClear), ::PropVariantClear> unique_prop_variant_default_init;

    unique_prop_variant_default_init propvariant;
    SomeFunction(&propvariant);
    ~~~

    Defined using a custom initializer
    ~~~
    typedef wil::unique_struct<PROPVARIANT, decltype(&::PropVariantClear), ::PropVariantClear, decltype(&::PropVariantInit), ::PropVariantInit> unique_prop_variant;

    unique_prop_variant propvariant;
    SomeFunction(&propvariant);
    ~~~
    */
    template <typename struct_t, typename close_fn_t, close_fn_t close_fn, typename init_fn_t = wistd::nullptr_t, init_fn_t init_fn = wistd::nullptr_t()>
    class unique_struct : public struct_t
    {
        using closer = details::close_invoker<close_fn_t, close_fn, struct_t*>;
    public:
        //! Initializes the managed struct using the user-provided initialization function, or ZeroMemory if no function is specified
        unique_struct()
        {
            call_init(use_default_init_fn());
        }

        //! Takes ownership of the struct by doing a shallow copy. Must explicitly be type struct_t
        explicit unique_struct(const struct_t& other) WI_NOEXCEPT :
        struct_t(other)
        {}

        //! Initializes the managed struct by taking the ownership of the other managed struct
        //! Then resets the other managed struct by calling the custom close function
        unique_struct(unique_struct&& other) WI_NOEXCEPT :
        struct_t(other.release())
        {}

        //! Resets this managed struct by calling the custom close function and takes ownership of the other managed struct
        //! Then resets the other managed struct by calling the custom close function
        unique_struct & operator=(unique_struct&& other) WI_NOEXCEPT
        {
            if (this != wistd::addressof(other))
            {
                reset(other.release());
            }
            return *this;
        }

        //! Calls the custom close function
        ~unique_struct() WI_NOEXCEPT
        {
            closer::close(this);
        }

        void reset(const unique_struct&) = delete;

        //! Resets this managed struct by calling the custom close function and begins management of the other struct
        void reset(const struct_t& other) WI_NOEXCEPT
        {
            closer::close_reset(this);
            struct_t::operator=(other);
        }

        //! Resets this managed struct by calling the custom close function
        //! Then initializes this managed struct using the user-provided initialization function, or ZeroMemory if no function is specified
        void reset() WI_NOEXCEPT
        {
            closer::close(this);
            call_init(use_default_init_fn());
        }

        void swap(struct_t&) = delete;

        //! Swaps the managed structs
        void swap(unique_struct& other) WI_NOEXCEPT
        {
            struct_t self(*this);
            struct_t::operator=(other);
            *(other.addressof()) = self;
        }

        //! Returns the managed struct
        //! Then initializes this managed struct using the user-provided initialization function, or ZeroMemory if no function is specified
        struct_t release() WI_NOEXCEPT
        {
            struct_t value(*this);
            call_init(use_default_init_fn());
            return value;
        }

        //! Returns address of the managed struct
        struct_t * addressof() WI_NOEXCEPT
        {
            return this;
        }

        //! Resets this managed struct by calling the custom close function
        //! Then initializes this managed struct using the user-provided initialization function, or ZeroMemory if no function is specified
        //! Returns address of the managed struct
        struct_t * reset_and_addressof() WI_NOEXCEPT
        {
            reset();
            return this;
        }

        unique_struct(const unique_struct&) = delete;
        unique_struct& operator=(const unique_struct&) = delete;
        unique_struct& operator=(const struct_t&) = delete;

    private:
        typedef typename wistd::is_same<init_fn_t, wistd::nullptr_t>::type use_default_init_fn;

        void call_init(wistd::true_type)
        {
            RtlZeroMemory(this, sizeof(*this));
        }

        void call_init(wistd::false_type)
        {
            init_fn(this);
        }
    };

    struct empty_deleter
    {
        template <typename T>
        void operator()(_Pre_opt_valid_ _Frees_ptr_opt_ T) const
        {
        }
    };

    /** unique_any_array_ptr is a RAII type for managing conformant arrays that need to be freed and have elements that may need to be freed.
    The intented use for this RAII type would be to capture out params from API like IPropertyValue::GetStringArray.
    This class also maintains the size of the array, so it can iterate over the members and deallocate them before it deallocates the base array pointer.

    If the type you're wrapping is a system type, you can share the code by declaring it in this file (Resource.h). Send requests to wildisc.
    Otherwise, if the type is local to your project, declare it locally.

    @tparam ValueType: The type of array you want to manage.
    @tparam ArrayDeleter: The type of the function to clean up the array. Takes one parameter of type T[] or T*. Return values are ignored. This is called in the destructor and reset functions.
    @tparam ElementDeleter: The type of the function to clean up the array elements. Takes one parameter of type T. Return values are ignored. This is called in the destructor and reset functions.

    ~~~
    void GetSomeArray(_Out_ size_t*, _Out_ NOTMYTYPE**);

    struct not_my_deleter
    {
    void operator()(NOTMYTYPE p) const
    {
    destroy(p);
    }
    };

    wil::unique_any_array_ptr<NOTMYTYPE, ::CoTaskMemFree, not_my_deleter> myArray;
    GetSomeArray(myArray.size_address(), &myArray);
    ~~~ */
    template <typename ValueType, typename ArrayDeleter, typename ElementDeleter = empty_deleter>
    class unique_any_array_ptr
    {
    public:
        typedef ValueType value_type;
        typedef size_t size_type;
        typedef ptrdiff_t difference_type;
        typedef ValueType *pointer;
        typedef const ValueType *const_pointer;
        typedef ValueType& reference;
        typedef const ValueType& const_reference;

        typedef ValueType* iterator;
        typedef const ValueType* const_iterator;

        unique_any_array_ptr() = default;
        unique_any_array_ptr(const unique_any_array_ptr&) = delete;
        unique_any_array_ptr& operator=(const unique_any_array_ptr&) = delete;

        unique_any_array_ptr(wistd::nullptr_t) WI_NOEXCEPT
        {
        }

        unique_any_array_ptr& operator=(wistd::nullptr_t) WI_NOEXCEPT
        {
            reset();
            return *this;
        }

        unique_any_array_ptr(pointer ptr, size_t size) WI_NOEXCEPT : m_ptr(ptr), m_size(size)
        {
        }

        unique_any_array_ptr(unique_any_array_ptr&& other) WI_NOEXCEPT : m_ptr(other.m_ptr), m_size(other.m_size)
        {
            other.m_ptr = nullptr;
            other.m_size = size_type{};
        }

        unique_any_array_ptr& operator=(unique_any_array_ptr&& other) WI_NOEXCEPT
        {
            if (this != wistd::addressof(other))
            {
                reset();
                swap(other);
            }
            return *this;
        }

        ~unique_any_array_ptr() WI_NOEXCEPT
        {
            reset();
        }

        void swap(unique_any_array_ptr& other) WI_NOEXCEPT
        {
            auto ptr = m_ptr;
            auto size = m_size;
            m_ptr = other.m_ptr;
            m_size = other.m_size;
            other.m_ptr = ptr;
            other.m_size = size;
        }

        WI_NODISCARD iterator begin() WI_NOEXCEPT
        {
            return (iterator(m_ptr));
        }

        WI_NODISCARD const_iterator begin() const WI_NOEXCEPT
        {
            return (const_iterator(m_ptr));
        }

        WI_NODISCARD iterator end() WI_NOEXCEPT
        {
            return (iterator(m_ptr + m_size));
        }

        WI_NODISCARD const_iterator end() const WI_NOEXCEPT
        {
            return (const_iterator(m_ptr + m_size));
        }

        WI_NODISCARD const_iterator cbegin() const WI_NOEXCEPT
        {
            return (begin());
        }

        WI_NODISCARD const_iterator cend() const WI_NOEXCEPT
        {
            return (end());
        }

        WI_NODISCARD size_type size() const WI_NOEXCEPT
        {
            return (m_size);
        }

        WI_NODISCARD bool empty() const WI_NOEXCEPT
        {
            return (size() == size_type{});
        }

        WI_NODISCARD reference operator[](size_type position)
        {
            WI_ASSERT(position < m_size);
            _Analysis_assume_(position < m_size);
            return (m_ptr[position]);
        }

        WI_NODISCARD const_reference operator[](size_type position) const
        {
            WI_ASSERT(position < m_size);
            _Analysis_assume_(position < m_size);
            return (m_ptr[position]);
        }

        WI_NODISCARD reference front()
        {
            WI_ASSERT(!empty());
            return (m_ptr[0]);
        }

        WI_NODISCARD const_reference front() const
        {
            WI_ASSERT(!empty());
            return (m_ptr[0]);
        }

        WI_NODISCARD reference back()
        {
            WI_ASSERT(!empty());
            return (m_ptr[m_size - 1]);
        }

        WI_NODISCARD const_reference back() const
        {
            WI_ASSERT(!empty());
            return (m_ptr[m_size - 1]);
        }

        WI_NODISCARD ValueType* data() WI_NOEXCEPT
        {
            return (m_ptr);
        }

        WI_NODISCARD const ValueType* data() const WI_NOEXCEPT
        {
            return (m_ptr);
        }

        WI_NODISCARD pointer get() const WI_NOEXCEPT
        {
            return m_ptr;
        }

        WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
        {
            return (m_ptr != pointer());
        }

        pointer release() WI_NOEXCEPT
        {
            auto result = m_ptr;
            m_ptr = nullptr;
            m_size = size_type{};
            return result;
        }

        void reset() WI_NOEXCEPT
        {
            if (m_ptr)
            {
                reset_array(ElementDeleter());
                ArrayDeleter()(m_ptr);
                m_ptr = nullptr;
                m_size = size_type{};
            }
        }

        void reset(pointer ptr, size_t size) WI_NOEXCEPT
        {
            reset();
            m_ptr = ptr;
            m_size = size;
        }

        pointer* addressof() WI_NOEXCEPT
        {
            return &m_ptr;
        }

        pointer* put() WI_NOEXCEPT
        {
            reset();
            return addressof();
        }

        pointer* operator&() WI_NOEXCEPT
        {
            return put();
        }

        size_type* size_address() WI_NOEXCEPT
        {
            return &m_size;
        }

        template <typename TSize>
        struct size_address_ptr
        {
            unique_any_array_ptr& wrapper;
            TSize size{};
            bool replace = true;

            size_address_ptr(_Inout_ unique_any_array_ptr& output) :
                wrapper(output)
            {
            }

            size_address_ptr(size_address_ptr&& other) WI_NOEXCEPT :
                wrapper(other.wrapper),
                size(other.size)
            {
                WI_ASSERT(other.replace);
                other.replace = false;
            }

            operator TSize*()
            {
                WI_ASSERT(replace);
                return &size;
            }

            ~size_address_ptr()
            {
                if (replace)
                {
                    *wrapper.size_address() = static_cast<size_type>(size);
                }
            }

            size_address_ptr(size_address_ptr const &other) = delete;
            size_address_ptr &operator=(size_address_ptr const &other) = delete;
        };

        template <typename T>
        size_address_ptr<T> size_address() WI_NOEXCEPT
        {
            return size_address_ptr<T>(*this);
        }

    private:
        pointer m_ptr = nullptr;
        size_type m_size{};

        void reset_array(const empty_deleter&)
        {
        }

        template <typename T>
        void reset_array(const T& deleter)
        {
            for (auto& element : make_range(m_ptr, m_size))
            {
                deleter(element);
            }
        }
    };

    // forward declaration
    template <typename T, typename err_policy>
    class com_ptr_t;

    /// @cond
    namespace details
    {
        template <typename UniqueAnyType>
        struct unique_any_array_deleter
        {
            template <typename T>
            void operator()(_Pre_opt_valid_ _Frees_ptr_opt_ T* p) const
            {
                UniqueAnyType::policy::close_reset(p);
            }
        };

        template <typename close_fn_t, close_fn_t close_fn>
        struct unique_struct_array_deleter
        {
            template <typename T>
            void operator()(_Pre_opt_valid_ _Frees_ptr_opt_ T& p) const
            {
                close_invoker<close_fn_t, close_fn, T*>::close(&p);
            }
        };

        struct com_unknown_deleter
        {
            template <typename T>
            void operator()(_Pre_opt_valid_ _Frees_ptr_opt_ T* p) const
            {
                if (p)
                {
                    p->Release();
                }
            }
        };

        template <class T>
        struct element_traits
        {
            typedef empty_deleter deleter;
            typedef T type;
        };

        template <typename storage_t>
        struct element_traits<unique_any_t<storage_t>>
        {
            typedef unique_any_array_deleter<unique_any_t<storage_t>> deleter;
            typedef typename unique_any_t<storage_t>::pointer type;
        };

        template <typename T, typename err_policy>
        struct element_traits<com_ptr_t<T, err_policy>>
        {
            typedef com_unknown_deleter deleter;
            typedef T* type;
        };

        template <typename struct_t, typename close_fn_t, close_fn_t close_fn, typename init_fn_t, init_fn_t init_fn>
        struct element_traits<unique_struct<struct_t, close_fn_t, close_fn, init_fn_t, init_fn>>
        {
            typedef unique_struct_array_deleter<close_fn_t, close_fn> deleter;
            typedef struct_t type;
        };
    }
    /// @endcond

    template <typename T, typename ArrayDeleter>
    using unique_array_ptr = unique_any_array_ptr<typename details::element_traits<T>::type, ArrayDeleter, typename details::element_traits<T>::deleter>;

    /** Adapter for single-parameter 'free memory' for `wistd::unique_ptr`.
    This struct provides a standard wrapper for calling a platform function to deallocate memory held by a
    `wistd::unique_ptr`, making declaring them as easy as declaring wil::unique_any<>.

    Consider this adapter in preference to `wil::unique_any<>` when the returned type is really a pointer or an
    array of items; `wistd::unique_ptr<>` exposes `operator->()` and `operator[]` for array-typed things safely.
    ~~~~
    EXTERN_C VOID WINAPI MyDllFreeMemory(void* p);
    EXTERN_C HRESULT MyDllGetString(_Outptr_ PWSTR* pString);
    EXTERN_C HRESULT MyDllGetThing(_In_ PCWSTR pString, _Outptr_ PMYSTRUCT* ppThing);
    template<typename T>
    using unique_mydll_ptr = wistd::unique_ptr<T, wil::function_deleter<decltype(&MyDllFreeMemory), MyDllFreeMemory>>;
    HRESULT Test()
    {
    unique_mydll_ptr<WCHAR[]> dllString;
    unique_mydll_ptr<MYSTRUCT> thing;
    RETURN_IF_FAILED(MyDllGetString(wil::out_param(dllString)));
    RETURN_IF_FAILED(MyDllGetThing(dllString.get(), wil::out_param(thing)));
    if (thing->Member)
    {
    // ...
    }
    return S_OK;
    }
    ~~~~ */
    template<typename Q, Q TDeleter> struct function_deleter
    {
        template<typename T> void operator()(_Frees_ptr_opt_ T* toFree) const
        {
            TDeleter(toFree);
        }
    };

    /** Use unique_com_token to define an RAII type for a token-based resource that is managed by a COM interface.
    By comparison, unique_any_t has the requirement that the close function must be static. This works for functions
    such as CloseHandle(), but for any resource cleanup function that relies on a more complex interface,
    unique_com_token can be used.

    @tparam interface_t A COM interface pointer that will manage this resource type.
    @tparam token_t The token type that relates to the COM interface management functions.
    @tparam close_fn_t The type of the function that is called when the resource is destroyed.
    @tparam close_fn The function used to destroy the associated resource. This function should have the signature void(interface_t* source, token_t token).
    @tparam invalid_token Optional:An invalid token value. Defaults to default-constructed token_t().

    Example
    ~~~
    void __stdcall MyInterfaceCloseFunction(IMyInterface* source, DWORD token)
    {
    source->MyCloseFunction(token);
    }
    using unique_my_interface_token = wil::unique_com_token<IMyInterface, DWORD, decltype(MyInterfaceCloseFunction), MyInterfaceCloseFunction, 0xFFFFFFFF>;
    ~~~ */
    template <typename interface_t, typename token_t, typename close_fn_t, close_fn_t close_fn, token_t invalid_token = token_t()>
    class unique_com_token
    {
    public:
        unique_com_token() = default;

        unique_com_token(_In_opt_ interface_t* source, token_t token = invalid_token) WI_NOEXCEPT
        {
            reset(source, token);
        }

        unique_com_token(unique_com_token&& other) WI_NOEXCEPT : m_source(other.m_source), m_token(other.m_token)
        {
            other.m_source = nullptr;
            other.m_token = invalid_token;
        }

        unique_com_token& operator=(unique_com_token&& other) WI_NOEXCEPT
        {
            if (this != wistd::addressof(other))
            {
                reset();
                m_source = other.m_source;
                m_token = other.m_token;

                other.m_source = nullptr;
                other.m_token = invalid_token;
            }
            return *this;
        }

        ~unique_com_token() WI_NOEXCEPT
        {
            reset();
        }

        //! Determine if the underlying source and token are valid
        WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
        {
            return (m_token != invalid_token) && m_source;
        }

        //! Associates a new source and releases the existing token if valid
        void associate(_In_opt_ interface_t* source) WI_NOEXCEPT
        {
            reset(source, invalid_token);
        }

        //! Assigns a new source and token
        void reset(_In_opt_ interface_t* source, token_t token) WI_NOEXCEPT
        {
            WI_ASSERT(source || (token == invalid_token));

            // Determine if we need to call the close function on our previous token.
            if (m_token != invalid_token)
            {
                if ((m_source != source) || (m_token != token))
                {
                    wistd::invoke(close_fn, m_source, m_token);
                }
            }

            m_token = token;

            // Assign our new source and manage the reference counts
            if (m_source != source)
            {
                auto oldSource = m_source;
                m_source = source;

                if (m_source)
                {
                    m_source->AddRef();
                }

                if (oldSource)
                {
                    oldSource->Release();
                }
            }
        }

        //! Assigns a new token without modifying the source; associate must be called first
        void reset(token_t token) WI_NOEXCEPT
        {
            reset(m_source, token);
        }

        //! Closes the token and the releases the reference to the source
        void reset() WI_NOEXCEPT
        {
            reset(nullptr, invalid_token);
        }

        //! Exchanges values with another managed token
        void swap(unique_com_token& other) WI_NOEXCEPT
        {
            wistd::swap_wil(m_source, other.m_source);
            wistd::swap_wil(m_token, other.m_token);
        }

        //! Releases the held token to the caller without closing it and releases the reference to the source.
        //! Requires that the associated COM interface be kept alive externally or the released token may be invalidated
        token_t release() WI_NOEXCEPT
        {
            auto token = m_token;
            m_token = invalid_token;
            reset();
            return token;
        }

        //! Returns address of the managed token; associate must be called first
        token_t* addressof() WI_NOEXCEPT
        {
            WI_ASSERT(m_source);
            return &m_token;
        }

        //! Releases the held token and allows attaching a new token; associate must be called first
        token_t* put() WI_NOEXCEPT
        {
            reset(invalid_token);
            return addressof();
        }

        //! Releases the held token and allows attaching a new token; associate must be called first
        token_t* operator&() WI_NOEXCEPT
        {
            return put();
        }

        //! Retrieves the token
        WI_NODISCARD token_t get() const WI_NOEXCEPT
        {
            return m_token;
        }

        unique_com_token(const unique_com_token&) = delete;
        unique_com_token& operator=(const unique_com_token&) = delete;

    private:
        interface_t* m_source = nullptr;
        token_t m_token = invalid_token;
    };

    /** Use unique_com_call to define an RAII type that demands a particular parameter-less method be called on a COM interface.
    This allows implementing an RAII type that can call a Close() method (think IClosable) or a SetSite(nullptr)
    method (think IObjectWithSite) or some other method when a basic interface call is required as part of the RAII contract.
    see wil::com_set_site in wil\com.h for the IObjectWithSite support.

    @tparam interface_t A COM interface pointer that provides context to make the call.
    @tparam close_fn_t The type of the function that is called to invoke the method.
    @tparam close_fn The function used to invoke the interface method.  This function should have the signature void(interface_t* source).

    Example
    ~~~
    void __stdcall CloseIClosable(IClosable* source)
    {
    source->Close();
    }
    using unique_closable_call = wil::unique_com_call<IClosable, decltype(CloseIClosable), CloseIClosable>;
    ~~~ */
    template <typename interface_t, typename close_fn_t, close_fn_t close_fn>
    class unique_com_call
    {
    public:
        unique_com_call() = default;

        explicit unique_com_call(_In_opt_ interface_t* ptr) WI_NOEXCEPT
        {
            reset(ptr);
        }

        unique_com_call(unique_com_call&& other) WI_NOEXCEPT
        {
            m_ptr = other.m_ptr;
            other.m_ptr = nullptr;
        }

        unique_com_call& operator=(unique_com_call&& other) WI_NOEXCEPT
        {
            if (this != wistd::addressof(other))
            {
                reset();
                m_ptr = other.m_ptr;
                other.m_ptr = nullptr;
            }
            return *this;
        }

        ~unique_com_call() WI_NOEXCEPT
        {
            reset();
        }

        //! Assigns an interface to make a given call on
        void reset(_In_opt_ interface_t* ptr = nullptr) WI_NOEXCEPT
        {
            if (ptr != m_ptr)
            {
                auto oldSource = m_ptr;
                m_ptr = ptr;
                if (m_ptr)
                {
                    m_ptr->AddRef();
                }
                if (oldSource)
                {
                    details::close_invoker<close_fn_t, close_fn, interface_t*>::close(oldSource);
                    oldSource->Release();
                }
            }
        }

        //! Exchanges values with another class
        void swap(unique_com_call& other) WI_NOEXCEPT
        {
            wistd::swap_wil(m_ptr, other.m_ptr);
        }

        //! Cancel the interface call that this class was expected to make
        void release() WI_NOEXCEPT
        {
            auto ptr = m_ptr;
            m_ptr = nullptr;
            if (ptr)
            {
                ptr->Release();
            }
        }

        //! Returns true if the call this class was expected to make is still outstanding
        WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
        {
            return (m_ptr != nullptr);
        }

        //! Returns address of the internal interface
        interface_t** addressof() WI_NOEXCEPT
        {
            return &m_ptr;
        }

        //! Releases the held interface (first performing the interface call if required)
        //! and allows attaching a new interface
        interface_t** put() WI_NOEXCEPT
        {
            reset();
            return addressof();
        }

        //! Releases the held interface (first performing the interface call if required)
        //! and allows attaching a new interface
        interface_t** operator&() WI_NOEXCEPT
        {
            return put();
        }

        unique_com_call(const unique_com_call&) = delete;
        unique_com_call& operator=(const unique_com_call&) = delete;

    private:
        interface_t* m_ptr = nullptr;
    };


    /** Use unique_call to define an RAII type that demands a particular parameter-less global function be called.
    This allows implementing a RAII types that can call methods like CoUninitialize.

    @tparam close_fn_t The type of the function that is called to invoke the call.
    @tparam close_fn The function used to invoke the call.  This function should have the signature void().
    @tparam default_value Determines whether the unique_call is active or inactive when default-constructed or reset.

    Example
    ~~~
    void __stdcall CoUninitializeFunction()
    {
    ::CoUninitialize();
    }
    using unique_couninitialize_call = wil::unique_call<decltype(CoUninitializeFunction), CoUninitializeFunction>;
    ~~~ */
    template <typename close_fn_t, close_fn_t close_fn, bool default_value = true>
    class unique_call
    {
    public:
        unique_call() = default;

        explicit unique_call(bool call) WI_NOEXCEPT : m_call(call)
        {
        }

        unique_call(unique_call&& other) WI_NOEXCEPT
        {
            m_call = other.m_call;
            other.m_call = false;
        }

        unique_call& operator=(unique_call&& other) WI_NOEXCEPT
        {
            if (this != wistd::addressof(other))
            {
                reset();
                m_call = other.m_call;
                other.m_call = false;
            }
            return *this;
        }

        ~unique_call() WI_NOEXCEPT
        {
            reset();
        }

        //! Assigns a new ptr and token
        void reset() WI_NOEXCEPT
        {
            auto call = m_call;
            m_call = false;
            if (call)
            {
                close_fn();
            }
        }

        //! Exchanges values with raii class
        void swap(unique_call& other) WI_NOEXCEPT
        {
            wistd::swap_wil(m_call, other.m_call);
        }

        //! Make the interface call that was expected of this class
        void activate() WI_NOEXCEPT
        {
            m_call = true;
        }

        //! Do not make the interface call that was expected of this class
        void release() WI_NOEXCEPT
        {
            m_call = false;
        }

        //! Returns true if the call that was expected is still outstanding
        WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
        {
            return m_call;
        }

        unique_call(const unique_call&) = delete;
        unique_call& operator=(const unique_call&) = delete;

    private:
        bool m_call = default_value;
    };

    // str_raw_ptr is an overloaded function that retrieves a const pointer to the first character in a string's buffer.
    // Overloads in this file support any string that is implicitly convertible to a PCWSTR, HSTRING, and any unique_any_t
    // that points to any other supported type (this covers unique_hstring, unique_cotaskmem_string, and similar).
    // An overload for std::wstring is available in stl.h.
    inline PCWSTR str_raw_ptr(PCWSTR str)
    {
        return str;
    }

    template <typename T>
    PCWSTR str_raw_ptr(const unique_any_t<T>& ua)
    {
        return str_raw_ptr(ua.get());
    }

#if !defined(__WIL_MIN_KERNEL) && !defined(WIL_KERNEL_MODE)
    namespace details
    {
        // Forward declaration
        template<typename string_type> struct string_maker;

        // Concatenate any number of strings together and store it in an automatically allocated string.  If a string is present
        // in the input buffer, it is overwritten.
        template <typename string_type>
        HRESULT str_build_nothrow(string_type& result, _In_reads_(strCount) PCWSTR* strList, size_t strCount)
        {
            size_t lengthRequiredWithoutNull{};
            for (auto& string : make_range(strList, strCount))
            {
                lengthRequiredWithoutNull += string ? wcslen(string) : 0;
            }

            details::string_maker<string_type> maker;
            RETURN_IF_FAILED(maker.make(nullptr, lengthRequiredWithoutNull));

            auto buffer = maker.buffer();
            auto bufferEnd = buffer + lengthRequiredWithoutNull + 1;
            for (auto& string : make_range(strList, strCount))
            {
                if (string)
                {
                    RETURN_IF_FAILED(StringCchCopyExW(buffer, (bufferEnd - buffer), string, &buffer, nullptr, STRSAFE_IGNORE_NULLS));
                }
            }

            result = maker.release();
            return S_OK;
        }

        // NOTE: 'Strings' must all be PCWSTR, or convertible to PCWSTR, but C++ doesn't allow us to express that cleanly
        template <typename string_type, typename... Strings>
        HRESULT str_build_nothrow(string_type& result, Strings... strings)
        {
            PCWSTR localStrings[] = { strings... };
            return str_build_nothrow(result, localStrings, sizeof...(Strings));
        }
    }

    // Concatenate any number of strings together and store it in an automatically allocated string.  If a string is present
    // in the input buffer, the remaining strings are appended to it.
    template <typename string_type, typename... strings>
    HRESULT str_concat_nothrow(string_type& buffer, const strings&... str)
    {
        static_assert(sizeof...(str) > 0, "attempting to concatenate no strings");
        return details::str_build_nothrow(buffer, details::string_maker<string_type>::get(buffer), str_raw_ptr(str)...);
    }
#endif // !defined(__WIL_MIN_KERNEL) && !defined(WIL_KERNEL_MODE)

#ifdef WIL_ENABLE_EXCEPTIONS
    // Concatenate any number of strings together and store it in an automatically allocated string.
    template <typename string_type, typename... arguments>
    string_type str_concat(arguments&&... args)
    {
        string_type result{};
        THROW_IF_FAILED(str_concat_nothrow(result, wistd::forward<arguments>(args)...));
        return result;
    }
#endif // WIL_ENABLE_EXCEPTIONS

    // Concatenate any number of strings together and store it in an automatically allocated string.
    template <typename string_type, typename... arguments>
    string_type str_concat_failfast(arguments&&... args)
    {
        string_type result{};
        FAIL_FAST_IF_FAILED(str_concat_nothrow(result, wistd::forward<arguments>(args)...));
        return result;
    }

#if !defined(__WIL_MIN_KERNEL) && !defined(WIL_KERNEL_MODE)
    namespace details
    {
        // Wraps StringCchPrintFExW and stores it in an automatically allocated string.  Takes a buffer followed by the same format arguments
        // that StringCchPrintfExW takes.
        template <typename string_type>
        HRESULT str_vprintf_nothrow(string_type& result, _Printf_format_string_ PCWSTR pszFormat, va_list& argsVL)
        {
            size_t lengthRequiredWithoutNull = _vscwprintf(pszFormat, argsVL);

            string_maker<string_type> maker;
            RETURN_IF_FAILED(maker.make(nullptr, lengthRequiredWithoutNull));

            auto buffer = maker.buffer();
            RETURN_IF_FAILED(StringCchVPrintfExW(buffer, lengthRequiredWithoutNull + 1, nullptr, nullptr, STRSAFE_NULL_ON_FAILURE, pszFormat, argsVL));

            result = maker.release();
            return S_OK;
        }
    }

    // Wraps StringCchPrintFExW and stores it in an automatically allocated string.  Takes a buffer followed by the same format arguments
    // that StringCchPrintfExW takes.
    template <typename string_type>
    HRESULT str_printf_nothrow(string_type& result, _Printf_format_string_ PCWSTR pszFormat, ...)
    {
        va_list argsVL;
        va_start(argsVL, pszFormat);
        auto hr = details::str_vprintf_nothrow(result, pszFormat, argsVL);
        va_end(argsVL);
        return hr;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    // Wraps StringCchPrintFExW and stores it in an automatically allocated string.  Takes a buffer followed by the same format arguments
    // that StringCchPrintfExW takes.
    template <typename string_type>
    string_type str_printf(_Printf_format_string_ PCWSTR pszFormat, ...)
    {
        string_type result{};
        va_list argsVL;
        va_start(argsVL, pszFormat);
        auto hr = details::str_vprintf_nothrow(result, pszFormat, argsVL);
        va_end(argsVL);
        THROW_IF_FAILED(hr);
        return result;
    }
#endif // WIL_ENABLE_EXCEPTIONS

    // Wraps StringCchPrintFExW and stores it in an automatically allocated string.  Takes a buffer followed by the same format arguments
    // that StringCchPrintfExW takes.
    template <typename string_type>
    string_type str_printf_failfast(_Printf_format_string_ PCWSTR pszFormat, ...)
    {
        string_type result{};
        va_list argsVL;
        va_start(argsVL, pszFormat);
        auto hr = details::str_vprintf_nothrow(result, pszFormat, argsVL);
        va_end(argsVL);
        FAIL_FAST_IF_FAILED(hr);
        return result;
    }
#endif // !defined(__WIL_MIN_KERNEL) && !defined(WIL_KERNEL_MODE)

} // namespace wil
#endif // __WIL_RESOURCE


  // Hash deferral function for unique_any_t
#if (defined(_UNORDERED_SET_) || defined(_UNORDERED_MAP_)) && !defined(__WIL_RESOURCE_UNIQUE_HASH)
#define __WIL_RESOURCE_UNIQUE_HASH
namespace std
{
    template <typename storage_t>
    struct hash<wil::unique_any_t<storage_t>>
    {
        WI_NODISCARD size_t operator()(wil::unique_any_t<storage_t> const &val) const
        {
            return (hash<typename wil::unique_any_t<storage_t>::pointer>()(val.get()));
        }
    };
}
#endif

// shared_any and weak_any implementation using <memory> STL header
#if defined(_MEMORY_) && defined(WIL_ENABLE_EXCEPTIONS) && !defined(WIL_RESOURCE_STL) && !defined(RESOURCE_SUPPRESS_STL)
#define WIL_RESOURCE_STL
namespace wil {

    template <typename storage_t>
    class weak_any;

    /// @cond
    namespace details
    {
        // This class provides the pointer storage behind the implementation of shared_any_t utilizing the given
        // resource_policy.  It is separate from shared_any_t to allow a type-specific specialization class to plug
        // into the inheritance chain between shared_any_t and shared_storage.  This allows classes like shared_event
        // to be a shared_any formed class, but also expose methods like SetEvent directly.

        template <typename UniqueT>
        class shared_storage
        {
        protected:
            typedef UniqueT unique_t;
            typedef typename unique_t::policy policy;
            typedef typename policy::pointer_storage pointer_storage;
            typedef typename policy::pointer pointer;
            typedef shared_storage<unique_t> base_storage;

        public:
            shared_storage() = default;

            explicit shared_storage(pointer_storage ptr)
            {
                if (policy::is_valid(ptr))
                {
                    m_ptr = std::make_shared<unique_t>(unique_t(ptr));      // unique_t on the stack to prevent leak on throw
                }
            }

            shared_storage(unique_t &&other)
            {
                if (other)
                {
                    m_ptr = std::make_shared<unique_t>(wistd::move(other));
                }
            }

            shared_storage(const shared_storage &other) WI_NOEXCEPT :
            m_ptr(other.m_ptr)
            {
            }

            shared_storage& operator=(const shared_storage &other) WI_NOEXCEPT
            {
                m_ptr = other.m_ptr;
                return *this;
            }

            shared_storage(shared_storage &&other) WI_NOEXCEPT :
            m_ptr(wistd::move(other.m_ptr))
            {
            }

            shared_storage(std::shared_ptr<unique_t> const &ptr) :
                m_ptr(ptr)
            {
            }

            WI_NODISCARD bool is_valid() const WI_NOEXCEPT
            {
                return (m_ptr && m_ptr->is_valid());
            }

            void reset(pointer_storage ptr = policy::invalid_value())
            {
                if (policy::is_valid(ptr))
                {
                    m_ptr = std::make_shared<unique_t>(unique_t(ptr));      // unique_t on the stack to prevent leak on throw
                }
                else
                {
                    m_ptr = nullptr;
                }
            }

            void reset(unique_t &&other)
            {
                m_ptr = std::make_shared<unique_t>(wistd::move(other));
            }

            void reset(wistd::nullptr_t) WI_NOEXCEPT
            {
                static_assert(wistd::is_same<typename policy::pointer_invalid, wistd::nullptr_t>::value, "reset(nullptr): valid only for handle types using nullptr as the invalid value");
                reset();
            }

            template <typename allow_t = typename policy::pointer_access, typename wistd::enable_if<!wistd::is_same<allow_t, details::pointer_access_none>::value, int>::type = 0>
            WI_NODISCARD pointer get() const WI_NOEXCEPT
            {
                return (m_ptr ? m_ptr->get() : policy::invalid_value());
            }

            template <typename allow_t = typename policy::pointer_access, typename wistd::enable_if<wistd::is_same<allow_t, details::pointer_access_all>::value, int>::type = 0>
            pointer_storage *addressof()
            {
                if (!m_ptr)
                {
                    m_ptr = std::make_shared<unique_t>();
                }
                return m_ptr->addressof();
            }

            WI_NODISCARD long int use_count() const WI_NOEXCEPT
            {
                return m_ptr.use_count();
            }

        protected:
            void replace(shared_storage &&other) WI_NOEXCEPT
            {
                m_ptr = wistd::move(other.m_ptr);
            }

        private:
            template <typename storage_t>
            friend class ::wil::weak_any;

            std::shared_ptr<unique_t> m_ptr;
        };
    }
    /// @endcond

    // This class when paired with shared_storage and an optional type-specific specialization class implements
    // the same interface as STL's shared_ptr<> for resource handle types.  It is both copyable and movable, supporting
    // weak references and automatic closure of the handle upon release of the last shared_any.

    template <typename storage_t>
    class shared_any_t : public storage_t
    {
    public:
        typedef typename storage_t::policy policy;
        typedef typename policy::pointer_storage pointer_storage;
        typedef typename policy::pointer pointer;
        typedef typename storage_t::unique_t unique_t;

        // default and forwarding constructor: forwards default, all 'explicit' and multi-arg constructors to the base class
        template <typename... args_t>
        explicit shared_any_t(args_t&&... args)
            __WI_NOEXCEPT_((wistd::is_nothrow_constructible_v<storage_t, args_t...>)) :
            storage_t(wistd::forward<args_t>(args)...)
        {
        }

        shared_any_t(wistd::nullptr_t) WI_NOEXCEPT
        {
            static_assert(wistd::is_same<typename policy::pointer_invalid, wistd::nullptr_t>::value, "nullptr constructor: valid only for handle types using nullptr as the invalid value");
        }

        shared_any_t(shared_any_t &&other) WI_NOEXCEPT :
        storage_t(wistd::move(other))
        {
        }

        shared_any_t(const shared_any_t &other) WI_NOEXCEPT :
            storage_t(other)
        {
        }

        shared_any_t& operator=(shared_any_t &&other) WI_NOEXCEPT
        {
            if (this != wistd::addressof(other))
            {
                storage_t::replace(wistd::move(static_cast<typename storage_t::base_storage &>(other)));
            }
            return (*this);
        }

        shared_any_t& operator=(const shared_any_t& other) WI_NOEXCEPT
        {
            storage_t::operator=(other);
            return (*this);
        }

        shared_any_t(unique_t &&other) :
            storage_t(wistd::move(other))
        {
        }

        shared_any_t& operator=(unique_t &&other)
        {
            storage_t::reset(wistd::move(other));
            return (*this);
        }

        shared_any_t& operator=(wistd::nullptr_t) WI_NOEXCEPT
        {
            static_assert(wistd::is_same<typename policy::pointer_invalid, wistd::nullptr_t>::value, "nullptr assignment: valid only for handle types using nullptr as the invalid value");
            storage_t::reset();
            return (*this);
        }

        void swap(shared_any_t &other) WI_NOEXCEPT
        {
            shared_any_t self(wistd::move(*this));
            operator=(wistd::move(other));
            other = wistd::move(self);
        }

        WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
        {
            return storage_t::is_valid();
        }

        pointer_storage *put()
        {
            static_assert(wistd::is_same<typename policy::pointer_access, details::pointer_access_all>::value, "operator & is not available for this handle");
            storage_t::reset();
            return storage_t::addressof();
        }

        pointer_storage *operator&()
        {
            return put();
        }

        WI_NODISCARD pointer get() const WI_NOEXCEPT
        {
            static_assert(!wistd::is_same<typename policy::pointer_access, details::pointer_access_none>::value, "get(): the raw handle value is not available for this resource class");
            return storage_t::get();
        }

        // The following functions are publicly exposed by their inclusion in the base class

        // void reset(pointer_storage ptr = policy::invalid_value()) WI_NOEXCEPT
        // void reset(wistd::nullptr_t) WI_NOEXCEPT
        // pointer_storage *addressof() WI_NOEXCEPT                                     // (note: not exposed for opaque resource types)
    };

    template <typename unique_t>
    void swap(shared_any_t<unique_t>& left, shared_any_t<unique_t>& right) WI_NOEXCEPT
    {
        left.swap(right);
    }

    template <typename unique_t>
    bool operator==(const shared_any_t<unique_t>& left, const shared_any_t<unique_t>& right) WI_NOEXCEPT
    {
        return (left.get() == right.get());
    }

    template <typename unique_t>
    bool operator==(const shared_any_t<unique_t>& left, wistd::nullptr_t) WI_NOEXCEPT
    {
        static_assert(wistd::is_same<typename shared_any_t<unique_t>::policy::pointer_invalid, wistd::nullptr_t>::value, "the resource class does not use nullptr as an invalid value");
        return !left;
    }

    template <typename unique_t>
    bool operator==(wistd::nullptr_t, const shared_any_t<unique_t>& right) WI_NOEXCEPT
    {
        static_assert(wistd::is_same<typename shared_any_t<unique_t>::policy::pointer_invalid, wistd::nullptr_t>::value, "the resource class does not use nullptr as an invalid value");
        return !right;
    }

    template <typename unique_t>
    bool operator!=(const shared_any_t<unique_t>& left, const shared_any_t<unique_t>& right) WI_NOEXCEPT
    {
        return (!(left.get() == right.get()));
    }

    template <typename unique_t>
    bool operator!=(const shared_any_t<unique_t>& left, wistd::nullptr_t) WI_NOEXCEPT
    {
        static_assert(wistd::is_same<typename shared_any_t<unique_t>::policy::pointer_invalid, wistd::nullptr_t>::value, "the resource class does not use nullptr as an invalid value");
        return !!left;
    }

    template <typename unique_t>
    bool operator!=(wistd::nullptr_t, const shared_any_t<unique_t>& right) WI_NOEXCEPT
    {
        static_assert(wistd::is_same<typename shared_any_t<unique_t>::policy::pointer_invalid, wistd::nullptr_t>::value, "the resource class does not use nullptr as an invalid value");
        return !!right;
    }

    template <typename unique_t>
    bool operator<(const shared_any_t<unique_t>& left, const shared_any_t<unique_t>& right) WI_NOEXCEPT
    {
        return (left.get() < right.get());
    }

    template <typename unique_t>
    bool operator>=(const shared_any_t<unique_t>& left, const shared_any_t<unique_t>& right) WI_NOEXCEPT
    {
        return (!(left < right));
    }

    template <typename unique_t>
    bool operator>(const shared_any_t<unique_t>& left, const shared_any_t<unique_t>& right) WI_NOEXCEPT
    {
        return (right < left);
    }

    template <typename unique_t>
    bool operator<=(const shared_any_t<unique_t>& left, const shared_any_t<unique_t>& right) WI_NOEXCEPT
    {
        return (!(right < left));
    }


    // This class provides weak_ptr<> support for shared_any<>, bringing the same weak reference counting and lock() acquire semantics
    // to shared_any.

    template <typename SharedT>
    class weak_any
    {
    public:
        typedef SharedT shared_t;

        weak_any() WI_NOEXCEPT
        {
        }

        weak_any(const shared_t &other) WI_NOEXCEPT :
            m_weakPtr(other.m_ptr)
        {
        }

        weak_any(const weak_any &other) WI_NOEXCEPT :
            m_weakPtr(other.m_weakPtr)
        {
        }

        weak_any& operator=(const weak_any &right) WI_NOEXCEPT
        {
            m_weakPtr = right.m_weakPtr;
            return (*this);
        }

        weak_any& operator=(const shared_t &right) WI_NOEXCEPT
        {
            m_weakPtr = right.m_ptr;
            return (*this);
        }

        void reset() WI_NOEXCEPT
        {
            m_weakPtr.reset();
        }

        void swap(weak_any &other) WI_NOEXCEPT
        {
            m_weakPtr.swap(other.m_weakPtr);
        }

        WI_NODISCARD bool expired() const WI_NOEXCEPT
        {
            return m_weakPtr.expired();
        }

        WI_NODISCARD shared_t lock() const WI_NOEXCEPT
        {
            return shared_t(m_weakPtr.lock());
        }

    private:
        std::weak_ptr<typename shared_t::unique_t> m_weakPtr;
    };

    template <typename shared_t>
    void swap(weak_any<shared_t>& left, weak_any<shared_t>& right) WI_NOEXCEPT
    {
        left.swap(right);
    }

    template <typename unique_t>
    using shared_any = shared_any_t<details::shared_storage<unique_t>>;

} // namespace wil
#endif


#if defined(WIL_RESOURCE_STL) && (defined(_UNORDERED_SET_) || defined(_UNORDERED_MAP_)) && !defined(__WIL_RESOURCE_SHARED_HASH)
#define __WIL_RESOURCE_SHARED_HASH
namespace std
{
    template <typename storage_t>
    struct hash<wil::shared_any_t<storage_t>>
    {
        WI_NODISCARD size_t operator()(wil::shared_any_t<storage_t> const &val) const
        {
            return (hash<typename wil::shared_any_t<storage_t>::pointer>()(val.get()));
        }
    };
}
#endif


namespace wil
{

#if defined(__NOTHROW_T_DEFINED) && !defined(__WIL__NOTHROW_T_DEFINED)
#define __WIL__NOTHROW_T_DEFINED
    /** Provides `std::make_unique()` semantics for resources allocated in a context that may not throw upon allocation failure.
    `wil::make_unique_nothrow()` is identical to `std::make_unique()` except for the following:
    * It returns `wistd::unique_ptr`, rather than `std::unique_ptr`
    * It returns an empty (null) `wistd::unique_ptr` upon allocation failure, rather than throwing an exception

    Note that `wil::make_unique_nothrow()` is not marked WI_NOEXCEPT as it may be used to create an exception-based class that may throw in its constructor.
    ~~~
    auto foo = wil::make_unique_nothrow<Foo>(fooConstructorParam1, fooConstructorParam2);
    if (foo)
    {
    foo->Bar();
    }
    ~~~
    */
    template <class _Ty, class... _Types>
    inline typename wistd::enable_if<!wistd::is_array<_Ty>::value, wistd::unique_ptr<_Ty> >::type make_unique_nothrow(_Types&&... _Args)
    {
        return (wistd::unique_ptr<_Ty>(new(std::nothrow) _Ty(wistd::forward<_Types>(_Args)...)));
    }

    /** Provides `std::make_unique()` semantics for array resources allocated in a context that may not throw upon allocation failure.
    See the overload of `wil::make_unique_nothrow()` for non-array types for more details.
    ~~~
    const size_t size = 42;
    auto foos = wil::make_unique_nothrow<Foo[]>(size); // the default constructor will be called on each Foo object
    if (foos)
    {
    for (auto& elem : wil::make_range(foos.get(), size))
    {
    elem.Bar();
    }
    }
    ~~~
    */
    template <class _Ty>
    inline typename wistd::enable_if<wistd::is_array<_Ty>::value && wistd::extent<_Ty>::value == 0, wistd::unique_ptr<_Ty> >::type make_unique_nothrow(size_t _Size)
    {
        typedef typename wistd::remove_extent<_Ty>::type _Elem;
        return (wistd::unique_ptr<_Ty>(new(std::nothrow) _Elem[_Size]()));
    }

    template <class _Ty, class... _Types>
    typename wistd::enable_if<wistd::extent<_Ty>::value != 0, void>::type make_unique_nothrow(_Types&&...) = delete;

#if !defined(__WIL_MIN_KERNEL) && !defined(WIL_KERNEL_MODE)
    /** Provides `std::make_unique()` semantics for resources allocated in a context that must fail fast upon allocation failure.
    See the overload of `wil::make_unique_nothrow()` for non-array types for more details.
    ~~~
    auto foo = wil::make_unique_failfast<Foo>(fooConstructorParam1, fooConstructorParam2);
    foo->Bar();
    ~~~
    */
    template <class _Ty, class... _Types>
    inline typename wistd::enable_if<!wistd::is_array<_Ty>::value, wistd::unique_ptr<_Ty> >::type make_unique_failfast(_Types&&... _Args)
    {
#pragma warning(suppress: 28193)    // temporary must be inspected (it is within the called function)
        return (wistd::unique_ptr<_Ty>(FAIL_FAST_IF_NULL_ALLOC(new(std::nothrow) _Ty(wistd::forward<_Types>(_Args)...))));
    }

    /** Provides `std::make_unique()` semantics for array resources allocated in a context that must fail fast upon allocation failure.
    See the overload of `wil::make_unique_nothrow()` for non-array types for more details.
    ~~~
    const size_t size = 42;
    auto foos = wil::make_unique_nothrow<Foo[]>(size); // the default constructor will be called on each Foo object
    for (auto& elem : wil::make_range(foos.get(), size))
    {
    elem.Bar();
    }
    ~~~
    */
    template <class _Ty>
    inline typename wistd::enable_if<wistd::is_array<_Ty>::value && wistd::extent<_Ty>::value == 0, wistd::unique_ptr<_Ty> >::type make_unique_failfast(size_t _Size)
    {
        typedef typename wistd::remove_extent<_Ty>::type _Elem;
#pragma warning(suppress: 28193)    // temporary must be inspected (it is within the called function)
        return (wistd::unique_ptr<_Ty>(FAIL_FAST_IF_NULL_ALLOC(new(std::nothrow) _Elem[_Size]())));
    }

    template <class _Ty, class... _Types>
    typename wistd::enable_if<wistd::extent<_Ty>::value != 0, void>::type make_unique_failfast(_Types&&...) = delete;
#endif // !defined(__WIL_MIN_KERNEL) && !defined(WIL_KERNEL_MODE)
#endif // __WIL__NOTHROW_T_DEFINED

#if defined(_WINBASE_) && !defined(__WIL_WINBASE_) && !defined(WIL_KERNEL_MODE)
#define __WIL_WINBASE_
    /// @cond
    namespace details
    {
        inline void __stdcall SetEvent(HANDLE h) WI_NOEXCEPT
        {
            __FAIL_FAST_ASSERT_WIN32_BOOL_FALSE__(::SetEvent(h));
        }

        inline void __stdcall ResetEvent(HANDLE h) WI_NOEXCEPT
        {
            __FAIL_FAST_ASSERT_WIN32_BOOL_FALSE__(::ResetEvent(h));
        }

        inline void __stdcall CloseHandle(HANDLE h) WI_NOEXCEPT
        {
            __FAIL_FAST_ASSERT_WIN32_BOOL_FALSE__(::CloseHandle(h));
        }

        inline void __stdcall ReleaseSemaphore(_In_ HANDLE h) WI_NOEXCEPT
        {
            __FAIL_FAST_ASSERT_WIN32_BOOL_FALSE__(::ReleaseSemaphore(h, 1, nullptr));
        }

        inline void __stdcall ReleaseMutex(_In_ HANDLE h) WI_NOEXCEPT
        {
            __FAIL_FAST_ASSERT_WIN32_BOOL_FALSE__(::ReleaseMutex(h));
        }

        inline void __stdcall CloseTokenLinkedToken(_In_ TOKEN_LINKED_TOKEN* linkedToken) WI_NOEXCEPT
        {
            if (linkedToken->LinkedToken && (linkedToken->LinkedToken != INVALID_HANDLE_VALUE))
            {
                __FAIL_FAST_ASSERT_WIN32_BOOL_FALSE__(::CloseHandle(linkedToken->LinkedToken));
            }
        }

        enum class PendingCallbackCancellationBehavior
        {
            Cancel,
            Wait,
            NoWait,
        };

        template <PendingCallbackCancellationBehavior cancellationBehavior>
        struct DestroyThreadPoolWait
        {
            static void Destroy(_In_ PTP_WAIT threadPoolWait) WI_NOEXCEPT
            {
                ::SetThreadpoolWait(threadPoolWait, nullptr, nullptr);
                ::WaitForThreadpoolWaitCallbacks(threadPoolWait, (cancellationBehavior == PendingCallbackCancellationBehavior::Cancel));
                ::CloseThreadpoolWait(threadPoolWait);
            }
        };

        template <>
        struct DestroyThreadPoolWait<PendingCallbackCancellationBehavior::NoWait>
        {
            static void Destroy(_In_ PTP_WAIT threadPoolWait) WI_NOEXCEPT
            {
                ::CloseThreadpoolWait(threadPoolWait);
            }
        };

        template <PendingCallbackCancellationBehavior cancellationBehavior>
        struct DestroyThreadPoolWork
        {
            static void Destroy(_In_ PTP_WORK threadpoolWork) WI_NOEXCEPT
            {
                ::WaitForThreadpoolWorkCallbacks(threadpoolWork, (cancellationBehavior == PendingCallbackCancellationBehavior::Cancel));
                ::CloseThreadpoolWork(threadpoolWork);
            }
        };

        template <>
        struct DestroyThreadPoolWork<PendingCallbackCancellationBehavior::NoWait>
        {
            static void Destroy(_In_ PTP_WORK threadpoolWork) WI_NOEXCEPT
            {
                ::CloseThreadpoolWork(threadpoolWork);
            }
        };

        // Non-RTL implementation for threadpool_t parameter of DestroyThreadPoolTimer<>
        struct SystemThreadPoolMethods
        {
            static void WINAPI SetThreadpoolTimer(_Inout_ PTP_TIMER Timer, _In_opt_ PFILETIME DueTime, _In_ DWORD Period, _In_ DWORD WindowLength) WI_NOEXCEPT
            {
                ::SetThreadpoolTimer(Timer, DueTime, Period, WindowLength);
            }
            static void WaitForThreadpoolTimerCallbacks(_Inout_ PTP_TIMER Timer, _In_ BOOL CancelPendingCallbacks) WI_NOEXCEPT
            {
                ::WaitForThreadpoolTimerCallbacks(Timer, CancelPendingCallbacks);
            }
            static void CloseThreadpoolTimer(_Inout_ PTP_TIMER Timer) WI_NOEXCEPT
            {
                ::CloseThreadpoolTimer(Timer);
            }
        };

        // SetThreadpoolTimer(timer, nullptr, 0, 0) will cancel any pending callbacks,
        // then CloseThreadpoolTimer will asynchronusly close the timer if a callback is running.
        template <typename threadpool_t, PendingCallbackCancellationBehavior cancellationBehavior>
        struct DestroyThreadPoolTimer
        {
            static void Destroy(_In_ PTP_TIMER threadpoolTimer) WI_NOEXCEPT
            {
                threadpool_t::SetThreadpoolTimer(threadpoolTimer, nullptr, 0, 0);
#pragma warning(suppress:4127) // conditional expression is constant
                if (cancellationBehavior != PendingCallbackCancellationBehavior::NoWait)
                {
                    threadpool_t::WaitForThreadpoolTimerCallbacks(threadpoolTimer, (cancellationBehavior == PendingCallbackCancellationBehavior::Cancel));
                }
                threadpool_t::CloseThreadpoolTimer(threadpoolTimer);
            }
        };

        // PendingCallbackCancellationBehavior::NoWait explicitly does not block waiting for
        // callbacks when destructing.
        template <typename threadpool_t>
        struct DestroyThreadPoolTimer<threadpool_t, PendingCallbackCancellationBehavior::NoWait>
        {
            static void Destroy(_In_ PTP_TIMER threadpoolTimer) WI_NOEXCEPT
            {
                threadpool_t::CloseThreadpoolTimer(threadpoolTimer);
            }
        };

        template <PendingCallbackCancellationBehavior cancellationBehavior>
        struct DestroyThreadPoolIo
        {
            static void Destroy(_In_ PTP_IO threadpoolIo) WI_NOEXCEPT
            {
                ::WaitForThreadpoolIoCallbacks(threadpoolIo, (cancellationBehavior == PendingCallbackCancellationBehavior::Cancel));
                ::CloseThreadpoolIo(threadpoolIo);
            }
        };

        template <>
        struct DestroyThreadPoolIo<PendingCallbackCancellationBehavior::NoWait>
        {
            static void Destroy(_In_ PTP_IO threadpoolIo) WI_NOEXCEPT
            {
                ::CloseThreadpoolIo(threadpoolIo);
            }
        };

        template <typename close_fn_t, close_fn_t close_fn>
        struct handle_invalid_resource_policy : resource_policy<HANDLE, close_fn_t, close_fn, details::pointer_access_all, HANDLE, INT_PTR, -1, HANDLE>
        {
            __forceinline static bool is_valid(HANDLE ptr) WI_NOEXCEPT { return ((ptr != INVALID_HANDLE_VALUE) && (ptr != nullptr)); }
        };

        template <typename close_fn_t, close_fn_t close_fn>
        struct handle_null_resource_policy : resource_policy<HANDLE, close_fn_t, close_fn>
        {
            __forceinline static bool is_valid(HANDLE ptr) WI_NOEXCEPT { return ((ptr != nullptr) && (ptr != INVALID_HANDLE_VALUE)); }
        };

        template <typename close_fn_t, close_fn_t close_fn>
        struct handle_null_only_resource_policy : resource_policy<HANDLE, close_fn_t, close_fn>
        {
            __forceinline static bool is_valid(HANDLE ptr) WI_NOEXCEPT { return (ptr != nullptr); }
        };

        typedef resource_policy<HANDLE, decltype(&details::CloseHandle), details::CloseHandle, details::pointer_access_all> handle_resource_policy;
    }
    /// @endcond

    template <typename close_fn_t, close_fn_t close_fn>
    using unique_any_handle_invalid = unique_any_t<details::unique_storage<details::handle_invalid_resource_policy<close_fn_t, close_fn>>>;

    template <typename close_fn_t, close_fn_t close_fn>
    using unique_any_handle_null = unique_any_t<details::unique_storage<details::handle_null_resource_policy<close_fn_t, close_fn>>>;

    template <typename close_fn_t, close_fn_t close_fn>
    using unique_any_handle_null_only = unique_any_t<details::unique_storage<details::handle_null_only_resource_policy<close_fn_t, close_fn>>>;

    typedef unique_any_handle_invalid<decltype(&::CloseHandle), ::CloseHandle> unique_hfile;
    typedef unique_any_handle_null<decltype(&::CloseHandle), ::CloseHandle> unique_handle;
    typedef unique_any_handle_invalid<decltype(&::FindClose), ::FindClose> unique_hfind;
    typedef unique_any<HMODULE, decltype(&::FreeLibrary), ::FreeLibrary> unique_hmodule;
    typedef unique_any_handle_null_only<decltype(&::CloseHandle), ::CloseHandle> unique_process_handle;

    typedef unique_struct<TOKEN_LINKED_TOKEN, decltype(&details::CloseTokenLinkedToken), details::CloseTokenLinkedToken> unique_token_linked_token;

#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_APP | WINAPI_PARTITION_SYSTEM)
    typedef unique_any<PSID, decltype(&::FreeSid), ::FreeSid> unique_sid;
    typedef unique_any_handle_null_only<decltype(&::DeleteBoundaryDescriptor), ::DeleteBoundaryDescriptor> unique_boundary_descriptor;

    namespace details
    {
        template<ULONG flags>
        inline void __stdcall ClosePrivateNamespaceHelper(HANDLE h) WI_NOEXCEPT
        {
            ::ClosePrivateNamespace(h, flags);
        }
    }

    template <ULONG flags = 0>
    using unique_private_namespace = unique_any_handle_null_only<void(__stdcall*)(HANDLE) WI_PFN_NOEXCEPT, &details::ClosePrivateNamespaceHelper<flags>>;

    using unique_private_namespace_close = unique_private_namespace<>;
    using unique_private_namespace_destroy = unique_private_namespace<PRIVATE_NAMESPACE_FLAG_DESTROY>;
#endif

    using unique_tool_help_snapshot = unique_hfile;

    typedef unique_any<PTP_WAIT, void(*)(PTP_WAIT), details::DestroyThreadPoolWait<details::PendingCallbackCancellationBehavior::Cancel>::Destroy> unique_threadpool_wait;
    typedef unique_any<PTP_WAIT, void(*)(PTP_WAIT), details::DestroyThreadPoolWait<details::PendingCallbackCancellationBehavior::Wait>::Destroy> unique_threadpool_wait_nocancel;
    typedef unique_any<PTP_WAIT, void(*)(PTP_WAIT), details::DestroyThreadPoolWait<details::PendingCallbackCancellationBehavior::NoWait>::Destroy> unique_threadpool_wait_nowait;
    typedef unique_any<PTP_WORK, void(*)(PTP_WORK), details::DestroyThreadPoolWork<details::PendingCallbackCancellationBehavior::Cancel>::Destroy> unique_threadpool_work;
    typedef unique_any<PTP_WORK, void(*)(PTP_WORK), details::DestroyThreadPoolWork<details::PendingCallbackCancellationBehavior::Wait>::Destroy> unique_threadpool_work_nocancel;
    typedef unique_any<PTP_WORK, void(*)(PTP_WORK), details::DestroyThreadPoolWork<details::PendingCallbackCancellationBehavior::NoWait>::Destroy> unique_threadpool_work_nowait;
    typedef unique_any<PTP_TIMER, void(*)(PTP_TIMER), details::DestroyThreadPoolTimer<details::SystemThreadPoolMethods, details::PendingCallbackCancellationBehavior::Cancel>::Destroy> unique_threadpool_timer;
    typedef unique_any<PTP_TIMER, void(*)(PTP_TIMER), details::DestroyThreadPoolTimer<details::SystemThreadPoolMethods, details::PendingCallbackCancellationBehavior::Wait>::Destroy> unique_threadpool_timer_nocancel;
    typedef unique_any<PTP_TIMER, void(*)(PTP_TIMER), details::DestroyThreadPoolTimer<details::SystemThreadPoolMethods, details::PendingCallbackCancellationBehavior::NoWait>::Destroy> unique_threadpool_timer_nowait;
    typedef unique_any<PTP_IO, void(*)(PTP_IO), details::DestroyThreadPoolIo<details::PendingCallbackCancellationBehavior::Cancel>::Destroy> unique_threadpool_io;
    typedef unique_any<PTP_IO, void(*)(PTP_IO), details::DestroyThreadPoolIo<details::PendingCallbackCancellationBehavior::Wait>::Destroy> unique_threadpool_io_nocancel;
    typedef unique_any<PTP_IO, void(*)(PTP_IO), details::DestroyThreadPoolIo<details::PendingCallbackCancellationBehavior::NoWait>::Destroy> unique_threadpool_io_nowait;

#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
    typedef unique_any_handle_invalid<decltype(&::FindCloseChangeNotification), ::FindCloseChangeNotification> unique_hfind_change;
#endif

    typedef unique_any<HANDLE, decltype(&details::SetEvent), details::SetEvent, details::pointer_access_noaddress> event_set_scope_exit;
    typedef unique_any<HANDLE, decltype(&details::ResetEvent), details::ResetEvent, details::pointer_access_noaddress> event_reset_scope_exit;

    // Guarantees a SetEvent on the given event handle when the returned object goes out of scope
    // Note: call SetEvent early with the reset() method on the returned object or abort the call with the release() method
    WI_NODISCARD inline event_set_scope_exit SetEvent_scope_exit(HANDLE hEvent) WI_NOEXCEPT
    {
        __FAIL_FAST_ASSERT__(hEvent != nullptr);
        return event_set_scope_exit(hEvent);
    }

    // Guarantees a ResetEvent on the given event handle when the returned object goes out of scope
    // Note: call ResetEvent early with the reset() method on the returned object or abort the call with the release() method
    WI_NODISCARD inline event_reset_scope_exit ResetEvent_scope_exit(HANDLE hEvent) WI_NOEXCEPT
    {
        __FAIL_FAST_ASSERT__(hEvent != nullptr);
        return event_reset_scope_exit(hEvent);
    }

    // Checks to see if the given *manual reset* event is currently signaled.  The event must not be an auto-reset event.
    // Use when the event will only be set once (cancellation-style) or will only be reset by the polling thread
    inline bool event_is_signaled(HANDLE hEvent) WI_NOEXCEPT
    {
        auto status = ::WaitForSingleObjectEx(hEvent, 0, FALSE);
        // Fast fail will trip for wait failures, auto-reset events, or when the event is being both Set and Reset
        // from a thread other than the polling thread (use event_wait directly for those cases).
        __FAIL_FAST_ASSERT__((status == WAIT_TIMEOUT) || ((status == WAIT_OBJECT_0) && (WAIT_OBJECT_0 == ::WaitForSingleObjectEx(hEvent, 0, FALSE))));
        return (status == WAIT_OBJECT_0);
    }

    // Waits on the given handle for the specified duration
    inline bool handle_wait(HANDLE hEvent, DWORD dwMilliseconds = INFINITE, BOOL bAlertable = FALSE) WI_NOEXCEPT
    {
        DWORD status = ::WaitForSingleObjectEx(hEvent, dwMilliseconds, bAlertable);
        __FAIL_FAST_ASSERT__((status == WAIT_TIMEOUT) || (status == WAIT_OBJECT_0) || (bAlertable && (status == WAIT_IO_COMPLETION)));
        return (status == WAIT_OBJECT_0);
    }

    enum class EventOptions
    {
        None = 0x0,
        ManualReset = 0x1,
        Signaled = 0x2
    };
    DEFINE_ENUM_FLAG_OPERATORS(EventOptions);

    template <typename storage_t, typename err_policy = err_exception_policy>
    class event_t : public storage_t
    {
    public:
        // forward all base class constructors...
        template <typename... args_t>
        explicit event_t(args_t&&... args) WI_NOEXCEPT : storage_t(wistd::forward<args_t>(args)...) {}

        // HRESULT or void error handling...
        typedef typename err_policy::result result;

        // Exception-based constructor to create an unnamed event
        event_t(EventOptions options)
        {
            static_assert(wistd::is_same<void, result>::value, "this constructor requires exceptions or fail fast; use the create method");
            create(options);
        }

        void ResetEvent() const WI_NOEXCEPT
        {
            details::ResetEvent(storage_t::get());
        }

        void SetEvent() const WI_NOEXCEPT
        {
            details::SetEvent(storage_t::get());
        }

        // Guarantees a SetEvent on the given event handle when the returned object goes out of scope
        // Note: call SetEvent early with the reset() method on the returned object or abort the call with the release() method
        WI_NODISCARD event_set_scope_exit SetEvent_scope_exit() const WI_NOEXCEPT
        {
            return wil::SetEvent_scope_exit(storage_t::get());
        }

        // Guarantees a ResetEvent on the given event handle when the returned object goes out of scope
        // Note: call ResetEvent early with the reset() method on the returned object or abort the call with the release() method
        WI_NODISCARD event_reset_scope_exit ResetEvent_scope_exit() const WI_NOEXCEPT
        {
            return wil::ResetEvent_scope_exit(storage_t::get());
        }

        // Checks if a *manual reset* event is currently signaled.  The event must not be an auto-reset event.
        // Use when the event will only be set once (cancellation-style) or will only be reset by the polling thread
        WI_NODISCARD bool is_signaled() const WI_NOEXCEPT
        {
            return wil::event_is_signaled(storage_t::get());
        }

        // Basic WaitForSingleObject on the event handle with the given timeout
        bool wait(DWORD dwMilliseconds = INFINITE, BOOL bAlertable = FALSE) const WI_NOEXCEPT
        {
            return wil::handle_wait(storage_t::get(), dwMilliseconds, bAlertable);
        }

        // Tries to create a named event -- returns false if unable to do so (gle may still be inspected with return=false)
        bool try_create(EventOptions options, PCWSTR name, _In_opt_ LPSECURITY_ATTRIBUTES securityAttributes = nullptr, _Out_opt_ bool *alreadyExists = nullptr)
        {
            auto handle = ::CreateEventExW(securityAttributes, name, (WI_IsFlagSet(options, EventOptions::ManualReset) ? CREATE_EVENT_MANUAL_RESET : 0) | (WI_IsFlagSet(options, EventOptions::Signaled) ? CREATE_EVENT_INITIAL_SET : 0), EVENT_ALL_ACCESS);
            if (!handle)
            {
                assign_to_opt_param(alreadyExists, false);
                return false;
            }
            assign_to_opt_param(alreadyExists, (::GetLastError() == ERROR_ALREADY_EXISTS));
            storage_t::reset(handle);
            return true;
        }

        // Returns HRESULT for unique_event_nothrow, void with exceptions for shared_event and unique_event
        result create(EventOptions options = EventOptions::None, PCWSTR name = nullptr, _In_opt_ LPSECURITY_ATTRIBUTES securityAttributes = nullptr, _Out_opt_ bool *alreadyExists = nullptr)
        {
            return err_policy::LastErrorIfFalse(try_create(options, name, securityAttributes, alreadyExists));
        }

        // Tries to open the named event -- returns false if unable to do so (gle may still be inspected with return=false)
        bool try_open(_In_ PCWSTR name, DWORD desiredAccess = SYNCHRONIZE | EVENT_MODIFY_STATE, bool inheritHandle = false)
        {
            auto handle = ::OpenEventW(desiredAccess, inheritHandle, name);
            if (handle == nullptr)
            {
                return false;
            }
            storage_t::reset(handle);
            return true;
        }

        // Returns HRESULT for unique_event_nothrow, void with exceptions for shared_event and unique_event
        result open(_In_ PCWSTR name, DWORD desiredAccess = SYNCHRONIZE | EVENT_MODIFY_STATE, bool inheritHandle = false)
        {
            return err_policy::LastErrorIfFalse(try_open(name, desiredAccess, inheritHandle));
        }
    };

    typedef unique_any_t<event_t<details::unique_storage<details::handle_resource_policy>, err_returncode_policy>>     unique_event_nothrow;
    typedef unique_any_t<event_t<details::unique_storage<details::handle_resource_policy>, err_failfast_policy>>       unique_event_failfast;
#ifdef WIL_ENABLE_EXCEPTIONS
    typedef unique_any_t<event_t<details::unique_storage<details::handle_resource_policy>, err_exception_policy>>      unique_event;
#endif

#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP) && ((_WIN32_WINNT >= _WIN32_WINNT_WIN8) || (__WIL_RESOURCE_ENABLE_QUIRKS && (_WIN32_WINNT >= _WIN32_WINNT_WIN7)))
    enum class SlimEventType
    {
        AutoReset,
        ManualReset,
    };

    /** A lean and mean event class.
    This class provides a very similar API to `wil::unique_event` but doesn't require a kernel object.

    The two variants of this class are:
    - `wil::slim_event_auto_reset`
    - `wil::slim_event_manual_reset`

    In addition, `wil::slim_event_auto_reset` has the alias `wil::slim_event`.

    Some key differences to `wil::unique_event` include:
    - There is no 'create()' function, as initialization occurs in the constructor and can't fail.
    - The move functions have been deleted.
    - For auto-reset events, the `is_signaled()` function doesn't reset the event. (Use `ResetEvent()` instead.)
    - The `ResetEvent()` function returns the previous state of the event.
    - To create a manual reset event, use `wil::slim_event_manual_reset'.
    ~~~~
    wil::slim_event finished;
    std::thread doStuff([&finished] () {
        Sleep(10);
        finished.SetEvent();
    });
    finished.wait();

    std::shared_ptr<wil::slim_event> CreateSharedEvent(bool startSignaled)
    {
        return std::make_shared<wil::slim_event>(startSignaled);
    }
    ~~~~ */
    template <SlimEventType Type>
    class slim_event_t
    {
    public:
        slim_event_t() WI_NOEXCEPT = default;

        slim_event_t(bool isSignaled) WI_NOEXCEPT :
            m_isSignaled(isSignaled ? TRUE : FALSE)
        {
        }

        // Cannot change memory location.
        slim_event_t(const slim_event_t&) = delete;
        slim_event_t(slim_event_t&&) = delete;
        slim_event_t& operator=(const slim_event_t&) = delete;
        slim_event_t& operator=(slim_event_t&&) = delete;

        // Returns the previous state of the event.
        bool ResetEvent() WI_NOEXCEPT
        {
            return !!InterlockedExchange(&m_isSignaled, FALSE);
        }

        void SetEvent() WI_NOEXCEPT
        {
            // FYI: 'WakeByAddress*' invokes a full memory barrier.
            WriteRelease(&m_isSignaled, TRUE);

            #pragma warning(suppress:4127) // conditional expression is constant
            if (Type == SlimEventType::AutoReset)
            {
                WakeByAddressSingle(&m_isSignaled);
            }
            else
            {
                WakeByAddressAll(&m_isSignaled);
            }
        }

        // Checks if the event is currently signaled.
        // Note: Unlike Win32 auto-reset event objects, this will not reset the event.
        WI_NODISCARD bool is_signaled() const WI_NOEXCEPT
        {
            return !!ReadAcquire(&m_isSignaled);
        }

        bool wait(DWORD timeoutMiliseconds) WI_NOEXCEPT
        {
            if (timeoutMiliseconds == 0)
            {
                return TryAcquireEvent();
            }
            else if (timeoutMiliseconds == INFINITE)
            {
                return wait();
            }

            UINT64 startTime{};
            QueryUnbiasedInterruptTime(&startTime);

            UINT64 elapsedTimeMilliseconds = 0;

            while (!TryAcquireEvent())
            {
                if (elapsedTimeMilliseconds >= timeoutMiliseconds)
                {
                    return false;
                }

                DWORD newTimeout = static_cast<DWORD>(timeoutMiliseconds - elapsedTimeMilliseconds);

                if (!WaitForSignal(newTimeout))
                {
                    return false;
                }

                UINT64 currTime;
                QueryUnbiasedInterruptTime(&currTime);

                elapsedTimeMilliseconds = (currTime - startTime) / static_cast<UINT64>(10 * 1000);
            }

            return true;
        }

        bool wait() WI_NOEXCEPT
        {
            while (!TryAcquireEvent())
            {
                if (!WaitForSignal(INFINITE))
                {
                    return false;
                }
            }

            return true;
        }

    private:
        bool TryAcquireEvent() WI_NOEXCEPT
        {
            #pragma warning(suppress:4127) // conditional expression is constant
            if (Type == SlimEventType::AutoReset)
            {
                return ResetEvent();
            }
            else
            {
                return is_signaled();
            }
        }

        bool WaitForSignal(DWORD timeoutMiliseconds) WI_NOEXCEPT
        {
            LONG falseValue = FALSE;
            BOOL waitResult = WaitOnAddress(&m_isSignaled, &falseValue, sizeof(m_isSignaled), timeoutMiliseconds);
            __FAIL_FAST_ASSERT__(waitResult || ::GetLastError() == ERROR_TIMEOUT);
            return !!waitResult;
        }

        LONG m_isSignaled = FALSE;
    };

    /** An event object that will atomically revert to an unsignaled state anytime a `wait()` call succeeds (i.e. returns true). */
    using slim_event_auto_reset = slim_event_t<SlimEventType::AutoReset>;

    /** An event object that once signaled remains that way forever, unless `ResetEvent()` is called. */
    using slim_event_manual_reset = slim_event_t<SlimEventType::ManualReset>;

    /** An alias for `wil::slim_event_auto_reset`. */
    using slim_event = slim_event_auto_reset;

#endif // WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP) && (_WIN32_WINNT >= _WIN32_WINNT_WIN8)

    typedef unique_any<HANDLE, decltype(&details::ReleaseMutex), details::ReleaseMutex, details::pointer_access_none> mutex_release_scope_exit;

    WI_NODISCARD inline mutex_release_scope_exit ReleaseMutex_scope_exit(_In_ HANDLE hMutex) WI_NOEXCEPT
    {
        __FAIL_FAST_ASSERT__(hMutex != nullptr);
        return mutex_release_scope_exit(hMutex);
    }

    // For efficiency, avoid using mutexes when an srwlock or condition variable will do.
    template <typename storage_t, typename err_policy = err_exception_policy>
    class mutex_t : public storage_t
    {
    public:
        // forward all base class constructors...
        template <typename... args_t>
        explicit mutex_t(args_t&&... args) WI_NOEXCEPT : storage_t(wistd::forward<args_t>(args)...) {}

        // HRESULT or void error handling...
        typedef typename err_policy::result result;

        // Exception-based constructor to create a mutex (prefer unnamed (nullptr) for the name)
        mutex_t(_In_opt_ PCWSTR name)
        {
            static_assert(wistd::is_same<void, result>::value, "this constructor requires exceptions or fail fast; use the create method");
            create(name);
        }

        void ReleaseMutex() const WI_NOEXCEPT
        {
            details::ReleaseMutex(storage_t::get());
        }

        WI_NODISCARD mutex_release_scope_exit ReleaseMutex_scope_exit() const WI_NOEXCEPT
        {
            return wil::ReleaseMutex_scope_exit(storage_t::get());
        }

        WI_NODISCARD mutex_release_scope_exit acquire(_Out_opt_ DWORD *pStatus = nullptr, DWORD dwMilliseconds = INFINITE, BOOL bAlertable = FALSE)  const WI_NOEXCEPT
        {
            auto handle = storage_t::get();
            DWORD status = ::WaitForSingleObjectEx(handle, dwMilliseconds, bAlertable);
            assign_to_opt_param(pStatus, status);
            __FAIL_FAST_ASSERT__((status == WAIT_TIMEOUT) || (status == WAIT_OBJECT_0) || (status == WAIT_ABANDONED) || (bAlertable && (status == WAIT_IO_COMPLETION)));
            return mutex_release_scope_exit(((status == WAIT_OBJECT_0) || (status == WAIT_ABANDONED)) ? handle : nullptr);
        }

        // Tries to create a named mutex -- returns false if unable to do so (gle may still be inspected with return=false)
        bool try_create(_In_opt_ PCWSTR name, DWORD dwFlags = 0, DWORD desiredAccess = MUTEX_ALL_ACCESS,
                        _In_opt_ PSECURITY_ATTRIBUTES mutexAttributes = nullptr, _Out_opt_ bool* alreadyExists = nullptr)
        {
            auto handle = ::CreateMutexExW(mutexAttributes, name, dwFlags, desiredAccess);
            if (handle == nullptr)
            {
                assign_to_opt_param(alreadyExists, false);
                return false;
            }
            assign_to_opt_param(alreadyExists, (::GetLastError() == ERROR_ALREADY_EXISTS));
            storage_t::reset(handle);
            return true;
        }

        // Returns HRESULT for unique_mutex_nothrow, void with exceptions for shared_mutex and unique_mutex
        result create(_In_opt_ PCWSTR name = nullptr, DWORD dwFlags = 0, DWORD desiredAccess = MUTEX_ALL_ACCESS,
                      _In_opt_ PSECURITY_ATTRIBUTES mutexAttributes = nullptr, _Out_opt_ bool* alreadyExists = nullptr)
        {
            return err_policy::LastErrorIfFalse(try_create(name, dwFlags, desiredAccess, mutexAttributes, alreadyExists));
        }

        // Tries to open a named mutex -- returns false if unable to do so (gle may still be inspected with return=false)
        bool try_open(_In_ PCWSTR name, DWORD desiredAccess = SYNCHRONIZE | MUTEX_MODIFY_STATE, bool inheritHandle = false)
        {
            auto handle = ::OpenMutexW(desiredAccess, inheritHandle, name);
            if (handle == nullptr)
            {
                return false;
            }
            storage_t::reset(handle);
            return true;
        }

        // Returns HRESULT for unique_mutex_nothrow, void with exceptions for shared_mutex and unique_mutex
        result open(_In_ PCWSTR name, DWORD desiredAccess = SYNCHRONIZE | MUTEX_MODIFY_STATE, bool inheritHandle = false)
        {
            return err_policy::LastErrorIfFalse(try_open(name, desiredAccess, inheritHandle));
        }
    };

    typedef unique_any_t<mutex_t<details::unique_storage<details::handle_resource_policy>, err_returncode_policy>>     unique_mutex_nothrow;
    typedef unique_any_t<mutex_t<details::unique_storage<details::handle_resource_policy>, err_failfast_policy>>       unique_mutex_failfast;
#ifdef WIL_ENABLE_EXCEPTIONS
    typedef unique_any_t<mutex_t<details::unique_storage<details::handle_resource_policy>, err_exception_policy>>      unique_mutex;
#endif

    typedef unique_any<HANDLE, decltype(&details::ReleaseSemaphore), details::ReleaseSemaphore, details::pointer_access_none> semaphore_release_scope_exit;

    WI_NODISCARD inline semaphore_release_scope_exit ReleaseSemaphore_scope_exit(_In_ HANDLE hSemaphore) WI_NOEXCEPT
    {
        __FAIL_FAST_ASSERT__(hSemaphore != nullptr);
        return semaphore_release_scope_exit(hSemaphore);
    }

    template <typename storage_t, typename err_policy = err_exception_policy>
    class semaphore_t : public storage_t
    {
    public:
        // forward all base class constructors...
        template <typename... args_t>
        explicit semaphore_t(args_t&&... args) WI_NOEXCEPT : storage_t(wistd::forward<args_t>(args)...) {}

        // HRESULT or void error handling...
        typedef typename err_policy::result result;

        // Note that for custom-constructors the type given the constructor has to match exactly as not all implicit conversions will make it through the
        // forwarding constructor.  This constructor, for example, uses 'int' instead of 'LONG' as the count to ease that particular issue (const numbers are int by default).
        explicit semaphore_t(int initialCount, int maximumCount, _In_opt_ PCWSTR name = nullptr, DWORD desiredAccess = SEMAPHORE_ALL_ACCESS, _In_opt_ PSECURITY_ATTRIBUTES pSemaphoreAttributes = nullptr)
        {
            static_assert(wistd::is_same<void, result>::value, "this constructor requires exceptions or fail fast; use the create method");
            create(initialCount, maximumCount, name, desiredAccess, pSemaphoreAttributes);
        }

        void ReleaseSemaphore(long nReleaseCount = 1, _In_opt_ long *pnPreviousCount = nullptr) WI_NOEXCEPT
        {
            long nPreviousCount = 0;
            __FAIL_FAST_ASSERT__(::ReleaseSemaphore(storage_t::get(), nReleaseCount, &nPreviousCount));
            assign_to_opt_param(pnPreviousCount, nPreviousCount);
        }

        WI_NODISCARD semaphore_release_scope_exit ReleaseSemaphore_scope_exit() WI_NOEXCEPT
        {
            return wil::ReleaseSemaphore_scope_exit(storage_t::get());
        }

        WI_NODISCARD semaphore_release_scope_exit acquire(_Out_opt_ DWORD *pStatus = nullptr, DWORD dwMilliseconds = INFINITE, BOOL bAlertable = FALSE) WI_NOEXCEPT
        {
            auto handle = storage_t::get();
            DWORD status = ::WaitForSingleObjectEx(handle, dwMilliseconds, bAlertable);
            assign_to_opt_param(pStatus, status);
            __FAIL_FAST_ASSERT__((status == WAIT_TIMEOUT) || (status == WAIT_OBJECT_0) || (bAlertable && (status == WAIT_IO_COMPLETION)));
            return semaphore_release_scope_exit((status == WAIT_OBJECT_0) ? handle : nullptr);
        }

        // Tries to create a named event -- returns false if unable to do so (gle may still be inspected with return=false)
        bool try_create(LONG lInitialCount, LONG lMaximumCount, _In_opt_ PCWSTR name, DWORD desiredAccess = SEMAPHORE_ALL_ACCESS, _In_opt_ PSECURITY_ATTRIBUTES pSemaphoreAttributes = nullptr, _Out_opt_ bool *alreadyExists = nullptr)
        {
            auto handle = ::CreateSemaphoreExW(pSemaphoreAttributes, lInitialCount, lMaximumCount, name, 0, desiredAccess);
            if (handle == nullptr)
            {
                assign_to_opt_param(alreadyExists, false);
                return false;
            }
            assign_to_opt_param(alreadyExists, (::GetLastError() == ERROR_ALREADY_EXISTS));
            storage_t::reset(handle);
            return true;
        }

        // Returns HRESULT for unique_semaphore_nothrow, void with exceptions for shared_event and unique_event
        result create(LONG lInitialCount, LONG lMaximumCount, _In_opt_ PCWSTR name = nullptr, DWORD desiredAccess = SEMAPHORE_ALL_ACCESS, _In_opt_ PSECURITY_ATTRIBUTES pSemaphoreAttributes = nullptr, _Out_opt_ bool *alreadyExists = nullptr)
        {
            return err_policy::LastErrorIfFalse(try_create(lInitialCount, lMaximumCount, name, desiredAccess, pSemaphoreAttributes, alreadyExists));
        }

        // Tries to open the named semaphore -- returns false if unable to do so (gle may still be inspected with return=false)
        bool try_open(_In_ PCWSTR name, DWORD desiredAccess = SYNCHRONIZE | SEMAPHORE_MODIFY_STATE, bool inheritHandle = false)
        {
            auto handle = ::OpenSemaphoreW(desiredAccess, inheritHandle, name);
            if (handle == nullptr)
            {
                return false;
            }
            storage_t::reset(handle);
            return true;
        }

        // Returns HRESULT for unique_semaphore_nothrow, void with exceptions for shared_semaphore and unique_semaphore
        result open(_In_ PCWSTR name, DWORD desiredAccess = SYNCHRONIZE | SEMAPHORE_MODIFY_STATE, bool inheritHandle = false)
        {
            return err_policy::LastErrorIfFalse(try_open(name, desiredAccess, inheritHandle));
        }
    };

    typedef unique_any_t<semaphore_t<details::unique_storage<details::handle_resource_policy>, err_returncode_policy>>  unique_semaphore_nothrow;
    typedef unique_any_t<semaphore_t<details::unique_storage<details::handle_resource_policy>, err_failfast_policy>>    unique_semaphore_failfast;
#ifdef WIL_ENABLE_EXCEPTIONS
    typedef unique_any_t<semaphore_t<details::unique_storage<details::handle_resource_policy>, err_exception_policy>>   unique_semaphore;
#endif

    typedef unique_any<SRWLOCK *, decltype(&::ReleaseSRWLockExclusive), ::ReleaseSRWLockExclusive, details::pointer_access_noaddress> rwlock_release_exclusive_scope_exit;
    typedef unique_any<SRWLOCK *, decltype(&::ReleaseSRWLockShared), ::ReleaseSRWLockShared, details::pointer_access_noaddress> rwlock_release_shared_scope_exit;

    WI_NODISCARD inline rwlock_release_exclusive_scope_exit AcquireSRWLockExclusive(_Inout_ SRWLOCK *plock) WI_NOEXCEPT
    {
        ::AcquireSRWLockExclusive(plock);
        return rwlock_release_exclusive_scope_exit(plock);
    }

    WI_NODISCARD inline rwlock_release_shared_scope_exit AcquireSRWLockShared(_Inout_ SRWLOCK *plock) WI_NOEXCEPT
    {
        ::AcquireSRWLockShared(plock);
        return rwlock_release_shared_scope_exit(plock);
    }

    WI_NODISCARD inline rwlock_release_exclusive_scope_exit TryAcquireSRWLockExclusive(_Inout_ SRWLOCK *plock) WI_NOEXCEPT
    {
        return rwlock_release_exclusive_scope_exit(::TryAcquireSRWLockExclusive(plock) ? plock : nullptr);
    }

    WI_NODISCARD inline rwlock_release_shared_scope_exit TryAcquireSRWLockShared(_Inout_ SRWLOCK *plock) WI_NOEXCEPT
    {
        return rwlock_release_shared_scope_exit(::TryAcquireSRWLockShared(plock) ? plock : nullptr);
    }

    class srwlock
    {
    public:
        srwlock(const srwlock&) = delete;
        srwlock(srwlock&&) = delete;
        srwlock& operator=(const srwlock&) = delete;
        srwlock& operator=(srwlock&&) = delete;

        srwlock() = default;

        WI_NODISCARD rwlock_release_exclusive_scope_exit lock_exclusive() WI_NOEXCEPT
        {
            return wil::AcquireSRWLockExclusive(&m_lock);
        }

        WI_NODISCARD rwlock_release_exclusive_scope_exit try_lock_exclusive() WI_NOEXCEPT
        {
            return wil::TryAcquireSRWLockExclusive(&m_lock);
        }

        WI_NODISCARD rwlock_release_shared_scope_exit lock_shared() WI_NOEXCEPT
        {
            return wil::AcquireSRWLockShared(&m_lock);
        }

        WI_NODISCARD rwlock_release_shared_scope_exit try_lock_shared() WI_NOEXCEPT
        {
            return wil::TryAcquireSRWLockShared(&m_lock);
        }

    private:
        SRWLOCK m_lock = SRWLOCK_INIT;
    };

    typedef unique_any<CRITICAL_SECTION *, decltype(&::LeaveCriticalSection), ::LeaveCriticalSection, details::pointer_access_noaddress> cs_leave_scope_exit;

    WI_NODISCARD inline cs_leave_scope_exit EnterCriticalSection(_Inout_ CRITICAL_SECTION *pcs) WI_NOEXCEPT
    {
        ::EnterCriticalSection(pcs);
        return cs_leave_scope_exit(pcs);
    }

    WI_NODISCARD inline cs_leave_scope_exit TryEnterCriticalSection(_Inout_ CRITICAL_SECTION *pcs) WI_NOEXCEPT
    {
        return cs_leave_scope_exit(::TryEnterCriticalSection(pcs) ? pcs : nullptr);
    }

    // Critical sections are worse than srwlocks in performance and memory usage (their only unique attribute
    // being recursive acquisition). Prefer srwlocks over critical sections when you don't need recursive acquisition.
    class critical_section
    {
    public:
        critical_section(const critical_section&) = delete;
        critical_section(critical_section&&) = delete;
        critical_section& operator=(const critical_section&) = delete;
        critical_section& operator=(critical_section&&) = delete;

        critical_section(ULONG spincount = 0) WI_NOEXCEPT
        {
            // Initialization will not fail without invalid params...
            ::InitializeCriticalSectionEx(&m_cs, spincount, 0);
        }

        ~critical_section() WI_NOEXCEPT
        {
            ::DeleteCriticalSection(&m_cs);
        }

        WI_NODISCARD cs_leave_scope_exit lock() WI_NOEXCEPT
        {
            return wil::EnterCriticalSection(&m_cs);
        }

        WI_NODISCARD cs_leave_scope_exit try_lock() WI_NOEXCEPT
        {
            return wil::TryEnterCriticalSection(&m_cs);
        }

    private:
        CRITICAL_SECTION m_cs;
    };

    class condition_variable
    {
    public:
        condition_variable(const condition_variable&) = delete;
        condition_variable(condition_variable&&) = delete;
        condition_variable& operator=(const condition_variable&) = delete;
        condition_variable& operator=(condition_variable&&) = delete;

        condition_variable() = default;

        void notify_one() WI_NOEXCEPT
        {
            ::WakeConditionVariable(&m_cv);
        }

        void notify_all() WI_NOEXCEPT
        {
            ::WakeAllConditionVariable(&m_cv);
        }

        void wait(const cs_leave_scope_exit& lock) WI_NOEXCEPT
        {
            wait_for(lock, INFINITE);
        }

        void wait(const rwlock_release_exclusive_scope_exit& lock) WI_NOEXCEPT
        {
            wait_for(lock, INFINITE);
        }

        void wait(const rwlock_release_shared_scope_exit& lock) WI_NOEXCEPT
        {
            wait_for(lock, INFINITE);
        }

        bool wait_for(const cs_leave_scope_exit& lock, DWORD timeoutMs) WI_NOEXCEPT
        {
            bool result = !!::SleepConditionVariableCS(&m_cv, lock.get(), timeoutMs);
            __FAIL_FAST_ASSERT__(result || ::GetLastError() == ERROR_TIMEOUT);
            return result;
        }

        bool wait_for(const rwlock_release_exclusive_scope_exit& lock, DWORD timeoutMs) WI_NOEXCEPT
        {
            bool result = !!::SleepConditionVariableSRW(&m_cv, lock.get(), timeoutMs, 0);
            __FAIL_FAST_ASSERT__(result || ::GetLastError() == ERROR_TIMEOUT);
            return result;
        }

        bool wait_for(const rwlock_release_shared_scope_exit& lock, DWORD timeoutMs) WI_NOEXCEPT
        {
            bool result = !!::SleepConditionVariableSRW(&m_cv, lock.get(), timeoutMs, CONDITION_VARIABLE_LOCKMODE_SHARED);
            __FAIL_FAST_ASSERT__(result || ::GetLastError() == ERROR_TIMEOUT);
            return result;
        }

    private:
        CONDITION_VARIABLE m_cv = CONDITION_VARIABLE_INIT;
    };

    /// @cond
    namespace details
    {
        template<typename string_class> struct string_allocator
        {
            static void* allocate(size_t /*size*/) WI_NOEXCEPT
            {
                static_assert(!wistd::is_same<string_class, string_class>::value, "This type did not provide a string_allocator, add a specialization of string_allocator to support your type.");
                return nullptr;
            }
        };
    }
    /// @endcond

    // This string helper does not support the ansi wil string helpers
    template<typename string_type>
    PCWSTR string_get_not_null(const string_type& string)
    {
        return string ? string.get() : L"";
    }

#ifndef MAKE_UNIQUE_STRING_MAX_CCH
#define MAKE_UNIQUE_STRING_MAX_CCH     2147483647  // max buffer size, in characters, that we support (same as INT_MAX)
#endif

    /** Copies a string (up to the given length) into memory allocated with a specified allocator returning null on failure.
    Use `wil::make_unique_string_nothrow()` for string resources returned from APIs that must satisfy a memory allocation contract
    that requires use of a specific allocator and free function (CoTaskMemAlloc/CoTaskMemFree, LocalAlloc/LocalFree, GlobalAlloc/GlobalFree, etc.).
    ~~~
    auto str = wil::make_unique_string_nothrow<wil::unique_cotaskmem_string>(L"a string of words", 8);
    RETURN_IF_NULL_ALLOC(str);
    std::wcout << L"This is " << str.get() << std::endl; // prints "This is a string"

    auto str = wil::make_unique_string_nothrow<unique_hlocal_string>(L"a string");
    RETURN_IF_NULL_ALLOC(str);
    std::wcout << L"This is " << str.get() << std::endl; // prints "This is a string"

    NOTE: If source is not null terminated, then length MUST be equal to or less than the size
          of the buffer pointed to by source.
    ~~~
    */
    template<typename string_type> string_type make_unique_string_nothrow(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        const wchar_t* source, size_t length = static_cast<size_t>(-1)) WI_NOEXCEPT
    {
        // guard against invalid parameters (null source with -1 length)
        FAIL_FAST_IF(!source && (length == static_cast<size_t>(-1)));

        // When the source string exists, calculate the number of characters to copy up to either
        // 1) the length that is given
        // 2) the length of the source string. When the source does not exist, use the given length
        //    for calculating both the size of allocated buffer and the number of characters to copy.
        size_t lengthToCopy = length;
        if (source)
        {
            size_t maxLength = length < MAKE_UNIQUE_STRING_MAX_CCH ? length : MAKE_UNIQUE_STRING_MAX_CCH;
            PCWSTR endOfSource = source;
            while (maxLength && (*endOfSource != L'\0'))
            {
                endOfSource++;
                maxLength--;
            }
            lengthToCopy = endOfSource - source;
        }

        if (length == static_cast<size_t>(-1))
        {
            length = lengthToCopy;
        }
        const size_t allocatedBytes = (length + 1) * sizeof(*source);
        auto result = static_cast<PWSTR>(details::string_allocator<string_type>::allocate(allocatedBytes));

        if (result)
        {
            if (source)
            {
                const size_t bytesToCopy = lengthToCopy * sizeof(*source);
                memcpy_s(result, allocatedBytes, source, bytesToCopy);
                result[lengthToCopy] = L'\0'; // ensure the copied string is zero terminated
            }
            else
            {
                *result = L'\0'; // ensure null terminated in the "reserve space" use case.
            }
            result[length] = L'\0'; // ensure the final char of the buffer is zero terminated
        }
        return string_type(result);
    }
#ifndef WIL_NO_ANSI_STRINGS
    template<typename string_type> string_type make_unique_ansistring_nothrow(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCSTR source, size_t length = static_cast<size_t>(-1)) WI_NOEXCEPT
    {
        if (length == static_cast<size_t>(-1))
        {
            // guard against invalid parameters (null source with -1 length)
            FAIL_FAST_IF(!source);
            length = strlen(source);
        }
        const size_t cb = (length + 1) * sizeof(*source);
        auto result = static_cast<PSTR>(details::string_allocator<string_type>::allocate(cb));
        if (result)
        {
            if (source)
            {
                memcpy_s(result, cb, source, cb - sizeof(*source));
            }
            else
            {
                *result = '\0'; // ensure null terminated in the "reserve space" use case.
            }
            result[length] = '\0'; // ensure zero terminated
        }
        return string_type(result);
    }
#endif // WIL_NO_ANSI_STRINGS

    /** Copies a given string into memory allocated with a specified allocator that will fail fast on failure.
    The use of variadic templates parameters supports the 2 forms of make_unique_string, see those for more details.
    */
    template<typename string_type> string_type make_unique_string_failfast(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCWSTR source, size_t length = static_cast<size_t>(-1)) WI_NOEXCEPT
    {
        auto result(make_unique_string_nothrow<string_type>(source, length));
        FAIL_FAST_IF_NULL_ALLOC(result);
        return result;
    }

#ifndef WIL_NO_ANSI_STRINGS
    template<typename string_type> string_type make_unique_ansistring_failfast(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCSTR source, size_t length = static_cast<size_t>(-1)) WI_NOEXCEPT
    {
        auto result(make_unique_ansistring_nothrow<string_type>(source, length));
        FAIL_FAST_IF_NULL_ALLOC(result);
        return result;
    }
#endif // WIL_NO_ANSI_STRINGS

#ifdef WIL_ENABLE_EXCEPTIONS
    /** Copies a given string into memory allocated with a specified allocator that will throw on failure.
    The use of variadic templates parameters supports the 2 forms of make_unique_string, see those for more details.
    */
    template<typename string_type> string_type make_unique_string(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCWSTR source, size_t length = static_cast<size_t>(-1))
    {
        auto result(make_unique_string_nothrow<string_type>(source, length));
        THROW_IF_NULL_ALLOC(result);
        return result;
    }
#ifndef WIL_NO_ANSI_STRINGS
    template<typename string_type> string_type make_unique_ansistring(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCSTR source, size_t length = static_cast<size_t>(-1))
    {
        auto result(make_unique_ansistring_nothrow<string_type>(source, length));
        THROW_IF_NULL_ALLOC(result);
        return result;
    }
#endif // WIL_NO_ANSI_STRINGS
#endif // WIL_ENABLE_EXCEPTIONS

    /// @cond
    namespace details
    {
        // string_maker abstracts creating a string for common string types. This form supports the
        // wil::unique_xxx_string types. Specializations of other types like HSTRING and std::wstring
        // are found in wil\winrt.h and wil\stl.h.
        // This design supports creating the string in a single step or using two phase construction.

        template<typename string_type> struct string_maker
        {
            HRESULT make(
                _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
                _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
                const wchar_t* source,
                size_t length)
            {
                m_value = make_unique_string_nothrow<string_type>(source, length);
                return m_value ? S_OK : E_OUTOFMEMORY;
            }

            wchar_t* buffer() { WI_ASSERT(m_value.get());  return m_value.get(); }

            // By default, assume string_type is a null-terminated string and therefore does not require trimming.
            HRESULT trim_at_existing_null(size_t /* length */) { return S_OK; }

            string_type release() { return wistd::move(m_value); }

            // Utility to abstract access to the null terminated m_value of all string types.
            static PCWSTR get(const string_type& value) { return value.get(); }

        private:
            string_type m_value; // a wil::unique_xxx_string type.
        };

        struct SecureZeroData
        {
            void *pointer;
            size_t sizeBytes;
            SecureZeroData(void *pointer_, size_t sizeBytes_ = 0) WI_NOEXCEPT { pointer = pointer_; sizeBytes = sizeBytes_; }
            WI_NODISCARD operator void*() const WI_NOEXCEPT { return pointer; }
            static void Close(SecureZeroData data) WI_NOEXCEPT { ::SecureZeroMemory(data.pointer, data.sizeBytes); }
        };
    }
    /// @endcond

    typedef unique_any<void*, decltype(&details::SecureZeroData::Close), details::SecureZeroData::Close, details::pointer_access_all, details::SecureZeroData> secure_zero_memory_scope_exit;

    WI_NODISCARD inline secure_zero_memory_scope_exit SecureZeroMemory_scope_exit(_In_reads_bytes_(sizeBytes) void* pSource, size_t sizeBytes)
    {
        return secure_zero_memory_scope_exit(details::SecureZeroData(pSource, sizeBytes));
    }

    WI_NODISCARD inline secure_zero_memory_scope_exit SecureZeroMemory_scope_exit(_In_ PWSTR initializedString)
    {
        return SecureZeroMemory_scope_exit(static_cast<void*>(initializedString), wcslen(initializedString) * sizeof(initializedString[0]));
    }

    /// @cond
    namespace details
    {
        inline void __stdcall FreeProcessHeap(_Pre_opt_valid_ _Frees_ptr_opt_ void* p)
        {
            ::HeapFree(::GetProcessHeap(), 0, p);
        }
    }
    /// @endcond

    struct process_heap_deleter
    {
        template <typename T>
        void operator()(_Pre_valid_ _Frees_ptr_ T* p) const
        {
            details::FreeProcessHeap(p);
        }
    };

    struct virtualalloc_deleter
    {
        template<typename T>
        void operator()(_Pre_valid_ _Frees_ptr_ T* p) const
        {
            ::VirtualFree(p, 0, MEM_RELEASE);
        }
    };

    struct mapview_deleter
    {
        template<typename T>
        void operator()(_Pre_valid_ _Frees_ptr_ T* p) const
        {
            ::UnmapViewOfFile(p);
        }
    };

    template <typename T = void>
    using unique_process_heap_ptr = wistd::unique_ptr<T, process_heap_deleter>;

    typedef unique_any<PWSTR, decltype(&details::FreeProcessHeap), details::FreeProcessHeap> unique_process_heap_string;

    /// @cond
    namespace details
    {
        template<> struct string_allocator<unique_process_heap_string>
        {
            static _Ret_opt_bytecap_(size) void* allocate(size_t size) WI_NOEXCEPT
            {
                return ::HeapAlloc(::GetProcessHeap(), HEAP_ZERO_MEMORY, size);
            }
        };
    }
    /// @endcond

    /** Manages a typed pointer allocated with VirtualAlloc
    A specialization of wistd::unique_ptr<> that frees via VirtualFree(p, 0, MEM_RELEASE).
    */
    template<typename T = void>
    using unique_virtualalloc_ptr = wistd::unique_ptr<T, virtualalloc_deleter>;

    /** Manages a typed pointer allocated with MapViewOfFile
    A specialization of wistd::unique_ptr<> that frees via UnmapViewOfFile(p).
    */
    template<typename T = void>
    using unique_mapview_ptr = wistd::unique_ptr<T, mapview_deleter>;

#endif // __WIL_WINBASE_

#if defined(__WIL_WINBASE_) && defined(__NOTHROW_T_DEFINED) && !defined(__WIL_WINBASE_NOTHROW_T_DEFINED)
#define __WIL_WINBASE_NOTHROW_T_DEFINED
    // unique_event_watcher, unique_event_watcher_nothrow, unique_event_watcher_failfast
    //
    // Clients must include <new> or <new.h> to enable use of this class as it uses new(std::nothrow).
    // This is to avoid the dependency on those headers that some clients can't tolerate.
    //
    // These classes makes it easy to execute a provided function when an event
    // is signaled. It will create the event handle for you, take ownership of one
    // or duplicate a handle provided. It supports the ability to signal the
    // event using SetEvent() and SetEvent_scope_exit();
    //
    // This can be used to support producer-consumer pattern
    // where a producer updates some state then signals the event when done.
    // The consumer will consume that state in the callback provided to unique_event_watcher.
    //
    // Note, multiple signals may coalesce into a single callback.
    //
    // Example use of throwing version:
    // auto globalStateWatcher = wil::make_event_watcher([]
    //     {
    //         currentState = GetGlobalState();
    //     });
    //
    // UpdateGlobalState(value);
    // globalStateWatcher.SetEvent(); // signal observers so they can update
    //
    // Example use of non-throwing version:
    // auto globalStateWatcher = wil::make_event_watcher_nothrow([]
    //     {
    //         currentState = GetGlobalState();
    //     });
    // RETURN_IF_NULL_ALLOC(globalStateWatcher);
    //
    // UpdateGlobalState(value);
    // globalStateWatcher.SetEvent(); // signal observers so they can update

    /// @cond
    namespace details
    {
        struct event_watcher_state
        {
            event_watcher_state(unique_event_nothrow &&eventHandle, wistd::function<void()> &&callback)
                : m_callback(wistd::move(callback)), m_event(wistd::move(eventHandle))
            {
            }
            wistd::function<void()> m_callback;
            unique_event_nothrow m_event;
            // The thread pool must be last to ensure that the other members are valid
            // when it is destructed as it will reference them.
            unique_threadpool_wait m_threadPoolWait;
        };

        inline void delete_event_watcher_state(_In_opt_ event_watcher_state *watcherStorage) { delete watcherStorage; }

        typedef resource_policy<event_watcher_state *, decltype(&delete_event_watcher_state),
            delete_event_watcher_state, details::pointer_access_none> event_watcher_state_resource_policy;
    }
    /// @endcond

    template <typename storage_t, typename err_policy = err_exception_policy>
    class event_watcher_t : public storage_t
    {
    public:
        // forward all base class constructors...
        template <typename... args_t>
        explicit event_watcher_t(args_t&&... args) WI_NOEXCEPT : storage_t(wistd::forward<args_t>(args)...) {}

        // HRESULT or void error handling...
        typedef typename err_policy::result result;

        // Exception-based constructors
        template <typename from_err_policy>
        event_watcher_t(unique_any_t<event_t<details::unique_storage<details::handle_resource_policy>, from_err_policy>> &&eventHandle, wistd::function<void()> &&callback)
        {
            static_assert(wistd::is_same<void, result>::value, "this constructor requires exceptions or fail fast; use the create method");
            create(wistd::move(eventHandle), wistd::move(callback));
        }

        event_watcher_t(_In_ HANDLE eventHandle, wistd::function<void()> &&callback)
        {
            static_assert(wistd::is_same<void, result>::value, "this constructor requires exceptions or fail fast; use the create method");
            create(eventHandle, wistd::move(callback));
        }

        event_watcher_t(wistd::function<void()> &&callback)
        {
            static_assert(wistd::is_same<void, result>::value, "this constructor requires exceptions or fail fast; use the create method");
            create(wistd::move(callback));
        }

        template <typename event_err_policy>
        result create(unique_any_t<event_t<details::unique_storage<details::handle_resource_policy>, event_err_policy>> &&eventHandle,
            wistd::function<void()> &&callback)
        {
            return err_policy::HResult(create_take_hevent_ownership(eventHandle.release(), wistd::move(callback)));
        }

        // Creates the event that you will be watching.
        result create(wistd::function<void()> &&callback)
        {
            unique_event_nothrow eventHandle;
            HRESULT hr = eventHandle.create(EventOptions::ManualReset); // auto-reset is supported too.
            if (FAILED(hr))
            {
                return err_policy::HResult(hr);
            }
            return err_policy::HResult(create_take_hevent_ownership(eventHandle.release(), wistd::move(callback)));
        }

        // Input is an event handler that is duplicated into this class.
        result create(_In_ HANDLE eventHandle, wistd::function<void()> &&callback)
        {
            unique_event_nothrow ownedHandle;
            if (!DuplicateHandle(GetCurrentProcess(), eventHandle, GetCurrentProcess(), &ownedHandle, 0, FALSE, DUPLICATE_SAME_ACCESS))
            {
                return err_policy::LastError();
            }
            return err_policy::HResult(create_take_hevent_ownership(ownedHandle.release(), wistd::move(callback)));
        }

        // Provide access to the inner event and the very common SetEvent() method on it.
        WI_NODISCARD unique_event_nothrow const& get_event() const WI_NOEXCEPT { return storage_t::get()->m_event; }
        void SetEvent() const WI_NOEXCEPT { storage_t::get()->m_event.SetEvent(); }

    private:

        // Had to move this from a Lambda so it would compile in C++/CLI (which thought the Lambda should be a managed function for some reason).
        static void CALLBACK wait_callback(PTP_CALLBACK_INSTANCE, void *context, TP_WAIT *pThreadPoolWait, TP_WAIT_RESULT)
        {
            auto pThis = static_cast<details::event_watcher_state *>(context);
            // Manual events must be re-set to avoid missing the last notification.
            pThis->m_event.ResetEvent();
            // Call the client before re-arming to ensure that multiple callbacks don't
            // run concurrently.
            pThis->m_callback();
            SetThreadpoolWait(pThreadPoolWait, pThis->m_event.get(), nullptr); // valid params ensure success
        }

        // To avoid template expansion (if unique_event/unique_event_nothrow forms were used) this base
        // create function takes a raw handle and assumes its ownership, even on failure.
        HRESULT create_take_hevent_ownership(_In_ HANDLE rawHandleOwnershipTaken, wistd::function<void()> &&callback)
        {
            __FAIL_FAST_ASSERT__(rawHandleOwnershipTaken != nullptr); // invalid parameter
            unique_event_nothrow eventHandle(rawHandleOwnershipTaken);
            wistd::unique_ptr<details::event_watcher_state> watcherState(new(std::nothrow) details::event_watcher_state(wistd::move(eventHandle), wistd::move(callback)));
            RETURN_IF_NULL_ALLOC(watcherState);

            watcherState->m_threadPoolWait.reset(CreateThreadpoolWait(wait_callback, watcherState.get(), nullptr));
            RETURN_LAST_ERROR_IF(!watcherState->m_threadPoolWait);
            storage_t::reset(watcherState.release()); // no more failures after this, pass ownership
            SetThreadpoolWait(storage_t::get()->m_threadPoolWait.get(), storage_t::get()->m_event.get(), nullptr);
            return S_OK;
        }
    };

    typedef unique_any_t<event_watcher_t<details::unique_storage<details::event_watcher_state_resource_policy>, err_returncode_policy>> unique_event_watcher_nothrow;
    typedef unique_any_t<event_watcher_t<details::unique_storage<details::event_watcher_state_resource_policy>, err_failfast_policy>> unique_event_watcher_failfast;

    template <typename err_policy>
    unique_event_watcher_nothrow make_event_watcher_nothrow(unique_any_t<event_t<details::unique_storage<details::handle_resource_policy>, err_policy>> &&eventHandle, wistd::function<void()> &&callback) WI_NOEXCEPT
    {
        unique_event_watcher_nothrow watcher;
        watcher.create(wistd::move(eventHandle), wistd::move(callback));
        return watcher; // caller must test for success using if (watcher)
    }

    inline unique_event_watcher_nothrow make_event_watcher_nothrow(_In_ HANDLE eventHandle, wistd::function<void()> &&callback) WI_NOEXCEPT
    {
        unique_event_watcher_nothrow watcher;
        watcher.create(eventHandle, wistd::move(callback));
        return watcher; // caller must test for success using if (watcher)
    }

    inline unique_event_watcher_nothrow make_event_watcher_nothrow(wistd::function<void()> &&callback) WI_NOEXCEPT
    {
        unique_event_watcher_nothrow watcher;
        watcher.create(wistd::move(callback));
        return watcher; // caller must test for success using if (watcher)
    }

    template <typename err_policy>
    unique_event_watcher_failfast make_event_watcher_failfast(unique_any_t<event_t<details::unique_storage<details::handle_resource_policy>, err_policy>> &&eventHandle, wistd::function<void()> &&callback)
    {
        return unique_event_watcher_failfast(wistd::move(eventHandle), wistd::move(callback));
    }

    inline unique_event_watcher_failfast make_event_watcher_failfast(_In_ HANDLE eventHandle, wistd::function<void()> &&callback)
    {
        return unique_event_watcher_failfast(eventHandle, wistd::move(callback));
    }

    inline unique_event_watcher_failfast make_event_watcher_failfast(wistd::function<void()> &&callback)
    {
        return unique_event_watcher_failfast(wistd::move(callback));
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    typedef unique_any_t<event_watcher_t<details::unique_storage<details::event_watcher_state_resource_policy>, err_exception_policy>> unique_event_watcher;

    template <typename err_policy>
    unique_event_watcher make_event_watcher(unique_any_t<event_t<details::unique_storage<details::handle_resource_policy>, err_policy>> &&eventHandle, wistd::function<void()> &&callback)
    {
        return unique_event_watcher(wistd::move(eventHandle), wistd::move(callback));
    }

    inline unique_event_watcher make_event_watcher(_In_ HANDLE eventHandle, wistd::function<void()> &&callback)
    {
        return unique_event_watcher(eventHandle, wistd::move(callback));
    }

    inline unique_event_watcher make_event_watcher(wistd::function<void()> &&callback)
    {
        return unique_event_watcher(wistd::move(callback));
    }
#endif // WIL_ENABLE_EXCEPTIONS

#endif // __WIL_WINBASE_NOTHROW_T_DEFINED

#if defined(__WIL_WINBASE_) && !defined(__WIL_WINBASE_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_WINBASE_STL
    typedef shared_any_t<event_t<details::shared_storage<unique_event>>> shared_event;
    typedef shared_any_t<mutex_t<details::shared_storage<unique_mutex>>> shared_mutex;
    typedef shared_any_t<semaphore_t<details::shared_storage<unique_semaphore>>> shared_semaphore;
    typedef shared_any<unique_hfile> shared_hfile;
    typedef shared_any<unique_handle> shared_handle;
    typedef shared_any<unique_hfind> shared_hfind;
    typedef shared_any<unique_hmodule> shared_hmodule;

#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
    typedef shared_any<unique_threadpool_wait> shared_threadpool_wait;
    typedef shared_any<unique_threadpool_wait_nocancel> shared_threadpool_wait_nocancel;
    typedef shared_any<unique_threadpool_work> shared_threadpool_work;
    typedef shared_any<unique_threadpool_work_nocancel> shared_threadpool_work_nocancel;

    typedef shared_any<unique_hfind_change> shared_hfind_change;
#endif

    typedef weak_any<shared_event> weak_event;
    typedef weak_any<shared_mutex> weak_mutex;
    typedef weak_any<shared_semaphore> weak_semaphore;
    typedef weak_any<shared_hfile> weak_hfile;
    typedef weak_any<shared_handle> weak_handle;
    typedef weak_any<shared_hfind> weak_hfind;
    typedef weak_any<shared_hmodule> weak_hmodule;

#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
    typedef weak_any<shared_threadpool_wait> weak_threadpool_wait;
    typedef weak_any<shared_threadpool_wait_nocancel> weak_threadpool_wait_nocancel;
    typedef weak_any<shared_threadpool_work> weak_threadpool_work;
    typedef weak_any<shared_threadpool_work_nocancel> weak_threadpool_work_nocancel;

    typedef weak_any<shared_hfind_change> weak_hfind_change;
#endif

#endif // __WIL_WINBASE_STL

#if defined(__WIL_WINBASE_) && defined(__NOTHROW_T_DEFINED) && !defined(__WIL_WINBASE_NOTHROW_T_DEFINED_STL) && defined(WIL_RESOURCE_STL) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_WINBASE_NOTHROW_T_DEFINED_STL
    typedef shared_any_t<event_watcher_t<details::shared_storage<unique_event_watcher>>> shared_event_watcher;
    typedef weak_any<shared_event_watcher> weak_event_watcher;
#endif // __WIL_WINBASE_NOTHROW_T_DEFINED_STL

#if defined(__WIL_WINBASE_) && !defined(__WIL_WINBASE_DESKTOP) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_WINBASE_DESKTOP
    /// @cond
    namespace details
    {
        inline void __stdcall DestroyPrivateObjectSecurity(_Pre_opt_valid_ _Frees_ptr_opt_ PSECURITY_DESCRIPTOR pObjectDescriptor) WI_NOEXCEPT
        {
            ::DestroyPrivateObjectSecurity(&pObjectDescriptor);
        }
    }
    /// @endcond

    using hlocal_deleter = function_deleter<decltype(&::LocalFree), LocalFree>;

    template <typename T = void>
    using unique_hlocal_ptr = wistd::unique_ptr<T, hlocal_deleter>;

    /** Provides `std::make_unique()` semantics for resources allocated with `LocalAlloc()` in a context that may not throw upon allocation failure.
    Use `wil::make_unique_hlocal_nothrow()` for resources returned from APIs that must satisfy a memory allocation contract that requires the use of `LocalAlloc()` / `LocalFree()`.
    Use `wil::make_unique_nothrow()` when `LocalAlloc()` is not required.

    Allocations are initialized with placement new and will call constructors (if present), but this does not guarantee initialization.

    Note that `wil::make_unique_hlocal_nothrow()` is not marked WI_NOEXCEPT as it may be used to create an exception-based class that may throw in its constructor.
    ~~~
    auto foo = wil::make_unique_hlocal_nothrow<Foo>();
    if (foo)
    {
    // initialize allocated Foo object as appropriate
    }
    ~~~
    */
    template <typename T, typename... Args>
    inline typename wistd::enable_if<!wistd::is_array<T>::value, unique_hlocal_ptr<T>>::type make_unique_hlocal_nothrow(Args&&... args)
    {
        static_assert(wistd::is_trivially_destructible<T>::value, "T has a destructor that won't be run when used with this function; use make_unique instead");
        unique_hlocal_ptr<T> sp(static_cast<T*>(::LocalAlloc(LMEM_FIXED, sizeof(T))));
        if (sp)
        {
            // use placement new to initialize memory from the previous allocation
            new (sp.get()) T(wistd::forward<Args>(args)...);
        }
        return sp;
    }

    /** Provides `std::make_unique()` semantics for array resources allocated with `LocalAlloc()` in a context that may not throw upon allocation failure.
    See the overload of `wil::make_unique_hlocal_nothrow()` for non-array types for more details.
    ~~~
    const size_t size = 42;
    auto foos = wil::make_unique_hlocal_nothrow<Foo[]>(size);
    if (foos)
    {
    for (auto& elem : wil::make_range(foos.get(), size))
    {
    // initialize allocated Foo objects as appropriate
    }
    }
    ~~~
    */
    template <typename T>
    inline typename wistd::enable_if<wistd::is_array<T>::value && wistd::extent<T>::value == 0, unique_hlocal_ptr<T>>::type make_unique_hlocal_nothrow(size_t size)
    {
        typedef typename wistd::remove_extent<T>::type E;
        static_assert(wistd::is_trivially_destructible<E>::value, "E has a destructor that won't be run when used with this function; use make_unique instead");
        FAIL_FAST_IF((__WI_SIZE_MAX / sizeof(E)) < size);
        size_t allocSize = sizeof(E) * size;
        unique_hlocal_ptr<T> sp(static_cast<E*>(::LocalAlloc(LMEM_FIXED, allocSize)));
        if (sp)
        {
            // use placement new to initialize memory from the previous allocation;
            // note that array placement new cannot be used as the standard allows for operator new[]
            // to consume overhead in the allocation for internal bookkeeping
            for (auto& elem : make_range(static_cast<E*>(sp.get()), size))
            {
                new (&elem) E();
            }
        }
        return sp;
    }

    /** Provides `std::make_unique()` semantics for resources allocated with `LocalAlloc()` in a context that must fail fast upon allocation failure.
    See the overload of `wil::make_unique_hlocal_nothrow()` for non-array types for more details.
    ~~~
    auto foo = wil::make_unique_hlocal_failfast<Foo>();
    // initialize allocated Foo object as appropriate
    ~~~
    */
    template <typename T, typename... Args>
    inline typename wistd::enable_if<!wistd::is_array<T>::value, unique_hlocal_ptr<T>>::type make_unique_hlocal_failfast(Args&&... args)
    {
        unique_hlocal_ptr<T> result(make_unique_hlocal_nothrow<T>(wistd::forward<Args>(args)...));
        FAIL_FAST_IF_NULL_ALLOC(result);
        return result;
    }

    /** Provides `std::make_unique()` semantics for array resources allocated with `LocalAlloc()` in a context that must fail fast upon allocation failure.
    See the overload of `wil::make_unique_hlocal_nothrow()` for non-array types for more details.
    ~~~
    const size_t size = 42;
    auto foos = wil::make_unique_hlocal_failfast<Foo[]>(size);
    for (auto& elem : wil::make_range(foos.get(), size))
    {
    // initialize allocated Foo objects as appropriate
    }
    ~~~
    */
    template <typename T>
    inline typename wistd::enable_if<wistd::is_array<T>::value && wistd::extent<T>::value == 0, unique_hlocal_ptr<T>>::type make_unique_hlocal_failfast(size_t size)
    {
        unique_hlocal_ptr<T> result(make_unique_hlocal_nothrow<T>(size));
        FAIL_FAST_IF_NULL_ALLOC(result);
        return result;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    /** Provides `std::make_unique()` semantics for resources allocated with `LocalAlloc()`.
    See the overload of `wil::make_unique_hlocal_nothrow()` for non-array types for more details.
    ~~~
    auto foo = wil::make_unique_hlocal<Foo>();
    // initialize allocated Foo object as appropriate
    ~~~
    */
    template <typename T, typename... Args>
    inline typename wistd::enable_if<!wistd::is_array<T>::value, unique_hlocal_ptr<T>>::type make_unique_hlocal(Args&&... args)
    {
        unique_hlocal_ptr<T> result(make_unique_hlocal_nothrow<T>(wistd::forward<Args>(args)...));
        THROW_IF_NULL_ALLOC(result);
        return result;
    }

    /** Provides `std::make_unique()` semantics for array resources allocated with `LocalAlloc()`.
    See the overload of `wil::make_unique_hlocal_nothrow()` for non-array types for more details.
    ~~~
    const size_t size = 42;
    auto foos = wil::make_unique_hlocal<Foo[]>(size);
    for (auto& elem : wil::make_range(foos.get(), size))
    {
    // initialize allocated Foo objects as appropriate
    }
    ~~~
    */
    template <typename T>
    inline typename wistd::enable_if<wistd::is_array<T>::value && wistd::extent<T>::value == 0, unique_hlocal_ptr<T>>::type make_unique_hlocal(size_t size)
    {
        unique_hlocal_ptr<T> result(make_unique_hlocal_nothrow<T>(size));
        THROW_IF_NULL_ALLOC(result);
        return result;
    }
#endif // WIL_ENABLE_EXCEPTIONS

    typedef unique_any<HLOCAL, decltype(&::LocalFree), ::LocalFree> unique_hlocal;
    typedef unique_any<PWSTR, decltype(&::LocalFree), ::LocalFree> unique_hlocal_string;
#ifndef WIL_NO_ANSI_STRINGS
    typedef unique_any<PSTR, decltype(&::LocalFree), ::LocalFree> unique_hlocal_ansistring;
#endif // WIL_NO_ANSI_STRINGS

    /// @cond
    namespace details
    {
        struct localalloc_allocator
        {
            static _Ret_opt_bytecap_(size) void* allocate(size_t size) WI_NOEXCEPT
            {
                return ::LocalAlloc(LMEM_FIXED, size);
            }
        };

        template<> struct string_allocator<unique_hlocal_string> : localalloc_allocator {};
#ifndef WIL_NO_ANSI_STRINGS
        template<> struct string_allocator<unique_hlocal_ansistring> : localalloc_allocator {};
#endif // WIL_NO_ANSI_STRINGS
    }
    /// @endcond

    inline auto make_hlocal_string_nothrow(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCWSTR source, size_t length = static_cast<size_t>(-1)) WI_NOEXCEPT
    {
        return make_unique_string_nothrow<unique_hlocal_string>(source, length);
    }

    inline auto make_hlocal_string_failfast(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCWSTR source, size_t length = static_cast<size_t>(-1)) WI_NOEXCEPT
    {
        return make_unique_string_failfast<unique_hlocal_string>(source, length);
    }

#ifndef WIL_NO_ANSI_STRINGS
    inline auto make_hlocal_ansistring_nothrow(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCSTR source, size_t length = static_cast<size_t>(-1)) WI_NOEXCEPT
    {
        return make_unique_ansistring_nothrow<unique_hlocal_ansistring>(source, length);
    }

    inline auto make_hlocal_ansistring_failfast(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCSTR source, size_t length = static_cast<size_t>(-1)) WI_NOEXCEPT
    {
        return make_unique_ansistring_failfast<unique_hlocal_ansistring>(source, length);
    }
#endif

#ifdef WIL_ENABLE_EXCEPTIONS
    inline auto make_hlocal_string(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCWSTR source, size_t length = static_cast<size_t>(-1))
    {
        return make_unique_string<unique_hlocal_string>(source, length);
    }

#ifndef WIL_NO_ANSI_STRINGS
    inline auto make_hlocal_ansistring(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCSTR source, size_t length = static_cast<size_t>(-1))
    {
        return make_unique_ansistring<unique_hlocal_ansistring>(source, length);
    }
#endif // WIL_NO_ANSI_STRINGS
#endif // WIL_ENABLE_EXCEPTIONS

    struct hlocal_secure_deleter
    {
        template <typename T>
        void operator()(_Pre_opt_valid_ _Frees_ptr_opt_ T* p) const
        {
            if (p)
            {
#pragma warning(suppress: 26006 26007) // LocalSize() ensures proper buffer length
                ::SecureZeroMemory(p, ::LocalSize(p)); // this is safe since LocalSize() returns 0 on failure
                ::LocalFree(p);
            }
        }
    };

    template <typename T = void>
    using unique_hlocal_secure_ptr = wistd::unique_ptr<T, hlocal_secure_deleter>;

    /** Provides `std::make_unique()` semantics for secure resources allocated with `LocalAlloc()` in a context that may not throw upon allocation failure.
    See the overload of `wil::make_unique_hlocal_nothrow()` for non-array types for more details.
    ~~~
    auto foo = wil::make_unique_hlocal_secure_nothrow<Foo>();
    if (foo)
    {
    // initialize allocated Foo object as appropriate
    }
    ~~~
    */
    template <typename T, typename... Args>
    inline typename wistd::enable_if<!wistd::is_array<T>::value, unique_hlocal_secure_ptr<T>>::type make_unique_hlocal_secure_nothrow(Args&&... args)
    {
        return unique_hlocal_secure_ptr<T>(make_unique_hlocal_nothrow<T>(wistd::forward<Args>(args)...).release());
    }

    /** Provides `std::make_unique()` semantics for secure array resources allocated with `LocalAlloc()` in a context that may not throw upon allocation failure.
    See the overload of `wil::make_unique_hlocal_nothrow()` for non-array types for more details.
    ~~~
    const size_t size = 42;
    auto foos = wil::make_unique_hlocal_secure_nothrow<Foo[]>(size);
    if (foos)
    {
    for (auto& elem : wil::make_range(foos.get(), size))
    {
    // initialize allocated Foo objects as appropriate
    }
    }
    ~~~
    */
    template <typename T>
    inline typename wistd::enable_if<wistd::is_array<T>::value && wistd::extent<T>::value == 0, unique_hlocal_secure_ptr<T>>::type make_unique_hlocal_secure_nothrow(size_t size)
    {
        return unique_hlocal_secure_ptr<T>(make_unique_hlocal_nothrow<T>(size).release());
    }

    /** Provides `std::make_unique()` semantics for secure resources allocated with `LocalAlloc()` in a context that must fail fast upon allocation failure.
    See the overload of `wil::make_unique_hlocal_nothrow()` for non-array types for more details.
    ~~~
    auto foo = wil::make_unique_hlocal_secure_failfast<Foo>();
    // initialize allocated Foo object as appropriate
    ~~~
    */
    template <typename T, typename... Args>
    inline typename wistd::enable_if<!wistd::is_array<T>::value, unique_hlocal_secure_ptr<T>>::type make_unique_hlocal_secure_failfast(Args&&... args)
    {
        unique_hlocal_secure_ptr<T> result(make_unique_hlocal_secure_nothrow<T>(wistd::forward<Args>(args)...));
        FAIL_FAST_IF_NULL_ALLOC(result);
        return result;
    }

    /** Provides `std::make_unique()` semantics for secure array resources allocated with `LocalAlloc()` in a context that must fail fast upon allocation failure.
    See the overload of `wil::make_unique_hlocal_nothrow()` for non-array types for more details.
    ~~~
    const size_t size = 42;
    auto foos = wil::make_unique_hlocal_secure_failfast<Foo[]>(size);
    for (auto& elem : wil::make_range(foos.get(), size))
    {
    // initialize allocated Foo objects as appropriate
    }
    ~~~
    */
    template <typename T>
    inline typename wistd::enable_if<wistd::is_array<T>::value && wistd::extent<T>::value == 0, unique_hlocal_secure_ptr<T>>::type make_unique_hlocal_secure_failfast(size_t size)
    {
        unique_hlocal_secure_ptr<T> result(make_unique_hlocal_secure_nothrow<T>(size));
        FAIL_FAST_IF_NULL_ALLOC(result);
        return result;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    /** Provides `std::make_unique()` semantics for secure resources allocated with `LocalAlloc()`.
    See the overload of `wil::make_unique_hlocal_nothrow()` for non-array types for more details.
    ~~~
    auto foo = wil::make_unique_hlocal_secure<Foo>();
    // initialize allocated Foo object as appropriate
    ~~~
    */
    template <typename T, typename... Args>
    inline typename wistd::enable_if<!wistd::is_array<T>::value, unique_hlocal_secure_ptr<T>>::type make_unique_hlocal_secure(Args&&... args)
    {
        unique_hlocal_secure_ptr<T> result(make_unique_hlocal_secure_nothrow<T>(wistd::forward<Args>(args)...));
        THROW_IF_NULL_ALLOC(result);
        return result;
    }

    /** Provides `std::make_unique()` semantics for secure array resources allocated with `LocalAlloc()`.
    See the overload of `wil::make_unique_hlocal_nothrow()` for non-array types for more details.
    ~~~
    const size_t size = 42;
    auto foos = wil::make_unique_hlocal_secure<Foo[]>(size);
    for (auto& elem : wil::make_range(foos.get(), size))
    {
    // initialize allocated Foo objects as appropriate
    }
    ~~~
    */
    template <typename T>
    inline typename wistd::enable_if<wistd::is_array<T>::value && wistd::extent<T>::value == 0, unique_hlocal_secure_ptr<T>>::type make_unique_hlocal_secure(size_t size)
    {
        unique_hlocal_secure_ptr<T> result(make_unique_hlocal_secure_nothrow<T>(size));
        THROW_IF_NULL_ALLOC(result);
        return result;
    }
#endif // WIL_ENABLE_EXCEPTIONS

    typedef unique_hlocal_secure_ptr<wchar_t[]> unique_hlocal_string_secure;

    /** Copies a given string into secure memory allocated with `LocalAlloc()` in a context that may not throw upon allocation failure.
    See the overload of `wil::make_hlocal_string_nothrow()` with supplied length for more details.
    ~~~
    auto str = wil::make_hlocal_string_secure_nothrow(L"a string");
    RETURN_IF_NULL_ALLOC(str);
    std::wcout << L"This is " << str.get() << std::endl; // prints "This is a string"
    ~~~
    */
    inline auto make_hlocal_string_secure_nothrow(_In_ PCWSTR source) WI_NOEXCEPT
    {
        return unique_hlocal_string_secure(make_hlocal_string_nothrow(source).release());
    }

    /** Copies a given string into secure memory allocated with `LocalAlloc()` in a context that must fail fast upon allocation failure.
    See the overload of `wil::make_hlocal_string_nothrow()` with supplied length for more details.
    ~~~
    auto str = wil::make_hlocal_string_secure_failfast(L"a string");
    std::wcout << L"This is " << str.get() << std::endl; // prints "This is a string"
    ~~~
    */
    inline auto make_hlocal_string_secure_failfast(_In_ PCWSTR source) WI_NOEXCEPT
    {
        unique_hlocal_string_secure result(make_hlocal_string_secure_nothrow(source));
        FAIL_FAST_IF_NULL_ALLOC(result);
        return result;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    /** Copies a given string into secure memory allocated with `LocalAlloc()`.
    See the overload of `wil::make_hlocal_string_nothrow()` with supplied length for more details.
    ~~~
    auto str = wil::make_hlocal_string_secure(L"a string");
    std::wcout << L"This is " << str.get() << std::endl; // prints "This is a string"
    ~~~
    */
    inline auto make_hlocal_string_secure(_In_ PCWSTR source)
    {
        unique_hlocal_string_secure result(make_hlocal_string_secure_nothrow(source));
        THROW_IF_NULL_ALLOC(result);
        return result;
    }
#endif

    using hglobal_deleter = function_deleter<decltype(&::GlobalFree), ::GlobalFree>;

    template <typename T = void>
    using unique_hglobal_ptr = wistd::unique_ptr<T, hglobal_deleter>;

    typedef unique_any<HGLOBAL, decltype(&::GlobalFree), ::GlobalFree> unique_hglobal;
    typedef unique_any<PWSTR, decltype(&::GlobalFree), ::GlobalFree> unique_hglobal_string;
#ifndef WIL_NO_ANSI_STRINGS
    typedef unique_any<PSTR, decltype(&::GlobalFree), ::GlobalFree> unique_hglobal_ansistring;
#endif // WIL_NO_ANSI_STRINGS

    /// @cond
    namespace details
    {
        template<> struct string_allocator<unique_hglobal_string>
        {
            static _Ret_opt_bytecap_(size) void* allocate(size_t size) WI_NOEXCEPT
            {
                return ::GlobalAlloc(GPTR, size);
            }
        };
    }
    /// @endcond

    inline auto make_process_heap_string_nothrow(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCWSTR source, size_t length = static_cast<size_t>(-1)) WI_NOEXCEPT
    {
        return make_unique_string_nothrow<unique_process_heap_string>(source, length);
    }

    inline auto make_process_heap_string_failfast(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCWSTR source, size_t length = static_cast<size_t>(-1)) WI_NOEXCEPT
    {
        return make_unique_string_failfast<unique_process_heap_string>(source, length);
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    inline auto make_process_heap_string(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCWSTR source, size_t length = static_cast<size_t>(-1))
    {
        return make_unique_string<unique_process_heap_string>(source, length);
    }
#endif // WIL_ENABLE_EXCEPTIONS

    typedef unique_any_handle_null<decltype(&::HeapDestroy), ::HeapDestroy> unique_hheap;
    typedef unique_any<DWORD, decltype(&::TlsFree), ::TlsFree, details::pointer_access_all, DWORD, DWORD, TLS_OUT_OF_INDEXES, DWORD> unique_tls;
    typedef unique_any<PSECURITY_DESCRIPTOR, decltype(&::LocalFree), ::LocalFree> unique_hlocal_security_descriptor;
    typedef unique_any<PSECURITY_DESCRIPTOR, decltype(&details::DestroyPrivateObjectSecurity), details::DestroyPrivateObjectSecurity> unique_private_security_descriptor;

#if defined(_WINUSER_) && !defined(__WIL__WINUSER_)
#define __WIL__WINUSER_
    typedef unique_any<HACCEL, decltype(&::DestroyAcceleratorTable), ::DestroyAcceleratorTable> unique_haccel;
    typedef unique_any<HCURSOR, decltype(&::DestroyCursor), ::DestroyCursor> unique_hcursor;
    typedef unique_any<HWND, decltype(&::DestroyWindow), ::DestroyWindow> unique_hwnd;
#if !defined(NOUSER) && !defined(NOWH)
    typedef unique_any<HHOOK, decltype(&::UnhookWindowsHookEx), ::UnhookWindowsHookEx> unique_hhook;
#endif
#if !defined(NOWINABLE)
    typedef unique_any<HWINEVENTHOOK, decltype(&::UnhookWinEvent), ::UnhookWinEvent> unique_hwineventhook;
#endif
#if !defined(NOCLIPBOARD)
    using unique_close_clipboard_call = unique_call<decltype(::CloseClipboard), &::CloseClipboard>;

    inline unique_close_clipboard_call open_clipboard(HWND hwnd)
    {
        return unique_close_clipboard_call { OpenClipboard(hwnd) != FALSE };
    }
#endif
#endif // __WIL__WINUSER_

#if !defined(NOGDI) && !defined(NODESKTOP)
    typedef unique_any<HDESK, decltype(&::CloseDesktop), ::CloseDesktop> unique_hdesk;
    typedef unique_any<HWINSTA, decltype(&::CloseWindowStation), ::CloseWindowStation> unique_hwinsta;
#endif // !defined(NOGDI) && !defined(NODESKTOP)

#endif
#if defined(__WIL_WINBASE_DESKTOP) && !defined(__WIL_WINBASE_DESKTOP_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_WINBASE_DESKTOP_STL
    typedef shared_any<unique_hheap> shared_hheap;
    typedef shared_any<unique_hlocal> shared_hlocal;
    typedef shared_any<unique_tls> shared_tls;
    typedef shared_any<unique_hlocal_security_descriptor> shared_hlocal_security_descriptor;
    typedef shared_any<unique_private_security_descriptor> shared_private_security_descriptor;
    typedef shared_any<unique_haccel> shared_haccel;
    typedef shared_any<unique_hcursor> shared_hcursor;
#if !defined(NOGDI) && !defined(NODESKTOP)
    typedef shared_any<unique_hdesk> shared_hdesk;
    typedef shared_any<unique_hwinsta> shared_hwinsta;
#endif // !defined(NOGDI) && !defined(NODESKTOP)
    typedef shared_any<unique_hwnd> shared_hwnd;
#if !defined(NOUSER) && !defined(NOWH)
    typedef shared_any<unique_hhook> shared_hhook;
#endif
#if !defined(NOWINABLE)
    typedef shared_any<unique_hwineventhook> shared_hwineventhook;
#endif

    typedef weak_any<shared_hheap> weak_hheap;
    typedef weak_any<shared_hlocal> weak_hlocal;
    typedef weak_any<shared_tls> weak_tls;
    typedef weak_any<shared_hlocal_security_descriptor> weak_hlocal_security_descriptor;
    typedef weak_any<shared_private_security_descriptor> weak_private_security_descriptor;
    typedef weak_any<shared_haccel> weak_haccel;
    typedef weak_any<shared_hcursor> weak_hcursor;
#if !defined(NOGDI) && !defined(NODESKTOP)
    typedef weak_any<shared_hdesk> weak_hdesk;
    typedef weak_any<shared_hwinsta> weak_hwinsta;
#endif // !defined(NOGDI) && !defined(NODESKTOP)
    typedef weak_any<shared_hwnd> weak_hwnd;
#if !defined(NOUSER) && !defined(NOWH)
    typedef weak_any<shared_hhook> weak_hhook;
#endif
#if !defined(NOWINABLE)
    typedef weak_any<shared_hwineventhook> weak_hwineventhook;
#endif
#endif // __WIL_WINBASE_DESKTOP_STL

#if defined(_COMBASEAPI_H_) && !defined(__WIL__COMBASEAPI_H_) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_SYSTEM) && !defined(WIL_KERNEL_MODE)
#define __WIL__COMBASEAPI_H_
#if (NTDDI_VERSION >= NTDDI_WIN8)
    typedef unique_any<CO_MTA_USAGE_COOKIE, decltype(&::CoDecrementMTAUsage), ::CoDecrementMTAUsage> unique_mta_usage_cookie;
#endif

    typedef unique_any<DWORD, decltype(&::CoRevokeClassObject), ::CoRevokeClassObject> unique_com_class_object_cookie;

    /// @cond
    namespace details
    {
        inline void __stdcall MultiQiCleanup(_In_ MULTI_QI* multiQi)
        {
            if (multiQi->pItf)
            {
                multiQi->pItf->Release();
                multiQi->pItf = nullptr;
            }
        }
    }
    /// @endcond

    //! A type that calls CoRevertToSelf on destruction (or reset()).
    using unique_coreverttoself_call = unique_call<decltype(&::CoRevertToSelf), ::CoRevertToSelf>;

    //! Calls CoImpersonateClient and fail-fasts if it fails; returns an RAII object that reverts
    WI_NODISCARD inline unique_coreverttoself_call CoImpersonateClient_failfast()
    {
        FAIL_FAST_IF_FAILED(::CoImpersonateClient());
        return unique_coreverttoself_call();
    }

    typedef unique_struct<MULTI_QI, decltype(&details::MultiQiCleanup), details::MultiQiCleanup> unique_multi_qi;
#endif // __WIL__COMBASEAPI_H_
#if defined(__WIL__COMBASEAPI_H_) && defined(WIL_ENABLE_EXCEPTIONS) && !defined(__WIL__COMBASEAPI_H_EXCEPTIONAL)
#define __WIL__COMBASEAPI_H_EXCEPTIONAL
    WI_NODISCARD inline unique_coreverttoself_call CoImpersonateClient()
    {
        THROW_IF_FAILED(::CoImpersonateClient());
        return unique_coreverttoself_call();
    }
#endif
#if defined(__WIL__COMBASEAPI_H_) && !defined(__WIL__COMBASEAPI_H__STL) && defined(WIL_RESOURCE_STL) && (NTDDI_VERSION >= NTDDI_WIN8)
#define __WIL__COMBASEAPI_H__STL
    typedef shared_any<unique_mta_usage_cookie> shared_mta_usage_cookie;
    typedef weak_any<shared_mta_usage_cookie> weak_mta_usage_cookie;
#endif // __WIL__COMBASEAPI_H__STL

#if defined(_COMBASEAPI_H_) && !defined(__WIL__COMBASEAPI_H_APP) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_APP | WINAPI_PARTITION_SYSTEM) && !defined(WIL_KERNEL_MODE)
#define __WIL__COMBASEAPI_H_APP
    //! A type that calls CoUninitialize on destruction (or reset()).
    using unique_couninitialize_call = unique_call<decltype(&::CoUninitialize), ::CoUninitialize>;

    //! Calls CoInitializeEx and fail-fasts if it fails; returns an RAII object that reverts
    WI_NODISCARD inline unique_couninitialize_call CoInitializeEx_failfast(DWORD coinitFlags = 0 /*COINIT_MULTITHREADED*/)
    {
        FAIL_FAST_IF_FAILED(::CoInitializeEx(nullptr, coinitFlags));
        return {};
    }
#endif // __WIL__COMBASEAPI_H_APP
#if defined(__WIL__COMBASEAPI_H_APP) && defined(WIL_ENABLE_EXCEPTIONS) && !defined(__WIL__COMBASEAPI_H_APPEXCEPTIONAL)
#define __WIL__COMBASEAPI_H_APPEXCEPTIONAL
    WI_NODISCARD inline unique_couninitialize_call CoInitializeEx(DWORD coinitFlags = 0 /*COINIT_MULTITHREADED*/)
    {
        THROW_IF_FAILED(::CoInitializeEx(nullptr, coinitFlags));
        return {};
    }
#endif

#if defined(__ROAPI_H_) && !defined(__WIL__ROAPI_H_APP) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_APP | WINAPI_PARTITION_SYSTEM) && (NTDDI_VERSION >= NTDDI_WIN8)
#define __WIL__ROAPI_H_APP

    typedef unique_any<RO_REGISTRATION_COOKIE, decltype(&::RoRevokeActivationFactories), ::RoRevokeActivationFactories> unique_ro_registration_cookie;

    //! A type that calls RoUninitialize on destruction (or reset()).
    //! Use as a replacement for Windows::Foundation::Uninitialize.
    using unique_rouninitialize_call = unique_call<decltype(&::RoUninitialize), ::RoUninitialize>;

    //! Calls RoInitialize and fail-fasts if it fails; returns an RAII object that reverts
    //! Use as a replacement for Windows::Foundation::Initialize
    WI_NODISCARD inline unique_rouninitialize_call RoInitialize_failfast(RO_INIT_TYPE initType = RO_INIT_MULTITHREADED)
    {
        FAIL_FAST_IF_FAILED(::RoInitialize(initType));
        return unique_rouninitialize_call();
    }
#endif // __WIL__ROAPI_H_APP
#if defined(__WIL__ROAPI_H_APP) && defined(WIL_ENABLE_EXCEPTIONS) && !defined(__WIL__ROAPI_H_APPEXCEPTIONAL)
#define __WIL__ROAPI_H_APPEXCEPTIONAL
    //! Calls RoInitialize and throws an exception if it fails; returns an RAII object that reverts
    //! Use as a replacement for Windows::Foundation::Initialize
    WI_NODISCARD inline unique_rouninitialize_call RoInitialize(RO_INIT_TYPE initType = RO_INIT_MULTITHREADED)
    {
        THROW_IF_FAILED(::RoInitialize(initType));
        return unique_rouninitialize_call();
    }
#endif

#if defined(__WINSTRING_H_) && !defined(__WIL__WINSTRING_H_) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_APP | WINAPI_PARTITION_SYSTEM)
#define __WIL__WINSTRING_H_
    typedef unique_any<HSTRING, decltype(&::WindowsDeleteString), ::WindowsDeleteString> unique_hstring;

    template<> inline unique_hstring make_unique_string_nothrow<unique_hstring>(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCWSTR source, size_t length) WI_NOEXCEPT
    {
        WI_ASSERT(source != nullptr); // the HSTRING version of this function does not suport this case
        if (length == static_cast<size_t>(-1))
        {
            length = wcslen(source);
        }

        unique_hstring result;
        ::WindowsCreateString(source, static_cast<UINT32>(length), &result);
        return result;
    }

    typedef unique_any<HSTRING_BUFFER, decltype(&::WindowsDeleteStringBuffer), ::WindowsDeleteStringBuffer> unique_hstring_buffer;

    /** Promotes an hstring_buffer to an HSTRING.
    When an HSTRING_BUFFER object is promoted to a real string it must not be passed to WindowsDeleteString. The caller owns the
    HSTRING afterwards.
    ~~~
    HRESULT Type::MakePath(_Out_ HSTRING* path)
    {
        wchar_t* bufferStorage = nullptr;
        wil::unique_hstring_buffer theBuffer;
        RETURN_IF_FAILED(::WindowsPreallocateStringBuffer(65, &bufferStorage, &theBuffer));
        RETURN_IF_FAILED(::PathCchCombine(bufferStorage, 65, m_foo, m_bar));
        RETURN_IF_FAILED(wil::make_hstring_from_buffer_nothrow(wistd::move(theBuffer), path)));
        return S_OK;
    }
    ~~~
    */
    inline HRESULT make_hstring_from_buffer_nothrow(unique_hstring_buffer&& source, _Out_ HSTRING* promoted)
    {
        HRESULT hr = ::WindowsPromoteStringBuffer(source.get(), promoted);
        if (SUCCEEDED(hr))
        {
            source.release();
        }
        return hr;
    }

    //! A fail-fast variant of `make_hstring_from_buffer_nothrow`
    inline unique_hstring make_hstring_from_buffer_failfast(unique_hstring_buffer&& source)
    {
        unique_hstring result;
        FAIL_FAST_IF_FAILED(make_hstring_from_buffer_nothrow(wistd::move(source), &result));
        return result;
    }

#if defined WIL_ENABLE_EXCEPTIONS
    /** Promotes an hstring_buffer to an HSTRING.
    When an HSTRING_BUFFER object is promoted to a real string it must not be passed to WindowsDeleteString. The caller owns the
    HSTRING afterwards.
    ~~~
    wil::unique_hstring Type::Make()
    {
        wchar_t* bufferStorage = nullptr;
        wil::unique_hstring_buffer theBuffer;
        THROW_IF_FAILED(::WindowsPreallocateStringBuffer(65, &bufferStorage, &theBuffer));
        THROW_IF_FAILED(::PathCchCombine(bufferStorage, 65, m_foo, m_bar));
        return wil::make_hstring_from_buffer(wistd::move(theBuffer));
    }
    ~~~
    */
    inline unique_hstring make_hstring_from_buffer(unique_hstring_buffer&& source)
    {
        unique_hstring result;
        THROW_IF_FAILED(make_hstring_from_buffer_nothrow(wistd::move(source), &result));
        return result;
    }
#endif

    /// @cond
    namespace details
    {
        template<> struct string_maker<unique_hstring>
        {
            string_maker() = default;
            string_maker(const string_maker&) = delete;
            void operator=(const string_maker&) = delete;
            string_maker& operator=(string_maker&& source) WI_NOEXCEPT
            {
                m_value = wistd::move(source.m_value);
                m_bufferHandle = wistd::move(source.m_bufferHandle);
                m_charBuffer = wistd::exchange(source.m_charBuffer, nullptr);
                return *this;
            }

            HRESULT make(
                _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
                _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
                const wchar_t* source,
                size_t length)
            {
                if (source)
                {
                    RETURN_IF_FAILED(WindowsCreateString(source, static_cast<UINT32>(length), &m_value));
                    m_charBuffer = nullptr;
                    m_bufferHandle.reset(); // do this after WindowsCreateString so we can trim_at_existing_null() from our own buffer
                }
                else
                {
                    // Need to set it to the empty string to support the empty string case.
                    m_value.reset();
                    RETURN_IF_FAILED(WindowsPreallocateStringBuffer(static_cast<UINT32>(length), &m_charBuffer, &m_bufferHandle));
                }
                return S_OK;
            }

            WI_NODISCARD wchar_t* buffer() { WI_ASSERT(m_charBuffer != nullptr);  return m_charBuffer; }
            WI_NODISCARD const wchar_t* buffer() const { return m_charBuffer; }

            HRESULT trim_at_existing_null(size_t length) { return make(buffer(), length); }

            unique_hstring release()
            {
                m_charBuffer = nullptr;
                if (m_bufferHandle)
                {
                    return make_hstring_from_buffer_failfast(wistd::move(m_bufferHandle));
                }
                return wistd::move(m_value);
            }

            static PCWSTR get(const wil::unique_hstring& value) { return WindowsGetStringRawBuffer(value.get(), nullptr); }

        private:
            unique_hstring m_value;
            unique_hstring_buffer m_bufferHandle;
            wchar_t* m_charBuffer = nullptr;
        };
    }
    /// @endcond

    // str_raw_ptr is an overloaded function that retrieves a const pointer to the first character in a string's buffer.
    // This is the overload for HSTRING.  Other overloads available above.
    inline PCWSTR str_raw_ptr(HSTRING str)
    {
        return WindowsGetStringRawBuffer(str, nullptr);
    }

    inline PCWSTR str_raw_ptr(const unique_hstring& str)
    {
        return str_raw_ptr(str.get());
    }

#endif // __WIL__WINSTRING_H_
#if defined(__WIL__WINSTRING_H_) && !defined(__WIL__WINSTRING_H_STL) && defined(WIL_RESOURCE_STL)
#define __WIL__WINSTRING_H_STL
    typedef shared_any<unique_hstring> shared_hstring;
    typedef shared_any<unique_hstring_buffer> shared_hstring_buffer;
    typedef weak_any<shared_hstring> weak_hstring;
    typedef weak_any<shared_hstring_buffer> weak_hstring_buffer;
#endif // __WIL__WINSTRING_H_STL


#if defined(_WINREG_) && !defined(__WIL_WINREG_) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP) && !defined(WIL_KERNEL_MODE)
#define __WIL_WINREG_
    typedef unique_any<HKEY, decltype(&::RegCloseKey), ::RegCloseKey> unique_hkey;
#endif // __WIL_WINREG_
#if defined(__WIL_WINREG_) && !defined(__WIL_WINREG_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_WINREG_STL
    typedef shared_any<unique_hkey> shared_hkey;
    typedef weak_any<shared_hkey> weak_hkey;
#endif // __WIL_WINREG_STL

#if defined(__propidl_h__) && !defined(_WIL__propidl_h__) && !defined(WIL_KERNEL_MODE)
#define _WIL__propidl_h__
    // if language extensions (/Za) disabled, PropVariantInit will not exist, PROPVARIANT has forward declaration only
#if defined(_MSC_EXTENSIONS)
    using unique_prop_variant = wil::unique_struct<PROPVARIANT, decltype(&::PropVariantClear), ::PropVariantClear, decltype(&::PropVariantInit), ::PropVariantInit>;
#endif
#endif // _WIL__propidl_h__

#if defined(_OLEAUTO_H_) && !defined(__WIL_OLEAUTO_H_) && !defined(WIL_KERNEL_MODE) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_APP | WINAPI_PARTITION_SYSTEM)
#define __WIL_OLEAUTO_H_
    using unique_variant = wil::unique_struct<VARIANT, decltype(&::VariantClear), ::VariantClear, decltype(&::VariantInit), ::VariantInit>;
    typedef unique_any<BSTR, decltype(&::SysFreeString), ::SysFreeString> unique_bstr;

    inline wil::unique_bstr make_bstr_nothrow(PCWSTR source) WI_NOEXCEPT
    {
        return wil::unique_bstr(::SysAllocString(source));
    }

    inline wil::unique_bstr make_bstr_failfast(PCWSTR source) WI_NOEXCEPT
    {
        return wil::unique_bstr(FAIL_FAST_IF_NULL_ALLOC(::SysAllocString(source)));
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    inline wil::unique_bstr make_bstr(PCWSTR source)
    {
        wil::unique_bstr result(make_bstr_nothrow(source));
        THROW_IF_NULL_ALLOC(result);
        return result;
    }
#endif // WIL_ENABLE_EXCEPTIONS

    inline wil::unique_variant make_variant_bstr_nothrow(PCWSTR source) WI_NOEXCEPT
    {
        wil::unique_variant result{};
        V_UNION(result.addressof(), bstrVal) = ::SysAllocString(source);
        if (V_UNION(result.addressof(), bstrVal) != nullptr)
        {
            V_VT(result.addressof()) = VT_BSTR;
        }
        return result;
    }

    inline wil::unique_variant make_variant_bstr_failfast(PCWSTR source) WI_NOEXCEPT
    {
        auto result{make_variant_bstr_nothrow(source)};
        FAIL_FAST_HR_IF(E_OUTOFMEMORY, V_VT(result.addressof()) == VT_EMPTY);
        return result;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    inline wil::unique_variant make_variant_bstr(PCWSTR source)
    {
        auto result{make_variant_bstr_nothrow(source)};
        THROW_HR_IF(E_OUTOFMEMORY, V_VT(result.addressof()) == VT_EMPTY);
        return result;
    }
#endif // WIL_ENABLE_EXCEPTIONS

#endif // __WIL_OLEAUTO_H_
#if defined(__WIL_OLEAUTO_H_) && !defined(__WIL_OLEAUTO_H_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_OLEAUTO_H_STL
    typedef shared_any<unique_bstr> shared_bstr;
    typedef weak_any<shared_bstr> weak_bstr;
#endif // __WIL_OLEAUTO_H_STL


#if (defined(_WININET_) || defined(_DUBINET_)) && !defined(__WIL_WININET_)
#define __WIL_WININET_
    typedef unique_any<HINTERNET, decltype(&::InternetCloseHandle), ::InternetCloseHandle> unique_hinternet;
#endif // __WIL_WININET_
#if defined(__WIL_WININET_) && !defined(__WIL_WININET_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_WININET_STL
    typedef shared_any<unique_hinternet> shared_hinternet;
    typedef weak_any<shared_hinternet> weak_hinternet;
#endif // __WIL_WININET_STL


#if defined(_WINHTTPX_) && !defined(__WIL_WINHTTP_)
#define __WIL_WINHTTP_
    typedef unique_any<HINTERNET, decltype(&::WinHttpCloseHandle), ::WinHttpCloseHandle> unique_winhttp_hinternet;
#endif // __WIL_WINHTTP_
#if defined(__WIL_WINHTTP_) && !defined(__WIL_WINHTTP_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_WINHTTP_STL
    typedef shared_any<unique_winhttp_hinternet> shared_winhttp_hinternet;
    typedef weak_any<shared_winhttp_hinternet> weak_winhttp_hinternet;
#endif // __WIL_WINHTTP_STL


#if defined(_WINSOCKAPI_) && !defined(__WIL_WINSOCKAPI_) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_WINSOCKAPI_
    typedef unique_any<SOCKET, int (WINAPI*)(SOCKET), ::closesocket, details::pointer_access_all, SOCKET, SOCKET, INVALID_SOCKET, SOCKET> unique_socket;
#endif // __WIL_WINSOCKAPI_
#if defined(__WIL_WINSOCKAPI_) && !defined(__WIL_WINSOCKAPI_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_WINSOCKAPI_STL
    typedef shared_any<unique_socket> shared_socket;
    typedef weak_any<shared_socket> weak_socket;
#endif // __WIL_WINSOCKAPI_STL


#if defined(_WINGDI_) && !defined(__WIL_WINGDI_) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP) && !defined(NOGDI) && !defined(WIL_KERNEL_MODE)
#define __WIL_WINGDI_
    struct window_dc
    {
        HDC dc;
        HWND hwnd;
        window_dc(HDC dc_, HWND hwnd_ = nullptr) WI_NOEXCEPT { dc = dc_; hwnd = hwnd_; }
        WI_NODISCARD operator HDC() const WI_NOEXCEPT { return dc; }
        static void close(window_dc wdc) WI_NOEXCEPT { ::ReleaseDC(wdc.hwnd, wdc.dc); }
    };
    typedef unique_any<HDC, decltype(&window_dc::close), window_dc::close, details::pointer_access_all, window_dc> unique_hdc_window;

    struct paint_dc
    {
        HWND hwnd;
        PAINTSTRUCT ps;
        paint_dc(HDC hdc = nullptr) { ::ZeroMemory(this, sizeof(*this)); ps.hdc = hdc; }
        WI_NODISCARD operator HDC() const WI_NOEXCEPT { return ps.hdc; }
        static void close(paint_dc pdc) WI_NOEXCEPT { ::EndPaint(pdc.hwnd, &pdc.ps); }
    };
    typedef unique_any<HDC, decltype(&paint_dc::close), paint_dc::close, details::pointer_access_all, paint_dc> unique_hdc_paint;

    struct select_result
    {
        HGDIOBJ hgdi;
        HDC hdc;
        select_result(HGDIOBJ hgdi_, HDC hdc_ = nullptr) WI_NOEXCEPT { hgdi = hgdi_; hdc = hdc_; }
        WI_NODISCARD operator HGDIOBJ() const WI_NOEXCEPT { return hgdi; }
        static void close(select_result sr) WI_NOEXCEPT { ::SelectObject(sr.hdc, sr.hgdi); }
    };
    typedef unique_any<HGDIOBJ, decltype(&select_result::close), select_result::close, details::pointer_access_all, select_result> unique_select_object;

    inline unique_hdc_window GetDC(HWND hwnd) WI_NOEXCEPT
    {
        return unique_hdc_window(window_dc(::GetDC(hwnd), hwnd));
    }

    inline unique_hdc_window GetWindowDC(HWND hwnd) WI_NOEXCEPT
    {
        return unique_hdc_window(window_dc(::GetWindowDC(hwnd), hwnd));
    }

    inline unique_hdc_paint BeginPaint(HWND hwnd, _Out_opt_ PPAINTSTRUCT pPaintStruct = nullptr) WI_NOEXCEPT
    {
        paint_dc pdc;
        pdc.hwnd = hwnd;
        HDC hdc = ::BeginPaint(hwnd, &pdc.ps);
        assign_to_opt_param(pPaintStruct, pdc.ps);
        return (hdc == nullptr) ? unique_hdc_paint() : unique_hdc_paint(pdc);
    }

    inline unique_select_object SelectObject(HDC hdc, HGDIOBJ gdiobj) WI_NOEXCEPT
    {
        return unique_select_object(select_result(::SelectObject(hdc, gdiobj), hdc));
    }

    typedef unique_any<HGDIOBJ, decltype(&::DeleteObject), ::DeleteObject> unique_hgdiobj;
    typedef unique_any<HPEN, decltype(&::DeleteObject), ::DeleteObject> unique_hpen;
    typedef unique_any<HBRUSH, decltype(&::DeleteObject), ::DeleteObject> unique_hbrush;
    typedef unique_any<HFONT, decltype(&::DeleteObject), ::DeleteObject> unique_hfont;
    typedef unique_any<HBITMAP, decltype(&::DeleteObject), ::DeleteObject> unique_hbitmap;
    typedef unique_any<HRGN, decltype(&::DeleteObject), ::DeleteObject> unique_hrgn;
    typedef unique_any<HPALETTE, decltype(&::DeleteObject), ::DeleteObject> unique_hpalette;
    typedef unique_any<HDC, decltype(&::DeleteDC), ::DeleteDC> unique_hdc;
    typedef unique_any<HICON, decltype(&::DestroyIcon), ::DestroyIcon> unique_hicon;
#if !defined(NOMENUS)
    typedef unique_any<HMENU, decltype(&::DestroyMenu), ::DestroyMenu> unique_hmenu;
#endif // !defined(NOMENUS)
#endif // __WIL_WINGDI_
#if defined(__WIL_WINGDI_) && !defined(__WIL_WINGDI_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_WINGDI_STL
    typedef shared_any<unique_hgdiobj> shared_hgdiobj;
    typedef shared_any<unique_hpen> shared_hpen;
    typedef shared_any<unique_hbrush> shared_hbrush;
    typedef shared_any<unique_hfont> shared_hfont;
    typedef shared_any<unique_hbitmap> shared_hbitmap;
    typedef shared_any<unique_hrgn> shared_hrgn;
    typedef shared_any<unique_hpalette> shared_hpalette;
    typedef shared_any<unique_hdc> shared_hdc;
    typedef shared_any<unique_hicon> shared_hicon;
#if !defined(NOMENUS)
    typedef shared_any<unique_hmenu> shared_hmenu;
#endif // !defined(NOMENUS)

    typedef weak_any<shared_hgdiobj> weak_hgdiobj;
    typedef weak_any<shared_hpen> weak_hpen;
    typedef weak_any<shared_hbrush> weak_hbrush;
    typedef weak_any<shared_hfont> weak_hfont;
    typedef weak_any<shared_hbitmap> weak_hbitmap;
    typedef weak_any<shared_hrgn> weak_hrgn;
    typedef weak_any<shared_hpalette> weak_hpalette;
    typedef weak_any<shared_hdc> weak_hdc;
    typedef weak_any<shared_hicon> weak_hicon;
#if !defined(NOMENUS)
    typedef weak_any<shared_hmenu> weak_hmenu;
#endif // !defined(NOMENUS)
#endif // __WIL_WINGDI_STL

#if defined(_INC_WTSAPI) && !defined(__WIL_WTSAPI)
#define __WIL_WTSAPI
    template<typename T>
    using unique_wtsmem_ptr = wistd::unique_ptr<T, function_deleter<decltype(&WTSFreeMemory), WTSFreeMemory>>;
#endif // __WIL_WTSAPI

#if defined(_WINSCARD_H_) && !defined(__WIL_WINSCARD_H_) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_WINSCARD_H_
    typedef unique_any<SCARDCONTEXT, decltype(&::SCardReleaseContext), ::SCardReleaseContext> unique_scardctx;
#endif // __WIL_WINSCARD_H_
#if defined(__WIL_WINSCARD_H_) && !defined(__WIL_WINSCARD_H_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_WINSCARD_H_STL
    typedef shared_any<unique_scardctx> shared_scardctx;
    typedef weak_any<shared_scardctx> weak_scardctx;
#endif // __WIL_WINSCARD_H_STL


#if defined(__WINCRYPT_H__) && !defined(__WIL__WINCRYPT_H__) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL__WINCRYPT_H__
    /// @cond
    namespace details
    {
        inline void __stdcall CertCloseStoreNoParam(_Pre_opt_valid_ _Frees_ptr_opt_ HCERTSTORE hCertStore) WI_NOEXCEPT
        {
            ::CertCloseStore(hCertStore, 0);
        }

        inline void __stdcall CryptReleaseContextNoParam(_Pre_opt_valid_ _Frees_ptr_opt_ HCRYPTPROV hCryptCtx) WI_NOEXCEPT
        {
            ::CryptReleaseContext(hCryptCtx, 0);
        }
    }
    /// @endcond

    struct cert_context_t : details::unique_storage<details::resource_policy<PCCERT_CONTEXT, decltype(&::CertFreeCertificateContext), ::CertFreeCertificateContext>>
    {
        // forward all base class constructors...
        template <typename... args_t>
        explicit cert_context_t(args_t&&... args) WI_NOEXCEPT : unique_storage(wistd::forward<args_t>(args)...) {}

        /** A wrapper around CertEnumCertificatesInStore.
        CertEnumCertificatesInStore takes ownership of its second paramter in an unclear fashion,
        making it error-prone to use in combination with unique_cert_context. This wrapper helps
        manage the resource correctly while ensuring the GetLastError state set by CertEnumCertificatesInStore.
        is not lost. See MSDN for more information on `CertEnumCertificatesInStore`.
        ~~~~
        void MyMethod(HCERTSTORE certStore)
        {
            wil::unique_cert_context enumCert;
            while (enumCert.CertEnumCertificatesInStore(certStore))
            {
                UseTheCertToDoTheThing(enumCert);
            }
        }
        ~~~~
        @param certStore A handle of a certificate store.
        @param 'true' if a certificate was enumerated by this call, false otherwise.
        */
        bool CertEnumCertificatesInStore(HCERTSTORE certStore) WI_NOEXCEPT
        {
            reset(::CertEnumCertificatesInStore(certStore, release()));
            return is_valid();
        }
    };

    // Warning - ::CertEnumCertificatesInStore takes ownership of its parameter. Prefer the
    // .CertEnumCertificatesInStore method of the unique_cert_context or else use .release
    // when calling ::CertEnumCertificatesInStore directly.
    typedef unique_any_t<cert_context_t> unique_cert_context;
    typedef unique_any<PCCERT_CHAIN_CONTEXT, decltype(&::CertFreeCertificateChain), ::CertFreeCertificateChain> unique_cert_chain_context;
    typedef unique_any<HCERTSTORE, decltype(&details::CertCloseStoreNoParam), details::CertCloseStoreNoParam> unique_hcertstore;
    typedef unique_any<HCRYPTPROV, decltype(&details::CryptReleaseContextNoParam), details::CryptReleaseContextNoParam> unique_hcryptprov;
    typedef unique_any<HCRYPTKEY, decltype(&::CryptDestroyKey), ::CryptDestroyKey> unique_hcryptkey;
    typedef unique_any<HCRYPTHASH, decltype(&::CryptDestroyHash), ::CryptDestroyHash> unique_hcrypthash;
    typedef unique_any<HCRYPTMSG, decltype(&::CryptMsgClose), ::CryptMsgClose> unique_hcryptmsg;
#endif // __WIL__WINCRYPT_H__
#if defined(__WIL__WINCRYPT_H__) && !defined(__WIL__WINCRYPT_H__STL) && defined(WIL_RESOURCE_STL)
#define __WIL__WINCRYPT_H__STL
    typedef shared_any<unique_cert_context> shared_cert_context;
    typedef shared_any<unique_cert_chain_context> shared_cert_chain_context;
    typedef shared_any<unique_hcertstore> shared_hcertstore;
    typedef shared_any<unique_hcryptprov> shared_hcryptprov;
    typedef shared_any<unique_hcryptkey> shared_hcryptkey;
    typedef shared_any<unique_hcrypthash> shared_hcrypthash;
    typedef shared_any<unique_hcryptmsg> shared_hcryptmsg;

    typedef weak_any<shared_cert_context> weak_cert_context;
    typedef weak_any<shared_cert_chain_context> weak_cert_chain_context;
    typedef weak_any<shared_hcertstore> weak_hcertstore;
    typedef weak_any<shared_hcryptprov> weak_hcryptprov;
    typedef weak_any<shared_hcryptkey> weak_hcryptkey;
    typedef weak_any<shared_hcrypthash> weak_hcrypthash;
    typedef weak_any<shared_hcryptmsg> weak_hcryptmsg;
#endif // __WIL__WINCRYPT_H__STL


#if defined(__NCRYPT_H__) && !defined(__WIL_NCRYPT_H__) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_NCRYPT_H__
    using ncrypt_deleter = function_deleter<decltype(&::NCryptFreeBuffer), NCryptFreeBuffer>;

    template <typename T>
    using unique_ncrypt_ptr = wistd::unique_ptr<T, ncrypt_deleter>;

    typedef unique_any<NCRYPT_PROV_HANDLE, decltype(&::NCryptFreeObject), ::NCryptFreeObject> unique_ncrypt_prov;
    typedef unique_any<NCRYPT_KEY_HANDLE, decltype(&::NCryptFreeObject), ::NCryptFreeObject> unique_ncrypt_key;
    typedef unique_any<NCRYPT_SECRET_HANDLE, decltype(&::NCryptFreeObject), ::NCryptFreeObject> unique_ncrypt_secret;
#endif // __WIL_NCRYPT_H__
#if defined(__WIL_NCRYPT_H__) && !defined(__WIL_NCRYPT_H_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_NCRYPT_H_STL
    typedef shared_any<unique_ncrypt_prov> shared_ncrypt_prov;
    typedef shared_any<unique_ncrypt_key> shared_ncrypt_key;
    typedef shared_any<unique_ncrypt_secret> shared_ncrypt_secret;

    typedef weak_any<shared_ncrypt_prov> weak_ncrypt_prov;
    typedef weak_any<shared_ncrypt_key> weak_ncrypt_key;
    typedef weak_any<shared_ncrypt_secret> weak_ncrypt_secret;
#endif // __WIL_NCRYPT_H_STL

#if defined(__BCRYPT_H__) && !defined(__WIL_BCRYPT_H__) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_BCRYPT_H__
    /// @cond
    namespace details
    {
        inline void __stdcall BCryptCloseAlgorithmProviderNoFlags(_Pre_opt_valid_ _Frees_ptr_opt_ BCRYPT_ALG_HANDLE hAlgorithm) WI_NOEXCEPT
        {
            if (hAlgorithm)
            {
                ::BCryptCloseAlgorithmProvider(hAlgorithm, 0);
            }
        }
    }
    /// @endcond

    using bcrypt_deleter = function_deleter<decltype(&::BCryptFreeBuffer), BCryptFreeBuffer>;

    template <typename T>
    using unique_bcrypt_ptr = wistd::unique_ptr<T, bcrypt_deleter>;

    typedef unique_any<BCRYPT_ALG_HANDLE, decltype(&details::BCryptCloseAlgorithmProviderNoFlags), details::BCryptCloseAlgorithmProviderNoFlags> unique_bcrypt_algorithm;
    typedef unique_any<BCRYPT_HASH_HANDLE, decltype(&::BCryptDestroyHash), ::BCryptDestroyHash> unique_bcrypt_hash;
    typedef unique_any<BCRYPT_KEY_HANDLE, decltype(&::BCryptDestroyKey), ::BCryptDestroyKey> unique_bcrypt_key;
    typedef unique_any<BCRYPT_SECRET_HANDLE, decltype(&::BCryptDestroySecret), ::BCryptDestroySecret> unique_bcrypt_secret;
#endif // __WIL_BCRYPT_H__
#if defined(__WIL_BCRYPT_H__) && !defined(__WIL_BCRYPT_H_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_BCRYPT_H_STL
    typedef shared_any<unique_bcrypt_algorithm> shared_bcrypt_algorithm;
    typedef shared_any<unique_bcrypt_hash> shared_bcrypt_hash;
    typedef shared_any<unique_bcrypt_key> shared_bcrypt_key;
    typedef shared_any<unique_bcrypt_secret> shared_bcrypt_secret;

    typedef weak_any<shared_bcrypt_algorithm> weak_bcrypt_algorithm;
    typedef weak_any<shared_bcrypt_hash> weak_bcrypt_hash;
    typedef weak_any<unique_bcrypt_key> weak_bcrypt_key;
    typedef weak_any<shared_bcrypt_secret> weak_bcrypt_secret;
#endif // __WIL_BCRYPT_H_STL


#if defined(__RPCNDR_H__) && !defined(__WIL__RPCNDR_H__) && !defined(WIL_KERNEL_MODE)
#define __WIL__RPCNDR_H__

    //! Function deleter for use with pointers allocated by MIDL_user_allocate
    using midl_deleter = function_deleter<decltype(&::MIDL_user_free), MIDL_user_free>;

    //! Unique-ptr holding a type allocated by MIDL_user_alloc or returned from an RPC invocation
    template<typename T = void> using unique_midl_ptr = wistd::unique_ptr<T, midl_deleter>;

    //! Unique-ptr for strings allocated by MIDL_user_alloc
    using unique_midl_string = unique_midl_ptr<wchar_t>;
#ifndef WIL_NO_ANSI_STRINGS
    using unique_midl_ansistring = unique_midl_ptr<char>;
#endif

    namespace details
    {
        struct midl_allocator
        {
            static _Ret_opt_bytecap_(size) void* allocate(size_t size) WI_NOEXCEPT
            {
                return ::MIDL_user_allocate(size);
            }
        };

        // Specialization to support construction of unique_midl_string instances
        template<> struct string_allocator<unique_midl_string> : midl_allocator {};

#ifndef WIL_NO_ANSI_STRINGS
        template<> struct string_allocator<unique_midl_ansistring> : midl_allocator {};
#endif
    }
#endif // __WIL__RPCNDR_H__

#if defined(_OBJBASE_H_) && !defined(__WIL_OBJBASE_H_) && !defined(WIL_KERNEL_MODE)
#define __WIL_OBJBASE_H_
    using cotaskmem_deleter = function_deleter<decltype(&::CoTaskMemFree), ::CoTaskMemFree>;

    template <typename T = void>
    using unique_cotaskmem_ptr = wistd::unique_ptr<T, cotaskmem_deleter>;

    template <typename T>
    using unique_cotaskmem_array_ptr = unique_array_ptr<T, cotaskmem_deleter>;

    /** Provides `std::make_unique()` semantics for resources allocated with `CoTaskMemAlloc()` in a context that may not throw upon allocation failure.
    Use `wil::make_unique_cotaskmem_nothrow()` for resources returned from APIs that must satisfy a memory allocation contract that requires the use of `CoTaskMemAlloc()` / `CoTaskMemFree()`.
    Use `wil::make_unique_nothrow()` when `CoTaskMemAlloc()` is not required.

    Allocations are initialized with placement new and will call constructors (if present), but this does not guarantee initialization.

    Note that `wil::make_unique_cotaskmem_nothrow()` is not marked WI_NOEXCEPT as it may be used to create an exception-based class that may throw in its constructor.
    ~~~
    auto foo = wil::make_unique_cotaskmem_nothrow<Foo>();
    if (foo)
    {
    // initialize allocated Foo object as appropriate
    }
    ~~~
    */
    template <typename T, typename... Args>
    inline typename wistd::enable_if<!wistd::is_array<T>::value, unique_cotaskmem_ptr<T>>::type make_unique_cotaskmem_nothrow(Args&&... args)
    {
        static_assert(wistd::is_trivially_destructible<T>::value, "T has a destructor that won't be run when used with this function; use make_unique instead");
        unique_cotaskmem_ptr<T> sp(static_cast<T*>(::CoTaskMemAlloc(sizeof(T))));
        if (sp)
        {
            // use placement new to initialize memory from the previous allocation
            new (sp.get()) T(wistd::forward<Args>(args)...);
        }
        return sp;
    }

    /** Provides `std::make_unique()` semantics for array resources allocated with `CoTaskMemAlloc()` in a context that may not throw upon allocation failure.
    See the overload of `wil::make_unique_cotaskmem_nothrow()` for non-array types for more details.
    ~~~
    const size_t size = 42;
    auto foos = wil::make_unique_cotaskmem_nothrow<Foo[]>(size);
    if (foos)
    {
    for (auto& elem : wil::make_range(foos.get(), size))
    {
    // initialize allocated Foo objects as appropriate
    }
    }
    ~~~
    */
    template <typename T>
    inline typename wistd::enable_if<wistd::is_array<T>::value && wistd::extent<T>::value == 0, unique_cotaskmem_ptr<T>>::type make_unique_cotaskmem_nothrow(size_t size)
    {
        typedef typename wistd::remove_extent<T>::type E;
        static_assert(wistd::is_trivially_destructible<E>::value, "E has a destructor that won't be run when used with this function; use make_unique instead");
        FAIL_FAST_IF((__WI_SIZE_MAX / sizeof(E)) < size);
        size_t allocSize = sizeof(E) * size;
        unique_cotaskmem_ptr<T> sp(static_cast<E*>(::CoTaskMemAlloc(allocSize)));
        if (sp)
        {
            // use placement new to initialize memory from the previous allocation;
            // note that array placement new cannot be used as the standard allows for operator new[]
            // to consume overhead in the allocation for internal bookkeeping
            for (auto& elem : make_range(static_cast<E*>(sp.get()), size))
            {
                new (&elem) E();
            }
        }
        return sp;
    }

    /** Provides `std::make_unique()` semantics for resources allocated with `CoTaskMemAlloc()` in a context that must fail fast upon allocation failure.
    See the overload of `wil::make_unique_cotaskmem_nothrow()` for non-array types for more details.
    ~~~
    auto foo = wil::make_unique_cotaskmem_failfast<Foo>();
    // initialize allocated Foo object as appropriate
    ~~~
    */
    template <typename T, typename... Args>
    inline typename wistd::enable_if<!wistd::is_array<T>::value, unique_cotaskmem_ptr<T>>::type make_unique_cotaskmem_failfast(Args&&... args)
    {
        unique_cotaskmem_ptr<T> result(make_unique_cotaskmem_nothrow<T>(wistd::forward<Args>(args)...));
        FAIL_FAST_IF_NULL_ALLOC(result);
        return result;
    }

    /** Provides `std::make_unique()` semantics for array resources allocated with `CoTaskMemAlloc()` in a context that must fail fast upon allocation failure.
    See the overload of `wil::make_unique_cotaskmem_nothrow()` for non-array types for more details.
    ~~~
    const size_t size = 42;
    auto foos = wil::make_unique_cotaskmem_failfast<Foo[]>(size);
    for (auto& elem : wil::make_range(foos.get(), size))
    {
    // initialize allocated Foo objects as appropriate
    }
    ~~~
    */
    template <typename T>
    inline typename wistd::enable_if<wistd::is_array<T>::value && wistd::extent<T>::value == 0, unique_cotaskmem_ptr<T>>::type make_unique_cotaskmem_failfast(size_t size)
    {
        unique_cotaskmem_ptr<T> result(make_unique_cotaskmem_nothrow<T>(size));
        FAIL_FAST_IF_NULL_ALLOC(result);
        return result;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    /** Provides `std::make_unique()` semantics for resources allocated with `CoTaskMemAlloc()`.
    See the overload of `wil::make_unique_cotaskmem_nothrow()` for non-array types for more details.
    ~~~
    auto foo = wil::make_unique_cotaskmem<Foo>();
    // initialize allocated Foo object as appropriate
    ~~~
    */
    template <typename T, typename... Args>
    inline typename wistd::enable_if<!wistd::is_array<T>::value, unique_cotaskmem_ptr<T>>::type make_unique_cotaskmem(Args&&... args)
    {
        unique_cotaskmem_ptr<T> result(make_unique_cotaskmem_nothrow<T>(wistd::forward<Args>(args)...));
        THROW_IF_NULL_ALLOC(result);
        return result;
    }

    /** Provides `std::make_unique()` semantics for array resources allocated with `CoTaskMemAlloc()`.
    See the overload of `wil::make_unique_cotaskmem_nothrow()` for non-array types for more details.
    ~~~
    const size_t size = 42;
    auto foos = wil::make_unique_cotaskmem<Foo[]>(size);
    for (auto& elem : wil::make_range(foos.get(), size))
    {
    // initialize allocated Foo objects as appropriate
    }
    ~~~
    */
    template <typename T>
    inline typename wistd::enable_if<wistd::is_array<T>::value && wistd::extent<T>::value == 0, unique_cotaskmem_ptr<T>>::type make_unique_cotaskmem(size_t size)
    {
        unique_cotaskmem_ptr<T> result(make_unique_cotaskmem_nothrow<T>(size));
        THROW_IF_NULL_ALLOC(result);
        return result;
    }
#endif // WIL_ENABLE_EXCEPTIONS

    typedef unique_any<void*, decltype(&::CoTaskMemFree), ::CoTaskMemFree> unique_cotaskmem;
    typedef unique_any<PWSTR, decltype(&::CoTaskMemFree), ::CoTaskMemFree> unique_cotaskmem_string;
#ifndef WIL_NO_ANSI_STRINGS
    typedef unique_any<PSTR, decltype(&::CoTaskMemFree), ::CoTaskMemFree> unique_cotaskmem_ansistring;
#endif // WIL_NO_ANSI_STRINGS

    /// @cond
    namespace details
    {
        struct cotaskmem_allocator
        {
            static _Ret_opt_bytecap_(size) void* allocate(size_t size) WI_NOEXCEPT
            {
                return ::CoTaskMemAlloc(size);
            }
        };

        template<> struct string_allocator<unique_cotaskmem_string> : cotaskmem_allocator {};

#ifndef WIL_NO_ANSI_STRINGS
        template<> struct string_allocator<unique_cotaskmem_ansistring> : cotaskmem_allocator {};
#endif // WIL_NO_ANSI_STRINGS
    }
    /// @endcond

    inline auto make_cotaskmem_string_nothrow(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCWSTR source, size_t length = static_cast<size_t>(-1)) WI_NOEXCEPT
    {
        return make_unique_string_nothrow<unique_cotaskmem_string>(source, length);
    }

    inline auto make_cotaskmem_string_failfast(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCWSTR source, size_t length = static_cast<size_t>(-1)) WI_NOEXCEPT
    {
        return make_unique_string_failfast<unique_cotaskmem_string>(source, length);
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    inline auto make_cotaskmem_string(
        _When_((source != nullptr) && length != static_cast<size_t>(-1), _In_reads_(length))
        _When_((source != nullptr) && length == static_cast<size_t>(-1), _In_z_)
        PCWSTR source, size_t length = static_cast<size_t>(-1))
    {
        return make_unique_string<unique_cotaskmem_string>(source, length);
    }

#endif // WIL_ENABLE_EXCEPTIONS
#endif // __WIL_OBJBASE_H_
#if defined(__WIL_OBJBASE_H_) && !defined(__WIL_OBJBASE_H_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_OBJBASE_H_STL
    typedef shared_any<unique_cotaskmem> shared_cotaskmem;
    typedef weak_any<shared_cotaskmem> weak_cotaskmem;
    typedef shared_any<unique_cotaskmem_string> shared_cotaskmem_string;
    typedef weak_any<shared_cotaskmem_string> weak_cotaskmem_string;
#endif // __WIL_OBJBASE_H_STL

#if defined(__WIL_OBJBASE_H_) && defined(__WIL_WINBASE_) && !defined(__WIL_OBJBASE_AND_WINBASE_H_) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_OBJBASE_AND_WINBASE_H_

    struct cotaskmem_secure_deleter
    {
        template <typename T>
        void operator()(_Pre_opt_valid_ _Frees_ptr_opt_ T* p) const
        {
            if (p)
            {
                IMalloc* malloc;
                if (SUCCEEDED(::CoGetMalloc(1, &malloc)))
                {
                    size_t const size = malloc->GetSize(p);
                    if (size != static_cast<size_t>(-1))
                    {
                        ::SecureZeroMemory(p, size);
                    }
                    malloc->Release();
                }
                ::CoTaskMemFree(p);
            }
        }
    };

    template <typename T = void>
    using unique_cotaskmem_secure_ptr = wistd::unique_ptr<T, cotaskmem_secure_deleter>;

    /** Provides `std::make_unique()` semantics for secure resources allocated with `CoTaskMemAlloc()` in a context that may not throw upon allocation failure.
    See the overload of `wil::make_unique_cotaskmem_nothrow()` for non-array types for more details.
    ~~~
    auto foo = wil::make_unique_cotaskmem_secure_nothrow<Foo>();
    if (foo)
    {
    // initialize allocated Foo object as appropriate
    }
    ~~~
    */
    template <typename T, typename... Args>
    inline typename wistd::enable_if<!wistd::is_array<T>::value, unique_cotaskmem_secure_ptr<T>>::type make_unique_cotaskmem_secure_nothrow(Args&&... args)
    {
        return unique_cotaskmem_secure_ptr<T>(make_unique_cotaskmem_nothrow<T>(wistd::forward<Args>(args)...).release());
    }

    /** Provides `std::make_unique()` semantics for secure array resources allocated with `CoTaskMemAlloc()` in a context that may not throw upon allocation failure.
    See the overload of `wil::make_unique_cotaskmem_nothrow()` for non-array types for more details.
    ~~~
    const size_t size = 42;
    auto foos = wil::make_unique_cotaskmem_secure_nothrow<Foo[]>(size);
    if (foos)
    {
    for (auto& elem : wil::make_range(foos.get(), size))
    {
    // initialize allocated Foo objects as appropriate
    }
    }
    ~~~
    */
    template <typename T>
    inline typename wistd::enable_if<wistd::is_array<T>::value && wistd::extent<T>::value == 0, unique_cotaskmem_secure_ptr<T>>::type make_unique_cotaskmem_secure_nothrow(size_t size)
    {
        return unique_cotaskmem_secure_ptr<T>(make_unique_cotaskmem_nothrow<T>(size).release());
    }

    /** Provides `std::make_unique()` semantics for secure resources allocated with `CoTaskMemAlloc()` in a context that must fail fast upon allocation failure.
    See the overload of `wil::make_unique_cotaskmem_nothrow()` for non-array types for more details.
    ~~~
    auto foo = wil::make_unique_cotaskmem_secure_failfast<Foo>();
    // initialize allocated Foo object as appropriate
    ~~~
    */
    template <typename T, typename... Args>
    inline typename wistd::enable_if<!wistd::is_array<T>::value, unique_cotaskmem_secure_ptr<T>>::type make_unique_cotaskmem_secure_failfast(Args&&... args)
    {
        unique_cotaskmem_secure_ptr<T> result(make_unique_cotaskmem_secure_nothrow<T>(wistd::forward<Args>(args)...));
        FAIL_FAST_IF_NULL_ALLOC(result);
        return result;
    }

    /** Provides `std::make_unique()` semantics for secure array resources allocated with `CoTaskMemAlloc()` in a context that must fail fast upon allocation failure.
    See the overload of `wil::make_unique_cotaskmem_nothrow()` for non-array types for more details.
    ~~~
    const size_t size = 42;
    auto foos = wil::make_unique_cotaskmem_secure_failfast<Foo[]>(size);
    for (auto& elem : wil::make_range(foos.get(), size))
    {
    // initialize allocated Foo objects as appropriate
    }
    ~~~
    */
    template <typename T>
    inline typename wistd::enable_if<wistd::is_array<T>::value && wistd::extent<T>::value == 0, unique_cotaskmem_secure_ptr<T>>::type make_unique_cotaskmem_secure_failfast(size_t size)
    {
        unique_cotaskmem_secure_ptr<T> result(make_unique_cotaskmem_secure_nothrow<T>(size));
        FAIL_FAST_IF_NULL_ALLOC(result);
        return result;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    /** Provides `std::make_unique()` semantics for secure resources allocated with `CoTaskMemAlloc()`.
    See the overload of `wil::make_unique_cotaskmem_nothrow()` for non-array types for more details.
    ~~~
    auto foo = wil::make_unique_cotaskmem_secure<Foo>();
    // initialize allocated Foo object as appropriate
    ~~~
    */
    template <typename T, typename... Args>
    inline typename wistd::enable_if<!wistd::is_array<T>::value, unique_cotaskmem_secure_ptr<T>>::type make_unique_cotaskmem_secure(Args&&... args)
    {
        unique_cotaskmem_secure_ptr<T> result(make_unique_cotaskmem_secure_nothrow<T>(wistd::forward<Args>(args)...));
        THROW_IF_NULL_ALLOC(result);
        return result;
    }

    /** Provides `std::make_unique()` semantics for secure array resources allocated with `CoTaskMemAlloc()`.
    See the overload of `wil::make_unique_cotaskmem_nothrow()` for non-array types for more details.
    ~~~
    const size_t size = 42;
    auto foos = wil::make_unique_cotaskmem_secure<Foo[]>(size);
    for (auto& elem : wil::make_range(foos.get(), size))
    {
    // initialize allocated Foo objects as appropriate
    }
    ~~~
    */
    template <typename T>
    inline typename wistd::enable_if<wistd::is_array<T>::value && wistd::extent<T>::value == 0, unique_cotaskmem_secure_ptr<T>>::type make_unique_cotaskmem_secure(size_t size)
    {
        unique_cotaskmem_secure_ptr<T> result(make_unique_cotaskmem_secure_nothrow<T>(size));
        THROW_IF_NULL_ALLOC(result);
        return result;
    }
#endif // WIL_ENABLE_EXCEPTIONS

    typedef unique_cotaskmem_secure_ptr<wchar_t[]> unique_cotaskmem_string_secure;

    /** Copies a given string into secure memory allocated with `CoTaskMemAlloc()` in a context that may not throw upon allocation failure.
    See the overload of `wil::make_cotaskmem_string_nothrow()` with supplied length for more details.
    ~~~
    auto str = wil::make_cotaskmem_string_secure_nothrow(L"a string");
    if (str)
    {
    std::wcout << L"This is " << str.get() << std::endl; // prints "This is a string"
    }
    ~~~
    */
    inline unique_cotaskmem_string_secure make_cotaskmem_string_secure_nothrow(_In_ PCWSTR source) WI_NOEXCEPT
    {
        return unique_cotaskmem_string_secure(make_cotaskmem_string_nothrow(source).release());
    }

    /** Copies a given string into secure memory allocated with `CoTaskMemAlloc()` in a context that must fail fast upon allocation failure.
    See the overload of `wil::make_cotaskmem_string_nothrow()` with supplied length for more details.
    ~~~
    auto str = wil::make_cotaskmem_string_secure_failfast(L"a string");
    std::wcout << L"This is " << str.get() << std::endl; // prints "This is a string"
    ~~~
    */
    inline unique_cotaskmem_string_secure make_cotaskmem_string_secure_failfast(_In_ PCWSTR source) WI_NOEXCEPT
    {
        unique_cotaskmem_string_secure result(make_cotaskmem_string_secure_nothrow(source));
        FAIL_FAST_IF_NULL_ALLOC(result);
        return result;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    /** Copies a given string into secure memory allocated with `CoTaskMemAlloc()`.
    See the overload of `wil::make_cotaskmem_string_nothrow()` with supplied length for more details.
    ~~~
    auto str = wil::make_cotaskmem_string_secure(L"a string");
    std::wcout << L"This is " << str.get() << std::endl; // prints "This is a string"
    ~~~
    */
    inline unique_cotaskmem_string_secure make_cotaskmem_string_secure(_In_ PCWSTR source)
    {
        unique_cotaskmem_string_secure result(make_cotaskmem_string_secure_nothrow(source));
        THROW_IF_NULL_ALLOC(result);
        return result;
    }
#endif
#endif // __WIL_OBJBASE_AND_WINBASE_H_

#if defined(_OLE2_H_) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP) && !defined(__WIL_OLE2_H_) && !defined(WIL_KERNEL_MODE)
#define __WIL_OLE2_H_
    typedef unique_struct<STGMEDIUM, decltype(&::ReleaseStgMedium), ::ReleaseStgMedium> unique_stg_medium;
    struct unique_hglobal_locked : public unique_any<void*, decltype(&::GlobalUnlock), ::GlobalUnlock>
    {
        unique_hglobal_locked() = delete;

        explicit unique_hglobal_locked(HGLOBAL global) : unique_any<void*, decltype(&::GlobalUnlock), ::GlobalUnlock>(global)
        {
            // GlobalLock returns a pointer to the associated global memory block and that's what callers care about.
            m_globalMemory = GlobalLock(global);
            if (!m_globalMemory)
            {
                release();
            }
        }

        explicit unique_hglobal_locked(STGMEDIUM& medium) : unique_hglobal_locked(medium.hGlobal)
        {
        }

        WI_NODISCARD pointer get() const
        {
            return m_globalMemory;
        }

    private:
        pointer m_globalMemory;
    };

    //! A type that calls OleUninitialize on destruction (or reset()).
    //! Use as a replacement for Windows::Foundation::Uninitialize.
    using unique_oleuninitialize_call = unique_call<decltype(&::OleUninitialize), ::OleUninitialize>;

    //! Calls RoInitialize and fail-fasts if it fails; returns an RAII object that reverts
    //! Use as a replacement for Windows::Foundation::Initialize
    _Check_return_ inline unique_oleuninitialize_call OleInitialize_failfast()
    {
        FAIL_FAST_IF_FAILED(::OleInitialize(nullptr));
        return unique_oleuninitialize_call();
    }
#endif // __WIL_OLE2_H_

#if defined(__WIL_OLE2_H_) && defined(WIL_ENABLE_EXCEPTIONS) && !defined(__WIL_OLE2_H_EXCEPTIONAL)
#define __WIL_OLE2_H_EXCEPTIONAL
    //! Calls RoInitialize and throws an exception if it fails; returns an RAII object that reverts
    //! Use as a replacement for Windows::Foundation::Initialize
    _Check_return_ inline unique_oleuninitialize_call OleInitialize()
    {
        THROW_IF_FAILED(::OleInitialize(nullptr));
        return unique_oleuninitialize_call();
    }
#endif

#if defined(_INC_COMMCTRL) && !defined(__WIL_INC_COMMCTRL) && !defined(WIL_KERNEL_MODE)
#define __WIL_INC_COMMCTRL
    typedef unique_any<HIMAGELIST, decltype(&::ImageList_Destroy), ::ImageList_Destroy> unique_himagelist;
#endif // __WIL_INC_COMMCTRL
#if defined(__WIL_INC_COMMCTRL) && !defined(__WIL_INC_COMMCTRL_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_INC_COMMCTRL_STL
    typedef shared_any<unique_himagelist> shared_himagelist;
    typedef weak_any<shared_himagelist> weak_himagelist;
#endif // __WIL_INC_COMMCTRL_STL

#if defined(_UXTHEME_H_) && !defined(__WIL_INC_UXTHEME) && !defined(WIL_KERNEL_MODE)
#define __WIL_INC_UXTHEME
    typedef unique_any<HTHEME, decltype(&::CloseThemeData), ::CloseThemeData> unique_htheme;
#endif // __WIL_INC_UXTHEME

#pragma warning(push)
#pragma warning(disable:4995)
#if defined(_INC_USERENV) && !defined(__WIL_INC_USERENV) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_SYSTEM) && !defined(WIL_KERNEL_MODE)
#define __WIL_INC_USERENV
    typedef unique_any<void*, decltype(&::DestroyEnvironmentBlock), ::DestroyEnvironmentBlock> unique_environment_block;
#endif // __WIL_INC_USERENV
#pragma warning(pop)

#if defined(__WINEVT_H__) && !defined(__WIL_INC_EVT_HANDLE) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_PKG_EVENTLOGSERVICE) && !defined(WIL_KERNEL_MODE)
#define __WIL_INC_EVT_HANDLE
    typedef unique_any<EVT_HANDLE, decltype(&::EvtClose), ::EvtClose> unique_evt_handle;
#endif // __WIL_INC_EVT_HANDLE

#if defined(_WINSVC_) && !defined(__WIL_HANDLE_H_WINSVC) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP) && !defined(WIL_KERNEL_MODE)
#define __WIL_HANDLE_H_WINSVC
    typedef unique_any<SC_HANDLE, decltype(&::CloseServiceHandle), ::CloseServiceHandle> unique_schandle;
#endif // __WIL_HANDLE_H_WINSVC
#if defined(__WIL_HANDLE_H_WINSVC) && !defined(__WIL_HANDLE_H_WINSVC_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_HANDLE_H_WINSVC_STL
    typedef shared_any<unique_schandle> shared_schandle;
    typedef weak_any<shared_schandle> weak_schandle;
#endif // __WIL_HANDLE_H_WINSVC_STL

#if defined(_INC_STDIO) && !defined(__WIL_INC_STDIO) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP) && !defined(WIL_KERNEL_MODE)
#define __WIL_INC_STDIO
    typedef unique_any<FILE*, decltype(&::_pclose), ::_pclose> unique_pipe;
    typedef unique_any<FILE*, decltype(&::fclose), ::fclose> unique_file;
#endif // __WIL_INC_STDIO
#if defined(__WIL_INC_STDIO) && !defined(__WIL__INC_STDIO_STL) && defined(WIL_RESOURCE_STL)
#define __WIL__INC_STDIO_STL
    typedef shared_any<unique_pipe> shared_pipe;
    typedef weak_any<shared_pipe> weak_pipe;
    typedef shared_any<unique_file> shared_file;
    typedef weak_any<unique_file> weak_file;
#endif // __WIL__INC_STDIO_STL

#if defined(_INC_LOCALE) && !defined(__WIL_INC_LOCALE) && !defined(WIL_KERNEL_MODE)
#define __WIL_INC_LOCALE
    typedef unique_any<_locale_t, decltype(&::_free_locale), ::_free_locale> unique_locale;
#endif // __WIL_INC_LOCALE
#if defined(__WIL_INC_LOCALE) && !defined(__WIL__INC_LOCALE_STL) && defined(WIL_RESOURCE_STL)
#define __WIL__INC_LOCALE_STL
    typedef shared_any<unique_locale> shared_locale;
    typedef weak_any<unique_locale> weak_locale;
#endif // __WIL__INC_LOCALE_STL

#if defined(_NTLSA_) && !defined(__WIL_NTLSA_) && !defined(WIL_KERNEL_MODE)
#define __WIL_NTLSA_
    typedef unique_any<LSA_HANDLE, decltype(&::LsaClose), ::LsaClose> unique_hlsa;

    using lsa_freemem_deleter = function_deleter<decltype(&::LsaFreeMemory), LsaFreeMemory>;

    template <typename T>
    using unique_lsamem_ptr = wistd::unique_ptr<T, lsa_freemem_deleter>;
#endif // _NTLSA_
#if defined(_NTLSA_) && !defined(__WIL_NTLSA_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_NTLSA_STL
    typedef shared_any<unique_hlsa> shared_hlsa;
    typedef weak_any<shared_hlsa> weak_hlsa;
#endif // _NTLSA_

#if defined(_LSALOOKUP_) && !defined(__WIL_LSALOOKUP_)
#define __WIL_LSALOOKUP_
    typedef unique_any<LSA_HANDLE, decltype(&::LsaLookupClose), ::LsaLookupClose> unique_hlsalookup;

    using lsalookup_freemem_deleter = function_deleter<decltype(&::LsaLookupFreeMemory), LsaLookupFreeMemory>;

    template <typename T>
    using unique_lsalookupmem_ptr = wistd::unique_ptr<T, lsalookup_freemem_deleter>;
#endif // _LSALOOKUP_
#if defined(_LSALOOKUP_) && !defined(__WIL_LSALOOKUP_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_LSALOOKUP_STL
    typedef shared_any<unique_hlsalookup> shared_hlsalookup;
    typedef weak_any<shared_hlsalookup> weak_hlsalookup;
#endif // _LSALOOKUP_

#if defined(_NTLSA_IFS_) && !defined(__WIL_HANDLE_H_NTLSA_IFS_) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_HANDLE_H_NTLSA_IFS_
    using lsa_deleter = function_deleter<decltype(&::LsaFreeReturnBuffer), LsaFreeReturnBuffer>;

    template <typename T>
    using unique_lsa_ptr = wistd::unique_ptr<T, lsa_deleter>;
#endif // __WIL_HANDLE_H_NTLSA_IFS_

#if defined(__WERAPI_H__) && !defined(__WIL_WERAPI_H__) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_WERAPI_H__
    typedef unique_any<HREPORT, decltype(&WerReportCloseHandle), WerReportCloseHandle> unique_wer_report;
#endif

#if defined(__MIDLES_H__) && !defined(__WIL_MIDLES_H__) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_MIDLES_H__
    typedef unique_any<handle_t, decltype(&::MesHandleFree), ::MesHandleFree> unique_rpc_pickle;
#endif
#if defined(__WIL_MIDLES_H__) && !defined(__WIL_MIDLES_H_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_MIDLES_H_STL
    typedef shared_any<unique_rpc_pickle> shared_rpc_pickle;
    typedef weak_any<shared_rpc_pickle> weak_rpc_pickle;
#endif

#if defined(__RPCDCE_H__) && !defined(__WIL_RPCDCE_H__) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_RPCDCE_H__
    /// @cond
    namespace details
    {
        inline void __stdcall WpRpcBindingFree(_Pre_opt_valid_ _Frees_ptr_opt_ RPC_BINDING_HANDLE binding)
        {
            ::RpcBindingFree(&binding);
        }

        inline void __stdcall WpRpcBindingVectorFree(_Pre_opt_valid_ _Frees_ptr_opt_ RPC_BINDING_VECTOR* bindingVector)
        {
            ::RpcBindingVectorFree(&bindingVector);
        }

        inline void __stdcall WpRpcStringFree(_Pre_opt_valid_ _Frees_ptr_opt_ RPC_WSTR wstr)
        {
            ::RpcStringFreeW(&wstr);
        }
    }
    /// @endcond

    typedef unique_any<RPC_BINDING_HANDLE, decltype(&details::WpRpcBindingFree), details::WpRpcBindingFree> unique_rpc_binding;
    typedef unique_any<RPC_BINDING_VECTOR*, decltype(&details::WpRpcBindingVectorFree), details::WpRpcBindingVectorFree> unique_rpc_binding_vector;
    typedef unique_any<RPC_WSTR, decltype(&details::WpRpcStringFree), details::WpRpcStringFree> unique_rpc_wstr;
#endif
#if defined(__WIL_RPCDCE_H__) && !defined(__WIL_RPCDCE_H_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_RPCDCE_H_STL
    typedef shared_any<unique_rpc_binding> shared_rpc_binding;
    typedef weak_any<shared_rpc_binding> weak_rpc_binding;
    typedef shared_any<unique_rpc_binding_vector> shared_rpc_binding_vector;
    typedef weak_any<shared_rpc_binding_vector> weak_rpc_binding_vector;
    typedef shared_any<unique_rpc_wstr> shared_rpc_wstr;
    typedef weak_any<unique_rpc_wstr> weak_rpc_wstr;
#endif

#if defined(_WCMAPI_H) && !defined(__WIL_WCMAPI_H_) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_WCMAPI_H_
    using wcm_deleter = function_deleter<decltype(&::WcmFreeMemory), WcmFreeMemory>;

    template<typename T>
    using unique_wcm_ptr = wistd::unique_ptr<T, wcm_deleter>;
#endif

#if defined(_NETIOAPI_H_) && defined(_WS2IPDEF_) && defined(MIB_INVALID_TEREDO_PORT_NUMBER) && !defined(__WIL_NETIOAPI_H_) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_NETIOAPI_H_
    typedef unique_any<PMIB_IF_TABLE2, decltype(&::FreeMibTable), ::FreeMibTable> unique_mib_iftable;
#endif
#if defined(__WIL_NETIOAPI_H_) && !defined(__WIL_NETIOAPI_H_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_NETIOAPI_H_STL
    typedef shared_any<unique_mib_iftable> shared_mib_iftable;
    typedef weak_any<shared_mib_iftable> weak_mib_iftable;
#endif

#if defined(_WLAN_WLANAPI_H) && !defined(__WIL_WLAN_WLANAPI_H) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_WLAN_WLANAPI_H
    using wlan_deleter = function_deleter<decltype(&::WlanFreeMemory), ::WlanFreeMemory>;

    template<typename T>
    using unique_wlan_ptr = wistd::unique_ptr < T, wlan_deleter >;

    /// @cond
    namespace details
    {
        inline void __stdcall CloseWlanHandle(_Frees_ptr_ HANDLE hClientHandle)
        {
            ::WlanCloseHandle(hClientHandle, nullptr);
        }
    }
    /// @endcond

    typedef unique_any<HANDLE, decltype(&details::CloseWlanHandle), details::CloseWlanHandle, details::pointer_access_all, HANDLE, INT_PTR, -1> unique_wlan_handle;
#endif
#if defined(__WIL_WLAN_WLANAPI_H) && !defined(__WIL_WLAN_WLANAPI_H_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_WLAN_WLANAPI_H_STL
    typedef shared_any<unique_wlan_handle> shared_wlan_handle;
    typedef weak_any<shared_wlan_handle> weak_wlan_handle;
#endif

#if defined(_HPOWERNOTIFY_DEF_) && !defined(__WIL_HPOWERNOTIFY_DEF_H_) && !defined(WIL_KERNEL_MODE)
#define __WIL_HPOWERNOTIFY_DEF_H_
    typedef unique_any<HPOWERNOTIFY, decltype(&::UnregisterPowerSettingNotification), ::UnregisterPowerSettingNotification> unique_hpowernotify;
#endif

#if defined(__WIL_WINBASE_DESKTOP) && defined(SID_DEFINED) && !defined(__WIL_PSID_DEF_H_)
#define __WIL_PSID_DEF_H_
    typedef unique_any<PSID, decltype(&::LocalFree), ::LocalFree> unique_any_psid;
#if defined(_OBJBASE_H_)
    typedef unique_any<PSID, decltype(&::CoTaskMemFree), ::CoTaskMemFree> unique_cotaskmem_psid;
#endif
#endif

#if defined(_PROCESSTHREADSAPI_H_) && !defined(__WIL_PROCESSTHREADSAPI_H_DESK_SYS) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_SYSTEM) && !defined(WIL_KERNEL_MODE)
#define __WIL_PROCESSTHREADSAPI_H_DESK_SYS
    /// @cond
    namespace details
    {
        inline void __stdcall CloseProcessInformation(_In_ PROCESS_INFORMATION* p)
        {
            if (p->hProcess)
            {
                CloseHandle(p->hProcess);
            }

            if (p->hThread)
            {
                CloseHandle(p->hThread);
            }
        }
    }
    /// @endcond

    /** Manages the outbound parameter containing handles returned by `CreateProcess()` and related methods.
    ~~~
    unique_process_information process;
    CreateProcessW(..., CREATE_SUSPENDED, ..., &process);
    THROW_LAST_ERROR_IF(ResumeThread(process.hThread) == -1);
    THROW_LAST_ERROR_IF(WaitForSingleObject(process.hProcess, INFINITE) != WAIT_OBJECT_0);
    ~~~
    */
    using unique_process_information = unique_struct<PROCESS_INFORMATION, decltype(&details::CloseProcessInformation), details::CloseProcessInformation>;
#endif

#if defined(_PROCESSENV_) && !defined(__WIL__PROCESSENV_) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_APP | WINAPI_PARTITION_SYSTEM)
#define __WIL__PROCESSENV_
    /** Manages lifecycle of an environment-strings block
    ~~~
    wil::unique_environstrings_ptr env { ::GetEnvironmentStringsW() };
    const wchar_t *nextVar = env.get();
    while (nextVar && *nextVar)
    {
        // consume 'nextVar'
        nextVar += wcslen(nextVar) + 1;
    }
    ~~~
    */
    using unique_environstrings_ptr = wistd::unique_ptr<wchar_t, function_deleter<decltype(&::FreeEnvironmentStringsW), FreeEnvironmentStringsW>>;

#ifndef WIL_NO_ANSI_STRINGS
    //! ANSI equivalent to unique_environstrings_ptr;
    using unique_environansistrings_ptr = wistd::unique_ptr<char, function_deleter<decltype(&::FreeEnvironmentStringsA), FreeEnvironmentStringsA>>;
#endif
#endif

#if defined(_APPMODEL_H_) && !defined(__WIL_APPMODEL_H_) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP)
#define __WIL_APPMODEL_H_
    typedef unique_any<PACKAGE_INFO_REFERENCE, decltype(&::ClosePackageInfo), ::ClosePackageInfo> unique_package_info_reference;
#endif // __WIL_APPMODEL_H_
#if defined(__WIL_APPMODEL_H_) && !defined(__WIL_APPMODEL_H_STL) && defined(WIL_RESOURCE_STL)
#define __WIL_APPMODEL_H_STL
    typedef shared_any<unique_package_info_reference> shared_package_info_reference;
    typedef weak_any<shared_package_info_reference> weak_package_info_reference;
#endif // __WIL_APPMODEL_H_STL

#if defined(WDFAPI) && !defined(__WIL_WDFAPI)
#define __WIL_WDFAPI

    namespace details
    {
        template<typename TWDFOBJECT>
        using wdf_object_resource_policy = resource_policy<TWDFOBJECT, decltype(&::WdfObjectDelete), &::WdfObjectDelete>;
    }

    template<typename TWDFOBJECT>
    using unique_wdf_any = unique_any_t<details::unique_storage<details::wdf_object_resource_policy<TWDFOBJECT>>>;

    using unique_wdf_object          = unique_wdf_any<WDFOBJECT>;

    using unique_wdf_timer           = unique_wdf_any<WDFTIMER>;
    using unique_wdf_work_item       = unique_wdf_any<WDFWORKITEM>;

    using unique_wdf_memory          = unique_wdf_any<WDFMEMORY>;

    using unique_wdf_dma_enabler     = unique_wdf_any<WDFDMAENABLER>;
    using unique_wdf_dma_transaction = unique_wdf_any<WDFDMATRANSACTION>;
    using unique_wdf_common_buffer   = unique_wdf_any<WDFCOMMONBUFFER>;

    using unique_wdf_key             = unique_wdf_any<WDFKEY>;
    using unique_wdf_string          = unique_wdf_any<WDFSTRING>;
    using unique_wdf_collection      = unique_wdf_any<WDFCOLLECTION>;

    using wdf_wait_lock_release_scope_exit =
        unique_any<
            WDFWAITLOCK,
            decltype(&::WdfWaitLockRelease),
            ::WdfWaitLockRelease,
            details::pointer_access_none>;

    inline
    WI_NODISCARD
    _IRQL_requires_max_(PASSIVE_LEVEL)
    _Acquires_lock_(lock)
    wdf_wait_lock_release_scope_exit
    acquire_wdf_wait_lock(WDFWAITLOCK lock) WI_NOEXCEPT
    {
        ::WdfWaitLockAcquire(lock, nullptr);
        return wdf_wait_lock_release_scope_exit(lock);
    }

    inline
    WI_NODISCARD
    _IRQL_requires_max_(APC_LEVEL)
    _When_(return, _Acquires_lock_(lock))
    wdf_wait_lock_release_scope_exit
    try_acquire_wdf_wait_lock(WDFWAITLOCK lock) WI_NOEXCEPT
    {
        LONGLONG timeout = 0;
        NTSTATUS status = ::WdfWaitLockAcquire(lock, &timeout);
        if (status == STATUS_SUCCESS)
        {
            return wdf_wait_lock_release_scope_exit(lock);
        }
        else
        {
            return wdf_wait_lock_release_scope_exit();
        }
    }

    using wdf_spin_lock_release_scope_exit =
        unique_any<
            WDFSPINLOCK,
            decltype(&::WdfSpinLockRelease),
            ::WdfSpinLockRelease,
            details::pointer_access_none>;

    inline
    WI_NODISCARD
    _IRQL_requires_max_(DISPATCH_LEVEL)
    _IRQL_raises_(DISPATCH_LEVEL)
    _Acquires_lock_(lock)
    wdf_spin_lock_release_scope_exit
    acquire_wdf_spin_lock(WDFSPINLOCK lock) WI_NOEXCEPT
    {
        ::WdfSpinLockAcquire(lock);
        return wdf_spin_lock_release_scope_exit(lock);
    }

    namespace details
    {
        template<typename TWDFLOCK>
        using unique_wdf_lock_storage = unique_storage<wdf_object_resource_policy<TWDFLOCK>>;

        class unique_wdf_spin_lock_storage : public unique_wdf_lock_storage<WDFSPINLOCK>
        {
            using wdf_lock_storage_t = unique_wdf_lock_storage<WDFSPINLOCK>;

        public:
            using pointer = wdf_lock_storage_t::pointer;

            // Forward all base class constructors, but have it be explicit.
            template <typename... args_t>
            explicit unique_wdf_spin_lock_storage(args_t&& ... args) WI_NOEXCEPT : wdf_lock_storage_t(wistd::forward<args_t>(args)...) {}

            NTSTATUS create(_In_opt_ WDF_OBJECT_ATTRIBUTES* attributes = WDF_NO_OBJECT_ATTRIBUTES)
            {
                return ::WdfSpinLockCreate(attributes, out_param(*this));
            }

            WI_NODISCARD
            _IRQL_requires_max_(DISPATCH_LEVEL)
            _IRQL_raises_(DISPATCH_LEVEL)
            wdf_spin_lock_release_scope_exit acquire() WI_NOEXCEPT
            {
                return wil::acquire_wdf_spin_lock(wdf_lock_storage_t::get());
            }
        };

        class unique_wdf_wait_lock_storage : public unique_wdf_lock_storage<WDFWAITLOCK>
        {
            using wdf_lock_storage_t = unique_wdf_lock_storage<WDFWAITLOCK>;

        public:
            using pointer = wdf_lock_storage_t::pointer;

            // Forward all base class constructors, but have it be explicit.
            template <typename... args_t>
            explicit unique_wdf_wait_lock_storage(args_t&& ... args) WI_NOEXCEPT : wdf_lock_storage_t(wistd::forward<args_t>(args)...) {}

            NTSTATUS create(_In_opt_ WDF_OBJECT_ATTRIBUTES* attributes = WDF_NO_OBJECT_ATTRIBUTES)
            {
                return ::WdfWaitLockCreate(attributes, out_param(*this));
            }

            WI_NODISCARD
            _IRQL_requires_max_(PASSIVE_LEVEL)
            wdf_wait_lock_release_scope_exit acquire() WI_NOEXCEPT
            {
                return wil::acquire_wdf_wait_lock(wdf_lock_storage_t::get());
            }

            WI_NODISCARD
            _IRQL_requires_max_(APC_LEVEL)
            wdf_wait_lock_release_scope_exit try_acquire() WI_NOEXCEPT
            {
                return wil::try_acquire_wdf_wait_lock(wdf_lock_storage_t::get());
            }
        };
    }

    using unique_wdf_wait_lock = unique_any_t<details::unique_wdf_wait_lock_storage>;
    using unique_wdf_spin_lock = unique_any_t<details::unique_wdf_spin_lock_storage>;

    //! unique_wdf_object_reference is a RAII type for managing WDF object references acquired using
    //! the WdfObjectReference* family of APIs. The behavior of this class is exactly identical to
    //! wil::unique_any but a few methods have some WDF-object-reference-specific enhancements.
    //!
    //! * The constructor takes not only a WDFOBJECT-compatible type or a wil::unique_wdf_any, but
    //!   optionally also a tag with which the reference was acquired.
    //! * A get_tag() method is provided to retrieve the tag.
    //! * reset() is similar to the constructor in that it also optionally takes a tag.
    //! * release() optionally takes an out-param that returns the tag.
    //!
    //! These subtle differences make it impossible to reuse the wil::unique_any_t template for its implementation.
    template<typename wdf_object_t>
    class unique_wdf_object_reference
    {
    public:
        unique_wdf_object_reference() WI_NOEXCEPT = default;

        //! Wrap a WDF object reference that has already been acquired into this RAII type. If you
        //! want to acquire a new reference instead, use WI_WdfObjectReferenceIncrement.
        explicit unique_wdf_object_reference(wdf_object_t wdfObject, void* tag = nullptr) WI_NOEXCEPT
            : m_wdfObject(wdfObject), m_tag(tag)
        {
        }

        //! This is similar to the constructor that takes a raw WDF handle but is enlightened to
        //! take a const-ref to a wil::unique_wdf_any<> instead, obviating the need to call .get()
        //! on it. As with the other constructor, the expectation is that the raw reference has
        //! already been acquired and ownership is being transferred into this RAII object.
        explicit unique_wdf_object_reference(const wil::unique_wdf_any<wdf_object_t>& wdfObject, void* tag = nullptr) WI_NOEXCEPT
            : unique_wdf_object_reference(wdfObject.get(), tag)
        {
        }

        unique_wdf_object_reference(const unique_wdf_object_reference&) = delete;
        unique_wdf_object_reference& operator=(const unique_wdf_object_reference&) = delete;

        unique_wdf_object_reference(unique_wdf_object_reference&& other)
            : m_wdfObject(other.m_wdfObject), m_tag(other.m_tag)
        {
            other.m_wdfObject = WDF_NO_HANDLE;
            other.m_tag = nullptr;
        }

        unique_wdf_object_reference& operator=(unique_wdf_object_reference&& other)
        {
            if (this != wistd::addressof(other))
            {
                reset(other.m_wdfObject, other.m_tag);
                other.m_wdfObject = WDF_NO_HANDLE;
                other.m_tag = nullptr;
            }

            return *this;
        }

        ~unique_wdf_object_reference() WI_NOEXCEPT
        {
            reset();
        }

        WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
        {
            return m_wdfObject != WDF_NO_HANDLE;
        }

        WI_NODISCARD wdf_object_t get() const WI_NOEXCEPT
        {
            return m_wdfObject;
        }

        WI_NODISCARD void* get_tag() const WI_NOEXCEPT
        {
            return m_tag;
        }

        //! Replaces the current instance (releasing it if it exists) with a new WDF object
        //! reference that has already been acquired by the caller.
        void reset(wdf_object_t wdfObject = WDF_NO_HANDLE, void* tag = nullptr) WI_NOEXCEPT
        {
            if (m_wdfObject != WDF_NO_HANDLE)
            {
                // We don't use WdfObjectDereferenceActual because there is no way to provide the
                // correct __LINE__ and __FILE__, but if you use RAII all the way, you shouldn't have to
                // worry about where it was released, only where it was acquired.
                WdfObjectDereferenceWithTag(m_wdfObject, m_tag);
            }

            m_wdfObject = wdfObject;
            m_tag = tag;
        }

        void reset(const wil::unique_wdf_any<wdf_object_t>& wdfObject, void* tag = nullptr) WI_NOEXCEPT
        {
            reset(wdfObject.get(), tag);
        }

        wdf_object_t release(_Outptr_opt_ void** tag = nullptr) WI_NOEXCEPT
        {
            const auto wdfObject = m_wdfObject;
            wil::assign_to_opt_param(tag, m_tag);
            m_wdfObject = WDF_NO_HANDLE;
            m_tag = nullptr;
            return wdfObject;
        }

        void swap(unique_wdf_object_reference& other) WI_NOEXCEPT
        {
            wistd::swap_wil(m_wdfObject, other.m_wdfObject);
            wistd::swap_wil(m_tag, other.m_tag);
        }

        //! Drops the current reference if any, and returns a pointer to a WDF handle which can
        //! receive a newly referenced WDF handle. The tag is assumed to be nullptr. If a different
        //! tag needs to be used, a temporary variable will need to be used to receive the WDF
        //! handle and a unique_wdf_object_reference will need to be constructed with it.
        //!
        //! The quintessential use-case for this method is WdfIoQueueFindRequest.
        wdf_object_t* put() WI_NOEXCEPT
        {
            reset();
            return &m_wdfObject;
        }

        wdf_object_t* operator&() WI_NOEXCEPT
        {
            return put();
        }

    private:
        wdf_object_t m_wdfObject = WDF_NO_HANDLE;
        void* m_tag = nullptr;
    };

    // Increment the ref-count on a WDF object and return a unique_wdf_object_reference for it. Use
    // WI_WdfObjectReferenceIncrement to automatically use the call-site source location. Use this
    // function only if the call-site source location is obtained from elsewhere (i.e., plumbed
    // through other abstractions).
    template<typename wdf_object_t>
    inline WI_NODISCARD unique_wdf_object_reference<wdf_object_t> wdf_object_reference_increment(
        wdf_object_t wdfObject, PVOID tag, LONG lineNumber, PCSTR fileName) WI_NOEXCEPT
    {
        // Parameter is incorrectly marked as non-const, so the const-cast is required.
        ::WdfObjectReferenceActual(wdfObject, tag, lineNumber, const_cast<char*>(fileName));
        return unique_wdf_object_reference<wdf_object_t>{ wdfObject, tag };
    }

    template<typename wdf_object_t>
    inline WI_NODISCARD unique_wdf_object_reference<wdf_object_t> wdf_object_reference_increment(
        const wil::unique_wdf_any<wdf_object_t>& wdfObject, PVOID tag, LONG lineNumber, PCSTR fileName) WI_NOEXCEPT
    {
        return wdf_object_reference_increment(wdfObject.get(), tag, lineNumber, fileName);
    }

// A macro so that we can capture __LINE__ and __FILE__.
#define WI_WdfObjectReferenceIncrement(wdfObject, tag) \
    wil::wdf_object_reference_increment(wdfObject, tag, __LINE__, __FILE__)

    //! wdf_request_completer is a unique_any-like RAII class for managing completion of a
    //! WDFREQUEST. On destruction or explicit reset() it completes the WDFREQUEST with parameters
    //! (status, information, priority boost) previously set using methods on this class.
    //!
    //! This class does not use the unique_any_t template primarily because the release() and put()
    //! methods need to return a WDFREQUEST/WDFREQUEST*, as opposed to the internal storage type.
    class wdf_request_completer
    {
    public:

        explicit wdf_request_completer(WDFREQUEST wdfRequest = WDF_NO_HANDLE) WI_NOEXCEPT
            : m_wdfRequest(wdfRequest)
        {
        }

        wdf_request_completer(const wdf_request_completer&) = delete;
        wdf_request_completer& operator=(const wdf_request_completer&) = delete;

        wdf_request_completer(wdf_request_completer&& other) WI_NOEXCEPT
            : m_wdfRequest(other.m_wdfRequest), m_status(other.m_status), m_information(other.m_information),
#if defined(WIL_KERNEL_MODE)
              m_priorityBoost(other.m_priorityBoost),
#endif
              m_completionFlags(other.m_completionFlags)
        {
            clear_state(other);
        }

        wdf_request_completer& operator=(wdf_request_completer&& other) WI_NOEXCEPT
        {
            if (this != wistd::addressof(other))
            {
                reset();
                m_wdfRequest = other.m_wdfRequest;
                m_status = other.m_status;
                m_information = other.m_information;
#if defined(WIL_KERNEL_MODE)
                m_priorityBoost = other.m_priorityBoost;
#endif
                m_completionFlags = other.m_completionFlags;
                clear_state(other);
            }

            return *this;
        }

        ~wdf_request_completer() WI_NOEXCEPT
        {
            reset();
        }

        WI_NODISCARD WDFREQUEST get() const WI_NOEXCEPT
        {
            return m_wdfRequest;
        }

        //! Set the NTSTATUS value with with the WDFREQUEST will be completed when the RAII object
        //! goes out of scope or .reset() is called explicitly. Calling this method does *not*
        //! complete the request right away. No effect if this object currently does not have
        //! ownership of a WDFREQUEST. The expected usage pattern is that set_status() is called
        //! only after ownership of a WDFREQUEST is transferred to this object.
        void set_status(NTSTATUS status) WI_NOEXCEPT
        {
            // The contract is that this method has no effect if we currently do not have a
            // m_wdfRequest. But that is enforced by discarding all state when a WDFREQUEST is
            // attached, not by explicitly checking for that condition here.

            m_status = status;
        }

        //! Set the IO_STATUS_BLOCK.Information value with which the WDFREQUEST will be completed.
        //! Note that the Information value is not stored directly in the WDFREQUEST using
        //! WdfRequestSetInformation. It is only used at the time of completion. No effect if this
        //! object currently does not have ownership of a WDFREQUEST. The expected usage pattern is
        //! that set_information() is called only after ownership of a WDFREQUEST is transferred to
        //! this object.
        void set_information(ULONG_PTR information) WI_NOEXCEPT
        {
            // The contract is that this method has no effect if we currently do not have a
            // m_wdfRequest. But that is enforced by discarding all state when a WDFREQUEST is
            // attached, not by explicitly checking for that condition here.

            m_completionFlags.informationSet = 1;
            m_information = information;
        }

#if defined(WIL_KERNEL_MODE)
        //! Set the priority boost with which the WDFREQUEST will be completed. If this method is
        //! called, the WDFREQUEST will eventually be completed with
        //! WdfRequestCompleteWithPriorityBoost. No effect if this object currently does not have
        //! ownership of a WDFREQUEST. The expected usage pattern is that set_priority_boost() is
        //! called only after ownership of a WDFREQUEST is transferred to this object.
        void set_priority_boost(CCHAR priorityBoost) WI_NOEXCEPT
        {
            // The contract is that this method has no effect if we currently do not have a
            // m_wdfRequest. But that is enforced by discarding all state when a WDFREQUEST is
            // attached, not by explicitly checking for that condition here.

            m_completionFlags.priorityBoostSet = 1;
            m_priorityBoost = priorityBoost;
        }
#endif

        WI_NODISCARD explicit operator bool() const WI_NOEXCEPT
        {
            return m_wdfRequest != WDF_NO_HANDLE;
        }

        WDFREQUEST* put() WI_NOEXCEPT
        {
            reset();
            return &m_wdfRequest;
        }

        WDFREQUEST* operator&() WI_NOEXCEPT
        {
            return put();
        }

        //! Relinquishes completion responsibility for the WDFREQUEST. Note that any state
        //! (information, priority boost, status) set on this object is lost. This design choice was
        //! made because it is atypical to set an information or priority boost value upfront; they
        //! are typically set at the point where the request is going to be completed. Hence a
        //! use-case wherein release() is called will typically not have set an information or
        //! priority boost.
        WDFREQUEST release() WI_NOEXCEPT
        {
            const auto wdfRequest = m_wdfRequest;
            clear_state(*this);
            return wdfRequest;
        }

        void swap(wdf_request_completer& other) WI_NOEXCEPT
        {
            wistd::swap_wil(m_wdfRequest, other.m_wdfRequest);
            wistd::swap_wil(m_information, other.m_information);
            wistd::swap_wil(m_status, other.m_status);
#if defined(WIL_KERNEL_MODE)
            wistd::swap_wil(m_priorityBoost, other.m_priorityBoost);
#endif
            wistd::swap_wil(m_completionFlags, other.m_completionFlags);
        }

        void reset(WDFREQUEST newWdfRequest = WDF_NO_HANDLE)
        {
            if (m_wdfRequest != WDF_NO_HANDLE)
            {
                // We try to match the usage patterns that the driver would have typically used in the
                // various scenarios. For instance, if the driver has set the information field, we'll
                // call WdfRequestCompleteWithInformation instead of calling WdfRequestSetInformation
                // followed by WdfRequestComplete.

#if defined(WIL_KERNEL_MODE)
                if (m_completionFlags.priorityBoostSet)
                {
                    if (m_completionFlags.informationSet)
                    {
                        WdfRequestSetInformation(m_wdfRequest, m_information);
                    }

                    WdfRequestCompleteWithPriorityBoost(m_wdfRequest, m_status, m_priorityBoost);
                }
                else
#endif
                if (m_completionFlags.informationSet)
                {
                    WdfRequestCompleteWithInformation(m_wdfRequest, m_status, m_information);
                }
                else
                {
                    WdfRequestComplete(m_wdfRequest, m_status);
                }
            }

            // We call clear_state unconditionally just in case some parameters (status,
            // information, etc.) were set prior to attaching a WDFREQUEST to this object. Those
            // parameters are not considered relevant to the WDFREQUEST being attached to this
            // object now.
            clear_state(*this, newWdfRequest);
        }

    private:

        static void clear_state(wdf_request_completer& completer, WDFREQUEST newWdfRequest = WDF_NO_HANDLE) WI_NOEXCEPT
        {
            completer.m_wdfRequest = newWdfRequest;
            completer.m_status = STATUS_UNSUCCESSFUL;
            completer.m_information = 0;
#if defined(WIL_KERNEL_MODE)
            completer.m_priorityBoost = 0;
#endif
            completer.m_completionFlags = {};
        }

        // Members are ordered in decreasing size to minimize padding.

        WDFREQUEST m_wdfRequest = WDF_NO_HANDLE;

        // This will not be used unless m_completionFlags.informationSet is set.
        ULONG_PTR m_information = 0;

        // There is no reasonably default NTSTATUS value. Callers are expected to explicitly set a
        // status value at the point where it is decided that the request needs to be completed.
        NTSTATUS m_status = STATUS_UNSUCCESSFUL;

// UMDF does not support WdfRequestCompleteWithPriorityBoost.
#if defined(WIL_KERNEL_MODE)
        // This will not be used unless m_completionFlags.priorityBoostSet is set.
        CCHAR m_priorityBoost = 0;
#endif

        struct
        {
            UINT8 informationSet : 1;
#if defined(WIL_KERNEL_MODE)
            UINT8 priorityBoostSet : 1;
#endif
        } m_completionFlags = {};
    };
#endif

#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_SYSTEM) && \
    defined(_CFGMGR32_H_) && \
    (WINVER >= _WIN32_WINNT_WIN8) && \
    !defined(__WIL_CFGMGR32_H_)
#define __WIL_CFGMGR32_H_
    typedef unique_any<HCMNOTIFICATION, decltype(&::CM_Unregister_Notification), ::CM_Unregister_Notification> unique_hcmnotification;
#endif

#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_SYSTEM) && \
    defined(_SWDEVICE_H_) && \
    (WINVER >= _WIN32_WINNT_WIN8) && \
    !defined(__WIL_SWDEVICE_H_)
#define __WIL_SWDEVICE_H_
        typedef unique_any<HSWDEVICE, decltype(&::SwDeviceClose), ::SwDeviceClose> unique_hswdevice;
#endif

#if defined(WIL_KERNEL_MODE) && (defined(_WDMDDK_) || defined(_NTDDK_)) && !defined(__WIL_RESOURCE_WDM)
#define __WIL_RESOURCE_WDM

    namespace details
    {
        struct kspin_lock_saved_irql
        {
            PKSPIN_LOCK spinLock = nullptr;
            KIRQL savedIrql = PASSIVE_LEVEL;

            kspin_lock_saved_irql() = default;

            kspin_lock_saved_irql(PKSPIN_LOCK /* spinLock */)
            {
                // This constructor exists simply to allow conversion of the pointer type to
                // pointer_storage type when constructing an invalid instance. The spinLock pointer
                // is expected to be nullptr.
            }

            // Exists to satisfy the interconvertibility requirement for pointer_storage and
            // pointer.
            WI_NODISCARD explicit operator PKSPIN_LOCK() const
            {
                return spinLock;
            }

            _IRQL_requires_(DISPATCH_LEVEL)
            static
            void Release(_In_ _IRQL_restores_ const kspin_lock_saved_irql& spinLockSavedIrql)
            {
                KeReleaseSpinLock(spinLockSavedIrql.spinLock, spinLockSavedIrql.savedIrql);
            }
        };

        // On some architectures KeReleaseSpinLockFromDpcLevel is a macro, and we need a thunk
        // function we can take the address of.
        inline
        _IRQL_requires_min_(DISPATCH_LEVEL)
        void __stdcall ReleaseSpinLockFromDpcLevel(_Inout_ PKSPIN_LOCK spinLock) WI_NOEXCEPT
        {
            KeReleaseSpinLockFromDpcLevel(spinLock);
        }
    }

    using kspin_lock_guard = unique_any<PKSPIN_LOCK, decltype(details::kspin_lock_saved_irql::Release), &details::kspin_lock_saved_irql::Release,
        details::pointer_access_none, details::kspin_lock_saved_irql>;

    using kspin_lock_at_dpc_guard = unique_any<PKSPIN_LOCK, decltype(details::ReleaseSpinLockFromDpcLevel), &details::ReleaseSpinLockFromDpcLevel,
        details::pointer_access_none>;

    WI_NODISCARD
    inline
    _IRQL_requires_max_(DISPATCH_LEVEL)
    _IRQL_saves_
    _IRQL_raises_(DISPATCH_LEVEL)
    kspin_lock_guard
    acquire_kspin_lock(_In_ PKSPIN_LOCK spinLock)
    {
        details::kspin_lock_saved_irql spinLockSavedIrql;
        KeAcquireSpinLock(spinLock, &spinLockSavedIrql.savedIrql);
        spinLockSavedIrql.spinLock = spinLock;
        return kspin_lock_guard(spinLockSavedIrql);
    }

    WI_NODISCARD
    inline
    _IRQL_requires_min_(DISPATCH_LEVEL)
    kspin_lock_at_dpc_guard
    acquire_kspin_lock_at_dpc(_In_ PKSPIN_LOCK spinLock)
    {
        KeAcquireSpinLockAtDpcLevel(spinLock);
        return kspin_lock_at_dpc_guard(spinLock);
    }

    class kernel_spin_lock
    {
    public:
        kernel_spin_lock() WI_NOEXCEPT
        {
            ::KeInitializeSpinLock(&m_kSpinLock);
        }

        ~kernel_spin_lock() = default;

        // Cannot change memory location.
        kernel_spin_lock(const kernel_spin_lock&) = delete;
        kernel_spin_lock& operator=(const kernel_spin_lock&) = delete;
        kernel_spin_lock(kernel_spin_lock&&) = delete;
        kernel_spin_lock& operator=(kernel_spin_lock&&) = delete;

        WI_NODISCARD
        _IRQL_requires_max_(DISPATCH_LEVEL)
        _IRQL_saves_
        _IRQL_raises_(DISPATCH_LEVEL)
        kspin_lock_guard acquire() WI_NOEXCEPT
        {
            return acquire_kspin_lock(&m_kSpinLock);
        }

        WI_NODISCARD
        _IRQL_requires_min_(DISPATCH_LEVEL)
        kspin_lock_at_dpc_guard acquire_at_dpc() WI_NOEXCEPT
        {
            return acquire_kspin_lock_at_dpc(&m_kSpinLock);
        }

    private:
        KSPIN_LOCK m_kSpinLock;
    };

    namespace details
    {
        template <EVENT_TYPE eventType>
        class kernel_event_t
        {
        public:
            explicit kernel_event_t(bool isSignaled = false) WI_NOEXCEPT
            {
                ::KeInitializeEvent(&m_kernelEvent, static_cast<EVENT_TYPE>(eventType), isSignaled ? TRUE : FALSE);
            }

            // Cannot change memory location.
            kernel_event_t(const kernel_event_t&) = delete;
            kernel_event_t(kernel_event_t&&) = delete;
            kernel_event_t& operator=(const kernel_event_t&) = delete;
            kernel_event_t& operator=(kernel_event_t&&) = delete;

            // Get the underlying KEVENT structure for more advanced usages like
            // KeWaitForMultipleObjects or KeWaitForSingleObject with non-default parameters.
            PRKEVENT get() WI_NOEXCEPT
            {
                return &m_kernelEvent;
            }

            void clear() WI_NOEXCEPT
            {
                // The most common use-case is to clear the event with no interest in its previous
                // value. Hence, that is the functionality we provide by default. If the previous
                // value is required, one may .get() the underlying event object and call
                // ::KeResetEvent().
                ::KeClearEvent(&m_kernelEvent);
            }

            // Returns the previous state of the event.
            bool set(KPRIORITY increment = IO_NO_INCREMENT) WI_NOEXCEPT
            {
                return ::KeSetEvent(&m_kernelEvent, increment, FALSE) ? true : false;
            }

            // Checks if the event is currently signaled. Does not change the state of the event.
            WI_NODISCARD bool is_signaled() const WI_NOEXCEPT
            {
                return ::KeReadStateEvent(const_cast<PRKEVENT>(&m_kernelEvent)) ? true : false;
            }

            // Return true if the wait was satisfied. Time is specified in 100ns units, relative
            // (negative) or absolute (positive). For more details, see the documentation of
            // KeWaitForSingleObject.
            bool wait(LONGLONG waitTime) WI_NOEXCEPT
            {
                LARGE_INTEGER duration;
                duration.QuadPart = waitTime;
                return wait_for_single_object(&duration);
            }

            // Waits indefinitely for the event to be signaled.
            void wait() WI_NOEXCEPT
            {
                wait_for_single_object(nullptr);
            }

        private:
            bool wait_for_single_object(_In_opt_ LARGE_INTEGER* waitDuration) WI_NOEXCEPT
            {
                auto status = ::KeWaitForSingleObject(&m_kernelEvent, Executive, KernelMode, FALSE, waitDuration);

                // We specified Executive and non-alertable, which means some of the return values are
                // not possible.
                WI_ASSERT((status == STATUS_SUCCESS) || (status == STATUS_TIMEOUT));
                return (status == STATUS_SUCCESS);
            }

            KEVENT m_kernelEvent;
        };
    }

    using kernel_event_auto_reset = details::kernel_event_t<SynchronizationEvent>;
    using kernel_event_manual_reset = details::kernel_event_t<NotificationEvent>;
    using kernel_event = kernel_event_auto_reset; // For parity with the default for other WIL event types.

    /**
    RAII class and lock-guards for a kernel FAST_MUTEX.
    */

    using fast_mutex_guard = unique_any<FAST_MUTEX*, decltype(::ExReleaseFastMutex), &::ExReleaseFastMutex, details::pointer_access_none>;

    WI_NODISCARD
    inline
    _IRQL_requires_max_(APC_LEVEL)
    fast_mutex_guard acquire_fast_mutex(FAST_MUTEX* fastMutex) WI_NOEXCEPT
    {
        ::ExAcquireFastMutex(fastMutex);
        return fast_mutex_guard(fastMutex);
    }

    WI_NODISCARD
    inline
    _IRQL_requires_max_(APC_LEVEL)
    fast_mutex_guard try_acquire_fast_mutex(FAST_MUTEX* fastMutex) WI_NOEXCEPT
    {
        if (::ExTryToAcquireFastMutex(fastMutex))
        {
            return fast_mutex_guard(fastMutex);
        }
        else
        {
            return fast_mutex_guard();
        }
    }

    class fast_mutex
    {
    public:
        fast_mutex() WI_NOEXCEPT
        {
            ::ExInitializeFastMutex(&m_fastMutex);
        }

        ~fast_mutex() WI_NOEXCEPT = default;

        // Cannot change memory location.
        fast_mutex(const fast_mutex&) = delete;
        fast_mutex& operator=(const fast_mutex&) = delete;
        fast_mutex(fast_mutex&&) = delete;
        fast_mutex& operator=(fast_mutex&&) = delete;

        // Calls ExAcquireFastMutex. Returned wil::unique_any object calls ExReleaseFastMutex on
        // destruction.
        WI_NODISCARD
        _IRQL_requires_max_(APC_LEVEL)
        fast_mutex_guard acquire() WI_NOEXCEPT
        {
            return acquire_fast_mutex(&m_fastMutex);
        }

        // Calls ExTryToAcquireFastMutex. Returned wil::unique_any may be empty. If non-empty, it
        // calls ExReleaseFastMutex on destruction.
        WI_NODISCARD
        _IRQL_requires_max_(APC_LEVEL)
        fast_mutex_guard try_acquire() WI_NOEXCEPT
        {
            return try_acquire_fast_mutex(&m_fastMutex);
        }

    private:
        FAST_MUTEX m_fastMutex;
    };

    namespace details
    {
        _IRQL_requires_max_(APC_LEVEL)
        inline void release_fast_mutex_with_critical_region(FAST_MUTEX* fastMutex) WI_NOEXCEPT
        {
            ::ExReleaseFastMutexUnsafe(fastMutex);
            ::KeLeaveCriticalRegion();
        }
    }

    using fast_mutex_with_critical_region_guard =
        unique_any<FAST_MUTEX*, decltype(details::release_fast_mutex_with_critical_region), &details::release_fast_mutex_with_critical_region, details::pointer_access_none>;

    WI_NODISCARD
    inline
    _IRQL_requires_max_(APC_LEVEL)
    fast_mutex_with_critical_region_guard acquire_fast_mutex_with_critical_region(FAST_MUTEX* fastMutex) WI_NOEXCEPT
    {
        ::KeEnterCriticalRegion();
        ::ExAcquireFastMutexUnsafe(fastMutex);
        return fast_mutex_with_critical_region_guard(fastMutex);
    }

    // A FAST_MUTEX lock class that calls KeEnterCriticalRegion and then ExAcquireFastMutexUnsafe.
    // Returned wil::unique_any lock-guard calls ExReleaseFastMutexUnsafe and KeLeaveCriticalRegion
    // on destruction. This is useful if calling code wants to stay at PASSIVE_LEVEL.
    class fast_mutex_with_critical_region
    {
    public:
        fast_mutex_with_critical_region() WI_NOEXCEPT
        {
            ::ExInitializeFastMutex(&m_fastMutex);
        }

        ~fast_mutex_with_critical_region() WI_NOEXCEPT = default;

        // Cannot change memory location.
        fast_mutex_with_critical_region(const fast_mutex_with_critical_region&) = delete;
        fast_mutex_with_critical_region& operator=(const fast_mutex_with_critical_region&) = delete;
        fast_mutex_with_critical_region(fast_mutex_with_critical_region&&) = delete;
        fast_mutex_with_critical_region& operator=(fast_mutex_with_critical_region&&) = delete;

        WI_NODISCARD
        _IRQL_requires_max_(APC_LEVEL)
        fast_mutex_with_critical_region_guard acquire() WI_NOEXCEPT
        {
            return acquire_fast_mutex_with_critical_region(&m_fastMutex);
        }

    private:
        FAST_MUTEX m_fastMutex;
    };

    //! A type that calls KeLeaveCriticalRegion on destruction (or reset()).
    using unique_leave_critical_region_call = unique_call<decltype(&::KeLeaveCriticalRegion), ::KeLeaveCriticalRegion>;

    //! Disables user APCs and normal kernel APCs; returns an RAII object that reverts
    WI_NODISCARD inline unique_leave_critical_region_call enter_critical_region()
    {
        KeEnterCriticalRegion();
        return{};
    }

    //! A type that calls KeLeaveGuardedRegion on destruction (or reset()).
    using unique_leave_guarded_region_call = unique_call<decltype(&::KeLeaveGuardedRegion), ::KeLeaveGuardedRegion>;

    //! Disables all APCs; returns an RAII object that reverts
    WI_NODISCARD inline unique_leave_guarded_region_call enter_guarded_region()
    {
        KeEnterGuardedRegion();
        return{};
    }

//! WDM version of EX_PUSH_LOCK is available starting with Windows 10 1809
#if (NTDDI_VERSION >= NTDDI_WIN10_RS5)
    namespace details
    {
        _IRQL_requires_max_(APC_LEVEL)
        inline void release_push_lock_exclusive(EX_PUSH_LOCK* pushLock) WI_NOEXCEPT
        {
            ::ExReleasePushLockExclusive(pushLock);
            ::KeLeaveCriticalRegion();
        }

        _IRQL_requires_max_(APC_LEVEL)
        inline void release_push_lock_shared(EX_PUSH_LOCK* pushLock) WI_NOEXCEPT
        {
            ::ExReleasePushLockShared(pushLock);
            ::KeLeaveCriticalRegion();
        }
    }

    using push_lock_exclusive_guard =
        unique_any<EX_PUSH_LOCK*, decltype(&details::release_push_lock_exclusive), &details::release_push_lock_exclusive, details::pointer_access_noaddress>;

    using push_lock_shared_guard =
        unique_any<EX_PUSH_LOCK*, decltype(&details::release_push_lock_shared), &details::release_push_lock_shared, details::pointer_access_noaddress>;

    WI_NODISCARD
    inline
    _IRQL_requires_max_(APC_LEVEL)
    push_lock_exclusive_guard acquire_push_lock_exclusive(EX_PUSH_LOCK* pushLock) WI_NOEXCEPT
    {
        ::KeEnterCriticalRegion();
        ::ExAcquirePushLockExclusive(pushLock);
        return push_lock_exclusive_guard(pushLock);
    }

    WI_NODISCARD
    inline
    _IRQL_requires_max_(APC_LEVEL)
    push_lock_shared_guard acquire_push_lock_shared(EX_PUSH_LOCK* pushLock) WI_NOEXCEPT
    {
        ::KeEnterCriticalRegion();
        ::ExAcquirePushLockShared(pushLock);
        return push_lock_shared_guard(pushLock);
    }

    class push_lock
    {
    public:
        push_lock() WI_NOEXCEPT
        {
            ::ExInitializePushLock(&m_pushLock);
        }

        ~push_lock() WI_NOEXCEPT = default;

        // Cannot change memory location.
        push_lock(const push_lock&) = delete;
        push_lock& operator=(const push_lock&) = delete;
        push_lock(push_lock&&) = delete;
        push_lock& operator=(push_lock&&) = delete;

        WI_NODISCARD
        _IRQL_requires_max_(APC_LEVEL)
        push_lock_exclusive_guard acquire_exclusive() WI_NOEXCEPT
        {
            return acquire_push_lock_exclusive(&m_pushLock);
        }

        WI_NODISCARD
        _IRQL_requires_max_(APC_LEVEL)
        push_lock_shared_guard acquire_shared() WI_NOEXCEPT
        {
            return acquire_push_lock_shared(&m_pushLock);
        }

    private:
        EX_PUSH_LOCK m_pushLock;
    };
#endif

    namespace details
    {
        // Define a templated type for pool functions in order to satisfy overload resolution below
        template <typename pointer, ULONG tag>
        struct pool_helpers
        {
            static inline
            _IRQL_requires_max_(DISPATCH_LEVEL)
            void __stdcall FreePoolWithTag(pointer value) WI_NOEXCEPT
            {
                if (value)
                {
                    ExFreePoolWithTag(value, tag);
                }
            }
        };
    }

    template <typename pointer, ULONG tag = 0>
    using unique_tagged_pool_ptr = unique_any<pointer, decltype(details::pool_helpers<pointer, tag>::FreePoolWithTag), &details::pool_helpers<pointer, tag>::FreePoolWithTag>;

    // For use with IRPs that need to be IoFreeIrp'ed when done, typically allocated using IoAllocateIrp.
    using unique_allocated_irp = wil::unique_any<PIRP, decltype(&::IoFreeIrp), ::IoFreeIrp, details::pointer_access_noaddress>;
    using unique_io_workitem = wil::unique_any<PIO_WORKITEM, decltype(&::IoFreeWorkItem), ::IoFreeWorkItem, details::pointer_access_noaddress>;

#endif // __WIL_RESOURCE_WDM

#if defined(WIL_KERNEL_MODE) && (defined(_WDMDDK_) || defined(_ZWAPI_)) && !defined(__WIL_RESOURCE_ZWAPI)
#define __WIL_RESOURCE_ZWAPI

    using unique_kernel_handle = wil::unique_any<HANDLE, decltype(&::ZwClose), ::ZwClose>;

#endif // __WIL_RESOURCE_ZWAPI

#if defined(WINTRUST_H) && defined(SOFTPUB_H) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP) && !defined(__WIL_WINTRUST)
#define __WIL_WINTRUST
    namespace details
    {
        inline void __stdcall CloseWintrustData(_Inout_ WINTRUST_DATA* wtData) WI_NOEXCEPT
        {
            GUID guidV2 = WINTRUST_ACTION_GENERIC_VERIFY_V2;
            wtData->dwStateAction = WTD_STATEACTION_CLOSE;
            WinVerifyTrust(static_cast<HWND>(INVALID_HANDLE_VALUE), &guidV2, wtData);
        }
    }
    typedef wil::unique_struct<WINTRUST_DATA, decltype(&details::CloseWintrustData), details::CloseWintrustData> unique_wintrust_data;
#endif // __WIL_WINTRUST

#if defined(MSCAT_H) && WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP) && !defined(__WIL_MSCAT)
#define __WIL_MSCAT
    namespace details
    {
        inline void __stdcall CryptCATAdminReleaseContextNoFlags(_Pre_opt_valid_ _Frees_ptr_opt_ HCATADMIN handle) WI_NOEXCEPT
        {
            CryptCATAdminReleaseContext(handle, 0);
        }
    }
    typedef wil::unique_any<HCATADMIN, decltype(&details::CryptCATAdminReleaseContextNoFlags), details::CryptCATAdminReleaseContextNoFlags> unique_hcatadmin;

#if defined(WIL_RESOURCE_STL)
    typedef shared_any<unique_hcatadmin> shared_hcatadmin;
    struct hcatinfo_deleter
    {
        hcatinfo_deleter(wil::shared_hcatadmin handle) WI_NOEXCEPT : m_hCatAdmin(wistd::move(handle)) {}
        void operator()(_Pre_opt_valid_ _Frees_ptr_opt_ HCATINFO handle) const WI_NOEXCEPT
        {
            CryptCATAdminReleaseCatalogContext(m_hCatAdmin.get(), handle, 0);
        }
        wil::shared_hcatadmin m_hCatAdmin;
    };
    // This stores HCATINFO, i.e. HANDLE (void *)
    typedef wistd::unique_ptr<void, hcatinfo_deleter> unique_hcatinfo;

    namespace details
    {
        class crypt_catalog_enumerator
        {
            wil::unique_hcatinfo m_hCatInfo;
            const BYTE* m_hash;
            DWORD m_hashLen;
            bool m_initialized = false;

            struct ref
            {
                explicit ref(crypt_catalog_enumerator &r) WI_NOEXCEPT :
                    m_r(r)
                {}

                WI_NODISCARD operator HCATINFO() const WI_NOEXCEPT
                {
                    return m_r.current();
                }

                wil::unique_hcatinfo move_from_unique_hcatinfo() WI_NOEXCEPT
                {
                    wil::unique_hcatinfo info(wistd::move(m_r.m_hCatInfo));
                    return info;
                }

                WI_NODISCARD bool operator==(wistd::nullptr_t) const WI_NOEXCEPT
                {
                    return m_r.m_hCatInfo == nullptr;
                }

                WI_NODISCARD bool operator!=(wistd::nullptr_t) const WI_NOEXCEPT
                {
                    return !(*this == nullptr);
                }

            private:
                crypt_catalog_enumerator &m_r;
            };

            struct iterator
            {
#ifdef _XUTILITY_
                // muse be input_iterator_tag as use of one instance invalidates the other.
                typedef ::std::input_iterator_tag iterator_category;
#endif

                explicit iterator(crypt_catalog_enumerator *r) WI_NOEXCEPT :
                    m_r(r)
                {}

                iterator(const iterator &) = default;
                iterator(iterator &&) = default;
                iterator &operator=(const iterator &) = default;
                iterator &operator=(iterator &&) = default;

                WI_NODISCARD bool operator==(const iterator &rhs) const WI_NOEXCEPT
                {
                    if (rhs.m_r == m_r)
                    {
                        return true;
                    }

                    return (*this == nullptr) && (rhs == nullptr);
                }

                WI_NODISCARD bool operator!=(const iterator &rhs) const WI_NOEXCEPT
                {
                    return !(rhs == *this);
                }

                WI_NODISCARD bool operator==(wistd::nullptr_t) const WI_NOEXCEPT
                {
                    return nullptr == m_r || nullptr == m_r->current();
                }

                WI_NODISCARD bool operator!=(wistd::nullptr_t) const WI_NOEXCEPT
                {
                    return !(*this == nullptr);
                }

                iterator &operator++() WI_NOEXCEPT
                {
                    if (m_r != nullptr)
                    {
                        m_r->next();
                    }

                    return *this;
                }

                WI_NODISCARD ref operator*() const WI_NOEXCEPT
                {
                    return ref(*m_r);
                }

            private:
                crypt_catalog_enumerator *m_r;
            };

            shared_hcatadmin &hcatadmin() WI_NOEXCEPT
            {
                return m_hCatInfo.get_deleter().m_hCatAdmin;
            }

            bool move_next() WI_NOEXCEPT
            {
                HCATINFO prevCatInfo = m_hCatInfo.release();
                m_hCatInfo.reset(
                    ::CryptCATAdminEnumCatalogFromHash(
                        hcatadmin().get(),
                        const_cast<BYTE *>(m_hash),
                        m_hashLen,
                        0,
                        &prevCatInfo));
                return !!m_hCatInfo;
            }

            HCATINFO next() WI_NOEXCEPT
            {
                if (m_initialized && m_hCatInfo)
                {
                    move_next();
                }

                return current();
            }

            HCATINFO init() WI_NOEXCEPT
            {
                if (!m_initialized)
                {
                    m_initialized = true;
                    move_next();
                }

                return current();
            }

            HCATINFO current() WI_NOEXCEPT
            {
                return m_hCatInfo.get();
            }

        public:
            crypt_catalog_enumerator(wil::shared_hcatadmin &hCatAdmin,
                                     const BYTE *hash,
                                     DWORD hashLen) WI_NOEXCEPT :
                m_hCatInfo(nullptr, hCatAdmin),
                m_hash(hash),
                m_hashLen(hashLen)
                // , m_initialized(false) // redundant
            {}

            WI_NODISCARD iterator begin() WI_NOEXCEPT
            {
                init();
                return iterator(this);
            }

            WI_NODISCARD iterator end() const WI_NOEXCEPT
            {
                return iterator(nullptr);
            }

            crypt_catalog_enumerator(crypt_catalog_enumerator &&) = default;
            crypt_catalog_enumerator &operator=(crypt_catalog_enumerator &&) = default;

            crypt_catalog_enumerator(const crypt_catalog_enumerator &) = delete;
            crypt_catalog_enumerator &operator=(const crypt_catalog_enumerator &) = delete;
        };
    }

    /** Use to enumerate catalogs containing a hash with a range-based for.
    This avoids handling a raw resource to call CryptCATAdminEnumCatalogFromHash correctly.
    Example:
    `for (auto&& cat : wil::make_catalog_enumerator(hCatAdmin, hash, hashLen))
     { CryptCATCatalogInfoFromContext(cat, &catInfo, 0); }` */
    inline details::crypt_catalog_enumerator make_crypt_catalog_enumerator(wil::shared_hcatadmin &hCatAdmin,
        _In_count_(hashLen) const BYTE *hash, DWORD hashLen) WI_NOEXCEPT
    {
        return details::crypt_catalog_enumerator(hCatAdmin, hash, hashLen);
    }

    template <size_t Size>
    details::crypt_catalog_enumerator make_crypt_catalog_enumerator(wil::shared_hcatadmin &hCatAdmin,
        const BYTE (&hash)[Size]) WI_NOEXCEPT
    {
        static_assert(Size <= static_cast<size_t>(0xffffffffUL), "Array size truncated");
        return details::crypt_catalog_enumerator(hCatAdmin, hash, static_cast<DWORD>(Size));
    }

#endif // WI_RESOURCE_STL

#endif // __WIL_MSCAT


#if !defined(__WIL_RESOURCE_LOCK_ENFORCEMENT)
#define __WIL_RESOURCE_LOCK_ENFORCEMENT

    /**
    Functions that need an exclusive lock use can use write_lock_required as a parameter to enforce lock
    safety at compile time. Similarly, read_lock_required may stand as a parameter where shared ownership
    of a lock is required. These are empty structs that will never be used, other than passing them on to
    another function that requires them.

    These types are implicitly convertible from various lock holding types, enabling callers to provide them as
    proof of the lock that they hold.

    The following example is intentially contrived to demonstrate multiple use cases:
      - Methods that require only shared/read access
      - Methods that require only exclusive write access
      - Methods that pass their proof-of-lock to a helper
    ~~~
        class RemoteControl
        {
        public:
            void VolumeUp();
            int GetVolume();
        private:
            int GetCurrentVolume(wil::read_lock_required);
            void AdjustVolume(int delta, wil::write_lock_required);
            void SetNewVolume(int newVolume, wil::write_lock_required);

            int m_currentVolume = 0;
            wil::srwlock m_lock;
        };

        void RemoteControl::VolumeUp()
        {
            auto writeLock = m_lock.lock_exclusive();
            AdjustVolume(1, writeLock);
        }

        int RemoteControl::GetVolume()
        {
            auto readLock = m_lock.lock_shared();
            return GetCurrentVolume(readLock);
        }

        int RemoteControl::GetCurrentVolume(wil::read_lock_required)
        {
            return m_currentVolume;
        }

        void AdjustVolume(int delta, wil::write_lock_required lockProof)
        {
            const auto currentVolume = GetCurrentVolume(lockProof);
            SetNewVolume(currentVolume + delta, lockProof);
        }

        void RemoteControl::SetNewVolume(int newVolume, wil::write_lock_required)
        {
            m_currentVolume = newVolume;
        }
    ~~~

    In this design it is impossible to not meet the "lock must be held" precondition and the function parameter types
    help you understand which one.

    Cases not handled:
      - Functions that need the lock held, but fail to specify this fact by requiring a lock required parameter need
        to be found via code inspection.
      - Recursively taking a lock, when it is already held, is not avoided in this pattern
        - Readers will learn to be suspicious of acquiring a lock in functions with lock required parameters.
      - Designs with multiple locks, that must be careful to take them in the same order every time, are not helped
        by this pattern.
      - Locking the wrong object
      - Use of a std::lock type that has not actually be secured yet (such as by std::try_to_lock or std::defer_lock)
        - or use of a lock type that had been acquired but has since been released, reset, or otherwise unlocked

    These utility types are not fool-proof against all lock misuse, anti-patterns, or other complex yet valid
    scenarios. However on the net, their usage in typical cases can assist in creating clearer, self-documenting
    code that catches the common issues of forgetting to hold a lock or forgetting whether a lock is required to
    call another method safely.
    */
    struct write_lock_required;

    /**
    Stands as proof that a shared lock has been acquired. See write_lock_required for more information.
    */
    struct read_lock_required;

    namespace details
    {
        // Only those lock types specialized by lock_proof_traits will allow either a write_lock_required or
        // read_lock_required to be constructed. The allows_exclusive value indicates if the type represents an exclusive,
        // write-safe lock aquisition, or a shared, read-only lock acquisition.
        template<typename T>
        struct lock_proof_traits { };

        // Base for specializing lock_proof_traits where the lock type is shared
        struct shared_lock_proof
        {
            static constexpr bool allows_shared = true;
        };

        // Base for specializing lock_proof_traits where the lock type is exclusive (super-set of shared_lock_proof)
        struct exclusive_lock_proof : shared_lock_proof
        {
            static constexpr bool allows_exclusive = true;
        };
    }

    struct write_lock_required
    {
        /**
        Construct a new write_lock_required object for use as proof that an exclusive lock has been acquired.
        */
        template <typename TLockProof>
        write_lock_required(const TLockProof&, wistd::enable_if_t<details::lock_proof_traits<TLockProof>::allows_exclusive, int> = 0) {}

        write_lock_required() = delete; // No default construction
    };

    struct read_lock_required
    {
        /**
        Construct a new read_lock_required object for use as proof that a shared lock has been acquired.
        */
        template <typename TLockProof>
        read_lock_required(const TLockProof&, wistd::enable_if_t<details::lock_proof_traits<TLockProof>::allows_shared, int> = 0) {}

        /**
        Uses a prior write_lock_required object to construct a read_lock_required object as proof that at shared lock
        has been acquired. (Exclusive locks held are presumed to suffice for proof of a read lock)
        */
        read_lock_required(const write_lock_required&) {}

        read_lock_required() = delete; // No default construction
    };
#endif // __WIL_RESOURCE_LOCK_ENFORCEMENT

#if defined(__WIL_WINBASE_) && !defined(__WIL__RESOURCE_LOCKPROOF_WINBASE) && defined(__WIL_RESOURCE_LOCK_ENFORCEMENT)
#define __WIL__RESOURCE_LOCKPROOF_WINBASE

    namespace details
    {
        // Specializations for srwlock
        template<>
        struct lock_proof_traits<rwlock_release_shared_scope_exit> : shared_lock_proof {};

        template<>
        struct lock_proof_traits<rwlock_release_exclusive_scope_exit> : exclusive_lock_proof {};

        // Specialization for critical_section
        template<>
        struct lock_proof_traits<cs_leave_scope_exit> : exclusive_lock_proof {};
    }

#endif //__WIL__RESOURCE_LOCKPROOF_WINBASE

#if defined(_MUTEX_) && !defined(__WIL__RESOURCE_LOCKPROOF_MUTEX) && defined(__WIL_RESOURCE_LOCK_ENFORCEMENT)
#define __WIL__RESOURCE_LOCKPROOF_MUTEX

    namespace details
    {
        template<typename TMutex>
        struct lock_proof_traits<std::unique_lock<TMutex>> : exclusive_lock_proof {};

        template<typename TMutex>
        struct lock_proof_traits<std::lock_guard<TMutex>> : exclusive_lock_proof {};
    }

#endif //__WIL__RESOURCE_LOCKPROOF_MUTEX

#if defined(_SHARED_MUTEX_) && !defined(__WIL__RESOURCE_LOCKPROOF_SHAREDMUTEX) && defined(__WIL_RESOURCE_LOCK_ENFORCEMENT)
#define __WIL__RESOURCE_LOCKPROOF_SHAREDMUTEX

    namespace details
    {
        template<typename TMutex>
        struct lock_proof_traits<std::shared_lock<TMutex>> : shared_lock_proof {};
    }

#endif //__WIL__RESOURCE_LOCKPROOF_SHAREDMUTEX

} // namespace wil

#pragma warning(pop)
