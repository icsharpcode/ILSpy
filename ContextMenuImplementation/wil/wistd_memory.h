// -*- C++ -*-
//===-------------------------- memory ------------------------------------===//
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

#ifndef _WISTD_MEMORY_H_
#define _WISTD_MEMORY_H_

// DO NOT add *any* additional includes to this file -- there should be no dependencies from its usage
#include "wistd_type_traits.h"

#if !defined(__WI_LIBCPP_HAS_NO_PRAGMA_SYSTEM_HEADER)
#pragma GCC system_header
#endif

/// @cond
namespace wistd     // ("Windows Implementation" std)
{
    // allocator_traits

    template <class _Tp, class = void>
    struct __has_pointer_type : false_type {};

    template <class _Tp>
    struct __has_pointer_type<_Tp,
              typename __void_t<typename _Tp::pointer>::type> : true_type {};

    namespace __pointer_type_imp
    {

        template <class _Tp, class _Dp, bool = __has_pointer_type<_Dp>::value>
        struct __pointer_type
        {
            using type = typename _Dp::pointer;
        };

        template <class _Tp, class _Dp>
        struct __pointer_type<_Tp, _Dp, false>
        {
            using type = _Tp*;
        };

    }  // __pointer_type_imp

    template <class _Tp, class _Dp>
    struct __pointer_type
    {
        using type = typename __pointer_type_imp::__pointer_type<_Tp, typename remove_reference<_Dp>::type>::type;
    };

    template <class _Tp, int _Idx,
              bool _CanBeEmptyBase =
                  is_empty<_Tp>::value && !__libcpp_is_final<_Tp>::value>
    struct __compressed_pair_elem {
      using _ParamT = _Tp;
      using reference = _Tp&;
      using const_reference = const _Tp&;

#ifndef __WI_LIBCPP_CXX03_LANG
      __WI_LIBCPP_INLINE_VISIBILITY constexpr __compressed_pair_elem() : __value_() {}

      template <class _Up, class = typename enable_if<
          !is_same<__compressed_pair_elem, typename decay<_Up>::type>::value
      >::type>
      __WI_LIBCPP_INLINE_VISIBILITY
      constexpr explicit
      __compressed_pair_elem(_Up&& __u)
          : __value_(wistd::forward<_Up>(__u))
        {
        }

      // NOTE: Since we have not added 'tuple' to 'wistd', the 'piecewise' constructor has been removed
#else
      __WI_LIBCPP_INLINE_VISIBILITY __compressed_pair_elem() : __value_() {}
      __WI_LIBCPP_INLINE_VISIBILITY
      __compressed_pair_elem(_ParamT __p) : __value_(wistd::forward<_ParamT>(__p)) {}
#endif

      __WI_LIBCPP_INLINE_VISIBILITY reference __get() WI_NOEXCEPT { return __value_; }
      __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY
      const_reference __get() const WI_NOEXCEPT { return __value_; }

    private:
      _Tp __value_;
    };

    template <class _Tp, int _Idx>
    struct __compressed_pair_elem<_Tp, _Idx, true> : private _Tp {
      using _ParamT = _Tp;
      using reference = _Tp&;
      using const_reference = const _Tp&;
      using __value_type = _Tp;

#ifndef __WI_LIBCPP_CXX03_LANG
      __WI_LIBCPP_INLINE_VISIBILITY constexpr __compressed_pair_elem() = default;

      template <class _Up, class = typename enable_if<
            !is_same<__compressed_pair_elem, typename decay<_Up>::type>::value
      >::type>
      __WI_LIBCPP_INLINE_VISIBILITY
      constexpr explicit
      __compressed_pair_elem(_Up&& __u)
          : __value_type(wistd::forward<_Up>(__u))
      {}

      // NOTE: Since we have not added 'tuple' to 'wistd', the 'piecewise' constructor has been removed
#else
      __WI_LIBCPP_INLINE_VISIBILITY __compressed_pair_elem() : __value_type() {}
      __WI_LIBCPP_INLINE_VISIBILITY
      __compressed_pair_elem(_ParamT __p)
          : __value_type(wistd::forward<_ParamT>(__p)) {}
#endif

      __WI_LIBCPP_INLINE_VISIBILITY reference __get() WI_NOEXCEPT { return *this; }
      __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY
      const_reference __get() const WI_NOEXCEPT { return *this; }
    };

    // Tag used to construct the second element of the compressed pair.
    struct __second_tag {};

