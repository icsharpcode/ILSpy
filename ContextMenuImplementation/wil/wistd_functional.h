// -*- C++ -*-
//===------------------------ functional ----------------------------------===//
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

#ifndef _WISTD_FUNCTIONAL_H_
#define _WISTD_FUNCTIONAL_H_

// DO NOT add *any* additional includes to this file -- there should be no dependencies from its usage
#include "wistd_memory.h"
#include <intrin.h> // For __fastfail
#include <new.h> // For placement new

#if !defined(__WI_LIBCPP_HAS_NO_PRAGMA_SYSTEM_HEADER)
#pragma GCC system_header
#endif

#pragma warning(push)
#pragma warning(disable: 4324)
#pragma warning(disable: 4800)

/// @cond
namespace wistd     // ("Windows Implementation" std)
{
    // wistd::function
    //
    // All of the code below is in direct support of wistd::function.  This class is identical to std::function
    // with the following exceptions:
    //
    // 1) It never allocates and is safe to use from exception-free code (custom allocators are not supported)
    // 2) It's slightly bigger on the stack (64 bytes, rather than 24 for 32bit)
    // 3) There is an explicit static-assert if a lambda becomes too large to hold in the internal buffer (rather than an allocation)

    template <class _Ret>
    struct __invoke_void_return_wrapper
    {
#ifndef __WI_LIBCPP_CXX03_LANG
        template <class ..._Args>
        static _Ret __call(_Args&&... __args) {
            return __invoke(wistd::forward<_Args>(__args)...);
        }
#else
        template <class _Fn>
        static _Ret __call(_Fn __f) {
            return __invoke(__f);
        }

        template <class _Fn, class _A0>
        static _Ret __call(_Fn __f, _A0& __a0) {
            return __invoke(__f, __a0);
        }

        template <class _Fn, class _A0, class _A1>
        static _Ret __call(_Fn __f, _A0& __a0, _A1& __a1) {
            return __invoke(__f, __a0, __a1);
        }

        template <class _Fn, class _A0, class _A1, class _A2>
        static _Ret __call(_Fn __f, _A0& __a0, _A1& __a1, _A2& __a2){
            return __invoke(__f, __a0, __a1, __a2);
        }
#endif
    };

    template <>
    struct __invoke_void_return_wrapper<void>
    {
#ifndef __WI_LIBCPP_CXX03_LANG
        template <class ..._Args>
        static void __call(_Args&&... __args) {
            (void)__invoke(wistd::forward<_Args>(__args)...);
        }
#else
        template <class _Fn>
        static void __call(_Fn __f) {
            __invoke(__f);
        }

        template <class _Fn, class _A0>
        static void __call(_Fn __f, _A0& __a0) {
            __invoke(__f, __a0);
        }

        template <class _Fn, class _A0, class _A1>
        static void __call(_Fn __f, _A0& __a0, _A1& __a1) {
            __invoke(__f, __a0, __a1);
        }

        template <class _Fn, class _A0, class _A1, class _A2>
        static void __call(_Fn __f, _A0& __a0, _A1& __a1, _A2& __a2) {
            __invoke(__f, __a0, __a1, __a2);
        }
#endif
    };

    ////////////////////////////////////////////////////////////////////////////////
    //                                FUNCTION
    //==============================================================================

    // bad_function_call

    __WI_LIBCPP_NORETURN inline __WI_LIBCPP_INLINE_VISIBILITY
    void __throw_bad_function_call()
    {
        __fastfail(7); // FAST_FAIL_FATAL_APP_EXIT
    }

    template<class _Fp> class __WI_LIBCPP_TEMPLATE_VIS function; // undefined

    namespace __function
    {

        template<class _Rp>
        struct __maybe_derive_from_unary_function
        {
        };

        template<class _Rp, class _A1>
        struct __maybe_derive_from_unary_function<_Rp(_A1)>
            : public unary_function<_A1, _Rp>
        {
        };

        template<class _Rp>
        struct __maybe_derive_from_binary_function
        {
        };

        template<class _Rp, class _A1, class _A2>
        struct __maybe_derive_from_binary_function<_Rp(_A1, _A2)>
            : public binary_function<_A1, _A2, _Rp>
        {
        };

        template <class _Fp>
        __WI_LIBCPP_INLINE_VISIBILITY
        bool __not_null(_Fp const&) { return true; }

        template <class _Fp>
        __WI_LIBCPP_INLINE_VISIBILITY
        bool __not_null(_Fp* __ptr) { return __ptr; }

