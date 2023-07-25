#ifndef __WIL_COROUTINE_INCLUDED
#define __WIL_COROUTINE_INCLUDED

   /*
    * A wil::task<T> / com_task<T> is a coroutine with the following characteristics:
    *
    * - T must be a copyable object, movable object, reference, or void.
    * - The coroutine may be awaited at most once. The second await will crash.
    * - The coroutine may be abandoned (allowed to destruct without co_await),
    *   in which case unobserved exceptions are fatal.
    * - By default, wil::task resumes on an arbitrary thread.
    * - By default, wil::com_task resumes in the same COM apartment.
    * - task.resume_any_thread() allows resumption on any thread.
    * - task.resume_same_apartment() forces resumption in the same COM apartment.
    *
    * The wil::task and wil::com_task are intended to supplement PPL and C++/WinRT,
    * not to replace them. It provides coroutine implementations for scenarios that PPL
    * and C++/WinRT do not support, but it does not support everything that PPL and
    * C++/WinRT do.
    *
    * The implementation is optimized on the assumption that the coroutine is
    * awaited only once, and that the coroutine is discarded after completion.
    * To ensure proper usage, the task object is move-only, and
    * co_await takes ownership of the task. See further discussion below.
    *
    * Comparison with PPL and C++/WinRT:
    *
    * |                                                     | PPL       | C++/WinRT | wil::*task    |
    * |-----------------------------------------------------|-----------|-----------|---------------|
    * | T can be non-constructible                          | No        | Yes       | Yes           |
    * | T can be void                                       | Yes       | Yes       | Yes           |
    * | T can be reference                                  | No        | No        | Yes           |
    * | T can be WinRT object                               | Yes       | Yes       | Yes           |
    * | T can be non-WinRT object                           | Yes       | No        | Yes           |
    * | T can be move-only                                  | No        | No        | Yes           |
    * | Coroutine can be cancelled                          | Yes       | Yes       | No            |
    * | Coroutine can throw arbitrary exceptions            | Yes       | No        | Yes           |
    * | Can co_await more than once                         | Yes       | No        | No            |
    * | Can have multiple clients waiting for completion    | Yes       | No        | No            |
    * | co_await resumes in same COM context                | Sometimes | Yes       | You choose [1]|
    * | Can force co_await to resume in same context        | Yes       | N/A       | Yes [1]       |
    * | Can force co_await to resume in any thread          | Yes       | No        | Yes           |
    * | Can change coroutine's resumption model             | No        | No        | Yes           |
    * | Can wait synchronously                              | Yes       | Yes       | Yes [2]       |
    * | Can be consumed by non-C++ languages                | No        | Yes       | No            |
    * | Implementation is small and efficient               | No        | Yes       | Yes           |
    * | Can abandon coroutine (fail to co_await)            | Yes       | Yes       | Yes           |
    * | Exception in abandoned coroutine                    | Crash     | Ignored   | Crash         |
    * | Coroutine starts automatically                      | Yes       | Yes       | Yes           |
    * | Coroutine starts synchronously                      | No        | Yes       | Yes           |
    * | Integrates with C++/WinRT coroutine callouts        | No        | Yes       | No            |
    * 
    * [1] Resumption in the same COM apartment requires that you include COM headers.
    * [2] Synchronous waiting requires that you include <synchapi.h> (usually via <windows.h>).
    *
    * You can include the COM headers and/or synchapi.h headers, and then
    * re-include this header file to activate the features dependent upon
    * those headers.
    *
    * Examples:
    *
    * Implement a coroutine that returns a move-only non-WinRT type
    * and which resumes on an arbitrary thread.
    *
    * wil::task<wil::unique_cotaskmem_string> GetNameAsync()
    * {
    *     co_await resume_background(); // do work on BG thread
    *     wil::unique_cotaskmem_string name;
    *     THROW_IF_FAILED(GetNameSlow(&name));
    *     co_return name; // awaiter will resume on arbitrary thread
    * }
    *
    * Consumers:
    *
    * winrt::IAsyncAction UpdateNameAsync()
    * {
    *     // wil::task resumes on an arbitrary thread.
    *     auto name = co_await GetNameAsync();
    *     // could be on any thread now
    *     co_await SendNameAsync(name.get());
    * }
    *
    * winrt::IAsyncAction UpdateNameAsync()
    * {
    *     // override default behavior of wil::task and
    *     // force it to resume in the same COM apartment.
    *     auto name = co_await GetNameAsync().resume_same_apartment();
    *     // so we are still on the UI thread
    *     NameElement().Text(winrt::hstring(name.get()));
    * }
    *
    * Conversely, a coroutine that returns a
    * wil::com_task<T> defaults to resuming in the same
    * COM apartment, but you can allow it to resume on any thread
    * by doing co_await GetNameAsync().resume_any_thread().
    *
    * There is no harm in doing resume_same_apartment() / resume_any_thread() for a
    * task that already defaults to resuming in that manner. In fact, awaiting the
    * task directly is just a shorthand for awaiting the corresponding
    * resume_whatever() method.
    *
    * Alternatively, you can just convert between wil::task<T> and wil::com_task<T>
    * to change the default resumption context.
    *
    * co_await wil::com_task(GetNameAsync()); // now defaults to resume_same_apartment();
    *
    * You can store the task in a variable, but since it is a move-only
    * object, you will have to use std::move in order to transfer ownership out of
    * an lvalue.
    *
    * winrt::IAsyncAction SomethingAsync()
    * {
    *     wil::com_task<int> task;
    *     switch (source)
    *     {
    *     // Some of these might return wil::task<int>,
    *     // but assigning to a wil::com_task<int> will make
    *     // the task resume in the same COM apartment.
    *     case widget: task = GetValueFromWidgetAsync(); break;
    *     case gadget: task = GetValueFromGadgetAsync(); break;
    *     case doodad: task = GetValueFromDoodadAsync(); break;
    *     default:     FAIL_FAST(); // unknown source
    *     }
    *     auto value = co_await std::move(task); // **** need std::move
    *     DoSomethingWith(value);
    * }
    *
    * You can wait synchronously by calling get(). The usual caveats
    * about synchronous waits on STA threads apply.
    *
    * auto value = GetValueFromWidgetAsync().get();
    * 
    * auto task = GetValueFromWidgetAsync();
    * auto value = std::move(task).get(); // **** need std::move
    */

