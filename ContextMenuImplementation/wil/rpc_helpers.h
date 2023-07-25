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
#ifndef __WIL_RPC_HELPERS_INCLUDED
#define __WIL_RPC_HELPERS_INCLUDED

#include "result.h"
#include "resource.h"
#include "wistd_functional.h"
#include "wistd_type_traits.h"

namespace wil
{

    /// @cond
    namespace details
    {
        // This call-adapter template converts a void-returning 'wistd::invoke' into
        // an HRESULT-returning 'wistd::invoke' that emits S_OK. It can be eliminated
        // with 'if constexpr' when C++17 is in wide use.
        template<typename TReturnType> struct call_adapter
        {
            template<typename... TArgs> static HRESULT call(TArgs&& ... args)
            {
                return wistd::invoke(wistd::forward<TArgs>(args)...);
            }
        };

        template<> struct call_adapter<void>
        {
            template<typename... TArgs> static HRESULT call(TArgs&& ... args)
            {
                wistd::invoke(wistd::forward<TArgs>(args)...);
                return S_OK;
            }
        };

        // Some RPC exceptions are already HRESULTs. Others are in the regular Win32
        // error space. If the incoming exception code isn't an HRESULT, wrap it.
        constexpr HRESULT map_rpc_exception(DWORD code)
        {
            return IS_ERROR(code) ? code : __HRESULT_FROM_WIN32(code);
        }
    }
    /// @endcond

    /** Invokes an RPC method, mapping structured exceptions to HRESULTs
    Failures encountered by the RPC infrastructure (such as server crashes, authentication
    errors, client parameter issues, etc.) are emitted by raising a structured exception from
    within the RPC machinery. This method wraps the requested call in the usual RpcTryExcept,
    RpcTryCatch, and RpcEndExcept sequence then maps the exceptions to HRESULTs for the usual
    flow control machinery to use.

    Many RPC methods are defined as returning HRESULT themselves, where the HRESULT indicates
    the result of the _work_. HRESULTs returned by a successful completion of the _call_ are
    returned as-is.

    RPC methods that have a return type of 'void' are mapped to returning S_OK when the _call_
    completes successfully.

    For example, consider an RPC interface method defined in idl as:
    ~~~
    HRESULT GetKittenState([in, ref, string] const wchar_t* name, [out, retval] KittenState** state);
    ~~~
    To call this method, use:
    ~~~
    wil::unique_rpc_binding binding = // typically gotten elsewhere;
    wil::unique_midl_ptr<KittenState> state;
    HRESULT hr = wil::invoke_rpc_nothrow(GetKittenState, binding.get(), L"fluffy", state.put());
    RETURN_IF_FAILED(hr);
    ~~~
    */
    template<typename... TCall> HRESULT invoke_rpc_nothrow(TCall&&... args) WI_NOEXCEPT
    {
        RpcTryExcept
        {
            // Note: this helper type can be removed with C++17 enabled via
            // 'if constexpr(wistd::is_same_v<void, result_t>)'
            using result_t = typename wistd::__invoke_of<TCall...>::type;
            RETURN_IF_FAILED(details::call_adapter<result_t>::call(wistd::forward<TCall>(args)...));
            return S_OK;
        }
        RpcExcept(RpcExceptionFilter(RpcExceptionCode()))
        {
            RETURN_HR(details::map_rpc_exception(RpcExceptionCode()));
        }
        RpcEndExcept
    }

    /** Invokes an RPC method, mapping structured exceptions to HRESULTs
    Failures encountered by the RPC infrastructure (such as server crashes, authentication
    errors, client parameter issues, etc.) are emitted by raising a structured exception from
    within the RPC machinery. This method wraps the requested call in the usual RpcTryExcept,
    RpcTryCatch, and RpcEndExcept sequence then maps the exceptions to HRESULTs for the usual
    flow control machinery to use.

    Some RPC methods return results (such as a state enumeration or other value) directly in
    their signature. This adapter writes that result into a caller-provided object then
    returns S_OK.

    For example, consider an RPC interface method defined in idl as:
    ~~~
    GUID GetKittenId([in, ref, string] const wchar_t* name);
    ~~~
    To call this method, use:
    ~~~
    wil::unique_rpc_binding binding = // typically gotten elsewhere;
    GUID id;
    HRESULT hr = wil::invoke_rpc_result_nothrow(id, GetKittenId, binding.get(), L"fluffy");
    RETURN_IF_FAILED(hr);
    ~~~
    */
    template<typename TResult, typename... TCall> HRESULT invoke_rpc_result_nothrow(TResult& result, TCall&&... args) WI_NOEXCEPT
    {
        RpcTryExcept
        {
            result = wistd::invoke(wistd::forward<TCall>(args)...);
            return S_OK;
        }
        RpcExcept(RpcExceptionFilter(RpcExceptionCode()))
        {
            RETURN_HR(details::map_rpc_exception(RpcExceptionCode()));
        }
        RpcEndExcept
    }

