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
#ifndef __WIL_WIN32_RESULTMACROS_INCLUDED
#define __WIL_WIN32_RESULTMACROS_INCLUDED

#include "result_macros.h"

// Helpers for return macros
#define __WIN32_RETURN_WIN32(error, str)                    __WI_SUPPRESS_4127_S do { const auto __error = (error); if (FAILED_WIN32(__error)) { __R_FN(Return_Win32)(__R_INFO(str) __error); } return __error; } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __WIN32_RETURN_GLE_FAIL(str)                        return __R_FN(Win32_Return_GetLastError)(__R_INFO_ONLY(str))

FORCEINLINE long __WIN32_FROM_HRESULT(HRESULT hr)
{
    if (SUCCEEDED(hr))
    {
        return ERROR_SUCCESS;
    }
    return HRESULT_FACILITY(hr) == FACILITY_WIN32 ? HRESULT_CODE(hr) : hr;
}

//*****************************************************************************
// Macros for returning failures as WIN32 error codes
//*****************************************************************************

// Always returns a known result (WIN32 error code) - always logs failures
#define WIN32_RETURN_WIN32(error)                             __WIN32_RETURN_WIN32(wil::verify_win32(error), #error)
#define WIN32_RETURN_LAST_ERROR()                             __WIN32_RETURN_GLE_FAIL(nullptr)

// Conditionally returns failures (WIN32 error code) - always logs failures
#define WIN32_RETURN_IF_WIN32_ERROR(error)                    __WI_SUPPRESS_4127_S do { const auto __errorRet = wil::verify_win32(error); if (FAILED_WIN32(__errorRet)) { __WIN32_RETURN_WIN32(__errorRet, #error); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define WIN32_RETURN_WIN32_IF(error, condition)               __WI_SUPPRESS_4127_S do { if (wil::verify_bool(condition)) { __WIN32_RETURN_WIN32(wil::verify_win32(error), #condition); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define WIN32_RETURN_WIN32_IF_NULL(error, ptr)                __WI_SUPPRESS_4127_S do { if ((ptr) == nullptr) { __WIN32_RETURN_WIN32(wil::verify_win32(error), #ptr); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define WIN32_RETURN_LAST_ERROR_IF(condition)                 __WI_SUPPRESS_4127_S do { if (wil::verify_bool(condition)) { __WIN32_RETURN_GLE_FAIL(#condition); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define WIN32_RETURN_LAST_ERROR_IF_NULL(ptr)                  __WI_SUPPRESS_4127_S do { if ((ptr) == nullptr) { __WIN32_RETURN_GLE_FAIL(#ptr); }} __WI_SUPPRESS_4127_E while ((void)0, 0)

// Conditionally returns failures (WIN32 error code) - use for failures that are expected in common use - failures are not logged - macros are only for control flow pattern
#define WIN32_RETURN_IF_WIN32_ERROR_EXPECTED(error)           __WI_SUPPRESS_4127_S do { const auto __errorRet = wil::verify_win32(error); if (FAILED_WIN32(__errorRet)) { return __errorRet; }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define WIN32_RETURN_WIN32_IF_EXPECTED(error, condition)      __WI_SUPPRESS_4127_S do { if (wil::verify_bool(condition)) { return wil::verify_win32(error); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define WIN32_RETURN_WIN32_IF_NULL_EXPECTED(error, ptr)       __WI_SUPPRESS_4127_S do { if ((ptr) == nullptr) { return wil::verify_win32(error); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define WIN32_RETURN_LAST_ERROR_IF_EXPECTED(condition)        __WI_SUPPRESS_4127_S do { if (wil::verify_bool(condition)) { return wil::verify_win32(wil::details::GetLastErrorFail()); }} __WI_SUPPRESS_4127_E while ((void)0, 0)
#define WIN32_RETURN_LAST_ERROR_IF_NULL_EXPECTED(ptr)         __WI_SUPPRESS_4127_S do { if ((ptr) == nullptr) { return wil::verify_win32(wil::details::GetLastErrorFail()); }} __WI_SUPPRESS_4127_E while ((void)0, 0)


//*****************************************************************************
// Macros to catch and convert exceptions on failure
//*****************************************************************************

// Use these macros *within* a catch (...) block to handle exceptions
#define WIN32_RETURN_CAUGHT_EXCEPTION()                            return __R_FN(Win32_Return_CaughtException)(__R_INFO_ONLY(nullptr))

// Use these macros in place of a catch block to handle exceptions
#define WIN32_CATCH_RETURN()                                       catch (...) { WIN32_RETURN_CAUGHT_EXCEPTION(); }

namespace wil
{
    //*****************************************************************************
    // Public Helpers that catch -- mostly only enabled when exceptions are enabled
    //*****************************************************************************

    // Win32ErrorFromCaughtException is a function that is meant to be called from within a catch(...) block. Internally
    // it re-throws and catches the exception to convert it to a WIN32 error code. If an exception is of an unrecognized type
    // the function will fail fast.
    //
    // try
    // {
    //     // Code
    // }
    // catch (...)
    // {
    //     status = wil::Win32ErrorFromCaughtException();
    // }
    _Always_(_Post_satisfies_(return > 0))
    __declspec(noinline) inline long Win32ErrorFromCaughtException() WI_NOEXCEPT
    {
        return __WIN32_FROM_HRESULT(ResultFromCaughtException());
    }

    namespace details::__R_NS_NAME
    {
#ifdef WIL_ENABLE_EXCEPTIONS
        __R_DIRECT_METHOD(long, Win32_Return_CaughtException)(__R_DIRECT_FN_PARAMS_ONLY) WI_NOEXCEPT
        {
            __R_FN_LOCALS;
            return __WIN32_FROM_HRESULT(wil::details::ReportFailure_CaughtException<FailureType::Return>(__R_DIRECT_FN_CALL_ONLY));
        }
#endif

        __R_DIRECT_METHOD(long, Win32_Return_GetLastError)(__R_DIRECT_FN_PARAMS_ONLY) WI_NOEXCEPT
        {
            __R_FN_LOCALS;
            return __WIN32_FROM_HRESULT(wil::details::ReportFailure_GetLastErrorHr<FailureType::Return>(__R_DIRECT_FN_CALL_ONLY));
        }
    }
}

#endif // __WIL_WIN32_RESULTMACROS_INCLUDED
