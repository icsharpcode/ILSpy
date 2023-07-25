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

#ifndef __WIL_CPPWINRT_HELPERS_DEFINED
#define __WIL_CPPWINRT_HELPERS_DEFINED

/// @cond
namespace wil::details
{
    struct dispatcher_RunAsync
    {
        template<typename Dispatcher, typename... Args>
        static void Schedule(Dispatcher const& dispatcher, Args&&... args)
        {
            dispatcher.RunAsync(std::forward<Args>(args)...);
        }
    };

    struct dispatcher_TryEnqueue
    {
        template<typename Dispatcher, typename... Args>
        static void Schedule(Dispatcher const& dispatcher, Args&&... args)
        {
            dispatcher.TryEnqueue(std::forward<Args>(args)...);
        }
    };

    template<typename Dispatcher> struct dispatcher_traits;
}

#if defined(_RESUMABLE_FUNCTIONS_SUPPORTED)
#include <experimental/coroutine>
namespace wil::details
{
    template<typename T = void> using coroutine_handle = std::experimental::coroutine_handle<T>;
}
#elif defined(__cpp_lib_coroutine) && (__cpp_lib_coroutine >= 201902L)
#include <coroutine>
namespace wil::details
{
    template<typename T = void> using coroutine_handle = std::coroutine_handle<T>;
}
#endif
/// @endcond

#if defined(_RESUMABLE_FUNCTIONS_SUPPORTED) || (defined(__cpp_lib_coroutine) && (__cpp_lib_coroutine >= 201902L))
/// @cond
namespace wil::details
{
    struct dispatched_handler_state
    {
        details::coroutine_handle<> handle{};
        bool orphaned = false;
    };

    struct dispatcher_handler
    {
        dispatcher_handler(dispatched_handler_state* state) : m_state(state) { }
        dispatcher_handler(dispatcher_handler&& other) noexcept : m_state(std::exchange(other.m_state, {})) {}

        ~dispatcher_handler()
        {
            if (m_state && m_state->handle)
            {
                m_state->orphaned = true;
                Complete();
            }
        }
        void operator()()
        {
            Complete();
        }

        void Complete()
        {
            auto state = std::exchange(m_state, nullptr);
            std::exchange(state->handle, {}).resume();
        }

        dispatched_handler_state* m_state;
    };
}
/// @endcond

namespace wil
{
    //! Resumes coroutine execution on the thread associated with the dispatcher, or throws
    //! an exception (from an arbitrary thread) if unable. Supported dispatchers are
    //! Windows.System.DispatcherQueue, Microsoft.System.DispatcherQueue,
    //! Microsoft.UI.Dispatching.DispatcherQueue, and Windows.UI.Core.CoreDispatcher,
    //! but you must include the corresponding <winrt/Namespace.h> header before including
    //! wil\cppwinrt_helpers.h. It is okay to include wil\cppwinrt_helpers.h multiple times:
    //! support will be enabled for any winrt/Namespace.h headers that were included since
    //! the previous inclusion of wil\cppwinrt_headers.h.
    template<typename Dispatcher>
    [[nodiscard]] auto resume_foreground(Dispatcher const& dispatcher,
        typename details::dispatcher_traits<Dispatcher>::Priority priority = details::dispatcher_traits<Dispatcher>::Priority::Normal)
    {
        using Traits = details::dispatcher_traits<Dispatcher>;
        using Priority = typename Traits::Priority;
        using Handler = typename Traits::Handler;

        struct awaitable
        {
            awaitable(Dispatcher const& dispatcher, Priority priority) noexcept :
                m_dispatcher(dispatcher),
                m_priority(priority)
            {
            }
            bool await_ready() const noexcept { return false; }

            void await_suspend(details::coroutine_handle<> handle)
            {
                m_state.handle = handle;
                Handler handler{ details::dispatcher_handler(&m_state) };
                try
                {
                    // The return value of Schedule is not reliable. Use the dispatcher_handler destructor
                    // to detect whether the work item failed to run.
                    Traits::Scheduler::Schedule(m_dispatcher, m_priority, handler);
                }
                catch (...)
                {
                    m_state.handle = nullptr; // the exception will resume the coroutine, so the handler shouldn't do it
                    throw;
                }
            }

            void await_resume() const
            {
                if (m_state.orphaned)
                {
                    throw winrt::hresult_error(static_cast<winrt::hresult>(0x800701ab)); // HRESULT_FROM_WIN32(ERROR_NO_TASK_QUEUE)
                }
            }

        private:
            Dispatcher const& m_dispatcher;
            Priority const m_priority;
            details::dispatched_handler_state m_state;
        };
        return awaitable{ dispatcher, priority };
    }
}
#endif // Coroutines are supported

