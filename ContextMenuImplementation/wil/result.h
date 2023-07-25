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
#ifndef __WIL_RESULT_INCLUDED
#define __WIL_RESULT_INCLUDED

// Most functionality is picked up from result_macros.h.  This file specifically provides higher level processing of errors when
// they are encountered by the underlying macros.
#include "result_macros.h"

// Note that we avoid pulling in STL's memory header from Result.h through Resource.h as we have
// Result.h customers who are still on older versions of STL (without std::shared_ptr<>).
#ifndef RESOURCE_SUPPRESS_STL
#define RESOURCE_SUPPRESS_STL
#include "resource.h"
#undef RESOURCE_SUPPRESS_STL
#else
#include "resource.h"
#endif

#ifdef WIL_KERNEL_MODE
#error This header is not supported in kernel-mode.
#endif

// The updated behavior of running init-list ctors during placement new is proper & correct, disable the warning that requests developers verify they want it
#pragma warning(push)
#pragma warning(disable : 4351)

namespace wil
{
    // WARNING: EVERYTHING in this namespace must be handled WITH CARE as the entities defined within
    //          are used as an in-proc ABI contract between binaries that utilize WIL.  Making changes
    //          that add v-tables or change the storage semantics of anything herein needs to be done
    //          with care and respect to versioning.
    ///@cond
    namespace details_abi
    {
        #define __WI_SEMAHPORE_VERSION L"_p0"

        // This class uses named semaphores to be able to stash a numeric value (including a pointer
        // for retrieval from within any module in a process).  This is a very specific need of a
        // header-based library that should not be generally used.
        //
        // Notes for use:
        // * Data members must be stable unless __WI_SEMAHPORE_VERSION is changed
        // * The class must not reference module code (v-table, function pointers, etc)
        // * Use of this class REQUIRES that there be a MUTEX held around the semaphore manipulation
        //   and tests as it doesn't attempt to handle thread contention on the semaphore while manipulating
        //   the count.
        // * This class supports storing a 31-bit number of a single semaphore or a 62-bit number across
        //   two semaphores and directly supports pointers.

        class SemaphoreValue
        {
        public:
            SemaphoreValue() = default;
            SemaphoreValue(const SemaphoreValue&) = delete;
            SemaphoreValue& operator=(const SemaphoreValue&) = delete;

            SemaphoreValue(SemaphoreValue&& other) WI_NOEXCEPT :
                m_semaphore(wistd::move(other.m_semaphore)),
                m_semaphoreHigh(wistd::move(other.m_semaphoreHigh))
            {
                static_assert(sizeof(m_semaphore) == sizeof(HANDLE), "unique_any must be a direct representation of the HANDLE to be used across module");
            }

            void Destroy()
            {
                m_semaphore.reset();
                m_semaphoreHigh.reset();
            }

            template <typename T>
            HRESULT CreateFromValue(PCWSTR name, T value)
            {
                return CreateFromValueInternal(name, (sizeof(value) > sizeof(unsigned long)), static_cast<unsigned __int64>(value));
            }

            HRESULT CreateFromPointer(PCWSTR name, void* pointer)
            {
                ULONG_PTR value = reinterpret_cast<ULONG_PTR>(pointer);
                FAIL_FAST_IMMEDIATE_IF(WI_IsAnyFlagSet(value, 0x3));
                return CreateFromValue(name, value >> 2);
            }

            template <typename T>
            static HRESULT TryGetValue(PCWSTR name, _Out_ T* value, _Out_opt_ bool *retrieved = nullptr)
            {
                *value = static_cast<T>(0);
                unsigned __int64 value64 = 0;
                __WIL_PRIVATE_RETURN_IF_FAILED(TryGetValueInternal(name, (sizeof(T) > sizeof(unsigned long)), &value64, retrieved));
                *value = static_cast<T>(value64);
                return S_OK;
            }

            static HRESULT TryGetPointer(PCWSTR name, _Outptr_result_maybenull_ void** pointer)
            {
                *pointer = nullptr;
                ULONG_PTR value = 0;
                __WIL_PRIVATE_RETURN_IF_FAILED(TryGetValue(name, &value));
                *pointer = reinterpret_cast<void*>(value << 2);
                return S_OK;
            }

        private:
            HRESULT CreateFromValueInternal(PCWSTR name, bool is64Bit, unsigned __int64 value)
            {
                WI_ASSERT(!m_semaphore && !m_semaphoreHigh);    // call Destroy first

                // This routine only supports 31 bits when semahporeHigh is not supplied or 62 bits when the value
                // is supplied.  It's a programming error to use it when either of these conditions are not true.

                FAIL_FAST_IMMEDIATE_IF((!is64Bit && WI_IsAnyFlagSet(value, 0xFFFFFFFF80000000)) ||
                    (is64Bit && WI_IsAnyFlagSet(value, 0xC000000000000000)));

                wchar_t localName[MAX_PATH];
                WI_VERIFY_SUCCEEDED(StringCchCopyW(localName, ARRAYSIZE(localName), name));
                WI_VERIFY_SUCCEEDED(StringCchCatW(localName, ARRAYSIZE(localName), __WI_SEMAHPORE_VERSION));

                const unsigned long highPart = static_cast<unsigned long>(value >> 31);
                const unsigned long lowPart = static_cast<unsigned long>(value & 0x000000007FFFFFFF);

                // We set the count of the semaphore equal to the max (the value we're storing).  The only exception to that
                // is ZERO, where you can't create a semaphore of value ZERO, where we push the max to one and use a count of ZERO.

                __WIL_PRIVATE_RETURN_IF_FAILED(m_semaphore.create(static_cast<LONG>(lowPart), static_cast<LONG>((lowPart > 0) ? lowPart : 1), localName));
                if (is64Bit)
                {
                    WI_VERIFY_SUCCEEDED(StringCchCatW(localName, ARRAYSIZE(localName), L"h"));
                    __WIL_PRIVATE_RETURN_IF_FAILED(m_semaphoreHigh.create(static_cast<LONG>(highPart), static_cast<LONG>((highPart > 0) ? highPart : 1), localName));
                }

                return S_OK;
            }