// Detect which version of the coroutine standard we have.
#if defined(_RESUMABLE_FUNCTIONS_SUPPORTED)
#include <experimental/coroutine>
#define __WI_COROUTINE_NAMESPACE ::std::experimental
#elif defined(__cpp_lib_coroutine)
#include <coroutine>
#define __WI_COROUTINE_NAMESPACE ::std
#else
#error You must compile with C++20 coroutine support to use coroutine.h.
#endif
#include <atomic>
#include <exception>
#include <wil/wistd_memory.h>
#include <wil/wistd_type_traits.h>
#include <wil/result_macros.h>

namespace wil
{
    // There are three general categories of T that you can
    // use with a task. We give them these names:
    //
    // T = void ("void category")
    // T = some kind of reference ("reference category")
    // T = non-void non-reference ("object category")
    //
    // Take care that the implementation supports all three categories.
    //
    // There is a sub-category of object category for move-only types.
    // We designed our task to be co_awaitable only once, so that
    // it can contain a move-only type. Any transfer of T as an
    // object category must be done as an rvalue reference.
    template<typename T>
    struct task;

    template<typename T>
    struct com_task;
}

namespace wil::details::coro
{
    template<typename T>
    struct task_promise;

    // Unions may not contain references, C++/CX types, or void.
    // To work around that, we put everything inside a result_wrapper
    // struct, and put the struct in the union. For void,
    // we create a special empty structure.
    //
    // get_value returns rvalue reference to T for object
    // category, or just T itself for void and reference
    // category.
    //
    // We take advantage of the reference collapsing rules
    // so that T&& = T if T is reference category.

    template<typename T>
    struct result_wrapper
    {
        T value;
        T get_value() { return wistd::forward<T>(value); }
    };

    template<>
    struct result_wrapper<void>
    {
        void get_value() { }
    };


    // The result_holder is basically a
    // std::variant<std::monotype, T, std::exception_ptr>
    // but with these extra quirks:
    // * The only valid transition is monotype -> something-else.
    //   Consequently, it does not have valueless_by_exception.

    template<typename T>
    struct result_holder
    {
        // The content of the result_holder
        // depends on the result_status:
        //
        // empty: No active member.
        // value: Active member is wrap.
        // error: Active member is error.
        enum class result_status { empty, value, error };

        result_status status{ result_status::empty };
        union variant
        {
            variant() {}
            ~variant() {}
            result_wrapper<T> wrap;
            std::exception_ptr error;
        } result;