        template <class _Ret, class _Class>
        __WI_LIBCPP_INLINE_VISIBILITY
        bool __not_null(_Ret _Class::*__ptr) { return __ptr; }

        template <class _Fp>
        __WI_LIBCPP_INLINE_VISIBILITY
        bool __not_null(function<_Fp> const& __f) { return !!__f; }

    } // namespace __function

#ifndef __WI_LIBCPP_CXX03_LANG

    namespace __function {

        template<class _Fp> class __base;

        template<class _Rp, class ..._ArgTypes>
        class __base<_Rp(_ArgTypes...)>
        {
            __base(const __base&);
            __base& operator=(const __base&);
        public:
            __WI_LIBCPP_INLINE_VISIBILITY __base() {}
            __WI_LIBCPP_INLINE_VISIBILITY virtual ~__base() {}
            virtual void __clone(__base*) const = 0;
            virtual void __move(__base*) = 0;
            virtual void destroy() WI_NOEXCEPT = 0;
            virtual _Rp operator()(_ArgTypes&& ...) = 0;
        };

        template<class _FD, class _FB> class __func;

        template<class _Fp, class _Rp, class ..._ArgTypes>
        class __func<_Fp, _Rp(_ArgTypes...)>
            : public  __base<_Rp(_ArgTypes...)>
        {
            _Fp __f_;
        public:
            __WI_LIBCPP_INLINE_VISIBILITY
            explicit __func(_Fp&& __f)
                : __f_(wistd::move(__f)) {}

            __WI_LIBCPP_INLINE_VISIBILITY
            explicit __func(const _Fp& __f)
                : __f_(__f) {}

            virtual void __clone(__base<_Rp(_ArgTypes...)>*) const;
            virtual void __move(__base<_Rp(_ArgTypes...)>*);
            virtual void destroy() WI_NOEXCEPT;
            virtual _Rp operator()(_ArgTypes&& ... __arg);
        };

        template<class _Fp, class _Rp, class ..._ArgTypes>
        void
        __func<_Fp, _Rp(_ArgTypes...)>::__clone(__base<_Rp(_ArgTypes...)>* __p) const
        {
            ::new (__p) __func(__f_);
        }

        template<class _Fp, class _Rp, class ..._ArgTypes>
        void
        __func<_Fp, _Rp(_ArgTypes...)>::__move(__base<_Rp(_ArgTypes...)>* __p)
        {
            ::new (__p) __func(wistd::move(__f_));
        }

        template<class _Fp, class _Rp, class ..._ArgTypes>
        void
        __func<_Fp, _Rp(_ArgTypes...)>::destroy() WI_NOEXCEPT
        {
            __f_.~_Fp();
        }

        template<class _Fp, class _Rp, class ..._ArgTypes>
        _Rp
        __func<_Fp, _Rp(_ArgTypes...)>::operator()(_ArgTypes&& ... __arg)
        {
            typedef __invoke_void_return_wrapper<_Rp> _Invoker;
            return _Invoker::__call(__f_, wistd::forward<_ArgTypes>(__arg)...);
        }

        // 'wistd::function' is most similar to 'inplace_function' in that it _only_ permits holding function objects
        // that can fit within its internal buffer. Therefore, we expand this size to accommodate space for at least 12
        // pointers (__base vtable takes an additional one).
        constexpr const size_t __buffer_size = 13 * sizeof(void*);

    }  // __function