            static HRESULT GetValueFromSemaphore(HANDLE semaphore, _Out_ LONG* count)
            {
                // First we consume a single count from the semaphore.  This will work in all cases other
                // than the case where the count we've recorded is ZERO which will TIMEOUT.

                DWORD result = ::WaitForSingleObject(semaphore, 0);
                __WIL_PRIVATE_RETURN_LAST_ERROR_IF(result == WAIT_FAILED);
                __WIL_PRIVATE_RETURN_HR_IF(E_UNEXPECTED, !((result == WAIT_OBJECT_0) || (result == WAIT_TIMEOUT)));

                LONG value = 0;
                if (result == WAIT_OBJECT_0)
                {
                    // We were able to wait.  To establish our count, all we have to do is release that count
                    // back to the semaphore and observe the value that we released.

                    __WIL_PRIVATE_RETURN_IF_WIN32_BOOL_FALSE(::ReleaseSemaphore(semaphore, 1, &value));
                    value++;    // we waited first, so our actual value is one more than the old value

                    // Make sure the value is correct by validating that we have no more posts.
                    BOOL expectedFailure = ::ReleaseSemaphore(semaphore, 1, nullptr);
                    __WIL_PRIVATE_RETURN_HR_IF(E_UNEXPECTED, expectedFailure || (::GetLastError() != ERROR_TOO_MANY_POSTS));
                }
                else
                {
                    WI_ASSERT(result == WAIT_TIMEOUT);

                    // We know at this point that the value is ZERO.  We'll do some verification to ensure that
                    // this address is right by validating that we have one and only one more post that we could use.

                    LONG expected = 0;
                    __WIL_PRIVATE_RETURN_IF_WIN32_BOOL_FALSE(::ReleaseSemaphore(semaphore, 1, &expected));
                    __WIL_PRIVATE_RETURN_HR_IF(E_UNEXPECTED, expected != 0);

                    const BOOL expectedFailure = ::ReleaseSemaphore(semaphore, 1, nullptr);
                    __WIL_PRIVATE_RETURN_HR_IF(E_UNEXPECTED, expectedFailure || (::GetLastError() != ERROR_TOO_MANY_POSTS));

                    result = ::WaitForSingleObject(semaphore, 0);
                    __WIL_PRIVATE_RETURN_LAST_ERROR_IF(result == WAIT_FAILED);
                    __WIL_PRIVATE_RETURN_HR_IF(E_UNEXPECTED, result != WAIT_OBJECT_0);
                }

                *count = value;
                return S_OK;
            }

            static HRESULT TryGetValueInternal(PCWSTR name, bool is64Bit, _Out_ unsigned __int64* value, _Out_opt_ bool* retrieved)
            {
                assign_to_opt_param(retrieved, false);
                *value = 0;

                wchar_t localName[MAX_PATH];
                WI_VERIFY_SUCCEEDED(StringCchCopyW(localName, ARRAYSIZE(localName), name));
                WI_VERIFY_SUCCEEDED(StringCchCatW(localName, ARRAYSIZE(localName), __WI_SEMAHPORE_VERSION));

                wil::unique_semaphore_nothrow semaphoreLow(::OpenSemaphoreW(SEMAPHORE_ALL_ACCESS, FALSE, localName));
                if (!semaphoreLow)
                {
                    __WIL_PRIVATE_RETURN_HR_IF(S_OK, (::GetLastError() == ERROR_FILE_NOT_FOUND));
                    __WIL_PRIVATE_RETURN_LAST_ERROR();
                }

                LONG countLow = 0;
                LONG countHigh = 0;

                __WIL_PRIVATE_RETURN_IF_FAILED(GetValueFromSemaphore(semaphoreLow.get(), &countLow));

                if (is64Bit)
                {
                    WI_VERIFY_SUCCEEDED(StringCchCatW(localName, ARRAYSIZE(localName), L"h"));
                    wil::unique_semaphore_nothrow semaphoreHigh(::OpenSemaphoreW(SEMAPHORE_ALL_ACCESS, FALSE, localName));
                    __WIL_PRIVATE_RETURN_LAST_ERROR_IF_NULL(semaphoreHigh);

                    __WIL_PRIVATE_RETURN_IF_FAILED(GetValueFromSemaphore(semaphoreHigh.get(), &countHigh));
                }

                WI_ASSERT((countLow >= 0) && (countHigh >= 0));

                const unsigned __int64 newValueHigh = (static_cast<unsigned __int64>(countHigh) << 31);
                const unsigned __int64 newValueLow = static_cast<unsigned __int64>(countLow);

                assign_to_opt_param(retrieved, true);
                *value = (newValueHigh | newValueLow);
                return S_OK;
            }

            wil::unique_semaphore_nothrow m_semaphore;
            wil::unique_semaphore_nothrow m_semaphoreHigh;
        };

        template <typename T>
        class ProcessLocalStorageData
        {
        public:
            ProcessLocalStorageData(unique_mutex_nothrow&& mutex, SemaphoreValue&& value) :
                m_mutex(wistd::move(mutex)),
                m_value(wistd::move(value)),
                m_data()
            {
                static_assert(sizeof(m_mutex) == sizeof(HANDLE), "unique_any must be equivalent to the handle size to safely use across module");
            }

            T* GetData()
            {
                WI_ASSERT(m_mutex);
                return &m_data;
            }

            void Release()
            {
                if (ProcessShutdownInProgress())
                {
                    // There are no other threads to contend with.
                    m_refCount = m_refCount - 1;
                    if (m_refCount == 0)
                    {
                        m_data.ProcessShutdown();
                    }
                }
                else
                {
                    auto lock = m_mutex.acquire();
                    m_refCount = m_refCount - 1;
                    if (m_refCount == 0)
                    {
                        // We must explicitly destroy our semaphores while holding the mutex
                        m_value.Destroy();
                        lock.reset();

                        this->~ProcessLocalStorageData();
                        ::HeapFree(::GetProcessHeap(), 0, this);
                    }
                }
            }

