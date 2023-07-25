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
#ifndef __WIL_CPPWINRT_WRL_INCLUDED
#define __WIL_CPPWINRT_WRL_INCLUDED

#include "cppwinrt.h"
#include <winrt\base.h>

#include "result_macros.h"
#include <wrl\module.h>

// wil::wrl_factory_for_winrt_com_class provides interopability between a
// C++/WinRT class and the WRL Module system, allowing the winrt class to be
// CoCreatable.
//
// Usage:
//   - In your cpp, add:
//         CoCreatableCppWinRtClass(className)
//
//   - In the dll.cpp (or equivalent) for the module containing your class, add:
//         CoCreatableClassWrlCreatorMapInclude(className)
//
namespace wil
{
    namespace details
    {
        template <typename TCppWinRTClass>
        class module_count_wrapper : public TCppWinRTClass
        {
        public:
            module_count_wrapper()
            {
                if (auto modulePtr = ::Microsoft::WRL::GetModuleBase())
                {
                    modulePtr->IncrementObjectCount();
                }
            }

            virtual ~module_count_wrapper()
            {
                if (auto modulePtr = ::Microsoft::WRL::GetModuleBase())
                {
                    modulePtr->DecrementObjectCount();
                }
            }
        };
    }

    template <typename TCppWinRTClass>
    class wrl_factory_for_winrt_com_class : public ::Microsoft::WRL::ClassFactory<>
    {
    public:
        IFACEMETHODIMP CreateInstance(_In_opt_ ::IUnknown* unknownOuter, REFIID riid, _COM_Outptr_ void **object) noexcept try
        {
            *object = nullptr;
            RETURN_HR_IF(CLASS_E_NOAGGREGATION, unknownOuter != nullptr);

            return winrt::make<details::module_count_wrapper<TCppWinRTClass>>().as(riid, object);
        }
        CATCH_RETURN()
    };
}

#define CoCreatableCppWinRtClass(className) CoCreatableClassWithFactory(className, ::wil::wrl_factory_for_winrt_com_class<className>)

#endif // __WIL_CPPWINRT_WRL_INCLUDED