        // emplace_value will be called with
        //
        // * no parameters (void category)
        // * The reference type T (reference category)
        // * Some kind of reference to T (object category)
        //
        // Set the status after constructing the object.
        // That way, if object construction throws an exception,
        // the holder remains empty.
        template<typename...Args>
        void emplace_value(Args&&... args)
        {
            WI_ASSERT(status == result_status::empty);
            new (wistd::addressof(result.wrap)) result_wrapper<T>{ wistd::forward<Args>(args)... };
            status = result_status::value;
        }

        void unhandled_exception() noexcept
        {
            WI_ASSERT(status == result_status::empty);
            new (wistd::addressof(result.error)) std::exception_ptr(std::current_exception());
            status = result_status::error;
        }

        T get_value()
        {
            if (status == result_status::value)
            {
                return result.wrap.get_value();
            }
            WI_ASSERT(status == result_status::error);
            std::rethrow_exception(wistd::exchange(result.error, {}));
        }

        result_holder() = default;
        result_holder(result_holder const&) = delete;
        void operator=(result_holder const&) = delete;

        ~result_holder() noexcept(false)
        {
            switch (status)
            {
            case result_status::value:
                result.wrap.~result_wrapper();
                break;
            case result_status::error:
                // Rethrow unobserved exception. Delete this line to
                // discard unobserved exceptions.
                if (result.error) std::rethrow_exception(result.error);
                result.error.~exception_ptr();
            }
        }
    };

    // Most of the work is done in the promise_base,
    // It is a CRTP-like base class for task_promise<void> and
    // task_promise<non-void> because the language forbids
    // a single promise from containing both return_value and
    // return_void methods (even if one of them is deleted by SFINAE).
    template<typename T>
    struct promise_base
    {
        // The coroutine state remains alive as long as the coroutine is
        // still running (hasn't reached final_suspend) or the associated
        // task has not yet abandoned the coroutine (either finished awaiting
        // or destructed without awaiting).
        //
        // This saves an allocation, but does mean that the local
        // frame of the coroutine will remain allocated (with the
        // coroutine's imbound parameters still live) until all
        // references are destroyed. To force the promise_base to be
        // destroyed after co_await, we make the promise_base a
        // move-only object and require co_await to be given an rvalue reference.
        
        // Special values for m_waiting.
        static void* running_ptr() { return nullptr; }
        static void* completed_ptr() { return reinterpret_cast<void*>(1); }
        static void* abandoned_ptr() { return reinterpret_cast<void*>(2); }

        // The awaiting coroutine is resumed by calling the
        // m_resumer with the m_waiting. If the resumer is null,
        // then the m_waiting is assumed to be the address of a
        // coroutine_handle<>, which is resumed synchronously.
        // Externalizing the resumer allows unused awaiters to be
        // removed by the linker and removes a hard dependency on COM.
        // Using nullptr to represent the default resumer avoids a
        // CFG check.

        void(__stdcall* m_resumer)(void*);
        std::atomic<void*> m_waiting{ running_ptr() };
        result_holder<T> m_holder;

        // Make it easier to access our CRTP derived class.
        using Promise = task_promise<T>;
        auto as_promise() noexcept
        {
            return static_cast<Promise*>(this);
        }

        // Make it easier to access the coroutine handle.
        auto as_handle() noexcept
        {
            return __WI_COROUTINE_NAMESPACE::coroutine_handle<Promise>::from_promise(*as_promise());
        }

        auto get_return_object() noexcept
        {
            // let the compiler construct the task / com_task from the promise.
            return as_promise();
        }

        void destroy()
        {
            as_handle().destroy();
        }

        // The client lost interest in the coroutine, either because they are discarding
        // the result without awaiting (risky!), or because they have finished awaiting.
        // Discarding the result without awaiting is risky because any exception in the coroutine
        // will be unobserved and result in a crash. If you want to disallow it, then
        // raise an exception if waiting == running_ptr.
        void abandon()
        {
            auto waiting = m_waiting.exchange(abandoned_ptr(), std::memory_order_acq_rel);
            if (waiting != running_ptr()) destroy();
        }

        __WI_COROUTINE_NAMESPACE::suspend_never initial_suspend() noexcept
        {
            return {};
        }

        template<typename...Args>
        void emplace_value(Args&&... args)
        {
            m_holder.emplace_value(wistd::forward<Args>(args)...);
        }

        void unhandled_exception() noexcept
        {
            m_holder.unhandled_exception();
        }

        void resume_waiting_coroutine(void* waiting) const
        {
            if (m_resumer)
            {
                m_resumer(waiting);
            }
            else
            {
                __WI_COROUTINE_NAMESPACE::coroutine_handle<>::from_address(waiting).resume();
            }
        }