            static HRESULT Acquire(PCSTR staticNameWithVersion, _Outptr_result_nullonfailure_ ProcessLocalStorageData<T>** data)
            {
                *data = nullptr;

                // NOTE: the '0' in SM0 below is intended as the VERSION number.  Changes to this class require
                //       that this value be revised.

                const DWORD size = static_cast<DWORD>(sizeof(ProcessLocalStorageData<T>));
                wchar_t name[MAX_PATH];
                WI_VERIFY(SUCCEEDED(StringCchPrintfW(name, ARRAYSIZE(name), L"Local\\SM0:%lu:%lu:%hs", ::GetCurrentProcessId(), size, staticNameWithVersion)));

                unique_mutex_nothrow mutex;
                mutex.reset(::CreateMutexExW(nullptr, name, 0, MUTEX_ALL_ACCESS));

                // This will fail in some environments and will be fixed with deliverable 12394134
                RETURN_LAST_ERROR_IF_EXPECTED(!mutex);
                auto lock = mutex.acquire();

                void* pointer = nullptr;
                __WIL_PRIVATE_RETURN_IF_FAILED(SemaphoreValue::TryGetPointer(name, &pointer));
                if (pointer)
                {
                    *data = reinterpret_cast<ProcessLocalStorageData<T>*>(pointer);
                    (*data)->m_refCount = (*data)->m_refCount + 1;
                }
                else
                {
                    __WIL_PRIVATE_RETURN_IF_FAILED(MakeAndInitialize(name, wistd::move(mutex), data));    // Assumes mutex handle ownership on success ('lock' will still be released)
                }

                return S_OK;
            }

        private:

            volatile long m_refCount = 1;
            unique_mutex_nothrow m_mutex;
            SemaphoreValue m_value;
            T m_data;

            static HRESULT MakeAndInitialize(PCWSTR name, unique_mutex_nothrow&& mutex, _Outptr_result_nullonfailure_ ProcessLocalStorageData<T>** data)
            {
                *data = nullptr;

                const DWORD size = static_cast<DWORD>(sizeof(ProcessLocalStorageData<T>));

                unique_process_heap_ptr<ProcessLocalStorageData<T>> dataAlloc(static_cast<ProcessLocalStorageData<T>*>(details::ProcessHeapAlloc(HEAP_ZERO_MEMORY, size)));
                __WIL_PRIVATE_RETURN_IF_NULL_ALLOC(dataAlloc);

                SemaphoreValue semaphoreValue;
                __WIL_PRIVATE_RETURN_IF_FAILED(semaphoreValue.CreateFromPointer(name, dataAlloc.get()));

                new(dataAlloc.get()) ProcessLocalStorageData<T>(wistd::move(mutex), wistd::move(semaphoreValue));
                *data = dataAlloc.release();

                return S_OK;
            }
        };

        template <typename T>
        class ProcessLocalStorage
        {
        public:
            ProcessLocalStorage(PCSTR staticNameWithVersion) WI_NOEXCEPT :
                m_staticNameWithVersion(staticNameWithVersion)
            {
            }

            ~ProcessLocalStorage() WI_NOEXCEPT
            {
                if (m_data)
                {
                    m_data->Release();
                }
            }

            T* GetShared() WI_NOEXCEPT
            {
                if (!m_data)
                {
                    ProcessLocalStorageData<T>* localTemp = nullptr;
                    if (SUCCEEDED(ProcessLocalStorageData<T>::Acquire(m_staticNameWithVersion, &localTemp)) && !m_data)
                    {
                        m_data = localTemp;
                    }
                }
                return m_data ? m_data->GetData() : nullptr;
            }

        private:
            PCSTR m_staticNameWithVersion = nullptr;
            ProcessLocalStorageData<T>* m_data = nullptr;
        };

        template <typename T>
        class ThreadLocalStorage
        {
        public:
            ThreadLocalStorage(const ThreadLocalStorage&) = delete;
            ThreadLocalStorage& operator=(const ThreadLocalStorage&) = delete;

            ThreadLocalStorage() = default;

            ~ThreadLocalStorage() WI_NOEXCEPT
            {
                for (auto &entry : m_hashArray)
                {
                    Node *pNode = entry;
                    while (pNode != nullptr)
                    {
                        auto pCurrent = pNode;
#pragma warning(push)
#pragma warning(disable:6001) // https://github.com/microsoft/wil/issues/164
                        pNode = pNode->pNext;
#pragma warning(pop)
                        pCurrent->~Node();
                        ::HeapFree(::GetProcessHeap(), 0, pCurrent);
                    }
                    entry = nullptr;
                }
            }

            // Note: Can return nullptr even when (shouldAllocate == true) upon allocation failure
            T* GetLocal(bool shouldAllocate = false) WI_NOEXCEPT
            {
                DWORD const threadId = ::GetCurrentThreadId();
                size_t const index = (threadId % ARRAYSIZE(m_hashArray));
                for (auto pNode = m_hashArray[index]; pNode != nullptr; pNode = pNode->pNext)
                {
                    if (pNode->threadId == threadId)
                    {
                        return &pNode->value;
                    }
                }

                if (shouldAllocate)
                {
                    if (auto pNewRaw = details::ProcessHeapAlloc(0, sizeof(Node)))
                    {
                        auto pNew = new (pNewRaw) Node{ threadId };

                        Node *pFirst;
                        do
                        {
                            pFirst = m_hashArray[index];
                            pNew->pNext = pFirst;
                        } while (::InterlockedCompareExchangePointer(reinterpret_cast<PVOID volatile *>(m_hashArray + index), pNew, pFirst) != pFirst);

                        return &pNew->value;
                    }
                }
                return nullptr;
            }

        private:

            struct Node
            {
                DWORD threadId = ULONG_MAX;
                Node* pNext = nullptr;
                T value{};
            };