    template <class _T1, class _T2>
    class __declspec(empty_bases) __compressed_pair : private __compressed_pair_elem<_T1, 0>,
                                                      private __compressed_pair_elem<_T2, 1> {
      using _Base1 = __compressed_pair_elem<_T1, 0>;
      using _Base2 = __compressed_pair_elem<_T2, 1>;

      // NOTE: This static assert should never fire because __compressed_pair
      // is *almost never* used in a scenario where it's possible for T1 == T2.
      // (The exception is wistd::function where it is possible that the function
      //  object and the allocator have the same type).
      static_assert((!is_same<_T1, _T2>::value),
        "__compressed_pair cannot be instantated when T1 and T2 are the same type; "
        "The current implementation is NOT ABI-compatible with the previous "
        "implementation for this configuration");

    public:
#ifndef __WI_LIBCPP_CXX03_LANG
      template <bool _Dummy = true,
          class = typename enable_if<
              __dependent_type<is_default_constructible<_T1>, _Dummy>::value &&
              __dependent_type<is_default_constructible<_T2>, _Dummy>::value
          >::type
      >
      __WI_LIBCPP_INLINE_VISIBILITY
      constexpr __compressed_pair() {}

      template <class _Tp, typename enable_if<!is_same<typename decay<_Tp>::type,
                                                       __compressed_pair>::value,
                                              bool>::type = true>
      __WI_LIBCPP_INLINE_VISIBILITY constexpr explicit
      __compressed_pair(_Tp&& __t)
          : _Base1(wistd::forward<_Tp>(__t)), _Base2() {}

      template <class _Tp>
      __WI_LIBCPP_INLINE_VISIBILITY constexpr
      __compressed_pair(__second_tag, _Tp&& __t)
          : _Base1(), _Base2(wistd::forward<_Tp>(__t)) {}

      template <class _U1, class _U2>
      __WI_LIBCPP_INLINE_VISIBILITY constexpr
      __compressed_pair(_U1&& __t1, _U2&& __t2)
          : _Base1(wistd::forward<_U1>(__t1)), _Base2(wistd::forward<_U2>(__t2)) {}

      // NOTE: Since we have not added 'tuple' to 'wistd', the 'piecewise' constructor has been removed
#else
      __WI_LIBCPP_INLINE_VISIBILITY
      __compressed_pair() {}

      __WI_LIBCPP_INLINE_VISIBILITY explicit
      __compressed_pair(_T1 __t1) : _Base1(wistd::forward<_T1>(__t1)) {}

      __WI_LIBCPP_INLINE_VISIBILITY
      __compressed_pair(__second_tag, _T2 __t2)
          : _Base1(), _Base2(wistd::forward<_T2>(__t2)) {}

      __WI_LIBCPP_INLINE_VISIBILITY
      __compressed_pair(_T1 __t1, _T2 __t2)
          : _Base1(wistd::forward<_T1>(__t1)), _Base2(wistd::forward<_T2>(__t2)) {}
#endif

      __WI_LIBCPP_INLINE_VISIBILITY
      typename _Base1::reference first() WI_NOEXCEPT {
        return static_cast<_Base1&>(*this).__get();
      }

      __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY
      typename _Base1::const_reference first() const WI_NOEXCEPT {
        return static_cast<_Base1 const&>(*this).__get();
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      typename _Base2::reference second() WI_NOEXCEPT {
        return static_cast<_Base2&>(*this).__get();
      }

      __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY
      typename _Base2::const_reference second() const WI_NOEXCEPT {
        return static_cast<_Base2 const&>(*this).__get();
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      void swap(__compressed_pair& __x)
        __WI_NOEXCEPT_(__is_nothrow_swappable<_T1>::value &&
                     __is_nothrow_swappable<_T2>::value)
      {
        using wistd::swap_wil;
        swap_wil(first(), __x.first());
        swap_wil(second(), __x.second());
      }
    };

    // Provide both 'swap_wil' and 'swap' since we now have two ADL scenarios that we need to work
    template <class _T1, class _T2>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    void swap(__compressed_pair<_T1, _T2>& __x, __compressed_pair<_T1, _T2>& __y)
        __WI_NOEXCEPT_(__is_nothrow_swappable<_T1>::value &&
                     __is_nothrow_swappable<_T2>::value) {
      __x.swap(__y);
    }

    template <class _T1, class _T2>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    void swap_wil(__compressed_pair<_T1, _T2>& __x, __compressed_pair<_T1, _T2>& __y)
        __WI_NOEXCEPT_(__is_nothrow_swappable<_T1>::value &&
                     __is_nothrow_swappable<_T2>::value) {
      __x.swap(__y);
    }

    // default_delete

    template <class _Tp>
    struct __WI_LIBCPP_TEMPLATE_VIS default_delete {
        static_assert(!is_function<_Tp>::value,
                      "default_delete cannot be instantiated for function types");
#ifndef __WI_LIBCPP_CXX03_LANG
      __WI_LIBCPP_INLINE_VISIBILITY constexpr default_delete() WI_NOEXCEPT = default;
#else
      __WI_LIBCPP_INLINE_VISIBILITY default_delete() {}
#endif
      template <class _Up>
      __WI_LIBCPP_INLINE_VISIBILITY
      default_delete(const default_delete<_Up>&,
                     typename enable_if<is_convertible<_Up*, _Tp*>::value>::type* =
                         nullptr) WI_NOEXCEPT {}

      __WI_LIBCPP_INLINE_VISIBILITY void operator()(_Tp* __ptr) const WI_NOEXCEPT {
        static_assert(sizeof(_Tp) > 0,
                      "default_delete can not delete incomplete type");
        static_assert(!is_void<_Tp>::value,
                      "default_delete can not delete incomplete type");
        delete __ptr;
      }
    };

    template <class _Tp>
    struct __WI_LIBCPP_TEMPLATE_VIS default_delete<_Tp[]> {
    private:
      template <class _Up>
      struct _EnableIfConvertible
          : enable_if<is_convertible<_Up(*)[], _Tp(*)[]>::value> {};

    public:
#ifndef __WI_LIBCPP_CXX03_LANG
      __WI_LIBCPP_INLINE_VISIBILITY constexpr default_delete() WI_NOEXCEPT = default;
#else
      __WI_LIBCPP_INLINE_VISIBILITY default_delete() {}
#endif

      template <class _Up>
      __WI_LIBCPP_INLINE_VISIBILITY
      default_delete(const default_delete<_Up[]>&,
                     typename _EnableIfConvertible<_Up>::type* = nullptr) WI_NOEXCEPT {}

      template <class _Up>
      __WI_LIBCPP_INLINE_VISIBILITY
      typename _EnableIfConvertible<_Up>::type
      operator()(_Up* __ptr) const WI_NOEXCEPT {
        static_assert(sizeof(_Tp) > 0,
                      "default_delete can not delete incomplete type");
        static_assert(!is_void<_Tp>::value,
                      "default_delete can not delete void type");
        delete[] __ptr;
      }
    };



#ifndef __WI_LIBCPP_CXX03_LANG
    template <class _Deleter>
    struct __unique_ptr_deleter_sfinae {
      static_assert(!is_reference<_Deleter>::value, "incorrect specialization");
      using __lval_ref_type = const _Deleter&;
      using __good_rval_ref_type = _Deleter&&;
      using __enable_rval_overload = true_type;
    };

    template <class _Deleter>
    struct __unique_ptr_deleter_sfinae<_Deleter const&> {
      using __lval_ref_type = const _Deleter&;
      using __bad_rval_ref_type = const _Deleter&&;
      using __enable_rval_overload = false_type;
    };

    template <class _Deleter>
    struct __unique_ptr_deleter_sfinae<_Deleter&> {
      using __lval_ref_type = _Deleter&;
      using __bad_rval_ref_type = _Deleter&&;
      using __enable_rval_overload = false_type;
    };
#endif // !defined(__WI_LIBCPP_CXX03_LANG)

    template <class _Tp, class _Dp = default_delete<_Tp> >
    class __WI_LIBCPP_TEMPLATE_VIS unique_ptr {
    public:
      using element_type = _Tp;
      using deleter_type = _Dp;
      using pointer = typename __pointer_type<_Tp, deleter_type>::type;

      static_assert(!is_rvalue_reference<deleter_type>::value,
                    "the specified deleter type cannot be an rvalue reference");

    private:
      __compressed_pair<pointer, deleter_type> __ptr_;

      struct __nat { int __for_bool_; };

#ifndef __WI_LIBCPP_CXX03_LANG
      using _DeleterSFINAE = __unique_ptr_deleter_sfinae<_Dp>;

      template <bool _Dummy>
      using _LValRefType =
          typename __dependent_type<_DeleterSFINAE, _Dummy>::__lval_ref_type;

      template <bool _Dummy>
      using _GoodRValRefType =
          typename __dependent_type<_DeleterSFINAE, _Dummy>::__good_rval_ref_type;

      template <bool _Dummy>
      using _BadRValRefType =
          typename __dependent_type<_DeleterSFINAE, _Dummy>::__bad_rval_ref_type;

      template <bool _Dummy, class _Deleter = typename __dependent_type<
                                 __identity<deleter_type>, _Dummy>::type>
      using _EnableIfDeleterDefaultConstructible =
          typename enable_if<is_default_constructible<_Deleter>::value &&
                             !is_pointer<_Deleter>::value>::type;

      template <class _ArgType>
      using _EnableIfDeleterConstructible =
          typename enable_if<is_constructible<deleter_type, _ArgType>::value>::type;

      template <class _UPtr, class _Up>
      using _EnableIfMoveConvertible = typename enable_if<
          is_convertible<typename _UPtr::pointer, pointer>::value &&
          !is_array<_Up>::value
      >::type;

      template <class _UDel>
      using _EnableIfDeleterConvertible = typename enable_if<
          (is_reference<_Dp>::value && is_same<_Dp, _UDel>::value) ||
          (!is_reference<_Dp>::value && is_convertible<_UDel, _Dp>::value)
        >::type;

      template <class _UDel>
      using _EnableIfDeleterAssignable = typename enable_if<
          is_assignable<_Dp&, _UDel&&>::value
        >::type;

    public:
      template <bool _Dummy = true,
                class = _EnableIfDeleterDefaultConstructible<_Dummy>>
      __WI_LIBCPP_INLINE_VISIBILITY
      constexpr unique_ptr() WI_NOEXCEPT : __ptr_(pointer()) {}

      template <bool _Dummy = true,
                class = _EnableIfDeleterDefaultConstructible<_Dummy>>
      __WI_LIBCPP_INLINE_VISIBILITY
      constexpr unique_ptr(nullptr_t) WI_NOEXCEPT : __ptr_(pointer()) {}

      template <bool _Dummy = true,
                class = _EnableIfDeleterDefaultConstructible<_Dummy>>
      __WI_LIBCPP_INLINE_VISIBILITY
      explicit unique_ptr(pointer __p) WI_NOEXCEPT : __ptr_(__p) {}

      template <bool _Dummy = true,
                class = _EnableIfDeleterConstructible<_LValRefType<_Dummy>>>
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(pointer __p, _LValRefType<_Dummy> __d) WI_NOEXCEPT
          : __ptr_(__p, __d) {}

      template <bool _Dummy = true,
                class = _EnableIfDeleterConstructible<_GoodRValRefType<_Dummy>>>
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(pointer __p, _GoodRValRefType<_Dummy> __d) WI_NOEXCEPT
          : __ptr_(__p, wistd::move(__d)) {
        static_assert(!is_reference<deleter_type>::value,
                      "rvalue deleter bound to reference");
      }

      template <bool _Dummy = true,
                class = _EnableIfDeleterConstructible<_BadRValRefType<_Dummy>>>
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(pointer __p, _BadRValRefType<_Dummy> __d) = delete;

      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(unique_ptr&& __u) WI_NOEXCEPT
          : __ptr_(__u.release(), wistd::forward<deleter_type>(__u.get_deleter())) {
      }

      template <class _Up, class _Ep,
          class = _EnableIfMoveConvertible<unique_ptr<_Up, _Ep>, _Up>,
          class = _EnableIfDeleterConvertible<_Ep>
      >
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(unique_ptr<_Up, _Ep>&& __u) WI_NOEXCEPT
          : __ptr_(__u.release(), wistd::forward<_Ep>(__u.get_deleter())) {}

      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr& operator=(unique_ptr&& __u) WI_NOEXCEPT {
        reset(__u.release());
        __ptr_.second() = wistd::forward<deleter_type>(__u.get_deleter());
        return *this;
      }

      template <class _Up, class _Ep,
          class = _EnableIfMoveConvertible<unique_ptr<_Up, _Ep>, _Up>,
          class = _EnableIfDeleterAssignable<_Ep>
      >
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr& operator=(unique_ptr<_Up, _Ep>&& __u) WI_NOEXCEPT {
        reset(__u.release());
        __ptr_.second() = wistd::forward<_Ep>(__u.get_deleter());
        return *this;
      }

#else  // __WI_LIBCPP_CXX03_LANG
    private:
      unique_ptr(unique_ptr&);
      template <class _Up, class _Ep> unique_ptr(unique_ptr<_Up, _Ep>&);

      unique_ptr& operator=(unique_ptr&);
      template <class _Up, class _Ep> unique_ptr& operator=(unique_ptr<_Up, _Ep>&);

    public:
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr() : __ptr_(pointer())
      {
        static_assert(!is_pointer<deleter_type>::value,
                      "unique_ptr constructed with null function pointer deleter");
        static_assert(is_default_constructible<deleter_type>::value,
                      "unique_ptr::deleter_type is not default constructible");
      }
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(nullptr_t) : __ptr_(pointer())
      {
        static_assert(!is_pointer<deleter_type>::value,
                      "unique_ptr constructed with null function pointer deleter");
      }
      __WI_LIBCPP_INLINE_VISIBILITY
      explicit unique_ptr(pointer __p)
          : __ptr_(wistd::move(__p)) {
        static_assert(!is_pointer<deleter_type>::value,
                      "unique_ptr constructed with null function pointer deleter");
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      operator __rv<unique_ptr>() {
        return __rv<unique_ptr>(*this);
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(__rv<unique_ptr> __u)
          : __ptr_(__u->release(),
                   wistd::forward<deleter_type>(__u->get_deleter())) {}

      template <class _Up, class _Ep>
      __WI_LIBCPP_INLINE_VISIBILITY
      typename enable_if<
          !is_array<_Up>::value &&
              is_convertible<typename unique_ptr<_Up, _Ep>::pointer,
                             pointer>::value &&
              is_assignable<deleter_type&, _Ep&>::value,
          unique_ptr&>::type
      operator=(unique_ptr<_Up, _Ep> __u) {
        reset(__u.release());
        __ptr_.second() = wistd::forward<_Ep>(__u.get_deleter());
        return *this;
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(pointer __p, deleter_type __d)
          : __ptr_(wistd::move(__p), wistd::move(__d)) {}
#endif // __WI_LIBCPP_CXX03_LANG

      __WI_LIBCPP_INLINE_VISIBILITY
      ~unique_ptr() { reset(); }

      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr& operator=(nullptr_t) WI_NOEXCEPT {
        reset();
        return *this;
      }

      __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY
      typename add_lvalue_reference<_Tp>::type
      operator*() const {
        return *__ptr_.first();
      }
      __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY
      pointer operator->() const WI_NOEXCEPT {
        return __ptr_.first();
      }
      __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY
      pointer get() const WI_NOEXCEPT {
        return __ptr_.first();
      }
      __WI_LIBCPP_INLINE_VISIBILITY
      deleter_type& get_deleter() WI_NOEXCEPT {
        return __ptr_.second();
      }
      __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY
      const deleter_type& get_deleter() const WI_NOEXCEPT {
        return __ptr_.second();
      }
      __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY
      __WI_LIBCPP_EXPLICIT operator bool() const WI_NOEXCEPT {
        return __ptr_.first() != nullptr;
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      pointer release() WI_NOEXCEPT {
        pointer __t = __ptr_.first();
        __ptr_.first() = pointer();
        return __t;
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      void reset(pointer __p = pointer()) WI_NOEXCEPT {
        pointer __tmp = __ptr_.first();
        __ptr_.first() = __p;
        if (__tmp)
          __ptr_.second()(__tmp);
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      void swap(unique_ptr& __u) WI_NOEXCEPT {
        __ptr_.swap(__u.__ptr_);
      }
    };


    template <class _Tp, class _Dp>
    class __WI_LIBCPP_TEMPLATE_VIS unique_ptr<_Tp[], _Dp> {
    public:
      using element_type = _Tp;
      using deleter_type = _Dp;
      using pointer = typename __pointer_type<_Tp, deleter_type>::type;

    private:
      __compressed_pair<pointer, deleter_type> __ptr_;

      template <class _From>
      struct _CheckArrayPointerConversion : is_same<_From, pointer> {};

      template <class _FromElem>
      struct _CheckArrayPointerConversion<_FromElem*>
          : integral_constant<bool,
              is_same<_FromElem*, pointer>::value ||
                (is_same<pointer, element_type*>::value &&
                 is_convertible<_FromElem(*)[], element_type(*)[]>::value)
          >
      {};

#ifndef __WI_LIBCPP_CXX03_LANG
      using _DeleterSFINAE = __unique_ptr_deleter_sfinae<_Dp>;

      template <bool _Dummy>
      using _LValRefType =
          typename __dependent_type<_DeleterSFINAE, _Dummy>::__lval_ref_type;

      template <bool _Dummy>
      using _GoodRValRefType =
          typename __dependent_type<_DeleterSFINAE, _Dummy>::__good_rval_ref_type;

      template <bool _Dummy>
      using _BadRValRefType =
          typename __dependent_type<_DeleterSFINAE, _Dummy>::__bad_rval_ref_type;

      template <bool _Dummy, class _Deleter = typename __dependent_type<
                                 __identity<deleter_type>, _Dummy>::type>
      using _EnableIfDeleterDefaultConstructible =
          typename enable_if<is_default_constructible<_Deleter>::value &&
                             !is_pointer<_Deleter>::value>::type;

      template <class _ArgType>
      using _EnableIfDeleterConstructible =
          typename enable_if<is_constructible<deleter_type, _ArgType>::value>::type;

      template <class _Pp>
      using _EnableIfPointerConvertible = typename enable_if<
          _CheckArrayPointerConversion<_Pp>::value
      >::type;

      template <class _UPtr, class _Up,
            class _ElemT = typename _UPtr::element_type>
      using _EnableIfMoveConvertible = typename enable_if<
          is_array<_Up>::value &&
          is_same<pointer, element_type*>::value &&
          is_same<typename _UPtr::pointer, _ElemT*>::value &&
          is_convertible<_ElemT(*)[], element_type(*)[]>::value
        >::type;

      template <class _UDel>
      using _EnableIfDeleterConvertible = typename enable_if<
          (is_reference<_Dp>::value && is_same<_Dp, _UDel>::value) ||
          (!is_reference<_Dp>::value && is_convertible<_UDel, _Dp>::value)
        >::type;

      template <class _UDel>
      using _EnableIfDeleterAssignable = typename enable_if<
          is_assignable<_Dp&, _UDel&&>::value
        >::type;

    public:
      template <bool _Dummy = true,
                class = _EnableIfDeleterDefaultConstructible<_Dummy>>
      __WI_LIBCPP_INLINE_VISIBILITY
      constexpr unique_ptr() WI_NOEXCEPT : __ptr_(pointer()) {}

      template <bool _Dummy = true,
                class = _EnableIfDeleterDefaultConstructible<_Dummy>>
      __WI_LIBCPP_INLINE_VISIBILITY
      constexpr unique_ptr(nullptr_t) WI_NOEXCEPT : __ptr_(pointer()) {}

      template <class _Pp, bool _Dummy = true,
                class = _EnableIfDeleterDefaultConstructible<_Dummy>,
                class = _EnableIfPointerConvertible<_Pp>>
      __WI_LIBCPP_INLINE_VISIBILITY
      explicit unique_ptr(_Pp __p) WI_NOEXCEPT
          : __ptr_(__p) {}

      template <class _Pp, bool _Dummy = true,
                class = _EnableIfDeleterConstructible<_LValRefType<_Dummy>>,
                class = _EnableIfPointerConvertible<_Pp>>
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(_Pp __p, _LValRefType<_Dummy> __d) WI_NOEXCEPT
          : __ptr_(__p, __d) {}

      template <bool _Dummy = true,
                class = _EnableIfDeleterConstructible<_LValRefType<_Dummy>>>
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(nullptr_t, _LValRefType<_Dummy> __d) WI_NOEXCEPT
          : __ptr_(nullptr, __d) {}

      template <class _Pp, bool _Dummy = true,
                class = _EnableIfDeleterConstructible<_GoodRValRefType<_Dummy>>,
                class = _EnableIfPointerConvertible<_Pp>>
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(_Pp __p, _GoodRValRefType<_Dummy> __d) WI_NOEXCEPT
          : __ptr_(__p, wistd::move(__d)) {
        static_assert(!is_reference<deleter_type>::value,
                      "rvalue deleter bound to reference");
      }

      template <bool _Dummy = true,
                class = _EnableIfDeleterConstructible<_GoodRValRefType<_Dummy>>>
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(nullptr_t, _GoodRValRefType<_Dummy> __d) WI_NOEXCEPT
          : __ptr_(nullptr, wistd::move(__d)) {
        static_assert(!is_reference<deleter_type>::value,
                      "rvalue deleter bound to reference");
      }

      template <class _Pp, bool _Dummy = true,
                class = _EnableIfDeleterConstructible<_BadRValRefType<_Dummy>>,
                class = _EnableIfPointerConvertible<_Pp>>
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(_Pp __p, _BadRValRefType<_Dummy> __d) = delete;

      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(unique_ptr&& __u) WI_NOEXCEPT
          : __ptr_(__u.release(), wistd::forward<deleter_type>(__u.get_deleter())) {
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr& operator=(unique_ptr&& __u) WI_NOEXCEPT {
        reset(__u.release());
        __ptr_.second() = wistd::forward<deleter_type>(__u.get_deleter());
        return *this;
      }

      template <class _Up, class _Ep,
          class = _EnableIfMoveConvertible<unique_ptr<_Up, _Ep>, _Up>,
          class = _EnableIfDeleterConvertible<_Ep>
      >
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(unique_ptr<_Up, _Ep>&& __u) WI_NOEXCEPT
          : __ptr_(__u.release(), wistd::forward<_Ep>(__u.get_deleter())) {
      }

      template <class _Up, class _Ep,
          class = _EnableIfMoveConvertible<unique_ptr<_Up, _Ep>, _Up>,
          class = _EnableIfDeleterAssignable<_Ep>
      >
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr&
      operator=(unique_ptr<_Up, _Ep>&& __u) WI_NOEXCEPT {
        reset(__u.release());
        __ptr_.second() = wistd::forward<_Ep>(__u.get_deleter());
        return *this;
      }

#else // __WI_LIBCPP_CXX03_LANG
    private:
      template <class _Up> explicit unique_ptr(_Up);

      unique_ptr(unique_ptr&);
      template <class _Up> unique_ptr(unique_ptr<_Up>&);

      unique_ptr& operator=(unique_ptr&);
      template <class _Up> unique_ptr& operator=(unique_ptr<_Up>&);

      template <class _Up>
      unique_ptr(_Up __u,
                 typename conditional<
                     is_reference<deleter_type>::value, deleter_type,
                     typename add_lvalue_reference<const deleter_type>::type>::type,
                 typename enable_if<is_convertible<_Up, pointer>::value,
                                    __nat>::type = __nat());
    public:
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr() : __ptr_(pointer()) {
        static_assert(!is_pointer<deleter_type>::value,
                      "unique_ptr constructed with null function pointer deleter");
      }
      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(nullptr_t) : __ptr_(pointer()) {
        static_assert(!is_pointer<deleter_type>::value,
                      "unique_ptr constructed with null function pointer deleter");
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      explicit unique_ptr(pointer __p) : __ptr_(__p) {
        static_assert(!is_pointer<deleter_type>::value,
                      "unique_ptr constructed with null function pointer deleter");
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(pointer __p, deleter_type __d)
          : __ptr_(__p, wistd::forward<deleter_type>(__d)) {}

      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(nullptr_t, deleter_type __d)
          : __ptr_(pointer(), wistd::forward<deleter_type>(__d)) {}

      __WI_LIBCPP_INLINE_VISIBILITY
      operator __rv<unique_ptr>() {
        return __rv<unique_ptr>(*this);
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr(__rv<unique_ptr> __u)
          : __ptr_(__u->release(),
                   wistd::forward<deleter_type>(__u->get_deleter())) {}

      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr& operator=(__rv<unique_ptr> __u) {
        reset(__u->release());
        __ptr_.second() = wistd::forward<deleter_type>(__u->get_deleter());
        return *this;
      }

#endif // __WI_LIBCPP_CXX03_LANG

    public:
      __WI_LIBCPP_INLINE_VISIBILITY
      ~unique_ptr() { reset(); }

      __WI_LIBCPP_INLINE_VISIBILITY
      unique_ptr& operator=(nullptr_t) WI_NOEXCEPT {
        reset();
        return *this;
      }

      __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY
      typename add_lvalue_reference<_Tp>::type
      operator[](size_t __i) const {
        return __ptr_.first()[__i];
      }
      __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY
      pointer get() const WI_NOEXCEPT {
        return __ptr_.first();
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      deleter_type& get_deleter() WI_NOEXCEPT {
        return __ptr_.second();
      }

      __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY
      const deleter_type& get_deleter() const WI_NOEXCEPT {
        return __ptr_.second();
      }
      __WI_LIBCPP_NODISCARD_ATTRIBUTE __WI_LIBCPP_INLINE_VISIBILITY
      __WI_LIBCPP_EXPLICIT operator bool() const WI_NOEXCEPT {
        return __ptr_.first() != nullptr;
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      pointer release() WI_NOEXCEPT {
        pointer __t = __ptr_.first();
        __ptr_.first() = pointer();
        return __t;
      }

      template <class _Pp>
      __WI_LIBCPP_INLINE_VISIBILITY
      typename enable_if<
          _CheckArrayPointerConversion<_Pp>::value
      >::type
      reset(_Pp __p) WI_NOEXCEPT {
        pointer __tmp = __ptr_.first();
        __ptr_.first() = __p;
        if (__tmp)
          __ptr_.second()(__tmp);
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      void reset(nullptr_t = nullptr) WI_NOEXCEPT {
        pointer __tmp = __ptr_.first();
        __ptr_.first() = nullptr;
        if (__tmp)
          __ptr_.second()(__tmp);
      }

      __WI_LIBCPP_INLINE_VISIBILITY
      void swap(unique_ptr& __u) WI_NOEXCEPT {
        __ptr_.swap(__u.__ptr_);
      }

    };

    // Provide both 'swap_wil' and 'swap' since we now have two ADL scenarios that we need to work
    template <class _Tp, class _Dp>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    typename enable_if<
        __is_swappable<_Dp>::value,
        void
    >::type
    swap(unique_ptr<_Tp, _Dp>& __x, unique_ptr<_Tp, _Dp>& __y) WI_NOEXCEPT {__x.swap(__y);}

    template <class _Tp, class _Dp>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    typename enable_if<
        __is_swappable<_Dp>::value,
        void
    >::type
    swap_wil(unique_ptr<_Tp, _Dp>& __x, unique_ptr<_Tp, _Dp>& __y) WI_NOEXCEPT {__x.swap(__y);}

    template <class _T1, class _D1, class _T2, class _D2>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator==(const unique_ptr<_T1, _D1>& __x, const unique_ptr<_T2, _D2>& __y) {return __x.get() == __y.get();}

    template <class _T1, class _D1, class _T2, class _D2>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator!=(const unique_ptr<_T1, _D1>& __x, const unique_ptr<_T2, _D2>& __y) {return !(__x == __y);}

    template <class _T1, class _D1, class _T2, class _D2>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator< (const unique_ptr<_T1, _D1>& __x, const unique_ptr<_T2, _D2>& __y)
    {
        typedef typename unique_ptr<_T1, _D1>::pointer _P1;
        typedef typename unique_ptr<_T2, _D2>::pointer _P2;
        typedef typename common_type<_P1, _P2>::type _Vp;
        return less<_Vp>()(__x.get(), __y.get());
    }

    template <class _T1, class _D1, class _T2, class _D2>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator> (const unique_ptr<_T1, _D1>& __x, const unique_ptr<_T2, _D2>& __y) {return __y < __x;}

    template <class _T1, class _D1, class _T2, class _D2>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator<=(const unique_ptr<_T1, _D1>& __x, const unique_ptr<_T2, _D2>& __y) {return !(__y < __x);}

    template <class _T1, class _D1, class _T2, class _D2>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator>=(const unique_ptr<_T1, _D1>& __x, const unique_ptr<_T2, _D2>& __y) {return !(__x < __y);}

    template <class _T1, class _D1>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator==(const unique_ptr<_T1, _D1>& __x, nullptr_t) WI_NOEXCEPT
    {
        return !__x;
    }

    template <class _T1, class _D1>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator==(nullptr_t, const unique_ptr<_T1, _D1>& __x) WI_NOEXCEPT
    {
        return !__x;
    }

    template <class _T1, class _D1>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator!=(const unique_ptr<_T1, _D1>& __x, nullptr_t) WI_NOEXCEPT
    {
        return static_cast<bool>(__x);
    }

    template <class _T1, class _D1>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator!=(nullptr_t, const unique_ptr<_T1, _D1>& __x) WI_NOEXCEPT
    {
        return static_cast<bool>(__x);
    }

    template <class _T1, class _D1>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator<(const unique_ptr<_T1, _D1>& __x, nullptr_t)
    {
        typedef typename unique_ptr<_T1, _D1>::pointer _P1;
        return less<_P1>()(__x.get(), nullptr);
    }

    template <class _T1, class _D1>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator<(nullptr_t, const unique_ptr<_T1, _D1>& __x)
    {
        typedef typename unique_ptr<_T1, _D1>::pointer _P1;
        return less<_P1>()(nullptr, __x.get());
    }

    template <class _T1, class _D1>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator>(const unique_ptr<_T1, _D1>& __x, nullptr_t)
    {
        return nullptr < __x;
    }

    template <class _T1, class _D1>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator>(nullptr_t, const unique_ptr<_T1, _D1>& __x)
    {
        return __x < nullptr;
    }

    template <class _T1, class _D1>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator<=(const unique_ptr<_T1, _D1>& __x, nullptr_t)
    {
        return !(nullptr < __x);
    }

    template <class _T1, class _D1>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator<=(nullptr_t, const unique_ptr<_T1, _D1>& __x)
    {
        return !(__x < nullptr);
    }

    template <class _T1, class _D1>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator>=(const unique_ptr<_T1, _D1>& __x, nullptr_t)
    {
        return !(__x < nullptr);
    }

    template <class _T1, class _D1>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    bool
    operator>=(nullptr_t, const unique_ptr<_T1, _D1>& __x)
    {
        return !(nullptr < __x);
    }

#ifdef __WI_LIBCPP_HAS_NO_RVALUE_REFERENCES

    template <class _Tp, class _Dp>
    inline __WI_LIBCPP_INLINE_VISIBILITY
    unique_ptr<_Tp, _Dp>
    move(unique_ptr<_Tp, _Dp>& __t)
    {
        return unique_ptr<_Tp, _Dp>(__rv<unique_ptr<_Tp, _Dp> >(__t));
    }

#endif
}
/// @endcond

#endif  // _WISTD_MEMORY_H_