        auto final_suspend() noexcept
        {
            struct awaiter : __WI_COROUTINE_NAMESPACE::suspend_always
            {
                promise_base& self;
                void await_suspend(__WI_COROUTINE_NAMESPACE::coroutine_handle<>) const noexcept
                {
                    // Need acquire so we can read from m_resumer.
                    // Need release so that the results are published in the case that nobody
                    // is awaiting right now, so that the eventual awaiter (possibly on another thread)
                    // can read the results.
                    auto waiting = self.m_waiting.exchange(completed_ptr(), std::memory_order_acq_rel);
                    if (waiting == abandoned_ptr())
                    {
                        self.destroy();
                    }
                    else if (waiting != running_ptr())
                    {
                        WI_ASSERT(waiting != completed_ptr());
                        self.resume_waiting_coroutine(waiting);
                    }
                };
            };
            return awaiter{ {}, *this };
        }

        // The remaining methods are used by the awaiters.
        bool client_await_ready()
        {
            // Need acquire in case the coroutine has already completed,
            // so we can read the results. This matches the release in
            // the final_suspend's await_suspend.
            auto waiting = m_waiting.load(std::memory_order_acquire);
            WI_ASSERT((waiting == running_ptr()) || (waiting == completed_ptr()));
            return waiting != running_ptr();
        }

        auto client_await_suspend(void* waiting, void(__stdcall* resumer)(void*))
        {
            // "waiting" needs to be a pointer to an object. We reserve the first 16
            // pseudo-pointers as sentinels.
            WI_ASSERT(reinterpret_cast<uintptr_t>(waiting) > 16);

            m_resumer = resumer;

            // Acquire to ensure that we can read the results of the return value, if the coroutine is completed.
            // Release to ensure that our resumption state is published, if the coroutine is not completed.
            auto previous = m_waiting.exchange(waiting, std::memory_order_acq_rel);

            // Suspend if the coroutine is still running.
            // Otherwise, the coroutine is completed: Nobody will resume us, so we will have to resume ourselves.
            WI_ASSERT((previous == running_ptr()) || (previous == completed_ptr()));
            return previous == running_ptr();
        }

        T client_await_resume()
        {
            return m_holder.get_value();
        }
    };

    template<typename T>
    struct task_promise : promise_base<T>
    {
        template<typename U>
        void return_value(U&& value)
        {
            this->emplace_value(wistd::forward<U>(value));
        }

        template<typename Dummy = void>
        wistd::enable_if_t<!wistd::is_reference_v<T>, Dummy>
            return_value(T const& value)
        {
            this->emplace_value(value);
        }
    };

    template<>
    struct task_promise<void> : promise_base<void>
    {
        void return_void()
        {
            this->emplace_value();
        }
    };

    template<typename T>
    struct promise_deleter
    {
        void operator()(promise_base<T>* promise) const noexcept
        {
            promise->abandon();
        }
    };

    template<typename T>
    using promise_ptr = wistd::unique_ptr<promise_base<T>, promise_deleter<T>>;

    template<typename T>
    struct agile_awaiter
    {
        agile_awaiter(promise_ptr<T>&& initial) : promise(wistd::move(initial)) { }

        promise_ptr<T> promise;

        bool await_ready()
        {
            return promise->client_await_ready();
        }

        auto await_suspend(__WI_COROUTINE_NAMESPACE::coroutine_handle<> handle)
        {
            // Use the default resumer.
            return promise->client_await_suspend(handle.address(), nullptr);
        }

        T await_resume()
        {
            return promise->client_await_resume();
        }
    };

    template<typename T>
    struct task_base
    {
        auto resume_any_thread() && noexcept
        {
            return agile_awaiter<T>{ wistd::move(promise) };
        }

        // You must #include <ole2.h> before <wil\coroutine.h> to enable apartment-aware awaiting.
        auto resume_same_apartment() && noexcept;

        // Compiler error message metaprogramming: Tell people that they
        // need to use std::move() if they try to co_await an lvalue.
        struct cannot_await_lvalue_use_std_move { void await_ready() {} };
        cannot_await_lvalue_use_std_move operator co_await() & = delete;

        // You must #include <synchapi.h> (usually via <windows.h>) to enable synchronous waiting.
        decltype(auto) get() &&;

    protected:
        task_base(task_promise<T>* initial = nullptr) noexcept : promise(initial) {}