            Node * volatile m_hashArray[10]{};
        };

        struct ThreadLocalFailureInfo
        {
            // ABI contract (carry size to facilitate additive change without re-versioning)
            unsigned short size;
            unsigned char reserved1[2];  // packing, reserved
            // When this failure was seen
            unsigned int sequenceId;

            // Information about the failure
            HRESULT hr;
            PCSTR fileName;
            unsigned short lineNumber;
            unsigned char failureType;  // FailureType
            unsigned char reserved2;    // packing, reserved
            PCSTR modulePath;
            void* returnAddress;
            void* callerReturnAddress;
            PCWSTR message;

            // The allocation (LocalAlloc) where structure strings point
            void* stringBuffer;
            size_t stringBufferSize;

            // NOTE: Externally Managed:  Must not have constructor or destructor

            void Clear()
            {
                ::HeapFree(::GetProcessHeap(), 0, stringBuffer);
                stringBuffer = nullptr;
                stringBufferSize = 0;
            }

            void Set(const FailureInfo& info, unsigned int newSequenceId)
            {
                sequenceId = newSequenceId;

                hr = info.hr;
                fileName = nullptr;
                lineNumber = static_cast<unsigned short>(info.uLineNumber);
                failureType = static_cast<unsigned char>(info.type);
                modulePath = nullptr;
                returnAddress = info.returnAddress;
                callerReturnAddress = info.callerReturnAddress;
                message = nullptr;

                size_t neededSize = details::ResultStringSize(info.pszFile) +
                    details::ResultStringSize(info.pszModule) +
                    details::ResultStringSize(info.pszMessage);

                if (!stringBuffer || (stringBufferSize < neededSize))
                {
                    auto newBuffer = details::ProcessHeapAlloc(HEAP_ZERO_MEMORY, neededSize);
                    if (newBuffer)
                    {
                        ::HeapFree(::GetProcessHeap(), 0, stringBuffer);
                        stringBuffer = newBuffer;
                        stringBufferSize = neededSize;
                    }
                }

                if (stringBuffer)
                {
                    unsigned char *pBuffer = static_cast<unsigned char *>(stringBuffer);
                    unsigned char *pBufferEnd = pBuffer + stringBufferSize;

                    pBuffer = details::WriteResultString(pBuffer, pBufferEnd, info.pszFile, &fileName);
                    pBuffer = details::WriteResultString(pBuffer, pBufferEnd, info.pszModule, &modulePath);
                    pBuffer = details::WriteResultString(pBuffer, pBufferEnd, info.pszMessage, &message);
                    ZeroMemory(pBuffer, pBufferEnd - pBuffer);
                }
            }

            void Get(FailureInfo& info) const
            {
                ::ZeroMemory(&info, sizeof(info));

                info.failureId = sequenceId;
                info.hr = hr;
                info.pszFile = fileName;
                info.uLineNumber = lineNumber;
                info.type = static_cast<FailureType>(failureType);
                info.pszModule = modulePath;
                info.returnAddress = returnAddress;
                info.callerReturnAddress = callerReturnAddress;
                info.pszMessage = message;
            }
        };

        struct ThreadLocalData
        {
            // ABI contract (carry size to facilitate additive change without re-versioning)
            unsigned short size = sizeof(ThreadLocalData);

            // Subscription information
            unsigned int threadId = 0;
            volatile long* failureSequenceId = nullptr;     // backpointer to the global ID

            // Information about thread errors
            unsigned int latestSubscribedFailureSequenceId = 0;

            // The last (N) observed errors
            ThreadLocalFailureInfo* errors = nullptr;
            unsigned short errorAllocCount = 0;
            unsigned short errorCurrentIndex = 0;

            // NOTE: Externally Managed:  Must allow ZERO init construction

            ~ThreadLocalData()
            {
                Clear();
            }

            void Clear()
            {
                for (auto& error : make_range(errors, errorAllocCount))
                {
                    error.Clear();
                }
                ::HeapFree(::GetProcessHeap(), 0, errors);
                errorAllocCount = 0;
                errorCurrentIndex = 0;
                errors = nullptr;
            }

            bool EnsureAllocated(bool create = true)
            {
                if (!errors && create)
                {
                    const unsigned short errorCount = 5;
                    errors = reinterpret_cast<ThreadLocalFailureInfo *>(details::ProcessHeapAlloc(HEAP_ZERO_MEMORY, errorCount * sizeof(ThreadLocalFailureInfo)));
                    if (errors)
                    {
                        errorAllocCount = errorCount;
                        errorCurrentIndex = 0;
                        for (auto& error : make_range(errors, errorAllocCount))
                        {
                            error.size = sizeof(ThreadLocalFailureInfo);
                        }
                    }
                }
                return (errors != nullptr);
            }

            void SetLastError(const wil::FailureInfo& info)
            {
                const bool hasListener = (latestSubscribedFailureSequenceId > 0);

                if (!EnsureAllocated(hasListener))
                {
                    // We either couldn't allocate or we haven't yet allocated and nobody
                    // was listening, so we ignore.
                    return;
                }

                if (hasListener)
                {
                    // When we have listeners, we can throw away any updates to the last seen error
                    // code within the same listening context presuming it's an update of the existing
                    // error with the same code.

                    for (auto& error : make_range(errors, errorAllocCount))
                    {
                        if ((error.sequenceId > latestSubscribedFailureSequenceId) && (error.hr == info.hr))
                        {
                            return;
                        }
                    }
                }

                // Otherwise we create a new failure...

                errorCurrentIndex = (errorCurrentIndex + 1) % errorAllocCount;
                errors[errorCurrentIndex].Set(info, ::InterlockedIncrementNoFence(failureSequenceId));
            }