    // NOTE: The extra 'alignas' here is to work around the x86 compiler bug mentioned in
    // https://github.com/microsoft/STL/issues/1533 to force alignment on the stack
    template<class _Rp, class ..._ArgTypes>
    class __WI_LIBCPP_TEMPLATE_VIS __WI_ALIGNAS(typename aligned_storage<__function::__buffer_size>::type)
    function<_Rp(_ArgTypes...)>
        : public __function::__maybe_derive_from_unary_function<_Rp(_ArgTypes...)>,
          public __function::__maybe_derive_from_binary_function<_Rp(_ArgTypes...)>
    {
        using __base = __function::__base<_Rp(_ArgTypes...)>;
        __WI_LIBCPP_SUPPRESS_NONINIT_ANALYSIS
        typename aligned_storage<__function::__buffer_size>::type __buf_;
        __base* __f_;

        __WI_LIBCPP_NO_CFI static __base *__as_base(void *p) {
          return static_cast<__base*>(p);
        }

        template <class _Fp, bool>
        struct __callable_imp
        {
            static const bool value = is_same<void, _Rp>::value ||
                is_convertible<typename __invoke_of<_Fp&, _ArgTypes...>::type,
                                _Rp>::value;
        };

        template <class _Fp>
        struct __callable_imp<_Fp, false>
        {
            static constexpr bool value = false;
        };

        template <class _Fp>
        struct __callable
        {
            static const bool value = __callable_imp<_Fp, __lazy_and<
                integral_constant<bool, !is_same<__uncvref_t<_Fp>, function>::value>,
                __invokable<_Fp&, _ArgTypes...>
            >::value>::value;
        };

      template <class _Fp>
      using _EnableIfCallable = typename enable_if<__callable<_Fp>::value>::type;
    public:
        using result_type = _Rp;

        // construct/copy/destroy:
        __WI_LIBCPP_INLINE_VISIBILITY __WI_LIBCPP_SUPPRESS_NONINIT_ANALYSIS
        function() WI_NOEXCEPT : __f_(0) {}

        __WI_LIBCPP_INLINE_VISIBILITY
        function(nullptr_t) WI_NOEXCEPT : __f_(0) {}
        function(const function&);
        function(function&&);
        template<class _Fp, class = _EnableIfCallable<_Fp>>
        function(_Fp);

        function& operator=(const function&);
        function& operator=(function&&);
        function& operator=(nullptr_t) WI_NOEXCEPT;
        template<class _Fp, class = _EnableIfCallable<_Fp>>
        function& operator=(_Fp&&);

        ~function();

        // function modifiers:
        void swap(function&);

        // function capacity:
        __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY
            __WI_LIBCPP_EXPLICIT operator bool() const WI_NOEXCEPT {return __f_;}

        // deleted overloads close possible hole in the type system
        template<class _R2, class... _ArgTypes2>
          bool operator==(const function<_R2(_ArgTypes2...)>&) const = delete;
        template<class _R2, class... _ArgTypes2>
          bool operator!=(const function<_R2(_ArgTypes2...)>&) const = delete;
    public:
        // function invocation:
        _Rp operator()(_ArgTypes...) const;

        // NOTE: type_info is very compiler specific, and on top of that, we're operating in a namespace other than
        // 'std' so all functions requiring RTTI have been removed
    };

    template<class _Rp, class ..._ArgTypes>
    __WI_LIBCPP_SUPPRESS_NONINIT_ANALYSIS
    function<_Rp(_ArgTypes...)>::function(const function& __f)
    {
        if (__f.__f_ == nullptr)
            __f_ = 0;
        else
        {
            __f_ = __as_base(&__buf_);
            __f.__f_->__clone(__f_);
        }
    }

    template<class _Rp, class ..._ArgTypes>
    __WI_LIBCPP_SUPPRESS_NONINIT_ANALYSIS __WI_LIBCPP_SUPPRESS_NOEXCEPT_ANALYSIS
    function<_Rp(_ArgTypes...)>::function(function&& __f)
    {
        if (__f.__f_ == nullptr)
            __f_ = 0;
        else
        {
            __f_ = __as_base(&__buf_);
            __f.__f_->__move(__f_);
            __f.__f_->destroy();
            __f.__f_ = 0;
        }
    }

    template<class _Rp, class ..._ArgTypes>
    template <class _Fp, class>
    __WI_LIBCPP_SUPPRESS_NONINIT_ANALYSIS
    function<_Rp(_ArgTypes...)>::function(_Fp __f)
        : __f_(nullptr)
    {
        if (__function::__not_null(__f))
        {
            typedef __function::__func<_Fp, _Rp(_ArgTypes...)> _FF;
            static_assert(sizeof(_FF) <= sizeof(__buf_),
                "The sizeof(wistd::function) has grown too large for the reserved buffer (12 pointers).  Refactor to reduce size of the capture.");
            __f_ = ::new(static_cast<void*>(&__buf_)) _FF(wistd::move(__f));
        }
    }

    template<class _Rp, class ..._ArgTypes>
    function<_Rp(_ArgTypes...)>&
    function<_Rp(_ArgTypes...)>::operator=(const function& __f)
    {
        *this = nullptr;
        if (__f.__f_)
        {
            __f_ = __as_base(&__buf_);
            __f.__f_->__clone(__f_);
        }
        return *this;
    }

    template<class _Rp, class ..._ArgTypes>
    function<_Rp(_ArgTypes...)>&
    function<_Rp(_ArgTypes...)>::operator=(function&& __f)
    {
        *this = nullptr;
        if (__f.__f_)
        {
            __f_ = __as_base(&__buf_);
            __f.__f_->__move(__f_);
            __f.__f_->destroy();
            __f.__f_ = 0;
        }
        return *this;
    }