        template<typename D>
        D& assign(D* self, task_base&& other) noexcept
        {
            static_cast<task_base&>(*this) = wistd::move(other);
            return *self;
        }

    private:
        promise_ptr<T> promise;

        static void __stdcall wake_by_address(void* completed);
    };
}

namespace wil
{
    // Must write out both classes separately
    // Cannot use deduction guides with alias template type prior to C++20.
    template<typename T>
    struct task : details::coro::task_base<T>
    {
        using base = details::coro::task_base<T>;
        // Constructing from task_promise<T>* cannot be explicit because get_return_object relies on implicit conversion.
        task(details::coro::task_promise<T>* initial = nullptr) noexcept : base(initial) {}
        explicit task(base&& other) noexcept : base(wistd::move(other)) {}
        task& operator=(base&& other) noexcept
        {
            return base::assign(this, wistd::move(other));
        }

        using base::operator co_await;

        auto operator co_await() && noexcept
        {
            return wistd::move(*this).resume_any_thread();
        }
    };

    template<typename T>
    struct com_task : details::coro::task_base<T>
    {
        using base = details::coro::task_base<T>;
        // Constructing from task_promise<T>* cannot be explicit because get_return_object relies on implicit conversion.
        com_task(details::coro::task_promise<T>* initial = nullptr) noexcept : base(initial) {}
        explicit com_task(base&& other) noexcept : base(wistd::move(other)) {}
        com_task& operator=(base&& other) noexcept
        {
            return base::assign(this, wistd::move(other));
        }

        using base::operator co_await;

        auto operator co_await() && noexcept
        {
            // You must #include <ole2.h> before <wil\coroutine.h> to enable non-agile awaiting.
            return wistd::move(*this).resume_same_apartment();
        }
    };

    template<typename T>
    task(com_task<T>&&)->task<T>;
    template<typename T>
    com_task(task<T>&&)->com_task<T>;
}

template <typename T, typename... Args>
struct __WI_COROUTINE_NAMESPACE::coroutine_traits<wil::task<T>, Args...>
{
    using promise_type = wil::details::coro::task_promise<T>;
};

template <typename T, typename... Args>
struct __WI_COROUTINE_NAMESPACE::coroutine_traits<wil::com_task<T>, Args...>
{
    using promise_type = wil::details::coro::task_promise<T>;
};

#endif // __WIL_COROUTINE_INCLUDED

// Can re-include this header after including synchapi.h (usually via windows.h) to enable synchronous wait.
#if defined(_SYNCHAPI_H_) && !defined(__WIL_COROUTINE_SYNCHRONOUS_GET_INCLUDED)
#define __WIL_COROUTINE_SYNCHRONOUS_GET_INCLUDED

namespace wil::details::coro
{
    template<typename T>
    decltype(auto) task_base<T>::get() &&
    {
        if (!promise->client_await_ready())
        {
            bool completed = false;
            if (promise->client_await_suspend(&completed, wake_by_address))
            {
                bool pending = false;
                while (!completed)
                {
                    WaitOnAddress(&completed, &pending, sizeof(pending), INFINITE);
                }
            }
        }
        return std::exchange(promise, {})->client_await_resume();
    }

    template<typename T>
    void __stdcall task_base<T>::wake_by_address(void* completed)
    {
        *reinterpret_cast<bool*>(completed) = true;
        WakeByAddressSingle(completed);
    }
}
#endif // __WIL_COROUTINE_SYNCHRONOUS_GET_INCLUDED

// Can re-include this header after including COM header files to enable non-agile tasks.
#if defined(_COMBASEAPI_H_) && defined(_THREADPOOLAPISET_H_) && !defined(__WIL_COROUTINE_NON_AGILE_INCLUDED)
#define __WIL_COROUTINE_NON_AGILE_INCLUDED
#include <ctxtcall.h>
#include <wil/com.h>

namespace wil::details::coro
{
    struct apartment_info
    {
        APTTYPE aptType;
        APTTYPEQUALIFIER aptTypeQualifier;

        void load()
        {
            if (FAILED(CoGetApartmentType(&aptType, &aptTypeQualifier)))
            {
                // If COM is not initialized, then act as if we are running
                // on the implicit MTA.
                aptType = APTTYPE_MTA;
                aptTypeQualifier = APTTYPEQUALIFIER_IMPLICIT_MTA;
            }
        }
    };

    // apartment_resumer resumes a coroutine in a captured apartment.
    struct apartment_resumer
    {
        static auto as_self(void* p)
        {
            return reinterpret_cast<apartment_resumer*>(p);
        }

