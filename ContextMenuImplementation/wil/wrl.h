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
#ifndef __WIL_WRL_INCLUDED
#define __WIL_WRL_INCLUDED

#include <wrl.h>
#include "result.h"
#include "common.h" // wistd type_traits helpers
#include <libloaderapi.h> // GetModuleHandleW

/// @cond
EXTERN_C IMAGE_DOS_HEADER __ImageBase;
/// @endcond

namespace wil
{

#ifdef WIL_ENABLE_EXCEPTIONS
#pragma region Object construction helpers that throw exceptions

    /** Used to construct a RuntimeClass based object that uses 2 phase construction.
    Construct a RuntimeClass based object that uses 2 phase construction (by implementing
    RuntimeClassInitialize() and returning error codes for failures.
    ~~~~
        // SomeClass uses 2 phase initialization by implementing RuntimeClassInitialize()
        auto someClass = MakeAndInitializeOrThrow<SomeClass>(L"input", true);
    ~~~~ */

    template <typename T, typename... TArgs>
    Microsoft::WRL::ComPtr<T> MakeAndInitializeOrThrow(TArgs&&... args)
    {
        Microsoft::WRL::ComPtr<T> obj;
        THROW_IF_FAILED(Microsoft::WRL::MakeAndInitialize<T>(&obj, Microsoft::WRL::Details::Forward<TArgs>(args)...));
        return obj;
    }

    /** Used to construct an RuntimeClass based object that uses exceptions in its constructor (and does
    not require 2 phase construction).
    ~~~~
        // SomeClass uses exceptions for error handling in its constructor.
        auto someClass = MakeOrThrow<SomeClass>(L"input", true);
    ~~~~ */

    template <typename T, typename... TArgs>
    Microsoft::WRL::ComPtr<T> MakeOrThrow(TArgs&&... args)
    {
        // This is how you can detect the presence of RuntimeClassInitialize() and find dangerous use.
        // Unfortunately this produces false positives as all RuntimeClass derived classes have
        // a RuntimeClassInitialize() method from their base class.
        // static_assert(!std::is_member_function_pointer<decltype(&T::RuntimeClassInitialize)>::value,
        //    "class has a RuntimeClassInitialize member, use MakeAndInitializeOrThrow instead");
        auto obj = Microsoft::WRL::Make<T>(Microsoft::WRL::Details::Forward<TArgs>(args)...);
        THROW_IF_NULL_ALLOC(obj.Get());
        return obj;
    }
#pragma endregion

#endif // WIL_ENABLE_EXCEPTIONS

    /** By default WRL Callback objects are not agile, use this to make an agile one. Replace use of Callback<> with MakeAgileCallback<>.
    Will return null on failure, translate that into E_OUTOFMEMORY using XXX_IF_NULL_ALLOC()
    from wil\result.h to test the result. */
    template<typename TDelegateInterface, typename ...Args>
    ::Microsoft::WRL::ComPtr<TDelegateInterface> MakeAgileCallbackNoThrow(Args&&... args) WI_NOEXCEPT
    {
        using namespace Microsoft::WRL;
        return Callback<Implements<RuntimeClassFlags<ClassicCom>, TDelegateInterface, FtmBase>>(wistd::forward<Args>(args)...);
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    template<typename TDelegateInterface, typename ...Args>
    ::Microsoft::WRL::ComPtr<TDelegateInterface> MakeAgileCallback(Args&&... args)
    {
        auto result = MakeAgileCallbackNoThrow<TDelegateInterface, Args...>(wistd::forward<Args>(args)...);
        THROW_IF_NULL_ALLOC(result);
        return result;
    }
#endif // WIL_ENABLE_EXCEPTIONS

    /** Holds a reference to the host WRL module to prevent it from being unloaded.
    Normally, the reference is held implicitly because you are a member function
    of a DLL-hosted COM object, or because you retain a strong reference
    to some DLL-hosted COM object, but if those do not apply to you, then you
    will need to hold a reference explicitly. For examples (and for the C++/WinRT
    equivalent), see winrt_module_reference.
    */
    struct [[nodiscard]] wrl_module_reference
    {
        wrl_module_reference()
        {
            if (auto modulePtr = ::Microsoft::WRL::GetModuleBase())
            {
                modulePtr->IncrementObjectCount();
            }
            else
            {
#ifdef GET_MODULE_HANDLE_EX_FLAG_PIN
                // If this assertion fails, then you are using wrl_module_reference
                // from a DLL that does not host WRL objects, and the module reference
                // has no effect.
                WI_ASSERT(reinterpret_cast<HMODULE>(&__ImageBase) == GetModuleHandleW(nullptr));
#endif
            }
        }

        wrl_module_reference(wrl_module_reference const&) : wrl_module_reference() {}

        ~wrl_module_reference()
        {
            if (auto modulePtr = ::Microsoft::WRL::GetModuleBase())
            {
                modulePtr->DecrementObjectCount();
            }
        }
    };

} // namespace wil

#endif // __WIL_WRL_INCLUDED
