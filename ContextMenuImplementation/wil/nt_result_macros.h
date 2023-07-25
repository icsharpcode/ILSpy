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
#ifndef __WIL_NT_RESULTMACROS_INCLUDED
#define __WIL_NT_RESULTMACROS_INCLUDED

#include "result_macros.h"

// Helpers for return macros
#define __NT_RETURN_NTSTATUS(status, str)                    __WI_SUPPRESS_4127_S do { NTSTATUS __status = (status); if (FAILED_NTSTATUS(__status)) { __R_FN(Return_NtStatus)(__R_INFO(str) __status); } return __status; } __WI_SUPPRESS_4127_E while ((void)0, 0)
#define __NT_RETURN_NTSTATUS_MSG(status, str, fmt, ...)      __WI_SUPPRESS_4127_S do { NTSTATUS __status = (status); if (FAILED_NTSTATUS(__status)) { __R_FN(Return_NtStatusMsg)(__R_INFO(str) __status, fmt, ##__VA_ARGS__); } return __status; } __WI_SUPPRESS_4127_E while ((void)0, 0)

//*****************************************************************************
// Macros for returning failures as NTSTATUS
//*****************************************************************************

// Always returns a known result (NTSTATUS) - always logs failures
#define NT_RETURN_NTSTATUS(status)                              __NT_RETURN_NTSTATUS(wil::verify_ntstatus(status), #status)

// Always returns a known failure (NTSTATUS) - always logs a var-arg message on failure
#define NT_RETURN_NTSTATUS_MSG(status, fmt, ...)                __NT_RETURN_NTSTATUS_MSG(wil::verify_ntstatus(status), #status, fmt, ##__VA_ARGS__)

// Conditionally returns failures (NTSTATUS) - always logs failures
#define NT_RETURN_IF_NTSTATUS_FAILED(status)                    __WI_SUPPRESS_4127_S do { const auto __statusRet = wil::verify_ntstatus(status); if (FAILED_NTSTATUS(__statusRet)) { __NT_RETURN_NTSTATUS(__statusRet, #status); }} __WI_SUPPRESS_4127_E while ((void)0, 0)

// Conditionally returns failures (NTSTATUS) - always logs a var-arg message on failure
#define NT_RETURN_IF_NTSTATUS_FAILED_MSG(status, fmt, ...)      __WI_SUPPRESS_4127_S do { const auto __statusRet = wil::verify_ntstatus(status); if (FAILED_NTSTATUS(__statusRet)) { __NT_RETURN_NTSTATUS_MSG(__statusRet, #status, fmt, ##__VA_ARGS__); }} __WI_SUPPRESS_4127_E while((void)0, 0)

//*****************************************************************************
// Macros to catch and convert exceptions on failure
//*****************************************************************************

// Use these macros *within* a catch (...) block to handle exceptions
#define NT_RETURN_CAUGHT_EXCEPTION()                            return __R_FN(Nt_Return_CaughtException)(__R_INFO_ONLY(nullptr))
#define NT_RETURN_CAUGHT_EXCEPTION_MSG(fmt, ...)                return __R_FN(Nt_Return_CaughtExceptionMsg)(__R_INFO(nullptr) fmt, ##__VA_ARGS__)

// Use these macros in place of a catch block to handle exceptions
#define NT_CATCH_RETURN()                                       catch (...) { NT_RETURN_CAUGHT_EXCEPTION(); }
#define NT_CATCH_RETURN_MSG(fmt, ...)                           catch (...) { NT_RETURN_CAUGHT_EXCEPTION_MSG(fmt, ##__VA_ARGS__); }


namespace wil
{
    //*****************************************************************************
    // Public Helpers that catch -- mostly only enabled when exceptions are enabled
    //*****************************************************************************

    // StatusFromCaughtException is a function that is meant to be called from within a catch(...) block.  Internally
    // it re-throws and catches the exception to convert it to an NTSTATUS.  If an exception is of an unrecognized type
    // the function will fail fast.
    //
    // try
    // {
    //     // Code
    // }
    // catch (...)
    // {
    //     status = wil::StatusFromCaughtException();
    // }
    _Always_(_Post_satisfies_(return < 0))
    __declspec(noinline) inline NTSTATUS StatusFromCaughtException() WI_NOEXCEPT
    {
        bool isNormalized = false;
        NTSTATUS status = STATUS_SUCCESS;
        if (details::g_pfnResultFromCaughtExceptionInternal)
        {
            status = details::g_pfnResultFromCaughtExceptionInternal(nullptr, 0, &isNormalized).status;
        }
        if (FAILED_NTSTATUS(status))
        {
            return status;
        }

        // Caller bug: an unknown exception was thrown
        __WIL_PRIVATE_FAIL_FAST_HR_IF(__HRESULT_FROM_WIN32(ERROR_UNHANDLED_EXCEPTION), g_fResultFailFastUnknownExceptions);
        return wil::details::HrToNtStatus(__HRESULT_FROM_WIN32(ERROR_UNHANDLED_EXCEPTION));
    }