        static bool is_sta()
        {
            apartment_info info;
            info.load();
            switch (info.aptType)
            {
            case APTTYPE_STA:
            case APTTYPE_MAINSTA:
                return true;
            case APTTYPE_NA:
                return info.aptTypeQualifier == APTTYPEQUALIFIER_NA_ON_STA ||
                    info.aptTypeQualifier == APTTYPEQUALIFIER_NA_ON_MAINSTA;
            default:
                return false;
            }
        }

        static wil::com_ptr<IContextCallback> current_context()
        {
            wil::com_ptr<IContextCallback> context;
            // This will fail if COM is not initialized. Treat as implicit MTA.
            // Do not use IID_PPV_ARGS to avoid ambiguity between ::IUnknown and winrt::IUnknown.
            CoGetObjectContext(__uuidof(IContextCallback), reinterpret_cast<void**>(&context));
            return context;
        }

        __WI_COROUTINE_NAMESPACE::coroutine_handle<> waiter;
        wil::com_ptr<IContextCallback> context{ nullptr };
        apartment_info info;
        HRESULT resume_result = S_OK;

        void capture_context(__WI_COROUTINE_NAMESPACE::coroutine_handle<> handle)
        {
            waiter = handle;
            info.load();
            context = current_context();
            if (context == nullptr)
            {
                __debugbreak();
            }
        }

        static void __stdcall resume_in_context(void* parameter)
        {
            auto self = as_self(parameter);
            if (self->context == nullptr || self->context == current_context())
            {
                self->context = nullptr; // removes the context cleanup from the resume path
                self->waiter();
            }
            else if (is_sta())
            {
                submit_threadpool_callback(resume_context, self);
            }
            else
            {
                self->resume_context_sync();
            }
        }

        static void submit_threadpool_callback(PTP_SIMPLE_CALLBACK callback, void* context)
        {
            THROW_IF_WIN32_BOOL_FALSE(TrySubmitThreadpoolCallback(callback, context, nullptr));
        }

        static void CALLBACK resume_context(PTP_CALLBACK_INSTANCE /*instance*/, void* parameter)
        {
            as_self(parameter)->resume_context_sync();
        }

        void resume_context_sync()
        {
            ComCallData data{};
            data.pUserDefined = this;
            // The call to resume_apartment_callback will destruct the context.
            // Capture into a local so we don't destruct it while it's in use.
            // This also removes the context cleanup from the resume path.
            auto local_context = wistd::move(context);
            auto result = local_context->ContextCallback(resume_apartment_callback, &data, IID_ICallbackWithNoReentrancyToApplicationSTA, 5, nullptr);
            if (FAILED(result))
            {
                // Unable to resume on the correct apartment.
                // Resume on the wrong apartment, but tell the coroutine why.
                resume_result = result;
                waiter();
            }
        }

        static HRESULT CALLBACK resume_apartment_callback(ComCallData* data) noexcept
        {
            as_self(data->pUserDefined)->waiter();
            return S_OK;
        }

        void check()
        {
            THROW_IF_FAILED(resume_result);
        }
    };

    // The COM awaiter captures the COM context when the co_await begins.
    // When the co_await completes, it uses that COM context to resume execution.
    // This follows the same algorithm employed by C++/WinRT, which has features like
    // avoiding stack buildup and proper handling of the neutral apartment.
    // It does, however, introduce fail-fast code paths if thread pool tasks cannot
    // be submitted. (Those fail-fasts could be removed by preallocating the tasks,
    // but that means paying an up-front cost for something that may never end up used,
    // as well as introducing extra cleanup code in the fast-path.)
    template<typename T>
    struct com_awaiter : agile_awaiter<T>
    {
        com_awaiter(promise_ptr<T>&& initial) : agile_awaiter<T>(wistd::move(initial)) { }
        apartment_resumer resumer;

        auto await_suspend(__WI_COROUTINE_NAMESPACE::coroutine_handle<> handle)
        {
            resumer.capture_context(handle);
            return this->promise->client_await_suspend(wistd::addressof(resumer), apartment_resumer::resume_in_context);
        }

        decltype(auto) await_resume()
        {
            resumer.check();
            return agile_awaiter<T>::await_resume();
        }
    };

    template<typename T>
    auto task_base<T>::resume_same_apartment() && noexcept
    {
        return com_awaiter<T>{ wistd::move(promise) };
    }
}
#endif // __WIL_COROUTINE_NON_AGILE_INCLUDED