            WI_NODISCARD bool GetLastError(_Inout_ wil::FailureInfo& info, unsigned int minSequenceId, HRESULT matchRequirement) const
            {
                if (!errors)
                {
                    return false;
                }

                // If the last error we saw doesn't meet the filter requirement or if the last error was never
                // set, then we couldn't return a result at all...
                auto& lastFailure = errors[errorCurrentIndex];
                if (minSequenceId >= lastFailure.sequenceId)
                {
                    return false;
                }

                // With no result filter, we just go to the last error and report it
                if (matchRequirement == S_OK)
                {
                    lastFailure.Get(info);
                    return true;
                }

                // Find the oldest result matching matchRequirement and passing minSequenceId
                ThreadLocalFailureInfo* find = nullptr;
                for (auto& error : make_range(errors, errorAllocCount))
                {
                    if ((error.hr == matchRequirement) && (error.sequenceId > minSequenceId))
                    {
                        if (!find || (error.sequenceId < find->sequenceId))
                        {
                            find = &error;
                        }
                    }
                }
                if (find)
                {
                    find->Get(info);
                    return true;
                }

                return false;
            }

            bool GetCaughtExceptionError(_Inout_ wil::FailureInfo& info, unsigned int minSequenceId, _In_opt_ const DiagnosticsInfo* diagnostics, HRESULT matchRequirement, void* returnAddress)
            {
                // First attempt to get the last error and then see if it matches the error returned from
                // the last caught exception.  If it does, then we're good to go and we return that last error.

                FailureInfo last = {};
                if (GetLastError(last, minSequenceId, matchRequirement) && (last.hr == ResultFromCaughtException()))
                {
                    info = last;
                    return true;
                }

                // The last error didn't match or we never had one... we need to create one -- we do so by logging
                // our current request and then using the last error.

                DiagnosticsInfo source;
                if (diagnostics)
                {
                    source = *diagnostics;
                }

                // NOTE:  FailureType::Log as it's only informative (no action) and SupportedExceptions::All as it's not a barrier, only recognition.
                wchar_t message[2048]{};
                message[0] = L'\0';
                const HRESULT hr = details::ReportFailure_CaughtExceptionCommon<FailureType::Log>(__R_DIAGNOSTICS_RA(source, returnAddress), message, ARRAYSIZE(message), SupportedExceptions::All).hr;

                // Now that the exception was logged, we should be able to fetch it.
                return GetLastError(info, minSequenceId, hr);
            }
        };

        struct ProcessLocalData
        {
            // ABI contract (carry size to facilitate additive change without re-versioning)
            unsigned short size = sizeof(ProcessLocalData);

            // Failure Information
            volatile long failureSequenceId = 1;    // process global variable
            ThreadLocalStorage<ThreadLocalData> threads;    // list of allocated threads

            void ProcessShutdown() {}
        };

        __declspec(selectany) ProcessLocalStorage<ProcessLocalData>* g_pProcessLocalData = nullptr;

        __declspec(noinline) inline ThreadLocalData* GetThreadLocalDataCache(bool allocate = true)
        {
            ThreadLocalData* result = nullptr;
            if (g_pProcessLocalData)
            {
                auto processData = g_pProcessLocalData->GetShared();
                if (processData)
                {
                    result = processData->threads.GetLocal(allocate);
                    if (result && !result->failureSequenceId)
                    {
                        result->failureSequenceId = &(processData->failureSequenceId);
                    }
                }
            }
            return result;
        }

        __forceinline ThreadLocalData* GetThreadLocalData(bool allocate = true)
        {
            return GetThreadLocalDataCache(allocate);
        }

    } // details_abi
    /// @endcond


    /** Returns a sequence token that can be used with wil::GetLastError to limit errors to those that occur after this token was retrieved.
    General usage pattern:  use wil::GetCurrentErrorSequenceId to cache a token, execute your code, on failure use wil::GetLastError with the token
    to provide information on the error that occurred while executing your code.  Prefer to use wil::ThreadErrorContext over this approach when
    possible.  */
    inline long GetCurrentErrorSequenceId()
    {
        auto data = details_abi::GetThreadLocalData();
        if (data)
        {
            // someone is interested -- make sure we can store errors
            data->EnsureAllocated();
            return *data->failureSequenceId;
        }

        return 0;
    }

    /** Caches failure information for later retrieval from GetLastError.
    Most people will never need to do this explicitly as failure information is automatically made available per-thread across a process when
    errors are encountered naturally through the WIL macros. */
    inline void SetLastError(const wil::FailureInfo& info)
    {
        static volatile unsigned int lastThread = 0;
        auto threadId = ::GetCurrentThreadId();
        if (lastThread != threadId)
        {
            static volatile long depth = 0;
            if (::InterlockedIncrementNoFence(&depth) < 4)
            {
                lastThread = threadId;
                auto data = details_abi::GetThreadLocalData(false);       // false = avoids allocation if not already present
                if (data)
                {
                    data->SetLastError(info);
                }
                lastThread = 0;
            }
            ::InterlockedDecrementNoFence(&depth);
        }
    }

    /** Retrieves failure information for the current thread with the given filters.
    This API can be used to retrieve information about the last WIL failure that occurred on the current thread.
    This error crosses DLL boundaries as long as the error occurred in the current process.  Passing a minSequenceId
    restricts the error returned to one that occurred after the given sequence ID.  Passing matchRequirement also filters
    the returned result to the given error code. */
    inline bool GetLastError(_Inout_ wil::FailureInfo& info, unsigned int minSequenceId = 0, HRESULT matchRequirement = S_OK)
    {
        auto data = details_abi::GetThreadLocalData(false);       // false = avoids allocation if not already present
        if (data)
        {
            return data->GetLastError(info, minSequenceId, matchRequirement);
        }
        return false;
    }

    /** Retrieves failure information when within a catch block for the current thread with the given filters.
    When unable to retrieve the exception information (when WIL hasn't yet seen it), this will attempt (best effort) to
    discover information about the exception and will attribute that information to the given DiagnosticsInfo position.
    See GetLastError for capabilities and filtering. */
    inline __declspec(noinline) bool GetCaughtExceptionError(_Inout_ wil::FailureInfo& info, unsigned int minSequenceId = 0, const DiagnosticsInfo* diagnostics = nullptr, HRESULT matchRequirement = S_OK)
    {
        auto data = details_abi::GetThreadLocalData();
        if (data)
        {
            return data->GetCaughtExceptionError(info, minSequenceId, diagnostics, matchRequirement, _ReturnAddress());
        }
        return false;
    }

