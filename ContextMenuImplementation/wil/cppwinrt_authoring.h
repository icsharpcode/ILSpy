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

namespace wil
{
#ifndef __WIL_CPPWINRT_AUTHORING_PROPERTIES_INCLUDED
#define __WIL_CPPWINRT_AUTHORING_PROPERTIES_INCLUDED
    namespace details
    {
        template<typename T>
        struct single_threaded_property_storage
        {
            T m_value{};
            single_threaded_property_storage() = default;
            single_threaded_property_storage(const T& value) : m_value(value) {}
            operator T& () { return m_value; }
            operator T const& () const { return m_value; }
            template<typename Q> auto operator=(Q&& q)
            {
                m_value = wistd::forward<Q>(q);
                return *this;
            }
        };
    }

    template <typename T>
    struct single_threaded_property : std::conditional_t<std::is_scalar_v<T> || std::is_final_v<T>, wil::details::single_threaded_property_storage<T>, T>
    {
        single_threaded_property() = default;
        template <typename... TArgs> single_threaded_property(TArgs&&... value) : base_type(std::forward<TArgs>(value)...) {}

        using base_type = std::conditional_t<std::is_scalar_v<T> || std::is_final_v<T>, wil::details::single_threaded_property_storage<T>, T>;

        T operator()() const
        {
            return *this;
        }

        template<typename Q> auto& operator()(Q&& q)
        {
            *this = std::forward<Q>(q);
            return *this;
        }

        template<typename Q> auto& operator=(Q&& q)
        {
            static_cast<base_type&>(*this) = std::forward<Q>(q);
            return *this;
        }
    };

    template <typename T>
    struct single_threaded_rw_property : single_threaded_property<T>
    {
        using base_type = single_threaded_property<T>;
        template<typename... TArgs> single_threaded_rw_property(TArgs&&... value) : base_type(std::forward<TArgs>(value)...) {}

        using base_type::operator();

        // needed in lieu of deducing-this
        template<typename Q> auto& operator()(Q&& q)
        {
            return *this = std::forward<Q>(q);
        }

        // needed in lieu of deducing-this
        template<typename Q> auto& operator=(Q&& q)
        {
            base_type::operator=(std::forward<Q>(q));
            return *this;
        }
    };

#endif // __WIL_CPPWINRT_AUTHORING_PROPERTIES_INCLUDED

#if !defined(__WIL_CPPWINRT_AUTHORING_INCLUDED_FOUNDATION) && defined(WINRT_Windows_Foundation_H) // WinRT / XAML helpers
#define __WIL_CPPWINRT_AUTHORING_INCLUDED_FOUNDATION
    namespace details
    {
        template<typename T>
        struct event_base {
            winrt::event_token operator()(T&& handler)
            {
                return m_handler.add(std::forward<T>(handler));
            }

            auto operator()(const winrt::event_token& token) noexcept
            {
                return m_handler.remove(token);
            }

            template<typename... TArgs>
            auto invoke(TArgs&&... args)
            {
                return m_handler(std::forward<TArgs>(args)...);
            }
        private:
            winrt::event<T> m_handler;
        };
    }

    /**
     * @brief A default event handler that maps to [Windows.Foundation.EventHandler](https://docs.microsoft.com/uwp/api/windows.foundation.eventhandler-1).
     * @tparam T The event data type.
    */
    template<typename T>
    struct simple_event : wil::details::event_base<winrt::Windows::Foundation::EventHandler<T>> {};

    /**
     * @brief A default event handler that maps to [Windows.Foundation.TypedEventHandler](https://docs.microsoft.com/uwp/api/windows.foundation.typedeventhandler-2).
     * @tparam T The event data type.
     * @details Usage example:
     * @code
     *         // In IDL, this corresponds to:
     *         //   event Windows.Foundation.TypedEventHandler<ModalPage, String> OkClicked;
     *         wil::typed_event<MarkupSample::ModalPage, winrt::hstring> OkClicked;
     * @endcode
    */
    template<typename TSender, typename TArgs>
    struct typed_event : wil::details::event_base<winrt::Windows::Foundation::TypedEventHandler<TSender, TArgs>> {};

#endif // !defined(__WIL_CPPWINRT_AUTHORING_INCLUDED_FOUNDATION) && defined(WINRT_Windows_Foundation_H)

#if !defined(__WIL_CPPWINRT_AUTHORING_INCLUDED_XAML_DATA) && (defined(WINRT_Microsoft_UI_Xaml_Data_H) || defined(WINRT_Windows_UI_Xaml_Data_H)) // INotifyPropertyChanged helpers
#define __WIL_CPPWINRT_AUTHORING_INCLUDED_XAML_DATA
    namespace details
    {
#ifdef WINRT_Microsoft_UI_Xaml_Data_H
        using Xaml_Data_PropertyChangedEventHandler = winrt::Microsoft::UI::Xaml::Data::PropertyChangedEventHandler;
        using Xaml_Data_PropertyChangedEventArgs = winrt::Microsoft::UI::Xaml::Data::PropertyChangedEventArgs;
#elif defined(WINRT_Windows_UI_Xaml_Data_H)
        using Xaml_Data_PropertyChangedEventHandler = winrt::Windows::UI::Xaml::Data::PropertyChangedEventHandler;
        using Xaml_Data_PropertyChangedEventArgs = winrt::Windows::UI::Xaml::Data::PropertyChangedEventArgs;
#endif
    }