    namespace details
    {
        template<FailureType>
        __declspec(noinline) inline NTSTATUS ReportStatus_CaughtException(__R_FN_PARAMS_FULL, SupportedExceptions supported = SupportedExceptions::Default);
        template<FailureType>
        __declspec(noinline) inline NTSTATUS ReportStatus_CaughtExceptionMsg(__R_FN_PARAMS_FULL, _Printf_format_string_ PCSTR formatString, va_list argList);

        namespace __R_NS_NAME
        {
#ifdef WIL_ENABLE_EXCEPTIONS
            __R_DIRECT_METHOD(NTSTATUS, Nt_Return_CaughtException)(__R_DIRECT_FN_PARAMS_ONLY) WI_NOEXCEPT
            {
                __R_FN_LOCALS;
                return wil::details::ReportStatus_CaughtException<FailureType::Return>(__R_DIRECT_FN_CALL_ONLY);
            }

            __R_DIRECT_METHOD(NTSTATUS, Nt_Return_CaughtExceptionMsg)(__R_DIRECT_FN_PARAMS _Printf_format_string_ PCSTR formatString, ...) WI_NOEXCEPT
            {
                va_list argList;
                va_start(argList, formatString);
                __R_FN_LOCALS;
                return wil::details::ReportStatus_CaughtExceptionMsg<FailureType::Return>(__R_DIRECT_FN_CALL formatString, argList);
            }
#endif
        }

        template<FailureType T>
        __declspec(noinline) inline NTSTATUS ReportStatus_CaughtException(__R_FN_PARAMS_FULL, SupportedExceptions supported)
        {
            wchar_t message[2048];
            message[0] = L'\0';
            return ReportFailure_CaughtExceptionCommon<T>(__R_FN_CALL_FULL, message, ARRAYSIZE(message), supported).status;
        }

        template<>
        __declspec(noinline) inline NTSTATUS ReportStatus_CaughtException<FailureType::FailFast>(__R_FN_PARAMS_FULL, SupportedExceptions supported)
        {
            wchar_t message[2048];
            message[0] = L'\0';
            RESULT_NORETURN_RESULT(ReportFailure_CaughtExceptionCommon<FailureType::FailFast>(__R_FN_CALL_FULL, message, ARRAYSIZE(message), supported).status);
        }

        template<>
        __declspec(noinline) inline NTSTATUS ReportStatus_CaughtException<FailureType::Exception>(__R_FN_PARAMS_FULL, SupportedExceptions supported)
        {
            wchar_t message[2048];
            message[0] = L'\0';
            RESULT_NORETURN_RESULT(ReportFailure_CaughtExceptionCommon<FailureType::Exception>(__R_FN_CALL_FULL, message, ARRAYSIZE(message), supported).status);
        }

        template<FailureType T>
        __declspec(noinline) inline NTSTATUS ReportStatus_CaughtExceptionMsg(__R_FN_PARAMS_FULL, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            // Pre-populate the buffer with our message, the exception message will be added to it...
            wchar_t message[2048];
            PrintLoggingMessage(message, ARRAYSIZE(message), formatString, argList);
            StringCchCatW(message, ARRAYSIZE(message), L" -- ");
            return ReportFailure_CaughtExceptionCommon<T>(__R_FN_CALL_FULL, message, ARRAYSIZE(message), SupportedExceptions::Default).status;
        }

        template<>
        __declspec(noinline) inline NTSTATUS ReportStatus_CaughtExceptionMsg<FailureType::FailFast>(__R_FN_PARAMS_FULL, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            // Pre-populate the buffer with our message, the exception message will be added to it...
            wchar_t message[2048];
            PrintLoggingMessage(message, ARRAYSIZE(message), formatString, argList);
            StringCchCatW(message, ARRAYSIZE(message), L" -- ");
            RESULT_NORETURN_RESULT(ReportFailure_CaughtExceptionCommon<FailureType::FailFast>(__R_FN_CALL_FULL, message, ARRAYSIZE(message), SupportedExceptions::Default).status);
        }

        template<>
        __declspec(noinline) inline NTSTATUS ReportStatus_CaughtExceptionMsg<FailureType::Exception>(__R_FN_PARAMS_FULL, _Printf_format_string_ PCSTR formatString, va_list argList)
        {
            // Pre-populate the buffer with our message, the exception message will be added to it...
            wchar_t message[2048];
            PrintLoggingMessage(message, ARRAYSIZE(message), formatString, argList);
            StringCchCatW(message, ARRAYSIZE(message), L" -- ");
            RESULT_NORETURN_RESULT(ReportFailure_CaughtExceptionCommon<FailureType::Exception>(__R_FN_CALL_FULL, message, ARRAYSIZE(message), SupportedExceptions::Default).status);
        }
    }
}

#endif // __WIL_NT_RESULTMACROS_INCLUDED