    /** Use this class to manage retrieval of information about an error occurring in the requested code.
    Construction of this class sets a point in time after which you can use the GetLastError class method to retrieve
    the origination of the last error that occurred on this thread since the class was created. */
    class ThreadErrorContext
    {
    public:
        ThreadErrorContext() :
            m_data(details_abi::GetThreadLocalData())
        {
            if (m_data)
            {
                m_sequenceIdLast = m_data->latestSubscribedFailureSequenceId;
                m_sequenceIdStart = *m_data->failureSequenceId;
                m_data->latestSubscribedFailureSequenceId = m_sequenceIdStart;
            }
        }

        ~ThreadErrorContext()
        {
            if (m_data)
            {
                m_data->latestSubscribedFailureSequenceId = m_sequenceIdLast;
            }
        }

        /** Retrieves the origination of the last error that occurred since this class was constructed.
        The optional parameter allows the failure information returned to be filtered to a specific
        result. */
        inline bool GetLastError(FailureInfo& info, HRESULT matchRequirement = S_OK)
        {
            if (m_data)
            {
                return m_data->GetLastError(info, m_sequenceIdStart, matchRequirement);
            }
            return false;
        }

        /** Retrieves the origin of the current exception (within a catch block) since this class was constructed.
        See @ref GetCaughtExceptionError for more information */
        inline __declspec(noinline) bool GetCaughtExceptionError(_Inout_ wil::FailureInfo& info, const DiagnosticsInfo* diagnostics = nullptr, HRESULT matchRequirement = S_OK)
        {
            if (m_data)
            {
                return m_data->GetCaughtExceptionError(info, m_sequenceIdStart, diagnostics, matchRequirement, _ReturnAddress());
            }
            return false;
        }

    private:
        details_abi::ThreadLocalData* m_data;
        unsigned long m_sequenceIdStart{};
        unsigned long m_sequenceIdLast{};
    };


    enum class WilInitializeCommand
    {
        Create,
        Destroy,
    };


    /// @cond
    namespace details
    {
        struct IFailureCallback
        {
            virtual bool NotifyFailure(FailureInfo const &failure) WI_NOEXCEPT = 0;
        };

        class ThreadFailureCallbackHolder;

        __declspec(selectany) details_abi::ThreadLocalStorage<ThreadFailureCallbackHolder*>* g_pThreadFailureCallbacks = nullptr;

        class ThreadFailureCallbackHolder
        {
        public:
            ThreadFailureCallbackHolder(_In_opt_ IFailureCallback *pCallbackParam, _In_opt_ CallContextInfo *pCallContext = nullptr, bool watchNow = true) WI_NOEXCEPT :
                m_ppThreadList(nullptr),
                m_pCallback(pCallbackParam),
                m_pNext(nullptr),
                m_threadId(0),
                m_pCallContext(pCallContext)
            {
                if (watchNow)
                {
                    StartWatching();
                }
            }

            ThreadFailureCallbackHolder(ThreadFailureCallbackHolder &&other) WI_NOEXCEPT :
                m_ppThreadList(nullptr),
                m_pCallback(other.m_pCallback),
                m_pNext(nullptr),
                m_threadId(0),
                m_pCallContext(other.m_pCallContext)
            {
                if (other.m_threadId != 0)
                {
                    other.StopWatching();
                    StartWatching();
                }
            }

            ~ThreadFailureCallbackHolder() WI_NOEXCEPT
            {
                if (m_threadId != 0)
                {
                    StopWatching();
                }
            }

            void SetCallContext(_In_opt_ CallContextInfo *pCallContext)
            {
                m_pCallContext = pCallContext;
            }

            CallContextInfo *CallContextInfo()
            {
                return m_pCallContext;
            }

            void StartWatching()
            {
                // out-of balance Start/Stop calls?
                __FAIL_FAST_IMMEDIATE_ASSERT__(m_threadId == 0);

                m_ppThreadList = g_pThreadFailureCallbacks ? g_pThreadFailureCallbacks->GetLocal(true) : nullptr; // true = allocate thread list if missing
                if (m_ppThreadList)
                {
                    m_pNext = *m_ppThreadList;
                    *m_ppThreadList = this;
                    m_threadId = ::GetCurrentThreadId();
                }
            }

            void StopWatching()
            {
                if (m_threadId != ::GetCurrentThreadId())
                {
                    // The thread-specific failure holder cannot be stopped on a different thread than it was started on or the
                    // internal book-keeping list will be corrupted.  To fix this change the telemetry pattern in the calling code
                    // to match one of the patterns available here:
                    //    https://microsoft.sharepoint.com/teams/osg_development/Shared%20Documents/Windows%20TraceLogging%20Helpers.docx

                    WI_USAGE_ERROR("MEMORY CORRUPTION: Calling code is leaking an activity thread-watcher and releasing it on another thread");
                }

                m_threadId = 0;

                while (*m_ppThreadList != nullptr)
                {
                    if (*m_ppThreadList == this)
                    {
                        *m_ppThreadList = m_pNext;
                        break;
                    }
                    m_ppThreadList = &((*m_ppThreadList)->m_pNext);
                }
                m_ppThreadList = nullptr;
            }

            WI_NODISCARD bool IsWatching() const
            {
                return (m_threadId != 0);
            }

            void SetWatching(bool shouldWatch)
            {
                if (shouldWatch && !IsWatching())
                {
                    StartWatching();
                }
                else if (!shouldWatch && IsWatching())
                {
                    StopWatching();
                }
            }