#endif // __WIL_CPPWINRT_HELPERS_DEFINED

/// @cond
#if defined(WINRT_Windows_UI_Core_H) && !defined(__WIL_CPPWINRT_WINDOWS_UI_CORE_HELPERS)
#define __WIL_CPPWINRT_WINDOWS_UI_CORE_HELPERS
namespace wil::details
{
    template<>
    struct dispatcher_traits<winrt::Windows::UI::Core::CoreDispatcher>
    {
        using Priority = winrt::Windows::UI::Core::CoreDispatcherPriority;
        using Handler = winrt::Windows::UI::Core::DispatchedHandler;
        using Scheduler = dispatcher_RunAsync;
    };
}
#endif // __WIL_CPPWINRT_WINDOWS_UI_CORE_HELPERS

#if defined(WINRT_Windows_System_H) && !defined(__WIL_CPPWINRT_WINDOWS_SYSTEM_HELPERS)
#define __WIL_CPPWINRT_WINDOWS_SYSTEM_HELPERS
namespace wil::details
{
    template<>
    struct dispatcher_traits<winrt::Windows::System::DispatcherQueue>
    {
        using Priority = winrt::Windows::System::DispatcherQueuePriority;
        using Handler = winrt::Windows::System::DispatcherQueueHandler;
        using Scheduler = dispatcher_TryEnqueue;
    };
}
#endif // __WIL_CPPWINRT_WINDOWS_SYSTEM_HELPERS

#if defined(WINRT_Microsoft_System_H) && !defined(__WIL_CPPWINRT_MICROSOFT_SYSTEM_HELPERS)
#define __WIL_CPPWINRT_MICROSOFT_SYSTEM_HELPERS
namespace wil::details
{
    template<>
    struct dispatcher_traits<winrt::Microsoft::System::DispatcherQueue>
    {
        using Priority = winrt::Microsoft::System::DispatcherQueuePriority;
        using Handler = winrt::Microsoft::System::DispatcherQueueHandler;
        using Scheduler = dispatcher_TryEnqueue;
    };
}
#endif // __WIL_CPPWINRT_MICROSOFT_SYSTEM_HELPERS

#if defined(WINRT_Microsoft_UI_Dispatching_H) && !defined(__WIL_CPPWINRT_MICROSOFT_UI_DISPATCHING_HELPERS)
#define __WIL_CPPWINRT_MICROSOFT_UI_DISPATCHING_HELPERS
namespace wil::details
{
    template<>
    struct dispatcher_traits<winrt::Microsoft::UI::Dispatching::DispatcherQueue>
    {
        using Priority = winrt::Microsoft::UI::Dispatching::DispatcherQueuePriority;
        using Handler = winrt::Microsoft::UI::Dispatching::DispatcherQueueHandler;
        using Scheduler = dispatcher_TryEnqueue;
    };
}
#endif // __WIL_CPPWINRT_MICROSOFT_UI_DISPATCHING_HELPERS
/// @endcond

#if defined(WINRT_Windows_Foundation_Collections_H) && !defined(__WIL_CPPWINRT_WINDOWS_FOUNDATION_COLLECTION_HELPERS)
#define __WIL_CPPWINRT_WINDOWS_FOUNDATION_COLLECTION_HELPERS
namespace wil
{
    /// @cond
    namespace details
    {
        template<typename T> struct is_winrt_vector_like {
        private:
            template <typename U,
                typename = decltype(std::declval<U>().GetMany(std::declval<U>().Size(),
                    winrt::array_view<decltype(std::declval<U>().GetAt(0))>{}))>
            static constexpr bool get_value(int) { return true; }
            template <typename> static constexpr bool get_value(...) { return false; }
        public:
            static constexpr bool value = get_value<T>(0);
        };

        template<typename T> struct is_winrt_iterator_like {
        private:
            template <typename U,
                typename = decltype(std::declval<U>().GetMany(winrt::array_view<decltype(std::declval<U>().Current())>{}))>
            static constexpr bool get_value(int) { return true; }
            template <typename> static constexpr bool get_value(...) { return false; }
        public:
            static constexpr bool value = get_value<T>(0);
        };