    /**
     * @brief Helper base class to inherit from to have a simple implementation of [INotifyPropertyChanged](https://docs.microsoft.com/uwp/api/windows.ui.xaml.data.inotifypropertychanged).
     * @tparam T CRTP type
     * @details When you declare your class, make this class a base class and pass your class as a template parameter:
     * @code
     * struct MyPage : MyPageT<MyPage>, wil::notify_property_changed_base<MyPage>
     * {
     *     wil::single_threaded_notifying_property<int> MyInt; 
     *     MyPage() : INIT_NOTIFYING_PROPERTY(MyInt, 42) { } 
     *     // or
     *     WIL_NOTIFYING_PROPERTY(int, MyInt, 42);
     * };
     * @endcode
    */
    template<typename T,
        typename Xaml_Data_PropertyChangedEventHandler = wil::details::Xaml_Data_PropertyChangedEventHandler,
        typename Xaml_Data_PropertyChangedEventArgs = wil::details::Xaml_Data_PropertyChangedEventArgs>
    struct notify_property_changed_base
    {
        using Type = T;
        auto PropertyChanged(Xaml_Data_PropertyChangedEventHandler const& value)
        {
            return m_propertyChanged.add(value);
        }
        
        void PropertyChanged(winrt::event_token const& token)
        {
            m_propertyChanged.remove(token);
        }
        
        Type& self()
        {
            return *static_cast<Type*>(this);
        }

        /**
         * @brief Raises a property change notification event
         * @param name The name of the property
         * @return
         * @details Usage example\n
         * C++
         * @code
         * void MyPage::DoSomething()
         * {
         *     // modify MyInt
         *     // MyInt = ...
         *
         *     // now send a notification to update the bound UI elements
         *     RaisePropertyChanged(L"MyInt");
         * }
         * @endcode
        */
        auto RaisePropertyChanged(std::wstring_view name)
        {
            return m_propertyChanged(self(), Xaml_Data_PropertyChangedEventArgs{ name });
        }
    protected:
        winrt::event<Xaml_Data_PropertyChangedEventHandler> m_propertyChanged;
    };

    /**
     * @brief Implements a property type with notifications
     * @tparam T the property type
     * @details Use the #INIT_NOTIFY_PROPERTY macro to initialize this property in your class constructor. This will set up the right property name, and bind it to the `notify_property_changed_base` implementation.
    */
    template<typename T,
        typename Xaml_Data_PropertyChangedEventHandler = wil::details::Xaml_Data_PropertyChangedEventHandler,
        typename Xaml_Data_PropertyChangedEventArgs = wil::details::Xaml_Data_PropertyChangedEventArgs>
    struct single_threaded_notifying_property : single_threaded_rw_property<T>
    {
        using Type = T;
        using base_type = single_threaded_rw_property<T>;
        using base_type::operator();

        template<typename Q> auto& operator()(Q&& q)
        {
            return *this = std::forward<Q>(q);
        }

        template<typename Q> auto& operator=(Q&& q)
        {
            if (q != this->operator()())
            {
                static_cast<base_type&>(*this) = std::forward<Q>(q);
                if (auto strong = m_sender.get(); (m_npc != nullptr) && (strong != nullptr))
                {
                    (*m_npc)(strong, Xaml_Data_PropertyChangedEventArgs{ m_name });
                }
            }
            return *this;
        }

        template<typename... TArgs>
        single_threaded_notifying_property(
            winrt::event<Xaml_Data_PropertyChangedEventHandler>* npc,
            const winrt::Windows::Foundation::IInspectable& sender,
            std::wstring_view name,
            TArgs&&... args) :
            single_threaded_rw_property<T>(std::forward<TArgs...>(args)...),
            m_name(name),
            m_npc(npc),
            m_sender(sender)
        {}

        single_threaded_notifying_property(const single_threaded_notifying_property&) = default;
        single_threaded_notifying_property(single_threaded_notifying_property&&) = default;
        std::wstring_view Name() const noexcept { return m_name; }
    private:
        std::wstring_view m_name;
        winrt::event<Xaml_Data_PropertyChangedEventHandler>* m_npc;
        winrt::weak_ref<winrt::Windows::Foundation::IInspectable> m_sender;
    };

    /**
    * @def WIL_NOTIFYING_PROPERTY
    * @brief use this to stamp out a property that calls RaisePropertyChanged upon changing its value. This is a zero-storage alternative to wil::single_threaded_notifying_property<T>
    * @details You can pass an initializer list for the initial property value in the variadic arguments to this macro.
    */
#define WIL_NOTIFYING_PROPERTY(type, name, ...)             \
    type m_##name{__VA_ARGS__};                             \
    auto name() const noexcept { return m_##name; }         \
    auto& name(type value)                                  \
    {                                                       \
        if (m_##name != value)                              \
        {                                                   \
            m_##name = std::move(value);                    \
            RaisePropertyChanged(L"" #name);                \
        }                                                   \
        return *this;                                       \
    }                                                       \

    /**
    * @def INIT_NOTIFYING_PROPERTY
    * @brief use this to initialize a wil::single_threaded_notifying_property in your class constructor.
    */
#define INIT_NOTIFYING_PROPERTY(NAME, VALUE)  \
        NAME(&m_propertyChanged, *this, L"" #NAME, VALUE)

#endif // !defined(__WIL_CPPWINRT_AUTHORING_INCLUDED_XAML_DATA) && (defined(WINRT_Microsoft_UI_Xaml_Data_H) || defined(WINRT_Windows_UI_Xaml_Data_H))
} // namespace wil
