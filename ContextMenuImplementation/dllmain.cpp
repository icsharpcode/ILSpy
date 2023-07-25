// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <wrl/module.h>
#include <wrl/implements.h>
#include <shobjidl_core.h>
#include "../ContextMenuImplementation/wil/resource.h"
#include <Shellapi.h>
#include <Shlwapi.h>
#include <Strsafe.h>


using namespace Microsoft::WRL;

HMODULE g_hModule = nullptr;



BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD ul_reason_for_call,
    LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        g_hModule = hModule;
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}


class ILSpyCommand : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IExplorerCommand, IObjectWithSite>
{
public:
    // IExplorerCommand methods
    IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name)
    {
        *name = nullptr;
        auto title = wil::make_cotaskmem_string_nothrow(L"Open In ILSpy");
        RETURN_IF_NULL_ALLOC(title);
        *name = title.release();
        return S_OK;
    }

    IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* iconPath)
    {
        *iconPath = nullptr;
        PWSTR itemPath = nullptr;

        if (items)
        {
            DWORD count;
            RETURN_IF_FAILED(items->GetCount(&count));

            if (count > 0)
            {
                ComPtr<IShellItem> item;
                RETURN_IF_FAILED(items->GetItemAt(0, &item));

                RETURN_IF_FAILED(item->GetDisplayName(SIGDN_FILESYSPATH, &itemPath));
                wil::unique_cotaskmem_string itemPathCleanup(itemPath);

                WCHAR modulePath[MAX_PATH];
                if (GetModuleFileNameW(g_hModule, modulePath, ARRAYSIZE(modulePath)))
                {
                    PathRemoveFileSpecW(modulePath);
                    StringCchCatW(modulePath, ARRAYSIZE(modulePath), L"\\ILSpy.ico");

                    auto iconPathStr = wil::make_cotaskmem_string_nothrow(modulePath);
                    if (iconPathStr)
                    {
                        *iconPath = iconPathStr.release();
                    }
                }
            }
        }



        return *iconPath ? S_OK : E_FAIL;
    }




    IFACEMETHODIMP GetToolTip(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* infoTip) { *infoTip = nullptr; return E_NOTIMPL; }
    IFACEMETHODIMP GetCanonicalName(_Out_ GUID* guidCommandName) { *guidCommandName = GUID_NULL;  return S_OK; }
    IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState)
    {
        *cmdState = ECS_ENABLED;
        return S_OK;
    }

    IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try
    {
        if (selection)
        {
            DWORD count;
            RETURN_IF_FAILED(selection->GetCount(&count));
            if (count > 0)
            {
                ComPtr<IShellItem> item;
                RETURN_IF_FAILED(selection->GetItemAt(0, &item));

                PWSTR filePath;
                RETURN_IF_FAILED(item->GetDisplayName(SIGDN_FILESYSPATH, &filePath));
                wil::unique_cotaskmem_string filePathCleanup(filePath);

                // Check if the file has the correct extension
                
                {
                    SHELLEXECUTEINFO sei = { 0 };
                    sei.cbSize = sizeof(SHELLEXECUTEINFO);
                    sei.fMask = SEE_MASK_DEFAULT;
                    sei.lpVerb = L"open";
                    sei.lpFile = L"ILSpy.exe"; // Add location of where ILSpy.exe is located 
                    sei.lpParameters = filePath;
                    sei.nShow = SW_SHOWNORMAL;

                    if (!ShellExecuteEx(&sei))
                    {
                        return HRESULT_FROM_WIN32(GetLastError());
                    }
                }
              
            }
        }

        return S_OK;
    }
    CATCH_RETURN();



    IFACEMETHODIMP GetFlags(EXPCMDFLAGS* flags)
    {
        *flags = ECF_DEFAULT;

        // Retrieve the file path of the selected item (if available)
        wil::unique_cotaskmem_string filePath;
        if (m_site)
        {
            ComPtr<IUnknown> site;
            if (SUCCEEDED(GetSite(IID_PPV_ARGS(&site))))
            {
                ComPtr<IShellItemArray> selection;
                if (SUCCEEDED(site.As(&selection)))
                {
                    DWORD count;
                    if (SUCCEEDED(selection->GetCount(&count)) && count > 0)
                    {
                        ComPtr<IShellItem> item;
                        if (SUCCEEDED(selection->GetItemAt(0, &item)))
                        {
                            PWSTR path = nullptr;
                            if (SUCCEEDED(item->GetDisplayName(SIGDN_FILESYSPATH, &path)))
                            {
                                filePath.reset(path);
                            }
                        }
                    }
                }
            }
        }

        // Check if the file has the correct extension
        if (filePath && (PathMatchSpecW(filePath.get(), L"*.exe") || PathMatchSpecW(filePath.get(), L"*.dll")))
        {
            return S_OK;
        }

        // File doesn't have the correct extension, so hide the context menu
        *flags = ECF_DEFAULT | ECF_HASSUBCOMMANDS;
        return S_OK;
    }






    IFACEMETHODIMP EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands) { *enumCommands = nullptr; return E_NOTIMPL; }

    // IObjectWithSite methods
    IFACEMETHODIMP SetSite(_In_ IUnknown* site) noexcept { m_site = site; return S_OK; }
    IFACEMETHODIMP GetSite(_In_ REFIID riid, _COM_Outptr_ void** site) noexcept { return m_site.CopyTo(riid, site); }

protected:
    ComPtr<IUnknown> m_site;
};


class __declspec(uuid("7A1E471F-0D43-4122-B1C4-D1AACE76CE9B")) ILSpyCommand1 final : public ILSpyCommand
{
//public:
    //const wchar_t* Title() override { return L"HelloWorld Command1"; }
    //const EXPCMDSTATE State(_In_opt_ IShellItemArray* selection) override { return ECS_DISABLED; }
};

CoCreatableClass(ILSpyCommand1)


STDAPI DllGetActivationFactory(_In_ HSTRING activatableClassId, _COM_Outptr_ IActivationFactory** factory)
{
    return Module<ModuleType::InProc>::GetModule().GetActivationFactory(activatableClassId, factory);
}

STDAPI DllCanUnloadNow()
{
    return Module<InProc>::GetModule().GetObjectCount() == 0 ? S_OK : S_FALSE;
}

STDAPI DllGetClassObject(_In_ REFCLSID rclsid, _In_ REFIID riid, _COM_Outptr_ void** instance)
{
    return Module<InProc>::GetModule().GetClassObject(rclsid, riid, instance);
}