            static bool GetThreadContext(_Inout_ FailureInfo *pFailure, _In_opt_ ThreadFailureCallbackHolder *pCallback, _Out_writes_(callContextStringLength) _Post_z_ PSTR callContextString, _Pre_satisfies_(callContextStringLength > 0) size_t callContextStringLength)
            {
                *callContextString = '\0';
                bool foundContext = false;
                if (pCallback != nullptr)
                {
                    foundContext = GetThreadContext(pFailure, pCallback->m_pNext, callContextString, callContextStringLength);

                    if (pCallback->m_pCallContext != nullptr)
                    {
                        auto &context = *pCallback->m_pCallContext;

                        // We generate the next telemetry ID only when we've found an error (avoid always incrementing)
                        if (context.contextId == 0)
                        {
                            context.contextId = ::InterlockedIncrementNoFence(&s_telemetryId);
                        }

                        if (pFailure->callContextOriginating.contextId == 0)
                        {
                            pFailure->callContextOriginating = context;
                        }

                        pFailure->callContextCurrent = context;

                        auto callContextStringEnd = callContextString + callContextStringLength;
                        callContextString += strlen(callContextString);

                        if ((callContextStringEnd - callContextString) > 2)     // room for at least the slash + null
                        {
                            *callContextString++ = '\\';
                            auto nameSizeBytes = strlen(context.contextName) + 1;
                            size_t remainingBytes = static_cast<size_t>(callContextStringEnd - callContextString);
                            auto copyBytes = (nameSizeBytes < remainingBytes) ? nameSizeBytes : remainingBytes;
                            memcpy_s(callContextString, remainingBytes, context.contextName, copyBytes);
                            *(callContextString + (copyBytes - 1)) = '\0';
                        }

                        return true;
                    }
                }
                return foundContext;
            }

            static void GetContextAndNotifyFailure(_Inout_ FailureInfo *pFailure, _Out_writes_(callContextStringLength) _Post_z_ PSTR callContextString, _Pre_satisfies_(callContextStringLength > 0) size_t callContextStringLength) WI_NOEXCEPT
            {
                *callContextString = '\0';
                bool reportedTelemetry = false;

                ThreadFailureCallbackHolder **ppListeners = g_pThreadFailureCallbacks ? g_pThreadFailureCallbacks->GetLocal() : nullptr;
                if ((ppListeners != nullptr) && (*ppListeners != nullptr))
                {
                    callContextString[0] = '\0';
                    if (GetThreadContext(pFailure, *ppListeners, callContextString, callContextStringLength))
                    {
                        pFailure->pszCallContext = callContextString;
                    }

                    auto pNode = *ppListeners;
                    do
                    {
                        reportedTelemetry |= pNode->m_pCallback->NotifyFailure(*pFailure);
                        pNode = pNode->m_pNext;
                    }
                    while (pNode != nullptr);
                }

                if (g_pfnTelemetryCallback != nullptr)
                {
                    // If the telemetry was requested to be suppressed,
                    // pretend like it has already been reported to the fallback callback
                    g_pfnTelemetryCallback(reportedTelemetry || WI_IsFlagSet(pFailure->flags, FailureFlags::RequestSuppressTelemetry), *pFailure);
                }
            }

            ThreadFailureCallbackHolder(ThreadFailureCallbackHolder const &) = delete;
            ThreadFailureCallbackHolder& operator=(ThreadFailureCallbackHolder const &) = delete;

        private:
            static long volatile s_telemetryId;

            ThreadFailureCallbackHolder **m_ppThreadList;
            IFailureCallback *m_pCallback;
            ThreadFailureCallbackHolder *m_pNext;
            DWORD m_threadId;
            wil::CallContextInfo *m_pCallContext;
        };

        __declspec(selectany) long volatile ThreadFailureCallbackHolder::s_telemetryId = 1;

        template <typename TLambda>
        class ThreadFailureCallbackFn final : public IFailureCallback
        {
        public:
            explicit ThreadFailureCallbackFn(_In_opt_ CallContextInfo *pContext, _Inout_ TLambda &&errorFunction) WI_NOEXCEPT :
                m_errorFunction(wistd::move(errorFunction)),
                m_callbackHolder(this, pContext)
            {
            }

            ThreadFailureCallbackFn(_Inout_ ThreadFailureCallbackFn && other) WI_NOEXCEPT :
                m_errorFunction(wistd::move(other.m_errorFunction)),
                m_callbackHolder(this, other.m_callbackHolder.CallContextInfo())
            {
            }

            bool NotifyFailure(FailureInfo const &failure) WI_NOEXCEPT override
            {
                return m_errorFunction(failure);
            }

        private:
            ThreadFailureCallbackFn(_In_ ThreadFailureCallbackFn const &);
            ThreadFailureCallbackFn & operator=(_In_ ThreadFailureCallbackFn const &);

            TLambda m_errorFunction;
            ThreadFailureCallbackHolder m_callbackHolder;
        };


        // returns true if telemetry was reported for this error
        inline void __stdcall GetContextAndNotifyFailure(_Inout_ FailureInfo *pFailure, _Out_writes_(callContextStringLength) _Post_z_ PSTR callContextString, _Pre_satisfies_(callContextStringLength > 0) size_t callContextStringLength) WI_NOEXCEPT
        {
            ThreadFailureCallbackHolder::GetContextAndNotifyFailure(pFailure, callContextString, callContextStringLength);

            // Update the process-wide failure cache
            wil::SetLastError(*pFailure);
        }

        template<typename T, typename... TCtorArgs> void InitGlobalWithStorage(WilInitializeCommand state, void* storage, T*& global, TCtorArgs&&... args)
        {
            if ((state == WilInitializeCommand::Create) && !global)
            {
                global = ::new (storage) T(wistd::forward<TCtorArgs>(args)...);
            }
            else if ((state == WilInitializeCommand::Destroy) && global)
            {
                global->~T();
                global = nullptr;
            }
        }
    }
    /// @endcond