    namespace details
    {
        // Provides an adapter around calling the context-handle-close method on an
        // RPC interface, which itself is an RPC call.
        template<typename TStorage, typename close_fn_t, close_fn_t close_fn>
        struct rpc_closer_t
        {
            static void Close(TStorage arg) WI_NOEXCEPT
            {
                LOG_IF_FAILED(invoke_rpc_nothrow(close_fn, &arg));
            }
        };
    }

    /** Manages explicit RPC context handles
    Explicit RPC context handles are used in many RPC interfaces. Most interfaces with
    context handles have an explicit `FooClose([in, out] CONTEXT*)` method that lets
    the server close out the context handle. As the close method itself is an RPC call,
    it can fail and raise a structured exception.

    This type routes the context-handle-specific `Close` call through the `invoke_rpc_nothrow`
    helper, ensuring correct cleanup and lifecycle management.
    ~~~
    // Assume the interface has two methods:
    // HRESULT OpenFoo([in] handle_t binding, [out] FOO_CONTEXT*);
    // HRESULT UseFoo([in] FOO_CONTEXT context;
    // void CloseFoo([in, out] PFOO_CONTEXT);
    using unique_foo_context = wil::unique_rpc_context_handle<FOO_CONTEXT, decltype(&CloseFoo), CloseFoo>;
    unique_foo_context context;
    RETURN_IF_FAILED(wil::invoke_rpc_nothrow(OpenFoo, m_binding.get(), context.put()));
    RETURN_IF_FAILED(wil::invoke_rpc_nothrow(UseFoo, context.get()));
    context.reset();
    ~~~
    */
    template<typename TContext, typename close_fn_t, close_fn_t close_fn>
    using unique_rpc_context_handle = unique_any<TContext, decltype(&details::rpc_closer_t<TContext, close_fn_t, close_fn>::Close), details::rpc_closer_t<TContext, close_fn_t, close_fn>::Close>;

#ifdef WIL_ENABLE_EXCEPTIONS
    /** Invokes an RPC method, mapping structured exceptions to C++ exceptions
    See `wil::invoke_rpc_nothrow` for additional information.  Failures during the _call_
    and those returned by the _method_ are mapped to HRESULTs and thrown inside a
    wil::ResultException. Using the example RPC method provided above:
    ~~~
    wil::unique_midl_ptr<KittenState> state;
    wil::invoke_rpc(GetKittenState, binding.get(), L"fluffy", state.put());
    // use 'state'
    ~~~
    */
    template<typename... TCall> void invoke_rpc(TCall&& ... args)
    {
        THROW_IF_FAILED(invoke_rpc_nothrow(wistd::forward<TCall>(args)...));
    }

    /** Invokes an RPC method, mapping structured exceptions to C++ exceptions
    See `wil::invoke_rpc_result_nothrow` for additional information. Failures during the
    _call_ are mapped to HRESULTs and thrown inside a `wil::ResultException`. Using the
    example RPC method provided above:
    ~~~
    GUID id = wil::invoke_rpc_result(GetKittenId, binding.get());
    // use 'id'
    ~~~
    */
    template<typename... TCall> auto invoke_rpc_result(TCall&& ... args)
    {
        using result_t = typename wistd::__invoke_of<TCall...>::type;
        result_t result{};
        THROW_IF_FAILED(invoke_rpc_result_nothrow(result, wistd::forward<TCall>(args)...));
        return result;
    }
#endif
}

#endif
