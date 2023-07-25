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

// Note: When origination is enabled by including this file, origination is done as part of the RETURN_* and THROW_* macros.  Before originating
// a new error we will observe whether there is already an error payload associated with the current thread.  If there is, and the HRESULTs match,
// then a new error will not be originated.  Otherwise we will overwrite it with a new origination.  The ABI boundary for WinRT APIs will check the
// per-thread error information.  The act of checking the error clears it, so there should be minimal risk of failing to originate distinct errors
// simply because the HRESULTs match.
//
// For THROW_ macros we will examine the thread-local error storage once per throw.  So typically once, with additional calls if the exception is
// caught and re-thrown.
//
// For RETURN_ macros we will have to examine the thread-local error storage once per frame as the call stack unwinds.  Because error conditions
// -should- be uncommon the performance impact of checking TLS should be minimal.  The more expensive part is originating the error because it must
// capture the entire stack and some additional data.

#ifndef __WIL_RESULT_ORIGINATE_INCLUDED
#define __WIL_RESULT_ORIGINATE_INCLUDED

#include "result.h"
#include <OleAuto.h> // RestrictedErrorInfo uses BSTRs :(
#include <winstring.h>
#include "resource.h"
#include "com.h"
#include <roerrorapi.h>

namespace wil
{
    namespace details
    {
        // Note: The name must begin with "Raise" so that the !analyze auto-bucketing will ignore this stack frame.  Otherwise this line of code gets all the blame.
        inline void __stdcall RaiseRoOriginateOnWilExceptions(wil::FailureInfo const& failure) WI_NOEXCEPT
        {
            if ((failure.type == FailureType::Return) || (failure.type == FailureType::Exception))
            {
                bool shouldOriginate = true;

                wil::com_ptr_nothrow<IRestrictedErrorInfo> restrictedErrorInformation;
                if (GetRestrictedErrorInfo(&restrictedErrorInformation) == S_OK)
                {
                    // This thread already has an error origination payload.  Don't originate again if it has the same HRESULT that we are
                    // observing right now.
                    wil::unique_bstr descriptionUnused;
                    HRESULT existingHr = failure.hr;
                    wil::unique_bstr restrictedDescriptionUnused;
                    wil::unique_bstr capabilitySidUnused;
                    if (SUCCEEDED(restrictedErrorInformation->GetErrorDetails(&descriptionUnused, &existingHr, &restrictedDescriptionUnused, &capabilitySidUnused)))
                    {
                        shouldOriginate = (failure.hr != existingHr);
                    }
                }

                if (shouldOriginate)
                {
#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP | WINAPI_PARTITION_SYSTEM)
                    wil::unique_hmodule errorModule;
                    if (GetModuleHandleExW(0, L"api-ms-win-core-winrt-error-l1-1-1.dll", &errorModule))
                    {
                        auto pfn = reinterpret_cast<decltype(&::RoOriginateErrorW)>(GetProcAddress(errorModule.get(), "RoOriginateErrorW"));
                        if (pfn != nullptr)
                        {
                            pfn(failure.hr, 0, failure.pszMessage);
                        }
                    }
#else // DESKTOP | SYSTEM
                    ::RoOriginateErrorW(failure.hr, 0, failure.pszMessage);
#endif // DESKTOP | SYSTEM
                }
                else if (restrictedErrorInformation)
                {
                    // GetRestrictedErrorInfo returns ownership of the error information.  If we aren't originating, and an error was already present,
                    // then we need to restore the error information for later observation.
                    SetRestrictedErrorInfo(restrictedErrorInformation.get());
                }
            }
        }

        // This method will check for the presence of stowed exception data on the current thread.  If such data exists, and the HRESULT
        // matches the current failure, then we will call RoFailFastWithErrorContext.  RoFailFastWithErrorContext in this situation will
        // result in -VASTLY- improved crash bucketing.  It is hard to express just how much better.  In other cases we just return and
        // the calling method fails fast the same way it always has.
        inline void __stdcall FailfastWithContextCallback(wil::FailureInfo const& failure) WI_NOEXCEPT
        {
            wil::com_ptr_nothrow<IRestrictedErrorInfo> restrictedErrorInformation;
            if (GetRestrictedErrorInfo(&restrictedErrorInformation) == S_OK)
            {
                wil::unique_bstr descriptionUnused;
                HRESULT existingHr = failure.hr;
                wil::unique_bstr restrictedDescriptionUnused;
                wil::unique_bstr capabilitySidUnused;
                if (SUCCEEDED(restrictedErrorInformation->GetErrorDetails(&descriptionUnused, &existingHr, &restrictedDescriptionUnused, &capabilitySidUnused)) &&
                    (existingHr == failure.hr))
                {
                    // GetRestrictedErrorInfo returns ownership of the error information.  We want it to be available for RoFailFastWithErrorContext
                    // so we must restore it via SetRestrictedErrorInfo first.
                    SetRestrictedErrorInfo(restrictedErrorInformation.get());
                    RoFailFastWithErrorContext(existingHr);
                }
                else
                {
                    // The error didn't match the current failure.  Put it back in thread-local storage even though we aren't failing fast
                    // in this method, so it is available in the debugger just-in-case.
                    SetRestrictedErrorInfo(restrictedErrorInformation.get());
                }
            }
        }
    } // namespace details
} // namespace wil

// Automatically call RoOriginateError upon error origination by including this file
WI_HEADER_INITITALIZATION_FUNCTION(ResultStowedExceptionInitialize, []
{
    ::wil::SetOriginateErrorCallback(::wil::details::RaiseRoOriginateOnWilExceptions);
    ::wil::SetFailfastWithContextCallback(::wil::details::FailfastWithContextCallback);
    return 1;
})

#endif // __WIL_RESULT_ORIGINATE_INCLUDED
