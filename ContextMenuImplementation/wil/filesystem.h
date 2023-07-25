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
#ifndef __WIL_FILESYSTEM_INCLUDED
#define __WIL_FILESYSTEM_INCLUDED

#ifdef _KERNEL_MODE
#error This header is not supported in kernel-mode.
#endif

#include <new>
#include <combaseapi.h> // Needed for CoTaskMemFree() used in output of some helpers.
#include <winbase.h> // LocalAlloc
#include <PathCch.h>
#include "result.h"
#include "win32_helpers.h"
#include "resource.h"

namespace wil
{
    //! Determines if a path is an extended length path that can be used to access paths longer than MAX_PATH.
    inline bool is_extended_length_path(_In_ PCWSTR path)
    {
        return wcsncmp(path, L"\\\\?\\", 4) == 0;
    }

#if (_WIN32_WINNT >= _WIN32_WINNT_WIN7)
    //! Find the last segment of a path. Matches the behavior of shlwapi!PathFindFileNameW()
    //! note, does not support streams being specified like PathFindFileNameW(), is that a bug or a feature?
    inline PCWSTR find_last_path_segment(_In_ PCWSTR path)
    {
        auto const pathLength = wcslen(path);
        // If there is a trailing slash ignore that in the search.
        auto const limitedLength = ((pathLength > 0) && (path[pathLength - 1] == L'\\')) ? (pathLength - 1) : pathLength;

        PCWSTR result = nullptr;
        auto const offset = FindStringOrdinal(FIND_FROMEND, path, static_cast<int>(limitedLength), L"\\", 1, TRUE);
        if (offset == -1)
        {
            result = path + pathLength; // null terminator
        }
        else
        {
            result = path + offset + 1; // just past the slash
        }
        return result;
    }
#endif

    //! Determine if the file name is one of the special "." or ".." names.
    inline bool path_is_dot_or_dotdot(_In_ PCWSTR fileName)
    {
        return ((fileName[0] == L'.') &&
               ((fileName[1] == L'\0') || ((fileName[1] == L'.') && (fileName[2] == L'\0'))));
    }

    //! Returns the drive number, if it has one. Returns true if there is a drive number, false otherwise. Supports regular and extended length paths.
    inline bool try_get_drive_letter_number(_In_ PCWSTR path, _Out_ int* driveNumber)
    {
        if (path[0] == L'\\' && path[1] == L'\\' && path[2] == L'?' && path[3] == L'\\')
        {
            path += 4;
        }
        if (path[0] && (path[1] == L':'))
        {
            if ((path[0] >= L'a') && (path[0] <= L'z'))
            {
                *driveNumber = path[0] - L'a';
                return true;
            }
            else if ((path[0] >= L'A') && (path[0] <= L'Z'))
            {
                *driveNumber = path[0] - L'A';
                return true;
            }
        }
        *driveNumber = -1;
        return false;
    }

#if WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP) && (_WIN32_WINNT >= _WIN32_WINNT_WIN7)

    // PathCch.h APIs are only in desktop API for now.

    // Compute the substring in the input value that is the parent folder path.
    // returns:
    //      true + parentPathLength - path has a parent starting at the beginning path and of parentPathLength length.
    //      false, no parent path, the input is a root path.
    inline bool try_get_parent_path_range(_In_ PCWSTR path, _Out_ size_t* parentPathLength)
    {
        *parentPathLength = 0;
        bool hasParent = false;
        PCWSTR rootEnd = nullptr;
        if (SUCCEEDED(PathCchSkipRoot(path, &rootEnd)) && (*rootEnd != L'\0'))
        {
            auto const lastSegment = find_last_path_segment(path);
            *parentPathLength = lastSegment - path;
            hasParent = (*parentPathLength != 0);
        }
        return hasParent;
    }