    template<class _Rp, class ..._ArgTypes>
    function<_Rp(_ArgTypes...)>&
    function<_Rp(_ArgTypes...)>::operator=(nullptr_t) WI_NOEXCEPT
    {
        __base* __t = __f_;
        __f_ = 0;
        if (__t)
            __t->destroy();
        return *this;
    }

    template<class _Rp, class ..._ArgTypes>
    template <class _Fp, class>
    function<_Rp(_ArgTypes...)>&
    function<_Rp(_ArgTypes...)>::operator=(_Fp&& __f)
    {
        *this = nullptr;
        if (__function::__not_null(__f))
        {
            typedef __function::__func<typename decay<_Fp>::type, _Rp(_ArgTypes...)> _FF;
            static_assert(sizeof(_FF) <= sizeof(__buf_),
                "The sizeof(wistd::function) has grown too large for the reserved buffer (12 pointers).  Refactor to reduce size of the capture.");
            __f_ = ::new(static_cast<void*>(&__buf_)) _FF(wistd::move(__f));
        }

        return *this;
    }

    template<class _Rp, class ..._ArgTypes>
    function<_Rp(_ArgTypes...)>::~function()
    {
        if (__f_)
            __f_->destroy();
    }

    template<class _Rp, class ..._ArgTypes>
    void
    function<_Rp(_ArgTypes...)>::swap(function& __f)
    {
        if (wistd::addressof(__f) == this)
          return;
        if (__f_ && __f.__f_)
        {
            typename aligned_storage<sizeof(__buf_)>::type __tempbuf;
            __base* __t = __as_base(&__tempbuf);
            __f_->__move(__t);
            __f_->destroy();
            __f_ = 0;
            __f.__f_->__move(__as_base(&__buf_));
            __f.__f_->destroy();
            __f.__f_ = 0;
            __f_ = __as_base(&__buf_);
            __t->__move(__as_base(&__f.__buf_));
            __t->destroy();
            __f.__f_ = __as_base(&__f.__buf_);
        }
        else if (__f_)
        {
            __f_->__move(__as_base(&__f.__buf_));
            __f_->destroy();
            __f_ = 0;
            __f.__f_ = __as_base(&__f.__buf_);
        }
        else if (__f.__f_)
        {
            __f.__f_->__move(__as_base(&__buf_));
            __f.__f_->destroy();
            __f.__f_ = 0;
            __f_ = __as_base(&__buf_);
        }
    }

    template<class _Rp, class ..._ArgTypes>
    _Rp
    function<_Rp(_ArgTypes...)>::operator()(_ArgTypes... __arg) const
    {
        if (__f_ == nullptr)
            __throw_bad_function_call();
        return (*__f_)(wistd::forward<_ArgTypes>(__arg)...);
    }

    template <class _Rp, class... _ArgTypes>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator==(const function<_Rp(_ArgTypes...)>& __f, nullptr_t) WI_NOEXCEPT {return !__f;}

    template <class _Rp, class... _ArgTypes>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator==(nullptr_t, const function<_Rp(_ArgTypes...)>& __f) WI_NOEXCEPT {return !__f;}

    template <class _Rp, class... _ArgTypes>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator!=(const function<_Rp(_ArgTypes...)>& __f, nullptr_t) WI_NOEXCEPT {return (bool)__f;}

    template <class _Rp, class... _ArgTypes>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator!=(nullptr_t, const function<_Rp(_ArgTypes...)>& __f) WI_NOEXCEPT {return (bool)__f;}

    // Provide both 'swap_wil' and 'swap' since we now have two ADL scenarios that we need to work
    template <class _Rp, class... _ArgTypes>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    void
    swap(function<_Rp(_ArgTypes...)>& __x, function<_Rp(_ArgTypes...)>& __y)
    {return __x.swap(__y);}

    template <class _Rp, class... _ArgTypes>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    void
    swap_wil(function<_Rp(_ArgTypes...)>& __x, function<_Rp(_ArgTypes...)>& __y)
    {return __x.swap(__y);}

    // std::invoke
    template <class _Fn, class ..._Args>
    typename __invoke_of<_Fn, _Args...>::type
    invoke(_Fn&& __f, _Args&&... __args)
        __WI_NOEXCEPT_((__nothrow_invokable<_Fn, _Args...>::value))
    {
        return wistd::__invoke(wistd::forward<_Fn>(__f), wistd::forward<_Args>(__args)...);
    }

#else // __WI_LIBCPP_CXX03_LANG

#error wistd::function and wistd::invoke not implemented for pre-C++11

#endif
}
/// @endcond

#pragma warning(pop)

#endif  // _WISTD_FUNCTIONAL_H_