        template<typename T> constexpr T empty() noexcept
        {
            if constexpr (std::is_base_of_v<winrt::Windows::Foundation::IUnknown, T>)
            {
                return nullptr;
            }
            else
            {
                return {};
            }
        }
    }
    /// @endcond

    /** Converts C++ / WinRT vectors, iterators, and iterables to std::vector by requesting the
    collection's data in bulk. This can be more efficient in terms of IPC cost than iteratively
    processing the collection.
    ~~~
    winrt::IVector<winrt::hstring> collection = GetCollection();
    std::vector<winrt::hstring> allData = wil::to_vector(collection); // read all data from collection
    for (winrt::hstring const& item : allData)
    {
        // use item
    }
    ~~~
    Can be used for IVector<T>, IVectorView<T>, IIterable<T>, IIterator<T>, and any type or
    interface that C++/WinRT projects those interfaces for (PropertySet, IMap<T,K>, etc.)
    Iterable-only types fetch content in units of 64. When used with an iterator, the returned
    vector contains the iterator's current position and any others after it.
    */
    template<typename TSrc> auto to_vector(TSrc const& src)
    {
        if constexpr (details::is_winrt_vector_like<TSrc>::value)
        {
            using T = decltype(src.GetAt(0));
            std::vector<T> result;
            if (auto expected = src.Size())
            {
                result.resize(expected + 1, details::empty<T>());
                auto actual = src.GetMany(0, result);
                if (actual > expected)
                {
                    throw winrt::hresult_changed_state();
                }
                result.resize(actual, details::empty<T>());
            }
            return result;
        }
        else if constexpr (details::is_winrt_iterator_like<TSrc>::value)
        {
            using T = decltype(src.Current());
            std::vector<T> result;
            constexpr uint32_t chunkSize = 64;
            while (true)
            {
                auto const lastSize = result.size();
                result.resize(lastSize + chunkSize, details::empty<T>());
                auto fetched = src.GetMany({result.data() + lastSize, result.data() + lastSize + chunkSize });
                if (fetched < chunkSize)
                {
                    result.resize(lastSize + fetched, details::empty<T>());
                    break;
                }
            }
            return result;
        }
        else
        {
            return to_vector(src.First());
        }
    }
}
#endif

#if defined(WINRT_Windows_UI_H) && defined(_WINDOWS_UI_INTEROP_H_) && !defined(__WIL_CPPWINRT_WINDOWS_UI_INTEROP_HELPERS)
#define __WIL_CPPWINRT_WINDOWS_UI_INTEROP_HELPERS
#if !defined(____x_ABI_CWindows_CFoundation_CIClosable_FWD_DEFINED__) && !defined(MIDL_NS_PREFIX)
#pragma push_macro("ABI")
#undef ABI
#define ABI
#endif

namespace wil
{
#if defined(NTDDI_VERSION) && (NTDDI_VERSION >= NTDDI_WIN10_CU)
    //! The following methods require that you include both <winrt/Windows.UI.h>
    //! <Windows.UI.Interop.h> before including wil/cppwinrt_helpers.h, and that NTDDI_VERSION
    //! is at least NTDDI_WIN10_CU. It is okay to include wil\cppwinrt_helpers.h multiple times:
    //! support will be enabled for any headers that were included since the previous inclusion
    //! of wil\cppwinrt_headers.h.
    inline winrt::Windows::UI::WindowId GetWindowIdFromWindow(HWND hwnd)
    {
        ABI::Windows::UI::WindowId abiWindowId;
        winrt::check_hresult(::GetWindowIdFromWindow(hwnd, &abiWindowId));
        return winrt::Windows::UI::WindowId{ abiWindowId.Value };
    }

    inline HWND GetWindowFromWindowId(winrt::Windows::UI::WindowId windowId)
    {
        HWND hwnd;
        winrt::check_hresult(::GetWindowFromWindowId({ windowId.Value }, &hwnd));
        return hwnd;
    }
#endif /*defined(NTDDI_VERSION) && (NTDDI_VERSION >= NTDDI_WIN10_CU)*/
}

#if !defined(____x_ABI_CWindows_CFoundation_CIClosable_FWD_DEFINED__) && !defined(MIDL_NS_PREFIX)
#pragma pop_macro("ABI")
#endif
#endif // __WIL_CPPWINRT_WINDOWS_UI_INTEROP_HELPERS