    // Creates directories for the specified path, creating parent paths
    // as needed.
    inline HRESULT CreateDirectoryDeepNoThrow(PCWSTR path) WI_NOEXCEPT
    {
        if (::CreateDirectoryW(path, nullptr) == FALSE)
        {
            DWORD lastError = ::GetLastError();
            if (lastError == ERROR_PATH_NOT_FOUND)
            {
                size_t parentLength{};
                if (try_get_parent_path_range(path, &parentLength))
                {
                    wistd::unique_ptr<wchar_t[]> parent(new (std::nothrow) wchar_t[parentLength + 1]);
                    RETURN_IF_NULL_ALLOC(parent.get());
                    RETURN_IF_FAILED(StringCchCopyNW(parent.get(), parentLength + 1, path, parentLength));
                    RETURN_IF_FAILED(CreateDirectoryDeepNoThrow(parent.get())); // recurs
                }
                if (::CreateDirectoryW(path, nullptr) == FALSE)
                {
                    lastError = ::GetLastError();
                    if (lastError != ERROR_ALREADY_EXISTS)
                    {
                        RETURN_WIN32(lastError);
                    }
                }
            }
            else if (lastError != ERROR_ALREADY_EXISTS)
            {
                RETURN_WIN32(lastError);
            }
        }
        return S_OK;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    inline void CreateDirectoryDeep(PCWSTR path)
    {
        THROW_IF_FAILED(CreateDirectoryDeepNoThrow(path));
    }
#endif // WIL_ENABLE_EXCEPTIONS

    //! A strongly typed version of the Win32 API GetFullPathNameW.
    //! Return a path in an allocated buffer for handling long paths.
    //! Optionally return the pointer to the file name part.
    template <typename string_type, size_t stackBufferLength = 256>
    HRESULT GetFullPathNameW(PCWSTR file, string_type& path, _Outptr_opt_ PCWSTR* filePart = nullptr)
    {
        wil::assign_null_to_opt_param(filePart);
        const auto hr = AdaptFixedSizeToAllocatedResult<string_type, stackBufferLength>(path,
            [&](_Out_writes_(valueLength) PWSTR value, size_t valueLength, _Out_ size_t* valueLengthNeededWithNull) -> HRESULT
        {
            // Note that GetFullPathNameW() is not limited to MAX_PATH
            // but it does take a fixed size buffer.
            *valueLengthNeededWithNull = ::GetFullPathNameW(file, static_cast<DWORD>(valueLength), value, nullptr);
            RETURN_LAST_ERROR_IF(*valueLengthNeededWithNull == 0);
            WI_ASSERT((*value != L'\0') == (*valueLengthNeededWithNull < valueLength));
            if (*valueLengthNeededWithNull < valueLength)
            {
                (*valueLengthNeededWithNull)++; // it fit, account for the null
            }
            return S_OK;
        });
        if (SUCCEEDED(hr) && filePart)
        {
            *filePart = wil::find_last_path_segment(details::string_maker<string_type>::get(path));
        }
        return hr;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    //! A strongly typed version of the Win32 API of GetFullPathNameW.
    //! Return a path in an allocated buffer for handling long paths.
    //! Optionally return the pointer to the file name part.
    template <typename string_type = wil::unique_cotaskmem_string, size_t stackBufferLength = 256>
    string_type GetFullPathNameW(PCWSTR file, _Outptr_opt_ PCWSTR* filePart = nullptr)
    {
        string_type result{};
        THROW_IF_FAILED((GetFullPathNameW<string_type, stackBufferLength>(file, result, filePart)));
        return result;
    }
#endif

    enum class RemoveDirectoryOptions
    {
        None = 0,
        KeepRootDirectory = 0x1,
        RemoveReadOnly = 0x2,
    };
    DEFINE_ENUM_FLAG_OPERATORS(RemoveDirectoryOptions);

    namespace details
    {
        // Reparse points should not be traversed in most recursive walks of the file system,
        // unless allowed through the appropriate reparse tag.
        inline bool CanRecurseIntoDirectory(const FILE_ATTRIBUTE_TAG_INFO& info)
        {
            return (WI_IsFlagSet(info.FileAttributes, FILE_ATTRIBUTE_DIRECTORY) &&
                    (WI_IsFlagClear(info.FileAttributes, FILE_ATTRIBUTE_REPARSE_POINT) ||
                    (IsReparseTagDirectory(info.ReparseTag) || (info.ReparseTag == IO_REPARSE_TAG_WCI))));
        }
    }

    // Retrieve a handle to a directory only if it is safe to recurse into.
    inline wil::unique_hfile TryCreateFileCanRecurseIntoDirectory(PCWSTR path, PWIN32_FIND_DATAW fileFindData, DWORD access = GENERIC_READ | /*DELETE*/ 0x00010000L, DWORD share = FILE_SHARE_READ)
    {
        wil::unique_hfile result(CreateFileW(path, access, share,
            nullptr, OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS | FILE_FLAG_OPEN_REPARSE_POINT, nullptr));
        if (result)
        {
            FILE_ATTRIBUTE_TAG_INFO fati{};
            if (GetFileInformationByHandleEx(result.get(), FileAttributeTagInfo, &fati, sizeof(fati)) &&
                details::CanRecurseIntoDirectory(fati))
            {
                if (fileFindData)
                {
                    // Refresh the found file's data now that we have secured the directory from external manipulation.
                    fileFindData->dwFileAttributes = fati.FileAttributes;
                    fileFindData->dwReserved0 = fati.ReparseTag;
                }
            }
            else
            {
                result.reset();
            }
        }

        return result;
    }

    // If inputPath is a non-normalized name be sure to pass an extended length form to ensure
    // it can be addressed and deleted.
    inline HRESULT RemoveDirectoryRecursiveNoThrow(PCWSTR inputPath, RemoveDirectoryOptions options = RemoveDirectoryOptions::None, HANDLE deleteHandle = INVALID_HANDLE_VALUE) WI_NOEXCEPT
    {
        wil::unique_hlocal_string path;
        PATHCCH_OPTIONS combineOptions = PATHCCH_NONE;

        if (is_extended_length_path(inputPath))
        {
            path = wil::make_hlocal_string_nothrow(inputPath);
            RETURN_IF_NULL_ALLOC(path);
            // PathAllocCombine will convert extended length paths to regular paths if shorter than
            // MAX_PATH, avoid that behavior to provide access inputPath with non-normalized names.
            combineOptions = PATHCCH_ENSURE_IS_EXTENDED_LENGTH_PATH;
        }
        else
        {
            // For regular paths normalize here to get consistent results when searching and deleting.
            RETURN_IF_FAILED(wil::GetFullPathNameW(inputPath, path));
            combineOptions = PATHCCH_ALLOW_LONG_PATHS;
        }

        wil::unique_hlocal_string searchPath;
        RETURN_IF_FAILED(::PathAllocCombine(path.get(), L"*", combineOptions, &searchPath));

        WIN32_FIND_DATAW fd{};
        wil::unique_hfind findHandle(::FindFirstFileW(searchPath.get(), &fd));
        RETURN_LAST_ERROR_IF(!findHandle);

        for (;;)
        {
            // skip "." and ".."
            if (!(WI_IsFlagSet(fd.dwFileAttributes, FILE_ATTRIBUTE_DIRECTORY) && path_is_dot_or_dotdot(fd.cFileName)))
            {
                // Need to form an extended length path to provide the ability to delete paths > MAX_PATH
                // and files with non-normalized names (dots or spaces at the end).
                wil::unique_hlocal_string pathToDelete;
                RETURN_IF_FAILED(::PathAllocCombine(path.get(), fd.cFileName,
                    PATHCCH_ENSURE_IS_EXTENDED_LENGTH_PATH | PATHCCH_DO_NOT_NORMALIZE_SEGMENTS, &pathToDelete));
                if (WI_IsFlagSet(fd.dwFileAttributes, FILE_ATTRIBUTE_DIRECTORY))
                {
                    // Get a handle to the directory to delete, preventing it from being replaced to prevent writes which could be used
                    // to bypass permission checks, and verify that it is not a name surrogate (e.g. symlink, mount point, etc).
                    wil::unique_hfile recursivelyDeletableDirectoryHandle = TryCreateFileCanRecurseIntoDirectory(pathToDelete.get(), &fd);
                    if (recursivelyDeletableDirectoryHandle)
                    {
                        RemoveDirectoryOptions localOptions = options;
                        RETURN_IF_FAILED(RemoveDirectoryRecursiveNoThrow(pathToDelete.get(), WI_ClearFlag(localOptions, RemoveDirectoryOptions::KeepRootDirectory), recursivelyDeletableDirectoryHandle.get()));
                    }
                    else if (WI_IsFlagSet(fd.dwFileAttributes, FILE_ATTRIBUTE_REPARSE_POINT))
                    {
                        // This is a directory reparse point that should not be recursed. Delete it without traversing into it.
                        RETURN_IF_WIN32_BOOL_FALSE(::RemoveDirectoryW(pathToDelete.get()));
                    }
                    else
                    {
                        // Failed to grab a handle to the file or to read its attributes. This is not safe to recurse.
                        RETURN_WIN32(::GetLastError());
                    }
                }
                else
                {
                    // Try a DeleteFile.  Some errors may be recoverable.
                    if (!::DeleteFileW(pathToDelete.get()))
                    {
                        // Fail for anything other than ERROR_ACCESS_DENIED with option to RemoveReadOnly available
                        bool potentiallyFixableReadOnlyProblem =
                            WI_IsFlagSet(options, RemoveDirectoryOptions::RemoveReadOnly) && ::GetLastError() == ERROR_ACCESS_DENIED;
                        RETURN_LAST_ERROR_IF(!potentiallyFixableReadOnlyProblem);

                        // Fail if the file does not have read-only set, likely just an ACL problem
                        DWORD fileAttr = ::GetFileAttributesW(pathToDelete.get());
                        RETURN_LAST_ERROR_IF(!WI_IsFlagSet(fileAttr, FILE_ATTRIBUTE_READONLY));

                        // Remove read-only flag, setting to NORMAL if completely empty
                        WI_ClearFlag(fileAttr, FILE_ATTRIBUTE_READONLY);
                        if (fileAttr == 0)
                        {
                            fileAttr = FILE_ATTRIBUTE_NORMAL;
                        }

                        // Set the new attributes and try to delete the file again, returning any failure
                        ::SetFileAttributesW(pathToDelete.get(), fileAttr);
                        RETURN_IF_WIN32_BOOL_FALSE(::DeleteFileW(pathToDelete.get()));
                    }
                }
            }

            if (!::FindNextFileW(findHandle.get(), &fd))
            {
                auto const err = ::GetLastError();
                if (err == ERROR_NO_MORE_FILES)
                {
                    break;
                }
                RETURN_WIN32(err);
            }
        }

        if (WI_IsFlagClear(options, RemoveDirectoryOptions::KeepRootDirectory))
        {
            if (deleteHandle != INVALID_HANDLE_VALUE)
            {
#if (NTDDI_VERSION >= NTDDI_WIN10_RS1)
                // DeleteFile and RemoveDirectory use POSIX delete, falling back to non-POSIX on most errors. Do the same here.
                FILE_DISPOSITION_INFO_EX fileInfoEx{};
                fileInfoEx.Flags = FILE_DISPOSITION_FLAG_DELETE | FILE_DISPOSITION_FLAG_POSIX_SEMANTICS;
                if (!SetFileInformationByHandle(deleteHandle, FileDispositionInfoEx, &fileInfoEx, sizeof(fileInfoEx)))
                {
                    auto const err = ::GetLastError();
                    // The real error we're looking for is STATUS_CANNOT_DELETE, but that's mapped to ERROR_ACCESS_DENIED.
                    if (err != ERROR_ACCESS_DENIED)
                    {
#endif
                        FILE_DISPOSITION_INFO fileInfo{};
                        fileInfo.DeleteFile = TRUE;
                        RETURN_IF_WIN32_BOOL_FALSE(SetFileInformationByHandle(deleteHandle, FileDispositionInfo, &fileInfo, sizeof(fileInfo)));
#if (NTDDI_VERSION >= NTDDI_WIN10_RS1)
                    }
                    else
                    {
                        RETURN_WIN32(err);
                    }
                }
#endif
            }
            else
            {
                RETURN_IF_WIN32_BOOL_FALSE(::RemoveDirectoryW(path.get()));
            }
        }
        return S_OK;
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    inline void RemoveDirectoryRecursive(PCWSTR path, RemoveDirectoryOptions options = RemoveDirectoryOptions::None)
    {
        THROW_IF_FAILED(RemoveDirectoryRecursiveNoThrow(path, options));
    }
#endif // WIL_ENABLE_EXCEPTIONS

    // Range based for that supports Win32 structures that use NextEntryOffset as the basis of traversing
    // a result buffer that contains data. This is used in the following FileIO calls:
    // FileStreamInfo, FILE_STREAM_INFO
    // FileIdBothDirectoryInfo, FILE_ID_BOTH_DIR_INFO
    // FileFullDirectoryInfo, FILE_FULL_DIR_INFO
    // FileIdExtdDirectoryInfo, FILE_ID_EXTD_DIR_INFO
    // ReadDirectoryChangesW, FILE_NOTIFY_INFORMATION

    template <typename T>
    struct next_entry_offset_iterator
    {
        // Fulfill std::iterator_traits requirements
        using difference_type = ptrdiff_t;
        using value_type = T;
        using pointer = const T*;
        using reference = const T&;
#ifdef _XUTILITY_
        using iterator_category = ::std::forward_iterator_tag;
#endif

        next_entry_offset_iterator(T *iterable = __nullptr) : current_(iterable) {}

        // range based for requires operator!=, operator++ and operator* to do its work
        // on the type returned from begin() and end(), provide those here.
        WI_NODISCARD bool operator!=(const next_entry_offset_iterator& other) const { return current_ != other.current_; }

        next_entry_offset_iterator& operator++()
        {
            current_ = (current_->NextEntryOffset != 0) ?
                reinterpret_cast<T *>(reinterpret_cast<unsigned char*>(current_) + current_->NextEntryOffset) :
                __nullptr;
            return *this;
        }

        next_entry_offset_iterator operator++(int)
        {
            auto copy = *this;
            ++(*this);
            return copy;
        }

        WI_NODISCARD reference operator*() const WI_NOEXCEPT { return *current_; }
        WI_NODISCARD pointer operator->() const WI_NOEXCEPT { return current_; }

        next_entry_offset_iterator<T> begin() { return *this; }
        next_entry_offset_iterator<T> end()   { return next_entry_offset_iterator<T>(); }

        T* current_;
    };

    template <typename T>
    next_entry_offset_iterator<T> create_next_entry_offset_iterator(T* p)
    {
        return next_entry_offset_iterator<T>(p);
    }

#pragma region Folder Watcher
    // Example use in exception based code:
    // auto watcher = wil::make_folder_watcher(folder.Path().c_str(), true, wil::allChangeEvents, []()
    //     {
    //         // respond
    //     });
    //
    // Example use in result code based code:
    // wil::unique_folder_watcher watcher;
    // THROW_IF_FAILED(watcher.create(folder, true, wil::allChangeEvents, []()
    //     {
    //         // respond
    //     }));

    enum class FolderChangeEvent : DWORD
    {
        ChangesLost = 0, // requies special handling, reset state as events were lost
        Added = FILE_ACTION_ADDED,
        Removed = FILE_ACTION_REMOVED,
        Modified = FILE_ACTION_MODIFIED,
        RenameOldName = FILE_ACTION_RENAMED_OLD_NAME,
        RenameNewName = FILE_ACTION_RENAMED_NEW_NAME,
    };

    enum class FolderChangeEvents : DWORD
    {
        None = 0,
        FileName = FILE_NOTIFY_CHANGE_FILE_NAME,
        DirectoryName = FILE_NOTIFY_CHANGE_DIR_NAME,
        Attributes = FILE_NOTIFY_CHANGE_ATTRIBUTES,
        FileSize = FILE_NOTIFY_CHANGE_SIZE,
        LastWriteTime = FILE_NOTIFY_CHANGE_LAST_WRITE,
        Security = FILE_NOTIFY_CHANGE_SECURITY,
        All = FILE_NOTIFY_CHANGE_FILE_NAME |
              FILE_NOTIFY_CHANGE_DIR_NAME |
              FILE_NOTIFY_CHANGE_ATTRIBUTES |
              FILE_NOTIFY_CHANGE_SIZE |
              FILE_NOTIFY_CHANGE_LAST_WRITE |
              FILE_NOTIFY_CHANGE_SECURITY
    };
    DEFINE_ENUM_FLAG_OPERATORS(FolderChangeEvents);

    /// @cond
    namespace details
    {
        struct folder_watcher_state
        {
            folder_watcher_state(wistd::function<void()> &&callback) : m_callback(wistd::move(callback))
            {
            }
            wistd::function<void()> m_callback;
            // Order is important, need to close the thread pool wait before the change handle.
            unique_hfind_change m_findChangeHandle;
            unique_threadpool_wait m_threadPoolWait;
        };

        inline void delete_folder_watcher_state(_In_opt_ folder_watcher_state *storage) { delete storage; }

        typedef resource_policy<folder_watcher_state *, decltype(&details::delete_folder_watcher_state),
            details::delete_folder_watcher_state, details::pointer_access_none> folder_watcher_state_resource_policy;
    }
    /// @endcond

    template <typename storage_t, typename err_policy = err_exception_policy>
    class folder_watcher_t : public storage_t
    {
    public:
        // forward all base class constructors...
        template <typename... args_t>
        explicit folder_watcher_t(args_t&&... args) WI_NOEXCEPT : storage_t(wistd::forward<args_t>(args)...) {}

        // HRESULT or void error handling...
        typedef typename err_policy::result result;

        // Exception-based constructors
        folder_watcher_t(PCWSTR folderToWatch, bool isRecursive, FolderChangeEvents filter, wistd::function<void()> &&callback)
        {
            static_assert(wistd::is_same<void, result>::value, "this constructor requires exceptions; use the create method");
            create(folderToWatch, isRecursive, filter, wistd::move(callback));
        }

        result create(PCWSTR folderToWatch, bool isRecursive, FolderChangeEvents filter, wistd::function<void()> &&callback)
        {
            return err_policy::HResult(create_common(folderToWatch, isRecursive, filter, wistd::move(callback)));
        }
    private:
        // Factored into a standalone function to support Clang which does not support conversion of stateless lambdas
        // to __stdcall
        static void __stdcall callback(PTP_CALLBACK_INSTANCE /*Instance*/, void *context, TP_WAIT *pThreadPoolWait, TP_WAIT_RESULT /*result*/)
        {
            auto watcherState = static_cast<details::folder_watcher_state *>(context);
            watcherState->m_callback();

            // Rearm the wait. Should not fail with valid parameters.
            FindNextChangeNotification(watcherState->m_findChangeHandle.get());
            SetThreadpoolWait(pThreadPoolWait, watcherState->m_findChangeHandle.get(), __nullptr);
        }

        // This function exists to avoid template expansion of this code based on err_policy.
        HRESULT create_common(PCWSTR folderToWatch, bool isRecursive, FolderChangeEvents filter, wistd::function<void()> &&callback)
        {
            wistd::unique_ptr<details::folder_watcher_state> watcherState(new(std::nothrow) details::folder_watcher_state(wistd::move(callback)));
            RETURN_IF_NULL_ALLOC(watcherState);

            watcherState->m_findChangeHandle.reset(FindFirstChangeNotificationW(folderToWatch, isRecursive, static_cast<DWORD>(filter)));
            RETURN_LAST_ERROR_IF(!watcherState->m_findChangeHandle);

            watcherState->m_threadPoolWait.reset(CreateThreadpoolWait(&folder_watcher_t::callback, watcherState.get(), __nullptr));
            RETURN_LAST_ERROR_IF(!watcherState->m_threadPoolWait);
            this->reset(watcherState.release()); // no more failures after this, pass ownership
            SetThreadpoolWait(this->get()->m_threadPoolWait.get(), this->get()->m_findChangeHandle.get(), __nullptr);
            return S_OK;
        }
    };

    typedef unique_any_t<folder_watcher_t<details::unique_storage<details::folder_watcher_state_resource_policy>, err_returncode_policy>> unique_folder_watcher_nothrow;

    inline unique_folder_watcher_nothrow make_folder_watcher_nothrow(PCWSTR folderToWatch, bool isRecursive, FolderChangeEvents filter, wistd::function<void()> &&callback) WI_NOEXCEPT
    {
        unique_folder_watcher_nothrow watcher;
        watcher.create(folderToWatch, isRecursive, filter, wistd::move(callback));
        return watcher; // caller must test for success using if (watcher)
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    typedef unique_any_t<folder_watcher_t<details::unique_storage<details::folder_watcher_state_resource_policy>, err_exception_policy>> unique_folder_watcher;

    inline unique_folder_watcher make_folder_watcher(PCWSTR folderToWatch, bool isRecursive, FolderChangeEvents filter, wistd::function<void()> &&callback)
    {
        return unique_folder_watcher(folderToWatch, isRecursive, filter, wistd::move(callback));
    }
#endif // WIL_ENABLE_EXCEPTIONS

#pragma endregion

#pragma region Folder Reader

    // Example use for throwing:
    // auto reader = wil::make_folder_change_reader(folder.Path().c_str(), true, wil::FolderChangeEvents::All,
    //     [](wil::FolderChangeEvent event, PCWSTR fileName)
    //     {
    //          switch (event)
    //          {
    //          case wil::FolderChangeEvent::ChangesLost: break;
    //          case wil::FolderChangeEvent::Added:    break;
    //          case wil::FolderChangeEvent::Removed:  break;
    //          case wil::FolderChangeEvent::Modified: break;
    //          case wil::FolderChangeEvent::RenamedOldName: break;
    //          case wil::FolderChangeEvent::RenamedNewName: break;
    //      });
    //
    // Example use for non throwing:
    // wil::unique_folder_change_reader_nothrow reader;
    // THROW_IF_FAILED(reader.create(folder, true, wil::FolderChangeEvents::All,
    //     [](wil::FolderChangeEvent event, PCWSTR fileName)
    //     {
    //         // handle changes
    //     }));
    //

    // @cond
    namespace details
    {
        struct folder_change_reader_state
        {
            folder_change_reader_state(bool isRecursive, FolderChangeEvents filter, wistd::function<void(FolderChangeEvent, PCWSTR)> &&callback)
                : m_callback(wistd::move(callback)), m_isRecursive(isRecursive), m_filter(filter)
            {
            }

            ~folder_change_reader_state()
            {
                if (m_tpIo != __nullptr)
                {
                    TP_IO *tpIo = m_tpIo;

                    // Indicate to the callback function that this object is being torn
                    // down.

                    {
                        auto autoLock = m_cancelLock.lock_exclusive();
                        m_tpIo = __nullptr;
                    }

                    // Cancel IO to terminate the file system monitoring operation.

                    if (m_folderHandle)
                    {
                        CancelIoEx(m_folderHandle.get(), &m_overlapped);

                        DWORD bytesTransferredIgnored = 0;
                        GetOverlappedResult(m_folderHandle.get(), &m_overlapped, &bytesTransferredIgnored, TRUE);
                    }

                    // Wait for callbacks to complete.
                    //
                    // N.B. This is a blocking call and must not be made within a
                    //      callback or within a lock which is taken inside the
                    //      callback.

                    WaitForThreadpoolIoCallbacks(tpIo, TRUE);
                    CloseThreadpoolIo(tpIo);
                }
            }

            HRESULT StartIo()
            {
                // Unfortunately we have to handle ref-counting of IOs on behalf of the
                // thread pool.
                StartThreadpoolIo(m_tpIo);
                HRESULT hr = ReadDirectoryChangesW(m_folderHandle.get(), m_readBuffer, sizeof(m_readBuffer),
                    m_isRecursive, static_cast<DWORD>(m_filter), __nullptr, &m_overlapped, __nullptr) ?
                        S_OK : HRESULT_FROM_WIN32(::GetLastError());
                if (FAILED(hr))
                {
                    // This operation does not have the usual semantic of returning
                    // ERROR_IO_PENDING.
                    // WI_ASSERT(hr != HRESULT_FROM_WIN32(ERROR_IO_PENDING));

                    // If the operation failed for whatever reason, ensure the TP
                    // ref counts are accurate.

                    CancelThreadpoolIo(m_tpIo);
                }
                return hr;
            }

            // void (wil::FolderChangeEvent event, PCWSTR fileName)
            wistd::function<void(FolderChangeEvent, PCWSTR)> m_callback;
            unique_handle m_folderHandle;
            BOOL m_isRecursive = FALSE;
            FolderChangeEvents m_filter = FolderChangeEvents::None;
            OVERLAPPED m_overlapped{};
            TP_IO *m_tpIo = __nullptr;
            srwlock m_cancelLock;
            unsigned char m_readBuffer[4096]{}; // Consider alternative buffer sizes. With 512 byte buffer i was not able to observe overflow.
        };

        inline void delete_folder_change_reader_state(_In_opt_ folder_change_reader_state *storage) { delete storage; }

        typedef resource_policy<folder_change_reader_state *, decltype(&details::delete_folder_change_reader_state),
            details::delete_folder_change_reader_state, details::pointer_access_none> folder_change_reader_state_resource_policy;
    }
    /// @endcond

    template <typename storage_t, typename err_policy = err_exception_policy>
    class folder_change_reader_t : public storage_t
    {
    public:
        // forward all base class constructors...
        template <typename... args_t>
        explicit folder_change_reader_t(args_t&&... args) WI_NOEXCEPT : storage_t(wistd::forward<args_t>(args)...) {}

        // HRESULT or void error handling...
        typedef typename err_policy::result result;

        // Exception-based constructors
        folder_change_reader_t(PCWSTR folderToWatch, bool isRecursive, FolderChangeEvents filter, wistd::function<void(FolderChangeEvent, PCWSTR)> &&callback)
        {
            static_assert(wistd::is_same<void, result>::value, "this constructor requires exceptions; use the create method");
            create(folderToWatch, isRecursive, filter, wistd::move(callback));
        }

        result create(PCWSTR folderToWatch, bool isRecursive, FolderChangeEvents filter, wistd::function<void(FolderChangeEvent, PCWSTR)> &&callback)
        {
            return err_policy::HResult(create_common(folderToWatch, isRecursive, filter, wistd::move(callback)));
        }

        wil::unique_hfile& folder_handle() { return this->get()->m_folderHandle; }

    private:
        // Factored into a standalone function to support Clang which does not support conversion of stateless lambdas
        // to __stdcall
        static void __stdcall callback(PTP_CALLBACK_INSTANCE /* Instance */, void *context, void * /*overlapped*/,
            ULONG result, ULONG_PTR /* BytesTransferred */, TP_IO * /* Io */)
        {
            auto readerState = static_cast<details::folder_change_reader_state *>(context);
            // WI_ASSERT(overlapped == &readerState->m_overlapped);

            if (result == ERROR_SUCCESS)
            {
                for (auto const& info : create_next_entry_offset_iterator(reinterpret_cast<FILE_NOTIFY_INFORMATION *>(readerState->m_readBuffer)))
                {
                    wchar_t realtiveFileName[MAX_PATH];
                    StringCchCopyNW(realtiveFileName, ARRAYSIZE(realtiveFileName), info.FileName, info.FileNameLength / sizeof(info.FileName[0]));

                    readerState->m_callback(static_cast<FolderChangeEvent>(info.Action), realtiveFileName);
                }
            }
            else if (result == ERROR_NOTIFY_ENUM_DIR)
            {
                readerState->m_callback(FolderChangeEvent::ChangesLost, __nullptr);
            }
            else
            {
                // No need to requeue
                return;
            }

            // If the lock is held non-shared or the TP IO is nullptr, this
            // structure is being torn down. Otherwise, monitor for further
            // changes.
            auto autoLock = readerState->m_cancelLock.try_lock_shared();
            if (autoLock && readerState->m_tpIo)
            {
                readerState->StartIo(); // ignoring failure here
            }
        }

        // This function exists to avoid template expansion of this code based on err_policy.
        HRESULT create_common(PCWSTR folderToWatch, bool isRecursive, FolderChangeEvents filter, wistd::function<void(FolderChangeEvent, PCWSTR)> &&callback)
        {
            wistd::unique_ptr<details::folder_change_reader_state> readerState(new(std::nothrow) details::folder_change_reader_state(
                isRecursive, filter, wistd::move(callback)));
            RETURN_IF_NULL_ALLOC(readerState);

            readerState->m_folderHandle.reset(CreateFileW(folderToWatch,
                FILE_LIST_DIRECTORY, FILE_SHARE_READ | FILE_SHARE_DELETE | FILE_SHARE_WRITE,
                __nullptr, OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS | FILE_FLAG_OVERLAPPED, __nullptr));
            RETURN_LAST_ERROR_IF(!readerState->m_folderHandle);

            readerState->m_tpIo = CreateThreadpoolIo(readerState->m_folderHandle.get(), &folder_change_reader_t::callback, readerState.get(), __nullptr);
            RETURN_LAST_ERROR_IF_NULL(readerState->m_tpIo);
            RETURN_IF_FAILED(readerState->StartIo());
            this->reset(readerState.release());
            return S_OK;
        }
    };

    typedef unique_any_t<folder_change_reader_t<details::unique_storage<details::folder_change_reader_state_resource_policy>, err_returncode_policy>> unique_folder_change_reader_nothrow;

    inline unique_folder_change_reader_nothrow make_folder_change_reader_nothrow(PCWSTR folderToWatch, bool isRecursive, FolderChangeEvents filter,
        wistd::function<void(FolderChangeEvent, PCWSTR)> &&callback) WI_NOEXCEPT
    {
        unique_folder_change_reader_nothrow watcher;
        watcher.create(folderToWatch, isRecursive, filter, wistd::move(callback));
        return watcher; // caller must test for success using if (watcher)
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    typedef unique_any_t<folder_change_reader_t<details::unique_storage<details::folder_change_reader_state_resource_policy>, err_exception_policy>> unique_folder_change_reader;

    inline unique_folder_change_reader make_folder_change_reader(PCWSTR folderToWatch, bool isRecursive, FolderChangeEvents filter,
        wistd::function<void(FolderChangeEvent, PCWSTR)> &&callback)
    {
        return unique_folder_change_reader(folderToWatch, isRecursive, filter, wistd::move(callback));
    }
#endif // WIL_ENABLE_EXCEPTIONS
#pragma endregion

    //! Dos and VolumeGuid paths are always extended length paths with the \\?\ prefix.
    enum class VolumePrefix
    {
        Dos = VOLUME_NAME_DOS,          // Extended Dos Device path form, e.g. \\?\C:\Users\Chris\AppData\Local\Temp\wil8C31.tmp
        VolumeGuid = VOLUME_NAME_GUID,  // \\?\Volume{588fb606-b95b-4eae-b3cb-1e49861aaf18}\Users\Chris\AppData\Local\Temp\wil8C31.tmp
        // The following are special paths which can't be used with Win32 APIs, but are useful in other scenarios.
        None = VOLUME_NAME_NONE,        // Path without the volume root, e.g. \Users\Chris\AppData\Local\Temp\wil8C31.tmp
        NtObjectName = VOLUME_NAME_NT,  // Unique name used by Object Manager, e.g. \Device\HarddiskVolume4\Users\Chris\AppData\Local\Temp\wil8C31.tmp
    };
    enum class PathOptions
    {
        Normalized = FILE_NAME_NORMALIZED,
        Opened = FILE_NAME_OPENED,
    };
    DEFINE_ENUM_FLAG_OPERATORS(PathOptions);

    /**  A strongly typed version of the Win32 API GetFinalPathNameByHandleW.
    Get the full path name in different forms
    Use this instead + VolumePrefix::None instead of GetFileInformationByHandleEx(FileNameInfo) to
    get that path form. */
    template <typename string_type, size_t stackBufferLength = 256>
    HRESULT GetFinalPathNameByHandleW(HANDLE fileHandle, string_type& path,
        wil::VolumePrefix volumePrefix = wil::VolumePrefix::Dos, wil::PathOptions options = wil::PathOptions::Normalized)
    {
        return AdaptFixedSizeToAllocatedResult<string_type, stackBufferLength>(path,
            [&](_Out_writes_(valueLength) PWSTR value, size_t valueLength, _Out_ size_t* valueLengthNeededWithNull) -> HRESULT
        {
            *valueLengthNeededWithNull = ::GetFinalPathNameByHandleW(fileHandle, value, static_cast<DWORD>(valueLength),
                static_cast<DWORD>(volumePrefix) | static_cast<DWORD>(options));
            RETURN_LAST_ERROR_IF(*valueLengthNeededWithNull == 0);
            WI_ASSERT((*value != L'\0') == (*valueLengthNeededWithNull < valueLength));
            if (*valueLengthNeededWithNull < valueLength)
            {
                (*valueLengthNeededWithNull)++; // it fit, account for the null
            }
            return S_OK;
        });
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    /** A strongly typed version of the Win32 API GetFinalPathNameByHandleW.
    Get the full path name in different forms. Use this + VolumePrefix::None
    instead of GetFileInformationByHandleEx(FileNameInfo) to get that path form. */
    template <typename string_type = wil::unique_cotaskmem_string, size_t stackBufferLength = 256>
    string_type GetFinalPathNameByHandleW(HANDLE fileHandle,
        wil::VolumePrefix volumePrefix = wil::VolumePrefix::Dos, wil::PathOptions options = wil::PathOptions::Normalized)
    {
        string_type result{};
        THROW_IF_FAILED((GetFinalPathNameByHandleW<string_type, stackBufferLength>(fileHandle, result, volumePrefix, options)));
        return result;
    }
#endif

    //! A strongly typed version of the Win32 API of GetCurrentDirectoryW.
    //! Return a path in an allocated buffer for handling long paths.
    template <typename string_type, size_t stackBufferLength = 256>
    HRESULT GetCurrentDirectoryW(string_type& path)
    {
        return AdaptFixedSizeToAllocatedResult<string_type, stackBufferLength>(path,
            [&](_Out_writes_(valueLength) PWSTR value, size_t valueLength, _Out_ size_t* valueLengthNeededWithNull) -> HRESULT
        {
            *valueLengthNeededWithNull = ::GetCurrentDirectoryW(static_cast<DWORD>(valueLength), value);
            RETURN_LAST_ERROR_IF(*valueLengthNeededWithNull == 0);
            WI_ASSERT((*value != L'\0') == (*valueLengthNeededWithNull < valueLength));
            if (*valueLengthNeededWithNull < valueLength)
            {
                (*valueLengthNeededWithNull)++; // it fit, account for the null
            }
            return S_OK;
        });
    }

#ifdef WIL_ENABLE_EXCEPTIONS
    //! A strongly typed version of the Win32 API of GetCurrentDirectoryW.
    //! Return a path in an allocated buffer for handling long paths.
    template <typename string_type = wil::unique_cotaskmem_string, size_t stackBufferLength = 256>
    string_type GetCurrentDirectoryW()
    {
        string_type result{};
        THROW_IF_FAILED((GetCurrentDirectoryW<string_type, stackBufferLength>(result)));
        return result;
    }
#endif

    // TODO: add support for these and other similar APIs.
    // GetShortPathNameW()
    // GetLongPathNameW()
    // GetTempDirectory()

    /// @cond
    namespace details
    {
        template <FILE_INFO_BY_HANDLE_CLASS infoClass> struct MapInfoClassToInfoStruct; // failure to map is a usage error caught by the compiler
#define MAP_INFOCLASS_TO_STRUCT(InfoClass, InfoStruct, IsFixed, Extra) \
        template <> struct MapInfoClassToInfoStruct<InfoClass> \
        { \
            typedef InfoStruct type; \
            static bool const isFixed = IsFixed; \
            static size_t const extraSize = Extra; \
        };

        MAP_INFOCLASS_TO_STRUCT(FileBasicInfo, FILE_BASIC_INFO, true, 0);
        MAP_INFOCLASS_TO_STRUCT(FileStandardInfo, FILE_STANDARD_INFO, true, 0);
        MAP_INFOCLASS_TO_STRUCT(FileNameInfo, FILE_NAME_INFO, false, 32);
        MAP_INFOCLASS_TO_STRUCT(FileRenameInfo, FILE_RENAME_INFO, false, 32);
        MAP_INFOCLASS_TO_STRUCT(FileDispositionInfo, FILE_DISPOSITION_INFO, true, 0);
        MAP_INFOCLASS_TO_STRUCT(FileAllocationInfo, FILE_ALLOCATION_INFO, true, 0);
        MAP_INFOCLASS_TO_STRUCT(FileEndOfFileInfo, FILE_END_OF_FILE_INFO, true, 0);
        MAP_INFOCLASS_TO_STRUCT(FileStreamInfo, FILE_STREAM_INFO, false, 32);
        MAP_INFOCLASS_TO_STRUCT(FileCompressionInfo, FILE_COMPRESSION_INFO, true, 0);
        MAP_INFOCLASS_TO_STRUCT(FileAttributeTagInfo, FILE_ATTRIBUTE_TAG_INFO, true, 0);
        MAP_INFOCLASS_TO_STRUCT(FileIdBothDirectoryInfo, FILE_ID_BOTH_DIR_INFO, false, 4096);
        MAP_INFOCLASS_TO_STRUCT(FileIdBothDirectoryRestartInfo, FILE_ID_BOTH_DIR_INFO, true, 0);
        MAP_INFOCLASS_TO_STRUCT(FileIoPriorityHintInfo, FILE_IO_PRIORITY_HINT_INFO, true, 0);
        MAP_INFOCLASS_TO_STRUCT(FileRemoteProtocolInfo, FILE_REMOTE_PROTOCOL_INFO, true, 0);
        MAP_INFOCLASS_TO_STRUCT(FileFullDirectoryInfo, FILE_FULL_DIR_INFO, false, 4096);
        MAP_INFOCLASS_TO_STRUCT(FileFullDirectoryRestartInfo, FILE_FULL_DIR_INFO, true, 0);
#if (_WIN32_WINNT >= _WIN32_WINNT_WIN8)
        MAP_INFOCLASS_TO_STRUCT(FileStorageInfo, FILE_STORAGE_INFO, true, 0);
        MAP_INFOCLASS_TO_STRUCT(FileAlignmentInfo, FILE_ALIGNMENT_INFO, true, 0);
        MAP_INFOCLASS_TO_STRUCT(FileIdInfo, FILE_ID_INFO, true, 0);
        MAP_INFOCLASS_TO_STRUCT(FileIdExtdDirectoryInfo, FILE_ID_EXTD_DIR_INFO, false, 4096);
        MAP_INFOCLASS_TO_STRUCT(FileIdExtdDirectoryRestartInfo, FILE_ID_EXTD_DIR_INFO, true, 0);
#endif

        // Type unsafe version used in the implementation to avoid template bloat.
        inline HRESULT GetFileInfo(HANDLE fileHandle, FILE_INFO_BY_HANDLE_CLASS infoClass, size_t allocationSize,
            _Outptr_result_maybenull_ void **result)
        {
            *result = nullptr;

            wistd::unique_ptr<char[]> resultHolder(new(std::nothrow) char[allocationSize]);
            RETURN_IF_NULL_ALLOC(resultHolder);

            for (;;)
            {
                if (GetFileInformationByHandleEx(fileHandle, infoClass, resultHolder.get(), static_cast<DWORD>(allocationSize)))
                {
                    *result = resultHolder.release();
                    break;
                }
                else
                {
                    DWORD const lastError = ::GetLastError();
                    if (lastError == ERROR_MORE_DATA)
                    {
                        allocationSize *= 2;
                        resultHolder.reset(new(std::nothrow) char[allocationSize]);
                        RETURN_IF_NULL_ALLOC(resultHolder);
                    }
                    else if (lastError == ERROR_NO_MORE_FILES) // for folder enumeration cases
                    {
                        break;
                    }
                    else if (lastError == ERROR_INVALID_PARAMETER) // operation not supported by file system
                    {
                        return HRESULT_FROM_WIN32(lastError);
                    }
                    else
                    {
                        RETURN_WIN32(lastError);
                    }
                }
            }
            return S_OK;
        }
    }
    /// @endcond

    /** Get file information for a variable sized structure, returns an HRESULT.
    ~~~
    wistd::unique_ptr<FILE_NAME_INFO> fileNameInfo;
    RETURN_IF_FAILED(GetFileInfoNoThrow<FileNameInfo>(fileHandle, fileNameInfo));
    ~~~
    */
    template <FILE_INFO_BY_HANDLE_CLASS infoClass, typename wistd::enable_if<!details::MapInfoClassToInfoStruct<infoClass>::isFixed, int>::type = 0>
    HRESULT GetFileInfoNoThrow(HANDLE fileHandle, wistd::unique_ptr<typename details::MapInfoClassToInfoStruct<infoClass>::type> &result) WI_NOEXCEPT
    {
        void *rawResult;
        HRESULT hr = details::GetFileInfo(fileHandle, infoClass,
            sizeof(typename details::MapInfoClassToInfoStruct<infoClass>::type) + details::MapInfoClassToInfoStruct<infoClass>::extraSize,
            &rawResult);
        result.reset(static_cast<typename details::MapInfoClassToInfoStruct<infoClass>::type*>(rawResult));
        RETURN_HR_IF_EXPECTED(hr, hr == E_INVALIDARG); // operation not supported by file system
        RETURN_IF_FAILED(hr);
        return S_OK;
    }

    /** Get file information for a fixed sized structure, returns an HRESULT.
    ~~~
    FILE_BASIC_INFO fileBasicInfo;
    RETURN_IF_FAILED(GetFileInfoNoThrow<FileBasicInfo>(fileHandle, &fileBasicInfo));
    ~~~
    */
    template <FILE_INFO_BY_HANDLE_CLASS infoClass, typename wistd::enable_if<details::MapInfoClassToInfoStruct<infoClass>::isFixed, int>::type = 0>
    HRESULT GetFileInfoNoThrow(HANDLE fileHandle, _Out_ typename details::MapInfoClassToInfoStruct<infoClass>::type *result) WI_NOEXCEPT
    {
        const HRESULT hr = GetFileInformationByHandleEx(fileHandle, infoClass, result, sizeof(*result)) ?
            S_OK : HRESULT_FROM_WIN32(::GetLastError());
        RETURN_HR_IF_EXPECTED(hr, hr == E_INVALIDARG); // operation not supported by file system
        RETURN_IF_FAILED(hr);
        return S_OK;
    }

    // Verifies that the given file path is not a hard or a soft link. If the file is present at the path, returns
    // a handle to it without delete permissions to block an attacker from swapping the file.
    inline HRESULT CreateFileAndEnsureNotLinked(PCWSTR path, wil::unique_hfile& fileHandle)
    {
        // Open handles to the original path and to the final path and compare each file's information
        // to verify they are the same file. If they are different, the file is a soft link.
        fileHandle.reset(CreateFileW(path, GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, nullptr, OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS | FILE_FLAG_OPEN_REPARSE_POINT, nullptr));
        RETURN_LAST_ERROR_IF(!fileHandle);
        BY_HANDLE_FILE_INFORMATION fileInfo;
        RETURN_IF_WIN32_BOOL_FALSE(GetFileInformationByHandle(fileHandle.get(), &fileInfo));

        // Open a handle without the reparse point flag to get the final path in case it is a soft link.
        wil::unique_hfile finalPathHandle(CreateFileW(path, 0, 0, nullptr, OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS, nullptr));
        RETURN_LAST_ERROR_IF(!finalPathHandle);
        BY_HANDLE_FILE_INFORMATION finalFileInfo;
        RETURN_IF_WIN32_BOOL_FALSE(GetFileInformationByHandle(finalPathHandle.get(), &finalFileInfo));
        finalPathHandle.reset();

        // The low and high indices and volume serial number uniquely identify a file. These must match if they are the same file.
        const bool isSoftLink =
            ((fileInfo.nFileIndexLow != finalFileInfo.nFileIndexLow) ||
             (fileInfo.nFileIndexHigh != finalFileInfo.nFileIndexHigh) ||
             (fileInfo.dwVolumeSerialNumber != finalFileInfo.dwVolumeSerialNumber));

        // Return failure if it is a soft link or a hard link (number of links greater than 1).
        RETURN_HR_IF(HRESULT_FROM_WIN32(ERROR_BAD_PATHNAME), (isSoftLink || fileInfo.nNumberOfLinks > 1));

        return S_OK;
    }

#ifdef _CPPUNWIND
    /** Get file information for a fixed sized structure, throws on failure.
    ~~~
    auto fileBasicInfo = GetFileInfo<FileBasicInfo>(fileHandle);
    ~~~
    */
    template <FILE_INFO_BY_HANDLE_CLASS infoClass, typename wistd::enable_if<details::MapInfoClassToInfoStruct<infoClass>::isFixed, int>::type = 0>
    typename details::MapInfoClassToInfoStruct<infoClass>::type GetFileInfo(HANDLE fileHandle)
    {
        typename details::MapInfoClassToInfoStruct<infoClass>::type result{};
        THROW_IF_FAILED(GetFileInfoNoThrow<infoClass>(fileHandle, &result));
        return result;
    }

    /** Get file information for a variable sized structure, throws on failure.
    ~~~
    auto fileBasicInfo = GetFileInfo<FileNameInfo>(fileHandle);
    ~~~
    */
    template <FILE_INFO_BY_HANDLE_CLASS infoClass, typename wistd::enable_if<!details::MapInfoClassToInfoStruct<infoClass>::isFixed, int>::type = 0>
    wistd::unique_ptr<typename details::MapInfoClassToInfoStruct<infoClass>::type> GetFileInfo(HANDLE fileHandle)
    {
        wistd::unique_ptr<typename details::MapInfoClassToInfoStruct<infoClass>::type> result;
        THROW_IF_FAILED(GetFileInfoNoThrow<infoClass>(fileHandle, result));
        return result;
    }
#endif // _CPPUNWIND
#endif // WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP) && (_WIN32_WINNT >= _WIN32_WINNT_WIN7)
}

#endif // __WIL_FILESYSTEM_INCLUDED