    /** Modules that cannot use CRT-based static initialization may call this method from their entrypoint
        instead. Disable the use of CRT-based initializers by defining RESULT_SUPPRESS_STATIC_INITIALIZERS
        while compiling this header.  Linking together libraries that disagree on this setting and calling
        this method will behave correctly. It may be necessary to recompile all statically linked libraries
        with the RESULT_SUPPRESS_... setting to eliminate all "LNK4201 - CRT section exists, but..." errors.
    */
    inline void WilInitialize_Result(WilInitializeCommand state)
    {
        static unsigned char s_processLocalData[sizeof(*details_abi::g_pProcessLocalData)];
        static unsigned char s_threadFailureCallbacks[sizeof(*details::g_pThreadFailureCallbacks)];

        details::InitGlobalWithStorage(state, s_processLocalData, details_abi::g_pProcessLocalData, "WilError_03");
        details::InitGlobalWithStorage(state, s_threadFailureCallbacks, details::g_pThreadFailureCallbacks);

        if (state == WilInitializeCommand::Create)
        {
            details::g_pfnGetContextAndNotifyFailure = details::GetContextAndNotifyFailure;
        }
    }

    /// @cond
    namespace details
    {
#ifndef RESULT_SUPPRESS_STATIC_INITIALIZERS
        __declspec(selectany) ::wil::details_abi::ProcessLocalStorage<::wil::details_abi::ProcessLocalData> g_processLocalData("WilError_03");
        __declspec(selectany) ::wil::details_abi::ThreadLocalStorage<ThreadFailureCallbackHolder*> g_threadFailureCallbacks;

        WI_HEADER_INITITALIZATION_FUNCTION(InitializeResultHeader, []
        {
            g_pfnGetContextAndNotifyFailure = GetContextAndNotifyFailure;
            ::wil::details_abi::g_pProcessLocalData = &g_processLocalData;
            g_pThreadFailureCallbacks = &g_threadFailureCallbacks;
            return 1;
        });
#endif
    }
    /// @endcond


    // This helper functions much like scope_exit -- give it a lambda and get back a local object that can be used to
    // catch all errors happening in your module through all WIL error handling mechanisms.  The lambda will be called
    // once for each error throw, error return, or error catch that is handled while the returned object is still in
    // scope.  Usage:
    //
    // auto monitor = wil::ThreadFailureCallback([](wil::FailureInfo const &failure)
    // {
    //     // Write your code that logs or cares about failure details here...
    //     // It has access to HRESULT, filename, line number, etc through the failure param.
    // });
    //
    // As long as the returned 'monitor' object remains in scope, the lambda will continue to receive callbacks for any
    // failures that occur in this module on the calling thread.  Note that this will guarantee that the lambda will run
    // for any failure that is through any of the WIL macros (THROW_XXX, RETURN_XXX, LOG_XXX, etc).

    template <typename TLambda>
    inline wil::details::ThreadFailureCallbackFn<TLambda> ThreadFailureCallback(_Inout_ TLambda &&fnAtExit) WI_NOEXCEPT
    {
        return wil::details::ThreadFailureCallbackFn<TLambda>(nullptr, wistd::forward<TLambda>(fnAtExit));
    }


    // Much like ThreadFailureCallback, this class will receive WIL failure notifications from the time it's instantiated
    // until the time that it's destroyed.  At any point during that time you can ask for the last failure that was seen
    // by any of the WIL macros (RETURN_XXX, THROW_XXX, LOG_XXX, etc) on the current thread.
    //
    // This class is most useful when utilized as a member of an RAII class that's dedicated to providing logging or
    // telemetry.  In the destructor of that class, if the operation had not been completed successfully (it goes out of
    // scope due to early return or exception unwind before success is acknowledged) then details about the last failure
    // can be retrieved and appropriately logged.
    //
    // Usage:
    //
    // class MyLogger
    // {
    // public:
    //     MyLogger() : m_fComplete(false) {}
    //     ~MyLogger()
    //     {
    //         if (!m_fComplete)
    //         {
    //             FailureInfo *pFailure = m_cache.GetFailure();
    //             if (pFailure != nullptr)
    //             {
    //                 // Log information about pFailure (pFileure->hr, pFailure->pszFile, pFailure->uLineNumber, etc)
    //             }
    //             else
    //             {
    //                 // It's possible that you get stack unwind from an exception that did NOT come through WIL
    //                 // like (std::bad_alloc from the STL).  Use a reasonable default like:  HRESULT_FROM_WIN32(ERROR_UNHANDLED_EXCEPTION).
    //             }
    //         }
    //     }
    //     void Complete() { m_fComplete = true; }
    // private:
    //     bool m_fComplete;
    //     ThreadFailureCache m_cache;
    // };

    class ThreadFailureCache final :
        public details::IFailureCallback
    {
    public:
        ThreadFailureCache() :
            m_callbackHolder(this)
        {
        }

        ThreadFailureCache(ThreadFailureCache && rhs) WI_NOEXCEPT :
            m_failure(wistd::move(rhs.m_failure)),
            m_callbackHolder(this)
        {
        }

        ThreadFailureCache& operator=(ThreadFailureCache && rhs) WI_NOEXCEPT
        {
            m_failure = wistd::move(rhs.m_failure);
            return *this;
        }

        void WatchCurrentThread()
        {
            m_callbackHolder.StartWatching();
        }

        void IgnoreCurrentThread()
        {
            m_callbackHolder.StopWatching();
        }

        FailureInfo const* GetFailure()
        {
            return (FAILED(m_failure.GetFailureInfo().hr) ? &(m_failure.GetFailureInfo()) : nullptr);
        }

        bool NotifyFailure(FailureInfo const& failure) WI_NOEXCEPT override
        {
            // When we "cache" a failure, we bias towards trying to find the origin of the last HRESULT
            // generated, so we ignore subsequent failures on the same error code (assuming propagation).

            if (failure.hr != m_failure.GetFailureInfo().hr)
            {
                m_failure.SetFailureInfo(failure);
            }
            return false;
        }

    private:
        StoredFailureInfo m_failure;
        details::ThreadFailureCallbackHolder m_callbackHolder;
    };

} // wil

#pragma warning(pop)

#endif
